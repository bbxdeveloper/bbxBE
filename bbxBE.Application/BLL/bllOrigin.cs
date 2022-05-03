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

        public static async Task<long> CreateRangeAsync(List<Origin> originList, IOriginRepositoryAsync _originRepository, CancellationToken cancellationToken)
        {
            return await _originRepository.AddOriginRangeAsync(originList);
        }

        public static async Task<long> CreateOriginRangeByStringAsync(List<string> originList, IOriginRepositoryAsync _originRepository, CancellationToken cancellationToken)
        {
            var oList = new List<Origin>();
            foreach (var origin in originList)
            {
                oList.Add(new Origin { OriginCode = origin, OriginDescription = origin});
            }
            return await _originRepository.AddOriginRangeAsync(oList);
        }
    }
}
