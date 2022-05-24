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

namespace bbxBE.Application.Commands.cmdUSR_USER
{

    public class UpdateUSR_USERCommand : IRequest<Response<USR_USER>>
    {
        public long ID { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string LoginName { get; set; }

        public string Password { get; set; }
        public string Comment { get; set; }
        public bool Active { get; set; }

    }

    public class UpdateUSR_USERCommandHandler : IRequestHandler<UpdateUSR_USERCommand, Response<USR_USER>>
    {
        private readonly IUSR_USERRepositoryAsync _usrRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateUSR_USERCommandHandler(IUSR_USERRepositoryAsync usrRepository, IMapper mapper, IConfiguration configuration)
        {
            _usrRepository = usrRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<USR_USER>> Handle(UpdateUSR_USERCommand request, CancellationToken cancellationToken)
        {

            var usr = _mapper.Map<USR_USER>(request);

            usr.USR_PASSWDHASH = bllUser.GetPasswordHash(request.Password, _configuration.GetValue<string>(bbxBEConsts.CONF_PwdSalt));

            await _usrRepository.UpdateAsync(usr);
            return new Response<USR_USER>(usr);
        }


    }
}
