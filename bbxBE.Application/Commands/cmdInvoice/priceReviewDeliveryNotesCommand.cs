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
	public class PriceReviewDeliveryNotesCommand : IRequest<Response<List<Invoice>>>
	{

		[Description("Számlasor")]
		public class InvoiceLine
		{

			[ColumnLabel("ID")]
			[Description("Azonosító")]
			public short ID { get; set; }

			[ColumnLabel("Ár")]
			[Description("Ár")]
			public decimal UnitPrice { get; set; }


		}

		[ColumnLabel("ID")]
		[Description("Azonosító")]
		public short ID { get; set; }
		[ColumnLabel("Ügyfél ID")]
		[Description("Ügyfél ID")]
		public long CustomerID { get; set; }

		[ColumnLabel("Árfolyam")]
		[Description("Árfolyam")]
		public decimal ExchangeRate { get; set; }

		[ColumnLabel("Felhasználó ID")]
		[Description("Felhasználó ID")]
		public long? UserID { get; set; } = 0;

		[ColumnLabel("Számlasorok")]
		[Description("Számlasorok")]
		public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

	}

	public class PriceReviewDeliveryNotesCommandHandler : IRequestHandler<PriceReviewDeliveryNotesCommand, Response<List<Invoice>>>
	{
		private readonly IInvoiceRepositoryAsync _InvoiceRepository;
		private readonly IInvoiceLineRepositoryAsync _InvoiceLineRepository;
		private readonly ICounterRepositoryAsync _CounterRepository;
		private readonly IWarehouseRepositoryAsync _WarehouseRepository;
		private readonly ICustomerRepositoryAsync _CustomerRepository;
		private readonly IProductRepositoryAsync _ProductRepository;
		private readonly IVatRateRepositoryAsync _VatRateRepository;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public PriceReviewDeliveryNotesCommandHandler(
			IInvoiceRepositoryAsync InvoiceRepository,
			IInvoiceLineRepositoryAsync InvoiceLineRepository,
			ICounterRepositoryAsync CounterRepository,
			IWarehouseRepositoryAsync WarehouseRepository,
			ICustomerRepositoryAsync CustomerRepository,
			IProductRepositoryAsync ProductRepository,
			IVatRateRepositoryAsync VatRateRepository,
			IMapper mapper, IConfiguration configuration)
		{
			_InvoiceRepository = InvoiceRepository;
			_InvoiceLineRepository = InvoiceLineRepository;
			_CounterRepository = CounterRepository;
			_WarehouseRepository = WarehouseRepository;
			_CustomerRepository = CustomerRepository;
			_ProductRepository = ProductRepository;
			_VatRateRepository = VatRateRepository;
			_mapper = mapper;
			_configuration = configuration;
		}
        public async Task<Response<List<Invoice>>> Handle(PriceReviewDeliveryNotesCommand request, CancellationToken cancellationToken)
		{
			/*
			var inv = await bllInvoice.CreateInvoiceAsynch(request, _mapper,
									_InvoiceRepository,
									_InvoiceLineRepository,
									_CounterRepository,
									_WarehouseRepository,
									_CustomerRepository,
									_ProductRepository,
									_VatRateRepository,
									cancellationToken);
			return new Response<Invoice>(inv);
			*/
			var res = new List<Invoice>();

			return new Response<List<Invoice>>(res);
		}


	}
}