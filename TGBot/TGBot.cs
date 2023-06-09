using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
            string[] cmnds = new string[3] { "/start", "Что я делаю?", "Узнать о компании" }; // Список команд
            
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] // Устанавливаем клавиатуру
            {
    new KeyboardButton[]
        { "Что я делаю?", "Узнать о компании" }})
            {
                ResizeKeyboard = true
            };

            var message = update.Message;
            int? cmnd = null;
            foreach (string i in cmnds)
            {
                Regex rx = new Regex($"^{i}"); // Проверяем сообщение на наличие команд
                if (rx.IsMatch(update.Message.Text))
                {
                    cmnd = Array.IndexOf(cmnds, i);
                    break;
                }
            }

            switch (cmnd) // Обрабатываем команды
            {
                case 0: // /start
                    ShowText("Привет я бот для получения информации о компании по ИНН.", client, message); 
                    await client.SendTextMessageAsync(message.Chat.Id, text: "Выберете что делать дальше? ", replyMarkup: replyKeyboardMarkup);
                    break;

                case 1: // Что я делаю?
                    ShowText("Привет я бот для получения информации о компании по ИНН.", client, message);
                    break;

                case 2: // Узнать о компании
                    ShowText("Введите ИНН компании:", client, message);
                    break;

                default: // Обработка ИНН

                    Regex rx = new Regex(@"\d{13}|\d{15}|\d{10}|\d{12}"); 
                    if (rx.IsMatch(update.Message.Text)) // Проверяем ИНН на соответствие
                    {
                        ShowText("Поиск компании по ИНН...", client, message);
                        ShowText(GetApi(update.Message.Text), client, message);
                    }
                    else
                    {
                        ShowText("ИНН указан неверно.", client, message);
                    }
                    break;
            }
        }


        private static string GetApi(string INN)
        {
            var client = new HttpClient();

            Uri uri = new("https://basis.myseldon.com/api/rest/login");

            HttpContent content = new StringContent("UserName=test00590736@test.ru&Password=YeVgM61u"); 
            // Добавляем логин и пароль в body запроса

            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            // Устанавливаем кодировку

            var response = client.PostAsync(uri, content).Result;
            // Логинимся в API


            CookieContainer cookies = new CookieContainer();


            foreach (Cookie cookie in cookies.GetCookies(uri)) // Получаем куки из запроса
            {
                if (cookie.Name == "SessionGuid") // Устанавливаем полученные данные в заголовок зароса
                    client.DefaultRequestHeaders.Add("SessionGuid", cookie.Value);
                if (cookie.Name == "LoginMyseldon")
                    client.DefaultRequestHeaders.Add("LoginMyseldon", cookie.Value);
            }

            response = client.GetAsync($"https://basis.myseldon.com/api/rest/find_company?Inn={INN}").Result; // Делаем запрос к API
            var json = response.Content.ReadAsStringAsync().Result; 


            var root = JsonConvert.DeserializeObject<Root>(json); // Производим десериализацию полученного json

            if (root.status.itemsFound == 0) // Если ничего не найдено, возвращаем сообщение об ошибке
                return "Компания с таким ИНН не найдена";


            string msg = "";

        
            client.GetAsync("https://basis.myseldon.com/api/rest/logout"); // Выходим из API

            msg += "Коды: \n"; // Формируем сообщение
            msg += $"   ОГРН - {root.companies_list[0].basic.ogrn} \n";
            msg += $"   ИНН - {root.companies_list[0].basic.ogrn} \n \n";
            msg += "Наименовения:  \n";
            msg += $"   Полное - {root.companies_list[0].basic.fullName} \n";
            msg += $"   Сокращенное - {root.companies_list[0].basic.shortName}\n";
            msg += $"Тел - {root.companies_list[0].phoneFormattedList[0].number}\n \n";
            msg += $"Адрес - {root.companies_list[0].address}\n";

            Console.WriteLine(msg);
            return msg;
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

        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }
    }

}