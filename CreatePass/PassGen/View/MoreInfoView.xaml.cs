using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CreatePass
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MoreInfoView : Page
    {

        public MoreInfoView()
        {
            this.InitializeComponent();
            
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

        private void btn_SendEmail_Click(object sender, RoutedEventArgs e)
        {
            ComposeEmail("helland@mailbox.org","[CreatPass]");
        }



        private async void ComposeEmail(string recipient, string subject)
        {
            var emailMessage = new EmailMessage();
            emailMessage.Subject = subject;

            var emailRecipient = new EmailRecipient(recipient);
            emailMessage.To.Add(emailRecipient);

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }
    }
}
