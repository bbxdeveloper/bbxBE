using bbxBE.Application.Wrappers;
using bbxBE.Common.Consts;
using bbxBE.Common.ExpiringData;
using MediatR;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdCustomer
{
    public class UnlockCustomerCommand : IRequest<Response<string>>
    {
        public long ID { get; set; }
        [JsonIgnore]
        public string SessionID { get; set; }

    }

    public class UnlockCustomerCommandHandler : IRequestHandler<UnlockCustomerCommand, Response<string>>
    {
        private readonly IExpiringData<ExpiringDataObject> _expiringData;
        public UnlockCustomerCommandHandler(IExpiringData<ExpiringDataObject> expiringData)
        {
            _expiringData = expiringData;
        }

        public async Task<Response<string>> Handle(UnlockCustomerCommand request, CancellationToken cancellationToken)
        {
            var key = bbxBEConsts.DEF_CUSTOMERLOCK_KEY + request.ID.ToString();
            await _expiringData.DeleteItemAsync(key);
            var resp = new Response<string>(key);
            resp.Succeeded = true;
            resp.Data = key;
            return resp;
        }


    }
}
