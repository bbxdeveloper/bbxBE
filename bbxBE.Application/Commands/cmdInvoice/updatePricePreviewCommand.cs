using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static bbxBE.Common.NAV.NAV_enums;
using bbxBE.Common.NAV;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;

namespace bxBE.Application.Commands.cmdInvoice
{
	public class UpdatePricePreviewCommand : IRequest<Response<Invoice>>
	{

		[Description("Számlasor")]
		public class InvoiceLine
		{

			[ColumnLabel("ID")]
			[Description("Sor száma")]
			public short ID { get; set; }

			[ColumnLabel("Ár")]
			[Description("Ár")]
			public decimal UnitPrice { get; set; }

		}

		[ColumnLabel("ID")]
		[Description("Bizonylat azonosító")]
		public short ID { get; set; }

		[ColumnLabel("Ügyfél ID")]
		[Description("Ügyfél ID")]
		public long CustomerID { get; set; }

		[ColumnLabel("Felhasználó ID")]
		[Description("Felhasználó ID")]
		public long? UserID { get; set; } = 0;

		[ColumnLabel("Számlasorok")]
		[Description("Számlasorok")]
		public List<UpdatePricePreviewCommand.InvoiceLine> InvoiceLines { get; set; } = new List<UpdatePricePreviewCommand.InvoiceLine>();

	}

	public class UpdatePricePreviewCommandHandler : IRequestHandler<UpdatePricePreviewCommand, Response<Invoice>>
	{
		private readonly IInvoiceRepositoryAsync _InvoiceRepository;
		private readonly IInvoiceLineRepositoryAsync _InvoiceLineRepository;
		private readonly ICustomerRepositoryAsync _CustomerRepository;
		private readonly IProductRepositoryAsync _ProductRepository;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public UpdatePricePreviewCommandHandler(
			IInvoiceRepositoryAsync InvoiceRepository,
			IInvoiceLineRepositoryAsync InvoiceLineRepository,
			ICustomerRepositoryAsync CustomerRepository,
			IProductRepositoryAsync ProductRepository,
			IMapper mapper, IConfiguration configuration)
		{
			_InvoiceRepository = InvoiceRepository;
			_InvoiceLineRepository = InvoiceLineRepository;
			_CustomerRepository = CustomerRepository;
			_ProductRepository = ProductRepository;
			_mapper = mapper;
			_configuration = configuration;
		}

        public async Task<Response<Invoice>> Handle(UpdatePricePreviewCommand request, CancellationToken cancellationToken)
		{

			var inv = await bllInvoice.UpdatePricePreviewAsynch(request, _mapper,
									_InvoiceRepository,
									_ProductRepository,
									cancellationToken);
			return new Response<Invoice>(inv);
		}


	}
}