using ActueelNS.Resources;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Q42.WinRT.Portable.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.PCL.Api.Models;
using Treintijden.Shared.Services.Interfaces;

namespace ActueelNS.ViewModel
{
	public class RitInfoViewModel : CustomViewModelBase
	{
    public INavigationService NavigationService { get; set; }
    public IStationService StationService { get; set; }
		public IStationNameService StationNameService { get; set; }
		public INSApiService RitnummerService { get; set; }
		public event EventHandler<EventArgs> RitInfoAvailable;

    public RelayCommand PlanCommand { get; private set; }

    private string _stationCode;



    private string _pageName;

		public string PageName
		{
			get { return _pageName.ToLower(); }
			set
			{
				_pageName = value;
				RaisePropertyChanged(() => PageName);
			}
		}

		private string _richting;

		public string Richting
		{
			get { return _richting; }
			set
			{
				_richting = value;
				RaisePropertyChanged(() => Richting);
			}
		}


		private List<RitInfoStop> _ritStops;

		public List<RitInfoStop> RitStops
		{
			get { return _ritStops; }
			set
			{
				_ritStops = value;
				RaisePropertyChanged(() => RitStops);
			}
		}

		private TreinInfo _treinInfo;
    private DateTime? _vertrekTijd;

    public TreinInfo TreinInfo
		{
			get { return _treinInfo; }
			set
			{
				_treinInfo = value;
				RaisePropertyChanged(nameof(TreinInfo));
			}
		}

		public DataLoader DataLoader { get; set; }

		public RitInfoViewModel()
		{
			StationService = SimpleIoc.Default.GetInstance<IStationService>();
			StationNameService = SimpleIoc.Default.GetInstance<IStationNameService>();
			RitnummerService = SimpleIoc.Default.GetInstance<INSApiService>();
      NavigationService = SimpleIoc.Default.GetInstance<INavigationService>();

      DataLoader = new DataLoader();

			if (IsInDesignMode)
			{
				var list = new List<RitInfoStop>();
				list.Add(new RitInfoStop() { Station = "Delft", Arrival = DateTime.Now });
				list.Add(new RitInfoStop() { Station = "Delft", Arrival = DateTime.Now });
				list.Add(new RitInfoStop() { Station = "Delft", Arrival = DateTime.Now });
				list.Add(new RitInfoStop() { Station = "Delft", Arrival = DateTime.Now });

				RitStops = list;
			}

      PlanCommand = new RelayCommand(() => PlanStation());


    }

    public async void Initialize(string ritId, string company, string trein, string richting, string stationCode)
		{
			PageName = trein;

      if(!string.IsNullOrEmpty(richting))
			  Richting = AppResources.RitInfoViewModelRichting + " " + richting;

      RitStops = new List<RitInfoStop>();
			TreinInfo = null;
      _stationCode = stationCode;

      var ritStopsLoader = DataLoader.LoadAsync(async () =>
					{

						var serviceInfo = await RitnummerService.GetRit(ritId, company, DateTime.Now);

						List<RitInfoStop> stops = null;

						if (serviceInfo.FirstOrDefault() != null)
						{
							stops = serviceInfo.First().Stops;

										//Fill station name for each stop
										foreach (var stop in stops)
							{
								var station = StationNameService.GetStationByCode(stop.Code);
								if (station != null)
									stop.Station = station.Name;

                if (stop.Code.ToLower() == stationCode.ToLower())
                {
                  stop.IsCurrent = true;
                  _vertrekTijd = stop.Departure;
                }
							}
						}

						return stops;
					});

			//Start loading treinfo (but dont await yet)
			var treinInfoLoader = RitnummerService.GetTreinInfo(ritId);

			RitStops = await ritStopsLoader;

			if (RitStops == null || !RitStops.Any())
			{
				RitStops = null;
				DataLoader.LoadingState = LoadingState.Error;
			}

			if (RitStops != null && RitInfoAvailable != null)
				RitInfoAvailable(null, null);

			try
			{
				//Optional Treinfo
				TreinInfo = await treinInfoLoader;
			}
			catch { }

		}

    private void PlanStation()
    {
      string name = _stationCode;
      string tijd = string.Empty;
      if (_vertrekTijd.HasValue)
        tijd = _vertrekTijd.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "T" + _vertrekTijd.Value.ToString("HH:mm", CultureInfo.InvariantCulture);

      NavigationService.NavigateTo(new Uri(string.Format("/Views/Planner.xaml?from={0}&dateTime={1}", name, _vertrekTijd), UriKind.Relative));
    }
  }
}
