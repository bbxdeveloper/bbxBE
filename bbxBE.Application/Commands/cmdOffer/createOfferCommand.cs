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

namespace bxBE.Application.Commands.cmdOffer
{
	public class CreateOfferCommand
	{

		[Description("Árajánlat-sor")]
		public class OfferLine
		{
			[ColumnLabel("#")]
			[Description("Sor száma")]
			public short LineNumber { get; set; }

			[ColumnLabel("Termék ID")]
			[Description("Termék ID")]
			public long? ProductID { get; set; }

			[ColumnLabel("Termékkód")]
			[Description("Termékkód")]
			public string ProductCode { get; set; }

			[ColumnLabel("Megnevezés")]
			[Description("A termék vagy szolgáltatás megnevezése")]
			public string LineDescription { get; set; }

			[ColumnLabel("Áfa ID")]
			[Description("Áfa ID")]
			public long VatRateID { get; set; }

			[ColumnLabel("Áfaleíró-kód")]
			[Description("Áfaleíró-kód")]
			public string VatRateCode { get; set; }

			[ColumnLabel("Áfa%")]
			[Description("Az alkalmazott adó mértéke - Áfa tv. 169. § j)")]
			public decimal VatPercentage { get; set; }

			[ColumnLabel("Ár")]
			[Description("Ár")]
			public decimal UnitPrice { get; set; }

			[ColumnLabel("Ár forintban")]
			[Description("Ár forintban")]
			public decimal UnitPriceHUF { get; set; }

		}

		[ColumnLabel("Ügyfél ID")]
		[Description("Ügyfél ID")]
		public long CustomerID { get; set; }


		[ColumnLabel("Kelt")]
		[Description("Kiállítás dátuma")]
		public DateTime OfferIssueDate { get; set; }

		[ColumnLabel("Érvényesség")]
		[Description("Érvényesség dátuma")]
		public DateTime OfferVaidityDate { get; set; }

		[ColumnLabel("Megjegyzés")]
		[Description("Megjegyzés")]
		public string Notice { get; set; }





	}

	public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, Response<Offer>>
    {
        private readonly IOfferRepositoryAsync _OfferRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateOfferCommandHandler(IOfferRepositoryAsync OfferRepository, IMapper mapper, IConfiguration configuration)
        {
            _OfferRepository = OfferRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Offer>> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
        {
            var offer = _mapper.Map<Offer>(request);

            await _OfferRepository.AddOfferAsync(offer);
            return new Response<Offer>(offer);
        }


    }
}
