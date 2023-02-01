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
using System;
using AutoMapper;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using bbxBE.Common.Attributes;
using System.ComponentModel;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using bbxBE.Common.Enums;
using bbxBE.Application.Interfaces.Queries;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class InvoiceRepositoryAsync : GenericRepositoryAsync<Invoice>, IInvoiceRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<Invoice> _dataShaperInvoice;
        private IDataShapeHelper<GetInvoiceViewModel> _dataShaperGetInvoiceViewModel;
        private IDataShapeHelper<GetPendigDeliveryNotesSummaryModel> _dataShaperGetPendigDeliveryNotesSummaryModel;
        private IDataShapeHelper<GetPendigDeliveryNotesModel> _dataShaperGetPendigDeliveryNotesModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IStockRepositoryAsync _stockRepository;
        private readonly ICustomerRepositoryAsync _customerRepository;
        public InvoiceRepositoryAsync(ApplicationDbContext dbContext,


        IDataShapeHelper<Invoice> dataShaperInvoice,
            IDataShapeHelper<GetInvoiceViewModel> dataShaperGetInvoiceViewModel,
            IDataShapeHelper<GetPendigDeliveryNotesSummaryModel> dataShaperGetPendigDeliveryNotesSummaryModel,
            IDataShapeHelper<GetPendigDeliveryNotesModel> dataShaperGetPendigDeliveryNotesModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            IStockRepositoryAsync stockRepository,
            ICustomerRepositoryAsync customerRepository
            ) : base(dbContext)
        {
            _dbContext = dbContext;

            _dataShaperInvoice = dataShaperInvoice;
            _dataShaperGetInvoiceViewModel = dataShaperGetInvoiceViewModel;
            _dataShaperGetPendigDeliveryNotesSummaryModel = dataShaperGetPendigDeliveryNotesSummaryModel;
            _dataShaperGetPendigDeliveryNotesModel = dataShaperGetPendigDeliveryNotesModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _stockRepository = stockRepository;
            _customerRepository = customerRepository;
        }


        public async Task<bool> IsUniqueInvoiceNumberAsync(string InvoiceNumber, long? ID = null)
        {
            return !await _dbContext.Invoice.AnyAsync(p => p.InvoiceNumber == InvoiceNumber && !p.Deleted && (ID == null || p.ID != ID.Value));
        }


        public async Task<Invoice> AddInvoiceAsync(Invoice p_invoice)
        {

            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var stockList = await _stockRepository.MaintainStockByInvoiceAsync(p_invoice);

                    //c# how to disable save related entity in EF ???
                    //TODO: ideiglenes megoldás, relációban álló objektumok Detach-olása hogy ne akarja menteni azokat az EF 
                    if (p_invoice.Customer != null)
                        p_invoice.Customer = null;

                    if (p_invoice.Supplier != null)
                        p_invoice.Supplier = null;

                    foreach (var il in p_invoice.InvoiceLines)
                    {
                        il.Product = null;
                        il.VatRate = null;


                    }

                    await AddAsync(p_invoice);
                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }

            return p_invoice;
        }

        public async Task<Invoice> InitInvoiceAsync(Invoice p_invoice)
        {
            throw new NotImplementedException("InitInvoiceAsync");
        }


        public async Task<Invoice> UpdateInvoiceAsync(Invoice p_invoice)
        {
            using (var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    await UpdateAsync(p_invoice);
                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
                return p_invoice;
            }
        }

        public async Task<Entity> GetInvoiceAsync(long ID, bool FullData)
        {

            Invoice item = await GetInvoiceRecordAsync(ID, FullData);

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, ID));
            }

            var itemModel = _mapper.Map<Invoice, GetInvoiceViewModel>(item);

            if (!FullData)
            {
                itemModel.InvoiceLines.Clear();         //itt már nem kellenek a sorok. 
                itemModel.SummaryByVatRates.Clear();         //itt már nem kellenek a sorok. 
            }
            var listFieldsModel = _modelHelper.GetModelFields<GetInvoiceViewModel>();

            // shape data
            var shapeData = _dataShaperGetInvoiceViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
        }

        public async Task<Invoice> GetInvoiceRecordAsync(long ID, bool FullData = true)
        {


            Invoice item;

            if (FullData)
            {
                item = await _dbContext.Invoice.AsNoTracking()
                  .Include(w => w.Warehouse).AsNoTracking()
                  .Include(s => s.Supplier).AsNoTracking()
                  .Include(c => c.Customer).AsNoTracking()
                  .Include(a => a.AdditionalInvoiceData).AsNoTracking()
                  .Include(i => i.InvoiceLines).ThenInclude(t => t.VatRate).AsNoTracking()
                  .Include(a => a.SummaryByVatRates).ThenInclude(t => t.VatRate).AsNoTracking()
                  .Include(u => u.User).AsNoTracking()
                  .Where(x => x.ID == ID).FirstOrDefaultAsync();
            }
            else
            {
                item = await _dbContext.Invoice.AsNoTracking()
                  .Include(w => w.Warehouse).AsNoTracking()
                  .Include(s => s.Supplier).AsNoTracking()
                  .Include(c => c.Customer).AsNoTracking()
                  .Include(a => a.AdditionalInvoiceData).AsNoTracking()
                  .Include(i => i.InvoiceLines).AsNoTracking()                   //nem full data esetén is szüség van az invoiceLines-re
                  .Where(x => x.ID == ID).FirstOrDefaultAsync();
            }
            return item;
        }

        public async Task<IEnumerable<Entity>> GetPendigDeliveryNotesSummaryAsync(bool incoming, long warehouseID, string currencyCode)
        {


            var lstEntities = new List<GetPendigDeliveryNotesSummaryModel>();

            IQueryable< GetPendigDeliveryNotesSummaryModel> q1;
            
            //a tételsorokben lévő PriceReview miatt nested groupra van szükség


            if (incoming)
            {
                //először grouo-olunk ccustomerre és PriceReview
                q1 = from InvoiceLine in _dbContext.InvoiceLine
                         join Invoice in _dbContext.Invoice on InvoiceLine.InvoiceID equals Invoice.ID
                         join Customer in _dbContext.Customer on Invoice.SupplierID equals Customer.ID
                         where InvoiceLine.PendingDNQuantity > 0
                            && Invoice.Incoming == incoming
                            && Invoice.WarehouseID == warehouseID
                            && Invoice.InvoiceType == enInvoiceType.DNI.ToString()
                            && Invoice.CurrencyCode == currencyCode
                         group InvoiceLine by
                         new { CustomerID = Invoice.SupplierID, CustomerName = Customer.CustomerName, PriceReview = InvoiceLine.PriceReview }
                         into grpInner
                         select new GetPendigDeliveryNotesSummaryModel()
                         {

                             WarehouseID = warehouseID,
                             CustomerID = grpInner.Key.CustomerID,
                             Customer = grpInner.Key.CustomerName,
                             PriceReview = grpInner.Key.PriceReview.HasValue,
                             SumNetAmount = grpInner.Sum(s => s.LineNetAmount)
                         };

             }
            else
            {
                 q1 = from InvoiceLine in _dbContext.InvoiceLine
                         join Invoice in _dbContext.Invoice on InvoiceLine.InvoiceID equals Invoice.ID
                         join Customer in _dbContext.Customer on Invoice.CustomerID equals Customer.ID
                         where InvoiceLine.PendingDNQuantity > 0
                            && Invoice.Incoming == incoming
                            && Invoice.WarehouseID == warehouseID
                            && Invoice.InvoiceType == enInvoiceType.DNO.ToString()
                            && Invoice.CurrencyCode == currencyCode
                         group InvoiceLine by
                         new { CustomerID = Invoice.CustomerID, CustomerName = Customer.CustomerName, PriceReview = InvoiceLine.PriceReview }
                         into grpInner
                      select new GetPendigDeliveryNotesSummaryModel()
                      {

                          WarehouseID = warehouseID,
                          CustomerID = grpInner.Key.CustomerID,
                          Customer = grpInner.Key.CustomerName,
                          PriceReview = grpInner.Key.PriceReview.HasValue,
                          SumNetAmount = grpInner.Sum(s => s.LineNetAmount)
                      };
            }

            var q2 = from res in q1
                     group res by
                     new { CustomerID = res.CustomerID, Customer = res.Customer }
                     into grpOuter
                     select new GetPendigDeliveryNotesSummaryModel()
                     {
                         WarehouseID = warehouseID,
                         CustomerID = grpOuter.Key.CustomerID,
                         Customer = grpOuter.Key.Customer,
                         PriceReview = grpOuter.Any(a => a.PriceReview),
                         SumNetAmount = grpOuter.Sum(s => s.SumNetAmount)
                     };

            q2 = q2.OrderBy(o => o.Customer);;

            lstEntities = await q2.ToListAsync();


            var shapeData = _dataShaperGetPendigDeliveryNotesSummaryModel.ShapeData(lstEntities, "");

            return shapeData;
        }
        public async Task<IEnumerable<Entity>> GetPendigDeliveryNotesAsync(bool incoming, long warehouseID, long customerID, string currencyCode)
        {
            var own = _customerRepository.GetOwnData();

            var query = _dbContext.InvoiceLine
                .Include(t => t.VatRate).AsNoTracking()
                .Include(i => i.Invoice).ThenInclude(s => s.Supplier)
                .Include(i => i.Invoice).ThenInclude(c => c.Customer).AsNoTracking()
                .Where(w => w.PendingDNQuantity > 0
                        && w.Invoice.Incoming == incoming
                        && w.Invoice.WarehouseID == warehouseID
                        && w.Invoice.InvoiceType == (incoming ? enInvoiceType.DNI.ToString() : enInvoiceType.DNO.ToString())
                        && w.Invoice.SupplierID == (incoming ? customerID : own.ID)
                        && w.Invoice.CustomerID == (incoming ? own.ID : customerID)
                        )
                .OrderBy(o => o.Invoice.InvoiceNumber);

            var resultData = await query.ToListAsync();

            //TODO: szebben megoldani
            var resultDataModel = new List<GetPendigDeliveryNotesModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<InvoiceLine, GetPendigDeliveryNotesModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetPendigDeliveryNotesModel>();

            var shapeData = _dataShaperGetPendigDeliveryNotesModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return shapeData;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvoiceAsync(QueryInvoice requestParameter)
        {

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetInvoiceViewModel, Invoice>();


            int recordsTotal, recordsFiltered;


            //var query = _dbContext.Invoice//.AsNoTracking().AsExpandable()
            //        .Include(i => i.Warehouse).AsQueryable();

            IQueryable<Invoice> query;
            if (requestParameter.FullData)
            {
                query = _dbContext.Invoice.AsNoTracking()
                 .Include(w => w.Warehouse).AsNoTracking()
                 .Include(s => s.Supplier).AsNoTracking()
                 .Include(c => c.Customer).AsNoTracking()
                 .Include(a => a.AdditionalInvoiceData).AsNoTracking()
                 .Include(i => i.InvoiceLines).ThenInclude(t => t.VatRate).AsNoTracking()
                 .Include(a => a.SummaryByVatRates).ThenInclude(t => t.VatRate).AsNoTracking()
                 .Include(u => u.User).AsNoTracking();
            }
            else
            {
                query = _dbContext.Invoice.AsNoTracking()
                 .Include(w => w.Warehouse).AsNoTracking()
                 .Include(s => s.Supplier).AsNoTracking()
                 .Include(c => c.Customer).AsNoTracking()
                 .Include(a => a.AdditionalInvoiceData).AsNoTracking()
                 .Include(i => i.InvoiceLines).AsNoTracking()                   //nem full data esetén is szüség van az invoiceLines-re
                 .Include(u => u.User).AsNoTracking();
            }


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

            // retrieve data to list
            List<Invoice> resultData = await GetPagedData(query, requestParameter);


            //TODO: szebben megoldani
            var resultDataModel = new List<GetInvoiceViewModel>();
            resultData.ForEach(i =>
            {
                var im = _mapper.Map<Invoice, GetInvoiceViewModel>(i);
                if (!requestParameter.FullData)
                {
                    im.InvoiceLines.Clear();         //itt már nem kellenek a sorok. 
                    im.SummaryByVatRates.Clear();         //itt már nem kellenek a sorok. 
                }
                resultDataModel.Add(im);  //nem full data esetén is szüség van az invoiceLines-re

            }
            );

            var listFieldsModel = _modelHelper.GetModelFields<GetInvoiceViewModel>();

            var shapeData = _dataShaperGetInvoiceViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
        }

        private void FilterBy(ref IQueryable<Invoice> p_items, bool Incoming, string WarehouseCode, string InvoiceNumber,
                                DateTime? InvoiceIssueDateFrom, DateTime? InvoiceIssueDateTo,
                                DateTime? InvoiceDeliveryDateFrom, DateTime? InvoiceDeliveryDateTo)
        {
            if (!p_items.Any())
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
                            && (!InvoiceIssueDateTo.HasValue || p.InvoiceIssueDate <= InvoiceIssueDateTo.Value)
                            && (!InvoiceDeliveryDateFrom.HasValue || p.InvoiceDeliveryDate >= InvoiceDeliveryDateFrom.Value)
                            && (!InvoiceDeliveryDateTo.HasValue || p.InvoiceDeliveryDate <= InvoiceDeliveryDateTo.Value)
                           );

            p_items = p_items.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }


    }
}