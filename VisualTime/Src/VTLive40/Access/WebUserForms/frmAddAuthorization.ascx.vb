Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class frmAddAuthorization
    Inherits UserControlBase

    'Public Property AvailableDates(Optional ByVal bolReload As Boolean = False) As DataTable
    '    Get
    '        Dim oLst = ViewState("frmAddAuthorization_AvailableDates")

    '        If oLst Is Nothing OrElse bolReload Then
    '            oLst = API.UserAdminServiceMethods.GetSupervisorPassports(Me.Page)
    '            ViewState("frmAddAuthorization_AvailableDates") = oLst
    '        End If
    '        Return oLst
    '    End Get
    '    Set(value As DataTable)
    '        ViewState("frmAddAuthorization_AvailableDates") = value
    '    End Set
    'End Property
    Public Property Authorizations(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim oLst = ViewState("frmAddAuthorization_Authorizations")

            If oLst Is Nothing OrElse bolReload Then
                oLst = AccessGroupServiceMethods.GetAccessGroups(Me.Page)

                ViewState("frmAddAuthorization_Authorizations") = oLst
            End If
            Return oLst
        End Get
        Set(value As DataTable)
            ViewState("frmAddAuthorization_Authorizations") = value
        End Set
    End Property

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Me.IsPostBack Then
            'AvailableDates = Nothing
            'Me.tbAvailableDates.Items.Clear()
            'Dim tmpDates = AvailableDates.Rows
            'If tmpDates IsNot Nothing Then
            '    For Each oVal As DataRow In tmpDates
            '        Me.tbAvailableDates.Items.Add(oVal("UserName"), oVal("UserID"))
            '    Next

            'End If
            Authorizations = Nothing
            Me.tbAuthorizations.Items.Clear()
            Dim tmpAuths = Authorizations.Rows
            If tmpAuths IsNot Nothing Then
                For Each oVal As DataRow In tmpAuths
                    Me.tbAuthorizations.Items.Add(oVal("Name"), oVal("ID"))
                Next

            End If
        End If

    End Sub

End Class