using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdUser
{
    public class DeleteUserCommand : IRequest<Response<long>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Response<long>>
    {
        private readonly IUserRepositoryAsync _userRepository;
        private readonly IMapper _mapper;

        public DeleteUserCommandHandler(IUserRepositoryAsync usreRepository, IMapper mapper)
        {
            _userRepository = usreRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var usr = _mapper.Map<Users>(request);
            await _userRepository.RemoveAsync(usr);
            return new Response<long>(usr.ID);
        }


    }
}
