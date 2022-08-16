﻿using bbxBE.Application.Commands.cmdUSR_USER;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.qStock;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
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
    public class SystemController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IProductRepositoryAsync _productRepositoryAsync;
         public SystemController( IWebHostEnvironment env, IConfiguration conf, IProductRepositoryAsync productRepositoryAsync)
        {
            _env = env;
            _conf = conf;
            _productRepositoryAsync = productRepositoryAsync;   
        }


        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("RefreshCaches")]
        public async Task<IActionResult> RefreshCaches()
        {
        // https://docs.microsoft.com/en-us/answers/questions/481854/load-controller-at-startup-web-api-core.html

            _productRepositoryAsync.RefreshProductCache(true);
            return Ok(true);
        }
    }
}