Imports System.Collections.Generic
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTRequests
Imports Robotics.Base.VTServiceApi
Imports Robotics.Base.VTWebLinks
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTPortal

    Public Class WebLinksHelper
        Public Shared Function GetAllPortalWebLinks(ByVal oState As roWebLinksManagerState) As roGenericResponse(Of List(Of roWebLink))
            Dim lrret As New roGenericResponse(Of List(Of roWebLink))
            Try
                Dim allWebLinks As List(Of roWebLink) = roWebLinksManager.GetAllWebLinks(oState)

                'Devolvemos solo los enlaces que se deben mostrar en el portal, ya sea DashBoard o Portal
                Dim filteredWebLinks As List(Of roWebLink) = allWebLinks.
                    Where(Function(link) link.ShowOnPortalDashboard OrElse link.ShowOnPortal).
                ToList()

                lrret.Value = filteredWebLinks
                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::WebLinksHelper::GetAllWebLinks")
                lrret.Value = Nothing
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function
    End Class

End Namespace