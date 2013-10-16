using Microsoft.Phone.Controls;
using ActueelNS.ViewModel;
using Microsoft.Phone.Shell;
using System;
using ActueelNS.Views.Base;

namespace ActueelNS.Views
{
    /// <summary>
    /// Description for Prijs.
    /// </summary>
    public partial class Prijs : ViewBase
    {
        private PrijsViewModel _vm;

        /// <summary>
        /// Initializes a new instance of the Prijs class.
        /// </summary>
        public Prijs()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _vm = (PrijsViewModel)DataContext;


            if (this.NavigationContext.QueryString.ContainsKey("id"))
            {
                string guidIdString = this.NavigationContext.QueryString["id"];

                Guid id = Guid.Parse(guidIdString);

                _vm.Initialize(id);

            }
            else
            {
                _vm.Initialize(null);

            }

            base.OnNavigatedTo(e);
        }


        private void CloseGeenDataPanel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                GeenDataPanel.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}