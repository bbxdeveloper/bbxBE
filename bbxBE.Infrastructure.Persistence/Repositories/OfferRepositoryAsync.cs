using AutoMapper;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qOffer;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Common.ExpiringData;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using bxBE.Application.Commands.cmdOffer;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Repositories
{

    /*
      
 {
"customerID": 5,
  "offerIssueDate": "2022-05-20",
  "offerVaidityDate": "2022-05-20",
  "notice": "első ajánlat",
  "offerLines": [
    {
     "lineNumber": 1,
      "productCode": "VEG-2973",
      "lineDescription": "Boyler 600W fűtőbetét",
     "vatRateCode": "27%",
      "discount": 10,
      "showDiscount": true,
       "unitPrice": 10,
      "unitVat": 2.7,
      "unitGross": 12.7
    }
  ]
}
     */
    public class OfferRepositoryAsync : GenericRepositoryAsync<Offer>, IOfferRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;

        private IDataShapeHelper<Offer> _dataShaperOffer;
        private IDataShapeHelper<GetOfferViewModel> _dataShaperGetOfferViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IOfferLineRepositoryAsync _offerLineRepository;
        private readonly ICounterRepositoryAsync _counterRepository;
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IVatRateRepositoryAsync _vatRateRepository;

        private readonly ICacheService<Product> _productCacheService;

        public OfferRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            ICacheService<Product> productCacheService,
            ICacheService<Customer> customerCacheService,
            ICacheService<ProductGroup> productGroupCacheService,
            ICacheService<Origin> originCacheService,
            ICacheService<VatRate> vatRateCacheService,
            IExpiringData<ExpiringDataObject> expiringData
            ) : base(dbContext)
        {
            _dbContext = dbContext;

            _dataShaperOffer = new DataShapeHelper<Offer>();
            _dataShaperGetOfferViewModel = new DataShapeHelper<GetOfferViewModel>();
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _offerLineRepository = new OfferLineRepositoryAsync(dbContext, modelHelper, mapper, mockData);
            _counterRepository = new CounterRepositoryAsync(dbContext, modelHelper, mapper, mockData);
            _productRepository = new ProductRepositoryAsync(dbContext, modelHelper, mapper, mockData, productCacheService, productGroupCacheService, originCacheService, vatRateCacheService);
            _vatRateRepository = new VatRateRepositoryAsync(dbContext, modelHelper, mapper, mockData, vatRateCacheService);
            _productCacheService = productCacheService;
        }
        public async Task<Offer> AddOfferAsync(Offer p_Offer)
        {

            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {
                    //Az előző verziók érvénytelenítése (ha van)
                    //
                    var prevVerisons = await _dbContext.Offer
                      .Where(x => x.OfferNumber == p_Offer.OfferNumber && x.OfferVersion < p_Offer.OfferVersion && x.ID != p_Offer.ID).ToListAsync();
                    prevVerisons.ForEach(i =>
                    {
                        i.LatestVersion = false;
                        _dbContext.Instance.Entry(i).State = EntityState.Modified;
                    });

                    //Az aktuális minden esetben Latest!
                    p_Offer.LatestVersion = true;

                    p_Offer.Copies = 1;

                    //Ha másolatot insert-elünk, a biztonság kedvéért kiürítjük az ID-ket
                    p_Offer.OfferLines.ToList().ForEach(e =>
                    {
                        if (e.Product != null)
                            _dbContext.Instance.Entry(e.Product).State = EntityState.Unchanged;
                        if (e.VatRate != null)
                            _dbContext.Instance.Entry(e.VatRate).State = EntityState.Unchanged;
                        _dbContext.Instance.Entry(e).State = EntityState.Added;

                        e.OfferID = 0;
                        e.ID = 0;
                    }
                    );

                    p_Offer.ID = 0;

                    await AddAsync(p_Offer);
                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }

            return p_Offer;
        }

        public async Task<Offer> UpdateOfferAsync(Offer p_Offer)
        {
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {
                    //Az előző verziók érvénytelenítése
                    //
                    var prevVerisons = await _dbContext.Offer
                      .Where(x => x.OfferNumber == p_Offer.OfferNumber && x.OfferVersion < p_Offer.OfferVersion && x.ID != p_Offer.ID).ToListAsync();
                    prevVerisons.ForEach(i =>
                    {
                        i.LatestVersion = false;
                        _dbContext.Instance.Entry(i).State = EntityState.Modified;
                    });

                    //Az aktuális minden esetben Latest!
                    p_Offer.LatestVersion = true;


                    var oriLines = await _dbContext.OfferLine.Where(w => w.OfferID == p_Offer.ID).ToListAsync();
                    await _offerLineRepository.RemoveRangeAsync(oriLines.Where(w => !p_Offer.OfferLines.Any(a => a.ID == w.ID)).ToList(), true);


                    p_Offer.OfferLines.ToList().ForEach(e =>
                    {
                        if (e.Product != null)
                            _dbContext.Instance.Entry(e.Product).State = EntityState.Unchanged;
                        if (e.VatRate != null)
                            _dbContext.Instance.Entry(e.VatRate).State = EntityState.Unchanged;
                        _dbContext.Instance.Entry(e).State = (e.ID == 0 ? EntityState.Added : EntityState.Modified);
                    });


                    await UpdateAsync(p_Offer, false);

                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
            return p_Offer;
        }
        public async Task<Offer> UpdateOfferRecordAsync(Offer p_Offer)
        {
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {
                    p_Offer.OfferLines.ToList().ForEach(e =>
                    {
                        e.Product = null;
                        e.VatRate = null;
                    });

                    await UpdateAsync(p_Offer);
                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
            return p_Offer;
        }

        public async Task<Offer> DeleteOfferAsync(long ID)
        {

            Offer offer = null;
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                offer = await _dbContext.Offer.AsNoTracking()
                        .Include(o => o.OfferLines).AsNoTracking()
                        .Where(x => x.ID == ID && !x.Deleted).FirstOrDefaultAsync();

                if (offer != null)
                {
                    offer.OfferLines.ToList().ForEach(e =>
                    {
                        e.Product = null;
                        e.VatRate = null;
                        e.Deleted = true;
                    });

                    offer.Deleted = true;

                    await UpdateAsync(offer);
                    await dbContextTransaction.CommitAsync();

                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_OFFERNOTFOUND, ID));
                }
            }
            return offer;
        }


        public async Task<Entity> GetOfferAsync(long ID, bool FullData = true)
        {

            var item = await GetOfferRecordAsync(ID);

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_OFFERNOTFOUND, ID));
            }

            var itemModel = _mapper.Map<Offer, GetOfferViewModel>(item);

            //a requestParameter.FullData kezelés miatt a SumNetAmount és SumBrtAmount mezőket ki kell számolni 
            itemModel.SumNetAmount = Math.Round(itemModel.OfferLines.Sum(s => s.NetAmount), (itemModel.CurrencyCode == enCurrencyCodes.HUF.ToString() ? 1 : 2));
            itemModel.SumBrtAmount = Math.Round(itemModel.OfferLines.Sum(s => s.BrtAmount), (itemModel.CurrencyCode == enCurrencyCodes.HUF.ToString() ? 1 : 2));
            if (FullData)
            {
                //Aktuális árak feltöltése a sorokban
                itemModel.OfferLines.ForEach(ol =>
                {
                    if (ol.ProductID.HasValue)
                    {
                        Product prod = null;
                        _productCacheService.TryGetValue(ol.ProductID.Value, out prod);
                        if (prod != null)
                        {
                            ol.UnitPrice1 = prod.UnitPrice1;
                            ol.UnitPrice2 = prod.UnitPrice2;
                        }
                    }
                });
            }
            else
            {
                //itemModel.OfferLines = new List<GetOfferViewModel.OfferLine>();
                itemModel.OfferLines = null;
            }


            var listFieldsModel = _modelHelper.GetModelFields<GetOfferViewModel>();

            // shape data
            var shapedData = _dataShaperGetOfferViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapedData;
        }

        public async Task<Offer> GetOfferRecordAsync(long ID)
        {


            Offer item;

            item = await _dbContext.Offer.AsNoTracking()
                  .Include(c => c.Customer).AsNoTracking()
                  .Include(u => u.User).AsNoTracking()
                  .Include(i => i.OfferLines).ThenInclude(t => t.VatRate).AsNoTracking()
                  .Where(x => x.ID == ID && !x.Deleted).FirstOrDefaultAsync();
            return item;
        }


        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedOfferAsync(QueryOffer requestParameter)
        {
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetOfferViewModel, Offer>();


            int recordsTotal, recordsFiltered;


            IQueryable<Offer> query;
            query = _dbContext.Offer.AsNoTracking()
          .Include(c => c.Customer).AsNoTracking()
          .Include(u => u.User).AsNoTracking()
          .Include(i => i.OfferLines).ThenInclude(t => t.VatRate).AsNoTracking();


            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
            FilterBy(ref query, requestParameter.CustomerID, requestParameter.OfferNumber,
                    requestParameter.OfferIssueDateFrom, requestParameter.OfferIssueDateTo,
                    requestParameter.OfferVaidityDateForm, requestParameter.OfferVaidityDateTo);

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

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter);

            //TODO: szebben megoldani
            var resultDataModel = new List<GetOfferViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<Offer, GetOfferViewModel>(i))
            );

            //a requestParameter.FullData kezelés miatt a SumNetAmount és SumBrtAmount mezőket ki kell számolni 
            resultDataModel.ForEach(i =>
            {
                i.SumNetAmount = Math.Round(i.OfferLines.Sum(s => s.NetAmount), (i.CurrencyCode == enCurrencyCodes.HUF.ToString() ? 1 : 2));
                i.SumBrtAmount = Math.Round(i.OfferLines.Sum(s => s.BrtAmount), (i.CurrencyCode == enCurrencyCodes.HUF.ToString() ? 1 : 2));
                if (requestParameter.FullData)
                {
                    //Aktuális árak feltöltése a sorokban
                    i.OfferLines.ForEach(ol =>
                    {
                        if (ol.ProductID.HasValue)
                        {
                            Product prod = null;
                            _productCacheService.TryGetValue(ol.ProductID.Value, out prod);
                            if (prod != null)
                            {
                                ol.UnitPrice1 = prod.UnitPrice1;
                                ol.UnitPrice2 = prod.UnitPrice2;
                            }
                        }
                    });
                }
                else
                {
                    //itemModel.OfferLines = new List<GetOfferViewModel.OfferLine>();
                    i.OfferLines = null;
                }
            });



            var listFieldsModel = _modelHelper.GetModelFields<GetOfferViewModel>();

            var shapedData = _dataShaperGetOfferViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapedData, recordsCount);
        }

        private void FilterBy(ref IQueryable<Offer> p_items, long CustomerID, string OfferNumber,
                                DateTime? OfferIssueDateFrom, DateTime? OfferIssueDateTo,
                                DateTime? OfferVaidityDateFrom, DateTime? OfferVaidityDateTo)
        {
            if (!p_items.Any())
                return;

            /*
            if (string.IsNullOrWhiteSpace(WarehouseCode) && string.IsNullOrWhiteSpace(OfferNumber) &&
                        !OfferIssueDateFrom.HasValue && !OfferIssueDateTo.HasValue &&
                        !OfferDeliveryDateFrom.HasValue && !OfferDeliveryDateTo.HasValue)
                return;
            */
            var predicate = PredicateBuilder.New<Offer>();

            predicate = predicate.And(p => !p.Deleted && (CustomerID == 0 || CustomerID == p.CustomerID)
                            && (string.IsNullOrWhiteSpace(OfferNumber) || (p.OfferNumber.ToUpper() + "/" + p.OfferVersion).Contains(OfferNumber.ToUpper()))
                            && (!OfferIssueDateFrom.HasValue || p.OfferIssueDate >= OfferIssueDateFrom.Value)
                            && (!OfferIssueDateTo.HasValue || p.OfferIssueDate <= OfferIssueDateTo.Value)
                            && (!OfferVaidityDateFrom.HasValue || p.OfferVaidityDate >= OfferVaidityDateFrom.Value)
                            && (!OfferVaidityDateTo.HasValue || p.OfferVaidityDate <= OfferVaidityDateTo.Value)
                            );

            p_items = p_items.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }
        public async Task<Offer> CreateOfferAsync(CreateOfferCommand request, CancellationToken cancellationToken)
        {
            var offer = _mapper.Map<Offer>(request);
            offer.OfferVersion = 0;
            offer.Notice = Utils.TidyHtml(offer.Notice);


            if (string.IsNullOrWhiteSpace(offer.CurrencyCode))
            {
                offer.CurrencyCode = enCurrencyCodes.HUF.ToString();    // Forintos a default
                offer.ExchangeRate = 1;
            }

            var counterCode = "";
            try
            {

                offer.LatestVersion = true;
                //Árajánlatszám megállapítása
                var whs = bbxBEConsts.DEF_WAREHOUSE_ID.ToString().PadLeft(3, '0');
                counterCode = String.Format($"{bbxBEConsts.DEF_WHTCOUNTER}_{whs}");

                offer.OfferNumber = await _counterRepository.GetNextValueAsync(counterCode, bbxBEConsts.DEF_WAREHOUSE_ID);
                offer.Copies = 1;

                //Tételsorok
                foreach (var ln in offer.OfferLines)
                {
                    var rln = request.OfferLines.SingleOrDefault(i => i.LineNumber == ln.LineNumber);


                    var prod = _productRepository.GetProductByProductCode(rln.ProductCode);

                    var vatRate = _vatRateRepository.GetVatRateByCode(rln.VatRateCode);
                    if (vatRate == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_VATRATECODENOTFOUND, rln.VatRateCode));
                    }
                    else
                    {
                        vatRate = _vatRateRepository.GetVatRateByCode(bbxBEConsts.VATCODE_27);
                    }

                    //	ln.Product = prod;
                    ln.ProductID = prod?.ID;
                    ln.ProductCode = rln.ProductCode;
                    ln.NoDiscount = (prod != null ? prod.NoDiscount : false);
                    //Ez modelből jön: ln.LineDescription = prod.Description;

                    //	ln.VatRate = vatRate;
                    ln.VatRateID = vatRate.ID;
                    ln.VatPercentage = vatRate.VatPercentage;

                }

                await this.AddOfferAsync(offer);

                await _counterRepository.FinalizeValueAsync(counterCode, bbxBEConsts.DEF_WAREHOUSE_ID, offer.OfferNumber);

                return offer;
            }
            catch (Exception)
            {
                if (!string.IsNullOrWhiteSpace(offer.OfferNumber) && !string.IsNullOrWhiteSpace(counterCode))
                {
                    await _counterRepository.RollbackValueAsync(counterCode, bbxBEConsts.DEF_WAREHOUSE_ID, offer.OfferNumber);
                }
                throw;
            }
        }

        public async Task<Offer> ModifyOfferAsync(UpdateOfferCommand request, CancellationToken cancellationToken)
        {
            var offer = _mapper.Map<Offer>(request);

            offer.Notice = Utils.TidyHtml(offer.Notice);

            try
            {
                //Tételsorok
                foreach (var ln in offer.OfferLines)
                {
                    var rln = request.OfferLines.SingleOrDefault(i => i.LineNumber == ln.LineNumber);

                    var prod = _productRepository.GetProductByProductCode(rln.ProductCode);
                    if (prod == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODCODENOTFOUND, rln.ProductCode));
                    }
                    var vatRate = _vatRateRepository.GetVatRateByCode(rln.VatRateCode);
                    if (vatRate == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_VATRATECODENOTFOUND, rln.VatRateCode));
                    }

                    //	ln.Product = prod;
                    ln.ProductID = prod.ID;
                    ln.ProductCode = rln.ProductCode;
                    ln.NoDiscount = prod.NoDiscount;
                    //Ez modelből jön: ln.LineDescription = prod.Description;

                    //	ln.VatRate = vatRate;
                    ln.VatRateID = vatRate.ID;
                    ln.VatPercentage = vatRate.VatPercentage;

                }
                if (request.NewOfferVersion)
                {
                    offer.OfferVersion++;
                    offer.ID = 0;
                    await UpdateOfferAsync(offer);
                }
                else
                {
                    await this.UpdateOfferAsync(offer);
                }


                return offer;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}