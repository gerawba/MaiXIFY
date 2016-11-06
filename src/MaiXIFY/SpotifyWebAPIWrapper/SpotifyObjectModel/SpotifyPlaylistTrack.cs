using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MaiXIFY.SpotifyWebAPIWrapper.SpotifyObjectModel
{
    public class SpotifyPlaylistTrack
    {
        [JsonProperty(PropertyName = "track")]
        public SpotifyTrack Track { get; set; }

        [JsonProperty(PropertyName = "is_local")]
        public bool IsLocal { get; set; }
    }
}
