using AutoMapper;
using bbxBE.Application.Commands.cmdCounter;
using bbxBE.Application.Commands.cmdCustomer;
using bbxBE.Application.Commands.cmdOrigin;
using bbxBE.Application.Commands.cmdProduct;
using bbxBE.Application.Commands.cmdProductGroup;
using bbxBE.Application.Commands.cmdUSR_USER;
using bbxBE.Application.Commands.cmdWarehouse;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdCounter;
using bxBE.Application.Commands.cmdCustomer;
using bxBE.Application.Commands.cmdInvoice;
using bxBE.Application.Commands.cmdOrigin;
using bxBE.Application.Commands.cmdProduct;
using bxBE.Application.Commands.cmdProductGroup;
using bxBE.Application.Commands.cmdWarehouse;
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

            CreateMap<CreateCustomerCommand, Customer>()
                .ForMember(dst => dst.TaxpayerId, 
                    opt => opt.MapFrom(src => !src.CountryCode.Equals("HU") ? 
                    src.TaxpayerNumber + "-" + src.VatCode + "-" + src.CountryCode 
                    : "HU" ));
            CreateMap<UpdateCustomerCommand, Customer>();
            CreateMap<DeleteCustomerCommand, Customer>();

            CreateMap<CreateProductGroupCommand, ProductGroup>();
            CreateMap<UpdateProductGroupCommand, ProductGroup>();
            CreateMap<DeleteProductGroupCommand, ProductGroup>();

            CreateMap<CreateOriginCommand, Origin>();
            CreateMap<UpdateOriginCommand, Origin>();
            CreateMap<DeleteOriginCommand, Origin>();

            CreateMap<CreateProductCommand, Product>().ReverseMap();
            CreateMap<UpdateProductCommand, Product>().ReverseMap();
            CreateMap<DeleteProductCommand, Product>();
            CreateMap<CreateProductCommand, UpdateProductCommand>().ReverseMap();

            CreateMap<CreateWarehouseCommand, Warehouse>();
            CreateMap<UpdateWarehouseCommand, Warehouse>();
            CreateMap<DeleteWarehouseCommand, Warehouse>();

            CreateMap<CreateCounterCommand, Counter>();
            CreateMap<UpdateCounterCommand, Counter>();
            CreateMap<DeleteCounterCommand, Counter>();

            CreateMap<CreateInvoiceCommand, Invoice>();
            CreateMap<CreateInvoiceCommand, Invoice>().ReverseMap();

            CreateMap<CreateInvoiceCommand.InvoiceLine, InvoiceLine>();
            CreateMap<CreateInvoiceCommand.InvoiceLine, InvoiceLine>().ReverseMap();
        }
    }
}
