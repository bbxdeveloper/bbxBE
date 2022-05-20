using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
//using ILogger = Serilog.ILogger;

namespace bbxBE.WebApi.Middlewares
{
    //https://blog.elmah.io/asp-net-core-request-logging-middleware/
    public class ErrorLoggingMiddlewareNemjo
    {
        private readonly RequestDelegate _next;
        //private Microsoft.Extensions.Logging.ILogger _logger;
        private ILogger _logger;
        //        private Serilog.Core.Logger _loggerX;
        //private Serilog.Core.Logger _logger;

        public ErrorLoggingMiddlewareNemjo(RequestDelegate next, /* IConfiguration configuration, */ ILogger logger)
        {
            _next = next;
          //  _logger = loggerFactory.CreateLogger<ErrorLoggingMiddleware>();
          /*
             _loggerX = new LoggerConfiguration()
               .ReadFrom.Configuration(configuration)
               .WriteTo.Console()
               .CreateLogger();
          */
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
            //    _loggerX.Error(e, e.Message);
                _logger.LogError(new Exception("tttttt"), "UUU");

                System.Diagnostics.Debug.WriteLine($"The following error happened: {e.Message}");
                throw;
            }
        }
    }
}
