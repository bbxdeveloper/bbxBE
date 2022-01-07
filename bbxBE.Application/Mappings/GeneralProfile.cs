using bbxBE.Application.Features.Employees.Queries.GetEmployees;
using bbxBE.Application.Features.Positions.Commands.CreatePosition;
using bbxBE.Application.Features.Positions.Queries.GetPositions;
using bbxBE.Domain.Entities;
using AutoMapper;

namespace bbxBE.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<Position, GetPositionsViewModel>().ReverseMap();
            CreateMap<Employee, GetEmployeesViewModel>().ReverseMap();
            CreateMap<CreatePositionCommand, Position>();
        }
    }
}