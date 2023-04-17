using bbxBE.Application.Interfaces.Commands;
using bbxBE.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdImport
{
    public class ImportProductCommand : IRequest<Response<string>>
    {
        public List<IFormFile> ProductFiles { get; set; }
        public string FieldSeparator { get; set; } = ";";

        [JsonIgnore]
        public string SessionID { get; set; }

    }

    public class ImportProductCommandHandler : ProductMappingParser, IRequestHandler<ImportProductCommand, Response<string>>
    {
        private readonly IImportProductProc _pproc;


        public ImportProductCommandHandler(IImportProductProc pproc)
        {
            _pproc = pproc;
        }


        public async Task<Response<string>> Handle(ImportProductCommand request, CancellationToken cancellationToken)
        {

            string mapContent = "";
            using (var reader = new StreamReader(request.ProductFiles[0].OpenReadStream()))
            {
                mapContent = reader.ReadToEnd();
            }
            string CSVContent = "";
            using (var reader = new StreamReader(request.ProductFiles[1].OpenReadStream()))
            {
                CSVContent = reader.ReadToEnd();
            }

            _ = Task.Run(async () =>
            {
                await _pproc.Process(mapContent, CSVContent, request.FieldSeparator, request.SessionID, cancellationToken);
            }
            );
            var resp = new Response<String>("Productimport has been started");
            resp.Succeeded = true;
            return resp;
        }

    }
}
