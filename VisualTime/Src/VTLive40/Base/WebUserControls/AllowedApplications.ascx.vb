Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Base_WebUserControls_AllowedApplications
    Inherits UserControlBase

    Private _exceptionCode As Integer

    Public Property Type() As LoadType
        Get
            Return Me.hdnType.Value
        End Get
        Set(ByVal value As LoadType)
            Me.hdnType.Value = value
        End Set
    End Property

    Public Property _ID() As Integer
        Get
            Return Me.hdnID.Value
        End Get
        Set(ByVal value As Integer)
            Me.hdnID.Value = value
        End Set
    End Property

    Public ReadOnly Property Exceptioncode() As Integer
        Get
            Return _exceptionCode
        End Get
    End Property

    Public Sub LoadData(ByVal PassportType As LoadType, ByVal intID As Integer, Optional ByRef _Passport As roPassport = Nothing)

        Me.Type = PassportType
        Me._ID = intID

        Dim oPassport As roPassport = _Passport
        If oPassport Is Nothing Then
            oPassport = UserAdminServiceMethods.GetPassport(Me.Page, Me._ID, Me.Type, True)
            _Passport = oPassport
        End If

        If roWsUserManagement.SessionObject.States.SecurityState.Result <> SecurityResultEnum.NoError Then
            _exceptionCode = roWsUserManagement.SessionObject.States.SecurityState.Result
        End If

        If oPassport IsNot Nothing Then
            Me.chkVisualTimeDesktop.Checked = oPassport.EnabledVTDesktop
            Me.chkVisualTimePortal.Checked = oPassport.EnabledVTPortal
            Me.chkVisualTimePortalApp.Checked = oPassport.EnabledVTPortalApp
            Me.chkVisualTimeVisites.Checked = oPassport.EnabledVTVisits
            Me.ckLocationEnabled.Checked = oPassport.LocationRequiered
            Me.ckPhotoEnabled.Checked = oPassport.PhotoRequiered
            Me.chkNoContract.Checked = oPassport.LoginWithoutContract
        End If

    End Sub

    Public Function SaveData(ByRef strErrorInfo As String) As Boolean
        Dim bolRet As Boolean = False

        Dim oPassport As roPassport = UserAdminServiceMethods.GetPassport(Me.Page, Me.ID, Me.Type)

        If oPassport IsNot Nothing Then

            ' Cargamos información de los métodos de identificación
            Me.LoadPassport(oPassport)
            ' Guardamos el passport
            bolRet = UserAdminServiceMethods.SavePassport(Me.Page, oPassport, False, True)
            If Not bolRet Then
                strErrorInfo = UserAdminServiceMethods.SecurityLastErrorText
            End If
        End If

        Return bolRet

    End Function

    ''' <summary>
    ''' Actualiza el passport con la información de los mètodos de autentificación actuales
    ''' </summary>
    ''' <param name="oPassport"></param>
    Public Sub LoadPassport(ByVal oPassport As roPassport)

        oPassport.EnabledVTDesktop = Me.chkVisualTimeDesktop.Checked
        oPassport.EnabledVTPortal = Me.chkVisualTimePortal.Checked
        oPassport.EnabledVTPortalApp = Me.chkVisualTimePortalApp.Checked
        oPassport.EnabledVTVisits = Me.chkVisualTimeVisites.Checked
        oPassport.LoginWithoutContract = Me.chkNoContract.Checked
        oPassport.LocationRequiered = Me.ckLocationEnabled.Checked
        oPassport.PhotoRequiered = Me.ckPhotoEnabled.Checked

    End Sub

    Public Sub SetEnabled(ByVal bolEnabled As Boolean)

        Me.chkVisualTimeDesktop.Disabled = (bolEnabled = False)
        Me.chkVisualTimePortal.Disabled = (bolEnabled = False)
        Me.chkVisualTimePortalApp.Disabled = (bolEnabled = False)
        Me.chkVisualTimeVisites.Disabled = (bolEnabled = False)
        Me.chkNoContract.Disabled = (bolEnabled = False)

        Me.ckLocationEnabled.Disabled = (bolEnabled = False)
        Me.ckPhotoEnabled.Disabled = (bolEnabled = False)

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cacheManager As New NoCachePageBase
        cacheManager.InsertExtraJavascript("AllowedApplications", "~/Base/Scripts/AllowedApplications.js", Me.Parent.Page)
    End Sub

End Class