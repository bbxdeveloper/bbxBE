using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace bbxBE.Application.Queries.ViewModels
{
    public class GetEnumModel
    {

        [DataMemberAttribute]
        [JsonPropertyNameAttribute("value")]
        public string Value { get; set; }

        [DataMemberAttribute]
        [JsonPropertyNameAttribute("text")]
        public string Text { get; set; }

        [DataMemberAttribute]
        [JsonPropertyNameAttribute("icon")]
        public string Icon { get; set; }

        [DataMemberAttribute]                   //additional data
        [JsonPropertyNameAttribute("data")]
        public string Data { get; set; }

    }
}
