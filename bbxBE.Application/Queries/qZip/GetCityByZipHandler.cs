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
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;

namespace bbxBE.Application.Queries.qZip
{
    public class GetCityByZip:  IRequest<Zip>
    {
        public string ZipCode { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetCityByZipHandlerHandler : IRequestHandler<GetCityByZip, Zip>
    {
        private readonly IZipRepositoryAsync _ZipRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetCityByZipHandlerHandler(IZipRepositoryAsync ZipRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _ZipRepository = ZipRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Zip> Handle(GetCityByZip request, CancellationToken cancellationToken)
        {
            var zip = await _ZipRepository.GetCityBzZip(request.ZipCode);

            if (zip == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_CITYNOTFOUND, request.ZipCode));
            }

            // response wrapper
            return zip;
        }
               
    }
}