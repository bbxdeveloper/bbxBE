﻿using bbxBE.Application.Parameters;
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
        Task<InvCtrlPeriod> UpdateInvCtrlPeriodAsync(InvCtrlPeriod p_invCtrlPeriod);
        Task<InvCtrlPeriod> DeleteInvCtrlPeriodAsync(long ID);
        Task<bool> CanDeleteAsync(long ID);
        Task<bool> CanCloseAsync(long ID);
        Task<bool> CanUpdateAsync(long ID);
        Task<Entity> GetInvCtrlPeriodAsync(GetInvCtrlPeriod requestParameter);
        Task<InvCtrlPeriod> GetInvCtrlPeriodRecordAsync(long ID);
        Task<bool> IsOverLappedPeriodAsync(DateTime DateFrom, DateTime DateTo, long? ID, long WarehouseID);
        Task<bool> SeedDataAsync(int rowCount);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedInvCtrlPeriodAsync(QueryInvCtrlPeriod requestParameters);
        Task<bool> CloseAsync(long ID);

    }
}