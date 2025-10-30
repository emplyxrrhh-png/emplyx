using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace VTSAGE200CLibrerias
{
    public class Params
    {
        public System.Xml.XmlNodeList Companies;
        public System.Xml.XmlNode Accrual;

        public Params()

        {
            string InstallPath = string.Empty;
            try {
                // Constructor Statements
                InstallPath = (new FileInfo(AppDomain.CurrentDomain.BaseDirectory)).Directory.Parent.FullName + @"\Conf\";

                //cargamos el configurador desde el path donde está instalado el servicio
                string myXmlText = File.ReadAllText(InstallPath + "Params.xml");
                XmlDocument XDOC = new XmlDocument();
                XDOC.LoadXml(myXmlText);
                Companies = XDOC.DocumentElement.SelectNodes("empresas/empresa");
                Accrual = XDOC.DocumentElement.SelectSingleNode("Accruals");
            }
            catch (Exception ex){
                Logger log = new Logger();
                log.CrearEntrada("Carpeta: " + InstallPath);
                log.CrearEntrada("Exception: " + ex.Message);
            }
            }
    }
}
