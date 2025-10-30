using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualTimeSageConnector
{
    class Configuracion
    {
        public static string Raiz = @"C://Robotics SA/VTSAGE200C/";
        public static string RutaLog = @"C://Robotics SA/VTSAGE200C/Log/";
        public static int DiasLog = 60;
        public static int MinutosTick = 60;
        public static string WSRuta = "https://empresa.visualtime.net/VTLiveBusiness/Datalink/ExternalApi.asmx";
        public static string WSUsuario = "AAS";
        public static string WSContraseña = "1234As";
    }

}
