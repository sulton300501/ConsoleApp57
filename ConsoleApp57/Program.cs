using System;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using TelegramBotExample;

namespace ConsoleApp57;

class Program
{
    static async Task Main(string[] args)
    {

        const string token = "6763712956:AAG1_IajuxXb3IEFeorlwDAjk5NKkxjDm3Q";

        TelegramBotClass botClass = new TelegramBotClass(token);
       try
        {
            await botClass.BotHandle();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

     

    }
}

