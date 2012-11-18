using System;
using System.Windows.Media;
using System.Collections.Generic;

namespace ActueelNS.Services.Models
{
    public class ReisMogelijkheid
    {
        public int AantalOverstappen { get; set; }
        public string GeplandeReisTijd { get; set; }

        public bool Optimaal { get; set; }

        public DateTime GeplandeVertrekTijd { get; set; }
        public DateTime ActueleVertrekTijd { get; set; }

        public DateTime GeplandeAankomstTijd { get; set; }
        public DateTime ActueleAankomstTijd { get; set; }

        public List<ReisDeel> ReisDelen { get; set; }

        private SolidColorBrush _backgroundColor;
        public SolidColorBrush BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
        }

        public void SetBackground(SolidColorBrush bg)
        {
            _backgroundColor = bg;
        }


        public string VertrekDisplayTijd
        {
            get
            {
                return GeplandeVertrekTijd.ToString("HH:mm");
            }
        }

        public string AankomstDisplayTijd
        {
            get
            {
                return GeplandeAankomstTijd.ToString("HH:mm");
            }
        }

        public string VertrekVertraging
        {
            get
            {
                return GetVertragingTekst(GeplandeVertrekTijd, ActueleVertrekTijd);
            }
        }

        public string AankomstVertraging
        {
            get
            {
                return GetVertragingTekst(GeplandeAankomstTijd, ActueleAankomstTijd);
            }
        }

        private static string GetVertragingTekst(DateTime gepland, DateTime actueel)
        {
            string result = string.Empty;

            if (gepland != null && actueel != null)
            {
                var verschil = actueel - gepland;

                if (verschil > new TimeSpan(0))
                {
                   if (verschil.Hours > 0 || verschil.Minutes > 0)
                    {
                        result = string.Format(" (+{0})", ((verschil.Hours * 60) + verschil.Minutes));
                    }
                }
            }

            return result;
        }

       
    }
}
