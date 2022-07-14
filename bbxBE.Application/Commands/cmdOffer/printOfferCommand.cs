using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Commands.cmdImport;
using bbxBE.Application.Consts;
using bbxBE.Application.Exceptions;
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

namespace bbxBE.Application.Commands.cmdOffer
{
    public class PrintOfferCommand : IRequest<FileStreamResult>
    {
        public long ID { get; set; }
        public string baseURL;
    }

    public class PrintOfferCommandHandler : IRequestHandler<PrintOfferCommand, FileStreamResult>
    {
        private readonly IOfferRepositoryAsync _offerRepository;
        private readonly IMapper _mapper;

        public PrintOfferCommandHandler(IOfferRepositoryAsync offerRepository, IMapper mapper)
        {
            _offerRepository = offerRepository;
            _mapper = mapper;
        }

        public async Task<FileStreamResult> Handle(PrintOfferCommand request, CancellationToken cancellationToken)
        {         
            var reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.Offer.trdx", Assembly.GetExecutingAssembly());
            var res = await bllOffer.CreateOfferReportAsynch(_offerRepository, reportTRDX, request, cancellationToken);
    
            return res;
        }


    }
}
