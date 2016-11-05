using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MaiXIFY.Spotify_WebAPI_Wrapper
{
    public interface ISpotifyAuthorization
    {
        string RequestAuthorization (string scope);
        bool RequestAccessAndRefreshTokens (HttpRequest request);
    }
}
