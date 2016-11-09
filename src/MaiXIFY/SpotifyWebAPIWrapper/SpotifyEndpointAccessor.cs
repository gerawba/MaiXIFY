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
            var client = GetSpotifyHttpClient ();

            var response = client.GetAsync (baseUrl + "/v1/me").Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            return JsonConvert.DeserializeObject<SpotifyUser> (responseContent);
        }


        public List<SpotifyPlaylistSimplified> GetUserPlaylists (string userId)
        {
            var client = GetSpotifyHttpClient ();

            var response = client.GetAsync (baseUrl + usersUrl + userId + "/playlists").Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            var userPlaylistsPaging = JsonConvert.DeserializeObject<SpotifyPaging<SpotifyPlaylistSimplified>> (responseContent);

            var playlistsNumber = userPlaylistsPaging.TotalItemNumbers > 20 ? 20 : userPlaylistsPaging.TotalItemNumbers;     //because paging

            List<SpotifyPlaylistSimplified> userPlaylists = new List<SpotifyPlaylistSimplified>();
            for (int i = 0; i < playlistsNumber; ++i)
                userPlaylists.Add (userPlaylistsPaging.Items[i]);

            return userPlaylists;
        }


        public SpotifyPlaylist GetPlaylist (string userId, string playlistId)
        {
            var client = GetSpotifyHttpClient ();
            
            var response = client.GetAsync (baseUrl + usersUrl + userId + "/playlists/" + playlistId).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            return JsonConvert.DeserializeObject<SpotifyPlaylist> (responseContent);
        }


        public List<SpotifyPlaylist> GetPlaylists (List<SpotifyHelpers.SelectedPlaylistElem> selectedPlaylists)
        {
            if (selectedPlaylists.Count == 0)
                return null;

            var client = GetSpotifyHttpClient ();

            List<SpotifyPlaylist> playlists = new List<SpotifyPlaylist> ();

            foreach (SpotifyHelpers.SelectedPlaylistElem playlistElem in selectedPlaylists) {
                var response = client.GetAsync (baseUrl + usersUrl + playlistElem.UserId + "/playlists/" + playlistElem.PlaylistId).Result;

                if (!response.IsSuccessStatusCode)
                    break;

                var responseContent = response.Content.ReadAsStringAsync ().Result;
                SpotifyPlaylist playlist = JsonConvert.DeserializeObject<SpotifyPlaylist> (responseContent);
                playlists.Add (playlist);
            }


            return playlists;
        }


        public SpotifyPlaylist CreatePlaylist (string userId, string playlistName, bool isPublic = true, bool isCollaborative = false)
        {
            var client = GetSpotifyHttpClient ();

            SpotifyHelpers.RequestContentCreatePlaylist requestContent = new SpotifyHelpers.RequestContentCreatePlaylist ();
            requestContent.Name = playlistName;
            requestContent.IsPublic = isPublic;
            requestContent.IsCollaborative = isCollaborative;

            var response = client.PostAsync (baseUrl + usersUrl + userId + "/playlists", new StringContent(JsonConvert.SerializeObject(requestContent))).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<SpotifyPlaylist> (responseContent);
        }


        public bool AddTrackToPlaylist (string userId, string playlistId, List<string> trackUriList)
        {
            var client = GetSpotifyHttpClient ();

            SpotifyHelpers.RequestContentAddTrackToPlaylist requestContent = new SpotifyHelpers.RequestContentAddTrackToPlaylist ();
            requestContent.TrackUriList = trackUriList;

            var response = client.PostAsync (baseUrl + usersUrl + userId + "/playlists/" + playlistId + "/tracks", new StringContent (JsonConvert.SerializeObject(requestContent))).Result;

            if (!response.IsSuccessStatusCode)
                return false;

            return true;
        }


        private static HttpClient GetSpotifyHttpClient ()
        {
            var client = new HttpClient ();

            client.DefaultRequestHeaders.Add ("Authorization", AuthorizationHeader);
            client.DefaultRequestHeaders.Add ("ContentType", "application/json");

            return client;
        }
    }
}
