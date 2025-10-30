using Microsoft.Win32;
using System;



namespace VTSAGE200CServicio
{
    // Classe en desuso. Porque no tiramos nada de los registros.
    class RegeditVT
    {
        string ServerPATH = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Robotics\\VisualTime\\Server";
        string PATH = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Robotics\\VisualTime\\Paths";
        public string directoryToftp { get; set; }
        public string directoryFromftp { get; set; }
        public string ftpPwd { get; set; }
        public string ftpServer { get; set; }
        public string ftpUser { get; set; }
        public string ftpLog { get; set; }

        public RegeditVT()
        {
            this.directoryToftp = getRegedit("directoryToftp", ServerPATH);
            this.directoryFromftp = getRegedit("directoryFromftp", ServerPATH);
            this.ftpPwd = getRegedit("ftpPwd", ServerPATH);
            this.ftpServer = getRegedit("ftpServer", ServerPATH);
            this.ftpUser = getRegedit("ftpUser", ServerPATH);
            this.ftpLog = getRegedit("ftpLog", ServerPATH);

        }

        public static string getRegedit(string valor, string path)
        {
            try
            {
                string reg = "";
                reg = (string)Registry.GetValue(path, valor, null);
                return reg;
            }
            catch (Exception e)
            {
                LogFTP.CrearEntrada("Error 2X0001  " + valor );
                string reg = e.ToString();
                return reg;
            }
        }

        
    }
}
