using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.DataTransfer;
using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using ZXing;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.Storage.Streams;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CreatePass
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Settings settings;
        PasswordGeneration passGenerator;
        ResourceLoader rsLoader;

        public object BitmapCacheOption { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            
            settings = new Settings();

            tgs_AutoCopy.IsOn = settings.AutoCopy;

            sld_Length.Value = settings.PwLength;
            txt_PwLength.Text = settings.PwLength.ToString();
            cb_Alphanum.IsChecked = settings.UseAlphaNumChars;
            cb_Numbers.IsChecked = settings.UseNumChars;
            cb_Special.IsChecked = settings.UseSpecialChars;
            txt_Salt.Text = settings.Salt;
            
            passGenerator = new PasswordGeneration(settings.PwLength,settings.Salt, settings.UseNumChars, settings.UseAlphaNumChars, settings.UseSpecialChars);
            rsLoader = new ResourceLoader();
            lv_siteKeys.ItemsSource = sitekeyList;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (IsSiteKeyListUnlocked)
            {
                RefreshSiteKeyList();
            }
            base.OnNavigatedTo(e);
        }

        #region PW . . .

        private async void btn_Generate_Click(object sender, RoutedEventArgs e)
        {
            if (txt_MasterKey.Password.Length < 8)
            {
                var test = new Windows.UI.Popups.MessageDialog(rsLoader.GetString("dia_MasterKeyToShort"));
                await test.ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(txt_SiteKey.Text))
                return;

            txt_Password.Text = passGenerator.Generate(txt_MasterKey.Password, txt_SiteKey.Text);
            btn_Copy.Visibility = Visibility.Visible;
            btn_QRCode.Visibility = Visibility.Visible;

            if (settings.AutoCopy)
                WriteToClipboard(txt_Password.Text);
        }

        private void btn_Copy_Click(object sender, RoutedEventArgs e)
        {
            WriteToClipboard(txt_Password.Text);
        }

        private void btn_QRCode_Click(object sender, RoutedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(QrCodeView), txt_Password.Text);
        }
        
        private void appbar_ResetPW_Click(object sender, RoutedEventArgs e)
        {
            txt_MasterKey.Password = string.Empty;
            txt_Password.Text = string.Empty;
            txt_SiteKey.Text = string.Empty;

            btn_Copy.Visibility = Visibility.Collapsed;
            btn_QRCode.Visibility = Visibility.Collapsed;

            WriteToClipboard(string.Empty);

            LockSiteKeyList();
        }

        private void WriteToClipboard(string text)
        {
            var datapack = new DataPackage();
            datapack.RequestedOperation = DataPackageOperation.Copy;
            datapack.SetText(text);
            Clipboard.SetContent(datapack);
        }



        #endregion

        #region Settings . . .

        private void tgs_AutoCopy_Toggled(object sender, RoutedEventArgs e)
        {
            settings.UpdateAutoCopy(tgs_AutoCopy.IsOn);
        }

        private void cb_Chars_Checked(object sender, RoutedEventArgs e)
        {
            if (passGenerator == null)
                return;

            settings.UpdatePwChars(cb_Numbers.IsChecked.Value, cb_Alphanum.IsChecked.Value, cb_Special.IsChecked.Value);
            passGenerator.UpdatePwChars(settings.UseNumChars, settings.UseAlphaNumChars, settings.UseSpecialChars);
        }

        private void btn_ToggelSalt_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Salt.Visibility == Visibility.Collapsed)
            {
                txt_Salt.Visibility = Visibility.Visible;
                btn_SaveSalt.Visibility = Visibility.Visible;
                btn_ToggelSalt.Content = rsLoader.GetString("main_txt_ShowSalt_hide");
                btn_SaltToQR.Visibility = Visibility.Visible;
            }
            else
            {
                txt_Salt.Visibility = Visibility.Collapsed;
                btn_SaveSalt.Visibility = Visibility.Collapsed;
                btn_ToggelSalt.Content = rsLoader.GetString("main_txt_ShowSalt_show");
                btn_SaltToQR.Visibility = Visibility.Collapsed;
            }
        }

        private async void btn_SaveSalt_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Salt.Text.Length < 8)
            {
                var mBoxLength = new Windows.UI.Popups.MessageDialog(rsLoader.GetString("dia_SaltToShort"));
                await mBoxLength.ShowAsync();
                return;
            }

            var messageBoxChange = new ContentDialog() {
                            Title = rsLoader.GetString("dia_ChangeSalt_Title"),
                            Content = rsLoader.GetString("dia_ChangeSalt_Content"),
                            PrimaryButtonText = rsLoader.GetString("dia_ChangeSalt_Primary"),
                            SecondaryButtonText = rsLoader.GetString("dia_ChangeSalt_Secondary") };
            var result = await messageBoxChange.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                settings.UpdateSalt(txt_Salt.Text);
                passGenerator.UpdateSalt(txt_Salt.Text);
            }
            else
            {
                txt_Salt.Text = settings.Salt;
            }
        }

        private void sld_Length_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (passGenerator == null)
            {
                return;
            }

            var newLength = (int)sld_Length.Value;

            settings.UpdatePwLen(newLength);
            passGenerator.UpdatePwLen(settings.PwLength);

            txt_PwLength.Text = settings.PwLength.ToString();
        }

        private void btn_SaltToQR_Click(object sender, RoutedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(QrCodeView), settings.Salt);
        }
        #endregion

        #region SiteKey List . . .

        private ObservableCollection<Model.SiteKeyItem> sitekeyList = new ObservableCollection<Model.SiteKeyItem>();

        private bool IsSiteKeyListUnlocked = false;

        private async void btn_siteListUnlock_Click(object sender, RoutedEventArgs e)
        {
            var isPinValid = await CheckAndHandleSiteKeyPin();
            if (!isPinValid) return;

            //Decrypt List
            if (RefreshSiteKeyList())
            {
                //one or more items could not be decrypted
                var messageBoxChange = new ContentDialog() {
                        Title = rsLoader.GetString("dia_SiteKeyDecryptionFailed_Title"),
                        Content = rsLoader.GetString("dia_SiteKeyDecryptionFailed_Content"),
                        PrimaryButtonText = "OK" };
                await messageBoxChange.ShowAsync();
            }

            appbar_AddSite.Visibility = Visibility.Visible;
            appbar_LockSitekeys.Visibility = Visibility.Visible;
            lv_siteKeys.Visibility = Visibility.Visible;
            IsSiteKeyListUnlocked = true;

        }

        private async Task<bool> CheckAndHandleSiteKeyPin()
        {
            if (string.IsNullOrEmpty(txt_siteKeyPin.Password))
                return false;

            if (txt_siteKeyPin.Password.Length < 4)
            {
                var dialog = new MessageDialog(rsLoader.GetString("dia_SiteKeyPinToShort"));
                await dialog.ShowAsync();
                return false;
            }
            return true;
        }

        private void appbar_AddSite_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_siteKeyPin.Password))
                return;

            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(View.AddSiteKeyView), txt_siteKeyPin.Password);
        }
        private void appbar_LockSitekeys_Click(object sender, RoutedEventArgs e)
        {
            LockSiteKeyList();
        }

        private void LockSiteKeyList()
        {
            IsSiteKeyListUnlocked = false;
            txt_siteKeyPin.Password = "";
            sitekeyList.Clear();
            appbar_AddSite.Visibility = Visibility.Collapsed;
            appbar_LockSitekeys.Visibility = Visibility.Collapsed;
        }

        private bool RefreshSiteKeyList()
        {
            sitekeyList.Clear();

            var hasErrors = false;
            var cryptor = new AES256Crypto();
            var dbService = new Service.DbService();
            foreach (var sitekey in dbService.GetSitekeys())
            {
                try
                {
                    sitekey.Url_PlainText = cryptor.Decrypt(sitekey.Url_Encrypted, txt_siteKeyPin.Password, settings.SitekeySalt);
                    sitekeyList.Add(sitekey);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"DecryptSiteKey: {ex.Message}");
                    hasErrors = true;
                }
            }
            return hasErrors;
        }



        private Model.SiteKeyItem sitekeyRightClicked;
        private void mf_deleteSiteKey_Click(object sender, RoutedEventArgs e)
        {
            if (sitekeyRightClicked != null)
            {
                try
                {
                    var dbService = new Service.DbService();
                    dbService.DeleteSiteKey(sitekeyRightClicked);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"DeleteSiteKey: {ex.Message}");
                }
                RefreshSiteKeyList();
            }
            //Reset
            sitekeyRightClicked = null;
        }

        private void StackPanel_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            var stackpanelitem = (StackPanel)sender;
            SiteKeyItemFlyout.ShowAt(stackpanelitem, e.GetPosition(stackpanelitem));
            sitekeyRightClicked = (Model.SiteKeyItem)((FrameworkElement)sender).DataContext;
        }


        private void StackPanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var sitekeyClicked = (Model.SiteKeyItem)((FrameworkElement)sender).DataContext;
            txt_SiteKey.Text = sitekeyClicked.Url_PlainText;
            PivotMain.SelectedItem = pivItem_PW;
        }

        #endregion

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((PivotItem)PivotMain.SelectedItem == pivItem_Sites)
            {
                if (IsSiteKeyListUnlocked)
                {
                    appbar_AddSite.Visibility = Visibility.Visible;
                    appbar_LockSitekeys.Visibility = Visibility.Visible;
                }
                appbar_ResetPW.Visibility = Visibility.Collapsed;
            }
            else if ((PivotItem)PivotMain.SelectedItem == pivItem_PW)
            {
                appbar_ResetPW.Visibility = Visibility.Visible;
                appbar_AddSite.Visibility = Visibility.Collapsed;
                appbar_LockSitekeys.Visibility = Visibility.Collapsed;
            }
            else
            {
                appbar_ResetPW.Visibility = Visibility.Collapsed;
                appbar_AddSite.Visibility = Visibility.Collapsed;
                appbar_LockSitekeys.Visibility = Visibility.Collapsed;
            }
        }


        private void btn_MoreInfo_Click(object sender, RoutedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(MoreInfoView));
        }

    }

}
