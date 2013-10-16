using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Treintijden.PCL.Api.Models
{
    public class ReisStop
    {
        public string Naam { get; set; }
        public DateTime? Tijd { get; set; }

        public string DisplayTijd
        {
            get
            {
                if (Tijd.HasValue)
                    return Tijd.Value.ToString("HH:mm", CultureInfo.InvariantCulture);
                else
                    return null;
            }
        }

        public string VertragingTekst { get; set; }

        public string Vertrekspoor { get; set; }

        public string VertrekspoorTwee
        {
            get
            {
                if (Vertrekspoor == null)
                    return Vertrekspoor;

                if (Vertrekspoor.Length <= 2)
                    return Vertrekspoor;
                else
                    return null;
            }
        }

        public string VertrekspoorDrie
        {
            get
            {
                if (Vertrekspoor == null)
                    return Vertrekspoor;

                if (Vertrekspoor.Length == 3)
                    return Vertrekspoor;
                else
                    return null;
            }
        }


        public bool IsVertrekspoorWijziging { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool HeeftVertrekspoor
        {
            get
            {
                if (!string.IsNullOrEmpty(Vertrekspoor))
                    return true;
                else
                    return false;
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public bool ShowVertrekspoorWijziging
        {
            get
            {
                if (IsVertrekspoorWijziging)
                    return false;
                else
                    return true;
            }
        }

    }
}
