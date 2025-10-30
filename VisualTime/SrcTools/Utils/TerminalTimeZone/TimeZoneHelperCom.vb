Imports Robotics.Base.VTBusiness.Terminal

<ComClass(TimeZoneHelperCom.ClassId, TimeZoneHelperCom.InterfaceId, TimeZoneHelperCom.EventsId)> _
Public Class TimeZoneHelperCom

#Region "GUID de COM"
    ' Estos GUID proporcionan la identidad de COM para esta clase 
    ' y las interfaces de COM. Si las cambia, los clientes 
    ' existentes no podrán obtener acceso a la clase.
    Public Const ClassId As String = "95c85e6a-53d2-4bbd-92e6-388c64b1a1b4"
    Public Const InterfaceId As String = "c58116ab-a66b-40f0-b28f-9814ac54a018"
    Public Const EventsId As String = "9eeba118-1fb8-4ba3-8e28-55740a857ee2"
#End Region

    ' Una clase COM que se puede crear debe tener Public Sub New() 
    ' sin parámetros, si no la clase no se 
    ' registrará en el registro COM y no se podrá crear a 
    ' través de CreateObject.
    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Retorna el número de minutos de diferencia entre la hora del servidor y la que se debe programar en el terminal
    ''' </summary>
    ''' <param name="iIdTerminal"></param>
    ''' <param name="bError"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCurrentDateTimeOffset(ByVal iIdTerminal As Integer, ByRef bError As Boolean) As Integer

        Try
            bError = True

            Dim oTerm As New roTerminal
            Dim oState As New Robotics.Base.VTBusiness.Terminal.roTerminalState
            oTerm = New roTerminal(iIdTerminal, oState)

            ' Miramos si el terminal tiene una zona horaria distinta a la del servidor
            If oTerm.IsDifferentZoneTime Then
                Dim dat As DateTime = Now
                ' Obtenemos información de la zona horaria del terminal
                Dim oTerminalTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(oTerm.TimeZoneName)
                ' Combertimos la fecha actual del servidor a la zona horaria del terminal
                dat = TimeZoneInfo.ConvertTime(dat, TimeZoneInfo.Local, oTerminalTimeZone)

                If oTerminalTimeZone.SupportsDaylightSavingTime Then ' Si la zona horaria tiene horario de verano

                    ' Si el terminal no tiene que cambiar automáticamente la hora según el horario de verano y
                    'estamos en zona de verano en la zona horaria del terminal
                    If Not oTerm.AutoDaylight AndAlso oTerminalTimeZone.IsDaylightSavingTime(dat) Then
                        ' Restamos la diferencia del cambio de verano a la fecha
                        For Each oRule As TimeZoneInfo.AdjustmentRule In oTerminalTimeZone.GetAdjustmentRules()
                            If dat >= oRule.DateStart And dat <= oRule.DateEnd Then
                                dat = dat.AddMinutes(-1 * oRule.DaylightDelta.TotalMinutes)
                            End If
                        Next
                    End If

                End If

                Return dat.Subtract(Now).TotalMinutes
            Else
                Return 0
            End If
        Catch ex As Exception
            bError = False
            Return 0
        End Try
    End Function
End Class


