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
        }

        public SpotifyPlaylist GenerateRecommendedPlaylist (List<SpotifyPlaylist> selectedPlaylists)
        {
            
            return null;
        }


        public class PlaylistMixerSettings
        {
            public double Threshold { get; set; }
            public bool RecommendedMusic { get; set; }
            public SortOptions SortOption { get; set; }

            public enum SortOptions {
                MostHit,
                SimilarGenre,
                Random
            };

            public static SortOptions ConvertStringToSortOptions (string sortOption)
            {
                switch (sortOption)
                {
                    case "mostHit":
                        return SortOptions.MostHit;
                    case "similarGenre":
                        return SortOptions.SimilarGenre;
                    case "random":
                    default:
                        return SortOptions.Random;
                }
            }
        }
    }
}
