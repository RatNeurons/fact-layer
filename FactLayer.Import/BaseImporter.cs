﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace FactLayer.Import
{
    public abstract class BaseImporter
    {

        protected static bool IgnoreUrl(string url)
        {
            if (url == "facebook.com"
                || url == "twitter.com"
                || url == "itunes.apple.com"
                || url == "yourcwtv.com"
                || url == "youtube.com"
                || url == "theaware.net" //no longer online
                || url == "mediabiasfactcheck.com"
                || url == "theearthchild.co.za" //Offline
                || url == null)
            {
                return true;
            }
            return false;
        }

        protected static string NormalizeUrl(string url)
        {
            if (url == "apnews.com")
            {
                return "ap.org";
            }
            else if (url == "cbs.com")
            {
                return "cbsnews.com";
            }
            else if (url == "en.search.farsnews.com")
            {
                return "farsnews.com";
            }
            else if (url == "eng.majalla.com")
            {
                return "majalla.com";
            }
            else if (url == "front.moveon.org")
            {
                return "moveon.org";
            }
            else if (url == "7online.com")
            {
                return "abc7ny.com";
            }
            else if (url == "edition.cnn.com")
            {
                return "cnn.com";
            }
            else if (url == "watchdog.org")
            {
                return "thecentersquare.com";
            }
            else if (url == "votesmart.org")
            {
                return "justfacts.votesmart.org";
            }
            else if (url == "qu.edu")
            {
                return "poll.qu.edu";
            }
            else if (url == "worldpoliticsus.com")
            {
                return "worldpoliticus.com";
            }
            else if (url == "addictinginfo.com")
            {
                return "addictinginfo.org";
            }
            else
            {
                return url;
            }
        }


        protected static string ExtractDomainNameFromURL(string Url)
        {
            if (String.IsNullOrEmpty(Url))
            {
                return null;
            }

            Url = Url.Replace(" ", "").Replace(",com",".com").Replace("`","");
            if (!Url.Contains("://"))
                Url = "http://" + Url;

            var host = new Uri(Url).Host.Replace("www.", "");

            return NormalizeUrl(host);
        }

        static private string Ellipsis(string text, int length)
        {
            if (text.Length <= length) return text;
            int pos = text.IndexOf(" ", length);
            if (pos >= 0)
                return text.Substring(0, pos) + "...";
            return text;
        }

        protected static string GetWikipediaDescription(string url)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            var request = WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            string html;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                html = sr.ReadToEnd();
            }
            doc.LoadHtml(html);

            var firstParagraph = doc.QuerySelectorAll("div.mw-parser-output > p:not(.mw-empty-elt)").Where(s => !s.InnerText.ToLower().StartsWith("coordinates")).FirstOrDefault().InnerText;
            firstParagraph = HttpUtility.HtmlDecode(firstParagraph);
            //Strip out links / citations
            firstParagraph = Regex.Replace(firstParagraph, @"\[\d*\]", "");
            firstParagraph = firstParagraph.Replace("[citation needed]", "");
            firstParagraph = firstParagraph.Replace("[better source needed]", "");
            firstParagraph = firstParagraph.Replace("\\n", "");

            return Ellipsis(firstParagraph, 400);

        }
    }
}
