Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace VTBudgets

    Public Class roProductiveUnitModeManager
        Private oState As roProductiveUnitState = Nothing

        Public Sub New()
            Me.oState = New roProductiveUnitState()
        End Sub

        Public Sub New(ByVal _State As roProductiveUnitState)
            Me.oState = _State
        End Sub

#Region "Methods"

        ''' <summary>
        ''' Carga datos del modo
        ''' </summary>
        ''' <param name="idProductiveUnitMode"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function LoadProductiveUnitMode(idProductiveUnitMode As Long, Optional ByVal bAudit As Boolean = False) As roProductiveUnitMode

            Dim bolRet As New roProductiveUnitMode

            oState.Result = ProductiveUnitResultEnum.NoError

            bolRet.ID = -1

            If idProductiveUnitMode > 0 Then
                Try
                    Dim strSQL As String = "@SELECT# * FROM ProductiveUnit_Modes " &
                                           "WHERE ID=" & idProductiveUnitMode.ToString
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        bolRet.ID = Any2Long(tb.Rows(0)("ID"))
                        bolRet.IDProductiveUnit = Any2Long(tb.Rows(0)("IDProductiveUnit"))
                        bolRet.Name = Any2String(tb.Rows(0)("Name"))
                        bolRet.ShortName = Any2String(tb.Rows(0)("ShortName"))
                        bolRet.Description = Any2String(tb.Rows(0)("Description"))
                        bolRet.HtmlColor = Drawing.ColorTranslator.ToHtml(Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(tb.Rows(0)("Color"))))
                        bolRet.CostValue = Any2Double(tb.Rows(0)("CostValue"))

                        bolRet.UnitModePositions = roProductiveUnitModePositionManager.LoadPositionsByMode(idProductiveUnitMode, Me.oState)
                    End If

                    If bAudit Then
                        ' Auditar lectura
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{ID}", bolRet.ID, "", 1)
                        oState.AddAuditParameter(tbParameters, "{Name}", bolRet.Name, "", 1)

                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tProductiveUnitMode, bolRet.Name, tbParameters, -1)
                    End If
                Catch ex As Data.Common.DbException
                    Me.oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::LoadProductiveUnitMode")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::LoadProductiveUnitMode")
                End Try

            End If

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda un modo de una unidad productiva
        ''' </summary>
        ''' <param name="oProductiveUnitMode"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SaveProductiveUnitMode(oProductiveUnitMode As roProductiveUnitMode, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If ValidateProductiveUnitMode(oProductiveUnitMode, Me.oState) Then

                    Dim tb As New DataTable("ProductiveUnitMode")
                    Dim strSQL As String = "@SELECT# * FROM ProductiveUnit_Modes WHERE ID=" & oProductiveUnitMode.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim oRow As DataRow

                    Dim bolActionInsert As Boolean = False

                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        bolIsNew = True
                        bolActionInsert = True
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)

                    End If

                    If oProductiveUnitMode.ID > 0 Then
                        oRow("ID") = oProductiveUnitMode.ID
                    Else
                        oRow("ID") = GetNextId()
                        oProductiveUnitMode.ID = oRow("ID")
                    End If

                    oRow("IDProductiveUnit") = oProductiveUnitMode.IDProductiveUnit
                    oRow("Name") = oProductiveUnitMode.Name
                    oRow("ShortName") = oProductiveUnitMode.ShortName
                    oRow("Description") = oProductiveUnitMode.Description
                    oRow("Color") = System.Drawing.ColorTranslator.ToWin32(Drawing.ColorTranslator.FromHtml(oProductiveUnitMode.HtmlColor))
                    oRow("CostValue") = oProductiveUnitMode.CostValue

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    Dim oProductiveUnitModePositionManager As New roProductiveUnitModePositionManager(oState)

                    ' Eliminamos las posiciones que ya no existan en el modo
                    If oProductiveUnitMode IsNot Nothing Then
                        Dim oPositions As roProductiveUnitModePosition() = roProductiveUnitModePositionManager.LoadPositionsByMode(oProductiveUnitMode.ID, oState)
                        For Each oPosition As roProductiveUnitModePosition In oPositions
                            Dim bolExistMode As Boolean = False
                            For Each _ActualPosition As roProductiveUnitModePosition In oProductiveUnitMode.UnitModePositions
                                If oPosition.ID = _ActualPosition.ID Then
                                    bolExistMode = True
                                    Exit For
                                End If
                            Next

                            If Not bolExistMode Then
                                bolRet = oProductiveUnitModePositionManager.DeleteProductiveUnitModePosition(oPosition, True)
                                If Not bolRet Then Exit For
                            End If
                        Next
                    End If

                    ' Guardamos los datos de las posiciones actuales
                    If bolRet Then
                        For Each oUnitModePosition As roProductiveUnitModePosition In oProductiveUnitMode.UnitModePositions
                            oUnitModePosition.IDProductiveUnitMode = oProductiveUnitMode.ID
                            bolRet = oProductiveUnitModePositionManager.SaveProductiveUnitModePosition(oUnitModePosition, True)
                            If Not bolRet Then
                                Exit For
                            End If
                        Next
                    End If

                    ' Auditamos
                    If bolRet And bAudit Then
                        oAuditDataNew = oRow
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)

                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Me.oState.Audit(If(bolActionInsert, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tProductiveUnitMode, oProductiveUnitMode.Name, tbParameters, -1)

                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::SaveProductiveUnitMode")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::SaveProductiveUnitMode")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina  un modo de una unidad productiva
        ''' </summary>
        ''' <param name="oProductiveUnitMode"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function DeleteProductiveUnitMode(oProductiveUnitMode As roProductiveUnitMode, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim strSQL As String
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oUnitModePositionManager As New roProductiveUnitModePositionManager

                bolRet = True

                'validar que el modo de la unidad productiva no este asignada a ningun presupuesto
                strSQL = "@SELECT# IDMode FROM DailyBudgets WHERE IDMode=" & oProductiveUnitMode.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Me.oState.Result = ProductiveUnitResultEnum.UP_ModeExistinBudget
                    bolRet = False
                End If

                If bolRet Then
                    ' Primero eliminamos las posiciones del modo
                    For Each oUnitModePosition As roProductiveUnitModePosition In oProductiveUnitMode.UnitModePositions
                        bolRet = oUnitModePositionManager.DeleteProductiveUnitModePosition(oUnitModePosition, True)
                        If Not bolRet Then Exit For
                    Next
                End If

                ' Posteriormente eliminamos el modo
                If bolRet Then
                    bolRet = ExecuteSql("@DELETE# ProductiveUnit_Modes WHERE ID=" & oProductiveUnitMode.ID)
                End If

                If bolRet And bAudit Then
                    ' Auditamos

                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tProductiveUnitMode, oProductiveUnitMode.Name, Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::DeleteProductiveUnitMode")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::DeleteProductiveUnitMode")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helpers"

        Public Shared Function LoadModesByUnit(ByVal _IDUnit As Double, ByRef oState As roProductiveUnitState) As roProductiveUnitMode()
            ' obtenemos todos los modos de la unidad productiva
            Dim oRet As New Generic.List(Of roProductiveUnitMode)
            Dim bolRet As Boolean = False

            Try
                Dim strQuery As String = String.Empty

                strQuery &= " @SELECT# ID FROM ProductiveUnit_Modes WHERE IDProductiveUnit= " & _IDUnit.ToString
                strQuery &= " ORDER BY Name"

                Dim dTbl As System.Data.DataTable = CreateDataTable(strQuery)

                Dim oProductiveUnitMode As roProductiveUnitMode = Nothing

                'Cargar los datos de los modos
                If dTbl IsNot Nothing Then
                    Dim intPos As Integer = 0

                    For Each oMode As DataRow In dTbl.Rows
                        ' Añadimos los datos del modo
                        oProductiveUnitMode = New roProductiveUnitMode

                        Dim oUnitModeManager As New roProductiveUnitModeManager
                        oProductiveUnitMode = oUnitModeManager.LoadProductiveUnitMode(Any2Long(oMode("ID")))
                        oRet.Add(oProductiveUnitMode)
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::LoadModesByUnit")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::LoadModesByUnit")
            End Try

            Return oRet.ToArray

        End Function

        Public Shared Function ValidateProductiveUnitMode(ByVal oProductiveUnitMode As roProductiveUnitMode, ByVal _State As roProductiveUnitState) As Boolean
            Dim bolRet As Boolean = True
            Dim strSQL As String = ""

            Try

                _State.Result = ProductiveUnitResultEnum.NoError

                If bolRet Then
                    'validar que el nombre no esté en blanco y no exista dentro de la misma unidad productiva
                    If (Not String.IsNullOrEmpty(oProductiveUnitMode.Name)) Then
                        strSQL = "@SELECT# ID FROM ProductiveUnit_Modes WHERE Name = LTRIM(RTRIM( '" & oProductiveUnitMode.Name.Replace("'", "''") & "')) AND ID <> " & oProductiveUnitMode.ID.ToString & " AND IDProductiveUnit=" & oProductiveUnitMode.IDProductiveUnit.ToString
                        Dim tb As DataTable = CreateDataTable(strSQL)
                        If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            _State.Result = ProductiveUnitResultEnum.InvalidName
                            bolRet = False
                        End If
                    Else
                        _State.Result = ProductiveUnitResultEnum.InvalidName
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    'validar que el nombre corto no esté en blanco y no exista dentro de la misma unidad productiva
                    If (Not String.IsNullOrEmpty(oProductiveUnitMode.ShortName)) Then
                        strSQL = "@SELECT# ID FROM ProductiveUnit_Modes WHERE ShortName = LTRIM(RTRIM( '" & oProductiveUnitMode.ShortName.Replace("'", "''") & "')) AND ID <> " & oProductiveUnitMode.ID.ToString & " AND IDProductiveUnit=" & oProductiveUnitMode.IDProductiveUnit.ToString
                        Dim tb As DataTable = CreateDataTable(strSQL)
                        If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            _State.Result = ProductiveUnitResultEnum.InvalidShortName
                            bolRet = False
                        End If
                    Else
                        _State.Result = ProductiveUnitResultEnum.InvalidShortName
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProductiveUnitModeManager::ValidateProductiveUnitMode")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProductiveUnitModeManager::ValidateProductiveUnitMode")
            End Try

            Return bolRet

        End Function

        Private Function GetNextId() As Long
            Dim bolRet As Boolean = True
            Dim intRet As Long = 0

            Try
                Dim strQry As String = "@SELECT# (ISNULL(MAX(ID), 0) + 1) FROM ProductiveUnit_Modes "
                intRet = roTypes.Any2Long(ExecuteScalar(strQry))
            Catch ex As Data.Common.DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::GetNextIDDailyBudget")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::GetNextIDDailyBudget")
            End Try

            Return intRet

        End Function

        Public Shared Function SaveModesByUnit(ByVal _ProductiveUnit As roProductiveUnit, ByRef oState As roProductiveUnitState) As Boolean
            ' Guardamos los modos de la unidad productiva
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oProductiveUnitModeManager As New roProductiveUnitModeManager(oState)

                ' Eliminamos los modos que ya no existan
                If _ProductiveUnit IsNot Nothing Then
                    bolRet = True
                    Dim oUnitModes As roProductiveUnitMode() = roProductiveUnitModeManager.LoadModesByUnit(_ProductiveUnit.ID, oState)
                    For Each oUnitMode As roProductiveUnitMode In oUnitModes
                        Dim bolExistMode As Boolean = False
                        For Each _ActualUnitMode As roProductiveUnitMode In _ProductiveUnit.UnitModes
                            If oUnitMode.ID = _ActualUnitMode.ID Then
                                bolExistMode = True
                                Exit For
                            End If
                        Next

                        If Not bolExistMode Then
                            bolRet = oProductiveUnitModeManager.DeleteProductiveUnitMode(oUnitMode, True)
                            If Not bolRet Then Exit For
                        End If
                    Next
                End If

                If bolRet Then
                    ' Guardamos los nodos existentes
                    If _ProductiveUnit IsNot Nothing AndAlso _ProductiveUnit.UnitModes IsNot Nothing Then
                        For Each oUnitMode As roProductiveUnitMode In _ProductiveUnit.UnitModes
                            oUnitMode.IDProductiveUnit = _ProductiveUnit.ID
                            bolRet = oProductiveUnitModeManager.SaveProductiveUnitMode(oUnitMode, True)
                            If Not bolRet Then Exit For
                        Next
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::LoadModesByUnit")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProductiveUnitModeManager::LoadModesByUnit")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace