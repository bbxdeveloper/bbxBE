using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class CounterRepositoryAsyncC : CounterRepositoryAsync, ICounterRepositoryAsyncC
    {

        public CounterRepositoryAsyncC(IApplicationCommandDbContext dbContext,
            IDataShapeHelper<Counter> dataShaperCounter,
            IDataShapeHelper<GetCounterViewModel> dataShaperGetCounterViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData)
            : base(dbContext, dataShaperCounter, dataShaperGetCounterViewModel, modelHelper, mapper, mockData)
        {

        }

    }
}