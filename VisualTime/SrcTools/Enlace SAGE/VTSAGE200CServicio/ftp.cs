using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace VTSAGE200CServicio
{
    // Classe en desuso. Porque no tiramos nada de los FTP.
    class ftp
    {
        private string host = null;
        private string user = null;
        private string pass = null;
        private string directoryFromftp = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;
        private Boolean PROXY = true;


        public ftp()
        {
            RegeditVT Conexion = new RegeditVT();
            host = Conexion.ftpServer;
            user = Conexion.ftpUser;
            pass = Conexion.ftpPwd;
            directoryFromftp = Conexion.directoryFromftp;
        }
        public bool CheckConec()
        {

            bool Activo = true;



            List<string> DirectoryList = new List<string>();
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/");
                /* Log in to the FTP Server with the User Name and Password Provided */

                // set the ftpWebRequest proxy
                if (PROXY)
                {
                    System.Net.WebProxy proxy = System.Net.WebProxy.GetDefaultProxy();
                    proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    ftpRequest.Proxy = proxy;
                }

                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream);
                /* Store the Raw Response */
                string directoryRaw = null;
                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;


            }
            catch (Exception EX)
            {
                LogFTP.CrearEntrada(EX.Message);
                Activo = false;
            }


            return Activo;
        }

        /* Download File */
        public void download(string remoteFile, string localFile)
        {
            try
            {

                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directoryFromftp + remoteFile);

                // set the ftpWebRequest proxy
                if (PROXY)
                {
                    System.Net.WebProxy proxy = System.Net.WebProxy.GetDefaultProxy();
                    proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    ftpRequest.Proxy = proxy;
                }

                    /* Log in to the FTP Server with the User Name and Password Provided */
                    ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Get the FTP Server's Response Stream */
                ftpStream = ftpResponse.GetResponseStream();
                /* Open a File Stream to Write the Downloaded File */
                FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
                try
                {
                    while (bytesRead > 0)
                    {
                        localFileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Resource Cleanup */
                localFileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                LogFTP.CrearEntrada("Error 0X2001");
                Console.WriteLine(ex.ToString());
            }
            return;
        }

        /* Upload File */
        public bool upload(string remoteFile, string localFile)
        {
            bool fallo = true;
            try
            {
                /* METODO 4 */
                /*
                 using (WebClient client = new WebClient())
                  {
                      client.Credentials = new NetworkCredential(user, pass);
                      client.UploadFile(host + "/" + directoryFromftp + remoteFile, WebRequestMethods.Ftp.UploadFile, localFile);
                  }
              */

                /* METODO 3 */
                /*
                 var ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/");
                ftpRequest.Credentials = new NetworkCredential(user.Normalize(), pass.Normalize());

                ServicePointManager.ServerCertificateValidationCallback +=
                   (sender, cert, chain, sslPolicyErrors) => true;

                ServicePointManager.Expect100Continue = false;

                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.EnableSsl = true;
                //ftpRequest.Proxy = ftpProxy;
                FileStream fs = File.OpenRead(localFile);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                Stream ftpstream = ftpRequest.GetRequestStream();
                ftpstream.Write(buffer, 0, buffer.Length);
                ftpstream.Close();
                */




                /* METODO 2 Que funciono pero que no tiene caracteres especiales */
                // Create an FTP Request 
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/"+ directoryFromftp + remoteFile);
                // set the ftpWebRequest proxy
                ftpRequest.Proxy = null;
                // Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                // When in doubt, use these options 
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                // Specify the Type of FTP Request 
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                FileStream stream = File.OpenRead(localFile);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();
                Stream reqStream = ftpRequest.GetRequestStream();
                reqStream.Write(buffer, 0, buffer.Length);
                reqStream.Flush();
                reqStream.Close();

                /* METODO 1 QUE NO HA FUNCIONADO */
                /* Establish Return Communication with the FTP Server */
                //ftpStream = ftpRequest.GetRequestStream();
                /* Open a File Stream to Read the File for Upload */
                //FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                /* Buffer for the Downloaded Data */
                //byte[] byteBuffer = new byte[bufferSize];
                //int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
                //try
                //{
                // while (bytesSent != 0)
                // {
                //ftpStream.Write(byteBuffer, 0, bytesSent);
                // bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                // }
                //}
                //catch (Exception ex) { LogFTP.CrearEntrada("ftp upload : Error " + ex.ToString()); }
                /* Resource Cleanup */
                //localFileStream.Close();
                //ftpStream.Close();
                //ftpRequest = null;
            }
            catch (Exception ex)
            {
                fallo = false;
                LogFTP.CrearEntrada("Error 0X2002");
                LogFTP.CrearEntrada(ex.ToString());
            }
            return fallo;
        }

        /* Delete File */
        public void delete(string deleteFile)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + deleteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                LogFTP.CrearEntrada("Error 0X2003"); Console.WriteLine(ex.ToString());
            }
            return;
        }

        /* Rename File */
        public void rename(string currentFileNameAndPath, string newFileName)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + currentFileNameAndPath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                /* Rename the File */
                ftpRequest.RenameTo = newFileName;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                LogFTP.CrearEntrada("Error 0X2004");
                Console.WriteLine(ex.ToString());
            }
            return;
        }

        /* Create a New Directory on the FTP Server */
        public void createDirectory(string newDirectory)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + newDirectory);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Resource Cleanup */
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                LogFTP.CrearEntrada("Error 0X2005");
                Console.WriteLine(ex.ToString());
            }
            return;
        }

        /* Get the Date/Time a File was Created */
        public string getFileCreatedDateTime(string fileName)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileName);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream);
                /* Store the Raw Response */
                string fileInfo = null;
                /* Read the Full Response Stream */
                try { fileInfo = ftpReader.ReadToEnd(); }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return File Created Date Time */
                return fileInfo;
            }
            catch (Exception ex)
            {
                LogFTP.CrearEntrada("Error 0X2006");
                Console.WriteLine(ex.ToString());
            }
            /* Return an Empty string Array if an Exception Occurs */
            return "";
        }

        /* Get the Size of a File */
        public string getFileSize(string fileName)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileName);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream);
                /* Store the Raw Response */
                string fileInfo = null;
                /* Read the Full Response Stream */
                try { while (ftpReader.Peek() != -1) { fileInfo = ftpReader.ReadToEnd(); } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return File Size */
                return fileInfo;
            }
            catch (Exception ex)
            {
                LogFTP.CrearEntrada("Error 0X2007");
                Console.WriteLine(ex.ToString());
            }
            /* Return an Empty string Array if an Exception Occurs */
            return "";
        }

        /* List Directory Contents File/Folder Name Only */
        public List<string> directoryListSimple(string directory)
        {
            List<string> DirectoryList = new List<string>();
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directoryFromftp);
                //LogFTP.CrearEntrada("Conexion contra:   "+ host + "/" + directoryFromftp);
                // set the ftpWebRequest proxy
                System.Net.WebProxy proxy = System.Net.WebProxy.GetDefaultProxy();
                proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                ftpRequest.Proxy = proxy;

                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);

                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream);
                /* Store the Raw Response */
                string directoryRaw = null;
                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                try {
                    while (ftpReader.Peek() != -1) {
                        directoryRaw += ftpReader.ReadLine() + "|";
                     }
                }
                catch (Exception ex) { LogFTP.CrearEntrada(ex.ToString()); }
                //LogFTP.CrearEntrada("directoryRaw :      " + directoryRaw);
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
                try
                {
                    //string[] directoryList = directoryRaw.Split("|".ToCharArray()); 
                    String[] directoryList = directoryRaw.Split("|".ToCharArray());
                    foreach (var fileD in directoryList)
                    {
                        if (PROXY)
                        {
                            string fileProv = fileD.Trim();
                            if (fileProv.StartsWith("<a href=\"//VT/IN/"))
                            {
                                fileProv = fileProv.Replace("<a href=\"//VT/IN/", "");
                                fileProv = fileProv.Replace("\">", "");
                                DirectoryList.Add(fileProv);
                            }
                            //DirectoryList.Add(fileProv);
                        }
                        else
                        {
                            string fileProv = fileD.Trim();
                            DirectoryList.Add(fileProv);
                        }
                           

                    }
                    return DirectoryList;
                }
                catch (Exception ex) { LogFTP.CrearEntrada(ex.ToString()); }
            }
            catch (Exception ex)
            {
                LogFTP.CrearEntrada("Error 0X2008"); Console.WriteLine(ex.ToString());
            }
            /* Return an Empty string Array if an Exception Occurs */
            return DirectoryList;
        }

        /* List Directory Contents in Detail (Name, Size, Created, etc.) */
        public string[] directoryListDetailed(string directory)
        {
            try
            {
                /* Create an FTP Request */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                /* Establish Return Communication with the FTP Server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                ftpStream = ftpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(ftpStream);
                /* Store the Raw Response */
                string directoryRaw = null;
                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                /* Resource Cleanup */
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
                try { string[] directoryList = directoryRaw.Split("|".ToCharArray()); return directoryList; }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
            catch (Exception ex)
            {
                LogFTP.CrearEntrada("Error 0X2009");
                Console.WriteLine(ex.ToString());
            }
            /* Return an Empty string Array if an Exception Occurs */
            return new string[] { "" };
        }
    }
}

