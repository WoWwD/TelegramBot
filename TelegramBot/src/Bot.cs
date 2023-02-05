using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.NewsOfVuz;
using TelegramBot.Parsers;
using TelegramBot.Services;
using TelegramBot.UI;

namespace TelegramBot
{
    class Bot
    {
        Buttons buttons;
        TimeTableParser timeTableParser;
        ExamTimeTableParser examTimeTableParser;
        NewsParser newsParser;
        Texts texts;
        ScreenshotService screenshotService;
        static TelegramBotClient bot;
        private string token;

        public Bot(
            string token, 
            Buttons buttons, 
            TimeTableParser timeTableParser, 
            ExamTimeTableParser examTimeTableParser, 
            NewsParser newsParser, 
            Texts texts, 
            ScreenshotService screenshotService
        )
        {
            this.token = token;
            this.buttons = buttons;
            this.timeTableParser = timeTableParser;
            this.examTimeTableParser = examTimeTableParser;
            this.newsParser = newsParser;
            this.texts = texts;
            this.screenshotService = screenshotService;
        }
        public void GetUpdates()
        {
            try
            {
                int offset = 0;
                bot = new TelegramBotClient(token);
                var me = bot.GetMeAsync().Result;
                if (me != null && !string.IsNullOrEmpty(me.Username))
                {
                    while (true)
                    {
                        var updates = bot.GetUpdatesAsync(offset).Result;
                        if (updates != null && updates.Count() > 0)
                        {
                            foreach (var update in updates)
                            {
                                ProcessUpdate(update);
                                offset = update.Id + 1;
                            }
                        }
                        Thread.Sleep(300); //Время ответа бота
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        private async void ProcessUpdate(Update update)
        {
            try
            {
                if (update.Message == null) return;
                var message = update.Message.Text;
                long chatId = update.Message.Chat.Id;
                switch (update.Type)
                {
                    case UpdateType.Message:
                        if (message.Contains("/start"))
                        {
                            await bot.SendTextMessageAsync(
                                chatId, 
                                text: Texts.firstLaunch, 
                                replyMarkup: buttons.GetMenuButtons()
                            );
                            break;
                        }
                        if (message.Contains("Команды"))
                        {
                            await bot.SendTextMessageAsync(
                                chatId, 
                                text: texts.GetMenu(),  
                                replyMarkup: buttons.GetMenuButtons()
                            );
                            break;
                        }
                        if (message.Contains("/r"))
                        {
                            if (message.Length < 6 || message.Contains(" "))
                            {
                                await bot.SendTextMessageAsync(chatId, text: "Неверный формат. Пример ввода:\n/r19сн1с");
                                break;
                            }
                            await bot.SendTextMessageAsync(chatId, text: texts.Loading());
                            string linkGroupTimeTable = await Task.Run(
                                () => timeTableParser.GetLinkGroup(message, "Расписание занятий")
                            );
                            string linkGroupExamTimeTable = await Task.Run(
                                () => examTimeTableParser.GetLinkGroup(message, "Промежуточная аттестация")
                            );
                            if (linkGroupTimeTable != null && linkGroupExamTimeTable != null)
                            {
                                await GetScreenshotTimeTableAsync(chatId, linkGroupTimeTable);
                                await GetScreenshotExamTimeTableAsync(chatId, linkGroupExamTimeTable);
                                break;
                            }
                            if (linkGroupTimeTable != null && linkGroupExamTimeTable == null)
                            {
                                await GetScreenshotTimeTableAsync(chatId, linkGroupTimeTable);
                                await bot.SendTextMessageAsync(
                                    chatId, 
                                    text: "Расписание промежуточной аттестации недоступно"
                                );
                                break;
                            }
                            if (linkGroupTimeTable == null && linkGroupExamTimeTable != null)
                            {
                                await GetScreenshotExamTimeTableAsync(chatId, linkGroupExamTimeTable);
                                await bot.SendTextMessageAsync(chatId, text: "Расписание занятий недоступно");
                                break;
                            }
                            if (linkGroupTimeTable == null && linkGroupExamTimeTable == null)
                            {
                                await bot.SendTextMessageAsync(
                                    chatId, 
                                    text: "Расписание  недоступно или введённой группы не существует"
                                );
                                break;
                            }
                        }
                        if (message.Contains("/links"))
                        {
                            Message a = bot.SendTextMessageAsync(
                                chatId, 
                                "Социальные сети", 
                                replyMarkup: buttons.GetSocialNetworksButtons()
                            ).Result;
                            break;
                        }
                        if (message.Contains("/newsOfWeek"))
                        {
                            await bot.SendTextMessageAsync(chatId, text: texts.Loading());
                            await GetNewsAsync(chatId);
                            break;
                        }
                        goto default;
                    default:
                        await bot.SendTextMessageAsync(chatId, text: "Такой команды я не знаю :(");
                        break;
                }
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message); 
                await bot.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    text: "Что-то пошло не так, повторите ввод снова или перезапустите бота"
                ); 
            }
        }
        public async Task GetNewsAsync(long chatId)
        {
            await Task.Run(() => {
                List<string> news = newsParser.GetNews();
                if (news != null)
                {
                    news.Reverse();
                    bot.SendTextMessageAsync(
                        chatId, 
                        text: texts.GetNews(news), 
                        ParseMode.Html, 
                        disableWebPagePreview: true
                    );
                }
                else bot.SendTextMessageAsync(chatId, text: "Новостей за неделю пока нет");
            });
        }
        public async Task GetScreenshotTimeTableAsync(long chatId, string linkGroup)
        {
            await Task.Run(() =>
            {
                try
                {
                    FileStream screenshot = screenshotService.GetScreenshot(1280, 900, "timeTable", linkGroup);
                    var a = bot.SendPhotoAsync(
                        chatId, 
                        new Telegram.Bot.Types.InputFiles.InputOnlineFile(screenshot), 
                        caption: $"Расписание занятий"
                    ).Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Исключение: {ex.Message}");
                }
            });
        }
        public async Task GetScreenshotExamTimeTableAsync(long chatId, string linkGroup)
        {
            await Task.Run(() =>
            {
                try
                {
                    FileStream screenshot = screenshotService.GetScreenshot(1100, 900, "examTimeTable", linkGroup);
                    var a = bot.SendPhotoAsync(
                        chatId,
                        new Telegram.Bot.Types.InputFiles.InputOnlineFile(screenshot),
                        caption: $"Расписание промежуточной аттестации"
                    ).Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Исключение: {ex.Message}");
                }
            });
        }
    }
}