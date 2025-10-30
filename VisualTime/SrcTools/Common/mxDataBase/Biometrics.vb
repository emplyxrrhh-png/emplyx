Imports System.Data.SQLite

Namespace Database
    Public Class clsBiometricsTbl

        Public Enum BioPosition
            image00
            image01
            image10
            image11
            custom
        End Enum

        Private Const COL_idEmployee = 0
        Private Const COL_idFinger = 1
        Private Const COL_Finger = 2
        Private Const COL_FingerAsBase64 = 3
        Private Const COL_FingerLenAsBase64 = 4


        ''' <summary>
        ''' Se genera 4 array para segmentar las huella para un reconocimiento más rapido
        ''' 00 huella primaria de la huella 1
        ''' 01 huella secundaria de la huella 1
        ''' 10 huella primaria de la huella 2
        ''' 11 huella secundaria de la huella 2
        ''' </summary>
        ''' <remarks></remarks>
        Private mFingers00 As Byte()
        Private mFingers01 As Byte()
        Private mFingers10 As Byte()
        Private mFingers11 As Byte()
        Private mFingersCustom As Byte()
        Private mFingerCount00 As Integer
        Private mFingerCount01 As Integer
        Private mFingerCount10 As Integer
        Private mFingerCount11 As Integer
        Private mFingerCountCustom As Integer
        Private mIDs00 As Integer()
        Private mIDs01 As Integer()
        Private mIDs10 As Integer()
        Private mIDs11 As Integer()
        Private mIDsCustom As Integer()

        Private mConn As SQLiteConnection

        ''' <summary>
        ''' Se inicializan los array biometricos
        ''' </summary>
        ''' <param name="conn"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef conn As SQLiteConnection)
            Try
                mConn = conn
                mFingers00 = Array.CreateInstance(GetType(Byte), 0)
                mIDs00 = Array.CreateInstance(GetType(Integer), 0)
                mFingerCount00 = 0

                mFingers01 = Array.CreateInstance(GetType(Byte), 0)
                mIDs01 = Array.CreateInstance(GetType(Integer), 0)
                mFingerCount01 = 0

                mFingers10 = Array.CreateInstance(GetType(Byte), 0)
                mIDs10 = Array.CreateInstance(GetType(Integer), 0)
                mFingerCount10 = 0

                mFingers11 = Array.CreateInstance(GetType(Byte), 0)
                mIDs11 = Array.CreateInstance(GetType(Integer), 0)
                mFingerCount11 = 0

                mFingersCustom = Array.CreateInstance(GetType(Byte), 0)
                mIDsCustom = Array.CreateInstance(GetType(Integer), 0)
                mFingerCountCustom = 0

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::New:", ex)
            End Try
        End Sub

        Public ReadOnly Property Fingers00() As Byte()
            Get
                Return mFingers00
            End Get
        End Property

        Public ReadOnly Property FingersCount00() As Integer
            Get
                Return mFingerCount00
            End Get
        End Property


        Public ReadOnly Property Fingers01() As Byte()
            Get
                Return mFingers01
            End Get
        End Property

        Public ReadOnly Property FingersCount01() As Integer
            Get
                Return mFingerCount01
            End Get
        End Property

        Public ReadOnly Property Fingers10() As Byte()
            Get
                Return mFingers10
            End Get
        End Property

        Public ReadOnly Property FingersCount10() As Integer
            Get
                Return mFingerCount10
            End Get
        End Property

        Public ReadOnly Property Fingers11() As Byte()
            Get
                Return mFingers11
            End Get
        End Property

        Public ReadOnly Property FingersCount11() As Integer
            Get
                Return mFingerCount11
            End Get
        End Property


        Public ReadOnly Property FingersCustom() As Byte()
            Get
                Return mFingersCustom
            End Get
        End Property

        Public ReadOnly Property FingersCountCustom() As Integer
            Get
                Return mFingerCountCustom
            End Get
        End Property


#Region "Gestion de tabla"

        ''' <summary>
        ''' Crea la tabla en la BBDD
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CreateTable(Optional ByVal _sTerminalType As String = "MX7")
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand

                'Creamos la tabla de biometricos
                Select Case _sTerminalType.ToUpper
                    Case "MX7"
                        SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS biometrics (idEmployee INTEGER, idFinger INTEGER, Finger BLOB);"
                    Case "MX8"
                        SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS biometrics (idEmployee INTEGER, idFinger INTEGER, Finger BLOB, zk_ftemplate varchar(2000), zk_ftlen smallint);"
                End Select

                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBiometricsTbl::CreateDB:biometrics table created.")

                'Creamos indice de tarjetas
                SQLcommand.CommandText = "CREATE INDEX IF NOT EXISTS idxbiometrics ON biometrics(idEmployee);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBiometricsTbl::CreateTable:biometrics index created.")
                SQLcommand.Dispose()


            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::CreateTable:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Borra datos de la tabla
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ClearTable()
            Try
                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "DELETE FROM biometrics;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBiometricsTbl::ClearTable:biometrics table empty.")
                'Load()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::ClearTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Reindexa la tabla
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ReindexTable()
            Try
                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "REINDEX biometrics;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBiometricsTbl::ReindexTable:biometrics index has updated.")
                'Load()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::ReindexTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Añade un registro a la tabla
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <param name="IDFinger"></param>
        ''' <param name="Finger"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function addRow(ByVal IDEmployee As Integer, ByVal IDFinger As Integer, ByVal Finger As Byte(), Optional ByVal _sTerminalType As String = "MX7") As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Añadimos la huella
                Select Case _sTerminalType.ToUpper
                    Case "MX7"
                        SQLcommand.CommandText = "INSERT OR REPLACE INTO biometrics(idEmployee,idFinger,Finger) VALUES(" + clsGlobal.Any2SQL(IDEmployee) + "," + clsGlobal.Any2SQL(IDFinger) + ",@finger);"
                    Case "MX8"
                        SQLcommand.CommandText = "INSERT OR REPLACE INTO biometrics(idEmployee,idFinger,Finger,zk_ftemplate, zk_ftlen) VALUES(" + clsGlobal.Any2SQL(IDEmployee) + "," + clsGlobal.Any2SQL(IDFinger) + ",@finger,'" + Convert.ToBase64String(Finger) + "'," + "2050" + ");"
                        'PDTE: Estoy poniendo la longitud de la huella fija a 2050. Se debería calcular
                End Select

                Dim SQLparm As New SQLiteParameter("@finger", DbType.Binary)
                SQLparm.Value = Finger
                SQLcommand.Parameters.Add(SQLparm)
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBiometricsTbl::addRow:New row(" + IDEmployee.ToString + "," + IDFinger.ToString + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::addRow:", ex)
                Return False
            End Try

        End Function
        ''' <summary>
        ''' Añade una huella a la BBDD (terminales zk)
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <param name="IDFinger"></param>
        ''' <param name="fFinger"></param>
        ''' <param name="iTemplateLen"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function addRow_zk(ByVal IDEmployee As Integer, ByVal IDFinger As Integer, ByVal fFinger As Byte(), ByVal iTemplateLen As Integer) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "INSERT OR REPLACE INTO biometrics(idEmployee,idFinger,Finger,zk_ftemplate, zk_ftlen) VALUES(" + clsGlobal.Any2SQL(IDEmployee) + "," + clsGlobal.Any2SQL(IDFinger) + ",@finger,'" + Convert.ToBase64String(fFinger) + "'," + iTemplateLen.ToString + ");"
                Dim SQLparm As New SQLiteParameter("@finger", DbType.Binary)
                SQLparm.Value = fFinger
                SQLcommand.Parameters.Add(SQLparm)
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBiometricsTbl::addRow_zk:New row(" + IDEmployee.ToString + "," + IDFinger.ToString + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::addRow_zk:", ex)
                Return False
            End Try

        End Function

        Public Function FingerAlreadyExsists(ByVal IDEmployee As Integer, ByVal IDFinger As Integer) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand
                SQLcommand = mConn.CreateCommand

                SQLcommand.CommandText = "SELECT count(*) from biometrics where idEmployee = " + IDEmployee.ToString + " and idFinger = " + IDFinger.ToString + ";"
                Return clsGlobal.Any2Long(SQLcommand.ExecuteScalar()) > 0

                SQLcommand.Dispose()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::FingerAlreadyExsists:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Borra registro de la tabla
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <param name="IDFinger"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function delRow(ByVal IDEmployee As Integer, ByVal IDFinger As Byte) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "DELETE FROM biometrics WHERE idEmployee=" + clsGlobal.Any2SQL(IDEmployee) + " AND idFinger=" + clsGlobal.Any2SQL(IDFinger) + ";"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBiometricsTbl::delRow:delete row(" + IDEmployee.ToString + "," + IDFinger.ToString + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::delRow:", ex)
                Return False
            End Try

        End Function
#End Region

#Region "Consultas"

        ''' <summary>
        ''' Devuelve la huella de un empleado
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <param name="IDFinger"></param>
        ''' <param name="Finger"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetBiometric(ByVal IDEmployee As Integer, ByVal IDFinger As Byte, ByRef Finger As Byte()) As Boolean
            Try

                Dim SQLcommand As New SQLiteCommand("Select * from biometrics where idEmployee=" + IDEmployee.ToString + " and idFinger=" + IDFinger.ToString, mConn)
                Dim sqReader As SQLiteDataReader = SQLcommand.ExecuteReader

                'Si hay datos los carga
                If sqReader.Read() Then
                    Dim len As Integer = 0
                    Dim bio() As Byte

                    len = sqReader.GetBytes(COL_Finger, 0, Nothing, 0, 0)
                    ReDim bio(len)

                    sqReader.GetBytes(COL_Finger, 0, bio, 0, len)
                    Finger = bio
                    Return True
                Else
                    Return False
                End If

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Obtiene el ID de empleado dada la posición de la huella
        ''' </summary>
        ''' <param name="ImageType"></param>
        ''' <param name="Position"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEmployeeByPos(ByVal ImageType As BioPosition, ByVal Position As Integer) As Integer
            Try
                Select Case ImageType
                    Case BioPosition.image00
                        If Position < mIDs00.Length And Position > -1 Then
                            Return mIDs00(Position)
                        End If
                    Case BioPosition.image01
                        If Position < mIDs01.Length And Position > -1 Then
                            Return mIDs01(Position)
                        End If
                    Case BioPosition.image10
                        If Position < mIDs10.Length And Position > -1 Then
                            Return mIDs10(Position)
                        End If
                    Case BioPosition.image11
                        If Position < mIDs11.Length And Position > -1 Then
                            Return mIDs11(Position)
                        End If
                    Case BioPosition.custom
                        If Position < mIDsCustom.Length And Position > -1 Then
                            Return mIDsCustom(Position)
                        End If
                End Select
                ' Si llego aquí, es que no encontré un empleado
                Return 0
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::GetEmployeeByPos:", ex)
                Return 0
            End Try
        End Function

        ''' <summary>
        ''' Crea el cache de huellas de un empleado
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadEmployeeFinger(ByVal IDEmployee As Integer) As Boolean
            Try

                Dim i As Integer = 0
                Dim len As Integer = 0
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBiometricsTbl::Load:Loading employee(" + IDEmployee.ToString + ") biometrics...")

                Dim SQLcommand As New SQLiteCommand("Select * from biometrics where idEmployee=" + IDEmployee.ToString, mConn)
                Dim sqReader As SQLiteDataReader = SQLcommand.ExecuteReader

                ''Inicializa las variables
                mFingersCustom = Array.CreateInstance(GetType(Byte), 0)
                mIDsCustom = Array.CreateInstance(GetType(Integer), 0)

                Dim bio(400) As Byte
                Dim bio0(200) As Byte
                Dim bio1(200) As Byte
                mFingerCountCustom = 0

                While sqReader.Read
                    'Separamos la huella en las 2 imagenes
                    len = sqReader.GetBytes(COL_Finger, 0, Nothing, 0, 0)
                    sqReader.GetBytes(COL_Finger, 0, bio, 0, len)
                    System.Buffer.BlockCopy(bio, 0, bio0, 0, 200)
                    System.Buffer.BlockCopy(bio, 200, bio1, 0, 200)

                    ReDim Preserve mFingersCustom(mFingersCustom.Length + 256)
                    ReDim Preserve mIDsCustom(mFingerCountCustom + 1)
                    mIDsCustom(mFingerCountCustom) = sqReader.GetInt32(COL_idEmployee)
                    bio0.CopyTo(mFingersCustom, mFingerCountCustom * 256)
                    mFingerCountCustom += 1

                    ReDim Preserve mFingersCustom(mFingersCustom.Length + 256)
                    ReDim Preserve mIDsCustom(mFingerCountCustom + 1)
                    mIDsCustom(mFingerCountCustom) = sqReader.GetInt32(COL_idEmployee)
                    bio1.CopyTo(mFingersCustom, mFingerCountCustom * 256)
                    mFingerCountCustom += 1

                End While
                sqReader.Dispose()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBiometricsTbl::LoadEmployeeFinger:Loaded(" + mFingerCountCustom.ToString + ").")
                Return mFingerCountCustom > 0

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::LoadEmployeeFinger:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Crea el cache de huellas
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Load() As Boolean
            Try

                Dim i As Integer = 0
                Dim len As Integer = 0
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBiometricsTbl::Load:Loading biometrics...")

                Dim SQLcommand As New SQLiteCommand("Select * from biometrics", mConn)
                Dim sqReader As SQLiteDataReader = SQLcommand.ExecuteReader

                ''Inicializa las variables
                mFingers00 = Array.CreateInstance(GetType(Byte), 0)
                mIDs00 = Array.CreateInstance(GetType(Integer), 0)
                mFingers01 = Array.CreateInstance(GetType(Byte), 0)
                mIDs01 = Array.CreateInstance(GetType(Integer), 0)
                mFingers10 = Array.CreateInstance(GetType(Byte), 0)
                mIDs10 = Array.CreateInstance(GetType(Integer), 0)
                mFingers11 = Array.CreateInstance(GetType(Byte), 0)
                mIDs11 = Array.CreateInstance(GetType(Integer), 0)

                Dim bio(400) As Byte
                Dim bio0(200) As Byte
                Dim bio1(200) As Byte
                mFingerCount00 = 0
                mFingerCount01 = 0
                mFingerCount10 = 0
                mFingerCount11 = 0
                While sqReader.Read
                    'Separamos la huella en las 2 imagenes
                    len = sqReader.GetBytes(COL_Finger, 0, Nothing, 0, 0)
                    sqReader.GetBytes(COL_Finger, 0, bio, 0, len)
                    System.Buffer.BlockCopy(bio, 0, bio0, 0, 200)
                    System.Buffer.BlockCopy(bio, 200, bio1, 0, 200)

                    If clsGlobal.Any2Long(sqReader.GetInt32(COL_idFinger)) = 0 Then
                        ReDim Preserve mFingers00(mFingers00.Length + 256)
                        ReDim Preserve mFingers01(mFingers01.Length + 256)
                        ReDim Preserve mIDs00(mFingerCount00 + 1)
                        ReDim Preserve mIDs01(mFingerCount01 + 1)
                        mIDs00(mFingerCount00) = sqReader.GetInt32(COL_idEmployee)
                        mIDs01(mFingerCount01) = sqReader.GetInt32(COL_idEmployee)
                        bio0.CopyTo(mFingers00, mFingerCount00 * 256)
                        bio1.CopyTo(mFingers01, mFingerCount01 * 256)
                        mFingerCount00 += 1
                        mFingerCount01 += 1
                    Else
                        ReDim Preserve mFingers10(mFingers10.Length + 256)
                        ReDim Preserve mFingers11(mFingers11.Length + 256)
                        ReDim Preserve mIDs10(mFingerCount10 + 1)
                        ReDim Preserve mIDs11(mFingerCount11 + 1)
                        mIDs10(mFingerCount10) = sqReader.GetInt32(COL_idEmployee)
                        mIDs11(mFingerCount11) = sqReader.GetInt32(COL_idEmployee)
                        bio0.CopyTo(mFingers10, mFingerCount10 * 256)
                        bio1.CopyTo(mFingers11, mFingerCount11 * 256)
                        mFingerCount10 += 1
                        mFingerCount11 += 1

                    End If

                End While
                sqReader.Dispose()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBiometricsTbl::Load:Loaded(" + mFingerCount00.ToString + "," + mFingerCount01.ToString + "," + mFingerCount10.ToString + "," + mFingerCount11.ToString + ").")
                clsGlobal.HasFingers = (mFingerCount00 > 0 Or mFingerCount01 > 0 Or mFingerCount10 > 0 Or mFingerCount11 > 0 Or mFingerCountCustom > 0)
                Return True

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::Load:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Obtienen número de huellas registradas
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FingerprintsCount() As Integer
            Try
                Dim SQLcommand As SQLiteCommand
                SQLcommand = mConn.CreateCommand

                SQLcommand.CommandText = "SELECT count(*) from biometrics;"
                Return clsGlobal.Any2Long(SQLcommand.ExecuteScalar())

                SQLcommand.Dispose()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBiometricsTbl::FingerprintsCount:", ex)
                Return -1
            End Try
        End Function


#End Region



    End Class
End Namespace