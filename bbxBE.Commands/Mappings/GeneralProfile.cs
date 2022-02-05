using AutoMapper;
using bbxBE.Commands.cmdUSR_USER;
using bbxBE.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static bbxBE.Commands.cmdUSR_USER.CreateUSR_USERCommand;

namespace bbxBE.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<CreateUSR_USERCommand, USR_USER>();
            CreateMap<UpdateUSR_USERCommand, USR_USER>();
            CreateMap<DeleteUSR_USERCommand, USR_USER>();
        }
    }
}
