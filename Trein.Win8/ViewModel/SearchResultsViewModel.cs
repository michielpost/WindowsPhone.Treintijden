using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight;
using Windows.ApplicationModel.Resources;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;

namespace Trein.Win8.ViewModel
{
    public class SearchResultsViewModel : CustomViewModelBase
    {
        private readonly IStationNameService _stationService;
        private ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView("Resources");


        private ObservableCollection<Station> _suggestStations = new ObservableCollection<Station>();

        public ObservableCollection<Station> SuggestStations
        {
            get { return _suggestStations; }
            set
            {
                _suggestStations = value;
                RaisePropertyChanged(() => SuggestStations);
            }
        }

        public string PageTitle
        {
            get
            {
                if (string.IsNullOrEmpty(QueryText))
                {
                   return _resourceLoader.GetString("SearchResultsViewModelNoInput");
                }
                else
                {
                    return string.Format(_resourceLoader.GetString("SearchResultsViewModelSearchFor"), QueryText);
                }
            }
        }

        private string _queryText;

        public string QueryText
        {
            get { return _queryText; }
            set { _queryText = value;
            RaisePropertyChanged(() => QueryText);
            RaisePropertyChanged(() => PageTitle);
            }
        }


        public SearchResultsViewModel(IStationNameService stationService)
        {
            _stationService = stationService;

            if (ViewModelBase.IsInDesignModeStatic)
            {
                List<Station> stationDesignList = new List<Station>();
                stationDesignList.Add(new Station() { Name = "Amsterdam Centraal" });
                stationDesignList.Add(new Station() { Name = "Delft" });

                foreach (var l in stationDesignList)
                    SuggestStations.Add(l);
            }


        }

        public async void SearchStation(string p)
        {
            QueryText = p;

            if (string.IsNullOrEmpty(p))
                SuggestStations.Clear();
            else
            {
                p = p.ToLower();

                //var newStations = await TaskEx.Run<List<Station>>(() =>
                //    {

                var stations = (_stationService.GetStations()).Where(x => x.Name.ToLower().StartsWith(p)).Take(15);

                if (stations.Count() < 15)
                {
                    var extraStations = (_stationService.GetStations()).Where(x => x.StartsWith(p)).Take(15 - stations.Count());

                    stations = stations.Union(extraStations);
                }

                //return stations.ToList();


                // });

                SuggestStations.Clear();

                //newStations.ForEach(x => StationList.Add(x));

                foreach (var s in stations)
                    SuggestStations.Add(s);

                //StationList = stations;
            }
        }


    }
}
