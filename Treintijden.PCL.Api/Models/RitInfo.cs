using System;
using System.Collections.Generic;
using System.Globalization;

namespace Treintijden.PCL.Api.Models
{

  public class ServiceRitInfo
  {
    public List<RitInfoStop> Stops { get; set; }
    public string CompanyCode { get; set; }
    public string ServiceCode { get; set; }

  }

    public class RitInfoStop
    {
        public string Code { get; set; }
        public string Station { get; set; }
        public DateTime? Arrival { get; set; }
        public DateTime? Departure { get; set; }
        public int? Prognose { get; set; }
        public string ArrivalTimeDelay { get; set; }
        public string DepartureTimeDelay { get; set; }
        public string DeparturePlatform { get; set; }

        public bool IsCurrent { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }

        public string DisplayTijd
        {
            get
            {
                if (Departure.HasValue)
                    return Departure.Value.ToString("HH:mm", CultureInfo.InvariantCulture);
                else if (Arrival.HasValue)
                    return Arrival.Value.ToString("HH:mm", CultureInfo.InvariantCulture);
                else
                    return string.Empty;
            }
        }

        public string DisplayDelay
        {
            get
            {
                if (!string.IsNullOrEmpty(DepartureTimeDelay))
                {
                    var extra = this.DepartureTimeDelay.Replace("PT", string.Empty).Trim();
                    if (!string.IsNullOrEmpty(extra))
                    {
                        return string.Format("+{0}", extra).ToLower();
                    }
                }
               
                
                    return string.Empty;
            }
        }

        public string DisplayArrivalDelay
        {
            get
            {
                if (!string.IsNullOrEmpty(ArrivalTimeDelay))
                {
                    var extra = this.ArrivalTimeDelay.Replace("PT", string.Empty).Trim();
                    if (!string.IsNullOrEmpty(extra))
                    {
                        return string.Format("+{0}", extra).ToLower();
                    }
                }


                return string.Empty;
            }
        }


        public string DisplayArrivalTime
        {
            get
            {
                if (Arrival.HasValue
                    && (!Departure.HasValue || Departure.Value != Arrival.Value)
                    && !IsLast)
                    return "Aankomst: " + Arrival.Value.ToString("HH:mm", CultureInfo.InvariantCulture) + " " + DisplayArrivalDelay;
                else
                    return string.Empty;
            }
        }

        public bool Busy1
        {
            get
            {
                if(Prognose.HasValue)
                    return true;

                return false;
            }
        }

        public bool Busy2
        {
            get
            {
                if (Prognose.HasValue && Prognose.Value >= 50)
                    return true;

                return false;
            }
        }

        public bool Busy3
        {
            get
            {
                if (Prognose.HasValue && Prognose.Value >= 80)
                    return true;

                return false;
            }
        }
    }


}
