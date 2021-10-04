using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SlackBotUpdate
{
    class Program
    {
        const string FirstResponseCode = "Xr02D1V9D92R";
        const string FirstResponseTrigger = "kibana is down";
        const string FirstResponseResponse = "Isaac: is it {0}?";

        const string SecondResponseCode = "Xr02CPH8L7D1";
        const string SecondResponseTrigger = "kibana down";
        const string SecondResponseResponse = "Isaac: is it {0}?";

        const string Token = "";
        const string Cookie = "";

        public class Response
        {
            public bool ok { get; set; }
        }

        static async Task Main(string[] args)
        {
            var dayOfWeek = GetDayOfTheWeek();

            await UpdateResponse(
                FirstResponseCode,
                FirstResponseTrigger,
                string.Format(FirstResponseResponse, dayOfWeek));

            await UpdateResponse(
                SecondResponseCode,
                SecondResponseTrigger,
                string.Format(SecondResponseResponse, dayOfWeek));
        }

        private static string GetDayOfTheWeek()
        {
            return DateTime.Now.DayOfWeek.ToString().ToLower();
        }

        private static async Task UpdateResponse(string responseCode, string responseTrigger, string responseResponse)
        {
            try
            {
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(responseCode), "response");
                form.Add(new StringContent(responseTrigger), "triggers");
                form.Add(new StringContent(responseResponse), "responses");
                form.Add(new StringContent(Token), "token");
                form.Add(new StringContent("edit_slackbot_response"), "_x_reason");
                form.Add(new StringContent("online"), "_x_mode");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Cookie", Cookie);
                var req = new HttpRequestMessage(HttpMethod.Post, "https://infotrack.slack.com/api/slackbot.responses.edit") { Content = form };
                var res = await client.SendAsync(req);

                if (!res.IsSuccessStatusCode)
                    throw new Exception("Bad status code result : " + res.StatusCode);

                var content = await res.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<Response>(content);

                if (!response.ok)
                    throw new Exception("Operation not successful : " + content);
            }
            catch (Exception ex)
            {
                var exFile = $@"D:\{DateTime.Now.Ticks}.txt";

                File.WriteAllText(exFile, ex.ToString());
                System.Diagnostics.Process.Start("notepad.exe", exFile);
            }
        }

    }
}
