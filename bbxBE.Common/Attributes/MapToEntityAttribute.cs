using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbxBE.Common.Attributes
{
    public class MapToEntityAttribute : Attribute
    {
        public string MatchingName { get; set; }

        public MapToEntityAttribute(string _matchingName)
        {
            MatchingName = _matchingName;
        }
    }
}
