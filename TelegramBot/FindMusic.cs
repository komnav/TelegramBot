using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace TelegramBot
{
    public class FindMusic
    {
        public static async Task SearchMusicOnYouTube(ITelegramBotClient client, Update update)
        {
            var chatId = update.Message.Chat.Id;
            await client.SendTextMessageAsync(chatId, "Now wait, I'll send to music...");
            string musicQuery = update.Message.Text;

            using (HttpClient httpClient = new HttpClient())
            {

                string url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q=" +
                    $"{Uri.EscapeDataString(musicQuery)}&type=video&key=AIzaSyALg0X4s6nxDZPkL6-9JwC9hk62sCBicsw";

                HttpResponseMessage response = await httpClient.GetAsync(url);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                JObject jsonData = JObject.Parse(jsonResponse);
                var items = jsonData["items"];
                if (items != null && items.HasValues)
                {

                    string videoId = items[0]["id"]["videoId"].ToString();
                    string title = items[0]["snippet"]["title"].ToString();
                    string videoUrl = $"https://www.youtube.com/watch?v={videoId}";

                    var youtube = new YoutubeClient();
                    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);
                    var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                    if (streamInfo != null)
                    {
                        var filePath = $"{title}.mp3";
                        if (!System.IO.File.Exists(filePath))
                        {
                            await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);

                        }
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await client.SendAudioAsync(update.Message.Chat.Id, new InputOnlineFile(fileStream, $"{title}.mp3"));
                        }
                        System.IO.File.Delete(filePath);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(update.Message?.Chat.Id, "Не удалось найти аудиофайл для скачивания.");
                    }

                }
                else
                {
                    await client.SendTextMessageAsync(update.Message?.Chat.Id, "К сожалению, я не нашел музыку с таким названием.");
                }

            }
        }
    }
}
