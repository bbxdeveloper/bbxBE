using AutoMapper;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Attributes;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qInvoice
{
    public class GetInvoiceXChangeXML : IRequest<FileStreamResult>
    {

        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }

    }

    public class GetInvoiceXChangeXMLHandler : IRequestHandler<GetInvoiceXChangeXML, FileStreamResult>
    {
        private readonly INAVXChangeRepositoryAsync _navXChangeRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public GetInvoiceXChangeXMLHandler(INAVXChangeRepositoryAsync navXChangeRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _navXChangeRepository = navXChangeRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<FileStreamResult> Handle(GetInvoiceXChangeXML request, CancellationToken cancellationToken)
        {

            var xChange = await _navXChangeRepository.GetByIdAsync(request.ID);
            if (xChange == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_XCHANGEFOUND, request.ID));
            }

            string fileName = $"NAVXChange_{xChange.InvoiceNumber}.xml";

            var enc = Encoding.GetEncoding(bbxBEConsts.DEF_ENCODING);
            Stream stream = new MemoryStream(enc.GetBytes(xChange.InvoiceXml));
            var fsr = new FileStreamResult(stream, $"application/xml") { FileDownloadName = fileName };

            return fsr;
        }
    }
}