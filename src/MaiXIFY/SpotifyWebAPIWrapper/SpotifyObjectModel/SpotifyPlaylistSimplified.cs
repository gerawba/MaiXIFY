using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MaiXIFY.SpotifyWebAPIWrapper.SpotifyObjectModel
{
    public class SpotifyPlaylistSimplified
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "public")]
        public bool IsPublic { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public SpotifyUser OwnerUser { get; set; }

        [JsonProperty(PropertyName = "tracks")]
        public SimplifiedTrack Tracks { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }


        public class SimplifiedTrack
        {
            [JsonProperty(PropertyName = "href")]
            public string Href { get; set; }

            [JsonProperty(PropertyName = "total")]
            public int TotalItemNumbers { get; set; }
        }
    }
}
