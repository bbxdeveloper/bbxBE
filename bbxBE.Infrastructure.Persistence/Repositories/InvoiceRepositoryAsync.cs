using AsyncKeyedLock;
using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Common.ExpiringData;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using bxBE.Application.Commands.cmdInvoice;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static bbxBE.Common.NAV.NAV_enums;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class InvoiceRepositoryAsync : GenericRepositoryAsync<Invoice>, IInvoiceRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<Invoice> _dataShaperInvoice;
        private IDataShapeHelper<GetInvoiceViewModel> _dataShaperGetInvoiceViewModel;
        private IDataShapeHelper<GetAggregateInvoiceViewModel> _dataShaperGetAggregateInvoiceViewModel;
        private IDataShapeHelper<GetPendigDeliveryNotesSummaryModel> _dataShaperGetPendigDeliveryNotesSummaryModel;
        private IDataShapeHelper<GetPendigDeliveryNotesModel> _dataShaperGetPendigDeliveryNotesModel;
        private IDataShapeHelper<GetPendigDeliveryNotesItemModel> _dataShaperGetPendigDeliveryNotesItemModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IExpiringData<ExpiringDataObject> _expiringData;

        private readonly IInvoiceLineRepositoryAsync _invoiceLineRepository;
        private readonly IStockRepositoryAsync _stockRepository;
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly ICounterRepositoryAsync _counterRepository;
        private readonly IWarehouseRepositoryAsync _warehouseRepository;
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IVatRateRepositoryAsync _vatRateRepository;
        private readonly INAVXChangeRepositoryAsync _NAVXChangeRepository;
        private readonly IZipRepositoryAsync _ZipRepositoryAsync;


        public InvoiceRepositoryAsync(IApplicationDbContext dbContext,
                IModelHelper modelHelper, IMapper mapper, IMockService mockData,
                IExpiringData<ExpiringDataObject> expiringData,
                ICacheService<Product> productCacheService,
                ICacheService<Customer> customerCacheService,
                ICacheService<ProductGroup> productGroupCacheService,
                ICacheService<Origin> originCacheService,
                ICacheService<VatRate> vatRateCacheService,
                AsyncKeyedLocker<string> asyncKeyedLocker) : base(dbContext)
        {
            _dbContext = dbContext;

            _dataShaperInvoice = new DataShapeHelper<Invoice>();
            _dataShaperGetInvoiceViewModel = new DataShapeHelper<GetInvoiceViewModel>();
            _dataShaperGetAggregateInvoiceViewModel = new DataShapeHelper<GetAggregateInvoiceViewModel>();
            _dataShaperGetPendigDeliveryNotesSummaryModel = new DataShapeHelper<GetPendigDeliveryNotesSummaryModel>();
            _dataShaperGetPendigDeliveryNotesModel = new DataShapeHelper<GetPendigDeliveryNotesModel>();
            _dataShaperGetPendigDeliveryNotesItemModel = new DataShapeHelper<GetPendigDeliveryNotesItemModel>();
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;

            _invoiceLineRepository = new InvoiceLineRepositoryAsync(dbContext, modelHelper, mapper, mockData);
            _stockRepository = new StockRepositoryAsync(dbContext, modelHelper, mapper, mockData, productCacheService, productGroupCacheService, originCacheService, vatRateCacheService, asyncKeyedLocker);
            _customerRepository = new CustomerRepositoryAsync(dbContext, modelHelper, mapper, mockData, expiringData, customerCacheService);
            _counterRepository = new CounterRepositoryAsync(dbContext, modelHelper, mapper, mockData);
            _warehouseRepository = new WarehouseRepositoryAsync(dbContext, modelHelper, mapper, mockData);
            _productRepository = new ProductRepositoryAsync(dbContext, modelHelper, mapper, mockData, productCacheService, productGroupCacheService, originCacheService, vatRateCacheService);
            _vatRateRepository = new VatRateRepositoryAsync(dbContext, modelHelper, mapper, mockData, vatRateCacheService);
            _NAVXChangeRepository = new NAVXChangeRepositoryAsync(dbContext, modelHelper, mapper, mockData);
            _ZipRepositoryAsync = new ZipRepositoryAsync(dbContext, modelHelper, mapper, mockData);
            _expiringData = expiringData;
        }
        public async Task<bool> IsUniqueInvoiceNumberAsync(string InvoiceNumber, long? ID = null)
        {
            return !await _dbContext.Invoice.AnyAsync(p => p.InvoiceNumber == InvoiceNumber && !p.Deleted && (ID == null || p.ID != ID.Value));
        }

        private void prepareInvoiceBeforePersistance(Invoice p_invoice)
        {
            //TODO: ideiglenes megoldás, relációban álló objektumok Detach-olása hogy ne akarja menteni azokat az EF 
            if (p_invoice.Customer != null)
            {
                //                p_invoice.Customer = null;
                _dbContext.Instance.Entry(p_invoice.Customer).State = EntityState.Detached;
            }

            if (p_invoice.Supplier != null)
            {
                // p_invoice.Supplier = null;
                _dbContext.Instance.Entry(p_invoice.Supplier).State = EntityState.Detached;
            }
            if (p_invoice.InvoiceLines != null)
            {
                foreach (var il in p_invoice.InvoiceLines)
                {
                    if (il.ID == 0)
                    {
                        _dbContext.Instance.Entry(il).State = EntityState.Added;
                    }
                    else
                    {
                        _dbContext.Instance.Entry(il).State = EntityState.Modified;
                    }

                    // il.Product = null;
                    // il.VatRate = null;

                    if (il.Product != null)
                    {
                        _dbContext.Instance.Entry(il.Product).State = EntityState.Detached;
                    }

                    if (il.VatRate != null)
                    {
                        _dbContext.Instance.Entry(il.VatRate).State = EntityState.Detached;
                    }

                    if (il.AdditionalInvoiceLineData != null)
                    {

                        foreach (var aild in il.AdditionalInvoiceLineData)
                        {
                            if (aild.ID == 0)
                            {
                                _dbContext.Instance.Entry(aild).State = EntityState.Added;
                            }
                            else
                            {
                                _dbContext.Instance.Entry(aild).State = EntityState.Modified;
                            }
                        }
                    }
                }
            }
            if (p_invoice.SummaryByVatRates != null)
            {
                foreach (var svr in p_invoice.SummaryByVatRates)
                {
                    if (svr.ID == 0)
                    {
                        _dbContext.Instance.Entry(svr).State = EntityState.Added;
                    }
                    else
                    {
                        _dbContext.Instance.Entry(svr).State = EntityState.Modified;
                    }
                    // svr.VatRate = null;

                    if (svr.VatRate != null)
                    {
                        _dbContext.Instance.Entry(svr.VatRate).State = EntityState.Detached;
                    }
                }
            }
        }

        public async Task<Invoice> AddInvoiceAsync(Invoice p_invoice, Dictionary<long, InvoiceLine> p_RelDNInvoiceLines)
        {

            try
            {

                prepareInvoiceBeforePersistance(p_invoice);
                await AddAsync(p_invoice);

                if (p_invoice.InvoiceCategory != enInvoiceCategory.AGGREGATE.ToString())
                {
                    //gyűjtőszámla esetén már nem mozog a készlet...
                    var stockList = await _stockRepository.MaintainStockByInvoiceAsync(p_invoice);
                }

                //Kapcsolt szállítólevelek PendingDNQuantity-k beaktualizálása
                // Gyűjtőszámla, mínuszos szállítók esetében
                //
                if (p_RelDNInvoiceLines.Count > 0)
                {
                    await _invoiceLineRepository.UpdateRangeAsync(p_RelDNInvoiceLines.Select(s => s.Value).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }

            return p_invoice;
        }

        public async Task<Invoice> UpdateInvoiceAsync(Invoice p_invoice, ICollection<SummaryByVatRate> delSummaryByVatrates)
        {
            try
            {
                prepareInvoiceBeforePersistance(p_invoice);
                _dbContext.Instance.Entry(p_invoice).State = EntityState.Modified;

                await UpdateAsync(p_invoice, true);

                if (delSummaryByVatrates != null)
                {
                    foreach (var entity in delSummaryByVatrates)
                    {
                        entity.VatRate = null;
                        _dbContext.Instance.Entry(entity).State = EntityState.Deleted;
                        _dbContext.Instance.Set<SummaryByVatRate>().Remove(entity);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return p_invoice;

        }

        public async Task<Entity> GetInvoiceAsync(long ID, invoiceQueryTypes invoiceQueryType = invoiceQueryTypes.full)
        {

            Invoice item = await GetInvoiceRecordAsync(ID, invoiceQueryType);

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, ID));
            }

            var itemModel = _mapper.Map<Invoice, GetInvoiceViewModel>(item);

            if (invoiceQueryType == invoiceQueryTypes.small)
            {
                itemModel.InvoiceLines.Clear();         //itt már nem kellenek a sorok. 
                itemModel.SummaryByVatRates.Clear();         //itt már nem kellenek a sorok. 
                itemModel.InvPayments.Clear();         //itt már nem kellenek a sorok. 
            }
            var listFieldsModel = _modelHelper.GetModelFields<GetInvoiceViewModel>();

            // shape data
            var shapedData = _dataShaperGetInvoiceViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapedData;
        }
        public async Task<Entity> GetInvoiceByInvoiceNumberAsync(string invoiceNumber, invoiceQueryTypes invoiceQueryType = invoiceQueryTypes.full)
        {

            Invoice item = await GetInvoiceRecordByInvoiceNumberAsync(invoiceNumber);

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND2, invoiceNumber));
            }

            var itemModel = _mapper.Map<Invoice, GetInvoiceViewModel>(item);

            if (invoiceQueryType == invoiceQueryTypes.small)
            {
                itemModel.InvoiceLines.Clear();         //itt már nem kellenek a sorok. 
                itemModel.SummaryByVatRates.Clear();         //itt már nem kellenek a sorok. 
                itemModel.InvPayments.Clear();         //itt már nem kellenek a sorok. 
            }
            var listFieldsModel = _modelHelper.GetModelFields<GetInvoiceViewModel>();

            // shape data
            var shapedData = _dataShaperGetInvoiceViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapedData;
        }
        public async Task<Entity> GetAggregateInvoiceAsync(long ID)
        {

            Invoice item = await GetInvoiceRecordAsync(ID, invoiceQueryTypes.full);

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, ID));
            }
            var itemModel = _mapper.Map<Invoice, GetAggregateInvoiceViewModel>(item);
            var DeliveryNotes = item.InvoiceLines
                .GroupBy(g => new
                {
                    RelDeliveryNoteInvoiceID = g.RelDeliveryNoteInvoiceID,
                    RelDeliveryNoteNumber = g.RelDeliveryNoteNumber,
                    LineDeliveryDate = g.LineDeliveryDate,
                    DeliveryNoteDiscountPercent = g.DeliveryNote != null ? g.DeliveryNote.InvoiceDiscountPercent : 0
                }, (k, g) =>
                new GetAggregateInvoiceDeliveryNoteViewModel
                {
                    DeliveryNoteInvoiceID = k.RelDeliveryNoteInvoiceID,
                    DeliveryNoteNumber = k.RelDeliveryNoteNumber,
                    DeliveryNoteDate = k.LineDeliveryDate,
                    DeliveryNoteNetAmount = Math.Round(g.Sum(s => s.LineNetAmount), 1),
                    DeliveryNoteNetAmountHUF = Math.Round(g.Sum(s => s.LineNetAmountHUF), 1),
                    DeliveryNoteDiscountPercent = k.DeliveryNoteDiscountPercent,
                    DeliveryNoteDiscountAmount = Math.Round(g.Sum(s => s.LineNetAmount - s.LineNetDiscountedAmount), 1),
                    DeliveryNoteDiscountAmountHUF = Math.Round(g.Sum(s => s.LineNetAmountHUF - s.LineNetDiscountedAmountHUF), 1),

                    DeliveryNoteDiscountedNetAmount = Math.Round(g.Sum(s => s.LineNetDiscountedAmount), 1),
                    DeliveryNoteDiscountedNetAmountHUF = Math.Round(g.Sum(s => s.LineNetDiscountedAmountHUF), 1),
                    InvoiceLines = _mapper.Map<List<InvoiceLine>, List<GetAggregateInvoiceDeliveryNoteViewModel.InvoiceLine>>(g.ToList())
                }).ToList();
            itemModel.DeliveryNotes = DeliveryNotes;
            itemModel.DeliveryNotesCount = DeliveryNotes.GroupBy(g => g.DeliveryNoteInvoiceID).Count();

            var listFieldsModel = _modelHelper.GetModelFields<GetAggregateInvoiceViewModel>();

            // shape data
            var shapedData = _dataShaperGetAggregateInvoiceViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapedData;
        }
        public async Task<Invoice> GetInvoiceRecordAsync(long ID, invoiceQueryTypes invoiceQueryType = invoiceQueryTypes.full)
        {

            Invoice item = null;
            switch (invoiceQueryType)
            {
                case (invoiceQueryTypes.small):
                    item = await getSmallInvoiceQuery()
                      .Where(x => x.ID == ID).AsNoTracking().FirstOrDefaultAsync();
                    break;
                case (invoiceQueryTypes.full):
                    item = await getFullInvoiceQuery()
                      .Where(x => x.ID == ID).AsNoTracking().FirstOrDefaultAsync();
                    break;
                case (invoiceQueryTypes.NAV):
                    item = await getNAVInvoiceQuery()
                      .Where(x => x.ID == ID).AsNoTracking().FirstOrDefaultAsync();
                    break;
            }
            return item;
        }
        public async Task<Invoice> GetInvoiceRecordByInvoiceNumberAsync(string invoiceNumber, invoiceQueryTypes invoiceQueryType = invoiceQueryTypes.full)
        {

            Invoice item = null;
            switch (invoiceQueryType)
            {
                case (invoiceQueryTypes.small):
                    item = await getSmallInvoiceQuery()
                                .Where(x => x.InvoiceNumber == invoiceNumber && !x.Deleted).AsNoTracking().FirstOrDefaultAsync();
                    break;
                case (invoiceQueryTypes.full):
                    item = await getFullInvoiceQuery()
                            .Where(x => x.InvoiceNumber == invoiceNumber && !x.Deleted).AsNoTracking().FirstOrDefaultAsync();
                    break;
                case (invoiceQueryTypes.NAV):
                    item = await getNAVInvoiceQuery()
                        .Where(x => x.InvoiceNumber == invoiceNumber && !x.Deleted).AsNoTracking().FirstOrDefaultAsync();
                    break;
            }
            return item;
        }
        public async Task<Dictionary<long, Invoice>> GetInvoiceRecordsByInvoiceLinesAsync(List<long> LstInvoiceLineID)
        {

            var q = _dbContext.InvoiceLine.AsNoTracking()
                .Include(i => i.Invoice).AsNoTracking()
                .Where(x => LstInvoiceLineID.Any(a => a == x.ID) && !x.Invoice.Deleted && !x.Deleted)
                .Select(s => new { inv = s.Invoice, lineID = s.ID }).Distinct();
            var invoices = await q.ToListAsync();


            return invoices.ToDictionary(k => k.lineID, i => i.inv);
        }

        public async Task<List<Invoice>> GetCorrectionInvoiceRecordsByInvoiceNumber(string invoiceNumber)
        {
            var q = getFullInvoiceQuery()
                  .Where(x => x.OriginalInvoiceNumber == invoiceNumber && !x.Deleted);
            return await q.ToListAsync();
        }

        public async Task<List<Invoice>> GetCorrectionInvoiceRecordsByInvoiceID(long invoiceID)
        {
            var q = getFullInvoiceQuery()
                    .Where(x => x.OriginalInvoiceID == invoiceID && !x.Deleted);
            return await q.ToListAsync();
        }

        private IQueryable<Invoice> getSmallInvoiceQuery()
        {
            return _dbContext.Invoice
                  .Include(w => w.Warehouse).AsNoTracking()
                  .Include(s => s.Supplier).AsNoTracking()
                  .Include(c => c.Customer).AsNoTracking()
                  .Include(a => a.AdditionalInvoiceData).AsNoTracking()
                  .Include(i => i.InvoiceLines).AsNoTracking();                   //nem full data esetén is szüség van az invoiceLines-re
        }

        private IQueryable<Invoice> getFullInvoiceQuery()
        {
            return getSmallInvoiceQuery()
                 .Include(i => i.InvoiceLines).ThenInclude(t => t.VatRate).AsNoTracking()
                 .Include(i => i.InvoiceLines).ThenInclude(x => x.AdditionalInvoiceLineData).AsNoTracking()
                 .Include(i => i.InvoiceLines).ThenInclude(x => x.DeliveryNote).AsNoTracking()
                 .Include(i => i.InvoiceLines).ThenInclude(x => x.Product).ThenInclude(x => x.ProductGroup).AsNoTracking()
                 .Include(a => a.SummaryByVatRates).ThenInclude(t => t.VatRate).AsNoTracking()
                 .Include(a => a.InvPayments).AsNoTracking()
                 .Include(u => u.User).AsNoTracking();
        }

        private IQueryable<Invoice> getNAVInvoiceQuery()
        {
            return getFullInvoiceQuery()
                 .Include(i => i.NAVXChanges).ThenInclude(x => x.NAVXResults).AsNoTracking();
        }
        public async Task<decimal> GetUnPaidAmountAsyn(long customerID)
        {
            //1. kimenő szállítólevélen lévő rendezetlen összeg
            // Megj: groupoljuk bizonylatonként, hogy a bizonylatkedvezmény bzonylatonként kerülkön a függő tételekre
            //
            var q1 = from InvoiceLine in _dbContext.InvoiceLine
                     join Invoice in _dbContext.Invoice on InvoiceLine.InvoiceID equals Invoice.ID
                     where InvoiceLine.PendingDNQuantity > 0
                        && Invoice.CustomerID == customerID
                        && !Invoice.Incoming
                        && Invoice.InvoiceType == enInvoiceType.DNO.ToString()
                     group InvoiceLine by
                     new
                     {
                         InvoiceID = Invoice.ID,
                         InvoiceDiscountPercent = Invoice.InvoiceDiscountPercent
                     }
                 into grpInner
                     select new
                     {
                         InvoiceID = grpInner.Key.InvoiceID,
                         InvoiceDiscountPercent = grpInner.Key.InvoiceDiscountPercent,
                         Cnt = grpInner.Count(),
                         SumNetAmountDiscountedHUF = Math.Round(grpInner.Sum(s => s.PendingDNQuantity * s.UnitPriceHUF) * (1 - grpInner.Key.InvoiceDiscountPercent / 100), 1)
                     };

            decimal pendingAmount = await q1.SumAsync(s => s.SumNetAmountDiscountedHUF);

            //2. kiegyenlítettlen számlák???

            return pendingAmount;
        }

        private IQueryable<GetPendigDeliveryNotesModel> getPendigDeliveryNotesQuery(bool incoming, long warehouseID)
        {
            IQueryable<GetPendigDeliveryNotesModel> q1;

            //1. groupolunk ügyfélre és PriceReview típusra
            //
            if (incoming)
            {
                //először grouo-olunk ccustomerre és számlákra, hogy a különböző kedvezményekkel 
                //kiszámoljuk a summákat
                //
                q1 = from InvoiceLine in _dbContext.InvoiceLine
                     join Invoice in _dbContext.Invoice on InvoiceLine.InvoiceID equals Invoice.ID
                     join Customer in _dbContext.Customer on Invoice.SupplierID equals Customer.ID
                     where InvoiceLine.PendingDNQuantity > 0
                        && Invoice.Incoming == incoming
                        && Invoice.WarehouseID == warehouseID
                        && Invoice.InvoiceType == enInvoiceType.DNI.ToString()
                     group InvoiceLine by
                     new
                     {
                         CustomerID = Customer.ID,
                         CustomerName = Customer.CustomerName,
                         FullAddress = (Customer.PostalCode + " " + Customer.City + " " + Customer.AdditionalAddressDetail).Trim(),
                         InvoiceID = Invoice.ID,
                         InvoiceNumber = Invoice.InvoiceNumber,
                         InvoiceDeliveryDate = Invoice.InvoiceDeliveryDate,
                         InvoiceDiscountPercent = Invoice.InvoiceDiscountPercent,
                         CurrencyCode = Invoice.CurrencyCode
                     }
                         into grpInner
                     select new GetPendigDeliveryNotesModel()
                     {

                         WarehouseID = warehouseID,
                         InvoiceID = grpInner.Key.InvoiceID,
                         InvoiceNumber = grpInner.Key.InvoiceNumber,
                         InvoiceDeliveryDate = grpInner.Key.InvoiceDeliveryDate,
                         CustomerID = grpInner.Key.CustomerID,
                         Customer = grpInner.Key.CustomerName,
                         FullAddress = grpInner.Key.FullAddress,
                         CurrencyCode = grpInner.Key.CurrencyCode,
                         PriceReview = grpInner.Where(w => w.PriceReview.HasValue && w.PriceReview.Value).Count() > 0,
                         SumNetAmount = grpInner.Sum(s => Math.Round(s.PendingDNQuantity * s.UnitPrice, 1)),
                         SumNetAmountDiscounted = Math.Round(grpInner.Sum(s => s.PendingDNQuantity * s.UnitPrice) * (1 - grpInner.Key.InvoiceDiscountPercent / 100), 1)
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
                     group InvoiceLine by
                     new
                     {
                         CustomerID = Customer.ID,
                         CustomerName = Customer.CustomerName,
                         FullAddress = (Customer.PostalCode + " " + Customer.City + " " + Customer.AdditionalAddressDetail).Trim(),
                         InvoiceID = Invoice.ID,
                         InvoiceNumber = Invoice.InvoiceNumber,
                         InvoiceDeliveryDate = Invoice.InvoiceDeliveryDate,
                         InvoiceDiscountPercent = Invoice.InvoiceDiscountPercent,
                         CurrencyCode = Invoice.CurrencyCode
                     }
                        into grpInner
                     select new GetPendigDeliveryNotesModel()
                     {

                         WarehouseID = warehouseID,
                         InvoiceID = grpInner.Key.InvoiceID,
                         InvoiceNumber = grpInner.Key.InvoiceNumber,
                         InvoiceDeliveryDate = grpInner.Key.InvoiceDeliveryDate,
                         CustomerID = grpInner.Key.CustomerID,
                         Customer = grpInner.Key.CustomerName,
                         FullAddress = grpInner.Key.FullAddress,
                         CurrencyCode = grpInner.Key.CurrencyCode,
                         PriceReview = grpInner.Where(w => w.PriceReview.HasValue && w.PriceReview.Value).Count() > 0,
                         SumNetAmount = grpInner.Sum(s => Math.Round(s.PendingDNQuantity * s.UnitPrice, 1)),
                         SumNetAmountDiscounted = Math.Round(grpInner.Sum(s => s.PendingDNQuantity * s.UnitPrice) * (1 - grpInner.Key.InvoiceDiscountPercent / 100), 1)
                     };
            }
            return q1;
        }

        public async Task<IEnumerable<Entity>> GetPendigDeliveryNotesSummaryAsync(bool incoming, long warehouseID)
        {

            var lstEntities = new List<GetPendigDeliveryNotesSummaryModel>();

            //1. groupolunk ügyfélre,  és PriceReview típusra és szállítóra
            //
            var q1 = getPendigDeliveryNotesQuery(incoming, warehouseID);


            //q1-et még egyszer meggroupoljuk, a számlák alapján számított összegeket summáuuzk
            //a tételsorokben lévő PriceReview miatt nested groupra van szükség
            //
            var q2 = from res in q1
                     group res by
                     new { CustomerID = res.CustomerID, Customer = res.Customer, FullAddress = res.FullAddress, CurrencyCode = res.CurrencyCode }
                     into grpOuter
                     select new GetPendigDeliveryNotesSummaryModel()
                     {
                         WarehouseID = warehouseID,
                         CustomerID = grpOuter.Key.CustomerID,
                         Customer = grpOuter.Key.Customer,
                         FullAddress = grpOuter.Key.FullAddress,
                         PriceReview = grpOuter.Count(c => c.PriceReview) > 0,          //van-e ProcePreview-es tétel  a groupban?
                         SumNetAmount = Math.Round(grpOuter.Sum(s => s.SumNetAmount)),
                         SumNetAmountDiscounted = Math.Round(grpOuter.Sum(s => s.SumNetAmountDiscounted)),
                         CurrencyCode = grpOuter.Key.CurrencyCode,

                     };
            q2 = q2.OrderBy(o => o.Customer);

            lstEntities = await q2.ToListAsync();
            lstEntities.ForEach(i =>
                {
                    i.SumNetAmount = Math.Round(i.SumNetAmount, 1);
                    i.CurrencyCodeX = Common.Utils.GetEnumDescription(
                            (enCurrencyCodes)Enum.Parse(typeof(enCurrencyCodes), i.CurrencyCode));
                }
                );

            var shapedData = _dataShaperGetPendigDeliveryNotesSummaryModel.ShapeData(lstEntities, "");

            return shapedData;
        }

        public async Task<IEnumerable<Entity>> GetPendigDeliveryNotesAsync(bool incoming, long warehouseID, string currencyCode)
        {

            var lstEntities = new List<GetPendigDeliveryNotesModel>();

            var q1 = getPendigDeliveryNotesQuery(incoming, warehouseID);
            q1 = q1.Where(w => w.CurrencyCode == currencyCode);
            q1 = q1.OrderBy(o => o.InvoiceNumber);

            lstEntities = await q1.ToListAsync();
            lstEntities.ForEach(i => i.SumNetAmount = Math.Round(i.SumNetAmount, 1));

            var shapedData = _dataShaperGetPendigDeliveryNotesModel.ShapeData(lstEntities, "");

            return shapedData;
        }
        public async Task<IEnumerable<Entity>> GetPendigDeliveryNotesItemsAsync(bool incoming, long warehouseID, long customerID, string currencyCode)
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
            var resultDataModel = new List<GetPendigDeliveryNotesItemModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<InvoiceLine, GetPendigDeliveryNotesItemModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetPendigDeliveryNotesItemModel>();

            var shapedData = _dataShaperGetPendigDeliveryNotesItemModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return shapedData;
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
                query = getFullInvoiceQuery();
            }
            else
            {
                query = getSmallInvoiceQuery()
                 .Include(u => u.User).AsNoTracking();
            }


            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data

            FilterQueryBy(ref query, requestParameter.InvoiceType, requestParameter.WarehouseCode, requestParameter.InvoiceNumber,
                    requestParameter.InvoiceIssueDateFrom, requestParameter.InvoiceIssueDateTo,
                    requestParameter.InvoiceDeliveryDateFrom, requestParameter.InvoiceDeliveryDateTo, requestParameter.CustomerID);


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
                    im.InvPayments.Clear();         //itt már nem kellenek a sorok. 
                }
                resultDataModel.Add(im);  //nem full data esetén is szüség van az invoiceLines-re
            }
            );

            var listFieldsModel = _modelHelper.GetModelFields<GetInvoiceViewModel>();

            var shapedData = _dataShaperGetInvoiceViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapedData, recordsCount);
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedUnpaidInvoiceAsync(QueryUnpaidInvoice requestParameter)
        {

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetInvoiceViewModel, Invoice>();


            int recordsTotal, recordsFiltered;


            var query = getFullInvoiceQuery()
                .Include(u => u.User).AsNoTracking();


            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data

            UnpaidFilterBy(ref query, requestParameter.Incoming,
                requestParameter.InvoiceNumber, requestParameter.CustomerInvoiceNumber, requestParameter.CustomerID,
             requestParameter.InvoiceIssueDateFrom, requestParameter.InvoiceIssueDateTo,
             requestParameter.InvoiceDeliveryDateFrom, requestParameter.InvoiceDeliveryDateTo,
             requestParameter.PaymentDateFrom, requestParameter.PaymentDateTo,
             requestParameter.Expired);

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
                im.InvoiceLines.Clear();         //itt már nem kellenek a sorok. 
                im.SummaryByVatRates.Clear();         //itt már nem kellenek a sorok. 
                im.InvPayments.Clear();         //itt már nem kellenek a sorok. 
                resultDataModel.Add(im);  //nem full data esetén is szüség van az invoiceLines-re
            }
            );

            var listFieldsModel = _modelHelper.GetModelFields<GetInvoiceViewModel>();

            var shapedData = _dataShaperGetInvoiceViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapedData, recordsCount);
        }

        public async Task<IList<GetInvoiceViewModel>> QueryForCSVInvoiceAsync(CSVInvoice requestParameter)
        {

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetInvoiceViewModel, Invoice>();

            //var query = _dbContext.Invoice//.AsNoTracking().AsExpandable()
            //        .Include(i => i.Warehouse).AsQueryable();

            IQueryable<Invoice> query;
            query = getFullInvoiceQuery()
                .Include(u => u.User).AsNoTracking();


            // filter data

            FilterQueryBy(ref query, requestParameter.InvoiceType, requestParameter.WarehouseCode, requestParameter.InvoiceNumber,
                    requestParameter.InvoiceIssueDateFrom, requestParameter.InvoiceIssueDateTo,
                    requestParameter.InvoiceDeliveryDateFrom, requestParameter.InvoiceDeliveryDateTo,
                    requestParameter.CustomerID);

            // set order by
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                query = query.OrderBy(orderBy);
            }

            // retrieve data to list
            List<Invoice> resultData = await query.ToListAsync();
            var resultDataModel = new List<GetInvoiceViewModel>();
            resultData.ForEach(i =>
            {
                var im = _mapper.Map<Invoice, GetInvoiceViewModel>(i);
                resultDataModel.Add(im);  //nem full data esetén is szüség van az invoiceLines-re
            }
            );

            return resultDataModel;
        }

        private void FilterQueryBy(ref IQueryable<Invoice> p_items, string invoiceType, string WarehouseCode, string InvoiceNumber,
                                DateTime? InvoiceIssueDateFrom, DateTime? InvoiceIssueDateTo,
                                DateTime? InvoiceDeliveryDateFrom, DateTime? InvoiceDeliveryDateTo,
                                long? CustomerID)
        {
            if (!p_items.Any())
                return;


            var predicate = PredicateBuilder.New<Invoice>();

            predicate = predicate.And(p => p.InvoiceType == invoiceType
                            && (WarehouseCode == null || p.Warehouse.WarehouseCode.ToUpper() == WarehouseCode)
                            && (InvoiceNumber == null || p.InvoiceNumber.Contains(InvoiceNumber))
                            && (!InvoiceIssueDateFrom.HasValue || p.InvoiceIssueDate >= InvoiceIssueDateFrom.Value)
                            && (!InvoiceIssueDateTo.HasValue || p.InvoiceIssueDate <= InvoiceIssueDateTo.Value)
                            && (!InvoiceDeliveryDateFrom.HasValue || p.InvoiceDeliveryDate >= InvoiceDeliveryDateFrom.Value)
                            && (!InvoiceDeliveryDateTo.HasValue || p.InvoiceDeliveryDate <= InvoiceDeliveryDateTo.Value)
                            && (!CustomerID.HasValue || p.SupplierID == CustomerID && p.Incoming || p.CustomerID == CustomerID && !p.Incoming)
                           );

            p_items = p_items.Where(predicate);
        }

        private void UnpaidFilterBy(ref IQueryable<Invoice> p_items, bool p_incoming,
            string p_invoiceNumber, string p_customerInvoiceNumber, long? p_customerID,
            DateTime p_invoiceIssueDateFrom, DateTime? p_invoiceIssueDateTo,
            DateTime? p_invoiceDeliveryDateFrom, DateTime? p_invoiceDeliveryDateTo,
            DateTime? p_paymentDateFrom, DateTime? p_paymentDateTo,
            bool? p_expired)
        {
            if (!p_items.Any())
                return;


            var predicate = PredicateBuilder.New<Invoice>();

            predicate = predicate.And(p =>
                            //kötelező mezők, indexbe
                            p.Incoming == p_incoming && p.PaymentMethod == PaymentMethodType.TRANSFER.ToString() && p.InvPayments.Sum(s => s.InvPaymentAmountHUF) < p.InvoiceGrossAmountHUF
                            && (p.InvoiceIssueDate >= p_invoiceIssueDateFrom)
                            //kötelező mezők
                            && (string.IsNullOrWhiteSpace(p_invoiceNumber) || p.InvoiceNumber.Contains(p_invoiceNumber))
                            && (string.IsNullOrWhiteSpace(p_customerInvoiceNumber) || p.CustomerInvoiceNumber.ToUpper().Contains(p_customerInvoiceNumber.ToUpper()))
                            && (!p_customerID.HasValue || (p.Incoming && p.SupplierID == p_customerID) || (!p.Incoming && p.CustomerID == p_customerID))
                            && (!p_invoiceIssueDateTo.HasValue || p.InvoiceIssueDate <= p_invoiceIssueDateTo.Value)
                            && (!p_invoiceDeliveryDateFrom.HasValue || p.InvoiceDeliveryDate >= p_invoiceDeliveryDateFrom.Value)
                            && (!p_invoiceDeliveryDateTo.HasValue || p.InvoiceDeliveryDate <= p_invoiceDeliveryDateTo.Value)
                            && (!p_paymentDateFrom.HasValue || p.PaymentDate >= p_paymentDateFrom.Value)
                            && (!p_paymentDateTo.HasValue || p.PaymentDate <= p_paymentDateTo.Value)
                            && (!p_expired.HasValue || p.PaymentDate < DateTime.UtcNow.Date)
                           );

            p_items = p_items.Where(predicate);
        }


        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

        public async Task<(IList<GetCustomerInvoiceSummary> data, RecordsCount recordsCount)> QueryPagedCustomerInvoiceSummaryAsync(QueryCustomerInvoiceSummary requestParameters)
        {
            int recordsTotal, recordsFiltered;

            var fields = _modelHelper.GetQueryableFields<QueryCustomerInvoiceSummary, Invoice>();

            //var query = _dbContext.Invoice//.AsNoTracking().AsExpandable()
            //        .Include(i => i.Warehouse).AsQueryable();

            IQueryable<Invoice> query;
            query = _dbContext.Invoice.AsNoTracking()
                .Include(w => w.Warehouse).AsNoTracking()
                .Include(s => s.Supplier).AsNoTracking()
                .Include(c => c.Customer).AsNoTracking()
                .Where(p => p.InvoiceCategory == enInvoiceCategory.NORMAL.ToString()
                                    && p.InvoiceType == (p.Incoming ? enInvoiceType.INC.ToString() : enInvoiceType.INV.ToString()));

            recordsTotal = await query.CountAsync();

            // filter data

            FilterCustomerInvoiceSummary(ref query, requestParameters.Incoming, requestParameters.CustomerID,
                requestParameters.WarehouseCode, requestParameters.InvoiceDeliveryDateFrom, requestParameters.InvoiceDeliveryDateTo);

            recordsFiltered = await query.CountAsync();


            var q2 = from inv in query
                     group inv by
            new
            {
                Incoming = inv.Incoming,
                CustomerID = inv.Incoming ? inv.SupplierID : inv.CustomerID.Value,
                CustomerName = inv.Incoming ? inv.Supplier.CustomerName : inv.Customer.CustomerName,
                CustomerFullAddress = (inv.Incoming ?
                        (inv.Supplier.PostalCode + " " + inv.Supplier.City + " " + inv.Supplier.AdditionalAddressDetail).Trim() :
                        (inv.Customer.PostalCode + " " + inv.Customer.City + " " + inv.Customer.AdditionalAddressDetail).Trim())
            }
            into grp
                     select new GetCustomerInvoiceSummary()
                     {
                         Incoming = grp.Key.Incoming,
                         CustomerID = grp.Key.CustomerID,
                         CustomerName = grp.Key.CustomerName,
                         CustomerFullAddress = grp.Key.CustomerFullAddress,
                         InvoiceCount = grp.Count(),
                         InvoiceDiscountSum = grp.Sum(s => s.InvoiceDiscount),
                         InvoiceDiscountHUFSum = grp.Sum(s => s.InvoiceDiscountHUF),
                         InvoiceNetAmountSum = grp.Sum(s => s.InvoiceNetAmount),
                         InvoiceNetAmountHUFSum = grp.Sum(s => s.InvoiceNetAmountHUF),
                         InvoiceVatAmountSum = grp.Sum(s => s.InvoiceVatAmount),
                         InvoiceVatAmountHUFSum = grp.Sum(s => s.InvoiceVatAmountHUF),
                         InvoiceGrossAmountSum = grp.Sum(s => s.InvoiceGrossAmount),
                         InvoiceGrossAmountHUFSum = grp.Sum(s => s.InvoiceGrossAmountHUF)
                     };

            if (string.IsNullOrWhiteSpace(requestParameters.OrderBy))
            {
                q2 = q2.OrderBy(o => o.CustomerName);
            }
            else
            {
                q2 = q2.OrderBy(requestParameters.OrderBy);
            }

            // paging
            q2 = q2.Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
                .Take(requestParameters.PageSize);
            List<GetCustomerInvoiceSummary> resultData = await q2.ToListAsync();


            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };
            return (resultData, recordsCount);
        }

        private void FilterCustomerInvoiceSummary(ref IQueryable<Invoice> p_items,
              bool incoming, long? customerID, string warehouseCode, DateTime? invoiceDeliveryDateFrom, DateTime? invoiceDeliveryDateTo)
        {
            if (!p_items.Any())
                return;

            var predicate = PredicateBuilder.New<Invoice>();

            predicate = predicate.And(p => p.Incoming == incoming
                            && (!customerID.HasValue || (p.Incoming && p.SupplierID == customerID) || (!p.Incoming && p.CustomerID == customerID))
                            && (warehouseCode == null || p.Warehouse.WarehouseCode.ToUpper() == warehouseCode)
                            && (!invoiceDeliveryDateFrom.HasValue || p.InvoiceDeliveryDate >= invoiceDeliveryDateFrom.Value)
                            && (!invoiceDeliveryDateTo.HasValue || p.InvoiceDeliveryDate <= invoiceDeliveryDateTo.Value)
                           );

            p_items = p_items.Where(predicate);
        }


        public async Task<Invoice> CreateInvoiceAsynch(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = _mapper.Map<Invoice>(request);
            var deliveryNotes = new Dictionary<int, Invoice>();
            var counterCode = "";
            var updatingProducts = new List<Product>();

            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {

                    var paymentMethod = (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType), request.PaymentMethod);
                    var invoiceType = (enInvoiceType)Enum.Parse(typeof(enInvoiceType), request.InvoiceType);

                    /*****************************************/
                    /* Mentés előtt Invoice mezők feltöltése */
                    /*****************************************/


                    //ID-k feloldása
                    if (string.IsNullOrWhiteSpace(request.WarehouseCode))
                    {
                        request.WarehouseCode = bbxBEConsts.DEF_WAREHOUSE;      //Átmenetileg
                    }

                    if (string.IsNullOrWhiteSpace(request.CurrencyCode))
                    {
                        request.CurrencyCode = enCurrencyCodes.HUF.ToString();
                    }
                    var wh = await _warehouseRepository.GetWarehouseByCodeAsync(request.WarehouseCode);
                    if (wh == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.WarehouseCode));
                    }
                    invoice.WarehouseID = wh.ID;

                    Customer cust = null;

                    if (invoiceType != enInvoiceType.BLK)
                    {
                        cust = _customerRepository.GetCustomerRecord(request.CustomerID.Value);
                        if (cust == null)
                        {
                            throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_CUSTOMERNOTFOUND, request.CustomerID.Value));
                        }
                    }

                    var ownData = _customerRepository.GetOwnData();
                    if (ownData == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_OWNNOTFOUND));
                    }

                    if (invoiceType != enInvoiceType.BLK)
                    {
                        if (request.Incoming)
                        {
                            invoice.SupplierID = request.CustomerID.Value;
                            invoice.CustomerID = ownData.ID;
                        }
                        else
                        {
                            invoice.SupplierID = ownData.ID;
                            invoice.CustomerID = request.CustomerID.Value;
                        }
                    }
                    else
                    {
                        invoice.SupplierID = ownData.ID;
                        invoice.CustomerID = null;
                    }

                    var RelDeliveryNotesByLineID = new Dictionary<long, Invoice>();
                    var RelDeliveryNoteLines = new Dictionary<long, InvoiceLine>();


                    //Kezelni kell-e kapcsolt szállítóleveleket?
                    //	- gyűjtőszámla esetén
                    //	- Korrekciós be- ill. kimenő számla esetén
                    //
                    var hasRelDeliveryNotes = (request.InvoiceCategory == enInvoiceCategory.AGGREGATE.ToString() ||
                                            (invoiceType == enInvoiceType.DNI || invoiceType == enInvoiceType.DNO)
                                            && (request.InvoiceCorrection.HasValue && request.InvoiceCorrection.Value));


                    if (hasRelDeliveryNotes)
                    {
                        var RelDeliveryNoteLineIDs = request.InvoiceLines.GroupBy(g => g.RelDeliveryNoteInvoiceLineID)
                                .Select(s => s.Key.Value).ToList();
                        RelDeliveryNotesByLineID = await this.GetInvoiceRecordsByInvoiceLinesAsync(RelDeliveryNoteLineIDs);

                        RelDeliveryNoteLines = await _invoiceLineRepository.GetInvoiceLineRecordsAsync(
                            request.InvoiceLines.Select(s => s.RelDeliveryNoteInvoiceLineID.Value).ToList());
                    }

                    int LineNumberReference = 1;

                    //Javítószámla

                    /* Jóváírás-kezelés:
                      Adózó módosító számlát bocsát ki, ezen módosító tételként -4 darab „D termék”,
                      illetve 1 darab „F termék” szerepel. Ezen módosításról történő adatszolgáltatásban az adózó újabb
                      tételként szerepelteti a -4 db „D terméket”, illetve az 1 db „F terméket”.
                      Ezen módosító okiratról történő adatszolgáltatásban:
                      a) A módosító okiratot leíró XML első tételsorában (lineNumber=1) a LineModificationReference
                      elemben a lineNumberReference elem értéke „6”, lineOperation elem értéke „CREATE”, ez
                      tartalmazza a -4 darab „D termék” adatait.NAV Online Számla Rendszer 100. oldal
                      b) A módosító okiratot leíró XML második tételsorában (lineNumber=2) a LineModificationReference
                      elemben a lineNumberReference elem értéke „7”, lineOperation elem értéke „CREATE”, ez
                      tartalmazza az 1 darab „F termék” adatait.
                      c) A módosító okiratot leíró XML invoiceSummary eleme teljes egészében szerepel, abban az egyes
                      értékek módosulásának előjeles összege szerepel.

                      RÖVIDEN : A jóváíró számlával az eredeti számlatételekhez "hozzáírjuk" a javító tételeket mínuszosan.
                      */

                    List<Invoice> ModificationInvoices = new List<Invoice>();
                    Invoice OriginalInvoice = null;
                    var isInvoiceCorrection = (request.OriginalInvoiceID.HasValue && request.OriginalInvoiceID.Value != 0)
                                                && (request.InvoiceCorrection.HasValue && request.InvoiceCorrection.Value);
                    if (isInvoiceCorrection)
                    {
                        OriginalInvoice = await this.GetInvoiceRecordAsync(request.OriginalInvoiceID.Value);
                        if (OriginalInvoice == null)
                        {
                            throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_ORIGINALINVOICENOTFOUND, request.OriginalInvoiceID.Value));
                        }

                        //API lerírás: a lineNumberReference(a számla és összes módosításaiban) sorfolytonosan új tételsorszámra mutat és lineOperation értéke „CREATE”.
                        //

                        // Javító bizonylat ReferenceLineNumber indulóérték meghatározás
                        //      - az eredeti bizonylat linenumber utáni tétel
                        //        + engedmény esetén áfakódonként 1 (az engedmény a NAV-hoz áfánként, tételsorokban van felküldve) sor készül a  NAV-hoz felküldött adatokban
                        //      - már elkészült javítószámlák tétel összesen
                        //        + engedmény esetén már elkészült javítószámlák áfakódonként 1 (az engedmény a NAV-hoz áfánként, tételsorokban van felküldve)

                        // eredeti számla
                        LineNumberReference += OriginalInvoice.InvoiceLines.Count()
                                         + (OriginalInvoice.InvoiceLines.Where(a => a.LineDiscountPercent != 0).GroupBy(g => g.VatRateID).Count());

                        // már elkészült javítószámlák (láncolt javítás)
                        ModificationInvoices = await this.GetCorrectionInvoiceRecordsByInvoiceID(request.OriginalInvoiceID.Value);
                        ModificationInvoices.ForEach(oi =>
                        {
                            LineNumberReference += oi.InvoiceLines.Count()
                                         + (oi.InvoiceLines.Where(a => a.LineDiscountPercent != 0).GroupBy(g => g.VatRateID).Count());
                        });

                        invoice.OriginalInvoiceNumber = OriginalInvoice.InvoiceNumber;
                        invoice.ModificationIndex = (short)(ModificationInvoices.Count() + 1);
                        invoice.ModifyWithoutMaster = false;
                        invoice.InvoiceCategory = OriginalInvoice.InvoiceCategory;
                    }

                    //Megjegyzés
                    if (!string.IsNullOrWhiteSpace(request.Notice))
                    {
                        invoice.AdditionalInvoiceData = new List<AdditionalInvoiceData>() {  new AdditionalInvoiceData()
                            { DataName = bbxBEConsts.DEF_NOTICE, DataDescription = bbxBEConsts.DEF_NOTICEDESC, DataValue = request.Notice }};

                    }


                    //Szállítólevél esetén a PaymentMethod OTHER
                    if (invoiceType == enInvoiceType.DNI || invoiceType == enInvoiceType.DNO)
                    {
                        invoice.PaymentMethod = PaymentMethodType.OTHER.ToString();
                        paymentMethod = PaymentMethodType.OTHER;    //FE hiba miatt
                    }


                    //Tételsorok előfeldolgozása
                    var lineErrors = new List<string>();
                    foreach (var ln in invoice.InvoiceLines)
                    {
                        var rln = request.InvoiceLines.SingleOrDefault(i => i.LineNumber == ln.LineNumber);


                        var prod = _productRepository.GetProductByProductCode(rln.ProductCode);
                        if (prod == null)
                        {
                            throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODCODENOTFOUND, rln.ProductCode));
                        }


                        if (!hasRelDeliveryNotes &&         //nem gyűjtőszámla
                            invoice.Incoming &&             //bevételezés
                            (invoice.InvoiceType == enInvoiceType.INC.ToString() || invoice.InvoiceType == enInvoiceType.DNI.ToString()))   //szla.v.száll.
                        {
                            prod.LatestSupplyPrice = rln.UnitPrice;     //megjegzezük a legutolsó eladási árat
                            prod.UnitPrice1 = rln.NewUnitPrice1HUF;        //árváltozás, új listaár
                            prod.UnitPrice2 = rln.NewUnitPrice2HUF;        //árváltozás, új egységár

                            if (updatingProducts.Any(a => a.ID == prod.ID))
                            {
                                // az előző előfordulást töröljük, a "legutolsó" árak lesznek az érvényesek
                                updatingProducts.RemoveAll(r => r.ID == prod.ID);
                            }
                            updatingProducts.Add(prod);
                        }


                        var vatRate = _vatRateRepository.GetVatRateByCode(rln.VatRateCode);
                        if (vatRate == null)
                        {
                            throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_VATRATECODENOTFOUND, rln.VatRateCode));
                        }

                        if (vatRate.VatDomesticReverseCharge)
                        {
                            //Fordított adózás!
                            if (cust == null || !cust.IsFA)
                            {
                                //Ha az ügyfél nem fordított adózó, akkor 27%-os adókulcsra váltunk
                                vatRate = _vatRateRepository.GetVatRateByCode(bbxBEConsts.VATCODE_27);
                            }
                        }

                        ln.PriceReview = request.PriceReview;

                        //	Product
                        //
                        ln.ProductID = prod.ID;
                        ln.ProductCode = rln.ProductCode;
                        ln.Product = prod;


                        ln.VTSZ = prod.ProductCodes.FirstOrDefault(c => c.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString()).ProductCodeValue;
                        ln.LineDescription = prod.Description;
                        ln.UnitWeight = prod.UnitWeight;


                        ln.VatRate = vatRate;
                        ln.VatRateID = vatRate.ID;
                        ln.VatPercentage = vatRate.VatPercentage;

                        ln.LineNatureIndicator = prod.NatureIndicator;

                        if (hasRelDeliveryNotes)
                        {
                            //gyűjtőszámla
                            if (!ln.RelDeliveryNoteInvoiceLineID.HasValue || ln.RelDeliveryNoteInvoiceLineID.Value == 0)
                            {
                                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVAGGR_RELATED_NOT_ASSIGNED,
                                        invoice.InvoiceNumber, rln.LineNumber, rln.ProductCode));
                            }

                            //gyűjtőszámla esetén is egy árfolyam lesz!

                            if (!RelDeliveryNotesByLineID.ContainsKey(ln.RelDeliveryNoteInvoiceLineID.Value))
                            {
                                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVAGGR_RELATED_NOT_FOUND,
                                        invoice.InvoiceNumber, rln.LineNumber, rln.ProductCode, ln.RelDeliveryNoteInvoiceLineID));
                            }

                            var relDeliveryNote = RelDeliveryNotesByLineID[ln.RelDeliveryNoteInvoiceLineID.Value];
                            var relDeliveryNoteLine = RelDeliveryNoteLines[ln.RelDeliveryNoteInvoiceLineID.Value];

                            ln.RelDeliveryNoteNumber = relDeliveryNote.InvoiceNumber;
                            ln.RelDeliveryNoteInvoiceID = relDeliveryNote.ID;
                            ln.RelDeliveryNoteInvoiceLineID = ln.RelDeliveryNoteInvoiceLineID.Value;

                            ln.LineExchangeRate = invoice.ExchangeRate; //Gyűjtőszámla esetén minden LineExchangeRate a bizonylat árfolyamával kell megegyeznie! Ez nem jó: relDeliveryNote.ExchangeRate;
                            ln.LineDeliveryDate = relDeliveryNote.InvoiceDeliveryDate;

                            //Bizonylatkedvezmény a kapcsolt szállítólevél alapján
                            if (!prod.NoDiscount)
                            {
                                //NoDiscount a kapcsolt szállítólevél alapján van meghatáriza
                                ln.NoDiscount = relDeliveryNoteLine.NoDiscount;
                                ln.LineDiscountPercent = relDeliveryNoteLine.LineDiscountPercent;
                            }
                            else
                            {
                                //elég extrém helyzet, a szállítólevél adásakor még igen, de a gyűjtőszámla készítésekor
                                //már nem adható kedvezmény.
                                //
                                //TODO: erre ne legyen figyelmeztetés?
                                ln.NoDiscount = true;
                                ln.LineDiscountPercent = relDeliveryNoteLine.LineDiscountPercent;
                            }

                            //Gyűjtőszámla kiegészítő adatok
                            ln.AdditionalInvoiceLineData = new List<AdditionalInvoiceLineData>();
                            ln.AdditionalInvoiceLineData.Add(new AdditionalInvoiceLineData()   //a kapcsolt szállítólevél számát külön is letároljuk, hogy menjen fel a NAV-hoz
                            {
                                DataName = bbxBEConsts.DEF_RELDELIVERYNOTENAME,
                                DataDescription = bbxBEConsts.DEF_RELDELIVERYNOTE,
                                DataValue = relDeliveryNote.InvoiceNumber
                            });

                            ln.AdditionalInvoiceLineData.Add(new AdditionalInvoiceLineData()   //a kapcsolt szállítólevél engedményt is letároljuk, hogy menjen fel a NAV-hoz
                            {
                                DataName = bbxBEConsts.DEF_RELDELIVERYDISCOUNTPERCENTNAME,
                                DataDescription = bbxBEConsts.DEF_RELDELIVERYDISCOUNTPERCENT,
                                DataValue = ln.LineDiscountPercent.ToString()
                            });


                            //Szállítólevélen lévő függő mennyiség aktualizálása
                            if (relDeliveryNoteLine.PendingDNQuantity < Math.Abs(ln.Quantity))
                            {
                                throw new DataContextException(string.Format(bbxBEConsts.ERR_INVAGGR_WRONG_AGGR_QTY,
                                        relDeliveryNoteLine.Invoice.InvoiceNumber, rln.LineNumber, rln.ProductCode,
                                         Math.Abs(ln.Quantity), relDeliveryNoteLine.PendingDNQuantity,
                                        ln.RelDeliveryNoteInvoiceLineID));
                            }
                            relDeliveryNoteLine.PendingDNQuantity -= Math.Abs(ln.Quantity); // mínuszos szállítólevelek miatt kell az abszolút érték
                        }
                        else
                        {
                            ln.LineExchangeRate = invoice.ExchangeRate;
                            ln.LineDeliveryDate = invoice.InvoiceDeliveryDate;

                            //NoDiscount a cikktörzs alapján van meghatározva
                            ln.NoDiscount = prod.NoDiscount;

                            //Bizonylatkedvezmény a request alapján
                            if (!prod.NoDiscount)
                            {
                                ln.LineDiscountPercent = request.InvoiceDiscountPercent;
                            }
                        }

                        //Normál szállítólevél esetén a rendezetlen mennyiséget is feltöltjük
                        if ((invoiceType == enInvoiceType.DNI || invoiceType == enInvoiceType.DNO) &&
                            (!request.InvoiceCorrection.HasValue || !request.InvoiceCorrection.Value))        // Szállítólevél korrekció esetén nincs PendingDNQuantity
                        {
                            ln.PendingDNQuantity = ln.Quantity;
                        }

                        if (isInvoiceCorrection)
                        {
                            //Termékkód ell.
                            if (!OriginalInvoice.InvoiceLines.Any(w => w.ProductID == ln.ProductID))
                            {
                                lineErrors.Add(string.Format(bbxBEConsts.ERR_CORRECTIONUNKOWNPROD, ln.ProductID, ln.ProductCode));

                            }
                            else
                            {

                                //Mennyiség ell.
                                var origQty = OriginalInvoice.InvoiceLines.Where(w => w.ProductID == ln.ProductID).Sum(s => s.Quantity);
                                var modQty = ModificationInvoices.Sum(s => s.InvoiceLines.Where(w => w.ProductID == ln.ProductID).Sum(s => s.Quantity));
                                if (origQty + modQty + ln.Quantity < 0)
                                {
                                    lineErrors.Add(string.Format(bbxBEConsts.ERR_WRONGCORRECTIONQTY,
                                            ln.ProductCode,
                                            origQty,
                                            modQty,
                                            ln.Quantity));
                                }
                            }

                            ln.LineNumberReference = (short)(LineNumberReference++);          //NAV doc:Az eredeti számla módosítással (CREATE) érintett tételének sorszáma, (lineNumber).Új tétel létrehozása esetén az új tétel sorszáma, az eredeti számla folytatásaként
                        }

                        //termékdíj
                        if (prod.ProductFee > 0)
                        {
                            ln.TakeoverReason = TakeoverType.Item02_ga.ToString();                  //egyelőre beégetjük a 02_ga-t

                            if (ln.TakeoverReason != TakeoverType.Item01.ToString())                 //későbbi felhasználásra is felkészülünk, 01 esetén nincs átvállalás
                            {
                                ln.TakeoverAmount = Math.Round(prod.ProductFee * ln.Quantity, 1);
                            }

                            //ln.ProductFeeProductCodeValue = //KT v. CSK
                            ln.ProductFeeQuantity = ln.Quantity;
                            ln.ProductFeeMeasuringUnit = ln.UnitOfMeasure;
                            //ln.ProductFeeRate = 
                            ln.ProductFeeAmount = Math.Round(prod.ProductFee * ln.Quantity, 1);
                        }
                    }
                    if (lineErrors.Any())
                    {
                        throw new ValidationException(lineErrors);
                    }

                    invoice = bllInvoice.CalcInvoiceAmounts(invoice);

                    //Bizonylatszám megállapítása
                    counterCode = bllCounter.GetCounterCode(invoiceType, paymentMethod, invoice.Incoming, isInvoiceCorrection, wh.ID);
                    invoice.InvoiceNumber = await _counterRepository.GetNextValueAsync(counterCode, wh.ID);
                    invoice.Copies = 1;


                    await this.AddInvoiceAsync(invoice, RelDeliveryNoteLines);
                    await _counterRepository.FinalizeValueAsync(counterCode, wh.ID, invoice.InvoiceNumber);

                    if (!request.Incoming
                         && !isInvoiceCorrection && !hasRelDeliveryNotes
                         && (invoiceType == enInvoiceType.INV || invoiceType == enInvoiceType.DNO)
                         && cust != null)
                    {
                        cust.LatestDiscountPercent = request.InvoiceDiscountPercent;
                        await _customerRepository.UpdateAsync(cust);

                    }

                    if (updatingProducts.Count > 0)
                    {
                        await _productRepository.UpdateProductRangeAsync(updatingProducts, true);
                    }

                    //szemafr kiütések
                    var key = bbxBEConsts.DEF_CUSTOMERLOCK_KEY + invoice.CustomerID.ToString();
                    await _expiringData.DeleteItemAsync(key);
                    key = bbxBEConsts.DEF_CUSTOMERLOCK_KEY + invoice.SupplierID.ToString();
                    await _expiringData.DeleteItemAsync(key);



                    await dbContextTransaction.CommitAsync();

                    if (!invoice.Incoming && invoice.InvoiceType == enInvoiceType.INV.ToString())
                    {
                        var invForSend = await this.GetInvoiceRecordAsync(invoice.ID, invoiceQueryTypes.full);  //Újraolvasás teljes adatkészlettel
                        await _NAVXChangeRepository.CreateNAVXChangeForManageInvoiceAsynch(invForSend, cancellationToken);
                    }



                    invoice.InvoiceLines.Clear();
                    invoice.SummaryByVatRates.Clear();
                    if (invoice.AdditionalInvoiceData != null)
                        invoice.AdditionalInvoiceData.Clear();
                    if (invoice.NAVXChanges != null)
                        invoice.NAVXChanges.Clear();
                    return invoice;
                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();

                    if (!string.IsNullOrWhiteSpace(invoice.InvoiceNumber) && !string.IsNullOrWhiteSpace(counterCode))
                    {
                        await _counterRepository.RollbackValueAsync(counterCode, invoice.WarehouseID, invoice.InvoiceNumber);
                    }
                    throw;
                }
            }
            return null;
        }
        public async Task<Invoice> UpdatePricePreviewAsynch(UpdatePricePreviewCommand request, CancellationToken cancellationToken)
        {

            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {
                    var invoice = await this.GetInvoiceRecordAsync(request.ID);
                    if (invoice == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, request.ID));
                    }

                    invoice.CustomerID = request.CustomerID;


                    //A SummaryByVatRates-t újrageneráljuk. Eltároljuk az eredei állapotot, mert az update során kitöröljük
                    //
                    var oriSummaryByVatRates = invoice.SummaryByVatRates;

                    //Tételsorok előfeldolgozása
                    foreach (var rln in request.InvoiceLines)
                    {
                        var ln = invoice.InvoiceLines.Where(w => w.ID == rln.ID).FirstOrDefault();
                        if (ln == null)
                        {
                            throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICELINENOTFOUND, invoice.ID,
                                    invoice.InvoiceNumber, rln.ID));
                        }
                        ln.UnitPrice = rln.UnitPrice;

                        if (ln.ProductID.HasValue)
                        {
                            var prod = _productRepository.GetProduct(ln.ProductID.Value);
                            if (prod == null)
                            {
                                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODNOTFOUND, ln.ProductID.Value));
                            }
                            ln.NoDiscount = prod.NoDiscount;
                        }
                        ln.PriceReview = false;         //megtörtént az ár felülvizsgálat
                    }

                    invoice = bllInvoice.CalcInvoiceAmounts(invoice);
                    invoice.SummaryByVatRates.ToList().ForEach(i => i.InvoiceID = invoice.ID);


                    await this.UpdateInvoiceAsync(invoice, oriSummaryByVatRates);

                    invoice.InvoiceLines.Clear();
                    invoice.SummaryByVatRates.Clear();
                    if (invoice.AdditionalInvoiceData != null)
                        invoice.AdditionalInvoiceData.Clear();

                    await dbContextTransaction.CommitAsync();

                    return invoice;
                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
            return null;
        }


        public IList<string> Import(string CSVContent, string warehouseCode)
        {

            var wh = _warehouseRepository.GetWarehouseByCodeAsync(warehouseCode).GetAwaiter().GetResult();
            if (wh == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, warehouseCode));
            }

            var own = _customerRepository.GetOwnData();
            if (own == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_OWNNOTFOUND));
            }


            var fieldSeparator = ";";
            string regExpPattern = $"{fieldSeparator}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
            Regex regexp = new Regex(regExpPattern);

            var invoicesFromCSV = new List<Dictionary<string, string>>();

            List<String> lines = CSVContent.Split('\n').ToList();
            int counter = 0;
            string[] headerArray = null;

            lines.Take(200).ToList().ForEach(currentLine =>
            {
                if (counter == 0)
                {
                    headerArray = regexp.Split(currentLine.Replace("\r", ""));

                }
                else if (!string.IsNullOrWhiteSpace(currentLine))
                {
                    Debug.WriteLine(currentLine);
                    string[] currentFieldsArray = regexp.Split(currentLine.Replace("\r", ""));
                    var item = new Dictionary<string, string>();
                    for (int i = 0; i < currentFieldsArray.Length; i++)
                    {
                        item.Add(headerArray[i], currentFieldsArray[i]);
                    }
                    invoicesFromCSV.Add(item);

                }
                counter++;
            });

            var invList = new List<Invoice>();

            CultureInfo provider = CultureInfo.InvariantCulture;

            invoicesFromCSV.ForEach(invoiceItem =>
            {
                Customer cust = null;
                //1. adószám alapján keresünk
                var adoszam = invoiceItem["ADOSZAM"];
                var veovNev = invoiceItem["VEVONEV"];
                if (!string.IsNullOrWhiteSpace(adoszam))
                {
                    cust = _customerRepository.GetCustomerRecordByTaxpayerId(adoszam.Substring(0, 8));
                }

                //név alapján keresünk
                if (cust == null)
                {
                    var custsFound = _customerRepository.GetCustomerRecordsByName(veovNev);

                    if (custsFound.Count == 1)
                    {
                        cust = custsFound.First();
                    }
                    else if (custsFound.Count > 1)
                    {
                        var varos = invoiceItem["VAROS"].ToUpper();
                        var utca = invoiceItem["UTCA"].ToUpper();
                        cust = custsFound.FirstOrDefault(f => f.City.ToUpper() == varos && f.AdditionalAddressDetail.ToUpper().Contains(utca));
                    }
                }
                if (cust == null)
                {

                    var IRSZAM = invoiceItem["IRSZAM"];
                    var VAROS = invoiceItem["VAROS"];
                    var UTCA = invoiceItem["UTCA"];
                    var HAZSZAM = invoiceItem["HAZSZAM"];
                    var BANK = invoiceItem["BANK"];
                    var MEGJGYZ = invoiceItem["MEGJGYZ"];
                    var EMAIL = invoiceItem["EMAIL"];
                    var V_FIZM = invoiceItem["V_FIZM"];
                    var V_FIZH = invoiceItem["V_FIZH"];
                    var V_ARTIP = invoiceItem["V_ARTIP"];
                    var V_ENG = invoiceItem["V_ENG"];
                    //var LIMIT = invoiceItem["LIMIT"];
                    var V_FAFA = invoiceItem["V_FAFA"];
                    var ORSZAG = invoiceItem["ORSZAG"];
                    var EUADOSZAM = invoiceItem["EUADOSZAM"];

                    if (string.IsNullOrWhiteSpace(IRSZAM) && !string.IsNullOrWhiteSpace(VAROS))
                    {
                        var zip = _ZipRepositoryAsync.GetZipByCity(VAROS).GetAwaiter().GetResult();
                        if (zip != null)
                        {
                            IRSZAM = zip.ZipCode;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(IRSZAM) && string.IsNullOrWhiteSpace(VAROS))
                    {
                        var zip = _ZipRepositoryAsync.GetCityByZip(IRSZAM).GetAwaiter().GetResult();
                        if (zip != null)
                        {
                            IRSZAM = zip.ZipCity;
                        }
                    }

                    cust = new Customer();
                    cust.CustomerName = veovNev;
                    cust.PostalCode = IRSZAM;
                    cust.City = VAROS;
                    cust.AdditionalAddressDetail = (UTCA + " " + HAZSZAM).Trim();
                    cust.Comment = MEGJGYZ;
                    cust.CustomerBankAccountNumber = BANK;
                    cust.UnitPriceType = V_ARTIP.Equals("1") ? "UNIT" : "LIST";
                    cust.DefPaymentMethod = V_FIZM.Equals("1") ? PaymentMethodType.CASH.ToString()
                            : V_FIZM.Equals("2") ? PaymentMethodType.TRANSFER.ToString() : PaymentMethodType.CASH.ToString();

                    if (!string.IsNullOrWhiteSpace(adoszam))
                    {
                        cust.TaxpayerId = adoszam.Substring(0, 8);
                        cust.VatCode = adoszam.Substring(9, 1);
                        cust.CountyCode = adoszam.Substring(11, 2);
                    }
                    cust.CountryCode = !string.IsNullOrWhiteSpace(ORSZAG) ? ORSZAG : "HU";
                    cust.LatestDiscountPercent = Decimal.Parse(V_ENG.Replace(".", ","));
                    cust.PaymentDays = short.Parse(V_FIZH.Replace(".", ","));
                    // cust.MaxLimit= Decimal.Parse(LIMIT.Replace(".", ","));
                    cust.ThirdStateTaxId = EUADOSZAM;
                    cust.IsFA = V_FAFA == "1";
                    cust.CustomerVatStatus = !string.IsNullOrWhiteSpace(adoszam) && cust.CountryCode == "HU" ? CustomerVatStatusType.DOMESTIC.ToString() :
                                             !string.IsNullOrWhiteSpace(EUADOSZAM) && cust.CountryCode != "HU" ? CustomerVatStatusType.OTHER.ToString() :
                                             CustomerVatStatusType.PRIVATE_PERSON.ToString();


                    _customerRepository.AddCustomerAsync(cust).GetAwaiter().GetResult();
                    _dbContext.SaveChangesAsync().GetAwaiter().GetResult();

                    //                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_CUSTNOTFOUND));
                }


                var SZAMLASZ = invoiceItem["SZAMLASZ"];
                var inv = GetInvoiceRecordByInvoiceNumberAsync(SZAMLASZ).GetAwaiter().GetResult();
                if (inv == null)
                {

                    /*
                    #define MAX_FIZMOD      9

                    #define FM_BLANK        0                    //
                    #define FM_KP           1                    // Kp
                    #define FM_ATUTALAS     2                    // Átutalás
                    #define FM_POSTAI       3                    // Utánvét
                    #define FM_BELSO        4                    // Belső számla
                    #define FM_BLKKP        5                    // Blokk készpénzes
                    #define FM_BLKCARD      6                    // Blokk kártya
                    #define FM_CARD         7                    // Kártya
                    */
                    var FIZMOD = invoiceItem["FIZMOD"];
                    var CURRENCY = invoiceItem["CURRENCY"];
                    var RATE = Decimal.Parse(invoiceItem["RATE"].Replace(".", ","));

                    var FELAR = Decimal.Parse(invoiceItem["FELAR"].Replace(".", ","));
                    var ENG = Decimal.Parse(invoiceItem["ENG"].Replace(".", ","));
                    var ENGFELOSSZ = Decimal.Parse(invoiceItem["ENGFELOSSZ"].Replace(".", ","));

                    var OSSZ = Decimal.Parse(invoiceItem["OSSZ"].Replace(".", ","));
                    var AFAERT = Decimal.Parse(invoiceItem["AFAERT"].Replace(".", ","));
                    var BRUTTO = Decimal.Parse(invoiceItem["BRUTTO"].Replace(".", ","));

                    var CRCY_OSSZ = Decimal.Parse(invoiceItem["CRCY_OSSZ"].Replace(".", ","));
                    var CRCY_AFA = Decimal.Parse(invoiceItem["CRCY_AFA"].Replace(".", ","));
                    var CRCY_BRT = Decimal.Parse(invoiceItem["CRCY_BRT"].Replace(".", ","));


                    inv = new Invoice();
                    inv.Incoming = false;
                    inv.InvoiceType = enInvoiceType.INV.ToString();
                    inv.WarehouseID = wh.ID;
                    inv.InvoiceNumber = SZAMLASZ;
                    inv.InvoiceIssueDate = DateTime.ParseExact(invoiceItem["SZAMLAD"].Substring(0, 10), "yyyy-MM-dd", provider);
                    inv.CompletenessIndicator = false;
                    inv.SupplierID = own.ID;
                    inv.CustomerID = cust.ID;
                    inv.InvoiceCategory = InvoiceCategoryType.NORMAL.ToString();    //minden régi számlát normálként töltünk be
                    inv.InvoiceDeliveryDate = DateTime.ParseExact(invoiceItem["SZAMLAE"].Substring(0, 10), "yyyy-MM-dd", provider);
                    inv.PaymentDate = DateTime.ParseExact(invoiceItem["SZAMLAF"].Substring(0, 10), "yyyy-MM-dd", provider);
                    inv.PaymentMethod = (FIZMOD == "1" ? PaymentMethodType.CASH.ToString() :
                                         FIZMOD == "2" ? PaymentMethodType.TRANSFER.ToString() :
                                         FIZMOD == "3" ? PaymentMethodType.TRANSFER.ToString() :
                                         FIZMOD == "7" ? PaymentMethodType.CARD.ToString() : PaymentMethodType.OTHER.ToString());
                    inv.CurrencyCode = (CURRENCY == "Ft" ? enCurrencyCodes.HUF.ToString() :
                                        CURRENCY == "EUR" ? enCurrencyCodes.EUR.ToString() :
                                        CURRENCY == "USD" ? enCurrencyCodes.USD.ToString() :
                                        CURRENCY == "HUF" ? enCurrencyCodes.HUF.ToString() : enCurrencyCodes.HUF.ToString());
                    inv.ExchangeRate = RATE == 0 || inv.CurrencyCode == enCurrencyCodes.HUF.ToString() ? 1 : RATE;
                    inv.InvoiceAppearance = InvoiceAppearanceType.PAPER.ToString();
                    inv.Copies = -1;
                    inv.IncomingInvReference = "";
                    inv.OriginalInvoiceID = 0;              //a betöltött számláknál nem kezeljükl a referenciát a javított számlára
                    inv.OriginalInvoiceNumber = "";
                    inv.ModifyWithoutMaster = false;
                    inv.InvoiceDiscountPercent = ENG - FELAR;
                    inv.InvoiceDiscount = ENGFELOSSZ;
                    inv.InvoiceDiscountHUF = Math.Round(inv.ExchangeRate * ENGFELOSSZ, 1);
                    inv.InvoiceNetAmount = Math.Round(inv.CurrencyCode == enCurrencyCodes.HUF.ToString() ? OSSZ : CRCY_OSSZ, 1);
                    inv.InvoiceNetAmountHUF = Math.Round(OSSZ, 1);
                    inv.InvoiceVatAmount = Math.Round(inv.CurrencyCode == enCurrencyCodes.HUF.ToString() ? AFAERT : CRCY_AFA, 1);
                    inv.InvoiceVatAmountHUF = Math.Round(AFAERT, 1);
                    inv.InvoiceGrossAmount = Math.Round(inv.CurrencyCode == enCurrencyCodes.HUF.ToString() ? BRUTTO : CRCY_BRT, 1);

                    invList.Add(inv);
                }
            });

            AddRangeAsync(invList).GetAwaiter().GetResult();
            return new List<string>();
        }

    }
}