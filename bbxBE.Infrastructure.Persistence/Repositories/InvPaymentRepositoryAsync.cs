using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public InvPaymentRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
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
    }
}