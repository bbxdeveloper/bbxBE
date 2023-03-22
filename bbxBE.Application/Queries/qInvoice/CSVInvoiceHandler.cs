using AutoMapper;
using MediatR;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Domain.Extensions;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Attributes;
using System.ComponentModel;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Linq;
using bbxBE.Common.Consts;

namespace bbxBE.Application.Queries.qInvoice
{
    public class CSVInvoice : IRequest<FileStreamResult>
    {

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylat sorszáma")]
        public string InvoiceNumber { get; set; }

        [ColumnLabel("Bizonylattípus")]
        [Description("Bizonylattípus")]
        public string InvoiceType { get; set; }


        [ColumnLabel("Kelt.tól")]
        [Description("Kiállítás dátumától")]
        public DateTime? InvoiceIssueDateFrom { get; set; }

        [ColumnLabel("Kelt.ig")]
        [Description("Kiállítás dátumáig")]
        public DateTime? InvoiceIssueDateTo { get; set; }

        [ColumnLabel("Teljesítés tól")]
        [Description("Teljesítés dátumától")]
        public DateTime? InvoiceDeliveryDateFrom { get; set; }

        [ColumnLabel("Teljesítés ig")]
        [Description("Teljesítés dátumig")]
        public DateTime? InvoiceDeliveryDateTo { get; set; }

        public string OrderBy { get; set; }

    }

    public class CSVInvoiceHandler : IRequestHandler<CSVInvoice, FileStreamResult>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public CSVInvoiceHandler(IInvoiceRepositoryAsync invoiceRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<FileStreamResult> Handle(CSVInvoice request, CancellationToken cancellationToken)
        {





            var lstInv = await _invoiceRepository.QueryForCSVInvoiceAsync(request);
            string csv = String.Join(Environment.NewLine,
                    lstInv.Select(x =>

								x.ID.ToString() + bbxBEConsts.DEF_CSVSEP +
								x.WarehouseID.ToString() + bbxBEConsts.DEF_CSVSEP +
								x.Warehouse + bbxBEConsts.DEF_CSVSEP +
								x.InvoiceNumber+ bbxBEConsts.DEF_CSVSEP +
								x.InvoiceIssueDate.ToString(bbxBEConsts.DEF_DATEFORMAT) + bbxBEConsts.DEF_CSVSEP +	
								x.InvoiceDeliveryDate.ToString(bbxBEConsts.DEF_DATEFORMAT) + bbxBEConsts.DEF_CSVSEP +
								x.PaymentDate.ToString(bbxBEConsts.DEF_DATEFORMAT) + bbxBEConsts.DEF_CSVSEP +
								x.SupplierID.ToString() + bbxBEConsts.DEF_CSVSEP +
								x.SupplierName + bbxBEConsts.DEF_CSVSEP +
								x.SupplierBankAccountNumber + bbxBEConsts.DEF_CSVSEP +
								x.SupplierTaxpayerNumber + bbxBEConsts.DEF_CSVSEP +
								x.SupplierCountryCode + bbxBEConsts.DEF_CSVSEP +
								x.SupplierPostalCode + bbxBEConsts.DEF_CSVSEP +
								x.SupplierCity + bbxBEConsts.DEF_CSVSEP +
								x.SupplierAdditionalAddressDetail + bbxBEConsts.DEF_CSVSEP +
								x.SupplierThirdStateTaxId + bbxBEConsts.DEF_CSVSEP +
								x.SupplierComment + bbxBEConsts.DEF_CSVSEP +
								x.CustomerID.ToString() + bbxBEConsts.DEF_CSVSEP +
								x.CustomerName + bbxBEConsts.DEF_CSVSEP +
								x.CustomerBankAccountNumber + bbxBEConsts.DEF_CSVSEP +
								x.CustomerTaxpayerNumber + bbxBEConsts.DEF_CSVSEP +
								x.CustomerCountryCode + bbxBEConsts.DEF_CSVSEP +
								x.CustomerPostalCode + bbxBEConsts.DEF_CSVSEP +
								x.CustomerCity + bbxBEConsts.DEF_CSVSEP +
								x.CustomerAdditionalAddressDetail + bbxBEConsts.DEF_CSVSEP +
								x.CustomerThirdStateTaxId + bbxBEConsts.DEF_CSVSEP +
								x.CustomerComment + bbxBEConsts.DEF_CSVSEP +
								x.PaymentMethod + bbxBEConsts.DEF_CSVSEP +
								x.PaymentMethodX + bbxBEConsts.DEF_CSVSEP +
								x.CustomerInvoiceNumber + bbxBEConsts.DEF_CSVSEP +
								x.Notice + bbxBEConsts.DEF_CSVSEP +
								x.Copies.ToString() + bbxBEConsts.DEF_CSVSEP +
double formázás!!!
			[ColumnLabel("Kedvezmény%")]
			[Description("A számlára adott teljes kedvezmény %")]
			public decimal InvoiceDiscountPercent { get; set; }
			[ColumnLabel("Kedvezmény")]
			[Description("A számlára adott teljes kedvezmény % értéke a számla pénznemében")]
			public decimal InvoiceDiscount { get; set; }

			[ColumnLabel("Kedvezmény HUF")]
			[Description("A számlára adott teljes kedvezmény % értéke fortintban")]
			public decimal InvoiceDiscountHUF { get; set; }


			[ColumnLabel("Nettó")]
			[Description("A számla nettó összege a számla pénznemében")]
			public decimal InvoiceNetAmount { get; set; }

			[ColumnLabel("Nettó HUF")]
			[Description("A számla nettó összege forintban")]
			public decimal InvoiceNetAmountHUF { get; set; }

			[ColumnLabel("Afa")]
			[Description("A számla áfa összege a számla pénznemében")]
			public decimal InvoiceVatAmount { get; set; }

			[ColumnLabel("Afa HUF")]
			[Description("A számla áfa összege forintban")]
			public decimal InvoiceVatAmountHUF { get; set; }

			[ColumnLabel("Bruttó")]
			[Description("A számla végösszege a számla pénznemében")]
			public decimal InvoiceGrossAmount { get; set; }

			[ColumnLabel("Bruttó HUF")]
			[Description("A számla végösszege forintban")]
			public decimal InvoiceGrossAmountHUF { get; set; }

			[ColumnLabel("Módosító bizonylat?")]
			[Description("Módosító bizonylat jelölése (értéke false)")]
			public bool Correction { get; set; }

			[ColumnLabel("Felhasználó ID")]
			[Description("Felhasználó ID")]
			public long UserID { get; set; }

			[ColumnLabel("Felhasználó")]
			[Description("Felhasználó")]
			[MapToEntity("UserName")]
			public string UserName { get; set; }

			[ColumnLabel("Munkaszám")]
			[Description("Munkaszám")]
			public string WorkNumber { get; set; }

			[ColumnLabel("Ar felülvizsgálat?")]
			[Description("Ar felülvizsgálat?")]
			public bool PriceReview { get; set; } = false;



			x.ProductCode + ";" + x.UnitPrice.ToString().Replace(",", ".")
                    
                    
                    
                    
                    
                    ).ToArray());



            var csvHeader = "ID;Raktár ID;Raktár;Bizonylatszám;Kelt;Teljesítés;Fiz.hat;Szállító ID;Szállítónév;Szállító bankszámlaszám;Szállító adószám;Szállító országkód;Szállító irányítószám;Szállító város;Szállítócím;Külföldi adószám;" +
                            "Szállító megjegyzés; Ügyfél ID; Ügyfélnév; Ügyfél bankszámlaszám; Ügyfél adószám; Ügyfél országkód; Ügyfél irányítószám; Ügyfél város; Ügyfélcím; Külföldi adószám; Ügyfél megjegyzés;" +
                            "Fiz.mód; Fizetési mód megnevezés; Eredeti.biz; Megjegyzés; Nyomtatott példány száma; Kedvezmény %; Kedvezmény; Kedvezmény HUF; Nettó; Nettó HUF;╡fa; Afa HUF; Bruttó; Bruttó HUF;" +
                            "Módosító bizonylat?; Felhasználó ID; Felhasználó; Munkaszám; Ar felülvizsgálat?";



            string csv = String.Join(Environment.NewLine, offer.OfferLines.Select(x => x.ProductCode + ";" + x.UnitPrice.ToString().Replace(",", ".")).ToArray());
            Stream stream = Utils.StringToStream(csv);
            string fileName = $"Offer{offer.OfferNumber.Replace("/", "-")}.csv";


            var fsr = new FileStreamResult(stream, $"application/csv") { FileDownloadName = fileName };

            return fsr;

            // query based on filter
            var entities = await _invoiceRepository.QueryPagedInvoiceAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetInvoiceViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}