Imports DevExpress.XtraPrinting
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Absence
Imports Robotics.Base.VTBusiness.Incidence
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class DocumentCompany
    Inherits UserControlBase

    Public Sub New()

    End Sub

    Public Property IdAbsence() As Integer
        Get
            Return Session(Me.ClientID & "DocumentCompany_IdAbsence")
        End Get

        Set(ByVal value As Integer)
            Session(Me.ClientID & "DocumentCompany_IdAbsence") = value
        End Set
    End Property

    Public Property Forecast() As ForecastType
        Get
            Return Session(Me.ClientID & "DocumentCompanys_ForecastType")
        End Get

        Set(ByVal value As ForecastType)
            Session(Me.ClientID & "DocumentCompany_ForecastType") = value
        End Set
    End Property

    Public Property IdRelatedObject() As Integer
        Get
            Return Session(Me.ClientID & "DocumentCompany_IdEmployee")
        End Get

        Set(ByVal value As Integer)
            Session(Me.ClientID & "DocumentCompany_IdEmployee") = value
        End Set
    End Property

    Public Property Type() As DocumentType
        Get
            Return Session(Me.ClientID & "DocumentCompany_Type")
        End Get

        Set(ByVal value As DocumentType)
            Session(Me.ClientID & "DocumentCompany_Type") = value
        End Set
    End Property

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
        End If

        Me.btnDownloadAll.Visible = False

    End Sub

    Public Sub SetScope(idRelatedObject As Integer, typeCaller As DocumentType, ByVal idAbsence As Integer, ByVal eForecast As ForecastType)
        Me.IdAbsence = idAbsence
        Me.IdRelatedObject = idRelatedObject
        Me.Forecast = eForecast
        Type = typeCaller
    End Sub

    Public Sub GetDocumentsFromExternalCall(ByVal employees As String, filter As String, filterUser As String)
        Me.hdnEmployees.Value = employees
        Me.hdnFilter.Value = filter
        Me.hdnFilterUser.Value = filterUser
    End Sub

    Protected Sub CallbackSession_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles CallbackSessionP.Callback
        Dim result = String.Empty
        Dim strFieldsGrid As List(Of roDocument) = Nothing

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        If e.Parameter.ToUpperInvariant.StartsWith("GETINFOSELECTED") Then
            GetDocumentsFromExternalCall(hdnEmployees.Value, hdnFilter.Value, hdnFilterUser.Value)
            strFieldsGrid = DocumentsServiceMethods.GetAllDocumentEmployeesBySelection(Me.hdnEmployees.Value, Me.hdnFilter.Value, Me.hdnFilterUser.Value, Page, False)

            If strFieldsGrid IsNot Nothing Then
                For Each document In strFieldsGrid

                    'APA: Comentamos esta linea, se ha modificado el LoadDocument para que recupere el nombre del empleado en caso de tenerlo.
                    'document.EmployeeName = API.EmployeeServiceMethods.GetEmployeeName(Page, document.IdEmployee)

                    If document.IdDaysAbsence <> 0 Then
                        Dim state As New roProgrammedAbsenceState
                        Dim absence As New roProgrammedAbsence(document.IdDaysAbsence, state)

                        If absence.BeginDate IsNot Nothing Then
                            Dim Params As Generic.List(Of String)
                            Params = New Generic.List(Of String)
                            Params.Add(CDate(absence.BeginDate).ToShortDateString)
                            Params.Add(CDate(absence.RealFinishDate).ToShortDateString)
                            Params.Add(API.CausesServiceMethods.GetCauseByID(Me.Page, absence.IDCause, False).Name)
                            Params.Add(absence.Description)

                            document.Remarks = Me.Language.Translate("ProgrammedAbsence.Literal", Me.DefaultScope, Params)
                        Else
                            document.Remarks = ""
                        End If
                    End If

                    If document.IdHoursAbsence <> 0 Then
                        Dim state As New roProgrammedCauseState
                        Dim absence As New roProgrammedCause(document.IdHoursAbsence, state)

                        If absence.BeginDate IsNot Nothing Then
                            Dim Params As Generic.List(Of String)
                            Params = New Generic.List(Of String)

                            Params.Add(CDate(absence.BeginDate).ToShortDateString)
                            Params.Add(CDate(absence.FinishDate).ToShortDateString)
                            Params.Add(CDate(roTypes.Any2Time(absence.Duration).Value).ToShortTimeString)
                            Params.Add(API.CausesServiceMethods.GetCauseByID(Me.Page, absence.IDCause, False).Name)
                            Params.Add(absence.Description)
                            If (absence.BeginTime Is Nothing) Then
                                Params.Add("00:00")
                            Else
                                Params.Add(CDate(absence.BeginTime).ToShortTimeString)
                            End If
                            If (absence.EndTime Is Nothing) Then
                                Params.Add("23:59")
                            Else
                                Params.Add(CDate(absence.EndTime).ToShortTimeString)
                            End If
                            document.Remarks = Me.Language.Translate("ProgrammedCause.Literal", Me.DefaultScope, Params)
                        Else
                            document.Remarks = ""
                        End If
                    End If

                Next

            End If
        End If

        CallbackSessionP.JSProperties.Add("cpActionRO", "GETLABAGREESTARTUP")
        CallbackSessionP.JSProperties.Add("cpValues", strFieldsGrid)
        CallbackSessionP.JSProperties.Add("cpResultRO", result)

    End Sub

#Region "SELECTOR DE EMPLEADOS"

    Private Function GetInfoSelected() As String
        Dim strRet As String = String.Empty

        Try
            If Not String.IsNullOrEmpty(Me.hdnEmployees.Value) Then
                GetDocumentsFromExternalCall(hdnEmployees.Value, hdnFilter.Value, hdnFilterUser.Value)
            End If
        Catch
        End Try

        Return strRet

    End Function

#End Region

End Class