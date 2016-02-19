using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PassGen
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MoreInfoView : Page
    {
        private LicenseInformation licenseInformation;


        public MoreInfoView()
        {
            this.InitializeComponent();

            licenseInformation = CurrentAppSimulator.LicenseInformation;
            //licenseInformation = CurrentApp.LicenseInformation;

            SetVersion();
        }


        private void SetVersion()
        {
            var version = Package.Current.Id.Version;
            var versionStr = string.Format("{0}.{1}.{2}-{3}",version.Major,version.Minor, version.Build, version.Revision);
            more_txt_Version.Text = string.Format("Version: {0}", versionStr);
        }
        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var frame = Window.Current.Content as Frame;

            if (frame == null)
                return;

            if (frame.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            
            base.OnNavigatedTo(e);
        }

        private async void btn_CoffeeDonationPurchase_Click(object sender, RoutedEventArgs e)
        {
            //not ready yet

            throw new NotImplementedException();

        }

        private void btn_CoffeeDonationBitCoin_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://andhell.github.io");
            var respons = Windows.System.Launcher.LaunchUriAsync(uri);

        }
    }
}
