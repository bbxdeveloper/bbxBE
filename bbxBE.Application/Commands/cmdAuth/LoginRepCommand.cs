using bbxBE.Application.BLL;
using bbxBE.Application.Commands.ResultModels;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Consts;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdAuth
{
    public class LoginRepCommand : IRequest<Response<LoginInfo>>
    {

        public string Username { get; set; }

        public string Password { get; set; }


    }


    public class LoginRepCommandHandler : IRequestHandler<LoginRepCommand, Response<LoginInfo>>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public LoginRepCommandHandler(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Response<LoginInfo>> Handle(LoginRepCommand request, CancellationToken cancellationToken)
        {

            var JWTSettings = _configuration.GetSection(bbxBEConsts.CONF_JWTSettings);
            var JWTKey = JWTSettings.GetValue<string>(bbxBEConsts.CONF_JWTKey);
            var JWTIssuer = JWTSettings.GetValue<string>(bbxBEConsts.CONF_JWTIssuer);
            var JWTAudience = JWTSettings.GetValue<string>(bbxBEConsts.CONF_JWTAudience);
            var JWTDurationInMinutes = JWTSettings.GetValue<double>(bbxBEConsts.CONF_JWTDurationInMinutes);

            var usr = new Users() { Name = "rep" };

            var token = BllAuth.GenerateJSONWebToken(usr, JWTKey, JWTIssuer, JWTAudience, JWTDurationInMinutes);

            return new Response<LoginInfo>(new LoginInfo() { Token = token, User = usr });
        }


    }
}
