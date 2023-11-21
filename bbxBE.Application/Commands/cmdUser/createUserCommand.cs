using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdUser
{
    public class createUserCommand : IRequest<Response<Users>>
    {
        [ColumnLabel("Név")]
        [Description("Név")]
        public string Name { get; set; }
        [ColumnLabel("E-mail")]
        [Description("E-mail")]
        public string Email { get; set; }
        [ColumnLabel("Login név")]
        [Description("Login név")]
        public string LoginName { get; set; }
        [ColumnLabel("Jelszó")]
        [Description("Jelszó")]
        public string Password { get; set; }
        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Comment { get; set; }
        [ColumnLabel("Aktív?")]
        [Description("Aktív?")]
        public bool Active { get; set; }

        [ColumnLabel("Raktár ID")]
        [Description("Alapértelmezett raktár ID")]
        public long? WarehouseID { get; set; }

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
