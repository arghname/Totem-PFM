using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TotemInteligente.Models;

namespace ServiceHelpers
{
    public static class QnAHelper
    {
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

        public static async Task<QnAMakerResults> RequestAnswer(string text)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                string uri = "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/3184701f-ddb6-44c5-b663-eb0a54536995/generateAnswer";
                HttpResponseMessage response;
                var body = new
                {
                    question = text
                };
                var stringPayload = JsonConvert.SerializeObject(body);
                StringContent content = new StringContent(stringPayload, Encoding.ASCII, "application/json");
                response = await client.PostAsync(uri, content);
                string jsonstringresult = await response.Content.ReadAsStringAsync();
                jsonstringresult = WebUtility.HtmlDecode(jsonstringresult);
                return JsonConvert.DeserializeObject<QnAMakerResults>(jsonstringresult);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
