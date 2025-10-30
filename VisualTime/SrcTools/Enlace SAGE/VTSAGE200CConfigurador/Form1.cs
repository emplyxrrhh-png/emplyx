using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Xml;
using System.Data.SqlClient;
using VTSAGE200CLibrerias;
using System.Net;
using VisualTimeSageConnector.ExternalApi;

namespace VTSAGE200CConfigurador
{
    public partial class VTSAGE200CConfigurador : Form
    {
        int cont = 0;
        public VTSAGE200CConfigurador()
        {
            InitializeComponent();
        }

        private void BtnPassword_Click(object sender, EventArgs e)
        {
            //mostrar ocultar password, ya que esta viene encriptada, este es el único medio de poder ver una password
            if (txtPassword.PasswordChar == '*')
            {
                Font fnt = new Font(btnPasswordRight.Font, FontStyle.Regular);
                btnPasswordRight.Font = fnt;
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                Font fnt = new Font(btnPasswordRight.Font, FontStyle.Strikeout);
                btnPasswordRight.Font = fnt;
                txtPassword.PasswordChar = '*';
            }

        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            string InstallPath = (new FileInfo(AppDomain.CurrentDomain.BaseDirectory)).Directory.Parent.FullName + @"\Conf";
            openFileDialog1.InitialDirectory = InstallPath;
            openFileDialog1.FileName = "Configurador.txt";
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {

                try {

                    var filePath = openFileDialog1.FileName;
                    var fileStream = openFileDialog1.OpenFile();
                    var fileContent = string.Empty;

                    //leemos el fichero encriptado que nos tendría que llegar. Hay que tener en cuenta que el fichero a cargar tiene que venir encriptado
                    //y con los elementos XML necesarios para el servicio
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }

                    //convertimos el fichero encriptado a XML, desencriptandolo antes y cargamos el nuevo objeto Xml para cargar los datos en pantalla 
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(Seguridad.DesEncriptar(fileContent));

                    var ConfigFile = new Xml(doc);

                    //Cargamos los datos en pantalla
                    //RO
                    txtWSURL.Text = ConfigFile.WebService;
                    txtWSUser.Text = ConfigFile.WSUser;
                    txtWSPassword.Text = ConfigFile.WSPassword;
                    txtDaysLog.Text = Convert.ToString(ConfigFile.DaysLog);
                    txtTimerTick.Text = Convert.ToString(ConfigFile.Timer);
                    txtUserUpdater.Text = ConfigFile.UpdaterUser;
                    txtPasswordUpdater.Text = ConfigFile.UpdaterPassword;
                    txtCodigoCliente.Text = ConfigFile.WSCodigoCliente;
                    chkMT.Checked = ConfigFile.WSMT;

                    //Sage
                    txtSrv.Text = ConfigFile.Server;
                    txtUser.Text = ConfigFile.User;
                    txtPassword.Text = ConfigFile.Password;
                    txtDBName.Text = ConfigFile.BDName;

                    //Correo
                    txtSMTPServer.Text = ConfigFile.MailServer;
                    txtMailPort.Text = ConfigFile.MailPort;
                    chkSSL.Checked = ConfigFile.MailUsesSSL;
                    cbAuthentication.SelectedItem = ConfigFile.MailAuthenticationAnonymous ? "Anónima" : "Básica";
                    txtMailUser.Text = ConfigFile.MailUser;
                    txtMailAccount.Text = ConfigFile.MailAccount;
                    txtMailPWD.Text = ConfigFile.MailPWD;
                    txtMailTo.Text = ConfigFile.MailTo;
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Error al cargar datos:" + Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }


        }

        private void LblDaysLog_Click(object sender, EventArgs e)
        {

        }

        private void TxtPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void TxtPathLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void LblRutaServicio_Click(object sender, EventArgs e)
        {

        }

        private void TxtDaysLog_TextChanged(object sender, EventArgs e)
        {

        }


        private void TxtDaysLog_KeyPress(object sender,KeyPressEventArgs e)
        {

            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void LblTimer_Click(object sender, EventArgs e)
        {

        }

        private void TxtTimerTick_TextChanged(object sender, EventArgs e)
        {

        }
        private void TxtTimerTick_KeyPress(object sender,KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void BtnPasswordLeft_Click(object sender, EventArgs e)
        {
            //mostrar ocultar password, ya que esta viene encriptada, este es el único medio de poder ver una password
            if (txtWSPassword.PasswordChar == '*')
            {
                Font fnt = new Font(btnPasswordLeft.Font,FontStyle.Regular);
                btnPasswordLeft.Font = fnt;
                txtWSPassword.PasswordChar = '\0';
            }
            else
            {
                Font fnt = new Font(btnPasswordLeft.Font, FontStyle.Strikeout);
                btnPasswordLeft.Font = fnt;
                txtWSPassword.PasswordChar = '*';
            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {

            if (RevisarControles())
            {
                //si todos los campos de texto tienen datos, cargamos la string configString con el formato necesario para el configurador del servicio
                lblWarning.Visible = false;
                string configString = String.Format("<?xml version=\"1.0\" encoding=\"utf - 8\"?>" +
                                         "<appSettings>" +
                                            "<WebService>{0}</WebService>" +
                                            "<WSUser>{1}</WSUser>" +
                                            "<WSPassword>{2}</WSPassword>" +
                                            "<DaysLog>{3}</DaysLog>" +
                                            "<Timer>{4}</Timer>" +
                                            "<Server>{5}</Server>" +
                                            "<User>{6}</User>" +
                                            "<Password>{7}</Password>" +
                                            "<BDName>{8}</BDName>" +
                                            "<UpdaterUser>{9}</UpdaterUser>" +
                                            "<UpdaterPassword>{10}</UpdaterPassword>" +
                                            "<MailServer>{11}</MailServer>" +
                                            "<MailPort>{12}</MailPort>" +
                                            "<MailAuthenticationAnonymous>{13}</MailAuthenticationAnonymous>" +
                                            "<MailUsesSSL>{14}</MailUsesSSL>" +
                                            "<MailUser>{15}</MailUser>" +
                                            "<MailPWD>{16}</MailPWD>" +
                                            "<MailTo>{17}</MailTo>" +
                                            "<MailAccount>{18}</MailAccount>" +
                                            "<WSCodigoCliente>{19}</WSCodigoCliente>" +
                                            "<WSMT>{20}</WSMT>" +
                                          "</appSettings>", txtWSURL.Text, txtWSUser.Text, txtWSPassword.Text, 
                                          txtDaysLog.Text, txtTimerTick.Text, txtSrv.Text, txtUser.Text, txtPassword.Text, txtDBName.Text, 
                                          "none","none",txtSMTPServer.Text,txtMailPort.Text, cbAuthentication != null && cbAuthentication.SelectedItem != null ? cbAuthentication.SelectedItem.ToString() == "Anónima" : false,
                                          chkSSL.Checked,txtMailUser.Text, txtMailPWD.Text, txtMailTo.Text, txtMailAccount.Text,
                                          txtCodigoCliente.Text, chkMT.Checked);

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                string InstallPath = (new FileInfo(AppDomain.CurrentDomain.BaseDirectory)).Directory.Parent.FullName + @"\Conf";
                saveFileDialog1.InitialDirectory = InstallPath;
                saveFileDialog1.FileName = "Configurador.txt";
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)

                {
                    StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile());
                    writer.WriteLine(Seguridad.Encriptar(configString));
                    writer.Dispose();
                    writer.Close();
                    MessageBox.Show("Fichero guardado correctamente", "Confirmación", MessageBoxButtons.OK);
                }
            }
            else
            {
                lblWarning.Visible = true;
            }
        }

        private bool RevisarControles() {
            //revisamos si están todos los controles informados o no
            if (txtWSURL.TextLength>0 & txtWSUser.TextLength>0 & txtWSPassword.TextLength > 0 & 
                //txtPath.TextLength > 0 & txtPathLog.TextLength > 0 & 
                txtDaysLog.TextLength > 0 & txtTimerTick.TextLength > 0 & txtSrv.TextLength > 0 & txtUser.TextLength > 0 & txtPassword.TextLength > 0 & txtDBName.TextLength > 0
                //& txtUserUpdater.TextLength >0 & txtPasswordUpdater.TextLength >0
                ) {
                return true;
            }
            return false;
        }
        private Boolean InstalarServicio()
        {
           
            Xml ConfigService = new VTSAGE200CLibrerias.Xml();

            try
            {
                //Apartado de la consola donde se ejecutará el comando. 
                //Tendremos que especificarle tanto el usuario como la contraseña, con tal de que no nos de un error al instalarlo
                // /username=.\adminCloud /password=VTSaaS#14 /unattended
                Process proc1 = new Process();
                proc1.StartInfo.UseShellExecute = true;
                proc1.StartInfo.UseShellExecute = false;
                proc1.StartInfo.FileName = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe";
                proc1.StartInfo.Arguments = @"/username=" + ConfigService.UpdaterUser + " /password=" + ConfigService.UpdaterPassword + " /unattended \"" + (new FileInfo(AppDomain.CurrentDomain.BaseDirectory).Directory.Parent.FullName) + "\\Servicio\\VisualTimeSageConnector.exe \"";
                proc1.StartInfo.RedirectStandardOutput = true;
                proc1.StartInfo.RedirectStandardError = true;
                proc1.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
                proc1.Start();
                proc1.StandardError.ReadToEnd();//miramo si ha ocurrido algún error durante la ejecución de la instalación.
                String error = proc1.StandardError.ReadToEnd();
                proc1.WaitForExit();
                if (error == null || error.Equals(""))
                {
                    //Logger.CrearEntrada("VisualTimeSageConnector instalado satisfactóriamente.");
                    MessageBox.Show("VisualTimeSageConnector instalado correctamente.", "Confirmación", MessageBoxButtons.OK);
                    return true;//retornamos true si se ha instalado correctamente.
                }
                else
                {
                    //Logger.CrearEntrada("Eror en el proceso de instalación \n" + error);
                    MessageBox.Show("Error en el proceso de instalación. " + error, "Confirmación", MessageBoxButtons.OK);
                    //enviamos un correo conforme se ha producido el error y la máquina que es
                    //queuesError.crearQueues();
                    return false;//Si ha ocurrido algún error lo pasaremos al log y pararemos la actualización. 
                }
            }
            catch (Exception e)
            {

                //Logger.CrearEntrada("Eror en el proceso de instalación :\n" + e);
                MessageBox.Show("Error en el proceso de instalación. " + e.Message, "Confirmación", MessageBoxButtons.OK);
                //enviamos un correo conforme se ha producido el error y la máquina que es
                //queuesError.crearQueues();
                return false;//Si ha ocurrido algún error lo pasaremos al log y pararemos la actualización. 
            }
        }
        private Boolean LanzarServicio() {

            try
            {
                //Ponemos que el startuptype del servicio sea Automático
                Process proc2 = new Process();
                proc2.StartInfo.UseShellExecute = false;
                String command2 = "net start VisualTimeSageConnector";
                proc2.StartInfo.WorkingDirectory = @"C:\Windows\System32";
                proc2.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                proc2.StartInfo.Arguments = "/c " + command2;
                proc2.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc2.StartInfo.RedirectStandardError = true;
                proc2.StartInfo.RedirectStandardOutput = true;
                proc2.StartInfo.RedirectStandardError = true;
                proc2.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
                proc2.Start();
                proc2.StandardError.ReadToEnd();
                String error2 = proc2.StandardError.ReadToEnd();
                proc2.WaitForExit();


                //iniciamos el servicio por consola, usaremos el parámetro net start.
                Process proc1 = new Process();
                proc1.StartInfo.UseShellExecute = false;
                String command = "sc config VisualTimeSageConnector start= delayed-auto";
                proc1.StartInfo.WorkingDirectory = @"C:\Windows\System32";
                proc1.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                proc1.StartInfo.Arguments = "/c " + command;
                proc1.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc1.StartInfo.RedirectStandardError = true;
                proc1.StartInfo.RedirectStandardOutput = true;
                proc1.StartInfo.RedirectStandardError = true;
                proc1.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
                proc1.Start();
                proc1.StandardError.ReadToEnd();
                String error = proc1.StandardError.ReadToEnd();
                proc1.WaitForExit();
                //si por comandos no se ha arrancado correctamente, miramos de hacerlo mediante el ServiceControllerStatus
                //solo se ejecutará si el servicio esta Stopped o StopPending
                ServiceController sc = new ServiceController("VisualTimeSageConnector");
                if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
                     (sc.Status.Equals(ServiceControllerStatus.StopPending)))
                {
                    sc.Start();
                }
                //miramos si se ha producido algún error durante la iniciación
                if (error == null || error.Equals(""))
                {
                    // Logger.CrearEntrada("El servicio VisualTimeSageConnector se ha iniciado satisfactóriamente.");
                    MessageBox.Show("El servicio VisualTimeSageConnector se ha iniciado satisfactóriamente.", "Confirmación", MessageBoxButtons.OK);
                    return true;
                }
                else
                {
                    // Logger.CrearEntrada("Eror al iniciar el proceso  \n" + error);
                    MessageBox.Show("Eror al iniciar el proceso. " + error , "Confirmación", MessageBoxButtons.OK);

                    return false;
                }
            }
            catch (Exception e)
            {
                //Logger.CrearEntrada("ERROR al Iniciar el servicio:\n " + e);
                MessageBox.Show("ERROR al Iniciar el servicio: " + e.Message, "Confirmación", MessageBoxButtons.OK);
                return false;
            }
        }
        private void VTSAGE200CConfigurador_Load(object sender, EventArgs e)
        {
            string Path = (new FileInfo(AppDomain.CurrentDomain.BaseDirectory)).Directory.Parent.FullName + @"\Conf\Configurador.txt";
            if (File.Exists(Path))
            {
                try {
                   
                    var fileContent = string.Empty;

                    //leemos el fichero encriptado que nos tendría que llegar. Hay que tener en cuenta que el fichero a cargar tiene que venir encriptado
                    //y con los elementos XML necesarios para el servicio
                    using (StreamReader reader = new StreamReader(Path))
                    {
                        fileContent = reader.ReadToEnd();
                    }

                    //convertimos el fichero encriptado a XML, desencriptandolo antes y cargamos el nuevo objeto Xml para cargar los datos en pantalla 
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(Seguridad.DesEncriptar(fileContent));

                    var ConfigFile = new Xml(doc);

                    //Cargamos los datos en pantalla
                    //RO
                    txtWSURL.Text = ConfigFile.WebService;
                    txtWSUser.Text = ConfigFile.WSUser;
                    txtWSPassword.Text = ConfigFile.WSPassword;
                    //txtPath.Text = ConfigFile.Path;
                    //txtPathLog.Text = ConfigFile.PathLog;
                    txtDaysLog.Text = Convert.ToString(ConfigFile.DaysLog);
                    txtTimerTick.Text = Convert.ToString(ConfigFile.Timer);
                    txtUserUpdater.Text = ConfigFile.UpdaterUser;
                    txtPasswordUpdater.Text = ConfigFile.UpdaterPassword;
                    txtCodigoCliente.Text = ConfigFile.WSCodigoCliente;
                    chkMT.Checked = ConfigFile.WSMT;

                    //Sage
                    txtSrv.Text = ConfigFile.Server;
                    txtUser.Text = ConfigFile.User;
                    txtPassword.Text = ConfigFile.Password;
                    txtDBName.Text = ConfigFile.BDName;
                    //Correo
                    txtSMTPServer.Text = ConfigFile.MailServer;
                    txtMailPort.Text = ConfigFile.MailPort;
                    chkSSL.Checked = ConfigFile.MailUsesSSL;
                    cbAuthentication.SelectedItem = ConfigFile.MailAuthenticationAnonymous ? "Anónima" : "Básica";
                    txtMailUser.Text = ConfigFile.MailUser;
                    txtMailAccount.Text = ConfigFile.MailAccount;
                    txtMailPWD.Text = ConfigFile.MailPWD;
                    txtMailTo.Text = ConfigFile.MailTo;
                }
                catch {

                }
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            cont = cont + 1;
            if (cont == 5)
            {
                btnLoad.Visible = true;
            }
        }

        private void BtnPasswordUpdater_Click(object sender, EventArgs e)
        {
            //mostrar ocultar password, ya que esta viene encriptada, este es el único medio de poder ver una password
            if (txtPasswordUpdater.PasswordChar == '*')
            {
                Font fnt = new Font(btnPasswordUpdater.Font, FontStyle.Regular);
                btnPasswordUpdater.Font = fnt;
                txtPasswordUpdater.PasswordChar = '\0';
            }
            else
            {
                Font fnt = new Font(btnPasswordUpdater.Font, FontStyle.Strikeout);
                btnPasswordUpdater.Font = fnt;
                txtPasswordUpdater.PasswordChar = '*';
            }
        }

        private void btnCheckWS_Click(object sender, EventArgs e)
        {
            if (chkMT.Checked)
            {
                ExternalApi service = new ExternalApi();
                try
                {
                    service.Url = txtWSURL.Text;
                    int returncode = -1;
                    bool returnSpecified = false;
                    service.CreateOrUpdateEmployee(txtCodigoCliente.Text, txtWSUser.Text, txtWSPassword.Text, null, out returncode, out returnSpecified);
                    switch (returncode)
                    {
                        case 1:
                        case 3:
                            MessageBox.Show("Credenciales incorrectas");
                            break;
                        default:
                            MessageBox.Show("Conexión establecida");
                            break;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Excepción: " + ex.Message);
                }
            }
            else
            {
                WebService.ExternalApi service = new WebService.ExternalApi();
                try
                {
                    service.Url = txtWSURL.Text;
                    int returncode = service.CreateOrUpdateEmployee(txtWSUser.Text, txtWSPassword.Text, null);
                    switch (returncode)
                    {
                        case 1:
                        case 3:
                            MessageBox.Show("Credenciales incorrectas");
                            break;
                        default:
                            MessageBox.Show("Conexión establecida");
                            break;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Excepción: " + ex.Message);
                }
            }
            
        }

        private void btnCheckDB_Click(object sender, EventArgs e)
        {
            if (CheckDBConnection()) {
                MessageBox.Show("Conexión establecida");
            } else
            {
                MessageBox.Show("Algún dato es incorrecto");
            }
        }

        private bool CheckDBConnection()
        {
            string connectionString;
            SqlConnection cnn;

            if (txtUser.Text.ToLower() == "windows")
            {
                connectionString = string.Format("Data Source={0}; Initial Catalog={1}; Integrated Security=SSPI;", txtSrv.Text, txtDBName.Text);
            }
            else
            {
                connectionString = string.Format("Data Source={0}; Initial Catalog={1}; User id={2}; Password={3};", txtSrv.Text, txtDBName.Text, txtUser.Text, txtPassword.Text);
            }
           cnn = new SqlConnection(connectionString);
            try
            {
                cnn.Open();
                cnn.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void btnShowMailPWD_Click(object sender, EventArgs e)
        {
            //mostrar ocultar password, ya que esta viene encriptada, este es el único medio de poder ver una password
            if (txtMailPWD.PasswordChar == '*')
            {
                Font fnt = new Font(btnShowMailPWD.Font, FontStyle.Regular);
                btnShowMailPWD.Font = fnt;
                txtMailPWD.PasswordChar = '\0';
            }
            else
            {
                Font fnt = new Font(btnShowMailPWD.Font, FontStyle.Strikeout);
                btnShowMailPWD.Font = fnt;
                txtMailPWD.PasswordChar = '*';
            }
        }

        private void cbVerifyMail_Click(object sender, EventArgs e)
        {
            //SendMail(); //Usando EASendMail
            try
            {
                string correoDesde = txtMailAccount.Text;
                string SMTPServer = txtSMTPServer.Text;
                System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

                //Direccion de correo electronico a la que queremos enviar el mensaje
                string correoPara;
                correoPara = txtMailTo.Text;

                mmsg.To.Add(correoPara);

                //Asunto
                mmsg.Subject = "Test: This is a test mail from VisualTime - SAGE 200c connector";
                mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

                //Cuerpo del Mensaje
                mmsg.Body = "Just a test!. Have a nice day :-)";
                mmsg.BodyEncoding = System.Text.Encoding.UTF8;
                mmsg.IsBodyHtml = false;

                //Correo electronico desde la que enviamos el mensaje
                mmsg.From = new System.Net.Mail.MailAddress(correoDesde);

                /*-------------------------CLIENTE DE CORREO----------------------*/

                //Creamos un objeto de cliente de correo
                System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();

                cliente.EnableSsl = chkSSL.Checked;
                cliente.Port = Int32.Parse(txtMailPort.Text);
                cliente.Host = SMTPServer;

                //Hay que crear las credenciales del correo emisor
                if (cbAuthentication.SelectedItem.ToString() == "Anónima")
                {
                    cliente.UseDefaultCredentials = true;
                }
                else
                {
                    cliente.UseDefaultCredentials = false;
                cliente.Credentials = new System.Net.NetworkCredential(txtMailUser.Text, txtMailPWD.Text);
                }
                /*-------------------------ENVIAMOS EL CORREO----------------------*/
                DateTime tsent = DateTime.Now;
                //Enviamos el mensaje de manera asíncrona
                cliente.SendCompleted += (s, es) =>
                {
                    MessageBox.Show("Correo enviado con éxito");
                    cliente.Dispose();
                    mmsg.Dispose();
                    return;
                };

                //cliente.Send(mmsg);
                cliente.SendAsync(mmsg, "test message");

            }
            catch (System.Net.Mail.SmtpException ex)
            {
                MessageBox.Show("No se pudo enviar el mensaje. Revise los parámetros de configuración. Error: " + ex.Message);
            }
        }


        //private void SendMail()
        //{
        //    string strRet = "OK";
        //    string smtpServer = "";
        //    int smtpPort = 0;
        //    string smtpUser = "";
        //    string smtpPassword = "";
        //    string smtpDomain = "";
        //    string strMailFrom = "";
        //    string strMailTo = txtMailTo.Text;
        //    string strConnType = "ANONYMOUS";
        //    bool bolUseSSL = false;
        //    // Dim objVTS As VTSInitTask
        //    // Dim objServer As Object
        //    // Dim oSec As VTMain.roSecurityBase64

        //    try
        //    {
        //        // objVTS = New VTSInitTask
        //        // objServer = objVTS.ConnectToServer()

        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //        // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        //        // Recuperamos los datos de la base de datos
        //        //smtpServer = txtSMTPServer.Text;
        //        //smtpPort = Int32.Parse(txtMailPort.Text);
        //        //strMailFrom = txtMailUser.Text;
        //        //smtpUser = txtMailUser.Text;
        //        //smtpPassword = txtMailPWD.Text;
        //        smtpServer = "in-v3.mailjet.com";
        //        smtpPort = 587;
        //        strMailFrom = "do_not_reply@visualtime.net";
        //        smtpUser = "d9ba595e35d95857f05acc7750eab57c";
        //        smtpPassword = "f9a37acec865e27865610b83e855c8f7";


        //        strConnType = cbAuthentication.SelectedItem.ToString();
        //        //smtpDomain = Any2String(oParameters.Parameter(Parameters.MailDomain));
        //        bolUseSSL = true;
        //    }
        //    catch { }

        //    // ******************************************************************************************
        //    // EASendMail
        //    // ******************************************************************************************
        //    string errStr = "";
        //    try
        //    {
        //        SmtpMail oMail = new SmtpMail("ES-E1582190613-00480-46DBFD974DB8FA45-54E62683E961B15U"); 
        //        SmtpClient oSmtp = new SmtpClient();

        //        oMail.Reset();

        //        oMail.From = new MailAddress(strMailFrom);
        //        oMail.Bcc = new AddressCollection("x.iglesias@robotics.es");

        //        oMail.Subject = "Test: This is a test mail from VisualTime - SAGE 200c connector"; 
        //        oMail.TextBody = "Just a test!. Have a nice day :-)";


        //        SmtpServer oServer = new SmtpServer(smtpServer, smtpPort);

        //        //if (cbAuthentication.SelectedItem.ToString() != "Anónima")
        //        //{
        //            oServer.AuthType = SmtpAuthType.AuthAuto;
        //            oServer.User = smtpUser;
        //            oServer.Password = smtpPassword;
        //        //}

        //        if ((bolUseSSL))
        //            oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

        //        oSmtp.Timeout = 60;
        //        oSmtp.SendMail(oServer, oMail);
        //        strRet = "OK";
        //    }
        //    catch (SmtpTerminatedException exp)
        //    {
        //        errStr = exp.Message;
        //    }
        //    catch (SmtpServerException exp)
        //    {
        //        errStr = string.Format("Exception: Server Respond: {0}", exp.ErrorMessage);
        //    }
        //    catch (System.Net.Sockets.SocketException exp)
        //    {
        //        errStr = string.Format("Exception: Networking Error: {0} {1}", exp.ErrorCode, exp.Message);
        //    }
        //    catch (Exception exp)
        //    {
        //        errStr = string.Format("Exception: Common: {0}", exp.Message);
        //    }
        //    if (errStr.Length > 0)
        //    {
        //        strRet = "ERROR:" + errStr + Environment.NewLine;
        //    }
        //    MessageBox.Show(strRet);
        //}
    }
}
