using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MaiXIFY.SpotifyWebAPIWrapper
{
    public static class SpotifyHelpers
    {
        public static string MakeWidgetSrc (string ownerId, string playlistId)
        {
            string embedWidgetSource = "https://embed.spotify.com/?uri=spotify";
            embedWidgetSource += ":user:";
            embedWidgetSource += ownerId;
            embedWidgetSource += ":playlist:";
            embedWidgetSource += playlistId;

            return embedWidgetSource;
        }


        public static string MakeWidgetSrc (string playlistUri)
        {
            return "https://embed.spotify.com/?uri=" + playlistUri;
        }


        public class RequestContentCreatePlaylist
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "public")]
            public bool IsPublic { get; set; }

            [JsonProperty(PropertyName = "collaborative")]
            public bool IsCollaborative { get; set; }
        }


        public class RequestContentAddTrackToPlaylist
        {
            [JsonProperty(PropertyName = "uris")]
            public List<string> TrackUriList { get; set; }
        }
    }
}
