﻿using AutoMapper;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using static bbxBE.Common.NAV.NAV_enums;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;

namespace bbxBE.Queries.Mappings
{
    public class MapQueries : Profile
    {
        public MapQueries()
        {
            CreateMap<GetUsersViewModel, Users>().ReverseMap();

            CreateMap<GetCustomerViewModel, Customer>().ReverseMap();
            CreateMap<List<Customer>, List<GetCustomerViewModel>>().ReverseMap();

            CreateMap<Customer, GetCustomerViewModel>()
                   .ForMember(dst => dst.TaxpayerNumber, opt => opt.MapFrom(src => String.Format("{0,7}-{1,1}-{2,2}", src.TaxpayerId, src.VatCode, src.CountyCode)))
                   .ForMember(dst => dst.FullAddress, opt => opt.MapFrom(src => String.Format("{0} {1} {2}", src.PostalCode, src.City, src.AdditionalAddressDetail).Trim()))
                   .ForMember(dst => dst.CountryCodeX, opt => opt.MapFrom(src => CountryCodeResolver(src.CountryCode)))
                   .ForMember(dst => dst.UnitPriceTypeX, opt => opt.MapFrom(src => UnitPriceTypeResolver(src.UnitPriceType)));


            CreateMap<List<ProductGroup>, List<GetProductGroupViewModel>>();
            CreateMap<ProductGroup, GetProductGroupViewModel>();

            CreateMap<List<Origin>, List<GetOriginViewModel>>();
            CreateMap<Origin, GetOriginViewModel>();


            CreateMap<List<Warehouse>, List<GetWarehouseViewModel>>();
            CreateMap<Warehouse, GetWarehouseViewModel>();


            CreateMap<List<Product>, List<GetProductViewModel>>();

            CreateMap<Product, GetProductViewModel>()
             .ForMember(dst => dst.ProductGroupCode, opt => opt.MapFrom(src => src.ProductGroup.ProductGroupCode))
             .ForMember(dst => dst.ProductGroup, opt => opt.MapFrom(src => src.ProductGroup.ProductGroupCode + "-" + src.ProductGroup.ProductGroupDescription))
             .ForMember(dst => dst.UnitOfMeasureX, opt => opt.MapFrom(src => enUnitOfMeasureNameResolver(src.UnitOfMeasure)))
             .ForMember(dst => dst.Origin, opt => opt.MapFrom(src => (src.Origin != null ? src.Origin.OriginCode + "-" + src.Origin.OriginDescription : "")))
             .ForMember(dst => dst.VatRateCode, opt => opt.MapFrom(src => src.VatRate.VatRateCode))
             .ForMember(dst => dst.VatPercentage, opt => opt.MapFrom(src => src.VatRate.VatPercentage))
             .ForMember(dst => dst.ProductCode, opt => opt.MapFrom(src => src.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue))
             .ForMember(dst => dst.VTSZ, opt => opt.MapFrom(src => src.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString()).ProductCodeValue))
             .ForMember(dst => dst.EAN, opt => opt.MapFrom(src => src.ProductCodes.Any(w => w.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString()) ?
                                                    src.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString()).ProductCodeValue
                                                   : ""));

            CreateMap<Counter, GetCounterViewModel>()
             .ForMember(dst => dst.Warehouse, opt => opt.MapFrom(src => src.Warehouse.WarehouseCode + "-" + src.Warehouse.WarehouseDescription));

            CreateMap<VatRate, GetVatRateViewModel>()
              .ForMember(dst => dst.VatRateDescription, opt => opt.MapFrom(src => src.VatRateCode + " (áfa%:" + (src.VatPercentage * 100).ToString("0.##") + ")"));

            CreateMap<Invoice, GetInvoiceViewModel>()
             .ForMember(dst => dst.Warehouse, opt => opt.MapFrom(src => src.Warehouse.WarehouseCode + "-" + src.Warehouse.WarehouseDescription))
             .ForMember(dst => dst.SupplierName, opt => opt.MapFrom(src => src.Supplier.CustomerName))
             .ForMember(dst => dst.SupplierBankAccountNumber, opt => opt.MapFrom(src => src.Supplier.CustomerBankAccountNumber))
             .ForMember(dst => dst.SupplierTaxpayerNumber, opt => opt.MapFrom(src => String.Format("{0,7}-{1,1}-{2,2}", src.Supplier.TaxpayerId, src.Supplier.VatCode, src.Supplier.CountyCode)))
             .ForMember(dst => dst.SupplierCountryCode, opt => opt.MapFrom(src => src.Supplier.CountryCode))
             .ForMember(dst => dst.SupplierPostalCode, opt => opt.MapFrom(src => src.Supplier.PostalCode))
             .ForMember(dst => dst.SupplierCity, opt => opt.MapFrom(src => src.Supplier.City))
             .ForMember(dst => dst.SupplierAdditionalAddressDetail, opt => opt.MapFrom(src => src.Supplier.AdditionalAddressDetail))
             .ForMember(dst => dst.SupplierThirdStateTaxId, opt => opt.MapFrom(src => src.Supplier.ThirdStateTaxId))
             .ForMember(dst => dst.SupplierComment, opt => opt.MapFrom(src => src.Supplier.Comment))

             .ForMember(dst => dst.CustomerName, opt => opt.MapFrom(src => src.Customer.CustomerName))
             .ForMember(dst => dst.CustomerBankAccountNumber, opt => opt.MapFrom(src => src.Customer.CustomerBankAccountNumber))
             .ForMember(dst => dst.CustomerTaxpayerNumber, opt => opt.MapFrom(src => String.Format("{0,7}-{1,1}-{2,2}", src.Customer.TaxpayerId, src.Customer.VatCode, src.Customer.CountyCode)))
             .ForMember(dst => dst.CustomerCountryCode, opt => opt.MapFrom(src => src.Customer.CountryCode))
             .ForMember(dst => dst.CustomerPostalCode, opt => opt.MapFrom(src => src.Customer.PostalCode))
             .ForMember(dst => dst.CustomerCity, opt => opt.MapFrom(src => src.Customer.City))
             .ForMember(dst => dst.CustomerAdditionalAddressDetail, opt => opt.MapFrom(src => src.Customer.AdditionalAddressDetail))
             .ForMember(dst => dst.CustomerThirdStateTaxId, opt => opt.MapFrom(src => src.Customer.ThirdStateTaxId))
             .ForMember(dst => dst.CustomerComment, opt => opt.MapFrom(src => src.Customer.Comment))

             .ForMember(dst => dst.PaymentMethodX, opt => opt.MapFrom(src => PaymentMethodNameResolver(src.PaymentMethod)))

             .ForMember(dst => dst.Notice, opt => opt.MapFrom(src => (src.AdditionalInvoiceData != null && src.AdditionalInvoiceData.Any(i => i.DataName == bbxBEConsts.DEF_NOTICE) ?
                                    src.AdditionalInvoiceData.Single(i => i.DataName == bbxBEConsts.DEF_NOTICE).DataValue : "")))
              .ForMember(dst => dst.PriceReview, opt => opt.MapFrom(src => src.InvoiceLines.Any( il=> il.PriceReview.HasValue && il.PriceReview.Value)))
             ;

            CreateMap<InvoiceLine, GetInvoiceViewModel.InvoiceLine>()
             .ForMember(dst => dst.UnitOfMeasureX, opt => opt.MapFrom(src => enUnitOfMeasureNameResolver(src.UnitOfMeasure)))
             .ForMember(dst => dst.VatRateCode, opt => opt.MapFrom(src => src.VatRate.VatRateCode));

            CreateMap<SummaryByVatRate, GetInvoiceViewModel.SummaryByVatRate>()
             .ForMember(dst => dst.VatRateCode, opt => opt.MapFrom(src => src.VatRate.VatRateCode));

            /********/
            CreateMap<Invoice, GetAggregateInvoiceViewModel>()
             .ForMember(dst => dst.Warehouse, opt => opt.MapFrom(src => src.Warehouse.WarehouseCode + "-" + src.Warehouse.WarehouseDescription))
             .ForMember(dst => dst.SupplierName, opt => opt.MapFrom(src => src.Supplier.CustomerName))
             .ForMember(dst => dst.SupplierBankAccountNumber, opt => opt.MapFrom(src => src.Supplier.CustomerBankAccountNumber))
             .ForMember(dst => dst.SupplierTaxpayerNumber, opt => opt.MapFrom(src => String.Format("{0,7}-{1,1}-{2,2}", src.Supplier.TaxpayerId, src.Supplier.VatCode, src.Supplier.CountyCode)))
             .ForMember(dst => dst.SupplierCountryCode, opt => opt.MapFrom(src => src.Supplier.CountryCode))
             .ForMember(dst => dst.SupplierPostalCode, opt => opt.MapFrom(src => src.Supplier.PostalCode))
             .ForMember(dst => dst.SupplierCity, opt => opt.MapFrom(src => src.Supplier.City))
             .ForMember(dst => dst.SupplierAdditionalAddressDetail, opt => opt.MapFrom(src => src.Supplier.AdditionalAddressDetail))
             .ForMember(dst => dst.SupplierThirdStateTaxId, opt => opt.MapFrom(src => src.Supplier.ThirdStateTaxId))
             .ForMember(dst => dst.SupplierComment, opt => opt.MapFrom(src => src.Supplier.Comment))

             .ForMember(dst => dst.CustomerName, opt => opt.MapFrom(src => src.Customer.CustomerName))
             .ForMember(dst => dst.CustomerBankAccountNumber, opt => opt.MapFrom(src => src.Customer.CustomerBankAccountNumber))
             .ForMember(dst => dst.CustomerTaxpayerNumber, opt => opt.MapFrom(src => String.Format("{0,7}-{1,1}-{2,2}", src.Customer.TaxpayerId, src.Customer.VatCode, src.Customer.CountyCode)))
             .ForMember(dst => dst.CustomerCountryCode, opt => opt.MapFrom(src => src.Customer.CountryCode))
             .ForMember(dst => dst.CustomerPostalCode, opt => opt.MapFrom(src => src.Customer.PostalCode))
             .ForMember(dst => dst.CustomerCity, opt => opt.MapFrom(src => src.Customer.City))
             .ForMember(dst => dst.CustomerAdditionalAddressDetail, opt => opt.MapFrom(src => src.Customer.AdditionalAddressDetail))
             .ForMember(dst => dst.CustomerThirdStateTaxId, opt => opt.MapFrom(src => src.Customer.ThirdStateTaxId))
             .ForMember(dst => dst.CustomerComment, opt => opt.MapFrom(src => src.Customer.Comment))

             .ForMember(dst => dst.PaymentMethodX, opt => opt.MapFrom(src => PaymentMethodNameResolver(src.PaymentMethod)))

             .ForMember(dst => dst.Notice, opt => opt.MapFrom(src => (src.AdditionalInvoiceData != null && src.AdditionalInvoiceData.Any(i => i.DataName == bbxBEConsts.DEF_NOTICE) ?
                                    src.AdditionalInvoiceData.Single(i => i.DataName == bbxBEConsts.DEF_NOTICE).DataValue : "")))
              .ForMember(dst => dst.PriceReview, opt => opt.MapFrom(src => src.InvoiceLines.Any(il => il.PriceReview.HasValue && il.PriceReview.Value)))
             ;

            /*
            CreateMap<InvoiceLine, GetAggregateInvoiceViewModel.DeliveryNote>()
             .ForMember(dst => dst.UnitOfMeasureX, opt => opt.MapFrom(src => enUnitOfMeasureNameResolver(src.UnitOfMeasure)))
             .ForMember(dst => dst.VatRateCode, opt => opt.MapFrom(src => src.VatRate.VatRateCode));
            */

            CreateMap<InvoiceLine, GetAggregateInvoiceDeliveryNoteViewModel.InvoiceLine>()
             .ForMember(dst => dst.UnitOfMeasureX, opt => opt.MapFrom(src => enUnitOfMeasureNameResolver(src.UnitOfMeasure)))
             .ForMember(dst => dst.VatRateCode, opt => opt.MapFrom(src => src.VatRate.VatRateCode));

            CreateMap<SummaryByVatRate, GetAggregateInvoiceViewModel.SummaryByVatRate>()
             .ForMember(dst => dst.VatRateCode, opt => opt.MapFrom(src => src.VatRate.VatRateCode));


            /*********/


            CreateMap<Offer, GetOfferViewModel>()
             .ForMember(dst => dst.OfferNumberX, opt => opt.MapFrom((src, dest) =>
             {
                 return src.OfferVersion > 0 ? src.OfferNumber + "/" + src.OfferVersion : src.OfferNumber;
             }))
             .ForMember(dst => dst.CustomerName, opt => opt.MapFrom(src => src.Customer.CustomerName))
             .ForMember(dst => dst.CustomerBankAccountNumber, opt => opt.MapFrom(src => src.Customer.CustomerBankAccountNumber))
             .ForMember(dst => dst.CustomerTaxpayerNumber, opt => opt.MapFrom(src => String.Format("{0,7}-{1,1}-{2,2}", src.Customer.TaxpayerId, src.Customer.VatCode, src.Customer.CountyCode)))
             .ForMember(dst => dst.CustomerCountryCode, opt => opt.MapFrom(src => src.Customer.CountryCode))
             .ForMember(dst => dst.CustomerPostalCode, opt => opt.MapFrom(src => src.Customer.PostalCode))
             .ForMember(dst => dst.CustomerCity, opt => opt.MapFrom(src => src.Customer.City))
             .ForMember(dst => dst.CustomerAdditionalAddressDetail, opt => opt.MapFrom(src => src.Customer.AdditionalAddressDetail))
             .ForMember(dst => dst.CustomerComment, opt => opt.MapFrom(src => src.Customer.Comment))
             .ForMember(dst => dst.CurrencyCodeX, opt => opt.MapFrom(src => CurrencyCodeResolver(src.CurrencyCode)));

            CreateMap<OfferLine, GetOfferViewModel.OfferLine>()
             .ForMember(dst => dst.UnitOfMeasureX, opt => opt.MapFrom(src => enUnitOfMeasureNameResolver( src.UnitOfMeasure)))
             .ForMember(dst => dst.VatRateCode, opt => opt.MapFrom(src => src.VatRate.VatRateCode));

            CreateMap<Stock, GetStockViewModel>()
             .ForMember(dst => dst.Warehouse, opt => opt.MapFrom(src => src.Warehouse.WarehouseCode + "-" + src.Warehouse.WarehouseDescription))
             .ForMember(dst => dst.ProductCode, opt => opt.MapFrom(src => src.Product.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue))
             .ForMember(dst => dst.Product, opt => opt.MapFrom(src => src.Product.Description))
             .ForMember(dst => dst.Location, opt => opt.MapFrom(src => (src.Location != null ? src.Location.LocationCode + "-" + src.Location.LocationDescription : "")));

            CreateMap<StockCard, GetStockCardViewModel>()
             .ForMember(dst => dst.ScTypeX, opt => opt.MapFrom(src => enStockCardTypeNameResolver(src.ScType)))
             .ForMember(dst => dst.Warehouse, opt => opt.MapFrom(src => src.Warehouse.WarehouseCode + "-" + src.Warehouse.WarehouseDescription))
             .ForMember(dst => dst.ProductCode, opt => opt.MapFrom(src => src.Product.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue))
             .ForMember(dst => dst.Product, opt => opt.MapFrom(src => src.Product.Description))
             .ForMember(dst => dst.Customer, opt => opt.MapFrom(src => src.Customer.CustomerName));

            CreateMap<InvCtrlPeriod, GetInvCtrlPeriodViewModel>()
             .ForMember(dst => dst.Warehouse, opt => opt.MapFrom(src => src.Warehouse.WarehouseCode + "-" + src.Warehouse.WarehouseDescription));

            CreateMap<InvCtrl, GetInvCtrlViewModel>()
             .ForMember(dst => dst.InvCtrlTypeX, opt => opt.MapFrom(src => InvCtrlTypeNameResolver(src.InvCtrlType)))
             .ForMember(dst => dst.Warehouse, opt => opt.MapFrom(src => src.Warehouse.WarehouseCode + "-" + src.Warehouse.WarehouseDescription))
             .ForMember(dst => dst.ProductCode, opt => opt.MapFrom(src => src.Product.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue))
             .ForMember(dst => dst.Product, opt => opt.MapFrom(src => src.Product.Description));

            CreateMap<CustDiscount, GetCustDiscountViewModel>()
            .ForMember(dst => dst.Customer, opt => opt.MapFrom(src => src.Customer.CustomerName))
            .ForMember(dst => dst.ProductGroupCode, opt => opt.MapFrom(src => src.ProductGroup.ProductGroupCode))
            .ForMember(dst => dst.ProductGroup, opt => opt.MapFrom(src => src.ProductGroup.ProductGroupCode + "-" +src.ProductGroup.ProductGroupDescription));

            CreateMap<InvoiceLine, GetPendigDeliveryNotesSummaryModel>();   //egyelőre a lekérdezés direktbe tölti fel, nincs mappelés
            CreateMap<InvoiceLine, GetPendigDeliveryNotesModel>();   //egyelőre a lekérdezés direktbe tölti fel, nincs mappelés

            CreateMap<InvoiceLine, GetPendigDeliveryNotesItemModel>()
            .ForMember(dst => dst.WarehouseID, opt => opt.MapFrom(src => src.Invoice.WarehouseID))
            .ForMember(dst => dst.CustomerID, opt => opt.MapFrom(src => src.Invoice.Incoming ? src.Invoice.Supplier.ID : src.Invoice.Customer.ID))
            .ForMember(dst => dst.Customer, opt => opt.MapFrom(src => src.Invoice.Incoming ? src.Invoice.Supplier.CustomerName : src.Invoice.Customer.CustomerName))
            .ForMember(dst => dst.InvoiceNumber, opt => opt.MapFrom(src => src.Invoice.InvoiceNumber))
            .ForMember(dst => dst.RelDeliveryNoteInvoiceLineID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dst => dst.RelDeliveryDate, opt => opt.MapFrom(src => src.Invoice.InvoiceDeliveryDate))
            .ForMember(dst => dst.Quantity, opt => opt.MapFrom(src => src.PendingDNQuantity))
            .ForMember(dst => dst.WorkNumber, opt => opt.MapFrom(src => src.Invoice.WorkNumber))
            .ForMember(dst => dst.CurrencyCode, opt => opt.MapFrom(src => src.Invoice.CurrencyCode))
            .ForMember(dst => dst.ExchangeRate, opt => opt.MapFrom(src => src.Invoice.ExchangeRate))
            .ForMember(dst => dst.PriceReview, opt => opt.MapFrom(src => src.PriceReview))
            .ForMember(dst => dst.InvoiceDiscountPercent, opt => opt.MapFrom(src => src.Invoice.InvoiceDiscountPercent))
            .ForMember(dst => dst.UnitPriceDiscounted, opt => opt.MapFrom(src => Math.Round(src.UnitPrice * (1 - src.Invoice.InvoiceDiscountPercent / 100), 1)))
            .ForMember(dst => dst.LineNetAmount, opt => opt.MapFrom(src => Math.Round(src.PendingDNQuantity * src.UnitPrice, 1)))
            .ForMember(dst => dst.LineNetAmountDiscounted, opt => opt.MapFrom(src => Math.Round(src.PendingDNQuantity * src.UnitPrice * (1 - src.Invoice.InvoiceDiscountPercent / 100), 1)))
            .ForMember(dst => dst.UnitOfMeasureX, opt => opt.MapFrom(src => enUnitOfMeasureNameResolver(src.UnitOfMeasure)))
            .ForMember(dst => dst.VatRateCode, opt => opt.MapFrom(src => src.VatRate.VatRateCode));

            CreateMap<List<Zip>, List<GetZipViewModel>>();
            CreateMap<Zip, GetZipViewModel>();

            CreateMap<List<Location>, List<GetLocationViewModel>>();
            CreateMap<Location, GetLocationViewModel>();


        }

        private static string enStockCardTypeNameResolver(string ScType)
        {
            if (!string.IsNullOrWhiteSpace(ScType))
            {
                var _scType = (enStockCardType)Enum.Parse(typeof(enStockCardType), ScType);
                return Common.Utils.GetEnumDescription(_scType);
            }
            return "";
        }

        public static string enUnitOfMeasureNameResolver(string UnitOfMeasure)
        {
            if (!string.IsNullOrWhiteSpace(UnitOfMeasure))
            {
                var _unitOfMeasure = (enUnitOfMeasure)Enum.Parse(typeof(enUnitOfMeasure), UnitOfMeasure);
                return Common.Utils.GetEnumDescription(_unitOfMeasure);
            }
            return "";
        }

        private static string PaymentMethodNameResolver(string PaymentMethod)
        {
            if (!string.IsNullOrWhiteSpace(PaymentMethod))
            {
                var pm = (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType), PaymentMethod);
                return Common.Utils.GetEnumDescription(pm);
            }
            return "";
        }

        private static string InvCtrlTypeNameResolver(string InvCtrlType)
        {
            if (!string.IsNullOrWhiteSpace(InvCtrlType))
            {
                var _invCtrlType = (enInvCtrlType)Enum.Parse(typeof(enInvCtrlType), InvCtrlType);
                return Common.Utils.GetEnumDescription(_invCtrlType);
            }
            return "";
        }


        private static string CurrencyCodeResolver(string CurrencyCode)
        {
            if (!string.IsNullOrWhiteSpace(CurrencyCode))
            {
                var _currencyCode = (enCurrencyCodes)Enum.Parse(typeof(enCurrencyCodes), CurrencyCode);
                return Common.Utils.GetEnumDescription(_currencyCode);
            }
            return "";
        }

        private static string CountryCodeResolver(string countryCode)
        {
            if (!string.IsNullOrWhiteSpace(countryCode))
            {
                var _countryCode = (enCountries)Enum.Parse(typeof(enCountries), countryCode);
                return Common.Utils.GetEnumDescription(_countryCode);
            }
            return "";
        }
        private static string UnitPriceTypeResolver(string unitPriceType)
        {
            if (!string.IsNullOrWhiteSpace(unitPriceType))
            {
                var _unitPriceType = (enUnitPriceType)Enum.Parse(typeof(enUnitPriceType), unitPriceType);
                return Common.Utils.GetEnumDescription(_unitPriceType);
            }
            return "";
        }


    }
}
