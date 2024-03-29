﻿using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Common.ExpiringData;
using MediatR;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdCustomer
{
    public class LockCustomerCommand : IRequest<Response<string>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public string SessionID { get; set; }

    }

    public class LockCustomerCommandHandler : IRequestHandler<LockCustomerCommand, Response<string>>
    {
        private readonly IExpiringData<ExpiringDataObject> _expiringData;
        public LockCustomerCommandHandler(IExpiringData<ExpiringDataObject> expiringData)
        {
            _expiringData = expiringData;
        }

        public async Task<Response<string>> Handle(LockCustomerCommand request, CancellationToken cancellationToken)
        {
            var key = bbxBEConsts.DEF_CUSTOMERLOCK_KEY + request.ID.ToString();
            await _expiringData.AddOrUpdateItemAsync(key, request.ID, request.SessionID, TimeSpan.FromSeconds(bbxBEConsts.CustomerLockExpirationSec));
            var resp = new Response<string>(key);
            resp.Succeeded = true;
            resp.Data = key;
            return resp;
        }


    }
}
