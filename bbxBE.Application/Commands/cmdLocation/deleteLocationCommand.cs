using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdLocation
{
    public class DeleteLocationCommand : IRequest<Response<long>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

    }

    public class DeleteLocationCommandHandler : IRequestHandler<DeleteLocationCommand, Response<long>>
    {
        private readonly ILocationRepositoryAsync _LocationRepository;
        private readonly IMapper _mapper;

        public DeleteLocationCommandHandler(ILocationRepositoryAsync LocationRepository, IMapper mapper)
        {
            _LocationRepository = LocationRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            await _LocationRepository.DeleteLocationAsync(request.ID);
            return new Response<long>(request.ID);
        }


    }
}
