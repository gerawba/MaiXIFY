using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MaiXIFY.SpotifyWebAPIWrapper.SpotifyObjectModel
{
    public class SpotifyTrack
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "artists")]
        public List<SpotifyArtist> Artists { get; set; }

        [JsonProperty(PropertyName = "popularity")]
        public int Popularity { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }


        public class SpotifyTracksList
        {
            [JsonProperty(PropertyName = "tracks")]
            public List<SpotifyTrack> Tracks { get; set; }
        }
    }
}
