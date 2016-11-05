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

namespace MaiXIFY.Spotify_WebAPI_Wrapper
{
    public class SpotifyAuthorization : ISpotifyAuthorization
    {
        private Spotify_WebAPI_Wrapper.SpotifyCredentialsSettings _spotifyCredentialsSettings { get; set; }
        private string authorizeEndpoint = "https://accounts.spotify.com/authorize";
        private string tokenEndpoint = "https://accounts.spotify.com/api/token";

        public SpotifyAuthorization(IOptions<Spotify_WebAPI_Wrapper.SpotifyCredentialsSettings> spotifyCredentialsSettings)
        {
            _spotifyCredentialsSettings = spotifyCredentialsSettings.Value;
        }


        public string RequestAuthorization (string scope)
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

            //cookie statekey torles

            string code = request.Query["code"];
            if (code == null)
                return false;

            var client = new HttpClient();
            client.BaseAddress = new Uri(tokenEndpoint);

            JObject body = new JObject();
            body.Add("grant_type", "authorization_code");
            body.Add("code", code);
            body.Add("redirect_uri", _spotifyCredentialsSettings.RedirectURI);

            string clientCredentialsString = _spotifyCredentialsSettings.ClientId + ":" + _spotifyCredentialsSettings.ClientSecret;
            byte[] clientCredentialsBytes = System.Text.Encoding.UTF8.GetBytes(clientCredentialsString);
            client.DefaultRequestHeaders.Add ("Authorization", "Basic " + Convert.ToBase64String(clientCredentialsBytes));

            var x = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");

            var res = client.PostAsync(tokenEndpoint, new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json")).Result;

            // var response = await client.GetAsync("");
            
            if (!res.IsSuccessStatusCode)
                return false;

            return true;
        }


        private string GenerateRandomString (int length)
        {
            string text = "";
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rnd = new Random();

            for (var i = 0; i < length; ++i)
                text += chars[Convert.ToInt32(rnd.Next(0, chars.Length - 1))];

            return text;
        }
    }
}
