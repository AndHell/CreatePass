using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CreatePass.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddSiteKeyView : Page
    {
        string encryptionPin = "";
        private ResourceLoader rsLoader;

        public AddSiteKeyView()
        {
            this.InitializeComponent();
            rsLoader = new ResourceLoader();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            encryptionPin = (string)e.Parameter;

            var frame = Window.Current.Content as Frame;
            
            if (frame?.CanGoBack == true)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            base.OnNavigatedTo(e);
        }

        private async void btn_addSiteKey_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_siteKey.Text))
                return;

            try
            {
                var settings = new Settings();
                var cryptor = new AES256Crypto();
                var sitekey = new Model.SiteKeyItem() { DateAdded = DateTime.Now, Url_Encrypted = cryptor.Encrypt(txt_siteKey.Text, encryptionPin, settings.SitekeySalt) };

                var dbService = new Service.DbService();
                dbService.AddSiteKey(sitekey);

                GoBack();
            }
            catch (Exception ex)
            {
                var diaLog = new MessageDialog(rsLoader.GetString("dia_AddSitekeyFailed"));
                await diaLog.ShowAsync();
                Debug.WriteLine($"SaveNewSiteKey: {ex.Message}");
            }
        }

        private void GoBack()
        {
            var frame = Window.Current.Content as Frame;
            if (frame.CanGoBack)
                frame.GoBack();
        }
    }
}
