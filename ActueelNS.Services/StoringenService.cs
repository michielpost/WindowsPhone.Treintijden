using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ActueelNS.Services.Interfaces;
using ActueelNS.Services.Models;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ActueelNS.Services.Constants;

namespace ActueelNS.Services
{
    public class StoringenService : IStoringenService
    {
        private List<Storing> _storingen;
        private DateTime? _syncDate;

        public async Task<List<Storing>> GetStoringen(string station)
        {
            if (_syncDate.HasValue
                && ((DateTime.Now - _syncDate.Value) < new TimeSpan(1, 0, 0))
                && _storingen != null)
                return _storingen;

            //ignore station
            station = string.Empty;

            string url = "http://webservices.ns.nl/ns-api-storingen?actual=true&planned=false";

            if(!string.IsNullOrEmpty(station))
                url = string.Format("http://webservices.ns.nl/ns-api-storingen?actual=true&planned=false&station={0}", station);

            Uri address = new Uri(url, UriKind.Absolute);
            WebClient webclient = new WebClient();
            webclient.Credentials = new NetworkCredential(NSApi.Login, NSApi.Password);

            string response = await webclient.DownloadStringTaskAsync(address);

            return await TaskEx.Run(() =>
            {
                //System.Threading.Thread.Sleep(5000);

                XElement storingenXmlElement = XElement.Parse(response);

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

                    storing.Id = GetElementText(element.Element("id"));
                    storing.Traject = GetElementText(element.Element("Traject"));
                    storing.Reden = GetElementText(element.Element("Reden"));
                    storing.Bericht = GetElementText(element.Element("Bericht"));



                    storingLijst.Add(storing);
                }

                //await TaskEx.Delay(TimeSpan.FromSeconds(5));  

                _syncDate = DateTime.Now;
                _storingen = storingLijst;

                return storingLijst;
            });

        }


        private string GetElementText(XElement element)
        {
            if (element != null)
                return element.Value;


            return null;
        }
    }
}
