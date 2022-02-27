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
using System;
using System.Linq;
using bbxBE.Common;
using bbxBE.Application.Enums;

namespace bbxBE.Application.Queries.qEnum
{
    public class GetEnum : IRequest<List<GetEnumModel>>
    {
        public Type type { get; set; }   
     }

    public class GetEnumHandler : IRequestHandler<GetEnum, List<GetEnumModel>>
    {
    private readonly IModelHelper _modelHelper;

    public GetEnumHandler(IProductRepositoryAsync positionRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _modelHelper = modelHelper;
        }


        public async Task<List<GetEnumModel>> Handle(GetEnum request, CancellationToken cancellationToken)
        {

            var e = Enum.GetValues(request.type).Cast<Enum>()
                            .Select(v => new GetEnumModel()
                            {
                                Value = v.ToString(),
                                Text = Utils.GetEnumDescription(v)
                            })
                            .ToList();
            return e;
        }
    }
}