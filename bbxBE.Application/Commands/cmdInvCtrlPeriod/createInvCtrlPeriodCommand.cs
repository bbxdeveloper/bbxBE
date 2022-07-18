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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdInvCtrlPeriod
{

	public class CreateInvCtrlPeriodCommand : IRequest<Response<InvCtrlPeriod>>
	{


		[Description("Leltáridőszak")]
		public class InvCtrlPeriodLine
		{
			[ColumnLabel("Kezdődátum")]
			[Description("Kezdődátum")]
			public DateTime DateFrom { get; set; }

			[ColumnLabel("Végdátum")]
			[Description("Végdátum")]
			public DateTime DateTo { get; set; }

			[ColumnLabel("Felhasználó ID")]
			[Description("Felhasználó ID")]
			public long? UserID { get; set; } = 0;

		}

		public class CreateInvCtrlPeriodCommandHandler : IRequestHandler<CreateInvCtrlPeriodCommand, Response<InvCtrlPeriod>>
		{
			private readonly IInvCtrlPeriodRepositoryAsync _InvCtrlPeriodRepository;
			private readonly IMapper _mapper;
			private readonly IConfiguration _configuration;

			public CreateInvCtrlPeriodCommandHandler(IInvCtrlPeriodRepositoryAsync InvCtrlPeriodRepository,
							IMapper mapper, IConfiguration configuration)
			{
				_InvCtrlPeriodRepository = InvCtrlPeriodRepository;


				_mapper = mapper;
				_configuration = configuration;
			}

			public async Task<Response<InvCtrlPeriod>> Handle(CreateInvCtrlPeriodCommand request, CancellationToken cancellationToken)
			{
				var InvCtrlPeriod = _mapper.Map<InvCtrlPeriod>(request);

			}
		}
	}
}
