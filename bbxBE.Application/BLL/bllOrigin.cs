using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.BLL
{
    public static class bllOrigin
    {

        public static async Task<Origin> CreateAsync(string originCode, string originDescription,
                          IOriginRepositoryAsync _originRepository, CancellationToken cancellationToken)
        {
            Origin origin = await _originRepository.AddAsync(new Origin
            {
                OriginCode = originCode,
                OriginDescription = originDescription
            });
            return origin;
        }

    }
}
