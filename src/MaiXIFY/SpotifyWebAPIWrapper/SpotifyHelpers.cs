using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MaiXIFY.SpotifyWebAPIWrapper
{
    public static class SpotifyHelpers
    {
        const string basePlaylistName = "MaiXIFY :) ";
        public const string trackUri = "spotify:track:";
        public const string thresholdSettingCookieKey = "thresholdSetting";
        public const string recommendedMusicSettingCookieKey = "recommendedMusicSetting";
        public const string sortOptionSettingCookieKey = "sortOptionSetting";
        public const string stateCookieKey = "stateKey";

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


        public static string MakeDefaultPlaylistName ()
        {
            string playlistName = basePlaylistName;
            DateTime date = DateTime.Now;

            return playlistName + "[" + date.Year.ToString().Remove(0, 2) + date.Month.ToString() + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() + date.Second.ToString() + "]";
        }


        public class TrackInfo
        {
            public int HitCount { get; set; }
            public int Popularity { get; set; }
        }


        public class SelectedPlaylistElem
        {
            [JsonProperty(PropertyName = "userId")]
            public string UserId { get; set; }

            [JsonProperty(PropertyName = "playlistId")]
            public string PlaylistId { get; set; }
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
