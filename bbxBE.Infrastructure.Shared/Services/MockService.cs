using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Shared.Mock;
using System.Collections.Generic;

namespace bbxBE.Infrastructure.Shared.Services
{
    public class MockService : IMockService
    {
        public List<USR_USER> GetUsers(int rowCount)
        {
    
            var usrFaker = new USR_USERInsertBogusConfig();
            return usrFaker.Generate(rowCount);
 
            }

    }
}