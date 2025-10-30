Imports Robotics.VTBase

Public Class BroadcasterHelper

#Region "Private Fields"

    Private _oSettings As roSettings

#End Region

    Public Property OSettings As roSettings
        Get
            Return _oSettings
        End Get
        Set(value As roSettings)
            _oSettings = value
        End Set
    End Property

    Sub New()
        _oSettings = New roSettings()
    End Sub

End Class