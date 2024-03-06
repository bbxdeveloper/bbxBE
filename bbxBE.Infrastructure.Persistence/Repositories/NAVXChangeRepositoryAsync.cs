using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvoice;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class NAVXChangeRepositoryAsync : GenericRepositoryAsync<NAVXChange>, INAVXChangeRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        private IDataShapeHelper<GetNAVXChangeViewModel> _dataShaperGetNAVXChangeViewModel;
        private IDataShapeHelper<GetNAVXResultViewModel> _dataShaperGetNAVXResultViewModel;

        public NAVXChangeRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _dataShaperGetNAVXChangeViewModel = new DataShapeHelper<GetNAVXChangeViewModel>();
            _dataShaperGetNAVXResultViewModel = new DataShapeHelper<GetNAVXResultViewModel>();

        }

        public async Task<NAVXChange> AddNAVXChangeAsync(NAVXChange NAVXChange)
        {
            _dbContext.Instance.Entry(NAVXChange).State = EntityState.Added;
            if (NAVXChange.NAVXResults != null)
            {
                NAVXChange.NAVXResults.ToList().ForEach(
                res =>
                _dbContext.Instance.Entry(res).State = EntityState.Added
                );
            }
            await AddAsync(NAVXChange);
            return NAVXChange;
        }
        public async Task<NAVXChange> UpdateNAVXChangeAsync(NAVXChange NAVXChange)
        {
            _dbContext.Instance.Entry(NAVXChange).State = EntityState.Modified;
            if (NAVXChange.NAVXResults != null)
            {
                NAVXChange.NAVXResults.ToList().ForEach(
                    res =>
                    {
                        res.NAVXChangeID = NAVXChange.ID;
                        _dbContext.Instance.Entry(res).State = res.ID > 0 ? EntityState.Modified : EntityState.Added;
                    }
                    );
            }
            await UpdateAsync(NAVXChange);
            return NAVXChange;
        }

        public async Task<NAVXChange> DeleteNAVXChangeAsync(long ID)
        {
            NAVXChange NAVXChange = null;
            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                NAVXChange = await _dbContext.NAVXChange.Where(x => x.ID == ID).SingleOrDefaultAsync();

                if (NAVXChange != null)
                {

                    await RemoveAsync(NAVXChange);
                    await dbContextTransaction.CommitAsync();
                }
                else
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_XCHANGEOTFOUND, ID));
                }
            }
            return NAVXChange;
        }



        public async Task<IList<NAVXChange>> GetXChangeRecordsByStatus(enNAVStatus NAVStatus, int itemCnt)
        {
            var q = _dbContext.NAVXChange.Where(w => w.Status == NAVStatus.ToString()).AsNoTracking();
            return await q.Take(itemCnt).ToListAsync();
        }

        public async Task<IList<NAVXChange>> GetXChangeRecordByInvoiceID(long invoiceID)
        {
            var q = _dbContext.NAVXChange.Where(w => w.InvoiceID == invoiceID).AsNoTracking();
            return await q.ToListAsync();
        }
        public async Task<NAVXChange> CreateNAVXChangeForManageInvoiceAsynch(Invoice invoice, CancellationToken cancellationToken)
        {
            if (invoice == null)
            {
                throw new ResourceNotFoundException(bbxBEConsts.ERR_INVOICEISNULL);
            }
            if (invoice.Incoming || invoice.InvoiceType != enInvoiceType.INV.ToString())
            {
                throw new NAVException(string.Format(bbxBEConsts.ERR_NAVINV, (invoice.InvoiceNumber)));
            }
            var currXChanges = await GetXChangeRecordByInvoiceID(invoice.ID);
            if (currXChanges.Any(a => a.Status == enNAVStatus.CREATED.ToString()))
            {
                throw new NAVException(string.Format(bbxBEConsts.ERR_INVOICEALREADYSETSEND, (invoice.InvoiceNumber)));

            }

            var resNAVXChange = new NAVXChange();

            var invoiceNAVXML = bllInvoice.GetInvoiceNAVXML(invoice);
            resNAVXChange.InvoiceID = invoice.ID;
            resNAVXChange.InvoiceNumber = invoice.InvoiceNumber;
            resNAVXChange.InvoiceXml = XMLUtil.Object2XMLString<InvoiceData>(invoiceNAVXML, Encoding.UTF8, NAVGlobal.XMLNamespaces);
            resNAVXChange.Operation = enNAVOperation.MANAGEINVOICE.ToString();
            resNAVXChange.Status = enNAVStatus.CREATED.ToString();
            await _dbContext.NAVXChange.AddAsync(resNAVXChange);
            await _dbContext.SaveChangesAsync();

            return resNAVXChange;

        }

        public async Task<NAVXChange> CreateNAVXChangeForManageAnnulmentAsynch(Invoice invoice, CancellationToken cancellationToken)
        {


            if (invoice == null)
            {
                throw new ResourceNotFoundException(bbxBEConsts.ERR_INVOICEISNULL);
            }
            if (invoice.Incoming || invoice.InvoiceType != enInvoiceType.INV.ToString())
            {
                throw new NAVException(string.Format(bbxBEConsts.ERR_NAVINV_ANULMENT, (invoice.InvoiceNumber)));
            }
            var currXChanges = await GetXChangeRecordByInvoiceID(invoice.ID);
            if (currXChanges.Any(a => a.Status == enNAVStatus.CREATED.ToString()))
            {
                throw new NAVException(string.Format(bbxBEConsts.ERR_INVOICEALREADYSETSEND, (invoice.InvoiceNumber)));

            }
            var resNAVXChange = new NAVXChange();

            var annulmentData = new InvoiceAnnulment(invoice.InvoiceNumber);
            var annulmentDataStr = XMLUtil.Object2XMLString<InvoiceAnnulment>(annulmentData, Encoding.UTF8, NAVGlobal.XMLNamespaces);

            resNAVXChange.InvoiceID = invoice.ID;
            resNAVXChange.InvoiceNumber = invoice.InvoiceNumber;
            resNAVXChange.InvoiceXml = annulmentDataStr;
            resNAVXChange.Operation = enNAVOperation.MANAGEANNULMENT.ToString();
            resNAVXChange.Status = enNAVStatus.CREATED.ToString();
            await _dbContext.NAVXChange.AddAsync(resNAVXChange);
            await _dbContext.SaveChangesAsync();

            return resNAVXChange;

        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedNAVXChangeAsync(QueryXChange requestParameter)
        {

            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetNAVXChangeViewModel, Invoice>();


            int recordsTotal, recordsFiltered;


            //var query = _dbContext.Invoice//.AsNoTracking().AsExpandable()
            //        .Include(i => i.Warehouse).AsQueryable();

            var query = _dbContext.NAVXChange.AsNoTracking()
                .Include(r => r.NAVXResults).AsNoTracking();

            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
            FilterNAVXChange(ref query, requestParameter.CreateTimeFrom, requestParameter.CreateTimeTo, requestParameter.InvoiceNumber, requestParameter.ErrorView);

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
            else
            {
                query = query.OrderBy(o => o.InvoiceNumber);
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                query = query.Select<NAVXChange>("new(" + fields + ")");
            }


            // retrieve data to list
            List<NAVXChange> resultData = await GetPagedData(query, requestParameter);


            //TODO: szebben megoldani

            var resultDataModel = new List<GetNAVXChangeViewModel>();
            resultData.ForEach(i =>
            {
                var xchg = _mapper.Map<NAVXChange, GetNAVXChangeViewModel>(i);
                if (i.NAVXResults != null)
                {
                    xchg.NAVXResults = new List<GetNAVXResultViewModel>();
                    i.NAVXResults.ToList().ForEach(res => xchg.NAVXResults.Add(_mapper.Map<NAVXResult, GetNAVXResultViewModel>(res)));
                }

                resultDataModel.Add(xchg);  //nem full data esetén is szüség van az invoiceLines-re
            }
            );

            var listFieldsModel = _modelHelper.GetModelFields<GetNAVXChangeViewModel>();

            var shapedData = _dataShaperGetNAVXChangeViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapedData, recordsCount);
        }

        private void FilterNAVXChange(ref IQueryable<NAVXChange> p_items,
            DateTime createTimeFrom, DateTime? createTimeTo, string invoiceNumber, bool errorView)
        {
            if (!p_items.Any())
                return;

            var predicate = PredicateBuilder.New<NAVXChange>();
            predicate = predicate.And(c =>
                            (c.CreateTime >= createTimeFrom && (!createTimeTo.HasValue || c.CreateTime.Date <= createTimeTo.Value))
                            && (string.IsNullOrWhiteSpace(invoiceNumber) || c.InvoiceNumber.ToUpper().StartsWith(invoiceNumber.ToUpper()))
                            && (!errorView || (c.NAVXResults.Any())));

            p_items = p_items.Where(predicate);
        }
    }
}