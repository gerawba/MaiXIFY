using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MaiXIFY.SpotifyWebAPIWrapper.SpotifyObjectModel
{
    public class SpotifyUserProfile
    {
        [JsonProperty (PropertyName = "display_name")]
        public string DisplayName { get; set; }
        [JsonProperty (PropertyName = "id")]
        public string Id { get; set; }
    }
}
