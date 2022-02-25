using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Application.Consts;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdOrigin
{
    public class CreateOriginCommand : IRequest<Response<Origin>>
    {
        public string OriginCode { get; set; }
        public string OriginDescription { get; set; }

    }

    public class CreateUSR_USERCommandHandler : IRequestHandler<CreateOriginCommand, Response<Origin>>
    {
        private readonly IOriginRepositoryAsync _OriginRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateUSR_USERCommandHandler(IOriginRepositoryAsync OriginRepository, IMapper mapper, IConfiguration configuration)
        {
            _OriginRepository = OriginRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Origin>> Handle(CreateOriginCommand request, CancellationToken cancellationToken)
        {
            var cust = _mapper.Map<Origin>(request);

            await _OriginRepository.AddAsync(cust);
            return new Response<Origin>(cust);
        }


    }
}
