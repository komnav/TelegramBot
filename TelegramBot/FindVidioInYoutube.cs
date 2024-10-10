using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using YoutubeExplode.Videos;

namespace TelegramBot
{
    public class FindVidioInYoutube
    {
        public static async Task FindVidioInYoutubeAsync(ITelegramBotClient client, Update update)
        {
            var chatId = update.Message?.Chat.Id;
            await client.SendTextMessageAsync(chatId, "Now wait. I'll send vidio...");
            var vidioQuery = update.Message.Text;

            using (HttpClient httpClient = new HttpClient())
            {
                string URL = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q=" +
                    $"{Uri.EscapeDataString(vidioQuery)}&type=video&key=AIzaSyALg0X4s6nxDZPkL6-9JwC9hk62sCBicsw";

                HttpResponseMessage responseMessage = await httpClient.GetAsync(URL);
                string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

                JObject jsonData = JObject.Parse(jsonResponse);
                var item = jsonData["item"];
                if (item != null && item.HasValues)
                {
                    string vidioId = item[0]["id"]["vidioId"].ToString();
                    string title = item[0]["snippet"]["title"].ToString();
                    string vidioUrl = $"https://www.youtube.com/watch?v={vidioId}";


                    await client.SendTextMessageAsync(chatId, $"I find: {title}\n{vidioUrl}");
                    return;
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, "I'm sorry I'll not find vidio.");
                }
            }
        }
    }
}
