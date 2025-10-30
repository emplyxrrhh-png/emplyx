Imports System.ServiceModel.Activation
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessMove
Imports Robotics.Base.VTBusiness.Common

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Required)>
Public Class AccessMoveProxy
    Implements IAccessMoveSvc

    Public Function KeepAlive() As Boolean Implements IAccessMoveSvc.KeepAlive
        Return True
    End Function
    '=========================================
    '============= CUBO ======================
    '=========================================
    Public Function GetAccessPlatesViewsDataSet(ByVal IdPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IAccessMoveSvc.GetAccessPlatesViewsDataSet
        Return AccessMoveMethods.GetAccessPlatesViewsDataSet(IdPassport, oState)

    End Function

    Public Function GetAccessPlatesViewbyID(ByVal ID As Integer, ByVal IdPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IAccessMoveSvc.GetAccessPlatesViewbyID
        Return AccessMoveMethods.GetAccessPlatesViewbyID(ID, IdPassport, oState)
    End Function

    Public Function DeleteAccessPlatesView(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IAccessMoveSvc.DeleteAccessPlatesView
        Return AccessMoveMethods.DeleteAccessPlatesView(intID, oState)

    End Function

    Public Function NewAccessPlatesView(ByVal IdView As Integer, ByVal IdPassport As Integer, ByVal NameView As String, ByVal Description As String, ByVal DateView As DateTime,
                                      ByVal Employees As String, ByVal DateInf As DateTime, ByVal DateSup As DateTime, ByVal CubeLayout As String, ByVal FilterData As String,
                                      ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IAccessMoveSvc.NewAccessPlatesView
        Return AccessMoveMethods.NewAccessPlatesView(IdView, IdPassport, NameView, Description, DateView, Employees, DateInf, DateSup, CubeLayout, FilterData, oState)
    End Function

End Class
