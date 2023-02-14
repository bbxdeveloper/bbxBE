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

			//Gyűjtőszámla - szállítólvél kapcsolat

			[ColumnLabel("Szállítólevél sor")]
			[Description("Kapcsolt szállítólevél sor")]
			public long? RelDeliveryNoteInvoiceLineID { get; set; }


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

        [ColumnLabel("Munkaszám")]
        [Description("Munkaszám")]
        public string WorkNumber { get; set; }

        [ColumnLabel("Ár felülvizsgálat?")]
        [Description("Ár felülvizsgálat?")]
        public bool? PriceReview { get; set; } = false;

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
			var invoice = _mapper.Map<Invoice>(request);
			var deliveryNotes = new Dictionary<int, Invoice>();
			var counterCode = "";

			try
			{

                /*****************************************/
                /* Mentés előtt Invoice mezők feltöltése */
                /*****************************************/


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

                var RelDeliveryNotes = new Dictionary<long, Invoice>();
                var RelDeliveryNoteLines = new Dictionary<long, InvoiceLine>();

				//gyűjtőszámla esetén kigyűjtjük a kapcsolt szállítólevél adatokat
				//
                if (request.InvoiceCategory == enInvoiceCategory.AGGREGATE.ToString())
                {
					var RelDeliveryNoteIDs = request.InvoiceLines.GroupBy(g => g.RelDeliveryNoteInvoiceLineID)
							.Select(s => s.Key.Value).ToList();
					RelDeliveryNotes = await _InvoiceRepository.GetInvoiceRecordsByInvoiceLinesAsync(RelDeliveryNoteIDs);

                    RelDeliveryNoteLines = await _InvoiceRepository.GetInvoiceLineRecordsAsync(
                        request.InvoiceLines.Select(s => s.RelDeliveryNoteInvoiceLineID.Value).ToList());

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


                //Tételsorok előfeldolgozása
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

					ln.PriceReview = request.PriceReview;

		
					//	Product
					//
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


                    decimal lineDiscountPercentage = 0;

                    if (request.InvoiceCategory == enInvoiceCategory.AGGREGATE.ToString())
                    {
						//gyűjtőszámla
                        if (!ln.RelDeliveryNoteInvoiceLineID.HasValue || ln.RelDeliveryNoteInvoiceLineID.Value > 0)
                        {
                            throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVAGGR_RELATED_NOT_ASSIGNED,
                                    invoice.InvoiceNumber, rln.LineNumber, rln.ProductCode));
                        }

                        //gyűjtőszámla esetén is egy árfolyam lesz!

                        if (!RelDeliveryNotes.ContainsKey(ln.RelDeliveryNoteInvoiceLineID.Value))
                        {
                            throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVAGGR_RELATED_NOT_FOUND,
                                    invoice.InvoiceNumber, rln.LineNumber, rln.ProductCode, ln.RelDeliveryNoteInvoiceLineID));
                        }

                        ln.RelDeliveryNoteNumber = RelDeliveryNotes[ln.RelDeliveryNoteInvoiceLineID.Value].InvoiceNumber;
                        ln.RelDeliveryNoteInvoiceID = RelDeliveryNotes[ln.RelDeliveryNoteInvoiceLineID.Value].ID;
                        ln.RelDeliveryNoteInvoiceLineID = ln.RelDeliveryNoteInvoiceLineID.Value;

                        ln.LineExchangeRate = RelDeliveryNotes[ln.RelDeliveryNoteInvoiceLineID.Value].ExchangeRate;
                        ln.LineDeliveryDate = RelDeliveryNotes[ln.RelDeliveryNoteInvoiceLineID.Value].InvoiceDeliveryDate;

						//NoDiscount a szállítólevél alapján van meghatáriza
						ln.NoDiscount = RelDeliveryNoteLines[ln.RelDeliveryNoteInvoiceLineID.Value].NoDiscount;

                        //Bizonylatkedvezmény a kapcsolt szállítólevél alapján
                        if (!prod.NoDiscount)
						{
							lineDiscountPercentage = RelDeliveryNotes[ln.RelDeliveryNoteInvoiceLineID.Value].InvoiceDiscountPercent;
						}
                    }
                    else
                    {
                        ln.LineExchangeRate = invoice.ExchangeRate;
                        ln.LineDeliveryDate = invoice.InvoiceDeliveryDate;
						//NoDiscount a cikktörzs alapján van meghatáriza
						ln.NoDiscount = prod.NoDiscount;

						//Bizonylatkedvezmény a request alapján
						if (!prod.NoDiscount)
						{
							lineDiscountPercentage = request.InvoiceDiscountPercent;
						}
                    }

                    ln.UnitPriceHUF = ln.UnitPrice * ln.LineExchangeRate;

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

	
					if (lineDiscountPercentage != 0)
					{
						InvoiceDiscount += Math.Round(ln.LineNetAmount * lineDiscountPercentage / 100, 2);
						InvoiceDiscountHUF += Math.Round(ln.LineNetAmountHUF * lineDiscountPercentage / 100, 2);

						ln.LineNetDiscountedAmount = Math.Round(ln.LineNetAmount * (1- lineDiscountPercentage / 100), 2);
						ln.LineNetDiscountedAmountHUF = Math.Round(ln.LineNetAmountHUF * (1 - lineDiscountPercentage / 100), 2);
						ln.LineVatDiscountedAmount = Math.Round(ln.LineVatAmount * (1 - lineDiscountPercentage / 100), 2);
						ln.LineVatDiscountedAmountHUF = Math.Round(ln.LineVatAmountHUF * (1 - lineDiscountPercentage / 100), 2);
                        ln.LineGrossDiscountedAmountNormal = Math.Round(ln.LineGrossAmountNormal * (1 - lineDiscountPercentage / 100), 2);
						ln.LineGrossDiscountedAmountNormalHUF = Math.Round(ln.LineGrossAmountNormalHUF * (1 - lineDiscountPercentage / 100), 2);
                    }
					else
					{
						ln.LineNetDiscountedAmount = ln.LineNetAmount;
						ln.LineNetDiscountedAmountHUF = ln.LineNetAmountHUF;
						ln.LineVatDiscountedAmount = ln.LineVatAmount;
						ln.LineVatDiscountedAmountHUF = ln.LineVatAmountHUF;
						ln.LineGrossDiscountedAmountNormal = ln.LineGrossAmountNormal;
						ln.LineGrossDiscountedAmountNormalHUF = ln.LineGrossAmountNormalHUF;
                    }
                }

				//SummaryByVatrate (Megj: A Bizonylatkedvezménnyel csökkentett árral kell számolni)

				invoice.SummaryByVatRates = invoice.InvoiceLines.GroupBy(g => g.VatRateID)
						.Select(g =>
						{
							var VatRateNetAmount = Math.Round(g.Sum(s => s.LineNetDiscountedAmount), 1);
							var VatRateNetAmountHUF = Math.Round(g.Sum(s => s.LineNetDiscountedAmountHUF), 1);
							var VatPercentage = g.First().VatPercentage;    //Áfa kulcs

                            var VatRateVatAmount = Math.Round(g.Sum(s => s.LineVatDiscountedAmount), (invoice.CurrencyCode == enCurrencyCodes.HUF.ToString() ? 0 : 1));
                            var VatRateVatAmountHUF = Math.Round(g.Sum(s => s.LineVatDiscountedAmountHUF), 0);


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


				await _InvoiceRepository.AddInvoiceAsync(invoice, RelDeliveryNoteLines);
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