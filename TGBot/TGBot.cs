using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace TGBot;
class Program
{
    private const string command0 = "/start";
    private const string command1 = "Что я делаю?";
    private const string command2 = "Узнать о компании";
    static void Main(string[] args)
    {
        var botClient = new TelegramBotClient("6078887309:AAHTzIa85EYBie9vmb9w3fm740T5ecH9Pyk");

        botClient.StartReceiving(Update, Error);

        Console.ReadLine();
    }


    async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {

        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
        {
            new KeyboardButton[]
                { "Что я делаю?", "Узнать о компании" }})
        {
            ResizeKeyboard = true
        };

        var message = update.Message;
        if (message != null )
        {

            switch(message.Text)
            {
                case command0:
                    showText("Привет я бот для получения информации о компании по ИНН.",client,message);
                    await client.SendTextMessageAsync(message.Chat.Id, text: "Выберете что делать дальше? ", replyMarkup: replyKeyboardMarkup);
                    break;
                case command1:
                    showText("Привет я бот для получения информации о компании по ИНН.", client, message);
                    break;
                case command2:
                    showText("Введите инн компании", client, message);
                    break;
                default:
                    await client.SendTextMessageAsync(message.Chat.Id, text: "Выберете что делать дальше? ", replyMarkup: replyKeyboardMarkup);
                }
                else
                {
                    ShowText(msg, client, message);
                }
                
                break;
        }
    }


    private static  string GetApi(string INN)
    {
        var client = new HttpClient();

        Uri uri = new("https://basis.myseldon.com/api/rest/login");

        HttpContent content = new StringContent("UserName=test00590736@test.ru&Password=YeVgM61u");

        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

        var response = client.PostAsync(uri, content).Result;

        CookieContainer cookies = new CookieContainer();


        foreach (var cookieHeader in response.Headers.GetValues("Set-Cookie"))
        {

            try
            {
                cookies.SetCookies(uri, cookieHeader);
            }
            catch
            {

            }

        }


        foreach (Cookie cookie in cookies.GetCookies(uri))
        {
            if (cookie.Name == "SessionGuid")
                client.DefaultRequestHeaders.Add("SessionGuid", cookie.Value);
            if (cookie.Name == "LoginMyseldon")
                client.DefaultRequestHeaders.Add("LoginMyseldon", cookie.Value);
        }

        response = client.GetAsync("https://basis.myseldon.com/api/rest/find_company?Inn=7736050003").Result;
        var json = response.Content.ReadAsStringAsync().Result;


        var root = JsonConvert.DeserializeObject<Root>(json);

        string msg = "";

        
       msg += $"ОГРН - {root.companies_list[0].basic.ogrn}\n";
        msg +=   $"ИНН - {root.companies_list[0].basic.ogrn}\n";
        msg += $" Н аименовение - {root.cmpanies_list[0].basic.ogrn}\n";
        msg += $"ОГ.companies_list} ic.ogrn}\n";
        msg += $"ОГРН - {root.companies_list[0].basic.ogrn}\n";
        msg += $"ОГРН - {root.companies_list[0].basic.ogrn}\n";
        client.GetAsync("https://basis.myseldon.com/api/rest/logout");

        return msg;
    }

    async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {
        if (update.Message is not { } message)
            return;
        if (update.Message == null)
            return;

        Task.Run(() => HandleCommands(client, update, token));
    }

    async static void showText(string text, ITelegramBotClient client, Message message ) 
    {
        await client.SendTextMessageAsync(message.Chat.Id, text);
    }


    private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }


}