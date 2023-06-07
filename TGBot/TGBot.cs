using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;



class Program
{
    private const string command0 = "/start";
    private const string command1 = "Что я делаю?";
    private const string command2 = "Узнать о компании";
    static void Main(string[] args)
    {
        var botClient = new TelegramBotClient("6013228378:AAE6POWPTmQgevv2bHdy1HjUK1O-8fNSmF8");

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
                    break;
            }
           
        }
        return;
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