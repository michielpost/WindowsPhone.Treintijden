using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace ReadStationXmlConsole
{

    public class Station
    {
        public Station()
        {
            NamesExtra = new List<string>();
        }
        public string Name { get; set; }
        public string NameKort { get; set; }
        public string NameMiddel { get; set; }


        public string Code { get; set; }

        public string Country { get; set; }

        public double Lat { get; set; }

        public double Long { get; set; }

        public List<string> NamesExtra { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }

   

    class Program
    {
        static void Main(string[] args)
        {

            XElement stationXmlElement2 = XDocument.Load("stationslijst-v2.xml").Elements().First();

            var list2 = (from x in stationXmlElement2.Descendants("Station")
                         where x.Element("Type").Value != "facultatiefStation"
                        select new Station
                        {
                            Name = x.Element("Namen").Element("Lang").Value,
                            NameKort = x.Element("Namen").Element("Kort").Value,
                            NameMiddel = x.Element("Namen").Element("Middel").Value,
                            NamesExtra = x.Descendants("Synoniem").Select(s => s.Value).ToList(),
                            Code = x.Element("Code").Value.ToLower(),
                            Country = x.Element("Land").Value,
                            //Alias = bool.Parse(x.Element("alias").Value),
                            Lat = double.Parse(x.Element("Lat").Value, CultureInfo.InvariantCulture),
                            Long = double.Parse(x.Element("Lon").Value, CultureInfo.InvariantCulture)
                        });



            if (list2 != null)
            {
              WriteListToFile(list2, "stations_all.xml");
              WriteListToFile(list2.Where(x => x.Country == "NL"), "stations.xml", "NL");
            }


        }

        private static void WriteListToFile(IEnumerable<Station> list2, string fileName, string country = null)
        {
          XDocument newDoc = new XDocument();

          XElement stations = new XElement("sts");

          foreach (var item in list2)
          {

            item.NamesExtra.Add(item.Name);
            item.NamesExtra.Add(item.NameKort);
            item.NamesExtra.Add(item.NameMiddel);

            var reduced = item.NamesExtra.Distinct().ToList();
            var extraReduced = reduced.Where(x => x != item.Name && !item.Name.StartsWith(x));

            item.NamesExtra = extraReduced.ToList();


            XElement Synoniemen = new XElement("Syns");

            foreach (var s in item.NamesExtra)
            {
              Synoniemen.Add(new XElement("Sy", s));
            }

            string itemCountry = country;
            if (string.IsNullOrEmpty(itemCountry))
              itemCountry = item.Country;

            stations.Add(new XElement("s",
            new XElement("name", item.Name),
            new XElement("id", item.Code),
            new XElement("c", itemCountry),
            new XElement("lat", item.Lat),
            new XElement("lon", item.Long), Synoniemen
            ));

          }

          newDoc.Add(stations);

          newDoc.Save(fileName);
        }
    }
}
