using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class TelegrammApp
    {
        private static ConcurrentDictionary<long, ITelegrammAction> _callBackActions = [];

        public Action<ITelegramBotClient, Update>? OnMessage;
        private readonly TelegramBotClient _bot;
        private readonly Dictionary<string, ITelegrammAction> _telegrammActions;

        public TelegrammApp(
            IConfiguration configuration,
            IEnumerable<ITelegrammAction> telegrammActions)
        {
            _bot = new TelegramBotClient(configuration["TelegrammKey"]);
            _telegrammActions = telegrammActions.ToDictionary(s => s.ActionKey);
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
            var chatId = GetChatId(update);

            if (chatId.HasValue && _callBackActions.TryGetValue(chatId.Value, out ITelegrammAction telegrammAction))
            {
                var hasCallBack = await telegrammAction.RunAction(client, update, true, chatId.Value);

                if (!hasCallBack)
                {
                    _callBackActions.Remove(chatId.Value, out _);
                }
            }
            else if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
            {
                await ShowOurActionItemsAsync(client, chatId.Value, token);
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                var callbackQuery = update.CallbackQuery;
                if (_telegrammActions.TryGetValue(callbackQuery.Data, out telegrammAction))
                {
                    var hasCallBack = await telegrammAction.RunAction(client, update, false, chatId.Value);
                    if (hasCallBack)
                    {
                        _callBackActions.TryAdd(chatId.Value, telegrammAction);
                    }
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, "Not the right move");
                }
            }
        }

        private long? GetChatId(Update update)
        {
            var chatId = update.Message?.Chat?.Id;
            if (!chatId.HasValue)
            {
                chatId = update.CallbackQuery?.Message?.Chat?.Id;
            }
            return chatId;
        }

        private async Task ShowOurActionItemsAsync(ITelegramBotClient client, long chatId, CancellationToken token)
        {
            List<List<InlineKeyboardButton>> inlineKeyboardButtons = [];
            foreach (var telegramAction in _telegrammActions.Values)
            {
                inlineKeyboardButtons.Add([InlineKeyboardButton.WithCallbackData(telegramAction.ActionTitle, telegramAction.ActionKey)]);
            }
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(inlineKeyboardButtons);

            await client.SendTextMessageAsync(chatId, "Welcom to Our service", replyMarkup: keyboard, cancellationToken: token);
        }
    }
}
