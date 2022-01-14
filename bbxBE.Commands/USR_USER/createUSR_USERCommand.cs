using bbxBE.Application.Wrappers;
using bbxBE.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Commands.USR_USER
{
    public class createUSR_USERCommand : IRequest<Response<long>>
    {
        public string USR_NAME { get; set; }
        public string USR_EMAIL { get; set; }
        public string USR_LOGIN { get; set; }
        public string USR_PASSWDHASH { get; set; }
        public string USR_COMMENT { get; set; }
        public bool USR_ACTIVE { get; set; }

        public class createUSR_USERCommandHandler : IRequestHandler<createUSR_USERCommand, Response<long>>
        {
            private readonly DapperContext _context;
            private readonly IConfiguration _conf;
            public createUSR_USERCommandHandler(DapperContext context, IConfiguration conf)
            {
                _context = context;
                _conf = conf;
            }
 

            public Task<Response<long>> Handle(createUSR_USERCommand request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException("Asd");
            }
        }
    }
 
}
