using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
using bbxBE.Application.Exceptions;
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

namespace bxBE.Application.Commands.cmdOffer
{
	public class UpdateOfferCommand2 : IRequest<Response<Offer>>
	{

		[Description("Árajánlat-sor")]
		public class OfferLine2
		{
			public short ID { get; set; }

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

			[ColumnLabel("Me.e.")]
			[Description("Mennyiségi egység kód")]
			public string UnitOfMeasure { get; set; }

			[ColumnLabel("Áfaleíró-kód")]
			[Description("Áfaleíró-kód")]
			public string VatRateCode { get; set; }

			[ColumnLabel("Ár")]
			[Description("Ár")]
			public decimal UnitPrice { get; set; }

			[ColumnLabel("Áfa értéke")]
			[Description("Áfa értéke")]
			public decimal UnitVat { get; set; }

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
		public List<UpdateOfferCommand2.OfferLine2> OfferLines { get; set; } = new List<UpdateOfferCommand2.OfferLine2>();

	}


	public class UpdateOfferCommandHandler : IRequestHandler<UpdateOfferCommand2, Response<Offer>>
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

        public async Task<Response<Offer>> Handle(UpdateOfferCommand2 request, CancellationToken cancellationToken)
        {
            var offer = _mapper.Map<Offer>(request);

			//Egyelőre csak forintos ajántatokról van szó
			offer.CurrencyCode = enCurrencyCodes.HUF.ToString();
			offer.ExchangeRate = 1;


			try
			{

				
				//Tételsorok
				foreach (var ln in offer.OfferLines)
				{
					var rln = request.OfferLines.SingleOrDefault(i => i.LineNumber == ln.LineNumber);

					var prod = _ProductRepository.GetProductByProductCode(rln.ProductCode);
					if (prod == null)
					{
						throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_PRODCODENOTFOUND, rln.ProductCode));
					}
					var vatRate = await _VatRateRepository.GetVatRateByCodeAsync(rln.VatRateCode);
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

					ln.UnitPriceHUF = ln.UnitPrice * offer.ExchangeRate;
					ln.UnitVatHUF = ln.UnitVat * offer.ExchangeRate;
					ln.UnitGrossHUF = ln.UnitGross * offer.ExchangeRate;
				}

		//		if (request.NewOffer)
				{
					await _OfferRepository.AddOfferAsync(offer);
				}
//				else
				{
					await _OfferRepository.UpdateOfferAsync(offer);
				}

				return new Response<Offer>(offer);
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}
