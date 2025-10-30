Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBudgets
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTHolidays
Imports Robotics.Base.VTNotifications
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace Requests

#Region "CLASS - roRequest"

    <DataContract()>
    Public Class roRequest

#Region "Declarations - Constructor"

        Private oState As roRequestState

        Private intID As Integer
        Private intIDEmployee As Integer
        Private iRequestType As eRequestType = eRequestType.UserFieldsChange
        Private dRequestDate As Date
        Private strComments As String
        Private iStatus As eRequestStatus
        Private iStatusLevel As Nullable(Of Integer)
        Private dDate1 As Nullable(Of Date)
        Private dDate2 As Nullable(Of Date)
        Private strFieldName As String
        Private strFieldValue As String
        Private intIDCause As Nullable(Of Integer)
        Private dblHours As Nullable(Of Double)
        Private intIDShift As Nullable(Of Integer)
        Private xStartShift As Nullable(Of DateTime)
        Private xFromTime As Nullable(Of DateTime)
        Private xToTime As Nullable(Of DateTime)
        Private intIDEmployeeExchange As Nullable(Of Integer)
        Private bolNotReaded As Boolean
        Private intIDTask1 As Nullable(Of Integer)
        Private intIDTask2 As Nullable(Of Integer)
        Private strNextLevelPassports As String
        Private strField1 As String
        Private strField2 As String
        Private strField3 As String
        Private dblField4 As Double
        Private dblField5 As Double
        Private dblField6 As Double
        Private bolCompletedTask As Nullable(Of Boolean)
        Private intIDCenter As Nullable(Of Integer)

        Private lstRequestApprovals As Generic.List(Of roRequestApproval)
        Private lstRequestDays As Generic.List(Of roRequestDay)

        Private bolAutomaticValidation As Boolean = False
        Private dValidationDate As Nullable(Of Date)
        Private dRejectedDate As Nullable(Of Date)
        Private strCRC As String

        Public Sub New()
            Me.oState = New roRequestState
            Me.ID = -1
            Me.lstRequestApprovals = New Generic.List(Of roRequestApproval)
            Me.lstRequestDays = New Generic.List(Of roRequestDay)

            Me.strNextLevelPassports = String.Empty
            Me.AutomaticValidation = False
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roRequestState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.ID = _ID
            Me.strNextLevelPassports = String.Empty
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Estado de la solicitud
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        <IgnoreDataMember>
        Public Property State() As roRequestState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roRequestState)
                Me.oState = value
            End Set
        End Property

        ''' <summary>
        ''' ID de la solicitud
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
            End Set
        End Property

        ''' <summary>
        ''' ID de Empleado que ha generado la solicitud
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <DataMember()>
        Public Property IDEmployee() As Integer
            Get
                Return intIDEmployee
            End Get
            Set(ByVal value As Integer)
                intIDEmployee = value
            End Set
        End Property

        ''' <summary>
        ''' Tipo de solicitud:<br />
        ''' - Cambio campos de la ficha<br />
        ''' - Marcaje olvidado<br />
        ''' - Justificación marcaje existente<br />
        ''' - Parte de trabajo externo<br />
        ''' - Cambio de horario<br />
        ''' - Vacaciones o permisos<br />
        ''' - Ausencias previstas o incidencias previstas<br />
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property RequestType() As eRequestType
            Get
                Return iRequestType
            End Get
            Set(ByVal value As eRequestType)
                iRequestType = value
            End Set
        End Property

        ''' <summary>
        ''' Fecha de la solicitud
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property RequestDate() As Date
            Get
                Return dRequestDate
            End Get
            Set(ByVal value As Date)
                dRequestDate = value
            End Set
        End Property

        ''' <summary>
        ''' Comentarios
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Comments() As String
            Get
                Return strComments
            End Get
            Set(ByVal value As String)
                strComments = value
            End Set
        End Property

        ''' <summary>
        ''' Estado de la solicitud:<br />
        ''' - Pendiente<br />
        ''' - En curso<br />
        ''' - Aceptada <br />
        ''' - Denegada<br />
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property RequestStatus() As eRequestStatus
            Get
                Return iStatus
            End Get
            Set(ByVal value As eRequestStatus)
                iStatus = value
            End Set
        End Property

        ''' <summary>
        ''' Nivel del estado de validación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property StatusLevel() As Nullable(Of Integer)
            Get
                Return iStatusLevel
            End Get
            Set(ByVal value As Nullable(Of Integer))
                iStatusLevel = value
            End Set
        End Property

        ''' <summary>
        ''' Fecha 1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Date1() As Nullable(Of Date)
            Get
                Return dDate1
            End Get
            Set(ByVal value As Nullable(Of Date))
                dDate1 = value
            End Set
        End Property

        <DataMember()>
        Public Property ValidationDate() As Nullable(Of Date)
            Get
                Return dValidationDate
            End Get
            Set(ByVal value As Nullable(Of Date))
                dValidationDate = value
            End Set
        End Property

        <DataMember()>
        Public Property RejectedDate() As Nullable(Of Date)
            Get
                Return dRejectedDate
            End Get
            Set(ByVal value As Nullable(Of Date))
                dRejectedDate = value
            End Set
        End Property

        ''' <summary>
        ''' Fecha 1 (cadena para jscript)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property strDate1() As String
            Get
                If dDate1 IsNot Nothing Then
                    Return dDate1.Value.Year.ToString & "-" & dDate1.Value.Month.ToString & "-" & dDate1.Value.Day.ToString & " " & dDate1.Value.Hour.ToString & ":" & dDate1.Value.Minute.ToString & ":" & dDate1.Value.Second.ToString
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" Then
                    Dim arrDateTime() As String = value.Split(" ")
                    Dim arrDate() As String = arrDateTime(0).Split("-")
                    Dim arrTime() As String = {"0", "0", "0"}
                    If arrDateTime.Length > 1 Then
                        Dim n As Integer = 0
                        For Each strPart As String In arrDateTime(1).Split(":")
                            arrTime(n) = strPart
                            n += 1
                        Next
                    End If
                    dDate1 = New Date(arrDate(0), arrDate(1), arrDate(2), CInt(arrTime(0)), CInt(arrTime(1)), CInt(arrTime(2)))
                End If
            End Set
        End Property

        ''' <summary>
        ''' Fecha 2
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Date2() As Nullable(Of Date)
            Get
                Return dDate2
            End Get
            Set(ByVal value As Nullable(Of Date))
                dDate2 = value
            End Set
        End Property

        ''' <summary>
        ''' Fecha 2 (cadena para jscript)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property strDate2() As String
            Get
                If dDate2 IsNot Nothing Then
                    Return dDate2.Value.Year.ToString & "-" & dDate2.Value.Month.ToString & "-" & dDate2.Value.Day.ToString & " " & dDate2.Value.Hour.ToString & ":" & dDate2.Value.Minute.ToString & ":" & dDate2.Value.Second.ToString
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" Then
                    Dim arrDateTime() As String = value.Split(" ")
                    Dim arrDate() As String = arrDateTime(0).Split("-")
                    Dim arrTime() As String = {"0", "0", "0"}
                    If arrDateTime.Length > 1 Then
                        Dim n As Integer = 0
                        For Each strPart As String In arrDateTime(1).Split(":")
                            arrTime(n) = strPart
                            n += 1
                        Next
                    End If
                    dDate2 = New Date(arrDate(0), arrDate(1), arrDate(2), CInt(arrTime(0)), CInt(arrTime(1)), CInt(arrTime(2)))
                End If
            End Set
        End Property

        ''' <summary>
        ''' Nombre del campo de la ficha
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property FieldName() As String
            Get
                Return strFieldName
            End Get
            Set(ByVal value As String)
                strFieldName = value
            End Set
        End Property

        ''' <summary>
        ''' Valor del campo de la ficha
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property FieldValue() As String
            Get
                Return strFieldValue
            End Get
            Set(ByVal value As String)
                strFieldValue = value
            End Set
        End Property

        ''' <summary>
        ''' ID de la justificación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDCause() As Nullable(Of Integer)
            Get
                Return intIDCause
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDCause = value
            End Set
        End Property

        ''' <summary>
        ''' ID del centro de coste
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDCenter() As Nullable(Of Integer)
            Get
                Return intIDCenter
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDCenter = value
            End Set
        End Property

        ''' <summary>
        ''' Horas
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Hours() As Nullable(Of Double)
            Get
                Return dblHours
            End Get
            Set(ByVal value As Nullable(Of Double))
                dblHours = value
            End Set
        End Property

        ''' <summary>
        ''' Hours (cadena para jscript)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property strHours() As String
            Get
                If Me.dblHours.HasValue Then
                    Return roConversions.ConvertHoursToTime(Me.dblHours)
                Else
                    Return "00:00"
                End If

            End Get
            Set(ByVal value As String)
                If value <> "" Then
                    Me.dblHours = roConversions.ConvertTimeToHours(value)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Id del horario
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDShift() As Nullable(Of Integer)
            Get
                Return intIDShift
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDShift = value
            End Set
        End Property

        ''' <summary>
        ''' Fecha/Hora de fin del periodo.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ToTime() As Nullable(Of DateTime)
            Get
                Return Me.xToTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xToTime = value
            End Set
        End Property

        ''' <summary>
        ''' Fecha/Hora de inicio del periodo.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property FromTime() As Nullable(Of DateTime)
            Get
                Return Me.xFromTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xFromTime = value
            End Set
        End Property

        ''' <summary>
        ''' Hora de inicio del horario flotante. Sólo se utiliza si el horario (IDShift) es flotante.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property StartShift() As Nullable(Of DateTime)
            Get
                Return Me.xStartShift
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xStartShift = value
            End Set
        End Property

        ''' <summary>
        ''' Fecha/Hora inicio del horario flotante (cadena para jscript)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property strStartShift() As String
            Get
                If Me.xStartShift IsNot Nothing Then
                    Return Me.xStartShift.Value.Year.ToString & "-" & Me.xStartShift.Value.Month.ToString & "-" & Me.xStartShift.Value.Day.ToString & " " & Me.xStartShift.Value.Hour.ToString & ":" & Me.xStartShift.Value.Minute.ToString & ":" & Me.xStartShift.Value.Second.ToString
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" Then
                    Dim arrDateTime() As String = value.Split(" ")
                    Dim arrDate() As String = arrDateTime(0).Split("-")
                    Dim arrTime() As String = {"0", "0", "0"}
                    If arrDateTime.Length > 1 Then
                        Dim n As Integer = 0
                        For Each strPart As String In arrDateTime(1).Split(":")
                            arrTime(n) = strPart
                            n += 1
                        Next
                    End If
                    Me.xStartShift = New Date(arrDate(0), arrDate(1), arrDate(2), CInt(arrTime(0)), CInt(arrTime(1)), CInt(arrTime(2)))
                End If
            End Set
        End Property

        ''' <summary>
        ''' Fecha/Hora inicio periodo (cadena para jscript)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property strFromTime() As String
            Get
                If Me.xFromTime IsNot Nothing Then
                    Return Me.xFromTime.Value.Year.ToString & "-" & Me.xFromTime.Value.Month.ToString & "-" & Me.xFromTime.Value.Day.ToString & " " & Me.xFromTime.Value.Hour.ToString & ":" & Me.xFromTime.Value.Minute.ToString & ":" & Me.xFromTime.Value.Second.ToString
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" Then
                    Dim arrDateTime() As String = value.Split(" ")
                    Dim arrDate() As String = arrDateTime(0).Split("-")
                    Dim arrTime() As String = {"0", "0", "0"}
                    If arrDateTime.Length > 1 Then
                        Dim n As Integer = 0
                        For Each strPart As String In arrDateTime(1).Split(":")
                            arrTime(n) = strPart
                            n += 1
                        Next
                    End If
                    Me.xFromTime = New Date(arrDate(0), arrDate(1), arrDate(2), CInt(arrTime(0)), CInt(arrTime(1)), CInt(arrTime(2)))
                End If
            End Set
        End Property

        ''' <summary>
        ''' Fecha/Hora fin  periodo (cadena para jscript)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property strToTime() As String
            Get
                If Me.xToTime IsNot Nothing Then
                    Return Me.xToTime.Value.Year.ToString & "-" & Me.xToTime.Value.Month.ToString & "-" & Me.xToTime.Value.Day.ToString & " " & Me.xToTime.Value.Hour.ToString & ":" & Me.xToTime.Value.Minute.ToString & ":" & Me.xToTime.Value.Second.ToString
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" Then
                    Dim arrDateTime() As String = value.Split(" ")
                    Dim arrDate() As String = arrDateTime(0).Split("-")
                    Dim arrTime() As String = {"0", "0", "0"}
                    If arrDateTime.Length > 1 Then
                        Dim n As Integer = 0
                        For Each strPart As String In arrDateTime(1).Split(":")
                            arrTime(n) = strPart
                            n += 1
                        Next
                    End If
                    Me.xToTime = New Date(arrDate(0), arrDate(1), arrDate(2), CInt(arrTime(0)), CInt(arrTime(1)), CInt(arrTime(2)))
                End If
            End Set
        End Property

        ''' <summary>
        ''' ID de Empleado ha intercambiar el horario...
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDEmployeeExchange() As Nullable(Of Integer)
            Get
                Return intIDEmployeeExchange
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDEmployeeExchange = value
            End Set
        End Property

        ''' <summary>
        ''' Si la solicitud no ha sido leída por el empleado después de un cambio de estado
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember>
        Public Property NotReaded() As Boolean
            Get
                Return Me.bolNotReaded
            End Get
            Set(ByVal value As Boolean)
                Me.bolNotReaded = value
            End Set
        End Property

        ''' <summary>
        ''' Id de la tarea
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDTask1() As Nullable(Of Integer)
            Get
                Return intIDTask1
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDTask1 = value
            End Set
        End Property

        <DataMember()>
        Public Property IDTask2() As Nullable(Of Integer)
            Get
                Return intIDTask2
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDTask2 = value
            End Set
        End Property

        <DataMember()>
        Public Property Field1() As String
            Get
                Return Me.strField1
            End Get
            Set(ByVal value As String)
                Me.strField1 = value
            End Set
        End Property

        <DataMember()>
        Public Property Field2() As String
            Get
                Return Me.strField2
            End Get
            Set(ByVal value As String)
                Me.strField2 = value
            End Set
        End Property

        <DataMember()>
        Public Property Field3() As String
            Get
                Return Me.strField3
            End Get
            Set(ByVal value As String)
                Me.strField3 = value
            End Set
        End Property

        <DataMember()>
        Public Property Field4() As Double
            Get
                Return Me.dblField4
            End Get
            Set(ByVal value As Double)
                Me.dblField4 = value
            End Set
        End Property

        <DataMember()>
        Public Property Field5() As Double
            Get
                Return Me.dblField5
            End Get
            Set(ByVal value As Double)
                Me.dblField5 = value
            End Set
        End Property

        <DataMember()>
        Public Property Field6() As Double
            Get
                Return Me.dblField6
            End Get
            Set(ByVal value As Double)
                Me.dblField6 = value
            End Set
        End Property

        <DataMember()>
        Public Property CompletedTask() As Nullable(Of Boolean)
            Get
                Return bolCompletedTask
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolCompletedTask = value
            End Set
        End Property

        <DataMember()>
        Public Property AutomaticValidation() As Boolean
            Get
                Return bolAutomaticValidation
            End Get
            Set(ByVal value As Boolean)
                bolAutomaticValidation = value
            End Set
        End Property

        <DataMember>
        Public Property RequestApprovals() As Generic.List(Of roRequestApproval)
            Get
                Return Me.lstRequestApprovals
            End Get
            Set(ByVal value As Generic.List(Of roRequestApproval))
                Me.lstRequestApprovals = value
            End Set
        End Property

        <DataMember>
        Public Property RequestDays() As Generic.List(Of roRequestDay)
            Get
                Return Me.lstRequestDays
            End Get
            Set(ByVal value As Generic.List(Of roRequestDay))
                Me.lstRequestDays = value
            End Set
        End Property

        ''' <summary>
        ''' NextLevelPassports
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property NextLevelPassports() As String
            Get
                Return strNextLevelPassports
            End Get
            Set(ByVal value As String)
                'Me.strNextLevelPassports = value
            End Set
        End Property

        <DataMember()>
        Public Property CRC() As String
            Get
                Return strCRC
            End Get
            Set(ByVal value As String)
                strCRC = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# *,'' As 'NextLevelPassports' FROM Requests WHERE ID = " & Me.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    intIDEmployee = oRow("IDEmployee")
                    iRequestType = oRow("RequestType")
                    dRequestDate = oRow("RequestDate")
                    strComments = Any2String(oRow("Comments"))
                    iStatus = oRow("Status")
                    If Not IsDBNull(oRow("StatusLevel")) Then iStatusLevel = CInt(oRow("StatusLevel"))
                    If Not IsDBNull(oRow("Date1")) Then dDate1 = oRow("Date1")
                    If Not IsDBNull(oRow("Date2")) Then dDate2 = oRow("Date2")
                    strFieldName = Any2String(oRow("FieldName"))
                    strFieldValue = Any2String(oRow("FieldValue"))
                    If Not IsDBNull(oRow("IDCause")) Then intIDCause = roTypes.Any2Integer(oRow("IDCause"))
                    If Not IsDBNull(oRow("IDCenter")) Then intIDCenter = roTypes.Any2Integer(oRow("IDCenter"))
                    If Not IsDBNull(oRow("Hours")) Then dblHours = CDbl(oRow("Hours"))
                    If Not IsDBNull(oRow("IDShift")) Then intIDShift = roTypes.Any2Integer(oRow("IDShift"))
                    If Not IsDBNull(oRow("IDTask1")) Then intIDTask1 = roTypes.Any2Integer(oRow("IDTask1"))
                    If Not IsDBNull(oRow("IDTask2")) Then intIDTask2 = roTypes.Any2Integer(oRow("IDTask2"))
                    If Not IsDBNull(oRow("CompletedTask")) Then bolCompletedTask = roTypes.Any2Boolean(oRow("CompletedTask"))
                    If Not IsDBNull(oRow("Field1")) Then strField1 = roTypes.Any2String(oRow("Field1"))
                    If Not IsDBNull(oRow("Field2")) Then strField2 = roTypes.Any2String(oRow("Field2"))
                    If Not IsDBNull(oRow("Field3")) Then strField3 = roTypes.Any2String(oRow("Field3"))
                    If Not IsDBNull(oRow("Field4")) Then dblField4 = roTypes.Any2Double(oRow("Field4")) Else dblField4 = -1
                    If Not IsDBNull(oRow("Field5")) Then dblField5 = roTypes.Any2Double(oRow("Field5")) Else dblField5 = -1
                    If Not IsDBNull(oRow("Field6")) Then dblField6 = roTypes.Any2Double(oRow("Field6")) Else dblField6 = -1

                    If Not IsDBNull(oRow("NextLevelPassports")) Then strNextLevelPassports = roTypes.Any2String(oRow("NextLevelPassports"))
                    If Not IsDBNull(oRow("StartShift")) Then xStartShift = oRow("StartShift")
                    If Not IsDBNull(oRow("IDEmployeeExchange")) Then intIDEmployeeExchange = roTypes.Any2Integer(oRow("IDEmployeeExchange"))
                    If Not IsDBNull(oRow("FromTime")) Then xFromTime = oRow("FromTime")
                    If Not IsDBNull(oRow("ToTime")) Then xToTime = oRow("ToTime")

                    Me.bolNotReaded = roTypes.Any2Boolean(oRow("NotReaded"))

                    Me.lstRequestApprovals = roRequestApproval.GetRequestApprovals(Me.ID, Me.oState)
                    Me.lstRequestDays = roRequestDay.GetRequestDays(Me.ID, Me.oState)

                    If Not IsDBNull(oRow("AutomaticValidation")) Then bolAutomaticValidation = roTypes.Any2Boolean(oRow("AutomaticValidation"))

                    If Not IsDBNull(oRow("ValidationDate")) Then dValidationDate = oRow("ValidationDate")

                    If Not IsDBNull(oRow("RejectedDate")) Then dRejectedDate = oRow("RejectedDate")

                    Me.CRC = roTypes.Any2String(oRow("CRC"))

                    bolRet = True

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tRequest, "", tbParameters, -1)
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roRequest::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::Load")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene el siguiente ID disponible para dar de alta una nueva solicitud
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextID() As Integer

            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# MAX(ID) FROM Requests"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet + 1

        End Function

        Public Shared Function GetProgrammedAbsencebyIdRequest(ByVal requestId As Integer) As Integer

            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# AbsenceId from ProgrammedAbsences where requestid =" & requestId.ToString
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet

        End Function

        Public Shared Function GetProgrammedOvertimebyIdRequest(ByVal requestId As Integer) As Integer
            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# Id from ProgrammedOvertimes where requestid =" & requestId.ToString
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet
        End Function

        Public Shared Function GetProgrammedCausebyIdRequest(ByVal requestId As Integer) As Integer

            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# AbsenceId from ProgrammedCauses where requestid =" & requestId.ToString
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet

        End Function

        Public Function GenerateRequestDays() As Boolean
            Dim bolRet As Boolean = True

            Try
                Dim strSQL As String = ""

                If Not dDate1.HasValue Or Not dDate2.HasValue Then
                    oState.Result = RequestResultEnum.IncorrectDates
                    bolRet = False
                End If

                If Not intIDShift.HasValue And Me.RequestType = eRequestType.VacationsOrPermissions Then
                    oState.Result = RequestResultEnum.ShiftRequired
                    bolRet = False
                End If

                If bolRet Then
                    If Me.RequestType = eRequestType.VacationsOrPermissions Then
                        ' Generamos los dias de vacaciones necesarios, en funcion del tipo  de horario
                        Dim oShiftState As New Shift.roShiftState
                        roBusinessState.CopyTo(Me.oState, oShiftState)
                        Dim oShift As New Shift.roShift(Me.IDShift, oShiftState)
                        If oShift.AreWorkingDays Then
                            ' Solo dias laborables
                            strSQL = "@SELECT# Date FROM DailySchedule , Shifts WHERE DailySchedule.IDShift1 = Shifts.ID And DailySchedule.IDEmployee =" & Me.IDEmployee & " And DailySchedule.Date >= " & Any2Time(dDate1.Value).SQLSmallDateTime & " AND DailySchedule.Date <=" & Any2Time(dDate2.Value).SQLSmallDateTime
                            strSQL += " AND isnull(DailySchedule.ExpectedWorkingHours, isnull(Shifts.ExpectedWorkingHours, 0)) > 0"
                            Dim tb As DataTable = CreateDataTable(strSQL, )
                            If tb IsNot Nothing And tb.Rows.Count > 0 Then
                                Me.lstRequestDays = New Generic.List(Of roRequestDay)
                                Dim oRequestDay As roRequestDay = Nothing
                                For Each oRow As DataRow In tb.Rows
                                    oRequestDay = New roRequestDay(Me.ID, oRow("Date"), Me.oState)
                                    Me.lstRequestDays.Add(oRequestDay)
                                Next
                            End If
                        Else
                            ' Todos los días del periodo
                            Dim xActualDate As Date = Me.Date1
                            Dim oRequestDay As roRequestDay = Nothing
                            While xActualDate <= Me.Date2
                                oRequestDay = New roRequestDay(Me.ID, xActualDate, Me.oState)
                                Me.lstRequestDays.Add(oRequestDay)
                                xActualDate = xActualDate.AddDays(1)
                            End While
                        End If
                    End If

                    If Me.RequestType = eRequestType.CancelHolidays Then
                        ' Generamos los dias de cancelacion de vacaciones, en funcion de los dias planificados con un horario de vacaciones
                        ' Solo dias laborables
                        strSQL = "@SELECT# Date FROM DailySchedule WHERE DailySchedule.IDEmployee =" & Me.IDEmployee & " And DailySchedule.Date >= " & Any2Time(dDate1.Value).SQLSmallDateTime & " AND DailySchedule.Date <=" & Any2Time(dDate2.Value).SQLSmallDateTime
                        strSQL += " AND isnull(IsHolidays,0) = 1"
                        Dim tb As DataTable = CreateDataTable(strSQL, )
                        If tb IsNot Nothing And tb.Rows.Count > 0 Then
                            Me.lstRequestDays = New Generic.List(Of roRequestDay)
                            Dim oRequestDay As roRequestDay = Nothing
                            For Each oRow As DataRow In tb.Rows
                                oRequestDay = New roRequestDay(Me.ID, oRow("Date"), Me.oState)
                                Me.lstRequestDays.Add(oRequestDay)
                            Next
                        End If
                    End If

                    bolRet = True
                End If
            Catch ex As DbException
                bolRet = False
                oState.UpdateStateInfo(ex, "roRequest::GenerateRequestDays")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::GenerateRequestDays")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function Validate(Optional ByVal bolCheckNames As Boolean = True, Optional ByVal bolIsImporting As Boolean = False, Optional ByVal bApplyValidations As Boolean = False, Optional ByVal bSaveAnswer As Boolean = False, Optional bForceValidation As Boolean = False) As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim strSQL As String

                If dDate1.HasValue AndAlso dDate2.HasValue Then
                    If dDate1.Value > dDate2.Value Then
                        oState.Result = RequestResultEnum.IncorrectDates
                        bolRet = False
                    End If
                End If

                Dim xEmptyDate As New Date(1970, 1, 1)
                If dDate1.HasValue And dDate2.HasValue Then
                    If dDate1.Value = xEmptyDate Or dDate2.Value = xEmptyDate Then
                        oState.Result = RequestResultEnum.IncorrectDates
                        bolRet = False
                    End If
                End If

                ' Validamos que todas las fechas solicitadas esten dentro del contrato del empleado
                If bolRet AndAlso CInt(Me.RequestStatus) < 3 Then
                    Dim oContract As Contract.roContract = Nothing
                    Dim oContractState As New Contract.roContractState
                    roBusinessState.CopyTo(Me.oState, oContractState)

                    If dDate1.HasValue Then
                        oContract = Contract.roContract.GetContractInDate(Me.IDEmployee, dDate1.Value, oContractState, False)
                        If oContract Is Nothing Then bolRet = False
                    End If
                    If bolRet AndAlso dDate2.HasValue Then
                        oContract = Contract.roContract.GetContractInDate(Me.IDEmployee, dDate2.Value, oContractState, False)
                        If oContract Is Nothing Then bolRet = False
                    End If

                    If bolRet AndAlso Me.lstRequestDays IsNot Nothing AndAlso Me.lstRequestDays.Count > 0 Then
                        For Each oRequestDay As roRequestDay In Me.lstRequestDays
                            oContract = Contract.roContract.GetContractInDate(Me.IDEmployee, oRequestDay.RequestDate, oContractState, False)
                            If oContract Is Nothing Then
                                bolRet = False
                                Exit For
                            End If
                        Next
                    End If

                    If Not bolRet Then
                        ' Si es una nueva solicitud y las fechas son posteriores a hoy y posteriores a la fecha de fin del ultimo contrato,dejamos realizarla
                        If Me.ID <= 0 Then
                            ' Obtenemos el ultimo fin de contrato
                            Dim xLastEndDateContract = New DateTime(1990, 1, 1)
                            Dim tbContracts As DataTable = Contract.roContract.GetContractsByIDEmployee(Me.IDEmployee, oContractState)
                            Try
                                If tbContracts IsNot Nothing AndAlso tbContracts.Rows.Count > 0 AndAlso Not tbContracts.Rows(tbContracts.Rows.Count - 1).Item("EndDate") Is Nothing Then
                                    xLastEndDateContract = tbContracts.Rows(tbContracts.Rows.Count - 1).Item("EndDate")
                                End If
                            Catch ex As Exception
                            End Try
                            If dDate1.HasValue AndAlso dDate1.Value > xLastEndDateContract Then
                                bolRet = True
                            ElseIf dDate2.HasValue AndAlso dDate2.Value > xLastEndDateContract Then
                                bolRet = True
                            End If
                        End If
                    End If

                    If Not bolRet Then
                        oState.Result = RequestResultEnum.DateOutOfContract
                    End If
                End If

                If bolRet AndAlso (Me.ID <= 0 OrElse bForceValidation) Then
                    ' Es una solicitud nueva
                    If Not bolIsImporting AndAlso Me.ID <= 0 Then
                        ' Verificamos que no sea una solicitud repetida
                        strSQL = "@SELECT# TOP 1 * FROM Requests WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND RequestType = " & Me.RequestType & " ORDER BY RequestDate DESC"
                        Dim tb As DataTable = CreateDataTable(strSQL, )
                        If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                            If Math.Abs(DateDiff(DateInterval.Second, Me.RequestDate, tb.Rows(0).Item("RequestDate"))) <= 2 Then
                                oState.Result = RequestResultEnum.RequestRepited
                                bolRet = False
                            End If
                        End If
                    End If

                    If bolRet Then
                        Try
                            Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState())
                            If roTypes.Any2String(oAdvParam.Value).ToUpper = "VPA" Then
                                ' En el caso de APV, en el caso de solicitar: Ausencias, Vacaciones/permisos o Intercambio de horarios
                                ' Si el empleado tiene actualmente una prevision de ausencia por dias de:
                                '   - Accidente baja laboral
                                '   - Baja enfermedad comun
                                '   - Riesgo durante el embarazo (LAS JUSTIFICACIONES QUE SE QUIERAN TENER EN CUENTA DEBERAN CONTENER EN LA DESCRIPCION NOTALLOWREQUEST)
                                ' No deje realizar la solicitud
                                Select Case Me.RequestType
                                    Case eRequestType.PlannedAbsences, eRequestType.PlannedCauses, eRequestType.VacationsOrPermissions, eRequestType.ExchangeShiftBetweenEmployees
                                        Dim absState As New Absence.roProgrammedAbsenceState
                                        Dim strWhere As String = ""
                                        Dim queryDate As String = Any2Time(Now.Date).SQLSmallDateTime
                                        strWhere &= " ( (BeginDate >= " & queryDate & " AND BeginDate <= " & queryDate & ")"
                                        strWhere &= " OR "
                                        strWhere &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDate & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & queryDate & ")"
                                        strWhere &= " OR "
                                        strWhere &= " (BeginDate <= " & queryDate & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDate & ") ) "
                                        strWhere &= " AND  IDCause IN(@SELECT# ID FROM CAUSES WHERE description LIKE '%NOTALLOWREQUEST%' )"

                                        Dim tbAbsences As DataTable = Absence.roProgrammedAbsence.GetProgrammedAbsences(Me.IDEmployee, strWhere, absState)
                                        If tbAbsences IsNot Nothing AndAlso tbAbsences.Rows.Count > 0 Then
                                            'oState.Result = RequestResultEnum.AnotherAbsenceExistInDate
                                            oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                            bolRet = False
                                        End If
                                        If bolRet AndAlso Me.RequestType = eRequestType.ExchangeShiftBetweenEmployees Then
                                            If Me.IDEmployeeExchange.HasValue Then
                                                tbAbsences = Absence.roProgrammedAbsence.GetProgrammedAbsences(Me.IDEmployeeExchange, strWhere, absState)
                                                If tbAbsences IsNot Nothing AndAlso tbAbsences.Rows.Count > 0 Then
                                                    oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                                    bolRet = False
                                                End If
                                            End If
                                        End If
                                End Select
                            End If
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "roRequest::Validate::APV Customization: Error validating absence status")
                        End Try
                    End If

                    If bolRet Then
                        Select Case Me.RequestType
                            Case eRequestType.ExchangeShiftBetweenEmployees
                                If Not Me.dDate1.HasValue Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf Not Me.IDShift.HasValue Then
                                    oState.Result = RequestResultEnum.ShiftRequired
                                    bolRet = False
                                ElseIf Not Me.IDEmployeeExchange.HasValue Then
                                    oState.Result = RequestResultEnum.ExchangeEmployeeRequired
                                    bolRet = False
                                ElseIf Me.dDate1.Value <= Now.Date.AddDays(-1) Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                End If

                                ' Verifico compensación, si es necesaria
                                Dim iShiftApplicant As Integer
                                Dim iShiftRequested As Integer
                                Dim iShiftApplicantForCompensation As Integer
                                Dim iShiftRequestedForCompensation As Integer

                                Dim oApplicantShiftData As roCalendarRowShiftData
                                Dim oRequestedShiftData As roCalendarRowShiftData
                                Dim oApplicantCompensationShiftData As roCalendarRowShiftData
                                Dim oRequestedCompensationShiftData As roCalendarRowShiftData

                                Dim intIDPassport As Integer = roConstants.GetSystemUserId()
                                Dim oCalendarState = New roCalendarState(intIDPassport)
                                'Dim oCalendarState = New roCalendarState(1)
                                Dim oCalendarManager As New roCalendarManager(oCalendarState)

                                iShiftApplicant = Me.Field4
                                iShiftRequested = Me.IDShift

                                oApplicantShiftData = oCalendarManager.LoadShiftDayDataByIdShift(iShiftApplicant)
                                oRequestedShiftData = oCalendarManager.LoadShiftDayDataByIdShift(iShiftRequested)

                                ' Valido que el intercambio de horario sigue siendo viable con el empleado seleccionado
                                Dim lrret As New DTOs.GenericList
                                Dim oPassportTicket As New roPassportTicket
                                Dim oEmployeePassport As roPassportTicket = roPassportManager.GetPassportTicket(Me.IDEmployee, LoadType.Employee)
                                Dim oEmpState As New Employee.roEmployeeState(oEmployeePassport.ID)
                                oPassportTicket.IDEmployee = Me.IDEmployee

                                lrret = VTRequests.Requests.roRequest.GetAvailableEmployeesForDateOnEmployeeShiftExchangeEx(oPassportTicket, Me.Date1.Value.ToString("dd/MM/yyyy"), iShiftRequested, Nothing, iShiftApplicant, oEmpState, Me.ID, Me.IDEmployeeExchange)

                                ' Miro si el empleado destino sigue siendo válido
                                If lrret.SelectFields Is Nothing OrElse (lrret.SelectFields.Any() AndAlso lrret.SelectFields.ToList.Find(Function(x) x.FieldValue = Me.IDEmployeeExchange AndAlso x.RelatedInfo = "") Is Nothing) Then
                                    ' Intentamos concretar el motivo del error
                                    If lrret.SelectFields IsNot Nothing AndAlso lrret.SelectFields.ToList.Find(Function(x) x.FieldValue = Me.IDEmployeeExchange) IsNot Nothing Then
                                        Select Case lrret.SelectFields.ToList.Find(Function(x) x.FieldValue = Me.IDEmployeeExchange).RelatedInfo
                                            Case "BlockedDay"
                                                oState.Result = RequestResultEnum.BlockedDayOnShiftExchange
                                            Case "RequestPending"
                                                oState.Result = RequestResultEnum.RequestPendingOnShiftExchange
                                            Case "NoPermission"
                                                oState.Result = RequestResultEnum.NoPermissionOnShiftExchange
                                            Case "OnAbsence"
                                                oState.Result = RequestResultEnum.OnAbsenceOnShiftExchange
                                            Case "OnHolidays"
                                                oState.Result = RequestResultEnum.OnHolidaysOnShiftExchange
                                            Case "ApplicantCantCoverEmployee"
                                                oState.Result = RequestResultEnum.ApplicantCantCoverEmployeeOnShiftExchange
                                            Case "EmployeeCantCoverApplicant"
                                                oState.Result = RequestResultEnum.EmployeeCantCoverApplicantOnShiftExchange
                                            Case "NoAssignment"
                                                oState.Result = RequestResultEnum.NoAssignmentOnShiftExchange
                                            Case "WrongAssignment"
                                                oState.Result = RequestResultEnum.WrongAssignmentOnShiftExchange
                                            Case "Indictment"
                                                oState.Result = RequestResultEnum.IndictmentOnShiftExchange
                                            Case Else
                                                oState.Result = RequestResultEnum.EmployeeNotSuitableForShiftExchange
                                        End Select
                                    Else
                                        oState.Result = RequestResultEnum.EmployeeNotSuitableForShiftExchange
                                    End If
                                    bolRet = False
                                End If

                                If bolRet Then
                                    Dim oParam As New AdvancedParameter.roAdvancedParameter("VTLive.ExchangeShiftBetweenEmployees.CompensationRequiered", New AdvancedParameter.roAdvancedParameterState(Me.oState.IDPassport))
                                    If roTypes.Any2Boolean(oParam.Value) AndAlso ((oApplicantShiftData.PlannedHours = 0 AndAlso oRequestedShiftData.PlannedHours > 0) OrElse (oApplicantShiftData.PlannedHours > 0 AndAlso oRequestedShiftData.PlannedHours = 0)) Then
                                        ' Requiere compensación
                                        Dim dailySchedule = New VTBusiness.Shift.roDailySchedule(Me.IDEmployee, Me.Date2, New Shift.roShiftState())
                                        iShiftApplicantForCompensation = IIf(dailySchedule.IDShiftUsed Is Nothing, dailySchedule.IDShift1, dailySchedule.IDShiftUsed)
                                        dailySchedule = New VTBusiness.Shift.roDailySchedule(Me.IDEmployeeExchange, Me.Date2, New Shift.roShiftState())
                                        iShiftRequestedForCompensation = IIf(dailySchedule.IDShiftUsed Is Nothing, dailySchedule.IDShift1, dailySchedule.IDShiftUsed)

                                        ' El intercambio en el día de compensación sigue siendo válido entre esos dos empleados
                                        lrret = New DTOs.GenericList
                                        lrret = VTRequests.Requests.roRequest.GetAvailableEmployeesForDateOnEmployeeShiftExchangeEx(oPassportTicket, Me.Date2, iShiftRequestedForCompensation, Nothing, iShiftApplicantForCompensation, oEmpState, Me.ID, Me.IDEmployeeExchange)

                                        ' Miro si el empleado destino sigue siendo válido
                                        If lrret.SelectFields Is Nothing OrElse (lrret.SelectFields.Count > 0 AndAlso lrret.SelectFields.ToList.Find(Function(x) x.FieldValue = Me.IDEmployeeExchange AndAlso x.RelatedInfo = "") Is Nothing) Then
                                            ' Intentamos concretar el motivo del error
                                            If lrret.SelectFields IsNot Nothing AndAlso lrret.SelectFields.ToList.Find(Function(x) x.FieldValue = Me.IDEmployeeExchange) IsNot Nothing Then
                                                Select Case lrret.SelectFields.ToList.Find(Function(x) x.FieldValue = Me.IDEmployeeExchange).RelatedInfo
                                                    Case "BlockedDay"
                                                        oState.Result = RequestResultEnum.BlockedDayOnShiftExchange
                                                    Case "RequestPending"
                                                        oState.Result = RequestResultEnum.RequestPendingOnShiftExchange
                                                    Case "NoPermission"
                                                        oState.Result = RequestResultEnum.NoPermissionOnShiftExchange
                                                    Case "OnAbsence"
                                                        oState.Result = RequestResultEnum.OnAbsenceOnShiftExchange
                                                    Case "OnHolidays"
                                                        oState.Result = RequestResultEnum.OnHolidaysOnShiftExchange
                                                    Case "ApplicantCantCoverEmployee"
                                                        oState.Result = RequestResultEnum.ApplicantCantCoverEmployeeOnShiftExchange
                                                    Case "EmployeeCantCoverApplicant"
                                                        oState.Result = RequestResultEnum.EmployeeCantCoverApplicantOnShiftExchange
                                                    Case "NoAssignment"
                                                        oState.Result = RequestResultEnum.NoAssignmentOnShiftExchange
                                                    Case "WrongAssignment"
                                                        oState.Result = RequestResultEnum.WrongAssignmentOnShiftExchange
                                                    Case "Indictment"
                                                        oState.Result = RequestResultEnum.IndictmentOnShiftExchange
                                                    Case Else
                                                        oState.Result = RequestResultEnum.EmployeeNotSuitableForShiftExchange
                                                End Select
                                            Else
                                                oState.Result = RequestResultEnum.EmployeeNotSuitableForShiftExchange
                                            End If
                                            bolRet = False
                                        End If

                                        oApplicantCompensationShiftData = oCalendarManager.LoadShiftDayDataByIdShift(iShiftApplicantForCompensation)
                                        oRequestedCompensationShiftData = oCalendarManager.LoadShiftDayDataByIdShift(iShiftRequestedForCompensation)

                                        If (oApplicantShiftData.PlannedHours = 0 AndAlso oApplicantCompensationShiftData.PlannedHours = 0) OrElse
                                       (oApplicantShiftData.PlannedHours > 0 AndAlso oApplicantCompensationShiftData.PlannedHours > 0) OrElse
                                       (oRequestedShiftData.PlannedHours = 0 AndAlso oRequestedCompensationShiftData.PlannedHours = 0) OrElse
                                       (oRequestedShiftData.PlannedHours > 0 AndAlso oRequestedCompensationShiftData.PlannedHours > 0) Then
                                            oState.Result = RequestResultEnum.CompensationRequired
                                            bolRet = False
                                        End If
                                    End If
                                End If
                            Case eRequestType.UserFieldsChange
                                If Me.FieldName = "" Then
                                    oState.Result = RequestResultEnum.UserFieldRequired
                                    bolRet = False
                                End If

                            Case eRequestType.ForbiddenPunch
                                If Not Me.dDate1.HasValue Then
                                    oState.Result = RequestResultEnum.PunchDateTimeRequired
                                    bolRet = False
                                End If
                                If bolRet Then
                                    ' Verificamos que no exista un fichaje en la misma fecha y hora
                                    Dim oPunchState As New Punch.roPunchState
                                    roBusinessState.CopyTo(Me.oState, oPunchState)
                                    Dim tbPunches As DataTable = Punch.roPunch.GetPunches("IDEmployee = " & Me.IDEmployee.ToString & " AND " &
                                                                                     " DateTime = " & Any2Time(Me.Date1).SQLDateTime & " AND ActualType IN(" & PunchTypeEnum._IN & "," & PunchTypeEnum._OUT & ")",
                                                                                     oPunchState)
                                    If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then
                                        oState.Result = RequestResultEnum.PunchExist
                                        bolRet = False
                                    End If
                                End If
                                If bolRet Then
                                    ' Verificamos que el fichaje no sea anterior a la fecha de cierre
                                    Dim xFreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee, False, oState)
                                    If Any2Time(Me.Date1).DateOnly <= xFreezeDate Then
                                        oState.Result = RequestResultEnum.FreezeDateException
                                        bolRet = False
                                    End If

                                End If

                            Case eRequestType.JustifyPunch
                                If Not Me.dDate1.HasValue Then
                                    oState.Result = RequestResultEnum.PunchDateTimeRequired
                                    bolRet = False
                                ElseIf Not Me.IDCause.HasValue OrElse Me.IDCause <= 0 Then
                                    oState.Result = RequestResultEnum.CauseRequired
                                    bolRet = False
                                End If

                            Case eRequestType.ExternalWorkResumePart
                                If Not Me.dDate1.HasValue Then
                                    oState.Result = RequestResultEnum.DateRequired
                                    bolRet = False
                                ElseIf Not Me.Hours.HasValue OrElse Me.Hours.Value = 0 Then
                                    oState.Result = RequestResultEnum.HoursRequired
                                    bolRet = False
                                End If

                            Case eRequestType.ChangeShift, eRequestType.VacationsOrPermissions
                                If Not Me.dDate1.HasValue Or Not Me.dDate2.HasValue Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf dDate1.Value > dDate2.Value Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf Not Me.IDShift.HasValue Then
                                    oState.Result = RequestResultEnum.ShiftRequired
                                    bolRet = False
                                Else
                                    Dim oShiftState As New Shift.roShiftState()
                                    roBusinessState.CopyTo(Me.oState, oShiftState)
                                    Dim oShift As New Shift.roShift(Me.IDShift, oShiftState, False)
                                    If oShiftState.Result = ShiftResultEnum.NoError Then
                                        If oShift.ShiftType = ShiftType.NormalFloating AndAlso Not Me.StartShift.HasValue Then
                                            oState.Result = RequestResultEnum.StartShiftRequired
                                            bolRet = False
                                        End If
                                    End If
                                End If

                                If Me.RequestType = eRequestType.ChangeShift Then
                                    ' Si es una petición de cambio de horario de uno en concreto a el mismo, damos un error de horario incorrecto
                                    If Me.Field4 > 0 AndAlso Me.Field4 = Me.IDShift Then
                                        oState.Result = RequestResultEnum.ShiftRequired
                                        bolRet = False
                                    End If
                                End If

                                If Me.RequestType = eRequestType.VacationsOrPermissions Then

                                    If Me.lstRequestDays Is Nothing OrElse Me.lstRequestDays.Count = 0 Then
                                        oState.Result = RequestResultEnum.IncorrectDates
                                        bolRet = False
                                    Else
                                        ' En el caso de solicitudes de vacaciones/permisos
                                        Dim xBeginPeriodActualDay As DateTime = Nothing
                                        Dim xBeginPeriodRequest As DateTime = Nothing
                                        Dim xEndPeriodRequest As DateTime = Nothing

                                        Try
                                            If Me.IDShift.HasValue Then
                                                ' Verificamos si el saldo actual del horario, se utiliza en algun otro horario como saldo en solicitudes del año o periodo laboral siguiente
                                                ' En caso afirmativo, hay que verificar que ninguna fecha solicitada sea del año o periodo laboral siguiente
                                                Dim strAllow As String = "@SELECT# TOP 1 isnull(ID, 0) FROM SHIFTS WHERE ISNULL(IDConceptRequestNextYear,0) > 0 and ISNULL(IDConceptRequestNextYear,0) in ( @SELECT# isnull(IDConceptBalance,0) FROM SHIFTS where ID=" & Me.IDShift.ToString & " AND isnull(IDConceptBalance,0) > 0 )"
                                                If Any2Integer(ExecuteScalar(strAllow)) > 0 Then
                                                    Dim strDefaultQuery As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from Shifts where id =" & IDShift.ToString & ")"))
                                                    If strDefaultQuery.Length = 0 Then strDefaultQuery = "Y"

                                                    xBeginPeriodActualDay = roParameters.BeginYearPeriod(Now.Date)
                                                    xBeginPeriodRequest = roParameters.BeginYearPeriod(Me.Date1)
                                                    xEndPeriodRequest = roParameters.BeginYearPeriod(Me.Date2)

                                                    ' Si el inicio o el final del periodo anual actual de la fecha solicitada es posterior al inicio del periodo actual
                                                    ' alguna fecha es del año siguiente
                                                    If xBeginPeriodRequest > xBeginPeriodActualDay OrElse xEndPeriodRequest > xBeginPeriodActualDay Then
                                                        oState.Result = RequestResultEnum.IncorrectDates
                                                        bolRet = False
                                                    End If
                                                End If

                                                ' Se valida si el horario tiene marcada la opcion de utilizar otro saldo para fechas del año siguiente
                                                ' En ese caso en la misma solicitud no se pueden realizar peticiones del año actual y del año siguiente a la vez
                                                strAllow = "@SELECT# isnull(ID, 0) FROM SHIFTS WHERE ISNULL(IDConceptRequestNextYear,0) > 0 and ID=" & Me.IDShift.ToString
                                                If Any2Integer(ExecuteScalar(strAllow)) > 0 Then
                                                    Dim strDefaultQuery As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptRequestNextYear,0) from Shifts where id =" & IDShift.ToString & ")"))
                                                    If strDefaultQuery.Length = 0 Then strDefaultQuery = "Y"

                                                    If xBeginPeriodActualDay = Nothing Then xBeginPeriodActualDay = roParameters.BeginYearPeriod(Now.Date)
                                                    If xBeginPeriodRequest = Nothing Then xBeginPeriodRequest = roParameters.BeginYearPeriod(Me.Date1)
                                                    If xEndPeriodRequest = Nothing Then xEndPeriodRequest = roParameters.BeginYearPeriod(Me.Date2)

                                                    ' Todas las fechas solcitadas deben ser o del periodo actual, o del siguiente, pero no de ambos
                                                    If xBeginPeriodRequest = xBeginPeriodActualDay AndAlso xEndPeriodRequest > xBeginPeriodActualDay Then
                                                        oState.Result = RequestResultEnum.IncorrectDates
                                                        bolRet = False
                                                    End If
                                                End If
                                            End If
                                        Catch ex As Exception
                                        End Try

                                        For Each oRequestDay As roRequestDay In Me.lstRequestDays
                                            Dim queryDate As String = roTypes.Any2Time(oRequestDay.RequestDate).SQLSmallDateTime()
                                            ' Para cada dia de la solicitud

                                            ' No se debe solapar con ninguna solicitud de vacaciones por dias existente
                                            If bolRet Then
                                                Dim strWhere As String = "(RequestType = " & eRequestType.VacationsOrPermissions & ") AND"

                                                strWhere &= " EXISTS ( @SELECT# 1 as ExistRec  from  sysroRequestDays WHERE Requests.ID = sysroRequestDays.IDRequest "
                                                strWhere &= " AND "
                                                strWhere &= " sysroRequestDays.Date=" & queryDate & ") "
                                                strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled
                                                strWhere &= " And ID <> " & Me.ID.ToString

                                                Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                                If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                                    If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.VacationsOrPermissions Then 'vacaciones por dias
                                                        oState.Result = RequestResultEnum.VacationsOrPermissionsOverlapped
                                                        bolRet = False
                                                    End If
                                                End If
                                            End If

                                            ' No se debe solapar con ninguna solicitud de prevision de vacaciones por horas
                                            If bolRet Then
                                                Dim strWhere As String = "(RequestType = " & eRequestType.PlannedHolidays & ") AND"

                                                strWhere &= " EXISTS ( @SELECT# 1 as ExistRec  from  sysroRequestDays WHERE Requests.ID = sysroRequestDays.IDRequest "
                                                strWhere &= " AND "
                                                strWhere &= " sysroRequestDays.Date=" & queryDate & ") "
                                                strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                                Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                                If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                                    If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedHolidays Then 'vacaciones/permisos por horas
                                                        oState.Result = RequestResultEnum.PlannedHolidaysOverlapped
                                                        bolRet = False
                                                    End If
                                                End If
                                            End If

                                            If bolRet Then
                                                'Validacion de previsiones de vacaciones por horas
                                                strSQL = "@SELECT# * from ProgrammedHolidays " &
                                                                "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                                strSQL &= " Date = " & queryDate
                                                Dim tb As DataTable = CreateDataTable(strSQL, )
                                                If tb IsNot Nothing Then
                                                    If tb.Rows.Count > 0 Then
                                                        oState.Result = RequestResultEnum.AnotherHolidayExistInDate
                                                        bolRet = False
                                                    Else
                                                        bolRet = True
                                                    End If
                                                End If
                                            End If

                                            If bolRet Then
                                                ' Verificamos que la solicitud no esté asignada a un dia con un horario de vacaciones planificado
                                                strSQL = "@SELECT# isnull(IsHolidays,0)  as Holidays from DailySchedule, Shifts " &
                                                    "WHERE Shifts.ID = DailySchedule.IDShift1  AND IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                                strSQL &= " Date = " & queryDate & " AND ( isnull(IsHolidays,0) = 1 ) "
                                                Dim tb As DataTable = CreateDataTable(strSQL, )
                                                If tb IsNot Nothing Then
                                                    If tb.Rows.Count > 0 Then
                                                        oState.Result = RequestResultEnum.InHolidayPlanification
                                                        bolRet = False
                                                    Else
                                                        bolRet = True
                                                    End If
                                                End If
                                            End If

                                            'Validacion respecto a solicitudes de prevision de ausencia dia/horas
                                            If bolRet Then
                                                Dim strWhere As String = "(RequestType IN(" & eRequestType.PlannedCauses & "," & eRequestType.PlannedAbsences & ")) AND"

                                                strWhere &= " ( (Date1 = " & queryDate & ")"
                                                strWhere &= " OR "
                                                strWhere &= " (IsNULL(Date2,Date1) >= " & queryDate & " AND IsNULL(Date2,Date1) <= " & queryDate & ")"
                                                strWhere &= " OR "
                                                strWhere &= " (Date1 <= " & queryDate & " AND IsNULL(Date2,Date1) >= " & queryDate & ") ) "
                                                strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                                Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                                If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                                    If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedCauses Then 'Ausencia por horas
                                                        oState.Result = RequestResultEnum.PlannedCausesOverlapped
                                                        bolRet = False
                                                    ElseIf roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedAbsences Then 'Ausencia por días
                                                        oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                                        bolRet = False
                                                    End If
                                                End If
                                            End If

                                            'Validacion que no se solape con una ProgrammedAbsences
                                            If bolRet Then
                                                Dim absState As New Absence.roProgrammedAbsenceState
                                                Dim strWhere As String = ""

                                                strWhere &= " ( (BeginDate >= " & queryDate & " AND BeginDate <= " & queryDate & ")"
                                                strWhere &= " OR "
                                                strWhere &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDate & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & queryDate & ")"
                                                strWhere &= " OR "
                                                strWhere &= " (BeginDate <= " & queryDate & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDate & ") ) "

                                                Dim tbAbsences As DataTable = Absence.roProgrammedAbsence.GetProgrammedAbsences(Me.IDEmployee, strWhere, absState)
                                                If tbAbsences IsNot Nothing AndAlso tbAbsences.Rows.Count > 0 Then
                                                    oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                                    bolRet = False
                                                End If
                                            End If

                                            'Validacion que no se solape con una ProgrammedCauses
                                            If bolRet Then
                                                Dim cauState As New Incidence.roProgrammedCauseState
                                                Dim strWhere = ""

                                                strWhere &= " ( (Date >= " & queryDate & " AND Date <= " & queryDate & ")"
                                                strWhere &= " OR "
                                                strWhere &= " (IsNULL(FinishDate,Date) >= " & queryDate & " AND IsNULL(FinishDate,Date) <= " & queryDate & ")"
                                                strWhere &= " OR "
                                                strWhere &= " (Date <= " & queryDate & " AND IsNULL(FinishDate,Date) >= " & queryDate & ") ) "

                                                Dim tbCauses As DataTable = Incidence.roProgrammedCause.GetProgrammedCauses(Me.IDEmployee, strWhere, cauState)
                                                If tbCauses IsNot Nothing AndAlso tbCauses.Rows.Count > 0 Then
                                                    oState.Result = RequestResultEnum.PlannedCausesOverlapped
                                                    bolRet = False
                                                End If
                                            End If
                                        Next
                                    End If
                                End If

                            Case eRequestType.CancelHolidays

                                If Not Me.dDate1.HasValue Or Not Me.dDate2.HasValue Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf dDate1.Value > dDate2.Value Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                End If

                            Case eRequestType.PlannedAbsences

                                If Not Me.dDate1.HasValue Or Not Me.dDate2.HasValue Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf dDate1.Value > dDate2.Value Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf Not (Me.IDCause.HasValue AndAlso Me.IDCause.Value > 0) Then
                                    oState.Result = RequestResultEnum.CauseRequired
                                    bolRet = False
                                End If

                                Dim queryDateStart As String = roTypes.Any2Time(Me.Date1).SQLSmallDateTime()
                                Dim queryDateEnd As String = IIf(Me.Date2.HasValue, roTypes.Any2Time(Me.Date2).SQLSmallDateTime(), roTypes.Any2Time(Me.Date1).SQLSmallDateTime())

                                'Validacion de la PlannedCauses con respecto a otras
                                If bolRet Then
                                    Dim strWhere As String = "(RequestType IN(" & eRequestType.PlannedCauses & "," & eRequestType.PlannedAbsences & ")) AND"

                                    strWhere &= " ( (Date1 >= " & queryDateStart & " AND Date1 <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (IsNULL(Date2,Date1) >= " & queryDateStart & " AND IsNULL(Date2,Date1) <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (Date1 <= " & queryDateStart & " AND IsNULL(Date2,Date1) >= " & queryDateEnd & ") ) "
                                    strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled
                                    strWhere &= " And ID <> " & Me.ID.ToString

                                    Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                    If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                        If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedCauses Then 'Ausencia por horas
                                            oState.Result = RequestResultEnum.PlannedCausesOverlapped
                                            bolRet = False
                                        ElseIf roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedAbsences Then 'Ausencia por días
                                            oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                            bolRet = False
                                        End If
                                    End If
                                End If

                                If bolRet Then
                                    'Validacion de solicitudes de previsiones de vacaciones por horas
                                    Dim strWhere As String = "(RequestType = " & eRequestType.PlannedHolidays & ") AND"

                                    strWhere &= " EXISTS ( @SELECT# 1 as ExistRec  from  sysroRequestDays WHERE Requests.ID = sysroRequestDays.IDRequest "
                                    strWhere &= " AND "
                                    strWhere &= " sysroRequestDays.Date>=" & queryDateStart & " AND  sysroRequestDays.Date<=" & queryDateEnd & ") "
                                    strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                    Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                    If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                        If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedHolidays Then 'vacaciones/permisos por horas
                                            oState.Result = RequestResultEnum.PlannedHolidaysOverlapped
                                            bolRet = False
                                        End If
                                    End If
                                End If

                                If bolRet Then
                                    'Validacion de solicitudes de previsiones de exceso
                                    Dim strWhere As String = "(RequestType = " & eRequestType.PlannedOvertimes & ") AND"

                                    strWhere &= " ( (Date1 >= " & queryDateStart & " AND Date1 <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (IsNULL(Date2,Date1) >= " & queryDateStart & " AND IsNULL(Date2,Date1) <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (Date1 <= " & queryDateStart & " AND IsNULL(Date2,Date1) >= " & queryDateEnd & ") ) "
                                    strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled
                                    Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                    If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                        If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedOvertimes Then 'excesos
                                            oState.Result = RequestResultEnum.PlannedOvertimesOverlapped
                                            bolRet = False
                                        End If
                                    End If
                                End If

                                If bolRet Then
                                    'Validacion de previsiones de vacaciones por horas
                                    strSQL = "@SELECT# * from ProgrammedHolidays " &
                                                    "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                    strSQL &= " Date> = " & queryDateStart & " AND  Date<=" & queryDateEnd
                                    Dim tb As DataTable = CreateDataTable(strSQL, )
                                    If tb IsNot Nothing Then
                                        If tb.Rows.Count > 0 Then
                                            oState.Result = RequestResultEnum.AnotherHolidayExistInDate
                                            bolRet = False
                                        Else
                                            bolRet = True
                                        End If
                                    End If
                                End If

                                'Validacion de la PlannedCauses con ProgrammedAbsences
                                ' 1er or --> La fecha inicial cae dentro el periodo de las ausencias ya existentes
                                ' 2on or --> La fecha final cae dentro el periodo de las ausencias ya existentes
                                ' 3er or --> La nueva ausencia engloba la ausencia ya existente
                                If bolRet Then
                                    Dim absState As New Absence.roProgrammedAbsenceState
                                    Dim strWhere As String = ""

                                    strWhere &= " ( (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (BeginDate <= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateEnd & ") ) "

                                    Dim tbAbsences As DataTable = Absence.roProgrammedAbsence.GetProgrammedAbsences(Me.IDEmployee, strWhere, absState)
                                    If tbAbsences IsNot Nothing AndAlso tbAbsences.Rows.Count > 0 Then
                                        oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                        bolRet = False
                                    End If
                                End If

                                'Validacion que no se solape con una ProgrammedCauses
                                If bolRet Then
                                    Dim cauState As New Incidence.roProgrammedCauseState
                                    Dim strWhere = ""

                                    strWhere &= " ( (Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (IsNULL(FinishDate,Date) >= " & queryDateStart & " AND IsNULL(FinishDate,Date) <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (Date <= " & queryDateStart & " AND IsNULL(FinishDate,Date) >= " & queryDateEnd & ") ) "

                                    Dim tbCauses As DataTable = Incidence.roProgrammedCause.GetProgrammedCauses(Me.IDEmployee, strWhere, cauState)
                                    If tbCauses IsNot Nothing AndAlso tbCauses.Rows.Count > 0 Then
                                        oState.Result = RequestResultEnum.PlannedCausesOverlapped
                                        bolRet = False
                                    End If
                                End If

                                If bolRet Then
                                    'Validacion de previsiones de exceso
                                    strSQL = "@SELECT# * from ProgrammedOvertimes " &
                                                    "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                    strSQL &= "  (  (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
                                    strSQL &= " OR "
                                    strSQL &= " (EndDate >= " & queryDateStart & " AND EndDate <= " & queryDateEnd & ")"
                                    strSQL &= " OR "
                                    strSQL &= " (BeginDate <= " & queryDateStart & " AND EndDate >= " & queryDateEnd & ") ) "
                                    Dim tb As DataTable = CreateDataTable(strSQL, )
                                    If tb IsNot Nothing Then
                                        If tb.Rows.Count > 0 Then
                                            oState.Result = RequestResultEnum.AnotherOvertimeExistInDate
                                            bolRet = False
                                        Else
                                            bolRet = True
                                        End If
                                    End If
                                End If

                                ' No se debe solapar con ninguna solicitud de vacaciones por dias existente
                                If bolRet Then
                                    Dim strWhere As String = "(RequestType = " & eRequestType.VacationsOrPermissions & ") AND"

                                    strWhere &= " EXISTS ( @SELECT# 1 as ExistRec  from  sysroRequestDays WHERE Requests.ID = sysroRequestDays.IDRequest "
                                    strWhere &= " AND "
                                    strWhere &= " sysroRequestDays.Date>=" & queryDateStart & " AND sysroRequestDays.Date<=" & queryDateEnd & ") "
                                    strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled
                                    strWhere &= " And ID <> " & Me.ID.ToString

                                    Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                    If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                        If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.VacationsOrPermissions Then 'vacaciones por dias
                                            oState.Result = RequestResultEnum.VacationsOrPermissionsOverlapped
                                            bolRet = False
                                        End If
                                    End If
                                End If

                                If bolRet Then
                                    Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState())
                                    If roTypes.Any2String(oAdvParam.Value).ToUpper <> "VPA" Then
                                        ' Verificamos que la solicitud no esté asignada a un dia con un horario de vacaciones planificado
                                        strSQL = "@SELECT# isnull(IsHolidays,0)  as Holidays from DailySchedule, Shifts " &
                                                    "WHERE Shifts.ID = DailySchedule.IDShift1  AND IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                        strSQL &= " Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & " AND ( isnull(IsHolidays,0) = 1 ) "
                                        Dim tb As DataTable = CreateDataTable(strSQL, )
                                        If tb IsNot Nothing Then
                                            If tb.Rows.Count > 0 Then
                                                oState.Result = RequestResultEnum.InHolidayPlanification
                                                bolRet = False
                                            Else
                                                bolRet = True
                                            End If
                                        End If
                                    End If
                                End If



                            Case eRequestType.PlannedCauses

                                'Validacion de la propia PlannedCauses
                                If Not Me.dDate1.HasValue Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf Not (Me.IDCause.HasValue AndAlso Me.IDCause.Value > 0) Then
                                    oState.Result = RequestResultEnum.CauseRequired
                                    bolRet = False
                                ElseIf Not Me.FromTime.HasValue Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf Not Me.ToTime.HasValue Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf Me.dblHours <= 0 Then
                                    oState.Result = RequestResultEnum.HoursRequired
                                    bolRet = False
                                ElseIf Me.dblHours = 0 Or roTypes.Any2Time(Me.dblHours).NumericValue > roTypes.Any2Time("23:59").NumericValue Then
                                    oState.Result = RequestResultEnum.HoursRequired
                                    bolRet = False
                                ElseIf roTypes.Any2Time(Me.dblHours).NumericValue > roTypes.Any2Time(roTypes.Any2Time(Me.ToTime.Value).NumericValue - roTypes.Any2Time(Me.FromTime.Value).NumericValue).NumericValue Then
                                    oState.Result = RequestResultEnum.HoursRequired
                                    bolRet = False
                                End If

                                Dim queryDateStart As String = roTypes.Any2Time(Me.Date1).SQLSmallDateTime()
                                Dim queryDateEnd As String = IIf(Me.Date2.HasValue, roTypes.Any2Time(Me.Date2).SQLSmallDateTime(), roTypes.Any2Time(Me.Date1).SQLSmallDateTime())
                                Dim queryStartHour As String = roTypes.Any2Time(Me.FromTime).SQLDateTime()
                                Dim queryEndHour As String = roTypes.Any2Time(Me.ToTime).SQLDateTime()

                                'Validacion de la PlannedCauses con respecto a otras
                                'Si es una ausencia por horas buscaremos si alguna que cumpla nuestro criterio
                                'Que este en un periodo en el que nosotros también estemos

                                If bolRet Then
                                    Dim strWhere As String = "(RequestType = " & eRequestType.PlannedCauses & ") AND"

                                    strWhere &= " ( ( (Date1 >= " & queryDateStart & " AND Date1 <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (IsNULL(Date2,Date1) >= " & queryDateStart & " AND IsNULL(Date2,Date1) <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (Date1 <= " & queryDateStart & " AND IsNULL(Date2,Date1) >= " & queryDateEnd & ") ) AND  ("

                                    strWhere &= " (ToTime > " & queryStartHour & " AND " & queryStartHour & " BETWEEN FromTime AND ToTime)"
                                    strWhere &= " OR "
                                    strWhere &= " (" & queryEndHour & " > FromTime AND " & queryStartHour & " < FromTime) ) )"

                                    strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                    Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                    If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                        If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedCauses Then 'Ausencia por horas
                                            oState.Result = RequestResultEnum.PlannedCausesOverlapped
                                            bolRet = False
                                        End If
                                    End If

                                    If bolRet Then
                                        strWhere = "(RequestType = " & eRequestType.PlannedAbsences & ") AND"

                                        strWhere &= " ( ( (Date1 >= " & queryDateStart & " AND Date1 <= " & queryDateEnd & ")"
                                        strWhere &= " OR "
                                        strWhere &= " (IsNULL(Date2,Date1) >= " & queryDateStart & " AND IsNULL(Date2,Date1) <= " & queryDateEnd & ")"
                                        strWhere &= " OR "
                                        strWhere &= " (Date1 <= " & queryDateStart & " AND IsNULL(Date2,Date1) >= " & queryDateEnd & ") ) )"

                                        strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                        tbRequest = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                        If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                            If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedAbsences Then 'Ausencia por días
                                                oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                                bolRet = False
                                            End If
                                        End If
                                    End If

                                    If bolRet Then
                                        strWhere = "(RequestType = " & eRequestType.PlannedHolidays & ") AND"

                                        strWhere &= " EXISTS ( @SELECT# 1 as ExistRec from  sysroRequestDays WHERE  Requests.ID = sysroRequestDays.IDRequest AND IDRequest <>   " & Me.ID
                                        strWhere &= " AND "
                                        strWhere &= " sysroRequestDays.Date>=" & queryDateStart & " AND sysroRequestDays.Date<=" & queryDateEnd & " AND "

                                        strWhere &= " ((" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime AND AllDay = 0)"
                                        strWhere &= " OR "
                                        strWhere &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime  AND AllDay = 0)"
                                        strWhere &= " OR "
                                        strWhere &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime AND AllDay = 0)"
                                        strWhere &= " OR "
                                        strWhere &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND AllDay = 0)  "
                                        ' o una prevision de vacaciones de todo el dia para la misma fecha
                                        strWhere &= " Or (sysroRequestDays.AllDay = 1))  )"
                                        strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                        tbRequest = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                        If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                            If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedHolidays Then 'vacaciones/permisos por horas
                                                oState.Result = RequestResultEnum.PlannedHolidaysOverlapped
                                                bolRet = False
                                            End If
                                        End If
                                    End If

                                    If bolRet Then
                                        strWhere = "(RequestType = " & eRequestType.PlannedOvertimes & ") AND"

                                        strWhere &= " ( ( (Date1 >= " & queryDateStart & " AND Date1 <= " & queryDateEnd & ")"
                                        strWhere &= " OR "
                                        strWhere &= " (IsNULL(Date2,Date1) >= " & queryDateStart & " AND IsNULL(Date2,Date1) <= " & queryDateEnd & ")"
                                        strWhere &= " OR "
                                        strWhere &= " (Date1 <= " & queryDateStart & " AND IsNULL(Date2,Date1) >= " & queryDateEnd & ") ) AND  ("

                                        strWhere &= " (ToTime > " & queryStartHour & " AND " & queryStartHour & " BETWEEN FromTime AND ToTime)"
                                        strWhere &= " OR "
                                        strWhere &= " (" & queryEndHour & " > FromTime AND " & queryStartHour & " < FromTime) ) )"

                                        strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                        tbRequest = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                        If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                            If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedOvertimes Then 'Ausencia por horas
                                                oState.Result = RequestResultEnum.PlannedOvertimesOverlapped
                                                bolRet = False
                                            End If
                                        End If

                                    End If

                                End If

                                'Validacion de la PlannedCauses con ProgrammedCauses
                                If bolRet Then
                                    Dim cauState As New Incidence.roProgrammedCauseState
                                    Dim strWhere = ""

                                    strWhere &= " ( ( (Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (IsNULL(FinishDate,Date) >= " & queryDateStart & " AND IsNULL(FinishDate,Date) <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (Date <= " & queryDateStart & " AND IsNULL(FinishDate,Date) >= " & queryDateEnd & ") ) AND  ("

                                    strWhere &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime)"
                                    strWhere &= " OR "
                                    strWhere &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime )"
                                    strWhere &= " OR "
                                    strWhere &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime)"
                                    strWhere &= " OR "
                                    strWhere &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime ) ) )"

                                    Dim tbCauses As DataTable = Incidence.roProgrammedCause.GetProgrammedCauses(Me.IDEmployee, strWhere, cauState)
                                    If tbCauses IsNot Nothing AndAlso tbCauses.Rows.Count > 0 Then
                                        oState.Result = RequestResultEnum.PlannedCausesOverlapped
                                        bolRet = False
                                    End If
                                End If

                                'Validacion de la PlannedCauses con ProgrammedAbsences
                                If bolRet Then
                                    Dim absState As New Absence.roProgrammedAbsenceState
                                    Dim strWhere = ""

                                    strWhere &= " ( ( (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (BeginDate <= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateEnd & ") ) )"

                                    Dim tbAbsences As DataTable = Absence.roProgrammedAbsence.GetProgrammedAbsences(Me.IDEmployee, strWhere, absState)
                                    If tbAbsences IsNot Nothing AndAlso tbAbsences.Rows.Count > 0 Then
                                        oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                        bolRet = False
                                    End If
                                End If

                                If bolRet Then
                                    ' Verificamos si existe otra prevision de vacaciones por horas en el mismo periodo
                                    strSQL = "@SELECT# * from ProgrammedHolidays " &
                                                         "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                    strSQL &= " Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & " AND "
                                    strSQL &= " ((" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime AND AllDay = 0)"
                                    strSQL &= " OR "
                                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime  AND AllDay = 0)"
                                    strSQL &= " OR "
                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime AND AllDay = 0)"
                                    strSQL &= " OR "
                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND AllDay = 0)  "
                                    ' o una prevision de vacaciones de todo el dia para la misma fecha
                                    strSQL &= " Or (AllDay = 1))"

                                    Dim tb As DataTable = CreateDataTable(strSQL, )
                                    If tb IsNot Nothing Then

                                        If tb.Rows.Count > 0 Then
                                            oState.Result = RequestResultEnum.AnotherHolidayExistInDate
                                            bolRet = False
                                        Else
                                            bolRet = True
                                        End If
                                    End If
                                End If

                                If bolRet Then
                                    ' Verificamos si existe otra prevision de exceso en el mismo periodo
                                    strSQL = "@SELECT# * from ProgrammedOvertimes " &
                                                         "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                    strSQL &= " ( (  (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
                                    strSQL &= " OR "
                                    strSQL &= " (EndDate >= " & queryDateStart & " AND EndDate <= " & queryDateEnd & ")"
                                    strSQL &= " OR "
                                    strSQL &= " (BeginDate <= " & queryDateStart & " AND EndDate >= " & queryDateEnd & ") ) AND  ("
                                    strSQL &= "(" & queryStartHour & " < BeginTime And " & queryEndHour & " > EndTime)"
                                    strSQL &= " Or "
                                    strSQL &= " (" & queryStartHour & " < BeginTime  And " & queryEndHour & " BETWEEN BeginTime And EndTime And " & queryEndHour & " <> BeginTime )"
                                    strSQL &= " Or "
                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime And EndTime And  EndTime <> " & queryStartHour & "  And " & queryEndHour & " > EndTime)"
                                    strSQL &= " Or "
                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime And EndTime   And " & queryEndHour & " BETWEEN BeginTime And EndTime ) ) ) "

                                    Dim tb As DataTable = CreateDataTable(strSQL, )
                                    If tb IsNot Nothing Then

                                        If tb.Rows.Count > 0 Then
                                            oState.Result = RequestResultEnum.AnotherOvertimeExistInDate
                                            bolRet = False
                                        Else
                                            bolRet = True
                                        End If
                                    End If

                                End If

                                ' No se debe solapar con ninguna solicitud de vacaciones por dias existente
                                If bolRet Then
                                    Dim strWhere As String = "(RequestType = " & eRequestType.VacationsOrPermissions & ") AND"

                                    strWhere &= " EXISTS ( @SELECT# 1 as ExistRec  from  sysroRequestDays WHERE Requests.ID = sysroRequestDays.IDRequest "
                                    strWhere &= " AND "
                                    strWhere &= " sysroRequestDays.Date>=" & queryDateStart & " AND sysroRequestDays.Date<=" & queryDateEnd & ") "
                                    strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                    Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                    If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                        If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.VacationsOrPermissions Then 'vacaciones por dias
                                            oState.Result = RequestResultEnum.VacationsOrPermissionsOverlapped
                                            bolRet = False
                                        End If
                                    End If
                                End If

                                If bolRet Then
                                    ' Verificamos que la solicitud no esté asignada a un dia con un horario de vacaciones planificado
                                    strSQL = "@SELECT# isnull(IsHolidays,0)  as Holidays from DailySchedule, Shifts " &
                                                    "WHERE Shifts.ID = DailySchedule.IDShift1  AND IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                    strSQL &= " Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & " AND ( isnull(IsHolidays,0) = 1 ) "
                                    Dim tb As DataTable = CreateDataTable(strSQL, )
                                    If tb IsNot Nothing Then
                                        If tb.Rows.Count > 0 Then
                                            oState.Result = RequestResultEnum.InHolidayPlanification
                                            bolRet = False
                                        Else
                                            bolRet = True
                                        End If
                                    End If
                                End If



                            Case eRequestType.ExternalWorkWeekResume
                                ' Validacion resumen trabajo semanal
                                If Not Me.dDate1.HasValue Or Not Me.dDate2.HasValue Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf dDate1.Value > dDate2.Value Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False

                                ElseIf Me.lstRequestDays.Count = 0 Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                End If

                                If bolRet Then
                                    For Each oRequestDay As roRequestDay In Me.lstRequestDays
                                        If Not oRequestDay.ActualType.HasValue Then
                                            oState.Result = RequestResultEnum.IncorrectActualType
                                            bolRet = False
                                            Exit For
                                        End If
                                    Next
                                End If

                            Case eRequestType.PlannedHolidays

                                'Validacion de la propia PlannedHolidays
                                If Me.lstRequestDays.Count = 0 Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf Not Me.IDCause.HasValue Then
                                    oState.Result = RequestResultEnum.CauseRequired
                                    bolRet = False
                                Else
                                    For Each oRequestDay As roRequestDay In Me.lstRequestDays
                                        Dim queryDate As String = roTypes.Any2Time(oRequestDay.RequestDate).SQLSmallDateTime()
                                        Dim queryStartHour As String = String.Empty
                                        Dim queryEndHour As String = String.Empty

                                        If oRequestDay.FromTime.HasValue Then queryStartHour = roTypes.Any2Time(oRequestDay.FromTime).SQLDateTime()
                                        If oRequestDay.ToTime.HasValue Then queryEndHour = roTypes.Any2Time(oRequestDay.ToTime).SQLDateTime()

                                        If Not oRequestDay.AllDay Then

                                            If oRequestDay.Duration <= 0 Then
                                                oState.Result = RequestResultEnum.HoursRequired
                                                bolRet = False
                                            ElseIf oRequestDay.Duration = 0 Or roTypes.Any2Time(oRequestDay.Duration).NumericValue > roTypes.Any2Time("23:59").NumericValue Then
                                                oState.Result = RequestResultEnum.HoursRequired
                                                bolRet = False
                                            ElseIf roTypes.Any2Time(oRequestDay.Duration).NumericValue <> roTypes.Any2Time(roTypes.Any2Time(oRequestDay.ToTime).NumericValue - roTypes.Any2Time(oRequestDay.FromTime).NumericValue).NumericValue Then
                                                oState.Result = RequestResultEnum.HoursRequired
                                                bolRet = False
                                            Else

                                                If bolRet Then
                                                    Dim strWhere As String = "(RequestType = " & eRequestType.PlannedHolidays & ") AND"

                                                    strWhere &= " EXISTS ( @SELECT# 1 as ExistRec from  sysroRequestDays WHERE  Requests.ID = sysroRequestDays.IDRequest AND IDRequest <>   " & Me.ID
                                                    strWhere &= " AND "
                                                    strWhere &= " sysroRequestDays.Date=" & queryDate & " AND "

                                                    strWhere &= " ((" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime AND AllDay = 0)"
                                                    strWhere &= " OR "
                                                    strWhere &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime  AND AllDay = 0)"
                                                    strWhere &= " OR "
                                                    strWhere &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime AND AllDay = 0)"
                                                    strWhere &= " OR "
                                                    strWhere &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND AllDay = 0)  "
                                                    ' o una prevision de vacaciones de todo el dia para la misma fecha
                                                    strWhere &= " Or (sysroRequestDays.AllDay = 1))  )"
                                                    strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                                    Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                                    If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                                        If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedHolidays Then 'vacaciones/permisos por horas
                                                            oState.Result = RequestResultEnum.PlannedHolidaysOverlapped
                                                            bolRet = False
                                                        End If
                                                    End If
                                                End If

                                                If bolRet Then
                                                    Dim strWhere As String = "(RequestType = " & eRequestType.PlannedCauses & ") AND"

                                                    strWhere &= " ( ( (Date1 >= " & queryDate & " AND Date1 <= " & queryDate & ")"
                                                    strWhere &= " OR "
                                                    strWhere &= " (IsNULL(Date2,Date1) >= " & queryDate & " AND IsNULL(Date2,Date1) <= " & queryDate & ")"
                                                    strWhere &= " OR "
                                                    strWhere &= " (Date1 <= " & queryDate & " AND IsNULL(Date2,Date1) >= " & queryDate & ") ) AND  ("

                                                    strWhere &= " (ToTime > " & queryStartHour & " AND " & queryStartHour & " BETWEEN FromTime AND ToTime)"
                                                    strWhere &= " OR "
                                                    strWhere &= " (" & queryEndHour & " > FromTime AND " & queryStartHour & " < FromTime) ) )"

                                                    strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                                    Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                                    If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                                        If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedCauses Then 'Ausencia por horas
                                                            oState.Result = RequestResultEnum.PlannedCausesOverlapped
                                                            bolRet = False
                                                        End If
                                                    End If

                                                    If bolRet Then
                                                        strWhere = "(RequestType = " & eRequestType.PlannedAbsences & ") AND"

                                                        strWhere &= " ( ( (Date1 >= " & queryDate & " AND Date1 <= " & queryDate & ")"
                                                        strWhere &= " OR "
                                                        strWhere &= " (IsNULL(Date2,Date1) >= " & queryDate & " AND IsNULL(Date2,Date1) <= " & queryDate & ")"
                                                        strWhere &= " OR "
                                                        strWhere &= " (Date1 <= " & queryDate & " AND IsNULL(Date2,Date1) >= " & queryDate & ") ) )"

                                                        strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                                        tbRequest = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                                        If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                                            If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedAbsences Then 'Ausencia por días
                                                                oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                                                bolRet = False
                                                            End If
                                                        End If
                                                    End If

                                                    If bolRet Then
                                                        strWhere = "(RequestType = " & eRequestType.PlannedOvertimes & ") AND"

                                                        strWhere &= " ( ( (Date1 >= " & queryDate & " AND Date1 <= " & queryDate & ")"
                                                        strWhere &= " OR "
                                                        strWhere &= " (IsNULL(Date2,Date1) >= " & queryDate & " AND IsNULL(Date2,Date1) <= " & queryDate & ")"
                                                        strWhere &= " OR "
                                                        strWhere &= " (Date1 <= " & queryDate & " AND IsNULL(Date2,Date1) >= " & queryDate & ") ) AND  ("

                                                        strWhere &= " (ToTime > " & queryStartHour & " AND " & queryStartHour & " BETWEEN FromTime AND ToTime)"
                                                        strWhere &= " OR "
                                                        strWhere &= " (" & queryEndHour & " > FromTime AND " & queryStartHour & " < FromTime) ) )"

                                                        strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                                        tbRequest = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                                        If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                                            If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedOvertimes Then 'exceso
                                                                oState.Result = RequestResultEnum.PlannedOvertimesOverlapped
                                                                bolRet = False
                                                            End If
                                                        End If

                                                    End If
                                                End If

                                                If bolRet Then
                                                    ' Verificamos si existe otra prevision de vacaciones por horas en el mismo periodo
                                                    strSQL = "@SELECT# * from ProgrammedHolidays " &
                                                         "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                                    strSQL &= " Date = " & queryDate & " AND "
                                                    strSQL &= " ((" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime AND AllDay = 0)"
                                                    strSQL &= " OR "
                                                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime  AND AllDay = 0)"
                                                    strSQL &= " OR "
                                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime AND AllDay = 0)"
                                                    strSQL &= " OR "
                                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND AllDay = 0)  "
                                                    ' o una prevision de vacaciones de todo el dia para la misma fecha
                                                    strSQL &= " Or (AllDay = 1))"

                                                    Dim tb As DataTable = CreateDataTable(strSQL, )
                                                    If tb IsNot Nothing Then

                                                        If tb.Rows.Count > 0 Then
                                                            oState.Result = RequestResultEnum.AnotherHolidayExistInDate
                                                            bolRet = False
                                                        Else
                                                            bolRet = True
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        Else
                                            ' Si es del dia completo
                                            If bolRet Then
                                                Dim strWhere As String = "(RequestType = " & eRequestType.PlannedHolidays & ") AND"

                                                strWhere &= " EXISTS ( @SELECT# 1 as ExistRec  from  sysroRequestDays WHERE Requests.ID = sysroRequestDays.IDRequest AND IDRequest <>   " & Me.ID
                                                strWhere &= " AND "
                                                strWhere &= " sysroRequestDays.Date=" & queryDate & ") "
                                                strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                                Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                                If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                                    If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedHolidays Then 'vacaciones/permisos por horas
                                                        oState.Result = RequestResultEnum.PlannedHolidaysOverlapped
                                                        bolRet = False
                                                    End If
                                                End If
                                            End If

                                            If bolRet Then
                                                Dim strWhere As String = "(RequestType = " & eRequestType.PlannedCauses & " or RequestType = " & eRequestType.PlannedAbsences & ") And "

                                                strWhere &= " ( ( (Date1 >= " & queryDate & " And Date1 <= " & queryDate & ")"
                                                strWhere &= " Or "
                                                strWhere &= " (IsNULL(Date2,Date1) >= " & queryDate & " And IsNULL(Date2,Date1) <= " & queryDate & ")"
                                                strWhere &= " Or "
                                                strWhere &= " (Date1 <= " & queryDate & " And IsNULL(Date2,Date1) >= " & queryDate & ") ) )"
                                                strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                                Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                                If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                                    If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedCauses Then 'Ausencia por horas
                                                        oState.Result = RequestResultEnum.PlannedCausesOverlapped
                                                        bolRet = False
                                                    End If
                                                    If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedAbsences Then 'Ausencia por dias
                                                        oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                                        bolRet = False
                                                    End If
                                                End If
                                            End If

                                            If bolRet Then
                                                Dim strWhere As String = "(RequestType = " & eRequestType.PlannedOvertimes & ") And "

                                                strWhere &= " ( ( (Date1 >= " & queryDate & " And Date1 <= " & queryDate & ")"
                                                strWhere &= " Or "
                                                strWhere &= " (IsNULL(Date2,Date1) >= " & queryDate & " And IsNULL(Date2,Date1) <= " & queryDate & ")"
                                                strWhere &= " Or "
                                                strWhere &= " (Date1 <= " & queryDate & " And IsNULL(Date2,Date1) >= " & queryDate & ") ) )"
                                                strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                                Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                                If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                                    If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedOvertimes Then ' exceso
                                                        oState.Result = RequestResultEnum.PlannedOvertimesOverlapped
                                                        bolRet = False
                                                    End If
                                                End If
                                            End If

                                            If bolRet Then
                                                ' Si es de todo el dia, verificamos que no hay otra prevision de vacaciones el mismo dia
                                                ' ya sea de todo el dia o de un único periodo
                                                strSQL = "@SELECT# * from ProgrammedHolidays " &
                                                    "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                                strSQL &= " Date = " & queryDate
                                                Dim tb As DataTable = CreateDataTable(strSQL, )
                                                If tb IsNot Nothing Then
                                                    If tb.Rows.Count > 0 Then
                                                        oState.Result = RequestResultEnum.AnotherHolidayExistInDate
                                                        bolRet = False
                                                    Else
                                                        bolRet = True
                                                    End If
                                                End If

                                            End If
                                        End If

                                        ' Verificamos si existe una prevision de dias de ausencia para el mismo dia
                                        If bolRet Then
                                            strSQL = "@SELECT# * from ProgrammedAbsences WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                            strSQL &= " ( ( (BeginDate >= " & queryDate & " AND BeginDate <= " & queryDate & ")"
                                            strSQL &= " OR "
                                            strSQL &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDate & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & queryDate & ")"
                                            strSQL &= " OR "
                                            strSQL &= " (BeginDate <= " & queryDate & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDate & ") ) )"

                                            Dim tbX As DataTable = CreateDataTable(strSQL, )
                                            If tbX IsNot Nothing Then
                                                If tbX.Rows.Count > 0 Then
                                                    oState.Result = RequestResultEnum.AnotherAbsenceExistInDate
                                                    bolRet = False
                                                Else
                                                    bolRet = True
                                                End If
                                            End If
                                        End If

                                        If bolRet Then
                                            ' Verificamos si existe una prevision de horas de ausencia para el mismo dia
                                            If Not oRequestDay.AllDay Then
                                                ' Si es de un periodo concreto
                                                strSQL = "@SELECT# * from ProgrammedCauses " &
                                                    "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                                strSQL &= " ( ( (Date >= " & queryDate & " AND Date <= " & queryDate & ")"
                                                strSQL &= " OR "
                                                strSQL &= " (IsNULL(FinishDate,Date) >= " & queryDate & " AND IsNULL(FinishDate,Date) <= " & queryDate & ")"
                                                strSQL &= " OR "
                                                strSQL &= " (Date <= " & queryDate & " AND IsNULL(FinishDate,Date) >= " & queryDate & ") ) AND  ("
                                                strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime)"
                                                strSQL &= " OR "
                                                strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime )"
                                                strSQL &= " OR "
                                                strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime)"
                                                strSQL &= " OR "
                                                strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime ) ) )"

                                                Dim tb As DataTable = CreateDataTable(strSQL, )
                                                If tb IsNot Nothing Then
                                                    If tb.Rows.Count > 0 Then
                                                        oState.Result = RequestResultEnum.AnotherAbsenceExistInDate
                                                        bolRet = False
                                                    Else
                                                        bolRet = True
                                                    End If
                                                End If

                                                If bolRet Then
                                                    strSQL = "@SELECT# * from ProgrammedOvertimes " &
                                                    "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                                    strSQL &= " ( (  (BeginDate >= " & queryDate & " AND BeginDate <= " & queryDate & ")"
                                                    strSQL &= " OR "
                                                    strSQL &= " (EndDate >= " & queryDate & " AND EndDate <= " & queryDate & ")"
                                                    strSQL &= " OR "
                                                    strSQL &= " (BeginDate <= " & queryDate & " AND EndDate >= " & queryDate & ") )  AND  ("
                                                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime)"
                                                    strSQL &= " OR "
                                                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime )"
                                                    strSQL &= " OR "
                                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime)"
                                                    strSQL &= " OR "
                                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime ) ) )"

                                                    tb = CreateDataTable(strSQL, )
                                                    If tb IsNot Nothing Then
                                                        If tb.Rows.Count > 0 Then
                                                            oState.Result = RequestResultEnum.AnotherOvertimeExistInDate
                                                            bolRet = False
                                                        Else
                                                            bolRet = True
                                                        End If
                                                    End If

                                                End If
                                            Else
                                                ' si es de un dia completo
                                                strSQL = "@SELECT# * from ProgrammedCauses " &
                                                    "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                                strSQL &= "  ( (Date >= " & queryDate & " AND Date <= " & queryDate & ")"
                                                strSQL &= " OR "
                                                strSQL &= " (IsNULL(FinishDate,Date) >= " & queryDate & " AND IsNULL(FinishDate,Date) <= " & queryDate & ")"
                                                strSQL &= " OR "
                                                strSQL &= " (Date <= " & queryDate & " AND IsNULL(FinishDate,Date) >= " & queryDate & ") ) "
                                                Dim tb As DataTable = CreateDataTable(strSQL, )
                                                If tb IsNot Nothing Then
                                                    If tb.Rows.Count > 0 Then
                                                        oState.Result = RequestResultEnum.AnotherAbsenceExistInDate
                                                        bolRet = False
                                                    Else
                                                        bolRet = True
                                                    End If
                                                End If

                                                If bolRet Then
                                                    strSQL = "@SELECT# * from ProgrammedOvertimes " &
                                                    "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                                    strSQL &= "  (  (BeginDate >= " & queryDate & " AND BeginDate <= " & queryDate & ")"
                                                    strSQL &= " OR "
                                                    strSQL &= " (EndDate >= " & queryDate & " AND EndDate <= " & queryDate & ")"
                                                    strSQL &= " OR "
                                                    strSQL &= " (BeginDate <= " & queryDate & " AND EndDate >= " & queryDate & ") )"
                                                    tb = CreateDataTable(strSQL)
                                                    If tb IsNot Nothing Then
                                                        If tb.Rows.Count > 0 Then
                                                            oState.Result = RequestResultEnum.AnotherOvertimeExistInDate
                                                            bolRet = False
                                                        Else
                                                            bolRet = True
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If

                                        If bolRet Then
                                            ' Verificamos que la prevision no esté asignada a un dia con un horario de vacaciones planificado o de no trabajo
                                            strSQL = "@SELECT# isnull(IsHolidays,0)  as Holidays from DailySchedule, Shifts " &
                                                "WHERE Shifts.ID = DailySchedule.IDShift1  AND IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                            strSQL &= " Date = " & queryDate & " AND ( isnull(IsHolidays,0) = 1 OR  isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  = 0 ) "
                                            Dim tb As DataTable = CreateDataTable(strSQL, )
                                            If tb IsNot Nothing Then
                                                If tb.Rows.Count > 0 Then

                                                    'Si el dia anterior o posterior hay un horario nocturno permitimos qu ese realize la solicitud
                                                    strSQL = "@SELECT# count(id) from Shifts where id in( " &
                                                        "@SELECT# distinct isnull(IDShiftUsed,IDShift1) from DailySchedule where IDEmployee = " & Me.IDEmployee.ToString &
                                                        " and Date between " & roTypes.Any2Time(oRequestDay.RequestDate.AddDays(-1)).SQLSmallDateTime() & " and " & roTypes.Any2Time(oRequestDay.RequestDate.AddDays(1)).SQLSmallDateTime() &
                                                        ") and DAY(Shifts.StartLimit) <> DAY(Shifts.EndLimit)"

                                                    Dim countNightShifts As Integer = roTypes.Any2Integer(ExecuteScalar(strSQL))
                                                    If countNightShifts = 0 Then
                                                        oState.Result = RequestResultEnum.InHolidayPlanification
                                                        bolRet = False
                                                    End If
                                                End If
                                            End If
                                        End If

                                        If bolRet Then
                                            ' verificamos que no haya otra solicitud de vacaciones por dia para el mismo dia
                                            Dim strWhere As String = "(RequestType = " & eRequestType.VacationsOrPermissions & ") AND"

                                            strWhere &= " EXISTS ( @SELECT# 1 as ExistRec  from  sysroRequestDays WHERE Requests.ID = sysroRequestDays.IDRequest AND IDRequest <>   " & Me.ID
                                            strWhere &= " AND "
                                            strWhere &= " sysroRequestDays.Date=" & queryDate & ") "
                                            strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                            Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                            If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                                If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.VacationsOrPermissions Then 'vacaciones por dias
                                                    oState.Result = RequestResultEnum.VacationsOrPermissionsOverlapped
                                                    bolRet = False
                                                End If
                                            End If
                                        End If

                                        If Not bolRet Then Exit For
                                    Next
                                End If

                            Case eRequestType.PlannedOvertimes
                                'Validacion de la propia PlannedOvertimes
                                If Not Me.dDate1.HasValue Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf Not (Me.IDCause.HasValue AndAlso Me.IDCause.Value > 0) Then
                                    oState.Result = RequestResultEnum.CauseRequired
                                    bolRet = False
                                ElseIf Not Me.FromTime.HasValue Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf Not Me.ToTime.HasValue Then
                                    oState.Result = RequestResultEnum.IncorrectDates
                                    bolRet = False
                                ElseIf Me.dblHours <= 0 Then
                                    oState.Result = RequestResultEnum.HoursRequired
                                    bolRet = False
                                ElseIf Me.dblHours = 0 Or roTypes.Any2Time(Me.dblHours).NumericValue > roTypes.Any2Time("23:59").NumericValue Then
                                    oState.Result = RequestResultEnum.HoursRequired
                                    bolRet = False
                                ElseIf roTypes.Any2Time(Me.dblHours).NumericValue > roTypes.Any2Time(roTypes.Any2Time(Me.ToTime.Value).NumericValue - roTypes.Any2Time(Me.FromTime.Value).NumericValue).NumericValue Then
                                    oState.Result = RequestResultEnum.HoursRequired
                                    bolRet = False
                                End If

                                Dim queryDateStart As String = roTypes.Any2Time(Me.Date1).SQLSmallDateTime()
                                Dim queryDateEnd As String = IIf(Me.Date2.HasValue, roTypes.Any2Time(Me.Date2).SQLSmallDateTime(), roTypes.Any2Time(Me.Date1).SQLSmallDateTime())
                                Dim queryStartHour As String = roTypes.Any2Time(Me.FromTime).SQLDateTime()
                                Dim queryEndHour As String = roTypes.Any2Time(Me.ToTime).SQLDateTime()

                                'Validacion de la PlannedOvertimes con respecto a otras
                                'Que este en un periodo en el que nosotros también estemos

                                If bolRet Then
                                    Dim strWhere As String = "(RequestType = " & eRequestType.PlannedOvertimes & ") AND"

                                    strWhere &= " ( ( (Date1 >= " & queryDateStart & " AND Date1 <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (IsNULL(Date2,Date1) >= " & queryDateStart & " AND IsNULL(Date2,Date1) <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (Date1 <= " & queryDateStart & " AND IsNULL(Date2,Date1) >= " & queryDateEnd & ") ) AND  ("

                                    strWhere &= " (ToTime > " & queryStartHour & " AND " & queryStartHour & " BETWEEN FromTime AND ToTime)"
                                    strWhere &= " OR "
                                    strWhere &= " (" & queryEndHour & " > FromTime AND " & queryStartHour & " < FromTime) ) )"

                                    strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                    Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                    If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                        If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedOvertimes Then ' horas de exceso
                                            oState.Result = RequestResultEnum.PlannedOvertimesOverlapped
                                            bolRet = False
                                        End If
                                    End If

                                    If bolRet Then
                                        strWhere = "(RequestType = " & eRequestType.PlannedAbsences & ") AND"

                                        strWhere &= " ( ( (Date1 >= " & queryDateStart & " AND Date1 <= " & queryDateEnd & ")"
                                        strWhere &= " OR "
                                        strWhere &= " (IsNULL(Date2,Date1) >= " & queryDateStart & " AND IsNULL(Date2,Date1) <= " & queryDateEnd & ")"
                                        strWhere &= " OR "
                                        strWhere &= " (Date1 <= " & queryDateStart & " AND IsNULL(Date2,Date1) >= " & queryDateEnd & ") ) )"

                                        strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                        tbRequest = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                        If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                            If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedAbsences Then 'Ausencia por días
                                                oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                                bolRet = False
                                            End If
                                        End If
                                    End If

                                    If bolRet Then
                                        strWhere = "(RequestType = " & eRequestType.PlannedHolidays & ") AND"

                                        strWhere &= " EXISTS ( @SELECT# 1 as ExistRec from  sysroRequestDays WHERE  Requests.ID = sysroRequestDays.IDRequest AND IDRequest <>   " & Me.ID
                                        strWhere &= " AND "
                                        strWhere &= " sysroRequestDays.Date>=" & queryDateStart & " AND sysroRequestDays.Date<=" & queryDateEnd & " AND "

                                        strWhere &= " ((" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime AND AllDay = 0)"
                                        strWhere &= " OR "
                                        strWhere &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime  AND AllDay = 0)"
                                        strWhere &= " OR "
                                        strWhere &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime AND AllDay = 0)"
                                        strWhere &= " OR "
                                        strWhere &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND AllDay = 0)  "
                                        ' o una prevision de vacaciones de todo el dia para la misma fecha
                                        strWhere &= " Or (sysroRequestDays.AllDay = 1))  )"
                                        strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                        tbRequest = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                        If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                            If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedHolidays Then 'vacaciones/permisos por horas
                                                oState.Result = RequestResultEnum.PlannedHolidaysOverlapped
                                                bolRet = False
                                            End If
                                        End If
                                    End If

                                End If

                                If bolRet Then
                                    'Validacion contra plannedcauses
                                    Dim strWhere As String = "(RequestType = " & eRequestType.PlannedCauses & ") AND"

                                    strWhere &= " ( ( (Date1 >= " & queryDateStart & " AND Date1 <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (IsNULL(Date2,Date1) >= " & queryDateStart & " AND IsNULL(Date2,Date1) <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (Date1 <= " & queryDateStart & " AND IsNULL(Date2,Date1) >= " & queryDateEnd & ") ) AND  ("

                                    strWhere &= " (ToTime > " & queryStartHour & " AND " & queryStartHour & " BETWEEN FromTime AND ToTime)"
                                    strWhere &= " OR "
                                    strWhere &= " (" & queryEndHour & " > FromTime AND " & queryStartHour & " < FromTime) ) )"

                                    strWhere &= " AND Status <> " & eRequestStatus.Denied & " AND Status <> " & eRequestStatus.Accepted & " AND Status <> " & eRequestStatus.Canceled

                                    Dim tbRequest As DataTable = GetRequestsByEmployee(Me.IDEmployee, strWhere, "", oState)
                                    If tbRequest IsNot Nothing AndAlso tbRequest.Rows.Count > 0 Then
                                        If roTypes.Any2Integer(tbRequest.Rows(0)("RequestType")) = eRequestType.PlannedCauses Then ' horas de exceso
                                            oState.Result = RequestResultEnum.PlannedCausesOverlapped
                                            bolRet = False
                                        End If
                                    End If

                                End If

                                'Validacion de la PlannedCauses con ProgrammedCauses
                                If bolRet Then
                                    Dim cauState As New Incidence.roProgrammedCauseState
                                    Dim strWhere = ""

                                    strWhere &= " ( ( (Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (IsNULL(FinishDate,Date) >= " & queryDateStart & " AND IsNULL(FinishDate,Date) <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (Date <= " & queryDateStart & " AND IsNULL(FinishDate,Date) >= " & queryDateEnd & ") ) AND  ("

                                    strWhere &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime)"
                                    strWhere &= " OR "
                                    strWhere &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime )"
                                    strWhere &= " OR "
                                    strWhere &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime)"
                                    strWhere &= " OR "
                                    strWhere &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime ) ) )"

                                    Dim tbCauses As DataTable = Incidence.roProgrammedCause.GetProgrammedCauses(Me.IDEmployee, strWhere, cauState)
                                    If tbCauses IsNot Nothing AndAlso tbCauses.Rows.Count > 0 Then
                                        oState.Result = RequestResultEnum.PlannedCausesOverlapped
                                        bolRet = False
                                    End If
                                End If

                                'Validacion de la PlannedCauses con ProgrammedAbsences
                                If bolRet Then
                                    Dim absState As New Absence.roProgrammedAbsenceState
                                    Dim strWhere = ""

                                    strWhere &= " ( ( (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & queryDateEnd & ")"
                                    strWhere &= " OR "
                                    strWhere &= " (BeginDate <= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateEnd & ") ) )"

                                    Dim tbAbsences As DataTable = Absence.roProgrammedAbsence.GetProgrammedAbsences(Me.IDEmployee, strWhere, absState)
                                    If tbAbsences IsNot Nothing AndAlso tbAbsences.Rows.Count > 0 Then
                                        oState.Result = RequestResultEnum.PlannedAbsencesOverlapped
                                        bolRet = False
                                    End If
                                End If

                                If bolRet Then
                                    ' Verificamos si existe otra prevision de vacaciones por horas en el mismo periodo
                                    strSQL = "@SELECT# * from ProgrammedHolidays " &
                                                         "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                    strSQL &= " Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & " AND "
                                    strSQL &= " ((" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime AND AllDay = 0)"
                                    strSQL &= " OR "
                                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime  AND AllDay = 0)"
                                    strSQL &= " OR "
                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime AND AllDay = 0)"
                                    strSQL &= " OR "
                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND AllDay = 0)  "
                                    ' o una prevision de vacaciones de todo el dia para la misma fecha
                                    strSQL &= " Or (AllDay = 1))"

                                    Dim tb As DataTable = CreateDataTable(strSQL, )
                                    If tb IsNot Nothing Then

                                        If tb.Rows.Count > 0 Then
                                            oState.Result = RequestResultEnum.AnotherHolidayExistInDate
                                            bolRet = False
                                        Else
                                            bolRet = True
                                        End If
                                    End If
                                End If

                                If bolRet Then
                                    ' Verificamos si existe otra prevision de horas de exceso el mismo periodo
                                    strSQL = "@SELECT# * from ProgrammedOvertimes " &
                                                         "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND "
                                    strSQL &= " ( (  (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
                                    strSQL &= " OR "
                                    strSQL &= " (EndDate >= " & queryDateStart & " AND EndDate <= " & queryDateEnd & ")"
                                    strSQL &= " OR "
                                    strSQL &= " (BeginDate <= " & queryDateStart & " AND EndDate >= " & queryDateEnd & ") )  AND  ("

                                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime)"
                                    strSQL &= " OR "
                                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime )"
                                    strSQL &= " OR "
                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime)"
                                    strSQL &= " OR "
                                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime ) ) )"
                                    Dim tb As DataTable = CreateDataTable(strSQL, )
                                    If tb IsNot Nothing Then

                                        If tb.Rows.Count > 0 Then
                                            oState.Result = RequestResultEnum.AnotherOvertimeExistInDate
                                            bolRet = False
                                        Else
                                            bolRet = True
                                        End If
                                    End If
                                End If

                            Case eRequestType.ForbiddenTaskPunch
                                If Not Me.dDate1.HasValue Then
                                    oState.Result = RequestResultEnum.PunchDateTimeRequired
                                    bolRet = False
                                End If

                                If Not Me.intIDTask1.HasValue Then
                                    oState.Result = RequestResultEnum.TaskRequiered
                                    bolRet = False
                                End If

                                If Me.dDate2.HasValue Then
                                    If Not Me.intIDTask2.HasValue Then
                                        oState.Result = RequestResultEnum.TaskRequiered
                                        bolRet = False
                                    End If
                                End If

                                If bolRet Then
                                    ' Verificamos que no existan fichajes iguales con la misma tarea o que se solapen dentro del mismo periodo
                                    Dim oPunchState As New Punch.roPunchState
                                    roBusinessState.CopyTo(Me.oState, oPunchState)

                                    If Not Me.Date2.HasValue Then
                                        Dim tbPunches As DataTable = Punch.roPunch.GetPunches("IDEmployee = " & Me.IDEmployee.ToString & " And " &
                                                                                         " DateTime = " & Any2Time(Me.Date1).SQLDateTime & " And ActualType In(" & PunchTypeEnum._TASK & ")" & " And TypeData=" & Me.IDTask1,
                                                                                         oPunchState)
                                        If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then
                                            oState.Result = RequestResultEnum.PunchExist
                                            bolRet = False
                                        End If

                                    ElseIf Me.Date2.HasValue And Me.IDTask2.HasValue Then
                                        Dim tbPunches As DataTable = Punch.roPunch.GetPunches("IDEmployee = " & Me.IDEmployee.ToString & " And " &
                                                                                         " DateTime > " & Any2Time(Me.Date1).SQLDateTime & " DateTime < " & Any2Time(Me.Date2).SQLDateTime & " And ActualType In(" & PunchTypeEnum._TASK & ")",
                                                                                         oPunchState)
                                        If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then
                                            oState.Result = RequestResultEnum.PunchExist
                                            bolRet = False
                                        End If
                                    End If
                                End If
                            Case eRequestType.ForgottenCostCenterPunch
                                If Not Me.dDate1.HasValue Then
                                    oState.Result = RequestResultEnum.PunchDateTimeRequired
                                    bolRet = False
                                End If
                                If (Not Me.IDCenter.HasValue) OrElse (Me.IDCenter.HasValue AndAlso Me.IDCenter.Value <= 0) Then
                                    oState.Result = RequestResultEnum.CostCenterRequiered
                                    bolRet = False
                                End If
                                If bolRet Then
                                    ' Verificamos que no exista un fichaje en la misma fecha y hora
                                    Dim oPunchState As New Punch.roPunchState
                                    roBusinessState.CopyTo(Me.oState, oPunchState)
                                    Dim tbPunches As DataTable = Punch.roPunch.GetPunches("IDEmployee = " & Me.IDEmployee.ToString & " And " &
                                                                                     " DateTime = " & Any2Time(Me.Date1).SQLDateTime & " And ActualType In(" & PunchTypeEnum._CENTER & ")", oPunchState)
                                    If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then
                                        oState.Result = RequestResultEnum.PunchExist
                                        bolRet = False
                                    End If
                                End If

                                If bolRet Then
                                    ' Verificamos que el fichaje no sea anterior a la fecha de cierre
                                    Dim xFreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee, False, oState)
                                    If Any2Time(Me.Date1).DateOnly <= xFreezeDate Then
                                        oState.Result = RequestResultEnum.FreezeDateException
                                        bolRet = False
                                    End If

                                End If

                        End Select
                    End If

                    ' Revisamos si debemos aplicar reglas de validacion de solicitudes,
                    ' en caso necesario
                    If bolRet Then

                        Dim bolIgnoreAutomaticValidationRule As Boolean = False
                        ' en el caso que la solicitud sea de tipo intercambio de horarios entre empleados
                        ' no debemos validar la regla de validacion automatica
                        ' hasta que el segundo empleado la apruebe
                        If Me.RequestType = eRequestType.ExchangeShiftBetweenEmployees Then
                            bolIgnoreAutomaticValidationRule = True
                        End If

                        ' Validamos las reglas de solicitudes
                        bolRet = roRequestRuleManager.RequestRulesValidate(Me, oState, ,,, bolIgnoreAutomaticValidationRule)

                        ' Si el empleado ha indicado que quiere continuar,
                        ' guardamos la solicitud, sin tener en cuenta las reglas
                        If bSaveAnswer And Not bolRet Then
                            bolRet = True
                            oState.Result = RequestResultEnum.NoError
                            oState.ErrorDetail = ""
                            oState.ErrorText = ""
                        End If

                    End If

                End If
            Catch ex As DbException
                bolRet = False
                oState.UpdateStateInfo(ex, "roRequest::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::Validate")
                bolRet = False
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False, Optional ByVal strCommentToAudit As String = "", Optional ByVal bolIsImporting As Boolean = False, Optional ByVal bolGenerateNotification As Boolean = True) As Boolean
            Return Me.SaveWithParams(False, False, bAudit, strCommentToAudit, bolIsImporting, bolGenerateNotification)
        End Function

        Public Function SaveWithParams(ByVal bSaveAnswer As Boolean, ByVal bApplyValidations As Boolean, Optional ByVal bAudit As Boolean = False, Optional ByVal strCommentToAudit As String = "", Optional ByVal bolIsImporting As Boolean = False, Optional ByVal bolGenerateNotification As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Dim bIsNew As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.oState.Result = RequestResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                bolRet = True

                If Me.RequestType = eRequestType.VacationsOrPermissions Or Me.RequestType = eRequestType.CancelHolidays Then
                    ' En el caso de una solicitud de dias de vacaciones o Cancelacion de vacaciones
                    ' si viene con formato antiguo, añadimos los datos necesarios
                    ' para que sea compatible con el comportamiento actual
                    If Me.lstRequestDays Is Nothing OrElse Me.lstRequestDays.Count = 0 Then
                        bolRet = Me.GenerateRequestDays()
                    End If
                End If

                If bolRet Then
                    bolRet = False
                    If Me.Validate(, bolIsImporting, bApplyValidations, bSaveAnswer, (Me.RequestType = eRequestType.ExchangeShiftBetweenEmployees AndAlso (Me.RequestStatus <> eRequestStatus.Denied AndAlso Me.RequestStatus <> eRequestStatus.Accepted))) Then
                        Dim oAuditDataOld As DataRow = Nothing
                        Dim oAuditDataNew As DataRow = Nothing

                        Dim oOldRequest As roRequest = Nothing

                        Dim tb As New DataTable("Requests")
                        Dim strSQL As String = "@SELECT# * FROM Requests " &
                                               "WHERE ID = " & Me.ID.ToString
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tb)

                        Dim oRow As DataRow
                        If tb.Rows.Count = 0 Then
                            oRow = tb.NewRow
                            oRow("ID") = Me.GetNextID()
                            Me.ID = oRow("ID")
                            bIsNew = True
                        Else
                            oOldRequest = New roRequest(Me.ID, Me.State)
                            oRow = tb.Rows(0)
                            oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                        End If

                        oRow("IDEmployee") = intIDEmployee
                        oRow("RequestType") = iRequestType
                        oRow("RequestDate") = dRequestDate
                        oRow("Comments") = strComments
                        oRow("Status") = iStatus

                        If bIsNew OrElse Me.iStatus = eRequestStatus.Canceled Then
                            ' Debemos asignar el nivel mínimo de aprobación que se necesita para poder empezar a gestionarla
                            Dim strSQLLevel As String = "@SELECT# isnull(dbo.GetMinLevelOfRequest(" & Me.RequestType & "," & IIf(Me.IDCause.HasValue, Me.IDCause.ToString, "0") & "),11) AS MinLevel "
                            iStatusLevel = Any2Integer(ExecuteScalar(strSQLLevel)) + 1
                        End If

                        If iStatusLevel.HasValue Then
                            oRow("StatusLevel") = iStatusLevel.Value
                        Else
                            oRow("StatusLevel") = DBNull.Value
                        End If
                        If dDate1.HasValue Then oRow("Date1") = dDate1.Value
                        If dDate2.HasValue Then oRow("Date2") = dDate2.Value
                        oRow("FieldName") = strFieldName
                        oRow("FieldValue") = strFieldValue
                        If intIDCause.HasValue Then oRow("IDCause") = intIDCause
                        If intIDCenter.HasValue Then oRow("IDCenter") = intIDCenter
                        If dblHours.HasValue Then oRow("Hours") = dblHours
                        If intIDShift.HasValue Then oRow("IDShift") = intIDShift
                        If intIDTask1.HasValue Then oRow("IDTask1") = intIDTask1
                        If intIDTask2.HasValue Then oRow("IDTask2") = intIDTask2
                        If Not strField1 Is Nothing Then oRow("Field1") = strField1
                        If Not strField2 Is Nothing Then oRow("Field2") = strField2
                        If Not strField3 Is Nothing Then oRow("Field3") = strField3
                        'If dblField4 <> -1 Then oRow("Field4") = dblField4 Else oRow("Field4") = DBNull.Value
                        'If dblField5 <> -1 Then oRow("Field5") = dblField5 Else oRow("Field5") = DBNull.Value
                        'If dblField6 <> -1 Then oRow("Field6") = dblField6 Else oRow("Field6") = DBNull.Value
                        oRow("Field4") = dblField4
                        oRow("Field5") = dblField5
                        oRow("Field6") = dblField6
                        If bolCompletedTask.HasValue Then oRow("CompletedTask") = bolCompletedTask

                        If Me.xStartShift.HasValue Then oRow("StartShift") = Me.xStartShift
                        If Me.xFromTime.HasValue Then oRow("FromTime") = Me.xFromTime
                        If Me.xToTime.HasValue Then oRow("ToTime") = Me.xToTime

                        If intIDEmployeeExchange.HasValue Then oRow("IDEmployeeExchange") = intIDEmployeeExchange
                        oRow("NotReaded") = Me.bolNotReaded

                        oRow("AutomaticValidation") = bolAutomaticValidation
                        If dValidationDate.HasValue Then oRow("ValidationDate") = dValidationDate.Value

                        If dRejectedDate.HasValue Then oRow("RejectedDate") = dRejectedDate.Value

                        oRow("CRC") = Me.CRC

                        If tb.Rows.Count = 0 Then
                            tb.Rows.Add(oRow)
                        End If
                        da.Update(tb)

                        ' Gestión de alertas de solicitud pendiente (si procede)
                        If bolGenerateNotification AndAlso Me.RequestType <> eRequestType.DailyRecord Then
                            Dim oNotificationState As New Notifications.roNotificationState(Me.oState.IDPassport)
                            Notifications.roNotification.GenerateNotificationsForRequest(Me.ID, bIsNew, oNotificationState, Not Me.AutomaticValidation)
                        End If

                        ' Actualizamos la aprobaciones de la solicitud
                        If Me.lstRequestApprovals IsNot Nothing AndAlso Me.lstRequestApprovals.Count > 0 Then
                            bolRet = roRequestApproval.SaveRequestApprovals(Me.ID, Me.IDEmployee, Me.RequestType, Me.lstRequestApprovals, Me.oState)
                        Else
                            bolRet = roRequestApproval.DeleteRequestApprovals(Me.ID, Me.State)
                        End If

                        ' Actualizamos los dias solicitados
                        If bolRet Then
                            If Me.lstRequestDays IsNot Nothing AndAlso Me.lstRequestDays.Count > 0 Then
                                bolRet = roRequestDay.SaveRequestDays(Me.ID, Me.lstRequestDays, Me.oState)
                            Else
                                bolRet = roRequestDay.DeleteRequestDays(Me.ID, Me.State)
                            End If
                        End If

                        If bolRet AndAlso bIsNew Then PersistRequestPermissions(Me.ID)

                        oAuditDataNew = oRow

                        If bolRet And bAudit Then

                            bolRet = False

                            ' Auditamos
                            Dim strObjectName As String = ""
                            Try
                                Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(Me.IDEmployee, LoadType.Employee)
                                strObjectName = oPassport.Name
                            Catch
                            End Try

                            Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()

                            'Estado
                            Select Case iStatus
                                Case eRequestStatus.Accepted
                                    Extensions.roAudit.AddParameter(tbAuditParameters, "{Status}", Me.oState.Language.Translate("RequestInfo.RequestStatus.Accepted", "").ToUpper, "", 1)
                                Case eRequestStatus.Denied
                                    Extensions.roAudit.AddParameter(tbAuditParameters, "{Status}", Me.oState.Language.Translate("RequestInfo.RequestStatus.Denied", "").ToUpper, "", 1)
                                Case eRequestStatus.OnGoing
                                    Extensions.roAudit.AddParameter(tbAuditParameters, "{Status}", Me.oState.Language.Translate("RequestInfo.RequestStatus.OnGoing", "").ToUpper, "", 1)
                                Case eRequestStatus.Pending
                                    Extensions.roAudit.AddParameter(tbAuditParameters, "{Status}", Me.oState.Language.Translate("RequestInfo.RequestStatus.Pending", "").ToUpper, "", 1)
                                Case eRequestStatus.Canceled
                                    Extensions.roAudit.AddParameter(tbAuditParameters, "{Status}", strCommentToAudit, "", 1)
                                Case Else
                            End Select

                            Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                            Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                            bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tRequest, strObjectName, tbAuditParameters, -1)
                        End If

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Borra la solicitud (siempre y cuando se encuentre en estado pendiente)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False
            Dim strSQL As String = String.Empty

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.iStatus = eRequestStatus.Pending OrElse (Me.RequestType = eRequestType.ExchangeShiftBetweenEmployees AndAlso Me.RequestStatus = eRequestStatus.OnGoing) Then

                    ' Si es una solicitud de fichaje olvidado, verificamos que el fichaje asociado exista y se pueda borrar
                    If Me.RequestType = eRequestType.ForbiddenPunch Then
                        Me.UpdateForbiddenPunch(False)
                    ElseIf Me.RequestType = eRequestType.ForgottenCostCenterPunch Then
                        Me.UpdateForgottenCenterPunch(False)
                    ElseIf Me.RequestType = eRequestType.ExternalWorkWeekResume Then
                        Me.UpdateForbiddenExternalWorkWeekResume(False)
                    End If

                    If oState.Result = RequestResultEnum.NoError Then

                        'Borramos el histórico de aprobaciones de la solicitud
                        bolRet = roRequestApproval.DeleteRequestApprovals(Me.ID, Me.oState)
                        If bolRet Then
                            'Borramos los dias solicitados, si aplica
                            bolRet = roRequestDay.DeleteRequestDays(Me.ID, Me.oState)
                        End If

                        If bolRet Then
                            bolRet = False

                            'Borramos documentos si los tiene
                            strSQL = $"@DELETE# FROM Documents WHERE IdRequest = {Me.ID}"
                            If Not ExecuteSql(strSQL) Then
                                oState.Result = RequestResultEnum.ConnectionError
                            End If

                            'Borramos la solicitud
                            strSQL = $"@DELETE# FROM Requests WHERE ID = {Me.ID}"
                            If Not ExecuteSql(strSQL) Then
                                oState.Result = RequestResultEnum.ConnectionError
                            End If

                            'Borramos notificaciones
                            strSQL = $"@DELETE# sysroNotificationTasks WHERE ID IN (
                                       @SELECT# sysroNotificationTasks.ID 
                                       FROM sysroNotificationTasks
                                       INNER JOIN Notifications ON sysroNotificationTasks.IDNotification = Notifications.id
                                       WHERE Notifications.IDType IN ({CInt(eNotificationType.Request_Pending)}, {CInt(eNotificationType.Absence_Canceled_By_User)}) AND sysroNotificationTasks.Key1Numeric = {Me.ID})"
                            If Not ExecuteSql(strSQL) Then
                                oState.Result = RequestResultEnum.ConnectionError
                            End If

                            bolRet = (oState.Result = RequestResultEnum.NoError)

                            ' Si se borró la solicitud, miro si debo eliminar alertas asociadas
                            If bolRet Then
                                Try
                                    If Not Notifications.roNotification.DeleteNotificationTask(eNotificationType.Request_Pending, Nothing, Me.ID) Then
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roRequest::Save::Unable to delete notification task for request id = " & Me.ID.ToString)
                                    End If
                                Catch ex As Exception
                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roRequest::Save::Unable to delete notification task for request id = " & Me.ID.ToString, ex)
                                End Try
                            End If

                            If bolRet And bAudit Then
                                ' Auditamos
                                bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tRequest, "", Nothing, -1)
                            End If

                        End If
                    Else
                        bolRet = False
                    End If

                    If bolRet Then
                        Try
                            strSQL = "@DELETE# sysroNotificationTasks WHERE Key1Numeric = " & Me.IDEmployee & " AND Key5Numeric = " & Me.ID & " AND IDNotification = 701 AND Parameters LIKE 'REQUEST%'"
                            bolRet = ExecuteSql(strSQL)
                        Catch ex As Exception
                        End Try
                    End If
                Else
                    oState.Result = RequestResultEnum.NoDeleteBecauseNotPending
                    bolRet = False
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Lanza la notificación (pendiente de implementar)
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendNotify() As Boolean

            Dim bolRet As Boolean = True

            Try
                'TODO: Per al notificador...
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest::SendNotify")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::SendNotify")
                bolRet = False
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Aprueba o denega la solicitud.
        ''' </summary>
        ''' <param name="_IDPassport">Código del passport asociado al supervisor</param>
        ''' <param name="_ApproveRefuse">True - aprobar, False - denegar</param>
        ''' <param name="_CheckLockedDays">Sólo para solicitudes de tipo planificación: True- verifica si hay algún día bloqueado y devuelve un error si lo hay, False- no verifica si hay algún día bloqueado y planifica todo el periodo. </param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ApproveRefuse(ByVal _IDPassport As Integer, ByVal _ApproveRefuse As Boolean, ByVal _Comments As String, Optional ByVal _CheckLockedDays As Boolean = True, Optional ByVal _forceApprove As Boolean = False, Optional ByVal bGenerateNotification As Boolean = True, Optional ByVal bSimpleMessage As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim IdAbsence As Integer = 0

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String = String.Empty
                Dim bolPermission As Boolean = False
                Dim bLevelOfAuthority As Byte = 10
                Dim oSupervisorPermission As Permission = Permission.None
                Dim oApprovalStatus As roRequestApproval = Nothing
                Dim intApprovedAtLevel As Integer = 1

                ' Obtenemos la configuración de seguridad de la solicitud en función del tipo
                Dim oRequestTypeSecurity As New roRequestTypeSecurity(Me.iRequestType, Me.oState)

                Dim oUser As roPassportTicket = roPassportManager.GetPassportTicket(_IDPassport, LoadType.Passport)

                Dim bMaxTimeAsMinTime As Boolean = False
                bMaxTimeAsMinTime = (roTypes.Any2String(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "VTPortal.Requests.MaxTimeAsMinTime")) = "1")

                If Me.RequestType = eRequestType.ExchangeShiftBetweenEmployees AndAlso Me.IDEmployeeExchange.HasValue AndAlso oUser.IDEmployee.HasValue AndAlso Me.IDEmployeeExchange = oUser.IDEmployee AndAlso Me.RequestStatus = eRequestStatus.Pending Then
                    bolPermission = (Security.WLHelper.GetFeaturePermission(oUser.ID, oRequestTypeSecurity.EmployeeFeatureName, "E") >= Permission.Write)
                    ' Asignar el nivel menor que tiene permisos
                    If bolPermission Then
                        bLevelOfAuthority = 11
                        Dim strSQLLevel As String
                        Dim intLevel As Integer

                        strSQLLevel = $"@SELECT# TOP 1 NextLevelOfAuthorityRequired FROM sysrovwSecurity_PendingRequestsDependencies 
                                        WHERE IdRequest = @idrequest AND DirectDependence = 1 AND AutomaticValidation = 0 AND IsRoboticsUser = 0"
                        Dim parameters As New List(Of CommandParameter)
                        parameters.Add(New CommandParameter("@idrequest", CommandParameter.ParameterType.tInt, Me.ID))

                        strSQLLevel &= SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.ApproveRefuse)

                        intLevel = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQLLevel, parameters))

                        If intLevel > 0 Then
                            bLevelOfAuthority = (intLevel + 1)
                        End If
                    End If
                Else
                    ' Verificar que el usuario/passport tiene permisos de validación sobre la solicitud actual
                    Dim bHasPermissionOverRequest As Boolean = False
                    Dim table As DataTable = New DataTable
                    strSQL = $"@SELECT# SupervisorLevelOfAuthority, ApprovedAtLevel FROM sysrovwSecurity_PermissionOverRequests WHERE IdRequest = @idrequest AND IdPassport = @idpassport"
                    strSQL &= SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.ApproveRefuse)
                    Dim Command As DbCommand = AccessHelper.CreateCommand(strSQL)
                    Dim intSupervisorLevelOfAuthority As Integer = 15

                    AccessHelper.AddParameter(Command, "@idrequest", DbType.Int32).Value = Me.intID
                    AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32).Value = _IDPassport
                    Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                    Adapter.Fill(table)
                    If table IsNot Nothing AndAlso table.Rows.Count > 0 Then
                        bHasPermissionOverRequest = True
                        intSupervisorLevelOfAuthority = roTypes.Any2Integer(table.Rows(0)("SupervisorLevelOfAuthority"))
                        intApprovedAtLevel = roTypes.Any2Integer(table.Rows(0)("ApprovedAtLevel"))
                    End If

                    If bHasPermissionOverRequest Then

                        ' Obtenemos el permiso que tiene el passport del supervisor para gestionar este tipo de solicitud
                        oSupervisorPermission = Security.WLHelper.GetPermissionOverFeature(_IDPassport, oRequestTypeSecurity.SupervisorFeatureName, "U")

                        ' Valido que tenga permiso sobre la funcionalidad (en ocasiones esta función devuelve None por problema de acceso a datos ...)
                        If oSupervisorPermission > Permission.None Then
                            If intSupervisorLevelOfAuthority <> 15 Then
                                bLevelOfAuthority = intSupervisorLevelOfAuthority
                            End If

                            ' Verificamos que no la haya aprobado/denegado un nivel de mando superior
                            oApprovalStatus = Me.GetApprovalStatus()

                            ' El supervisor puede aprobar/denegar si se cumple cualquier de las condiciones :
                            ' - El estado de la solicitud es pendiente (nadie la ha aprobado/denegado)
                            ' - La solicitud está en curso y
                            '       - la ha denegado un supervisor con nivel de mando de rango inferior
                            '       - o la ha aprobado o denegado el mismo supervisor
                            If oApprovalStatus.Status = eRequestStatus.Pending Then
                                bolPermission = True
                            ElseIf oApprovalStatus.Status = eRequestStatus.OnGoing Then
                                ' Si la última acción ha sido aprobarla por mi mismo, la puedo modificar
                                bolPermission = (oApprovalStatus.StatusLevel > bLevelOfAuthority OrElse (oApprovalStatus.StatusLevel = bLevelOfAuthority AndAlso oApprovalStatus.IDPassport = _IDPassport))
                            ElseIf oApprovalStatus.Status = eRequestStatus.Accepted Then
                                ' La solicitud ya ha sido aprobada definitivamente. No se puede deshacer.
                                bolPermission = False
                            Else

                                Dim Validator As roPassportTicket = roPassportManager.GetPassportTicket(oApprovalStatus.IDPassport, LoadType.Passport)
                                If Validator IsNot Nothing AndAlso Validator.ID = roConstants.GetSystemUserId() Then
                                    ' Si el ultimo validador ha sido SYSTEM y esta denegada, quiere decir que se está intentando aprobar por un supervisor real, por lo que
                                    ' se debe modificar el tipo de validacion a normal en vez de automatica
                                    bolPermission = True
                                    Me.AutomaticValidation = False
                                Else
                                    ' Si la última acción ha sido denegarla por mi mismo, la puedo modificar
                                    bolPermission = (oApprovalStatus.StatusLevel = bLevelOfAuthority AndAlso oApprovalStatus.IDPassport = _IDPassport)
                                End If
                            End If
                        Else
                            Me.oState.Result = RequestResultEnum.SqlError
                            Return False
                        End If
                    End If
                End If

                If Not bolPermission Then
                    ' El passport del supervisor no tiene permisos suficientes para aprovar o denegar la solicitud
                    Me.oState.Result = RequestResultEnum.NoApprovePermissions

                    If oApprovalStatus IsNot Nothing AndAlso oApprovalStatus.StatusLevel > bLevelOfAuthority Then
                        ' El supervisor no puede modificar el estado de la solicitud ya que su nivel de mando no lo permite
                        Me.oState.Result = RequestResultEnum.NoApproveRefuseLevelOfAuthorityRequired
                    End If
                Else
                    ' Si se está aprobando y el supervisor es de nivel de mando 1 o tiene permisos de administración, se aprueba definitivamente la solicitud.
                    If _ApproveRefuse Then
                        ' Revisamos si debemos aplicar reglas de validacion de solicitudes, en caso necesario
                        ' Si el empleado ha indicado que quiere continuar, guardamos la solicitud, sin validar nada mas
                        If Not _forceApprove Then
                            ' Validamos las reglas de solicitudes
                            bolRet = roRequestRuleManager.RequestRulesValidate(Me, oState, True, bSimpleMessage)
                        Else
                            bolRet = True
                        End If

                        If Not (bLevelOfAuthority = 1 OrElse oSupervisorPermission = Permission.Admin OrElse intApprovedAtLevel = bLevelOfAuthority) Then
                            ' En el caso que no sea un usuario Administrador o de Nivel 1, (o del mismo nivel que el mas bajo para aprobar v3)
                            ' debemos revisar si el Supervisor actual es el último que puede validar dicha solicitud,
                            ' en ese caso hay que tratarlo como si fuera Admin o de nivel 1 y que se apruebe definitivamente la solicitud

                            ' Obtenemos todos los supervisores con nivel de supervision mas bajo que el actual
                            ' que tengan permisos sobre la solicitud

                            strSQL = $"@SELECT# COUNT(*) FROM sysrovwSecurity_PermissionOverRequests 
                                       WHERE IdRequest = @idrequest and SupervisorLevelOfAuthority < @levelofauth AND Permission > 3 AND IsRoboticsUser = 0"
                            Dim parameters As New List(Of CommandParameter)
                            parameters.Add(New CommandParameter("@idrequest", CommandParameter.ParameterType.tInt, Me.intID))
                            parameters.Add(New CommandParameter("@levelofauth", CommandParameter.ParameterType.tInt, bLevelOfAuthority))

                            strSQL &= SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.ApproveRefuse)

                            Dim iTotal As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar(strSQL, parameters))

                            If iTotal = 0 Then
                                ' Si no existe ningún Supervisor que pueda posteriormente seguir validando la solicitud,
                                ' la aprobamos definitivamente en este momento
                                oSupervisorPermission = Permission.Admin
                            End If
                        End If

                        If (bLevelOfAuthority = 1 OrElse oSupervisorPermission = Permission.Admin OrElse intApprovedAtLevel = bLevelOfAuthority) Then
                            If bolRet Then
                                Dim oEmployeeState As New Employee.roEmployeeState(oState.IDPassport)

                                Select Case Me.iRequestType
                                    Case eRequestType.UserFieldsChange ' Solicitud modificación campo ficha empleado

                                        Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                                        roBusinessState.CopyTo(Me.oState, oUserFieldState)
                                        Dim oUserField As New VTUserFields.UserFields.roUserField(oUserFieldState, Me.strFieldName, Types.EmployeeField, False)
                                        If oUserFieldState.Result = UserFieldResultEnum.NoError Then

                                            ' Verifica que el campo de la ficha sea visible para solicitudes
                                            If oUserField.RequestVisible(Me.intIDEmployee) Then

                                                ' Guardar el valor del campo de la ficha
                                                Dim xUserFieldDate As Date = Now.Date

                                                If Not oUserField.History Then
                                                    xUserFieldDate = New Date(1900, 1, 1)
                                                Else
                                                    If Me.dDate1.HasValue Then
                                                        ' Si se ha informado fecha valor
                                                        xUserFieldDate = Me.dDate1.Value
                                                    End If
                                                End If

                                                Dim employeeUserfieldState As roUserFieldState = New VTUserFields.UserFields.roUserFieldState(oState.IDPassport)
                                                Dim oEmployeeUserField As New VTUserFields.UserFields.roEmployeeUserField(Me.intIDEmployee, Me.strFieldName, xUserFieldDate.Date, employeeUserfieldState)
                                                oEmployeeUserField.FieldValue = Me.strFieldValue
                                                bolRet = oEmployeeUserField.Save()
                                                If Not bolRet Then
                                                    roBusinessState.CopyTo(oEmployeeState, Me.oState)
                                                    Me.oState.Result = RequestResultEnum.UserFieldValueSaveError

                                                    If employeeUserfieldState.Result = UserFieldResultEnum.UniqueValueAlreadyExists Then
                                                        Me.oState.ErrorText &= ". " & employeeUserfieldState.ErrorText
                                                    Else
                                                        Me.oState.ErrorText &= ". " & oEmployeeState.ErrorText
                                                    End If

                                                End If
                                            Else
                                                Me.oState.Result = RequestResultEnum.UserFieldNoRequestVisible
                                                bolRet = False
                                            End If
                                        Else
                                            ' No se ha podido obtener la definición del campo de la ficha
                                            roBusinessState.CopyTo(oUserFieldState, Me.oState)
                                            Me.oState.Result = RequestResultEnum.UserFieldValueSaveError
                                            Me.oState.ErrorText &= " " & oUserFieldState.ErrorText
                                            bolRet = False
                                        End If

                                    Case eRequestType.ExternalWorkWeekResume ' Solicitud de resumen de trabajo semanal

                                        If Not Me.UpdateForbiddenExternalWorkWeekResume(_ApproveRefuse) Then
                                            Select Case Me.oState.Result
                                                Case RequestResultEnum.RequestMoveTooMany
                                                    ' Si existen varios movimientos relacionados devolvemos error y marcamos la solicitud cómo denegada
                                                    _ApproveRefuse = False 'Forzamos denegar solicitud
                                                    _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                    Me.oState.Language.Translate("ResultEnum.RequestMoveTooMany", "") & ")"
                                                    bolRet = True

                                                Case RequestResultEnum.RequestMoveNotExist
                                                    ' Si no existe devolvemos error y marcamos la solicitud cómo denegada
                                                    _ApproveRefuse = False 'Forzamos denegar solicitud
                                                    _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                    Me.oState.Language.Translate("ResultEnum.RequestMoveNotExist", "") & ")"
                                                    bolRet = True

                                                Case Else
                                                    bolRet = False

                                            End Select
                                        Else
                                            bolRet = True
                                        End If

                                        If bolRet Then
                                            roConnector.InitTask(TasksType.MOVES)
                                        End If

                                    Case eRequestType.ForbiddenPunch ' Solicitud de fichaje olvidado
                                        If Not _ApproveRefuse Then
                                            Dim xFreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee, False, oState)
                                            If Any2Time(Me.Date1).DateOnly <= xFreezeDate Then
                                                oState.Result = RequestResultEnum.FreezeDateException
                                                bolRet = False
                                            End If
                                        End If

                                        If bolRet Then
                                            If Not Me.UpdateForbiddenPunch(_ApproveRefuse) Then
                                                Select Case Me.oState.Result
                                                    Case RequestResultEnum.RequestMoveTooMany
                                                        ' Si existen varios movimientos relacionados devolvemos error y marcamos la solicitud cómo denegada
                                                        _ApproveRefuse = False 'Forzamos denegar solicitud
                                                        _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                    Me.oState.Language.Translate("ResultEnum.RequestMoveTooMany", "") & ")"
                                                        bolRet = True

                                                    Case RequestResultEnum.RequestMoveNotExist
                                                        ' Si no existe devolvemos error y marcamos la solicitud cómo denegada
                                                        _ApproveRefuse = False 'Forzamos denegar solicitud
                                                        _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                    Me.oState.Language.Translate("ResultEnum.RequestMoveNotExist", "") & ")"
                                                        bolRet = True

                                                    Case Else
                                                        bolRet = False

                                                End Select
                                            Else
                                                bolRet = True
                                            End If
                                        End If

                                    Case eRequestType.DailyRecord
                                        ' Marco fichajes del parte como fiables si se aceptó la declaración, o los borro si se debegó
                                        If bolRet Then
                                            If Not Me.UpdateDailyRecordPunches(_ApproveRefuse) Then
                                                Select Case Me.oState.Result
                                                    Case RequestResultEnum.NoPunchesForDailyRecord
                                                        ' Si no existen fichajes (no debería pasar), deniego
                                                        _ApproveRefuse = False
                                                        _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") & Me.oState.Language.Translate("ResultEnum.DailyReccordWithoutPunches", "") & ")"
                                                        bolRet = True
                                                    Case Else
                                                        bolRet = False
                                                End Select
                                            Else
                                                bolRet = True
                                            End If
                                        End If

                                    Case eRequestType.JustifyPunch ' Justificación marcaje existente

                                        ' Buscamos el fichaje asociado a la solicitud
                                        Dim oPunchState As New Punch.roPunchState
                                        roBusinessState.CopyTo(Me.oState, oPunchState)

                                        Dim strFilter As String = "IDEmployee = " & Me.IDEmployee.ToString & " AND " &
                                                              "(DateTime = " & Any2Time(Me.Date1).SQLDateTime & " AND " &
                                                              "ActualType IN(" & PunchTypeEnum._IN & "," & PunchTypeEnum._OUT & "))"
                                        Dim tbPunches As DataTable = Punch.roPunch.GetPunches(strFilter, oPunchState)

                                        If tbPunches IsNot Nothing Then
                                            If tbPunches.Rows.Count = 1 Then
                                                ' Marcar el fichaje con la justificación solicitada
                                                If Not IsDBNull(tbPunches.Rows(0)("DateTime")) AndAlso CDate(tbPunches.Rows(0)("DateTime")) = Me.Date1 Then
                                                    tbPunches.Rows(0)("TypeData") = Me.IDCause
                                                End If
                                                Dim oPunches As New Punch.roPunchList(oPunchState)
                                                bolRet = oPunches.Save(tbPunches, Nothing, False)
                                                If Not bolRet Then
                                                    roBusinessState.CopyTo(oPunchState, Me.oState)
                                                    Me.oState.Result = RequestResultEnum.JustifyPunchError
                                                    Me.oState.ErrorText &= " " & oPunchState.ErrorText
                                                End If
                                            ElseIf tbPunches.Rows.Count > 0 Then
                                                ' Si existen varios fichajes relacionados devolvemos error y marcamos la solicitud cómo denegada
                                                _ApproveRefuse = False 'Forzamos denegar solicitud
                                                Me.oState.Result = RequestResultEnum.RequestMoveTooMany
                                                _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                Me.oState.Language.Translate("ResultEnum.RequestMoveTooMany", "") & ")"
                                                bolRet = True
                                            Else
                                                ' Si no existe devolvemos error y marcamos la solicitud cómo denegada
                                                _ApproveRefuse = False 'Forzamos denegar solicitud
                                                Me.oState.Result = RequestResultEnum.RequestMoveNotExist
                                                _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                Me.oState.Language.Translate("ResultEnum.RequestMoveNotExist", "") & ")"
                                                bolRet = True
                                            End If
                                        Else
                                            roBusinessState.CopyTo(oPunchState, Me.oState)
                                            Me.oState.Result = RequestResultEnum.JustifyPunchError
                                            Me.oState.ErrorText &= " " & oPunchState.ErrorText
                                            bolRet = False
                                        End If

                                    Case eRequestType.ExternalWorkResumePart ' Parte de trabajo externo

                                        Dim oCauseState As New Cause.roCauseState
                                        roBusinessState.CopyTo(Me.oState, oCauseState)

                                        Dim oBusinessCenterState As New BusinessCenter.roBusinessCenterState(-1)
                                        Dim intIDefaultCenter = BusinessCenter.roBusinessCenter.GetEmployeeDefaultBusinessCenter(oBusinessCenterState, intIDEmployee, Me.Date1)

                                        Dim idCause As Integer = 1

                                        If Me.IDCause.HasValue Then
                                            idCause = Me.IDCause
                                        End If

                                        Dim oDailyCause As New Cause.roDailyCause(Me.IDEmployee, Me.Date1, 0, idCause, 0, intIDefaultCenter, 0, 0, oCauseState)
                                        If oCauseState.Result = CauseResultEnum.NoError Then

                                            oDailyCause.Value = Me.Hours
                                            oDailyCause.Manual = True
                                            oDailyCause.IsNotReliable = True

                                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) Set Status=65, [GUID] = '' WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND Date=" & Any2Time(Me.Date1).SQLSmallDateTime
                                            bolRet = ExecuteSql(strSQL)
                                            If Not bolRet Then
                                                oState.Result = RequestResultEnum.ConnectionError
                                            End If

                                            If bolRet Then
                                                bolRet = oDailyCause.Save(True, True)
                                                If Not bolRet Then
                                                    roBusinessState.CopyTo(oCauseState, Me.oState)
                                                    Me.oState.Result = RequestResultEnum.ExternalWorkResumePartError
                                                    Me.oState.ErrorText &= " " & oCauseState.ErrorText
                                                End If
                                            End If
                                        Else
                                            roBusinessState.CopyTo(oCauseState, Me.oState)
                                            Me.oState.Result = RequestResultEnum.ExternalWorkResumePartError
                                            Me.oState.ErrorText &= " " & oCauseState.ErrorText
                                            bolRet = False
                                        End If

                                    Case eRequestType.ChangeShift ' Cambio de horario
                                        Dim strLockedDays As String = ""
                                        If _CheckLockedDays Then
                                            Dim tbPlan As DataTable = VTBusiness.Scheduler.roScheduler.GetPlan(Me.IDEmployee, Me.Date1, Me.Date1, oEmployeeState)
                                            If tbPlan IsNot Nothing Then
                                                Dim oRows As DataRow() = tbPlan.Select("LockedDay = 1")
                                                bolRet = Not oRows.Any()
                                                For Each oRow As DataRow In oRows
                                                    strLockedDays &= "," & oRow("Date")
                                                Next
                                                If strLockedDays <> "" Then strLockedDays = strLockedDays.Substring(1)
                                            Else
                                                roBusinessState.CopyTo(oEmployeeState, Me.oState)
                                                Me.oState.Result = RequestResultEnum.ChangeShiftError
                                                Me.oState.ErrorText &= " " & oEmployeeState.ErrorText
                                                bolRet = False
                                            End If
                                        Else
                                            bolRet = True
                                        End If
                                        If bolRet Then
                                            Dim xDate As Date = Me.Date1.Value.Date
                                            Dim _StartShift As DateTime = Nothing
                                            If Me.StartShift.HasValue Then _StartShift = Me.StartShift.Value
                                            While bolRet And xDate <= Me.Date2.Value.Date
                                                Dim bReplaceShift As Boolean = True

                                                'Si nos piden remplazar un horario en concreto debemos buscar el que tiene asignado el dia en concreto
                                                If Me.Field4 > 0 Then
                                                    Dim dtActualShift As DataTable = VTBusiness.Scheduler.roScheduler.GetPlan(Me.IDEmployee, xDate, xDate, oEmployeeState)
                                                    If dtActualShift IsNot Nothing AndAlso dtActualShift.Rows.Count = 1 Then
                                                        Dim actualShiftID As Integer = dtActualShift.Rows(0)("IDShift1")
                                                        If dtActualShift.Rows(0)("IDShiftUsed") IsNot DBNull.Value Then
                                                            actualShiftID = dtActualShift.Rows(0)("IDShiftUsed")
                                                        End If

                                                        If Shift.roShift.IsHolidays(actualShiftID) Then
                                                            If dtActualShift.Rows(0)("IDShiftBase") IsNot DBNull.Value Then
                                                                actualShiftID = dtActualShift.Rows(0)("IDShiftBase")
                                                            Else
                                                                actualShiftID = -1
                                                            End If
                                                        End If

                                                        If actualShiftID <> roTypes.Any2Integer(Me.Field4) Then
                                                            bReplaceShift = False
                                                        End If

                                                    End If
                                                End If

                                                If bReplaceShift Then bolRet = VTBusiness.Scheduler.roScheduler.AssignShift(Me.IDEmployee, xDate, Me.IDShift, 0, 0, 0, _StartShift, Nothing, Nothing, Nothing, 0, LockedDayAction.NoReplaceAll, LockedDayAction.NoReplaceAll, oEmployeeState, , , True, ,,,,, "Calendar.Requests.ShiftChange")
                                                xDate = xDate.AddDays(1)
                                            End While
                                            If Not bolRet Then
                                                roBusinessState.CopyTo(oEmployeeState, Me.oState)
                                                Me.oState.Result = RequestResultEnum.ChangeShiftError
                                                Me.oState.ErrorText &= " " & oEmployeeState.ErrorText
                                            End If
                                        ElseIf strLockedDays <> "" Then ' No se puede planificar el día porqué está bloqueado.
                                            Me.oState.Result = RequestResultEnum.ExistsLockedDaysInPeriod
                                            Me.oState.ErrorText = Me.oState.ErrorText.Replace("{0}", strLockedDays)
                                        End If

                                    Case eRequestType.VacationsOrPermissions ' Solicitud de vacaciones o permisos

                                        Dim strLockedDays As String = ""
                                        If _CheckLockedDays Then
                                            For Each oRequestDay As roRequestDay In Me.RequestDays
                                                Dim tbPlan As DataTable = VTBusiness.Scheduler.roScheduler.GetPlan(Me.IDEmployee, oRequestDay.RequestDate, oRequestDay.RequestDate, oEmployeeState)
                                                If tbPlan IsNot Nothing Then
                                                    Dim oRows As DataRow() = tbPlan.Select("LockedDay = 1")
                                                    bolRet = Not oRows.Any()
                                                    For Each oRow As DataRow In oRows
                                                        strLockedDays &= "," & oRow("Date")
                                                    Next
                                                    If strLockedDays <> "" Then strLockedDays = strLockedDays.Substring(1)
                                                    If Not bolRet Then
                                                        Exit For
                                                    End If
                                                Else
                                                    roBusinessState.CopyTo(oEmployeeState, Me.oState)
                                                    Me.oState.Result = RequestResultEnum.ChangeShiftError
                                                    Me.oState.ErrorText &= " " & oEmployeeState.ErrorText
                                                    bolRet = False
                                                End If
                                            Next
                                        Else
                                            bolRet = True
                                        End If

                                        If bolRet Then
                                            ' Verificamos en caso necesario si se pasa del saldo
                                            ' de vacaciones en el caso que aprobara la solicitud
                                            Dim oValue As Boolean = False

                                            oValue = (New AdvancedParameter.roAdvancedParameter("ConfirmHolidayOverFlow", New AdvancedParameter.roAdvancedParameterState()).Value.Trim = 1)
                                            If Not oValue Then
                                                Const parameterName = "CheckHolidaysRquest"
                                                Dim advancedParameter As New AdvancedParameter.roAdvancedParameter("CheckHolidaysRquest", New AdvancedParameter.roAdvancedParameterState())
                                                If Not advancedParameter.Exists Then
                                                    If Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", "Running", "False") <> Nothing Then
                                                        advancedParameter.Value = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", parameterName, "False")
                                                    Else
                                                        advancedParameter.Value = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime\Server", parameterName, "False")
                                                    End If
                                                    advancedParameter.Save()
                                                End If
                                                oValue = Any2Boolean(advancedParameter.Value)
                                            End If

                                            If Not _forceApprove AndAlso oValue Then
                                                Dim intDone As Double = 0
                                                Dim intPending As Double = 0
                                                Dim intLasting As Double = 0
                                                Dim intDisponible As Double = 0
                                                Dim intExpired As Double = 0
                                                Dim intWithoutEnjoyment As Double = 0
                                                roBusinessSupport.VacationsResumeQuery(Me.IDEmployee, Me.IDShift, Now.Date, Nothing, Nothing, Me.Date1, intDone, intPending, intLasting, intDisponible, oState, intExpired, intWithoutEnjoyment)
                                                If intDisponible - intLasting - intPending - intExpired - intWithoutEnjoyment < 0 Then
                                                    bolRet = False
                                                    Me.oState.Result = RequestResultEnum.NotEnoughConceptBalance
                                                End If
                                            End If

                                            If bolRet Then

                                                Dim lstShifts As New Generic.List(Of String)
                                                If Not Me.StartShift.HasValue Then
                                                    lstShifts.Add(Me.IDShift.ToString & "****-2*0*-2")
                                                Else
                                                    lstShifts.Add(Me.IDShift.ToString & "~" & Format(Me.StartShift.Value, "yyyyMMddHHmm") & "****-2*0*-2")
                                                End If

                                                For Each oRequestDay As roRequestDay In Me.RequestDays
                                                    Dim xDateLocked As Date
                                                    bolRet = VTBusiness.Scheduler.roScheduler.CopyPlan(lstShifts, Me.IDEmployee, oRequestDay.RequestDate, oRequestDay.RequestDate, ActionShiftType.AllShift, LockedDayAction.NoReplaceAll, LockedDayAction.NoReplaceAll, xDateLocked, True, oEmployeeState, True,, "Calendar.Requests.Vacations")
                                                    If (Not bolRet) Or (oEmployeeState.Result = EmployeeResultEnum.InPeriodOfFreezing) Then
                                                        bolRet = False
                                                        roBusinessState.CopyTo(oEmployeeState, Me.oState)
                                                        Me.oState.Result = RequestResultEnum.VacationsOrPermissionsError
                                                        Me.oState.ErrorText &= " " & oEmployeeState.ErrorText
                                                        Exit For
                                                    End If
                                                Next

                                            End If
                                        Else ' No se puede planificar el día porqué está bloqueado.
                                            Me.oState.Result = RequestResultEnum.ExistsLockedDaysInPeriod
                                            Me.oState.ErrorText = Me.oState.ErrorText.Replace("{0}", strLockedDays)
                                        End If

                                    Case eRequestType.CancelHolidays ' Solicitud de cancelacion de vacaciones o permisos
                                        Dim xDate As Date = Me.Date1.Value.Date
                                        Dim _StartShift As DateTime = Nothing
                                        If Me.StartShift.HasValue Then _StartShift = Me.StartShift.Value

                                        Dim oProgrammedHolidayState As New roProgrammedHolidayState
                                        roBusinessState.CopyTo(Me.oState, oProgrammedHolidayState)
                                        Dim oProgrammedManager As New roProgrammedHolidayManager

                                        bolRet = True
                                        For Each oRequestDay As roRequestDay In Me.RequestDays

                                            Dim bolApplyShift As Boolean = True
                                            ' Si esta indicado una justificacion, aplicamos sobre vacaciones por horas
                                            If Me.IDCause.HasValue AndAlso Me.IDCause.Value > 0 Then
                                                bolApplyShift = False
                                            End If

                                            If bolApplyShift Then
                                                'Cancelamos vacaciones por dias
                                                Dim tbPlan As DataTable = VTBusiness.Scheduler.roScheduler.GetPlan(Me.IDEmployee, oRequestDay.RequestDate, oRequestDay.RequestDate, oEmployeeState)
                                                If tbPlan IsNot Nothing Then
                                                    Dim strFillterShift As String = "IsHolidays = 1"
                                                    If Me.IDShift.HasValue AndAlso Me.IDShift.Value > 0 Then
                                                        strFillterShift += " AND IDShift1=" & Me.IDShift.ToString
                                                    End If

                                                    Dim oRows As DataRow() = tbPlan.Select(strFillterShift)
                                                    If oRows IsNot Nothing AndAlso oRows.Count = 1 Then
                                                        bolRet = VTBusiness.Scheduler.roScheduler.AssignShift(Me.IDEmployee, oRequestDay.RequestDate, -1, 0, 0, 0, _StartShift, Nothing, Nothing, Nothing, 0, LockedDayAction.NoReplaceAll, LockedDayAction.NoReplaceAll, oEmployeeState, , , True, , , , , , "Calendar.Requests.Vacations")
                                                        If Not bolRet Then Exit For

                                                    End If
                                                End If
                                            Else
                                                ' Cancelamos previsiones de vacaciones por horas
                                                Dim oProgrammedHoliday As New DTOs.roProgrammedHoliday()
                                                oProgrammedHoliday.IDEmployee = Me.IDEmployee
                                                oProgrammedHoliday.ProgrammedDate = oRequestDay.RequestDate
                                                Dim oProgrammedHolidays As List(Of DTOs.roProgrammedHoliday) = oProgrammedManager.GetProgrammedHolidays(Me.IDEmployee, oProgrammedHolidayState, "Date=" & Any2Time(oRequestDay.RequestDate).SQLSmallDateTime & " AND IDCause=" & Me.IDCause.Value)
                                                For Each olstProgrammedHoliday As DTOs.roProgrammedHoliday In oProgrammedHolidays
                                                    bolRet = oProgrammedManager.DeleteProgrammedHoliday(olstProgrammedHoliday)
                                                    If bolRet Then
                                                        bolRet = oProgrammedManager.RegisterDeleteProgrammedHoliday(Me.IDEmployee, olstProgrammedHoliday.ID, oRequestDay.RequestDate)
                                                    End If
                                                    If Not bolRet Then Exit For
                                                Next
                                            End If
                                        Next

                                        If Not bolRet Then
                                            roBusinessState.CopyTo(oEmployeeState, Me.oState)
                                            Me.oState.Result = RequestResultEnum.ChangeShiftError
                                            Me.oState.ErrorText &= " " & oEmployeeState.ErrorText
                                        End If

                                    Case eRequestType.PlannedAbsences ' Ausencias previstas

                                        Dim oAbsenceState As New Absence.roProgrammedAbsenceState
                                        roBusinessState.CopyTo(Me.oState, oAbsenceState)
                                        Dim oAbsence As New Absence.roProgrammedAbsence(Me.IDEmployee, Me.Date1, oAbsenceState)
                                        If oAbsence.Load(False) Then
                                            oAbsence.RequestId = Me.ID
                                            oAbsence.FinishDate = Me.Date2
                                            oAbsence.IDCause = Me.IDCause
                                            oAbsence.Description = Comments
                                            bolRet = oAbsence.Save()
                                            If Not bolRet Then
                                                roBusinessState.CopyTo(oAbsenceState, Me.oState)
                                                Me.oState.Result = RequestResultEnum.PlannedAbsencesError
                                                Me.oState.ErrorText &= " " & oAbsenceState.ErrorText
                                            Else
                                                IdAbsence = oAbsence.IdAbsence
                                            End If
                                        Else
                                            Me.oState.Result = RequestResultEnum.PlannedAbsencesError
                                            bolRet = False
                                        End If
                                    Case eRequestType.Telecommute

                                        If Me.Field1 = "0" Then
                                            roCalendarManager.ChangeTelecommuting(IDEmployee, Me.Date1, TelecommutingTypeEnum._AtOffice, False)
                                        Else
                                            roCalendarManager.ChangeTelecommuting(IDEmployee, Me.Date1, TelecommutingTypeEnum._AtHome, False)
                                        End If

                                    Case eRequestType.PlannedCauses  ' Incidencias previstas

                                        Dim oAbsenceState As New Incidence.roProgrammedCauseState
                                        roBusinessState.CopyTo(Me.oState, oAbsenceState)
                                        Dim oAbsence As New Incidence.roProgrammedCause(Me.IDEmployee, Me.Date1, oAbsenceState)
                                        oAbsence.RequestId = Me.ID
                                        'Dim oAbsence As New Incidence.roProgrammedCause
                                        oAbsence.ProgrammedEndDate = Me.Date2
                                        oAbsence.MinDuration = 0
                                        If bMaxTimeAsMinTime Then oAbsence.MinDuration = Me.Hours
                                        oAbsence.Duration = Me.Hours

                                        Dim oParam As New AdvancedParameter.roAdvancedParameter("VTLive.ProgrammedCauses.MinDurationEnabled", New AdvancedParameter.roAdvancedParameterState(Me.oState.IDPassport))
                                        If Not roTypes.Any2Boolean(oParam.Value) Then
                                            oAbsence.MinDuration = oAbsence.Duration
                                        End If

                                        oAbsence.IDCause = Me.IDCause
                                        oAbsence.BeginTime = Me.FromTime
                                        oAbsence.EndTime = Me.ToTime
                                        oAbsence.Description = Me.Comments
                                        bolRet = oAbsence.Save()
                                        If Not bolRet Then
                                            roBusinessState.CopyTo(oAbsenceState, Me.oState)
                                            Me.oState.Result = RequestResultEnum.PlannedCausesError
                                            Me.oState.ErrorText &= " " & oAbsence.State.ErrorText
                                        Else
                                            IdAbsence = oAbsence.AbsenceID
                                        End If

                                    Case eRequestType.PlannedOvertimes ' Prevision horas de exceso
                                        Dim oProgrammedOvertimeState As New roProgrammedOvertimeState
                                        roBusinessState.CopyTo(Me.oState, oProgrammedOvertimeState)
                                        Dim oProgrammedManager As New roProgrammedOvertimeManager
                                        Dim oProgrammedOvertime As New DTOs.roProgrammedOvertime

                                        oProgrammedOvertime.RequestId = Me.ID
                                        oProgrammedOvertime.IDEmployee = Me.IDEmployee
                                        oProgrammedOvertime.ProgrammedBeginDate = Me.Date1
                                        oProgrammedOvertime.ProgrammedEndDate = Me.Date2
                                        oProgrammedOvertime.MinDuration = 0
                                        If bMaxTimeAsMinTime Then oProgrammedOvertime.MinDuration = Me.Hours
                                        oProgrammedOvertime.Duration = Me.Hours
                                        oProgrammedOvertime.IDCause = Me.IDCause
                                        oProgrammedOvertime.BeginTime = Me.FromTime
                                        oProgrammedOvertime.EndTime = Me.ToTime
                                        oProgrammedOvertime.Description = Me.Comments
                                        bolRet = oProgrammedManager.SaveProgrammedOvertime(oProgrammedOvertime)
                                        If Not bolRet Then
                                            roBusinessState.CopyTo(oProgrammedManager.State, Me.oState)
                                            Me.oState.Result = RequestResultEnum.PlannedOvertimesError
                                            Me.oState.ErrorText &= " " & oProgrammedManager.State.ErrorText
                                        Else
                                            IdAbsence = oProgrammedOvertime.ID
                                        End If

                                    Case eRequestType.PlannedHolidays   ' Previsiones de vacaciones por horas

                                        Dim oProgrammedHolidayState As New roProgrammedHolidayState
                                        roBusinessState.CopyTo(Me.oState, oProgrammedHolidayState)
                                        Dim oProgrammedManager As New roProgrammedHolidayManager

                                        For Each oRequestDay As roRequestDay In Me.RequestDays
                                            ' Para cada dia solicituado generamos una prevision de vacaciones por horas
                                            Dim oProgrammedHoliday As New DTOs.roProgrammedHoliday()
                                            oProgrammedHoliday.IDEmployee = Me.IDEmployee
                                            oProgrammedHoliday.ProgrammedDate = oRequestDay.RequestDate
                                            oProgrammedHoliday.IDCause = Me.IDCause
                                            oProgrammedHoliday.AllDay = oRequestDay.AllDay
                                            oProgrammedHoliday.Description = Me.Comments
                                            If Not oProgrammedHoliday.AllDay Then
                                                oProgrammedHoliday.BeginTime = oRequestDay.FromTime
                                                oProgrammedHoliday.EndTime = oRequestDay.ToTime
                                                oProgrammedHoliday.Duration = Any2Time(oProgrammedHoliday.EndTime).NumericValue - Any2Time(oProgrammedHoliday.BeginTime).NumericValue
                                            End If
                                            bolRet = oProgrammedManager.SaveProgrammedHoliday(oProgrammedHoliday)
                                            If Not bolRet Then
                                                roBusinessState.CopyTo(oProgrammedManager.State, Me.oState)
                                                Me.oState.Result = RequestResultEnum.PlannedHolidaysError
                                                Me.oState.ErrorText &= " " & oProgrammedManager.State.ErrorText
                                                Exit For
                                            End If
                                        Next

                                    Case eRequestType.ForbiddenTaskPunch ' Solicitud de fichaje olvidado de tarea
                                        ' Verificamos que el fichaje no sea anterior a la fecha de cierre
                                        Dim xFreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee, False, oState)
                                        If Any2Time(Me.Date1).DateOnly <= xFreezeDate Then
                                            oState.Result = RequestResultEnum.FreezeDateException
                                            bolRet = False
                                        Else
                                            If Not Me.UpdateForbiddenTaskPunch(_ApproveRefuse) Then
                                                bolRet = False
                                                Me.oState.Result = RequestResultEnum.ForbiddenPunchError
                                            Else
                                                bolRet = True
                                            End If
                                        End If

                                    Case eRequestType.ForgottenCostCenterPunch
                                        If Not _ApproveRefuse Then
                                            Dim xFreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee, False, oState)
                                            If Any2Time(Me.Date1).DateOnly <= xFreezeDate Then
                                                oState.Result = RequestResultEnum.FreezeDateException
                                                bolRet = False
                                            End If
                                        End If
                                        If bolRet Then
                                            If Not Me.UpdateForgottenCenterPunch(_ApproveRefuse) Then
                                                Select Case Me.oState.Result
                                                    Case RequestResultEnum.RequestMoveTooMany
                                                        ' Si existen varios movimientos relacionados devolvemos error y marcamos la solicitud cómo denegada
                                                        _ApproveRefuse = False 'Forzamos denegar solicitud
                                                        _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                    Me.oState.Language.Translate("ResultEnum.RequestMoveTooMany", "") & ")"
                                                        bolRet = True

                                                    Case RequestResultEnum.RequestMoveNotExist
                                                        ' Si no existe devolvemos error y marcamos la solicitud cómo denegada
                                                        _ApproveRefuse = False 'Forzamos denegar solicitud
                                                        _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                    Me.oState.Language.Translate("ResultEnum.RequestMoveNotExist", "") & ")"
                                                        bolRet = True

                                                    Case Else
                                                        bolRet = False

                                                End Select
                                            Else
                                                bolRet = True
                                            End If
                                        End If
                                    Case eRequestType.ExchangeShiftBetweenEmployees ' Intercambio de horarios entre empleados
                                        bolRet = ApproveExchangeShiftBetweenEmployees()
                                End Select
                            End If
                        Else
                            bolRet = True
                        End If
                    Else
                        ' Si se está denegando la solicitud hacemos en función del tipo
                        Select Case Me.iRequestType
                            Case eRequestType.ExternalWorkWeekResume ' Solicitud de resumen de trabajo semanal

                                If Not Me.UpdateForbiddenExternalWorkWeekResume(_ApproveRefuse) Then
                                    Select Case Me.oState.Result
                                        Case RequestResultEnum.RequestMoveTooMany
                                            ' Si existen varios movimientos relacionados devolvemos error y marcamos la solicitud cómo denegada
                                            _ApproveRefuse = False 'Forzamos denegar solicitud
                                            _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                Me.oState.Language.Translate("ResultEnum.RequestMoveTooMany", "") & ")"
                                            bolRet = True

                                        Case RequestResultEnum.RequestMoveNotExist
                                            ' Si no existe devolvemos error y marcamos la solicitud cómo denegada
                                            _ApproveRefuse = False 'Forzamos denegar solicitud
                                            _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                Me.oState.Language.Translate("ResultEnum.RequestMoveNotExist", "") & ")"
                                            bolRet = True

                                        Case Else
                                            bolRet = False

                                    End Select
                                Else
                                    bolRet = True
                                End If

                                If bolRet Then
                                    roConnector.InitTask(TasksType.MOVES)
                                End If
                            Case eRequestType.ForbiddenPunch ' Solicitud de fichaje olvidado

                                If Not Me.UpdateForbiddenPunch(_ApproveRefuse) Then
                                    Select Case Me.oState.Result
                                        Case RequestResultEnum.RequestMoveTooMany
                                            ' Si existen varios movimientos relacionados devolvemos error y marcamos la solicitud cómo denegada
                                            _ApproveRefuse = False 'Forzamos denegar solicitud
                                            _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                Me.oState.Language.Translate("ResultEnum.RequestMoveTooMany", "") & ")"
                                            bolRet = True

                                        Case RequestResultEnum.RequestMoveNotExist
                                            ' Si no existe devolvemos error y marcamos la solicitud cómo denegada
                                            _ApproveRefuse = False 'Forzamos denegar solicitud
                                            _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                Me.oState.Language.Translate("ResultEnum.RequestMoveNotExist", "") & ")"
                                            bolRet = True

                                        Case Else
                                            bolRet = False

                                    End Select
                                Else
                                    bolRet = True
                                End If
                            Case eRequestType.DailyRecord
                                ' Si se denegó la solicitud, los fichajes que voy a borrar no están en fecha congelada
                                Dim xFreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee, False, oState)
                                bolRet = True
                                If Any2Time(Me.Date1).DateOnly <= xFreezeDate Then
                                    oState.Result = RequestResultEnum.FreezeDateException
                                    bolRet = False
                                End If

                                ' La declaración se denegó. Borro fichajes
                                If bolRet Then
                                    If Not Me.UpdateDailyRecordPunches(_ApproveRefuse) Then
                                        Select Case Me.oState.Result
                                            Case RequestResultEnum.NoPunchesForDailyRecord
                                                ' Si no existe devolvemos error y marcamos la solicitud cómo denegada
                                                _ApproveRefuse = False 'Forzamos denegar solicitud
                                                _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                            Me.oState.Language.Translate("ResultEnum.RequestMoveNotExist", "") & ")"
                                                bolRet = True
                                            Case Else
                                                bolRet = False
                                        End Select
                                    Else
                                        bolRet = True
                                    End If
                                End If

                            Case eRequestType.ForgottenCostCenterPunch ' Solicitud de fichaje olvidado de centros de coste
                                If Not Me.UpdateForgottenCenterPunch(_ApproveRefuse) Then
                                    Select Case Me.oState.Result
                                        Case RequestResultEnum.RequestMoveTooMany
                                            ' Si existen varios movimientos relacionados devolvemos error y marcamos la solicitud cómo denegada
                                            _ApproveRefuse = False 'Forzamos denegar solicitud
                                            _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                Me.oState.Language.Translate("ResultEnum.RequestMoveTooMany", "") & ")"
                                            bolRet = True

                                        Case RequestResultEnum.RequestMoveNotExist
                                            ' Si no existe devolvemos error y marcamos la solicitud cómo denegada
                                            _ApproveRefuse = False 'Forzamos denegar solicitud
                                            _Comments &= " (" & Me.oState.Language.Translate("RequestInfo.RequestChangedToDenied", "") &
                                                                Me.oState.Language.Translate("ResultEnum.RequestMoveNotExist", "") & ")"
                                            bolRet = True

                                        Case Else
                                            bolRet = False

                                    End Select
                                Else
                                    bolRet = True
                                End If

                            Case Else

                                bolRet = True

                        End Select
                    End If

                    If bolRet Then
                        ' Añadir el registro en 'RequestApprovals' para actualizar el estado de la solicitud
                        bolRet = Me.SetNextStatus(_ApproveRefuse, _IDPassport, bLevelOfAuthority, oSupervisorPermission, _Comments, intApprovedAtLevel, bGenerateNotification)
                        If bolRet Then
                            Dim bNotifyChangeToEmployee As Boolean = bGenerateNotification
                            bolRet = Me.Save(True, _Comments, True, bNotifyChangeToEmployee)

                            If bolRet AndAlso IdAbsence > 0 Then
                                ' Si se creó ausencia a partir de la solicitud, miro si hay documentos para traspasarlos de la solicitud a la previsión
                                If Me.iRequestType = eRequestType.PlannedAbsences OrElse Me.iRequestType = eRequestType.PlannedCauses OrElse Me.iRequestType = eRequestType.PlannedOvertimes Then
                                    Dim strSQLaux As String = $"@SELECT# Id FROM Documents WHERE IdRequest = {Me.ID}"
                                    Dim docIds As DataTable = CreateDataTable(strSQLaux)

                                    Dim oDocManager As VTDocuments.roDocumentManager = New VTDocuments.roDocumentManager(New VTDocuments.roDocumentState(oState.IDPassport))
                                    Dim oDoc As roDocument

                                    If docIds IsNot Nothing AndAlso docIds.Rows.Count > 0 Then
                                        For Each oRow As DataRow In docIds.Rows
                                            oDoc = oDocManager.LoadDocument(oRow(0))
                                            Select Case Me.iRequestType
                                                Case eRequestType.PlannedAbsences
                                                    strSQLaux = $"@UPDATE# Documents SET IdDaysAbsence = {IdAbsence} WHERE ID = {oDoc.Id}"
                                                    bolRet = ExecuteSql(strSQLaux)
                                                    ' Actualizo Id en hipotética alerta
                                                    If bolRet Then
                                                        strSQLaux = $"@UPDATE# sysroNotificationTasks SET Key5Numeric = {IdAbsence}, Parameters = REPLACE(CONVERT(VARCHAR,Parameters),'REQUEST', 'DAYS') WHERE Key5Numeric = {Me.ID} AND Key1Numeric = {Me.IDEmployee} AND IDNotification = 701  AND  Parameters LIKE 'REQUEST%' AND Key2Numeric = {oDoc.DocumentTemplate.Id}"
                                                        bolRet = ExecuteSql(strSQLaux)
                                                    End If
                                                Case eRequestType.PlannedCauses
                                                    strSQLaux = $"@UPDATE# Documents SET IdHoursAbsence = {IdAbsence} WHERE ID = {oDoc.Id.ToString}"
                                                    bolRet = ExecuteSql(strSQLaux)
                                                    If bolRet Then
                                                        strSQLaux = $"@UPDATE# sysroNotificationTasks SET Key5Numeric = {IdAbsence}, Parameters = REPLACE(CONVERT(VARCHAR,Parameters),'REQUEST', 'HOURS') WHERE Key5Numeric = {Me.ID} AND Key1Numeric = {Me.IDEmployee} AND IDNotification = 701  AND  Parameters LIKE 'REQUEST%' AND Key2Numeric = {oDoc.DocumentTemplate.Id}"
                                                        bolRet = ExecuteSql(strSQLaux)
                                                    End If
                                                Case eRequestType.PlannedOvertimes
                                                    strSQLaux = $"@UPDATE# Documents SET IdOvertimeForecast = {IdAbsence} WHERE ID = {oDoc.Id}"
                                                    bolRet = ExecuteSql(strSQLaux)
                                                    If bolRet Then
                                                        strSQLaux = $"@UPDATE# sysroNotificationTasks SET Key5Numeric = {IdAbsence}, Parameters = REPLACE(CONVERT(VARCHAR,Parameters),'REQUEST', 'OVERTIME') WHERE Key5Numeric = {Me.ID} AND Key1Numeric = {Me.IDEmployee} AND IDNotification = 701  AND  Parameters LIKE 'REQUEST%' AND Key2Numeric = {oDoc.DocumentTemplate.Id}"
                                                        bolRet = ExecuteSql(strSQLaux)
                                                    End If
                                            End Select

                                            ' Si no se trata de una aprobación automática, gestiono el documento
                                            If bolRet AndAlso (oUser Is Nothing OrElse oUser.ID <> roConstants.GetSystemUserId()) Then

                                                Try
                                                    ' Debo mirar si el supervisor puede validar, y si el documento no lo gestionó ya otro supervisor con mayor nivel de mando, en cuyo caso, no debo hacer nada.
                                                    If oDoc.Status = DocumentStatus.Pending AndAlso (oDoc.StatusLevel >= oDocManager.GetPassportLevelOfAuthority(oState.IDPassport, oDoc.DocumentTemplate.Area)) Then
                                                        If _ApproveRefuse Then
                                                            oDoc.Status = DocumentStatus.Validated
                                                            oDocManager.ChangeDocumentState(oDoc.Id, DocumentStatus.Validated, "", Now)
                                                        Else
                                                            oDoc.Status = DocumentStatus.Rejected
                                                            oDocManager.ChangeDocumentState(oDoc.Id, DocumentStatus.Rejected, "", Now)
                                                        End If
                                                    End If
                                                Catch ex As Exception
                                                End Try
                                            End If
                                        Next
                                    Else
                                        ' Gestiono posibles alertas de documentación que se pudieran haber generado para la solicitud, que ahora pasará a ser una previsión
                                        Select Case Me.iRequestType
                                            Case eRequestType.PlannedAbsences
                                                strSQLaux = $"@UPDATE# sysroNotificationTasks SET Key5Numeric = {IdAbsence}, Parameters = REPLACE(CONVERT(VARCHAR,Parameters),'REQUEST', 'DAYS') WHERE Key5Numeric = {Me.ID} AND Key1Numeric = {Me.IDEmployee} AND IDNotification = 701  AND  Parameters LIKE 'REQUEST%'"
                                                bolRet = ExecuteSql(strSQLaux)
                                            Case eRequestType.PlannedCauses
                                                strSQLaux = $"@UPDATE# sysroNotificationTasks SET Key5Numeric = {IdAbsence}, Parameters = REPLACE(CONVERT(VARCHAR,Parameters),'REQUEST', 'HOURS') WHERE Key5Numeric = {Me.ID} AND Key1Numeric = {Me.IDEmployee} AND IDNotification = 701  AND  Parameters LIKE 'REQUEST%'"
                                                bolRet = ExecuteSql(strSQLaux)
                                            Case eRequestType.PlannedOvertimes
                                                strSQLaux = $"@UPDATE# sysroNotificationTasks SET Key5Numeric = {IdAbsence}, Parameters = REPLACE(CONVERT(VARCHAR,Parameters),'REQUEST', 'OVERTIME') WHERE Key5Numeric = {Me.ID} AND Key1Numeric = {Me.IDEmployee} AND IDNotification = 701  AND  Parameters LIKE 'REQUEST%'"
                                                bolRet = ExecuteSql(strSQLaux)
                                        End Select
                                    End If
                                End If
                            End If
                        End If

                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest::ApproveRefuse")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::ApproveRefuse")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function HasPermissionOverRequest(ByVal idPassport As Integer, ByVal idRequest As Integer, ByVal _State As roRequestState) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim sSQL As String = String.Empty
                sSQL = $"@SELECT# Permission, * FROM sysrovwSecurity_PermissionOverRequests WHERE IdPassport = @idpassport AND IdRequest = @idrequest"
                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idpassport", CommandParameter.ParameterType.tInt, idPassport))
                parameters.Add(New CommandParameter("@idrequest", CommandParameter.ParameterType.tInt, idRequest))

                sSQL &= SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.HasPermissionOverRequest)

                Dim iPermission As Integer = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar(sSQL, parameters))
                bolRet = (iPermission > 3)

            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roRequest::HasPermissionOverRequest")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequest::HasPermissionOverRequest")
            End Try

            Return bolRet
        End Function

        Private Function ApproveExchangeShiftBetweenEmployees() As Boolean
            Dim bRet As Boolean = True
            Try
                ' Verificamos que el intercambio sigue siendo válido
                bRet = Me.Validate(,, True,, True)

                If bRet Then
                    ' Persistimos datos si se va a aprobar ...

                    Dim intIDPassport As Integer = roConstants.GetSystemUserId()
                    Dim oCalendarState = New roCalendarState(intIDPassport)
                    'Dim oCalendarState As New roCalendarState(1)
                    Dim oCalendarManager As New roCalendarManager(oCalendarState)

                    Dim oCalendarDayOne As DTOs.roCalendar = Nothing
                    Dim oCalendarCompensationDay As DTOs.roCalendar = Nothing
                    Dim oApplicantShiftData As roCalendarRowShiftData = Nothing
                    Dim oRequestedShiftData As roCalendarRowShiftData = Nothing
                    Dim oApplicantAssignData As roCalendarAssignmentCellData = Nothing
                    Dim oRequestedAssignData As roCalendarAssignmentCellData = Nothing
                    ' Carga de calendarios
                    ' Día solicitado
                    oCalendarDayOne = oCalendarManager.Load(Me.Date1, Me.Date1, "B" & Me.IDEmployee & ",B" & Me.IDEmployeeExchange, CalendarView.Planification, CalendarDetailLevel.Daily, True)
                    For Each oCalDayOneRow As roCalendarRow In oCalendarDayOne.CalendarData
                        If oCalDayOneRow.EmployeeData.IDEmployee = Me.IDEmployee Then
                            oApplicantShiftData = oCalDayOneRow.PeriodData.DayData(0).MainShift
                            oApplicantAssignData = oCalDayOneRow.PeriodData.DayData(0).AssigData
                        ElseIf oCalDayOneRow.EmployeeData.IDEmployee = Me.IDEmployeeExchange Then
                            oRequestedShiftData = oCalDayOneRow.PeriodData.DayData(0).MainShift
                            oRequestedAssignData = oCalDayOneRow.PeriodData.DayData(0).AssigData
                        End If
                    Next

                    If oApplicantShiftData IsNot Nothing AndAlso oRequestedShiftData IsNot Nothing Then
                        Me.oState.Language.ClearUserTokens()
                        Me.oState.Language.AddUserToken(Common.roBusinessSupport.GetEmployeeName(Me.IDEmployee, Me.oState))
                        Me.oState.Language.AddUserToken(Common.roBusinessSupport.GetEmployeeName(Me.IDEmployeeExchange, Me.oState))
                        Me.oState.Language.AddUserToken(oRequestedShiftData.Name.ToString)
                        Me.oState.Language.AddUserToken(oApplicantShiftData.Name.ToString)

                        For Each oCalDayOneRow As roCalendarRow In oCalendarDayOne.CalendarData
                            If oCalDayOneRow.EmployeeData.IDEmployee = Me.IDEmployee Then
                                Dim remark1 = New VTBusiness.Shift.roRemark()
                                oCalDayOneRow.PeriodData.DayData(0).MainShift = oRequestedShiftData
                                oCalDayOneRow.PeriodData.DayData(0).AssigData = oRequestedAssignData
                                oCalDayOneRow.PeriodData.DayData(0).HasChanged = True

                                If oCalDayOneRow.PeriodData.DayData(0).Remarks = "0" Then
                                    remark1.Text = Me.oState.Language.Translate("RequestsHelper.ShiftExchangeAtoB", "")
                                Else
                                    Dim dailySchedule = New VTBusiness.Shift.roDailySchedule(Me.IDEmployee, oCalDayOneRow.PeriodData.DayData(0).PlanDate, New Shift.roShiftState())
                                    remark1 = dailySchedule.Remarks
                                    remark1.Text = $"{remark1.Text}{vbCrLf}{Me.oState.Language.Translate("RequestsHelper.ShiftExchangeBtoA", "")}"
                                End If
                                If remark1.Save() Then
                                    oCalDayOneRow.PeriodData.DayData(0).Remarks = remark1.ID.ToString()
                                Else
                                    bRet = False
                                    Me.oState.Result = RequestResultEnum.SaveCalendarException
                                End If
                            ElseIf oCalDayOneRow.EmployeeData.IDEmployee = Me.IDEmployeeExchange Then
                                Dim remark2 = New VTBusiness.Shift.roRemark()
                                oCalDayOneRow.PeriodData.DayData(0).MainShift = oApplicantShiftData
                                oCalDayOneRow.PeriodData.DayData(0).AssigData = oApplicantAssignData
                                oCalDayOneRow.PeriodData.DayData(0).HasChanged = True

                                If oCalDayOneRow.PeriodData.DayData(0).Remarks = "0" Then
                                    remark2.Text = Me.oState.Language.Translate("RequestsHelper.ShiftExchangeBtoA", "")
                                Else
                                    Dim dailySchedule = New VTBusiness.Shift.roDailySchedule(Me.IDEmployeeExchange, oCalDayOneRow.PeriodData.DayData(0).PlanDate, New Shift.roShiftState())
                                    remark2 = dailySchedule.Remarks
                                    remark2.Text = $"{remark2.Text}{vbCrLf}{Me.oState.Language.Translate("RequestsHelper.ShiftExchangeBtoA", "")}"
                                End If
                                If remark2.Save() Then
                                    oCalDayOneRow.PeriodData.DayData(0).Remarks = remark2.ID.ToString()
                                Else
                                    bRet = False
                                    Me.oState.Result = RequestResultEnum.SaveCalendarException
                                End If
                            End If
                        Next

                        If Me.Date2 IsNot Nothing Then
                            ' Si hay compensación
                            Dim oApplicantCompensationShiftData As roCalendarRowShiftData = Nothing
                            Dim oRequestedCompensationShiftData As roCalendarRowShiftData = Nothing
                            Dim oApplicantCompensationAssignData As roCalendarAssignmentCellData = Nothing
                            Dim oRequestedCompensationAssignData As roCalendarAssignmentCellData = Nothing
                            oCalendarCompensationDay = oCalendarManager.Load(Me.Date2, Me.Date2, "B" & Me.IDEmployee & ",B" & Me.IDEmployeeExchange, CalendarView.Planification, CalendarDetailLevel.Daily, True)
                            For Each oCalCompensationDayRow As roCalendarRow In oCalendarCompensationDay.CalendarData
                                If oCalCompensationDayRow.EmployeeData.IDEmployee = Me.IDEmployee Then
                                    oApplicantCompensationShiftData = oCalCompensationDayRow.PeriodData.DayData(0).MainShift
                                    oApplicantCompensationAssignData = oCalCompensationDayRow.PeriodData.DayData(0).AssigData
                                ElseIf oCalCompensationDayRow.EmployeeData.IDEmployee = Me.IDEmployeeExchange Then
                                    oRequestedCompensationShiftData = oCalCompensationDayRow.PeriodData.DayData(0).MainShift
                                    oRequestedCompensationAssignData = oCalCompensationDayRow.PeriodData.DayData(0).AssigData
                                End If
                            Next

                            If oApplicantCompensationShiftData IsNot Nothing AndAlso oRequestedCompensationShiftData IsNot Nothing Then
                                Me.oState.Language.ClearUserTokens()
                                Me.oState.Language.AddUserToken(Common.roBusinessSupport.GetEmployeeName(Me.IDEmployee, Me.oState))
                                Me.oState.Language.AddUserToken(Common.roBusinessSupport.GetEmployeeName(Me.IDEmployeeExchange, Me.oState))
                                Me.oState.Language.AddUserToken(oApplicantCompensationShiftData.Name.ToString)
                                Me.oState.Language.AddUserToken(oRequestedCompensationShiftData.Name.ToString)

                                For Each oCalCompensationDayRow As roCalendarRow In oCalendarCompensationDay.CalendarData
                                    If oCalCompensationDayRow.EmployeeData.IDEmployee = Me.IDEmployee Then
                                        Dim remark1 = New VTBusiness.Shift.roRemark()
                                        oCalCompensationDayRow.PeriodData.DayData(0).MainShift = oRequestedCompensationShiftData
                                        oCalCompensationDayRow.PeriodData.DayData(0).AssigData = oRequestedCompensationAssignData
                                        oCalCompensationDayRow.PeriodData.DayData(0).HasChanged = True

                                        If oCalCompensationDayRow.PeriodData.DayData(0).Remarks = "0" Then
                                            remark1.Text = Me.oState.Language.Translate("RequestsHelper.ShiftExchangeAtoB", "")
                                        Else
                                            Dim dailySchedule = New VTBusiness.Shift.roDailySchedule(Me.IDEmployee, oCalCompensationDayRow.PeriodData.DayData(0).PlanDate, New Shift.roShiftState())
                                            remark1 = dailySchedule.Remarks
                                            remark1.Text = $"{remark1.Text}{vbCrLf}{Me.oState.Language.Translate("RequestsHelper.ShiftExchangeBtoA", "")}"
                                        End If
                                        If remark1.Save() Then
                                            oCalCompensationDayRow.PeriodData.DayData(0).Remarks = remark1.ID.ToString()
                                        Else
                                            bRet = False
                                            Me.oState.Result = RequestResultEnum.SaveCalendarException
                                        End If
                                    ElseIf oCalCompensationDayRow.EmployeeData.IDEmployee = Me.IDEmployeeExchange Then
                                        Dim remark2 = New VTBusiness.Shift.roRemark()
                                        oCalCompensationDayRow.PeriodData.DayData(0).MainShift = oApplicantCompensationShiftData
                                        oCalCompensationDayRow.PeriodData.DayData(0).AssigData = oApplicantCompensationAssignData
                                        oCalCompensationDayRow.PeriodData.DayData(0).HasChanged = True

                                        If oCalCompensationDayRow.PeriodData.DayData(0).Remarks = "0" Then
                                            remark2.Text = Me.oState.Language.Translate("RequestsHelper.ShiftExchangeBtoA", "")
                                        Else
                                            Dim dailySchedule = New VTBusiness.Shift.roDailySchedule(Me.IDEmployeeExchange, oCalCompensationDayRow.PeriodData.DayData(0).PlanDate, New Shift.roShiftState())
                                            remark2 = dailySchedule.Remarks
                                            remark2.Text = $"{remark2.Text}{vbCrLf}{Me.oState.Language.Translate("RequestsHelper.ShiftExchangeBtoA", "")}"
                                        End If
                                        If remark2.Save() Then
                                            oCalCompensationDayRow.PeriodData.DayData(0).Remarks = remark2.ID.ToString()
                                        Else
                                            bRet = False
                                            Me.oState.Result = RequestResultEnum.SaveCalendarException
                                        End If
                                    End If
                                Next

                            Else
                                bRet = False
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "roRequest::ApproveExchangeShiftBetweenEmployees::Both applicant and requested compensation day should be specified")
                            End If
                        End If

                        Me.oState.Language.ClearUserTokens()

                        If bRet Then
                            Dim oCalendarResult As roCalendarResult
                            oCalendarResult = oCalendarManager.Save(oCalendarDayOne, True, True)
                            If oCalendarResult.Status <> CalendarStatusEnum.OK Then
                                bRet = False
                                Me.oState.Result = RequestResultEnum.SaveCalendarException
                            ElseIf oCalendarCompensationDay IsNot Nothing Then
                                oCalendarResult = oCalendarManager.Save(oCalendarCompensationDay, True, True)
                                If oCalendarResult.Status <> CalendarStatusEnum.OK Then
                                    bRet = False
                                    Me.oState.Result = RequestResultEnum.SaveCalendarException
                                End If
                            End If
                        End If
                    Else
                        bRet = False
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "roRequest::ApproveExchangeShiftBetweenEmployees::Both applicant and requested day should be specified")
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roRequest::ApproveExchangeShiftBetweenEmployees::Unexpected error approving exchange shift between employees request.", ex)
                bRet = False
            End Try

            Return bRet
        End Function

        Public Function GetApprovalStatus() As roRequestApproval

            Dim oRet As roRequestApproval = Nothing
            Dim bolRet As Boolean = True

            Try
                If Me.lstRequestApprovals IsNot Nothing Then

                    If Me.lstRequestApprovals.Count > 0 Then
                        ' Obtenemos la última estado de aprobación/denegación
                        oRet = Me.lstRequestApprovals(Me.lstRequestApprovals.Count - 1)
                    End If

                End If

                If oRet Is Nothing Then
                    oRet = New roRequestApproval()
                    oRet.IDRequest = Me.ID
                    oRet.IDPassport = 0
                    oRet.ApprovalDateTime = Me.RequestDate
                    oRet.Status = Me.iStatus

                    ' Debemos asignar el nivel mínimo de aprobación que se necesita para poder empezar a gestionarla
                    Dim strSQLLevel As String = "@SELECT# isnull(dbo.GetMinLevelOfRequest(" & Me.RequestType & "," & IIf(Me.IDCause.HasValue, Me.IDCause.ToString, "0") & "),11) AS MinLevel "
                    oRet.StatusLevel = Any2Integer(ExecuteScalar(strSQLLevel))
                End If
            Catch ex As Exception
                bolRet = False
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Añade un nueva validación a la solicitud. Si se trata de una validación de aceptación y en función del pasaporte/supervisor, se actualizará la lista de aprobaciones de la solicitud.
        ''' </summary>
        ''' <param name="_ApproveRefuse"></param>
        ''' <param name="_IDPassport"></param>
        ''' <param name="_LevelOfAuthority"></param>
        ''' <param name="_SupervisorPermission"></param>
        ''' <param name="oActiveTransaction"></param>e
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SetNextStatus(ByVal _ApproveRefuse As Boolean, ByVal _IDPassport As Integer, ByVal _LevelOfAuthority As Byte, ByVal _SupervisorPermission As Permission, ByVal _Comments As String, ByVal intLevelApproved As Integer, Optional ByVal bGenerateNotification As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oLastStatus As eRequestStatus = Me.RequestStatus

                Dim oNewApproval As New roRequestApproval(Me.intID, _IDPassport, Now, Me.oState)
                If _ApproveRefuse Then
                    If _LevelOfAuthority = 1 OrElse _SupervisorPermission = Permission.Admin OrElse intLevelApproved = _LevelOfAuthority Then
                        oNewApproval.Status = eRequestStatus.Accepted
                    Else
                        oNewApproval.Status = eRequestStatus.OnGoing
                    End If
                Else
                    oNewApproval.Status = eRequestStatus.Denied
                End If

                oNewApproval.StatusLevel = _LevelOfAuthority
                oNewApproval.Comments = _Comments

                ' Miramos la última validación sobre la solicitud es la mimsa (misma acción y mismo supervisor)
                Dim oApprovalStatus As roRequestApproval = Me.GetApprovalStatus()
                If Not (oApprovalStatus.StatusLevel = oNewApproval.StatusLevel AndAlso oApprovalStatus.IDPassport = oNewApproval.IDPassport AndAlso oApprovalStatus.Status = oNewApproval.Status) Then

                    Me.lstRequestApprovals.Add(oNewApproval)

                    Me.iStatus = oNewApproval.Status

                    If Me.RequestType <> eRequestType.ExchangeShiftBetweenEmployees OrElse (Me.RequestType = eRequestType.ExchangeShiftBetweenEmployees AndAlso oLastStatus <> eRequestStatus.Pending) Then
                        Me.iStatusLevel = oNewApproval.StatusLevel
                    End If

                    If Me.RequestType = eRequestType.ExchangeShiftBetweenEmployees AndAlso oLastStatus = eRequestStatus.Pending AndAlso oNewApproval.Status = eRequestStatus.OnGoing Then
                        ' En el caso que el segundo empleado guarde la solicitud
                        ' se valida únicamente la regla de validacion automatica en caso necesario
                        roRequestRuleManager.RequestRulesValidate(Me, oState,,, True)
                    End If

                    Me.bolNotReaded = True

                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest::SetNextStatus")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::SetNextStatus")
            End Try

            Return bolRet

        End Function

        Public Function RequestInfo(ByVal bolDetail As Boolean) As String

            Dim strRet As String = ""

            Try
                Dim strMsg As String = "RequestInfo"
                If bolDetail Then strMsg &= ".Detail"

                Select Case Me.RequestType
                    Case eRequestType.UserFieldsChange

                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.FieldName)
                        Me.Language.AddUserToken(Me.FieldValue)
                        If Not Me.Date1.HasValue Then
                            strMsg &= ".UserFieldsChange"
                        Else
                            strMsg &= ".UserFieldsChange.History"

                            Me.Language.AddUserToken(Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        End If
                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()

                    Case eRequestType.ForbiddenPunch
                        Me.Language.ClearUserTokens()

                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        Me.Language.AddUserToken(Me.Date1.Value.ToShortTimeString)
                        If Not Me.IDCause.HasValue Then
                            strMsg &= ".ForbiddenPunch"
                        Else
                            strMsg &= ".ForbiddenPunch.WithCause"
                            ' Obtenemos el nombre de la justificación
                            Dim oCauseState As New Cause.roCauseState
                            roBusinessState.CopyTo(Me.oState, oCauseState)
                            Dim oCause As New Cause.roCause(Me.IDCause, oCauseState, False)
                            Me.Language.AddUserToken(oCause.Name)
                        End If
                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()
                    Case eRequestType.Telecommute
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        Dim location = ""
                        If Me.Field1 = "1" Then
                            strMsg &= ".TelecommuteHome"
                        Else
                            strMsg &= ".TelecommuteOffice"
                        End If

                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()
                    Case eRequestType.JustifyPunch
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        Me.Language.AddUserToken(Me.Date1.Value.ToShortTimeString)
                        strMsg &= ".JustifyPunch"
                        ' Obtenemos el nombre de la justificación
                        Dim oCauseState As New Cause.roCauseState
                        roBusinessState.CopyTo(Me.oState, oCauseState)
                        Dim oCause As New Cause.roCause(Me.IDCause, oCauseState, False)
                        Me.Language.AddUserToken(oCause.Name)
                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()

                    Case eRequestType.ExternalWorkResumePart
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        Me.Language.AddUserToken(Me.strHours)

                        If Me.IDCause.HasValue Then
                            Dim oCauseState As New Cause.roCauseState
                            roBusinessState.CopyTo(oState, oCauseState)
                            Dim oCause As New Cause.roCause(Me.IDCause, oCauseState, False)
                            Me.oState.Language.AddUserToken(oCause.Name)
                        Else
                            Me.oState.Language.AddUserToken(Me.oState.Language.Translate("List.NoCause", ""))
                        End If

                        strMsg &= ".ExternalWorkResumePart"
                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()
                    Case eRequestType.ChangeShift
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        Me.Language.AddUserToken(Me.Date2.Value.ToString(oState.Language.GetShortDateFormat))
                        ' Obtenemos el nombre del horario
                        Dim oShiftState As New Shift.roShiftState
                        roBusinessState.CopyTo(Me.oState, oShiftState)
                        Dim oShift As New Shift.roShift(Me.IDShift, oShiftState)
                        Dim strName As String = oShift.Name
                        If Me.StartShift.HasValue Then strName = oShift.FloatingShiftName(Me.StartShift.Value)
                        Me.Language.AddUserToken(strName)

                        If Any2Integer(Field4) > 0 Then
                            Dim oRepShift As New Shift.roShift(Any2Integer(Field4), oShiftState)
                            Dim strRepName As String = oRepShift.Name
                            Me.Language.AddUserToken(strRepName)

                            If Me.Date1 <> Me.Date2 Then
                                strMsg &= ".ChangeShift"
                            Else
                                strMsg &= ".ChangeShift.OneDate"
                            End If
                        Else
                            If Me.Date1 <> Me.Date2 Then
                                strMsg &= ".ChangeAllShift"
                            Else
                                strMsg &= ".ChangeAllShift.OneDate"
                            End If
                        End If

                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()

                    Case eRequestType.VacationsOrPermissions
                        Me.Language.ClearUserTokens()
                        Dim requestDatesByPeriod = BuildDatesByPeriodString(Me.ID, Me.Language, Me.Date1, Me.Date2)
                        Me.Language.AddUserToken(requestDatesByPeriod)
                        ' Obtenemos el nombre del horario
                        Dim oShiftState As New Shift.roShiftState
                        roBusinessState.CopyTo(Me.oState, oShiftState)
                        Dim oShift As New Shift.roShift(Me.IDShift, oShiftState)
                        Dim strName As String = oShift.Name
                        If Me.StartShift.HasValue Then strName = oShift.FloatingShiftName(Me.StartShift.Value)
                        Me.Language.AddUserToken(strName)

                        Dim oRequest As New Requests.roRequest(Me.ID, oState)

                        If oRequest.lstRequestDays Is Nothing OrElse oRequest.lstRequestDays.Count = 0 Then
                            ' version v1
                            If Me.Date1 <> Me.Date2 Then
                                Dim intDays As Integer = 0
                                strMsg &= ".VacationsOrPermissions"
                                If oShift.AreWorkingDays Then
                                    intDays = roBusinessSupport.LaboralDaysInPeriod(Me.IDEmployee, Me.Date1, Me.Date2, oState)
                                Else
                                    intDays = Math.Abs(DateDiff(DateInterval.Day, Me.Date1.Value.Date, Me.Date2.Value.Date)) + 1
                                End If
                                Me.Language.AddUserToken(Math.Abs(intDays))
                            Else
                                strMsg &= ".VacationsOrPermissions.OneDate"
                            End If
                            strRet = Me.oState.Language.Translate(strMsg, "")
                            Me.Language.ClearUserTokens()
                        Else
                            If oRequest.lstRequestDays.Count > 1 Then
                                Dim intDays As Integer = 0
                                strMsg &= ".VacationsOrPermissions"
                                intDays = oRequest.lstRequestDays.Count
                                Me.Language.AddUserToken(Math.Abs(intDays))
                            Else
                                strMsg &= ".VacationsOrPermissions.OneDate"
                            End If

                            strRet = Me.Language.Translate(strMsg, "")
                            Me.Language.ClearUserTokens()
                        End If

                    Case eRequestType.CancelHolidays
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        Me.Language.AddUserToken(Me.Date2.Value.ToString(oState.Language.GetShortDateFormat))

                        Dim oRequest As New Requests.roRequest(Me.ID, oState)

                        If oRequest.lstRequestDays Is Nothing OrElse oRequest.lstRequestDays.Count = 0 Then
                            ' version v1
                            If Me.Date1 <> Me.Date2 Then
                                Dim intDays As Integer = 0
                                strMsg &= ".CancelHolidays"
                                intDays = roBusinessSupport.HolidayDaysInPeriod(Me.IDEmployee, Me.Date1, Me.Date2, oState)

                                Me.Language.AddUserToken(Math.Abs(intDays))
                            Else
                                strMsg &= ".CancelHolidays.OneDate"
                            End If
                            strRet = Me.oState.Language.Translate(strMsg, "")
                            Me.Language.ClearUserTokens()
                        Else

                            If Me.IDShift.HasValue AndAlso Me.IDShift.Value > 0 Then
                                ' Cancelamos dias de vacaciones
                                Dim oShiftState As New Shift.roShiftState
                                roBusinessState.CopyTo(Me.oState, oShiftState)
                                Dim oRepShift As New Shift.roShift(Me.IDShift.Value, oShiftState)
                                Dim strRepName As String = oRepShift.Name
                                Me.Language.AddUserToken(strRepName)
                                strMsg &= ".CancelHolidaysShiftCause"
                            End If

                            If Me.IDCause.HasValue AndAlso Me.IDCause.Value > 0 Then
                                ' Cancelamos previsiones de vacaciones por horas
                                Dim oCauseState As New Cause.roCauseState
                                roBusinessState.CopyTo(Me.oState, oCauseState)
                                Dim oRepCause As New Cause.roCause(Me.IDCause.Value, oCauseState)
                                Dim strRepName As String = oRepCause.Name
                                Me.Language.AddUserToken(strRepName)
                                strMsg &= ".CancelHolidaysShiftCause"
                            End If

                            If oRequest.lstRequestDays.Count > 1 Then
                                Dim intDays As Integer = 0
                                strMsg &= ".CancelHolidays"
                                intDays = oRequest.lstRequestDays.Count
                                Me.Language.AddUserToken(Math.Abs(intDays))
                            Else
                                strMsg &= ".CancelHolidays.OneDate"
                            End If
                            strRet = Me.Language.Translate(strMsg, "")
                            Me.Language.ClearUserTokens()

                        End If

                    Case eRequestType.PlannedAbsences
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        Me.Language.AddUserToken(Me.Date2.Value.ToString(oState.Language.GetShortDateFormat))
                        ' Obtenemos el nombre de la justificación
                        Dim oCauseState As New Cause.roCauseState
                        roBusinessState.CopyTo(Me.oState, oCauseState)
                        Dim oCause As New Cause.roCause(Me.IDCause, oCauseState, False)
                        Me.Language.AddUserToken(oCause.Name)
                        strMsg &= ".PlannedAbsences"
                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()

                    Case eRequestType.PlannedCauses
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        If Me.Date2.HasValue Then
                            Me.Language.AddUserToken(Me.Date2.Value.ToString(oState.Language.GetShortDateFormat))
                        Else
                            Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        End If
                        ' Obtenemos el nombre de la justificación
                        Dim oCauseState As New Cause.roCauseState
                        roBusinessState.CopyTo(Me.oState, oCauseState)
                        Dim oCause As New Cause.roCause(Me.IDCause, oCauseState, False)
                        Me.Language.AddUserToken(oCause.Name)
                        Me.Language.AddUserToken(Me.strHours)
                        Me.Language.AddUserToken(Me.FromTime.Value.ToShortTimeString)
                        Me.Language.AddUserToken(Me.ToTime.Value.ToShortTimeString)
                        strMsg &= ".PlannedCauses"
                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()

                    Case eRequestType.ExternalWorkWeekResume
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        If Me.Date2.HasValue Then
                            Me.Language.AddUserToken(Me.Date2.Value.ToString(oState.Language.GetShortDateFormat))
                        Else
                            Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        End If

                        Dim oRequest As New Requests.roRequest(Me.ID, oState)

                        Dim intDays As Integer = 0
                        strMsg &= ".ExternalWorkWeekResume"
                        intDays = oRequest.lstRequestDays.Count
                        Me.Language.AddUserToken(Math.Abs(intDays))

                        strRet = Me.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()

                    Case eRequestType.PlannedOvertimes

                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        If Me.Date2.HasValue Then
                            Me.Language.AddUserToken(Me.Date2.Value.ToString(oState.Language.GetShortDateFormat))
                        Else
                            Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        End If
                        ' Obtenemos el nombre de la justificación
                        Dim oCauseState As New Cause.roCauseState
                        roBusinessState.CopyTo(Me.oState, oCauseState)
                        Dim oCause As New Cause.roCause(Me.IDCause, oCauseState, False)
                        Me.Language.AddUserToken(oCause.Name)
                        Me.Language.AddUserToken(Me.strHours)
                        Me.Language.AddUserToken(Me.FromTime.Value.ToShortTimeString)
                        Me.Language.AddUserToken(Me.ToTime.Value.ToShortTimeString)
                        strMsg &= ".PlannedOvertimes"
                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()

                    Case eRequestType.PlannedHolidays
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        If Date2.HasValue Then
                            Me.Language.AddUserToken(Me.Date2.Value.ToString(oState.Language.GetShortDateFormat))
                        Else
                            Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        End If
                        ' Obtenemos el nombre de la justificación
                        Dim oCauseState As New Cause.roCauseState
                        roBusinessState.CopyTo(oState, oCauseState)
                        Dim oCause As New Cause.roCause(Me.IDCause, oCauseState, False)
                        Me.Language.AddUserToken(oCause.Name)

                        Dim oRequest As New Requests.roRequest(Me.ID, oState)

                        If oRequest.lstRequestDays.Count > 1 Then
                            Dim intDays As Integer = 0
                            strMsg &= ".PlannedHolidays"
                            intDays = oRequest.lstRequestDays.Count
                            Me.Language.AddUserToken(Math.Abs(intDays))
                        Else
                            strMsg &= ".PlannedHolidays.OneDate"
                        End If

                        If oRequest.lstRequestDays(0).AllDay Then
                            strMsg &= ".AllDay"
                        Else
                            strMsg &= ".Period"
                            Me.Language.AddUserToken(oRequest.lstRequestDays(0).FromTime.Value.ToShortTimeString)
                            Me.Language.AddUserToken(oRequest.lstRequestDays(0).ToTime.Value.ToShortTimeString)
                            Me.Language.AddUserToken(roConversions.ConvertHoursToTime(oRequest.lstRequestDays(0).Duration.Value))

                        End If
                        strRet = Me.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()

                    Case eRequestType.ExchangeShiftBetweenEmployees
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))

                        Me.Language.AddUserToken(roBusinessSupport.GetEmployeeName(Me.IDEmployee, oState))
                        Me.Language.AddUserToken(roBusinessSupport.GetEmployeeName(Me.IDEmployeeExchange, oState))

                        Me.Language.AddUserToken(Robotics.Base.VTBusiness.Shift.roShift.GetName(Me.IDShift, New Shift.roShiftState()))
                        Me.Language.AddUserToken(Robotics.Base.VTBusiness.Shift.roShift.GetName(Me.Field4, New Shift.roShiftState()))

                        If Me.Date2 IsNot Nothing Then
                            ' Hay compensación
                            Me.Language.AddUserToken(Me.Date2.Value.ToString(oState.Language.GetShortDateFormat))

                            ' Busco id's de horarios del día de compensación
                            Dim idRequestedCompensationIdShift As Integer = -1
                            Dim idApplicantCompensationIdShift As Integer = -1

                            Dim strSQLAux As String = String.Empty

                            strSQLAux = "@SELECT# isnull(IDShiftUsed, Idshift1) from DailySchedule where date = " & roTypes.Any2Time(Me.Date2).SQLSmallDateTime & " and IDEmployee = " & Me.IDEmployeeExchange.ToString
                            idRequestedCompensationIdShift = Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQLAux))

                            strSQLAux = "@SELECT# isnull(IDShiftUsed, Idshift1) from DailySchedule where date = " & roTypes.Any2Time(Me.Date2).SQLSmallDateTime & " and IDEmployee = " & Me.IDEmployee.ToString
                            idApplicantCompensationIdShift = Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQLAux))

                            Me.Language.AddUserToken(Robotics.Base.VTBusiness.Shift.roShift.GetName(idRequestedCompensationIdShift, New Shift.roShiftState()))
                            Me.Language.AddUserToken(Robotics.Base.VTBusiness.Shift.roShift.GetName(idApplicantCompensationIdShift, New Shift.roShiftState()))

                            strMsg &= ".ExchangeShiftBetweenEmployees.WithCompensation"
                        Else
                            strMsg &= ".ExchangeShiftBetweenEmployees"
                        End If

                        strRet = Me.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()

                    Case eRequestType.ForbiddenTaskPunch
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        Me.Language.AddUserToken(Me.Date1.Value.ToShortTimeString)
                        Dim oTaskState As New Task.roTaskState
                        roBusinessState.CopyTo(Me.oState, oTaskState)

                        ' Obtenemos el nombre de la tarea
                        Dim oTask As New Task.roTask(Me.IDTask1, oTaskState)
                        If oTask IsNot Nothing Then
                            Me.Language.AddUserToken(oTask.Project & " " & oTask.Name)
                        End If

                        If Not Me.Date2.HasValue Then
                            strMsg &= ".ForbiddenTaskPunch"
                        Else
                            If Any2Boolean(Me.CompletedTask) Then
                                strMsg &= ".ForbiddenTaskPunchWithEndExtendedCompleted"
                            Else
                                strMsg &= ".ForbiddenTaskPunchWithEndExtendedNotCompleted"
                            End If

                            Me.Language.AddUserToken(Me.Date2.Value.ToShortTimeString)

                            oTask = New Task.roTask(Any2Long(Me.IDTask2), oTaskState)
                            If oTask IsNot Nothing Then
                                Me.Language.AddUserToken(oTask.Project & " " & oTask.Name)
                            Else
                                Me.Language.AddUserToken("")
                            End If

                            Me.Language.AddUserToken(Any2String(Me.Field1))
                            Me.Language.AddUserToken(Any2String(Me.Field2))
                            Me.Language.AddUserToken(Any2String(Me.Field3))
                            If Any2Double(Me.Field4) >= 0 Then
                                Me.Language.AddUserToken(Format(Any2Double(Me.Field4), "##0.00"))
                            Else
                                Me.Language.AddUserToken("")
                            End If

                            If Any2Double(Me.Field5) >= 0 Then
                                Me.Language.AddUserToken(Format(Any2Double(Me.Field5), "##0.00"))
                            Else
                                Me.Language.AddUserToken("")
                            End If
                            If Any2Double(Me.Field6) >= 0 Then
                                Me.Language.AddUserToken(Format(Any2Double(Me.Field6), "##0.00"))
                            Else
                                Me.Language.AddUserToken("")
                            End If

                        End If

                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()
                    Case eRequestType.ForgottenCostCenterPunch
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))
                        Me.Language.AddUserToken(Me.Date1.Value.ToShortTimeString)

                        strMsg &= ".ForgottenCostCenterPunch"
                        ' Obtenemos el nombre de la justificación
                        Dim oCostCenterState As New BusinessCenter.roBusinessCenterState
                        roBusinessState.CopyTo(Me.oState, oCostCenterState)
                        Dim oCostCenter As New BusinessCenter.roBusinessCenter(Me.IDCenter, oCostCenterState, False)
                        Me.Language.AddUserToken(oCostCenter.Name)

                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()
                    Case eRequestType.DailyRecord
                        Me.Language.ClearUserTokens()
                        Me.Language.AddUserToken(Me.Date1.Value.ToString(oState.Language.GetShortDateFormat))

                        strMsg &= ".dailyRecord"

                        strRet = Me.oState.Language.Translate(strMsg, "")
                        Me.Language.ClearUserTokens()
                End Select
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest::RequestInfo")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::RequestInfo")
            End Try

            Return strRet

        End Function

        Private Function Language() As roLanguage
            Return Me.oState.Language
        End Function

        Private Function UpdateForbiddenTaskPunch(ByVal _ApproveRefuse As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.RequestType = eRequestType.ForbiddenTaskPunch Then

                    ' Creamos el fichaje de tareas con sus atributos
                    Dim oPunchState As New Punch.roPunchState
                    roBusinessState.CopyTo(Me.oState, oPunchState)
                    Dim oPunch As New Punch.roPunch(oPunchState)
                    oPunch.IDEmployee = Me.IDEmployee
                    oPunch.Type = PunchTypeEnum._TASK
                    oPunch.ActualType = PunchTypeEnum._TASK
                    oPunch.TypeData = Me.IDTask1
                    oPunch.DateTime = Me.Date1
                    oPunch.Source = PunchSource.Request
                    oPunch.Field4 = -1
                    oPunch.Field5 = -1
                    oPunch.Field6 = -1
                    oPunch.Remarks = Me.Comments
                    oPunch.NotReliableCause = DTOs.NotReliableCause.TaskForgottenPunch.ToString()

                    If oPunch.Save() Then
                        bolRet = True
                        If Me.CompletedTask Then
                            ' Completamos la tarea en caso necesario
                            Dim oState As New Task.roTaskState
                            roBusinessState.CopyTo(Me.oState, oState)
                            Dim oTask1 As New Task.roTask(Me.IDTask1, oState)
                            If oTask1 IsNot Nothing Then
                                If oTask1.Status = TaskStatusEnum._ON Then
                                    oTask1.Status = TaskStatusEnum._PENDING
                                    oTask1.IDEmployeeUpdateStatus = Me.IDEmployee
                                    If Me.Date2.HasValue Then
                                        oTask1.UpdateStatusDate = Date2
                                    Else
                                        oTask1.UpdateStatusDate = Now
                                    End If

                                    If Not oTask1.Save() Then
                                        bolRet = False
                                    End If
                                End If
                            End If
                        End If

                        If bolRet And Me.IDTask2.HasValue And Me.Date2.HasValue Then

                            ' Creamos el final de tarea con los atributos necesarios
                            Dim oPunch2 As New Punch.roPunch(oPunchState)
                            oPunch2.IDEmployee = Me.IDEmployee
                            oPunch2.Type = PunchTypeEnum._TASK
                            oPunch2.ActualType = PunchTypeEnum._TASK
                            oPunch2.TypeData = Me.IDTask1
                            oPunch2.DateTime = Me.Date2

                            Dim oFieldState As New roTaskFieldState
                            roBusinessState.CopyTo(Me.oState, oFieldState)

                            Dim TaskFieldsTask As Generic.List(Of roTaskField) = roTaskField.GetTaskFieldsList(Me.IDTask1, oFieldState)
                            Dim strField1 As String = ""
                            Dim strField2 As String = ""
                            Dim strField3 As String = ""
                            Dim dblField4 As Double = -1
                            Dim dblField5 As Double = -1
                            Dim dblField6 As Double = -1

                            oPunch2.Field1 = strField1
                            oPunch2.Field2 = strField2
                            oPunch2.Field3 = strField3
                            oPunch2.Field4 = dblField4
                            oPunch2.Field5 = dblField5
                            oPunch2.Field6 = dblField6

                            Dim bolInsertPunch As Boolean = False
                            Dim indexField As Integer = 0
                            For indexField = 0 To TaskFieldsTask.Count - 1

                                If TaskFieldsTask(indexField).IDField = 1 And TaskFieldsTask(indexField).Action = ActionTypes.tChange Then
                                    oPunch2.Field1 = Me.Field1
                                    bolInsertPunch = True
                                ElseIf TaskFieldsTask(indexField).IDField = 1 And TaskFieldsTask(indexField).Action = ActionTypes.tComplete Then
                                    strField1 = Me.Field1
                                ElseIf TaskFieldsTask(indexField).IDField = 2 And TaskFieldsTask(indexField).Action = ActionTypes.tChange Then
                                    oPunch2.Field2 = Me.Field2
                                    bolInsertPunch = True
                                ElseIf TaskFieldsTask(indexField).IDField = 2 And TaskFieldsTask(indexField).Action = ActionTypes.tComplete Then
                                    strField2 = Me.Field2
                                ElseIf TaskFieldsTask(indexField).IDField = 3 And TaskFieldsTask(indexField).Action = ActionTypes.tChange Then
                                    oPunch2.Field3 = Me.Field3
                                    bolInsertPunch = True
                                ElseIf TaskFieldsTask(indexField).IDField = 3 And TaskFieldsTask(indexField).Action = ActionTypes.tComplete Then
                                    strField3 = Me.Field3
                                ElseIf TaskFieldsTask(indexField).IDField = 4 And TaskFieldsTask(indexField).Action = ActionTypes.tChange Then
                                    oPunch2.Field4 = Me.Field4
                                    bolInsertPunch = True
                                ElseIf TaskFieldsTask(indexField).IDField = 4 And TaskFieldsTask(indexField).Action = ActionTypes.tComplete Then
                                    dblField4 = Me.Field4
                                ElseIf TaskFieldsTask(indexField).IDField = 5 And TaskFieldsTask(indexField).Action = ActionTypes.tChange Then
                                    oPunch2.Field5 = Me.Field5
                                    bolInsertPunch = True
                                ElseIf TaskFieldsTask(indexField).IDField = 5 And TaskFieldsTask(indexField).Action = ActionTypes.tComplete Then
                                    dblField5 = Me.Field5
                                ElseIf TaskFieldsTask(indexField).IDField = 6 And TaskFieldsTask(indexField).Action = ActionTypes.tChange Then
                                    oPunch2.Field6 = Me.Field6
                                    bolInsertPunch = True
                                ElseIf TaskFieldsTask(indexField).IDField = 6 And TaskFieldsTask(indexField).Action = ActionTypes.tComplete Then
                                    dblField6 = Me.Field6
                                End If
                            Next

                            If bolInsertPunch Then
                                ' Guardmaos el final de tarea con los atributos
                                oPunch2.Source = PunchSource.Request
                                If Not oPunch2.Save() Then
                                    bolRet = False
                                End If
                            End If

                            If bolRet Then
                                ' Creamos el inicio de la siguiente tarea
                                Dim oPunch3 As New Punch.roPunch(oPunchState)
                                oPunch3.IDEmployee = Me.IDEmployee
                                oPunch3.Type = PunchTypeEnum._TASK
                                oPunch3.ActualType = PunchTypeEnum._TASK
                                oPunch3.TypeData = Me.IDTask2
                                oPunch3.DateTime = Me.Date2
                                oPunch3.Source = PunchSource.Request
                                oPunch3.Field4 = -1
                                oPunch3.Field5 = -1
                                oPunch3.Field6 = -1

                                If Not oPunch3.Save() Then
                                    bolRet = False
                                End If

                                If bolRet Then
                                    ' Asignamos los campos a la tarea en caso necesario
                                    Dim oStateTask As New Task.roTaskState
                                    roBusinessState.CopyTo(Me.oState, oStateTask)

                                    bolRet = Task.roTask.SaveTaskFieldsFromPunch(Me.IDTask1, strField1, strField2, strField3, dblField4, dblField5, dblField6, oStateTask)
                                End If

                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest::UpdateForbiddenTaskPunch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::UpdateForbiddenTaskPunch")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function UpdateForgottenCenterPunch(ByVal _ApproveRefuse As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.RequestType = eRequestType.ForgottenCostCenterPunch Then

                    Dim oPunchState As New Punch.roPunchState
                    roBusinessState.CopyTo(Me.oState, oPunchState)
                    Dim tbPunches As DataTable = Punch.roPunch.GetPunches("IDEmployee = " & Me.IDEmployee.ToString & " AND " &
                                                                     "(DateTime = " & Any2Time(Me.Date1).SQLDateTime & " AND ActualType IN(" & PunchTypeEnum._CENTER & ")" & ")", oPunchState)

                    If tbPunches IsNot Nothing Then
                        If tbPunches.Rows.Count = 1 Then
                            If _ApproveRefuse Then
                                ' Marcar el fichaje cómo fiable y guardar
                                If Not IsDBNull(tbPunches.Rows(0)("DateTime")) AndAlso CDate(tbPunches.Rows(0)("DateTime")) = Me.Date1 Then
                                    tbPunches.Rows(0)("IsNotReliable") = False
                                End If
                            Else
                                ' Eliminamos el fichaje de la tabla Punches
                                If Not IsDBNull(tbPunches.Rows(0)("DateTime")) AndAlso CDate(tbPunches.Rows(0)("DateTime")) = Me.Date1 Then
                                    tbPunches.Rows(0).Delete()
                                End If
                            End If
                            Dim oPunches As New Punch.roPunchList(oPunchState)
                            bolRet = oPunches.Save(tbPunches, False, )
                            If Not bolRet Then
                                roBusinessState.CopyTo(oPunchState, Me.oState)
                                Me.oState.Result = RequestResultEnum.ForgottenCostCenterPunchError
                                Me.oState.ErrorText &= " " & oPunchState.ErrorText
                            End If
                        ElseIf tbPunches.Rows.Count > 0 Then
                            ' Si existen varios movimientos relacionados devolvemos error
                            Me.oState.Result = RequestResultEnum.RequestMoveTooMany
                            bolRet = False
                        Else
                            ' Si no existe devolvemos error
                            Me.oState.Result = RequestResultEnum.RequestMoveNotExist
                            bolRet = False
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest::UpdateForgottenCenterPunch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::UpdateForgottenCenterPunch")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function UpdateForbiddenExternalWorkWeekResume(ByVal _ApproveRefuse As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Try

                If Me.RequestType = eRequestType.ExternalWorkWeekResume Then

                    For Each oRequestDay As roRequestDay In Me.RequestDays

                        Dim oPunchState As New Punch.roPunchState
                        roBusinessState.CopyTo(Me.oState, oPunchState)
                        Dim tbPunches As DataTable = Punch.roPunch.GetPunches("IDEmployee = " & Me.IDEmployee.ToString & " AND " &
                                                                     "(DateTime = " & Any2Time(oRequestDay.RequestDate).SQLDateTime & " AND ActualType IN(" & PunchTypeEnum._IN & "," & PunchTypeEnum._OUT & ")" & ")",
                                                                     oPunchState)
                        If tbPunches IsNot Nothing Then
                            If tbPunches.Rows.Count = 1 Then
                                If _ApproveRefuse Then
                                    ' Marcar el fichaje cómo fiable y guardar
                                    If Not IsDBNull(tbPunches.Rows(0)("DateTime")) AndAlso CDate(tbPunches.Rows(0)("DateTime")) = oRequestDay.RequestDate Then
                                        tbPunches.Rows(0)("IsNotReliable") = False
                                    End If
                                Else
                                    ' Eliminamos el fichaje de la tabla Punches
                                    If Not IsDBNull(tbPunches.Rows(0)("DateTime")) AndAlso CDate(tbPunches.Rows(0)("DateTime")) = oRequestDay.RequestDate Then
                                        tbPunches.Rows(0).Delete()
                                    End If
                                End If
                                Dim oPunches As New Punch.roPunchList(oPunchState)
                                bolRet = oPunches.Save(tbPunches, False, )
                                If Not bolRet Then
                                    roBusinessState.CopyTo(oPunchState, Me.oState)
                                    Me.oState.Result = RequestResultEnum.ExternalWorkWeekResumeError
                                    Me.oState.ErrorText &= " " & oPunchState.ErrorText
                                ElseIf Not _ApproveRefuse Then
                                    ' Si se ha podido borrar el fichaje, reordenamos los fichajes para que no queden descuadrados
                                    bolRet = Punch.roPunch.ReorderPunches(Me.intIDEmployee, oRequestDay.RequestDate.Date, Me.oState)
                                    If Not bolRet Then
                                        Me.oState.Result = RequestResultEnum.ExternalWorkWeekResumeError
                                        Me.oState.ErrorText &= " " & oPunchState.ErrorText
                                    End If
                                ElseIf _ApproveRefuse Then
                                    ' En el caso de aprobar la solicitud guardamos los comentarios en las notas del dia
                                    If oRequestDay.Comments.Length > 0 Then
                                        Dim oShiftState As New Shift.roShiftState
                                        roBusinessState.CopyTo(Me.oState, oShiftState)

                                        Dim oSchedulerDay As New Shift.roDailySchedule(Me.intIDEmployee, Any2Time(oRequestDay.RequestDate).DateOnly, oShiftState)
                                        If oSchedulerDay.Remarks IsNot Nothing Then
                                            oSchedulerDay.Remarks.Text = Any2String(Any2String(oSchedulerDay.Remarks.Text) & " " & oRequestDay.Comments).Trim
                                        End If
                                        bolRet = oSchedulerDay.Save()
                                        If Not bolRet Then
                                            roBusinessState.CopyTo(oShiftState, Me.oState)
                                            Me.oState.Result = RequestResultEnum.ExternalWorkWeekResumeError
                                            Me.oState.ErrorText &= " " & oShiftState.ErrorText
                                            Exit For

                                        End If
                                    End If
                                End If
                            ElseIf tbPunches.Rows.Count > 0 Then
                                ' Si existen varios movimientos relacionados devolvemos error
                                Me.oState.Result = RequestResultEnum.RequestMoveTooMany
                                bolRet = False
                            Else
                                ' Si no existe devolvemos error
                                Me.oState.Result = RequestResultEnum.RequestMoveNotExist
                                bolRet = False
                            End If
                        End If

                        If Not bolRet Then Exit For
                    Next

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest::UpdateForbiddenExternalWorkWeekResume")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::UpdateForbiddenExternalWorkWeekResume")
            Finally

            End Try

            Return bolRet
        End Function

        Private Function UpdateForbiddenPunch(ByVal _ApproveRefuse As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.RequestType = eRequestType.ForbiddenPunch Then

                    Dim oPunchState As New Punch.roPunchState
                    roBusinessState.CopyTo(Me.oState, oPunchState)
                    Dim tbPunches As DataTable = Punch.roPunch.GetPunches("IDEmployee = " & Me.IDEmployee.ToString & " AND " &
                                                                     "(DateTime = " & Any2Time(Me.Date1).SQLDateTime & " AND ActualType IN(" & PunchTypeEnum._IN & "," & PunchTypeEnum._OUT & ")" & ")",
                                                                     oPunchState)
                    If tbPunches IsNot Nothing Then
                        If tbPunches.Rows.Count = 1 Then
                            If _ApproveRefuse Then
                                ' Marcar el fichaje cómo fiable y guardar
                                If Not IsDBNull(tbPunches.Rows(0)("DateTime")) AndAlso CDate(tbPunches.Rows(0)("DateTime")) = Me.Date1 Then
                                    tbPunches.Rows(0)("IsNotReliable") = False
                                End If
                            Else
                                ' Eliminamos el fichaje de la tabla Punches
                                If Not IsDBNull(tbPunches.Rows(0)("DateTime")) AndAlso CDate(tbPunches.Rows(0)("DateTime")) = Me.Date1 Then
                                    tbPunches.Rows(0).Delete()
                                End If
                            End If
                            Dim oPunches As New Punch.roPunchList(oPunchState)
                            bolRet = oPunches.Save(tbPunches, False, )
                            If Not bolRet Then
                                roBusinessState.CopyTo(oPunchState, Me.oState)
                                Me.oState.Result = RequestResultEnum.ForbiddenPunchError
                                Me.oState.ErrorText &= " " & oPunchState.ErrorText
                            ElseIf Not _ApproveRefuse Then
                                ' Si se ha podido borrar el fichaje, reordenamos los fichajes para que no queden descuadrados
                                bolRet = Punch.roPunch.ReorderPunches(Me.intIDEmployee, Me.Date1.Value.Date, Me.oState)
                                If Not bolRet Then
                                    Me.oState.Result = RequestResultEnum.ForbiddenPunchError
                                    Me.oState.ErrorText &= " " & oPunchState.ErrorText
                                End If
                            End If
                        ElseIf tbPunches.Rows.Count > 0 Then
                            ' Si existen varios movimientos relacionados devolvemos error
                            Me.oState.Result = RequestResultEnum.RequestMoveTooMany
                            bolRet = False
                        Else
                            ' Si no existe devolvemos error
                            Me.oState.Result = RequestResultEnum.RequestMoveNotExist
                            bolRet = False
                        End If

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest::UpdateForbiddenPunch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::UpdateForbiddenPunch")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function UpdateDailyRecordPunches(ByVal _ApproveRefuse As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.RequestType = eRequestType.DailyRecord AndAlso Me.ID > 0 Then

                    Dim oPunchState As New Punch.roPunchState
                    roBusinessState.CopyTo(Me.oState, oPunchState)
                    Dim tbPunches As DataTable = Punch.roPunch.GetPunches("IDEmployee = " & Me.IDEmployee.ToString & " AND IdRequest = " & Me.ID.ToString, oPunchState)
                    If tbPunches IsNot Nothing Then
                        If tbPunches.Rows.Count > 0 Then
                            If _ApproveRefuse Then
                                ' Marcamos fichajes como fiables
                                For i As Integer = 0 To tbPunches.Rows.Count - 1
                                    tbPunches.Rows(i)("IsNotReliable") = False
                                Next
                            Else
                                ' Eliminamos el fichaje de la tabla Punches
                                For i As Integer = 0 To tbPunches.Rows.Count - 1
                                    tbPunches.Rows(i).Delete()
                                Next
                            End If
                            Dim oPunches As New Punch.roPunchList(oPunchState)
                            bolRet = oPunches.Save(tbPunches, False,, False, Not _ApproveRefuse)
                            If Not bolRet Then
                                roBusinessState.CopyTo(oPunchState, Me.oState)
                                Me.oState.Result = RequestResultEnum.DailyRecordPunchesError
                                Me.oState.ErrorText &= " " & oPunchState.ErrorText
                            Else
                                If Not _ApproveRefuse Then
                                    ' Solo en el caso que eliminamos los fichajes, recalculamos el dia
                                    Dim oContext As New roCollection
                                    oContext.Add("User.ID", Me.intIDEmployee)
                                    roConnector.InitTask(TasksType.MOVES, oContext)

                                End If
                            End If
                        Else
                            ' Si no existe devolvemos error
                            Me.oState.Result = RequestResultEnum.NoPunchesForDailyRecord
                            bolRet = False
                        End If
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest::UpdateDailyRecordPunches")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::UpdateDailyRecordPunches")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetRequestNextSupervisorsNames(ByVal _IDRequest As Integer, ByVal _State As roRequestState, Optional ByVal bolAudit As Boolean = False) As String

            Dim retSupervisorNames As String = String.Empty

            Try
                Dim strSQL As String = $"@SELECT# STRING_AGG(sysroPassports.Name, ', ') WITHIN GROUP (ORDER BY sysroPassports.Name) AS SupervisorName
                                        FROM sysrovwSecurity_PendingRequestsDependencies
                                        INNER JOIN sysroPassports On sysroPassports.Id = sysrovwSecurity_PendingRequestsDependencies.IdPassport
                                        WHERE IdRequest = @idrequest AND DirectDependence = 1 AND AutomaticValidation = 0 AND IsRoboticsUser = 0"
                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idrequest", CommandParameter.ParameterType.tInt, _IDRequest))

                If DataLayer.AccessHelper.GetDatabaseCompatibilityLevel = 120 Then
                    strSQL &= SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetRequestNextSupervisorsNames)
                End If

                retSupervisorNames = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar(strSQL, parameters))

            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roRequest::Load")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequest::Load")
            End Try

            Return retSupervisorNames
        End Function


        ''' <summary>
        ''' Devuelve un Datatable con todos las solicitudes de un solo empleado
        ''' </summary>
        ''' <param name="_IDEmployee">ID de empleado a recuperar las solicitudes</param>
        ''' <param name="_SQLFilter">Filtro SQL para el Where (ejemplo: 'RequestType = 1 And Reque...')</param>
        ''' <param name="_SQLOrder">Ordenación SQL (ejemplo: 'RequestType ASC' o 'RequestDate ASC')</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRequestsByEmployee(ByVal _IDEmployee As Integer, ByVal _SQLFilter As String, ByVal _SQLOrder As String, ByVal _State As roRequestState, Optional ByVal _bolShowNextSupervisor As Boolean = False, Optional ByVal oLng As roLanguage = Nothing) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                ' Obtenemos el passport asociado al empleado
                Dim oPassport As roPassportTicket = Nothing
                Try
                    oPassport = roPassportManager.GetPassportTicket(_IDEmployee, LoadType.Employee, )
                Catch ex As Exception
                End Try

                ' Obtenemos los tipos de solicitudes a los que tiene permisos de lectura el empleado
                Dim bHasExchangeShiftPermission As Boolean = False
                Dim strFilterPermissions As String = ""
                If oPassport IsNot Nothing Then
                    Dim oRequestTypeSecurity As roRequestTypeSecurity = Nothing
                    For Each oRequestType As eRequestType In System.Enum.GetValues(GetType(eRequestType))
                        oRequestTypeSecurity = New roRequestTypeSecurity(oRequestType, _State)
                        If Security.WLHelper.GetPermissionOverFeature(oPassport.ID, oRequestTypeSecurity.EmployeeFeatureName, "E") >= Permission.Read Then
                            strFilterPermissions &= "," & Val(oRequestType)
                            If oRequestType = eRequestType.ExchangeShiftBetweenEmployees Then bHasExchangeShiftPermission = True
                        End If
                    Next
                End If
                If strFilterPermissions <> "" Then
                    strFilterPermissions = "(RequestType IN (" & strFilterPermissions.Substring(1) & ") AND IDEmployee = " & _IDEmployee & ")"
                Else
                    strFilterPermissions = "IDEmployee = " & _IDEmployee
                End If

                If bHasExchangeShiftPermission Then
                    strFilterPermissions = "(" & strFilterPermissions & " OR " & "(RequestType = 8 and IdEmployeeExchange = " & _IDEmployee & "))"
                End If

                Dim strSQL As String = "@SELECT# *, '' As RequestDetailInfo from Requests Where 1=1"
                If _SQLFilter <> "" Then strSQL &= " AND (" & _SQLFilter
                If strFilterPermissions <> "" Then strSQL &= " AND " & strFilterPermissions
                If _SQLFilter <> "" Then
                    strSQL &= ")"
                End If
                If _SQLOrder <> "" Then
                    strSQL &= " ORDER BY " & _SQLOrder
                Else
                    strSQL &= " ORDER BY RequestDate DESC"
                End If

                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then
                    For Each oRow As DataRow In oRet.Rows
                        oRow("RequestDetailInfo") = GetRequestInfo(False, oRow, _State,, _bolShowNextSupervisor, oLng)
                    Next

                    oRet.AcceptChanges()
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequest::GetRequests")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequest::GetRequests")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve un Datatable con todos las solicitudes a las que tiene acceso un supervisor o validador
        ''' </summary>
        ''' <param name="_IDPassport">ID de passaporte del usuario supervisor</param>
        ''' <param name="_SQLFilter">Filtro SQL para el Where (ejemplo: 'RequestType = 1 And Reque...')</param>
        ''' <param name="_SQLOrder">Ordenación SQL (ejemplo: 'RequestType ASC' o 'RequestDate ASC')</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="_Audit">Auditar consulta masiva</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRequestsSupervisor(ByVal _IDPassport As Integer, ByVal _SQLFilter As String, ByVal _SQLOrder As String, ByVal NumRequestToLoad As Integer, ByVal LevelsBelow As String,
                                                     ByVal IdCause As Integer, ByVal IdSupervisor As Integer, ByVal bIncludeAutomaticRequests As Boolean, ByVal _State As roRequestState, Optional ByVal _Audit As Boolean = True,
                                                     Optional ByVal needLevelsBelow As Boolean = False, Optional ByVal canSeeRequestsWithReadPermission As Boolean = False, Optional ByVal LoadDetail As Boolean = True) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Dim bolApplyAPV As Boolean = False
            Dim strSQLAPV As String = ""
            Dim strOrderBy As String = ""

            Try
                ' si existe la customizacion de APV
                ' debemos obtener el horario utilizado en date1
                ' y ordenar siempre previamente por : fecha solicitada(date1) y horario planificado en esa fecha(date1)
                Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState())
                If roTypes.Any2String(oAdvParam.Value).ToUpper = "VPA" Then
                    bolApplyAPV = True
                    strSQLAPV = " , (@SELECT# isnull(Name, '????') FROM Shifts WHERE ID IN (@SELECT# isnull(IDShift1,0) FROM DailySchedule WHERE DailySchedule.IDEmployee=Requests.IDEmployee AND DailySchedule.Date = Requests.Date1)) as ShiftNameDate1 "
                End If

                Dim strSQL As String = $"@SELECT# {If(NumRequestToLoad > 0, "TOP " & NumRequestToLoad, "")}"

                If LevelsBelow <> String.Empty Then

                    strSQL &= " Requests.*, ceg.IDEmployee, ceg.EmployeeName, ceg.GroupName, prd.NextlevelOfAuthorityRequired, " &
                    "(@SELECT# TOP 1 lrp.DateTime FROM RequestsApprovals lrp With (nolock) WHERE lrp.IDRequest = prd.IdRequest ORDER BY DateTime DESC) As 'LastRequestApprovalDate', " &
                    "(@SELECT# TOP 1 lrp.IDPassport FROM RequestsApprovals lrp with (nolock) WHERE lrp.IDRequest = prd.IdRequest ORDER BY DateTime DESC) AS 'LastRequestApprovalIDPassport', " &
                    "(@SELECT# TOP 1 sysroPassports.Name FROM RequestsApprovals lrp with (nolock) INNER JOIN sysroPassports ON lrp.IDPassport = sysroPassports.ID WHERE " &
                    "lrp.IDRequest = prd.IdRequest ORDER BY DateTime DESC) AS 'LastRequestApprovalPassportName', " &
                    "'' AS 'RequestInfo' "

                    If bolApplyAPV AndAlso strSQLAPV.Length > 0 Then strSQL &= strSQLAPV

                    strSQL &= " From sysrovwSecurity_PendingRequestsDependencies prd with (nolock) " &
                          " inner join Requests With (nolock) On prd.IdRequest = Requests.id " &
                          " inner join sysrovwCurrentEmployeeGroups ceg with (nolock) on prd.IdEmployee = ceg.IDEmployee "

                    If IdSupervisor > 0 Then
                        strSQL &= " INNER JOIN RequestsApprovals ra with (nolock) on Requests.ID = ra.IDRequest AND ra.IDPassport = " & IdSupervisor & " "
                    End If

                    strSQL &= " WHERE prd.IdPassport = " & _IDPassport & " AND prd.Permission " & IIf(canSeeRequestsWithReadPermission, ">=", ">") & " 3 "

                    If LevelsBelow = "1" Then
                        strSQL &= " AND prd.DirectDependence = 1  "

                    ElseIf LevelsBelow = "gt1" Then
                        strSQL &= " AND prd.DirectDependence = 0  "
                    End If
                Else
                    strSQL &= " Requests.*, ceg.IDEmployee, ceg.EmployeeName, ceg.GroupName, 0 AS NextlevelOfAuthorityRequired, " &
                    "(@SELECT# TOP 1 lrp.DateTime FROM RequestsApprovals lrp With (nolock) WHERE lrp.IDRequest = prd.IdRequest ORDER BY DateTime DESC) As 'LastRequestApprovalDate', " &
                    "(@SELECT# TOP 1 lrp.IDPassport FROM RequestsApprovals lrp with (nolock) WHERE lrp.IDRequest = prd.IdRequest ORDER BY DateTime DESC) AS 'LastRequestApprovalIDPassport', " &
                    "(@SELECT# TOP 1 sysroPassports.Name FROM RequestsApprovals lrp with (nolock) INNER JOIN sysroPassports ON lrp.IDPassport = sysroPassports.ID WHERE " &
                    "lrp.IDRequest = prd.IdRequest ORDER BY DateTime DESC) AS 'LastRequestApprovalPassportName', " &
                    "'' AS 'RequestInfo' "

                    If bolApplyAPV AndAlso strSQLAPV.Length > 0 Then strSQL &= strSQLAPV

                    strSQL &= " From sysrovwSecurity_PermissionOverRequests prd with (nolock) " &
                          " inner join Requests With (nolock) On prd.IdRequest = Requests.id " &
                          " inner join sysrovwCurrentEmployeeGroups ceg with (nolock) on prd.IdEmployee = ceg.IDEmployee "

                    strSQL &= " WHERE prd.IdPassport = " & _IDPassport & " AND prd.Permission " & IIf(canSeeRequestsWithReadPermission, ">=", ">") & " 3 "
                End If

                If IdCause > 0 Then strSQL &= " AND IsNull(Requests.IdCause,0) = " & IdCause & " "
                If _SQLFilter <> String.Empty Then strSQL &= " AND " & _SQLFilter
                If Not bIncludeAutomaticRequests Then strSQL &= " AND prd.AutomaticValidation = 0 "

                If IdSupervisor > 0 Then
                    Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(IdSupervisor, LoadType.Passport, New roSecurityState(_State.IDPassport, _State.Context))

                    If oPassport.ID = roConstants.GetSystemUserId() Then
                        strSQL = strSQL & " And (Requests.AutomaticValidation = 1"
                        strSQL = strSQL & " OR (@SELECT# TOP 1 RequestsApprovals.IDPassport FROM RequestsApprovals WHERE RequestsApprovals.IDRequest = Requests.ID ORDER BY DateTime DESC)=" & IdSupervisor & ") "
                    Else
                        strSQL = strSQL & " and (@SELECT# TOP 1 RequestsApprovals.IDPassport FROM RequestsApprovals WHERE RequestsApprovals.IDRequest = Requests.ID ORDER BY DateTime DESC)=" & IdSupervisor & " "
                    End If
                End If

                strOrderBy = $" ORDER BY {If(bolApplyAPV, " Date1 ASC, ShiftNameDate1 ASC ,", "")}{If(_SQLOrder <> String.Empty, _SQLOrder, "  RequestDate ASC")}"
                strSQL &= strOrderBy

                strSQL &= SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetRequestsSupervisor)

                oRet = CreateDataTableWithoutTimeouts(strSQL)

                Dim eResult = _State.Result

                If oRet IsNot Nothing AndAlso LoadDetail Then
                    For Each oRow As DataRow In oRet.Rows
                        oRow("RequestInfo") = GetRequestInfo(False, oRow, _State, bolApplyAPV)
                    Next
                    oRet.AcceptChanges()
                End If

                _State.Result = eResult

                If _Audit Then
                    ' Obtenemos la información del pasport asociado al supervisor
                    Dim oSecurityState As New roSecurityState(_State.IDPassport, _State.Context)
                    roBusinessState.CopyTo(_State, oSecurityState)
                    Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(_IDPassport, LoadType.Passport, oSecurityState)
                    roBusinessState.CopyTo(oSecurityState, _State)

                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Dim strAuditName As String = ""
                    For Each oRow As DataRow In oRet.Rows
                        strAuditName &= $"{IIf(strAuditName <> "", ",", "")}{oRow("ID")}"
                    Next
                    Extensions.roAudit.AddParameter(tbAuditParameters, "{IDRequests}", strAuditName, "", 1)
                    If oPassport IsNot Nothing Then
                        Extensions.roAudit.AddParameter(tbAuditParameters, "{SupervisorName}", oPassport.Name, "", 1)
                    End If
                    _State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tRequest, strAuditName & " (" & oPassport.Name & ")", tbAuditParameters, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequest::GetRequestsSupervisor")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequest::GetRequestsSupervisor")
            End Try

            Return oRet

        End Function

        Public Shared Function GetRequestsDashboardResume(ByVal _IDPassport As Integer, ByVal _State As roRequestState) As DataTable
            Dim oRet As DataTable = Nothing

            Try
                Dim strSQL As String = "if exists (@SELECT# distinct 1 from requests inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = Requests.IDEmployee and poe.IdPassport = " & _IDPassport & " and convert(date,getdate()) between poe.BeginDate and poe.EndDate " &
                                        " where Status in (0,1) and RequestDate between dateadd(d,-30,getdate()) And getdate()) " &
                         " @select# tmp.RequestType, COUNT(*) AS Total, 1 AS LevelsBelow from ( " &
                         " @select# req.RequestType, req.ID as IDRequest , prd.SupervisorLevelOfAuthority as PassportLevelOfAuthority " &
                         " FROM sysrovwSecurity_PendingRequestsDependencies prd  with (nolock) " &
                         "     INNER JOIN Requests req with (nolock) ON prd.IDRequest = req.ID " &
                         " WHERE prd.AutomaticValidation = 0 And prd.RequestCurrentStatus In(0, 1) and prd.DirectDependence = 1 and prd.idpassport = " & _IDPassport & " and prd.Permission > 3) tmp " &
                         " group by tmp.RequestType "

                strSQL &= SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetRequestsDashboardResume)

                oRet = CreateDataTableWithoutTimeouts(strSQL, )

            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequest::GetRequestsSupervisor")
            End Try

            Return oRet

        End Function

        Public Shared Function GetRequestTypes(ByVal strLanguageKey As String) As DataTable

            Dim tbRet As DataTable = Nothing

            Dim oLang As New roLanguage
            oLang.SetLanguageReference("LiveRequests", strLanguageKey)

            tbRet = New DataTable("RequestTypes")
            tbRet.Columns.Add(New DataColumn("ElementID", GetType(Integer)))
            tbRet.Columns.Add(New DataColumn("ElementName", GetType(String)))
            tbRet.Columns.Add(New DataColumn("ElementDesc", GetType(String)))

            Dim oRow As DataRow
            For Each oRequestType As eRequestType In System.Enum.GetValues(GetType(eRequestType))
                If oRequestType <> eRequestType.None Then
                    oRow = tbRet.NewRow
                    oRow("ElementID") = oRequestType
                    oRow("ElementName") = oRequestType.ToString
                    oRow("ElementDesc") = oLang.Translate("Requests.RequestType." & System.Enum.GetName(GetType(eRequestType), oRequestType), "Requests")
                    tbRet.Rows.Add(oRow)
                End If
            Next

            Return tbRet

        End Function

        Public Shared Function GetRequestStates(ByVal strLanguageKey As String) As DataTable

            Dim tbRet As DataTable = Nothing

            Dim oLang As New roLanguage
            oLang.SetLanguageReference("LiveRequests", strLanguageKey)

            tbRet = New DataTable("RequestStates")
            tbRet.Columns.Add(New DataColumn("ElementID", GetType(Integer)))
            tbRet.Columns.Add(New DataColumn("ElementName", GetType(String)))
            tbRet.Columns.Add(New DataColumn("ElementDesc", GetType(String)))

            Dim oRow As DataRow
            For Each oRequestState As eRequestStatus In System.Enum.GetValues(GetType(eRequestStatus))
                oRow = tbRet.NewRow
                oRow("ElementID") = oRequestState
                oRow("ElementName") = oRequestState.ToString
                oRow("ElementDesc") = oLang.Translate("Request.RequestState." & System.Enum.GetName(GetType(eRequestStatus), oRequestState), "Request")
                tbRet.Rows.Add(oRow)
            Next

            Return tbRet

        End Function

        Private Shared Function BuildDatesByPeriodString(ByVal idRequest As Integer, ByVal oLanguage As roLanguage, ByVal startdate As Date, ByVal endDate As Date)
            Dim strDatesPeriodString As String = ""

            Dim strSQLAux As Object
            strSQLAux = "@SELECT# Date FROM sysroRequestDays where IDRequest = " & idRequest
            Dim requestedDates As DataTable = CreateDataTable(strSQLAux, )
            Dim dates As List(Of Date) = requestedDates.Rows.Cast(Of DataRow).Select(Function(dr) Any2DateTime(dr(0))).ToList()
            dates.Sort()

            Dim groups As List(Of List(Of Date)) = New List(Of List(Of Date))
            Dim group1 As List(Of Date) = New List(Of Date)


            If Not dates.Any() Then
                startdate = startdate.Date
                endDate = endDate.Date

                Do While (startdate <= endDate)
                    dates.Add(startdate)
                    startdate = startdate.AddDays(1)
                Loop

            End If

            Dim lastDate As Date = dates(0)
            group1.Add(dates(0))
            groups.Add(group1)

            For i As Integer = 0 To dates.Count - 1
                Dim currDate As DateTime = dates(i)
                Dim timeDiff As TimeSpan = currDate - lastDate
                Dim isNewGroup As Boolean = timeDiff.Days > 1
                If isNewGroup Then
                    groups.Add(New List(Of Date))
                End If
                groups.Last().Add(currDate)
                lastDate = currDate
            Next

            If groups IsNot Nothing And groups.Count > 0 Then
                Dim groupIndex As Integer = 0
                For Each datePeriod As List(Of Date) In groups
                    If strDatesPeriodString Is Nothing Or strDatesPeriodString = "" Then
                        strDatesPeriodString = oLanguage.Translate("since", "RequestInfo") & "<b> " & Format(datePeriod.First(), oLanguage.GetShortDateFormat) & "</b> " & oLanguage.Translate("To", "RequestInfo") & "<b> " & Format(datePeriod.Last, oLanguage.GetShortDateFormat) & "</b>"
                    Else
                        If groupIndex = groups.Count - 1 Then
                            strDatesPeriodString = strDatesPeriodString & " " & oLanguage.Translate("andSince", "RequestInfo") & "<b> " & Format(datePeriod.First, oLanguage.GetShortDateFormat) & "</b> " & oLanguage.Translate("To", "RequestInfo") & "<b> " & Format(datePeriod.Last, oLanguage.GetShortDateFormat) & "</b>"
                        Else
                            strDatesPeriodString = strDatesPeriodString & " , " & oLanguage.Translate("since", "RequestInfo") & "<b> " & Format(datePeriod.First, oLanguage.GetShortDateFormat) & "</b> " & oLanguage.Translate("To", "RequestInfo") & " <b> " & Format(datePeriod.Last, oLanguage.GetShortDateFormat) & "</b>"
                        End If
                    End If
                    groupIndex = groupIndex + 1
                Next
            End If

            Return strDatesPeriodString
        End Function

        Private Shared Function GetRequestInfo(ByVal bolDetail As Boolean, ByRef oRow As DataRow, ByRef oState As roRequestState,
                                               Optional bolShowShiftNameDate1 As Boolean = False, Optional bolShowNextSupervisor As Boolean = False, Optional ByVal oLng As roLanguage = Nothing) As String

            Dim strRet As String = ""

            Try
                Dim oLanguage As roLanguage = Nothing
                Dim userCulture As CultureInfo = Nothing

                If oLng Is Nothing Then
                    Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(oState.IDPassport, LoadType.Passport)
                    Dim strCulture As String = "es-ES"
                    If oPassport IsNot Nothing Then
                        If Not String.IsNullOrEmpty(oPassport.Language.Culture) Then
                            userCulture = New CultureInfo(oPassport.Language.Culture)
                        Else
                            userCulture = New CultureInfo(strCulture)
                        End If
                    Else
                        userCulture = New CultureInfo(strCulture)
                    End If
                    oLanguage = oState.Language
                Else
                    userCulture = oLng.GetLanguageCulture
                    oLanguage = oLng
                End If

                Dim strMsg As String = "RequestInfo"
                If bolDetail Then strMsg &= ".Detail"

                Dim tmpDate1 As Nullable(Of Date)
                Dim tmpDate2 As Nullable(Of Date)

                If Not IsDBNull(oRow("Date1")) Then tmpDate1 = oRow("Date1")
                If Not IsDBNull(oRow("Date2")) Then tmpDate2 = oRow("Date2")

                Select Case CType(oRow("RequestType"), eRequestType)

                    Case eRequestType.UserFieldsChange

                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Any2String(oRow("FieldName")))
                        oLanguage.AddUserToken(Any2String(oRow("FieldValue")))
                        If Not tmpDate1.HasValue Then
                            strMsg &= ".UserFieldsChange"
                        Else
                            strMsg &= ".UserFieldsChange.History"
                            oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        End If
                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()

                    Case eRequestType.ForbiddenPunch
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortTimePattern))
                        If IsDBNull(oRow("IDCause")) Then
                            strMsg &= ".ForbiddenPunch"
                        Else
                            strMsg &= ".ForbiddenPunch.WithCause"
                            ' Obtenemos el nombre de la justificación
                            Dim oCauseState As New Cause.roCauseState
                            roBusinessState.CopyTo(oState, oCauseState)
                            Dim oCause As New Cause.roCause(Any2Integer(oRow("IDCause")), oCauseState, False)
                            oLanguage.AddUserToken(oCause.Name)
                        End If
                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()

                    Case eRequestType.JustifyPunch
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortTimePattern))
                        strMsg &= ".JustifyPunch"
                        ' Obtenemos el nombre de la justificación
                        Dim oCauseState As New Cause.roCauseState
                        roBusinessState.CopyTo(oState, oCauseState)
                        Dim oCause As New Cause.roCause(Any2Integer(oRow("IDCause")), oCauseState, False)
                        oLanguage.AddUserToken(oCause.Name)
                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()

                    Case eRequestType.ExternalWorkResumePart
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        oLanguage.AddUserToken(roConversions.ConvertHoursToTime(Any2Double(oRow("Hours"))))

                        If Not IsDBNull(oRow("IDCause")) Then
                            Dim oCauseState As New Cause.roCauseState
                            roBusinessState.CopyTo(oState, oCauseState)
                            Dim oCause As New Cause.roCause(Any2Integer(oRow("IDCause")), oCauseState, False)
                            oLanguage.AddUserToken(oCause.Name)
                        Else
                            oLanguage.AddUserToken(oLanguage.Translate("List.NoCause", ""))
                        End If

                        strMsg &= ".ExternalWorkResumePart"
                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()

                    Case eRequestType.ChangeShift
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        oLanguage.AddUserToken(Format(tmpDate2.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        ' Obtenemos el nombre del horario

                        Dim oShiftState As New Shift.roShiftState
                        roBusinessState.CopyTo(oState, oShiftState)
                        Dim oShift As New Shift.roShift(Any2Integer(oRow("IDShift")), oShiftState)
                        Dim strName As String = oShift.Name
                        If Not IsDBNull(oRow("StartShift")) Then strName = oShift.FloatingShiftName(oRow("StartShift"))
                        oLanguage.AddUserToken(strName)

                        If oRow("Field4") IsNot DBNull.Value AndAlso Any2Integer(Any2Double(oRow("Field4"))) > 0 Then
                            Dim oRepShift As New Shift.roShift(Any2Integer(Any2Double(oRow("Field4"))), oShiftState)
                            Dim strRepName As String = oRepShift.Name
                            oLanguage.AddUserToken(strRepName)

                            If tmpDate1 <> tmpDate2 Then
                                strMsg &= ".ChangeShift"
                            Else
                                strMsg &= ".ChangeShift.OneDate"
                            End If
                        Else
                            If tmpDate1 <> tmpDate2 Then
                                strMsg &= ".ChangeAllShift"
                            Else
                                strMsg &= ".ChangeAllShift.OneDate"
                            End If
                        End If

                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()

                    Case eRequestType.CancelHolidays

                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        oLanguage.AddUserToken(Format(tmpDate2.Value, userCulture.DateTimeFormat.ShortDatePattern))

                        Dim oRequest As New Requests.roRequest(oRow("ID"), oState)

                        If oRequest.lstRequestDays Is Nothing OrElse oRequest.lstRequestDays.Count = 0 Then
                            ' version v1
                            If tmpDate1 <> tmpDate2 Then
                                Dim intDays As Integer = 0
                                strMsg &= ".CancelHolidays"
                                intDays = roBusinessSupport.HolidayDaysInPeriod(oRow("IDEmployee"), tmpDate1, tmpDate2, oState)
                                oLanguage.AddUserToken(Math.Abs(intDays))
                            Else
                                strMsg &= ".CancelHolidays.OneDate"
                            End If
                            strRet = oState.Language.Translate(strMsg, "")
                            oLanguage.ClearUserTokens()
                        Else

                            If Any2Double(oRow("IDShift")) > 0 Then
                                ' Cancelamos dias de vacaciones
                                Dim oShiftState As New Shift.roShiftState
                                roBusinessState.CopyTo(oState, oShiftState)
                                Dim oRepShift As New Shift.roShift(oRow("IDShift"), oShiftState)
                                Dim strRepName As String = oRepShift.Name
                                oLanguage.AddUserToken(strRepName)
                                strMsg &= ".CancelHolidaysShiftCause"
                            End If

                            If Any2Double(oRow("IDCause")) > 0 Then
                                ' Cancelamos previsiones de vacaciones por horas
                                Dim oCauseState As New Cause.roCauseState
                                roBusinessState.CopyTo(oState, oCauseState)
                                Dim oRepCause As New Cause.roCause(oRow("IDCause"), oCauseState)
                                Dim strRepName As String = oRepCause.Name
                                oLanguage.AddUserToken(strRepName)
                                strMsg &= ".CancelHolidaysShiftCause"
                            End If

                            If oRequest.lstRequestDays.Count > 1 Then
                                Dim intDays As Integer = 0
                                strMsg &= ".CancelHolidays"
                                intDays = oRequest.lstRequestDays.Count
                                oLanguage.AddUserToken(Math.Abs(intDays))
                            Else
                                strMsg &= ".CancelHolidays.OneDate"
                            End If
                            strRet = oLanguage.Translate(strMsg, "")
                            oLanguage.ClearUserTokens()
                        End If

                    Case eRequestType.VacationsOrPermissions
                        oLanguage.ClearUserTokens()
                        Dim requestDatesByPeriod = BuildDatesByPeriodString(Any2Integer(oRow("ID")), oLanguage, Any2DateTime(oRow("Date1")), Any2DateTime(oRow("Date2")))
                        oLanguage.AddUserToken(requestDatesByPeriod)
                        ' Obtenemos el nombre del horario
                        Dim oShiftState As New Shift.roShiftState
                        roBusinessState.CopyTo(oState, oShiftState)
                        Dim oShift As New Shift.roShift(Any2Integer(oRow("IDShift")), oShiftState)
                        Dim strName As String = oShift.Name
                        If Not IsDBNull(oRow("StartShift")) Then strName = oShift.FloatingShiftName(oRow("StartShift"))
                        oLanguage.AddUserToken(strName)

                        Dim oRequest As New Requests.roRequest(oRow("ID"), oState)

                        If oRequest.lstRequestDays Is Nothing OrElse oRequest.lstRequestDays.Count = 0 Then
                            ' version v1
                            If tmpDate1.Value <> tmpDate2.Value Then
                                Dim intDays As Integer = 0
                                strMsg &= ".VacationsOrPermissions"
                                If oShift.AreWorkingDays Then
                                    intDays = roBusinessSupport.LaboralDaysInPeriod(oRow("IDEmployee"), tmpDate1.Value.Date, tmpDate2.Value.Date, oState)
                                Else
                                    intDays = Math.Abs(DateDiff(DateInterval.Day, tmpDate1.Value.Date, tmpDate2.Value.Date)) + 1
                                End If
                                oLanguage.AddUserToken(Math.Abs(intDays))
                            Else
                                strMsg &= ".VacationsOrPermissions.OneDate"
                            End If
                            strRet = oState.Language.Translate(strMsg, "")
                            oLanguage.ClearUserTokens()
                        Else
                            If oRequest.lstRequestDays.Count > 1 Then
                                Dim intDays As Integer = 0
                                strMsg &= ".VacationsOrPermissions"
                                intDays = oRequest.lstRequestDays.Count
                                oLanguage.AddUserToken(Math.Abs(intDays))
                            Else
                                strMsg &= ".VacationsOrPermissions.OneDate"
                            End If

                            strRet = oLanguage.Translate(strMsg, "")
                            oLanguage.ClearUserTokens()
                        End If

                    Case eRequestType.PlannedAbsences
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        oLanguage.AddUserToken(Format(tmpDate2.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        ' Obtenemos el nombre de la justificación
                        Dim oCauseState As New Cause.roCauseState
                        roBusinessState.CopyTo(oState, oCauseState)
                        Dim oCause As New Cause.roCause(Any2Integer(oRow("IDCause")), oCauseState, False)
                        oLanguage.AddUserToken(oCause.Name)
                        strMsg &= ".PlannedAbsences"
                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()

                    Case eRequestType.PlannedCauses
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        If tmpDate2.HasValue Then
                            oLanguage.AddUserToken(Format(tmpDate2.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        Else
                            oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        End If

                        ' Obtenemos el nombre de la justificación
                        Dim oCauseState As New Cause.roCauseState
                        roBusinessState.CopyTo(oState, oCauseState)
                        Dim oCause As New Cause.roCause(Any2Integer(oRow("IDCause")), oCauseState, False)

                        Dim tmpFromTime As Nullable(Of Date)
                        Dim tmpToTime As Nullable(Of Date)
                        If Not IsDBNull(oRow("FromTime")) Then tmpFromTime = oRow("FromTime")
                        If Not IsDBNull(oRow("ToTime")) Then tmpToTime = oRow("ToTime")

                        oLanguage.AddUserToken(oCause.Name)
                        oLanguage.AddUserToken(roConversions.ConvertHoursToTime(Any2Double(oRow("Hours"))))
                        oLanguage.AddUserToken(tmpFromTime.Value.ToShortTimeString)
                        oLanguage.AddUserToken(tmpToTime.Value.ToShortTimeString)
                        strMsg &= ".PlannedCauses"
                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()

                    Case eRequestType.PlannedOvertimes

                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        If tmpDate2.HasValue Then
                            oLanguage.AddUserToken(Format(tmpDate2.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        Else
                            oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        End If

                        ' Obtenemos el nombre de la justificación
                        Dim oCauseState As New Cause.roCauseState
                        roBusinessState.CopyTo(oState, oCauseState)
                        Dim oCause As New Cause.roCause(Any2Integer(oRow("IDCause")), oCauseState, False)

                        Dim tmpFromTime As Nullable(Of Date)
                        Dim tmpToTime As Nullable(Of Date)
                        If Not IsDBNull(oRow("FromTime")) Then tmpFromTime = oRow("FromTime")
                        If Not IsDBNull(oRow("ToTime")) Then tmpToTime = oRow("ToTime")

                        oLanguage.AddUserToken(oCause.Name)
                        oLanguage.AddUserToken(roConversions.ConvertHoursToTime(Any2Double(oRow("Hours"))))
                        oLanguage.AddUserToken(tmpFromTime.Value.ToShortTimeString)
                        oLanguage.AddUserToken(tmpToTime.Value.ToShortTimeString)
                        strMsg &= ".PlannedOvertimes"
                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()

                    Case eRequestType.ExternalWorkWeekResume
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        If tmpDate2.HasValue Then
                            oLanguage.AddUserToken(Format(tmpDate2.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        Else
                            oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        End If

                        Dim oRequest As New Requests.roRequest(oRow("ID"), oState)

                        Dim intDays As Integer = 0
                        strMsg &= ".ExternalWorkWeekResume"
                        intDays = oRequest.lstRequestDays.Count
                        oLanguage.AddUserToken(Math.Abs(intDays))

                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()

                    Case eRequestType.PlannedHolidays

                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        If tmpDate2.HasValue Then
                            oLanguage.AddUserToken(Format(tmpDate2.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        Else
                            oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        End If
                        ' Obtenemos el nombre de la justificación
                        Dim oCauseState As New Cause.roCauseState
                        roBusinessState.CopyTo(oState, oCauseState)
                        Dim oCause As New Cause.roCause(oRow("IDCause"), oCauseState, False)
                        oLanguage.AddUserToken(oCause.Name)

                        Dim oRequest As New Requests.roRequest(oRow("ID"), oState)

                        If oRequest.lstRequestDays.Count > 1 Then
                            Dim intDays As Integer = 0
                            strMsg &= ".PlannedHolidays"
                            intDays = oRequest.lstRequestDays.Count
                            oLanguage.AddUserToken(Math.Abs(intDays))
                        Else
                            strMsg &= ".PlannedHolidays.OneDate"
                        End If

                        If oRequest.lstRequestDays(0).AllDay Then
                            strMsg &= ".AllDay"
                        Else
                            strMsg &= ".Period"
                            oLanguage.AddUserToken(oRequest.lstRequestDays(0).FromTime.Value.ToShortTimeString)
                            oLanguage.AddUserToken(oRequest.lstRequestDays(0).ToTime.Value.ToShortTimeString)
                            oLanguage.AddUserToken(roConversions.ConvertHoursToTime(oRequest.lstRequestDays(0).Duration.Value))

                        End If
                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()

                    Case eRequestType.ExchangeShiftBetweenEmployees
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(roTypes.Any2DateTime(oRow("Date1")).ToString(oState.Language.GetShortDateFormat))

                        oLanguage.AddUserToken(roBusinessSupport.GetEmployeeName(roTypes.Any2Integer(oRow("IDEmployee")), oState))
                        oLanguage.AddUserToken(roBusinessSupport.GetEmployeeName(roTypes.Any2Double(oRow("IDEmployeeExchange")), oState))

                        oLanguage.AddUserToken(Robotics.Base.VTBusiness.Shift.roShift.GetName(roTypes.Any2Integer(oRow("IDShift")), New Shift.roShiftState()))
                        oLanguage.AddUserToken(Robotics.Base.VTBusiness.Shift.roShift.GetName(roTypes.Any2Double(oRow("Field4")), New Shift.roShiftState()))

                        If IsDBNull(oRow("Date2")) = False Then
                            ' Hay compensación
                            oLanguage.AddUserToken(roTypes.Any2DateTime(oRow("Date2")).ToString(oState.Language.GetShortDateFormat))

                            ' Busco id's de horarios del día de compensación
                            Dim idRequestedCompensationIdShift As Integer = -1
                            Dim idApplicantCompensationIdShift As Integer = -1

                            Dim strSQLAux As String = String.Empty

                            strSQLAux = "@SELECT# isnull(IDShiftUsed, Idshift1) from DailySchedule where date = " & roTypes.Any2Time(roTypes.Any2DateTime(oRow("Date2"))).SQLSmallDateTime & " and IDEmployee = " & roTypes.Any2Double(oRow("IDEmployeeExchange")).ToString
                            idRequestedCompensationIdShift = Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQLAux))

                            strSQLAux = "@SELECT# isnull(IDShiftUsed, Idshift1) from DailySchedule where date = " & roTypes.Any2Time(roTypes.Any2DateTime(oRow("Date2"))).SQLSmallDateTime & " and IDEmployee = " & roTypes.Any2Integer(oRow("IDEmployee")).ToString
                            idApplicantCompensationIdShift = Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQLAux))

                            oLanguage.AddUserToken(Robotics.Base.VTBusiness.Shift.roShift.GetName(idRequestedCompensationIdShift, New Shift.roShiftState()))
                            oLanguage.AddUserToken(Robotics.Base.VTBusiness.Shift.roShift.GetName(idApplicantCompensationIdShift, New Shift.roShiftState()))

                            strMsg &= ".ExchangeShiftBetweenEmployees.WithCompensation"
                        Else
                            strMsg &= ".ExchangeShiftBetweenEmployees"
                        End If

                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()

                        '

                    Case eRequestType.ForbiddenTaskPunch
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortTimePattern))
                        Dim oTaskState As New Task.roTaskState
                        roBusinessState.CopyTo(oState, oTaskState)

                        Dim straux As String = ""

                        ' Obtenemos el nombre de la tarea
                        Dim oTask As New Task.roTask(Any2Long(oRow("IDTask1")), oTaskState)
                        If oTask IsNot Nothing Then
                            oLanguage.AddUserToken(oTask.Project & " " & oTask.Name)
                        End If

                        If Not tmpDate2.HasValue Then
                            strMsg &= ".ForbiddenTaskPunch"
                        Else

                            If Any2Boolean(oRow("CompletedTask")) Then
                                strMsg &= ".ForbiddenTaskPunchWithEndExtendedCompleted"
                            Else
                                strMsg &= ".ForbiddenTaskPunchWithEndExtendedNotCompleted"
                            End If

                            oLanguage.AddUserToken(Format(tmpDate2.Value, userCulture.DateTimeFormat.ShortDatePattern))

                            oTask = New Task.roTask(Any2Long(oRow("IDTask2")), oTaskState)
                            If oTask IsNot Nothing Then
                                oLanguage.AddUserToken(oTask.Project & " " & oTask.Name)
                            Else
                                oLanguage.AddUserToken("")
                            End If

                            oLanguage.AddUserToken(Any2String(oRow("Field1")))
                            oLanguage.AddUserToken(Any2String(oRow("Field2")))
                            oLanguage.AddUserToken(Any2String(oRow("Field3")))
                            If Any2Double(oRow("Field4")) >= 0 Then
                                oLanguage.AddUserToken(Format(Any2Double(oRow("Field4")), "##0.00"))
                            Else
                                oLanguage.AddUserToken("")
                            End If

                            If Any2Double(oRow("Field5")) >= 0 Then
                                oLanguage.AddUserToken(Format(Any2Double(oRow("Field5")), "##0.00"))
                            Else
                                oLanguage.AddUserToken("")
                            End If
                            If Any2Double(oRow("Field6")) >= 0 Then
                                oLanguage.AddUserToken(Format(Any2Double(oRow("Field6")), "##0.00"))
                            Else
                                oLanguage.AddUserToken("")
                            End If

                        End If

                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()
                    Case eRequestType.ForgottenCostCenterPunch
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortTimePattern))

                        strMsg &= ".ForgottenCostCenterPunch"
                        ' Obtenemos el nombre de la justificación
                        Dim oCostCenterState As New BusinessCenter.roBusinessCenterState
                        roBusinessState.CopyTo(oState, oCostCenterState)
                        Dim oCostCenter As New BusinessCenter.roBusinessCenter(oRow("IDCenter"), oCostCenterState, False)
                        oLanguage.AddUserToken($" {oCostCenter.Name}")

                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()
                    Case eRequestType.Telecommute
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(Format(tmpDate1.Value, userCulture.DateTimeFormat.ShortDatePattern))
                        Dim location = ""
                        If oRow("Field1") = "1" Then
                            strMsg &= ".TelecommuteHome"
                        Else
                            strMsg &= ".TelecommuteOffice"
                        End If

                        strRet = oLanguage.Translate(strMsg, "")
                        oLanguage.ClearUserTokens()
                End Select

                If strRet.Length > 0 Then
                    ' En caso necesario añadimos la info de la fecha de validacion y rechazo automatico
                    If Any2Integer(oRow("Status")) = eRequestStatus.Pending Or Any2Integer(oRow("Status")) = eRequestStatus.OnGoing Then
                        If Not IsDBNull(oRow("ValidationDate")) AndAlso Not IsDBNull(oRow("AutomaticValidation")) AndAlso Any2Boolean(oRow("AutomaticValidation")) Then
                            oLanguage.ClearUserTokens()
                            strMsg = "RequestInfo.RequestStatus.ValidationDate"
                            oLanguage.AddUserToken(Format(CDate(oRow("ValidationDate")), userCulture.DateTimeFormat.ShortDatePattern))
                            strRet = strRet & ". " & oLanguage.Translate(strMsg, "")
                            oLanguage.ClearUserTokens()
                        End If

                        If Not IsDBNull(oRow("RejectedDate")) Then
                            oLanguage.ClearUserTokens()
                            strMsg = "RequestInfo.RequestStatus.RejectedDate"
                            oLanguage.AddUserToken(Format(CDate(oRow("RejectedDate")), userCulture.DateTimeFormat.ShortDatePattern))
                            strRet = strRet & ". " & oLanguage.Translate(strMsg, "")
                            oLanguage.ClearUserTokens()
                        End If
                    End If
                End If

                If bolShowShiftNameDate1 Then
                    ' En caso de tener que mostrar el nombre del horario planificado
                    Try
                        Dim strNameShift As String = "<b> *** ?????  *** </b>"
                        If Not IsDBNull(oRow("ShiftNameDate1")) Then
                            strNameShift = "<b> *** " & Any2String(oRow("ShiftNameDate1")).ToUpper & " *** </b>"
                        End If
                        strRet = strNameShift & strRet
                    Catch ex As Exception
                    End Try
                End If

                If bolShowNextSupervisor Then
                    ' En caso de tener que mostrar el siguiente supervisor
                    Try
                        If Any2Integer(oRow("Status")) = eRequestStatus.Pending Or Any2Integer(oRow("Status")) = eRequestStatus.OnGoing Then
                            Dim strNameSupervisor As String = GetRequestNextSupervisorsNames(oRow("ID"), oState)
                            oLanguage.ClearUserTokens()
                            strMsg = "Request.NextApprovalInfo"
                            oLanguage.AddUserToken(strNameSupervisor.Trim)
                            strRet = strRet & ". " & oLanguage.Translate(strMsg, "")
                            oLanguage.ClearUserTokens()

                        End If
                    Catch ex As Exception
                    End Try
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequest:: RequestInfo")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequest::RequestInfo")
            End Try

            Return strRet

        End Function

        Public Shared Function GetFilterRequests(ByVal IdPassport As Integer, ByRef _State As roRequestState) As String

            Dim strFilter As String = String.Empty

            Try

                Dim strSQL As String = "@SELECT# DataUser FROM sysroPassports WHERE ID = " & IdPassport
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oParameters As New roCollection(roTypes.Any2String(tb.Rows(0).Item("DataUser")))
                    strFilter = roTypes.Any2String(oParameters("RequestFilters"))
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequest::GetFilterRequests")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequest::GetFilterRequests")
            Finally

            End Try

            Return strFilter

        End Function

        Public Shared Function SetFilterRequests(ByVal IdPassport As Integer, ByVal Filter As String, ByRef _State As roRequestState) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Filter = "" Then

                    Dim strSQL As String = "@UPDATE# sysroPassports SET DataUser = '' WHERE ID = " & IdPassport
                    bolRet = ExecuteSql(strSQL)
                Else
                    Dim strSQL As String = "@SELECT# DataUser FROM sysroPassports WHERE ID = " & IdPassport
                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        Dim oParameters As New roCollection(roTypes.Any2String(tb.Rows(0).Item("DataUser")))
                        oParameters("RequestFilters") = Filter

                        strSQL = "@UPDATE# sysroPassports SET DataUser = '" & oParameters.XML() & "' WHERE ID = " & IdPassport
                        bolRet = ExecuteSql(strSQL)

                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequest::GetFilterRequests")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequest::GetFilterRequests")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function GetEmployeeRankingPositions(ByVal _IDEmployee As Integer, ByVal xDate As Date, ByRef oReqState As roRequestState) As roRequestEmployeeRanking

            Dim oRet As New roRequestEmployeeRanking
            Dim bolRet As Boolean = False
            Dim strEmployees As String = String.Empty
            Dim strSQL As String = ""

            Try


                ' Obtenemos el nodo del empleado
                ' utilizado para la IDN de APV, no modificar
                Dim _IDNode As Integer = roGroup.GetSecurityNodeFromGroupOrEmployee(New roGroupState(-1), _IDEmployee, False, xDate)

                ' Obtenemos los empleados del nodo
                ' utilizado para la IDN de APV, no modificar
                strEmployees = roBudgetManager.GetEmployeeListFromNode(_IDNode, New roBudgetState(-1), xDate, False)

                If strEmployees.Length = 0 Then strEmployees = "-1"
                strEmployees = "(" & strEmployees & ")"

                Dim intIDShift As Integer = Any2Integer(ExecuteScalar("@SELECT# isnull(idshift1, 0) from DailySchedule where idemployee=" & _IDEmployee.ToString & " AND Date =" & Any2Time(xDate).SQLSmallDateTime))

                ' Obtenemos todas las solicitudes
                ' para ese dia concreto
                ' que esten pendientes o en curso
                ' de los empleados del mismo centro productivo
                ' Ordenados por fecha de validacion y fecha en que se solicito
                ' y que tengan la regla de minimos de cobertura
                ' y que tenga planificado el mismo horario
                strSQL = "@SELECT# Requests.IDEmployee,(@SELECT# isnull(name,'') from employees where id= requests.idemployee) as EmployeeName, RequestType, Date1, IDShift, IDCause, RequestDate   " &
                        " FROM Requests WITH (NOLOCK) " &
                        " INNER JOIN EmployeeContracts WITH (NOLOCK)  " &
                        " ON Requests.IDEmployee = EmployeeContracts.IDEmployee " &
                    "WHERE Requests.IDEmployee IN " & strEmployees &
                         " AND Requests.Status in( " & eRequestStatus.Pending & "," & eRequestStatus.OnGoing & ")" &
                        " AND Requests.Date1 = " & roTypes.Any2Time(xDate).SQLSmallDateTime &
                        " AND (EmployeeContracts.EndDate >= " & roTypes.Any2Time(xDate).SQLSmallDateTime & " ) AND (EmployeeContracts.BeginDate <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ")  " &
                        " AND Requests.IDEmployee in(@SELECT# idemployee from dailyschedule where date=" & Any2Time(xDate).SQLSmallDateTime & " AND IDShift1=" & intIDShift.ToString & ") " &
                        " Order by isnull(Requests.ValidationDate, Requests.RequestDate) , Requests.RequestDate "

                Dim dTbl As System.Data.DataTable = CreateDataTableWithoutTimeouts(strSQL)

                Dim oRequestEmployeeRankingPositions As New Generic.List(Of roRequestEmployeeRankingPosition)
                'Cargar los datos de las posiciones
                Dim intPos As Integer = 0
                If dTbl IsNot Nothing Then
                    For Each dRow As DataRow In dTbl.Rows

                        ' Solo tener en cuenta las solicitudes que tengan una regla de minimo cobertura relacionada
                        Dim xRequest As New roRequest
                        xRequest.RequestType = dRow("RequestType")
                        xRequest.IDEmployee = dRow("IDEmployee")
                        xRequest.Date1 = dRow("Date1")
                        If Not IsDBNull(dRow("IDShift")) Then xRequest.IDShift = Any2Integer(dRow("IDShift"))
                        If Not IsDBNull(dRow("IDCause")) Then xRequest.IDCause = Any2Integer(dRow("IDCause"))

                        ' Y que no exista ya el empleado en la lista de rankings
                        Dim oRequestEmployeeRankingPosition As New roRequestEmployeeRankingPosition
                        oRequestEmployeeRankingPosition.IDEmployee = roTypes.Any2Integer(dRow("IDEmployee"))
                        If _IDEmployee = oRequestEmployeeRankingPosition.IDEmployee Then
                            oRequestEmployeeRankingPosition.EmployeeName = Any2DateTime(dRow("RequestDate")).ToString("dd/MM/yyyy HH:mm:ss") & " " & roTypes.Any2String(dRow("EmployeeName"))
                        Else
                            oRequestEmployeeRankingPosition.EmployeeName = Any2DateTime(dRow("RequestDate")).ToString("dd/MM/yyyy HH:mm:ss")
                        End If

                        If oRequestEmployeeRankingPositions.FindAll(Function(x) x.IDEmployee = oRequestEmployeeRankingPosition.IDEmployee).Count = 0 AndAlso roRequestRuleManager.IsRuleUsedOnRequest(xRequest, eRequestRuleType.MinCoverageRequiered, oReqState) Then
                            intPos = intPos + 1
                            oRequestEmployeeRankingPosition.IDPosition = intPos
                            oRequestEmployeeRankingPositions.Add(oRequestEmployeeRankingPosition)
                        End If
                    Next
                End If
                oRet.RequestEmployeeRankingPositions = oRequestEmployeeRankingPositions.ToArray

                bolRet = True
            Catch ex As DbException
                oReqState.UpdateStateInfo(ex, "roBudgetManager::GetEmployeeRankingPositions")
            Catch ex As Exception
                oReqState.UpdateStateInfo(ex, "roBudgetManager::GetEmployeeRankingPositions")
            End Try
            Return oRet
        End Function

        Public Shared Function GetCoverageOnEmployeeDate(ByVal _IDEmployee As Integer, ByVal xDate As Date, ByRef oReqState As roRequestState) As roRequestSummaryCoverageOnEmployeeDate
            ' obtenemos la cobertura mínima definida en un presupuesto
            ' a partir de la planificacion de un empleado en una fecha
            ' y los empleados planificados actualmente con el mismo horario/puesto que el empleado

            Dim oRet As New roRequestSummaryCoverageOnEmployeeDate
            Dim bolRet As Boolean = False
            Dim strEmployees As String = String.Empty
            Try

                ' Obtenemos el nodo del empleado
                'TODO Replace roSecurityNodes For Something
                Dim _IDNode As Integer = 0 'VTSecurity.roSecurityNodeManager.GetSecurityNodeFromGroupOrEmployee(New VTSecurity.roSecurityNodeState(-1), _IDEmployee, False, xDate)

                If _IDNode > 0 Then
                    ' Obtenemos el passport de sistema
                    Dim intIDPassport As Integer = roConstants.GetSystemUserId()

                    Dim oCalendarState = New VTCalendar.roCalendarState(intIDPassport)
                    Dim oCalendarManager As New VTCalendar.roCalendarManager(oCalendarState)

                    ' Obtenemos el horario y puesto asignado
                    Dim oEmployeeCalendar As New roCalendar
                    oEmployeeCalendar = oCalendarManager.Load(xDate, xDate, "B" & _IDEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)
                    If oEmployeeCalendar IsNot Nothing AndAlso oEmployeeCalendar.CalendarData IsNot Nothing Then

                        ' Parametro avanzado para indicar si tenemos que tener unicamente en cuenta el horario planificado
                        ' sin el puesto
                        Dim oParam As New AdvancedParameter.roAdvancedParameter("VTLive.RequestRules.MinCoverageWithoutAssignment", New AdvancedParameter.roAdvancedParameterState(-1))
                        Dim bolMinCoverageWithoutAssignment As Boolean = roTypes.Any2Boolean(oParam.Value)

                        For Each oEmployeeCalendarRowEmployeeData As roCalendarRow In oEmployeeCalendar.CalendarData
                            If oEmployeeCalendarRowEmployeeData IsNot Nothing AndAlso oEmployeeCalendarRowEmployeeData.PeriodData IsNot Nothing Then
                                For Each oEmployeeCalendarRowDayData As roCalendarRowDayData In oEmployeeCalendarRowEmployeeData.PeriodData.DayData
                                    If oEmployeeCalendarRowDayData.PlanDate = xDate Then
                                        If oEmployeeCalendarRowDayData.MainShift IsNot Nothing AndAlso oEmployeeCalendarRowDayData.MainShift.ID > 0 Then
                                            ' Obtenemos la cobertura minima para ese horario y puesto(en caso necesario)
                                            Dim oBudgetManager As New VTBudgets.roBudgetManager(New roBudgetState(intIDPassport))
                                            Dim oProductiveUnitModePosition As New roProductiveUnitModePosition
                                            oProductiveUnitModePosition.ShiftData = New roCalendarRowShiftData
                                            oProductiveUnitModePosition.ShiftData = oEmployeeCalendarRowDayData.MainShift
                                            If oEmployeeCalendarRowDayData.AssigData IsNot Nothing AndAlso oEmployeeCalendarRowDayData.AssigData.ID > 0 AndAlso Not bolMinCoverageWithoutAssignment Then
                                                oProductiveUnitModePosition.AssignmentData = New roCalendarAssignmentCellData
                                                oProductiveUnitModePosition.AssignmentData = oEmployeeCalendarRowDayData.AssigData
                                            End If

                                            If bolMinCoverageWithoutAssignment OrElse (oProductiveUnitModePosition.AssignmentData IsNot Nothing AndAlso oProductiveUnitModePosition.AssignmentData.ID > 0) Then
                                                oRet.MinCoverage = oBudgetManager.GetMinimumAmountWithSpecificSchedulerInNode(_IDNode, xDate, oProductiveUnitModePosition)

                                                ' Obtenemos los empleados planificados con el mismo horario/puesto
                                                Dim oEmployeesAvailable As roEmployeeAvailableForNode = oBudgetManager.GetEmployeesAvailableWithSpecificSchedulerInNode(_IDNode, xDate, oProductiveUnitModePosition)

                                                ' Diferencial
                                                If oEmployeesAvailable IsNot Nothing AndAlso oEmployeesAvailable.BudgetEmployeeAvailableForNode IsNot Nothing Then
                                                    oRet.DifferentialQuantity = oEmployeesAvailable.BudgetEmployeeAvailableForNode.Count - oRet.MinCoverage
                                                End If
                                            End If
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If
                End If

                bolRet = True
            Catch ex As DbException
                oReqState.UpdateStateInfo(ex, "roBudgetManager::GetCoverageOnEmployeeDate")
            Catch ex As Exception
                oReqState.UpdateStateInfo(ex, "roBudgetManager::GetCoverageOnEmployeeDate")
            End Try
            Return oRet
        End Function

        Public Shared Function GetAvailableEmployeesForDateOnEmployeeShiftExchange(ByVal oPassportTicket As roPassportTicket, ByVal sDate As String, ByVal eDate As String, ByVal idShift As Integer, ByVal oLng As roLanguage, ByVal idShiftApplicant As Integer, ByRef ostate As Employee.roEmployeeState, Optional ByVal idRequest As Integer = -1) As GenericList
            Return GetAvailableEmployeesForDateOnEmployeeShiftExchangeEx(oPassportTicket, sDate, idShift, oLng, idShiftApplicant, ostate, idRequest)
        End Function

        Public Shared Function GetAvailableEmployeesForDateOnEmployeeShiftExchangeEx(ByVal oPassportTicket As roPassportTicket, ByVal sDate As String, ByVal idShift As Integer, ByVal oLng As roLanguage, ByVal idShiftApplicant As Integer, ByRef oState As Employee.roEmployeeState, Optional ByVal idRequest As Integer = -1, Optional ByVal iEmployeeForValidation As Integer = 0) As GenericList
            Dim lrret As New DTOs.GenericList

            Try
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roRequest::GetAvailableEmployeesForDateOnEmployeeShiftExchangeEx::Start checking candidates for shift exchange ...")

                Dim selDate As DateTime = DateTime.ParseExact(sDate, "dd/MM/yyyy", Nothing)
                Dim selEmployee As Integer = -1
                Dim bSoftModeCheck As Boolean = False

                Dim oParam As New AdvancedParameter.roAdvancedParameter("VTLive.ExchangeShiftBetweenEmployees.SoftmodeCheck", New AdvancedParameter.roAdvancedParameterState(oState.IDPassport))
                bSoftModeCheck = roTypes.Any2Boolean(oParam.Value)

                If oPassportTicket.IDEmployee.HasValue Then selEmployee = oPassportTicket.IDEmployee

                ' Empleados candidatos al cambio
                ' - Con contrato el día solicado
                ' - Con día planificado
                ' - Con el día sin bloquear
                ' - El mismo puesto, si lo tiene, que el solicitante el día solicitado

                Dim strSQL As String
                Dim iCurrentAssignment As Integer = -1
                strSQL = "@SELECT# isnull(IDAssignment,-1) from DailySchedule where IDEmployee = " & selEmployee.ToString & " and date = '" & selDate.ToString("yyyyMMdd") & "'"
                iCurrentAssignment = roTypes.Any2Long(DataLayer.AccessHelper.ExecuteScalar(strSQL))
                Dim strAssignmentFilter As String = String.Empty

                If iCurrentAssignment = -1 Then
                    strAssignmentFilter = " and IDAssignment is NULL "
                Else
                    strAssignmentFilter = " and IDAssignment = " & iCurrentAssignment.ToString & " "
                End If

                strSQL = "@SELECT# distinct employees.Id, Employees.name, DailySchedule.LockedDay, isnull(IDLabAgree,0) as IDLabAgree from Employees " &
                                        " inner Join EmployeeContracts on Employees.ID = EmployeeContracts.IDEmployee " &
                                        " inner Join DailySchedule on Employees.ID = DailySchedule.IDEmployee and Date = '" & selDate.ToString("yyyyMMdd") & "' and IdShift1 is not NULL" & strAssignmentFilter &
                                        " where employees.id <> " & selEmployee & " and '" & selDate.ToString("yyyyMMdd") & "' between EmployeeContracts.BeginDate and EmployeeContracts.EndDate" &
                                        " and (ISNULL(DailySchedule.IDShiftUsed, DailySchedule.IDShift1) = " & idShift & " OR IDShiftBase = " & idShift & ")"

                If iEmployeeForValidation > 0 Then
                    strSQL = strSQL & " and employees.id = " & roTypes.Any2String(iEmployeeForValidation)
                End If

                Dim oTable As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)

                Dim lCandidateEmployees As New Generic.List(Of SelectField)
                Dim lCandidateEmployeesIdList As New Generic.List(Of String)
                Dim lNotSuitableCandidates As New Generic.List(Of SelectField)
                Dim dEmployeeLabagreeOnDate As New Dictionary(Of Integer, Integer)

                ' Comprobamos qué reglas de planificación hay que validar.
                ' En FASE I, esto se configura en registro.

                ' En fases posteriores se podrá configurar como regla de solicitud nueva. Será algo así ...
                ' Dim oLabAgreeState As New LabAgree.roLabAgreeState(oState.IDPassport)
                ' Dim labAgreeRules As Generic.List(Of LabAgree.roRequestRule) = LabAgree.roRequestRule.GetRequestsRules("IDLabAgree IN (" & sIdLabAgrees & ")", oLabAgreeState,, False)
                ' For Each oRequestRule As LabAgree.roRequestRule In labAgreeRules.FindAll(Function(x) x.IDRequestType = eRequestType.ExchangeShiftBetweenEmployees AndAlso x.IDRuleType = eRequestRuleType.ScheduleRuleToValidate)
                '    'TODO: Hay que guardar el par IDRule - Convenio

                ' Next

                Dim oCalRuleState As New roCalendarScheduleRulesState(oState.IDPassport)
                Dim oCalRulesManager As New roCalendarScheduleRulesManager(oCalRuleState)
                Dim dLabAgreeRulesToCheck As New Dictionary(Of Integer, List(Of Integer))
                Dim sRulesToCheck As String = String.Empty
                oParam = New AdvancedParameter.roAdvancedParameter("VTLive.Requests.RulesToCheck", New AdvancedParameter.roAdvancedParameterState(oState.IDPassport))
                sRulesToCheck = roTypes.Any2String(oParam.Value)
                Dim dPlanificationRules As New Dictionary(Of Integer, List(Of Integer))

                Try
                    If sRulesToCheck.Length > 0 Then
                        If sRulesToCheck.Contains(";") Then
                            For Each strRulesDef As String In sRulesToCheck.Split("@")
                                If Not strRulesDef.Contains(";") Then
                                    ' Formato incorrecto
                                    dLabAgreeRulesToCheck = Nothing
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roRequests::GetAvailableEmployeesForDateOnEmployeeShiftExchangeEx::Incorrect format on advanced parameter VTLive.Requests.RulesToCheck. Ignoring!")
                                    Exit For
                                Else
                                    dLabAgreeRulesToCheck.Add(strRulesDef.Split(";")(0), strRulesDef.Split(";")(1).Split(",").ToList.ConvertAll(Of Integer)(Function(x) CInt(x)))
                                End If
                            Next
                            sRulesToCheck = ""
                        Else
                            ' Limito por reglas independientemente del convenio
                            dLabAgreeRulesToCheck = Nothing
                        End If
                    Else
                        ' Obtenemos las reglas de planificacion si tiene
                        Dim oLabAgreeState As New LabAgree.roLabAgreeState(oState.IDPassport)
                        Dim IdLabAgree As Integer = -1

                        Dim strGetLabAgreeSQL As String = " @SELECT# IDLabAgree From EmployeeContracts " &
                                       " Where BeginDate <= " & roTypes.Any2Time(selDate).SQLSmallDateTime & " " &
                                       " And EndDate >= " & roTypes.Any2Time(selDate).SQLSmallDateTime & " " &
                                       " And IDEmployee = '" & selEmployee & "' "
                        IdLabAgree = roTypes.Any2Integer(ExecuteScalar(strGetLabAgreeSQL))
                        If IdLabAgree >= 0 Then
                            Dim labAgreeRules As Generic.List(Of LabAgree.roRequestRule) = LabAgree.roRequestRule.GetRequestsRules("IDLabAgree IN (" & IdLabAgree & ")", oLabAgreeState, False)

                            For Each oRequestRule As LabAgree.roRequestRule In labAgreeRules.FindAll(Function(x) x.IDRequestType = eRequestType.ExchangeShiftBetweenEmployees AndAlso x.IDRuleType = eRequestRuleType.ScheduleRuleToValidate)
                                Dim shiftIds = oRequestRule.Definition.IDReasons
                                Dim rulePlanificationIds = oRequestRule.Definition.IDPlanificationRules

                                If shiftIds IsNot Nothing AndAlso shiftIds.Count > 0 Then
                                    If shiftIds.Contains(-1) OrElse shiftIds.Contains(idShift) Then
                                        If rulePlanificationIds IsNot Nothing AndAlso rulePlanificationIds.Count > 0 Then
                                            If Not dPlanificationRules.ContainsKey(IdLabAgree) Then
                                                If rulePlanificationIds.Contains(-1) Then
                                                    dPlanificationRules.Add(IdLabAgree, oCalRulesManager.GetScheduleRules(iIDLabAgree:=IdLabAgree).ToList().ConvertAll(Of Integer)(Function(x) x.Id))
                                                Else
                                                    dPlanificationRules.Add(IdLabAgree, rulePlanificationIds)
                                                End If
                                            Else
                                                If rulePlanificationIds.Contains(-1) Then
                                                    dPlanificationRules(IdLabAgree) = oCalRulesManager.GetScheduleRules(iIDLabAgree:=IdLabAgree).ToList().ConvertAll(Of Integer)(Function(x) x.Id)
                                                Else
                                                    dPlanificationRules(IdLabAgree) = rulePlanificationIds
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            Next

                        End If
                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roRequests::GetAvailableEmployeesForDateOnEmployeeShiftExchangeEx::Incorrect format on advanced parameter VTLive.Requests.RulesToCheck. Ignoring!", ex)
                End Try

                If oTable IsNot Nothing AndAlso oTable.Rows.Count > 0 Then
                    Dim selectF As New SelectField
                    For Each dRow As DataRow In oTable.Rows
                        If dRow("LockedDay") = 0 Then
                            selectF = New SelectField
                            selectF.FieldValue = dRow(0)
                            selectF.FieldName = dRow(1)
                            selectF.RelatedInfo = String.Empty

                            lCandidateEmployees.Add(selectF)
                            lCandidateEmployeesIdList.Add(selectF.FieldValue)
                        Else
                            selectF = New SelectField
                            selectF.FieldValue = dRow(0)
                            selectF.FieldName = dRow(1)
                            selectF.RelatedInfo = "BlockedDay"
                            lNotSuitableCandidates.Add(selectF)
                        End If
                    Next

                    ' Limitaciones adicionales
                    ' 1.- Sin solicitud pendiente que pueda modificar el horario planificado (status <> Accepted=2 status <> Denied=3 ) de cambio horario (eRequestType.ChangeShift=5, VacationsOrPermissions=6, PlannedAbsences=7, PlannedHolidays=13, ExchangeShiftBetweenEmployees=8)
                    If lCandidateEmployeesIdList.Count > 0 Then
                        strSQL = String.Empty
                        strSQL = "@SELECT# * from ( "
                        strSQL = strSQL & "@SELECT# id, idEmployee, isnull(IDEmployeeExchange,-1) as idCandidateEmployee from Requests where RequestType = " & eRequestType.ChangeShift & " And '" & selDate.ToString("yyyyMMdd") & "' between Date1 and Date2 and status not in (" & eRequestStatus.Accepted & "," & eRequestStatus.Denied & ") "
                        strSQL = strSQL & "UNION "
                        strSQL = strSQL & "@SELECT# id, idEmployee, isnull(IDEmployeeExchange,-1) as idCandidateEmployee from Requests where RequestType = " & eRequestType.PlannedAbsences & " And '" & selDate.ToString("yyyyMMdd") & "' between Date1 and Date2 and status not in (" & eRequestStatus.Accepted & "," & eRequestStatus.Denied & ") "
                        strSQL = strSQL & "UNION "
                        strSQL = strSQL & "@SELECT# id, Requests.idEmployee, isnull(Requests.IDEmployeeExchange,-1) as idCandidateEmployee from Requests "
                        strSQL = strSQL & "inner Join sysroRequestDays on sysroRequestDays.IDRequest = Requests.ID "
                        strSQL = strSQL & "where RequestType = " & eRequestType.VacationsOrPermissions & "  And status Not In (" & eRequestStatus.Accepted & "," & eRequestStatus.Denied & ") And '" & selDate.ToString("yyyyMMdd") & "' = sysroRequestDays.Date And sysroRequestDays.BeginTime is NULL And sysroRequestDays.EndTime is NULL "
                        strSQL = strSQL & "UNION "
                        strSQL = strSQL & "@SELECT# id, idEmployee, isnull(IDEmployeeExchange,-1) as idCandidateEmployee from Requests where RequestType = " & eRequestType.PlannedHolidays & " And '" & selDate.ToString("yyyyMMdd") & "' between Date1 and Date2 and status not in (" & eRequestStatus.Accepted & "," & eRequestStatus.Denied & ") "
                        strSQL = strSQL & "UNION "
                        strSQL = strSQL & "@SELECT# id, idEmployee, isnull(IDEmployeeExchange,-1) as idCandidateEmployee from Requests where RequestType = " & eRequestType.ExchangeShiftBetweenEmployees & " And ('" & selDate.ToString("yyyyMMdd") & "' = Date1 OR '" & selDate.ToString("yyyyMMdd") & "' = Date2) and status not in (" & eRequestStatus.Accepted & "," & eRequestStatus.Denied & ") "
                        strSQL = strSQL & " ) as tmp "
                        strSQL = strSQL & " where (idCandidateEmployee in (" & String.Join(",", lCandidateEmployeesIdList) & "," & selEmployee.ToString & ") OR idEmployee in (" & String.Join(",", lCandidateEmployeesIdList) & "," & selEmployee.ToString & ")) "
                        strSQL = strSQL & " and id <> " & idRequest.ToString

                        Dim oTable1 As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)

                        If oTable1 IsNot Nothing AndAlso oTable1.Rows.Count > 0 Then
                            Dim selectF0 As New SelectField
                            Dim selectF1 As New SelectField
                            Dim selectF2 As New SelectField
                            For Each dRow As DataRow In oTable1.Rows
                                ' Si el empleado que solicita tiene una solicitud pendiente para ese día, no hace falta que siga ...
                                If selEmployee = Any2Integer(dRow("idEmployee")) OrElse selEmployee = Any2Integer(dRow("idCandidateEmployee")) Then
                                    ' Añado todos los empleados a la lista de no válidos
                                    '...
                                    ' Marco a todos los empleados como no compatibles por impacto de planificación
                                    For Each idEmployee As Integer In lCandidateEmployeesIdList
                                        If lCandidateEmployees.FindAll(Function(x) x.FieldValue = idEmployee).Count > 0 Then
                                            selectF0 = New SelectField
                                            selectF0.FieldValue = lCandidateEmployees.Find(Function(x) x.FieldValue = idEmployee).FieldValue
                                            selectF0.FieldName = lCandidateEmployees.Find(Function(x) x.FieldValue = idEmployee).FieldName
                                            selectF0.RelatedInfo = "RequestPending"
                                            lNotSuitableCandidates.Add(selectF0)
                                        End If
                                    Next
                                    ' ... y elimino de la de candidatos
                                    lCandidateEmployeesIdList.Clear()
                                    lCandidateEmployees.Clear()
                                    Exit For
                                Else
                                    ' Añado a lista de candidatos no válidos ...
                                    If lCandidateEmployees.FindAll(Function(x) x.FieldValue = dRow("idEmployee")).Count > 0 Then
                                        selectF1 = New SelectField
                                        selectF1.FieldValue = lCandidateEmployees.Find(Function(x) x.FieldValue = dRow("idEmployee")).FieldValue
                                        selectF1.FieldName = lCandidateEmployees.Find(Function(x) x.FieldValue = dRow("idEmployee")).FieldName
                                        selectF1.RelatedInfo = "RequestPending"
                                        lNotSuitableCandidates.Add(selectF1)
                                        ' ... y elimino de la de candidatos
                                        lCandidateEmployeesIdList.RemoveAll(Function(x) x = dRow("idEmployee"))
                                        lCandidateEmployees.RemoveAll(Function(x) x.FieldValue = dRow("idEmployee"))
                                    End If
                                    If dRow("idCandidateEmployee") > 0 Then
                                        ' Añado a lista de candidatos no válidos ...
                                        If lCandidateEmployees.FindAll(Function(x) x.FieldValue = dRow("idCandidateEmployee")).Count > 0 Then
                                            selectF2 = New SelectField
                                            selectF2.FieldValue = lCandidateEmployees.Find(Function(x) x.FieldValue = dRow("idCandidateEmployee")).FieldValue
                                            selectF2.FieldName = lCandidateEmployees.Find(Function(x) x.FieldValue = dRow("idCandidateEmployee")).FieldName
                                            selectF2.RelatedInfo = "RequestPending"
                                            lNotSuitableCandidates.Add(selectF2)
                                            ' ... y elimino de la de candidatos
                                            lCandidateEmployeesIdList.RemoveAll(Function(x) x = dRow("idCandidateEmployee"))
                                            lCandidateEmployees.RemoveAll(Function(x) x.FieldValue = dRow("idCandidateEmployee"))
                                        End If
                                    End If
                                End If
                            Next
                        End If

                        ' 2.- Con permiso sobre el horario que se tiene asignado el solicitante
                        If lCandidateEmployeesIdList.Count > 0 Then
                            Dim oShiftState As Shift.roShiftState
                            Dim oReqState As Requests.roRequestState
                            Dim oTable2 As DataTable
                            Dim dv As DataView
                            Dim lCandidateEmployeesToDeleteList As New Generic.List(Of String)
                            Dim bHasPermission As Boolean = False
                            Dim idPassportEmployee As Integer
                            For Each iIDEmployee In lCandidateEmployeesIdList
                                idPassportEmployee = roPassportManager.GetPassportTicket(iIDEmployee, LoadType.Employee).ID
                                oShiftState = New Shift.roShiftState(idPassportEmployee)
                                oReqState = New Requests.roRequestState(idPassportEmployee)
                                oTable2 = Shift.roShift.GetShiftsByEmployeeVisibilityPermissions(iIDEmployee, oShiftState,,,, True)
                                dv = New DataView(oTable2)
                                dv.RowFilter = "id = " & idShiftApplicant.ToString
                                oTable2 = dv.ToTable
                                ' 2.1.- Con permiso sobre la funcionalidad de intercambio de horarios
                                bHasPermission = (Security.WLHelper.GetFeaturePermission(idPassportEmployee, New Requests.roRequestTypeSecurity(eRequestType.ExchangeShiftBetweenEmployees, oReqState).EmployeeFeatureName, "E") >= Permission.Write)
                                If (Not oTable2 Is Nothing AndAlso oTable2.Rows.Count = 0) OrElse Not bHasPermission Then
                                    ' No tiene permisos para ese horario. Quito al empleado
                                    lCandidateEmployeesToDeleteList.Add(iIDEmployee)
                                End If
                            Next
                            If lCandidateEmployeesToDeleteList.Count > 0 Then
                                For Each iIDEmployeeToDelete In lCandidateEmployeesToDeleteList
                                    ' Añado a lista de candidatos no válidos ...
                                    Dim selectF3 As New SelectField
                                    selectF3.FieldValue = lCandidateEmployees.Find(Function(x) x.FieldValue = iIDEmployeeToDelete).FieldValue
                                    selectF3.FieldName = lCandidateEmployees.Find(Function(x) x.FieldValue = iIDEmployeeToDelete).FieldName
                                    selectF3.RelatedInfo = "NoPermission"
                                    lNotSuitableCandidates.Add(selectF3)
                                    ' ... y elimino de la de candidatos
                                    lCandidateEmployeesIdList.RemoveAll(Function(x) x = iIDEmployeeToDelete)
                                    lCandidateEmployees.RemoveAll(Function(x) x.FieldValue = iIDEmployeeToDelete)
                                Next
                            End If

                            Dim intIDPassport As Integer = roConstants.GetSystemUserId()
                            Dim oCalendarState = New roCalendarState(intIDPassport)
                            'Dim oCalendarState = New roCalendarState(1)
                            Dim oCalendarManager As New roCalendarManager(oCalendarState)
                            Dim oCandidatesCalendar As New DTOs.roCalendar
                            Dim oApplicantCalendar As New DTOs.roCalendar
                            Dim oParameters As New roParameters("OPTIONS", True)
                            Dim iMonthIniDay As Integer
                            Dim iYearIniMonth As Integer
                            Dim dYearFirstDate As Date = DateSerial(2000, 1, 1)
                            Dim dYearLastDate As Date = DateSerial(2000, 1, 1)
                            Dim oIndictments As New List(Of roCalendarScheduleIndictment)
                            Dim oApplicantShiftData As roCalendarRowShiftData = Nothing
                            Dim oApplicantRequestedShiftData As roCalendarRowShiftData = Nothing
                            dYearFirstDate = oCalRulesManager.GetYearFirstDate(selDate, oParameters, iMonthIniDay, iYearIniMonth)
                            dYearLastDate = dYearFirstDate.AddYears(1).AddDays(-1)

                            ' 3.- Que el horario solicitado no provoque incumplimiento de reglas en su planificación. Si es así, no hace falta continuar ...
                            If lCandidateEmployeesIdList.Count > 0 Then
                                ' Cargo definición del horario del solicitante el día solicitado
                                ' TODO: Con horarios flotantes o por horas, el objeto oApplicantRequestedShiftData y también oApplicantShiftData debería cargarlo del calendario, y no por el id de horario, poque los datos variables del horario dependen del día, y alguna regla, como la de descanso entre jornadas, también
                                '       Y como consecuencia, esta validación de imcumplimiento del solicitante, debería hacerse después de elegir el candidato, y por tanto en el validate
                                oApplicantShiftData = oCalendarManager.LoadShiftDayDataByIdShift(idShiftApplicant)
                                oApplicantRequestedShiftData = oCalendarManager.LoadShiftDayDataByIdShift(idShift)

                                oApplicantCalendar = oCalendarManager.Load(dYearFirstDate, dYearLastDate, "B" & selEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, True)

                                For Each oCalendarRow As roCalendarRow In oApplicantCalendar.CalendarData
                                    For Each oCalendarRowDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                                        If oCalendarRowDayData.PlanDate = selDate Then
                                            oCalendarRowDayData.MainShift = oApplicantRequestedShiftData
                                            oCalendarRowDayData.HasChanged = True
                                            Exit For
                                        End If
                                    Next
                                Next

                                If (Not IsNothing(dLabAgreeRulesToCheck) AndAlso dLabAgreeRulesToCheck.Count > 0) OrElse Not String.IsNullOrEmpty(sRulesToCheck) Then
                                    oIndictments = oCalRulesManager.CheckScheduleRules(oApplicantCalendar, dLabAgreeRulesToCheck, sRulesToCheck)
                                ElseIf dPlanificationRules IsNot Nothing AndAlso dPlanificationRules.Count > 0 Then
                                    oIndictments = oCalRulesManager.CheckScheduleRules(oApplicantCalendar, dLabAgreeActualIdRulesToCheck:=dPlanificationRules)
                                End If

                                ' Si hay impactos, vacío la lista de candidatos ...
                                If oIndictments IsNot Nothing AndAlso oIndictments.Count > 0 Then
                                    Dim selectF31 As New SelectField
                                    For Each oIndictment As roCalendarScheduleIndictment In oIndictments
                                        ' Si hay impacto en la fecha a planificar ...
                                        If oIndictment.Dates.Contains(selDate) Then
                                            ' Marco a todos los empleados como no compatibles por impacto de planificación
                                            For Each idEmployee As Integer In lCandidateEmployeesIdList
                                                If lCandidateEmployees.FindAll(Function(x) x.FieldValue = idEmployee).Count > 0 Then
                                                    selectF31 = New SelectField
                                                    selectF31.FieldValue = lCandidateEmployees.Find(Function(x) x.FieldValue = idEmployee).FieldValue
                                                    selectF31.FieldName = lCandidateEmployees.Find(Function(x) x.FieldValue = idEmployee).FieldName
                                                    selectF31.RelatedInfo = "Indictment" & roTypes.Any2String(oIndictment.IDScheduleRule)
                                                    lNotSuitableCandidates.Add(selectF31)
                                                End If
                                            Next
                                            ' ... y elimino de la de candidatos
                                            lCandidateEmployeesIdList.Clear()
                                            lCandidateEmployees.Clear()
                                        End If
                                    Next
                                End If
                            End If

                            ' 4.- Limitamos:
                            '     - Que el día solicitado los empleados no tengan ni vacaciones ni ausencia por días
                            '     - Que si hay puestos planificados, puedan ser intercambiados junto con los horarios
                            If lCandidateEmployeesIdList.Count > 0 Then

                                ' Cargo calendatios para el día solicitado, tanto para los candidatos como para el solicitante
                                oCandidatesCalendar = oCalendarManager.Load(selDate, selDate, "B" & String.Join(",", lCandidateEmployeesIdList).Replace(",", ",B"), CalendarView.Planification, CalendarDetailLevel.Daily, True)
                                oApplicantCalendar = oCalendarManager.Load(selDate, selDate, "B" & selEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, True)

                                Dim selectF4 As New SelectField
                                Dim selectF401 As New SelectField
                                Dim selectF41 As New SelectField
                                Dim selectF42 As New SelectField
                                For Each oCalendarRow As roCalendarRow In oCandidatesCalendar.CalendarData
                                    ' 4.1.- Descarto empleados que tengan una ausencia para ese día, o estén de vacaciones
                                    If oCalendarRow.PeriodData.DayData(0).Alerts.OnAbsenceDays OrElse oCalendarRow.PeriodData.DayData(0).Alerts.OnHolidays Then
                                        ' Añado a lista de candidatos no válidos ...
                                        selectF4 = New SelectField
                                        Dim sReason As String = String.Empty
                                        If oCalendarRow.PeriodData.DayData(0).Alerts.OnAbsenceDays Then sReason = "OnAbsence" Else sReason = "OnHolidays"
                                        selectF4.FieldValue = lCandidateEmployees.Find(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee).FieldValue
                                        selectF4.FieldName = lCandidateEmployees.Find(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee).FieldName
                                        selectF4.RelatedInfo = sReason
                                        lNotSuitableCandidates.Add(selectF4)
                                        ' ... y elimino de la de candidatos
                                        lCandidateEmployeesIdList.RemoveAll(Function(x) x = oCalendarRow.EmployeeData.IDEmployee)
                                        lCandidateEmployees.RemoveAll(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee)
                                    Else
                                        ' Si no estoy en modo de validacíón suave, verifico que se intercambien puestos correcots
                                        If Not bSoftModeCheck Then
                                            ' 4.1.- Verifico que si el solicitante está cubriendo un puesto, el solicitado debe estar también asignado al mismo.
                                            '         Si el solicitante no está cubriendo puesto, el solicitado tampoco debe estarlo

                                            ' 4.1.1.- Si intercambiamos un día no laborable por uno laborable con puesto asignado, el empleado que hará el laborable debe ser capaz de cubrir el puesto
                                            If (oCalendarRow.PeriodData.DayData(0).MainShift.PlannedHours > 0 AndAlso Not oCalendarRow.PeriodData.DayData(0).AssigData Is Nothing) AndAlso oApplicantCalendar.CalendarData(0).PeriodData.DayData(0).MainShift.PlannedHours = 0 Then
                                                ' El solicitante debe poder hacer el puesto del destino
                                                Dim strSQLAux As String = "@SELECT# DISTINCT Employees.ID " &
                                                                "FROM Employees " &
                                                                "INNER JOIN EmployeeAssignments ON EmployeeAssignments.IDEmployee = Employees.ID  " &
                                                                "WHERE EmployeeAssignments.IDAssignment = " & oCalendarRow.PeriodData.DayData(0).AssigData.ID.ToString & " " &
                                                                "AND Employees.ID = " & selEmployee
                                                If roTypes.Any2Long(DataLayer.AccessHelper.ExecuteScalar(strSQLAux)) = 0 Then
                                                    selectF401 = New SelectField
                                                    Dim sReason As String = String.Empty
                                                    selectF401.FieldValue = lCandidateEmployees.Find(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee).FieldValue
                                                    selectF401.FieldName = lCandidateEmployees.Find(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee).FieldName
                                                    selectF401.RelatedInfo = "ApplicantCantCoverEmployee"
                                                    lNotSuitableCandidates.Add(selectF401)
                                                    ' ... y elimino de la de candidatos
                                                    lCandidateEmployeesIdList.RemoveAll(Function(x) x = oCalendarRow.EmployeeData.IDEmployee)
                                                    lCandidateEmployees.RemoveAll(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee)
                                                End If
                                            ElseIf (oApplicantCalendar.CalendarData(0).PeriodData.DayData(0).MainShift.PlannedHours > 0 AndAlso Not oApplicantCalendar.CalendarData(0).PeriodData.DayData(0).AssigData Is Nothing) AndAlso oCalendarRow.PeriodData.DayData(0).MainShift.PlannedHours = 0 Then
                                                ' 4.1.2.- Si intercambiamos un día laborable con puesto asignado por uno no laborable, el empleado que hará el laborable debe ser capaz de cubrir el puesto
                                                Dim strSQLAux As String = "@SELECT# DISTINCT Employees.ID " &
                                                                "FROM Employees " &
                                                                "INNER JOIN EmployeeAssignments ON EmployeeAssignments.IDEmployee = Employees.ID  " &
                                                                "WHERE EmployeeAssignments.IDAssignment = " & oApplicantCalendar.CalendarData(0).PeriodData.DayData(0).AssigData.ID.ToString & " " &
                                                                "AND Employees.ID = " & oCalendarRow.EmployeeData.IDEmployee
                                                If roTypes.Any2Long(DataLayer.AccessHelper.ExecuteScalar(strSQLAux)) = 0 Then
                                                    selectF401 = New SelectField
                                                    Dim sReason As String = String.Empty
                                                    selectF401.FieldValue = lCandidateEmployees.Find(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee).FieldValue
                                                    selectF401.FieldName = lCandidateEmployees.Find(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee).FieldName
                                                    selectF401.RelatedInfo = "EmployeeCantCoverApplicant"
                                                    lNotSuitableCandidates.Add(selectF401)
                                                    ' ... y elimino de la de candidatos
                                                    lCandidateEmployeesIdList.RemoveAll(Function(x) x = oCalendarRow.EmployeeData.IDEmployee)
                                                    lCandidateEmployees.RemoveAll(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee)
                                                End If
                                            Else
                                                If oCalendarRow.PeriodData.DayData(0).AssigData Is Nothing Then
                                                    ' 4.1.3.-  Si el día solicitado no tiene puesto asignado, el solicitante tampoco
                                                    If Not oApplicantCalendar.CalendarData(0).PeriodData.DayData(0).AssigData Is Nothing Then
                                                        selectF41 = New SelectField
                                                        Dim sReason As String = String.Empty
                                                        selectF41.FieldValue = lCandidateEmployees.Find(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee).FieldValue
                                                        selectF41.FieldName = lCandidateEmployees.Find(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee).FieldName
                                                        selectF41.RelatedInfo = "NoAssignment"
                                                        lNotSuitableCandidates.Add(selectF41)
                                                        ' ... y elimino de la de candidatos
                                                        lCandidateEmployeesIdList.RemoveAll(Function(x) x = oCalendarRow.EmployeeData.IDEmployee)
                                                        lCandidateEmployees.RemoveAll(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee)
                                                    End If
                                                Else
                                                    ' 4.1.3.-  Si el día solicitado tiene puesto asignado, el solicitante también, y es el mismo
                                                    If oApplicantCalendar.CalendarData(0).PeriodData.DayData(0).AssigData Is Nothing OrElse (oCalendarRow.PeriodData.DayData(0).AssigData.ID <> oApplicantCalendar.CalendarData(0).PeriodData.DayData(0).AssigData.ID) Then
                                                        selectF42 = New SelectField

                                                        Dim sReason As String = String.Empty
                                                        selectF42.FieldValue = lCandidateEmployees.Find(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee).FieldValue
                                                        selectF42.FieldName = lCandidateEmployees.Find(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee).FieldName
                                                        selectF42.RelatedInfo = "WrongAssignment"
                                                        lNotSuitableCandidates.Add(selectF42)
                                                        ' ... y elimino de la de candidatos
                                                        lCandidateEmployeesIdList.RemoveAll(Function(x) x = oCalendarRow.EmployeeData.IDEmployee)
                                                        lCandidateEmployees.RemoveAll(Function(x) x.FieldValue = oCalendarRow.EmployeeData.IDEmployee)
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                Next

                                ' 5.- Limitamos los empleados a aquellos que, si les planificamos el horario del solicitante del intercambio, no generan alertas de reglas de planificación
                                If Not bSoftModeCheck AndAlso lCandidateEmployeesIdList.Count > 0 Then
                                    oIndictments = New List(Of roCalendarScheduleIndictment)
                                    Try
                                        ' ESTO ES REDUNDANTE?
                                        'oCandidatesCalendar = oCalendarManager.Load(selDate, selDate, "B" & String.Join(",", lCandidateEmployeesIdList).Replace(",", ",B"), CalendarView.Planification, CalendarDetailLevel.Daily, True)
                                        ' ESTO ES REDUNDANTE?

                                        ' Planifico el horario del solicitante a los candidatos
                                        For Each oCalendarRow As roCalendarRow In oCandidatesCalendar.CalendarData
                                            For Each oCalendarRowDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                                                If oCalendarRowDayData.PlanDate = selDate Then
                                                    oCalendarRowDayData.MainShift = oApplicantShiftData
                                                    oCalendarRowDayData.HasChanged = True
                                                    Exit For
                                                End If
                                            Next
                                        Next

                                        oIndictments = oCalRulesManager.CheckScheduleRules(oCandidatesCalendar, dLabAgreeRulesToCheck, sRulesToCheck)

                                        ' Elimino de la lista de candidatos a aquellos empleados que tengan impactos
                                        If oIndictments IsNot Nothing AndAlso oIndictments.Count > 0 Then
                                            Dim selectF5 As New SelectField
                                            For Each oIndictment As roCalendarScheduleIndictment In oIndictments
                                                ' Si hay impacto en la fecha a planificar ...
                                                If oIndictment.Dates.Contains(selDate) Then
                                                    ' Añado a lista de candidatos no válidos ...
                                                    If lCandidateEmployees.FindAll(Function(x) x.FieldValue = oIndictment.IDEmployee).Count > 0 Then
                                                        selectF5 = New SelectField
                                                        selectF5.FieldValue = lCandidateEmployees.Find(Function(x) x.FieldValue = oIndictment.IDEmployee).FieldValue
                                                        selectF5.FieldName = lCandidateEmployees.Find(Function(x) x.FieldValue = oIndictment.IDEmployee).FieldName
                                                        selectF5.RelatedInfo = "Indictment" & roTypes.Any2String(oIndictment.IDScheduleRule)
                                                        lNotSuitableCandidates.Add(selectF5)
                                                    End If
                                                    ' ... y elimino de la de candidatos
                                                    lCandidateEmployeesIdList.RemoveAll(Function(x) x = oIndictment.IDEmployee)
                                                    lCandidateEmployees.RemoveAll(Function(x) x.FieldValue = oIndictment.IDEmployee)
                                                End If
                                            Next
                                        End If
                                    Catch ex As Exception
                                        lrret.Status = ErrorCodes.GENERAL_ERROR
                                    End Try
                                End If
                            End If
                        End If
                    End If

                    Dim res As New List(Of SelectField)
                    If lCandidateEmployeesIdList.Count > 0 Then
                        lrret.Status = ErrorCodes.OK
                        res.AddRange(lCandidateEmployees)
                    End If
                    If lNotSuitableCandidates.Count > 0 Then
                        res.AddRange(lNotSuitableCandidates)
                    End If
                    lrret.SelectFields = res.ToArray
                End If

                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roRequest::GetAvailableEmployeesForDateOnEmployeeShiftExchangeEx:: ... done !!!")
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::CommonHelper::GetAvailableEmployeesForDate")
            End Try

            Return lrret
        End Function

        ''' <summary>
        ''' Retorna una lista de empleados que pueden intercambiar un periodo de planificación con otro (oPassportTicket.IdEmployee)
        ''' </summary>
        ''' <param name="oPassportTicket"></param>
        ''' <param name="sDateStart"></param>
        ''' <param name="sDateEnd"></param>
        ''' <param name="oLng"></param>
        ''' <param name="ostate"></param>
        ''' <param name="idRequest"></param>
        ''' <returns></returns>
        Public Shared Function GetAvailableEmployeesForPeriodOnEmployeeShiftExchange(ByVal oPassportTicket As roPassportTicket, ByVal sDateStart As String, ByVal sDateEnd As String, ByVal idRequestedShift As Integer, ByVal oLng As roLanguage, ByVal idApplicantIdShift As Integer, ByRef ostate As Employee.roEmployeeState, Optional ByVal idRequest As Integer = -1) As GenericList
            Dim oRet As New GenericList
            Dim oDayEmployeeList As New GenericList
            Dim oFinalEmployeeList As New GenericList
            Dim lDayEmployeeAvailable As New List(Of SelectField)
            Dim lFinalEmployeeAvailable As New List(Of SelectField)
            Dim lNotAvailableEmployees As New List(Of SelectField)

            Dim dStartDate As Date = Any2DateTime(sDateStart).Date
            Dim dEndDate As Date = Any2DateTime(sDateEnd).Date
            Dim dDate As Date = dStartDate
            Dim iIDEmployee As Integer = -1
            Dim oSelComparer = New SelectFieldComparer
            Dim oApplicantCalendar As New DTOs.roCalendar
            Dim oCalendarState = New roCalendarState(1)
            Dim oCalendarManager As New roCalendarManager(oCalendarState)
            Dim idEmployeeApplicant As Integer
            Dim iAux As Integer = 0
            Dim sIDEmployeesFilter As String = String.Empty

            Try
                If oPassportTicket.IDEmployee.HasValue Then idEmployeeApplicant = oPassportTicket.IDEmployee
                oApplicantCalendar = oCalendarManager.Load(dStartDate, dEndDate, "B" & idEmployeeApplicant.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, True)

                For Each oCalendarRow In oApplicantCalendar.CalendarData
                    For Each oDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                        dDate = oDayData.PlanDate
                        If iAux = 0 Then
                            oDayEmployeeList = GetAvailableEmployeesForDateOnEmployeeShiftExchangeEx(oPassportTicket, dDate, idRequestedShift, oLng, oDayData.MainShift.ID, ostate)
                        Else
                            sIDEmployeesFilter = String.Join(",", lFinalEmployeeAvailable.FindAll(Function(x) x.RelatedInfo = "").ConvertAll(Function(y) y.FieldValue))
                            If sIDEmployeesFilter.Length > 0 Then
                                oDayEmployeeList = GetAvailableEmployeesForDateOnEmployeeShiftExchangeEx(oPassportTicket, dDate, idRequestedShift, oLng, oDayData.MainShift.ID, ostate)
                            Else
                                ' Ya podemos salir. no quedó ningún empleado
                                Exit For
                            End If
                        End If

                        lDayEmployeeAvailable.Clear()
                        lDayEmployeeAvailable = oDayEmployeeList.SelectFields.ToList
                        ' Guardo los empleados que no están disponibles, para marcarlos al final
                        lNotAvailableEmployees.AddRange(lDayEmployeeAvailable.FindAll(Function(x) x.RelatedInfo <> "").ToList)

                        ' Comparo la lista que ya tenía (si no era nueva), con la obtenida para este día.
                        ' Dejo sólo los empleados que estén en las dos listas y cuyo RelatedInfo sea Empty (en RelatedInfo guardo el motivo por el que un empleado no es candidato
                        If iAux = 0 Then
                            lFinalEmployeeAvailable = New List(Of SelectField)(lDayEmployeeAvailable)
                        Else
                            lFinalEmployeeAvailable = New List(Of SelectField)(lFinalEmployeeAvailable.Intersect(lDayEmployeeAvailable, oSelComparer).ToList)
                        End If
                        iAux = iAux + 1
                    Next
                Next

                ' Ahora reviso los RelatedInfo para asegurar que la disponibilidad de los empleados se muestra correctamente
                ' TODO: Optimizar este punto sin recursión
                For Each oSel As SelectField In lFinalEmployeeAvailable
                    If oSel.RelatedInfo = "" Then
                        If lNotAvailableEmployees.FindAll(Function(z) z.FieldValue = oSel.FieldValue).Count > 0 Then
                            oSel.RelatedInfo = lNotAvailableEmployees.Find(Function(z) z.FieldValue = oSel.FieldValue).RelatedInfo
                        End If
                    End If
                Next

                ' Por último, miro a los empleados que con esta planificación, generarían indictments
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::CommonHelper::GetAvailableEmployeesForPeriodOnEmployeeShiftExchange")
            End Try

            oRet.SelectFields = lFinalEmployeeAvailable.ToArray

            Return oRet
        End Function

        Public Shared Function RecalculateStatus(ByVal _State As roRequestState, ByVal oRequestType As eRequestType, ByVal IDCause As Integer) As Boolean
            Dim bolRet As Boolean = False

            Dim bIsNew As Boolean = False

            Try

                bolRet = True
                ' Obtenemos todas las solicitudes a recalcular su estado que esten pendientes o en curso
                ' de la justificación indicada o del tipo indicado
                Dim strSQL As String = "@SELECT# ID FROM Requests " &
                                               "WHERE Status IN(" & eRequestStatus.OnGoing & "," & eRequestStatus.Pending & ")"

                If oRequestType <> eRequestType.None Then
                    strSQL += " AND RequestType=" & oRequestType
                End If

                If IDCause <> -1 Then
                    strSQL += " AND IDCause=" & IDCause.ToString
                    strSQL += " AND RequestType IN(@SELECT# IDType FROM sysroRequestType WHERE IDCategory=-1)"
                End If

                Dim tb As New DataTable("Requests")
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb.Rows IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        bolRet = True
                        Dim oRequest = New roRequest(oRow("ID"), _State)

                        If oRequest IsNot Nothing Then
                            oRequest.lstRequestApprovals = Nothing
                            Dim strSQLLevel As String = "@SELECT# isnull(dbo.GetMinLevelOfRequest(" & oRequest.RequestType & "," & IIf(oRequest.IDCause.HasValue, oRequest.IDCause.ToString, "0") & "),11) AS MinLevel "
                            oRequest.StatusLevel = Any2Integer(ExecuteScalar(strSQLLevel)) + 1
                            oRequest.RequestStatus = eRequestStatus.Pending
                            bolRet = oRequest.Save(True)

                            If bolRet Then
                                PersistRequestPermissions(oRequest.ID)
                            End If
                        End If
                        If Not bolRet Then Exit For
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequest::RecalculateStatus")
                bolRet = False
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequest::RecalculateStatus")
                bolRet = False
            End Try

            Return bolRet
        End Function

        Private Shared Sub PersistRequestPermissions(idRequest As Integer)
            Dim Command As DbCommand = CreateCommand("ExecuteRequestPassportPermissionsAction")
            Command.CommandType = CommandType.StoredProcedure
            Command.CommandTimeout = 0
            AddParameter(Command, "@IDAction", DbType.Int32, 1).Value = 2
            AddParameter(Command, "@IDObject", DbType.Int32, 1).Value = idRequest
            AddParameter(Command, "@Version", DbType.Int32, 1).Value = 3
            Command.ExecuteNonQuery()
        End Sub

        Public Shared Function GetOverlappingEmployees(ByVal idrequest As Integer, ByVal _State As roRequestState) As DataTable
            Dim lrret As DataTable = Nothing
            Dim dtOverlappedAbsences As New DataTable

            Try

                Dim oRequest As New Requests.roRequest(idrequest, _State)

                If oRequest IsNot Nothing Then
                    Dim BeginDate As Date
                    Dim EndDate As Date
                    Dim strSQL As String = ""

                    ' Generamos tramos consecutivos de dias solicitudas
                    ' y sobre cada uno de los tramos generados realizaremos las validaciones
                    Dim oListPeriod As New Generic.List(Of roTimePeriod)
                    Dim oPeriod As roTimePeriod = Nothing
                    If oRequest.RequestDays Is Nothing OrElse oRequest.RequestDays.Count = 0 Then
                        ' En caso que la solicitud no tenga lista de dias solicitados, un tramo con la fecha de inicio y fin de la solicitud
                        oPeriod = New roTimePeriod
                        oPeriod.Begin = Any2Time(oRequest.Date1.Value)
                        oPeriod.Finish = Any2Time(IIf(oRequest.Date2.HasValue, oRequest.Date2.Value, oRequest.Date1.Value))
                        oListPeriod.Add(oPeriod)
                    Else
                        For Each oRequestDay As roRequestDay In oRequest.RequestDays
                            If oPeriod Is Nothing Then
                                oPeriod = New roTimePeriod
                                oPeriod.Begin = Any2Time(oRequestDay.RequestDate)
                                oPeriod.Finish = Any2Time(oRequestDay.RequestDate)
                            Else
                                If oPeriod.Finish.Add(1, "d").Value = Any2Time(oRequestDay.RequestDate).Value Then
                                    ' Si el siguiente registro es justo el dia posterior, hacemos el periodo mas grande
                                    oPeriod.Finish = Any2Time(oRequestDay.RequestDate)
                                Else
                                    ' Si no es consecutivo, guardamos el periodo anterior en la lista y creamos uno nuevo con la fecha actual
                                    oListPeriod.Add(oPeriod)
                                    oPeriod = New roTimePeriod
                                    oPeriod.Begin = Any2Time(oRequestDay.RequestDate)
                                    oPeriod.Finish = Any2Time(oRequestDay.RequestDate)
                                End If
                            End If
                        Next
                        If oPeriod IsNot Nothing Then
                            oListPeriod.Add(oPeriod)
                        End If
                    End If

                    For Each _Period As roTimePeriod In oListPeriod
                        BeginDate = _Period.Begin.Value
                        EndDate = _Period.Finish.Value

                        Dim queryDateStart As String = roTypes.Any2Time(BeginDate).SQLSmallDateTime
                        Dim queryDateEnd As String = roTypes.Any2Time(EndDate).SQLSmallDateTime

                        ' Filtro de empleados del mismo departamento que el solicitante a fecha inicial de la solicitud
                        Dim strEmployees As String = $"(@SELECT# DISTINCT ceg.IDEmployee FROM sysrovwCurrentEmployeeGroups ceg with (nolock)
	                                                    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature pef ON pef.IdFeature = 2 and {roTypes.Any2Time(BeginDate).SQLSmallDateTime} between pef.BeginDate and pef.EndDate and idpassport = {_State.IDPassport.ToString()} and pef.IdEmployee = ceg.IDEmployee and FeaturePermission > 0
	                                                    WHERE IDGroup = (@SELECT# idgroup from EmployeeGroups eg where idemployee= {oRequest.IDEmployee.ToString} AND {roTypes.Any2Time(BeginDate).SQLSmallDateTime} between eg.BeginDate And eg.EndDate) 
                                                            AND {roTypes.Any2Time(BeginDate).SQLSmallDateTime} between ceg.BeginDate And ceg.EndDate AND ceg.IDEmployee <> {oRequest.IDEmployee.ToString} AND ceg.CurrentEmployee = 1) z"

                        ' Solicitudes de previsiones de ausencia por dias/horas
                        strSQL = "@SELECT# r.idemployee, e.Name as employeename, e.image, 'Request' as tipo, isnull(r.IDCause,0) as IDCause, r.Date1 as BeginDate, isnull(r.Date2, r.Date1) as finishdate, r.FromTime as begintime, isnull(r.ToTime,r.FromTime) as endtime, CONVERT(NUMERIC(8, 6), isnull(r.Hours,0)) as duration ,Datediff(DAY, r.date1, isnull(r.date2, r.date1)) + 1 as cantidaddias , r.RequestType , r.id as idrequest, 0 as AllDay   "
                        strSQL &= " FROM Requests r with (nolock)  INNER JOIN employees e with (nolock)  ON e.id = r.idemployee  "
                        strSQL &= " INNER JOIN " & strEmployees & " ON e.id = Z.idemployee  "
                        strSQL &= " WHERE (r.RequestType IN(" & eRequestType.PlannedCauses & "," & eRequestType.PlannedAbsences & ")) AND"
                        strSQL &= " ( (r.Date1 >= " & queryDateStart & " AND r.Date1 <= " & queryDateEnd & ")"
                        strSQL &= " OR "
                        strSQL &= " (IsNULL(r.Date2,r.Date1) >= " & queryDateStart & " AND IsNULL(r.Date2,r.Date1) <= " & queryDateEnd & ")"
                        strSQL &= " OR "
                        strSQL &= " (r.Date1 <= " & queryDateStart & " AND IsNULL(r.Date2,r.Date1) >= " & queryDateEnd & ") ) "
                        strSQL &= " AND r.Status <> " & eRequestStatus.Denied & " AND r.Status <> " & eRequestStatus.Accepted & " AND r.Status <> " & eRequestStatus.Canceled
                        'strSQL &= " And r.IDEmployee IN (" & strEmployees & ")"
                        dtOverlappedAbsences = CreateDataTable(strSQL)
                        If lrret Is Nothing Then
                            lrret = New DataTable
                            lrret = dtOverlappedAbsences
                        Else
                            AddOverlappingEmployeeRow(lrret, dtOverlappedAbsences, _State)
                        End If

                        ' Solicitudes de vacaciones por horas
                        strSQL = "@SELECT# r.idemployee, e.Name As employeename, e.image, 'Request' as tipo, isnull(r.IDCause,0) as IDCause, r.Date1 as BeginDate, isnull(r.Date2, r.Date1) as finishdate, (@SELECT# TOP 1 BeginTime from  sysroRequestDays with (nolock) WHERE r.ID = sysroRequestDays.IDRequest) as begintime, (@SELECT# TOP 1 EndTime from  sysroRequestDays with (nolock) WHERE r.ID = sysroRequestDays.IDRequest)  as endtime, (@SELECT# TOP 1 Duration from  sysroRequestDays with (nolock) WHERE r.ID = sysroRequestDays.IDRequest) as duration ,(@SELECT# count(*) from  sysroRequestDays with (nolock) WHERE r.ID = sysroRequestDays.IDRequest) as cantidaddias , r.RequestType , r.id as idrequest  , (@SELECT# TOP 1 isnull(AllDay,0) from  sysroRequestDays with (nolock) WHERE r.ID = sysroRequestDays.IDRequest) as AllDay "
                        strSQL &= " FROM Requests r with (nolock)  INNER JOIN employees e with (nolock)  ON e.id = r.idemployee "
                        strSQL &= " INNER JOIN " & strEmployees & " ON e.id = Z.idemployee  "
                        strSQL &= " WHERE (r.RequestType = " & eRequestType.PlannedHolidays & ") AND"
                        strSQL &= " EXISTS ( @SELECT# 1 as ExistRec  from  sysroRequestDays with (nolock) WHERE r.ID = sysroRequestDays.IDRequest "
                        strSQL &= " AND "
                        strSQL &= " sysroRequestDays.Date>=" & queryDateStart & " AND  sysroRequestDays.Date<=" & queryDateEnd & ") "
                        strSQL &= " AND r.Status <> " & eRequestStatus.Denied & " AND r.Status <> " & eRequestStatus.Accepted & " AND r.Status <> " & eRequestStatus.Canceled
                        'strSQL &= " And r.IDEmployee IN (" & strEmployees & ")"
                        dtOverlappedAbsences = CreateDataTable(strSQL)
                        AddOverlappingEmployeeRow(lrret, dtOverlappedAbsences, _State)

                        ' Solicitud de vacaciones por dias
                        strSQL = "@SELECT# r.idemployee, e.Name as employeename, e.image, 'Request' as tipo, isnull(r.IDShift,0) as IDCause, r.Date1 as BeginDate, isnull(r.Date2, r.Date1) as finishdate, r.FromTime as begintime, isnull(r.ToTime,r.FromTime) as endtime, CONVERT(NUMERIC(8, 6), isnull(r.Hours,0)) as duration ,(@SELECT# count(*) from  sysroRequestDays with (nolock) WHERE r.ID = sysroRequestDays.IDRequest) as cantidaddias , r.RequestType , r.id as idrequest  , NULL as AllDay "
                        strSQL &= " FROM Requests r with (nolock)  INNER JOIN employees e with (nolock)  ON e.id = r.idemployee  "
                        strSQL &= " INNER JOIN " & strEmployees & " ON e.id = Z.idemployee  "
                        strSQL &= " WHERE (r.RequestType = " & eRequestType.VacationsOrPermissions & ") AND"
                        strSQL &= " EXISTS ( @SELECT# 1 as ExistRec  from  sysroRequestDays with (nolock) WHERE r.ID = sysroRequestDays.IDRequest "
                        strSQL &= " AND "
                        strSQL &= " sysroRequestDays.Date>=" & queryDateStart & " AND sysroRequestDays.Date<=" & queryDateEnd & ") "
                        strSQL &= " AND r.Status <> " & eRequestStatus.Denied & " AND r.Status <> " & eRequestStatus.Accepted & " AND r.Status <> " & eRequestStatus.Canceled
                        'strSQL &= " And r.IDEmployee IN (" & strEmployees & ")"
                        dtOverlappedAbsences = CreateDataTable(strSQL)
                        AddOverlappingEmployeeRow(lrret, dtOverlappedAbsences, _State)

                        ' Previsiones de vacaciones por horas
                        strSQL = "@SELECT# r.idemployee, e.Name as employeename, e.image, 'Holidays Hours' as tipo, isnull(r.IDCause,0) as IDCause, r.Date as BeginDate, r.Date as finishdate, r.BeginTime as begintime, r.EndTime as endtime, CONVERT(NUMERIC(8, 6), isnull(r.Duration,0)) as duration ,1 as cantidaddias , 0 as RequestType , r.id as idrequest , isnull(r.AllDay,0) as AllDay   "
                        strSQL &= " FROM ProgrammedHolidays r with (nolock)   INNER JOIN employees e with (nolock)  ON e.id = r.idemployee  "
                        strSQL &= " INNER JOIN " & strEmployees & " ON e.id = Z.idemployee  "
                        strSQL &= " WHERE Date> = " & queryDateStart & " AND  Date<=" & queryDateEnd
                        'strSQL &= " AND IDEmployee IN (" & strEmployees & ") "
                        dtOverlappedAbsences = CreateDataTable(strSQL)
                        AddOverlappingEmployeeRow(lrret, dtOverlappedAbsences, _State)

                        ' Dias planificados de vacaciones
                        strSQL = "@SELECT# r.idemployee, e.Name as employeename, (@SELECT# image from employees z where z.id = e.id ) as Image, 'Holidays' as tipo, NULL as IDCause, MIN(r.Date) as BeginDate, MAX(r.Date) as finishdate, NULL as begintime, NULL as endtime, NULL as duration ,Count(*)  as cantidaddias , 0 as RequestType , 0  as idrequest , NULL as AllDay "
                        strSQL &= " FROM DailySchedule r with (nolock)   INNER JOIN employees e with (nolock)  ON e.id = r.idemployee  "
                        strSQL &= " INNER JOIN " & strEmployees & " ON e.id = Z.idemployee  "
                        strSQL &= " WHERE isnull(r.IsHolidays,0) = 1  AND  r.Date>=  " & queryDateStart & " AND  r.Date<=" & queryDateEnd
                        'strSQL &= " AND r.IDEmployee IN (" & strEmployees & ") "
                        strSQL &= " GROUP BY r.idemployee,e.Name, e.id"
                        dtOverlappedAbsences = CreateDataTable(strSQL)
                        AddOverlappingEmployeeRow(lrret, dtOverlappedAbsences, _State)

                        ' Previsiones de ausencias por dias
                        strSQL = "@SELECT# r.idemployee, e.Name as employeename, e.image, 'Days' as tipo, isnull(r.IDCause,0) as IDCause, r.BeginDate as BeginDate, ISNULL(r.FinishDate, DATEADD(DAY,r.MaxLastingDays - 1,r.BeginDate )) as finishdate, NULL as begintime, NULL as endtime, NULL as duration ,DATEDIFF(day,r.BeginDate,ISNULL(r.FinishDate, DATEADD(DAY,r.MaxLastingDays - 1,r.BeginDate )))+1 as cantidaddias , 0 as RequestType , r.AbsenceID as idrequest , NULL as AllDay "
                        strSQL &= " FROM ProgrammedAbsences r with (nolock)   INNER JOIN employees e with (nolock)  ON e.id = r.idemployee  "
                        strSQL &= " INNER JOIN " & strEmployees & " ON e.id = Z.idemployee  "
                        strSQL &= " WHERE ( (r.BeginDate >= " & queryDateStart & " AND r.BeginDate <= " & queryDateEnd & ")"
                        strSQL &= " OR "
                        strSQL &= " (ISNULL(r.FinishDate, DATEADD(DAY,r.MaxLastingDays - 1,r.BeginDate )) >= " & queryDateStart & " AND ISNULL(r.FinishDate, DATEADD(DAY,r.MaxLastingDays - 1,r.BeginDate )) <= " & queryDateEnd & ")"
                        strSQL &= " OR "
                        strSQL &= " (r.BeginDate <= " & queryDateStart & " AND ISNULL(r.FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateEnd & ") ) "
                        'strSQL &= " And r.IDEmployee IN (" & strEmployees & ")"
                        dtOverlappedAbsences = CreateDataTable(strSQL)
                        AddOverlappingEmployeeRow(lrret, dtOverlappedAbsences, _State)

                        ' Previsiones de ausencia por horas
                        strSQL = "@SELECT# r.idemployee, e.Name as employeename, e.image, 'Hours' as tipo, isnull(r.IDCause,0) as IDCause, r.Date as BeginDate, ISNULL(r.FinishDate, r.Date) as finishdate, r.BeginTime as begintime, r.EndTime as endtime, r.Duration as duration ,DATEDIFF(day,r.Date,ISNULL(r.FinishDate, r.Date)) + 1 as cantidaddias , 0 as RequestType , r.AbsenceID as idrequest  , NULL as AllDay "
                        strSQL &= " FROM ProgrammedCauses r with (nolock)   INNER JOIN employees e with (nolock)  ON e.id = r.idemployee  "
                        strSQL &= " INNER JOIN " & strEmployees & " ON e.id = Z.idemployee  "
                        strSQL &= " WHERE ( (r.Date >= " & queryDateStart & " AND r.Date <= " & queryDateEnd & ")"
                        strSQL &= " OR "
                        strSQL &= " (IsNULL(r.FinishDate,r.Date) >= " & queryDateStart & " AND IsNULL(r.FinishDate,r.Date) <= " & queryDateEnd & ")"
                        strSQL &= " OR "
                        strSQL &= " (r.Date <= " & queryDateStart & " AND IsNULL(FinishDate,Date) >= " & queryDateEnd & ") ) "
                        'strSQL &= " And r.IDEmployee IN (" & strEmployees & ")"
                        dtOverlappedAbsences = CreateDataTable(strSQL)
                        AddOverlappingEmployeeRow(lrret, dtOverlappedAbsences, _State)
                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequest::GetOverlappingEmployees")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequest::GetOverlappingEmployees")
            Finally

            End Try

            If lrret Is Nothing Then lrret = New DataTable
            Return lrret
        End Function

        Public Shared Function AddOverlappingEmployeeRow(ByRef dtOverlappedAbsences As DataTable, ByVal dtAbsences As DataTable, ByVal _State As roRequestState) As Boolean
            Dim lrret As Boolean = True
            Try

                If dtAbsences IsNot Nothing AndAlso dtAbsences.Rows.Count > 0 Then
                    Dim oRowOverLapped() As DataRow = Nothing
                    For Each orow As DataRow In dtAbsences.Rows
                        If orow("tipo") <> "Holidays" Then
                            ' En el caso que el tipo no sea dias planificados de Vacaciones, añadimos el registro en caso que no exista
                            oRowOverLapped = dtOverlappedAbsences.Select("idrequest = " & orow("idrequest") & " And tipo = '" & orow("tipo") & "'")
                        Else
                            ' En el caso que sean dias planificados de Vacaciones, añadimos todos los dias al mismo registro si ya existe, si no existe lo creamos
                            oRowOverLapped = dtOverlappedAbsences.Select("idrequest = " & orow("idrequest") & " And tipo = '" & orow("tipo") & "' AND idemployee = " & orow("idemployee"))
                        End If

                        If oRowOverLapped Is Nothing OrElse oRowOverLapped.Length = 0 Then
                            Dim aRow As DataRow = dtOverlappedAbsences.NewRow()
                            aRow("idemployee") = orow("idemployee")
                            aRow("employeename") = orow("employeename")
                            aRow("image") = orow("image")
                            aRow("tipo") = orow("tipo")
                            aRow("IDCause") = orow("IDCause")
                            aRow("BeginDate") = orow("BeginDate")
                            aRow("finishdate") = orow("finishdate")
                            aRow("begintime") = orow("begintime")
                            aRow("endtime") = orow("endtime")
                            aRow("duration") = orow("duration")
                            aRow("cantidaddias") = orow("cantidaddias")
                            aRow("RequestType") = orow("RequestType")
                            aRow("idrequest") = orow("idrequest")
                            aRow("AllDay") = orow("AllDay")
                            dtOverlappedAbsences.Rows.Add(aRow)
                            dtOverlappedAbsences.AcceptChanges()
                        ElseIf orow("tipo") = "Holidays" Then
                            ' en el caso de dias de vacaciones , si ya existe un registro añadimos el día y ampliamos el periodo de fechas
                            oRowOverLapped(0)("BeginDate") = IIf(orow("BeginDate") < oRowOverLapped(0)("BeginDate"), orow("BeginDate"), oRowOverLapped(0)("BeginDate"))
                            oRowOverLapped(0)("finishdate") = IIf(orow("finishdate") > oRowOverLapped(0)("finishdate"), orow("finishdate"), oRowOverLapped(0)("finishdate"))
                            oRowOverLapped(0)("cantidaddias") = oRowOverLapped(0)("cantidaddias") + orow("cantidaddias")
                            dtOverlappedAbsences.AcceptChanges()
                        End If
                    Next
                End If

                lrret = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequest::AddOverlappingEmployeeRow")
                lrret = False
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequest::AddOverlappingEmployeeRow")
                lrret = False
            Finally
            End Try

            Return lrret
        End Function

#End Region

    End Class

    Public Class SelectFieldComparer
        Implements IEqualityComparer(Of SelectField)

        Public Shadows Function Equals(x As SelectField, y As SelectField) As Boolean Implements IEqualityComparer(Of SelectField).Equals
            Return x.FieldValue = y.FieldValue
        End Function

        Public Shadows Function GetHashCode(obj As SelectField) As Integer Implements IEqualityComparer(Of SelectField).GetHashCode
            Return obj.FieldValue.GetHashCode
        End Function

    End Class

#End Region

#Region "CLASS - roRequestTypeSecurity"

    <DataContract>
    Public Class roRequestTypeSecurity

#Region "Declarations - Constructor"

        Private oState As roRequestState

        Private oRequestType As eRequestType
        Private strEmployeeFeatureName
        Private strSupervisorFeatureName As String
        Private strSupervisorFeatureEmployeesName As String
        Private intSupervisorFeatureEmployeesID As Integer

        Public Sub New()
            Me.oState = New roRequestState

        End Sub

        Public Sub New(ByVal _RequestType As eRequestType, ByVal _State As roRequestState)

            Me.oState = _State

            Me.oRequestType = _RequestType
            Select Case Me.oRequestType
                Case eRequestType.None ' Cambio campos de la ficha
                    Me.strEmployeeFeatureName = "None"
                    Me.strSupervisorFeatureName = "None"
                    Me.strSupervisorFeatureEmployeesName = "None"
                Case eRequestType.UserFieldsChange ' Cambio campos de la ficha
                    Me.strEmployeeFeatureName = "UserFields.Requests"
                    Me.strSupervisorFeatureName = "Employees.UserFields.Requests"
                    Me.strSupervisorFeatureEmployeesName = "Employees"
                Case eRequestType.ForbiddenPunch ' Marcaje olvidado
                    Me.strEmployeeFeatureName = "Punches.Requests.Forgotten"
                    Me.strSupervisorFeatureName = "Calendar.Punches.Requests.Forgotten"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.JustifyPunch ' Justificación marcaje existente
                    Me.strEmployeeFeatureName = "Punches.Requests.Justify"
                    Me.strSupervisorFeatureName = "Calendar.Punches.Requests.Justify"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.ExternalWorkResumePart ' Parte de trabajo externo
                    Me.strEmployeeFeatureName = "Punches.Requests.ExternalParts"
                    Me.strSupervisorFeatureName = "Calendar.Punches.Requests.ExternalParts"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.ChangeShift ' Cambio de horario
                    Me.strEmployeeFeatureName = "Planification.Requests.ShiftChange"
                    Me.strSupervisorFeatureName = "Calendar.Requests.ShiftChange"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.VacationsOrPermissions ' Vacaciones o permisos
                    Me.strEmployeeFeatureName = "Planification.Requests.Vacations"
                    Me.strSupervisorFeatureName = "Calendar.Requests.Vacations"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.PlannedAbsences ' Ausencias previstas
                    Me.strEmployeeFeatureName = "Planification.Requests.PlannedAbsence"
                    Me.strSupervisorFeatureName = "Calendar.Requests.PlannedAbsence"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.PlannedCauses  ' Incidencias previstas
                    Me.strEmployeeFeatureName = "Planification.Requests.PlannedCause"
                    Me.strSupervisorFeatureName = "Calendar.Requests.PlannedCause"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.PlannedHolidays  ' Vacaciones por horas
                    Me.strEmployeeFeatureName = "Planification.Requests.PlannedHour"
                    Me.strSupervisorFeatureName = "Calendar.Requests.PlannedHour"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.PlannedOvertimes
                    Me.strEmployeeFeatureName = "Planification.Requests.PlannedOvertime"
                    Me.strSupervisorFeatureName = "Calendar.Requests.PlannedOvertime"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.ExchangeShiftBetweenEmployees ' Intercambio de horarios entre empleados
                    Me.strEmployeeFeatureName = "Planification.Requests.ShiftExchange"
                    Me.strSupervisorFeatureName = "Calendar.Requests.ShiftExchange"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.ForbiddenTaskPunch ' Fichaje olvidado tareas
                    Me.strEmployeeFeatureName = "TaskPunches.Requests.Forgotten"
                    Me.strSupervisorFeatureName = "Tasks.Requests.Forgotten"
                    Me.strSupervisorFeatureEmployeesName = "Tasks"
                Case eRequestType.CancelHolidays
                    Me.strEmployeeFeatureName = "Planification.Requests.Vacations"
                    Me.strSupervisorFeatureName = "Calendar.Requests.CancelHolidays"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.ForgottenCostCenterPunch ' Marcaje olvidado de centro de costes
                    Me.strEmployeeFeatureName = "Punches.Requests.CostCenterForgotten"
                    Me.strSupervisorFeatureName = "Calendar.Punches.Requests.CostCenterForgotten"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.ExternalWorkWeekResume ' Marcaje olvidado de centro de costes
                    Me.strEmployeeFeatureName = "Punches.Requests.ExternalWorkWeekResume"
                    Me.strSupervisorFeatureName = "Calendar.Punches.Requests.ExternalWorkWeekResume"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.Telecommute ' Marcaje olvidado de centro de costes
                    Me.strEmployeeFeatureName = "None"
                    Me.strSupervisorFeatureName = "Calendar.Requests.Telecommute"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"
                Case eRequestType.DailyRecord
                    Me.strEmployeeFeatureName = "Punches.DailyRecord"
                    Me.strSupervisorFeatureName = "Calendar.DailyRecord"
                    Me.strSupervisorFeatureEmployeesName = "Calendar"

            End Select

            Me.intSupervisorFeatureEmployeesID = Me.GetSupervisorFeatureEmployeesID()

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Obtiene el tipo de solicitud actual.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        <DataMember>
        Public Property RequestType() As eRequestType
            Get
                Return Me.oRequestType
            End Get
            Set(ByVal value As eRequestType)
                Me.oRequestType = value
            End Set
        End Property

        ''' <summary>
        ''' Obtiene el alias del permiso de empleado correspondiente al tipo de solicitud.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        <DataMember>
        Public Property EmployeeFeatureName() As String
            Get
                Return Me.strEmployeeFeatureName
            End Get
            Set(ByVal value As String)
                Me.strEmployeeFeatureName = value
            End Set
        End Property

        ''' <summary>
        ''' Ibtiene el alias del permiso del supervisor correspondiente al tipo de solicitud.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        <DataMember>
        Public Property SupervisorFeatureName() As String
            Get
                Return Me.strSupervisorFeatureName
            End Get
            Set(ByVal value As String)
                Me.strSupervisorFeatureName = value
            End Set
        End Property

        ''' <summary>
        ''' Obtiene el alias del permiso para obtener los empleados que puede supervisar el supervisor.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        <DataMember>
        Public Property SupervisorFeatureEmployeesName() As String
            Get
                Return Me.strSupervisorFeatureEmployeesName
            End Get
            Set(ByVal value As String)
                Me.strSupervisorFeatureEmployeesName = value
            End Set
        End Property

        ''' <summary>
        ''' Obtiene el id del permiso para obtener los empleados que puede supervisar el supervisor.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>

        <DataMember>
        Public Property SupervisorFeatureEmployeesID() As Integer
            Get
                Return Me.intSupervisorFeatureEmployeesID
            End Get
            Set(ByVal value As Integer)
                Me.intSupervisorFeatureEmployeesID = value
            End Set
        End Property

#End Region

#Region "Methods"

        Private Function GetSupervisorFeatureEmployeesID() As Integer

            Dim intRet As Integer = 0

            Try

                Dim strSQL As String = "@SELECT# ID FROM sysroFeatures WHERE Alias = '" & Me.strSupervisorFeatureEmployeesName & "'"
                intRet = Any2Integer(ExecuteScalar(strSQL))
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roRequestTypeSecurity::GetSupervisorFeatureEmployeesID")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roRequestTypeSecurity::GetSupervisorFeatureEmployeesID")
            Finally

            End Try

            Return intRet

        End Function

        'Public Shared Function CreateList(ByVal _State As roRequestState) As Generic.List(Of roRequestTypeSecurity)

        '    Dim oRet As New Generic.List(Of roRequestTypeSecurity)

        '    Dim oItem As roRequestTypeSecurity = Nothing
        '    For Each oRequestType As eRequestType In System.Enum.GetValues(GetType(eRequestType))
        '        oItem = New roRequestTypeSecurity(oRequestType, _State)
        '        oRet.Add(oItem)
        '    Next

        '    Return oRet

        'End Function

#End Region

    End Class

#End Region

#Region "CLASS - roRequestApproval"

    <DataContract>
    Public Class roRequestApproval

#Region "Properties - Contructor"

        Private oState As roRequestState

        Private intIDRequest As Integer
        Private intIDPassport As Integer
        Private xDateTime As DateTime
        Private intStatus As eRequestStatus
        Private intStatusLevel As Integer
        Private strComments As String

        Private intIDRequestOriginal As Integer
        Private intIDPassportOriginal As Integer
        Private xDateTimeOriginal As DateTime

        Public Sub New()
            Me.oState = New roRequestState
            Me.intIDRequest = -1
            Me.intIDPassport = -1
        End Sub

        Public Sub New(ByVal _State As roRequestState)
            Me.oState = _State
            Me.intIDRequest = -1
            Me.intIDPassport = -1
        End Sub

        Public Sub New(ByVal _IDRequest As Integer, ByVal _IDPassport As Integer, ByVal _DateTime As DateTime, ByVal _State As roRequestState)
            Me.oState = _State
            Me.intIDRequest = _IDRequest
            Me.intIDPassport = _IDPassport
            Me.xDateTime = _DateTime
            Me.Load()
        End Sub

        Public Sub New(ByVal _DataRow As DataRow, ByVal _State As roRequestState)
            Me.oState = _State
            Me.LoadFromRow(_DataRow)
        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property IDRequest() As Integer
            Get
                Return Me.intIDRequest
            End Get
            Set(ByVal value As Integer)
                Me.intIDRequest = value
            End Set
        End Property

        <DataMember>
        Public Property IDPassport() As Integer
            Get
                Return Me.intIDPassport
            End Get
            Set(ByVal value As Integer)
                Me.intIDPassport = value
            End Set
        End Property

        <DataMember>
        Public Property ApprovalDateTime() As DateTime
            Get
                Return Me.xDateTime
            End Get
            Set(ByVal value As DateTime)
                Me.xDateTime = value
            End Set
        End Property

        <DataMember>
        Public Property Status() As eRequestStatus
            Get
                Return Me.intStatus
            End Get
            Set(ByVal value As eRequestStatus)
                Me.intStatus = value
            End Set
        End Property

        <DataMember>
        Public Property StatusLevel() As Integer
            Get
                Return Me.intStatusLevel
            End Get
            Set(ByVal value As Integer)
                Me.intStatusLevel = value
            End Set
        End Property

        <DataMember>
        Public Property Comments() As String
            Get
                Return Me.strComments
            End Get
            Set(ByVal value As String)
                Me.strComments = value
            End Set
        End Property

        <DataMember>
        Public Property OriginalIDRequest() As Integer
            Get
                Return Me.intIDRequestOriginal
            End Get
            Set(ByVal value As Integer)
                Me.intIDRequestOriginal = value
            End Set
        End Property

        <DataMember>
        Public Property OriginalIDPassport() As Integer
            Get
                Return Me.intIDPassportOriginal
            End Get
            Set(ByVal value As Integer)
                Me.intIDPassportOriginal = value
            End Set
        End Property

        <DataMember>
        Public Property OriginalApprovalDateTime() As DateTime
            Get
                Return Me.xDateTimeOriginal
            End Get
            Set(ByVal value As DateTime)
                Me.xDateTimeOriginal = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Try

                Me.intIDRequestOriginal = Me.intIDRequest
                Me.intIDPassportOriginal = Me.intIDPassport
                Me.xDateTimeOriginal = Me.xDateTime

                Dim strSQL As String = "@SELECT# * FROM RequestsApprovals " &
                                       "WHERE IDRequest = " & Me.intIDRequestOriginal.ToString & " AND " &
                                             "IDPassport = " & Me.intIDPassportOriginal.ToString & " AND " &
                                             "CONVERT(varchar, [DateTime], 120) = '" & Format(Me.xDateTimeOriginal, "yyyy-MM-dd HH:mm:ss") & "'"
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.intIDRequest = oRow("IDRequest")
                    Me.intIDPassport = oRow("IDPassport")
                    Me.xDateTime = oRow("DateTime")
                    Me.intStatus = oRow("Status")
                    Me.intStatusLevel = oRow("StatusLevel")
                    Me.strComments = Any2String(oRow("Comments"))

                    bolRet = True

                    ' Auditar lectura
                    'If _Audit Then
                    '    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    '    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tRequestApproval, "", tbParameters, -1)
                    'End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roRequestApproval::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestApproval::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Private Sub LoadFromRow(ByVal _DataRow As DataRow)

            Try

                Me.intIDRequest = _DataRow("IDRequest")
                Me.intIDPassport = _DataRow("IDPassport")
                Me.xDateTime = _DataRow("DateTime")
                Me.intStatus = _DataRow("Status")
                Me.intStatusLevel = _DataRow("StatusLevel")
                Me.strComments = Any2String(_DataRow("Comments"))

                Me.intIDRequestOriginal = Me.intIDRequest
                Me.intIDPassportOriginal = Me.intIDPassport
                Me.xDateTimeOriginal = Me.xDateTime
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roRequestApproval::LoadFromRow")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestApproval::LoadFromRow")
            End Try

        End Sub

        Public Function Save(ByVal IDEmployee As Integer, ByVal iRequestType As eRequestType, Optional ByVal _Audit As Boolean = True) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                Dim IsNewRow As Boolean = False

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim tb As New DataTable("RequestsApprovals")
                Dim strSQL As String = "@SELECT# * FROM RequestsApprovals " &
                                       "WHERE IDRequest = " & Me.intIDRequestOriginal.ToString & " AND " &
                                             "IDPassport = " & Me.intIDPassportOriginal.ToString & " AND " &
                                             "CONVERT(varchar, [DateTime], 120) = '" & Format(Me.xDateTimeOriginal, "yyyy-MM-dd HH:mm:ss") & "'"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    IsNewRow = True
                    oRow = tb.NewRow
                    oRow("IDRequest") = Me.intIDRequest
                    oRow("IDPassport") = Me.intIDPassport
                    oRow("DateTime") = Me.xDateTime
                Else
                    oRow = tb.Rows(0)
                End If

                oRow("IDRequest") = Me.intIDRequest
                oRow("IDPassport") = Me.intIDPassport
                oRow("DateTime") = Me.xDateTime
                oRow("Status") = Me.intStatus
                oRow("StatusLevel") = Me.intStatusLevel
                oRow("Comments") = Me.strComments

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                bolRet = True

                'Auditar aprobacion/denegacion de la solicitud
                If bolRet And _Audit Then

                    Dim oPassport As roPassportTicket
                    Dim strObjectName As String = ""
                    Dim strLanguageKey As String = "ESP"
                    Dim intPriority As Integer = 1

                    Try
                        oPassport = roPassportManager.GetPassportTicket(Me.intIDPassport, LoadType.Passport)
                        strObjectName = oPassport.Name
                        strLanguageKey = oPassport.Language.Key
                    Catch
                    End Try

                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()

                    'Estado
                    Select Case Me.intStatus
                        Case eRequestStatus.Accepted
                            Extensions.roAudit.AddParameter(tbAuditParameters, "{Status}", Me.oState.Language.Translate("RequestInfo.RequestStatus.Accepted", "").ToUpper, "", intPriority)
                        Case eRequestStatus.Denied
                            Extensions.roAudit.AddParameter(tbAuditParameters, "{Status}", Me.oState.Language.Translate("RequestInfo.RequestStatus.Denied", "").ToUpper, "", intPriority)
                        Case eRequestStatus.OnGoing
                            Extensions.roAudit.AddParameter(tbAuditParameters, "{Status}", Me.oState.Language.Translate("RequestInfo.RequestStatus.OnGoing", "").ToUpper, "", intPriority)
                        Case eRequestStatus.Pending
                            Extensions.roAudit.AddParameter(tbAuditParameters, "{Status}", Me.oState.Language.Translate("RequestInfo.RequestStatus.Pending", "").ToUpper, "", intPriority)
                        Case Else
                    End Select
                    intPriority += 1

                    'Empleado
                    Try

                        Extensions.roAudit.AddParameter(tbAuditParameters, "{EmployeeName}", strObjectName, "", intPriority)
                        intPriority += 1
                    Catch
                    End Try

                    'Tipo Solicitud
                    Dim strRequestTypeName As String = String.Empty
                    Try
                        Dim oLang As New roLanguage
                        oLang.SetLanguageReference("LiveRequests", strLanguageKey)
                        strRequestTypeName = oLang.Translate("Requests.RequestType." & System.Enum.GetName(GetType(eRequestType), iRequestType), "Requests")
                    Catch
                    End Try
                    Extensions.roAudit.AddParameter(tbAuditParameters, "{RequestTypeName}", strRequestTypeName, "", intPriority)
                    intPriority += 1

                    'Comentario
                    Extensions.roAudit.AddParameter(tbAuditParameters, "{Comments}", Me.strComments, "", intPriority)

                    Dim oAuditAction As Audit.Action = IIf(IsNewRow = True, Audit.Action.aInsert, Audit.Action.aUpdate)
                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tRequestApproval, strObjectName, tbAuditParameters, -1)

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequestApproval::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestApproval::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal _Audit As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                'Borramos la solicitud
                Dim DelQuerys() As String = {"@DELETE# FROM RequestsApprovals " &
                                             "WHERE IDRequest = " & Me.intIDRequest.ToString & " AND " &
                                                   "IDPassport = " & Me.intIDPassport.ToString & " AND " &
                                                   "CONVERT(varchar, [DateTime], 120) = '" & Format(Me.xDateTime, "yyyy-MM-dd HH:mm:ss") & "'"}
                For n As Integer = 0 To DelQuerys.Length - 1
                    If Not ExecuteSql(DelQuerys(n)) Then
                        oState.Result = RequestResultEnum.ConnectionError
                        Exit For
                    End If
                Next

                bolRet = (oState.Result = RequestResultEnum.NoError)

                'If bolRet And _Audit Then
                '    ' Auditamos
                '    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tRequestApproval, "", Nothing, -1, oTrans.Connection)
                'End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequestApproval::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestApproval::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetRequestApprovals(ByVal _IDRequest As Integer, ByVal _State As roRequestState) As Generic.List(Of roRequestApproval)

            Dim oRet As New Generic.List(Of roRequestApproval)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM RequestsApprovals " &
                                       "WHERE IDRequest = " & _IDRequest.ToString & " " &
                                       "ORDER BY DateTime"
                Dim tb As DataTable = CreateDataTable(strSQL, )

                Dim oRequestApproval As roRequestApproval = Nothing
                For Each oRow As DataRow In tb.Rows
                    oRequestApproval = New roRequestApproval(oRow("IDRequest"), oRow("IDPassport"), oRow("DateTime"), _State)
                    oRequestApproval.Comments = Any2String(oRow("Comments"))
                    oRet.Add(oRequestApproval)
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestApproval::GetRequestApprovals")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestApproval::GetRequestApprovals")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveRequestApprovals(ByVal _IDRequest As Integer, ByVal _IDEmployee As Integer, ByVal iRequestType As eRequestType,
                                                    ByVal oRequestApprovals As Generic.List(Of roRequestApproval), ByVal _State As roRequestState) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos las reglas del convenio actuales
                Dim OldRequestApprovals As Generic.List(Of roRequestApproval) = GetRequestApprovals(_IDRequest, _State)

                Dim IDRequestApprovalsSaved As New Generic.List(Of String)
                If oRequestApprovals IsNot Nothing Then
                    bolRet = True
                    For Each oRequestApproval As roRequestApproval In oRequestApprovals
                        oRequestApproval.IDRequest = _IDRequest
                        bolRet = oRequestApproval.Save(_IDEmployee, iRequestType)
                        If Not bolRet Then Exit For
                        IDRequestApprovalsSaved.Add(oRequestApproval.IDRequest.ToString & "*" & oRequestApproval.IDPassport.ToString & "*" & Format(oRequestApproval.ApprovalDateTime, "dd/MM/yyyy"))
                    Next
                Else
                    bolRet = True
                End If

                If bolRet Then
                    ' Borramos las aprovaciones de la tabla que no esten en la lista y las borramos
                    For Each oRequestApproval As roRequestApproval In OldRequestApprovals
                        If ExistRequestApprovalInList(oRequestApprovals, oRequestApproval) Is Nothing Then
                            bolRet = oRequestApproval.Delete()
                            If Not bolRet Then Exit For
                        End If
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestApproval::SaveRequestApprovals")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestApproval::SaveRequestApprovals")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteRequestApprovals(ByVal _IDRequest As Integer, ByVal _State As roRequestState) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Obtenemos las aprovaciones de la solicitud actuales
                Dim OldRequestApprovals As Generic.List(Of roRequestApproval) = GetRequestApprovals(_IDRequest, _State)

                bolRet = True
                For Each oRequestApproval As roRequestApproval In OldRequestApprovals
                    bolRet = oRequestApproval.Delete()
                    If Not bolRet Then Exit For
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestApproval::DeleteRequestApprovals")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestApproval::DeleteRequestApprovals")
            End Try

            Return bolRet

        End Function

        Private Shared Function ExistRequestApprovalInList(ByVal lstRequestApprovals As Generic.List(Of roRequestApproval), ByVal oRequestApproval As roRequestApproval) As roRequestApproval

            Dim oRet As roRequestApproval = Nothing

            If lstRequestApprovals IsNot Nothing Then

                For Each oItem As roRequestApproval In lstRequestApprovals
                    If oItem.IDRequest = oRequestApproval.IDRequest And
                       oItem.IDPassport = oRequestApproval.IDPassport And
                       oItem.ApprovalDateTime = oRequestApproval.ApprovalDateTime Then
                        oRet = oItem
                        Exit For
                    End If
                Next

            End If

            Return oRet

        End Function

#End Region

    End Class

#End Region

#Region "CLASS - roRequestDay"

    <DataContract>
    Public Class roRequestDay

#Region "Properties - Contructor"

        Private oState As roRequestState

        Private intIDRequest As Integer
        Private xDate As Date
        Private bolAllDay As Nullable(Of Boolean)
        Private xFromTime As Nullable(Of DateTime)
        Private xToTime As Nullable(Of DateTime)
        Private dblDuration As Nullable(Of Double)

        Private intActualType As Nullable(Of PunchTypeEnum)
        Private intIDCause As Nullable(Of Integer)
        Private strComments As String

        Public Sub New()
            Me.oState = New roRequestState
            Me.intIDRequest = -1
            Me.strComments = String.Empty
        End Sub

        Public Sub New(ByVal _State As roRequestState)
            Me.oState = _State
            Me.intIDRequest = -1
            Me.strComments = String.Empty

        End Sub

        Public Sub New(ByVal _IDRequest As Integer, ByVal _Date As Date, ByVal _State As roRequestState)
            Me.oState = _State
            Me.intIDRequest = _IDRequest
            Me.xDate = _Date
            Me.strComments = String.Empty
            Me.Load()
        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property IDRequest() As Integer
            Get
                Return Me.intIDRequest
            End Get
            Set(ByVal value As Integer)
                Me.intIDRequest = value
            End Set
        End Property

        <DataMember>
        Public Property RequestDate() As Date
            Get
                Return Me.xDate
            End Get
            Set(ByVal value As Date)
                Me.xDate = value
            End Set
        End Property

        <DataMember>
        Public Property AllDay() As Nullable(Of Boolean)
            Get
                Return Me.bolAllDay
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bolAllDay = value
            End Set
        End Property

        <DataMember>
        Public Property FromTime() As Nullable(Of DateTime)
            Get
                Return Me.xFromTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xFromTime = value
            End Set
        End Property

        <DataMember>
        Public Property ToTime() As Nullable(Of DateTime)
            Get
                Return Me.xToTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xToTime = value
            End Set
        End Property

        <DataMember>
        Public Property Duration() As Nullable(Of Double)
            Get
                Return dblDuration
            End Get
            Set(ByVal value As Nullable(Of Double))
                dblDuration = value
            End Set
        End Property

        <DataMember>
        Public Property ActualType() As Nullable(Of PunchTypeEnum)
            Get
                Return intActualType
            End Get
            Set(ByVal value As Nullable(Of PunchTypeEnum))
                intActualType = value
            End Set
        End Property

        <DataMember>
        Public Property IDCause() As Nullable(Of Integer)
            Get
                Return intIDCause
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDCause = value
            End Set
        End Property

        <DataMember>
        Public Property Comments() As String
            Get
                Return strComments
            End Get
            Set(ByVal value As String)
                strComments = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroRequestDays " &
                                       "WHERE IDRequest = " & Me.intIDRequest.ToString & " AND " &
                                             "Date = " & Any2Time(Me.xDate).SQLSmallDateTime
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.intIDRequest = oRow("IDRequest")
                    Me.xDate = oRow("Date")
                    If Not IsDBNull(oRow("AllDay")) Then bolAllDay = Any2Boolean(oRow("AllDay"))
                    If Not IsDBNull(oRow("BeginTime")) Then xFromTime = oRow("BeginTime")
                    If Not IsDBNull(oRow("EndTime")) Then xToTime = oRow("EndTime")
                    If Not IsDBNull(oRow("Duration")) Then dblDuration = Any2Double(oRow("Duration"))
                    If Not IsDBNull(oRow("ActualType")) Then intActualType = Any2Integer(oRow("ActualType"))
                    If Not IsDBNull(oRow("IDCause")) Then intIDCause = Any2Integer(oRow("IDCause"))
                    If Not IsDBNull(oRow("Comments")) Then strComments = Any2String(oRow("Comments")).Trim

                    bolRet = True
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roRequestDay::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestDay::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal _Audit As Boolean = True) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                Dim IsNewRow As Boolean = False

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim tb As New DataTable("sysroRequestDays")
                Dim strSQL As String = "@SELECT# * FROM sysroRequestDays " &
                                       "WHERE IDRequest = " & Me.intIDRequest.ToString & " AND " &
                                       "Date = " & Any2Time(Me.xDate).SQLSmallDateTime
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    IsNewRow = True
                    oRow = tb.NewRow
                    oRow("IDRequest") = Me.intIDRequest
                    oRow("Date") = Me.xDate
                Else
                    oRow = tb.Rows(0)
                End If

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If

                If Me.bolAllDay.HasValue Then oRow("AllDay") = Me.bolAllDay
                If Me.xFromTime.HasValue Then oRow("BeginTime") = Me.xFromTime
                If Me.xToTime.HasValue Then oRow("EndTime") = Me.xToTime
                If Me.xFromTime.HasValue And Me.xToTime.HasValue Then oRow("Duration") = Any2Time(Me.xToTime).NumericValue - Any2Time(Me.xFromTime).NumericValue

                If Me.intActualType.HasValue Then oRow("ActualType") = Me.intActualType.Value
                If Me.intIDCause.HasValue Then oRow("IDCause") = Me.intIDCause.Value
                If Me.strComments.Length > 0 Then oRow("Comments") = Me.strComments.Trim

                da.Update(tb)

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequestDay::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestDay::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Delete() As Boolean

            Dim bolRet As Boolean = False

            Try

                'Borramos la fecha de solicitud
                Dim DelQuerys() As String = {"@DELETE# FROM sysroRequestDays " &
                                             "WHERE IDRequest = " & Me.intIDRequest.ToString & " AND " &
                                                   "Date = " & Any2Time(Me.xDate).SQLSmallDateTime}
                For n As Integer = 0 To DelQuerys.Length - 1
                    If Not ExecuteSql(DelQuerys(n)) Then
                        oState.Result = RequestResultEnum.ConnectionError
                        Exit For
                    End If
                Next

                bolRet = (oState.Result = RequestResultEnum.NoError)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequestDay::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestDay::Delete")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetRequestDays(ByVal _IDRequest As Integer, ByVal _State As roRequestState) As Generic.List(Of roRequestDay)

            Dim oRet As New Generic.List(Of roRequestDay)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM sysroRequestDays " &
                                       "WHERE IDRequest = " & _IDRequest.ToString & " " &
                                       "ORDER BY Date"
                Dim tb As DataTable = CreateDataTable(strSQL, )

                Dim oRequestDay As roRequestDay = Nothing
                For Each oRow As DataRow In tb.Rows
                    oRequestDay = New roRequestDay(oRow("IDRequest"), oRow("Date"), _State)
                    oRet.Add(oRequestDay)
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestDay::GetRequestDays")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestDay::GetRequestDays")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveRequestDays(ByVal _IDRequest As Integer,
                                                    ByVal oRequestDays As Generic.List(Of roRequestDay), ByVal _State As roRequestState) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                'Borramos las fechas de la solicitud
                bolRet = DeleteRequestDays(_IDRequest, _State)

                If oRequestDays IsNot Nothing Then
                    bolRet = True
                    For Each oRequestDay As roRequestDay In oRequestDays
                        oRequestDay.IDRequest = _IDRequest
                        bolRet = oRequestDay.Save()
                        If Not bolRet Then Exit For
                    Next
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestDay::SaveRequestDays")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestDay::SaveRequestDays")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteRequestDays(ByVal _IDRequest As Integer, ByVal _State As roRequestState) As Boolean

            Dim bolRet As Boolean = False

            Try

                'Borramos las fechas de la solicitud
                Dim strSQL As String = "@DELETE# FROM sysroRequestDays WHERE IDRequest = " & _IDRequest.ToString

                bolRet = ExecuteSql(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestDay::DeleteRequestApprovals")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestDay::DeleteRequestApprovals")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

#End Region

End Namespace