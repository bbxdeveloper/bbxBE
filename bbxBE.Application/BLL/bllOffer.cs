using bbxBE.Application.Commands.cmdOffer;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.Reporting.XmlSerialization;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace bbxBE.Application.BLL
{
    public static class bllOffer
    {

        public static async Task<FileStreamResult> CreateOfferReportAsynch(IOfferRepositoryAsync _offerRepository, string reportTRDX,  PrintOfferCommand request, CancellationToken cancellationToken)
        {

            var offer = await _offerRepository.GetOfferRecordAsync(request.ID);
            if (offer == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_OFFERNOTFOUND, request.ID));
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

                reportSource.Parameters.Add(new Telerik.Reporting.Parameter("OfferID", request.ID));
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
                offer.Copies++;
                await _offerRepository.UpdateOfferRecordAsync(offer);

                Stream stream = new MemoryStream(result.DocumentBytes);
                var codPdf = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                foreach (PdfPage page in codPdf.Pages)
                {
                    resultPdf.AddPage(page);
                }
            }


            string fileName = $"Offer{offer.OfferNumber.Replace("/", "-")}.pdf";

            MemoryStream resultStream = new MemoryStream();
            resultPdf.Save(resultStream, false);
            resultPdf.Close();

            resultStream.Position = 0;

            var fsr = new FileStreamResult(resultStream, $"application/pdf") { FileDownloadName = fileName };
            return fsr;
        }


    }
}
