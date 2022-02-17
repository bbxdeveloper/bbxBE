using AutoMapper;
using bbxBE.Domain.Entities;
using bbxBE.Queries.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Queries.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<GetCustomerViewModel, Customer>().ReverseMap();
            CreateMap<GetUSR_USERViewModel, USR_USER>().ReverseMap();
            CreateMap<List<Customer>, List<GetCustomerViewModel>>().ReverseMap();
            CreateMap<List<Customer>, List<GetCustomerViewModel>>();
            CreateMap<Customer, GetCustomerViewModel>();
        }
    }
}
