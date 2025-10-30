Imports Robotics.Base.DTOs

''' <summary>
''' Provides an object representation of a feature check query.
''' </summary>
Friend Class FeaturesCheckItem

#Region " Declarations "

    Private _featureAlias As String = ""
    Private _featureType As String = ""
    Private _requiredPermission As Permission
    Private _operator As String = Nothing
    Private _validOperators As String() = New String() {"AND", "OR"}

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets or sets the feature to check.
    ''' </summary>
    Public Property FeatureAlias() As String
        Get
            Return _featureAlias
        End Get
        Set(ByVal value As String)
            _featureAlias = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the type of the feature to check.
    ''' </summary>
    Public Property FeatureType() As String
        Get
            Return _featureType
        End Get
        Set(ByVal value As String)
            _featureType = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the required permission over feature.
    ''' </summary>
    Public Property RequiredPermission() As Permission
        Get
            Return _requiredPermission
        End Get
        Set(ByVal value As Permission)
            _requiredPermission = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets an AND or OR operator to apply against previous query.
    ''' </summary>
    Public Property [Operator]() As String
        Get
            Return _operator
        End Get
        Set(ByVal value As String)
            If value Is Nothing Then
                _operator = value
            Else
                ' Only allow valid operator values: 'AND' or 'OR'
                value = value.ToUpper()
                If Array.IndexOf(_validOperators, value) > -1 Then
                    _operator = value
                Else
                    Throw New FeaturesFormatException(_featureAlias)
                End If
            End If
        End Set
    End Property

#End Region

End Class