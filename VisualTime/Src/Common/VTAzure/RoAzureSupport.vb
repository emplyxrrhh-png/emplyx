Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading.Tasks
Imports Azure
Imports Azure.Data.Tables
Imports Azure.Identity
Imports Azure.Messaging.ServiceBus
Imports Azure.Security.KeyVault.Secrets
Imports Azure.Storage.Blobs
Imports Azure.Storage.Blobs.Models
Imports Azure.Storage.Blobs.Specialized
Imports Azure.Storage.Queues
Imports Azure.Storage.Sas
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Class RoAzureSupport

    '
    ' Clase con funciones de propósito general de Azure
    '

#Region "Declarations - Constructor"

    Public Sub New()
    End Sub

#End Region

#Region "Service Bus messages"

    Public Shared Function SendTasksToQueueBatch(ByVal tasksHash As Hashtable, Optional iSendMaxMessages As Integer = 0) As Boolean
        Dim bRet As Boolean = True
        Try

            For Each oKey As Object In tasksHash.Keys
                Dim oTaskType As roLiveTaskTypes = CType(oKey, roLiveTaskTypes)
                Dim oTaskInfo As Hashtable = tasksHash(oTaskType)

                Dim oMessages As New Generic.List(Of ServiceBusMessage)
                Dim oMessage As ServiceBusMessage = Nothing

                For Each oCompanyTask As String In oTaskInfo.Keys
                    Select Case oTaskType
                        Case roLiveTaskTypes.AnalyticsTask, roLiveTaskTypes.GenerateAnalyticsTasks,
                             roLiveTaskTypes.ReportTaskDX, roLiveTaskTypes.GenerateReportsDxTasks,
                             roLiveTaskTypes.Export, roLiveTaskTypes.Import, roLiveTaskTypes.GenerateDatalinkTasks,
                             roLiveTaskTypes.SendNotifications, roLiveTaskTypes.GenerateNotifications,
                             roLiveTaskTypes.KeepAlive, roLiveTaskTypes.CacheControl, roLiveTaskTypes.PunchConnectorTask,
                             roLiveTaskTypes.BroadcasterTask, roLiveTaskTypes.RunEngine, roLiveTaskTypes.UpdateEngineCache,
                             roLiveTaskTypes.CTAIMA

                            oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(oCompanyTask & "@" & oTaskInfo(oCompanyTask).ToString()))
                        Case roLiveTaskTypes.RunEngineEmployee
                            oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(oCompanyTask.Split("@")(0) & "@" & oTaskInfo(oCompanyTask).ToString()))
                        Case Else
                            oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(oCompanyTask & "@" & oTaskInfo(oCompanyTask).ToString() & "@" & oTaskType.ToString))

                    End Select

                    oMessages.Add(oMessage)
                Next

                Dim oServiceRepository As roServiceConfigurationRepository = Nothing
                Select Case oTaskType
                    Case roLiveTaskTypes.AnalyticsTask, roLiveTaskTypes.GenerateAnalyticsTasks
                        oServiceRepository = New roServiceConfigurationRepository(roLiveQueueTypes.analytics)
                    Case roLiveTaskTypes.ReportTaskDX, roLiveTaskTypes.GenerateReportsDxTasks
                        oServiceRepository = New roServiceConfigurationRepository(roLiveQueueTypes.reports)
                    Case roLiveTaskTypes.Export, roLiveTaskTypes.Import, roLiveTaskTypes.GenerateDatalinkTasks, roLiveTaskTypes.CTAIMA
                        oServiceRepository = New roServiceConfigurationRepository(roLiveQueueTypes.datalink)
                    Case roLiveTaskTypes.SendNotifications, roLiveTaskTypes.GenerateNotifications
                        oServiceRepository = New roServiceConfigurationRepository(roLiveQueueTypes.notifications)
                    Case roLiveTaskTypes.KeepAlive
                        oServiceRepository = New roServiceConfigurationRepository(roLiveQueueTypes.eog)
                    Case roLiveTaskTypes.BroadcasterTask
                        oServiceRepository = New roServiceConfigurationRepository(roLiveQueueTypes.broadcaster)
                    Case roLiveTaskTypes.CacheControl
                        oServiceRepository = New roServiceConfigurationRepository(roLiveQueueTypes.scheduler)
                    Case roLiveTaskTypes.RunEngine, roLiveTaskTypes.RunEngineEmployee, roLiveTaskTypes.UpdateEngineCache
                        oServiceRepository = New roServiceConfigurationRepository(roLiveQueueTypes.engine)
                    Case roLiveTaskTypes.PunchConnectorTask
                        oServiceRepository = New roServiceConfigurationRepository(roLiveQueueTypes.connector)
                    Case Else
                        oServiceRepository = New roServiceConfigurationRepository(roLiveQueueTypes.eog)
                End Select

                Dim oConf As roServiceConfiguration = oServiceRepository.GetServiceConfiguration()

                If oConf IsNot Nothing AndAlso oConf.servicebusconnectionstring <> String.Empty Then
                    Dim oQueueClient As New ServiceBusClient(oConf.servicebusconnectionstring) ', oConf.servicebusqueuename, ReceiveMode.PeekLock)
                    Dim oQueueSender As ServiceBusSender = oQueueClient.CreateSender(oConf.servicebusqueuename)

                    Dim iCount As Integer = 0

                    Dim oSplitMessages As New List(Of ServiceBusMessage)

                    If iSendMaxMessages > 0 Then
                        If iSendMaxMessages > oMessages.Count Then iSendMaxMessages = oMessages.Count
                        oMessages = oMessages.Take(iSendMaxMessages).ToList()
                    End If

                    For Each oSplitMessage As ServiceBusMessage In oMessages

                        'oSplitMessage.

                        oSplitMessages.Add(oSplitMessage)
                        iCount = iCount + 1

                        If iCount = 150 Then
                            Task.Run(Async Function()
                                         Await oQueueSender.SendMessagesAsync(oSplitMessages)
                                     End Function).GetAwaiter().GetResult()

                            iCount = 0
                            oSplitMessages = New List(Of ServiceBusMessage)
                        End If
                    Next

                    If oSplitMessages IsNot Nothing AndAlso oSplitMessages.Count > 0 Then
                        Task.Run(Async Function()
                                     Await oQueueSender.SendMessagesAsync(oSplitMessages)
                                     Await oQueueClient.DisposeAsync()
                                 End Function).GetAwaiter().GetResult()
                    End If
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roAzureSupport::SendTasksToQueueBatch::" & oTaskType.ToString() & "::TaskCount::" & oMessages.Count & "::No Service Bus Configuration")
                End If
            Next
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roAzureSupport::SendTasksToQueueBatch::", ex)
            bRet = False
        End Try

        Return bRet
    End Function

    Public Shared Function SendEOGTasksBatch(ByVal messages As Generic.List(Of String), Optional iSendMaxMessages As Integer = 0) As Boolean
        Dim bRet As Boolean = True
        Try

            Dim oMessages As New Generic.List(Of ServiceBusMessage)
            Dim oMessage As ServiceBusMessage = Nothing

            For Each queueMessage As String In messages
                oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(queueMessage))

                oMessages.Add(oMessage)
            Next

            Dim oServiceRepository As roServiceConfigurationRepository = New roServiceConfigurationRepository(roLiveQueueTypes.eog)

            Dim oConf As roServiceConfiguration = oServiceRepository.GetServiceConfiguration()

            If oConf IsNot Nothing AndAlso oConf.servicebusconnectionstring <> String.Empty Then
                Dim oQueueClient As New ServiceBusClient(oConf.servicebusconnectionstring) ', oConf.servicebusqueuename, ReceiveMode.PeekLock)
                Dim oQueueSender As ServiceBusSender = oQueueClient.CreateSender(oConf.servicebusqueuename)

                Dim iCount As Integer = 0

                Dim oSplitMessages As New List(Of ServiceBusMessage)

                If iSendMaxMessages > 0 Then
                    If iSendMaxMessages > oMessages.Count Then iSendMaxMessages = oMessages.Count
                    oMessages = oMessages.Take(iSendMaxMessages).ToList()
                End If

                For Each oSplitMessage As ServiceBusMessage In oMessages

                    'oSplitMessage.

                    oSplitMessages.Add(oSplitMessage)
                    iCount = iCount + 1

                    If iCount = 150 Then
                        Task.Run(Async Function()
                                     Await oQueueSender.SendMessagesAsync(oSplitMessages)
                                 End Function).GetAwaiter().GetResult()

                        iCount = 0
                        oSplitMessages = New List(Of ServiceBusMessage)
                    End If
                Next

                If oSplitMessages IsNot Nothing AndAlso oSplitMessages.Count > 0 Then
                    Task.Run(Async Function()
                                 Await oQueueSender.SendMessagesAsync(oSplitMessages)
                                 Await oQueueClient.DisposeAsync()
                             End Function).GetAwaiter().GetResult()
                End If
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roAzureSupport::SendEOGTasksBatch::TaskCount::" & oMessages.Count & "::No Service Bus Configuration")
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roAzureSupport::SendEOGTasksBatch::", ex)
            bRet = False
        End Try

        Return bRet
    End Function


    Public Shared Function SendTaskToQueue(ByVal _IDTask As Integer, ByVal _Company As String, ByVal _Action As roLiveTaskTypes, Optional ByVal strMessageContent As String = "") As Boolean
        Dim bRet As Boolean = True
        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing
            Dim oMessage As ServiceBusMessage = Nothing
            Select Case _Action
                Case roLiveTaskTypes.AnalyticsTask
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.analytics).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(_Company & "@" & _IDTask.ToString))
                Case roLiveTaskTypes.ReportTaskDX
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.reports).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(_Company & "@" & _IDTask.ToString))
                Case roLiveTaskTypes.Export, roLiveTaskTypes.Import, roLiveTaskTypes.GenerateDatalinkTasks, roLiveTaskTypes.CTAIMA, roLiveTaskTypes.Suprema
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.datalink).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(_Company & "@" & _IDTask.ToString))
                Case roLiveTaskTypes.SendNotifications, roLiveTaskTypes.GenerateNotifications
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.notifications).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(_Company & "@" & _IDTask.ToString))
                Case roLiveTaskTypes.KeepAlive
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.eog).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(_Company & "@" & _IDTask.ToString))
                Case roLiveTaskTypes.BroadcasterTask
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.broadcaster).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(_Company & "@" & _IDTask.ToString))
                Case roLiveTaskTypes.CacheControl
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.scheduler).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(_Company & "@" & _IDTask.ToString))
                Case roLiveTaskTypes.SendEmail
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.email).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(_Company & "@" & _IDTask.ToString & "@" & CompressionHelper.CompressString(strMessageContent)))
                Case roLiveTaskTypes.SendPushNotification
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.pushnotifications).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(strMessageContent))
                Case roLiveTaskTypes.RunEngine, roLiveTaskTypes.RunEngineEmployee, roLiveTaskTypes.UpdateEngineCache
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.engine).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(_Company & "@" & _IDTask.ToString))
                Case roLiveTaskTypes.PunchConnectorTask
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.connector).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(_Company & "@" & _IDTask.ToString))
                Case Else
                    oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.eog).GetServiceConfiguration()
                    oMessage = New ServiceBusMessage(Encoding.UTF8.GetBytes(_Company & "@" & _IDTask.ToString & "@" & _Action.ToString))
            End Select

            If oServiceConfiguration IsNot Nothing AndAlso oServiceConfiguration.servicebusconnectionstring <> String.Empty AndAlso oMessage IsNot Nothing Then

                Dim oQueueClient As New ServiceBusClient(oServiceConfiguration.servicebusconnectionstring) ', oConf.servicebusqueuename, ReceiveMode.PeekLock)
                Dim oQueueSender As ServiceBusSender = oQueueClient.CreateSender(oServiceConfiguration.servicebusqueuename)

                Task.Run(Async Function()
                             Await oQueueSender.SendMessageAsync(oMessage)
                             Await oQueueClient.DisposeAsync()
                         End Function).GetAwaiter().GetResult()

            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roAzureSupport::SendTaskToQueue::" & _Action.ToString() & "::", ex)
            bRet = False
        End Try

        Return bRet
    End Function

#End Region

#Region "Storage messaging"

    Public Shared Function SendSMSMessage(ByVal message As String, ByVal storageConnectionString As String, ByVal smsQueueName As String) As Boolean
        'Dim queueClient As New QueueClient(storageConnectionString, smsQueueName)
        'queueClient.CreateIfNotExists()

        'queueClient.SendMessageAsync(message)

        Dim oQueueClient As New QueueServiceClient(storageConnectionString)
        Dim oQueue As QueueClient = oQueueClient.GetQueueClient(smsQueueName)

        oQueue.SendMessage(Convert.ToBase64String(Encoding.UTF8.GetBytes(message)))

        'Dim storageAccount As CloudStorageAccount = CloudStorageAccount.Parse(smsConnectionString)
        ''CloudConfigurationManager.GetSetting("StorageConnectionString")
        'Dim queueClient As CloudQueueClient = storageAccount.CreateCloudQueueClient()
        'Dim queue As CloudQueue = queueClient.GetQueueReference(smsQueueName)
        'queue.CreateIfNotExists()
        'Dim message As CloudQueueMessage = New CloudQueueMessage(strMessage)

        'queue.AddMessage(message)

    End Function

#End Region

#Region "Blob Storage"

    Public Shared Function ListFiles(strBlobNamePattern As String, strExtension As String, ByVal QueueType As roLiveQueueTypes, Optional folder As String = "", Optional bFromComanyContainer As Boolean = False, Optional strCompanyName As String = "") As List(Of String)
        Dim oRet As New List(Of String)
        Dim lSortedByLastModified As New SortedList(Of String, BlobItemProperties)
        Dim lSortedByName As New SortedList(Of String, String)
        Dim oServiceConfiguration As roServiceConfiguration = Nothing
        Dim ePatternType As roPatternTypes = roPatternTypes.none
        Try

            oServiceConfiguration = New roServiceConfigurationRepository(QueueType).GetServiceConfiguration()
            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)

            Dim strStorageContainer As String = oServiceConfiguration.storageblobcontainer
            If bFromComanyContainer Then strStorageContainer = strCompanyName.ToLower 'Los nombres de container sólo pueden contener minúsculas

            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(strStorageContainer)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            If (strBlobNamePattern.IndexOf("[") > 1 AndAlso strBlobNamePattern.IndexOf("]") > 1 AndAlso strBlobNamePattern.IndexOf("[") < strBlobNamePattern.IndexOf("]")) Then
                ePatternType = roPatternTypes.dateandtime
            ElseIf strBlobNamePattern.Split("*").Count > 1 Then
                ePatternType = roPatternTypes.asterisc
            End If

            Dim sPrefix As String = strBlobNamePattern
            Dim sSufix As String = strBlobNamePattern
            Dim sDatePattern As String = String.Empty

            Select Case ePatternType
                Case roPatternTypes.asterisc
                    sPrefix = strBlobNamePattern.Split("*")(0)
                    sSufix = strBlobNamePattern.Split("*")(1)
                Case roPatternTypes.dateandtime
                    sPrefix = strBlobNamePattern.Split("[")(0)
                    sSufix = strBlobNamePattern.Split("]")(1)
                    sDatePattern = strBlobNamePattern.Replace(sPrefix & "[", "").Replace("]" & sSufix, "")
            End Select

            Dim strPatternEx As String = sPrefix

            If folder.Length > 0 Then
                strPatternEx = folder & "/" & sPrefix
            End If

            Dim lBlobs = containerClient.GetBlobs(,, strPatternEx)
            Dim sBlobFileName As String

            For Each oBlob As BlobItem In lBlobs
                sBlobFileName = oBlob.Name.Replace(folder & "/", "").Replace("." & strExtension, "")
                Select Case ePatternType
                    Case roPatternTypes.asterisc, roPatternTypes.none
                        If sBlobFileName.StartsWith(sPrefix) AndAlso sBlobFileName.EndsWith(sSufix) AndAlso oBlob.Name.EndsWith("." & strExtension) Then
                            lSortedByLastModified.Add(oBlob.Name.Replace(folder & "/", ""), oBlob.Properties)
                        End If
                    Case roPatternTypes.dateandtime
                        If sBlobFileName.StartsWith(sPrefix) AndAlso sBlobFileName.EndsWith(sSufix) AndAlso oBlob.Name.EndsWith("." & strExtension) Then
                            Dim sDateTimeStamp As String = sBlobFileName
                            If sPrefix.Length > 0 Then sDateTimeStamp = sDateTimeStamp.Replace(sPrefix, "")
                            If sSufix.Length > 0 Then sDateTimeStamp = sDateTimeStamp.Replace(sSufix, "")
                            Dim dDateTimeStamp As DateTime
                            If DateTime.TryParseExact(sDateTimeStamp, sDatePattern, Nothing, Globalization.DateTimeStyles.None, dDateTimeStamp) Then
                                sDateTimeStamp = dDateTimeStamp.ToString("yyyyMMddHHmmssffff")
                            End If
                            lSortedByName.Add(oBlob.Name.Replace(folder & "/", ""), sPrefix & sDateTimeStamp & sSufix)
                        End If
                End Select
            Next

            Select Case ePatternType
                Case roPatternTypes.dateandtime
                    ' Ordeno por la fecha que viene en el nombre
                    For Each item As Generic.KeyValuePair(Of String, String) In lSortedByName.OrderBy(Function(f) f.Value)
                        oRet.Add(item.Key)
                    Next
                Case Else
                    ' Ordeno por fecha de creación del fichero
                    For Each item As Generic.KeyValuePair(Of String, BlobItemProperties) In lSortedByLastModified.OrderBy(Function(f) f.Value.LastModified)
                        oRet.Add(item.Key)
                    Next
            End Select
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::ListFiles::", ex)
        End Try

        Return oRet

    End Function

    Public Shared Function ListAllFiles(ByVal QueueType As roLiveQueueTypes, Optional folder As String = "", Optional bFromComanyContainer As Boolean = False, Optional strCompanyName As String = "") As List(Of String)
        Dim oRet As New List(Of String)
        Dim lSortedByLastModified As New SortedList(Of String, BlobItemProperties)
        Dim lSortedByName As New SortedList(Of String, String)
        Dim oServiceConfiguration As roServiceConfiguration = Nothing
        Dim bPatternWithDateTime As Boolean = False
        Try

            oServiceConfiguration = New roServiceConfigurationRepository(QueueType).GetServiceConfiguration()
            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)

            Dim strStorageContainer As String = oServiceConfiguration.storageblobcontainer
            If bFromComanyContainer Then strStorageContainer = strCompanyName.ToLower 'Los nombres de container sólo pueden contener minúsculas

            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(strStorageContainer)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            Dim strPatternEx As String = String.Empty

            If folder.Length > 0 Then
                strPatternEx = folder & "/"
            End If

            Dim lBlobs = containerClient.GetBlobs(,, strPatternEx)
            For Each oBlob As BlobItem In lBlobs
                If oBlob.Name.Split("/").Length = 2 Then
                    oRet.Add(oBlob.Name.Replace(folder & "/", ""))
                End If
            Next
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::ListFiles::", ex)
        End Try

        Return oRet

    End Function

    Public Shared Function DownloadFileFromCompanyContainer(ByVal fileName As String, ByVal folder As String, ByVal QueueType As roLiveQueueTypes, Optional bAlertIfnotExists As Boolean = True) As Byte()
        Dim sFileName As String
        If folder.Length > 0 Then
            sFileName = folder & "/" & fileName
        Else
            sFileName = fileName
        End If
        Return DownloadFile(sFileName, QueueType, True, RoAzureSupport.GetCompanyName, bAlertIfnotExists)
    End Function

    Public Shared Function RenameFileInCompanyContainer(ByVal oldfileName As String, ByVal newfileName As String, folder As String, ByVal QueueType As roLiveQueueTypes) As Boolean
        Dim bRet As Boolean

        Try
            ' Bajo
            Dim arrFile As Byte()
            arrFile = DownloadFileFromCompanyContainer(oldfileName, folder, roLiveQueueTypes.datalink)
            ' Subo
            bRet = SaveFileOnCompanyContainer(New System.IO.MemoryStream(arrFile), newfileName, folder, roLiveQueueTypes.datalink, True)
            ' Borro
            bRet = Azure.RoAzureSupport.DeleteFileFromCompanyContainer(oldfileName, folder, roLiveQueueTypes.datalink, True)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::RenameFileInCompanyContainer::", ex)
            bRet = False
        End Try
        Return bRet
    End Function

    Public Shared Function DeleteFileFromCompanyContainer(filename As String, folder As String, _QueueType As roLiveQueueTypes, Optional bCheckSuccess As Boolean = False) As Boolean
        Dim bRet As Boolean = False
        Try
            Dim oServiceConfiguration As roServiceConfiguration = New roServiceConfigurationRepository(_QueueType).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(RoAzureSupport.GetCompanyName)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            ' Create the container if it doesn't already exist.
            If containerClient.Exists().Value Then
                Dim sFileName As String
                If folder.Length > 0 Then
                    sFileName = folder & "/" & filename
                Else
                    sFileName = filename
                End If
                Dim blockBlob As BlobClient = containerClient.GetBlobClient(sFileName)
                bRet = blockBlob.DeleteIfExists()
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::DownloadFileFromCompany::", ex)
            bRet = False
        End Try
        Return bRet
    End Function

    Public Shared Function SaveFileOnCompanyContainer(stream As Stream, filename As String, folder As String, _QueueType As roLiveQueueTypes, Optional bCheckSuccess As Boolean = False, Optional oServiceConfiguration As roServiceConfiguration = Nothing, Optional sCompanyName As String = "") As Boolean
        Dim bRet As Boolean = False
        Try

            If oServiceConfiguration Is Nothing Then oServiceConfiguration = New roServiceConfigurationRepository(_QueueType).GetServiceConfiguration()
            If sCompanyName.Length = 0 Then sCompanyName = RoAzureSupport.GetCompanyName

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(sCompanyName)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            Dim sFileName As String
            If folder.Length > 0 Then
                sFileName = folder & "/" & filename
            Else
                sFileName = filename
            End If
            Dim blockBlob As BlobClient = containerClient.GetBlobClient(sFileName)
            blockBlob.Upload(stream, True)
            'Verificamos que se copió
            If bCheckSuccess Then
                bRet = blockBlob.Exists().Value
            Else
                bRet = True
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::SaveFileOnCompanyContainer::", ex)
            bRet = False
        End Try
        Return bRet
    End Function

    Public Shared Function DownloadFile(ByVal fileName As String, ByVal QueueType As roLiveQueueTypes, ByVal folder As String, Optional bAlertIfnotExists As Boolean = True) As Byte()
        Dim sFileName As String
        If folder.Length > 0 Then
            sFileName = folder & "/" & fileName
        Else
            sFileName = fileName
        End If
        Return DownloadFile(sFileName, QueueType,,, bAlertIfnotExists)
    End Function

    Public Shared Function DownloadFile(ByVal fileName As String, ByVal QueueType As roLiveQueueTypes, Optional bFromComanyContainer As Boolean = False, Optional strComapnyName As String = "", Optional bAlertIfnotExists As Boolean = True) As Byte()

        Dim arrFile As Byte() = Nothing

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing

            oServiceConfiguration = New roServiceConfigurationRepository(QueueType).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)

            Dim strStorageContainer As String = oServiceConfiguration.storageblobcontainer

            If bFromComanyContainer AndAlso strComapnyName.Trim.Length > 0 Then
                strStorageContainer = strComapnyName.Trim
            End If

            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(strStorageContainer)
            containerClient.CreateIfNotExists()

            Dim blockBlob As BlobClient = containerClient.GetBlobClient(fileName)
            If blockBlob.Exists().Value Then
                Using stream As New MemoryStream()
                    Dim response As Response = blockBlob.DownloadTo(stream)
                    arrFile = stream.ToArray()
                End Using
            End If
        Catch ex As Exception
            If bAlertIfnotExists Then roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::DownloadFile::File not found::" & fileName)
            arrFile = Nothing
        Finally

        End Try

        Return arrFile

    End Function

    Public Shared Function DownloadFileToFile(ByVal fileName As String, ByVal destinationFileName As String, ByVal QueueType As roLiveQueueTypes, Optional bFromComanyContainer As Boolean = False, Optional strComapnyName As String = "") As Byte()

        Dim arrFile As Byte() = Nothing

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing

            oServiceConfiguration = New roServiceConfigurationRepository(QueueType).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)

            ' Retrieve a reference to a container.
            Dim strStorageContainer As String = oServiceConfiguration.storageblobcontainer

            If bFromComanyContainer AndAlso strComapnyName.Trim.Length > 0 Then
                strStorageContainer = strComapnyName.Trim
            End If

            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(strStorageContainer)
            containerClient.CreateIfNotExists()

            Dim appendBlob As BlobClient = containerClient.GetBlobClient(fileName)

            If appendBlob.Exists().Value Then
                Using mStream As New MemoryStream
                    appendBlob.DownloadTo(mStream)

                    Using fStream As FileStream = System.IO.File.Create(destinationFileName)
                        mStream.Seek(0, SeekOrigin.Begin)
                        mStream.CopyTo(fStream)

                        fStream.Flush()
                        fStream.Close()
                    End Using

                    mStream.Flush()
                    mStream.Close()
                End Using
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::DownloadFileToFile::", ex)
            arrFile = Nothing
        Finally

        End Try

        Return arrFile

    End Function

    Public Shared Function GetLastModified(ByVal fileName As String, ByVal folder As String, ByVal QueueType As roLiveQueueTypes) As DateTime

        Dim oRet As BlobProperties = Nothing

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing

            oServiceConfiguration = New roServiceConfigurationRepository(QueueType).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(oServiceConfiguration.storageblobcontainer)
            containerClient.CreateIfNotExists()

            Dim blockBlob As BlobClient = Nothing
            If folder <> String.Empty Then
                blockBlob = containerClient.GetBlobClient(folder & "/" & fileName)
            Else
                blockBlob = containerClient.GetBlobClient(fileName)
            End If

            If blockBlob.Exists Then
                oRet = blockBlob.GetProperties()
            End If
        Catch ex As Exception
            oRet = Nothing
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetFileProperties::", ex)
        Finally

        End Try

        If oRet IsNot Nothing Then
            Return oRet.LastModified.DateTime
        Else
            Return DateTime.MinValue
        End If

    End Function

    Public Shared Function GetLastModifiedOnCompanyContainter(ByVal fileName As String, ByVal folder As String, ByVal QueueType As roLiveQueueTypes, Optional sCompanyName As String = "") As DateTime

        Dim oRet As BlobProperties = Nothing

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing

            oServiceConfiguration = New roServiceConfigurationRepository(QueueType).GetServiceConfiguration()
            If sCompanyName.Length = 0 Then sCompanyName = RoAzureSupport.GetCompanyName

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(sCompanyName)
            containerClient.CreateIfNotExists()

            Dim blockBlob As BlobClient = Nothing
            If folder <> String.Empty Then
                blockBlob = containerClient.GetBlobClient(folder & "/" & fileName)
            Else
                blockBlob = containerClient.GetBlobClient(fileName)
            End If

            If blockBlob.Exists Then
                oRet = blockBlob.GetProperties()
            End If
        Catch ex As Exception
            oRet = Nothing
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetFileProperties::", ex)
        Finally

        End Try

        If oRet IsNot Nothing Then
            Return oRet.LastModified.DateTime
        Else
            Return DateTime.MinValue
        End If

    End Function

    Public Shared Function UploadStream2Blob(stream As Stream, filename As String, _QueueType As roLiveQueueTypes, folder As String, Optional bCheckSuccess As Boolean = False) As Boolean
        Dim bRet As Boolean = False
        Try
            Dim oServiceConfiguration As roServiceConfiguration = New roServiceConfigurationRepository(_QueueType).GetServiceConfiguration()
            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(oServiceConfiguration.storageblobcontainer)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            Dim blockBlob As BlobClient = containerClient.GetBlobClient(folder & "/" & filename)
            blockBlob.Upload(stream, True)
            'Verificamos que se copió
            If bCheckSuccess Then
                bRet = blockBlob.Exists().Value
            Else
                bRet = True
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::UploadStream2Blob::", ex)
            bRet = False
        End Try
        Return bRet
    End Function

    Public Shared Function UploadStream2BlobInCompanyContainer(stream As Stream, filename As String, _QueueType As roLiveQueueTypes, client As String, Optional bCheckSuccess As Boolean = False) As Boolean
        Dim bRet As Boolean = False
        Try
            Dim oServiceConfiguration As roServiceConfiguration = New roServiceConfigurationRepository(_QueueType).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(client)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            Dim blockBlob As BlobClient = containerClient.GetBlobClient(filename)
            blockBlob.Upload(stream, True)
            'Verificamos que se copió
            If bCheckSuccess Then
                bRet = blockBlob.Exists().Value
            Else
                bRet = True
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::UploadStream2Blob::", ex)
            bRet = False
        End Try
        Return bRet
    End Function

    Public Shared Function DeleteFileFromBlob(filename As String, _QueueType As roLiveQueueTypes, folder As String, Optional bCheckSuccess As Boolean = False) As Boolean
        Dim bRet As Boolean = False
        Try
            Dim oServiceConfiguration As roServiceConfiguration = New roServiceConfigurationRepository(_QueueType).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(oServiceConfiguration.storageblobcontainer)
            containerClient.CreateIfNotExists()

            ' Create the container if it doesn't already exist.
            If containerClient.Exists().Value Then
                Dim blockBlob As BlobClient = containerClient.GetBlobClient(folder & "/" & filename)
                bRet = blockBlob.DeleteIfExists()
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::DeleteFileFromBlob::", ex)
            bRet = False
        End Try
        Return bRet
    End Function

#End Region

#Region "Analytics"

    Public Sub Any2Genius(oLst As IEnumerable(Of Analytics_Base), strFileName As String, userFieldNames() As String, oLng As roLanguage, queueType As roLiveQueueTypes, companyName As String, oSchema As Analytics_Base)

        Dim oTranlateHash As Hashtable = Nothing

        If oLng IsNot Nothing Then
            oTranlateHash = New Hashtable()

            Dim actualType As Type = oSchema.GetType()
            For Each oProperty As Reflection.PropertyInfo In actualType.GetProperties
                oTranlateHash.Add(oProperty.Name, oLng.Translate("columnname." + actualType.Name + "." + oProperty.Name + ".rotext", ""))

                If oProperty.Name.StartsWith("UserField") Then
                    Dim index = roTypes.Any2Integer(oProperty.Name.Replace("UserField", ""))

                    If index - 1 < userFieldNames.Length Then
                        oTranlateHash(oProperty.Name) = userFieldNames(index - 1)
                    End If

                End If
            Next
        End If

        Using stream As MemoryStream = New MemoryStream()
            Using sw As New StreamWriter(stream)
                sw.WriteLine("[")

                sw.WriteLine(roJSONHelper.ToGeniusJSONDefinitionString(oSchema, oTranlateHash))
                If oLst.Count > 0 Then sw.WriteLine(",")

                Dim i As Integer = 0
                For i = 0 To oLst.Count - 1
                    If i < oLst.Count - 1 Then
                        sw.WriteLine(roJSONHelper.ToGeniusJSONString(oLst(i), oTranlateHash, userFieldNames) & ",")
                    Else
                        sw.WriteLine(roJSONHelper.ToGeniusJSONString(oLst(i), oTranlateHash, userFieldNames))
                    End If
                Next

                sw.WriteLine("]")
                sw.Flush()

                stream.Seek(0, SeekOrigin.Begin)
                UploadStream2Blob(stream, strFileName, queueType, companyName)
            End Using

            stream.Flush()
            stream.Close()
        End Using
    End Sub

    Public Sub Any2GeniusBySlice(oThreadData As roThreadData, begin As Integer, _end As Integer, dt As DataTable, oTranslations As Generic.List(Of roLayoutDescription), ByVal userFieldTypes As Base.DTOs.UserFieldProperties(), queueType As roLiveQueueTypes, companyName As String, dicKeywords As Generic.Dictionary(Of String, String),
                          dicInvalidDescription As Generic.Dictionary(Of Integer, String), dicMethodDescription As Generic.Dictionary(Of Integer, String), dicPassportName As Generic.Dictionary(Of Integer, String), dicPunchDirectionDescription As Generic.Dictionary(Of Integer, String),
                          dicStatusDescription As Generic.Dictionary(Of Integer, String), dicTypeDescription As Generic.Dictionary(Of Integer, String), dicVersionDescription As Generic.Dictionary(Of String, String), dicPassportUserFieldPermission As Generic.Dictionary(Of Integer, Base.DTOs.Permission), dicDayOfWeekDescription As Generic.Dictionary(Of Integer, String), dicReliableDescription As Generic.Dictionary(Of Integer, String), dicMonthDescription As Generic.Dictionary(Of Integer, String), dicUnespecifiedZoneDescription As Generic.Dictionary(Of String, String), strFileName As String, BIExecutionName As String)

        roConstants.RestoreThreadData(oThreadData)

        Dim i As Integer = 0
        Dim numberOfElements As Integer = 0

        Dim writeLines As List(Of String) = New List(Of String)()
        For i = begin To _end
            Dim strJSON As String = roJSONHelper.ToGeniusJSONString(dt.Rows(i), oTranslations, userFieldTypes, dicKeywords, dicInvalidDescription, dicMethodDescription, dicPassportName, dicPunchDirectionDescription, dicStatusDescription, dicTypeDescription, dicVersionDescription, dicPassportUserFieldPermission, dt.Columns, dicDayOfWeekDescription, dicReliableDescription, dicMonthDescription, dicUnespecifiedZoneDescription, BIExecutionName)
            If strJSON <> String.Empty Then
                writeLines.Add(strJSON + ",")
                numberOfElements = numberOfElements + 1
            End If

            If numberOfElements = 250 Or i = _end Then
                numberOfElements = 0
                If queueType = roLiveQueueTypes.analytics Then
                    WriteAnalitycsLines(writeLines, strFileName, queueType, companyName)
                Else
                    WriteAnalitycsLinesInCompanyContainer(writeLines, strFileName, queueType, companyName)
                End If
                writeLines = New List(Of String)()
            End If
        Next
    End Sub

    Public Sub Any2Genius(dt As DataTable, strFileName As String, oTranslations As Generic.List(Of roLayoutDescription), ByVal userFieldTypes As Base.DTOs.UserFieldProperties(), queueType As roLiveQueueTypes, companyName As String, dicKeywords As Generic.Dictionary(Of String, String),
                          dicInvalidDescription As Generic.Dictionary(Of Integer, String), dicMethodDescription As Generic.Dictionary(Of Integer, String), dicPassportName As Generic.Dictionary(Of Integer, String), dicPunchDirectionDescription As Generic.Dictionary(Of Integer, String),
                          dicStatusDescription As Generic.Dictionary(Of Integer, String), dicTypeDescription As Generic.Dictionary(Of Integer, String), dicVersionDescription As Generic.Dictionary(Of String, String), dicPassportUserFieldPermission As Generic.Dictionary(Of Integer, Base.DTOs.Permission), dicDayOfWeekDescription As Generic.Dictionary(Of Integer, String), dicReliableDescription As Generic.Dictionary(Of Integer, String), dicMonthDescription As Generic.Dictionary(Of Integer, String), dicUnespecifiedZoneDescription As Generic.Dictionary(Of String, String), BIExecutionName As String)

        Using stream As MemoryStream = New MemoryStream()
            Using sw As New StreamWriter(stream)
                sw.WriteLine("[")

                If BIExecutionName Is Nothing Then
                    sw.WriteLine(roJSONHelper.ToGeniusJSONDefinitionString(dt, oTranslations, userFieldTypes))
                    If dt.Rows.Count > 0 Then sw.WriteLine(",")
                End If

                Dim i As Integer = 0

                For i = 0 To dt.Rows.Count - 1
                    Dim strJSON As String = roJSONHelper.ToGeniusJSONString(dt.Rows(i), oTranslations, userFieldTypes, dicKeywords, dicInvalidDescription, dicMethodDescription, dicPassportName, dicPunchDirectionDescription, dicStatusDescription, dicTypeDescription, dicVersionDescription, dicPassportUserFieldPermission, dt.Columns, dicDayOfWeekDescription, dicReliableDescription, dicMonthDescription, dicUnespecifiedZoneDescription, BIExecutionName)
                    If strJSON <> String.Empty Then
                        If i < dt.Rows.Count - 1 Then
                            sw.WriteLine(strJSON & ",")
                        Else
                            sw.WriteLine(strJSON)
                        End If
                    End If
                Next

                sw.WriteLine("]")
                sw.Flush()

                stream.Seek(0, SeekOrigin.Begin)
                If queueType = roLiveQueueTypes.analytics Then
                    UploadStream2Blob(stream, strFileName, queueType, companyName)
                Else
                    UploadStream2BlobInCompanyContainer(stream, strFileName, queueType, companyName)
                End If

            End Using

            stream.Flush()
            stream.Close()
        End Using
    End Sub

    Public Sub Any2GeniusParalellized(dt As DataTable, strFileName As String, oTranslations As Generic.List(Of roLayoutDescription), ByVal userFieldTypes As Base.DTOs.UserFieldProperties(), queueType As roLiveQueueTypes, companyName As String, dicKeywords As Generic.Dictionary(Of String, String),
                          dicInvalidDescription As Generic.Dictionary(Of Integer, String), dicMethodDescription As Generic.Dictionary(Of Integer, String), dicPassportName As Generic.Dictionary(Of Integer, String), dicPunchDirectionDescription As Generic.Dictionary(Of Integer, String),
                          dicStatusDescription As Generic.Dictionary(Of Integer, String), dicTypeDescription As Generic.Dictionary(Of Integer, String), dicVersionDescription As Generic.Dictionary(Of String, String), dicPassportUserFieldPermission As Generic.Dictionary(Of Integer, Base.DTOs.Permission), dicDayOfWeekDescription As Generic.Dictionary(Of Integer, String), dicReliableDescription As Generic.Dictionary(Of Integer, String), dicMonthDescription As Generic.Dictionary(Of Integer, String), dicUnespecifiedZoneDescription As Generic.Dictionary(Of String, String), BIExecutionName As String)

        Dim writeLines As List(Of String) = New List(Of String)()
        If BIExecutionName Is Nothing Then
            writeLines.Add("[" + roJSONHelper.ToGeniusJSONDefinitionString(dt, oTranslations, userFieldTypes) & ",")
        Else
            writeLines.Add("[")
        End If
        If queueType = roLiveQueueTypes.analytics Then
            WriteAnalitycsLines(writeLines, strFileName, queueType, companyName)
        Else
            WriteAnalitycsLinesInCompanyContainer(writeLines, strFileName, queueType, companyName)
        End If
        writeLines = New List(Of String)()

        If (dt.Rows.Count > 0) Then
            writeLines = New List(Of String)()

            Dim partialCount = dt.Rows.Count / 6
            Dim i As Integer = 0
            Dim icn1 = 0
            Dim fcn1 = Math.Round(partialCount)
            Dim icn2 = fcn1 + 1
            Dim fcn2 = Math.Round(partialCount * 2)
            Dim icn3 = fcn2 + 1
            Dim fcn3 = Math.Round(partialCount * 3)
            Dim icn4 = fcn3 + 1
            Dim fcn4 = Math.Round(partialCount * 4)
            Dim icn5 = fcn4 + 1
            Dim fcn5 = Math.Round(partialCount * 5)
            Dim icn6 = fcn5 + 1
            Dim fcn6 = dt.Rows.Count - 2

            Dim oThreadData As roThreadData = VTBase.roConstants.BackupThreadData()

            Parallel.Invoke(Sub() Any2GeniusBySlice(oThreadData, icn1, fcn1, dt, oTranslations, userFieldTypes, queueType, companyName, dicKeywords, dicInvalidDescription, dicMethodDescription, dicPassportName, dicPunchDirectionDescription, dicStatusDescription, dicTypeDescription, dicVersionDescription, dicPassportUserFieldPermission, dicDayOfWeekDescription, dicReliableDescription, dicMonthDescription, dicUnespecifiedZoneDescription, strFileName, BIExecutionName),
                                        Sub() Any2GeniusBySlice(oThreadData, icn2, fcn2, dt, oTranslations, userFieldTypes, queueType, companyName, dicKeywords, dicInvalidDescription, dicMethodDescription, dicPassportName, dicPunchDirectionDescription, dicStatusDescription, dicTypeDescription, dicVersionDescription, dicPassportUserFieldPermission, dicDayOfWeekDescription, dicReliableDescription, dicMonthDescription, dicUnespecifiedZoneDescription, strFileName, BIExecutionName),
                                        Sub() Any2GeniusBySlice(oThreadData, icn3, fcn3, dt, oTranslations, userFieldTypes, queueType, companyName, dicKeywords, dicInvalidDescription, dicMethodDescription, dicPassportName, dicPunchDirectionDescription, dicStatusDescription, dicTypeDescription, dicVersionDescription, dicPassportUserFieldPermission, dicDayOfWeekDescription, dicReliableDescription, dicMonthDescription, dicUnespecifiedZoneDescription, strFileName, BIExecutionName),
                                        Sub() Any2GeniusBySlice(oThreadData, icn4, fcn4, dt, oTranslations, userFieldTypes, queueType, companyName, dicKeywords, dicInvalidDescription, dicMethodDescription, dicPassportName, dicPunchDirectionDescription, dicStatusDescription, dicTypeDescription, dicVersionDescription, dicPassportUserFieldPermission, dicDayOfWeekDescription, dicReliableDescription, dicMonthDescription, dicUnespecifiedZoneDescription, strFileName, BIExecutionName),
                                        Sub() Any2GeniusBySlice(oThreadData, icn5, fcn5, dt, oTranslations, userFieldTypes, queueType, companyName, dicKeywords, dicInvalidDescription, dicMethodDescription, dicPassportName, dicPunchDirectionDescription, dicStatusDescription, dicTypeDescription, dicVersionDescription, dicPassportUserFieldPermission, dicDayOfWeekDescription, dicReliableDescription, dicMonthDescription, dicUnespecifiedZoneDescription, strFileName, BIExecutionName),
                                        Sub() Any2GeniusBySlice(oThreadData, icn6, fcn6, dt, oTranslations, userFieldTypes, queueType, companyName, dicKeywords, dicInvalidDescription, dicMethodDescription, dicPassportName, dicPunchDirectionDescription, dicStatusDescription, dicTypeDescription, dicVersionDescription, dicPassportUserFieldPermission, dicDayOfWeekDescription, dicReliableDescription, dicMonthDescription, dicUnespecifiedZoneDescription, strFileName, BIExecutionName))

            writeLines = New List(Of String)()
            writeLines.Add(roJSONHelper.ToGeniusJSONString(dt.Rows(dt.Rows.Count - 1), oTranslations, userFieldTypes, dicKeywords, dicInvalidDescription, dicMethodDescription, dicPassportName, dicPunchDirectionDescription, dicStatusDescription, dicTypeDescription, dicVersionDescription, dicPassportUserFieldPermission, dt.Columns, dicDayOfWeekDescription, dicReliableDescription, dicMonthDescription, dicUnespecifiedZoneDescription, BIExecutionName))
        End If
        writeLines.Add("]")
        If queueType = roLiveQueueTypes.analytics Then
            WriteAnalitycsLines(writeLines, strFileName, queueType, companyName)
        Else
            WriteAnalitycsLinesInCompanyContainer(writeLines, strFileName, queueType, companyName)
        End If

    End Sub

    Public Sub Any2Blob(Of T)(ByVal list As IEnumerable(Of T), ByVal filename As String, ByVal _QueueType As roLiveQueueTypes, ByVal folder As String)
        Using stream As MemoryStream = New System.IO.MemoryStream()

            Dim bin As BinaryFormatter = New BinaryFormatter()
            For Each item As T In list
                bin.Serialize(stream, item)
            Next

            stream.Seek(0, SeekOrigin.Begin)

            Using tmpStream As New MemoryStream()
                Using cFile As GZipStream = New GZipStream(tmpStream, CompressionMode.Compress, True)
                    ' Copy the source file into the compression stream.
                    stream.CopyTo(cFile)
                End Using
                tmpStream.Seek(0, SeekOrigin.Begin)
                UploadStream2Blob(tmpStream, filename, _QueueType, folder)
            End Using
        End Using
    End Sub

    Public Shared Function WriteAnalitycsLines(ByVal strAnalyticLines As List(Of String), filename As String, _QueueType As roLiveQueueTypes, folder As String, Optional bCheckSuccess As Boolean = False) As Boolean

        Dim oRet As Boolean = True

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing

            oServiceConfiguration = New roServiceConfigurationRepository(_QueueType).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(oServiceConfiguration.storageblobcontainer)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            Dim blockBlob As AppendBlobClient = containerClient.GetAppendBlobClient(folder & "/" & filename)
            blockBlob.CreateIfNotExists()

            Dim i As Integer

            Using stream As New MemoryStream
                Using writer As New StreamWriter(stream)
                    For i = 0 To strAnalyticLines.Count - 1
                        writer.WriteLine(strAnalyticLines(i))
                    Next
                    writer.Flush()
                    stream.Position = 0
                    blockBlob.AppendBlock(stream)
                End Using
            End Using
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::WriteAnalyticLines::", ex)
            oRet = False

        End Try

        Return oRet

    End Function

    Public Shared Function WriteAnalitycsLinesInCompanyContainer(ByVal strAnalyticLines As List(Of String), filename As String, _QueueType As roLiveQueueTypes, client As String, Optional bCheckSuccess As Boolean = False) As Boolean

        Dim oRet As Boolean = True

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing

            oServiceConfiguration = New roServiceConfigurationRepository(_QueueType).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(client)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            Dim blockBlob As AppendBlobClient = containerClient.GetAppendBlobClient(filename)
            blockBlob.CreateIfNotExists()

            Dim i As Integer

            Using stream As New MemoryStream
                Using writer As New StreamWriter(stream)
                    For i = 0 To strAnalyticLines.Count - 1
                        writer.WriteLine(strAnalyticLines(i))
                    Next
                    writer.Flush()
                    stream.Position = 0
                    blockBlob.AppendBlock(stream)

                End Using
            End Using
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::WriteAnalyticLines::", ex)
            oRet = False

        End Try

        Return oRet

    End Function

#End Region

#Region "Audit Tables"
    Public Shared Function AddAuditLineToTable(ByVal strAuditLine As String, ByVal xDate As DateTime) As Boolean
        Try
            Return AddAuditLineToTable(RoAzureSupport.GetCompanyName(), strAuditLine, xDate)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::AddAuditLineToTable::", ex)
            Return False
        End Try
    End Function

    Public Shared Function AddAuditLineToTable(ByVal companyId As String, ByVal strAuditLine As String, ByVal xDate As DateTime) As Boolean

        Dim oRet As Boolean = True

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing
            oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.audit).GetServiceConfiguration()

            Dim tableClient As New TableClient(oServiceConfiguration.storageconnectionstring, companyId)
            tableClient.CreateIfNotExists()

            tableClient.UpsertEntity(Of roAzureAuditEntity)(New roAzureAuditEntity(xDate.ToString("yyyyMMdd"), xDate, strAuditLine))
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::AddAuditLineToTable_2::", ex)
            oRet = False
        End Try

        Return oRet
    End Function

    Public Shared Function AddAuditLineToTableBatch(ByVal strAuditLine As String(), ByVal xDate As DateTime()) As Boolean
        Try
            Return AddAuditLineToTableBatch(RoAzureSupport.GetCompanyName(), strAuditLine, xDate)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::AddAuditLineToTable::", ex)
            Return False
        End Try
    End Function

    Public Shared Function AddAuditLineToTableBatch(ByVal companyId As String, ByVal strAuditLine As String(), ByVal xDate As DateTime()) As Boolean

        Dim oRet As Boolean = True

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing
            oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.audit).GetServiceConfiguration()

            Dim tableClient As New TableClient(oServiceConfiguration.storageconnectionstring, companyId)
            tableClient.CreateIfNotExists()

            Dim lastAuditKey As String = String.Empty
            Dim actualAuditKey As String = String.Empty
            Dim auditText As String = String.Empty

            Dim batchOperation As New List(Of TableTransactionAction)
            For i As Integer = 0 To strAuditLine.Length - 1
                actualAuditKey = xDate(i).ToString("yyyyMMdd")

                If lastAuditKey = String.Empty Then
                    lastAuditKey = xDate(i).ToString("yyyyMMdd")
                End If

                If lastAuditKey <> actualAuditKey Then
                    If batchOperation.Count > 0 Then
                        tableClient.SubmitTransaction(batchOperation)
                    End If
                    batchOperation = New List(Of TableTransactionAction)
                    lastAuditKey = actualAuditKey
                End If

                batchOperation.Add(New TableTransactionAction(TableTransactionActionType.Add, New roAzureAuditEntity(xDate(i).ToString("yyyyMMdd"), xDate(i), strAuditLine(i))))
            Next

            If batchOperation.Count > 0 Then tableClient.SubmitTransaction(batchOperation)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::AddAuditLineToTable_2::", ex)
            oRet = False
        End Try

        Return oRet
    End Function

    Public Shared Function GetAzureAuditBetweenDates(ByVal xBeginDate As Date, ByVal xEndDate As DateTime) As String()
        Try
            Return GetAzureAuditBetweenDates(RoAzureSupport.GetCompanyName(), xBeginDate, xEndDate)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetAzureAuditBetweenDates_1::", ex)
            Return {}
        End Try

    End Function

    Public Shared Function GetAzureAuditBetweenDates(ByVal companyId As String, ByVal xBeginDate As Date, ByVal xEndDate As DateTime) As String()

        Dim oRet As String()

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing
            oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.audit).GetServiceConfiguration()

            Dim tableClient As New TableClient(oServiceConfiguration.storageconnectionstring, companyId)
            tableClient.CreateIfNotExists()

            Dim auditRecords As New List(Of String)
            Dim startDate As DateTime = xBeginDate.Date
            Dim endDate As DateTime = xEndDate.Date

            While startDate <= endDate

                For Each auditline As roAzureAuditEntity In tableClient.Query(Of roAzureAuditEntity)($"PartitionKey eq '{startDate.ToString("yyyyMMdd")}'")
                    auditRecords.Add(auditline.Content)
                Next
                startDate = startDate.AddDays(1)

            End While

            oRet = auditRecords.ToArray
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::AddAuditLineToTable_2::", ex)
            oRet = {}
        End Try

        Return oRet

    End Function


#End Region

#Region "Booking Tables"
    Public Shared Function AddLogBookLineToTable(ByVal complaintRef As String, ByVal message As String) As Boolean
        Try
            Return AddLogBookLineToTable(RoAzureSupport.GetCompanyName(), complaintRef, message)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::AddLogBookLineToTable::", ex)
            Return False
        End Try
    End Function

    Public Shared Function AddLogBookLineToTable(ByVal companyId As String, ByVal complaintRef As String, ByVal logInfo As String) As Boolean

        Dim oRet As Boolean = True

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing
            oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.logbook).GetServiceConfiguration()

            Dim tableClient As New TableClient(oServiceConfiguration.storageconnectionstring, companyId)
            tableClient.CreateIfNotExists()

            tableClient.UpsertEntity(Of roAzureLogBooklEntity)(New roAzureLogBooklEntity(complaintRef, logInfo))
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::AddLogBookLineToTable_2::", ex)
            oRet = False
        End Try

        Return oRet
    End Function

    Public Shared Function GetAzureLogBookByReference(ByVal xComplaintRef As String) As String()
        Try

            Return GetAzureLogBookByReference(RoAzureSupport.GetCompanyName(), xComplaintRef)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetAzureLogBookByReference::", ex)
            Return {}
        End Try

    End Function

    Public Shared Function GetAzureLogBookByReference(ByVal companyId As String, ByVal xComplaintRef As String)
        Dim oRet As String()
        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing
            oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.logbook).GetServiceConfiguration()

            Dim tableClient As New TableClient(oServiceConfiguration.storageconnectionstring, companyId)
            tableClient.CreateIfNotExists()

            Dim logRecords As New List(Of String)
            Dim complaintRef As String = xComplaintRef
            Dim logBooklines As List(Of roAzureLogBooklEntity) = New List(Of roAzureLogBooklEntity)()
            logBooklines = tableClient.Query(Of roAzureLogBooklEntity)($"PartitionKey eq '{complaintRef.ToString()}'").ToList()
            logBooklines = logBooklines.OrderBy(Function(msg) msg.Timestamp).ToList()
            For Each logBookline As roAzureLogBooklEntity In logBooklines
                logRecords.Add(logBookline.LogInfo)
            Next

            oRet = logRecords.ToArray
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetAzureLogBookByReference::", ex)
            oRet = {}
        End Try

        Return oRet

    End Function

    Public Shared Function GetLogBookByReference(ByVal companyId As String, ByVal xComplaintRef As String) As String()

        Dim oRet As String()

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing
            oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.audit).GetServiceConfiguration()

            Dim tableClient As New TableClient(oServiceConfiguration.storageconnectionstring, companyId)
            tableClient.CreateIfNotExists()

            Dim logBookRecords As New List(Of String)
            Dim complaintRef As String = xComplaintRef

            For Each logline As roAzureAuditEntity In tableClient.Query(Of roAzureAuditEntity)($"PartitionKey eq '{complaintRef}'")
                logBookRecords.Add(logline.Content)
            Next

            oRet = logBookRecords.ToArray
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::AddLogBooktLineToTable_2::", ex)
            oRet = {}
        End Try

        Return oRet

    End Function

#End Region

#Region "KeyValut"

    Public Shared Function GetCompanySecret(ByVal companyApiID As String) As String
#If DEBUG Then
        Return "a8c81362d9d3d27d837c8dd05a436821878ec635cb924223c8fb510b4069fececda68cebd2d265f4720abfa9ee2dc2b57ea760071f9713fa69c8f73090ddcb10"
#End If

        Dim oKeyvaultManager As New roAzureKeyvault
        Return oKeyvaultManager.GeCompanyDexKey(companyApiID, True)
    End Function

    Public Shared Function GetVisualtimePGPPassphrase() As String
#If DEBUG Then
        Return $"9U8p_yuU51IA"
#End If
        Dim oConf As roAzureConfig

        Dim oKeyvaultManager As New roAzureKeyvault
        oConf = oKeyvaultManager.GeKeyVaultKey(roKeyvaultParameter.roVisualtimePGPPassphrase)
        Return If(oConf?.value, "")
    End Function

    Public Shared Function GetVisualtimeDBUsername() As String
#If DEBUG Then
        Return $"sa"
#End If
        Dim oConf As roAzureConfig

        Dim oKeyvaultManager As New roAzureKeyvault
        oConf = oKeyvaultManager.GeKeyVaultKey(roKeyvaultParameter.roVisualtimeDBUsername)
        Return If(oConf?.value, "")
    End Function

    Public Shared Function GetVisualtimeDBPassword() As String
#If DEBUG Then
        Return $"VisualTime#1"
#End If
        Dim oConf As roAzureConfig

        Dim oKeyvaultManager As New roAzureKeyvault
        oConf = oKeyvaultManager.GeKeyVaultKey(roKeyvaultParameter.roVisualtimeDBPassword)
        Return If(oConf?.value, "")
    End Function

#End Region

#Region "Documents"

    Public Shared Function UploadDocument2Azure(stream As Stream, filename As String, Optional bCheckSuccess As Boolean = False) As Boolean
        Try
            Return UploadDocument2Azure(RoAzureSupport.GetCompanyName(), stream, filename, bCheckSuccess)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::UploadDocument2Azure-1::", ex)
            Return False
        End Try
    End Function

    Public Shared Function UploadDocument2Azure(ByVal companyId As String, stream As Stream, filename As String, Optional bCheckSuccess As Boolean = False) As Boolean
        Dim bRet As Boolean = False
        Try
            Dim oServiceConfiguration As roServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.documents).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(companyId)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            Dim blockBlob As BlobClient = containerClient.GetBlobClient("documents/" & filename)
            blockBlob.Upload(stream, True)
            'Verificamos que se copió
            If bCheckSuccess Then
                bRet = blockBlob.Exists.Value
            Else
                bRet = True
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::UploadDocument2Azure-2::", ex)
            bRet = False
        End Try
        Return bRet
    End Function

    Public Shared Function GetDocumentFile(ByVal fileName As String) As Byte()
        Try
            Return GetDocumentFile(RoAzureSupport.GetCompanyName(), fileName)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetDocumentFile-2::", ex)
            Return {}
        End Try
    End Function

    Public Shared Function GetDocumentFile(ByVal companyId As String, ByVal fileName As String) As Byte()

        Dim arrFile As Byte() = Nothing

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing

            oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.documents).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(companyId)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            Dim blockBlob As BlobClient = containerClient.GetBlobClient("documents/" & fileName)

            If blockBlob.Exists.Value Then
                Using stream As New MemoryStream()
                    Dim response As Response = blockBlob.DownloadTo(stream)
                    arrFile = stream.ToArray()
                End Using
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetDocumentFile-2::", ex)
            arrFile = Nothing
        End Try

        Return arrFile

    End Function

#End Region

#Region "Punch Photos"

    ''' <summary>
    ''' Upload a punch photo to Azure. Client should encrypt if needed
    ''' </summary>
    ''' <param name="stream"></param>
    ''' <param name="filename"></param>
    ''' <param name="bCheckSuccess"></param>
    ''' <returns></returns>
    Public Shared Function UploadPunchPhoto2Azure(stream As Stream, idPunch As String, Optional bCheckSuccess As Boolean = False) As Boolean
        Try
            Return UploadPunchPhoto2Azure(RoAzureSupport.GetCompanyName(), stream, idPunch, bCheckSuccess)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::UploadPunchPhoto2Azure::", ex)
            Return False
        End Try
    End Function

    Public Shared Function UploadPunchPhoto2Azure(ByVal companyId As String, stream As Stream, idPunch As String, Optional bCheckSuccess As Boolean = False) As Boolean
        Dim bRet As Boolean = False
        Try

            Dim filename As String = "pic_" & idPunch & ".photo"

            Dim oServiceConfiguration As roServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.punchphotos).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(companyId)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            Dim blockBlob As BlobClient = containerClient.GetBlobClient("punchphotos/" & filename)

            blockBlob.Upload(stream, True)
            'Verificamos que se copió
            If bCheckSuccess Then
                bRet = blockBlob.Exists.Value
            Else
                bRet = True
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::UploadPunchPhoto2Azure::", ex)
            bRet = False
        End Try
        Return bRet
    End Function

    Public Shared Function GetPunchPhotoFile(ByVal idPunch As String) As Byte()
        Try
            Return GetPunchPhotoFile(RoAzureSupport.GetCompanyName(), idPunch)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetPunchPhotoFile::", ex)
            Return {}
        End Try
    End Function

    Public Shared Function GetPunchPhotoFile(ByVal companyId As String, ByVal idPunch As String) As Byte()

        Dim arrEncryptedFile As Byte() = Nothing

        Try
            Dim filename As String = "pic_" & idPunch & ".photo"

            Dim oServiceConfiguration As roServiceConfiguration = Nothing

            oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.punchphotos).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(companyId)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            Dim blockBlob As BlobClient = containerClient.GetBlobClient("punchphotos/" & filename)

            If blockBlob.Exists.Value Then
                Using Stream As New MemoryStream()
                    Dim response As Response = blockBlob.DownloadTo(Stream)
                    arrEncryptedFile = Stream.ToArray()
                End Using
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "RoAzureSupport::GetPunchPhotoFile::Punch photo file for punch with id " & idPunch.ToString & " does not exists")
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetPunchPhotoFile::", ex)
            arrEncryptedFile = Nothing
        End Try

        Return arrEncryptedFile

    End Function

    Public Shared Function DeletePunchPhotoFile(ByVal idPunch As String) As Boolean
        Dim bRet As Boolean = False
        Try
            Dim filename As String = "pic_" & idPunch & ".photo"
            bRet = Azure.RoAzureSupport.DeleteFileFromCompanyContainer(filename, roLiveQueueTypes.punchphotos.ToString, roLiveQueueTypes.punchphotos)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::DeletePunchPhotoFile::", ex)
        End Try

        Return bRet
    End Function

#End Region

#Region "Helper"

    Public Shared Function SaveFileOnAzure(sFile As Byte(), strFileBlobName As String, type As roLiveQueueTypes) As Boolean
        Dim bRet As Boolean = False
        Try
            Dim ms As New System.IO.MemoryStream(sFile)
            bRet = Azure.RoAzureSupport.UploadStream2Blob(ms, strFileBlobName, type, RoAzureSupport.GetCompanyName, True)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::SaveFileOnAzure::", ex)
            bRet = False
        End Try
        Return bRet
    End Function

    Public Shared Function GetFileBytesFromAzure(strFileBlobName As String, type As roLiveQueueTypes) As Byte()
        Dim aRet As Byte() = {}

        Try
            aRet = RoAzureSupport.DownloadFile(strFileBlobName, type, RoAzureSupport.GetCompanyName)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetFileBytesFromAzure::", ex)
            aRet = {}
        End Try
        Return aRet
    End Function

    Public Shared Function GetFileSizeFromAzure(ByVal fileName As String, ByVal QueueType As roLiveQueueTypes, Optional bFromComanyContainer As Boolean = False, Optional strComapnyName As String = "") As Integer

        Dim fileSize As Integer = 0

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing

            oServiceConfiguration = New roServiceConfigurationRepository(QueueType).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            ' Retrieve a reference to a container.
            Dim strStorageContainer As String = oServiceConfiguration.storageblobcontainer

            If bFromComanyContainer AndAlso strComapnyName.Trim.Length > 0 Then
                strStorageContainer = strComapnyName.Trim
            ElseIf strComapnyName.Length > 0 Then
                fileName = strComapnyName & "/" & fileName
            End If

            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(strStorageContainer)
            containerClient.CreateIfNotExists()

            ' Create the blob client.
            Dim appendBlob As BlobClient = containerClient.GetBlobClient(fileName)

            If (appendBlob.Exists.Value) Then
                fileSize = appendBlob.GetProperties.Value.ContentLength
            Else
                fileSize = 0
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetFileSaSTokenWithURI::", ex)
            fileSize = String.Empty
        Finally

        End Try

        Return fileSize

    End Function

    Public Shared Function GetCommonTemplateBytesFromAzure(strFileBlobName As String, type As roLiveQueueTypes, Optional bAlertIfnotExists As Boolean = True) As Byte()
        Dim aRet As Byte() = {}

        Try
            aRet = RoAzureSupport.DownloadFile(strFileBlobName, type, "common_templates", bAlertIfnotExists)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetCommonTemplateBytesFromAzure::", ex)
            aRet = {}
        End Try
        Return aRet
    End Function

    Public Shared Function DeleteFileFromAzure(strFileBlobName As String, type As roLiveQueueTypes) As Boolean
        Dim bRet As Boolean

        Try
            bRet = Azure.RoAzureSupport.DeleteFileFromBlob(strFileBlobName, type, GetCompanyName)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::DeleteFileFromAzure::", ex)
            bRet = False
        End Try
        Return bRet
    End Function

#End Region

#Region "Upgrade DB version"

    Public Shared Function GetUpgradeDBMasterDic() As Dictionary(Of String, String)
        Dim oDic As New Dictionary(Of String, String)
        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing
            oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.upgrade).GetServiceConfiguration()

            Dim patchInfo As Byte() = DownloadFile("DBPatchIndex.vtu", roLiveQueueTypes.upgrade, "updates", False)

            If patchInfo.Length > 0 Then
                Dim memStream As New MemoryStream(patchInfo)
                Dim oReader As New StreamReader(memStream)
                While Not oReader.EndOfStream
                    Dim sRead As String = oReader.ReadLine()
                    Dim keys As String() = sRead.Split("=")

                    If keys.Length > 1 Then
                        oDic.Add(keys(0), keys(1))
                    End If

                End While
                oReader.Close()
                memStream.Close()
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetUpgradeDBMasterDic::", ex)
            If oDic IsNot Nothing Then
                oDic.Clear()
            End If
        End Try

        Return oDic
    End Function

    Public Shared Function GetUpgradeDBReportsDic() As Dictionary(Of String, String)
        Dim oDic As New Dictionary(Of String, String)
        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing
            oServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.upgrade).GetServiceConfiguration()

            Dim patchInfo As Byte() = DownloadFile("DBPatchIndex.vtu", roLiveQueueTypes.upgrade, "reports", False)

            If patchInfo.Length > 0 Then
                Dim memStream As New MemoryStream(patchInfo)
                Dim oReader As New StreamReader(memStream)
                While Not oReader.EndOfStream
                    Dim sRead As String = oReader.ReadLine()
                    Dim keys As String() = sRead.Split("=")

                    If keys.Length > 1 Then
                        oDic.Add(keys(0), keys(1))
                    End If

                End While
                oReader.Close()
                memStream.Close()
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetUpgradeDBReportsDic::", ex)
            If oDic IsNot Nothing Then
                oDic.Clear()
            End If
        End Try

        Return oDic
    End Function

    Public Shared Function CheckIfFileExists(filename As String, folder As String, _QueueType As roLiveQueueTypes, Optional oServiceConfiguration As roServiceConfiguration = Nothing, Optional sCompanyName As String = "") As Boolean

        Dim oRet As Boolean = True 'Lo ponemos a true para que en la función donde se llama ésta se registre un error y no continúe

        Try

            If oServiceConfiguration Is Nothing Then oServiceConfiguration = New roServiceConfigurationRepository(_QueueType).GetServiceConfiguration()
            If sCompanyName.Length = 0 Then sCompanyName = RoAzureSupport.GetCompanyName

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(sCompanyName)
            containerClient.CreateIfNotExists(PublicAccessType.None)

            Dim sFileName As String
            If folder.Length > 0 Then
                sFileName = folder & "/" & filename
            Else
                sFileName = filename
            End If
            Dim blockBlob As BlobClient = containerClient.GetBlobClient(sFileName)
            oRet = blockBlob.Exists().Value
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::CheckIfFileExists::", ex)
            oRet = True
        Finally

        End Try

        Return oRet
    End Function

    Public Shared Function GetUpgradeFile(fileName) As String()

        Dim oRet As String()

        Try
            Dim oServiceConfiguration As roServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.upgrade).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)

            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(oServiceConfiguration.storageblobcontainer)
            containerClient.CreateIfNotExists()

            Dim auditBlob As BlobClient = containerClient.GetBlobClient("updates/" & fileName)

            If auditBlob.Exists().Value = False Then
                oRet = {}
            Else
                Dim oAuditLines As New Generic.List(Of String)

                Using auditStream As Stream = auditBlob.OpenRead()
                    Using reader As StreamReader = New StreamReader(auditStream)
                        While Not reader.EndOfStream
                            oAuditLines.Add(reader.ReadLine.Trim)
                        End While
                    End Using
                End Using

                oRet = oAuditLines.ToArray()
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetUpgradeFile::", ex)
            oRet = {}
        Finally

        End Try

        Return oRet

    End Function

    Public Shared Function GetReportFile(fileName) As String()

        Dim oRet As String()

        Try
            Dim oServiceConfiguration As roServiceConfiguration = New roServiceConfigurationRepository(roLiveQueueTypes.upgrade).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)

            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(oServiceConfiguration.storageblobcontainer)
            containerClient.CreateIfNotExists()

            Dim auditBlob As BlobClient = containerClient.GetBlobClient("reports/" & fileName)

            If auditBlob.Exists().Value = False Then
                oRet = {}
            Else
                Dim oAuditLines As New Generic.List(Of String)

                Using auditStream As Stream = auditBlob.OpenRead()
                    Using reader As StreamReader = New StreamReader(auditStream)
                        While Not reader.EndOfStream
                            oAuditLines.Add(reader.ReadLine.Trim)
                        End While
                    End Using
                End Using

                oRet = oAuditLines.ToArray()
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetReportFile::", ex)
            oRet = {}
        Finally

        End Try

        Return oRet

    End Function

#End Region

#Region "Azure sas URI"

    Public Shared Function GetFileSaSTokenWithURI(ByVal fileName As String, ByVal QueueType As roLiveQueueTypes, Optional bFromComanyContainer As Boolean = False, Optional strComapnyName As String = "") As String

        Dim strBlobURI As String = String.Empty

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing

            oServiceConfiguration = New roServiceConfigurationRepository(QueueType).GetServiceConfiguration()

            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)
            ' Retrieve a reference to a container.
            Dim strStorageContainer As String = oServiceConfiguration.storageblobcontainer

            If bFromComanyContainer AndAlso strComapnyName.Trim.Length > 0 Then
                strStorageContainer = strComapnyName.Trim
            ElseIf strComapnyName.Length > 0 Then
                fileName = strComapnyName & "/" & fileName
            End If

            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(strStorageContainer)
            containerClient.CreateIfNotExists()

            Dim appendBlob As BlobClient = containerClient.GetBlobClient(fileName)
            If appendBlob.Exists.Value Then
                Dim sasKey As String = String.Empty
                If (QueueType = roLiveQueueTypes.analyticsbi And bFromComanyContainer) Then

                    Dim sasBuilder As New BlobSasBuilder() With {
                        .StartsOn = DateTimeOffset.UtcNow,
                        .ExpiresOn = DateTimeOffset.UtcNow.AddYears(2),
                        .Protocol = SasProtocol.Https
                    }
                    sasBuilder.SetPermissions(BlobAccountSasPermissions.Read)

                    strBlobURI = appendBlob.GenerateSasUri(sasBuilder).ToString()
                Else
                    Dim sasBuilder As New BlobSasBuilder() With {
                        .StartsOn = DateTimeOffset.UtcNow,
                        .ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(15),
                        .Protocol = SasProtocol.Https
                    }
                    sasBuilder.SetPermissions(BlobAccountSasPermissions.Read)

                    strBlobURI = appendBlob.GenerateSasUri(sasBuilder).ToString()
                End If
            Else
                strBlobURI = String.Empty
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GetFileSaSTokenWithURI::", ex)
            strBlobURI = String.Empty
        Finally

        End Try

        Return strBlobURI

    End Function

    Public Shared Function GenerateContainerSaSTokenWithURI(ByVal QueueType As roLiveQueueTypes, containerName As String, permission As String) As String

        Dim sasKey As String = String.Empty

        Try
            Dim oServiceConfiguration As roServiceConfiguration = Nothing

            oServiceConfiguration = New roServiceConfigurationRepository(QueueType).GetServiceConfiguration()
            Dim blobClient As BlobServiceClient = New BlobServiceClient(oServiceConfiguration.storageconnectionstring)

            Dim strStorageContainer As String = oServiceConfiguration.storageblobcontainer
            strStorageContainer = containerName.Trim

            Dim containerClient As BlobContainerClient = blobClient.GetBlobContainerClient(strStorageContainer)
            containerClient.CreateIfNotExists()

            Dim policyNumber As Integer = 1
            If containerClient.GetAccessPolicy().Value.SignedIdentifiers.Count > 0 AndAlso containerClient.GetAccessPolicy().Value.SignedIdentifiers.Where(Function(x) x.Id.Contains("externAccess")).Count > 0 Then
                policyNumber = Integer.Parse(containerClient.GetAccessPolicy().Value.SignedIdentifiers(0).Id.Replace("externAccess", ""))
            End If

            Dim signedIdentifier As New BlobSignedIdentifier With {
                .Id = "externAccess" & (policyNumber + 1).ToString(),
                .AccessPolicy = New BlobAccessPolicy() With {
                        .Permissions = permission,' rw = lectura y escritura, r = solo lectura
                        .StartsOn = DateTimeOffset.UtcNow,
                        .ExpiresOn = DateTimeOffset.UtcNow.AddYears(100)
                    }
            }

            containerClient.SetAccessPolicy(PublicAccessType.None, {signedIdentifier})

            sasKey = containerClient.GenerateSasUri(New BlobSasBuilder() With {
                        .Identifier = "externAccess" & (policyNumber + 1).ToString(),
                        .Protocol = SasProtocol.Https
                    }).ToString()
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoAzureSupport::GenerateContainerSaSTokenWithURI::", ex)
            sasKey = String.Empty
        Finally

        End Try

        Return sasKey

    End Function

#End Region

#Region "LogLevel"

    Public Shared Sub SetDefaultLogLevel(defaultLogLevel As Integer, defaultTraceLevel As Integer, Optional ByVal strAppName As String = "")
        Try

            If VTBase.roConstants.IsMultitenantServiceEnabled Then
                Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultLogLevel", defaultLogLevel)
                Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultTraceLevel", defaultTraceLevel)
                If strAppName <> String.Empty Then Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_AppName", strAppName)
            Else
                roConstants.SetGlobalEnvironmentParameter(GlobalAsaxParameter.AppName, strAppName)
                roConstants.SetGlobalEnvironmentParameter(GlobalAsaxParameter.LogFileName, strAppName)
                roConstants.SetGlobalEnvironmentParameter(GlobalAsaxParameter.LogLevel, defaultLogLevel)
                roConstants.SetGlobalEnvironmentParameter(GlobalAsaxParameter.TraceLevel, defaultTraceLevel)
            End If




        Catch ex As Exception
            roLog.GetInstance.logSystemMessage(roLog.EventType.roError, "RoAzureSupport::SetDefaultLogLevel::Could not set default log level", ex)
        End Try

    End Sub

#End Region

    Public Shared Function GetCompanyName() As String
        Dim strCompanyName As String = String.Empty
        Try
            If roTypes.Any2Boolean(VTBase.roConstants.GetConfigurationParameter("VTLive.MultitenantService")) Then
                strCompanyName = roTypes.Any2String(Threading.Thread.GetDomain.GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString & "_company"))
            Else
                strCompanyName = roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId))

                If strCompanyName = String.Empty Then
                    strCompanyName = roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.ClientCompanyId))

                    'Sólo aplicará si debugamos en local
                    If strCompanyName = String.Empty AndAlso VTBase.roTypes.Any2Boolean(VTBase.roConstants.GetConfigurationParameter("VTLive.DebugProcessLocally")) Then
                        roLog.GetInstance.logMessage(roLog.EventType.roCritic, "roAzureSupport::GetCompanyName:Alarm if you read that in PROD!")
                        strCompanyName = roTypes.Any2String(Threading.Thread.GetDomain.GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString & "_company"))
                    End If
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance.logMessage(roLog.EventType.roError, "roAzureSupport::GetCompanyName:Error recovering company name::", ex)
            strCompanyName = String.Empty
        End Try

        Return strCompanyName.Trim.ToLower
    End Function

End Class