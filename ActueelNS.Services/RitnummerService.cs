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
using System.Globalization;

namespace ActueelNS.Services
{
    public class RitnummerService : IRitnummerService
    {
      public async Task<List<ServiceRitInfo>> GetRit(string id, string company, DateTime date)
        {
            string stringDateTime = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "T" + date.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            string query = string.Format("ritnummer={0}&companycode={1}&datetime={2}", id, company, stringDateTime);


            Uri address = new Uri(string.Format(NSApi.BaseUrl + "/mobile-api-serviceinfo?{0}", query), UriKind.Absolute);
            WebClient webclient = new WebClient();

            string response = await webclient.DownloadStringTaskAsync(address);

            XElement prijzenXmlElement = XElement.Parse(response);

            XElement tijdenXmlElement = XElement.Parse(response);

            List<ServiceRitInfo> serviceList = new List<ServiceRitInfo>();

            foreach (var serviceElement in tijdenXmlElement.Descendants("ServiceInfo"))
            {
              ServiceRitInfo info = new ServiceRitInfo();
              info.CompanyCode = GetElementText(serviceElement.Element("CompanyCode"));

              List<RitInfoStop> stopList = new List<RitInfoStop>();
              info.Stops = stopList;

              foreach (var element in serviceElement.Descendants("Stop"))
              {
                  RitInfoStop stop = new RitInfoStop();

                  stop.Code = GetElementText(element.Element("StopCode"));
                  stop.DepartureTimeDelay = GetElementText(element.Element("DepartureTimeDelay"));
                  stop.DeparturePlatform = GetElementText(element.Element("DeparturePlatform"));
                  stop.ArrivalTimeDelay = GetElementText(element.Element("ArrivalTimeDelay"));

                  stop.Arrival = GetDateTime(element, "Arrival");
                  stop.Departure = GetDateTime(element, "Departure");

                  if (element.Element("prognose") != null)
                    stop.Prognose = int.Parse(element.Element("prognose").Value);

                  if (stop.Arrival.HasValue || stop.Departure.HasValue)
                    stopList.Add(stop);
              }

              if (stopList.Any())
              {
                stopList.First().IsFirst = true;
                stopList.Last().IsLast = true;

                serviceList.Add(info);
              }
            }

            return serviceList;
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
