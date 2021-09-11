using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Telegram;
using Telegram.Bot;
using static ConsoleTelegramGot.AsyncMethods;

namespace ConsoleTelegramGot
{
    class Program
    {
        static TelegramBotClient bot;
        static string FAQstr = "Данный бот представляет из себя подобие облачного хранилища.\n " +
            "Вы можете отправлять файлы и они будут загружены на сервер.\n" +
            "Командой: \"Скаченные файлы\" можно получить список сохраненных файлов\n" +
            "Командой \"Скачать файл: имя_файла\"  необходимого файла можно скачать файл";

        [Obsolete]
        static void Main(string[] args)
        {
            // Получить токен из файла
            string token = File.ReadAllText(@"teken.txt");
            // Получить список ранее загруженных файлов
            //files.AddRange(File.ReadAllText(@"filesList.txt").Split('\n'));
            #region Прокси
            //// Получить прокси
            //var proxy = new WebProxy()
            //{
            //    Address = new Uri(@"http://77.87.240.74:3128"),
            //    UseDefaultCredentials = false,
            //    //Credentials = new NetworkCredential(userName: "", password: "")
            //};
            //// добавляем HttpClientHandler на основе прокси
            //HttpClientHandler ch = new HttpClientHandler() { Proxy = proxy};

            //// создать httpClient на основе HttpClientHandler
            //HttpClient client = new HttpClient(ch);

            //// Создать телеграм бота на основе HttpClient и token
            //bot = new TelegramBotClient(token,client);
            #endregion
            // Создать телеграм бота на основе token
            bot = new TelegramBotClient(token);
            
            // Привязка метода к событию получения сообщений
            bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();

            Console.ReadKey();
        }

        [Obsolete]
        private static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            // Показать сообщение пользователя или его тип
            string str = $"{DateTime.Now.ToLongTimeString()} ";
            str += " MessageId: " + e.Message.MessageId;
            str += " Username: " + e.Message.From.Username;
            str += " Type: " + e.Message.Type;
            if(e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                str += " Text: " + e.Message.Text;
                DoIfTextMessage(e);
            }
            else
            // Сохранение голосовых сообщений
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Voice)
            {
                SaveVoiceMessage(e,bot);
            } else
            // Сохранение картинок
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
            {
                SavePhotoMessage(e, bot);
            } else
            // Сохранение документов
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                SaveDocumentMessage(e, bot);
                str += $" Document Name: {e.Message.Document.FileName}";
            }
            if (e.Message.Caption != null)
            {
                str += " Caption: " + e.Message.Caption;
            }
            Console.WriteLine(str);
        }
        /// <summary>
        /// Метод получения списка сохраненных файлов
        /// </summary>
        /// <returns></returns>
        static string GetSaveFilesNames()
        {
            string str = "";

            if (!File.Exists(@"teken.txt")) File.Create(@"teken.txt");
            FileInfo fI = new FileInfo(@"teken.txt");
            if(!File.Exists(@$"{fI.DirectoryName}\\files")) Directory.CreateDirectory(@$"{fI.DirectoryName}\\files");
            DirectoryInfo filesDI = new DirectoryInfo(@$"{fI.DirectoryName}\\files");
            var directoryes = filesDI.GetDirectories();
            foreach(var e in directoryes)
            {
                var files = e.GetFiles();
                foreach(var f in files)
                {
                    str += $"{f.Name}\n";
                }
            }

            return str;
        }
        /// <summary>
        /// Метод обработки текстровой строки
        /// </summary>
        /// <param name="e"></param>
        [Obsolete]
        static void DoIfTextMessage(Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Text == "/start")
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id, FAQstr);
            } else
            if (e.Message.Text == "Скаченные файлы")
            {
                bot.SendTextMessageAsync(e.Message.Chat.Id, GetSaveFilesNames());
            }
            else if (e.Message.Text.Contains("Скачать файл:"))
            {
                string filePath = ""; // Строка для хранения пути файла
                var fileName = e.Message.Text.Split(':'); // Получение имени файла из сообщения пользователя
                fileName[1] = fileName[1].Trim(); // Удаление пробелов в пути файла
                // Проверка на наличие файла в системе
                if (!GetSaveFilesNames().Contains($"{fileName[1]}"))
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Выбранный вами файл не найден");
                }else
                // Проверка папки, в которой будет лежать файл
                if (fileName[1].Contains("mp3")) {filePath = $"files\\audio\\{fileName[1]}";}
                else if (fileName[1].Contains("jpg")) {filePath = $"files\\photo\\{fileName[1]}";}
                else {filePath = $"files\\documents\\{fileName[1]}";}

                if (filePath != "") SendDocumentAsync(e, bot, filePath, fileName[1]);
            }
        }


    }
}
