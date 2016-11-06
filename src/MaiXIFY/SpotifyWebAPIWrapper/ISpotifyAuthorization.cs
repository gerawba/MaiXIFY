using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MaiXIFY.SpotifyWebAPIWrapper
{
    public interface ISpotifyAuthorization
    {
        string RequestAuthorization (string scope);
        bool RequestAccessAndRefreshTokens (HttpRequest request);
    }
}
