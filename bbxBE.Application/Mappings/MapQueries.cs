using AutoMapper;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Queries.Mappings
{
    public class MapQueries : Profile
    {
        public MapQueries()
        {
            CreateMap<GetCustomerViewModel, Customer>().ReverseMap();
            CreateMap<GetUSR_USERViewModel, USR_USER>().ReverseMap();
            CreateMap<List<Customer>, List<GetCustomerViewModel>>().ReverseMap();
            CreateMap<List<Customer>, List<GetCustomerViewModel>>();
            CreateMap<Customer, GetCustomerViewModel>();
        }
    }
}
