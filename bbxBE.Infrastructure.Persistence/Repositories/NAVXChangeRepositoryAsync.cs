using AutoMapper;
using bbxBE.Application.BLL;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Common.NAV;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class NAVXChangeRepositoryAsync : GenericRepositoryAsync<NAVXChange>, INAVXChangeRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public NAVXChangeRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }

        public async Task<IList<NAVXChange>> GetXChangeRecordsByStatus(enNAVStatus NAVStatus, int itemCnt)
        {
            var q = _dbContext.NAVXChange.Where(w => w.Status == NAVStatus.ToString()).AsNoTracking();
            return await q.Take(itemCnt).ToListAsync();
        }

        public async Task<IList<NAVXChange>> GetXChangeRecordByInvoiceID(long invoiceID)
        {
            var q = _dbContext.NAVXChange.Where(w => w.InvoiceID == invoiceID).AsNoTracking();
            return await q.ToListAsync();
        }
        public async Task<NAVXChange> CreateNAVXChangeForManageInvoiceAsynch(Invoice invoice, CancellationToken cancellationToken)
        {
            if (invoice == null)
            {
                throw new ResourceNotFoundException(bbxBEConsts.ERR_INVOICEISNULL);
            }
            if (invoice.Incoming || invoice.InvoiceType != enInvoiceType.INV.ToString())
            {
                throw new NAVException(string.Format(bbxBEConsts.ERR_NAVINV, (invoice.InvoiceNumber)));
            }
            var currXChanges = await GetXChangeRecordByInvoiceID(invoice.ID);
            if (currXChanges.Any(a => a.Status == enNAVStatus.CREATED.ToString()))
            {
                throw new NAVException(string.Format(bbxBEConsts.ERR_INVOICEALREADYSETSEND, (invoice.InvoiceNumber)));

            }

            var resNAVXChange = new NAVXChange();

            var invoiceNAVXML = bllInvoice.GetInvoiceNAVXML(invoice);
            resNAVXChange.InvoiceID = invoice.ID;
            resNAVXChange.InvoiceNumber = invoice.InvoiceNumber;
            resNAVXChange.InvoiceXml = XMLUtil.Object2XMLString<InvoiceData>(invoiceNAVXML, Encoding.UTF8, NAVGlobal.XMLNamespaces);
            resNAVXChange.Operation = enNAVOperation.MANAGEINVOICE.ToString();
            resNAVXChange.Status = enNAVStatus.CREATED.ToString();
            await _dbContext.NAVXChange.AddAsync(resNAVXChange);
            await _dbContext.SaveChangesAsync();

            return resNAVXChange;

        }


    }
}