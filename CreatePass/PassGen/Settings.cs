using Windows.ApplicationModel;
using Windows.Storage;

namespace PassGen
{
    public class Settings
    {
        ApplicationDataContainer localSettings;

        public int PwLength { get; private set; }
        public bool AutoCopy { get; private set; }

        public bool UseNumChars { get; private set; }
        public bool UseAlphaNumChars { get; private set; }
        public bool UseSpecialChars { get; private set; }

        public bool IsFirstLaunch { get; private set; }

        public string Salt { get; private set; }

        public Settings()
        {
            localSettings = ApplicationData.Current.LocalSettings;

            LoadOrCreatePwLen();
            LoadOrCreateAutoCopy();
            LoadOrCreatePwChars();
            LoadOrCreateSalt();
        }

        private void LoadOrCreateAutoCopy()
        {
            var autoCopySetting = localSettings.Values["autoCopy"];

            if (autoCopySetting == null)
            {
                localSettings.Values["autoCopy"] = false;
                autoCopySetting = localSettings.Values["autoCopy"];
            }
            AutoCopy = (bool)autoCopySetting;
        }

        private void LoadOrCreatePwLen()
        {
            var pwLenSetting = localSettings.Values["pwlen"];

            if (pwLenSetting == null)
            {
                localSettings.Values["pwlen"] = 18;
                pwLenSetting = localSettings.Values["pwlen"];
            }
            PwLength = (int)pwLenSetting;
        }

        private void LoadOrCreatePwChars()
        {
            var pwNum = localSettings.Values["pwCharsNum"];
            var pwAlphaNum = localSettings.Values["pwAlphaNum"];
            var pwSecial = localSettings.Values["pwSecial"];

            if (pwNum == null)
            {
                localSettings.Values["pwCharsNum"] = true;
                pwNum = localSettings.Values["pwCharsNum"];
            }

            if (pwAlphaNum == null)
            {
                localSettings.Values["pwAlphaNum"] = true;
                pwAlphaNum = localSettings.Values["pwAlphaNum"];
            }

            if (pwSecial == null)
            {
                localSettings.Values["pwSecial"] = true;
                pwSecial = localSettings.Values["pwSecial"];
            }

            UseNumChars = (bool)pwNum;
            UseAlphaNumChars = (bool)pwAlphaNum;
            UseSpecialChars = (bool)pwSecial;
        }

        private void LoadOrCreateSalt()
        {
            var salt = localSettings.Values["salt"];
            if (salt == null)
            {
                localSettings.Values["salt"] = Package.Current.InstalledDate.ToString();
                salt = localSettings.Values["salt"];
            }
            Salt = (string)salt;
        }

        private void LoadOrCreateFirstLoad()
        {
            var firstload = localSettings.Values["firstLoad"];
            if (firstload == null)
            {
                localSettings.Values["firstLoad"] = true;
                firstload = localSettings.Values["firstLoad"];
            }
            IsFirstLaunch = (bool)firstload;
        }

        public void UpdatePwLen(int pwLen)
        {
            localSettings.Values["pwLen"] = pwLen;
            PwLength = (int)localSettings.Values["pwLen"];
        }

        public void UpdateAutoCopy(bool autoCopy)
        {
            localSettings.Values["autoCopy"] = autoCopy;
            AutoCopy = (bool)localSettings.Values["autoCopy"];
        }

        public void UpdatePwChars(bool num, bool aplhaNum, bool special)
        {
            localSettings.Values["pwCharsNum"] = num;
            localSettings.Values["pwAlphaNum"] = aplhaNum;
            localSettings.Values["pwSecial"] = special;

            UseNumChars = (bool)localSettings.Values["pwCharsNum"];
            UseAlphaNumChars = (bool)localSettings.Values["pwAlphaNum"];
            UseSpecialChars = (bool)localSettings.Values["pwSecial"];

        }
        
        public void UpdateSalt(string newSalt)
        {
            localSettings.Values["salt"] = newSalt;
            Salt = (string)localSettings.Values["salt"];
        }
    }
}
