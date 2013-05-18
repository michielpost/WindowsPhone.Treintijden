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
using System.Xml.Linq;
using ActueelNS.Services.Models;
using ActueelNS.Services.Constants;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace ActueelNS.Services
{
    public class RitnummerService : IRitnummerService
    {
        public async Task<List<RitInfoStop>> GetRit(string id, string company, DateTime date)
        {
            string stringDateTime = date.ToString("yyyy-MM-dd") + "T" + date.ToString("HH:mm:ss");
            string query = string.Format("ritnummer={0}&companycode={1}&datetime={2}", id, company, stringDateTime);


            Uri address = new Uri(string.Format(NSApi.BaseUrl + "/mobile-api-serviceinfo?{0}", query), UriKind.Absolute);
            WebClient webclient = new WebClient();

            string response = await webclient.DownloadStringTaskAsync(address);

            XElement prijzenXmlElement = XElement.Parse(response);

            XElement tijdenXmlElement = XElement.Parse(response);

            List<RitInfoStop> stopList = new List<RitInfoStop>();

            foreach (var element in tijdenXmlElement.Descendants("Stop"))
            {
                RitInfoStop stop = new RitInfoStop();

                stop.Code = GetElementText(element.Element("StopCode"));

                stop.Arrival = GetDateTime(element, "Arrival");
                stop.Departure = GetDateTime(element, "Departure");

                if (element.Element("prognose") != null)
                    stop.Prognose = int.Parse(element.Element("prognose").Value);

                if (stop.Arrival.HasValue || stop.Departure.HasValue)
                    stopList.Add(stop);
            }


            return stopList;
        }


        private string GetElementText(XElement element)
        {
            if (element != null)
                return element.Value;


            return null;
        }

        private static DateTime? GetDateTime(XElement element, string name)
        {
            DateTime? dtime = null;
            if (element.Element(name) != null)
            {
                string time = element.Element(name).Value;
                int zoneIndex = time.LastIndexOf('+');
                if (zoneIndex > 0)
                {
                    time = time.Substring(0, zoneIndex);
                }

                if (!string.IsNullOrEmpty(time))
                    dtime = DateTime.Parse(time);
            }

            return dtime;
        }
    }
}
