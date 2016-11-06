using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MaiXIFY.Controllers
{
    public class HomeController : Controller
    {
        private SpotifyWebAPIWrapper.ISpotifyEndpointAccessor _spotifyEndpointAccessor;

        public HomeController (SpotifyWebAPIWrapper.ISpotifyEndpointAccessor spotifyEndpointAccessor)
        {
            _spotifyEndpointAccessor = spotifyEndpointAccessor;
        }

        public IActionResult Index ()
        {
            return View ();
        }

        public IActionResult About ()
        {
            var spotifyUserProfile = _spotifyEndpointAccessor.GetCurrentUserProfile ();
            if (spotifyUserProfile == null)
                ViewData["Message"] = "Hiba történt a GetCurrentProfile lekérésekor.";

            ViewData["Message"] = spotifyUserProfile.DisplayName + spotifyUserProfile.Id;
            return View ();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
