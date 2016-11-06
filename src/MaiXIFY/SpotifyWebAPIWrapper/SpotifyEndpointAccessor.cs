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


        public List<SpotifyPlaylistSimplified> GetUserPlaylists (string userId)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add ("Authorization", AuthorizationHeader);

            var response = client.GetAsync (baseUrl + usersUrl + userId + "/playlists").Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            var userPlaylistsPaging = JsonConvert.DeserializeObject<SpotifyPaging<SpotifyPlaylistSimplified>> (responseContent);

            var playlistsNumber = userPlaylistsPaging.TotalItemNumbers > 20 ? 20 : userPlaylistsPaging.TotalItemNumbers;     //because paging

            List<SpotifyPlaylistSimplified> userPlaylists = new List<SpotifyPlaylistSimplified>();
            for (int i = 0; i < playlistsNumber; ++i)
                userPlaylists.Add(userPlaylistsPaging.Items[i]);

            return userPlaylists;
        }


        public SpotifyPlaylist GetPlaylist (string userId, string playlistId)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add ("Authorization", AuthorizationHeader);
            
            var response = client.GetAsync (baseUrl + usersUrl + userId + "/playlists/" + playlistId).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<SpotifyPlaylist> (responseContent);
        }

        public SpotifyPlaylist CreatePlaylist (string userId, string playlistName, bool isPublic = true, bool isCollaborative = false)
        {
            var client = new HttpClient ();

            client.DefaultRequestHeaders.Add ("Authorization", AuthorizationHeader);
            client.DefaultRequestHeaders.Add ("ContentType", "application/json");

            SpotifyHelpers.RequestContentCreate requestContent = new SpotifyHelpers.RequestContentCreate ();
            requestContent.Name = playlistName;
            requestContent.IsPublic = isPublic;
            requestContent.IsCollaborative = isCollaborative;

            var response = client.PostAsync (baseUrl + usersUrl + userId + "/playlists", new StringContent(JsonConvert.SerializeObject(requestContent))).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<SpotifyPlaylist> (responseContent);
        }


    }
}
