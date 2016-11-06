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
        public string Track { get; set; }
    }
}
