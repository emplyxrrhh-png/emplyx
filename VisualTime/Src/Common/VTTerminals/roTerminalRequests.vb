Imports System.Drawing
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTRequests
Imports Robotics.VTBase

Namespace VTTerminals

    ' 20180721 - Clases para gestión de solicitudes en terminal
    ' Son heredadas directamente de las usadas en mx8.
    ' Se deberían reescribir, definiendo sus DTO's y managers correspondientes ...

    Public Class roTerminalRequest

        ' Solicitud
        Private _Request As Requests.roRequest
        Private _Result As ResultEnum

        'Variable globales entre las solicitudes
        Private _IDEmployee As Integer = 0
        Private _RequestType As eRequestType
        Private _RequestTimeStamp As DateTime = Now
        Private _RequestStatus As eRequestStatus = eRequestStatus.Pending

        'Variables de tiempo
        Private _Date1 As Nullable(Of DateTime)
        Private _Date2 As Nullable(Of DateTime)
        Private _Time1 As Nullable(Of DateTime)
        Private _Time2 As Nullable(Of DateTime)
        Private _MaxTime As Nullable(Of DateTime)
        'Valores por defecto
        Private _Date1DefaultValue As Nullable(Of DateTime) = Now
        Private _Date2DefaultValue As Nullable(Of DateTime) = Now
        Private _Time1DefaultValue As Nullable(Of DateTime) = Nothing
        Private _Time2DefaultValue As Nullable(Of DateTime) = Nothing
        Private _MaxTimeDefaultValue As String = ""

        'Variable necesarias dependiendo del tipo de solicitud
        Private _IDShiftGroup As Nullable(Of Integer)
        Private _IDShift As Nullable(Of Integer)
        Private _ShiftName As String = ""
        Private _IDCause As Nullable(Of Integer)
        Private _CauseName As String = ""
        'Valores por defecto
        Private _IDShiftDefaultValue As Nullable(Of Integer)
        Private _IDCauseDefaultValue As Nullable(Of Integer)
        Private oState As roTerminalsState

        'Estado de validación
        Public Enum ResultEnum
            NoError
            Exception
            ConnectionError
            SqlError
            NoDeleteBecauseNotPending
            IncorrectDates
            NoApprovePermissions
            UserFieldNoRequestVisible ' El campo de la ficha no es visible para solicitudes
            NoApproveRefuseLevelOfAuthorityRequired ' No se puede aprobar/denegar la solicitud ya que un nivel de mando de rango superior ya la ha aprobado/denegado
            UserFieldValueSaveError ' No se ha podido guardar el valor del campo de la ficha
            InvalidPassport
            ChangeShiftError ' No se ha podido cambiar la planificación del horario
            VacationsOrPermissionsError ' No se ha podido planificar el periodo  de vacaciones
            ExistsLockedDaysInPeriod ' Hay días bloqueados en el periodo a planificar
            ForbiddenPunchError ' Error al validar el fichaje olvidado
            JustifyPunchError ' Error al validar la justificación del fichaje
            RequestMoveNotExist ' El fichaje relacionado con la solicitud no existe o se ha modificado
            RequestMoveTooMany  ' Hay más de un fichaje relacionado con la solicitud
            PlannedAbsencesError ' Error al validar la solicitud de ausencia prolongada
            PlannedCausesError ' Error al validar la solicitud de incidencia prevista
            ExternalWorkResumePartError ' Error al validar la solicitud de parte de trabajo externo
            UserFieldRequired ' Campo de la ficha requerido
            PunchDateTimeRequired ' Fecha y hora del fichaje requerida
            CauseRequired ' Justificación requerida
            DateRequired ' Fecha requerida
            HoursRequired ' Horas requeridas
            ShiftRequired ' Horario requerido
            RequestRepited ' Solicitud repetida (ya existe una solicitud con el mismo tipo de solicitud y el mismo empleado en los dos últimos segundos)
            PunchExist ' Ya existe un fichaje con la misma fecha y hora
            StartShiftRequired ' Inicio de horario flotante requerido
            PlannedCausesOverlapped 'Solicitudes de Horas de Ausencia solapadas
            PlannedAbsencesOverlapped 'Solicitudes de Dias de ausencia solapadas
            TaskRequiered ' Tarea requerida
            NotEnoughConceptBalance ' No tiene saldo de vacaciones suficiente
        End Enum

        ''' <summary>
        ''' ID del empleado que solicita el request.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IDEmployee() As Integer
            Get
                Return _IDEmployee
            End Get
            Set(ByVal Value As Integer)
                _IDEmployee = Value
            End Set
        End Property

        ''' <summary>
        ''' Tipo se solicitud.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RequestType() As eRequestType
            Get
                Return _RequestType
            End Get
            Set(ByVal Value As eRequestType)
                _RequestType = Value
            End Set
        End Property

        ''' <summary>
        ''' Fecha y hora de cuando se ha creado la solicitud.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RequestTimeStamp() As DateTime
            Get
                Return _RequestTimeStamp
            End Get
            Set(ByVal Value As DateTime)
                _RequestTimeStamp = Value
            End Set
        End Property

        ''' <summary>
        ''' Estado de la solicitud. El valor inicial siempre es pendiente.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RequestStatus() As eRequestStatus
            Get
                Return _RequestStatus
            End Get
            Set(ByVal Value As eRequestStatus)
                _RequestStatus = Value
            End Set
        End Property

        ''' <summary>
        ''' Fecha inicial.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Fecha usada en las solicitudes de:
        ''' Cambio de horario
        ''' Dias de ausencia
        ''' Presivión de horas
        ''' </remarks>
        Public Property Date1() As Nullable(Of DateTime)
            Get
                Return _Date1
            End Get
            Set(ByVal Value As Nullable(Of DateTime))
                If Value.HasValue Then _Date1DefaultValue = Value
                _Date1 = Value
            End Set
        End Property

        Public ReadOnly Property Date1DefaultValue() As Nullable(Of DateTime)
            Get
                If Not _Date1DefaultValue.HasValue Then _Date1DefaultValue = Now
                Return _Date1DefaultValue
            End Get
        End Property

        Public Property Date2() As Nullable(Of DateTime)
            Get
                Return _Date2
            End Get
            Set(ByVal Value As Nullable(Of DateTime))
                If Value.HasValue Then _Date2DefaultValue = Value
                _Date2 = Value
            End Set
        End Property

        Public ReadOnly Property Date2DefaultValue() As Nullable(Of DateTime)
            Get
                ' La fecha fin, si no está informada, toma por defecto el valor de la fecha inicial
                If Not _Date2DefaultValue.HasValue Then
                    If _Date1.HasValue Then
                        _Date2DefaultValue = Date1
                    Else
                        _Date2DefaultValue = Now
                    End If
                End If
                Return _Date2DefaultValue
            End Get
        End Property

        Public Property Time1() As Nullable(Of DateTime)
            Get
                Return _Time1
            End Get
            Set(ByVal Value As Nullable(Of DateTime))
                If Value.HasValue Then
                    _Time1DefaultValue = Value
                    UpdateMaxTimeDefaultValue()
                End If
                _Time1 = Value
            End Set
        End Property

        Public ReadOnly Property Time1DefaultValue() As Nullable(Of DateTime)
            Get
                If Not _Time1DefaultValue.HasValue Then
                    Return TimeSerial(0, 0, 0)
                Else
                    Return _Time1DefaultValue
                End If
            End Get
        End Property

        Public Property Time2() As Nullable(Of DateTime)
            Get
                Return _Time2
            End Get
            Set(ByVal Value As Nullable(Of DateTime))
                _Time2 = Value
                If Value.HasValue Then
                    _Time2DefaultValue = Value
                    UpdateMaxTimeDefaultValue()
                End If
            End Set
        End Property

        Public ReadOnly Property Time2DefaultValue() As Nullable(Of DateTime)
            Get
                'La hora final, si no está informada, toma por valor el de la hora inicial, si está informada
                If Not _Time2DefaultValue.HasValue Then
                    If _Time1.HasValue Then
                        Return _Time1
                    Else
                        Return TimeSerial(23, 59, 0)
                    End If
                Else
                    Return _Time2DefaultValue
                End If
            End Get
        End Property

        Public Property MaxTime() As Nullable(Of DateTime)
            Get
                Return _MaxTime
            End Get
            Set(ByVal Value As Nullable(Of DateTime))
                If Value.HasValue Then _MaxTimeDefaultValue = Value
                _MaxTime = Value
            End Set
        End Property

        Public ReadOnly Property MaxTimeDefaultValue() As String
            Get
                UpdateMaxTimeDefaultValue()
                Return _MaxTimeDefaultValue
            End Get
        End Property

        Private Sub UpdateMaxTimeDefaultValue()
            If Time1.HasValue AndAlso Time2.HasValue Then
                Dim tmpTime1 As DateTime = Time1
                Dim tmpTime2 As DateTime = Time2
                'Límites
                Dim TS As TimeSpan
                Dim hour As Integer
                Dim mins As Integer
                If tmpTime1 > tmpTime2 Then
                    tmpTime2 = tmpTime2.AddDays(1)
                End If
                TS = tmpTime2.AddMinutes(1) - tmpTime1
                hour = TS.Hours
                mins = TS.Minutes
                _MaxTimeDefaultValue = "00.00-" + (hour.ToString("00") & "." + mins.ToString("00"))
                'Valor por defecto
                TS = tmpTime2 - tmpTime1
                hour = TS.Hours
                mins = TS.Minutes
                _MaxTimeDefaultValue += "-" + (hour.ToString("00") & "." + mins.ToString("00"))
            Else
                _MaxTimeDefaultValue = "00.00-23.59-23.59"
            End If
        End Sub

        Public Property IDShiftGroup() As Integer
            Get
                Return _IDShiftGroup
            End Get
            Set(ByVal Value As Integer)
                _IDShiftGroup = Value
            End Set
        End Property

        Public Property IDShift() As Nullable(Of Integer)
            Get
                Return _IDShift
            End Get
            Set(ByVal Value As Nullable(Of Integer))
                If Value.HasValue Then _IDShiftDefaultValue = Value
                _IDShift = Value
                _ShiftName = ""
            End Set
        End Property

        Public ReadOnly Property IDShiftDefaultValue() As Nullable(Of Integer)
            Get
                Return _IDShiftDefaultValue
            End Get
        End Property

        Public ReadOnly Property ShiftName() As String
            Get
                If _ShiftName.Length = 0 Then
                    Dim state As New Shift.roShiftState
                    Dim shift As New Shift.roShift(_IDShift, state)
                    _ShiftName = shift.Name
                End If
                Return _ShiftName
            End Get
        End Property

        Public Property IDCause() As Nullable(Of Integer)
            Get
                Return _IDCause
            End Get
            Set(ByVal Value As Nullable(Of Integer))
                If Value.HasValue Then _IDCauseDefaultValue = Value
                _IDCause = Value
                _CauseName = ""
            End Set
        End Property

        Public ReadOnly Property IDCauseDefaultValue() As Nullable(Of Integer)
            Get
                Return _IDCauseDefaultValue
            End Get
        End Property

        Public ReadOnly Property CauseName() As String
            Get
                If _CauseName.Length = 0 Then
                    Dim state As New Cause.roCauseState
                    Dim cause As New Cause.roCause(_IDCause, state)
                    _CauseName = cause.Name
                End If
                Return _CauseName
            End Get
        End Property

        Public ReadOnly Property Result() As ResultEnum
            Get
                Return _Result
            End Get
        End Property

        Public Sub New(ByVal IDEmployee As Integer, oTState As roTerminalsState)
            oState = oTState
            _IDEmployee = IDEmployee
            Clear()
        End Sub

        Public Sub Clear()
            Try
                _RequestTimeStamp = Now
                _RequestStatus = eRequestStatus.Pending
                _Date1 = Nothing
                _Date1DefaultValue = Nothing
                _Date2 = Nothing
                _Date2DefaultValue = Nothing
                _Time1 = Nothing
                _Time1DefaultValue = Nothing
                _Time2 = Nothing
                _Time2DefaultValue = Nothing
                _MaxTime = Nothing
                _MaxTimeDefaultValue = Nothing
                _IDShiftGroup = Nothing
                _IDShift = -1
                _IDShiftDefaultValue = Nothing
                _ShiftName = ""
                _IDCause = Nothing
                _IDCauseDefaultValue = Nothing
                _CauseName = ""
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "clsTerminalRequest::Clear::Error:", ex)
            End Try
        End Sub

        Public Function Save() As Boolean
            Try
                _Request = New Requests.roRequest
                _Request.ID = -1
                _Request.RequestDate = Now
                _Request.IDEmployee = Me.IDEmployee
                _Request.RequestStatus = eRequestStatus.Pending
                _Request.Comments = ""

                Select Case Me.RequestType
                    Case eRequestType.PlannedAbsences
                        _Request.RequestType = eRequestType.PlannedAbsences
                        _Request.IDCause = Me.IDCause
                        _Request.Date1 = Me.Date1
                        _Request.Date2 = Me.Date2
                    Case eRequestType.PlannedCauses
                        _Request.RequestType = eRequestType.PlannedCauses
                        _Request.IDCause = Me.IDCause
                        _Request.Date1 = Me.Date1
                        _Request.Date2 = Me.Date2
                        _Request.FromTime = DateSerial(1899, 12, 30).AddHours(Me.Time1.Value.Hour).AddMinutes(Me.Time1.Value.Minute)
                        If Me.Time1 < Me.Time2 Then
                            _Request.ToTime = DateSerial(1899, 12, 30).AddHours(Me.Time2.Value.Hour).AddMinutes(Me.Time2.Value.Minute)
                        Else
                            _Request.ToTime = DateSerial(1899, 12, 31).AddHours(Me.Time2.Value.Hour).AddMinutes(Me.Time2.Value.Minute)
                        End If
                        _Request.Hours = Robotics.VTBase.roTypes.Any2Time(Me.MaxTime.Value.ToShortTimeString).NumericValue
                    Case eRequestType.ChangeShift
                        _Request.RequestType = eRequestType.ChangeShift
                        _Request.IDCause = Me.IDCause
                        _Request.Date1 = Me.Date1
                        _Request.Date2 = Me.Date2
                        _Request.IDShift = Me.IDShift
                    Case eRequestType.VacationsOrPermissions
                        _Request.RequestType = eRequestType.VacationsOrPermissions
                        _Request.IDShift = Me.IDShift
                        _Request.Date1 = Me.Date1
                        _Request.Date2 = Me.Date2
                    Case eRequestType.ForbiddenPunch
                        _Request.RequestType = eRequestType.ForbiddenPunch
                        _Request.Date1 = Me.Date1
                End Select
                If _Request.Save(True) Then
                    Return True
                Else
                    Select Case _Request.State.Result
                        Case RequestResultEnum.PlannedAbsencesOverlapped
                            _Result = ResultEnum.PlannedAbsencesOverlapped
                        Case RequestResultEnum.PlannedCausesOverlapped
                            _Result = ResultEnum.PlannedCausesOverlapped
                        Case RequestResultEnum.IncorrectDates
                            _Result = ResultEnum.IncorrectDates
                        Case Else
                            _Result = _Request.State.Result
                    End Select
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "clsTerminalRequest::Save::Error:", ex)
                Return False
            End Try
        End Function

        Public Function Delete(ByVal id As Integer, oState As roTerminalsState)
            Try
                Dim oRequestState As New Requests.roRequestState
                _Request = New Requests.roRequest(id, oRequestState)
                _Request.Delete()
                Select Case oRequestState.Result
                    Case RequestResultEnum.NoError
                        Return True
                    Case Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "clsTerminalRequest::Delete::Error deleting request id=" + id.ToString + ". Detail: " + oRequestState.Result.ToString)
                        Return False
                End Select
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "clsTerminalRequest::Delete::Error:", ex)
                Return False
            End Try
        End Function

        Public Function GetAvailableShifts(ByVal OnlyHolidays As Boolean) As Dictionary(Of Integer, String)
            Try
                Dim oTable As DataTable
                Dim oShiftState As New Shift.roShiftState
                Dim ret As New Dictionary(Of Integer, String)
                oTable = Shift.roShift.GetShiftsByEmployeeVisibilityPermissions(_IDEmployee, oShiftState, , , , True)
                For Each oRow As DataRow In oTable.Rows
                    If OnlyHolidays Then
                        ' Horarios para vacaciones
                        If Not IsDBNull(oRow.Item("ShiftType")) AndAlso oRow.Item("ShiftType") = 2 Then
                            ret.Add(oRow.Item("ID"), oRow.Item("Name").ToString)
                        End If
                    Else
                        ' Horarios para cambio de turno
                        If IsDBNull(oRow.Item("ShiftType")) OrElse oRow.Item("ShiftType") = 0 OrElse oRow.Item("ShiftType") = 1 Then
                            ret.Add(oRow.Item("ID"), oRow.Item("Name").ToString)
                        End If
                    End If
                Next
                Return ret
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "clsTerminalRequest::GetShifts::Error:", ex)
                Return Nothing
            End Try
        End Function

    End Class

    Public Class roTerminalSchedulingData
        Private m_Rnd As New Random
        Public Status As Long
        Public EndDate As Date
        Public DayColors As String()
        Public DayNames As String()
        Public AbsencesColors As String()

        Public Function RandomRGBColor() As System.Drawing.Color
            Return System.Drawing.Color.FromArgb(255,
            m_Rnd.Next(0, 255),
            m_Rnd.Next(0, 255),
            m_Rnd.Next(0, 255))
        End Function

    End Class

    Public Class roTerminalRequestsList
        Public Status As Long
        Public CanCreateRequest As Boolean
        Public Requests As roTerminalRequestDetail()
    End Class

    Public Class roTerminalRequestDetail
        Public Name As String
        Public IcoStatus As String
        Public NameStatus As String
        Public RequestType As String
        Public NotReaded As Boolean
        Public RequestDate As Date
        Public Id As Integer
        Public Title As String
        Public ObjectDateStart As Date
        Public ObjectDateEnd As Date
        Public ObjectDateDuration As Integer
        Public Requests As String()
        Public RequestedCauseName As String
        Public RequestedShiftName As String
        Public ObjectHourStart As Date
        Public ObjectHourEnd As Date
        Public RequestedHours As Integer
        Public RequestedFieldName As String
    End Class

    Public Class roTerminalHolidaysInfo
        Public Shifts As roTerminalHolidaysResumeShifts()
        Public Status As Long
    End Class

    Public Class roTerminalHolidaysResumeShifts
        Public VacationsShiftName As String
        Public VacationsResumeHeaders As String()
        Public VacationsResumeValue As Double()
    End Class

    Public Class roTerminalAccrual

        Public Enum AccrualType
            Hours
            Ocurrences
        End Enum

        Public Enum AccrualQueryPeriod
            Month
            Year
            Week
            Contract
        End Enum

        Private intID As Integer
        Private strName As String
        Private oType As AccrualType
        Private oQueryPeriod As AccrualQueryPeriod
        Private dblValue As Double

        Public Sub New(ByVal _ID As Integer, ByVal _Name As String, ByVal _Type As AccrualType, ByVal _QueryPeriod As AccrualQueryPeriod, Optional ByVal _Value As Double = 0)

            Me.intID = _ID
            Me.strName = _Name
            Me.oType = _Type
            Me.oQueryPeriod = _QueryPeriod
            Me.dblValue = _Value

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _Name As String, ByVal _strType As String, ByVal _strQueryPeriod As String, Optional ByVal _Value As Double = 0)

            Me.intID = _ID
            Me.strName = _Name
            Select Case _strType
                Case "H"
                    Me.oType = AccrualType.Hours
                Case "O"
                    Me.oType = AccrualType.Ocurrences
            End Select
            Select Case _strQueryPeriod
                Case "M"
                    Me.oQueryPeriod = AccrualQueryPeriod.Month
                Case "Y"
                    Me.oQueryPeriod = AccrualQueryPeriod.Year
            End Select
            Me.dblValue = _Value

        End Sub

        Public ReadOnly Property ID() As Integer
            Get
                Return Me.intID
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return Me.strName
            End Get
        End Property

        Public ReadOnly Property Type() As AccrualType
            Get
                Return Me.oType
            End Get
        End Property

        Public ReadOnly Property QueryPeriod() As AccrualQueryPeriod
            Get
                Return Me.oQueryPeriod
            End Get
        End Property

        Public Property Value() As Double
            Get
                Return Me.dblValue
            End Get
            Set(ByVal value As Double)
                Me.dblValue = value
            End Set
        End Property

    End Class

    Public Class roTerminalShift

        Private xShiftdate As DateTime
        Private intID As Integer
        Private strName As String
        Private oColor As Color
        Private strShortName As String

        Private oAlterShifts As roTerminalShift()

        Public Sub New(ByVal _Shiftdate As DateTime, ByVal _ID As Integer, ByVal _Name As String, ByVal _ShortName As String, ByVal _Color As Integer)

            Me.xShiftdate = _Shiftdate
            Me.intID = _ID
            Me.strName = _Name
            Me.strShortName = _ShortName
            'oColor = Color.FromArgb(_Color)

            Dim r, g, b As Byte

            r = _Color And 255
            g = (_Color \ 256) And 255
            b = (_Color \ 65536) And 255

            oColor = Color.FromArgb(r, g, b)

            ReDim Me.oAlterShifts(-1)

        End Sub

        Public ReadOnly Property Shiftdate() As DateTime
            Get
                Return Me.xShiftdate
            End Get
        End Property

        Public ReadOnly Property ID() As Integer
            Get
                Return Me.intID
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return Me.strName
            End Get
        End Property

        Public ReadOnly Property ShortName() As String
            Get
                Return Me.strShortName
            End Get
        End Property

        Public ReadOnly Property Color() As Color
            Get
                Return Me.oColor
            End Get
        End Property

        Public ReadOnly Property AlterShifts() As roTerminalShift()
            Get
                Return Me.oAlterShifts
            End Get
        End Property

        Public Sub AddAlterShift(ByVal _Shiftdate As DateTime, ByVal _ID As Integer, ByVal _Name As String, ByVal _ShortName As String, ByVal _Color As Integer)

            ReDim Preserve Me.oAlterShifts(Me.oAlterShifts.Length)

            Me.oAlterShifts(Me.oAlterShifts.Length - 1) = New roTerminalShift(_Shiftdate, _ID, _Name, _ShortName, _Color)

        End Sub

    End Class

    Public Class roTerminalRequestHelper

        Public Shared Function CheckResultFilter(ByVal showAll As Boolean, ByVal dateStart As Date, ByVal dateEnd As Date, ByVal filter As String) As String
            Dim strResult As String = ""
            If showAll Then Return strResult

            If dateStart = Nothing And dateEnd = Nothing Then 'Sense dates
            ElseIf dateStart = Nothing And dateEnd <> Nothing Then 'Data fi
                strResult = "RequestDate <= " & roTypes.SQLDateTime(dateEnd) & " AND "
            ElseIf dateStart <> Nothing And dateEnd <> Nothing Then 'Data inici i data fi
                strResult = "RequestDate Between " & roTypes.SQLDateTime(dateStart) & " AND " & roTypes.SQLDateTime(dateEnd) & " AND "
            ElseIf dateStart <> Nothing And dateEnd = Nothing Then ' data inici
                strResult = "RequestDate >= " & roTypes.SQLDateTime(dateStart) & " AND "
            End If

            If filter <> "" AndAlso filter.IndexOf("|") >= 0 Then
                Dim arrFilter() As String = filter.Split("|")
                Dim arrStatus() As String = arrFilter(0).Split("*")
                Dim arrRequestType() As String = arrFilter(1).Split("*")

                If arrFilter(0).ToString.Trim <> "" Then
                    If arrStatus.Length > 0 Then strResult &= "Status IN("
                    For Each strStat As String In arrStatus
                        strResult &= strStat & ","
                    Next
                    If arrStatus.Length > 0 Then strResult = strResult.Substring(0, strResult.Length - 1) & ") AND "
                End If

                If arrFilter(1).ToString.Trim <> "" Then
                    If arrRequestType.Length > 0 Then strResult &= "RequestType IN("
                    For Each strRT As String In arrRequestType
                        strResult &= strRT & ","
                    Next
                    If arrRequestType.Length > 0 Then strResult = strResult.Substring(0, strResult.Length - 1) & ") AND "
                End If
            Else
                If filter <> "" Then
                    ' Filtro directo
                    strResult &= filter
                End If
            End If
            If strResult.EndsWith(" AND ") Then strResult = strResult.Substring(0, strResult.Length - 4)
            Return strResult
        End Function

        Public Shared Function GetRequestDetail(ByVal oRD As roTerminalRequestDetail) As String
            'Tipo de solicitud: ${1}
            'Fecha de solicitud:${2}
            'Estado de la solicitud: ${3}
            'Fecha de inicio solicitada:${4}
            'Fecha de fin solicitada: ${5}
            'Hora de inicio solicitada: ${6}
            'Hora de fin solicitada: ${7}
            'Número de horas: ${8}
            'Duración en días: ${9}
            'Justificación: ${10}
            'Horario: ${11}
            'Hora del fichaje olvidado: ${12}
            'Campo de la ficha: ${13}

            Select Case oRD.RequestType
                Case "ChangeShift"
                    Return "Portal.req.ChangeShift.Detail" + "#" + "Horario ${11} del ${4} al ${5} (${9} días)"
                Case "VacationsOrPermissions"
                    Return "Portal.req.VacationsOrPermissions.Detail" + "#" + "${11} del ${4} al ${5} (${9} días)"
                Case "PlannedAbsences"
                    Return "Portal.req.PlannedAbsences.Detail" + "#" + "${10} del ${4} al ${5}"
                Case "PlannedCauses"
                    Return "Portal.req.PlannedCauses.Detail" + "#" + "${10} del ${4} al ${5} entre ${6} y ${7} ${8} horas"
                Case "ExchangeShiftBetweenEmployees"
                Case "ExternalWorkResumePart"
                    Return "Portal.req.ExternalWorkResumePart.Detail" + "#" + "${10} del ${4} al ${5} entre ${6} y ${7} ${8} horas"
                Case "ForbiddenPunch"
                    If Not oRD.RequestedCauseName Is Nothing Then
                        Return "Portal.req.ForbiddenPunch.Detail" + "#" + "El día ${4} a las ${12} por ${10}"
                    Else
                        Return "Portal.req.ForbiddenPunchWithoutCause.Detail" + "#" + "El día ${4} a las ${12}"
                    End If
                Case "ForbiddenTaskPunch"
                    Return "Portal.req.ForbiddenTaskPunch.Detail" + "#" + "El día ${4} a las ${12}"
                Case "JustifyPunch"
                    Return "Portal.req.JustifyPunch.Detail" + "#" + "El día ${4} a las ${12} por ${10}"
                Case "UserFieldsChange"
                    Return "Portal.req.UserFieldsChange.Detail" + "#" + "Campo de la ficha ${13}"
            End Select
            Return ""
        End Function

    End Class

End Namespace