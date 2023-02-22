using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Attributes;
using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IInvoiceLineRepositoryAsync : IGenericRepositoryAsync<InvoiceLine>
    {
        Task<Dictionary<long, InvoiceLine>> GetInvoiceLineRecordsAsync(List<long> LstInvoiceLineID);
        Task<List<InvoiceLine>> GetInvoiceLineRecordsByRelDeliveryNoteIDAsync(long InvoiceID, long RelDeliveryNoteInvoiceID);
        Task<List<Entity>> GetInvoiceLinesByRelDeliveryNoteIDAsync(long InvoiceID, long RelDeliveryNoteInvoiceID);
    }
}