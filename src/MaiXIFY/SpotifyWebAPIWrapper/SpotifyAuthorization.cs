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
        public static int TokenExpirationTimeInSeconds { get; set; }
        public static DateTime TokenObtained { get; set; }


        public SpotifyAuthorization (IOptions<SpotifyWebAPIWrapper.SpotifyCredentialsSettings> spotifyCredentialsSettings)
        {
            _spotifyCredentialsSettings = spotifyCredentialsSettings.Value;
        }


        public string RequestAuthorization (string scope, HttpContext context)
        {
            var stateCookie = GenerateRandomString (16);

            QueryString queryString = new QueryString ();
            queryString = queryString.Add ("client_id", _spotifyCredentialsSettings.ClientId);
            queryString = queryString.Add ("response_type", "code");
            queryString = queryString.Add ("redirect_uri", _spotifyCredentialsSettings.RedirectURI);
            queryString = queryString.Add ("state", stateCookie);
            queryString = queryString.Add ("scope", scope);

            context.Response.Cookies.Append (SpotifyHelpers.stateCookieKey, stateCookie);

            return authorizeEndpoint + queryString.ToUriComponent ();
        }


        public bool RequestAccessAndRefreshTokens (HttpContext context)
        {
            string state = context.Request.Query["state"];
            if (!context.Request.Cookies.ContainsKey (SpotifyHelpers.stateCookieKey))
                return false;

            if (state == null || !state.Equals (context.Request.Cookies[SpotifyHelpers.stateCookieKey]))
                return false;

            string error = context.Request.Query["error"];
            if (error != null)
                return false;

            context.Response.Cookies.Delete (SpotifyHelpers.stateCookieKey);

            string code = context.Request.Query["code"];
            if (code == null)
                return false;

            var client = new HttpClient ();

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
            TokenExpirationTimeInSeconds = spotifyToken.ExpiresIn;
            TokenObtained = DateTime.Now;

            return true;
        }


        public bool RequestAccesTokenFromRefreshToken ()
        {
            var client = new HttpClient ();

            var requestContent = new FormUrlEncodedContent (new[] {
                new KeyValuePair<string, string> ("grant_type", "refresh_token"),
                new KeyValuePair<string, string> ("refresh_token", RefreshToken)
            });

            string clientCredentialsString = _spotifyCredentialsSettings.ClientId + ":" + _spotifyCredentialsSettings.ClientSecret;
            byte[] clientCredentialsBytes = System.Text.Encoding.UTF8.GetBytes (clientCredentialsString);
            client.DefaultRequestHeaders.Add ("Authorization", "Basic " + Convert.ToBase64String (clientCredentialsBytes));

            var response = client.PostAsync (tokenEndpoint, requestContent).Result;

            if (!response.IsSuccessStatusCode)
                return false;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            var spotifyToken = JsonConvert.DeserializeObject<SpotifyToken> (responseContent);

            AccessToken = spotifyToken.AccessToken;
            TokenExpirationTimeInSeconds = spotifyToken.ExpiresIn;
            TokenObtained = DateTime.Now;

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


        public class SpotifyToken
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
