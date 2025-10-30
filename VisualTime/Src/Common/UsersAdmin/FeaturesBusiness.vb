Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.UsersAdmin.DataAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace Business

    ''' <summary>
    ''' Exposes a list of applications and application parts with permissions
    ''' information required to draw an editable tree of features.
    ''' </summary>
    ''' <remarks>Features information is calculated in 3 steps:
    ''' Step 1: Load applications and application parts from database with those fields:
    '''         ID, IDParent, Alias, Name, Type, IsGroup, PermissionTypes.
    ''' Step 2: Check permissions for all entries where "IsGroup = false" to fill those fields:
    '''         MaxConfigurable, ObjectValue, InheritedValue, EditedValue.
    ''' Step 3: Calculate values for groups based on their content. Remember that there
    '''         are no permissions defined directly on features groups.
    '''
    ''' Here is a description of table fields
    '''     ID, IDParent, Alias, Name, PermissionTypes: basic information about features.
    '''     IsGroup: Wether feature contain other features. If it does,
    '''         it is considered as a group, otherwise, it is considered as a feature.
    '''     MaxConfigurable: The highest value that can be assigned to the feature, restricted by parent passport.
    '''     ObjectValue: The permission(Admin, Write, Read or None) defined for current user or group.
    '''     InheritedValue: The permission(Admin, Write, Read or None) defined for parent group.
    '''     EditedValue: The permissions changes made by the user.
    ''' </remarks>
    Public Class FeaturesBusiness

#Region " Declarations / Constructor "

        Private _data As FeaturesDataSet
        Private _idPassport As Integer
        Private _featureType As String = "U"

        ''' <summary>
        ''' Initializes a new instance of the FeaturessBusiness class
        ''' and computes features data.
        ''' </summary>
        ''' <param name="idPassport">The passport for which to calculate permissions.</param>
        Public Sub New(ByVal idPassport As Integer)
            _idPassport = idPassport
            _data = New FeaturesDataSet()
            ComputeDataSet()
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the FeaturessBusiness class
        ''' and computes features data.
        ''' </summary>
        ''' <param name="idPassport">The passport for which to calculate permissions.</param>
        Public Sub New(ByVal idPassport As Integer, ByVal featureType As String)
            _idPassport = idPassport
            _featureType = featureType
            _data = New FeaturesDataSet()
            ComputeDataSet()
        End Sub

        Public Sub New()
            _idPassport = 0
            _data = New FeaturesDataSet
            ComputeDataSet()
        End Sub

#End Region

#Region " Calculate permissions "

        Public Sub ComputeDataSet()
            _data.Clear()

            If _idPassport = -1 Then _idPassport = roConstants.GetSystemUserId()

            ' Step 1: Load data into DataSet.
            Dim oLicSupport As New Extensions.roLicenseSupport()
            Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()
            Dim sEdition As String = String.Empty
            Select Case oLicInfo.Edition
                Case roServerLicense.roVisualTimeEdition.NotSet
                Case Else
                    sEdition = oLicInfo.Edition.ToString
            End Select
            FeaturesAccess.GetList(_data.Features, _featureType, sEdition)

            ' Step 2: Check permissions over features.
            CheckFeaturesPermissions()

            ' Step 3: Calculate values for features groups.
            CalculateGroupValues()

            _data.AcceptChanges()
        End Sub

        ''' <summary>
        ''' Checks permissions over all features to fill fields in table.
        ''' </summary>
        Private Sub CheckFeaturesPermissions()
            ' Loop through all features.
            Dim _rightsOverFeatures As New PermissionsOverFeatures(roPassportManager.GetPassport(_idPassport))

            Dim Rows As DataRow() = _data.Features.Select("IsGroup = false")
            Dim Value As Permission
            For Each row As FeaturesDataSet.FeaturesRow In Rows
                row.MaxConfigurable = _rightsOverFeatures.MaxConfigurable(row._Alias, row.Type)
                Value = _rightsOverFeatures.GetSys(row._Alias, _featureType)
                row.ObjectValue = Value
                row.EditedValue = Value
                row.InheritedValue = _rightsOverFeatures.GetSys(row._Alias, _featureType, PermissionCheckMode.InheritedOnly)
            Next
        End Sub

        ''' <summary>
        ''' Calculates permissions for all features groups to fill fields in table.
        ''' </summary>
        Private Sub CalculateGroupValues()
            ' Start recursive calculation from roots.
            For Each row As FeaturesDataSet.FeaturesRow In _data.Features.Select("IDParent IS NULL")
                If row.IsGroup Then
                    CalculateGroupValuesRecursive(row)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Calculates values for specified features group and sub-groups to fill these fields in table:
        ''' MaxConfigurable, ObjectValue, InheritedValue, EditedValue.
        ''' </summary>
        ''' <param name="groupRow">The features group to calculate value for.</param>
        ''' <remarks>These fields are set to the highest among child values.</remarks>
        Private Sub CalculateGroupValuesRecursive(ByVal groupRow As FeaturesDataSet.FeaturesRow)
            groupRow.MaxConfigurable = 0
            groupRow.ObjectValue = 0
            groupRow.InheritedValue = 0
            groupRow.EditedValue = 0

            For Each row As FeaturesDataSet.FeaturesRow In groupRow.GetFeaturesRows()
                If row.IsGroup Then
                    CalculateGroupValuesRecursive(row)
                End If

                If row.MaxConfigurable > groupRow.MaxConfigurable Then groupRow.MaxConfigurable = row.MaxConfigurable
                If row.ObjectValue > groupRow.ObjectValue Then groupRow.ObjectValue = row.ObjectValue
                If row.InheritedValue > groupRow.InheritedValue Then groupRow.InheritedValue = row.InheritedValue
                If row.EditedValue > groupRow.EditedValue Then groupRow.EditedValue = row.EditedValue
                For Each p As String In {"R", "W", "A"}
                    If row.PermissionTypes.Contains(p) AndAlso Not groupRow.PermissionTypes.Contains(p) Then groupRow.PermissionTypes += p
                Next
            Next
        End Sub

#End Region

#Region " SetPermission / RestaureDefault "

        ''' <summary>
        ''' Updates permissions for specified item.
        ''' </summary>
        ''' <param name="row">The row of the application part to set value for.</param>
        ''' <param name="value">The value to set.</param>
        Public Sub SetPermission(ByVal row As FeaturesDataSet.FeaturesRow, ByVal value As Permission)
            row.EditedValue = value
            UpdateChildPermissions(row, value)
            '*** Update parents ***
            UpdateParentPermissions(row, value)
        End Sub

        ''' <summary>
        ''' Restores inherited value for specified feature or features group.
        ''' </summary>
        ''' <param name="row">The row for which to restore inherited value.</param>
        ''' <remarks>This method is called recursively.</remarks>
        Public Sub RestoreInherited(ByVal row As FeaturesDataSet.FeaturesRow)
            ' Restore row value.
            row.EditedValue = row.InheritedValue

            ' Restore for all childs.
            For Each childRow As FeaturesDataSet.FeaturesRow In row.GetFeaturesRows()
                RestoreInherited(childRow)
            Next
        End Sub

        ''' <summary>
        ''' Restores original value for specified feature or features group.
        ''' </summary>
        ''' <param name="row">The row for which to restore original value.</param>
        ''' <remarks>This method is called recursively.</remarks>
        Public Sub RestoreOriginal(ByVal row As FeaturesDataSet.FeaturesRow)
            ' Restore row value.
            row.EditedValue = row.ObjectValue

            ' Restore for all childs.
            For Each childRow As FeaturesDataSet.FeaturesRow In row.GetFeaturesRows()
                RestoreOriginal(childRow)
            Next
        End Sub

        ''' <summary>
        ''' Updates the value of all childs of specified features group.
        ''' </summary>
        ''' <param name="row">The row for which to update childs.</param>
        ''' <param name="value">The value to set.</param>
        ''' <remarks>If value can't be set for a child, the closest lower value will be set.</remarks>
        Private Sub UpdateChildPermissions(ByVal row As FeaturesDataSet.FeaturesRow, ByVal value As Permission)
            Dim ValueToSet As Permission
            For Each childRow As FeaturesDataSet.FeaturesRow In row.GetFeaturesRows()
                ValueToSet = value
                If ValueToSet > Permission.Write AndAlso Not childRow.PermissionTypes.Contains("A") Then ValueToSet = Permission.Write
                If ValueToSet > Permission.Read AndAlso Not childRow.PermissionTypes.Contains("W") Then
                    If Not childRow.PermissionTypes.Contains("A") Then _
                        ValueToSet = Permission.Read
                End If
                If ValueToSet > Permission.None AndAlso Not childRow.PermissionTypes.Contains("R") Then
                    If Not childRow.PermissionTypes.Contains("W") Then _
                        ValueToSet = Permission.None
                End If
                childRow.EditedValue = ValueToSet

                ' Update recursively for sub-childs.
                UpdateChildPermissions(childRow, CType(childRow.EditedValue, Permission))
            Next
        End Sub

        Private Sub UpdateParentPermissions(ByVal row As FeaturesDataSet.FeaturesRow, ByVal value As Permission)

            Dim oRootParent As FeaturesDataSet.FeaturesRow = row
            While Not oRootParent.IsIDParentNull
                oRootParent = _data.Features.FindByID(oRootParent.IDParent)
            End While

            ' Start recursive calculation from root.
            If oRootParent.IsGroup Then
                CalculateGroupValuesRecursive(oRootParent)
            End If
        End Sub

#End Region

#Region " Update"

        ''' <summary>
        ''' Updates permissions changes back to the database, according to EditedValue field.
        ''' </summary>
        ''' <param name="trans">The transaction under which to perform data access.</param>
        Public Sub Update()
            Dim Changes As DataRow() = Table.Select("(EditedValue <> ObjectValue AND IsGroup = False) OR IsGroup = True")
            If Changes.Length > 0 Then
                UpdateRows(Changes)
            End If
        End Sub

        ''' <summary>
        ''' Updates all rows to the database even if they were not changed.
        ''' </summary>
        ''' <param name="trans">The transaction under which to perform data access.</param>
        ''' <remarks>This is usefull when moving a user or group to ensure permissions
        ''' are consistent.</remarks>
        Public Sub UpdateAll()
            UpdateRows(Table.Select("IsGroup = False"))
        End Sub

        ''' <summary>
        ''' Updates the specified list of rows.
        ''' </summary>
        ''' <param name="rowsColl">The list of rows to update, of type FeaturesDataSet.FeaturesRow.</param>
        Private Sub UpdateRows(ByVal rowsColl As DataRow())

            Dim _rightsOverFeatures As New PermissionsOverFeatures(roPassportManager.GetPassport(_idPassport))

            For Each row As FeaturesDataSet.FeaturesRow In rowsColl
                If (row.EditedValue = row.InheritedValue) Then
                    _rightsOverFeatures.Remove(row._Alias, row.Type, False)
                Else
                    _rightsOverFeatures.Set(row._Alias, row.Type, CType(row.EditedValue, Permission), False)
                End If
            Next

            Try
                Dim oStateTask As New roLiveTaskState(-1)
                Dim oParameters As New roCollection
                oParameters.Add("Mode", "")
                roLiveTask.CreateLiveTask(roLiveTaskTypes.ChangeRequestPermissions, oParameters, oStateTask)

                Dim Command As DbCommand = AccessHelper.CreateCommand("sysro_SetInvalidationCodeSession")
                Command.CommandType = CommandType.StoredProcedure
                AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32, 1).Value = roTypes.Any2Integer(_idPassport)
                AccessHelper.AddParameter(Command, "@invalidationCode", DbType.Int32, 1).Value = 1
                Command.ExecuteNonQuery()
            Catch ex As Exception
                'do nothing
            End Try

        End Sub

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets or sets the ID of the passport to manage permissions for.
        ''' </summary>
        Public Property IDPassport() As Integer
            Get
                Return _idPassport
            End Get
            Set(ByVal value As Integer)
                _idPassport = value
                ComputeDataSet()
            End Set
        End Property

        ''' <summary>
        ''' Returns the DataSet containing calculated data.
        ''' </summary>
        Public Property DataSet() As FeaturesDataSet
            Get
                Return _data
            End Get
            Set(ByVal value As FeaturesDataSet)
                _data = value
            End Set
        End Property

        ''' <summary>
        ''' Returns the DataTable containing calculated data.
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property Table() As FeaturesDataSet.FeaturesDataTable
            Get
                Return _data.Features
            End Get
        End Property

#End Region

#Region "Shared"

        Public Shared Function GetFeaturesFromPassport(idPassport As Integer, idFeature As Integer?, type As String, ByRef bState As roSecurityState) As List(Of Feature)
            Dim oFeatures As New Generic.List(Of Feature)
            Try

                Dim _business As New FeaturesBusiness(idPassport, type)

                Dim strWhere As String = ""
                If Not idFeature.HasValue Then ' Cargamos las funcionalidades de la raiz
                    strWhere = "IDParent IS NULL"
                Else
                    Dim oFeatureRow As FeaturesDataSet.FeaturesRow = _business.Table.FindByID(idFeature.Value)
                    If oFeatureRow IsNot Nothing AndAlso oFeatureRow.IsGroup Then
                        strWhere = "IDParent = " & idFeature.Value ' Cargamos las funcionalidades
                    End If
                End If

                If strWhere <> "" Then
                    Dim oFeature As Feature
                    For Each row As FeaturesDataSet.FeaturesRow In _business.Table.Select(strWhere & " AND Type='" & type & "'", "ID")
                        If row.MaxConfigurable > Permission.None Then
                            oFeature = New Feature(row, bState)
                            ' Prevent editing of UsersAdmin for current user.
                            If IsUsersAdminForCurrentUser(row, idPassport, bState.IDPassport) Then
                                oFeature.Permissions.Clear()
                                oFeature.Permissions.Add(Permission.Admin)
                                oFeature.Permissions.Add(Permission.Write)
                            End If
                            oFeatures.Add(oFeature)
                        End If
                    Next
                End If
            Catch ex As System.Data.Common.DbException
                bState.UpdateStateInfo(ex, "wsUserAdmin::GetFeaturesFromPassport")
            Catch ex As Exception
                bState.UpdateStateInfo(ex, "wsUserAdmin::GetFeaturesFromPassport")
            End Try

            Return oFeatures
        End Function

        Public Shared Function GetFeaturesFromPassportAll(idPassport As Integer, type As String, ByRef bState As roSecurityState) As List(Of Feature)
            Dim oFeatures As New Generic.List(Of Feature)
            Dim intMaxConfigurable As Integer = -1

            Try

                Dim _business As New FeaturesBusiness(idPassport, type)

                ' En el caso que sea modo seguridad v2, y las funcionalidades sean de tipo empleado 'E'
                ' dejamos configurar todos los permisos sin restricciones de su grupo de usuario

                If type = "E" Then
                    intMaxConfigurable = 9
                End If

                Dim oFeature As Feature
                For Each row As FeaturesDataSet.FeaturesRow In _business.Table.Select("Type='" & type & "'", "ID")
                    If intMaxConfigurable = 9 Then
                        row.MaxConfigurable = 9
                    End If
                    If row.MaxConfigurable > Permission.None Then
                        oFeature = New Feature(row, bState)
                        ' Prevent editing of UsersAdmin for current user.
                        If IsUsersAdminForCurrentUser(row, idPassport, bState.IDPassport) Then
                            oFeature.Permissions.Clear()
                            oFeature.Permissions.Add(Permission.Admin)
                            oFeature.Permissions.Add(Permission.Write)
                        End If
                        oFeatures.Add(oFeature)
                    End If
                Next
            Catch ex As System.Data.Common.DbException
                bState.UpdateStateInfo(ex, "wsUserAdmin::GetFeaturesFromPassportAll")
            Catch ex As Exception
                bState.UpdateStateInfo(ex, "wsUserAdmin::GetFeaturesFromPassportAll")
            End Try

            Return oFeatures
        End Function

        Public Shared Function IsUsersAdminForCurrentUser(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal FeatureType As String, ByRef oState As roSecurityState) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim _business As New FeaturesBusiness(idPassport, FeatureType)

                Dim row As FeaturesDataSet.FeaturesRow = _business.Table.FindByID(idFeature)

                bolRet = FeaturesBusiness.IsUsersAdminForCurrentUser(row, idPassport, oState.IDPassport)
            Catch ex As System.Data.Common.DbException
                oState.UpdateStateInfo(ex, "wsUserAdmin::IsUsersAdminForCurrentUser")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "wsUserAdmin::IsUsersAdminForCurrentUser")
            End Try

            Return bolRet

        End Function

        Public Shared Function IsUsersAdminForCurrentUser(ByVal row As FeaturesDataSet.FeaturesRow, ByVal idPassport As Integer, ByVal idCurrentPassport As Integer, Optional ByVal intPermission As Integer = -1) As Boolean

            Dim bolRet As Boolean = False

            If row IsNot Nothing Then

                ' Make sure we don't remove UsersAdmin for current user or for a parent group.
                Dim bolIsUsersAdminFeature As Boolean = False
                If intPermission = -1 Then
                    bolIsUsersAdminFeature = (row._Alias = Features.UsersAdmin)
                Else
                    bolIsUsersAdminFeature = FeaturesBusiness.IsFeature(Features.UsersAdmin, row, CType(intPermission, Permission))
                End If
                If bolIsUsersAdminFeature Then 'If row._Alias = Features.UsersAdmin Then
                    Dim oManager As New roPassportManager
                    Dim User As roPassportTicket = oManager.LoadPassportTicket(idCurrentPassport, LoadType.Passport)
                    If User.ID = idPassport OrElse oManager.GetParents(User.ID).Contains(idPassport) Then
                        bolRet = True
                    End If
                End If

            End If

            Return bolRet

        End Function

        Public Shared Function IsFeature(ByVal strAlias As String, ByVal row As FeaturesDataSet.FeaturesRow, ByVal oPermission As Permission) As Boolean
            Dim bolRet As Boolean = False
            If row._Alias = strAlias Then
                If oPermission < Permission.Write Then ' If row.EditedValue > oPermission Then
                    bolRet = True
                End If
            Else
                For Each oChild As FeaturesDataSet.FeaturesRow In row.GetChildRows("FK_Features_Features")
                    bolRet = IsFeature(strAlias, oChild, oPermission)
                    If bolRet Then Exit For
                Next
            End If
            Return bolRet
        End Function

        Public Shared Function GetFeaturePermissions(idPassport As Integer, idFeature As Integer, FeatureType As String, ByRef oState As roSecurityState) As List(Of Permission)
            Dim oPermissions As New Generic.List(Of Permission)
            Try

                Dim _business As New FeaturesBusiness(idPassport, FeatureType)

                Dim Row As FeaturesDataSet.FeaturesRow = _business.Table.FindByID(idFeature)
                If Row IsNot Nothing Then

                    oPermissions.Add(Permission.Admin)
                    oPermissions.Add(Permission.Write)
                    oPermissions.Add(Permission.Read)
                    oPermissions.Add(Permission.None)
                    ''oPermissions.Add(Permission.Default)

                    ' Prevent editing of UsersAdmin for current user.
                    If IsUsersAdminForCurrentUser(Row, idPassport, oState.IDPassport) Then
                        oPermissions.Remove(Permission.Admin)
                        oPermissions.Remove(Permission.Read)
                        oPermissions.Remove(Permission.None)
                        ''oPermissions.Remove(Permission.Default)
                    Else
                        If Row.MaxConfigurable < Permission.Admin Or Not Row.PermissionTypes.Contains("A") Then oPermissions.Remove(Permission.Admin)
                        If Row.MaxConfigurable < Permission.Write Or Not Row.PermissionTypes.Contains("W") Then oPermissions.Remove(Permission.Write)
                        If Row.MaxConfigurable < Permission.Read Or Not Row.PermissionTypes.Contains("R") Then oPermissions.Remove(Permission.Read)
                    End If

                End If
            Catch ex As System.Data.Common.DbException
                oState.UpdateStateInfo(ex, "wsUserAdmin::GetFeaturePermissions")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "wsUserAdmin::GetFeaturePermissions")
            End Try

            Return oPermissions
        End Function

        Public Shared Function SetFeaturePermission(idPassport As Integer, idFeature As Integer, FeatureType As String, _permission As Permission, ByRef FeaturesChanged As List(Of Feature), ByRef bState As roSecurityState) As Boolean
            Dim bolRet As Boolean = False
            Dim intMaxConfigurable As Integer = -1

            Try

                Dim _business As New FeaturesBusiness(idPassport, FeatureType)

                ' En el caso que sea modo seguridad v2, y las funcionalidades sean de tipo empleado 'E'
                ' dejamos configurar todos los permisos sin restricciones de su grupo de usuario

                If FeatureType = "E" Then
                    intMaxConfigurable = 9
                End If

                Dim row As FeaturesDataSet.FeaturesRow = _business.Table.FindByID(idFeature)
                If row IsNot Nothing Then
                    If intMaxConfigurable = 9 Then
                        row.MaxConfigurable = 9
                    End If
                End If

                If row IsNot Nothing AndAlso _permission <= row.MaxConfigurable AndAlso (_permission = Permission.None OrElse row.PermissionTypes.Contains(_permission.ToString().Chars(0))) Then
                    ' Make sure we don't remove Write permission to UsersAdmin for current user.
                    If Not IsUsersAdminForCurrentUser(row, idPassport, bState.IDPassport, _permission) Then
                        _business.SetPermission(row, _permission)
                        _business.Update()
                        _business.ComputeDataSet()
                        bolRet = True
                    Else
                        bState.Result = SecurityResultEnum.IsUsersAdminForCurrentUser
                    End If
                End If

                If bolRet Then
                    ' Obtenemos los permisos hijos
                    row = _business.Table.FindByID(idFeature)
                    FeaturesChanged = FeaturesBusiness.GetChildFeatures(row, bState)
                    If FeaturesChanged Is Nothing Then FeaturesChanged = New Generic.List(Of Feature)
                    FeaturesChanged.Add(New Feature(row, bState))
                    While Not row.IsIDParentNull
                        row = _business.Table.FindByID(row.IDParent)
                        If row Is Nothing Then Exit While
                        FeaturesChanged.Add(New Feature(row, bState))
                    End While

                    'PPR (integracion visitas con VT Live)
                    If FeatureType = "E" Then
                        Dim sSQL As String = String.Empty
                        For Each ft As Feature In FeaturesChanged
                            If ft.IDParent.HasValue Then
                                If ft.IDParent.Value = 23 Then 'Grupo Visitas
                                    Select Case ft.ID
                                        Case 23010 'Definir tipos de visitas (AllowVisitsAdmin)
                                            If _permission >= Permission.Write Then
                                                sSQL = "AllowVisitsAdmin = 1,"
                                            Else
                                                sSQL = "AllowVisitsAdmin = 0,"
                                            End If
                                        Case 23020 'Programar visitas (AllowVisitsPlan)
                                            If _permission >= Permission.Write Then
                                                sSQL &= "AllowVisitsPlan = 1,"
                                            Else
                                                sSQL &= "AllowVisitsPlan = 0,"
                                            End If
                                        Case 23030 'Recibir visitas (AllowVisits)
                                            If _permission >= Permission.Write Then
                                                sSQL &= "AllowVisits = 1,"
                                            Else
                                                sSQL &= "AllowVisits = 0,"
                                            End If
                                    End Select
                                End If
                            End If
                        Next

                        If sSQL <> String.Empty Then
                            Dim oManager As New roPassportManager(bState)
                            Dim oPassport As roPassportTicket = oManager.LoadPassportTicket(idPassport, LoadType.Passport)
                            If oPassport IsNot Nothing AndAlso oPassport.IDEmployee.HasValue Then
                                If sSQL.EndsWith(",") Then sSQL = sSQL.Remove(sSQL.Length() - 1)
                                sSQL = "@UPDATE# Employees SET " & sSQL & " FROM Employees WHERE ID = " & oPassport.IDEmployee.Value
                                AccessHelper.ExecuteSql(sSQL)
                            End If
                        End If
                    End If

                End If
            Catch ex As System.Data.Common.DbException
                bState.UpdateStateInfo(ex, "wsUserAdmin::SetFeaturePermission")
            Catch ex As Exception
                bState.UpdateStateInfo(ex, "wsUserAdmin::SetFeaturePermission")
            End Try

            Return bolRet
        End Function

        Public Shared Function SetDefaultFeaturePermission(idPassport As Integer, idFeature As Integer, FeatureType As String, ByRef FeaturesChanged As List(Of Feature), ByRef bState As roSecurityState) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim _business As New FeaturesBusiness(idPassport, FeatureType)

                Dim row As FeaturesDataSet.FeaturesRow = _business.Table.FindByID(idFeature)

                If row IsNot Nothing Then
                    ' Make sure we don't remove Write permission to UsersAdmin for current user.
                    If Not IsUsersAdminForCurrentUser(row, idPassport, bState.IDPassport, row.InheritedValue) Then
                        _business.RestoreInherited(row)
                        _business.Update()
                        _business.ComputeDataSet()
                        bolRet = True
                    Else
                        bState.Result = SecurityResultEnum.IsUsersAdminForCurrentUser
                    End If
                End If

                If bolRet Then
                    ' Obtenemos los permisos hijos
                    row = _business.Table.FindByID(idFeature)
                    FeaturesChanged = FeaturesBusiness.GetChildFeatures(row, bState)
                    If FeaturesChanged Is Nothing Then FeaturesChanged = New Generic.List(Of Feature)
                    FeaturesChanged.Add(New Feature(row, bState))
                    While Not row.IsIDParentNull
                        row = _business.Table.FindByID(row.IDParent)
                        If row Is Nothing Then Exit While
                        FeaturesChanged.Add(New Feature(row, bState))
                    End While
                End If
            Catch ex As System.Data.Common.DbException
                bState.UpdateStateInfo(ex, "wsUserAdmin::SetDefaultFeaturePermission")
            Catch ex As Exception
                bState.UpdateStateInfo(ex, "wsUserAdmin::SetDefaultFeaturePermission")
            End Try

            Return bolRet

        End Function

        Private Shared Function GetChildFeatures(ByVal row As FeaturesDataSet.FeaturesRow, ByVal oState As roSecurityState) As Generic.List(Of Feature)

            Dim lstFeatures As New Generic.List(Of Feature)
            Dim lstChilds As Generic.List(Of Feature)

            For Each childRow As FeaturesDataSet.FeaturesRow In row.GetFeaturesRows()
                lstFeatures.Add(New Feature(childRow, oState))
                lstChilds = FeaturesBusiness.GetChildFeatures(childRow, oState)
                For Each oFeatureChild As Feature In lstChilds
                    lstFeatures.Add(oFeatureChild)
                Next
            Next

            Return lstFeatures

        End Function

#End Region

    End Class

End Namespace