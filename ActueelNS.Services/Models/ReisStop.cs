using System;
using System.Windows;

namespace ActueelNS.Services.Models
{
    public class ReisStop
    {
        public string Naam { get; set; }
        public DateTime Tijd { get; set; }

        public string DisplayTijd
        {
            get
            {
                return Tijd.ToString("HH:mm");
            }
        }

        public string VertragingTekst { get; set; }

        private string _vertrekSpoor;

        public string Vertrekspoor
        {
            get
            {
                if (_vertrekSpoor == null)
                    return _vertrekSpoor;

                if (_vertrekSpoor.Length <= 2)
                    return _vertrekSpoor;
                else
                    return null;
            }
            set { _vertrekSpoor = value; }
        }

        public string VertrekspoorDrie
        {
            get
            {
                if (_vertrekSpoor == null)
                    return _vertrekSpoor;

                if (_vertrekSpoor.Length == 3)
                    return _vertrekSpoor;
                else
                    return null;
            }
            set { _vertrekSpoor = value; }
        }


        public bool IsVertrekspoorWijziging { get; set; }

        public Visibility HeeftVertrekspoor
        {
            get
            {
                if (!string.IsNullOrEmpty(Vertrekspoor))
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public Visibility ShowVertrekspoorWijziging
        {
            get
            {
                if (IsVertrekspoorWijziging)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }

    }
}
