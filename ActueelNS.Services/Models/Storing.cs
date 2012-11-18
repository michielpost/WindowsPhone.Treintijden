using System;

namespace ActueelNS.Services.Models
{
    public class Storing
    {
        public string Id { get; set; }

        public string Traject { get; set; }

        public string Reden { get; set; }

        public string Bericht { get; set; }

        public DateTime Datum { get; set; }

        public string DisplayTijd
        {
            get
            {
                return Datum.ToString("HH:mm");
            }
        }

    }
}
