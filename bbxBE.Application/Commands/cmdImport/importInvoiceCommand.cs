using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdImport
{
    public class ImportInvoiceCommand : IRequest<Response<string>>
    {
        [ColumnLabel("Importfájl")]
        [Description("Importfájl")]
        public IFormFile CSVFile { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string WarehouseCode { get; set; }


    }

    public class ImportInvoiceCommandHandler : ProductMappingParser, IRequestHandler<ImportInvoiceCommand, Response<string>>
    {
        private readonly IInvoiceRepositoryAsync _InvoiceRepository;

        public ImportInvoiceCommandHandler(IInvoiceRepositoryAsync InvoiceRepository)
        {
            _InvoiceRepository = InvoiceRepository;
        }


        public async Task<Response<string>> Handle(ImportInvoiceCommand request, CancellationToken cancellationToken)
        {

            string CSVContent = "";
            using (var reader = new StreamReader(request.CSVFile.OpenReadStream(), Encoding.UTF8))
            {
                CSVContent = reader.ReadToEnd();
            }

            _InvoiceRepository.Import(CSVContent, request.WarehouseCode);

            var resp = new Response<String>("InvoiceImport finished");
            resp.Succeeded = true;
            return resp;
        }

    }
}
