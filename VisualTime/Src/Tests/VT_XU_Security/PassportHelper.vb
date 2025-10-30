Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.UsersAdmin
Imports VT_XU_Base

Public Class PassportHelper

    Public Property PhotoRequiered As Boolean = False
    Public Property LocationRequiered As Boolean = False
    Public Property UpdatePassportCalled As Boolean

    Public Property EnabledVTDesktop As Boolean = False

    Public Property EnabledVTPortal As Boolean = False

    Public Property EnabledVTPortalApp As Boolean = False

    Public Property EnabledVTVisits As Boolean = False

    Public Property LoginWithoutContract As Boolean = False

    Public Property UpdatePassportPhotoRequieredCalled As Boolean = False
    Public Property UpdatePassportLocationRequieredCalled As Boolean = False
    Public Property SavedRole As Integer = -1

    Public Property SavedCategories As roPassportCategories = Nothing

    Public Property SavedGroups As roPassportGroups = Nothing

    Public Property SetTimezone As Boolean = False

    Function PassportStub(ByVal idPassport As Integer, datalayerhelper As DatalayerHelper, Optional ByVal pass As roPassport = Nothing)

        Robotics.Security.Base.Fakes.ShimroPassportManager.GetPassportInt32LoadTyperoSecurityStateRef =
            Function()
                Dim oPassport As New roPassport() With {
                                    .ID = idPassport,
                                    .Name = If(pass IsNot Nothing, pass.Name, $"Usuario {idPassport}"),
                                    .Description = "",
                                    .AuthenticationMethods = If(pass IsNot Nothing, pass.AuthenticationMethods, Nothing),
                                    .Language = New roPassportLanguage() With {
                                            .Key = "ESP",
                                            .ID = 1
                                        },
                                        .IsSupervisor = If(pass IsNot Nothing, pass.IsSupervisor, False),
                                        .IDGroupFeature = If(pass IsNot Nothing, pass.IDGroupFeature, Nothing),
                                        .Groups = If(pass IsNot Nothing, pass.Groups, Nothing)
                                }

                UpdatePassportPhotoRequieredCalled = False
                UpdatePassportLocationRequieredCalled = False
                Return oPassport
            End Function


        Robotics.Security.Base.Fakes.ShimroPassportManager.AllInstances.LoadPassportInt32LoadTypeBoolean =
            Function(oPassportManager As roPassportManager, ByVal _ID As Integer, ByVal loadType As LoadType, ByVal bAudit As Boolean)
                If _ID = -1 AndAlso UpdatePassportCalled = False Then Return Nothing
                Dim oRetPassport As New roPassport() With {
                                    .ID = idPassport,
                                    .Name = If(pass IsNot Nothing, pass.Name, $"Usuario {idPassport}"),
                                    .Description = "",
                                    .Language = New roPassportLanguage() With {
                                            .Key = "ESP",
                                            .ID = 1
                                        }
                                }
                UpdatePassportPhotoRequieredCalled = False
                UpdatePassportLocationRequieredCalled = False
                Return oRetPassport
            End Function


        Robotics.Security.Base.Fakes.ShimroPassportManager.GetPassportTicketInt32LoadTyperoSecurityStateRef =
            Function()
                Dim oPassport As New roPassportTicket() With {
                                    .ID = idPassport,
                                    .Name = If(pass IsNot Nothing, pass.Name, $"Usuario {idPassport}"),
                                    .Description = "",
                                    .Language = New roPassportLanguage() With {
                                            .Key = "ESP",
                                            .ID = 1
                                        }
                                }
                UpdatePassportPhotoRequieredCalled = False
                UpdatePassportLocationRequieredCalled = False
                Return oPassport
            End Function


        Robotics.Security.Base.Fakes.ShimroPassportManager.AllInstances.LoadPassportTicketInt32LoadTypeBoolean =
            Function()
                Dim oPassport As New roPassportTicket() With {
                                    .ID = idPassport,
                                    .Name = If(pass IsNot Nothing, pass.Name, $"Usuario {idPassport}"),
                                    .Description = "",
                                    .Language = New roPassportLanguage() With {
                                            .Key = "ESP",
                                            .ID = 1
                                        }
                                }
                UpdatePassportPhotoRequieredCalled = False
                UpdatePassportLocationRequieredCalled = False
                Return oPassport
            End Function

    End Function

    Function Update()
        Robotics.Security.Base.Fakes.ShimroPassportManager.AllInstances.SaveroPassportRefBooleanBoolean =
            Function()
                UpdatePassportCalled = True
                Return True
            End Function
    End Function

    Function InitPassportCredentials(ByVal fakePassport As roPassportTicket)
        Robotics.Security.Base.Fakes.ShimroPassportManager.ValidateCredentialsAuthenticationMethodStringStringRefBooleanStringBooleanroSecurityStateRef =
            Function()
                Return fakePassport
            End Function

        Robotics.Security.Base.Fakes.ShimAuthHelper.AuthenticateroPassportTicketAuthenticationMethodStringStringRefBooleanroSecurityStateRefBooleanStringStringStringBooleanStringStringRefBoolean =
            Function()
                Return fakePassport
            End Function
    End Function

    Function LoginProcessSpys()
        Robotics.Security.Base.Fakes.ShimroPassportManager.SetTimeZoneDataroAppTypeInt32StringroSecurityStateRef =
            Function()
                SetTimezone = True
            End Function
    End Function


    Function UpdatePassportPhotoRequiered()
        Robotics.Base.DTOs.Fakes.ShimroPassport.AllInstances.PhotoRequieredSetBoolean =
            Function(instance As roPassport, value As Boolean)
                PhotoRequiered = value
                UpdatePassportPhotoRequieredCalled = True
            End Function
    End Function

    Function UpdatePassportLocationRequiered()
        Robotics.Base.DTOs.Fakes.ShimroPassport.AllInstances.LocationRequieredSetBoolean =
            Function(instance As roPassport, value As Boolean)
                LocationRequiered = value
                UpdatePassportLocationRequieredCalled = True
            End Function
    End Function

    Function UpdatePassportEnabledVTDesktop()
        Robotics.Base.DTOs.Fakes.ShimroPassport.AllInstances.EnabledVTDesktopSetBoolean =
            Function(instance As roPassport, value As Boolean)
                EnabledVTDesktop = value
            End Function
    End Function

    Function UpdatePassportEnabledVTPortal()
        Robotics.Base.DTOs.Fakes.ShimroPassport.AllInstances.EnabledVTPortalSetBoolean =
            Function(instance As roPassport, value As Boolean)
                EnabledVTPortal = value
            End Function
    End Function

    Function UpdatePassportEnabledVTPortalApp()
        Robotics.Base.DTOs.Fakes.ShimroPassport.AllInstances.EnabledVTPortalAppSetBoolean =
            Function(instance As roPassport, value As Boolean)
                EnabledVTPortalApp = value
            End Function
    End Function

    Function UpdatePassportEnabledVTVisits()
        Robotics.Base.DTOs.Fakes.ShimroPassport.AllInstances.EnabledVTVisitsSetBoolean =
            Function(instance As roPassport, value As Boolean)
                EnabledVTVisits = value
            End Function
    End Function

    Function UpdatePassportLoginWithoutContract()
        Robotics.DataLayer.Fakes.ShimAccessHelper.ExecuteSqlStringroBaseConnection =
            Function(sql As String, roBaseConnection As Robotics.DataLayer.roBaseConnection)
                If sql.ToLower().Contains("@update# sysropassports set loginwithoutcontract=") Then

                    Dim startIndex As Integer = sql.IndexOf("LoginWithoutContract=")

                    If startIndex >= 0 Then
                        startIndex += "LoginWithoutContract=".Length

                        Dim endIndex As Integer = sql.IndexOf(" ", startIndex)

                        If endIndex = -1 Then
                            endIndex = sql.Length
                        End If

                        Dim loginWithoutContractValue As String = sql.Substring(startIndex, endIndex - startIndex)
                        If loginWithoutContractValue = 1 Then
                            LoginWithoutContract = True
                        Else LoginWithoutContract = False
                        End If
                    End If

                End If
            End Function
    End Function

    Function SavePassportSpy()
        Robotics.Security.Base.Fakes.ShimroPassportManager.AllInstances.SaveroPassportRefBooleanBoolean =
            Function(a As roPassportManager, ByRef oPassport As roPassport, bAudit As Boolean, bolLaunchSecurityTask As Boolean)
                SavedRole = oPassport.IDGroupFeature
                SavedCategories = oPassport.Categories
                SavedGroups = oPassport.Groups
                Return True
            End Function
    End Function

    Function GetPassportsByRole(Optional ByVal oPassport As roPassport = Nothing)
        Robotics.Security.Base.Fakes.ShimroPassportManager.AllInstances.GetPassportsByRoleNullableOfInt32roSecurityState =
            Function(a As roPassportManager, ByVal idRole As Nullable(Of Integer), ByVal _State As roSecurityState)
                If oPassport Is Nothing Then
                    Return New List(Of Integer)() From {}
                Else
                    Return New List(Of Integer)() From {oPassport.ID}
                End If
            End Function
    End Function

    Sub GetFeatureListRoles(lstGroupFeatures As roGroupFeature())

        Robotics.Security.Base.Fakes.ShimroGroupFeatureManager.GetGroupFeaturesListroGroupFeatureStateRef = Function(ByRef oState As roGroupFeatureState)
                                                                                                                Return lstGroupFeatures
                                                                                                            End Function

    End Sub


    Sub GetPassportFeaturesStub(lstFeatures As Generic.List(Of Feature))

        Robotics.UsersAdmin.Business.Fakes.ShimFeaturesBusiness.GetFeaturesFromPassportAllInt32StringroSecurityStateRef = Function(a As Integer, b As String, ByRef c As Robotics.Security.Base.roSecurityState)
                                                                                                                              Return lstFeatures
                                                                                                                          End Function

    End Sub


End Class