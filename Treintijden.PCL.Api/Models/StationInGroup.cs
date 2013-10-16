using System.Collections.Generic;

namespace Treintijden.PCL.Api.Models
{
    public class StationInGroup : List<Station>
    {
        public StationInGroup(string category)
        {
            Key = category;
        }

        public string Key { get; set; }

        public bool HasItems { get { return Count > 0; } }
    }
}