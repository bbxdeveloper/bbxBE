using AutoMapper.Configuration.Conventions;

namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetUSR_USERViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetUSR_USERViewModel
    {
        [MapTo("ID")]
        public string ID { get; set; }

        [MapTo("Name")]
        public string USR_NAME { get; set; }

        [MapTo("LoginName")]
        public string USR_LOGIN { get; set; }

        [MapTo("Email")]
        public string USR_EMAIL { get; set; }

        [MapTo("Comment")]
        public string USR_COMMENT { get; set; }

        [MapTo("Active")]
        public bool USR_ACTIVE { get; set; }
    }
}
