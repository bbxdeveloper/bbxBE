using bbxBE.Application.Behaviours;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;


namespace bbxBE.Application.Queries
{
    public static class QueryServiceRegistration
    {
        public static void AddQueryInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

       
            services.AddScoped<IDataShapeHelper<Users>, DataShapeHelper<Users>>();

            services.AddScoped<IDataShapeHelper<Customer>, DataShapeHelper<Customer>>();
            services.AddScoped<IDataShapeHelper<GetCustomerViewModel>, DataShapeHelper<GetCustomerViewModel>>();


            services.AddScoped<IDataShapeHelper<ProductGroup>, DataShapeHelper<ProductGroup>>();
            services.AddScoped<IDataShapeHelper<GetProductGroupViewModel>, DataShapeHelper<GetProductGroupViewModel>>();

            services.AddScoped<IDataShapeHelper<Origin>, DataShapeHelper<Origin>>();
            services.AddScoped<IDataShapeHelper<GetOriginViewModel>, DataShapeHelper<GetOriginViewModel>>();

            services.AddScoped<IDataShapeHelper<Product>, DataShapeHelper<Product>>();
            services.AddScoped<IDataShapeHelper<GetProductViewModel>, DataShapeHelper<GetProductViewModel>>();

            services.AddScoped<IDataShapeHelper<Warehouse>, DataShapeHelper<Warehouse>>();
            services.AddScoped<IDataShapeHelper<GetWarehouseViewModel>, DataShapeHelper<GetWarehouseViewModel>>();

            services.AddScoped<IDataShapeHelper<Counter>, DataShapeHelper<Counter>>();
            services.AddScoped<IDataShapeHelper<GetCounterViewModel>, DataShapeHelper<GetCounterViewModel>>();

            services.AddScoped<IDataShapeHelper<Invoice>, DataShapeHelper<Invoice>>();
            services.AddScoped<IDataShapeHelper<InvoiceLine>, DataShapeHelper<InvoiceLine>>();

            services.AddScoped<IDataShapeHelper<GetInvoiceViewModel>, DataShapeHelper<GetInvoiceViewModel>>();
            services.AddScoped<IDataShapeHelper<GetAggregateInvoiceViewModel>, DataShapeHelper<GetAggregateInvoiceViewModel>>();
            services.AddScoped<IDataShapeHelper<GetPendigDeliveryNotesSummaryModel>, DataShapeHelper<GetPendigDeliveryNotesSummaryModel>>();
            services.AddScoped<IDataShapeHelper<GetPendigDeliveryNotesModel>, DataShapeHelper<GetPendigDeliveryNotesModel>>();
            
            services.AddScoped<IDataShapeHelper<VatRate>, DataShapeHelper<VatRate>>();
            services.AddScoped<IDataShapeHelper<GetVatRateViewModel>, DataShapeHelper<GetVatRateViewModel>>();

            services.AddScoped<IDataShapeHelper<Offer>, DataShapeHelper<Offer>>();
            services.AddScoped<IDataShapeHelper<GetOfferViewModel>, DataShapeHelper<GetOfferViewModel>>();

            services.AddScoped<IDataShapeHelper<Stock>, DataShapeHelper<Stock>>();
            services.AddScoped<IDataShapeHelper<GetStockViewModel>, DataShapeHelper<GetStockViewModel>>();
            
            services.AddScoped<IDataShapeHelper<StockCard>, DataShapeHelper<StockCard>>();
            services.AddScoped<IDataShapeHelper<GetStockCardViewModel>, DataShapeHelper<GetStockCardViewModel>>();

            services.AddScoped<IDataShapeHelper<InvCtrlPeriod>, DataShapeHelper<InvCtrlPeriod>>();
            services.AddScoped<IDataShapeHelper<GetInvCtrlPeriodViewModel>, DataShapeHelper<GetInvCtrlPeriodViewModel>>();

            services.AddScoped<IDataShapeHelper<InvCtrl>, DataShapeHelper<InvCtrl>>();
            services.AddScoped<IDataShapeHelper<GetInvCtrlViewModel>, DataShapeHelper<GetInvCtrlViewModel>>();

            services.AddScoped<IDataShapeHelper<CustDiscount>, DataShapeHelper<CustDiscount>>();
            services.AddScoped<IDataShapeHelper<GetCustDiscountViewModel>, DataShapeHelper<GetCustDiscountViewModel>>();

            services.AddScoped<IDataShapeHelper<Zip>, DataShapeHelper<Zip>>();
            services.AddScoped<IDataShapeHelper<GetZipViewModel>, DataShapeHelper<GetZipViewModel>>();

            services.AddScoped<IDataShapeHelper<Location>, DataShapeHelper<Location>>();
            services.AddScoped<IDataShapeHelper<GetLocationViewModel>, DataShapeHelper<GetLocationViewModel>>();


            Assembly.GetExecutingAssembly().GetTypes().Where(w => w.Name.EndsWith("Handler")).ToList().ForEach((t) =>
            {
                services.AddTransient(t.GetTypeInfo().ImplementedInterfaces.First(), t);
            });
        }



    }
}
