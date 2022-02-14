using AutoMapper.Configuration.Conventions;
using bbxBE.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace bbxBE.Domain.Extensions
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Maps a list of entity by renaming the properties based on the values of the <see cref="MapToAttribute"/> annotations.
        /// </summary>
        /// <typeparam name="TDestination">Type of eg. viewmodel</typeparam>
        /// <param name="pData">Entities to map</param>
        /// <returns></returns>
        public static IEnumerable<Entity> MapItemsFieldsByMapToAnnotation<TDestination>(this IEnumerable<Entity> pData)
        {
            var mapped = new List<Entity>();
            foreach (Entity m in pData)
            {
                mapped.Add(m.MapItemFieldsByMapToAnnotation<TDestination>());
            }
            return mapped;
        }
        /// <summary>
        /// Maps an entity by renaming the properties based on the values of the <see cref="MapToAttribute"/> annotations.
        /// </summary>
        /// <typeparam name="TDestination">Type of eg. viewmodel</typeparam>
        /// <param name="pData">Entity to map</param>
        /// <returns></returns>
        public static Entity MapItemFieldsByMapToAnnotation<TDestination>(this Entity pData)
        {
            // Entity for result
            var mapped = new Entity();

            // Type of eg. GetUSR_USERViewModel
            var typeOfTDestination = typeof(TDestination);

            // Don't install AutoMapper v11, v8.1.1 is perfectly fine.
            // This class can't be found in v11.
            var typeOfMapToAttribute = typeof(MapToAttribute);

            foreach (string propertyName in pData.Keys)
            {
                // Get eg. "USR_NAME" property
                var attrs = typeOfTDestination.GetProperty(propertyName);

                var newPropertyName = propertyName;
                // Get [MapTo("...")] attribute
                var mapToAttribute = (MapToAttribute)attrs.GetCustomAttributes(typeOfMapToAttribute, false).FirstOrDefault();
                if (mapToAttribute != null)
                {
                    // Get value from attribute
                    newPropertyName = mapToAttribute.MatchingName;
                }
                // Build mapped Entity
                mapped.Add(newPropertyName, pData[propertyName]);
            }

            // Returning result
            return mapped;
        }
    }
}
