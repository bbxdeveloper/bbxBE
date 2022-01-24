using Bogus;
using bbxBE.Domain.Entities;

namespace bbxBE.Infrastructure.Shared.Mock
{
    public class USR_USERInsertBogusConfig : Faker<USR_USER>
    {
        public USR_USERInsertBogusConfig()
        {
            RuleFor(o => o.USR_NAME, f => f.Name.FirstName());
        }
    }
}