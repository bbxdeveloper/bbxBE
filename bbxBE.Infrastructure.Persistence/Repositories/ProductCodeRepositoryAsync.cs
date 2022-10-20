using LinqKit;
using Microsoft.EntityFrameworkCore;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using bbxBE.Infrastructure.Persistence.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.BLL;
using System;
using AutoMapper;
using bbxBE.Application.Queries.qProductGroup;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using bbxBE.Infrastructure.Persistence.Caches;
using static bbxBE.Common.NAV.NAV_enums;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ProductCodeRepositoryAsync : GenericRepositoryAsync<ProductCode>, IProductCodeRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<ProductGroup> _dataShaperProductGroup;
        private IDataShapeHelper<GetProductGroupViewModel> _dataShaperGetProductGroupViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public ProductCodeRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<ProductGroup> dataShaperProductGroup,
            IDataShapeHelper<GetProductGroupViewModel> dataShaperGetProductGroupViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperProductGroup = dataShaperProductGroup;
            _dataShaperGetProductGroupViewModel = dataShaperGetProductGroupViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }

        public async Task<ICollection<ProductCode>> MaintainProductCodeListAsync(ICollection<ProductCode> p_lstOldProductCodes, ICollection<ProductCode> p_lstNewProductCodes)
        {

            foreach (var pcx in p_lstNewProductCodes)
            {
                pcx.ProductCodeValue = pcx.ProductCodeValue.ToUpper();
            }

            // ID-k összevadászása
            //termékkód
            var pc = p_lstNewProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString());
            if (pc != null)
            {
                var pcID = p_lstOldProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.OWN.ToString())?.ID;
                if (pcID != null)
                    pc.ID = pcID.Value;
            }

            var vtsz = p_lstNewProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString());
            if (vtsz != null)
            {
                var vtszID = p_lstOldProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.VTSZ.ToString())?.ID;
                if (vtszID != null)
                    vtsz.ID = vtszID.Value;
            }



            var eanUpd = p_lstNewProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString());
            if (eanUpd != null)
            {
                var eanOrig = p_lstOldProductCodes.SingleOrDefault(x => x.ProductCodeCategory == enCustproductCodeCategory.EAN.ToString());
                if (eanOrig != null)
                    eanUpd.ID = eanOrig.ID;
                else
                {

                    //ez nem akar működni egyelőre nem kell
                    //_dbContext.Entry(eanOrig).State = EntityState.Deleted;

                    //_dbContext.ProductCode.Remove(eanOrig);
                }
            }

            await RemoveRangeAsync(p_lstOldProductCodes.Where(w=> !p_lstNewProductCodes.Any( a=>a.ID == w.ID)).ToList(), false);
            await AddRangeAsync(p_lstNewProductCodes.Where(w => !p_lstOldProductCodes.Any(a => a.ID == w.ID)).ToList(), false);
            await UpdateRangeAsync(p_lstNewProductCodes.Where(w => p_lstOldProductCodes.Any(a => a.ID == w.ID)).ToList(), true);

            return p_lstNewProductCodes;
        }
    }
}