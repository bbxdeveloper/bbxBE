using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.Commands.cmdImport;
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
        public string OfferNumber { get; set; }

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

            var reportTRDX = loadEmbeddedResource("bbxBE.Application.Reports.Offer.trdx");

            InstanceReportSource reportSource = null;
            Telerik.Reporting.Report rep = null;

            

            System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (System.Xml.XmlReader xmlReader = XmlReader.Create(new StringReader(reportTRDX), settings))
            //using (System.Xml.XmlReader xmlReader =   System.Xml.XmlReader.Create(@"Reports/Invoice.trdx", settings))
            {
                ReportXmlSerializer xmlSerializer = new ReportXmlSerializer();
                rep = (Telerik.Reporting.Report)xmlSerializer.Deserialize(xmlReader);
                reportSource = new Telerik.Reporting.InstanceReportSource();

                reportSource.ReportDocument = rep;
            }

            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("OfferID", request.ID));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("BaseURL", request.baseURL));

            ReportProcessor reportProcessor = new ReportProcessor();

            System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();

            Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", reportSource, deviceInfo);

            if (result == null)
                throw new Exception("Offer report result is null!");


            Stream stream = new MemoryStream(result.DocumentBytes);
            string fileName = "Offer.pdf";

             
            var fsr = new FileStreamResult(stream, $"application/pdf") { FileDownloadName = fileName };

            return fsr;
        }


        protected string loadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
