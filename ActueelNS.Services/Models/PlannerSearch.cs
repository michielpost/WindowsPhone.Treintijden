using System;
using System.Text;

namespace ActueelNS.Services.Models
{
    public class PlannerSearch
    {
        public Station VanStation { get; set; }
        public Station NaarStation { get; set; }
        public Station ViaStation { get; set; }

        public DateTime Date { get; set; }
        public DateTime Time { get; set; }

        public bool IsHogesnelheid { get; set; }
        public bool IsYearCard { get; set; }

        public string Type { get; set; }

        public Guid Id { get; set; }
        public DateTime SearchDateTime { get; set; }

        public string DisplayTijd
        {
            get
            {
                return Date.ToString("dd-MM-yyyy") + " " + Time.ToString("HH:mm");
            }
        }

        public string DisplayDag
        {
            get
            {
                return Date.ToString("dd-MM-yyyy");
            }
        }

        public string DisplayVia
        {
            get
            {
                if (ViaStation != null)
                    return "via " + ViaStation.Name;
                else
                    return string.Empty;
            }
        }

        public string DisplayFull
        {
            get
            {
                return string.Format("{0} - {1} {2}", VanStation.Name, NaarStation.Name, DisplayVia);
            }
        }


        public string GetUniqueId()
        {
            StringBuilder sb = new StringBuilder();

            if (VanStation != null)
                sb.AppendFormat("_{0}_", VanStation.Code);

            if (NaarStation != null)
                sb.AppendFormat("_{0}_", NaarStation.Code);

            if (ViaStation != null)
                sb.AppendFormat("_{0}_", ViaStation.Code);

            return sb.ToString();

        }



    }
}
