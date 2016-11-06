using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MaiXIFY.SpotifyWebAPIWrapper
{
    public class SpotifyAuthorization : ISpotifyAuthorization
    {
        private SpotifyWebAPIWrapper.SpotifyCredentialsSettings _spotifyCredentialsSettings { get; set; }
        private const string authorizeEndpoint = "https://accounts.spotify.com/authorize";
        private const string tokenEndpoint = "https://accounts.spotify.com/api/token";
        public static string AccessToken { get; set; }
        public static string RefreshToken { get; set; }

        public SpotifyAuthorization (IOptions<SpotifyWebAPIWrapper.SpotifyCredentialsSettings> spotifyCredentialsSettings)
        {
            _spotifyCredentialsSettings = spotifyCredentialsSettings.Value;
        }


        public string RequestAuthorization(string scope)
        {
            QueryString queryString = new QueryString();
            queryString = queryString.Add("client_id", _spotifyCredentialsSettings.ClientId);
            queryString = queryString.Add("response_type", "code");
            queryString = queryString.Add("redirect_uri", _spotifyCredentialsSettings.RedirectURI);
            queryString = queryString.Add("state", GenerateRandomString(16));
            queryString = queryString.Add("scope", scope);

            return authorizeEndpoint + queryString.ToUriComponent();
        }


        public bool RequestAccessAndRefreshTokens(HttpRequest request)
        {
            string state = request.Query["state"];
            if (state == null) //|| !state.Equals(cookie state))
                return false;

            string error = request.Query["error"];
            if (error != null)
                return false;

            //cookie statekey torles

            string code = request.Query["code"];
            if (code == null)
                return false;

            var client = new HttpClient();

            var requestContent = new FormUrlEncodedContent (new[] {
                new KeyValuePair<string, string> ("grant_type", "authorization_code"),
                new KeyValuePair<string, string> ("code", code),
                new KeyValuePair<string, string> ("redirect_uri", _spotifyCredentialsSettings.RedirectURI)
            });

            string clientCredentialsString = _spotifyCredentialsSettings.ClientId + ":" + _spotifyCredentialsSettings.ClientSecret;
            byte[] clientCredentialsBytes = System.Text.Encoding.UTF8.GetBytes (clientCredentialsString);
            client.DefaultRequestHeaders.Add ("Authorization", "Basic " + Convert.ToBase64String (clientCredentialsBytes));

            var response = client.PostAsync (tokenEndpoint, requestContent).Result;

            if (!response.IsSuccessStatusCode)
                return false;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            var spotifyToken = JsonConvert.DeserializeObject<SpotifyToken> (responseContent);

            AccessToken = spotifyToken.AccessToken;
            RefreshToken = spotifyToken.RefreshToken;

            return true;
        }


        private string GenerateRandomString(int length)
        {
            string text = "";
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rnd = new Random();

            for (var i = 0; i < length; ++i)
                text += chars[Convert.ToInt32(rnd.Next(0, chars.Length - 1))];

            return text;
        }


        private class SpotifyToken
        {
            [JsonProperty(PropertyName = "access_token")]
            public string AccessToken { get; set; }
            [JsonProperty(PropertyName = "token_type")]
            public string TokenType { get; set; }
            [JsonProperty(PropertyName = "scope")]
            public string Scope { get; set; }
            [JsonProperty(PropertyName = "expires_in")]
            public int ExpiresIn { get; set; }
            [JsonProperty(PropertyName = "refresh_token")]
            public string RefreshToken { get; set; }
        }
    }
}
