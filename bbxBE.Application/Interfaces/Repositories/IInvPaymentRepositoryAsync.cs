using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using static bxBE.Application.Commands.cmdLocation.CreateInvPaymentCommand;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IInvPaymentRepositoryAsync : IGenericRepositoryAsync<InvPayment>
    {
        Task<List<InvPayment>> MaintainRangeAsync(List<InvPaymentItem> InvPaymentItems);

    }
}