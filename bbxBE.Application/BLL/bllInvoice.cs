using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static Invoice CalcInvoiceAmounts(Invoice invoice)
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


        public static InvoiceData GetInvoiceNAVXML(Invoice invoice)
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


                if (invoice.InvoiceCorrection)
                {
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
                            if (um != UnitOfMeasureType.OWN)
                            {
                                invlineNAV.unitOfMeasureOwn = null;
                            }
                            else
                            {
                                invlineNAV.unitOfMeasureOwn = ili.UnitOfMeasure;    //OWN kód esetén tölteni kell a unitOfMeasureOwn-t is!

                            }
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
                    var maxLineNumber = Int32.Parse(invoiceLinesNAV.Max(m => m.lineNumber)) + 1;
                    discountLineNAV.lineNumber = maxLineNumber.ToString();


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
