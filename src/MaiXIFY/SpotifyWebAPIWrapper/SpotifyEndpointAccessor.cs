using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MaiXIFY.SpotifyWebAPIWrapper.SpotifyObjectModel;
using Newtonsoft.Json;

namespace MaiXIFY.SpotifyWebAPIWrapper
{
    public class SpotifyEndpointAccessor : ISpotifyEndpointAccessor
    {
        private static string baseUrl = "https://api.spotify.com";
        private static string usersUrl = "/v1/users/";

        public static string AuthorizationHeader
        {
            get 
            {
                if (SpotifyWebAPIWrapper.SpotifyAuthorization.AccessToken == null)
                    return "Bearer ";
                else
                    return "Bearer " + SpotifyWebAPIWrapper.SpotifyAuthorization.AccessToken;
            }
        }


        public SpotifyUser GetCurrentUserProfile ()
        {
            var client = new HttpClient ();
            client.DefaultRequestHeaders.Add ("Authorization", AuthorizationHeader);

            var response = client.GetAsync (baseUrl + "/v1/me").Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            var userProfile = JsonConvert.DeserializeObject<SpotifyUser> (responseContent);

            return userProfile;
        }


        public List<SpotifyPlaylist> GetUserPlaylists (string userId)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add ("Authorization", AuthorizationHeader);

            var response = client.GetAsync (baseUrl + usersUrl + userId + "/playlists").Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            var userPlaylistsPaging = JsonConvert.DeserializeObject<SpotifyPaging<SpotifyPlaylist>> (responseContent);

            var playlistsNumber = userPlaylistsPaging.TotalItemNumbers > 20 ? 20 : userPlaylistsPaging.TotalItemNumbers;     //because paging

            List<SpotifyPlaylist> userPlaylists = new List<SpotifyPlaylist>();
            for (int i = 0; i < playlistsNumber; ++i)
                userPlaylists.Add(userPlaylistsPaging.Items[i]);

            return userPlaylists;
        }
    }
}
