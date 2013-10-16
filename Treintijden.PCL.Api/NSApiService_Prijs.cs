using System;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Constants;

namespace Treintijden.PCL.Api
{
    public partial class NSApiService : INSApiService
    {
        public async Task<ReisPrijs> GetPrijs(PlannerSearch search)
        {

            string query = string.Format("from={0}&to={1}", search.VanStation.Code, search.NaarStation.Code);

            if (search.ViaStation != null)
                query += "&via=" + search.ViaStation.Code;


           Uri address = new Uri(string.Format(NSApi.BaseUrl + "/ns-api-prijzen-v2?{0}&a={1}", query, DateTime.Now.Ticks), UriKind.Absolute);
            HttpClient webclient = new HttpClient();
            //webclient.Credentials = new NetworkCredential(NSApi.Login, NSApi.Password);

            string response = await webclient.GetStringAsync(address);

            XElement prijzenXmlElement = XElement.Parse(response);

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


            return prijs;
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

      
    }
}
