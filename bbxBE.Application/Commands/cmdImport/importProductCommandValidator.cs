using bbxBE.Application.Commands.cmdImport;
using bbxBE.Application.Consts;
using bbxBE.Application.Enums;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Application.Wrappers;
using bxBE.Application.Commands.cmdProduct;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdProduct
{

    public class importProductCommandValidator : AbstractValidator<ImportProductCommand>
    {
        private readonly IProductRepositoryAsync _ProductRepository;

        public importProductCommandValidator()
        {
            RuleFor(f => f.ProductFiles).NotNull().WithMessage(bbxBEConsts.ERR_FILELISTISNULL).NotEmpty().WithMessage(bbxBEConsts.ERR_FILELISTISEMPTY);

            RuleFor(f => f.ProductFiles).Must(CheckFileCount).WithMessage(bbxBEConsts.ERR_FILELISTCONUTER);

            RuleFor(f => f.ProductFiles).Must(CheckFileLength).WithMessage(bbxBEConsts.ERR_FILESIZE);
        }

        private bool CheckFileCount(List<IFormFile> productFiles)
        {
            return productFiles.Count.Equals(2);
        }

        private bool CheckFileLength(List<IFormFile> productFiles)
        {
            return (!productFiles[0].Length.Equals(0)) && (!productFiles[1].Length.Equals(0));
        }
    }
}