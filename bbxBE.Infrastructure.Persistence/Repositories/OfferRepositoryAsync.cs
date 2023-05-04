﻿using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qOffer;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
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
        private readonly ICacheService<Product> _productCacheService;

        public OfferRepositoryAsync(IApplicationDbContext dbContext,
            IDataShapeHelper<Offer> dataShaperOffer,
            IDataShapeHelper<GetOfferViewModel> dataShaperGetOfferViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData,
            IOfferLineRepositoryAsync offerLineRepository,
            ICacheService<Product> productCacheService) : base(dbContext)
        {
            _dbContext = dbContext;

            _dataShaperOffer = dataShaperOffer;
            _dataShaperGetOfferViewModel = dataShaperGetOfferViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _offerLineRepository = offerLineRepository;
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
            var shapeData = _dataShaperGetOfferViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
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

            var shapeData = _dataShaperGetOfferViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
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
    }
}