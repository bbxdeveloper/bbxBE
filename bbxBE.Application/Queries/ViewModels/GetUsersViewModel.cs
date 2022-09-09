using AutoMapper.Configuration.Conventions;
using AutoMapper;
using bbxBE.Common.Attributes;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetUsersViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetUsersViewModel
    {
        [MapToEntity("ID")]
        public long ID { get; set; }

        [MapToEntity("Name")]
        public string Name { get; set; }

        [MapToEntity("LoginName")]
        public string LoginName { get; set; }

        [MapToEntity("Email")]
        public string Email { get; set; }

        [MapToEntity("Comment")]
        public string Comment { get; set; }

        [MapToEntity("Active")]
        public bool Active { get; set; }
    }
}
