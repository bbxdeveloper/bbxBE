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
    public class GetZipByCity:  IRequest<Zip>
    {
        public string ZipCity { get; set; }
    }

    public class GetZipByCityHandlerHandler : IRequestHandler<GetZipByCity, Zip>
    {
        private readonly IZipRepositoryAsync _ZipRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetZipByCityHandlerHandler(IZipRepositoryAsync ZipRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _ZipRepository = ZipRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Zip> Handle(GetZipByCity request, CancellationToken cancellationToken)
        {
            var zip = await _ZipRepository.GetZipByCity(request.ZipCity);

            if (zip == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_ZIPNOTFOUND, request.ZipCity));
            }

            // response wrapper
            return zip;
        }
               
    }
}