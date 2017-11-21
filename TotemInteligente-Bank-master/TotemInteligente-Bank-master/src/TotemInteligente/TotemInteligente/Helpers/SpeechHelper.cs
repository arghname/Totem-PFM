using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;

namespace ServiceHelpers
{
    public static class SpeechHelper
    {
        public static async Task<SpeechSynthesisStream> TTS(string text)
        {

            var synthesizer = new SpeechSynthesizer();

            var speechContext = ResourceContext.GetForCurrentView();
            speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };

            var voices = SpeechSynthesizer.AllVoices;

            VoiceInformation currentVoice = synthesizer.Voice;

            SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(text);

            return synthesisStream;
        }
    }
}
