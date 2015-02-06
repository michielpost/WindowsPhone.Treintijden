using ActueelNS.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Store;

namespace Trein.Win8.ViewModel
{
    public class StoreItem : CustomViewModelBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }

        private bool _purchased;

        public bool Purchased
        {
            get { return _purchased; }
            set
            {
                _purchased = value;
                RaisePropertyChanged(() => Purchased);
            }
        }

      

    }

    public class DonateViewModel : CustomViewModelBase
    {
        private ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        private bool _showUpgrade;

        public bool ShowUpgrade
        {
            get { return _showUpgrade; }
            set
            {
                _showUpgrade = value;
                RaisePropertyChanged(() => ShowUpgrade);
            }
        }

        private string _userType;

        public string UserType
        {
            get { return _userType; }
            set { _userType = value;
            RaisePropertyChanged(() => UserType);
            }
        }


        public List<StoreItem> AvailableProducts { get; set; }

        public LicenseInformation LicenseInformation { get; set; }

        public RelayCommand<StoreItem> TapCommand { get; private set; }


        public DonateViewModel()
        {
            AvailableProducts = new List<StoreItem>();

            AvailableProducts.Add(new StoreItem() { Code = "upgrade1", Image = "/Assets/Purchase/upgrade1.png", Name = _resourceLoader.GetString("Reiziger"), Price = "€ 1,19" });
            AvailableProducts.Add(new StoreItem() { Code = "upgrade2", Image = "/Assets/Purchase/upgrade2.png", Name = _resourceLoader.GetString("Conducteur"), Price = "€ 2,49" });
            AvailableProducts.Add(new StoreItem() { Code = "upgrade3", Image = "/Assets/Purchase/upgrade3.png", Name = _resourceLoader.GetString("Machinist"), Price = "€ 4,99" });

#if DEBUG
            LicenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            LicenseInformation = CurrentApp.LicenseInformation;
#endif


            CheckBuy();

            CheckType();

            TapCommand = new RelayCommand<StoreItem>(DoTapAction);
        }

        private void CheckType()
        {
            var purchased = AvailableProducts.Where(x => x.Purchased).LastOrDefault();

            if (purchased != null)
            {
                ShowUpgrade = false;
                UserType = purchased.Name;
            }
            else
            {
                ShowUpgrade = true;
                UserType = _resourceLoader.GetString("gratis");
            }
        }

        private void CheckBuy()
        {
            foreach (var item in AvailableProducts)
            {
                item.Purchased = LicenseInformation.ProductLicenses[item.Code].IsActive;
            }
        }

        private async void DoTapAction(StoreItem key)
        {
            if (!LicenseInformation.ProductLicenses[key.Code].IsActive)
            {
                try
                {

#if DEBUG
                    var result = await CurrentAppSimulator.RequestProductPurchaseAsync(key.Code);
#else
                    var result = await CurrentApp.RequestProductPurchaseAsync(key.Code);
#endif

                    CheckBuy();

                    CheckType();

                }
                catch (Exception e)
                {
                }
            }
            else
            {
                var item = AvailableProducts.Where(x => x.Code == key.Code).FirstOrDefault();
                if (item != null)
                    item.Purchased = true;

            }

        }
    }
}
