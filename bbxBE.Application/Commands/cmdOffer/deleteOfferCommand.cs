using AutoMapper;
using AutoMapper.Configuration.Conventions;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdOffer
{
    public class DeleteOfferCommand : IRequest<Response<long>>
    {
        public long ID { get; set; }

    }

    public class DeleteOfferCommandHandler : IRequestHandler<DeleteOfferCommand, Response<long>>
    {
        private readonly IOfferRepositoryAsync _OfferRepository;
        private readonly IMapper _mapper;

        public DeleteOfferCommandHandler(IOfferRepositoryAsync OfferRepository, IMapper mapper)
        {
            _OfferRepository = OfferRepository;
            _mapper = mapper;
        }

        public async Task<Response<long>> Handle(DeleteOfferCommand request, CancellationToken cancellationToken)
        {
            await _OfferRepository.DeleteOfferAsync(request.ID);
            return new Response<long>(request.ID);
        }

      
    }
}
