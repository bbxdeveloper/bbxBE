using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qWhsTransfer;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common;
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
using System.Reflection;
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
        public WhsTransferRepositoryAsync(IApplicationDbContext dbContext,
                IDataShapeHelper<WhsTransfer> dataShaperWhsTransfer,
                IDataShapeHelper<GetWhsTransferViewModel> dataShaperGetWhsTransferViewModel,
                IModelHelper modelHelper, IMapper mapper, IMockService mockData,
                IStockRepositoryAsync stockRepository,
                ICustomerRepositoryAsync customerRepository
            ) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperWhsTransfer = dataShaperWhsTransfer;
            _dataShaperGetWhsTransferViewModel = dataShaperGetWhsTransferViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _stockRepository = stockRepository;
            _customerRepository = customerRepository;
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
            var shapeData = _dataShaperGetWhsTransferViewModel.ShapeData(itemModel, String.Join(",", listFieldsModel));

            return shapeData;
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

            var shapeData = _dataShaperGetWhsTransferViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapeData, recordsCount);
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
    }
}