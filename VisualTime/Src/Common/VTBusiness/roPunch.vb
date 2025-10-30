Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Visitors
Imports Robotics.Base.VTBusiness.Visits
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTNotifications
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace Punch

    <DataContract()>
    Public Class roDoSequencePunch

        <DataMember>
        Public Property Punch As roPunch

        <DataMember>
        Public Property Status As PunchStatus

        <DataMember>
        Public Property SeqStatus As PunchSeqStatus

        <DataMember>
        Public Property Saved As Boolean
    End Class

    <DataContract>
    Public Class roPunch

#Region "Declarations - Constructor"

        Private oState As roPunchState
        Private lngID As Long
        Private lngIDCredential As Long
        Private intIDEmployee As Integer
        Private intType As PunchTypeEnum
        Private intActualType As Nullable(Of PunchTypeEnum)
        Private intInvalidType As Nullable(Of InvalidTypeEnum)
        Private intTypeData As Integer
        Private strTypeDetails As String
        Private xShiftDate As Nullable(Of Date)
        Private xDateTime As Nullable(Of DateTime)
        Private intIDTerminal As Integer
        Private bIDReader As Byte
        Private intIDZone As Integer
        Private strLocation As String
        Private strLocationZone As String
        Private strIP As String
        Private bolIsNotReliable As Boolean
        Private bolZoneIsNotReliable As Boolean = False
        Private intAction As Integer
        Private intIDPassport As Integer
        Private oCapture As Image
        Private strTimeZone As String
        Private bolExported As Boolean
        Private strCRC As String
        Private strFullAddress As String
        Private bMaskAlert As Nullable(Of Boolean)
        Private bTemperatureAlert As Nullable(Of Boolean)
        Private intVerificationType As DTOs.VerificationType
        Private bInTelecommute As Boolean
        Private intIDRequest As Integer
        Private bHasPhoto As Boolean
        Private bPhotoOnAzure As Boolean
        Private strRemarks As String
        Private strNotReliableCause As String

        Private strField1 As String
        Private strField2 As String
        Private strField3 As String
        Private dblField4 As Double
        Private dblField5 As Double
        Private dblField6 As Double

        Private xTimeStamp As Nullable(Of DateTime)

        Private strWorkcenter As String
        Private strSystemDetails As String = String.Empty

        Private intSource As DTOs.PunchSource

        Public Const DEFAULT_ZONE As Integer = 255

        Public Sub New()

            Me.oState = New roPunchState()

            Me.ID = 0
            Me.Load()

        End Sub

        Public Sub New(ByVal _State As roPunchState)
            Me.oState = _State
            Me.ID = 0
            Me.Load()
        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _ID As Long, ByVal _State As roPunchState)

            Me.oState = _State

            Me.intIDEmployee = _IDEmployee

            Me.ID = _ID
            Me.Load()

        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _ID As Long, ByVal bLoadPhoto As Boolean, ByVal _State As roPunchState)

            Me.oState = _State

            Me.intIDEmployee = _IDEmployee

            Me.ID = _ID
            Me.Load(, bLoadPhoto)

        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property Type() As PunchTypeEnum
            Get
                Return Me.intType
            End Get
            Set(ByVal value As PunchTypeEnum)
                Me.intType = value
            End Set
        End Property

        <IgnoreDataMember>
        Public Property State() As roPunchState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roPunchState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property ID() As Long
            Get
                Return Me.lngID
            End Get
            Set(ByVal value As Long)
                Me.lngID = value
                ' Me.Load(oTransaction)
            End Set
        End Property

        <DataMember>
        Public Property IDCredential() As Long
            Get
                Return Me.lngIDCredential
            End Get
            Set(ByVal value As Long)
                Me.lngIDCredential = value
            End Set
        End Property

        <DataMember>
        Public Property VerificationType As DTOs.VerificationType
            Get
                Return intVerificationType
            End Get
            Set(value As DTOs.VerificationType)
                intVerificationType = value
            End Set
        End Property

        <DataMember>
        Public Property IDEmployee() As Integer
            Get
                Return Me.intIDEmployee
            End Get
            Set(ByVal value As Integer)
                Me.intIDEmployee = value
            End Set
        End Property

        <DataMember>
        Public Property ActualType() As Nullable(Of PunchTypeEnum)
            Get
                Return Me.intActualType
            End Get
            Set(ByVal value As Nullable(Of PunchTypeEnum))
                Me.intActualType = value
            End Set
        End Property

        <DataMember>
        Public Property InvalidType() As Nullable(Of InvalidTypeEnum)
            Get
                Return Me.intInvalidType
            End Get
            Set(ByVal value As Nullable(Of InvalidTypeEnum))
                Me.intInvalidType = value
            End Set
        End Property

        <DataMember>
        Public Property ShiftDate() As Nullable(Of DateTime)
            Get
                Return Me.xShiftDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xShiftDate = value
            End Set
        End Property

        <DataMember>
        Public Property DateTime() As Nullable(Of DateTime)
            Get
                Return Me.xDateTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xDateTime = value
            End Set
        End Property

        <DataMember>
        Public Property IDTerminal() As Integer
            Get
                Return Me.intIDTerminal
            End Get
            Set(ByVal value As Integer)
                Me.intIDTerminal = value
            End Set
        End Property

        <DataMember>
        Public Property IDReader() As Byte
            Get
                Return Me.bIDReader
            End Get
            Set(ByVal value As Byte)
                Me.bIDReader = value
            End Set
        End Property

        <DataMember>
        Public Property IDZone() As Integer
            Get
                Return Me.intIDZone
            End Get
            Set(ByVal value As Integer)
                Me.intIDZone = value
            End Set
        End Property

        <DataMember>
        Public Property TypeData() As Integer
            Get
                Return Me.intTypeData
            End Get
            Set(ByVal value As Integer)
                Me.intTypeData = value
            End Set
        End Property

        <DataMember>
        Public Property TypeDetails() As String
            Get
                Return Me.strTypeDetails
            End Get
            Set(ByVal value As String)
                Me.strTypeDetails = value
            End Set
        End Property

        <DataMember>
        Public Property Location() As String
            Get
                Return Me.strLocation
            End Get
            Set(ByVal value As String)
                Me.strLocation = value
            End Set
        End Property

        <DataMember>
        Public Property LocationZone() As String
            Get
                Return Me.strLocationZone
            End Get
            Set(ByVal value As String)
                Me.strLocationZone = value
            End Set
        End Property

        <DataMember>
        Public Property FullAddress() As String
            Get
                Return Me.strFullAddress
            End Get
            Set(ByVal value As String)
                Me.strFullAddress = value
            End Set
        End Property

        <DataMember>
        Public Property TimeZone() As String
            Get
                Return Me.strTimeZone
            End Get
            Set(ByVal value As String)
                Me.strTimeZone = value
            End Set
        End Property

        <DataMember>
        Public Property Exported() As Boolean
            Get
                Return Me.bolExported
            End Get
            Set(ByVal value As Boolean)
                Me.bolExported = value
            End Set
        End Property

        <DataMember>
        Public Property IP() As String
            Get
                Return Me.strIP
            End Get
            Set(ByVal value As String)
                Me.strIP = value
            End Set
        End Property

        <DataMember>
        Public Property IsNotReliable() As Nullable(Of Boolean)
            Get
                Return Me.bolIsNotReliable
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                If value.HasValue Then
                    Me.bolIsNotReliable = value.Value
                Else
                    Me.bolIsNotReliable = False
                End If
            End Set
        End Property

        <DataMember>
        Public Property Action() As Integer
            Get
                Return Me.intAction
            End Get
            Set(ByVal value As Integer)
                Me.intAction = value
            End Set
        End Property

        <DataMember>
        Public Property Passport() As Integer
            Get
                Return Me.intIDPassport
            End Get
            Set(ByVal value As Integer)
                Me.intIDPassport = value
            End Set
        End Property

        <XmlIgnore()>
        <IgnoreDataMember>
        Public Property Capture() As Image
            Get
                Return Me.oCapture
            End Get
            Set(ByVal value As Image)
                Me.oCapture = value
            End Set
        End Property

        Public Function GetCaptureBytes() As Byte()
            Return Image2Bytes(Me.oCapture)
        End Function

        Public Sub SetCaptureBytes(value As Byte())
            Me.oCapture = Bytes2Image(value)
        End Sub

        <DataMember>
        Public Property Field1() As String
            Get
                Return Me.strField1
            End Get
            Set(ByVal value As String)
                Me.strField1 = value
            End Set
        End Property

        <DataMember>
        Public Property Field2() As String
            Get
                Return Me.strField2
            End Get
            Set(ByVal value As String)
                Me.strField2 = value
            End Set
        End Property

        <DataMember>
        Public Property Field3() As String
            Get
                Return Me.strField3
            End Get
            Set(ByVal value As String)
                Me.strField3 = value
            End Set
        End Property

        <DataMember>
        Public Property Field4() As Double
            Get
                Return Me.dblField4
            End Get
            Set(ByVal value As Double)
                Me.dblField4 = value
            End Set
        End Property

        <DataMember>
        Public Property Field5() As Double
            Get
                Return Me.dblField5
            End Get
            Set(ByVal value As Double)
                Me.dblField5 = value
            End Set
        End Property

        <DataMember>
        Public Property Field6() As Double
            Get
                Return Me.dblField6
            End Get
            Set(ByVal value As Double)
                Me.dblField6 = value
            End Set
        End Property

        <DataMember>
        Public Property TimeStamp() As Nullable(Of DateTime)
            Get
                Return Me.xTimeStamp
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xTimeStamp = value
            End Set
        End Property

        <DataMember>
        Public Property CRC() As String
            Get
                Return Me.strCRC
            End Get
            Set(ByVal value As String)
                Me.strCRC = value
            End Set
        End Property

        <DataMember>
        Public Property MaskAlert As Nullable(Of Boolean)
            Get
                Return Me.bMaskAlert
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bMaskAlert = value
            End Set
        End Property

        <DataMember>
        Public Property TemperatureAlert As Nullable(Of Boolean)
            Get
                Return Me.bTemperatureAlert
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bTemperatureAlert = value
            End Set
        End Property

        <DataMember>
        Public Property InTelecommute As Boolean
            Get
                Return Me.bInTelecommute
            End Get
            Set(ByVal value As Boolean)
                Me.bInTelecommute = value
            End Set
        End Property

        <DataMember>
        Public Property HasPhoto As Boolean
            Get
                Return Me.bHasPhoto
            End Get
            Set(ByVal value As Boolean)
                Me.bHasPhoto = value
            End Set
        End Property

        <DataMember>
        Public Property PhotoOnAzure As Boolean
            Get
                Return Me.bPhotoOnAzure
            End Get
            Set(ByVal value As Boolean)
                Me.bPhotoOnAzure = value
            End Set
        End Property

        <DataMember>
        Public Property Workcenter As String
            Get
                Return Me.strWorkcenter
            End Get
            Set(ByVal value As String)
                Me.strWorkcenter = value
            End Set
        End Property

        <DataMember>
        Public Property Source() As PunchSource
            Get
                Return Me.intSource
            End Get
            Set(ByVal value As PunchSource)
                Me.intSource = value
            End Set
        End Property

        <DataMember>
        Public Property IDRequest As Integer
            Get
                Return Me.intIDRequest
            End Get
            Set(ByVal value As Integer)
                Me.intIDRequest = value
            End Set
        End Property

        <DataMember>
        Public Property SystemDetails As String
            Get
                Return Me.strSystemDetails
            End Get
            Set(ByVal value As String)
                Me.strSystemDetails = value
            End Set
        End Property

        Public Property Remarks As String
            Get
                Return Me.strRemarks
            End Get
            Set(ByVal value As String)
                Me.strRemarks = value
            End Set
        End Property

        Public Property NotReliableCause As String
            Get
                Return Me.strNotReliableCause
            End Get
            Set(ByVal value As String)
                Me.strNotReliableCause = value
            End Set
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Carga datos de un fichaje
        ''' </summary>
        Public Function Load(Optional ByVal bolAudit As Boolean = False, Optional bolLoadPhoto As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            If Me.lngID <= 0 Then
                Me.oCapture = Nothing
                Me.intActualType = Nothing
                Me.intInvalidType = Nothing
                Me.xShiftDate = Nothing
                Me.xDateTime = Nothing
                Me.intAction = 0
                Me.intIDPassport = 0
                Me.strIP = ""
                Me.strLocation = ""
                Me.strLocationZone = ""
                Me.strFullAddress = ""
                Me.strTimeZone = ""
                Me.bolExported = False
                Me.strTypeDetails = ""
                Me.strField1 = ""
                Me.strField2 = ""
                Me.strField3 = ""
                Me.dblField4 = 0
                Me.dblField5 = 0
                Me.dblField6 = 0
                Me.xTimeStamp = Nothing
                Me.strCRC = String.Empty
                Me.intVerificationType = VerificationType.UNKNOWN
                Me.bTemperatureAlert = Nothing
                Me.bMaskAlert = Nothing
                Me.strWorkcenter = ""
                Me.InTelecommute = False
                Me.intIDRequest = -1
                Me.Source = PunchSource.Unknown
            Else

                Try

                    Dim tb As DataTable = CreateDataTable("@SELECT# * FROM Punches WHERE [ID] = " & Me.ID.ToString, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                        Dim oRow As DataRow = tb.Rows(0)

                        If Not IsDBNull(oRow("IDCredential")) Then
                            Me.lngIDCredential = oRow("IDCredential")
                        Else
                            Me.lngIDCredential = 0
                        End If

                        If Not IsDBNull(oRow("IDEmployee")) Then
                            Me.intIDEmployee = oRow("IDEmployee")
                        Else
                            Me.intIDEmployee = 0
                        End If

                        Me.intType = oRow("Type")

                        If Not IsDBNull(oRow("ActualType")) Then
                            Me.intActualType = Any2Integer(oRow("ActualType"))
                        Else
                            Me.intActualType = Nothing
                        End If

                        If Not IsDBNull(oRow("InvalidType")) Then
                            Me.intInvalidType = Any2Integer(oRow("InvalidType"))
                        Else
                            Me.intInvalidType = Nothing
                        End If

                        If Not IsDBNull(oRow("ShiftDate")) Then
                            Me.xShiftDate = oRow("ShiftDate")
                        Else
                            Me.xShiftDate = Nothing
                        End If

                        Me.xDateTime = oRow("DateTime")

                        If Not IsDBNull(oRow("IDTerminal")) Then
                            Me.intIDTerminal = oRow("IDTerminal")
                        Else
                            Me.intIDTerminal = 0
                        End If

                        If Not IsDBNull(oRow("IDReader")) Then
                            Me.bIDReader = oRow("IDReader")
                        Else
                            Me.bIDReader = 0
                        End If

                        If Not IsDBNull(oRow("IDZone")) Then
                            Me.intIDZone = oRow("IDZone")
                        Else
                            Me.intIDZone = 0
                        End If

                        If Not IsDBNull(oRow("TypeData")) Then
                            Me.intTypeData = oRow("TypeData")
                        Else
                            Me.intTypeData = 0
                        End If

                        If Not IsDBNull(oRow("TypeDetails")) Then
                            Me.strTypeDetails = oRow("TypeDetails")
                        Else
                            Me.strTypeDetails = ""
                        End If

                        If Not IsDBNull(oRow("Location")) Then
                            Me.strLocation = oRow("Location")
                        Else
                            Me.strLocation = ""
                        End If

                        If Not IsDBNull(oRow("LocationZone")) Then
                            Me.strLocationZone = oRow("LocationZone")
                        Else
                            Me.strLocationZone = ""
                        End If

                        If Not IsDBNull(oRow("FullAddress")) Then
                            Me.strFullAddress = oRow("FullAddress")
                        Else
                            Me.strFullAddress = ""
                        End If

                        If Not IsDBNull(oRow("TimeZone")) Then
                            Me.strTimeZone = oRow("TimeZone")
                        Else
                            Me.strTimeZone = ""
                        End If

                        If Not IsDBNull(oRow("Exported")) Then
                            Me.bolExported = roTypes.Any2Boolean(oRow("Exported"))
                        Else
                            Me.bolExported = False
                        End If

                        If Not IsDBNull(oRow("IP")) Then
                            Me.strIP = oRow("IP")
                        Else
                            Me.strIP = ""
                        End If

                        If Not IsDBNull(oRow("IsNotReliable")) Then
                            Me.bolIsNotReliable = oRow("IsNotReliable")
                        Else
                            Me.bolIsNotReliable = False
                        End If

                        If Not IsDBNull(oRow("Action")) Then
                            Me.intAction = oRow("Action")
                        Else
                            Me.intAction = 0
                        End If

                        If Not IsDBNull(oRow("IDPassport")) Then
                            Me.intIDPassport = oRow("IDPassport")
                        Else
                            Me.intIDPassport = 0
                        End If

                        If Not IsDBNull(oRow("Field1")) Then
                            Me.strField1 = oRow("Field1")
                        Else
                            Me.strField1 = ""
                        End If

                        If Not IsDBNull(oRow("Field2")) Then
                            Me.strField2 = oRow("Field2")
                        Else
                            Me.strField2 = ""
                        End If

                        If Not IsDBNull(oRow("Field3")) Then
                            Me.strField3 = oRow("Field3")
                        Else
                            Me.strField3 = ""
                        End If

                        If Not IsDBNull(oRow("Field4")) Then
                            Me.dblField4 = oRow("Field4")
                        End If

                        If Not IsDBNull(oRow("Field5")) Then
                            Me.dblField5 = oRow("Field5")
                        End If

                        If Not IsDBNull(oRow("Field6")) Then
                            Me.dblField6 = oRow("Field6")
                        End If

                        If Not IsDBNull(oRow("TimeStamp")) Then
                            Me.xTimeStamp = oRow("TimeStamp")
                        Else
                            Me.xTimeStamp = Nothing
                        End If

                        If Not IsDBNull(oRow("CRC")) Then
                            Me.strCRC = oRow("CRC")
                        Else
                            Me.strCRC = String.Empty
                        End If

                        If Not IsDBNull(oRow("MaskAlert")) Then
                            Me.bMaskAlert = roTypes.Any2Boolean(oRow("MaskAlert"))
                        End If

                        If Not IsDBNull(oRow("TemperatureAlert")) Then
                            Me.bTemperatureAlert = roTypes.Any2Boolean(oRow("TemperatureAlert"))
                        End If

                        If Not IsDBNull(oRow("VerificationType")) Then
                            Me.intVerificationType = roTypes.Any2Integer(oRow("VerificationType"))
                        End If

                        If Not IsDBNull(oRow("Workcenter")) Then
                            Me.strWorkcenter = oRow("Workcenter")
                        Else
                            Me.strWorkcenter = ""
                        End If

                        Me.InTelecommute = oRow("InTelecommute")

                        Me.HasPhoto = roTypes.Any2Boolean(oRow("HasPhoto"))

                        Me.PhotoOnAzure = roTypes.Any2Boolean(oRow("PhotoOnAzure"))

                        Me.oCapture = Nothing

                        Me.Source = oRow("Source")

                        If Not IsDBNull(oRow("IDRequest")) Then
                            Me.intIDRequest = oRow("IDRequest")
                        Else
                            Me.intIDRequest = -1
                        End If

                        If Not IsDBNull(oRow("NotReliableCause")) Then
                            Me.NotReliableCause = roTypes.Any2String(oRow("NotReliableCause"))
                        End If

                        If Not IsDBNull(oRow("Remarks")) Then
                            Me.Remarks = roTypes.Any2String(oRow("Remarks"))
                        End If

                    End If

                    ' Recuperamos foto del fichaje si aplica
                    If bolLoadPhoto Then
                        If Not Me.PhotoOnAzure Then
                            tb = CreateDataTable("@SELECT# * FROM PunchesCaptures WHERE IDPunch = " & Me.ID.ToString, )
                            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                                Dim oRow As DataRow = tb.Rows(0)

                                Dim bImage As Byte() = CType(oRow("Capture"), Byte())
                                If bImage.Length > 0 Then
                                    Dim ms As MemoryStream = New MemoryStream(bImage)
                                    Me.oCapture = CType(Image.FromStream(ms), Bitmap)
                                End If
                            End If
                        Else
                            Try
                                Dim arrPhoto() As Byte
                                Dim arrDecryptedPhoto() As Byte
                                arrPhoto = Azure.RoAzureSupport.GetPunchPhotoFile(Me.ID.ToString)
                                If arrPhoto IsNot Nothing AndAlso arrPhoto.Length > 0 Then
                                    arrDecryptedPhoto = VTBase.Extensions.roEncrypt.Decrypt(arrPhoto)
                                    Dim ms As MemoryStream = New MemoryStream(arrDecryptedPhoto)
                                    Me.oCapture = CType(Image.FromStream(ms), Bitmap)
                                End If
                            Catch ex As Exception
                                roLog.GetInstance.logMessage(roLog.EventType.roError, "roPunch::Load:Error loading punch photo", ex)
                            End Try
                        End If
                    End If

                    ' Auditar lectura
                    If bolAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Dim strObjectName As String = ""
                        strObjectName &= " ID: " & Me.lngID.ToString & " IDEmployee: " & Me.intIDEmployee.ToString
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tMove, strObjectName, tbParameters, -1)
                    End If

                    bolRet = True
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roPunch::Load")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roPunch::Load")
                End Try

            End If

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda fichaje
        ''' </summary>
        Public Function Save(Optional ByVal bolInitTask As Boolean = True, Optional ByVal bolAudit As Boolean = False, Optional ByVal bolAutomaticBeginJobCheck As Boolean = True, Optional ByVal bolAutomaticFinishJobCheck As Boolean = True, Optional ByVal bolReprocessPunches As Boolean = True, Optional ByVal bolTypeManual As Boolean = False, Optional ByVal bolNotifyExport As Boolean = True, Optional ByVal bolTerminalPunchHasSeconds As Boolean = True, Optional ByVal bolUpdateDailyStatus As Boolean = True, Optional ByVal remarks As String = Nothing, Optional ByVal notReliableCause As String = Nothing) As Boolean

            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False
            Dim olog As New roLog("roPunch::Save")
            Dim oTermState As New Terminal.roTerminalState

            Try

                Dim oPunchOld As DataRow = Nothing
                Dim iIDReaderUsed As Integer = -1
                Dim oZone As Zone.roZone = Nothing
                Dim iOldZone As Integer = -1

                'Cargar fichaje anterior para recuperar zona
                iOldZone = roTypes.Any2Integer(ExecuteScalar("@SELECT# top 1 IDZone FROM Punches where Datetime < " & roTypes.Any2Time(Me.DateTime).SQLDateTime & " and IDEmployee = " & Me.IDEmployee & " ORDER BY Datetime desc"))
                If iOldZone = 0 Then iOldZone = DEFAULT_ZONE

                Dim tbPunch As New DataTable("Punches")
                Dim strSQL As String = "@SELECT# * FROM Punches WHERE [ID] = " & Me.lngID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tbPunch)

                Dim oRow As DataRow = Nothing
                If Me.lngID <= 0 Then
                    oRow = tbPunch.NewRow
                    bolIsNew = True
                ElseIf tbPunch.Rows.Count = 1 Then
                    oRow = tbPunch.Rows(0)
                    bolIsNew = False
                End If

                Dim tbAuditOld As DataTable = New DataTable("Punches")
                tbAuditOld = tbPunch.Clone
                If Not bolIsNew Then
                    tbAuditOld.ImportRow(oRow)
                    oPunchOld = tbAuditOld.Rows(0)
                End If

                ' Cargo parámetro de personalización
                Dim customization As String = roBusinessSupport.GetCustomizationCode().ToUpper()

                ' Si no me informaron el idempleado, pero si tengo el código de tarjeta, intento localizar el idempleado.
                ' Este código parece redundante y además no contempla el caso de que la tarjeta esté asignada a través de la CardAliases.
                ' Parece que se hizo para el caso de asignar tarjetas a empleados a través del asistente de asignación, pero ese asistente ya recupera el
                ' id de empleado antes de hacer el Punch.Save ... SE DEBERÍA ELIMINAR ...
                ' ACLARADO PDTE DE ARREGLO: Aquí podemos llegar desde MOVES Net con ciertos Behaviors (como PUNCHCONNECTOR), con ID tarjeta e IDempleado 0. Esto no se debería resolver aquí ...
                Dim sTermType As String = String.Empty
                If Me.intIDEmployee = 0 AndAlso Me.lngIDCredential > 0 Then
                    sTermType = Terminal.roTerminal.GetTerminalType(Me.intIDTerminal, oTermState)
                    If sTermType.ToUpper <> "VIRTUAL" AndAlso sTermType.ToUpper <> "MX9" AndAlso sTermType.ToUpper <> "SUPREMA" Then
                        ' Intentamos obtener el id del empleado a partir de la tarjeta
                        Dim tb As DataTable = roBusinessSupport.GetEmployeesByIDCard(Me.lngIDCredential, "", "", "", State)
                        If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            Dim oRowCard As DataRow = tb.Rows(0)
                            If Not IsDBNull(oRowCard("IDEmployee")) Then Me.intIDEmployee = oRowCard("IDEmployee")
                        End If
                    End If
                End If

                ' Aplico cambio de comportamiento ACCTA o ACC en función de configuraciones especiales de la ficha
                CheckIfAttControlled()

                oRow("IDCredential") = Me.lngIDCredential
                oRow("IDEmployee") = Me.intIDEmployee
                oRow("Type") = Me.intType
                oRow("VerificationType") = Me.VerificationType

                If Not Me.xDateTime.HasValue Then
                    Me.xDateTime = Date.Now
                End If

                If Not Me.bMaskAlert.HasValue Then
                    oRow("MaskAlert") = DBNull.Value
                Else
                    oRow("MaskAlert") = Me.bMaskAlert
                End If

                If Not Me.bTemperatureAlert.HasValue Then
                    oRow("TemperatureAlert") = DBNull.Value
                Else
                    oRow("TemperatureAlert") = Me.bTemperatureAlert
                End If

                If Me.Remarks IsNot Nothing Then
                    oRow("Remarks") = Me.Remarks
                Else
                    oRow("Remarks") = DBNull.Value
                End If

                If Me.NotReliableCause IsNot Nothing Then
                    oRow("NotReliableCause") = Me.NotReliableCause
                Else
                    oRow("NotReliableCause") = DBNull.Value
                End If

                oRow("Workcenter") = Me.strWorkcenter

                Dim FormatDate As Date = Any2Time(xDateTime.Value).DateOnly
                FormatDate = DateAdd(DateInterval.Hour, DatePart(DateInterval.Hour, xDateTime.Value), FormatDate)
                FormatDate = DateAdd(DateInterval.Minute, DatePart(DateInterval.Minute, xDateTime.Value), FormatDate)
                FormatDate = DateAdd(DateInterval.Second, DatePart(DateInterval.Second, xDateTime.Value), FormatDate)

                Me.xDateTime = FormatDate

                ' Calculamos el ActualType
                If (Not bolTypeManual) OrElse (Not Me.intActualType.HasValue) Then
                    Select Case Me.intType
                        Case PunchTypeEnum._IN, PunchTypeEnum._OUT, PunchTypeEnum._AV, PunchTypeEnum._AI, PunchTypeEnum._INI, PunchTypeEnum._OUTI, PunchTypeEnum._CENTER
                            ' La operación fichada y la actual tiene que ser siempre la misma
                            ' siempre y cuando no sea una accion de cambio de sentido
                            If Me.intAction <> 3 Then
                                Me.intActualType = Me.intType
                            End If
                            ' Miramos si hay que validar tiempos mínimos entre fichajes del mismo sentido.
                            Dim iMinMinutes As Integer
                            iMinMinutes = roTypes.Any2Integer(roAdvancedParameter.GetAdvancedParameterValue("AttMinMinutesBetweenPunches", Nothing, False))
                            If (Me.intType = PunchTypeEnum._IN OrElse Me.intType = PunchTypeEnum._OUT) AndAlso iMinMinutes > 0 Then
                                ' Para adidas, fichajes de terminal en modo rápido, deben contemplar los parámetros de máximo entre entrada y salida y entre salida y entrada
                                Me.intActualType = ValidateAttRepeatededPunch(iMinMinutes)
                                If Me.intActualType = PunchTypeEnum._NOTDEFINDED Then
                                    Select Case Me.intType
                                        Case PunchTypeEnum._IN
                                            Me.intType = PunchTypeEnum._RPTIN
                                        Case PunchTypeEnum._OUT
                                            Me.intType = PunchTypeEnum._RPTOUT
                                    End Select
                                    oRow("Type") = Me.intType
                                End If
                            End If
                        Case PunchTypeEnum._AUTO
                            ' Si el fichaje es nuevo y existe otro fichaje a la misma hora del empleado , no lo guardamos
                            If bolIsNew AndAlso Me.intIDEmployee > 0 Then
                                Dim tb As DataTable = CreateDataTable("@SELECT# ID FROM Punches WHERE IDEmployee = " & Me.intIDEmployee & " AND DateTime = " & Any2Time(Me.xDateTime).SQLSmallDateTime & " AND ActualType IN(" & PunchTypeEnum._IN & "," & PunchTypeEnum._OUT & ")" &
                                      "", )
                                If tb IsNot Nothing AndAlso tb.Rows.Count >= 1 Then
                                    olog.logMessage(roLog.EventType.roDebug, "Automatic punch on same minute already exists!. It will not be saved. IdEmployee=" & Me.intIDEmployee & " IdTerminal=" & Me.intIDTerminal & " DateTime=" & Me.DateTime.ToString & " Type=" & Me.intType.ToString)
                                    bolRet = True
                                    Return bolRet
                                End If
                            End If

                            ' Se debe calcular la operación de presencia actual en base a los fichajes que ac tualmente
                            ' tiene el empleado
                            Me.intActualType = CalculateTypeAtt()
                        Case PunchTypeEnum._L
                            ' Se debe calcular la operación de presencia actual en base a la zona a la que ha accedido
                            ' Si no viene la zona , intentamos obtenerla a partir del terminal y el lector
                            If Me.intIDZone = 0 Then
                                iIDReaderUsed = GetIDReaderUsed(olog)
                                Me.intIDZone = Terminal.roTerminal.GetTerminalReaderZoneByID(Me.intIDTerminal, iIDReaderUsed, False, )
                            End If
                            Me.intActualType = CalculateTypeAcc()
                    End Select

                    ' Control de fichajes repetidos que vienen de terminales físicos.
                    ' No guardo si coinciden hora, empleado, tipo, terminal y el terminal guarda segundos
                    If bolIsNew AndAlso Me.intIDEmployee > 0 AndAlso bolTerminalPunchHasSeconds AndAlso Me.Type <> PunchTypeEnum._TASK Then
                        Dim tb As DataTable = CreateDataTable("@SELECT# ID FROM Punches WHERE IDEmployee = " & Me.intIDEmployee & " AND DateTime = " & Any2Time(Me.xDateTime).SQLDateTime & " AND Type = " & Me.intType & " AND IDTerminal = " & Me.intIDTerminal.ToString, )
                        If tb IsNot Nothing AndAlso tb.Rows.Count >= 1 Then
                            olog.logMessage(roLog.EventType.roDebug, "This punch already exists!. It will not be saved. IdEmployee=" & Me.intIDEmployee & " IdTerminal=" & Me.intIDTerminal & " DateTime=" & Me.DateTime.ToString & " Type=" & Me.intType.ToString)
                            Return True
                        End If
                    End If
                End If

                If Me.intActualType.HasValue Then
                    oRow("ActualType") = Me.intActualType
                Else
                    oRow("ActualType") = DBNull.Value
                End If

                ' Calculamos la zona si es necesario
                Dim bSearchZone As Boolean = True
                If sTermType = String.Empty Then sTermType = Terminal.roTerminal.GetTerminalType(Me.intIDTerminal, oTermState)

                ' 0.- Vemos si nos fijan la zona a través de la justificación fichada
                If bSearchZone AndAlso (Me.intActualType = PunchTypeEnum._IN OrElse Me.intActualType = PunchTypeEnum._OUT) Then
                    ' 1.- Si es un fichaje de control horario y ficharon con justificación con zona asociada, esta prevalece.
                    Dim idZone As Integer = 0
                    Try
                        strSQL = "@SELECT# IDZone FROM Causes WHERE [ID] = " & Me.intTypeData.ToString
                        idZone = roTypes.Any2Long(ExecuteScalar(strSQL))
                        If idZone > 0 Then
                            Me.intIDZone = idZone
                            bSearchZone = False
                        Else
                            ' En el caso que la justificacion sea de trabajo externo, no asignamnos zona por defecto
                            ' para que no salga en los informes de emergencia
                            ' TODO: queda pendiente poder configurar la zona desde la pantalla de justificaciones para cada una
                            strSQL = "@SELECT# isnull(ExternalWork,0) FROM Causes WHERE [ID] = " & Me.intTypeData.ToString
                            If roTypes.Any2Boolean(ExecuteScalar(strSQL)) Then
                                Me.intIDZone = DEFAULT_ZONE
                                bSearchZone = False
                            End If
                        End If
                    Catch ex As Exception
                        ' Do Something
                    End Try
                End If

                ' Si viene de un terminal físico (o es un Portal del Empleado en OP) y aún no tengo la zona ...
                If bSearchZone AndAlso Me.intIDTerminal <> 0 AndAlso sTermType.ToUpper <> "LIVEPORTAL" Then
                    If iIDReaderUsed = -1 Then iIDReaderUsed = GetIDReaderUsed(olog)
                    Dim iIDZone As Integer = Me.intIDZone

                    ' Si es un terminal Timegate, tiene comportamiento Portal y no tiene zona asignada, la zona se calcula de la misma manera que en el Portal ...
                    Dim searchZoneByReader As Boolean = True
                    If sTermType.ToUpper = "TIME GATE" Then
                        Dim readerMode As String = Terminal.roTerminal.GetTerminalReaderMode(Me.intIDTerminal, 1)
                        iIDZone = Terminal.roTerminal.GetTerminalReaderZoneByID(Me.intIDTerminal, 1, False, )
                        searchZoneByReader = Not (readerMode = "EIP" AndAlso iIDZone = 0)
                    End If

                    If searchZoneByReader Then
                        If (Me.intActualType = PunchTypeEnum._IN AndAlso Me.intType <> PunchTypeEnum._L) OrElse Me.intType = PunchTypeEnum._RPTIN Then
                            ' Fichaje de entrada de presencia
                            ' OJO: temporalmente, el campo IDZoneOut de la tabla TerminalReaders puede contener una zona de entrada !!!
                            iIDZone = Terminal.roTerminal.GetTerminalReaderZoneByID(Me.intIDTerminal, iIDReaderUsed, False, )
                        ElseIf (Me.intActualType = PunchTypeEnum._OUT AndAlso Me.intType <> PunchTypeEnum._L) OrElse Me.intType = PunchTypeEnum._RPTOUT Then
                            ' Fichaje de salida de presencia
                            iIDZone = Terminal.roTerminal.GetTerminalReaderZoneByID(Me.intIDTerminal, iIDReaderUsed, True, )
                        Else
                            ' Cualquier otro tipo de fichaje
                            ' Si en el fichaje no me viene la zona (porque no es de accesos, la asigno ahora ...)
                            If Me.intIDZone = 0 OrElse Me.intIDZone = -1 Then
                                ' Recupero la zona definida en el lector, teniendo en cuenta el tipo de fichajes ...
                                iIDZone = Terminal.roTerminal.GetTerminalReaderZoneByID(Me.intIDTerminal, iIDReaderUsed, False, )
                            End If
                        End If
                        If iIDZone > 0 Then
                            Me.intIDZone = iIDZone
                        End If
                    End If
                End If

                ' 1.- Si ya tengo una zona manual indicada desde el Portal, no sigo buscando ...
                If (Me.intIDZone > 0 AndAlso Me.intIDZone <> DEFAULT_ZONE) Then bSearchZone = False

                If bSearchZone AndAlso sTermType.ToUpper <> "NFC" AndAlso Me.Location IsNot Nothing AndAlso Me.Location.Length > 0 Then
                    ' Si tengo coordenadas, y no estooy en un OnPremise, busco la zona por las coordenadas.
                    oZone = Zone.roZone.GetZoneFromCoordinates(Me.Location, New Zone.roZoneState(-1))
                    If oZone IsNot Nothing Then
                        Me.intIDZone = oZone.ID
                        bSearchZone = False
                    Else
                        Me.intIDZone = DEFAULT_ZONE
                        bSearchZone = False
                    End If
                End If

                'Si es fichaje Productiv (Tipo 4) y no tiene intIDZone le asignamos el iOldZone (Zona del ultimo fichaje o Zona mundial si no tiene)
                If bSearchZone AndAlso Me.intActualType = PunchTypeEnum._TASK AndAlso Me.intIDZone <= 0 Then
                    Me.intIDZone = iOldZone
                    bSearchZone = False
                End If

                If Me.intInvalidType.HasValue Then
                    oRow("InvalidType") = Me.intInvalidType
                Else
                    oRow("InvalidType") = DBNull.Value
                End If

                If Me.xShiftDate.HasValue Then
                    oRow("ShiftDate") = Me.xShiftDate.Value
                Else
                    oRow("ShiftDate") = Any2Time(Me.xDateTime.Value).DateOnly
                End If

                oRow("DateTime") = Me.xDateTime
                oRow("IDTerminal") = IIf(Me.intIDTerminal <> -1, Me.intIDTerminal, DBNull.Value)
                oRow("IDReader") = IIf(Me.bIDReader <> -1, Me.bIDReader, DBNull.Value)
                oRow("IDZone") = IIf(Me.intIDZone <> -1, Me.intIDZone, DEFAULT_ZONE)
                oRow("ZoneIsNotReliable") = If(Me.bolZoneIsNotReliable, 1, 0)
                oRow("TypeData") = IIf(Me.intTypeData <> -1, Me.intTypeData, DBNull.Value)
                oRow("TypeDetails") = IIf(Me.strTypeDetails <> "", Me.strTypeDetails, DBNull.Value)
                oRow("Location") = IIf(Me.strLocation <> "" AndAlso Me.strLocation <> "0,0", Me.strLocation, DBNull.Value)

                oRow("FullAddress") = IIf(Me.strFullAddress <> "", Me.strFullAddress, DBNull.Value)

                oRow("IP") = IIf(Me.strIP <> "", Me.strIP, DBNull.Value)

                oRow("Action") = IIf(Me.intAction <> -1, Me.intAction, DBNull.Value)
                oRow("IDPassport") = IIf(Me.intIDPassport <> -1, Me.intIDPassport, DBNull.Value)

                oRow("Field1") = IIf(Me.strField1 <> "", Me.strField1, DBNull.Value)
                oRow("Field2") = IIf(Me.strField2 <> "", Me.strField2, DBNull.Value)
                oRow("Field3") = IIf(Me.strField3 <> "", Me.strField3, DBNull.Value)
                oRow("Field4") = IIf(Me.dblField4 <> -1, Me.dblField4, DBNull.Value)
                oRow("Field5") = IIf(Me.dblField5 <> -1, Me.dblField5, DBNull.Value)
                oRow("Field6") = IIf(Me.dblField6 <> -1, Me.dblField6, DBNull.Value)

                oRow("Exported") = Me.bolExported

                Try
                    oRow("SystemDetails") = Me.strSystemDetails
                Catch ex As Exception
                    ' Do nothing
                End Try

                If roTypes.Any2String(customization) = "FCBPS" AndAlso bolIsNew Then
                    ' Para integración con FCB - PeopleSoft, todos los fichajes se guardan como no fiables antes de ser enviadeos a PeopleSoft para que desde edición de fichajes estandar se pueda distinguir gráficamente qué fichajes se han enviado, y cuáles no.
                    oRow("IsNotReliable") = 1
                    ' Si el fichaje es de salida y viene de un terminal, se le asigna la misma zona configurada para fichajes de entrada
                    If Me.intActualType = PunchTypeEnum._OUT AndAlso Me.intIDTerminal <> 0 Then
                        oRow("Field4") = Terminal.roTerminal.GetTerminalReaderZoneByID(Me.intIDTerminal, iIDReaderUsed, False, )
                    Else
                        oRow("Field4") = oRow("IDZone")
                    End If
                Else
                    oRow("IsNotReliable") = If(Me.bolIsNotReliable, 1, 0)
                End If

                oRow("TimeStamp") = IIf(Me.TimeStamp.HasValue, Me.TimeStamp, DBNull.Value)

                If Me.strTimeZone <> "" Then
                    oRow("TimeZone") = Me.strTimeZone
                Else
                    Dim tZoneInfo As TimeZoneInfo = TimeZoneInfo.Local
                    Dim bContinueSearching As Boolean = True

                    If bContinueSearching AndAlso IDTerminal > 0 Then
                        'Dim oTerminal As Terminal.roTerminal = New Terminal.roTerminal(Me.intIDTerminal, New Terminal.roTerminalState)
                        Dim sTermTimeZone As String = Terminal.roTerminal.GetTerminalTimeZone(Me.intIDTerminal, oTermState)

                        If sTermTimeZone <> String.Empty Then
                            tZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(sTermTimeZone)
                            bContinueSearching = False
                        End If
                    End If

                    'Si no tiene terminal, pero tiene zona, aplicaremos la de la zona
                    If bContinueSearching AndAlso Me.intIDZone > 0 Then
                        oZone = New Zone.roZone(Me.intIDZone, New Zone.roZoneState)
                        If oZone.DefaultTimezone <> String.Empty Then
                            tZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(oZone.DefaultTimezone)
                            bContinueSearching = False
                        End If
                    End If

                    'Aplicaremos la zona horaria que tiene por defecto el empleado
                    If bContinueSearching Then
                        Dim workingZoneID As Integer = -1
                        Dim nonWorkingZoneID As Integer = -1

                        Dim oMobility As Employee.roMobility = Employee.roMobility.GetCurrentMobility(Me.intIDEmployee, New Employee.roEmployeeState)

                        If oMobility IsNot Nothing Then
                            Group.roGroup.GetGroupZones(oMobility.IdGroup, workingZoneID, nonWorkingZoneID, New Group.roGroupState)
                            If workingZoneID > -1 Then
                                Dim oZoneAux As Zone.roZone = New Zone.roZone(workingZoneID, New Zone.roZoneState(-1))  'ZoneService.ZoneServiceMethods.GetZoneByID(Nothing, workingZoneID, False)
                                If oZoneAux.DefaultTimezone <> String.Empty Then
                                    tZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(oZoneAux.DefaultTimezone)
                                    bContinueSearching = False
                                End If
                            End If
                        End If

                    End If

                    oRow("TimeZone") = tZoneInfo.Id
                End If

                If IsDBNull(oRow("TypeData")) AndAlso Me.intType = PunchTypeEnum._TASK Then
                    oRow("TypeData") = 0
                End If

                ' Centro de trabajo.
                If Me.intIDEmployee > 0 AndAlso Me.intIDZone > 0 Then
                    ' El de la zona, si lo tiene
                    If oZone Is Nothing Then oZone = New Zone.roZone(Me.intIDZone, New Zone.roZoneState)
                    Me.strWorkcenter = roTypes.Any2String(oZone.WorkCenter)

                    ' Localidad si está definida en la Zona
                    If oZone.ZoneNameAsLocation.HasValue AndAlso oZone.ZoneNameAsLocation.Value Then
                        Me.strLocationZone = oZone.Name
                    End If
                End If

                oRow("Workcenter") = Me.strWorkcenter
                oRow("LocationZone") = IIf(Me.strLocationZone <> "", Me.strLocationZone, DBNull.Value)
                oRow("InTelecommute") = Me.InTelecommute
                If Me.intIDRequest > 0 Then
                    oRow("IDRequest") = Me.intIDRequest
                Else
                    oRow("IDRequest") = DBNull.Value
                End If

                Dim iFactor As Integer = 0
                Select Case Me.intActualType
                    Case PunchTypeEnum._OUT, PunchTypeEnum._OUTI, PunchTypeEnum._RPTOUT
                        iFactor = 1
                    Case PunchTypeEnum._IN, PunchTypeEnum._INI, Me.intActualType = PunchTypeEnum._RPTIN
                        iFactor = -1
                End Select
                If iFactor <> 0 Then oRow("Timespan") = iFactor * Me.xDateTime.Value.Subtract(New Date(2021, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)).TotalSeconds

                ' CRC
                Try
                    Me.strCRC = GetCRC(oRow)
                    oRow("CRC") = Me.strCRC
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roPunch::Save:GetCRC")
                End Try

                ' Origen del fichaje
                If Me.Source = PunchSource.Unknown AndAlso Me.lngID <= 0 Then
                    If Me.intIDPassport = 0 AndAlso IDTerminal > 0 Then
                        Me.Source = PunchSource.Terminal
                    ElseIf Me.intIDPassport > 0 Then
                        Me.Source = PunchSource.Supervisor
                    End If
                End If
                oRow("Source") = Me.Source

                If Me.lngID <= 0 Then
                    tbPunch.Rows.Add(oRow)
                End If

                Dim bDeletePhotoFromDatabase As Boolean = False
                If Me.oCapture IsNot Nothing Then
                    oRow("HasPhoto") = 1
                    oRow("PhotoOnAzure") = 1
                    ' Si el fichaje ya existía y la foto no estaba en Azure, cuando se guarde en Azure eliminaré la de base de datos
                    bDeletePhotoFromDatabase = (Not bolIsNew AndAlso Not roTypes.Any2Boolean(Me.PhotoOnAzure))
                End If

                If Me.Source = PunchSource.PortalApp AndAlso sTermType.ToUpper = "LIVEPORTAL" Then
                    ' Si fiché desde la APP del Portal, cambio el id de terminal por el del Portal APP
                    Dim idLivePortalApp As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID FROM Terminals WHERE UPPER(Type) = 'LIVEPORTAL' AND Other = 'APP'"))
                    If idLivePortalApp > 0 Then
                        oRow("IDTerminal") = idLivePortalApp
                        Me.intIDTerminal = idLivePortalApp
                    End If
                End If

                da.Update(tbPunch)

                'Alerta relacionada con fichaje actual
                If Me.intIDZone <> DEFAULT_ZONE Then
                    If oZone Is Nothing Then oZone = New Zone.roZone(Me.intIDZone, New Zone.roZoneState)
                    roPunch.SendCapacityNotificationIfNeeded(oZone, Me.DateTime.Value)
                End If

                'Alerta de aforo sobre la zona que se abandona
                If iOldZone <> DEFAULT_ZONE Then
                    Dim oOldZone As roZone = New Zone.roZone(iOldZone, New Zone.roZoneState)
                    roPunch.SendCapacityNotificationIfNeeded(oOldZone, Me.DateTime.Value)
                End If

                ' ARGAL
                If customization = "LAGRA" AndAlso bolIsNew Then
                    Try
                        Dim sSql As String = String.Empty
                        sSql = "@UPDATE# DailySchedule WITH (ROWLOCK) SET IsExported=0, IsModified=1  "
                        sSql = sSql & " WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND Date = " & Any2Time(Me.ShiftDate).SQLSmallDateTime
                        bolRet = ExecuteSql(sSql)
                    Catch ex As Exception
                        Me.oState.UpdateStateInfo(ex, "roPunch::Save:ARGAL:Exception on new punch")
                    End Try
                End If

                ' Guardamos las transacciones para generar backup de fichajes.
                Try
                    Dim sqlBCKCommandText As String
                    If bolIsNew Then
                        sqlBCKCommandText = DataLayer.AccessHelper.GetSQLCommandText(da.InsertCommand)
                    Else
                        sqlBCKCommandText = DataLayer.AccessHelper.GetSQLCommandText(da.UpdateCommand)
                        ' Si es un UPDATE, no localizo por ID, dado que puede haber cambiado al crear los nuevos inserts.Localizo por CRC e IDEmployee y DateTime (por si CRC es NULL)
                        Dim sSQLAux As String = sqlBCKCommandText.Substring(0, sqlBCKCommandText.LastIndexOf("[ID]"))
                        Dim sCRCWhere As String
                        If (IsDBNull(oPunchOld("CRC")) OrElse Any2String(oPunchOld("CRC")).Length = 0) Then sCRCWhere = " is NULL " Else sCRCWhere = " = '" & oPunchOld("CRC") & "'"
                        sqlBCKCommandText = sSQLAux & " [CRC] " & sCRCWhere & " AND IDEmployee = " & oPunchOld("IDEmployee") & " AND DateTime = " & Any2Time(oPunchOld("DateTime")).SQLDateTime & "))"
                    End If
                    sqlBCKCommandText = sqlBCKCommandText.Replace("'", "''")
                    ExecuteSql("@INSERT# INTO sysroPunchesTransactions values ('" & sqlBCKCommandText & "'," & Any2Time(oRow("ShiftDate")).SQLSmallDateTime & "," & Any2Integer(oRow("IDEmployee")) & "," & Any2Time(Now).SQLDateTime & ",0)")
                Catch ex As Exception
                    'PDTE: En algún caso se ha detectado un error "Value can't be converted to roTime". Habría que poner traza para ver qué fichaje está llegnado aquí ... En teoría todos los valores convertidos mediante Any2Time deben ser correctos y estar informados.
                    Me.oState.UpdateStateInfo(ex, "roPunch::Save:SavePunchesTransactionsSQL")
                End Try

                ' Recupero el ID asignado al fichaje para guardar la foto, si aplica
                If bolIsNew Then
                    Dim tb As DataTable = CreateDataTable("@SELECT# TOP 1 [ID] FROM Punches WHERE IDTerminal=" & Me.intIDTerminal.ToString & " AND IDEmployee= " & Me.intIDEmployee.ToString & " ORDER BY [ID] DESC", )

                    If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                        Me.lngID = tb.Rows(0)("ID")
                        oRow("ID") = Me.lngID
                    End If
                End If

                ' Grabar imagen
                If Me.oCapture IsNot Nothing AndAlso (bolIsNew OrElse bDeletePhotoFromDatabase) Then
                    bolRet = Me.SaveCapture(bDeletePhotoFromDatabase)
                Else
                    bolRet = True
                End If

                If bolAudit AndAlso bolRet Then
                    ' Añadimos auditoria
                    Dim sEmployeeName As String = roTypes.Any2String(ExecuteScalar("@SELECT# Name FROM Employees WHERE [ID] = " & Me.intIDEmployee.ToString))
                    Dim sCauseName As String = ""
                    Dim sPunchType As String = ""

                    'Añadimos info para el mensaje
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()

                    'Detalle "técnico" de valores de campos de la tabla
                    'oState.AddAuditFieldsValues(tbParameters, oPunchNew, oPunchOld)
                    oState.AddAuditFieldsValues(tbParameters, oRow, oPunchOld)

                    'Detalle para "humanos"
                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", sEmployeeName, "", 1)
                    oState.AddAuditParameter(tbParameters, "{Date}", xDateTime.Value.ToShortDateString, "", 1)
                    oState.AddAuditParameter(tbParameters, "{Time}", xDateTime.Value.ToShortTimeString, "", 1)
                    sPunchType = System.Enum.GetName(GetType(PunchTypeEnum), Me.intType)
                    oState.AddAuditParameter(tbParameters, "{PunchType}", oState.Language.Translate("PunchType." & sPunchType, ""), "", 1)
                    If Me.intActualType = PunchTypeEnum._IN Or Me.intActualType = PunchTypeEnum._OUT Then
                        If Me.intTypeData <> -1 AndAlso Me.intTypeData <> 0 Then
                            'Recupero nombre de la justificación
                            Dim oCauseState As New VTBusiness.Cause.roCauseState
                            Dim oCause As New VTBusiness.Cause.roCause(Me.intTypeData, oCauseState)
                            sCauseName = " (" & oCause.Name & ")"
                        End If
                        oState.AddAuditParameter(tbParameters, "{CauseName}", sCauseName, "", 1)
                    End If

                    Dim oAuditAction As Audit.Action = IIf(bolIsNew, Audit.Action.aInsert, Audit.Action.aUpdate)

                    Me.oState.Audit(oAuditAction, Audit.ObjectType.tMove, sEmployeeName, tbParameters, -1)
                End If

                ' Obtenemos el tipo de fichaje actual
                Dim _intActualType As PunchTypeEnum
                _intActualType = PunchTypeEnum._NOTDEFINDED
                If Me.intActualType.HasValue Then
                    _intActualType = Me.intActualType
                End If

                ' Si el fichaje es de presencia y esta asignado a un empleado
                If (_intActualType = PunchTypeEnum._IN OrElse _intActualType = PunchTypeEnum._OUT OrElse _intActualType = PunchTypeEnum._CENTER OrElse Me.intType = PunchTypeEnum._L) AndAlso Me.intIDEmployee > 0 Then

                    ' Actualizamos el status
                    If bolUpdateDailyStatus Then
                        UpdateDailySchedule()
                    End If

                    ' Generamos fichajes de produccion en caso necesario
                    If bolAutomaticBeginJobCheck OrElse bolAutomaticFinishJobCheck Then
                        AutomaticJobPunch(bolAutomaticBeginJobCheck, bolAutomaticFinishJobCheck)
                    End If

                    ' Notificamos al servidor
                    If bolInitTask Then
                        If Me.DateTime.Value.Date = Date.Now.Date Then
                            ExecuteSql("@UPDATE# sysroParameters SET DATA='1' WHERE ID='RUNENGINE'")
                        Else
                            Dim oContext As New roCollection
                            oContext.Add("User.ID", Me.intIDEmployee)
                            roConnector.InitTask(TasksType.MOVES, oContext)
                        End If
                    End If

                    ' Reprocesamos los fichajes posteriores en caso necesario
                    If bolReprocessPunches Then
                        ReprocessPunches(Me.intIDEmployee, Me.xDateTime)
                    End If

                    ' Si el fichaje es de tareas
                ElseIf _intActualType = PunchTypeEnum._TASK AndAlso Me.intIDEmployee > 0 Then

                    ' Actualizamos el status
                    If bolUpdateDailyStatus Then
                        UpdateDailyTaskSchedule()
                    End If

                    ' Actualizamos la fecha/hora de inicio de la tarea en caso necesario
                    If Me.intTypeData > 0 Then
                        Dim sSql As String = "@UPDATE# Tasks SET StartDate=" & Any2Time(xDateTime).SQLSmallDateTime & " WHERE ID=" & Me.intTypeData.ToString & " AND StartDate is null"
                        ExecuteSql(sSql)
                    End If

                    ' Notificamos al servidor
                    If bolInitTask Then
                        If Me.DateTime.Value.Date = Date.Now.Date Then
                            ExecuteSql("@UPDATE# sysroParameters SET DATA='1' WHERE ID='RUNENGINE'")
                        Else
                            Dim oContext As New roCollection
                            oContext.Add("User.ID", Me.intIDEmployee)
                            roConnector.InitTask(TasksType.TASKS, oContext)
                        End If

                    End If
                End If

                ' Si el fichaje es de Centros de coste
                If _intActualType = PunchTypeEnum._CENTER AndAlso Me.intIDEmployee > 0 Then
                    ' Actualizamos el status de días futuros en caso necesario
                    If bolUpdateDailyStatus Then
                        UpdateDailyCenterSchedule()
                    End If

                    ' Notificamos al servidor
                    If bolInitTask Then
                        If Me.DateTime.Value.Date = Date.Now.Date Then
                            ExecuteSql("@UPDATE# sysroParameters SET DATA='1' WHERE ID='RUNENGINE'")
                        Else
                            Dim oContext As New roCollection
                            oContext.Add("User.ID", Me.intIDEmployee)
                            roConnector.InitTask(TasksType.MOVES, oContext)
                        End If
                    End If
                End If

                ' Si el fichaje es de presencia, pero no está asignado a ningún empleado
                ' debemos añadir tarea de usuario para poder asignar la tarjeta al empleado
                If (Me.intType = PunchTypeEnum._IN OrElse Me.intType = PunchTypeEnum._OUT OrElse Me.intType = PunchTypeEnum._L OrElse Me.intType = PunchTypeEnum._AUTO OrElse Me.intType = PunchTypeEnum._AI) AndAlso Me.intIDEmployee <= 0 AndAlso Me.lngIDCredential > 0 Then
                    Dim oUserTask As New UserTask.roUserTask()
                    oUserTask.ID = UserTask.roUserTask.roUserTaskObject & ":\\" & UserTask.roUserTask.NO_EMPLOYEE_TASK
                    oUserTask.DateCreated = DateTime
                    oUserTask.TaskType = UserTask.TaskType.UserTaskRepair
                    oUserTask.ResolverURL = "FN:\\Resolver_MovesInvalidCardID"
                    oUserTask.Message = oState.Language.Translate("NoEmployeeFound.Task", "")
                    oUserTask.Save()
                End If

                'Si el fichaje es de Acceso valido o de Acceso integrado con presencia y se realiza sobre la zona de cierre de Visits,
                'debemos cerrar la salida de Visits si procede
                If (Me.intType = PunchTypeEnum._AV OrElse Me.intType = PunchTypeEnum._L) Then
                    Dim oVisits As New roVisits(New roVisitsState())
                    Dim oVisitors As New roVisitors(New roVisitorsState())

                    'get idzone
                    Dim zoneId = oVisits.GetConfiguration(roVisits.ConfigurationZoneKey)

                    'check if idzone is defined and equivalent from punch
                    If (zoneId IsNot Nothing AndAlso IDZone = CInt(zoneId)) Then

                        'check if employeeid belongs to active visit
                        Dim VisitId = oVisits.FindByEmployeeId(Me.IDEmployee)
                        If VisitId <> Guid.Empty Then
                            'complete visit
                            oVisits.UpdateStatus(VisitId, roVisits.StatusVisitCompleted)
                            'insert punh on visitor
                            oVisitors.InsertPunch(VisitId, roVisitors.TypePunchOut)
                        End If
                    End If
                End If

                ' Si el fichaje indica que hay una alerta de empleado sin mascarilla, creamos la notificacion en caso necesario
                If (Me.MaskAlert.HasValue AndAlso Me.MaskAlert) AndAlso Me.intIDEmployee > 0 Then
                    Dim oNotificationState As New Notifications.roNotificationState(-1)
                    Dim oAlertNotifications As Generic.List(Of Notifications.roNotification) = Notifications.roNotification.GetNotifications("IDType = 69 And Activated=1", oNotificationState,, True)

                    If oAlertNotifications IsNot Nothing AndAlso oAlertNotifications.Count > 0 Then
                        For Each oNotification As Notifications.roNotification In oAlertNotifications
                            Dim xDailyDate As DateTime
                            If Me.xShiftDate.HasValue Then
                                xDailyDate = Me.xShiftDate.Value.Date
                            ElseIf Me.xDateTime.HasValue Then
                                xDailyDate = Me.xDateTime.Value.Date
                            End If
                            Dim strSQLAlert As String = ""
                            strSQLAlert = " IF NOT EXISTS (@SELECT# 1 FROM sysroNotificationTasks WHERE IDNotification=" & oNotification.ID.ToString & " AND Key1Numeric = " & Me.intIDEmployee.ToString & " AND  Key3DateTime = " & Any2Time(xDailyDate.Date).SQLDateTime & ") "
                            strSQLAlert += "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key3DateTime) VALUES " &
                                                    "(" & oNotification.ID.ToString & ", " & Me.intIDEmployee.ToString & "," & Any2Time(xDailyDate.Date).SQLSmallDateTime & ")"

                            ExecuteSql(strSQLAlert)
                        Next
                    End If
                End If

                ' Si el fichaje indica que hay una alerta de empleado con temperatura alta, creamos la notificacion en caso necesario
                If (Me.TemperatureAlert.HasValue AndAlso Me.TemperatureAlert) AndAlso Me.intIDEmployee > 0 Then
                    Dim oNotificationState As New Notifications.roNotificationState(-1)
                    Dim oAlertNotifications As Generic.List(Of Notifications.roNotification) = Notifications.roNotification.GetNotifications("IDType = 73 And Activated=1", oNotificationState,, True)

                    If oAlertNotifications IsNot Nothing AndAlso oAlertNotifications.Count > 0 Then
                        For Each oNotification As Notifications.roNotification In oAlertNotifications
                            Dim xDailyDate As DateTime
                            If Me.xShiftDate.HasValue Then
                                xDailyDate = Me.xShiftDate.Value.Date
                            ElseIf Me.xDateTime.HasValue Then
                                xDailyDate = Me.xDateTime.Value.Date
                            End If
                            Dim strSQLAlert As String = ""
                            strSQLAlert = " IF NOT EXISTS (@SELECT# 1 FROM sysroNotificationTasks WHERE IDNotification=" & oNotification.ID.ToString & " AND Key1Numeric = " & Me.intIDEmployee.ToString & " AND  Key3DateTime = " & Any2Time(xDailyDate.Date).SQLDateTime & ") "
                            strSQLAlert += "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key3DateTime) VALUES " &
                                                    "(" & oNotification.ID.ToString & ", " & Me.intIDEmployee.ToString & "," & Any2Time(xDailyDate.Date).SQLSmallDateTime & ")"

                            ExecuteSql(strSQLAlert)
                        Next
                    End If
                End If

                bolRet = True
            Catch Ex As DbException
                If Ex.Message.Contains("UC_RepeteatedPunches") Then
                    olog.logMessage(roLog.EventType.roDebug, "This punch already exists!. It has been detected by Unique Constraint on table punches. It will not be saved. Detail: " & Me.ToString())
                    bolRet = True
                Else
                    Me.oState.UpdateStateInfo(Ex, "roPunch::Save")
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunc::Save")
            End Try

            Return bolRet

        End Function

        Private Shared Sub SendCapacityNotificationIfNeeded(zone As Zone.roZone, xPunch As Date)

            Try
                If zone IsNot Nothing AndAlso zone.Supervisor <> -1 AndAlso zone.Capacity.HasValue AndAlso zone.Capacity.Value > 0 Then
                    Dim oNotificationState As New Notifications.roNotificationState(-1)
                    Dim oAlertNotifications As Generic.List(Of Notifications.roNotification) = Notifications.roNotification.GetNotifications("IDType = 86 And Activated=1", oNotificationState, , True)

                    If oAlertNotifications IsNot Nothing AndAlso oAlertNotifications.Count > 0 Then
                        Dim iActualCapacity As Integer = roZone.GetEmployeesCurrentlyInZone(zone.ID, New roZoneState())

                        For Each oNotification As Notifications.roNotification In oAlertNotifications
                            Dim oParam As String = "REACHED"

                            Dim sSql As String = "@SELECT# Parameters FROM sysronotificationTasks WHERE IDNotification =" & oNotification.ID & " and key1Numeric = " & zone.Supervisor.ToString & " AND Key2Numeric = " & zone.ID.ToString & " and CAST(Key3DateTime AS DATE) = '" & xPunch.Date.ToString("yyyyMMdd") & "' ORDER BY Key3DateTime desc"
                            Dim sLastAction As String = roTypes.Any2String(ExecuteScalar(sSql))

                            Dim strSQLAlert As String = ""
                            If sLastAction = String.Empty OrElse sLastAction = "AVAILABLE" Then
                                If iActualCapacity = zone.Capacity.Value Then
                                    oParam = "REACHED"

                                    strSQLAlert += "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime, Parameters) VALUES " &
                                                    "(" & oNotification.ID.ToString & ", " & zone.Supervisor.ToString & "," & zone.ID & "," & Any2Time(xPunch).SQLDateTime & ",'" & oParam & "')"
                                End If
                            ElseIf sLastAction = "REACHED" Then
                                If iActualCapacity < zone.Capacity.Value Then
                                    oParam = "AVAILABLE"

                                    strSQLAlert += "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime,Parameters) VALUES " &
                                                    "(" & oNotification.ID.ToString & ", " & zone.Supervisor.ToString & "," & zone.ID & "," & Any2Time(xPunch).SQLDateTime & ",'" & oParam & "')"
                                End If
                            End If

                            If strSQLAlert <> String.Empty Then ExecuteSql(strSQLAlert)
                        Next
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "roPunch::SendCapacityNotificationIfNeeded::", ex)
            End Try

        End Sub

        Private Sub CheckIfAttControlled()
            Try
                Dim sField As String = String.Empty
                Dim sFieldValue As String = String.Empty
                sField = roTypes.Any2String(roAdvancedParameter.GetAdvancedParameterValue("AttControlledOnACCTA", Nothing, False))
                If Me.intType = PunchTypeEnum._L AndAlso sField.Length > 0 Then
                    ' Miro si la limitación es a nivel de empresa o a nivel de empleado
                    If sField.Trim.ToUpper.EndsWith("@EMP") Then
                        ' A nivel de empleado
                        Dim cSplitChar As String = String.Empty
                        If sField.Split("=").Count > 1 Then
                            cSplitChar = "="
                        ElseIf sField.Split("<>").Count > 1 Then
                            cSplitChar = "<>"
                        End If
                        If cSplitChar.Length > 0 Then
                            Dim oEmployeeState = New roUserFieldState(State.IDPassport)
                            Dim oEmpUserField As New roEmployeeUserField(oEmployeeState)
                            oEmpUserField = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, sField.Split(cSplitChar)(0), Now, oEmployeeState)
                            If oEmpUserField IsNot Nothing Then
                                sFieldValue = roTypes.Any2String(oEmpUserField.FieldValue)
                                Select Case cSplitChar
                                    Case "="
                                        If sField.Split(cSplitChar)(1).Substring(0, sField.Split(cSplitChar)(1).Length - 4).Trim.ToUpper <> sFieldValue.Trim.ToUpper Then
                                            Me.intType = PunchTypeEnum._AV
                                        End If
                                    Case "<>"
                                        If sField.Split(cSplitChar)(1).Substring(1, sField.Split(cSplitChar)(1).Length - 5).Trim.ToUpper = sFieldValue.Trim.ToUpper Then
                                            Me.intType = PunchTypeEnum._AV
                                        End If
                                End Select
                            End If
                        End If
                    Else
                        ' A nivel de empresa
                        If sField.Split("=").Count > 1 Then
                            Dim iCompanyId As Integer = 0
                            iCompanyId = roBusinessSupport.GetCurrentCompanyId(Me.intIDEmployee, State)
                            Dim oUserFieldState As New roUserFieldState(State.IDPassport)
                            Dim oGroupUserField As New roGroupUserField()
                            oGroupUserField = roGroupUserField.GetUserField(iCompanyId, "[USR_" & sField.Split("=")(0) & "]", oUserFieldState)
                            If oGroupUserField IsNot Nothing Then sFieldValue = roTypes.Any2String(oGroupUserField.FieldValue)
                            If sField.Split("=")(1).Trim.ToUpper <> sFieldValue.Trim.ToUpper Then
                                Me.intType = PunchTypeEnum._AV
                            End If
                        End If
                    End If
                Else
                    sField = roTypes.Any2String(roAdvancedParameter.GetAdvancedParameterValue("AttControlledOnACC", Nothing, False))
                    If Me.intType = PunchTypeEnum._AV AndAlso sField.Length > 0 AndAlso sField.Split("=").Count > 1 Then
                        ' Para fichajes de accesos, miro si al empleado se le controla presencia o no a través de un campo de la ficha del propio empleado ...
                        Dim oEmpUserFieldState As New roUserFieldState(State.IDPassport)
                        Dim oEmpUserField As New roEmployeeUserField()
                        oEmpUserField = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, sField.Split("=")(0), Now, oEmpUserFieldState)
                        If oEmpUserField IsNot Nothing Then sFieldValue = roTypes.Any2String(oEmpUserField.FieldValue)
                        If sField.Split("=")(1).Trim.ToUpper = sFieldValue.Trim.ToUpper Then
                            Me.intType = PunchTypeEnum._L
                        End If
                    End If
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch::Save:CheckIfAttControlled")
            End Try
        End Sub

        Private Function GetIDReaderUsed(oLog As roLog) As Integer
            Dim iReaderIDUsed As Integer = Me.bIDReader
            Try
                ' Recuperamos el lector, en caso de venir de terminal físico (por si no venía informado, o se informó uno que no existía...
                If Me.intIDTerminal <> 0 Then
                    ' Si no tengo informado el lector (esto no debería pasar), lo intento informar ahora.
                    If Me.bIDReader = -1 OrElse Me.bIDReader = 0 Then
                        ' Miro si el terminal tiene definido un lector 1. Si es así lo asigno ... (esto pasa con LivePortal por ejemplo)
                        Dim iFirstReader As Integer = -1
                        If Terminal.roTerminal.GetTerminalReaderCount(Me.intIDTerminal, iFirstReader) > 0 Then
                            Me.bIDReader = iFirstReader
                            iReaderIDUsed = Me.bIDReader
                        End If
                    Else
                        ' Si tengo el lector pero no está configurado, es que viene de un terminal NET (varios lectores, pero todos con una misma configuración (ID=1)
                        If Terminal.roTerminal.GetTerminalReaderMode(Me.intIDTerminal, Me.bIDReader) = "" Then
                            ' Me quedo con el primer
                            iReaderIDUsed = 1
                        End If
                    End If
                End If
                Return iReaderIDUsed
            Catch ex As Exception
                oLog.logMessage(roLog.EventType.roDebug, "Save::Debug: Using reader 1 configuration because reader " & Me.bIDReader.ToString & " does exist but is not configured !.Got exception: " & ex.Message.ToString)
                Return 1
            End Try
        End Function

        ''' <summary>
        ''' Obtiene el CRC de un fichaje
        ''' </summary>
        ''' <param name="oRow">Fila de la tabla Puches</param>
        ''' <param name="version">Versión de algoritmo</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCRC(ByVal oRow As System.Data.DataRow, Optional ByVal version As Integer = 2) As String
            'CAMBIAR EL VALOR POR DEFECTO DE LA VARIABLE OPCIONAL version CUANDO SE CREE UNA NUEVA VERSION DE ALGORITMO
            Try
                Dim crc_base As String = ""
                Dim res As String
                Dim iCurrentAlgorithmVersion As Integer
                iCurrentAlgorithmVersion = version

                If version = 2 Then
                    ' Verisón actual de algoritmo
                    crc_base = Format(IIf(IsDBNull(oRow("Field6")), 0, oRow("Field6")), "0.0000") & "@" &
                    oRow("IDEmployee") & "|" &
                    oRow("Type") & "/" &
                    oRow("Field3") & "=" &
                    oRow("TypeData") & "&" &
                    CDate(oRow("DateTime")).ToString("HHmmss") & "{" &
                    CDate(oRow("DateTime")).ToString("yyyyMMdd") & "}" &
                    oRow("IDTerminal") & "-" &
                    oRow("IDReader") & "_" &
                    oRow("Field2") & "<" &
                    oRow("Location") & ">" &
                    Format(IIf(IsDBNull(oRow("Field5")), 0, oRow("Field5")), "0.0000") & "R" &
                    oRow("IP") & "o" &
                    oRow("IsNotReliable") & "B" &
                    oRow("Action") & "o" &
                    oRow("IDPassport") & "T" &
                    oRow("Field1") & "i" &
                    oRow("LocationZone") & "C" &
                    oRow("IDZone") & "s" &
                    Format(IIf(IsDBNull(oRow("Field4")), 0, oRow("Field4")), "0.0000") & "V" &
                    oRow("ActualType") & "i" &
                    oRow("TypeDetails") & "S" &
                    oRow("IDCredential") & "l" &
                    oRow("InvalidType") & "U" &
                    oRow("TimeZone")
                Else
                    ' Versiones antiguas
                    Select Case version
                        Case 1
                            crc_base = Format(IIf(IsDBNull(oRow("Field6")), 0, oRow("Field6")), "0.0000") & "@" &
                            oRow("IDEmployee") & "|" &
                            oRow("Type") & "/" &
                            oRow("Field3") & "=" &
                            oRow("TypeData") & "&" &
                            CDate(oRow("DateTime")).ToString("hhmmss") & "{" &
                            CDate(oRow("DateTime")).ToString("yyyyMMdd") & "}" &
                            oRow("IDTerminal") & "-" &
                            oRow("IDReader") & "_" &
                            oRow("Field2") & "<" &
                            oRow("Location") & ">" &
                            Format(IIf(IsDBNull(oRow("Field5")), 0, oRow("Field5")), "0.0000") & "R" &
                            oRow("IP") & "o" &
                            oRow("IsNotReliable") & "B" &
                            oRow("Action") & "o" &
                            oRow("IDPassport") & "T" &
                            oRow("Field1") & "i" &
                            oRow("LocationZone") & "C" &
                            oRow("IDZone") & "s" &
                            Format(IIf(IsDBNull(oRow("Field4")), 0, oRow("Field4")), "0.0000") & "V" &
                            oRow("ActualType") & "i" &
                            oRow("TypeDetails") & "S" &
                            oRow("IDCredential") & "l" &
                            oRow("InvalidType") & "U" &
                            oRow("TimeZone")
                    End Select
                End If

                Dim o As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5.Create
                Dim arr As Byte() = o.ComputeHash(System.Text.Encoding.UTF8.GetBytes(crc_base))
                Dim sBuilder As New System.Text.StringBuilder()

                Dim i As Integer
                For i = 0 To arr.Length - 1
                    sBuilder.Append(arr(i).ToString("x2"))
                Next i
                res = sBuilder.ToString()
                Return res.Substring(0, 5) & res.Substring(27, 5) & "." & iCurrentAlgorithmVersion.ToString
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch::GetCRC")
                Return "error"
            End Try
        End Function

        ''' <summary>
        ''' Verifica si el CRC de un fichaje es correcto
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckCRC() As Boolean

            Dim bolRet As Boolean = False

            If Me.lngID > 0 Then

                Try

                    Dim tb As DataTable = CreateDataTable("@SELECT# * FROM Punches WHERE [ID] = " & Me.ID.ToString, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        'Usar el código siguiente cuando haya versiones de algoritmo diferente a la 1
                        Dim oRow As DataRow = tb.Rows(0)
                        Dim version As Integer = oRow("CRC").ToString.Split(".")(1)
                        Return oRow("CRC") = GetCRC(oRow, version)
                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roPunch::CheckCRC")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roPunch::CheckCRC")
                End Try

            End If

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina fichaje
        ''' </summary>
        Public Function Delete(Optional ByVal bolInitTask As Boolean = True, Optional ByVal bolAudit As Boolean = True, Optional ByVal bolReprocessPunches As Boolean = True, Optional ByVal bolNotifyExport As Boolean = True, Optional ByVal bolUpdateDailyStatus As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim idNotifyEmployeeID As Integer = -1
                If bolNotifyExport Then
                    Dim tmpSql As String = "@SELECT# IdEmployee FROM Punches WHERE ID = " & Me.ID.ToString
                    Dim obj As Object = ExecuteScalar(tmpSql)
                    If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                        idNotifyEmployeeID = roTypes.Any2Integer(obj)
                    End If
                End If

                Dim sSql As String = String.Empty

                If bolRet Then

                    ' Cargamos los datos del fichaje
                    Dim oPunch As New roPunch(Me.intIDEmployee, Me.ID.ToString, False, Me.State)

                    ' Borramos fichaje si hay
                    If oPunch.HasPhoto Then
                        If oPunch.PhotoOnAzure Then
                            Azure.RoAzureSupport.DeletePunchPhotoFile(oPunch.ID)
                        Else
                            sSql = "@DELETE# FROM PunchesCaptures WHERE IDPunch = " & Me.ID.ToString
                            bolRet = ExecuteSql(sSql)
                        End If
                    End If

                    sSql = "@DELETE# FROM Punches WHERE [ID] = " & Me.ID.ToString
                    bolRet = ExecuteSql(sSql)
                    If bolRet Then

                        ' Guardamos las transacciones para generar backup de fichajes.
                        Try
                            Dim sqlBCKCommandText As String
                            Dim sCRCWhere As String
                            If (Me.CRC Is Nothing OrElse Me.CRC.Length = 0) Then sCRCWhere = " is NULL " Else sCRCWhere = " = '" & Me.CRC & "'"
                            sqlBCKCommandText = "@DELETE# FROM PunchesCaptures WHERE IDPunch in (@SELECT# ID FROM PUNCHES WHERE [CRC] " & sCRCWhere & " AND IDEmployee = " & Me.IDEmployee.ToString & " AND DateTime = " & Any2Time(Me.DateTime).SQLDateTime & ");"
                            sqlBCKCommandText = sqlBCKCommandText & "@DELETE# FROM Punches WHERE [CRC] " & sCRCWhere & " AND IDEmployee = " & Me.IDEmployee.ToString & " AND DateTime = " & Any2Time(Me.DateTime).SQLDateTime
                            sqlBCKCommandText = sqlBCKCommandText.Replace("'", "''")
                            ExecuteSql("@INSERT# INTO sysroPunchesTransactions values ('" & sqlBCKCommandText & "'," & Any2Time(Me.ShiftDate).SQLSmallDateTime & "," & Me.IDEmployee.ToString & "," & Any2Time(Now).SQLDateTime & ",0)")
                        Catch ex As Exception
                            Me.oState.UpdateStateInfo(ex, "roPunch::Delete:SavePunchesTransactionsSQL")
                        End Try

                        If bolNotifyExport Then
                            Dim customization As String = roBusinessSupport.GetCustomizationCode().ToUpper()
                            If roTypes.Any2String(customization) = "PU" Then
                                sSql = "@INSERT# INTO sysroDeletedPunchesSync(PunchID,EmployeeID) VALUES (" & Me.ID.ToString & "," & idNotifyEmployeeID & ")"
                                bolRet = ExecuteSql(sSql)
                            End If
                        End If

                        ' Obtenemos el tipo de fichaje actual
                        Dim _intActualType As PunchTypeEnum
                        _intActualType = PunchTypeEnum._NOTDEFINDED
                        If Me.intActualType.HasValue Then
                            _intActualType = Me.intActualType
                        End If

                        ' Si el fichaje es de presencia
                        If (_intActualType = PunchTypeEnum._IN OrElse _intActualType = PunchTypeEnum._OUT OrElse _intActualType = PunchTypeEnum._CENTER OrElse Me.intType = PunchTypeEnum._L) AndAlso Me.intIDEmployee > 0 Then
                            ' Actualizamos el status
                            If bolUpdateDailyStatus Then
                                UpdateDailySchedule()
                            End If

                            ' Notificamos al servidor
                            If bolInitTask Then
                                Dim oContext As New roCollection
                                oContext.Add("User.ID", Me.intIDEmployee)
                                roConnector.InitTask(TasksType.MOVES)
                            End If

                            ' Reprocesamos los fichajes posteriores en caso necesario
                            If bolReprocessPunches Then
                                ReprocessPunches(Me.intIDEmployee, oPunch.xDateTime)
                            End If

                        ElseIf _intActualType = PunchTypeEnum._TASK Then
                            ' Si es de tareas

                            ' Actualizamos el status de tareas
                            If bolUpdateDailyStatus Then
                                UpdateDailyTaskSchedule()
                            End If

                            ' Notificamos al servidor
                            If bolInitTask Then
                                roConnector.InitTask(TasksType.TASKS)
                            End If

                        End If

                        If _intActualType = PunchTypeEnum._CENTER Then
                            ' Si es de centro de coste

                            ' Actualizamos el status de tareas
                            If bolUpdateDailyStatus Then
                                UpdateDailyCenterSchedule()
                            End If

                            ' Notificamos al servidor
                            If bolInitTask Then
                                Dim oContext As New roCollection
                                oContext.Add("User.ID", Me.intIDEmployee)
                                roConnector.InitTask(TasksType.MOVES)
                            End If

                        End If

                        If bolAudit Then
                            ' Auditamos borrado fichaje
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            Dim strObjectName As String = ""
                            If Me.xDateTime.HasValue Then strObjectName &= "DateTime: " & Me.xDateTime.Value.ToString("dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture)
                            If Me.intActualType.HasValue Then strObjectName &= " ActualType: " & Me.intActualType.Value.ToString
                            Dim oEmployeeState As New Employee.roEmployeeState(Me.oState.IDPassport)
                            Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(Me.intIDEmployee, oEmployeeState, False)
                            If oEmployee IsNot Nothing Then
                                strObjectName &= " (" & oEmployee.Name & ")"
                            End If
                            oState.AddAuditParameter(tbParameters, "{MoveName}", strObjectName, "", 1)
                            bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tMove, strObjectName, tbParameters, -1)

                        End If

                        Me.ID = 0
                        Me.Load()

                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roPunch::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch::Delete")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda captura del fichaje
        ''' </summary>
        Public Function SaveCapture(bDeletePhotoFromDatabase As Boolean) As Boolean

            Dim bolRet As Boolean = False
            Dim strSQL As String = String.Empty
            Try
                Dim ms As IO.MemoryStream
                ms = New IO.MemoryStream(roEncrypt.Encrypt(roTypes.Image2Bytes(Me.Capture)))

                If Azure.RoAzureSupport.UploadPunchPhoto2Azure(ms, Me.ID.ToString, True) Then
                    If bDeletePhotoFromDatabase Then
                        strSQL = "@DELETE# PunchesCaptures WHERE IDPunch = " & Me.ID.ToString & "; @UPDATE# Punches SET PhotoOnAzure = 1 WHERE ID = " & Me.ID.ToString
                        AccessHelper.ExecuteSql(strSQL)
                    End If
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roPunch::SaveCaptureOnOnAzure:Unable to save punch photo for punchid " & Me.ID.ToString & "!!")
                End If

                bolRet = True
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roPunch::SaveCapture")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch::SaveCapture")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda captura del fichaje
        ''' </summary>
        Private Function SaveCaptureOld() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim tbCaptures As New DataTable("PunchesCaptures")
                Dim strSQL As String = "@SELECT# * FROM PunchesCaptures WHERE IDPunch = " & Me.lngID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tbCaptures)

                Dim oRow As DataRow = Nothing
                Dim bolNewRow As Boolean = False
                If tbCaptures.Rows.Count = 0 Then
                    oRow = tbCaptures.NewRow
                    bolNewRow = True
                    oRow("IDPunch") = Me.lngID
                Else
                    oRow = tbCaptures.Rows(0)
                End If

                If Me.oCapture IsNot Nothing Then
                    oRow("Capture") = roTypes.Image2Bytes(Me.oCapture)
                End If

                If bolNewRow Then
                    tbCaptures.Rows.Add(oRow)
                End If

                da.Update(tbCaptures)

                bolRet = True
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roPunch::SaveCapture")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch::SaveCapture")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Carga en memoria informacion de un fichaje de entrada ya creado
        ''' </summary>
        Public Sub LoadNewIN(ByVal _DateTimeIN As Nullable(Of DateTime), ByVal _IDTerminalIN As Integer, Optional ByVal _IDPunch As Long = -1, Optional ByVal _IDCauseIN As Integer = -1, Optional ByVal _IDZoneIN As Integer = -1, Optional ByVal _ReaderTypeIN As Integer = -1, Optional ByVal _CaptureIN As Image = Nothing, Optional ByVal _IsNotReliableIN As Boolean = False, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal LocationZone As String = "", Optional ByVal TimeZone As String = "")

            Me.ID = _IDPunch
            Me.Load()

            Me.xDateTime = _DateTimeIN
            Me.xShiftDate = Any2Time(_DateTimeIN).DateOnly
            Me.intIDTerminal = _IDTerminalIN
            Me.intTypeData = _IDCauseIN
            Me.intIDZone = _IDZoneIN
            Me.oCapture = _CaptureIN
            Me.intType = PunchTypeEnum._IN
            Me.intActualType = PunchTypeEnum._IN
            Me.bolIsNotReliable = _IsNotReliableIN
            If _Lat <> -1 AndAlso _Lon <> -1 Then
                Me.strLocation = Replace(_Lat.ToString, ",", ".") & "," & Replace(_Lon.ToString.ToString, ",", ".")
            End If

            If LocationZone.Length > 0 Then
                Me.strLocationZone = LocationZone
            End If

            If TimeZone.Length > 0 Then
                Me.strTimeZone = TimeZone
            End If

            RaiseEvent OnLoadNewIN(Me)

        End Sub

        Public Sub LoadNewIN_Ext(ByVal _DateTimeIN As Nullable(Of DateTime), ByVal _IDTerminalIN As Integer, Optional ByVal _IDPunch As Long = -1, Optional ByVal _IDCauseIN As Integer = -1, Optional ByVal _IDZoneIN As Integer = -1, Optional ByVal _ReaderTypeIN As Integer = -1, Optional ByVal _CaptureIN As Image = Nothing, Optional ByVal _IsNotReliableIN As Boolean = False, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal LocationZone As String = "", Optional ByVal FullAddress As String = "", Optional ByVal TimeZone As String = "", Optional ByVal _IsNotReliableZone As Boolean = False)

            Me.ID = _IDPunch
            Me.Load()

            Me.xDateTime = _DateTimeIN
            Me.xShiftDate = Any2Time(_DateTimeIN).DateOnly
            Me.intIDTerminal = _IDTerminalIN
            Me.intTypeData = _IDCauseIN
            Me.intIDZone = _IDZoneIN
            Me.oCapture = _CaptureIN
            Me.intType = PunchTypeEnum._IN
            Me.intActualType = PunchTypeEnum._IN
            Me.bolIsNotReliable = _IsNotReliableIN
            Me.bolZoneIsNotReliable = _IsNotReliableZone
            If _Lat <> -1 AndAlso _Lon <> -1 Then
                Me.strLocation = Replace(_Lat.ToString, ",", ".") & "," & Replace(_Lon.ToString.ToString, ",", ".")
            End If

            If LocationZone.Length > 0 Then
                Me.strLocationZone = LocationZone
            End If

            If FullAddress.Length > 0 Then
                Me.strFullAddress = FullAddress
            End If

            If TimeZone.Length > 0 Then
                Me.strTimeZone = TimeZone
            End If

            RaiseEvent OnLoadNewIN(Me)

        End Sub

        Public Sub LoadNewAcc(ByVal _DateTime As Nullable(Of DateTime), ByVal _IDTerminal As Integer, Optional ByVal _IDPunch As Long = -1, Optional ByVal _IDZone As Integer = -1, Optional ByVal _ReaderTypeIN As Integer = -1, Optional ByVal _Capture As Image = Nothing, Optional ByVal _IsNotReliable As Boolean = False, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal LocationZone As String = "", Optional ByVal FullAddress As String = "", Optional ByVal TimeZone As String = "")

            Me.ID = _IDPunch
            Me.Load()

            Me.xDateTime = _DateTime
            Me.xShiftDate = Any2Time(_DateTime).DateOnly
            Me.intIDTerminal = _IDTerminal
            Me.intIDZone = _IDZone
            Me.oCapture = _Capture
            Me.intType = PunchTypeEnum._AV
            Me.intActualType = PunchTypeEnum._AV
            Me.bolIsNotReliable = _IsNotReliable
            If _Lat <> -1 AndAlso _Lon <> -1 Then
                Me.strLocation = Replace(_Lat.ToString, ",", ".") & "," & Replace(_Lon.ToString.ToString, ",", ".")
            End If

            If LocationZone.Length > 0 Then
                Me.strLocationZone = LocationZone
            End If

            If FullAddress.Length > 0 Then
                Me.strFullAddress = FullAddress
            End If

            If TimeZone.Length > 0 Then
                Me.strTimeZone = TimeZone
            End If

            RaiseEvent OnLoadNewACC(Me)

        End Sub

        Public Sub LoadNewBEGIN(ByVal _DateTimeIN As Nullable(Of DateTime), ByVal _IDTerminalIN As Integer, ByVal _Task As Task.roTask, Optional ByVal _IDPunch As Long = -1, Optional ByVal _IDZoneIN As Integer = -1, Optional ByVal _ReaderTypeIN As Integer = -1, Optional ByVal _CaptureIN As Image = Nothing, Optional ByVal _IsNotReliableIN As Boolean = False, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal LocationZone As String = "", Optional ByVal TimeZone As String = "", Optional ByVal Field1 As String = "", Optional ByVal Field2 As String = "", Optional ByVal Field3 As String = "", Optional ByVal Field4 As Double = -1, Optional ByVal Field5 As Double = -1, Optional ByVal Field6 As Double = -1)

            Me.ID = _IDPunch
            Me.Load()

            Me.xDateTime = _DateTimeIN
            Me.xShiftDate = Any2Time(_DateTimeIN).DateOnly
            Me.intIDTerminal = _IDTerminalIN
            Me.intTypeData = _Task.ID

            Me.strField1 = Field1
            Me.strField2 = Field2
            Me.strField3 = Field3
            Me.dblField4 = Field4
            Me.dblField5 = Field5
            Me.dblField6 = Field6

            Me.intIDZone = _IDZoneIN
            Me.oCapture = _CaptureIN
            Me.intType = PunchTypeEnum._TASK
            Me.intActualType = PunchTypeEnum._TASK
            Me.bolIsNotReliable = _IsNotReliableIN
            If _Lat <> -1 AndAlso _Lon <> -1 Then
                Me.strLocation = Replace(_Lat.ToString, ",", ".") & "," & Replace(_Lon.ToString.ToString, ",", ".")
            End If

            If LocationZone.Length > 0 Then
                Me.strLocationZone = LocationZone
            End If

            If TimeZone.Length > 0 Then
                Me.strTimeZone = TimeZone
            End If

            RaiseEvent OnLoadNewBegin(Me)
        End Sub

        Public Sub LoadNewBEGIN_Ext(ByVal _DateTimeIN As Nullable(Of DateTime), ByVal _IDTerminalIN As Integer, ByVal _Task As Task.roTask, Optional ByVal _IDPunch As Long = -1, Optional ByVal _IDZoneIN As Integer = -1, Optional ByVal _ReaderTypeIN As Integer = -1, Optional ByVal _CaptureIN As Image = Nothing, Optional ByVal _IsNotReliableIN As Boolean = False, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal LocationZone As String = "", Optional ByVal FullAddress As String = "", Optional ByVal TimeZone As String = "", Optional ByVal Field1 As String = "", Optional ByVal Field2 As String = "", Optional ByVal Field3 As String = "", Optional ByVal Field4 As Double = -1, Optional ByVal Field5 As Double = -1, Optional ByVal Field6 As Double = -1)

            Me.ID = _IDPunch
            Me.Load()

            Me.xDateTime = _DateTimeIN
            Me.xShiftDate = Any2Time(_DateTimeIN).DateOnly
            Me.intIDTerminal = _IDTerminalIN
            Me.intTypeData = _Task.ID

            Me.strField1 = Field1
            Me.strField2 = Field2
            Me.strField3 = Field3
            Me.dblField4 = Field4
            Me.dblField5 = Field5
            Me.dblField6 = Field6

            Me.intIDZone = _IDZoneIN
            Me.oCapture = _CaptureIN
            Me.intType = PunchTypeEnum._TASK
            Me.intActualType = PunchTypeEnum._TASK
            Me.bolIsNotReliable = _IsNotReliableIN
            If _Lat <> -1 AndAlso _Lon <> -1 Then
                Me.strLocation = Replace(_Lat.ToString, ",", ".") & "," & Replace(_Lon.ToString.ToString, ",", ".")
            End If

            If LocationZone.Length > 0 Then
                Me.strLocationZone = LocationZone
            End If

            If FullAddress.Length > 0 Then
                Me.strFullAddress = FullAddress
            End If

            If TimeZone.Length > 0 Then
                Me.strTimeZone = TimeZone
            End If

            RaiseEvent OnLoadNewBegin(Me)
        End Sub

        Public Sub LoadNewCostCenterIN(ByVal _DateTimeIN As Nullable(Of DateTime), ByVal _IDCostCenter As Integer, ByVal _IDTerminalIN As Integer, Optional ByVal _IDPunch As Long = -1, Optional ByVal _IDZoneIN As Integer = -1, Optional ByVal _ReaderTypeIN As Integer = -1, Optional ByVal _CaptureIN As Image = Nothing, Optional ByVal _IsNotReliableIN As Boolean = False, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal LocationZone As String = "", Optional ByVal TimeZone As String = "")

            Me.ID = _IDPunch
            Me.Load()

            Me.xDateTime = _DateTimeIN
            Me.xShiftDate = Any2Time(_DateTimeIN).DateOnly
            Me.intIDTerminal = _IDTerminalIN
            Me.intTypeData = _IDCostCenter
            Me.intIDZone = _IDZoneIN
            Me.oCapture = _CaptureIN
            Me.intType = PunchTypeEnum._CENTER
            Me.intActualType = PunchTypeEnum._CENTER
            Me.bolIsNotReliable = _IsNotReliableIN
            If _Lat <> -1 AndAlso _Lon <> -1 Then
                Me.strLocation = Replace(_Lat.ToString, ",", ".") & "," & Replace(_Lon.ToString.ToString, ",", ".")
            End If

            If LocationZone.Length > 0 Then
                Me.strLocationZone = LocationZone
            End If

            If TimeZone.Length > 0 Then
                Me.strTimeZone = TimeZone
            End If

            RaiseEvent OnLoadNewCostCenter(Me)

        End Sub

        Public Sub LoadNewCostCenterIN_Ext(ByVal _DateTimeIN As Nullable(Of DateTime), ByVal _IDCostCenter As Integer, ByVal _IDTerminalIN As Integer, Optional ByVal _IDPunch As Long = -1, Optional ByVal _IDZoneIN As Integer = -1, Optional ByVal _ReaderTypeIN As Integer = -1, Optional ByVal _CaptureIN As Image = Nothing, Optional ByVal _IsNotReliableIN As Boolean = False, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal LocationZone As String = "", Optional ByVal FullAddress As String = "", Optional ByVal TimeZone As String = "")

            Me.ID = _IDPunch
            Me.Load()

            Me.xDateTime = _DateTimeIN
            Me.xShiftDate = Any2Time(_DateTimeIN).DateOnly
            Me.intIDTerminal = _IDTerminalIN
            Me.intTypeData = _IDCostCenter
            Me.intIDZone = _IDZoneIN
            Me.oCapture = _CaptureIN
            Me.intType = PunchTypeEnum._CENTER
            Me.intActualType = PunchTypeEnum._CENTER
            Me.bolIsNotReliable = _IsNotReliableIN
            If _Lat <> -1 AndAlso _Lon <> -1 Then
                Me.strLocation = Replace(_Lat.ToString, ",", ".") & "," & Replace(_Lon.ToString.ToString, ",", ".")
            End If

            If LocationZone.Length > 0 Then
                Me.strLocationZone = LocationZone
            End If

            If FullAddress.Length > 0 Then
                Me.strFullAddress = FullAddress
            End If

            If TimeZone.Length > 0 Then
                Me.strTimeZone = TimeZone
            End If

            RaiseEvent OnLoadNewCostCenter(Me)

        End Sub

        ''' <summary>
        ''' Carga en memoria informacion de un fichaje de salida ya creado
        ''' </summary>
        Public Sub LoadNewOUT(ByVal _DateTimeOUT As Nullable(Of DateTime), ByVal _IDTerminalOUT As Integer, Optional ByVal _IDPunch As Long = -1, Optional ByVal _IDCauseOUT As Integer = -1, Optional ByVal _IDZoneOUT As Integer = -1, Optional ByVal _ReaderTypeOUT As Integer = -1, Optional ByVal _CaptureOUT As Image = Nothing, Optional ByVal _IsNotReliableOUT As Boolean = False, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal LocationZone As String = "", Optional ByVal TimeZone As String = "")

            Me.ID = _IDPunch
            Me.Load()

            Me.xDateTime = _DateTimeOUT
            If Not Me.xShiftDate.HasValue Then Me.xShiftDate = Any2Time(_DateTimeOUT).DateOnly
            Me.intIDTerminal = _IDTerminalOUT
            Me.intTypeData = _IDCauseOUT
            Me.intIDZone = _IDZoneOUT
            Me.oCapture = _CaptureOUT
            Me.bolIsNotReliable = _IsNotReliableOUT
            Me.intType = PunchTypeEnum._OUT
            Me.intActualType = PunchTypeEnum._OUT

            If _Lat <> -1 AndAlso _Lon <> -1 Then
                Me.strLocation = Replace(_Lat.ToString, ",", ".") & "," & Replace(_Lon.ToString.ToString, ",", ".")
            End If

            If LocationZone.Length > 0 Then
                Me.strLocationZone = LocationZone
            End If

            If TimeZone.Length > 0 Then
                Me.strTimeZone = TimeZone
            End If

            RaiseEvent OnLoadNewOUT(Me)

        End Sub

        ''' <summary>
        ''' Carga en memoria informacion de un fichaje de salida ya creado
        ''' </summary>
        Public Sub LoadNewOUT_Ext(ByVal _DateTimeOUT As Nullable(Of DateTime), ByVal _IDTerminalOUT As Integer, Optional ByVal _IDPunch As Long = -1, Optional ByVal _IDCauseOUT As Integer = -1, Optional ByVal _IDZoneOUT As Integer = -1, Optional ByVal _ReaderTypeOUT As Integer = -1, Optional ByVal _CaptureOUT As Image = Nothing, Optional ByVal _IsNotReliableOUT As Boolean = False, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal LocationZone As String = "", Optional ByVal FullAddress As String = "", Optional ByVal TimeZone As String = "", Optional ByVal _IsNotReliableZone As Boolean = False)

            Me.ID = _IDPunch
            Me.Load()

            Me.xDateTime = _DateTimeOUT
            If Not Me.xShiftDate.HasValue Then Me.xShiftDate = Any2Time(_DateTimeOUT).DateOnly
            Me.intIDTerminal = _IDTerminalOUT
            Me.intTypeData = _IDCauseOUT
            Me.intIDZone = _IDZoneOUT
            Me.oCapture = _CaptureOUT
            Me.bolIsNotReliable = _IsNotReliableOUT
            Me.bolZoneIsNotReliable = _IsNotReliableZone
            Me.intType = PunchTypeEnum._OUT
            Me.intActualType = PunchTypeEnum._OUT

            If _Lat <> -1 AndAlso _Lon <> -1 Then
                Me.strLocation = Replace(_Lat.ToString, ",", ".") & "," & Replace(_Lon.ToString.ToString, ",", ".")
            End If

            If LocationZone.Length > 0 Then
                Me.strLocationZone = LocationZone
            End If

            If FullAddress.Length > 0 Then
                Me.strFullAddress = FullAddress
            End If

            If TimeZone.Length > 0 Then
                Me.strTimeZone = TimeZone
            End If

            RaiseEvent OnLoadNewOUT(Me)

        End Sub

        ''' <summary>
        ''' Carga en memoria informacion de un fichaje de entrada
        ''' </summary>
        Public Sub LoadIN(ByVal _DateTimeIN As Nullable(Of DateTime), ByVal _IDTerminalIN As Integer, Optional ByVal _IDCauseIN As Integer = -1, Optional ByVal _IDZoneIN As Integer = -1, Optional ByVal _ReaderTypeIN As Integer = -1, Optional ByVal _CaptureIN As Image = Nothing, Optional ByVal _IsNotReliableIN As Boolean = False)

            Me.xDateTime = _DateTimeIN
            Me.xShiftDate = Any2Time(_DateTimeIN).DateOnly
            Me.intIDTerminal = _IDTerminalIN
            Me.intTypeData = _IDCauseIN
            Me.intIDZone = _IDZoneIN
            Me.oCapture = _CaptureIN
            Me.bolIsNotReliable = _IsNotReliableIN
            Me.intType = PunchTypeEnum._IN

            RaiseEvent OnLoadIN(Me)

        End Sub

        ''' <summary>
        ''' Carga en memoria informacion de un fichaje de salida
        ''' </summary>
        Public Sub LoadOUT(ByVal _DateTimeOUT As Nullable(Of DateTime), ByVal _IDTerminalOUT As Integer, Optional ByVal _IDCauseOUT As Integer = -1, Optional ByVal _IDZoneOUT As Integer = -1, Optional ByVal _ReaderTypeOUT As Integer = -1, Optional ByVal _CaptureOUT As Image = Nothing, Optional ByVal _IsNotReliableOUT As Boolean = False)

            Me.xDateTime = _DateTimeOUT
            If Not Me.xShiftDate.HasValue Then Me.xShiftDate = Any2Time(_DateTimeOUT).DateOnly
            Me.intIDTerminal = _IDTerminalOUT
            Me.intTypeData = _IDCauseOUT
            Me.intIDZone = _IDZoneOUT
            Me.oCapture = _CaptureOUT
            Me.bolIsNotReliable = _IsNotReliableOUT
            Me.intType = PunchTypeEnum._OUT

            RaiseEvent OnLoadOUT(Me)

        End Sub

        ''' <summary>
        ''' Obtiene el último fichaje de tareas
        ''' </summary>
        Public Sub GetLastPunchTask(ByRef oLastPunchType As PunchStatus, ByRef xLastPunchDateTime As DateTime, ByRef lngLastPunchID As Long)

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# TOP 1 ID, DateTime, ActualType FROM Punches " &
                                       "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND ActualType IN(" & PunchTypeEnum._TASK & ")" &
                                       " ORDER BY DateTime DESC, ID DESC"
                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oLastPunchType = PunchStatus.Task_
                    xLastPunchDateTime = CDate(tb.Rows(0)("DateTime"))
                    lngLastPunchID = tb.Rows(0)("ID")
                Else
                    oLastPunchType = PunchStatus.Indet_
                    lngLastPunchID = -1
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roPunch:GetLastPunchTask")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch:GetLastPunchTask")
            End Try

        End Sub

        ''' <summary>
        ''' Obtiene el último fichaje de presencia
        ''' </summary>
        Public Sub GetLastPunchPres(ByRef oLastPunchType As PunchStatus, ByRef xLastPunchDateTime As DateTime, ByRef lngLastPunchID As Long)

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# TOP 1 ID, DateTime, ActualType FROM Punches " &
                                       "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND ActualType IN(" & PunchTypeEnum._IN & "," & PunchTypeEnum._OUT & ")" &
                                       " ORDER BY DateTime DESC, ID DESC"
                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If tb.Rows(0)("ActualType") = PunchTypeEnum._IN Then
                        oLastPunchType = PunchStatus.In_
                    Else
                        oLastPunchType = PunchStatus.Out_
                    End If
                    xLastPunchDateTime = CDate(tb.Rows(0)("DateTime"))
                    lngLastPunchID = tb.Rows(0)("ID")
                Else
                    oLastPunchType = PunchStatus.Indet_
                    lngLastPunchID = -1
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roPunch:GetLastPunchPres")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch:GetLastPunchPres")
            End Try

        End Sub

        ''' <summary>
        ''' Obtiene el último fichaje de presencia
        ''' </summary>
        Public Sub GetLastPunchPresEx(ByRef oLastPunchType As PunchStatus, ByRef xLastPunchDateTime As DateTime, ByRef lngLastPunchID As Long, ByRef xLastPunchShiftDate As Date)

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# TOP 1 ID, DateTime, ActualType, Shiftdate FROM Punches " &
                                       "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND ActualType IN(" & PunchTypeEnum._IN & "," & PunchTypeEnum._OUT & ")" &
                                       " ORDER BY DateTime DESC, ID DESC"
                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If tb.Rows(0)("ActualType") = PunchTypeEnum._IN Then
                        oLastPunchType = PunchStatus.In_
                    Else
                        oLastPunchType = PunchStatus.Out_
                    End If
                    xLastPunchDateTime = CDate(tb.Rows(0)("DateTime"))
                    lngLastPunchID = tb.Rows(0)("ID")
                    xLastPunchShiftDate = CDate(tb.Rows(0)("ShiftDate"))
                Else
                    oLastPunchType = PunchStatus.Indet_
                    lngLastPunchID = -1
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roPunch:GetLastPunchPres")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch:GetLastPunchPres")
            End Try

        End Sub

        ''' <summary>
        ''' Obtiene información sobre el último fichaje de accesos
        ''' </summary>
        Public Sub GetLastAccPunchInfo(ByRef oLastPunchType As Integer, ByRef xLastPunchDateTime As DateTime, ByRef lngLastPunchID As Long, ByRef iLastPunchZone As Long, Optional ByVal sAPBControlledZones As String = "")

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# TOP 1 ID, DateTime, Type, ActualType, IDZone FROM Punches " &
                                       "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND Type IN(" & PunchTypeEnum._AV & "," & PunchTypeEnum._L & ")"
                If sAPBControlledZones <> "" Then strSQL = strSQL & " AND IDZone in (" & sAPBControlledZones & ") "
                strSQL = strSQL & " ORDER BY DateTime DESC, ID DESC"

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oLastPunchType = tb.Rows(0)("Type")
                    xLastPunchDateTime = CDate(tb.Rows(0)("DateTime"))
                    lngLastPunchID = tb.Rows(0)("ID")
                    iLastPunchZone = tb.Rows(0)("IDZone")
                Else
                    oLastPunchType = PunchStatus.Indet_
                    lngLastPunchID = -1
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roPunch:GetLastAccPunchInfo")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch:GetLastAccPunchInfo")
            End Try

        End Sub

        ''' <summary>
        ''' Crea un fichaje de producción automatico
        ''' </summary>
        Private Function AutomaticJobPunch(ByVal bolAutomaticBeginJobCheck As Boolean, ByVal bolAutomaticFinishJobCheck As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Try

                If Not bolAutomaticBeginJobCheck AndAlso Not bolAutomaticFinishJobCheck Then
                    bolRet = True
                    Return bolRet
                End If

                Dim oEmployeeState As New Employee.roEmployeeState
                Dim oEmployee As Employee.roEmployee = Nothing
                Dim oMoveState As New Move.roMoveState

                If bolAutomaticBeginJobCheck AndAlso Me.intType = PunchTypeEnum._IN Then
                    ' Es una entrada
                    Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
                    If oSettings.GetVTSetting(eKeys.AutomaticBeginJob) Then ' Está configurada la opción inicios automáticos de producción
                        oEmployee = Employee.roEmployee.GetEmployee(Me.intIDEmployee, oEmployeeState, )
                        If oEmployee.ID > 0 AndAlso oEmployee.Type = "J" Then
                            ' El empleado es de producción
                            Dim oJobMove As New Move.roJobMove(Me.intIDEmployee, -1, oMoveState)

                            ' Obtener el último movimiento de producción
                            Dim oLastJobMoveType As Move.roJobMove.MovementStatus
                            Dim xLastJobMoveDate As Date
                            Dim lngLastMoveID As Long
                            oJobMove.GetLastMove(oLastJobMoveType, xLastJobMoveDate, lngLastMoveID)

                            If oLastJobMoveType <> Move.roJobMove.MovementStatus.Indet_ Then
                                Dim oLastJobMove As New Move.roJobMove(Me.intIDEmployee, lngLastMoveID, oMoveState)
                                Dim bolBegin As Boolean = False
                                If oLastJobMoveType = Move.roJobMove.MovementStatus.Begin_ Then
                                    bolBegin = (oLastJobMove.DateTimeIN.Value < Me.xDateTime.Value)
                                Else
                                    bolBegin = (oLastJobMove.DateTimeOUT.Value < Me.xDateTime.Value)
                                End If
                                If bolBegin Then
                                    oJobMove.LoadNewBEGIN(Me.xDateTime.Value, Me.intIDTerminal, oLastJobMove.Job, , oLastJobMove.IDMachine, oLastJobMove.IDIncidence)
                                    oJobMove.Save()
                                End If
                            End If
                        End If
                    End If
                End If

                If bolAutomaticFinishJobCheck AndAlso Me.intType = PunchTypeEnum._OUT Then
                    ' Es una salida
                    Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
                    If oSettings.GetVTSetting(eKeys.AutomaticFinishJob) Then ' Está configurada la opción finales automáticos de producción
                        oEmployee = Employee.roEmployee.GetEmployee(Me.intIDEmployee, oEmployeeState, )
                        If oEmployee.ID > 0 AndAlso oEmployee.Type = "J" Then
                            ' El empleado es de producción
                            Dim oJobMove As New Move.roJobMove(Me.intIDEmployee, -1, oMoveState)

                            ' Obtener el último movimiento de producción
                            Dim oLastJobMoveType As Move.roJobMove.MovementStatus
                            Dim xLastJobMoveDate As Date
                            Dim lngLastMoveID As Long
                            oJobMove.GetLastMove(oLastJobMoveType, xLastJobMoveDate, lngLastMoveID)

                            ' Si el último movimiento es un inicio
                            If oLastJobMoveType = Move.roJobMove.MovementStatus.Begin_ Then
                                Dim oLastJobMove As New Move.roJobMove(Me.intIDEmployee, lngLastMoveID, oMoveState)
                                If (oLastJobMove.DateTimeIN.Value < Me.xDateTime.Value) Then
                                    oJobMove = New Move.roJobMove(Me.intIDEmployee, lngLastMoveID, oMoveState)
                                    oJobMove.LoadNewEND(Me.xDateTime.Value, Me.intIDTerminal, lngLastMoveID)
                                    oJobMove.Save()
                                End If
                            End If
                        End If
                    End If
                End If

                bolRet = True
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roPunch::AutomaticJobPunch")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch::AutomaticJobPunch")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Actualizamos el status para recalcular los centros de coste
        ''' </summary>
        Private Function UpdateDailyCenterSchedule() As Boolean
            Dim bolRet As Boolean = False

            Try

                ' En el caso que este activado el modo de buscar el ultimo fichaje de costes
                ' debemos recalcular todos los dias posteriores hasta encontrar un nuevo fichaje

                Dim oParameter As New AdvancedParameter.roAdvancedParameter("LastPunchCenterEnabled", New AdvancedParameter.roAdvancedParameterState(oState.IDPassport))
                If oParameter.Value <> "1" Then
                    bolRet = True
                    Return bolRet
                End If

                ' Actualizamos el status para recalcular
                Dim xDailyDate As DateTime
                If Me.xShiftDate.HasValue Then
                    xDailyDate = Me.xShiftDate.Value
                ElseIf Me.xDateTime.HasValue Then
                    xDailyDate = Me.xDateTime
                End If

                Dim xLastDailyDate As DateTime

                ' Obtenemos el siguiente fichaje de centro de coste
                ' y marcamos para recalcular hasta esa fecha
                Dim strAux As String = Any2String(ExecuteScalar("@SELECT# TOP 1 DateTime FROM Punches WHERE IDEmployee=" & Me.intIDEmployee.ToString & " AND ActualType =" & PunchTypeEnum._CENTER & " AND DateTime > " & Any2Time(Me.xDateTime.Value).SQLDateTime & " Order by DateTime ASC "))
                If IsDate(strAux) Then
                    xLastDailyDate = Any2Time(strAux).DateOnly
                Else
                    xLastDailyDate = Any2Time(Now.Date).DateOnly
                End If

                While xDailyDate <= xLastDailyDate
                    Dim tbSchedule As New DataTable("DailySchedule")
                    Dim strSQL As String = "@SELECT# * FROM DailySchedule WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                                                               "[Date] = " & Any2Time(xDailyDate.Date).SQLSmallDateTime
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tbSchedule)
                    Dim oRow As DataRow = Nothing

                    If tbSchedule.Rows.Count = 0 Then
                        oRow = tbSchedule.NewRow
                        oRow("IDEmployee") = Me.intIDEmployee
                        oRow("Date") = xDailyDate.Date
                    Else
                        oRow = tbSchedule.Rows(0)
                    End If
                    oRow("Status") = 0
                    oRow("GUID") = ""

                    If tbSchedule.Rows.Count = 0 Then tbSchedule.Rows.Add(oRow)
                    da.Update(tbSchedule)

                    xDailyDate = xDailyDate.AddDays(1)

                End While

                bolRet = True
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roPunch::UpdateDailyCenterSchedule")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch::UpdateDailyCenterSchedule")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Actualizamos el status de tareas para los procesos
        ''' </summary>
        Private Function UpdateDailyTaskSchedule() As Boolean
            Dim bolRet As Boolean = False

            Try

                ' Actualizamos el status para recalcular
                Dim xDailyDate As DateTime
                If Me.xShiftDate.HasValue Then
                    xDailyDate = Me.xShiftDate.Value
                ElseIf Me.xDateTime.HasValue Then
                    xDailyDate = Me.xDateTime
                End If

                Dim xLastDailyDate As DateTime

                ' Obtenemos el siguiente fichaje de tareas
                ' y marcamos para recalcular hasta esa fecha
                Dim strAux As String = Any2String(ExecuteScalar("@SELECT# TOP 1 DateTime FROM Punches WHERE IDEmployee=" & Me.intIDEmployee.ToString & " AND ActualType =" & PunchTypeEnum._TASK & " AND DateTime > " & Any2Time(Me.xDateTime.Value).SQLDateTime & " Order by DateTime ASC "))
                If IsDate(strAux) Then
                    xLastDailyDate = Any2Time(strAux).DateOnly
                Else
                    xLastDailyDate = Any2Time(Now.Date).DateOnly
                End If

                While xDailyDate <= xLastDailyDate
                    Dim tbSchedule As New DataTable("DailySchedule")
                    Dim strSQL As String = "@SELECT# * FROM DailySchedule WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                                                               "[Date] = " & Any2Time(xDailyDate.Date).SQLSmallDateTime
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tbSchedule)
                    Dim oRow As DataRow = Nothing

                    If tbSchedule.Rows.Count = 0 Then
                        oRow = tbSchedule.NewRow
                        oRow("IDEmployee") = Me.intIDEmployee
                        oRow("Date") = xDailyDate.Date
                    Else
                        oRow = tbSchedule.Rows(0)
                    End If
                    oRow("TaskStatus") = 0

                    If tbSchedule.Rows.Count = 0 Then tbSchedule.Rows.Add(oRow)
                    da.Update(tbSchedule)

                    xDailyDate = xDailyDate.AddDays(1)

                End While

                bolRet = True
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roPunch::UpdateDailyTaskSchedule")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch::UpdateDailyTaskSchedule")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Actualizamos el status para los procesos
        ''' </summary>
        Private Function UpdateDailySchedule() As Boolean
            Dim bolRet As Boolean = False

            Try

                ' Actualizamos el status para recalcular
                Dim xDailyDate As DateTime
                If Me.xShiftDate.HasValue Then
                    xDailyDate = Me.xShiftDate.Value
                ElseIf Me.xDateTime.HasValue Then
                    xDailyDate = Me.xDateTime
                End If

                ' Validamos que la fecha a recalcular no este dentro del periodo de cierre del empleado
                Dim xLockDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.intIDEmployee, False, Me.oState)
                If xLockDate >= xDailyDate.Date Then
                    ' Eliminamos alertas posteriores a la fecha del fichaje, en caso necesario
                    Dim strSQLAlert As String = "@DELETE# FROM sysroNotificationTasks WHERE IDNotification=904   AND Key1Numeric = " & IDEmployee.ToString & " AND  Key3DateTime > " & Any2Time(xDailyDate.Date).SQLDateTime
                    bolRet = ExecuteSql(strSQLAlert)

                    ' Creamos alerta de fichajes en periodo de cierre del empleado, siempre y cuando no exista otra ya creada  en una fecha igual o anterior
                    ' y no actualizamos el status del empleado para recalcular
                    strSQLAlert = " IF NOT EXISTS (@SELECT# 1 FROM sysroNotificationTasks WHERE IDNotification=904   AND Key1Numeric = " & IDEmployee.ToString & " AND  Key3DateTime <= " & Any2Time(xDailyDate.Date).SQLDateTime & ") "
                    strSQLAlert += "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key3DateTime) VALUES " &
                                                    "(904, " & IDEmployee.ToString & "," & Any2Time(xDailyDate.Date).SQLSmallDateTime & ")"

                    bolRet = ExecuteSql(strSQLAlert)
                    Return False
                End If

                Dim tbSchedule As New DataTable("DailySchedule")
                Dim strSQL As String = "@SELECT# * FROM DailySchedule WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                                                           "[Date] = " & Any2Time(xDailyDate.Date).SQLSmallDateTime
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tbSchedule)
                Dim oRow As DataRow = Nothing

                If tbSchedule.Rows.Count = 0 Then
                    oRow = tbSchedule.NewRow
                    oRow("IDEmployee") = Me.intIDEmployee
                    oRow("Date") = xDailyDate.Date
                Else
                    oRow = tbSchedule.Rows(0)
                End If
                oRow("Status") = 0
                oRow("JobStatus") = 0
                oRow("TaskStatus") = 0
                oRow("GUID") = ""

                If tbSchedule.Rows.Count = 0 Then tbSchedule.Rows.Add(oRow)
                da.Update(tbSchedule)

                bolRet = True
            Catch Ex As DbException
                ' Evito log de error si el empleado es el 9999 dado que es el admin de terminales
                If Me.intIDEmployee <> 9999 Then
                    Me.oState.UpdateStateInfo(Ex, "roPunch::UpdateDailySchedule")
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPunch::UpdateDailySchedule")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Calcula el sentido del fichaje de accesos integrado con presencia
        ''' </summary>
        Private Function CalculateTypeAcc() As Nullable(Of PunchTypeEnum)
            '
            ' Calculamos el tipo de operación actual de presencia en base la zona a la que ha accedido
            '
            Dim bolRet As Boolean = False
            Dim intNewType As Nullable(Of PunchTypeEnum)

            Try

                Dim intActualStatus As Nullable(Of PunchTypeEnum)
                Dim bEmployeeAlreadyIn As Boolean
                Dim bIsAccToWorkingZone As Boolean = False
                Dim oZoneState As New Zone.roZoneState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)
                bIsAccToWorkingZone = Zone.roZone.GetIsWorkingZone(Me.intIDZone, oZoneState)
                intActualStatus = CalculateTypeAtt(Not bIsAccToWorkingZone)
                Select Case intActualStatus
                    Case PunchTypeEnum._IN
                        ' Este fichaje debe ser una entrada, luego estoy fuera ...
                        bEmployeeAlreadyIn = False
                    Case PunchTypeEnum._OUT
                        ' Este fichaje debe ser una salida, luego estoy dentro ...
                        bEmployeeAlreadyIn = True
                End Select

                If bIsAccToWorkingZone Then
                    ' Si la zona es de trabajo es una entrada, a no ser que ya esté dentro
                    If Not bEmployeeAlreadyIn Then
                        intNewType = PunchTypeEnum._IN
                    Else
                        intNewType = PunchTypeEnum._AV
                    End If
                Else
                    ' Si la zona es de no trabajo es una salida, a no ser que ya esté fuera
                    If bEmployeeAlreadyIn Then
                        intNewType = PunchTypeEnum._OUT
                    Else
                        intNewType = PunchTypeEnum._AV
                    End If
                End If

                bolRet = True
            Catch Ex As DbException
                ' Si se produce una excepción marcamos un sentido, aunque sea incorrecto, no podemos dejar un fichaje sin sentido
                intNewType = PunchTypeEnum._IN
                Me.oState.UpdateStateInfo(Ex, "roPunch::CalculateTypeAcc")
            Catch ex As Exception
                ' Si se produce una excepción marcamos un sentido, aunque sea incorrecto, no podemos dejar un fichaje sin sentido
                intNewType = PunchTypeEnum._IN
                Me.oState.UpdateStateInfo(ex, "roPunch::CalculateTypeAcc")
            End Try

            Return intNewType
        End Function

        ''' <summary>
        ''' Calcula el sentido del fichaje automatico
        ''' </summary>
        Public Function CalculateTypeAtt(Optional bAccOutAndAtt As Boolean = False) As Nullable(Of PunchTypeEnum)
            '
            ' Calculamos el tipo de operación actual de presencia en base a los fichajes que
            ' tiene el empleado actualmente en la base de datos
            Dim bolRet As Boolean = False
            Dim intNewType As Nullable(Of PunchTypeEnum)

            Try
                If Me.intIDEmployee <= 0 OrElse Not Me.xDateTime.HasValue Then
                    Return Nothing
                End If

                ' Obtenemos el anterior fichaje para determinar
                Dim tb As New DataTable("LastPunch")
                Dim strSQL As String = "@SELECT# TOP 1 * FROM Punches WHERE IDEmployee = " & Me.intIDEmployee.ToString &
                " AND DateTime <= " & Any2Time(Me.xDateTime).SQLDateTime &
                " AND ID <> " & Me.lngID &
                " AND ActualType IN(" & PunchTypeEnum._IN & "," & PunchTypeEnum._OUT & ")" &
                " Order by Datetime DESC, ID DESC"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oRow = tb.Rows(0)
                    If oRow("ActualType") = PunchTypeEnum._OUT Then
                        ' Si el anterior es una Salida, el nuevo es una Entrada
                        intNewType = PunchTypeEnum._IN
                    Else
                        ' Si el anterior es una Entrada y el actual NO es de accesos
                        If Not bAccOutAndAtt Then
                            Dim oParameters As New roParameters("OPTIONS", True)
                            Dim intMaxMovement As Integer = 0
                            Dim oTime As roTime = roTypes.Any2Time(oParameters.Parameter(Parameters.MovMaxHours))
                            If oTime.IsValid Then intMaxMovement = oTime.Minutes

                            If intMaxMovement > 0 AndAlso
                           DateDiff(DateInterval.Minute, oRow("DateTime"), Me.xDateTime.Value) > intMaxMovement Then
                                ' Si el tiempo entre el marcaje actual y el anterior és superior al máximo (MaxMovementHours)
                                ' vuelve a ser una Entrada
                                intNewType = PunchTypeEnum._IN
                            Else
                                ' Sino es una Salida
                                intNewType = PunchTypeEnum._OUT
                            End If
                        Else
                            ' Si el que estoy calculando es de accesos integrados con presencia, A UNA ZONA DE SALIDA, y el anterior era  una entrada, hayan pasado las horas que hayan pasado es una salida.
                            intNewType = PunchTypeEnum._OUT
                        End If
                    End If
                Else
                    ' Si no hay ningún fichaje es una entrada
                    intNewType = PunchTypeEnum._IN
                End If

                bolRet = True
            Catch Ex As DbException
                ' Si se produce una excepción marcamos un sentido, aunque sea incorrecto, no podemos dejar un fichaje sin sentido
                intNewType = PunchTypeEnum._IN
                Me.oState.UpdateStateInfo(Ex, "roPunch::CalculateTypeAtt")
            Catch ex As Exception
                ' Si se produce una excepción marcamos un sentido, aunque sea incorrecto, no podemos dejar un fichaje sin sentido
                intNewType = PunchTypeEnum._IN
                Me.oState.UpdateStateInfo(ex, "roPunch::CalculateTypeAtt")
            End Try

            Return intNewType

        End Function

        ''' <summary>
        ''' Calcula si un fichaje de presencia es repetido o no en función del parámetro avanzado de tiempo máximo entre fichajes con el mismo sentido en el mismo terminal. Sólo para fichajes de tipo E o S (no X)
        ''' </summary>
        Private Function ValidateAttRepeatededPunch(iMinMinutes As Integer) As Nullable(Of PunchTypeEnum)
            Dim bolRet As Boolean = False
            Dim intNewType As Nullable(Of PunchTypeEnum)

            Try

                If Me.intIDEmployee <= 0 OrElse Not Me.xDateTime.HasValue Then
                    Return Nothing
                End If

                ' Obtenemos el anterior fichaje para determinar
                Dim tb As New DataTable("LastPunch")
                Dim strSQL As String = "@SELECT# TOP 1 * FROM Punches WHERE IDEmployee = " & Me.intIDEmployee.ToString &
                " AND DateTime <= " & Any2Time(Me.xDateTime).SQLDateTime &
                " AND ID <> " & Me.lngID &
                " AND Type IN(" & PunchTypeEnum._IN & "," & PunchTypeEnum._OUT & ")" &
                " AND IDTerminal = " & Me.intIDTerminal.ToString & 'TODO: Esta validación se podría obviar.
                " Order by Datetime DESC, ID DESC"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oRow = tb.Rows(0)
                    If (oRow("Type") = PunchTypeEnum._OUT AndAlso Me.intType = PunchTypeEnum._OUT) OrElse (oRow("Type") = PunchTypeEnum._IN AndAlso Me.intType = PunchTypeEnum._IN) Then
                        ' Valido mínimo entre
                        If DateDiff(DateInterval.Minute, oRow("DateTime"), Me.xDateTime.Value) <= iMinMinutes Then
                            intNewType = 0
                        Else
                            intNewType = Me.intType
                        End If
                    Else
                        intNewType = Me.intType
                    End If
                Else
                    ' Si no hay ningún fichaje es una entrada
                    intNewType = Me.intType
                End If

                bolRet = True
            Catch Ex As DbException
                ' Si se produce una excepción marcamos un sentido, aunque sea incorrecto, no podemos dejar un fichaje sin sentido
                intNewType = PunchTypeEnum._IN
                Me.oState.UpdateStateInfo(Ex, "roPunch::CalculateTypeAtt")
            Catch ex As Exception
                ' Si se produce una excepción marcamos un sentido, aunque sea incorrecto, no podemos dejar un fichaje sin sentido
                intNewType = PunchTypeEnum._IN
                Me.oState.UpdateStateInfo(ex, "roPunch::CalculateTypeAtt")
            End Try

            Return intNewType

        End Function

        ''' <summary>
        ''' Reprocesa los fichajes automaticos a partir de una hora
        ''' </summary>
        Private Function ReprocessPunches(ByVal _IDEmployee As Integer, ByVal _xDateTime As Date) As Boolean
            '
            ' Reprocesamos los fichajes necesarios de tipo 3 posteriores al fichaje actual
            '
            Dim bolRet As Boolean = False

            Try
                ' Obtenemos todos los fichajes posteriores de tipo 3 hasta encontrar
                ' un fichaje de tipo 1, 2, 7 o [3 con el sentido fijado por un usuario]
                '
                Dim strSQL As String
                strSQL = "@SELECT# * FROM Punches with (nolock) WHERE"
                strSQL &= " IDEmployee = " & Me.intIDEmployee & " AND"
                strSQL &= " ID <>" & Me.lngID & " AND"
                strSQL &= " DateTime > " & Any2Time(_xDateTime).SQLDateTime & " AND"
                strSQL &= " DateTime <= " & Any2Time(_xDateTime).Add(2, "d").SQLDateTime & " AND"
                strSQL &= " (ActualType IN(" & PunchTypeEnum._IN & "," & PunchTypeEnum._OUT & ")"
                strSQL &= " OR (Type =" & PunchTypeEnum._L & " AND ActualType =" & PunchTypeEnum._AV & "))"
                strSQL &= " Order by DateTime ASC, ID ASC"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        If oRow("Type") = PunchTypeEnum._IN Or oRow("Type") = PunchTypeEnum._OUT Or (oRow("Type") = PunchTypeEnum._AUTO And Any2Integer(oRow("Action")) = 3) Then
                            Exit For
                        End If

                        ' Cargamos los datos del fichaje
                        Dim oPunch As New roPunch(_IDEmployee, oRow("ID"), Me.State)

                        ' Obtenemos tipo actual
                        Dim oldActualType As Nullable(Of PunchTypeEnum) = oPunch.intActualType

                        ' Recalculamos el tipo de fichaje actual
                        If oRow("Type") = PunchTypeEnum._L Then
                            oPunch.intActualType = oPunch.CalculateTypeAcc()
                        Else
                            oPunch.intActualType = oPunch.CalculateTypeAtt()
                        End If

                        If oPunch.intActualType <> oldActualType Then
                            ' Si el tipo a cambiado guardamos , seguimos procesando
                            oPunch.Save(False, False, False)
                        Else
                            ' Sigo recalculando si ha cambiado el tipo, o se trata de un ACCTA, en cuyo caso puede que no haya cambiado el tipo, pero cambie el de alguno posterior ...
                            If oRow("Type") <> PunchTypeEnum._L Then Exit For
                        End If
                    Next

                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPunch::ReprocessPunches")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPunch::ReprocessPunches")
            End Try

            Return bolRet
        End Function

#Region "Helper methods"

        ''' <summary>
        ''' Devuelve los registros de la tabla punches según el filtrado por parametros
        ''' </summary>
        Public Shared Function GetPortalPunchesDataTable(ByVal _State As roPunchState,
                                                     Optional ByVal dShiftDateBegin As Date = Nothing,
                                                     Optional ByVal dShiftDateEnd As Date = Nothing,
                                                     Optional ByVal dPeriodBegin As DateTime = Nothing,
                                                     Optional ByVal dPeriodEnd As DateTime = Nothing,
                                                     Optional ByVal iIDEmployee As String = "",
                                                     Optional ByVal strTypes As String = "") As DataTable
            Dim dRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Punches.ID, Punches.InTelecommute as InTelecommute, Punches.Type, Punches.ActualType, Punches.TypeData, Punches.DateTime as Datetime, Punches.LocationZone, Punches.FullAddress, Punches.Field1, Punches.Field2, Punches.Field3, Punches.Field4, Punches.Field5, Punches.Field6,  Employees.Name as EmployeeName, Zones.Name as ZoneName, Terminals.Description AS TerminalName, " &
                      " CASE " &
                      "  WHEN (Punches.Type = 4 And Punches.TypeData Is Not null) THEN (@SELECT# Name from Tasks where ID = Punches.TypeData ) " &
                      "  WHEN (Punches.Type = 13 and Punches.TypeData is not null) THEN (@SELECT# Name from BusinessCenters where ID = Punches.TypeData ) " &
                      "  WHEN (Punches.Type <> 4 And Punches.Type <> 13 And Punches.TypeData Is Not null And Punches.TypeData > 0) THEN (@SELECT# Name from Causes where ID = Punches.TypeData ) " &
                      " END As RelatedInfo, " &
                      " (@SELECT# top 1 Causes.Name from Requests, Causes where RequestType = 3 and Punches.DateTime = Requests.Date1 and Punches.IdEmployee = Requests.IdEmployee and Requests.IDCause = Causes.ID and Requests.Status in (0,1) order by Requests.RequestDate desc) as RequestedTypeData " &
                      " FROM Punches " &
                      "  LEFT JOIN Employees On Punches.IDEmployee = Employees.ID" &
                      "  LEFT JOIN Zones On Punches.IDZone = Zones.ID " &
                      "  LEFT JOIN Terminals On Punches.IDTerminal = Terminals.ID "

                Dim strWhere As String = " Where 1= 1 "

                If dShiftDateBegin <> Nothing Then
                    strWhere &= " And Punches.ShiftDate >= " & Any2Time(dShiftDateBegin).SQLSmallDateTime
                End If

                If dShiftDateEnd <> Nothing Then
                    strWhere &= " And Punches.ShiftDate <= " & Any2Time(dShiftDateEnd).SQLSmallDateTime
                End If

                If dPeriodBegin <> Nothing Then
                    strWhere &= " And Punches.DateTime >= " & Any2Time(dPeriodBegin).SQLDateTime
                End If

                If dPeriodEnd <> Nothing Then
                    strWhere &= " And Punches.DateTime <= " & Any2Time(dPeriodEnd).SQLDateTime
                End If

                'Filtres
                If iIDEmployee <> "" Then
                    strWhere &= " And Punches.IDEmployee In (" & iIDEmployee & ")"
                End If

                If strTypes <> "" Then
                    strWhere &= " And Punches.Type In (" & strTypes & ")"
                End If

                'TaskRequests Pendientes

                Dim strRequests As String = " UNION @select# r.id, null, 4, 4, IDTask1, Date1 as Datetime, null, null, null, null, null, null, null, null, e.name, null, null, (t.name + '; ' + CAST(t.Description as NVARCHAR(max))) as relatedinfo, null  " &
                            " from requests r inner join employees e on r.IDEmployee=e.id inner join Tasks t on t.ID = r.IDTask1  " &
                            " where RequestType=10 and IDEmployee= " & iIDEmployee & " and r.status=0 "

                If dShiftDateBegin <> Nothing Then
                    strRequests &= " And Date1 >= " & Any2Time(dShiftDateBegin).SQLSmallDateTime
                End If

                If dShiftDateEnd <> Nothing Then
                    strRequests &= " And Date1 < " & Any2Time(dShiftDateEnd.AddDays(1)).SQLSmallDateTime
                End If

                strWhere &= strRequests

                strWhere &= " Order by DateTime ASC"

                strSQL &= strWhere

                dRet = CreateDataTable(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch:GetPunchesDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch:GetPunchesDataTable")
            End Try

            Return dRet

        End Function

        ''' <summary>
        ''' Devuelve los registros de la tabla punches según el filtrado por parametros
        ''' </summary>
        Public Shared Function GetPunchesDataTable(ByRef _State As roPunchState,
                                                     Optional ByVal dShiftDateBegin As Date = Nothing,
                                                     Optional ByVal dShiftDateEnd As Date = Nothing,
                                                     Optional ByVal dPeriodBegin As DateTime = Nothing,
                                                     Optional ByVal dPeriodEnd As DateTime = Nothing,
                                                     Optional ByVal iIDEmployee As String = "",
                                                     Optional ByVal iIDTerminal As String = "",
                                                     Optional ByVal iIDReader As String = "",
                                                     Optional ByVal iIDCause As String = "",
                                                     Optional ByVal iIDZone As String = "",
                                                     Optional ByVal iIsNotReliable As Integer = -1,
                                                     Optional ByVal strTypes As String = "",
                                                     Optional ByVal strActualTypes As String = "",
                                                     Optional ByVal strInvalidTypes As String = "",
                                                     Optional ByVal strActions As String = "",
                                                     Optional ByVal strOrderBy As String = "") As DataTable
            Dim dRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# CASE Upper(Terminals.Type) WHEN 'NFC' THEN TerminalReaders.Description ELSE '' END As NFC, Punches.*, Employees.Name as EmployeeName, Zones.Name as ZoneName FROM Punches " &
                            " Left Join  Employees ON Punches.IDEmployee = Employees.ID " &
                            " Left Join  Zones ON Punches.IDZone = Zones.ID " &
                            " Left Join terminals on terminals.id = punches.IDTerminal " &
                            " Left Join terminalreaders on terminalreaders.id = punches.idreader And terminals.id = terminalreaders.idterminal "

                Dim strWhere As String = " Where 1= 1 "

                If dShiftDateBegin <> Nothing Then
                    strWhere &= " And Punches.ShiftDate >= " & Any2Time(dShiftDateBegin).SQLSmallDateTime
                End If

                If dShiftDateEnd <> Nothing Then
                    strWhere &= " And Punches.ShiftDate <= " & Any2Time(dShiftDateEnd).SQLSmallDateTime
                End If

                If dPeriodBegin <> Nothing Then
                    strWhere &= " And Punches.DateTime >= " & Any2Time(dPeriodBegin).SQLDateTime
                End If

                If dPeriodEnd <> Nothing Then
                    strWhere &= " And Punches.DateTime <= " & Any2Time(dPeriodEnd).SQLDateTime
                End If

                'Filtres
                If iIDEmployee <> "" Then
                    strWhere &= " And Punches.IDEmployee IN (" & iIDEmployee & ")"
                End If

                If iIDTerminal <> "" Then
                    strWhere &= " And Punches.IDTerminal IN ( " & iIDTerminal & ")"
                End If

                If iIDReader <> "" Then
                    strWhere &= " And Punches.IDReader IN (" & iIDReader & ")"
                End If

                If iIDCause <> "" Then
                    strWhere &= " And Punches.TypeData IN( " & iIDCause & ")"
                End If

                If iIDZone <> "" Then
                    strWhere &= " And Punches.IDZone IN( " & iIDZone & ")"
                End If

                If iIsNotReliable <> -1 Then
                    strWhere &= " And Punches.IsNotReliable = " & iIsNotReliable
                End If

                If strTypes <> "" Then
                    strWhere &= " And Punches.Type IN (" & strTypes & ")"
                End If

                If strActualTypes <> "" Then
                    strWhere &= " And Punches.ActualType IN (" & strActualTypes & ")"
                End If

                If strInvalidTypes <> "" Then
                    strWhere &= " And Punches.InvalidType IN (" & strInvalidTypes & ")"
                End If

                If strActions <> "" Then
                    strWhere &= " And Punches.Action IN (" & strActions & ")"
                End If

                If strOrderBy <> "" Then
                    strWhere &= " Order by " & strOrderBy
                End If

                strSQL &= strWhere

                dRet = CreateDataTable(strSQL)

                Dim bolPunchPermission As Boolean = True
                For Each oRow As DataRow In dRet.Rows
                    bolPunchPermission = True
                    If Any2Integer(oRow("IDEmployee")) > 0 Then
                        Select Case Any2Integer(oRow("ActualType"))
                            Case PunchTypeEnum._IN, PunchTypeEnum._OUT
                                bolPunchPermission = WLHelper.HasFeaturePermissionByEmployeeOnDate(_State.IDPassport, "Calendar.Punches.Punches", Permission.Read, oRow("IDEmployee"), oRow("ShiftDate"))
                            Case PunchTypeEnum._AV
                                bolPunchPermission = WLHelper.HasFeaturePermissionByEmployeeOnDate(_State.IDPassport, "Access.Zones", Permission.Read, oRow("IDEmployee"), oRow("ShiftDate"))
                            Case PunchTypeEnum._TASK
                                bolPunchPermission = WLHelper.HasFeaturePermissionByEmployeeOnDate(_State.IDPassport, "Tasks.Punches", Permission.Read, oRow("IDEmployee"), oRow("ShiftDate"))
                        End Select
                        If Not bolPunchPermission Then oRow.Delete()
                    End If
                Next

                dRet.AcceptChanges()
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch:GetPunchesDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch:GetPunchesDataTable")
            End Try

            Return dRet

        End Function

        Public Shared Function GetPunchesDataTableCount(ByRef _State As roPunchState,
                                                     Optional ByVal dShiftDateBegin As Date = Nothing,
                                                     Optional ByVal dShiftDateEnd As Date = Nothing,
                                                     Optional ByVal dPeriodBegin As DateTime = Nothing,
                                                     Optional ByVal dPeriodEnd As DateTime = Nothing,
                                                     Optional ByVal iIDEmployee As String = "",
                                                     Optional ByVal iIDTerminal As String = "",
                                                     Optional ByVal iIDReader As String = "",
                                                     Optional ByVal iIDCause As String = "",
                                                     Optional ByVal iIDZone As String = "",
                                                     Optional ByVal iIsNotReliable As Integer = -1,
                                                     Optional ByVal strTypes As String = "",
                                                     Optional ByVal strActualTypes As String = "",
                                                     Optional ByVal strInvalidTypes As String = "",
                                                     Optional ByVal strActions As String = "") As Integer
            Dim dRet As Integer = 0

            Try

                Dim strSQL As String = "@SELECT# ISNULL(COUNT(*),0) FROM Punches LEFT JOIN " &
                      " Employees ON Punches.IDEmployee = Employees.ID LEFT JOIN " &
                      " Zones ON Punches.IDZone = Zones.ID "

                Dim strWhere As String = " Where 1= 1 "

                If dShiftDateBegin <> Nothing Then
                    strWhere &= " And Punches.ShiftDate >= " & Any2Time(dShiftDateBegin).SQLSmallDateTime
                End If

                If dShiftDateEnd <> Nothing Then
                    strWhere &= " And Punches.ShiftDate <= " & Any2Time(dShiftDateEnd).SQLSmallDateTime
                End If

                If dPeriodBegin <> Nothing Then
                    strWhere &= " And Punches.DateTime >= " & Any2Time(dPeriodBegin).SQLDateTime
                End If

                If dPeriodEnd <> Nothing Then
                    strWhere &= " And Punches.DateTime <= " & Any2Time(dPeriodEnd).SQLDateTime
                End If

                'Filtres
                If iIDEmployee <> "" Then
                    strWhere &= " And Punches.IDEmployee IN (" & iIDEmployee & ")"
                End If

                If iIDTerminal <> "" Then
                    strWhere &= " And Punches.IDTerminal IN ( " & iIDTerminal & ")"
                End If

                If iIDReader <> "" Then
                    strWhere &= " And Punches.IDReader IN (" & iIDReader & ")"
                End If

                If iIDCause <> "" Then
                    strWhere &= " And Punches.TypeData IN( " & iIDCause & ")"
                End If

                If iIDZone <> "" Then
                    strWhere &= " And Punches.IDZone IN( " & iIDZone & ")"
                End If

                If iIsNotReliable <> -1 Then
                    strWhere &= " And Punches.IsNotReliable = " & iIsNotReliable
                End If

                If strTypes <> "" Then
                    strWhere &= " And Punches.Type IN (" & strTypes & ")"
                End If

                If strActualTypes <> "" Then
                    strWhere &= " And Punches.ActualType IN (" & strActualTypes & ")"
                End If

                If strInvalidTypes <> "" Then
                    strWhere &= " And Punches.InvalidType IN (" & strInvalidTypes & ")"
                End If

                If strActions <> "" Then
                    strWhere &= " And Punches.Action IN (" & strActions & ")"
                End If

                strSQL &= strWhere

                dRet = roTypes.Any2Integer(ExecuteScalar(strSQL))
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch:GetPunchesDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch:GetPunchesDataTable")
            End Try

            Return dRet

        End Function

        ''' <summary>
        ''' Devuelve el último registro de la tabla punches según el filtrado por parametros
        ''' </summary>
        Public Shared Function GetLastPunchDataTable(ByRef _State As roPunchState,
                                             Optional ByVal dShiftDateBegin As Date = Nothing,
                                             Optional ByVal dShiftDateEnd As Date = Nothing,
                                             Optional ByVal dPeriodBegin As DateTime = Nothing,
                                             Optional ByVal dPeriodEnd As DateTime = Nothing,
                                             Optional ByVal iIDEmployee As String = "",
                                             Optional ByVal iIDTerminal As String = "",
                                             Optional ByVal iIDReader As String = "",
                                             Optional ByVal iIDCause As String = "",
                                             Optional ByVal iIDZone As String = "",
                                             Optional ByVal iIsNotReliable As Integer = -1,
                                             Optional ByVal strTypes As String = "",
                                             Optional ByVal strActualTypes As String = "",
                                             Optional ByVal strInvalidTypes As String = "",
                                             Optional ByVal strActions As String = "") As DataTable
            Dim dRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# top 1 * from Punches "
                Dim strWhere As String = " Where 1= 1 "

                If dShiftDateBegin <> Nothing Then
                    strWhere &= " And ShiftDate >= " & Any2Time(dShiftDateBegin).SQLSmallDateTime
                End If

                If dShiftDateEnd <> Nothing Then
                    strWhere &= " And ShiftDate <= " & Any2Time(dShiftDateEnd).SQLSmallDateTime
                End If

                If dPeriodBegin <> Nothing Then
                    strWhere &= " And DateTime >= " & Any2Time(dPeriodBegin).SQLDateTime
                End If

                If dPeriodEnd <> Nothing Then
                    strWhere &= " And DateTime <= " & Any2Time(dPeriodEnd).SQLDateTime
                End If

                'Filtres
                If iIDEmployee <> "" Then
                    strWhere &= " And IDEmployee IN ( " & iIDEmployee & ")"
                End If

                If iIDTerminal <> "" Then
                    strWhere &= " And IDTerminal IN (" & iIDTerminal & ")"
                End If

                If iIDReader <> "" Then
                    strWhere &= " And IDReader IN( " & iIDReader & ")"
                End If

                If iIDCause <> "" Then
                    strWhere &= " And TypeData IN( " & iIDCause & ")"
                End If

                If iIDZone <> "" Then
                    strWhere &= " And IDZone IN( " & iIDZone & ")"
                End If

                If iIsNotReliable <> -1 Then
                    strWhere &= " And IsNotReliable = " & iIsNotReliable
                End If

                If strTypes <> "" Then
                    strWhere &= " And Type IN (" & strTypes & ")"
                End If

                If strActualTypes <> "" Then
                    strWhere &= " And ActualType IN (" & strActualTypes & ")"
                End If

                If strInvalidTypes <> "" Then
                    strWhere &= " And InvalidType IN (" & strInvalidTypes & ")"
                End If

                If strActions <> "" Then
                    strWhere &= " And Action IN (" & strActions & ")"
                End If

                strWhere &= " Order by DateTime desc, ID desc"

                strSQL &= strWhere

                dRet = CreateDataTable(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch:GetLastPunchDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch:GetLastPunchDataTable")
            End Try

            Return dRet

        End Function

        Public Shared Function GetEmployeesLastPunchByDateDataTable(ByRef _State As roPunchState,
                                             ByVal dDateBegin As DateTime,
                                             ByVal dDateEnd As DateTime,
                                             Optional ByVal iIDEmployee As String = "") As DataTable
            Dim dRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * from sysrovwEmployeesLastPunchByDate "

                Dim strWhere As String = String.Empty
                strWhere &= " WHERE ShiftDate >= " & Any2Time(dDateBegin).SQLSmallDateTime
                strWhere &= " And ShiftDate <= " & Any2Time(dDateEnd).SQLSmallDateTime

                If iIDEmployee <> "" Then
                    strWhere &= " And IDEmployee IN ( " & iIDEmployee & ")"
                End If

                strWhere &= " Order by DateTime desc"
                strSQL &= strWhere

                dRet = CreateDataTable(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch:GetEmployeesLastPunchByDateDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch:GetEmployeesLastPunchByDateDataTable")
            End Try

            Return dRet

        End Function

        Public Shared Function GetEmployeesLastMonthZoneMovesDataTable(ByRef _State As roPunchState, Optional ByVal sEmployeeIDList As String = "") As DataTable
            Dim dRet As DataTable = Nothing
            Dim strWhere As String = String.Empty

            Try
                Dim strSQL As String = "@SELECT# sysrovwLastMonthZoneMoves.*, Terminals.Type AS TerminalOutType FROM sysrovwLastMonthZoneMoves " &
                    " LEFT JOIN Terminals ON Terminals.ID = sysrovwLastMonthZoneMoves.IDterminalOut "
                If sEmployeeIDList.Length > 0 Then
                    strWhere &= " WHERE IDEmployee IN ( " & sEmployeeIDList & ")"
                End If

                strWhere &= " ORDER BY IDEmployee ASC, DateTimeIn ASC"
                strSQL &= strWhere

                dRet = CreateDataTable(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch:GetEmployeesLastMonthZoneMovesDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch:GetEmployeesLastMonthZoneMovesDataTable")
            End Try

            Return dRet

        End Function

        Public Shared Function GetEmployeesInfoLastPunchByDateDataTable(ByRef _State As roPunchState,
                                             ByVal dDateBegin As DateTime,
                                             ByVal dDateEnd As DateTime,
                                             Optional ByVal iIDEmployee As String = "") As DataTable
            Dim dRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# sysrovwEmployeesLastPunchByDate.*, Employees.Name from sysrovwEmployeesLastPunchByDate  LEFT JOIN Employees ON Employees.ID = sysrovwEmployeesLastPunchByDate.IDEmployee   "

                Dim strWhere As String = String.Empty
                strWhere &= " WHERE ShiftDate >= " & Any2Time(dDateBegin).SQLSmallDateTime
                strWhere &= " And ShiftDate <= " & Any2Time(dDateEnd).SQLSmallDateTime

                If iIDEmployee <> "" Then
                    strWhere &= " And IDEmployee IN ( " & iIDEmployee & ")"
                End If

                strWhere &= " Order by DateTime desc"
                strSQL &= strWhere

                dRet = CreateDataTable(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch:GetEmployeesInfoLastPunchByDateDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch:GetEmployeesInfoLastPunchByDateDataTable")
            End Try

            Return dRet

        End Function

        ''' <summary>
        ''' Devuelve los fichajes en función del WHERE Indicado.
        ''' </summary>
        ''' <param name="strFilter">Filtro a aplicar</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns>DataTable con los registros de la tabla 'Punches' coincidentes con el filtro.</returns>
        ''' <remarks></remarks>
        Public Shared Function GetPunches(ByVal strFilter As String, ByRef _State As roPunchState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try
                Dim strSQL As String

                strSQL = "@SELECT# * FROM Punches WITH (nolock) "
                If strFilter <> "" Then
                    strSQL &= "WHERE " & strFilter
                End If
                tbRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::GetPunches")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::GetPunches")
            End Try

            Return tbRet

        End Function

        ''' <summary>
        ''' Añade un fichaje de presencia para un empleado. El tipo de fichaje generado se calcula en función del estado actual del empleado.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_InputDateTime">Fecha y hora del fichaje a generar</param>
        ''' <param name="_Direction">Sentido del fichaje 'E' entrada - 'S' salida</param>
        ''' <param name="_IDTerminal">Código del terminal por el que se realiza el fichaje</param>
        ''' <param name="_IDReader">Número del lector por el que se realiza el fichaje</param>
        ''' <param name="_IDCause">Código de la justificación. Si no hay, se tiene que informar como '-1'.</param>
        ''' <param name="_InputCapture">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
        ''' <param name="_Punch">Devuelve el fichaje generado.</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DoPortalPunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _Direction As String, ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _IDCause As Integer,
                                             ByVal _InputCapture As System.Drawing.Image, ByVal _IsReliable As Boolean, ByRef _Punch As roPunch, ByVal _State As roPunchState, ByRef oPunchStatus As PunchSeqStatus,
                                             Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal _LocationZone As String = "",
                                             Optional ByVal _FullAddress As String = "", Optional ByVal _TimeZone As String = "", Optional ByVal _isForgottenPunch As Boolean = False, Optional ByVal _nfcTag As String = "",
                                             Optional _isTelecommuting As Boolean = False, Optional ByVal _IsReliableZone As Boolean = True, Optional ByVal selectedZone As Integer = -1, Optional strIP As String = "", Optional isApp As Boolean = False, Optional remarks As String = Nothing, Optional notReliableCause As String = Nothing) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Modo de fichaje: Presencia (TA) o Acceso (ACC)
                Dim _Mode As String = "TA"

                ' Obtener información del último marcaje de presencia del empleado
                Dim oLastPunchType As PunchStatus = PunchStatus.Indet_
                Dim xLastPunchDateTime As DateTime
                Dim lngLastPunchID As Long = -1
                _Punch = New roPunch(_IDEmployee, -1, _State)
                _Punch.GetLastPunchPres(oLastPunchType, xLastPunchDateTime, lngLastPunchID)
                _Punch.ID = lngLastPunchID
                _Punch.Load()

                Dim oParameters As New roParameters("OPTIONS", True)
                Dim intPunchPeriodRTIn As Integer = 0
                Dim intPunchPeriodRTOut As Integer = 0
                Dim idZone As Integer = selectedZone

                ' Fichaje mediante TAG NFC
                Try
                    If _nfcTag IsNot Nothing AndAlso _nfcTag.Trim.Length > 0 Then
                        Dim strSQL As String = String.Empty

                        strSQL = "@SELECT# TerminalReaders.IDTerminal, TerminalReaders.ID, TerminalReaders.IDZone, Zones.IsWorkingZone, TerminalReaders.Mode " &
                                 "FROM TerminalReaders " &
                                 "LEFT JOIN Zones ON TerminalReaders.IDZone = Zones.ID WHERE TerminalReaders.NFC = '" & _nfcTag & "'"

                        Dim tb As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)
                        If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            _IDTerminal = roTypes.Any2Integer(tb.Rows(0)("IDTerminal"))
                            _IDReader = roTypes.Any2Integer(tb.Rows(0)("ID"))
                            _Punch.IDReader = _IDReader
                            ' La zona es la de la configuración del lector, y no la que me haya podido pasar el portal por geolocalizar
                            idZone = roTypes.Any2Integer(tb.Rows(0)("IDZone"))
                            ' Y por tanto el fichaje y la zona registrada es fiable
                            _IsReliableZone = True
                            _IsReliable = True

                            _Mode = roTypes.Any2String(tb.Rows(0)("Mode"))
                            Select Case _Mode
                                Case "ACC"
                                    _Punch.LoadNewAcc(_InputDateTime, _IDTerminal, -1, idZone, , _InputCapture, Not _IsReliable, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone)
                                Case "TA"
                                    If roTypes.Any2Boolean(tb.Rows(0)("IsWorkingZone")) Then
                                        _Direction = "E"
                                    Else
                                        _Direction = "S"
                                    End If
                            End Select
                        Else
                            oPunchStatus = PunchSeqStatus.NFC_TAG_NOT_FOUND
                            Return False
                        End If
                    End If
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roPunch::DoPortalPunch")
                End Try

                If _Mode <> "ACC" Then
                    ' Obtengo configuración tiempo mínimo entre una entrada-salida i una salida-entrada
                    Dim oTerminalState As New Terminal.roTerminalState : roBusinessState.CopyTo(_State, oTerminalState)
                    Dim oReader As New Terminal.roTerminal.roTerminalReader(_IDTerminal, _IDReader, oTerminalState)
                    If oReader IsNot Nothing AndAlso oTerminalState.Result <> TerminalBaseResultEnum.NoError AndAlso oReader.InteractiveConfig IsNot Nothing Then
                        intPunchPeriodRTIn = oReader.InteractiveConfig.PunchPeriodRTIn
                        intPunchPeriodRTOut = oReader.InteractiveConfig.PunchPeriodRTOut
                    Else
                        intPunchPeriodRTIn = Any2Integer(oParameters.Parameter(Parameters.PunchPeriodRTIn))
                        intPunchPeriodRTOut = Any2Integer(oParameters.Parameter(Parameters.PunchPeriodRTOut))
                    End If

                    If _Direction = "E" Then
                        If Not _isForgottenPunch Then
                            If oLastPunchType = PunchStatus.In_ AndAlso DateDiff(DateInterval.Minute, xLastPunchDateTime, _InputDateTime) < intPunchPeriodRTIn Then
                                oPunchStatus = PunchSeqStatus.Repited_IN
                            Else
                                oPunchStatus = PunchStatus.In_
                                _Punch.LoadNewIN_Ext(_InputDateTime, _IDTerminal, -1, _IDCause, idZone, , _InputCapture, Not _IsReliable, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone, Not _IsReliableZone)
                            End If
                        Else
                            oPunchStatus = PunchStatus.In_
                            _Punch.LoadNewIN_Ext(_InputDateTime, _IDTerminal, -1, _IDCause, idZone, , _InputCapture, Not _IsReliable, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone, Not _IsReliableZone)
                        End If
                    ElseIf _Direction = "S" Then
                        If Not _isForgottenPunch Then
                            If oLastPunchType = MovementStatus.Out_ AndAlso DateDiff(DateInterval.Minute, xLastPunchDateTime, _InputDateTime) < intPunchPeriodRTOut Then
                                oPunchStatus = PunchSeqStatus.Repited_OUT
                            Else
                                oPunchStatus = PunchStatus.Out_
                                _Punch.LoadNewOUT_Ext(_InputDateTime, _IDTerminal, , _IDCause, idZone, , _InputCapture, Not _IsReliable, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone, Not _IsReliableZone)
                            End If
                        Else
                            oPunchStatus = PunchStatus.Out_
                            _Punch.LoadNewOUT_Ext(_InputDateTime, _IDTerminal, , _IDCause, idZone, , _InputCapture, Not _IsReliable, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone, Not _IsReliableZone)
                        End If
                    End If
                End If

                _Punch.IP = strIP

                If oPunchStatus <> PunchSeqStatus.Repited_IN AndAlso oPunchStatus <> PunchSeqStatus.Repited_OUT Then
                    _Punch.TemperatureAlert = Nothing
                    _Punch.MaskAlert = Nothing
                    _Punch.InTelecommute = _isTelecommuting
                    _Punch.Remarks = remarks
                    If isApp Then _Punch.Source = PunchSource.PortalApp
                    _Punch.NotReliableCause = notReliableCause
                    If _isForgottenPunch Then _Punch.Source = PunchSource.Request

                    ' Guardamos el fichaje
                    bolRet = _Punch.Save()
                    If bolRet Then
                        oPunchStatus = PunchSeqStatus.OK
                    Else
                        oPunchStatus = PunchSeqStatus.NoSequence
                    End If
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::DoPortalPunch")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::DoPortalPunch")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Añade un fichaje de presencia para un empleado. El tipo de fichaje generado se calcula en función del estado actual del empleado.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_InputDateTime">Fecha y hora del fichaje a generar</param>
        ''' <param name="_IDTerminal">Código del terminal por el que se realiza el fichaje</param>
        ''' <param name="_IDReader">Número del lector por el que se realiza el fichaje</param>
        ''' <param name="_IDCause">Código de la justificación. Si no hay, se tiene que informar como '-1'.</param>
        ''' <param name="_InputCapture">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
        ''' <param name="_Punch">Devuelve el fichaje generado.</param>
        ''' <param name="_InputType">Devuelve el tipo de fichaje generado</param>
        ''' <param name="_SeqStatus">Devuelve el estado de la secuencia resultado de la acción</param>
        ''' <param name="_SaveData">Indica si se tiene que guardar el fichaje generado o no.</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DoSequencePunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _IDCause As Integer, ByVal _InputCapture As System.Drawing.Image,
                                               ByRef _Punch As roPunch, ByRef _InputType As PunchStatus, ByRef _SeqStatus As PunchSeqStatus, ByVal _SaveData As Boolean, ByRef _State As roPunchState,
                                               Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal _LocationZone As String = "", Optional ByVal _FullAddress As String = "", Optional ByVal _TimeZone As String = "") As Boolean

            Dim bolRet As Boolean = False

            Try

                _SeqStatus = PunchSeqStatus.OK

                ' Obtener información del último marcaje de presencia del empleado
                Dim oLastPunchType As PunchStatus = PunchStatus.Indet_
                Dim xLastPunchDateTime As DateTime
                Dim lngLastPunchID As Long = -1
                _Punch = New roPunch(_IDEmployee, -1, _State)
                _Punch.GetLastPunchPres(oLastPunchType, xLastPunchDateTime, lngLastPunchID)
                _Punch.ID = lngLastPunchID
                _Punch.Load()

                If oLastPunchType <> MovementStatus.Indet_ Then

                    ' Verificar tiempo entre marcajes (MaxMovementHours)
                    Dim oParameters As New roParameters("OPTIONS", True)
                    Dim intMaxMovement As Integer = 0
                    Dim oTime As roTime = roTypes.Any2Time(oParameters.Parameter(Parameters.MovMaxHours))
                    If oTime.IsValid Then intMaxMovement = oTime.Minutes

                    If intMaxMovement > 0 AndAlso
                       DateDiff(DateInterval.Minute, xLastPunchDateTime, _InputDateTime) > intMaxMovement Then

                        ' El tiempo entre el marcaje actual y el anterior és superior al máximo (MaxMovementHours)
                        If oLastPunchType = PunchStatus.Out_ Then
                            _SeqStatus = PunchSeqStatus.Max_SeqOK
                        Else
                            _SeqStatus = PunchSeqStatus.Max_SeqERR
                        End If
                        _Punch.LoadNewIN_Ext(_InputDateTime, _IDTerminal, , _IDCause, , , _InputCapture, , _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone)
                        _InputType = PunchStatus.In_
                    Else

                        Dim intPunchPeriodRTIn As Integer = 0
                        Dim intPunchPeriodRTOut As Integer = 0

                        ' Obtengo configuración tiempo mínimo entre una entrada-salida i una salida-entrada
                        Dim oTerminalState As New Terminal.roTerminalState : roBusinessState.CopyTo(_State, oTerminalState)
                        Dim oReader As New Terminal.roTerminal.roTerminalReader(_IDTerminal, _IDReader, oTerminalState)
                        If oReader IsNot Nothing AndAlso oTerminalState.Result <> TerminalBaseResultEnum.NoError AndAlso oReader.InteractiveConfig IsNot Nothing Then
                            intPunchPeriodRTIn = oReader.InteractiveConfig.PunchPeriodRTIn
                            intPunchPeriodRTOut = oReader.InteractiveConfig.PunchPeriodRTOut
                        Else
                            intPunchPeriodRTIn = Any2Integer(oParameters.Parameter(Parameters.PunchPeriodRTIn))
                            intPunchPeriodRTOut = Any2Integer(oParameters.Parameter(Parameters.PunchPeriodRTOut))
                        End If

                        ' Detectar marcaje repetido (PunchPeriodRTIn, PunchPeriodRTOut)
                        If oLastPunchType = PunchStatus.In_ Then
                            If DateDiff(DateInterval.Minute, xLastPunchDateTime, _InputDateTime) < intPunchPeriodRTIn Then
                                _SeqStatus = PunchSeqStatus.Repited_IN
                                _InputType = PunchStatus.Indet_
                            Else
                                _InputType = PunchStatus.Out_
                                _Punch.LoadNewOUT_Ext(_InputDateTime, _IDTerminal, , _IDCause, , , _InputCapture, , _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone)
                            End If
                        ElseIf oLastPunchType = MovementStatus.Out_ Then
                            If DateDiff(DateInterval.Minute, xLastPunchDateTime, _InputDateTime) < intPunchPeriodRTOut Then
                                _SeqStatus = PunchSeqStatus.Repited_OUT
                                _InputType = PunchStatus.Indet_
                            Else
                                _InputType = PunchStatus.In_
                                _Punch.LoadNewIN_Ext(_InputDateTime, _IDTerminal, -1, _IDCause, , , _InputCapture, , _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone)
                            End If
                        End If

                    End If
                Else

                    _InputType = MovementStatus.In_
                    _Punch.LoadNewIN_Ext(_InputDateTime, _IDTerminal, -1, _IDCause, , , _InputCapture, , _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone)
                    _SeqStatus = PunchSeqStatus.OK

                End If

                bolRet = True
                If _SaveData AndAlso _SeqStatus <> PunchSeqStatus.Repited_IN AndAlso _SeqStatus <> PunchSeqStatus.Repited_OUT Then
                    bolRet = _Punch.Save()
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::DoSequencePunch")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::DoSequencePunch")
            End Try

            Return bolRet

        End Function

        Public Shared Function DoTaskPunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDTask As Integer,
                                           ByVal _InputCapture As System.Drawing.Image, ByRef _Punch As roPunch, ByRef _InputType As PunchStatus, ByVal bIsReliable As Boolean,
                                           ByVal _State As roPunchState, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal _LocationZone As String = "",
                                           Optional ByVal _FullAddress As String = "", Optional ByVal _TimeZone As String = "", Optional ByVal Field1 As String = "",
                                           Optional ByVal Field2 As String = "", Optional ByVal Field3 As String = "", Optional ByVal Field4 As Double = -1,
                                           Optional ByVal Field5 As Double = -1, Optional ByVal Field6 As Double = -1, Optional isApp As Boolean = False, Optional notReliableCause As String = Nothing) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim selTask As New Task.roTask
                selTask.ID = _IDTask
                selTask.Load(False)

                Dim oPunchState As Punch.roPunchState = New Punch.roPunchState
                _Punch = New Punch.roPunch(_IDEmployee, -1, oPunchState)
                _Punch.LoadNewBEGIN_Ext(_InputDateTime, _IDTerminal, selTask, , , , _InputCapture, Not bIsReliable, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone, Field1, Field2, Field3, Field4, Field5, Field6)
                If Not bIsReliable Then _Punch.Source = PunchSource.Request
                If isApp Then _Punch.Source = PunchSource.PortalApp
                If notReliableCause IsNot Nothing Then _Punch.NotReliableCause = notReliableCause
                If (_Punch.Save()) Then
                    _InputType = PunchStatus.Task_
                    bolRet = True
                Else
                    _Punch.ID = -1
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::DoSequencePunch")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::DoSequencePunch")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Añade un fichaje de presencia para un empleado. El tipo de fichaje generado se calcula en función del estado actual del empleado.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_InputDateTime">Fecha y hora del fichaje a generar</param>
        ''' <param name="_IDTerminal">Código del terminal por el que se realiza el fichaje</param>
        ''' <param name="_IDTask">Código de la tarea que se va a fichar</param>
        ''' <param name="_InputCapture">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
        ''' <param name="_Punch">Devuelve el fichaje generado.</param>
        ''' <param name="_InputType">Devuelve el tipo de fichaje generado</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DoTaskPunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDTask As Integer, ByVal _InputCapture As System.Drawing.Image, ByRef _Punch As roPunch,
                                           ByRef _InputType As PunchStatus, ByVal _State As roPunchState, Optional ByVal _Lat As Double = -1,
                                           Optional ByVal _Lon As Double = -1, Optional ByVal _LocationZone As String = "", Optional ByVal _FullAddress As String = "", Optional ByVal _TimeZone As String = "", Optional ByVal Field1 As String = "", Optional ByVal Field2 As String = "", Optional ByVal Field3 As String = "" _
                                           , Optional ByVal Field4 As Double = -1, Optional ByVal Field5 As Double = -1, Optional ByVal Field6 As Double = -1) As Boolean

            Dim bolRet As Boolean = False

            Try
                bolRet = DoTaskPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDTask, _InputCapture, _Punch, _InputType, True, _State, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone, Field1, Field2, Field3, Field4, Field5, Field6)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::DoSequencePunch")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::DoSequencePunch")
            End Try

            Return bolRet

        End Function

        Public Shared Function DoCostCenterPunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDCostCenter As Integer, ByVal _InputCapture As System.Drawing.Image, ByRef _Punch As roPunch,
                                           ByRef _InputType As PunchStatus, ByVal bIsReliable As Boolean, ByVal _State As roPunchState, Optional ByVal _Lat As Double = -1,
                                           Optional ByVal _Lon As Double = -1, Optional ByVal _LocationZone As String = "", Optional ByVal _FullAddress As String = "", Optional ByVal _TimeZone As String = "", Optional isApp As Boolean = False, Optional comments As String = Nothing, Optional notReliableCause As String = Nothing) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oPunchState As Punch.roPunchState = New Punch.roPunchState
                _Punch = New Punch.roPunch(_IDEmployee, -1, oPunchState)
                _Punch.LoadNewCostCenterIN_Ext(_InputDateTime, _IDCostCenter, _IDTerminal, , , , _InputCapture, Not bIsReliable, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone)
                If Not bIsReliable Then _Punch.Source = PunchSource.Request
                If isApp Then _Punch.Source = PunchSource.PortalApp
                If comments IsNot Nothing Then _Punch.Remarks = comments
                If notReliableCause IsNot Nothing Then _Punch.NotReliableCause = notReliableCause
                If (_Punch.Save()) Then
                    _InputType = PunchStatus.Task_
                    bolRet = True
                Else
                    _Punch.ID = -1
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::DoSequencePunch")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::DoSequencePunch")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Añade un fichaje de presencia para un empleado. El tipo de fichaje generado se calcula en función del estado actual del empleado.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_InputDateTime">Fecha y hora del fichaje a generar</param>
        ''' <param name="_IDTerminal">Código del terminal por el que se realiza el fichaje</param>
        ''' <param name="_IDCostCenter">Código de la tarea que se va a fichar</param>
        ''' <param name="_InputCapture">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
        ''' <param name="_Punch">Devuelve el fichaje generado.</param>
        ''' <param name="_InputType">Devuelve el tipo de fichaje generado</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DoCostCenterPunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDCostCenter As Integer, ByVal _InputCapture As System.Drawing.Image, ByRef _Punch As roPunch,
                                           ByRef _InputType As PunchStatus, ByVal _State As roPunchState, Optional ByVal _Lat As Double = -1,
                                           Optional ByVal _Lon As Double = -1, Optional ByVal _LocationZone As String = "", Optional ByVal _FullAddress As String = "", Optional ByVal _TimeZone As String = "") As Boolean

            Dim bolRet As Boolean = False

            Try

                bolRet = DoCostCenterPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDCostCenter, _InputCapture, _Punch, _InputType, True, _State, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::DoSequencePunch")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::DoSequencePunch")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Cambia el estado de presencia actual del empleado generando un fichaje a la hora indicada.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_NowDateTime">Fecha y hora actual</param>
        ''' <param name="_InputDateTime">Hora en la que se generará el fichaje olvidado para el cambio de estado</param>
        ''' <param name="_IDTerminal">Código de terminal por el que se realiza la operación</param>
        ''' <param name="_IDReader">Número de lector por el que se realiza la operación</param>
        ''' <param name="_IDCause">Código de la justificación. Si no hay, se tiene que informar como '-1'.</param>
        ''' <param name="_InputCapture">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
        ''' <param name="_Punch">Devuelve el fichaje generado</param>
        ''' <param name="_SaveData">Indica si se tiene que guardar el fichaje generado o no.</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ChangeState(ByVal _IDEmployee As Integer, ByVal _NowDateTime As DateTime, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _IDCause As Integer, ByVal _InputCapture As System.Drawing.Image, ByRef _Punch As roPunch, ByVal _SaveData As Boolean, ByVal _State As roPunchState) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Obtener información del último marcaje de presencia del empleado
                Dim oLastPunchType As PunchStatus = PunchStatus.Indet_
                Dim xLastPunchDateTime As DateTime
                Dim lngLastPunchID As Long = -1
                _Punch = New roPunch(_IDEmployee, -1, _State)
                _Punch.GetLastPunchPres(oLastPunchType, xLastPunchDateTime, lngLastPunchID)
                _Punch.ID = lngLastPunchID
                _Punch.Load()

                Dim bolValidInputDateTime As Boolean = True

                ' Verificar que la fecha y hora indicada sea válida en función del último fichaje
                If oLastPunchType <> PunchStatus.Indet_ Then

                    Dim xMinValue As Date
                    Dim xMaxValue As Date
                    If _NowDateTime.DayOfYear = xLastPunchDateTime.DayOfYear AndAlso _NowDateTime.Year = xLastPunchDateTime.Year Then
                        ' Último marcaje en el mismo día
                        xMinValue = xLastPunchDateTime.AddMinutes(1)
                        xMaxValue = _NowDateTime.AddMinutes(-1)
                    Else
                        ' Último marcaje día o días anterior
                        xMinValue = New Date(_NowDateTime.Year, _NowDateTime.Month, _NowDateTime.Day, 0, 0, 0, DateTimeKind.Unspecified)
                        xMaxValue = _NowDateTime.AddMinutes(-1)
                    End If

                    If _InputDateTime.Hour = xMinValue.Hour Then
                        If _InputDateTime.Minute >= xMinValue.Minute Then
                            bolValidInputDateTime = True
                        End If
                    ElseIf _InputDateTime.Hour > xMinValue.Hour Then
                        bolValidInputDateTime = True
                    End If

                    If bolValidInputDateTime Then
                        bolValidInputDateTime = False
                        If _InputDateTime.Hour = xMaxValue.Hour Then
                            If _InputDateTime.Minute <= xMaxValue.Minute Then
                                bolValidInputDateTime = True
                            End If
                        ElseIf _InputDateTime.Hour < xMaxValue.Hour Then
                            bolValidInputDateTime = True
                        End If
                    End If

                End If

                If bolValidInputDateTime Then

                    Dim xMarca As DateTime
                    xMarca = New DateTime(_NowDateTime.Year, _NowDateTime.Month, _NowDateTime.Day, _InputDateTime.Hour, _InputDateTime.Minute, _InputDateTime.Second, DateTimeKind.Unspecified)
                    Dim intNowMin As Integer = (_NowDateTime.Hour * 60) + _NowDateTime.Minute
                    Dim intInputMin As Integer = (_InputDateTime.Hour * 60) + _InputDateTime.Minute
                    If intNowMin <= intInputMin Then
                        xMarca = xMarca.AddDays(-1)
                    End If

                    ' Generar marcaje olvidado
                    If oLastPunchType = PunchStatus.Out_ OrElse oLastPunchType = PunchStatus.Indet_ Then
                        ' Informamos marcaje de entrada olvidado
                        _Punch.LoadNewIN(xMarca, _IDTerminal, , , , , , True)
                    Else
                        If xMarca.DayOfYear = _NowDateTime.DayOfYear Then
                            ' Informamos marcaje de salida olvidado
                            _Punch.LoadOUT(xMarca, _IDTerminal, , , , _InputCapture, True)
                        Else
                            ' Error.

                        End If
                    End If

                    bolRet = True

                    If _SaveData Then
                        ' Guardamos el movimiento
                        bolRet = _Punch.Save()
                    End If
                Else
                    _State.Result = PunchResultEnum.InvalidChangeStateTime
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::ChangeState")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::ChangeState")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve un DataSet con amb 4 Tablas (Contadores accesos, Accessos, Salida, Incorrectos)
        ''' </summary>
        ''' <param name="_State">oState</param>
        ''' <param name="Feature">FeaturePermission</param>
        ''' <param name="IDZones">ID Zona</param>
        ''' <param name="oActiveTransaction">Transaccion</param>
        ''' <returns>DataSet con  4 DataTables (Contadores accesos, Accessos, Salida, Incorrectos)</returns>
        ''' <remarks></remarks>
        Public Shared Function GetAccessPunchesStatus(ByRef _State As roPunchState, Optional ByVal Feature As String = "", Optional ByVal Type As String = "U",
                                                        Optional ByVal IDAccessGroups As Generic.List(Of Integer) = Nothing,
                                                        Optional ByVal IDZones As Generic.List(Of Integer) = Nothing,
                                                        Optional ByVal bColImage As Boolean = False, Optional ByVal bOnlyCounters As Boolean = False) As DataSet

            Dim dsRet As DataSet = Nothing

            Dim dTblCountZones As New DataTable
            Dim dTblInZone As New DataTable
            Dim dTblOutZone As New DataTable
            Dim dTblIncZone As New DataTable
            Dim dTblInZoneOrdered As New DataTable
            Dim dTblOutZoneOrdered As New DataTable

            Try
                'Contador de accesos OK / Incorrectes per zones
                dTblCountZones.Columns.Add("IDZone", GetType(Integer))
                dTblCountZones.Columns.Add("NameZone", GetType(String))
                dTblCountZones.Columns.Add("AccessOK", GetType(Integer))
                dTblCountZones.Columns.Add("AccessIncorrects", GetType(Integer))

                'Creem la estructura de les taules

                'Zone Access
                If bColImage Then
                    dTblInZone.Columns.Add("ImageEmployee", GetType(Integer))
                End If
                dTblInZone.Columns.Add("IDEmployee", GetType(Integer))
                dTblInZone.Columns.Add("NameEmployee", GetType(String))
                dTblInZone.Columns.Add("PunchesTime", GetType(DateTime))
                dTblInZone.Columns.Add("IDZone", GetType(Integer))
                dTblInZone.Columns.Add("NameZone", GetType(String))
                dTblInZone.Columns.Add("SourceIDZone", GetType(Integer))
                dTblInZone.Columns.Add("SourceNameZone", GetType(String))

                If bColImage Then
                    dTblInZoneOrdered.Columns.Add("ImageEmployee", GetType(Integer))
                End If
                dTblInZoneOrdered.Columns.Add("IDEmployee", GetType(Integer))
                dTblInZoneOrdered.Columns.Add("NameEmployee", GetType(String))
                dTblInZoneOrdered.Columns.Add("PunchesTime", GetType(DateTime))
                dTblInZoneOrdered.Columns.Add("IDZone", GetType(Integer))
                dTblInZoneOrdered.Columns.Add("NameZone", GetType(String))
                dTblInZoneOrdered.Columns.Add("SourceIDZone", GetType(Integer))
                dTblInZoneOrdered.Columns.Add("SourceNameZone", GetType(String))

                'Zone sortida
                If bColImage Then
                    dTblOutZone.Columns.Add("ImageEmployee", GetType(Integer))
                End If
                dTblOutZone.Columns.Add("IDEmployee", GetType(Integer))
                dTblOutZone.Columns.Add("NameEmployee", GetType(String))
                dTblOutZone.Columns.Add("PunchesTime", GetType(DateTime))
                dTblOutZone.Columns.Add("IDZone", GetType(Integer))
                dTblOutZone.Columns.Add("NameZone", GetType(String))
                dTblOutZone.Columns.Add("SourceIDZone", GetType(Integer))
                dTblOutZone.Columns.Add("SourceNameZone", GetType(String))

                If bColImage Then
                    dTblOutZoneOrdered.Columns.Add("ImageEmployee", GetType(Integer))
                End If
                dTblOutZoneOrdered.Columns.Add("IDEmployee", GetType(Integer))
                dTblOutZoneOrdered.Columns.Add("NameEmployee", GetType(String))
                dTblOutZoneOrdered.Columns.Add("PunchesTime", GetType(DateTime))
                dTblOutZoneOrdered.Columns.Add("IDZone", GetType(Integer))
                dTblOutZoneOrdered.Columns.Add("NameZone", GetType(String))
                dTblOutZoneOrdered.Columns.Add("SourceIDZone", GetType(Integer))
                dTblOutZoneOrdered.Columns.Add("SourceNameZone", GetType(String))

                'Accesos incorrectes
                If bColImage Then
                    dTblIncZone.Columns.Add("ImageEmployee", GetType(Integer))
                End If
                dTblIncZone.Columns.Add("IDEmployee", GetType(Integer))
                dTblIncZone.Columns.Add("NameEmployee", GetType(String))
                dTblIncZone.Columns.Add("PunchesTime", GetType(DateTime))
                dTblIncZone.Columns.Add("IDZone", GetType(Integer))
                dTblIncZone.Columns.Add("NameZone", GetType(String))
                dTblIncZone.Columns.Add("Detail", GetType(String))

                Dim strSQL As String = ""
                If bOnlyCounters Then
                    'Si es passan els IDAccessGroups, recuperem les zones
                    If IDAccessGroups IsNot Nothing Then
                        Dim strSQLIdAcc As String = "@SELECT# DISTINCT(IDZone) From AccessGroupsPermissions Where IDAccessGroup IN (" & String.Join(",", IDAccessGroups) & ")"

                        Dim oTblIDacc As DataTable = CreateDataTable(strSQLIdAcc)
                        If oTblIDacc IsNot Nothing AndAlso oTblIDacc.Rows.Count > 0 Then
                            IDZones = New Generic.List(Of Integer)
                            For Each dRowIdAcc As DataRow In oTblIDacc.Rows
                                IDZones.Add(CInt(dRowIdAcc("IDZone")))
                            Next
                        End If
                    End If

                    'camps obligatoris SQL
                    strSQL = $"@SELECT# Distinct(Punches.IdEmployee), Max(DateTime) As DateTime FROM Punches {If(Feature <> String.Empty, roSelector.GetEmployeePermissonInnerJoin(_State.IDPassport, Permission.Read, Feature, Type, "", "Punches.IDEmployee"), "")} 
                            WHERE DateDiff(Day,DateTime,GETDATE()) <= 2 and Punches.Type IN (" & PunchTypeEnum._AV & "," & PunchTypeEnum._L & ")"

                    Dim zonesIDList As String = ""
                    If IDZones IsNot Nothing AndAlso IDZones.Count > 0 Then
                        strSQL &= " AND IDZone IN (" & String.Join(",", IDZones) & ")"
                    End If

                    strSQL = strSQL & " GROUP BY Punches.IdEmployee" ' ORDER BY DateTime DESC"

                    'obtenim el ultim acces a la zona dels usuaris
                    Dim oTblEmp As DataTable = CreateDataTable(strSQL)
                    If oTblEmp IsNot Nothing AndAlso oTblEmp.Rows.Count > 0 Then
                        For Each dRowEmp As DataRow In oTblEmp.Rows
                            'Agafem les dades del moviment d'entrada a la zona
                            Dim strSQLIn As String = "@SELECT# TOP 1 DateTime, Employees.ID as EmployeeID, Employees.Name as NameEmployee," &
                                                        " Zones.ID as ZoneID, Zones.Name as NameZone From Punches, Zones, Employees Where Punches.IDZone = Zones.ID" &
                                                        " And Punches.IDEmployee = Employees.ID and Employees.ID = " & dRowEmp("IDEmployee") & " AND Punches.Type IN (" & PunchTypeEnum._AV & "," & PunchTypeEnum._L & ")" &
                                                        " And DateTime = " & roTypes.SQLDateTime(roTypes.Any2DateTime(dRowEmp("DateTime")))
                            If (zonesIDList <> String.Empty) Then strSQLIn = strSQLIn & " And Punches.IDZone IN (" & zonesIDList & ")"
                            strSQLIn &= " Order By Punches.DateTime DESC, Punches.ID DESC"

                            'Mirem si te un moviment anterior a l'ultima entrada a la zona
                            Dim strSQLBeforeIn As String = "@SELECT# TOP 1 DateTime, Employees.ID as EmployeeID, Employees.Name as NameEmployee," &
                                                        " Zones.ID as ZoneID, Zones.Name as NameZone From Punches, Zones, Employees Where Punches.IDZone = Zones.ID" &
                                                        " And Punches.IDEmployee = Employees.ID and Employees.ID = " & dRowEmp("IDEmployee") & " AND Punches.Type IN (" & PunchTypeEnum._AV & "," & PunchTypeEnum._L & ")" &
                                                        " And DateTime < " & roTypes.SQLDateTime(roTypes.Any2DateTime(dRowEmp("DateTime")))
                            If (zonesIDList <> String.Empty) Then strSQLBeforeIn = strSQLBeforeIn & " And Punches.IDZone NOT IN (" & zonesIDList & ")"
                            strSQLBeforeIn &= " Order By Punches.DateTime DESC, Punches.ID DESC"

                            'Mirem si te un moviment posterior a l'ultim en una zona diferent
                            Dim strSQLOut As String = "@SELECT# TOP 1 DateTime, Employees.ID as EmployeeID, Employees.Name as NameEmployee," &
                                                        " Zones.ID as ZoneID, Zones.Name as NameZone From Punches, Zones, Employees Where Punches.IDZone = Zones.ID" &
                                                        " And Punches.IDEmployee = Employees.ID and Employees.ID = " & dRowEmp("IDEmployee") & " AND Punches.Type IN (" & PunchTypeEnum._AV & "," & PunchTypeEnum._L & ")" &
                                                        " And DateTime > " & roTypes.SQLDateTime(roTypes.Any2DateTime(dRowEmp("DateTime")))
                            If (zonesIDList <> String.Empty) Then strSQLOut = strSQLOut & " And Punches.IDZone NOT IN (" & zonesIDList & ")"
                            strSQLOut &= " Order By Punches.DateTime ASC, Punches.ID ASC"

                            Dim incomeRow As DataRow = Nothing
                            Dim dTblIn As DataTable = CreateDataTable(strSQLIn)
                            If dTblIn IsNot Nothing AndAlso dTblIn.Rows.Count > 0 Then
                                incomeRow = dTblIn.Rows(0)
                            End If

                            Dim beforeRow As DataRow = Nothing
                            Dim dTblBeforeIn As DataTable = CreateDataTable(strSQLBeforeIn)
                            If dTblBeforeIn IsNot Nothing AndAlso dTblBeforeIn.Rows.Count > 0 Then
                                beforeRow = dTblBeforeIn.Rows(0)
                            End If

                            Dim dTblOut As DataTable = CreateDataTable(strSQLOut)
                            Dim outputRow As DataRow = Nothing
                            If dTblOut IsNot Nothing AndAlso dTblOut.Rows.Count > 0 Then
                                outputRow = dTblOut.Rows(0)
                            End If

                            Dim dNewRow As DataRow
                            If outputRow Is Nothing Then
                                dNewRow = dTblInZone.NewRow
                                If incomeRow IsNot Nothing Then
                                    dNewRow("IDEmployee") = incomeRow("EmployeeID")
                                    dNewRow("NameEmployee") = incomeRow("NameEmployee")
                                    dNewRow("IDZone") = incomeRow("ZoneID")
                                    dNewRow("NameZone") = incomeRow("NameZone")
                                    dNewRow("PunchesTime") = incomeRow("DateTime")
                                    If (beforeRow IsNot Nothing) Then
                                        dNewRow("SourceIDZone") = beforeRow("ZoneID")
                                        dNewRow("SourceNameZone") = beforeRow("NameZone")
                                    Else
                                        dNewRow("SourceIDZone") = 0
                                        dNewRow("SourceNameZone") = ""
                                    End If

                                    dTblInZone.Rows.Add(dNewRow)
                                End If
                            Else
                                dNewRow = dTblOutZone.NewRow
                                If outputRow IsNot Nothing AndAlso DateDiff(DateInterval.Minute, outputRow("DateTime"), Now) <= 60 Then
                                    dNewRow("IDEmployee") = outputRow("EmployeeID")
                                    dNewRow("NameEmployee") = outputRow("NameEmployee")
                                    dNewRow("IDZone") = incomeRow("ZoneID")
                                    dNewRow("NameZone") = incomeRow("NameZone")
                                    dNewRow("PunchesTime") = outputRow("DateTime")
                                    dNewRow("SourceIDZone") = outputRow("ZoneID")
                                    dNewRow("SourceNameZone") = outputRow("NameZone")

                                    dTblOutZone.Rows.Add(dNewRow)
                                End If
                            End If
                        Next
                    End If

                    Dim dTblIncAcc As DataTable
                    Dim strSQLInc As String = "@SELECT# DateTime, Employees.ID as EmployeeID, Employees.Name as NameEmployee," &
                                " Zones.ID, Zones.Name as NameZone, Punches.InvalidType From Punches, Zones, Employees WHERE Punches.IDZone = Zones.ID" &
                                " And Punches.IDEmployee = Employees.ID AND Punches.Type = 6"

                    'Recorrem les taules per zones
                    If IDZones IsNot Nothing AndAlso IDZones.Count > 0 Then
                        strSQLInc &= " AND Punches.IDZone IN (" & String.Join(",", IDZones) & ")"
                    End If

                    strSQLInc &= " And DateDiff(Day,DateTime,GETDATE()) <= 2 Order By Punches.DateTime DESC, Punches.ID Desc"
                    dTblIncAcc = CreateDataTable(strSQLInc)

                    If dTblIncAcc IsNot Nothing AndAlso dTblIncAcc.Rows.Count > 0 Then
                        For Each dRowIncAcc As DataRow In dTblIncAcc.Rows
                            Dim dNR As DataRow = dTblIncZone.NewRow
                            dNR("IDEmployee") = dRowIncAcc("EmployeeID")
                            dNR("NameEmployee") = dRowIncAcc("NameEmployee")
                            dNR("IDZone") = dRowIncAcc("ID")
                            dNR("NameZone") = dRowIncAcc("NameZone")
                            dNR("PunchesTime") = dRowIncAcc("DateTime")
                            dNR("Detail") = _State.Language.Translate("IncorrectAccess.Type." & dRowIncAcc("InvalidType").ToString, "")
                            dTblIncZone.Rows.Add(dNR)
                        Next
                    End If

                    'Ordenamos las tablas de fichajes de entrada y salida
                    Dim orderedInRows As DataRow() = dTblInZone.Select("", "PunchesTime DESC")
                    Dim orderedOutRows As DataRow() = dTblOutZone.Select("", "PunchesTime DESC")

                    For Each cRow As DataRow In orderedInRows
                        dTblInZoneOrdered.ImportRow(cRow)
                    Next

                    For Each cRow As DataRow In orderedOutRows
                        dTblOutZoneOrdered.ImportRow(cRow)
                    Next
                End If

                'Recuperem el idparent
                Dim intParent As Integer = 1
                Dim dTbl As DataTable
                Dim oZoneState As New Zone.roZoneState(_State.IDPassport, _State.Context, _State.ClientAddress)
                dTbl = Zone.roZone.GetZonesDataTable("", oZoneState)

                Dim dRowsP As DataRow() = dTbl.Select("IDParent IS NULL")
                If dRowsP.Length > 0 Then
                    intParent = dRowsP(0)("ID")
                End If

                If IDZones Is Nothing Then
                    strSQL = "@SELECT# ID as IDZone, Name as NameZone, 0 as AccessOK, 0 as AccessIncorrects from Zones Where IDParent = " & intParent
                    dTblCountZones = CreateDataTable(strSQL)
                Else
                    strSQL = "@SELECT# ID as IDZone, Name as NameZone, 0 as AccessOK, 0 as AccessIncorrects from Zones Where IDParent = " & intParent
                    If IDZones.Count > 0 Then
                        strSQL &= " AND ID IN (" & String.Join(",", IDZones) & ")"
                    End If
                    dTblCountZones = CreateDataTable(strSQL)
                End If

                'Carreguem les columnes
                For Each dRow As DataRow In dTblCountZones.Rows
                    Dim dRows As DataRow() = dTblInZone.Select("IDZone = " & dRow("IDZone"))
                    dRow("AccessOk") = dRows.Count

                    Dim pSQL As String = $"@SELECT# DISTINCT(Punches.idemployee) from Punches {If(Feature <> String.Empty, roSelector.GetEmployeePermissonInnerJoin(_State.IDPassport, Permission.Read, Feature, Type, "", "Punches.IDEmployee"), "")} 
                            Where IDzone = " & dRow("IDZone") & " and DateDiff(Day,Datetime,GETDATE()) <= 2 AND Punches.IDEmployee > 0 AND InvalidType > 0"

                    Dim dTblInv As DataTable = CreateDataTable(pSQL)

                    Dim oCountInv As Integer = 0
                    If dTblInv IsNot Nothing AndAlso dTblInv.Rows.Count > 0 Then
                        oCountInv = dTblInv.Rows.Count
                    End If
                    dRow("AccessIncorrects") = oCountInv
                Next

                'Afegim les dues taules al DataSet
                dsRet = New DataSet

                dsRet.Tables.Add(dTblCountZones) 'Contadors
                dsRet.Tables.Add(dTblInZoneOrdered) 'Usuaris a les zones
                dsRet.Tables.Add(dTblOutZoneOrdered) 'Usuaris han sortit ult.hora
                dsRet.Tables.Add(dTblIncZone) 'Accesos incorrectes
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roPunch::GetAccessPunchStatus")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::GetAccessPunchStatus")
            End Try

            Return dsRet

        End Function

        ''' <summary>
        ''' Devuelve un Datatable con los fichajes no asignados a ningún empleado
        ''' </summary>
        ''' <param name="_State">oState</param>
        ''' <param name="oActiveConnection">Conexión</param>
        ''' <param name="oActiveTransaction">Transaccion</param>
        ''' <param name="lngIDCard">Tarjeta asociada</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetInvalidPunches(ByRef _State As roPunchState, Optional ByVal lngIDCard As Long = -1) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM Punches " &
                                       " WHERE IDEmployee Is Null OR  IDEmployee <= 0 " &
                                       " AND Type IN(" & PunchTypeEnum._IN & "," & PunchTypeEnum._OUT & "," & PunchTypeEnum._L & "," & PunchTypeEnum._AUTO & "," & PunchTypeEnum._AI & ") "
                If lngIDCard <> -1 Then
                    strSQL &= " AND IDCredential = " & Any2String(lngIDCard)
                Else
                    strSQL &= " AND IDCredential > 0 "
                End If

                strSQL &= " ORDER BY Punches.ID "

                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing AndAlso oRet.Rows.Count = 0 Then
                    Dim oUserTask As New UserTask.roUserTask()
                    oUserTask.ID = UserTask.roUserTask.roUserTaskObject & ":\\" & UserTask.roUserTask.NO_EMPLOYEE_TASK
                    oUserTask.Delete()
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::GetInvalidPunches")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::GetInvalidPunches")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve un Datatable con los fichajes no importados
        ''' </summary>
        ''' <param name="_State">oState</param>
        ''' <param name="oActiveConnection">Conexión</param>
        ''' <param name="oActiveTransaction">Transaccion</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetInvalidEntries(ByRef _State As roPunchState) As DataTable

            Dim oRet As DataTable = Nothing
            Try

                Dim strSQL As String = "@SELECT# * FROM InvalidEntries where processed <> 'Y' "

                strSQL &= " ORDER BY CreatedOn Desc "

                oRet = CreateDataTableWithoutTimeouts(strSQL)

                If oRet IsNot Nothing AndAlso oRet.Rows.Count = 0 Then
                    Dim oUserTask As New UserTask.roUserTask()
                    oUserTask.ID = UserTask.roUserTask.roUserTaskObject & ":\\" & UserTask.roUserTask.BAD_ENTRIES_TASK
                    oUserTask.Delete()
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::GetInvalidEntries")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::GetInvalidEntries")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guardalos fichajes no asignados a ningún empleado e intenta obtener el empleado al que pueden estar asignados
        ''' en base a la tarjeta que tenga asignada
        ''' </summary>
        ''' <param name="tbEntries">Datatable de fichajes</param>
        ''' <param name="_State">oState</param>
        ''' <param name="oActiveTransaction">Transaccion</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveInvalidPunches(ByVal tbEntries As DataTable, ByRef _State As roPunchState) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oPunch As roPunch
                Dim bolSaved As Boolean = True

                bolRet = True

                For Each oRow As DataRow In tbEntries.Rows

                    bolSaved = True

                    Select Case oRow.RowState
                        Case DataRowState.Unchanged, DataRowState.Added

                        Case DataRowState.Modified

                            oPunch = New roPunch(_State)
                            oPunch.ID = oRow("ID")
                            oPunch.Load()
                            With oPunch
                                If Not IsDBNull(oRow("IDCredential")) Then .IDCredential = oRow("IDCredential")
                                If Not IsDBNull(oRow("IDEmployee")) Then .IDEmployee = oRow("IDEmployee")

                                If .IDEmployee = 0 AndAlso .IDCredential > 0 Then
                                    ' Intentamos obtener el id del empleado que tenga dicha tarjeta
                                    Dim tb As DataTable = roBusinessSupport.GetEmployeesByIDCard(.IDCredential, "", "", "", _State)
                                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                                        Dim oRowCard As DataRow = tb.Rows(0)
                                        If Not IsDBNull(oRowCard("IDEmployee")) Then .IDEmployee = oRowCard("IDEmployee")
                                    End If
                                End If

                            End With
                            bolRet = oPunch.Save()

                        Case DataRowState.Deleted
                            oRow.RejectChanges() ' Cmabiar el estado de la fila para poder leer sus datos
                            oPunch = New roPunch(_State)
                            oPunch.ID = oRow("ID")
                            oPunch.Load()
                            bolRet = oPunch.Delete()
                        Case Else
                            bolRet = True
                            bolSaved = False

                    End Select

                    If Not bolRet Then

                        Exit For
                    End If

                Next

                If bolRet Then
                    ' Recalculamos
                    roConnector.InitTask(TasksType.MOVES)
                End If

                If bolRet Then
                    ' Miramos si aun existen fichajes no asignados, en caso negativo
                    ' eliminamos la tarea de usuario
                    Dim tbInvalidPunches As DataTable = roPunch.GetInvalidPunches(_State)
                    If tbInvalidPunches IsNot Nothing AndAlso tbInvalidPunches.Rows.Count = 0 Then
                        Dim oUserTask As New UserTask.roUserTask()
                        oUserTask.ID = UserTask.roUserTask.roUserTaskObject & ":\\" & UserTask.roUserTask.NO_EMPLOYEE_TASK
                        oUserTask.Delete()
                    End If
                End If
            Catch ex As DbException
                bolRet = False
                _State.UpdateStateInfo(ex, "roPunch::SavePunches")
            Catch ex As Exception
                bolRet = False
                _State.UpdateStateInfo(ex, "roPunch::SavePunches")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function ReprocessInvalidEntries(ByVal aInvalidEntries() As DTOs.roPunchInvalidEntry, ByRef _State As roPunchState) As Boolean

            Dim bolRet As Boolean = False
            Dim gExportFile As String
            Dim lInvalidEntries As New List(Of DTOs.roPunchInvalidEntry)

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                bolRet = True

                Dim tmpSet As New roSettings
                lInvalidEntries = aInvalidEntries.ToList()

                For Each sBehavior As String In lInvalidEntries.Select(Function(y) y.Behavior).Distinct
                    '0.- Guardamos en fichero
                    Try
                        gExportFile = tmpSet.GetVTSetting(eKeys.Readings) & "\" & sBehavior.Replace("Behavior ", "") & ".VTR.repro"
                        File.AppendAllLines(gExportFile, aInvalidEntries.ToList.FindAll(Function(z) z.Behavior = sBehavior).Select(Function(x) x.RawData))
                    Catch ex As Exception
                        bolRet = False
                        Exit For
                    End Try

                    '1.- Marcamos registros
                    Dim strQuery As String = "@UPDATE# InvalidEntries SET Processed = 'Y' WHERE ID IN (" & String.Join(",", aInvalidEntries.ToList.FindAll(Function(z) z.Behavior = sBehavior).Select(Function(x) x.ID).ToArray) & ")"
                    bolRet = ExecuteSql(strQuery)

                    If Not bolRet Then
                        Exit For
                    End If
                Next

                If bolRet Then
                    ' Lanzamos collector
                    roConnector.InitTask(TasksType.MOVES)
                End If

                If bolRet Then
                    ' Miramos si aun existen fichajes incorrectos, en caso negativo eliminamos la tarea de usuario
                    Dim tbInvalidEntries As DataTable = roPunch.GetInvalidEntries(_State)
                    If tbInvalidEntries IsNot Nothing AndAlso tbInvalidEntries.Rows.Count = 0 Then
                        Dim oUserTask As New UserTask.roUserTask()
                        oUserTask.ID = UserTask.roUserTask.roUserTaskObject & ":\\BAD_ENTRIES"
                        oUserTask.Delete()
                    End If
                End If
            Catch ex As DbException
                bolRet = False
                _State.UpdateStateInfo(ex, "roPunch::ReprocessInvalidEntries")
            Catch ex As Exception
                bolRet = False
                _State.UpdateStateInfo(ex, "roPunch::ReprocessInvalidEntries")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene el resumen del último fichaje del empleado
        ''' </summary>
        Public Shared Function GetEmployeeLastPunchSummary(idEmployee As Integer, ByVal _State As roPunchState) As DataTable

            Dim oRet As DataTable = Nothing
            Try

                Dim strSQL As String = "@SELECT# top 1 p.ID, p.ActualType, DateTime, t.Description Terminal, z.Name Zone, p.Location,
                                        p.LocationZone, p.hasphoto, p.photoonazure, pc.Capture, isnull(ta.Project,'') + ' ' + isnull(ta.Name,'')  as Name
                                        From Punches p
                                        Left Join Terminals t on p.IDTerminal = t.ID
                                        Left Join Zones z on p.IDZone = z.ID
                                        Left Join PunchesCaptures pc on p.ID = pc.IDPunch
                                        Left Join Tasks ta on p.TypeData = ta.ID
                                        where idEmployee = " & idEmployee & "
                                        order by DateTime desc"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::GetInvalidPunches")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::GetInvalidPunches")
            End Try

            Return oRet

        End Function

        Public Overrides Function ToString() As String
            Dim str As String = String.Empty
            str = "IDEmployee = " & Me.IDEmployee.ToString & ", DateTime = " & Me.DateTime.ToString & ", Type = " & Me.intType.ToString & ", ActualType = " & Me.intActualType.ToString & ", TypeData = " & Me.TypeData & ", IDTerminal = " & Me.intIDTerminal.ToString & ", IDZone = " & Me.intIDZone.ToString
            Return str
        End Function

        Public Function ToLogInfo() As String
            Dim str As String = String.Empty
            str = vbCr
            str = str & "IDEmployee: " & Me.IDEmployee.ToString & vbCr
            str = str & "DateTime: " & Me.DateTime.ToString & vbCr
            str = str & "Type: " & Me.intType.ToString & vbCr
            str = str & "ActualType: " & Me.intActualType.ToString & vbCr
            str = str & "TypeData: " & Me.TypeData & vbCr
            str = str & "IDTerminal: " & Me.intIDTerminal.ToString & vbCr
            str = str & "IDZone: " & Me.intIDZone.ToString & vbCr
            Return str
        End Function

#End Region

#End Region

#Region "Helper Methods"

        ''' <summary>
        ''' Retorna un DataSet amb 2 Taules (Accessos, Incorrectes)
        ''' </summary>
        ''' <param name="_State">oState</param>
        ''' <param name="ListIdZones">Lista de Zonas</param>
        ''' <param name="ListFields">Lista de la ficha a incluirZonas</param>
        ''' <returns>DataSet amb 2 DataTables (Accessos, Incorrectes)</returns>
        ''' <remarks></remarks>
        Public Shared Function GetAccessPunchesMonitor(ByRef _State As roPunchState, ByVal ListIdZones As Generic.List(Of Integer),
                                                       ByVal ListFields As Generic.List(Of String), ByVal bColImage As Boolean) As DataSet

            Dim dsRet As DataSet = Nothing

            Try

                Dim strSQL As String
                Dim strSQLImage As String = ""
                Dim strSQLListFields As String = ""

                Dim dTblInZone As New DataTable
                Dim dTblInvalidZone As New DataTable

                If ListIdZones IsNot Nothing AndAlso ListIdZones.Count > 0 Then

                    Dim fld As roEmployeeUserField
                    Dim oEmployeeState As New roUserFieldState(_State.IDPassport)

                    If bColImage Then strSQLImage = ", Employees.Image "

                    If ListFields IsNot Nothing AndAlso ListFields.Count > 0 Then
                        For Each item As String In ListFields
                            strSQLListFields = $"{strSQLListFields}'' AS {item}, "
                        Next
                        strSQLListFields = "," & strSQLListFields.Substring(0, Len(strSQLListFields) - 2)
                    End If

                    strSQL = $"@SELECT# TOP 12 Punches.IDEmployee, Employees.Name AS NameEmployee, Punches.DateTime, Punches.IDZone, Zones.Name AS NameZone {strSQLImage} {strSQLListFields}
                               FROM Punches LEFT OUTER JOIN Zones On Punches.IDZone = Zones.ID LEFT OUTER JOIN Employees On Punches.IDEmployee = Employees.ID 
                               WHERE (Punches.Type = {PunchTypeEnum._AV} OR Punches.Type = {PunchTypeEnum._L}) AND (DATEDIFF(Minute, Punches.DateTime, GETDATE()) <= 30) And (Punches.IDZone In ("
                    For Each IDZone As Integer In ListIdZones
                        strSQL = $"{strSQL} {IDZone},"
                    Next
                    strSQL = strSQL.Substring(0, Len(strSQL) - 1) & ")) ORDER BY Punches.DateTime DESC"
                    dTblInZone = CreateDataTable(strSQL)
                    If dTblInZone IsNot Nothing AndAlso dTblInZone.Rows.Count > 0 Then
                        For Each dRowEmp As DataRow In dTblInZone.Rows
                            If WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Access", Permission.Read, roTypes.Any2Integer(dRowEmp("IDEmployee"))) Then
                                If strSQLListFields <> "" Then
                                    For Each ItemField As String In ListFields
                                        fld = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(roTypes.Any2String(dRowEmp("IDEmployee")),
                                                                                                           ItemField, dRowEmp("Datetime"), oEmployeeState, False)
                                        If fld IsNot Nothing Then
                                            dRowEmp(ItemField) = fld.FieldValue
                                        Else
                                            dRowEmp(ItemField) = ""
                                        End If
                                    Next
                                End If
                            Else
                                dRowEmp.Delete()
                            End If
                        Next
                        dTblInZone.AcceptChanges()
                    End If

                    strSQL = $"@SELECT# TOP 12 Punches.IDEmployee, Employees.Name As NameEmployee, Punches.DateTime, Punches.IDZone, Zones.Name As NameZone, Punches.InvalidType, Punches.TypeDetails {strSQLImage} {strSQLListFields} 
                               FROM Punches LEFT OUTER JOIN Zones On Punches.IDZone = Zones.ID LEFT OUTER JOIN Employees On Punches.IDEmployee = Employees.ID 
                               WHERE (Punches.Type = {PunchTypeEnum._AI}) 
                               AND (DATEDIFF(Minute, Punches.DateTime, GETDATE()) <= 30) And (Punches.IDZone In ("
                    For Each IDZone As Integer In ListIdZones
                        strSQL = $"{strSQL} {IDZone}, "
                    Next
                    strSQL = strSQL.Substring(0, Len(strSQL) - 1) & ")) ORDER BY Punches.DateTime DESC"
                    dTblInvalidZone = CreateDataTable(strSQL)
                    If dTblInvalidZone IsNot Nothing AndAlso dTblInvalidZone.Rows.Count > 0 Then
                        For Each dRowEmp As DataRow In dTblInvalidZone.Rows
                            If WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Access", Permission.Read, dRowEmp("IDEmployee")) Then
                                If strSQLListFields <> "" Then
                                    For Each ItemField As String In ListFields
                                        fld = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(roTypes.Any2String(dRowEmp("IDEmployee")),
                                                                                                           ItemField, dRowEmp("Datetime"), oEmployeeState, False)
                                        If fld IsNot Nothing Then
                                            dRowEmp(ItemField) = fld.FieldValue
                                        Else
                                            dRowEmp(ItemField) = ""
                                        End If
                                    Next
                                End If
                            Else
                                dRowEmp.Delete()
                            End If
                        Next
                        dTblInvalidZone.AcceptChanges()
                    End If
                End If

                'Afegim les dues taules al DataSet
                dsRet = New DataSet

                dsRet.Tables.Add(dTblInZone)
                dsRet.Tables.Add(dTblInvalidZone)
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessMoves::GetAccessPunchesMonitor")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessMoves::GetAccessPunchesMonitor")
            End Try

            Return dsRet

        End Function

        Public Shared Function GetAllowTasksByEmployeeOnPunch(ByVal intIDEmployee As Integer, ByVal oState As roPunchState, Optional ByVal checkActualTask As Boolean = False, Optional ByVal actualTaskId As Integer = -1) As Generic.List(Of Task.roTask)
            Return GetAllowTasksByEmployeeOnPunchFiltered(intIDEmployee, oState, checkActualTask, actualTaskId, "--no--filter--")
        End Function

        ''' <summary>
        ''' Carga la lista de tareas que puede fichar un empleado
        ''' </summary>
        ''' <param name="intIDEmployee">ID del empleado</param>
        ''' <returns>Devuelve los datos de las tareas que puede fichar el empleado</returns>
        ''' <remarks></remarks>
        Public Shared Function GetAllowTasksByEmployeeOnPunchFiltered(ByVal intIDEmployee As Integer, ByVal oState As roPunchState, Optional ByVal checkActualTask As Boolean = False, Optional ByVal actualTaskId As Integer = -1, Optional ByVal taskFilter As String = "") As Generic.List(Of Task.roTask)
            Dim oRet As New Generic.List(Of Task.roTask)

            Try

                Dim bContinue = False
                If (taskFilter = "--no--filter--") Then
                    bContinue = True
                    taskFilter = ""
                End If

                ' 01. Obtenemos las tareas que puede fichar cualquier empleado
                Dim strSQL As String = "@SELECT# ID " &
                                            " FROM Tasks " &
                                                "  WHERE TypeAuthorization = 0 and Tasks.ID > 0 and Status= 0 AND ((Name like '%" & taskFilter & "%') OR (Project like '%" & taskFilter & "%'))"
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If Not bContinue Then
                    'Debemos aplicar conteo de conteo de tareas
                    If (tb.Rows.Count < 200) Then
                        bContinue = True
                    Else
                        Dim oTask As New Task.roTask
                        oTask.ID = -2
                        oTask.Project = ""
                        oTask.Name = oState.Language.Translate("Tasks.Over200.Error", "")

                        oRet.Add(oTask)
                    End If
                End If

                If bContinue Then
                    Dim strTasks As String = ""
                    If tb IsNot Nothing Then
                        For Each oRow As DataRow In tb.Rows
                            If strTasks.Length > 0 Then
                                strTasks = $"{strTasks},{Any2String(oRow("ID"))}"
                            Else
                                strTasks = Any2String(oRow("ID"))
                            End If
                        Next
                    End If

                    ' 02. Obtenemos las tareas que puede fichar el empleado indicado
                    strSQL = "@SELECT# IDTask  as ID from EmployeeTasks "
                    strSQL = strSQL & " Where IDEmployee = " & intIDEmployee.ToString
                    strSQL = strSQL & " AND IDTask In(@SELECT# DISTINCT ID FROM Tasks WHERE TypeAuthorization = 1 AND Tasks.ID > 0 and Status= 0)"
                    tb = CreateDataTable(strSQL, )
                    If tb IsNot Nothing Then
                        For Each oRow As DataRow In tb.Rows
                            If strTasks.Length > 0 Then
                                strTasks = $"{strTasks}, {Any2String(oRow("ID"))}"
                            Else
                                strTasks = Any2String(oRow("ID"))
                            End If
                        Next
                    End If

                    If strTasks.Length = 0 Then strTasks = "-1"

                    strSQL = "@SELECT# IDGroup, Path from sysrovwCurrentEmployeeGroups "
                    strSQL = strSQL & " Where IDEmployee = " & intIDEmployee.ToString
                    tb = CreateDataTable(strSQL, )
                    Dim intIDGroup As Integer = 0
                    Dim strPathGroup As String = ""

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        intIDGroup = Any2Integer(tb.Rows(0).Item("IDGroup"))
                        strPathGroup = Any2String(tb.Rows(0).Item("Path"))
                    End If

                    Dim i As Integer = 0
                    Dim strAllPathGroups As String = ""
                    For i = 0 To StringItemsCount(strPathGroup, "\") - 1
                        If strAllPathGroups.Length = 0 Then
                            strAllPathGroups = String2Item(strPathGroup, i, "\")
                        Else
                            strAllPathGroups = $"{strAllPathGroups},  {String2Item(strPathGroup, i, "\")}"
                        End If
                    Next
                    If strAllPathGroups.Length = 0 Then strAllPathGroups = "-1"

                    strSQL = "@SELECT# DISTINCT IDTask  as ID from GroupTasks "
                    strSQL = strSQL & " Where (IDGroup  = " & intIDGroup.ToString & " OR IDGroup IN(" & strAllPathGroups & "))"
                    strSQL = strSQL & " AND IDTask In(@SELECT# DISTINCT ID FROM Tasks WHERE TypeAuthorization = 1 AND Tasks.ID > 0 and Status= 0)"
                    strSQL = strSQL & " AND IDTask Not In(" & strTasks & ")"
                    tb = CreateDataTable(strSQL, )
                    If tb IsNot Nothing Then
                        For Each oRow As DataRow In tb.Rows
                            If strTasks.Length > 0 Then
                                strTasks = $"{strTasks}, {Any2String(oRow("ID"))}"
                            Else
                                strTasks = Any2String(oRow("ID"))
                            End If
                        Next
                    End If

                    strSQL = "@SELECT# ID, countWorking  FROM Tasks "
                    strSQL &= " LEFT OUTER JOIN (@SELECT# COUNT(*) countWorking, TypeData AS IDTask FROM ( "
                    strSQL &= " @SELECT# ROW_NUMBER() OVER (PARTITION BY idemployee ORDER BY idemployee,datetime DESC, id DESC) AS RowNumber, * "
                    strSQL &= " FROM Punches WHERE ActualType = 4 AND IDEmployee <> " & intIDEmployee.ToString & ") countPunches WHERE countPunches.RowNumber = 1 GROUP BY typedata) aux ON aux.IDTask = Tasks.ID "
                    strSQL &= " WHERE ID IN (" & strTasks & ")  "
                    strSQL &= " ORDER By Priority asc, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) "
                    tb = CreateDataTable(strSQL, )
                    If tb IsNot Nothing Then
                        Dim oTask As Task.roTask = Nothing
                        Dim oTaskState As Task.roTaskState
                        For Each oRow As DataRow In tb.Rows
                            Dim bolAllowTask As Boolean = True
                            oTaskState = New Task.roTaskState
                            oTask = New Task.roTask(Any2Long(oRow("ID")), oTaskState, False)

                            If oTask.TypeCollaboration = TaskTypeCollaborationEnum._THEFIRST Then
                                ' Solo puede fichar el primero
                                Dim intFirstIDEmployee As Integer = 0
                                intFirstIDEmployee = Any2Integer(ExecuteScalar("@SELECT# TOP 1 IDEMPLOYEE FROM PUNCHES WHERE ActualType =" & PunchTypeEnum._TASK & " AND TypeData=" & oTask.ID & " Order by DateTime asc, ID asc"))
                                If intFirstIDEmployee > 0 AndAlso intFirstIDEmployee <> intIDEmployee Then bolAllowTask = False
                            End If
                            If bolAllowTask AndAlso oTask.ModeCollaboration = TaskModeCollaborationEnum._ONEPERSONATTIME Then
                                ' Solo una persona a la vez
                                Dim intCountIDEmployee As Integer = 0
                                intCountIDEmployee = roTypes.Any2Integer(oRow("countWorking"))
                                If intCountIDEmployee > 0 Then bolAllowTask = False
                            End If

                            If bolAllowTask Then
                                If oTask.TypeActivation = TaskTypeActivationEnum._ATDATE Then
                                    ' Activa a una fecha
                                    If oTask.ActivationDate >= Now Then bolAllowTask = False
                                End If
                                If oTask.TypeActivation = TaskTypeActivationEnum._ATFINISHTASK AndAlso oTask.ActivationTask > 0 Then
                                    ' Al finalizar otra tarea
                                    Dim xState As Task.roTaskState = New Task.roTaskState
                                    Dim xTask As Task.roTask = New Task.roTask(oTask.ActivationTask, xState)

                                    If checkActualTask Then
                                        If xTask.ID <> actualTaskId Then bolAllowTask = False
                                    Else
                                        If xTask.Status = TaskStatusEnum._ON Then bolAllowTask = False
                                    End If
                                End If
                                If oTask.TypeActivation = TaskTypeActivationEnum._ATRUNTASK AndAlso oTask.ActivationTask > 0 Then
                                    ' Al iniciar otra tarea
                                    Dim intCountIDEmployee As Integer = 0
                                    intCountIDEmployee = Any2Integer(ExecuteScalar("@SELECT# TOP 1 IDEMPLOYEE FROM PUNCHES WHERE ActualType =" & PunchTypeEnum._TASK & " AND TypeData=" & oTask.ActivationTask & " Order by DateTime asc, ID asc"))
                                    If intCountIDEmployee = 0 Then bolAllowTask = False
                                End If
                            End If

                            If bolAllowTask AndAlso oTask.TypeClosing = TaskTypeClosingEnum._ATDATE Then
                                ' Finalizada a partir de una fecha
                                If oTask.ClosingDate < Now Then bolAllowTask = False
                            End If

                            If bolAllowTask Then oRet.Add(oTask)
                        Next
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPunch::GetAllowTasksByEmployeeOnPunch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPunch::GetAllowTasksByEmployeeOnPunch")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAvailableTasksByEmployee(ByVal intIDEmployee As Integer, ByVal oState As roPunchState, ByRef iLimit As Integer, Optional ByVal searchCriteria As String = Nothing) As Integer
            Dim iRet As Integer = 0
            Try
                'Límite de tareas a mostrar
                Dim advMaxTasksOnList As Integer = 200
                iLimit = 200
                Dim oParameter As New roAdvancedParameter("VTPortal.Tasks.MaxTasksOnList", New roAdvancedParameterState)
                advMaxTasksOnList = roTypes.Any2Integer(oParameter.Value)
                If advMaxTasksOnList <= 200 Then
                    iLimit = 200
                ElseIf advMaxTasksOnList >= 500 Then
                    iLimit = 500
                Else
                    iLimit = advMaxTasksOnList
                End If

                Dim strSQL As String = ""

                strSQL = "@DECLARE# @actualDate DATETIME " &
                                       "SET @actualDate = GETDATE() " &
                                        "@SELECT# count(*) from( " &
                                            "@SELECT# ID, Priority, (isnull(Tasks.Project,'') + ': ' + Tasks.Name) As FullTaskName, Tasks.ModeCollaboration, Tasks.TypeCollaboration, Tasks.TypeActivation, Tasks.TypeClosing, Tasks.ActivationTask, Tasks.ActivationDate, Tasks.ClosingDate, isnull(Tasks.BarCode,'') as BarCodeStr, Tasks.Project, Tasks.Name,CAST(tasks.Description AS VARCHAR(4000)) as TaskDescription  " &
                                                "From Tasks  " &
                                                    "WHERE TypeAuthorization = 0 and Tasks.ID > 0 and Status= 0 "
                If searchCriteria IsNot Nothing AndAlso searchCriteria.Length > 0 Then
                    strSQL += " and (isnull(Tasks.Project, '') + ': ' + Tasks.Name) LIKE '%" & searchCriteria.Replace("*", "%") & "%'"
                End If
                strSQL += " UNION " &
                                                    "@SELECT# IDTask  as ID, Priority, (isnull(Tasks.Project,'') + ': ' + Tasks.Name) As FullTaskName, Tasks.ModeCollaboration, Tasks.TypeCollaboration, Tasks.TypeActivation, Tasks.TypeClosing, Tasks.ActivationTask, Tasks.ActivationDate, Tasks.ClosingDate, isnull(Tasks.BarCode,'') as BarCodeStr, CAST(tasks.Description AS VARCHAR(4000)) as TaskDescription, Tasks.Project, Tasks.Name    " &
                                                        "from EmployeeTasks  " &
                                                            "inner join Tasks on EmployeeTasks.IDTask = Tasks.ID " &
                                                                "Where EmployeeTasks.IDEmployee = " & intIDEmployee & " and Tasks.TypeAuthorization = 1 And Tasks.Status = 0  "
                If searchCriteria IsNot Nothing AndAlso searchCriteria.Length > 0 Then
                    strSQL += " and (isnull(Tasks.Project, '') + ': ' + Tasks.Name) LIKE '%" & searchCriteria.Replace("*", "%") & "%'"
                End If
                strSQL += " UNION  " &
                                                    "@SELECT# IDTask as ID, Priority, (isnull(Tasks.Project,'') + ': ' + Tasks.Name) As FullTaskName, Tasks.ModeCollaboration, Tasks.TypeCollaboration, Tasks.TypeActivation, Tasks.TypeClosing, Tasks.ActivationTask, Tasks.ActivationDate, Tasks.ClosingDate, isnull(Tasks.BarCode,'') as BarCodeStr, CAST(tasks.Description AS VARCHAR(4000)) as TaskDescription, Tasks.Project, Tasks.Name    " &
                                                        "from GroupTasks  " &
                                                            "inner join Tasks on GroupTasks.IDTask = Tasks.ID " &
                                                                "where IDGroup in(@SELECT# * from dbo.GetEmployeeGroupTree(" & intIDEmployee & ", -1,@actualDate)) and Tasks.TypeAuthorization = 1 And Tasks.Status = 0  "
                If searchCriteria IsNot Nothing AndAlso searchCriteria.Length > 0 Then
                    strSQL += " and (isnull(Tasks.Project, '') + ': ' + Tasks.Name) LIKE '%" & searchCriteria.Replace("*", "%") & "%'"
                End If
                strSQL += ") availableTasks " &
                                            " left outer join (@SELECT# COUNT(*) countWorking, TypeData as IDTask from ( " &
                                            " @SELECT# ROW_NUMBER() over (partition by idemployee order by idemployee,datetime desc, id desc) as RowNumber, * " &
                                            " from Punches where ActualType = 4 and IDEmployee <>  " & intIDEmployee & ") countPunches where  countPunches.RowNumber = 1 group by typedata) aux on aux.IDTask = availableTasks.ID "

                iRet = roTypes.Any2Integer(ExecuteScalar(strSQL))
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPunch::GetAvailableTasksByEmployee")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPunch::GetAvailableTasksByEmployee")
            End Try

            Return iRet

        End Function

        Public Shared Function GetAllowTasksByEmployeeOnPunchV2(ByVal intIDEmployee As Integer, ByVal oState As roPunchState, Optional ByVal checkActualTask As Boolean = False, Optional ByVal actualTaskId As Integer = -1, Optional ByVal limitTaskLength As Boolean = True, Optional ByVal searchCriteria As String = Nothing) As DataTable
            Dim oRet As New DataTable

            Dim intShowBarCode As Integer = 0
            Try

                intShowBarCode = roTypes.Any2Integer(roAdvancedParameter.GetAdvancedParameterValue("VTPortal.ShowBarCode", Nothing, False))

                Dim strSQL As String = "@DECLARE# @actualDate DATETIME " &
                                       "SET @actualDate = GETDATE() " &
                                        "@SELECT# availableTasks.*, isnull(aux.countWorking,0) as ActuallyWorking from( " &
                                            "@SELECT# ID, Priority, (isnull(Tasks.Project,'') + ': ' + Tasks.Name) As FullTaskName, Tasks.ModeCollaboration, Tasks.TypeCollaboration, Tasks.TypeActivation, Tasks.TypeClosing, Tasks.ActivationTask, Tasks.ActivationDate, Tasks.ClosingDate, isnull(Tasks.BarCode,'') as BarCodeStr, Tasks.Project, Tasks.Name,CAST(tasks.Description AS VARCHAR" & IIf(Not limitTaskLength, "(4000)", "") & ") as TaskDescription  " &
                                                "From Tasks  " &
                                                    "WHERE TypeAuthorization = 0 and Tasks.ID > 0 and Status= 0 "

                If searchCriteria IsNot Nothing AndAlso searchCriteria.Length > 0 Then
                    strSQL += " and (isnull(Tasks.Project, '') + ': ' + Tasks.Name) LIKE '%" & searchCriteria.Replace("*", "%") & "%'"
                End If

                strSQL += "UNION " &
                                                    "@SELECT# IDTask  as ID, Priority, (isnull(Tasks.Project,'') + ': ' + Tasks.Name) As FullTaskName, Tasks.ModeCollaboration, Tasks.TypeCollaboration, Tasks.TypeActivation, Tasks.TypeClosing, Tasks.ActivationTask, Tasks.ActivationDate, Tasks.ClosingDate, isnull(Tasks.BarCode,'') as BarCodeStr, CAST(tasks.Description AS VARCHAR" & IIf(Not limitTaskLength, "(4000)", "") & ") as TaskDescription, Tasks.Project, Tasks.Name    " &
                                                        "from EmployeeTasks  " &
                                                            "inner join Tasks on EmployeeTasks.IDTask = Tasks.ID " &
                                                                "Where EmployeeTasks.IDEmployee = " & intIDEmployee & " and Tasks.TypeAuthorization = 1 And Tasks.Status = 0  "
                If searchCriteria IsNot Nothing AndAlso searchCriteria.Length > 0 Then
                    strSQL += " and (isnull(Tasks.Project, '') + ': ' + Tasks.Name) LIKE '%" & searchCriteria.Replace("*", "%") & "%'"
                End If

                strSQL += "UNION  " &
                                                    "@SELECT# IDTask as ID, Priority, (isnull(Tasks.Project,'') + ': ' + Tasks.Name) As FullTaskName, Tasks.ModeCollaboration, Tasks.TypeCollaboration, Tasks.TypeActivation, Tasks.TypeClosing, Tasks.ActivationTask, Tasks.ActivationDate, Tasks.ClosingDate, isnull(Tasks.BarCode,'') as BarCodeStr, CAST(tasks.Description AS VARCHAR" & IIf(Not limitTaskLength, "(4000)", "") & ") as TaskDescription, Tasks.Project, Tasks.Name    " &
                                                        "from GroupTasks  " &
                                                            "inner join Tasks on GroupTasks.IDTask = Tasks.ID " &
                                                                "where IDGroup in(@SELECT# * from dbo.GetEmployeeGroupTree(" & intIDEmployee & ", -1,@actualDate)) and Tasks.TypeAuthorization = 1 And Tasks.Status = 0  "
                If searchCriteria IsNot Nothing AndAlso searchCriteria.Length > 0 Then
                    strSQL += " and (isnull(Tasks.Project, '') + ': ' + Tasks.Name) LIKE '%" & searchCriteria.Replace("*", "%") & "%'"
                End If

                strSQL += ") availableTasks " &
                                            " left outer join (@SELECT# COUNT(*) countWorking, TypeData as IDTask from ( " &
                                            " @SELECT# ROW_NUMBER() over (partition by idemployee order by idemployee,datetime desc, id desc) as RowNumber, * " &
                                            " from Punches where ActualType = 4 and IDEmployee <>  " & intIDEmployee & ") countPunches where  countPunches.RowNumber = 1 group by typedata) aux on aux.IDTask = availableTasks.ID " &
                                            " ORDER By availableTasks.Priority asc, availableTasks.FullTaskName "

                oRet = CreateDataTable(strSQL, )
                If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then
                    For Each oRow As DataRow In oRet.Rows
                        Dim bolAllowTask As Boolean = True

                        If intShowBarCode Then
                            'Si hay que mostrar el codigo de barras,
                            ' lo añadimos al campo FullTaskName
                            oRow("FullTaskName") = roTypes.Any2String(oRow("BarCodeStr")) & " " & roTypes.Any2String(oRow("FullTaskName"))
                        End If

                        If Any2Long(oRow("TypeCollaboration")) = TaskTypeCollaborationEnum._THEFIRST Then
                            ' Solo puede fichar el primero
                            Dim intFirstIDEmployee As Integer = 0
                            intFirstIDEmployee = Any2Integer(ExecuteScalar("@SELECT# TOP 1 IDEMPLOYEE FROM PUNCHES WHERE ActualType =" & PunchTypeEnum._TASK & " And TypeData=" & Any2Long(oRow("ID")) & " Order by DateTime asc, ID asc"))
                            If intFirstIDEmployee > 0 AndAlso intFirstIDEmployee <> intIDEmployee Then bolAllowTask = False
                        End If
                        If bolAllowTask AndAlso Any2Long(oRow("ModeCollaboration")) = TaskModeCollaborationEnum._ONEPERSONATTIME Then
                            ' Solo una persona a la vez
                            Dim intCountIDEmployee As Integer = 0
                            intCountIDEmployee = Any2Integer(oRow("ActuallyWorking"))
                            If intCountIDEmployee > 0 Then bolAllowTask = False
                        End If

                        If bolAllowTask Then
                            If Any2Long(oRow("TypeActivation")) = TaskTypeActivationEnum._ATDATE Then
                                ' Activa a una fecha
                                If Not IsDBNull(oRow("ActivationDate")) AndAlso roTypes.Any2DateTime(oRow("ActivationDate")) >= Date.Now Then bolAllowTask = False
                            End If
                            If Any2Long(oRow("TypeActivation")) = TaskTypeActivationEnum._ATFINISHTASK AndAlso Any2Long(oRow("ActivationTask")) > 0 Then
                                ' Al finalizar otra tarea
                                Dim xState As Task.roTaskState = New Task.roTaskState
                                Dim xTask As Task.roTask = New Task.roTask(Any2Long(oRow("ActivationTask")), xState, False)

                                If checkActualTask Then
                                    If xTask.ID <> actualTaskId Then bolAllowTask = False
                                Else
                                    If xTask.Status = TaskStatusEnum._ON Then bolAllowTask = False
                                End If
                            End If
                            If Any2Long(oRow("TypeActivation")) = TaskTypeActivationEnum._ATRUNTASK AndAlso Any2Long(oRow("ActivationTask")) > 0 Then
                                ' Al iniciar otra tarea
                                Dim intCountIDEmployee As Integer = 0
                                intCountIDEmployee = Any2Integer(ExecuteScalar("@SELECT# TOP 1 IDEMPLOYEE FROM PUNCHES WHERE ActualType =" & PunchTypeEnum._TASK & " And TypeData=" & Any2Long(oRow("ActivationTask")) & " Order by DateTime asc, ID asc"))
                                If intCountIDEmployee = 0 Then bolAllowTask = False
                            End If
                        End If

                        If bolAllowTask AndAlso Any2Long(oRow("TypeClosing")) = TaskTypeClosingEnum._ATDATE Then
                            ' Finalizada a partir de una fecha
                            If Not IsDBNull(oRow("ClosingDate")) AndAlso roTypes.Any2DateTime(oRow("ClosingDate")) >= Date.Now Then bolAllowTask = False
                        End If

                        If Not bolAllowTask Then oRow.Delete()
                    Next

                    oRet.AcceptChanges()
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPunch::GetAllowTasksByEmployeeOnPunch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPunch::GetAllowTasksByEmployeeOnPunch")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Carga la lista de tareas que puede fichar un empleado, incluyendo un parámetro de filtrado sobre nombre de tareas
        ''' </summary>
        ''' <param name="intIDEmployee">ID del empleado</param>
        ''' <returns>Devuelve los datos de las tareas que puede fichar el empleado</returns>
        ''' <remarks></remarks>
        Public Shared Function GetAllowTasksByEmployeeOnPunchWithPattern(ByVal intIDEmployee As Integer, ByVal oState As roPunchState, ByRef bProbablyMoreThan20Results As Boolean, Optional ByVal checkActualTask As Boolean = False, Optional ByVal actualTaskId As Integer = -1, Optional sPattern As String = "") As Generic.List(Of Task.roTask)
            Dim oRet As New Generic.List(Of Task.roTask)
            Dim sPatternForWhere As String = ""

            Try
                If sPattern <> "" Then
                    sPatternForWhere = " AND (isnull(Tasks.Project,'') + ' ' + Tasks.Name) like '%" & sPattern.ToLower.Replace("*", "%") & "%' "
                End If

                ' 01. Obtenemos las tareas que puede fichar cualquier empleado
                Dim strSQL As String = "@SELECT# ID " &
                                            " FROM Tasks " &
                                                "  WHERE TypeAuthorization = 0 and Tasks.ID > 0 and Status= 0" & sPatternForWhere

                Dim tb As DataTable = CreateDataTable(strSQL, )
                Dim strTasks As String = ""
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        If strTasks.Length > 0 Then
                            strTasks = $"{strTasks}, {Any2String(oRow("ID"))}"
                            If strTasks.Split(",").Length > 100 Then
                                ' No sigo buscando. Más que probablemente pasaré del límite de 20. Solicitaré que refinen la búsqueda ...
                                bProbablyMoreThan20Results = True
                                Return oRet
                            End If
                        Else
                            strTasks = Any2String(oRow("ID"))
                        End If
                    Next
                End If

                ' 02. Obtenemos las tareas que puede fichar el empleado indicado
                strSQL = "@SELECT# IDTask  as ID from EmployeeTasks "
                strSQL = strSQL & " Where IDEmployee = " & intIDEmployee.ToString
                strSQL = strSQL & " AND IDTask In(@SELECT# DISTINCT ID FROM Tasks WHERE TypeAuthorization = 1 AND Tasks.ID > 0 and Status= 0 " & sPatternForWhere & ")"
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        If strTasks.Length > 0 Then
                            strTasks = $"{strTasks}, {Any2String(oRow("ID"))}"
                            If strTasks.Split(",").Length > 100 Then
                                ' No sigo buscando. Más que probablemente pasaré del límite de 20. Solicitaré que refinen la búsqueda ...
                                bProbablyMoreThan20Results = True
                                Return oRet
                            End If
                        Else
                            strTasks = Any2String(oRow("ID"))
                        End If
                    Next
                End If

                If strTasks.Length = 0 Then strTasks = "-1"

                strSQL = "@SELECT# IDGroup, Path from sysrovwCurrentEmployeeGroups "
                strSQL = strSQL & " Where IDEmployee = " & intIDEmployee.ToString
                tb = CreateDataTable(strSQL, )
                Dim intIDGroup As Integer = 0
                Dim strPathGroup As String = ""

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    intIDGroup = Any2Integer(tb.Rows(0).Item("IDGroup"))
                    strPathGroup = Any2String(tb.Rows(0).Item("Path"))
                End If

                Dim i As Integer = 0
                Dim strAllPathGroups As String = ""
                For i = 0 To StringItemsCount(strPathGroup, "\") - 1
                    If strAllPathGroups.Length = 0 Then
                        strAllPathGroups = String2Item(strPathGroup, i, "\")
                    Else
                        strAllPathGroups = $"{strAllPathGroups},{String2Item(strPathGroup, i, "\")} "
                    End If
                Next
                If strAllPathGroups.Length = 0 Then strAllPathGroups = "-1"

                strSQL = "@SELECT# DISTINCT IDTask  as ID from GroupTasks "
                strSQL = strSQL & " Where (IDGroup  = " & intIDGroup.ToString & " OR IDGroup IN(" & strAllPathGroups & "))"
                strSQL = strSQL & " AND IDTask In(@SELECT# DISTINCT ID FROM Tasks WHERE TypeAuthorization = 1 AND Tasks.ID > 0 and Status= 0 " & sPatternForWhere & ")"
                strSQL = strSQL & " AND IDTask Not In(" & strTasks & ")"
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        If strTasks.Length > 0 Then
                            strTasks = $"{strTasks}, {Any2String(oRow("ID"))}"
                            If strTasks.Split(",").Length > 100 Then
                                ' No sigo buscando. Más que probablemente pasaré del límite de 20. Solicitaré que refinen la búsqueda ...
                                bProbablyMoreThan20Results = True
                                Return oRet
                            End If
                        Else
                            strTasks = Any2String(oRow("ID"))
                        End If
                    Next
                End If

                strSQL = "@SELECT# ID, countWorking  FROM Tasks "
                strSQL &= " LEFT OUTER JOIN (@SELECT# COUNT(*) countWorking, TypeData AS IDTask FROM ( "
                strSQL &= " @SELECT# ROW_NUMBER() OVER (PARTITION BY idemployee ORDER BY idemployee,datetime DESC, id DESC) AS RowNumber, * "
                strSQL &= " FROM Punches WHERE ActualType = 4 AND IDEmployee <> " & intIDEmployee.ToString & ") countPunches WHERE countPunches.RowNumber = 1 GROUP BY typedata) aux ON aux.IDTask = Tasks.ID "
                strSQL &= " WHERE ID IN (" & strTasks & ")  "
                strSQL &= " ORDER By Priority asc, (isnull(Tasks.Project,'') + ' ' + Tasks.Name) "
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    Dim oTask As Task.roTask = Nothing
                    Dim oTaskState As Task.roTaskState
                    For Each oRow As DataRow In tb.Rows
                        Dim bolAllowTask As Boolean = True
                        oTaskState = New Task.roTaskState
                        oTask = New Task.roTask(Any2Long(oRow("ID")), oTaskState, False)

                        If oTask.TypeCollaboration = TaskTypeCollaborationEnum._THEFIRST Then
                            ' Solo puede fichar el primero
                            Dim intFirstIDEmployee As Integer = 0
                            intFirstIDEmployee = Any2Integer(ExecuteScalar("@SELECT# TOP 1 IDEMPLOYEE FROM PUNCHES WHERE ActualType =" & PunchTypeEnum._TASK & " AND TypeData=" & oTask.ID & " Order by DateTime asc, ID asc"))
                            If intFirstIDEmployee > 0 AndAlso intFirstIDEmployee <> intIDEmployee Then bolAllowTask = False
                        End If
                        If bolAllowTask AndAlso oTask.ModeCollaboration = TaskModeCollaborationEnum._ONEPERSONATTIME Then
                            ' Solo una persona a la vez
                            Dim intCountIDEmployee As Integer = 0
                            intCountIDEmployee = roTypes.Any2Integer(oRow("countWorking"))
                            If intCountIDEmployee > 0 Then bolAllowTask = False
                        End If

                        If bolAllowTask Then
                            If oTask.TypeActivation = TaskTypeActivationEnum._ATDATE Then
                                ' Activa a una fecha
                                If oTask.ActivationDate >= Now Then bolAllowTask = False
                            End If
                            If oTask.TypeActivation = TaskTypeActivationEnum._ATFINISHTASK AndAlso oTask.ActivationTask > 0 Then
                                ' Al finalizar otra tarea
                                Dim xState As Task.roTaskState = New Task.roTaskState
                                Dim xTask As Task.roTask = New Task.roTask(oTask.ActivationTask, xState)

                                If checkActualTask Then
                                    If xTask.ID <> actualTaskId Then bolAllowTask = False
                                Else
                                    If xTask.Status = TaskStatusEnum._ON Then bolAllowTask = False
                                End If
                            End If
                            If oTask.TypeActivation = TaskTypeActivationEnum._ATRUNTASK AndAlso oTask.ActivationTask > 0 Then
                                ' Al iniciar otra tarea
                                Dim intCountIDEmployee As Integer = 0
                                intCountIDEmployee = Any2Integer(ExecuteScalar("@SELECT# TOP 1 IDEMPLOYEE FROM PUNCHES WHERE ActualType =" & PunchTypeEnum._TASK & " AND TypeData=" & oTask.ActivationTask & " Order by DateTime asc, ID asc"))
                                If intCountIDEmployee = 0 Then bolAllowTask = False
                            End If
                        End If

                        If bolAllowTask AndAlso oTask.TypeClosing = TaskTypeClosingEnum._ATDATE Then
                            ' Finalizada a partir de una fecha
                            If oTask.ClosingDate < Now Then bolAllowTask = False
                        End If

                        If bolAllowTask Then oRet.Add(oTask)
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPunch::GetAllowTasksByEmployeeOnPunch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPunch::GetAllowTasksByEmployeeOnPunch")
            End Try

            Return oRet

        End Function

        Public Shared Function GetPunchesExternalAPI(ByVal oPunchFilterType As PunchFilterType, ByVal Timestamp As DateTime, ByVal StartDate As Date, ByVal EndDate As Date, ByRef _State As roPunchState, Optional ByVal EmployeeID As String = "") As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = ""

                Dim strEmployee As String = ""
                If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
                    Dim iIdEmployee As Integer = 0
                    ' Obtenemos el campo identificador del empleado
                    Dim strImportPrimaryKeyUserField = New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value
                    If strImportPrimaryKeyUserField = String.Empty Then strImportPrimaryKeyUserField = "NIF"

                    Dim tbEmployee As DataTable = roEmployeeUserField.GetIDEmployeesFromUserFieldValue(strImportPrimaryKeyUserField, EmployeeID, Now, New roUserFieldState(-1))
                    If tbEmployee IsNot Nothing AndAlso tbEmployee.Rows.Count > 0 Then iIdEmployee = tbEmployee.Rows(0).Item("idemployee")
                    If iIdEmployee > 0 Then
                        strEmployee = " AND Punches.IDEmployee = " & iIdEmployee.ToString & " "
                    Else
                        strEmployee = " AND Punches.IDEmployee = -1 "
                    End If

                End If

                Select Case oPunchFilterType
                    Case PunchFilterType.ByTimeStamp
                        strSQL = "@SELECT# TOP 1000 * FROM Punches WHERE isnull(TimeStamp, CreatedOn) >=" & Any2Time(Timestamp).SQLDateTime & " AND IDEmployee > 0 "
                        strSQL += strEmployee
                        strSQL += " Order by isnull(TimeStamp, CreatedOn) asc "

                    Case PunchFilterType.ByDatePeriod
                        strSQL = "@SELECT# TOP 1000 * FROM Punches WHERE DateTime >=" & Any2Time(StartDate).SQLDateTime & " AND DateTime <= " & Any2Time(EndDate).SQLDateTime & " AND IDEmployee > 0 "
                        strSQL += strEmployee
                        strSQL += " Order by Datetime asc "
                End Select

                tbRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::GetPunchesExternalAPI")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::GetPunchesExternalAPI")
            End Try

            Return tbRet

        End Function

        Public Shared Function GetPunchesExternalAPIEx(ByVal oPunchFilterType As PunchFilterType, ByVal Timestamp As DateTime, ByVal StartDate As Date, ByVal EndDate As Date, ByRef _State As roPunchState, Optional ByVal EmployeeID As String = "") As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim sImportPrimaryKeyUserField = New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value
                If sImportPrimaryKeyUserField = String.Empty Then sImportPrimaryKeyUserField = "NIF"

                Dim strSQL As String = String.Empty
                Dim strSQL_Period As String = String.Empty
                Dim strSQL_Order As String = String.Empty

                Select Case oPunchFilterType
                    Case PunchFilterType.ByTimeStamp
                        strSQL_Period = "isnull(TimeStamp, CreatedOn) >=" & Any2Time(Timestamp).SQLDateTime & " "
                        strSQL_Order = " Order by isnull(TimeStamp, CreatedOn) asc"
                    Case PunchFilterType.ByDatePeriod
                        strSQL_Period = "DateTime >=" & Any2Time(StartDate).SQLDateTime & " And DateTime <= " & Any2Time(EndDate).SQLDateTime & " "
                        strSQL_Order = " Order by Datetime asc"
                End Select

                strSQL = $"@SELECT# Causes.ShortName, CONVERT(VARCHAR, NifTable.value) AS NIF, CONVERT(VARCHAR, IdTable.value) AS IdImport, Punches.* FROM Punches 
                           LEFT JOIN Causes ON Causes.Id = Punches.TypeData And Causes.Id > 0 
                           LEFT JOIN (@SELECT# Row_number() OVER (partition BY idemployee, fieldname ORDER BY date DESC) AS 'RowNumber1',* FROM employeeuserfieldvalues WHERE fieldname = 'NIF') NifTable ON NifTable.idemployee = Punches.idemployee AND NifTable.date < Getdate() 
                           LEFT JOIN (@SELECT# Row_number() OVER (partition BY idemployee, fieldname ORDER BY date DESC) AS 'RowNumber1',* FROM employeeuserfieldvalues WHERE fieldname = '{sImportPrimaryKeyUserField}') IdTable ON IdTable.idemployee = Punches.idemployee AND IdTable.date < Getdate() 
                           WHERE {strSQL_Period} 
                           AND Punches.IDEmployee > 0 "

                ' Si se filtra por empleado
                If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
                    Dim iIdEmployee As Integer = 0
                    ' Obtenemos el campo identificador del empleado
                    Dim tbEmployee As DataTable = roEmployeeUserField.GetIDEmployeesFromUserFieldValue(sImportPrimaryKeyUserField, EmployeeID, Now, New roUserFieldState(-1))
                    If tbEmployee IsNot Nothing AndAlso tbEmployee.Rows.Count > 0 Then iIdEmployee = tbEmployee.Rows(0).Item("idemployee")
                    If iIdEmployee > 0 Then
                        strSQL = strSQL & " AND Punches.IDEmployee = " & iIdEmployee.ToString & " "
                    Else
                        strSQL = strSQL & " AND Punches.IDEmployee = -1 "
                    End If
                End If

                strSQL = strSQL & "AND ( NifTable.rownumber1 IS NULL OR NifTable.rownumber1 = 1 ) "
                strSQL = strSQL & "AND ( IdTable.rownumber1 IS NULL OR IdTable.rownumber1 = 1 ) "
                strSQL = strSQL & strSQL_Order

                tbRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::GetPunchesExternalAPIEx")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::GetPunchesExternalAPIEx")
            End Try

            Return tbRet

        End Function

        Public Shared Function GetPunchesWithIDExternalAPIEx(ByVal lIDPunches As List(Of String), ByRef _State As roPunchState) As DataTable
            Dim tbRet As DataTable = Nothing

            Try
                Dim sImportPrimaryKeyUserField = New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value
                If sImportPrimaryKeyUserField = String.Empty Then sImportPrimaryKeyUserField = "NIF"

                Dim strSQL As String = String.Empty

                strSQL = $"@SELECT# Causes.ShortName, CONVERT(VARCHAR, NifTable.value) AS NIF, CONVERT(VARCHAR, IdTable.value) AS IdImport, Punches.* FROM Punches 
                           LEFT JOIN Causes ON Causes.Id = Punches.TypeData And Causes.Id > 0 
                           LEFT JOIN (@SELECT# Row_number() OVER (partition BY idemployee, fieldname ORDER BY date DESC) AS 'RowNumber1',* FROM employeeuserfieldvalues WHERE fieldname = 'NIF') NifTable ON NifTable.idemployee = Punches.idemployee AND NifTable.date < Getdate() 
                           LEFT JOIN (@SELECT# Row_number() OVER (partition BY idemployee, fieldname ORDER BY date DESC) AS 'RowNumber1',* FROM employeeuserfieldvalues WHERE fieldname = '{sImportPrimaryKeyUserField}') IdTable ON IdTable.idemployee = Punches.idemployee AND IdTable.date < Getdate() 
                           WHERE Punches.ID IN ({String.Join(",", lIDPunches)}) 
                           AND Punches.IDEmployee > 0 AND Punches.Type IN (1,2,3)
                           AND ( NifTable.rownumber1 IS NULL OR NifTable.rownumber1 = 1 ) 
                           AND ( IdTable.rownumber1 IS NULL OR IdTable.rownumber1 = 1 ) 
                           ORDER BY Datetime ASC"

                tbRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roPunch::GetPunchesWithIDExternalAPIEx")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roPunch::GetPunchesWithIDExternalAPIEx")
            End Try

            Return tbRet

        End Function

#End Region

#Region "Events"

        Public Event OnLoadNewIN(ByVal oSender As roPunch)

        Public Event OnLoadNewBegin(ByVal oSender As roPunch)

        Public Event OnLoadNewOUT(ByVal oSender As roPunch)

        Public Event OnLoadIN(ByVal oSender As roPunch)

        Public Event OnLoadOUT(ByVal oSender As roPunch)

        Public Event OnLoadNewCostCenter(ByVal oSender As roPunch)

        Public Event OnLoadNewACC(ByVal oSender As roPunch)

#End Region

#Region "Helper Moves"

        Public Shared Function GetMoves(ByVal intIDEmployee As Integer, ByVal xBegin As Date, ByVal xEnd As Date, ByVal bolIncludeCaptures As Boolean, ByRef oState As Employee.roEmployeeState) As DataTable

            Dim tbMoves As DataTable = Nothing

            Try

                Dim sKey As String = "Software\Robotics\VisualTime"
                Dim oRegKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sKey, False)
                Dim strView As String = ""
                If oRegKey IsNot Nothing Then
                    strView = oRegKey.GetValue("ViewPunches", "")
                End If

                Dim strSQL As String
                '## If Not bolIncludeCaptures Then
                '##     strSQL = "@SELECT# Moves.*, CASE WHEN MovesCaptures.InCapture IS NULL THEN 0 ELSE 1 END AS IsInCapture, " & _
                '##                              "CASE WHEN MovesCaptures.OutCapture IS NULL THEN 0 ELSE 1 END AS IsOutCapture "
                '## Else
                '##     strSQL = "@SELECT# Moves.*, MovesCaptures.InCapture, MovesCaptures.OutCapture "
                '## End If
                '## strSQL &= ", CASE WHEN Moves.InDateTime IS NULL THEN Moves.ShiftDate ELSE Moves.InDateTime END AS RealDate "
                '## strSQL &= "FROM Moves LEFT JOIN MovesCaptures " & _
                '##                     "ON Moves.[ID] = MovesCaptures.IDMove " & _
                '##           "WHERE Moves.IDEmployee = " & intIDEmployee.ToString & " AND " & _
                '##                     "Moves.ShiftDate BETWEEN " & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " & Any2Time(xEnd.Date).SQLSmallDateTime & " " & _
                '##              "ORDER BY ISNULL(Moves.InDateTime, Moves.OutDateTime)"

                If strView = "" Then
                    If Not bolIncludeCaptures Then
                        strSQL = "@SELECT# Moves.*, CASE WHEN MovesCaptures.InCapture IS NULL THEN 0 ELSE 1 END AS IsInCapture, " &
                                                 "CASE WHEN MovesCaptures.OutCapture IS NULL THEN 0 ELSE 1 END AS IsOutCapture "
                    Else
                        strSQL = "@SELECT# Moves.*, MovesCaptures.InCapture, MovesCaptures.OutCapture "
                    End If
                    strSQL &= ", CASE WHEN Moves.InDateTime IS NULL THEN Moves.ShiftDate ELSE Moves.InDateTime END AS RealDate "
                    strSQL &= "FROM Moves LEFT JOIN MovesCaptures " &
                                        "ON Moves.[ID] = MovesCaptures.IDMove " &
                              "WHERE Moves.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                        "Moves.ShiftDate BETWEEN " & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " & Any2Time(xEnd.Date).SQLSmallDateTime & " " &
                                 "ORDER BY ISNULL(Moves.InDateTime, Moves.OutDateTime)"
                Else

                    strSQL = "@SELECT# Punches.ID,Punches.IDEmployee,Datetime as InDateTime, null as OutDateTime, IDTerminal as InIDReader, null as OutIDReader, TypeData as InIDCause, null as OutIDCause, ShiftDate, IsNotReliable as IsNotReliableIN,null as IsNotReliableOUT , CASE WHEN PunchesCaptures.Capture IS NULL THEN 0 ELSE 1 END AS IsInCapture, Null as IsOutCapture "
                    strSQL &= ", CASE WHEN Punches.DateTime IS NULL THEN Punches.ShiftDate ELSE Punches.DateTime END AS RealDate "
                    strSQL &= "FROM Punches LEFT JOIN PunchesCaptures " &
                                        "ON Punches.[ID] = PunchesCaptures.IDPunch " &
                              "WHERE Punches.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                        "Punches.ShiftDate BETWEEN " & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " & Any2Time(xEnd.Date).SQLSmallDateTime & " AND " &
                                        "ActualType IN(1) " &
                        "   UNION " &
                        "@SELECT# Punches.ID,Punches.IDEmployee,null as InDateTime, Datetime as OutDateTime, null as InIDReader, IDTerminal as OutIDReader, null as InIDCause, IDCause as OutIDCause, ShiftDate, Null as IsNotReliableIN,IsNotReliable as IsNotReliableOUT , null as IsInCapture , CASE WHEN PunchesCaptures.Capture IS NULL THEN 0 ELSE 1 END AS IsOutCapture "
                    strSQL &= ", CASE WHEN Punches.DateTime IS NULL THEN Punches.ShiftDate ELSE Punches.DateTime END AS RealDate "
                    strSQL &= "FROM Punches LEFT JOIN PunchesCaptures " &
                                        "ON Punches.[ID] = PunchesCaptures.IDPunch " &
                              "WHERE Punches.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                        "Punches.ShiftDate BETWEEN " & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " & Any2Time(xEnd.Date).SQLSmallDateTime & " AND " &
                                        "ActualType IN(2) "
                    strSQL &= "ORDER BY ID"
                End If
                tbMoves = CreateDataTable(strSQL, "Moves")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetMoves")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetMoves")
            End Try

            Return tbMoves

        End Function

        Public Shared Function GetPunchesPres(ByVal intIDEmployee As Integer, ByVal xBegin As Date, ByVal xEnd As Date, ByVal bolIncludeCaptures As Boolean, ByRef oState As roBusinessState, Optional ByVal strFilter As String = "") As DataTable
            Dim tbMoves As DataTable = Nothing

            Try

                Dim strSQL As String
                If Not bolIncludeCaptures Then
                    strSQL = "@SELECT# Punches.*, CASE WHEN PunchesCaptures.Capture IS NULL THEN 0 ELSE 1 END AS IsCapture "
                Else
                    strSQL = "@SELECT# Punches.*, PunchesCaptures.Capture, PunchesCaptures.Capture "
                End If
                strSQL &= ", Punches.DateTime AS RealDate "
                strSQL &= "FROM Punches LEFT JOIN PunchesCaptures " &
                                    "ON Punches.[ID] = PunchesCaptures.IDPunch " &
                          "WHERE Punches.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                    "Punches.ShiftDate BETWEEN " & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " & Any2Time(xEnd.Date).SQLSmallDateTime & " "
                If strFilter.Length > 0 Then
                    strSQL &= " AND " & strFilter
                End If
                strSQL &= "ORDER BY Punches.DateTime, ID"

                tbMoves = CreateDataTable(strSQL, "Punches")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetPunchesPres")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetPunchesPres")
            End Try

            Return tbMoves

        End Function

        Public Shared Function GetMoveList(ByVal intIDEmployee As Integer, ByVal xBegin As Date, ByVal xEnd As Date, ByRef oState As Employee.roEmployeeState) As Move.roMoveList

            Dim oMoves As New Move.roMoveList

            Try

                Dim tbMoves As DataTable = roPunch.GetMoves(intIDEmployee, xBegin, xEnd, False, oState)
                If oState.Result = EmployeeResultEnum.NoError AndAlso tbMoves IsNot Nothing Then

                    Dim oMove As Move.roMove
                    Dim oMoveState As New Move.roMoveState
                    For Each oRow As DataRow In tbMoves.Rows
                        oMove = New Move.roMove(oRow("IDEmployee"), oRow("ID"), oMoveState)
                        If oMoveState.Result = MoveResultEnum.NoError AndAlso oMove IsNot Nothing Then
                            oMoves.Moves.Add(oMove)
                        End If
                    Next

                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetMoveList")
            End Try

            Return oMoves

        End Function

        Public Shared Function GetIncompletMoves(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByRef oState As Employee.roEmployeeState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# Employees.Name, Moves.IDEmployee, Moves.ShiftDate AS Date, Groups.Name as GroupName " &
                         "FROM Employees,EmployeeGroups, Moves, Groups " &
                         "WHERE Employees.ID = EmployeeGroups.IDEmployee AND " &
                               "Employees.ID = Moves.IDEmployee AND " &
                               "EmployeeGroups.IDGroup = Groups.ID AND " &
                               "EmployeeGroups.BeginDate <= ShiftDate AND " &
                               "EmployeeGroups.EndDate >= ShiftDate AND " &
                               "(InDateTime IS NULL OR OutDateTime IS NULL) AND " &
                               "ShiftDate >=" & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " &
                               "ShiftDate <=" & Any2Time(xEnd.Date).SQLSmallDateTime & " "
                If intIDGroup > 0 Then
                    strSQL &= " AND (PATINDEX('%\" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                                    "IDGroup = " & intIDGroup.ToString & ") "
                End If
                If strIDEmployees <> "" Then
                    strSQL &= " AND Moves.IDEmployee IN (" & strIDEmployees & ") "
                End If

                strSQL &= "GROUP BY Moves.ShiftDate, Employees.Name, Moves.IDEmployee,  Groups.Name"

                oRet = CreateDataTable(strSQL, "IncompleteMoves")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetIncompletMoves")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetIncompletMoves")
            End Try

            Return oRet

        End Function

        Public Shared Function GetIncompletDays(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByRef oState As Employee.roEmployeeState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = ""

                strSQL &= "@SELECT# totalPunches.Name, totalPunches.IDEmployee, totalPunches.Date AS Date, totalPunches.GroupName from(" &
                    "@SELECT# ISNULL(pIn.PunchCount,0) as CountIn, ISNULL(pOut.PunchCount,0) as CountOut, ISNULL(pIn.Name,pOut.Name) as Name, Isnull(pIn.IDEmployee,pout.IDEmployee) as IDEmployee ,isnull(pIn.Date,pout.date) as Date ,isnull(pIn.GroupName,pout.GroupName) as GroupName FROM "
                strSQL &= "(@SELECT# count(*) PunchCount, Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date, Groups.Name as GroupName " &
                         "FROM Employees,EmployeeGroups, Punches, Groups " &
                         "WHERE Employees.ID = EmployeeGroups.IDEmployee AND " &
                               "Employees.ID = Punches.IDEmployee AND " &
                               "EmployeeGroups.IDGroup = Groups.ID AND " &
                               "EmployeeGroups.BeginDate <= ShiftDate AND " &
                               "EmployeeGroups.EndDate >= ShiftDate AND " &
                               "(ActualType = 1) AND " &
                               "ShiftDate >=" & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " &
                               "ShiftDate <=" & Any2Time(xEnd.Date).SQLSmallDateTime & " "
                If intIDGroup > 0 Then
                    strSQL &= " AND (PATINDEX('%\" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                                    " PATINDEX('" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                                    "IDGroup = " & intIDGroup.ToString & ") "
                End If
                If strIDEmployees <> "" Then
                    strSQL &= " AND Punches.IDEmployee IN (" & strIDEmployees & ") "
                End If
                strSQL &= " GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee,  Groups.Name)pIn "
                strSQL &= "  FULL OUTER JOIN "

                strSQL &= "(@SELECT# count(*) PunchCount, Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date, Groups.Name as GroupName " &
                         "FROM Employees,EmployeeGroups, Punches, Groups " &
                         "WHERE Employees.ID = EmployeeGroups.IDEmployee AND " &
                               "Employees.ID = Punches.IDEmployee AND " &
                               "EmployeeGroups.IDGroup = Groups.ID AND " &
                               "EmployeeGroups.BeginDate <= ShiftDate AND " &
                               "EmployeeGroups.EndDate >= ShiftDate AND " &
                               "(ActualType = 2) AND " &
                               "ShiftDate >=" & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " &
                               "ShiftDate <=" & Any2Time(xEnd.Date).SQLSmallDateTime & " "
                If intIDGroup > 0 Then
                    strSQL &= " AND (PATINDEX('%\" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                                    " PATINDEX('" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                                    "IDGroup = " & intIDGroup.ToString & ") "
                End If
                If strIDEmployees <> "" Then
                    strSQL &= " AND Punches.IDEmployee IN (" & strIDEmployees & ") "
                End If
                strSQL &= " GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee,  Groups.Name)pOut"
                strSQL &= " ON pIn.IDEmployee = pOut.IDEmployee AND pIn.Date = pOut.Date ) totalPunches "
                strSQL &= " INNER Join sysrovwSecurity_PermissionOverEmployees poe with (NOLOCK) on poe.IDPassport = " & oState.IDPassport & "  And poe.IDEmployee = totalPunches.IDEmployee And DateAdd(Day, DateDiff(Day, 0, totalPunches.Date), 0) between poe.BeginDate And poe.EndDate "
                strSQL &= " INNER Join sysrovwSecurity_PermissionOverFeatures pof With (NOLOCK) On pof.IDPassport = " & oState.IDPassport & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Calendar.Punches.Punches' AND Permission >3 "
                strSQL &= " Where totalPunches.CountIn <> totalPunches.CountOut "

                oRet = CreateDataTableWithoutTimeouts(strSQL,, "IncompleteDays")

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetIncompletDays")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetIncompletDays")
            End Try

            Return oRet

        End Function

        Public Shared Function GetSuspiciousMoves(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByRef oState As Employee.roEmployeeState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# Employees.Name, Moves.IDEmployee, Moves.ShiftDate AS Date, Groups.Name as GroupName " &
                         "FROM Employees,EmployeeGroups, Moves, Groups " &
                         "WHERE Employees.ID = EmployeeGroups.IDEmployee AND " &
                               "Employees.ID = Moves.IDEmployee AND " &
                               "EmployeeGroups.IDGroup = Groups.ID AND " &
                               "EmployeeGroups.BeginDate <= ShiftDate AND " &
                               "EmployeeGroups.EndDate >= ShiftDate AND " &
                               "(IsNotReliableIN = 1 OR IsNotReliableOUT = 1) AND " &
                               "ShiftDate >=" & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " &
                               "ShiftDate <=" & Any2Time(xEnd.Date).SQLSmallDateTime & " "
                If intIDGroup > 0 Then
                    strSQL &= " AND (PATINDEX('%\" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                              "IDGroup = " & intIDGroup.ToString & ") "
                End If
                If strIDEmployees <> "" Then
                    strSQL &= " AND Moves.IDEmployee IN (" & strIDEmployees & ") "
                End If
                strSQL &= "GROUP BY Moves.ShiftDate, Employees.Name, Moves.IDEmployee, Groups.Name " &
                          "UNION " &
                          "@SELECT# Employees.Name, DailyCauses.IDEmployee, DailyCauses.Date, Groups.Name as GroupName " &
                          "FROM Employees,EmployeeGroups, DailyCauses, Groups " &
                          "WHERE Employees.ID = EmployeeGroups.IDEmployee AND " &
                                "Employees.ID = DailyCauses.IDEmployee AND " &
                                "EmployeeGroups.IDGroup = Groups.ID AND " &
                                "EmployeeGroups.BeginDate <= Date AND " &
                                "EmployeeGroups.EndDate >= Date AND " &
                                "(IsNotReliable = 1 ) AND " &
                                "Date >=" & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " &
                                "Date <=" & Any2Time(xEnd.Date).SQLSmallDateTime & " "
                If intIDGroup > 0 Then
                    strSQL &= " AND (PATINDEX('%\" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                              "IDGroup = " & intIDGroup.ToString & ") "
                End If
                If strIDEmployees <> "" Then
                    strSQL &= " AND DailyCauses.IDEmployee IN (" & strIDEmployees & ") "
                End If
                strSQL &= "GROUP BY DailyCauses.Date, Employees.Name, DailyCauses.IDEmployee, Groups.Name"

                oRet = CreateDataTable(strSQL, "SuspiciousMoves")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetSuspiciousMoves")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetSuspiciousMoves")
            End Try

            Return oRet

        End Function

        Public Shared Function GetSuspiciousPunches(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByRef oState As Employee.roEmployeeState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# * FROM (@SELECT# Employees.Name, Punches.IDEmployee, Punches.ShiftDate AS Date, Groups.Name as GroupName " &
                         "FROM Employees,EmployeeGroups, Punches, Groups " &
                         "WHERE Employees.ID = EmployeeGroups.IDEmployee AND " &
                               "Employees.ID = Punches.IDEmployee AND " &
                               "EmployeeGroups.IDGroup = Groups.ID AND " &
                               "EmployeeGroups.BeginDate <= ShiftDate AND " &
                               "EmployeeGroups.EndDate >= ShiftDate AND " &
                               "IsNotReliable = 1  AND " &
                               "ShiftDate >=" & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " &
                               "ShiftDate <=" & Any2Time(xEnd.Date).SQLSmallDateTime & " "
                If intIDGroup > 0 Then
                    strSQL &= " AND (PATINDEX('%\" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                                " PATINDEX('" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                              "IDGroup = " & intIDGroup.ToString & ") "
                End If
                If strIDEmployees <> "" Then
                    strSQL &= " AND Punches.IDEmployee IN (" & strIDEmployees & ") "
                End If
                strSQL &= "GROUP BY Punches.ShiftDate, Employees.Name, Punches.IDEmployee, Groups.Name " &
                          "UNION " &
                          "@SELECT# Employees.Name, DailyCauses.IDEmployee, DailyCauses.Date, Groups.Name as GroupName " &
                          "FROM Employees,EmployeeGroups, DailyCauses, Groups " &
                          "WHERE Employees.ID = EmployeeGroups.IDEmployee AND " &
                                "Employees.ID = DailyCauses.IDEmployee AND " &
                                "EmployeeGroups.IDGroup = Groups.ID AND " &
                                "EmployeeGroups.BeginDate <= Date AND " &
                                "EmployeeGroups.EndDate >= Date AND " &
                                "(IsNotReliable = 1 ) AND " &
                                "Date >=" & Any2Time(xBegin.Date).SQLSmallDateTime & " AND " &
                                "Date <=" & Any2Time(xEnd.Date).SQLSmallDateTime & " "
                If intIDGroup > 0 Then
                    strSQL &= " AND (PATINDEX('%\" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                              "IDGroup = " & intIDGroup.ToString & ") "
                End If
                If strIDEmployees <> "" Then
                    strSQL &= " AND DailyCauses.IDEmployee IN (" & strIDEmployees & ") "
                End If

                strSQL &= "GROUP BY DailyCauses.Date, Employees.Name, DailyCauses.IDEmployee, Groups.Name) suspicious"

                strSQL &= " INNER Join sysrovwSecurity_PermissionOverEmployees poe with (NOLOCK) on poe.IDPassport = " & oState.IDPassport & "  And poe.IDEmployee = suspicious.IDEmployee And DateAdd(Day, DateDiff(Day, 0, suspicious.Date), 0) between poe.BeginDate And poe.EndDate "
                strSQL &= " INNER Join sysrovwSecurity_PermissionOverFeatures pof With (NOLOCK) On pof.IDPassport = " & oState.IDPassport & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Calendar.Punches.Punches' AND Permission >3 "

                oRet = CreateDataTableWithoutTimeouts(strSQL,, "SuspiciousPunches")

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetSuspiciousPunches")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetSuspiciousPunches")
            End Try

            Return oRet

        End Function

        Public Shared Function GetLastMove(ByVal intIDEmployee As Integer, ByRef oState As Employee.roEmployeeState) As Move.roMove

            Dim oMove As Move.roMove = Nothing
            Dim oMoveState As New Move.roMoveState(oState.IDPassport)

            Dim lngLastMoveID As Long = 0
            Dim oLastMoveType As MovementStatus
            Dim xLastMoveDateTime As DateTime

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# TOP 1 ID, InDateTime FROM Moves " &
                                       "WHERE IDEmployee = " & intIDEmployee.ToString & " AND OutDateTime IS NULL " &
                                       "ORDER BY InDateTime DESC"
                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oLastMoveType = MovementStatus.In_
                    xLastMoveDateTime = CDate(tb.Rows(0)("InDateTime"))
                    lngLastMoveID = tb.Rows(0)("ID")
                Else
                    oLastMoveType = MovementStatus.Indet_
                    lngLastMoveID = -1
                End If

                strSQL = "@SELECT# TOP 1 ID, OutDateTime FROM Moves " &
                         "WHERE IDEmployee = " & intIDEmployee.ToString & " AND OutDateTime IS NOT NULL " &
                         "ORDER BY OutDateTime DESC"
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 AndAlso (CDate(tb.Rows(0)("OutDateTime")) > xLastMoveDateTime OrElse oLastMoveType = MovementStatus.Indet_) Then
                    oLastMoveType = MovementStatus.Out_
                    xLastMoveDateTime = CDate(tb.Rows(0)("OutDateTime"))
                    lngLastMoveID = tb.Rows(0)("ID")
                End If

                If lngLastMoveID > 0 Then
                    oMove = New Move.roMove(intIDEmployee, lngLastMoveID, oMoveState)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetLastMove")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetLastMove")
            End Try

            Return oMove

        End Function

        Public Shared Function GetLastPunchPres(ByVal intIDEmployee As Integer, ByRef oState As Employee.roEmployeeState) As Punch.roPunch

            Dim oPunch As Punch.roPunch = Nothing
            Dim oPunchState As New Punch.roPunchState(oState.IDPassport)

            Dim lngLastPunchID As Long = 0
            Dim oLastPunchType As PunchStatus
            Dim xLastPunchDateTime As DateTime

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# TOP 1 ID, DateTime FROM Punches " &
                                       "WHERE IDEmployee = " & intIDEmployee.ToString & " AND ActualType= " & PunchTypeEnum._IN &
                                       "ORDER BY DateTime DESC, ID DESC"
                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oLastPunchType = PunchStatus.In_
                    xLastPunchDateTime = CDate(tb.Rows(0)("DateTime"))
                    lngLastPunchID = tb.Rows(0)("ID")
                Else
                    oLastPunchType = PunchStatus.Indet_
                    lngLastPunchID = -1
                End If

                strSQL = "@SELECT# TOP 1 ID, DateTime FROM Punches " &
                         "WHERE IDEmployee = " & intIDEmployee.ToString & " AND ActualType= " & PunchTypeEnum._OUT &
                         "ORDER BY DateTime DESC, ID DESC"
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 AndAlso (CDate(tb.Rows(0)("DateTime")) > xLastPunchDateTime OrElse oLastPunchType = PunchStatus.Indet_) Then
                    oLastPunchType = PunchStatus.Out_
                    xLastPunchDateTime = CDate(tb.Rows(0)("DateTime"))
                    lngLastPunchID = tb.Rows(0)("ID")
                End If

                If lngLastPunchID > 0 Then
                    oPunch = New Punch.roPunch(intIDEmployee, lngLastPunchID, oPunchState)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetLastPunchPres")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetLastPunchPres")
            End Try

            Return oPunch

        End Function

        ''' <summary>
        ''' Reordenar movimientos por empleado / dia (solo consulta)
        ''' </summary>
        ''' <param name="intIDEmployee">ID de Empleado</param>
        ''' <param name="xBegin">Fecha Inicio</param>
        ''' <param name="oState">Estado de la funcion</param>
        ''' <returns>DataTable</returns>
        ''' <remarks></remarks>
        Public Shared Function ReorderMovesPreview(ByVal intIDEmployee As Integer, ByVal xBegin As Date, ByRef oState As Employee.roEmployeeState) As DataTable

            Dim tbMoves As DataTable = Nothing 'Taula de Moviments per dia
            Dim tbTempMoves As DataTable 'Taula temporal on es possaran barrejats els moviments
            Dim tbMovesOrdered As New DataTable  'Taula de resultat

            Try

                Dim strSQL As String = "@SELECT# * FROM Moves " &
                          "WHERE Moves.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                    "Moves.ShiftDate = " & Any2Time(xBegin.Date).SQLSmallDateTime

                tbMoves = CreateDataTable(strSQL, "Moves")

                'Clonem l'estructura de la taula i afegim 2 camps per guardar els valors
                tbTempMoves = tbMoves.Clone
                tbMovesOrdered = tbMoves.Clone
                tbTempMoves.Columns.Add("Data_Temp", GetType(Date))
                tbTempMoves.Columns.Add("INOUT_Temp", GetType(String))

                Dim dNewRow As DataRow
                'Busquem les columnes necessaries
                For Each dRow As DataRow In tbMoves.Rows
                    If dRow("InDateTime").ToString.Length > 0 AndAlso dRow("InDateTime").ToString <> "  :  " Then
                        dNewRow = tbTempMoves.NewRow
                        For n As Integer = 0 To tbMoves.Columns.Count - 1
                            dNewRow(n) = dRow(n)
                        Next
                        dNewRow("Data_Temp") = dRow("InDateTime")
                        dNewRow("INOUT_Temp") = "IN"
                        tbTempMoves.Rows.Add(dNewRow)
                    End If
                    If dRow("OutDateTime").ToString.Length > 0 AndAlso dRow("OutDateTime").ToString <> "  :  " Then
                        dNewRow = tbTempMoves.NewRow
                        For n As Integer = 0 To tbMoves.Columns.Count - 1
                            dNewRow(n) = dRow(n)
                        Next
                        dNewRow("Data_Temp") = dRow("OutDateTime")
                        dNewRow("INOUT_Temp") = "OUT"
                        tbTempMoves.Rows.Add(dNewRow)
                    End If
                Next

                Dim dRowsOrdered() As DataRow = tbTempMoves.Select("", "Data_Temp")
                Dim dNewRowOrd As DataRow
                Dim hasSave As Boolean = False ' Es te de grabar perque toca OUT (SI/NO)

                dNewRowOrd = tbMovesOrdered.NewRow

                'Recreem la taula de Moviments reorganitzada
                For n As Integer = 0 To dRowsOrdered.Length - 1
                    dNewRowOrd("IDEmployee") = dRowsOrdered(n)("IDEmployee")
                    dNewRowOrd("ShiftDate") = dRowsOrdered(n)("ShiftDate")
                    If Not hasSave Then  'IN i no graba el registre, espera al següent
                        dNewRowOrd("InDateTime") = dRowsOrdered(n)("Data_Temp")
                        If dRowsOrdered(n)("INOUT_Temp") = "IN" Then
                            dNewRowOrd("InIDReader") = dRowsOrdered(n)("InIDReader")
                            dNewRowOrd("InIDCause") = dRowsOrdered(n)("InIDCause")
                            dNewRowOrd("InIDZone") = dRowsOrdered(n)("InIDZone")
                            dNewRowOrd("InIDReaderType") = dRowsOrdered(n)("InIDReaderType")
                            dNewRowOrd("IsNotReliableIN") = dRowsOrdered(n)("IsNotReliableIN")
                        Else 'INOUT_Temp = "OUT"
                            dNewRowOrd("InIDReader") = dRowsOrdered(n)("OutIDReader")
                            dNewRowOrd("InIDCause") = dRowsOrdered(n)("OutIDCause")
                            dNewRowOrd("InIDZone") = dRowsOrdered(n)("OutIDZone")
                            dNewRowOrd("InIDReaderType") = dRowsOrdered(n)("OutIDReaderType")
                            dNewRowOrd("IsNotReliableIN") = dRowsOrdered(n)("IsNotReliableOUT")
                        End If
                        hasSave = True
                    Else 'OUT i graba el registre
                        dNewRowOrd("OutDateTime") = dRowsOrdered(n)("Data_Temp")
                        If dRowsOrdered(n)("INOUT_Temp") = "IN" Then
                            dNewRowOrd("OutIDReader") = dRowsOrdered(n)("InIDReader")
                            dNewRowOrd("OutIDCause") = dRowsOrdered(n)("InIDCause")
                            dNewRowOrd("OutIDZone") = dRowsOrdered(n)("InIDZone")
                            dNewRowOrd("OutIDReaderType") = dRowsOrdered(n)("InIDReaderType")
                            dNewRowOrd("IsNotReliableOUT") = dRowsOrdered(n)("IsNotReliableIN")
                        Else 'INOUT_Temp = "OUT"
                            dNewRowOrd("OutIDReader") = dRowsOrdered(n)("OutIDReader")
                            dNewRowOrd("OutIDCause") = dRowsOrdered(n)("OutIDCause")
                            dNewRowOrd("OutIDZone") = dRowsOrdered(n)("OutIDZone")
                            dNewRowOrd("OutIDReaderType") = dRowsOrdered(n)("OutIDReaderType")
                            dNewRowOrd("IsNotReliableOUT") = dRowsOrdered(n)("IsNotReliableOUT")
                        End If
                        tbMovesOrdered.Rows.Add(dNewRowOrd)
                        dNewRowOrd = tbMovesOrdered.NewRow 'Nou Row, començem de nou
                        hasSave = False
                    End If
                Next

                'Si no s'ha grabat l'ultim registre, el grabem
                If hasSave Then
                    tbMovesOrdered.Rows.Add(dNewRowOrd)
                End If

                Return tbMovesOrdered
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::ReorderMovesPreview")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::ReorderMovesPreview")
            End Try

            Return tbMoves

        End Function

        ''' <summary>
        ''' Reordenar movimientos por empleado / dia (actualización)
        ''' </summary>
        ''' <param name="intIDEmployee">ID de Empleado</param>
        ''' <param name="xBegin">Fecha Inicio</param>
        ''' <param name="oState">Estado de la funcion</param>
        ''' <returns>True / False si graba correctament</returns>
        ''' <remarks></remarks>
        Public Shared Function ReorderMoves(ByVal intIDEmployee As Integer, ByVal xBegin As Date, ByRef oState As Employee.roEmployeeState) As Boolean
            Dim lRet As Boolean = True
            Dim tbMoves As DataTable = Nothing 'Taula de Moviments per dia
            Dim tbTempMoves As DataTable 'Taula temporal on es possaran barrejats els moviments
            Dim tbMovesOrdered As New DataTable  'Taula de resultat

            Try

                Dim strSQL As String
                strSQL = "@SELECT# Moves.*, MovesCaptures.InCapture, MovesCaptures.OutCapture "
                strSQL &= "FROM Moves LEFT JOIN MovesCaptures " &
                                    "ON Moves.[ID] = MovesCaptures.IDMove " &
                          "WHERE Moves.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                    "Moves.ShiftDate = " & Any2Time(xBegin.Date).SQLSmallDateTime & " " &
                             "ORDER BY ISNULL(Moves.InDateTime, Moves.OutDateTime)"

                tbMoves = CreateDataTable(strSQL, "Moves")

                'Clonem l'estructura de la taula i afegim 2 camps per guardar els valors
                tbTempMoves = tbMoves.Clone
                tbMovesOrdered = tbMoves.Clone
                tbTempMoves.Columns.Add("Data_Temp", GetType(Date))
                tbTempMoves.Columns.Add("INOUT_Temp", GetType(String))

                Dim dNewRow As DataRow
                'Busquem les columnes necessaries
                For Each dRow As DataRow In tbMoves.Rows
                    If dRow("InDateTime").ToString.Length > 0 AndAlso dRow("InDateTime").ToString <> "  :  " Then
                        dNewRow = tbTempMoves.NewRow
                        For n As Integer = 0 To tbMoves.Columns.Count - 1
                            dNewRow(n) = dRow(n)
                        Next
                        dNewRow("Data_Temp") = dRow("InDateTime")
                        dNewRow("INOUT_Temp") = "IN"
                        tbTempMoves.Rows.Add(dNewRow)
                    End If
                    If dRow("OutDateTime").ToString.Length > 0 AndAlso dRow("OutDateTime").ToString <> "  :  " Then
                        dNewRow = tbTempMoves.NewRow
                        For n As Integer = 0 To tbMoves.Columns.Count - 1
                            dNewRow(n) = dRow(n)
                        Next
                        dNewRow("Data_Temp") = dRow("OutDateTime")
                        dNewRow("INOUT_Temp") = "OUT"
                        tbTempMoves.Rows.Add(dNewRow)
                    End If
                Next

                Dim dRowsOrdered() As DataRow = tbTempMoves.Select("", "Data_Temp")
                Dim dNewRowOrd As DataRow
                Dim hasSave As Boolean = False ' Es te de grabar perque toca OUT (SI/NO)

                dNewRowOrd = tbMovesOrdered.NewRow

                'Recreem la taula de Moviments reorganitzada
                For n As Integer = 0 To dRowsOrdered.Length - 1
                    dNewRowOrd("IDEmployee") = dRowsOrdered(n)("IDEmployee")
                    dNewRowOrd("ShiftDate") = dRowsOrdered(n)("ShiftDate")
                    dNewRowOrd("ID") = 0
                    If Not hasSave Then  'IN i no graba el registre, espera al següent
                        dNewRowOrd("InDateTime") = dRowsOrdered(n)("Data_Temp")
                        If dRowsOrdered(n)("INOUT_Temp") = "IN" Then
                            dNewRowOrd("InIDReader") = dRowsOrdered(n)("InIDReader")
                            dNewRowOrd("InIDCause") = dRowsOrdered(n)("InIDCause")
                            dNewRowOrd("InIDZone") = dRowsOrdered(n)("InIDZone")
                            dNewRowOrd("InIDReaderType") = dRowsOrdered(n)("InIDReaderType")
                            dNewRowOrd("IsNotReliableIN") = dRowsOrdered(n)("IsNotReliableIN")
                        Else 'INOUT_Temp = "OUT"
                            dNewRowOrd("InIDReader") = dRowsOrdered(n)("OutIDReader")
                            dNewRowOrd("InIDCause") = dRowsOrdered(n)("OutIDCause")
                            dNewRowOrd("InIDZone") = dRowsOrdered(n)("OutIDZone")
                            dNewRowOrd("InIDReaderType") = dRowsOrdered(n)("OutIDReaderType")
                            dNewRowOrd("IsNotReliableIN") = dRowsOrdered(n)("IsNotReliableOUT")
                        End If
                        hasSave = True
                    Else 'OUT i graba el registre
                        dNewRowOrd("OutDateTime") = dRowsOrdered(n)("Data_Temp")
                        If dRowsOrdered(n)("INOUT_Temp") = "IN" Then
                            dNewRowOrd("OutIDReader") = dRowsOrdered(n)("InIDReader")
                            dNewRowOrd("OutIDCause") = dRowsOrdered(n)("InIDCause")
                            dNewRowOrd("OutIDZone") = dRowsOrdered(n)("InIDZone")
                            dNewRowOrd("OutIDReaderType") = dRowsOrdered(n)("InIDReaderType")
                            dNewRowOrd("IsNotReliableOUT") = dRowsOrdered(n)("IsNotReliableIN")
                        Else 'INOUT_Temp = "OUT"
                            dNewRowOrd("OutIDReader") = dRowsOrdered(n)("OutIDReader")
                            dNewRowOrd("OutIDCause") = dRowsOrdered(n)("OutIDCause")
                            dNewRowOrd("OutIDZone") = dRowsOrdered(n)("OutIDZone")
                            dNewRowOrd("OutIDReaderType") = dRowsOrdered(n)("OutIDReaderType")
                            dNewRowOrd("IsNotReliableOUT") = dRowsOrdered(n)("IsNotReliableOUT")
                        End If
                        tbMovesOrdered.Rows.Add(dNewRowOrd)
                        dNewRowOrd = tbMovesOrdered.NewRow 'Nou Row, començem de nou
                        hasSave = False
                    End If
                Next

                'Si no s'ha grabat l'ultim registre, el grabem
                If hasSave Then
                    tbMovesOrdered.Rows.Add(dNewRowOrd)
                End If

                'Borrem les linies del Moves / MovesCapture
                'Inserim la nova Taula amb Moves/MovesCapture

                Dim sSQL1 As String = "@DELETE# FROM MovesCaptures " _
                & " WHERE IDMove IN (@SELECT# id from moves where IDEmployee= " & intIDEmployee _
                & " AND" _
                & " ShiftDate = " & Any2Time(xBegin.Date).SQLSmallDateTime & ")"

                Dim sSQL2 As String = "@DELETE# FROM Moves " _
                & " WHERE IDEmployee = " & intIDEmployee _
                & " AND" _
                & " ShiftDate = " & Any2Time(xBegin.Date).SQLSmallDateTime

                ExecuteSql(sSQL1)
                ExecuteSql(sSQL2)

                Dim oMoveState As New Move.roMoveState(oState.IDPassport)
                Dim oMove As New Move.roMoveList(oMoveState)

                If oMove.Save(tbMovesOrdered, ) Then
                    lRet = True
                Else
                    lRet = False
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::ReorderMovesPreview")
                lRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::ReorderMovesPreview")
                lRet = False
            End Try

            Return lRet
        End Function

        ''' <summary>
        ''' Reordenar fichajes por empleado / dia (actualización)
        ''' </summary>
        ''' <param name="intIDEmployee">ID de Empleado</param>
        ''' <param name="xBegin">Fecha Inicio</param>
        ''' <param name="oState">Estado de la funcion</param>
        ''' <returns>True / False si graba correctament</returns>
        ''' <remarks></remarks>
        Public Shared Function ReorderPunches(ByVal intIDEmployee As Integer, ByVal xBegin As Date, ByRef oState As roBusinessState) As Boolean

            'TODO: No se pueden reordenar los fichajes así como así, sin tener en cuenta la lógica de punches.

            Dim lRet As Boolean = True

            Return lRet
        End Function

#End Region

    End Class

    Public Class roPunchList

#Region "Declarations - Constructors"

        Private oState As roPunchState

        Private PunchItems As ArrayList

        Public Sub New()

            Me.oState = New roPunchState
            Me.PunchItems = New ArrayList

        End Sub

        Public Sub New(ByVal _State As roPunchState)

            Me.oState = _State
            Me.PunchItems = New ArrayList

        End Sub

#End Region

#Region "Properties"

        <XmlArray("Punches"), XmlArrayItem("roPunch", GetType(roPunch))>
        Public Property Punches() As ArrayList
            Get
                Return Me.PunchItems
            End Get
            Set(ByVal value As ArrayList)
                Me.PunchItems = value
            End Set
        End Property

        Public ReadOnly Property State() As roPunchState
            Get
                Return Me.oState
            End Get
        End Property

#End Region

#Region "Methods"

        Public Function Save(Optional ByVal bolAutomaticBeginJobCheck As Boolean = True, Optional ByVal bolTypeManual As Boolean = False) As Boolean

            Dim bolRet As Boolean = True

            Try
                For Each oPunch As roPunch In Me.Punches
                    Dim customization As String = roBusinessSupport.GetCustomizationCode().ToUpper()
                    If roTypes.Any2String(customization) = "PU" AndAlso oPunch.ID > 0 Then
                        oPunch.Field4 = 1
                    End If

                    bolRet = oPunch.Save(, True, bolAutomaticBeginJobCheck, , , bolTypeManual, False)
                    If Not bolRet Then
                        Exit For
                    End If
                Next
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roPunchList::Save")
            End Try

            Return bolRet

        End Function

        Public Function Save(ByVal tbPunches As DataTable, Optional ByVal bolAutomaticBeginJobCheck As Boolean = True, Optional ByVal bolTypeManual As Boolean = False, Optional ByVal bolInitTask As Boolean = True, Optional ByVal bolUpdateDailyStatus As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try
                Dim oPunch As roPunch
                Dim bolSaved As Boolean = True
                Dim sOldCRC As String = String.Empty

                If tbPunches.Rows.Count > 0 Then

                    Dim customization As String = roBusinessSupport.GetCustomizationCode().ToUpper()

                    For Each oRow As DataRow In tbPunches.Rows
                        bolSaved = True
                        Select Case oRow.RowState
                            Case DataRowState.Added, DataRowState.Modified
                                oPunch = New roPunch(oRow("IDEmployee"), oRow("ID"), Me.oState)
                                With oPunch
                                    .DateTime = IIf(Not IsDBNull(oRow("DateTime")), oRow("DateTime"), Nothing)
                                    .IDTerminal = IIf(Not IsDBNull(oRow("IDTerminal")), oRow("IDTerminal"), -1)
                                    .IDReader = IIf(Not IsDBNull(oRow("IDReader")), oRow("IDReader"), 0)
                                    .IDCredential = IIf(Not IsDBNull(oRow("IDCredential")), oRow("IDCredential"), -1)
                                    .TypeData = IIf(Not IsDBNull(oRow("TypeData")), oRow("TypeData"), -1)
                                    .TypeDetails = IIf(Not IsDBNull(oRow("TypeDetails")), oRow("TypeDetails"), "")
                                    .ShiftDate = oRow("ShiftDate")
                                    .IDZone = IIf(Not IsDBNull(oRow("IDZone")), oRow("IDZone"), -1)
                                    .IsNotReliable = IIf(Not IsDBNull(oRow("IsNotReliable")), oRow("IsNotReliable"), False)
                                    .InTelecommute = IIf(Not IsDBNull(oRow("InTelecommute")), oRow("InTelecommute"), False)
                                    .IDRequest = IIf(Not IsDBNull(oRow("IDRequest")), oRow("IDRequest"), -1)
                                    .Remarks = IIf(Not IsDBNull(oRow("Remarks")), oRow("Remarks"), Nothing)
                                    If tbPunches.Columns.Contains("Capture") AndAlso Not IsDBNull(oRow("Capture")) Then
                                        Dim bImage As Byte() = CType(oRow("Capture"), Byte())
                                        If bImage.Length > 0 Then
                                            Dim ms As MemoryStream = New MemoryStream(bImage)
                                            .Capture = CType(Image.FromStream(ms), Bitmap)
                                        Else
                                            .Capture = Nothing
                                        End If
                                    End If
                                    .Type = IIf(Not IsDBNull(oRow("Type")), oRow("Type"), 0)
                                    If Not IsDBNull(oRow("InvalidType")) Then
                                        .InvalidType = oRow("InvalidType")
                                    Else
                                        .InvalidType = Nothing
                                    End If

                                    If Not IsDBNull(oRow("ActualType")) Then
                                        .ActualType = Any2Integer(oRow("ActualType"))
                                    Else
                                        .ActualType = Nothing
                                    End If
                                    .Location = IIf(Not IsDBNull(oRow("Location")), oRow("Location"), "")
                                    .LocationZone = IIf(Not IsDBNull(oRow("LocationZone")), oRow("LocationZone"), "")

                                    .TimeStamp = Now

                                    .IP = IIf(Not IsDBNull(oRow("IP")), oRow("IP"), "")
                                    .Action = IIf(Not IsDBNull(oRow("Action")), oRow("Action"), -1)
                                    .Passport = IIf(Not IsDBNull(oRow("IDPassport")), oRow("IDPassport"), -1)

                                    If customization = "PU" Then
                                        If oRow.RowState = DataRowState.Modified Then
                                            .Field4 = 1
                                        End If
                                    Else
                                        .Field1 = IIf(Not IsDBNull(oRow("Field1")), oRow("Field1"), "")
                                        .Field2 = IIf(Not IsDBNull(oRow("Field2")), oRow("Field2"), "")
                                        .Field3 = IIf(Not IsDBNull(oRow("Field3")), oRow("Field3"), "")
                                        .Field4 = IIf(Not IsDBNull(oRow("Field4")), oRow("Field4"), 0)
                                        .Field5 = IIf(Not IsDBNull(oRow("Field5")), oRow("Field5"), 0)
                                        .Field6 = IIf(Not IsDBNull(oRow("Field6")), oRow("Field6"), 0)
                                    End If
                                    sOldCRC = oPunch.CRC

                                    .MaskAlert = IIf(Not IsDBNull(oRow("MaskAlert")), oRow("MaskAlert"), Nothing)
                                    .TemperatureAlert = IIf(Not IsDBNull(oRow("TemperatureAlert")), oRow("TemperatureAlert"), Nothing)
                                    .VerificationType = IIf(Not IsDBNull(oRow("VerificationType")), oRow("VerificationType"), VerificationType.AUTOMATIC)
                                    .Source = IIf(Not IsDBNull(oRow("Source")), oRow("Source"), 0)
                                End With
                                bolRet = oPunch.Save(bolInitTask, True, bolAutomaticBeginJobCheck, , , bolTypeManual, False,, bolUpdateDailyStatus)

                                If customization = "LAGRA" AndAlso oRow.RowState = DataRowState.Modified AndAlso sOldCRC <> oPunch.CRC Then
                                    ' ARGAL
                                    Try
                                        Dim sSql As String = String.Empty
                                        sSql = "@UPDATE# DailySchedule WITH (ROWLOCK) SET IsExported=0, IsModified=1  "
                                        sSql = sSql & " WHERE IDEmployee = " & oPunch.IDEmployee.ToString & " AND Date = " & Any2Time(oPunch.ShiftDate).SQLSmallDateTime
                                        bolRet = ExecuteSql(sSql)
                                    Catch ex As Exception
                                        Me.oState.UpdateStateInfo(ex, "roPunch::Save:ARGAL:Exception on new punch")
                                    End Try
                                End If

                            Case DataRowState.Deleted
                                oRow.RejectChanges() ' Cmabiar el estado de la fila para poder leer sus datos
                                oPunch = New roPunch(oRow("IDEmployee"), oRow("ID"), False, Me.oState)
                                Dim dShiftDate As Date = DateTime.MinValue
                                Dim iIDEmployee As Integer = 0
                                dShiftDate = oPunch.ShiftDate
                                iIDEmployee = oPunch.IDEmployee

                                bolRet = oPunch.Delete(bolInitTask, True, , False, bolUpdateDailyStatus)

                                If bolRet AndAlso customization = "PU" Then
                                    Dim sSql As String = "@INSERT# INTO sysroDeletedPunchesSync(PunchID,EmployeeID) VALUES (" & oRow("ID") & "," & oRow("IDEmployee") & ")"
                                    bolRet = ExecuteSql(sSql)
                                End If

                                If bolRet AndAlso customization = "LAGRA" Then
                                    Try
                                        Dim sSql As String = String.Empty
                                        sSql = "@UPDATE# DailySchedule WITH (ROWLOCK) SET IsExported=0, IsModified=1  "
                                        sSql = sSql & " WHERE IDEmployee = " & iIDEmployee.ToString & " AND Date = " & Any2Time(dShiftDate).SQLSmallDateTime
                                        bolRet = ExecuteSql(sSql)
                                    Catch ex As Exception
                                        Me.oState.UpdateStateInfo(ex, "roPunchList::Save:ARGAL:Exception on delete punch")
                                    End Try
                                End If

                            Case Else
                                bolRet = True
                                bolSaved = False

                        End Select

                        If Not bolRet Then
                            Exit For
                        End If
                    Next
                Else

                    bolRet = True

                End If
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roPunchList::Save")
            End Try

            Return bolRet

        End Function

        Public Function LoadData(ByVal ds As DataSet, ByRef oState As roPunchState) As Boolean

            Dim bolRet As Boolean = False

            Try

                If ds.Tables.Contains("Punches") Then

                    Dim tbPunches As DataTable = ds.Tables("Punches")
                    Dim oPunch As roPunch

                    For Each oRow As DataRow In tbPunches.Rows
                        oPunch = New roPunch(oState)
                        With oPunch
                            .IDEmployee = oRow("IDEmployee")
                            .ID = oRow("ID")
                            .Load()
                            ' ...
                        End With
                        Me.Punches.Add(oPunch)
                    Next

                    bolRet = True

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPunchList:LoadData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPunchList:LoadData")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

    Public Class roDailyRecordPunchHelper

        Public Shared Function CheckDailyRecordPunches(lDailyRecordPunches As roDailyRecordPunch(), ByRef CRC As String) As DailyRecordPunchesResultEnum
            Dim oRet As DailyRecordPunchesResultEnum = DailyRecordPunchesResultEnum.NoError
            Try
                ' No se puede hacer una declaración sin fichajes,
                If lDailyRecordPunches.Length = 0 Then
                    Return DailyRecordPunchesResultEnum.PunchesListCantBeEmpty
                End If

                ' Ni tampoco si no está formado por pares completos de fichajes
                If lDailyRecordPunches.Length Mod 2 <> 0 Then
                    Return DailyRecordPunchesResultEnum.PunchesNumberShouldBeEven
                End If

                'Validamos secuencia de fichajes
                Dim lPunchesCheck As New List(Of String)
                Dim oCurrentPunch As roDailyRecordPunch = Nothing
                For i = 0 To lDailyRecordPunches.Length - 1
                    oCurrentPunch = lDailyRecordPunches(i)
                    'Se evitan repetidos
                    If lPunchesCheck.Contains(DailyRecordPunchToString(oCurrentPunch)) Then
                        ' La secuencia no puede contener dos fichajes iguales
                        If oCurrentPunch.Type = PunchTypeEnum._IN Then
                            Return DailyRecordPunchesResultEnum.PunchesOverlaped
                        Else
                            Return DailyRecordPunchesResultEnum.PunchRepeated
                        End If
                    End If
                    lPunchesCheck.Add(DailyRecordPunchToString(oCurrentPunch))
                    'Se valida que el fichaje sea posterior al último
                    If i > 0 AndAlso lDailyRecordPunches(i - 1).DateTime > oCurrentPunch.DateTime Then
                        ' La secuencia no puede contener fichajes desordenados
                        Return DailyRecordPunchesResultEnum.InvalidSequence
                    End If
                    If (i + 1) Mod 2 <> 0 Then
                        ' Posiciones impares. Deben ser Entradas
                        If CRC.Length > 0 Then
                            CRC = CRC & "*"
                        End If
                        CRC = CRC & oCurrentPunch.DateTime.ToString("yyyyMMddHHmmss")

                        If oCurrentPunch.Type <> PunchTypeEnum._IN Then
                            Return DailyRecordPunchesResultEnum.InvalidSequence
                        End If
                    Else
                        ' Posiciones pares. Deben ser Salidas
                        CRC = CRC & "-" & oCurrentPunch.DateTime.ToString("yyyyMMddHHmmss")
                        If oCurrentPunch.Type <> PunchTypeEnum._OUT Then
                            Return DailyRecordPunchesResultEnum.InvalidSequence
                        End If
                    End If

                    If oCurrentPunch.DateTime.Year = 1900 Then
                        'No puede haber más de 1 día de diferencia entre los fichajes
                        Return DailyRecordPunchesResultEnum.InvalidSequence
                    End If
                Next

                CRC = CryptographyHelper.EncryptWithMD5(CRC)
            Catch ex As Exception
                Return DailyRecordPunchesResultEnum.Exception
            End Try

            Return oRet

        End Function

        Public Shared Function DailyRecordPunchToString(oPunch As roDailyRecordPunch) As String
            Try
                Return oPunch.DateTime.ToString("yyyyMMddHHmmss") & "-" & oPunch.ShiftDate.ToString("yyyyMMddHHmmss")
            Catch ex As Exception
                Return "?"
            End Try
        End Function

    End Class

End Namespace