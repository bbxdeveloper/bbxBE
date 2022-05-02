using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.qCustomer;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdInvoice
{
    public class CreateInvoiceCommand : IRequest<Response<Invoice>>
    {

		[Description("Számlasor")]
		public class InvoiceLine
		{ 

			[ColumnLabel("#")]
			[Description("Sor száma")]
			public short LineNumber { get; set; }

			[ColumnLabel("Termékkód")]
			[Description("Termékkód")]
			public string ProductCode { get; set; }

			[ColumnLabel("Áfakód")]
			[Description("Áfakód")]
			public string VatRateCode { get; set; }

			[ColumnLabel("Mennyiség")]
			[Description("Mennyiség")]
			public decimal Quantity { get; set; }

			[ColumnLabel("Me.e.")]
			[Description("Mennyiségi egység kód")]
			public string UnitOfMeasure { get; set; }
			[ColumnLabel("Ár")]
			[Description("Ár")]
			public decimal Price { get; set; }
			
			[ColumnLabel("Nettó érték")]
			[Description("Ár a számla pénznemében")]
			public decimal LineNetAmount { get; set; }
			
			[ColumnLabel("Áfa érték")]
			[Description("Áfa a számla pénznemében")]
			public decimal lineVatAmount { get; set; }

			[ColumnLabel("Bruttó érték")]
			[Description("Bruttó értéka számla pénznemében")]
			public decimal lineGrossAmount { get; set; }

		}
		/*
				[Description("Számla áfánkénti összesítő")]
				public class SummaryByVatRate 
				{
					[ColumnLabel("Áfakód")]
					[Description("Áfakód")]
					public string VatRateCode { get; set; }

					[ColumnLabel("Áfa értéke")]
					[Description("Áfa értéke a számla pénznemében")]
					public decimal VatRateNetAmount { get; set; }
				}
		*/

		[ColumnLabel("B/K")]
		[Description("Bejővő/Kimenő")]
		public bool Incoming { get; set; }

		[ColumnLabel("Típus")]
		[Description("Típus")]
		public string InvoiceType { get; set; }

		[ColumnLabel("Raktár")]
		[Description("Raktár")]
		public string WarehouseCode { get; set; }

		[ColumnLabel("Kelt")]
		[Description("Kiállítás dátuma")]
		public DateTime InvoiceIssueDate { get; set; }

		[ColumnLabel("Teljesítés")]
		[Description("Teljesítés dátuma")]
		public DateTime InvoiceDeliveryDate { get; set; }

		[ColumnLabel("Fiz.hat")]
		[Description("Fizetési határidő dátuma")]
		public DateTime PaymentDate { get; set; }


		[ColumnLabel("Ügyfél ID")]
		[Description("Ügyfél ID")]
		public long CustomerID { get; set; }


		[ColumnLabel("Fiz.mód")]
		[Description("Fizetési mód")]
		public string PaymentMethod { get; set; }

		[ColumnLabel("Pénznem")]
		[Description("Pénznem")]
		public string CurrencyCode { get; set; }


		[ColumnLabel("Árfolyam")]
		[Description("Árfolyam")]
		public decimal ExchangeRate { get; set; }

		[ColumnLabel("Megjegyzés")]
		[Description("Megjegyzés")]
		public string Notice { get; set; }	//AdditionalInvoiceData-ban tároljuk!

		[ColumnLabel("Nettó")]
		[Description("A számla nettó összege a számla pénznemében")]
		public decimal InvoiceNetAmount { get; set; }

		[ColumnLabel("Áfa")]
		[Description("A számla áfa összege a számla pénznemében")]
		public decimal InvoiceVatAmount { get; set; }

		[ColumnLabel("Számlasorok")]
		[Description("Számlasorok")]
		public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

		/*
		[ColumnLabel("Áfaösszesítők")]
		[Description("Áfaösszesítők")]
		public List<SummaryByVatRate> SummaryByVatRates { get; set; } = new List<SummaryByVatRate>();
		*/
	}

	public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Response<Invoice>>
    {
		private readonly IInvoiceRepositoryAsync _InvoiceRepository;
		private readonly ICounterRepositoryAsync _CounterRepository;
		private readonly IWarehouseRepositoryAsync _WarehouseRepository;
		private readonly ICustomerRepositoryAsync _CustomerRepository;
		private readonly IProductRepositoryAsync _ProductRepository;
		private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateInvoiceCommandHandler(IInvoiceRepositoryAsync InvoiceRepository,
			ICounterRepositoryAsync CounterRepository,
			IWarehouseRepositoryAsync WarehouseRepository,
			ICustomerRepositoryAsync CustomerRepository,
			IProductRepositoryAsync ProductRepository,
			IMapper mapper, IConfiguration configuration)
        {
            _InvoiceRepository = InvoiceRepository;
			_CounterRepository = CounterRepository;
			_WarehouseRepository = WarehouseRepository;
			_CustomerRepository = CustomerRepository;
			_ProductRepository = ProductRepository;
			_mapper = mapper;
            _configuration = configuration;
        }
		/*
		 {
		  "incoming": false,
		  "invoiceType": "INV",
		  "warehouseCode": "001",
		  "invoiceIssueDate": "2022-04-29",
		  "invoiceDeliveryDate": "2022-04-29",
		  "paymentDate": "2022-04-29",
		  "customerID": 12,
		  "paymentMethod": "TRANSFER",
		 "currencyCode": "HUF",
		  "exchangeRate": 1,		  "notice": "Megjegyzés",
		  "invoiceNetAmount": 110,
		  "invoiceVatAmount": 297,
		  "invoiceLines": [
			{
			  "lineNumber": 1,
			  "productCode": "TOK-7238",
			  "vatRateCode": "27%",
			  "quantity": 1,
			  "unitOfMeasure": "PIECE",
			  "price": 10,
			  "lineNetAmount": 10,
			  "lineVatAmount": 27,
			  "lineGrossAmount": 127
			},
			{
			  "lineNumber": 2,
			  "productCode": "MUA-571",
			  "vatRateCode": "27%",
			  "quantity": 1,
			  "unitOfMeasure": "PIECE",
			  "price": 100,
			  "lineNetAmount": 100,
			  "lineVatAmount": 270,
			  "lineGrossAmount": 1270
			}
		  ]
		}


		 */


		public async Task<Response<Invoice>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
			var invoice = _mapper.Map<Invoice>(request);

			

			List<InvoiceLine> invoiceLines = null;
			List<SummaryByVatRate> summaryByVatRate = null;
			List<AdditionalInvoiceData> additionalInvoiceData = null;
			List<AdditionalInvoiceLineData> additionalInvoiceLineData = null;


			//ID-k feloldása
			request.WarehouseCode = bbxBEConsts.DEF_WAREHOUSE;		//Átmenetileg

			var wh = await _WarehouseRepository.GetWarehouseByCodeAsync( request.WarehouseCode);
			if (wh == null)
			{
				throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_WAREHOUSENOTFOUND, request.WarehouseCode));
			}
			invoice.WarehouseID = wh.ID;
			
			/* ez nem kell
			var cust =  _CustomerRepository.GetCustomer(new GetCustomer() { ID = request.CustomerID });
			if (cust == null)
			{
				throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_CUSTNOTFOUND, request.CustomerID));
			}
			invoice.Customer = cust;
			*/

			//Megjegyzés
			if( !string.IsNullOrWhiteSpace( request.Notice))
            {
				invoice.AdditionalInvoiceData = new List<AdditionalInvoiceData>() {  new AdditionalInvoiceData()
							{ DataName = "Notice", DataDescription = "Megjegyzés", DataValue = request.Notice }};

			}

			//Számlaszám megállapítása
			var invoiceType = (enInvoiceType)Enum.Parse(typeof(enInvoiceType), invoice.InvoiceType);
			var counterCode = bllCounter.GetCounterCode(invoiceType, invoice.Incoming, wh.ID);
            invoice.InvoiceNumber = await _CounterRepository.GetNextValueAsync(counterCode, wh.ID);


			//Kiszámítható mezők kiszámolása
			invoice.InvoiceNetAmountHUF = invoice.InvoiceNetAmount * invoice.ExchangeRate;
			invoice.InvoiceVatAmountHUF = invoice.InvoiceVatAmount * invoice.ExchangeRate;

			invoice.InvoiceGrossAmount = invoice.InvoiceNetAmount + invoice.InvoiceVatAmount;
			invoice.invoiceGrossAmountHUF = invoice.InvoiceNetAmountHUF + invoice.InvoiceVatAmountHUF;

			//Tételsorok
			

			invoice = await _InvoiceRepository.AddInvoiceAsync(invoice, invoiceLines, summaryByVatRate, additionalInvoiceData, additionalInvoiceLineData);
            return new Response<Invoice>(invoice);
        }


    }
}
