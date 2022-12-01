
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IZipRepositoryAsync : IGenericRepositoryAsync<Zip>
    {
        Task<Zip> GetCityBzZip(string zipCode);
    }
}