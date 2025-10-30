Namespace Session

    Public Class roSessionList
        Inherits ArrayList

        Public Shadows Sub Add(ByVal item As roSession)
            MyBase.Add(item)
        End Sub

        Public Shadows Function Contains(ByVal SessionID As String) As Boolean
            Dim bolRet As Boolean = False
            For Each oItem As roSession In Me
                If oItem.ID = SessionID Then
                    bolRet = True
                    Exit For
                End If
            Next
            Return bolRet
        End Function

        Public Shadows Function Remove(ByVal SessionID As String) As Boolean
            Dim bolRet As Boolean = False
            For n As Integer = 0 To Me.Count - 1
                If MyBase.Item(n).ID = SessionID Then
                    bolRet = True
                    MyBase.RemoveAt(n)
                    Exit For
                End If
            Next
            Return bolRet
        End Function

        Public Shadows Function Item(ByVal SessionID As String) As roSession
            Dim oItem As roSession = Nothing
            For Each oItem In Me
                If oItem.ID = SessionID Then
                    Return oItem
                End If
            Next
            Return Nothing
        End Function

    End Class

End Namespace