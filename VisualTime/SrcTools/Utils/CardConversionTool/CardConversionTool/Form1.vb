Public Class Form1

    Dim oCardManager As CardManager

    Private Sub btnGetCardForBBDD_Click(sender As Object, e As EventArgs) Handles btnGetCardForBBDD.Click
        Dim _DriverCardCodification As CardManager.eCardCode = CardManager.eCardCode.Robotics
        Dim _TerminalCardCodification As CardManager.eCardCode = CardManager.eCardCode.Numeric
        Dim _CardType As CardManager.eCardType = CardManager.eCardType.Unique
        Select Case cmbDriverCodification.Text
            Case "Robotics"
                _DriverCardCodification = CardManager.eCardCode.Robotics
            Case "Ninguna"
                _DriverCardCodification = CardManager.eCardCode.Numeric
            Case "Hexa"
                _DriverCardCodification = CardManager.eCardCode.Hex
        End Select

        'TODO: Codificación de terminal, como en el caso de mx8
        Select Case cmbDriverCodification.Text

        End Select

        Select Case cmbCardType.Text
            Case "HID"
                _CardType = CardManager.eCardType.HID
            Case "MiFare"
                _CardType = CardManager.eCardType.Mifare
            Case "Unique"
                _CardType = CardManager.eCardType.Unique
        End Select

        oCardManager = New CardManager(CInt(txtBytesTerminalReader.Text), _DriverCardCodification, _CardType, _TerminalCardCodification)
        oCardManager.CommsReceivedCardCode = txtCardFromCommunications.Text
        txtToVTDatabase.Text = oCardManager.BBDDCode

    End Sub

    Private Sub btnGetCarForTerminal_Click(sender As Object, e As EventArgs) Handles btnGetCarForTerminal.Click
        Dim _DriverCardCodification As CardManager.eCardCode = CardManager.eCardCode.Robotics
        Dim _TerminalCardCodification As CardManager.eCardCode = CardManager.eCardCode.Numeric
        Dim _CardType As CardManager.eCardType = CardManager.eCardType.Unique
        Select Case cmbDriverCodification.Text
            Case "Robotics"
                _DriverCardCodification = CardManager.eCardCode.Robotics
            Case "Ninguna"
                _DriverCardCodification = CardManager.eCardCode.Numeric
            Case "Hexa"
                _DriverCardCodification = CardManager.eCardCode.Hex
        End Select

        'TODO: Codificación de terminal, como en el caso de mx8
        Select Case cmbDriverCodification.Text

        End Select

        Select Case cmbCardType.Text
            Case "HID"
                _CardType = CardManager.eCardType.HID
            Case "MiFare"
                _CardType = CardManager.eCardType.Mifare
            Case "Unique"
                _CardType = CardManager.eCardType.Unique
        End Select

        oCardManager = New CardManager(CInt(txtBytesTerminalReader.Text), _DriverCardCodification, _CardType, _TerminalCardCodification)
        oCardManager.BBDDCode = txtCardFromBBDD.Text
        txtToTerminal.Text = oCardManager.CommsReceivedCardCode
    End Sub
End Class
