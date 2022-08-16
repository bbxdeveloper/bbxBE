using AutoMapper.Configuration;
using bbxBE.Common.Consts;
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Common;
using bbxBE.Common.NAV;
using bbxBE.Domain.Settings;
using bxBE.Application.Commands.cmdInvoice;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;

namespace bbxBE.Application.BLL
{
    public class bllNAV
    {
        NAVSettings _NAVSettings { get; }
        private readonly ILogger _logger;

        public bllNAV(NAVSettings p_NAVSettings, ILogger p_Logger)
        {
            _NAVSettings = p_NAVSettings;
            _logger = p_Logger;

        }
        public const string DEF_procname = "importFromNAV";
        public List<InvoiceDigestType> QueryInvoiceDigest(importFromNAVCommand request)
        {

            var invoiceDigestRes = new List<InvoiceDigestType>();

            var ter = new TokenExchangeRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);

            var reqTer = XMLUtil.Object2XMLString<TokenExchangeRequest>(ter, Encoding.UTF8, NAVGlobal.XMLNamespaces);

            string response = "";
            if (bllNAV.NAVPost(_NAVSettings.TokenExchange, ter.header.requestId, reqTer, DEF_procname, out response))
            {
                TokenExchangeResponse resp = XMLUtil.XMLStringToObject<TokenExchangeResponse>(response);
                var page = 1;

                var dig = new QueryInvoiceDigestRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey,
                                page, Enum.Parse<InvoiceDirectionType>(request.InvoiceDirection), true, request.IssueDateFrom, request.IssueDateTo);

                var digTer = XMLUtil.Object2XMLString<QueryInvoiceDigestRequest>(dig, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                if (bllNAV.NAVPost(_NAVSettings.QueryInvoiceDigest, dig.header.requestId, digTer, DEF_procname, out response))
                {

                    QueryInvoiceDigestResponse respDigest = XMLUtil.XMLStringToObject<QueryInvoiceDigestResponse>(response);
                    if (respDigest.result.funcCode == FunctionCodeType.OK)
                    {
                        if (respDigest.invoiceDigestResult.availablePage > 0)
                        {
                            invoiceDigestRes.AddRange(respDigest.invoiceDigestResult.invoiceDigest.ToList());
                            var availablePage = respDigest.invoiceDigestResult.availablePage;
                            while (page++ < availablePage)
                            {
                                var digPg = new QueryInvoiceDigestRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey,
                                     page, Enum.Parse<InvoiceDirectionType>(request.InvoiceDirection), true, request.IssueDateFrom, request.IssueDateTo);

                                var digTerPg = XMLUtil.Object2XMLString<QueryInvoiceDigestRequest>(digPg, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                                if (bllNAV.NAVPost(_NAVSettings.QueryInvoiceDigest, digPg.header.requestId, digTerPg, DEF_procname, out response))
                                {
                                    QueryInvoiceDigestResponse respDigestPg = XMLUtil.XMLStringToObject<QueryInvoiceDigestResponse>(response);
                                    if (respDigestPg.result.funcCode == FunctionCodeType.OK)
                                    {
                                        invoiceDigestRes.AddRange(respDigestPg.invoiceDigestResult.invoiceDigest.ToList());
                                    }
                                }
                                else
                                {
                                    throw new Exception(String.Format(bbxBEConsts.NAV_QINVDIGEST_NEXTPG_ERR, DEF_procname, response));
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(String.Format(bbxBEConsts.NAV_QINVDIGEST_FIRSTPG_ERR, DEF_procname, response));
                    }

                    string msg = String.Format(bbxBEConsts.NAV_QINVDIGEST_OK, DEF_procname,
                        request.InvoiceDirection, true, request.IssueDateFrom, request.IssueDateTo);
                    Console.WriteLine(msg);
                    _logger.LogInformation(msg);
                }
            }
            else
            {
                throw new Exception(String.Format(bbxBEConsts.NAV_TOKENEXCHANGE_ERR, DEF_procname, response));

            }

            return invoiceDigestRes;
        }

        public InvoiceData QueryInvoiceData(string p_invoiceNumber, InvoiceDirectionType p_invoiceDirection)
        {
            InvoiceData result = null;

            var ter = new TokenExchangeRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);
            var reqTer = XMLUtil.Object2XMLString<TokenExchangeRequest>(ter, Encoding.UTF8, NAVGlobal.XMLNamespaces);
            string response = "";
            if (bllNAV.NAVPost(_NAVSettings.TokenExchange, ter.header.requestId, reqTer, DEF_procname, out response))
            {
                TokenExchangeResponse resp = XMLUtil.XMLStringToObject<TokenExchangeResponse>(response);

                var invData = new QueryInvoiceDataRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey,
                                p_invoiceNumber, p_invoiceDirection);

                var invDataReq = XMLUtil.Object2XMLString<QueryInvoiceDataRequest>(invData, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                if (bllNAV.NAVPost(_NAVSettings.QueryInvoiceData, invData.header.requestId, invDataReq, DEF_procname, out response))
                {

                    QueryInvoiceDataResponse respInvData = XMLUtil.XMLStringToObject<QueryInvoiceDataResponse>(response);
                    if (respInvData.result.funcCode == FunctionCodeType.OK)
                    {
                        var respInvoiceData = XMLUtil.XMLStringToObject<QueryInvoiceDataResponse>(response);
                        if (respInvoiceData.invoiceDataResult != null && respInvoiceData.invoiceDataResult != null)
                        {
                            string InvoiceDataStr = "";
                            if (respInvoiceData.invoiceDataResult.compressedContentIndicator)
                            {
                                //TODO: Tömörített számlám még nincs, tesztelni !!! a megoldás elvileg jó.
                                InvoiceDataStr = Utils.Unzip(respInvoiceData.invoiceDataResult.invoiceData);
                                throw new NotImplementedException("Tömörített számlát tesztelni !!! (compressedContentIndicator)");
                            }
                            else
                            {
                                InvoiceDataStr = Encoding.UTF8.GetString(respInvoiceData.invoiceDataResult.invoiceData);
                            }
                            result = XMLUtil.XMLStringToObject<InvoiceData>(InvoiceDataStr);
                        }
                        else
                        {
                            throw new Exception(String.Format(bbxBEConsts.NAV_QINVDATA_NOTFND_ERR, DEF_procname, p_invoiceNumber));

                        }

                    }
                    else
                    {
                        throw new Exception(String.Format(bbxBEConsts.NAV_QINVDATA_ERR, DEF_procname, response));
                    }

                    var msg = String.Format(bbxBEConsts.NAV_QINVDATA_OK, DEF_procname, resp.result.funcCode,
                                   (resp.result.errorCode != null ? resp.result.errorCode : ""), (resp.result.message != null ? resp.result.message : ""));
                    Console.WriteLine(msg);
                    _logger.LogInformation(msg);
                }
            }

            return result;
        }


        public static bool NAVPost(string p_uri, string p_requestId, string p_content, string p_procname, out string o_response)
        {
            o_response = "";
            try
            {

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                p_content = p_content.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
                p_content = p_content.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

                //Util.Log2File(String.Format("{0} NAV POST. requestId:{1}, uri:{2}, Content:\n{3} ", p_procname, p_requestId, p_uri, p_content), Global.POSTLOG_NAME);

                var request = (HttpWebRequest)WebRequest.Create(p_uri);
                request.Method = "POST";
                request.ContentType = "application/xml; charset=utf-8";
                request.ContentType = "application/xml";
                byte[] postBytes = Encoding.UTF8.GetBytes(p_content);
                request.ContentLength = postBytes.Length;
                var requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                return GetResponse(request, p_requestId, p_content, p_procname, out o_response);


            }
            catch (Exception ex)
            {
                //Util.Log2File(String.Format("{0} NAV POST exception. p_requestId:{1}, uri:{2}, Content:\n{3}", p_procname, p_requestId, p_uri, p_content), Global.POSTLOG_NAME);
                //Util.ExceptionLog(ex);
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }


        }

        private static bool GetResponse(WebRequest request, string p_requestId, string p_content, string p_procname, out string o_response)
        {
            o_response = "";
            try
            {

                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    throw;
              //      response = (HttpWebResponse)ex.Response;
                }

                if (response != null)
                {

                    //Response kiolvasása
                    long length = response.ContentLength;
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    o_response = reader.ReadToEnd();


                    if (response.StatusCode.ToString() == NAVGlobal.NAV_OK)
                    {
                        //Util.Log2File(String.Format("{0} NAV OK response. requestId:{1}, status:{2}, response length:{3}, response:{4}", p_procname, p_requestId, response.StatusDescription, response.ContentLength, o_response), Global.POSTLOG_NAME);
                        Console.WriteLine(String.Format("{0} NAV OK response. requestId:{1}, status:{2}, response length:{3}, response:{4}", p_procname, p_requestId, response.StatusDescription, response.ContentLength, o_response));
                        return true;
                    }
                    else
                    {
                        // Util.Log2File(String.Format("{0} NAV error response. requestId:{1}, status:{2}, response:{3}", p_procname, p_requestId, response.StatusDescription, o_response), Global.POSTLOG_NAME);
                        //                        Console.WriteLine(String.Format("{0} NAV error response. requestId:{1}, status:{2}, response:{3}", p_procname, p_requestId, response.StatusDescription, o_response));
                        throw new Exception(String.Format("{ 0} NAV error response. requestId:{1}, status:{2}, response:{3}", p_procname, p_requestId, response.StatusDescription, o_response));
                        //      return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //Util.Log2File(String.Format("{0} NAV GetResponse exception. requestId:{1}", p_procname, p_requestId), Global.POSTLOG_NAME);
                Console.WriteLine(String.Format("{0} NAV GetResponse exception. requestId:{1}", p_procname, p_requestId));
                //Util.ExceptionLog(we);
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }

        }

        public TaxpayerDataType QueryTaxPayer(QueryTaxPayer request)
        {

            var invoiceDigestRes = new List<InvoiceDigestType>();

            var ter = new TokenExchangeRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);

            var reqTer = XMLUtil.Object2XMLString<TokenExchangeRequest>(ter, Encoding.UTF8, NAVGlobal.XMLNamespaces);
            TaxpayerDataType result = null;
            string response = "";
            string msg = "";
            if (bllNAV.NAVPost(_NAVSettings.TokenExchange, ter.header.requestId, reqTer, DEF_procname, out response))
            {

                TokenExchangeResponse resp = XMLUtil.XMLStringToObject<TokenExchangeResponse>(response);

                var qtp = new QueryTaxpayerRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey, request.Taxnumber);

                var qtpTer = XMLUtil.Object2XMLString<QueryTaxpayerRequest>(qtp, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                if (bllNAV.NAVPost(_NAVSettings.QueryTaxPayer, qtp.header.requestId, qtpTer, DEF_procname, out response))
                {

                    QueryTaxpayerResponse respQtp = XMLUtil.XMLStringToObject<QueryTaxpayerResponse>(response);
                    if (respQtp.taxpayerData != null)
                    {
                        result = respQtp.taxpayerData;
                        msg = String.Format(bbxBEConsts.NAV_QTAXPAYERT_OK, DEF_procname, request.Taxnumber);
                        Console.WriteLine(msg);
                    }
                    else
                    {
                        msg  = String.Format(bbxBEConsts.NAV_QTAXPAYER_ERR, DEF_procname, request.Taxnumber, response);
                    }
                    
               }
            }
            else
            {
                msg = String.Format(bbxBEConsts.NAV_QTAXPAYER_TOKEN_ERR, DEF_procname, request.Taxnumber, response);
            }
            _logger.LogInformation(msg);
            return result;
        }
    }
}
