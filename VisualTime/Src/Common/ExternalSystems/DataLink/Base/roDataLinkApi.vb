Imports System.Drawing
Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.Security.Base

Namespace DataLink

    Public Class roDataLinkApi

        Private oState As roDataLinkState

        Protected oDataImport As roDataLinkImport = Nothing


        Public ReadOnly Property State As roDataLinkState
            Get
                Return oState
            End Get
        End Property


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            Me.oState = IIf(state Is Nothing, New roDataLinkState(-1), state)
        End Sub


    End Class

End Namespace