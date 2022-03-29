using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
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
    public class createOutgoingInvoiceCommand : IRequest<Response<Invoice>>
    {

		[Description("Számlasor")]
		public class InvoiceLine
		{ 

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

		}

		[ColumnLabel("Raktár")]
		[Description("Raktár")]
		public long WarehouseCode { get; set; }

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

	public class CreateInvoiceCommandHandler : IRequestHandler<createOutgoingInvoiceCommand, Response<Invoice>>
    {
        private readonly IInvoiceRepositoryAsync _InvoiceRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateInvoiceCommandHandler(IInvoiceRepositoryAsync InvoiceRepository, IMapper mapper, IConfiguration configuration)
        {
            _InvoiceRepository = InvoiceRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Invoice>> Handle(createOutgoingInvoiceCommand request, CancellationToken cancellationToken)
        {
			//   var cnt = _mapper.Map<Invoice>(request);

			Invoice invoice = null;
			List<InvoiceLine> invoiceLines = null;
			List<SummaryByVatRate> summaryByVatRate = null;
			List<AdditionalInvoiceData> additionalInvoiceData = null;
			List<AdditionalInvoiceLineData> additionalInvoiceLineData = null;

			invoice = await _InvoiceRepository.AddInvoiceAsync(invoice, invoiceLines, summaryByVatRate, additionalInvoiceData, additionalInvoiceLineData);
            return new Response<Invoice>(invoice);
        }


    }
}
