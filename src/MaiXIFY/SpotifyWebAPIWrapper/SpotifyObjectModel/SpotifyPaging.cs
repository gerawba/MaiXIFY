using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MaiXIFY.SpotifyWebAPIWrapper.SpotifyObjectModel
{
    public class SpotifyPaging<T>
    {
        [JsonProperty(PropertyName = "total")]
        public int TotalItemNumbers { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<T> Items { get; set; }

        //TODO - next param (URL to the next page of items) - because limit is 100
    }
}
