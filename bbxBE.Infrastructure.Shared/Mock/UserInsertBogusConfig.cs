using Bogus;
using bbxBE.Domain.Entities;

namespace bbxBE.Infrastructure.Shared.Mock
{
    public class UserInsertBogusConfig : Faker<Users>
    {
        public UserInsertBogusConfig()
        {
            RuleFor(o => o.Name, f => f.Name.FirstName());
        }
    }
}