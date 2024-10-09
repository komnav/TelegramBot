using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class FindMusic
    {
        public static async Task SearchMusicOnYouTube(ITelegramBotClient client, Update update)
        {
            string musicQuery = update.Message.Text;

            using (HttpClient httpClient = new HttpClient())
            {

                string url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={Uri.EscapeDataString(musicQuery)}&type=video&key=AIzaSyALg0X4s6nxDZPkL6-9JwC9hk62sCBicsw";

                HttpResponseMessage response = await httpClient.GetAsync(url);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                JObject jsonData = JObject.Parse(jsonResponse);
                var items = jsonData["items"];
                if (items != null && items.HasValues)
                {

                    string videoId = items[0]["id"]["videoId"].ToString();
                    string title = items[0]["snippet"]["title"].ToString();
                    string videoUrl = $"https://www.youtube.com/watch?v={videoId}";


                    await client.SendTextMessageAsync(update.Message?.Chat.Id, $"Я нашел: {title}\n{videoUrl}");
                }
                else
                {
                    await client.SendTextMessageAsync(update.Message?.Chat.Id, "К сожалению, я не нашел музыку с таким названием.");
                }

            }
        }
    }
}
