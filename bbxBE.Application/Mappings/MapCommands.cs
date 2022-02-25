using AutoMapper;
using bbxBE.Application.Commands.cmdUSR_USER;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdCustomer;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Command.Mappings
{
    public class MapCommands : Profile
    {
        public MapCommands()
        {
            CreateMap<CreateUSR_USERCommand, USR_USER>();
            CreateMap<UpdateUSR_USERCommand, USR_USER>();
            CreateMap<DeleteUSR_USERCommand, USR_USER>();

            CreateMap<CreateCustomerCommand, Customer>();
            CreateMap<UpdateCustomerCommand, Customer>();
            CreateMap<DeleteCustomerCommand, Customer>();
        }
    }
}
