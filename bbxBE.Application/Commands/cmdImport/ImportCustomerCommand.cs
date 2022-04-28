using AutoMapper;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdImport
{
    public class ImportCustomerCommand : IRequest<Response<ImportedItemsStatistics>>
    {
        public List<IFormFile> ProductFiles { get; set; }
        public string FieldSeparator { get; set; } = ";";
    }

    public class ImportCustomerCommandHandler : CustomerMappingParser, IRequestHandler<ImportCustomerCommand, Response<ImportedItemsStatistics>>
    {
        private const string DescriptionFieldName = "Description";
        private const string ProductGroupCodeFieldName = "ProductGroupCode";
        private const string OriginCodeFieldName = "OriginCode";
        private const string UnitOfMeasureFieldName = "UnitOfMeasure";
        private const string UnitPrice1FieldName = "UnitPrice1";
        private const string UnitPrice2FieldName = "UnitPrice2";
        private const string LatestSupplyPriceFieldName = "LatestSupplyPrice";
        private const string IsStockFieldName = "IsStock";
        private const string MinStockFieldName = "MinStock";
        private const string OrdUnitFieldName = "OrdUnit";
        private const string ProductFeeFieldName = "ProductFee";
        private const string ActiveFieldName = "Active";
        private const string ProductCodeFieldName = "ProductCode";
        private const string EANFieldName = "EAN";
        private const string VTSZFieldName = "VTSZ";

        private readonly ICustomerRepositoryAsync _customerRepository;
        //private readonly IUnitOfMEasure
        private readonly IMapper _mapper;
        private readonly ILogger _logger;



        public ImportCustomerCommandHandler(ICustomerRepositoryAsync customerRepository,
  
                                            IMapper mapper,
                                            ILogger<ImportProductCommandHandler> logger)
        {
            _customerRepository =  customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<ImportedItemsStatistics>> Handle(ImportCustomerCommand request, CancellationToken cancellationToken)
        {
            return new Response<ImportedItemsStatistics>();
        }
    }
}
