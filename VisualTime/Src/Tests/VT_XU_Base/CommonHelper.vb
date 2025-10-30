Imports System.ServiceModel.Channels
Imports System.Text.RegularExpressions
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Class CommonHelper
    Public Property Action As String

    Public Shared Function IsBase64String(s As String) As Boolean
        s = s.Trim()
        Return (s.Length Mod 4 = 0) AndAlso Regex.IsMatch(s, "^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None)
    End Function


    Public Shared Sub DecryptMessage()
        Robotics.VTBase.Fakes.ShimroJSONHelper.DeserializeNewtonSoftStringType = Function(s1 As String, oType As System.Type)
                                                                                     Return New roMessage()
                                                                                 End Function
    End Sub

    Public Shared Sub InitTask()
        Robotics.VTBase.Extensions.Fakes.ShimroConnector.InitTaskTasksTyperoCollection = Function(a, b) String.Empty
    End Sub


    Public Shared Sub InitLicense(Optional ByVal availablelicenses As List(Of String) = Nothing)

        Robotics.VTBase.Extensions.Fakes.ShimroServerLicense.Constructor = Function() New Robotics.VTBase.Extensions.Fakes.ShimroServerLicense
        Robotics.VTBase.Extensions.Fakes.ShimroServerLicense.AllInstances.FeatureDataStringStringroLog = Function() "licenseKey"

        Robotics.VTBase.Extensions.Fakes.ShimroServerLicense.AllInstances.FeatureIsInstalledString = Function(serverLicense, feature)
                                                                                                         If availablelicenses IsNot Nothing AndAlso availablelicenses.Contains(feature) Then
                                                                                                             Return True
                                                                                                         Else
                                                                                                             Return False
                                                                                                         End If
                                                                                                     End Function
    End Sub

    Public Sub CryptographyHelper(action As String, oldkey As String, newkey As String)

        sAction = action

        Robotics.VTBase.Extensions.Fakes.ShimroServerLicense.Constructor = Function() New Robotics.VTBase.Extensions.Fakes.ShimroServerLicense
        Robotics.VTBase.Extensions.Fakes.ShimroServerLicense.AllInstances.FeatureDataStringStringroLog = Function()
                                                                                                             If sAction = "encrypt" Then
                                                                                                                 Return oldkey
                                                                                                             Else
                                                                                                                 Return newkey
                                                                                                             End If
                                                                                                         End Function

    End Sub

    Public Shared Sub InitHAEnvironmentFakes()
        Robotics.VTBase.Fakes.ShimroLog.AllInstances.logMessageroLogEventTypeStringDictionaryOfStringString = Function() True

        Robotics.VTBase.Fakes.ShimroBaseState.GetClientAddressHttpRequest = Function()
                                                                                Return ":::1"
                                                                            End Function

    End Sub

End Class
