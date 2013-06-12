using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActueelNS.Services.Models
{

    public class RitInfoStop
    {
        public string Code { get; set; }
        public string Station { get; set; }
        public DateTime? Arrival { get; set; }
        public DateTime? Departure { get; set; }
        public int? Prognose { get; set; }

        public bool IsCurrent { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }

        public string DisplayTijd
        {
            get
            {
                if (Departure.HasValue)
                    return Departure.Value.ToString("HH:mm");
                else if (Arrival.HasValue)
                    return Arrival.Value.ToString("HH:mm");
                else
                    return string.Empty;
            }
        }

        public string DisplayArrivalTime
        {
            get
            {
                if (Arrival.HasValue)
                    return "Aankomst: " + Arrival.Value.ToString("HH:mm");
                else
                    return string.Empty;
            }
        }
    }


}
