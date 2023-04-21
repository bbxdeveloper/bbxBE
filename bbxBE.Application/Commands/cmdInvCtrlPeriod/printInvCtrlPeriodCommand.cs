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

namespace bbxBE.Application.Commands.cmdInvCtrlPeriod
{
    public class PrintInvCtrlPeriodCommand : IRequest<FileStreamResult>
    {
        [ColumnLabel("Leltáridőszak ID")]
        [Description("Leltáridőszak ID")]
        public long InvCtrlPeriodID { get; set; }

        [ColumnLabel("Leltáridőszak megnevezése")]
        [Description("Leltáridőszak megnevezése")]
        public string InvPeriodTitle { get; set; }

        [ColumnLabel("Backend URL")]
        [Description("Backend URL")]
        public string baseURL;
    }

    public class PrintInvCtrlPeriodCommandHandler : IRequestHandler<PrintInvCtrlPeriodCommand, FileStreamResult>
    {
        private readonly IInvCtrlPeriodRepositoryAsync _invCtrlPeriodRepository;
        private readonly IMapper _mapper;

        public PrintInvCtrlPeriodCommandHandler(IInvCtrlPeriodRepositoryAsync invCtrlPeriodRepository, IMapper mapper)
        {
            _invCtrlPeriodRepository = invCtrlPeriodRepository;
            _mapper = mapper;
        }

        public async Task<FileStreamResult> Handle(PrintInvCtrlPeriodCommand request, CancellationToken cancellationToken)
        {
            var reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.InvCtrlICPReport.trdx", Assembly.GetExecutingAssembly());
            var res = await bllInvCtrlPeriod.CreateInvCtrlPeriodReportAsynch(_invCtrlPeriodRepository, reportTRDX, request, cancellationToken);

            return res;
        }


    }
}
