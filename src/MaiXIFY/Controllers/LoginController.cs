using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MaiXIFY.Controllers
{
    public class LoginController : Controller
    {
        private SpotifyWebAPIWrapper.ISpotifyAuthorization _spotifyAuthorization;

        public LoginController (SpotifyWebAPIWrapper.ISpotifyAuthorization spotifyAuthorization) 
        {
            _spotifyAuthorization = spotifyAuthorization;
        }


        public IActionResult Index ()
        {
            string scope = "user-read-private playlist-read-private playlist-modify-public playlist-modify-private";

            return Redirect (_spotifyAuthorization.RequestAuthorization (scope, HttpContext));
        }

        public IActionResult Callback ()
        {
            bool success = _spotifyAuthorization.RequestAccessAndRefreshTokens (HttpContext);

            if (!success)
                return RedirectToAction ("Error", "Home", new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (401, "Unauthorized - This page is only available after redirecting from Spotify."));

            return RedirectToAction ("Index", "Home", _spotifyAuthorization.Token);
        }
    }
}
