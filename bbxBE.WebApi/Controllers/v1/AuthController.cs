using bbxBE.Application.Commands.cmdCounter;
using bbxBE.Application.Commands.cmdUser;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Queries.qCounter;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdCounter;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
 //   [Authorize]
    public class AuthController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        public AuthController( IWebHostEnvironment env, IConfiguration conf)
        {
            _env = env;
            _conf = conf;
        }


        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetCounter filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryCounter filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateCounterCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        // POST: USRController/Edit/5
        [HttpPut]
 //       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateCounterCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        // POST: USRController/Edit/5
        [HttpPatch]
        //       [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetNextNumber(GetNextNumberCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // GET: USRController/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteCounterCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
