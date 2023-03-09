using AutoMapper;
using AutoMapper.Configuration.Conventions;
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
using bbxBE.Domain.Enums;
using bbxBE.Common.Enums;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace bbxBE.Application.Commands.cmdInvoice
{
    public class PrintAggregateInvoiceCommand : IRequest<FileStreamResult>
    {
        public long ID { get; set; }
        public string baseURL;
        public int Copies { get; set; } = 1;
    }

    public class PrintAggregateInvoiceCommandHandler : IRequestHandler<PrintAggregateInvoiceCommand, FileStreamResult>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;

        public PrintAggregateInvoiceCommandHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<FileStreamResult> Handle(PrintAggregateInvoiceCommand request, CancellationToken cancellationToken)
        {

            var invoice = await _invoiceRepository.GetInvoiceRecordAsync(request.ID, false);
            if (invoice == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, request.ID));
            }

            InstanceReportSource reportSource = null;
            Telerik.Reporting.Report rep = null;
            System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            string reportTRDX = String.Empty;

            Enum.TryParse(invoice.InvoiceType, out enInvoiceType invoiceType);

            switch (invoiceType)
            {
                case enInvoiceType.INC:
                {
                    reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.AggregateINC.trdx", Assembly.GetExecutingAssembly());
                        break;
                }
                case enInvoiceType.INV:
                default:
                {
                    reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.AggregateINV.trdx", Assembly.GetExecutingAssembly());
                        break;
                }
            }
            

            var resultPdf = new PdfDocument();
            for (int cp = 0; cp < request.Copies; cp++)
            {

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
                await _invoiceRepository.UpdateInvoiceAsync(invoice, null);

                //TODO : Az eredeti példány folderbe el kell rakni ay első a PDF-et
                Stream stream = new MemoryStream(result.DocumentBytes);
                var codPdf = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                foreach (PdfPage page in codPdf.Pages)
                {
                    resultPdf.AddPage(page);
                }

            }
            string fileName = $"AggregateInvoice{invoice.InvoiceNumber.Replace("/", "-")}.pdf";

            MemoryStream resultStream = new MemoryStream();
            resultPdf.Save(resultStream, false);
            resultPdf.Close();

            resultStream.Position = 0;

            var fsr = new FileStreamResult(resultStream, $"application/pdf") { FileDownloadName = fileName };

            return fsr;
        }
    }
}
