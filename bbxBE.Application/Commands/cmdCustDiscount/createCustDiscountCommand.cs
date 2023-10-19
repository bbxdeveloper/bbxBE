﻿using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.ExpiringData;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdCustDiscount
{
    public class CreateCustDiscountCommand : IRequest<Response<List<CustDiscount>>>
    {
        public class CustDiscountItem
        {

            [ColumnLabel("Termékcsoport ID")]
            [Description("Termékcsoport ID")]
            public long ProductGroupID { get; set; }

            [ColumnLabel("Árengedmény %")]
            [Description("Árengedmény %)")]
            public decimal Discount { get; set; }
        }

        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }
        public List<CustDiscountItem> Items { get; set; } = new List<CustDiscountItem>();
    }

    public class CreateCustDiscountCommandHandler : IRequestHandler<CreateCustDiscountCommand, Response<List<CustDiscount>>>
    {
        private readonly ICustDiscountRepositoryAsync _custDiscountRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IExpiringData<ExpiringDataObject> _expiringData;

        public CreateCustDiscountCommandHandler(ICustDiscountRepositoryAsync custDiscountRepository,
                IMapper mapper,
                IConfiguration configuration,
                IExpiringData<ExpiringDataObject> expiringData)
        {
            _custDiscountRepository = custDiscountRepository;
            _mapper = mapper;
            _configuration = configuration;
            _expiringData = expiringData;
        }

        public async Task<Response<List<CustDiscount>>> Handle(CreateCustDiscountCommand request, CancellationToken cancellationToken)
        {
            var CustDiscountItems = new List<CustDiscount>();
            if (request.Items != null)
            {
                request.Items.ForEach(i =>
                {
                    if (i.ProductGroupID > 0)
                    {
                        var CustDiscount = _mapper.Map<CustDiscount>(i);
                        CustDiscount.CustomerID = request.CustomerID;
                        CustDiscountItems.Add(CustDiscount);
                    }
                }
                );
            }
            var res = await _custDiscountRepository.MaintanenceCustDiscountRangeAsync(CustDiscountItems, request.CustomerID, _expiringData);
            return new Response<List<CustDiscount>>(CustDiscountItems);
        }


    }
}
