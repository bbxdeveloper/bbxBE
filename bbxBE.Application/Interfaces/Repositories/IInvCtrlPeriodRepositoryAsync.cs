//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvCtrlPeriod;
using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IInvCtrlPeriodRepositoryAsync : IGenericRepositoryAsync<InvCtrlPeriod>
    {
        Task<InvCtrlPeriod> AddInvCtrlPeriodAsync(InvCtrlPeriod p_invCtrlPeriod);
        Task<Entity> GetInvCtrlPeriodAsync(GetInvCtrlPeriod requestParameter);

        Task<bool> IsOverLappedPeriodAsync(DateTime DateFrom, DateTime DateTo);
        Task<bool> SeedDataAsync(int rowCount);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvCtrlPeriodAsync(QueryInvCtrlPeriod requestParameters);
    }
}