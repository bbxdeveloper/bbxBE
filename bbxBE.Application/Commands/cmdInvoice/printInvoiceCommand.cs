using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Common.NAV;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
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

    public class PrintInvoiceCommandHandler : IRequestHandler<PrintInvoiceCommand, FileStreamResult>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;

        public PrintInvoiceCommandHandler(IInvoiceRepositoryAsync invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<FileStreamResult> Handle(PrintInvoiceCommand request, CancellationToken cancellationToken)
        {

            var invoice = await _invoiceRepository.GetInvoiceRecordAsync(request.ID, invoiceQueryTypes.full);
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
            Enum.TryParse(invoice.InvoiceCategory, out InvoiceCategoryType invoiceCategory);

            switch (invoiceType)
            {
                case enInvoiceType.INC:
                    {
                        if (invoiceCategory == InvoiceCategoryType.NORMAL)
                        {
                            if (!invoice.InvoiceCorrection)
                            {
                                reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.InvoiceINC.trdx", Assembly.GetExecutingAssembly());
                            }
                            else
                            {
                                reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.InvoiceJSB.trdx", Assembly.GetExecutingAssembly());
                            }
                        }
                        else
                        {
                            reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.AggregateINC.trdx", Assembly.GetExecutingAssembly());
                        }
                        break;
                    }
                case enInvoiceType.DNI:
                    {
                        reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.InvoiceDNI.trdx", Assembly.GetExecutingAssembly());
                        break;
                    }
                case enInvoiceType.DNO:
                    {
                        reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.InvoiceDNO.trdx", Assembly.GetExecutingAssembly());
                        break;
                    }
                case enInvoiceType.BLK:
                    {
                        reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.InvoiceBLK.trdx", Assembly.GetExecutingAssembly());
                        break;
                    }
                case enInvoiceType.INV:
                default:
                    {
                        if (invoiceCategory == InvoiceCategoryType.NORMAL)
                        {
                            if (!invoice.InvoiceCorrection)
                            {
                                reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.Invoice.trdx", Assembly.GetExecutingAssembly());
                            }
                            else
                            {
                                reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.InvoiceJSK.trdx", Assembly.GetExecutingAssembly());
                            }
                        }
                        else
                        {
                            reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.AggregateINV.trdx", Assembly.GetExecutingAssembly());
                        }
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

                reportSource.Parameters.Add(new Telerik.Reporting.Parameter("JWT", ""));
                reportSource.Parameters.Add(new Telerik.Reporting.Parameter("InvoiceID", request.ID));
                reportSource.Parameters.Add(new Telerik.Reporting.Parameter("BaseURL", request.baseURL));

                ReportProcessor reportProcessor = new ReportProcessor();

                System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();

                Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", reportSource, deviceInfo);

                if (result == null)
                    throw new Exception(bbxBEConsts.ERR_INVOICEREPORT_NULL);
                if (result.Errors.Length > 0)
                    throw new Exception(string.Format(bbxBEConsts.ERR_INVOICEREPORT, result.Errors[0].Message));


                //Példányszám beállítása
                //
                invoice.Copies++;

                //Nullra vesszük a detail táblákat, csak az Invoice táblát kell menteni
                invoice.InvoiceLines = null;
                invoice.SummaryByVatRates = null;
                invoice.AdditionalInvoiceData = null;

                await _invoiceRepository.UpdateInvoiceAsync(invoice, null);

                //TODO : Az eredeti példány folderbe el kell rakni ay első a PDF-et
                Stream stream = new MemoryStream(result.DocumentBytes);
                var codPdf = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                foreach (PdfPage page in codPdf.Pages)
                {
                    resultPdf.AddPage(page);
                }

            }
            string fileName = $"Invoice{invoice.InvoiceNumber.Replace("/", "-")}.pdf";

            MemoryStream resultStream = new MemoryStream();
            resultPdf.Save(resultStream, false);
            resultPdf.Close();

            resultStream.Position = 0;

            var fsr = new FileStreamResult(resultStream, $"application/pdf") { FileDownloadName = fileName };

            return fsr;
        }
    }
}
