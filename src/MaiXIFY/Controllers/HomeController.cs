using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MaiXIFY.Controllers
{
    public class HomeController : Controller
    {
        private SpotifyWebAPIWrapper.ISpotifyEndpointAccessor _spotifyEndpointAccessor;

        public HomeController (SpotifyWebAPIWrapper.ISpotifyEndpointAccessor spotifyEndpointAccessor)
        {
            _spotifyEndpointAccessor = spotifyEndpointAccessor;
        }


        public IActionResult Index (SpotifyWebAPIWrapper.SpotifyAuthorization.SpotifyToken token)
        {
            if (token.AccessToken != null) {
                _spotifyEndpointAccessor.SetAuthorizationToken (token);

                HttpContext.Response.Cookies.Append (SpotifyWebAPIWrapper.SpotifyHelpers.accessTokenCookieKey, token.AccessToken);
                HttpContext.Response.Cookies.Append (SpotifyWebAPIWrapper.SpotifyHelpers.refreshTokenCookieKey, token.RefreshToken);
                HttpContext.Response.Cookies.Append (SpotifyWebAPIWrapper.SpotifyHelpers.expiresInKey, token.ExpiresIn.ToString ());
                HttpContext.Response.Cookies.Append (SpotifyWebAPIWrapper.SpotifyHelpers.tokenObtainedKey, token.TokenObtained.ToString ("yyyy-MM-ddTHH:mm:ss.fff"));
            } else {
                token.AccessToken = HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.accessTokenCookieKey];
                token.RefreshToken = HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.refreshTokenCookieKey];
                token.ExpiresIn = int.Parse(HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.expiresInKey]);
                token.TokenObtained = SpotifyWebAPIWrapper.SpotifyHelpers.ParseDateTimeString (HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.tokenObtainedKey]);

                _spotifyEndpointAccessor.SetAuthorizationToken (token);
            }

            ViewData["userName"] = _spotifyEndpointAccessor.GetCurrentUserProfile ().Id;

            return View ();
        }


        public IActionResult GetUserPlaylists (string userId, string token)
        {
            if (token == null)
                return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (400, "Spotify token required!"));

            if (userId == null)
                return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (400, "Please add a Spotify username!"));

            SpotifyWebAPIWrapper.SpotifyAuthorization.SpotifyToken spotifyToken = JsonConvert.DeserializeObject<SpotifyWebAPIWrapper.SpotifyAuthorization.SpotifyToken> (token);
            _spotifyEndpointAccessor.SetAuthorizationToken (spotifyToken);

            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyUser selectedUser = _spotifyEndpointAccessor.GetUserProfile (userId);
            if (selectedUser == null)
                return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (404, "No such user."));

            List<SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylistSimplified> playlists = _spotifyEndpointAccessor.GetUserPlaylists (userId);
            if (playlists == null)
                return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (503, "Cannot get playlists."));

            return Json (playlists);
        }


        public IActionResult GetPlaylist (string userId, string playlistId, string token)
        {
            if (token == null)
                return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (400, "Spotify token required!"));

            if (userId == null || playlistId == null)
                return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (400, "Please add a Spotify username/playlist id!"));

            SpotifyWebAPIWrapper.SpotifyAuthorization.SpotifyToken spotifyToken = JsonConvert.DeserializeObject<SpotifyWebAPIWrapper.SpotifyAuthorization.SpotifyToken> (token);
            _spotifyEndpointAccessor.SetAuthorizationToken (spotifyToken);

            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist playlist = _spotifyEndpointAccessor.GetPlaylist (userId, playlistId);
            
            if (playlist == null)
                return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (404, "Not found/Invalid user or playlist id."));

            return Json (playlist);
        }


        public IActionResult GeneratePlaylist (List<SpotifyWebAPIWrapper.SpotifyHelpers.SelectedPlaylistElem> selectedPlaylists, string playlistName, bool isPublic, bool isCollaborative, string token)
        {
            if (token == null)
                return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (400, "Spotify token required!"));

            if (selectedPlaylists.Count < 2) {
                return RedirectToAction ("Error", new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (400, "You have to select minimum 2 playlists!"));
            }

            SpotifyWebAPIWrapper.SpotifyAuthorization.SpotifyToken spotifyToken = JsonConvert.DeserializeObject<SpotifyWebAPIWrapper.SpotifyAuthorization.SpotifyToken> (token);
            _spotifyEndpointAccessor.SetAuthorizationToken (spotifyToken);

            List<SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist> selectedPlaylistsObject = _spotifyEndpointAccessor.GetPlaylists (selectedPlaylists);

            SpotifyWebAPIWrapper.SpotifyObjectModel.PlaylistMixerCoreLogic mixer = new SpotifyWebAPIWrapper.SpotifyObjectModel.PlaylistMixerCoreLogic (_spotifyEndpointAccessor);
            if (HttpContext.Request.Cookies.ContainsKey (SpotifyWebAPIWrapper.SpotifyHelpers.thresholdSettingCookieKey) &&
                HttpContext.Request.Cookies.ContainsKey (SpotifyWebAPIWrapper.SpotifyHelpers.recommendedMusicSettingCookieKey) &&
                HttpContext.Request.Cookies.ContainsKey (SpotifyWebAPIWrapper.SpotifyHelpers.sortOptionSettingCookieKey))
            {
                string thresholdString = HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.thresholdSettingCookieKey];
                double thresholdDouble;
                Double.TryParse (thresholdString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out thresholdDouble);

                mixer.Settings.Threshold = thresholdDouble;
                mixer.Settings.RecommendedMusic = Convert.ToBoolean (HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.recommendedMusicSettingCookieKey]);
                mixer.Settings.SortOption = SpotifyWebAPIWrapper.SpotifyObjectModel.PlaylistMixerCoreLogic.PlaylistMixerSettings.ConvertStringToSortOptions (HttpContext.Request.Cookies[SpotifyWebAPIWrapper.SpotifyHelpers.sortOptionSettingCookieKey]);
            }

            if (playlistName == null || playlistName == "")
                playlistName = SpotifyWebAPIWrapper.SpotifyHelpers.MakeDefaultPlaylistName ();

            SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyPlaylist generatedPlaylist = mixer.GenerateMaixifyPlaylist (selectedPlaylistsObject, playlistName, isPublic, isCollaborative);
            if (generatedPlaylist == null)
                return RedirectToAction ("Error", new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (400, "Cannot make playlist with the given settings!"));

            return RedirectToAction ("Maixified", new { userId = generatedPlaylist.OwnerUser.Id, playlistId = generatedPlaylist.Id, token = token });
        }


        public IActionResult Maixified (string userId, string playlistId, string token)
        {
            if (token == null)
                return Json (new SpotifyWebAPIWrapper.SpotifyObjectModel.SpotifyError (400, "Spotify token required!"));

            SpotifyWebAPIWrapper.SpotifyAuthorization.SpotifyToken spotifyToken = JsonConvert.DeserializeObject<SpotifyWebAPIWrapper.SpotifyAuthorization.SpotifyToken> (token);
            _spotifyEndpointAccessor.SetAuthorizationToken (spotifyToken);

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
            ViewData["Message"] = "MaiXFIY - " + DateTime.Now.Year;
            return View ();
        }


        public IActionResult Contact ()
        {
            ViewData["Message"] = "Contact us: gerawba@hotmail.com or hegedus21@gmail.com";

            return View ();
        }
    }
}
