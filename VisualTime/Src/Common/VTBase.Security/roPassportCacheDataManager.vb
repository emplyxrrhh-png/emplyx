Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes
Imports Robotics.VTBase.Scheduler

Namespace Base

    Public Class roPassportCacheDataManager
        Private oState As roPassportState = Nothing

        Public ReadOnly Property State As roPassportState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roPassportState()
        End Sub

        Public Sub New(ByVal idPassport As Integer)
            Me.oState = New roPassportState(idPassport)
        End Sub

        Public Sub New(ByVal state As roPassportState)
            If state Is Nothing Then state = New roPassportState()

            Me.oState = state
        End Sub


#Region "Methods"

        Public Function Load(ByVal idPassport As Integer, ByVal idCacheData As String, selectorType As SeletorType) As String

            Dim oRet As String = String.Empty

            Try

                Dim strSQL As String = "@DELETE# sysroPassports_CacheData WHERE IDPassport = @idPassport and Timestamp < DATEADD(DAY,-15,GETDATE()) And [Type] = @selectorType"


                Dim parameters As New List(Of CommandParameter) From {
                    New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, idPassport),
                    New CommandParameter("@idCacheData", CommandParameter.ParameterType.tString, idCacheData),
                    New CommandParameter("@selectorType", CommandParameter.ParameterType.tInt, CInt(selectorType))
                }

                ExecuteSql(strSQL, parameters)

                strSQL = "@SELECT# Value From sysroPassports_CacheData WHERE IDPassport = @idPassport and IDCacheData = @idCacheData and [Type] = @selectorType"


                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL, parameters)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count = 1 Then
                    ' Cargamos datos generales
                    oRet = dTbl.Rows(0)("value").ToString
                End If

            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPassportCacheDataManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportCacheDataManager::Load")
            End Try

            Return oRet

        End Function

        Public Function Save(ByVal idPassport As Integer, ByVal idCacheData As String, ByVal value As String, selectorType As SeletorType) As Boolean
            Dim bolRet As Boolean = False

            Try


                Dim strSQL As String = $"IF EXISTS (@SELECT# 1 FROM sysroPassports_CacheData WHERE IDPassport = @idPassport and IDCacheData = @idCacheData and [Type] = @selectorType)
                                        BEGIN
                                            @UPDATE# sysroPassports_CacheData SET Value = @value, Timestamp = GETDATE() WHERE IDPassport = @idPassport and IDCacheData = @idCacheData and [Type] = @selectorType
                                        END
                                        ELSE
                                        BEGIN
                                            @INSERT# INTO sysroPassports_CacheData (IDPassport, IDCacheData, Value, Timestamp, [Type]) VALUES (@idPassport, @idCacheData, @value, GETDATE(), @selectorType)
                                        END"

                Dim parameters As New List(Of CommandParameter) From {
                    New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, idPassport),
                    New CommandParameter("@idCacheData", CommandParameter.ParameterType.tString, idCacheData),
                    New CommandParameter("@value", CommandParameter.ParameterType.tString, value),
                    New CommandParameter("@selectorType", CommandParameter.ParameterType.tInt, CInt(selectorType))
                }

                bolRet = ExecuteSql(strSQL, parameters)

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roPassportCacheDataManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPassportCacheDataManager::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(ByVal idPassport As Integer, ByVal idCacheData As String, selectorType As SeletorType) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@DELETE# value From sysroPassports_CacheData WHERE IDPassport = @idPassport and IDCacheData = @idCacheData and [Type] = @selectorType"

                Dim parameters As New List(Of CommandParameter) From {
                    New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, idPassport),
                    New CommandParameter("@idCacheData", CommandParameter.ParameterType.tString, idCacheData),
                    New CommandParameter("@selectorType", CommandParameter.ParameterType.tInt, CInt(selectorType))
                }

                bolRet = ExecuteSql(strSQL, parameters)

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roPassportCacheDataManager::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPassportCacheDataManager::Delete")
            End Try
            Return bolRet

        End Function

#End Region

    End Class

End Namespace