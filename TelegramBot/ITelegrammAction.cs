using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public interface ITelegrammAction
    {
        public string ActionKey { get; }

        public string ActionTitle { get; }

        public Task<bool> RunAction(ITelegramBotClient client, Update update, bool isCallback, long chatId);
    }
}
