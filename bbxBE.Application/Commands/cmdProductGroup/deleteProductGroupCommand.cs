using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdProductGroup
{
    public class DeleteProductGroupCommand : IRequest<Response<long>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

    }

    public class DeleteProductGroupCommandHandler : IRequestHandler<DeleteProductGroupCommand, Response<long>>
    {
        private readonly IProductGroupRepositoryAsync _ProductGroupRepository;
        private readonly IMapper _mapper;

        public DeleteProductGroupCommandHandler(IProductGroupRepositoryAsync ProductGroupRepository, IMapper mapper)
        {
            _ProductGroupRepository = ProductGroupRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteProductGroupCommand request, CancellationToken cancellationToken)
        {
            await _ProductGroupRepository.DeleteProductGroupAsync(request.ID);
            return new Response<long>(request.ID);
        }


    }
}
