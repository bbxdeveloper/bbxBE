using bbxBE.Application.Interfaces;
using System;

namespace bbxBE.Infrastructure.Shared.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}