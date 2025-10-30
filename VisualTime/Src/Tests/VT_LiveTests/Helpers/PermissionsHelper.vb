Imports Robotics.Base.DTOs
Imports Robotics.Security
Imports ServiceApi

Public Class PermissionsHelper

    Public Sub New()
    End Sub

    Public Sub StubSetCompany(ByVal companyName As String, ByVal companyInfo As Dictionary(Of String, String))
        Robotics.Azure.Fakes.ShimRoAzureSupport.GetCompanyName = Function()
                                                                     Return companyName
                                                                 End Function

        Robotics.DataLayer.Fakes.ShimroCacheManager.AllInstances.GetCompanyInfoObject = Function(reference, idCompany)

                                                                                            If (companyInfo.ContainsKey(idCompany)) Then
                                                                                                Return New roCompanyInfo() With {
                                                                                                .code = companyInfo(idCompany),
                                                                                                .name = "Company " & companyInfo(idCompany)
                                                                                                }
                                                                                            Else
                                                                                                Return Nothing
                                                                                            End If

                                                                                        End Function

    End Sub

    Public Sub StubSetIdentity(ByVal idPassport As Integer, ByVal guid As String, ByVal auth As String)

        Robotics.Web.Base.Fakes.ShimWLHelperWeb.CurrentPassportIDGet = Function()
                                                                           Return idPassport
                                                                       End Function

        Robotics.Web.Base.Fakes.ShimWLHelperWeb.CurrentPassportIDSetInt32 = Function()
                                                                            End Function

        Robotics.Web.Base.Fakes.ShimWLHelperWeb.CurrentPassportGetBoolean = Function()
                                                                                If idPassport > 0 Then
                                                                                    Return New roPassportTicket() With {
                                                                                            .ID = idPassport,
                                                                                            .Language = New roPassportLanguage() With {.Key = "ESP"}
                                                                                        }
                                                                                Else
                                                                                    Return Nothing
                                                                                End If

                                                                            End Function

        Robotics.Web.Base.Fakes.ShimroWsUserManagement.CurrentPassportGUIDGet = Function()
                                                                                    Return guid
                                                                                End Function

        Robotics.Web.Base.Fakes.ShimWLHelperWeb.AuthTokenGet = Function()
                                                                   Return auth
                                                               End Function

        Robotics.Web.Base.Fakes.ShimWLHelperWeb.AuthTokenSetString = Function(value)

                                                                     End Function
    End Sub

    'Allowed features format: {"feature#featuretype#permission","Employees#U#3","Calendar#U#9"}
    'Allowed employees format: {"idemployee#idpassport","1#1","2#0"}
    Public Sub SetFeatureAndUsersPermissions(ByVal idPassport As Integer, allowedFeatures As String(), allowedEmployees As String(), allowedCompanies As Integer())

        Robotics.Web.Base.API.Fakes.ShimSecurityServiceMethods.HasPermissionOverEmployeePageInt32StringStringPermission =
            Function(reference, idEmployee, feature, featureType, permission)
                Dim bHasPermission As Boolean = False

                For Each oPerm As String In allowedFeatures
                    Dim featureDesc As String() = oPerm.Split("#")

                    If featureDesc(0).ToLower = feature.ToLower And featureDesc(1).ToLower = featureType.ToLower And [Enum].Parse(GetType(Robotics.Base.DTOs.Permission), featureDesc(2), True) >= permission Then
                        bHasPermission = True
                        Exit For
                    End If
                Next

                If bHasPermission Then

                    For Each oCheckEmp As String In allowedEmployees
                        Dim employeeDesc As String() = oCheckEmp.Split("#")

                        If employeeDesc(0) = idEmployee.ToString Then
                            Return True
                        End If
                    Next

                    Return False
                Else
                    Return False
                End If
            End Function

        Robotics.Web.Base.API.Fakes.ShimSecurityServiceMethods.HasPermissionOverGroupAppAliasPageInt32StringStringPermission =
            Function(reference, idGroup, feature, featureType, permission)
                Dim bHasPermission As Boolean = False

                For Each oPerm As String In allowedFeatures
                    Dim featureDesc As String() = oPerm.Split("#")

                    If featureDesc(0).ToLower = feature.ToLower And featureDesc(1).ToLower = featureType.ToLower And [Enum].Parse(GetType(Robotics.Base.DTOs.Permission), featureDesc(2), True) >= permission Then
                        bHasPermission = True
                        Exit For
                    End If
                Next

                If bHasPermission Then
                    If allowedCompanies.Contains(idGroup) Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            End Function

        Robotics.Web.Base.API.Fakes.ShimSecurityServiceMethods.HasPermissionOverFeaturePageStringStringPermission =
            Function(reference, feature, featureType, permission)
                Dim bHasPermission As Boolean = False

                For Each oPerm As String In allowedFeatures
                    Dim featureDesc As String() = oPerm.Split("#")

                    If featureDesc(0).ToLower = feature.ToLower And featureDesc(1).ToLower = featureType.ToLower And [Enum].Parse(GetType(Robotics.Base.DTOs.Permission), featureDesc(2), True) >= permission Then
                        bHasPermission = True
                        Exit For
                    End If
                Next

                Return bHasPermission
            End Function

        Robotics.Web.Base.API.Fakes.ShimSecurityServiceMethods.UpdateLastAccessTimeMVCPage =
            Function(reference)
                Return True
            End Function

    End Sub

    Public Sub InitAvailablePassports(allowedEmployees As String())
        Robotics.Web.Base.API.Fakes.ShimUserAdminServiceMethods.GetPassportPageInt32LoadTypeBooleanBooleanroPassportTicket =
            Function(reference, idpassport, type, decrypt, excludestate, passporticket)
                For Each oCheckEmp As String In allowedEmployees
                    Dim employeeDesc As String() = oCheckEmp.Split("#")

                    If employeeDesc(1) = idpassport.ToString Then
                        Return New roPassport() With {.ID = CInt(employeeDesc(1)), .IDEmployee = CInt(employeeDesc(0))}
                    End If
                Next

                Return Nothing
            End Function
    End Sub

End Class