using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.BLL;
using bbxBE.Common.Consts;
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

namespace bbxBE.Application.Commands.cmdUser
{
    public class createUserCommand : IRequest<Response<Users>>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string Comment { get; set; }
        public bool Active { get; set; }

    }

    public class CreateUserCommandHandler : IRequestHandler<createUserCommand, Response<Users>>
    {
        private readonly IUserRepositoryAsync _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CreateUserCommandHandler(IUserRepositoryAsync userRepository, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Users>> Handle(createUserCommand request, CancellationToken cancellationToken)
        {
            var usr = _mapper.Map<Users>(request);
            usr.PasswordHash = bllUser.GetPasswordHash(request.Password, _configuration.GetValue<string>(bbxBEConsts.CONF_PwdSalt));

            await _userRepository.AddAsync(usr);
            return new Response<Users>(usr);
        }


    }
}
