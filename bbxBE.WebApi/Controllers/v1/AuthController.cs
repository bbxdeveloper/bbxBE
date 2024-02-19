using bxBE.Application.Commands.cmdAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AuthController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        public AuthController(IWebHostEnvironment env, IConfiguration conf)
        {
            _env = env;
            _conf = conf;
        }


        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginCommand login)
        {
            return Ok(await Mediator.Send(login));
        }


#if (!DEBUG)
        //       [Authorize]
#else
        [AllowAnonymous]
#endif
        [Route("logout")]
        [HttpPost()]
        public IActionResult Logout()
        {
            // throw new NotImplementedException();
            return Ok();
        }
    }
}
