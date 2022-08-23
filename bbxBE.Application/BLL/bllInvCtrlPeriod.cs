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
using bbxBE.Application.Commands.cmdInvCtrlPeriod;

namespace bbxBE.Application.BLL
{
    public static class bllInvCtrlPeriod
    {

        public static async Task<FileStreamResult> CreateInvCtrlPeriodReportAsynch(IInvCtrlPeriodRepositoryAsync _invCtrlPeriodRepository, string reportTRDX, PrintInvCtrlPeriodCommand request, CancellationToken cancellationToken)
        {

            var invCtrlPeriod = await _invCtrlPeriodRepository.GetInvCtrlPeriodRecordAsync(request.ID);
            if (invCtrlPeriod == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLPERIODNOTFOUND, request.ID));
            }


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

            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("InvCtrlPeriodID", request.ID));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("InvPeriodTitle", request.Title));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("BaseURL", request.baseURL));

            ReportProcessor reportProcessor = new ReportProcessor();

            System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();

            Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", reportSource, deviceInfo);

            if (result == null)
                throw new Exception("InvCtrlPeriod report result is null!");

            if (result.HasErrors)
                throw new Exception("Report engine has reference ERROR!");

            //Példányszám beállítása
            //
            //await _invCtrlPeriodRepository.UpdateInvCtrlPeriodAsync().UpdateOfferRecordAsync(invCtrlPeriod);

            Stream stream = new MemoryStream(result.DocumentBytes);
            string fileName = $"InvCtrliCpReport_{invCtrlPeriod.Warehouse.WarehouseDescription}.pdf";


            var fsr = new FileStreamResult(stream, $"application/pdf") { FileDownloadName = fileName };

            return fsr;
        }


    }
}
