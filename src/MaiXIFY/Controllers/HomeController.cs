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
        private Spotify_WebAPI_Wrapper.SpotifyCredentialsSettings _spotifyCredentialsSettings { get; set; }

        public HomeController (IOptions<Spotify_WebAPI_Wrapper.SpotifyCredentialsSettings> spotifyCredentialsSettings)
        {
            _spotifyCredentialsSettings = spotifyCredentialsSettings.Value;
        }

        public IActionResult Index()
        {
            var cID = _spotifyCredentialsSettings.ClientId;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
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
