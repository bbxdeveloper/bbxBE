using AutoMapper;
using bbxBE.Application.Commands.cmdWhsTransfer;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdWhsTransfer;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.Reporting.XmlSerialization;

namespace bbxBE.Application.BLL
{
    public static class bllWhsTransfer
    {
        public static async Task<WhsTransfer> CreateWhsTransferAsynch(CreateWhsTransferCommand request,
                         IMapper mapper,
                         IWhsTransferRepositoryAsync whsTransferRepository,
                         IWarehouseRepositoryAsync warehouseRepository,
                         ICounterRepositoryAsync counterRepository,
                         IProductRepositoryAsync productRepository,
                         CancellationToken cancellationToken)
        {
            var whsTransfer = mapper.Map<WhsTransfer>(request);
            var counterCode = "";
            try
            {
                await prepareWhsTransferAsynch(whsTransfer, request.FromWarehouseCode, request.ToWarehouseCode, warehouseRepository, productRepository, cancellationToken);


                var prefix = "WHT";
                var whs = whsTransfer.FromWarehouseID.ToString().PadLeft(3, '0');
                counterCode = String.Format($"{prefix}_{whs}");
                whsTransfer.WhsTransferNumber = await counterRepository.GetNextValueAsync(counterCode, whsTransfer.FromWarehouseID);
                whsTransfer.Copies = 1;
                whsTransfer.WhsTransferStatus = enWhsTransferStatus.READY.ToString();

                await whsTransferRepository.AddWhsTransferAsync(whsTransfer);
                await counterRepository.FinalizeValueAsync(counterCode, whsTransfer.FromWarehouseID, whsTransfer.WhsTransferNumber);
                return whsTransfer;

            }
            catch (Exception)
            {
                if (!string.IsNullOrWhiteSpace(whsTransfer.WhsTransferNumber) && !string.IsNullOrWhiteSpace(counterCode))
                {
                    await counterRepository.RollbackValueAsync(counterCode, whsTransfer.FromWarehouseID, whsTransfer.WhsTransferNumber);
                }
                throw;
            }
        }

        public static async Task<WhsTransfer> UpdateWhsTransferAsynch(UpdateWhsTransferCommand request,
                 IMapper mapper,
                 IWhsTransferRepositoryAsync whsTransferRepository,
                 IWarehouseRepositoryAsync warehouseRepository,
                 ICounterRepositoryAsync counterRepository,
                 IProductRepositoryAsync productRepository,
                 CancellationToken cancellationToken)
        {
            var whsTransfer = mapper.Map<WhsTransfer>(request);
            var counterCode = "";
            try
            {
                await prepareWhsTransferAsynch(whsTransfer, request.FromWarehouseCode, request.ToWarehouseCode, warehouseRepository, productRepository, cancellationToken);

                whsTransfer = await whsTransferRepository.UpdateWhsTransferAsync(whsTransfer);
                return whsTransfer;

            }
            catch (Exception)
            {
                if (!string.IsNullOrWhiteSpace(whsTransfer.WhsTransferNumber) && !string.IsNullOrWhiteSpace(counterCode))
                {
                    await counterRepository.RollbackValueAsync(counterCode, whsTransfer.FromWarehouseID, whsTransfer.WhsTransferNumber);
                }
                throw;
            }
        }

        public static async Task<WhsTransfer> ProcessWhsTransferAsynch(ProcessWhsTransferCommand request,
                 IMapper mapper,
                 IWhsTransferRepositoryAsync whsTransferRepository,
                 IStockRepositoryAsync stockRepository,
                 CancellationToken cancellationToken)
        {
            var whsTransfer = await whsTransferRepository.GetWhsTransferRecordAsync(request.ID, true);
            if (whsTransfer == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WHSTRANSFERNOTFOUND, request.ID));
            }

            try
            {

                var stockList = await stockRepository.MaintainStockByInvoiceAsync(p_invoice);


                await prepareWhsTransferAsynch(whsTransfer, request.FromWarehouseCode, request.ToWarehouseCode, warehouseRepository, productRepository, cancellationToken);

                whsTransfer = await whsTransferRepository.UpdateWhsTransferAsync(whsTransfer);
                return whsTransfer;

            }
            catch (Exception)
            {
                if (!string.IsNullOrWhiteSpace(whsTransfer.WhsTransferNumber) && !string.IsNullOrWhiteSpace(counterCode))
                {
                    await counterRepository.RollbackValueAsync(counterCode, whsTransfer.FromWarehouseID, whsTransfer.WhsTransferNumber);
                }
                throw;
            }
        }


        private static async Task prepareWhsTransferAsynch(WhsTransfer whsTransfer,
                        string fromWarehouseCode, string toWarehouseCode,
                        IWarehouseRepositoryAsync warehouseRepository,
                        IProductRepositoryAsync productRepository,
                        CancellationToken cancellationToken)
        {
            try
            {

                var fromWarehouse = await warehouseRepository.GetWarehouseByCodeAsync(fromWarehouseCode);
                if (fromWarehouse == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, fromWarehouseCode));
                }
                whsTransfer.FromWarehouseID = fromWarehouse.ID;



                var toWarehouse = await warehouseRepository.GetWarehouseByCodeAsync(toWarehouseCode);
                if (toWarehouse == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, toWarehouseCode));
                }
                whsTransfer.ToWarehouseID = toWarehouse.ID;

                //Tételsorok előfeldolgozása
                var lineErrors = new List<string>();
                foreach (var ln in whsTransfer.WhsTransferLines)
                {
                    var rln = whsTransfer.WhsTransferLines.SingleOrDefault(i => i.WhsTransferLineNumber == ln.WhsTransferLineNumber);

                    var prod = productRepository.GetProductByProductCode(rln.ProductCode);
                    if (prod == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODCODENOTFOUND, rln.ProductCode));
                    }

                    ln.ProductID = prod.ID;
                    ln.ProductCode = rln.ProductCode;
                    //ln.Product = prod;

                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<FileStreamResult> CreateWhsTransferReportAsynch(IWhsTransferRepositoryAsync _whsTransferRepositoryAsync, string reportTRDX, PrintWhsTransferCommand request, CancellationToken cancellationToken)
        {

            var whsTransfer = await _whsTransferRepositoryAsync.GetWhsTransferRecordAsync(request.ID, false);
            if (whsTransfer == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WHSTRANSFERNOTFOUND, request.ID));
            }


            var resultPdf = new PdfDocument();
            for (int cp = 0; cp < request.Copies; cp++)
            {

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

                reportSource.Parameters.Add(new Telerik.Reporting.Parameter("ID", request.ID));
                reportSource.Parameters.Add(new Telerik.Reporting.Parameter("BaseURL", request.baseURL));

                ReportProcessor reportProcessor = new ReportProcessor();

                System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();

                //throw new Exception(String.Join(",", reportSource.Parameters) + "::::" + ((Telerik.Reporting.ReportItemBase)reportSource.ReportDocument).Name);

                Telerik.Reporting.Processing.RenderingResult result = null;
                try
                {
                    result = reportProcessor.RenderReport("PDF", reportSource, deviceInfo);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (result == null)
                    throw new Exception("Offer report result is null!");

                if (result.HasErrors)
                    throw new Exception("Report engine has some reference ERROR!");

                //Példányszám beállítása
                //
                whsTransfer.Copies++;
                await _whsTransferRepositoryAsync.UpdateAsync(whsTransfer);

                Stream stream = new MemoryStream(result.DocumentBytes);
                var codPdf = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                foreach (PdfPage page in codPdf.Pages)
                {
                    resultPdf.AddPage(page);
                }
            }


            string fileName = $"Offer{whsTransfer.WhsTransferNumber.Replace("/", "-")}.pdf";

            MemoryStream resultStream = new MemoryStream();
            resultPdf.Save(resultStream, false);
            resultPdf.Close();

            resultStream.Position = 0;

            var fsr = new FileStreamResult(resultStream, $"application/pdf") { FileDownloadName = fileName };
            return fsr;
        }

    }
}
