using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaiXIFY.SpotifyWebAPIWrapper
{
    public class SpotifyCredentialsSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectURI { get; set; }
    }
}
