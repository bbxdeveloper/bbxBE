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
	 * 
 {
  "customerID": 185997,
  "offerIssueDate": "2022-11-09",
  "offerVaidityDate": "2022-11-14",
  "notice": "<div><br></div><div><br></div><div><br></div><div><br></div><div><br></div><div>&#160; <br></div><div>&#160;<br></div>",
  "offerLines": [
    {
      "productCode": "000-0MEGT",
      "lineDescription": "megr. temér",
      "vatRateCode": "27%",
      "unitPriceOriginal": 30,
      "unitPriceSwitch": true,
      "unitPrice": 32,
      "unitGross": 41,
      "discount": 0,
      "showDiscount": true,
      "unitOfMeasure": "KILOGRAM",
      "quantity": 1,
      "lineNumber": 1
    }
  ],
  "offerGrossAmount": 41,
  "offerNetAmount": 32
}	*/
    public class CreateOfferCommand : IRequest<Response<Offer>>
	{


		[Description("Árajánlat-sor")]
		public class OfferLine
		{
			[ColumnLabel("#")]
			[Description("Sor száma")]
			public short LineNumber { get; set; }


			[ColumnLabel("Termékkód")]
			[Description("Termékkód")]
			public string ProductCode { get; set; }

			[ColumnLabel("Megnevezés")]
			[Description("A termék vagy szolgáltatás megnevezése")]
			public string LineDescription { get; set; }

			[ColumnLabel("Mennyiség")]
			[Description("Mennyiség")]
			public decimal Quantity { get; set; }
			[ColumnLabel("Me.e.")]
			[Description("Mennyiségi egység kód")]
			public string UnitOfMeasure { get; set; }

			[ColumnLabel("Árengedmény %")]
			[Description("Árengedmény %)")]
			public decimal Discount { get; set; }
			[ColumnLabel("Árengedmény megjelenítés?")]
			[Description("Árengedmény megjelenítés)")]
			public bool ShowDiscount { get; set; }

			/*
			[ColumnLabel("Áfa ID")]
			[Description("Áfa ID")]
			public long VatRateID { get; set; }
			*/


			[ColumnLabel("Áfaleíró-kód")]
			[Description("Áfaleíró-kód")]
			public string VatRateCode { get; set; }

			[ColumnLabel("Ár")]
			[Description("Ár")]
			public decimal UnitPrice { get; set; }

            [ColumnLabel("Eredeti ár")]                 //a törzsbeli ár
            [Description("Eredeti ár")]
            public decimal OriginalUnitPrice { get; set; }


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

		[ColumnLabel("Ügyfél ID")]
		[Description("Ügyfél ID")]
		public long CustomerID { get; set; }


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

		[ColumnLabel("Ajánlatsorok")]
		[Description("Ajánlatsorok")]
		public List<CreateOfferCommand.OfferLine> OfferLines { get; set; } = new List<CreateOfferCommand.OfferLine>();


        [ColumnLabel("Felhasználó ID")]
        [Description("Felhasználó ID")]
        public long? UserID { get; set; } = 0;


    }

    public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, Response<Offer>>
	{
		private readonly IOfferRepositoryAsync _OfferRepository;
		private readonly ICounterRepositoryAsync _CounterRepository;
		private readonly ICustomerRepositoryAsync _CustomerRepository;
		private readonly IProductRepositoryAsync _ProductRepository;
		private readonly IVatRateRepositoryAsync _VatRateRepository;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public CreateOfferCommandHandler(IOfferRepositoryAsync OfferRepository,
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

		public async Task<Response<Offer>> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
		{
			var offer = _mapper.Map<Offer>(request);

			offer.Notice = Utils.TidyHtml(offer.Notice);

			//Egyelőre csak forintos ajántatokról van szó
			if (string.IsNullOrWhiteSpace(offer.CurrencyCode))
			{
				offer.CurrencyCode = enCurrencyCodes.HUF.ToString();
				offer.ExchangeRate = 1;
			}

			var counterCode = "";
			try
			{

				offer.LatestVersion = true;
				//Árajánlatszám megállapítása
				counterCode = bbxBEConsts.DEF_OFFERCOUNTER;
				offer.OfferNumber = await _CounterRepository.GetNextValueAsync(counterCode, bbxBEConsts.DEF_WAREHOUSE_ID);
				offer.Copies = 1;

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
						throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_VATRATECODENOTFOUND, rln.VatRateCode));
					}

					//	ln.Product = prod;
					ln.ProductID = prod.ID;
					ln.ProductCode = rln.ProductCode;
					//Ez modelből jön: ln.LineDescription = prod.Description;

					//	ln.VatRate = vatRate;
					ln.VatRateID = vatRate.ID;
					ln.VatPercentage = vatRate.VatPercentage;

				}



				await _OfferRepository.AddOfferAsync(offer);

				await _CounterRepository.FinalizeValueAsync(counterCode, bbxBEConsts.DEF_WAREHOUSE_ID, offer.OfferNumber);

				return new Response<Offer>(offer);
			}
			catch (Exception ex)
			{
				if (!string.IsNullOrWhiteSpace(offer.OfferNumber) && !string.IsNullOrWhiteSpace(counterCode))
				{
					await _CounterRepository.RollbackValueAsync(counterCode, bbxBEConsts.DEF_WAREHOUSE_ID, offer.OfferNumber);
				}
				throw;
			}
		}
	}
}
