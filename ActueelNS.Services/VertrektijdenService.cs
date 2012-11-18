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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using ActueelNS.Services.Constants;

namespace ActueelNS.Services
{
    public class VertrektijdenService : IVertrektijdenService
    {
        public async Task<List<Vertrektijd>> GetVertrektijden(string station)
        {
            Uri address = new Uri(string.Format("http://webservices.ns.nl/ns-api-avt?station={0}&a={1}", station, DateTime.Now.Ticks), UriKind.Absolute);
            WebClient webclient = new WebClient();
            webclient.Credentials = new NetworkCredential(NSApi.Login, NSApi.Password);

            string response = await webclient.DownloadStringTaskAsync(address);

            return await TaskEx.Run(() =>
             {

            //System.Threading.Thread.Sleep(5000);

            XElement tijdenXmlElement = XElement.Parse(response);

            List<Vertrektijd> vertrektijdList = new List<Vertrektijd>();

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



                vertrektijdList.Add(tijd);
            }

            //await TaskEx.Delay(TimeSpan.FromSeconds(5));  

            return vertrektijdList;
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
