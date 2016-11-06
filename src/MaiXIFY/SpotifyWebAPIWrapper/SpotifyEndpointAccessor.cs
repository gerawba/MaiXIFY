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
        public static string AuthorizationHeader
        {
            get 
            {
                if (SpotifyWebAPIWrapper.SpotifyAuthorization.AccessToken == null)
                    return "";
                else
                    return "Bearer " + SpotifyWebAPIWrapper.SpotifyAuthorization.AccessToken;
            }
        }


        public SpotifyUserProfile GetCurrentUserProfile ()
        {
            var client = new HttpClient ();
            client.DefaultRequestHeaders.Add ("Authorization", AuthorizationHeader);

            var response = client.GetAsync (baseUrl + "/v1/me").Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync ().Result;
            var userProfile = JsonConvert.DeserializeObject<SpotifyUserProfile> (responseContent);

            return userProfile;
        }       
    }
}
