using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace bbxBE.Application.Queries.ViewModels
{
    public class GetEnumModel
    {

        [DataMemberAttribute]
        [JsonProperty("value")]
        public string Value { get; set; }

        [DataMemberAttribute]
        [JsonProperty("text")]
        public string Text { get; set; }

        [DataMemberAttribute]
        [JsonProperty("icon")]
        public string Icon { get; set; }

        [DataMemberAttribute]                   //additional data
        [JsonProperty("data")]
        public string Data { get; set; }

    }
}
