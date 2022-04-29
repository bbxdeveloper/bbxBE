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
using bbxBE.Application.Queries.qWarehouse;
using bbxBE.Application.Queries.ViewModels;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class WarehouseRepositoryAsync : GenericRepositoryAsync<Warehouse>, IWarehouseRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Warehouse> _Warehouses;
        private IDataShapeHelper<Warehouse> _dataShaperWarehouse;
        private IDataShapeHelper<GetWarehouseViewModel> _dataShaperGetWarehouseViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public WarehouseRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Warehouse> dataShaperWarehouse,
            IDataShapeHelper<GetWarehouseViewModel> dataShaperGetWarehouseViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _Warehouses = dbContext.Set<Warehouse>();
            _dataShaperWarehouse = dataShaperWarehouse;
            _dataShaperGetWarehouseViewModel = dataShaperGetWarehouseViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueWarehouseCodeAsync(string WarehouseCode, long? ID = null)
        {
            return !await _Warehouses.AnyAsync(p => p.WarehouseCode == WarehouseCode && !p.Deleted && (ID == null || p.ID != ID.Value));
        }
        public async Task<Warehouse> GetWarehouseByCodeAsync(string WarehouseCode)
        {
            return await _Warehouses.FirstOrDefaultAsync(p => p.WarehouseCode == WarehouseCode && !p.Deleted );
        }


        public async Task<Entity> GetWarehouseAsync(GetWarehouse requestParameter)
        {


            var ID = requestParameter.ID;

            var item = await GetByIdAsync(ID);

            //            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<Warehouse, GetWarehouseViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetWarehouseViewModel>();

            // shape data
            var shapeData = _dataShaperGetWarehouseViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedWarehouseAsync(QueryWarehouse requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetWarehouseViewModel, Warehouse>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _Warehouses
                .AsNoTracking()
                .AsExpandable();

            // Count records total
            recordsTotal = await result.CountAsync();

            // filter data
            FilterBySearchString(ref result, searchString);

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
                result = result.Select<Warehouse>("new(" + fields + ")");
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetWarehouseViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Warehouse, GetWarehouseViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetWarehouseViewModel>();

            var shapeData = _dataShaperGetWarehouseViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<Warehouse> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<Warehouse>();

            var srcFor = p_searchString.ToUpper().Trim();
            predicate = predicate.And(p => p.WarehouseDescription.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

   
    }
}