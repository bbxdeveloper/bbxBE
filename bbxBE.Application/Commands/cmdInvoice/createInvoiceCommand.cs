using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using static bbxBE.Common.NAV.NAV_enums;

namespace bxBE.Application.Commands.cmdInvoice
{
    public class CreateInvoiceCommand : IRequest<Response<Invoice>>
    {

        [Description("Számlasor")]
        public class InvoiceLine
        {

            [ColumnLabel("#")]
            [Description("Sor száma")]
            public short LineNumber { get; set; }

            [ColumnLabel("Termékkód")]
            [Description("Termékkód")]
            public string ProductCode { get; set; }

            [ColumnLabel("Áfakód")]
            [Description("Áfakód")]
            public string VatRateCode { get; set; }

            [ColumnLabel("Mennyiség")]
            [Description("Mennyiség")]
            public decimal Quantity { get; set; }

            [ColumnLabel("Me.e.")]
            [Description("Mennyiségi egység kód")]
            public string UnitOfMeasure { get; set; }

            [ColumnLabel("Ár")]
            [Description("Ár")]
            public decimal UnitPrice { get; set; }

            //Gyűjtőszámla - szállítólvél kapcsolat

            [ColumnLabel("Szállítólevél sor")]
            [Description("Kapcsolt szállítólevél sor")]
            public long? RelDeliveryNoteInvoiceLineID { get; set; }

            [ColumnLabel("Új listaár")]
            [Description("Új listaár (csak bevételezés esetén értelmezett)")]
            public decimal NewUnitPrice1 { get; set; }

            [ColumnLabel("Új egységár")]
            [Description("Új egységár (csak bevételezés esetén értelmezett)")]
            public decimal NewUnitPrice2 { get; set; }

        }

        [ColumnLabel("B/K")]
        [Description("Bejővő/Kimenő")]
        public bool Incoming { get; set; }

        [ColumnLabel("Típus")]
        [Description("Típus")]
        public string InvoiceType { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Kelt")]
        [Description("Kiállítás dátuma")]
        public DateTime InvoiceIssueDate { get; set; }

        [ColumnLabel("Teljesítés")]
        [Description("Teljesítés dátuma")]
        public DateTime InvoiceDeliveryDate { get; set; }

        [ColumnLabel("Fiz.hat")]
        [Description("Fizetési határidő dátuma")]
        public DateTime PaymentDate { get; set; }


        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long? CustomerID { get; set; }

        [ColumnLabel("Kapcsolódó számla")]
        [Description("Bevételhez kapcsolódó számla")]
        public string CustomerInvoiceNumber { get; set; }

        [ColumnLabel("Fiz.mód")]
        [Description("Fizetési mód")]
        public string PaymentMethod { get; set; }
        // Javítószámla
        [ColumnLabel("Eredeti számla ID")]
        [Description("Az eredeti számla ID,amelyre a módosítás vonatkozik")]
        public long? OriginalInvoiceID { get; set; } = null;


        [ColumnLabel("Pénznem")]
        [Description("Pénznem")]
        public string CurrencyCode { get; set; }

        [ColumnLabel("Árfolyam")]
        [Description("Árfolyam")]
        public decimal ExchangeRate { get; set; }

        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Notice { get; set; }  //AdditionalInvoiceData-ban tároljuk!

        [ColumnLabel("Kedvezmény%")]
        [Description("A számlára adott teljes kedvezmény %")]
        public decimal InvoiceDiscountPercent { get; set; }

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;

        [ColumnLabel("Munkaszám")]
        [Description("Munkaszám")]
        public string WorkNumber { get; set; }

        [ColumnLabel("Ár felülvizsgálat?")]
        [Description("Ár felülvizsgálat jelölése")]
        public bool? PriceReview { get; set; } = false;

        [ColumnLabel("Módosító bizonylat?")]
        [Description("Módosító bizonylat jelölése (mínuszos szállító vagy javítószámla)")]
        public bool? InvoiceCorrection { get; set; } = false;

        [ColumnLabel("Típus")]
        [Description("Típus")]
        public string InvoiceCategory { get; set; } = enInvoiceCategory.NORMAL.ToString();


        [ColumnLabel("Számlasorok")]
        [Description("Számlasorok")]
        public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

    }

    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Response<Invoice>>
    {
        private readonly IInvoiceRepositoryAsync _InvoiceRepository;

        public CreateInvoiceCommandHandler(IInvoiceRepositoryAsync InvoiceRepository)
        {
            _InvoiceRepository = InvoiceRepository;
        }
        /*
{
  "customerID": 206568,
  "invoiceDeliveryDate": "2023-01-30",
  "invoiceIssueDate": "2023-01-30",
  "invoiceLines": [
    {
      "lineNetAmount": 23296,
      "lineNumber": 1,
      "quantity": 1,
      "productCode": "SCH-004600100",
      "productDescription": "Fali modul STR100",
      "unitOfMeasure": "PIECE",
      "unitPrice": 23296,
      "vatRate": 0.27,
      "vatRateCode": "27%"
    },
    {
      "lineNetAmount": 440,
      "lineNumber": 2,
      "quantity": 10,
      "productCode": "001-TESZTÚJ",
      "productDescription": "Új tesztadat neki nincs DÜW engedménye",
      "unitOfMeasure": "PIECE",
      "unitPrice": 44,
      "vatRate": 0.27,
      "vatRateCode": "27%"
    }
  ],
  "notice": "megjegyzés szöveg",
  "paymentDate": "2023-01-30",
  "paymentMethod": "OTHER",
  "warehouseCode": "001",
  "currencyCode": "HUF",
  "exchangeRate": 1,
  "incoming": false,
  "invoiceType": "DNO",
  "invoiceDiscountPercent": 10,
  "workNumber": "workNumber #1",
  "priceReview": true
}st
		 */


        public async Task<Response<Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {

            var inv = await _InvoiceRepository.CreateInvoiceAsynch(request, cancellationToken);
            return new Response<Invoice>(inv);
        }
    }
}