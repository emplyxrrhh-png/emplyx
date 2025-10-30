Imports System.ComponentModel
Imports Robotics.Base.DTOs
Imports Robotics.ExternalSystems
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports Xunit

Namespace Unit.Test

    <Collection("CTAIMA")>
    <CollectionDefinition("CTAIMA", DisableParallelization:=True)>
    <Category("CTAIMA")>
    Public Class CTAIMATest

        Private ReadOnly helperAdvancedParameters As AdvancedParametersHelper
        Private ReadOnly helperEmployees As EmployeeHelper
        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperDataLayer As DatalayerHelper
        Private ReadOnly helperBusiness As BusinessHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperAdvancedParameters = New AdvancedParametersHelper()
            helperEmployees = New EmployeeHelper()
            helperPassport = New PassportHelper()
            helperDataLayer = New DatalayerHelper()
            helperBusiness = New BusinessHelper()
        End Sub

        <Fact(DisplayName:="Should Set Entra As N If User Documentation Is Valid Until Yesterday")>
        Sub ShouldSetEntraAsNIfUserDocumentationIsValidUntilYesterday()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"CTaimaTenantId", "1234"}, {"CTaimaApiVersion", "1"}, {"CTaimaOcmApimSubscriptionKey", "1"}})
                helperEmployees.GetEmployeesStub()
                helperEmployees.GetEmployeeUserFieldValueAtDateStub(New Dictionary(Of String, String) From {{"CTaimaPRLUserField", ""}})
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperEmployees.UserFieldLoadStub()
                helperEmployees.SaveEmployeeUserFieldSpy()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDataLayer, passport)
                System.Net.Http.Fakes.ShimHttpClient.AllInstances.GetAsyncString = Function(a, b)
                                                                                       Dim response As System.Net.Http.HttpResponseMessage = New System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Accepted)
                                                                                       Dim content As Net.Http.HttpContent = New Net.Http.StringContent("test")
                                                                                       response.Content = content
                                                                                       Return Task.FromResult(response)
                                                                                   End Function
                System.Net.Http.Fakes.ShimHttpContent.AllInstances.ReadAsStringAsync = Function(a) Task.FromResult("[{""dueDate"": """ & Date.Now.Date.AddDays(-1) & """, ""status"":""authorized""}]")

                'Act
                Dim ctaimaSystem As CTAIMA.CTAIMASystem = New CTAIMA.CTAIMASystem()
                ctaimaSystem.SyncDataRestful()

                'Assert
                Assert.Equal(helperEmployees.EntraUserFieldSave, "N")

            End Using
        End Sub

        <Fact(DisplayName:="Should Set Entra As S If User Documentation Is Valid Until Today")>
        Sub ShouldSetEntraAsSIfUserDocumentationIsValidUntilToday()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"CTaimaTenantId", "1234"}, {"CTaimaApiVersion", "1"}, {"CTaimaOcmApimSubscriptionKey", "1"}})
                helperEmployees.GetEmployeesStub()
                helperEmployees.GetEmployeeUserFieldValueAtDateStub(New Dictionary(Of String, String) From {{"CTaimaPRLUserField", ""}})
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperEmployees.UserFieldLoadStub()
                helperEmployees.SaveEmployeeUserFieldSpy()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDataLayer, passport)
                System.Net.Http.Fakes.ShimHttpClient.AllInstances.GetAsyncString = Function(a, b)
                                                                                       Dim response As System.Net.Http.HttpResponseMessage = New System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Accepted)
                                                                                       Dim content As Net.Http.HttpContent = New Net.Http.StringContent("test")
                                                                                       response.Content = content
                                                                                       Return Task.FromResult(response)
                                                                                   End Function
                System.Net.Http.Fakes.ShimHttpContent.AllInstances.ReadAsStringAsync = Function(a) Task.FromResult("[{""dueDate"": """ & Date.Now.Date & """, ""status"":""authorized""}]")
                CommonHelper.InitTask()
                'Act
                Dim ctaimaSystem As CTAIMA.CTAIMASystem = New CTAIMA.CTAIMASystem()
                ctaimaSystem.SyncDataRestful()

                'Assert
                Assert.Equal(helperEmployees.EntraUserFieldSave, "S")

            End Using
        End Sub

        <Fact(DisplayName:="Should Set Entra As S If User Documentation Is Valid Until Tomorrow")>
        Sub ShouldSetEntraAsSIfUserDocumentationIsValidUntilTomorrow()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"CTaimaTenantId", "1234"}, {"CTaimaApiVersion", "1"}, {"CTaimaOcmApimSubscriptionKey", "1"}})
                helperEmployees.GetEmployeesStub()
                helperEmployees.GetEmployeeUserFieldValueAtDateStub(New Dictionary(Of String, String) From {{"CTaimaPRLUserField", ""}})
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperEmployees.UserFieldLoadStub()
                helperEmployees.SaveEmployeeUserFieldSpy()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDataLayer, passport)
                System.Net.Http.Fakes.ShimHttpClient.AllInstances.GetAsyncString = Function(a, b)
                                                                                       Dim response As System.Net.Http.HttpResponseMessage = New System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Accepted)
                                                                                       Dim content As Net.Http.HttpContent = New Net.Http.StringContent("test")
                                                                                       response.Content = content
                                                                                       Return Task.FromResult(response)
                                                                                   End Function
                System.Net.Http.Fakes.ShimHttpContent.AllInstances.ReadAsStringAsync = Function(a) Task.FromResult("[{""dueDate"": """ & Date.Now.Date.AddDays(1) & """, ""status"":""authorized""}]")
                CommonHelper.InitTask()
                'Act
                Dim ctaimaSystem As CTAIMA.CTAIMASystem = New CTAIMA.CTAIMASystem()
                ctaimaSystem.SyncDataRestful()

                'Assert
                Assert.Equal(helperEmployees.EntraUserFieldSave, "S")

            End Using
        End Sub

        <Fact(DisplayName:="Should Set Entra As N If User Is Not Authorized By CTAIMA")>
        Sub ShouldSetEntraAsNIfUserIsNotAuthorizedByCTAIMA()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"CTaimaTenantId", "1234"}, {"CTaimaApiVersion", "1"}, {"CTaimaOcmApimSubscriptionKey", "1"}})
                helperEmployees.GetEmployeesStub()
                helperEmployees.GetEmployeeUserFieldValueAtDateStub(New Dictionary(Of String, String) From {{"CTaimaPRLUserField", ""}})
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperEmployees.UserFieldLoadStub()
                helperEmployees.SaveEmployeeUserFieldSpy()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDataLayer, passport)
                System.Net.Http.Fakes.ShimHttpClient.AllInstances.GetAsyncString = Function(a, b)
                                                                                       Dim response As System.Net.Http.HttpResponseMessage = New System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Accepted)
                                                                                       Dim content As Net.Http.HttpContent = New Net.Http.StringContent("test")
                                                                                       response.Content = content
                                                                                       Return Task.FromResult(response)
                                                                                   End Function
                System.Net.Http.Fakes.ShimHttpContent.AllInstances.ReadAsStringAsync = Function(a) Task.FromResult("[{""dueDate"": """ & Date.Now.Date.AddDays(-1) & """, ""status"":""authorized""}]")
                CommonHelper.InitTask()
                'Act
                Dim ctaimaSystem As CTAIMA.CTAIMASystem = New CTAIMA.CTAIMASystem()
                ctaimaSystem.SyncDataRestful()

                'Assert
                Assert.Equal(helperEmployees.EntraUserFieldSave, "N")

            End Using
        End Sub

        <Fact(DisplayName:="Should Set Entra As S If User Is Not Authorized In One Company But Is Authorized In Another Company")>
        Sub ShouldSetEntraAsSIfUserIsNotAuthorizedInOneCompanyButIsAuthorizedInAnotherCompany()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"CTaimaTenantId", "1234"}, {"CTaimaApiVersion", "1"}, {"CTaimaOcmApimSubscriptionKey", "1"}})
                helperEmployees.GetEmployeesStub()
                helperEmployees.GetEmployeeUserFieldValueAtDateStub(New Dictionary(Of String, String) From {{"CTaimaPRLUserField", ""}})
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperEmployees.UserFieldLoadStub()
                helperEmployees.SaveEmployeeUserFieldSpy()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDataLayer, passport)
                System.Net.Http.Fakes.ShimHttpClient.AllInstances.GetAsyncString = Function(a, b)
                                                                                       Dim response As System.Net.Http.HttpResponseMessage = New System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Accepted)
                                                                                       Dim content As Net.Http.HttpContent = New Net.Http.StringContent("test")
                                                                                       response.Content = content
                                                                                       Return Task.FromResult(response)
                                                                                   End Function
                System.Net.Http.Fakes.ShimHttpContent.AllInstances.ReadAsStringAsync = Function(a) Task.FromResult("[{""dueDate"": """ & Date.Now.Date.AddDays(-1) & """, ""status"":""authorized""},{""dueDate"": """ & Date.Now.Date.AddYears(1) & """, ""status"":""authorized""}]")
                CommonHelper.InitTask()
                'Act
                Dim ctaimaSystem As CTAIMA.CTAIMASystem = New CTAIMA.CTAIMASystem()
                ctaimaSystem.SyncDataRestful()

                'Assert
                Assert.Equal(helperEmployees.EntraUserFieldSave, "S")

            End Using
        End Sub

        <Fact(DisplayName:="Should Set Entra As S If User Is Authorized In Only One Company By CTAIMA")>
        Sub ShouldSetEntraAsSIfUserIsAuthorizedInOnlyOneCompanyByCTAIMA()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"CTaimaTenantId", "1234"}, {"CTaimaApiVersion", "1"}, {"CTaimaOcmApimSubscriptionKey", "1"}})
                helperEmployees.GetEmployeesStub()
                helperEmployees.GetEmployeeUserFieldValueAtDateStub(New Dictionary(Of String, String) From {{"CTaimaPRLUserField", ""}})
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperEmployees.UserFieldLoadStub()
                helperEmployees.SaveEmployeeUserFieldSpy()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDataLayer, passport)
                System.Net.Http.Fakes.ShimHttpClient.AllInstances.GetAsyncString = Function(a, b)
                                                                                       Dim response As System.Net.Http.HttpResponseMessage = New System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Accepted)
                                                                                       Dim content As Net.Http.HttpContent = New Net.Http.StringContent("test")
                                                                                       response.Content = content
                                                                                       Return Task.FromResult(response)
                                                                                   End Function
                System.Net.Http.Fakes.ShimHttpContent.AllInstances.ReadAsStringAsync = Function(a) Task.FromResult("[{""dueDate"": """ & Date.Now.Date.AddYears(1) & """, ""status"":""authorized""}]")
                CommonHelper.InitTask()
                'Act
                Dim ctaimaSystem As CTAIMA.CTAIMASystem = New CTAIMA.CTAIMASystem()
                ctaimaSystem.SyncDataRestful()

                'Assert
                Assert.Equal(helperEmployees.EntraUserFieldSave, "S")

            End Using
        End Sub

    End Class

End Namespace