using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TotemInteligente.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        private static ApplicationDataContainer localSettings;
        private static StorageFolder localFolder;
        public Settings()
        {
            this.InitializeComponent();
            localSettings = ApplicationData.Current.LocalSettings;
            localFolder = ApplicationData.Current.LocalFolder;
            if (Convert.ToBoolean(localSettings.Values["IsSpeechOnline"]))
            {
                IsSpeechOnline.IsOn = true;
            }
        }

        /// <summary>
        /// Saves the subscription key to isolated storage.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        private static void SaveSettingsToIsolatedStorage(string subscriptionKey)
        {
            
            localSettings.Values["IsSpeechOnline"] = App.IsSpeechOnline;
        }

        private void NewFace(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    App.IsSpeechOnline = true;
                    localSettings.Values["IsSpeechOnline"] = App.IsSpeechOnline;
                }
                else
                {
                    App.IsSpeechOnline = false;
                    localSettings.Values["IsSpeechOnline"] = App.IsSpeechOnline;
                }
            }
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
