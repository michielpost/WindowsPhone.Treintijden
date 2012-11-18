using AgFx;

namespace ActueelNS.Services.ViewModels.Context
{
    public class StationLoadContext : LoadContext {
        public string Station {
            get {
                return (string)Identity;
            }            
        }

        public StationLoadContext(string station)
            : base(station) {

        }
    }
}
