using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using AngleSharp.Html;
using System.IO;
using bbxBE.Common;

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

			/*
			[ColumnLabel("Áfa értéke")]
			[Description("Áfa értéke")]
			public decimal UnitVat { get; set; }
			*/

			[ColumnLabel("Bruttó ár")]
			[Description("Bruttó ár")]
			public decimal UnitGross { get; set; }
		}
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
		public bool NewOffer { get; set; } = false;

		[ColumnLabel("Ajánlatsorok")]
		[Description("Ajánlatsorok")]
		public List<UpdateOfferCommand.OfferLine> OfferLines { get; set; } = new List<UpdateOfferCommand.OfferLine>();

	}


	public class UpdateOfferCommandHandler : IRequestHandler<UpdateOfferCommand, Response<Offer>>
	{
		private readonly IOfferRepositoryAsync _OfferRepository;
		private readonly ICounterRepositoryAsync _CounterRepository;
		private readonly ICustomerRepositoryAsync _CustomerRepository;
		private readonly IProductRepositoryAsync _ProductRepository;
		private readonly IVatRateRepositoryAsync _VatRateRepository;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public UpdateOfferCommandHandler(IOfferRepositoryAsync OfferRepository,
						ICounterRepositoryAsync CounterRepository,
						ICustomerRepositoryAsync CustomerRepository,
						IProductRepositoryAsync ProductRepository,
						IVatRateRepositoryAsync VatRateRepository,
						IMapper mapper, IConfiguration configuration)
		{
			_OfferRepository = OfferRepository;
			_CounterRepository = CounterRepository;
			_CustomerRepository = CustomerRepository;
			_ProductRepository = ProductRepository;
			_VatRateRepository = VatRateRepository;


			_mapper = mapper;
			_configuration = configuration;
		}

		public async Task<Response<Offer>> Handle(UpdateOfferCommand request, CancellationToken cancellationToken)
		{
			var offer = _mapper.Map<Offer>(request);
			offer.Notice = Utils.TidyHtml(offer.Notice);


			//Egyelőre csak forintos ajántatokról van szó
			offer.CurrencyCode = enCurrencyCodes.HUF.ToString();
			offer.ExchangeRate = 1;

			var counterCode = bbxBEConsts.DEF_OFFERCOUNTER;
			var newOfferNumber = "";

			try
			{


				//Tételsorok
				foreach (var ln in offer.OfferLines)
				{
					var rln = request.OfferLines.SingleOrDefault(i => i.LineNumber == ln.LineNumber);

					var prod = _ProductRepository.GetProductByProductCode(rln.ProductCode);
					if (prod == null)
					{
						throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODCODENOTFOUND, rln.ProductCode));
					}
					var vatRate = _VatRateRepository.GetVatRateByCode(rln.VatRateCode);
					if (vatRate == null)
					{
						throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_VATRATECODENOTFOUND, rln.VatRateCode));
					}

					//	ln.Product = prod;
					ln.ProductID = prod.ID;
					ln.ProductCode = rln.ProductCode;
					//Ez modelből jön: ln.LineDescription = prod.Description;

					//	ln.VatRate = vatRate;
					ln.VatRateID = vatRate.ID;
					ln.VatPercentage = vatRate.VatPercentage;
				}

				if (request.NewOffer)
				{

					newOfferNumber = await _CounterRepository.GetNextValueAsync(counterCode, bbxBEConsts.DEF_WAREHOUSE_ID);
					offer.OfferNumber = newOfferNumber;

					await _OfferRepository.AddOfferAsync(offer);
					await _CounterRepository.FinalizeValueAsync(counterCode, bbxBEConsts.DEF_WAREHOUSE_ID, offer.OfferNumber);
				}
				else
				{
					await _OfferRepository.UpdateOfferAsync(offer);
				}

				return new Response<Offer>(offer);
			}
			catch (Exception ex)
			{
				if (request.NewOffer && !string.IsNullOrWhiteSpace(newOfferNumber) && !string.IsNullOrWhiteSpace(counterCode))
				{
					await _CounterRepository.RollbackValueAsync(counterCode, bbxBEConsts.DEF_WAREHOUSE_ID, newOfferNumber);
				}
				throw;
			}
		}
	}
}
