using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaiXIFY.Spotify_WebAPI_Wrapper
{
    public interface ISpotifyAuthorization
    {
        string RequestAuthorization (string scope);
    }
}
