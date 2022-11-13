using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using TelegramBot.Abstracts;
using TelegramBot.All_Paths;

namespace TelegramBot.Schedule_of_groups
{
    class TimeTableParser: TimeTableActions
    {
        private List<string> formsOfStudy;
        public static string linkGroup;
        public bool GetRasp(Update update)
        {
            formsOfStudy = new List<string>();
            var msg = update.Message.Text;
            HtmlWeb aa = new HtmlWeb();
            aa.OverrideEncoding = Encoding.UTF8;
            HtmlDocument v = aa.Load(Constants.timeTableLevelHigh);
            AddFormsOfEducation(formsOfStudy, v);
            HtmlDocument m = aa.Load(Constants.timeTableLevelMiddle);
            AddFormsOfEducation(formsOfStudy, m);
            foreach (var f in formsOfStudy)
            {
                HtmlDocument formOfEducation = aa.Load(f.ToString());
                foreach (HtmlNode r in formOfEducation.DocumentNode.SelectNodes(Constants.parsingTimeTable))
                {
                    if (r.OuterHtml.Contains("Расписание занятий") || r.OuterHtml.Contains("Установочная сессия"))
                    {
                        var linkFormOfEducation = GetLinkFormOfEducation(r);
                        HtmlDocument ra = aa.Load(linkFormOfEducation);
                        if (ra.DocumentNode.SelectNodes(Constants.parsingTimeTable) == null)
                        {
                            break;
                        }
                        foreach (HtmlNode c in ra.DocumentNode.SelectNodes(Constants.parsingTimeTable))
                        {
                            var linkCours = GetLinkCours(c, msg);
                            if (linkCours != null)
                            {
                                HtmlDocument cours = aa.Load(linkCours);
                                foreach (HtmlNode g in cours.DocumentNode.SelectNodes(Constants.parsingTimeTable))
                                {
                                    linkGroup = GetLinkGroup(g, msg);
                                    if (linkGroup != null)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public static bool GetResRasp(Update update)
        {
            TimeTableParser r = new TimeTableParser();
            var res = r.GetRasp(update);
            if (res == true)
            {
                return true;
            }
            return false;
        }
        public override string GetLinkFormOfEducation(HtmlNode r)
        {
            string url = r.OuterHtml.ToString().Split('=').Last().Split('>')[0];
            return Constants.timeTableUrl + r.OuterHtml.ToString().Split('=').Last().Split('>')[0].Substring(2, url.Length - 4) + "";
        }
        public override string GetLinkCours(HtmlNode htmlNode, string message)
        {
            int NowYear = DateTime.Now.Year - 1999;
            int coursPars = Convert.ToInt32(htmlNode.OuterHtml.ToUpper().Split('>')[1].Split('<')[0].Substring(0, 1));
            int coursInMessage = Convert.ToInt32(message.ToUpper().Split('R')[1].Substring(0, 2));
            if ((NowYear - coursInMessage) == coursPars)
            {
                string url = htmlNode.OuterHtml.Split('=').Last().Split('>')[0];
                return Constants.timeTableUrl + url.Substring(2, url.Length - 4) + "";
            }
            return null;
        }
        private void AddFormsOfEducation(List<string> array, HtmlDocument levelOfEducation)
        {
            string url;
            foreach (HtmlNode s in levelOfEducation.DocumentNode.SelectNodes(Constants.parsingTimeTable))
            {
                url = s.OuterHtml.Split('=').Last().Split('>')[0];
                url = Constants.timeTableUrl + url.Substring(2, url.Length - 4) + "";
                array.Add(url);
            }
        }
    }
}
