using System.Device.Location;
using System.Linq;

namespace ActueelNS.Services.Models
{
    /// <summary>
    /// <station>
    ///    <name>'s-Gravenhage</name>
    ///    <code>gvc</code>
    ///    <country>NL</country>
    ///    <lat>52.081261</lat>
    ///    <long>4.323973</long>
    ///    <alias>true</alias>
    /// </station>
    /// </summary>
    public class Station
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Country { get; set; }

        public double Lat { get; set; }

        public double Long { get; set; }

        public string[] NamesExtra { get; set; }

        public bool StartsWith(string search)
        {
            if (Name.ToLower().StartsWith(search))
                return true;

            if (NamesExtra.Where(x => x.ToLower().StartsWith(search)).Any())
                return true;

            if (Code.StartsWith(search))
                return true;

            return false;
        }



        //public bool Alias { get; set; }

        public Station ShallowCopy()
        {
            return (Station)this.MemberwiseClone();
        }

        //public override bool Equals(object obj)
        //{
        //    if (this != null && obj != null && obj is Station)
        //    {
        //        return this.Code.ToLowerInvariant() == ((Station)obj).Code.ToLowerInvariant();
        //    }
        //    else
        //        return base.Equals(obj);
        //}


        public static string GetFirstNameKey(Station person)
        {
            char key = char.ToLower(person.Name[0]);

            if (key < 'a' || key > 'z')
            {
                key = '#';
            }

            return key.ToString();
        }

        public static int CompareByFirstName(object obj1, object obj2)
        {
            Station p1 = (Station)obj1;
            Station p2 = (Station)obj2;

            int result = p1.Name.CompareTo(p2.Name);
            if (result == 0)
            {
                result = p1.Name.CompareTo(p2.Name);
            }

            return result;
        }

        private double _distance;

        public double Distance
        {
            get { return _distance; }
        }

        public bool IsDistance
        {
            get { return !(_distance == default(double)); }
        }

        public bool IsNormal
        {
            get { return !IsDistance; }
        }
        

        public void SetDistance(double distance)
        {
            _distance = distance;
        }


        public override string ToString()
        {
            return Name;
        }

        public GeoCoordinate GeoCoordinate
        {
            get
            {
                return new GeoCoordinate(this.Lat, this.Long);
            }
        }
    }
}
