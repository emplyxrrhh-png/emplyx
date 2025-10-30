Imports System.Data.Common
Imports System.Drawing
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Public Class roTeam

#Region "Declarations - Constructor"

    Private intID As Integer
    Private intIDCard As Integer
    Private strName As String
    Private strDescription As String
    Private oTeamImage As Image

#End Region

#Region "Properties"

    Public Property ID() As Integer
        Get
            Return Me.intID
        End Get
        Set(ByVal value As Integer)
            Me.intID = value
        End Set
    End Property

    Public Property IDCard() As Integer
        Get
            Return Me.intIDCard
        End Get
        Set(ByVal value As Integer)
            Me.intIDCard = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return Me.strName
        End Get
        Set(ByVal value As String)
            Me.strName = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return Me.strDescription
        End Get
        Set(ByVal value As String)
            Me.strDescription = value
        End Set
    End Property

    Public ReadOnly Property TeamImage() As Image
        Get
            Return Me.oTeamImage
        End Get
    End Property

#End Region

#Region "Methods"

    Public Function Load(ByRef LogHandle As roLog) As Boolean
        '
        ' Carga datos del equipo de un intID.
        '
        '
        Dim bolRet As Boolean = False
        Dim sSQL As String
        Dim rdPositioned As DbDataReader

        Try

            If Me.intID <> 0 Then

                sSQL = "@SELECT# * FROM Teams WHERE [ID] = " & Me.intID
                rdPositioned = CreateDataReader(sSQL)

                If rdPositioned.Read Then

                    Me.strName = Any2String(rdPositioned("Name"))
                    Me.strDescription = Any2String(rdPositioned("Description"))

                    Me.intIDCard = Any2Integer(rdPositioned("IDCard"))

                    Me.oTeamImage = My.Resources.Team256

                    bolRet = True
                Else
                    bolRet = False
                End If

                rdPositioned.Close()
            Else
                bolRet = False
            End If
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "Unexpected error on LoadFromActiveDataset, Team '" & Me.intID & "', error: ", ex)
            Me.intID = 0
            bolRet = False
        End Try

        Return bolRet

    End Function

    Public Function GetTeamIDFromCardID(ByRef LogHandle As roLog, ByVal CardID As String) As Integer
        '
        ' Recupera el ID de un equipo pasandole el ID de Tarjeta
        '  Si no encuentra el equipo especificado devuelve -1
        '
        Dim intRet As Integer = -1
        Dim sSQL As String
        Dim aDate As New roTime
        Dim Value As Object
        Dim CardString As String

        Try

            aDate.Value = Now

            ' Convertir código tarjeta
            CardID = Me.DataReader2VTValue(CardID)

            ' Si la tarjeta tiene un alias, obtiene ahora
            'Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE CONVERT(decimal(20,0),RealValue)=" & CardID, ActiveConnection)
            Dim strCard As String = CardID
            Dim bolCardAliases As Boolean
            If strCard.Length Mod 4 > 0 Then strCard = strCard.PadLeft(strCard.Length + (4 - (strCard.Length Mod 4)), "0")
            Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE RealValue LIKE '%" & strCard & "'")
            If Value IsNot Nothing Then
                CardString = CStr(Value)
                bolCardAliases = True
            Else
                CardString = CardID
                bolCardAliases = False
            End If

            'Creamos la sentencia SQL
            sSQL = "@SELECT# [ID] FROM Teams WHERE "
            If bolCardAliases Then
                sSQL &= "IDCard = " & CardString
            Else
                sSQL &= "CONVERT(varchar, IDCard) LIKE '%" & strCard & "'"
            End If

            intRet = Any2Double(ExecuteScalar(sSQL))

            If intRet = 0 Then
                sSQL = "@SELECT# [ID] FROM Teams WHERE "
                sSQL &= "CONVERT(varchar, IDCard) LIKE '%" & Val(strCard) & "'"
                intRet = Any2Double(ExecuteScalar(sSQL))
            End If
        Catch ex As Exception
            intRet = -1
        End Try

        Return intRet

    End Function

    Public Function GetTeamIDFromCardIDV1(ByRef LogHandle As roLog, ByVal CardID As String) As Integer
        '
        ' Recupera el ID de un equipo pasandole el ID de Tarjeta
        '  Si no encuentra el equipo especificado devuelve -1
        '
        Dim intRet As Integer = -1
        Dim sSQL As String
        Dim aDate As New roTime
        Dim Value As Object
        Dim CardString As String

        Try

            aDate.Value = Now

            If Not IsNumeric(CardID) Then
                CardID = Convert.ToUInt64(CardID, 16)
            End If

            ' Si la tarjeta tiene un alias, obtiene ahora
            'Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE CONVERT(decimal(20,0),RealValue)=" & CardID, ActiveConnection)
            Dim strCard As String = CardID
            Dim bolCardAliases As Boolean
            If strCard.Length Mod 4 > 0 Then strCard = strCard.PadLeft(strCard.Length + (4 - (strCard.Length Mod 4)), "0")
            Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE RealValue LIKE '%" & strCard & "'")
            If Value IsNot Nothing Then
                CardString = CStr(Value)
                bolCardAliases = True
            Else
                CardString = CardID
                bolCardAliases = False
            End If

            'Creamos la sentencia SQL
            sSQL = "@SELECT# [ID] FROM Teams WHERE "
            If bolCardAliases Then
                sSQL &= "IDCard = " & CardString
            Else
                sSQL &= "CONVERT(varchar, IDCard) LIKE '%" & strCard & "'"
            End If

            intRet = Any2Double(ExecuteScalar(sSQL))

            If intRet = 0 Then
                sSQL = "@SELECT# [ID] FROM Teams WHERE "
                sSQL &= "CONVERT(varchar, IDCard) LIKE '%" & CLng(strCard) & "'"
                intRet = Any2Double(ExecuteScalar(sSQL))
            End If
        Catch ex As Exception
            intRet = -1
        End Try

        Return intRet

    End Function

    Public Function GetLastIDJob() As Long
        '
        'Retorna el último trabajo donde se ha estado sin incidencia
        '

        Dim sSQL As String
        Dim ads As DbDataReader

        sSQL = "@SELECT# top 1 * FROM TeamJobMoves WHERE IDTeam = " & Me.intID
        sSQL = sSQL & " AND InDateTime is Not Null AND OutDateTime is Not Null"
        '    sSQL = sSQL & " AND IDIncidence = 0"
        sSQL = sSQL & " Order by OutDateTime DESC,ID DESC "

        GetLastIDJob = 0

        ads = CreateDataReader(sSQL)
        If ads.Read Then
            GetLastIDJob = Any2Double(ads.Item("IDJob"))
            ads.Close()
            ads = Nothing
            Exit Function
        End If
        ads.Close()
        ads = Nothing

    End Function

    Public Function AssignEmployee(ByVal intIDEmployee As Integer, ByVal xAssignedDate As DateTime, ByVal oLog As roLog) As Boolean
        '
        ' Recupera el ID de un Equipo pasandole el ID de Tarjeta
        '  Si no encuentra el equipo especificado devuelve -1
        '
        Dim bolRet As Boolean = False
        Dim sSQL As String
        Dim ActualTeamID As Double
        Dim ActualFinishDate As New roTime
        Dim NewJobEmployee As Boolean

        Try

            NewJobEmployee = False

            'Primero buscamos el Equipo donde esta asignado actualmente
            sSQL = "@SELECT# IDTeam,FinishDate FROM EmployeeTeams WHERE IDEmployee= " & intIDEmployee
            sSQL = sSQL & " AND BeginDate <= " & Any2Time(Now).SQLSmallDateTime
            sSQL = sSQL & " AND FinishDate >= " & Any2Time(Now).SQLSmallDateTime
            Dim rd As DbDataReader = CreateDataReader(sSQL)
            If rd.Read Then
                ActualTeamID = Any2Double(rd("IDTeam"))
                ActualFinishDate = Any2Time(rd("FinishDate"))
            End If
            rd.Close()

            If ActualTeamID <> Me.intID Then

                If Any2String(ExecuteScalar("@SELECT# Type FROM Employees WHERE ID = " & intIDEmployee)) <> "J" Then
                    'Si el empledo no era de produccion lo cambiamos
                    ExecuteSql("@UPDATE# Employees SET Type='J' WHERE ID=" & intIDEmployee)
                    NewJobEmployee = True
                End If

                'Cerramos la asignacion al antiguo equipo
                sSQL = "@UPDATE# EmployeeTeams SET FinishDate = " & Any2Time(xAssignedDate.AddDays(-1)).SQLSmallDateTime & " WHERE IDEmployee= " & intIDEmployee
                sSQL = sSQL & " AND BeginDate <= " & Any2Time(Now).SQLSmallDateTime
                sSQL = sSQL & " AND FinishDate >= " & Any2Time(Now).SQLSmallDateTime
                ExecuteSql(sSQL)

                'Insertamos la nueva
                sSQL = "@INSERT# INTO EmployeeTeams (IDEmployee, IDTeam, BeginDate, FinishDate) "
                If ActualFinishDate.Value = Any2Time("01/01/2079").Value Or NewJobEmployee Then
                    sSQL = sSQL & " VALUES (" & intIDEmployee & "," & Me.intID & "," & Any2Time(xAssignedDate).SQLSmallDateTime & "," & Any2Time("01/01/2079").SQLSmallDateTime & ")"
                Else
                    sSQL = sSQL & " VALUES (" & intIDEmployee & "," & Me.intID & "," & Any2Time(xAssignedDate).SQLSmallDateTime & "," & ActualFinishDate.SQLSmallDateTime & ")"
                End If
                ExecuteSql(sSQL)

                'Notificamos qque se han producido cambios
                Dim strLastMoveInOut As String = ""
                Dim intLastMoveID As Integer
                Dim oLastJobMove As roTime = GetLastJobMoveDateTime(oLog, strLastMoveInOut, intLastMoveID)
                Me.ResetJobStatusForThisTeamMove(Any2Time(xAssignedDate).DateOnly, oLastJobMove.DateOnly, oLog)
                'LogHandle.Context(roVarDataOp) = roTableObject & ":\\TEAMJOBTIME"
                roConnector.InitTask(TasksType.TEAMJOBTIME)

                bolRet = True
            Else
                'ya esta asignado al equipo en cuestion no hacemos nada
                bolRet = True
            End If
        Catch Ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roTeam::AssignEmployee :", Ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roTeam::AssignEmployee :", ex)
        Finally

        End Try

        Return bolRet

    End Function

    Private Function ResetJobStatusForThisTeamMove(ByVal xDateTimeIN As DateTime, ByVal xDateTimeOUT As DateTime, ByVal oLog As roLog) As Boolean
        '
        ' Marca como no procesado los dias del DailySchedule de todos los empleados a
        '  los que afecta este movimiento de producción de equipo.
        '
        Dim bolRet As Boolean = False
        Dim sSQL As String

        Try

            ' Como versión preliminar, marcamos como no calculados desde el dia antes hasta el dia
            '  despues, asi nos curamos en salud.
            xDateTimeIN = xDateTimeIN.AddDays(-1)
            xDateTimeOUT = xDateTimeOUT.AddDays(1)

            ' Obtenemos todos los empleados que están en el equipo durante este movimiento
            sSQL = "@SELECT# IDEmployee,BeginDate,IsNull(FinishDate," & Any2Time(Now).SQLDateTime & ") AS MyFinishDate FROM EmployeeTeams WHERE " _
                    & "IDTeam=" & Me.intID & " AND " _
                    & "BeginDate<" & Any2Time(xDateTimeOUT).SQLDateTime &
                    " AND IsNull(FinishDate," & Any2Time(Now).SQLDateTime & ")>" & Any2Time(xDateTimeIN).SQLDateTime & " ORDER BY BeginDate"
            Dim rd As DbDataReader = CreateDataReader(sSQL)
            While rd.Read

                sSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET JobStatus=0 WHERE IDEmployee=" & rd("IDEmployee") &
                        " AND Date BETWEEN " & Any2Time(xDateTimeIN).SQLSmallDateTime & " AND " &
                        Any2Time(xDateTimeOUT).SQLDateTime
                ExecuteSql(sSQL)

            End While

            bolRet = True
        Catch Ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roTeam::ResetJobStatusForThisTeamMove :", Ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roTeam::ResetJobStatusForThisTeamMove :", ex)
        End Try

        Return bolRet

    End Function

    Public Function GetLastJobMoveDateTime(ByVal oLog As roLog, ByRef LastMoveInOut As String, ByRef LastMoveID As Double) As roTime
        ' Mira el último movimiento de produccion del Equipo y
        ' devuelve si es Inicio de Trabajo o final o inicio de Incidencia
        ' y el ID del movimiento
        Dim xRet As roTime = Nothing
        Dim LastInJob As New roTime
        Dim LastOutJob As New roTime
        Dim LastINJobIncidence As Integer
        Dim LastJobOut As New roTime
        Dim LastInJobID As Long
        Dim LastOutJobID As Long

        Try

            'Obtiene ultimo inicio de produccion
            LastINJobIncidence = 0
            Dim rd As DbDataReader = CreateDataReader("@SELECT# TOP 1 ID,InDateTime,IDIncidence FROM TeamJobMoves WHERE IDTeam=" & Me.intID & " AND OutDateTime IS Null ORDER BY InDateTime DESC")
            If rd.Read Then
                LastInJob = Any2Time(rd("InDateTime"))
                LastInJobID = rd("ID")
                LastINJobIncidence = rd("IDIncidence")
            Else
                LastInJob = Nothing
                LastInJobID = 0
                LastINJobIncidence = 0
            End If
            rd.Close()

            'Obtiene ultimo cierre de produccion
            rd = CreateDataReader("@SELECT# TOP 1 ID,OutDateTime,IDIncidence FROM TeamJobMoves WHERE IDTeam=" & Me.intID & " ORDER BY OutDateTime DESC")
            If rd.Read Then
                LastOutJob = Any2Time(rd("OutDateTime"))
                LastOutJobID = rd("ID")
            Else
                LastOutJob = Nothing
                LastOutJobID = 0
            End If
            rd.Close()

            If LastOutJob.NumericValue = 0 And LastInJob.NumericValue = 0 Then
                LastMoveInOut = S_FINISH
            Else
                LastMoveInOut = IIf(LastOutJob.NumericValue > LastInJob.NumericValue, S_FINISH, S_BEGIN)
            End If
        Catch Ex As DbException
            'oLog.logMessage(roLog.EventType.roError, "roTeam::GetLastJobMoveDateTime :", Ex)
            LastMoveID = 0
            LastMoveInOut = S_FINISH
        Catch ex As Exception
            'oLog.logMessage(roLog.EventType.roError, "roTeam::GetLastJobMoveDateTime :", ex)
            LastMoveID = 0
            LastMoveInOut = S_FINISH
        Finally
            ' Almacena los datos del ultimo fichaje
            If LastMoveInOut = S_BEGIN Then
                LastMoveID = LastInJobID
                If LastINJobIncidence <> 0 Then LastMoveInOut = S_BEGININCIDENCE
                xRet = LastInJob
            ElseIf LastMoveInOut = S_FINISH Then
                LastMoveID = LastOutJobID
                xRet = LastOutJob
            End If
        End Try

        Return xRet

    End Function

#Region "Cards conversion methods"

    Public Function DataReader2VTValue(ByVal strData As String) As Long
        '
        ' Convierte el código en hexadecimal que obtenemos del lector de poximidad a el código almacenado a VT
        '
        Dim intValue As Long = Convert.ToUInt64(strData, 16)

        strData = intValue.ToString

        strData = Binary(strData, 2)
        If strData.Length Mod 4 > 0 Then
            strData = strData.PadLeft(strData.Length + (4 - (strData.Length Mod 4)), "0")
        End If

        Dim strVTValue As String = ""
        For i As Integer = 0 To strData.Length - 1 Step 4
            strVTValue &= CStr(Deciml(strData.Substring(i, 4), 2)).PadLeft(2, "0")
        Next

        Return CLng(strVTValue)

    End Function

    Public Function VTValue2DataReader(ByVal lngVTValue As Long) As String

        Dim RealValueDecimal As String
        Dim RealValue As String = lngVTValue.ToString

        RealValueDecimal = ""
        Dim str As String
        For i As Integer = 1 To Len(RealValue) Step 2
            str = "0000" & Binary(Mid$(RealValue, i, 2), 2)
            RealValueDecimal &= str.Substring(str.Length - 4, 4)
        Next i

        Return Deciml(RealValueDecimal, 2)

    End Function

    Private Function Binary(ByVal InptD As Object, ByVal BaseD As Object) As String

        Dim g As Object

        Try
            Binary = ""
            g = InptD
            Do
                Binary = (g Mod BaseD) & Binary
                g = g \ BaseD
            Loop Until g = 0

            Exit Function
        Catch
            Binary = ""
        End Try

    End Function

    Private Function Deciml(ByVal InptB As Object, ByVal BaseB As Object) As String

        Dim b As Object
        Dim E As Object
        Dim F As Object
        Dim A As Object
        Dim C As Object
        Dim D As Object

        Try
            b = InptB
            E = 0
            F = 0

            'loop
            Do
                A = CStr(b).Substring(CStr(b).Length - 1, 1) ' Right(b, 1)
                b = CStr(b).Substring(0, CStr(b).Length - 1) ' Left(b, Len(b) - 1)

                C = BaseB ^ F
                D = A * C
                E = E + D

                F = F + 1 'counter
            Loop Until b = ""
            'end loop

            Deciml = E

            Exit Function
        Catch
            Deciml = ""
        End Try

    End Function

#End Region

#End Region

End Class