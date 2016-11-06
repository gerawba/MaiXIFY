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

        public IActionResult GetUserPlaylists (string userId)
        {
            List<SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylistSimplified> playlists = new List<SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylistSimplified>();

            if (userId == null) {
                ViewData["Message"] = "Nincs felhasználó kiválasztva!";
            } else
                playlists = _spotifyEndpointAccessor.GetUserPlaylists (userId);

            return Json (playlists);
        }


        public IActionResult GetPlaylist (string userId, string playlistId)
        {
            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist playlist = new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist ();

            if (userId == null || playlist == null)
            {
                ViewData["Message"] = "Nincs felhasználó/playlist kiválasztva!";
            }
            else
                playlist = _spotifyEndpointAccessor.GetPlaylist (userId, playlistId);

            return Json (playlist);
        }


        public IActionResult SelectPlaylists (List<SpotifyWebAPIWrapper.SpotifyHelpers.SelectedPlaylistElem> selectedPlaylist)
        {
            
            return Json ("Ok");
        }


        public IActionResult CreatePlaylist (string playlistName, bool isPublic = true, bool isCollaborative = false)
        {
            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist playlist = new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist ();

            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyUser currentUser = _spotifyEndpointAccessor.GetCurrentUserProfile ();

            if (currentUser.Id == null || playlistName == null)
            {
                ViewData["Message"] = "Nem adtál meg nevet a létrehozandó playlistedhez!";
            }
            else
                playlist = _spotifyEndpointAccessor.CreatePlaylist (currentUser.Id, playlistName, isPublic, isCollaborative);

            return Json (playlist);
        }


        public IActionResult AddTrackToPlaylist (string playlistId, List<string> trackUriList)
        {
            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist playlist = new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist();

            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyUser currentUser = _spotifyEndpointAccessor.GetCurrentUserProfile();

            if (trackUriList == null)
                trackUriList = new List<string> ();
            
            // TODO csak a peldaert ezeket majd torolni kell
            trackUriList.Add("spotify:track:1UbqS1I358tyD3Dz6sEJ4P");
            trackUriList.Add("spotify:track:7FnaZ2MLfUk8Mt2hSbquAU");

            if (currentUser.Id == null || playlistId == null)
            {
                ViewData["Message"] = "Nem adtál meg nevet a létrehozandó playlistedhez!";
            }
            else
            {
                bool success = _spotifyEndpointAccessor.AddTrackToPlaylist(currentUser.Id, playlistId, trackUriList);
                if (success)
                    ViewData["Message"] = "Sikeresen hozzaadtuk a szamokat a playlisthez!";
            }

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


        public IActionResult Contact ()
        {
            ViewData["Message"] = "Your contact page.";

            string ownerId = "gerawba";

            _spotifyEndpointAccessor.GetUserPlaylists (ownerId);

            return View ();
        }


        public IActionResult Error()
        {
            return View();
        }
    }
}
