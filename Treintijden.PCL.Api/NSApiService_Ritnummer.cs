using System;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Constants;
using System.Net.Http;
using Treintijden.PCL.Api.Helpers;

namespace Treintijden.PCL.Api
{
    public partial class NSApiService : INSApiService
    {
      public async Task<List<ServiceRitInfo>> GetRit(string id, string company, DateTime date)
        {
            string stringDateTime = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "T" + date.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            string query = string.Format("ritnummer={0}&companycode={1}&datetime={2}", id, company, stringDateTime);


            Uri address = new Uri(string.Format(NSApi.BaseUrl + "/mobile-api-serviceinfo?{0}", query), UriKind.Absolute);
            HttpClient webclient = new HttpClient();

            string response = await webclient.GetStringAsync(address);

            XElement prijzenXmlElement = XElement.Parse(response);

            XElement tijdenXmlElement = XElement.Parse(response);

            List<ServiceRitInfo> serviceList = new List<ServiceRitInfo>();

            foreach (var serviceElement in tijdenXmlElement.Descendants("ServiceInfo"))
            {
              ServiceRitInfo info = new ServiceRitInfo();
              info.CompanyCode = XmlHelper.GetElementText(serviceElement.Element("CompanyCode"));

              List<RitInfoStop> stopList = new List<RitInfoStop>();
              info.Stops = stopList;

              foreach (var element in serviceElement.Descendants("Stop"))
              {
                  RitInfoStop stop = new RitInfoStop();

                  stop.Code = XmlHelper.GetElementText(element.Element("StopCode"));
                  stop.DepartureTimeDelay = XmlHelper.GetElementText(element.Element("DepartureTimeDelay"));
                  stop.DeparturePlatform = XmlHelper.GetElementText(element.Element("DeparturePlatform"));
                  stop.ArrivalTimeDelay = XmlHelper.GetElementText(element.Element("ArrivalTimeDelay"));

                  stop.Arrival = XmlHelper.GetDateTime(element, "Arrival");
                  stop.Departure = XmlHelper.GetDateTime(element, "Departure");

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
      
    }
}
