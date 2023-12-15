using bbxBE.Application.Interfaces.Commands;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace bbxBE.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(IgnoreApi = true)] // hide from swagger
    public class TestController : BaseApiController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _conf;
        private readonly IImportProductProc _pproc;

        private readonly Serilog.ILogger _logger;
        private readonly ILogger<TestController> _msSerilogILogger;

        public TestController(IWebHostEnvironment env, IConfiguration conf, IImportProductProc pproc, Serilog.ILogger logger, ILogger<TestController> msSerilogILogger)
        {
            _env = env;
            _conf = conf;
            _pproc = pproc;

            _logger = logger;
            _msSerilogILogger = msSerilogILogger;
        }

        /// <summary>
        /// GET: api/controller
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Uses Serilog.ILogger interface
            _logger.Information("information log 1 Serilog.ILogger");
            // Uses Serilog but with Microsoft.Extension.Logging.ILogger interface
            _msSerilogILogger.LogInformation("information log Serilog with Microsoft.Extension.Logging.ILogger");
            // Uses Serilog, no DI
            Serilog.Log.Information("information log 3 Serilog.Log");

            return Ok();
        }
    }
}
