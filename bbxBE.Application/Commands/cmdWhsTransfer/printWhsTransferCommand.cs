using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common;
using bbxBE.Common.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdWhsTransfer
{
    public class PrintWhsTransferCommand : IRequest<FileStreamResult>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

        [ColumnLabel("Backend URL")]
        [Description("Backend URL")]
        public string baseURL;

        [ColumnLabel("Példány")]
        [Description("Példány")]
        public int Copies { get; set; } = 1;
    }

    public class PrintWhsTransferCommandHandler : IRequestHandler<PrintWhsTransferCommand, FileStreamResult>
    {
        private readonly IWhsTransferRepositoryAsync _whsTransferRepositoryAsync;
        private readonly IMapper _mapper;

        public PrintWhsTransferCommandHandler(IWhsTransferRepositoryAsync whsTransferRepositoryAsync, IMapper mapper)
        {
            _whsTransferRepositoryAsync = whsTransferRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<FileStreamResult> Handle(PrintWhsTransferCommand request, CancellationToken cancellationToken)
        {
            var reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.WarehouseTransfer.trdx", Assembly.GetExecutingAssembly());
            var res = await bllWhsTransfer.CreateWhsTransferReportAsynch(_whsTransferRepositoryAsync, reportTRDX, request, cancellationToken);

            return res;
        }


    }
}
