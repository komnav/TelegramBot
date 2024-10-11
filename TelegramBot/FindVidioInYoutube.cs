using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using YoutubeExplode.Videos;

namespace TelegramBot
{
    public class FindVidioInYoutube : ITelegrammAction
    {
        public string ActionKey => "vidio_clicked";

        public string ActionTitle => "Find vidio in youtube";

        public async Task<bool> RunAction(ITelegramBotClient client, Update update, bool isCallback, long chatId)
        {
            if (!isCallback)
            {
                await client.SendTextMessageAsync(chatId, "Send me video name");
                return true;
            }
            else
            {
                await client.SendTextMessageAsync(chatId, "Now wait. I'll send vidio...");
                var vidioQuery = update.Message.Text;
                using (HttpClient httpClient = new HttpClient())
                {
                    string URL = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q=" +
                        $"{Uri.EscapeDataString(vidioQuery)}&type=video&key=AIzaSyALg0X4s6nxDZPkL6-9JwC9hk62sCBicsw";

                    HttpResponseMessage responseMessage = await httpClient.GetAsync(URL);
                    string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

                    JObject jsonData = JObject.Parse(jsonResponse);
                    var item = jsonData["items"];
                    if (item != null && item.HasValues)
                    {
                        string vidioId = item[0]["id"]["vidioId"].ToString();
                        string title = item[0]["snippet"]["title"].ToString();
                        string vidioUrl = $"https://www.youtube.com/watch?v={vidioId}";


                        await client.SendTextMessageAsync(chatId, $"I find: {title}\n{vidioUrl}");
                        return false;
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, "I'm sorry I'll not find vidio.");
                    }
                    return false;
                }
            }
        }
    }
}
