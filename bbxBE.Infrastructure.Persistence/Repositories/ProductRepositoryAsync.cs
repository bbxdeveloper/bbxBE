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
using bbxBE.Application.Queries.qProduct;
using bbxBE.Application.Queries.ViewModels;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductRepositoryAsync : GenericRepositoryAsync<Product>, IProductRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Product> _Products;
        private IDataShapeHelper<Product> _dataShaperProduct;
        private IDataShapeHelper<GetProductViewModel> _dataShaperGetProductViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public ProductRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Product> dataShaperProduct,
            IDataShapeHelper<GetProductViewModel> dataShaperGetProductViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _Products = dbContext.Set<Product>();
            _dataShaperProduct = dataShaperProduct;
            _dataShaperGetProductViewModel = dataShaperGetProductViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueProductCodeAsync(string ProductCode, long? ID = null)
        {
            //           return !await _Products.AnyAsync(p => p.ProductCode == ProductCode && !p.Deleted && (ID == null || p.ID != ID.Value));
            return false;
        }


        public async Task<Entity> GetProductReponseAsync(GetProduct requestParameter)
        {


            var ID = requestParameter.ID;

            var item = await GetByIdAsync(ID);

            //            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<Product, GetProductViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

            // shape data
            var shapeData = _dataShaperGetProductViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedProductReponseAsync(QueryProduct requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetProductViewModel, Product>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _Products
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
                result = result.Select<Product>("new(" + fields + ")");
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetProductViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Product, GetProductViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetProductViewModel>();

            var shapeData = _dataShaperGetProductViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<Product> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<Product>();

            var srcFor = p_searchString.ToUpper().Trim();
//TODO: átírni
  //          predicate = predicate.And(p => p.ProductDescription.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }
    }
}