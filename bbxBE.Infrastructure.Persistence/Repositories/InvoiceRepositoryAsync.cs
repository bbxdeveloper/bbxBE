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
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Consts;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class InvoiceRepositoryAsync : GenericRepositoryAsync<Invoice>, IInvoiceRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Invoice> _Invoices;
        private readonly DbSet<Warehouse> _Warehouses;
        private IDataShapeHelper<Invoice> _dataShaperInvoice;
        private IDataShapeHelper<GetInvoiceViewModel> _dataShaperGetInvoiceViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public InvoiceRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Invoice> dataShaperInvoice,
            IDataShapeHelper<GetInvoiceViewModel> dataShaperGetInvoiceViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _Invoices = dbContext.Set<Invoice>();
            _Warehouses = dbContext.Set<Warehouse>();
            _dataShaperInvoice = dataShaperInvoice;
            _dataShaperGetInvoiceViewModel = dataShaperGetInvoiceViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueInvoiceNumberAsync(string InvoiceNumber, long? ID = null)
        {
            return !await _Invoices.AnyAsync(p => p.InvoiceNumber == InvoiceNumber && !p.Deleted && (ID == null || p.ID != ID.Value));
        }

        public async Task<Invoice> AddInvoiceAsync(Invoice p_Invoice, string p_WarehouseCode)
        {
            /*
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {

                if (!string.IsNullOrWhiteSpace(p_WarehouseCode))
                {
                    p_Invoice.WarehouseID = _Warehouses.SingleOrDefault(x => x.WarehouseCode == p_WarehouseCode)?.ID;
                }

                _Invoices.Add(p_Invoice);
                await _dbContext.SaveChangesAsync();
                dbContextTransaction.Commit();

            }
            */
            return p_Invoice;
        }

        public async Task<Invoice> UpdateInvoiceAsync(Invoice p_Invoice, string p_WarehouseCode)
        {
            /*
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {

                var cnt = _Invoices.Where(x => x.ID == p_Invoice.ID).FirstOrDefault();

                if (cnt != null)
                {
                    if (!string.IsNullOrWhiteSpace(p_WarehouseCode))
                    {
                        p_Invoice.WarehouseID = _Warehouses.SingleOrDefault(x => x.WarehouseCode == p_WarehouseCode)?.ID;
                    }

                    _Invoices.Update(p_Invoice);
                    await _dbContext.SaveChangesAsync();
                    dbContextTransaction.Commit();


                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_InvoiceNOTFOUND, p_Invoice.ID));
                }
            }
            */
            return p_Invoice;
        }

        public async Task<Entity> GetInvoiceAsync(GetInvoice requestParameter)
        {


            var ID = requestParameter.ID;

            var item = await GetByIdAsync(ID);

            //            var fields = requestParameter.Fields;

            var itemModel = _mapper.Map<Invoice, GetInvoiceViewModel>(item);
            var listFieldsModel = _modelHelper.GetModelFields<GetInvoiceViewModel>();

            // shape data
            var shapeData = _dataShaperGetInvoiceViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvoiceAsync(QueryInvoice requestParameter)
        {

            var searchString = requestParameter.SearchString;

            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetInvoiceViewModel, Invoice>();


            int recordsTotal, recordsFiltered;


            var query = _Invoices//.AsNoTracking().AsExpandable()
                    .Include(i => i.Warehouse).AsQueryable();


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
            /*
            if (!string.IsNullOrWhiteSpace(fields))
            {
                result = result.Select<Invoice>("new(" + fields + ")");
            }
            */

            // paging
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await query.ToListAsync();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetInvoiceViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Invoice, GetInvoiceViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetInvoiceViewModel>();

            var shapeData = _dataShaperGetInvoiceViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBySearchString(ref IQueryable<Invoice> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<Invoice>();

            var srcFor = p_searchString.ToUpper().Trim();
           predicate = predicate.And(p => p.InvoiceNumber.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

   
    }
}