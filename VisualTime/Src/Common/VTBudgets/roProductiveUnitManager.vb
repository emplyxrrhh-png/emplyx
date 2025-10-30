Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace VTBudgets

    Public Class roProductiveUnitManager
        Private oState As roProductiveUnitState = Nothing

        Public ReadOnly Property State As roProductiveUnitState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roProductiveUnitState()
        End Sub

        Public Sub New(ByVal _State As roProductiveUnitState)
            oState = _State
        End Sub

#End Region

#Region "Métodos"

        ''' <summary>
        ''' Carga datos unidad productiva
        ''' </summary>
        ''' <param name="idProductiveUnit"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function LoadProductiveUnit(idProductiveUnit As Integer, Optional ByVal bAudit As Boolean = False, Optional bolLoadShiftHourData As Boolean = False) As roProductiveUnit

            Dim bolRet As New roProductiveUnit

            oState.Result = ProductiveUnitResultEnum.NoError

            bolRet.ID = -1

            If idProductiveUnit > 0 Then

                Try

                    Dim strSQL As String = "@SELECT# * FROM ProductiveUnits " &
                                           "WHERE ID=" & idProductiveUnit.ToString
                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        bolRet.ID = Any2Long(tb.Rows(0)("ID"))
                        bolRet.Name = Any2String(tb.Rows(0)("Name"))
                        bolRet.ShortName = Any2String(tb.Rows(0)("ShortName"))
                        bolRet.Description = Any2String(tb.Rows(0)("Description"))
                        bolRet.Color = Any2Integer(tb.Rows(0)("Color"))
                        bolRet.IDCenter = Any2Integer(tb.Rows(0)("IDCenter"))

                        bolRet.UnitModes = roProductiveUnitModeManager.LoadModesByUnit(idProductiveUnit, Me.oState)

                        If bolLoadShiftHourData Then
                            ' Cargamos el detalle de las tramos de cada posicion de cada modo de la unidad productiva
                            If bolRet.UnitModes IsNot Nothing AndAlso bolRet.UnitModes.Count > 0 Then
                                For Each oUnitMode As roProductiveUnitMode In bolRet.UnitModes
                                    If oUnitMode.UnitModePositions IsNot Nothing AndAlso oUnitMode.UnitModePositions.Count > 0 Then
                                        For Each oUnitModePosition As roProductiveUnitModePosition In oUnitMode.UnitModePositions
                                            oUnitModePosition.ShiftHourData = roProductiveUnitModePositionManager.LoadTheoricLayers(oUnitModePosition, Me.oState)
                                        Next
                                    End If
                                Next
                            End If
                        End If
                    End If

                    If bAudit Then
                        ' Auditar lectura
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{ID}", bolRet.ID, "", 1)
                        oState.AddAuditParameter(tbParameters, "{Name}", bolRet.Name, "", 1)

                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tProductiveUnit, bolRet.Name, tbParameters, -1)
                    End If
                Catch ex As Data.Common.DbException
                    Me.oState.UpdateStateInfo(ex, "roProductiveUnitManager::LoadProductiveUnit")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roProductiveUnitManager::LoadProductiveUnit")
                Finally

                End Try

            End If

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda una unidad productiva
        ''' </summary>
        ''' <param name="oProductiveUnit"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SaveProductiveUnit(oProductiveUnit As roProductiveUnit, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If ValidateProductiveUnit(oProductiveUnit, Me.State) Then

                    Dim tb As New DataTable("ProductiveUnits")
                    Dim strSQL As String = "@SELECT# * FROM ProductiveUnits WHERE ID=" & oProductiveUnit.ID.ToString
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
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    If oProductiveUnit.ID > 0 Then
                        oRow("ID") = oProductiveUnit.ID
                    Else
                        oRow("ID") = GetNextId()
                        oProductiveUnit.ID = oRow("ID")
                    End If

                    oRow("Name") = oProductiveUnit.Name
                    oRow("ShortName") = oProductiveUnit.ShortName
                    oRow("Description") = oProductiveUnit.Description
                    oRow("Color") = oProductiveUnit.Color
                    oRow("IDCenter") = oProductiveUnit.IDCenter

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = roProductiveUnitModeManager.SaveModesByUnit(oProductiveUnit, oState)

                    ' Auditamos
                    If bolRet And bAudit Then
                        oAuditDataNew = oRow
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)

                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Me.oState.Audit(If(bolActionInsert, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tProductiveUnit, oProductiveUnit.Name, tbParameters, -1)

                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitManager::SaveProductiveUnit")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitManager::SaveProductiveUnit")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        '''' <summary>
        '''' Elimina  una unidad productiva
        '''' </summary>
        '''' <param name="oProductiveUnit"></param>
        '''' <param name="oActiveTransaction"></param>
        '''' <param name="bAudit"></param>
        '''' <returns></returns>
        Public Function DeleteProductiveUnit(oProductiveUnit As roProductiveUnit, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = True
            Dim strSQL As String = ""
            Dim bHaveToClose As Boolean = False

            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If bolRet Then
                    'validar que la unidad productiva no este asignada a ningun presupuesto
                    strSQL = "@SELECT# IDProductiveUnit FROM DailyBudgets WHERE IDProductiveUnit=" & oProductiveUnit.ID.ToString
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        State.Result = ProductiveUnitResultEnum.UPExistinBudget
                        bolRet = False
                    End If
                End If

                Dim oProductiveUnitModeManager As New roProductiveUnitModeManager

                ' Eliminamos previamente los modos asociados
                Dim oUnitModes As roProductiveUnitMode() = roProductiveUnitModeManager.LoadModesByUnit(oProductiveUnit.ID, oState)
                For Each oUnitMode As roProductiveUnitMode In oUnitModes
                    bolRet = oProductiveUnitModeManager.DeleteProductiveUnitMode(oUnitMode, True)
                    If Not bolRet Then Exit For
                Next

                If bolRet Then
                    bolRet = ExecuteSql("@DELETE# ProductiveUnits WHERE ID=" & oProductiveUnit.ID)
                End If

                If bolRet And bAudit Then
                    ' Auditamos

                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tProductiveUnit, oProductiveUnit.Name, Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitManager::DeleteProductiveUnit")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitManager::DeleteProductiveUnit")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function GetProductiveUnits(ByVal _State As roProductiveUnitState, Optional ByVal strWhere As String = "") As List(Of roProductiveUnit)

            Dim oRet As New List(Of roProductiveUnit)

            Try

                _State.Result = ProductiveUnitResultEnum.NoError

                Dim strSQL As String
                strSQL = "@SELECT# ID " &
                         "FROM ProductiveUnits " &
                         "WHERE 1=1"

                If strWhere <> "" Then
                    strSQL = strSQL & " AND " & strWhere
                End If
                strSQL = strSQL & " ORDER BY Name"

                Dim tbUnits As DataTable = CreateDataTable(strSQL)
                If (tbUnits IsNot Nothing AndAlso tbUnits.Rows.Count > 0) Then
                    For Each rowunit As DataRow In tbUnits.Rows
                        Dim ProductiveUnit = New roProductiveUnit
                        ProductiveUnit = LoadProductiveUnit(rowunit("Id"))
                        oRet.Add(ProductiveUnit)
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProductiveUnit::GetProductiveUnits")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProductiveUnit::GetProductiveUnits")
            Finally

            End Try

            Return oRet

        End Function

        '''' <summary>
        '''' Resumen de la unidad productiva
        '''' </summary>
        '''' <param name="oProductiveUnit"></param>
        '''' <param name="oActiveTransaction"></param>
        '''' <param name="bAudit"></param>
        '''' <returns></returns>
        Public Function GetProductiveUnitSummary(ByVal _ProductiveUnit As roProductiveUnit, ByVal _ProductiveUnitSummaryType As ProductiveUnitSummaryType) As roProductiveUnitSummary
            Dim bolRet As Boolean = True
            Dim oRet As New roProductiveUnitSummary
            Dim strSQL As String = ""
            Dim oBeginPeriod As Date = Now.Date
            Dim oEndPeriod As Date = Now.Date

            Try
                Select Case _ProductiveUnitSummaryType
                    Case ProductiveUnitSummaryType.Anual
                        ' Año en curso
                        oBeginPeriod = DateSerial(Now.Date.Year, 1, 1)
                        oEndPeriod = oBeginPeriod.AddYears(1).AddDays(-1)
                    Case ProductiveUnitSummaryType.Daily
                        ' Dia actual

                    Case ProductiveUnitSummaryType.Monthly
                        ' Mes actual
                        oBeginPeriod = DateSerial(Now.Date.Year, Now.Date.Month, 1)
                        oEndPeriod = oBeginPeriod.AddMonths(1).AddDays(-1)
                    Case ProductiveUnitSummaryType.Weekly
                        ' Semana actual
                        Dim iDayOfWeek As Integer = Now.Date.DayOfWeek
                        Dim intWeekIniDay As Integer = 1
                        If iDayOfWeek = 0 Then iDayOfWeek = 7
                        If intWeekIniDay > iDayOfWeek Then intWeekIniDay = intWeekIniDay - 7
                        oBeginPeriod = Now.Date.AddDays(intWeekIniDay - iDayOfWeek)
                        oEndPeriod = oBeginPeriod.AddDays(6)
                End Select

                If bolRet Then
                    'validar que la unidad productiva no este asignada a ningun presupuesto
                    strSQL = " @SELECT# count(*) as Total, IDMode, ProductiveUnit_Modes.Name, ProductiveUnit_Modes.Color  FROM DailyBudgets, ProductiveUnit_Modes WHERE DailyBudgets.IDProductiveUnit=" & _ProductiveUnit.ID.ToString
                    strSQL += " AND DailyBudgets.IDProductiveUnit = ProductiveUnit_Modes.IDProductiveUnit "
                    strSQL += " AND DailyBudgets.IDMode = ProductiveUnit_Modes.ID "
                    strSQL += " And Date >= " & roTypes.Any2Time(oBeginPeriod).SQLSmallDateTime
                    strSQL += " And Date <= " & roTypes.Any2Time(oEndPeriod).SQLSmallDateTime
                    strSQL += " Group by IDMode,ProductiveUnit_Modes.Name,ProductiveUnit_Modes.Color"

                    oRet.ProductiveUnitSummary_ModeDetail = Nothing

                    Dim oProductiveUnitSummary_ModeDetailList As New Generic.List(Of roProductiveUnitSummary_ModeDetail)

                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        For Each oRow As DataRow In tb.Rows
                            ' Para cada modo planificado nos guardamos sus totales
                            Dim oProductiveUnitSummary_ModeDetail As New roProductiveUnitSummary_ModeDetail
                            oProductiveUnitSummary_ModeDetail.IDProductiveUnit = _ProductiveUnit.ID
                            oProductiveUnitSummary_ModeDetail.IDMode = Any2Integer(oRow("IDMode"))
                            oProductiveUnitSummary_ModeDetail.ModeName = Any2String(oRow("Name"))
                            oProductiveUnitSummary_ModeDetail.Color = Any2Integer(oRow("Color"))
                            oProductiveUnitSummary_ModeDetail.Quantity = Any2Double(oRow("Total"))
                            oProductiveUnitSummary_ModeDetailList.Add(oProductiveUnitSummary_ModeDetail)
                        Next
                    End If
                    oRet.ProductiveUnitSummary_ModeDetail = oProductiveUnitSummary_ModeDetailList.ToArray
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitManager::GetProductiveUnitSummary")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitManager::GetProductiveUnitSummary")
            End Try

            Return oRet

        End Function

#End Region

#Region "Helper Methods"

        Public Shared Function ValidateProductiveUnit(ByVal oProductiveUnit As roProductiveUnit, ByVal _State As roProductiveUnitState) As Boolean

            Dim bolRet As Boolean = True
            Dim strSQL As String = ""

            Try

                _State.Result = ProductiveUnitResultEnum.NoError

                If bolRet Then
                    'validar que el nombre no esté en blanco y no exista
                    If (Not String.IsNullOrEmpty(oProductiveUnit.Name)) Then
                        strSQL = "@SELECT# ID FROM ProductiveUnits WHERE Name = LTRIM(RTRIM( '" & oProductiveUnit.Name.Replace("'", "''") & "')) AND ID <> " & oProductiveUnit.ID.ToString
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
                    'validar que el nombre corto no esté en blanco y no exista
                    If (Not String.IsNullOrEmpty(oProductiveUnit.ShortName)) Then
                        strSQL = "@SELECT# ID FROM ProductiveUnits WHERE ShortName = LTRIM(RTRIM( '" & oProductiveUnit.ShortName.Replace("'", "''") & "')) AND ID <> " & oProductiveUnit.ID.ToString
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
                _State.UpdateStateInfo(ex, "roProductiveUnit::ValidateProductiveUnit")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProductiveUnit::ValidateProductiveUnit")
            End Try

            Return bolRet

        End Function

        Private Function GetNextId() As Long
            Dim intRet As Long = 0

            Try

                Dim strQry As String = "@SELECT# (ISNULL(MAX(ID), 0) + 1) FROM ProductiveUnits "

                intRet = roTypes.Any2Long(ExecuteScalar(strQry))
            Catch ex As Data.Common.DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnit::GetNextId")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnit::GetNextId")
            End Try

            Return intRet
        End Function

#End Region

    End Class

End Namespace