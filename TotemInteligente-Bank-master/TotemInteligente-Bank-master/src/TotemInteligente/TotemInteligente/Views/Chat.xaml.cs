using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using TotemInteligente.Models;
using System.Collections.ObjectModel;
using ServiceHelpers;
using Windows.Media.SpeechSynthesis;
using System.Text;
using System.Net;
using ZXing;
using Windows.UI.Core;
using Windows.Media.SpeechRecognition;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TotemInteligente.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Chat : Page
    {
        public ObservableCollection<Messages> ListOfMessage { get; set; } = new ObservableCollection<Messages>();
        private UserData globalData;
        bool IsVoiceEnabled;
        private SpeechRecognizer speechRecognizer;
        private CoreDispatcher dispatcher;
        private StringBuilder dictatedTextBuilder;
        private string SpeechResult;
        private bool isListening;
        private static uint HResultPrivacyStatementDeclined = 0x80045509;
        private Task processingLoopTask;
        private bool isProcessingLoopInProgress;
        private bool isProcessingPhoto;

        public Chat()
        {
            this.InitializeComponent();
            lv.DataContext = this;
            ListOfMessage.CollectionChanged += (s, args) => ScrollToBottom();
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

            UserData result = new UserData();
            await e.DetectEmotionAsync();
            if (e.DetectedEmotion.Any())
            {
                result.Emotion = e.DetectedEmotion.FirstOrDefault();
                //SAVE INFO ABOUT EMOTION
            }
            else
            {
                //SAVE INFO ABOUT EMOTION
            }

            TimeSpan latency = DateTime.Now - start;
            this.isProcessingLoopInProgress = false;
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
                            await this.ProcessCameraCapture(await this.cameraControl.CaptureFrameAsync());
                        }
                    }
                });

                await Task.Delay(this.cameraControl.NumFacesOnLastFrame == 0 ? 100 : 1000);
            }
        }
        #endregion

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.StartWebCameraAsync();
            if (e.Parameter == null)
            {
                //??
                UserName.Text = "Olá! Seja bem vindo.";
            }
            else
            {
                UserData result = e.Parameter as UserData;
                globalData = result;

                if (result.Person != null)
                {
                    UserName.Text = $"Olá, {result.Person.Person.Name}";
                }
                else
                {
                    UserName.Text = "Olá! Seja bem vindo.";
                }

                if (result.Chat == TypeOfChat.Text)
                {
                    rpText.Visibility = Visibility.Visible;
                    rpVoice.Visibility = Visibility.Collapsed;
                    rpLibras.Visibility = Visibility.Collapsed;
                    ImageHeader.Source = new BitmapImage(new Uri("ms-appx:///Assets/Elementos/ATEND_TEXTO@3x.png"));
                }
                else if (result.Chat == TypeOfChat.Voice)
                {
                    IsVoiceEnabled = true;
                    this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
                    dictatedTextBuilder = new StringBuilder();
                    StartRecognizing();
                    rpText.Visibility = Visibility.Collapsed;
                    rpVoice.Visibility = Visibility.Visible;
                    rpLibras.Visibility = Visibility.Collapsed;
                    ImageHeader.Source = new BitmapImage(new Uri("ms-appx:///Assets/Elementos/ATEND_VOZ@3x.png"));
                }
                else if (result.Chat == TypeOfChat.Libras)
                {
                    rpText.Visibility = Visibility.Collapsed;
                    lv.Visibility = Visibility.Collapsed;
                    rpVoice.Visibility = Visibility.Collapsed;
                    rpLibras.Visibility = Visibility.Visible;
                    ImageHeader.Source = new BitmapImage(new Uri("ms-appx:///Assets/Elementos/ATEND_LIBRAS@3x.png"));
                    try
                    {
                        Uri targetUri = new Uri("https://www.projetogiulia.com.br/api/?sinal=" + UserName.Text);
                        wv.Navigate(targetUri);
                    }
                    catch (Exception ex)
                    {
                        // Bad address.
                    }
                }
            }
            ShowResponseMessage("Em que posso lhe ajudar?", true);
            base.OnNavigatedTo(e);
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.isProcessingLoopInProgress = false;
            this.cameraControl.CameraAspectRatioChanged -= CameraControl_CameraAspectRatioChanged;

            await this.cameraControl.StopStreamAsync();
            if (this.speechRecognizer != null)
            {
                if (isListening)
                {
                    await this.speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                    isListening = false;
                }

                speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
                speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                speechRecognizer.HypothesisGenerated -= SpeechRecognizer_HypothesisGenerated;

                this.speechRecognizer.Dispose();
                this.speechRecognizer = null;
            }
        }

        public async void ShowResponseMessage(string text, bool welcome = false)
        {
            StartProcessingLoop();
            if (text == "No good match found in the KB")
            {
                text = "Desculpe, encontrei um problema.";
                ListOfMessage.Add(new Messages { Username = "BIA", Message = text });
                if (IsVoiceEnabled)
                {
                    if (App.IsSpeechOnline)
                    {
                        BingSpeechHelper.TTS(text);
                    }
                    else
                    {
                        SpeechSynthesisStream synthesisStream = await SpeechHelper.TTS(text);
                        media.AutoPlay = true;
                        media.SetSource(synthesisStream, synthesisStream.ContentType);
                        media.Play();
                    }
                }
            }
            else
            {
                ListOfMessage.Add(new Messages { Username = "BIA", Message = text });
                if (IsVoiceEnabled)
                {
                    if (App.IsSpeechOnline)
                    {
                        BingSpeechHelper.TTS(text);
                    }
                    else
                    {
                        SpeechSynthesisStream synthesisStream = await SpeechHelper.TTS(text);
                        media.AutoPlay = true;
                        media.SetSource(synthesisStream, synthesisStream.ContentType);
                        media.Play();
                    }
                    if (!welcome)
                    {
                        ListOfMessage.Add(new Messages { Username = "BIA", Message = "Quer imprimir seu código do pré-atendimento? Fale SIM ou NÃO" });
                    }
                }
                else
                {
                    if (!welcome)
                    {
                        ListOfMessage.Add(new Messages { Username = "BIA", Message = "Quer imprimir seu código do pré-atendimento? Fale SIM ou NÃO" });
                    }
                }
            }
        }

        public async void Send_Click(object sender, RoutedEventArgs e)
        {
            string name = globalData.Person != null ? globalData.Person.Person.Name : "Usuário";
            Messages msg = new Messages { Username = name, Message = text.Text };
            ListOfMessage.Add(msg);
            if (text.Text.ToLower() == "sim")
            {
                GenerateBarcode("");
            }
            else
            {
                if (text.Text.ToLower().Replace(" ","") == "não" || text.Text.ToLower().Replace(" ", "") == "nao")
                {
                    ShowResponseMessage("Em que mais posso lhe ajudar?", true);
                }
                else
                {
                    QnAMakerResults response = await QnAHelper.RequestAnswer(text.Text);
                    if (response != null)
                    {
                        ShowResponseMessage(response.answers[0].answer);
                    }
                    else
                    {
                        ShowResponseMessage("Ocorreu algum problema. Podemos tentar novamente?");
                    }
                }
            }
        }

        public async void SendAudioText(string text)
        {
            string name = globalData.Person != null ? globalData.Person.Person.Name : "Usuário";
            Messages msg = new Messages { Username = name, Message = text };
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ListOfMessage.Add(msg);
            });
            if (text.ToLower().Replace(" ", "") == "sim")
            {
                GenerateBarcode("");
            }
            else
            {
                QnAMakerResults response = await QnAHelper.RequestAnswer(text);
                if (response != null)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ShowResponseMessage(response.answers[0].answer);
                    });
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ShowResponseMessage("Ocorreu algum problema. Podemos tentar novamente?");
                    });
                }
            }
            SpeechResult = "";
        }

        private void ScrollToBottom()
        {
            var selectedIndex = lv.Items.Count - 1;
            if (selectedIndex < 0)
                return;

            lv.SelectedIndex = selectedIndex;
            lv.UpdateLayout();

            lv.ScrollIntoView(lv.SelectedItem);
        }

        private void Text_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Send_Click(sender, e);
                text.Text = "";
            }
        }


        #region Generate Barcode and QRCode
        private async void GenerateBarcode(string text)
        {
            //Just for tests
            Random R = new Random();
            text = "78" + ((long)R.Next(0, 100000) * (long)R.Next(0, 100000)).ToString().PadLeft(10, '0');

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BarcodeWriter barcodeWriter = new BarcodeWriter()
                {
                    Format = BarcodeFormat.EAN_13
                };
                var bitmap = barcodeWriter.Write(text);
                CodeImage.Source = bitmap;
                CodeGrid.Visibility = Visibility.Visible;
            });
        }

        private void GenerateQRCode(string text)
        {
            //Just for tests
            text = "Prova de vida";

            BarcodeWriter barcodeWriter = new BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE
            };
            var bitmap = barcodeWriter.Write(text);
            CodeImage.Source = bitmap;
            CodeGrid.Visibility = Visibility.Visible;
        }

        #endregion

        #region MainPageButton
        private void GoBackToMainPage(object sender, RoutedEventArgs e)
        {
            this.Frame.BackStack.Clear();
            this.Frame.Navigate(typeof(MainPage));
        }
        #endregion

        #region VOICE
        private async void StartRecognizing()
        {
            if (speechRecognizer != null)
            {
                speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
                speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                speechRecognizer.HypothesisGenerated -= SpeechRecognizer_HypothesisGenerated;

                this.speechRecognizer.Dispose();
                this.speechRecognizer = null;
            }

            //Choose language
            this.speechRecognizer = new SpeechRecognizer();

            var dictationConstraint = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "dictation");
            speechRecognizer.Constraints.Add(dictationConstraint);
            SpeechRecognitionCompilationResult result = await speechRecognizer.CompileConstraintsAsync();

            speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
            speechRecognizer.HypothesisGenerated += SpeechRecognizer_HypothesisGenerated;
        }

        private async void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            if (args.Result.Confidence == SpeechRecognitionConfidence.Medium || args.Result.Confidence == SpeechRecognitionConfidence.High)
            {
                dictatedTextBuilder.Append(args.Result.Text + " ");
                SpeechResult = dictatedTextBuilder.ToString();
            }
            else
            {
                SpeechResult = dictatedTextBuilder.ToString();
            }
            string tk = await GetTranslateKey();
            string r = await Translate(tk, SpeechResult);
            SendAudioText(r);
            dictatedTextBuilder.Clear();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ContinuousRecognize_Click(this, null);
            });
        }

        private void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
        {
            if (args.Status != SpeechRecognitionResultStatus.Success)
            {
                if (args.Status == SpeechRecognitionResultStatus.TimeoutExceeded)
                {
                    SpeechResult = dictatedTextBuilder.ToString();
                    isListening = false;
                }
                else
                {
                    isListening = false;
                }
            }
        }

        private async void SpeechRecognizer_HypothesisGenerated(SpeechRecognizer sender, SpeechRecognitionHypothesisGeneratedEventArgs args)
        {

            string hypothesis = args.Hypothesis.Text;
            string textboxContent = dictatedTextBuilder.ToString() + " " + hypothesis + " ...";

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SpeechResult = textboxContent;
            });
        }

        public async void ContinuousRecognize_Click(object sender, RoutedEventArgs e)
        {
            mic.IsEnabled = false;
            if (isListening == false)
            {
                if (speechRecognizer.State == SpeechRecognizerState.Idle)
                {
                    try
                    {
                        isListening = true;
                        await speechRecognizer.ContinuousRecognitionSession.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        if ((uint)ex.HResult == HResultPrivacyStatementDeclined)
                        {
                            var contentDialog = new ContentDialog
                            {
                                Title = "Privacidade",
                                Content = "Você precisa autorizar o uso do microfone. Deseja fazer isso agora?",
                                CloseButtonText = "Não",
                                PrimaryButtonText = "Sim"
                            };
                            ContentDialogResult result = await contentDialog.ShowAsync();
                            if (result == ContentDialogResult.Primary)
                            {
                                OpenPrivacySettings();
                            }
                        }
                        else
                        {
                            var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Exception");
                            await messageDialog.ShowAsync();
                        }

                        isListening = false;
                    }
                }
            }
            else
            {
                isListening = false;
                if (speechRecognizer.State != SpeechRecognizerState.Idle)
                {
                    try
                    {
                        await speechRecognizer.ContinuousRecognitionSession.StopAsync();

                        SpeechResult = String.Empty;
                    }
                    catch (Exception exception)
                    {
                        var messageDialog = new Windows.UI.Popups.MessageDialog(exception.Message, "Exception");
                        await messageDialog.ShowAsync();
                    }
                }
            }
            mic.IsEnabled = true;
        }

        private async void OpenPrivacySettings()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-speechtyping"));
        }

        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {

        }

        #endregion


        #region Code for Tests and Samples

        private async Task<string> GetTranslateKey()
        {
            string apiKey = "5bf58bb0db60486ebf18e07a3a493df7";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            string uri = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
            HttpResponseMessage response;
            StringContent content = new StringContent(String.Empty, Encoding.UTF8, "application/json");

            response = await client.PostAsync(uri, content);
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> Translate(string token, string text)
        {
            string from = "en";
            string to = "pt";
            string uri = "https://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + WebUtility.UrlEncode(text) + "&from=" + from + "&to=" + to;

            WebRequest webRequest = WebRequest.Create(uri);
            webRequest.Headers["Authorization"] = "Bearer" + " " + token;

            using (WebResponse response = await webRequest.GetResponseAsync())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    DataContractSerializer dcs = new DataContractSerializer(Type.GetType("System.String"));
                    string translation = (string)dcs.ReadObject(stream);
                    return translation;
                }
            }

        }

        #endregion

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            CodeGrid.Visibility = Visibility.Collapsed;
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}

