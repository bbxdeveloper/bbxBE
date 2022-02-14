using bbxBE.Application.Parameters;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Interfaces.Queries
{

    public interface IGetCustomerRQuery : IQueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string CustomerName { get; set; }
        public string TaxPayerId { get; set; }
    }
}
