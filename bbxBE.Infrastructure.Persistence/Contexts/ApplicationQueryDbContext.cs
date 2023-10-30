using bbxBE.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace bbxBE.Infrastructure.Persistence.Contexts
{
    public class ApplicationQueryDbContext : ApplicationDbContext, IApplicationQueryDbContext
    {
        public ApplicationQueryDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeService dateTime, ILoggerFactory loggerFactory)
            : base(options, dateTime, loggerFactory)
        {
        }
    }
}