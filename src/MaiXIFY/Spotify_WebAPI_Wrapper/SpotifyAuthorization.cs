using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MaiXIFY.Spotify_WebAPI_Wrapper
{
    public class SpotifyAuthorization : ISpotifyAuthorization
    {
        private Spotify_WebAPI_Wrapper.SpotifyCredentialsSettings _spotifyCredentialsSettings { get; set; }
        private string authorizeEndpoint = "https://accounts.spotify.com/authorize";

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
