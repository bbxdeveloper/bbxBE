using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Wrappers;
using bbxBE.Commands.cmdUSR_USER;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using static bbxBE.Commands.cmdUSR_USER.createUSR_USERCommand;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
 //   [Authorize]
    public class USRController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IRequestHandler<createUSR_USERCommand, Response<long>> _USRCommandHandler;
        public USRController(
           IWebHostEnvironment env,
           IConfiguration conf,
           IRequestHandler<createUSR_USERCommand, Response<long>> USRCommandHandler)
        {
            _env = env;
            _conf = conf;
            _USRCommandHandler = USRCommandHandler;
    }


        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] IGetUSR_USERQuery filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(createUSR_USERCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // GET: USRController/Details/5
        [HttpGet("details/{id}")]
        public ActionResult Details(int id)
        {
            return Ok();
        }

        // POST: USRController/Create
        [HttpPost("create")]
        public async Task<ActionResult> Create(createUSR_USERCommand req )
        {
            
            return Ok(await Mediator.Send(_USRCommandHandler.Handle(req, default)));

        }

        // POST: USRController/Edit/5
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            return Ok();
        }

        // GET: USRController/Delete/5
        [HttpDelete("delete/{id}")]
        public ActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
