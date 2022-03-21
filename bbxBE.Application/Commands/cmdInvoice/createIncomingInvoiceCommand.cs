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
    public class CreateIncomingInvoiceCommand : IRequest<Response<Invoice>>
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

			[ColumnLabel("Ár")]
			[Description("Ár")]
			public decimal Price { get; set; }
			0
			[ColumnLabel("Nettó érték")]
			[Description("Ár a számla pénznemében")]
			public decimal LineNetAmount { get; set; }

			[ColumnLabel("Áfa érték")]
			[Description("Áfa a számla pénznemében")]
			public decimal lineVatAmount { get; set; }

			[ColumnLabel("Bruttó érték")]
			[Description("Bruttó a számla pénznemében")]
			public decimal lineGrossAmountNormal { get; set; }

		}

		[Description("Számla áfánkénti összesítő")]
		public class SummaryByVatRate 
		{
			[ColumnLabel("Áfakód")]
			[Description("Áfakód")]
			public string VatRateCode { get; set; }

			[ColumnLabel("Áfa értéke")]
			[Description("Áfa értéke a számla pénznemében")]
			public decimal VatRateNetAmount { get; set; }
			[ColumnLabel("Áfa HUF")]
			[Description("Áfa értéke forintban")]
			public decimal VatRateNetAmountHUF { get; set; }



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

		[ColumnLabel("Bevétel biz.")]
		[Description("Bevétel alapjául szolgáló bizonylat")]
		public string IncomingInvReference { get; set; }

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
		[ColumnLabel("Bruttó")]
		[Description("A számla végösszege a számla pénznemében")]
		public decimal InvoiceGrossAmount { get; set; }

		[ColumnLabel("Számlasorok")]
		[Description("Számlasorok")]
		public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();




	}

	public class CreateInvoiceCommandHandler : IRequestHandler<CreateIncomingInvoiceCommand, Response<Invoice>>
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

        public async Task<Response<Invoice>> Handle(CreateIncomingInvoiceCommand request, CancellationToken cancellationToken)
        {
            var cnt = _mapper.Map<Invoice>(request);

            cnt = await _InvoiceRepository.AddInvoiceAsync(cnt, request.WarehouseCode);
            return new Response<Invoice>(cnt);
        }


    }
}
