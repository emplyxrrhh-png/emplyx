Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.Indicator
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace GroupIndicator

    <DataContract>
    Public Class roGroupIndicator

#Region "Declarations - Constructors"

        Private oState As roGroupState
        Private intIDIndicator As Integer
        Private intIDGroup As Integer

        Private oIndicator As roIndicator

        Public Sub New()

            Me.oState = New roGroupState
            Me.intIDIndicator = -1
            Me.intIDGroup = -1

        End Sub

        Public Sub New(ByVal _IDIndicator As Integer, ByVal _IDGroup As Integer, ByVal _State As roGroupState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.intIDIndicator = _IDIndicator
            Me.intIDGroup = _IDGroup

            Me.Load(_Audit)

        End Sub

        Public Sub New(ByVal _Row As DataRow, ByVal _State As roGroupState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State

            Me.LoadFromRow(_Row, _Audit)

        End Sub

#End Region

#Region "Properties"

        <XmlIgnore()>
        <IgnoreDataMember>
        Public Property State() As roGroupState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roGroupState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property IDIndicator() As Integer
            Get
                Return Me.intIDIndicator
            End Get
            Set(ByVal value As Integer)
                Me.intIDIndicator = value
            End Set
        End Property

        <DataMember>
        Public Property IDGroup() As Integer
            Get
                Return Me.intIDGroup
            End Get
            Set(ByVal value As Integer)
                Me.intIDGroup = value
            End Set
        End Property

        <DataMember>
        Public ReadOnly Property Indicator() As roIndicator
            Get
                If (Me.intIDIndicator = -1) Then
                    Return Nothing
                Else
                    oIndicator = New roIndicator
                    oIndicator.ID = Me.intIDIndicator
                    oIndicator.Load(False)
                    Return oIndicator
                End If
            End Get
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.Exists() Then
                    bolRet = True
                End If

                ' Auditar lectura
                'If _Audit Then
                If False Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.IDGroup & "_" & Me.IDIndicator, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tAssignment, Me.IDGroup & "_" & Me.IDIndicator, tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roGroupIndicator::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGroupIndicator::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function Exists()
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM GroupIndicators WHERE IDIndicator = " & Me.intIDIndicator & " AND IDGroup = " & Me.intIDGroup
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    bolRet = True
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roGroupIndicator::Exists")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGroupIndicator::Exists")
            End Try

            Return bolRet
        End Function

        Private Function LoadFromRow(ByVal oRow As DataRow, Optional ByVal _Audit As Boolean = False)

            Dim bolRet As Boolean = False

            If oRow IsNot Nothing Then

                Me.intIDGroup = oRow("IDGroup")
                Me.intIDIndicator = oRow("IDIndicator")

                bolRet = True

                If _Audit Then
                    ' ***
                End If

            End If

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("GroupIndicators")
                    Dim oRow As DataRow = tb.NewRow

                    oRow("IDGroup") = Me.intIDGroup
                    oRow("IDIndicator") = Me.intIDIndicator

                    oRow.AcceptChanges()

                    oAuditDataNew = oRow

                    bolRet = True

                    'If bolRet And bAudit Then
                    If False Then
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
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tAssignment, strObjectName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroupIndicator::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupIndicator::Save")
            End Try

            Return bolRet

        End Function

        Public Function Validate(Optional ByVal bolCheckNames As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try

                ' El primer saldo no puede ser nulo
                If Me.IDGroup = -1 Then
                    oState.Result = GroupResultEnum.GroupCannotBeNull
                    bolRet = False
                    Return False
                End If

                ' El primer saldo no puede ser nulo
                If Me.IDIndicator = -1 Then
                    oState.Result = GroupResultEnum.IndicatorCannotBeNull
                    bolRet = False
                    Return False
                End If

                If bolRet And Me.Exists() Then
                    bolRet = False
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroupIndicator::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupIndicator::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Borra el puesto siempre y cuando no se use.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Not Me.IsUsed() Then

                    bolRet = False

                    'Borramos el puesto
                    Dim DelQuerys() As String = {"@DELETE# FROM Indicators WHERE WHERE IDIndicator = " & Me.intIDIndicator & " AND IDGroup = " & Me.intIDGroup}
                    For n As Integer = 0 To DelQuerys.Length - 1
                        If Not ExecuteSql(DelQuerys(n)) Then
                            oState.Result = GroupResultEnum.ConnectionError
                            Exit For
                        End If
                    Next

                    bolRet = (oState.Result = GroupResultEnum.NoError)

                    'If bolRet And bAudit Then
                    If False Then
                        '' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAssignment, Me.IDGroup & "_" & Me.IDIndicator, Nothing, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroupIndicator::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupIndicator::Delete")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Verifica si el puesto se está usando. En oState.Result establece quien lo está usando.
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsUsed() As Boolean

            Dim bolIsUsed As Boolean = False

            Try

                bolIsUsed = False
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroupIndicator::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupIndicator::IsUsed")
            Finally

            End Try

            Return bolIsUsed

        End Function

        Shared Function GetGroupIndicators(ByVal IDGroup As Integer, ByRef oState As roGroupState) As Generic.List(Of roGroupIndicator)
            Dim oRet As New Generic.List(Of roGroupIndicator)

            Try

                Dim strSQL As String = "@SELECT# GroupIndicators.* " &
                                       "FROM GroupIndicators " &
                                       "WHERE GroupIndicators.IDGroup = " & IDGroup
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(New roGroupIndicator(oRow, oState, False))
                    Next

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroupIndicator::GetGroupIndicators")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupIndicator::GetGroupIndicators")
            Finally

            End Try

            Return oRet
        End Function

        Shared Function GetIndicatorsDataTable(ByVal IDGroup As Integer, ByRef _State As roGroupState) As DataTable
            ' Devuelvo todos los userfields de un grupo en un Dataset
            Dim oRet As DataTable = Nothing

            _State.UpdateStateInfo()

            Dim gIndicators As Generic.List(Of roGroupIndicator) = roGroupIndicator.GetGroupIndicators(IDGroup, _State)
            If _State.Result = GroupResultEnum.NoError Then

                Try

                    oRet = New DataTable

                    oRet.Columns.Add(New DataColumn("Name", GetType(String)))
                    oRet.Columns.Add(New DataColumn("Description", GetType(String)))

                    oRet.Columns.Add(New DataColumn("IDGroup", GetType(Integer)))
                    oRet.Columns.Add(New DataColumn("IDIndicator", GetType(Integer)))

                    If gIndicators IsNot Nothing Then

                        Dim oNewRow As DataRow = Nothing

                        For Each oGroupIndicator As roGroupIndicator In gIndicators

                            oNewRow = oRet.NewRow
                            oNewRow("IDGroup") = oGroupIndicator.IDGroup
                            oNewRow("IDIndicator") = oGroupIndicator.IDIndicator

                            If oGroupIndicator.Indicator IsNot Nothing Then
                                oNewRow("Name") = oGroupIndicator.Indicator.Name
                                oNewRow("Description") = oGroupIndicator.Indicator.Description
                            End If

                            oRet.Rows.Add(oNewRow)
                        Next

                    End If
                Catch ex As DbException
                    _State.UpdateStateInfo(ex, "roGroupIndicators::GetIndicatorsDataTable")
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roGroupIndicators::GetIndicatorsDataTable")
                End Try

            End If

            Return oRet
        End Function

        Shared Function SaveGroupIndicators(ByVal _IDGroup As Integer, ByVal _IDsIndicators As Generic.List(Of Integer), ByRef oState As roGroupState,
                                             Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                'Borramos empleados y grupos asignados
                Dim DeleteQuerys() As String = {"@DELETE# FROM GroupIndicators WHERE IDGroup = " & _IDGroup.ToString}
                For Each strSQLDelete As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQLDelete)
                    If Not bolRet Then Exit For
                Next

                'Asignamos los nuevos centros de coste
                If bolRet Then
                    'Asignamos los nuevos grupos
                    Dim i As Integer = 0
                    For i = 0 To _IDsIndicators.Count - 1
                        bolRet = ExecuteSql("@INSERT# INTO GroupIndicators (IDGroup, IDIndicator) VALUES(" & _IDGroup.ToString & "," & _IDsIndicators.Item(i).ToString & ")")
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet And bAudit Then
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroupIndicators::SaveGroupIndicators")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupIndicators::SaveGroupIndicators")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

#End Region

    End Class

End Namespace