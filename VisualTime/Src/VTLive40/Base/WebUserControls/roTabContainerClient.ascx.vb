Partial Class base_WebUserControls_roTabContainerClient
    Inherits System.Web.UI.UserControl

    Private strEventTabClick As String = ""

    Public Property onEventTabClick() As String
        Get
            Return strEventTabClick
        End Get
        Set(ByVal value As String)
            strEventTabClick = value
        End Set
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabTitle1() As PlaceHolder
        Get
            Return Me.tabTitle01
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabTitle2() As PlaceHolder
        Get
            Return Me.tabTitle02
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabTitle3() As PlaceHolder
        Get
            Return Me.tabTitle03
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabTitle4() As PlaceHolder
        Get
            Return Me.tabTitle04
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabTitle5() As PlaceHolder
        Get
            Return Me.tabTitle05
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabTitle6() As PlaceHolder
        Get
            Return Me.tabTitle06
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabTitle7() As PlaceHolder
        Get
            Return Me.tabTitle07
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabTitle8() As PlaceHolder
        Get
            Return Me.tabTitle08
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabTitle9() As PlaceHolder
        Get
            Return Me.tabTitle09
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabContainer1() As PlaceHolder
        Get
            Return Me.tabContainer01
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabContainer2() As PlaceHolder
        Get
            Return Me.tabContainer02
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabContainer3() As PlaceHolder
        Get
            Return Me.tabContainer03
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabContainer4() As PlaceHolder
        Get
            Return Me.tabContainer04
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabContainer5() As PlaceHolder
        Get
            Return Me.tabContainer05
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabContainer6() As PlaceHolder
        Get
            Return Me.tabContainer06
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabContainer7() As PlaceHolder
        Get
            Return Me.tabContainer07
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabContainer8() As PlaceHolder
        Get
            Return Me.tabContainer08
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TabContainer9() As PlaceHolder
        Get
            Return Me.tabContainer09
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public Property TabVisible(ByVal tabIndex As Integer) As Boolean
        Get
            Select Case tabIndex
                Case 1 : Return (Me.tdTitle1.Style("display") = "none")
                Case 2 : Return (Me.tdTitle2.Style("display") = "none")
                Case 3 : Return (Me.tdTitle3.Style("display") = "none")
                Case 4 : Return (Me.tdTitle4.Style("display") = "none")
                Case 5 : Return (Me.tdTitle5.Style("display") = "none")
                Case 6 : Return (Me.tdTitle6.Style("display") = "none")
                Case 7 : Return (Me.tdTitle7.Style("display") = "none")
                Case 8 : Return (Me.tdTitle8.Style("display") = "none")
                Case 9 : Return (Me.tdTitle9.Style("display") = "none")
                Case Else : Return False
            End Select

        End Get
        Set(ByVal value As Boolean)
            Select Case tabIndex
                Case 1 : Me.tdTitle1.Style("display") = IIf(value, "", "none")
                Case 2 : Me.tdTitle2.Style("display") = IIf(value, "", "none")
                Case 3 : Me.tdTitle3.Style("display") = IIf(value, "", "none")
                Case 4 : Me.tdTitle4.Style("display") = IIf(value, "", "none")
                Case 5 : Me.tdTitle5.Style("display") = IIf(value, "", "none")
                Case 6 : Me.tdTitle6.Style("display") = IIf(value, "", "none")
                Case 7 : Me.tdTitle7.Style("display") = IIf(value, "", "none")
                Case 8 : Me.tdTitle8.Style("display") = IIf(value, "", "none")
                Case 9 : Me.tdTitle9.Style("display") = IIf(value, "", "none")
            End Select
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If tabTitle01.Controls.Count = 0 Then Me.tdTitle1.Style("display") = "none"
            If tabTitle02.Controls.Count = 0 Then Me.tdTitle2.Style("display") = "none"
            If tabTitle03.Controls.Count = 0 Then Me.tdTitle3.Style("display") = "none"
            If tabTitle04.Controls.Count = 0 Then Me.tdTitle4.Style("display") = "none"
            If tabTitle05.Controls.Count = 0 Then Me.tdTitle5.Style("display") = "none"
            If tabTitle06.Controls.Count = 0 Then Me.tdTitle6.Style("display") = "none"
            If tabTitle07.Controls.Count = 0 Then Me.tdTitle7.Style("display") = "none"
            If tabTitle08.Controls.Count = 0 Then Me.tdTitle8.Style("display") = "none"
            If tabTitle09.Controls.Count = 0 Then Me.tdTitle9.Style("display") = "none"

            tab01.Attributes("onclick") = "activeTabContainer('" & Me.ClientID & "',0);"
            tab02.Attributes("onclick") = "activeTabContainer('" & Me.ClientID & "',1);"
            tab03.Attributes("onclick") = "activeTabContainer('" & Me.ClientID & "',2);"
            tab04.Attributes("onclick") = "activeTabContainer('" & Me.ClientID & "',3);"
            tab05.Attributes("onclick") = "activeTabContainer('" & Me.ClientID & "',4);"
            tab06.Attributes("onclick") = "activeTabContainer('" & Me.ClientID & "',5);"
            tab07.Attributes("onclick") = "activeTabContainer('" & Me.ClientID & "',6);"
            tab08.Attributes("onclick") = "activeTabContainer('" & Me.ClientID & "',7);"
            tab09.Attributes("onclick") = "activeTabContainer('" & Me.ClientID & "',8);"

            If strEventTabClick <> "" Then tab01.Attributes("onclick") &= strEventTabClick & "('" & Me.ClientID & "',0);"
            If strEventTabClick <> "" Then tab02.Attributes("onclick") &= strEventTabClick & "('" & Me.ClientID & "',1);"
            If strEventTabClick <> "" Then tab03.Attributes("onclick") &= strEventTabClick & "('" & Me.ClientID & "',2);"
            If strEventTabClick <> "" Then tab04.Attributes("onclick") &= strEventTabClick & "('" & Me.ClientID & "',3);"
            If strEventTabClick <> "" Then tab05.Attributes("onclick") &= strEventTabClick & "('" & Me.ClientID & "',4);"
            If strEventTabClick <> "" Then tab06.Attributes("onclick") &= strEventTabClick & "('" & Me.ClientID & "',5);"
            If strEventTabClick <> "" Then tab07.Attributes("onclick") &= strEventTabClick & "('" & Me.ClientID & "',6);"
            If strEventTabClick <> "" Then tab08.Attributes("onclick") &= strEventTabClick & "('" & Me.ClientID & "',7);"
            If strEventTabClick <> "" Then tab09.Attributes("onclick") &= strEventTabClick & "('" & Me.ClientID & "',8);"

            If hdnActiveTab.Value = "" Then hdnActiveTab.Value = "0"

            tab01.Attributes("class") = "tabHeader"
            tab02.Attributes("class") = "tabHeader"
            tab03.Attributes("class") = "tabHeader"
            tab04.Attributes("class") = "tabHeader"
            tab05.Attributes("class") = "tabHeader"
            tab06.Attributes("class") = "tabHeader"
            tab07.Attributes("class") = "tabHeader"
            tab08.Attributes("class") = "tabHeader"
            tab09.Attributes("class") = "tabHeader"
            tbC01.Style("display") = "none"
            tbC02.Style("display") = "none"
            tbC03.Style("display") = "none"
            tbC04.Style("display") = "none"
            tbC05.Style("display") = "none"
            tbC06.Style("display") = "none"
            tbC07.Style("display") = "none"
            tbC08.Style("display") = "none"
            tbC09.Style("display") = "none"

            Select Case hdnActiveTab.Value
                Case "0"
                    tab01.Attributes("class") = "tabHeader-Active"
                    tbC01.Style("display") = ""
                Case "1"
                    tab02.Attributes("class") = "tabHeader-Active"
                    tbC02.Style("display") = ""
                Case "2"
                    tab03.Attributes("class") = "tabHeader-Active"
                    tbC03.Style("display") = ""
                Case "3"
                    tab04.Attributes("class") = "tabHeader-Active"
                    tbC04.Style("display") = ""
                Case "4"
                    tab05.Attributes("class") = "tabHeader-Active"
                    tbC05.Style("display") = ""
                Case "5"
                    tab06.Attributes("class") = "tabHeader-Active"
                    tbC06.Style("display") = ""
                Case "6"
                    tab07.Attributes("class") = "tabHeader-Active"
                    tbC07.Style("display") = ""
                Case "7"
                    tab08.Attributes("class") = "tabHeader-Active"
                    tbC08.Style("display") = ""
                Case "8"
                    tab09.Attributes("class") = "tabHeader-Active"
                    tbC09.Style("display") = ""
            End Select
        Catch ex As Exception

        End Try
    End Sub

End Class