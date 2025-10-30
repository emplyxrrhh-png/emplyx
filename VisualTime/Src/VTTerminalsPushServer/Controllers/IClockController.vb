Imports System.Threading
Imports Robotics
Imports Robotics.Base.VTTerminals
Imports Robotics.VTBase

Namespace Controllers

    Public Class iclockController
        Inherits Controller

        Private Shared oLock As New Object()
        Private Shared dTerminalLocks As New Dictionary(Of String, Object)

        Private Function GetLockObject(sSerial As String) As Object
            SyncLock oLock
                If Not dTerminalLocks.ContainsKey(sSerial) Then
                    dTerminalLocks.Add(sSerial, New Object)
                End If
            End SyncLock

            Return dTerminalLocks(sSerial)
        End Function

#Region "Terminales PUSH y CENTRALITA"

        ' GET: iclock/cdata
        Sub Cdata() 'As ActionResult

            If Not roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Items("SkipRequest")) Then

                Dim oState = New roTerminalsState(-1)
                Dim strResponse As String = String.Empty
                Dim oHelper As New roPushServerHelper

                Dim SN As String = VTBase.roTypes.Any2String(ControllerContext.RequestContext.HttpContext.Request.Params("SN"))
                Dim strIncomingMessage As String = oHelper.GetPushRawMessageFromHttpRequest(ControllerContext.RequestContext.HttpContext.Request)

                Dim oIncomingMessage As Object = Nothing
                Dim oResponseMessage As Object = Nothing
                Dim oPushTerminalController As Object
                Dim strResponseForSecureConnect As String = String.Empty
                Dim bCommesFormSecureConnect As Boolean = False
                roTrace.GetInstance.AddTraceEvent($"Incoming message:{vbCrLf & strIncomingMessage & vbCrLf}")

                If WebApiApplication.LoggedIn AndAlso WebApiApplication.TerminalLogicIdentity IsNot Nothing Then

                    Dim bLockTaken As Boolean = False

                    Try
                        ' Verificar si la solicitud contiene el encabezado "VTSecureConnect"
                        bCommesFormSecureConnect = roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Request.Headers("VTSecureConnect"))

                        Monitor.TryEnter(GetLockObject(SN), New TimeSpan(0, 0, 5), bLockTaken)
                        If Not bLockTaken Then
                            LogSkippedRequest()
                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:GET iclock/cdata: Lock on terminal logic:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString)

                            ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                            ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                            ControllerContext.RequestContext.HttpContext.Response.End()

                            oIncomingMessage = Nothing
                            oPushTerminalController = Nothing
                            oResponseMessage = Nothing
                            GC.Collect()
                            Return
                        End If

                        Select Case WebApiApplication.TerminalLogicIdentity.mTerminal.Model.ToUpper
                            Case roTerminalApiSession.roTerminalModel.mxS.ToString.ToUpper
                                oIncomingMessage = New Comms.DrivermxS.BusinesProtocol.MessageMxS(Encoding.Default.GetBytes(strIncomingMessage))
                                oPushTerminalController = New roInBioTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                            Case Else
                                oIncomingMessage = New Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2(Encoding.Default.GetBytes(strIncomingMessage))
                                oPushTerminalController = New roPushTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                        End Select

                        oResponseMessage = oPushTerminalController.ProcessMessage(oIncomingMessage)
                    Catch ex As Exception
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalsPushServer:IClockController:GET iclock/cdata:Exception processing message:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString, ex)
                    Finally
                        If bLockTaken Then Monitor.Exit(GetLockObject(SN))
                    End Try

                    If oResponseMessage IsNot Nothing AndAlso oState.Result = roTerminalsState.ResultEnum.NoError Then

                        Select Case WebApiApplication.TerminalLogicIdentity.mTerminal.Model.ToUpper
                            Case roTerminalApiSession.roTerminalModel.mxS.ToString.ToUpper
                                Select Case CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).Command
                                    Case Comms.DrivermxS.BusinesProtocol.MessageMxS.msgCommand.init
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_initresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_initresponse.HttpContent
                                    Case Comms.DrivermxS.BusinesProtocol.MessageMxS.msgCommand.command
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_table.toString()
                                        strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_table.HttpContent
                                    Case Else
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.HttpContent
                                End Select
                            Case Else
                                Select Case CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).Command
                                    Case Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2.msgCommand.init
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_initresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_initresponse.HttpContent
                                    Case Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2.msgCommand.command
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_table.toString()
                                        strResponse = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_table.HttpContent
                                    Case Else
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_genericresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_genericresponse.HttpContent
                                End Select
                        End Select

                        roTrace.GetInstance.AddTraceEvent($"Response message:{vbCrLf & strResponse & vbCrLf}")
                    ElseIf oState.Result <> roTerminalsState.ResultEnum.NoError Then
                        ' Respondo sin dar OK para que el terminal vuelva a enviar el mensaje (en driver no respondía, pero aquí no hay opción de no responder
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:GET iclock/cdata:Error processing message:" & oState.Result.ToString & ":" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    ElseIf oResponseMessage Is Nothing Then
                        ' No respondo para que el terminal vuelva a enviar el mensaje
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:GET iclock/cdata:No response message generated:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    End If

                    ControllerContext.RequestContext.HttpContext.Response.Write(If(bCommesFormSecureConnect, strResponseForSecureConnect, strResponse))
                    ControllerContext.RequestContext.HttpContext.Response.End()
                Else
                    LogSkippedRequest()
                    ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                    ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                    ControllerContext.RequestContext.HttpContext.Response.End()
                End If

                oIncomingMessage = Nothing
                oPushTerminalController = Nothing
                oResponseMessage = Nothing
                GC.Collect()
            Else
                LogSkippedRequest()
                ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                ControllerContext.RequestContext.HttpContext.Response.End()
            End If
        End Sub

        ' GET: iclock/getrequest
        Sub Getrequest(querystring As String) 'As ActionResult
            If Not roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Items("SkipRequest")) Then
                Dim oState = New roTerminalsState(-1)
                Dim strResponse As String = String.Empty
                Dim oHelper As New roPushServerHelper

                Dim SN As String = VTBase.roTypes.Any2String(ControllerContext.RequestContext.HttpContext.Request.Params("SN"))
                Dim strIncomingMessage As String = oHelper.GetPushRawMessageFromHttpRequest(ControllerContext.RequestContext.HttpContext.Request)

                Dim oIncomingMessage As Object = Nothing
                Dim oResponseMessage As Object = Nothing
                Dim oPushTerminalController As Object
                Dim strResponseForSecureConnect As String = String.Empty
                Dim bCommesFormSecureConnect As Boolean = False
                roTrace.GetInstance.AddTraceEvent($"Incoming message:{vbCrLf & strIncomingMessage & vbCrLf}")

                If WebApiApplication.LoggedIn AndAlso WebApiApplication.TerminalLogicIdentity IsNot Nothing Then

                    Dim bLockTaken As Boolean = False

                    Try
                        ' Verificar si la solicitud contiene el encabezado "VTSecureConnect"
                        bCommesFormSecureConnect = roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Request.Headers("VTSecureConnect"))

                        Monitor.TryEnter(GetLockObject(SN), New TimeSpan(0, 0, 5), bLockTaken)
                        If Not bLockTaken Then
                            LogSkippedRequest()
                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:GET iclock/getrequest: Lock on terminal logic:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString)

                            ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                            ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                            ControllerContext.RequestContext.HttpContext.Response.End()

                            oIncomingMessage = Nothing
                            oPushTerminalController = Nothing
                            oResponseMessage = Nothing
                            GC.Collect()
                            Return
                        End If

                        Select Case WebApiApplication.TerminalLogicIdentity.mTerminal.Model.ToUpper
                            Case roTerminalApiSession.roTerminalModel.mxS.ToString.ToUpper
                                oIncomingMessage = New Comms.DrivermxS.BusinesProtocol.MessageMxS(Encoding.Default.GetBytes(strIncomingMessage))
                                oPushTerminalController = New roInBioTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                            Case Else
                                oIncomingMessage = New Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2(Encoding.Default.GetBytes(strIncomingMessage))
                                oPushTerminalController = New roPushTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                        End Select

                        oResponseMessage = oPushTerminalController.ProcessMessage(oIncomingMessage)
                    Catch ex As Exception
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalsPushServer:IClockController:GET iclock/getrequest:Exception processing message:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString, ex)
                    Finally
                        If bLockTaken Then Monitor.Exit(GetLockObject(SN))
                    End Try

                    If oResponseMessage IsNot Nothing AndAlso oState.Result = roTerminalsState.ResultEnum.NoError Then
                        Select Case WebApiApplication.TerminalLogicIdentity.mTerminal.Model.ToUpper
                            Case roTerminalApiSession.roTerminalModel.mxS.ToString.ToUpper
                                Select Case CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).Command
                                    Case Comms.DrivermxS.BusinesProtocol.MessageMxS.msgCommand.command
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_table.toString()
                                        strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_table.HttpContent
                                    Case Else
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.HttpContent
                                End Select
                            Case Else
                                Select Case CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).Command
                                    Case Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2.msgCommand.command
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_table.toString()
                                        strResponse = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_table.HttpContent
                                    Case Else
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_genericresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_genericresponse.HttpContent
                                End Select
                        End Select

                        roTrace.GetInstance.AddTraceEvent($"Response message:{vbCrLf & strResponse & vbCrLf}")
                    ElseIf oState.Result <> roTerminalsState.ResultEnum.NoError Then
                        ' Respondo sin dar OK para que el terminal vuelva a enviar el mensaje (en driver no respondía, pero aquí no hay opción de no responder
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:GET iclock/getrequest:Error processing message:" & oState.Result.ToString & ":" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    ElseIf oResponseMessage Is Nothing Then
                        ' No respondo para que el terminal vuelva a enviar el mensaje
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:GET iclock/getrequest:No response message generated:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    End If

                    ControllerContext.RequestContext.HttpContext.Response.Write(If(bCommesFormSecureConnect, strResponseForSecureConnect, strResponse))
                    ControllerContext.RequestContext.HttpContext.Response.End()
                Else
                    LogSkippedRequest()
                    ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                    ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                    ControllerContext.RequestContext.HttpContext.Response.End()
                End If

                oIncomingMessage = Nothing
                oPushTerminalController = Nothing
                oResponseMessage = Nothing
                GC.Collect()
            Else
                LogSkippedRequest()
                ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                ControllerContext.RequestContext.HttpContext.Response.End()
            End If

        End Sub

        ' POST: iclock/devicecmd
        <HttpPost()>
        Sub Devicecmd(ByVal collection As FormCollection) 'As ActionResult
            If Not roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Items("SkipRequest")) Then
                Dim oState = New roTerminalsState(-1)
                Dim strResponse As String = String.Empty
                Dim oHelper As New roPushServerHelper

                Dim SN As String = VTBase.roTypes.Any2String(ControllerContext.RequestContext.HttpContext.Request.Params("SN"))
                Dim strIncomingMessage As String = oHelper.GetPushRawMessageFromHttpRequest(ControllerContext.RequestContext.HttpContext.Request)

                Dim oIncomingMessage As Object = Nothing
                Dim oResponseMessage As Object = Nothing
                Dim oPushTerminalController As Object
                Dim strResponseForSecureConnect As String = String.Empty
                Dim bCommesFormSecureConnect As Boolean = False
                roTrace.GetInstance.AddTraceEvent($"Incoming message:{vbCrLf & strIncomingMessage & vbCrLf}")

                If WebApiApplication.LoggedIn AndAlso WebApiApplication.TerminalLogicIdentity IsNot Nothing Then

                    Dim bLockTaken As Boolean = False

                    Try
                        ' Verificar si la solicitud contiene el encabezado "VTSecureConnect"
                        bCommesFormSecureConnect = roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Request.Headers("VTSecureConnect"))

                        Monitor.TryEnter(GetLockObject(SN), New TimeSpan(0, 0, 5), bLockTaken)
                        If Not bLockTaken Then
                            LogSkippedRequest()
                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/devicecmd: Lock on terminal logic:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString)

                            ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                            ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                            ControllerContext.RequestContext.HttpContext.Response.End()

                            oIncomingMessage = Nothing
                            oPushTerminalController = Nothing
                            oResponseMessage = Nothing
                            GC.Collect()
                            Return
                        End If

                        Select Case WebApiApplication.TerminalLogicIdentity.mTerminal.Model.ToUpper
                            Case roTerminalApiSession.roTerminalModel.mxS.ToString.ToUpper
                                oIncomingMessage = New Comms.DrivermxS.BusinesProtocol.MessageMxS(Encoding.Default.GetBytes(strIncomingMessage))
                                oPushTerminalController = New roInBioTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                            Case Else
                                oIncomingMessage = New Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2(Encoding.Default.GetBytes(strIncomingMessage))
                                oPushTerminalController = New roPushTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                        End Select

                        oResponseMessage = oPushTerminalController.ProcessMessage(oIncomingMessage)
                    Catch ex As Exception
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalsPushServer:IClockController:POST iclock/devicecmd:Exception processing message:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString, ex)
                    Finally
                        If bLockTaken Then Monitor.Exit(GetLockObject(SN))
                    End Try

                    If oResponseMessage IsNot Nothing AndAlso oState.Result = roTerminalsState.ResultEnum.NoError Then

                        Select Case WebApiApplication.TerminalLogicIdentity.mTerminal.Model.ToUpper
                            Case roTerminalApiSession.roTerminalModel.mxS.ToString.ToUpper
                                Select Case oResponseMessage.Command
                                    Case Comms.DrivermxS.BusinesProtocol.MessageMxS.msgCommand.command
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_table.toString()
                                        strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_table.HttpContent
                                    Case Else
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.HttpContent
                                End Select
                            Case Else
                                Select Case oResponseMessage.Command
                                    Case Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2.msgCommand.command
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_table.toString()
                                        strResponse = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_table.HttpContent
                                    Case Else
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_genericresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_genericresponse.HttpContent
                                End Select
                        End Select

                        roTrace.GetInstance.AddTraceEvent($"Response message:{vbCrLf & strResponse & vbCrLf}")
                    ElseIf oState.Result <> roTerminalsState.ResultEnum.NoError Then
                        ' Respondo sin dar OK para que el terminal vuelva a enviar el mensaje (en driver no respondía, pero aquí no hay opción de no responder
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/devicecmd:Error processing message:" & oState.Result.ToString & ":" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    ElseIf oResponseMessage Is Nothing Then
                        ' No respondo para que el terminal vuelva a enviar el mensaje
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/devicecmd:No response message generated:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    End If

                    ControllerContext.RequestContext.HttpContext.Response.Write(If(bCommesFormSecureConnect, strResponseForSecureConnect, strResponse))
                    ControllerContext.RequestContext.HttpContext.Response.End()
                Else
                    LogSkippedRequest()
                    ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                    ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                    ControllerContext.RequestContext.HttpContext.Response.End()
                End If

                oIncomingMessage = Nothing
                oPushTerminalController = Nothing
                oResponseMessage = Nothing
                GC.Collect()
            Else
                LogSkippedRequest()
                ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                ControllerContext.RequestContext.HttpContext.Response.End()
            End If

        End Sub

        ' POST: iclock/cdata
        <HttpPost()>
        Sub Cdata(ByVal collection As FormCollection) 'As ActionResult
            If Not roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Items("SkipRequest")) Then
                Dim oState = New roTerminalsState(-1)
                Dim strResponse As String = String.Empty
                Dim oHelper As New roPushServerHelper

                Dim SN As String = VTBase.roTypes.Any2String(ControllerContext.RequestContext.HttpContext.Request.Params("SN"))
                Dim strIncomingMessage As String = oHelper.GetPushRawMessageFromHttpRequest(ControllerContext.RequestContext.HttpContext.Request)

                Dim oIncomingMessage As Object = Nothing
                Dim oResponseMessage As Object = Nothing
                Dim oPushTerminalController As Object
                Dim strResponseForSecureConnect As String = String.Empty
                Dim bCommesFormSecureConnect As Boolean = False
                roTrace.GetInstance.AddTraceEvent($"Incoming message:{vbCrLf & strIncomingMessage & vbCrLf}")

                If WebApiApplication.LoggedIn AndAlso WebApiApplication.TerminalLogicIdentity IsNot Nothing Then

                    Dim bLockTaken As Boolean = False

                    Try
                        ' Verificar si la solicitud contiene el encabezado "VTSecureConnect"
                        bCommesFormSecureConnect = roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Request.Headers("VTSecureConnect"))

                        Monitor.TryEnter(GetLockObject(SN), New TimeSpan(0, 0, 5), bLockTaken)
                        If Not bLockTaken Then
                            LogSkippedRequest()
                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/cdata: Lock on terminal logic:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString)
                            ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                            ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                            ControllerContext.RequestContext.HttpContext.Response.End()


                            oIncomingMessage = Nothing
                            oPushTerminalController = Nothing
                            oResponseMessage = Nothing
                            GC.Collect()
                            Return
                        End If

                        Select Case WebApiApplication.TerminalLogicIdentity.mTerminal.Model.ToUpper
                            Case roTerminalApiSession.roTerminalModel.mxS.ToString.ToUpper
                                oIncomingMessage = New Comms.DrivermxS.BusinesProtocol.MessageMxS(Encoding.Default.GetBytes(strIncomingMessage))
                                oPushTerminalController = New roInBioTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                            Case Else
                                oIncomingMessage = New Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2(Encoding.Default.GetBytes(strIncomingMessage))
                                oPushTerminalController = New roPushTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                        End Select

                        oResponseMessage = oPushTerminalController.ProcessMessage(oIncomingMessage)
                    Catch ex As Exception
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalsPushServer:IClockController:POST iclock/cdata:Exception processing message:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString, ex)
                    Finally
                        If bLockTaken Then Monitor.Exit(GetLockObject(SN))
                    End Try

                    If oResponseMessage IsNot Nothing AndAlso oState.Result = roTerminalsState.ResultEnum.NoError Then

                        Select Case WebApiApplication.TerminalLogicIdentity.mTerminal.Model.ToUpper
                            Case roTerminalApiSession.roTerminalModel.mxS.ToString.ToUpper
                                Select Case CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).Command
                                    Case Comms.DrivermxS.BusinesProtocol.MessageMxS.msgCommand.init
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_initresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_initresponse.HttpContent
                                    Case Comms.DrivermxS.BusinesProtocol.MessageMxS.msgCommand.command
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_table.toString()
                                        strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_table.HttpContent
                                    Case Else
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.HttpContent
                                End Select
                            Case Else
                                Select Case CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).Command
                                    Case Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2.msgCommand.init
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_initresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_initresponse.HttpContent
                                    Case Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2.msgCommand.command
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_table.toString()
                                        strResponse = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_table.HttpContent
                                    Case Else
                                        strResponseForSecureConnect = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_genericresponse.toString()
                                        strResponse = CType(oResponseMessage, Comms.DriverZKPush2.BusinesProtocol.MessageZKPush2).data_genericresponse.HttpContent
                                End Select
                        End Select


                        roTrace.GetInstance.AddTraceEvent($"Response message:{vbCrLf & strResponse & vbCrLf}")
                    ElseIf oState.Result <> roTerminalsState.ResultEnum.NoError Then
                        ' Respondo sin dar OK para que el terminal vuelva a enviar el mensaje (en driver no respondía, pero aquí no hay opción de no responder
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/cdata:Error processing message:" & oState.Result.ToString & ":" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try" & vbCrLf & "Incoming message: " & strIncomingMessage)
                    ElseIf oResponseMessage Is Nothing Then
                        ' No respondo para que el terminal vuelva a enviar el mensaje
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/cdata:No response message generated:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try" & vbCrLf & "Incoming message: " & strIncomingMessage)
                    End If

                    ControllerContext.RequestContext.HttpContext.Response.Write(If(bCommesFormSecureConnect, strResponseForSecureConnect, strResponse))
                    ControllerContext.RequestContext.HttpContext.Response.End()
                Else
                    LogSkippedRequest()
                    ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                    ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                    ControllerContext.RequestContext.HttpContext.Response.End()
                End If

                oIncomingMessage = Nothing
                oPushTerminalController = Nothing
                oResponseMessage = Nothing
                GC.Collect()
            Else
                LogSkippedRequest()
                ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                ControllerContext.RequestContext.HttpContext.Response.End()
            End If
        End Sub

#End Region

#Region "Sólo CENTRALITA"

        ' POST: iclock/registry
        <HttpPost()>
        Sub Registry(ByVal collection As FormCollection) 'As ActionResult
            If Not roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Items("SkipRequest")) Then
                Dim oState = New roTerminalsState(-1)
                Dim strResponse As String = String.Empty
                Dim oHelper As New roPushServerHelper

                Dim SN As String = VTBase.roTypes.Any2String(ControllerContext.RequestContext.HttpContext.Request.Params("SN"))
                Dim strIncomingMessage As String = oHelper.GetPushRawMessageFromHttpRequest(ControllerContext.RequestContext.HttpContext.Request)

                Dim oIncomingMessage As New Comms.DrivermxS.BusinesProtocol.MessageMxS(Encoding.ASCII.GetBytes(strIncomingMessage))
                Dim oResponseMessage As Comms.DrivermxS.BusinesProtocol.MessageMxS = Nothing
                Dim oPushTerminalController As roInBioTerminalManager = Nothing
                Dim strResponseForSecureConnect As String = String.Empty
                Dim bCommesFormSecureConnect As Boolean = False
                roTrace.GetInstance.AddTraceEvent($"Incoming message:{vbCrLf & strIncomingMessage & vbCrLf}")

                If WebApiApplication.LoggedIn AndAlso WebApiApplication.TerminalLogicIdentity IsNot Nothing Then

                    Dim bLockTaken As Boolean = False

                    Try
                        ' Verificar si la solicitud contiene el encabezado "VTSecureConnect"
                        bCommesFormSecureConnect = roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Request.Headers("VTSecureConnect"))

                        Monitor.TryEnter(GetLockObject(SN), New TimeSpan(0, 0, 5), bLockTaken)
                        If Not bLockTaken Then
                            LogSkippedRequest()
                            ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                            ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                            ControllerContext.RequestContext.HttpContext.Response.End()

                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/registry: Lock on terminal logic:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString)

                            oIncomingMessage = Nothing
                            oPushTerminalController = Nothing
                            oResponseMessage = Nothing
                            GC.Collect()
                            Return
                        End If

                        oPushTerminalController = New roInBioTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                        oResponseMessage = oPushTerminalController.ProcessMessage(oIncomingMessage)
                    Catch ex As Exception
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalsPushServer:IClockController:POST iclock/registry:Exception processing message:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString, ex)
                    Finally
                        If bLockTaken Then Monitor.Exit(GetLockObject(SN))
                    End Try

                    If oResponseMessage IsNot Nothing AndAlso oState.Result = roTerminalsState.ResultEnum.NoError Then
                        Select Case oResponseMessage.Command
                            Case Comms.DrivermxS.BusinesProtocol.MessageMxS.msgCommand.register
                                strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_registerresponse.toString()
                                strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_registerresponse.HttpContent
                            Case Else
                                strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.toString()
                                strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.HttpContent
                        End Select

                        roTrace.GetInstance.AddTraceEvent($"Response message:{vbCrLf & strResponse & vbCrLf}")
                    ElseIf oState.Result <> roTerminalsState.ResultEnum.NoError Then
                        ' Respondo sin dar OK para que el terminal vuelva a enviar el mensaje (en driver no respondía, pero aquí no hay opción de no responder
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/registry:Error processing message:" & oState.Result.ToString & ":" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    ElseIf oResponseMessage Is Nothing Then
                        ' No respondo para que el terminal vuelva a enviar el mensaje
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/registry:No response message generated:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    End If

                    ControllerContext.RequestContext.HttpContext.Response.Write(If(bCommesFormSecureConnect, strResponseForSecureConnect, strResponse))
                    ControllerContext.RequestContext.HttpContext.Response.End()
                Else
                    LogSkippedRequest()
                    ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                    ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                    ControllerContext.RequestContext.HttpContext.Response.End()
                End If

                oIncomingMessage = Nothing
                oPushTerminalController = Nothing
                oResponseMessage = Nothing
                GC.Collect()
            Else
                LogSkippedRequest()
                ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                ControllerContext.RequestContext.HttpContext.Response.End()
            End If
        End Sub

        ' GET: iclock/push
        Sub Push(querystring As String) 'As ActionResult
            If Not roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Items("SkipRequest")) Then
                Dim oState = New roTerminalsState(-1)
                Dim strResponse As String = String.Empty
                Dim oHelper As New roPushServerHelper

                Dim SN As String = VTBase.roTypes.Any2String(ControllerContext.RequestContext.HttpContext.Request.Params("SN"))
                Dim strIncomingMessage As String = oHelper.GetPushRawMessageFromHttpRequest(ControllerContext.RequestContext.HttpContext.Request)

                Dim oIncomingMessage As New Comms.DrivermxS.BusinesProtocol.MessageMxS(Encoding.ASCII.GetBytes(strIncomingMessage))
                Dim oResponseMessage As Comms.DrivermxS.BusinesProtocol.MessageMxS = Nothing
                Dim oPushTerminalController As roInBioTerminalManager = Nothing
                Dim strResponseForSecureConnect As String = String.Empty
                Dim bCommesFormSecureConnect As Boolean = False
                roTrace.GetInstance.AddTraceEvent($"Incoming message:{vbCrLf & strIncomingMessage & vbCrLf}")

                If WebApiApplication.LoggedIn AndAlso WebApiApplication.TerminalLogicIdentity IsNot Nothing Then

                    Dim bLockTaken As Boolean = False

                    Try
                        ' Verificar si la solicitud contiene el encabezado "VTSecureConnect"
                        bCommesFormSecureConnect = roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Request.Headers("VTSecureConnect"))

                        Monitor.TryEnter(GetLockObject(SN), New TimeSpan(0, 0, 5), bLockTaken)
                        If Not bLockTaken Then
                            LogSkippedRequest()
                            ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                            ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                            ControllerContext.RequestContext.HttpContext.Response.End()

                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:GET iclock/push: Lock on terminal logic:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString)

                            oIncomingMessage = Nothing
                            oPushTerminalController = Nothing
                            oResponseMessage = Nothing
                            GC.Collect()
                            Return
                        End If

                        oPushTerminalController = New roInBioTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                        oResponseMessage = oPushTerminalController.ProcessMessage(oIncomingMessage)
                    Catch ex As Exception
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalsPushServer:IClockController:GET iclock/push:Exception processing message:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString, ex)
                    Finally
                        If bLockTaken Then Monitor.Exit(GetLockObject(SN))
                    End Try

                    If oResponseMessage IsNot Nothing AndAlso oState.Result = roTerminalsState.ResultEnum.NoError Then
                        Select Case oResponseMessage.Command
                            Case Comms.DrivermxS.BusinesProtocol.MessageMxS.msgCommand.getpushconfig
                                strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_configresponse.toString()
                                strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_configresponse.HttpContent
                                oResponseMessage.data_configresponse.GetHttpHeader()
                            Case Else
                                strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.toString()
                                strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.HttpContent
                        End Select

                        roTrace.GetInstance.AddTraceEvent($"Response message:{vbCrLf & strResponse & vbCrLf}")
                    ElseIf oState.Result <> roTerminalsState.ResultEnum.NoError Then
                        ' Respondo sin dar OK para que el terminal vuelva a enviar el mensaje (en driver no respondía, pero aquí no hay opción de no responder
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:GET iclock/push:Error processing message:" & oState.Result.ToString & ":" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    ElseIf oResponseMessage Is Nothing Then
                        ' No respondo para que el terminal vuelva a enviar el mensaje
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:GET iclock/push:No response message generated:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    End If

                    ControllerContext.RequestContext.HttpContext.Response.Write(If(bCommesFormSecureConnect, strResponseForSecureConnect, strResponse))
                    ControllerContext.RequestContext.HttpContext.Response.End()
                Else
                    LogSkippedRequest()
                    ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                    ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                    ControllerContext.RequestContext.HttpContext.Response.End()
                End If

                oIncomingMessage = Nothing
                oPushTerminalController = Nothing
                oResponseMessage = Nothing
                GC.Collect()
            Else
                LogSkippedRequest()
                ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                ControllerContext.RequestContext.HttpContext.Response.End()
            End If
        End Sub

        ' POST: iclock/querydata
        <HttpPost()>
        Sub Querydata(ByVal collection As FormCollection) 'As ActionResult
            If Not roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Items("SkipRequest")) Then
                Dim oState = New roTerminalsState(-1)
                Dim strResponse As String = String.Empty
                Dim oHelper As New roPushServerHelper

                Dim SN As String = VTBase.roTypes.Any2String(ControllerContext.RequestContext.HttpContext.Request.Params("SN"))
                Dim strIncomingMessage As String = oHelper.GetPushRawMessageFromHttpRequest(ControllerContext.RequestContext.HttpContext.Request)

                Dim oIncomingMessage As New Comms.DrivermxS.BusinesProtocol.MessageMxS(Encoding.ASCII.GetBytes(strIncomingMessage))
                Dim oResponseMessage As Comms.DrivermxS.BusinesProtocol.MessageMxS = Nothing
                Dim oPushTerminalController As roInBioTerminalManager = Nothing
                Dim strResponseForSecureConnect As String = String.Empty
                Dim bCommesFormSecureConnect As Boolean = False
                roTrace.GetInstance.AddTraceEvent($"Incoming message:{vbCrLf & strIncomingMessage & vbCrLf}")

                If WebApiApplication.LoggedIn AndAlso WebApiApplication.TerminalLogicIdentity IsNot Nothing Then

                    Dim bLockTaken As Boolean = False

                    Try
                        ' Verificar si la solicitud contiene el encabezado "VTSecureConnect"
                        bCommesFormSecureConnect = roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Request.Headers("VTSecureConnect"))

                        Monitor.TryEnter(GetLockObject(SN), New TimeSpan(0, 0, 5), bLockTaken)
                        If Not bLockTaken Then
                            LogSkippedRequest()
                            ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                            ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                            ControllerContext.RequestContext.HttpContext.Response.End()

                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/querydata: Lock on terminal logic:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString)

                            oIncomingMessage = Nothing
                            oPushTerminalController = Nothing
                            oResponseMessage = Nothing
                            GC.Collect()
                            Return
                        End If

                        oPushTerminalController = New roInBioTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                        oResponseMessage = oPushTerminalController.ProcessMessage(oIncomingMessage)
                    Catch ex As Exception
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalsPushServer:IClockController:POST iclock/querydata:Exception processing message:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString, ex)
                    Finally
                        If bLockTaken Then Monitor.Exit(GetLockObject(SN))
                    End Try

                    If oResponseMessage IsNot Nothing AndAlso oState.Result = roTerminalsState.ResultEnum.NoError Then
                        Select Case oResponseMessage.Command
                            Case Comms.DrivermxS.BusinesProtocol.MessageMxS.msgCommand.queryresult
                                strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_queryresult.toString()
                                strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_queryresult.HttpContent
                            Case Else
                                strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.toString()
                                strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.HttpContent
                        End Select

                        roTrace.GetInstance.AddTraceEvent($"Response message:{vbCrLf & strResponse & vbCrLf}")
                    ElseIf oState.Result <> roTerminalsState.ResultEnum.NoError Then
                        ' Respondo sin dar OK para que el terminal vuelva a enviar el mensaje (en driver no respondía, pero aquí no hay opción de no responder
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/querydata:Error processing message:" & oState.Result.ToString & ":" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    ElseIf oResponseMessage Is Nothing Then
                        ' No respondo para que el terminal vuelva a enviar el mensaje
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/querydata:No response message generated:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    End If

                    ControllerContext.RequestContext.HttpContext.Response.Write(If(bCommesFormSecureConnect, strResponseForSecureConnect, strResponse))
                    ControllerContext.RequestContext.HttpContext.Response.End()
                Else
                    LogSkippedRequest()
                    ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                    ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                    ControllerContext.RequestContext.HttpContext.Response.End()
                End If

                oIncomingMessage = Nothing
                oPushTerminalController = Nothing
                oResponseMessage = Nothing
                GC.Collect()
            Else
                LogSkippedRequest()
                ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                ControllerContext.RequestContext.HttpContext.Response.End()
            End If
        End Sub

        ' GET: iclock/rtdata
        Sub Rtdata(querystring As String) 'As ActionResult
            If Not roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Items("SkipRequest")) Then
                ' INFO: Mensaje que se recibe desde la centralita cuando se le envía la hora a las bravas
                ' A este mensaje se le debe responder con una mensaje del tipo: DateTime=583050990,ServerTZ=+0800
                Dim oState = New roTerminalsState(-1)
                Dim strResponse As String = String.Empty
                Dim oHelper As New roPushServerHelper

                Dim SN As String = VTBase.roTypes.Any2String(ControllerContext.RequestContext.HttpContext.Request.Params("SN"))
                Dim strIncomingMessage As String = oHelper.GetPushRawMessageFromHttpRequest(ControllerContext.RequestContext.HttpContext.Request)

                Dim oIncomingMessage As New Comms.DrivermxS.BusinesProtocol.MessageMxS(Encoding.ASCII.GetBytes(strIncomingMessage))
                Dim oResponseMessage As Comms.DrivermxS.BusinesProtocol.MessageMxS = Nothing
                Dim oPushTerminalController As roInBioTerminalManager = Nothing
                Dim strResponseForSecureConnect As String = String.Empty
                Dim bCommesFormSecureConnect As Boolean = False
                roTrace.GetInstance.AddTraceEvent($"Incoming message:{vbCrLf & strIncomingMessage & vbCrLf}")

                If WebApiApplication.LoggedIn AndAlso WebApiApplication.TerminalLogicIdentity IsNot Nothing Then

                    Dim bLockTaken As Boolean = False

                    Try
                        ' Verificar si la solicitud contiene el encabezado "VTSecureConnect"
                        bCommesFormSecureConnect = roTypes.Any2Boolean(ControllerContext.RequestContext.HttpContext.Request.Headers("VTSecureConnect"))

                        Monitor.TryEnter(GetLockObject(SN), New TimeSpan(0, 0, 5), bLockTaken)
                        If Not bLockTaken Then
                            LogSkippedRequest()
                            ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                            ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                            ControllerContext.RequestContext.HttpContext.Response.End()

                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:Devicecmd: Lock on terminal logic:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString)

                            oIncomingMessage = Nothing
                            oPushTerminalController = Nothing
                            oResponseMessage = Nothing
                            GC.Collect()
                            Return
                        End If

                        oPushTerminalController = New roInBioTerminalManager(oState, WebApiApplication.TerminalLogicIdentity)
                        oResponseMessage = oPushTerminalController.ProcessMessage(oIncomingMessage)
                    Catch ex As Exception
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalsPushServer:IClockController:POST iclock/rtdata:Exception processing message:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString, ex)
                    Finally
                        If bLockTaken Then Monitor.Exit(GetLockObject(SN))
                    End Try

                    If oResponseMessage IsNot Nothing AndAlso oState.Result = roTerminalsState.ResultEnum.NoError Then
                        Select Case oResponseMessage.Command
                            Case Comms.DrivermxS.BusinesProtocol.MessageMxS.msgCommand.synctime
                                strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_synctimeresponse.toString()
                                strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_synctimeresponse.HttpContent
                            Case Else
                                strResponseForSecureConnect = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.toString()
                                strResponse = CType(oResponseMessage, Comms.DrivermxS.BusinesProtocol.MessageMxS).data_genericresponse.HttpContent
                        End Select

                        roTrace.GetInstance.AddTraceEvent($"Response message:{vbCrLf & strResponse & vbCrLf}")
                    ElseIf oState.Result <> roTerminalsState.ResultEnum.NoError Then
                        ' Respondo sin dar OK para que el terminal vuelva a enviar el mensaje (en driver no respondía, pero aquí no hay opción de no responder
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/rtdata:Error processing message:" & oState.Result.ToString & ":" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    ElseIf oResponseMessage Is Nothing Then
                        ' No respondo para que el terminal vuelva a enviar el mensaje
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "TerminalsPushServer:IClockController:POST iclock/rtdata:No response message generated:" & WebApiApplication.TerminalLogicIdentity.mTerminal.ToString & ": Void response message waiting next device try")
                    End If

                    ControllerContext.RequestContext.HttpContext.Response.Write(If(bCommesFormSecureConnect, strResponseForSecureConnect, strResponse))
                    ControllerContext.RequestContext.HttpContext.Response.End()
                Else
                    LogSkippedRequest()
                    ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                    ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                    ControllerContext.RequestContext.HttpContext.Response.End()
                End If

                oIncomingMessage = Nothing
                oPushTerminalController = Nothing
                oResponseMessage = Nothing
                GC.Collect()
            Else
                LogSkippedRequest()
                ControllerContext.RequestContext.HttpContext.Response.StatusCode = 401
                ControllerContext.RequestContext.HttpContext.Response.Status = "403 Access Denied"
                ControllerContext.RequestContext.HttpContext.Response.End()
            End If
        End Sub

#End Region


        Private Sub LogSkippedRequest()
            Try
                Dim oHelper As New roPushServerHelper
                Dim strIncomingMessage As String = oHelper.GetPushRawMessageFromHttpRequest(ControllerContext.RequestContext.HttpContext.Request)

                If strIncomingMessage.Contains("SN=") Then VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, $"TerminalsPushServer:IClockController:Message skipped:{strIncomingMessage}")
            Catch ex As Exception
                VTBase.roLog.GetInstance().logSystemMessage(roLog.EventType.roError, "TerminalsPushServer:IClockController:LogSkippedRequest::Unknown error", ex)
            End Try

        End Sub
    End Class

End Namespace