using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaiXIFY.SpotifyWebAPIWrapper.SpotifyObjectModel;

namespace MaiXIFY.SpotifyWebAPIWrapper
{
    public interface ISpotifyEndpointAccessor
    {
        void SetAuthorizationToken (SpotifyWebAPIWrapper.SpotifyAuthorization.SpotifyToken token);

        SpotifyUser GetCurrentUserProfile ();

        SpotifyUser GetUserProfile (string userId);

        SpotifyTrack GetTrack (string trackId);

        List<SpotifyTrack> GetArtistsTopTracks (string artistId);

        List<SpotifyPlaylistSimplified> GetUserPlaylists (string userId);

        SpotifyPlaylist GetPlaylist (string userId, string playlistId);

        List<SpotifyPlaylist> GetPlaylists (List<SpotifyWebAPIWrapper.SpotifyHelpers.SelectedPlaylistElem> selectedPlaylists);

        SpotifyPlaylist CreatePlaylist (string userId, string playlistName, bool isPublic = true, bool isCollaborative = false);

        bool AddTracksToPlaylist (string userId, string playlistId, List<string> trackUriList);
    }
}
