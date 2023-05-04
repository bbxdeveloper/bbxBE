using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.NAV;
using bbxBE.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdInvoice
{
    public class importFromNAVCommand : IRequest<Response<long>>
    {

        [ColumnLabel("Bejővő/Kimenő")]
        [Description("INBOUND/OUTBOUND")]
        public string InvoiceDirection { get; set; }    //InvoiceDirectionType 

        [ColumnLabel("Kelt kezdet")]
        [Description("Kiállítás dátuma kezdet")]
        public DateTime IssueDateFrom { get; set; }

        [ColumnLabel("Kelt vége")]
        [Description("Kiállítás dátuma vége")]
        public DateTime IssueDateTo { get; set; }

    }

    public class getIncomingInvoicesNAVCommandHandler : IRequestHandler<importFromNAVCommand, Response<long>>
    {
        private readonly IInvoiceRepositoryAsync _InvoiceRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private readonly NAVSettings _NAVSettings;
        private readonly ILogger<importFromNAVCommand> _logger;


        public getIncomingInvoicesNAVCommandHandler(IInvoiceRepositoryAsync InvoiceRepository, IMapper mapper, IOptions<NAVSettings> NAVSettings, ILogger<importFromNAVCommand> logger, IConfiguration configuration)
        {
            _InvoiceRepository = InvoiceRepository;
            _mapper = mapper;
            _NAVSettings = NAVSettings.Value;
            _logger = logger;
            _configuration = configuration;

        }

        public async Task<Response<long>> Handle(importFromNAVCommand request, CancellationToken cancellationToken)
        {
            var bllNavObj = new bllNAV(_NAVSettings, _logger);
            var invoicesFromNav = bllNavObj.QueryInvoiceDigest(request);
            if (invoicesFromNav != null)
            {
                foreach (var invItem in invoicesFromNav)
                {
                    var invoiceDataFromNav = bllNavObj.QueryInvoiceData(invItem.invoiceNumber, Enum.Parse<InvoiceDirectionType>(request.InvoiceDirection));

                }

            }

            return new Response<long>(invoicesFromNav.Count());
        }




    }
}
