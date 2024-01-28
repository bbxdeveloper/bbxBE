using bbxBE.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace bbxBE.Infrastructure.Persistence.Contexts
{
    public class ApplicationGlobalDbContext : ApplicationDbContext, IApplicationGlobalDbContext
    {
        public ApplicationGlobalDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeService dateTime, ILoggerFactory loggerFactory)
            : base(options, dateTime, loggerFactory)
        {
        }
    }
}
