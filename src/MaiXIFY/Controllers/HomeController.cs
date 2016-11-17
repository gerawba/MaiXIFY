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
            ViewData["userName"] = _spotifyEndpointAccessor.GetCurrentUserProfile ().Id;
            return View ();
        }


        public IActionResult GetUserPlaylists (string userId)
        {
            List<SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylistSimplified> playlists = new List<SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylistSimplified> ();

            if (userId == null) {
                ViewData["Message"] = "Nincs felhasználó kiválasztva!";
                return View();
            }
            else {
                SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyUser selectedUser = _spotifyEndpointAccessor.GetUserProfile (userId);
                if (selectedUser == null)
                    return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (404, "No such user"));

                playlists = _spotifyEndpointAccessor.GetUserPlaylists (userId);
                if (playlists == null)
                    return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (503, "Cannot get playlists"));
            }

            return Json (playlists);
        }


        public IActionResult GetPlaylist (string userId, string playlistId)
        {
            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist playlist = new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist ();

            if (userId == null || playlistId == null) {
                ViewData["Message"] = "Nincs felhasználó/playlist kiválasztva!";
                return View ();
            }
            else {
                playlist = _spotifyEndpointAccessor.GetPlaylist (userId, playlistId);
                if (playlist == null)
                    return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (404, "Not found/Invalid user or playlist id"));
            }

            return Json (playlist);
        }


        public IActionResult GeneratePlaylist (List<SpotifyWebAPIWrapper.SpotifyHelpers.SelectedPlaylistElem> selectedPlaylists, string playlistName, bool isPublic, bool isCollaborative)
        {
            if (selectedPlaylists.Count < 2) {
                return RedirectToAction ("Error", new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (400, "You have to select minimum 2 playlists!"));
            }

            List<SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist> selectedPlaylistsObject = _spotifyEndpointAccessor.GetPlaylists (selectedPlaylists);

            SpotifyWebAPIWrapper.SpotifyObjectModel.PlaylistMixerCoreLogic mixer = new SpotifyWebAPIWrapper.SpotifyObjectModel.PlaylistMixerCoreLogic (_spotifyEndpointAccessor);
            if (HttpContext.Request.Cookies.ContainsKey (SpotifyWebAPIWrapper.SpotifyHelpers.thresholdSettingCookieKey) &&
                HttpContext.Request.Cookies.ContainsKey (SpotifyWebAPIWrapper.SpotifyHelpers.recommendedMusicSettingCookieKey) &&
                HttpContext.Request.Cookies.ContainsKey (SpotifyWebAPIWrapper.SpotifyHelpers.sortOptionSettingCookieKey))
            {
                string thresholdString = HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.thresholdSettingCookieKey];
                double thresholdDouble;
                Double.TryParse (thresholdString, out thresholdDouble);

                mixer.Settings.Threshold = thresholdDouble;
                mixer.Settings.RecommendedMusic = Convert.ToBoolean (HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.recommendedMusicSettingCookieKey]);
                mixer.Settings.SortOption = SpotifyWebAPIWrapper.SpotifyObjectModel.PlaylistMixerCoreLogic.PlaylistMixerSettings.ConvertStringToSortOptions (HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.sortOptionSettingCookieKey]);
            }

            if (playlistName == null || playlistName == "")
                playlistName = SpotifyWebAPIWrapper.SpotifyHelpers.MakeDefaultPlaylistName ();

            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist generatedPlaylist = mixer.GenerateMaixifyPlaylist (selectedPlaylistsObject, playlistName, isPublic, isCollaborative);
            if (generatedPlaylist == null)
                return RedirectToAction ("Error", new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (400, "Cannot make playlist with the given settings!"));

            return RedirectToAction ("Maixified", new { userId = generatedPlaylist.OwnerUser.Id, playlistId = generatedPlaylist.Id });
        }


        public IActionResult Maixified (string userId, string playlistId)
        {
            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist generatedPlaylist = _spotifyEndpointAccessor.GetPlaylist (userId, playlistId);

            return View (generatedPlaylist);
        }

        public IActionResult Error (SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError error)
        {
            ViewData["Message"] = "Ooop! Something went wrong...";

            if (error == null)
                return View ();

            ViewData["ErrorMessage"] = error.ErrorMessage;
            return View ();
        }


        public IActionResult About ()
        {
            ViewData["Message"] = _spotifyEndpointAccessor.GetCurrentUserProfile ().Id;
            return View ();
        }


        public IActionResult Contact ()
        {
            ViewData["Message"] = "MaiXIFY contact page.";

            return View ();
        }
    }
}
