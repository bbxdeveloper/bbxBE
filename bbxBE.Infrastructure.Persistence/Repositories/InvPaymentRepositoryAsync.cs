using AutoMapper;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qLocation;
using bbxBE.Application.Queries.ViewModels;
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
using System.Threading;
using System.Threading.Tasks;
using static bxBE.Application.Commands.cmdLocation.CreateInvPaymentCommand;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class InvPaymentRepositoryAsync : GenericRepositoryAsync<InvPayment>, IInvPaymentRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private IDataShapeHelper<GetInvPaymentViewModel> _dataShaperGetInvPaymentViewModel;

        public InvPaymentRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
            _dataShaperGetInvPaymentViewModel = new DataShapeHelper<GetInvPaymentViewModel>();
        }

        public async Task<List<InvPayment>> MaintainRangeAsync(List<InvPaymentItem> InvPaymentItems)
        {
            var invoices = new Dictionary<long, Invoice>();
            var invErrors = new List<string>();
            var invErrWrongType = new List<string>();
            var invPayments = new List<InvPayment>();

            foreach (var i in InvPaymentItems)
            {
                var inv = _dbContext.Invoice.Where(w => w.InvoiceNumber == i.InvoiceNumber).FirstOrDefault();
                if (inv != null)
                {
                    if (inv.PaymentMethod == PaymentMethodType.TRANSFER.ToString())     //csak átutalkásos bizonylatok írhatóak jóvá
                    {
                        var ip = _mapper.Map<InvPayment>(i);
                        ip.InvoiceID = inv.ID;
                        invPayments.Add(ip);
                    }
                    else
                    {
                        invErrWrongType.Add(i.InvoiceNumber);
                    }
                }
                else
                {
                    invErrors.Add(i.InvoiceNumber);
                }
                if (!invoices.ContainsKey(inv.ID))
                {
                    invoices.Add(inv.ID, inv);
                }
            }

            if (invErrors.Count > 0 || invErrWrongType.Count > 0)
            {
                var errMsg = "";
                if (invErrors.Count > 0)
                {
                    errMsg = string.Format(bbxBEConsts.ERR_INVPAYMENT_INVNOTFND, string.Join(",", invErrors));
                }
                if (invErrWrongType.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(errMsg))
                        errMsg += "\n";

                    errMsg += string.Format(bbxBEConsts.ERR_INVPAYMENT_WRONGTYPE, string.Join(",", invErrWrongType));
                }
                throw new ResourceNotFoundException(errMsg);
            }

            var AddInvCtrlItems = new List<InvPayment>();
            var UpdInvCtrlItems = new List<InvPayment>();
            var RemoveInvCtrlItems = new List<InvPayment>();

            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {



                    foreach (var invPayment in invPayments)
                    {

                        if (invPayment.CurrencyCode == enCurrencyCodes.HUF.ToString())
                        {
                            invPayment.ExchangeRate = 1;
                        }

                        InvPayment existingInvPayment = null;

                        if (!string.IsNullOrWhiteSpace(invPayment.BankTransaction))
                        {

                            existingInvPayment = await _dbContext.InvPayment
                                           .Where(x => x.BankTransaction == invPayment.BankTransaction && !x.Deleted)
                                           .FirstOrDefaultAsync();
                        }

                        if (existingInvPayment != null)
                        {
                            if (invPayment.InvPaymentAmount != 0)
                            {
                                invPayment.ID = existingInvPayment.ID;           //A visszaadott értékhez

                                existingInvPayment.InvPaymentDate = invPayment.InvPaymentDate;
                                existingInvPayment.InvPaymentDate = invPayment.InvPaymentDate.Date;
                                existingInvPayment.InvPaymentAmount = invPayment.InvPaymentAmount;
                                existingInvPayment.CurrencyCode = invPayment.CurrencyCode;
                                existingInvPayment.ExchangeRate = invPayment.ExchangeRate;
                                existingInvPayment.InvPaymentAmountHUF = Math.Round(invPayment.InvPaymentAmount * invPayment.ExchangeRate, 1);

                                var index = UpdInvCtrlItems.FindIndex(a => a.BankTransaction == existingInvPayment.BankTransaction);

                                if (index < 0)
                                {
                                    UpdInvCtrlItems.Add(existingInvPayment);
                                }
                                else
                                {
                                    UpdInvCtrlItems[index] = existingInvPayment;
                                }
                            }
                            else
                            {
                                var index = RemoveInvCtrlItems.FindIndex(a => a.BankTransaction == invPayment.BankTransaction);
                                if (index < 0)
                                {
                                    RemoveInvCtrlItems.Add(invPayment);
                                }
                                else
                                {
                                    RemoveInvCtrlItems[index] = invPayment;
                                }
                            }
                        }
                        else
                        {
                            invPayment.InvPaymentDate = invPayment.InvPaymentDate.Date.Date;        //csak a dátum kell ide
                            invPayment.InvPaymentAmountHUF = Math.Round(invPayment.InvPaymentAmount * invPayment.ExchangeRate, 1);

                            var paidItems = await _dbContext.InvPayment.AsNoTracking()
                                        .Where(w => w.InvoiceID == invPayment.InvoiceID && !w.Deleted).ToListAsync();
                            invPayment.PayableAmount = invoices[invPayment.InvoiceID].InvoiceGrossAmount - paidItems.Sum(s => s.InvPaymentAmount);
                            invPayment.PayableAmountHUF = invoices[invPayment.InvoiceID].InvoiceGrossAmountHUF - paidItems.Sum(s => s.InvPaymentAmountHUF);


                            var index = AddInvCtrlItems.FindIndex(a => a.BankTransaction == invPayment.BankTransaction);
                            if (index < 0)
                            {
                                AddInvCtrlItems.Add(invPayment);
                            }
                            else
                            {
                                AddInvCtrlItems[index] = invPayment;
                            }
                        }
                    }

                    if (AddInvCtrlItems.Count > 0)
                    {
                        await AddRangeAsync(AddInvCtrlItems, false);
                    }
                    if (UpdInvCtrlItems.Count > 0)
                    {
                        await UpdateRangeAsync(UpdInvCtrlItems, false);
                    }
                    if (RemoveInvCtrlItems.Count > 0)
                    {
                        await RemoveRangeAsync(RemoveInvCtrlItems, false);
                    }

                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();
                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
                var res = new List<InvPayment>();

                res.AddRange(AddInvCtrlItems);
                res.AddRange(UpdInvCtrlItems);
                res.AddRange(RemoveInvCtrlItems);

                return res;
            }
        }


        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvPaymentAsync(QueryInvPayment requestParameter, CancellationToken cancellationToken)
        {

            var orderBy = requestParameter.OrderBy;


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var query = _dbContext.InvPayment.AsNoTracking()
                        .Include(i => i.Invoice).AsNoTracking()
                        .Include(i => i.Invoice).ThenInclude(c => c.Customer).AsNoTracking()
                        .Include(i => i.Invoice).ThenInclude(s => s.Supplier).AsNoTracking()
                        .Include(u => u.User).AsNoTracking()
                        .Where(w => !w.Deleted);

            ;

            // Count records total
            recordsTotal = await query.CountAsync();

            // filter data
            PagedQueryFilter(ref query, requestParameter.SearchString, requestParameter.DateFrom, requestParameter.DateTo);

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

            // retrieve data to list
            var resultData = await GetPagedData(query, requestParameter);

            //TODO: szebben megoldani
            var resultDataModel = new List<GetInvPaymentViewModel>();
            resultData.ForEach(i => resultDataModel.Add(
               _mapper.Map<InvPayment, GetInvPaymentViewModel>(i))
            );


            var listFieldsModel = _modelHelper.GetModelFields<GetInvPaymentViewModel>();

            var shapedData = _dataShaperGetInvPaymentViewModel.ShapeData(resultDataModel, String.Join(",", listFieldsModel));

            return (shapedData, recordsCount);
        }

        private void PagedQueryFilter(ref IQueryable<InvPayment> p_item, string p_searchString, DateTime? DateFrom, DateTime? DateTo)
        {
            if (!p_item.Any())
                return;

            var predicate = PredicateBuilder.New<InvPayment>();

            var srcFor = "";
            if (p_searchString != null)
            {
                srcFor = p_searchString.ToUpper().Trim();
            }
            predicate = predicate.And(p => (string.IsNullOrWhiteSpace(srcFor) || p.Invoice.InvoiceNumber.ToUpper().Contains(srcFor) || p.BankTransaction.ToUpper().Contains(srcFor))
                                           && (!DateFrom.HasValue || p.InvPaymentDate >= DateFrom.Value)
                                           && (!DateTo.HasValue || p.InvPaymentDate <= DateTo.Value));



            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

    }
}