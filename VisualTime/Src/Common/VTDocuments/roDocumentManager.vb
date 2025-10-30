Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports DevExpress.DashboardWeb.Native
Imports DevExpress.Pdf
Imports DevExpress.XtraRichEdit.Import.Doc
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.ValidateID
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTDocuments

    Public Class roDocumentManager
        Private Const _SignreportExt = "_signreport"
        Private oState As roDocumentState = Nothing

        Public ReadOnly Property State As roDocumentState
            Get
                Return oState
            End Get
        End Property

        Private Const A3PayrollTemplateName = "A3_PR"

#Region "Constructores"

        Public Sub New()
            oState = New roDocumentState()
        End Sub

        Public Sub New(ByVal _State As roDocumentState)
            oState = _State
        End Sub

#End Region

#Region "Métodos"

        Public Function LoadDocumentTemplateByName(templateName As String, Optional ByVal bAudit As Boolean = False) As roDocumentTemplate
            Dim tempRet As roDocumentTemplate = Nothing

            Dim idTemplate As Integer = 0

            Try

                Dim strSQL As String = "@SELECT# id from documenttemplates where name like '" & templateName & "'"
                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    idTemplate = tb.Rows(0)("Id")
                    tempRet = LoadDocumentTemplate(idTemplate, bAudit)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadDocumentTemplate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadDocumentTemplate")
            End Try

            Return tempRet
        End Function


        Public Function LoadSystemTemplate(scope As DocumentScope) As roDocumentTemplate
            Dim bolRet As roDocumentTemplate = Nothing

            Try
                oState.Result = DocumentResultEnum.NoError

                bolRet = New roDocumentTemplate
                Dim strSQL As String = $"@SELECT# Id FROM DocumentTemplates WHERE Scope = {CInt(scope)} AND IsSystem = 1"
                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    bolRet = LoadDocumentTemplate(roTypes.Any2Integer(tb.Rows(0)("Id")), False)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadSystemTemplate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadSystemTemplate")
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Carga plantilla de documento
        ''' </summary>
        ''' <param name="idDocumentTemplate"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function LoadDocumentTemplate(idDocumentTemplate As Integer, Optional ByVal bAudit As Boolean = False) As roDocumentTemplate
            Dim bolRet As roDocumentTemplate = Nothing

            Try
                oState.Result = DocumentResultEnum.NoError

                bolRet = New roDocumentTemplate
                Dim strSQL As String = "@SELECT# * FROM DocumentTemplates WHERE ID = " & idDocumentTemplate.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    bolRet.Id = oRow("ID")
                    bolRet.Name = oRow("Name")
                    bolRet.ShortName = oRow("ShortName")
                    bolRet.Description = roTypes.Any2String(oRow("Description"))
                    bolRet.Scope = oRow("Scope")
                    Select Case bolRet.Scope
                        Case DocumentScope.Company
                            bolRet.Type = DocumentType.Company
                        Case Else
                            bolRet.Type = DocumentType.Employee
                            'Case DocumentScope.Visit
                            '    bolRet.Type = DocumentType.Visit
                    End Select
                    bolRet.Area = oRow("Area")
                    bolRet.AccessValidation = oRow("AccessValidation")
                    bolRet.BeginValidity = oRow("BeginValidity")
                    bolRet.EndValidity = oRow("EndValidity")
                    bolRet.ApprovalLevelRequired = oRow("ApprovalLevelRequired")
                    bolRet.EmployeeDeliverAllowed = oRow("EmployeeDeliverAllowed")
                    bolRet.SupervisorDeliverAllowed = oRow("SupervisorDeliverAllowed")
                    bolRet.IsSystem = roTypes.Any2Boolean(oRow("IsSystem"))
                    bolRet.DefaultExpiration = roTypes.Any2String(oRow("DefaultExpiration"))
                    bolRet.ExpirePrevious = oRow("ExpirePrevious")
                    bolRet.Compulsory = oRow("Compulsory")
                    bolRet.LOPDAccessLevel = oRow("LOPDAccessLevel")
                    bolRet.DaysBeforeDelete = oRow("DaysBeforeDelete")
                    bolRet.Notifications = roTypes.Any2String(oRow("Notifications"))
                    ' Para documentos de baja, miro si se puede definir el tipo
                    If bolRet.Scope = DocumentScope.LeaveOrPermission AndAlso Not IsDBNull(oRow("LeaveDocType")) Then
                        ' Veo si está en uso en alguna justificación
                        bolRet.LeaveDocumentType = roTypes.Any2Integer(oRow("LeaveDocType"))
                    Else
                        bolRet.LeaveDocumentType = LeaveDocumentType.NotApplicable
                    End If

                    bolRet.RequiresSigning = roTypes.Any2Boolean(oRow("RequiresSigning"))

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{DocumentTemplateName}", bolRet.Name, "", 1)
                        oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDocumentTemplate, bolRet.Name, tbParameters, -1)
                    End If
                Else
                    bolRet.Id = -1
                    bolRet.Scope = DocumentScope.EmployeeContract
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadDocumentTemplate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadDocumentTemplate")
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Carga los detalles de un documento concreto
        ''' </summary>
        ''' <param name="idDocument"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <param name="bCalcWeight"></param>
        ''' <returns></returns>
        Public Function LoadDocument(idDocument As Integer, Optional ByVal bAudit As Boolean = False, Optional bCalcWeight As Boolean = True) As roDocument
            Dim bolRet As roDocument = Nothing

            Try
                oState.Result = DocumentResultEnum.NoError

                bolRet = New roDocument
                Dim strSQL As String

                If bCalcWeight Then
                    strSQL = "@SELECT# Documents.[Id],Documents.[Title],Documents.[IdDocumentTemplate],Documents.[IdEmployee],Documents.[IdCompany],Documents.[IdPunch],Documents.[IdContract],Documents.[IdDaysAbsence]
                              ,Documents.[IdHoursAbsence],Documents.[DocumentType],Documents.[DeliveryDate],Documents.[DeliveryChannel],Documents.[DeliveredBy],Documents.[Status]
                              ,Documents.[StatusLevel],Documents.[LastStatusChange],Documents.[IdLastStatusSupervisor],Documents.[BeginDate],Documents.[EndDate],Documents.[CurrentlyValid]
                              ,Documents.[Remarks],Documents.[IdRequest],Documents.[IdOvertimeForecast],Documents.[SupervisorRemarks],Documents.[BlobFileName]
                              ,Documents.[CRC],Documents.[Received],Documents.[ReceivedDate],Documents.[IdCommunique],Documents.[SignStatus],Documents.[SignDate],Documents.[SignReport], Employees.Name as EmployeeName, len(dbo.f_BinaryToBase64(Documents.Document)) DocLen
                              ,Documents.[DocumentExternalId] FROM Documents left join Employees on Employees.id = Documents.IdEmployee WHERE Documents.ID = " & idDocument.ToString
                Else
                    strSQL = "@SELECT# Documents.[Id],Documents.[Title],Documents.[IdDocumentTemplate],Documents.[IdEmployee],Documents.[IdCompany],Documents.[IdPunch],Documents.[IdContract],Documents.[IdDaysAbsence]
                              ,Documents.[IdHoursAbsence],Documents.[DocumentType],Documents.[DeliveryDate],Documents.[DeliveryChannel],Documents.[DeliveredBy],Documents.[Status]
                              ,Documents.[StatusLevel],Documents.[LastStatusChange],Documents.[IdLastStatusSupervisor],Documents.[BeginDate],Documents.[EndDate],Documents.[CurrentlyValid]
                              ,Documents.[Remarks],Documents.[IdRequest],Documents.[IdOvertimeForecast],Documents.[SupervisorRemarks],Documents.[BlobFileName]
                              ,Documents.[CRC],Documents.[Received],Documents.[ReceivedDate],Documents.[IdCommunique],Documents.[SignStatus],Documents.[SignDate],Documents.[SignReport], Employees.Name as EmployeeName, NULL DocLen 
                              ,Documents.[DocumentExternalId] FROM Documents left join Employees on Employees.id = Documents.IdEmployee WHERE Documents.ID = " & idDocument.ToString
                End If

                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    bolRet.Id = oRow("ID")
                    bolRet.Title = oRow("Title")
                    bolRet.DocumentTemplate = LoadDocumentTemplate(oRow("IdDocumentTemplate"))
                    bolRet.BeginDate = oRow("BeginDate")
                    bolRet.EndDate = oRow("EndDate")
                    bolRet.DeliveredBy = oRow("DeliveredBy")
                    bolRet.DeliveredDate = oRow("DeliveryDate")
                    bolRet.DeliveryChannel = oRow("DeliveryChannel")
                    If roTypes.Any2String(oRow("DocumentType")).Trim <> String.Empty Then
                        Dim rxMatchStr As String = Regex.Match(oRow("DocumentType").Substring(1), "[\[\]\^\$\.\|\?\*\+\(\)\\~`\!@#%&\-_+={}'""<>:;, ]{1,}").ToString
                        bolRet.DocumentType = If(rxMatchStr Is "", oRow("DocumentType"), oRow("DocumentType").Substring(0, oRow("DocumentType").IndexOf(rxMatchStr)))
                    Else
                        bolRet.DocumentType = roTypes.Any2String(oRow("DocumentType"))
                    End If

                    bolRet.IdCompany = IIf(IsDBNull(oRow("IdCompany")), 0, oRow("IdCompany"))
                    bolRet.IdEmployee = IIf(IsDBNull(oRow("IdEmployee")), 0, oRow("IdEmployee"))
                    bolRet.EmployeeName = IIf(IsDBNull(oRow("EmployeeName")), "", oRow("EmployeeName"))
                    bolRet.IdContract = IIf(IsDBNull(oRow("IdContract")), "", oRow("IdContract"))
                    bolRet.IdPunch = IIf(IsDBNull(oRow("IdPunch")), 0, oRow("IdPunch"))
                    bolRet.IdDaysAbsence = IIf(IsDBNull(oRow("IdDaysAbsence")), 0, oRow("IdDaysAbsence"))
                    bolRet.IdHoursAbsence = IIf(IsDBNull(oRow("IdHoursAbsence")), 0, oRow("IdHoursAbsence"))
                    bolRet.IdOvertimeForecast = IIf(IsDBNull(oRow("IdOvertimeForecast")), 0, oRow("IdOvertimeForecast"))
                    bolRet.LastStatusChange = oRow("LastStatusChange")
                    bolRet.IdLastStatusSupervisor = IIf(IsDBNull(oRow("IdLastStatusSupervisor")), 0, oRow("IdLastStatusSupervisor"))
                    bolRet.Status = oRow("Status")
                    bolRet.StatusLevel = oRow("StatusLevel")
                    bolRet.Validity = oRow("CurrentlyValid")
                    bolRet.Received = oRow("Received")
                    bolRet.Weight = roTypes.Any2Integer(oRow("DocLen"))
                    bolRet.ReceivedDate = IIf(IsDBNull(oRow("ReceivedDate")), Nothing, oRow("ReceivedDate"))
                    bolRet.Remarks = roTypes.Any2String(oRow("Remarks"))
                    bolRet.SupervisorRemarks = roTypes.Any2String(oRow("SupervisorRemarks"))
                    bolRet.BlobFileName = roTypes.Any2String(oRow("BlobFileName"))
                    bolRet.IdRequest = IIf(IsDBNull(oRow("IdRequest")), 0, oRow("IdRequest"))
                    bolRet.CRC = roTypes.Any2String(oRow("CRC"))

                    bolRet.SignStatus = IIf(IsDBNull(oRow("SignStatus")), SignStatusEnum.NA, oRow("SignStatus"))

                    bolRet.SignDate = IIf(IsDBNull(oRow("SignDate")), Nothing, oRow("SignDate"))
                    bolRet.DocumentExternalID = roTypes.Any2String(oRow("DocumentExternalId"))

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{DocumentName}", bolRet.Title, "", 1)
                        oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDocumentTemplate, bolRet.Title, tbParameters, -1)
                    End If
                Else
                    bolRet.Id = -1
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadDocument")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadDocument")
            Finally

            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Carga los detalles de un documento concreto
        ''' </summary>
        ''' <param name="idDocument"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function LoadEmployeeDocumentByContent(content As String, idEmployee As Integer, Optional ByVal bAudit As Boolean = False) As roDocument
            Dim bolRet As roDocument = Nothing

            Try
                oState.Result = DocumentResultEnum.NoError

                ' Contenido encriptado
                Dim oData As Byte() = roEncrypt.Encrypt(Convert.FromBase64String(content))
                content = Convert.ToBase64String(oData)

                bolRet = New roDocument
                Dim strSQL As String = String.Empty

                Dim CRC As String = String.Empty
                CRC = CryptographyHelper.EncryptWithMD5(content)
                strSQL = "@SELECT# * FROM Documents WHERE IDEmployee = " & idEmployee.ToString & " AND (CRC = '" & CRC & "' OR dbo.f_BinaryToBase64(Document) = '" & content & "') AND DocumentExternalId IS NULL"

                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    bolRet.Id = oRow("ID")
                    bolRet.Title = oRow("Title")
                    bolRet.DocumentTemplate = LoadDocumentTemplate(oRow("IdDocumentTemplate"))
                    bolRet.BeginDate = oRow("BeginDate")
                    bolRet.EndDate = oRow("EndDate")
                    bolRet.DeliveredBy = oRow("DeliveredBy")
                    bolRet.DeliveredDate = oRow("DeliveryDate")
                    bolRet.DeliveryChannel = oRow("DeliveryChannel")
                    bolRet.DocumentType = oRow("DocumentType")
                    bolRet.IdCompany = IIf(IsDBNull(oRow("IdCompany")), 0, oRow("IdCompany"))
                    bolRet.IdEmployee = IIf(IsDBNull(oRow("IdEmployee")), 0, oRow("IdEmployee"))
                    bolRet.IdContract = IIf(IsDBNull(oRow("IdContract")), "", oRow("IdContract"))
                    bolRet.IdPunch = IIf(IsDBNull(oRow("IdPunch")), 0, oRow("IdPunch"))
                    bolRet.IdDaysAbsence = IIf(IsDBNull(oRow("IdDaysAbsence")), 0, oRow("IdDaysAbsence"))
                    bolRet.IdHoursAbsence = IIf(IsDBNull(oRow("IdHoursAbsence")), 0, oRow("IdHoursAbsence"))
                    bolRet.IdOvertimeForecast = IIf(IsDBNull(oRow("IdOvertimeForecast")), 0, oRow("IdOvertimeForecast"))
                    bolRet.LastStatusChange = oRow("LastStatusChange")
                    bolRet.IdLastStatusSupervisor = IIf(IsDBNull(oRow("IdLastStatusSupervisor")), 0, oRow("IdLastStatusSupervisor"))
                    bolRet.Status = oRow("Status")
                    bolRet.StatusLevel = oRow("StatusLevel")
                    bolRet.Validity = oRow("CurrentlyValid")
                    bolRet.Remarks = roTypes.Any2String(oRow("Remarks"))
                    bolRet.SupervisorRemarks = roTypes.Any2String(oRow("SupervisorRemarks"))
                    bolRet.BlobFileName = roTypes.Any2String(oRow("BlobFileName"))
                    bolRet.CRC = roTypes.Any2String(oRow("CRC"))
                    bolRet.DocumentExternalID = roTypes.Any2String(oRow("DocumentExternalId"))
                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{DocumentName}", bolRet.Title, "", 1)
                        oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDocumentTemplate, bolRet.Title, tbParameters, -1)
                    End If
                Else
                    bolRet.Id = -1
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadDocument")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadDocument")
            Finally

            End Try
            Return bolRet
        End Function


        ''' <summary>
        ''' Carga los detalles de un documento concreto por el DocumentExternalId
        ''' </summary>
        ''' <param name="externalId"></param>
        ''' <param name="idEmployee"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function LoadDocumentByExternalId(externalId As String, Optional ByVal bAudit As Boolean = False) As roDocument
            Dim document As roDocument = Nothing

            Try
                oState.Result = DocumentResultEnum.NoError
                document = New roDocument

                If externalId.Trim = String.Empty Then
                    document.Id = -1
                    Return document
                End If

                Dim strSQL As String = $"@SELECT# Id FROM Documents WHERE DocumentExternalId = '{externalId}'"

                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    document = LoadDocument(oRow("Id"))
                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{DocumentName}", document.Title, "", 1)
                        oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDocumentTemplate, document.Title, tbParameters, -1)
                    End If
                Else
                    document.Id = -1
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadDocumentByExternalId")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::LoadDocumentByExternalId")
            End Try
            Return document
        End Function

        ''' <summary>
        ''' Devuelve todos los documentos que se deben exigir a empleados o empresas en global, o para un empleado en concreto, o para una empresa en concreto
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="idEmployee"></param>
        ''' <param name="idCompany"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Public Function GetPRLDocumentationFaults(Optional type As DTOs.DocumentType = -1, Optional idEmployee As Integer = 0, Optional idCompany As Integer = 0, Optional iAccessValidation As DocumentAccessValidation = -1) As List(Of roDocument)
            ' TODO
            Return New List(Of roDocument)
        End Function

        ''' <summary>
        ''' Valida que la información de la plantilla de documento sea correcta
        ''' </summary>
        ''' <param name="oDocumentTemplate"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="isDeleting"></param>
        ''' <returns></returns>
        Public Function ValidateDocumentTemplate(oDocumentTemplate As roDocumentTemplate, oExistingDocTemplate As roDocumentTemplate, isDeleting As Boolean, isUpdating As Boolean) As Boolean
            Dim bolRet As Boolean = True

            Try
                oState.Result = DocumentResultEnum.NoError

                If isUpdating Then
                    ' No se puede cambiar el ámbito si ya existen documentos del tipo
                    If Not oExistingDocTemplate Is Nothing AndAlso oExistingDocTemplate.Scope <> oDocumentTemplate.Scope Then
                        oState.Result = DocumentResultEnum.TemplateInUse
                        bolRet = False
                    End If
                End If

                If bolRet AndAlso (Not isDeleting) Then
                    'validar que el nombre no esté en blanco y no exista
                    If (Not String.IsNullOrEmpty(oDocumentTemplate.Name)) Then
                        If ExistsDocumentTemplateName(oDocumentTemplate) Then
                            oState.Result = DocumentResultEnum.InvalidName
                            bolRet = False
                        End If
                    Else
                        oState.Result = DocumentResultEnum.EmptyName
                        bolRet = False
                    End If
                End If

                If bolRet AndAlso (Not isDeleting) Then
                    'validar que el nombre corto no esté en blanco y no exista
                    If (Not String.IsNullOrEmpty(oDocumentTemplate.ShortName)) Then
                        If ExistsDocumentTemplateExportName(oDocumentTemplate) Then
                            oDocumentTemplate.ShortName = oDocumentTemplate.Id.ToString
                            'oState.Result = DocumentResultEnum.InvalidShortName
                            'bolRet = False
                        End If
                    Else
                        oDocumentTemplate.ShortName = oDocumentTemplate.Id.ToString
                        'oState.Result = DocumentResultEnum.EmptyShortName
                        'bolRet = False
                    End If
                End If

                If bolRet AndAlso (isDeleting) Then
                    'No se puede eliminar una plantilla de documento si ya hay algún documento de este tipo en la biblioteca
                    If IsTemplateDocumentInUse(oDocumentTemplate.Id) Then
                        oState.Result = DocumentResultEnum.TemplateInUse
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::ValidateDocumentTemplate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::ValidateDocumentTemplate")
            Finally

            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Guarda una plantilla de documento
        ''' </summary>
        ''' <param name="oDocumentTemplate"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SaveDocumentTemplate(ByRef oDocumentTemplate As roDocumentTemplate, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bolActionInsert As Boolean = False
            Dim oExistingDocTemplate As roDocumentTemplate = Nothing
            Dim oUserField As VTUserFields.UserFields.roUserField
            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(oDocumentTemplate) Then
                    Me.oState.Result = DocumentResultEnum.XSSvalidationError
                    Return False
                End If


                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                'Creamos o actualizamos una plantilla de documento
                If (oDocumentTemplate.Id.Equals(-1)) Then
                    oDocumentTemplate.Id = GetNextDocumentTemplateID()
                    bolActionInsert = True
                Else
                    oExistingDocTemplate = LoadDocumentTemplate(oDocumentTemplate.Id)
                End If

                ' Vaalidamos datos
                bolRet = ValidateDocumentTemplate(oDocumentTemplate, oExistingDocTemplate, False, Not bolActionInsert)

                If bolRet Then
                    bolRet = SaveDocumentTemplateData(oDocumentTemplate)
                    'Si la plantilla aplica a ficha de empleado, creo el campo correspondiente ahora
                    If bolRet AndAlso bolActionInsert AndAlso oDocumentTemplate.Scope = DocumentScope.EmployeeField Then
                        oUserField = New VTUserFields.UserFields.roUserField(oState.IDPassport)
                        oUserField.FieldName = oDocumentTemplate.Name
                        oUserField.Type = Types.EmployeeField
                        oUserField.isSystem = False
                        oUserField.Category = ""
                        oUserField.DocumentTemplateId = oDocumentTemplate.Id
                        oUserField.FieldType = FieldTypes.tDocument
                        oUserField.Used = True
                        'oUserField.RequestPermissions = 1 ' no se puede solictar desde el portal
                        Dim sCategoryDocuments As String = String.Empty
                        sCategoryDocuments = oState.Language.Translate("roDocumentManager.UserFields.Category", "")
                        oUserField.Category = IIf(sCategoryDocuments.IndexOf("UNDEFINED LANGUAGE") > -1, "Documentos", sCategoryDocuments)
                        oUserField.AccessLevel = oDocumentTemplate.LOPDAccessLevel
                        bolRet = oUserField.Save(True)
                    ElseIf bolRet AndAlso oDocumentTemplate.Scope = DocumentScope.EmployeeField AndAlso Not oExistingDocTemplate Is Nothing AndAlso oExistingDocTemplate.Name <> oDocumentTemplate.Name Then
                        ' Han cambiado el nombre de la plantilla. Debo cambiar el nombre del campo de la ficha
                        oUserField = New VTUserFields.UserFields.roUserField(New VTUserFields.UserFields.roUserFieldState, oExistingDocTemplate.Name, Types.EmployeeField, False)
                        oUserField.FieldName = oDocumentTemplate.Name
                        bolRet = oUserField.Save()
                    End If
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(If(bolActionInsert, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tDocumentTemplate, oDocumentTemplate.Name, tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SaveDocumentTemplate")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SaveDocumentTemplate")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Auxiliar para guardar los datos de una plantilla de documento
        ''' </summary>
        ''' <param name="oDocumentTemplate"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function SaveDocumentTemplateData(oDocumentTemplate As roDocumentTemplate) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim bolNotifyBroadCaster As Boolean = False
                Dim tb As New DataTable("DocumentTemplate")
                Dim strSQL As String = "@SELECT# * FROM DocumentTemplates WHERE Id = " & oDocumentTemplate.Id.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If (tb.Rows.Count.Equals(0)) Then
                    oRow = tb.NewRow
                    bolNotifyBroadCaster = (oDocumentTemplate.Scope = DocumentScope.EmployeeAccessAuthorization OrElse oDocumentTemplate.Scope = DocumentScope.CompanyAccessAuthorization)
                Else
                    oRow = tb.Rows(0)
                    bolNotifyBroadCaster = DocumentTemplateNotifyBroadCaster(oRow, oDocumentTemplate)
                End If

                oRow("ID") = oDocumentTemplate.Id
                oRow("Name") = oDocumentTemplate.Name
                oRow("ShortName") = oDocumentTemplate.ShortName
                oRow("Description") = oDocumentTemplate.Description
                oRow("Scope") = oDocumentTemplate.Scope
                oRow("Area") = oDocumentTemplate.Area
                oRow("BeginValidity") = oDocumentTemplate.BeginValidity
                oRow("EndValidity") = oDocumentTemplate.EndValidity
                oRow("AccessValidation") = oDocumentTemplate.AccessValidation
                oRow("ApprovalLevelRequired") = oDocumentTemplate.ApprovalLevelRequired
                oRow("EmployeeDeliverAllowed") = oDocumentTemplate.EmployeeDeliverAllowed
                oRow("SupervisorDeliverAllowed") = oDocumentTemplate.SupervisorDeliverAllowed
                oRow("IsSystem") = oDocumentTemplate.IsSystem
                oRow("DefaultExpiration") = oDocumentTemplate.DefaultExpiration
                oRow("ExpirePrevious") = oDocumentTemplate.ExpirePrevious
                oRow("Compulsory") = oDocumentTemplate.Compulsory
                oRow("LOPDAccessLevel") = oDocumentTemplate.LOPDAccessLevel
                oRow("DaysBeforeDelete") = oDocumentTemplate.DaysBeforeDelete
                oRow("Notifications") = oDocumentTemplate.Notifications
                oRow("RequiresSigning") = oDocumentTemplate.RequiresSigning

                If oDocumentTemplate.Scope <> DTOs.DocumentScope.LeaveOrPermission Then oRow("LeaveDocType") = DTOs.LeaveDocumentType.NotApplicable

                If tb.Rows.Count = 0 Then tb.Rows.Add(oRow)
                da.Update(tb)
                bolRet = True

                If bolRet AndAlso bolNotifyBroadCaster Then roConnector.InitTask(TasksType.BROADCASTER)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::SaveDocumentTemplateData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::SaveDocumentTemplateData")
            Finally

            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Auxiliar para guardar la inforamción de un documento entregado
        ''' </summary>
        ''' <param name="oDocument"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function SaveDocumentData(oDocument As roDocument) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = True

            Try

                Dim bolNotifyBroadCaster As Boolean = False

                Dim levelAuthoritySup As Integer
                If oState.IDPassport > 0 Then
                    levelAuthoritySup = GetPassportLevelOfAuthority(oState.IDPassport, oDocument.DocumentTemplate.Area)
                End If

                Dim tb As New DataTable("Documents")
                Dim strSQL As String = "@SELECT# * FROM Documents WHERE Id = " & oDocument.Id.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If (tb.Rows.Count.Equals(0)) Then
                    oRow = tb.NewRow
                Else
                    oRow = tb.Rows(0)
                    bolIsNew = False
                End If

                ' Validaciones al guardar un documento
                ' 0. Si no require aprobación, lo pongo en estado aprobado
                If oDocument.DocumentTemplate.ApprovalLevelRequired = 0 AndAlso oDocument.Status = DocumentStatus.Pending Then
                    oDocument.Status = DocumentStatus.Validated
                End If

                ' 1. Para documentos adjuntados a solicitudes de previsión, si la solicitud está aprobada y no se informó la previsión, lo hago ahora
                If oDocument.IdRequest > 0 AndAlso (oDocument.DocumentTemplate.Scope = DocumentScope.LeaveOrPermission OrElse oDocument.DocumentTemplate.Scope = DocumentScope.CauseNote) AndAlso oDocument.IdDaysAbsence = 0 AndAlso oDocument.IdHoursAbsence = 0 AndAlso oDocument.IdOvertimeForecast = 0 Then

                    Dim sSQLAux As String
                    Dim idForecast As Integer
                    Dim idRequestType As Integer

                    sSQLAux = "@SELECT# RequestType FROM Requests WHERE Id = " & oDocument.IdRequest.ToString
                    idRequestType = VTBase.roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(sSQLAux))

                    Try
                        Select Case idRequestType
                            Case eRequestType.PlannedAbsences
                                sSQLAux = "@SELECT# ProgrammedAbsences.AbsenceId FROM ProgrammedAbsences " &
                                                        "INNER JOIN Requests ON ProgrammedAbsences.IdCause = Requests.IDCause " &
                                                        "AND ProgrammedAbsences.BeginDate = Requests.Date1 " &
                                                        "AND ProgrammedAbsences.IDEmployee = Requests.IDEmployee " &
                                                        "WHERE Requests.Id = " & oDocument.IdRequest.ToString
                                idForecast = VTBase.roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(sSQLAux))
                                If idForecast > 0 Then oDocument.IdDaysAbsence = idForecast
                            Case eRequestType.PlannedCauses
                                sSQLAux = "@SELECT# ProgrammedCauses.AbsenceId FROM ProgrammedCauses " &
                                                        "INNER Join Requests ON ProgrammedCauses.IdCause = Requests.IDCause  " &
                                                        "And ProgrammedCauses.Date = Requests.Date1 " &
                                                        "And ProgrammedCauses.BeginTime = Requests.FromTime " &
                                                        "And ProgrammedCauses.IDEmployee = Requests.IDEmployee " &
                                                        "WHERE Requests.Id = " & oDocument.IdRequest.ToString
                                idForecast = VTBase.roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(sSQLAux))
                                If idForecast > 0 Then oDocument.IdHoursAbsence = idForecast
                            Case eRequestType.PlannedOvertimes
                                sSQLAux = "@SELECT# ProgrammedOvertimes.Id FROM ProgrammedOvertimes " &
                                                        "INNER JOIN Requests ON ProgrammedOvertimes.IdCause = Requests.IDCause " &
                                                        "And ProgrammedOvertimes.BeginDate = Requests.Date1 " &
                                                        "AND ProgrammedOvertimes.BeginTime = Requests.FromTime " &
                                                        "And ProgrammedOvertimes.IDEmployee = Requests.IDEmployee " &
                                                        "WHERE Requests.Id = " & oDocument.IdRequest.ToString
                                idForecast = VTBase.roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(sSQLAux))
                                If idForecast > 0 Then oDocument.IdOvertimeForecast = idForecast
                        End Select
                    Catch ex As Exception
                        ' TODO: Documento no asociado a su previsión
                    End Try
                End If

                ' 2. Para documentación de ausencias por días, la fecha de referencia es, si existe, la fecha de fin de la ausencia
                Dim refDate As Date = Now.Date
                If oDocument.IdDaysAbsence > 0 Then
                    Dim oPAState As New Absence.roProgrammedAbsenceState
                    Dim oPA As New Absence.roProgrammedAbsence(oDocument.IdDaysAbsence.ToString, oPAState)
                    If oPA IsNot Nothing AndAlso oPA.BeginDate.HasValue Then
                        If oDocument.EndDate >= oPA.RealFinishDate Then
                            refDate = oPA.RealFinishDate
                        End If
                    End If
                End If

                ' 2.2 Para documentación de ausencias por horas, la fecha de referencia es, si existe, la fecha de fin de la ausencia
                If oDocument.IdHoursAbsence > 0 Then
                    Dim oPCState As New Incidence.roProgrammedCauseState
                    Dim oPC As New Incidence.roProgrammedCause(oDocument.IdHoursAbsence.ToString, oPCState)
                    If oPC IsNot Nothing AndAlso oPC.BeginDate.HasValue Then
                        If oDocument.EndDate >= oPC.RealFinishDate Then
                            refDate = oPC.RealFinishDate
                        End If
                    End If
                End If

                ' 3.- Trato posible cambio de fecha de caducidad
                If (oDocument.Status = DocumentStatus.Validated AndAlso oDocument.EndDate < refDate) Then
                    oDocument.Status = DocumentStatus.Expired
                ElseIf (oDocument.Status = DocumentStatus.Expired AndAlso oDocument.EndDate >= refDate) Then
                    oDocument.Status = DocumentStatus.Validated
                End If

                ' 4. Aplico caducidad por defecto, si no se definió fecha de caducidad, y aplica
                If oDocument.EndDate = DateSerial(2079, 1, 1) Then
                    ' Si ha pasado a aprobado, aplico validez por defecto si toca ...
                    If oDocument.Status = DocumentStatus.Validated AndAlso oDocument.DocumentTemplate.DefaultExpiration.Contains("@") Then
                        oDocument.EndDate = CalculateDefaultExpirationDate(oDocument.DocumentTemplate.DefaultExpiration, oDocument.DeliveredDate)
                        ' Si ya ha caducado, lo cambio de estado
                        If oDocument.EndDate < refDate Then oDocument.Status = DocumentStatus.Expired 'Me.ChangeDocumentState(oDocument.Id, DocumentStatus.Expired, Now.Date)
                    End If
                End If

                ' 5. Calculo estado de validez del documento a día de hoy
                oDocument.Validity = IsDocumentCurrentlyValid(oDocument, refDate)

                ' 6. Validamos en el caso que sea un nuevo documento que requiera firma digital, que no nos hayamos pasado del maximo
                If bolIsNew AndAlso oDocument.DocumentTemplate IsNot Nothing AndAlso oDocument.DocumentTemplate.RequiresSigning Then
                    ' Obtenemos el nº total de documentos signados actualmente + 1
                    Dim mCount As Long = GetTotalSignedDocumentsOnYear(Now.Year, oState) + 1

                    Dim oServerLicense As New roServerLicense
                    If Not oServerLicense.FeatureIsInstalled("Feature\Signdocument") OrElse mCount > roTypes.Any2Long(oServerLicense.FeatureData("VisualTime Server", "MaxSigndocuments")) Then
                        bolRet = False
                        oState.Result = DocumentResultEnum.NumberOfSignedDocumentsExceeded
                        Return bolRet
                    End If
                End If

                ' 7. Documento que requiere aprobación y al ser aprobado invalida el resto de documentos del mismo tipo
                If oDocument.DocumentTemplate.ExpirePrevious AndAlso oDocument.Validity = DocumentValidity.CurrentlyValid Then
                    bolRet = InvalidateDocumentsOnNewApproval(oDocument)
                End If

                If bolIsNew Then
                    bolNotifyBroadCaster = (oDocument.DocumentTemplate.Scope = DocumentScope.EmployeeAccessAuthorization OrElse oDocument.DocumentTemplate.Scope = DocumentScope.CompanyAccessAuthorization)
                Else
                    bolNotifyBroadCaster = DocumentNotifyBroadCaster(oRow, oDocument)
                End If

                ' Documentos que vienen de Portal de empleado web vienen con la ruta completa!
                Try
                    If oDocument.Title.Contains("\") Then
                        oDocument.Title = oDocument.Title.Split("\")(oDocument.Title.Split("\").Count - 1)
                    End If
                Catch ex As Exception
                End Try

                oRow("ID") = oDocument.Id
                oRow("Title") = oDocument.Title
                oRow("IdDocumentTemplate") = oDocument.DocumentTemplate.Id
                oRow("IdEmployee") = oDocument.IdEmployee
                If bolIsNew AndAlso oDocument.DocumentTemplate.Scope = DocumentScope.EmployeeContract Then
                    Dim oContractState As New Contract.roContractState(oState.IDPassport)
                    Dim oContract As Contract.roContract = Contract.roContract.GetActiveContract(oDocument.IdEmployee, oContractState, False)
                    If oContract IsNot Nothing Then
                        oDocument.IdContract = oContract.IDContract
                    Else
                        Dim lastContract = Contract.roContract.GetLastContract(oDocument.IdEmployee, oContractState, False)
                        If lastContract IsNot Nothing Then
                            oDocument.IdContract = lastContract.IDContract
                        Else
                            bolRet = False
                            oState.Result = DocumentResultEnum.ContractRequired
                            Return bolRet
                        End If
                    End If
                End If
                oRow("IdContract") = oDocument.IdContract
                oRow("IdCompany") = oDocument.IdCompany
                oRow("IdPunch") = oDocument.IdPunch
                oRow("IdHoursAbsence") = oDocument.IdHoursAbsence
                oRow("IdDaysAbsence") = oDocument.IdDaysAbsence
                oRow("IdCommunique") = If(oDocument.IdCommunique > 0, oDocument.IdCommunique, System.DBNull.Value)
                oRow("IdOvertimeForecast") = oDocument.IdOvertimeForecast
                If oDocument.Document IsNot Nothing AndAlso oDocument.DeliveryChannel.ToUpper <> "TRASPASOGPA" Then
                    ' Si guardo el documento en un Blob ...
                    Dim strDocumentBlobName As String = String.Empty
                    Dim fileCRC As String = String.Empty
                    If SaveDocumentOnAzure(oDocument, strDocumentBlobName, fileCRC) Then
                        oRow("BlobFileName") = strDocumentBlobName
                        oRow("CRC") = fileCRC
                        oRow("Document") = Convert.FromBase64String("")
                    Else
                        Return False
                    End If

                    If oDocument.SignReport IsNot Nothing AndAlso oDocument.DocumentTemplate.RequiresSigning Then
                        ' En caso necesario, guardamos el informe de evidencias de la firma digital del documento
                        ' Si guardo el documento en un Blob ...
                        If SaveSignReportOnAzure(oDocument, strDocumentBlobName, fileCRC) Then
                            oRow("SignReport") = Convert.FromBase64String("")
                        Else
                            Return False
                        End If
                    End If

                End If

                Dim rxMatchStr As String = Regex.Match(oDocument.DocumentType.Substring(1), "[\[\]\^\$\.\|\?\*\+\(\)\\~`\!@#%&\-_+={}'""<>:;, ]{1,}").ToString
                oRow("DocumentType") = If(rxMatchStr Is "", oDocument.DocumentType, oDocument.DocumentType.Substring(0, oDocument.DocumentType.IndexOf(rxMatchStr)))
                oRow("DeliveryDate") = oDocument.DeliveredDate
                oRow("DeliveryChannel") = oDocument.DeliveryChannel
                oRow("DeliveredBy") = oDocument.DeliveredBy
                oRow("Status") = oDocument.Status
                If oState.IDPassport > -1 Then
                    oRow("StatusLevel") = levelAuthoritySup
                    oRow("IdLastStatusSupervisor") = oState.IDPassport
                End If
                oRow("LastStatusChange") = oDocument.LastStatusChange
                oRow("BeginDate") = oDocument.BeginDate
                oRow("EndDate") = oDocument.EndDate
                oRow("CurrentlyValid") = oDocument.Validity
                oRow("Remarks") = If(oDocument.Remarks Is Nothing, DBNull.Value, oDocument.Remarks)
                oRow("SupervisorRemarks") = If(oDocument.SupervisorRemarks Is Nothing, DBNull.Value, oDocument.SupervisorRemarks)
                oRow("IdRequest") = If(oDocument.IdRequest <= 0, DBNull.Value, oDocument.IdRequest)

                If bolIsNew AndAlso oDocument.DocumentTemplate IsNot Nothing AndAlso oDocument.DocumentTemplate.RequiresSigning Then
                    oRow("SignStatus") = SignStatusEnum.Pending
                Else
                    oRow("SignStatus") = oDocument.SignStatus
                End If

                oRow("SignDate") = If(oDocument.SignDate Is Nothing, DBNull.Value, oDocument.SignDate)
                oRow("DocumentExternalId") = If(String.IsNullOrEmpty(oDocument.DocumentExternalID), DBNull.Value, oDocument.DocumentExternalID)

                If tb.Rows.Count = 0 Then tb.Rows.Add(oRow)
                da.Update(tb)
                bolRet = True

                'Lanzo Broadcaster en caso necesario
                If bolRet AndAlso bolNotifyBroadCaster Then roConnector.InitTask(TasksType.BROADCASTER)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager:: SaveDocumentData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::SaveDocumentData")
            Finally

            End Try

            Return bolRet
        End Function

        Public Function SaveDocumentOnAzure(oDocument As roDocument, ByRef strDocumentBlobName As String, ByRef fileCRC As String) As Boolean
            Dim bRet As Boolean = False
            Try
                fileCRC = CryptographyHelper.EncryptWithMD5(Convert.ToBase64String(roEncrypt.Encrypt(oDocument.Document)))
                strDocumentBlobName = "DocumentManager#" & fileCRC & "#" & oDocument.Id & "#" & DateTime.Now.ToString("yyyyMMddHHmmss") & oDocument.DocumentType
                Dim ms As New System.IO.MemoryStream(roEncrypt.Encrypt(oDocument.Document))

                bRet = Azure.RoAzureSupport.UploadDocument2Azure(ms, strDocumentBlobName, True)
            Catch ex As Exception
                bRet = False
            End Try
            Return bRet
        End Function

        Public Function SaveSignReportOnAzure(oDocument As roDocument, ByVal strDocumentBlobName As String, ByVal fileCRC As String) As Boolean
            Dim bRet As Boolean = False
            Try
                strDocumentBlobName = strDocumentBlobName & _SignreportExt
                Dim ms As New System.IO.MemoryStream(roEncrypt.Encrypt(oDocument.SignReport))

                bRet = Azure.RoAzureSupport.UploadDocument2Azure(ms, strDocumentBlobName, True)
            Catch ex As Exception
                bRet = False
            End Try
            Return bRet
        End Function

        ''' <summary>
        ''' Determina si un documento aprobado es realmente válido. Si no lo es, devuelve el motivo
        ''' </summary>
        ''' <param name="oDocument"></param>
        ''' <param name="dDate"></param>
        ''' <returns></returns>
        Public Function IsDocumentCurrentlyValid(oDocument As roDocument, dDate As Date) As DTOs.DocumentValidity
            Dim oRet As DTOs.DocumentValidity
            Try
                ' Requiere validación
                If oDocument.DocumentTemplate.ApprovalLevelRequired > 0 Then
                    oRet = DocumentValidity.CheckPending
                    Select Case oDocument.Status
                        Case DocumentStatus.Validated
                            ' Estado validado
                            ' Por un supervisor con suficiente nivel de mando
                            If oDocument.StatusLevel <= oDocument.DocumentTemplate.ApprovalLevelRequired Then
                                ' Está vigente en la fecha indicada
                                If oDocument.BeginDate <= dDate.Date AndAlso oDocument.EndDate >= dDate.Date Then
                                    oRet = DocumentValidity.CurrentlyValid
                                Else
                                    If oDocument.BeginDate > dDate.Date Then
                                        oRet = DocumentValidity.ValidOnFuture
                                    End If
                                End If
                            Else
                                oRet = DocumentValidity.NotEnoughAuthorityLevel
                            End If
                        Case DocumentStatus.Pending
                            oRet = DocumentValidity.CheckPending
                        Case DocumentStatus.Rejected, DocumentStatus.Invalidated
                            oRet = DocumentValidity.Invalid
                        Case DocumentStatus.Expired
                            ' Para documentación de bajas, los documentos expirados se consideran válidos si caducan fuera de la ausencia a la que están asignados
                            If oDocument.IdDaysAbsence > 0 Then
                                Dim oPAState As New Absence.roProgrammedAbsenceState
                                Dim oPA As New Absence.roProgrammedAbsence(oDocument.IdDaysAbsence.ToString, oPAState)
                                If oPA IsNot Nothing AndAlso oPA.BeginDate.HasValue AndAlso oDocument.EndDate >= oPA.RealFinishDate Then
                                    oRet = DocumentValidity.CurrentlyValid
                                Else
                                    oRet = DocumentValidity.Invalid
                                End If
                            End If
                    End Select
                Else
                    ' No requiere aprobación
                    oRet = DocumentValidity.CurrentlyValid
                    Select Case oDocument.Status
                        Case DocumentStatus.Pending, DocumentStatus.Validated
                            If oDocument.BeginDate <= dDate.Date AndAlso oDocument.EndDate >= dDate.Date Then
                                oRet = DocumentValidity.CurrentlyValid
                            Else
                                If oDocument.BeginDate > dDate.Date Then
                                    oRet = DocumentValidity.ValidOnFuture
                                End If
                            End If
                        Case DocumentStatus.Rejected, DocumentStatus.Invalidated
                            oRet = DocumentValidity.Invalid
                        Case DocumentStatus.Expired
                            ' Para documentación de bajas, los documentos expirados se consideran válidos si caducan fuera de la ausencia a la que están asignados
                            If oDocument.IdDaysAbsence > 0 Then
                                Dim oPAState As New Absence.roProgrammedAbsenceState
                                Dim oPA As New Absence.roProgrammedAbsence(oDocument.IdDaysAbsence.ToString, oPAState)
                                If oPA IsNot Nothing AndAlso oPA.BeginDate.HasValue AndAlso oDocument.EndDate >= oPA.RealFinishDate Then
                                    oRet = DocumentValidity.CurrentlyValid
                                Else
                                    oRet = DocumentValidity.Invalid
                                End If
                            End If
                    End Select
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::IsDocumentCurrentlyValid")
            End Try
            Return oRet
        End Function

        ''' <summary>
        ''' Elimina  una platilla de documento
        ''' </summary>
        ''' <param name="oDocumentTemplate"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function DeleteDocumentTemplate(oDocumentTemplate As roDocumentTemplate, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bolActionInsert As Boolean = False
            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If ValidateDocumentTemplate(oDocumentTemplate, Nothing, True, False) Then
                    Dim strQuery = "@DELETE# DocumentTemplates WHERE ID = " & oDocumentTemplate.Id.ToString
                    If Not ExecuteSql(strQuery) Then
                        oState.Result = DocumentResultEnum.ConnectionError
                        oState.ErrorText = ""
                        bolRet = False
                    Else
                        bolRet = True
                    End If

                    If bolRet AndAlso oDocumentTemplate.Scope = DocumentScope.EmployeeField Then
                        ' Borramos el campo de la ficha asociado
                        Dim oUserField As New VTUserFields.UserFields.roUserField(New VTUserFields.UserFields.roUserFieldState, oDocumentTemplate.Name, Types.EmployeeField, False)
                        If Not oUserField Is Nothing Then oUserField.Delete(True) ' No hace falta validar si hay valores para el
                    End If

                    If bolRet AndAlso bAudit Then
                        ' Auditamos borrado de plantilla
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{DocumenttName}", oDocumentTemplate.Name, "", 1)
                        bolRet = oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tDocumentTemplate, oDocumentTemplate.Name, tbParameters, -1)
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::DeleteDocumentTemplate")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::DeleteDocumentTemplate")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Guarda un documento entregado
        ''' </summary>
        ''' <param name="oDocument"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SaveDocument(ByRef oDocument As roDocument, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bolActionInsert As Boolean = False
            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(oDocument) Then
                    Me.oState.Result = DocumentResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                'Creamos o actualizamos el documento
                If (oDocument.Id.Equals(-1)) Then
                    oDocument.Id = GetNextDocumentID()
                    bolActionInsert = True
                End If

                ' Documentos subidos de manera masiva, se asignan a empleados o empresas en función del nombre, que contiene un NIF o un CIF
                If oDocument.DocumentTemplate.Scope <> DocumentScope.Communique AndAlso oDocument.DocumentTemplate.Scope <> DocumentScope.BioCertificate Then
                    If IsEmployeeDocument(oDocument) Then
                        Dim sPattern As String = String.Empty
                        If oDocument.IdEmployee = 0 AndAlso oDocument.Title.Length > 0 Then
                            SearchEmployeeByNIF(oDocument, sPattern)
                        End If

                        If oDocument.IdEmployee = 0 Then
                            bolRet = False
                            ' Personalizamos el mensaje de error
                            If sPattern <> "_nif_(" Then
                                oState.Result = DocumentResultEnum.EmployeeRequired
                            Else
                                oState.Result = DocumentResultEnum.EmployeeRequiredPatternAlt1
                            End If

                            Return bolRet
                        Else
                            ' Miro si el supervisor tiene permiso sobre el usuario para entregar documentos
                            If bolActionInsert AndAlso oDocument.DeliveryChannel <> "VisualTimePortal" AndAlso WLHelper.GetFeaturePermissionByEmployee(oState.IDPassport, "Documents.Permision." & oDocument.DocumentTemplate.Area.ToString, oDocument.IdEmployee, "U") < Permission.Write Then
                                bolRet = False
                                oState.Result = DocumentResultEnum.NoPermissionOverEmployee
                                Return bolRet
                            End If
                        End If

                        ' En el caso que requiera firma digital solo podemos guardar documentos pdf
                        If oDocument.DocumentTemplate.RequiresSigning Then
                            If Not oDocument.DocumentType.ToUpper.Contains("PDF") Then
                                bolRet = False
                                oState.Result = DocumentResultEnum.PDFDocumentRequired
                                Return bolRet
                            End If
                        End If

                    Else
                        If oDocument.IdCompany = 0 AndAlso oDocument.Title.Length > 0 AndAlso oDocument.Title.Contains("_") Then
                            ' Documento de empresa sin idcompany informado
                            Dim sCIF As String = String.Empty
                            sCIF = oDocument.Title.Split("_")(0)
                            Dim oGroupState As New VTBusiness.Group.roGroupState(oState.IDPassport)
                            Dim oGroup As VTBusiness.Group.roGroup = VTBusiness.Group.roGroup.GetCompanyByCIF(sCIF, oGroupState)
                            If oGroup IsNot Nothing AndAlso oGroup.ID > 0 Then
                                oDocument.IdCompany = oGroup.ID
                            End If
                        End If
                        If oDocument.IdCompany = 0 Then
                            bolRet = False
                            oState.Result = DocumentResultEnum.CompanyRequired
                            Return bolRet
                        Else
                            ' Miro si el supervisor tiene permiso sobre la empresa para entregar documentos
                            If bolActionInsert AndAlso oDocument.DeliveryChannel <> "VisualTimePortal" AndAlso WLHelper.GetFeaturePermissionByGroup(oState.IDPassport, "Documents.Permision." & oDocument.DocumentTemplate.Area.ToString, oDocument.IdCompany, "U") < Permission.Write Then
                                bolRet = False
                                oState.Result = DocumentResultEnum.NoPermissionOverGroup
                                Return bolRet
                            End If
                        End If
                    End If
                End If

                ' Documento de parte de alta. Si aplica (se acaba de entregar y no require aprobación), debe cerrar la ausencia
                If oDocument.DocumentTemplate.Scope = DocumentScope.LeaveOrPermission AndAlso oDocument.DocumentTemplate.LeaveDocumentType = LeaveDocumentType.ReturnReport AndAlso bolActionInsert Then
                    'Recupero la ausencia
                    Dim oAbsenceState As New Absence.roProgrammedAbsenceState
                    roBusinessState.CopyTo(Me.oState, oAbsenceState)

                    Dim oAbsence As New Absence.roProgrammedAbsence(oDocument.IdDaysAbsence, oAbsenceState)
                    If oAbsence IsNot Nothing Then
                        oAbsence.FinishDate = oDocument.EndDate
                        ' Quito fecha de fin del documento, porque en realidad no es la de fin, sino que la he usado para informar la fecha de alta
                        oDocument.EndDate = New Date(2079, 1, 1)
                        bolRet = oAbsence.Save()
                    End If
                End If

                bolRet = SaveDocumentData(oDocument)

                'Si viene con el DocumentExternalID, lo buscamos y eliminamos el anterior
                If bolRet AndAlso Not String.IsNullOrEmpty(oDocument.DocumentExternalID) Then
                    Dim existingDocuments As List(Of roDocument) = GetDocumentsByExternalId(oDocument)
                    Dim currentDocumentEmployee As Integer = oDocument.IdEmployee
                    ' Si la lista contiene un documento de otro idemployee, devuelvo error
                    If existingDocuments.Count > 0 Then
                        If existingDocuments.FindAll(Function(x) x.IdEmployee <> currentDocumentEmployee).Any Then
                            bolRet = False
                            oState.Result = DocumentResultEnum.ExternalIdDuplicated
                            Return bolRet
                        Else
                            Dim existingDocument As roDocument = existingDocuments.First(Function(x) x.IdEmployee = currentDocumentEmployee)
                            If existingDocument IsNot Nothing Then
                                Dim bDeletedDocument = DeleteDocument(existingDocument.Id, bAudit)
                                If Not bDeletedDocument Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"roDocumentManager::SaveDocument: Error deleting duplicated document {existingDocument.Id}")
                                    bolRet = False
                                    oState.Result = DocumentResultEnum.ErrorDeletingDocument
                                    Return bolRet
                                End If
                            End If
                        End If
                    End If
                End If

                If bolRet AndAlso oDocument.DocumentTemplate.Scope = DocumentScope.EmployeeField Then
                    ' Para documentos que vayan a la ficha, creo o actualizo el valor ahora.
                    ' Si requiere aprobación, sólo paso el doc a la ficha si el doc está aprobado
                    Dim bUpdateLink As Boolean = True
                    If oDocument.DocumentTemplate.ApprovalLevelRequired > 0 AndAlso oDocument.Validity <> DocumentValidity.CurrentlyValid Then bUpdateLink = False
                    If bUpdateLink Then
                        Dim oEmployeeUserField As VTUserFields.UserFields.roEmployeeUserField = New VTUserFields.UserFields.roEmployeeUserField(oDocument.IdEmployee, oDocument.DocumentTemplate.Name, New Date(1900, 1, 1), New VTUserFields.UserFields.roUserFieldState)
                        oEmployeeUserField.FieldValue = oDocument.Id.ToString
                        bolRet = oEmployeeUserField.Save()
                    End If
                End If

                ' Se notifica al supervisor
                If bolRet AndAlso bolActionInsert AndAlso oDocument.DocumentTemplate.Notifications.Contains("700") Then
                    Dim strSQL As String = "@SELECT# IsNull(Activated,0) As Active from Notifications where ID = 700"
                    Dim isActive As Boolean = roTypes.Any2Boolean(ExecuteScalar(strSQL))
                    If isActive Then
                        strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters ) VALUES " &
                                            "(700, " & oDocument.IdEmployee & "," & oDocument.Id & " ,'" & oDocument.Title & "@" & oDocument.Remarks & "')"
                        bolRet = ExecuteSql(strSQL)
                    End If
                End If

                ' Si hay alerta de documento no entregado referente a este doc, la elimino
                If bolRet AndAlso oDocument.IdDaysAbsence > 0 OrElse oDocument.IdHoursAbsence > 0 OrElse oDocument.IdRequest > 0 OrElse oDocument.IdOvertimeForecast > 0 Then
                    Dim strSQLBase As String = "@DELETE# sysroNotificationTasks WHERE Key1Numeric = " & oDocument.IdEmployee & " AND Key2Numeric = " & oDocument.DocumentTemplate.Id & " AND IDNotification = 701"
                    Dim strSQL As String = String.Empty
                    If oDocument.IdDaysAbsence > 0 Then
                        strSQL = strSQLBase & " AND Key5Numeric = " & oDocument.IdDaysAbsence.ToString
                        bolRet = ExecuteSql(strSQL)
                    End If
                    If bolRet AndAlso oDocument.IdHoursAbsence > 0 Then
                        strSQL = strSQLBase & " AND Key5Numeric = " & oDocument.IdHoursAbsence.ToString
                        bolRet = ExecuteSql(strSQL)
                    End If
                    If bolRet AndAlso oDocument.IdRequest > 0 Then
                        strSQL = strSQLBase & " AND Key5Numeric = " & oDocument.IdRequest.ToString
                        bolRet = ExecuteSql(strSQL)
                    End If
                    If bolRet AndAlso oDocument.IdOvertimeForecast > 0 Then
                        strSQL = strSQLBase & " AND Key5Numeric = " & oDocument.IdOvertimeForecast.ToString
                        bolRet = ExecuteSql(strSQL)
                    End If
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos=
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(If(bolActionInsert, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tDocument, oDocument.Title, tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SaveDocument")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SaveDocument")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet
        End Function

        Private Sub SearchEmployeeByNIF(ByRef oDocument As roDocument, ByRef sPattern As String)
            ' Probamos patrones para localización de NIF
            Dim bKeepSearching As Boolean = True
            Dim sTemplateStart = "_nif_("
            Dim sTemplateEnd = ")"
            If oDocument.Title.ToLower.Contains(sTemplateStart) AndAlso oDocument.Title.ToLower.Contains(sTemplateEnd) Then
                ' Patrón nif_(NIF)
                bKeepSearching = False
                sPattern = "_nif_("
                Try
                    Dim iStartPosition As Integer = -1
                    Dim iEndPosition As Integer = -1
                    iStartPosition = oDocument.Title.ToLower.IndexOf(sTemplateStart) + sTemplateStart.Length
                    iEndPosition = oDocument.Title.ToLower.IndexOf(sTemplateEnd)
                    Dim sNIF As String = String.Empty
                    sNIF = oDocument.Title.Substring(iStartPosition, iEndPosition - iStartPosition)
                    Dim tbEmployees As New DataTable
                    Dim oEmployeeState As New UserFields.roUserFieldState(Me.oState.IDPassport)
                    tbEmployees = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue("NIF", sNIF, Now, oEmployeeState)
                    If tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0 Then
                        oDocument.IdEmployee = tbEmployees.Rows(0).Item("idemployee")
                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDocumentManager::SearchEmployeeByNIF: Error parsing fine name searching employees NIF")
                End Try
            End If

            If bKeepSearching AndAlso oDocument.Title.Contains("_") Then
                ' Patrón NIF_
                Dim sNIF As String = String.Empty
                sNIF = oDocument.Title.Split("_")(0)
                Dim tbEmployees As New DataTable
                Dim oEmployeeState As New UserFields.roUserFieldState(Me.oState.IDPassport)
                tbEmployees = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue("NIF", sNIF, Now, oEmployeeState)
                If tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0 Then
                    oDocument.IdEmployee = tbEmployees.Rows(0).Item("idemployee")
                End If
            End If
        End Sub

        ''' <summary>
        ''' Devuelve la lista de identificadores de ámbito en función del tipo de documento
        ''' </summary>
        ''' <param name="eType"></param>
        ''' <returns></returns>
        Public Function GetDocumentTemplateScopesIds(eType As DocumentType, Optional ByVal bIncludeAbsences As Boolean = True) As List(Of Integer)
            Dim lRet As New List(Of Integer)
            Try
                Select Case eType
                    Case DocumentType.Company
                        lRet.Add(DocumentScope.Company)
                        lRet.Add(DocumentScope.CompanyAccessAuthorization)
                    Case DocumentType.Employee
                        lRet.Add(DocumentScope.EmployeeContract)
                        lRet.Add(DocumentScope.EmployeeField)
                        lRet.Add(DocumentScope.EmployeeAccessAuthorization)

                        If bIncludeAbsences Then
                            lRet.Add(DocumentScope.LeaveOrPermission)
                            lRet.Add(DocumentScope.CauseNote)
                        End If
                        'lRet.Add(DocumentScope.Punch)
                    Case DocumentType.Visit
                        'lRet.Add(DocumentScope.Visit)
                End Select
            Catch ex As Exception
            End Try
            Return lRet
        End Function

        ''' <summary>
        ''' Obtiene todas las plantillas de documentos, si es preciso, limitada por su ámbito
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetAvailableEmployeeTemplateDocuments(Optional ByVal bAudit As Boolean = False) As List(Of roDocumentTemplate)
            Dim bolRet As New List(Of roDocumentTemplate)
            Dim tbTemplates As DataTable = Nothing

            Try
                'Se llama desde el Portal. No se le debe aplicar seguridad de supervisor
                Dim strQuery = " @SELECT# dt.Id FROM DocumentTemplates dt WHERE "
                strQuery &= " dt.scope In (" & DocumentScope.EmployeeContract & "," & DocumentScope.EmployeeField & ") " 'plantillas que aplican a empleados"n a empleados"
                strQuery &= " And dt.EmployeeDeliverAllowed = 1 and dt.RequiresSigning = 0"
                strQuery &= " ORDER BY dt.Name"

                Dim tbDocTemplates As DataTable = CreateDataTable(strQuery)
                If (tbDocTemplates IsNot Nothing AndAlso tbDocTemplates.Rows.Count > 0) Then
                    For Each rowDocTemplate As DataRow In tbDocTemplates.Rows
                        Dim documentTemplate = New roDocumentTemplate
                        documentTemplate = LoadDocumentTemplate(rowDocTemplate("Id"))
                        bolRet.Add(documentTemplate)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetTemplateDocuments")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetTemplateDocuments")
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Invalida un documento
        ''' </summary>
        ''' <param name="oDocument"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Public Function InvalidateDocumentsOnNewApproval(oDocument As roDocument) As Boolean
            Dim bolRet As Boolean = False
            Try
                Dim strSQL As String = String.Empty
                strSQL = "@UPDATE# Documents set Status = " & DocumentStatus.Invalidated & " where IdDocumentTemplate = " & oDocument.DocumentTemplate.Id.ToString & " and IDEmployee = " & oDocument.IdEmployee.ToString & " and ID <> " & oDocument.Id
                bolRet = ExecuteSql(strSQL)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::InvalidateDocumentsOnNewApproval")
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene todas las plantillas de documentos, si es preciso, limitada por su ámbito
        ''' </summary>
        ''' <param name="bFilterScope"></param>
        ''' <param name="docScope"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetTemplateDocuments(ByVal bFilterScope As Boolean, ByVal docScope As DocumentScope, Optional ByVal bAudit As Boolean = False) As List(Of roDocumentTemplate)
            Dim bolRet As New List(Of roDocumentTemplate)
            Dim strWhere As String = String.Empty

            Try

                Dim idPassport As Integer = 1
                If Me.oState.IDPassport > 0 Then idPassport = Me.oState.IDPassport

                Dim strQuery = " @SELECT# Id, Name, ShortName, Description FROM DocumentTemplates"

                ' Calculamos permisos para documentos por nivel de LOPD
                Dim aPermOverLOPD As List(Of Integer) = New List(Of Integer)
                aPermOverLOPD = GetAllowedDocumentLOPDAccessLevels(idPassport)
                If aPermOverLOPD.Count > 0 Then strWhere = " WHERE LOPDAccessLevel in (" & String.Join(",", aPermOverLOPD) & ")"
                If strWhere.Trim.Length > 0 Then strQuery = strQuery & strWhere

                ' Calculamos permisos sobre tipos de documentos. Deben tener lectura o superior
                Dim aPermOverDocAreas As List(Of Integer) = New List(Of Integer)
                aPermOverDocAreas = GetAllowedDocumentAreas(idPassport)
                If aPermOverDocAreas.Count > 0 Then strWhere = " And DocumentTemplates.Area in (" & String.Join(",", aPermOverDocAreas) & ")"
                If strWhere.Trim.Length > 0 Then strQuery = strQuery & strWhere

                If bFilterScope Then
                    strQuery = strQuery & " AND Scope = " & docScope & " "
                End If
                strQuery = strQuery & " ORDER BY Name"

                Dim tbDocTemplates As DataTable = CreateDataTable(strQuery)
                If (tbDocTemplates IsNot Nothing AndAlso tbDocTemplates.Rows.Count > 0) Then
                    For Each rowDocTemplate As DataRow In tbDocTemplates.Rows
                        Dim documentTemplate = New roDocumentTemplate
                        documentTemplate = LoadDocumentTemplate(rowDocTemplate("Id"))
                        bolRet.Add(documentTemplate)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetTemplateDocuments")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetTemplateDocuments")
            Finally

            End Try
            Return bolRet
        End Function

        Public Function GetDocumentsByTemplate(documentTemplate As roDocumentTemplate) As List(Of roDocument)

            If documentTemplate.Scope = DocumentScope.BioCertificate Then
                Return GetDocumentsByTemplateId(documentTemplate.Id, False, "", True, False)
            Else
                Return GetDocumentsByTemplateId(documentTemplate.Id, False, "", True, True)
            End If
        End Function

        Public Function GetDocumentsByTemplateId(templateId As Integer, Optional ByVal bAudit As Boolean = False, Optional searchTerm As String = "", Optional showAll As Boolean = False) As List(Of roDocument)
            Return GetDocumentsByTemplateId(templateId, bAudit, searchTerm, showAll, True)
        End Function

        Private Function GetDocumentsByTemplateId(templateId As Integer, ByVal bAudit As Boolean, ByVal searchTerm As String, ByVal showAll As Boolean, ByVal appliesPermissions As Boolean) As List(Of roDocument)
            Dim lstRet As New List(Of roDocument)

            Web.HttpContext.Current.Session("LimitReached") = False

            Try

                Dim idPassport As Integer = 1
                If Me.oState.IDPassport > 0 Then idPassport = Me.oState.IDPassport

                Dim table As DataTable = New DataTable
                Dim strSQL As String = ""

                If appliesPermissions Then
                    strSQL = $"@SELECT# Documents.Id From 
                                         sysrofnSecurity_PermissionsOverDocumentTemplates(@templateid, 0, 0, @idpassport ) POD
                                         INNER JOIN Documents ON POD.IdDocumentTemplate = Documents.IdDocumentTemplate AND ((POD.IdEmployee > 0 AND  Documents.IdEmployee = POD.IdEmployee) OR (POD.IdCompany > 0 AND POD.IdCompany = Documents.IdCompany))
                                         WHERE Documents.Title LIKE @searchterm
                                         ORDER BY Title"
                Else
                    strSQL = $"@SELECT# Documents.Id From 
                                         Documents
                                         WHERE Documents.IdDocumentTemplate={templateId} AND Documents.Title LIKE @searchterm
                                         ORDER BY Title"
                End If

                Dim Command As DbCommand = AccessHelper.CreateCommand(strSQL)
                AccessHelper.AddParameter(Command, "@templateid", DbType.Int32).Value = templateId
                AccessHelper.AddParameter(Command, "@idpassport", DbType.Int32).Value = idPassport
                AccessHelper.AddParameter(Command, "@searchterm", DbType.String).Value = IIf(searchTerm = String.Empty, "%", $"%{searchTerm}%")
                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                Adapter.Fill(table)

                If table.Rows.Count > 0 Then
                    Dim indx = 0
                    For Each row As DataRow In table.Rows
                        lstRet.Add(LoadDocument(row("Id")))
                        'Limitamos a 200 el numero máximo de documentos a mostrar
                        indx = indx + 1
                        If showAll.Equals(False) AndAlso indx >= 200 Then
                            Web.HttpContext.Current.Session("LimitReached") = True
                            Exit For
                        End If
                    Next
                End If

                If bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tDocument, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentsByTemplateId")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentsByTemplateId")
            Finally

            End Try
            Return lstRet
        End Function

        Public Function GetDocuments(lstEmployees As String, ByVal bAudit As Boolean, ByVal bForAllEmployees As Boolean, ByVal bForOneEmployee As Boolean, ByVal type As String, ByVal title As String, ByVal company As String, ByVal timestamp As Date?, ByVal updateType As String, ByVal extension As String, ByVal template As String, ByVal importPrimaryKey As String, Optional ByVal numberOfDocuments As Integer = 10) As List(Of roDocument)
            Dim lstRet As New List(Of roDocument)

            Dim documents As New List(Of roDocument)

            Try

                Dim idPassport As Integer = 1
                If Me.oState.IDPassport > 0 Then idPassport = Me.oState.IDPassport

                Dim table As DataTable = New DataTable
                Dim strSQL As String = ""
                Dim strUniqueidentifierField As String = ""

                If extension IsNot Nothing AndAlso extension <> "" AndAlso Not extension.Contains(".") Then
                    extension = "." & extension
                End If

                If lstEmployees.Length > 0 OrElse bForAllEmployees OrElse type = "company" Then
                    If Not bForOneEmployee Then
                        strUniqueidentifierField = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState, ).Value)
                    End If
                    If lstEmployees.Length > 0 AndAlso bForOneEmployee Then
                        strSQL = "@SELECT# TOP " & numberOfDocuments & " Title, dt.Name as DocumentType, d.IdCompany, d.IdDaysAbsence, d.IdHoursAbsence, d.BlobFileName, DeliveryChannel, len(dbo.f_BinaryToBase64(Document)) DocLen, Document, d.documenttype as Extension, d.DeliveryDate, d.DeliveryChannel, d.DeliveredBy, d.SignDate, d.status, d.IdLastStatusSupervisor, d.BeginDate, d.EndDate, d.LastStatusChange, case when SignDate is  null or LastStatusChange > SignDate then LastStatusChange else SignDate end as LastUpdateTimestamp, DocumentExternalId FROM Documents as d"
                        strSQL += " inner join DocumentTemplates as dt on dt.id = d.IdDocumentTemplate"
                        If type = "company" Then
                            strSQL += " left join Groups as g on g.ID = d.IdCompany"
                        Else
                            strSQL += " left join EmployeeGroups as eg on eg.IDEmployee = d.IdEmployee and GETDATE() between eg.BeginDate and eg.EndDate"
                            strSQL += " left join Groups as g on g.ID = eg.IdGroup"
                        End If
                        strSQL += " left join ProgrammedAbsences as pa on pa.AbsenceID = d.IdDaysAbsence"
                        strSQL += " left join ProgrammedCauses as pc on pc.AbsenceID = d.IdHoursAbsence"
                        strSQL += " left join sysroPassports as sp on sp.ID = d.IdLastStatusSupervisor"
                        strSQL += " where lower(dt.Name) <> 'biocertificate' and d.IdCommunique is null"
                        If type.ToLower.Equals("employee") Then
                            strSQL += " and d.IdEmployee is not null and d.IdEmployee <> -1 and d.IdEmployee <> 0"
                        Else
                            strSQL += " and d.IdCompany is not null and d.IdCompany <> -1 and d.IdCompany <> 0"
                        End If
                        If title IsNot Nothing AndAlso title <> "" Then
                            strSQL += " and lower(Title) like '%" & title.ToLower() & "%'"
                        End If
                        If type = "employee" Then
                            strSQL += " and d.IDEmployee in( " & lstEmployees.ToString & ")"
                        End If
                        If company IsNot Nothing AndAlso company <> "" Then
                            strSQL += " and lower(g.fullgroupname) like '" & company.ToLower() & "%'"
                        End If
                        If timestamp IsNot Nothing AndAlso timestamp <> New Date(1, 1, 1) Then
                            If updateType.ToLower().Equals("statuschanged") Then
                                strSQL += " and LastStatusChange >= " & roTypes.Any2Time(timestamp).SQLDateTime
                            Else
                                If updateType.ToLower().Equals("signed") Then
                                    strSQL += " and SignDate >= " & roTypes.Any2Time(timestamp).SQLDateTime
                                Else
                                    If updateType.ToLower().Equals("delivered") Then
                                        strSQL += " and DeliveryDate >= " & roTypes.Any2Time(timestamp).SQLDateTime
                                    Else
                                        If updateType.Equals("") Then
                                            strSQL += " and (LastStatusChange >= " & roTypes.Any2Time(timestamp).SQLDateTime & " or SignDate >= " & roTypes.Any2Time(timestamp).SQLDateTime & " or DeliveryDate >= " & roTypes.Any2Time(timestamp).SQLDateTime & ")"
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        If extension IsNot Nothing AndAlso extension <> "" Then
                            strSQL += " and lower(DocumentType) = '" & extension.ToLower() & "'"
                        End If
                        If template IsNot Nothing AndAlso template <> "" Then
                            strSQL += " and lower(dt.Name) = '" & template.ToLower() & "'"
                        End If

                        strSQL = strSQL & " order by LastUpdateTimestamp asc"
                    Else
                        strSQL = "@SELECT# TOP " & numberOfDocuments & " Title, dt.Name as DocumentType, d.IdCompany, d.IdDaysAbsence, d.IdHoursAbsence, d.BlobFileName, DeliveryChannel, len(dbo.f_BinaryToBase64(Document)) DocLen, Document, d.documenttype as Extension, d.DeliveryDate, d.DeliveryChannel, d.DeliveredBy, d.SignDate, d.status, d.IdLastStatusSupervisor, d.BeginDate, d.EndDate, CONVERT(VARCHAR,NifTable.Value) AS NIF, CONVERT(VARCHAR,IdTable.Value) AS IdImport, d.LastStatusChange, case when SignDate is  null or LastStatusChange > SignDate then LastStatusChange else SignDate end as LastUpdateTimestamp, DocumentExternalId FROM Documents as d"
                        strSQL += " inner join DocumentTemplates as dt on dt.id = d.IdDocumentTemplate"
                        If type = "company" Then
                            strSQL += " left join Groups as g on g.ID = d.IdCompany"
                        Else
                            strSQL += " left join EmployeeGroups as eg on eg.IDEmployee = d.IdEmployee and GETDATE() between eg.BeginDate and eg.EndDate"
                            strSQL += " left join Groups as g on g.ID = eg.IdGroup"
                        End If
                        strSQL += " left join ProgrammedAbsences as pa on pa.AbsenceID = d.IdDaysAbsence"
                        strSQL += " left join ProgrammedCauses as pc on pc.AbsenceID = d.IdHoursAbsence"
                        strSQL += " left join sysroPassports as sp on sp.ID = d.IdLastStatusSupervisor"
                        strSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = d.IDEmployee AND NifTable.Date < GETDATE() "
                        strSQL += " LEFT JOIN (@SELECT# row_number() over (partition by IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = d.IDEmployee AND IdTable.Date < GETDATE()"
                        strSQL += " where lower(dt.Name) <> 'biocertificate' and d.IdCommunique is null"
                        If type.ToLower.Equals("employee") Then
                            strSQL += " and d.IdEmployee is not null and d.IdEmployee <> -1 and d.IdEmployee <> 0"
                        Else
                            strSQL += " and d.IdCompany is not null and d.IdCompany <> -1 and d.IdCompany <> 0"
                        End If
                        If title IsNot Nothing AndAlso title <> "" Then
                            strSQL += " and lower(Title) like '%" & title.ToLower() & "%'"
                        End If
                        If Not bForAllEmployees AndAlso type = "employee" Then
                            strSQL += " and d.IDEmployee in( " & lstEmployees.ToString & ")"
                        End If
                        If company IsNot Nothing AndAlso company <> "" Then
                            strSQL += " and lower(g.fullgroupname) like '" & company.ToLower() & "%'"
                        End If
                        If timestamp IsNot Nothing AndAlso timestamp <> New Date(1, 1, 1) Then
                            If updateType.ToLower().Equals("statuschanged") Then
                                strSQL += " and LastStatusChange >= " & roTypes.Any2Time(timestamp).SQLDateTime
                            Else
                                If updateType.ToLower().Equals("signed") Then
                                    strSQL += " and SignDate >= " & roTypes.Any2Time(timestamp).SQLDateTime
                                Else
                                    If updateType.ToLower().Equals("delivered") Then
                                        strSQL += " and DeliveryDate >= " & roTypes.Any2Time(timestamp).SQLDateTime
                                    Else
                                        If updateType.Equals("") Then
                                            strSQL += " and (LastStatusChange >= " & roTypes.Any2Time(timestamp).SQLDateTime & " or SignDate >= " & roTypes.Any2Time(timestamp).SQLDateTime & " or DeliveryDate >= " & roTypes.Any2Time(timestamp).SQLDateTime & ")"
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        If extension IsNot Nothing AndAlso extension <> "" Then
                            strSQL += " And lower(DocumentType) = '" & extension.ToLower() & "'"
                        End If
                        If template IsNot Nothing AndAlso template <> "" Then
                            strSQL += " and lower(dt.Name) = '" & template.ToLower() & "'"
                        End If
                        strSQL += " AND (NifTable.RowNumber1 IS NULL OR NifTable.RowNumber1 = 1) "
                        strSQL += " AND (IdTable.RowNumber1 IS NULL OR IdTable.RowNumber1 = 1)"
                        strSQL = strSQL & " order by LastUpdateTimestamp asc"
                    End If
                    ' Cargamos lista de documentos
                    Dim documentsDT As DataTable = CreateDataTableWithoutTimeouts(strSQL)

                    If documentsDT IsNot Nothing AndAlso documentsDT.Rows.Count > 0 Then
                        For Each documentRow As DataRow In documentsDT.Rows
                            Dim document As New roDocument()
                            document.Title = roTypes.Any2String(documentRow("Title"))
                            document.DocumentType = roTypes.Any2String(documentRow("DocumentType"))
                            If type = "employee" Then
                                document.IdCompany = 0
                            Else
                                document.IdCompany = roTypes.Any2Integer(documentRow("IdCompany"))
                            End If
                            document.IdDaysAbsence = roTypes.Any2Integer(documentRow("IdDaysAbsence"))
                            document.IdHoursAbsence = roTypes.Any2Integer(documentRow("IdHoursAbsence"))
                            Try
                                If roTypes.Any2String(documentRow("DeliveryChannel")).ToUpper = "TRASPASOGPA" Then
                                    ' Para documentos migrados desde GPA no hay que desencriptar, porque se migraron desencriptados
                                    document.Document = CType(documentRow("Document"), Byte())
                                Else
                                    Dim strDocumentBlobName As String = String.Empty
                                    If Not IsDBNull(documentRow("BlobFileName")) AndAlso roTypes.Any2Long(documentRow("DocLen")) = 0 Then
                                        ' Recupero de Azure
                                        strDocumentBlobName = documentRow("BlobFileName")

                                        document.Document = VTBase.Extensions.roEncrypt.Decrypt(Azure.RoAzureSupport.GetDocumentFile(strDocumentBlobName))
                                    Else
                                        ' Recupero de BBDD
                                        document.Document = VTBase.Extensions.roEncrypt.Decrypt(CType(documentRow("Document"), Byte()))
                                    End If
                                End If
                            Catch ex As Exception

                            End Try
                            Dim documentTemplate As roDocumentTemplate = New roDocumentTemplate()
                            documentTemplate.Name = roTypes.Any2String(documentRow("DocumentType"))
                            document.DocumentTemplate = documentTemplate
                            document.DocumentType = roTypes.Any2String(documentRow("Extension"))
                            document.DeliveredDate = roTypes.Any2DateTime(documentRow("DeliveryDate")).Date
                            document.DeliveryChannel = roTypes.Any2String(documentRow("DeliveryChannel"))
                            document.DeliveredBy = roTypes.Any2String(documentRow("DeliveredBy"))
                            If roTypes.Any2DateTime(documentRow("SignDate")) <> New Date(1, 1, 1) Then
                                document.SignDate = roTypes.Any2DateTime(documentRow("SignDate"))
                            Else
                                document.SignDate = Nothing
                            End If
                            document.LastStatusChange = roTypes.Any2DateTime(documentRow("LastStatusChange"))
                            document.Status = roTypes.Any2String(documentRow("Status"))
                            document.IdLastStatusSupervisor = roTypes.Any2Integer(documentRow("IdLastStatusSupervisor"))
                            document.BeginDate = roTypes.Any2DateTime(documentRow("BeginDate"))
                            document.EndDate = roTypes.Any2DateTime(documentRow("EndDate"))
                            If type.ToLower() = "employee" Then
                                If Not bForOneEmployee Then
                                    document.IdEmployee = roTypes.Any2Integer(documentRow("IdImport"))
                                Else
                                    document.IdEmployee = roTypes.Any2Integer(importPrimaryKey)
                                End If
                            Else
                                document.IdEmployee = 0
                            End If
                            document.DocumentExternalID = roTypes.Any2String(documentRow("DocumentExternalId"))
                            documents.Add(document)

                        Next
                    End If
                End If

                If bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tDocument, "", tbParameters, -1)
                End If
                Return documents
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocuments")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocuments")
            Finally

            End Try
            Return lstRet
        End Function

        Public Function GetDocumentsByFilterName(filterExpression As String, Optional ByVal bAudit As Boolean = False) As List(Of roDocument)
            Dim lstRet As New List(Of roDocument)

            Dim tbDocs As DataTable = Nothing
            Dim strWhere1 As String = String.Empty
            Dim strWhere2 As String = String.Empty

            Try

                Dim idPassport As Integer = 1
                If Me.oState.IDPassport > 0 Then idPassport = Me.oState.IDPassport

                Dim table As DataTable = New DataTable
                Dim strSQL As String = $"@SELECT# Documents.Id  
                                         FROM sysrofnSecurity_PermissionsOverDocumentTemplates(0, 0, 0, @idpassport ) POD
                                         INNER JOIN Documents ON POD.IdDocumentTemplate = Documents.IdDocumentTemplate AND ((POD.IdEmployee > 0 AND  Documents.IdEmployee = POD.IdEmployee) OR (POD.IdCompany > 0 AND POD.IdCompany = Documents.IdCompany))
                                         LEFT JOIN Employees ON Documents.IdEmployee = Employees.Id
                                         LEFT JOIN Groups ON Documents.IdCompany = Groups.Id
                                         WHERE UPPER(Documents.Title + Documents.DocumentType) Like @filterExpression
                                         OR (Employees.Name IS NOT NULL AND UPPER(Employees.Name) LIKE @filterExpression)
                                         OR (Groups.Name IS NOT NULL AND UPPER(Groups.Name) LIKE @filterExpression)
                                         ORDER BY Title"
                Dim Command As DbCommand = AccessHelper.CreateCommand(strSQL)
                AccessHelper.AddParameter(Command, "@filterExpression", DbType.String).Value = IIf(filterExpression = String.Empty, "%", $"%{filterExpression}%")
                AccessHelper.AddParameter(Command, "@idpassport", DbType.Int32).Value = idPassport
                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                Adapter.Fill(table)
                If table IsNot Nothing AndAlso table.Rows.Count > 0 Then
                    For Each row As DataRow In table.Rows
                        lstRet.Add(LoadDocument(row("Id")))
                    Next
                End If

                If bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tDocument, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentsByTemplateId")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentsByTemplateId")
            Finally

            End Try
            Return lstRet
        End Function

        ''' <summary>
        ''' Obtiene los documentos que han sido entregados por un empleado o empresa concretos, filtrando los que puede ver el supervisor en función de sus permisos y nivel
        ''' </summary>
        ''' <param name="idDocSource"></param>
        ''' <param name="type"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetDocumentsByType(idDocSource As Integer, type As DocumentType, Optional ByVal bAudit As Boolean = False, Optional bCalcWeight As Boolean = True) As List(Of roDocument)
            Dim lstRet As New List(Of roDocument)

            Dim tbDocs As DataTable = Nothing
            Dim strWhere1 As String = String.Empty
            Dim strWhere2 As String = String.Empty

            Try

                Dim idPassport As Integer = 1
                If Me.oState.IDPassport > 0 Then idPassport = Me.oState.IDPassport

                Dim table As DataTable = New DataTable
                Dim strSQL As String = $"@SELECT# Documents.Id  
                                         FROM sysrofnSecurity_PermissionsOverDocumentTemplates(0, @idemployee, @idcompany, @idpassport ) POD
                                         INNER JOIN Documents ON POD.IdDocumentTemplate = Documents.IdDocumentTemplate AND ((POD.IdEmployee > 0 AND  Documents.IdEmployee = POD.IdEmployee) OR (POD.IdCompany > 0 AND POD.IdCompany = Documents.IdCompany))
                                         ORDER BY Title"
                Dim Command As DbCommand = AccessHelper.CreateCommand(strSQL)
                AccessHelper.AddParameter(Command, "@idpassport", DbType.Int32).Value = idPassport
                AccessHelper.AddParameter(Command, "@idemployee", DbType.Int32).Value = IIf(type = DocumentType.Employee, idDocSource, -2)
                AccessHelper.AddParameter(Command, "@idcompany", DbType.Int32).Value = IIf(type = DocumentType.Company, idDocSource, -2)
                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                Adapter.Fill(table)
                If table IsNot Nothing AndAlso table.Rows.Count > 0 Then
                    For Each row As DataRow In table.Rows
                        lstRet.Add(LoadDocument(row("Id")))
                    Next
                End If

                If bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tDocument, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentsByType")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentsByType")
            Finally

            End Try
            Return lstRet
        End Function

        ''' <summary>
        ''' Obtiene los documentos que han sido entregados por un empleado concreto
        ''' </summary>
        ''' <param name="idEmployee"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetDocumentsByEmployee(idEmployee As Integer, ByVal iDate As DateTime, ByVal eDate As DateTime, ByVal filter As String, ByVal orderBy As String, Optional ByVal bAudit As Boolean = False) As List(Of roDocument)
            Dim bolRet As New List(Of roDocument)

            Try

                'Se llama desde Portal. No hay que aplicar permisos de supervisor
                Dim tbDocs As DataTable = Nothing
                Dim strQuery = " @SELECT# d.Id As Id FROM Documents d INNER JOIN DocumentTemplates dt ON d.IdDocumentTemplate = dt.Id where d.IdEmployee = " & idEmployee

                strQuery &= " AND d.DeliveryDate between " & roTypes.Any2Time(iDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(eDate).SQLSmallDateTime

                If filter <> String.Empty Then
                    strQuery &= " AND dt.Scope in(" & filter.Replace("*", ",") & ")"
                End If

                If orderBy <> String.Empty Then
                    strQuery &= " ORDER BY d." & orderBy
                Else
                    strQuery &= " ORDER BY d.Title"
                End If

                tbDocs = CreateDataTable(strQuery)
                If (tbDocs IsNot Nothing AndAlso tbDocs.Rows.Count > 0) Then
                    For Each row As DataRow In tbDocs.Rows
                        bolRet.Add(LoadDocument(row("Id")))
                    Next
                End If

                If bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tDocument, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentsByType")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentsByType")
            Finally

            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene los documentos que han sido entregados por los empleados de un grupo en concreto
        ''' </summary>
        ''' <param name="idEmployee"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetDocumentEmployeesByGroup(idGroup As Integer, ByVal filter As String, ByVal orderBy As String, Optional ByVal bAudit As Boolean = False) As List(Of roDocument)
            Dim bolRet As New List(Of roDocument)

            Try
                Dim employeesInGroup = VTBusiness.Group.roGroup.GetEmployeesFromGroup(idGroup, "Documents", "U", New VTBusiness.Group.roGroupState)
                bolRet.AddRange(GetDocumentsByType(idGroup, DocumentType.Company, False, False))
                For Each Employee As DataRow In employeesInGroup.Tables(0).Rows
                    bolRet.AddRange(GetDocumentsByType(Employee.Item("IDEmployee"), DocumentType.Employee, False, False))
                Next
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentEmployeesByGroup")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentEmployeesByGroup")
            Finally

            End Try
            Return bolRet
        End Function

        Public Function GetAllDocumentEmployeesBySelection(ByVal employees As String, ByVal filter As String, ByVal filterUser As String, Optional ByVal bAudit As Boolean = False) As List(Of roDocument)
            Dim bolRet As New List(Of roDocument)

            Dim bState = New VTBusiness.Group.roGroupState(-1)

            Try
                Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(oState.IDPassport, "Documents", "U", Permission.Read,
                                                                        employees, filter, filterUser, False, Nothing, Nothing)

                For Each Employee As Integer In lstEmployees
                    bolRet.AddRange(GetDocumentsByType(Employee, DocumentType.Employee, False, False))
                Next

                bolRet = bolRet.Distinct().ToList()
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentEmployeesByGroup")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentEmployeesByGroup")
            End Try
            Return bolRet
        End Function

        Public Function SetDocumentRead(idDocument As Integer, idEmployee As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim updateStatus As Boolean = False

            Try
                Dim strSQL = "@UPDATE# documents set Received=1, ReceivedDate=getdate() where id= " & idDocument & " and idEmployee= " & idEmployee
                updateStatus = ExecuteSql(strSQL)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SetDocumentRead")
            End Try
            Return updateStatus

        End Function

        Public Function GetBioCertificateBytes(filename As String, Optional ByVal bAudit As Boolean = False) As roDocumentFile
            Dim documentFile As roDocumentFile = Nothing

            Try
                Dim oTemplate As roDocumentTemplate = LoadSystemTemplate(DocumentScope.BioCertificate)
                If oTemplate IsNot Nothing Then
                    Dim strSQL = "@SELECT# id FROM Documents where Title= '" & filename & "' and idDocumentTemplate= " & oTemplate.Id
                    Dim idDocument As Integer = roTypes.Any2Integer(ExecuteScalar(strSQL))

                    If idDocument > 0 Then
                        documentFile = GetDocumentBytesById(idDocument, False)
                    End If
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SetDocumentRead")
            End Try
            Return documentFile


        End Function


        ''' <summary>
        ''' obtienes el documento de la bbdd para mostrarlo en pantalla
        ''' </summary>
        ''' <param name="idDocument"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Public Function GetDocumentBytesById(idDocument As Integer, Optional ByVal bAudit As Boolean = False) As roDocumentFile
            Dim bolRet As New roDocumentFile
            Dim updateStatus As Boolean = False
            Dim tbDocuments As DataTable = Nothing

            Try

                Dim strQuery = " @SELECT# Title,DocumentType, Document, DeliveryChannel, BlobFileName, CRC, len(dbo.f_BinaryToBase64(Document)) DocLen  FROM Documents where Id = " & idDocument
                tbDocuments = CreateDataTable(strQuery)

                If (tbDocuments IsNot Nothing AndAlso tbDocuments.Rows.Count > 0) Then
                    For Each rowDocument As DataRow In tbDocuments.Rows
                        If roTypes.Any2String(rowDocument("DeliveryChannel")).ToUpper = "TRASPASOGPA" Then
                            ' Para documentos migrados desde GPA no hay que desencriptar, porque se migraron desencriptados
                            bolRet.DocumentContent = CType(rowDocument("Document"), Byte())
                        Else
                            Dim strDocumentBlobName As String = String.Empty
                            If Not IsDBNull(rowDocument("BlobFileName")) AndAlso roTypes.Any2Long(rowDocument("DocLen")) = 0 Then
                                ' Recupero de Azure
                                strDocumentBlobName = rowDocument("BlobFileName")

                                bolRet.DocumentContent = roEncrypt.Decrypt(Azure.RoAzureSupport.GetDocumentFile(strDocumentBlobName))
                            Else
                                ' Recupero de BBDD
                                bolRet.DocumentContent = roEncrypt.Decrypt(CType(rowDocument("Document"), Byte()))
                            End If
                        End If
                        bolRet.DocumentName = roTypes.Any2String(rowDocument("Title")) & roTypes.Any2String(rowDocument("DocumentType"))
                        Exit For
                    Next
                End If

                If bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDocument, bolRet.DocumentName, tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentBytesById")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentBytesById")
            Finally

            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' obtienes el informe de evidencias del documento firmado de la bbdd para mostrarlo en pantalla
        ''' </summary>
        ''' <param name="idDocument"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Public Function GetSignReportDocumentBytesById(idDocument As Integer, Optional ByVal bAudit As Boolean = False) As roDocumentFile
            Dim bolRet As New roDocumentFile
            Dim updateStatus As Boolean = False
            Dim tbDocuments As DataTable = Nothing

            Try

                Dim strQuery = " @SELECT# Title,DocumentType, SignReport, DeliveryChannel, BlobFileName, CRC, len(dbo.f_BinaryToBase64(Document)) DocLen  FROM Documents where Id = " & idDocument
                tbDocuments = CreateDataTable(strQuery)

                If (tbDocuments IsNot Nothing AndAlso tbDocuments.Rows.Count > 0) Then
                    For Each rowDocument As DataRow In tbDocuments.Rows
                        If roTypes.Any2String(rowDocument("DeliveryChannel")).ToUpper = "TRASPASOGPA" Then
                            ' Para documentos migrados desde GPA no hay que desencriptar, porque se migraron desencriptados
                            bolRet.DocumentContent = CType(rowDocument("SignReport"), Byte())
                        Else
                            Dim strDocumentBlobName As String = String.Empty
                            If Not IsDBNull(rowDocument("BlobFileName")) AndAlso roTypes.Any2Long(rowDocument("DocLen")) = 0 Then
                                ' Recupero de Azure
                                strDocumentBlobName = rowDocument("BlobFileName") & "_signreport"

                                bolRet.DocumentContent = roEncrypt.Decrypt(Azure.RoAzureSupport.GetDocumentFile(strDocumentBlobName))
                            Else
                                ' Recupero de BBDD
                                bolRet.DocumentContent = roEncrypt.Decrypt(CType(rowDocument("SignReport"), Byte()))
                            End If
                        End If
                        bolRet.DocumentName = "Report_" & roTypes.Any2String(rowDocument("Title")) & ".pdf"
                        Exit For
                    Next
                End If

                If bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDocument, bolRet.DocumentName, tbParameters, -1)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetSignReportDocumentBytesById")
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene lista de plantillas por tipo: empleado o empresa
        ''' </summary>
        ''' <param name="docType"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetAvailableDocumentTemplateByType(ByVal docType As DocumentType, ByVal idRelatedObject As Integer, ByVal minPermission As Permission, Optional ByVal bAudit As Boolean = False) As List(Of roDocumentTemplate)
            Dim bolRet As New List(Of roDocumentTemplate)
            Dim tbTemplates As DataTable = Nothing
            Dim strWhere As String = String.Empty

            Try

                Dim availableAreas As New Generic.List(Of Integer)

                Dim idPassport As Integer = 1
                If Me.oState.IDPassport > 0 Then idPassport = Me.oState.IDPassport

                Dim strQuery As String = String.Empty
                Select Case docType
                    Case DocumentType.Employee
                        strQuery = " @SELECT# Id, Name, ShortName, Description FROM DocumentTemplates WHERE Scope in (" & String.Join(",", GetDocumentTemplateScopesIds(DocumentType.Employee)) & ") "

                        ' Calculamos permisos en función de nivel de LOPD
                        Dim aPermOverLOPD As List(Of Integer) = New List(Of Integer)
                        aPermOverLOPD = GetAllowedDocumentLOPDAccessLevels(idPassport, minPermission)
                        If aPermOverLOPD.Count > 0 Then strWhere = " AND LOPDAccessLevel in (" & String.Join(",", aPermOverLOPD) & ")"
                        If strWhere.Trim.Length > 0 Then strQuery = strQuery & strWhere
                        strQuery = strQuery & " ORDER BY Name"

                        For Each eArea As DocumentArea In System.Enum.GetValues(GetType(DocumentArea))
                            If WLHelper.GetFeaturePermissionByEmployee(oState.IDPassport, "Documents.Permision." & eArea.ToString, idRelatedObject, "U") >= minPermission Then
                                availableAreas.Add(eArea)
                            End If
                        Next
                    Case DocumentType.Company
                        strQuery = " @SELECT# Id, Name, ShortName, Description FROM DocumentTemplates WHERE Scope in (" & String.Join(",", GetDocumentTemplateScopesIds(DocumentType.Company)) & ") ORDER BY Name"
                        For Each eArea As DocumentArea In System.Enum.GetValues(GetType(DocumentArea))
                            If WLHelper.GetFeaturePermissionByGroup(oState.IDPassport, "Documents.Permision." & eArea.ToString, idRelatedObject, "U") >= minPermission Then
                                availableAreas.Add(eArea)
                            End If
                        Next
                End Select

                Dim tbDocTemplates As DataTable = CreateDataTable(strQuery)
                If (tbDocTemplates IsNot Nothing AndAlso tbDocTemplates.Rows.Count > 0) Then
                    For Each rowDocTemplate As DataRow In tbDocTemplates.Rows
                        Dim documentTemplate = New roDocumentTemplate
                        documentTemplate = LoadDocumentTemplate(rowDocTemplate("Id"))
                        If availableAreas.Contains(CInt(documentTemplate.Area)) Then bolRet.Add(documentTemplate)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetAvailableDocumentTemplateByType")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetAvailableDocumentTemplateByType")
            Finally

            End Try
            Return bolRet
        End Function

        Public Function GetAvailableDocumentTemplateByTypeForEmployee(ByVal docType As DocumentType, ByVal idRelatedObject As Integer, ByVal minPermission As Permission, Optional ByVal bAudit As Boolean = False) As List(Of roDocumentTemplate)
            Dim bolRet As New List(Of roDocumentTemplate)
            Dim tbTemplates As DataTable = Nothing
            Dim strWhere As String = String.Empty

            Try

                Dim availableAreas As New Generic.List(Of Integer)

                Dim idPassport As Integer = 1
                If Me.oState.IDPassport > 0 Then idPassport = Me.oState.IDPassport

                Dim strQuery As String = String.Empty

                strQuery = " @SELECT# Id, Name, ShortName, Description FROM DocumentTemplates WHERE EmployeeDeliverAllowed=1 and RequiresSigning=0 and Scope in (1,0,5) "

                Dim tbDocTemplates As DataTable = CreateDataTable(strQuery)
                If (tbDocTemplates IsNot Nothing AndAlso tbDocTemplates.Rows.Count > 0) Then
                    For Each rowDocTemplate As DataRow In tbDocTemplates.Rows
                        Dim documentTemplate = New roDocumentTemplate
                        documentTemplate = LoadDocumentTemplate(rowDocTemplate("Id"))
                        bolRet.Add(documentTemplate)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetAvailableDocumentTemplateByType")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetAvailableDocumentTemplateByType")
            Finally

            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene lista de plantillas asociadas a una justificación
        ''' </summary>
        ''' <param name="idAbsence"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetTemplateDocumentsAvailableByAbsence(ByVal idAbsence As Integer, ByVal idEmployee As Integer, ByVal eforecast As ForecastType, Optional isStarting As Boolean = False, Optional ByVal bAudit As Boolean = False) As List(Of roDocumentTemplate)
            Dim bolRet As New List(Of roDocumentTemplate)
            Dim tbTemplates As DataTable = Nothing
            Dim strWhere As String = String.Empty

            Try

                Dim idCause As Integer = -1
                Dim strSQL As String = String.Empty
                Select Case eforecast
                    Case ForecastType.AbsenceHours
                        strSQL = "@SELECT# IDCause FROM ProgrammedCauses WHERE AbsenceId = " & idAbsence
                    Case ForecastType.AbsenceDays, ForecastType.Leave
                        strSQL = "@SELECT# IDCause FROM ProgrammedAbsences WHERE AbsenceId = " & idAbsence
                    Case ForecastType.OverWork
                        strSQL = "@SELECT# IDCause FROM ProgrammedOvertimes WHERE ID = " & idAbsence
                    Case Else

                End Select

                If strSQL <> String.Empty Then
                    idCause = roTypes.Any2Integer(ExecuteScalar(strSQL))
                    bolRet = GetCauseAvailableDocumentTemplateByEmployee(idCause, idEmployee, isStarting)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetCauseAvailableDocumentTemplate")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetCauseAvailableDocumentTemplate")
            Finally

            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene lista de plantillas asociadas a una justificación
        ''' </summary>
        ''' <param name="idCause"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetCauseAvailableDocumentTemplateByEmployee(ByVal idCause As Integer, ByVal idEmployee As Integer, Optional isStarting As Boolean = False, Optional ByVal bAudit As Boolean = False) As List(Of roDocumentTemplate)
            Dim bolRet As New List(Of roDocumentTemplate)
            Dim tbTemplates As DataTable = Nothing
            Dim strWhere As String = String.Empty

            Try

                Dim idCurrentLabAgree As Integer = 0

                Dim oContractState As New Contract.roContractState(oState.IDPassport)
                Dim oContract As Contract.roContract = Contract.roContract.GetActiveContract(idEmployee, oContractState, False)

                If oContractState.Result = ContractsResultEnum.NoError OrElse oContractState.Result = ContractsResultEnum.ContractNotFound Then

                    If oContract IsNot Nothing Then
                        If oContract.LabAgree IsNot Nothing Then
                            idCurrentLabAgree = oContract.LabAgree.ID
                        Else
                            idCurrentLabAgree = -1
                        End If
                    Else
                        idCurrentLabAgree = -1
                    End If

                    Dim strQuery = "@SELECT# DocumentTemplates.Id FROM DocumentTemplates" &
                                " inner join CausesDocumentsTracking on CausesDocumentsTracking.IDDocument = DocumentTemplates.Id" &
                                " where CausesDocumentsTracking.IDCause = " & idCause &
                                " AND DocumentTemplates.Scope in (" & String.Join(",", GetDocumentTemplateScopesIds(DocumentType.Employee, True)) & ")" &
                                " AND CausesDocumentsTracking.IDLabAgree In(0," & idCurrentLabAgree & ")"
                    If isStarting Then strQuery = strQuery & " AND DocumentTemplates.LeaveDocType = " & LeaveDocumentType.LeaveReport

                    Dim tbDocTemplates As DataTable = CreateDataTable(strQuery)
                    If (tbDocTemplates IsNot Nothing AndAlso tbDocTemplates.Rows.Count > 0) Then
                        For Each rowDocTemplate As DataRow In tbDocTemplates.Rows
                            bolRet.Add(LoadDocumentTemplate(rowDocTemplate("Id")))
                        Next
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetCauseAvailableDocumentTemplate")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetCauseAvailableDocumentTemplate")
            Finally

            End Try
            Return bolRet
        End Function

        Public Function SplitA3PayrollDocument(ByVal document As roDocument, Optional ByVal bAudit As Boolean = False) As List(Of roDocument)
            Dim documentsList As New List(Of roDocument)

            Try
                Dim documentPprocessor As PdfDocumentProcessor = New PdfDocumentProcessor

                ' Cargamos la configuración de plantilla
                Dim a3payrolltemplate As DTOs.PayrollFormTemplate = New DTOs.PayrollFormTemplate
                Dim advA3payrollTemplate As roAdvancedParameter = New roAdvancedParameter("Documents.A3PayrollTemplate", New roAdvancedParameterState())
                If advA3payrollTemplate.Value.Trim = String.Empty Then
                    a3payrolltemplate.RecognitionType = PayrollRecognitionMechanism.ReferenceWord
                    a3payrolltemplate.ReferenceWord = "D.N.I."
                    a3payrolltemplate.ReferenceOffsetX = -40
                    a3payrolltemplate.ReferenceOffsetY = -12
                    a3payrolltemplate.Width = 70
                    a3payrolltemplate.Height = 9
                Else
                    a3payrolltemplate = roJSONHelper.Deserialize(Of DTOs.PayrollFormTemplate)(advA3payrollTemplate.Value)
                End If

                ' Validamos documento A3
                ' 0: El documento es un pdf válido
                Dim documentStream As New MemoryStream(document.Document)
                Try
                    documentPprocessor.LoadDocument(documentStream)
                Catch ex As Exception
                    Me.oState.Result = DocumentResultEnum.InvalidA3PayrollFormat
                    Return documentsList
                End Try

                ' 1: El documento existe y tiene al menos una página
                If documentPprocessor.Document Is Nothing OrElse documentPprocessor.Document.Pages.Count = 0 Then
                    Me.oState.Result = DocumentResultEnum.A3PayrollDocumentEmpty
                    Return documentsList
                End If

                ' 2: En modo palabra por referencia, calidamos que en una página aparece una única vez la palabra de referencia, diferenciando entre mayúsculas y minúsculas
                If a3payrolltemplate.RecognitionType = PayrollRecognitionMechanism.ReferenceWord Then
                    Dim pageText As String = documentPprocessor.GetPageText(1)
                    Dim count As Integer = UBound(pageText.Split({a3payrolltemplate.ReferenceWord}, StringSplitOptions.None))
                    If count = 0 OrElse count > 1 Then
                        Me.oState.Result = DocumentResultEnum.InvalidA3PayrollFormat
                        Return documentsList
                    End If
                End If

                Dim pdfSearchParameters As PdfTextSearchParameters = New PdfTextSearchParameters()
                pdfSearchParameters.WholeWords = True
                pdfSearchParameters.CaseSensitive = True

                Dim searchRpdfSearchParametersesults As PdfTextSearchResults = documentPprocessor.FindText(a3payrolltemplate.ReferenceWord, pdfSearchParameters)
                Dim x As Double = searchRpdfSearchParametersesults.Words(0).Rectangles(0).Vertices(3).X + a3payrolltemplate.ReferenceOffsetX
                Dim y As Double = searchRpdfSearchParametersesults.Words(0).Rectangles(0).Vertices(3).Y + a3payrolltemplate.ReferenceOffsetY

                Dim searchRect As New PdfRectangle(x, y, x + a3payrolltemplate.Width, y + a3payrolltemplate.Height)
                Dim area As PdfDocumentArea
                Dim extractedText As String

                ' Recorrer cada página y guardamos página y NIF
                Dim totalPages As Integer = documentPprocessor.Document.Pages.Count
                Dim nifDictionaries As New Dictionary(Of Integer, String)

                For pageIndex As Integer = 1 To totalPages
                    area = New PdfDocumentArea(pageIndex, searchRect)
                    extractedText = documentPprocessor.GetText(area)
                    If extractedText = String.Empty Then extractedText = "???"
                    nifDictionaries.Add(pageIndex, extractedText.Replace(vbCrLf, ""))
                Next

                ' Recorro las entradas del diccionario nifDictioanries
                Dim filenames As New List(Of String)
                Dim filename As String = String.Empty
                Dim outputDocumentPprocessor As PdfDocumentProcessor = New PdfDocumentProcessor
                Dim outstream As MemoryStream
                Dim splittedDocument As roDocument
                For Each entry As KeyValuePair(Of Integer, String) In nifDictionaries
                    outputDocumentPprocessor.CreateEmptyDocument()
                    outputDocumentPprocessor.Document.Pages.Add(documentPprocessor.Document.Pages(entry.Key - 1))
                    outstream = New MemoryStream
                    outputDocumentPprocessor.SaveDocument(outstream)
                    splittedDocument = New roDocument

                    splittedDocument.Id = -1
                    splittedDocument.DocumentTemplate = document.DocumentTemplate
                    splittedDocument.IdEmployee = 0
                    'Si hay más de una página para el mismo DNI, a todas menos la primera le añado el número de página para generar documentos diferentes y que no se sobreescriban
                    filename = $"{entry.Value}_{document.Title}"
                    If filenames.Contains(filename) Then
                        filename = $"{entry.Value}_{document.Title}_{entry.Key}"
                    End If
                    filenames.Add(filename)
                    splittedDocument.Title = filename
                    splittedDocument.Document = outstream.ToArray()
                    splittedDocument.DeliveryChannel = document.DeliveryChannel
                    splittedDocument.DeliveredDate = document.DeliveredDate
                    splittedDocument.DocumentType = document.DocumentType
                    splittedDocument.DeliveredBy = "SYSTEM"

                    documentsList.Add(splittedDocument)
                Next


            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SplitA3PayrollDocument")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SplitA3PayrollDocument")
            End Try
            Return documentsList
        End Function

        ''' <summary>
        ''' Obtiene lista de plantillas asociadas a una justificación
        ''' </summary>
        ''' <param name="idCause"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetCauseDocumentTemplate(ByVal idCause As Integer, ByVal idEmployee As Integer, Optional ByVal bAudit As Boolean = False) As List(Of roDocumentTemplate)
            Dim bolRet As New List(Of roDocumentTemplate)
            Dim tbTemplates As DataTable = Nothing
            Dim strWhere As String = String.Empty

            Try

                Dim idCurrentLabAgree As Integer = 0

                Dim oContractState As New Contract.roContractState(oState.IDPassport)
                Dim oContract As Contract.roContract = Contract.roContract.GetActiveContract(idEmployee, oContractState, False)

                If oContractState.Result = ContractsResultEnum.NoError OrElse oContractState.Result = ContractsResultEnum.ContractNotFound Then

                    If oContract IsNot Nothing Then
                        If oContract.LabAgree IsNot Nothing Then
                            idCurrentLabAgree = oContract.LabAgree.ID
                        Else
                            idCurrentLabAgree = -1
                        End If
                    Else
                        idCurrentLabAgree = -1
                    End If

                    Dim strQuery = "@SELECT# DocumentTemplates.Id FROM DocumentTemplates" &
                                " inner join CausesDocumentsTracking on CausesDocumentsTracking.IDDocument = DocumentTemplates.Id" &
                                " where CausesDocumentsTracking.IDCause = " & idCause &
                                " AND DocumentTemplates.Scope in (4)" &
                                " AND CausesDocumentsTracking.IDLabAgree In(0," & idCurrentLabAgree & ")"

                    Dim tbDocTemplates As DataTable = CreateDataTable(strQuery)
                    If (tbDocTemplates IsNot Nothing AndAlso tbDocTemplates.Rows.Count > 0) Then
                        For Each rowDocTemplate As DataRow In tbDocTemplates.Rows
                            bolRet.Add(LoadDocumentTemplate(rowDocTemplate("Id")))
                        Next
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetCauseDocumentTemplate")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetCauseDocumentTemplate")
            Finally

            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene lista de plantillas por tipo: empleado o empresa
        ''' </summary>
        ''' <param name="docType"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetTemplateDocumentsByType(ByVal docType As DocumentType, Optional ByVal bAudit As Boolean = False) As List(Of roDocumentTemplate)
            Dim bolRet As New List(Of roDocumentTemplate)
            Dim tbTemplates As DataTable = Nothing

            Try

                'TODO: ¿Se debe aplicar seguridad?. ¿Desde dónde se llama?
                Dim strQuery As String = String.Empty
                Select Case docType
                    Case DocumentType.Employee
                        strQuery = " @SELECT# Id, Name, ShortName, Description FROM DocumentTemplates WHERE Scope in (" & String.Join(",", GetDocumentTemplateScopesIds(DocumentType.Employee)) & ") ORDER BY Name"
                    Case DocumentType.Company
                        strQuery = " @SELECT# Id, Name, ShortName, Description FROM DocumentTemplates WHERE Scope in (" & String.Join(",", GetDocumentTemplateScopesIds(DocumentType.Company)) & ") ORDER BY Name"
                End Select

                Dim tbDocTemplates As DataTable = CreateDataTable(strQuery)
                If (tbDocTemplates IsNot Nothing AndAlso tbDocTemplates.Rows.Count > 0) Then
                    For Each rowDocTemplate As DataRow In tbDocTemplates.Rows
                        Dim documentTemplate = New roDocumentTemplate
                        documentTemplate = LoadDocumentTemplate(rowDocTemplate("Id"))
                        bolRet.Add(documentTemplate)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetTemplateDocumentsByType")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetTemplateDocumentsByType")
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene lista de plantillas por tipo: empleado o empresa
        ''' </summary>
        ''' <param name="docType"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetTemplateByShortName(ByVal shortname As String) As List(Of roDocumentTemplate)
            Dim bolRet As New List(Of roDocumentTemplate)

            Try
                Dim strQuery As String = String.Empty
                strQuery = $" @SELECT# Id, Name, ShortName, Description FROM DocumentTemplates WHERE ShortName = '{shortname}' ORDER BY Name"

                Dim tbDocTemplates As DataTable = CreateDataTable(strQuery)
                If (tbDocTemplates IsNot Nothing AndAlso tbDocTemplates.Rows.Count > 0) Then
                    For Each rowDocTemplate As DataRow In tbDocTemplates.Rows
                        Dim documentTemplate = New roDocumentTemplate
                        documentTemplate = LoadDocumentTemplate(rowDocTemplate("Id"))
                        bolRet.Add(documentTemplate)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetTemplateByShortName")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetTemplateByShortName")
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene lista de plantillas de autorizaciones de acceso
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetAccessAuthorizationsTemplates(Optional ByVal bAudit As Boolean = False) As List(Of roDocumentTemplate)
            Dim bolRet As New List(Of roDocumentTemplate)
            Dim tbTemplates As DataTable = Nothing

            Try

                'TODO: ¿Se debe aplicar seguridad?. ¿Desde dónde se llama?
                Dim strQuery As String = String.Empty
                strQuery = " @SELECT# Id, Name, ShortName, Description FROM DocumentTemplates WHERE Scope in (" & CInt(DocumentScope.CompanyAccessAuthorization) & "," & CInt(DocumentScope.EmployeeAccessAuthorization) & ") ORDER BY Name"

                Dim tbDocTemplates As DataTable = CreateDataTable(strQuery)
                If (tbDocTemplates IsNot Nothing AndAlso tbDocTemplates.Rows.Count > 0) Then
                    For Each rowDocTemplate As DataRow In tbDocTemplates.Rows
                        Dim documentTemplate = New roDocumentTemplate
                        documentTemplate = LoadDocumentTemplate(rowDocTemplate("Id"))
                        bolRet.Add(documentTemplate)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetAccessAuthorizationsTemplates")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetAccessAuthorizationsTemplates")
            Finally

            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' cambio el estado de un documento entregado
        ''' </summary>
        ''' <param name="idDocument"></param>
        ''' <param name="oNewDocStatus"></param>
        ''' <param name="updateStatusDate"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function ChangeDocumentState(ByVal idDocument As Integer, ByVal oNewDocStatus As DocumentStatus, ByVal strSupervisorRemark As String, ByVal updateStatusDate As DateTime, Optional bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bolActionInsert As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim roPassport As roPassportTicket = roPassportManager.GetPassportTicket(oState.IDPassport)
                'Si viene de proceso EOG, no hay passport. Mantengo el que había

                Dim levelAuthoritySup As Integer = 0

                Dim oDocument As roDocument = LoadDocument(idDocument)
                bolRet = (oDocument.Id <> -1)
                If bolRet Then
                    If roPassport IsNot Nothing Then levelAuthoritySup = GetPassportLevelOfAuthority(roPassport.ID, oDocument.DocumentTemplate.Area)
                    oDocument.Status = oNewDocStatus
                    oDocument.LastStatusChange = updateStatusDate
                    oDocument.SupervisorRemarks = strSupervisorRemark

                    If roPassport IsNot Nothing Then
                        oDocument.IdLastStatusSupervisor = roPassport.ID
                    End If
                    If levelAuthoritySup > 0 Then oDocument.StatusLevel = levelAuthoritySup
                    oDocument.Validity = IsDocumentCurrentlyValid(oDocument, Now.Date)
                    bolRet = Me.SaveDocument(oDocument)
                End If

                ' Si el documento caducó, elimino posibles notificaciones para que se generen las nuevas, si aplica
                If bolRet Then
                    If oNewDocStatus = DocumentStatus.Expired AndAlso oDocument.IdEmployee > 0 Then
                        Dim strSQL As String = "@DELETE# sysronotificationtasks where idnotification = 701 and Key1Numeric = " & oDocument.IdEmployee &
                            " and Key2Numeric = " & oDocument.DocumentTemplate.Id.ToString '& " and Executed = 0 "
                        ' Para documentos de absentismo añado tipo de ausencia
                        If oDocument.DocumentTemplate.Scope = DocumentScope.LeaveOrPermission Then
                            If oDocument.IdDaysAbsence > 0 Then
                                strSQL = strSQL & " and Parameters Like 'DAYS%'"
                            ElseIf oDocument.IdHoursAbsence > 0 Then
                                strSQL = strSQL & " and Parameters Like 'HOURS%'"
                            End If
                        End If
                        bolRet = ExecuteSql(strSQL)
                    End If
                End If

                ' Genero aviso de denegación e invalidación de documento para que el empleado lo pueda saber
                If bolRet Then
                    Select Case oNewDocStatus
                        Case DocumentStatus.Invalidated, DocumentStatus.Rejected
                            Dim strSQL As String = String.Empty
                            strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters, FiredDate) VALUES " &
                                                "(751, " & oDocument.IdEmployee & "," & oDocument.Id & ",'" & oNewDocStatus.ToString & "', " & roTypes.Any2Time(DateTime.Now).SQLSmallDateTime() & ")"
                            bolRet = ExecuteSql(strSQL)
                            If oNewDocStatus = DocumentStatus.Rejected AndAlso oDocument.DocumentTemplate.Notifications.Contains("703") Then
                                strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters, FiredDate, Key5Numeric) VALUES " &
                                                    "(703, " & oDocument.IdEmployee & "," & oDocument.Id & ",'" & oNewDocStatus.ToString & ";" & strSupervisorRemark & "', " & roTypes.Any2Time(DateTime.Now).SQLSmallDateTime() & ",1)"
                                bolRet = ExecuteSql(strSQL)
                                strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters, FiredDate, Key5Numeric) VALUES " &
                                                    "(703, " & oDocument.IdEmployee & "," & oDocument.Id & ",'" & oNewDocStatus.ToString & ";" & strSupervisorRemark & "', " & roTypes.Any2Time(DateTime.Now).SQLSmallDateTime() & ",0)"
                                bolRet = ExecuteSql(strSQL)
                            End If
                    End Select
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.AddAuditParameter(tbParameters, "{Status}", oNewDocStatus.ToString, "", 1)
                    Me.oState.Audit(If(bolActionInsert, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tDocument, oDocument.Title, tbParameters, -1)
                End If
            Catch ex As DbException
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::ChangeDocumentState")
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::ChangeDocumentState")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Elimina un documento entregado
        ''' </summary>
        ''' <param name="idDocument"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function DeleteDocument(ByVal idDocument As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oDocManager As New roDocumentManager
                Dim oDoc As New roDocument
                oDoc = oDocManager.LoadDocument(idDocument, , False)

                Dim strQuery As String = String.Empty
                Dim strTitle As String = oDoc.Title

                strQuery = "@DELETE# Documents WHERE ID = " & idDocument
                If Not ExecuteSql(strQuery) Then
                    oState.Result = DocumentResultEnum.ErrorDeletingDocument
                    oState.ErrorText = ""
                    bolRet = False
                Else
                    If oDoc.BlobFileName?.Length > 0 Then
                        Robotics.Azure.RoAzureSupport.DeleteFileFromCompanyContainer(oDoc.BlobFileName, roLiveQueueTypes.documents.ToString, roLiveQueueTypes.documents)

                        If oDoc.SignStatus = SignStatusEnum.Signed Then
                            Azure.RoAzureSupport.DeleteFileFromBlob(oDoc.BlobFileName & _SignreportExt, roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName)
                        End If
                    End If
                    bolRet = True
                End If

                ' Si es un documento de tipo campo de la ficha, debo borrar el valor
                If bolRet AndAlso oDoc.DocumentTemplate.Scope = DocumentScope.EmployeeField Then
                    strQuery = "@DELETE# EmployeeuserFieldValues WHERE convert(nvarchar,Value) =  '" & idDocument.ToString & "'"
                    If Not ExecuteSql(strQuery) Then
                        oState.Result = DocumentResultEnum.ErrorDeletingDocument
                        oState.ErrorText = ""
                        bolRet = False
                    Else
                        bolRet = True
                    End If
                End If

                ' Lanzo broadcaster si aplica
                If bolRet AndAlso (oDoc.DocumentTemplate.Scope = DocumentScope.CompanyAccessAuthorization OrElse oDoc.DocumentTemplate.Scope = DocumentScope.EmployeeAccessAuthorization) Then
                    roConnector.InitTask(TasksType.BROADCASTER)
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos borrado de actividad
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{DocumentTitle}", strTitle, "", 1)
                    bolRet = oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tDocument, strTitle, tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::DeleteDocument")
                oState.Result = DocumentResultEnum.ErrorDeletingDocument
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::DeleteDocument")
                oState.Result = DocumentResultEnum.ErrorDeletingDocument
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Subimos el documento a firmar a Validate ID y retornamos la URL para el signante
        ''' </summary>
        ''' <param name="idDocument"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Public Function UploadDocumenttoSign(ByVal idDocument As Integer, Optional ByVal bAudit As Boolean = False) As DocumentVID_PostDocResult
            Dim bolRet As DocumentVID_PostDocResult = Nothing

            Try

                ' Obtenemos el documento
                Dim oDocument As roDocument = LoadDocument(idDocument)

                If oDocument IsNot Nothing Then
                    Dim docContent As Byte() = GetDocumentBytesById(oDocument.Id).DocumentContent
                    oDocument.Weight = docContent.Length
                    oDocument.Document = docContent
                Else
                    oState.Result = DocumentResultEnum.EmptyFile
                    oState.ErrorText = ""
                    Return bolRet
                End If

                ' Obtenemos el nº total de documentos signados actualmente + 1
                Dim mCount As Long = GetTotalSignedDocumentsOnYear(Now.Year, oState) + 1
                Dim oServerLicense As New roServerLicense
                If Not oServerLicense.FeatureIsInstalled("Feature\Signdocument") OrElse mCount > roTypes.Any2Long(oServerLicense.FeatureData("VisualTime Server", "MaxSigndocuments")) Then
                    oState.Result = DocumentResultEnum.NumberOfSignedDocumentsExceeded
                    oState.ErrorText = ""
                    Return bolRet
                End If

                ' Creamos  un objeto válido para ser publicado en Validate ID
                ' con toda la info necesaria
                Dim oValidateIDSupport As New RoValidateIDSupport
                Dim DocData As New DocumentVID_BasicData
                DocData.EmployeeName = oDocument.EmployeeName ' "Lluis Casamayor"
                DocData.FileBytes = oDocument.Document 'File.ReadAllBytes("c:\work\Justificaciones.pdf")
                DocData.FileName = oDocument.Title '"Trial_Remote.pdf"
                DocData.IssuerName = "VisualTime" ' Emisor
                DocData.NIF = ""
                Dim IDEmployeeField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(oDocument.IdEmployee, "NIF", Now.Date, New VTUserFields.UserFields.roUserFieldState(-1), False)
                If IDEmployeeField IsNot Nothing AndAlso IDEmployeeField.FieldValue IsNot Nothing Then
                    DocData.NIF = VTBase.roTypes.Any2String(IDEmployeeField.FieldValue).Trim
                End If

                If DocData.NIF.Length = 0 Then DocData.NIF = oDocument.IdEmployee
                ' Obtenemos el movil del empleado
                'Format E164 Ex: +34666666666
                Dim strFieldmobile As String = ""
                DocData.PhoneNumber = strFieldmobile
                Dim obj As Object = DataLayer.AccessHelper.ExecuteScalar("@SELECT# isnull(FieldName, '') from sysroUserFields where Alias like 'sysroMobile'")

                If Not IsDBNull(obj) Then
                    strFieldmobile = VTBase.roTypes.Any2String(obj)
                    If strFieldmobile.Length = 0 Then
                        strFieldmobile = "Móvil"
                    End If
                    IDEmployeeField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(oDocument.IdEmployee, strFieldmobile, Now.Date, New VTUserFields.UserFields.roUserFieldState(-1), False)
                    If IDEmployeeField IsNot Nothing AndAlso IDEmployeeField.FieldValue IsNot Nothing Then
                        DocData.PhoneNumber = IDEmployeeField.FieldValue
                        If DocData.PhoneNumber.Length > 0 Then
                            ' Aseguro formato correcto para VIDSigner (norma E.164).
                            Dim sPhone As String = DocData.PhoneNumber.Trim
                            ' Elimino espacios y posibles carácteres no permitidos
                            sPhone = Regex.Replace(sPhone, "[^0-9+]", "")
                            ' El número debe obligatoriamente incluir el código de país empezando por +
                            If sPhone.StartsWith("+") Then
                                ' Código de país informado. Únicamente quito posibles ceros a la derecha del símbolo + porque VIDSigner no los soporta
                                sPhone = Regex.Replace(sPhone, "^\+0+", "+")
                            Else
                                ' Sin código de país, o sin marcarlo con el +
                                If sPhone.Length = 9 Then
                                    ' Teléfonos sin + y longitud 9, presupongo es un móvíl español. Añado código de país.
                                    sPhone = $"+34{sPhone}"
                                Else
                                    ' Suponemos han indicado el código de país, pero no han puesto el +, que es obligatorio. Quito ceros por la izquierda, y añado el +
                                    sPhone = $"+{sPhone.TrimStart("0"c)}"
                                End If
                            End If

                            DocData.PhoneNumber = sPhone
                        End If
                    End If
                End If

                If DocData.PhoneNumber.Length = 0 OrElse DocData.PhoneNumber.Length > 16 Then
                    ' Numero de movil incorrecto
                    oState.Result = DocumentResultEnum.InvalidMobileNumber
                    oState.ErrorText = ""
                    Return bolRet
                End If

                Dim xDocVID As DocumentDTO = oValidateIDSupport.CreateDocWithRemoteSigner(DocData, oState.Language.GetLanguageCulture().Name)

                ' Enviamos el doc a ValidateID
                Dim oPostAsyncResult As DocumentVID_PostDocResult = oValidateIDSupport.PostDocument(xDocVID)
                If oPostAsyncResult.IsValid Then
                    bolRet = oPostAsyncResult
                Else
                    oState.Result = DocumentResultEnum.ErrorUploadingDocumentToSign
                    oState.ErrorText = ""
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roDocumentManager::UploadDocumenttoSign:PostsDocument error:: Parameters: " & DocData.PhoneNumber & " " & DocData.EmployeeName & " " & DocData.FileName)
                    Return bolRet
                End If

                If bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDocument, DocData.FileName, tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::UploadDocumenttoSign")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::UploadDocumenttoSign")
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Obtenemos el estado del documento en ValidateID
        ''' </summary>
        ''' <param name="GUIDDoc"></param>
        ''' <returns></returns>
        Public Function DocumentSignatureStatus(ByVal GUIDDoc As String, Optional ByVal bAudit As Boolean = False) As DocumentInfoDTO
            Dim bolRet As DocumentInfoDTO = Nothing

            Dim oValidateIDSupport As New RoValidateIDSupport

            Try

                ' Obtenemos la info del documento
                bolRet = oValidateIDSupport.GetDocumentInfo(GUIDDoc)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::DocumentSignatureStatus")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::DocumentSignatureStatus")
            Finally

            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Obtenemos el documento firmado en Validate ID
        ''' </summary>
        ''' <param name="GUIDDoc"></param>
        ''' <param name="documentid"></param>
        ''' <returns></returns>
        Public Function DownloadSignedDocument(ByVal GUIDDoc As String, ByVal documentid As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim oValidateIDSupport As New RoValidateIDSupport

            Try

                ' Obtenemos el documento firmado
                Dim oDocumentInfoDTO As SignedDocumentDTO = oValidateIDSupport.GetSignedDocument(GUIDDoc)
                If oDocumentInfoDTO IsNot Nothing Then
                    ' Obtenemos el informe de evidencias
                    Dim oSignedDocumentReport As SignedDocumentReportDTO = oValidateIDSupport.GetSignedDocumentReport(GUIDDoc)

                    If oSignedDocumentReport IsNot Nothing Then
                        ' Guardamos los documentos
                        Dim oDocument As roDocument = LoadDocument(documentid)
                        If oDocument IsNot Nothing AndAlso oDocument.Id > 0 Then
                            oDocument.Document = Convert.FromBase64String(oDocumentInfoDTO.DocContent)
                            oDocument.SignReport = Convert.FromBase64String(oSignedDocumentReport.DocContent)
                            oDocument.SignStatus = SignStatusEnum.Signed
                            bolRet = SaveDocument(oDocument, True)
                        End If
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::DownloadSignedDocument")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::DownloadSignedDocument")
            Finally

            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' validamos el estado del documento en ValidateID y en funcion de su estado realizamos las acciones oportunas
        ''' </summary>
        ''' <param name="GUIDDoc"></param>
        ''' <param name="documentid"></param>
        ''' <returns></returns>
        Public Function ValidateSignStatusDocument(ByVal GUIDDoc As String, ByVal documentid As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            '
            ' validamos el estado del documento en ValidateID y en funcion de su estado realizamos las acciones oportunas
            '

            Dim bolRet As Boolean = False

            Dim oValidateIDSupport As New RoValidateIDSupport
            Dim oDocument As roDocument = Nothing
            Try

                ' Obtenemos el documento
                oDocument = LoadDocument(documentid)
                If oDocument IsNot Nothing AndAlso oDocument.Id > 0 AndAlso oDocument.SignStatus = SignStatusEnum.InProgress Then
                    ' Obtenemos el estado del documento
                    Dim DocumentStatus As DocumentInfoDTO = DocumentSignatureStatus(GUIDDoc)
                    If DocumentStatus IsNot Nothing Then
                        Select Case DocumentStatus.DocStatus
                            Case "Signed"
                                ' Obtenemos el documento firmado
                                Dim oDocumentInfoDTO As SignedDocumentDTO = oValidateIDSupport.GetSignedDocument(GUIDDoc)
                                If oDocumentInfoDTO IsNot Nothing Then
                                    ' Obtenemos el informe de evidencias
                                    Dim oSignedDocumentReport As SignedDocumentReportDTO = oValidateIDSupport.GetSignedDocumentReport(GUIDDoc)

                                    If oSignedDocumentReport IsNot Nothing Then
                                        ' Guardamos los documentos
                                        oDocument.Document = Convert.FromBase64String(oDocumentInfoDTO.DocContent)
                                        oDocument.SignReport = Convert.FromBase64String(oSignedDocumentReport.DocContent)
                                        oDocument.SignStatus = SignStatusEnum.Signed
                                        oDocument.SignDate = Now
                                        bolRet = SaveDocument(oDocument, True)

                                        ' Actualizamos el contador de documentos firmados
                                        Dim strSQL = "@UPDATE# sysroParameters SET Data = CONVERT(nvarchar(max),CONVERT(numeric(18,0), CONVERT(nvarchar(max),Data)) + 1)  WHERE ID = 'VID_SDocs'"
                                        ExecuteSql(strSQL)

                                        ' Actualizamos log de firmas
                                        strSQL = $"@INSERT# INTO sysroTrackedEvents (Event, IDObject, Date) VALUES ('{TrackedEvent.DocumentSigned}', {oDocument.Id}, {roTypes.Any2Time(oDocument.SignDate).SQLSmallDateTime})"
                                        ExecuteSql(strSQL)
                                    End If
                                End If

                            Case "Rejected"
                                ' Actualizamos el estado del documento a rechazado
                                oDocument.SignStatus = SignStatusEnum.Rejected
                                bolRet = SaveDocument(oDocument, True)
                            Case Else
                                ' No hacemos nada , solo actualizamos el estado a pendiente
                                oDocument.SignStatus = SignStatusEnum.Pending
                                bolRet = SaveDocument(oDocument, True)
                        End Select
                    Else
                        oDocument.SignStatus = SignStatusEnum.Pending
                        bolRet = SaveDocument(oDocument, True)
                    End If
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::ValidateSignStatusDocument")
                If oDocument IsNot Nothing AndAlso oDocument.SignStatus = SignStatusEnum.InProgress Then
                    oDocument.SignStatus = SignStatusEnum.Pending
                    SaveDocument(oDocument, True)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::ValidateSignStatusDocument")
                If oDocument IsNot Nothing AndAlso oDocument.SignStatus = SignStatusEnum.InProgress Then
                    oDocument.SignStatus = SignStatusEnum.Pending
                    SaveDocument(oDocument, True)
                End If
            Finally

            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Marcamos el documento en proceso de firma
        ''' </summary>
        ''' <param name="GUIDDoc"></param>
        ''' <param name="documentid"></param>
        ''' <returns></returns>
        Public Function SignStatusDocumentInProgress(ByVal GUIDDoc As String, ByVal documentid As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            '
            ' Marcamos el documento en proceso de firma  y creamos la tarea para procesarlo
            '

            Dim bolRet As Boolean = False
            Dim oValidateIDSupport As New RoValidateIDSupport

            Try

                ' Guardamos los documentos
                Dim oDocument As roDocument = LoadDocument(documentid)
                If oDocument IsNot Nothing AndAlso oDocument.Id > 0 Then
                    oDocument.SignStatus = SignStatusEnum.InProgress
                    bolRet = SaveDocument(oDocument, True)

                    If bolRet Then
                        Dim oStateTask As New roLiveTaskState(roTypes.Any2Integer(roConstants.GetSystemUserId()))

                        Dim oParameters As New roCollection
                        oParameters.Add("GUIDDoc", GUIDDoc)
                        oParameters.Add("DocumentID", documentid)

                        roLiveTask.CreateLiveTask(roLiveTaskTypes.ValidateSignStatusDocument, oParameters, oStateTask)

                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SignStatusDocumentInProgress")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::SignStatusDocumentInProgress")
            End Try
            Return bolRet
        End Function


        Public Function GetSystemDocumentList(scope As DocumentScope, bAudit As Boolean) As List(Of roDocument)
            Dim lstRet As New List(Of roDocument)

            Try
                oState.Result = DocumentResultEnum.NoError

                Dim documentTemplate As roDocumentTemplate = LoadSystemTemplate(scope)

                If documentTemplate IsNot Nothing Then lstRet = GetDocumentsByTemplate(documentTemplate)

                If bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tDocument, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetSystemDocumentList")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetSystemDocumentList")
            End Try

            Return lstRet
        End Function

#Region "Documentation faults"

        ''' <summary>
        ''' Devuelve las alertas a mostrar en DeskTop
        ''' </summary>
        ''' <param name="idRelatedObject">Identificador del objeto empresa/empleado</param>
        ''' <param name="type">Tipo del objecto que consultamos</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDocumentationFaultAlerts(ByVal idRelatedObject As Integer, ByVal type As DocumentType, ByVal idForecast As Integer, Optional bCheckPermissions As Boolean = True, Optional dCheckOnDate As DateTime = Nothing, Optional forecastType As DTOs.ForecastType = DTOs.ForecastType.AnyAbsence, Optional iLastNMonths As Integer = -1, Optional checkStatusLevel As Boolean = False, Optional bOnlyCheck As Boolean = False) As DocumentAlerts
            Dim oAlerts As New DocumentAlerts
            Dim dicStats As New Dictionary(Of String, Double)
            Dim bLogStats As Boolean = False

            ' Stats ON
            Dim watch As Stopwatch = Nothing
            Dim oParam As roAdvancedParameter
            oParam = New roAdvancedParameter("AlertsStatsOn", New roAdvancedParameterState)
            bLogStats = (roTypes.Any2String(oParam.Value) = "1")

            If bLogStats Then
                'watch = New Stopwatch
                watch = Stopwatch.StartNew
            End If

            Try
                If idForecast <= 0 Then
                    '--------- Documentos entregados (fuera o no necesario), y que requiriendo validación, están en un estado incorrecto ------------------------
                    'Necesito medir el tiempo en segundos que tarda la función en eje   

                    oAlerts.DocumentsValidation = GetDocumentationFaultAlerts_ValidateDocuments(idRelatedObject, type, checkStatusLevel)
                    If bLogStats AndAlso watch IsNot Nothing Then AddStat("ValidateDocuments " & type.ToString, dicStats, watch)
                    If bOnlyCheck AndAlso oAlerts.DocumentsValidation.Any() Then Return oAlerts

                    '----------------------- Documentos obligatorios no entregados -------------------------
                    oAlerts.MandatoryDocuments = GetDocumentationFaultAlerts_UndeliveredDocuments(idRelatedObject, type, dCheckOnDate)
                    If bLogStats AndAlso watch IsNot Nothing Then AddStat("UndeliveredDocuments " & type.ToString, dicStats, watch)
                    If bOnlyCheck AndAlso oAlerts.MandatoryDocuments.Any() Then Return oAlerts

                    '----------------------- Documentos de Autorizaciones de acceso ------------------------
                    oAlerts.AccessAuthorizationDocuments = GetAccessAuthorizationDocumentationFaultAlerts(idRelatedObject, type, , bCheckPermissions, dCheckOnDate, checkStatusLevel)
                    If bLogStats AndAlso watch IsNot Nothing Then AddStat("AccessAuthorization " & type.ToString, dicStats, watch)
                    If bOnlyCheck AndAlso oAlerts.AccessAuthorizationDocuments.Any() Then Return oAlerts
                Else
                    oAlerts.DocumentsValidation = {}
                    oAlerts.MandatoryDocuments = {}
                    oAlerts.WorkForecastDocuments = {}
                End If

                If (type = DocumentType.Employee) Then
                    '----------------------- Alertas de doc de absentismo de empleado -------------------------
                    oAlerts.AbsenteeismDocuments = {}
                    oAlerts.WorkForecastDocuments = {}

                    Select Case forecastType
                        Case ForecastType.AbsenceDays, ForecastType.AbsenceHours, ForecastType.Leave, ForecastType.AnyAbsence
                            oAlerts.AbsenteeismDocuments = GetForecastDocumentationFaultAlerts(idRelatedObject, type, idForecast, forecastType, bCheckPermissions, dCheckOnDate, iLastNMonths, checkStatusLevel)
                            If bLogStats AndAlso watch IsNot Nothing Then AddStat("Forecast " & type.ToString & " " & forecastType.ToString, dicStats, watch)
                            If bOnlyCheck AndAlso oAlerts.AbsenteeismDocuments.Any() Then Return oAlerts

                        Case ForecastType.OverWork
                            oAlerts.WorkForecastDocuments = GetForecastDocumentationFaultAlerts(idRelatedObject, type, idForecast, ForecastType.OverWork, bCheckPermissions, dCheckOnDate, iLastNMonths, checkStatusLevel)
                            If bLogStats AndAlso watch IsNot Nothing Then AddStat("Forecast " & type.ToString & " " & forecastType.ToString, dicStats, watch)
                            If bOnlyCheck AndAlso oAlerts.WorkForecastDocuments.Any() Then Return oAlerts

                        Case ForecastType.Any
                            oAlerts.AbsenteeismDocuments = GetForecastDocumentationFaultAlerts(idRelatedObject, type, idForecast, ForecastType.AnyAbsence, bCheckPermissions, dCheckOnDate, iLastNMonths, checkStatusLevel)
                            If bLogStats AndAlso watch IsNot Nothing Then AddStat("Forecast " & type.ToString & " " & ForecastType.AnyAbsence.ToString, dicStats, watch)
                            If bOnlyCheck AndAlso oAlerts.AbsenteeismDocuments.Any() Then Return oAlerts

                            oAlerts.WorkForecastDocuments = GetForecastDocumentationFaultAlerts(idRelatedObject, type, idForecast, ForecastType.OverWork, bCheckPermissions, dCheckOnDate, iLastNMonths, checkStatusLevel)
                            If bLogStats AndAlso watch IsNot Nothing Then AddStat("Forecast " & type.ToString & " " & ForecastType.OverWork.ToString, dicStats, watch)
                            If bOnlyCheck AndAlso oAlerts.WorkForecastDocuments.Any() Then Return oAlerts
                    End Select
                Else

                    oAlerts.GpaAlerts = {}
                    oAlerts.AbsenteeismDocuments = {}
                    oAlerts.WorkForecastDocuments = {}

                    If (type = DocumentType.Company) Then

                        Dim employeesInCompany = VTBusiness.Group.roGroup.GetEmployeesFromGroup(idRelatedObject, "Documents", "U", New VTBusiness.Group.roGroupState)
                        If employeesInCompany IsNot Nothing Then
                            For Each Employee As DataRow In employeesInCompany.Tables(0).Rows
                                Select Case forecastType
                                    Case ForecastType.AbsenceDays, ForecastType.AbsenceHours, ForecastType.Leave, ForecastType.AnyAbsence
                                        oAlerts.AbsenteeismDocuments = oAlerts.AbsenteeismDocuments.Union(GetForecastDocumentationFaultAlerts(Employee.Item("IDEmployee"), DocumentType.Employee, idForecast, forecastType, bCheckPermissions, dCheckOnDate, iLastNMonths, checkStatusLevel)).ToArray
                                        If bOnlyCheck AndAlso oAlerts.AbsenteeismDocuments.Any() Then Return oAlerts

                                    Case ForecastType.OverWork
                                        oAlerts.WorkForecastDocuments = oAlerts.WorkForecastDocuments.Union(GetForecastDocumentationFaultAlerts(Employee.Item("IDEmployee"), DocumentType.Employee, idForecast, ForecastType.OverWork, bCheckPermissions, dCheckOnDate, iLastNMonths, checkStatusLevel)).ToArray
                                        If bOnlyCheck AndAlso oAlerts.WorkForecastDocuments.Any() Then Return oAlerts

                                    Case ForecastType.Any
                                        oAlerts.AbsenteeismDocuments = oAlerts.AbsenteeismDocuments.Union(GetForecastDocumentationFaultAlerts(Employee.Item("IDEmployee"), DocumentType.Employee, idForecast, ForecastType.AnyAbsence, bCheckPermissions, dCheckOnDate, iLastNMonths, checkStatusLevel)).ToArray
                                        If bOnlyCheck AndAlso oAlerts.AbsenteeismDocuments.Any() Then Return oAlerts

                                        oAlerts.WorkForecastDocuments = oAlerts.WorkForecastDocuments.Union(GetForecastDocumentationFaultAlerts(Employee.Item("IDEmployee"), DocumentType.Employee, idForecast, ForecastType.OverWork, bCheckPermissions, dCheckOnDate, iLastNMonths, checkStatusLevel)).ToArray
                                        If bOnlyCheck AndAlso oAlerts.WorkForecastDocuments.Any() Then Return oAlerts
                                End Select
                            Next
                        End If
                    End If
                End If

                Try
                    If bLogStats Then roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDocumentManager::GetDocumentationFaultAlerts::Stats for passport " & oState.IDPassport.ToString & " -> Total: (" & dicStats.Sum(Function(pair) pair.Value) & ")" & vbCrLf & String.Join(vbCrLf, dicStats.ToArray))
                Catch ex As Exception
                End Try

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentationFaultAlerts")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentationFaultAlerts")
            End Try
            Return oAlerts
        End Function

        ''' <summary>
        ''' Documentos entregados que requiren aprobación y no están en estado correcto. No incluyen documentación de absentismo (esta se considera no entregada si el estado no es válido, dado que no require validación)
        ''' </summary>
        ''' <param name="idRelatedObject">Identificador del objeto empresa/empleado</param>
        ''' <param name="type">Tipo del objecto que consultamos</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetDocumentationFaultAlerts_ValidateDocuments(ByVal idRelatedObject As Integer, ByVal type As DocumentType, Optional checkStatusLevel As Boolean = False) As DocumentAlert()
            Dim oDocAlerts As New Generic.List(Of DocumentAlert)
            Dim strWhere1 As String = String.Empty
            Dim strWhere2 As String = String.Empty
            Dim idPassport As Integer = 1

            Try
                Dim oServerLicense As New roServerLicense
                If Not (oServerLicense.FeatureIsInstalled("Feature\Documents") OrElse oServerLicense.FeatureIsInstalled("Feature\Absences")) Then Return oDocAlerts.ToArray

                If Me.oState.IDPassport > 0 Then idPassport = Me.oState.IDPassport

                Dim strQuery As String = String.Empty
                strQuery = $"@SELECT#   valids.isok, 
                                        doc.rownumber, 
                                        doct.id templateid, 
                                        doct.name templatename, 
                                        doct.shortname templateshortname, 
                                        doct.beginvalidity, doct.endvalidity, 
                                        doc.IdEmployee idemp, 
                                        doc.id docid, 
                                        doc.deliverydate deliverydate, 
                                        doc.IdCompany idComp, 
                                        doc.Title doctitle, 
                                        doc.Status docstatus, 
                                        doc.begindate docbegindate, 
                                        doc.enddate docenddate, 
                                        doct.Area area, 
                                        doc.StatusLevel statuslevel,
                                        PODT.SupervisorLevelOfAuthority,
										CASE 
											WHEN ce.IDEmployee IS NOT NULL THEN 1
											ELSE 0
										END AS ContractStatus
                          FROM DocumentTemplates doct  "
                If idRelatedObject > 0 Then
                    Select Case type
                        Case DocumentType.Employee
                            strQuery = strQuery & 'plantillas que aplican a empleados
                                   "Left outer join (@SELECT# row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) as 'RowNumber', * from documents where Status not in (" & DocumentStatus.Rejected & "," & DocumentStatus.Invalidated & "," & DocumentStatus.Expired & ")) doc on doc.IdDocumentTemplate = doct.id  " &
                                   "Left outer join (@SELECT#  'OK' as IsOk, doct.Id IdDocumentTemplate from Documents doc, DocumentTemplates doct where doc.IdDocumentTemplate = doct.Id and doc.IdEmployee = " & idRelatedObject.ToString & "and doc.Status = 1 and (getdate() between Doc.begindate And Doc.enddate) and (doc.StatusLevel = 1 or (doct.ApprovalLevelRequired >= doc.StatusLevel or doct.ApprovalLevelRequired = 0))) valids on  doct.id = valids.IdDocumentTemplate  " &
                                   "LEFT JOIN sysrosubvwCurrentEmployeePeriod ce ON ce.IDEmployee = doc.IdEmployee " &
                                   "INNER JOIN sysrofnSecurity_PermissionsOverDocumentTemplates(0, " & idRelatedObject.ToString & ", -1, " & idPassport.ToString & ") PODT ON PODT.IdDocumentTemplate = doct.Id AND PODT.IdEmployee = doc.IdEmployee " &
                                   "where (getdate() between doct.beginvalidity And doct.endvalidity)  " & 'Es exigible por la plantilla
                                   "And (doc.Status Is null Or doc.Status <> 1 Or (doc.Status = 1 and doc.StatusLevel <> 1) Or (doc.Status = 1 And doc.statuslevel = 1 and (Doc.begindate Is null Or  Doc.enddate Is null) Or (getdate() Not between Doc.begindate And Doc.enddate)))  " & 'Estado incorrecto correcto: no validado, o valdiado por supervisor con nivel insuficiente, o fuera de periodo de validez
                                   "And valids.IsOk Is null   " & ' No se entregó ninguno válido
                                   "And (doc.RowNumber Is null Or doc.RowNumber = 1)  " & ' Hay algún documento entregado
                                   "And doct.ApprovalLevelRequired > 0 " & ' Requiere aprobación
                                   "And doct.scope IN (" & DocumentScope.EmployeeContract & "," & DocumentScope.EmployeeField & ") " &
                                   "And doc.IdEmployee = " & idRelatedObject.ToString & " "
                        Case DocumentType.Company
                            strQuery = strQuery &  'plantillas que aplican a empresas
                                   "Left outer join (@SELECT# row_number() over (partition by IdCompany, IdDocumentTemplate order by deliverydate desc) as 'RowNumber', * from documents where Status not in (" & DocumentStatus.Rejected & "," & DocumentStatus.Invalidated & "," & DocumentStatus.Expired & ")) doc on doc.IdDocumentTemplate = doct.id  " &
                                   "Left outer join (@SELECT#  'OK' as IsOk, doct.Id IdDocumentTemplate from Documents doc, DocumentTemplates doct where doc.IdDocumentTemplate = doct.Id and doc.IdCompany = " & idRelatedObject.ToString & "and doc.Status = 1 and (getdate() between Doc.begindate And Doc.enddate) and (doc.StatusLevel = 1 or (doct.ApprovalLevelRequired >= doc.StatusLevel or doct.ApprovalLevelRequired = 0))) valids on  doct.id = valids.IdDocumentTemplate  " &
                                   "LEFT JOIN sysrosubvwCurrentEmployeePeriod ce ON ce.IDEmployee = doc.IdEmployee " &
                                   "INNER JOIN sysrofnSecurity_PermissionsOverDocumentTemplates(0, -1 , " & idRelatedObject.ToString & ", " & idPassport.ToString & ") PODT ON PODT.IdDocumentTemplate = doct.Id AND PODT.IdCompany = doc.IdCompany " &
                                   "where (getdate() between doct.beginvalidity And doct.endvalidity)  " & 'Es exigible por la plantilla
                                   "And (doc.Status Is null Or doc.Status <> 1 Or (doc.Status = 1 and doc.StatusLevel <> 1) Or (doc.Status = 1 And doc.statuslevel = 1 and (Doc.begindate Is null Or  Doc.enddate Is null) Or (getdate() Not between Doc.begindate And Doc.enddate)))  " & 'Estado incorrecto correcto: no validado, o valdiado por supervisor con nivel insuficiente, o fuera de periodo de validez
                                   "And valids.IsOk Is null   " & ' No se entregó ninguno válido
                                   "And (doc.RowNumber Is null Or doc.RowNumber = 1)  " & ' Hay algún documento entregado
                                   "And doct.ApprovalLevelRequired > 0 " & ' Requiere aprobación
                                   "And doct.scope = " & DocumentScope.Company &
                                   "And doc.IdCompany = " & idRelatedObject.ToString & " "
                    End Select
                Else
                    Select Case type
                        Case DocumentType.Employee
                            strQuery = strQuery &  'plantillas que aplican a empleados
                                    "Left outer join (@SELECT# row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) as 'RowNumber', * from documents where Status not in (" & DocumentStatus.Rejected & "," & DocumentStatus.Invalidated & "," & DocumentStatus.Expired & ")) doc on doc.IdDocumentTemplate = doct.id  " &
                                    "Left outer join (@SELECT#  'OK' as IsOk, doc.idemployee, doct.Id IdDocumentTemplate from Documents doc, DocumentTemplates doct where doc.IdDocumentTemplate = doct.Id and doc.Status = 1 and (getdate() between Doc.begindate And Doc.enddate) and (doc.StatusLevel = 1 or (doct.ApprovalLevelRequired >= doc.StatusLevel or doct.ApprovalLevelRequired = 0))) valids on  doct.id = valids.IdDocumentTemplate and doc.IdEmployee = valids.idemployee " &
                                    "LEFT JOIN sysrosubvwCurrentEmployeePeriod ce ON ce.IDEmployee = doc.IdEmployee " &
                                    "INNER JOIN sysrofnSecurity_PermissionsOverDocumentTemplates(0, 0 , -1, " & idPassport.ToString & ") PODT ON PODT.IdDocumentTemplate = doct.Id AND PODT.IdEmployee = doc.IdEmployee " &
                                    "where (getdate() between doct.beginvalidity And doct.endvalidity)  " & 'Es exigible por la plantilla
                                    "And (doc.Status Is null Or doc.Status <> 1 Or (doc.Status = 1 and doc.StatusLevel <> 1) Or (doc.Status = 1 And doc.statuslevel = 1 and (Doc.begindate Is null Or  Doc.enddate Is null) Or (getdate() Not between Doc.begindate And Doc.enddate)))  " & 'Estado incorrecto correcto: no validado, o valdiado por supervisor con nivel insuficiente, o fuera de periodo de validez
                                    "And valids.IsOk Is null   " & ' No se entregó ninguno válido
                                    "And (doc.RowNumber Is null Or doc.RowNumber = 1)  " & ' Hay algún documento entregado
                                    "And doct.ApprovalLevelRequired > 0 " &' Requiere aprobación
                                    "And doct.scope IN (" & DocumentScope.EmployeeContract & "," & DocumentScope.EmployeeField & ") "
                        Case DocumentType.Company
                            strQuery = strQuery & 'plantillas que aplican a empresas
                                    "Left outer join (@SELECT# row_number() over (partition by IdCompany, IdDocumentTemplate order by deliverydate desc) as 'RowNumber', * from documents where Status not in (" & DocumentStatus.Rejected & "," & DocumentStatus.Invalidated & "," & DocumentStatus.Expired & ")) doc on doc.IdDocumentTemplate = doct.id  " &
                                    "Left outer join (@SELECT#  'OK' as IsOk, doc.idcompany, doct.Id IdDocumentTemplate from Documents doc, DocumentTemplates doct where doc.IdDocumentTemplate = doct.Id and doc.Status = 1 and (getdate() between Doc.begindate And Doc.enddate) and (doc.StatusLevel = 1 or (doct.ApprovalLevelRequired >= doc.StatusLevel or doct.ApprovalLevelRequired = 0))) valids on  doct.id = valids.IdDocumentTemplate and valids.idcompany = doc.idcompany " &
                                    "LEFT JOIN sysrosubvwCurrentEmployeePeriod ce ON ce.IDEmployee = doc.IdEmployee " &
                                    "INNER JOIN sysrofnSecurity_PermissionsOverDocumentTemplates(0, -1 , 0, " & idPassport.ToString & ") PODT ON PODT.IdDocumentTemplate = doct.Id AND PODT.IdCompany = doc.IdCompany " &
                                    "where (getdate() between doct.beginvalidity And doct.endvalidity)  " & 'Es exigible por la plantilla
                                    "And (doc.Status Is null Or doc.Status <> 1 Or (doc.Status = 1 and doc.StatusLevel <> 1) Or (doc.Status = 1 And doc.statuslevel = 1 and (Doc.begindate Is null Or  Doc.enddate Is null) Or (getdate() Not between Doc.begindate And Doc.enddate)))  " & 'Estado incorrecto correcto: no validado, o valdiado por supervisor con nivel insuficiente, o fuera de periodo de validez
                                    "And valids.IsOk Is null   " & ' No se entregó ninguno válido
                                    "And (doc.RowNumber Is null Or doc.RowNumber = 1)  " & ' Hay algún documento entregado
                                    "And doct.ApprovalLevelRequired > 0 " & ' Requiere aprobación
                                    "And doct.scope = " & DocumentScope.Company & " "
                    End Select
                End If

                Dim tbNew As DataTable = CreateDataTable(strQuery)
                If tbNew.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbNew.Rows
                        Dim bSkipAlert As Boolean = False

                        Try
                            If checkStatusLevel AndAlso Not IsDBNull(oRow("statuslevel")) AndAlso Not IsDBNull(oRow("area")) AndAlso roTypes.Any2Integer(oRow("docstatus")) = 1 Then
                                Dim levelAuthoritySup As Integer
                                ' Sólo para pantalla de alertas, no muestro alertas de documentos en estado pendiente de validar si tienen un nivel de validadión igual o superior al mío (dado que no podré hacer nada con ellos)
                                ' Esto sólo para V3
                                ' El nivel
                                levelAuthoritySup = roTypes.Any2Integer(oRow("SupervisorLevelOfAuthority"))
                                bSkipAlert = (levelAuthoritySup >= roTypes.Any2Integer(oRow("statuslevel")))
                            End If
                        Catch ex As Exception
                        End Try

                        If Not bSkipAlert AndAlso roTypes.Any2Boolean(oRow("ContractStatus")) Then
                            Dim oAlert As New DocumentAlert
                            oAlert.IDDocument = oRow("docid")
                            Select Case type
                                Case DocumentType.Employee
                                    oAlert.IdRelatedObject = If(IsDBNull(oRow("idemp")), idRelatedObject, oRow("idemp"))
                                    oAlert.ObjectName = If(oAlert.IdRelatedObject > 0, ExecuteScalar("@SELECT# ISNULL(Name,'') FROM Employees WHERE ID=" & oAlert.IdRelatedObject), "")
                                Case DocumentType.Company
                                    oAlert.IdRelatedObject = If(IsDBNull(oRow("idComp")), idRelatedObject, oRow("idComp"))
                                    oAlert.ObjectName = If(oAlert.IdRelatedObject > 0, ExecuteScalar("@SELECT# ISNULL(Name,'') FROM Groups WHERE ID=" & oAlert.IdRelatedObject), "")
                            End Select

                            'Mensaje para un empleado o empresa. No hace falta poner el nombre ....
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(Me.LoadDocument(oAlert.IDDocument).DocumentTemplate.Name)
                            oAlert.Description = oState.Language.Translate("roDocumentManager.Document.Invalid", "")
                            oState.Language.ClearUserTokens()
                            oAlert.IsUrgent = True

                            oDocAlerts.Add(oAlert)
                        End If

                    Next
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roNotification::GetDocumentationFaultAlerts_ValidateDocuments")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roNotification::GetDocumentationFaultAlerts_ValidateDocuments")
            End Try

            Return oDocAlerts.ToArray()
        End Function

        '' <summary>
        '' Documentos obligatorios que faltan
        '' </summary>
        '' <param name="idRelatedObject">Identificador del objeto empresa/empleado</param>
        '' <param name="type">Tipo del objecto que consultamos</param>
        '' <param name="oActiveTransaction"></param>
        '' <returns></returns>
        '' <remarks></remarks>
        Public Function GetPortalAlerts_UndeliveredDocumentsByEmployee(ByVal idRelatedObject As Integer, ByVal type As DocumentType, Optional dOnDate As DateTime = Nothing) As DocumentAlert()

            Dim oMandatoryDocs As New Generic.List(Of DocumentAlert)
            Dim strQuery As String = String.Empty
            Dim strWhere1 As String = String.Empty
            Dim strWhere2 As String = String.Empty
            Dim idPassport As Integer = 1

            Try
                ' Calculo días de offset para hacer validaciones de validez a futuro (para alertas con previsión)
                Dim dDaysOffset As Integer = 0
                If dOnDate.Year > 1 Then
                    dDaysOffset = dOnDate.Date.Subtract(Now.Date).Days
                End If

                ' DOCUMENTOS INCONDICIONALMENTE OBLIGATORIOS
                strQuery = "@SELECT# doct.id templateid, " & idRelatedObject.ToString & " idemp, doc.IdCompany idComp, NULL absenceid, doct.scope, NULL absencetype, NULL begindate, NULL enddate, NULL starthour, null endhour, null idcause " &
                            "From DocumentTemplates doct "
                strQuery = strQuery &
                                    "Left outer join (@SELECT# * from documents where Status not in (" & DocumentStatus.Rejected & "," & DocumentStatus.Invalidated & "," & DocumentStatus.Expired & ")) doc on doc.IdDocumentTemplate = doct.id And doc.IDEmployee = " & idRelatedObject.ToString & " " & 'plantillas con documentos entregados, contando un documento por plantilla
                                    "where (DateAdd(Day, " & dDaysOffset & ", DateAdd(Day, DateDiff(Day, 0, GETDATE()), 0)) between doct.beginvalidity And doct.endvalidity) " & ' plantilla exigible por su fecha de validez '"where (getdate() between doct.beginvalidity And doct.endvalidity) " &
                                    "And (doc.id Is null And doct.Compulsory = 1) " & 'plantilla obligatorias de las que no hay ningún doc entregado
                                    "And doct.scope IN (" & DocumentScope.EmployeeContract & "," & DocumentScope.EmployeeField & ") "

                If idRelatedObject <= 0 Then strQuery = strQuery & " order by idemp asc"

                Dim tbAux As DataTable = CreateDataTable(strQuery)

                If tbAux.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbAux.Rows
                        Dim oAlert As New DocumentAlert
                        oAlert.IDDocument = 0 ' Lo dejo informado por compatibilidad con versiones iniciales en las que no existía la propierada IDDocumentTemplate
                        oAlert.IDDocumentTemplate = oRow("templateid")
                        oAlert.DateTime = Now
                        oAlert.IdRelatedObject = If(IsDBNull(oRow("idemp")), idRelatedObject, oRow("idemp"))
                        oAlert.ObjectName = If(oAlert.IdRelatedObject > 0, ExecuteScalar("@SELECT# ISNULL(Name,'') FROM Employees WHERE ID=" & oAlert.IdRelatedObject), "")

                        'Mensaje para un empleado o empresa. No hace falta poner el nombre ....
                        Me.oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(Me.LoadDocumentTemplate(oAlert.IDDocumentTemplate).Name)
                        oAlert.Description = oState.Language.Translate("roDocumentManager.Document.Undelivered", "") & " (" & oAlert.ObjectName & ")"
                        oState.Language.ClearUserTokens()
                        oAlert.IsUrgent = True
                        oAlert.IsCritic = True

                        oMandatoryDocs.Add(oAlert)
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification:: GetDocumentationFaultAlerts_UndeliveredDocuments")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDocumentationFaultAlerts_UndeliveredDocuments")
            Finally

            End Try

            Return oMandatoryDocs.ToArray
        End Function

        '' <summary>
        '' Documentos que estan pendientes de ser firmados
        '' </summary>
        '' <param name="idRelatedObject">Identificador del objeto empresa/empleado</param>
        '' <param name="type">Tipo del objecto que consultamos</param>
        '' <param name="oActiveTransaction"></param>
        '' <returns></returns>
        '' <remarks></remarks>
        Public Function GetPortalAlerts_PendingSignDocumentsByEmployee(ByVal idRelatedObject As Integer, ByVal type As DocumentType) As DocumentAlert()

            Dim oMandatoryDocs As New Generic.List(Of DocumentAlert)
            Dim strQuery As String = String.Empty
            Dim strWhere1 As String = String.Empty
            Dim strWhere2 As String = String.Empty
            Dim idPassport As Integer = 1

            Try
                strQuery = "@SELECT# doc.ID as documentid, doct.id templateid, " & idRelatedObject.ToString & " idemp, doc.IdCompany idComp, NULL absenceid, doct.scope, NULL absencetype, NULL begindate, NULL enddate, NULL starthour, null endhour, null idcause " &
                            "From DocumentTemplates doct "
                strQuery = strQuery &
                                    "Left outer join (@SELECT# * from documents) doc on doc.IdDocumentTemplate = doct.id And doc.IDEmployee = " & idRelatedObject.ToString & " " &
                                    "where  doct.scope IN (" & DocumentScope.EmployeeContract & "," & DocumentScope.EmployeeField & ")  " &
                                    " And (isnull(doct.RequiresSigning,0) = 1 and doc.SignStatus =  " & SignStatusEnum.Pending & " ) "
                If idRelatedObject <= 0 Then strQuery = strQuery & " order by idemp asc"

                Dim tbAux As DataTable = CreateDataTable(strQuery)

                If tbAux.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbAux.Rows
                        Dim oAlert As New DocumentAlert
                        oAlert.IDDocument = 0 ' Lo dejo informado por compatibilidad con versiones iniciales en las que no existía la propierada IDDocumentTemplate
                        oAlert.IDDocumentTemplate = oRow("templateid")
                        oAlert.DateTime = Now

                        oAlert.IdRelatedObject = If(IsDBNull(oRow("idemp")), idRelatedObject, oRow("idemp"))
                        oAlert.ObjectName = If(oAlert.IdRelatedObject > 0, ExecuteScalar("@SELECT# ISNULL(Name,'') FROM Employees WHERE ID=" & oAlert.IdRelatedObject), "")

                        'Mensaje para un empleado o empresa. No hace falta poner el nombre ....
                        oAlert.Description = oState.Language.Translate("roDocumentManager.Document.Pendingsignature", "")
                        oAlert.IsUrgent = True
                        oAlert.IsCritic = True
                        oAlert.DocumentTemplateName = oState.Language.Translate("roDocumentManager.Document.DocumentsToSign", "") 'Documentos para firmar
                        oMandatoryDocs.Add(oAlert)
                        Exit For
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification:: GetPortalAlerts_PendingSignDocumentsByEmployee")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetPortalAlerts_PendingSignDocumentsByEmployee")
            Finally

            End Try

            Return oMandatoryDocs.ToArray
        End Function

        ''' <summary>
        ''' Documentos obligatorios que faltan
        ''' </summary>
        ''' <param name="idRelatedObject">Identificador del objeto empresa/empleado</param>
        ''' <param name="type">Tipo del objecto que consultamos</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetDocumentationFaultAlerts_UndeliveredDocuments(ByVal idRelatedObject As Integer, ByVal type As DocumentType, Optional dOnDate As DateTime = Nothing) As DocumentAlert()
            Dim oMandatoryDocs As New Generic.List(Of DocumentAlert)
            Dim strQuery As String = String.Empty
            Dim strWhere1 As String = String.Empty
            Dim strWhere2 As String = String.Empty
            Dim idPassport As Integer = 1

            Try
                Dim oServerLicense As New roServerLicense
                If Not (oServerLicense.FeatureIsInstalled("Feature\Documents") OrElse oServerLicense.FeatureIsInstalled("Feature\Absences")) Then Return oMandatoryDocs.ToArray

                If Me.oState.IDPassport > 0 Then idPassport = Me.oState.IDPassport

                ' Calculo días de offset para hacer validaciones de validez a futuro (para alertas con previsión)
                Dim dDaysOffset As Integer = 0
                If dOnDate.Year > 1 Then
                    dDaysOffset = dOnDate.Date.Subtract(Now.Date).Days
                End If

                ' DOCUMENTOS INCONDICIONALMENTE OBLIGATORIOS
                If idRelatedObject > 0 Then
                    ' Para un sólo empleado o empresa
                    Select Case type
                        Case DocumentType.Employee
                            strQuery = $"@SELECT# doct.id 
                                                templateid, 
                                                {idRelatedObject} idemp, 
                                                doc.IdCompany idComp, 
                                                NULL absenceid, 
                                                doct.scope, 
                                                NULL absencetype, 
                                                NULL begindate, 
                                                NULL enddate, 
                                                NULL starthour, 
                                                null endhour, 
                                                null idcause,
                                                doct.name templatename
                                                From DocumentTemplates doct "
                            strQuery = strQuery &
                                    "Left outer join (@SELECT# * from documents where Status not in (" & DocumentStatus.Rejected & "," & DocumentStatus.Invalidated & "," & DocumentStatus.Expired & ")) doc on doc.IdDocumentTemplate = doct.id And doc.IDEmployee = " & idRelatedObject.ToString & " " & 'plantillas con documentos entregados, contando un documento por plantilla
                                    "INNER JOIN sysrofnSecurity_PermissionsOverDocumentTemplates(0, " & idRelatedObject.ToString & ", -1, " & idPassport.ToString & ") PODT ON PODT.IdDocumentTemplate = doct.Id AND PODT.IdEmployee = " & idRelatedObject.ToString & " " &
                                    "where (DateAdd(Day, " & dDaysOffset & ", DateAdd(Day, DateDiff(Day, 0, GETDATE()), 0)) between doct.beginvalidity And doct.endvalidity) " & ' plantilla exigible por su fecha de validez '"where (getdate() between doct.beginvalidity And doct.endvalidity) " &
                                    "And (doc.id Is null And doct.Compulsory = 1) " & 'plantilla obligatorias de las que no hay ningún doc entregado
                                    "And doct.scope IN (" & DocumentScope.EmployeeContract & "," & DocumentScope.EmployeeField & ") "
                        Case DocumentType.Company
                            strQuery = "@SELECT# doct.id templateid, doc.IdEmployee idemp, " & idRelatedObject.ToString & " idComp, NULL absenceid, doct.scope, NULL absencetype, NULL begindate, NULL enddate, NULL starthour, null endhour, null idcause, doct.name templatename " &
                                           "From DocumentTemplates doct "
                            strQuery = strQuery &
                                "Left outer join (@SELECT# * from documents where Status not in (" & DocumentStatus.Rejected & "," & DocumentStatus.Invalidated & "," & DocumentStatus.Expired & ")) doc on doc.IdDocumentTemplate = doct.id And doc.IDCompany = " & idRelatedObject.ToString & 'plantillas con documentos entregados, contando un documento por plantilla
                                "INNER JOIN sysrofnSecurity_PermissionsOverDocumentTemplates(0, -1, " & idRelatedObject.ToString & ", " & idPassport.ToString & ") PODT ON PODT.IdDocumentTemplate = doct.Id AND PODT.IdCompany = " & idRelatedObject.ToString & " " &
                                "where (DateAdd(Day, " & dDaysOffset & ", DateAdd(Day, DateDiff(Day, 0, GETDATE()), 0)) between doct.beginvalidity And doct.endvalidity) " & ' plantilla exigible por su fecha de validez '"where (getdate() between doct.beginvalidity And doct.endvalidity) " &
                                "And (doc.id Is null And doct.Compulsory = 1) " & 'plantilla obligatorias de las que no hay ningún doc entregado
                                "And doct.scope = " & DocumentScope.Company
                    End Select
                Else
                    ' Para todos los empleados o empresas
                    Select Case type
                        Case DocumentType.Employee
                            strQuery = "@SELECT# doct.id templateid, PODT.Idemployee idemp, doc.IdCompany idComp, NULL absenceid, doct.scope, NULL absencetype, NULL begindate, NULL enddate, NULL starthour, null endhour, null idcause, doct.name templatename, sysrovwcurrentemployeegroups.EmployeeName  " &
                            "From DocumentTemplates doct " &
                            "INNER JOIN sysrofnSecurity_PermissionsOverDocumentTemplates(0, 0, -1, " & idPassport.ToString & ") PODT On PODT.IdDocumentTemplate = doct.Id " &
                            "INNER Join sysrovwcurrentemployeegroups ON sysrovwcurrentemployeegroups.IDEmployee = PODT.IdEmployee And  sysrovwcurrentemployeegroups.CurrentEmployee = 1 " &
                            "Left outer join (@SELECT# * from documents where Status not in (" & DocumentStatus.Rejected & "," & DocumentStatus.Invalidated & "," & DocumentStatus.Expired & ")) doc on doc.IdDocumentTemplate = doct.id And doc.IDEmployee = PODT.IDEmployee " & 'plantillas con documentos entregados, contando un documento por plantilla
                            "where (DateAdd(Day, " & dDaysOffset & ", DateAdd(Day, DateDiff(Day, 0, GETDATE()), 0)) between doct.beginvalidity And doct.endvalidity) " & ' plantilla exigible por su fecha de validez ' "where (getdate() between doct.beginvalidity And doct.endvalidity) " & ' plantilla exigible por su fecha de validez
                            "And (doc.id Is null And doct.Compulsory = 1) " & 'plantilla obligatorias de las que no hay ningún doc entregado
                            "And doct.scope IN (" & DocumentScope.EmployeeContract & "," & DocumentScope.EmployeeField & ") "
                        Case DocumentType.Company
                            strQuery = "@SELECT# doct.id templateid, doc.IdEmployee idemp, PODT.idcompany idComp, NULL absenceid, doct.scope, NULL absencetype, NULL begindate, NULL enddate, NULL starthour, null endhour, null idcause, doct.name templatename, Groups.Name GroupName  " &
                            "From DocumentTemplates doct " &
                            "INNER JOIN sysrofnSecurity_PermissionsOverDocumentTemplates(0, -1, 0, " & idPassport.ToString & ") PODT On PODT.IdDocumentTemplate = doct.Id " &
                            "INNER JOIN Groups ON Groups.Id = PODT.IdCompany AND Charindex('\', groups.path) = 0 " &
                            "LEFT OUTER JOIN (@SELECT# * from documents where Status not in (" & DocumentStatus.Rejected & "," & DocumentStatus.Invalidated & "," & DocumentStatus.Expired & ")) doc on doc.IdDocumentTemplate = doct.id AND doc.idcompany = PODT.idcompany " & 'plantillas con documentos entregados, contando un documento por plantilla
                            "where (DateAdd(Day, " & dDaysOffset & ", DateAdd(Day, DateDiff(Day, 0, GETDATE()), 0)) between doct.beginvalidity And doct.endvalidity) " & ' plantilla exigible por su fecha de validez '"where (getdate() between doct.beginvalidity And doct.endvalidity) " & ' plantilla exigible por su fecha de validez
                            "And (doc.id Is null And doct.Compulsory = 1) " & 'plantilla obligatorias de las que no hay ningún doc entregado
                            "And doct.scope = " & DocumentScope.Company
                    End Select
                End If

                If idRelatedObject <= 0 Then strQuery = strQuery & " order by idemp asc"

                Dim tbDocuments As DataTable = CreateDataTable(strQuery)

                If tbDocuments.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbDocuments.Rows
                        Dim oAlert As New DocumentAlert
                        oAlert.IDDocument = oRow("templateid") ' Lo dejo informado por compatibilidad con versiones iniciales en las que no existía la propierada IDDocumentTemplate
                        oAlert.IDDocumentTemplate = oRow("templateid")
                        oAlert.DateTime = Now

                        Select Case type
                            Case DocumentType.Employee
                                oAlert.IdRelatedObject = If(IsDBNull(oRow("idemp")), idRelatedObject, oRow("idemp"))
                                If tbDocuments.Columns.Contains("EmployeeName") Then
                                    oAlert.ObjectName = oRow("EmployeeName")
                                Else
                                    oAlert.ObjectName = If(oAlert.IdRelatedObject > 0, ExecuteScalar("@SELECT# ISNULL(Name,'') FROM Employees WHERE ID=" & oAlert.IdRelatedObject), "")
                                End If
                            Case DocumentType.Company
                                oAlert.IdRelatedObject = If(IsDBNull(oRow("idComp")), idRelatedObject, oRow("idComp"))
                                If tbDocuments.Columns.Contains("GroupName") Then
                                    oAlert.ObjectName = oRow("GroupName")
                                Else
                                    oAlert.ObjectName = If(oAlert.IdRelatedObject > 0, ExecuteScalar("@SELECT# ISNULL(Name,'') FROM Groups WHERE ID=" & oAlert.IdRelatedObject), "")
                                End If
                        End Select

                        'Mensaje para un empleado o empresa. No hace falta poner el nombre ....
                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(oRow("templatename")) 'oState.Language.AddUserToken(Me.LoadDocumentTemplate(oAlert.IDDocument).Name)
                        oAlert.Description = oState.Language.Translate("roDocumentManager.Document.Undelivered", "") & " (" & oAlert.ObjectName & ")"
                        oState.Language.ClearUserTokens()
                        oAlert.IsUrgent = True
                        oAlert.IsCritic = True

                        oMandatoryDocs.Add(oAlert)
                    Next
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roNotification:: GetDocumentationFaultAlerts_UndeliveredDocuments")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roNotification::GetDocumentationFaultAlerts_UndeliveredDocuments")
            End Try

            Return oMandatoryDocs.ToArray
        End Function

        ''' <summary>
        ''' Devuelve colección de alertas de documentos de bajas (ausencias de días por justificaciones que no requieren ser solicitadas. Contiene documentos que, por las justificaciones de las ausencias del empleado, deben ser presentados y, o no han sido presentados, o han sido presentados y no están en estado válido, o son válidos peros caducarán en una fecha (si se informa la fecha de evaluación)
        ''' </summary>
        ''' <param name="idRelatedObject">Identificador de empleado. Para retornar todos los empleados indicar valor 0</param>
        ''' <param name="type">Debe indicarse empleado</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="idForecast">Identificador de ausencia.Si se informa, debería indicarse también el tipo de ausencias</param>
        ''' <param name="forecastType">Filtra por tipo de absentismo: DAYS-> ausencias por días, HOURS-> ausencias por horas, otro valor o nada-> todas las ausencias</param>
        ''' <param name="bCheckPermissions">Indica si se deben validar persmisos sobre empleados, areas de documentos y LOPD de documentos</param>
        ''' <param name="dOnDate">Fecha en que se evalua la validez de documentos. Si no se informa se usa el día actual</param>
        ''' <returns></returns>
        Public Function GetForecastDocumentationFaultAlerts(ByVal idRelatedObject As Integer, ByVal type As DocumentType, Optional ByVal idForecast As Integer = 0, Optional ByVal forecastType As DTOs.ForecastType = DTOs.ForecastType.AnyAbsence, Optional bCheckPermissions As Boolean = True, Optional dOnDate As DateTime = Nothing, Optional iLastNMonths As Integer = -1, Optional checkStatusLevel As Boolean = False) As DocumentAlert()

            Dim oForecastDocs As New Generic.List(Of DocumentAlert)
            Dim strQueryDayAbsences As String = String.Empty
            Dim strQueryHourAbsences As String = String.Empty
            Dim strQueryOverWork As String = String.Empty
            Dim strQuery As String = String.Empty
            Dim strSQL As String = String.Empty
            Dim strWhere1 As String = String.Empty
            Dim strWhere2 As String = String.Empty
            Dim idPassport As Integer = 1
            Dim strTimeLimitWhere As String = String.Empty
            Dim iHasPermission As Boolean = True

            Try
                Dim oServerLicense As New roServerLicense
                If Not (oServerLicense.FeatureIsInstalled("Feature\Absences")) Then Return oForecastDocs.ToArray

                If Me.oState.IDPassport > 0 Then idPassport = Me.oState.IDPassport

                ' Calculo días de offset para hacer validaciones de validez a futuro (para alertas con previsión)
                Dim dDaysOffset As Integer = 0
                If dOnDate.Year > 1 Then
                    dDaysOffset = dOnDate.Date.Subtract(Now.Date).Days
                End If

                If bCheckPermissions Then
                    iHasPermission = roTypes.Any2Boolean(AccessHelper.ExecuteScalar(" @SELECT# 1 from sysrovwSecurity_PermissionOverFeatures pof  WITH (NOLOCK)  where pof.FeatureAlias = 'Documents' and pof.FeatureType = 'U' and pof.Permission >= 3 and pof.IDPassport = " & idPassport))
                End If

                If Not bCheckPermissions OrElse (bCheckPermissions AndAlso iHasPermission) Then

                    ' Si no se filtra por empleado, limitamos a 6 meses
                    If idRelatedObject <= 0 OrElse iLastNMonths > -1 Then
                        If iLastNMonths = -1 Then iLastNMonths = -6
                        strTimeLimitWhere = roTypes.Any2Time(Now.AddMonths(iLastNMonths)).SQLSmallDateTime
                    End If

                    If type = DocumentType.Employee Then

                        'DOCUMENTOS DE PREVISIONES DE HORAS DE EXCESO
                        strQueryOverWork = "@SELECT# case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,po.EndDate) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,po.BeginDate) END DueDate," &
                        "isnull(doc.id,docpending.id) docid, " & ' "doc.id docid, "
                        "docsabs.documenttemplateid templateid,  " &
                        "po.IDEmployee idemp,  " &
                        "Null idcomp,  " &
                        "po.id as forecastId,  " &
                        "docsabs.scope,  " &
                        "'overtime' forecasttype, " &
                        "po.begindate begindate, " &
                        "po.EndDate enddate,  " &
                        "po.begintime starthour,  " &
                        "po.endtime endhour,  " &
                        "po.idcause idcause, " &
                        "docsabs.Area, " &
                        "docpending.Status, " &
                        "docpending.StatusLevel " &
                        "from ProgrammedOvertimes po WITH (NOLOCK)  " &
                        "inner join employeecontracts ec WITH (NOLOCK) on ec.idemployee = po.idemployee and po.BeginDate between ec.BeginDate and ec.EndDate "

                        If idRelatedObject > 0 Then
                            ' Para un empleado dado
                            strQueryOverWork = strQueryOverWork & " and po.idemployee = " & idRelatedObject.ToString
                        End If

                        If idForecast > 0 Then
                            strQueryOverWork = strQueryOverWork & " and po.ID = " & idForecast.ToString
                        End If

                        strQueryOverWork = strQueryOverWork & " Left join (@SELECT# doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, doct.Compulsory, adt.IDLabAgree, doct.ApprovalLevelRequired from DocumentTemplates doct WITH (NOLOCK) inner join CausesDocumentsTracking adt WITH (NOLOCK) on adt.iddocument  = doct.id where (doct.scope = " & DocumentScope.CauseNote & " )) docsabs on docsabs.idcause = po.IDCause  " &
                        "and (CONVERT(smalldatetime,docsabs.BeginValidity, 120) = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (CONVERT(smalldatetime,docsabs.BeginValidity,120) <= po.BeginDate))   " &
                        " @@LOPD_CATEGORY@@ @@AREA@@ " &
                        "Left  join (@SELECT# row_number() over (partition by IdEmployee, IdDocumentTemplate, IdOverTimeForecast order by IdDocumentTemplate desc) As 'RowNumber1', documents.id, documents.IdDocumentTemplate, documents.idovertimeforecast, documents.IdRequest, documents.Status, documents.StatusLevel from documents WITH (NOLOCK) where Status = " & DocumentStatus.Pending & " OR Status = " & DocumentStatus.Validated & " ) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid and docpending.IdOverTimeForecast = po.ID and ((docpending.Status = " & DocumentStatus.Pending & " or (docpending.Status= " & DocumentStatus.Validated & " and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 ) " &
                        "Left join (@SELECT# documents.id, documents.iddocumenttemplate, documents.idemployee, documents.idovertimeforecast, documents.IdRequest, documents.status, documents.StatusLevel, documents.BeginDate, documents.EndDate " &
                        "                    from documents WITH (NOLOCK) " &
                        "                    INNER JOIN DocumentTemplates WITH (NOLOCK) ON DocumentTemplates.Id = Documents.IdDocumentTemplate WHERE DocumentTemplates.Scope in (" & DocumentScope.LeaveOrPermission & "," & DocumentScope.CauseNote & ") ) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid " &
                        "                               and doc.IdEmployee = po.IDEmployee   " &
                        "                               and doc.IdOvertimeForecast = po.id  " &
                        "                               and ((doc.Status = " & DocumentStatus.Validated & " and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired <> 0 ) OR docsabs.ApprovalLevelRequired = 0)) OR doc.Status = " & DocumentStatus.Expired & ")" & ' Documentos correctos
                        "                               and ((doc.EndDate >= DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= po.EndDate)  " & '--Estado correcto o expirado, pero caduca, siempre que sea dentro de la ausencia
                        "                               and (doc.BeginDate <= DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  " & '-- Estado correcto pero todavía no es vigente
                        "where doc.id IS NULL " &
                        "and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) " &
                        "and ((docpending.RowNumber1 is NULL and docsabs.Compulsory = 1) or docpending.RowNumber1 =1 )  " & '<-- "and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1) " &
                        "and (((docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 ) OR ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,po.BeginDate, DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,po.BeginDate,po.EndDate) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,po.EndDate, DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend)))) "

                        If strTimeLimitWhere <> String.Empty Then
                            strQueryOverWork = strQueryOverWork & " AND case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,po.EndDate) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,po.BeginDate) END >= " & strTimeLimitWhere
                        End If

                        ' DOCUMENTOS PREVISIONES DE AUSENCIAS POR DIAS (BAJAS Y AUSENCIAS)
                        strQueryDayAbsences = "@SELECT# case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate))) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pa.BeginDate) END DueDate," &
                        "isnull(doc.id,docpending.id) docid, " & ' "doc.id docid, "
                        "docsabs.documenttemplateid templateid,  " &
                        "pa.IDEmployee idemp,  " &
                        "Null idcomp,  " &
                        "CASE pa.IsRequest WHEN 0 THEN pa.absenceid ELSE pa.requestid END as forecastId, " &
                        "docsabs.scope,  " &
                        "CASE pa.IsRequest WHEN 1 THEN 'requestdays' ELSE 'days' END as forecasttype, " &
                        "pa.begindate begindate, " &
                        "isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate)) enddate,  " &
                        "NULL starthour,  " &
                        "null endhour,  " &
                        "pa.idcause idcause, " &
                        "docsabs.Area, " &
                        "docpending.Status, " &
                        "docpending.StatusLevel " &
                        "from sysrovwDaysAbsences pa WITH (NOLOCK) " &
                        "inner join employeecontracts ec WITH (NOLOCK) on ec.idemployee = pa.idemployee and pa.BeginDate between ec.BeginDate and ec.EndDate "

                        If idRelatedObject > 0 Then
                            ' Para un empleado dado
                            strQueryDayAbsences = strQueryDayAbsences & " and pa.idemployee = " & idRelatedObject.ToString
                        End If

                        If idForecast > 0 Then
                            'strQueryDayAbsences = strQueryDayAbsences & " and pa.absenceid = " & idForecast.ToString
                            strQueryDayAbsences = strQueryDayAbsences & " and CASE pa.isrequest WHEN 0 THEN pa.absenceid ELSE pa.requestid END = " & idForecast.ToString
                        End If

                        strQueryDayAbsences = strQueryDayAbsences & " Left join (@SELECT# doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, doct.Compulsory, adt.IDLabAgree, doct.ApprovalLevelRequired from DocumentTemplates doct WITH (NOLOCK) inner join CausesDocumentsTracking adt WITH (NOLOCK) on adt.iddocument  = doct.id where ((doct.Scope = " & DocumentScope.LeaveOrPermission & " or doct.scope = " & DocumentScope.CauseNote & "))) docsabs on docsabs.idcause = pa.IDCause " &
                        "And (CONVERT(smalldatetime,docsabs.BeginValidity, 120) = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (CONVERT(smalldatetime,docsabs.BeginValidity,120) <= pa.BeginDate))   " &
                        " @@LOPD_CATEGORY@@ @@AREA@@ " &
                        "Left  join (@SELECT# row_number() over (partition by IdEmployee, IdDocumentTemplate, IdDaysAbsence, IdRequest order by IdDocumentTemplate desc) As 'RowNumber1', documents.id, documents.idemployee, documents.IdDocumentTemplate, documents.IdDaysAbsence, documents.IdRequest, documents.Status, documents.StatusLevel from documents WITH (NOLOCK) where Status = " & DocumentStatus.Pending & " OR Status = " & DocumentStatus.Validated & " ) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid AND ec.IDEmployee = docpending.IdEmployee and (docpending.IdDaysAbsence = pa.AbsenceID OR docpending.IdRequest = pa.RequestID) and ((docpending.Status = " & DocumentStatus.Pending & " or (docpending.Status= " & DocumentStatus.Validated & " and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 ) " &
                        "Left join (@SELECT# documents.id, documents.iddocumenttemplate, documents.idemployee, documents.IdDaysAbsence, documents.IdRequest, documents.status, documents.StatusLevel, documents.BeginDate, documents.EndDate from documents WITH (NOLOCK)  INNER JOIN DocumentTemplates WITH (NOLOCK) ON DocumentTemplates.Id = Documents.IdDocumentTemplate WHERE DocumentTemplates.Scope in (" & DocumentScope.LeaveOrPermission & "," & DocumentScope.CauseNote & ")) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid " &
                        "       and doc.IdEmployee = pa.IDEmployee   " &
                        "       and (doc.IdDaysAbsence = pa.absenceid OR doc.IdRequest = pa.RequestID)  " &
                        "       and ((doc.Status = " & DocumentStatus.Validated & " and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired <> 0 ) OR docsabs.ApprovalLevelRequired = 0)) OR doc.Status = " & DocumentStatus.Expired & "  ) " & ' Documentos correctos
                        "       and ((doc.EndDate >= DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate)))  " & '--Estado correcto o expirado, pero caduca, siempre que sea dentro de la ausencia
                        "       and (doc.BeginDate <= DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  " & '-- Estado correcto pero todavía no es vigente
                        "where doc.id IS NULL " &
                        "and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) " &
                        "and ((docpending.RowNumber1 is NULL and docsabs.Compulsory = 1) or docpending.RowNumber1 =1 )  " & '<-- "and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1) " &
                        "and (((docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 ) OR ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,pa.BeginDate, DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) " &
                        "And (DATEDIFF(day,pa.BeginDate,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate))) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL) " &  'Sólo verifico esto para documentos que se requieren cada X días
                        ") " &
                        "Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate)), DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend))))  "

                        If strTimeLimitWhere <> String.Empty Then
                            strQueryDayAbsences = strQueryDayAbsences & " AND case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate))) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pa.BeginDate) END >= " & strTimeLimitWhere
                        End If

                        ' DOCUMENTOS DE PREVISIONES DE AUSENCIAS POR HORAS
                        strQueryHourAbsences = "@SELECT# case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,pc.FinishDate) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pc.Date) END DueDate," &
                        "isnull(doc.id,docpending.id) docid, " & ' "doc.id docid, "
                        "docsabs.documenttemplateid templateid,  " &
                        "pc.IDEmployee idemp,  " &
                        "Null idcomp,  " &
                        "CASE pc.IsRequest WHEN 0 THEN pc.absenceid ELSE pc.requestid END as forecastId," &
                        "docsabs.scope,  " &
                        "CASE pc.IsRequest WHEN 1 THEN 'requesthours' ELSE 'hours' END AS forecasttype, " &
                        "pc.date begindate, " &
                        "pc.FinishDate enddate,  " &
                        "pc.begintime starthour,  " &
                        "pc.endtime endhour,  " &
                        "pc.idcause idcause, " &
                        "docsabs.Area, " &
                        "docpending.Status, " &
                        "docpending.StatusLevel " &
                        "from sysrovwHoursAbsences pc WITH (NOLOCK) " &
                        "inner join employeecontracts ec WITH (NOLOCK) on ec.idemployee = pc.idemployee and pc.Date between ec.BeginDate and ec.EndDate "

                        If idRelatedObject > 0 Then
                            ' Para un empleado dado
                            strQueryHourAbsences = strQueryHourAbsences & " and pc.idemployee = " & idRelatedObject.ToString
                        End If

                        If idForecast > 0 Then
                            'strQueryHourAbsences = strQueryHourAbsences & " and pc.absenceid = " & idForecast.ToString
                            strQueryHourAbsences = strQueryHourAbsences & " and CASE pc.IsRequest WHEN 0 THEN pc.absenceid ELSE pc.requestid END = " & idForecast.ToString
                        End If

                        strQueryHourAbsences = strQueryHourAbsences & " Left join (@SELECT# doct.Id documenttemplateid, doct.BeginValidity, adt.idcause, adt.every, doct.LOPDAccessLevel, doct.Area, adt.daysafterabsencebegin, adt.daysafterabsenceend, doct.scope, doct.Compulsory, adt.IDLabAgree, doct.ApprovalLevelRequired from DocumentTemplates doct WITH (NOLOCK) inner join CausesDocumentsTracking adt WITH (NOLOCK) on adt.iddocument  = doct.id where ((doct.Scope = " & DocumentScope.LeaveOrPermission & " or doct.scope = " & DocumentScope.CauseNote & "))) docsabs on docsabs.idcause = pc.IDCause " &
                        "and (CONVERT(smalldatetime,docsabs.BeginValidity, 120) = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (CONVERT(smalldatetime,docsabs.BeginValidity,120) <= pc.Date))   " &
                        " @@LOPD_CATEGORY@@ @@AREA@@ " &
                        "left join (@SELECT# row_number() over (partition by IdEmployee, IdDocumentTemplate, IdHoursAbsence, IdRequest order by IdDocumentTemplate desc) As 'RowNumber1', documents.id, documents.idemployee, documents.IdDocumentTemplate, documents.idhoursabsence, documents.IdRequest, documents.Status, documents.StatusLevel from documents WITH (NOLOCK) where Status = " & DocumentStatus.Pending & " OR Status = " & DocumentStatus.Validated & " ) docpending on docpending.IdDocumentTemplate = docsabs.documenttemplateid AND ec.IDEmployee = docpending.IdEmployee AND (docpending.IdHoursAbsence = pc.AbsenceID OR docpending.IdRequest = pc.RequestID) and ((docpending.Status = " & DocumentStatus.Pending & " or (docpending.Status= " & DocumentStatus.Validated & " and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 ) " &
                        "Left join (@SELECT# documents.id, documents.iddocumenttemplate, documents.idemployee, documents.idhoursabsence, documents.IdRequest, documents.status, documents.StatusLevel, documents.BeginDate, documents.EndDate from documents  WITH (NOLOCK) INNER JOIN DocumentTemplates WITH (NOLOCK) ON DocumentTemplates.Id = Documents.IdDocumentTemplate WHERE DocumentTemplates.Scope in (" & DocumentScope.LeaveOrPermission & "," & DocumentScope.CauseNote & ")) doc on doc.IdDocumentTemplate = docsabs.documenttemplateid " &
                        "       and doc.IdEmployee = pc.IDEmployee   " &
                        "       and (doc.IdHoursAbsence = pc.absenceid OR doc.IdRequest = pc.RequestID)  " &
                        "       and ((doc.Status = " & DocumentStatus.Validated & " and ((doc.StatusLevel <= docsabs.ApprovalLevelRequired and docsabs.ApprovalLevelRequired <> 0 ) OR docsabs.ApprovalLevelRequired = 0)) OR doc.Status = " & DocumentStatus.Expired & ")" & ' Documentos correctos
                        "       and ((doc.EndDate >= DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) or doc.EndDate >= pc.FinishDate)  " & '--Estado correcto o expirado, pero caduca, siempre que sea dentro de la ausencia
                        "       and (doc.BeginDate <= DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)))  " & '-- Estado correcto pero todavía no es vigente
                        "where doc.id IS NULL " &
                        "and (docsabs.IDLabAgree = 0 or docsabs.IDLabAgree = ec.IDLabAgree) " &
                        "and ((docpending.RowNumber1 is NULL and docsabs.Compulsory = 1) or docpending.RowNumber1 =1 )  " & '<-- "and (docpending.RowNumber1 is NULL or docpending.RowNumber1 =1) " &
                        "and (((docpending.Status = 0 or (docpending.Status= 1 and docpending.StatusLevel > docsabs.ApprovalLevelRequired)) and docsabs.ApprovalLevelRequired > 0 ) OR ((docsabs.daysafterabsencebegin Is Not null And (DATEDIFF(day,pc.Date, DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsencebegin) And (DATEDIFF(day,pc.Date,pc.FinishDate) >= docsabs.daysafterabsencebegin OR docsabs.every IS NULL)) Or  (docsabs.daysafterabsenceend Is Not null And (DATEDIFF(day,pc.FinishDate, DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))) >= docsabs.daysafterabsenceend))))  "

                        If strTimeLimitWhere <> String.Empty Then
                            strQueryHourAbsences = strQueryHourAbsences & " AND case WHEN docsabs.daysafterabsencebegin IS NULL  THEN DATEADD(day, docsabs.daysafterabsenceend,pc.FinishDate) ELSE DATEADD(DAY,docsabs.daysafterabsencebegin ,pc.Date) END >= " & strTimeLimitWhere
                        End If

                        ' TODO: El filtro de LOPD y AREA, subirlos al JOIN para obtener docspending
                        ' Calculamos permisos sobre campos de la ficha en función del nivel de accesos
                        Dim aPermOverLOPD As List(Of Integer) = New List(Of Integer)
                        aPermOverLOPD = GetAllowedDocumentLOPDAccessLevels(idPassport)
                        If aPermOverLOPD.Count > 0 AndAlso bCheckPermissions Then
                            strWhere1 = " And LOPDAccessLevel In (" & String.Join(",", aPermOverLOPD) & ")"
                        End If

                        ' Calculamos permisos sobre tipos de documentos. Deben tener lectura o superior
                        Dim aPermOverDocAreas As List(Of Integer) = New List(Of Integer)
                        aPermOverDocAreas = GetAllowedDocumentAreas(idPassport)
                        If aPermOverDocAreas.Count > 0 AndAlso bCheckPermissions Then
                            strWhere2 = " And (Area In (" & String.Join(",", aPermOverDocAreas) & "))"
                        End If

                        If strWhere1.Trim.Length > 0 Then
                            If strQueryDayAbsences.Trim.Length > 0 Then strQueryDayAbsences = strQueryDayAbsences.Replace("@@LOPD_CATEGORY@@", strWhere1)
                            If strQueryHourAbsences.Trim.Length > 0 Then strQueryHourAbsences = strQueryHourAbsences.Replace("@@LOPD_CATEGORY@@", strWhere1)
                            If strQueryOverWork.Trim.Length > 0 Then strQueryOverWork = strQueryOverWork.Replace("@@LOPD_CATEGORY@@", strWhere1)
                        Else
                            If strQueryDayAbsences.Trim.Length > 0 Then strQueryDayAbsences = strQueryDayAbsences.Replace("@@LOPD_CATEGORY@@", "")
                            If strQueryHourAbsences.Trim.Length > 0 Then strQueryHourAbsences = strQueryHourAbsences.Replace("@@LOPD_CATEGORY@@", "")
                            If strQueryOverWork.Trim.Length > 0 Then strQueryOverWork = strQueryOverWork.Replace("@@LOPD_CATEGORY@@", "")
                        End If

                        If strWhere2.Trim.Length > 0 Then
                            If strQueryDayAbsences.Trim.Length > 0 Then strQueryDayAbsences = strQueryDayAbsences.Replace("@@AREA@@", strWhere2)
                            If strQueryHourAbsences.Trim.Length > 0 Then strQueryHourAbsences = strQueryHourAbsences.Replace("@@AREA@@", strWhere2)
                            If strQueryOverWork.Trim.Length > 0 Then strQueryOverWork = strQueryOverWork.Replace("@@AREA@@", strWhere2)
                        Else
                            If strQueryDayAbsences.Trim.Length > 0 Then strQueryDayAbsences = strQueryDayAbsences.Replace("@@AREA@@", "")
                            If strQueryHourAbsences.Trim.Length > 0 Then strQueryHourAbsences = strQueryHourAbsences.Replace("@@AREA@@", "")
                            If strQueryOverWork.Trim.Length > 0 Then strQueryOverWork = strQueryOverWork.Replace("@@AREA@@", "")
                        End If

                        Select Case forecastType
                            Case ForecastType.AbsenceDays 'Ausencias de días, incluyendo bajas
                                strQuery = strQueryDayAbsences
                            Case ForecastType.AbsenceHours 'Ausencias por horas
                                strQuery = strQueryHourAbsences
                            Case ForecastType.OverWork 'Horas de exceso
                                strQuery = strQueryOverWork
                            Case ForecastType.Leave 'Bajas
                                strQuery = strQueryDayAbsences & " AND Scope = " & DTOs.DocumentScope.LeaveOrPermission
                            Case ForecastType.AnyAbsence
                                strQuery = strQueryDayAbsences & " UNION " & strQueryHourAbsences
                            Case ForecastType.Any
                                strQuery = strQueryDayAbsences & " UNION " & strQueryHourAbsences & " UNION " & strQueryOverWork
                        End Select

                        strSQL = strQuery
                        'Si pedimos permisos estamos solicitando alertas desde Live sobre los usuarios que supervisas asi que debemos ver si tienen contrato activo o no, si no venimos desde el Portal mirando nuestras propias alertas
                        If bCheckPermissions Then
                            strSQL = " @SELECT# tempPerm.*,
                                        CASE 
											WHEN ce.IDEmployee IS NOT NULL THEN 1
											ELSE 0
										END AS ContractStatus FROM (" & strQuery & ") AS tempPerm " &
                                "INNER JOIN sysrovwSecurity_PermissionOverEmployees poe on poe.IDPassport = " & idPassport.ToString & " And poe.IDEmployee = tempPerm.idemp AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate  " &
                                " LEFT JOIN sysrosubvwCurrentEmployeePeriod ce ON ce.IDEmployee = tempPerm.idemp"
                        Else
                            strSQL = " @SELECT# tempPerm.*,
                                       1 AS ContractStatus FROM (" & strQuery & ") AS tempPerm"
                        End If
                    End If

                    If DataLayer.AccessHelper.GetDatabaseCompatibilityLevel = 120 Then
                        strSQL = strSQL & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetForecastDocumentationFaultAlerts)
                    End If

                    Dim tbAux As DataTable = CreateDataTable(strSQL)

                    Dim levelAuthoritySup As Integer

                    If tbAux.Rows.Count > 0 Then
                        For Each oRow As DataRow In tbAux.Rows
                            Dim oAlert As New DocumentAlert
                            Dim sDocName As String = String.Empty
                            Dim sAlertType As String = String.Empty

                            Dim bSkipAlert As Boolean = False

                            If Not IsDBNull(oRow("docid")) Then oAlert.IDDocument = oRow("docid")
                            oAlert.IDDocumentTemplate = oRow("templateid")
                            oAlert.IdRelatedObject = If(IsDBNull(oRow("idemp")), idRelatedObject, oRow("idemp"))
                            oAlert.ObjectName = If(oAlert.IdRelatedObject > 0, ExecuteScalar("@SELECT# ISNULL(Name,'') FROM Employees WHERE ID=" & oAlert.IdRelatedObject), "")
                            oAlert.DateTime = oRow("DueDate")
                            oAlert.Scope = oRow("scope")
                            If oAlert.IDDocument > 0 Then
                                sAlertType = "ValidationPending"
                                Try
                                    If checkStatusLevel AndAlso Not IsDBNull(oRow("statuslevel")) AndAlso Not IsDBNull(oRow("Area")) AndAlso Not IsDBNull(oRow("Status")) AndAlso roTypes.Any2Integer(oRow("Status")) = 1 Then
                                        ' Sólo para pantalla de alertas, no muestro alertas de documentos en estado pendiente de validar si tienen un nivel de validadión igual o superior al mío (dado que no podré hacer nada con ellos)
                                        ' Esto sólo para V3
                                        ' El nivel
                                        levelAuthoritySup = GetPassportLevelOfAuthority(idPassport, oRow("Area"))
                                        bSkipAlert = (levelAuthoritySup >= roTypes.Any2Integer(oRow("statuslevel")))
                                    End If
                                Catch ex As Exception
                                End Try
                            Else
                                sAlertType = "Undelivered"
                            End If

                            If Not bSkipAlert AndAlso roTypes.Any2Boolean(oRow("ContractStatus")) Then
                                sDocName = Me.LoadDocumentTemplate(oAlert.IDDocumentTemplate).Name
                                oAlert.DocumentTemplateName = sDocName
                                oAlert.IdCause = oRow("idcause")

                                'Mensaje para un empleado. No hace falta poner el nombre ....
                                oState.Language.ClearUserTokens()
                                oState.Language.AddUserToken(sDocName)

                                'En función del ámbito documento
                                oState.Language.AddUserToken(ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oRow("idcause")))
                                If oRow("forecasttype") = "days" Then
                                    oState.Language.AddUserToken(oRow("begindate"))
                                    oState.Language.AddUserToken(oRow("enddate"))

                                    oAlert.Description = oState.Language.Translate("roDocumentManager.Document." & sAlertType & ".DaysAbsence", "") & " (" & oAlert.ObjectName & ")"
                                    oAlert.IdDaysAbsence = oRow("forecastId")
                                ElseIf oRow("forecasttype") = "requestdays" Then
                                    oState.Language.AddUserToken(oRow("begindate"))
                                    oState.Language.AddUserToken(oRow("enddate"))

                                    oAlert.Description = oState.Language.Translate("roDocumentManager.Document." & sAlertType & ".RequestDaysAbsence", "") & " (" & oAlert.ObjectName & ")"
                                    oAlert.IdRequest = oRow("forecastId")
                                ElseIf oRow("forecasttype") = "hours" Then
                                    oState.Language.AddUserToken(oRow("begindate"))
                                    oState.Language.AddUserToken(oRow("enddate"))
                                    oState.Language.AddUserToken(oRow("starthour"))
                                    oState.Language.AddUserToken(oRow("endhour"))

                                    If roTypes.Any2DateTime(oRow("begindate")) <> roTypes.Any2DateTime(oRow("enddate")) Then
                                        oAlert.Description = oState.Language.Translate("roDocumentManager.Document." & sAlertType & ".HoursAbsence.Period", "") & " (" & oAlert.ObjectName & ")"
                                    Else
                                        oAlert.Description = oState.Language.Translate("roDocumentManager.Document." & sAlertType & ".HoursAbsence", "") & " (" & oAlert.ObjectName & ")"
                                    End If
                                    oAlert.IdHoursAbsence = oRow("forecastId")
                                ElseIf oRow("forecasttype") = "requesthours" Then
                                    oState.Language.AddUserToken(oRow("begindate"))
                                    oState.Language.AddUserToken(oRow("enddate"))
                                    oState.Language.AddUserToken(oRow("starthour"))
                                    oState.Language.AddUserToken(oRow("endhour"))

                                    If roTypes.Any2DateTime(oRow("begindate")) <> roTypes.Any2DateTime(oRow("enddate")) Then
                                        oAlert.Description = oState.Language.Translate("roDocumentManager.Document." & sAlertType & ".RequestHoursAbsence.Period", "") & " (" & oAlert.ObjectName & ")"
                                    Else
                                        oAlert.Description = oState.Language.Translate("roDocumentManager.Document." & sAlertType & ".RequestHoursAbsence", "") & " (" & oAlert.ObjectName & ")"
                                    End If
                                    oAlert.IdRequest = oRow("forecastId")
                                ElseIf oRow("forecasttype") = "overtime" Then
                                    oState.Language.AddUserToken(oRow("begindate"))
                                    oState.Language.AddUserToken(oRow("enddate"))
                                    oState.Language.AddUserToken(oRow("starthour"))
                                    oState.Language.AddUserToken(oRow("endhour"))

                                    If roTypes.Any2DateTime(oRow("begindate")) <> roTypes.Any2DateTime(oRow("enddate")) Then
                                        oAlert.Description = oState.Language.Translate("roDocumentManager.Document." & sAlertType & ".OvertimeForecast.Period", "") & " (" & oAlert.ObjectName & ")"
                                    Else
                                        oAlert.Description = oState.Language.Translate("roDocumentManager.Document." & sAlertType & ".OvertimeForecast", "") & " (" & oAlert.ObjectName & ")"
                                    End If
                                    oAlert.IdOvertimeForecast = oRow("forecastId")
                                End If

                                oState.Language.ClearUserTokens()
                                oAlert.IsUrgent = True
                                oAlert.IsCritic = True

                                oForecastDocs.Add(oAlert)
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roNotification:: GetLeaveDocumentationFaultAlerts")
            Finally

            End Try

            Return oForecastDocs.ToArray
        End Function

        Public Function GetAccessAuthorizationDocumentationFaultAlerts(ByVal idRelatedObject As Integer, ByVal type As DocumentType, Optional ByVal idAuthorization As Integer = 0, Optional bCheckPermissions As Boolean = True, Optional dOnDate As DateTime = Nothing, Optional checkStatusLevel As Boolean = False) As DocumentAlert()

            Dim oPRLDocsAlerts As New Generic.List(Of DocumentAlert)

            Dim strQuery As String = String.Empty
            Dim strEmployeeQuery As String = String.Empty
            Dim strCompanyQuery As String = String.Empty
            Dim strWhere1 As String = String.Empty
            Dim strWhere2 As String = String.Empty
            Dim idPassport As Integer = 1

            Try
                Dim oServerLicense As New roServerLicense
                If Not (oServerLicense.FeatureIsInstalled("Feature\OHP")) Then Return oPRLDocsAlerts.ToArray

                If Me.oState.IDPassport > 0 Then idPassport = Me.oState.IDPassport

                ' Calculo días de offset para hacer validaciones de validez a futuro (para alertas con previsión)
                Dim dDaysOffset As Integer = 0
                If dOnDate.Year > 1 Then
                    dDaysOffset = dOnDate.Date.Subtract(Now.Date).Days
                End If

                Select Case type
                    Case DocumentType.Employee
                        'DOCUMENTOS DE PRL EXIGIBLES DIRECTAMENTE A EMPLEADOS
                        strEmployeeQuery = "@SELECT# docsauth.BeginValidity DueDate, " &
                        "isnull(doc.id, docpending.id) docid,  " &
                        "docsauth.documenttemplateid templateid,  " &
                        "svaa.IDEmployee idemp, " &
                        "Null idcomp,   " &
                        "svaa.IDAuthorization As authorizationId, " &
                        "docsauth.scope, " &
                        "docsauth.AccessValidation, " &
                        "docsauth.area, " &
                        "docpending.Status, "
                        If bCheckPermissions Then
                            strEmployeeQuery = strEmployeeQuery & "PODT.SupervisorLevelOfAuthority, "
                        End If
                        strEmployeeQuery = strEmployeeQuery & "docpending.StatusLevel " &
                        "From sysrovwEmployeeAccessAuthorizations svaa " &
                        "left join employeecontracts ec on ec.idemployee = svaa.idemployee and DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) between ec.BeginDate and ec.EndDate " &
                        "inner join (@SELECT# doct.Id documenttemplateid, doct.BeginValidity, doct.EndValidity, aadt.IDAuthorization, doct.LOPDAccessLevel, doct.Area, doct.scope, doct.AccessValidation, doct.ApprovalLevelRequired from DocumentTemplates doct inner join AccessAuthorizationDocumentsTracking aadt on aadt.IDDocument  = doct.id where doct.Scope = " & DocumentScope.EmployeeAccessAuthorization & " and doct.compulsory = 1) docsauth on docsauth.IDAuthorization = svaa.IDAuthorization  " &
                        "Left join (@SELECT# row_number() over (partition by IdEmployee, IdDocumentTemplate order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = " & DocumentStatus.Pending & " OR Status = " & DocumentStatus.Validated & " ) docpending on docpending.IdDocumentTemplate = docsauth.documenttemplateid and docpending.IdEmployee = svaa.IDEmployee  and (docpending.Status = " & DocumentStatus.Pending & " or (docpending.Status= " & DocumentStatus.Validated & " and docpending.StatusLevel > docsauth.ApprovalLevelRequired))" &
                        "Left join (@SELECT# * from documents) doc on doc.IdDocumentTemplate = docsauth.documenttemplateid         " &
                        "	And doc.IdEmployee = svaa.IDEmployee  " &
                        "   And (doc.Status = " & DocumentStatus.Validated & " and ((doc.StatusLevel <= docsauth.ApprovalLevelRequired and docsauth.ApprovalLevelRequired <> 0) OR docsauth.ApprovalLevelRequired = 0)) " &
                        "   And DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) between doc.BeginDate and doc.EndDate "
                        If bCheckPermissions Then
                            strEmployeeQuery = strEmployeeQuery & "INNER JOIN sysrofnSecurity_PermissionsOverDocumentTemplates(0, " & IIf(idRelatedObject > 0, idRelatedObject.ToString, "0") & ", -1, " & idPassport.ToString & ") PODT ON PODT.IdDocumentTemplate = docsauth.documenttemplateid AND PODT.IdEmployee = svaa.IdEmployee "
                        End If
                        strEmployeeQuery = strEmployeeQuery & "where doc.id is NULL  " &
                        "And (docpending.RowNumber1 Is NULL Or docpending.RowNumber1 = 1) " &
                        "And (docsauth.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsauth.BeginValidity <= DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))))  " &
                        "And (docsauth.EndValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsauth.EndValidity >= DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))))    "

                        If idRelatedObject > 0 Then
                            ' Para un empleado dado
                            strEmployeeQuery = strEmployeeQuery & " and svaa.idemployee = " & idRelatedObject.ToString
                        End If
                        If idAuthorization > 0 Then
                            strEmployeeQuery = strEmployeeQuery & " and svaa.IDAuthorization = " & idAuthorization.ToString
                        End If
                        strQuery = strEmployeeQuery
                    Case DocumentType.Company
                        'DOCUMENTOS DE PRL EXIGIBLES A EMPRESAS
                        strCompanyQuery = "@SELECT# docsauth.BeginValidity DueDate, " &
                        "isnull(doc.id, docpending.id) docid,  " &
                        "docsauth.documenttemplateid templateid,  " &
                        "NULL idemp, " &
                        "svac.IDCompany idcomp,   " &
                        "svac.IDAuthorization As authorizationId, " &
                        "docsauth.scope, " &
                        "docsauth.AccessValidation, " &
                        "docsauth.area, " &
                        "docpending.Status, "
                        If bCheckPermissions Then
                            strCompanyQuery = strCompanyQuery & "PODT.SupervisorLevelOfAuthority, "
                        End If
                        strCompanyQuery = strCompanyQuery & "docpending.StatusLevel " &
                        "From sysrovwAccessAuthorizationsbyCompany svac " &
                        "inner join (@SELECT# doct.Id documenttemplateid, doct.BeginValidity, doct.EndValidity, aadt.IDAuthorization, doct.LOPDAccessLevel, doct.Area, doct.scope, doct.AccessValidation, doct.ApprovalLevelRequired  from DocumentTemplates doct inner join AccessAuthorizationDocumentsTracking aadt on aadt.IDDocument  = doct.id where doct.Scope = " & DocumentScope.CompanyAccessAuthorization & " and doct.compulsory = 1) docsauth on docsauth.IDAuthorization = svac.IDAuthorization  " &
                        "Left join (@SELECT# row_number() over (partition by IdEmployee, IdDocumentTemplate order by IdDocumentTemplate desc) As 'RowNumber1', * from documents where Status = " & DocumentStatus.Pending & " OR Status = " & DocumentStatus.Validated & " ) docpending on docpending.IdDocumentTemplate = docsauth.documenttemplateid and docpending.IDCompany = svac.IDCompany  and (docpending.Status = " & DocumentStatus.Pending & " or (docpending.Status= " & DocumentStatus.Validated & " and docpending.StatusLevel > docsauth.ApprovalLevelRequired))" &
                        "Left join (@SELECT# * from documents) doc on doc.IdDocumentTemplate = docsauth.documenttemplateid         " &
                        "	and doc.IdCompany = svac.IDCompany  " &
                        "   And (doc.Status = " & DocumentStatus.Validated & " and ((doc.StatusLevel <= docsauth.ApprovalLevelRequired and docsauth.ApprovalLevelRequired <> 0) OR docsauth.ApprovalLevelRequired = 0)) " &
                        "   And DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) between doc.BeginDate and doc.EndDate "
                        If bCheckPermissions Then
                            strCompanyQuery = strCompanyQuery & "INNER JOIN sysrofnSecurity_PermissionsOverDocumentTemplates(0, -1, " & IIf(idRelatedObject > 0, idRelatedObject.ToString, "0") & ", " & idPassport.ToString & ") PODT ON PODT.IdDocumentTemplate = docsauth.documenttemplateid AND PODT.IdCompany = svac.IDCompany "
                        End If
                        strCompanyQuery = strCompanyQuery & "where doc.id is NULL  " &
                        " And (docpending.RowNumber1 Is NULL Or docpending.RowNumber1 = 1) " &
                        "And (docsauth.BeginValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsauth.BeginValidity <= DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))))  " &
                        "And (docsauth.EndValidity = CONVERT(smalldatetime,'1900-01-01 00:00:00.000',120) or (docsauth.EndValidity >= DATEADD(DAY," & dDaysOffset & ",DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))))    "

                        If idRelatedObject > 0 Then
                            ' Para una empresa dada
                            strCompanyQuery = strCompanyQuery & " and svac.IDCompany = " & idRelatedObject.ToString
                        End If
                        If idAuthorization > 0 Then
                            strCompanyQuery = strCompanyQuery & " and svac.IDAuthorization = " & idAuthorization.ToString
                        End If
                        strQuery = strCompanyQuery
                End Select

                If idRelatedObject <= 0 Then
                    Select Case type
                        Case DocumentType.Employee
                            strQuery = strQuery & " order by idemp asc"
                        Case DocumentType.Company
                            strQuery = strQuery & " order by idcomp asc"
                    End Select
                End If
                Dim tbDocuments As DataTable = CreateDataTable(strQuery)

                If tbDocuments.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbDocuments.Rows

                        Dim bSkipAlert As Boolean = False
                        Try
                            If checkStatusLevel AndAlso Not IsDBNull(oRow("statuslevel")) AndAlso Not IsDBNull(oRow("area")) AndAlso roTypes.Any2Integer(oRow("Status")) = 1 Then
                                Dim levelAuthoritySup As Integer
                                ' Sólo para pantalla de alertas, no muestro alertas de documentos en estado pendiente de validar si tienen un nivel de validadión igual o superior al mío (dado que no podré hacer nada con ellos)
                                ' Esto sólo para V3
                                ' El nivel
                                levelAuthoritySup = roTypes.Any2Integer(oRow("SupervisorLevelOfAuthority"))
                                bSkipAlert = (levelAuthoritySup >= roTypes.Any2Integer(oRow("statuslevel")))
                            End If
                        Catch ex As Exception
                        End Try

                        If Not bSkipAlert Then
                            Dim oAlert As New DocumentAlert
                            Dim sDocName As String = String.Empty
                            Dim sAlertType As String = String.Empty
                            If Not IsDBNull(oRow("docid")) Then oAlert.IDDocument = oRow("docid")
                            oAlert.IDDocumentTemplate = oRow("templateid")
                            Select Case type
                                Case DocumentType.Employee
                                    oAlert.IdRelatedObject = If(IsDBNull(oRow("idemp")), idRelatedObject, oRow("idemp"))
                                    oAlert.ObjectName = If(oAlert.IdRelatedObject > 0, ExecuteScalar("@SELECT# ISNULL(Name,'') FROM Employees WHERE ID=" & oAlert.IdRelatedObject), "")
                                Case DocumentType.Company
                                    oAlert.IdRelatedObject = If(IsDBNull(oRow("idcomp")), idRelatedObject, oRow("idcomp"))
                                    oAlert.ObjectName = If(oAlert.IdRelatedObject > 0, ExecuteScalar("@SELECT# ISNULL(Name,'') FROM Groups WHERE ID=" & oAlert.IdRelatedObject), "")
                            End Select
                            oAlert.DateTime = oRow("DueDate")
                            oAlert.Scope = oRow("scope")
                            If oAlert.IDDocument > 0 Then
                                sAlertType = "ValidationPending"
                            Else
                                sAlertType = "Undelivered"
                            End If

                            sDocName = Me.LoadDocumentTemplate(oAlert.IDDocumentTemplate).Name
                            oAlert.DocumentTemplateName = sDocName
                            oAlert.IdAccessAuthorization = oRow("authorizationId")
                            oAlert.AccessValidation = oRow("AccessValidation")

                            'Mensaje para un empleado. No hace falta poner el nombre ....
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(sDocName)

                            'En función del ámbito documento
                            oState.Language.AddUserToken(ExecuteScalar("@SELECT# Name FROM accessgroups WHERE ID=" & oRow("authorizationId")))
                            oAlert.Description = oState.Language.Translate("roDocumentManager.Document." & sAlertType & ".PRL", "")

                            oState.Language.ClearUserTokens()
                            oAlert.IsUrgent = True
                            oAlert.IsCritic = True

                            oPRLDocsAlerts.Add(oAlert)
                        End If
                    Next
                End If

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetPRLDocumentationFaultAlerts")
            End Try

            Return oPRLDocsAlerts.ToArray
        End Function

        ''' <summary>
        ''' Obtiene la lista de ids de area sobre los que un passport tiene permiso
        ''' </summary>
        ''' <param name="idPassport"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="iMinimumPermissionLevel"></param>
        ''' <returns></returns>
        Public Function GetAllowedDocumentAreas(ByVal idPassport As Integer, Optional iMinimumPermissionLevel As Integer = 3) As List(Of Integer)
            Dim oRet As New List(Of Integer)
            Dim strAuxQuery As String = String.Empty

            Try
                ' Calculamos permisos sobre tipos de documentos. Deben tener lectura o superior
                ' Siempre hay un elemento inexistente, por si no tiene ninguno de los permisos ...
                oRet.Add(-1)
                strAuxQuery = $"@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures WHERE IdPassport = {idPassport} AND FeatureAlias = 'Documents.Permision.Prevention' AND FeatureType = 'U'"
                If roTypes.Any2Integer(ExecuteScalar(strAuxQuery)) >= iMinimumPermissionLevel Then oRet.Add(DocumentArea.Prevention)
                strAuxQuery = $"@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures WHERE IdPassport = {idPassport} AND FeatureAlias = 'Documents.Permision.Labor' AND FeatureType = 'U'"
                If roTypes.Any2Integer(ExecuteScalar(strAuxQuery)) >= iMinimumPermissionLevel Then oRet.Add(DocumentArea.Labor)
                strAuxQuery = $"@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures WHERE IdPassport = {idPassport} AND FeatureAlias = 'Documents.Permision.Legal' AND FeatureType = 'U'"
                If roTypes.Any2Integer(ExecuteScalar(strAuxQuery)) >= iMinimumPermissionLevel Then oRet.Add(DocumentArea.Legal)
                strAuxQuery = $"@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures WHERE IdPassport = {idPassport} AND FeatureAlias = 'Documents.Permision.Security' AND FeatureType = 'U'"
                If roTypes.Any2Integer(ExecuteScalar(strAuxQuery)) >= iMinimumPermissionLevel Then oRet.Add(DocumentArea.Security)
                strAuxQuery = $"@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures WHERE IdPassport = {idPassport} AND FeatureAlias = 'Documents.Permision.Quality' AND FeatureType = 'U'"
                If roTypes.Any2Integer(ExecuteScalar(strAuxQuery)) >= iMinimumPermissionLevel Then oRet.Add(DocumentArea.Quality)

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetAllowedDocumentAreas")
            End Try
            Return oRet
        End Function

        ''' <summary>
        ''' Obtiene la lista de niveles de acceso LOPD sobre los que un passport tiene permiso
        ''' </summary>
        ''' <param name="idPassport"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="iMinimumPermissionLevel"></param>
        ''' <returns></returns>
        Public Function GetAllowedDocumentLOPDAccessLevels(ByVal idPassport As Integer, Optional iMinimumPermissionLevel As Integer = 3) As List(Of Integer)
            Dim oRet As New List(Of Integer)
            Dim strAuxQuery As String = String.Empty

            Try
                ' Calculamos permisos sobre campos de la ficha en función del nivel de accesos
                ' Siempre hay un elemento inexistente, por si no tiene ninguno de los permisos ...
                oRet.Add(-1)
                strAuxQuery = $"@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures WHERE IdPassport = {idPassport} AND FeatureAlias = 'Documents.AccessLevel.Low' AND FeatureType = 'U'"
                If roTypes.Any2Integer(ExecuteScalar(strAuxQuery)) >= iMinimumPermissionLevel Then oRet.Add(0)
                strAuxQuery = $"@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures WHERE IdPassport = {idPassport} AND FeatureAlias = 'Documents.AccessLevel.Medium' AND FeatureType = 'U'"
                If roTypes.Any2Integer(ExecuteScalar(strAuxQuery)) >= iMinimumPermissionLevel Then oRet.Add(1)
                strAuxQuery = $"@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures WHERE IdPassport = {idPassport} AND FeatureAlias = 'Documents.AccessLevel.High' AND FeatureType = 'U'"
                If roTypes.Any2Integer(ExecuteScalar(strAuxQuery)) >= iMinimumPermissionLevel Then oRet.Add(2)

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetAllowedDocumentLOPDAccessLevels")
            End Try
            Return oRet
        End Function

        Public Function GetDocumentsbyRequest(ByVal idRequest As Integer) As Generic.List(Of Integer)
            Dim oRet As New Generic.List(Of Integer)
            Dim strQuery As String = String.Empty
            Dim dTbl As DataTable

            Try
                strQuery = "@SELECT# Id from Documents where IdRequest = " & idRequest

                dTbl = CreateDataTable(strQuery)

                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each dRow As DataRow In dTbl.Rows
                        oRet.Add(dRow("ID"))
                    Next
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::GetDocumentsbyRequest")
            End Try
            Return oRet
        End Function

        Public Function CanAccessRequestDocumentation(ByVal idDocument As Integer) As Integer
            Dim oRet As Boolean = False
            Dim doc As Integer = 0
            Dim strQuery As String = String.Empty

            Try
                strQuery = "@SELECT# Id from Documents where Id = " & idDocument
                Dim idDoc = roTypes.Any2Integer(ExecuteScalar(strQuery))

                If idDoc > 0 Then oRet = CanEditDocument(idDoc, 3)

                If oRet Then
                    doc = idDoc
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::CanAccessRequestDocumentation")
            End Try
            Return doc
        End Function

        Public Function RegisterA3Payrroll() As Boolean
            Dim ret As Boolean = False
            Try

                Dim documenttemplate As roDocumentTemplate = New roDocumentTemplate
                documenttemplate.Id = -1
                documenttemplate.Name = oState.Language.Translate("roDocumentManager.A3PayrrollTemplate", "")
                documenttemplate.ShortName = A3PayrollTemplateName
                documenttemplate.Description = String.Empty
                documenttemplate.Notifications = String.Empty
                documenttemplate.Scope = DocumentScope.EmployeeContract
                documenttemplate.Area = DocumentArea.Labor
                documenttemplate.AccessValidation = 0
                documenttemplate.BeginValidity = roTypes.CreateDateTime(1900, 1, 1, 0, 0, 0)
                documenttemplate.EndValidity = roTypes.CreateDateTime(2079, 1, 1, 0, 0, 0)
                documenttemplate.ApprovalLevelRequired = 0
                documenttemplate.EmployeeDeliverAllowed = False
                documenttemplate.SupervisorDeliverAllowed = True
                documenttemplate.IsSystem = True
                documenttemplate.LOPDAccessLevel = DocumentLOPDLevel.High
                documenttemplate.Compulsory = False
                documenttemplate.ExpirePrevious = False
                documenttemplate.DaysBeforeDelete = 0
                documenttemplate.RequiresSigning = False
                documenttemplate.DefaultExpiration = 0
                documenttemplate.LeaveDocumentType = LeaveDocumentType.NotApplicable

                ret = Me.SaveDocumentTemplate(documenttemplate)

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::RegisterA3Payrroll")
            End Try
            Return ret
        End Function

        ''' <summary>
        ''' Si existe la plantilla de A3 Payroll, la elimina con todos los documentos!
        ''' </summary>
        ''' <returns></returns>
        Public Function UnRegisterA3Payrroll() As Boolean
            Dim ret As Boolean = False
            Try
                oState.Result = DocumentResultEnum.NoError

                ' Cargo plantilla Nómina A3
                Dim a3PayrollTemplates As List(Of roDocumentTemplate) = New List(Of roDocumentTemplate)
                a3PayrollTemplates = Me.GetTemplateByShortName(A3PayrollTemplateName)

                Select Case a3PayrollTemplates.Count
                    Case 0
                        ret = True
                    Case 1
                        ' Borro todos los documentos de la plantilla
                        Dim documents As List(Of roDocument) = GetDocumentsByTemplate(a3PayrollTemplates(0))

                        For Each doc In documents
                            DeleteDocument(doc.Id, True)
                        Next

                        ' Borro la plantilla
                        ret = DeleteDocumentTemplate(a3PayrollTemplates(0))
                    Case Else
                        oState.Result = DocumentResultEnum.A3MoreThanOneTemplate
                End Select

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDocumentManager::UnRegisterA3Payrroll")
            End Try
            Return ret
        End Function

#End Region

#End Region

#Region "Helper Methods"
        Public Shared Sub AddStat(sKey As String, ByRef dicStats As Dictionary(Of String, Double), ByRef watch As Stopwatch)
            Try
                watch.Stop()
                dicStats.Add(sKey, watch.Elapsed.TotalSeconds)
                watch.Restart()
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>
        ''' Valida si existe el nombre de un documento
        ''' </summary>
        ''' <param name="oDocTemplate"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function ExistsDocumentTemplateName(oDocTemplate As roDocumentTemplate) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@SELECT# ID FROM DocumentTemplates WHERE Name = LTRIM(RTRIM('" & oDocTemplate.Name & "')) AND ID <> " & oDocTemplate.Id
                Dim tb As DataTable = CreateDataTable(strSQL)

                bolRet = (tb.Rows.Count > 0)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::ExistsDocumentTemplateName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::ExistsDocumentTemplateName")
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Valida si un nombre corto ya existe
        ''' </summary>
        ''' <param name="oDocumentTemplate"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function ExistsDocumentTemplateExportName(oDocumentTemplate As roDocumentTemplate) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@SELECT# ID FROM DocumentTemplates WHERE ShortName = LTRIM(RTRIM( '" & oDocumentTemplate.ShortName & "')) and id <> " & oDocumentTemplate.Id.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                bolRet = (tb.Rows.Count > 0)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::ExistsDocumentTemplateExportName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::ExistsDocumentTemplateExportName")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Comprueba si se ha entregado algún documento correspondiente a una plantilla
        ''' </summary>
        ''' <param name="idTemplateDocument"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function IsTemplateDocumentInUse(idTemplateDocument As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@SELECT# 1 FROM Documents WHERE IdDocumentTemplate = " & idTemplateDocument
                strSQL = strSQL & " UNION "
                strSQL = strSQL & "@SELECT# 1 from CausesDocumentsTracking where IDDocument = " & idTemplateDocument
                Dim tb As DataTable = CreateDataTable(strSQL)

                bolRet = (tb.Rows.Count > 0)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::IsTemplateDocumentInUse")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::IsTemplateDocumentInUse")
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene el siguiente id de la tabla de documentsTemplate
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function GetNextDocumentTemplateID() As Integer
            ' Recupera el siguiente codigo de employee a usar
            Dim strQuery As String
            Dim intNextID As Integer

            intNextID = -1

            strQuery = "@SELECT# Max(ID) as Contador From DocumentTemplates "
            Try
                Dim tb As DataTable = CreateDataTable(strQuery)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    intNextID = roTypes.Any2Integer(tb.Rows(0).Item(0)) + 1
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::GetNextDocumentTemplateID")
            End Try

            Return intNextID

        End Function

        ''' <summary>
        ''' indica si el empleado disponde de permisos para editar el documento
        ''' </summary>
        ''' <param name="idDocument"></param>
        ''' <returns></returns>
        Public Function CanEditDocument(idDocument As Integer, Optional ByVal requieredPermission As Integer = 6) As Boolean
            ' Recupera el siguiente codigo de employee a usar
            Dim bolRet As Boolean = False
            Dim strQuery As String
            Dim intNextID As Integer
            intNextID = -1

            Try
                Dim aPermOverLOPD As Generic.List(Of Integer) = GetAllowedDocumentLOPDAccessLevels(Me.oState.IDPassport, requieredPermission)
                Dim aPermOverDocAreas As List(Of Integer) = GetAllowedDocumentAreas(Me.oState.IDPassport, requieredPermission)

                strQuery = "@SELECT# count(*) as Contador From DocumentTemplates dt inner join Documents dc on  dc.IdDocumentTemplate = dt.Id WHERE dc.Id = " & idDocument

                If aPermOverLOPD.Count > 0 Then
                    strQuery &= " AND dt.LOPDAccessLevel in (" & String.Join(",", aPermOverLOPD) & ")"
                Else
                    strQuery &= " AND dt.LOPDAccessLevel in (-1)"
                End If

                If aPermOverDocAreas.Count > 0 Then
                    strQuery &= " And dt.Area in (" & String.Join(",", aPermOverDocAreas) & ")"
                Else
                    strQuery &= " And dt.Area in (-1)"
                End If

                Dim numObjects As Integer = roTypes.Any2Integer(ExecuteScalar(strQuery))

                bolRet = (numObjects = 1)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::GetNextDocumentID")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' obtiene el siguiente Id de la tabla de documentos
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function GetNextDocumentID() As Integer
            ' Recupera el siguiente codigo de employee a usar
            Dim strQuery As String
            Dim intNextID As Integer

            intNextID = -1
            strQuery = "@SELECT# Max(ID) as Contador From Documents "

            Try
                Dim tb As DataTable = CreateDataTable(strQuery)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    intNextID = roTypes.Any2Integer(tb.Rows(0).Item(0)) + 1
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::GetNextDocumentID")
            End Try

            Return intNextID

        End Function

        Private Function CalculateDefaultExpirationDate(sExpirationPattern As String, dDateBegin As DateTime) As DateTime
            Dim dRet As DateTime = Date.MinValue
            Try
                Dim iPeriodType As Integer = 0
                Dim iRepetitions As Integer = 0
                If sExpirationPattern.IndexOf("@") > -1 Then
                    iPeriodType = roTypes.Any2Integer(sExpirationPattern.Split("@")(0))
                    iRepetitions = roTypes.Any2Integer(sExpirationPattern.Split("@")(1))
                End If

                Select Case iPeriodType
                    Case 1
                        dRet = dDateBegin.AddDays(iRepetitions)
                End Select
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::GetNextDocumentID")
            End Try
            Return dRet
        End Function

        ''' <summary>
        ''' Obtiene el nivel de autoridad de un passport
        ''' </summary>
        ''' <param name="idPassport"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Public Function GetPassportLevelOfAuthority(idPassport As Integer, ByVal oArea As DocumentArea) As Integer
            ' Recupera el siguiente codigo de employee a usar
            Dim iLevel As Integer = 0
            Dim strQuery As String

            Try
                strQuery = $"@SELECT# LevelOfAuthority FROM sysroPassports_Categories WHERE IDPassport = @idpassport and IdCategory = @area"
                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idpassport", CommandParameter.ParameterType.tInt, idPassport))
                parameters.Add(New CommandParameter("@area", CommandParameter.ParameterType.tInt, oArea))
                iLevel = roTypes.Any2Integer(AccessHelper.ExecuteScalar(strQuery, parameters))

                If iLevel = 0 Then iLevel = 15

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::GetPassportLevelOfAuthority")
            End Try

            Return iLevel

        End Function

        ''' <summary>
        ''' Valida si la plantilla tiene cambios que fuercen un broadcaster
        ''' </summary>
        ''' <param name="iOldTemplate"></param>
        ''' <param name="iNewTemplate"></param>
        ''' <returns></returns>
        Private Function DocumentTemplateNotifyBroadCaster(iOldTemplate As DataRow, iNewTemplate As roDocumentTemplate) As Boolean
            Dim bolRet As Boolean = False
            Try
                Select Case iNewTemplate.Scope
                    Case DocumentScope.CompanyAccessAuthorization, DocumentScope.EmployeeAccessAuthorization
                        If Not (roTypes.Any2String(iOldTemplate("Name")).Equals(iNewTemplate.Name)) Then Return True
                        If Not (roTypes.Any2Integer(iOldTemplate("AccessValidation")).Equals(iNewTemplate.AccessValidation)) Then Return True
                        If Not (roTypes.Any2Boolean(iOldTemplate("Compulsory")).Equals(iNewTemplate.Compulsory)) Then Return True
                        If Not (DateDiff(DateInterval.Day, roTypes.Any2DateTime(iOldTemplate("BeginValidity")), iNewTemplate.BeginValidity).Equals(0)) Then Return True
                        If Not (DateDiff(DateInterval.Day, roTypes.Any2DateTime(iOldTemplate("EndValidity")), iNewTemplate.EndValidity).Equals(0)) Then Return True
                End Select
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::DocumentTemplateNotifyBroadCaster")
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Valida si el documento entregado tiene cambios que fuercen un broadcaster
        ''' </summary>
        ''' <param name="iOldDocument"></param>
        ''' <param name="iNewDocument"></param>
        ''' <returns></returns>
        Private Function DocumentNotifyBroadCaster(iOldDocument As DataRow, iNewDocument As roDocument) As Boolean
            Dim bolRet As Boolean = False
            Try
                Select Case iNewDocument.DocumentTemplate.Scope
                    Case DocumentScope.CompanyAccessAuthorization, DocumentScope.EmployeeAccessAuthorization
                        If Not (roTypes.Any2Integer(iOldDocument("Status")).Equals(iNewDocument.Status)) Then Return True
                        If Not (roTypes.Any2Integer(iOldDocument("StatusLevel")).Equals(iNewDocument.StatusLevel)) Then Return True
                        If Not (DateDiff(DateInterval.Day, roTypes.Any2DateTime(iOldDocument("BeginDate")), iNewDocument.BeginDate).Equals(0)) Then Return True
                        If Not (DateDiff(DateInterval.Day, roTypes.Any2DateTime(iOldDocument("EndDate")), iNewDocument.EndDate).Equals(0)) Then Return True
                End Select
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::DocumentNotifyBroadCaster")
            End Try

            Return bolRet
        End Function

        Private Function IsEmployeeDocument(oDocument As roDocument) As Boolean
            Dim bolRet As Boolean = False
            Try
                Return Not (oDocument.DocumentTemplate.Scope = DocumentScope.Company OrElse oDocument.DocumentTemplate.Scope = DocumentScope.CompanyAccessAuthorization)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDocumentManager::IsEmployeeDocument")
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Indica si un documento de empleado existe. Se considera que existe si tiene el mismo título y extensión, plantilla y está asignado al mismo empleado.
        ''' </summary>
        ''' <param name="document"></param>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        Public Function CheckIfEmployeeDocumentExists(ByVal document As roDocument) As roDocument
            Dim retDocument As roDocument = Nothing

            Try
                State.Result = DocumentResultEnum.NoError
                Dim sqlString As String
                sqlString = $"@SELECT# TOP 1 ID FROM Documents WHERE UPPER(Title) = '{document.Title.ToUpper}' AND DocumentType = '{document.DocumentType}' AND IdDocumentTemplate = {document.DocumentTemplate.Id} AND IdEmployee = {document.IdEmployee} AND Id <> {document.Id} ORDER BY DeliveryDate DESC"
                Dim idexistingdocument = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(sqlString))

                If idexistingdocument > 0 Then retDocument = LoadDocument(idexistingdocument)

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::CheckIfEmployeeDocumentExists")
            End Try

            Return retDocument
        End Function


        ''' <summary>
        ''' Indica si un documento de empleado existe. Se considera que existe si tiene el mismo título y extensión, plantilla y está asignado al mismo empleado.
        ''' </summary>
        ''' <param name="document"></param>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        Public Function GetDocumentsByExternalId(ByVal newdocument As roDocument) As List(Of roDocument)
            Dim retDocument As List(Of roDocument) = New List(Of roDocument)

            Try
                State.Result = DocumentResultEnum.NoError
                Dim idexistingdocument As Integer
                Dim sqlString As String
                Dim tableresult As DataTable
                If Not String.IsNullOrEmpty(newdocument.DocumentExternalID) Then
                    sqlString = $"@SELECT# Id FROM Documents WHERE UPPER(DocumentExternalID) = '{newdocument.DocumentExternalID.ToUpper}' AND Id <> {newdocument.Id} ORDER BY DeliveryDate DESC"
                    tableresult = CreateDataTable(sqlString)
                    If tableresult IsNot Nothing AndAlso tableresult.Rows.Count > 0 Then
                        For Each row As DataRow In tableresult.Rows
                            idexistingdocument = roTypes.Any2Integer(row("ID"))
                            retDocument.Add(LoadDocument(idexistingdocument))
                        Next
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager:: GetDocumentsByExternalId")
            End Try

            Return retDocument
        End Function

        Public Shared Function GetTotalSignedDocumentsOnYear(year As Integer, oState As roDocumentState) As Integer
            Dim result As Integer = 0
            Try
                Dim sqlTotalSignedDocsThisYear As String = $"@SELECT# COUNT(*) FROM sysroTrackedEvents WHERE Event = '{DTOs.TrackedEvent.DocumentSigned.ToString}' AND DATEPART(YEAR,Date) = {year}"
                result = roTypes.Any2Integer(ExecuteScalar(sqlTotalSignedDocsThisYear))
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDocumentManager::GetTotalSignedDocuments")
            End Try

            Return result
        End Function

#End Region

    End Class

End Namespace