using AutoMapper;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using static bbxBE.Common.NAV.NAV_enums;

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
             .ForMember(dst => dst.Origin, opt => opt.MapFrom(src => src.Origin.OriginCode + "-" + src.Origin.OriginDescription))
             .ForMember(dst => dst.ProductCode, opt => opt.MapFrom(src => src.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue))
             .ForMember(dst => dst.VTSZ, opt => opt.MapFrom(src => src.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString()).ProductCodeValue))
             .ForMember(dst => dst.EAN, opt => opt.MapFrom(src => src.ProductCodes.Any(w => w.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString()) ?
                                                    src.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString()).ProductCodeValue
                                                   : ""));

            CreateMap<Counter, GetCounterViewModel>()
             .ForMember(dst => dst.Warehouse, opt => opt.MapFrom(src => src.Warehouse.WarehouseCode + "-" + src.Warehouse.WarehouseDescription));

        }
    }
}
