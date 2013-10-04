using System;
using AgFx;
using ActueelNS.Services.Models;
using System.Collections.ObjectModel;
using ActueelNS.Services.ViewModels.Context;
using System.Xml.Linq;
using System.Linq;
using ActueelNS.Services.Constants;

namespace ActueelNS.Services.ViewModels
{
    [CachePolicy(CachePolicy.ValidCacheOnly, 60 * 5)]
    public class StoringDataModel : ModelItemBase<LoadContext>
    {
        public StoringDataModel()
        {
        }

        public StoringDataModel(string stad)
            : base(new LoadContext(stad))
        {
        }

      
        /// <summary>
        /// Our collection of weather periods.  
        /// </summary>
        ObservableCollection<Storing> _storingen = new ObservableCollection<Storing>();
        public ObservableCollection<Storing> Storingen
        {
            get
            {
                return _storingen;
            }
            set
            {
                if (_storingen != null)
                {
                    _storingen.Clear();

                    if (value != null)
                    {
                        foreach (var wp in value)
                        {
                            _storingen.Add(wp);
                        }
                    }
                }
                RaisePropertyChanged("Storingen");
            }
        }

        ObservableCollection<Werkzaamheden> _werkzaamheden = new ObservableCollection<Werkzaamheden>();
        public ObservableCollection<Werkzaamheden> Werkzaamheden
        {
            get
            {
                return _werkzaamheden;
            }
            set
            {
                if (_werkzaamheden != null)
                {
                    _werkzaamheden.Clear();

                    if (value != null)
                    {
                        foreach (var wp in value)
                        {
                            _werkzaamheden.Add(wp);
                        }
                    }
                }
                RaisePropertyChanged("Storingen");
            }
        }



        /// <summary>
        /// Our loader, which knows how to do two things:
        /// 1. Build the URI for requesting data for a given zipcode
        /// 2. Parse the return value from that URI
        /// </summary>
        public class StoringDataModelLoader : IDataLoader<LoadContext>
        {
            string StoringenUrl = NSApi.BaseUrl_NS + "/ns-api-storingen?actual=true&unplanned=true";

            /// <summary>
            /// Build a LoadRequest that knows how to fetch new data for our object.
            /// In this case, it's just a URL so we construct the URL and then pass it to the
            /// default WebLoadRequest object, along with our LoadContext
            /// </summary>
            public LoadRequest GetLoadRequest(LoadContext lc, Type objectType)
            {
                return new NSWebLoadRequest_Auth(lc, new Uri(StoringenUrl));
            }

            /// <summary>
            /// Once our LoadRequest has executed, we'll be handed back a stream containing the response from the 
            /// above URI, which we'll parse.
            /// 
            /// Note this will execute in two cases:
            /// 1. When we fetch fresh data from the Internet
            /// 2. When we are deserializing cached data off the disk.  The operation is equivelent at this point.
            /// </summary>
            public object Deserialize(LoadContext lc, Type objectType, System.IO.Stream stream)
            {
                var vm = new StoringDataModel(string.Empty);

                try
                {
                    XElement storingenXmlElement = XElement.Load(stream);


                    ParseStoringen(vm.Storingen, storingenXmlElement);
                    ParseWerkzaamheden(vm.Werkzaamheden, storingenXmlElement);

                    //vm.Storingen.Add(new Storing());
                }
                catch (Exception e)
                {
                }

                return vm;

            }

            private void ParseStoringen(ObservableCollection<Storing> storingLijst, XElement storingenXmlElement)
            {
                foreach (var element in storingenXmlElement.Element("Ongepland").Descendants("Storing"))
                {
                    Storing storing = new Storing();

                    if (element.Element("Datum") != null)
                    {
                        string time = element.Element("Datum").Value;
                        int zoneIndex = time.LastIndexOf('+');
                        if (zoneIndex > 0)
                        {
                            time = time.Substring(0, zoneIndex);
                        }

                        storing.Datum = DateTime.Parse(time);
                    }

                    storing.Id = GetElementText(element.Element("id"));
                    storing.Traject = GetElementText(element.Element("Traject"));
                    storing.Reden = GetElementText(element.Element("Reden"));
                    storing.Bericht = GetElementText(element.Element("Bericht"));



                    storingLijst.Add(storing);
                }
            }

            private void ParseWerkzaamheden(ObservableCollection<Werkzaamheden> werkzaamhedenLijst, XElement storingenXmlElement)
            {

                foreach (var element in storingenXmlElement.Element("Gepland").Descendants("Storing"))
                {
                    Werkzaamheden werk = new Werkzaamheden();

                    werk.Id = GetElementText(element.Element("id"));
                    werk.Traject = GetElementText(element.Element("Traject"));
                    werk.Periode = GetElementText(element.Element("Periode"));
                    werk.Reden = "Oorzaak: " + GetElementText(element.Element("Reden"));
                    werk.Advies = GetElementText(element.Element("Advies"));
                    werk.Vertraging = "Vertraging: " + GetElementText(element.Element("Vertraging"));

                    if (!werkzaamhedenLijst.Where(x => x.Id == werk.Id).Any())
                    {
                        werkzaamhedenLijst.Add(werk);
                    }
                }
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
