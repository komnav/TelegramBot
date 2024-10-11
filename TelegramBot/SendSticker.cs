using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBot
{
    public class SendSticker : ITelegrammAction
    {
        public string ActionKey => "sticker_clicked";

        public string ActionTitle => "Find Sticker";

        public async Task<bool> RunAction(ITelegramBotClient client, Update update, bool isCallback, long chatId)
        {
            //var message = update.Message;
            var sticker = new InputOnlineFile("https://telegrambots.github.io/book/docs/sticker-dali.webp");
            await client.SendStickerAsync(chatId, sticker);
            var caption = "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>";
            await client.SendTextMessageAsync(chatId, caption, parseMode: ParseMode.Html);
            return false;
        }
    }
}
