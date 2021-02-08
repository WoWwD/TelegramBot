using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;

namespace TelegramBot.Schedule_of_groups
{
    class PromA: IDisposable
    {
        private List<string> FormsOfEducationPromA = new List<string>();
        private string linkPromA;
        private string linkCoursPromA;
        public static string linkGroupPromA;
        public bool GetPromA(Update update)
        {
            string urlFrm;
            HtmlWeb aa = new HtmlWeb();
            aa.OverrideEncoding = Encoding.UTF8;
            HtmlDocument v = aa.Load(Paths.ulevelHigh);
            foreach (HtmlNode s in v.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
            {
                urlFrm = s.OuterHtml.ToString().Split('=').Last().Split('>')[0];
                urlFrm = Paths.uTTVuz + urlFrm.Substring(2, urlFrm.Length - 4) + "";
                FormsOfEducationPromA.Add(urlFrm);
            }
            HtmlDocument m = aa.Load(Paths.ulevelMid);
            foreach (HtmlNode s in m.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
            {
                urlFrm = s.OuterHtml.ToString().Split('=').Last().Split('>')[0];
                urlFrm = Paths.uTTVuz + urlFrm.Substring(2, urlFrm.Length - 4) + "";
                FormsOfEducationPromA.Add(urlFrm);
            }
            var msg = update.Message.Text;
            string url, nameGroup;
            int potok, yearInStr;
            HtmlWeb html = new HtmlWeb();
            html.OverrideEncoding = Encoding.UTF8;
            for (int i = 0; i < FormsOfEducationPromA.Count; i++)
            {
                HtmlDocument formOfEducation = html.Load(FormsOfEducationPromA[i].ToString());
                foreach (HtmlNode li1 in formOfEducation.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                {
                    if (li1.OuterHtml.Contains("Промежуточная аттестация"))
                    {
                        url = li1.OuterHtml.ToString().Split('=').Last().Split('>')[0];
                        linkPromA = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                        HtmlDocument pa = html.Load(linkPromA);
                        if (pa.DocumentNode.SelectNodes(Paths.tForParsRaspPromA) == null)
                        {
                            break;
                        }
                        foreach (HtmlNode li2 in pa.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                        {
                            potok = Convert.ToInt32(li2.OuterHtml.ToString().ToUpper().Split('>')[1].Split('<')[0].Substring(0, 2));
                            yearInStr = Convert.ToInt32(msg.ToUpper().Split('R')[1].Substring(0, 2));
                            if (potok == yearInStr)
                            {
                                url = li2.OuterHtml.ToString().Split('=').Last().Split('>')[0];
                                linkCoursPromA = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                                HtmlDocument courss = html.Load(linkCoursPromA);
                                foreach (HtmlNode li in courss.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                                {
                                    nameGroup = li.OuterHtml.ToString().ToUpper().Split('>')[1].Split('<')[0];
                                    linkGroupPromA = li.OuterHtml.ToString().Split('=').Last().Split('>')[0];
                                    linkGroupPromA = Paths.uTTVuz + linkGroupPromA.Substring(2, linkGroupPromA.Length - 4) + "";
                                    if (nameGroup == msg.ToUpper().Split('R')[1])
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
    }
}