using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace TelegramBot
{
    public class FindVidioInYoutube : ITelegrammAction
    {
        public string ActionKey => "vidio_clicked";

        public string ActionTitle => "Find vidio in youtube";

        public async Task<bool> RunAction(ITelegramBotClient client, Update update, bool isCallback, long chatId)
        {
            string vidioQuery = update.Message?.Text;
            using (HttpClient httpClient = new HttpClient())
            {
                if (!isCallback)
                {
                    await client.SendTextMessageAsync(chatId, "Send me video name");
                    return true;
                }
                await client.SendTextMessageAsync(chatId, "Now wait. I'll send vidio...");
                string URL = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q=" +
                    $"{Uri.EscapeDataString(vidioQuery)}&type=video&key=AIzaSyALg0X4s6nxDZPkL6-9JwC9hk62sCBicsw";

                HttpResponseMessage responseMessage = await httpClient.GetAsync(URL);
                string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

                JObject jsonData = JObject.Parse(jsonResponse);
                var items = jsonData["items"];
                if (items != null && items.HasValues)
                {

                    string videoId = items[0]["id"]["videoId"].ToString();
                    string title = items[0]["snippet"]["title"].ToString();
                    string videoUrl = $"https://www.youtube.com/watch?v={videoId}";
                    await client.SendTextMessageAsync(chatId, $"I find: {title}\n{videoUrl}");
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, "К сожалению, я не нашел музыку с таким названием.");
                }
                return false;
            }
        }
    }
}
