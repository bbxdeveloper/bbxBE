using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
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
    public class sendInvoiceToNAVCommand : IRequest<Response<NAVXChange>>
    {

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylatszám")]
        public string InvoiceNumber { get; set; }

    }

    public class sendInvoiceToNAVCommandHandler : IRequestHandler<sendInvoiceToNAVCommand, Response<NAVXChange>>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly INAVXChangeRepositoryAsync _NAVXChangeRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private readonly ILoggerFactory _loggerFactory;
        private readonly NAVSettings _NAVSettings;

        public sendInvoiceToNAVCommandHandler(IInvoiceRepositoryAsync invoiceRepository, INAVXChangeRepositoryAsync NAVXChangeRepository, IMapper mapper, IOptions<NAVSettings> NAVSettings, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _invoiceRepository = invoiceRepository;
            _NAVXChangeRepository = NAVXChangeRepository;
            _mapper = mapper;
            _NAVSettings = NAVSettings.Value;
            _loggerFactory = loggerFactory;
            _configuration = configuration;

        }

        public async Task<Response<NAVXChange>> Handle(sendInvoiceToNAVCommand request, CancellationToken cancellationToken)
        {

            var invoice = await _invoiceRepository.GetInvoiceRecordByInvoiceNumberAsync(request.InvoiceNumber, invoiceQueryTypes.full);
            if (invoice == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, (request.InvoiceNumber)));
            }

            var resNAVXChange = await _NAVXChangeRepository.CreateNAVXChangeForManageInvoiceAsynch(invoice, cancellationToken);

            return new Response<NAVXChange>(resNAVXChange);
        }




    }
}
