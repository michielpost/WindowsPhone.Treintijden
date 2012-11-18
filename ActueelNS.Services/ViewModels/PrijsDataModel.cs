using System;
using ActueelNS.Services.ViewModels.Context;
using AgFx;
using ActueelNS.Services.Models;
using System.Xml.Linq;
using System.Linq;
using ActueelNS.Services.Constants;

namespace ActueelNS.Services.ViewModels
{
    [CachePolicy(CachePolicy.Forever)]
    public class PrijsDataModel: ModelItemBase<PrijsLoadContext>
    {
        public PrijsDataModel()
        {
        }

        public PrijsDataModel(PlannerSearch search)
            : base(new PrijsLoadContext(search))
        {

                     
        }


        /// <summary>
        /// Our collection of weather periods.  
        /// </summary>
        ReisPrijs _prijs;
        public ReisPrijs Prijs
        {
            get
            {
                return _prijs;
            }
            set
            {
                if (_prijs != value)
                {
                    _prijs = value;
                    RaisePropertyChanged("Prijs");
                }
            }
        }



        /// <summary>
        /// Our loader, which knows how to do two things:
        /// 1. Build the URI for requesting data for a given zipcode
        /// 2. Parse the return value from that URI
        /// </summary>
        public class PrijsDataModelLoader : IDataLoader<PrijsLoadContext>
        {
            string PrijzenUrl = NSApi.BaseUrl + "/ns-api-prijzen-v2?{0}&a={1}";

            /// <summary>
            /// Build a LoadRequest that knows how to fetch new data for our object.
            /// In this case, it's just a URL so we construct the URL and then pass it to the
            /// default WebLoadRequest object, along with our PrijsLoadContext
            /// </summary>
            public LoadRequest GetLoadRequest(PrijsLoadContext lc, Type objectType)
            {
                var search = lc.PlannerSearch;

                string query = string.Format("from={0}&to={1}", search.VanStation.Code, search.NaarStation.Code);

                if (search.ViaStation != null)
                    query += "&via=" + search.ViaStation.Code;


                Uri address = new Uri(string.Format(PrijzenUrl, query, DateTime.Now.Ticks), UriKind.Absolute);


                return new NSWebLoadRequest(lc, address);
            }

            /// <summary>
            /// Once our LoadRequest has executed, we'll be handed back a stream containing the response from the 
            /// above URI, which we'll parse.
            /// 
            /// Note this will execute in two cases:
            /// 1. When we fetch fresh data from the Internet
            /// 2. When we are deserializing cached data off the disk.  The operation is equivelent at this point.
            /// </summary>
            public object Deserialize(PrijsLoadContext lc, Type objectType, System.IO.Stream stream)
            {
                PrijsDataModel vm = new PrijsDataModel(lc.PlannerSearch);

                try
                {
                    XElement prijzenXmlElement = XElement.Load(stream);



                    ReisPrijs prijs = new ReisPrijs();

                    //Get enkele reis
                    var enkel = prijzenXmlElement.Descendants("Product")
                        .Where(x => x.Attribute("naam") != null && x.Attribute("naam").Value == "Enkele reis")
                        .FirstOrDefault();

                    if (enkel != null)
                    {
                        prijs.Enkel_1_Vol = GetPrijsValue(enkel, "1", "vol tarief");
                        prijs.Enkel_1_20 = GetPrijsValue(enkel, "1", "reductie_20");
                        prijs.Enkel_1_40 = GetPrijsValue(enkel, "1", "reductie_40");

                        prijs.Enkel_2_Vol = GetPrijsValue(enkel, "2", "vol tarief");
                        prijs.Enkel_2_20 = GetPrijsValue(enkel, "2", "reductie_20");
                        prijs.Enkel_2_40 = GetPrijsValue(enkel, "2", "reductie_40");


                    }


                    //Get Retour
                    var retour = prijzenXmlElement.Descendants("Product")
                        .Where(x => x.Attribute("naam") != null && x.Attribute("naam").Value == "Dagretour")
                        .FirstOrDefault();
                    if (retour != null)
                    {
                        prijs.Dag_1_Vol = GetPrijsValue(retour, "1", "vol tarief");
                        prijs.Dag_1_20 = GetPrijsValue(retour, "1", "reductie_20");
                        prijs.Dag_1_40 = GetPrijsValue(retour, "1", "reductie_40");

                        prijs.Dag_2_Vol = GetPrijsValue(retour, "2", "vol tarief");
                        prijs.Dag_2_20 = GetPrijsValue(retour, "2", "reductie_20");
                        prijs.Dag_2_40 = GetPrijsValue(retour, "2", "reductie_40");

                    }



                    vm.Prijs = prijs;
                }
                catch (Exception e)
                {
                }

                return vm;



            }

            private string GetPrijsValue(XElement enkel, string klasse, string korting)
            {
                try
                {
                    return "€ " + enkel.Descendants().Where(x => x.Attribute("klasse").Value == klasse).Where(x => x.Attribute("korting").Value == korting).FirstOrDefault().Value;
                }
                catch
                {
                }

                return string.Empty;
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
