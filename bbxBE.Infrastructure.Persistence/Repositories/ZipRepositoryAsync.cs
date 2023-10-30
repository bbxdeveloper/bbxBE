using AutoMapper;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Repositories
{
    public class ZipRepositoryAsync : GenericRepositoryAsync<Zip>, IZipRepositoryAsync
    {
        private readonly IApplicationDbContext _dbContext;
        private IDataShapeHelper<Zip> _dataShaperZip;
        private IDataShapeHelper<GetZipViewModel> _dataShaperGetZipViewModel;
        private readonly IMockService _mockData;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;


        public ZipRepositoryAsync(IApplicationDbContext dbContext,
            IModelHelper modelHelper, IMapper mapper, IMockService mockData) : base(dbContext)
        {
            _dbContext = dbContext;
            _dataShaperZip = new DataShapeHelper<Zip>();
            _dataShaperGetZipViewModel = new DataShapeHelper<GetZipViewModel>();
            _modelHelper = modelHelper;
            _mapper = mapper;
            _mockData = mockData;
        }

        public async Task<Zip> GetCityByZip(string zipCode)
        {
            Zip Zip = await _dbContext.Zip
                    .Where(x => x.ZipCode == zipCode && !x.Deleted)
                    .OrderBy(x => x.ZipCity)
                    .FirstOrDefaultAsync();

            return Zip;
        }
        public async Task<Zip> GetZipByCity(string zipCity)
        {
            Zip Zip = await _dbContext.Zip
                    .Where(x => x.ZipCity.ToUpper() == zipCity.ToUpper() && !x.Deleted)
                    .OrderBy(o => o.ZipCode)
                    .FirstOrDefaultAsync();

            return Zip;
        }

        public Task<bool> SeedDataAsync(int rowCount)
        {
            throw new System.NotImplementedException();
        }

    }
}