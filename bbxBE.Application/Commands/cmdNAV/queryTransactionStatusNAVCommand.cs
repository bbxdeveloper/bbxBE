using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
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

        private readonly ILoggerFactory _loggerFactory;
        private readonly NAVSettings _NAVSettings;

        public queryTransactionStatusNAVCommandHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IOptions<NAVSettings> NAVSettings, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _NAVSettings = NAVSettings.Value;
            _loggerFactory = loggerFactory;
            _configuration = configuration;

        }

        public async Task<Response<NAVXChange>> Handle(queryTransactionStatusNAVCommand request, CancellationToken cancellationToken)
        {



            var bllNavObj = new bllNAV(_NAVSettings, _loggerFactory);

            var resNAVXChange = new NAVXChange();
            bllNavObj.QueryTransactionStatus(request.TransactionID, resNAVXChange);


            return new Response<NAVXChange>(resNAVXChange);
        }




    }
}
