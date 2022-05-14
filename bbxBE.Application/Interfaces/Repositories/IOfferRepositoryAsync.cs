using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Application.Parameters;
using bbxBE.Application.Queries.qOffer;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Repositories
{
    public interface IOfferRepositoryAsync : IGenericRepositoryAsync<Offer>
    {
        Task<bool> SeedDataAsync(int rowCount);
        Task<Offer> AddOfferAsync(Offer p_Offer);
        Task<Offer> UpdateOfferAsync(Offer p_Offer, List<OfferLine> p_OfferLines, List<SummaryByVatRate> p_summaryByVatRate, List<AdditionalOfferData> p_additionalOfferData, List<AdditionalOfferLineData> p_additionalOfferLineData);
        
        Task<Entity> GetOfferAsync(GetOffer requestParameters);
         Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> QueryPagedOfferAsync(QueryOffer requestParameters);
        
    }
}