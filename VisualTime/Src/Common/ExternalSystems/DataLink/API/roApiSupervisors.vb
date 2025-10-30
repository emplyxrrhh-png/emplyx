Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security.Base

Namespace DataLink

    Public Class roApiSupervisors
        Inherits roDataLinkApi
        Protected ReadOnly Property ImportEngine As roSupervisorsImport
            Get
                Return CType(Me.oDataImport, roSupervisorsImport)
            End Get
        End Property


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)

            If Me.oDataImport Is Nothing Then
                Me.oDataImport = New roSupervisorsImport(DataLink.eImportType.IsCustom, "", Me.State)
            End If
        End Sub

        Public Function GetAllSupervisors(ByRef oSupervisors As Generic.List(Of roSupervisor), ByRef strErrorMsg As String, ByRef iReturnCode As RoboticsExternAccess.Core.DTOs.ReturnCode) As Boolean
            Dim bolRet As Boolean = False
            Try
                iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                oSupervisors = roPassportManager.GetAllSupervisors(Nothing, False).ToList
                iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roApiSupervisors::GetAllSupervisors")
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Function GetRoles() As List(Of roGroupFeature)
            Dim groupFeatures As List(Of roGroupFeature) = New List(Of roGroupFeature)()
            Try
                Dim bState = New roGroupFeatureState()
                roBusinessState.CopyTo(Me.State, bState)
                bState.UpdateStateInfo()

                Dim featuresArray = roGroupFeatureManager.GetGroupFeaturesList(bState)

                If featuresArray IsNot Nothing Then
                    groupFeatures = featuresArray.ToList()
                End If

                If bState.Result <> GroupFeatureResultEnum.NoError Then
                    Me.State.Result = DataLinkResultEnum.ErrorRecoveringData
                End If

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roApiSupervisors::GetRoles")
            End Try

            Return groupFeatures
        End Function

        Public Function GetCategories() As List(Of roSecurityCategory)
            Dim categories As List(Of roSecurityCategory) = Nothing
            Try
                Dim bState = New roSecurityCategoryState()
                roBusinessState.CopyTo(Me.State, bState)
                bState.UpdateStateInfo()

                categories = roSecurityCategoryManager.LoadSecurityCategories(bState)

                If bState.Result <> SecurityResultEnum.NoError Then
                    Me.State.Result = DataLinkResultEnum.ErrorRecoveringData
                End If

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roApiSupervisors::GetCategories")
            End Try

            Return If(categories IsNot Nothing, categories, New List(Of roSecurityCategory)())
        End Function

        Public Function CreateOrUpdateSupervisor(supervisorPassport As roPassport) As Boolean
            Dim passportManager As New roPassportManager
            passportManager.Save(supervisorPassport)
            Throw New NotImplementedException("CreateOrUpdateSupervisor method is not implemented.")
        End Function

        Public Function DeleteSupervisor(supervisorPassport As roPassport) As Boolean
            Dim passportManager As New roPassportManager
            passportManager.Delete(supervisorPassport)
            Throw New NotImplementedException("DeleteSupervisor method is not implemented.")
        End Function

    End Class

End Namespace