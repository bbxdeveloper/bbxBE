﻿using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common;
using bbxBE.Common.Consts;
using bbxBE.Common.Exceptions;
using bbxBE.Common.ExpiringData;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Queries.qInvoice
{
    public class GetInvoiceNAVXML : IRequest<FileStreamResult>
    {
        public long ID { get; set; }

    }

    public class GetInvoiceNAVXMLHandler : IRequestHandler<GetInvoiceNAVXML, FileStreamResult>
    {
        private readonly IInvoiceRepositoryAsync _invoiceRepository;
        private readonly IInvoiceLineRepositoryAsync _InvoiceLineRepository;
        private readonly ICounterRepositoryAsync _CounterRepository;
        private readonly IWarehouseRepositoryAsync _WarehouseRepository;
        private readonly ICustomerRepositoryAsync _CustomerRepository;
        private readonly IProductRepositoryAsync _ProductRepository;
        private readonly IVatRateRepositoryAsync _VatRateRepository;
        private readonly IExpiringData<ExpiringDataObject> _expiringData;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetInvoiceNAVXMLHandler(IInvoiceRepositoryAsync invoiceRepository,
            IInvoiceLineRepositoryAsync InvoiceLineRepository,
            ICounterRepositoryAsync CounterRepository,
            IWarehouseRepositoryAsync WarehouseRepository,
            ICustomerRepositoryAsync CustomerRepository,
            IProductRepositoryAsync ProductRepository,
            IVatRateRepositoryAsync VatRateRepository,
            IExpiringData<ExpiringDataObject> expiringData,
            IMapper mapper, IConfiguration configuration)

        {
            _invoiceRepository = invoiceRepository;
            _InvoiceLineRepository = InvoiceLineRepository;
            _CounterRepository = CounterRepository;
            _WarehouseRepository = WarehouseRepository;
            _CustomerRepository = CustomerRepository;
            _ProductRepository = ProductRepository;
            _VatRateRepository = VatRateRepository;
            _expiringData = expiringData;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<FileStreamResult> Handle(GetInvoiceNAVXML request, CancellationToken cancellationToken)
        {

            var invoice = await _invoiceRepository.GetInvoiceRecordAsync(request.ID, true);
            if (invoice == null)
            {
                throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_INVOICENOTFOUND, request.ID));
            }

            Invoice originalInvoice = null;
            if (invoice.InvoiceCorrection)
            {
                originalInvoice = await _invoiceRepository.GetInvoiceRecordAsync(invoice.OriginalInvoiceID.Value, true);
                if (originalInvoice == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_ORIGINALINVOICENOTFOUND, invoice.OriginalInvoiceID.Value));
                }
            }
            var invoiceNAVXML = bllInvoice.GetInvoiceNAVXML(invoice, cancellationToken);
            var xmlStr = XMLUtil.Object2XMLString<InvoiceData>(invoiceNAVXML, Encoding.UTF8, NAVGlobal.XMLNamespaces);

            // response wrapper
            Random rnd = new Random();
            string fileName = $"{invoice.InvoiceNumber}_{rnd.Next()}.xml";
            var enc = Encoding.GetEncoding(bbxBEConsts.DEF_ENCODING);
            Stream stream = new MemoryStream(enc.GetBytes(xmlStr));
            var fsr = new FileStreamResult(stream, $"application/xml") { FileDownloadName = fileName };

            return fsr;
        }
    }
}