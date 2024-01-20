using AsyncKeyedLock;
using AutoMapper;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qWhsTransfer;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Common.ExpiringData;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using bxBE.Application.Commands.cmdWhsTransfer;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class WhsTransferRepositoryAsync : GenericRepositoryAsync<WhsTransfer>, IWhsTransferRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<WhsTransfer> _dataShaperWhsTransfer;
        private IDataShapeHelper<GetWhsTransferViewModel> _dataShaperGetWhsTransferViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IStockRepositoryAsync _stockRepository;
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly ICounterRepositoryAsync _counterRepository;
        private readonly IWarehouseRepositoryAsync _warehouseRepository;
        private readonly IProductRepositoryAsync _productRepository;
        private readonly AsyncKeyedLocker<string> _asyncKeyedLocker;

        public WhsTransferRepositoryAsync(IApplicationDbContext dbContext,
                IExpiringData<ExpiringDataObject> expiringData,
                ICacheService<Product> productCacheService,
                ICacheService<Customer> customerCacheService,
                ICacheService<ProductGroup> productGroupCacheService,
                ICacheService<Origin> originCacheService,
                ICacheService<VatRate> vatRateCacheService,
                AsyncKeyedLocker<string> asyncKeyedLocker,
                IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperWhsTransfer = new DataShapeHelper<WhsTransfer>();
            _dataShaperGetWhsTransferViewModel = new DataShapeHelper<GetWhsTransferViewModel>();
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _stockRepository = new StockRepositoryAsync(dbContext, modelHelper, mapper, mockData, productCacheService, productGroupCacheService, originCacheService, vatRateCacheService, asyncKeyedLocker);
            _customerRepository = new CustomerRepositoryAsync(dbContext, modelHelper, mapper, mockData, expiringData, customerCacheService);
            _counterRepository = new CounterRepositoryAsync(dbContext, modelHelper, mapper, mockData);
            _warehouseRepository = new WarehouseRepositoryAsync(dbContext, modelHelper, mapper, mockData);
            _productRepository = new ProductRepositoryAsync(dbContext, modelHelper, mapper, mockData, productCacheService, productGroupCacheService, originCacheService, vatRateCacheService);
        }

        public async Task<WhsTransfer> AddWhsTransferAsync(WhsTransfer whsTransfer)
        {

            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {
                    await AddAsync(whsTransfer);
                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }

            return whsTransfer;
        }
        public async Task<WhsTransfer> UpdateWhsTransferAsync(WhsTransfer whsTransfer)
        {

            WhsTransfer storedWhsTransfer = null;
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {
                    storedWhsTransfer = await _dbContext.WhsTransfer
                                                    .Include(i => i.WhsTransferLines)
                                                    .Where(x => x.ID == whsTransfer.ID).FirstOrDefaultAsync();
                    if (storedWhsTransfer == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WHSTRANSFERNOTFOUND, whsTransfer.ID));
                    }

                    //sorokat kitöröljük 
                    _dbContext.WhsTransferLine.RemoveRange(storedWhsTransfer.WhsTransferLines);

                    var exludedProps = new List<PropertyInfo>()
                            {   storedWhsTransfer.GetType().GetProperty("ID"),
                                storedWhsTransfer.GetType().GetProperty("WhsTransferNumber")};

                    Utils.CopyAllTo<WhsTransfer, WhsTransfer>(whsTransfer, storedWhsTransfer, exludedProps);
                    storedWhsTransfer.WhsTransferLines.ForEach(i =>
                    {
                        i.WhsTransferID = whsTransfer.ID;
                        _dbContext.Instance.Entry(i).State = EntityState.Added;
                    });


                    await UpdateAsync(storedWhsTransfer);
                    await dbContextTransaction.CommitAsync();

                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }

            return storedWhsTransfer;
        }

        public async Task<WhsTransfer> DeleteWhsTransferAsync(long ID)
        {

            WhsTransfer whsTransfer = null;
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                whsTransfer = await _dbContext.WhsTransfer.Where(x => x.ID == ID).FirstOrDefaultAsync();
                if (whsTransfer != null)
                {
                    whsTransfer.Deleted = true;
                    await UpdateAsync(whsTransfer);
                    await dbContextTransaction.CommitAsync();

                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WHSTRANSFERNOTFOUND, ID));
                }
            }
            return whsTransfer;
        }

        public async Task<Entity> GetWhsTransferAsync(long ID)
        {

            WhsTransfer item = await GetWhsTransferRecordAsync(ID, true);

            if (item == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WHSTRANSFERNOTFOUND, ID));
            }

            var itemModel = _mapper.Map<WhsTransfer, GetWhsTransferViewModel>(item);

            var listFieldsModel = _modelHelper.GetModelFields<GetWhsTransferViewModel>();

            // shape data
            var shapedData = _dataShaperGetWhsTransferViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapedData;
        }
        public async Task<WhsTransfer> GetWhsTransferRecordAsync(long ID, bool fulldata)
        {
            WhsTransfer item;
            if (fulldata)
            {
                item = await _dbContext.WhsTransfer
                  .Include(w => w.FromWarehouse).AsNoTracking()
                  .Include(w => w.ToWarehouse).AsNoTracking()
                  .Include(l => l.WhsTransferLines).ThenInclude(p => p.Product).AsNoTracking()
                  .Where(x => x.ID == ID && !x.Deleted).AsNoTracking().FirstOrDefaultAsync();
            }
            else
            {
                item = await _dbContext.WhsTransfer
                  .Where(x => x.ID == ID && !x.Deleted).AsNoTracking().FirstOrDefaultAsync();
            }
            return item;
        }


        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedWhsTransferAsync(QueryWhsTransfer requestParameter)
        {

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetWhsTransferViewModel, GetWhsTransferViewModel>();


            int recordsTotal, recordsFiltered;


            IQueryable<WhsTransfer> query;
            query = _dbContext.WhsTransfer
              .Include(w => w.FromWarehouse)
              .Include(w => w.ToWarehouse)
              .Include(l => l.WhsTransferLines).ThenInclude(p => p.Product)
              .AsNoTracking();
            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data

            FilterBy(ref query, requestParameter.WhsTransferStatus, requestParameter.FromWarehouseCode, requestParameter.ToWarehouseCode,
                                requestParameter.TransferDateFrom, requestParameter.TransferDateTo, requestParameter.Deleted);


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
            List<WhsTransfer> resultData = await GetPagedData(query, requestParameter);


            //TODO: szebben megoldani
            var resultDataModel = new List<GetWhsTransferViewModel>();
            resultData.ForEach(i =>
            {
                var im = _mapper.Map<WhsTransfer, GetWhsTransferViewModel>(i);
                resultDataModel.Add(im);  //nem full data esetén is szüség van az invoiceLines-re
            }
            );

            var listFieldsModel = _modelHelper.GetModelFields<GetWhsTransferViewModel>();

            var shapedData = _dataShaperGetWhsTransferViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapedData, recordsCount);
        }

        private void FilterBy(ref IQueryable<WhsTransfer> p_items, string WhsTransferStatus, string FromWarehouseCode, string ToWarehouseCode,
                                DateTime? TransferDateFrom, DateTime? TransferDateTo, bool? Deleted)
        {
            if (!p_items.Any())
                return;


            var predicate = PredicateBuilder.New<WhsTransfer>();

            predicate = predicate.And(p =>
                            p.WhsTransferStatus == WhsTransferStatus
                            && (FromWarehouseCode == null || p.FromWarehouse.WarehouseCode.ToUpper().Contains(FromWarehouseCode))
                            && (ToWarehouseCode == null || p.ToWarehouse.WarehouseCode.ToUpper().Contains(ToWarehouseCode))
                            && (!TransferDateFrom.HasValue || p.TransferDate >= TransferDateFrom.Value)
                            && (!TransferDateTo.HasValue || p.TransferDate <= TransferDateTo.Value)
                            && (!p.Deleted || (Deleted.HasValue && Deleted.Value))
                           );

            p_items = p_items.Where(predicate);
        }


        public async Task<WhsTransfer> ProcessAsync(long ID, DateTime transferDateIn)
        {
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {

                try
                {
                    var whsTransfer = await GetWhsTransferRecordAsync(ID, true);
                    if (whsTransfer == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WHSTRANSFERNOTFOUND, ID));
                    }
                    if (whsTransfer.TransferDate > transferDateIn)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WHSTRANSFERWRONGINDATE, whsTransfer.TransferDate));
                    }

                    if (whsTransfer.WhsTransferStatus != enWhsTransferStatus.READY.ToString())
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WHSTRANSFERWRONGSTATE));
                    }

                    var own = _customerRepository.GetOwnData();

                    whsTransfer.WhsTransferStatus = enWhsTransferStatus.COMPLETED.ToString();
                    whsTransfer.TransferDateIn = transferDateIn;


                    await _stockRepository.MaintainStockByWhsTransferAsync(whsTransfer, own);

                    await UpdateAsync(whsTransfer);

                    // await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                    return whsTransfer;

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

        public async Task<WhsTransfer> CreateWhsTransferAsynch(CreateWhsTransferCommand request, CancellationToken cancellationToken)
        {
            var whsTransfer = _mapper.Map<WhsTransfer>(request);
            var counterCode = "";
            try
            {

                await prepareWhsTransferAsynch(whsTransfer, request.FromWarehouseCode, request.ToWarehouseCode, cancellationToken);

                if (whsTransfer.FromWarehouseID == whsTransfer.ToWarehouseID)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WHSTRANSFERSAMEWHS));
                }

                whsTransfer.Notice = Utils.TidyHtml(whsTransfer.Notice);


                var whs = whsTransfer.FromWarehouseID.ToString().PadLeft(3, '0');
                counterCode = String.Format($"{bbxBEConsts.DEF_WHTCOUNTER}_{whs}");
                whsTransfer.WhsTransferNumber = await _counterRepository.GetNextValueAsync(counterCode, whsTransfer.FromWarehouseID);
                whsTransfer.Copies = 1;
                whsTransfer.WhsTransferStatus = enWhsTransferStatus.READY.ToString();

                await this.AddWhsTransferAsync(whsTransfer);
                await _counterRepository.FinalizeValueAsync(counterCode, whsTransfer.FromWarehouseID, whsTransfer.WhsTransferNumber);
                return whsTransfer;

            }
            catch (Exception)
            {
                if (!string.IsNullOrWhiteSpace(whsTransfer.WhsTransferNumber) && !string.IsNullOrWhiteSpace(counterCode))
                {
                    await _counterRepository.RollbackValueAsync(counterCode, whsTransfer.FromWarehouseID, whsTransfer.WhsTransferNumber);
                }
                throw;
            }
        }

        public async Task<WhsTransfer> UpdateWhsTransferAsynch(UpdateWhsTransferCommand request, CancellationToken cancellationToken)
        {
            var whsTransfer = _mapper.Map<WhsTransfer>(request);
            try
            {

                await prepareWhsTransferAsynch(whsTransfer, request.FromWarehouseCode, request.ToWarehouseCode, cancellationToken);
                if (whsTransfer.FromWarehouseID == whsTransfer.ToWarehouseID)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WHSTRANSFERSAMEWHS));
                }
                whsTransfer.Notice = Utils.TidyHtml(whsTransfer.Notice);

                whsTransfer = await this.UpdateWhsTransferAsync(whsTransfer);
                return whsTransfer;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task prepareWhsTransferAsynch(WhsTransfer whsTransfer, string fromWarehouseCode, string toWarehouseCode, CancellationToken cancellationToken)
        {
            try
            {

                var fromWarehouse = await _warehouseRepository.GetWarehouseByCodeAsync(fromWarehouseCode);
                if (fromWarehouse == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, fromWarehouseCode));
                }
                whsTransfer.FromWarehouseID = fromWarehouse.ID;



                var toWarehouse = await _warehouseRepository.GetWarehouseByCodeAsync(toWarehouseCode);
                if (toWarehouse == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, toWarehouseCode));
                }
                whsTransfer.ToWarehouseID = toWarehouse.ID;

                //Tételsorok előfeldolgozása
                var lineErrors = new List<string>();
                foreach (var ln in whsTransfer.WhsTransferLines)
                {
                    var rln = whsTransfer.WhsTransferLines.SingleOrDefault(i => i.WhsTransferLineNumber == ln.WhsTransferLineNumber);

                    var prod = _productRepository.GetProductByProductCode(rln.ProductCode);
                    if (prod == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODCODENOTFOUND, rln.ProductCode));
                    }

                    ln.ProductID = prod.ID;
                    ln.ProductCode = rln.ProductCode;
                    //ln.Product = prod;

                }

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}