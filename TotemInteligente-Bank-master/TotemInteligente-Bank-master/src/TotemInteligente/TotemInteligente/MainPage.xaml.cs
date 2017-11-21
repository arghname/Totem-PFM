using System;
using System.Linq;
using ServiceHelpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using TotemInteligente.Views;
using TotemInteligente.Models;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TotemInteligente
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Task processingLoopTask;
        private bool isProcessingLoopInProgress;
        private bool isProcessingPhoto;

        public MainPage()
        {
            this.InitializeComponent();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            this.cameraControl.ImageCaptured += CameraControl_ImageCaptured;
            this.cameraControl.CameraRestarted += CameraControl_CameraRestarted;
            this.cameraControl.FilterOutSmallFaces = true;
            this.cameraControl.HideCameraControls();
            this.cameraControl.CameraAspectRatioChanged += CameraControl_CameraAspectRatioChanged;
        }

        #region Camera
        private void CameraControl_CameraAspectRatioChanged(object sender, EventArgs e)
        {
            this.UpdateWebCamHostGridSize();
        }

        private async void CameraControl_CameraRestarted(object sender, EventArgs e)
        {
            // We induce a delay here to give the camera some time to start rendering before we hide the last captured photo.
            // This avoids a black flash.
            await Task.Delay(500);
        }

        private async void CameraControl_ImageCaptured(object sender, ImageAnalyzer e)
        {
            await this.cameraControl.StopStreamAsync();
        }

        private async Task StartWebCameraAsync()
        {
            await this.cameraControl.StartStreamAsync(isForRealTimeProcessing: true);
            await Task.Delay(250);

            UpdateWebCamHostGridSize();
        }

        private void UpdateWebCamHostGridSize()
        {
            this.webCamHostGrid.Width = this.webCamHostGrid.ActualHeight * (this.cameraControl.CameraAspectRatio != 0 ? this.cameraControl.CameraAspectRatio : 1.777777777777);
        }

        private async Task ProcessCameraCapture(ImageAnalyzer e)
        {
            if (e == null)
            {
                this.isProcessingPhoto = false;
                return;
            }

            DateTime start = DateTime.Now;

            await e.DetectFacesAsync(true, true);

            AddStep();

            if (e.DetectedFaces.Any())
            {
                AddStep();
                UserData result = new UserData();
                result.Face = e.DetectedFaces.FirstOrDefault();
                await e.IdentifyFacesAsync();
                AddStep();
                result.Datetime = DateTime.Now;
                if (e.IdentifiedPersons.Any())
                {
                    //Encontrou alguem
                    result.Person = e.IdentifiedPersons.FirstOrDefault();
                    await e.DetectEmotionAsync();
                    if (e.DetectedEmotion.Any())
                    {
                        result.Emotion = e.DetectedEmotion.FirstOrDefault();
                        this.Frame.Navigate(typeof(Home), result);
                    }
                    else
                    {
                        this.Frame.Navigate(typeof(Home), result);
                    }
                }
                else
                {
                    //Não encontrou ninguém.
                    await e.DetectEmotionAsync();
                    if (e.DetectedEmotion.Any())
                    {
                        result.Emotion = e.DetectedEmotion.FirstOrDefault();
                        this.Frame.Navigate(typeof(Home), result);
                    }
                    else
                    {
                        this.Frame.Navigate(typeof(Home), result);
                    }
                }
            }
            else
            {

            }

            TimeSpan latency = DateTime.Now - start;

            this.isProcessingPhoto = false;
        }

        private void StartProcessingLoop()
        {
            this.isProcessingLoopInProgress = true;

            if (this.processingLoopTask == null || this.processingLoopTask.Status != TaskStatus.Running)
            {
                this.processingLoopTask = Task.Run(() => this.ProcessingLoop());
            }
        }

        private async void ProcessingLoop()
        {
            while (this.isProcessingLoopInProgress)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (!this.isProcessingPhoto)
                    {
                        this.isProcessingPhoto = true;
                        if (this.cameraControl.NumFacesOnLastFrame == 0)
                        {
                            await this.ProcessCameraCapture(null);
                        }
                        else
                        {
                            AddStep();
                            await this.ProcessCameraCapture(await this.cameraControl.CaptureFrameAsync());
                        }
                    }
                });

                await Task.Delay(this.cameraControl.NumFacesOnLastFrame == 0 ? 100 : 1000);
            }
        }
        #endregion

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this.isProcessingLoopInProgress = false;
            this.cameraControl.CameraAspectRatioChanged -= CameraControl_CameraAspectRatioChanged;

            await this.cameraControl.StopStreamAsync();
            base.OnNavigatingFrom(e);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.StartWebCameraAsync();
            base.OnNavigatedTo(e);
        }

        private async void AddStep()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.Steps.Value += 1;
            });
        }


        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.TestConnection())
            {
                if (!isProcessingLoopInProgress)
                {
                    this.Steps.Value = 1;
                    this.StartProcessingLoop();
                }
            }
        }

        private async void Config(object sender, RoutedEventArgs e)
        {
            PasswordBox inputTextBox = new PasswordBox();
            inputTextBox.Height = 32;
            inputTextBox.PlaceholderText = "*********";
            ContentDialog dialog = new ContentDialog();
            dialog.Content = inputTextBox;
            dialog.Title = "Senha";
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Ok";
            dialog.SecondaryButtonText = "Cancel";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                GoSetting(inputTextBox.Password);
            }

        }

        private async void GoSetting(string text)
        {
            if (text == "123")
            {
                this.Frame.Navigate(typeof(Settings));
            }
            else
            {
                App.ShowDialog("Senha inválida!");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
