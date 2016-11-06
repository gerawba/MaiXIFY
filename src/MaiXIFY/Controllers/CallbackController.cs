using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace MaiXIFY.Controllers
{
    public class CallbackController : Controller
    {
        private SpotifyWebAPIWrapper.ISpotifyAuthorization _spotifyAuthorization;

        public CallbackController (SpotifyWebAPIWrapper.ISpotifyAuthorization spotifyAuthorization)
        {
            _spotifyAuthorization = spotifyAuthorization;
        }


        public IActionResult Index ()
        {
            bool success = _spotifyAuthorization.RequestAccessAndRefreshTokens (HttpContext);

            if (!success)
                return new RedirectToActionResult ("Error", "Callback", null);

            ViewData["Message"] = "Hozzaférés OK";
            return new RedirectToActionResult ("SelectPlaylists", "Home", null);
        }


        public IActionResult Error ()
        {
            ViewBag.Message = "Hiba történt a CALLBACK hivásakor - ez az oldal csak átirányitás után érhető el. Talén nem adtál engedélyt a MaiXIFIY appnek? Netén manuálisan tévedtél erre a címre?";
            return View ();
        }
    }
}
