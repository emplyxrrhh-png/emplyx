using System;
using System.Collections.Generic;
using System.Xml;
using VTSAGE200CLibrerias;
using System.Data;
using VisualTimeSageConnector.WebService;
using System.Data.SqlClient;

namespace VisualTimeSageConnector
{

    public class WS
    {

        private WebService.ExternalApi service = new WebService.ExternalApi();
        private VTSAGE200CLibrerias.Xml ConfigService;
        private VTSAGE200CLibrerias.Logger Logger = new VTSAGE200CLibrerias.Logger();
        private VTSAGE200CLibrerias.Params Params = new VTSAGE200CLibrerias.Params();

        public WS()
        {
            //Constructor, necesario para cargar de forma dinámica el endpoint del Ws en cuestión, también nos sirve para instanciar y acceder a los datos de configuración
            ConfigService = new VTSAGE200CLibrerias.Xml();
            service.Url = ConfigService.WebService;
        }

        //Listado de metodos, aquí se listarán cada uno de los metodos necesarios para atacar al WS
        //public void ObtenerAccruals()
        //{
        //    try
        //    {
        //        Params = new VTSAGE200CLibrerias.Params();
        //        var accrualCrit = new roDatalinkStandarAccrualCriteria
        //        {
        //            NifEmpleado = "74155258",
        //            NifLetter = "S",
        //            UniqueEmployeeID = "10002",
        //            StartAccrualPeriod = Convert.ToDateTime("01/01/1999"),
        //            EndAccrualPeriod = Convert.ToDateTime("01/01/2020")
        //        };
        //        //Logger.CrearEntrada("WS ObtenerAccruals: prellamada");
        //        roDatalinkStandarAccrualResponse myAccruals = service.GetAccruals(ConfigService.WSUser, ConfigService.WSPassword, accrualCrit);
        //        ConexionSage cnnSage = new ConexionSage();
        //        cnnSage.InsertarAccrual(myAccruals);

        //    }
        //    catch (Exception Ex)
        //    {
        //        Logger.CrearEntrada("Error ObtenerAccruals: User:" + ConfigService.WSUser + " Message:" + Ex.Message);

        //    }
        //}

        //Listado de metodos, aquí se listarán cada uno de los metodos necesarios para atacar al WS
        public void ObtenerAccrualsEmpleados(DataTable Empleados)
        {
            try
            {
                Params = new VTSAGE200CLibrerias.Params();

                foreach (DataRow dr in Empleados.Rows)
                {
                    var accrualCrit = new roDatalinkStandarAccrualCriteria
                    {
                        NifEmpleado = dr["Dni"].ToString().Substring(0, dr["Dni"].ToString().Length - 1),
                        NifLetter = dr["Dni"].ToString().Substring(dr["Dni"].ToString().Length - 1),
                        UniqueEmployeeID = dr["CodigoEmpleado"].ToString(),
                        StartAccrualPeriod = DateTime.Now.Date.AddDays(Convert.ToDouble(Params.Accrual.SelectSingleNode("days").InnerText) * -1),
                        EndAccrualPeriod = DateTime.Now.Date
                    };

                    Logger.CrearEntrada("ObtenerAccruals: " + accrualCrit.NifEmpleado + " " +  accrualCrit.NifLetter + " " + accrualCrit.StartAccrualPeriod.ToShortDateString() + " " + accrualCrit.EndAccrualPeriod.ToShortDateString());

                    roDatalinkStandarAccrualResponse myAccruals = service.GetAccruals(ConfigService.WSUser, ConfigService.WSPassword, accrualCrit);
                    
                    ConexionSage cnnSage = new ConexionSage();
                    cnnSage.InsertarAccrual(myAccruals, accrualCrit);

                }

            }
            catch (Exception Ex)
            {
                Logger.CrearEntrada("Error ObtenerAccruals Message:" + Ex.Message);

            }
        }

        public int Importarempleado(DataRow data, SqlConnection cnn)
        {
            string notificationbody = string.Empty;
            string sEmpleado = string.Empty;
            try
            {

                Params = new Params();

                XmlNodeList EmpresasSync = Params.Companies;

                //Miramos en todas las empresas definidas del params, cuando encontremos la empresa correspondiente tratamos el empleado y salimos del bucle
                foreach (XmlNode Empresa in EmpresasSync)
                {
                    if (Empresa.Attributes.GetNamedItem("id").Value == data["CodigoEmpresa"].ToString())
                    {
                        Logger.CrearEntrada("Conector::WS::Detectados cambios en datos del empleado '" + data["Nombre"].ToString() + "' de la empresa con código " + Empresa.Attributes.GetNamedItem("id").Value);
                        //Tratamos el empleado en base a la parametrización de esa empresa 
                        roDatalinkStandarEmployee Empleado = new roDatalinkStandarEmployee();

                        Empleado.NombreEmpleado = data["Nombre"].ToString();
                        if (data["FechaInicioContrato"].ToString().Length > 0)
                        {
                            Empleado.StartContractDate = DateTime.Parse(data["FechaInicioContrato"].ToString());
                        }

                        Empleado.NifEmpleado = data["Dni"].ToString();

                        //Gestión de contratos en base a parametros
                        switch (Empresa.SelectSingleNode("ContractType").InnerText)
                        {
                            case "0":
                                Empleado.IDContract = data["IdEmpleado"].ToString();
                                break;
                            case "1":
                                Empleado.IDContract = data["CodigoContrato"].ToString();
                                break;
                            case "2":
                                Empleado.IDContract = data["CodigoEmpresa"].ToString() + Empresa.SelectSingleNode("Delimiter").InnerText.ToString() + data["CodigoEmpleado"].ToString() + Empresa.SelectSingleNode("Delimiter").InnerText.ToString() + data["CodigoContrato"].ToString();
                                break;
                            case "3":
                                Empleado.IDContract = data["Dni"].ToString() + Empresa.SelectSingleNode("Delimiter").InnerText.ToString() + Convert.ToDateTime(data["FechaInicioContrato"]).ToString("yyyyMMdd");  //data["FechaInicioContrato"].ToString();
                                break;
                            case "4":
                                Empleado.IDContract = data["CodigoEmpresa"].ToString() + Empresa.SelectSingleNode("Delimiter").InnerText.ToString() + data["CodigoEmpleado"].ToString() + Empresa.SelectSingleNode("Delimiter").InnerText.ToString() + Convert.ToDateTime(data["FechaInicioContrato"]).ToString("yyyyMMdd");
                                break;
                        }

                        // Formato anidado, solo informaremos niveles altos si el previo existe 
                        Empleado.Nivel0 = data["Empresa"].ToString();

                        //Niveles de empresa en base a Params y si están definidos o no
                        if (Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel1") != null) Empleado.Nivel1 = data[Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel1").InnerText].ToString();
                        if (Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel2") != null) Empleado.Nivel2 = data[Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel2").InnerText].ToString();
                        if (Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel3") != null) Empleado.Nivel3 = data[Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel3").InnerText].ToString();
                        if (Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel4") != null) Empleado.Nivel4 = data[Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel4").InnerText].ToString();
                        if (Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel5") != null) Empleado.Nivel5 = data[Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel5").InnerText].ToString();
                        if (Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel6") != null) Empleado.Nivel6 = data[Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel6").InnerText].ToString();
                        if (Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel7") != null) Empleado.Nivel7 = data[Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel7").InnerText].ToString();
                        if (Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel8") != null) Empleado.Nivel8 = data[Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel8").InnerText].ToString();
                        if (Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel9") != null) Empleado.Nivel9 = data[Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel9").InnerText].ToString();
                        if (Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel10") != null) Empleado.Nivel10 = data[Empresa.SelectSingleNode("Niveles").SelectSingleNode("Nivel10").InnerText].ToString();

                        Empleado.EndContractDate = null;
                        if (data["FechaFinalContrato"].ToString().Length > 0)
                        {
                            Empleado.EndContractDate = DateTime.Parse(data["FechaFinalContrato"].ToString());
                        }

                        DateTime dVirtualNow = DateTime.Now;

                        //Si tenemos marcada movilidad en params, la aplicamos, teniendo en cuenta que no puede caer fuera de contrato
                        Empleado.MobilityDate = null;
                        if (Empresa.SelectSingleNode("Mobility").InnerText == "1")
                        {
                            if (Empleado.EndContractDate == null || Empleado.EndContractDate > dVirtualNow)
                            {
                                Empleado.MobilityDate = dVirtualNow;

                            }
                            if (Empleado.StartContractDate > dVirtualNow)
                            {
                                Empleado.MobilityDate = Empleado.StartContractDate;
                            }
                        }

                        Empleado.LabAgreeName = data["Convenio"].ToString();

                        //Employee ID Codigo empleado de sage + delimitador + empresa para posible multi empresa
                        Empleado.UniqueEmployeeID = data["CodigoEmpleado"].ToString() + Empresa.SelectSingleNode("Delimiter").InnerText.ToString() + data["CodigoEmpresa"].ToString();
                        
                        //Informar Fields del empleado, por cada campo, introduciremos una nueva posición en el array
                        roDatalinkEmployeeUserFieldValue myFieldVal;
                        List<roDatalinkEmployeeUserFieldValue> myTotalFields = new List<roDatalinkEmployeeUserFieldValue>();

                        //Incorporamos todos los campos de la ficha definidos en params
                        foreach (XmlNode campoFicha in Empresa.SelectSingleNode("UserFields"))
                        {
                            if (data.Table.Columns.Contains(campoFicha.Name.ToString()) && data[campoFicha.Name.ToString()] != DBNull.Value)
                            {
                                myFieldVal = new roDatalinkEmployeeUserFieldValue();
                                myFieldVal.UserFieldName = campoFicha.InnerText;
                                DateTime temp;
                                if (DateTime.TryParse(data[campoFicha.Name.ToString()].ToString(), out temp))
                                {
                                    myFieldVal.UserFieldValue = temp.ToString("dd-MM-yyyy");
                                }
                                else
                                {
                                    myFieldVal.UserFieldValue = data[campoFicha.Name.ToString()].ToString();
                                }
                                myFieldVal.UserFieldValueDate = dVirtualNow; //Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                                myTotalFields.Add(myFieldVal);
                            }
                        }

                        Empleado.UserFields = myTotalFields.ToArray();

                        //Antes de llamar al WS miramos que estén todos los niveles informados para esta V1?
                        if (Empleado.IDContract.ToString() != "")
                        {      
                            int importarEmployee = service.CreateOrUpdateEmployee(ConfigService.WSUser, ConfigService.WSPassword, Empleado);

                            string errortext = importarEmployee.ToString();
                            if (Enum.IsDefined(typeof(ReturnCode), System.Convert.ToInt32(importarEmployee)))
                            {
                                errortext = Enum.Parse(typeof(ReturnCode), importarEmployee.ToString()).ToString();
                            }

                            //Notifico vía email si es necesario
                            if (importarEmployee != 0)
                            {
                                sEmpleado = Empleado.NombreEmpleado;
                                notificationbody = string.Empty;
                                notificationbody = "Empleado: " + Empleado.NombreEmpleado + "\r\n";
                                notificationbody += "Código de error: " + importarEmployee.ToString() + "\r\n";
                                notificationbody += "Motivo error: " + errortext + "\r\n";
                                notificationbody += "Acción requerida: Debe corregir datos en SAGE200c o en VisualTime Live, y volver a enviar la información (para ello basta hacer cualquier cambio en SAGE 200c \r\n";
                                notificationbody += "Detalle para Soporte: \r\n\r\n";
                                notificationbody += Logger.SerializeNewtonSoft(Empleado);
                            }

                            Logger.CrearEntrada("Conector::WS::Resultado del envío: " + errortext + " .Detalle de datos enviados -> " + Logger.SerializeNewtonSoft(Empleado));
                            return importarEmployee;
                        }
                        else
                        {
                            Logger.CrearEntrada("Conector::WS::Resultado: No se pudo enviar. No hay código de contrato!");
                            notificationbody = string.Empty;
                            sEmpleado = Empleado.NombreEmpleado;
                            notificationbody = "Empleado: " + Empleado.NombreEmpleado + "\r\n";
                            notificationbody += "Código de error: 97 \r\n";
                            notificationbody += "Motivo error: No hay código de contrato para el empleado \r\n";
                            notificationbody += "Acción requerida: Debe corregir datos en SAGE200c. Esto provocará el reenvío de la información del empleado \r\n";
                            return 97;
                        }

                    }
                }
                //Si llego aquí, ninguna de las empresas configuradas es la configurada para el empleado. Esto no debería pasar porque ya no la cogería la select.
                Logger.CrearEntrada("Conector::WS::Resultado: No se pudo enviar. La empresa del empleado en SAGE (" + data["CodigoEmpresa"].ToString() + ") no es una de las configuradas en el conector");
                notificationbody = string.Empty;
                notificationbody += "Código de error: 98 \r\n";
                notificationbody += "Motivo error: Empresa " + data["CodigoEmpresa"].ToString() + " no es una de las configuradas en el conector \r\n";
                notificationbody += "Acción requerida: Incluya la empresa en el filtro del conector \r\n";
                return 98;
            }
            catch (Exception Ex)
            {
                Logger.CrearEntrada("Conector::WS::Error inesperado: " + Ex.Message + Ex.Source);
                notificationbody = string.Empty;
                notificationbody += "Código de error: 99 \r\n";
                notificationbody += "Motivo error: Error inesperado sincronizando datos \r\n";
                notificationbody += "Acción requerida: Compruebe que el PC tiene acceso a Internet y que el servicio de importación de empleados de VisualTime Live está activo. El envío se seguirá intentando de manera automática \r\n";
                notificationbody += "Detalle para Soporte: \r\n\r\n";
                notificationbody += Ex.Message.ToString();
                return 99;
            }
            finally
            {
                //Envío correo si no se puedo sincronizar
                if (notificationbody != string.Empty && ConfigService.MailServer != "" && ConfigService.MailTo != "")
                {
                    //Correo.EnviarCorreo("Error importando empleado " + sEmpleado + " a VisualTime", notificationbody, ConfigService, Logger);
                }
            }

        }

    }

}

