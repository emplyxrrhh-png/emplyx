using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Win32;

namespace VTSAGE200CLibrerias
{
    public class Xml
    {
        //Datos RO
        public string WebService = string.Empty;
        public string WSUser = string.Empty;
        public string WSPassword = string.Empty;
        public string WSCodigoCliente = string.Empty;
        public bool WSMT = false;
        //public string Path = string.Empty;
        //public string PathLog = string.Empty;
        public int DaysLog = 60;
        public int Timer = 30;
        //Datos Sage
        public string Server = string.Empty;
        public string User = string.Empty;
        public string Password = string.Empty;
        public string BDName = string.Empty;
        public string UpdaterUser = string.Empty;
        public string UpdaterPassword = string.Empty;
        //Correo
        public string MailServer = string.Empty;
        public string MailPort = string.Empty;
        public bool MailAuthenticationAnonymous = false;
        public bool MailUsesSSL = true;
        public string MailAccount = string.Empty;
        public string MailUser = string.Empty;
        public string MailPWD = string.Empty;
        public string MailTo = string.Empty;



        public Xml()
        {
            // Constructor Statements
            //cargamos el path base de donde se encuentra instalado el servicio
            //string InstallPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Services\VTSage200C", "ImagePath", @"C:\VTSAGE200C\");
            //int pos = InstallPath.LastIndexOf(@"\") + 1;
            string InstallPath = (new FileInfo(AppDomain.CurrentDomain.BaseDirectory)).Directory.Parent.FullName + @"\Conf\";

            //cargamos el configurador desde el path donde está instalado el servicio
            string myXmlText = File.ReadAllText(InstallPath + "Configurador.txt");
            //string myXmlText = File.ReadAllText(InstallPath.Substring(0, pos) + "Configurador.txt");
            XmlDocument XDOC = new XmlDocument();
            XDOC.LoadXml(Seguridad.DesEncriptar(myXmlText));
            WebService = XDOC.DocumentElement.SelectSingleNode("WebService").InnerText;
            WSUser = XDOC.DocumentElement.SelectSingleNode("WSUser").InnerText;
            WSPassword = XDOC.DocumentElement.SelectSingleNode("WSPassword").InnerText;
            WSCodigoCliente = XDOC.DocumentElement.SelectSingleNode("WSCodigoCliente").InnerText;
            WSMT = XDOC.DocumentElement.SelectSingleNode("WSMT") != null ? (XDOC.DocumentElement.SelectSingleNode("WSMT").InnerText == "True") : true;
            //Path = XDOC.DocumentElement.SelectSingleNode("Path").InnerText;
            //PathLog = XDOC.DocumentElement.SelectSingleNode("PathLog").InnerText;
            DaysLog = Convert.ToInt32(XDOC.DocumentElement.SelectSingleNode("DaysLog").InnerText);
            Timer = Convert.ToInt32(XDOC.DocumentElement.SelectSingleNode("Timer").InnerText);
            Server = XDOC.DocumentElement.SelectSingleNode("Server").InnerText;
            User = XDOC.DocumentElement.SelectSingleNode("User").InnerText;
            Password = XDOC.DocumentElement.SelectSingleNode("Password").InnerText;
            BDName = XDOC.DocumentElement.SelectSingleNode("BDName").InnerText;
            //Version= XDOC.DocumentElement.SelectSingleNode("Version").InnerText;
            UpdaterUser = XDOC.DocumentElement.SelectSingleNode("UpdaterUser").InnerText;
            UpdaterPassword = XDOC.DocumentElement.SelectSingleNode("UpdaterPassword").InnerText;
            MailServer = (XDOC.DocumentElement.SelectSingleNode("MailServer") != null) ? XDOC.DocumentElement.SelectSingleNode("MailServer").InnerText : string.Empty;
            MailPort = (XDOC.DocumentElement.SelectSingleNode("MailPort") != null) ? XDOC.DocumentElement.SelectSingleNode("MailPort").InnerText : "";
            MailAuthenticationAnonymous = XDOC.DocumentElement.SelectSingleNode("MailAuthenticationAnonymous") != null ? (XDOC.DocumentElement.SelectSingleNode("MailAuthenticationAnonymous").InnerText == "True") : false;
            MailUsesSSL = XDOC.DocumentElement.SelectSingleNode("MailUsesSSL") != null ? (XDOC.DocumentElement.SelectSingleNode("MailUsesSSL").InnerText == "True") : true;
            MailUser = XDOC.DocumentElement.SelectSingleNode("MailUser") != null ? XDOC.DocumentElement.SelectSingleNode("MailUser").InnerText : "";
            MailPWD = XDOC.DocumentElement.SelectSingleNode("MailPWD") != null ? XDOC.DocumentElement.SelectSingleNode("MailPWD").InnerText : "";
            MailTo = XDOC.DocumentElement.SelectSingleNode("MailTo") != null ? XDOC.DocumentElement.SelectSingleNode("MailTo").InnerText : "";
            MailAccount = XDOC.DocumentElement.SelectSingleNode("MailAccount") != null ? XDOC.DocumentElement.SelectSingleNode("MailAccount").InnerText : "";
        }

        public Xml(XmlDocument XDOC)
        {
            WebService = XDOC.DocumentElement.SelectSingleNode("WebService").InnerText;
            WSUser = XDOC.DocumentElement.SelectSingleNode("WSUser").InnerText;
            WSPassword = XDOC.DocumentElement.SelectSingleNode("WSPassword").InnerText;
            WSCodigoCliente = XDOC.DocumentElement.SelectSingleNode("WSCodigoCliente").InnerText;
            WSMT = XDOC.DocumentElement.SelectSingleNode("WSMT") != null ? (XDOC.DocumentElement.SelectSingleNode("WSMT").InnerText == "True") : true;
            //Path = XDOC.DocumentElement.SelectSingleNode("Path").InnerText;
            //PathLog = XDOC.DocumentElement.SelectSingleNode("PathLog").InnerText;
            DaysLog = Convert.ToInt32(XDOC.DocumentElement.SelectSingleNode("DaysLog").InnerText);
            Timer = Convert.ToInt32(XDOC.DocumentElement.SelectSingleNode("Timer").InnerText);
            Server = XDOC.DocumentElement.SelectSingleNode("Server").InnerText;
            User = XDOC.DocumentElement.SelectSingleNode("User").InnerText;
            Password = XDOC.DocumentElement.SelectSingleNode("Password").InnerText;
            BDName = XDOC.DocumentElement.SelectSingleNode("BDName").InnerText;
           // Version = XDOC.DocumentElement.SelectSingleNode("Version").InnerText;
            UpdaterUser = XDOC.DocumentElement.SelectSingleNode("UpdaterUser").InnerText;
            UpdaterPassword = XDOC.DocumentElement.SelectSingleNode("UpdaterPassword").InnerText;
            MailServer = (XDOC.DocumentElement.SelectSingleNode("MailServer") != null) ? XDOC.DocumentElement.SelectSingleNode("MailServer").InnerText : string.Empty;
            MailPort = (XDOC.DocumentElement.SelectSingleNode("MailPort") != null) ? XDOC.DocumentElement.SelectSingleNode("MailPort").InnerText : "";
            MailAuthenticationAnonymous = XDOC.DocumentElement.SelectSingleNode("MailAuthenticationAnonymous") != null ? (XDOC.DocumentElement.SelectSingleNode("MailAuthenticationAnonymous").InnerText == "True") : false;
            MailUsesSSL = XDOC.DocumentElement.SelectSingleNode("MailUsesSSL") != null ? (XDOC.DocumentElement.SelectSingleNode("MailUsesSSL").InnerText == "True") : true;
            MailUser = XDOC.DocumentElement.SelectSingleNode("MailUser")!= null ? XDOC.DocumentElement.SelectSingleNode("MailUser").InnerText : "";
            MailPWD = XDOC.DocumentElement.SelectSingleNode("MailPWD")!= null ? XDOC.DocumentElement.SelectSingleNode("MailPWD").InnerText : "";
            MailTo = XDOC.DocumentElement.SelectSingleNode("MailTo") != null ? XDOC.DocumentElement.SelectSingleNode("MailTo").InnerText: "";
            MailAccount = XDOC.DocumentElement.SelectSingleNode("MailAccount") != null ? XDOC.DocumentElement.SelectSingleNode("MailAccount").InnerText : "";
        }

    }
}