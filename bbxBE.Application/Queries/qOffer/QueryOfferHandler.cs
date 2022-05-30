using AutoMapper;
using MediatR;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Parameters;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using bbxBE.Application.Interfaces.Queries;
using bbxBE.Domain.Extensions;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Common.Attributes;
using System.ComponentModel;
using System;

namespace bbxBE.Application.Queries.qOffer
{
    public class QueryOffer : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {

        [ColumnLabel("Ügyfél ID")]
        [Description("Ügyfél ID")]
        public long CustomerID { get; set; }

        [ColumnLabel("Ajánlat száma")]
        [Description("Ajánlat száma")]
        public string OfferNumber { get; set; }

        [ColumnLabel("Kelt.tól")]
        [Description("Kiállítás dátumától")]
        public DateTime? OfferIssueDateFrom { get; set; }

        [ColumnLabel("Kelt.ig")]
        [Description("Kiállítás dátumáig")]
        public DateTime? OfferIssueDateTo { get; set; }

        [ColumnLabel("Érvényesség tól")]
        [Description("Érvényesség dátumától")]
        public DateTime? OfferVaidityDateForm { get; set; }

        [ColumnLabel("Érvényességig")]
        [Description("Érvényesség dátumáig")]
        public DateTime? OfferVaidityDateTo { get; set; }

        //Teljes reációs szerkezet kell? I/N
        public bool FullData { get; set; } = true;
    }

    public class QueryOfferHandler : IRequestHandler<QueryOffer, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IOfferRepositoryAsync _OfferRepository;
        private readonly IMapper _mapper;
        private readonly IModelHelper _modelHelper;

        public QueryOfferHandler(IOfferRepositoryAsync OfferRepository, IMapper mapper, IModelHelper modelHelper)
        {
            _OfferRepository = OfferRepository;
            _mapper = mapper;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(QueryOffer request, CancellationToken cancellationToken)
        {
            var validFilter = request;
            var pagination = request;

            /* TODO: törölni
            //filtered fields security
            if (!string.IsNullOrEmpty(validFilter.Fields))
            {
                //limit to fields in view model
                validFilter.Fields = _modelHelper.ValidateModelFields<GetOfferViewModel, Offer>(validFilter.Fields);
            }
  
            if (string.IsNullOrEmpty(validFilter.Fields))
            {
                //default fields from view model
                validFilter.Fields = _modelHelper.GetQueryableFields<GetOfferViewModel, Offer>();
            }
            */


            // query based on filter
            var entities = await _OfferRepository.QueryPagedOfferAsync(validFilter);
            var data = entities.data.MapItemsFieldsByMapToAnnotation<GetOfferViewModel>();
            RecordsCount recordCount = entities.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, validFilter.PageNumber, validFilter.PageSize, recordCount);
        }
    }
}