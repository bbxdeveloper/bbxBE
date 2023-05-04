﻿using AutoMapper;
using MediatR;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Domain.Extensions;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Application.Commands.cmdImport;

namespace bbxBE.Application.Queries.qCustomer
{
    public class GetCustomer:  IRequest<Entity>
    {
        public long ID { get; set; }
  //      public string Fields { get; set; }
    }

    public class GetCustomerHandler : IRequestHandler<GetCustomer, Entity>
    {
        private readonly ICustomerRepositoryAsync _customerRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetCustomerHandler(ICustomerRepositoryAsync customerRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<Entity> Handle(GetCustomer request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;
          
            // query based on filter
            var entity = _customerRepository.GetCustomer(validFilter.ID);
            var data = entity.MapItemFieldsByMapToAnnotation<GetCustomerViewModel>();

            // response wrapper
            return data;
        }
    }
}