using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Common.NAV;
using bbxBE.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace bbxBE.Application.Commands.cmdNAV
{
    public class sendInvoiceToNAVCommand : IRequest<Response<long>>
    {

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylatszám")]
        public string InvoiceNumber { get; set; }

    }

    public class sendInvoiceToNAVCommandHandler : IRequestHandler<sendInvoiceToNAVCommand, Response<long>>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private readonly NAVSettings _NAVSettings;
        private readonly ILogger<sendInvoiceToNAVCommand> _logger;


        public sendInvoiceToNAVCommandHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IOptions<NAVSettings> NAVSettings, ILogger<sendInvoiceToNAVCommand> logger, IConfiguration configuration)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _NAVSettings = NAVSettings.Value;
            _logger = logger;
            _configuration = configuration;

        }

        public async Task<Response<long>> Handle(sendInvoiceToNAVCommand request, CancellationToken cancellationToken)
        {

            var invoice = await _invoiceRepository.GetInvoiceRecordByInvoiceNumberAsync(request.InvoiceNumber, invoiceQueryTypes.full);
            if (invoice == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, (request.InvoiceNumber)));
            }
            var invoiceNAVXML = await _invoiceRepository.GetInvoiceNAVXMLAsynch(invoice, cancellationToken);
            var xmlStr = XMLUtil.Object2XMLString<InvoiceData>(invoiceNAVXML, Encoding.UTF8, NAVGlobal.XMLNamespaces);

            var bllNavObj = new bllNAV(_NAVSettings, _logger);

            //            var xml = bllNavObj.get


            return new Response<long>(1);
        }




    }
}
