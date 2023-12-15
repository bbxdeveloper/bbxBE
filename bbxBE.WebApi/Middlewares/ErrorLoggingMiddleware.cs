using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using ILogger = Serilog.ILogger;

namespace bbxBE.WebApi.Middlewares
{
    //https://blog.elmah.io/asp-net-core-request-logging-middleware/
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger _logger;

        public ErrorLoggingMiddleware(RequestDelegate next, IConfiguration configuration, ILogger logger)
        {
            _next = next;
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
                _logger.Error(e, e.Message);
                System.Diagnostics.Debug.WriteLine($"The following error happened: {e.Message}");
                throw;
            }
        }
    }
}
