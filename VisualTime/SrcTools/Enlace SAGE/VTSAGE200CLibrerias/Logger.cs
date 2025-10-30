using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;


//clase para guardar información en un archivo .txt ""un log"
namespace VTSAGE200CLibrerias
{
    public class Logger
    {
        //public static object Conexion { get; private set; }
        private Xml ConfigService = new Xml();

        public void CrearEntrada(String texto)
        {
            try
            {
                string rutaAntigua = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\" + "Log_" + DateTime.Today.AddDays(-ConfigService.DaysLog).ToString("yyyy-MM-dd") + ".log";
                File.Delete(rutaAntigua);
            }
            catch (DirectoryNotFoundException dirNotFound)
            {
                Console.WriteLine(dirNotFound.Message);
            }


            String ruta = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\" + "Log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

            try
            {
                using (StreamWriter swriter = File.AppendText(ruta)) { 
                swriter.WriteLine(DateTime.Now + " \t " + texto );
                swriter.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string SerializeNewtonSoft(object obj)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.MissingMemberHandling = MissingMemberHandling.Ignore;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            JsonTextWriter jsonWriter = new JsonTextWriter(sw);

            serializer.Serialize(jsonWriter, obj);

            string Json = sw.ToString();
            Json = Json.Replace("1899-12-29", "1999-12-29");
            Json = Json.Replace("1899-12-30", "1999-12-30");
            Json = Json.Replace("1899-12-31", "1999-12-31");

            return Json;
        }

    }
}

