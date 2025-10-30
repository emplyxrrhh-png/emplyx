Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink

    Public Class roDataLinkImportTemplateManager
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

        Public Function Load(ByVal idTemplate As Integer, Optional ByVal bAudit As Boolean = False) As roDatalinkImportGuideTemplate

            Dim bolRet As roDatalinkImportGuideTemplate = Nothing

            Try

                ' Auditar lectura
                If bAudit AndAlso bolRet IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDataLink, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDataLinkImportTemplateManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkImportTemplateManager::Load")
            End Try

            Return bolRet
        End Function

        Public Function Save(ByRef oDatalinkExportGuide As roDatalinkImportGuideTemplate, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try

                Me.oState.Result = DataLinkGuideResultEnum.NoError

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tDataLink, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkImportTemplateManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkImportTemplateManager::Save")
            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal oDatalinkExportGuide As roDatalinkImportGuideTemplate) As Boolean
            Dim bolRet As Boolean = True

            Try

                Me.oState.Result = DataLinkGuideResultEnum.NoError
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkImportTemplateManager::Validate")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkImportTemplateManager::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function GetDataLinkImportGuideTemplates(ByVal oGuide As roDatalinkImportGuide, Optional ByVal bAudit As Boolean = False) As List(Of roDatalinkImportGuideTemplate)
            Dim oGuideTemplates As New List(Of roDatalinkImportGuideTemplate)

            Try
                Dim strSql = "@SELECT# * from ImportGuidesTemplates where IDParentGuide = " & oGuide.Id.ToString

                Dim tbImportGuidesTemplates As New DataTable
                Dim oGuideTemplate As roDatalinkImportGuideTemplate
                tbImportGuidesTemplates = CreateDataTable(strSql)
                If Not tbImportGuidesTemplates Is Nothing AndAlso tbImportGuidesTemplates.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbImportGuidesTemplates.Rows
                        oGuideTemplate = New roDatalinkImportGuideTemplate
                        oGuideTemplate.ID = roTypes.Any2Integer(oRow("ID"))
                        oGuideTemplate.IdParentGuide = oGuide.Id
                        oGuideTemplate.Name = roTypes.Any2String(oRow("Name"))
                        oGuideTemplate.IdName = roTypes.Any2String(oRow("Name"))
                        oGuideTemplate.PreProcessScript = roTypes.Any2String(oRow("PreProcessScript"))
                        oGuideTemplate.TemplateFile = roTypes.Any2String(oRow("Profile"))
                        oGuideTemplate.IsDefault = (oGuide.IdDefaultTemplate = oGuideTemplate.ID)
                        oGuideTemplates.Add(oGuideTemplate)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkManager::GetDataLinkImportGuideTemplates")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkManager::GetDataLinkImportGuideTemplates")
            End Try
            Return oGuideTemplates
        End Function

#End Region

    End Class

End Namespace