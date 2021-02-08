using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot.NewsOfVuz
{
    class News: IDisposable
    {
        public List<string> news = new List<string>();
        public bool GetNews()
        {
            string urlFrm, nameEvent, DayMonthYearInPars, result;
            int countWeek = (int)DateTime.Now.DayOfWeek, today = DateTime.Now.Day, dayInPars;
            if (countWeek == 0)
            {
                countWeek = 6;
            }
            int startDayOfWeek = today - countWeek;
            HtmlWeb aa = new HtmlWeb();
            aa.OverrideEncoding = Encoding.UTF8;
            HtmlDocument v = aa.Load(Paths.uEvents);
            foreach (HtmlNode s in v.DocumentNode.SelectNodes(".//div[contains(@id, 'middle')]/table/tr/td[contains(@id, 'col2')]//div[contains(@id, 'c7687')]//div[contains(@class, 'news_list_wrapper')]/table/tr/td"))
            {
                int start = s.OuterHtml.ToString().IndexOf("<span>");
                int end = s.OuterHtml.ToString().IndexOf("</span>", start);
                dayInPars = Convert.ToInt32(s.OuterHtml.ToString().Substring(start, end - start + "<span>".Length).Split('>')[1].Split('&')[0].Replace(" ", ""));
                DayMonthYearInPars = s.OuterHtml.ToString().Substring(start, end - start + "<span>".Length).Split('>')[1].Split('<')[0].Replace("&nbsp;", " ");
                if (dayInPars >= startDayOfWeek && dayInPars <= today)
                {
                    start = s.OuterHtml.ToString().IndexOf("title");
                    end = s.OuterHtml.ToString().IndexOf("</a>", start);
                    nameEvent = s.OuterHtml.ToString().Substring(start, end - start + "<title>".Length).Split('\"')[1].Split('\"')[0];
                    start = s.OuterHtml.ToString().IndexOf("href");
                    end = s.OuterHtml.ToString().IndexOf("title", start);
                    urlFrm = s.OuterHtml.ToString().Substring(start, end - start + "href".Length).Split('\"')[1].Split('\"')[0];
                    result = $"{nameEvent}\nПодробнее =>{Paths.uVuz}{urlFrm}\nДата публикации: {DayMonthYearInPars} г.";
                    news.Add(result);
                }
            }
            if (news.Count > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Dispose()
        {
            GC.Collect();
        }
    }
}