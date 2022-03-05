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
            CreateMap<List<Customer>, List<GetCustomerViewModel>>();
            CreateMap<Customer, GetCustomerViewModel>();

            CreateMap<List<ProductGroup>, List<GetProductGroupViewModel>>();
            CreateMap<ProductGroup, GetProductGroupViewModel>();

            CreateMap<List<Origin>, List<GetOriginViewModel>>();
            CreateMap<Origin, GetOriginViewModel>();

            CreateMap<List<Product>, List<GetProductViewModel>>();
            CreateMap<Product, GetProductViewModel>();

            CreateMap<List<Warehouse>, List<GetWarehouseViewModel>>();
            CreateMap<Product, GetProductViewModel>()
             .ForMember(dst => dst.ProductGroup, opt => opt.MapFrom(src => src.ProductGroup.ProductGroupDescription))
             .ForMember(dst => dst.Origin, opt => opt.MapFrom(src => src.Origin.OriginDescription))
             .ForMember(dst => dst.ProductCode, opt => opt.MapFrom(src => src.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString()).ProductCodeValue))
             .ForMember(dst => dst.VTSZ, opt => opt.MapFrom(src => src.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString()).ProductCodeValue))
             .ForMember(dst => dst.EAN, opt => opt.MapFrom(src => src.ProductCodes.Any(w => w.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString()) ?
                                                    src.ProductCodes.SingleOrDefault(w => w.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString()).ProductCodeValue
                                                   : ""));
        }
    }
}
