using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VTSAGE200CLibrerias;

namespace VisualTimeSageConnector
{

    public enum ReturnCode
    {
        _OK = 0,                         // OK
        _LoginError = 1,                 // (Error de incio de sesión)
        _PermissionDenied = 2,           // (No dispone de permisos para acceder a ws)
        _PasswordUsernameEmpty = 3,      // (nombre o pcontraseña vacio)
        _UnknownError = 4,               // (Error indefinido de aplicación)
        _ConnectionError = 5,            // (Error de usuario y contraseña)
        _SqlError = 6,                   // (Error de conexión a SQL)
        _InvalidEmployee = 7,            // (No se ha podido crear el empleado)
        _InvalidGroup = 8,               // (No se ha podido crear el departamento)
        _InvalidContract = 9,            // (No se ha podido crear el contrato)
        _InvalidCard = 10,               // (La tarjeta indicada no se ha podido asignar)
        _AuthorizationError = 11,        // (No se han podido asignar las autorizaciones indicadas)
        _SomeUserFieldsNotSaved = 12,    // (No se han podido guardar todos los campos de la ficha)
        _InvalidCause = 13,              // (La justificación no existe)
        _ContractAlreadyClosed = 14,     // (El contrato ya esta finalizado)
        _InvalidLabAgree = 15,           // (No se encuentra ningun convenio con el identificador)
        _InvalidCloseDate = 16,          // (Se están modificando datos en periodo de cierre)
        _MandatoryDataMissing = 17,      // (No se han informado todos los datos obligatorios)
        _InvalidShift = 18,              // (El horario no existe)
        _AbsenceOverlapping = 19,        // (La ausencia se solapa con otra ya existente)
        _InvalidPeriod = 20,             // (El periodo de fechas o horas es incorrecto)
        _InvalidCalendarData = 21,       // (Calendario erroneo)
        _InvalidAccrualData = 22,        // (Saldo erroneo)
        _InvalidDocumentType = 23,       // (Tipo de documento incorrecto)
        _InvalidDocumentData = 24,       // (Documneto incorrecto)
        _ErrorSavingDocument = 25,       // (No se pudo guardar el documento)
        _DocumentNotDeliverable = 26,    // (Los documentos de la plantilla no se pueden entregar electrónicamente
        _UnexistentDocumentTemplate = 27, // (No exsite la plantilla de documento especificada
        _DocumentTooBig = 28,            // (Documento demasiado grande)
        _InvalidDocumentTitle = 29,      // (Nombre de documento no permitido)
        _DocumentAlreadyExists = 30,     // (Documento ya asignado al empleado)
        _InvalidPunchType = 31,          // (Tipo de fichaje no soportado)
        _ErrorSavingPunch = 32,          // (No se pudo guardar el fichaje)
        _ImcompleteDataRecovered = 33,   // No se pudieron recuperar algunos datos
        _InvalidMovility = 34            // Movilidad incorrecta
    }


    public class ConexionSage
    {
       
        private VTSAGE200CLibrerias.Xml ConfigService;
        private string connectionString = string.Empty;
        private SqlConnection cnn;
        private VTSAGE200CLibrerias.Logger Logger = new VTSAGE200CLibrerias.Logger();
        private VTSAGE200CLibrerias.Params Params;
        private DateTime dLastLog = DateTime.Now.AddMinutes(-30);


        public ConexionSage()
        {
            //Constructor del acceso a Sage, se define la cadena de conexión cargada desde el configurador,se crean funciones internas para abrir/cerrar conexión
            // y se crean los metodos que harán acceso a la BD
            ConfigService = new VTSAGE200CLibrerias.Xml();
            //IP, DBName, User, Pass
            if (ConfigService.User.ToLower() == "windows") {
                connectionString = string.Format("Data Source={0}; Initial Catalog={1}; Integrated Security=SSPI;", ConfigService.Server, ConfigService.BDName);
            }
            else
            {
                connectionString = string.Format("Data Source={0}; Initial Catalog={1}; User id={2}; Password={3};", ConfigService.Server, ConfigService.BDName, ConfigService.User, ConfigService.Password);
            }
            //Logger.CrearEntrada(connectionString);
            cnn = new SqlConnection(connectionString);
        }

        private void AbrirConexion() {
            cnn = new SqlConnection(connectionString);
            cnn.Open();
        }
        private void CerrarConexion() {
            cnn.Dispose();
            cnn.Close();
        }
        //Metodos que llamaremos para acceder a la Base de datos y hacer las funciones en el servicio
        public void ObtenerEntidades()
            //Función de pruebas que NO se usa
        {
            try {
                AbrirConexion();
                SqlCommand cmd = new SqlCommand("SELECT * FROM GDPR_Entidades", cnn);
                var data = new DataTable();
                string path = "C:\\entidades.txt";
                string FileDelimiter = ",";
                data.Load(cmd.ExecuteReader());

                StreamWriter sw = null;
                sw = new StreamWriter(path, false);

                // primero escribimos la cabecera de la tabla para identificar los campos
                int ColumnCount = data.Columns.Count;
                for (int ic = 0; ic < ColumnCount; ic++)
                {
                    sw.Write(data.Columns[ic]);
                    if (ic < ColumnCount - 1)
                    {
                        sw.Write(FileDelimiter);
                    }
                }
                sw.Write(sw.NewLine);

                // por cada row, guardamos los datos con el delimiter que hemos definido
                foreach (DataRow dr in data.Rows)
                {
                    for (int ir = 0; ir < ColumnCount; ir++)
                    {
                        if (!Convert.IsDBNull(dr[ir]))
                        {
                            sw.Write(dr[ir].ToString());
                        }
                        if (ir < ColumnCount - 1)
                        {
                            sw.Write(FileDelimiter);
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
                CerrarConexion();
            } catch(Exception Ex) {
                Logger.CrearEntrada(Ex.Message);
                CerrarConexion();
            }

        }

        //Función para obtener un DataTable con todos los empleados de Sage a través de filtro o sin
        public DataTable ObtenerEmpleados(string filtro) {
            DataTable data = new DataTable();
            try
            {
                AbrirConexion();
                //Si tenemos filtro, aplicaremos el filtro en el where
                    SqlCommand cmd = new SqlCommand("select distinct EN.CodigoEmpresa, EN.CodigoEmpleado, EN.Dni ,P.RazonSocialEmpleado from EmpleadoNomina EN " + 
                                                    "join Personas P on EN.Dni = P.Dni " + filtro, cnn);

                Logger.CrearEntrada("ObtenerEmpleados: " + "select distinct EN.CodigoEmpresa, EN.CodigoEmpleado, EN.Dni ,P.RazonSocialEmpleado from EmpleadoNomina EN " +
                                                    "join Personas P on EN.Dni = P.Dni " + filtro);

                data.Load(cmd.ExecuteReader());
                
            }
            catch (Exception Ex)
            {
                Logger.CrearEntrada("Error CSage ObtenerEmpleados: " + Ex.Message);
                CerrarConexion();
            }
            finally {
                CerrarConexion();
                
            }
            return data;
        }
        public void ObtenerEmpleado()
        {
            var data = new DataTable();
            bool bWritelog = true;
            try
            {
                AbrirConexion();

                this.Params = new Params();
                XmlNodeList companies = (XmlNodeList)this.Params.Companies;
                string strEmpresaFilterAux = " AND ES.CodigoEmpresa IN ('";
                foreach (XmlNode xmlNode in companies) strEmpresaFilterAux = strEmpresaFilterAux + xmlNode.Attributes.GetNamedItem("id").Value + "' , '";
                string strEmpresaFilter = strEmpresaFilterAux.Substring(0, strEmpresaFilterAux.Length - 3) + ")";

                //obtenemos el total de empleados a actualizar, hay que tener en cuenta que tenga registro en EmpleadoNominaCategorias, si no tiene convenio,no se sincronizará, el convenio es necesario para VT
                string sSQL = "SELECT DISTINCT EN.IdEmpleado, " +
                                "EN.CodigoEmpleado, " +
                                "CASE EN.ExcluirProceso WHEN 1 THEN 'Siempre' WHEN 2 THEN 'No calcula pagas' WHEN 3 THEN 'Sólo calcula pagas' WHEN 4 THEN 'Nunca' END SeCalcula, " +
                                "EN.ExcluirProceso, " +
                                "RazonSocialEmpleado AS Nombre, " +
                                "EN.FechaInicioContrato, " +
                                "EN.FechaFinalContrato, " +
                                "EN.FechaAlta, " +
                                "EN.FechaBaja, " +
                                "EN.FechaAntiguedadReconocida, " +
                                "CONCAT(PD.CodigoSigla, ' ',PD.ViaPublica, ' ',PD.Numero1, ' ',PD.Numero2, ' ',PD.Escalera, ' ',PD.Piso, ' ',PD.Puerta, ' ', PD.Letra, ' ') AS DireccionPostal, " +
                                "PD.CodigoPostal AS CodigoPostal, " +
                                "PD.Municipio AS Municipio, " +
                                "PD.Telefono AS TelefonoFijo, " +
                                "PD.TelefonoMovil AS Movil, " +
                                "EN.Dni, " +
                                "'NUEVAS INCORPORACIONES' AS [NUEVAS INCORPORACIONES], " +
                                "EN.ID_Contrato AS CodigoContrato, " +
                                "EN.IdDelegacion AS IdDelegacion, " +
                                "P.EMail1 AS Email, " +
                                "P.EMail2 AS Email2, " +
                                "P.NombreTC2 AS usr_codigo, " +
                                "ENC.CodigoConvenio, " +
                                "ENC.Categoria, " +
                                "ENC.PuestoTrabajo AS Puesto, " +
                                "C.Convenio, " +
                                "EMP.Empresa, " +
                                "EN.CodigoSeccion, " +
                                "EN.CodigoDepartamento, " +
                                "ES.CodigoEmpresa, " +
                                "DEL.Delegacion, " +
                                "SEC.Seccion, " +
                                "DEP.Departamento, " +
                                "EN.PorJornada, " +
                                "EN.CodigoProyecto, " +
                                "PRO.Proyecto, " +
                                "EN.CodigoCanal, " +
                                "CE.Canal, " +
                                "EN.Profesional, " +
                                "P.Profesion, " +
                                "ES.sysFechaRegistro, " +
                                "ES.sysModifiedDate, " +
                                "ES.sysTick, " +
                                "ES.sysGuidRegistro " +
                                "FROM   EmpleadoNomina_Sync ES " +
                                "JOIN EmpleadoNomina EN ON ES.sysGuidRegistro = EN.IdEmpleado AND EN.FechaInicioContrato IS NOT NULL " +
                                "JOIN Personas P ON EN.Dni = P.Dni AND EN.SiglaNacion = P.SiglaNacion " +
                                "LEFT JOIN PersonasDomicilios PD ON EN.Dni = PD.Dni AND EN.SiglaNacion = PD.SiglaNacion " +
                                "LEFT JOIN EmpleadoNominaCategorias ENC ON ENC.IdEmpleado = EN.IdEmpleado " +
                                "JOIN Convenio C ON C.CodigoConvenio = ENC.CodigoConvenio " +
                                "JOIN Empresas EMP ON EN.CodigoEmpresa = EMP.CodigoEmpresa " +
                                "LEFT JOIN Delegaciones DEL ON EN.IdDelegacion = DEL.IdDelegacion AND EN.CodigoEmpresa = DEL.CodigoEmpresa " +
                                "LEFT JOIN Departamentos DEP ON EN.CodigoDepartamento = DEP.CodigoDepartamento AND EN.CodigoEmpresa = DEP.CodigoEmpresa " +
                                "LEFT JOIN Secciones SEC ON EN.CodigoSeccion = SEC.CodigoSeccion AND EN.CodigoEmpresa = SEC.CodigoEmpresa " +
                                "LEFT JOIN Proyectos PRO ON EN.CodigoProyecto = PRO.CodigoProyecto AND EN.CodigoEmpresa = PRO.CodigoEmpresa " +
                                "LEFT JOIN CanalesEmpresa CE ON EN.CodigoCanal = CE.CodigoCanal AND EN.CodigoEmpresa = CE.CodigoEmpresa " +
                                "WHERE ES.sysTipoAccionRegistro IN ('U','I') " +
                                "      AND ES.sysAppId = 0 " +
                                strEmpresaFilter +
                                "ORDER BY EN.CodigoEmpleado ASC, " +
                                "      EN.FechaAlta ASC";
                //"      ES.sysFechaRegistro ASC, " +
                //"      ES.sysTick DESC ";
                
                SqlCommand cmdGet = new SqlCommand(sSQL, cnn);

                data.Load(cmdGet.ExecuteReader());
            
                bWritelog = (data.Rows.Count > 0);

                foreach (DataRow dr in data.Rows)
                {
                    
                    WS wsCalls = new WS();
                    WSMT wsCallsMT = new WSMT();
                    int retornoImportarEmpleado = -1;

                    if (ConfigService.WSMT) {
                        retornoImportarEmpleado = wsCallsMT.Importarempleado(dr, cnn);
                    }
                    else
                    {
                        retornoImportarEmpleado = wsCalls.Importarempleado(dr, cnn);
                    }

                    if (retornoImportarEmpleado == 0)
                    {
                        //Todo fue bien
                        SqlCommand cmdUpdate = new SqlCommand("UPDATE EmpleadoNomina_Sync SET sysAppId =1  where sysGuidRegistro='" + dr["IdEmpleado"].ToString() + "'", cnn);
                        cmdUpdate.ExecuteNonQuery();
                    }
                    else if (retornoImportarEmpleado != 99)
                    {
                        //Hubo un error y no fue falta de acceso al servicio de VT. Marco el registro con el código de retorno (+1000 para evitar el código de error 1, y diferenciarlo del caso en que ha ido bien, que se marca también con 1)
                        int errorCode = 1000 + retornoImportarEmpleado;
                        SqlCommand cmdUpdate = new SqlCommand("UPDATE EmpleadoNomina_Sync SET sysAppId = " + errorCode.ToString() +  " where sysGuidRegistro='" + dr["IdEmpleado"].ToString() + "'", cnn);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

                if (bWritelog || DateTime.Now.Subtract(dLastLog).TotalMinutes > 15)
                {
                    Logger.CrearEntrada("ConectorSage::ObtenerEmpleado:No hay datos pendientes de sincronizar ...");
                    dLastLog = DateTime.Now;
                }
                
            }
            catch (Exception Ex)
            {
                Logger.CrearEntrada("ConectorSage::ObtenerEmpleado:Se produjo una excepción: " + Ex.Message);
            }
            finally {
                CerrarConexion();
            }
        }

        public void InsertarAccrual(WebService.roDatalinkStandarAccrualResponse Accrual, WebService.roDatalinkStandarAccrualCriteria accrualCrit ) {
            SqlTransaction transaction;
            AbrirConexion();
            transaction = cnn.BeginTransaction("AccrualTransaction");
            try
            {
                //eliminamos los marcajes actuales del empleado para este intervalo de tiempo
                Logger.CrearEntrada("InsertarAccrual: Delete Accruals:: " + accrualCrit.NifEmpleado + " " + accrualCrit.NifLetter + " " + accrualCrit.StartAccrualPeriod.ToShortDateString() + " " + accrualCrit.EndAccrualPeriod.ToShortDateString());
                SqlCommand cmdDeleteAccruals = new SqlCommand(string.Format(" Delete from RHH_Marcajes " +
                                                        " where CodigoEmpleado = '{0}'  and FechaMarcaje >= '{1}' and FechaMarcaje <= '{2}'  "
                                                        , accrualCrit.UniqueEmployeeID,  accrualCrit.StartAccrualPeriod.ToString("yyyyMMdd"), accrualCrit.EndAccrualPeriod.ToString("yyyyMMdd")), cnn);
                cmdDeleteAccruals.Transaction = transaction;
                cmdDeleteAccruals.ExecuteNonQuery();
                

                if (Accrual.Accruals != null)
                {
                    if (Accrual.Accruals.Length > 0)
                    {
                        foreach (WebService.roDatalinkStandarAccrual myAccrual in Accrual.Accruals)
                        {



                            SqlCommand cmdTipoMarcaje = new SqlCommand("select CodigoTipoMarcaje, DescripcionC from RHH_TipoMarcaje ", cnn);
                            DataTable TiposMarcaje = new DataTable();
                            cmdTipoMarcaje.Transaction = transaction;
                            TiposMarcaje.Load(cmdTipoMarcaje.ExecuteReader());
                            int equivalencia = 0;
                            foreach (DataRow dr in TiposMarcaje.Rows)
                            {
                                if (myAccrual.AccrualExportKey == dr["CodigoTipoMarcaje"].ToString())
                                {
                                    equivalencia = 1;
                                }
                            }


                            if (equivalencia == 1)
                            {

                                SqlCommand cmd = new SqlCommand(string.Format("select CodigoEmpresa, IdEmpleado " +
                                                                "from EmpleadoNomina where CodigoEmpleado='{0}' and FechaAlta <= '{1}' and isnull(FechaBaja,'20790101') >= '{1}' ", myAccrual.UniqueEmployeeID, myAccrual.AccrualDate.ToString("yyyyMMdd"))
                                                                , cnn);
                                var data = new DataTable();
                                cmd.Transaction = transaction;
                                data.Load(cmd.ExecuteReader());
                                if (data.Rows.Count > 0)
                                {
                                    string codEmpresa = data.Rows[0]["CodigoEmpresa"].ToString();
                                    string idEmpleado = data.Rows[0]["IdEmpleado"].ToString();

                                    Guid obj = Guid.NewGuid();
                                    SqlCommand cmdInsertAccrual = new SqlCommand(string.Format("insert into RHH_Marcajes " +
                                                                                    "(CodigoEmpresa,CodigoEmpleado,IdEmpleado,CodigoTipoMarcaje,FechaMarcaje,FechaAlta, Cuantia,IdProcesoRhh, EstadoMarcaje) " +
                                                                                    " VALUES " +
                                                                                    " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','1') ", codEmpresa, myAccrual.UniqueEmployeeID, idEmpleado, myAccrual.AccrualExportKey, myAccrual.AccrualDate.ToString("yyyyMMdd"), myAccrual.AccrualDate.ToString("yyyyMMdd"), myAccrual.AccrualValue.ToString().Replace(",", "."), Guid.NewGuid().ToString()), cnn);

                                    cmdInsertAccrual.Transaction = transaction;
                                    cmdInsertAccrual.ExecuteNonQuery();
                                    Logger.CrearEntrada("ConectorSage::InsertarAccrual:Inserted on RHH_Marcajes : " + accrualCrit.NifEmpleado + " " + accrualCrit.NifLetter + " " + myAccrual.AccrualExportKey + " " + myAccrual.AccrualDate.ToString("yyyyMMdd") + " " + myAccrual.AccrualValue.ToString());


                                }
                                else
                                {
                                    Logger.CrearEntrada("ConectorSage::InsertarAccrual:No se ha encontrado idempleado y empresa para el codigo de empleado: " + myAccrual.UniqueEmployeeID + " en la fecha: " + myAccrual.AccrualDate.ToString("yyyyMMdd"));

                                }

                            }
                        }

                    }
                }



                transaction.Commit();
            }

            catch (Exception Ex)
            {
                Logger.CrearEntrada("ConectorSage::InsertarAccrual:Se produjo una excepción: " + Ex.Message);
                transaction.Rollback();
            }
            finally {
                CerrarConexion();
            }
        }

        public void InsertarAccrual(ExternalApi.roDatalinkStandarAccrualResponse Accrual, ExternalApi.roDatalinkStandarAccrualCriteria accrualCrit)
        {
            SqlTransaction transaction;
            AbrirConexion();
            transaction = cnn.BeginTransaction("AccrualTransaction");
            try
            {
                //eliminamos los marcajes actuales del empleado para este intervalo de tiempo
                Logger.CrearEntrada("InsertarAccrual: Delete Accruals:: " + accrualCrit.NifEmpleado + " " + accrualCrit.NifLetter + " " + accrualCrit.StartAccrualPeriod.ToShortDateString() + " " + accrualCrit.EndAccrualPeriod.ToShortDateString());
                SqlCommand cmdDeleteAccruals = new SqlCommand(string.Format(" Delete from RHH_Marcajes " +
                                                        " where CodigoEmpleado = '{0}'  and FechaMarcaje >= '{1}' and FechaMarcaje <= '{2}'  "
                                                        , accrualCrit.UniqueEmployeeID, accrualCrit.StartAccrualPeriod.ToString("yyyyMMdd"), accrualCrit.EndAccrualPeriod.ToString("yyyyMMdd")), cnn);
                cmdDeleteAccruals.Transaction = transaction;
                cmdDeleteAccruals.ExecuteNonQuery();


                if (Accrual.Accruals != null)
                {
                    if (Accrual.Accruals.Length > 0)
                    {
                        foreach (ExternalApi.roDatalinkStandarAccrual myAccrual in Accrual.Accruals)
                        {



                            SqlCommand cmdTipoMarcaje = new SqlCommand("select CodigoTipoMarcaje, DescripcionC from RHH_TipoMarcaje ", cnn);
                            DataTable TiposMarcaje = new DataTable();
                            cmdTipoMarcaje.Transaction = transaction;
                            TiposMarcaje.Load(cmdTipoMarcaje.ExecuteReader());
                            int equivalencia = 0;
                            foreach (DataRow dr in TiposMarcaje.Rows)
                            {
                                if (myAccrual.AccrualExportKey == dr["CodigoTipoMarcaje"].ToString())
                                {
                                    equivalencia = 1;
                                }
                            }


                            if (equivalencia == 1)
                            {

                                SqlCommand cmd = new SqlCommand(string.Format("select CodigoEmpresa, IdEmpleado " +
                                                                "from EmpleadoNomina where CodigoEmpleado='{0}' and FechaAlta <= '{1}' and isnull(FechaBaja,'20790101') >= '{1}' ", myAccrual.UniqueEmployeeID, myAccrual.AccrualDate.ToString("yyyyMMdd"))
                                                                , cnn);
                                var data = new DataTable();
                                cmd.Transaction = transaction;
                                data.Load(cmd.ExecuteReader());
                                if (data.Rows.Count > 0)
                                {
                                    string codEmpresa = data.Rows[0]["CodigoEmpresa"].ToString();
                                    string idEmpleado = data.Rows[0]["IdEmpleado"].ToString();

                                    Guid obj = Guid.NewGuid();
                                    SqlCommand cmdInsertAccrual = new SqlCommand(string.Format("insert into RHH_Marcajes " +
                                                                                    "(CodigoEmpresa,CodigoEmpleado,IdEmpleado,CodigoTipoMarcaje,FechaMarcaje,FechaAlta, Cuantia,IdProcesoRhh, EstadoMarcaje) " +
                                                                                    " VALUES " +
                                                                                    " ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','1') ", codEmpresa, myAccrual.UniqueEmployeeID, idEmpleado, myAccrual.AccrualExportKey, myAccrual.AccrualDate.ToString("yyyyMMdd"), myAccrual.AccrualDate.ToString("yyyyMMdd"), myAccrual.AccrualValue.ToString().Replace(",", "."), Guid.NewGuid().ToString()), cnn);

                                    cmdInsertAccrual.Transaction = transaction;
                                    cmdInsertAccrual.ExecuteNonQuery();
                                    Logger.CrearEntrada("ConectorSage::InsertarAccrual:Inserted on RHH_Marcajes : " + accrualCrit.NifEmpleado + " " + accrualCrit.NifLetter + " " + myAccrual.AccrualExportKey + " " + myAccrual.AccrualDate.ToString("yyyyMMdd") + " " + myAccrual.AccrualValue.ToString());


                                }
                                else
                                {
                                    Logger.CrearEntrada("ConectorSage::InsertarAccrual:No se ha encontrado idempleado y empresa para el codigo de empleado: " + myAccrual.UniqueEmployeeID + " en la fecha: " + myAccrual.AccrualDate.ToString("yyyyMMdd"));

                                }

                            }
                        }

                    }
                }



                transaction.Commit();
            }

            catch (Exception Ex)
            {
                Logger.CrearEntrada("ConectorSage::InsertarAccrual:Se produjo una excepción: " + Ex.Message);
                transaction.Rollback();
            }
            finally
            {
                CerrarConexion();
            }
        }

    }
}
