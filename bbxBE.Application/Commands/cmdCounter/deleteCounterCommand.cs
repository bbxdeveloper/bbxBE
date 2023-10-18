using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdCounter
{
    public class DeleteCounterCommand : IRequest<Response<long>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

    }

    public class DeleteCounterCommandHandler : IRequestHandler<DeleteCounterCommand, Response<long>>
    {
        private readonly ICounterRepositoryAsyncC _CounterRepository;
        private readonly IMapper _mapper;

        public DeleteCounterCommandHandler(ICounterRepositoryAsyncC CounterRepository, IMapper mapper)
        {
            _CounterRepository = CounterRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteCounterCommand request, CancellationToken cancellationToken)
        {
            var pg = _mapper.Map<Counter>(request);
            await _CounterRepository.RemoveAsync(pg);
            return new Response<long>(pg.ID);
        }
    }
}
