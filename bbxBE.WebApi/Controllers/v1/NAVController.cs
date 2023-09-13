using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.ExpiringData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    //   [Authorize]
    public class NAVController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IProductRepositoryAsync _productRepositoryAsync;
        private readonly IExpiringData<ExpiringDataObject> _expiringData;
        public NAVController(IWebHostEnvironment env, IConfiguration conf,
            IProductRepositoryAsync productRepositoryAsync,
             IExpiringData<ExpiringDataObject> expiringData)
        {
            _env = env;
            _conf = conf;
            _productRepositoryAsync = productRepositoryAsync;
            _expiringData = expiringData;
        }


        //   [Authorize]
        [HttpGet("gettoken")]
        public async Task<IActionResult> GetToken()
        {
            return Ok(true);
        }

    }
}
