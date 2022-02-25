﻿using AutoMapper;
using bbxBE.Application.Commands.cmdCustomer;
using bbxBE.Application.Commands.cmdProductGroup;
using bbxBE.Application.Commands.cmdUSR_USER;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdCustomer;
using bxBE.Application.Commands.cmdProductGroup;
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

            CreateMap<CreateProductGroupCommand, ProductGroup>();
            CreateMap<UpdateProductGroupCommand, ProductGroup>();
            CreateMap<DeleteProductGroupCommand, ProductGroup>();

        }
    }
}