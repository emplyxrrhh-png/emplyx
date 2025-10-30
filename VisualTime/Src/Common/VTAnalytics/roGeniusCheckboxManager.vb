Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Analytics.Manager

    <DataContract()>
    Public Class roGeniusCheckboxManager

#Region "Declarations - Constructor"

        Private oState As roAnalyticState

        Public Sub New()
            Me.oState = New roAnalyticState
        End Sub

        Public Sub New(ByVal _State As roAnalyticState)
            Me.oState = _State
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Estado de la solicitud
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <IgnoreDataMember>
        Public Property State() As roAnalyticState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAnalyticState)
                Me.oState = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function GetGeniusCheckboxes(Optional ByVal bAudit As Boolean = False) As Generic.List(Of roGeniusCheckbox)

            Dim oRet As New Generic.List(Of roGeniusCheckbox)

            Try

                Dim strSQL As String = "@SELECT# * from GeniusCustomReportCheckboxes ORDER BY [Order] ASC"

                Dim dTbl As DataTable = CreateDataTable(strSQL)

                If dTbl IsNot Nothing Then
                    Dim oManager As New roGeniusCheckboxManager(Me.State)
                    For Each dRow As DataRow In dTbl.Rows
                        Dim oReportScheduler As roGeniusCheckbox = oManager.Load(dRow("IdCheck"), False, True)
                        If (oReportScheduler IsNot Nothing) Then oRet.Add(oReportScheduler)
                    Next
                End If

                If oRet IsNot Nothing AndAlso oRet.Count > 0 Then
                    If bAudit Then
                        ' Auditamos consulta masiva
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Dim strAuditName As String = String.Empty
                        For Each oView As roGeniusCheckbox In oRet
                            strAuditName &= IIf(strAuditName <> String.Empty, ",", String.Empty) & oView.Name
                        Next
                        Extensions.roAudit.AddParameter(tbAuditParameters, "{GeniusViewNames}", strAuditName, String.Empty, 1)
                        Me.State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tGenius, strAuditName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetGeniusViews")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetGeniusViews")
            Finally

            End Try

            Return oRet

        End Function

        Public Function Load(ByVal _ID As Integer, Optional ByVal bolAudit As Boolean = True, Optional ByVal bTranslateName As Boolean = True, Optional ByVal bIgnorePermissions As Boolean = False) As roGeniusCheckbox

            Dim bolRet As New roGeniusCheckbox With {
                .Id = _ID
            }

            Try

                Dim strSQL As String = "@SELECT# GCH.[IdCheck],[CheckName], GC.Class, gv.RequieredFeature FROM GeniusCustomReportCheckboxes as GCH inner join GeniusCustomReportCombs as GC on GC.IdCheck = GCH.IdCheck inner join GeniusCustomReportViews AS GCV on GCV.Class = GC.Class inner join GeniusViews as GV on GV.Name = GCV.NameViewResult  WHERE GCH.IdCheck = " & bolRet.Id.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                Dim tmpLang As New roLanguage
                tmpLang.SetLanguageReference("LiveGenius", Me.oState.Language.LanguageKey)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    bolRet.Id = roTypes.Any2Integer(oRow("IdCheck"))
                    If bTranslateName = True Then
                        Dim tmpName As String = roTypes.Any2String(oRow("CheckName"))
                        bolRet.Name = tmpLang.Translate("templateName." & tmpName, "")
                        If bolRet.Name.ToUpper.Contains("NOTFOUND") Then bolRet.Name = tmpName
                    Else
                        bolRet.Name = roTypes.Any2String(oRow("CheckName"))
                    End If

                    For Each oRowClass As DataRow In tb.Rows
                        bolRet.RequieredFeature = roTypes.Any2String(oRowClass("RequieredFeature"))
                        If bIgnorePermissions OrElse bolRet.RequieredFeature = String.Empty OrElse Security.WLHelper.HasPermissionOverFeature(oState.IDPassport, bolRet.RequieredFeature, "U", Permission.Read) Then
                            bolRet.Classes.Add(roTypes.Any2String(oRowClass("Class")))
                        End If
                    Next

                    If bolRet.Classes.Count = 0 Then
                        bolRet = Nothing
                    End If

                    If bolRet IsNot Nothing AndAlso bolAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tGenius, bolRet.Name, tbParameters, -1)
                    End If
                Else
                    bolRet = Nothing
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Load")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace