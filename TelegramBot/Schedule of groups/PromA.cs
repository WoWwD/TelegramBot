using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using TelegramBot.All_Paths;

namespace TelegramBot.Schedule_of_groups
{
    class PromA : IDisposable
    {
        private List<string> FormsOfEducationPromA = new List<string>();
        public static string linkGroupPromA;
        public bool GetPromA(Update update)
        {
            var msg = update.Message.Text;
            HtmlWeb aa = new HtmlWeb();
            aa.OverrideEncoding = Encoding.UTF8;
            HtmlDocument v = aa.Load(Paths.ulevelHigh);
            AddFormsOfEducation(FormsOfEducationPromA, v);
            HtmlDocument m = aa.Load(Paths.ulevelMid);
            AddFormsOfEducation(FormsOfEducationPromA, m);
            for (int i = 0; i < FormsOfEducationPromA.Count; i++)
            {
                HtmlDocument formOfEducation = aa.Load(FormsOfEducationPromA[i].ToString());
                foreach (HtmlNode p in formOfEducation.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                {
                    if (p.OuterHtml.Contains("Промежуточная аттестация"))
                    {
                        var linkFormOfEducation = GetLinkFormOfEducation(p);
                        HtmlDocument pa = aa.Load(linkFormOfEducation);
                        if (pa.DocumentNode.SelectNodes(Paths.tForParsRaspPromA) == null)
                        {
                            break;
                        }
                        foreach (HtmlNode c in pa.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                        {
                            var linkCours = GetLinkCours(c, msg);
                            if (linkCours != null)
                            {
                                HtmlDocument cours = aa.Load(linkCours);
                                foreach (HtmlNode g in cours.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                                {
                                    linkGroupPromA = GetLinkGroup(g, msg);
                                    if (linkGroupPromA != null)
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
        public static bool GetResPromA(Update update)
        {
            using (PromA p = new PromA())
            {
                var res = p.GetPromA(update);
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
        private string GetLinkFormOfEducation(HtmlNode p)
        {
            string url = p.OuterHtml.ToString().Split('=').Last().Split('>')[0];
            return Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
        }
        private string GetLinkCours(HtmlNode c, string message)
        {
            string potok = c.OuterHtml.ToUpper().Split('>')[1].Split('<')[0].Substring(0, 2);
            string yearInStr = message.ToUpper().Split('R')[1].Substring(0, 2);
            if (potok == yearInStr)
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
                return Paths.uTTVuz + link.Substring(2, link.Length - 4) + ""; ;
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