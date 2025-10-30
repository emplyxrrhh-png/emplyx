using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSAGE200CServicio
{
    //Clase en desuso. De momento no se plantea que tenga que hacer ficheros.
    class directory
    {
        public string direct = null;
        RegeditVT regedit = new RegeditVT();

        public directory()
        {
            RegeditVT Conexion = new RegeditVT();
            this.direct = Conexion.directoryToftp;
        }

        public List<string> filesListSimple()
        {
            List<string> filesList = new List<string>();
            try
            {
                DirectoryInfo dir = new DirectoryInfo(direct);
                foreach (FileInfo file in dir.GetFiles())
                {
                    filesList.Add(file.ToString());
                    File.Copy(direct + "\\" + file.ToString(), direct + @"\tmpVT\" + file.ToString());
                    System.IO.FileInfo fi = new System.IO.FileInfo(direct + "\\"+ file.ToString());
                    fi.Delete();

                }
            }
            catch (Exception ex) {
                LogFTP.CrearEntrada("Error 0X1001");
                Console.WriteLine(ex.ToString());
            }
            return filesList;
        }

        public List<string> DirectoryListSimple()
        {
            List<string> DirectoryList = new List<string>();
            try
            {
                DirectoryInfo dir = new DirectoryInfo(direct);
                foreach (DirectoryInfo file in dir.GetDirectories())
                {
                    DirectoryList.Add(file.ToString());
                }
            }
            catch (Exception ex) {
                LogFTP.CrearEntrada("Error 0X1002");
                Console.WriteLine(ex.ToString()); }
            return DirectoryList;
        }

        public void deleteFile(string fichero)
        {
            string aEliminar =  fichero;
            LogFTP.CrearEntrada(aEliminar);
            System.IO.FileInfo fi = new System.IO.FileInfo(aEliminar);
            try
            {
                fi.Delete();
            }
            catch (Exception e)
            {
                LogFTP.CrearEntrada("Error 0X1003");
                Console.WriteLine(e.Message);
            }
        }

        public void deleteFile2(string fichero)
        {
            string aEliminar = fichero;
            LogFTP.CrearEntrada(aEliminar);
            System.IO.FileInfo fi2 = new System.IO.FileInfo(aEliminar);
            try
            {
                fi2.Delete();
            }
            catch (Exception e)
            {
                LogFTP.CrearEntrada("Error 0X1004");
                Console.WriteLine(e.Message);
            }
        }
        public void ComprobarFicherosNecesarios()
        {
            if (!System.IO.Directory.Exists(regedit.directoryToftp + @"\tmp"))
            {
                System.IO.Directory.CreateDirectory(regedit.directoryToftp + @"\tmp");
            }
            if (!System.IO.Directory.Exists(regedit.directoryToftp + @"\tmpVT"))
            {
                System.IO.Directory.CreateDirectory(regedit.directoryToftp + @"\tmpVT");
            }
        }
    }
}
