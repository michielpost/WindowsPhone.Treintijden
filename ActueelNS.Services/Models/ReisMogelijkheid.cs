using System;
using System.Windows.Media;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Text;
using System.Globalization;
using System.Linq;

namespace ActueelNS.Services.Models
{
    public class ReisMogelijkheid
    {
        public int AantalOverstappen { get; set; }
        public string GeplandeReisTijd { get; set; }
        public string Status { get; set; }

        public bool Optimaal { get; set; }

        public DateTime GeplandeVertrekTijd { get; set; }
        public DateTime ActueleVertrekTijd { get; set; }

        public DateTime GeplandeAankomstTijd { get; set; }
        public DateTime ActueleAankomstTijd { get; set; }

        public List<ReisDeel> ReisDelen { get; set; }

        //private SolidColorBrush _backgroundColor;

        //[JsonIgnore]
        //public SolidColorBrush BackgroundColor
        //{
        //    get
        //    {
        //        return _backgroundColor;
        //    }
        //}

        //public void SetBackground(SolidColorBrush bg)
        //{
        //    _backgroundColor = bg;
        //}

        public bool IsAlternate { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public SolidColorBrush BackgroundColor
        {
            get
            {
                if (IsAlternate)
                {
                    Color a = new Color() { R = 242, G = 242, B = 247, A = 255 };
                    SolidColorBrush alternateColor = new SolidColorBrush(a);

                    return alternateColor;
                }

                return new SolidColorBrush(Colors.White);
            }
        }



        public string VertrekDisplayTijd
        {
            get
            {
                return GeplandeVertrekTijd.ToString("HH:mm", CultureInfo.InvariantCulture);
            }
        }

        public string AankomstDisplayTijd
        {
            get
            {
                return GeplandeAankomstTijd.ToString("HH:mm", CultureInfo.InvariantCulture);
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

        public string VanNaar
        {
            get
            {
                return this.ReisDelen.First().FirstStop.Naam + " › " + this.ReisDelen.Last().LastStop.Naam;
            }
        }

        public string VanTot
        {
            get
            {
                return VertrekDisplayTijd + " › " + AankomstDisplayTijd;
            }
        }

        public string DisplayOverstappen
        {
            get
            {
                if (this.AantalOverstappen == 0)
                    return "Geen overstappen";
                else if (this.AantalOverstappen == 1)
                    return "1 overstap";
                else
                    return string.Format("{0} overstappen", this.AantalOverstappen);
            }
        }

        public string DisplayReisduur
        {
            get
            {
                try
                {
                    TimeSpan ts = TimeSpan.ParseExact(GeplandeReisTijd, @"h\:mm", CultureInfo.InvariantCulture);

                    if (ts.Hours == 0)
                    {
                        return string.Format("{0} minuten", ts.Minutes);
                    }
                    else if (ts.Minutes == 0)
                    {
                        if (ts.Hours == 1)
                            return string.Format("{0} uur", ts.Hours);
                        else
                            return string.Format("{0} uur", ts.Hours);


                    }
                    else
                    {
                        if (ts.Hours == 1 && ts.Minutes == 1)
                            return string.Format("{0} uur {1} minuut", ts.Hours, ts.Minutes);
                        else if (ts.Hours == 1)
                            return string.Format("{0} uur {1} minuten", ts.Hours, ts.Minutes);
                        else if (ts.Minutes == 1)
                            return string.Format("{0} uur {1} minuut", ts.Hours, ts.Minutes);
                        else
                            return string.Format("{0} uur {1} minuten", ts.Hours, ts.Minutes);

                    }

                }
                catch
                {
                }

                return "De reisduur is " + GeplandeReisTijd;
            }
        }


        public string GetAsText()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(VanNaar);
            sb.AppendLine(VanTot);
            sb.AppendLine(DisplayReisduur);
            sb.AppendLine(DisplayOverstappen);
            
            sb.Append(Environment.NewLine);

            foreach (var deel in this.ReisDelen)
            {
                sb.AppendLine(deel.FirstStop.DisplayTijd + " |" + deel.FirstStop.Vertrekspoor + "| \t" + deel.FirstStop.Naam);
                sb.AppendLine(deel.LastStop.DisplayTijd + " |" + deel.LastStop.Vertrekspoor + "| \t" + deel.LastStop.Naam);
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public string GetAsHtml()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(VanNaar);
            sb.Append("<br />");

            sb.AppendLine(VanTot);
            sb.Append("<br />");

            sb.AppendLine(DisplayReisduur);
            sb.Append("<br />");

            sb.AppendLine(DisplayOverstappen);
            sb.Append("<br />");

            sb.Append("<table>");

            foreach (var deel in this.ReisDelen)
            {
                sb.Append("<tr>");
                sb.AppendFormat("<td>{0}</td>", deel.FirstStop.DisplayTijd);
                sb.AppendFormat("<td>{0}</td>", deel.FirstStop.Naam);
                sb.AppendFormat("<td>{0}</td>", deel.FirstStop.Vertrekspoor);
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.AppendFormat("<td>{0}</td>", deel.LastStop.DisplayTijd);
                sb.AppendFormat("<td>{0}</td>", deel.LastStop.Naam);
                sb.AppendFormat("<td>{0}</td>", deel.LastStop.Vertrekspoor);
                sb.Append("</tr>");

                sb.Append("<tr><td></td><td></td><td></td></tr>");

            }
            sb.Append("</table>");


            return sb.ToString();
        }

        public string GetAsHtmlForPrint()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<p>");
            sb.AppendFormat("<b>{0}</b>", VanNaar);
            sb.Append("</p>");

            sb.AppendFormat("<p>{0}</p>", VanTot);

            sb.AppendFormat("<p>{0}</p>", DisplayReisduur);
            sb.AppendFormat("<p>{0}</p>", DisplayOverstappen);

            sb.Append("<p></p>");
            sb.Append("<p></p>");


            foreach (var deel in this.ReisDelen)
            {
                if (string.IsNullOrEmpty(deel.FirstStop.VertragingTekst))
                    sb.AppendFormat("<p><b>{0}: {1} spoor {2}</b></p>", deel.FirstStop.DisplayTijd, deel.FirstStop.Naam, deel.FirstStop.Vertrekspoor);
                else
                    sb.AppendFormat("<p><b>{0} ({3}): {1} spoor {2}</b></p>", deel.FirstStop.DisplayTijd, deel.FirstStop.Naam, deel.FirstStop.Vertrekspoor, deel.FirstStop.VertragingTekst);


                if (string.IsNullOrEmpty(deel.LastStop.VertragingTekst))
                    sb.AppendFormat("<p>{0}: {1} spoor {2}</p>", deel.LastStop.DisplayTijd, deel.LastStop.Naam, deel.LastStop.Vertrekspoor);
                else
                    sb.AppendFormat("<p>{0} ({3}): {1} spoor {2}</p>", deel.LastStop.DisplayTijd, deel.LastStop.Naam, deel.LastStop.Vertrekspoor, deel.LastStop.VertragingTekst);

                sb.Append("<p></p>");


            }
            sb.Append("<p>----------</p>");
            sb.Append("<p></p>");


            return sb.ToString();
        }

       
    }
}
