﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
#else
using Store = Windows.ApplicationModel.Store;
#endif

namespace ActueelNS.ViewModel
{

    public class StoreItem
    {
        public Store.ProductListing ProductListing { get; set; }
        public bool Purchased { get; set; }
    }

    public class DonateViewModel : CustomViewModelBase
    {
        private bool _isError;

        public bool IsError
        {
            get { return _isError; }
            set { _isError = value;
            RaisePropertyChanged(() => IsError);
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value;
            RaisePropertyChanged(() => IsBusy);
            }
        }
        
        

        private List<StoreItem> _availableProducts;

        public List<StoreItem> AvailableProducts
        {
            get { return _availableProducts; }
            set { _availableProducts = value;
            RaisePropertyChanged(() => AvailableProducts);
            }
        }

        public RelayCommand<string> TapCommand { get; private set; }


        /// <summary>
        /// Initializes a new instance of the AboutViewModel class.
        /// </summary>
        public DonateViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real": Connect to service, etc...
            ////}

            TapCommand = new RelayCommand<string>(DoTapAction);

            Init();
        }

        private async void DoTapAction(string key)
        {
            if (!Store.CurrentApp.LicenseInformation.ProductLicenses[key].IsActive)
            {
                Store.ListingInformation li = await Store.CurrentApp.LoadListingInformationAsync();
                string pID = li.ProductListings[key].ProductId;

                string receipt = await Store.CurrentApp.RequestProductPurchaseAsync(pID, false);

                Init();
            }
        }


        private async void Init()
        {
            try
            {
                IsError = false;
                IsBusy = true;

                //var prodLit = await CurrentApp.LicenseInformation.ProductLicenses.First().;

                var prodLit = await Store.CurrentApp.LoadListingInformationAsync();
                var own = Store.CurrentApp.LicenseInformation.ProductLicenses;

                List<StoreItem> list = new List<StoreItem>();

                foreach (var item in prodLit.ProductListings.Values)
                {
                    StoreItem pr = new StoreItem();
                    pr.ProductListing = item;

                    if (Store.CurrentApp.LicenseInformation.ProductLicenses.Keys.Contains(item.ProductId)
                        && Store.CurrentApp.LicenseInformation.ProductLicenses[item.ProductId].IsActive)
                    {
                        pr.Purchased = true;
                    }

                    list.Add(pr);
                }

                AvailableProducts = list.OrderBy(x => x.ProductListing.FormattedPrice).ToList();

            }
            catch
            {
                IsError = true;
            }
            finally
            {
                IsBusy = false;

            }

        }
    }
}