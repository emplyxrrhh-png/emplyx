Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase.Extensions

Namespace VTBudgets

    Public Class roBudgetRowManager
        Private oState As roBudgetRowState = Nothing

        Public Sub New()
            Me.oState = New roBudgetRowState()
        End Sub

        Public Sub New(ByVal _State As roBudgetRowState)
            Me.oState = _State
        End Sub

#Region "Helpers"

        Public Shared Function LoadRowsByBudget(ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime, ByVal _strNodeFilter As String, ByRef oState As roBudgetRowState,
                                                  ByVal _TypeView As BudgetView, ByVal _DetailLevel As BudgetDetailLevel, ByVal bolLicenseHRScheduling As Boolean,
                                                  Optional ByVal _IDProductiveUnit As Integer = -1, Optional ByVal strProductiveUnitFilter As String = "") As roBudgetRow()
            ' Llenamos las filas del presupuesto
            Dim oRet As New Generic.List(Of roBudgetRow)
            Dim bolRet As Boolean = False
            Dim EmployeeIDs As New Generic.List(Of Integer)
            Dim EmployeeNotAllowedIDs As New Generic.List(Of Integer)

            Dim ProductiveUnitIDs As New Generic.List(Of Integer)
            Dim StrProductiveUnits As String = ""

            Dim strFilterNodes As String = ""

            Dim oShiftCache As New Hashtable

            Try

                Dim oParams As New roParameters("OPTIONS", True)

                ' Obtenemos todas las unidades productivas asignadas al presupuesto
                Dim strQuery As String = String.Empty
                strQuery &= " @SELECT# DISTINCT sysroSecurityNodes.Name as NodeName, DailyBudgets.IDNode, DailyBudgets.IDProductiveUnit, ProductiveUnits.Name as ProductiveUnitName, ProductiveUnits.Color as ProductiveUnitColor, ProductiveUnits.ShortName as ProductiveUnitShortName  "
                strQuery &= " FROM DailyBudgets INNER JOIN ProductiveUnits ON DailyBudgets.IDProductiveUnit = ProductiveUnits.ID  INNER JOIN sysroSecurityNodes ON DailyBudgets.IDNode = sysroSecurityNodes.ID "
                strQuery &= " WHERE DailyBudgets.IDNode = " & _strNodeFilter
                If _IDProductiveUnit > 0 Then
                    strQuery &= " AND  DailyBudgets.IDProductiveUnit = " & _IDProductiveUnit.ToString
                End If

                If strProductiveUnitFilter <> String.Empty Then
                    strQuery &= " AND  DailyBudgets.IDProductiveUnit IN (" & strProductiveUnitFilter & ")"
                End If

                strQuery &= " ORDER BY ProductiveUnits.Name "

                Dim dTbl As System.Data.DataTable = CreateDataTable(strQuery)

                Dim oBudgetRow As roBudgetRow = Nothing

                Dim oPermissionPassport As Permission = Permission.None
                oPermissionPassport = WLHelper.GetPermissionOverFeature(oState.IDPassport, "Calendar.Scheduler", "U")

                'Cargar los datos de las unidades productivas seleccionadas
                If dTbl IsNot Nothing Then
                    Dim intPos As Integer = 0

                    For Each oRowUP As DataRow In dTbl.Rows
                        Dim bolAddProductiveUnit As Boolean = True

                        If oPermissionPassport < Permission.Read Then bolAddProductiveUnit = False

                        If bolAddProductiveUnit Then
                            ' Añadimos los datos de cada UP
                            oBudgetRow = New roBudgetRow
                            intPos += 1
                            oBudgetRow.Pos = intPos

                            ' Datos generales de la unidad productiva
                            oBudgetRow.ProductiveUnitData = New roBudgetRowProductiveUnitData
                            oBudgetRow.ProductiveUnitData.IDNode = oRowUP("IDNode")
                            oBudgetRow.ProductiveUnitData.NodeName = oRowUP("NodeName")
                            oBudgetRow.ProductiveUnitData.ProductiveUnit = New roProductiveUnit

                            oBudgetRow.ProductiveUnitData.Permission = oPermissionPassport

                            Dim oProductiveUnitState As New roProductiveUnitState(oState.IDPassport)
                            Dim oProductiveUnitManager As New roProductiveUnitManager(oProductiveUnitState)
                            oBudgetRow.ProductiveUnitData.ProductiveUnit = oProductiveUnitManager.LoadProductiveUnit(oRowUP("IDProductiveUnit"))

                            ' Datos del periodo seleccionado
                            Dim oBudgetRowPeriodDataState As New roBudgetRowPeriodDataState(oState.IDPassport)
                            oBudgetRow.PeriodData = roBudgetRowPeriodDataManager.LoadCellsByBudget(_FirstDay, _LastDay, oRowUP("IDProductiveUnit"), oRowUP("IDNode"), oPermissionPassport, oParams, _TypeView, _DetailLevel, oBudgetRowPeriodDataState, bolLicenseHRScheduling, False, oShiftCache)

                            oRet.Add(oBudgetRow)
                        End If

                    Next
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetRowManager::LoadRowsByBudget")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetRowManager::LoadRowsByBudget")
            End Try

            Return oRet.ToArray

        End Function

#End Region

    End Class

End Namespace