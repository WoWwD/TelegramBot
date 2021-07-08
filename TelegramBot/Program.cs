namespace TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot("1674698636:AAHjunKIirrDECO0xHe3f72UvbogU25T6AM"); 
            bot.GetUpdates();
        }
    }
}
