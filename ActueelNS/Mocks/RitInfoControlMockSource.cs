using ActueelNS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActueelNS.Mocks
{
    public class RitInfoControlMockSource : RitInfoStop
    {
        public RitInfoControlMockSource()
        {
            Station = "Delft";
            this.Departure = DateTime.Now;
            this.Arrival = DateTime.Now;
        }
    }
}
