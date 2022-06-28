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
using bbxBE.Application.Queries.qStockCard;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Consts;
using static bbxBE.Common.NAV.NAV_enums;
using bbxBE.Common.Enums;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class StockCardRepositoryAsync : GenericRepositoryAsync<StockCard>, IStockCardRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Stock> _Stocks;
        private readonly DbSet<StockCard> _StockCards;
        private IDataShapeHelper<StockCard> _dataShaperStockCard;
        private IDataShapeHelper<GetStockCardViewModel> _dataShaperGetStockCardViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public StockCardRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<StockCard> dataShaperStockCard,
            IDataShapeHelper<GetStockCardViewModel> dataShaperGetStockCardViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _StockCards = dbContext.Set<StockCard>();
            _Stocks = dbContext.Set<Stock>();
            _dataShaperStockCard = dataShaperStockCard;
            _dataShaperGetStockCardViewModel = dataShaperGetStockCardViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }

      
        public async Task<StockCard> CreateStockCard(DateTime StockCardDate, long StockID,
            long WarehouseID, long? ProductID, long? UserID, long? InvoiceLineID, long? CustomerID,
            enStockCardTypes ScType,
            decimal OCalcQty, decimal ORealQty,
            decimal XCalcQty, decimal XRealQty,
            decimal OAvgCost, decimal NAvgCost,
            string XRel)



        {
            var sc = new StockCard()
            {
                StockCardDate = StockCardDate,
                StockID = StockID,
                WarehouseID = WarehouseID,
                ProductID = ProductID,
                UserID = UserID,
                InvoiceLineID = InvoiceLineID,
                CustomerID = CustomerID,
                ScType = ScType.ToString(),
                OCalcQty = OCalcQty,
                ORealQty = ORealQty,
                OOutQty = 0,                //ha lesz árukiadás, töltjük!
                XCalcQty = XCalcQty,
                XRealQty = XRealQty,
                XOutQty = 0,                //ha lesz árukiadás, töltjük!
                NCalcQty = OCalcQty + XCalcQty,
                NRealQty = ORealQty + XRealQty,
                NOutQty = 0,                //ha lesz árukiadás, töltjük!
                OAvgCost = OAvgCost,
                NAvgCost = NAvgCost,
                XRel = XRel
            };
            await _StockCards.AddAsync(sc);
            return sc;
        }



        public async Task<Entity> GetStockCardAsync(GetStockCard requestParameter)
        {

            var ID = requestParameter.ID;

            var item = await _StockCards.AsNoTracking()
                        .Include(p => p.Product).ThenInclude(p2 => p2.ProductCodes.FirstOrDefault(pc => pc.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString())).AsNoTracking()
                        .Include(w => w.Warehouse).AsNoTracking()
                        .Where(x => x.ID == ID && !x.Deleted).FirstOrDefaultAsync();

            //            var fields = requestParameter.Fields;

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_STOCKCARDNOTFOUND, ID));
            }

            var itemModel = _mapper.Map<StockCard, GetStockCardViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetStockCardViewModel>();

            // shape data
            var shapeData = _dataShaperGetStockCardViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedStockCardAsync(QueryStockCard requestParameter)
        {


            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetStockCardViewModel, StockCard>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _StockCards.AsNoTracking()
                        .Include(p => p.Product).ThenInclude(p2 => p2.ProductCodes.FirstOrDefault(pc => pc.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString())).AsNoTracking()
                        .Include(w => w.Warehouse).AsNoTracking();

            // Count records total
            recordsTotal = await result.CountAsync();

            // filter data

     

        FilterBy(ref result, requestParameter.WarehouseID, requestParameter.StockCardDateFrom, requestParameter.StockCardDateTo, requestParameter.InvoiceNumber, requestParameter.ProductID);

            // Count records after filter
            recordsFiltered = await result.CountAsync();

            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            // set order by
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                result = result.OrderBy(orderBy);
            }

            // select columns
            if (!string.IsNullOrWhiteSpace(fields))
            {
                result = result.Select<StockCard>("new(" + fields + ")");
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetStockCardViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<StockCard, GetStockCardViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetStockCardViewModel>();

            var shapeData = _dataShaperGetStockCardViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBy(ref IQueryable<StockCard> p_item,
            long? WarehouseID, DateTime? StockCardDateFrom, DateTime? StockCardDateTo, string InvoiceNumber, long? ProductID)


        {
            if (!p_item.Any())
                return;


            var predicate = PredicateBuilder.New<StockCard>();
            if (WarehouseID.HasValue && WarehouseID.Value > 0)
            {
                predicate = predicate.And(p => p.WarehouseID == WarehouseID.Value);
            }
            if (StockCardDateFrom.HasValue)
            {
                predicate = predicate.And(p => p.StockCardDate >= StockCardDateFrom.Value);
            }
            if (StockCardDateTo.HasValue)
            {
                predicate = predicate.And(p => p.StockCardDate <= StockCardDateTo.Value);
            }
            if (ProductID.HasValue && ProductID.Value > 0)
            {
                predicate = predicate.And(p => p.ProductID == ProductID.Value);
            }


            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }


    }
}