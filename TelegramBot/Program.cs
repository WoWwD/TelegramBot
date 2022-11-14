using System;
using TelegramBot.NewsOfVuz;
using TelegramBot.Parsers;
using TelegramBot.Services;
using TelegramBot.UI;

namespace TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Buttons buttons = new Buttons();
            TimeTableParser timeTableParser = new TimeTableParser();
            ExamTimeTableParser examTimeTableParser = new ExamTimeTableParser();
            NewsParser newsParser = new NewsParser();
            Texts texts = new Texts();
            ScreenshotService screenshotService = new ScreenshotService();
            string token = "1674698636:AAHjunKIirrDECO0xHe3f72UvbogU25T6AM";

            Console.WriteLine("Бот в Telegram - @mmfrtbot");
            Bot bot = new Bot(token, buttons, timeTableParser, examTimeTableParser, newsParser, texts, screenshotService); 
            bot.GetUpdates();
        }
    }
}
