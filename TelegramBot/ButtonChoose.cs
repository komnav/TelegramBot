using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class ButtonChoose
    {
        private readonly SendSticker _sendSticker;
        private readonly ImageConverter _imageConverter;
        private readonly EnhanceImage _enhanceImage;
        private readonly FindMusic _findMusic;
        private readonly FindVidioInYoutube _findVidioInYoutube;
        private TelegramBotClient _bot;
        public ButtonChoose(SendSticker sendSticker, ImageConverter imageConverter, EnhanceImage enhanceImage,
            FindMusic findMusic, FindVidioInYoutube findVidioInYoutube, string token)
        {
            _sendSticker = sendSticker;
            _imageConverter = imageConverter;
            _enhanceImage = enhanceImage;
            _findMusic = findMusic;
            _findVidioInYoutube = findVidioInYoutube;
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
            throw new NotImplementedException();
        }
    }
}
