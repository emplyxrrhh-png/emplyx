Imports Robotics.Base.DTOs

''' <summary>
''' Represents a License check query.
''' </summary>
''' <remarks>The functionalities query is in this format :
''' Type[(!)LicenseEntry [;(!)LicenceEntry]]
''' For example: "Feature\ConcertRules;!Version\Live" Feature Concert rules is requiered and Version\Live must not exists
''' </remarks>
Public Class LicenseCheck

#Region " Declarations / Constructor "

    Private _licenceEntries As String()
    Private _IdPassport As Integer
    Private _oServerLicense As New VTBase.Extensions.roServerLicense

    ''' <summary>
    ''' Initializes a new instance of the PermissionCheck class.
    ''' </summary>
    ''' <param name="licenseQuery">The License check query.</param>
    Public Sub New(ByVal licenseQuery As String, ByVal IdPassport As Integer)
        _licenceEntries = licenseQuery.Trim().Split(";")
        _IdPassport = IdPassport
    End Sub

#End Region

#Region " IsAuthorized "

    ''' <summary>
    ''' Verify permissions as specified by the query and returns wether
    ''' specified user has access.
    ''' </summary>
    Public Function IsAuthorized() As Boolean
        Dim ItemResult As Boolean
        Dim Result As Boolean = True

        ' Otherwise, make security check from parsed query.
        For Each Item As String In _licenceEntries
            ItemResult = IsItemAuthorized(Item)

            If Item.Contains("|") Then
                Result = (Result OrElse ItemResult)
            Else
                Result = (Result AndAlso ItemResult)
            End If
        Next

        Return Result
    End Function

    ''' <summary>
    ''' Returns wether specified user permissions match specified functionalities.
    ''' </summary>
    ''' <param name="licenseQuery">A query representing license functionalities.</param>
    Public Shared Function IsAuthorized(ByVal licenseQuery As String, ByVal IdPassport As Integer) As Boolean
        Dim Checker As New LicenseCheck(licenseQuery, IdPassport)
        Return Checker.IsAuthorized()
    End Function

    Public Shared Function IsInstalled(ByVal item As String) As Boolean
        Dim Checker As New LicenseCheck("", -1)
        Return Checker.FeatureIsInstalled(item)
    End Function

    ''' <summary>
    ''' Returns wether user is granted access to the specified feature.
    ''' </summary>
    ''' <param name="item">The feature and permission to check.</param>
    Private Function IsItemAuthorized(ByVal item As String) As Boolean
        Dim expectedResult As Boolean = True
        Dim checkItem As String = item

        If item.StartsWith("|") Then
            checkItem = item.Substring(1)
        End If

        If item.StartsWith("!") Then
            expectedResult = False
            checkItem = item.Substring(1)
        End If

        Dim Result As Permission = _oServerLicense.FeatureIsInstalled(checkItem)

        Return (Result = expectedResult)

    End Function

    Private Function FeatureIsInstalled(ByVal item As String) As Boolean
        Return _oServerLicense.FeatureIsInstalled(item)
    End Function

#End Region

End Class