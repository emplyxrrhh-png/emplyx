Imports System.Net
Imports System.Text
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports Robotics.Base.DTOs
Imports System.Data.Common

Namespace PlanningEngine

    Public Class roPlanningEngineManager

        Private _SolverURI As Uri
        Private _SolverUserName As String
        Private _SolverUserPwd As String

        Private oState As roPlanningEngineState = Nothing

        Public ReadOnly Property State
            Get
                Return oState
            End Get
        End Property

        Public Sub New(ByVal _State As roPlanningEngineState)
            Me.oState = _State
        End Sub

        Public Property SolverURI As String
            Get
                Return _SolverURI.OriginalString
            End Get
            Set(value As String)
                _SolverURI = New Uri(value)
            End Set
        End Property

        Public Function SendRequest(oRequest As roPlanningEngineRequest) As roPlanningEngineResponse
            Dim retEngineResponse As roPlanningEngineResponse = Nothing
            Try
                Dim sJsonRequest As String = VTBase.roJSONHelper.SerializeNewtonSoft(oRequest)
                retEngineResponse = SendRequest(sJsonRequest).Result
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPlanningEngineManager::SendRequest")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPlanningEngineManager::SendRequest")
            End Try

            Return retEngineResponse
        End Function

        Public Async Function SendRequest(sJsonRequest As String) As Task(Of roPlanningEngineResponse)
            Dim retEngineResponse As roPlanningEngineResponse = Nothing
            Try
                oState.Result = roPlanningEngineState.ResultEnum.NoError

                Dim oEngineClient As HttpClient = New HttpClient()
                oEngineClient.BaseAddress = _SolverURI
                oEngineClient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

                Dim Buffer As Byte() = Encoding.UTF8.GetBytes(sJsonRequest)
                Dim byteContent As New ByteArrayContent(Buffer)
                byteContent.Headers.ContentType = New MediaTypeWithQualityHeaderValue("application/json")

                Dim task As Task(Of HttpResponseMessage) = oEngineClient.PostAsync(oEngineClient.BaseAddress, byteContent)

                Dim sResult As String = Await task.Result.Content.ReadAsStringAsync()

                Dim oErrorResponse As New ErrorResponse

                Dim bIsOk As Boolean = False
                retEngineResponse = VTBase.roJSONHelper.DeserializeNewtonSoft(sResult, GetType(roPlanningEngineResponse))
                bIsOk = (retEngineResponse.scenarioResponse IsNot Nothing)
                If Not bIsOk Then
                    ' Probablemente ha devuelto un error
                    oErrorResponse = VTBase.roJSONHelper.DeserializeNewtonSoft(sResult, GetType(ErrorResponse))
                    oState.Result = roPlanningEngineState.ResultEnum.EngineError
                    oState.ResultDetail = "Status = " & oErrorResponse.status & " Error = " & oErrorResponse.status & " Message = " & oErrorResponse.message
                End If

                oEngineClient.Dispose()

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPlanningEngineManager::SendRequest")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPlanningEngineManager::SendRequest")
            End Try

            Return retEngineResponse
        End Function

        Public Async Function CheckRequest(idRequest As Integer) As Task(Of roPlanningEngineResponse)
            Dim retEngineResponse As roPlanningEngineResponse = Nothing
            Try
                Dim bIsOk As Boolean = False
                Dim oEngineClient As HttpClient = New HttpClient()
                oEngineClient.BaseAddress = New Uri(SolverURI & "\" & idRequest)
                oEngineClient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
                Dim response As HttpResponseMessage = oEngineClient.GetAsync("").Result
                Dim strResponse As String
                bIsOk = (response.IsSuccessStatusCode)

                If bIsOk Then
                    strResponse = Await response.Content.ReadAsStringAsync()
                    retEngineResponse = VTBase.roJSONHelper.DeserializeNewtonSoft(strResponse, GetType(roPlanningEngineResponse))
                    bIsOk = (retEngineResponse.scenarioResponse IsNot Nothing)
                    Dim oErrorResponse As New ErrorResponse
                    If Not bIsOk Then
                        ' Probablemente ha devuelto un error
                        oErrorResponse = VTBase.roJSONHelper.DeserializeNewtonSoft(strResponse, GetType(ErrorResponse))
                        oState.Result = roPlanningEngineState.ResultEnum.EngineError
                        oState.ResultDetail = "Status = " & oErrorResponse.status & " Error = " & oErrorResponse.status & " Message = " & oErrorResponse.message
                    End If
                End If

                oEngineClient.Dispose()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPlanningEngineManager::CheckRequest")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPlanningEngineManager::CheckRequest")
            End Try

            Return retEngineResponse
        End Function

    End Class

End Namespace

