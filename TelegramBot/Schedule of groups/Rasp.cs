using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using TelegramBot.All_Paths;

namespace TelegramBot.Schedule_of_groups
{
    class Rasp: IDisposable
    {
        private List<string> FormsOfEducationRasp = new List<string>();
        public static string linkGroupRasp;
        public bool GetRasp(Update update)
        {
            var msg = update.Message.Text;
            HtmlWeb aa = new HtmlWeb();
            aa.OverrideEncoding = Encoding.UTF8;
            HtmlDocument v = aa.Load(Paths.ulevelHigh);
            AddFormsOfEducation(FormsOfEducationRasp, v);
            HtmlDocument m = aa.Load(Paths.ulevelMid);
            AddFormsOfEducation(FormsOfEducationRasp, m);
            for (int i = 0; i < FormsOfEducationRasp.Count; i++) 
            {
                HtmlDocument formOfEducation = aa.Load(FormsOfEducationRasp[i].ToString());
                foreach (HtmlNode r in formOfEducation.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                {
                    if (r.OuterHtml.Contains("Расписание занятий") || r.OuterHtml.Contains("Установочная сессия"))
                    {
                        var linkFormOfEducation = GetLinkFormOfEducation(r);
                        HtmlDocument ra = aa.Load(linkFormOfEducation);
                        if (ra.DocumentNode.SelectNodes(Paths.tForParsRaspPromA) == null)
                        {
                            break;
                        }
                        foreach (HtmlNode c in ra.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                        {
                            var linkCours = GetLinkCours(c, msg);
                            if (linkCours != null)
                            {
                                HtmlDocument cours = aa.Load(linkCours);
                                foreach (HtmlNode g in cours.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                                {
                                    linkGroupRasp = GetLinkGroup(g, msg);
                                    if (linkGroupRasp != null)
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
            using (Rasp r = new Rasp())
            {
                var res = r.GetRasp(update);
                if (res == true)
                {
                    return true;
                }
                return false;
            }
        }
        public void Dispose()
        {
            GC.Collect();
        }
        private string GetLinkFormOfEducation(HtmlNode r)
        {
            string url = r.OuterHtml.ToString().Split('=').Last().Split('>')[0];
            return Paths.uTTVuz + r.OuterHtml.ToString().Split('=').Last().Split('>')[0].Substring(2, url.Length - 4) + "";
        }
        private string GetLinkCours(HtmlNode c, string message)
        {
            int NowYear = DateTime.Now.Year - 2000;
            int coursPars = Convert.ToInt32(c.OuterHtml.ToUpper().Split('>')[1].Split('<')[0].Substring(0, 1));
            int coursInMessage = Convert.ToInt32(message.ToUpper().Split('R')[1].Substring(0, 2));
            if ((NowYear - coursInMessage) == coursPars)
            {
                string url = c.OuterHtml.Split('=').Last().Split('>')[0];
                return Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
            }
            return null;
        }
        private string GetLinkGroup(HtmlNode g, string message)
        {
            string nameGroup = g.OuterHtml.ToUpper().Split('>')[1].Split('<')[0];
            if (nameGroup == message.ToUpper().Split('R')[1])
            {
                string link = g.OuterHtml.Split('=').Last().Split('>')[0];
                return Paths.uTTVuz + link.Substring(2, link.Length - 4) + "";
            }
            return null;
        }
        private void AddFormsOfEducation(List<string> array, HtmlDocument levelOfEducation)
        {
            string url;
            foreach (HtmlNode s in levelOfEducation.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
            {
                url = s.OuterHtml.Split('=').Last().Split('>')[0];
                url = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                array.Add(url);
            }
        }
    }
}