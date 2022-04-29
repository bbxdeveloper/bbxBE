using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
using bbxBE.Application.Exceptions;
using bbxBE.Application.Interfaces.Repositories;
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
		private readonly ICounterRepositoryAsync _CounterRepositoryAsync;
		private readonly IWarehouseRepositoryAsync _WarehouseRepositoryAsync;
		private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateInvoiceCommandHandler(IInvoiceRepositoryAsync InvoiceRepository,
			ICounterRepositoryAsync CounterRepositoryAsync,
			IWarehouseRepositoryAsync WarehouseRepositoryAsync,
			IMapper mapper, IConfiguration configuration)
        {
            _InvoiceRepository = InvoiceRepository;
			_CounterRepositoryAsync = CounterRepositoryAsync;
			_WarehouseRepositoryAsync = WarehouseRepositoryAsync;
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
		  "notice": "Megjegyzés",
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
			var cnt = _mapper.Map<Invoice>(request);

			Invoice invoice = null;
			List<InvoiceLine> invoiceLines = null;
			List<SummaryByVatRate> summaryByVatRate = null;
			List<AdditionalInvoiceData> additionalInvoiceData = null;
			List<AdditionalInvoiceLineData> additionalInvoiceLineData = null;

			request.WarehouseCode = bbxBEConsts.DEF_WAREHOUSE;		//Átmenetileg

			var wh = await _WarehouseRepositoryAsync.GetWarehouseByCodeAsync( request.WarehouseCode);
			if (wh == null)
			{
				throw new ResourceNotFoundException(string.Format(bbxBEConsts.FV_WAREHOUSENOTFOUND, request.WarehouseCode));
			}

			invoice.WarehouseID = wh.ID;

			var invoiceType = (enInvoiceType)Enum.Parse(typeof(enInvoiceType), invoice.InvoiceType);
			var counterCode = bllCounter.GetCounterCode(invoiceType, invoice.Incoming, wh.ID);
			var invNum = _CounterRepositoryAsync.GetNextValueAsync(counterCode, wh.ID);

			invoice = await _InvoiceRepository.AddInvoiceAsync(invoice, invoiceLines, summaryByVatRate, additionalInvoiceData, additionalInvoiceLineData);
            return new Response<Invoice>(invoice);
        }


    }
}
