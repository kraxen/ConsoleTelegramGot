using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace ConsoleTelegramGot
{
    public static class AsyncMethods
    {
        /// <summary>
        /// Сохраняет файл, если его тип был Voice
        /// </summary>
        /// <param name="e">сообщение с типом Voice</param>
        /// <param name="startPath">стартовый path</param>
        [Obsolete]
        public static async void SaveVoiceMessage(Telegram.Bot.Args.MessageEventArgs e, TelegramBotClient bot)
        {
            if (!Directory.Exists($"files\\audio")) Directory.CreateDirectory($"files\\audio");
            string path = $"files\\audio\\{e.Message.Voice.FileUniqueId}.mp3";
            var file = await bot.GetFileAsync(e.Message.Voice.FileId);
            FileStream fs = new FileStream(path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            Console.WriteLine("Файл успешно загружен");
            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Файл успешно загружен");
        }

        /// <summary>
        /// Сохраняет файл, если его тип Photo
        /// </summary>
        /// <param name="e"></param>
        [Obsolete]
        public static async void SavePhotoMessage(Telegram.Bot.Args.MessageEventArgs e, TelegramBotClient bot)
        {
            if (!Directory.Exists($"files\\photo")) Directory.CreateDirectory($"files\\photo");
            string path = $"files\\photo\\{e.Message.Photo[2].FileUniqueId}.jpg";
            var file = await bot.GetFileAsync(e.Message.Photo[2].FileId);
            FileStream fs = new FileStream(path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            Console.WriteLine("Файл успешно загружен");
            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Файл успешно загружен");
        }
        /// <summary>
        /// Сохранение документа
        /// </summary>
        /// <param name="e"></param>
        [Obsolete]
        public static async void SaveDocumentMessage(Telegram.Bot.Args.MessageEventArgs e, TelegramBotClient bot)
        {
            if (!Directory.Exists($"files\\documents")) Directory.CreateDirectory($"files\\documents");
            string path = $"files\\documents\\{e.Message.Document.FileName}";
            var file = await bot.GetFileAsync(e.Message.Document.FileId);
            FileStream fs = new FileStream(path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            Console.WriteLine("Файл успешно загружен");
            await bot.SendTextMessageAsync(e.Message.Chat.Id, "Файл успешно загружен");
        }
        /// <summary>
        /// Метод для отправки файла пользователю
        /// </summary>
        /// <param name="e">сообщение пользователя</param>
        /// <param name="bot">бот, который отправит сообщение</param>
        /// <param name="path">путь к файлу на компьютере</param>
        [Obsolete]
        public static async void SendDocumentAsync(Telegram.Bot.Args.MessageEventArgs e, TelegramBotClient bot, string path, string name)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                Telegram.Bot.Types.InputFiles.InputOnlineFile iof = new Telegram.Bot.Types.InputFiles.InputOnlineFile(fs);
                Telegram.Bot.Types.InputFiles.InputTelegramFile itf = new Telegram.Bot.Types.InputFiles.InputTelegramFile(fs);
                iof.FileName = name;
                await bot.SendDocumentAsync(e.Message.Chat.Id, iof, "Сообщение");
                fs.Close();
                Console.WriteLine($"Файл {name} успешно отправлен пользователю {e.Message.Chat.FirstName}");
            }
        }
    }
}
