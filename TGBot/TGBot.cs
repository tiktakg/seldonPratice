using System.Text.RegularExpressions;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;



class Program
{
#pragma warning disable 1998
#pragma warning disable 4014

    static void Main(string[] args)
    {
        var botClient = new TelegramBotClient("6013228378:AAE6POWPTmQgevv2bHdy1HjUK1O-8fNSmF8");

        botClient.StartReceiving(Update, Error);

        Console.ReadLine();
    }

    static private async void HandleCommands(ITelegramBotClient client, Update update, CancellationToken token)
    {
        string[] cmnds = new string[3] { "/start", "Что я делаю?", "Узнать о компании" };
        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
        {
            new KeyboardButton[]
                { "Что я делаю?", "Узнать о компании" }})
        {
            ResizeKeyboard = true
        };


        int? cmnd = null;
        foreach (string i in cmnds)
        {
            Regex rx = new Regex($"^{i}");
            if (rx.IsMatch(update.Message.Text))
            {
                cmnd = Array.IndexOf(cmnds, i);
                break;
            }
        }
        if (cmnd == null)
        {
            return;
        }

        var message = update.Message;
        switch (cmnd)
        {
            case 0:
                ShowText("Привет я бот для получения информации о компании по ИНН.", client, message);
                await client.SendTextMessageAsync(message.Chat.Id, text: "Выберете что делать дальше? ", replyMarkup: replyKeyboardMarkup);
                break;

            case 1:
                ShowText("Привет я бот для получения информации о компании по ИНН.", client, message);
                break;

            case 2:
                ShowText("Введите инн компании", client, message);
                break;

            default:
                string msg = GetApi(message.Text);
                if (msg == null)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, text: "Выберете что делать дальше? ", replyMarkup: replyKeyboardMarkup);
                }
                else
                {
                    ShowText(msg, client, message);
                }
                
                break;
        }
    }


    private static string GetApi(string INN)
    {
        // Типа Код
        return null; 
    }

    async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {
        if (update.Message is not { } message)
            return;
        if (update.Message == null)
            return;

        Task.Run(() => HandleCommands(client, update, token));
    }


    async static void ShowText(string text, ITelegramBotClient client, Message message ) 
    {
        await client.SendTextMessageAsync(message.Chat.Id, text);
    }


    private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }


}