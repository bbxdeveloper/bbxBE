using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qWarehouse;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class WarehouseRepositoryAsync : GenericRepositoryAsync<Warehouse>, IWarehouseRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<Warehouse> _dataShaperWarehouse;
        private IDataShapeHelper<GetWarehouseViewModel> _dataShaperGetWarehouseViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public WarehouseRepositoryAsync(IApplicationDbContext dbContext,
            IDataShapeHelper<Warehouse> dataShaperWarehouse,
            IDataShapeHelper<GetWarehouseViewModel> dataShaperGetWarehouseViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperWarehouse = dataShaperWarehouse;
            _dataShaperGetWarehouseViewModel = dataShaperGetWarehouseViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<Warehouse> AddWarehouseAsync(Warehouse p_Warehouse)
        {

            await AddAsync(p_Warehouse);
            return p_Warehouse;
        }
        public async Task<Warehouse> UpdateWarehouseAsync(Warehouse p_Warehouse)
        {
            await UpdateAsync(p_Warehouse);
            //                await dbContextTransaction.CommitAsync();

            return p_Warehouse;
        }

        public async Task<Warehouse> DeleteWarehouseAsync(long ID)
        {

            Warehouse Warehouse = null;
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                Warehouse = _dbContext.Warehouse.Where(x => x.ID == ID).FirstOrDefault();

                if (Warehouse != null)
                {

                    await RemoveAsync(Warehouse);
                    await dbContextTransaction.CommitAsync();
                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, ID));
                }
            }
            return Warehouse;
        }

        public async Task<bool> IsUniqueWarehouseCodeAsync(string WarehouseCode, long? ID = null)
        {
            return !await _dbContext.Warehouse.AsNoTracking().AnyAsync(p => p.WarehouseCode == WarehouseCode && !p.Deleted && (ID == null || p.ID != ID.Value));
        }
        public async Task<Warehouse> GetWarehouseByCodeAsync(string WarehouseCode)
        {
            return await _dbContext.Warehouse.AsNoTracking().FirstOrDefaultAsync(p => p.WarehouseCode == WarehouseCode && !p.Deleted);
        }


        public async Task<Entity> GetWarehouseAsync(long ID)
        {

            var item = await GetByIdAsync(ID);

            //            var fields = requestParameter.Fields;

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, ID));
            }

            var itemModel = _mapper.Map<Warehouse, GetWarehouseViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetWarehouseViewModel>();

            // shape data
            var shapeData = _dataShaperGetWarehouseViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedWarehouseAsync(QueryWarehouse requestParameter)
        {

            var searchString = requestParameter.SearchString;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetWarehouseViewModel, Warehouse>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var query = _dbContext.Warehouse
                .AsNoTracking()
                .AsExpandable();

            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
            FilterBySearchString(ref query, searchString);

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
            if (!string.IsNullOrWhiteSpace(fields))
            {
                query = query.Select<Warehouse>("new(" + fields + ")");
            }

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter);

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