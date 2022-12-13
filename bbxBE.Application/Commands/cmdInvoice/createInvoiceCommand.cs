using AutoMapper;
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
using static bbxBE.Common.NAV.NAV_enums;
using bbxBE.Common.NAV;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;

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

			[ColumnLabel("Nettó érték")]
			[Description("Ár a számla pénznemében")]
			public decimal LineNetAmount { get; set; }

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
		public long CustomerID { get; set; }

		[ColumnLabel("Kapcsolódó számla")]
		[Description("Bevételhez kapcsolódó számla")]
		public string CustomerInvoiceNumber { get; set; }

		[ColumnLabel("Fiz.mód")]
		[Description("Fizetési mód")]
		public string PaymentMethod { get; set; }

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

		[ColumnLabel("Számlasorok")]
		[Description("Számlasorok")]
		public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

	}

	public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Response<Invoice>>
	{
		private readonly IInvoiceRepositoryAsync _InvoiceRepository;
		private readonly ICounterRepositoryAsync _CounterRepository;
		private readonly IWarehouseRepositoryAsync _WarehouseRepository;
		private readonly ICustomerRepositoryAsync _CustomerRepository;
		private readonly IProductRepositoryAsync _ProductRepository;
		private readonly IVatRateRepositoryAsync _VatRateRepository;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public CreateInvoiceCommandHandler(IInvoiceRepositoryAsync InvoiceRepository,
			ICounterRepositoryAsync CounterRepository,
			IWarehouseRepositoryAsync WarehouseRepository,
			ICustomerRepositoryAsync CustomerRepository,
			IProductRepositoryAsync ProductRepository,
			IVatRateRepositoryAsync VatRateRepository,
			IMapper mapper, IConfiguration configuration)
		{
			_InvoiceRepository = InvoiceRepository;
			_CounterRepository = CounterRepository;
			_WarehouseRepository = WarehouseRepository;
			_CustomerRepository = CustomerRepository;
			_ProductRepository = ProductRepository;
			_VatRateRepository = VatRateRepository;
			_mapper = mapper;
			_configuration = configuration;
		}
		/*
{
  "customerID": 206568,
  "invoiceDeliveryDate": "2022-12-13",
  "invoiceIssueDate": "2022-12-13",
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
  "notice": "megjegyzés szövehg",
  "paymentDate": "2022-12-13",
  "paymentMethod": "TRANSFER",
  "warehouseCode": "001",
  "currencyCode": "HUF",
  "exchangeRate": 1,
  "incoming": false,
  "invoiceType": "INV",
  "invoiceDiscountPercent": 10
}

		 */


		public async Task<Response<Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
		{
			var invoice = _mapper.Map<Invoice>(request);

			var counterCode = "";
			try
			{


				//ID-k feloldása
				if (string.IsNullOrWhiteSpace(request.WarehouseCode))
				{
					request.WarehouseCode = bbxBEConsts.DEF_WAREHOUSE;      //Átmenetileg
				}

				if (string.IsNullOrWhiteSpace(request.CurrencyCode))
				{
					request.CurrencyCode = enCurrencyCodes.HUF.ToString();
				}
				var wh = await _WarehouseRepository.GetWarehouseByCodeAsync(request.WarehouseCode);
				if (wh == null)
				{
					throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.WarehouseCode));
				}
				invoice.WarehouseID = wh.ID;

				var ownData = _CustomerRepository.GetOwnData();
				if (ownData == null)
				{
					throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_OWNNOTFOUND));
				}

				if (request.Incoming)
				{
					invoice.SupplierID = request.CustomerID;
					invoice.CustomerID = ownData.ID;
				}
				else
				{
					invoice.SupplierID = ownData.ID;
					invoice.CustomerID = request.CustomerID;
				}

				//Megjegyzés
				if (!string.IsNullOrWhiteSpace(request.Notice))
				{
					invoice.AdditionalInvoiceData = new List<AdditionalInvoiceData>() {  new AdditionalInvoiceData()
							{ DataName = bbxBEConsts.DEF_NOTICE, DataDescription = bbxBEConsts.DEF_NOTICEDESC, DataValue = request.Notice }};

				}

				//Számlaszám megállapítása
				var invoiceType = (enInvoiceType)Enum.Parse(typeof(enInvoiceType), invoice.InvoiceType);
				counterCode = bllCounter.GetCounterCode(invoiceType, invoice.Incoming, wh.ID);
				invoice.InvoiceNumber = await _CounterRepository.GetNextValueAsync(counterCode, wh.ID);
				invoice.Copies = 1;

				//Szállítólevél esetén a rendezetlen mennyiséget is feltöltjük
				if (invoiceType == enInvoiceType.DNI || invoiceType == enInvoiceType.DNO)
				{
					invoice.PaymentMethod = PaymentMethodType.OTHER.ToString();
				}

				var paymentMethod = (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType), invoice.PaymentMethod);

				//Nettóból adott kedvezmény mértéke
				decimal InvoiceDiscount = 0;
				decimal InvoiceDiscountHUF = 0;


				//Tételsorok
				foreach (var ln in invoice.InvoiceLines)
				{
					var rln = request.InvoiceLines.SingleOrDefault(i => i.LineNumber == ln.LineNumber);


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

					ln.LineExchangeRate = invoice.ExchangeRate;             //gyűjtőszámla esetén is egy árfolyam lesz!

					//	ln.Product = prod;
					ln.ProductID = prod.ID;
					ln.ProductCode = rln.ProductCode;
					ln.Product = prod;


					ln.VTSZ = prod.ProductCodes.FirstOrDefault(c => c.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString()).ProductCodeValue;
					ln.LineDescription = prod.Description;

					//	ln.VatRate = vatRate;
					ln.VatRateID = vatRate.ID;
					ln.VatPercentage = vatRate.VatPercentage;
					//ln.VatRate = vatRate;

					ln.LineNatureIndicator = prod.NatureIndicator;

					ln.UnitPriceHUF = ln.UnitPrice * ln.LineExchangeRate;

					// A LineNetAmount a modelből jön !!! ln.LineNetAmount = Math.Round( ln.Quantity * ln.UnitPrice ,1);
					ln.LineNetAmountHUF = Math.Round(ln.LineNetAmount * ln.LineExchangeRate, 1);

					ln.LineVatAmount = Math.Round(ln.LineNetAmount * vatRate.VatPercentage, 1);
					ln.LineVatAmountHUF = Math.Round(ln.LineVatAmount * ln.LineExchangeRate, 1);

					ln.LineGrossAmountNormal = ln.LineNetAmount + ln.LineVatAmount;
					ln.LineGrossAmountNormalHUF = ln.LineNetAmountHUF + ln.LineVatAmountHUF;


					//Szállítólevél esetén a rendezetlen mennyiséget is feltöltjük
					if (invoiceType == enInvoiceType.DNI || invoiceType == enInvoiceType.DNO)
					{
						ln.PendingDNQuantity = ln.Quantity;
					}

					//Bizonylatkedvezmény
					if (!prod.NoDiscount)
					{
						InvoiceDiscount += Math.Round(ln.LineNetAmount * request.InvoiceDiscountPercent / 100, 2);
						InvoiceDiscountHUF += Math.Round(ln.LineNetAmountHUF * request.InvoiceDiscountPercent / 100, 2);
					}
				}

				//SummaryByVatrate
				invoice.SummaryByVatRates = invoice.InvoiceLines.GroupBy(g => g.VatRateID)
						.Select(g =>
						{
							var VatRateNetAmount = Math.Round(g.Sum(s => s.LineNetAmount * (1 - request.InvoiceDiscountPercent / 100)), 1);
							var VatRateNetAmountHUF = Math.Round(g.Sum(s => s.LineNetAmountHUF * (1 - request.InvoiceDiscountPercent / 100)), 1);
							var VatPercentage = g.First().VatPercentage;    //Áfa kulcs

							var VatRateVatAmount = Math.Round(VatRateNetAmount * VatPercentage, (invoice.CurrencyCode == enCurrencyCodes.HUF.ToString() ? 0 : 1));
							var VatRateVatAmountHUF = Math.Round(VatRateNetAmountHUF * VatPercentage, 0);

							var VatRateGrossAmount = VatRateNetAmount + VatRateVatAmount;
							var VatRateGrossAmountHUF = VatRateNetAmountHUF + VatRateVatAmountHUF;

							return new SummaryByVatRate()
							{
								VatRateID = g.Key,
								VatRateNetAmount = VatRateNetAmount,
								VatRateNetAmountHUF = VatRateNetAmountHUF,
								VatRateVatAmount = VatRateVatAmount,
								VatRateVatAmountHUF = VatRateVatAmountHUF,
								VatRateGrossAmount = VatRateGrossAmount,
								VatRateGrossAmountHUF = VatRateGrossAmountHUF
							};
						}
						).ToList();

				//összesítők

				//Kedvezmény nélküli nettó
				var InvoiceNetAmountWithoutDiscount = invoice.InvoiceLines.Sum(s => s.LineNetAmountHUF);
				var InvoiceNetAmountWithoutDiscountHUF = invoice.InvoiceLines.Sum(s => s.LineNetAmountHUF);

				//Nettóból adott kedvezmény mértéke
				invoice.InvoiceDiscount = Math.Round(InvoiceDiscount, 1);
				invoice.InvoiceDiscountHUF = Math.Round(InvoiceDiscountHUF, 1);

				//Kedvezménnyel csökkentett nettó
				invoice.InvoiceNetAmount = InvoiceNetAmountWithoutDiscount - invoice.InvoiceDiscount;
				invoice.InvoiceNetAmountHUF = InvoiceNetAmountWithoutDiscountHUF - invoice.InvoiceDiscountHUF;

				//Áfa értéke
				invoice.InvoiceVatAmount = invoice.SummaryByVatRates.Sum(s => s.VatRateVatAmount);
				invoice.InvoiceVatAmountHUF = invoice.SummaryByVatRates.Sum(s => s.VatRateVatAmountHUF);

				//Számla bruttó értéke (kedvezménnyel együtt)
				invoice.InvoiceGrossAmount = Math.Round(invoice.InvoiceNetAmount + invoice.InvoiceVatAmount, (invoice.CurrencyCode == enCurrencyCodes.HUF.ToString() ? 0 : 1));
				invoice.InvoiceGrossAmountHUF = Math.Round(invoice.InvoiceNetAmountHUF + invoice.InvoiceVatAmountHUF, 0);

				//kp-s kerekítések HUF számla esetén
				if (invoice.CurrencyCode == enCurrencyCodes.HUF.ToString()
					&& invoice.PaymentMethod == PaymentMethodType.CASH.ToString())
				{
					invoice.InvoiceGrossAmount = CASHRound(invoice.InvoiceGrossAmount);
					invoice.InvoiceGrossAmountHUF = CASHRound(invoice.InvoiceGrossAmountHUF);

				}


				await _InvoiceRepository.AddInvoiceAsync(invoice);
				await _CounterRepository.FinalizeValueAsync(counterCode, wh.ID, invoice.InvoiceNumber);


				invoice.InvoiceLines.Clear();
				invoice.SummaryByVatRates.Clear();
				if (invoice.AdditionalInvoiceData != null)
					invoice.AdditionalInvoiceData.Clear();
				return new Response<Invoice>(invoice);
			}
			catch (Exception ex)
			{
				if (!string.IsNullOrWhiteSpace(invoice.InvoiceNumber) && !string.IsNullOrWhiteSpace(counterCode))
				{
					await _CounterRepository.RollbackValueAsync(counterCode, invoice.WarehouseID, invoice.InvoiceNumber);
				}
				throw;
			}
			return null;
		}

		private decimal CASHRound(decimal p_num)
		{
			if (p_num == 0)
				return p_num;

			var lastDigit = (p_num % 10);
			var roundNum = 5;
			if (lastDigit >= 8)
			{
				roundNum = 10;
			}
			else if (lastDigit <= 2)
			{
				roundNum = 10;
			}
			else
			{
				roundNum = 5;
			}

			if (p_num > 0)
			{
				return p_num - lastDigit + roundNum;
			}
			else
			{
				return p_num + lastDigit - roundNum;
			}
		}

	}
}