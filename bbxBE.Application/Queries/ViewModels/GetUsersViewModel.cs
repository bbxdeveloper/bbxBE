using bbxBE.Common.Attributes;
using System.ComponentModel;
using System.Runtime.Serialization;

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


        #region UserLevel

        [DataMember]
        [ColumnLabel("Szint")]
        [Description("Szint")]
        public string UserLevel { get; set; }

        [ColumnLabel("Szint")]
        [Description("Szint megnevezés")]
        [DataMember]
        [NotDBField]
        public string UserLevelX { get; set; }
        #endregion


        [MapToEntity("Email")]
        public string Email { get; set; }

        [MapToEntity("Comment")]
        public string Comment { get; set; }

        [MapToEntity("Active")]
        public bool Active { get; set; }

        [ColumnLabel("Raktár ID")]
        [Description("Raktár ID")]
        public long WarehouseID { get; set; }

        [ColumnLabel("Raktár")]
        [Description("Raktár")]
        public string Warehouse { get; set; }

    }
}
