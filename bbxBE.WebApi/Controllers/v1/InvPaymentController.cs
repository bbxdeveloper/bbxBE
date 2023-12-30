﻿using bbxBE.Application.Queries.qLocation;
using bxBE.Application.Commands.cmdLocation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    //   [Authorize]
    public class InvPaymentController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IHttpContextAccessor _context;

        public InvPaymentController(IWebHostEnvironment env, IConfiguration conf, IHttpContextAccessor context)
        {
            _env = env;
            _conf = conf;
            _context = context;
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateInvPaymentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryInvPayment request)
        {
            return Ok(await Mediator.Send(request));
        }


        /*
         * 
        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetInvCtrl filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("record")]
        public async Task<IActionResult> GetRecord([FromQuery] GetInvCtrlICPRecord req)
        {
            return Ok(await Mediator.Send(req));
        }

        [HttpGet("getlatesticc")]
        public async Task<IActionResult> GetLastestInvCtrlICC([FromQuery] GetLastestInvCtrlICC req)
        {
            return Ok(await Mediator.Send(req));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryInvCtrl filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("creicp")]
        public async Task<IActionResult> Create(createInvCtrlICPCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("creicc")]
        public async Task<IActionResult> Create(createInvCtrlICCCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("report")]
        public async Task<IActionResult> Print(PrintInvCtrlCommand command)
        {

            command.baseURL = $"{_context.HttpContext.Request.Scheme.ToString()}://{_context.HttpContext.Request.Host.ToString()}";
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(result.FileStream, "application/octet-stream", result.FileDownloadName); // returns a FileStreamResult
        }



        //átalaktani
        [HttpGet("csv")]
        public async Task<IActionResult> CSV([FromQuery] CSVInvCtrl command)
        {
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(result.FileStream, "application/octet-stream", result.FileDownloadName); // returns a FileStreamResult
        }
        */
    }
}
