using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace TGBot
{
    class Program
    {
#pragma warning disable 1998
#pragma warning disable 4014

        static void Main(string[] args)
        {
            var botClient = new TelegramBotClient("6078887309:AAHTzIa85EYBie9vmb9w3fm740T5ecH9Pyk");

            botClient.StartReceiving(Update, Error);

            Console.ReadLine();
        }

        static private async void HandleCommands(ITelegramBotClient client, Update update, CancellationToken token)
        {
            string[] cmnds = new string[3] { "/start", "Чтояделаю?", "Узнатьокомпании" }; // Список команд
            int? cmnd = null;
            var message = update.Message;
            string msg = update.Message.Text.Replace(" ", "");

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] // Устанавливаем клавиатуру
            { new KeyboardButton[] {"Что я делаю?", "Узнать о компании" }})
            { ResizeKeyboard = true };

            foreach (string i in cmnds)
                if (new Regex($"^{i}").IsMatch(msg))  // Проверяем сообщение на наличие команд
                {
                    cmnd = Array.IndexOf(cmnds, i);
                    break;
                }

            switch (cmnd) // Обрабатываем команды
            {
                case 0 or 1: // /start
                    ShowText("Привет я бот для получения информации о компании по ИНН.", client, message);
                    await client.SendTextMessageAsync(message.Chat.Id, text: "Выберете что делать дальше? ", replyMarkup: replyKeyboardMarkup);
                    break;
                case 2: // Узнать о компании
                    ShowText("Введите ИНН компании:", client, message);
                    break;
                default: // Обработка ИНН
                    if (new Regex(@"\d{13}|\d{15}|\d{10}|\d{12}").IsMatch(msg)) // Проверяем ИНН на соответствие
                    {
                        ShowText("Поиск компании по ИНН...", client, message);
                        ShowText(GetApiPdf(msg, client, message), client, message);
                        
                    }
                    else
                        ShowText("ИНН указан неверно.", client, message);

                    await client.SendTextMessageAsync(message.Chat.Id, text: "Выберете что делать дальше? ", replyMarkup: replyKeyboardMarkup);
                    break;
            }
        }

        private static string GetApiPdf(string INN, ITelegramBotClient TGclient, Message message)
        {
            var client = new HttpClient();

            Uri uri = new("https://basis.myseldon.com/api/rest/login");
            CookieContainer cookies = new CookieContainer();
            HttpContent content = new StringContent("UserName=test00590736@test.ru&Password=YeVgM61u"); // Добавляем логин и пароль в body запроса
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded"); // Устанавливаем кодировку

            Root root;

            var response = client.PostAsync(uri, content).Result; // Логинимся в API
            string res;

            foreach (Cookie cookie in cookies.GetCookies(uri)) // Получаем куки из запроса
            {
                if (cookie.Name == "SessionGuid") // Устанавливаем полученные данные в заголовок зароса
                    client.DefaultRequestHeaders.Add("SessionGuid", cookie.Value);
                if (cookie.Name == "LoginMyseldon")
                    client.DefaultRequestHeaders.Add("LoginMyseldon", cookie.Value);
            }

            response = client.GetAsync($"https://basis.myseldon.com/api/rest/find_company?Inn={INN}").Result; // Делаем запрос к API

            root = JsonConvert.DeserializeObject<Root>(response.Content.ReadAsStringAsync().Result); // Производим десериализацию полученного json

            if (root.status.itemsFound == 0) // Если ничего не найдено, возвращаем сообщение об ошибке
                return "Компания с таким ИНН не найдена";


            res = client.GetAsync($"https://basis.myseldon.com/api/rest/order_excerpt_pdf?ogrn={root.companies_list[0].basic.ogrn}").Result.Content.ReadAsStringAsync().Result;
            root = JsonConvert.DeserializeObject<Root>(res);

            if (root.status.methodStatus == "Error")
                return "Компания с таким ИННне найдена." + root.status.name;

            while (true)
            {
                Thread.Sleep(5000);
                res = client.GetAsync($"https://basis.myseldon.com/api/rest/get_excerpt_pdf?orderNum={root.orderNum}").Result.Content.ReadAsStringAsync().Result;

                root = JsonConvert.DeserializeObject<Root>(res);

                if (root.excerpt_body != null)
                    break;
            }

            if (System.IO.File.Exists("company.pdf"))
                System.IO.File.Delete("company.pdf");

            System.IO.File.WriteAllBytes("company.pdf", Convert.FromBase64String(root.excerpt_body));
            ShowFile("company.pdf", TGclient, message);

            Thread.Sleep(3000);
            return "Готово!";
        }

        async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message is not { } message) // Проверка на сообщения на наличие текста
                return;
            if (update.Message.Text == null)
                return;

            Console.WriteLine(update.Message.Text);
            Task.Run(() => HandleCommands(client, update, token));
        }

        async static void ShowText(string text, ITelegramBotClient client, Message message)
        {
            await client.SendTextMessageAsync(message.Chat.Id, text);
        }

        async static void ShowFile(string fileName, ITelegramBotClient client, Message message)
        {
            await using Stream file = System.IO.File.OpenRead(fileName);
            await client.SendDocumentAsync(message.Chat.Id, InputFile.FromStream(file, fileName));
            file.Close();
        }

        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }
    }
}