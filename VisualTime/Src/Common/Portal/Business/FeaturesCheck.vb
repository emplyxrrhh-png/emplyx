Imports Robotics.Base.DTOs
Imports Robotics.Security
Imports Robotics.Security.Base

''' <summary>
''' Represents a functionalities check query.
''' </summary>
''' <remarks>The functionalities query is in this format :
''' Type[:App.Feature=Permission [AND|OR Type:App.Feature=Permission]]
''' For example: "U:Tasks.Manage=Write AND U:Tasks.Reports=Read"
''' AND and OR operators are always checked against preceding item.
''' You can also specify U and E values alone, to verify if user
''' is a user or an employee.</remarks>
Public Class FeaturesCheck

#Region " Declarations / Constructor "

    Private _items As New FeaturesCheckItemCollection
    Private _features As String
    Private _featureType As String
    Private _IdPassport As Integer

    ''' <summary>
    ''' Initializes a new instance of the PermissionCheck class.
    ''' </summary>
    ''' <param name="functionalities">The functionalities check query.</param>
    ''' <param name="featureType">'U' to load elements of users, 'E' for employees.</param>
    Public Sub New(ByVal functionalities As String, ByVal featureType As String, ByVal idPassport As Integer)
        _features = functionalities.Trim()
        _featureType = featureType
        _IdPassport = idPassport
        Parse()
    End Sub

#End Region

#Region " Parse "

    ''' <summary>
    ''' Parses the functionalities check query.
    ''' </summary>
    Private Sub Parse()
        ' Ensure query is specified.
        If _features Is Nothing OrElse _features.Trim().Length = 0 Then
            ThrowFormatException()
        ElseIf _features.ToUpper = "U" OrElse _features.ToUpper = "E" Then
            ' If U or E is specified, skip parsing.
            _features = _features.ToUpper()
            Return
        End If

        ' Start at the beginning.
        Parse(0)
    End Sub

    ''' <summary>
    ''' Parses the next functionalities check query, starting at specified position.
    ''' </summary>
    ''' <param name="pos">The position at which to start parsing.</param>
    ''' <remarks>This function is recursively called until all the query is parsed.</remarks>
    Private Sub Parse(ByVal pos As Integer)
        Dim SpacePos As Integer
        Dim DotsPos As Integer
        Dim CurrentPos As Integer = pos
        Dim NewItem As New FeaturesCheckItem()
        Dim PermissionString As String

        ' If it is not the first feature specified, read the operator.
        If _items.Count > 0 Then
            ' Look for " " between operator and feature.
            SpacePos = FindNextSpace(CurrentPos, True)
            ' Read operator.
            NewItem.Operator = _features.Substring(CurrentPos, SpacePos - CurrentPos)
            CurrentPos = SpacePos + 1
        End If

        ' Look for ":" between type and feature.
        DotsPos = FindNextDots(":"c, CurrentPos)
        If DotsPos - CurrentPos <> 1 Then Throw New FormatException()
        ' Read type.
        NewItem.FeatureType = _features.Substring(CurrentPos, DotsPos - CurrentPos)
        CurrentPos = DotsPos + 1

        ' Look for "=" between feature and permission.
        DotsPos = FindNextDots("="c, CurrentPos)
        ' Read feature name.
        NewItem.FeatureAlias = _features.Substring(CurrentPos, DotsPos - CurrentPos)
        CurrentPos = DotsPos + 1

        ' Look for " " between permission and operator, if any.
        SpacePos = FindNextSpace(CurrentPos, False)
        ' Read required permission.
        If SpacePos > -1 Then
            PermissionString = _features.Substring(CurrentPos, SpacePos - CurrentPos)
        Else
            PermissionString = _features.Substring(CurrentPos)
        End If
        NewItem.RequiredPermission = CType([Enum].Parse(GetType(Permission), PermissionString), Permission)

        ' Add item to collection.
        _items.Add(NewItem)

        ' Parse next feature.
        If SpacePos > -1 Then
            CurrentPos = SpacePos + 1
            Parse(CurrentPos)
        End If
    End Sub

    ''' <summary>
    ''' Returns the position of next space in _features,
    ''' ensuring there is no unexpected space before.
    ''' </summary>
    ''' <param name="currentPos">The position at which to start searching.</param>
    ''' <param name="required">Wether to throw an exception if no space is found.</param>
    Private Function FindNextSpace(ByVal currentPos As Integer, ByVal required As Boolean) As Integer
        Dim SpacePos As Integer = _features.IndexOf(" ", currentPos)
        Dim DotsPos As Integer = _features.IndexOf(":", currentPos)

        ' Make sure there is no unexpected dots before space.
        If DotsPos > -1 AndAlso DotsPos < SpacePos Then
            ThrowFormatException()
        End If

        ' If there should be a match but there isn't, throw an exception.
        If required AndAlso SpacePos < 0 Then
            ThrowFormatException()
        End If

        Return SpacePos
    End Function

    ''' <summary>
    ''' Returns the position of next occurence in _features.
    ''' An exception is thrown if a space is found before.
    ''' </summary>
    ''' <param name="searchedChar">The char to look for.</param>
    ''' <param name="currentPos">The position at which to start searching.</param>
    Private Function FindNextDots(ByVal searchedChar As Char, ByVal currentPos As Integer) As Integer
        Dim SpacePos As Integer = _features.IndexOf(" ", currentPos)
        Dim DotsPos As Integer = _features.IndexOf(searchedChar, currentPos)

        ' Make sure there is no unexpected space before dots.
        If SpacePos > -1 AndAlso SpacePos < DotsPos Then
            ThrowFormatException()
        End If

        ' If there should be a match but there isn't, throw an exception.
        If DotsPos < 0 Then
            ThrowFormatException()
        End If

        Return DotsPos
    End Function

    ''' <summary>
    ''' Throws an exception when the functionalities query is not correctly formed.
    ''' </summary>
    Private Sub ThrowFormatException()
        Throw New FeaturesFormatException(_features)
    End Sub

#End Region

#Region " IsAuthorized "

    ''' <summary>
    ''' Verify permissions as specified by the query and returns wether
    ''' specified user has access.
    ''' </summary>
    Public Function IsAuthorized() As Boolean
        Dim ItemResult As Boolean
        Dim Result As Boolean = False

        Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(Me._IdPassport)

        ' If U or E are specified, check wether current passport is a user or employee.
        If _features = "U" Then
            Result = (_featureType = "U" AndAlso oPassport.IDUser.HasValue)
        ElseIf _features = "E" Then
            Result = (_featureType = "E" AndAlso oPassport.IDEmployee.HasValue)
        Else
            ' Otherwise, make security check from parsed query.
            For Each Item As FeaturesCheckItem In _items
                ItemResult = IsItemAuthorized(Item)
                If Item.Operator Is Nothing Then
                    Result = ItemResult
                ElseIf Item.Operator = "OR" Then
                    Result = (Result OrElse ItemResult)
                ElseIf Item.Operator = "AND" Then
                    Result = (Result AndAlso ItemResult)
                End If
            Next
        End If

        Return Result
    End Function

    ''' <summary>
    ''' Returns wether specified user permissions match specified functionalities.
    ''' </summary>
    ''' <param name="functionalities">A query representing required functionalities.</param>
    ''' <param name="featureType">'U' to load elements of users, 'E' for employees.</param>
    Public Shared Function IsAuthorized(ByVal functionalities As String, ByVal featureType As String, ByVal idPassport As Integer) As Boolean
        Dim Checker As New FeaturesCheck(functionalities, featureType, idPassport)
        Return Checker.IsAuthorized()
    End Function

    ''' <summary>
    ''' Returns wether user is granted access to the specified feature.
    ''' </summary>
    ''' <param name="item">The feature and permission to check.</param>
    Private Function IsItemAuthorized(ByVal item As FeaturesCheckItem) As Boolean
        If item.FeatureType <> _featureType Then
            Return False
        Else
            Dim Result As Permission = WLHelper.GetPermissionOverFeature(_IdPassport, item.FeatureAlias, item.FeatureType)
            Return (Result >= item.RequiredPermission)
        End If
    End Function

#End Region

End Class