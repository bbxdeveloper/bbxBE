﻿//using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qProductGroup;
using bbxBE.Application.Queries.qVatRate;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IVatRateRepositoryAsync : IGenericRepositoryAsync<VatRate>
    {
    
        Task<Entity> GetVatRateAsync(GetVatRate requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedVatRateAsync(QueryVatRate requestParameters);
    }
}