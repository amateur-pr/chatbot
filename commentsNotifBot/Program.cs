using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading;using Telegram.Bot.Examples.Polling;
using Telegram.Bot.Extensions.Polling;



namespace commentsNotifBot
{
    class Program
    {
        private static string token { get; set; } = "5545931362:AAFe08KgSSHdcNZaKYavw5xwq4u-5_cybcs";
        private static TelegramBotClient Bot;
        static void Main(string[] args)
        {
          
            Bot = new TelegramBotClient(token);
            using var cts = new CancellationTokenSource();
            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
            Bot.StartReceiving(Handlers.HandleUpdateAsync,
                               Handlers.HandleErrorAsync,                           
                               receiverOptions,
                               cts.Token); ;
            Console.WriteLine($"Bot ist gestartet...");
            Console.ReadLine();
            cts.Cancel();
        }
    }
}
