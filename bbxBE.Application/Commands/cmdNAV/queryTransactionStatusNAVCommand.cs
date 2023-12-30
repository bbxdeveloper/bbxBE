using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;


namespace bbxBE.Application.Commands.cmdNAV
{
    public class queryTransactionStatusNAVCommand : IRequest<Response<NAVXChange>>
    {

        [ColumnLabel("Tranzakció ID")]
        [Description("NAV tranzakció ID")]
        public string TransactionID { get; set; }

    }

    public class queryTransactionStatusNAVCommandHandler : IRequestHandler<queryTransactionStatusNAVCommand, Response<NAVXChange>>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private readonly ILogger _logger;
        private readonly NAVSettings _NAVSettings;

        public queryTransactionStatusNAVCommandHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IOptions<NAVSettings> NAVSettings, ILogger logger, IConfiguration configuration)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _NAVSettings = NAVSettings.Value;
            _logger = logger;
            _configuration = configuration;

        }

        public async Task<Response<NAVXChange>> Handle(queryTransactionStatusNAVCommand request, CancellationToken cancellationToken)
        {
            var bllNavObj = new bllNAV(_NAVSettings, _logger);

            var NAVXChange = new NAVXChange();
            NAVXChange.TransactionID = request.TransactionID;

            bllNavObj.QueryTransactionStatusByXChange(NAVXChange);

            return new Response<NAVXChange>(NAVXChange);
        }
    }
}
