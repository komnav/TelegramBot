using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot;

class Program
{
    static void Main()
    {
        Host g4bot = new Host("8108866828:AAEWIXfwG0oU3aM-NmUtcSjZ0aZT23jG7Mg");
        g4bot.Start();
        g4bot.OnMessage += OnMessage;

        Console.ReadLine();
    }

    private static async void OnMessage(ITelegramBotClient client, Update update)
    {
        if (update.Message?.Text == "/start")
        {
            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 1891099619, "Welcome",
                replyToMessageId: update.Message?.MessageId);
        }
        else if (update.Message?.Text == "/help")
        {
            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 1891099619, "My command\n/start\n/help",
                replyToMessageId: update.Message?.MessageId);
        }
        else
        {
            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 8108866828, update.Message?.Text ?? "[no message]",
                replyToMessageId: update.Message?.MessageId);
        }
    }
}