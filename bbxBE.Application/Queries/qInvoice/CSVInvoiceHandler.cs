using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qInvoice
{
    public class CSVInvoice : IRequest<FileStreamResult>
    {

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylat sorszáma")]
        public string InvoiceNumber { get; set; }

        [ColumnLabel("Bizonylattípus")]
        [Description("Bizonylattípus")]
        public string InvoiceType { get; set; }


        [ColumnLabel("Kelt.tól")]
        [Description("Kiállítás dátumától")]
        public DateTime? InvoiceIssueDateFrom { get; set; }

        [ColumnLabel("Kelt.ig")]
        [Description("Kiállítás dátumáig")]
        public DateTime? InvoiceIssueDateTo { get; set; }

        [ColumnLabel("Teljesítés tól")]
        [Description("Teljesítés dátumától")]
        public DateTime? InvoiceDeliveryDateFrom { get; set; }

        [ColumnLabel("Teljesítés ig")]
        [Description("Teljesítés dátumig")]
        public DateTime? InvoiceDeliveryDateTo { get; set; }

        public string OrderBy { get; set; }

    }

    public class CSVInvoiceHandler : IRequestHandler<CSVInvoice, FileStreamResult>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public CSVInvoiceHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<FileStreamResult> Handle(CSVInvoice request, CancellationToken cancellationToken)
        {

            var lstInv = await _invoiceRepository.QueryForCSVInvoiceAsync(request);
            string csv = String.Join(Environment.NewLine,
                    lstInv.Select(x =>
                                x.ID.ToString() + bbxBEConsts.DEF_CSVSEP +
                                x.WarehouseID.ToString() + bbxBEConsts.DEF_CSVSEP +
                                x.Warehouse + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceNumber + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceIssueDate.ToString(bbxBEConsts.DEF_DATEFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceDeliveryDate.ToString(bbxBEConsts.DEF_DATEFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                x.PaymentDate.ToString(bbxBEConsts.DEF_DATEFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                x.SupplierID.ToString() + bbxBEConsts.DEF_CSVSEP +
                                x.SupplierName + bbxBEConsts.DEF_CSVSEP +
                                x.SupplierBankAccountNumber + bbxBEConsts.DEF_CSVSEP +
                                (String.IsNullOrWhiteSpace(x.SupplierTaxpayerNumber.Replace("-", "")) ? "" : "\"" + x.SupplierTaxpayerNumber + "\"") + bbxBEConsts.DEF_CSVSEP +
                                x.SupplierCountryCode + bbxBEConsts.DEF_CSVSEP +
                                x.SupplierPostalCode + bbxBEConsts.DEF_CSVSEP +
                                x.SupplierCity + bbxBEConsts.DEF_CSVSEP +
                                x.SupplierAdditionalAddressDetail + bbxBEConsts.DEF_CSVSEP +
                                "\"" + x.SupplierThirdStateTaxId + "\"" + bbxBEConsts.DEF_CSVSEP +
                                x.SupplierComment + bbxBEConsts.DEF_CSVSEP +
                                x.CustomerID.ToString() + bbxBEConsts.DEF_CSVSEP +
                                x.CustomerName + bbxBEConsts.DEF_CSVSEP +
                                x.CustomerBankAccountNumber + bbxBEConsts.DEF_CSVSEP +
                                (String.IsNullOrWhiteSpace(x.CustomerTaxpayerNumber.Replace("-", "")) ? "" : "\"" + x.CustomerTaxpayerNumber + "\"") + bbxBEConsts.DEF_CSVSEP +
                                x.CustomerCountryCode + bbxBEConsts.DEF_CSVSEP +
                                x.CustomerPostalCode + bbxBEConsts.DEF_CSVSEP +
                                x.CustomerCity + bbxBEConsts.DEF_CSVSEP +
                                x.CustomerAdditionalAddressDetail + bbxBEConsts.DEF_CSVSEP +
                                "\"" + x.CustomerThirdStateTaxId + "\"" + bbxBEConsts.DEF_CSVSEP +
                                x.CustomerComment + bbxBEConsts.DEF_CSVSEP +
                                x.PaymentMethod + bbxBEConsts.DEF_CSVSEP +
                                x.PaymentMethodX + bbxBEConsts.DEF_CSVSEP +
                                x.CustomerInvoiceNumber + bbxBEConsts.DEF_CSVSEP +
                                x.Notice + bbxBEConsts.DEF_CSVSEP +
                                x.Copies.ToString() + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceDiscountPercent.ToString(bbxBEConsts.DEF_NUMFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceDiscount.ToString(bbxBEConsts.DEF_NUMFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceDiscountHUF.ToString(bbxBEConsts.DEF_NUMFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceNetAmount.ToString(bbxBEConsts.DEF_NUMFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceNetAmountHUF.ToString(bbxBEConsts.DEF_NUMFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceVatAmount.ToString(bbxBEConsts.DEF_NUMFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceVatAmountHUF.ToString(bbxBEConsts.DEF_NUMFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceGrossAmount.ToString(bbxBEConsts.DEF_NUMFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                x.InvoiceGrossAmountHUF.ToString(bbxBEConsts.DEF_NUMFORMAT) + bbxBEConsts.DEF_CSVSEP +
                                (x.InvoiceCorrection ? bbxBEConsts.DEF_TRUE : bbxBEConsts.DEF_FALSE) + bbxBEConsts.DEF_CSVSEP +
                                x.UserID.ToString() + bbxBEConsts.DEF_CSVSEP +
                                x.UserName + bbxBEConsts.DEF_CSVSEP +
                                x.WorkNumber + bbxBEConsts.DEF_CSVSEP +
                                (x.PriceReview ? bbxBEConsts.DEF_TRUE : bbxBEConsts.DEF_FALSE)
                    ).ToArray());

            var csvHeader = "ID;Raktár ID;Raktár;Bizonylatszám;Kelt;Teljesítés;Fiz.hat;Szállító ID;Szállítónév;Szállító bankszámlaszám;Szállító adószám;Szállító országkód;Szállító irányítószám;Szállító város;Szállítócím;Külföldi adószám;" +
                            "Szállító megjegyzés;Ügyfél ID;Ügyfélnév;Ügyfél bankszámlaszám;Ügyfél adószám;Ügyfél országkód;Ügyfél irányítószám;Ügyfél város; Ügyfélcím; Külföldi adószám;Ügyfél megjegyzés;" +
                            "Fiz.mód;Fizetési mód megnevezés;Eredeti.biz;Megjegyzés;Nyomtatott példány száma;Kedvezmény %;Kedvezmény;Kedvezmény HUF;Nettó;Nettó HUF;Áfa;Afa HUF;Bruttó;Bruttó HUF;" +
                            "Módosító bizonylat?;Felhasználó ID;Felhasználó;Munkaszám;Ár felülvizsgálat?";

            Random rnd = new Random();
            string fileName = $"Invoices_{rnd.Next()}.csv";
            var sbContent = new StringBuilder();

            sbContent.AppendLine(csvHeader);
            sbContent.Append(csv);

            var enc = Encoding.GetEncoding(bbxBEConsts.DEF_ENCODING);
            Stream stream = new MemoryStream(enc.GetBytes(sbContent.ToString()));
            var fsr = new FileStreamResult(stream, $"application/csv") { FileDownloadName = fileName };

            return fsr;
        }
    }
}