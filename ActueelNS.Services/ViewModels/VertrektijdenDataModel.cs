using System;
using System.Windows;
using System.Windows.Media;
using AgFx;
using ActueelNS.Services.ViewModels.Context;
using ActueelNS.Services.Models;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using ActueelNS.Services.Constants;

namespace ActueelNS.Services.ViewModels
{
    [CachePolicy(CachePolicy.ValidCacheOnly, 45)]
    public class VertrektijdenDataModel : ModelItemBase<StationLoadContext>
    {
        public VertrektijdenDataModel()
        {
        }

        public VertrektijdenDataModel(string station)
            : base(new StationLoadContext(station))
        {
            
                     
        }


        /// <summary>
        /// Our collection of weather periods.  
        /// </summary>
        ObservableCollection<Vertrektijd> _vertrektijden = new ObservableCollection<Vertrektijd>();
        public ObservableCollection<Vertrektijd> Vertrektijden
        {
            get
            {
                return _vertrektijden;
            }
            set
            {
                if (_vertrektijden != null)
                {
                    _vertrektijden.Clear();

                    if (value != null)
                    {
                        foreach (var wp in value)
                        {
                            _vertrektijden.Add(wp);
                        }
                    }
                }
                RaisePropertyChanged("Vertrektijden");
            }
        }



        /// <summary>
        /// Our loader, which knows how to do two things:
        /// 1. Build the URI for requesting data for a given zipcode
        /// 2. Parse the return value from that URI
        /// </summary>
        public class VertrektijdenDataModelLoader : IDataLoader<StationLoadContext>
        {
            string TijdenUrl = NSApi.BaseUrl + "/ns-api-avt?station={0}&a={1}";

            /// <summary>
            /// Build a LoadRequest that knows how to fetch new data for our object.
            /// In this case, it's just a URL so we construct the URL and then pass it to the
            /// default WebLoadRequest object, along with our StationLoadContext
            /// </summary>
            public LoadRequest GetLoadRequest(StationLoadContext lc, Type objectType)
            {
                return new NSWebLoadRequest(lc, new Uri(string.Format(TijdenUrl, lc.Station, DateTime.Now.Ticks)));
            }

            /// <summary>
            /// Once our LoadRequest has executed, we'll be handed back a stream containing the response from the 
            /// above URI, which we'll parse.
            /// 
            /// Note this will execute in two cases:
            /// 1. When we fetch fresh data from the Internet
            /// 2. When we are deserializing cached data off the disk.  The operation is equivelent at this point.
            /// </summary>
            public object Deserialize(StationLoadContext lc, Type objectType, System.IO.Stream stream)
            {

                VertrektijdenDataModel vm = new VertrektijdenDataModel(lc.Station);



                try
                {
                    XElement tijdenXmlElement = XElement.Load(stream);

                    foreach (var element in tijdenXmlElement.Descendants("VertrekkendeTrein"))
                    {
                        Vertrektijd tijd = new Vertrektijd();

                        if (element.Element("RitNummer") != null)
                            tijd.Ritnummer = int.Parse(element.Element("RitNummer").Value);

                        if (element.Element("VertrekTijd") != null)
                        {
                            string time = element.Element("VertrekTijd").Value;
                            int zoneIndex = time.LastIndexOf('+');
                            if (zoneIndex > 0)
                            {
                                time = time.Substring(0, zoneIndex);
                            }

                            tijd.Tijd = DateTime.Parse(time);
                        }

                        tijd.Vertraging = GetElementText(element.Element("VertrekVertraging"));
                        tijd.VertragingTekst = GetElementText(element.Element("VertrekVertragingTekst"));
                        tijd.Eindbestemming = GetElementText(element.Element("EindBestemming"));
                        tijd.TreinSoort = GetElementText(element.Element("TreinSoort"));
                        tijd.Route = GetElementText(element.Element("RouteTekst"));
                        tijd.ReisTip = GetElementText(element.Element("ReisTip"));

                        tijd.Vertrekspoor = GetElementText(element.Element("VertrekSpoor"));

                        if (element.Element("VertrekSpoor") != null
                            && element.Element("VertrekSpoor").Attribute("wijziging") != null)
                        {
                            tijd.IsVertrekspoorWijziging = bool.Parse(element.Element("VertrekSpoor").Attribute("wijziging").Value);
                        }

                        string sep = string.Empty;
                        foreach (var opm in element.Descendants("Opmerking"))
                        {
                            tijd.Opmerkingen += string.Format("{0}{1}", sep, opm.Value)
                                .Replace("\r", string.Empty)
                                .Replace("\t", string.Empty)
                                .Replace("\n", string.Empty)
                                .Trim();
                            sep = ", ";
                        }



                        vm.Vertrektijden.Add(tijd);



                    }
                }
                catch
                {
                }


                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    bool _useAlternate = false;

                    SolidColorBrush backgroundColor = new SolidColorBrush(Colors.White);

                    Color a = new Color() { R = 242, G = 242, B = 247, A = 255 };
                    SolidColorBrush alternateColor = new SolidColorBrush(a);

                    if (vm.Vertrektijden != null)
                    {

                        foreach (var tijd in vm.Vertrektijden)
                        {
                            ////Set background color here, for performance
                            if (_useAlternate)
                            {
                                tijd.BackgroundColor = alternateColor;

                            }
                            else
                            {
                                tijd.BackgroundColor = backgroundColor;
                            }


                            _useAlternate = !_useAlternate;

                        }
                    }
                });


                return vm;



            }

            private string GetElementText(XElement element)
            {
                if (element != null)
                    return element.Value;


                return null;
            }
        }

    }
}
