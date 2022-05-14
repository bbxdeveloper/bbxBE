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
    public class OfferRepositoryAsync : GenericRepositoryAsync<Offer>, IOfferRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Offer> _Offers;
        private readonly DbSet<OfferLine> _OfferLines;
        private readonly DbSet<SummaryByVatRate> _summaryByVatRates;
        private readonly DbSet<AdditionalOfferData> _additionalOfferData;
        private readonly DbSet<AdditionalOfferLineData> _additionalOfferLineData;

        private readonly DbSet<Customer> _customers;
        private readonly DbSet<VatRate> _vatRates;
        private readonly DbSet<Warehouse> _warehouses;

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
            _summaryByVatRates = dbContext.Set<SummaryByVatRate>();
            _additionalOfferData = dbContext.Set<AdditionalOfferData>();
            _additionalOfferLineData = dbContext.Set<AdditionalOfferLineData>();
            _customers = dbContext.Set<Customer>();
            _vatRates = dbContext.Set<VatRate>();
            _warehouses = dbContext.Set<Warehouse>();

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

        public async Task<Offer> UpdateOfferAsync(Offer p_Offer, List<OfferLine> p_OfferLines, List<SummaryByVatRate> p_summaryByVatRate, List<AdditionalOfferData> p_additionalOfferData, List<AdditionalOfferLineData> p_additionalOfferLineData)
        {
            throw new NotImplementedException("UpdateOfferAsync");
        }

        public async Task<Entity> GetOfferAsync(GetOffer requestParameter)
        {


            var ID = requestParameter.ID;

            var item = _Offers.AsNoTracking()
              .Include(w => w.Warehouse).AsNoTracking()
              .Include(s => s.Supplier).AsNoTracking()
              .Include(c => c.Customer).AsNoTracking()
              .Include(a => a.AdditionalOfferData).AsNoTracking()
              .Include(i => i.OfferLines).ThenInclude(t => t.VatRate).AsNoTracking()
              .Include(a => a.SummaryByVatRates).ThenInclude(t => t.VatRate).AsNoTracking()
              .Where(x => x.ID == ID).FirstOrDefault();

            //            var fields = requestParameter.Fields;

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
             .Include(w => w.Warehouse).AsNoTracking()
             .Include(s => s.Supplier).AsNoTracking()
             .Include(c => c.Customer).AsNoTracking()
             .Include(a => a.AdditionalOfferData).AsNoTracking()
             .Include(i => i.OfferLines).ThenInclude(t => t.VatRate).AsNoTracking()
             .Include(a => a.SummaryByVatRates).ThenInclude(t => t.VatRate).AsNoTracking();


            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
            FilterBy(ref query, requestParameter.Incoming, requestParameter.WarehouseCode, requestParameter.OfferNumber, 
                    requestParameter.OfferIssueDateFrom, requestParameter.OfferIssueDateTo,
                    requestParameter.OfferDeliveryDateFrom, requestParameter.OfferDeliveryDateTo);

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

        private void FilterBy(ref IQueryable<Offer> p_item, bool Incoming,  string WarehouseCode, string OfferNumber, 
                                DateTime? OfferIssueDateFrom, DateTime? OfferIssueDateTo, 
                                DateTime? OfferDeliveryDateFrom, DateTime? OfferDeliveryDateTo)
        {
            if (!p_item.Any())
                return;

            /*
            if (string.IsNullOrWhiteSpace(WarehouseCode) && string.IsNullOrWhiteSpace(OfferNumber) &&
                        !OfferIssueDateFrom.HasValue && !OfferIssueDateTo.HasValue &&
                        !OfferDeliveryDateFrom.HasValue && !OfferDeliveryDateTo.HasValue)
                return;
            */
            var predicate = PredicateBuilder.New<Offer>();

           predicate = predicate.And(p => p.Incoming == Incoming
                           && (WarehouseCode == null || p.Warehouse.WarehouseCode.ToUpper().Contains(WarehouseCode))
                           && (OfferNumber == null || p.OfferNumber.Contains(OfferNumber))
                           && (!OfferIssueDateFrom.HasValue || p.OfferIssueDate >= OfferIssueDateFrom.Value)
                           && (!OfferIssueDateTo.HasValue || p.OfferIssueDate <= OfferIssueDateFrom.Value)
                           && (!OfferDeliveryDateFrom.HasValue || p.OfferDeliveryDate >= OfferDeliveryDateFrom.Value)
                           && (!OfferDeliveryDateTo.HasValue || p.OfferDeliveryDate <= OfferDeliveryDateTo.Value)
                           );

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

   
    }
}