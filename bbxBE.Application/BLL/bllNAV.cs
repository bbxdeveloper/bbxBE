using bbxBE.Application.Commands.cmdNAV;
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Common;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace bbxBE.Application.BLL
{
    public class bllNAV
    {
        NAVSettings _NAVSettings { get; }
        private readonly ILogger _logger;

        public bllNAV(NAVSettings p_NAVSettings, ILoggerFactory loggerFactory)
        {
            _NAVSettings = p_NAVSettings;
            _logger = loggerFactory.CreateLogger("NAV");

        }

        public bool NAVPost(string p_uri, string p_requestId, string p_content, string p_procname, out string o_response)
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
                //     request.ContentType = "application/xml";
                byte[] postBytes = Encoding.UTF8.GetBytes(p_content);
                request.ContentLength = postBytes.Length;
                var requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                return GetNAVResponse(request, p_requestId, p_content, p_procname, out o_response);


            }
            catch (Exception)
            {
                //Util.Log2File(String.Format("{0} NAV POST exception. p_requestId:{1}, uri:{2}, Content:\n{3}", p_procname, p_requestId, p_uri, p_content), Global.POSTLOG_NAME);
                //Util.ExceptionLog(ex);
                //ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }


        }

        private bool GetNAVResponse(WebRequest request, string p_requestId, string p_content, string p_procname, out string o_response)
        {
            o_response = "";
            try
            {

                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException)
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
                        throw new NAVException(String.Format("{ 0} NAV error response. requestId:{1}, status:{2}, response:{3}", p_procname, p_requestId, response.StatusDescription, o_response));
                        //      return false;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                //Util.Log2File(String.Format("{0} NAV GetResponse exception. requestId:{1}", p_procname, p_requestId), Global.POSTLOG_NAME);
                Console.WriteLine(String.Format("{0} NAV GetResponse exception. requestId:{1}", p_procname, p_requestId));
                //Util.ExceptionLog(we);
                //ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }

        }

        public List<InvoiceDigestType> QueryInvoiceDigest(importFromNAVCommand request)
        {

            var invoiceDigestRes = new List<InvoiceDigestType>();

            var ter = new TokenExchangeRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);

            var reqTer = XMLUtil.Object2XMLString<TokenExchangeRequest>(ter, Encoding.UTF8, NAVGlobal.XMLNamespaces);

            string response = "";
            if (NAVPost(_NAVSettings.TokenExchange, ter.header.requestId, reqTer, MethodBase.GetCurrentMethod().Name, out response))
            {
                TokenExchangeResponse resp = XMLUtil.XMLStringToObject<TokenExchangeResponse>(response);
                var page = 1;

                var dig = new QueryInvoiceDigestRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey,
                                page, Enum.Parse<InvoiceDirectionType>(request.InvoiceDirection), true, request.IssueDateFrom, request.IssueDateTo);

                var digTer = XMLUtil.Object2XMLString<QueryInvoiceDigestRequest>(dig, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                if (NAVPost(_NAVSettings.QueryInvoiceDigest, dig.header.requestId, digTer, MethodBase.GetCurrentMethod().Name, out response))
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

                                if (NAVPost(_NAVSettings.QueryInvoiceDigest, digPg.header.requestId, digTerPg, MethodBase.GetCurrentMethod().Name, out response))
                                {
                                    QueryInvoiceDigestResponse respDigestPg = XMLUtil.XMLStringToObject<QueryInvoiceDigestResponse>(response);
                                    if (respDigestPg.result.funcCode == FunctionCodeType.OK)
                                    {
                                        invoiceDigestRes.AddRange(respDigestPg.invoiceDigestResult.invoiceDigest.ToList());
                                    }
                                }
                                else
                                {
                                    var errmsg = String.Format(bbxBEConsts.NAV_QINVDIGEST_NEXTPG_ERR, MethodBase.GetCurrentMethod().Name, response);
                                    _logger.LogError(errmsg);
                                    throw new NAVException(errmsg);
                                }
                            }
                        }
                    }
                    else
                    {
                        var errmsg = String.Format(bbxBEConsts.NAV_QINVDIGEST_FIRSTPG_ERR, MethodBase.GetCurrentMethod().Name, response);
                        _logger.LogError(errmsg);
                        throw new NAVException(errmsg);
                    }

                    string msg = String.Format(bbxBEConsts.NAV_QINVDIGEST_OK, MethodBase.GetCurrentMethod().Name,
                        request.InvoiceDirection, true, request.IssueDateFrom, request.IssueDateTo);
                    Console.WriteLine(msg);
                    _logger.LogInformation(msg);
                }
            }
            else
            {
                var msg = String.Format(bbxBEConsts.NAV_TOKENEXCHANGE_ERR, MethodBase.GetCurrentMethod().Name, response);
                _logger.LogError(msg);
                throw new NAVException(msg);
            }

            return invoiceDigestRes;
        }

        public InvoiceData QueryInvoiceData(string p_invoiceNumber, InvoiceDirectionType p_invoiceDirection)
        {
            InvoiceData result = null;

            var ter = new TokenExchangeRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);
            var reqTer = XMLUtil.Object2XMLString<TokenExchangeRequest>(ter, Encoding.UTF8, NAVGlobal.XMLNamespaces);
            string response = "";
            if (NAVPost(_NAVSettings.TokenExchange, ter.header.requestId, reqTer, MethodBase.GetCurrentMethod().Name, out response))
            {
                TokenExchangeResponse resp = XMLUtil.XMLStringToObject<TokenExchangeResponse>(response);

                var invData = new QueryInvoiceDataRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey,
                                p_invoiceNumber, p_invoiceDirection);

                var invDataReq = XMLUtil.Object2XMLString<QueryInvoiceDataRequest>(invData, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                if (NAVPost(_NAVSettings.QueryInvoiceData, invData.header.requestId, invDataReq, MethodBase.GetCurrentMethod().Name, out response))
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
                            var errmsg = String.Format(bbxBEConsts.NAV_QINVDATA_NOTFND_ERR, MethodBase.GetCurrentMethod().Name, p_invoiceNumber);
                            _logger.LogError(errmsg);
                            throw new NAVException(errmsg);
                        }

                    }
                    else
                    {
                        var errmsg = String.Format(bbxBEConsts.NAV_QINVDATA_ERR, MethodBase.GetCurrentMethod().Name, response);
                        _logger.LogError(errmsg);
                        throw new NAVException(errmsg);
                    }

                    var msg = String.Format(bbxBEConsts.NAV_QINVDATA_OK, MethodBase.GetCurrentMethod().Name, resp.result.funcCode,
                                   (resp.result.errorCode != null ? resp.result.errorCode : ""), (resp.result.message != null ? resp.result.message : ""));
                    Console.WriteLine(msg);
                    _logger.LogInformation(msg);
                }
            }
            else
            {
                var msg = String.Format(bbxBEConsts.NAV_TOKENEXCHANGE_ERR, MethodBase.GetCurrentMethod().Name, response);
                _logger.LogError(msg);
                throw new NAVException(msg);
            }

            return result;
        }


        public TaxpayerDataType QueryTaxPayer(QueryTaxPayer request)
        {

            var invoiceDigestRes = new List<InvoiceDigestType>();

            var ter = new TokenExchangeRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);

            var reqTer = XMLUtil.Object2XMLString<TokenExchangeRequest>(ter, Encoding.UTF8, NAVGlobal.XMLNamespaces);
            TaxpayerDataType result = null;
            string response = "";
            string msg = "";
            if (NAVPost(_NAVSettings.TokenExchange, ter.header.requestId, reqTer, MethodBase.GetCurrentMethod().Name, out response))
            {

                TokenExchangeResponse resp = XMLUtil.XMLStringToObject<TokenExchangeResponse>(response);

                var qtp = new QueryTaxpayerRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey, request.Taxnumber);

                var qtpTer = XMLUtil.Object2XMLString<QueryTaxpayerRequest>(qtp, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                if (NAVPost(_NAVSettings.QueryTaxPayer, qtp.header.requestId, qtpTer, MethodBase.GetCurrentMethod().Name, out response))
                {

                    QueryTaxpayerResponse respQtp = XMLUtil.XMLStringToObject<QueryTaxpayerResponse>(response);
                    if (respQtp.taxpayerData != null)
                    {
                        result = respQtp.taxpayerData;
                        msg = String.Format(bbxBEConsts.NAV_QTAXPAYERT_OK, MethodBase.GetCurrentMethod().Name, request.Taxnumber);
                        Console.WriteLine(msg);
                        _logger.LogInformation(msg);
                    }
                    else
                    {
                        // egyelőre csak logoljuk a hibát
                        msg = String.Format(bbxBEConsts.NAV_QTAXPAYER_ERR, MethodBase.GetCurrentMethod().Name, request.Taxnumber, response);
                        _logger.LogError(msg);
                    }

                }
            }
            else
            {
                // egyelőre csak logoljuk a hibát
                msg = String.Format(bbxBEConsts.NAV_QTAXPAYER_TOKEN_ERR, MethodBase.GetCurrentMethod().Name, request.Taxnumber, response);
                _logger.LogError(msg);
            }
            return result;
        }

        public static void ProcessGeneralSendErrorResponse(NAVXChange xChangeResult, string generalErrResp)
        {

            xChangeResult.Status = enNAVStatus.ERROR.ToString();
            xChangeResult.SendResponse = generalErrResp;
            // Általános hibák nem blokkolják a feldolgozást, 
            // ezért a Status nincs ERROR-ra állítva. A program a következő menetben 
            // újra probálja a beküldést
            if (generalErrResp.Contains(bbxBEConsts.DEF_NAVGeneralExceptionResponse))
            {
                GeneralExceptionResponse gex = XMLUtil.XMLStringToObject<GeneralExceptionResponse>(generalErrResp);

                //                                p_XChange.Status = Global.NAV_STATUS_ERROR;
                xChangeResult.SendResponse = gex.funcCode.ToString();
                xChangeResult.SendMessage = (gex.errorCode + " " + gex.message).Trim();
            }

            if (generalErrResp.Contains(bbxBEConsts.DEF_NAVGeneralErrorResponse))
            {
                GeneralErrorResponseType ger = XMLUtil.XMLStringToObject<GeneralErrorResponseType>(generalErrResp);
                xChangeResult.SendResponse = ger.result.funcCode.ToString();
                xChangeResult.SendMessage = (ger.result.errorCode + " " + ger.result.message).Trim();

                //                                p_XChange.Status = Global.NAV_STATUS_ERROR;
                if (ger.technicalValidationMessages != null)
                {
                    xChangeResult.NAVXResults = new List<NAVXResult>();

                    foreach (var err in ger.technicalValidationMessages)
                    {
                        var xres = new NAVXResult()
                        {
                            ResultCode = err.validationResultCode.ToString(),
                            ErrorCode = err.validationErrorCode,
                            Message = err.message,
                        };
                        xChangeResult.NAVXResults.Add(xres);
                    }
                }
            }
        }


        public NAVXChange SendManageInvoice(Invoice invoice)
        {
            var resNAVXChange = new NAVXChange();

            var invoiceNAVXML = bllInvoice.GetInvoiceNAVXML(invoice);
            resNAVXChange.InvoiceID = invoice.ID;
            resNAVXChange.InvoiceNumber = invoice.InvoiceNumber;
            resNAVXChange.InvoiceXml = XMLUtil.Object2XMLString<InvoiceData>(invoiceNAVXML, Encoding.UTF8, NAVGlobal.XMLNamespaces);
            resNAVXChange.Operation = enNAVOperation.MANAGEINVOICE.ToString();

            var ter = new TokenExchangeRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);

            var reqTer = XMLUtil.Object2XMLString<TokenExchangeRequest>(ter, Encoding.UTF8, NAVGlobal.XMLNamespaces);
            resNAVXChange.TokenTime = DateTime.UtcNow;
            resNAVXChange.TokenRequest = reqTer;
            resNAVXChange.Status = enNAVStatus.CREATED.ToString();
            string resp = "";
            if (NAVPost(_NAVSettings.TokenExchange, ter.header.requestId, reqTer, MethodBase.GetCurrentMethod().Name, out resp))
            {
                TokenExchangeResponse tokenResponse = XMLUtil.XMLStringToObject<TokenExchangeResponse>(resp);

                var token = tokenResponse.encodedExchangeToken;
                resNAVXChange.TokenResponse = resp;
                resNAVXChange.Token = Convert.ToBase64String(token);
                resNAVXChange.TokenFuncCode = tokenResponse.result.funcCode.ToString();
                resNAVXChange.TokenMessage = (tokenResponse.result.errorCode + " " + tokenResponse.result.message).Trim();
                resNAVXChange.Status = enNAVStatus.TOKEN_SENT.ToString();

                var mar = new ManageInvoiceRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey, _NAVSettings.ExchangeKey,
                    token, new string[] { resNAVXChange.InvoiceXml });
                var reqManageInvoice = XMLUtil.Object2XMLString<ManageInvoiceRequest>(mar, Encoding.UTF8, NAVGlobal.XMLNamespaces);
                resNAVXChange.SendTime = DateTime.UtcNow;
                resNAVXChange.SendRequest = reqManageInvoice;
                resp = "";
                if (NAVPost(_NAVSettings.ManageInvoice, mar.header.requestId, reqManageInvoice, MethodBase.GetCurrentMethod().Name, out resp))
                {
                    ManageInvoiceResponse miresp = XMLUtil.XMLStringToObject<ManageInvoiceResponse>(resp);

                    resNAVXChange.Status = enNAVStatus.DATA_SENT.ToString();
                    resNAVXChange.SendResponse = resp;
                    resNAVXChange.SendFuncCode = miresp.result.funcCode.ToString();
                    resNAVXChange.SendMessage = (miresp.result.errorCode + " " + miresp.result.message).Trim();
                    resNAVXChange.TransactionID = miresp.transactionId;
                }
                else
                {
                    var msg = String.Format(bbxBEConsts.NAV_MANAGEINVOICE_ERR, MethodBase.GetCurrentMethod().Name, resp);
                    _logger.LogError(msg);
                    ProcessGeneralSendErrorResponse(resNAVXChange, resp);
                }
            }
            else
            {
                var msg = String.Format(bbxBEConsts.NAV_TOKENEXCHANGE_ERR, MethodBase.GetCurrentMethod().Name, resp);
                _logger.LogError(msg);
            }
            return resNAVXChange;
        }

    }
}
