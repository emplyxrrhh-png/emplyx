Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace AdvancedParameter

    <DataContract()>
    Public Class roAdvancedParameter

#Region "Declarations - Constructor"

        Private oState As roAdvancedParameterState

        Private strParameterName As String
        Private strParameterValue As String
        Private bolExists As Boolean

        Public Sub New()
            Me.oState = New roAdvancedParameterState(-2)
            Me.strParameterName = ""
        End Sub

        Public Sub New(ByVal _Name As String, ByVal _State As roAdvancedParameterState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.strParameterName = _Name
            Me.Load(bAudit)
        End Sub

        Public Sub New(ByVal companyName As String, ByVal _Name As String, ByVal _State As roAdvancedParameterState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.strParameterName = _Name
            Me.LoadForCompany(companyName, bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roAdvancedParameterState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAdvancedParameterState)
                Me.oState = value
            End Set
        End Property

        <DataMember()>
        Public Property Name() As String
            Get
                Return Me.strParameterName
            End Get
            Set(ByVal value As String)
                Me.strParameterName = value
            End Set
        End Property

        <DataMember()>
        Public Property Exists() As Boolean
            Get
                Return Me.bolExists
            End Get
            Set(ByVal value As Boolean)
                Me.bolExists = value
            End Set
        End Property

        <DataMember()>
        Public Property Value() As String
            Get
                Return Me.strParameterValue
            End Get
            Set(ByVal value As String)
                Me.strParameterValue = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function LoadForCompany(ByVal companyName As String, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oCn As roBaseConnection = Nothing

            Try

                oCn = AccessHelper.CreateConnectionForCompany(companyName)

                Dim strSQL As String = "@SELECT# * FROM sysroLiveAdvancedParameters " &
                                       "WHERE [ParameterName] = '" & Me.strParameterName & "'"

                Dim tb As DataTable = CreateDataTable(strSQL, oCn)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("ParameterName")) Then Me.strParameterName = oRow("ParameterName")
                    If Not IsDBNull(oRow("Value")) Then Me.strParameterValue = oRow("Value")
                    Me.bolExists = True
                Else
                    Me.strParameterValue = ""
                    Me.bolExists = False
                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strParameterName, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tAdvancedParameter, Me.strParameterName, tbParameters, -1)
                End If

                bolRet = True
            Catch ex As ConnectionStringException
                Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "roAdvancedParameter::LoadForCompany::", ex)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAdvancedParameter::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAdvancedParameter::Load")
            Finally
                If oCn IsNot Nothing Then oCn.CloseIfNeeded()
            End Try

            Return bolRet

        End Function

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@SELECT# * FROM sysroLiveAdvancedParameters " &
                                       "WHERE [ParameterName] = '" & Me.strParameterName & "'"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("ParameterName")) Then Me.strParameterName = oRow("ParameterName")
                    If Not IsDBNull(oRow("Value")) Then Me.strParameterValue = oRow("Value")
                    Me.bolExists = True
                Else
                    Me.strParameterValue = ""
                    Me.bolExists = False
                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strParameterName, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tAdvancedParameter, Me.strParameterName, tbParameters, -1)
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAdvancedParameter::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAdvancedParameter::Load")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("Cameras")
                Dim strSQL As String = "@SELECT# * FROM sysroLiveAdvancedParameters WHERE ParameterName = '" & Me.strParameterName & "'"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    oRow("ParameterName") = Me.Name
                    bolIsNew = True
                Else
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                End If

                Dim bUpdateCache As Boolean = False

                If DataLayer.roSupport.IsXSSSafe(Me.strParameterValue) Then

                    If bolIsNew OrElse roTypes.Any2String(oRow("Value")).Trim() <> Me.strParameterValue.Trim() Then bUpdateCache = True


                    oRow("Value") = Me.strParameterValue

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oAuditDataNew = oRow
                    bolRet = True

                    If bUpdateCache Then DataLayer.roCacheManager.GetInstance().UpdateParamCache()
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                    Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                    Dim strObjectName As String
                    strObjectName = oAuditDataNew("ParameterName")
                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tAdvancedParameter, strObjectName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAdvancedParameter::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAdvancedParameter::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim strSql As String = "@DELETE# FROM sysroLiveAdvancedParameters WHERE  ParameterName = '" & Me.strParameterName & "'"

                bolRet = ExecuteSql(strSql)

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAdvancedParameter, Me.Name, Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAdvancedParameter::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAdvancedParameter::Delete")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetAdvancedParametersList(ByVal strOrderBy As String, ByVal _State As roAdvancedParameterState, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roAdvancedParameter)

            Dim oRet As New Generic.List(Of roAdvancedParameter)
            Try

                Dim strSQL As String = "@SELECT# * FROM sysroLiveAdvancedParameters ORDER BY "
                If strOrderBy <> "" Then
                    strSQL &= strOrderBy
                Else
                    strSQL &= "ParameterName ASC"
                End If

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oAdvParameter As roAdvancedParameter = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oAdvParameter = New roAdvancedParameter(oRow("ParameterName"), _State, bAudit)
                        oRet.Add(oAdvParameter)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAdvancedParameter::GetCamerasList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAdvancedParameter::GetCamerasList")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAdvancedParametersDataTable(ByVal strOrderBy As String, ByVal _State As roAdvancedParameterState, Optional ByVal bAudit As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroLiveAdvancedParameters ORDER BY "
                If strOrderBy <> "" Then
                    strSQL &= strOrderBy
                Else
                    strSQL = "ParameterName ASC"
                End If

                oRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAdvancedParameter::GetCamerasList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAdvancedParameter::GetCamerasList")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAdvancedParameterValue(ByVal strParameterName As String, ByVal _State As roAdvancedParameterState, Optional ByVal bAudit As Boolean = False) As String
            Dim strParameterValue As String = String.Empty

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroLiveAdvancedParameters " &
                                       "WHERE [ParameterName] = '" & strParameterName & "'"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("ParameterName")) Then strParameterName = oRow("ParameterName")
                    If Not IsDBNull(oRow("Value")) Then strParameterValue = oRow("Value")
                Else
                    strParameterValue = ""
                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = _State.CreateAuditParameters()
                    _State.AddAuditParameter(tbParameters, "{Name}", strParameterName, "", 1)
                    _State.Audit(Audit.Action.aSelect, Audit.ObjectType.tAdvancedParameter, strParameterName, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAdvancedParameter::GetAdvancedParameterValue")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAdvancedParameter::GetAdvancedParameterValue")
            End Try

            Return strParameterValue
        End Function


        Public Shared Function SetIpRestrictionStatus(ByVal bStatus As Boolean, ByRef _State As roAdvancedParameterState, Optional ByVal bAudit As Boolean = True) As Boolean

            Dim bolRet As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                bolRet = ExecuteSql($"@UPDATE# sysroLiveAdvancedParameters SET Value = '{If(bStatus, "1", "0")}' WHERE  ParameterName = 'VTPortal.ZoneRestrictedByIP'")

                'Solo si se ha activado la restricción por IP, se desactiva la geolocalización y se obliga a que se seleccione una zona
                If bStatus Then
                    Dim tmpParameter As New Common.AdvancedParameter.roAdvancedParameter("VTPortal.GeolocalizationConfiguration", New Common.AdvancedParameter.roAdvancedParameterState)
                    Dim oPortalGeolocalizationConfiguration As roPortalGeolocalizationConfiguration = New roPortalGeolocalizationConfiguration()
                    oPortalGeolocalizationConfiguration = roJSONHelper.DeserializeNewtonSoft(tmpParameter.Value, GetType(roPortalGeolocalizationConfiguration))
                    oPortalGeolocalizationConfiguration.Geolocalization = 2

                    If bolRet Then bolRet = ExecuteSql($"@UPDATE# sysroLiveAdvancedParameters SET Value = '{roJSONHelper.SerializeNewtonSoft(oPortalGeolocalizationConfiguration)}' WHERE  ParameterName = 'VTPortal.GeolocalizationConfiguration'")


                    tmpParameter = New Common.AdvancedParameter.roAdvancedParameter("VTPortal.PunchOptions", New Common.AdvancedParameter.roAdvancedParameterState)
                    Dim oPortalPunchOptions As roPortalPunchOptions = New roPortalPunchOptions()
                    oPortalPunchOptions = roJSONHelper.DeserializeNewtonSoft(tmpParameter.Value, GetType(roPortalPunchOptions))
                    oPortalPunchOptions.ZoneRequired = True

                    If bolRet Then bolRet = ExecuteSql($"@UPDATE# sysroLiveAdvancedParameters SET Value = '{roJSONHelper.SerializeNewtonSoft(oPortalPunchOptions)}' WHERE  ParameterName = 'VTPortal.PunchOptions'")
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    _State.Audit(Audit.Action.aUpdate, Audit.ObjectType.tAdvancedParameter, "VTPortal.ZoneRestrictedByIP", Nothing, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roAdvancedParameter::SetIpRestrictionStatus")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAdvancedParameter::SetIpRestrictionStatus")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace