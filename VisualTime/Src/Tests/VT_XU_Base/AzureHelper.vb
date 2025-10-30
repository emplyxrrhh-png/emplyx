Imports System.IO
Imports System.Text

Public Class AzureHelper

    Public Property UploadPunchPhoto2AzureWasCalled As Boolean = False
    Public Property emailWasSended As Boolean = False

    Public Function UploadPunchPhoto2AzureSpy(ms As MemoryStream, result As Boolean)

        Robotics.Azure.Fakes.ShimRoAzureSupport.UploadPunchPhoto2AzureStreamStringBoolean =
                                             Function(stream As MemoryStream, fileName As String, isPunch As Boolean)
                                                 UploadPunchPhoto2AzureWasCalled = True
                                                 Return result
                                             End Function
    End Function

    Public Function GetAzureLogBookByReferenceStub(reference As String) As List(Of Robotics.Base.DTOs.roMessage)

        Robotics.Azure.Fakes.ShimRoAzureSupport.GetAzureLogBookByReferenceString =
                                             Function(ref As String)
                                                 Dim retLogBook As String() = New String() {"Message1", "Message2"}
                                                 Return retLogBook
                                             End Function

    End Function

    Public Function GetCompanySecretStub(compName As String) As String

        Robotics.Azure.Fakes.ShimRoAzureSupport.GetCompanySecretString =
                                             Function(companyName As String)
                                                 Return "CompanySecret"
                                             End Function

    End Function


    Public Function InitAzureConfiguration() As String

        Robotics.Azure.Fakes.ShimroServiceConfigurationRepository.AllInstances.GetServiceConfiguration = Function()
                                                                                                             Dim conf As Robotics.Base.DTOs.roServiceConfiguration = New Robotics.Base.DTOs.roServiceConfiguration()
                                                                                                             conf.storageconnectionstring = "DefaultEndpointsProtocol=https;AccountName=romtidi05storage;AccountKey=jZ/3PajpDepRHHUP/KHljpBlXElH4wiirLVwhUKm3Bctg58TkMsdaigPMYE+YlZUXtJHm5bGnEUuiCy0Kr8rZA==;EndpointSuffix=core.windows.net"
                                                                                                             conf.storageblobcontainer = "test"
                                                                                                             Return conf
                                                                                                         End Function

    End Function

    Public Function UploadStream2Blob()
        Robotics.Azure.Fakes.ShimRoAzureSupport.UploadStream2BlobStreamStringroLiveQueueTypesStringBoolean =
                                             Function(stream As MemoryStream, fileName As String, queueType As Robotics.Base.DTOs.roLiveQueueTypes, container As String, isPunch As Boolean)
                                                 Return True
                                             End Function
    End Function

    Public Function UploadStream2BlobInCompanyContainer()
        Robotics.Azure.Fakes.ShimRoAzureSupport.UploadStream2BlobInCompanyContainerStreamStringroLiveQueueTypesStringBoolean =
                                             Function(stream As MemoryStream, fileName As String, queueType As Robotics.Base.DTOs.roLiveQueueTypes, container As String, isPunch As Boolean)
                                                 Return True
                                             End Function
    End Function

    Public Function SendTaskToQueue()
        Robotics.Azure.Fakes.ShimRoAzureSupport.SendTaskToQueueInt32StringroLiveTaskTypesString =
                                             Function(priority As Integer, companyName As String, taskType As Robotics.Base.DTOs.roLiveTaskTypes, message As String)
                                                 If taskType = Robotics.Base.DTOs.roLiveTaskTypes.SendEmail Then
                                                     emailWasSended = True
                                                 End If
                                                 Return True
                                             End Function

    End Function

    Public Function GetFileSaSTokenWithURI()
        Robotics.Azure.Fakes.ShimRoAzureSupport.GetFileSaSTokenWithURIStringroLiveQueueTypesBooleanString =
                                             Function(fileName As String, queueType As Robotics.Base.DTOs.roLiveQueueTypes, isPunch As Boolean, container As String)
                                                 Return "SASToken"
                                             End Function
    End Function

    Public Function GetFileSizeFromAzure()
        Robotics.Azure.Fakes.ShimRoAzureSupport.GetFileSizeFromAzureStringroLiveQueueTypesBooleanString =
                                             Function(fileName As String, queueType As Robotics.Base.DTOs.roLiveQueueTypes, isPunch As Boolean, container As String)
                                                 Return 100
                                             End Function
    End Function

    Public Function ListFiles(Optional ByVal isEmpty As Boolean = False)
        Robotics.Azure.Fakes.ShimRoAzureSupport.ListFilesStringStringroLiveQueueTypesStringBooleanString =
                                             Function(blobName As String, extension As String, queueType As Robotics.Base.DTOs.roLiveQueueTypes, folder As String, fromCompanyContainer As Boolean, companyName As String)
                                                 blobName = blobName.Replace("_*", "")
                                                 Dim retList As String() = New String() {}
                                                 If Not isEmpty Then
                                                     retList = New String() {blobName + "_1." + extension, blobName + "_2." + extension}
                                                 End If

                                                 Return retList.ToList()
                                             End Function

    End Function

    Public Function GetDocumentFile()
        Robotics.Azure.Fakes.ShimRoAzureSupport.GetDocumentFileString =
                                             Function(fileName As String)
                                                 Dim content As Byte() = {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}

                                                 Return content
                                             End Function
    End Function

End Class