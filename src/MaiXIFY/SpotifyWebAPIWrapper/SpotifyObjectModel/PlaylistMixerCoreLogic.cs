using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaiXIFY.SpotifyWebAPIWrapper.SpotifyObjectModel
{
    public class PlaylistMixerCoreLogic
    {
        private SpotifyWebAPIWrapper.ISpotifyEndpointAccessor _spotifyEndpointAccessor;
        public  PlaylistMixerSettings Settings { get; set; }


        public PlaylistMixerCoreLogic (SpotifyWebAPIWrapper.ISpotifyEndpointAccessor spotifyEndpointAccessor)
        {
            _spotifyEndpointAccessor = spotifyEndpointAccessor;
            Settings = new PlaylistMixerSettings ();
        }


        public SpotifyPlaylist GenerateMaixifyPlaylist (List<SpotifyPlaylist> selectedPlaylists, string playlistName = "MaiXIFY :)", bool isPublic = true, bool isCollaborative = false)
        {
            if (selectedPlaylists.Count < 2)
                return null;

            if (Settings.Threshold <= 0.0 || Settings.Threshold > 1.0)
                return null;

            var playlistNumber = selectedPlaylists.Count;

            var tracksFrequency = new Dictionary<string, SpotifyHelpers.TrackInfo> ();
            var artistsFrequency = new Dictionary<string, int> ();

            foreach (SpotifyPlaylist playlist in selectedPlaylists) {
                SpotifyPaging<SpotifyPlaylistTrack> playlistTrackPaging = playlist.Tracks;
                List<SpotifyPlaylistTrack> playlistTrackList = playlistTrackPaging.Items;

                foreach (SpotifyPlaylistTrack playlistTrack in playlistTrackList) {
                    SpotifyTrack track = playlistTrack.Track;
                    string trackId = track.Id;

                    if (trackId == null)
                        continue;

                    string trackArtist = track.Artists[0].Id;
                    int trackPopularity = track.Popularity;

                    if (!tracksFrequency.ContainsKey (trackId)) {
                        tracksFrequency[trackId] = new SpotifyHelpers.TrackInfo ();
                        tracksFrequency[trackId].HitCount = 0;
                        tracksFrequency[trackId].Popularity = trackPopularity;
                    }
                    tracksFrequency[trackId].HitCount = tracksFrequency[trackId].HitCount + 1;

                    if (Settings.RecommendedMusic) {
                        if (!artistsFrequency.ContainsKey (trackArtist))
                            artistsFrequency[trackArtist] = 0;
                        artistsFrequency[trackArtist] = artistsFrequency[trackArtist] + 1;
                    }

                }
            }

            List<KeyValuePair<string, SpotifyHelpers.TrackInfo>> sortedTracks = new List<KeyValuePair<string, SpotifyHelpers.TrackInfo>> ();
            if (Settings.SortOption == PlaylistMixerSettings.SortOptions.MostHit)
                sortedTracks = tracksFrequency.OrderByDescending (t => t.Value.HitCount).ToList ();
            else if (Settings.SortOption == PlaylistMixerSettings.SortOptions.Popularity)
                sortedTracks = tracksFrequency.OrderByDescending(t => t.Value.Popularity).ToList();
            else if (Settings.SortOption == PlaylistMixerSettings.SortOptions.Random)
                sortedTracks = tracksFrequency.ToList ();

            List<string> recommendedTrackUriList = new List<string> ();

            foreach (var track in sortedTracks) {
                if (track.Value.HitCount >= (playlistNumber * Settings.Threshold) && track.Value.HitCount >= 2) {
                    recommendedTrackUriList.Add (SpotifyHelpers.trackUri + track.Key);
                }
            }

            if (recommendedTrackUriList.Count < 1 || (recommendedTrackUriList.Count < 10 && Settings.RecommendedMusic)) {
                if (Settings.RecommendedMusic == false)
                    return null;
                else {
                    recommendedTrackUriList.AddRange (GenerateRecommendedPlaylist (sortedTracks, artistsFrequency));
                    recommendedTrackUriList = recommendedTrackUriList.Distinct ().ToList ();
                }
            }

            string userId = _spotifyEndpointAccessor.GetCurrentUserProfile ().Id;
            SpotifyPlaylist generatedPlaylist = _spotifyEndpointAccessor.CreatePlaylist (userId, playlistName, isPublic, isCollaborative);

            if (_spotifyEndpointAccessor.AddTracksToPlaylist (userId, generatedPlaylist.Id, recommendedTrackUriList) == false)
                return null;

            return generatedPlaylist;
        }


        private List<string> GenerateRecommendedPlaylist (List<KeyValuePair<string, SpotifyHelpers.TrackInfo>> gatheredTracks, Dictionary<string, int> artistsFrequency)
        {
            if (gatheredTracks == null || artistsFrequency == null)
                return null;

            List<string> recommendedTrackUriList = new List<string> ();

            // add some tracks from user's playlists if it is poupular (>77) right now
            gatheredTracks = gatheredTracks.Distinct ().OrderByDescending (p => p.Value.Popularity).ToList ();
            foreach (var track in gatheredTracks) {
                if (track.Value.Popularity > 77)
                    recommendedTrackUriList.Add (SpotifyHelpers.trackUri + track.Key);
            }

            // add the top 3 tracks from user's playlists most frequent artists
            int topArtistsFrequencyLimit = (from artist in artistsFrequency
                                            orderby artist.Value descending
                                            select artist.Value).Distinct ().Skip (2).First ();

            if (topArtistsFrequencyLimit < 2)
                return recommendedTrackUriList;

            List<string> topArtists = artistsFrequency.Where (a => (a.Value >= topArtistsFrequencyLimit)).Select (a => a.Key).ToList ();

            List<SpotifyTrack> topArtistsMostPopularTracks = new List<SpotifyTrack> ();
            foreach (string artist in topArtists)
                topArtistsMostPopularTracks.AddRange (_spotifyEndpointAccessor.GetArtistsTopTracks (artist).Take (3));

            foreach (var track in topArtistsMostPopularTracks) 
                    recommendedTrackUriList.Add (SpotifyHelpers.trackUri + track.Id);

            return recommendedTrackUriList.Distinct ().ToList ();
        }


        public class PlaylistMixerSettings
        {
            public double Threshold { get; set; }
            public bool RecommendedMusic { get; set; }
            public SortOptions SortOption { get; set; }

            public PlaylistMixerSettings ()
            {
                Threshold = 1.0;
                RecommendedMusic = false;
                SortOption = SortOptions.MostHit;
            }

            public enum SortOptions {
                MostHit,
                Popularity,
                Random
            };


            public static SortOptions ConvertStringToSortOptions (string sortOption)
            {
                switch (sortOption)
                {
                    case "mostHit":
                        return SortOptions.MostHit;
                    case "popularity":
                        return SortOptions.Popularity;
                    case "random":
                    default:
                        return SortOptions.Random;
                }
            }
        }
    }
}
