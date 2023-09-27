using AutoMapper;
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
            catch (Exception)
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
            var updatingProducts = new List<Product>();

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

                Customer cust = null;

                if (invoiceType != enInvoiceType.BLK)
                {
                    cust = customerRepository.GetCustomerRecord(request.CustomerID.Value);
                    if (cust == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_CUSTOMERNOTFOUND, request.CustomerID.Value));
                    }
                }

                var ownData = customerRepository.GetOwnData();
                if (ownData == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_OWNNOTFOUND));
                }

                if (invoiceType != enInvoiceType.BLK)
                {
                    if (request.Incoming)
                    {
                        invoice.SupplierID = request.CustomerID.Value;
                        invoice.CustomerID = ownData.ID;
                    }
                    else
                    {
                        invoice.SupplierID = ownData.ID;
                        invoice.CustomerID = request.CustomerID.Value;
                    }
                }
                else
                {
                    invoice.SupplierID = ownData.ID;
                    invoice.CustomerID = null;
                }

                var RelDeliveryNotesByLineID = new Dictionary<long, Invoice>();
                var RelDeliveryNoteLines = new Dictionary<long, InvoiceLine>();


                //Kezelni kell-e kapcsolt szállítóleveleket?
                //	- gyűjtőszámla esetén
                //	- Korrekciós be- ill. kimenő számla esetén
                //
                var hasRelDeliveryNotes = (request.InvoiceCategory == enInvoiceCategory.AGGREGATE.ToString() ||
                                        (invoiceType == enInvoiceType.DNI || invoiceType == enInvoiceType.DNO)
                                        && (request.InvoiceCorrection.HasValue && request.InvoiceCorrection.Value));


                if (hasRelDeliveryNotes)
                {
                    var RelDeliveryNoteLineIDs = request.InvoiceLines.GroupBy(g => g.RelDeliveryNoteInvoiceLineID)
                            .Select(s => s.Key.Value).ToList();
                    RelDeliveryNotesByLineID = await invoiceRepository.GetInvoiceRecordsByInvoiceLinesAsync(RelDeliveryNoteLineIDs);

                    RelDeliveryNoteLines = await invoiceLineRepository.GetInvoiceLineRecordsAsync(
                        request.InvoiceLines.Select(s => s.RelDeliveryNoteInvoiceLineID.Value).ToList());
                }

                int RealLineNumber = 1;

                //Javítószámla

                /* Jóváírás-kezelés:
                  Adózó módosító számlát bocsát ki, ezen módosító tételként -4 darab „D termék”,
                  illetve 1 darab „F termék” szerepel. Ezen módosításról történő adatszolgáltatásban az adózó újabb
                  tételként szerepelteti a -4 db „D terméket”, illetve az 1 db „F terméket”.
                  Ezen módosító okiratról történő adatszolgáltatásban:
                  a) A módosító okiratot leíró XML első tételsorában (lineNumber=1) a LineModificationReference
                  elemben a lineNumberReference elem értéke „6”, lineOperation elem értéke „CREATE”, ez
                  tartalmazza a -4 darab „D termék” adatait.NAV Online Számla Rendszer 100. oldal
                  b) A módosító okiratot leíró XML második tételsorában (lineNumber=2) a LineModificationReference
                  elemben a lineNumberReference elem értéke „7”, lineOperation elem értéke „CREATE”, ez
                  tartalmazza az 1 darab „F termék” adatait.
                  c) A módosító okiratot leíró XML invoiceSummary eleme teljes egészében szerepel, abban az egyes
                  értékek módosulásának előjeles összege szerepel.

                  RÖVIDEN : A jóváíró számlával az eredeti számlatételekhez "hozzáírjuk" a javító tételeket mínuszosan.
                  */

                List<Invoice> ModificationInvoices = new List<Invoice>();
                Invoice OriginalInvoice = null;
                var isInvoiceCorrection = (request.OriginalInvoiceID.HasValue && request.OriginalInvoiceID.Value != 0)
                                            && (request.InvoiceCorrection.HasValue && request.InvoiceCorrection.Value);
                if (isInvoiceCorrection)
                {
                    OriginalInvoice = await invoiceRepository.GetInvoiceRecordAsync(request.OriginalInvoiceID.Value, true);
                    if (OriginalInvoice == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_ORIGINALINVOICENOTFOUND, request.OriginalInvoiceID.Value));
                    }

                    // Javító bizonylat linenumber meghatározás
                    //      - az eredeti bizonylat linenumber utáni tétel
                    //        + engedmény esetén áfakódonként 1 (az engedmény a NAV-hoz áfánként, tételsorokban van felküldve) sor készül a  NAV-hoz felküldött adatokban
                    //      - már elkészült javítószámlák tétel összesen
                    //        + engedmény esetén már elkészült javítószámlák áfakódonként 1 (az engedmény a NAV-hoz áfánként, tételsorokban van felküldve)

                    // eredeti számla
                    RealLineNumber += OriginalInvoice.InvoiceLines.Count()
                                     + (OriginalInvoice.InvoiceLines.Where(a => a.LineDiscountPercent != 0).GroupBy(g => g.VatRateID).Count());

                    // már elkészült javítószámlák (láncolt javítás)
                    ModificationInvoices = await invoiceRepository.GetCorrectionInvoiceRecordsByInvoiceID(request.OriginalInvoiceID.Value);
                    ModificationInvoices.ForEach(oi =>
                    {
                        RealLineNumber += oi.InvoiceLines.Count()
                                     + (oi.InvoiceLines.Where(a => a.LineDiscountPercent != 0).GroupBy(g => g.VatRateID).Count());
                    });

                    invoice.OriginalInvoiceNumber = OriginalInvoice.InvoiceNumber;
                    invoice.ModificationIndex = (short)(ModificationInvoices.Count() + 1);
                    invoice.ModifyWithoutMaster = false;
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
                var lineErrors = new List<string>();
                foreach (var ln in invoice.InvoiceLines)
                {
                    var rln = request.InvoiceLines.SingleOrDefault(i => i.LineNumber == ln.LineNumber);


                    var prod = productRepository.GetProductByProductCode(rln.ProductCode);
                    if (prod == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODCODENOTFOUND, rln.ProductCode));
                    }


                    if (!hasRelDeliveryNotes &&         //nem gyűjtőszámla
                        invoice.Incoming &&             //bevételezés
                        (invoice.InvoiceType == enInvoiceType.INC.ToString() || invoice.InvoiceType == enInvoiceType.DNI.ToString()))   //szla.v.száll.
                    {
                        prod.LatestSupplyPrice = rln.UnitPrice;     //megjegzezük a legutolsó eladási árat
                        prod.UnitPrice1 = rln.NewUnitPrice1;        //árváltozás, úgy listaár
                        prod.UnitPrice2 = rln.NewUnitPrice2;        //árváltozás, úgy egységár
                        updatingProducts.Add(prod);
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

                        //Gyűjtőszámla kiegészítő adatok
                        ln.AdditionalInvoiceLineData = new List<AdditionalInvoiceLineData>();
                        ln.AdditionalInvoiceLineData.Add(new AdditionalInvoiceLineData()   //a kapcsolt szállítólevél számát külön is letároljuk, hogy menjen fel a NAV-hoz
                        {
                            DataName = "X001_RELDELIVERYNOTE",
                            DataDescription = bbxBEConsts.DEF_RELDELIVERYNOTE,
                            DataValue = relDeliveryNote.InvoiceNumber
                        });

                        ln.AdditionalInvoiceLineData.Add(new AdditionalInvoiceLineData()   //a kapcsolt szállítólevél engedményt is letároljuk, hogy menjen fel a NAV-hoz
                        {
                            DataName = "X001_RELDELIVERYDISCOUNTPERCENT",
                            DataDescription = bbxBEConsts.DEF_RELDELIVERYDISCOUNTPERCENT,
                            DataValue = ln.LineDiscountPercent.ToString()
                        });


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
                        (!request.InvoiceCorrection.HasValue || !request.InvoiceCorrection.Value))        // Szállítólevél korrekció esetén nincs PendingDNQuantity
                    {
                        ln.PendingDNQuantity = ln.Quantity;
                    }

                    if (isInvoiceCorrection)
                    {
                        //Termékkód ell.
                        if (!OriginalInvoice.InvoiceLines.Any(w => w.ProductID == ln.ProductID))
                        {
                            lineErrors.Add(string.Format(bbxBEConsts.ERR_CORRECTIONUNKOWNPROD, ln.ProductID, ln.ProductCode));

                        }
                        else
                        {

                            //Mennyiség ell.
                            var origQty = OriginalInvoice.InvoiceLines.Where(w => w.ProductID == ln.ProductID).Sum(s => s.Quantity);
                            var modQty = ModificationInvoices.Sum(s => s.InvoiceLines.Where(w => w.ProductID == ln.ProductID).Sum(s => s.Quantity));
                            if (origQty + modQty + ln.Quantity < 0)
                            {
                                lineErrors.Add(string.Format(bbxBEConsts.ERR_WRONGCORRECTIONQTY,
                                        ln.ProductCode,
                                        origQty,
                                        modQty,
                                        ln.Quantity));
                            }
                        }

                        // linenumber véglegesítés javítószámla esetén
                        ln.LineNumber = (short)(RealLineNumber++);
                        ln.LineNumberReference = ln.LineNumber;          //Mivel a javítószámlán a CREATE operációt használjuk csak, ezért a LineNumberReference megyegyezik a LineNumber-el
                                                                         //NAV doc:Az eredeti számla módosítással érintett tételének sorszáma, (lineNumber).Új tétel létrehozása esetén az új tétel sorszáma, az eredeti számla folytatásaként
                    }

                    //termékdíj
                    if (prod.ProductFee > 0)
                    {
                        ln.TakeoverReason = TakeoverType.Item02_ga.ToString();                  //egyelőre beégetjük a 02_ga-t

                        if (ln.TakeoverReason != TakeoverType.Item01.ToString())                 //későbbi felhasználásra is felkészülünk, 01 esetén nincs átvállalás
                        {
                            ln.TakeoverAmount = Math.Round(prod.ProductFee * ln.Quantity, 1);
                        }

                        //ln.ProductFeeProductCodeValue = //KT v. CSK
                        ln.ProductFeeQuantity = ln.Quantity;
                        ln.ProductFeeMeasuringUnit = ln.UnitOfMeasure;
                        //ln.ProductFeeRate = 
                        ln.ProductFeeAmount = Math.Round(prod.ProductFee * ln.Quantity, 1);
                    }
                }
                if (lineErrors.Any())
                {
                    throw new ValidationException(lineErrors);
                }

                invoice = await CalcInvoiceAmountsAsynch(invoice, cancellationToken);

                //Bizonylatszám megállapítása
                counterCode = bllCounter.GetCounterCode(invoiceType, paymentMethod, invoice.Incoming, isInvoiceCorrection, wh.ID);
                invoice.InvoiceNumber = await counterRepository.GetNextValueAsync(counterCode, wh.ID);
                invoice.Copies = 1;


                await invoiceRepository.AddInvoiceAsync(invoice, RelDeliveryNoteLines);
                await counterRepository.FinalizeValueAsync(counterCode, wh.ID, invoice.InvoiceNumber);
                if (updatingProducts.Count > 0)
                {
                    await productRepository.UpdateProductRangeAsync(updatingProducts, true);
                }

                if (!request.Incoming
                     && !isInvoiceCorrection && !hasRelDeliveryNotes
                     && (invoiceType == enInvoiceType.INV || invoiceType == enInvoiceType.DNO)
                     && cust != null)
                {
                    cust.LatestDiscountPercent = request.InvoiceDiscountPercent;
                    await customerRepository.UpdateAsync(cust);

                }

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
            catch (Exception)
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
                    var ln = invoice.InvoiceLines.Where(w => w.ID == rln.ID).FirstOrDefault();
                    if (ln == null)
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
                    ln.PriceReview = false;         //megtörtént az ár felülvizsgálat
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
            catch (Exception)
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


        public static async Task<InvoiceData> GetInvoiceNAVXMLAsynch(Invoice invoice,
                  IInvoiceRepositoryAsync invoiceRepository,
                  CancellationToken cancellationToken)
        {

            try
            {

                if (invoice.InvoiceType != enInvoiceType.INV.ToString())
                {
                    throw new Exception(string.Format(bbxBEConsts.ERR_NAVXML_NOINV, invoice.ID, invoice.InvoiceNumber));

                }

                var invoiceDataNAV = new InvoiceData();
                invoiceDataNAV.invoiceNumber = invoice.InvoiceNumber;
                invoiceDataNAV.invoiceIssueDate = invoice.InvoiceIssueDate;
                var invoiceNAV = new InvoiceType();

                invoiceDataNAV.invoiceMain.Items = new InvoiceType[] { invoiceNAV }; //számánként egy-egy adatküldés


                Invoice originalInvoice = null;
                if (invoice.InvoiceCorrection)
                {
                    originalInvoice = await invoiceRepository.GetInvoiceRecordAsync(invoice.OriginalInvoiceID.Value, true);
                    if (originalInvoice == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_ORIGINALINVOICENOTFOUND, invoice.OriginalInvoiceID.Value));
                    }
                    var invRefNAV = new InvoiceReferenceType();
                    invRefNAV.modifyWithoutMaster = invoice.ModifyWithoutMaster;
                    invRefNAV.modificationIndex = invoice.ModificationIndex;
                    invRefNAV.originalInvoiceNumber = invoice.OriginalInvoiceNumber;
                    invoiceNAV.invoiceReference = invRefNAV;
                }

                /////////////////
                // F E J L É C //
                /////////////////

                InvoiceHeadType invHeadNAV = new InvoiceHeadType();
                invoiceNAV.invoiceHead = invHeadNAV;
                invHeadNAV.supplierInfo = new SupplierInfoType();
                invHeadNAV.supplierInfo.supplierTaxNumber = new TaxNumberType();

                invHeadNAV.supplierInfo.supplierTaxNumber.taxpayerId = invoice.Supplier.TaxpayerId;
                invHeadNAV.supplierInfo.supplierTaxNumber.vatCode = invoice.Supplier.VatCode;
                invHeadNAV.supplierInfo.supplierTaxNumber.countyCode = invoice.Supplier.CountyCode;

                invHeadNAV.supplierInfo.supplierName = invoice.Supplier.CustomerName;

                invHeadNAV.supplierInfo.supplierAddress = new AddressType();
                var supplierAddressNAV = new SimpleAddressType();
                invHeadNAV.supplierInfo.supplierAddress.Item = supplierAddressNAV;

                supplierAddressNAV.countryCode = (!string.IsNullOrWhiteSpace(invoice.Supplier.CountryCode) ? invoice.Supplier.CountryCode : NAVGlobal.NAV_HU);
                supplierAddressNAV.postalCode = invoice.Supplier.PostalCode;
                supplierAddressNAV.city = invoice.Supplier.City;
                if (!string.IsNullOrWhiteSpace(invoice.Supplier.AdditionalAddressDetail))
                    supplierAddressNAV.additionalAddressDetail = invoice.Supplier.AdditionalAddressDetail;


                invHeadNAV.supplierInfo.supplierBankAccountNumber = invoice.Supplier.CustomerBankAccountNumber;
                //invHead.supplierInfo.individualExemption = false;                 //BBX: nem tartjuk nyilván
                //invHead.supplierInfo.exciseLicenceNum                             //BBX: nem tartjuk nyilván

                ///////////////

                invHeadNAV.customerInfo = new CustomerInfoType();

                if (invoice.Customer.CustomerVatStatus != CustomerVatStatusType.PRIVATE_PERSON.ToString())
                {
                    //Külföldi vevő esetén a a vatStatus OTHER
                    if (!string.IsNullOrWhiteSpace(invoice.Customer.CountryCode) && invoice.Customer.CountryCode != NAVGlobal.NAV_HU)
                    {
                        invHeadNAV.customerInfo.customerVatStatus = CustomerVatStatusType.OTHER;
                        if (!string.IsNullOrWhiteSpace(invoice.Customer.ThirdStateTaxId))
                        {
                            invHeadNAV.customerInfo.customerVatData = new CustomerVatDataType();
                            invHeadNAV.customerInfo.customerVatData.ItemElementName = ItemChoiceType.communityVatNumber;
                            invHeadNAV.customerInfo.customerVatData.Item = invoice.Customer.ThirdStateTaxId;
                        }
                    }
                    else
                    {
                        invHeadNAV.customerInfo.customerVatStatus = CustomerVatStatusType.DOMESTIC;

                        invHeadNAV.customerInfo.customerVatData = new CustomerVatDataType();
                        var customerTaxNumber = new CustomerTaxNumberType();

                        customerTaxNumber.taxpayerId = invoice.Customer.TaxpayerId;
                        customerTaxNumber.vatCode = invoice.Customer.VatCode;
                        customerTaxNumber.countyCode = invoice.Customer.CountyCode;

                        invHeadNAV.customerInfo.customerVatData.ItemElementName = ItemChoiceType.customerTaxNumber;
                        invHeadNAV.customerInfo.customerVatData.Item = customerTaxNumber;
                    }

                    invHeadNAV.customerInfo.customerName = invoice.Customer.CustomerName;
                    invHeadNAV.customerInfo.customerAddress = new AddressType();
                    var customerAddressNAV = new SimpleAddressType();
                    invHeadNAV.customerInfo.customerAddress.Item = customerAddressNAV;
                    customerAddressNAV.countryCode = (!string.IsNullOrWhiteSpace(invoice.Customer.CountryCode) ? invoice.Customer.CountryCode : NAVGlobal.NAV_HU);
                    customerAddressNAV.postalCode = (!string.IsNullOrWhiteSpace(invoice.Customer.PostalCode) ? invoice.Customer.PostalCode : "0000");
                    customerAddressNAV.city = invoice.Customer.City;
                    if (!string.IsNullOrWhiteSpace(invoice.Customer.AdditionalAddressDetail))
                        customerAddressNAV.additionalAddressDetail = invoice.Customer.AdditionalAddressDetail;


                    invHeadNAV.customerInfo.customerBankAccountNumber = invoice.Customer.CustomerBankAccountNumber;
                }
                else
                {
                    invHeadNAV.customerInfo.customerVatStatus = CustomerVatStatusType.PRIVATE_PERSON;
                    invHeadNAV.customerInfo.customerVatData = null;

                }

                ///////////
                invHeadNAV.fiscalRepresentativeInfo = null;                        //BBX: nem kezeljük


                var smallBusinessIndicator = false;                                 //nem kisadózó

                var invoiceDetailNAV = new InvoiceDetailType(
                    p_invoiceDeliveryPeriodStartSpecified: false,
                    p_invoiceDeliveryPeriodEndSpecified: false,
                    p_invoiceAccountingDeliveryDateSpecified: false,
                    p_periodicalSettlementSpecified: false,
                    p_smallBusinessIndicatorSpecified: smallBusinessIndicator,
                    p_utilitySettlementIndicatorSpecified: false,
                    p_selfBillingIndicatorSpecified: false,
                    p_paymentMethodSpecified: true,
                    p_paymentDateSpecified: true,
                    p_cashAccountingIndicatorSpecified: smallBusinessIndicator      //A kisadózó pénforgalmi elszámolású
                    );
                invHeadNAV.invoiceDetail = invoiceDetailNAV;


                invoiceDetailNAV.invoiceCategory = Enum.Parse<InvoiceCategoryType>(invoice.InvoiceCategory);

                invoiceDetailNAV.periodicalSettlement = false;                 //BBX:ilyen nincs a NYIL-ban
                invoiceDetailNAV.invoiceDeliveryDate = invoice.InvoiceDeliveryDate;
                //invoiceDetailNAV.invoiceDeliveryPeriodStart                //BBX:nincs időszakra számlázás
                //invoiceDetailNAV.invoiceDeliveryPeriodEnd                  //BBX:nincs időszakra számlázás
                //invoiceDetailNAV.invoiceAccountingDeliveryDate = Util.GetDateTimeField(rSzamlak, "SZAMLAF");   //Nem kötelező, nem töltjük !

                invoiceDetailNAV.currencyCode = invoice.CurrencyCode;
                invoiceDetailNAV.exchangeRate = invoice.ExchangeRate;

                invoiceDetailNAV.paymentMethod = Enum.Parse<PaymentMethodType>(invoice.PaymentMethod);
                invoiceDetailNAV.paymentDate = invoice.PaymentDate;

                //invoiceData.cashAccountingIndicator                   //BBX: nem kezeljük      
                invoiceDetailNAV.invoiceAppearance = InvoiceAppearanceType.PAPER;
                //invoiceDataNAV.electronicInvoiceHash                    //BBX: a számlánk papír típusú

                if (invoice.AdditionalInvoiceData?.Count > 0)
                {
                    invoiceDetailNAV.additionalInvoiceData =
                        invoice.AdditionalInvoiceData.Select(s =>
                        new AdditionalDataType()
                        {
                            dataDescription = s.DataDescription,
                            dataName = s.DataName,
                            dataValue = s.DataValue
                        }).ToArray();
                }

                ///////////////
                // S O R O K //
                ///////////////
                var invoiceLinesNAV = new List<LineType>();
                foreach (InvoiceLine ili in invoice.InvoiceLines)
                {
                    var invlineNAV = new LineType(
                            p_lineNatureIndicatorSpecified: false,
                            p_quantitySpecified: true,
                            p_unitOfMeasureSpecified: true,
                            p_unitPriceSpecified: true,
                            p_unitPriceHUFSpecified: true,
                            p_intermediatedServiceSpecified: false,
                            p_depositIndicatorSpecified: false,
                            p_obligatedForProductFeeSpecified: false,
                            p_GPCExciseSpecified: false,
                            p_netaDeclarationSpecified: false);
                    invoiceLinesNAV.Add(invlineNAV);

                    if (string.IsNullOrWhiteSpace(ili.LineDescription))
                    {
                        throw new Exception(String.Format(bbxBEConsts.ERR_LINEDESC_MISSING, invoice.InvoiceNumber, ili.ProductCode));
                    }


                    invlineNAV.lineNumber = ili.LineNumber.ToString();
                    if (invoiceDetailNAV.invoiceCategory == InvoiceCategoryType.AGGREGATE)
                    {
                        ////////////////////////
                        // Gyűjtőszámla sorok //
                        ////////////////////////
                        invlineNAV.aggregateInvoiceLineData = new AggregateInvoiceLineDataType(
                                    p_lineExchangeRateSpecified: true);
                        invlineNAV.aggregateInvoiceLineData.lineExchangeRate = ili.LineExchangeRate;
                        invlineNAV.aggregateInvoiceLineData.lineDeliveryDate = ili.LineDeliveryDate;

                    }

                    if (ili.AdditionalInvoiceLineData?.Count > 0)
                    {
                        invlineNAV.additionalLineData =
                            ili.AdditionalInvoiceLineData.Select(s =>
                            new AdditionalDataType()
                            {
                                dataDescription = s.DataDescription,
                                dataName = s.DataName,
                                dataValue = s.DataValue
                            }).ToArray();
                    }

                    //Módosító szála
                    if (invoice.InvoiceCorrection)
                    {
                        invlineNAV.lineModificationReference = new LineModificationReferenceType();

                        //A javítószámlán lévő tétellel korrigáljuk az eredetit
                        invlineNAV.lineModificationReference.lineOperation = LineOperationType.CREATE;

                        //API lerírás: a lineNumberReference(a számla és összes módosításaiban) sorfolytonosan új tételsorszámra mutat és lineOperation értéke „CREATE”.
                        //
                        invlineNAV.lineModificationReference.lineNumberReference = invlineNAV.lineNumber;
                    }

                    //invlineNAV.referencesToOtherLines               //BBX: nem kezeljük
                    //invlineNAV.advanceIndicator                     //BBX: nem kezeljük, nincs előleg jellegű tétel

                    var productCodes = new List<ProductCodeType>();
                    var productCode = new ProductCodeType()
                    {
                        productCodeCategory = ProductCodeCategoryType.OWN,
                        Item = ili.ProductCode
                    };
                    productCodes.Add(productCode);

                    var VTSZCode = new ProductCodeType()
                    {
                        productCodeCategory = ProductCodeCategoryType.VTSZ,
                        Item = ili.VTSZ
                    };
                    productCodes.Add(VTSZCode);

                    invlineNAV.productCodes = productCodes.ToArray();

                    invlineNAV.lineDescription = ili.LineDescription;
                    invlineNAV.quantity = ili.Quantity;

                    if (!string.IsNullOrWhiteSpace(ili.UnitOfMeasure))
                    {
                        invlineNAV.lineExpressionIndicator = true;
                        UnitOfMeasureType um;

                        if (Enum.TryParse<UnitOfMeasureType>(ili.UnitOfMeasure, out um))
                        {
                            invlineNAV.unitOfMeasure = um;
                            invlineNAV.unitOfMeasureOwn = null;
                        }
                        else
                        {
                            invlineNAV.unitOfMeasure = UnitOfMeasureType.OWN;
                            invlineNAV.unitOfMeasureOwn = ili.UnitOfMeasure;
                        }
                    }
                    else
                    {
                        invlineNAV.lineExpressionIndicator = false;
                        invlineNAV.unitOfMeasure = UnitOfMeasureType.OWN;
                        invlineNAV.unitOfMeasureOwn = bbxBEConsts.DEF_NOTFILLED;
                    }

                    invlineNAV.unitPrice = ili.UnitPrice;
                    invlineNAV.unitPriceHUF = ili.UnitPriceHUF;

                    var lineAmountsNormalNAV = new LineAmountsNormalType();
                    invlineNAV.Item = lineAmountsNormalNAV;

                    lineAmountsNormalNAV.lineNetAmountData.lineNetAmount = ili.LineNetAmount;
                    lineAmountsNormalNAV.lineNetAmountData.lineNetAmountHUF = ili.LineNetAmountHUF;

                    lineAmountsNormalNAV.lineVatData.lineVatAmount = ili.LineVatAmount;
                    lineAmountsNormalNAV.lineVatData.lineVatAmountHUF = ili.LineVatAmountHUF;

                    lineAmountsNormalNAV.lineGrossAmountData.lineGrossAmountNormal = ili.LineGrossAmountNormal;
                    lineAmountsNormalNAV.lineGrossAmountData.lineGrossAmountNormalHUF = ili.LineGrossAmountNormalHUF;

                    //Áfa
                    //
                    lineAmountsNormalNAV.lineVatRate = getVatRateNAV(ili.VatRate);


                    //invlineNAV.lineDiscountData               //BBX: A kedvezmény tételsorként lesz elküldve
                    //invlineNAV.intermediatedService           //BBX: nem kezeljük

                    //invlineNAV.newTransportMean               //BBX: nem kezeljük
                    //invlineNAV.depositIndicator               //BBX: betétdíjat kezelni kellene ?
                    //invlineNAV.marginSchemeIndicator          //BBX: nem kezeljük
                    //invlineNAV.ekaerIds                       //BBX: nem kezeljük

                    //invlineNAV.GPCExcise                      //BBX: nem kezeljük
                    //invlineNAV.netaDeclaration                //BBX: nem kezeljük

                    //Termékdíj
                    //
                    if (!string.IsNullOrWhiteSpace(ili.TakeoverReason))
                    {

                        /* egyelőre csak az átvállalást küldjük */
                        invlineNAV.obligatedForProductFee = true;

                        invlineNAV.productFeeClause = new ProductFeeClauseType();
                        var productFeeTakeoverDataNAV = new ProductFeeTakeoverDataType(
                                                p_takeoverAmountSpecified: true);
                        invlineNAV.productFeeClause.Item = productFeeTakeoverDataNAV;

                        productFeeTakeoverDataNAV.takeoverReason = Enum.Parse<TakeoverType>(ili.TakeoverReason);
                        productFeeTakeoverDataNAV.takeoverAmount = ili.TakeoverAmount;


                        /* ITT vannank a KT kódok, soon....
                        invlineNAV.lineProductFeeContent = new ProductFeeDataType();
                        invlineNAV.lineProductFeeContent.productFeeCode = new ProductCodeType();
                        invlineNAV.lineProductFeeContent.productFeeCode.productCodeCategory = enproductCodeCategory.KT.ToString();
                        invlineNAV.lineProductFeeContent.productFeeCode.productCodeValue = "??KT kód??";      //Ezt törzsben meg kellene adni
                        invlineNAV.lineProductFeeContent.productFeeQuantity
                        liinvlineNAVne.lineProductFeeContent.productFeeMeasuringUnit
                        invlineNAV.lineProductFeeContent.productFeeRate
                        invlineNAV.lineProductFeeContent.productFeeAmount
                        */

                    }


                }
                //Felár/engedmény
                /*

                Ha az árengedményt nem a tételsorhoz közvetlenül kapcsolódóan, hanem a számla végösszegéből,
                százalékosan vagy fix összegben adja az eladó, az árengedményt az adatszolgáltatásban külön tételként
                szükséges szerepeltetni, nem pedig a lineDiscountData elemben. Ha a számla több, különböző
                áfamérték alá tartozó tételt tartalmaz, akkor szükséges a végösszegből adott kedvezmény megbontása
                a különböző adómértékek között, így az ilyen árengedményt több tételként szükséges szerepeltetni

                */

                foreach (var disountedVRGrp in invoice.InvoiceLines.Where(w => w.LineDiscountPercent != 0)
                                                                    .OrderBy(o => o.VatRateID)
                                                                    .GroupBy(g => g.VatRateID).ToList())
                {

                    var discountValue = Math.Round(disountedVRGrp.Sum(s => (-s.LineDiscountPercent / 100) * s.LineNetAmount), 1);
                    var discountValueHUF = Math.Round(disountedVRGrp.Sum(s => (-s.LineDiscountPercent / 100) * s.LineNetAmountHUF), 1);

                    var discountLineNAV = new LineType(
                             p_lineNatureIndicatorSpecified: false,
                             p_quantitySpecified: true,
                             p_unitOfMeasureSpecified: true,
                             p_unitPriceSpecified: true,
                             p_unitPriceHUFSpecified: true,
                             p_intermediatedServiceSpecified: false,
                             p_depositIndicatorSpecified: false,
                             p_obligatedForProductFeeSpecified: false,
                             p_GPCExciseSpecified: false,
                             p_netaDeclarationSpecified: false);

                    invoiceLinesNAV.Add(discountLineNAV);
                    discountLineNAV.lineNumber = (invoiceLinesNAV.Count).ToString();


                    discountLineNAV.lineDescription = discountValue > 0 ? bbxBEConsts.DEF_CHARGE : bbxBEConsts.DEF_DISCOUNT;
                    discountLineNAV.quantity = 1;
                    discountLineNAV.unitOfMeasure = UnitOfMeasureType.PIECE;
                    discountLineNAV.unitPrice = discountValue;
                    discountLineNAV.unitPriceHUF = discountValueHUF;

                    var vatRateDiscount = disountedVRGrp.FirstOrDefault().VatRate;
                    if (vatRateDiscount == null)
                    {
                        throw new Exception(string.Format(bbxBEConsts.ERR_NAVXML_VATRATEMISSING, invoice.InvoiceNumber));
                    }


                    var discountLineAmountsNormalNAV = new LineAmountsNormalType();
                    discountLineNAV.Item = discountLineAmountsNormalNAV;


                    discountLineAmountsNormalNAV.lineNetAmountData.lineNetAmount = discountLineNAV.unitPrice * discountLineNAV.quantity;
                    discountLineAmountsNormalNAV.lineNetAmountData.lineNetAmountHUF = discountLineNAV.unitPriceHUF * discountLineNAV.quantity;

                    discountLineAmountsNormalNAV.lineVatData.lineVatAmount = Math.Round(vatRateDiscount.VatPercentage / 100 * discountLineAmountsNormalNAV.lineNetAmountData.lineNetAmount, 0);
                    discountLineAmountsNormalNAV.lineVatData.lineVatAmountHUF = Math.Round(vatRateDiscount.VatPercentage / 100 * discountLineAmountsNormalNAV.lineNetAmountData.lineNetAmountHUF, 0); ;

                    discountLineAmountsNormalNAV.lineGrossAmountData.lineGrossAmountNormal = discountLineAmountsNormalNAV.lineNetAmountData.lineNetAmount + discountLineAmountsNormalNAV.lineVatData.lineVatAmount;
                    discountLineAmountsNormalNAV.lineGrossAmountData.lineGrossAmountNormalHUF = discountLineAmountsNormalNAV.lineNetAmountData.lineNetAmountHUF + discountLineAmountsNormalNAV.lineVatData.lineVatAmountHUF;

                    //Áfa
                    discountLineAmountsNormalNAV.lineVatRate = getVatRateNAV(vatRateDiscount);

                    //Módosító szála
                    if (invoice.InvoiceCorrection)
                    {
                        discountLineNAV.lineModificationReference = new LineModificationReferenceType();

                        //A javítószámlán lévő tétellel korrigáljuk az eredetit
                        discountLineNAV.lineModificationReference.lineOperation = LineOperationType.CREATE;

                        //API lerírás: a lineNumberReference(a számla és összes módosításaiban) sorfolytonosan új tételsorszámra mutat és lineOperation értéke „CREATE”.
                        //
                        discountLineNAV.lineModificationReference.lineNumberReference = discountLineNAV.lineNumber;
                    }
                }
                ///////////////////////
                // Ö S S Z E S í T Ő //
                ///////////////////////

                var invoiceSummaryNAV = new SummaryType();
                invoiceNAV.invoiceSummary = invoiceSummaryNAV;

                var invoiceSummaryNormalNAV = new SummaryNormalType();
                invoiceSummaryNormalNAV.summaryByVatRate = invoice.SummaryByVatRates.Select(x =>
                    new SummaryByVatRateType()
                    {
                        vatRate = getVatRateNAV(x.VatRate),
                        vatRateNetData = new VatRateNetDataType()
                        {
                            vatRateNetAmount = x.VatRateNetAmount,
                            vatRateNetAmountHUF = x.VatRateNetAmountHUF
                        },

                        vatRateVatData = new VatRateVatDataType()
                        {
                            vatRateVatAmount = x.VatRateVatAmount,
                            vatRateVatAmountHUF = x.VatRateVatAmountHUF
                        },

                        vatRateGrossData = new VatRateGrossDataType()
                        {
                            vatRateGrossAmount = x.VatRateGrossAmount,
                            vatRateGrossAmountHUF = x.VatRateGrossAmountHUF
                        }
                    }
                    ).ToArray();


                invoiceSummaryNAV.Items = new SummaryNormalType[1];
                invoiceSummaryNAV.Items[0] = invoiceSummaryNormalNAV;

                invoiceSummaryNormalNAV.invoiceNetAmount = invoice.InvoiceNetAmount;
                invoiceSummaryNormalNAV.invoiceNetAmountHUF = invoice.InvoiceNetAmountHUF;

                invoiceSummaryNormalNAV.invoiceVatAmount = invoice.InvoiceVatAmount;
                invoiceSummaryNormalNAV.invoiceVatAmountHUF = invoice.InvoiceVatAmountHUF;

                invoiceNAV.invoiceSummary.summaryGrossData.invoiceGrossAmount = invoice.InvoiceGrossAmount;
                invoiceNAV.invoiceSummary.summaryGrossData.invoiceGrossAmountHUF = invoice.InvoiceGrossAmountHUF;



                invoiceNAV.invoiceLines = new LinesType();
                invoiceNAV.invoiceLines.mergedItemIndicator = false;           //BBX: A számla NEM tartlamaz összevont adattartalmú tétel(eke)t !
                invoiceNAV.invoiceLines.line = invoiceLinesNAV.ToArray();

                return invoiceDataNAV;

            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        private static VatRateType getVatRateNAV(VatRate vatRate)
        {
            var vatRateNAV = new VatRateType();
            if (!vatRate.VatDomesticReverseCharge)
            {
                if (vatRate.VatPercentage != 0)
                {
                    //normál áfa
                    vatRateNAV.ItemElementName = ItemChoiceType2.vatPercentage;
                    vatRateNAV.Item = vatRate.VatPercentage;
                }
                else
                {
                    //Áfamentesség
                    if (!string.IsNullOrWhiteSpace(vatRate.VatExemptionCase))
                    {
                        //Tárgyi áfamentesség
                        vatRateNAV.ItemElementName = ItemChoiceType2.vatExemption;
                        var vatReason = new DetailedReasonType();
                        vatReason.@case = vatRate.VatExemptionCase;
                        vatReason.reason = vatRate.VatRateCode;
                        vatRateNAV.Item = vatReason;
                    }
                    else
                    if (!string.IsNullOrWhiteSpace(vatRate.VatOutOfScopeCase))
                    {
                        //alanyi adómentes
                        vatRateNAV.ItemElementName = ItemChoiceType2.vatOutOfScope;
                        var vatReason = new DetailedReasonType();
                        vatReason.@case = vatRate.VatOutOfScopeCase;
                        vatReason.reason = vatRate.VatOutOfScopeReason;
                        vatRateNAV.Item = vatReason;
                    }
                }
            }
            else
            {
                //fordított adózás
                vatRateNAV.ItemElementName = ItemChoiceType2.vatDomesticReverseCharge;
                vatRateNAV.Item = true;
            }
            return vatRateNAV;
        }

    }
}
