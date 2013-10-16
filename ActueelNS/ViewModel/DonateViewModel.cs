using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
using Q42.WinRT.Portable.Data;
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

        public DataLoader DataLoader { get; set; }

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
            DataLoader = new DataLoader();

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
                try
                {
                    Store.ListingInformation li = await Store.CurrentApp.LoadListingInformationAsync();
                    string pID = li.ProductListings[key].ProductId;

                    string receipt = await Store.CurrentApp.RequestProductPurchaseAsync(pID, false);
                }
                catch (Exception e)
                {
                }
                finally
                {
                    Init();
                }

            }
        }


        private async void Init()
        {
            AvailableProducts = await DataLoader.LoadAsync(() => GetAvailableProducts());
        }

        private async Task<List<StoreItem>> GetAvailableProducts()
        {
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

            return list.OrderBy(x => x.ProductListing.FormattedPrice).ToList();
        }
    }
}