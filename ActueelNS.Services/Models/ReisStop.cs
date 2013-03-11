using Newtonsoft.Json;
using System;
using System.Windows;
using System.Xml.Serialization;

namespace ActueelNS.Services.Models
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
                    return Tijd.Value.ToString("HH:mm");
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
        public Visibility HeeftVertrekspoor
        {
            get
            {
                if (!string.IsNullOrEmpty(Vertrekspoor))
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public Visibility ShowVertrekspoorWijziging
        {
            get
            {
                if (IsVertrekspoorWijziging)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }

    }
}
