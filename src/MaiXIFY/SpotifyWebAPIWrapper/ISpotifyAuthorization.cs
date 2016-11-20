using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MaiXIFY.SpotifyWebAPIWrapper
{
    public interface ISpotifyAuthorization
    {
        SpotifyAuthorization.SpotifyToken Token { get; set; }

        string RequestAuthorization (string scope, HttpContext context);

        bool RequestAccessAndRefreshTokens (HttpContext context);

        bool RequestAccesTokenFromRefreshToken ();
    }
}
