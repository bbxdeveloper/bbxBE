using AutoMapper;
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
    public class createUSR_USERCommand : IRequest<Response<long>>
    {
        public string USR_NAME { get; set; }
        public string USR_EMAIL { get; set; }
        public string USR_LOGIN { get; set; }
        public string USR_PASSWDHASH { get; set; }
        public string USR_COMMENT { get; set; }
        public bool USR_ACTIVE { get; set; }



        public class createUSR_USERCommandHandler : IRequestHandler<createUSR_USERCommand, Response<long>>
        {
            private readonly IUSR_USERRepositoryAsync _usrRepository;
            private readonly IMapper _mapper;

            public createUSR_USERCommandHandler(IUSR_USERRepositoryAsync positionRepository, IMapper mapper)
            {
                _usrRepository = positionRepository;
                _mapper = mapper;
            }

            public async Task<Response<long>> Handle(createUSR_USERCommand request, CancellationToken cancellationToken)
            {
                var usr = _mapper.Map<USR_USER>(request);
                await _usrRepository.AddAsync(usr);
                return new Response<long>(usr.ID);
            }

     
        }
    }
 
}
