using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.DataTransfer;
using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PassGen
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Settings settings;
        PasswordGeneration passGenerator;
        ResourceLoader rsLoader;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            
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
      
        }
        
        

        private async void btn_Generate_Click(object sender, RoutedEventArgs e)
        {
            if (txt_MasterKey.Password.Length < 8)
            {
                var test = new Windows.UI.Popups.MessageDialog(rsLoader.GetString("MasterKeyToShortDialog"));
                await test.ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(txt_SiteHint.Text))
                return;

            txt_Password.Text = passGenerator.Generate(txt_MasterKey.Password, txt_SiteHint.Text);
            btn_Copy.Visibility = Visibility.Visible;

            if (settings.AutoCopy)
                WritePwToClipboard();
        }
        
        private void btn_Copy_Click(object sender, RoutedEventArgs e)
        {
            WritePwToClipboard();
        }

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
                btn_ToggelSalt.Content = rsLoader.GetString("sett_txb_ShowSalt_hide");
            }
            else
            {
                txt_Salt.Visibility = Visibility.Collapsed;
                btn_SaveSalt.Visibility = Visibility.Collapsed;
                btn_ToggelSalt.Content = rsLoader.GetString("sett_txb_ShowSalt_show");
            }
        }

        private async void btn_SaveSalt_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Salt.Text.Length < 8)
            {
                var mBoxLength = new Windows.UI.Popups.MessageDialog(rsLoader.GetString("SaltToShortDialog"));
                await mBoxLength.ShowAsync();
                return;
            }

            var messageBoxChange = new ContentDialog() { Title = rsLoader.GetString("ChangeSaltDia_Titel"), Content = rsLoader.GetString("ChangeSaltDia_Content"), PrimaryButtonText = rsLoader.GetString("ChangeSaltDia_Primary"), SecondaryButtonText = rsLoader.GetString("ChangeSaltDia_Secondary") };
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

        private void WritePwToClipboard()
        {
            var datapack = new DataPackage();
            datapack.RequestedOperation = DataPackageOperation.Copy;
            datapack.SetText(txt_Password.Text);
            Clipboard.SetContent(datapack);
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

        private void btn_MoreInfo_Click(object sender, RoutedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(MoreInfoView));
        }
    }
}
