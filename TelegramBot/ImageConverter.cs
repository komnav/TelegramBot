using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;


namespace TelegramBot
{
    public class ImageConverter : ITelegrammAction
    {
        public string ActionKey => ".jpg_clicked";

        public string ActionTitle => "Convert photo to .jpg";

        public async Task<bool> RunAction(ITelegramBotClient client, Update update, bool isCallBack, long chatId)
        {
            if (!isCallBack)
            {
                await client.SendTextMessageAsync(chatId, "Send me photo");
                return true;
            }
            else
            {
                var message = update.Message;
                if (message?.Photo != null && message.Photo.Any())
                {
                    await client.SendTextMessageAsync(chatId, "Now, wait, I'll convert the photo to .jpg");

                    var fileId = update.Message?.Photo.Last().FileId;
                    var fileInfo = await client.GetFileAsync(fileId);
                    var filePath = fileInfo.FilePath;

                    string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Telegram Bot");
                    string destinationFilePath = Path.Combine(folderPath, message.Document?.FileName ?? $@"{message.MessageId}.jpg");
                    await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                    await client.DownloadFileAsync(filePath, fileStream);
                    fileStream.Close();

                    using (Image image = Image.Load(destinationFilePath))
                    {
                        image.Save(destinationFilePath, new JpegEncoder());
                    }
                    await using FileStream convertedFileStream = System.IO.File.OpenRead(destinationFilePath);
                    await client.SendDocumentAsync
                        (chatId: message.Chat.Id, document: new InputOnlineFile(convertedFileStream, $"{message.MessageId}.jpg"));
                }
                else
                {
                    await client.SendTextMessageAsync(update.Message?.Chat.Id, "No photo found. Please send a valid photo.");
                }
            }

            return false;
        }
    }
}
