using bbxBE.Application.Commands.cmdInvCtrl;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.Reporting.XmlSerialization;

namespace bbxBE.Application.BLL
{
    public static class bllInvCtrl
    {

        public static async Task<FileStreamResult> CreateInvCtrlReportAsynch(IInvCtrlRepositoryAsync _invCtrlRepository, string reportTRDX, PrintInvCtrlCommand request, CancellationToken cancellationToken)
        {

            var invCtrl = await _invCtrlRepository.GetInvCtrl(new Queries.qInvCtrl.GetInvCtrl { ID = request.InvCtrlPeriodID });
            if (invCtrl == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVCTRLNOTFOUND, request.InvCtrlPeriodID));
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

            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("InvCtrlPeriodID", request.InvCtrlPeriodID));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("InvPeriodTitle", request.InvPeriodTitle));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("IsInStock", request.IsInStock));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter(bbxBEConsts.JWT_REPPARAMETER, string.Format(bbxBEConsts.JWT_BEARER, request.JWT)));
            reportSource.Parameters.Add(new Telerik.Reporting.Parameter("BaseURL", request.baseURL));

            ReportProcessor reportProcessor = new ReportProcessor();

            System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();

            Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", reportSource, deviceInfo);

            if (result == null)
                throw new Exception("InvCtrl report result is null!");

            if (result.HasErrors)
                throw new Exception("Report engine has reference ERROR!");

            //Példányszám beállítása
            //
            //await _invCtrlPeriodRepository.UpdateInvCtrlPeriodAsync().UpdateOfferRecordAsync(invCtrlPeriod);

            Stream stream = new MemoryStream(result.DocumentBytes);
            string fileName = $"InvCtrlCpAbsenedReport.pdf";


            var fsr = new FileStreamResult(stream, $"application/pdf") { FileDownloadName = fileName };

            return fsr;
        }


    }
}
