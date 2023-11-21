using bbxBE.Application.Wrappers;
using bbxBE.Common.Consts;
using bbxBE.Common.ExpiringData;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdCustomer
{
    public class UnlockAllCustomersCommand : IRequest<Response<string>>
    {

    }

    public class UnlockAllCustomersCommandHandler : IRequestHandler<UnlockAllCustomersCommand, Response<string>>
    {
        private readonly IExpiringData<ExpiringDataObject> _expiringData;
        public UnlockAllCustomersCommandHandler(IExpiringData<ExpiringDataObject> expiringData)
        {
            _expiringData = expiringData;
        }

        public async Task<Response<string>> Handle(UnlockAllCustomersCommand request, CancellationToken cancellationToken)
        {
            await _expiringData.DeleteItemsByKeyPartAsync(bbxBEConsts.DEF_CUSTOMERLOCK_KEY);
            var resp = new Response<string>(bbxBEConsts.DEF_CUSTOMERLOCK_KEY);
            resp.Succeeded = true;
            resp.Data = bbxBEConsts.DEF_CUSTOMERLOCK_KEY;
            return resp;
        }


    }
}
