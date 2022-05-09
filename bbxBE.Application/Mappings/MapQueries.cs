using AutoMapper;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using static bbxBE.Common.NAV.NAV_enums;
using bbxBE.Application.Consts;

namespace bbxBE.Queries.Mappings
{
    public class MapQueries : Profile
    {
        public MapQueries()
        {
            CreateMap<GetUSR_USERViewModel, USR_USER>().ReverseMap();

            CreateMap<GetCustomerViewModel, Customer>().ReverseMap();
            CreateMap<List<Customer>, List<GetCustomerViewModel>>().ReverseMap();

            CreateMap<Customer, GetCustomerViewModel>()
                   .ForMember(dst => dst.TaxpayerNumber, opt => opt.MapFrom(src => String.Format("{0,7}-{1,1}-{2,2}", src.TaxpayerId, src.VatCode, src.CountyCode)))
                   .ForMember(dst => dst.FullAddress, opt => opt.MapFrom(src => String.Format("{0} {1} {2}", src.PostalCode, src.City, src.AdditionalAddressDetail).Trim()));


            CreateMap<List<ProductGroup>, List<GetProductGroupViewModel>>();
            CreateMap<ProductGroup, GetProductGroupViewModel>();

            CreateMap<List<Origin>, List<GetOriginViewModel>>();
            CreateMap<Origin, GetOriginViewModel>();

            CreateMap<List<Product>, List<GetProductViewModel>>();
            CreateMap<Product, GetProductViewModel>();

            CreateMap<List<Warehouse>, List<GetWarehouseViewModel>>();
            CreateMap<Warehouse, GetWarehouseViewModel>();


            CreateMap<Product, GetProductViewModel>()
             .ForMember(dst => dst.ProductGroup, opt => opt.MapFrom(src => src.ProductGroup.ProductGroupCode +"-"+src.ProductGroup.ProductGroupDescription))
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
             .ForMember(dst => dst.SupplierCountryCode, opt => opt.MapFrom(src => src.Supplier.CountyCode))
             .ForMember(dst => dst.SupplierPostalCode, opt => opt.MapFrom(src => src.Supplier.PostalCode))
             .ForMember(dst => dst.SupplierCity, opt => opt.MapFrom(src => src.Supplier.City))
             .ForMember(dst => dst.SupplierAdditionalAddressDetail, opt => opt.MapFrom(src => src.Supplier.AdditionalAddressDetail))
             .ForMember(dst => dst.SupplierComment, opt => opt.MapFrom(src => src.Supplier.Comment))

             .ForMember(dst => dst.CustomerName, opt => opt.MapFrom(src => src.Customer.CustomerName))
             .ForMember(dst => dst.CustomerBankAccountNumber, opt => opt.MapFrom(src => src.Customer.CustomerBankAccountNumber))
             .ForMember(dst => dst.CustomerTaxpayerNumber, opt => opt.MapFrom(src => String.Format("{0,7}-{1,1}-{2,2}", src.Customer.TaxpayerId, src.Customer.VatCode, src.Customer.CountyCode)))
             .ForMember(dst => dst.CustomerCountryCode, opt => opt.MapFrom(src => src.Customer.CountyCode))
             .ForMember(dst => dst.CustomerPostalCode, opt => opt.MapFrom(src => src.Customer.PostalCode))
             .ForMember(dst => dst.CustomerCity, opt => opt.MapFrom(src => src.Customer.City))
             .ForMember(dst => dst.CustomerAdditionalAddressDetail, opt => opt.MapFrom(src => src.Customer.AdditionalAddressDetail))
             .ForMember(dst => dst.CustomerComment, opt => opt.MapFrom(src => src.Customer.Comment))

             .ForMember(dst => dst.Notice, opt => opt.MapFrom(src => (src.AdditionalInvoiceData != null && src.AdditionalInvoiceData.Any(i => i.DataName == bbxBEConsts.DEF_NOTICE) ?
                                    src.AdditionalInvoiceData.Single(i => i.DataName == bbxBEConsts.DEF_NOTICE).DataDescription : "")));


            CreateMap<InvoiceLine, GetInvoiceViewModel.InvoiceLine>();
        }
    }
}
