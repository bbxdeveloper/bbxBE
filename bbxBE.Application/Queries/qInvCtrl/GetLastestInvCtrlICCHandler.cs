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

namespace bbxBE.Application.Queries.qInvCtrl
{
    public class GetLastestInvCtrlICC:  IRequest<Entity>
    {
        [ColumnLabel("Raktárkód")]
        [Description("Raktár kódja")]
        public string WarehouseCode { get; set; }

        [ColumnLabel("Termék ID")]
        [Description("Termék ID")]
        public long ProductID { get; set; }

        [ColumnLabel("Hány nappal régebbi adatra van szükség?")]
        [Description("Hány nappal régebbi adatra van szükség?")]
        public int RetroDays { get; set; }
    }

    public class GetLastestInvCtrlICCHandler : IRequestHandler<GetLastestInvCtrlICC, Entity>
    {
        private readonly IInvCtrlRepositoryAsync _InvCtrlRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetLastestInvCtrlICCHandler(IInvCtrlRepositoryAsync InvCtrlRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _InvCtrlRepository = InvCtrlRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetLastestInvCtrlICC request, CancellationToken cancellationToken)
        {
            // query based on filter
            var item = await _InvCtrlRepository.GetLastestInvCtrlICC(request);
            if (item == null)
                return null;

            var data = item.MapItemFieldsByMapToAnnotation<GetCustDiscountViewModel>();
            return data;
        }
    }
}