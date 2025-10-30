Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Zone

    <DataContract()>
    Public Class roZone

#Region "Declarations - Constructor"

        Private oState As roZoneState

        Private intID As Integer
        Private strName As String
        Private strNameExtended As String
        Private iCapacity As Integer?
        Private bCapacityVisible As Boolean?
        Private strDescription As String
        Private bolIsWorkingZone As Boolean
        Private bolIsEmergencyZone As Boolean?
        Private bolZoneNameAsLocation As Boolean?
        Private intX1 As Integer
        Private intX2 As Integer
        Private intY1 As Integer
        Private intY2 As Integer
        Private intCurrentPeopleIn As Integer
        Private intCurrentPeopleOut As Integer
        Private strCurrentPeopleOutDesc As String
        Private strCurrentPeopleInDesc As String
        Private strCapacityDesc As String
        Private dblProportion As Double
        Private oZoneGroup As roZoneGroup
        Private intIDPlane As Nullable(Of Integer)
        Private intIDCamera As Nullable(Of Integer)
        Private strDefaultTimezone As String
        Private intTelecommutingZoneType As Integer
        Private oParentZone As roZone

        Private intColor As Integer
        Private oImage As Byte()

        Private oIpsRestriction As List(Of String)
        Private oGoogleMapInfo As roGoogleMapInfo
        Private dblArea As Double
        Private strWorkcenter As String
        Private iIdSupervisor As Integer

        Private oZonesInactivity As Generic.List(Of roZoneInactivity)
        Private oZonesException As Generic.List(Of roZoneException)

        Public Sub New()
            Me.oState = New roZoneState()
            Me.intID = -1
            Me.oZoneGroup = Nothing
            Me.oParentZone = Nothing
            Me.iIdSupervisor = -1
            ReDim Me.oImage(-1)
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roZoneState,
                       Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intID = _ID
            Me.iIdSupervisor = -1
            ReDim Me.oImage(-1)
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roZoneState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roZoneState)
                Me.oState = value
            End Set
        End Property

        <DataMember()>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
                If Me.oZonesException IsNot Nothing Then
                    For Each oException As roZoneException In Me.oZonesException
                        oException.IDZone = Me.intID
                    Next
                End If
                If Me.oZonesInactivity IsNot Nothing Then
                    For Each oInactivity As roZoneInactivity In Me.oZonesInactivity
                        oInactivity.IDZone = Me.intID
                    Next
                End If
            End Set
        End Property
        <DataMember()>
        Public Property NameExtended() As String
            Get
                Return Me.strNameExtended
            End Get
            Set(ByVal value As String)
                Me.strNameExtended = value
            End Set
        End Property
        <DataMember()>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property

        <DataMember()>
        Public Property WorkCenter() As String
            Get
                Return Me.strWorkcenter
            End Get
            Set(ByVal value As String)
                Me.strWorkcenter = value
            End Set
        End Property

        Public Property Supervisor() As Integer
            Get
                Return Me.iIdSupervisor
            End Get
            Set(ByVal value As Integer)
                Me.iIdSupervisor = value
            End Set
        End Property

        <DataMember()>
        Public Property Capacity() As Integer?
            Get
                Return Me.iCapacity
            End Get
            Set(ByVal value As Integer?)
                Me.iCapacity = value
            End Set
        End Property

        <DataMember()>
        Public ReadOnly Property CurrentCapacity() As String
            Get
                If Me.iCapacity IsNot Nothing Then

                    Return GetEmployeesCurrentlyInZone(Me.ID, Me.State).ToString() & "/" & Me.iCapacity.ToString()
                Else
                    Return Nothing
                End If
            End Get
        End Property

        <DataMember()>
        Public ReadOnly Property CurrentCapacityInt() As Integer
            Get
                If Me.iCapacity IsNot Nothing Then

                    Return GetEmployeesCurrentlyInZone(Me.ID, Me.State)
                Else
                    Return -1
                End If
            End Get
        End Property

        <DataMember()>
        Public Property CapacityVisible() As Boolean?
            Get
                Return Me.bCapacityVisible
            End Get
            Set(ByVal value As Boolean?)
                Me.bCapacityVisible = value
            End Set
        End Property

        <DataMember()>
        Public Property Description() As String
            Get
                Return Me.strDescription
            End Get
            Set(ByVal value As String)
                Me.strDescription = value
            End Set
        End Property

        <DataMember()>
        Public Property IsWorkingZone() As Boolean
            Get
                Return Me.bolIsWorkingZone
            End Get
            Set(ByVal value As Boolean)
                Me.bolIsWorkingZone = value
            End Set
        End Property

        <DataMember()>
        Public Property IsEmergencyZone() As Boolean?
            Get
                Return Me.bolIsEmergencyZone
            End Get
            Set(ByVal value As Boolean?)
                Me.bolIsEmergencyZone = value
            End Set
        End Property

        <DataMember()>
        Public Property ZoneNameAsLocation() As Boolean?
            Get
                Return Me.bolZoneNameAsLocation
            End Get
            Set(ByVal value As Boolean?)
                Me.bolZoneNameAsLocation = value
            End Set
        End Property

        <DataMember()>
        Public Property DefaultTimezone() As String
            Get
                Return Me.strDefaultTimezone
            End Get
            Set(ByVal value As String)
                Me.strDefaultTimezone = value
            End Set
        End Property

        <DataMember()>
        Public Property X1() As Integer
            Get
                Return Me.intX1
            End Get
            Set(ByVal value As Integer)
                Me.intX1 = value
            End Set
        End Property

        <DataMember()>
        Public Property X2() As Integer
            Get
                Return Me.intX2
            End Get
            Set(ByVal value As Integer)
                Me.intX2 = value
            End Set
        End Property

        <DataMember()>
        Public Property CapacityDesc() As String
            Get
                Return Me.strCapacityDesc
            End Get
            Set(ByVal value As String)
                Me.strCapacityDesc = value
            End Set
        End Property

        <DataMember()>
        Public Property CurrentPeopleInDesc() As String
            Get
                Return Me.strCurrentPeopleInDesc
            End Get
            Set(ByVal value As String)
                Me.strCurrentPeopleInDesc = value
            End Set
        End Property

        <DataMember()>
        Public Property CurrentPeopleOutDesc() As String
            Get
                Return Me.strCurrentPeopleOutDesc
            End Get
            Set(ByVal value As String)
                Me.strCurrentPeopleOutDesc = value
            End Set
        End Property

        <DataMember()>
        Public Property CurrentPeopleIn() As Integer
            Get
                Return Me.intCurrentPeopleIn
            End Get
            Set(ByVal value As Integer)
                Me.intCurrentPeopleIn = value
            End Set
        End Property

        <DataMember()>
        Public Property CurrentPeopleOut() As Integer
            Get
                Return Me.intCurrentPeopleOut
            End Get
            Set(ByVal value As Integer)
                Me.intCurrentPeopleOut = value
            End Set
        End Property

        <DataMember()>
        Public Property Y1() As Integer
            Get
                Return Me.intY1
            End Get
            Set(ByVal value As Integer)
                Me.intY1 = value
            End Set
        End Property

        <DataMember()>
        Public Property Y2() As Integer
            Get
                Return Me.intY2
            End Get
            Set(ByVal value As Integer)
                Me.intY2 = value
            End Set
        End Property

        <DataMember()>
        Public Property GoogleMapInfo() As roGoogleMapInfo
            Get
                Return Me.oGoogleMapInfo
            End Get
            Set(ByVal value As roGoogleMapInfo)
                Me.oGoogleMapInfo = value
            End Set
        End Property

        <DataMember()>
        Public Property IpsRestriction As List(Of String)
            Get
                Return Me.oIpsRestriction
            End Get
            Set(ByVal value As List(Of String))
                Me.oIpsRestriction = value
            End Set
        End Property


        <DataMember()>
        Public Property Area() As Double
            Get
                Return Me.dblArea
            End Get
            Set(ByVal value As Double)
                Me.dblArea = value
            End Set
        End Property

        <DataMember()>
        Public Property Proportion() As Double
            Get
                Return Me.dblProportion
            End Get
            Set(ByVal value As Double)
                Me.dblProportion = value
            End Set
        End Property

        <DataMember()>
        Public Property ZoneGroup() As roZoneGroup
            Get
                Return Me.oZoneGroup
            End Get
            Set(ByVal value As roZoneGroup)
                Me.oZoneGroup = value
            End Set
        End Property

        <DataMember()>
        Public Property ParentZone() As roZone
            Get
                Return Me.oParentZone
            End Get
            Set(ByVal value As roZone)
                Me.oParentZone = value
            End Set
        End Property

        <DataMember()>
        Public Property Color() As Integer
            Get
                Return intColor
            End Get
            Set(ByVal value As Integer)
                intColor = value
            End Set
        End Property

        <DataMember()>
        Public Property Image As Byte()
            Get
                Return Me.oImage
            End Get
            Set(ByVal value As Byte())
                Me.oImage = value
            End Set
        End Property

        <DataMember()>
        Public Property IDPlane() As Nullable(Of Integer)
            Get
                Return intIDPlane
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDPlane = value
            End Set
        End Property

        <DataMember()>
        Public Property IDCamera() As Nullable(Of Integer)
            Get
                Return intIDCamera
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDCamera = value
            End Set
        End Property

        <DataMember()>
        Public Property TelecommutingZoneType() As Integer
            Get
                Return Me.intTelecommutingZoneType
            End Get
            Set(ByVal value As Integer)
                Me.intTelecommutingZoneType = value
            End Set
        End Property

        <DataMember()>
        Public Property ZonesInactivity() As Generic.List(Of roZoneInactivity)
            Get
                Return Me.oZonesInactivity
            End Get
            Set(ByVal value As Generic.List(Of roZoneInactivity))
                Me.oZonesInactivity = value
            End Set
        End Property

        <DataMember()>
        Public Property ZonesException() As Generic.List(Of roZoneException)
            Get
                Return Me.oZonesException
            End Get
            Set(ByVal value As Generic.List(Of roZoneException))
                Me.oZonesException = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM Zones " &
                                       "WHERE [ID] = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then Me.strName = oRow("Name")
                    If Me.ID = 255 Then
                        Dim oLng = New roLanguage()
                        If State IsNot Nothing Then
                            oLng.SetLanguageReference("ZonesService", State.Language.LanguageKey)
                        Else
                            oLng.SetLanguageReference("ZonesService", "ESP")
                        End If
                        Me.Name = oLng.Translate("Zones.WorldZone", "")
                    End If
                    If Not IsDBNull(oRow("Capacity")) Then Me.iCapacity = oRow("Capacity")
                    If Not IsDBNull(oRow("CapacityVisible")) Then Me.bCapacityVisible = oRow("CapacityVisible")
                    If Not IsDBNull(oRow("Description")) Then Me.strDescription = oRow("Description")
                    Me.bolIsWorkingZone = oRow("IsWorkingZone")
                    If Not IsDBNull(oRow("IsEmergencyZone")) Then Me.bolIsEmergencyZone = oRow("IsEmergencyZone")
                    If Not IsDBNull(oRow("ZoneNameAsLocation")) Then Me.bolZoneNameAsLocation = oRow("ZoneNameAsLocation")
                    If Not IsDBNull(oRow("X1")) Then Me.intX1 = oRow("X1")
                    If Not IsDBNull(oRow("X2")) Then Me.intX2 = oRow("X2")
                    If Not IsDBNull(oRow("Y1")) Then Me.intY1 = oRow("Y1")
                    If Not IsDBNull(oRow("Y2")) Then Me.intY2 = oRow("Y2")
                    If Not IsDBNull(oRow("Proportion")) Then Me.dblProportion = oRow("Proportion")
                    If Not IsDBNull(oRow("IDZoneGroup")) Then Me.oZoneGroup = New roZoneGroup(oRow("IDZoneGroup"), Me.oState)
                    If Not IsDBNull(oRow("TimeZone")) Then
                        Me.strDefaultTimezone = oRow("TimeZone")
                    Else
                        Me.strDefaultTimezone = ""
                    End If
                    If Not IsDBNull(oRow("Workcenter")) Then Me.strWorkcenter = oRow("Workcenter")
                    If Not IsDBNull(oRow("IdZoneSupervisor")) Then Me.iIdSupervisor = oRow("IdZoneSupervisor")

                    If Not IsDBNull(oRow("IDPlane")) Then Me.intIDPlane = oRow("IDPlane")
                    If Not IsDBNull(oRow("IDCamera")) Then Me.intIDCamera = oRow("IDCamera")

                    If Not IsDBNull(oRow("IDParent")) Then
                        Me.oParentZone = Nothing
                        Dim idParentZone As Integer = roTypes.Any2Integer(oRow("IDParent"))
                        If Me.ID = idParentZone Then idParentZone = GetRootZone(Me.oState)
                        If idParentZone > 0 AndAlso Me.ID <> idParentZone Then Me.oParentZone = New roZone(idParentZone, Me.oState, False)

                        If Me.oParentZone Is Nothing Then
                            roLog.GetInstance().logMessage(roLog.EventType.roInfo, "roZone::Save::The zone " & Me.ID & " is parent of itself. The parent zone is set to null.")
                        End If
                    Else
                        Me.oParentZone = Nothing
                    End If

                    If Not IsDBNull(oRow("IDZoneType")) Then
                        Me.intTelecommutingZoneType = oRow("IDZoneType")
                    Else
                        Me.intTelecommutingZoneType = ZoneTelecommutingType.AskUser
                    End If

                    If Not IsDBNull(oRow("Color")) Then Me.intColor = oRow("Color")

                    If Not IsDBNull(oRow("ZoneImage")) Then
                        Dim bits As Byte() = CType(oRow("ZoneImage"), Byte())
                        Me.oImage = bits
                    Else
                        Me.oImage = Nothing
                    End If

                    If Not IsDBNull(oRow("MapInfo")) Then Me.GoogleMapInfo = JsonConvert.DeserializeObject(Of roGoogleMapInfo)(oRow("MapInfo"))
                    If Not IsDBNull(oRow("Area")) Then Me.Area = oRow("Area")

                    Me.IpsRestriction = New List(Of String)
                    If Not IsDBNull(oRow("IpsRestriction")) AndAlso roTypes.Any2String(oRow("IpsRestriction")) <> String.Empty Then Me.IpsRestriction = roTypes.Any2String(oRow("IpsRestriction")).Split("@").ToList()


                    'Carrega de ZonesException i ZonesInactivity
                    Me.oZonesException = roZoneException.GetZonesExceptionList(oRow("ID"), oState)
                    Me.oZonesInactivity = roZoneInactivity.GetZonesInactivityList(oRow("ID"), oState)
                Else
                    Me.intX1 = -1
                    Me.intX2 = -1
                    Me.intY1 = -1
                    Me.intY2 = -1
                    Me.oGoogleMapInfo = Nothing
                    Me.IpsRestriction = New List(Of String)
                    Me.oZoneGroup = Nothing
                    Me.oParentZone = Nothing
                    Me.oZonesException = Nothing
                    Me.oZonesInactivity = Nothing
                End If

                bolRet = True

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tAccessZone, Me.strName, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roZone::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roZone::Load")
            End Try

            Return bolRet

        End Function

        Public Function GetZoneByName(ByVal ZoneName As String, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# id FROM Zones " &
                                       "WHERE [Name] = '" & ZoneName & "'"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    Me.ID = oRow("id")
                    bolRet = Me.Load()

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tAccessZone, Me.strName, tbParameters, -1)
                    End If
                Else
                    bolRet = True
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roZone::GetZoneByName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roZone::GetZoneByName")
            End Try

            Return bolRet
        End Function

        Public Shared Function GetZoneNameByID(id As Integer) As String
            Dim oRet As String = "?"
            Try
                Dim strSQL As String = "@SELECT# Name FROM Zones WHERE ID = " & id.ToString
                oRet = roTypes.Any2String(ExecuteScalar(strSQL))
            Catch ex As Exception
                ' No hacemos nada
            End Try
            Return oRet
        End Function

        Public Function ValidateZone() As Boolean

            Dim strQuery As String
            Dim oDataSet As System.Data.DataSet

            ' El nombre no puede estar en blanco
            If Me.Name = "" Then
                oState.Result = DTOs.ZoneResultEnum.InvalidName
                Return False
            End If

            ' El nombre no puede existir en la bdd para otra justificación
            strQuery = " @SELECT# * From Zones "
            strQuery = strQuery & " Where id <> " & Me.ID
            strQuery = strQuery & " And name = '" & Me.Name.Replace("'", "''") & "' "

            oDataSet = CreateDataSet(strQuery)
            If oDataSet.CreateDataReader.HasRows Then
                ' Si la select me ha devuelto es que alguien tiene el nombre
                oState.Result = DTOs.ZoneResultEnum.NameAlreadyExist
                Return False
            End If

            'Buscamos si hay algún día de la semana coincidente
            If VerifyOverLaps() Then
                oState.Result = DTOs.ZoneResultEnum.OverlapedInactiviy
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Busca si hay algún día de la semana coincidente.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function VerifyOverLaps() As Boolean
            Dim bolRet As Boolean = False

            Dim oZI As roZoneInactivity
            Dim oZISearch As roZoneInactivity
            Try
                If oZonesInactivity Is Nothing Then Return False

                For nZI As Integer = 0 To Me.oZonesInactivity.Count - 1
                    oZI = Me.oZonesInactivity(nZI)
                    For nZI2 As Integer = 0 To Me.oZonesInactivity.Count - 1
                        If nZI = nZI2 Then Continue For
                        oZISearch = Me.oZonesInactivity(nZI2)

                        If oZI.WeekDay = oZISearch.WeekDay AndAlso
                            ((oZI.Begin >= oZISearch.Begin AndAlso oZI.Begin < oZISearch.End) OrElse
                               (oZI.End > oZISearch.Begin AndAlso oZI.End < oZISearch.End) OrElse
                               (oZISearch.Begin >= oZI.Begin AndAlso oZISearch.Begin < oZI.End) OrElse
                               (oZISearch.End > oZI.Begin AndAlso oZISearch.End < oZI.End)) Then
                            Return True
                        End If
                    Next
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roZone::VerifyOverLaps")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.State.Result = ZoneResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If ValidateZone() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim bolIsNew As Boolean = False

                    Dim tb As New DataTable("Zones")
                    Dim strSQL As String = "@SELECT# * FROM Zones WHERE ID = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        Me.ID = Me.GetNextID()
                        oRow = tb.NewRow
                        oRow("ID") = Me.ID
                        bolIsNew = True
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Name") = Me.strName
                    If (Me.iCapacity.HasValue) Then
                        oRow("Capacity") = Me.iCapacity
                    Else
                        oRow("Capacity") = System.DBNull.Value
                    End If
                    If (Me.bCapacityVisible.HasValue) Then
                        oRow("CapacityVisible") = Me.bCapacityVisible
                    Else
                        oRow("CapacityVisible") = System.DBNull.Value
                    End If
                    If (Me.bolZoneNameAsLocation.HasValue) Then
                        oRow("ZoneNameAsLocation") = Me.bolZoneNameAsLocation
                    Else
                        oRow("ZoneNameAsLocation") = System.DBNull.Value
                    End If
                    If (Me.bolIsEmergencyZone.HasValue) Then
                        oRow("IsEmergencyZone") = Me.bolIsEmergencyZone
                    Else
                        oRow("IsEmergencyZone") = System.DBNull.Value
                    End If
                    oRow("Description") = Me.strDescription
                    oRow("IsWorkingZone") = Me.bolIsWorkingZone
                    oRow("X1") = Me.intX1
                    oRow("Y1") = Me.intY1
                    oRow("X2") = Me.intX2
                    oRow("Y2") = Me.intY2
                    oRow("MapInfo") = JsonConvert.SerializeObject(Me.oGoogleMapInfo)
                    oRow("IDZoneType") = Me.intTelecommutingZoneType

                    ' Areas rectangulares (1.- vértice superior derecho 2.- vértice inferior izquierdo
                    If oGoogleMapInfo IsNot Nothing Then
                        If oGoogleMapInfo.Coordinates.Count = 2 Then
                            Me.dblArea = (oGoogleMapInfo.Coordinates(0).Latitud - oGoogleMapInfo.Coordinates(1).Latitud) * (oGoogleMapInfo.Coordinates(0).Longitud - oGoogleMapInfo.Coordinates(1).Longitud) * 1000000
                        Else
                            ' Error
                            Me.dblArea = -1
                        End If
                    Else
                        ' Error
                        Me.dblArea = -1
                    End If

                    oRow("Area") = Me.dblArea
                    oRow("Proportion") = Me.dblProportion

                    If Me.IpsRestriction IsNot Nothing AndAlso Me.IpsRestriction.Any() Then
                        oRow("IpsRestriction") = String.Join("@", Me.IpsRestriction)
                    Else
                        oRow("IpsRestriction") = DBNull.Value
                    End If

                    If intIDPlane.HasValue Then
                        oRow("IDPlane") = Me.intIDPlane
                    Else
                        oRow("IDPlane") = System.DBNull.Value
                    End If

                    If intIDCamera.HasValue Then
                        oRow("IDCamera") = Me.intIDCamera
                    Else
                        oRow("IDCamera") = System.DBNull.Value
                    End If

                    If oZoneGroup IsNot Nothing Then
                        oRow("IDZoneGroup") = Me.oZoneGroup.ID
                    Else
                        oRow("IDZoneGroup") = System.DBNull.Value
                    End If

                    If Me.oParentZone IsNot Nothing Then
                        oRow("IDParent") = Me.oParentZone.ID

                        If Me.oParentZone.ID = Me.ID Then
                            oRow("IDParent") = GetRootZone(State)
                        End If
                    Else
                        oRow("IDParent") = GetRootZone(State)
                    End If

                    If roTypes.Any2Integer(oRow("IDParent")) = Me.ID Then
                        oRow("IDParent") = DBNull.Value
                        roLog.GetInstance().logMessage(roLog.EventType.roInfo, "roZone::Save::The zone " & Me.ID & " is parent of itself. The parent zone is set to null.")
                    End If

                    oRow("Color") = Me.intColor
                    If Me.oImage IsNot Nothing AndAlso Me.oImage.Length > 0 Then
                        oRow("ZoneImage") = Me.oImage
                    Else
                        oRow("ZoneImage") = DBNull.Value
                    End If

                    oRow("TimeZone") = Me.strDefaultTimezone

                    oRow("Workcenter") = Me.strWorkcenter
                    oRow("IdZoneSupervisor") = Me.iIdSupervisor

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    'Gravem les ZonesException
                    bolRet = roZoneException.SaveZoneExceptions(Me.ID, Me.oZonesException, Me.oState)

                    If bolRet Then
                        'Gravem les ZonesInactivity
                        bolRet = roZoneInactivity.SaveZoneInactivities(Me.ID, Me.oZonesInactivity, Me.oState)
                    End If

                    oAuditDataNew = oRow

                    If bolRet AndAlso bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tAccessZone, strObjectName, tbAuditParameters, -1)
                    End If

                    If bolRet Then
                        ' Notificamos al servidor que tiene que lanzar el broadcaster
                        ' ** Queda pendiente informar los IDs de los terminales a regenerar. Actualmente regenera los ficheros para todos los terminales tipo mx6
                        roConnector.InitTask(TasksType.BROADCASTER)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roZone::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roZone::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = True

            Try

                'Comprovem si existeix algun terminal amb la zona associada
                Dim dTblTermReader As DataTable = CreateDataTable("@SELECT# * from TerminalReaders Where IDZone = " & Me.ID)
                If dTblTermReader.Rows.Count > 0 Then
                    oState.Result = DTOs.ZoneResultEnum.ZoneInTerminalReaders
                    bolRet = False
                End If

                If bolRet Then
                    'Comprovem si existeix algun periode amb la zona
                    Dim dTblPeriod As DataTable = CreateDataTable("@SELECT# * from AccessGroupsPermissions Where IDZone = " & Me.ID)
                    If dTblPeriod.Rows.Count > 0 Then
                        oState.Result = DTOs.ZoneResultEnum.ZoneInGroupPermissions
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    'Comprovem si existeix algun centro de coste con la zona
                    Dim dTblPeriod As DataTable = CreateDataTable("@SELECT# * from BusinessCenterZones Where IDZone = " & Me.ID)
                    If dTblPeriod.Rows.Count > 0 Then
                        oState.Result = DTOs.ZoneResultEnum.ZoneInBusinessCenter
                        bolRet = False
                    End If
                End If

                If bolRet Then

                    Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()

                    Dim DeleteQuerys As String() = {"@DELETE# FROM ZonesInactivity WHERE IDZone = " & Me.intID.ToString,
                                                    "@DELETE# FROM ZonesException WHERE IDZone = " & Me.intID.ToString,
                                                    "@DELETE# FROM Zones WHERE ID = " & Me.intID.ToString}

                    For Each strSQL As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQL)
                        If Not bolRet Then Exit For
                    Next

                    Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAccessZone, Me.strName, Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roZone::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roZone::Delete")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Recupera el siguiente codigo zona a usar
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextID() As Integer
            Dim intRet As Integer = 0

            Try

                Dim strSQL As String = "@SELECT# Max(ID) AS Contador FROM Zones WHERE ID <> 255 "
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 AndAlso Not IsDBNull(tb.Rows(0).Item(0)) Then
                    intRet = tb.Rows(0).Item(0)
                End If

                intRet += 1
            Catch ex As Data.Common.DbException
                Me.oState.UpdateStateInfo(ex, "roZone::GetNextID")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roZone::GetNextID")
            End Try

            Return intRet

        End Function

#Region "Helper methods"

        Public Shared Function GetZonesList(ByVal strOrderBy As String, ByVal state As roZoneState,
                                            Optional ByVal bAudit As Boolean = False) As Generic.List(Of roZone)

            Dim oRet As New Generic.List(Of roZone)

            Try

                Dim strSQL As String = "@SELECT# * FROM Zones ORDER BY "
                If strOrderBy <> "" Then
                    strSQL &= strOrderBy
                Else
                    strSQL &= "Name ASC"
                End If

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oZone As roZone = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oZone = New roZone(oRow("ID"), state)
                        If oZone.ID = 255 Then oZone.Name = GetWorldZoneName(state)
                        oRet.Add(oZone)
                    Next

                End If
            Catch ex As Data.Common.DbException
                If state IsNot Nothing Then state.UpdateStateInfo(ex, "roZone::GetZonesList")
            Catch ex As Exception
                If state IsNot Nothing Then state.UpdateStateInfo(ex, "roZone::GetZonesList")
            End Try

            Return oRet

        End Function

        Private Shared Function GetWorldZoneName(state As roZoneState) As String
            Dim oLng = New roLanguage()
            If state IsNot Nothing Then
                oLng.SetLanguageReference("ZonesService", state.Language.LanguageKey)
            Else
                oLng.SetLanguageReference("ZonesService", "ESP")
            End If
            Return oLng.Translate("Zones.WorldZone", "")
        End Function

        Public Shared Function GetZonesDataTable(ByVal strOrderBy As String, ByVal state As roZoneState, Optional idPassport As Integer = 0) As DataTable

            Dim tbRet As DataTable = Nothing

            Dim bolByAuth = False
            Dim idParentPassport = String.Empty

            Try

                If (Not idPassport.Equals(0)) Then
                    idParentPassport = roPassportManager.GetPassportTicket(idPassport).IDParentPassport.ToString()
                    bolByAuth = True
                End If

                Dim strSQL As String = "@SELECT# * FROM Zones "
                If (bolByAuth) Then
                    strSQL &= "where ID in (@SELECT# IDZone
                              FROM AccessGroupsPermissions
                              where IDAccessGroup in (@SELECT# IDAccessGroup from sysroPassports_AccessGroup where idpassport = " & idParentPassport & ")) or IDParent is null "
                End If

                strSQL &= "ORDER BY "
                If strOrderBy <> "" Then
                    strSQL &= strOrderBy
                Else
                    strSQL &= "Name ASC"
                End If

                tbRet = CreateDataTable(strSQL, )

                If tbRet IsNot Nothing AndAlso tbRet.Rows.Count > 0 Then
                    Dim worldZoneRow() As Data.DataRow = tbRet.Select("ID = 255")
                    If worldZoneRow IsNot Nothing AndAlso worldZoneRow.Length > 0 Then worldZoneRow(0)("Name") = GetWorldZoneName(state)
                End If
            Catch ex As Data.Common.DbException
                If state IsNot Nothing Then state.UpdateStateInfo(ex, "roZone::GetZonesDataTable")
            Catch ex As Exception
                If state IsNot Nothing Then state.UpdateStateInfo(ex, "roZone::GetZonesDataTable")
            End Try

            Return tbRet

        End Function

        Public Shared Function GetIDZoneFromReader(ByVal intIDTerminal As Integer, ByVal intIDReader As Integer, ByVal state As roZoneState) As Integer

            Dim intRet As Integer = 0

            Try

                Dim strSQL As String
                strSQL = "@SELECT# IDZone FROM TerminalReaders WHERE IDTerminal = " & intIDTerminal.ToString & " AND ID = " & intIDReader.ToString

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 AndAlso Not IsDBNull(tb.Rows(0).Item("IDZone")) Then
                    intRet = tb.Rows(0).Item("IDZone")
                End If
            Catch ex As DbException
                state.UpdateStateInfo(ex, "roZone::GetIDZoneFromReader")
            Catch ex As Exception
                state.UpdateStateInfo(ex, "roZone::GetIDZoneFromReader")
            End Try

            Return intRet

        End Function

        Public Shared Function GetIsWorkingZone(ByVal intIDZone As Integer, ByVal state As roZoneState) As Boolean

            Dim bolRet As Integer = False

            Try

                Dim strSQL As String
                strSQL = "@SELECT# IsWorkingZone FROM Zones WHERE ID = " & intIDZone.ToString

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    bolRet = tb.Rows(0).Item("IsWorkingZone")

                End If
            Catch ex As DbException
                state.UpdateStateInfo(ex, "roZone::GetIsWorkingZone")
            Catch ex As Exception
                state.UpdateStateInfo(ex, "roZone::GetIsWorkingZone")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetRootZone(ByVal state As roZoneState) As Integer

            Dim bolRet As Integer = -1

            Try

                Dim strSQL As String
                strSQL = "@select# top 1 id from Zones WITH (NOLOCK) where IDParent is null order by id asc"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    bolRet = tb.Rows(0).Item("id")
                End If
            Catch ex As DbException
                state.UpdateStateInfo(ex, "roZone::GetRootZone")
            Catch ex As Exception
                state.UpdateStateInfo(ex, "roZone::GetRootZone")
            End Try

            Return bolRet

        End Function

#End Region

#End Region

#Region "Helper methods"

        Public Shared Function GetZoneByAdvancedCode(ByVal advancedCode As String, ByVal _State As roZoneState) As Integer

            Dim oRet As Integer = -1

            Try

                Dim strSQL As String = "@SELECT# TOP 1 ID FROM Zones WHERE Description like '%ID=" & advancedCode & ";%'"
                oRet = roTypes.Any2Integer(ExecuteScalar(strSQL))
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZone::GetZoneByAdvancedCode")
                oRet = 0
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZone::GetZoneByAdvancedCode")
                oRet = 0
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function IsThisAWorkingZone(iIDZone As Integer) As Boolean
            Dim strSQL As String = "@SELECT# isWorkingZone FROM Zones WHERE ID=" & iIDZone.ToString
            Return (roTypes.Any2Boolean(ExecuteScalar(strSQL)))
        End Function

        Public Shared Function GetZoneFromCoordinates(ByVal location As String, ByVal _State As roZoneState) As roZone
            Dim oRet As roZone = Nothing

            Try
                Dim strSQL As String = "@SELECT# * FROM Zones WHERE MapInfo IS NOT NULL ORDER BY Area ASC"
                Dim oZone As roZone

                Dim lLatitude As Double
                Dim lLongitude As Double

                ' Validamos
                If location.Split(",").Count <> 2 Then Return oRet

                lLatitude = roTypes.Any2Double(location.Replace(",", ";").Replace(".", ",").Split(";")(0))
                lLongitude = roTypes.Any2Double(location.Replace(",", ";").Replace(".", ",").Split(";")(1))

                Dim dt As DataTable = CreateDataTable(strSQL)
                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each oRow As DataRow In dt.Rows
                        oZone = New roZone(oRow("Id"), New roZoneState(-1))
                        If oZone IsNot Nothing AndAlso oZone.ID = 255 AndAlso _State IsNot Nothing Then
                            Dim oLng = New roLanguage()
                            If _State IsNot Nothing Then
                                oLng.SetLanguageReference("ZonesService", _State.Language.LanguageKey)
                            Else
                                oLng.SetLanguageReference("ZonesService", "ESP")
                            End If
                            oZone.Name = oLng.Translate("Zones.WorldZone", "")
                        End If
                        If oZone.GoogleMapInfo IsNot Nothing AndAlso oZone.GoogleMapInfo.Coordinates.Count = 2 Then
                            If lLatitude >= oZone.GoogleMapInfo.Coordinates(1).Latitud AndAlso lLatitude <= oZone.GoogleMapInfo.Coordinates(0).Latitud _
                               AndAlso lLongitude >= oZone.GoogleMapInfo.Coordinates(1).Longitud AndAlso lLongitude <= oZone.GoogleMapInfo.Coordinates(0).Longitud Then
                                oRet = oZone
                                Exit For
                            End If
                        End If
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZone::GetZoneFromCoordinates")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZone::GetZoneFromCoordinates")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Último fichaje por jornada por empleado. Si han pasado las horas de cortesía (12 por defecto) se considera que ya no está en esa zona.
        ''' El último fichaje puede ser de cualquier tipo. Única excepción, fichaje de salida desde Portal. En este caso, si son de salida y hace más de 15 minutos, se considera que ya no está (criterio fichero de emergencia)
        ''' </summary>
        ''' <param name="idZone"></param>
        ''' <param name="_State"></param>
        ''' <param name="oCn"></param>
        ''' <returns></returns>
        Public Shared Function GetEmployeesCurrentlyInZone(ByVal idZone As Integer, ByVal _State As roZoneState, Optional ByVal iCourtesyHours As Integer = 12) As Integer
            Dim oRet As Integer = 0

            Try
                ' Recupero fichaje de los últimos dos días (por si hay nocturnos y hay personas del turno de ayer
                Dim dt As New DataTable
                dt = Punch.roPunch.GetEmployeesLastPunchByDateDataTable(New Punch.roPunchState(-1), Now.Date.AddDays(-1), Now.Date)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    oRet = dt.Select(" IDZone = " & idZone.ToString & " AND Offset <= " & iCourtesyHours.ToString & " AND (IsPortal = 0 OR (IsPortal = 1 AND ActualType <> 2))").Count
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZone::GetEmployeesCurrentlyInZone")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZone::GetEmployeesCurrentlyInZone")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesOutOfZoneLastHour(ByVal idZone As Integer, ByVal _State As roZoneState) As Integer
            Dim oRet As Integer = 0

            Try
                ' Recupero fichaje de los últimos dos días (por si hay nocturnos y hay personas del turno de ayer
                Dim dt As New DataTable
                dt = Punch.roPunch.GetEmployeesLastMonthZoneMovesDataTable(New Punch.roPunchState(-1))
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    oRet = dt.Select(" IDZoneIn = " & idZone.ToString & " AND DateTimeOut >= " & roTypes.Any2Time(Now.AddHours(-1)).SQLSmallDateTime & " AND (ISNULL(IDZoneOut,0) <> " & idZone.ToString & " OR (TerminalOutType = 'LivePortal' AND ActualTypeOut = 2))").Count
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZone::GetEmployeesOutOfZoneLastHour")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZone::GetEmployeesOutOfZoneLastHour")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene información de los empleados que están actualmente en una zona
        ''' </summary>
        ''' <param name="idZone"></param>
        ''' <param name="_State"></param>
        ''' <param name="oCn"></param>
        ''' <returns></returns>
        Public Shared Function GetInfoEmployeesInZone(ByVal idZone As Integer, ByVal _State As roZoneState, Optional ByVal iCourtesyHours As Integer = 12) As List(Of EmployeeInfo)
            Dim oRet As List(Of EmployeeInfo) = New List(Of EmployeeInfo)()

            Try
                ' Recupero fichaje de los últimos dos días (por si hay nocturnos y hay personas del turno de ayer
                Dim dt As New DataTable
                dt = Punch.roPunch.GetEmployeesInfoLastPunchByDateDataTable(New Punch.roPunchState(-1), Now.Date.AddDays(-1), Now.Date)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each oRow As DataRow In dt.Select(" ActualType <> 2 AND IDZone = " & idZone.ToString & " AND Offset <= " & iCourtesyHours.ToString)
                        oRet.Add(New EmployeeInfo With {
                              .EmployeeId = roTypes.Any2Integer(oRow("IDEmployee")),
                              .Name = roTypes.Any2String(oRow("Name")),
                              .Image = "Employee/GetEmployeeLargePhoto/" & roTypes.Any2String(oRow("IDEmployee")),
                              .LastPunchTime = roTypes.Any2DateTime(oRow("Datetime")).ToString("H:mm")
                            })
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZone::GetInfoEmployeesInZone")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZone::GetInfoEmployeesInZone")
            Finally

            End Try

            Return oRet

        End Function

        ''' Obtiene información de los empleados que estuvieron en la zona en la última hora
        ''' </summary>
        ''' <param name="idZone"></param>
        ''' <param name="_State"></param>
        ''' <param name="oCn"></param>
        ''' <returns></returns>
        Public Shared Function GetInfoEmployeesInZoneLastHour(ByVal idZone As Integer, ByVal _State As roZoneState) As List(Of EmployeeInfo)
            Dim oRet As List(Of EmployeeInfo) = New List(Of EmployeeInfo)()

            Try
                ' Recupero fichaje de los últimos dos días (por si hay nocturnos y hay personas del turno de ayer
                Dim dt As New DataTable
                dt = Punch.roPunch.GetEmployeesInfoLastPunchByDateDataTable(New Punch.roPunchState(-1), Now.Date.AddDays(-1), Now.Date)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each oRow As DataRow In dt.Select(" ActualType = 2 AND IDZone = " & idZone.ToString & " AND Offset <= 1")
                        oRet.Add(New EmployeeInfo With {
                              .EmployeeId = roTypes.Any2Integer(oRow("IDEmployee")),
                              .Name = roTypes.Any2String(oRow("Name")),
                              .Image = "Employee/GetEmployeeLargePhoto/" & roTypes.Any2String(oRow("IDEmployee")),
                              .LastPunchTime = roTypes.Any2DateTime(oRow("Datetime")).ToString("H:mm")
                            })
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZone::GetInfoEmployeesInZone")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZone::GetInfoEmployeesInZone")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve la previsión de ocupación de una zona. Es una cantidad teórica en base a la zona esperada, y no tiene que ver con los fichajes reales realizados en la zona.
        ''' </summary>
        ''' <param name="oZone"></param>
        ''' <param name="dDate"></param>
        ''' <param name="_State"></param>
        ''' <param name="oCn"></param>
        ''' <param name="iIDEmployeeException">Si se indica un id de empleado, se devuleve la capacidad descontando al empleado en cuestión</param>
        ''' <returns></returns>
        Public Shared Function GetZoneExpectedOccupation(ByVal oZone As roZone, ByVal dDate As Date, ByVal _State As roZoneState, ByRef Optional iIDEmployeeException As Integer = 0) As Tuple(Of Integer, Boolean)
            Dim oRet As New Tuple(Of Integer, Boolean)(0, False)

            Try

                ' Recupero fichaje de los últimos dos días (por si hay nocturnos y hay personas del turno de ayer
                Dim dTable As DataTable = Nothing
                Dim iTotal As Integer = 0
                Dim iContainsEmployee As Integer = 0
                Dim strSQL As String = String.Empty

                If Zone.roZone.CapacityControlEnabled(New roZoneState(-1)) Then
                    strSQL = "@SELECT# * " &
                             "FROM [dbo].[EmployeeZonesBetweenDates] (" & roTypes.Any2Time(dDate).SQLSmallDateTime & "," & roTypes.Any2Time(dDate).SQLSmallDateTime & ",'')" &
                             "WHERE NoWork = 0 " &
                             "AND InAbsence = 0 " &
                             "AND (ISNULL(ZoneOnDate,'') = '" & oZone.Name & "' " & " OR (ISNULL(ExpectedZone,'') = '" & oZone.Name & "' AND ISNULL(TelecommutePlanned, TelecommutingExpected) = 0) )"
                    dTable = CreateDataTable(strSQL)
                End If

                If dTable IsNot Nothing Then
                    iTotal = dTable.Select("RefDate = '" & Format(dDate, "yyyy/MM/dd") & "'").Count
                    iContainsEmployee = dTable.Select("RefDate = '" & Format(dDate, "yyyy/MM/dd") & "' AND IdEmployee = " & iIDEmployeeException.ToString).Count
                End If

                oRet = New Tuple(Of Integer, Boolean)(iTotal, iContainsEmployee > 0)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZone::GetZoneExpectedOccupation")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZone::GetZoneExpectedOccupation")
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Devuelve la máxima ocupación de centro de trabajo asociado a una zona. Es decir, la suma de las máximas capacidades de todas las zonas del Centro de trabajo
        ''' </summary>
        ''' <param name="oZone"></param>
        ''' <param name="_State"></param>
        ''' <param name="oCn"></param>
        ''' <returns></returns>
        Public Shared Function GetZoneWorkCenterMaxOccupation(ByVal oZone As roZone, ByVal _State As roZoneState) As Integer
            Dim oRet As Integer = 0

            Try

                ' Recupero fichaje de los últimos dos días (por si hay nocturnos y hay personas del turno de ayer
                oRet = roTypes.Any2Integer(ExecuteScalar("@SELECT# ISNULL(SUM(Capacity),0) FROM Zones WHERE WorkCenter = '" & oZone.WorkCenter & "'"))
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZone::GetZoneWorkCenterMaxOccupation")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZone::GetZoneWorkCenterMaxOccupation")
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Devuelve si hay control de aforo
        ''' </summary>
        ''' <param name="_State"></param>
        ''' <param name="oCn"></param>
        ''' <returns></returns>
        Public Shared Function CapacityControlEnabled(ByVal _State As roZoneState) As Boolean
            Dim oRet As Boolean = False

            Try

                oRet = roTypes.Any2Boolean(ExecuteScalar("@SELECT# ISNULL(SUM(Capacity),0) FROM Zones"))
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roZone::CapacityControlEnabled")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roZone::CapacityControlEnabled")
            End Try

            Return oRet
        End Function

#End Region

    End Class

    <DataContract()>
    Public Class ZoneStatusMax

        Public Sub New()
            MaxLatitude = 0
            MaxLongitude = 0
        End Sub

        <DataMember()>
        Public MaxLatitude As Single
        <DataMember()>
        Public MaxLongitude As Single
    End Class

    <DataContract()>
    Public Class roZoneLite

        <DataMember()>
        Public Id As Integer
        <DataMember()>
        Public Name As String
    End Class

End Namespace