using bbxBE.Infrastructure.Persistence.Contexts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Commands.USR_USER
{
    internal class createUSR_USERCommand : IRequest<long>
    {
        public string USR_NAME { get; set; }
        public string USR_EMAIL { get; set; }
        public string USR_LOGIN { get; set; }
        public string USR_PASSWDHASH { get; set; }
        public string USR_COMMENT { get; set; }
        public bool USR_ACTIVE { get; set; }

        public class createUSR_USERCommandHandler : IRequestHandler<createUSR_USERCommand, long>
        {
            private readonly DapperContext _context;
            public CreateProductCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(CreateProductCommand command, CancellationToken cancellationToken)
            {
                var product = new Product();
                product.Barcode = command.Barcode;
                product.Name = command.Name;
                product.Rate = command.Rate;
                product.Description = command.Description;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product.Id;
            }
        }
    }

}
}
