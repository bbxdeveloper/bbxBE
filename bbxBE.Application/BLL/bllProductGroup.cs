using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.BLL
{
    public static class bllProductGroup
    {
        public static async Task<ProductGroup> CreateAsync(string productGroupName, string productGroupDescription,
                          IProductGroupRepositoryAsync _productGroupRepository, CancellationToken cancellationToken)
        {
            ProductGroup productGroup = await _productGroupRepository.AddProudctGroupAsync(new ProductGroup
            {
                ProductGroupCode = productGroupName,
                ProductGroupDescription = productGroupDescription
            });
            return productGroup;
        }
    }
}
