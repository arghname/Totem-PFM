using TotemInteligente.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using ServiceHelpers;
using Windows.Media.SpeechSynthesis;
using System.Net.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TotemInteligente.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        private UserData globalData;
        public Home()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null)
            {
                //??
                NoneButton.Visibility = Visibility.Collapsed;
                UserName.Text = "Olá! Seja bem vindo.";
            }
            else
            {
                UserData result = e.Parameter as UserData;
                globalData = result;
                result.Chat = TypeOfChat.None;
                if (result.Person != null)
                {
                    NoneButton.Visibility = Visibility.Visible;
                    UserName.Text = $"Olá, {result.Person.Person.Name}";

                }
                else
                {
                    NoneButton.Visibility = Visibility.Collapsed;
                    UserName.Text = "Olá! Seja bem vindo.";
                }
            }
            if (App.IsSpeechOnline)
            {
                BingSpeechHelper.TTS(UserName.Text);
            }
            else
            {
                TTS(UserName.Text);
            }
            base.OnNavigatedTo(e);
        }

        private async void TTS(string text)
        {
            SpeechSynthesisStream synthesisStream = await SpeechHelper.TTS(text);
            media.AutoPlay = true;
            media.SetSource(synthesisStream, synthesisStream.ContentType);
            media.Play();
        }

        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {

        }

        private void Text_Choose(object sender, RoutedEventArgs e)
        {
            globalData.Chat = TypeOfChat.Text;
            this.Frame.Navigate(typeof(Chat), globalData);
        }

        private void Voice_Choose(object sender, RoutedEventArgs e)
        {
            globalData.Chat = TypeOfChat.Voice;
            this.Frame.Navigate(typeof(Chat), globalData);
        }

        private void GoBackToMainPage(object sender, RoutedEventArgs e)
        {
            this.Frame.BackStack.Clear();
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Libras_Choose(object sender, RoutedEventArgs e)
        {
            globalData.Chat = TypeOfChat.Libras;
            this.Frame.Navigate(typeof(Chat), globalData);
        }

        private void None(object sender, RoutedEventArgs e)
        {
            UserName.Text = "Olá! Seja bem vindo.";
            globalData.Person = null;
        }
    }
}
