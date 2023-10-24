using AutoMapper;
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

namespace bxBE.Application.Commands.cmdOffer
{

    /*
	 
 {
  "offerVersion": 1,
  "newOffer": false,

"id":1,
"offerNumber": "AJ00005/22"
"customerID": 5,
  "offerIssueDate": "2022-05-20",
  "offerVaidityDate": "2022-05-20",
  "notice": "első MÓDOSITOTT ajánlat",
  "offerLines": [
    {
"id":1,
     "lineNumber": 1,
      "productCode": "VEG-2973",
      "lineDescription": "Boyler 600W fűtőbetét",
"UnitOfMeasure" : "PIECE",
     "vatRateCode": "27%",
      "discount": 10,
      "showDiscount": true,
       "unitPrice": 10,
      "unitPriceOriginal": 12,
      "unitPriceSwitch": false,
      "unitVat": 2.7,
      "unitGross": 12.7
    },
      {
"id":2,
     "lineNumber": 2,
      "productCode": "IZZ-861",
      "lineDescription": "HANDY 10139 Érvéghüvely prés MÓD",
     "vatRateCode": "27%",
"UnitOfMeasure" : "PIECE",
      "discount": 10,
      "showDiscount": true,
       "unitPrice": 100,
      "unitVat": 27,
      "unitGross": 127
    }
  ]
}

	 */
    public class UpdateOfferCommand : IRequest<Response<Offer>>
    {

        [Description("Árajánlat-sor")]
        public class OfferLine
        {
            [ColumnLabel("ID")]
            [Description("ID")]
            public long ID { get; set; }

            [ColumnLabel("#")]
            [Description("Sor száma")]
            public short LineNumber { get; set; }


            [ColumnLabel("Termékkód")]
            [Description("Termékkód")]
            public string ProductCode { get; set; }

            [ColumnLabel("Megnevezés")]
            [Description("A termék vagy szolgáltatás megnevezése")]
            public string LineDescription { get; set; }

            [ColumnLabel("Árengedmény %")]
            [Description("Árengedmény %)")]
            public decimal Discount { get; set; }

            [ColumnLabel("Árengedmény megjelenítés?")]
            [Description("Árengedmény megjelenítés)")]
            public bool ShowDiscount { get; set; }

            [ColumnLabel("Pénznem")]
            [Description("Pénznem")]
            public string CurrencyCode { get; set; }


            [ColumnLabel("Árfolyam")]
            [Description("Árfolyam")]
            public decimal ExchangeRate { get; set; }
            [ColumnLabel("Mennyiség")]
            [Description("Mennyiség")]
            public decimal Quantity { get; set; }

            [ColumnLabel("Me.e.")]
            [Description("Mennyiségi egység kód")]
            public string UnitOfMeasure { get; set; }

            [ColumnLabel("Áfaleíró-kód")]
            [Description("Áfaleíró-kód")]
            public string VatRateCode { get; set; }

            [ColumnLabel("Ár")]
            [Description("Ár")]
            public decimal UnitPrice { get; set; }

            [ColumnLabel("Eredeti ár")]                 //a törzsbeli ár
            [Description("Eredeti ár")]
            public decimal OriginalUnitPrice { get; set; }

            [ColumnLabel("Eredeti ár HUF")]                 //a törzsbeli ár
            [Description("Eredeti ár forintban")]
            public decimal OriginalUnitPriceHUF { get; set; }

            [ColumnLabel("E/L")]                        //Eygségár/listaár flag
            [Description("Egységár/Listaár")]
            public bool UnitPriceSwitch { get; set; }
            /*
                [ColumnLabel("Áfa értéke")]
                [Description("Áfa értéke")]
                public decimal UnitVat { get; set; }
                */

            [ColumnLabel("Bruttó ár")]
            [Description("Bruttó ár")]
            public decimal UnitGross { get; set; }
        }

        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }

        [ColumnLabel("Ajánlat száma")]
        [Description("Ajánlat száma")]
        public string OfferNumber { get; set; }

        [ColumnLabel("Kelt")]
        [Description("Kiállítás dátuma")]
        public DateTime OfferIssueDate { get; set; }

        [ColumnLabel("Érvényesség")]
        [Description("Érvényesség dátuma")]
        public DateTime OfferVaidityDate { get; set; }

        [ColumnLabel("Bruttó árak?")]
        [Description("Bruttó árak megjelenítése?")]
        public bool IsBrutto { get; set; }

        [ColumnLabel("Pénznem")]
        [Description("Pénznem")]
        public string CurrencyCode { get; set; }

        [ColumnLabel("Árfolyam")]
        [Description("Árfolyam")]
        public decimal ExchangeRate { get; set; }

        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Notice { get; set; }

        [ColumnLabel("Verzió")]
        [Description("Verzió")]
        public short OfferVersion { get; set; }


        [ColumnLabel("Új verzió?")]
        [Description("Új verzió?")]
        public bool NewOfferVersion { get; set; } = false;

        [ColumnLabel("Ajánlatsorok")]
        [Description("Ajánlatsorok")]
        public List<UpdateOfferCommand.OfferLine> OfferLines { get; set; } = new List<UpdateOfferCommand.OfferLine>();

        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;
    }


    public class UpdateOfferCommandHandler : IRequestHandler<UpdateOfferCommand, Response<Offer>>
    {
        private readonly IOfferRepositoryAsync _offerRepository;
        private readonly IMapper _mapper;

        public UpdateOfferCommandHandler(IOfferRepositoryAsync offerRepository, IMapper mapper)
        {
            _offerRepository = offerRepository;
            _mapper = mapper;
        }

        public async Task<Response<Offer>> Handle(UpdateOfferCommand request, CancellationToken cancellationToken)
        {
            var offer = _mapper.Map<Offer>(request);
            offer = await _offerRepository.UpdateOfferAsync(offer);
            return new Response<Offer>(offer);
        }
    }
}
