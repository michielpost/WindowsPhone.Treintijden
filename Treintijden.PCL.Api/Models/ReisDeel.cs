using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Treintijden.PCL.Api.Models
{
    public class ReisDeel : INotifyPropertyChanged
    {
        public string Vervoerder { get; set; }
        public string VervoerType { get; set; }

      /// <summary>
        /// VOLGENS-PLAN, GEANNULEERD (=vervallen trein), GEWIJZIGD (=planaanpassing in de bijsturing op de dag zelf), OVERSTAP-NIET-MOGELIJK, VERTRAAGD, NIEUW (=extra trein).
      /// </summary>
        public string Status { get; set; }
        public bool IsAankomst { get; set; }

        public List<ReisStop> ReisStops { get; set; }

        public List<ReisStop> TussenStops
        {
            get
            {
                if (ReisStops != null && ReisStops.Count > 2)
                    return ReisStops.Skip(1).Take(ReisStops.Count - 2).ToList();
                else
                    return null;
            }
        }


        public string RegelTwee
        {
            get
            {
                if (ReisStops != null)
                {
                    if (ReisStops.Count <= 1)
                        return VervoerType;


                    if (!string.IsNullOrEmpty(Vervoerder))
                        return string.Format("{0} ({2}) richting {1}", VervoerType, ReisStops.Skip(1).FirstOrDefault().Naam, Vervoerder);
                    else
                        return string.Format("{0} richting {1}", VervoerType, ReisStops.Skip(1).FirstOrDefault().Naam);

                }
                else
                    return VervoerType;
            }

        }

        public string RegelDrie
        {
            get
            {
                if (TussenStops != null && TussenStops.Count > 0)
                {
                    string text = "1 tussenstation";
                    if (TussenStops.Count > 1)
                        text = string.Format("{0} tussenstations", TussenStops.Count);

                    string statusText = this.StatusDisplay;
                    if (!string.IsNullOrEmpty(statusText))
                        text = statusText + ", " + text;

                    return text;

                }
                else
                    return string.Empty;
            }

        }

        public ReisStop FirstStop
        {
            get
            {
                if (ReisStops != null && ReisStops.Count >= 0)
                    return ReisStops.First();
                else
                    return null;
            }
           
        }

        public ReisStop LastStop
        {
            get
            {
                if (ReisStops != null && ReisStops.Count >= 0)
                    return ReisStops.Last();
                else
                    return null;
            }

        }


        public bool HasSingleMessage
        {
            get
            {
                return IsAankomst;
                //if (ReisStops != null && ReisStops.Count > 1)
                //    return false;
                //else
                //    return true;
            }
           
        }

        private bool _isExpanded;

        [XmlIgnore]
        [JsonIgnore]
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value;
            NotifyPropertyChanged("IsExpanded");

            }
        }
        

        public bool IsAlternate { get; set; }

        //[XmlIgnore]
        //[JsonIgnore]
        //public SolidColorBrush BackgroundColor
        //{
        //    get
        //    {
        //        if (IsAlternate)
        //        {
        //            Color a = new Color() { R = 242, G = 242, B = 247, A = 255 };
        //            SolidColorBrush alternateColor = new SolidColorBrush(a);

        //            return alternateColor;
        //        }

        //        return new SolidColorBrush(Colors.White);
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        // NotifyPropertyChanged will raise the PropertyChanged event passing the
        // source property that is being updated.
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        public string StatusDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(Status))
                {
                    if (Status.ToUpper() == "GEANNULEERD")
                        return "geannuleerd";
                    else if (Status.ToUpper() == "NIEUW")
                        return "extra trein";
                }

                return null;

            }
        }
    }
}
