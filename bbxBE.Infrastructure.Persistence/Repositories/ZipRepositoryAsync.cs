using Microsoft.EntityFrameworkCore;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using bbxBE.Infrastructure.Persistence.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System;
using AutoMapper;
using bbxBE.Application.Queries.ViewModels;

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