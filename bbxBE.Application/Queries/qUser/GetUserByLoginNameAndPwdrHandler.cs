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
using bbxBE.Common.Attributes;
using System.ComponentModel;

namespace bbxBE.Application.Queries.qUser
{
    public class GetUserByLoginNameAndPwd :  IRequest<Entity>
    {
        [ColumnLabel("Login név")]
        [Description("Login név")]
        public string LoginName { get; set; }

        [ColumnLabel("Jelszó")]
        [Description("Jelszó")]
        public string Password { get; set; }
    }

    public class GetUserByLoginNameAndPwdHandler : IRequestHandler<GetUserByLoginNameAndPwd, Entity>
    {
        private readonly IUserRepositoryAsync _userRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetUserByLoginNameAndPwdHandler(IUserRepositoryAsync userRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetUserByLoginNameAndPwd request, CancellationToken cancellationToken)
        {
      
            // query based on filter
            var entity = await _userRepository.GetUserByLoginNameAndPwd(request.LoginName, request.Password);


            var data = entity.MapItemFieldsByMapToAnnotation<GetUsersViewModel>();

            // response wrapper
            return data;
        }
    }
}