Imports System.ComponentModel
Imports Robotics.Base.DTOs
Imports VT_XU_Base
Imports VT_XU_Security
Imports Robotics.Security.Base
Imports Xunit

Namespace Unit.Test

    <CollectionDefinition("Passport", DisableParallelization:=True)>
    <Collection("Passport")>
    <Category("Passport")>
    Public Class PassportTest

        Private ReadOnly datalayer As DatalayerHelper
        Private ReadOnly helper As PassportHelper
        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helper = New PassportHelper
            datalayer = New DatalayerHelper()
        End Sub

#Region "Load Passports"
        <Fact(DisplayName:="Should Get Nothing As User Passport When SysroPassports Count Is Zero")>
        Sub ShouldGetNothingAsUserPassportWhenSysroPassportsCountIsZero()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim id As Integer = 1
                Dim loadType As LoadType = LoadType.User

                'Act
                Dim _passport As roPassport = roPassportManager.GetPassport(id, loadType)

                'Assert
                Assert.Equal(_passport, Nothing)
            End Using
        End Sub

        <Fact(DisplayName:="Should Get User Passport When SysroPassports Count NOT Is Zero")>
        Sub ShouldGetUserPassportWhenSysroPassportsCountNotIsZero()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                datalayer.CreateDataTableSpy()
                Dim id As Integer = 2
                Dim loadType As LoadType = LoadType.User

                helper.PassportStub(id, datalayer)

                'Act
                Dim _passport As roPassport = roPassportManager.GetPassport(id, loadType)

                'Assert
                Assert.NotEqual(_passport, Nothing)

            End Using
        End Sub

        <Fact(DisplayName:="Should Use IdEmployee Parameter When Get Employee Passport")>
        Sub ShouldUseIdEmployeeParameterWhenGetEmployeePassport()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                datalayer.CreateDataTableSpy()
                Dim id As Integer = 3
                Dim loadType As LoadType = LoadType.Employee

                'Act
                Dim _passport As roPassport = roPassportManager.GetPassport(id, loadType)

                'Assert
                Assert.Equal(datalayer.ExecuteSqlWasCalled, DatalayerHelper.CreateDataTableString.LoadPassportByEmployee)

            End Using
        End Sub

        <Fact(DisplayName:="Should Use Id Parameter When Get Passport")>
        Sub ShouldUseIdParameterWhenGetPassport()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                datalayer.CreateDataTableSpy()
                Dim id As Integer = 3
                Dim loadType As LoadType = LoadType.Passport

                'Act
                Dim _passport As roPassport = roPassportManager.GetPassport(id, loadType)

                'Assert
                Assert.Equal(datalayer.ExecuteSqlWasCalled, DatalayerHelper.CreateDataTableString.LoadPassport)

            End Using
        End Sub

        <Fact(DisplayName:="Should Use ID Parameter When Get Passport and no type is specified")>
        Sub ShouldUseIdParameterWhenGetPassportAndNoTypeIsSpecified()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                datalayer.CreateDataTableSpy()
                Dim id As Integer = 3

                'Act
                Dim _passport As roPassport = roPassportManager.GetPassport(id)

                'Assert
                Assert.Equal(datalayer.ExecuteSqlWasCalled, DatalayerHelper.CreateDataTableString.LoadPassport)

            End Using
        End Sub

        <Fact(DisplayName:="Should Use IdUser Parameter When Get User Passport")>
        Sub ShouldUseIdUserParameterWhenGetUserPassport()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                datalayer.CreateDataTableSpy()
                Dim id As Integer = 3
                Dim loadType As LoadType = LoadType.User

                'Act
                Dim _passport As roPassport = roPassportManager.GetPassport(id, loadType)

                'Assert
                Assert.Equal(datalayer.ExecuteSqlWasCalled, DatalayerHelper.CreateDataTableString.LoadPassportByUser)

            End Using
        End Sub

#End Region

#Region "Load PassportsTicket"
        <Fact(DisplayName:="Should Get Nothing As User PassportTicket When SysroPassports Count Is Zero")>
        Sub ShouldGetNothingAsUserPassportTicketWhenSysroPassportsCountIsZero()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim id As Integer = 1
                Dim loadType As LoadType = LoadType.User

                'Act
                Dim _passport As roPassportTicket = roPassportManager.GetPassportTicket(id, loadType)

                'Assert
                Assert.Equal(_passport, Nothing)
            End Using
        End Sub

        <Fact(DisplayName:="Should Get User PassportTicket When SysroPassports Count NOT Is Zero")>
        Sub ShouldGetUserPassportTicketWhenSysroPassportsCountNotIsZero()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                datalayer.CreateDataTableSpy()
                Dim id As Integer = 2
                Dim loadType As LoadType = LoadType.User

                helper.PassportStub(id, datalayer)

                'Act
                Dim _passport As roPassportTicket = roPassportManager.GetPassportTicket(id, loadType)

                'Assert
                Assert.NotEqual(_passport, Nothing)

            End Using
        End Sub

        <Fact(DisplayName:="Should Use IdEmployee Parameter When Get Employee PassportTicket")>
        Sub ShouldUseIdEmployeeParameterWhenGetEmployeePassportTicket()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                datalayer.CreateDataTableSpy()
                Dim id As Integer = 3
                Dim loadType As LoadType = LoadType.Employee

                'Act
                Dim _passport As roPassportTicket = roPassportManager.GetPassportTicket(id, loadType)

                'Assert
                Assert.Equal(datalayer.ExecuteSqlWasCalled, DatalayerHelper.CreateDataTableString.LoadPassportByEmployee)

            End Using
        End Sub

        <Fact(DisplayName:="Should Use Id Parameter When Get PassportTicket")>
        Sub ShouldUseIdParameterWhenGetPassportTicket()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                datalayer.CreateDataTableSpy()
                Dim id As Integer = 3
                Dim loadType As LoadType = LoadType.Passport

                'Act
                Dim _passport As roPassportTicket = roPassportManager.GetPassportTicket(id, loadType)

                'Assert
                Assert.Equal(datalayer.ExecuteSqlWasCalled, DatalayerHelper.CreateDataTableString.LoadPassport)

            End Using
        End Sub

        <Fact(DisplayName:="Should Use ID Parameter When Get PassportTicket and no type is specified")>
        Sub ShouldUseIdParameterWhenGetPassportTicketAndNoTypeIsSpecified()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                datalayer.CreateDataTableSpy()
                Dim id As Integer = 3

                'Act
                Dim _passport As roPassportTicket = roPassportManager.GetPassportTicket(id)

                'Assert
                Assert.Equal(datalayer.ExecuteSqlWasCalled, DatalayerHelper.CreateDataTableString.LoadPassport)

            End Using
        End Sub

        <Fact(DisplayName:="Should Use IdUser Parameter When Get User PassportTicket")>
        Sub ShouldUseIdUserParameterWhenGetUserPassportTicket()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                datalayer.CreateDataTableSpy()
                Dim id As Integer = 3
                Dim loadType As LoadType = LoadType.User

                'Act
                Dim _passport As roPassportTicket = roPassportManager.GetPassportTicket(id, loadType)

                'Assert
                Assert.Equal(datalayer.ExecuteSqlWasCalled, DatalayerHelper.CreateDataTableString.LoadPassportByUser)

            End Using
        End Sub

#End Region


#Region "Passportphoto"
        <Fact(DisplayName:="Should Get PassportPhoto When passportIdExists")>
        Sub ShouldGetPassportPhotoWhenPassportIdExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                datalayer.CreateDataTableStub(New Dictionary(Of String, Dictionary(Of String, Object)()) From {
                                                  {"sysropassports", {New Dictionary(Of String, Object) From {{"IdPassport", 1}}}}
                                              })

                'Act
                Dim _passport As roPassportWithPhoto = roPassportManager.GetPassportWithPhoto(1)

                'Assert
                Assert.NotEqual(_passport, Nothing)
            End Using
        End Sub

        <Fact(DisplayName:="Should Get PassportPhoto in base64 When passportHasPhoto")>
        Sub ShouldGetPassportPhotoInBase64WhenPassportHasPhoto()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim photoArray As Byte() = {0, 1, 2, 3, 4, 5}
                datalayer.CreateDataTableStub(New Dictionary(Of String, Dictionary(Of String, Object)()) From {
                                                  {"sysropassports",
                                                    {
                                                        New Dictionary(Of String, Object) From {{"IdPassport", 1}, {"EmployeePhoto", photoArray}}
                                                    }
                                                  }
                                              })

                'Act
                Dim _passport As roPassportWithPhoto = roPassportManager.GetPassportWithPhoto(1)

                'Assert
                Assert.NotEqual(_passport, Nothing)
                Assert.True(CommonHelper.IsBase64String(_passport.EmployeePhoto))
            End Using
        End Sub

        <Fact(DisplayName:="Should Get NullObject when gets more than 1 passport")>
        Sub ShouldGetNullObjectWhenGetsMoreThan1Passport()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim photoArray As Byte() = {0, 1, 2, 3, 4, 5}
                datalayer.CreateDataTableStub(New Dictionary(Of String, Dictionary(Of String, Object)()) From {
                                                  {"sysropassports",
                                                    {
                                                        New Dictionary(Of String, Object) From {{"IdPassport", 1}, {"EmployeePhoto", photoArray}},
                                                        New Dictionary(Of String, Object) From {{"IdPassport", 2}, {"EmployeePhoto", photoArray}}
                                                    }
                                                  }
                                              })

                'Act
                Dim _passport As roPassportWithPhoto = roPassportManager.GetPassportWithPhoto(1)

                'Assert
                Assert.NotEqual(_passport, Nothing)
                Assert.Equal(_passport.IdPassport, -1)
            End Using
        End Sub

#End Region


    End Class

End Namespace