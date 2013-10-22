using System;
using System.Globalization;

namespace Treintijden.PCL.Api.Models
{
    public class Vertrektijd
    {
        public int Ritnummer { get; set; }

        public DateTime Tijd { get; set; }

        public string DisplayTijd
        {
            get
            {
                return Tijd.ToString("HH:mm", CultureInfo.InvariantCulture);
            }
        }

        public string Vertraging { get; set; }
        public string VertragingTekst { get; set; }

        public string Eindbestemming { get; set; }

        public string TreinSoort { get; set; }

        public string Route { get; set; }
        public string ReisTip { get; set; }

        public string Vertrekspoor { get; set; }

        public string VertrekspoorTwee
        {
            get {
                if (Vertrekspoor == null)
                    return Vertrekspoor;

                if (Vertrekspoor.Length <= 2)
                    return Vertrekspoor;
                else
                    return null;
            }
        }

        public string VertrekspoorDrie
        {
            get
            {
                if (Vertrekspoor == null)
                    return Vertrekspoor;

                if (Vertrekspoor.Length == 3)
                    return Vertrekspoor;
                else
                    return null;
            }
        }
        
        public bool IsVertrekspoorWijziging { get; set; }

        public string Opmerkingen { get; set; }


        public bool IsAlternate { get; set; }


        public string RegelTwee
        {
            get 
            {
              if (Route != null && Route.Length > 0)
              {
                if (string.IsNullOrEmpty(Vervoerder))
                  return string.Format("{0} via {1}", TreinSoort, Route);
                else
                  return string.Format("{0} ({2}) via {1}", TreinSoort, Route, Vervoerder);

              }
              else
              {

                if (string.IsNullOrEmpty(Vervoerder))
                  return TreinSoort;
                else
                  return string.Format("{0} ({1})", TreinSoort, Vervoerder);
              }
            }
           
        }


        public bool IsWarningVisible
        {
            get { return (VertragingTekst != null && VertragingTekst.Length > 0); }
        }


        public bool IsOpmerkingVisible
        {
            get { return (Opmerkingen != null && Opmerkingen.Length > 0); }
        }

        public bool IsReisTipVisible
        {
            get { return (ReisTip != null && ReisTip.Length > 0); }
        }


        public string Vervoerder { get; set; }
    }
}
