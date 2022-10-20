
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qProductGroup;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IProductCodeRepositoryAsync : IGenericRepositoryAsync<ProductCode>
    {
        Task<ICollection<ProductCode>> MaintainProductCodeListAsync(ICollection<ProductCode> p_lstOldProductCodes, ICollection<ProductCode> p_lstNewProductCodes);
    }
}