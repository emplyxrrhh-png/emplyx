Imports Robotics.Base.VTBusiness

Public Class roTerminalApiSession

    Public Enum roTerminalModel
        rxCP
        rxCeP
        rx1
        rxFP
        rxFPTD
        rxFL
        mxS
        rxFe
        rxTe
    End Enum

    Public Sub New()
        SN = String.Empty

    End Sub

    Public Property SN As String

    Public Property Terminal As Terminal.roTerminal

    'Public Property TerminalLogic As TerminalLogicZKPush2
    Public Property TerminalLogic As Object 'Puede ser de terminal PUSH o centralita InBio Pro

    Public Property CompanyId As String

    Public Property LastRequest As DateTime

    Public Property Model As roTerminalModel

    Public Property LastTerminalReload As DateTime

End Class