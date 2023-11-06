using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace bxBE.Application.Commands.cmdProductGroup
{
    public class UpdateProductGroupCommand : IRequest<Response<ProductGroup>>
    {
        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }
        [ColumnLabel("Temékcsoport kód")]
        [Description("Temékcsoport kód")]
        public string ProductGroupCode { get; set; }

        [ColumnLabel("Temékcsoport megnevezés")]
        [Description("Temékcsoport megnevezés")]
        public string ProductGroupDescription { get; set; }

        [ColumnLabel("Mimimum árrés")]
        [Description("Mimimum árrés")]
        public decimal MinMargin { get; set; }
    }

    public class UpdateProductGroupCommandHandler : IRequestHandler<UpdateProductGroupCommand, Response<ProductGroup>>
    {
        private readonly IProductGroupRepositoryAsync _ProductGroupRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateProductGroupCommandHandler(IProductGroupRepositoryAsync ProductGroupRepository, IMapper mapper, IConfiguration configuration)
        {
            _ProductGroupRepository = ProductGroupRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<ProductGroup>> Handle(UpdateProductGroupCommand request, CancellationToken cancellationToken)
        {
            var pg = _mapper.Map<ProductGroup>(request);
            await _ProductGroupRepository.UpdateProductGroupAsync(pg);
            return new Response<ProductGroup>(pg);
        }


    }
}
