using LinqKit;
using Microsoft.EntityFrameworkCore;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using bbxBE.Infrastructure.Persistence.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.BLL;
using System;
using AutoMapper;
using bbxBE.Application.Queries.qOffer;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Consts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{

    /*
      
 {
"customerID": 5,
  "offerIssueDate": "2022-05-20",
  "offerVaidityDate": "2022-05-20",
  "notice": "első ajánlat",
  "offerLines": [
    {
     "lineNumber": 1,
      "productCode": "VEG-2973",
      "lineDescription": "Boyler 600W fűtőbetét",
     "vatRateCode": "27%",
      "discount": 10,
      "showDiscount": true,
       "unitPrice": 10,
      "unitVat": 2.7,
      "unitGross": 12.7
    }
  ]
}
     */
    public class OfferRepositoryAsync : GenericRepositoryAsync<Offer>, IOfferRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Offer> _Offers;
        private readonly DbSet<OfferLine> _OfferLines;

        private readonly DbSet<Customer> _customers;
        private readonly DbSet<VatRate> _vatRates;

        private IDataShapeHelper<Offer> _dataShaperOffer;
        private IDataShapeHelper<GetOfferViewModel> _dataShaperGetOfferViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public OfferRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Offer> dataShaperOffer,
            IDataShapeHelper<GetOfferViewModel> dataShaperGetOfferViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
    
            _Offers = dbContext.Set<Offer>();
            _OfferLines = dbContext.Set<OfferLine>();
            _customers = dbContext.Set<Customer>();
            _vatRates = dbContext.Set<VatRate>();

            _dataShaperOffer = dataShaperOffer;
            _dataShaperGetOfferViewModel = dataShaperGetOfferViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<Offer> AddOfferAsync(Offer p_Offer)
        {

            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    await _Offers.AddAsync(p_Offer);
                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }

            return p_Offer;
        }

        public async Task<Offer> UpdateOfferAsync(Offer p_Offer, List<OfferLine> p_OfferLines)
        {
            throw new NotImplementedException("UpdateOfferAsync");
        }

        public async Task<Offer> DeleteOfferAsync(long ID)
        {

            Offer offer = null;
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                offer = await _Offers.Where(x => x.ID == ID).FirstOrDefaultAsync();

                if (offer != null)
                {

                    throw new NotImplementedException("DeleteOfferAsync");
                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_ORIGINNOTFOUND, ID));
                }
            }
            return offer;
        }


        public async Task<Entity> GetOfferAsync(GetOffer requestParameter)
        {


            var ID = requestParameter.ID;

            var item = await _Offers.AsNoTracking()
              .Include(c => c.Customer).AsNoTracking()
              .Include(i => i.OfferLines).ThenInclude(t => t.VatRate).AsNoTracking()
              .Where(x => x.ID == ID).FirstOrDefaultAsync();

            if( item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_OFFERNOTFOUND, ID));
            }

            var itemModel = _mapper.Map<Offer, GetOfferViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetOfferViewModel>();

            // shape data
            var shapeData = _dataShaperGetOfferViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedOfferAsync(QueryOffer requestParameter)
        {
            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetOfferViewModel, Offer>();


            int recordsTotal, recordsFiltered;


            //var query = _Offers//.AsNoTracking().AsExpandable()
            //        .Include(i => i.Warehouse).AsQueryable();
            var query = _Offers.AsNoTracking()
             .Include(c => c.Customer).AsNoTracking()
             .Include(i => i.OfferLines).ThenInclude(t => t.VatRate).AsNoTracking();


            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
            FilterBy(ref query,requestParameter.CustomerID, requestParameter.OfferNumber, 
                    requestParameter.OfferIssueDateFrom, requestParameter.OfferIssueDateTo,
                    requestParameter.OfferVaidityDateForm, requestParameter.OfferVaidityDateTo );

            // Count records after filter
            recordsFiltered = await query.CountAsync();

            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            // set order by
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                query = query.OrderBy(orderBy);
            }

            // select columns
            /*
            if (!string.IsNullOrWhiteSpace(fields))
            {
                result = result.Select<Offer>("new(" + fields + ")");
            }
            */

            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await query.ToListAsync();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetOfferViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Offer, GetOfferViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetOfferViewModel>();

            var shapeData = _dataShaperGetOfferViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBy(ref IQueryable<Offer> p_items, long CustomerID, string OfferNumber, 
                                DateTime? OfferIssueDateFrom, DateTime? OfferIssueDateTo, 
                                DateTime? OfferVaidityDateFrom, DateTime? OfferVaidityDateTo)
        {
            if (!p_items.Any())
                return;

            /*
            if (string.IsNullOrWhiteSpace(WarehouseCode) && string.IsNullOrWhiteSpace(OfferNumber) &&
                        !OfferIssueDateFrom.HasValue && !OfferIssueDateTo.HasValue &&
                        !OfferDeliveryDateFrom.HasValue && !OfferDeliveryDateTo.HasValue)
                return;
            */
            var predicate = PredicateBuilder.New<Offer>();

           predicate = predicate.And(p => (CustomerID == 0 || CustomerID == p.CustomerID)
                           && (string.IsNullOrWhiteSpace(OfferNumber) || p.OfferNumber.ToUpper().Contains(OfferNumber.ToUpper()))
                           && (!OfferIssueDateFrom.HasValue || p.OfferIssueDate >= OfferIssueDateFrom.Value)
                           && (!OfferIssueDateTo.HasValue || p.OfferIssueDate <= OfferIssueDateTo.Value)
                           && (!OfferVaidityDateFrom.HasValue || p.OfferVaidityDate >= OfferVaidityDateFrom.Value)
                           && (!OfferVaidityDateTo.HasValue || p.OfferVaidityDate <= OfferVaidityDateTo.Value)
                           );

            p_items = p_items.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

   
    }
}