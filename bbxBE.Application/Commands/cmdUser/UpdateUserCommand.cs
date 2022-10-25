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
using bbxBE.Common.Exceptions;

namespace bbxBE.Application.Commands.cmdUser
{

    public class UpdateUserCommand : IRequest<Response<Users>>
    {
        public long ID { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string LoginName { get; set; }

        public string Password { get; set; }
        public string Comment { get; set; }
        public bool Active { get; set; }

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
                usr.PasswordHash = bllUser.GetPasswordHash(request.Password, _configuration.GetValue<string>(bbxBEConsts.CONF_PwdSalt));
            }
            else
            {
                var oriUsr = await _usrRepository.GetUserRecordByIDAsync(request.ID);
                if( oriUsr == null)
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_USERNOTFOUND, request.ID));
                usr.PasswordHash = oriUsr.PasswordHash;
            }




            await _usrRepository.UpdateAsync(usr);
            return new Response<Users>(usr);
        }


    }
}
