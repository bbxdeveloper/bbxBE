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
using bbxBE.Application.Queries.qZip;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Exceptions;
using bbxBE.Common.Consts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections;
using Org.BouncyCastle.Asn1.X500;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ZipRepositoryAsync : GenericRepositoryAsync<Zip>, IZipRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private IDataShapeHelper<Zip> _dataShaperZip;
        private IDataShapeHelper<GetZipViewModel> _dataShaperGetZipViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;


        public ZipRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Zip> dataShaperZip,
            IDataShapeHelper<GetZipViewModel> dataShaperGetZipViewModel,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperZip = dataShaperZip;
            _dataShaperGetZipViewModel = dataShaperGetZipViewModel;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }

         public async Task<Zip> GetCityBzZip(string zipCode)
        {
            Zip Zip = await _dbContext.Zip
                    .Where(x => x.ZipCode == zipCode && !x.Deleted)
                    .FirstOrDefaultAsync();

            return Zip;
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

    }
}