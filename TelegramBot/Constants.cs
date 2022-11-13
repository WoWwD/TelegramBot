namespace TelegramBot.All_Paths
{
    class Constants
    {
        public const string mainUrl = "http://www.penzgtu.ru/";
        public const string mainUrlEvents = "http://www.penzgtu.ru";
        public const string instagramUrl = "https://www.instagram.com/penzgtu/";
        public const string vkontakteUrl = "https://vk.com/penzgtu";
        public const string youtubeUrl = "https://www.youtube.com/user/pstumedia";
        public const string facebookUrl = "https://www.facebook.com/penzgtu";
        public const string timeTableUrl = "http://tt.penzgtu.ru/";
        public const string timeTableLevelHigh = "http://tt.penzgtu.ru/site/level/high/";
        public const string timeTableLevelMiddle = "http://tt.penzgtu.ru/site/level/mid/";
        public const string screensPath = @"Data\RaspAndPromA\";
        public const string parsingTimeTable = ".//div[contains(@id, 'content')]/ul/li";
        public const string parsingNews = ".//div[contains(@id, 'middle')]/table/tr/td[contains(@id, 'col2')]//div[contains(@id, 'c7687')]//div[contains(@class, 'news_list_wrapper')]/table/tr/td";
    }
}