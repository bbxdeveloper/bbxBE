using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using bbxBE.Domain.Extensions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qUser
{
    public class GetUser : IRequest<Entity>
    {
        public long ID { get; set; }
        public string Fields { get; set; }
    }

    public class GetUserHandler : IRequestHandler<GetUser, Entity>
    {
        private readonly IUserRepositoryAsync _userRepository;

        public GetUserHandler(IUserRepositoryAsync userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Entity> Handle(GetUser request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;

            // query based on filter
            var entity = await _userRepository.GetUserAsync(validFilter.ID);

            var data = entity.MapItemFieldsByMapToAnnotation<GetUsersViewModel>();

            // response wrapper
            return data;
        }
    }
}