using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;


namespace bbxBE.Application.Commands.cmdNAV
{
    public class manageAnnulmentNAVCommand : IRequest<Response<NAVXChange>>
    {

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylatszám")]
        public string InvoiceNumber { get; set; }

    }

    public class manageAnnulmentNAVCommandHandler : IRequestHandler<manageAnnulmentNAVCommand, Response<NAVXChange>>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private readonly ILoggerFactory _loggerFactory;
        private readonly NAVSettings _NAVSettings;

        public manageAnnulmentNAVCommandHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IOptions<NAVSettings> NAVSettings, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _NAVSettings = NAVSettings.Value;
            _loggerFactory = loggerFactory;
            _configuration = configuration;

        }

        public async Task<Response<NAVXChange>> Handle(manageAnnulmentNAVCommand request, CancellationToken cancellationToken)
        {


            var invoice = await _invoiceRepository.GetInvoiceRecordByInvoiceNumberAsync(request.InvoiceNumber, invoiceQueryTypes.full);
            if (invoice == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, (request.InvoiceNumber)));
            }
            if (invoice.Incoming || invoice.InvoiceType != enInvoiceType.INV.ToString())
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_NAVINV, (request.InvoiceNumber)));
            }

            var bllNavObj = new bllNAV(_NAVSettings, _loggerFactory);

            var resNAVXChange = bllNavObj.SendManageAnnulmentFull(invoice);


            return new Response<NAVXChange>(resNAVXChange);
        }




    }
}
