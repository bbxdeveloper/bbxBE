using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdLocation
{
    public class CreateInvPaymentCommand : IRequest<Response<List<InvPayment>>>
    {
        public class InvPaymentItem
        {
            [ColumnLabel("Számla")]
            [Description("Számla")]
            public string InvoiceNumber { get; set; }

            [ColumnLabel("Banki tranzakció")]
            [Description("Banki tranzakció azonosító")]
            public string BankTransaction { get; set; }

            [ColumnLabel("Dátum")]
            [Description("Banki tranzakció dátuma")]
            public DateTime InvPaymentDate { get; set; }

            [ColumnLabel("Pénznem")]
            [Description("Pénznem")]
            public string CurrencyCode { get; set; }

            [ColumnLabel("Árfolyam")]
            [Description("Árfolyam")]
            public decimal ExchangeRate { get; set; }

            [ColumnLabel("Összeg")]
            [Description("Banki tranzakció összege")]
            public decimal InvPaymentAmount { get; set; }

            [ColumnLabel("Felhasználó ID")]
            [Description("Felhasználó ID")]
            public long UserID { get; set; } = 0;
        }

        public List<InvPaymentItem> InvPaymentItems { get; set; } = new List<InvPaymentItem>();
    }

    public class CreateInvPaymentCommandHandler : IRequestHandler<CreateInvPaymentCommand, Response<List<InvPayment>>>
    {
        private readonly IInvPaymentRepositoryAsync _invPaymentRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateInvPaymentCommandHandler(IInvPaymentRepositoryAsync invPaymentRepositoryAsync, IMapper mapper, IConfiguration configuration)
        {
            _invPaymentRepositoryAsync = invPaymentRepositoryAsync;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<List<InvPayment>>> Handle(CreateInvPaymentCommand request, CancellationToken cancellationToken)
        {

            var res = await _invPaymentRepositoryAsync.MaintainRangeAsync(request.InvPaymentItems);
            return new Response<List<InvPayment>>(res);
        }


    }
}
