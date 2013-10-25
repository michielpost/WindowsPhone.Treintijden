using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight;
using Windows.ApplicationModel.Resources;
using Treintijden.PCL.Api.Models;
using Treintijden.PCL.Api.Interfaces;
using Q42.WinRT.Portable.Data;

namespace Trein.Win8.ViewModel
{
    public class StoringenViewModel : CustomViewModelBase
    {
        private readonly INSApiService _storingenService;
        public DataLoader DataLoader { get; set; }

        private ResourceLoader _resourceLoader = new ResourceLoader("Resources");

        private List<Storing> _currentStoringen;

        public List<Storing> CurrentStoringen
        {
            get { return _currentStoringen; }
            set
            {
                _currentStoringen = value;
                RaisePropertyChanged(() => CurrentStoringen);
                RaisePropertyChanged(() => Title);
            }
        }

        private List<Werkzaamheden> _werkzaamheden;

        public List<Werkzaamheden> Werkzaamheden
        {
            get { return _werkzaamheden; }
            set
            {
                _werkzaamheden = value;
                RaisePropertyChanged(() => Werkzaamheden);
            }
        }

        public string Title
        {
            get
            {
                if (CurrentStoringen == null || CurrentStoringen.Count == 0)
                {
                    return _resourceLoader.GetString("Storingen");
                }
                else
                    return string.Format(_resourceLoader.GetString("StoringenCount"), CurrentStoringen.Count);
            }
        }


        public StoringenViewModel(INSApiService storingenService)
        {
            _storingenService = storingenService;
            DataLoader = new DataLoader();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Code runs in Blend --> create design time data.

                var list = new List<Storing>();

                list.Add(new Storing()
                {
                    Bericht = "Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. ",
                    Datum = DateTime.Now,
                    Reden = "seinstoring",
                    Traject = "Delft - Amsterdam vv",

                });

                list.Add(new Storing()
                {
                    Bericht = "Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. ",
                    Datum = DateTime.Now,
                    Reden = "seinstoring",
                    Traject = "Delft - Amsterdam vv"
                });

                CurrentStoringen = list;



                var werkList = new List<Werkzaamheden>();

                werkList.Add(new Werkzaamheden()
                {
                    Advies = "Maak gebruik van de bus.... ",
                    Periode = DateTime.Now.ToString(),
                    Reden = "seinstoring",
                    Traject = "Delft - Amsterdam vv",
                    Vertraging = "ga eerder op reis"
                });

                werkList.Add(new Werkzaamheden()
                {
                    Advies = "Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. Dit is een hele lange storing. ",
                    Periode = DateTime.Now.ToString(),
                    Reden = "seinstoring",
                    Traject = "Delft - Amsterdam vv",
                    Vertraging = "ga eerder op reis"
                });

                Werkzaamheden = werkList;

            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                LoadData();
            }
        }

        private async Task LoadData()
        {
          var StoringenEnWerkzaamheden = DataLoader.LoadAsync(() => _storingenService.GetStoringenEnWerkzaamheden(""));

          //TODO
          //CurrentStoringen = await 
          //Werkzaamheden = _storingenService.GetWerkzaamheden();
        }
    }
}
