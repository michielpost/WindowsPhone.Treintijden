using System;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Constants;
using Treintijden.PCL.Api.Helpers;

namespace Treintijden.PCL.Api
{
    public partial class NSApiService : INSApiService
    {
        private StoringenEnWerkzaamheden _data;
        private DateTime? _syncDate;

        public async Task<StoringenEnWerkzaamheden> GetStoringenEnWerkzaamheden(string station)
        {
            if (_syncDate.HasValue
                && ((DateTime.Now - _syncDate.Value) < new TimeSpan(1, 0, 0))
                && _data != null)
                return _data;

            //ignore station
            station = string.Empty;

            string url = NSApi.BaseUrl + "/ns-api-storingen?actual=true&unplanned=true";

            if(!string.IsNullOrEmpty(station))
                url = string.Format(NSApi.BaseUrl + "/ns-api-storingen?actual=true&planned=false&station={0}", station);

            Uri address = new Uri(url, UriKind.Absolute);
            HttpClient webclient = new HttpClient();
            webclient.MaxResponseContentBufferSize = 9000000;

            //webclient.Credentials = new NetworkCredential(NSApi.Login, NSApi.Password);

            string response = await webclient.GetStringAsync(address);

#if DEBUG
            await Task.Delay(TimeSpan.FromSeconds(2));
#endif

            return await Task.Run(() =>
            {
                //System.Threading.Thread.Sleep(5000);

                XElement storingenXmlElement = XElement.Parse(response);

                List<Storing> storingLijst = ParseStoringen(storingenXmlElement);

                List<Werkzaamheden> werkzaamhedenLijst = ParseWerkzaamheden(storingenXmlElement);

#if DEBUG
                storingLijst.Add(new Storing() { Bericht = "Test" });
#endif


                _syncDate = DateTime.Now;
                _data = new StoringenEnWerkzaamheden();
                _data.Storingen = storingLijst;
                _data.Werkzaamheden = werkzaamhedenLijst;

                return _data;
            });

        }

        private List<Storing> ParseStoringen(XElement storingenXmlElement)
        {
            List<Storing> storingLijst = new List<Storing>();

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

                storing.Id = XmlHelper.GetElementText(element.Element("id"));
                storing.Traject = XmlHelper.GetElementText(element.Element("Traject"));
                storing.Reden = XmlHelper.GetElementText(element.Element("Reden"));
                storing.Bericht = XmlHelper.GetElementText(element.Element("Bericht"));

#if DEBUG
                for (int i = 0; i < 15; i++)
                {
                  storingLijst.Add(storing);

                }
#endif

                storingLijst.Add(storing);
            }


            //for (int i = 0; i < 1; i++)
            //{

            //    Storing a = new Storing() { Traject = "A", Reden = "a" };
            //    Storing b = new Storing() { Traject = "Bbbbb bbbb ", Reden = "aaaa aaa" };
            //    Storing c = new Storing() { Traject = "Bbbbbbbbbbbbbbbbbbbbbbbbbb bbbb bbbbbbbbbbbbbbbbbb", Reden = "a aaaaaaaaaaaaa aaaaa aaaaaa" };

            //    storingLijst.Add(c);

            //    storingLijst.Add(a);
            //    storingLijst.Add(b);



            //}

            return storingLijst;
        }

        private List<Werkzaamheden> ParseWerkzaamheden(XElement storingenXmlElement)
        {
            List<Werkzaamheden> werkzaamhedenLijst = new List<Werkzaamheden>();

            foreach (var element in storingenXmlElement.Element("Gepland").Descendants("Storing"))
            {
                Werkzaamheden werk = new Werkzaamheden();

                werk.Id = XmlHelper.GetElementText(element.Element("id"));
                werk.Traject = XmlHelper.GetElementText(element.Element("Traject"));
                werk.Periode = XmlHelper.GetElementText(element.Element("Periode"));
                werk.Reden = "Oorzaak: " + XmlHelper.GetElementText(element.Element("Reden"));
                werk.Advies = XmlHelper.GetElementText(element.Element("Advies"));
                werk.Vertraging = "Vertraging: " + XmlHelper.GetElementText(element.Element("Vertraging"));

                if (!werkzaamhedenLijst.Where(x => x.Id == werk.Id).Any())
                {
                    werkzaamhedenLijst.Add(werk);
                }
            }
            return werkzaamhedenLijst;
        }
      
    }
}
