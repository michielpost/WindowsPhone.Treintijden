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
using System.Threading.Tasks;
using ActueelNS.Services.Models;
using System.Collections.Generic;

namespace ActueelNS.Services.Mocks
{
    public class VertrektijdenServiceMock : IVertrektijdenService
    {

        public Task<List<Vertrektijd>> GetVertrektijden(string station)
        {
            return TaskEx.Run<List<Vertrektijd>>(() => {
                 var list =  new List<Vertrektijd>();

                 list.Add(new Vertrektijd()
                 {
                     Tijd = DateTime.Now,
                     Eindbestemming = "Amsterdam",
                     IsVertrekspoorWijziging = false,
                     Ritnummer = 123,
                     Route = "Delft, Den Haag",
                     TreinSoort = "Sprinter",
                     Vertrekspoor = "2",
                     Opmerkingen = "Rijdt vandaag niet."
                 });

                 list.Add(new Vertrektijd()
                 {
                     Tijd = DateTime.Now,
                     Eindbestemming = "Delft",
                     IsVertrekspoorWijziging = false,
                     Ritnummer = 123,
                     Route = "Delft, Den Haag",
                     TreinSoort = "Intercity",
                     Vertrekspoor = "2b"
                 });


                 list.Add(new Vertrektijd()
                 {
                     Tijd = DateTime.Now,
                     Eindbestemming = "Groningen",
                     IsVertrekspoorWijziging = true,
                     Ritnummer = 123,
                     Route = "Amsterdam, Den Haag",
                     TreinSoort = "Intercity",
                     Vertrekspoor = "2b",
                      VertragingTekst = "+5 min",
                     Opmerkingen = "Rijdt vandaag niet."

                      
                 });

                 return list;
            });
        }
    }
}
