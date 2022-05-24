﻿using AutoMapper.Configuration.Conventions;
using bbxBE.Common.Attributes;
using System;


namespace bbxBE.Application.Queries.ViewModels
{
    /// <summary>
    /// MapTo properties marks the names in the output Entity
    /// Don't use with AutoMapper, but with <see cref="Domain.Extensions.EntityExtensions.MapFieldsByMapToAnnotation"/>
    /// In this case, <see cref="GetProductGroupViewModel"/> will be the value for the TDestination parameter.
    /// </summary>
    public class GetProductGroupViewModel
    {
        [MapToEntityAttribute("ID")]
        public long ID { get; set; }

        public string ProductGroupCode { get; set; }

        public string ProductGroupDescription { get; set; }
    }
}
