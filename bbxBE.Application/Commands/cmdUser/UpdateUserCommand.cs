using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdUser
{

    public class UpdateUserCommand : IRequest<Response<Users>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

        [ColumnLabel("Név")]
        [Description("Név")]
        public string Name { get; set; }
        [ColumnLabel("Szint")]
        [Description("Szint")]
        public string UserLevel { get; set; }
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

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Response<Users>>
    {
        private readonly IUserRepositoryAsync _usrRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateUserCommandHandler(IUserRepositoryAsync usrRepository, IMapper mapper, IConfiguration configuration)
        {
            _usrRepository = usrRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<Users>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {

            var usr = _mapper.Map<Users>(request);

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                //Jelszóváltozás
                var salt = Environment.GetEnvironmentVariable(bbxBEConsts.ENV_PWDSALT);
                usr.PasswordHash = bllUser.GetPasswordHash(request.Password, _configuration.GetValue<string>(salt));
            }
            else
            {
                var oriUsr = await _usrRepository.GetUserRecordByIDAsync(request.ID);
                if (oriUsr == null)
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_USERNOTFOUND, request.ID));
                usr.PasswordHash = oriUsr.PasswordHash;
            }




            await _usrRepository.UpdateAsync(usr);
            return new Response<Users>(usr);
        }


    }
}
