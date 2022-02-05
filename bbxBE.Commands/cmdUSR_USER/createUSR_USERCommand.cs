using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Commands.cmdUSR_USER
{
    public class CreateUSR_USERCommand : IRequest<Response<long>>
    {
        [MapTo("USR_NAME")]
        public string Name { get; set; }
        [MapTo("USR_EMAIL")]
        public string Email { get; set; }
        [MapTo("USR_LOGIN")]
        public string LoginName { get; set; }
        public string Password { get; set; }
        [MapTo("USR_LOGIN")]
        public string Comment { get; set; }

    }

    public class CreateUSR_USERCommandHandler : IRequestHandler<CreateUSR_USERCommand, Response<long>>
    {
        private readonly IUSR_USERRepositoryAsync _usrRepository;
        private readonly IMapper _mapper;

        public CreateUSR_USERCommandHandler(IUSR_USERRepositoryAsync positionRepository, IMapper mapper)
        {
            _usrRepository = positionRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(CreateUSR_USERCommand request, CancellationToken cancellationToken)
        {
            var usr = _mapper.Map<USR_USER>(request);
            await _usrRepository.AddAsync(usr);
            return new Response<long>(0);
        }


    }
}
