Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTCommuniques
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTRequests
Imports Robotics.Security
Imports Robotics.VTBase

Namespace VTPortal

    Public Class CommuniqueHelper
        Public Shared Function GetMyCommuniques(ByVal idEmployee As Integer, ByRef oDocState As VTCommuniques.roCommuniqueState) As roCommuniqueListResponse
            Dim cList As New roCommuniqueListResponse

            Try
                Dim oManager As New roCommuniqueManager(oDocState)

                Dim oCommuniques As Generic.List(Of roCommunique) = oManager.GetAllCommuniques(idEmployee, False)

                cList.Communiques = oCommuniques.ToArray
                If oManager.State.Result = CommuniqueResultEnum.NoError Then
                    cList.oState.Result = ErrorCodes.OK
                Else
                    cList.Communiques = {}
                    cList.oState.Result = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                cList.Communiques = {}
                cList.oState.Result = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New Common.roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::CommuniquesHelper::GetMyCommuniques")
            End Try

            Return cList
        End Function
    End Class
End Namespace