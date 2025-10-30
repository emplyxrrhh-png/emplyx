using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Saas_Printing_Service
{
    public class Utils
    {

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity); 
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void WriteLog(string log)
        {
            try
            {
                log += DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff tt");
                Console.WriteLine(ConfigurationManager.AppSettings["LogPath"] + DateTime.Today.ToString("yyyy_MM_dd")+".log", new string[] { log });
                File.AppendAllLines(ConfigurationManager.AppSettings["LogPath"] + DateTime.Today.ToString("yyyy_MM_dd")+".log", new string[] { log });
            }
            catch { }
        }

        public static bool IsHtmlContent(string text)
        {
            // Regex to check for any HTML tag
            Regex htmlTagRegex = new Regex("<[^>]+>");
            return htmlTagRegex.IsMatch(text);
        }
    }
}
