//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qInvCtrl;
using bbxBE.Application.Queries.qStock;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IInvCtrlRepositoryAsync : IGenericRepositoryAsync<InvCtrl>
    {
        Task<InvCtrl> AddInvCtrlAsync(InvCtrl p_InvCtrl);
        Task<InvCtrl> UpdateInvCtrlAsync(InvCtrl p_InvCtrl);
        Task<bool> AddOrUpdateRangeInvCtrlAsync(List<InvCtrl> p_InvCtrl);
        Task<bool> AddRangeInvCtrlICCAsync(List<InvCtrl> p_InvCtrl, string p_XRel);

        Task<InvCtrl> DeleteInvCtrlAsync(long ID);
        Task<bool> CheckInvCtrlDateAsync(long InvCtlPeriodID, DateTime InvCtrlDate);

        Task<Entity> GetInvCtrl(GetInvCtrl requestParameter);
        Task<Entity> GetLastestInvCtrlICC(GetLastestInvCtrlICC requestParameter);
        Task<InvCtrl> GetInvCtrlICPRecordAsync(long InvCtlPeriodID, long ProductID);
        Task<List<InvCtrl>> GetInvCtrlICPRecordsByPeriodAsync(long InvCtlPeriodID);

        Task<bool> SeedDataAsync(int rowCount);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryInvCtrlStockAbsentAsync(QueryInvCtrlStockAbsent requestParameters);

        Task<(IEnumerable<GetInvCtrlViewModel> data, RecordsCount recordsCount)> QueryPagedInvCtrlViewModelAsync(QueryInvCtrl requestParameter);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvCtrlAsync(QueryInvCtrl requestParameters);

    }
}