using bbxBE.Application.Commands.cmdProduct;
using bbxBE.Application.Commands.cmdUSR_USER;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Queries.qEnum;
using bbxBE.Application.Queries.qProduct;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdProduct;
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
    public class ProductController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        public ProductController( IWebHostEnvironment env, IConfiguration conf)
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
        public async Task<IActionResult> Get([FromQuery] GetProduct filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("productbycode")]
        public async Task<IActionResult> GetProductByProductCode([FromQuery] GetProductByProductCode filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] QueryProduct filter)
        {
            return Ok(await Mediator.Send(filter));
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
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        // POST: USRController/Edit/5
        [HttpPut]
 //       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateProductCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // GET: USRController/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteProductCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
