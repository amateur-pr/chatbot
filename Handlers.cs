using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Examples.Polling
{

    public class Handlers
    {

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }



        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var massage = update.Message;
            if (massage.Text != null)
            {
                if (massage.Text.ToLower().Contains("wie gehts?"))
                {
                    await botClient.SendTextMessageAsync(massage.Chat.Id, "Gut, und dir?");
                    return;
                }
                if (massage.Text.ToLower().Contains("wie heißt du?"))
                {
                    await botClient.SendTextMessageAsync(massage.Chat.Id, "Erraten!");
                    return;
                }
                if (massage.Text.ToLower().Contains("tom"))
                {
                    await botClient.SendTextMessageAsync(massage.Chat.Id, "Jaaaa Cool!");
                    return;
                }
                if (massage.Text.ToLower().Contains("wer dich gemacht hat?"))
                {
                    await botClient.SendTextMessageAsync(massage.Chat.Id, "Gruppe (Sarah, Garvin, Mansour, Sergey)");
                    return;
                }
                if (massage.Text.ToLower().Contains("gut"))
                {
                    await botClient.SendTextMessageAsync(massage.Chat.Id, "Möchtest du reden?");
                    return;
                }
                if (massage.Text.ToLower().Contains("hallo"))
                {
                    await botClient.SendTextMessageAsync(massage.Chat.Id, "Moin ;)");
                    return;
                }
               
            }
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
                UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery!),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult!),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        public static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            List<string> lstSpam = new List<string>();
            lstSpam.Add("http:");
            lstSpam.Add("https:");
            lstSpam.Add("www.");
            lstSpam.Add("www");

            Console.WriteLine($"Receive message type: {message.Type}");
            if (message.Type != MessageType.Text)
                return;

            bool prov = lstSpam.Any(s => message.Text.ToLower().Contains(s));

            if (prov)

                if (prov)
                    await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
        }

    
        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}");

            await botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Received {callbackQuery.Data}");
        }

        private static async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
        {
            Console.WriteLine($"Received inline query from: {inlineQuery.From.Id}");

            InlineQueryResult[] results = {
         
            new InlineQueryResultArticle(
                id: "3",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent(
                    "hello"
                )
            )
        };

            await botClient.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
                                                   results: results,
                                                   isPersonal: true,
                                                   cacheTime: 0);
        }

        private static Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
    }
}