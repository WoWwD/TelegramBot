using TelegramBot.All_Paths;
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

            Bot bot = new Bot(Constants.botToken, buttons, timeTableParser, examTimeTableParser, newsParser, texts, screenshotService); 
            bot.GetUpdates();
        }
    }
}
