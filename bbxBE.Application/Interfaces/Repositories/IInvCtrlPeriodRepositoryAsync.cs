//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvCtrlPeriod;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IInvCtrlPeriodRepositoryAsync : IGenericRepositoryAsync<InvCtrlPeriod>
    {
        Task<bool> IsUniqueInvCtrlPeriodCodeAsync(string InvCtrlPeriodCode, long? ID = null);
        Task<bool> SeedDataAsync(int rowCount);
        Task<InvCtrlPeriod> GetInvCtrlPeriodByCodeAsync(string InvCtrlPeriodCode);

        Task<Entity> GetInvCtrlPeriodAsync(GetInvCtrlPeriod requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvCtrlPeriodAsync(QueryInvCtrlPeriod requestParameters);
    }
}