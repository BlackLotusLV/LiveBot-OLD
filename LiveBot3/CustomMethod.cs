using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LiveBot
{
    internal class CustomMethod
    {
        public static string LanguageIfNull(CommandContext ctx)
        {
            DiscordChannel current = ctx.Channel;
            return (current.Id) switch
            {
                (150283740172517376) => "gb",
                (249586001167515650) => "de",
                (253231012492869632) => "fr",
                (410790788738580480) => "nl",
                (410835311602565121) => "se",
                (363977914196295681) => "ru",
                (423845614686699521) => "lv",
                _ => "gb"
            };
        }

        public static string ParamsStringConverter(string[] words)
        {
            string fullmsg = "";
            foreach (string item in words)
            {
                fullmsg += item + " ";
            }
            return fullmsg;
        }

        public static List<string> WebToHTML(string url, string regex)
        {
            HttpWebRequest WebReq = (HttpWebRequest)HttpWebRequest.Create(url);
            WebReq.Method = "GET";
            HttpWebResponse WebRes = (HttpWebResponse)WebReq.GetResponse();
            StreamReader WebSource = new StreamReader(WebRes.GetResponseStream());
            string source = WebSource.ReadToEnd();
            WebRes.Close();
            List<string> list = new List<string>();
            foreach (Match item in Regex.Matches(source, regex))
            {
                list.Add(item.ToString());
            }
            return list;
        }

        public static Rgba32 GetColour(string incolour)
        {
            return (incolour) switch
            {
                "black" => Rgba32.Black,
                "white" => Rgba32.WhiteSmoke,
                "red" => Rgba32.Red,
                "green" => Rgba32.Green,
                _ => Rgba32.Transparent
            };
        }

        public static string GetConnString()
        {
            string json;
            using (var fs = File.OpenRead("DBCFG.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            var cfgjson = JsonConvert.DeserializeObject<Json.DB>(json);
            return $"Host={cfgjson.Host};Username={cfgjson.Username};Password={cfgjson.Password};Database={cfgjson.Database}";
        }
    }
}