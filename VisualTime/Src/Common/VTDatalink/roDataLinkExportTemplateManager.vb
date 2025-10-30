Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink

    Public Class roDataLinkExportTemplateManager
        Private oState As roDataLinkGuideState = Nothing

        Public ReadOnly Property State As roDataLinkGuideState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roDataLinkGuideState()
        End Sub

        Public Sub New(ByVal _State As roDataLinkGuideState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal idTemplate As Integer, Optional ByVal bAudit As Boolean = False) As roDatalinkExportGuideTemplate

            Dim bolRet As roDatalinkExportGuideTemplate = Nothing

            Try

                ' Auditar lectura
                If bAudit AndAlso bolRet IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDataLink, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDataLinkExportTemplateManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExportTemplateManager::Load")
            End Try

            Return bolRet
        End Function

        Public Function Save(ByRef oDatalinkExportGuide As roDatalinkExportGuideTemplate, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bolIsNew As Boolean = False

            Try

                Me.oState.Result = DataLinkGuideResultEnum.NoError

                bolRet = Validate(oDatalinkExportGuide)

                If bolRet Then
                    Dim strSQL As String = String.Empty
                    Dim tbAux As New DataTable
                    Dim cmd As DbCommand
                    Dim da As DbDataAdapter
                    strSQL = "@SELECT# * from ExportGuidesTemplates where ID = " & oDatalinkExportGuide.ID
                    cmd = CreateCommand(strSQL)
                    da = CreateDataAdapter(cmd, True)

                    da.Fill(tbAux)

                    Dim oRow As DataRow = Nothing
                    If oDatalinkExportGuide.ID <= 0 Then
                        oRow = tbAux.NewRow
                        oRow("ID") = GetNextID()
                        bolIsNew = True
                    ElseIf tbAux.Rows.Count = 1 Then
                        oRow = tbAux.Rows(0)
                        bolIsNew = False
                    End If

                    oRow("IDParentGuide") = oDatalinkExportGuide.IdParentGuide
                    oRow("Name") = oDatalinkExportGuide.Name
                    oRow("Profile") = oDatalinkExportGuide.TemplateFile
                    oRow("Parameters") = DBNull.Value
                    oRow("PostProcessScript") = DBNull.Value
                    oRow("PreProcessScript") = DBNull.Value

                    If bolIsNew <= 0 Then
                        tbAux.Rows.Add(oRow)
                    End If

                    da.Update(tbAux)
                    bolRet = True
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tDataLink, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkExportTemplateManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExportTemplateManager::Save")
            Finally
            End Try

            Return bolRet

        End Function

        Private Function GetNextID() As Integer
            Dim intRet As Integer = 0
            Dim strSQL As String = "@SELECT# MAX(ID) FROM ExportGuidesTemplates "
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = roTypes.Any2Integer(tb.Rows(0).Item(0))
            End If
            Return intRet + 1
        End Function

        Public Function Validate(ByVal oDatalinkExportGuide As roDatalinkExportGuideTemplate) As Boolean
            Dim bolRet As Boolean = True

            Try

                Me.oState.Result = DataLinkGuideResultEnum.NoError

                Dim strSQL As String = "@SELECT# count(ID) from ExportGuidesTemplates where IDParentGuide=" & oDatalinkExportGuide.IdParentGuide & " AND Name='" & oDatalinkExportGuide.Name & "'"

                If roTypes.Any2Integer(ExecuteScalar(strSQL)) > 0 Then
                    bolRet = False
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkImportTemplateManager::Validate")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkImportTemplateManager::Validate")
                bolRet = False
            Finally
            End Try

            Return bolRet

        End Function

        Public Function GetDataLinkExportGuideTemplates(ByVal oGuide As roDatalinkExportGuide, Optional ByVal bAudit As Boolean = False) As List(Of roDatalinkExportGuideTemplate)
            Dim oGuideTemplates As New List(Of roDatalinkExportGuideTemplate)
            Dim tbTemplates As DataTable = Nothing
            Dim strWhere As String = String.Empty

            Try

                Dim strQuery = "@SELECT# * FROM ExportGuidesTemplates where IDParentGuide =" & oGuide.Id

                Dim tbDatalinkTemplates As DataTable = CreateDataTable(strQuery)
                Dim oGuideTemplate As roDatalinkExportGuideTemplate
                If tbDatalinkTemplates IsNot Nothing AndAlso tbDatalinkTemplates.Rows.Count > 0 Then
                    Dim oLang As New roLanguage
                    oLang.SetLanguageReference("LiveDataLink", oState.Language.LanguageKey)

                    For Each oRow As DataRow In tbDatalinkTemplates.Rows
                        oGuideTemplate = New roDatalinkExportGuideTemplate
                        oGuideTemplate.ID = roTypes.Any2Integer(oRow("ID"))
                        oGuideTemplate.IdParentGuide = oGuide.Id

                        Dim oTemplateName As String = roTypes.Any2String(oRow("Name")).Replace("*", "")
                        If roTypes.Any2String(oRow("Name")).IndexOf("*") = -1 Then
                            oTemplateName = oLang.Translate("ProfileExportName." & oTemplateName & ".Text", "DataLinkBusiness")
                        End If

                        oGuideTemplate.Name = oTemplateName
                        oGuideTemplate.IdName = roTypes.Any2String(oRow("Name"))
                        oGuideTemplate.PostProcessScript = roTypes.Any2String(oRow("PostProcessScript"))
                        oGuideTemplate.TemplateFile = roTypes.Any2String(oRow("Profile"))
                        oGuideTemplate.IsDefault = False
                        oGuideTemplates.Add(oGuideTemplate)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkExportTemplateManager::GetDataLinkExportGuideTemplates")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExportTemplateManager::GetDataLinkExportGuideTemplates")
            End Try

            Return oGuideTemplates
        End Function

#End Region

    End Class

End Namespace