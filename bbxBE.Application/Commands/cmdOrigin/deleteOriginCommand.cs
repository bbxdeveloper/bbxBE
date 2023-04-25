using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdOrigin
{
    public class DeleteOriginCommand : IRequest<Response<long>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

    }

    public class DeleteOriginCommandHandler : IRequestHandler<DeleteOriginCommand, Response<long>>
    {
        private readonly IOriginRepositoryAsync _OriginRepository;
        private readonly IMapper _mapper;

        public DeleteOriginCommandHandler(IOriginRepositoryAsync OriginRepository, IMapper mapper)
        {
            _OriginRepository = OriginRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteOriginCommand request, CancellationToken cancellationToken)
        {
            await _OriginRepository.DeleteOriginAsync(request.ID);
            return new Response<long>(request.ID);
        }


    }
}
