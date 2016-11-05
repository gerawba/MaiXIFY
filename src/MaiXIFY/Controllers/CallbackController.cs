using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MaiXIFY.Controllers
{
    public class CallbackController : Controller
    {
        private Spotify_WebAPI_Wrapper.ISpotifyAuthorization _spotifyAuthorization;

        public CallbackController(Spotify_WebAPI_Wrapper.ISpotifyAuthorization spotifyAuthorization)
        {
            _spotifyAuthorization = spotifyAuthorization;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            _spotifyAuthorization.RequestAccessAndRefreshTokens (HttpContext.Request);

            return View();
        }


        public IActionResult Error()
        {
            return View();
        }
    }
}
