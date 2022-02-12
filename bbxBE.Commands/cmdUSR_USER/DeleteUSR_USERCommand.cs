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
    public class DeleteUSR_USERCommand : IRequest<Response<long>>
    {
        public long ID { get; set; }

    }

    public class DeleteUSR_USERCommandHandler : IRequestHandler<DeleteUSR_USERCommand, Response<long>>
    {
        private readonly IUSR_USERRepositoryAsync _usrRepository;
        private readonly IMapper _mapper;

        public DeleteUSR_USERCommandHandler(IUSR_USERRepositoryAsync usrRepository, IMapper mapper)
        {
            _usrRepository = usrRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteUSR_USERCommand request, CancellationToken cancellationToken)
        {
            var usr = _mapper.Map<USR_USER>(request);
            await _usrRepository.DeleteAsync(usr);
            return new Response<long>(usr.ID);
        }


    }
}
