using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MaiXIFY.SpotifyWebAPIWrapper.SpotifyObjectModel
{
    public class SpotifyUser
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty (PropertyName = "display_name")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }
    }
}
