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
    public class OfferLineRepositoryAsync : GenericRepositoryAsync<OfferLine>, IOfferLineRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;

        public OfferLineRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<ProductGroup> dataShaperProductGroup,
            IDataShapeHelper<GetProductGroupViewModel> dataShaperGetProductGroupViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }
    }
}