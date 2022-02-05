using bbxBE.Application.Parameters;
using bbxBE.Application.Wrappers;
using bbxBE.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Interfaces.Queries
{

    public interface IGetUSR_USERQuery : IQueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string USR_LOGIN { get; set; }
    }
}
