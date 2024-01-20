using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qUser;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IUserRepositoryAsync : IGenericRepositoryAsync<Users>
    {
        Task<bool> IsUniqueNameAsync(string Name, long? ID = null);

        Task<bool> SeedDataAsync(int rowCount);

        Task<Entity> GetUserAsync(long ID);
        Task<Users> GetUserRecordByIDAsync(long ID);
        Task<Users> GetUserRecordByNameAsync(string name);
        Task<Users> GetUserRecordByLoginNameAsync(string name);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedUserAsync(QueryUser requestParameters);
        Task<Entity> GetUserByLoginNameAndPwd(string LoginName, string Password);


    }
}