using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qOffer;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdOffer;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IOfferRepositoryAsync : IGenericRepositoryAsync<Offer>
    {
        Task<bool> SeedDataAsync(int rowCount);
        Task<Offer> AddOfferAsync(Offer p_Offer);
        Task<Offer> UpdateOfferAsync(Offer p_Offer);
        Task<Offer> UpdateOfferRecordAsync(Offer p_Offer);
        Task<Offer> DeleteOfferAsync(long ID);

        Task<Entity> GetOfferAsync(long ID, bool FullData = true);
        Task<Offer> GetOfferRecordAsync(long ID);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedOfferAsync(QueryOffer requestParameters);

        Task<Offer> CreateOfferAsync(CreateOfferCommand request, CancellationToken cancellationToken);
        Task<Offer> ModifyOfferAsync(UpdateOfferCommand request, CancellationToken cancellationToken);




    }
}