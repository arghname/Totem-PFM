using System;
using System.IO;
using System.Threading;
using TotemInteligente.Models;
using Windows.UI.Popups;
using Windows.Media.Playback;
using Windows.Media.Core;
using Windows.Storage;
using System.Net.Http;
using System.Text;
using Windows.System.Threading;

namespace ServiceHelpers
{
    public static class BingSpeechHelper
    {
        private static string accessToken;
        private static string apiKey;
        public static string ApiKey
        {
            get { return apiKey; }
            set
            {
                var changed = apiKey != value;
                apiKey = value;
            }
        }

        public static async void TTS(string text)
        {
            TTSAuthentication auth = new TTSAuthentication(apiKey);
            await auth.HttpPost();
            try
            {
                accessToken = auth.GetAccessToken();
                string requestUri = "https://speech.platform.bing.com/synthesize";

                var cortana = new Synthesize();

                cortana.OnAudioAvailable += PlayAudio;
                cortana.OnError += ErrorHandler;

                // Reuse Synthesize object to minimize latency
                cortana.Speak(CancellationToken.None, new Synthesize.InputOptions()
                {
                    RequestUri = new Uri(requestUri),
                    // Text to be spoken.
                    Text = text,
                    VoiceType = Gender.Female,
                    // Refer to the documentation for complete list of supported locales.
                    Locale = "pt-BR",
                    // You can also customize the output voice. Refer to the documentation to view the different
                    // voices that the TTS service can output.
                    VoiceName = "Microsoft Server Speech Text to Speech Voice (pt-BR, HeloisaRUS)",
                    // Service can return audio in different output format.
                    OutputFormat = AudioOutputFormat.Riff16Khz16BitMonoPcm,
                    AuthorizationToken = "Bearer " + accessToken,
                }).Wait();
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog("Unable to complete the TTS request: [{0}]", ex.Message);
                return;
            }
        }

        /// <summary>
        /// This method is called once the audio returned from the service.
        /// It will then attempt to play that audio file.
        /// Note that the playback will fail if the output audio format is not pcm encoded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="GenericEventArgs{Stream}"/> instance containing the event data.</param>
        private static async void PlayAudio(object sender, GenericEventArgs<Stream> args)
        {
            var musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            var path = Path.Combine(musicLibrary.SaveFolder.Path, $"{ Guid.NewGuid() }.wav");
            using (var file = File.OpenWrite(path))
            {
                args.EventData.CopyTo(file);
                file.Flush();
            }
            var localFile = await StorageFile.GetFileFromPathAsync(path.ToString());
            // For SoundPlayer to be able to play the wav file, it has to be encoded in PCM.
            // Use output audio format AudioOutputFormat.Riff16Khz16BitMonoPcm to do that.
            MediaPlayer player = new MediaPlayer
            {
                Source = MediaSource.CreateFromStorageFile(localFile)
            };
            player.Play();
            args.EventData.Dispose();
            TimeSpan delay = TimeSpan.FromSeconds(15);
            ThreadPoolTimer DelayTimer = ThreadPoolTimer.CreateTimer(
                (source) =>
                {
                    player.Dispose();
                    File.Delete(path);
                }, delay);
            
        }

        /// <summary>
        /// Handler an error when a TTS request failed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="GenericEventArgs{Exception}"/> instance containing the event data.</param>
        private static void ErrorHandler(object sender, GenericEventArgs<Exception> e)
        {
            MessageDialog md = new MessageDialog("Unable to complete the TTS request: [{0}]", e.ToString());
        }
    }
}
