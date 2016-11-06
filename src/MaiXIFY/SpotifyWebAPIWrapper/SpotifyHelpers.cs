using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaiXIFY.SpotifyWebAPIWrapper
{
    public static class SpotifyHelpers
    {
        public static string MakeWidgetSrc(string ownerId, string playlistId)
        {
            string embedWidgetSource = "https://embed.spotify.com/?uri=spotify";
            embedWidgetSource += ":user:";
            embedWidgetSource += ownerId;
            embedWidgetSource += ":playlist:";
            embedWidgetSource += playlistId;

            return embedWidgetSource;
        }

        public static string MakeWidgetSrc(string playlistUri)
        {
            return "https://embed.spotify.com/?uri=" + playlistUri;
        }
    }
}
