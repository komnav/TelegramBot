using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class Host
    {
        private static Dictionary<long, bool> _waitingForPhoto = new Dictionary<long, bool>();
        private static Dictionary<long, bool> _waitingForMusicName = new Dictionary<long, bool>();

        public Action<ITelegramBotClient, Update>? OnMessage;
        private TelegramBotClient _bot;
        public Host(string token)
        {
            _bot = new TelegramBotClient(token);
        }

        public void Start()
        {
            _bot.StartReceiving(UpdateHandler, ErrorHandler);
            Console.WriteLine("Bot lounch");
        }

        private async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            await Console.Out.WriteLineAsync("Error: " + exception.Message);
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;
            Console.WriteLine($"{message?.Chat.FirstName} | {message?.Text}");
            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
            {
                var chatId = update.Message.Chat.Id;

                if (_waitingForMusicName.ContainsKey(chatId) && _waitingForMusicName[chatId])
                {
                    _waitingForMusicName[chatId] = false;

                    _ = FindMusic.SearchMusicOnYouTube(client, update);
                }
                //else
                //{
                //    await client.SendTextMessageAsync(chatId, "Unfortunately, I didn't find any music with that title");
                //}
                else
                {

                    InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new[]
                    {
                    new []{InlineKeyboardButton.WithCallbackData("Find Sticker", "sticker_clicked") },
                    new []{InlineKeyboardButton.WithCallbackData("Convert photo to .jpg", ".jpg_clicked") },
                    new []{InlineKeyboardButton.WithCallbackData("Image quality enhancement", "image_clicked") },
                    new []{InlineKeyboardButton.WithCallbackData("Find Music", "music_clicked") },

                });

                    await client.SendTextMessageAsync(chatId, "Welcom to Our service", replyMarkup: keyboard, cancellationToken: token);
                }
            }


            else if (update.Type == UpdateType.CallbackQuery)
            {
                var callbackQuery = update.CallbackQuery;
                var chatId = callbackQuery.Message.Chat.Id;
                if (callbackQuery.Data == "sticker_clicked")
                {

                    _ = SendSticker.SendStickerAsync(client, update);

                    //await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Какой стикер вам нужен?", cancellationToken: token);
                }
                else if (callbackQuery.Data == ".jpg_clicked")
                {
                    await client.SendTextMessageAsync(chatId, "Please send me a photo to convert it to .jpg");
                    _waitingForPhoto[chatId] = true;

                }
                else if (callbackQuery.Data == "image_clicked")
                {
                    await client.SendTextMessageAsync(chatId, "Please send me a document to enhancer");
                    _waitingForPhoto[chatId] = true;
                }
                else if (callbackQuery.Data == "music_clicked")
                {
                    await client.SendTextMessageAsync(chatId, "Please send me name music");
                    _waitingForMusicName[chatId] = true;
                }
            }
            else if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Photo)
            {
                var chatId = update.Message.Chat.Id;

                if (_waitingForPhoto.ContainsKey(chatId) && _waitingForPhoto[chatId])
                {
                    _waitingForPhoto[chatId] = false;

                    _ = ImageConverter.ConvertToJpgAsync(client, update);
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, "I'm not waiting for a photo right now.");
                }
            }
            else if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Document)
            {
                var chatId = update.Message.Chat.Id;

                if (_waitingForPhoto.ContainsKey(chatId) && _waitingForPhoto[chatId])
                {
                    _waitingForPhoto[chatId] = false;

                    _ = EnhanceImage.EnchanceImageAsync(client, update);
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, "I'm not waiting for a document right now.");
                }
            }
        }
    }
}
