using bbxBE.Domain.Entities;
using System.Collections.Generic;

namespace bbxBE.Application.Interfaces
{
    public interface IMockService
    {
        List<Users> GetUsers(int rowCount);

    }
}