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

namespace bbxBE.Application.Commands.cmdInvCtrl
{
    public class PrintInvCtrlCommand : IRequest<FileStreamResult>
    {
        [ColumnLabel("Leltáridőszak ID")]
        [Description("Leltáridőszak ID")]
        public long InvCtrlPeriodID { get; set; }

        [ColumnLabel("Leltáridőszak megnevezése")]
        [Description("Leltáridőszak megnevezése")]
        public string InvPeriodTitle { get; set; }

        [ColumnLabel("Csak készleten lévőt?")]
        [Description("Csak készleten lévőt?")]
        public bool IsInStock { get; set; }


        [ColumnLabel("Backend URL")]
        [Description("Backend URL")]
        public string baseURL;
    }

    public class PrintInvCtrlCommandHandler : IRequestHandler<PrintInvCtrlCommand, FileStreamResult>
    {
        private readonly IInvCtrlRepositoryAsync _invCtrlRepository;
        private readonly IMapper _mapper;

        public PrintInvCtrlCommandHandler(IInvCtrlRepositoryAsync invCtrlRepository, IMapper mapper)
        {
            _invCtrlRepository = invCtrlRepository;
            _mapper = mapper;
        }

        public async Task<FileStreamResult> Handle(PrintInvCtrlCommand request, CancellationToken cancellationToken)
        {
            var reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.InvCtrlICP.trdx", Assembly.GetExecutingAssembly());
            var res = await bllInvCtrl.CreateInvCtrlReportAsynch(_invCtrlRepository, reportTRDX, request, cancellationToken);

            return res;
        }


    }
}
