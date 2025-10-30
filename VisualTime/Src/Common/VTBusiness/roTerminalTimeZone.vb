Imports System.IO
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Namespace Terminal

    <DataContract>
    Public Class roTerminalTimeZone
        Implements IComparer(Of roTerminalTimeZone)

#Region "Declarations - Constructors"

        Private strName As String
        Private strDisplayName As String
        Private bolSupportsDaylightSavingTime As Boolean

        Private bolIsValid As Boolean

        Public Sub New()
            Me.strName = ""
            Me.strDisplayName = ""
            Me.bolSupportsDaylightSavingTime = False
            Me.bolIsValid = False
        End Sub

        Public Sub New(ByVal _TimeZoneId As String)

            Me.strName = _TimeZoneId
            Me.bolIsValid = False
            Me.Load()

        End Sub

        Public Sub New(ByVal _TimeZoneInfo As TimeZoneInfo)
            Me.strName = _TimeZoneInfo.Id
            Me.strDisplayName = _TimeZoneInfo.DisplayName
            Me.bolSupportsDaylightSavingTime = _TimeZoneInfo.SupportsDaylightSavingTime
            Me.bolIsValid = True
        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property

        <DataMember>
        Public Property DisplayName() As String
            Get
                Return Me.strDisplayName
            End Get
            Set(ByVal value As String)
                Me.strDisplayName = value
            End Set
        End Property

        <DataMember>
        Public Property SupportsDaylightSavingTime() As Boolean
            Get
                Return Me.bolSupportsDaylightSavingTime
            End Get
            Set(ByVal value As Boolean)
                Me.bolSupportsDaylightSavingTime = value
            End Set
        End Property

        <DataMember>
        Public Property IsValid() As Boolean
            Get
                Return Me.bolIsValid
            End Get
            Set(ByVal value As Boolean)
                Me.bolIsValid = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub Load()

            Try

                Dim oTimeZoneInfo As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(Me.strName)

                Me.strDisplayName = oTimeZoneInfo.DisplayName
                Me.bolSupportsDaylightSavingTime = oTimeZoneInfo.SupportsDaylightSavingTime()

                Me.bolIsValid = True
            Catch ex As TimeZoneNotFoundException
                Me.bolIsValid = False
            Catch ex As Exception
                Me.bolIsValid = False
            End Try

        End Sub

        Public Function Compare(ByVal x As roTerminalTimeZone, ByVal y As roTerminalTimeZone) As Integer Implements System.Collections.Generic.IComparer(Of roTerminalTimeZone).Compare

            Return String.Compare(x.DisplayName, y.DisplayName)

        End Function

#Region "Helper methods"

        ''' <summary>
        ''' Obtiene una lista con todas la zonas horarias configurables a los terminales.
        ''' </summary>
        ''' <param name="_State"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTerminalTimeZones(ByVal _TerminalType As String, ByVal _State As roTerminalState) As Generic.List(Of roTerminalTimeZone)

            Dim oRet As New Generic.List(Of roTerminalTimeZone)

            Try

                Dim oTerminalTimeZone As roTerminalTimeZone = Nothing

                Dim oSettings As New roSettings
                Dim strTimeZonesFileName As String = Path.Combine(oSettings.GetVTSetting(eKeys.PathProcesses), "TimeZones_" & _TerminalType & ".reg")

                If File.Exists(strTimeZonesFileName) Then ' Si existe el fichero con la definición de zonas horarias del terminal

                    Dim stmReader As New IO.StreamReader(strTimeZonesFileName)
                    Dim strLine As String = stmReader.ReadLine

                    Dim strName As String = ""
                    Dim strDisplayName As String = ""
                    Dim bolSupportsDaylightSavingTime As Boolean = False

                    While Not stmReader.EndOfStream
                        If strLine.StartsWith("[") And strLine.EndsWith("]") Then

                            Dim NameParts() As String = strLine.Substring(1, strLine.Length - 2).Split("\")
                            strName = NameParts(NameParts.Length - 1)

                        ElseIf strLine.StartsWith("""Display""=") Then
                            strDisplayName = strLine.Split("=")(1).Replace("""", "")
                        End If

                        If strName <> "" And strDisplayName <> "" Then

                            oTerminalTimeZone = New roTerminalTimeZone()
                            oTerminalTimeZone.Name = strName
                            oTerminalTimeZone.DisplayName = strDisplayName
                            oTerminalTimeZone.SupportsDaylightSavingTime = True
                            oRet.Add(oTerminalTimeZone)

                            strName = ""
                            strDisplayName = ""

                        End If

                        strLine = stmReader.ReadLine

                    End While
                Else ' Si no existe el fichero de zonas horarias, cargamos las zonas del sistema

                    For Each oTimeZoneInfo As TimeZoneInfo In TimeZoneInfo.GetSystemTimeZones
                        oTerminalTimeZone = New roTerminalTimeZone(oTimeZoneInfo)
                        oRet.Add(oTerminalTimeZone)
                    Next

                End If

                Dim oComparer As New roTerminalTimeZone
                oRet.Sort(oComparer)
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTerminalTimeZone:GetTerminalTimeZones")
            End Try

            Return oRet

        End Function

#End Region

#End Region

    End Class

End Namespace