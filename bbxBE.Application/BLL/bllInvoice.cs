using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Enums;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdInvoice;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.BLL
{

      public static class bllInvoice
    {
        public static async Task<Invoice> CreateAsynch(createOutgoingInvoiceCommand request,
                   IInvoiceRepositoryAsync _InvoiceRepository, IMapper _mapper, CancellationToken cancellationToken)
        {
            return new Invoice();
        }

        public static string GetCounterCode(enInvoiceType p_nvoiceType)
        {
            return "";  
        }
    }
}
