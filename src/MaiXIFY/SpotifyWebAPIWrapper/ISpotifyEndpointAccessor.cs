using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaiXIFY.SpotifyWebAPIWrapper.SpotifyObjectModel;

namespace MaiXIFY.SpotifyWebAPIWrapper
{
    public interface ISpotifyEndpointAccessor
    {
        SpotifyUser GetCurrentUserProfile ();

        List<SpotifyPlaylistSimplified> GetUserPlaylists (string userId);

        SpotifyPlaylist GetPlaylist (string userId, string playlistId);

        List<SpotifyPlaylist> GetPlaylists (List<SpotifyWebAPIWrapper.SpotifyHelpers.SelectedPlaylistElem> selectedPlaylists);

        SpotifyPlaylist CreatePlaylist (string userId, string playlistName, bool isPublic = true, bool isCollaborative = false);

        bool AddTrackToPlaylist (string userId, string playlistId, List<string> trackUriList);
    }
}
