using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MaiXIFY.SpotifyWebAPIWrapper.SpotifyObjectModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MaiXIFY.SpotifyWebAPIWrapper
{
    public class SpotifyEndpointAccessor : ISpotifyEndpointAccessor
    {
        private SpotifyWebAPIWrapper.ISpotifyAuthorization _spotifyAuthorization { get; }
        private const string baseUrl = "https://api.spotify.com";
        private const string usersUrl = "/v1/users/";
        private const string artistsUrl = "/v1/artists/";
        private const string tracksUrl = "/v1/tracks/";


        public SpotifyEndpointAccessor (SpotifyWebAPIWrapper.ISpotifyAuthorization spotifyAuthorization)
        {
            _spotifyAuthorization = spotifyAuthorization;
        }


        public void SetAuthorizationToken (SpotifyWebAPIWrapper.SpotifyAuthorization.SpotifyToken token, HttpContext context)
        {
            _spotifyAuthorization.Token = token;

            if (_spotifyAuthorization.Token.AccessToken == null || _spotifyAuthorization.Token.TokenObtained.AddSeconds (_spotifyAuthorization.Token.ExpiresIn) < DateTime.Now)
            {
                bool success = _spotifyAuthorization.RequestAccesTokenFromRefreshToken ();
                if (success) {
                    context.Response.Cookies.Append (SpotifyWebAPIWrapper.SpotifyHelpers.accessTokenCookieKey, token.AccessToken);
                    context.Response.Cookies.Append (SpotifyWebAPIWrapper.SpotifyHelpers.refreshTokenCookieKey, token.RefreshToken);
                    context.Response.Cookies.Append (SpotifyWebAPIWrapper.SpotifyHelpers.expiresInKey, token.ExpiresIn.ToString ());
                    context.Response.Cookies.Append (SpotifyWebAPIWrapper.SpotifyHelpers.tokenObtainedKey, token.TokenObtained.ToString ("yyyy-MM-ddTHH:mm:ss.fff"));
                }
            }
        }


        private string AuthorizationHeader
        {
            get 
            {
                return "Bearer " + _spotifyAuthorization.Token.AccessToken;
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


        public SpotifyUser GetUserProfile (string userId)
        {
            var client = GetSpotifyHttpClient ();

            var response = client.GetAsync (baseUrl + usersUrl + userId).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            return JsonConvert.DeserializeObject<SpotifyUser> (responseContent);
        }


        public SpotifyTrack GetTrack (string trackId)
        {
            if (trackId == null)
                return null;

            var client = GetSpotifyHttpClient ();

            var response = client.GetAsync (baseUrl + tracksUrl + trackId).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            return JsonConvert.DeserializeObject<SpotifyTrack> (responseContent);
        }


        public List<SpotifyTrack> GetArtistsTopTracks (string artistId)
        {
            if (artistId == null)
                return null;

            var client = GetSpotifyHttpClient ();

            var response = client.GetAsync (baseUrl + artistsUrl + artistId + "/top-tracks?country=" + GetCurrentUserProfile ().Country).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            return JsonConvert.DeserializeObject<SpotifyTrack.SpotifyTracksList> (responseContent).Tracks;
        }


        public List<SpotifyPlaylistSimplified> GetUserPlaylists (string userId)
        {
            if (userId == null)
                return null;

            var client = GetSpotifyHttpClient ();

            var response = client.GetAsync (baseUrl + usersUrl + userId + "/playlists").Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            var userPlaylistsPaging = JsonConvert.DeserializeObject<SpotifyPaging<SpotifyPlaylistSimplified>> (responseContent);

            //because paging
            var playlistsNumber = userPlaylistsPaging.TotalItemNumbers > 20 ? 20 : userPlaylistsPaging.TotalItemNumbers;

            List<SpotifyPlaylistSimplified> userPlaylists = new List<SpotifyPlaylistSimplified> ();
            for (int i = 0; i < playlistsNumber; ++i)
                userPlaylists.Add (userPlaylistsPaging.Items[i]);

            return userPlaylists;
        }


        public SpotifyPlaylist GetPlaylist (string userId, string playlistId)
        {
            if (userId == null || playlistId == null)
                return null;

            var client = GetSpotifyHttpClient ();
            
            var response = client.GetAsync (baseUrl + usersUrl + userId + "/playlists/" + playlistId).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            return JsonConvert.DeserializeObject<SpotifyPlaylist> (responseContent);
        }


        public List<SpotifyPlaylist> GetPlaylists (List<SpotifyHelpers.SelectedPlaylistElem> selectedPlaylists)
        {
            if (selectedPlaylists.Count < 0)
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
            if (userId == null)
                return null;

            if (playlistName == null)
                playlistName = SpotifyWebAPIWrapper.SpotifyHelpers.MakeDefaultPlaylistName ();

            var client = GetSpotifyHttpClient ();

            SpotifyHelpers.RequestContentCreatePlaylist requestContent = new SpotifyHelpers.RequestContentCreatePlaylist ();
            requestContent.Name = playlistName;
            requestContent.IsPublic = isPublic;
            requestContent.IsCollaborative = isCollaborative;

            var response = client.PostAsync (baseUrl + usersUrl + userId + "/playlists", new StringContent (JsonConvert.SerializeObject(requestContent))).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<SpotifyPlaylist> (responseContent);
        }


        public bool AddTracksToPlaylist (string userId, string playlistId, List<string> trackUriList)
        {
            if (userId == null || playlistId == null || trackUriList.Count < 1)
                return false;

            var client = GetSpotifyHttpClient ();

            SpotifyHelpers.RequestContentAddTrackToPlaylist requestContent = new SpotifyHelpers.RequestContentAddTrackToPlaylist ();
            requestContent.TrackUriList = trackUriList;

            var response = client.PostAsync (baseUrl + usersUrl + userId + "/playlists/" + playlistId + "/tracks", new StringContent (JsonConvert.SerializeObject(requestContent))).Result;

            if (!response.IsSuccessStatusCode)
                return false;

            return true;
        }


        private HttpClient GetSpotifyHttpClient ()
        {
            var client = new HttpClient ();

            client.DefaultRequestHeaders.Add ("Authorization", AuthorizationHeader);
            client.DefaultRequestHeaders.Add ("ContentType", "application/json");

            return client;
        }
    }
}
