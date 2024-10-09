using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBot
{
    public class SendSticker
    {
        public static async Task SendStickerAsync(ITelegramBotClient client, Update update)
        {
            //var message = update.Message;
            var callbackQuery = update.CallbackQuery;
            var chatId = callbackQuery.Message.Chat.Id;
            if (callbackQuery.Data == "sticker_clicked")
            {

                var sticker = new InputOnlineFile("https://telegrambots.github.io/book/docs/sticker-dali.webp");
                await client.SendStickerAsync(chatId, sticker);
                var caption = "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>";
                await client.SendTextMessageAsync(chatId, caption, parseMode: ParseMode.Html);
            }
            else
            {
                await client.SendTextMessageAsync(chatId, "Not the right move");
            }
            return;
        }
    }
}
