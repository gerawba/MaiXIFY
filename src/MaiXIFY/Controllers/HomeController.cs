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

        //torolheto majd
        public IActionResult GetTrack (string trackId)
        {
            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyTrack track = _spotifyEndpointAccessor.GetTrack (trackId);

            if (track == null) {
                ViewData["Message"] = "Nem sikerült lekérni a számot!";
            }

            return Json (track);
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


        public IActionResult GeneratePlaylist (List<SpotifyWebAPIWrapper.SpotifyHelpers.SelectedPlaylistElem> selectedPlaylists, string playlistName, bool isPublic, bool isCollaborative)
        {
            //if (selectedPlaylists.Count < 2) {
            //    ViewData["Message"] = "Nincs legalább 2 playlist kiválasztva!";
            //    return View ();
            //}

            SpotifyWebAPIWrapper.SpotifyHelpers.SelectedPlaylistElem pl1 = new SpotifyWebAPIWrapper.SpotifyHelpers.SelectedPlaylistElem ();
            pl1.UserId = "gerawba";
            pl1.PlaylistId = "47VN6Xbly3m0n77coFlFV9";

            SpotifyWebAPIWrapper.SpotifyHelpers.SelectedPlaylistElem pl2 = new SpotifyWebAPIWrapper.SpotifyHelpers.SelectedPlaylistElem ();
            pl2.UserId = "gerawba";
            pl2.PlaylistId = "6VOydczE7fWoHEaND4qqPxB";

            selectedPlaylists.Add(pl1);
            selectedPlaylists.Add(pl2);

            List<SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist> selectedPlaylistsObject = _spotifyEndpointAccessor.GetPlaylists (selectedPlaylists);

            SpotifyWebAPIWrapper.SpotifyObjectModel.PlaylistMixerCoreLogic mixer = new SpotifyWebAPIWrapper.SpotifyObjectModel.PlaylistMixerCoreLogic(_spotifyEndpointAccessor);
            if (HttpContext.Request.Cookies.ContainsKey (SpotifyWebAPIWrapper.SpotifyHelpers.thresholdSettingCookieKey) &&
                HttpContext.Request.Cookies.ContainsKey (SpotifyWebAPIWrapper.SpotifyHelpers.recommendedMusicSettingCookieKey) &&
                HttpContext.Request.Cookies.ContainsKey (SpotifyWebAPIWrapper.SpotifyHelpers.sortOptionSettingCookieKey))
            {               
                mixer.Settings.Threshold = Convert.ToDouble(HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.thresholdSettingCookieKey]);
                mixer.Settings.RecommendedMusic = Convert.ToBoolean(HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.recommendedMusicSettingCookieKey]);
                mixer.Settings.SortOption = SpotifyWebAPIWrapper.SpotifyObjectModel.PlaylistMixerCoreLogic.PlaylistMixerSettings.ConvertStringToSortOptions(HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.sortOptionSettingCookieKey]);
            }

            if (playlistName == null || playlistName == "")
                playlistName = SpotifyWebAPIWrapper.SpotifyHelpers.MakeDefaultPlaylistName ();

            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist generatedPlaylist = mixer.GenerateRecommendedPlaylist (selectedPlaylistsObject, playlistName, isPublic, isCollaborative);

            return Json (generatedPlaylist);
        }


        public IActionResult CreatePlaylist (string playlistName, bool isPublic = true, bool isCollaborative = false)
        {
            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist playlist = new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist ();

            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyUser currentUser = _spotifyEndpointAccessor.GetCurrentUserProfile ();

            if (currentUser.Id == null || playlistName == null) {
                ViewData["Message"] = "Nem adtál meg nevet a létrehozandó playlistedhez!";
            }
            else
                playlist = _spotifyEndpointAccessor.CreatePlaylist (currentUser.Id, playlistName, isPublic, isCollaborative);

            return Json (playlist);
        }


        public IActionResult AddTrackToPlaylist (string playlistId, List<string> trackUriList)
        {
            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist playlist = new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist ();

            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyUser currentUser = _spotifyEndpointAccessor.GetCurrentUserProfile ();

            if (trackUriList.Count < 1)
                ViewData["Message"] = "Nem adtál meg egy hozzáadandó számot sem!";
            else if (currentUser.Id == null || playlistId == null)
                ViewData["Message"] = "Nem adtál meg nevet a létrehozandó playlistedhez!";
            else {
                bool success = _spotifyEndpointAccessor.AddTracksToPlaylist(currentUser.Id, playlistId, trackUriList);
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
