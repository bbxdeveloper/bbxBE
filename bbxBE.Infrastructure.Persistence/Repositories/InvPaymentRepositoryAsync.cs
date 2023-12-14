﻿using AutoMapper;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qLocation;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Consts;
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

            using (var dbContextTransaction = await _dbContext.Instance.Database.BeginTransactionAsync())
            {
                try
                {


                    var AddInvCtrlItems = new List<InvPayment>();
                    var UpdInvCtrlItems = new List<InvPayment>();
                    var RemoveInvCtrlItems = new List<InvPayment>();

                    foreach (var invPayment in invPayments)
                    {

                        var existing = await _dbContext.InvPayment
                                       .Where(x => x.BankTransaction == invPayment.BankTransaction && !x.Deleted)
                                       .FirstOrDefaultAsync();

                        if (existing != null)
                        {
                            if (invPayment.InvPaymentAmount != 0)
                            {
                                invPayment.ID = existing.ID;           //A visszaadott értékhez

                                existing.InvPaymentDate = invPayment.InvPaymentDate;
                                existing.InvPaymentDate = invPayment.InvPaymentDate.Date;
                                existing.InvPaymentAmount = invPayment.InvPaymentAmount;

                                existing.InvPaymentAmountHUF = Math.Round(invPayment.InvPaymentAmount * existing.ExchangeRate, 1);

                                _dbContext.Instance.Entry(existing).State = EntityState.Modified;
                                UpdInvCtrlItems.Add(existing);
                            }
                            else
                            {
                                _dbContext.Instance.Entry(existing).State = EntityState.Deleted;

                                RemoveInvCtrlItems.Add(existing);
                            }
                        }
                        else
                        {
                            invPayment.InvPaymentDate = invPayment.InvPaymentDate.Date.Date;        //csak a dátum kell ide
                            invPayment.InvPaymentAmountHUF = Math.Round(invPayment.InvPaymentAmount * invPayment.ExchangeRate, 1);

                            _dbContext.Instance.Entry(invPayment).State = EntityState.Added;
                            AddInvCtrlItems.Add(invPayment);
                        }
                    }

                    if (AddInvCtrlItems.Count > 0)
                    {
                        await AddRangeAsync(AddInvCtrlItems);
                    }
                    if (UpdInvCtrlItems.Count > 0)
                    {
                        await UpdateRangeAsync(UpdInvCtrlItems);
                    }
                    if (RemoveInvCtrlItems.Count > 0)
                    {
                        await RemoveRangeAsync(RemoveInvCtrlItems);
                    }

                    await _dbContext.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();
                }
                catch (Exception)
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
                return invPayments;
            }
        }


        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvPaymentAsync(QueryInvPayment requestParameter, CancellationToken cancellationToken)
        {

            var searchString = requestParameter.SearchString;
            var orderBy = requestParameter.OrderBy;
            //      var fields = requestParameter.Fields;
            var fields = _modelHelper.GetQueryableFields<GetInvPaymentViewModel, Location>();


            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var query = _dbContext.InvPayment.AsNoTracking()
                        .Include(i => i.Invoice).AsNoTracking()
                        .Include(i => i.Invoice).ThenInclude(c => c.Customer).AsNoTracking()
                        .Include(i => i.Invoice).ThenInclude(s => s.Supplier).AsNoTracking()
                        .Where(w => !w.Deleted);

            ;

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
            if (!string.IsNullOrWhiteSpace(fields))
            {
                query = query.Select<InvPayment>("new(" + fields + ")");
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

        private void FilterBySearchString(ref IQueryable<InvPayment> p_item, string p_searchString)
        {
            if (!p_item.Any())
                return;

            if (string.IsNullOrWhiteSpace(p_searchString))
                return;

            var predicate = PredicateBuilder.New<InvPayment>();

            var srcFor = p_searchString.ToUpper().Trim();
            predicate = predicate.And(p => p.Invoice.InvoiceNumber.ToUpper().Contains(srcFor) || p.BankTransaction.ToUpper().Contains(srcFor));

            p_item = p_item.Where(predicate);
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

    }
}