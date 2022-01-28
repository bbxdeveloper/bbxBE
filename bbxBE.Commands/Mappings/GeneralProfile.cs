using AutoMapper;
using bbxBE.Commands.cmdUSR_USER;
using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static bbxBE.Commands.cmdUSR_USER.createUSR_USERCommand;

namespace bbxBE.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<createUSR_USERCommand, USR_USER>();
        }
    }
}
