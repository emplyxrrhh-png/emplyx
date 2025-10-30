Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

Public Class roWsStateManager

    Public Shared Sub CopyTo(ByVal oSrcState As roWsState, ByVal oDstState As roBaseState)

        With oDstState
            .IDPassport = oSrcState.IDPassport
            .ClientAddress = oSrcState.ClientAddress
            .SessionID = oSrcState.SessionID
            Try
                If oDstState.GetType().GetProperty("Result") IsNot Nothing Then
                    CType(oDstState, Object).Result = CType(oSrcState, Object).Result
                End If
            Catch
            End Try
            .ErrorText = oSrcState.ErrorText
            .ErrorDetail = oSrcState.ErrorDetail
            .ErrorNumber = oSrcState.ErrorNumber
            .ReturnCode = oSrcState.ReturnCode
            .AppType = oSrcState.AppType
            .AppSource = oSrcState.AppSource
        End With

    End Sub

    Public Shared Sub CopyTo(ByVal oSrcState As roBaseState, ByVal oDstState As roWsState)

        With oDstState
            .IDPassport = oSrcState.IDPassport
            .ClientAddress = oSrcState.ClientAddress
            .SessionID = oSrcState.SessionID
            Try
                If oSrcState.GetType().GetProperty("Result") IsNot Nothing Then
                    CType(oDstState, Object).Result = CType(oSrcState, Object).Result
                End If
            Catch
            End Try
            .ErrorText = oSrcState.ErrorText
            .ErrorDetail = oSrcState.ErrorDetail
            .ErrorNumber = oSrcState.ErrorNumber
            .ReturnCode = oSrcState.ReturnCode
            .AppSource = oSrcState.AppSource
            .AppType = oSrcState.AppType
        End With

    End Sub

    Public Shared Sub CopyTo(ByVal oSrcState As roWsState, ByVal oDstState As roBusinessState)

        With oDstState
            .IDPassport = oSrcState.IDPassport
            .ClientAddress = oSrcState.ClientAddress
            .SessionID = oSrcState.SessionID
            Try
                If roTypes.Any2String(oSrcState.Result) <> String.Empty Then
                    CType(oDstState, Object).Result = CType(oSrcState, Object).Result
                End If
            Catch
            End Try
            .ErrorText = oSrcState.ErrorText
            .ErrorDetail = oSrcState.ErrorDetail
            .ErrorNumber = oSrcState.ErrorNumber
            .ReturnCode = oSrcState.ReturnCode
            .AppSource = oSrcState.AppSource
            .AppType = oSrcState.AppType
        End With

    End Sub

    Public Shared Sub CopyTo(ByVal oSrcState As roBusinessState, ByVal oDstState As roWsState)

        With oDstState
            .IDPassport = oSrcState.IDPassport
            .ClientAddress = oSrcState.ClientAddress
            .SessionID = oSrcState.SessionID
            Try
                CType(oDstState, Object).Result = CType(oSrcState, Object).Result
            Catch
            End Try
            .ErrorText = oSrcState.ErrorText
            .ErrorDetail = oSrcState.ErrorDetail
            .ErrorNumber = oSrcState.ErrorNumber
            .ReturnCode = oSrcState.ReturnCode
            .AppSource = oSrcState.AppSource
            .AppType = oSrcState.AppType
        End With

    End Sub

    Public Shared Sub CopyTo(ByVal oSrcState As roWsTerminalState, ByVal oDstState As roBusinessState)

        With oDstState
            .IDPassport = -1
            .ClientAddress = oSrcState.ClientAddress
            .SessionID = oSrcState.SessionID
            CType(oDstState, Object).Result = CInt(oSrcState.Result)
            .ErrorText = oSrcState.ErrorText
            .ErrorDetail = oSrcState.ErrorDetail
            .ErrorNumber = oSrcState.ErrorNumber
            .ReturnCode = oSrcState.ReturnCode
            .AppSource = oSrcState.AppSource
            .AppType = oSrcState.AppType
        End With

    End Sub

    Public Shared Sub CopyTo(ByVal oSrcState As roBusinessState, ByVal oDstState As roWsTerminalState, ByVal IDTerminal As Integer)

        With oDstState
            .IDTerminal = IDTerminal
            .ClientAddress = oSrcState.ClientAddress
            .SessionID = oSrcState.SessionID
            .Result = roTypes.Any2Integer(CType(oSrcState, Object).Result)
            .ErrorText = oSrcState.ErrorText
            .ErrorDetail = oSrcState.ErrorDetail
            .ErrorNumber = oSrcState.ErrorNumber
            .ReturnCode = oSrcState.ReturnCode
            .AppSource = oSrcState.AppSource
            .AppType = oSrcState.AppType
        End With

    End Sub

End Class