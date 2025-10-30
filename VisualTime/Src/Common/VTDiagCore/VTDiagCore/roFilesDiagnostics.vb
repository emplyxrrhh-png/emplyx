Namespace Robotics.Diagnostics

    Public Class roFilesDiagnostics

        Public Shared Function getFilesSize(ByVal gSystemPath As String, ByVal Name As String, ByRef oState As VTDiagCore.wscDiagnosticsState)
            Try
                Dim path As String = IO.Path.Combine(gSystemPath, Name + "_" + Now.ToString("yyyyMMdd") + ".DAT")
                If IO.File.Exists(path) Then
                    Dim fil As New IO.FileInfo(path)
                    Return fil.Length / 1024 / 1024
                Else
                    Return 0
                End If
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.FileAccessError
                Return 0
            End Try
        End Function

        Public Shared Function canCreateFile(ByVal gSystemPath As String, ByVal Name As String, ByRef oState As VTDiagCore.wscDiagnosticsState)
            Try
                Dim path As String = IO.Path.Combine(gSystemPath, Name + "_" + Now.ToString("yyyyMMddHHmmss") + ".DAT")

                Dim sFile As IO.FileStream = IO.File.Open(path, IO.FileMode.Create, IO.FileAccess.Write)

                If sFile.CanWrite Then
                    sFile.Close()
                    Return True
                Else
                    oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.FileAccessError
                    Return False
                End If
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.FileAccessError
                Return False
            End Try
        End Function

    End Class

End Namespace