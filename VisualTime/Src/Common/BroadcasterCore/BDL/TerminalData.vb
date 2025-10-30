Imports Robotics.VTBase

Public Class TerminalData

    Public Enum eTerminalType
        unknown
        mx8
        masterasp
        mcxinbio
        rxcp
        rxcep
        rx1
        mx9
        mxv
        rxfp
        rxfptd
        rxfl
        mxs
        rxfe
        rxte
    End Enum

    Private _ID As Integer = 0
    Private _Type As eTerminalType = eTerminalType.unknown
    Private _TypeStr As String
    Private _Terminal As VTBusiness.Terminal.roTerminal
    Private _TerminalState As VTBusiness.Terminal.roTerminalState
    Private _SupportsAdvancedAccessMode As Boolean = False
    Private _PendingTasks As Integer = 0

    Public ReadOnly Property ID() As Integer
        Get
            Return _ID
        End Get
    End Property

    Public ReadOnly Property Type() As eTerminalType
        Get
            Return _Type
        End Get
    End Property

    Public ReadOnly Property TypeStr() As String
        Get
            Return _TypeStr
        End Get
    End Property

    Public ReadOnly Property SupportsAdvancedAccessMode() As Boolean
        Get
            Return _SupportsAdvancedAccessMode
        End Get
    End Property

    Public ReadOnly Property ConfData() As String
        Get
            Return _Terminal.ConfData
        End Get
    End Property

    Public ReadOnly Property RDRBehavior(ByVal Reader As Byte) As String
        Get
            Try
                Return _Terminal.ReaderByID(Reader).Mode
            Catch ex As Exception
                Return "TA"
            End Try
        End Get
    End Property

    Public ReadOnly Property RDRInteractionAction(ByVal Reader As Byte) As String
        Get
            Try
                Return _Terminal.ReaderByID(Reader).InteractionAction.Value.ToString
            Catch ex As Exception
                Return ""
            End Try
        End Get
    End Property

    Public ReadOnly Property RDRHasAccess(ByVal Reader As Byte) As Boolean
        Get
            Try
                Return _Terminal.ReaderByID(Reader).HasAccess
            Catch ex As Exception
                Return False
            End Try
        End Get
    End Property

    Public ReadOnly Property SirensOutput() As Integer
        Get
            Return _Terminal.SirensOutput
        End Get
    End Property

    Public Property PendingTasks As Integer
        Get
            Return _PendingTasks
        End Get
        Set(value As Integer)
            _PendingTasks = value
        End Set
    End Property

    Public Property Enabled As Boolean
        Get
            Return _Terminal.Enabled
        End Get
        Set(value As Boolean)
            _Terminal.Enabled = value
        End Set
    End Property

    Public Function EmployeePermit(ByVal IDEmployee As Integer, Optional ByVal IDReader As Byte = 1, Optional ByVal bAdvancedAccessMode As Boolean = False) As Boolean

        Try
            Dim iTerminalReaders As Byte = _Terminal.Readers.Count
            If IDReader <= iTerminalReaders Then
                Return _Terminal.ReaderByID(IDReader).EmployeePermit(IDEmployee, (_Type <> eTerminalType.mx8 AndAlso _Type <> eTerminalType.mx9), bAdvancedAccessMode, Nothing,)
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "EmployeePermit::Terminal " + _ID.ToString + "::Unexistent reader id (" & IDReader.ToString & ")")
                Return False
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeePermit::Terminal " + _ID.ToString + "::Unexpected error: ", ex)
        End Try
        Return True
    End Function

    Public Sub New(ByVal ID As Integer)
        Try
            _Terminal = New VTBusiness.Terminal.roTerminal(ID, _TerminalState)
            _ID = ID

            LoadTerminalData()
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalData::New: Unexpected error: ", ex)
        End Try
    End Sub

    Public Sub LoadTerminalData()
        Try

            _SupportsAdvancedAccessMode = False
            If _Type = eTerminalType.unknown Then
                _TypeStr = _Terminal.Type.ToLower
                If _TypeStr.ToLower.IndexOf("rxfl") >= 0 Then
                    _Type = eTerminalType.rxfl
                    _SupportsAdvancedAccessMode = True
                ElseIf _TypeStr.ToLower.IndexOf("rxfe") >= 0 Then
                    _Type = eTerminalType.rxfe
                    _SupportsAdvancedAccessMode = True
                ElseIf _TypeStr.ToLower.IndexOf("rxte") >= 0 Then
                    _Type = eTerminalType.rxte
                    _SupportsAdvancedAccessMode = True
                ElseIf _TypeStr.ToLower.IndexOf("rxfptd") >= 0 Then
                    _Type = eTerminalType.rxfptd
                    _SupportsAdvancedAccessMode = True
                ElseIf _TypeStr.ToLower.IndexOf("rxfp") >= 0 Then
                    _Type = eTerminalType.rxfp
                    _SupportsAdvancedAccessMode = True
                ElseIf _TypeStr.IndexOf("rxcp") >= 0 Then
                    _Type = eTerminalType.rxcp
                    _SupportsAdvancedAccessMode = True
                ElseIf _TypeStr.IndexOf("rxcep") >= 0 Then
                    _Type = eTerminalType.rxcep
                    _SupportsAdvancedAccessMode = True
                ElseIf _TypeStr.IndexOf("rx1") >= 0 Then
                    _Type = eTerminalType.rx1
                    _SupportsAdvancedAccessMode = True
                ElseIf _TypeStr.IndexOf("mx8") >= 0 Then
                    _Type = eTerminalType.mx8
                    'Modelo PLUS y LITE soportan accesos avanzados
                    If _Terminal.Model = "PLUS" OrElse _Terminal.Model = "LITE" Then
                        _SupportsAdvancedAccessMode = True
                    Else
                        _SupportsAdvancedAccessMode = False
                    End If
                ElseIf _TypeStr.IndexOf("mx9") >= 0 Then
                    _Type = eTerminalType.mx9
                    _SupportsAdvancedAccessMode = True
                ElseIf _TypeStr.IndexOf("mxc") >= 0 Then
                    _Type = eTerminalType.mcxinbio
                    _SupportsAdvancedAccessMode = True
                ElseIf _TypeStr.IndexOf("mxs") >= 0 Then
                    _Type = eTerminalType.mxs
                    _SupportsAdvancedAccessMode = True
                Else
                    _Type = eTerminalType.unknown
                    _SupportsAdvancedAccessMode = False
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalData::LoadTerminalData: Unexpected error: ", ex)
        End Try
    End Sub

    Public Overrides Function ToString() As String
        Return ":Terminal " + _ID.ToString + ":" + _TypeStr + ":"
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class