﻿using LinqKit;
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
        private readonly DbSet<Invoice> _invoices;
        private readonly DbSet<InvoiceLine> _invoiceLines;
        private readonly DbSet<SummaryByVatRate> _summaryByVatRates;
        private readonly DbSet<AdditionalInvoiceData> _additionalInvoiceData;
        private readonly DbSet<AdditionalInvoiceLineData> _additionalInvoiceLineData;

        private readonly DbSet<Customer> _customers;
        private readonly DbSet<VatRate> _vatRates;
        private readonly DbSet<Warehouse> _warehouses;

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
    
            _invoices = dbContext.Set<Invoice>();
            _invoiceLines = dbContext.Set<InvoiceLine>();
            _summaryByVatRates = dbContext.Set<SummaryByVatRate>();
            _additionalInvoiceData = dbContext.Set<AdditionalInvoiceData>();
            _additionalInvoiceLineData = dbContext.Set<AdditionalInvoiceLineData>();
            _customers = dbContext.Set<Customer>();
            _vatRates = dbContext.Set<VatRate>();
            _warehouses = dbContext.Set<Warehouse>();

            _dataShaperInvoice = dataShaperInvoice;
            _dataShaperGetInvoiceViewModel = dataShaperGetInvoiceViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }


        public async Task<bool> IsUniqueInvoiceNumberAsync(string InvoiceNumber, long? ID = null)
        {
            return !await _invoices.AnyAsync(p => p.InvoiceNumber == InvoiceNumber && !p.Deleted && (ID == null || p.ID != ID.Value));
        }


        public async Task<Invoice> AddInvoiceAsync(Invoice p_invoice, List<InvoiceLine> p_invoiceLines, List<SummaryByVatRate> p_summaryByVatRate , List<AdditionalInvoiceData> p_additionalInvoiceData, List<AdditionalInvoiceLineData> p_additionalInvoiceLineData)
        {
            
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
//                p_invoice = bllCounter.SafeGetNextAsync()

                _invoices.Add(p_invoice);
                await _dbContext.SaveChangesAsync();
                p_invoiceLines.ForEach(i => i.InvoiceID = p_invoice.ID);
                _ = _invoiceLines.AddRangeAsync(p_invoiceLines);

                p_summaryByVatRate.ForEach(i => i.InvoiceID = p_invoice.ID);
                _ = _summaryByVatRates.AddRangeAsync(p_summaryByVatRate);

                if (p_additionalInvoiceData != null && p_additionalInvoiceData.Count > 0)
                {
                    p_additionalInvoiceData.ForEach(i => i.InvoiceID = p_invoice.ID);
                    _ = _additionalInvoiceData.AddRangeAsync(p_additionalInvoiceData);
                }

                if (p_additionalInvoiceData != null && p_additionalInvoiceData.Count > 0)
                {
                    p_additionalInvoiceData.ForEach(i => i.InvoiceID = p_invoice.ID);
                    _ = _additionalInvoiceLineData.AddRangeAsync(p_additionalInvoiceLineData);
                }
                await _dbContext.SaveChangesAsync();
                dbContextTransaction.Commit();
            }
            
            return p_invoice;
        }

        public async Task<Invoice> UpdateInvoiceAsync(Invoice p_invoice, List<InvoiceLine> p_invoiceLines, List<SummaryByVatRate> p_summaryByVatRate, List<AdditionalInvoiceData> p_additionalInvoiceData, List<AdditionalInvoiceLineData> p_additionalInvoiceLineData)
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
            return p_invoice;
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


            var pageNumber = requestParameter.PageNumber;
            var pageSize = requestParameter.PageSize;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetInvoiceViewModel, Invoice>();


            int recordsTotal, recordsFiltered;


            var query = _invoices//.AsNoTracking().AsExpandable()
                    .Include(i => i.Warehouse).AsQueryable();


            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
            FilterBy(ref query, requestParameter.Incoming, requestParameter.WarehouseCode, requestParameter.InvoiceNumber, 
                    requestParameter.InvoiceIssueDateFrom, requestParameter.InvoiceIssueDateTo,
                    requestParameter.InvoiceDeliveryDateFrom, requestParameter.InvoiceDeliveryDateTo);

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

        private void FilterBy(ref IQueryable<Invoice> p_item, bool Incoming,  string WarehouseCode, string InvoiceNumber, 
                                DateTime? InvoiceIssueDateFrom, DateTime? InvoiceIssueDateTo, 
                                DateTime? InvoiceDeliveryDateFrom, DateTime? InvoiceDeliveryDateTo)
        {
            if (!p_item.Any())
                return;

            /*
            if (string.IsNullOrWhiteSpace(WarehouseCode) && string.IsNullOrWhiteSpace(InvoiceNumber) &&
                        !InvoiceIssueDateFrom.HasValue && !InvoiceIssueDateTo.HasValue &&
                        !InvoiceDeliveryDateFrom.HasValue && !InvoiceDeliveryDateTo.HasValue)
                return;
            */
            var predicate = PredicateBuilder.New<Invoice>();

           predicate = predicate.And(p => p.Incoming == Incoming
                           && (WarehouseCode == null || p.Warehouse.WarehouseCode.ToUpper().Contains(WarehouseCode))
                           && (InvoiceNumber == null || p.InvoiceNumber.Contains(InvoiceNumber))
                           && (!InvoiceIssueDateFrom.HasValue || p.InvoiceIssueDate >= InvoiceIssueDateFrom.Value)
                           && (!InvoiceIssueDateTo.HasValue || p.InvoiceIssueDate <= InvoiceIssueDateFrom.Value)
                           && (!InvoiceDeliveryDateFrom.HasValue || p.InvoiceDeliveryDate >= InvoiceDeliveryDateFrom.Value)
                           && (!InvoiceDeliveryDateTo.HasValue || p.InvoiceDeliveryDate <= InvoiceDeliveryDateTo.Value)
                           );

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

   
    }
}