using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common;
using bbxBE.Common.Attributes;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdInvoice
{
    public class importFromNAVCommand : IRequest<Response<long>>
    {
        public string InvoiceDirection { get; set; }    //InvoiceDirectionType 
        public DateTime IssueDateFrom { get; set; }
        public DateTime IssueDateTo { get; set; }

    }

    public class getIncomingInvoicesNAVCommandHandler : IRequestHandler<importFromNAVCommand, Response<long>>
    {
        private readonly IInvoiceRepositoryAsync _InvoiceRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        NAVSettings _NAVSettings { get; }

        private readonly string _taxnum;
        private readonly string _techUserLogin;
        private readonly string _techUserPwd;
        private readonly string _XMLSignKey;

        public getIncomingInvoicesNAVCommandHandler(IInvoiceRepositoryAsync InvoiceRepository, IMapper mapper, IOptions<NAVSettings> NAVSettings, IConfiguration configuration)
        {
            _InvoiceRepository = InvoiceRepository;
            _mapper = mapper;
            _NAVSettings = NAVSettings.Value;
            _configuration = configuration;

    }

    public async Task<Response<long>> Handle(importFromNAVCommand request, CancellationToken cancellationToken)
        {

            QueryInvoiceDigest(request);

            return new Response<long>(1);
        }

        private List<InvoiceDigestType> QueryInvoiceDigest(importFromNAVCommand request)
            {

            var invoiceDigestRes = new List<InvoiceDigestType>();

            var ter = new TokenExchangeRequest( _NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey);

            var reqTer = NAVUtil.Object2XMLString<TokenExchangeRequest>(ter);

            string response = "";
            if (bllNAV.NAVPost(NAVGlobal.NAV_TOKENEXCHANGE_TEST, ter.header.requestId, reqTer, _NAVSettings.SoftwareId, out response))
            {
                TokenExchangeResponse resp = NAVUtil.XMLStringToObject<TokenExchangeResponse>(response);
                var page = 1;

                var dig = new QueryInvoiceDigestRequest(_NAVSettings.Taxnum, _NAVSettings.TechUser, _NAVSettings.TechUserPwd, _NAVSettings.SignKey,
                                page, Enum.Parse<InvoiceDirectionType>( request.InvoiceDirection), true, request.IssueDateFrom, request.IssueDateTo);

                var digTer = NAVUtil.Object2XMLString<QueryInvoiceDigestRequest>(dig);

                if (bllNAV.NAVPost(NAVGlobal.NAV_QUERYINVOICEDIGEST_TEST, dig.header.requestId, digTer, _NAVSettings.SoftwareId, out response))
                {

                    QueryInvoiceDigestResponse respDigest = NAVUtil.XMLStringToObject<QueryInvoiceDigestResponse>(response);
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

                                var digTerPg = NAVUtil.Object2XMLString<QueryInvoiceDigestRequest>(digPg);

                                if (bllNAV.NAVPost(NAVGlobal.NAV_QUERYINVOICEDIGEST_TEST, digPg.header.requestId, digTerPg, _NAVSettings.SoftwareId, out response))
                                {
                                    QueryInvoiceDigestResponse respDigestPg = NAVUtil.XMLStringToObject<QueryInvoiceDigestResponse>(response);
                                    if (respDigestPg.result.funcCode == FunctionCodeType.OK)
                                    {
                                        invoiceDigestRes.AddRange(respDigestPg.invoiceDigestResult.invoiceDigest.ToList());
                                    }
                                }
                                else
                                {
                                    throw new Exception(String.Format("{0} NAV queryInvoiceDigest nextpage error result:{1}", _NAVSettings.SoftwareId, response));
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(String.Format("{0} NAV queryInvoiceDigest firstpage error result:{1}", _NAVSettings.SoftwareId, response));
                    }

                    Console.WriteLine(String.Format("{0} NAV queryInvoiceDigest OK, invoiceDirection:{1}, issue:{2}, dateFromUTC:{3}, dateToUTC:{4}", _NAVSettings.SoftwareId,
                        request.InvoiceDirection, true, request.IssueDateFrom, request.IssueDateTo));
                }
            }

            return invoiceDigestRes;
        }

        static InvoiceData QueryInvoiceData(string p_taxnum, string p_techUserLogin, string p_techUserPwd, string p_XMLSignKey,
            string p_invoiceNumber, InvoiceDirectionType p_invoiceDirection)
        {
            InvoiceData result = null;

            var ter = new TokenExchangeRequest(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey);
            var reqTer = NAVUtil.Object2XMLString<TokenExchangeRequest>(ter);
            string response = "";
            if (NAVLogic.Post(NAVConsts.NAV_TOKENEXCHANGE_TEST, ter.header.requestId, reqTer, procName, out response))
            {
                TokenExchangeResponse resp = XMLUtil.XMLStringToObject<TokenExchangeResponse>(response);

                var invData = new QueryInvoiceDataRequest(p_taxnum, p_techUserLogin, p_techUserPwd, p_XMLSignKey,
                                p_invoiceNumber, p_invoiceDirection);

                var invDataReq = XMLUtil.Object2XMLString<QueryInvoiceDataRequest>(invData);

                if (NAVLogic.Post(NAVConsts.NAV_QUERYINVOICEDATA_TEST, invData.header.requestId, invDataReq, procName, out response))
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
                                InvoiceDataStr = XMLUtil.Unzip(respInvoiceData.invoiceDataResult.invoiceData);
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
                            throw new Exception(String.Format("{0} invoice not found:{1}", procName, p_invoiceNumber));

                        }

                    }
                    else
                    {
                        throw new Exception(String.Format("{0} NAV queryInvoiceData error result:{1}", procName, response));
                    }

                    Console.WriteLine(String.Format("{0} NAV test result: funcCode:{1}, errorCode:{2}, message:{3}", procName, resp.result.funcCode,
                                   (resp.result.errorCode != null ? resp.result.errorCode : ""), (resp.result.message != null ? resp.result.message : "")));
                }
            }

            return result;
        }




    }
}
