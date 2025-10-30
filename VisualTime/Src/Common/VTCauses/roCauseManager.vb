Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Causes

    Public Class roCauseManager
        Private oState As roCauseManagerState = Nothing

        Public ReadOnly Property State As roCauseManagerState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roCauseManagerState()
        End Sub

        Public Sub New(ByVal _State As roCauseManagerState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal IDCause As Integer, Optional ByVal bAudit As Boolean = False) As roCauseEngine

            Dim oCause As roCauseEngine = Nothing
            Try

                Dim strQuery As String = " @SELECT# * from Causes Where ID = " & IDCause.ToString

                Dim oDt As DataTable = CreateDataTable(strQuery)

                If oDt IsNot Nothing AndAlso oDt.Rows.Count = 1 Then
                    oCause = New roCauseEngine

                    oCause.AutomaticEquivalenceType = eAutomaticEquivalenceType.DeactivatedType
                    oCause.AutomaticEquivalenceCriteria = New roEngineAutomaticEquivalenceCriteria
                    oCause.ApplyWorkDaysOnConcept = False
                    oCause.ID = IDCause
                    Dim oRow As DataRow = oDt.Rows(0)

                    oCause.Name = oRow("Name")
                    oCause.RoundingBy = oRow("RoundingBy")

                    oCause.CostFactor = oRow("CostFactor")

                    oCause.MaxTimeToForecast = oRow("MaxTimeToForecast")

                    Select Case oRow("RoundingType")
                        Case "+"
                            oCause.RoundingType = eRoundingType.Round_UP
                        Case "-"
                            oCause.RoundingType = eRoundingType.Round_Down
                        Case "~"
                            oCause.RoundingType = eRoundingType.Round_Near
                    End Select

                    oCause.AllowInputFromReader = oRow("AllowInputFromReader")
                    If Not IsDBNull(oRow("ReaderInputcode")) Then
                        oCause.ReaderInputcode = oRow("ReaderInputcode")
                    Else
                        oCause.ReaderInputcode = 0
                    End If
                    oCause.WorkingType = oRow("WorkingType")
                    oCause.Description = oRow("Description")
                    oCause.Color = oRow("Color")
                    oCause.ShortName = oRow("ShortName")
                    oCause.StartsProgrammedAbsence = oRow("StartsProgrammedAbsence")
                    If Not IsDBNull(oRow("MaxProgrammedAbsenceDays")) Then
                        oCause.MaxProgrammedAbsence = oRow("MaxProgrammedAbsenceDays")
                    Else
                        oCause.MaxProgrammedAbsence = 0
                    End If

                    If Not IsDBNull(oRow("AbsenceMandatoryDays")) Then
                        oCause.AbsenceMandatoryDays = oRow("AbsenceMandatoryDays")
                    Else
                        oCause.AbsenceMandatoryDays = -1
                    End If

                    oCause.RoundingByDailyScope = oRow("RoundingByDailyScope")
                    If Not IsDBNull(oRow("ApplyAbsenceOnHolidays")) Then
                        oCause.ApplyAbsenceOnHolidays = oRow("ApplyAbsenceOnHolidays")
                    Else
                        oCause.ApplyAbsenceOnHolidays = False
                    End If
                    oCause.CauseType = oRow("CauseType")
                    oCause.PunchCloseProgrammedAbsence = oRow("PunchCloseProgrammedAbsence")

                    oCause.VisibilityPermissions = oRow("VisibilityPermissions")
                    oCause.InputPermissions = oRow("InputPermissions")

                    oCause.ApplyJustifyPeriod = oRow("ApplyJustifyPeriod")
                    If Not IsDBNull(oRow("JustifyPeriodStart")) Then
                        oCause.JustifyPeriodStart = oRow("JustifyPeriodStart")
                    End If
                    If Not IsDBNull(oRow("JustifyPeriodEnd")) Then
                        oCause.JustifyPeriodEnd = oRow("JustifyPeriodEnd")
                    End If
                    If Not IsDBNull(oRow("JustifyPeriodType")) Then
                        If oRow("JustifyPeriodType") = True Then
                            oCause.JustifyPeriodType = eJustifyPeriodType.JustifyPeriod
                        Else
                            oCause.JustifyPeriodType = eJustifyPeriodType.DontJustify
                        End If
                    End If

                    If Not IsDBNull(oRow("Export")) Then
                        oCause.Export = oRow("Export")
                    End If

                    If Not IsDBNull(oRow("ExternalWork")) Then
                        oCause.ExternalWork = oRow("ExternalWork")
                    End If

                    If Not IsDBNull(oRow("IsHoliday")) Then
                        oCause.IsHoliday = roTypes.Any2Boolean(oRow("IsHoliday"))
                    End If

                    If Not IsDBNull(oRow("DayType")) Then
                        oCause.DayType = roTypes.Any2Boolean(oRow("DayType"))
                    End If

                    If Not IsDBNull(oRow("CustomType")) Then
                        oCause.CustomType = roTypes.Any2Boolean(oRow("CustomType"))
                    End If

                    If Not IsDBNull(oRow("BusinessGroup")) Then
                        oCause.BusinessCenter = oRow("BusinessGroup")
                    End If

                    oCause.IDConceptBalance = IIf(Not IsDBNull(oRow("IDConceptBalance")), oRow("IDConceptBalance"), 0)

                    oCause.TraceDocumentsAbsences = roTypes.Any2Boolean(oRow("TraceDocumentsAbsences"))

                    Try
                        Dim oCollection As New roCollection(roTypes.Any2String(oRow("DefaultValuesAbsences")))

                        If Not oCollection("MaxDays") Is Nothing Then oCause.Absence_MaxDays = roTypes.Any2Integer(oCollection("MaxDays"))

                        If Not oCollection("BetweenMax") Is Nothing Then oCause.Absence_BetweenMax = roTypes.Any2DateTime(oCollection("BetweenMax"))
                        If Not oCollection("BetweenMin") Is Nothing Then oCause.Absence_BetweenMin = roTypes.Any2DateTime(oCollection("BetweenMin"))

                        If Not oCollection("DurationMax") Is Nothing Then oCause.Absence_DurationMax = roTypes.Any2DateTime(oCollection("DurationMax"))
                        If Not oCollection("DurationMin") Is Nothing Then oCause.Absence_DurationMin = roTypes.Any2DateTime(oCollection("DurationMin"))
                    Catch ex As Exception
                        oState.UpdateStateInfo(ex, "roCause::Load")
                    End Try

                    oCause.Documents = Me.GetDocumentsByIdCause(IDCause)

                    oCause.AutomaticEquivalenceType = roTypes.Any2Integer(oRow("AutomaticEquivalenceType"))
                    oCause.AutomaticEquivalenceIDCause = roTypes.Any2Integer(oRow("AutomaticEquivalenceIDCause"))

                    If oCause.AutomaticEquivalenceType <> eAutomaticEquivalenceType.DeactivatedType Then
                        oCause.AutomaticEquivalenceCriteria = New roEngineAutomaticEquivalenceCriteria
                        oCause.AutomaticEquivalenceCriteria.AutomaticEquivalenceType = oCause.AutomaticEquivalenceType

                        Dim strXml As String = roTypes.Any2String(oRow("AutomaticEquivalenceCriteria"))
                        If strXml <> "" Then
                            ' Añadimos la composición a la colección
                            Dim oDefinition As New roCollection(strXml)

                            If oDefinition.Exists("FactorValue") And oCause.AutomaticEquivalenceCriteria.AutomaticEquivalenceType = eAutomaticEquivalenceType.DirectValueType Then
                                oCause.AutomaticEquivalenceCriteria.FactorValue = oDefinition("FactorValue")
                            End If

                            If oDefinition.Exists("FactorField") And oCause.AutomaticEquivalenceCriteria.AutomaticEquivalenceType = eAutomaticEquivalenceType.FieldType Then
                                Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState(-1)
                                oCause.AutomaticEquivalenceCriteria.UserField = New VTUserFields.UserFields.roUserField(oUserFieldState, oDefinition("FactorField"), UserFieldsTypes.Types.EmployeeField, False)
                            End If
                        End If
                    End If

                    If Not IsDBNull(oRow("RequestAvailability")) Then
                        oCause.RequestAvailability = roTypes.Any2String(oRow("RequestAvailability"))
                    Else
                        oCause.RequestAvailability = "-1"
                    End If

                    oCause.ApplyWorkDaysOnConcept = False
                    If Not IsDBNull(oRow("ApplyWorkDaysOnConcept")) Then
                        oCause.ApplyWorkDaysOnConcept = roTypes.Any2Boolean(oRow("ApplyWorkDaysOnConcept"))
                    End If

                    If Not IsDBNull(oRow("MinLevelOfAuthority")) Then
                        oCause.MinLevelOfAuthority = roTypes.Any2Integer(oRow("MinLevelOfAuthority"))
                    Else
                        oCause.MinLevelOfAuthority = 11
                    End If

                    If Not IsDBNull(oRow("ApprovedAtLevel")) Then
                        oCause.ApprovedAtLevel = roTypes.Any2Integer(oRow("ApprovedAtLevel"))
                    Else
                        oCause.ApprovedAtLevel = 1
                    End If

                    If Not IsDBNull(oRow("IDCategory")) Then
                        oCause.IDCategory = roTypes.Any2Integer(oRow("IDCategory"))
                    Else
                        oCause.IDCategory = 6
                    End If
                End If

                ' Auditar lectura
                If bAudit AndAlso oCause IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCause, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCauseManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCauseManager::Load")
            Finally

            End Try

            Return oCause
        End Function

        Public Function GetDocumentsByIdCause(ByVal IDCause As Integer) As Generic.List(Of roEngineCauseDocument)
            Dim lst As New Generic.List(Of roEngineCauseDocument)

            Try

                Dim oCauseDocument As roEngineCauseDocument

                Dim SQL As String = "@SELECT# CausesDocumentsTracking.ID, CausesDocumentsTracking.IDCause, CausesDocumentsTracking.IDLabAgree, CausesDocumentsTracking.IDDocument, CausesDocumentsTracking.Parameters,  " &
                                    "DocumentTemplates.Name AS DocumentName, LabAgree.Name AS LabAgreeName FROM CausesDocumentsTracking LEFT OUTER JOIN DocumentTemplates ON  " &
                                    "CausesDocumentsTracking.IDDocument = DocumentTemplates.ID LEFT OUTER JOIN LabAgree ON CausesDocumentsTracking.IDLabAgree = LabAgree.ID WHERE IDCause = " & IDCause &
                                    "ORDER BY DocumentTemplates.Name, LabAgree.Name "

                Dim tb As DataTable = CreateDataTable(SQL, "Docs")
                For Each oRow As DataRow In tb.Rows
                    oCauseDocument = New roEngineCauseDocument()

                    oCauseDocument.ID = roTypes.Any2Integer(oRow("ID"))
                    oCauseDocument.IDCause = roTypes.Any2Integer(oRow("IDCause"))
                    oCauseDocument.IDLabAgree = roTypes.Any2Integer(oRow("IDLabAgree"))
                    oCauseDocument.IDdocument = roTypes.Any2Integer(oRow("IDDocument"))
                    oCauseDocument.LabAgreeName = roTypes.Any2String(oRow("LabAgreeName"))
                    oCauseDocument.DocumentName = roTypes.Any2String(oRow("DocumentName"))

                    Dim oXmlConf As String = roTypes.Any2String(oRow("Parameters"))
                    If oXmlConf <> String.Empty Then
                        Dim oCollection As New roCollection(oXmlConf)
                        oCauseDocument.TypeRequest = oCollection("TypeRequest")
                        oCauseDocument.NumberItems = oCollection("NumberItems")
                        oCauseDocument.NumberItems2 = oCollection("NumberItems2")
                        oCauseDocument.FlexibleWhen = oCollection("FlexibleWhen")
                        oCauseDocument.FlexibleWhen2 = oCollection("FlexibleWhen2")
                    Else
                        oCauseDocument.TypeRequest = TypeRequestEnum.AtBegin
                        oCauseDocument.NumberItems = -1
                        oCauseDocument.NumberItems2 = -1
                        oCauseDocument.FlexibleWhen = -1
                        oCauseDocument.FlexibleWhen2 = -1
                    End If

                    lst.Add(oCauseDocument)
                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCauseManager::GetDocumentsByIdCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCauseManager::GetDocumentsByIdCause")
            Finally

            End Try

            Return lst

        End Function

#End Region

    End Class

End Namespace