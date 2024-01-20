using AutoMapper;
using bbxBE.Application.Commands.cmdCounter;
using bbxBE.Application.Commands.cmdCustomer;
using bbxBE.Application.Commands.cmdLocation;
using bbxBE.Application.Commands.cmdOrigin;
using bbxBE.Application.Commands.cmdProduct;
using bbxBE.Application.Commands.cmdProductGroup;
using bbxBE.Application.Commands.cmdUser;
using bbxBE.Application.Commands.cmdWarehouse;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdCounter;
using bxBE.Application.Commands.cmdCustDiscount;
using bxBE.Application.Commands.cmdCustomer;
using bxBE.Application.Commands.cmdInvCtrl;
using bxBE.Application.Commands.cmdInvCtrlPeriod;
using bxBE.Application.Commands.cmdInvoice;
using bxBE.Application.Commands.cmdLocation;
using bxBE.Application.Commands.cmdOffer;
using bxBE.Application.Commands.cmdOrigin;
using bxBE.Application.Commands.cmdProduct;
using bxBE.Application.Commands.cmdProductGroup;
using bxBE.Application.Commands.cmdWarehouse;
using bxBE.Application.Commands.cmdWhsTransfer;

namespace bbxBE.Command.Mappings
{
    public class MapCommands : Profile
    {
        public MapCommands()
        {
            CreateMap<createUserCommand, Users>();
            /*
         .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
         .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
         .ForMember(dst => dst.LoginName, opt => opt.MapFrom(src => src.LoginName))
         .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => src.Comment));
            */

            CreateMap<UpdateUserCommand, Users>();
            /*
         .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
         .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
         .ForMember(dst => dst.LoginName, opt => opt.MapFrom(src => src.LoginName))
         .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => src.Comment));
            */

            CreateMap<DeleteUserCommand, Users>();

            CreateMap<CreateCustomerCommand, Customer>()
                .ForMember(dst => dst.TaxpayerId,
                    opt => opt.MapFrom(src => src.CountryCode.Equals(bbxBEConsts.CNTRY_HU) && src.TaxpayerNumber.Length >= 8 ?
                    src.TaxpayerNumber.Substring(0, 8) : ""))
                .ForMember(dst => dst.VatCode,
                    opt => opt.MapFrom(src => src.CountryCode.Equals(bbxBEConsts.CNTRY_HU) && src.TaxpayerNumber.Length >= 10 ?
                    src.TaxpayerNumber.Substring(9, 1) : ""))
                .ForMember(dst => dst.CountyCode,
                    opt => opt.MapFrom(src => src.CountryCode.Equals(bbxBEConsts.CNTRY_HU) && src.TaxpayerNumber.Length >= 13 ?
                    src.TaxpayerNumber.Substring(11, 2) : ""))
                .ForMember(dst => dst.CustomerVatStatus,
                    opt => opt.MapFrom(src => src.PrivatePerson ? CustomerVatStatusType.PRIVATE_PERSON.ToString() :
                                    string.IsNullOrWhiteSpace(src.CountryCode) || src.CountryCode == bbxBEConsts.CNTRY_HU ?
                                            CustomerVatStatusType.DOMESTIC.ToString()
                                            : CustomerVatStatusType.OTHER.ToString()))
                .ForMember(dst => dst.CountryCode,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.CountryCode) ? bbxBEConsts.CNTRY_HU : src.CountryCode.ToUpper()))
                .ForMember(dst => dst.UnitPriceType,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.UnitPriceType) ? enUnitPriceType.LIST.ToString() : src.UnitPriceType.ToUpper()));


            CreateMap<UpdateCustomerCommand, Customer>()
                .ForMember(dst => dst.TaxpayerId,
                    opt => opt.MapFrom(src => src.CountryCode.Equals(bbxBEConsts.CNTRY_HU) && src.TaxpayerNumber.Length >= 8 ?
                    src.TaxpayerNumber.Substring(0, 8) : ""))
                .ForMember(dst => dst.VatCode,
                    opt => opt.MapFrom(src => src.CountryCode.Equals(bbxBEConsts.CNTRY_HU) && src.TaxpayerNumber.Length >= 10 ?
                    src.TaxpayerNumber.Substring(9, 1) : ""))
                .ForMember(dst => dst.CountyCode,
                    opt => opt.MapFrom(src => src.CountryCode.Equals(bbxBEConsts.CNTRY_HU) && src.TaxpayerNumber.Length >= 13 ?
                    src.TaxpayerNumber.Substring(11, 2) : ""))
                .ForMember(dst => dst.CustomerVatStatus,
                    opt => opt.MapFrom(src => src.PrivatePerson ? CustomerVatStatusType.PRIVATE_PERSON.ToString() :
                                    string.IsNullOrWhiteSpace(src.CountryCode) || src.CountryCode == bbxBEConsts.CNTRY_HU ?
                                            CustomerVatStatusType.DOMESTIC.ToString()
                                            : CustomerVatStatusType.OTHER.ToString()))
                .ForMember(dst => dst.CountryCode,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.CountryCode) ? bbxBEConsts.CNTRY_HU : src.CountryCode.ToUpper()))
                .ForMember(dst => dst.UnitPriceType,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.UnitPriceType) ? enUnitPriceType.LIST.ToString() : src.UnitPriceType.ToUpper()));

            CreateMap<DeleteCustomerCommand, Customer>();

            CreateMap<CreateProductGroupCommand, ProductGroup>();
            CreateMap<UpdateProductGroupCommand, ProductGroup>();
            CreateMap<DeleteProductGroupCommand, ProductGroup>();

            CreateMap<CreateOriginCommand, Origin>();
            CreateMap<UpdateOriginCommand, Origin>();
            CreateMap<DeleteOriginCommand, Origin>();

            CreateMap<CreateProductCommand, Product>();
            CreateMap<UpdateProductCommand, Product>();
            CreateMap<DeleteProductCommand, Product>();
            CreateMap<CreateProductCommand, UpdateProductCommand>().ReverseMap();

            CreateMap<CreateWarehouseCommand, Warehouse>();
            CreateMap<UpdateWarehouseCommand, Warehouse>();
            CreateMap<DeleteWarehouseCommand, Warehouse>();

            CreateMap<CreateCounterCommand, Counter>();
            CreateMap<UpdateCounterCommand, Counter>();
            CreateMap<DeleteCounterCommand, Counter>();

            CreateMap<CreateInvoiceCommand, Invoice>()
               .ForMember(dst => dst.InvoiceIssueDate, opt => opt.MapFrom(src => src.InvoiceIssueDate.Date))
               .ForMember(dst => dst.InvoiceDeliveryDate, opt => opt.MapFrom(src => src.InvoiceDeliveryDate.Date))
               .ForMember(dst => dst.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate.Date));
            ;

            CreateMap<CreateInvoiceCommand.InvoiceLine, InvoiceLine>();

            CreateMap<CreateOfferCommand, Offer>()
               .ForMember(dst => dst.OfferIssueDate, opt => opt.MapFrom(src => src.OfferIssueDate.Date))
               .ForMember(dst => dst.OfferVaidityDate, opt => opt.MapFrom(src => src.OfferVaidityDate.Date));

            CreateMap<CreateOfferCommand.OfferLine, OfferLine>();

            CreateMap<UpdateOfferCommand, Offer>()
               .ForMember(dst => dst.OfferIssueDate, opt => opt.MapFrom(src => src.OfferIssueDate.Date))
               .ForMember(dst => dst.OfferVaidityDate, opt => opt.MapFrom(src => src.OfferVaidityDate.Date));

            CreateMap<UpdateOfferCommand.OfferLine, OfferLine>();

            CreateMap<CreateInvCtrlPeriodCommand, InvCtrlPeriod>()
               .ForMember(dst => dst.DateFrom, opt => opt.MapFrom(src => src.DateFrom.Date))
               .ForMember(dst => dst.DateTo, opt => opt.MapFrom(src => src.DateTo.Date));

            CreateMap<UpdateInvCtrlPeriodCommand, InvCtrlPeriod>()
               .ForMember(dst => dst.DateFrom, opt => opt.MapFrom(src => src.DateFrom.Date))
               .ForMember(dst => dst.DateTo, opt => opt.MapFrom(src => src.DateTo.Date));

            CreateMap<createInvCtrlICPCommand.InvCtrlItem, InvCtrl>()
               .ForMember(dst => dst.InvCtrlDate, opt => opt.MapFrom(src => src.InvCtrlDate.Date));

            CreateMap<createInvCtrlICCCommand.InvCtrlItem, InvCtrl>()
               .ForMember(dst => dst.InvCtrlDate, opt => opt.MapFrom(src => src.InvCtrlDate.Date));
            //    CreateMap<UpdateInvCtrlCommand, InvCtrl>();

            CreateMap<CreateCustDiscountCommand.CustDiscountItem, CustDiscount>();

            CreateMap<CreateLocationCommand, Location>();
            CreateMap<UpdateLocationCommand, Location>();
            CreateMap<DeleteLocationCommand, Location>();

            CreateMap<CreateWhsTransferCommand, WhsTransfer>();
            CreateMap<CreateWhsTransferCommand.WhsTransferLine, WhsTransferLine>();

            CreateMap<UpdateWhsTransferCommand, WhsTransfer>();
            CreateMap<UpdateWhsTransferCommand.WhsTransferLine, WhsTransferLine>();

            CreateMap<CreateInvPaymentCommand.InvPaymentItem, InvPayment>();


        }
    }
}
