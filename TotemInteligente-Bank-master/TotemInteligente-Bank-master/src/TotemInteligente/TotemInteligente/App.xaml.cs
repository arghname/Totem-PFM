using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ServiceHelpers;
using Windows.UI.Core;
using Windows.Storage;
using Windows.Networking.Connectivity;
using Windows.Graphics.Display;

namespace TotemInteligente
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static bool IsSpeechOnline = false;
        public static FontFamily Gilroy = new FontFamily("/Fonts/Gilroy/Gilroy-Regular.otf#Gilroy-Regular");
        public static FontFamily GilroyBold = new FontFamily("/Fonts/Gilroy/Gilroy-Bold.otf#Gilroy-Bold");
        public static FontFamily GilroyThin = new FontFamily("/Fonts/Gilroy/Gilroy-Thin.otf#Gilroy-Thin");
        public static FontFamily GilroyItalic = new FontFamily("/Fonts/Gilroy/Gilroy-RegularItalic.otf#Gilroy-RegularItalic");
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                //Set all API Keys here
                FaceServiceHelper.ApiKey = "823224ee82d4438fb5d059dea5cc89f0";
                FaceServiceHelper.ApiKeyRegion = "WestUS";
                EmotionServiceHelper.ApiKey = "547a1c3d5372494d9906c5daa35afe2d";
                QnAHelper.ApiKey = "6e07db4ab74f45cfaf485fa76e8ed103";
                BingSpeechHelper.ApiKey = "4979002208f3474ebc7e34d6e44c423c";
                if (Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values["IsSpeechOnline"]))
                {
                    IsSpeechOnline = true;
                }

                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait | DisplayOrientations.PortraitFlipped;
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                // Register a global back event handler. This can be registered on a per-page-bases if you only have a subset of your pages
                // that needs to handle back or if you want to do page-specific logic before deciding to navigate back on those pages.
                SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        public static void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // If we can go back and the event has not already been handled, do so.
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        #region TestConnection
        public static bool TestConnection()
        {
            ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
            if (InternetConnectionProfile == null)
            {
                ShowDialog("Sem conexão com a internet");
                return false;
            }
            return true;
        }
        #endregion

        public async static void ShowDialog(string message)
        {
            ContentDialog cd = new ContentDialog()
            {
                CloseButtonText = "OK",
                Content = message
            };
            await cd.ShowAsync();
        }
    }
}
