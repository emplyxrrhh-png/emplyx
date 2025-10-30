Public Class CContextMenuButton

    Private strID As String
    Private bolVisible As Boolean
    Private bolEnabled As Boolean

    Public Sub New(ByVal _ID As String, ByVal _Visible As Boolean, ByVal _Enabled As Boolean)
        Me.strID = _ID
        Me.bolVisible = _Visible
        Me.bolEnabled = _Enabled
    End Sub

    Public ReadOnly Property ID() As String
        Get
            Return Me.strID
        End Get
    End Property
    Public Property Visible() As Boolean
        Get
            Return Me.bolVisible
        End Get
        Set(ByVal value As Boolean)
            Me.bolVisible = value
        End Set
    End Property
    Public Property Enabled() As Boolean
        Get
            Return Me.bolEnabled
        End Get
        Set(ByVal value As Boolean)
            Me.bolEnabled = value
        End Set
    End Property

End Class

Public Class CContextMenu

    Private strMenuControlID As String
    Private strParentControlID As String

    Private lstButtons() As CContextMenuButton

    Public Sub New(ByVal _MenuControlID As String, ByVal _ParentControlID As String, ByVal strIDs As String)

        Me.strMenuControlID = _MenuControlID
        Me.strParentControlID = _ParentControlID

        If strIDs <> "" Then

            ReDim Me.lstButtons(strIDs.Split(";").Length - 1)

            For n As Integer = 0 To strIDs.Split(";").Length - 1
                Me.lstButtons(n) = New CContextMenuButton(strIDs.Split(";")(n), True, True)
            Next
        Else
            ReDim Me.lstButtons(-1)
        End If

    End Sub

    Public Property ButtonVisible(ByVal _ID As String) As Boolean
        Get
            For Each oButton As CContextMenuButton In Me.lstButtons
                If oButton.ID = _ID Then
                    Return oButton.Visible
                    Exit For
                End If
            Next
            Return False
        End Get
        Set(ByVal value As Boolean)
            For Each oButton As CContextMenuButton In Me.lstButtons
                If oButton.ID = _ID Then
                    oButton.Visible = value
                    Exit For
                End If
            Next
        End Set
    End Property
    Public Property ButtonEnabled(ByVal _ID As String) As Boolean
        Get
            For Each oButton As CContextMenuButton In Me.lstButtons
                If oButton.ID = _ID Then
                    Return oButton.Enabled
                    Exit For
                End If
            Next
            Return False
        End Get
        Set(ByVal value As Boolean)
            For Each oButton As CContextMenuButton In Me.lstButtons
                If oButton.ID = _ID Then
                    oButton.Enabled = value
                    Exit For
                End If
            Next
        End Set
    End Property

    Public Property Visible() As Boolean
        Get
            Dim bolRet As Boolean = False
            For Each oButton As CContextMenuButton In Me.lstButtons
                If oButton.Visible Then
                    bolRet = True
                    Exit For
                End If
            Next
            Return bolRet
        End Get
        Set(ByVal value As Boolean)
            For Each oButton As CContextMenuButton In Me.lstButtons
                oButton.Visible = value
            Next
        End Set
    End Property
    Public Property Enabled() As Boolean
        Get
            Dim bolRet As Boolean = True
            For Each oButton As CContextMenuButton In Me.lstButtons
                If oButton.Enabled Then
                    bolRet = True
                    Exit For
                End If
            Next
            Return bolRet
        End Get
        Set(ByVal value As Boolean)
            For Each oButton As CContextMenuButton In Me.lstButtons
                oButton.Enabled = value
            Next
        End Set
    End Property

    Public Function ButtonsIDs() As String
        Dim strRet As String = ""
        For Each oButton As CContextMenuButton In Me.lstButtons
            strRet &= oButton.ID & ";"
        Next
        If strRet <> "" Then strRet = strRet.Substring(0, strRet.Length - 1)
        Return strRet
    End Function

    Public Function ButtonsDisplay() As String
        Dim strRet As String = ""
        For Each oButton As CContextMenuButton In Me.lstButtons
            strRet &= IIf(oButton.Visible, "", "none") & ";"
        Next
        If strRet <> "" Then strRet = strRet.Substring(0, strRet.Length - 1)
        Return strRet
    End Function

    Public Function ButtonsEnabled() As String
        Dim strRet As String = ""
        For Each oButton As CContextMenuButton In Me.lstButtons
            strRet &= IIf(oButton.Enabled, "true", "false") & ";"
        Next
        If strRet <> "" Then strRet = strRet.Substring(0, strRet.Length - 1)
        Return strRet
    End Function

    Public Function Script() As String
        Dim strContextMenu As String = "return false;"
        If Me.Visible Then
            strContextMenu = String.Format("return showMenuCell('{0}', '{1}', '{2}', '{3}', '{4}');",
                                           Me.strMenuControlID,
                                           Me.ButtonsIDs(),
                                           Me.ButtonsDisplay(),
                                           Me.ButtonsEnabled(),
                                           Me.strParentControlID)
        End If
        Return strContextMenu
    End Function

End Class