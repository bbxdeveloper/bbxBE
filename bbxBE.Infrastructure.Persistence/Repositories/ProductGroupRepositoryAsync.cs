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
using bbxBE.Application.Queries.qProductGroup;
using bbxBE.Application.Queries.ViewModels;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductGroupRepositoryAsync : GenericRepositoryAsync<ProductGroup>, IProductGroupRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<ProductGroup> _productGroups;
        private IDataShapeHelper<ProductGroup> _dataShaperProductGroup;
        private IDataShapeHelper<GetProductGroupViewModel> _dataShaperGetProductGroupViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public ProductGroupRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<ProductGroup> dataShaperProductGroup,
            IDataShapeHelper<GetProductGroupViewModel> dataShaperGetProductGroupViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _productGroups = dbContext.Set<ProductGroup>();
            _dataShaperProductGroup = dataShaperProductGroup;
            _dataShaperGetProductGroupViewModel = dataShaperGetProductGroupViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueProductGroupCodeAsync(string ProductGroupCode, long? ID = null)
        {
            return !await _productGroups.AnyAsync(p => p.ProductGroupCode == ProductGroupCode && !p.Deleted && (ID == null || p.ID != ID.Value));
        }


        public async Task<Entity> GetProductGroupAsync(GetProductGroup requestParameter)
        {


            var ID = requestParameter.ID;

            var item = await GetByIdAsync(ID);

            //            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<ProductGroup, GetProductGroupViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetProductGroupViewModel>();

            // shape data
            var shapeData = _dataShaperGetProductGroupViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductGroupAsync(QueryProductGroup requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetProductGroupViewModel, ProductGroup>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _productGroups
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
                result = result.Select<ProductGroup>("new(" + fields + ")");
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetProductGroupViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<ProductGroup, GetProductGroupViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetProductGroupViewModel>();

            var shapeData = _dataShaperGetProductGroupViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<ProductGroup> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<ProductGroup>();

            var srcFor = p_searchString.ToUpper().Trim();
            predicate = predicate.And(p => p.ProductGroupDescription.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

   
    }
}