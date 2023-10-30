using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdWhsTransfer
{
    public class DeleteWhsTransferCommand : IRequest<Response<WhsTransfer>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

    }

    public class DeleteWhsTransferCommandHandler : IRequestHandler<DeleteWhsTransferCommand, Response<WhsTransfer>>
    {
        private readonly IWhsTransferRepositoryAsync _WhsTransferRepository;
        private readonly IMapper _mapper;

        public DeleteWhsTransferCommandHandler(IWhsTransferRepositoryAsync WhsTransferRepository, IMapper mapper)
        {
            _WhsTransferRepository = WhsTransferRepository;
            _mapper = mapper;
        }

        public async Task<Response<WhsTransfer>> Handle(DeleteWhsTransferCommand request, CancellationToken cancellationToken)
        {
            var res = await _WhsTransferRepository.DeleteWhsTransferAsync(request.ID);
            return new Response<WhsTransfer>(res);
        }


    }
}
