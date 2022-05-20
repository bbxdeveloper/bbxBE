using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
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
        private Serilog.Core.Logger _loggerX;

        public ErrorLoggingMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;

             _loggerX = new LoggerConfiguration()
               .ReadFrom.Configuration(configuration)
               .WriteTo.Console()
               .CreateLogger();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _loggerX.Error(e, e.Message);
                System.Diagnostics.Debug.WriteLine($"The following error happened: {e.Message}");
                throw;
            }
        }
    }
}
