using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

namespace bbxBE.Application.Queries.qInvoice
{

    public class PrintCustomerInvoiceSummaryCommand : IRequest<FileStreamResult>
    {
        [ColumnLabel("B/K")]
        [Description("Bejővő/Kimenő")]
        public bool Incoming { get; set; }

        [ColumnLabel("Ügyfél")]
        [Description("Ügyfél ID")]
        public long? CustomerID { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Teljesítés tól")]
        [Description("Teljesítés dátumától")]
        public DateTime? InvoiceDeliveryDateFrom { get; set; }

        [ColumnLabel("Teljesítés ig")]
        [Description("Teljesítés dátumig")]
        public DateTime? InvoiceDeliveryDateTo { get; set; }

        [JsonIgnore]
        [ColumnLabel("JWT")]
        [Description("JWT")]
        public string JWT;

        [JsonIgnore]
        [ColumnLabel("Backend URL")]
        [Description("Backend URL")]
        public string baseURL;
    }
    public class PrintCustomerInvoiceSummaryCommandHandler : IRequestHandler<PrintCustomerInvoiceSummaryCommand, FileStreamResult>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly IWarehouseRepositoryAsync _warehouseRepository;
        private readonly IMapper _mapper;

        public PrintCustomerInvoiceSummaryCommandHandler(IInvoiceRepositoryAsync invoiceRepository,
            ICustomerRepositoryAsync customerRepository,
            IWarehouseRepositoryAsync warehouseRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        public async Task<FileStreamResult> Handle(PrintCustomerInvoiceSummaryCommand request, CancellationToken cancellationToken)
        {
            var customerName = "";
            if (request.CustomerID.HasValue)
            {
                var customer = _customerRepository.GetCustomerRecord(request.CustomerID.Value);
                if (customer == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_CUSTOMERNOTFOUND, request.CustomerID.Value));
                }

                customerName = customer.CustomerName;
            }

            var warehouseName = "";
            if (!string.IsNullOrWhiteSpace(request.WarehouseCode))
            {
                var warehouse = await _warehouseRepository.GetWarehouseByCodeAsync(request.WarehouseCode);
                if (warehouse == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.WarehouseCode));
                }
                warehouseName = warehouse.WarehouseDescription;
            }

            InstanceReportSource reportSource = null;
            Telerik.Reporting.Report rep = null;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            string reportTRDX = string.Empty;

            reportTRDX = Utils.LoadEmbeddedResource("bbxBE.Application.Reports.CustomerInvoiceSummary.trdx", Assembly.GetExecutingAssembly());

            var resultPdf = new PdfDocument();


            using (XmlReader xmlReader = XmlReader.Create(new StringReader(reportTRDX), settings))
            {
                ReportXmlSerializer xmlSerializer = new ReportXmlSerializer();
                rep = (Telerik.Reporting.Report)xmlSerializer.Deserialize(xmlReader);
                reportSource = new InstanceReportSource();

                reportSource.ReportDocument = rep;
            }

            reportSource.Parameters.Add(new Telerik.Reporting.Parameter(bbxBEConsts.JWT_REPPARAMETER, string.Format(bbxBEConsts.JWT_BEARER, request.JWT)));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("BaseURL", request.baseURL));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("Incoming", request.Incoming));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("InvoiceDeliveryDateFrom", request.InvoiceDeliveryDateFrom));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("InvoiceDeliveryDateTo", request.InvoiceDeliveryDateTo));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("CustomerID", request.CustomerID));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("CustomerName", customerName));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("WarehouseCode", request.WarehouseCode == null ? "" : request.WarehouseCode));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("WarehouseName", warehouseName));


            ReportProcessor reportProcessor = new ReportProcessor();

            System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();

            RenderingResult result = reportProcessor.RenderReport("PDF", reportSource, deviceInfo);

            if (result == null)
                throw new Exception(bbxBEConsts.ERR_CUSTOMERINVOICESUMMARYREPORT_NULL);
            if (result.Errors.Length > 0)
                throw new Exception(string.Format(bbxBEConsts.ERR_CUSTOMERINVOICESUMMARYREPORT, result.Errors[0].Message));


            //TODO : Az eredeti példány folderbe el kell rakni ay első a PDF-et
            Stream stream = new MemoryStream(result.DocumentBytes);
            var codPdf = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
            foreach (PdfPage page in codPdf.Pages)
            {
                resultPdf.AddPage(page);
            }

            string fileName = $"CustomerInvoiceSummary{request.InvoiceDeliveryDateFrom?.ToString(bbxBEConsts.DEF_DATEFORMAT)}-{request.InvoiceDeliveryDateTo?.ToString(bbxBEConsts.DEF_DATEFORMAT)}.pdf";

            MemoryStream resultStream = new MemoryStream();
            resultPdf.Save(resultStream, false);
            resultPdf.Close();

            resultStream.Position = 0;

            var fsr = new FileStreamResult(resultStream, $"application/pdf") { FileDownloadName = fileName };

            return fsr;
        }
    }
}
