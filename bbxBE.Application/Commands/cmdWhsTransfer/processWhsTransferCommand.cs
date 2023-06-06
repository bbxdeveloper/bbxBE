﻿using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdWhsTransfer
{
    public class ProcessWhsTransferCommand : IRequest<Response<WhsTransfer>>
    {

        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

        [ColumnLabel("Bevétel dátuma")]
        [Description("Bevétel dátuma")]
        public DateTime TransferDateIn { get; set; }

    }


    public class ProcessWhsTransferCommandHandler : IRequestHandler<ProcessWhsTransferCommand, Response<WhsTransfer>>
    {
        private readonly IWhsTransferRepositoryAsync _whsTransferRepositoryAsync;
        private readonly IWarehouseRepositoryAsync _warehouseRepositoryAsync;
        private readonly ICounterRepositoryAsync _counterRepository;
        private readonly IProductRepositoryAsync _productRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ProcessWhsTransferCommandHandler(
            IWhsTransferRepositoryAsync whsTransferRepositoryAsync,
            IWarehouseRepositoryAsync warehouseRepositoryAsync,
            ICounterRepositoryAsync counterRepository,
            IProductRepositoryAsync productRepository,
            IMapper mapper, IConfiguration configuration)
        {
            _whsTransferRepositoryAsync = whsTransferRepositoryAsync;
            _warehouseRepositoryAsync = warehouseRepositoryAsync;
            _counterRepository = counterRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<WhsTransfer>> Handle(ProcessWhsTransferCommand request, CancellationToken cancellationToken)
        {
            var res = await _whsTransferRepositoryAsync.ProcessAsync(request.ID, request.TransferDateIn);
            return new Response<WhsTransfer>(res);
        }

    }
}
