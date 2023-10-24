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

        public bllNAV(NAVSettings p_NAVSettings, ILogger p_Logger)
        {
            _NAVSettings = p_NAVSettings;
            _logger = p_Logger;

        }
        public List<InvoiceDigestType> QueryInvoiceDigest(importFromNAVCommand request)
        {

            var invoiceDigestRes = new List<InvoiceDigestType>();

            var ter = new TokenExchangeRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);

            var reqTer = XMLUtil.Object2XMLString<TokenExchangeRequest>(ter, Encoding.UTF8, NAVGlobal.XMLNamespaces);

            string response = "";
            if (bllNAV.NAVPost(_NAVSettings.TokenExchange, ter.header.requestId, reqTer, MethodBase.GetCurrentMethod().Name, out response))
            {
                TokenExchangeResponse resp = XMLUtil.XMLStringToObject<TokenExchangeResponse>(response);
                var page = 1;

                var dig = new QueryInvoiceDigestRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey,
                                page, Enum.Parse<InvoiceDirectionType>(request.InvoiceDirection), true, request.IssueDateFrom, request.IssueDateTo);

                var digTer = XMLUtil.Object2XMLString<QueryInvoiceDigestRequest>(dig, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                if (bllNAV.NAVPost(_NAVSettings.QueryInvoiceDigest, dig.header.requestId, digTer, MethodBase.GetCurrentMethod().Name, out response))
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

                                if (bllNAV.NAVPost(_NAVSettings.QueryInvoiceDigest, digPg.header.requestId, digTerPg, MethodBase.GetCurrentMethod().Name, out response))
                                {
                                    QueryInvoiceDigestResponse respDigestPg = XMLUtil.XMLStringToObject<QueryInvoiceDigestResponse>(response);
                                    if (respDigestPg.result.funcCode == FunctionCodeType.OK)
                                    {
                                        invoiceDigestRes.AddRange(respDigestPg.invoiceDigestResult.invoiceDigest.ToList());
                                    }
                                }
                                else
                                {
                                    throw new NAVException(String.Format(bbxBEConsts.NAV_QINVDIGEST_NEXTPG_ERR, MethodBase.GetCurrentMethod().Name, response));
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new NAVException(String.Format(bbxBEConsts.NAV_QINVDIGEST_FIRSTPG_ERR, MethodBase.GetCurrentMethod().Name, response));
                    }

                    string msg = String.Format(bbxBEConsts.NAV_QINVDIGEST_OK, MethodBase.GetCurrentMethod().Name,
                        request.InvoiceDirection, true, request.IssueDateFrom, request.IssueDateTo);
                    Console.WriteLine(msg);
                    _logger.LogInformation(msg);
                }
            }
            else
            {
                throw new NAVException(String.Format(bbxBEConsts.NAV_TOKENEXCHANGE_ERR, MethodBase.GetCurrentMethod().Name, response));

            }

            return invoiceDigestRes;
        }

        public InvoiceData QueryInvoiceData(string p_invoiceNumber, InvoiceDirectionType p_invoiceDirection)
        {
            InvoiceData result = null;

            var ter = new TokenExchangeRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);
            var reqTer = XMLUtil.Object2XMLString<TokenExchangeRequest>(ter, Encoding.UTF8, NAVGlobal.XMLNamespaces);
            string response = "";
            if (bllNAV.NAVPost(_NAVSettings.TokenExchange, ter.header.requestId, reqTer, MethodBase.GetCurrentMethod().Name, out response))
            {
                TokenExchangeResponse resp = XMLUtil.XMLStringToObject<TokenExchangeResponse>(response);

                var invData = new QueryInvoiceDataRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey,
                                p_invoiceNumber, p_invoiceDirection);

                var invDataReq = XMLUtil.Object2XMLString<QueryInvoiceDataRequest>(invData, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                if (bllNAV.NAVPost(_NAVSettings.QueryInvoiceData, invData.header.requestId, invDataReq, MethodBase.GetCurrentMethod().Name, out response))
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
                            throw new NAVException(String.Format(bbxBEConsts.NAV_QINVDATA_NOTFND_ERR, MethodBase.GetCurrentMethod().Name, p_invoiceNumber));

                        }

                    }
                    else
                    {
                        throw new NAVException(String.Format(bbxBEConsts.NAV_QINVDATA_ERR, MethodBase.GetCurrentMethod().Name, response));
                    }

                    var msg = String.Format(bbxBEConsts.NAV_QINVDATA_OK, MethodBase.GetCurrentMethod().Name, resp.result.funcCode,
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
                //     request.ContentType = "application/xml";
                byte[] postBytes = Encoding.UTF8.GetBytes(p_content);
                request.ContentLength = postBytes.Length;
                var requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                return GetResponse(request, p_requestId, p_content, p_procname, out o_response);


            }
            catch (Exception)
            {
                //Util.Log2File(String.Format("{0} NAV POST exception. p_requestId:{1}, uri:{2}, Content:\n{3}", p_procname, p_requestId, p_uri, p_content), Global.POSTLOG_NAME);
                //Util.ExceptionLog(ex);
                //ExceptionDispatchInfo.Capture(ex).Throw();
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

        public TaxpayerDataType QueryTaxPayer(QueryTaxPayer request)
        {

            var invoiceDigestRes = new List<InvoiceDigestType>();

            var ter = new TokenExchangeRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);

            var reqTer = XMLUtil.Object2XMLString<TokenExchangeRequest>(ter, Encoding.UTF8, NAVGlobal.XMLNamespaces);
            TaxpayerDataType result = null;
            string response = "";
            string msg = "";
            if (bllNAV.NAVPost(_NAVSettings.TokenExchange, ter.header.requestId, reqTer, MethodBase.GetCurrentMethod().Name, out response))
            {

                TokenExchangeResponse resp = XMLUtil.XMLStringToObject<TokenExchangeResponse>(response);

                var qtp = new QueryTaxpayerRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey, request.Taxnumber);

                var qtpTer = XMLUtil.Object2XMLString<QueryTaxpayerRequest>(qtp, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                if (bllNAV.NAVPost(_NAVSettings.QueryTaxPayer, qtp.header.requestId, qtpTer, MethodBase.GetCurrentMethod().Name, out response))
                {

                    QueryTaxpayerResponse respQtp = XMLUtil.XMLStringToObject<QueryTaxpayerResponse>(response);
                    if (respQtp.taxpayerData != null)
                    {
                        result = respQtp.taxpayerData;
                        msg = String.Format(bbxBEConsts.NAV_QTAXPAYERT_OK, MethodBase.GetCurrentMethod().Name, request.Taxnumber);
                        Console.WriteLine(msg);
                    }
                    else
                    {
                        msg = String.Format(bbxBEConsts.NAV_QTAXPAYER_ERR, MethodBase.GetCurrentMethod().Name, request.Taxnumber, response);
                    }

                }
            }
            else
            {
                msg = String.Format(bbxBEConsts.NAV_QTAXPAYER_TOKEN_ERR, MethodBase.GetCurrentMethod().Name, request.Taxnumber, response);
            }
            _logger.LogInformation(msg);
            return result;
        }

        /*
        public static void ProcessSendInvoiceErrorResponse(XChange p_XChange, string p_response, NAVSqliteAccess p_SQLiteDB)
        {
            p_XChange.InvResponse = p_response;

            // Általános hibák nem blokkolják a feldolgozást, 
            // ezért a Status nincs ERROR-ra állítva. A program a következő menetben 
            // újra probálja a beküldést

            if (p_response.Contains(Global.DEF_GeneralExceptionResponse))
            {
                GeneralExceptionResponse gex = XMLUtil.XMLStringToObject<GeneralExceptionResponse>(p_response);

                //                                p_XChange.Status = Global.NAV_STATUS_ERROR;
                p_XChange.InvFuncCode = gex.funcCode.ToString();
                p_XChange.InvMessage = (gex.errorCode + " " + gex.message).Trim();
            }

            if (p_response.Contains(Global.DEF_GeneralErrorResponse))
            {
                GeneralErrorResponseType ger = XMLUtil.XMLStringToObject<GeneralErrorResponseType>(p_response);

                //                                p_XChange.Status = Global.NAV_STATUS_ERROR;
                p_XChange.InvFuncCode = ger.result.funcCode.ToString();
                p_XChange.InvMessage = (ger.result.errorCode + " " + ger.result.message).Trim();
                if (ger.technicalValidationMessages != null && p_SQLiteDB != null)
                {
                    foreach (var err in ger.technicalValidationMessages)
                    {
                        XResult xres = new XResult()
                        {
                            XChangeID = p_XChange.RowID,
                            ErrorCode = err.validationErrorCode,
                            ResultCode = err.validationResultCode.ToString(),
                            Message = err.message
                        };

                        p_SQLiteDB.InsertObjEx(xres);
                    }
                }
            }
        }

        */
        public NAVXChange SendManageInvoice(string p_InvoiceXML)
        {
            var result = new NAVXChange();
            var ter = new TokenExchangeRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);

            var reqTer = XMLUtil.Object2XMLString<TokenExchangeRequest>(ter, Encoding.UTF8, NAVGlobal.XMLNamespaces);
            result.TokenTime = DateTime.UtcNow;
            result.TokenRequest = reqTer;
            result.Status = enNAVStatus.CREATED.ToString();
            string resp = "";
            if (bllNAV.NAVPost(_NAVSettings.TokenExchange, ter.header.requestId, reqTer, MethodBase.GetCurrentMethod().Name, out resp))
            {
                TokenExchangeResponse tokenResponse = XMLUtil.XMLStringToObject<TokenExchangeResponse>(resp);

                var token = tokenResponse.encodedExchangeToken;
                result.TokenResponse = resp;
                result.Token = Convert.ToBase64String(token);
                result.TokenFuncCode = tokenResponse.result.funcCode.ToString();
                result.TokenMessage = (tokenResponse.result.errorCode + " " + tokenResponse.result.message).Trim();
                result.Status = enNAVStatus.TOKEN_SENT.ToString();

                var mar = new ManageInvoiceRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey, _NAVSettings.ExchangeKey,
                    token, new string[] { p_InvoiceXML });
                var reqManageInvoice = XMLUtil.Object2XMLString<ManageInvoiceRequest>(mar, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                resp = "";
                if (bllNAV.NAVPost(_NAVSettings.ManageInvoice, ter.header.requestId, reqTer, MethodBase.GetCurrentMethod().Name, out resp))
                {
                    TokenExchangeResponse tokenResponse = XMLUtil.XMLStringToObject<TokenExchangeResponse>(resp);


                    result.Status = enNAVStatus.DATA_SENT.ToString();
                    result.SendResponse = resp;
                    result.SendFuncCode = resp.result.funcCode.ToString();
                    p_XChange.InvMessage = (miresp.result.errorCode + " " + miresp.result.message).Trim();
                    p_XChange.TransactionID = miresp.transactionId;

                    bResult = (miresp.result.funcCode == FunctionCodeType.OK);


                }
                else
                {
                    result.TokenResponse = resp;
                    result.TokenFuncCode = FunctionCodeType.ERROR.ToString();
                    result.TokenMessage = String.Format(bbxBEConsts.NAV_TOKENEXCHANGE_ERR, MethodBase.GetCurrentMethod().Name, resp);
                    throw new NAVException(result.TokenMessage);
                }
                ///////////////////

                /*
                try
                {
                    //1.Token kikérése
                    string o_request = "";
                    string o_response = "";
                    string tokenUri = (p_test ? NAVGlobal.NAV_TOKENEXCHANGE_TEST : NAVGlobal.NAV_TOKENEXCHANGE);

                    p_XChange.TokenTime = DateTime.Now.ToString(Global.DATETIMEFORMAT);
                    p_XChange.InvoiceXml = p_InvoiceData;

                    if (p_SQLiteDB != null)
                        p_SQLiteDB.updateObjEx(p_XChange);

                    var tokenResp = XNAVGetToken.GetToken(tokenUri, p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey, p_procName, out o_request, out o_response);
                    if (tokenResp != null)
                    {

                        var token = tokenResp.encodedExchangeToken;
                        if (token.Length > 0)
                        {
                            // tokenkérés eredményének beírása a DB-be 
                            p_XChange.TokenRequest = o_request;
                            p_XChange.TokenResponse = o_response;
                            p_XChange.Token = Convert.ToBase64String(token);
                            p_XChange.TokenFuncCode = tokenResp.result.funcCode.ToString();
                            p_XChange.TokenMessage = (tokenResp.result.errorCode + " " + tokenResp.result.message).Trim();

                            if (p_SQLiteDB != null)
                                p_SQLiteDB.updateObjEx(p_XChange);


                            //2. Manageinvoice
                            var mar = new ManageInvoiceRequest(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey, p_XMLXchangeKey, token, new string[] { p_InvoiceData });
                            var marStr = XMLUtil.Object2XMLString<ManageInvoiceRequest>(mar, Encoding.UTF8, NAVGlobal.XMLNamespaces);

                            string manageInvoiceUri = (p_test ? NAVGlobal.NAV_MANAGEINVOICE_TEST : NAVGlobal.NAV_MANAGEINVOICE);

                            // DB !
                            p_XChange.InvTime = DateTime.Now.ToString(Global.DATETIMEFORMAT);
                            p_XChange.InvRequest = marStr;

                            if (p_SQLiteDB != null)
                                p_SQLiteDB.updateObjEx(p_XChange);

                            var manageInvoieSucceed = (XNAV.Post(manageInvoiceUri, mar.RequestId, marStr, p_procName, out o_response));
                            if (manageInvoieSucceed)
                            {

                                p_XChange.Status = NAVGlobal.NAV_STATUS_SENT;
                                p_XChange.InvResponse = o_response;
                                ManageInvoiceResponse miresp = XMLUtil.XMLStringToObject<ManageInvoiceResponse>(o_response);

                                p_XChange.InvFuncCode = miresp.result.funcCode.ToString();
                                p_XChange.InvMessage = (miresp.result.errorCode + " " + miresp.result.message).Trim();
                                p_XChange.TransactionID = miresp.transactionId;

                                bResult = (miresp.result.funcCode == FunctionCodeType.OK);
                            }
                            else
                            {
                                XNAV.ProcessGeneralErrorResponse(p_XChange, o_response, p_SQLiteDB);
                            }
                            if (p_SQLiteDB != null)
                                p_SQLiteDB.updateObjEx(p_XChange);
                        }
                        else
                        {
                            // Token hiba nem blokkolja a feldolgozást
                            //      p_XChange.Status = Global.NAV_STATUS_ERROR;
                            p_XChange.TokenFuncCode = Global.DEF_EMPTYTOKEN;
                            p_XChange.TokenMessage = String.Format("{0} Empty token! p_uri:{1}, p_taxnum:{2}, p_techUserLogin:{3}, p_techUserPwd:{4}, p_XMLSignKey:{5}, p_XMLXchangeKey:{6}",
                                        p_procName, tokenUri, p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey, p_XMLXchangeKey);

                            if (p_SQLiteDB != null)
                                p_SQLiteDB.updateObjEx(p_XChange);

                            throw new Exception(p_XChange.TokenMessage);
                        }
                    }
                    else
                    {
                        // Token hiba nem blokkolja a feldolgozást
                        //    p_XChange.Status = Global.NAV_STATUS_ERROR;
                        p_XChange.TokenFuncCode = Global.DEF_NULLTOKEN;
                        p_XChange.TokenMessage = String.Format("{0} GetToken returned null! p_uri:{1}, p_taxnum:{2}, p_techUserLogin:{3}, p_techUserPwd:{4}, p_XMLSignKey:{5}, p_XMLXchangeKey:{6}",
                                    p_procName, tokenUri, p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey, p_XMLXchangeKey);

                        if (p_SQLiteDB != null)
                            p_SQLiteDB.updateObjEx(p_XChange);

                        throw new Exception(p_XChange.TokenMessage);
                    }

                    return bResult;
                }
                catch (Exception ex)
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
                return false;
            }

                */
            }

        }
    }
