Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTDataLink.DataLink
Imports Robotics.ExternalSystems.DataLink

Public Class DataLinkBaseProxy
    Implements IDataLinkBaseSvc

    Public Function KeepAlive() As Boolean Implements IDataLinkBaseSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Devuelve un dataset con todas las guias de exportación
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetGuides(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IDataLinkBaseSvc.GetGuides
        Return DataLinkBaseMethods.GetGuides(oState)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con una guia de importacion concreta
    ''' </summary>
    ''' <param name="_ID"></param>
    ''' <param name="_Audit"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetImportGuide(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roImportGuide) Implements IDataLinkBaseSvc.GetImportGuide
        Return DataLinkBaseMethods.GetImportGuide(_ID, _Audit, oState)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con todas las guias de importación
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetImports(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IDataLinkBaseSvc.GetImports
        Return DataLinkBaseMethods.GetImports(oState)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con todas las guias de exportación
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetExports(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IDataLinkBaseSvc.GetExports
        Return DataLinkBaseMethods.GetExports(oState)
    End Function

    ''' <summary>
    ''' Obtiene la guia de extraccion  con el ID indicado
    ''' </summary>
    ''' <param name="IDGuide"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetGuideByID(ByVal IDGuide As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roGuide) Implements IDataLinkBaseSvc.GetGuideByID
        Return DataLinkBaseMethods.GetGuideByID(IDGuide, oState)
    End Function


    Public Function ImportEmployeeExcel(ByVal oExcelFileData() As Byte, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportEmployeeExcel
        Return DataLinkBaseMethods.ImportEmployeeExcel(oExcelFileData, oState)
    End Function


    Public Function ImportEmployeeAscii(ByVal oAsciiFileData() As Byte, ByVal oExcelFileData() As Byte, ByVal strSeparator As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportEmployeeAscii
        Return DataLinkBaseMethods.ImportEmployeeAscii(oAsciiFileData, oExcelFileData, strSeparator, oState)
    End Function


    Public Function ImportDailyCoverage(ByVal oAsciiFileData() As Byte, ByVal strSeparator As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportDailyCoverage
        Return DataLinkBaseMethods.ImportDailyCoverage(oAsciiFileData, strSeparator, oState)
    End Function


    Public Function ImportGroupExcel(ByVal oExcelFileData() As Byte, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportGroupExcel
        Return DataLinkBaseMethods.ImportGroupExcel(oExcelFileData, oState)
    End Function


    Public Function ImportTaskExcel(ByVal oExcelFileData() As Byte, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportTaskExcel
        Return DataLinkBaseMethods.ImportTaskExcel(oExcelFileData, oState)
    End Function


    Public Function ImportAssignmentsExcel(ByVal oExcelFileData() As Byte, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportAssignmentsExcel
        Return DataLinkBaseMethods.ImportAssignmentsExcel(oExcelFileData, oState)
    End Function


    Public Function ImportEmployeeSageMurano(ByVal oAsciiFileData() As Byte, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportEmployeeSageMurano
        Return DataLinkBaseMethods.ImportEmployeeSageMurano(oAsciiFileData, oState)
    End Function


    Public Function ImportCalendarAbsencesXml(ByVal oXMLFileData() As Byte, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportCalendarAbsencesXml
        Return DataLinkBaseMethods.ImportCalendarAbsencesXml(oXMLFileData, oState)
    End Function



    Public Function ImportCalendarAbsencesAscii(ByVal oAsciiFileData() As Byte, ByVal oExcelFileData() As Byte, ByVal strSeparator As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportCalendarAbsencesAscii
        Return DataLinkBaseMethods.ImportCalendarAbsencesAscii(oAsciiFileData, oExcelFileData, strSeparator, oState)
    End Function


    Public Function ImportEmployeeXml(ByVal strXmlData As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportEmployeeXml
        Return DataLinkBaseMethods.ImportEmployeeXml(strXmlData, oState)
    End Function


    Public Function ImportDailyCausesExcel(ByVal oExcelFileData() As Byte, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportDailyCausesExcel
        Return DataLinkBaseMethods.ImportDailyCausesExcel(oExcelFileData, oState)
    End Function


    Public Function ExecuteExtractionData(ByVal IDGuide As Integer, ByVal FilePath As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal mEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExecuteExtractionData
        Return DataLinkBaseMethods.ExecuteExtractionData(IDGuide, FilePath, BeginDate, EndDate, mEmployees, oState)
    End Function


    Public Function ExecuteExtractionDataStdXML(ByVal FilePath As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal mEmployees As String, ByVal ExportUsrFieldName As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExecuteExtractionDataStdXML
        Return DataLinkBaseMethods.ExecuteExtractionDataStdXML(FilePath, BeginDate, EndDate, mEmployees, ExportUsrFieldName, oState)
    End Function

    Public Function ExecuteExtractionDataTemplate(ByVal IDGuide As Integer, ByVal FilePath As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal mEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExecuteExtractionDataTemplate
        Return DataLinkBaseMethods.ExecuteExtractionDataTemplate(IDGuide, FilePath, BeginDate, EndDate, mEmployees, oState)
    End Function


    ''' <summary>
    ''' Importamos la planificación de empleados en base a una Hoja Excel con un formato específico
    ''' </summary>
    ''' <param name="BeginDate"></param>
    ''' <param name="EndDate"></param>
    ''' <param name="mEmployees"></param>
    ''' <param name="oExcelFileData"></param>
    ''' <param name="strMsg"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function ImportEmployeeScheduler(ByVal BeginDate As Date, ByVal EndDate As Date, ByVal mEmployees As String, ByVal oExcelFileData() As Byte, ByVal strMsg As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ImportEmployeeScheduler
        Return DataLinkBaseMethods.ImportEmployeeScheduler(BeginDate, EndDate, mEmployees, oExcelFileData, strMsg, oState)
    End Function

    ''' <summary>
    ''' Devuelve una lista generica de cadenas con todas las plantillas Excel de exportacion de datos existentes en la carpeta configurada
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns>ArrayList of elements with format: [NameToShow]#[TemplateFilePath]</returns>
    ''' <remarks></remarks>

    Public Function GetTemplatesExcel(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of String)) Implements IDataLinkBaseSvc.GetTemplatesExcel
        Return DataLinkBaseMethods.GetTemplatesExcel(oState)
    End Function

    ''' <summary>
    ''' Comprueba si tenemos una versión de excel que nos permita importar datos de xml
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns>Retorna Boolean indicando si la versión de excel es correcta</returns>
    ''' <remarks></remarks>

    Public Function CanExecuteExcelDataTemplate(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.CanExecuteExcelDataTemplate
        Return DataLinkBaseMethods.CanExecuteExcelDataTemplate(oState)
    End Function

    ''' <summary>
    ''' Comprueba si ya se ha exportado el periodo indicado
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns>Retorna Boolean indicando si el periodo ya se ha exportado</returns>
    ''' <remarks></remarks>

    Public Function ExistsExportPeriod(ByVal BeginPeriod As Date, ByVal EndPeriod As Date, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.ExistsExportPeriod
        Return DataLinkBaseMethods.ExistsExportPeriod(BeginPeriod, EndPeriod, oState)
    End Function


    ''' <summary>
    ''' Comprueba si tenemos Excel instalado
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function IsExcelInstalled(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.IsExcelInstalled
        Return DataLinkBaseMethods.IsExcelInstalled(oState)
    End Function

    ''' <summary>
    ''' Ejecuta la plantilla de Excel especificada. Antes genera un XML temporal.
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns>Retorna Boolean indicando exito de la exportacion</returns>
    ''' <remarks></remarks>

    Public Function ExecuteExtractionDataTemplateExcel(ByVal strTemplateFileName As String, ByVal strExportFileName As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal mEmployees As String, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExecuteExtractionDataTemplateExcel
        Return DataLinkBaseMethods.ExecuteExtractionDataTemplateExcel(strTemplateFileName, strExportFileName, BeginDate, EndDate, mEmployees, oState)
    End Function

    ''' <summary>
    ''' Guarda los datos de la guia de importación. Si és nuevo, se actualiza el ID de la guia pasado.<br/>
    ''' </summary>
    ''' <param name="oImportGuide">Puesto a guardar (roImportGuide)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar la guia.</returns>
    ''' <remarks></remarks>

    Public Function SaveImportGuide(ByVal oImportGuide As roImportGuide, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.SaveImportGuide
        Return DataLinkBaseMethods.SaveImportGuide(oImportGuide, oState, bAudit)
    End Function


    Public Function GetExportTemplates(ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.GetExportTemplates
        Return DataLinkBaseMethods.GetExportTemplates(oState)
    End Function


    Public Function ExcelExportAccrualsDaily(ByVal mEmployees As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal Is2007Version As Boolean, ByVal nIdExport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExcelExportAccrualsDaily
        Return DataLinkBaseMethods.ExcelExportAccrualsDaily(mEmployees, BeginDate, EndDate, Is2007Version, nIdExport, oState)
    End Function


    Public Function ExcelExportAccrualsPeriode(ByVal mEmployees As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal Is2007Version As Boolean, ByVal nIdExport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExcelExportAccrualsPeriode
        Return DataLinkBaseMethods.ExcelExportAccrualsPeriode(mEmployees, BeginDate, EndDate, Is2007Version, nIdExport, oState)
    End Function


    Public Function ExcelExportHolidays(ByVal mEmployees As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal Is2007Version As Boolean, ByVal bolExportShifts As Boolean, ByVal nIdExport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExcelExportHolidays
        Return DataLinkBaseMethods.ExcelExportHolidays(mEmployees, BeginDate, EndDate, Is2007Version, bolExportShifts, nIdExport, oState)
    End Function


    Public Function ExcelExportSchedule(ByVal mEmployees As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal Is2007Version As Boolean, ByVal nIdExport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExcelExportSchedule
        Return DataLinkBaseMethods.ExcelExportSchedule(mEmployees, BeginDate, EndDate, Is2007Version, nIdExport, oState)
    End Function


    Public Function ExportProfileAccrualsASCII(ByVal intIDExport As Integer, ByVal mEmployees As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal ConceptGroup As String, ByVal ExcelProfileName As String, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExportProfileAccrualsASCII
        Return DataLinkBaseMethods.ExportProfileAccrualsASCII(intIDExport, mEmployees, BeginDate, EndDate, ConceptGroup, ExcelProfileName, DelimiterChar, StartCalculDay, oState)
    End Function


    Public Function ExportProfileAccrualsEXCEL(ByVal IDExport As Integer, ByVal mEmployees As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal ConceptGroup As String, ByVal ExcelProfileName As String, ByVal OutputFileExcelIs2003 As Boolean, ByVal StartCalculDay As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExportProfileAccrualsEXCEL
        Return DataLinkBaseMethods.ExportProfileAccrualsEXCEL(IDExport, mEmployees, BeginDate, EndDate, ConceptGroup, ExcelProfileName, OutputFileExcelIs2003, StartCalculDay, oState)
    End Function


    Public Function ExportProfileDinnersASCII(ByVal IDExport As Integer, ByVal mEmployees As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal ExcelProfileName As String, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExportProfileDinnersASCII
        Return DataLinkBaseMethods.ExportProfileDinnersASCII(IDExport, mEmployees, BeginDate, EndDate, ExcelProfileName, DelimiterChar, StartCalculDay, oState)
    End Function


    Public Function ExportProfileDinnersEXCEL(ByVal IDExport As Integer, ByVal mEmployees As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal ExcelProfileName As String, ByVal OutputFileExcelIs2003 As Boolean, ByVal StartCalculDay As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExportProfileDinnersEXCEL
        Return DataLinkBaseMethods.ExportProfileDinnersEXCEL(IDExport, mEmployees, BeginDate, EndDate, ExcelProfileName, OutputFileExcelIs2003, StartCalculDay, oState)
    End Function


    Public Function ExportProfileProlonguedAbsencesASCII(ByVal IDExport As Integer, ByVal mEmployees As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal ExcelProfileName As String, ByVal DelimiterChar As String, ByVal StartCalculDay As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExportProfileProlonguedAbsencesASCII
        Return DataLinkBaseMethods.ExportProfileProlonguedAbsencesASCII(IDExport, mEmployees, BeginDate, EndDate, ExcelProfileName, DelimiterChar, StartCalculDay, oState)
    End Function


    Public Function ExportProfileProlonguedAbsencesEXCEL(ByVal IDExport As Integer, ByVal mEmployees As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal ExcelProfileName As String, ByVal OutputFileExcelIs2003 As Boolean, ByVal StartCalculDay As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IDataLinkBaseSvc.ExportProfileProlonguedAbsencesEXCEL
        Return DataLinkBaseMethods.ExportProfileProlonguedAbsencesEXCEL(IDExport, mEmployees, BeginDate, EndDate, ExcelProfileName, OutputFileExcelIs2003, StartCalculDay, oState)
    End Function

    ''' <summary>
    ''' Guarda los datos de la guia de exportacion. Si és nuevo, se actualiza el ID de la guia pasado.<br/>
    ''' </summary>
    ''' <param name="oExportGuide">Puesto a guardar (roExportGuide)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar la guia.</returns>
    ''' <remarks></remarks>

    Public Function SaveExportGuide(ByVal oExportGuide As roExportGuide, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IDataLinkBaseSvc.SaveExportGuide
        Return DataLinkBaseMethods.SaveExportGuide(oExportGuide, oState, bAudit)
    End Function



    Public Function GetNextExportGuideId(ByVal iMinRange As Integer, ByVal iMaxRange As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements IDataLinkBaseSvc.GetNextExportGuideId
        Return DataLinkBaseMethods.GetNextExportGuideId(iMinRange, iMaxRange, oState)
    End Function

    ''' <summary>
    ''' Devuelve todas las plantillas disponibles de la carpeta datalink del tipo solicitado
    ''' </summary>
    ''' <param name="ProfileMask"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetTemplatesByProfileMask(ByVal ProfileMask As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataTable) Implements IDataLinkBaseSvc.GetTemplatesByProfileMask
        Return DataLinkBaseMethods.GetTemplatesByProfileMask(ProfileMask, oState)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con una guia de importacion concreta
    ''' </summary>
    ''' <param name="_ID"></param>
    ''' <param name="_Audit"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetExportGuide(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roExportGuide) Implements IDataLinkBaseSvc.GetExportGuide
        Return DataLinkBaseMethods.GetExportGuide(_ID, _Audit, oState)
    End Function

End Class
