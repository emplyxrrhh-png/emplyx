Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Base

    Public Class roSecurityCategoryManager
        Private oState As roSecurityCategoryState = Nothing

        Public ReadOnly Property State As roSecurityCategoryState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roSecurityCategoryState()
        End Sub

        Public Sub New(ByVal _State As roSecurityCategoryState)
            Me.oState = _State
        End Sub

#Region "Helpers"

        Public Shared Function LoadSecurityCategories(ByRef oState As roSecurityCategoryState) As Generic.List(Of roSecurityCategory)
            Dim oRet As New Generic.List(Of roSecurityCategory)
            Try

                Dim strSQL As String = "@SELECT# * From sysroCategoryTypes Order by ID"
                Dim dTblCategories As DataTable = CreateDataTable(strSQL)
                If dTblCategories IsNot Nothing AndAlso dTblCategories.Rows.Count > 0 Then
                    For Each oRow As DataRow In dTblCategories.Rows
                        Dim tmpCategory As New roSecurityCategory
                        tmpCategory.ID = oRow("ID")
                        tmpCategory.Description = oState.Language.Translate("CategoryType." & oRow("ID").ToString, "CategoryType") 'oRow("Description")
                        oRet.Add(tmpCategory)
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roSecurityCategorytManager::LoadCategories")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityCategorytManager::LoadCategories")
            End Try

            Return oRet

        End Function

        Public Shared Function LoadRequestTypesWithCategories(ByRef oState As roSecurityCategoryState) As Generic.List(Of roRequestType)
            Dim oRet As New Generic.List(Of roRequestType)

            Try

                Dim strSQL As String = "@SELECT# * From sysroRequestType Order by IdType"
                Dim dTblRequestTypes As DataTable = CreateDataTable(strSQL)
                If dTblRequestTypes IsNot Nothing AndAlso dTblRequestTypes.Rows.Count > 0 Then
                    For Each oRow As DataRow In dTblRequestTypes.Rows
                        Dim tmpRequestType As New roRequestType
                        tmpRequestType.IDType = oRow("IdType")
                        tmpRequestType.TypeDescription = oState.Language.Translate("RequestType." & oRow("Type").ToString, "RequestType")
                        tmpRequestType.IDCategory = IIf(IsDBNull(oRow("IDCategory")) OrElse oRow("IDCategory") = -1, 0, oRow("IDCategory"))
                        tmpRequestType.CategoryOnCause = IIf(Not IsDBNull(oRow("IDCategory")) AndAlso oRow("IDCategory") = -1, True, False)
                        oRet.Add(tmpRequestType)
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roSecurityCategorytManager::LoadRequestTypesWithCategories")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityCategorytManager::LoadRequestTypesWithCategories")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveNotificationTypesWithCategories(ByRef oState As roSecurityCategoryState, ByVal oNotificationTypes As Generic.List(Of roNotificationType), Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                For Each oNotificationType As roNotificationType In oNotificationTypes
                    Dim tb As New DataTable("sysroNotificationTypes")
                    Dim strSQL As String = "@SELECT# * FROM sysroNotificationTypes WHERE Id = " & oNotificationType.Id
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow

                    If (tb.Rows.Count > 0) Then
                        oRow = tb.Rows(0)

                        oRow("IDCategory") = roTypes.Any2Integer(oNotificationType.IDCategory)

                        If tb.Rows.Count = 0 Then tb.Rows.Add(oRow)
                        da.Update(tb)

                        ' Auditamos
                        If bAudit Then
                            oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tNotification, oNotificationType.Name, Nothing, -1)
                        End If
                    End If
                Next

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roSecurityCategorytManager::SaveNotificationTypesWithCategories")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityCategorytManager::SaveNotificationTypesWithCategories")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet

        End Function



#End Region

    End Class

End Namespace