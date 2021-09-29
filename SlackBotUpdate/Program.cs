using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SlackBotUpdate
{
    class Program
    {
        public class Response
        {
            public bool ok { get; set; }
        }

        static async Task Main(string[] args)
        {
            var dayOfWeek = GetDayOfTheWeek();
            string myToken = "";

            try
            {
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent("Xr02F6TFLX0F"), "response");
                form.Add(new StringContent("vlad_test123"), "triggers");
                form.Add(new StringContent("test 12 4 5 6 7 8 "), "responses");
                form.Add(new StringContent(myToken), "token");
                form.Add(new StringContent("edit_slackbot_response"), "_x_reason");
                form.Add(new StringContent("online"), "_x_mode");

                var client = new HttpClient();
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
                var exFile = $@"D:\{dayOfWeek}.txt";

                File.WriteAllText(exFile, ex.ToString());
                System.Diagnostics.Process.Start("notepad.exe", exFile);
            }
        }

        private static string GetDayOfTheWeek()
        {
            return DateTime.Now.DayOfWeek.ToString();
        }
    }
}
