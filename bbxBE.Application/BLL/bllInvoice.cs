﻿using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Common.ExpiringData;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static bbxBE.Common.NAV.NAV_enums;

namespace bbxBE.Application.BLL
{

	public static class bllInvoice
	{

		/// <summary>
		/// Összegek és összesítők átszámolása
		/// </summary>
		/// <param name="invoice"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public static async Task<Invoice> CalcInvoiceAmountsAsynch(Invoice invoice, CancellationToken cancellationToken)
		{
			try
			{

				//Nettóból adott kedvezmény mértéke
				decimal InvoiceDiscount = 0;
				decimal InvoiceDiscountHUF = 0;

				//Tételsorok előfeldolgozása
				foreach (var ln in invoice.InvoiceLines)
				{

					ln.UnitPriceHUF = ln.UnitPrice * ln.LineExchangeRate;

					ln.LineNetAmount = Math.Round(ln.UnitPrice * ln.Quantity, 1);
					ln.LineNetAmountHUF = Math.Round(ln.LineNetAmount * ln.LineExchangeRate, 1);

					ln.LineVatAmount = Math.Round(ln.LineNetAmount * ln.VatPercentage, 1);
					ln.LineVatAmountHUF = Math.Round(ln.LineVatAmount * ln.LineExchangeRate, 1);

					ln.LineGrossAmountNormal = ln.LineNetAmount + ln.LineVatAmount;
					ln.LineGrossAmountNormalHUF = ln.LineNetAmountHUF + ln.LineVatAmountHUF;

					if (ln.LineDiscountPercent != 0)
					{
						//kerekítési veszteségek elkerüléséért 2 tizedesre kerekítjük a discounted mezőket
						//
						InvoiceDiscount += Math.Round(ln.LineNetAmount * ln.LineDiscountPercent / 100, 2);
						InvoiceDiscountHUF += Math.Round(ln.LineNetAmountHUF * ln.LineDiscountPercent / 100, 2);

						ln.LineNetDiscountedAmount = Math.Round(ln.LineNetAmount * (1 - ln.LineDiscountPercent / 100), 2);
						ln.LineNetDiscountedAmountHUF = Math.Round(ln.LineNetAmountHUF * (1 - ln.LineDiscountPercent / 100), 2);
						ln.LineVatDiscountedAmount = Math.Round(ln.LineVatAmount * (1 - ln.LineDiscountPercent / 100), 2);
						ln.LineVatDiscountedAmountHUF = Math.Round(ln.LineVatAmountHUF * (1 - ln.LineDiscountPercent / 100), 2);
						ln.LineGrossDiscountedAmountNormal = Math.Round(ln.LineGrossAmountNormal * (1 - ln.LineDiscountPercent / 100), 2);
						ln.LineGrossDiscountedAmountNormalHUF = Math.Round(ln.LineGrossAmountNormalHUF * (1 - ln.LineDiscountPercent / 100), 2);
					}
					else
					{
						// ln.LineDiscountPercent == 0 esetén ne legyenek benne kerekítések
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
				return invoice;
			}
			catch (Exception ex)
			{
				throw;
			}
		}



		public static async Task<Invoice> CreateInvoiceAsynch(CreateInvoiceCommand request,
						IMapper mapper,
						IInvoiceRepositoryAsync invoiceRepository,
						IInvoiceLineRepositoryAsync invoiceLineRepository,
						ICounterRepositoryAsync counterRepository,
						IWarehouseRepositoryAsync warehouseRepository,
						ICustomerRepositoryAsync customerRepository,
						IProductRepositoryAsync productRepository,
						IVatRateRepositoryAsync vatRateRepository,
                        IExpiringData<ExpiringDataObject> expiringData,
                        CancellationToken cancellationToken)
		{
			var invoice = mapper.Map<Invoice>(request);
			var deliveryNotes = new Dictionary<int, Invoice>();
			var counterCode = "";

			try
			{

				var paymentMethod = (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType), request.PaymentMethod);
				var invoiceType = (enInvoiceType)Enum.Parse(typeof(enInvoiceType), request.InvoiceType);

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
				var wh = await warehouseRepository.GetWarehouseByCodeAsync(request.WarehouseCode);
				if (wh == null)
				{
					throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.WarehouseCode));
				}
				invoice.WarehouseID = wh.ID;

				var ownData = customerRepository.GetOwnData();
				if (ownData == null)
				{
					throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_OWNNOTFOUND));
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

				var RelDeliveryNotesByLineID = new Dictionary<long, Invoice>();
				var RelDeliveryNoteLines = new Dictionary<long, InvoiceLine>();


				//Kezelni kell-e kapcsolt szállítóleveleket?
				//	- gyűjtőszámla esetén
				//	- Korrekciós be- ill. kimenő számla esetén
				//
				var hasRelDeliveryNotes = (request.InvoiceCategory == enInvoiceCategory.AGGREGATE.ToString() ||
									  (request.Correction.HasValue && request.Correction.Value &&
										(invoiceType == enInvoiceType.DNI || invoiceType == enInvoiceType.DNO)
										));


				if (hasRelDeliveryNotes)
				{
					var RelDeliveryNoteLineIDs = request.InvoiceLines.GroupBy(g => g.RelDeliveryNoteInvoiceLineID)
							.Select(s => s.Key.Value).ToList();
					RelDeliveryNotesByLineID = await invoiceRepository.GetInvoiceRecordsByInvoiceLinesAsync(RelDeliveryNoteLineIDs);

					RelDeliveryNoteLines = await invoiceLineRepository.GetInvoiceLineRecordsAsync(
						request.InvoiceLines.Select(s => s.RelDeliveryNoteInvoiceLineID.Value).ToList());
				}

				//Megjegyzés
				if (!string.IsNullOrWhiteSpace(request.Notice))
				{
					invoice.AdditionalInvoiceData = new List<AdditionalInvoiceData>() {  new AdditionalInvoiceData()
							{ DataName = bbxBEConsts.DEF_NOTICE, DataDescription = bbxBEConsts.DEF_NOTICEDESC, DataValue = request.Notice }};

				}


				//Szállítólevél esetén a PaymentMethod OTHER
				if (invoiceType == enInvoiceType.DNI || invoiceType == enInvoiceType.DNO)
				{
					invoice.PaymentMethod = PaymentMethodType.OTHER.ToString();
				}


				//Tételsorok előfeldolgozása
				foreach (var ln in invoice.InvoiceLines)
				{
					var rln = request.InvoiceLines.SingleOrDefault(i => i.LineNumber == ln.LineNumber);


					var prod = productRepository.GetProductByProductCode(rln.ProductCode);
					if (prod == null)
					{
						throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODCODENOTFOUND, rln.ProductCode));
					}

					var vatRate = vatRateRepository.GetVatRateByCode(rln.VatRateCode);
					if (vatRate == null)
					{
						throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_VATRATECODENOTFOUND, rln.VatRateCode));
					}

					ln.PriceReview = request.PriceReview;

					//	Product
					//
					ln.ProductID = prod.ID;
					ln.ProductCode = rln.ProductCode;
					ln.Product = prod;


					ln.VTSZ = prod.ProductCodes.FirstOrDefault(c => c.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString()).ProductCodeValue;
					ln.LineDescription = prod.Description;

					ln.VatRate = vatRate;
					ln.VatRateID = vatRate.ID;
					ln.VatPercentage = vatRate.VatPercentage;

					ln.LineNatureIndicator = prod.NatureIndicator;

					if (hasRelDeliveryNotes)
					{
						//gyűjtőszámla
						if (!ln.RelDeliveryNoteInvoiceLineID.HasValue || ln.RelDeliveryNoteInvoiceLineID.Value == 0)
						{
							throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVAGGR_RELATED_NOT_ASSIGNED,
									invoice.InvoiceNumber, rln.LineNumber, rln.ProductCode));
						}

						//gyűjtőszámla esetén is egy árfolyam lesz!

						if (!RelDeliveryNotesByLineID.ContainsKey(ln.RelDeliveryNoteInvoiceLineID.Value))
						{
							throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVAGGR_RELATED_NOT_FOUND,
									invoice.InvoiceNumber, rln.LineNumber, rln.ProductCode, ln.RelDeliveryNoteInvoiceLineID));
						}

						var relDeliveryNote = RelDeliveryNotesByLineID[ln.RelDeliveryNoteInvoiceLineID.Value];
						var relDeliveryNoteLine = RelDeliveryNoteLines[ln.RelDeliveryNoteInvoiceLineID.Value];

						ln.RelDeliveryNoteNumber = relDeliveryNote.InvoiceNumber;
						ln.RelDeliveryNoteInvoiceID = relDeliveryNote.ID;
						ln.RelDeliveryNoteInvoiceLineID = ln.RelDeliveryNoteInvoiceLineID.Value;

						ln.LineExchangeRate = relDeliveryNote.ExchangeRate;
						ln.LineDeliveryDate = relDeliveryNote.InvoiceDeliveryDate;

						//Bizonylatkedvezmény a kapcsolt szállítólevél alapján
						if (!prod.NoDiscount)
						{
							//NoDiscount a kapcsolt szállítólevél alapján van meghatáriza
							ln.NoDiscount = relDeliveryNoteLine.NoDiscount;
							ln.LineDiscountPercent = relDeliveryNoteLine.LineDiscountPercent;
						}
						else
						{
							//elég extrém helyzet, a szállítólevél adásakor még igen, de a gyűjtőszámla készítésekor
							//már nem adható kedvezmény.
							//
							//TODO: erre ne legyen figyelmeztetés?
							ln.NoDiscount = true;
							ln.LineDiscountPercent = relDeliveryNoteLine.LineDiscountPercent;
						}

						//Szállítólevélen lévő függő mennyiség aktualizálása
						if (relDeliveryNoteLine.PendingDNQuantity < Math.Abs(ln.Quantity))
						{
							throw new DataContextException(string.Format(bbxBEConsts.ERR_INVAGGR_WRONG_AGGR_QTY,
									relDeliveryNoteLine.Invoice.InvoiceNumber, rln.LineNumber, rln.ProductCode,
									 Math.Abs(ln.Quantity), relDeliveryNoteLine.PendingDNQuantity,
									ln.RelDeliveryNoteInvoiceLineID));
						}
						relDeliveryNoteLine.PendingDNQuantity -= Math.Abs(ln.Quantity); // mínuszos szállítólevelek miatt kell az abszolút érték
					}
					else
					{
						ln.LineExchangeRate = invoice.ExchangeRate;
						ln.LineDeliveryDate = invoice.InvoiceDeliveryDate;

						//NoDiscount a cikktörzs alapján van meghatározva
						ln.NoDiscount = prod.NoDiscount;

						//Bizonylatkedvezmény a request alapján
						if (!prod.NoDiscount)
						{
							ln.LineDiscountPercent = request.InvoiceDiscountPercent;
						}
					}

					//Normál szállítólevél esetén a rendezetlen mennyiséget is feltöltjük
					if ((invoiceType == enInvoiceType.DNI || invoiceType == enInvoiceType.DNO) &&
						(!request.Correction.HasValue || !request.Correction.Value))        // Szállítólevél korrekció esetén nincs PendingDNQuantity
					{
						ln.PendingDNQuantity = ln.Quantity;
					}

				}

				invoice = await CalcInvoiceAmountsAsynch(invoice, cancellationToken);

				//Számlaszám megállapítása
				counterCode = bllCounter.GetCounterCode(invoiceType, invoice.Incoming, wh.ID);
				invoice.InvoiceNumber = await counterRepository.GetNextValueAsync(counterCode, wh.ID);
				invoice.Copies = 1;

				await invoiceRepository.AddInvoiceAsync(invoice, RelDeliveryNoteLines);
				await counterRepository.FinalizeValueAsync(counterCode, wh.ID, invoice.InvoiceNumber);

				//szemafr kiütések
                var key = bbxBEConsts.DEF_CUSTOMERLOCK_KEY + invoice.CustomerID.ToString();
                await expiringData.DeleteItemAsync(key);
                key = bbxBEConsts.DEF_CUSTOMERLOCK_KEY + invoice.SupplierID.ToString();
                await expiringData.DeleteItemAsync(key);



                invoice.InvoiceLines.Clear();
				invoice.SummaryByVatRates.Clear();
				if (invoice.AdditionalInvoiceData != null)
					invoice.AdditionalInvoiceData.Clear();
				return invoice;
			}
			catch (Exception ex)
			{
				if (!string.IsNullOrWhiteSpace(invoice.InvoiceNumber) && !string.IsNullOrWhiteSpace(counterCode))
				{
					await counterRepository.RollbackValueAsync(counterCode, invoice.WarehouseID, invoice.InvoiceNumber);
				}
				throw;
			}
			return null;
		}

		public static async Task<Invoice> UpdatePricePreviewAsynch(UpdatePricePreviewCommand request,
				IMapper mapper,
				IInvoiceRepositoryAsync invoiceRepository,
				IProductRepositoryAsync productRepository,
				CancellationToken cancellationToken)
		{

			try
			{
				var invoice = await invoiceRepository.GetInvoiceRecordAsync(request.ID, true);
				if (invoice == null)
				{
					throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, request.ID));
					}

				invoice.CustomerID = request.CustomerID;
				

				//A SummaryByVatRates-t újrageneráljuk. Eltároljuk az eredei állapotot, mert az update során kitöröljük
				//
				var oriSummaryByVatRates = invoice.SummaryByVatRates; 

				//Tételsorok előfeldolgozása
				foreach (var rln in request.InvoiceLines)
				{
					var ln = invoice.InvoiceLines.Where( w=>w.ID == rln.ID).FirstOrDefault();
					if( ln == null)
                    {
						throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICELINENOTFOUND, invoice.ID, 
								invoice.InvoiceNumber, rln.ID));
					}
					ln.UnitPrice = rln.UnitPrice;

					if (ln.ProductID.HasValue)
					{
						var prod = productRepository.GetProduct(ln.ProductID.Value);
						if (prod == null)
						{
							throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODNOTFOUND, ln.ProductID.Value));
						}
						ln.NoDiscount = prod.NoDiscount;
					}
					ln.PriceReview = false;			//megtörtént az ár felülvizsgálat
				}

				invoice = await CalcInvoiceAmountsAsynch(invoice, cancellationToken);
				invoice.SummaryByVatRates.ToList().ForEach(i => i.InvoiceID = invoice.ID);


				await invoiceRepository.UpdateInvoiceAsync(invoice, oriSummaryByVatRates);

				invoice.InvoiceLines.Clear();
				invoice.SummaryByVatRates.Clear();
				if (invoice.AdditionalInvoiceData != null)
					invoice.AdditionalInvoiceData.Clear();
				return invoice;
			}
			catch (Exception ex)
			{
				throw;
			}
			return null;
		}
		public static decimal CASHRound(decimal p_num)
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
 