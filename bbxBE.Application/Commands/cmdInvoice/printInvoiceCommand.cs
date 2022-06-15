using AutoMapper;
using AutoMapper.Configuration.Conventions;
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

namespace bbxBE.Application.Commands.cmdInvoice
{
    public class PrintInvoiceCommand : IRequest<FileStreamResult>
    {
        public long ID { get; set; }
        public string InvoiceNumber { get; set; }

        public string baseURL;
    }

    public class PrintInvoiceCommandHandler : IRequestHandler<PrintInvoiceCommand, FileStreamResult>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;

        public PrintInvoiceCommandHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<FileStreamResult> Handle(PrintInvoiceCommand request, CancellationToken cancellationToken)
        {

            var invoice = await _invoiceRepository.GetInvoiceRecordAsync(request.ID, false);
            if (invoice == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, request.ID));
            }


            var reportTRDX = loadEmbeddedResource("bbxBE.Application.Reports.Invoice.trdx");

            InstanceReportSource reportSource = null;
            Telerik.Reporting.Report rep = null;



            System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (System.Xml.XmlReader xmlReader = XmlReader.Create(new StringReader(reportTRDX), settings))
            {
                ReportXmlSerializer xmlSerializer = new ReportXmlSerializer();
                rep = (Telerik.Reporting.Report)xmlSerializer.Deserialize(xmlReader);
                reportSource = new Telerik.Reporting.InstanceReportSource();

                reportSource.ReportDocument = rep;
            }

            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("InvoiceID", request.ID));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("BaseURL", request.baseURL));

            ReportProcessor reportProcessor = new ReportProcessor();

            System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();

            Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", reportSource, deviceInfo);

            if (result == null)
                throw new Exception("Invoice report result is null!");
            if (result.Errors.Length > 0)
                throw new Exception("Invoice report finished with error:" + result.Errors[0].Message);


            //Példányszám beállítása
            //
            invoice.Copies++;
            await _invoiceRepository.UpdateInvoiceAsync(invoice);

            Stream stream = new MemoryStream(result.DocumentBytes);
            string fileName = $"Invoice{invoice.InvoiceNumber.Replace("/", "-")}.pdf";

             
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
