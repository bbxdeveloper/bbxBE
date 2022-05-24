using AutoMapper.Configuration.Conventions;
using AutoMapper;
using bbxBE.Common.Attributes;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapToEntity properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetUSR_USERViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetUSR_USERViewModel
    {
        [MapToEntity("ID")]
        public long ID { get; set; }

        [MapToEntity("Name")]
        public string USR_NAME { get; set; }

        [MapToEntity("LoginName")]
        public string USR_LOGIN { get; set; }

        [MapToEntity("Email")]
        public string USR_EMAIL { get; set; }

        [MapToEntity("Comment")]
        public string USR_COMMENT { get; set; }

        [MapToEntity("Active")]
        public bool USR_ACTIVE { get; set; }
    }
}
