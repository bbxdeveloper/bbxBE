using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdProduct
{
    public class DeleteProductCommand : IRequest<Response<long>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Response<long>>
    {
        private readonly IProductRepositoryAsync _ProductRepository;
        private readonly IMapper _mapper;

        public DeleteProductCommandHandler(IProductRepositoryAsync ProductRepository, IMapper mapper)
        {
            _ProductRepository = ProductRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var res = await _ProductRepository.DeleteProductAsync(request.ID);
            return new Response<long>(request.ID);
        }


    }
}
