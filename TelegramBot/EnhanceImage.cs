using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;


namespace TelegramBot
{
    public class EnhanceImage : ITelegrammAction
    {
        public string ActionKey => "image_clicked";

        public string ActionTitle => "Image quality enhancement";

        public async Task<bool> RunAction(ITelegramBotClient client, Update update, bool isCallback, long chatId)
        {
            var message = update.Message;
            if (message?.Document != null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Now, wait, I'll process the document...");

                var fileId = update.Message.Document.FileId;
                var fileInfo = await client.GetFileAsync(fileId);
                var filePath = fileInfo.FilePath;

                string foldeerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Telegram Bot");
                string destinationFilePath = Path.Combine(foldeerPath, message.Document.FileName);
                await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                await client.DownloadFileAsync(filePath, fileStream);
                fileStream.Close();

                if (message.Document.MimeType.StartsWith("image/"))
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Document is an image, processing...");

                    using (Image image = Image.Load(destinationFilePath))
                    {
                        image.Mutate(x => x.Resize(image.Width * 2, image.Height * 2));

                        string folderBot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Telegram Bot");
                        string newFilePath = Path.Combine(folderBot, message.Document.FileName);

                        image.Save(newFilePath, new JpegEncoder { Quality = 100 });

                        await using FileStream processedFileStream = System.IO.File.OpenRead(newFilePath);
                        await client.SendDocumentAsync
                        (chatId: message.Chat.Id, document: new InputOnlineFile(processedFileStream, $"{message.Document.FileName}"));

                    }
                }
                else
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "This document is not an image.");
                }
                return false;
            }
            return false;
        }
    }
}
