using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Commands.cmdImport;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.Reporting.XmlSerialization;

namespace bbxBE.Application.Commands.cmdInvCtrlPeriod
{
    public class PrintInvCtrlPeriodCommand : IRequest<FileStreamResult>
    {
        public long InvCtrlPeriodID { get; set; }
        public string InvPeriodTitle { get; set; }
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
