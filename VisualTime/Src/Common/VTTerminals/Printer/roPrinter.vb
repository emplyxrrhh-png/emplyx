Imports System.Drawing.Printing
Imports Robotics.VTBase

Namespace VTTerminals

    Public Class clsPrinter

        Private _Printer As PrintDocument
        Private _PrinterSetting As PrinterSettings

        Private _Lines As List(Of clsPrinterLine) = New List(Of clsPrinterLine)

        Public Sub New(ByVal PrinterName As String, ByVal oState As roTerminalsState)
            Try
                _Printer = New PrintDocument
                _PrinterSetting = New PrinterSettings

                If PrinterName.Length > 0 Then
                    _PrinterSetting.PrinterName = PrinterName
                End If

                'Dim margins As New Margins(clsParameters.PrinterMarginLeft, clsParameters.PrinterMarginLeft, clsParameters.PrinterMarginTop, clsParameters.PrinterMarginTop)
                Dim margins As New Margins(0, 0, 0, 0)
                _PrinterSetting.DefaultPageSettings.Margins = margins

                _Printer.PrinterSettings = _PrinterSetting

                AddHandler _Printer.PrintPage, AddressOf print_PrintPage
            Catch ex As Exception
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPrinter::New:Error:", ex)
            End Try

        End Sub

        Public Sub AddLine(ByVal Text As String, oState As roTerminalsState)
            Try
                _Lines.Add(New clsPrinterLine(Text))
            Catch ex As Exception
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPrinter::AddLine:Error:", ex)
            End Try
        End Sub

        Public Sub Print(oState As roTerminalsState)
            Try

                _Printer.Print()
            Catch ex As Exception
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPrinter::Print:Error:", ex)
            End Try
        End Sub

        Private Sub print_PrintPage(ByVal sender As Object, ByVal e As PrintPageEventArgs)
            Try

                'Dim xPos As Single = clsParameters.PrinterMarginLeft
                'Dim yPos As Single = clsParameters.PrinterMarginTop
                Dim xPos As Single = 0
                Dim yPos As Single = 0

                For Each oLine As clsPrinterLine In _Lines
                    oLine.Print(e.Graphics, xPos, yPos)
                Next

                e.HasMorePages = False
            Catch ex As Exception
                'oState.Log.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPrinter::print_PrintPage:Error:", ex)
            End Try

        End Sub

    End Class

End Namespace