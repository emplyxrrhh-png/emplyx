Imports System.ComponentModel
Imports Microsoft.QualityTools.Testing.Fakes
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTChannels
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports Xunit

Namespace Unit.Test

    <CollectionDefinition("Channels", DisableParallelization:=True)>
    <Collection("Channels")>
    <Category("Channels")>
    Public Class ChannelsTest
        Private ReadOnly helperDataLayer As DatalayerHelper
        Private ReadOnly helperAdvancedParameters As AdvancedParametersHelper
        Private ReadOnly helperNotifications As NotificationsHelper
        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperSecurity As SecurityHelper
        Private ReadOnly helperChannels As ChannelsHelper
        Private ReadOnly helperAzure As AzureHelper
        Private ReadOnly helperCryptography As CryptographyHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>

        ''' <summary>
        ''' Test Setup
        ''' </summary>

        Sub New()
            helperDataLayer = New DatalayerHelper
            helperAdvancedParameters = New AdvancedParametersHelper
            helperNotifications = New NotificationsHelper
            helperPassport = New PassportHelper
            helperSecurity = New SecurityHelper
            helperChannels = New ChannelsHelper
            helperAzure = New AzureHelper
            helperCryptography = New CryptographyHelper
        End Sub

        <Fact(DisplayName:="Should not load a channel if channels license is not installed")>
        Sub ShouldNotLoadAChannelIfChannelsLicenseIsNotInstalled()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Complaints"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)

                'Visibilidad sólo del canal 1
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"1"}, New Object() {"2"}}}
                dDataTStub.Add("sysrovwChannelEmployees", tMock)
                'Datos del canal 1
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("[dbo].[Channels]", tMock)
                'Empleados suscritos
                tMock = New DataTableMock With {.columns = {"IdEmployee"}, .values = New Object()() {New Object() {"10"}}}
                dDataTStub.Add("ChannelEmployees", tMock)
                'Departamentos suscritos
                tMock = New DataTableMock With {.columns = {"IdGroup"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelGroups", tMock)
                'Supervisores suscritos
                tMock = New DataTableMock With {.columns = {"IdSupervisor"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelSupervisors", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Como empleado 10 intento acceder al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(1,,, 10)

                'Assert
                Assert.Null(oChannel)
            End Using
        End Sub

        <Fact(DisplayName:="Should load a channel if channels license is installed")>
        Sub ShouldLoadAChannelIfChannelsLicenseNotInstalled()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)

                'Visibilidad sólo del canal 1
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"1"}, New Object() {"2"}}}
                dDataTStub.Add("sysrovwChannelEmployees", tMock)
                'Datos del canal 1
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("[dbo].[Channels]", tMock)
                'Empleados suscritos
                tMock = New DataTableMock With {.columns = {"IdEmployee"}, .values = New Object()() {New Object() {"10"}}}
                dDataTStub.Add("ChannelEmployees", tMock)
                'Departamentos suscritos
                tMock = New DataTableMock With {.columns = {"IdGroup"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelGroups", tMock)
                'Supervisores suscritos
                tMock = New DataTableMock With {.columns = {"IdSupervisor"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelSupervisors", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Como empleado 10 intento acceder al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(1,,, 10)

                'Assert
                Assert.NotNull(oChannel)
            End Using
        End Sub

        <Fact(DisplayName:="Should load complaint channel if channels license is installed")>
        Sub ShouldLoadComplaintChannelIfChannelsLicenseIsInstalled()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)

                'Visibilidad sólo del canal 1
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"1"}, New Object() {"2"}}}
                dDataTStub.Add("sysrovwChannelEmployees", tMock)
                'Datos del canal 1
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de denuncias", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "1"}}}
                dDataTStub.Add("[dbo].[Channels]", tMock)
                'Empleados suscritos
                tMock = New DataTableMock With {.columns = {"IdEmployee"}, .values = New Object()() {New Object() {"10"}}}
                dDataTStub.Add("ChannelEmployees", tMock)
                'Departamentos suscritos
                tMock = New DataTableMock With {.columns = {"IdGroup"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelGroups", tMock)
                'Supervisores suscritos
                tMock = New DataTableMock With {.columns = {"IdSupervisor"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelSupervisors", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Como empleado 10 intento acceder al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(1,,, 10)

                'Assert
                Assert.NotNull(oChannel)
            End Using
        End Sub

        <Fact(DisplayName:="Should load complaint channel if only complaints license is installed")>
        Sub ShouldLoadComplaintChannelIfOnlyComplaintsLicenseIsInstalled()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Complaints"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)

                'Visibilidad sólo del canal 1
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"1"}, New Object() {"2"}}}
                dDataTStub.Add("sysrovwChannelEmployees", tMock)
                'Datos del canal 1
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de denuncias", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "1"}}}
                dDataTStub.Add("Channels", tMock)
                'Empleados suscritos
                tMock = New DataTableMock With {.columns = {"IdEmployee"}, .values = New Object()() {New Object() {"10"}}}
                dDataTStub.Add("ChannelEmployees", tMock)
                'Departamentos suscritos
                tMock = New DataTableMock With {.columns = {"IdGroup"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelGroups", tMock)
                'Supervisores suscritos
                tMock = New DataTableMock With {.columns = {"IdSupervisor"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelSupervisors", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Como empleado 10 intento acceder al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(1,,, 10)

                'Assert
                Assert.NotNull(oChannel)
            End Using
        End Sub

        <Fact(DisplayName:="Should not load complaint channel if no license is installed")>
        Sub ShouldNotLoadComplaintChannelIfNoLicenseIsInstalled()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)

                'Visibilidad sólo del canal 1
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"1"}, New Object() {"2"}}}
                dDataTStub.Add("sysrovwChannelEmployees", tMock)
                'Datos del canal 1
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de denuncias", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("[dbo].[Channels]", tMock)
                'Empleados suscritos
                tMock = New DataTableMock With {.columns = {"IdEmployee"}, .values = New Object()() {New Object() {"10"}}}
                dDataTStub.Add("ChannelEmployees", tMock)
                'Departamentos suscritos
                tMock = New DataTableMock With {.columns = {"IdGroup"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelGroups", tMock)
                'Supervisores suscritos
                tMock = New DataTableMock With {.columns = {"IdSupervisor"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelSupervisors", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Como empleado 10 intento acceder al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(1,,, 10)

                'Assert
                Assert.Null(oChannel)
            End Using
        End Sub

        <Fact(DisplayName:="Employee can see channels if subscribed")>
        Sub EmployeeCanSeeChannelsIfSubscribed()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)

                'Visibilidad sólo del canal 1
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"1"}, New Object() {"2"}}}
                dDataTStub.Add("sysrovwChannelEmployees", tMock)
                'Datos del canal 1
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("[dbo].[Channels]", tMock)
                'Empleados suscritos
                tMock = New DataTableMock With {.columns = {"IdEmployee"}, .values = New Object()() {New Object() {"10"}}}
                dDataTStub.Add("ChannelEmployees", tMock)
                'Departamentos suscritos
                tMock = New DataTableMock With {.columns = {"IdGroup"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelGroups", tMock)
                'Supervisores suscritos
                tMock = New DataTableMock With {.columns = {"IdSupervisor"}, .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("ChannelSupervisors", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Como empleado 10 intento acceder al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(1,,, 10)

                'Assert
                Assert.True(oChannel.Id = 1)
            End Using
        End Sub

        <Fact(DisplayName:="Employee can not see channels if not subscribed")>
        Sub EmployeeCanSeeChannelsOnlyIfSubscribed()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Visibilidad como empleado sólo del canal 12
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"12"}}}
                dDataTStub.Add("sysrovwChannelEmployees", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Intento acceder como empleado al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(1,,, 1)

                'Assert
                Assert.True(oChannel Is Nothing AndAlso _state.Result = ChannelResultEnum.NoPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Employee can not see conversations of channels if not subscribed to the channel")>
        Sub EmployeeCanNotSeeConversationsOfChannelsIfNotSubscribedToTheChannel()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Visibilidad como empleado sólo del canal 12
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"12"}}}
                dDataTStub.Add("sysrovwChannelEmployees", tMock)
                helperDataLayer.ExecuteScalarStub("IdChannel FROM ChannelConversations WHERE Id", 13)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = oConversationManager.GetConversation(1, 1)

                'Assert
                Assert.True(oConversation Is Nothing AndAlso _state.Result = ConversationResultEnum.NoPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Employee can see conversations of channels if he is the conversation creator")>
        Sub EmployeeCanNotSeeConversationsOfChannelsIfHeIsNotTheConversationCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim employeeViewwerId As String = "10"
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Visibilidad como empleado sólo del canal 12
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"12"}}}
                dDataTStub.Add("sysrovwChannelEmployees", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"12", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "IdChannel", "CreatedBy", "CreatedOn", "Title", "ReferenceNumber", "IsAnonymous", "Status", "LastStatusChangeBy", "LastStatusChangeOn"},
                    .values = New Object()() {New Object() {"1", "12", "10", "2023-10-01", "Título", "0000000001", "0", "0", "1", "2023-10-01"}}}
                dDataTStub.Add("select.*channelconversations.*createdby.*\=.*" & employeeViewwerId & ".*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperDataLayer.ExecuteScalarStub("IdChannel FROM ChannelConversations WHERE Id", 12)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = oConversationManager.GetConversation(1, employeeViewwerId)

                'Assert
                Assert.True(oConversation.Id = 1 AndAlso oConversation.CreatedBy = employeeViewwerId)
            End Using
        End Sub

        <Fact(DisplayName:="Employee Id Is not revelaed when conversation is anonymous")>
        Sub EmployeeIdIsNotRevealedWhenConversationIsAnonymous()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Visibilidad como empleado sólo del canal 12
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"12"}}}
                dDataTStub.Add("sysrovwChannelEmployees", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "IdChannel", "CreatedBy", "CreatedOn", "Title", "ReferenceNumber", "IsAnonymous", "Status", "LastStatusChangeBy", "LastStatusChangeOn", "ExtraData", "Complexity"},
                    .values = New Object()() {New Object() {"1", "12", "1", "2023-10-01", "Título", "0000000001", "1", "0", "1", "2023-10-01", ""}}}
                dDataTStub.Add("\bselect\b.*\bchannelconversations\b.*\bcreatedby\b.*\b" & 1 & "\b*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperDataLayer.ExecuteScalarStub("IdChannel FROM ChannelConversations WHERE Id", 12)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = oConversationManager.GetConversation(1, 1)

                'Assert
                Assert.True(oConversation.Id = 1 AndAlso oConversation.CreatedBy = -1)
            End Using
        End Sub

        <Fact(DisplayName:="Consultant user can not see any channel")>
        Sub ConsultantUserCanNotSeeAnyChannel()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Si de la manera que sea consulto la tabla Channels, devuelvo uno.
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("Channels", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                Dim passport As roPassport = New roPassport()
                passport.Description = "@@ROBOTICS@@"
                helperPassport.PassportStub(1, helperDataLayer, passport)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Intento acceder como supervisor al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(1)

                'Assert
                Assert.True(oChannel Is Nothing AndAlso _state.Result = ChannelResultEnum.NoPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Supervisor Can Not See Any Channel If Permission Less Than Read")>
        Sub SupervisorCanNotSeeAnyChannelIfPermissionLessThanRead()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Si de la manera que sea consulto la tabla Channels, devuelvo uno.
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("Channels", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 1)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Intento acceder como supervisor al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(1)

                'Assert
                Assert.True(oChannel Is Nothing AndAlso _state.Result = ChannelResultEnum.NoPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Supervisor Can Not See Complaint Channel If Permission Over Complaint Channel Less Than Admin")>
        Sub SupervisorCanNotSeeComplaintChannelIfPermissionOverComplaintChannelLessThanAdmin()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Si de la manera que sea consulto la tabla Channels, devuelvo uno.
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"1", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "1"}}}
                dDataTStub.Add("IsComplaintChannel = 1", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Complaints", 1900, 7)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Intento acceder como supervisor al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(1)

                'Assert
                Assert.True(oChannel Is Nothing AndAlso _state.Result = ChannelResultEnum.NoPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Supervisor Can See Complaint Channel If Permission Over Complaint Channel Is Admin")>
        Sub SupervisorCanSeeComplaintChannelIfPermissionOverComplaintChannelIsAdmin()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Si de la manera que sea consulto la tabla Channels, devuelvo uno.
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel", "PrivacyPolicy"},
                    .values = New Object()() {New Object() {"1", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "1", ""}}}
                dDataTStub.Add("IsComplaintChannel = 1", tMock)
                dDataTStub.Add("[dbo].[Channels]", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Complaints", 1900, 9)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Intento acceder como supervisor al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(1)

                'Assert
                Assert.True(oChannel.Id = 1 AndAlso oChannel.IsComplaintChannel = True AndAlso _state.Result = ChannelResultEnum.NoError)
            End Using
        End Sub

        <Fact(DisplayName:="Supervisor can see any conversation on a channel only if he is subscribed")>
        Sub SupervisorCanSeeAnyConversationOnAChannelIfHeHasAdminPermission()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim supervisorViewwerId As String = "1"
                Dim allowedChannelId As String = "12"
                Dim allowedConversationId As String = "1"
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Supervisores suscritos
                tMock = New DataTableMock With {.columns = {"IdSupervisor"}, .values = New Object()() {New Object() {supervisorViewwerId}}}
                dDataTStub.Add("ChannelSupervisors.*where.*idchannel", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel", "PrivacyPolicy"},
                    .values = New Object()() {New Object() {allowedChannelId, "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0", ""}}}
                dDataTStub.Add(".*SELECT.*Channels.*", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "IdChannel", "CreatedBy", "CreatedOn", "Title", "ReferenceNumber", "IsAnonymous", "Status", "LastStatusChangeBy", "LastStatusChangeOn"},
                    .values = New Object()() {New Object() {allowedConversationId, allowedChannelId, "10", "2023-10-01", "Título", "0000000001", "0", "0", "1", "2023-10-01"}}}
                dDataTStub.Add("select.*channelconversations.*Id.*\=.*" & allowedConversationId & ".*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperDataLayer.ExecuteScalarStub("IdChannel FROM ChannelConversations WHERE Id", allowedChannelId)
                helperPassport.PassportStub(1, helperDataLayer)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 9)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = oConversationManager.GetConversation(1,)

                'Assert
                Assert.True(oConversation.Channel.SubscribedSupervisors.ToList.Contains(supervisorViewwerId))
            End Using
        End Sub

        <Fact(DisplayName:="Supervisor can not see any conversation on a channel if he is not subscribed even if he has admin permission")>
        Sub SupervisorCanNotSeeAnyConversationOnAChannelIfHeIsNotSubscribedEvenIfHeHasAdminPermission()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim supervisorViewwerId As String = "1"
                Dim allowedChannelId As String = "12"
                Dim allowedConversationId As String = "1"
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Supervisores suscritos
                tMock = New DataTableMock With {.columns = {"IdSupervisor"}, .values = New Object()() {New Object() {"111"}}}
                dDataTStub.Add("ChannelSupervisors.*where.*idchannel", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {allowedChannelId, "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add(".*SELECT.*Channels.*", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "IdChannel", "CreatedBy", "CreatedOn", "Title", "ReferenceNumber", "IsAnonymous", "Status", "LastStatusChangeBy", "LastStatusChangeOn", "ExtraData", "Complexity"},
                    .values = New Object()() {New Object() {allowedConversationId, allowedChannelId, "10", "2023-10-01", "Título", "0000000001", "0", "0", "1", "2023-10-01", "", "0"}}}
                dDataTStub.Add("select.*channelconversations.*Id.*\=.*" & allowedConversationId & ".*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperDataLayer.ExecuteScalarStub("IdChannel FROM ChannelConversations WHERE Id", allowedChannelId)
                helperPassport.PassportStub(1, helperDataLayer)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 9)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = oConversationManager.GetConversation(1,)

                'Assert
                Assert.Null(oConversation)
            End Using
        End Sub

        <Fact(DisplayName:="Supervisor can see any conversation on complaint channel if he is not subscribed and he has admin permissiono over complaint channel")>
        Sub SupervisorCanSeeAnyConversationOnComplaintChannelIfHeIsNotSubscribedAndHeHasAdminPermissionoOverComplaintChannel()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim supervisorViewwerId As String = "1"
                Dim allowedChannelId As String = "12"
                Dim allowedConversationId As String = "1"
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Supervisores suscritos
                tMock = New DataTableMock With {.columns = {"IdSupervisor"}, .values = New Object()() {New Object() {"111"}}}
                dDataTStub.Add("ChannelSupervisors.*where.*idchannel", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {allowedChannelId, "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "1"}}}
                dDataTStub.Add(".*SELECT.*Channels.*", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "IdChannel", "CreatedBy", "CreatedOn", "Title", "ReferenceNumber", "IsAnonymous", "Status", "LastStatusChangeBy", "LastStatusChangeOn"},
                    .values = New Object()() {New Object() {allowedConversationId, allowedChannelId, "10", "2023-10-01", "Título", "0000000001", "0", "0", "1", "2023-10-01"}}}
                dDataTStub.Add("select.*channelconversations.*Id.*\=.*" & allowedConversationId & ".*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperDataLayer.ExecuteScalarStub("IdChannel FROM ChannelConversations WHERE Id", allowedChannelId)
                helperPassport.PassportStub(1, helperDataLayer)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Complaints", 1900, 9)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = oConversationManager.GetConversation(1,)

                'Assert
                Assert.NotNull(oConversation)
            End Using
        End Sub

        <Fact(DisplayName:="Can not create conversation if identifier already exists")>
        Sub CanNotCreateConversationIfIdentifierAlreadyExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = New roConversation With {.Id = 2}
                Dim ret As Boolean = oConversationManager.CreateConversation(oConversation, Nothing)

                'Assert
                Assert.True(Not ret AndAlso _state.Result = ConversationResultEnum.ErrorConversationAlreadyExists)
            End Using
        End Sub

        <Fact(DisplayName:="Can not create conversation without initial message")>
        Sub CanNotCreateConversationWithoutInitialMessage()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = New roConversation With {.CreatedBy = 1}
                Dim oChannel As roChannel = New roChannel With {.Id = 1}
                oConversation.Channel = oChannel
                Dim ret As Boolean = oConversationManager.CreateConversation(oConversation, Nothing)

                'Assert
                Assert.True(Not ret AndAlso _state.Result = ConversationResultEnum.MessageRequiredToCreateConversation)
            End Using
        End Sub

        <Fact(DisplayName:="Can not create conversation with empty initial message")>
        Sub CanNotCreateConversationWithEmptyInitialMessage()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = New roConversation With {.CreatedBy = 1}
                Dim oChannel As roChannel = New roChannel With {.Id = 1}
                oConversation.Channel = oChannel
                Dim oInitialMessage As roMessage = New roMessage With {.Body = " "}
                Dim ret As Boolean = oConversationManager.CreateConversation(oConversation, oInitialMessage)

                'Assert
                Assert.True(Not ret AndAlso _state.Result = ConversationResultEnum.InitialMessageCanNotBeEmpty)
            End Using
        End Sub

        <Fact(DisplayName:="Only Subscribed Employees Can Create Conversations In A Channel")>
        Sub OnlyEmployeesCanCreateConversationsInAChannel()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim viewerEmployeeId As String = "10"
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Visibilidad como empleado sólo del canal 12
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"12"}}}
                dDataTStub.Add("sysrovwChannelEmployees.*IdEmployee.*\=.*" & viewerEmployeeId & ".*", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"12", "Canal de pruebas", "1", "1", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperDataLayer.StartTransaction()
                helperDataLayer.EndTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = New roConversation With {.CreatedBy = CInt(viewerEmployeeId)}
                Dim oChannel As roChannel = New roChannel With {.Id = 12}
                oConversation.Channel = oChannel
                oConversation.CreatedOn = Now
                Dim oInitialMessage As roMessage = New roMessage With {.Body = "Hola qué tal?"}
                oInitialMessage.Conversation = oConversation
                oInitialMessage.CreatedBy = CInt(viewerEmployeeId)
                Dim ret As Boolean = oConversationManager.CreateConversation(oConversation, oInitialMessage)

                'Assert
                Assert.True(ret AndAlso _state.Result = ConversationResultEnum.NoError)
            End Using
        End Sub

        <Fact(DisplayName:="Non Subscribed Employees Can Not Create Conversations In A Channel")>
        Sub NonEmployeesCanNotCreateConversationsInAChannel()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim viewerEmployeeId As String = "10"
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Visibilidad como empleado sólo del canal 10. No del 12 que es donde intentaré crer la conversación
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"10"}}}
                dDataTStub.Add("sysrovwChannelEmployees.*IdEmployee.*\=.*" & viewerEmployeeId & ".*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperDataLayer.StartTransaction()
                helperDataLayer.EndTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = New roConversation With {.CreatedBy = CInt(viewerEmployeeId)}
                Dim oChannel As roChannel = New roChannel With {.Id = 12}
                oConversation.Channel = oChannel
                Dim oInitialMessage As roMessage = New roMessage With {.Body = "Hola qué tal?"}
                Dim ret As Boolean = oConversationManager.CreateConversation(oConversation, oInitialMessage)

                'Assert
                Assert.True(Not ret AndAlso _state.Result = ConversationResultEnum.NoPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Conversation Should Be Created By A Known Employee")>
        Sub ConversationShouldBeCreatedByAKnownEmployee()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = New roConversation
                Dim oInitialMessage As roMessage = New roMessage With {.Body = "Hola qué hase?"}
                oConversation.Channel = New roChannel With {.Id = 1}
                Dim ret As Boolean = oConversationManager.CreateConversation(oConversation, Nothing)

                'Assert
                Assert.True(Not ret AndAlso _state.Result = ConversationResultEnum.ConversationShouldBeCreatedByAnEmployee)
            End Using
        End Sub

        <Fact(DisplayName:="Can not create a conversation on a draft channel")>
        Sub CanNotCreateAConversationOnADraftChannel()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)
                Dim viewerEmployeeId As String = "10"
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Visibilidad como empleado sólo del canal 12
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"12"}}}
                dDataTStub.Add("sysrovwChannelEmployees.*IdEmployee.*\=.*" & viewerEmployeeId & ".*", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"12", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = New roConversation
                oConversation.CreatedBy = 1
                Dim oInitialMessage As roMessage = New roMessage With {.Body = "Hola qué hase?"}
                oConversation.Channel = New roChannel With {.Id = 12}
                oInitialMessage.Conversation = oConversation
                Dim ret As Boolean = oConversationManager.CreateConversation(oConversation, oInitialMessage)

                'Assert
                Assert.True(Not ret AndAlso _state.Result = ConversationResultEnum.ChannelShouldBePublishedToCreateConversations)
            End Using
        End Sub

        <Fact(DisplayName:="Can not create a message on a closed  conversation")>
        Sub CanNotCreateAMessageOnAClosedOrDismissedConversation()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)
                Dim viewerEmployeeId As String = "10"
                helperDataLayer.StartTransaction()
                helperDataLayer.EndTransaction()

                'Act
                Dim _state As roMessageState = New roMessageState(1)
                Dim oMessageManager As roMessageManager = New roMessageManager(_state)
                Dim oMessage As New roMessage With {.Body = "Hola qué hase?", .CreatedBy = CInt(viewerEmployeeId)}
                Dim oConversation As New roConversation With {.Id = 1, .Channel = New roChannel With {.Id = 12}, .Status = ConversationStatusEnum.Closed}
                oMessage.Conversation = oConversation
                Dim ret As Boolean = oMessageManager.CreateMessage(oMessage)

                'Assert
                Assert.True(Not ret AndAlso _state.Result = MessageResultEnum.ConversationClosedOrDismissed)
            End Using
        End Sub

        <Fact(DisplayName:="Can not create a message on a dismissed  conversation")>
        Sub CanNotCreateAMessageOnADismissedConversation()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)
                Dim viewerEmployeeId As String = "10"
                helperDataLayer.StartTransaction()
                helperDataLayer.EndTransaction()

                'Act
                Dim _state As roMessageState = New roMessageState(1)
                Dim oMessageManager As roMessageManager = New roMessageManager(_state)
                Dim oMessage As New roMessage With {.Body = "Hola qué hase?", .CreatedBy = CInt(viewerEmployeeId)}
                Dim oConversation As New roConversation With {.Id = 1, .Channel = New roChannel With {.Id = 12}, .Status = ConversationStatusEnum.Dismissed}
                oMessage.Conversation = oConversation
                Dim ret As Boolean = oMessageManager.CreateMessage(oMessage)

                'Assert
                Assert.True(Not ret AndAlso _state.Result = MessageResultEnum.ConversationClosedOrDismissed)
            End Using
        End Sub

        <Fact(DisplayName:="Can create a conversation on a Published channel")>
        Sub CanCreateAConversationOnAPublishedChannel()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)
                Dim viewerEmployeeId As String = "10"
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Visibilidad como empleado sólo del canal 12
                Dim tMock As New DataTableMock With {.columns = {"Id"}, .values = New Object()() {New Object() {"12"}}}
                dDataTStub.Add("sysrovwChannelEmployees.*IdEmployee.*\=.*" & viewerEmployeeId & ".*", tMock)
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel"},
                    .values = New Object()() {New Object() {"12", "Canal de pruebas", "1", "1", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "0"}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperDataLayer.StartTransaction()
                helperDataLayer.EndTransaction()

                'Act
                Dim _state As roConversationState = New roConversationState(1)
                Dim oConversationManager As roConversationManager = New roConversationManager(_state)
                Dim oConversation As roConversation = New roConversation
                oConversation.CreatedBy = 1
                oConversation.CreatedOn = Now
                Dim oInitialMessage As roMessage = New roMessage With {.Body = "Hola qué hase?", .CreatedBy = CInt(viewerEmployeeId)}
                oInitialMessage.Conversation = oConversation
                oConversation.Channel = New roChannel With {.Id = 12}
                Dim ret As Boolean = oConversationManager.CreateConversation(oConversation, oInitialMessage)

                'Assert
                Assert.True(ret AndAlso _state.Result = ConversationResultEnum.NoError)
            End Using
        End Sub

        <Fact(DisplayName:="Should Save Employees Subscribed If Any")>
        Sub ShouldSaveEmployeesSubscribedIfAny()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"Global.ValidateTextIntegrity", ""}})
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 9)
                helperDataLayer.ExecuteSqlSpy()
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.StartTransaction()
                helperDataLayer.EndTransaction()

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim oChannel As roChannel = helperChannels.GetNewChannel(True, False, False)
                oChannelManager.CreateOrUpdateChannel(oChannel)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.ChannelEmployeesInserted)
            End Using
        End Sub

        <Fact(DisplayName:="Should Save Groups Subscribed If Any")>
        Sub ShouldSaveGroupsSubscribedIfAny()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Árrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"Global.ValidateTextIntegrity", ""}})
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 9)
                helperDataLayer.ExecuteSqlSpy()
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.StartTransaction()
                helperDataLayer.EndTransaction()

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim oChannel As roChannel = helperChannels.GetNewChannel(True, True, False)
                oChannelManager.CreateOrUpdateChannel(oChannel)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.ChannelGroupsInserted)
            End Using
        End Sub

        <Fact(DisplayName:="Should Save Supervisors Subscribed If Any")>
        Sub ShouldSaveSupervisorsSubscribedIfAny()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Árrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"Global.ValidateTextIntegrity", ""}})
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 9)
                helperDataLayer.ExecuteSqlSpy()
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.StartTransaction()
                helperDataLayer.EndTransaction()

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim oChannel As roChannel = helperChannels.GetNewChannel(True, True, True)
                oChannelManager.CreateOrUpdateChannel(oChannel)

                'Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.ChannelSupervisorsInserted)
            End Using
        End Sub

        <Fact(DisplayName:="Channel Id Should Be Returned When New Channel Saved")>
        Sub ChannelIdShouldBeReturnedWhenNewChannelSaved()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Árrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"Global.ValidateTextIntegrity", ""}})
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 9)
                helperDataLayer.ExecuteSqlSpy()
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.StartTransaction()
                helperDataLayer.EndTransaction()
                helperDataLayer.ExecuteScalarWithParametersStub(New Dictionary(Of String, Object) From {{"INTO Channels ([Title]", 1000}})

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim oChannel As roChannel = helperChannels.GetNewChannel(True, True, True)
                oChannelManager.CreateOrUpdateChannel(oChannel)

                'Assert
                Assert.True(oChannel.Id = 1000)
            End Using
        End Sub

        <Fact(DisplayName:="Channel Can Not Be Deleted If Supervisor Has No Access")>
        Sub ChannelCanNotBeDeletedIfSupervisorHasNoAccess()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Árrange
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 5)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim oChannel As roChannel = helperChannels.GetExistingChannel(1200, 1, True, True, True)
                Dim ret As Boolean = oChannelManager.DeleteChannel(oChannel)

                'Assert
                Assert.True(Not ret AndAlso _state.Result = ChannelResultEnum.NoPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Channel Can Not Be Deleted If Supervisor Has Only Write Access But Is Not Channel Creator")>
        Sub ChannelCanNotBeDeletedIfSupervisorHasOnlyWriteAccessButIsNotChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Árrange
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 6)
                helperPassport.PassportStub(1, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim oChannel As roChannel = helperChannels.GetExistingChannel(1, 5, True, True, True)
                Dim ret As Boolean = oChannelManager.DeleteChannel(oChannel)

                'Assert
                Assert.True(Not ret AndAlso _state.Result = ChannelResultEnum.NoPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Channel Can Be Deleted If Supervisor Has Only Write Access And Is Channel Creator")>
        Sub ChannelCanBeDeletedIfSupervisorHasOnlyWriteAccessAndIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Árrange
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 6)
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim oChannel As roChannel = helperChannels.GetExistingChannel(1, 1, True, True, True)
                Dim ret As Boolean = oChannelManager.DeleteChannel(oChannel)

                'Assert
                Assert.True(ret AndAlso helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.ChannelUpdated AndAlso _state.Result = ChannelResultEnum.NoError)
            End Using
        End Sub

        <Fact(DisplayName:="Channel Can Be Deleted If Supervisor Has Admin Access Even If He Is Not Channel Creator")>
        Sub ChannelCanBeDeletedIfSupervisorHasAdminAccessEvenIfHeIsNotChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Árrange
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 9)
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim oChannel As roChannel = helperChannels.GetExistingChannel(1, 5, True, True, True)
                Dim ret As Boolean = oChannelManager.DeleteChannel(oChannel)

                'Assert
                Assert.True(ret AndAlso helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.ChannelUpdated AndAlso _state.Result = ChannelResultEnum.NoError)
            End Using
        End Sub

        <Fact(DisplayName:="No Database Row Is Deleted When Channel Is Deleted")>
        Sub NoDatabaseRowIsDeletedWhenChannelIsDeleted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Árrange
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 9)
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.ExecuteSqlSpy()

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim oChannel As roChannel = helperChannels.GetExistingChannel(1, 5, True, True, True)
                Dim ret As Boolean = oChannelManager.DeleteChannel(oChannel)

                'Assert
                Assert.True(ret AndAlso helperDataLayer.ExecuteSqlWasCalled <> DatalayerHelper.SqlExecuteString.ChannelDeleted AndAlso _state.Result = ChannelResultEnum.NoError)
            End Using
        End Sub

        <Fact(DisplayName:="Supervisor Can Only See Their Channels If Access Level Is Write")>
        Sub SupervisorCanOnlySeeTheirChannelsIfAccessLevelIsWrite()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Sólo devuelvo filas si la consulta filtra por IdCreatedBy. Sería equivalente poner un spy
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel", "PrivacyPolicy"},
                    .values = New Object()() {New Object() {"121", "Canal de pruebas", "3", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "1", ""}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*\bIdCreatedBy\b.*", tMock)
                dDataTStub.Add("[dbo].[Channels]", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 6)
                helperPassport.PassportStub(3, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Intento acceder como supervisor al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(121)

                'Assert
                Assert.True(oChannel.Id = 121 AndAlso oChannel.CreatedBy = 3 AndAlso _state.Result = ChannelResultEnum.NoError)
            End Using
        End Sub

        <Fact(DisplayName:="Supervisors Can Only See Channels In Wich They Are Subscribed If Access Level Is Read")>
        Sub SupervisorsCanOnlySeeChannelsInWichTheyAreSubscribedIfAccessLevelIsRead()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Sólo devuelvo filas si la consulta filtra por supervisores suscritos
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel", "PrivacyPolicy"},
                    .values = New Object()() {New Object() {"121", "Canal de pruebas", "3", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "0", "2023-10-01", "1", ""}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*\bSELECT\b.*\bIdChannel\b.*\bFROM\b.*\bChannelSupervisors\b.*", tMock)
                dDataTStub.Add("[dbo].[Channels]", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 3)
                helperPassport.PassportStub(3, helperDataLayer)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Intento acceder como supervisor al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(121)

                'Assert
                Assert.True(oChannel.Id = 121 AndAlso _state.Result = ChannelResultEnum.NoError)
            End Using
        End Sub

        <Fact(DisplayName:="Name Of Supervisor Who Created Channel Should Be Informed On Channel When Recovered")>
        Sub NameOfSupervisorWhoCreatedChannelShouldBeInformedOnChannelWhenRecovered()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Sólo devuelvo filas si la consulta filtra por supervisores suscritos
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel", "PrivacyPolicy"},
                    .values = New Object()() {New Object() {"121", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "1", "2023-10-01", "1", ""}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                Dim passport As New roPassport
                passport.Name = "Creador"
                helperPassport.PassportStub(1, helperDataLayer, passport)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 9)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Intento acceder como supervisor al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(121)

                'Assert
                Assert.True(oChannel.CreatedByName = "Creador" AndAlso _state.Result = ChannelResultEnum.NoError)
            End Using
        End Sub

        <Fact(DisplayName:="Name Of Supervisor Who Last Modified Channel Should Be Informed On Channel When Recovered")>
        Sub NameOfSupervisorWhoLastModifiedChannelShouldBeInformedOnChannelWhenRecovered()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Sólo devuelvo filas si la consulta filtra por supervisores suscritos
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel", "PrivacyPolicy"},
                    .values = New Object()() {New Object() {"121", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "1", "2023-10-01", "1", ""}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                Dim passport As New roPassport
                passport.Name = "Modificador"
                helperPassport.PassportStub(1, helperDataLayer, passport)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 9)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Intento acceder como supervisor al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(121)

                'Assert
                Assert.True(oChannel.ModifiedByName = "Modificador" AndAlso _state.Result = ChannelResultEnum.NoError)
            End Using
        End Sub

        <Fact(DisplayName:="Name Of Supervisor Who Deleted Channel Should Be Informed On Channel When Recovered")>
        Sub NameOfSupervisorWhoDeletedChannelShouldBeInformedOnChannelWhenRecovered()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                CommonHelper.InitLicense(New List(Of String) From {"Feature\Channels", "Feature\Complaints"})
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Sólo devuelvo filas si la consulta filtra por supervisores suscritos
                tMock = New DataTableMock With {.columns = {"Id", "Title", "IdCreatedBy", "Status", "CreatedOn", "IdModifiedBy", "ModifiedOn", "PublishedOn", "ReceiptAcknowledgment", "AllowAnonymous", "Deleted", "IdDeletedBy", "DeletedOn", "IsComplaintChannel", "PrivacyPolicy"},
                    .values = New Object()() {New Object() {"121", "Canal de pruebas", "1", "0", "2023-10-01", "1", "2023-10-01", "2023-10-01", "1", "1", "0", "1", "2023-10-01", "1", ""}}}
                dDataTStub.Add("\bSELECT\b.*\bChannels\b.*", tMock)
                Dim dtEmployeeChannels As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                Dim passport As New roPassport
                passport.Name = "Eliminador"
                helperPassport.PassportStub(1, helperDataLayer, passport)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Channels", 1850, 9)

                'Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                'Intento acceder como supervisor al canal 1
                Dim oChannel As roChannel = oChannelManager.GetChannel(121)

                'Assert
                Assert.True(oChannel.DeletedByName = "Eliminador" AndAlso _state.Result = ChannelResultEnum.NoError)
            End Using
        End Sub

        <Fact(DisplayName:="Should not return complaint log messages if user dont have permissions over complaint channels")>
        Sub ShouldNotReturnComplaintLogMessagesIfUserDontHavePermissionsOverComplaintChannels()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Complaints", 1900, 0)
                'Act
                Dim oState As roLogBookState = New roLogBookState(1)
                Dim oLogBookManager As New roLogBookManager(oState)
                Dim oMessages = oLogBookManager.GetComplaintLog(complaintRef, bAudit)
                'Assert
                Assert.True(oMessages.Count = 0)
            End Using
        End Sub

        <Fact(DisplayName:="Should not return complaint log messages if user have reading permissions over complaint channels")>
        Sub ShouldNotReturnComplaintLogMessagesIfUserHaveReadingPermissionsOverComplaintChannels()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Complaints", 1900, 3)
                'Act
                Dim oState As roLogBookState = New roLogBookState(1)
                Dim oLogBookManager As New roLogBookManager(oState)
                Dim oMessages = oLogBookManager.GetComplaintLog(complaintRef, bAudit)
                'Assert
                Assert.True(oMessages.Count = 0)
            End Using
        End Sub

        <Fact(DisplayName:="Should not return complaint log messages if user have writing permissions over complaint channels")>
        Sub ShouldNotReturnComplaintLogMessagesIfUserHaveWritingPermissionsOverComplaintChannels()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Complaints", 1900, 6)
                'Act
                Dim oState As roLogBookState = New roLogBookState(1)
                Dim oLogBookManager As New roLogBookManager(oState)
                Dim oMessages = oLogBookManager.GetComplaintLog(complaintRef, bAudit)
                'Assert
                Assert.True(oMessages.Count = 0)
            End Using
        End Sub

        <Fact(DisplayName:="Should not return complaint log messages if user is consultor")>
        Sub ShouldNotReturnComplaintLogMessagesIfUserIsConsultor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oPassport As roPassport = New roPassport()
                oPassport.ID = 1
                oPassport.Description = "@@ROBOTICS@@Consultor"
                helperPassport.PassportStub(1, helperDataLayer, oPassport)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Complaints", 1900, 9)
                'Act
                Dim oState As roLogBookState = New roLogBookState(1)
                Dim oLogBookManager As New roLogBookManager(oState)
                Dim oMessages = oLogBookManager.GetComplaintLog(complaintRef, bAudit)
                'Assert
                Assert.True(oMessages.Count = 0)
            End Using
        End Sub

        <Fact(DisplayName:="Should return complaint log messages if user has permissions and is not consultor")>
        Sub ShouldReturnComplaintLogMessagesIfUserHasPermissionsAndIsNotConsultor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Complaints", 1900, 9)
                helperAzure.GetAzureLogBookByReferenceStub("complaintRef")
                helperAzure.GetCompanySecretStub("company")
                CommonHelper.DecryptMessage()

                helperCryptography.DecryptExStub("encrypted", "decrypted")
                'Act
                Dim oState As roLogBookState = New roLogBookState(1)
                Dim oLogBookManager As New roLogBookManager(oState)
                Dim oMessages = oLogBookManager.GetComplaintLog("complaintRef", bAudit)
                'Assert
                Assert.True(oMessages.Count > 0)
            End Using
        End Sub

        <Fact(DisplayName:="Should decrypt log book messages")>
        Sub ShouldDecryptLogBookMessages()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDataLayer)
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Employees.Complaints", 1900, 9)
                helperAzure.GetAzureLogBookByReferenceStub("complaintRef")
                helperAzure.GetCompanySecretStub("company")


                CommonHelper.DecryptMessage()

                helperCryptography.DecryptExSpy("", "")
                'Act
                Dim oState As roLogBookState = New roLogBookState(1)
                Dim oLogBookManager As New roLogBookManager(oState)
                Dim oMessages = oLogBookManager.GetComplaintLog("complaintRef", bAudit)
                'Assert
                Assert.True(helperCryptography.MessageDecrypted)
            End Using
        End Sub

        <Fact(DisplayName:="Should return True when no old messages was found")>
        Public Sub ShouldReturnTrueWhenNoOldMessagesWasFound()
            Using s = ShimsContext.Create()
                ' Arrange
                Dim complexity As New ConversationComplexity()
                Dim limitDate As New DateTime(2023, 1, 1)

                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim fakeDt As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)

                ' Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim result As Boolean = oChannelManager.DeleteOldMessagesByComplexity(complexity, limitDate)

                ' Assert
                Assert.True(result)
            End Using
        End Sub

        <Fact(DisplayName:="Should not call deleteChannelConversations on SQL when no old messages was found")>
        Public Sub ShouldNotCallDeleteChannelConversationsOnSQLWhenNoOldMessagesWasFound()
            Using s = ShimsContext.Create()
                ' Arrange
                Dim complexity As New ConversationComplexity()
                Dim limitDate As New DateTime(2023, 1, 1)

                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim result As Boolean = oChannelManager.DeleteOldMessagesByComplexity(complexity, limitDate)

                ' Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.ChannelConversationsDeleted)
            End Using
        End Sub

        <Fact(DisplayName:="Should not call deleteChannelConversationsMessages on SQL when no old messages was found")>
        Public Sub ShouldNotCallDeleteChannelConversationsMessagesOnSQLWhenNoOldMessagesWasFound()
            Using s = ShimsContext.Create()
                ' Arrange
                Dim complexity As New ConversationComplexity()
                Dim limitDate As New DateTime(2023, 1, 1)

                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim result As Boolean = oChannelManager.DeleteOldMessagesByComplexity(complexity, limitDate)

                ' Assert
                Assert.False(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.ChannelConversationMessagesDeleted)
            End Using
        End Sub

        <Fact(DisplayName:="Should Return True when old messages are deleted")>
        Public Sub ShouldReturnTrueWhenOldMessagesAreDeleted()
            Using s = ShimsContext.Create()
                ' Arrange
                Dim complexity As New ConversationComplexity()
                Dim limitDate As New DateTime(2023, 1, 1)

                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                tMock = New DataTableMock With {.columns = {"ID"},
                .values = New Object()() {New Object() {1}}}
                dDataTStub.Add("Channels", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim result As Boolean = oChannelManager.DeleteOldMessagesByComplexity(complexity, limitDate)

                ' Assert
                Assert.True(result)
            End Using
        End Sub

        <Fact(DisplayName:="Should call deleteChannelConversations when old messages are deleted")>
        Public Sub ShouldCallDeleteChannelConversationsWhenOldMessagesAreDeleted()
            Using s = ShimsContext.Create()
                ' Arrange
                Dim complexity As New ConversationComplexity()
                Dim limitDate As New DateTime(2023, 1, 1)

                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                tMock = New DataTableMock With {.columns = {"ID"},
                .values = New Object()() {New Object() {1}}}
                dDataTStub.Add("Channels", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim result As Boolean = oChannelManager.DeleteOldMessagesByComplexity(complexity, limitDate)

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.ChannelConversationsDeleted)
            End Using
        End Sub

        <Fact(DisplayName:="Should return false when deletion fails from sql")>
        Public Sub ShouldReturnFalseWhenDeletionFailsFromSql()
            Using s = ShimsContext.Create()
                ' Arrange
                Dim complexity As New ConversationComplexity()
                Dim limitDate As New DateTime(2023, 1, 1)

                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                tMock = New DataTableMock With {.columns = {"ID"},
                .values = New Object()() {New Object() {1}}}
                dDataTStub.Add("Channels", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)

                ' Simulamos un error en el delete del sql
                Robotics.DataLayer.Fakes.ShimAccessHelper.ExecuteSqlStringroBaseConnection = Function() False

                ' Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim result As Boolean = oChannelManager.DeleteOldMessagesByComplexity(complexity, limitDate)

                ' Assert
                Assert.False(result)
            End Using
        End Sub

        <Fact(DisplayName:="Should return false when an exception ocurrs during deletion")>
        Public Sub ShouldReturnFalseWhenAnExceptionOcurrsDuringDeletion()
            Using s = ShimsContext.Create()
                ' Arrange
                Dim complexity As New ConversationComplexity()
                Dim limitDate As New DateTime(2023, 1, 1)

                Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function()
                                                                                                            ' Throw exception
                                                                                                            Throw New Exception("Test")
                                                                                                        End Function
                helperPassport.PassportStub(1, helperDataLayer)

                ' Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim result As Boolean = oChannelManager.DeleteOldMessagesByComplexity(complexity, limitDate)

                ' Assert
                Assert.False(result)
                'Assert.Contains("roChannelManager::DeleteOldMessagesByComplexity", oChannelManager.State.ErrorText) 'Revisar
            End Using
        End Sub

        <Fact(DisplayName:="Should delete all old messages when multiple channelConversations are founded")>
        Public Sub ShouldDeleteAllOldMessagesWhenMultipleChannelConversationsAreFounded()
            Using s = ShimsContext.Create()
                ' Arrange
                Dim complexity As New ConversationComplexity()
                Dim limitDate As New DateTime(2023, 1, 1)

                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {
                    .columns = {"ID"},
                    .values = New Object()() {
                        New Object() {1},
                        New Object() {2},
                        New Object() {3}
                    }
                }
                dDataTStub.Add("Channels", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)
                helperPassport.PassportStub(1, helperDataLayer)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                Dim _state As roChannelState = New roChannelState(1)
                Dim oChannelManager As roChannelManager = New roChannelManager(_state)
                Dim result As Boolean = oChannelManager.DeleteOldMessagesByComplexity(complexity, limitDate)

                ' Assert
                Assert.True(result)
                Assert.Equal(3, helperDataLayer.ExecuteSqlCallCountTotal("DeleteChannelConversationMessages"))
                Assert.Equal(3, helperDataLayer.ExecuteSqlCallCountTotal("DeleteChannelConversations"))
            End Using
        End Sub

    End Class

End Namespace