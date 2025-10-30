using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSAGE200CLibrerias
{
    public class Correo
    {

        public static void EnviarCorreo(string asunto, string cuerpoMensaje, Xml config, VTSAGE200CLibrerias.Logger log, bool bolIsAdmin = false)
        {
            try
            {
                string correoDesde = config.MailAccount;
                string SMTPServer = config.MailServer;
                System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

                //Direccion de correo electronico a la que queremos enviar el mensaje
                string correoPara;
                correoPara = config.MailTo;

                mmsg.To.Add(correoPara);

                //Asunto
                mmsg.Subject = asunto;
                mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

                //Cuerpo del Mensaje
                mmsg.Body = cuerpoMensaje;
                mmsg.BodyEncoding = System.Text.Encoding.UTF8;
                mmsg.IsBodyHtml = false;

                //Correo electronico desde la que enviamos el mensaje
                mmsg.From = new System.Net.Mail.MailAddress(correoDesde);

                /*-------------------------CLIENTE DE CORREO----------------------*/

                //Creamos un objeto de cliente de correo
                System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();

                cliente.EnableSsl = config.MailUsesSSL;
                cliente.Port = Int32.Parse(config.MailPort);
                cliente.Host = SMTPServer;

                //Hay que crear las credenciales del correo emisor
                //cliente.Credentials = new System.Net.NetworkCredential(Configuracion.CorreoEnvio, Configuracion.ServidorEnvioCredenciales);
                if (config.MailAuthenticationAnonymous)
                {
                    cliente.UseDefaultCredentials = true;
                }
                else
                {
                    cliente.UseDefaultCredentials = false;
                    cliente.Credentials = new System.Net.NetworkCredential(config.MailUser, config.MailPWD);
                }
                /*-------------------------ENVIAMOS EL CORREO----------------------*/

                //Enviamos el mensaje de manera asíncrona
                cliente.SendCompleted += (s, es) =>
                {
                    log.CrearEntrada("Conector::Correo::Mail enviado con exito");
                    cliente.Dispose();
                    mmsg.Dispose();
                    return;
                };

                cliente.SendAsync(mmsg, "test message");

            }
            catch (System.Net.Mail.SmtpException ex)
            {
                log.CrearEntrada("Conector::Correo::No se pudo notificar vía email: Error: " + ex.Message);
            }
        }
    }
}
