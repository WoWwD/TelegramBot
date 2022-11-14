using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using TelegramBot.All_Paths;

namespace TelegramBot.NewsOfVuz
{
    class NewsParser
    {
        public List<string> GetNews()
        {
            List<string> news = new List<string>();
            string urlFrm, nameNews, DayMonthYearInPars, result;
            int countWeek = (int)DateTime.Now.DayOfWeek, today = DateTime.Now.Day, dayInPars, firstWord, endWord;
            if (countWeek == 0)
            {
                countWeek = 6;
            }
            int startDayOfWeek = today - countWeek;
            HtmlWeb htmlWeb = new HtmlWeb();
            htmlWeb.OverrideEncoding = Encoding.UTF8;
            HtmlDocument n = htmlWeb.Load(Constants.mainUrlEvents);
            foreach (HtmlNode htmlNode in n.DocumentNode.SelectNodes(Constants.parsingNews))
            {
                firstWord = htmlNode.OuterHtml.IndexOf("<span>");
                endWord = htmlNode.OuterHtml.IndexOf("</span>", firstWord);
                dayInPars = Convert.ToInt32(
                    htmlNode.OuterHtml
                    .Substring(firstWord, endWord - firstWord + "<span>".Length)
                    .Split('>')[1]
                    .Split('&')[0]
                    .Replace(" ", "")
                );
                DayMonthYearInPars = htmlNode.OuterHtml
                    .Substring(firstWord, endWord - firstWord + "<span>".Length)
                    .Split('>')[1].Split('<')[0]
                    .Replace("&nbsp;", " ");
                if (dayInPars >= startDayOfWeek && dayInPars <= today)
                {
                    firstWord = htmlNode.OuterHtml.IndexOf("title");
                    endWord = htmlNode.OuterHtml.IndexOf("</a>", firstWord);
                    nameNews = htmlNode.OuterHtml
                        .Substring(firstWord, endWord - firstWord + "<title>".Length)
                        .Split('\"')[1]
                        .Split('\"')[0];
                    firstWord = htmlNode.OuterHtml.IndexOf("href");
                    endWord = htmlNode.OuterHtml.IndexOf("title", firstWord);
                    urlFrm = htmlNode.OuterHtml
                        .Substring(firstWord, endWord - firstWord + "href".Length)
                        .Split('\"')[1]
                        .Split('\"')[0];
                    result = $"{nameNews}" +
                        $"\nПодробнее =>{Constants.mainUrl}{urlFrm}" +
                        $"\nОпубликовано: {DayMonthYearInPars}г.";
                    news.Add(result);
                }
            }
            if (news.Count >= 1) return news;
            else return null;
        }
    }
}