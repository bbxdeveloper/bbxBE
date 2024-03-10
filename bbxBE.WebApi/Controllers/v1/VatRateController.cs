﻿using bbxBE.Application.Queries.qVatRate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
#if (!DEBUG)
    [Authorize]
#else
    [AllowAnonymous]
#endif
    public class VatRateController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        public VatRateController(IWebHostEnvironment env, IConfiguration conf)
        {
            _env = env;
            _conf = conf;
        }


        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetVatRate request)
        {
            return Ok(await Mediator.Send(request));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryVatRate request)
        {
            return Ok(await Mediator.Send(request));
        }

        /*
        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("VatRatebycode")]
        public async Task<IActionResult> GetVatRateByVatRateCode([FromQuery] GetVatRateByVatRateCode request)
        {
            return Ok(await Mediator.Send(request));
        }

      


        [HttpGet("unitofmeasure")]
        public async Task<IActionResult> GetUnitOfMeasure()
        {
            var req = new GetEnum() {  type = typeof(enUnitOfMeasure) };
            return Ok(await Mediator.Send(req));
        }

        /// <summary>
        /// POST api/controller
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateVatRateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        // POST: USRController/Edit/5
        [HttpPut]
 //       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateVatRateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // GET: USRController/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteVatRateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        */
    }
}
