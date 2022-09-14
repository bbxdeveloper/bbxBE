using bbxBE.Application.Commands.cmdCounter;
using bbxBE.Application.Commands.cmdUser;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Queries.qCounter;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdAuth;
using bxBE.Application.Commands.cmdCounter;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    //   [Authorize]
    public class AuthController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        public AuthController(IWebHostEnvironment env, IConfiguration conf)
        {
            _env = env;
            _conf = conf;
        }


        [Route("auth/v1/login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginCommand login)
        {
            return Ok(await Mediator.Send(login));
        }

#if (!DEBUG)
        [Authorize]
#else
        [AllowAnonymous]
#endif
        [Route("auth/v1/logout")]
        [HttpPost()]
        public IActionResult Logout()
        {
            throw new NotImplementedException();
        }
    }
}
