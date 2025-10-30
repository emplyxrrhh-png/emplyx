Imports System.Drawing

Public Class clsPrinterLine

    Public Enum eFontFamily
        _Courier
        _Arial
        _Tahoma
    End Enum

    Private _Text As String = ""
    Private _FontFamily As eFontFamily
    Private _FontFamilyString As String = "Arial"
    Private _FontSize As Byte = 18
    Private _FontStyle As FontStyle = FontStyle.Regular
    Private _Font As Font = New Font(_FontFamilyString, _FontSize, _FontStyle)

    Private Property Text() As String
        Get
            Return _Text
        End Get
        Set(ByVal value As String)
            ParseText(value)
        End Set
    End Property

    Private Property FontFamily() As eFontFamily
        Get
            Return _FontFamily
        End Get
        Set(ByVal value As eFontFamily)
            _FontFamily = value
            Select Case _FontFamily
                Case eFontFamily._Arial
                    _FontFamilyString = "Arial"
                Case eFontFamily._Courier
                    _FontFamilyString = "Courier New"
                Case eFontFamily._Tahoma
                    _FontFamilyString = "Tahoma"
                Case Else
                    _FontFamilyString = "Arial"
            End Select
            _Font = New Font(_FontFamilyString, _FontSize, _FontStyle)
        End Set
    End Property

    Public Sub New(ByVal pText As String, Optional ByVal pFontSize As Byte = 18, Optional ByVal pFontStyle As FontStyle = FontStyle.Regular, Optional ByVal pFontFamily As eFontFamily = eFontFamily._Arial)
        FontFamily = pFontFamily
        _FontSize = pFontSize
        _FontStyle = pFontStyle

        ParseText(pText)
        _Font = New Font(_FontFamilyString, _FontSize, _FontStyle)
    End Sub

    Private Sub ParseText(ByVal Text As String)
        Try
            If Text.IndexOf("{") >= 0 Then
                Dim Style As String = Text.Substring(Text.IndexOf("{") + 1, Text.IndexOf("}") - Text.IndexOf("{") - 1)
                ParseStype(Style)
                _Text = Text.Split("}")(1)
            Else
                _Text = Text
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ParseStype(ByVal Style As String)
        Try

            For Each str As String In Style.Split(",")
                Try
                    Select Case str.Substring(0, 1)
                        Case "$"
                            _FontSize = Robotics.VTBase.roTypes.Any2Integer(str.Split("$")(1))
                        Case "&"
                            Select Case Robotics.VTBase.roTypes.Any2String(str.Split("&")(1)).ToLower
                                Case "b", "bold"
                                    _FontStyle = FontStyle.Bold
                                Case "i", "italic"
                                    _FontStyle = FontStyle.Italic
                                Case "r", "regular"
                                    _FontStyle = FontStyle.Regular
                                Case "s", "underline"
                                    _FontStyle = FontStyle.Underline
                                Case Else
                                    _FontStyle = FontStyle.Regular
                            End Select
                        Case "+"
                            Select Case Robotics.VTBase.roTypes.Any2String(str.Split("+")(1)).ToLower
                                Case "a", "arial"
                                    FontFamily = eFontFamily._Arial
                                Case "c", "courier"
                                    FontFamily = eFontFamily._Courier
                                Case "t", "tahoma"
                                    FontFamily = eFontFamily._Tahoma
                                Case Else
                                    FontFamily = eFontFamily._Arial
                            End Select
                    End Select
                Catch ex As Exception
                End Try

            Next
            _Font = New Font(_FontFamilyString, _FontSize, _FontStyle)
        Catch ex As Exception

        End Try

    End Sub

    Public Sub Print(ByRef Graph As Graphics, ByVal xPos As Integer, ByRef yPos As Integer)
        Try
            yPos += _Font.GetHeight(Graph)
            Graph.DrawString(_Text, _Font, Brushes.Black, xPos, yPos)
        Catch ex As Exception

        End Try
    End Sub

End Class