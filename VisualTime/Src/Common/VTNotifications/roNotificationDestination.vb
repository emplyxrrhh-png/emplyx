Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.VTBase

Namespace Notifications

    <DataContract()>
    Public Class roNotificationDestination

#Region "Declarations - Constructor"

        Private oState As roNotificationState

        Private strDestination As String

        Public Sub New()
            Me.oState = New roNotificationState
        End Sub

        Public Sub New(ByVal _State As roNotificationState)
            Me.oState = _State
        End Sub

        Public Sub New(ByVal _State As roNotificationState, ByVal strXml As String)
            Me.oState = _State
            Me.LoadFromXml(strXml)

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roNotificationState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roNotificationState)
                Me.oState = value
            End Set
        End Property

        <DataMember()>
        Public Property Destination() As String
            Get
                Return Me.strDestination
            End Get
            Set(ByVal value As String)
                Me.strDestination = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function GetXml() As String

            Dim oDestination As New roCollection

            Dim strIDCauses As String = ""
            oDestination.Add("Destination", strDestination)
            Return oDestination.XML

        End Function

        Private Sub LoadFromXml(ByVal strXml As String)

            If strXml <> "" Then
                ' Añadimos la composición a la colección
                Dim oDestination As New roCollection(strXml)
                If oDestination.Exists("Destination") Then strDestination = oDestination.Item("Destination")
            End If

        End Sub

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                'TODO: Validacións pendents
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotificationDestination::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotificationDestination::Validate")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace