Imports System.IO

Public Class CryptographyHelper

    Public Property MessageDecrypted As Boolean = False

    Public Function DecryptExSpy(s1 As String, s2 As String)

        Robotics.VTBase.Fakes.ShimCryptographyHelper.DecryptExStringString =
                                             Function(str1 As String, str2 As String)
                                                 MessageDecrypted = True
                                                 Return "decrypt"
                                             End Function
    End Function

    Public Function DecryptExStub(s1 As String, s2 As String)

        Robotics.VTBase.Fakes.ShimCryptographyHelper.DecryptExStringString =
                                             Function(str1 As String, str2 As String)
                                                 Return "decrypt"
                                             End Function
    End Function

End Class