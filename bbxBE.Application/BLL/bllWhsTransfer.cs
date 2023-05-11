using AutoMapper;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Common.Consts;
using bbxBE.Common.Enums;
using bbxBE.Common.Exceptions;
using bbxBE.Domain.Entities;
using bxBE.Application.Commands.cmdWarehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.BLL
{
    public static class bllWhsTransfer
    {
        public static async Task<WhsTransfer> CreateWhsTransferAsynch(CreateWhsTransferCommand request,
                         IMapper mapper,
                         IWhsTransferRepositoryAsync whsTransferRepository,
                         IWarehouseRepositoryAsync warehouseRepository,
                         ICounterRepositoryAsync counterRepository,
                         IProductRepositoryAsync productRepository,
                         CancellationToken cancellationToken)
        {
            var whsTransfer = mapper.Map<WhsTransfer>(request);
            var counterCode = "";
            try
            {

                var fromWarehouse = await warehouseRepository.GetWarehouseByCodeAsync(request.FromWarehouseCode);
                if (fromWarehouse == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.FromWarehouseCode));
                }
                whsTransfer.FromWarehouseID = fromWarehouse.ID;



                var toWarehouse = await warehouseRepository.GetWarehouseByCodeAsync(request.ToWarehouseCode);
                if (toWarehouse == null)
                {
                    throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_WAREHOUSENOTFOUND, request.ToWarehouseCode));
                }
                whsTransfer.ToWarehouseID = toWarehouse.ID;

                //Tételsorok előfeldolgozása
                var lineErrors = new List<string>();
                foreach (var ln in whsTransfer.WhsTransferLines)
                {
                    var rln = request.WhsTransferLines.SingleOrDefault(i => i.WhsTransferLineNumber == ln.WhsTransferLineNumber);

                    var prod = productRepository.GetProductByProductCode(rln.ProductCode);
                    if (prod == null)
                    {
                        throw new ResourceNotFoundException(string.Format(bbxBEConsts.ERR_PRODCODENOTFOUND, rln.ProductCode));
                    }

                    ln.ProductID = prod.ID;
                    ln.ProductCode = rln.ProductCode;
                    //ln.Product = prod;

                }

                var prefix = "WHT";
                var whs = whsTransfer.FromWarehouseID.ToString().PadLeft(3, '0');
                counterCode = String.Format($"{prefix}_{whs}");
                whsTransfer.WhsTransferNumber = await counterRepository.GetNextValueAsync(counterCode, whsTransfer.FromWarehouseID);
                whsTransfer.Copies = 1;
                whsTransfer.WhsTransferStatus = enWhsTransferStatus.READY.ToString();

                await whsTransferRepository.AddWhsTransferAsync(whsTransfer);
                await counterRepository.FinalizeValueAsync(counterCode, whsTransfer.FromWarehouseID, whsTransfer.WhsTransferNumber);
                return whsTransfer;

            }
            catch (Exception)
            {
                if (!string.IsNullOrWhiteSpace(whsTransfer.WhsTransferNumber) && !string.IsNullOrWhiteSpace(counterCode))
                {
                    await counterRepository.RollbackValueAsync(counterCode, whsTransfer.FromWarehouseID, whsTransfer.WhsTransferNumber);
                }
                throw;
            }
            return null;
        }
    }
}
