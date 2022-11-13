using Freezer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.All_Paths;
using TelegramBot.NewsOfVuz;
using TelegramBot.Schedule_of_groups;
using TelegramBot.UI;

namespace TelegramBot
{
    class Bot
    {
        Buttons buttons = new Buttons();
        static TelegramBotClient bot;
        private string Token;
        public Bot(string Token)
        {
            this.Token = Token;
        }
        public void GetUpdates()
        {
            try
            {
                int offset = 0;
                bot = new TelegramBotClient(Token);
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
                        Thread.Sleep(500); //Время ответа бота
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
                var msg = update.Message.Text;
                var upm = update.Message;
                switch (update.Type)
                {
                    case UpdateType.Message:
                        string message = $"{DateTime.Now}:  {upm.Chat.FirstName}  {upm.Chat.Id}  Message: {msg}";
                        //System.IO.File.AppendAllText(@"Data\messages.txt", $"{message}\n");
                        Console.WriteLine(message);
                        if (msg.Contains("/start"))
                        {
                            await bot.SendTextMessageAsync(upm.Chat.Id, text: "Привет!\nЧтобы начать пользоваться ботом, необходимо нажать кнопку \"Команды\", " +
                                "которая появилась под полем для ввода сообщений.", replyMarkup: buttons.GetMenuButtons());
                            break;
                        }
                        if (msg.Contains("Команды"))
                        {
                            await bot.SendTextMessageAsync(upm.Chat.Id, text: "Список доступных команд:\n\n" +
                                $"{char.ConvertFromUtf32(0x1F4DA)}  /r[группа] - Расписание группы\n\n" +
                                $"{char.ConvertFromUtf32(0x1F4F0)}  /newsOfWeek - Получить новости за текущую неделю\n\n" +
                                $"{char.ConvertFromUtf32(0x1F3EB)}  /links - Ссылки на социальные сети ПензГТУ\n\n", replyMarkup: buttons.GetMenuButtons());
                            break;
                        }
                        if (msg.Contains("/r"))
                        {
                            if (msg.Length < 6 || msg.Contains(" "))
                            {
                                await bot.SendTextMessageAsync(update.Message.Chat.Id, text: "Неверный формат. Пример ввода:\n/r19сн1с");
                                break;
                            }
                            await bot.SendTextMessageAsync(update.Message.Chat.Id, text: $"{char.ConvertFromUtf32(0x23F3)} Пожалуйста, подождите {char.ConvertFromUtf32(0x23F3)}");
                            var rasp = GetResRaspAsync(update);
                            var promA = GetResPromAAsync(update);
                            if (rasp.Result == true && promA.Result == true)
                            {
                                GetScrRaspAsync(update);
                                GetScrPromAAsync(update);
                                break;
                            }
                            if (rasp.Result == true && promA.Result == false)
                            {
                                GetScrRaspAsync(update);
                                await bot.SendTextMessageAsync(update.Message.Chat.Id, text: "Расписание промежуточной аттестации недоступно");
                                break;
                            }
                            if (rasp.Result == false && promA.Result == true)
                            {
                                GetScrPromAAsync(update);
                                await bot.SendTextMessageAsync(update.Message.Chat.Id, text: "Расписание занятий недоступно");
                                break;
                            }
                            if (rasp.Result == false && promA.Result == false)
                            {
                                await bot.SendTextMessageAsync(update.Message.Chat.Id, text: "Расписание занятий и промежуточной аттестации недоступно или введённой группы не существует");
                                break;
                            }
                        }
                        if (msg.Contains("/links"))
                        {
                            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            {
                        new[]
                        {
                             InlineKeyboardButton.WithUrl($"{char.ConvertFromUtf32(0x1F535)}  VK", Constants.vkontakteUrl)
                        },
                        new[]
                        {
                             InlineKeyboardButton.WithUrl($"{char.ConvertFromUtf32(0x26AB)}  Instagram", Constants.instagramUrl)
                        },
                        new[]
                        {
                             InlineKeyboardButton.WithUrl($"{char.ConvertFromUtf32(0x26AA)}  Facebook", Constants.facebookUrl)
                        },
                        new[]
                        {
                             InlineKeyboardButton.WithUrl($"{char.ConvertFromUtf32(0x1F534)}  YouTube", Constants.youtubeUrl)
                        }
                        });
                            var a = bot.SendTextMessageAsync(update.Message.Chat.Id, "Социальные сети", replyMarkup: inlineKeyboard).Result;
                            break;
                        }
                        if (msg.Contains("/newsOfWeek"))
                        {
                            GetResEventsAsync(update);
                            break;
                        }
                        goto default;
                    default:
                        await bot.SendTextMessageAsync(upm.Chat.Id, text: "Такой команды я не знаю :(");
                        break;
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); await bot.SendTextMessageAsync(update.Message.Chat.Id, text: "Что-то пошло не так, повторите ввод снова"); }
        }
        public static async Task<bool> GetResRaspAsync(Update update)
        {
            var rasp = await Task.Run(() => TimeTableParser.GetResRasp(update));
            return rasp;
        }
        public static async Task<bool> GetResPromAAsync(Update update)
        {
            var promA = await Task.Run(() => ExamTimeTableParser.GetResPromA(update));
            return promA;
        }
        public static async Task GetResEventsAsync(Update update)
        {
          await Task.Run(() => {
            NewsParser n = new NewsParser();
            var res = n.GetNews();
            if (res == true)
            {
                n.news.Reverse();
                bot.SendTextMessageAsync(update.Message.Chat.Id, text: $"{char.ConvertFromUtf32(0x2757)}<b>Новости недели</b>{char.ConvertFromUtf32(0x2757)}\n\n{string.Join("\n\n", n.news)}", ParseMode.Html, disableWebPagePreview: true);
            }
            else
            {
                bot.SendTextMessageAsync(update.Message.Chat.Id, text: "Новостей за неделю пока нет");
            }
          });
        }
        public static async Task GetScrRaspAsync(Update update)
        {
            await Task.Run(() =>
            {
                try
                {
                    Guid guid = Guid.NewGuid();
                    var screenshotJob1 = ScreenshotJobBuilder.Create(TimeTableParser.linkGroup).SetBrowserSize(1280, 900).SetCaptureZone(CaptureZone.FullPage).SetTrigger(new WindowLoadTrigger());
                    System.IO.File.WriteAllBytes($"{guid}Rasp.png", screenshotJob1.Freeze());
                    string[] findFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, $"{guid}Rasp.png", SearchOption.AllDirectories);
                    foreach (string file in findFiles)
                    {
                        var q = file;
                        using (var str1 = System.IO.File.OpenRead(q))
                        {
                            var a = bot.SendPhotoAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(str1), caption: $"Расписание занятий").Result;
                            str1.Close();
                            System.IO.File.Delete(q);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Исключение: {ex.Message}");
                }
            });
        }
        public static async Task GetScrPromAAsync(Update update)
        {
            await Task.Run(() =>
            {
                try
                {
                    Guid guid = Guid.NewGuid();
                    var screenshotJob2 = ScreenshotJobBuilder.Create(ExamTimeTableParser.linkGroupPromA).SetBrowserSize(1100, 900).SetCaptureZone(CaptureZone.FullPage).SetTrigger(new WindowLoadTrigger());
                    System.IO.File.WriteAllBytes($"{guid}PromA.png", screenshotJob2.Freeze());
                    string[] findFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, $"{guid}PromA.png", SearchOption.AllDirectories);
                    foreach (string file in findFiles)
                    {
                        var q = file;
                        using (var str2 = System.IO.File.OpenRead(q))
                        {
                            var a = bot.SendPhotoAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(str2), caption: $"Расписание промежуточной аттестации").Result;
                            str2.Close();
                            System.IO.File.Delete(q);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Исключение: {ex.Message}");
                }
            });
        }
    }
}