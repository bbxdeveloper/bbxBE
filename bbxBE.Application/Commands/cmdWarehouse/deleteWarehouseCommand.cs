using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdWarehouse
{
    public class DeleteWarehouseCommand : IRequest<Response<long>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

    }

    public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, Response<long>>
    {
        private readonly IWarehouseRepositoryAsync _WarehouseRepository;
        private readonly IMapper _mapper;

        public DeleteWarehouseCommandHandler(IWarehouseRepositoryAsync WarehouseRepository, IMapper mapper)
        {
            _WarehouseRepository = WarehouseRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
        {
            await _WarehouseRepository.DeleteWarehouseAsync(request.ID);
            return new Response<long>(request.ID);
        }


    }
}
