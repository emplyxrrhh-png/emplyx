Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Security.Base
Imports Robotics.VTBase.Extensions

Public Class roSDKManager
    Private oState As roSDKState = Nothing

    Public ReadOnly Property State As roSDKState
        Get
            Return oState
        End Get
    End Property

#Region "Constructores"

    Public Sub New()
        oState = New roSDKState()
    End Sub

    Public Sub New(ByVal _State As roSDKState)
        oState = _State
    End Sub

#End Region

#Region "Métodos"

    ''' <summary>
    ''' Carga datos de fichajes
    ''' </summary>
    ''' <param name="Where"></param>
    ''' <param name="oActiveTransaction"></param>
    ''' <param name="bAudit"></param>
    ''' <returns></returns>
    Public Function LoadPunches(Where As String, Optional ByVal bAudit As Boolean = False) As roSDKPunchList
        Dim bolRet As Boolean = True
        Dim response As New roSDKPunchList
        response.Result = {}
        oState.Result = SDKResultEnum.NoError

        Try

            Dim oStatePunch As roPunchState = New roPunchState
            roBusinessState.CopyTo(Me.oState, oStatePunch)
            'TODO: separar el where en varies crides per si es molt llarg?
            Dim tmpTable = roPunch.GetPunches(Where, oStatePunch)
            Dim punchList As New List(Of roSDKPunch)
            If oStatePunch.Result <> PunchResultEnum.NoError Then
                roBusinessState.CopyTo(oStatePunch, oState) 'copia la informacio de error
                bolRet = False
            Else
                For Each oRow As DataRow In tmpTable.Rows
                    Dim NewPunch As roSDKPunch = New roSDKPunch
                    With NewPunch
                        .IdEmployee = oRow.Item("IDEmployee")
                        .IdPunch = oRow.Item("ID")
                        .ShiftDate = oRow.Item("ShiftDate")
                        .DateTime = oRow.Item("DateTime")
                        .Type = oRow.Item("Type")
                        .Terminal = oRow.Item("IDReader")
                    End With
                    punchList.Add(NewPunch)
                Next
                response.Result = punchList.ToArray
            End If
        Catch ex As DbException
            Me.oState.UpdateStateInfo(ex, "roSDKManager::LoadPunches")
        Catch ex As Exception
            Me.oState.UpdateStateInfo(ex, "roSDKManager::LoadPunches")
        End Try

        response.oState = New roWsState
        roWsStateManager.CopyTo(oState, response.oState)

        Return response

    End Function

    ''' <summary>
    ''' procesar fichajes a importar
    ''' </summary>
    ''' <param name="punches"></param>
    ''' <param name="oActiveTransaction"></param>
    ''' <param name="bAudit"></param>
    ''' <returns></returns>
    Public Function ProcessPunches(ByVal punches As List(Of roSDKPunch), ByVal DeletePunchesIDs As String, Optional ByVal bAudit As Boolean = False) As roSDKGenericResponse
        Dim bolRet As Boolean = False
        Dim response As New roSDKGenericResponse
        response.Result = True
        oState.Result = SDKResultEnum.NoError

        Dim bHaveToClose As Boolean = False
        Try
            bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

            Dim oStatePunch As roPunchState = New roPunchState
            roBusinessState.CopyTo(Me.oState, oStatePunch)

            bolRet = True
            For Each id As String In DeletePunchesIDs.Split(",")
                Dim roPunch As New roPunch(Nothing, id, oStatePunch)

                If roPunch IsNot Nothing AndAlso roPunch.DateTime IsNot Nothing Then
                    bolRet = roPunch.Delete(False, True)
                    If Not bolRet Then Exit For
                End If
            Next

            If Not bolRet Then
                If oStatePunch.Result <> PunchResultEnum.NoError Then
                    roBusinessState.CopyTo(oStatePunch, oState) 'copia la informacio de error
                    response.Result = False
                    Return response
                End If
            End If

            If bolRet Then
                For Each p As roSDKPunch In punches
                    Dim roPunch As New roPunch(p.IdEmployee, 0, oStatePunch)
                    roPunch.IDEmployee = p.IdEmployee
                    roPunch.IDTerminal = p.Terminal
                    roPunch.DateTime = p.DateTime
                    roPunch.ShiftDate = p.DateTime.Date
                    If (p.Type = "E") Then
                        roPunch.ActualType = PunchTypeEnum._IN
                        roPunch.Type = PunchTypeEnum._IN
                    End If
                    If (p.Type = "S") Then
                        roPunch.ActualType = PunchTypeEnum._OUT
                        roPunch.Type = PunchTypeEnum._OUT
                    End If

                    roPunch.TypeData = Cause.roCause.GetCauseByShortName(p.Cause, New Cause.roCauseState(-1))
                    roPunch.Source = PunchSource.StandardImport

                    bolRet = roPunch.Save(False, True) 'només que 1 Save() retorni False, bolRet serà false i no farà commit
                    If Not bolRet Then Exit For
                Next
                roConnector.InitTask(TasksType.MOVES) ' per recalcular els saldos, etc

            End If

            If oStatePunch.Result <> PunchResultEnum.NoError Then    'el punch.state estara canviat per referencia
                roBusinessState.CopyTo(oStatePunch, oState) 'copia la informacio de error
                response.Result = False
            End If
        Catch ex As DbException
            Me.oState.UpdateStateInfo(ex, "roSDKManager::InsertPunches")
        Catch ex As Exception
            Me.oState.UpdateStateInfo(ex, "roSDKManager::InsertPunches")
        Finally
            Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
        End Try

        response.oState = New roWsState
        roWsStateManager.CopyTo(oState, response.oState)
        Return response

    End Function

#End Region

End Class