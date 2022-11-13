using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using TelegramBot.All_Paths;

namespace TelegramBot.NewsOfVuz
{
    class NewsParser
    {
        public List<string> news;
        public bool GetNews()
        {
            news = new List<string>();
            string urlFrm, nameNews, DayMonthYearInPars, result;
            int countWeek = (int)DateTime.Now.DayOfWeek, today = DateTime.Now.Day, dayInPars, firstWord, endWord;
            if (countWeek == 0)
            {
                countWeek = 6;
            }
            int startDayOfWeek = today - countWeek;
            HtmlWeb aa = new HtmlWeb();
            aa.OverrideEncoding = Encoding.UTF8;
            HtmlDocument n = aa.Load(Constants.mainUrlEvents);
            foreach (HtmlNode s in n.DocumentNode.SelectNodes(Constants.parsingNews))
            {
                firstWord = s.OuterHtml.IndexOf("<span>");
                endWord = s.OuterHtml.IndexOf("</span>", firstWord);
                dayInPars = Convert.ToInt32(
                    s.OuterHtml
                    .Substring(firstWord, endWord - firstWord + "<span>".Length)
                    .Split('>')[1]
                    .Split('&')[0]
                    .Replace(" ", "")
                );
                DayMonthYearInPars = s.OuterHtml
                    .Substring(firstWord, endWord - firstWord + "<span>".Length)
                    .Split('>')[1].Split('<')[0]
                    .Replace("&nbsp;", " ");
                if (dayInPars >= startDayOfWeek && dayInPars <= today)
                {
                    firstWord = s.OuterHtml.IndexOf("title");
                    endWord = s.OuterHtml.IndexOf("</a>", firstWord);
                    nameNews = s.OuterHtml
                        .Substring(firstWord, endWord - firstWord + "<title>".Length)
                        .Split('\"')[1]
                        .Split('\"')[0];
                    firstWord = s.OuterHtml.IndexOf("href");
                    endWord = s.OuterHtml.IndexOf("title", firstWord);
                    urlFrm = s.OuterHtml
                        .Substring(firstWord, endWord - firstWord + "href".Length)
                        .Split('\"')[1]
                        .Split('\"')[0];
                    result = $"{nameNews}" +
                        $"\nПодробнее =>{Constants.mainUrl}{urlFrm}" +
                        $"\nОпубликовано: {DayMonthYearInPars}г.";
                    news.Add(result);
                }
            }
            if (news.Count >= 1) return true;
            else return false;
        }
    }
}