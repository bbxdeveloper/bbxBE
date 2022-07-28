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

namespace bxBE.Application.Commands.cmdInvCtrl
{


	[Description("Leltáridőszak")]
	public class UpdateInvCtrlCommand : IRequest<Response<InvCtrl>>
	{
		[ColumnLabel("ID")]
		[Description("ID")]
		public long ID { get; set; }

		[ColumnLabel("Raktár ID")]
		[Description("Raktár ID")]
		public long WarehouseID { get; set; }

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

	public class UpdateInvCtrlCommandHandler : IRequestHandler<UpdateInvCtrlCommand, Response<InvCtrl>>
	{
		private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public UpdateInvCtrlCommandHandler(IInvCtrlRepositoryAsync InvCtrlRepository,
						IMapper mapper, IConfiguration configuration)
		{
			_InvCtrlRepository = InvCtrlRepository;


			_mapper = mapper;
			_configuration = configuration;
		}

		public async Task<Response<InvCtrl>> Handle(UpdateInvCtrlCommand request, CancellationToken cancellationToken)
		{
			var InvCtrl = _mapper.Map<InvCtrl>(request);
			await _InvCtrlRepository.UpdateInvCtrlAsync(InvCtrl);
			return new Response<InvCtrl>(InvCtrl);

		}
	}
}