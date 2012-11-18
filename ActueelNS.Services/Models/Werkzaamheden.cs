using System.ComponentModel;

namespace ActueelNS.Services.Models
{
    public class Werkzaamheden : INotifyPropertyChanged
    {
        public string Id { get; set; }

        public string Traject { get; set; }
        public string Periode { get; set; }

        public string Reden { get; set; }
        public string Vertraging { get; set; }

        public string Advies { get; set; }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                NotifyPropertyChanged("IsExpanded");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        // NotifyPropertyChanged will raise the PropertyChanged event passing the
        // source property that is being updated.
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }  

    }
}
