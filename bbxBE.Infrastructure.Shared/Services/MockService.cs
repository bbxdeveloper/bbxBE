using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Shared.Mock;
using System.Collections.Generic;

namespace bbxBE.Infrastructure.Shared.Services
{
    public class MockService : IMockService
    {
        public List<Users> GetUsers(int rowCount)
        {
    
            var usrFaker = new UserInsertBogusConfig();
            return usrFaker.Generate(rowCount);
 
            }

    }
}