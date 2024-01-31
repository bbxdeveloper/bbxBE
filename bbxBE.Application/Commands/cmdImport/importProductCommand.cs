using bbxBE.Application.Interfaces.Commands;
using bbxBE.Application.Wrappers;
using bbxBE.Common.Attributes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdImport
{
    public class ImportProductCommand : IRequest<Response<string>>
    {
        [ColumnLabel("Importf�jlok")]
        [Description("Importf�jlok: 1.Mapper, 2.CSV")]
        public List<IFormFile> ProductFiles { get; set; }

        [ColumnLabel("Mez�elv�laszt�")]
        [Description("Mez�elv�laszt�")]
        public string FieldSeparator { get; set; } = ";";

        [ColumnLabel("Csak �j t�tel felvitele?")]
        [Description("Csak �j t�tel felvitele?")]
        public bool OnlyInsert { get; set; } = false;

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
                await _pproc.Process(mapContent, CSVContent, request.FieldSeparator, request.SessionID, request.OnlyInsert, cancellationToken);
            }
            );
            var resp = new Response<String>("Productimport has been started");
            resp.Succeeded = true;
            return resp;
        }

    }
}
