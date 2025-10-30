//using System;
//using System.ServiceProcess;
//using System.IO;

//namespace VisualTimeSageConnector
//{
//    class servicio
//    {

//        /*
//         * ################## APARTADO COMPROVACIÓN ##################
//        */
//        //miramos si el estado se encuentra Running o Stopped/StopPending
//        public static bool EstadoServicio(string NombreServicio)
//        {
//            bool estado = true;
//            ServiceController sc = new ServiceController(NombreServicio);

//            if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
//                 (sc.Status.Equals(ServiceControllerStatus.StopPending)))
//            {
//                estado = false;//si Stopped parado o StopPending 
//            }
//            else if (sc.Status.Equals(ServiceControllerStatus.Running))
//            {
//                estado = true;//si está encendido
//            }
//            return estado;
//        }
//        //Ahora tenemos que diferenciar entre si está Stopped o StopPending, si es falso es que esta stopped y verdadero si está stopPending
//        public static bool servicioFalse(string NombreServicio)
//        {
//            bool estadofalse = true;
//            ServiceController sc = new ServiceController(NombreServicio);

//            if (sc.Status.Equals(ServiceControllerStatus.Stopped))
//            {
//                estadofalse = false;//si está parado
//            }
//            else if (sc.Status.Equals(ServiceControllerStatus.StopPending))
//            {
//                estadofalse = true;//si está StopPending
//            }
//            return estadofalse;
//        }
//        /*
//         * ################## FIN APARTADO COMPROVACIÓN ##################
//        */

//        /*
//         * ################## APARTADO DE EJECUCIÓN SEGUN EL CONTADOR ##################
//         * El contador tendrá las siguientes posibilidades:
//                --->cont = 0; --> Corresponde a que el servicio está "Running", por lo que si detecta que no lo está y tiene esta variable de contador
//                                  es que es la primera vez que se detecta el error, y se procederá a intentar arrancarlo. 
//                --->cont = 1; --> El servicio se ha intentado arrancar varias veces, pero se detecta que sigue parado. 
//                --->cont = 2; --> El servicio está parado y ya se ha intentado arrancar y notificado.
//                --->cont = 3; --> Se ha detectado que el servicio está como "StopPending".
//                --->cont = 4; --> Se detecta que el servicio ha estado durante 5 minutos en "StopPending".
//                --->cont = 5; --> El servicio se ha arrancado correctamente después de detectar un problema y se notifica al administrador 
//            SE IRÁ CAMBIANDO EL CONTADOR DEPENDIENDO EL PROCESO QUE SE EJECUTE
//        */

//        //si el estado está stop o stopPending:
//        public static int estadoStop(int cont, string estadoDelServicio, string NombreServicio)
//        {   //iniciamos las variables necesarias
//            String mensajeCorreo = "";
//            String mensajeLog = "";
//            String maquina = "";
//            //conseguimos el nombre de la máquina
//            String mypath = Path.Combine(@"C:\", "VTCcomprobarVT", "archivosTXT", "config.txt");
//            String[] lines = System.IO.File.ReadAllLines(mypath);
//            maquina = lines[0];
//            //switch para diferenciar entre ejecutandose y error
//            switch (estadoDelServicio)
//            {
//                case "Ejecutando":
//                    switch (cont)
//                    {
//                        case 3:
//                            //se está ejecutando 
//                            mensajeLog = "El servicio está: ejecutandondose --->cont 3";
//                            Log.CrearEntrada(mensajeLog);//añadimos error al log
//                            cont = 4;
//                            Queues.EnviarQueEstado(estadoDelServicio);//enviamos queues
//                            break;

//                        case 4:
//                            //ha estado 5 minutos ejecutandose y se reporta el error
//                            mensajeLog = "El servicio está: ejecutandondose pero lleva más de 5 minutos ejecutandose --->cont 4";
//                            Log.CrearEntrada(mensajeLog);//añadimos error al log
//                            mensajeCorreo = "El servicio  " + NombreServicio + " en " + maquina + " ha estado durante más de 5 minutos en estado ejecutandose.";
//                            enviarCorreo.EnviarCorreo(asunto: "VTCcomprobarVT detenido", cuerpoMensaje: mensajeCorreo);//enviamos correo
//                            cont = 3;
//                            Queues.EnviarQueEstado(estadoDelServicio);//enviamos queues
//                            break;

//                    }
//                    break;
//                case "Error":
//                    switch (cont)
//                    {
//                        case 0://Cambia el estado del servicio a error por lo que tendremos que enviar un correo y intentarlo arrancar mediante -->arrancarServicio(string NombreServicio)

//                            mensajeLog = "El servicio se ha parado --->cont 0";
//                            Log.CrearEntrada(mensajeLog);//añadimos error al log
//                            //enviamos correo al administrador
//                            mensajeCorreo = "Se ha detenido el servicio " + NombreServicio + ", en " + maquina + ", se procederá a intentar arrancar el servicio, si no se recibe de nuevo un mail, es debido a que no se ha podido arrancar el servicio. ";
//                            enviarCorreo.EnviarCorreo(asunto: "Detenido el servicio VT", cuerpoMensaje: mensajeCorreo);//enviamos correo
//                            //intentamos arrancar el servicio
//                            mensajeLog = "Intentamos arrancar el servicio";
//                            Log.CrearEntrada(mensajeLog);//añadimos error al log
//                            arrancarServicio(NombreServicio);
//                            //cambiamos el contador con tal de pasar a otro apartado del case en al siguiente vuelta
//                            cont = 1;
//                            //Eniamos la Queue correspondiente
//                            Queues.EnviarQueEstado(estadoDelServicio);
//                            break;

//                        case 1://se ha intentado arrancar pero no se ha conseguido 

//                            mensajeLog = "El servicio se ha parado despues de intentar arrancarlo --->cont 1";
//                            Log.CrearEntrada(mensajeLog);
//                            //cambiamos el contador con tal de pasar a otro apartado del case en al siguiente vuelta
//                            cont = 2;
//                            //Eniamos la Queue correspondiente
//                            arrancarServicio(NombreServicio);
//                            Queues.EnviarQueEstado(estadoDelServicio);
//                            break;

//                        case 2://continua el error pero ya se ha enviado el mail

//                            mensajeLog = "El servicio sigue parado --->cont 2";
//                            //Log.CrearEntrada(mensajeLog);
//                            //Eniamos la Queue correspondiente
//                            arrancarServicio(NombreServicio);
//                            Queues.EnviarQueEstado(estadoDelServicio);
//                            break;
//                    }
//                    break;
//            }
//            return cont;
//        }
//        // si el estado es correcto, 
//        public static int estadoRun(int cont, string estadoDelServicio, string NombreServicio)
//        {   //iniciamos las variables necesarias
//            String mensajeCorreo = "";
//            String mensajeLog = "";
//            String maquina = "";
//            VTSAGE200CLibrerias.Logger Log = new VTSAGE200CLibrerias.Logger();
//            //conseguimos el nombre de la máquina
//            String mypath = Path.Combine(@"C:\", "VTCcomprobarVT", "archivosTXT", "config.txt");
//            String[] lines = System.IO.File.ReadAllLines(mypath);
//            maquina = lines[0];
//            if (cont == 5)
//            {
//                mensajeLog = "El servicio se ha arrancado correctamente --->cont 5";
//                Log.CrearEntrada(mensajeLog);//añadimos error al log
//                //enviamos correo al administrador
//                mensajeCorreo = "Se ha arrancado correctamente el servicio " + NombreServicio + " en " + maquina;
//                enviarCorreo.EnviarCorreo(asunto: "Arrancado el servicio VT", cuerpoMensaje: mensajeCorreo);
//                cont = 0;
//                Queues.EnviarQueEstado(estadoDelServicio);//enviamos queues
//                return cont;
//            }
//            else
//            {
//                //cambiamos el contador con tal de que a la siguiente vuelta si da error se tramite
//                cont = 0;
//                Queues.EnviarQueEstado(estadoDelServicio);//enviamos queues
//                return cont;
//            }
//        }

//        //Si está parado intentamos arrancarlo
//        public static void arrancarServicio(string NombreServicio)
//        {
//            ServiceController sc = new ServiceController(NombreServicio);//sc será como llamaremos a ese servico con tal de arrancarlo

//            if (sc.Status == ServiceControllerStatus.Stopped)
//            {
//                try
//                {
//                    // Arrancamos el servicio y esperamos que esté "Running".
//                    sc.Start();
//                    sc.WaitForStatus(ServiceControllerStatus.Running);
//                }
//                catch (InvalidOperationException)
//                {
//                    //Si no arranca devido al proceso enviamos un mensaje de error
//                    string estadoDelServicio = "ErrorIniciar";
//                    Queues.EnviarQueEstado(estadoDelServicio);
//                }
//            }
//        }
//        public static void detenerServicio(string NombreServicio)
//        {
//            ServiceController sc = new ServiceController(NombreServicio);//sc será como llamaremos a ese servico con tal de arrancarlo

//            if (sc.Status == ServiceControllerStatus.Running)
//            {
//                try
//                {
//                    sc.Stop();
//                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
//                    sc.Start();
//                }
//                catch (InvalidOperationException)
//                {
//                    //Si no arranca devido al proceso enviamos un mensaje de error
//                    string estadoDelServicio = "ErrorIniciar";
//                    Queues.EnviarQueEstado(estadoDelServicio);
//                }
//            }
//        }
//    }
//}