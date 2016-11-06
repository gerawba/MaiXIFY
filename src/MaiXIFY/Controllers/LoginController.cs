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

        // GET: /<controller>/
        public IActionResult Index ()
        {
            string scope = "user-follow-modify streaming";

            return Redirect (_spotifyAuthorization.RequestAuthorization (scope, HttpContext));
        }
    }
}
