Imports System.Data.Common
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Xml
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace DataLink

    <DataContract>
    Public Class roDataLink

        Private Const VbeInstallationPathKey As String = "SOFTWARE\Microsoft\VBA"
        Private Const VbeInstallationPath64Key As String = "SOFTWARE\Wow6432Node\Microsoft\VBA"
        Private Const Vbe6InstallationPathValue As String = "Vbe6DllPath"
        Private Const Vbe7InstallationPathValue As String = "Vbe7DllPath"

        Private Enum StandardXmlExportSections
            <EnumMember> EmployeeFields = 0
            <EnumMember> Contracts = 1
            <EnumMember> Mobilities = 2
            <EnumMember> DailyConcepts = 3
            <EnumMember> TotalConcepts = 4
            <EnumMember> Punches = 5
            <EnumMember> Dinings = 6
            <EnumMember> DailySchedule = 7
            <EnumMember> DailyScheduleLayers = 8
            <EnumMember> DailyTasks = 9
            <EnumMember> PunchesTasks = 10
            <EnumMember> ProgrammedAbsences = 11
            <EnumMember> PunchesDinner = 12
            <EnumMember> CompanyFields = 13
        End Enum

#Region "Declarations"

        Public Const roNullDate = "1/1/2079"

        Private oState As roDataLinkState

        Public Sub New()
            Me.oState = New roDataLinkState
        End Sub

        Public Sub New(ByVal _State As roDataLinkState)
            Me.oState = _State
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roDataLinkState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roDataLinkState)
                Me.oState = value
            End Set
        End Property

#End Region

#Region "ExportData methods"

        Public Function GetTemplatesExcel() As Generic.List(Of String)
            Dim strIni As String = String.Empty
            Dim strInf As String = String.Empty
            Dim strNameToShow As String = String.Empty
            Dim strExportPath As String = String.Empty
            Dim strFileType As String = String.Empty
            Dim strExportSections As String = String.Empty

            Dim myList As New Generic.List(Of String)

            Try
                Dim oSupport As New roSupport()

                'obtener clave de registro PATH/DATALINK
                Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
                Dim strPathTemplates As String = oSettings.GetVTSetting(eKeys.DataLink)

                Dim myDir As New DirectoryInfo(strPathTemplates)
                'comprobar si el directorio existe realmente
                If myDir.Exists Then

                    Dim FilesXLS As FileInfo() = myDir.GetFiles("*.xls")
                    For Each AuxFileXLS As FileInfo In FilesXLS
                        strIni = Path.Combine(AuxFileXLS.DirectoryName, Path.GetFileNameWithoutExtension(AuxFileXLS.Name) & ".ini")
                        strInf = Path.Combine(AuxFileXLS.DirectoryName, Path.GetFileNameWithoutExtension(AuxFileXLS.Name) & ".inf")
                        If File.Exists(strIni) And File.Exists(strInf) Then

                            strNameToShow = oSupport.INIRead(strIni, "Config", "Name")
                            strNameToShow = IIf(strNameToShow <> String.Empty, strNameToShow, Path.GetFileNameWithoutExtension(strIni))

                            strExportPath = oSupport.INIRead(strIni, "Config", "ExportPath")
                            If strExportPath = String.Empty Then strExportPath = AuxFileXLS.DirectoryName
                            If strExportPath <> String.Empty And Not strExportPath.EndsWith("\") Then strExportPath = strExportPath & "\"

                            strFileType = oSupport.INIRead(strIni, "Config", "OutputFileType")
                            If strFileType <> String.Empty And Not strFileType.StartsWith(".") Then strFileType = "." & strFileType

                            strExportSections = oSupport.INIRead(strInf, "Config", "ExportSections")
                            If strExportSections <> String.Empty Then strExportSections = strExportSections

                            If strFileType <> String.Empty Then
                                Dim wSepChar As String = Chr(94) & Chr(124) & Chr(94)
                                myList.Add(AuxFileXLS.FullName & wSepChar & strNameToShow & wSepChar & strExportPath & wSepChar & strFileType & wSepChar & strExportSections)
                            End If
                        End If
                    Next
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLink::GetTemplatesExcel")
            End Try

            Return myList

        End Function

        'Public Function ExecuteExtractionDataTemplateExcel(ByVal strTemplateFileName As String, ByVal strExportFileName As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal mEmployees As String, Optional ByVal bolAudit As Boolean = True, Optional ExportUserFieldName As Boolean = False) As Byte()
        '    Dim bRet As Byte() = Nothing
        '    Dim bRet2 As Byte() = Nothing

        '    Dim FilePath As String = Path.ChangeExtension(strTemplateFileName, ".xml")

        '    bRet = ExecuteExtractionDataXMLStandard(BeginDate, EndDate, mEmployees, FilePath, False, bolAudit)
        '    If Not bRet Is Nothing Then

        '        If roDataLink.IsExcelInstalled(oState) Then

        '            'crear referencia a aplicacion excel
        '            Dim oExcelApp As Robotics.Excel9.Interop.Application = Nothing
        '            Dim oBook As Robotics.Excel9.Interop.Workbook = Nothing

        '            Try

        '                oExcelApp = New Robotics.Excel9.Interop.Application
        '                oBook = oExcelApp.Workbooks.Open(strTemplateFileName)

        '                'comprobar que la ruta de destino exista sino se asume que es la misma que donde esta situada la plantilla
        '                If Not System.IO.File.Exists(strExportFileName) Then
        '                    strExportFileName = System.IO.Path.GetDirectoryName(strTemplateFileName) & "\" & System.IO.Path.GetFileName(strExportFileName)
        '                End If

        '                'comprobar si despues de abrir la plantilla, el fichero de salida se ha generado
        '                If File.Exists(strExportFileName) Then
        '                    bRet2 = Me.GetFileBytes(strExportFileName)
        '                End If

        '            Catch ex As System.Runtime.InteropServices.COMException 'controla que el fichero xls no exista cuando realmente se requiere
        '                Me.oState.UpdateStateInfo(ex, "roDataLink::ExecuteExtractionDataTemplateExcel")

        '            Catch ex As Exception
        '                Me.oState.UpdateStateInfo(ex, "roDataLink::ExecuteExtractionDataTemplateExcel")
        '            Finally
        '                If oBook IsNot Nothing Then oBook.Close()
        '                If oExcelApp IsNot Nothing Then oExcelApp.Quit()
        '            End Try
        '        Else

        '            bRet2 = bRet

        '        End If

        '    End If

        '    Return bRet2

        'End Function

        Public Function ExecuteExtractionDataTemplate(ByVal IDGuide As Integer, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal mEmployees As String, ByVal FilePath As String, Optional ByVal bolAudit As Boolean = True) As Byte()
            Dim bRet() As Byte = Nothing

            Dim ExistErr As Boolean = False

            Try

                If mEmployees.EndsWith(",") Then mEmployees = mEmployees.Substring(0, mEmployees.Length - 1)

                ' Cargamos los empleados a exportat
                Dim tb As DataTable
                tb = New DataTable("ListEmployees")

                Dim strSQL As String = "@SELECT# * FROM Employees WHERE ID IN (" & mEmployees.ToString & ") Order by ID"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim strLine As String = ""
                Dim sw As StreamWriter = Nothing
                If FilePath = "" Then
                    FilePath = GetTmpFilePath(IDGuide, roGuide.eGuideType.ASCII)
                End If

                ' Abrimos el fichero a exportar
                sw = New StreamWriter(FilePath)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRowAux As DataRow In tb.Rows
                        ' Obtenemos el codigo de Empleado y Empresa
                        Dim strEmployeeCode As String = GetEmployeeCode(Any2Integer(oRowAux("ID")))
                        Dim strCompanyCode As String = GetCompanyCode(Any2Integer(oRowAux("ID")))
                        Dim eSQL As String = ""

                        Dim xEndDate As Date

                        If IDGuide = 8000 Then
                            ' Diario
                            eSQL = "@SELECT# IDConcept , convert(numeric(18,6), sum(Value)) as total , ShortName, Name, Export, Date "
                        Else
                            ' Periodo
                            eSQL = "@SELECT# IDConcept , convert(numeric(18,6), sum(Value)) as total , ShortName, Name, Export"

                            ' Obtenemos la fecha final del periodo a exportar en funcion del contrato y el periodo seleccionado
                            xEndDate = GetMinEndDate(Any2Double(oRowAux("ID")), BeginDate, EndDate)
                        End If

                        eSQL = eSQL & " From DailyAccruals, Concepts "
                        eSQL = eSQL & " Where IDEmployee= " & oRowAux("ID")
                        eSQL = eSQL & " and Concepts.ID = DailyAccruals.IDCOncept"
                        eSQL = eSQL & " And Concepts.Export is not null and Concepts.Export <> '0' "
                        eSQL = eSQL & " and Date >=" & Any2Time(BeginDate).SQLSmallDateTime & " and Date <=" & Any2Time(EndDate).SQLSmallDateTime

                        If IDGuide = 8000 Then
                            ' Diario
                            eSQL = eSQL & " Group By IDConcept, ShortName, Name, Export, Date"
                            eSQL = eSQL & " Order By IDConcept, Date"
                        Else
                            ' Periodo
                            eSQL = eSQL & " Group By IDConcept, ShortName, Name, Export"
                            eSQL = eSQL & " Order By IDConcept"
                        End If

                        Dim etb As DataTable = CreateDataTable(eSQL)
                        If etb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            For Each eRowAux As DataRow In etb.Rows
                                strLine = Right("00000" & Any2Double(strCompanyCode).ToString, 5)
                                strLine += ";" & Right("00000" & Any2Double(strEmployeeCode).ToString, 5)

                                If IDGuide = 8000 Then
                                    strLine += ";" & Format(eRowAux("Date"), "dd/MM/yyyy")
                                Else
                                    strLine += ";" & Format(BeginDate, "dd/MM/yyyy")
                                    strLine += ";" & Format(xEndDate, "dd/MM/yyyy")
                                End If

                                strLine += ";" & Right("0000" & Any2Double(eRowAux("Export")).ToString, 4)

                                Dim FormatValue As String = ""
                                FormatValue = Any2Double(eRowAux("total")).ToString
                                FormatValue = FormatValue.Replace(",", ".")

                                Dim strInt As String = ""
                                If StringItemsCount(FormatValue, ".") = 2 Then
                                    strInt = Math.Abs(Any2Double(String2Item(FormatValue, 0, "."))).ToString
                                    strInt = Right("00000000" & strInt, 8)
                                    FormatValue = strInt & "," & Left(String2Item(FormatValue, 1, ".") & "00", 2)
                                Else
                                    strInt = Math.Abs(Any2Double(eRowAux("total"))).ToString
                                    FormatValue = Right("00000000" & strInt, 8) & ",00"
                                End If

                                If Any2Double(eRowAux("total")) < 0 Then
                                    FormatValue = "-" & Mid(FormatValue, 2)
                                End If

                                strLine += ";" & FormatValue

                                sw.WriteLine(strLine)
                            Next
                        End If
                    Next
                End If
                sw.Close()

                bRet = Me.GetFileBytes(FilePath)

                If bolAudit Then
                    'Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tDataLink, "", tbParameters, -1)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLink::ExecuteExtractionDataTemplate")
            Finally

                If FilePath <> "" Then
                    ' Borrar fichero temporal
                    Try
                        File.Delete(FilePath)
                    Catch ex As Exception
                    End Try
                End If
            End Try

            Return bRet

        End Function

        Public Function ExecuteExtractionDataXMLStandard(ByVal BeginDate As Date, ByVal EndDate As Date, ByVal mEmployees As String, ByVal FilePath As String, ByVal ExportUsrFieldName As Boolean, Optional ByVal bolAudit As Boolean = True) As Byte()
            Dim bRet() As Byte = Nothing
            Dim oSettings As New roSettings
            Dim strPath As String = oSettings.GetVTSetting(eKeys.DataLink)
            Dim memory_stream As New FileStream(System.IO.Path.Combine(strPath, "StandardExport" & "_" & oState.IDPassport & ".xml"), FileMode.Create)
            'Dim aux As String
            Dim XMLDoc As New Xml.XmlDocument
            Dim strSQL As String = ""

            Dim XML_Text_Writer As New XmlTextWriter(memory_stream,
                System.Text.Encoding.UTF8)
            Dim stream_reader As New StreamReader(memory_stream)
            Dim ElementGroupCollection As New roCollection

            Dim DeleteTempFile As Boolean = False

            Try

                If mEmployees.EndsWith(",") Then mEmployees = mEmployees.Substring(0, mEmployees.Length - 1)

                ' Inicializamos el fichero XML
                XML_Text_Writer.Formatting = Formatting.Indented
                XML_Text_Writer.Indentation = 4

                XML_Text_Writer.WriteStartDocument()
                XML_Text_Writer.WriteStartElement("Employees")

                ' Cargamos los empleados a exportat
                Dim tb As DataTable
                tb = New DataTable("ListEmployees")

                strSQL = "@SELECT# * FROM Employees WHERE ID IN (" & mEmployees.ToString & ") Order by ID"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim excelTemplates As Generic.List(Of String) = GetTemplatesExcel()
                    Dim exportSections As New Generic.List(Of Integer)

                    For Each aElement As Object In excelTemplates
                        Dim splitSep() As String = {Chr(94) & Chr(124) & Chr(94)}
                        Dim cads As String() = aElement.ToString.Split(splitSep, StringSplitOptions.None)

                        If cads(0) = FilePath.Replace("xml", "xls") Then
                            Dim sections As String() = cads(4).Split(";")
                            For Each mySec As String In sections
                                If mySec.Trim() <> "" Then
                                    exportSections.Add(roTypes.Any2Integer(mySec.Trim()))
                                End If

                            Next
                            Exit For
                        End If
                    Next

                    Dim oPassport As roPassportTicket = Me.GetPassport()
                    Dim oSupport As roSupport = Me.GetSupport(oPassport)

                    For Each oRowAux As DataRow In tb.Rows
                        ' Para cada Empleado
                        XML_Text_Writer.WriteStartElement("Employee")

                        ' Datos Generales
                        ElementGroupCollection = New roCollection

                        ElementGroupCollection.Add("Language", oSupport.ActiveLanguage)
                        ElementGroupCollection.Add("ExportBeginPeriod", Format(BeginDate, "dd/MM/yyyy"))
                        ElementGroupCollection.Add("ExportEndPeriod", Format(EndDate, "dd/MM/yyyy"))
                        ElementGroupCollection.Add("Name", oRowAux("Name"))
                        ElementGroupCollection.Add("HighLightColor", oRowAux("HighlightColor"))

                        XMLInsertElementGroup(XML_Text_Writer, "General", ElementGroupCollection)

                        'Sección 0
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.EmployeeFields) Then
                            ' Datos de la ficha
                            'XML_Text_Writer.WriteStartElement("EmployeeFields")
                            GetDataFieldsElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate, ExportUsrFieldName)
                            'XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 1
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.Contracts) Then
                            ' Datos de los contratos
                            XML_Text_Writer.WriteStartElement("Contracts")
                            GetDataContractsElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 2
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.Mobilities) Then
                            ' Datos de las movilidades
                            XML_Text_Writer.WriteStartElement("Mobilities")
                            GetDataMobilitiesElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 3
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.DailyConcepts) Then
                            ' Datos de los Acumulados diarios
                            XML_Text_Writer.WriteStartElement("DailyConcepts")
                            GetDataConceptsElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 4
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.TotalConcepts) Then
                            ' Datos de los Acumulados totales del periodo
                            XML_Text_Writer.WriteStartElement("TotalConcepts")
                            GetDataTotalConceptsElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 5
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.Punches) Then
                            ' Datos de los fichajes y planificación
                            '## XML_Text_Writer.WriteStartElement("Moves")
                            '## GetDataMovesElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            XML_Text_Writer.WriteStartElement("Punches")
                            GetDataPunchesElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 6
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.Dinings) Then
                            'Total Fichajes de Comedor
                            XML_Text_Writer.WriteStartElement("Dinings")
                            GetDataDiningElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 7
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.DailySchedule) Then
                            ' Datos de la planificacion
                            XML_Text_Writer.WriteStartElement("DailySchedule")
                            GetDailyScheduleElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate, exportSections.Contains(StandardXmlExportSections.DailyScheduleLayers))
                            XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 9
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.DailyTasks) Then
                            ' Datos de los Acumulados diarios de tareas
                            XML_Text_Writer.WriteStartElement("DailyTasks")
                            GetDataTasksElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 10
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.PunchesTasks) Then
                            ' Datos de los fichajes de produccion y tareas
                            XML_Text_Writer.WriteStartElement("PunchTasks")
                            GetDataPunchesTasksElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 11
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.ProgrammedAbsences) Then
                            ' Datos de ausencias programadas
                            XML_Text_Writer.WriteStartElement("ProgrammedAbsences")
                            GetDataProgrammedAbsencesElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 12
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.PunchesDinner) Then
                            ' Datos de los fichajes de produccion y tareas
                            XML_Text_Writer.WriteStartElement("PunchDinner")
                            GetDataPunchesDinnerElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            XML_Text_Writer.WriteEndElement()
                        End If

                        'Sección 13
                        If exportSections.Count = 0 Or exportSections.Contains(StandardXmlExportSections.CompanyFields) Then
                            ' Datos de la ficha
                            'XML_Text_Writer.WriteStartElement("EmployeeFields")
                            GetDataCompanyFieldsElement(XML_Text_Writer, ElementGroupCollection, Any2Long(oRowAux("ID")), BeginDate, EndDate)
                            'XML_Text_Writer.WriteEndElement()
                        End If

                        XML_Text_Writer.WriteEndElement()

                    Next
                End If

                XML_Text_Writer.WriteEndDocument()
                XML_Text_Writer.Flush()

                XML_Text_Writer.Close()

                bRet = Me.GetFileBytes(memory_stream.Name)

                If bolAudit Then
                    'Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tDataLink, "", tbParameters, -1)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLink::ExecuteExtractionDataXMLStandard")
            Finally

                If DeleteTempFile Then
                    If FilePath <> "" Then
                        ' Borrar fichero temporal
                        Try
                            File.Delete(FilePath)
                        Catch ex As Exception
                        End Try
                    End If
                End If
            End Try

            Return bRet

        End Function

        Private Function GetPassport() As roPassportTicket

            Dim oPassport As roPassportTicket = Nothing
            Try
                oPassport = roPassportManager.GetPassportTicket(Me.oState.IDPassport, LoadType.Passport)
            Catch
            End Try

            Return oPassport

        End Function

        Private Function GetSupport(Optional ByVal _Passport As roPassportTicket = Nothing) As roSupport

            Dim oRet As roSupport

            Dim oPassport As roPassportTicket = _Passport
            If oPassport Is Nothing Then
                Try
                    oPassport = GetPassport()
                Catch
                End Try
            End If
            If oPassport IsNot Nothing Then
                oRet = New roSupport(oPassport.Language.Key)
            Else
                oRet = New roSupport()
            End If

            Return oRet

        End Function

        Private Sub GetDailyScheduleElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal exportLayers As Boolean)
            '
            ' Obtenemos los datos de los acumulados totales del periodo indicado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            Dim ctrSQL As String = ""
            Dim dtrSQL As String = ""
            Dim rtrSQl As String = ""
            Dim strName As String = ""
            Dim strShortName As String = ""
            Dim strDescripcion As String = ""
            Dim Color As Integer = 0
            Dim workingHour As Double = 0
            Dim shiftGroup As String = ""
            Dim isHolidays As Boolean = False
            tb = New DataTable("Fields")

            strSQL = "@SELECT# Date,IDShift1,IDShift2,IDShift3,IDShift4,IDShiftUsed,IDShiftBase,Remarks,IDAssignment  " &
                    " FROM DailySchedule" &
                    " WHERE IDEmployee = " & lngIDEmployee.ToString &
                    " AND Date >= " & Any2Time(dtBeginDate).SQLSmallDateTime &
                    " AND Date <= " & Any2Time(dtEndDate).SQLSmallDateTime

            Dim aState As New Absence.roProgrammedAbsenceState
            Dim absencesDt As DataTable = Absence.roProgrammedAbsence.GetProgrammedAbsences(lngIDEmployee, dtBeginDate, dtEndDate, aState)

            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada Campo
                    ElementGroupCollection = New roCollection

                    ElementGroupCollection.Add("Date", Format(oRowAux("Date"), "yyyy/MM/dd"))

                    If oRowAux("IDShift1").ToString <> "" Then
                        ctrSQL = "@SELECT# Name from Shifts where ID = " & oRowAux("IDShift1").ToString
                        strName = Any2String(ExecuteScalar(ctrSQL))
                        ctrSQL = "@SELECT# ShortName from Shifts where ID = " & oRowAux("IDShift1").ToString
                        strShortName = Any2String(ExecuteScalar(ctrSQL))
                        dtrSQL = "@SELECT# Description from Shifts where ID = " & oRowAux("IDShift1").ToString
                        strDescripcion = Any2String(ExecuteScalar(dtrSQL))
                        rtrSQl = "@SELECT# color from Shifts where ID = " & oRowAux("IDShift1").ToString
                        Color = Any2String(ExecuteScalar(rtrSQl))

                        isHolidays = Shift.roShift.IsHolidays(roTypes.Any2Integer(oRowAux("IDShift1")))

                        Try
                            If isHolidays Then
                                rtrSQl = "@SELECT# ExpectedWorkingHours from Shifts where ID = " & oRowAux("IDShiftBase").ToString
                            Else
                                rtrSQl = "@SELECT# ExpectedWorkingHours from Shifts where ID = " & oRowAux("IDShift1").ToString
                            End If

                            workingHour = Any2Double(ExecuteScalar(rtrSQl))
                        Catch ex As Exception
                            workingHour = 0
                        End Try

                        rtrSQl = "@SELECT# dbo.getfullgrouppathname(IDGroup) FROM EmployeeGroups where IDEmployee = " & lngIDEmployee.ToString & " And BeginDate <= " & Any2Time(oRowAux("Date")).SQLSmallDateTime & " and EndDate >= " & Any2Time(oRowAux("Date")).SQLSmallDateTime
                        shiftGroup = Any2String(ExecuteScalar(rtrSQl))

                        ElementGroupCollection.Add("Shift1", oRowAux("IDShift1").ToString)
                        ElementGroupCollection.Add("ShiftName1", strName.ToString)
                        ElementGroupCollection.Add("ShiftShortName1", strShortName.ToString)
                        ElementGroupCollection.Add("Description1", strDescripcion.ToString)
                        ElementGroupCollection.Add("Color1", Color.ToString)
                        If exportLayers Then
                            ElementGroupCollection.Add("ShiftZones1", GetShiftZones(oRowAux("IDShift1")))
                        Else
                            ElementGroupCollection.Add("ShiftZones1", "")
                        End If
                        ElementGroupCollection.Add("WorkingHours1", workingHour.ToString())
                        ElementGroupCollection.Add("ShiftGroup1", shiftGroup.ToString())
                        ElementGroupCollection.Add("IsHolidays1", IIf(isHolidays = True, "1", "0"))
                        ElementGroupCollection.Add("ProgrammedAbsence1", GetInProgrammedAbsence(roTypes.Any2DateTime(oRowAux("Date")), absencesDt))
                        ElementGroupCollection.Add("HasRequest1", GetRequestsInDate(lngIDEmployee, workingHour, roTypes.Any2DateTime(oRowAux("Date"))))
                    Else
                        ElementGroupCollection.Add("Shift1", oRowAux("IDShift1").ToString)
                    End If
                    If oRowAux("IDShift2").ToString <> "" Then
                        ctrSQL = "@SELECT# Name from Shifts where ID = " & oRowAux("IDShift2").ToString
                        strName = Any2String(ExecuteScalar(ctrSQL))
                        ctrSQL = "@SELECT# ShortName from Shifts where ID = " & oRowAux("IDShift2").ToString
                        strShortName = Any2String(ExecuteScalar(ctrSQL))
                        dtrSQL = "@SELECT# Description from Shifts where ID = " & oRowAux("IDShift2").ToString
                        strDescripcion = Any2String(ExecuteScalar(dtrSQL))
                        rtrSQl = "@SELECT# color from Shifts where ID = " & oRowAux("IDShift2").ToString
                        Color = Any2String(ExecuteScalar(rtrSQl))
                        rtrSQl = "@SELECT# ExpectedWorkingHours from Shifts where ID = " & oRowAux("IDShift2").ToString
                        workingHour = Any2Double(ExecuteScalar(rtrSQl))
                        rtrSQl = "@SELECT# dbo.getfullgrouppathname(IDGroup) FROM EmployeeGroups where IDEmployee = " & lngIDEmployee.ToString & " And BeginDate <= " & Any2Time(oRowAux("Date")).SQLSmallDateTime & " and EndDate >= " & Any2Time(oRowAux("Date")).SQLSmallDateTime
                        shiftGroup = Any2String(ExecuteScalar(rtrSQl))
                        ElementGroupCollection.Add("Shift2", oRowAux("IDShift2").ToString)
                        ElementGroupCollection.Add("ShiftName2", strName.ToString)
                        ElementGroupCollection.Add("ShiftShortName2", strShortName.ToString)
                        ElementGroupCollection.Add("Description2", strDescripcion.ToString)
                        ElementGroupCollection.Add("Color2", Color.ToString)
                        If exportLayers Then
                            ElementGroupCollection.Add("ShiftZones2", GetShiftZones(oRowAux("IDShift2")))
                        Else
                            ElementGroupCollection.Add("ShiftZones2", "")
                        End If
                        ElementGroupCollection.Add("WorkingHours2", workingHour.ToString())
                        ElementGroupCollection.Add("ShiftGroup2", shiftGroup.ToString())
                        ElementGroupCollection.Add("IsHolidays2", "0")
                        ElementGroupCollection.Add("ProgrammedAbsence2", "0")
                        ElementGroupCollection.Add("HasRequest2", "")
                    Else
                        ElementGroupCollection.Add("Shift2", oRowAux("IDShift2").ToString)
                    End If
                    If oRowAux("IDShift3").ToString <> "" Then
                        ctrSQL = "@SELECT# Name from Shifts where ID = " & oRowAux("IDShift3").ToString
                        strName = Any2String(ExecuteScalar(ctrSQL))
                        ctrSQL = "@SELECT# ShortName from Shifts where ID = " & oRowAux("IDShift3").ToString
                        strShortName = Any2String(ExecuteScalar(ctrSQL))
                        dtrSQL = "@SELECT# Description from Shifts where ID = " & oRowAux("IDShift3").ToString
                        strDescripcion = Any2String(ExecuteScalar(dtrSQL))
                        rtrSQl = "@SELECT# color from Shifts where ID = " & oRowAux("IDShift3").ToString
                        Color = Any2String(ExecuteScalar(rtrSQl))
                        rtrSQl = "@SELECT# ExpectedWorkingHours from Shifts where ID = " & oRowAux("IDShift3").ToString
                        workingHour = Any2Double(ExecuteScalar(rtrSQl))
                        rtrSQl = "@SELECT# dbo.getfullgrouppathname(IDGroup) FROM EmployeeGroups where IDEmployee = " & lngIDEmployee.ToString & " And BeginDate <= " & Any2Time(oRowAux("Date")).SQLSmallDateTime & " and EndDate >= " & Any2Time(oRowAux("Date")).SQLSmallDateTime
                        shiftGroup = Any2String(ExecuteScalar(rtrSQl))
                        ElementGroupCollection.Add("Shift3", oRowAux("IDShift3").ToString)
                        ElementGroupCollection.Add("ShiftName3", strName.ToString)
                        ElementGroupCollection.Add("ShiftShortName3", strShortName.ToString)
                        ElementGroupCollection.Add("Description3", strDescripcion.ToString)
                        ElementGroupCollection.Add("Color3", Color.ToString)
                        If exportLayers Then
                            ElementGroupCollection.Add("ShiftZones3", GetShiftZones(oRowAux("IDShift3")))
                        Else
                            ElementGroupCollection.Add("ShiftZones3", "")
                        End If
                        ElementGroupCollection.Add("WorkingHours3", workingHour.ToString())
                        ElementGroupCollection.Add("ShiftGroup3", shiftGroup.ToString())
                        ElementGroupCollection.Add("IsHolidays3", "0")
                        ElementGroupCollection.Add("ProgrammedAbsence3", "0")
                        ElementGroupCollection.Add("HasRequest3", "")
                    Else
                        ElementGroupCollection.Add("Shift3", oRowAux("IDShift3").ToString)
                    End If
                    If oRowAux("IDShift4").ToString <> "" Then
                        ctrSQL = "@SELECT# Name from Shifts where ID = " & oRowAux("IDShift4").ToString
                        strName = Any2String(ExecuteScalar(ctrSQL))
                        ctrSQL = "@SELECT# ShortName from Shifts where ID = " & oRowAux("IDShift4").ToString
                        strShortName = Any2String(ExecuteScalar(ctrSQL))
                        dtrSQL = "@SELECT# Description from Shifts where ID = " & oRowAux("IDShift4").ToString
                        strDescripcion = Any2String(ExecuteScalar(dtrSQL))
                        rtrSQl = "@SELECT# color from Shifts where ID = " & oRowAux("IDShift4").ToString
                        Color = Any2String(ExecuteScalar(rtrSQl))
                        rtrSQl = "@SELECT# ExpectedWorkingHours from Shifts where ID = " & oRowAux("IDShift4").ToString
                        workingHour = Any2Double(ExecuteScalar(rtrSQl))
                        rtrSQl = "@SELECT# dbo.getfullgrouppathname(IDGroup) FROM EmployeeGroups where IDEmployee = " & lngIDEmployee.ToString & " And BeginDate <= " & Any2Time(oRowAux("Date")).SQLSmallDateTime & " and EndDate >= " & Any2Time(oRowAux("Date")).SQLSmallDateTime
                        shiftGroup = Any2String(ExecuteScalar(rtrSQl))
                        ElementGroupCollection.Add("Shift4", oRowAux("IDShift4").ToString)
                        ElementGroupCollection.Add("ShiftName4", strName.ToString)
                        ElementGroupCollection.Add("ShiftShortName4", strShortName.ToString)
                        ElementGroupCollection.Add("Description4", strDescripcion.ToString)
                        ElementGroupCollection.Add("Color4", Color.ToString)
                        If exportLayers Then
                            ElementGroupCollection.Add("ShiftZones4", GetShiftZones(oRowAux("IDShift4")))
                        Else
                            ElementGroupCollection.Add("ShiftZones4", "")
                        End If
                        ElementGroupCollection.Add("WorkingHours4", workingHour.ToString())
                        ElementGroupCollection.Add("ShiftGroup4", shiftGroup.ToString())
                        ElementGroupCollection.Add("IsHolidays4", "0")
                        ElementGroupCollection.Add("ProgrammedAbsence4", "0")
                        ElementGroupCollection.Add("HasRequest4", "")
                    Else
                        ElementGroupCollection.Add("Shift4", oRowAux("IDShift4").ToString)
                    End If
                    If oRowAux("IDShiftUsed").ToString <> "" Then
                        ctrSQL = "@SELECT# Name from Shifts where ID = " & oRowAux("IDShiftUsed").ToString
                        strName = Any2String(ExecuteScalar(ctrSQL))
                        ctrSQL = "@SELECT# ShortName from Shifts where ID = " & oRowAux("IDShiftUsed").ToString
                        strShortName = Any2String(ExecuteScalar(ctrSQL))
                        dtrSQL = "@SELECT# Description from Shifts where ID = " & oRowAux("IDShiftUsed").ToString
                        strDescripcion = Any2String(ExecuteScalar(dtrSQL))
                        rtrSQl = "@SELECT# color from Shifts where ID = " & oRowAux("IDShiftUsed").ToString
                        Color = Any2String(ExecuteScalar(rtrSQl))

                        isHolidays = Shift.roShift.IsHolidays(roTypes.Any2Integer(oRowAux("IDShiftUsed")))

                        Try
                            If isHolidays Then
                                rtrSQl = "@SELECT# ExpectedWorkingHours from Shifts where ID = " & oRowAux("IDShiftBase").ToString
                            Else
                                rtrSQl = "@SELECT# ExpectedWorkingHours from Shifts where ID = " & oRowAux("IDShiftUsed").ToString
                            End If

                            workingHour = Any2Double(ExecuteScalar(rtrSQl))
                        Catch ex As Exception
                            workingHour = 0
                        End Try

                        rtrSQl = "@SELECT# dbo.getfullgrouppathname(IDGroup) FROM EmployeeGroups where IDEmployee = " & lngIDEmployee.ToString & " And BeginDate <= " & Any2Time(oRowAux("Date")).SQLSmallDateTime & " and EndDate >= " & Any2Time(oRowAux("Date")).SQLSmallDateTime
                        shiftGroup = Any2String(ExecuteScalar(rtrSQl))

                        ElementGroupCollection.Add("ShiftUsed", oRowAux("IDShiftUsed").ToString)
                        ElementGroupCollection.Add("ShiftNameUsed", strName.ToString)
                        ElementGroupCollection.Add("ShiftShortNameUsed", strShortName.ToString)
                        ElementGroupCollection.Add("DescriptionUsed", strDescripcion.ToString)
                        ElementGroupCollection.Add("ColorUsed", Color.ToString)
                        If exportLayers Then
                            ElementGroupCollection.Add("ShiftZonesUsed", GetShiftZones(oRowAux("IDShiftUsed")))
                        Else
                            ElementGroupCollection.Add("ShiftZonesUsed", "")
                        End If
                        ElementGroupCollection.Add("WorkingHoursUsed", workingHour.ToString())
                        ElementGroupCollection.Add("ShiftGroupUsed", shiftGroup.ToString())
                        ElementGroupCollection.Add("IsHolidaysUsed", IIf(isHolidays = True, "1", "0"))
                        ElementGroupCollection.Add("ProgrammedAbsenceUsed", GetInProgrammedAbsence(roTypes.Any2DateTime(oRowAux("Date")), absencesDt))
                        ElementGroupCollection.Add("HasRequestUsed", GetRequestsInDate(lngIDEmployee, workingHour, roTypes.Any2DateTime(oRowAux("Date"))))
                    Else
                        ElementGroupCollection.Add("ShiftUsed", oRowAux("IDShiftUsed").ToString)
                    End If

                    ElementGroupCollection.Add("Remarks", oRowAux("Remarks").ToString)

                    ' Obtenemos el puesto asignado en caso que tenga

                    If oRowAux("IDAssignment").ToString <> "" Then
                        ctrSQL = "@SELECT# Name from Assignments where ID = " & oRowAux("IDAssignment").ToString
                        strName = Any2String(ExecuteScalar(ctrSQL))
                        ctrSQL = "@SELECT# ShortName from Assignments where ID = " & oRowAux("IDAssignment").ToString
                        strShortName = Any2String(ExecuteScalar(ctrSQL))
                        dtrSQL = "@SELECT# Description from Assignments where ID = " & oRowAux("IDAssignment").ToString
                        strDescripcion = Any2String(ExecuteScalar(dtrSQL))
                        rtrSQl = "@SELECT# color from Assignments where ID = " & oRowAux("IDAssignment").ToString
                        Color = Any2String(ExecuteScalar(rtrSQl))

                        ElementGroupCollection.Add("AssignmentName", strName.ToString)
                        ElementGroupCollection.Add("AssignmentShortName", strShortName.ToString)
                        ElementGroupCollection.Add("AssignmentColor", Color.ToString)
                        ElementGroupCollection.Add("AssignmentDescription", strDescripcion.ToString)
                    Else
                        ElementGroupCollection.Add("AssignmentName", oRowAux("IDAssignment").ToString)
                    End If

                    XMLInsertElementGroup(xml_text_writer, "Schedule", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Function GetRequestsInDate(ByVal idEmployee As Long, ByVal baseWorkingHours As Double, ByVal sDate As Date) As String
            Dim rVal As String = ""

            Dim rtrSQl As String = "@SELECT# * from Requests where RequestType = 6 AND IDEmployee = " & idEmployee & " AND Date1 <= " & Any2Time(sDate).SQLSmallDateTime & " AND Date2 >= " & Any2Time(sDate).SQLSmallDateTime & " AND Status IN(0,1)"

            Dim exObj As DataTable = CreateDataTable(rtrSQl, )

            If exObj IsNot Nothing AndAlso exObj.Rows.Count > 0 Then

                Dim idRequestedShift As Integer = roTypes.Any2Integer(exObj.Rows(0)("IDShift"))
                rtrSQl = "@SELECT# color from Shifts where ID = " & idRequestedShift.ToString
                Dim color As String = Any2String(ExecuteScalar(rtrSQl))

                rVal = color & "#" & baseWorkingHours
            End If

            Return rVal
        End Function

        Private Function GetInProgrammedAbsence(ByVal sDate As Date, ByVal absencesDt As DataTable) As String
            Dim rVal As String = "0"

            If absencesDt IsNot Nothing AndAlso absencesDt.Rows.Count > 0 Then

                For Each oAbsence As DataRow In absencesDt.Rows
                    Dim beginDate As Date = roTypes.Any2DateTime(oAbsence("BeginDate"))
                    Dim endDate As Date
                    If oAbsence("FinishDate") Is DBNull.Value Then
                        endDate = beginDate.AddDays(roTypes.Any2Integer(oAbsence("MaxLastingDays")) - 1)
                    Else
                        endDate = roTypes.Any2DateTime(oAbsence("FinishDate"))
                    End If

                    If sDate.Ticks >= beginDate.Ticks AndAlso sDate.Ticks <= endDate.Ticks Then
                        rVal = "1"
                    End If

                    If rVal = "1" Then
                        Exit For
                    End If
                Next
            End If

            Return rVal
        End Function

        Private Function GetShiftZones(ByVal shiftId) As String
            Dim zones As String = ""

            Dim oCurrentShift As New Shift.roShift
            oCurrentShift.ID = shiftId
            oCurrentShift.Load(False)

            Dim oData As roCollection
            For Each oLayer As Shift.roShiftLayer In oCurrentShift.Layers
                oData = New roCollection(oLayer.DataStoredXML)

                If (oLayer.LayerType = roLayerTypes.roLTMandatory) Then

                    If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.Begin)) AndAlso oData.Exists(oLayer.XmlKey(roXmlLayerKeys.Finish)) Then
                        Dim oInitTime As Date = roTypes.Any2DateTime(oData.Item(oLayer.XmlKey(roXmlLayerKeys.Begin)))
                        Dim oMaxTime As Date = roTypes.Any2DateTime(oData.Item(oLayer.XmlKey(roXmlLayerKeys.Finish)))

                        If (Not String.IsNullOrEmpty(zones)) Then
                            zones = zones & "#"
                        End If

                        Select Case (oInitTime.Day)
                            Case 29
                                zones = zones & "-" & oInitTime.ToString("HH:mm")
                            Case 30
                                zones = zones & "=" & oInitTime.ToString("HH:mm")
                            Case 31
                                zones = zones & "+" & oInitTime.ToString("HH:mm")
                            Case Else
                                zones = zones & "=" & oInitTime.ToString("HH:mm")
                        End Select
                        zones = zones & "@"
                        Select Case (oMaxTime.Day)
                            Case 29
                                zones = zones & "-" & oMaxTime.ToString("HH:mm")
                            Case 30
                                zones = zones & "=" & oMaxTime.ToString("HH:mm")
                            Case 31
                                zones = zones & "+" & oMaxTime.ToString("HH:mm")
                            Case Else
                                zones = zones & "=" & oMaxTime.ToString("HH:mm")
                        End Select

                    End If
                End If
            Next

            Return zones
        End Function

        Private Sub GetDataDiningElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de los movimentos del periodo indicado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            tb = New DataTable("Dinings")

            strSQL = "@SELECT# isnull(count(*), 0) as Total " &
                    " FROM Punches " &
                    " WHERE " &
                    " IDEmployee = " & lngIDEmployee.ToString &
                    " AND ShiftDate >= " & Any2Time(dtBeginDate).SQLSmallDateTime &
                    " AND ShiftDate <= " & Any2Time(dtEndDate).SQLSmallDateTime &
                    " AND DateTime is not null " &
                    " AND Type IN(" & Any2Double(PunchTypeEnum._DR) & ")" &
                    " AND (InvalidType = 0 or InvalidType is null)"

            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada movimiento
                    ElementGroupCollection = New roCollection
                    ElementGroupCollection.Add("DiningTotalPeriod", oRowAux("Total").ToString.Replace(",", "."))
                    XMLInsertElementGroup(xml_text_writer, "Dining", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Sub GetDataPunchesElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de los movimentos del periodo indicado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            tb = New DataTable("Punches")

            strSQL = "@SELECT# Punches.*, (@SELECT# ReaderInputCode FROM Causes WHERE ID = Punches.TypeData) as CauseCode, (@SELECT# Name FROM Shifts WHERE ID IN(@SELECT# IDShiftUsed FROM DailySchedule Where IDEmployee = Punches.IDEmployee AND Date = Punches.ShiftDate)) as PunchShiftName, (@SELECT# ShortName FROM Shifts WHERE ID IN(@SELECT# IDShiftUsed FROM DailySchedule Where IDEmployee = Punches.IDEmployee AND Date = Punches.ShiftDate)) as PunchShiftShortName " &
                    " , (@SELECT# Shifts.ExpectedWorkingHours FROM Shifts WHERE ID IN(@SELECT# IDShiftUsed FROM DailySchedule Where IDEmployee = Punches.IDEmployee AND Date = Punches.ShiftDate)) as PunchShiftExpectedWorkingHours " &
                    " FROM Punches " &
                    " WHERE " &
                    " IDEmployee = " & lngIDEmployee.ToString &
                    " AND ShiftDate >= " & Any2Time(dtBeginDate).SQLSmallDateTime &
                    " AND ShiftDate <= " & Any2Time(dtEndDate).SQLSmallDateTime &
                    " AND DateTime is not null " &
                    " AND ActualType IN(" & Any2Double(PunchTypeEnum._IN) & "," & Any2Double(PunchTypeEnum._OUT) & ")" &
                    " Order by ShiftDate, DateTime, ID"
            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada movimiento
                    ElementGroupCollection = New roCollection
                    ElementGroupCollection.Add("PunchShiftDate", Format(oRowAux("ShiftDate"), "yyyy/MM/dd"))
                    ElementGroupCollection.Add("PunchShiftName", oRowAux("PunchShiftName").ToString)
                    ElementGroupCollection.Add("PunchShiftShortName", oRowAux("PunchShiftShortName").ToString)
                    ElementGroupCollection.Add("PunchShiftExpectedWorkingHours", oRowAux("PunchShiftExpectedWorkingHours").ToString.Replace(",", "."))
                    ElementGroupCollection.Add("PunchDateTime", Format(oRowAux("DateTime"), "yyyy/MM/dd HH:mm"))
                    ElementGroupCollection.Add("PunchOperation", oRowAux("ActualType").ToString)
                    ElementGroupCollection.Add("PunchTerminal", oRowAux("IDTerminal").ToString)
                    ElementGroupCollection.Add("PunchCauseCode", oRowAux("CauseCode").ToString)
                    ElementGroupCollection.Add("PunchType", oRowAux("Type").ToString)
                    XMLInsertElementGroup(xml_text_writer, "Punch", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Sub GetDataPunchesTasksElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de los movimentos del periodo indicado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            tb = New DataTable("Punches")

            strSQL = "@SELECT# Punches.*, (@SELECT# Name FROM Shifts WHERE ID IN(@SELECT# IDShiftUsed FROM DailySchedule Where IDEmployee = Punches.IDEmployee AND Date = Punches.ShiftDate)) as PunchShiftName, (@SELECT# ShortName FROM Shifts WHERE ID IN(@SELECT# IDShiftUsed FROM DailySchedule Where IDEmployee = Punches.IDEmployee AND Date = Punches.ShiftDate)) as PunchShiftShortName " &
                    " , (@SELECT# Shifts.ExpectedWorkingHours FROM Shifts WHERE ID IN(@SELECT# IDShiftUsed FROM DailySchedule Where IDEmployee = Punches.IDEmployee AND Date = Punches.ShiftDate)) as PunchShiftExpectedWorkingHours " &
                    " , dbo.BusinessCenters.Name AS Center, dbo.Tasks.Name AS TaskName, dbo.Tasks.Project, dbo.Tasks.ShortName AS TaskShortName " &
                    " FROM dbo.Punches INNER JOIN  dbo.Tasks ON dbo.Punches.TypeData = dbo.Tasks.id INNER JOIN dbo.BusinessCenters ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID " &
                    " WHERE " &
                    " IDEmployee = " & lngIDEmployee.ToString &
                    " AND ShiftDate >= " & Any2Time(dtBeginDate).SQLSmallDateTime &
                    " AND ShiftDate <= " & Any2Time(dtEndDate).SQLSmallDateTime &
                    " AND DateTime is not null " &
                    " AND ActualType IN(" & Any2Double(PunchTypeEnum._TASK) & ")" &
                    " and dbo.Punches.Type = 4  " &
                    " Order by ShiftDate, DateTime, Punches.ID"
            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada movimiento
                    ElementGroupCollection = New roCollection
                    ElementGroupCollection.Add("TasksPunchShiftDate", Format(oRowAux("ShiftDate"), "yyyy/MM/dd"))
                    ElementGroupCollection.Add("TasksPunchShiftName", oRowAux("PunchShiftName").ToString)
                    ElementGroupCollection.Add("TasksPunchShiftShortName", oRowAux("PunchShiftShortName").ToString)
                    ElementGroupCollection.Add("TasksPunchShiftExpectedWorkingHours", oRowAux("PunchShiftExpectedWorkingHours").ToString.Replace(",", "."))
                    ElementGroupCollection.Add("TasksPunchDateTime", Format(oRowAux("DateTime"), "yyyy/MM/dd HH:mm"))
                    ElementGroupCollection.Add("TasksPunchOperation", oRowAux("ActualType").ToString)
                    ElementGroupCollection.Add("TasksPunchTerminal", oRowAux("IDTerminal").ToString)
                    ElementGroupCollection.Add("TasksPunchType", oRowAux("Type").ToString)
                    ElementGroupCollection.Add("TasksPunchTypeData", oRowAux("TypeData").ToString)
                    ElementGroupCollection.Add("TasksPunchCenter", oRowAux("Center").ToString)
                    ElementGroupCollection.Add("TasksPunchTaskName", oRowAux("TaskName").ToString)
                    ElementGroupCollection.Add("TasksPunchProject", oRowAux("Project").ToString)
                    ElementGroupCollection.Add("TasksPunchTaskShortName", oRowAux("TaskShortName").ToString)
                    ElementGroupCollection.Add("TasksPunchField1", oRowAux("Field1").ToString)
                    ElementGroupCollection.Add("TasksPunchField2", oRowAux("Field2").ToString)
                    ElementGroupCollection.Add("TasksPunchField3", oRowAux("Field3").ToString)
                    ElementGroupCollection.Add("TasksPunchField4", oRowAux("Field4").ToString)
                    ElementGroupCollection.Add("TasksPunchField5", oRowAux("Field5").ToString)
                    ElementGroupCollection.Add("TasksPunchField6", oRowAux("Field6").ToString)
                    XMLInsertElementGroup(xml_text_writer, "TasksPunch", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Sub GetDataPunchesDinnerElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de los movimentos del periodo indicado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            tb = New DataTable("Punches")

            strSQL = "@SELECT# Punches.* " &
                    " FROM dbo.Punches " &
                    " WHERE " &
                    " IDEmployee = " & lngIDEmployee.ToString &
                    " AND ShiftDate >= " & Any2Time(dtBeginDate).SQLSmallDateTime &
                    " AND ShiftDate <= " & Any2Time(dtEndDate).SQLSmallDateTime &
                    " AND DateTime is not null " &
                    " AND ActualType IN(" & Any2Double(PunchTypeEnum._DR) & ")" &
                    " and dbo.Punches.Type = 10 " &
                    " Order by ShiftDate, DateTime, Punches.ID"
            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada movimiento
                    ElementGroupCollection = New roCollection
                    ElementGroupCollection.Add("DinnerPunchShiftDate", Format(oRowAux("ShiftDate"), "yyyy/MM/dd"))
                    ElementGroupCollection.Add("DinnerPunchType", oRowAux("Type").ToString)
                    ElementGroupCollection.Add("DinnerPunchInvalidType", oRowAux("InvalidType").ToString)

                    XMLInsertElementGroup(xml_text_writer, "DinnerPunch", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Sub GetDataMovesElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de los movimentos del periodo indicado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            tb = New DataTable("Moves")

            strSQL = "@SELECT# Moves.*, (@SELECT# ReaderInputCode FROM Causes WHERE ID = Moves.InIDCause) as InCauseCode, (@SELECT# ReaderInputCode FROM Causes WHERE ID = Moves.OutIDCause) as OutCauseCode , (@SELECT# Name FROM Shifts WHERE ID IN(@SELECT# IDShiftUsed FROM DailySchedule Where IDEmployee = Moves.IDEmployee AND Date = Moves.ShiftDate)) as MoveShiftName, (@SELECT# ShortName FROM Shifts WHERE ID IN(@SELECT# IDShiftUsed FROM DailySchedule Where IDEmployee = Moves.IDEmployee AND Date = Moves.ShiftDate)) as MoveShiftShortName " &
                    " , (@SELECT# Shifts.ExpectedWorkingHours FROM Shifts WHERE ID IN(@SELECT# IDShiftUsed FROM DailySchedule Where IDEmployee = Moves.IDEmployee AND Date = Moves.ShiftDate)) as MoveShiftExpectedWorkingHours " &
                    " FROM Moves " &
                    " WHERE " &
                    " IDEmployee = " & lngIDEmployee.ToString &
                    " AND ShiftDate >= " & Any2Time(dtBeginDate).SQLSmallDateTime &
                    " AND ShiftDate <= " & Any2Time(dtEndDate).SQLSmallDateTime &
                    " AND InDateTime is not null and OutDatetime is not null Order by ShiftDate, isnull(InDateTime, OutDateTime), isnull(OutDateTime, InDateTime), ID"

            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada movimiento
                    ElementGroupCollection = New roCollection

                    ElementGroupCollection.Add("MoveShiftDate", Format(oRowAux("ShiftDate"), "yyyy/MM/dd"))
                    ElementGroupCollection.Add("MoveShiftName", oRowAux("MoveShiftName").ToString)
                    ElementGroupCollection.Add("MoveShiftShortName", oRowAux("MoveShiftShortName").ToString)
                    ElementGroupCollection.Add("MoveShiftExpectedWorkingHours", oRowAux("MoveShiftExpectedWorkingHours").ToString.Replace(",", "."))
                    ElementGroupCollection.Add("MoveInDateTime", Format(oRowAux("InDateTime"), "yyyy/MM/dd HH:mm"))
                    ElementGroupCollection.Add("MoveInIDTerminal", oRowAux("InIDReader").ToString)
                    ElementGroupCollection.Add("MoveInCauseCode", oRowAux("InCauseCode").ToString)
                    ElementGroupCollection.Add("MoveOutDateTime", Format(oRowAux("OutDateTime"), "yyyy/MM/dd HH:mm"))
                    ElementGroupCollection.Add("MoveOutIDTerminal", oRowAux("OutIDReader").ToString)
                    ElementGroupCollection.Add("MoveOutCauseCode", oRowAux("OutCauseCode").ToString)
                    XMLInsertElementGroup(xml_text_writer, "Move", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Sub GetDataTotalConceptsElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de los acumulados totales del periodo indicado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            tb = New DataTable("Fields")

            strSQL = "@SELECT# Concepts.Name, ShortName, Export, convert(numeric(18,6),sum(Value)) as Total  " &
                    " FROM DailyAccruals, Concepts " &
                    " WHERE Concepts.ID = DailyAccruals.IDCOncept " &
                    " AND IDEmployee = " & lngIDEmployee.ToString &
                    " AND Date >= " & Any2Time(dtBeginDate).SQLSmallDateTime &
                    " AND Date <= " & Any2Time(dtEndDate).SQLSmallDateTime &
                    " Group By  Concepts.Name, ShortName, Export Order By  Concepts.Name "

            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada Campo
                    ElementGroupCollection = New roCollection

                    ElementGroupCollection.Add("ConceptTotalName", oRowAux("Name").ToString)
                    ElementGroupCollection.Add("ConceptTotalShortName", oRowAux("ShortName").ToString)
                    ElementGroupCollection.Add("ConceptTotalExportCode", oRowAux("Export").ToString)
                    ElementGroupCollection.Add("ConceptTotalValue", oRowAux("Total").ToString.Replace(",", "."))

                    XMLInsertElementGroup(xml_text_writer, "TotalConcept", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Sub GetDataTasksElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de los acumulados de tareas del periodo indicado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            tb = New DataTable("Tasks")

            strSQL = "@SELECT# Tasks.ID, Tasks.Name, Tasks.Project, BusinessCenters.Name as Center, DailyTaskAccruals.Date, DailyTaskAccruals.IDPart, DailyTaskAccruals.Value, DailyTaskAccruals.Field1 , DailyTaskAccruals.Field2 , DailyTaskAccruals.Field3, DailyTaskAccruals.Field4, DailyTaskAccruals.Field5, DailyTaskAccruals.Field6, Tasks.BarCode  " &
                    " FROM DailyTaskAccruals, Tasks , BusinessCenters " &
                    " WHERE Tasks.ID = DailyTaskAccruals.IDTask " &
                    " AND Tasks.IDCenter =  BusinessCenters.ID " &
                    " AND IDEmployee = " & lngIDEmployee.ToString &
                    " AND Date >= " & Any2Time(dtBeginDate).SQLSmallDateTime &
                    " AND Date <= " & Any2Time(dtEndDate).SQLSmallDateTime & " Order by Date, IDPart "

            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada Campo
                    ElementGroupCollection = New roCollection

                    ElementGroupCollection.Add("TaskID", oRowAux("ID").ToString)
                    ElementGroupCollection.Add("TaskCenter", oRowAux("Center").ToString)
                    ElementGroupCollection.Add("TaskProject", oRowAux("Project").ToString)
                    ElementGroupCollection.Add("TaskName", oRowAux("Name").ToString)
                    ElementGroupCollection.Add("TaskDate", Format(oRowAux("Date"), "yyyy/MM/dd"))
                    ElementGroupCollection.Add("TaskPart", oRowAux("IDPart").ToString)
                    ElementGroupCollection.Add("TaskDateValue", oRowAux("Value").ToString.Replace(",", "."))
                    ElementGroupCollection.Add("TaskDateAtribute1", oRowAux("Field1").ToString)
                    ElementGroupCollection.Add("TaskDateAtribute2", oRowAux("Field2").ToString)
                    ElementGroupCollection.Add("TaskDateAtribute3", oRowAux("Field3").ToString)
                    ElementGroupCollection.Add("TaskDateAtribute4", oRowAux("Field4").ToString.Replace(",", "."))
                    ElementGroupCollection.Add("TaskDateAtribute5", oRowAux("Field5").ToString.Replace(",", "."))
                    ElementGroupCollection.Add("TaskDateAtribute6", oRowAux("Field6").ToString.Replace(",", "."))
                    ElementGroupCollection.Add("TaskBarCode", oRowAux("BarCode").ToString)

                    XMLInsertElementGroup(xml_text_writer, "Task", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Sub GetDataProgrammedAbsencesElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de los movimentos del periodo indicado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            tb = New DataTable("ProgrammedAbsences")

            strSQL = "@SELECT# dbo.EmployeeContracts.IDEmployee, dbo.ProgrammedAbsences.BeginDate, dbo.ProgrammedAbsences.FinishDate, dbo.ProgrammedAbsences.IDCause, " &
                     "dbo.Causes.Name , dbo.Causes.Description, dbo.Causes.ShortName, MaxLastingDays " &
                     "FROM dbo.EmployeeContracts INNER JOIN dbo.ProgrammedAbsences ON dbo.EmployeeContracts.IDEmployee = dbo.ProgrammedAbsences.IDEmployee INNER JOIN " &
                     "dbo.Causes ON dbo.ProgrammedAbsences.IDCause = dbo.Causes.ID " &
                     "WHERE " &
                     " ProgrammedAbsences.IDEmployee = " & lngIDEmployee.ToString &
                     " and (dbo.ProgrammedAbsences.FinishDate >=" & Any2Time(dtBeginDate).SQLSmallDateTime &
                     " and (dbo.ProgrammedAbsences.BeginDate <= " & Any2Time(dtEndDate).SQLSmallDateTime &
                     "  OR dbo.ProgrammedAbsences.FinishDate IS NULL)) " &
                     " Order by ProgrammedAbsences.BeginDate"
            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada movimiento
                    ElementGroupCollection = New roCollection
                    ElementGroupCollection.Add("ProgAbsBeginDate", Format(oRowAux("BeginDate"), "yyyy/MM/dd"))
                    If Not IsDBNull(oRowAux("FinishDate")) Then
                        ElementGroupCollection.Add("ProgAbsFinishDate", Format(oRowAux("FinishDate"), "yyyy/MM/dd"))
                    Else
                        ElementGroupCollection.Add("ProgAbsFinishDate", "")
                    End If

                    ElementGroupCollection.Add("ProgAbsCauseName", oRowAux("Name").ToString)
                    ElementGroupCollection.Add("ProgAbsCauseDescription", oRowAux("Description").ToString)
                    ElementGroupCollection.Add("ProgAbsCauseShortName", oRowAux("ShortName").ToString)
                    ElementGroupCollection.Add("ProgAbsMaxLastingDays", oRowAux("MaxLastingDays").ToString)
                    XMLInsertElementGroup(xml_text_writer, "ProgrammedAbsences", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Sub GetDataConceptsElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de los acumulados del periodo indicado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            tb = New DataTable("Fields")

            strSQL = "@SELECT# Concepts.Name, ShortName, Description, Export, Date, Value, CarryOver, StartupValue  " &
                    " FROM DailyAccruals, Concepts " &
                    " WHERE Concepts.ID = DailyAccruals.IDCOncept " &
                    " AND IDEmployee = " & lngIDEmployee.ToString &
                    " AND Date >= " & Any2Time(dtBeginDate).SQLSmallDateTime &
                    " AND Date <= " & Any2Time(dtEndDate).SQLSmallDateTime & " Order by Date"

            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada Campo
                    ElementGroupCollection = New roCollection

                    ElementGroupCollection.Add("ConceptDate", Format(oRowAux("Date"), "yyyy/MM/dd"))
                    ElementGroupCollection.Add("ConceptName", oRowAux("Name").ToString)
                    ElementGroupCollection.Add("ConceptShortName", oRowAux("ShortName").ToString)
                    ElementGroupCollection.Add("ConceptExportCode", oRowAux("Export").ToString)
                    ElementGroupCollection.Add("ConceptDateValue", oRowAux("Value").ToString.Replace(",", "."))
                    ElementGroupCollection.Add("ConceptRuleValue", oRowAux("CarryOver").ToString)
                    ElementGroupCollection.Add("ConceptStartUpValue", oRowAux("StartupValue").ToString)
                    ElementGroupCollection.Add("Description", oRowAux("Description").ToString)

                    XMLInsertElementGroup(xml_text_writer, "Concept", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Sub GetDataFieldsElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal ExportUsrFieldName As Boolean)
            '
            ' Obtenemos los datos de las campos del empleado
            '
            Dim tb As DataTable
            Dim strSQL As String = ""
            Dim i As Integer = 1

            tb = New DataTable("Fields")

            'strSQL = "@SELECT# sysroUserFields.FieldName," & _
            '            " (@SELECT# TOP 1 CONVERT(varchar(4000), [Value])" & _
            '            " FROM EmployeeUserFieldValues " & _
            '            " WHERE EmployeeUserFieldValues.IDEmployee = " & lngIDEmployee.ToString & " AND " & _
            '            " EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND" & _
            '            " EmployeeUserFieldValues.Date < " & Any2Time(dtBeginDate).SQLSmallDateTime & _
            '            " ORDER BY EmployeeUserFieldValues.Date DESC) as Value ," & _
            '            " ISNULL((@SELECT# TOP 1 [Date]" & _
            '            " FROM EmployeeUserFieldValues " & _
            '            " WHERE EmployeeUserFieldValues.IDEmployee = " & lngIDEmployee.ToString & " AND " & _
            '            " EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND" & _
            '            " EmployeeUserFieldValues.Date < " & Any2Time(dtBeginDate).SQLSmallDateTime & _
            '            " ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120)) as date" & _
            '            " FROM sysroUserFields " & _
            '            " WHERE(sysroUserFields.Type = 0 And sysroUserFields.Used = 1)" & _
            '            " UNION " & _
            '            " @SELECT# sysroUserFields.FieldName, CONVERT(varchar(4000), [Value]) as Value, [Date] as Date" & _
            '            " FROM EmployeeUserFieldValues, sysroUserFields " & _
            '            " WHERE EmployeeUserFieldValues.IDEmployee = " & lngIDEmployee.ToString & " AND " & _
            '            " EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND" & _
            '            " EmployeeUserFieldValues.Date >= " & Any2Time(dtBeginDate).SQLSmallDateTime & " and " & _
            '            " EmployeeUserFieldValues.Date <= " & Any2Time(dtEndDate).SQLSmallDateTime & " and " & _
            '            " sysroUserFields.Type = 0 And sysroUserFields.Used = 1"

            strSQL = "@SELECT# sysroUserFields.FieldName," &
                        " (@SELECT# TOP 1 CONVERT(varchar(4000), [Value])" &
                        " FROM EmployeeUserFieldValues " &
                        " WHERE EmployeeUserFieldValues.IDEmployee = " & lngIDEmployee.ToString & " AND " &
                        " EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND" &
                        " EmployeeUserFieldValues.Date <= " & Any2Time(dtEndDate).SQLSmallDateTime &
                        " ORDER BY EmployeeUserFieldValues.Date DESC) as Value ," &
                        " ISNULL((@SELECT# TOP 1 [Date]" &
                        " FROM EmployeeUserFieldValues " &
                        " WHERE EmployeeUserFieldValues.IDEmployee = " & lngIDEmployee.ToString & " AND " &
                        " EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND" &
                        " EmployeeUserFieldValues.Date <= " & Any2Time(dtEndDate).SQLSmallDateTime &
                        " ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120)) as date" &
                        " FROM sysroUserFields " &
                        " WHERE(sysroUserFields.Type = 0 And sysroUserFields.Used = 1)"

            tb = CreateDataTable(strSQL, )
            Dim strAux As String = ""
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada Campo
                    ElementGroupCollection = New roCollection

                    'ElementGroupCollection.Add("FieldValueDate", Format(oRowAux("Date"), "yyyy/MM/dd"))
                    If ExportUsrFieldName = False Then
                        ElementGroupCollection.Add("FieldName" & i.ToString, oRowAux("FieldName").ToString)
                        ElementGroupCollection.Add("FieldValue" & i.ToString, oRowAux("Value").ToString)
                        XMLInsertElementGroup(xml_text_writer, "Field" & i.ToString, ElementGroupCollection)
                    Else
                        strAux = i.ToString & "_" & oRowAux("FieldName").ToString.Replace(" ", "_")
                        strAux = strAux.Replace("+", "_")
                        strAux = strAux.Replace("/", "_")
                        strAux = strAux.Replace("(", "_")
                        strAux = strAux.Replace(")", "_")

                        ElementGroupCollection.Add("FieldName_" & strAux, oRowAux("FieldName").ToString)
                        ElementGroupCollection.Add("FieldValue" & strAux, oRowAux("Value").ToString)
                        XMLInsertElementGroup(xml_text_writer, "Field_" & strAux, ElementGroupCollection)
                    End If
                    i = i + 1

                Next
            End If

        End Sub

        Private Sub GetDataCompanyFieldsElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de las campos del empleado
            '
            Dim tb As DataTable
            Dim strSQL As String = ""
            Dim i As Integer = 1
            Dim n As Integer

            tb = New DataTable("CompanyFields")

            strSQL = "@SELECT# * " &
                     " FROM Groups " &
                     " where Path='1' "

            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For n = 0 To tb.Columns.Count - 1
                    If tb.Columns(n).ColumnName.Length > 4 AndAlso tb.Columns(n).ColumnName.Substring(0, 4) = "USR_" Then
                        ' Para cada Campo
                        ElementGroupCollection = New roCollection

                        ElementGroupCollection.Add("CompanyFieldName" & i.ToString, tb.Columns(n).ColumnName.Substring(4, tb.Columns(n).ColumnName.Length - 4).ToString)
                        ElementGroupCollection.Add("CompanyFieldValue" & i.ToString, tb.Rows(0)(n).ToString)
                        'ElementGroupCollection.Add("FieldValueDate", Format(oRowAux("Date"), "yyyy/MM/dd"))
                        XMLInsertElementGroup(xml_text_writer, "CompanyField" & i.ToString, ElementGroupCollection)
                        i += 1
                    End If
                Next n

            End If

        End Sub

        Private Sub GetDataMobilitiesElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de las movilidades del empleado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            tb = New DataTable("Contracts")

            strSQL = "@SELECT#  GroupName, BeginDate, EndDate, FullGroupName, Path " &
                       "FROM Employees " &
                                "INNER JOIN sysroEmployeeGroups " &
                                "ON Employees.ID = sysroEmployeeGroups.IDEmployee " &
                                "WHERE Employees.ID = " & lngIDEmployee.ToString &
                                " And ((" & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " >= sysroEmployeeGroups.BeginDate" &
                                " And " & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " <= sysroEmployeeGroups.EndDate)" &
                                " Or (" & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " >= sysroEmployeeGroups.BeginDate" &
                                " And " & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " <= sysroEmployeeGroups.EndDate)" &
                                " Or (" & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " <= sysroEmployeeGroups.BeginDate" &
                                " And " & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " >= sysroEmployeeGroups.BeginDate))"

            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada Movilidad
                    ElementGroupCollection = New roCollection

                    ElementGroupCollection.Add("GroupName", oRowAux("GroupName").ToString)
                    ElementGroupCollection.Add("MobilityBeginDate", Format(oRowAux("BeginDate"), "yyyy/MM/dd"))
                    ElementGroupCollection.Add("MobilityEndDate", Format(oRowAux("EndDate"), "yyyy/MM/dd"))
                    ElementGroupCollection.Add("FullGroupName", oRowAux("FullGroupName").ToString)
                    ElementGroupCollection.Add("Path", oRowAux("Path").ToString)
                    XMLInsertElementGroup(xml_text_writer, "Mobility", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Sub GetDataContractsElement(ByVal xml_text_writer As XmlTextWriter, ByRef ElementGroupCollection As roCollection, ByVal lngIDEmployee As Long, ByVal dtBeginDate As Date, ByVal dtEndDate As Date)
            '
            ' Obtenemos los datos de los contratos del empleado
            '

            Dim tb As DataTable
            Dim strSQL As String = ""
            tb = New DataTable("Contracts")

            'strSQL = "@SELECT#  sysroPassports_AuthenticationMethods.Credential AS IDCard, CardAliases.RealValue, EmployeeContracts.IDContract, EmployeeContracts.BeginDate, EmployeeContracts.EndDate " & _
            '           "FROM Employees " & _
            '                    "INNER JOIN sysroPassports " & _
            '                    "ON Employees.ID = sysroPassports.IDEmployee " & _
            '                    "INNER JOIN sysroPassports_AuthenticationMethods " & _
            '                    "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " & _
            '                    "INNER JOIN EmployeeContracts " & _
            '                    "ON Employees.ID = EmployeeContracts.IDEmployee " & _
            '                    "LEFT JOIN CardAliases ON sysroPassports_AuthenticationMethods.Credential = CardAliases.IDCard " & _
            '           "WHERE sysroPassports_AuthenticationMethods.Method = 3 AND " & _
            '                 "sysroPassports_AuthenticationMethods.Version = '' AND " & _
            '                 "sysroPassports_AuthenticationMethods.BiometricID = 0 AND " & _
            '                 "sysroPassports_AuthenticationMethods.Enabled = 1 AND " & _
            '                " Employees.ID = " & lngIDEmployee.ToString & _
            '                " And ((" & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate" & _
            '                " And " & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.EndDate)" & _
            '                " Or (" & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate" & _
            '                " And " & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.EndDate)" & _
            '                " Or (" & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.BeginDate" & _
            '                " And " & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate))"

            strSQL = "@SELECT# sysroPassports_AuthenticationMethods.Credential AS IDCard, CardAliases.RealValue, EmployeeContracts.IDContract, EmployeeContracts.BeginDate, EmployeeContracts.EndDate " &
                     "FROM Employees INNER JOIN " &
                     "sysroPassports ON Employees.ID = sysroPassports.IDEmployee INNER JOIN " &
                     "EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee LEFT OUTER JOIN " &
                     "sysroPassports_AuthenticationMethods ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport AND " &
                     "3 = sysroPassports_AuthenticationMethods.Method AND '' = sysroPassports_AuthenticationMethods.Version AND  " &
                     "0 = sysroPassports_AuthenticationMethods.BiometricID AND 1 = sysroPassports_AuthenticationMethods.Enabled LEFT OUTER JOIN " &
                     "CardAliases ON sysroPassports_AuthenticationMethods.Credential = CardAliases.IDCard " &
                     "WHERE Employees.ID = " & lngIDEmployee.ToString & " AND " &
                     "((" & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate" &
                     " And " & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.EndDate)" &
                     " Or (" & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate" &
                     " And " & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.EndDate)" &
                     " Or (" & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.BeginDate" &
                     " And " & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate))"

            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRowAux As DataRow In tb.Rows
                    ' Para cada Contrato
                    ElementGroupCollection = New roCollection

                    ElementGroupCollection.Add("IDContract", "" & oRowAux("IDContract").ToString)
                    ElementGroupCollection.Add("ContractBeginDate", Format(oRowAux("BeginDate"), "yyyy/MM/dd"))
                    ElementGroupCollection.Add("ContractEndDate", Format(oRowAux("EndDate"), "yyyy/MM/dd"))
                    ElementGroupCollection.Add("IDCard", "" & oRowAux("IDCard").ToString)
                    ElementGroupCollection.Add("RealValue", "" & oRowAux("RealValue").ToString)
                    XMLInsertElementGroup(xml_text_writer, "Contract", ElementGroupCollection)
                Next
            End If

        End Sub

        Private Sub XMLInsertElementGroup(ByVal xml_text_writer As _
        XmlTextWriter, ByVal GroupName As String, ByVal ElementGroupCollection As roCollection)
            Dim i As Integer = 0

            ' Inicializamos el grupo
            xml_text_writer.WriteStartElement(GroupName)

            For i = 1 To ElementGroupCollection.Count
                xml_text_writer.WriteStartElement(ElementGroupCollection.Key(i).ToString)
                xml_text_writer.WriteString(ElementGroupCollection(i, roCollection.roSearchMode.roByIndex).ToString)
                xml_text_writer.WriteEndElement()
            Next

            xml_text_writer.WriteEndElement()

        End Sub

        Private Function GetMinEndDate(ByVal IDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date) As Date
            '
            ' verificamos que la fecha de fin de periodo este dentro de un contrato activo,
            ' de no ser buscamos un final de contrato anterior
            Dim strSQL As String = ""
            Dim retDate As Date
            Dim strDate As String = ""

            Try

                strSQL = "@SELECT#  count(*) FROM EmployeeContracts WHERE IDEmployee=" & IDEmployee.ToString & " AND BeginDate <= " & Any2Time(xEndPeriod).SQLSmallDateTime
                strSQL += " AND EndDate>=" & Any2Time(xEndPeriod).SQLSmallDateTime
                If Any2Double(ExecuteScalar(strSQL)) > 0 Then
                    retDate = xEndPeriod
                Else
                    ' Buscamos la fecha de fin de contrato anterior
                    strSQL = "@SELECT#  top 1 EndDate FROM EmployeeContracts WHERE IDEmployee=" & IDEmployee.ToString & " AND EndDate <  " & Any2Time(xEndPeriod).SQLSmallDateTime & " Order by EndDate desc"
                    strDate = Any2String(ExecuteScalar(strSQL))

                    If IsDate(strDate) Then
                        retDate = Any2Time(strDate).Value
                    Else
                        retDate = xBeginPeriod
                    End If
                End If
            Catch ex As Exception
                retDate = xEndPeriod
            End Try
            Return retDate
        End Function

        Private Function GetCompanyCode(ByVal IDEmployee As Integer) As String
            '
            ' Obtenemos el campo empresa
            '
            Dim intRet As Integer = 0
            Dim strSQL As String = ""
            Dim strCompanyCode As String = ""

            Dim DateField As Date
            DateField = New Date(1900, 1, 1)

            strSQL = "@SELECT# Value FROM EmployeeUserFieldValues WHERE IDEmployee=" & IDEmployee.ToString & " AND FieldName ='CODIGO_EMPRESA' AND Date=" & Any2Time(DateField).SQLSmallDateTime

            strCompanyCode = Any2String(ExecuteScalar(strSQL))

            Return strCompanyCode
        End Function

        Private Function GetEmployeeCode(ByVal IDEmployee As Integer) As String
            '
            ' Obtenemos el campo empresa
            '
            Dim intRet As Integer = 0
            Dim strSQL As String = ""
            Dim strEmployeeCode As String = ""

            Dim DateField As Date
            DateField = New Date(1900, 1, 1)

            strSQL = "@SELECT# Value FROM EmployeeUserFieldValues WHERE IDEmployee=" & IDEmployee.ToString & " AND FieldName ='CODIGO_EMPLEADO' AND Date=" & Any2Time(DateField).SQLSmallDateTime
            strEmployeeCode = Any2String(ExecuteScalar(strSQL))

            Return strEmployeeCode
        End Function

        Private Sub ConstructSql(ByRef eSQL As String, ByRef ExistErr As Boolean, ByVal GuideID As Integer, ByVal Employees As String, ByVal PatIndex As String, ByVal BeginDate As Date, ByVal EndDate As Date)
            '
            ' Construye la instrucción Sql a partir del formato seleccionado.
            '

            Dim bolRet As Boolean = False

            Dim DateInf As String, DateSup As String
            Dim GuideField As roGuideField
            Dim ExistMasterField As Boolean
            Dim NameFields As String = String.Empty

            Try

                ExistErr = False

                If Len(PatIndex) > 0 Then
                    PatIndex = " or " & PatIndex
                End If

                DateInf = Any2Time(BeginDate).Value
                DateSup = Any2Time(EndDate).Value

                Dim tb As New DataTable("GuideExtractionFields")
                Dim strSQL As String = "@SELECT# * FROM GuideExtractionFields WHERE IDGuide=" & GuideID.ToString & " Order by Pos"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    For Each oRow As DataRow In tb.Rows
                        'Me.strName = Any2String(oRow("Name"))
                        'Obtenemos todos la ficha del empleado
                        GuideField = New roGuideField

                        'Cargamos los datos del campo
                        GuideField.LoadFromActiveDataset(oRow, da)

                        Select Case GuideField.Data(roGuideField.PROPERTY_FIELDTYPE)
                            Case roGuideField.VALUE_FIELDTYPE_USERFIELD
                                If NameFields <> "" Then
                                    NameFields = NameFields & ", "
                                End If

                                NameFields = NameFields & "convert(nvarchar(500), Employees.[USR_" & GuideField.Data(roGuideField.PROPERTY_FIELDVALUE) & "]) as [" & GuideField.Data(roGuideField.PROPERTY_FIELDVALUE) & "] "
                            Case roGuideField.VALUE_FIELDTYPE_EMPLOYEENAME, roGuideField.VALUE_FIELDTYPE_CONTRACT, roGuideField.VALUE_FIELDTYPE_CARD, roGuideField.VALUE_FIELDTYPE_BIOMETRICID
                                ExistMasterField = True
                        End Select
                    Next

                    If Not ExistMasterField Then
                        ExistErr = True
                        Exit Sub
                    End If

                    If NameFields <> "" Then NameFields = NameFields & ","
                    If Employees.EndsWith(",") Then Employees = Employees.Substring(0, Employees.Length - 1)
                    eSQL = "@SELECT# " & NameFields
                    eSQL = eSQL & " Employees.ID as EmployeesID, Employees.Name as EmployeesName, BiometricID, "
                    eSQL = eSQL & " IDContract , BeginDate, EndDate, IDCard "
                    eSQL = eSQL & " From EmployeeContracts, Employees"
                    eSQL = eSQL & " Where EmployeeContracts.IDEmployee = Employees.ID"
                    eSQL = eSQL & " And ((" & Any2Time(Any2Time(DateInf).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate"
                    eSQL = eSQL & " And " & Any2Time(Any2Time(DateInf).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.EndDate)"
                    eSQL = eSQL & " Or (" & Any2Time(Any2Time(DateSup).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate"
                    eSQL = eSQL & " And " & Any2Time(Any2Time(DateSup).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.EndDate)"
                    eSQL = eSQL & " Or (" & Any2Time(Any2Time(DateInf).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.BeginDate"
                    eSQL = eSQL & " And " & Any2Time(Any2Time(DateSup).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate))"
                    eSQL = eSQL & " And Employees.ID IN(@SELECT# idemployee from sysrovwCurrentEmployeeGroups where IDEmployee IN (" & Employees & "))"
                    'eSQL = eSQL & " And  SUBSTRING(SecurityFlags," & gSession(roVarUserGroup) & ", 1) > 0 )"
                    eSQL = eSQL & " Order by Employees.Name"

                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLink::ConstructSql")
            End Try

        End Sub

        Private Sub GenerateLinesAccruals(ByVal m_Guide As roGuide, ByVal oRow As DataRow, ByRef row As Double, ByVal BeginDate As String, ByVal EndDate As String, ByVal GuideType As roGuide.eGuideType, ByVal sw As StreamWriter)
            Dim X As Integer
            Dim GuideField As roGuideField
            Dim Value As String = String.Empty
            Dim Pos As String
            Dim IsLogicClass As Boolean
            Dim i As Integer
            Dim auxPath As String
            Dim Path As String = String.Empty
            Dim ConceptValue As Boolean
            Dim ConceptValueField As roGuideField = Nothing
            Dim PercentageField As roGuideField = Nothing
            Dim ConceptDate As Boolean
            Dim ConceptDateField As roGuideField = Nothing
            Dim ConceptExportField As roGuideField = Nothing
            Dim ConceptShortNameField As roGuideField = Nothing
            Dim ConceptNameField As roGuideField = Nothing
            Dim AccrualType As String
            Dim TmpLine As String

            Try
                X = 1

                ConceptValue = False
                ConceptDate = False

                'Cuando sea la guia LogicClass no pintamos el codigo de empresa dentro de cada uno de los empleados
                Dim strSQL As String = """"
                Dim tb As DataTable = Nothing
                'If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                '    GuideName = Any2String(tb.Rows(0).Item(0))
                'End If

                'If InStr(UCase(GuideName), "LOGICCLASS") Then
                '    IsLogicClass = True
                'End If

                'Obtenemos todos los campos de la guia de extraccion
                TmpLine = ""
                tb = New DataTable("GuideExtractionFields")
                strSQL = "@SELECT# * FROM GuideExtractionFields WHERE IDGuide=" & m_Guide.ID & " Order by Pos"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRowAux As DataRow In tb.Rows
                        GuideField = New roGuideField
                        'Cargamos los datos del campo
                        GuideField.LoadFromActiveDataset(oRowAux, da)

                        'Añadimos al registro el valor correspondiente
                        Select Case GuideField.Data(roGuideField.PROPERTY_FIELDTYPE)
                            Case roGuideField.VALUE_FIELDTYPE_LITERAL
                                PrintFile(Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDVALUE)), sw, GuideField, TmpLine, GuideType)
                            Case roGuideField.VALUE_FIELDTYPE_USERFIELD
                                'Si la guia es xml de LogicClass no pintamos el campo del codigo de empresa
                                If Not IsLogicClass And GuideType <> roGuide.eGuideType.XML Then
                                    PrintFile(Any2String(oRow(GuideField.Data(roGuideField.PROPERTY_FIELDVALUE))), sw, GuideField, TmpLine, GuideType)
                                End If
                            Case roGuideField.VALUE_FIELDTYPE_CONTRACT
                                PrintFile(Any2String(oRow("IDContract")), sw, GuideField, TmpLine, GuideType)
                            Case roGuideField.VALUE_FIELDTYPE_CARD
                                PrintFile(Any2String(oRow("IDCard")), sw, GuideField, TmpLine, GuideType)
                            Case roGuideField.VALUE_FIELDTYPE_BIOMETRICID
                                PrintFile(Any2String(oRow("BiometricID")), sw, GuideField, TmpLine, GuideType)
                            Case roGuideField.VALUE_FIELDTYPE_EMPLOYEENAME
                                PrintFile(Any2String(oRow("EmployeesName")), sw, GuideField, TmpLine, GuideType)
                            Case roGuideField.VALUE_FIELDTYPE_GROUP
                                Pos = Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDVALUE))
                                Dim tbaux As DataTable = CreateDataTable("@SELECT# Path From sysrovwCurrentEmployeeGroups where idEmployee = " & oRow("EmployeesID"))
                                If tbaux IsNot Nothing AndAlso tbaux.Rows.Count > 0 Then
                                    Path = Any2String(tbaux.Rows(0).Item(0))
                                End If

                                auxPath = ""
                                For i = 0 To Pos
                                    If Len(auxPath) = 0 Then
                                        auxPath = String2Item(Path, i, "\")
                                    Else
                                        auxPath = auxPath & "\" & String2Item(Path, i, "\")
                                    End If
                                Next i
                                tbaux = CreateDataTable("@SELECT# Name From Groups where Path like '" & auxPath & "'")
                                If tbaux IsNot Nothing AndAlso tbaux.Rows.Count > 0 Then
                                    Value = Any2String(tbaux.Rows(0).Item(0))
                                End If

                                PrintFile(Value, sw, GuideField, TmpLine, GuideType)
                            Case roGuideField.VALUE_FIELDTYPE_ACCESS
                                Dim tbaux = CreateDataTable("@SELECT# count(*) from Punches where Type IN(" & Any2Double(PunchTypeEnum._AV) & "," & Any2Double(PunchTypeEnum._L) & ") " & " AND (IDZone = " & Any2Double(GuideField.Data(roGuideField.PROPERTY_FIELDVALUE)) & " or IDTerminal = 99 ) and IDEmployee= " & oRow("EmployeesID") & " and DateTime >=" & Any2Time(BeginDate).SQLDateTime & " and DateTime <" & Any2Time(EndDate).Add(1, "d").SQLDateTime & " and DateTime >=" & Any2Time(oRow("BeginDate")).SQLDateTime & " and DateTime <" & Any2Time(oRow("EndDate")).Add(1, "d").SQLDateTime)
                                If tbaux IsNot Nothing AndAlso tbaux.Rows.Count > 0 Then
                                    Value = Any2Double(tbaux.Rows(0).Item(0))
                                End If

                                PrintFile(Value, sw, GuideField, TmpLine, GuideType)
                            Case roGuideField.VALUE_FIELDTYPE_BLANKS
                                PrintFile("", sw, GuideField, TmpLine, GuideType)
                            Case roGuideField.VALUE_FIELDTYPE_CONCEPTDATE
                                ConceptDate = True
                                ConceptDateField = New roGuideField
                                ConceptDateField = GuideField
                                TmpLine = TmpLine + "VALUE_FIELDTYPE_CONCEPTDATE"
                            Case roGuideField.VALUE_FIELDTYPE_BEGINCONTRACT
                                Value = Format$(Any2Time(Any2String(oRow("BeginDate"))).DateOnly, Trim(GuideField.Data(roGuideField.PROPERTY_FIELDDATEFORMAT)))

                                PrintFile(Value, sw, GuideField, TmpLine, GuideType)

                            Case roGuideField.VALUE_FIELDTYPE_ENDCONTRACT

                                Value = Format$(Any2Time(Any2String(oRow("EndDate"))).DateOnly, Trim(GuideField.Data(roGuideField.PROPERTY_FIELDDATEFORMAT)))

                                If Any2Time(Value).Value = Any2Time(roNullDate).Value Then
                                    PrintFile("", sw, GuideField, TmpLine, GuideType)
                                Else
                                    PrintFile(Value, sw, GuideField, TmpLine, GuideType)
                                End If
                            Case roGuideField.VALUE_FIELDTYPE_CONCEPTNAME
                                ConceptNameField = New roGuideField
                                ConceptNameField = GuideField
                                TmpLine = TmpLine + "VALUE_FIELDTYPE_CONCEPTNAME"
                            Case roGuideField.VALUE_FIELDTYPE_CONCEPTSHORTNAME
                                ConceptShortNameField = New roGuideField
                                ConceptShortNameField = GuideField
                                TmpLine = TmpLine + "VALUE_FIELDTYPE_CONCEPTSHORTNAME"

                            Case roGuideField.VALUE_FIELDTYPE_CONCEPTEXPORT
                                ConceptExportField = New roGuideField
                                ConceptExportField = GuideField
                                TmpLine = TmpLine + "VALUE_FIELDTYPE_CONCEPTEXPORT"

                            Case roGuideField.VALUE_FIELDTYPE_CONCEPTVALUE
                                ConceptValue = True
                                ConceptValueField = New roGuideField
                                ConceptValueField = GuideField
                                TmpLine = TmpLine + "VALUE_FIELDTYPE_CONCEPTVALUE"
                            Case roGuideField.VALUE_FIELDTYPE_BEGINPERIOD
                                Value = ""
                                Value = Format$(Any2Time(BeginDate).DateOnly, Trim(GuideField.Data(roGuideField.PROPERTY_FIELDDATEFORMAT)))
                                PrintFile(Value, sw, GuideField, TmpLine, GuideType)
                            Case roGuideField.VALUE_FIELDTYPE_ENDPERIOD
                                Value = ""
                                Value = Format$(Any2Time(EndDate).DateOnly, Trim(GuideField.Data(roGuideField.PROPERTY_FIELDDATEFORMAT)))
                                PrintFile(Value, sw, GuideField, TmpLine, GuideType)
                        End Select

                        X = X + 1

                    Next

                    AccrualType = ""
                    If ConceptDate And ConceptValue Then
                        'Hay que crear un registro por cada dia del acumulado
                        AccrualType = "BYDAY"
                    Else
                        'Hay que indicar solo el total de acumulado en esta linea
                        AccrualType = "BYPERIOD"
                    End If

                    'Creamos las lineas de acumulados para cada dia o para el total del periodo
                    GenerateTotalAccrualLine(oRow, BeginDate, EndDate, m_Guide, sw, TmpLine, ConceptValueField, ConceptExportField, ConceptShortNameField, ConceptNameField, ConceptDateField, AccrualType, PercentageField, GuideType)

                    Exit Sub
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGuide::GenerateLinesAccruals")
                Exit Sub
            End Try

        End Sub

        Private Sub GenerateTotalAccrualLine(ByVal oRow As DataRow, ByVal BeginDate As String, ByVal EndDate As String, ByVal m_Guide As roGuide, ByVal sw As StreamWriter, ByVal TmpLine As String, ByVal ConceptValueField As roGuideField, ByVal ConceptExportField As roGuideField, ByVal ConceptShortNameField As roGuideField, ByVal ConceptNameField As roGuideField, ByVal ConceptDateField As roGuideField, ByVal AccrualType As String, ByVal PercentageField As roGuideField, ByVal GuideType As String)
            '
            ' Creamos una linea para cada acumulado con su total
            '
            Dim Value As String
            Dim NewValue As String = String.Empty
            Dim i As Integer
            Dim mType As Double
            Dim eSQL As String
            Dim GuideField As roGuideField
            Dim OriginalLine As String
            Dim DifZero As String

            Try

                mType = Any2Double(m_Guide.Data(roGuideField.PROPERTY_TYPESELECCTIONCONCEPTS))
                DifZero = Any2Double(m_Guide.Data(roGuideField.PROPERTY_ZEROVALUE))
                OriginalLine = TmpLine

                If AccrualType = "BYPERIOD" Then
                    eSQL = "@SELECT# IDConcept , convert(numeric(18,6), sum(Value)) as total , ShortName, Name, Export "
                Else
                    eSQL = "@SELECT# IDConcept , convert(numeric(18,6), sum(Value)) as total , ShortName, Name, Export, Date "
                End If
                eSQL = eSQL & " From DailyAccruals, Concepts "
                eSQL = eSQL & " Where IDEmployee= " & oRow("EmployeesID")
                eSQL = eSQL & " and Concepts.ID = DailyAccruals.IDCOncept"

                Select Case mType
                    Case 2 : eSQL = eSQL & " And DailyAccruals.IDCOncept In(" & m_Guide.Data(roGuideField.PROPERTY_CONCEPTS) & ")"
                    Case 1 : eSQL = eSQL & " And Concepts.Export is not null and Concepts.Export <> '0' "
                End Select

                Select Case DifZero
                    Case roGuideField.VALUE_CONCEPT_NOTZEROVALUE : eSQL = eSQL & " And value <> 0 "
                End Select

                eSQL = eSQL & " and Date >=" & Any2Time(BeginDate).SQLSmallDateTime & " and Date <=" & Any2Time(EndDate).SQLSmallDateTime & " and Date >=" & Any2Time(oRow("BeginDate")).SQLSmallDateTime & " and Date <=" & Any2Time(oRow("EndDate")).SQLSmallDateTime

                If AccrualType = "BYPERIOD" Then
                    eSQL = eSQL & " Group By IDConcept, ShortName, Name, Export"
                    eSQL = eSQL & " Order By IDConcept"
                Else
                    eSQL = eSQL & " Group By IDConcept, ShortName, Name, Export, Date"
                    eSQL = eSQL & " Order By IDConcept, Date"
                End If

                Dim tb As DataTable = CreateDataTable(eSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRowAux As DataRow In tb.Rows

                        TmpLine = OriginalLine
                        Value = Any2String(oRowAux("Total"))

                        If m_Guide.Data(roGuideField.PROPERTY_FORMATCONCEPT) = roGuideField.VALUE_FORMATCONCEPT_SEXA Then
                            Value = roConversions.ConvertHoursToTime(oRowAux("Total"))
                        End If

                        FormatDecimals(CDbl(Value), NewValue, m_Guide, ConceptValueField)

                        For i = 1 To 6
                            GuideField = New roGuideField
                            Select Case i
                                Case 1 : Value = "" : GuideField = Nothing
                                Case 2 : Value = Any2String(CInt(oRowAux("Export"))) : GuideField = ConceptExportField
                                Case 3 : Value = Any2String(oRowAux("ShortName")) : GuideField = ConceptShortNameField
                                Case 4 : Value = Any2String(oRowAux("Name")) : GuideField = ConceptNameField
                                Case 5
                                    GuideField = ConceptDateField
                                    If Not GuideField Is Nothing Then
                                        Value = Any2String(oRowAux("Date"))
                                        Value = Format$(Any2Time(Value).DateOnly, Trim(GuideField.Data(roGuideField.PROPERTY_FIELDDATEFORMAT)))
                                    End If

                                Case 6 : Value = NewValue : GuideField = ConceptValueField
                            End Select

                            If Not GuideField Is Nothing Then

                                'En el caso de recoger un 99999 o nada introducimos un espacio en blanco
                                If Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDPADDING)) = "99999" Or Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDPADDING)) = "" Then
                                    GuideField.Data(roGuideField.PROPERTY_FIELDPADDING) = " "
                                End If

                                If GuideField.Data(roGuideField.PROPERTY_FIELDPADDING) <> "" Then
                                    Value = Right$(StrDup(Any2Integer(GuideField.Data(roGuideField.PROPERTY_FIELDLEN)), Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDPADDING))) & Value, Any2Double(GuideField.Data(roGuideField.PROPERTY_FIELDLEN)))
                                Else
                                    Value = Left$(Value & StrDup(Any2Integer(GuideField.Data(roGuideField.PROPERTY_FIELDLEN)), Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDPADDING))), Any2Double(GuideField.Data(roGuideField.PROPERTY_FIELDLEN)))
                                End If

                                Select Case i
                                    Case 6
                                        TmpLine = Strings.Replace(TmpLine, "VALUE_FIELDTYPE_CONCEPTVALUE", Value)
                                    Case 2
                                        TmpLine = Strings.Replace(TmpLine, "VALUE_FIELDTYPE_CONCEPTEXPORT", Value)
                                    Case 3
                                        TmpLine = Strings.Replace(TmpLine, "VALUE_FIELDTYPE_CONCEPTSHORTNAME", Value)
                                    Case 4
                                        TmpLine = Strings.Replace(TmpLine, "VALUE_FIELDTYPE_CONCEPTNAME", Value)
                                    Case 5
                                        TmpLine = Strings.Replace(TmpLine, "VALUE_FIELDTYPE_CONCEPTDATE", Value)
                                End Select
                            End If

                        Next i

                        If GuideType <> "XML" Then
                            sw.WriteLine(TmpLine)
                        End If

                    Next
                End If

                Exit Sub
            Catch

            End Try
        End Sub

        Private Function Separator(ByVal cadena As String, ByVal separador As String) As Boolean
            '
            ' Separa con el carácter "separador" los decimales, sustituyendo
            ' la coma.
            '
            Dim X As Integer

            Try

                Separator = False
                For X = 1 To Len(cadena)
                    If Mid$(cadena, X, 1) = "," Or Mid$(cadena, X, 1) = "." Then
                        Mid$(cadena, X, 1) = separador
                        Separator = True
                    End If
                Next X

                Exit Function
            Catch
                Separator = False
            End Try

        End Function

        Private Sub FormatDecimals(ByVal Valor As Double, ByRef DadaInt As String, ByVal m_Guide As roGuide, ByVal GuideField As roGuideField)
            '
            ' Formatea el valor para que salga en decimales.
            '
            Dim Negative As Boolean
            Dim NumDecimals As Integer

            Try

                If GuideField Is Nothing Then
                    Exit Sub
                End If

                DadaInt = Valor
                ' Mira cuántos decimales hay en el valor
                MirarDecimals(DadaInt, NumDecimals)
                ' Dependiendo de si el número de decimales es mayor al
                ' que se tiene que formatear, formateamos el valor (dadaint).
                If Any2Double(NumDecimals) <= Any2Double(m_Guide.Data(roGuideField.PROPERTY_LENDECIMALS)) Then
                    DadaInt = DadaInt & StrDup(Any2Integer(m_Guide.Data(roGuideField.PROPERTY_LENDECIMALS)) - Any2Integer(NumDecimals), "0")
                Else
                    DadaInt = Mid$(DadaInt, 1, Len(DadaInt) - (Any2Double(NumDecimals) - Any2Double(m_Guide.Data(roGuideField.PROPERTY_LENDECIMALS))))
                End If

                ' Activamos el carácter separador de decimales.
                If Not IsNothing(m_Guide.Data(roGuideField.PROPERTY_DECIMALSYMBOL)) Or m_Guide.Data(roGuideField.PROPERTY_LENDECIMALS) = "" Then
                    If Not Separator(DadaInt, m_Guide.Data(roGuideField.PROPERTY_DECIMALSYMBOL)) Then
                        DadaInt = Left$(DadaInt, Len(DadaInt) - m_Guide.Data(roGuideField.PROPERTY_LENDECIMALS)) & m_Guide.Data(roGuideField.PROPERTY_DECIMALSYMBOL) & Right$(DadaInt, m_Guide.Data(roGuideField.PROPERTY_LENDECIMALS))
                    Else
                        DadaInt = Left$(DadaInt, Len(DadaInt) - m_Guide.Data(roGuideField.PROPERTY_LENDECIMALS) - 1) & m_Guide.Data(roGuideField.PROPERTY_DECIMALSYMBOL) & Right$(DadaInt, m_Guide.Data(roGuideField.PROPERTY_LENDECIMALS))
                    End If
                End If
                ' Formateamos el padding.
                If Val(DadaInt) < 0 Then
                    DadaInt = Mid$(DadaInt, 2, Len(DadaInt) - 1)
                    Negative = True
                Else
                    Negative = False
                End If

                'En el caso de que sea 99999 el padding es el espacio en blanco
                If Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDPADDING)) = "99999" Then
                    GuideField.Data(roGuideField.PROPERTY_FIELDPADDING) = " "
                End If

                If GuideField.Data(roGuideField.PROPERTY_FIELDPADDING) <> "" Then
                    DadaInt = Right$(StrDup(Any2Integer(GuideField.Data(roGuideField.PROPERTY_FIELDLEN)), Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDPADDING))) & DadaInt, Any2Double(GuideField.Data(roGuideField.PROPERTY_FIELDLEN)))
                End If
                If Negative = True Then
                    Mid$(DadaInt, 1, 1) = "-"
                End If

                Exit Sub
            Catch

            End Try

        End Sub

        Private Sub MirarDecimals(ByVal Numero As String, ByRef Cantidad As Integer)
            '
            ' Mira cuántos decimales tiene el número (lo devuelve en "Cantidad").
            '

            Dim X As Integer
            Dim InitCompt As Integer

            Try

                InitCompt = False
                Cantidad = 0
                For X = 1 To Len(Numero)
                    If InitCompt Then
                        Cantidad = Cantidad + 1
                    End If
                    If Mid$(Numero, X, 1) = "," OrElse Mid$(Numero, X, 1) = "." Then
                        InitCompt = True
                    End If
                Next X

                Return
            Catch

            End Try

        End Sub

        Private Sub PrintFile(ByVal Valor As String, ByVal sw As StreamWriter, ByVal GuideField As roGuideField, ByRef TmpLine As String, ByVal GuideType As roGuide.eGuideType)
            '
            ' Imprime en el fichero ASCII.
            '
            Try

                'En el caso de que el paddings sea 99999 o no tenga le introducimos un espacio en blanco
                If Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDPADDING)) = "99999" OrElse Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDPADDING)) = "" Then
                    GuideField.Data(roGuideField.PROPERTY_FIELDPADDING) = " "
                End If

                'En el caso de que sea numerico se pintan por la derecha y las cadenas por la izquierda
                If IsNumeric(Valor) Then
                    If GuideType <> roGuide.eGuideType.XML Then
                        TmpLine = TmpLine + Right$(StrDup(Any2Integer(GuideField.Data(roGuideField.PROPERTY_FIELDLEN)), Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDPADDING))) & Valor, Any2Double(GuideField.Data(roGuideField.PROPERTY_FIELDLEN)))
                    End If
                Else
                    If GuideType <> roGuide.eGuideType.XML Then
                        TmpLine = TmpLine + Left$(Valor & StrDup(Any2Integer(GuideField.Data(roGuideField.PROPERTY_FIELDLEN)), Any2String(GuideField.Data(roGuideField.PROPERTY_FIELDPADDING))), Any2Double(GuideField.Data(roGuideField.PROPERTY_FIELDLEN)))
                    End If
                End If
                Return
            Catch
                'dp nothing
            End Try

        End Sub


        Public Sub SaveDefaultDefinition(ByVal PositionedDataset As DataRow, ByVal ad As DbDataAdapter)
            Dim mData As roCollection

            mData = New roCollection

            mData.Add(roGuideField.PROPERTY_LENDECIMALS, "2")
            mData.Add(roGuideField.PROPERTY_HEADER, roGuideField.VALUE_HEADER_VIEW)
            mData.Add(roGuideField.PROPERTY_FORMATCONCEPT, roGuideField.VALUE_FORMATCONCEPT_DECIMAL)
            mData.Add(roGuideField.PROPERTY_NAMECONCEPT, roGuideField.VALUE_NAMECONCEPT_NAMECONCEPT)
            mData.Add(roGuideField.PROPERTY_DECIMALSYMBOL, ",")
            mData.Add(roGuideField.PROPERTY_CONCEPTS, ",")
            mData.Add(roGuideField.PROPERTY_TYPESELECCTIONCONCEPTS, "")
            mData.Add(roGuideField.PROPERTY_CONCEPTVALUE, roGuideField.VALUE_CONCEPT_PERIOD)
            mData.Add(roGuideField.PROPERTY_CONCEPTVALUE, roGuideField.VALUE_CONCEPT_PERIOD)
            mData.Add(roGuideField.PROPERTY_ZEROVALUE, roGuideField.VALUE_CONCEPT_NOTZEROVALUE)

            PositionedDataset("Definition") = mData.XML
            Try
                ad.Update(PositionedDataset.Table)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGuide::SaveDefaultDefinition")
            End Try

        End Sub

        Private Function ISAccrualsGuide(ByVal m_Guide As roGuide) As Boolean
            Dim GuideField As roGuideField

            ISAccrualsGuide = False
            Try
                'Obtenemos todos los campos de la guia de extraccion

                Dim tb As New DataTable("GuideExtractionFields")
                Dim strSQL As String = "@SELECT# * FROM GuideExtractionFields WHERE IDGuide=" & m_Guide.ID & " Order by Pos"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        GuideField = New roGuideField

                        'Cargamos los datos del campo
                        GuideField.LoadFromActiveDataset(oRow, da)

                        Select Case GuideField.Data(roGuideField.PROPERTY_FIELDTYPE)
                            Case roGuideField.VALUE_FIELDTYPE_CONCEPTDATE, roGuideField.VALUE_FIELDTYPE_CONCEPTNAME, roGuideField.VALUE_FIELDTYPE_CONCEPTSHORTNAME, roGuideField.VALUE_FIELDTYPE_CONCEPTEXPORT, roGuideField.VALUE_FIELDTYPE_CONCEPTVALUE, roGuideField.VALUE_FIELDTYPE_PERCENTAGE
                                ISAccrualsGuide = True
                                Exit Function
                        End Select
                    Next

                End If

                Exit Function
            Catch
            End Try

        End Function

        'Private Sub GenerateHeader(ByRef WorksSheet As Robotics.Excel9.Interop.Worksheet, ByVal Guide As roGuide, ByRef row As Double)
        '    Dim X As Integer
        '    Dim GuideField As roGuideField
        '    Dim strSQL As String

        '    X = 1

        '    strSQL = "@SELECT# * FROM GuideExtractionFields WHERE IDGuide=" & Guide.ID.ToString & " Order by Pos"
        '    Dim tb As New DataTable("GuideExtractionFields")
        '    Dim cmd As DbCommand = CreateCommand(strSQL)
        '    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
        '    da.Fill(tb)

        '    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

        '        For Each oRow As DataRow In tb.Rows

        '            GuideField = New roGuideField

        '            'Cargamos los datos del campo
        '            GuideField.LoadFromActiveDataset(oRow, da)

        '            If Guide.Data(roGuideField.PROPERTY_HEADER) = roGuideField.VALUE_HEADER_VIEW Then
        '                WorksSheet.Rows(row).Font.Bold = True

        '                Select Case GuideField.Data(roGuideField.PROPERTY_FIELDTYPE)
        '                    Case roGuideField.VALUE_FIELDTYPE_CONCEPT
        '                        If Guide.Data(roGuideField.PROPERTY_NAMECONCEPT) = roGuideField.VALUE_NAMECONCEPT_NAMECONCEPT Then
        '                            WorksSheet.Cells(1, X) = Any2String(ExecuteScalar("@SELECT# Name FROM CONCEPTS WHERE ID=" & GuideField.Data(roGuideField.PROPERTY_FIELDVALUE)))

        '                        ElseIf Guide.Data(roGuideField.PROPERTY_NAMECONCEPT) = roGuideField.VALUE_NAMECONCEPT_SHORTNAME Then
        '                            WorksSheet.Cells(1, X) = Any2String(ExecuteScalar("@SELECT# ShortName FROM CONCEPTS WHERE ID=" & GuideField.Data(roGuideField.PROPERTY_FIELDVALUE)))
        '                        ElseIf Guide.Data(roGuideField.PROPERTY_NAMECONCEPT) = roGuideField.VALUE_NAMECONCEPT_EXPORT Then
        '                            WorksSheet.Cells(1, X) = Any2String(ExecuteScalar("@SELECT# Export FROM CONCEPTS WHERE ID=" & GuideField.Data(roGuideField.PROPERTY_FIELDVALUE)))
        '                        End If
        '                    Case Else
        '                        WorksSheet.Cells(1, X) = GuideField.Data(roGuideField.PROPERTY_FIELDNAMEHEADER)
        '                End Select

        '                WorksSheet.Columns(X).ColumnWidth = Len(WorksSheet.Cells(1, X).ToString) + 2

        '                If WorksSheet.Columns(X).ColumnWidth = 0 Then WorksSheet.Columns(X).ColumnWidth = 13

        '            Else
        '                WorksSheet.Columns(X).ColumnWidth = 13
        '            End If

        '            X = X + 1
        '        Next

        '        If Guide.Data(roGuideField.PROPERTY_HEADER) = roGuideField.VALUE_HEADER_VIEW Then
        '            row = 2
        '        End If
        '    End If

        'End Sub

        Public Function GetTmpFilePath(ByVal intIDGuide As Integer, ByVal oFileFormat As roGuide.eGuideType) As String

            Dim oSettings As New roSettings

            Dim strPath As String = oSettings.GetVTSetting(eKeys.Reports)
            Dim strPrefix As String = "##" & intIDGuide.ToString
            If Me.oState.IDPassport >= 0 Then strPrefix &= "#" & Me.oState.IDPassport.ToString
            Dim strExtension As String = "txt"
            Select Case oFileFormat
                Case roGuide.eGuideType.ASCII
                    strExtension = "txt"
                Case roGuide.eGuideType.EXCEL
                    strExtension = "xls"
                Case roGuide.eGuideType.XML
                    strExtension = "xml"
            End Select
            Dim Files() As String = Directory.GetFiles(strPath, strPrefix & "_*." & strExtension)
            Dim intIndex As Integer = -1
            Dim i As Integer
            For Each strFile As String In Files
                i = CInt(strFile.Split("_")(1).Split(".")(0))
                If i > intIndex Then
                    intIndex = i
                End If
            Next
            intIndex += 1

            ' Obtener nombre del fichero temporal para generar la exportación
            Return Path.Combine(strPath, strPrefix & "_" & intIndex.ToString & "." & strExtension)

        End Function

        Private Function GetFileBytes(ByVal strFileName As String) As Byte()

            Dim bRet() As Byte = Nothing

            Dim objFileStream As FileStream = Nothing
            Try

                'Dim iBinaryLen As Long
                'iBinaryLen = 1024
                objFileStream = New FileStream(strFileName, FileMode.Open, FileAccess.Read)

                ReDim bRet(objFileStream.Length - 1)
                objFileStream.Read(bRet, 0, bRet.Length)

                ''Dim bf As BinaryFormatter = New BinaryFormatter()
                ''Dim ms As MemoryStream = New MemoryStream()
                ''bf.Serialize(ms, bRet)
                ''Return ms.ToArray()
            Catch Ex As Exception
            Finally
                If objFileStream IsNot Nothing Then objFileStream.Close()
            End Try

            Return bRet

        End Function

#End Region

        'Private Shared Function IsVbe6Installed(ByRef oState As roDataLinkState) As Boolean
        '    Try
        '        Dim oRegKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(VbeInstallationPathKey, False)
        '        Dim oReg64Key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(VbeInstallationPath64Key, False)

        '        If oRegKey IsNot Nothing Then
        '            If oRegKey.GetValue(Vbe6InstallationPathValue) IsNot Nothing Then
        '                Dim pathToVbe As String = DirectCast(oRegKey.GetValue(Vbe6InstallationPathValue), String)
        '                If Not System.IO.File.Exists(pathToVbe) Then
        '                    Return False
        '                End If
        '            End If
        '        Else
        '            If oReg64Key IsNot Nothing Then
        '                If oReg64Key.GetValue(Vbe6InstallationPathValue) IsNot Nothing Then
        '                    Dim pathToVbe As String = DirectCast(oReg64Key.GetValue(Vbe6InstallationPathValue), String)
        '                    If Not System.IO.File.Exists(pathToVbe) Then
        '                        Return False
        '                    End If
        '                End If
        '            End If

        '        End If

        '    Catch ex As Exception
        '        oState.UpdateStateInfo(ex, "roDataLink::IsVbe6Installed:Cannot acces to windows registry")
        '    End Try

        '    Return True
        'End Function

        'Private Shared Function IsVbe7Installed(ByRef oState As roDataLinkState) As Boolean
        '    Try
        '        Dim oRegKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(VbeInstallationPathKey, False)
        '        Dim oReg64Key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(VbeInstallationPath64Key, False)

        '        If oRegKey IsNot Nothing Then
        '            If oRegKey.GetValue(Vbe7InstallationPathValue) IsNot Nothing Then
        '                Dim pathToVbe As String = DirectCast(oRegKey.GetValue(Vbe7InstallationPathValue), String)
        '                If Not System.IO.File.Exists(pathToVbe) Then
        '                    Return False
        '                End If
        '            End If
        '        Else
        '            If oReg64Key IsNot Nothing Then
        '                If oReg64Key.GetValue(Vbe7InstallationPathValue) IsNot Nothing Then
        '                    Dim pathToVbe As String = DirectCast(oReg64Key.GetValue(Vbe7InstallationPathValue), String)
        '                    If Not System.IO.File.Exists(pathToVbe) Then
        '                        Return False
        '                    End If
        '                End If
        '            End If

        '        End If

        '    Catch ex As Exception
        '        oState.UpdateStateInfo(ex, "roDataLink::IsVbe7Installed:Cannot acces to windows registry")
        '    End Try

        '    Return True
        'End Function

        'Public Shared Function CanExecuteExcelDataTemplate(ByRef oState As roDataLinkState) As Boolean
        '    Dim oExcelApp As Robotics.Excel9.Interop.Application = Nothing
        '    Try
        '        oExcelApp = New Robotics.Excel9.Interop.Application
        '        Dim version As Integer = Nothing
        '        Integer.TryParse(oExcelApp.Version.Split(".")(0), version)

        '        If (version >= 11) Then
        '            If version >= 11 And version < 14 Then
        '                Return IsVbe6Installed(oState)
        '            ElseIf version = 14 Then
        '                Return IsVbe7Installed(oState)
        '            Else
        '                Return True
        '            End If
        '        Else
        '            Return False
        '        End If
        '    Catch ex As Exception
        '        Return False
        '    Finally
        '        If oExcelApp IsNot Nothing Then oExcelApp.Quit()
        '    End Try
        'End Function

        Public Shared Function ExistsExportPeriod(ByVal BeginPeriod As Date, EndPeriod As Date, ByRef oState As roDataLinkState) As Boolean
            Dim bolRet As Boolean = False

            Dim datBegindate As Date
            Dim datEndDate As Date

            Try

                Dim strSQL As String = "@SELECT# * FROM ExportGuidePeriods "
                Dim oDatatable As DataTable = CreateDataTable(strSQL, )
                If oDatatable IsNot Nothing Then
                    For Each oDataRow In oDatatable.Rows
                        datBegindate = oDataRow("BeginDate")
                        datEndDate = oDataRow("EndDate")

                        If BeginPeriod >= datBegindate And BeginPeriod <= datEndDate Then
                            bolRet = True
                            Exit For
                        End If

                        If EndPeriod >= datBegindate And EndPeriod <= datEndDate Then
                            bolRet = True
                            Exit For
                        End If

                        If BeginPeriod <= datBegindate And EndPeriod >= datBegindate Then
                            bolRet = True
                            Exit For
                        End If

                        If BeginPeriod <= datEndDate And EndPeriod >= datEndDate Then
                            bolRet = True
                            Exit For
                        End If
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDataLink::ExistsExportPeriod")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLink::ExistsExportPeriod")
            Finally

            End Try

            Return bolRet

        End Function

        'Public Shared Function IsExcelInstalled(ByRef oState As roDataLinkState) As Boolean
        '    Dim bolRet As Boolean = False
        '    Try

        '        'http://www.devx.com/vb2themax/Tip/19507

        '        'Define the RegistryKey objects for the registry hives.
        '        Dim regClasses As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot

        '        'Check whether Microsoft Excel is installed on this computer, by searching the HKEY_CLASSES_ROOT\Excel.Application key.
        '        Dim regExcel As Microsoft.Win32.RegistryKey = regClasses.OpenSubKey("Excel.Application")

        '        If regExcel IsNot Nothing Then
        '            bolRet = True
        '            regExcel.Close()
        '        End If

        '    Catch ex As Exception
        '        oState.UpdateStateInfo(ex, "roDataLink::IsExcelInstalled")
        '    End Try

        '    Return bolRet

        'End Function

    End Class

    Public Class roGuideField

#Region "Declarations"

        Public Const PROPERTY_HEADER = "Header"
        Public Const PROPERTY_FORMATCONCEPT = "FormatConceptValue"
        Public Const PROPERTY_NAMECONCEPT = "NameConcept"
        Public Const PROPERTY_DECIMALSYMBOL = "Symbol"
        Public Const PROPERTY_LENDECIMALS = "LenDecimals"
        Public Const PROPERTY_CONCEPTS = "Concepts"
        Public Const PROPERTY_TYPESELECCTIONCONCEPTS = "TypeConcepts"
        Public Const PROPERTY_CONCEPTVALUE = "ConceptValue"
        Public Const PROPERTY_ZEROVALUE = "ConceptZero"

        Public Const VALUE_HEADER_VIEW = "1"
        Public Const VALUE_HEADER_NOTVIEW = "0"
        Public Const VALUE_FORMATCONCEPT_DECIMAL = "1"
        Public Const VALUE_FORMATCONCEPT_SEXA = "0"
        Public Const VALUE_NAMECONCEPT_NAMECONCEPT = "1"
        Public Const VALUE_NAMECONCEPT_SHORTNAME = "2"
        Public Const VALUE_NAMECONCEPT_EXPORT = "3"

        Public Const VALUE_CONCEPT_PERIOD = "1"
        Public Const VALUE_CONCEPT_DAILY = "0"
        Public Const VALUE_CONCEPT_ZEROVALUE = "0"
        Public Const VALUE_CONCEPT_NOTZEROVALUE = "1"

        Public Const PROPERTY_FIELDTYPE = "Type"
        Public Const PROPERTY_FIELDVALUE = "Value"
        Public Const PROPERTY_FIELDNAMEHEADER = "NameHeader"
        Public Const PROPERTY_FIELDLEN = "Len"
        Public Const PROPERTY_FIELDPADDING = "Padding"
        Public Const PROPERTY_FIELDDATEFORMAT = "DateFormat"
        Public Const PROPERTY_FIELDTAGXML = "TagXML"

        Public Const VALUE_FIELDTYPE_LITERAL = "1"
        Public Const VALUE_FIELDTYPE_USERFIELD = "2"
        Public Const VALUE_FIELDTYPE_CONTRACT = "3"
        Public Const VALUE_FIELDTYPE_CARD = "4"
        Public Const VALUE_FIELDTYPE_CONCEPT = "5"
        Public Const VALUE_FIELDTYPE_EMPLOYEENAME = "6"
        Public Const VALUE_FIELDTYPE_ACCESS = "7"
        Public Const VALUE_FIELDTYPE_GROUP = "8"
        Public Const VALUE_FIELDTYPE_BLANKS = "9"
        Public Const VALUE_FIELDTYPE_CONCEPTDATE = "10"
        Public Const VALUE_FIELDTYPE_BEGINCONTRACT = "11"
        Public Const VALUE_FIELDTYPE_ENDCONTRACT = "12"
        Public Const VALUE_FIELDTYPE_CONCEPTNAME = "13"
        Public Const VALUE_FIELDTYPE_CONCEPTSHORTNAME = "14"
        Public Const VALUE_FIELDTYPE_CONCEPTEXPORT = "15"
        Public Const VALUE_FIELDTYPE_CONCEPTVALUE = "16"
        Public Const VALUE_FIELDTYPE_BEGINPERIOD = "17"
        Public Const VALUE_FIELDTYPE_ENDPERIOD = "18"
        Public Const VALUE_FIELDTYPE_BIOMETRICID = "19"
        Public Const VALUE_FIELDTYPE_PERCENTAGE = "20"
        Public Const VALUE_FIELDTYPE_TAGXML = "21"

        Public Const ALL_ACCRUAL = 0
        Public Const NOTZEROACCRUAL = 1
        Public Const SELECTEDACCRUAL = 2

        Private mID As Integer
        Private mIDGuide As Integer
        Private mPos As Integer
        Private mData As New roCollection
        Private oState As roDataLinkState

        Public Sub New()
            Me.oState = New roDataLinkState
            mID = 0
            mIDGuide = 0
            mData.Clear()
            mData.Add(PROPERTY_FIELDTYPE, "")
            mData.Add(PROPERTY_FIELDVALUE, "")
            mData.Add(PROPERTY_FIELDNAMEHEADER, "")
            mData.Add(PROPERTY_FIELDLEN, "0")
            mData.Add(PROPERTY_FIELDPADDING, "")
            mData.Add(PROPERTY_FIELDDATEFORMAT, "")
            mData.Add(PROPERTY_FIELDTAGXML, "")

        End Sub

        Public Sub New(ByVal _State As roDataLinkState)
            Me.oState = _State
        End Sub

#End Region

#Region "Properties"

        Public Property State() As roDataLinkState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roDataLinkState)
                Me.oState = value
            End Set
        End Property

        Public Property Pos() As Object
            Get
                Return mPos
            End Get
            Set(ByVal vdata As Object)
                mPos = vdata
            End Set
        End Property

        Public Property ID() As Object
            Get
                Return mID
            End Get
            Set(ByVal vdata As Object)
                mID = vdata
            End Set
        End Property

        Public Property IDGuide() As Object
            Get
                Return mIDGuide
            End Get
            Set(ByVal vdata As Object)
                'used when assigning a value to the property, on the left side of an assignment.
                mIDGuide = vdata
            End Set
        End Property

#End Region

        Public Function LoadFromActiveDataset(ByVal PositionedDataset As DataRow, ByVal db As DbDataAdapter) As Boolean
            '
            ' Carga datos del campo de la guia de extraccion de un dataset posicionado.
            '   Devuelve True si ha cargado una guia, False si hay algun error.
            '

            Try

                ' Carga tipo de capa
                mID = PositionedDataset("ID")
                mIDGuide = PositionedDataset("IDGuide")
                mPos = PositionedDataset("Pos")

                If Any2String(PositionedDataset("Definition")) = "" Then
                    SaveToActiveDataset(PositionedDataset, db)
                End If

                ' Carga datos según tipo de capa
                If Not ParseLayerData(PositionedDataset("Definition")) Then
                    LoadFromActiveDataset = False
                    mIDGuide = Nothing
                    mID = Nothing
                    Exit Function
                End If

                ' Valida
                LoadFromActiveDataset = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGuideField::LoadFromActiveDataset")
                LoadFromActiveDataset = False
                mIDGuide = Nothing
                mID = Nothing
            End Try

        End Function

        Private Function ParseLayerData(ByVal Data As String) As Boolean
            '
            ' Obtiene datos de una guia.
            '  Devuelve True si todo va bien, False si hay un error en la definición de la capa.
            '

            Try
                mData.Clear()
                mData.LoadXMLString(Data)
                ParseLayerData = True
            Catch
                ParseLayerData = False
            End Try

        End Function

        Public Sub SaveToActiveDataset(ByRef PositionedDataset As DataRow, ByVal ad As DbDataAdapter)
            '
            ' Guarda una capa en la base de datos
            '

            PositionedDataset("Definition") = mData.XML

            Try
                ad.Update(PositionedDataset.Table)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGuideField::SaveToActiveDataset")
            End Try

        End Sub

        Public Function Data() As roCollection
            '
            ' Devuelve los datos de la capa
            '
            Data = mData

        End Function

    End Class

    <DataContract>
    Public Class roGuide

#Region "Declarations"

        Public Enum eGuideType
            <EnumMember> NONE
            <EnumMember> ASCII
            <EnumMember> EXCEL
            <EnumMember> XML
        End Enum

        Private oState As roDataLinkState

        Private mID As Integer
        Private mType As eGuideType

        Private strName As String
        Private strDescription As String
        Private strFilePath As String
        Private eType As eGuideType
        Private oDefinition As New roCollection

        Public Sub New()
            Me.oState = New roDataLinkState
            mType = eGuideType.NONE
            mID = 0

            oDefinition.Clear()

            oDefinition.Add(roGuideField.PROPERTY_HEADER, "")
            oDefinition.Add(roGuideField.PROPERTY_FORMATCONCEPT, "")
            oDefinition.Add(roGuideField.PROPERTY_NAMECONCEPT, "")
            oDefinition.Add(roGuideField.PROPERTY_DECIMALSYMBOL, "")
            oDefinition.Add(roGuideField.PROPERTY_LENDECIMALS, "0")
            oDefinition.Add(roGuideField.PROPERTY_CONCEPTS, "")
            oDefinition.Add(roGuideField.PROPERTY_TYPESELECCTIONCONCEPTS, "")
            oDefinition.Add(roGuideField.PROPERTY_CONCEPTVALUE, "")
            oDefinition.Add(roGuideField.PROPERTY_ZEROVALUE, "")

        End Sub

        Public Sub New(ByVal _IDGuide As Integer, ByVal _State As roDataLinkState)
            Me.oState = _State
            Me.ID = _IDGuide
            Me.Load()
        End Sub

#End Region

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM GuidesExtraction WHERE ID = " & Me.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.ID = oRow("ID")
                    Me.Name = Any2String(oRow("Name"))
                    Me.Type = String2GuideType(oRow("Type"))
                    Me.Description = Any2String(oRow("Description"))
                    Me.Definition = New roCollection(Any2String(oRow("Definition")))
                    Me.FilePath = Any2String(oRow("FilePath"))

                    bolRet = True

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGuide::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGuide::Load")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Carga datos de la guia de extraccion de un dataset posicionado.
        ''' </summary>
        ''' <param name="PositionedDataset"></param>
        ''' <param name="db"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadFromActiveDataset(ByVal PositionedDataset As DataRow, ByVal db As DbDataAdapter) As Boolean
            Try

                ' Carga tipo de capa
                mType = String2GuideType(PositionedDataset("Type"))
                mID = PositionedDataset("ID")
                strDescription = PositionedDataset("Description")

                If Any2String(PositionedDataset("Definition")) = "" Then
                    SaveToActiveDataset(PositionedDataset, db)
                End If

                ' Carga datos según tipo de capa
                If Not ParseLayerData(PositionedDataset("Definition")) Then
                    LoadFromActiveDataset = False
                    mType = eGuideType.NONE
                    mID = Nothing
                    Exit Function
                End If

                ' Valida
                LoadFromActiveDataset = True
            Catch
                LoadFromActiveDataset = False
                mType = eGuideType.NONE
                mID = 0
            End Try
        End Function

        ''' <summary>
        ''' Obtiene datos de una guia.
        ''' </summary>
        ''' <param name="Data"></param>
        ''' <returns>Devuelve True si todo va bien, False si hay un error en la definición de la capa.</returns>
        ''' <remarks></remarks>
        Private Function ParseLayerData(ByVal Data As String) As Boolean
            Try
                oDefinition.Clear()
                oDefinition.LoadXMLString(Data)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGuide::ParserLayerData")
                ParseLayerData = False
                Exit Function
            End Try
            ParseLayerData = True

        End Function

        ''' <summary>
        ''' Guarda una capa en la base de datos
        ''' </summary>
        ''' <param name="PositionedDataset"></param>
        ''' <param name="ad"></param>
        ''' <remarks></remarks>
        Public Sub SaveToActiveDataset(ByVal PositionedDataset As DataRow, ByVal ad As DbDataAdapter)

            PositionedDataset("Definition") = oDefinition.XML
            Try
                ad.Update(PositionedDataset.Table)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGuide::SaveToActiveDataset")
            End Try

        End Sub

        ''' <summary>
        ''' Devuelve los datos de la capa
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Data() As roCollection

            Data = oDefinition

        End Function

        Public Shared Function String2GuideType(ByVal strType As String) As roGuide.eGuideType

            Select Case strType
                Case "EXCEL"
                    Return eGuideType.EXCEL
                Case "ASCII"
                    Return eGuideType.ASCII
                Case "XML"
                    Return eGuideType.XML
                Case Else
                    Return eGuideType.NONE
            End Select
        End Function

        ''' <summary>
        ''' Recupera les Guies d'importacio
        ''' </summary>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetImports(ByVal _State As roDataLinkState) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Dim bCalendarV2 As Boolean = True

            Try

                bCalendarV2 = True

                Dim strSQL As String
                strSQL = "@SELECT# * from ImportGuides Order By ID"

                oRet = CreateDataTable(strSQL, )
                For Each oRow As DataRow In oRet.Rows
                    Select Case oRow("ID")
                        Case 1
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportEmployees.Name", "")
                        Case 2
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportScheduler.Name", "")
                        Case 3
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportTeoricCoverage.Name", "")
                        Case 4
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportEnterprises.Name", "")
                        Case 5
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportTasks.Name", "")
                        Case 6
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportAssignments.Name", "")
                        Case 7
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportHolidays.Name", "")
                        Case 8
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportProgrammedCause.Name", "")
                        Case 9
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportProgrammedAbsence.Name", "")
                        Case 10
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportProgrammedAbsence.Name", "")
                        Case 11
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportDailyCauses.Name", "")
                        Case 12
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportWorkSheets.Name", "")
                        Case 13
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportBusinessCenterName", "")
                        Case 14
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportPlannigV2.Name", "")
                        Case 15
                            oRow("Name") = _State.Language.Translate("ImportGuides.ImportEmployeePhotos.Name", "")
                    End Select
                Next

                Dim bolDelete As Boolean = True
                For Each row As DataRow In oRet.Rows
                    bolDelete = True
                    If row.RowState <> DataRowState.Deleted Then
                        If row("RequieredFunctionalities") Is DBNull.Value Then
                            bolDelete = False
                        Else
                            Dim requieredFeature As String = roTypes.Any2String(row("RequieredFunctionalities")).Trim

                            If WLHelper.GetPermissionOverFeature(_State.IDPassport, requieredFeature, "U") >= Permission.Read Then
                                bolDelete = False
                            End If
                        End If

                        ' Miro si debo cargar guía de importación V2
                        ' If row("ID") = 14 Then bolDelete = Not bCalendarV2

                        If bolDelete Then
                            row.Delete()
                        End If
                    End If
                Next

                oRet.AcceptChanges()
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roGuide::GetImports")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roGuide::GetImports")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Recupera les Guies d'exportació
        ''' </summary>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetExports(ByVal _State As roDataLinkState) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Dim bCalendarV2 As Boolean = True

            Try

                bCalendarV2 = True

                Dim strSQL As String
                strSQL = "@SELECT# * from ExportGuides Order By ID"

                oRet = CreateDataTable(strSQL, )
                For Each oRow As DataRow In oRet.Rows
                    Select Case oRow("ID")
                        Case 8980
                            oRow("Name") = _State.Language.Translate("ExportGuides.Shifts.Name", "")
                        Case 8981
                            oRow("Name") = _State.Language.Translate("ExportGuides.Iberper.Name", "")
                        Case 8982
                            oRow("Name") = _State.Language.Translate("ExportGuides.LogicClass.Name", "")
                        Case 8983
                            oRow("Name") = _State.Language.Translate("ExportGuides.Labor.Name", "")
                        Case 8984
                            oRow("Name") = _State.Language.Translate("ExportGuides.HolidaysShifts.Name", "")
                        Case 8985
                            oRow("Name") = _State.Language.Translate("ExportGuides.TasksByEmployee.Name", "")
                        Case 8986
                            oRow("Name") = _State.Language.Translate("ExportGuides.Assignments.Name", "")
                        Case 8987
                            oRow("Name") = _State.Language.Translate("ExportGuides.ScheduleAssignmentsByName.Name", "")
                        Case 8988
                            oRow("Name") = _State.Language.Translate("ExportGuides.ScheduleAssignments.Name", "")
                        Case 9000
                            oRow("Name") = _State.Language.Translate("ExportGuides.DailyAccruals.Name", "")
                        Case 9001
                            oRow("Name") = _State.Language.Translate("ExportGuides.PeriodicalAccruals.Name", "")
                        Case 9002
                            oRow("Name") = _State.Language.Translate("ExportGuides.Scheduler.Name", "")
                        Case 9003
                            oRow("Name") = _State.Language.Translate("ExportGuides.Holidays.Name", "")
                        Case 9004
                            oRow("Name") = _State.Language.Translate("ExportGuides.AdvAccruals.Name", "")
                        Case 9005
                            oRow("Name") = _State.Language.Translate("ExportGuides.AdvProlonguedAbsences.Name", "")
                        Case 9006
                            oRow("Name") = _State.Language.Translate("ExportGuides.AdvDinner.Name", "")
                        Case 9007
                            oRow("Name") = _State.Language.Translate("ExportGuides.AdvPunches.Name", "")
                        Case 9008
                            oRow("Name") = _State.Language.Translate("ExportGuides.AdvTasks.Name", "")
                        Case 9009
                            oRow("Name") = _State.Language.Translate("ExportGuides.AdvEmployees.Name", "")
                        Case 9010
                            oRow("Name") = _State.Language.Translate("ExportGuides.AdvCostCenters.Name", "")
                        Case 9012
                            oRow("Name") = _State.Language.Translate("ExportGuides.PlanningV2.Name", "")
                    End Select
                Next

                Dim bolDelete As Boolean = True
                For Each row As DataRow In oRet.Rows
                    bolDelete = True
                    If row.RowState <> DataRowState.Deleted Then
                        If row("RequieredFunctionalities") Is DBNull.Value Then
                            bolDelete = False
                        Else
                            Dim requieredFeature As String = roTypes.Any2String(row("RequieredFunctionalities")).Trim

                            If requieredFeature = String.Empty OrElse WLHelper.GetPermissionOverFeature(_State.IDPassport, requieredFeature, "U") >= Permission.Read Then
                                bolDelete = False
                            End If
                        End If

                        ' Miro si debo cargar guía de importación V2
                        ' If row("ID") = 9012 Then bolDelete = Not bCalendarV2

                        If bolDelete Then
                            row.Delete()
                        End If
                    End If
                Next

                oRet.AcceptChanges()
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roGuide::GetExports")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roGuide::GetExports")
            Finally

            End Try

            Return oRet

        End Function

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roDataLinkState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roDataLinkState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property ID() As Integer
            Get
                Return mID
            End Get
            Set(ByVal vdata As Integer)
                mID = vdata
            End Set
        End Property

        <DataMember>
        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        <DataMember>
        Public Property Description() As String
            Get
                Return strDescription
            End Get
            Set(ByVal value As String)
                strDescription = value
            End Set
        End Property

        <DataMember>
        Public Property Type() As eGuideType
            Get
                Return mType
            End Get
            Set(ByVal value As eGuideType)
                mType = value
            End Set
        End Property

        <DataMember>
        Public Property FilePath() As String
            Get
                Return strFilePath
            End Get
            Set(ByVal value As String)
                strFilePath = value
            End Set
        End Property

        <DataMember>
        Public Property Definition() As roCollection
            Get
                Return oDefinition
            End Get
            Set(ByVal value As roCollection)
                oDefinition = value
            End Set
        End Property

#End Region

    End Class

    <DataContract>
    Public Class roImportGuide

#Region "Declarations - Constructors"

        Private oState As roDataLinkState

        Private intID As Integer
        Private strName As String
        Private intTemplate As Integer
        Private intMode As Integer
        Private intType As Integer
        Private strFormatFilePath As String
        Private strSourceFilePath As String
        Private strSeparator As String
        Private bolCopySource As Nullable(Of Boolean)
        Private strRequieredFunctionalities As String
        Private strFeatureAliasID As String
        Private intDestination As Integer
        Private strParameters As String
        Private intEnabled As Nullable(Of Boolean)
        Private iVersion As Integer = 1 ' Por defecto importación antigua
        Private strConcept As String = String.Empty
        Private intIDDefaultTemplate As Integer

        Public Sub New()

            Me.oState = New roDataLinkState
            Me.intID = -1

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roDataLinkState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.intID = _ID

            Me.Load(_Audit)

        End Sub

        Public Sub New(ByVal _Row As DataRow, ByVal _State As roDataLinkState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State

            Me.LoadFromRow(_Row, _Audit)

        End Sub

#End Region

#Region "Properties"

        <XmlIgnore()>
        <IgnoreDataMember>
        Public Property State() As roDataLinkState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roDataLinkState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property

        <DataMember>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property

        <DataMember>
        Public Property Template() As Integer
            Get
                Return Me.intTemplate
            End Get
            Set(ByVal value As Integer)
                Me.intTemplate = value
            End Set
        End Property

        <DataMember>
        Public Property Mode() As Integer
            Get
                Return Me.intMode
            End Get
            Set(ByVal value As Integer)
                Me.intMode = value
            End Set
        End Property

        <DataMember>
        Public Property Type() As Integer
            Get
                Return Me.intType
            End Get
            Set(ByVal value As Integer)
                Me.intType = value
            End Set
        End Property

        <DataMember>
        Public Property FormatFilePath() As String
            Get
                Return Me.strFormatFilePath
            End Get
            Set(ByVal value As String)
                Me.strFormatFilePath = value
            End Set
        End Property

        <DataMember>
        Public Property SourceFilePath() As String
            Get
                Return Me.strSourceFilePath
            End Get
            Set(ByVal value As String)
                Me.strSourceFilePath = value
            End Set
        End Property

        <DataMember>
        Public Property Separator() As String
            Get
                Return Me.strSeparator
            End Get
            Set(ByVal value As String)
                Me.strSeparator = value
            End Set
        End Property

        <DataMember>
        Public Property CopySource() As Nullable(Of Boolean)
            Get
                Return bolCopySource
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolCopySource = value
            End Set
        End Property

        <DataMember>
        Public Property RequieredFunctionalities() As String
            Get
                Return Me.strRequieredFunctionalities
            End Get
            Set(ByVal value As String)
                Me.strRequieredFunctionalities = value
            End Set
        End Property

        <DataMember>
        Public Property FeatureAliasID() As String
            Get
                Return Me.strFeatureAliasID
            End Get
            Set(ByVal value As String)
                Me.strFeatureAliasID = value
            End Set
        End Property

        <DataMember>
        Public Property Parameters() As String
            Get
                Return Me.strParameters
            End Get
            Set(ByVal value As String)
                Me.strParameters = value
            End Set
        End Property

        <DataMember>
        Public Property Destination() As Integer
            Get
                Return Me.intDestination
            End Get
            Set(ByVal value As Integer)
                Me.intDestination = value
            End Set
        End Property

        <DataMember>
        Public Property Enabled() As Nullable(Of Boolean)
            Get
                Return intEnabled
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                intEnabled = value
            End Set
        End Property

        <DataMember>
        Public Property Version As Integer
            Get
                Return iVersion
            End Get
            Set(value As Integer)
                iVersion = value
            End Set
        End Property

        <DataMember>
        Public Property Concept As String
            Get
                Return Me.strConcept
            End Get
            Set(value As String)
                Me.strConcept = value
            End Set
        End Property

        <DataMember>
        Public Property DefaultTemplateID As Integer
            Get
                Return intIDDefaultTemplate
            End Get
            Set(value As Integer)
                intIDDefaultTemplate = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM ImportGuides WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = Any2String(oRow("Name"))
                    Me.intTemplate = Any2Integer(oRow("Template"))
                    Me.intMode = Any2Integer(oRow("Mode"))
                    Me.intType = Any2Integer(oRow("Type"))
                    Me.strFormatFilePath = Any2String(oRow("FormatFilePath"))
                    Me.strSourceFilePath = Any2String(oRow("SourceFilePath"))
                    Me.strSeparator = Any2String(oRow("Separator"))
                    Me.CopySource = Any2Boolean(oRow("CopySource"))
                    Me.strRequieredFunctionalities = Any2String(oRow("RequieredFunctionalities"))
                    Me.strFeatureAliasID = Any2String(oRow("FeatureAliasID"))
                    Me.Destination = Any2Integer(oRow("Destination"))
                    Me.intEnabled = Any2Boolean(oRow("Enabled"))
                    Me.iVersion = Any2Integer(oRow("Version"))
                    Me.strConcept = Any2String(oRow("Concept"))
                    Me.intIDDefaultTemplate = Any2Integer(oRow("IDDefaultTemplate"))

                    bolRet = True

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roImportGuide::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roImportGuide::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function LoadFromRow(ByVal oRow As DataRow, Optional ByVal _Audit As Boolean = False)

            Dim bolRet As Boolean = False

            If oRow IsNot Nothing Then

                Me.intID = oRow("ID")
                Me.strName = Any2String(oRow("Name"))
                Me.intTemplate = Any2Integer(oRow("Template"))
                Me.intMode = Any2Integer(oRow("Mode"))
                Me.intType = Any2Integer(oRow("Type"))
                Me.strFormatFilePath = Any2String(oRow("FormatFilePath"))
                Me.strSourceFilePath = Any2String(oRow("SourceFilePath"))
                Me.strSeparator = Any2String(oRow("Separator"))
                Me.CopySource = Any2Boolean(oRow("CopySource"))
                Me.strRequieredFunctionalities = Any2String(oRow("RequieredFunctionalities"))
                Me.strFeatureAliasID = Any2String(oRow("FeatureAliasID"))
                Me.Destination = Any2Integer(oRow("Destination"))
                Me.intEnabled = Any2Boolean(oRow("Enabled"))
                Me.iVersion = Any2Integer(oRow("Version"))
                Me.strConcept = Any2String(oRow("Concept"))
                Me.intIDDefaultTemplate = Any2Integer(oRow("IDDefaultTemplate"))

                bolRet = True

                If _Audit Then
                    ' ***

                End If

            End If

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("ImportGuides")
                    Dim strSQL As String = "@SELECT# * FROM ImportGuides " &
                                           "WHERE ID = " & Me.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("ID") = Me.GetNextID()
                        Me.ID = oRow("ID")
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Name") = Me.strName
                    oRow("Template") = Me.intTemplate
                    oRow("Mode") = Me.intMode
                    If Me.intTemplate = 1 Then
                        Me.intType = 1
                    End If
                    oRow("Type") = Me.intType
                    oRow("FormatFilePath") = Me.strFormatFilePath
                    oRow("SourceFilePath") = Me.strSourceFilePath
                    oRow("Separator") = Me.strSeparator
                    oRow("CopySource") = Me.CopySource
                    oRow("Destination") = Me.intDestination

                    If Me.ID = 15 Then
                        oRow("Parameters") = Me.strParameters
                        oRow("Enabled") = Me.intEnabled
                    End If

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oAuditDataNew = oRow

                    bolRet = True

                    If bolRet And bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = roAudit.CreateParametersTable()
                        roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tDataLink, strObjectName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roImportGuide::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roImportGuide::Save")
            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Si esta en modo automático comprueba ...
                If Me.intMode = 2 Then
                    ' Si el fichero es tipo ASCII
                    If Me.intType = 2 Then
                        ' Si no es carga de dotación teórica comprueba la plantilla
                        If Me.ID <> 3 Then
                            If Any2String(Me.strFormatFilePath).Trim = "" Then
                                oState.Result = DataLinkResultEnum.InvalidProfileName
                                Exit Try
                            End If
                        End If
                    End If

                    ' rtort - 20160511 - validate if has data link
                    If Me.Destination = 0 Then
                        oState.Result = DataLinkResultEnum.InvalidDestination
                        Exit Try
                    End If

                    ' Comprueba el fichero a importar
                    If Me.ID <> 15 Then
                        If Any2String(Me.SourceFilePath).Trim = "" Then
                            oState.Result = DataLinkResultEnum.InvalidImportFileName
                            Exit Try
                        End If
                    End If
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roImportGuide::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roImportGuide::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene el siguiente ID disponible para dar de alta un nueva guia
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextID() As Integer

            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# MAX(ID) FROM ImportGuides"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet + 1

        End Function

        ''' <summary>
        ''' Borra la guia siempre y cuando no se use.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                bolRet = False

                'Borramos la guia
                Dim DelQuerys() As String = {"@DELETE# FROM ImportGuides WHERE ID = " & Me.ID.ToString}
                For n As Integer = 0 To DelQuerys.Length - 1
                    If Not ExecuteSql(DelQuerys(n)) Then
                        oState.Result = DataLinkResultEnum.ConnectionError
                        Exit For
                    End If
                Next

                bolRet = (oState.Result = DataLinkResultEnum.NoError)

                'If bolRet And bAudit Then
                '    '' Auditamos
                '    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAssignment, Me.strName, Nothing, -1, oTrans.Connection)
                'End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roImportGuide::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roImportGuide::Delete")
            End Try

            Return bolRet

        End Function

#End Region

        Public Shared Function GetExportTemplates(ByVal ostate As roDataLinkState) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                Dim oSettings As New roSettings()
                Dim strPathFile As String = oSettings.GetVTSetting(eKeys.DataLink) & "\ExportGuides.rar"

                If File.Exists(strPathFile) Then
                    arrFile = File.ReadAllBytes(strPathFile)
                End If
            Catch ex As Exception
                ostate.UpdateStateInfo(ex, "roImportGuide::GetExportTemplates")
            Finally
            End Try

            Return arrFile

        End Function

    End Class

    <DataContract>
    Public Class roDatalinkDisplayConfiguration
        Public bAutomaticModeEnabled As Boolean
        Public bAcceptProfile As Boolean
        Public bCanExportToExcel As Boolean
        Public bCanExportToASCII As Boolean
        Public bCanExportToXML As Boolean
        Public strDisplayParameters As String

#Region "Properties"

        <DataMember>
        Public Property AutomaticModeEnabled() As Boolean
            Get
                Return Me.bAutomaticModeEnabled
            End Get
            Set(ByVal value As Boolean)
                Me.bAutomaticModeEnabled = value
            End Set
        End Property

        <DataMember>
        Public Property AcceptProfile() As Boolean
            Get
                Return Me.bAcceptProfile
            End Get
            Set(ByVal value As Boolean)
                Me.bAcceptProfile = value
            End Set
        End Property

        <DataMember>
        Public Property CanExportToExcel() As Boolean
            Get
                Return Me.bCanExportToExcel
            End Get
            Set(ByVal value As Boolean)
                Me.bCanExportToExcel = value
            End Set
        End Property

        <DataMember>
        Public Property CanExportToAscii() As Boolean
            Get
                Return Me.bCanExportToASCII
            End Get
            Set(ByVal value As Boolean)
                Me.bCanExportToASCII = value
            End Set
        End Property

        <DataMember>
        Public Property CanExportToXML() As Boolean
            Get
                Return Me.bCanExportToXML
            End Get
            Set(ByVal value As Boolean)
                Me.bCanExportToXML = value
            End Set
        End Property

        <DataMember>
        Public Property DisplayParameters() As String
            Get
                Return Me.strDisplayParameters
            End Get
            Set(ByVal value As String)
                Me.strDisplayParameters = value
            End Set
        End Property

#End Region

#Region "Constructors"

        Public Sub New()
            bAutomaticModeEnabled = False
            bAcceptProfile = False
            bCanExportToExcel = False
            bCanExportToASCII = False
            bCanExportToXML = False
        End Sub

        Public Sub New(ByVal strParameters As String)

            strDisplayParameters = strParameters
            bAutomaticModeEnabled = False
            bAcceptProfile = False
            bCanExportToExcel = False
            bCanExportToASCII = False
            bCanExportToXML = False

            Dim sParams() As String = strParameters.Split("@")

            If sParams.Length = 3 Then
                Dim sTypes() As String = sParams(0).Split(",")

                If sTypes.Length = 3 Then
                    If sTypes(0) = "1" Then bCanExportToASCII = True Else bCanExportToASCII = False
                    If sTypes(1) = "1" Then bCanExportToExcel = True Else bCanExportToExcel = False
                    If sTypes(2) = "1" Then bCanExportToXML = True Else bCanExportToXML = False
                End If

                If sParams(1) = "1" Then bAutomaticModeEnabled = True Else bAutomaticModeEnabled = False
                If sParams(2) = "1" Then bAcceptProfile = True Else bAcceptProfile = False
            End If

        End Sub

#End Region

    End Class

    Public Class roExportGuideTemplate
        Private oState As roDataLinkState
        Private intID As Integer
        Private intIDParentGuide As Integer
        Private strName As String = String.Empty
        Private strProfile As String = String.Empty
        Private oParameters As New roCollection
        Private strPreProcessScript As String = String.Empty
        Private strPostProcessScript As String = String.Empty

        Public ReadOnly Property ID As Integer
            Get
                Return intID
            End Get
        End Property

        Public ReadOnly Property IDParentGuide As Integer
            Get
                Return intIDParentGuide
            End Get
        End Property

        Public ReadOnly Property Name As String
            Get
                Return strName
            End Get
        End Property

        Public ReadOnly Property Profile As String
            Get
                Return strProfile
            End Get
        End Property

        Public ReadOnly Property PreProcessScript As String
            Get
                Return strPreProcessScript
            End Get
        End Property

        Public ReadOnly Property PostProcessScript As String
            Get
                Return strPostProcessScript
            End Get
        End Property

        Public ReadOnly Property Parameters() As roCollection
            Get
                Return oParameters
            End Get
        End Property

        Public Sub New()
            Me.oState = New roDataLinkState
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _State As roDataLinkState)
            Me.oState = _State
            Me.intID = -1
        End Sub

        Public Sub New(iID As Integer, ByVal _State As roDataLinkState, Optional ByVal _Audit As Boolean = False)
            Me.oState = _State
            Me.intID = iID
            Me.Load()
        End Sub

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM ExportGuidesTemplates WHERE ID = " & Me.intID.ToString 'De momento sólo habrá 1 plantilla por exportación
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = Any2String(oRow("Name"))
                    Me.intIDParentGuide = Any2Integer(oRow("IDParentGuide"))
                    Me.strProfile = Any2String(oRow("Profile"))

                    Me.strPostProcessScript = Any2String(oRow("PostProcessScript"))
                    Me.strPreProcessScript = Any2String(oRow("PreProcessScript"))

                    If Not IsDBNull(oRow("Parameters")) Then
                        Dim oTask As New VTLiveTasks.roLiveTask
                        Me.oParameters = oTask.BuildFromXML(oRow("Parameters"))
                    End If

                    bolRet = True
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roExportGuideTemplate::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roExportGuideTemplate::Load")
            Finally

            End Try
            Return bolRet
        End Function

        Public Function LoadByParentId(iIDParent As Integer, iIDTemplate As Integer) As Boolean

            Dim bolRet As Boolean = False

            Try

                Me.intIDParentGuide = iIDParent

                Dim strSQL As String = "@SELECT# * FROM ExportGuidesTemplates WHERE IDParentGuide = " & Me.intIDParentGuide.ToString & " AND ID = " & iIDTemplate.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.intID = Any2Integer(oRow("ID"))
                    Me.strName = Any2String(oRow("Name"))
                    Me.strProfile = Any2String(oRow("Profile"))

                    Me.strPostProcessScript = Any2String(oRow("PostProcessScript"))
                    Me.strPreProcessScript = Any2String(oRow("PreProcessScript"))

                    Dim oTask As New VTLiveTasks.roLiveTask
                    Me.oParameters = oTask.BuildFromXML(Any2String(oRow("Parameters")))

                    bolRet = True
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roExportGuideTemplate::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roExportGuideTemplate::Load")
            Finally

            End Try
            Return bolRet
        End Function

    End Class

    <DataContract>
    Public Class roImportGuideTemplate
        Private oState As roDataLinkState
        Private intID As Integer
        Private intIDParentGuide As Integer
        Private strName As String = String.Empty
        Private strProfile As String = String.Empty
        Private oParameters As New roCollection
        Private strPreProcessScript As String = String.Empty
        Private strPostProcessScript As String = String.Empty

        <DataMember>
        Public ReadOnly Property ID As Integer
            Get
                Return intID
            End Get
        End Property

        <DataMember>
        Public ReadOnly Property IDParentGuide As Integer
            Get
                Return intIDParentGuide
            End Get
        End Property

        <DataMember>
        Public ReadOnly Property Name As String
            Get
                Return strName
            End Get
        End Property

        <DataMember>
        Public ReadOnly Property Profile As String
            Get
                Return strProfile
            End Get
        End Property

        <DataMember>
        Public ReadOnly Property PreProcessScript As String
            Get
                Return strPreProcessScript
            End Get
        End Property

        <DataMember>
        Public ReadOnly Property PostProcessScript As String
            Get
                Return strPostProcessScript
            End Get
        End Property

        <DataMember>
        Public ReadOnly Property Parameters() As roCollection
            Get
                Return oParameters
            End Get
        End Property

        Public Sub New()
            Me.oState = New roDataLinkState
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _State As roDataLinkState)
            Me.oState = _State
            Me.intID = -1
        End Sub

        Public Sub New(iID As Integer, ByVal _State As roDataLinkState, Optional ByVal _Audit As Boolean = False)
            Me.oState = _State
            Me.intID = iID
            Me.Load()
        End Sub

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM ImportGuidesTemplates WHERE ID = " & Me.intID.ToString 'De momento sólo habrá 1 plantilla por exportación
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = Any2String(oRow("Name"))
                    Me.intIDParentGuide = Any2Integer(oRow("IDParentGuide"))
                    Me.strProfile = Any2String(oRow("Profile"))

                    Me.strPostProcessScript = Any2String(oRow("PostProcessScript"))
                    Me.strPreProcessScript = Any2String(oRow("PreProcessScript"))

                    Dim oTask As New VTLiveTasks.roLiveTask
                    Me.oParameters = oTask.BuildFromXML(oRow("Parameters"))

                    bolRet = True
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roImportGuideTemplate::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roImportGuideTemplate::Load")
            Finally

            End Try
            Return bolRet
        End Function

        Public Function LoadByParentId(iIDParent As Integer, iIDTemplate As Integer) As Boolean

            Dim bolRet As Boolean = False

            Try

                Me.intIDParentGuide = iIDParent

                Dim strSQL As String = "@SELECT# * FROM ImportGuidesTemplates WHERE IDParentGuide = " & Me.intIDParentGuide.ToString & " AND ID = " & iIDTemplate.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.intID = Any2Integer(oRow("ID"))
                    Me.strName = Any2String(oRow("Name"))
                    Me.strProfile = Any2String(oRow("Profile"))

                    Me.strPostProcessScript = Any2String(oRow("PostProcessScript"))
                    Me.strPreProcessScript = Any2String(oRow("PreProcessScript"))

                    Dim oTask As New VTLiveTasks.roLiveTask
                    Me.oParameters = oTask.BuildFromXML(Any2String(oRow("Parameters")))

                    bolRet = True
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roImportGuideTemplate::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roImportGuideTemplate::Load")
            Finally

            End Try
            Return bolRet
        End Function

    End Class

    <DataContract>
    Public Class roExportGuide

#Region "Declarations - Constructors"

        Public Enum ExportTypeEnum
            <EnumMember> EXCEL = 1
            <EnumMember> ASCII = 2
        End Enum

        Public Enum ExportModeEnum
            <EnumMember> MANUAL = 1
            <EnumMember> AUTO = 2
        End Enum

        Private oState As roDataLinkState
        Private intID As Integer
        Private strName As String
        Private intMode As ExportModeEnum
        Private strProfileMask As String
        Private intProfileType As ExportTypeEnum
        Private strProfileName As String
        Private strDestination As String
        Private strExportFileName As String
        Private intExportFileType As Integer
        Private strSeparator As String
        Private intStartCalculDay As Integer
        Private strStartExecutionHour As String
        Private strLastLog As String
        Private datNextExecution As Date
        Private intIntervalMinutes As Integer
        Private strEmployeeFilter As String
        Private strFeatureAliasID As String
        Private strRequieredFunctionalities As String
        Private strSourceFilePath As String
        Private bApplyLockDate As Boolean

        Private strAutomaticDatePeriod As String

        Private oScheduler As roReportSchedulerSchedule
        Private oDisplayConf As roDatalinkDisplayConfiguration

        Private oWsConf As roCollection
        Private strWsConf As String
        Private iVersion As Integer = 1
        Private strConcept As String = String.Empty
        Private intIDDefaultTemplate As Integer

        Public Sub New()
            Me.oState = New roDataLinkState
            Me.intID = -1
            oWsConf = Nothing
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roDataLinkState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.intID = _ID

            Me.Load(_Audit)

        End Sub

        Public Sub New(ByVal _Row As DataRow, ByVal _State As roDataLinkState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State

            Me.LoadFromRow(_Row, _Audit)

        End Sub

#End Region

#Region "Properties"

        <XmlIgnore()>
        <IgnoreDataMember>
        Public Property State() As roDataLinkState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roDataLinkState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property

        <DataMember>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property

        <DataMember>
        Public Property StartExecutionHour() As String
            Get
                Return Me.strStartExecutionHour
            End Get
            Set(ByVal value As String)
                Me.strStartExecutionHour = value
            End Set
        End Property

        <DataMember>
        Public Property Mode() As Integer
            Get
                Return Me.intMode
            End Get
            Set(ByVal value As Integer)
                Me.intMode = value
            End Set
        End Property

        <DataMember>
        Public Property ProfileType() As Integer
            Get
                Return Me.intProfileType
            End Get
            Set(ByVal value As Integer)
                Me.intProfileType = value
            End Set
        End Property

        <DataMember>
        Public Property ProfileName() As String
            Get
                Return Me.strProfileName
            End Get
            Set(ByVal value As String)
                Me.strProfileName = value
            End Set
        End Property

        <DataMember>
        Public Property ProfileMask() As String
            Get
                Return Me.strProfileMask
            End Get
            Set(ByVal value As String)
                Me.strProfileMask = value
            End Set
        End Property

        <DataMember>
        Public Property ExportFileType() As String
            Get
                Return Me.intExportFileType
            End Get
            Set(ByVal value As String)
                Me.intExportFileType = value
            End Set
        End Property

        <DataMember>
        Public Property Destination() As String
            Get
                Return Me.strDestination
            End Get
            Set(ByVal value As String)
                Me.strDestination = value
            End Set
        End Property

        <DataMember>
        Public Property ExportFileName() As String
            Get
                Return Me.strExportFileName
            End Get
            Set(ByVal value As String)
                Me.strExportFileName = value
            End Set
        End Property

        <DataMember>
        Public Property StartCalculDay() As Integer
            Get
                Return Me.intStartCalculDay
            End Get
            Set(ByVal value As Integer)
                Me.intStartCalculDay = value
            End Set
        End Property

        <DataMember>
        Public Property Separator() As String
            Get
                Return Me.strSeparator
            End Get
            Set(ByVal value As String)
                Me.strSeparator = value
            End Set
        End Property

        <DataMember>
        Public Property NextExecutionDate() As Date
            Get
                Return Me.datNextExecution
            End Get
            Set(ByVal value As Date)
                Me.datNextExecution = value
            End Set
        End Property

        <DataMember>
        Public Property IntervalMinutes() As Integer
            Get
                Return Me.intIntervalMinutes
            End Get
            Set(ByVal value As Integer)
                Me.intIntervalMinutes = value
            End Set
        End Property

        <DataMember>
        Public Property LastLog() As String
            Get
                Return Me.strLastLog
            End Get
            Set(ByVal value As String)
                Me.strLastLog = value
            End Set
        End Property

        <DataMember>
        Public Property EmployeeFilter() As String
            Get
                Return Me.strEmployeeFilter
            End Get
            Set(ByVal value As String)
                Me.strEmployeeFilter = value
            End Set
        End Property

        <DataMember>
        Public Property RequieredFunctionalities() As String
            Get
                Return Me.strRequieredFunctionalities
            End Get
            Set(ByVal value As String)
                Me.strRequieredFunctionalities = value
            End Set
        End Property

        <DataMember>
        Public Property FeatureAliasID() As String
            Get
                Return Me.strFeatureAliasID
            End Get
            Set(ByVal value As String)
                Me.strFeatureAliasID = value
            End Set
        End Property

        <DataMember>
        Public Property Scheduler() As roReportSchedulerSchedule
            Get
                Return oScheduler
            End Get
            Set(ByVal value As roReportSchedulerSchedule)
                oScheduler = value
            End Set
        End Property

        <DataMember>
        Public Property DisplayConfiguration() As roDatalinkDisplayConfiguration
            Get
                Return oDisplayConf
            End Get
            Set(ByVal value As roDatalinkDisplayConfiguration)
                oDisplayConf = value
            End Set
        End Property

        <DataMember>
        Public Property WSConf() As roCollection
            Get
                Return oWsConf
            End Get
            Set(ByVal value As roCollection)
                oWsConf = value
            End Set
        End Property

        <DataMember>
        Public Property WSxmlConf As String
            Get
                Return Me.strWsConf
            End Get
            Set(ByVal value As String)
                Me.strWsConf = value
            End Set
        End Property

        <DataMember>
        Public Property AutomaticDatePeriod As String
            Get
                Return Me.strAutomaticDatePeriod
            End Get
            Set(ByVal value As String)
                Me.strAutomaticDatePeriod = value
            End Set
        End Property

        <DataMember>
        Public Property Version As Integer
            Get
                Return Me.iVersion
            End Get
            Set(value As Integer)
                Me.iVersion = value
            End Set
        End Property

        <DataMember>
        Public Property Concept As String
            Get
                Return Me.strConcept
            End Get
            Set(value As String)
                Me.strConcept = value
            End Set
        End Property

        <DataMember>
        Public Property DefaultTemplateID As Integer
            Get
                Return Me.intIDDefaultTemplate
            End Get
            Set(value As Integer)
                Me.intIDDefaultTemplate = value
            End Set
        End Property

        <DataMember>
        Public Property ApplyLockDate As Boolean
            Get
                Return Me.bApplyLockDate
            End Get
            Set(value As Boolean)
                Me.bApplyLockDate = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM ExportGuides WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = Any2String(oRow("Name"))
                    Me.intMode = Any2Integer(oRow("Mode"))
                    Me.strProfileMask = Any2String(oRow("ProfileMask"))
                    Me.intProfileType = Any2Integer(oRow("ProfileType"))
                    Me.strProfileName = Any2String(oRow("ProfileName"))
                    Me.strDestination = Any2Integer(oRow("Destination"))
                    Me.strExportFileName = Any2String(oRow("ExportFileName"))
                    Me.intExportFileType = Any2String(oRow("ExportFileType"))
                    Me.strSeparator = Any2String(oRow("Separator"))
                    Me.intStartCalculDay = Any2Integer(oRow("StartCalculDay"))
                    Me.strStartExecutionHour = Any2String(oRow("StartExecutionHour"))
                    Me.datNextExecution = Any2DateTime(oRow("NextExecution"))
                    Me.intIntervalMinutes = Any2Integer(oRow("IntervalMinutes"))
                    Me.strLastLog = Any2String(oRow("LastLog"))
                    Me.strEmployeeFilter = Any2String(oRow("EmployeeFilter"))
                    Me.strRequieredFunctionalities = Any2String(oRow("RequieredFunctionalities"))
                    Me.strFeatureAliasID = Any2String(oRow("FeatureAliasID"))
                    Me.iVersion = Any2Integer(oRow("Version"))
                    Me.strConcept = Any2String(oRow("Concept"))
                    Me.intIDDefaultTemplate = Any2Integer(oRow("IDDefaultTemplate"))
                    Me.bApplyLockDate = Any2Boolean(oRow("ApplyLockDate"))

                    If Not IsDBNull(oRow("DisplayParameters")) Then
                        Me.oDisplayConf = New roDatalinkDisplayConfiguration(oRow("DisplayParameters"))
                    Else
                        Me.oDisplayConf = New roDatalinkDisplayConfiguration()
                    End If

                    If Not IsDBNull(oRow("Scheduler")) Then
                        Me.oScheduler = roReportSchedulerScheduleManager.Load(oRow("Scheduler"))
                    Else
                        If Me.strStartExecutionHour <> String.Empty Then Me.oScheduler = roReportSchedulerScheduleManager.Load("0@" & strStartExecutionHour & "@1")
                    End If

                    If Not IsDBNull(oRow("WSParameters")) Then
                        Dim oTask As New VTLiveTasks.roLiveTask
                        Me.oWsConf = oTask.BuildFromXML(oRow("WSParameters"))
                        Me.strWsConf = oRow("WSParameters")
                    End If

                    If Not IsDBNull(oRow("AutomaticDatePeriod")) Then
                        Me.strAutomaticDatePeriod = oRow("AutomaticDatePeriod")
                    Else
                        Me.strAutomaticDatePeriod = String.Empty
                    End If

                    bolRet = True

                    ' Auditar lectura
                    'If _Audit Then
                    '    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    '    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    '    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tAssignment, Me.strName, tbParameters, -1)
                    'End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roExportGuide::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roExportGuide::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function LoadFromRow(ByVal oRow As DataRow, Optional ByVal _Audit As Boolean = False)

            Dim bolRet As Boolean = False

            If oRow IsNot Nothing Then

                Me.intID = oRow("ID")
                Me.strName = Any2String(oRow("Name"))
                Me.intMode = Any2Integer(oRow("Mode"))
                Me.strProfileMask = Any2String(oRow("ProfileMask"))
                Me.intProfileType = Any2Integer(oRow("ProfileType"))
                Me.strProfileName = Any2String(oRow("ProfileName"))
                Me.strDestination = Any2String(oRow("Destination"))
                Me.strExportFileName = Any2String(oRow("ExportFileName"))
                Me.intExportFileType = Any2String(oRow("ExportFileType"))
                Me.strSeparator = Any2String(oRow("Separator"))
                Me.intStartCalculDay = Any2Integer(oRow("StartCalculDay"))
                Me.strStartExecutionHour = Any2String(oRow("StartHour"))
                Me.datNextExecution = Any2String(oRow("NextExecution"))
                Me.intIntervalMinutes = Any2Integer(oRow("IntervalMinutes"))
                Me.strLastLog = Any2String(oRow("LastLog"))
                Me.strEmployeeFilter = Any2String(oRow("EmployeeFilter"))
                Me.strRequieredFunctionalities = Any2String(oRow("RequieredFunctionalities"))
                Me.strFeatureAliasID = Any2String(oRow("FeatureAliasID"))
                Me.iVersion = Any2Integer(oRow("Version"))
                Me.strConcept = Any2String(oRow("Concept"))
                Me.intIDDefaultTemplate = Any2Integer(oRow("IDDefaultTemplate"))
                Me.bApplyLockDate = Any2Boolean(oRow("ApplyLockDate"))

                If Not IsDBNull(oRow("DisplayParameters")) Then
                    Me.oDisplayConf = New roDatalinkDisplayConfiguration(oRow("DisplayParameters"))
                Else
                    Me.oDisplayConf = New roDatalinkDisplayConfiguration()
                End If

                If Not IsDBNull(oRow("Scheduler")) Then
                    Me.oScheduler = roReportSchedulerScheduleManager.Load(oRow("Scheduler"))
                Else
                    If Me.strStartExecutionHour <> String.Empty Then Me.oScheduler = roReportSchedulerScheduleManager.Load("0@" & strStartExecutionHour & "@1")
                End If

                If Not IsDBNull(oRow("WSParameters")) Then
                    Dim oTask As New VTLiveTasks.roLiveTask
                    Me.oWsConf = oTask.BuildFromXML(oRow("WSParameters"))
                    Me.strWsConf = oRow("WSParameters")
                End If

                If Not IsDBNull(oRow("AutomaticDatePeriod")) Then
                    Me.strAutomaticDatePeriod = oRow("AutomaticDatePeriod")
                Else
                    Me.strAutomaticDatePeriod = String.Empty
                End If

                bolRet = True

                If _Audit Then
                    ' ***

                End If

            End If

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = True

            Try

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("ExportGuides")
                    Dim strSQL As String = "@SELECT# * FROM ExportGuides " &
                                           "WHERE ID = " & Me.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow

                        If Me.ID <= 15000 Then
                            oRow("ID") = Me.GetNextID()
                            Me.ID = oRow("ID")
                        Else
                            oRow("ID") = Me.ID
                        End If

                        oRow("Name") = Me.strName
                        oRow("ProfileMask") = Me.strProfileMask
                        oRow("ProfileType") = Me.intProfileType
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Mode") = Me.intMode
                    oRow("ProfileName") = Me.strProfileName
                    oRow("Destination") = Me.strDestination
                    oRow("ExportFileName") = Me.strExportFileName
                    oRow("ExportFileType") = Me.intExportFileType
                    oRow("Separator") = Me.strSeparator
                    oRow("StartCalculDay") = Me.intStartCalculDay
                    oRow("NextExecution") = DBNull.Value
                    oRow("IntervalMinutes") = Me.intIntervalMinutes
                    oRow("FeatureAliasID") = Me.strFeatureAliasID
                    oRow("RequieredFunctionalities") = Me.strRequieredFunctionalities
                    oRow("DisplayParameters") = Convert.ToInt32(Me.oDisplayConf.bCanExportToASCII).ToString + "," + Convert.ToInt32(Me.oDisplayConf.bCanExportToExcel).ToString + "," + Convert.ToInt32(Me.oDisplayConf.bCanExportToXML).ToString + "@" + Convert.ToInt32(Me.oDisplayConf.bAutomaticModeEnabled).ToString + "@" + Convert.ToInt32(Me.oDisplayConf.bAcceptProfile).ToString
                    oRow("EmployeeFilter") = Me.strEmployeeFilter
                    oRow("ApplyLockDate") = Me.bApplyLockDate

                    ' Si es en modo automático determina la siguiente hora de ejecución
                    If Me.intMode = ExportModeEnum.AUTO Then
                        oRow("Scheduler") = roReportSchedulerScheduleManager.retScheduleString(Me.oScheduler)
                        Dim gNextException As Exception = Nothing

                        Dim nextDate As Nullable(Of Date) = Support.roLiveSupport.GetNextRun(roReportSchedulerScheduleManager.retScheduleString(oScheduler), Nothing, gNextException)
                        If nextDate IsNot Nothing Then
                            oRow("NextExecution") = nextDate
                        Else
                            oRow("NextExecution") = DBNull.Value
                        End If

                        If gNextException IsNot Nothing Then
                            oState.UpdateStateInfo(gNextException, "roExportGuide::GetNextRun")
                            bolRet = False
                        End If
                    Else
                        oRow("Scheduler") = ""
                    End If

                    If Me.WSxmlConf IsNot Nothing AndAlso Me.WSxmlConf <> String.Empty Then
                        oRow("WSParameters") = Me.WSxmlConf
                    End If

                    If Me.strAutomaticDatePeriod <> String.Empty Then
                        oRow("AutomaticDatePeriod") = Me.strAutomaticDatePeriod
                    Else
                        oRow("AutomaticDatePeriod") = DBNull.Value
                    End If

                    oRow("Version") = Me.iVersion

                    If bolRet Then
                        If tb.Rows.Count = 0 Then
                            tb.Rows.Add(oRow)
                        End If
                        da.Update(tb)

                        oAuditDataNew = oRow

                        bolRet = True

                        If bolRet And bAudit Then
                            bolRet = False
                            ' Auditamos
                            Dim tbAuditParameters As DataTable = roAudit.CreateParametersTable()
                            roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                            Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                            Dim strObjectName As String
                            If oAuditAction = Audit.Action.aInsert Then
                                strObjectName = oAuditDataNew("Name")
                            Else
                                strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                            End If
                            bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tDataLink, strObjectName, tbAuditParameters, -1)
                        End If

                    End If
                Else
                    bolRet = False

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roExportGuide::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roExportGuide::Save")
            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Campos obligatorios
                ' ProfileMask no puede estar en blanco
                If Me.strProfileMask = "" Then
                    oState.Result = DataLinkResultEnum.InvalidProfileMask
                    Exit Try
                End If

                ' ProfileType no puede estar en blanco
                If Me.intProfileType = 0 Then
                    oState.Result = DataLinkResultEnum.InvalidProfileType
                    Exit Try
                End If

                ' Mode no puede estar en blanco
                If Me.intMode <> 1 And Me.intMode <> 2 Then
                    oState.Result = DataLinkResultEnum.InvalidMode
                    Exit Try
                End If

                ' Type no puede estar en blanco
                If Me.intExportFileType <> 1 And Me.intExportFileType <> 2 Then
                    oState.Result = DataLinkResultEnum.InvalidExportFileType
                    Exit Try
                End If

                ' PUEDE ESTAR EN BLANCO PORQUE PUEDE SER UNA EXPORTACION EN ASCII FORMATEADA
                ' En exportación ASCII El caracter separador no puede estar en blanco
                'If intExportFileType = ExportTypeEnum.ASCII And Me.strSeparator = "" Then
                ' oState.Result = roDataLinkState.ResultEnum.InvalidDelimiter
                ' Exit Try
                ' End If

                ' En exportación automática
                If Me.intMode = ExportModeEnum.AUTO Then
                    ' El fichero plantilla no puede estar en blanco
                    If Me.strProfileName = "" Then
                        oState.Result = DataLinkResultEnum.InvalidProfileName
                        Exit Try
                    End If

                    ' El destino no puede estar en blanco
                    If Me.strDestination = "" Then
                        oState.Result = DataLinkResultEnum.InvalidDestination
                        Exit Try
                    End If

                    ' El nombre del fichero destino no puede estar en blanco
                    If Me.strExportFileName = "" Then
                        oState.Result = DataLinkResultEnum.InvalidExportFileName
                        Exit Try
                    End If

                    ' La hora de inicio no puede estar en blanco
                    If roReportSchedulerScheduleManager.retScheduleString(Me.oScheduler) = "" Then
                        oState.Result = DataLinkResultEnum.InvalidStartHour
                        Exit Try
                    End If

                    ' La hora de inicio tiene que ser correcta
                    If Not IsDate(Now.Year & "/" & Now.Month & "/" & Now.Day & " " & strStartExecutionHour) Then
                        oState.Result = DataLinkResultEnum.InvalidStartHour
                        Exit Try
                    End If

                    ' El día de inicio de cálculo no puede estar en blanco
                    If Me.intStartCalculDay = 0 Or Me.intStartCalculDay > 31 Then
                        oState.Result = DataLinkResultEnum.InvalidStartCalculDay
                        Exit Try
                    End If

                    ' No se puede configurar en un dia de calculo y aplicando fecha de cierre
                    If Me.strAutomaticDatePeriod = String.Empty And Me.bApplyLockDate Then
                        oState.Result = DataLinkResultEnum.InvalidMode
                        Exit Try
                    End If
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roExportGuide::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roExportGuide::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene el siguiente ID disponible para dar de alta un nueva guia
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        Public Function GetNextID() As Integer

            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# MAX(ID) FROM ExportGuides"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet + 1

        End Function

        ''' <summary>
        ''' Borra la guia siempre y cuando no se use.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Try
                bolRet = False

                'Borramos la guia
                Dim DelQuerys() As String = {"@DELETE# FROM ExportGuides WHERE ID = " & Me.ID.ToString}
                For n As Integer = 0 To DelQuerys.Length - 1
                    If Not ExecuteSql(DelQuerys(n)) Then
                        oState.Result = DataLinkResultEnum.ConnectionError
                        Exit For
                    End If
                Next

                bolRet = (oState.Result = DataLinkResultEnum.NoError)

                'If bolRet And bAudit Then
                '    '' Auditamos
                '    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAssignment, Me.strName, Nothing, -1, oTrans.Connection)
                'End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roExportGuide::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roExportGuide::Delete")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve todas las plantillas disponibles de la carpeta datalink con el texto de la hoja de calculo
        ''' </summary>
        ''' <param name="ProfileMask"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTemplatesByProfileMask(ByVal ProfileMask As String, idPassport As Integer) As DataTable
            Dim Profiles As String = ""
            Dim dt As New DataTable("Profiles")

            Try
                Dim pass As roPassportTicket = roPassportManager.GetPassportTicket(idPassport)
                Dim idLang As Integer = 0
                ' Determina el idioma
                Select Case pass.Language.Key.ToUpper
                    Case "ESP" : idLang = 1
                    Case "CAT" : idLang = 2
                    Case "ENG" : idLang = 3
                End Select

                ' Crea la tabla
                dt.Columns.Add("Name", GetType(String))
                dt.Columns.Add("Description", GetType(String))
                dt.Columns.Add("NeedTemplate", GetType(Boolean))

                ' Determina el directorio datatalink
                Dim oSettings As New roSettings()
                Dim targetDirectory As String = oSettings.GetVTSetting(eKeys.DataLink)

                ' Lee todos los ficheros que empiecen con el nombre solicitado
                Dim fileEntries As String() = Directory.GetFiles(targetDirectory)

                ' Process the list of files found in the directory.
                Dim fileName As String = ""
                Dim r As DataRow = Nothing
                Dim Descript As String = ""

                If fileEntries IsNot Nothing Then
                    For Each fileName In fileEntries
                        ' Si empieza segun el nombre solicitado lo añade a la lista
                        If InStr(fileName.ToUpper, ProfileMask.ToUpper) Then
                            r = dt.NewRow
                            If fileName.Length - targetDirectory.Length - 1 = ProfileMask.Length Then
                                r("Name") = ProfileMask
                            Else
                                r("Name") = fileName.Substring(targetDirectory.Length + 1, fileName.Length - targetDirectory.Length - 1)
                            End If

                            ' Lee la descripcion de la plantilla
                            Dim ExcelProfile As New ExcelExport(fileName)
                            Dim idSheet As Integer = ExcelProfile.GetIDSheet("Hoja1")
                            Descript = ExcelProfile.GetCellValue(idLang, 10)

                            ' Si la hoja no tiene description se asigna el nombre del fichero
                            If Descript = "" Then Descript = r("Name")
                            r("Description") = Descript

                            Dim bNeedTemplate As Boolean = False

                            Dim firstRow As Integer = 4
                            Dim cellValue As String = ExcelProfile.GetCellValue(firstRow, 1)
                            While cellValue <> String.Empty

                                If cellValue.ToUpper.StartsWith("SALDO_FECHA") And Not cellValue.ToUpper.StartsWith("SALDO_FECHA_") Or
                                   cellValue.ToUpper.StartsWith("SALDO_V") And Not cellValue.ToUpper.StartsWith("SALDO_V_") Or
                                   cellValue.ToUpper.StartsWith("SALDO_NC") And Not cellValue.ToUpper.StartsWith("SALDO_NC_") Or
                                   cellValue.ToUpper.StartsWith("SALDO_EXPORTARCOMO") And Not cellValue.ToUpper.StartsWith("SALDO_EXPORTARCOMO_") Or
                                   cellValue.ToUpper.StartsWith("SALDO_TIPO") And Not cellValue.ToUpper.StartsWith("SALDO_TIPO_") Or
                                   cellValue.ToUpper.StartsWith("SALDO_DISPONIBILIDAD") And Not cellValue.ToUpper.StartsWith("SALDO_DISPONIBILIDAD_") Then
                                    bNeedTemplate = True
                                End If

                                r("NeedTemplate") = bNeedTemplate
                                If bNeedTemplate Then Exit While

                                firstRow = firstRow + 1
                                cellValue = ExcelProfile.GetCellValue(firstRow, 1)

                            End While

                            dt.Rows.Add(r)
                        End If

                    Next fileName
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roExportGuide::GetTemplatesByProfileMask")

            End Try

            Return dt
        End Function

#End Region

        Public Shared Function GetExportTemplates(ByVal ostate As roDataLinkState) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                Dim oSettings As New roSettings()
                Dim strPathFile As String = oSettings.GetVTSetting(eKeys.DataLink) & "\ExportGuides.rar"

                If File.Exists(strPathFile) Then
                    arrFile = File.ReadAllBytes(strPathFile)
                End If
            Catch ex As Exception
                ostate.UpdateStateInfo(ex, "roExportGuide::GetExportTemplates")
            Finally
            End Try

            Return arrFile

        End Function

        Public Shared Function GetNextExportGuideId(ByVal MinID As Integer, ByVal MaxID As Integer, Optional ByVal _Audit As Boolean = False) As Integer
            Dim intRet As Integer = 0
            Dim strSQL As String = "@SELECT# MAX(ID) FROM ExportGuides WHERE ID > " & MinID & " and ID < " & MaxID
            Dim tb As DataTable = CreateDataTable(strSQL)

            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If
            If intRet = 0 Then
                intRet = 15000
            End If
            Return intRet + 1

        End Function

    End Class

End Namespace