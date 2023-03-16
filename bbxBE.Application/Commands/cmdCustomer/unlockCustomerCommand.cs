using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.Commands.cmdImport;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Consts;
using bbxBE.Common.ExpiringData;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
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
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly IExpiringData<ExpiringDataObject> _expiringData;
        public UnlockCustomerCommandHandler(ICustomerRepositoryAsync customerRepository, IExpiringData<ExpiringDataObject> expiringData)
        {
            _customerRepository = customerRepository;
            _expiringData = expiringData;
        }

        public async Task<Response<string>> Handle(UnlockCustomerCommand request, CancellationToken cancellationToken)
        {
            var key = bbxBEConsts.DEF_CUSTOMERLOCK_KEY + request.ID.ToString();
            await _expiringData.DeleteItemAsync(key);
            var resp = new Response<string>(key);
            resp.Succeeded = true;
            return resp;
        }


    }
}
