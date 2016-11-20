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

        public const string accessTokenCookieKey = "accessToken";
        public const string refreshTokenCookieKey = "refreshToken";
        public const string expiresInKey = "expiresIn";
        public const string tokenObtainedKey = "tokenObtained";

        public const string thresholdSettingCookieKey = "thresholdSetting";
        public const string recommendedMusicSettingCookieKey = "recommendedMusicSetting";
        public const string sortOptionSettingCookieKey = "sortOptionSetting";

        public const string stateCookieKey = "state";

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


        public static DateTime ParseDateTimeString (string dateString)
        {
            string[] splitOne = dateString.Split ('-');
            if (splitOne.Length != 3)
                return new DateTime ();

            string[] splitTwo = splitOne[2].Split ('T');
            if (splitTwo.Length != 2)
                return new DateTime ();

            string[] splitThree = splitTwo[1].Split (':');
            if (splitThree.Length != 3)
                return new DateTime ();

            string[] splitFour = splitThree[2].Split('.');
            if (splitFour.Length != 2)
                return new DateTime ();

            int year = int.Parse (splitOne[0]);
            int month = int.Parse (splitOne[1]);
            int day = int.Parse (splitTwo[0]);
            int hour = int.Parse (splitThree[0]);
            int minute = int.Parse (splitThree[1]);
            int second = int.Parse (splitFour[0]);

            DateTime date = new DateTime (year, month, day, hour, minute, second);

            return date;
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
