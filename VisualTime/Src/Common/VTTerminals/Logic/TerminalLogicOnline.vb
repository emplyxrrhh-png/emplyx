Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness

Namespace VTTerminals

    ''' <summary>
    ''' TO BE CONTINUED
    ''' Motor de pantalla interactivas para terminales mx9
    ''' </summary>
    Public Class roTerminalLogicOnline

        Public Enum ePunchState
            INIT
            IDLE
            PRE
            PRE_INV
        End Enum

        Private mTerminal As Terminal.roTerminal

        Protected Const RESP_BTN_INCIDENCE = "INC"

        Protected Const ICON_CAUSE = "cause"

        Public Sub New(oTerminal As Terminal.roTerminal)
            Me.mTerminal = oTerminal
        End Sub

        ''' <summary>
        ''' Ha llegado un fichaje desde el terminal
        ''' </summary>
        ''' <param name="oIncomingTerminalPunch"></param>
        ''' <param name="oCurrentPunch"></param>
        ''' <returns></returns>
        Public Function STATE_OUT_PRE(oIncomingTerminalPunch As roTerminalInteractivePunch, ByRef mCurrentPunch As roTerminalPunch) As roTerminalInteractivePunch
            ''Dim oResponse As New roTerminalInteractivePunch(InteractivePunchCommand.Display)
            ''Dim lRightButtons As New List(Of roTerminalButton)
            ''Try
            ''    If mCurrentPunch.IDEmployee > 0 Then
            ''        'Solo miramos si es un empleado valido.
            ''        If Not (mTerminal.ReaderByID(1).Mode = "ACC" OrElse mTerminal.ReaderByID(1).Mode = "ACCTA") And Not mTerminal.ReaderByID(1).EmployeePermit(mCurrentPunch.IDEmployee, False, False, False) Then
            ''            'mCurrentPunch.Discard = True
            ''            oResponse = STATE_OUT_PRE_INV(mCurrentPunch)
            ''            Return oResponse
            ''        End If
            ''    End If
            ''    Dim strWork As String = ""

            ''    If mCurrentPunch.IDEmployee > 0 Then
            ''        oResponse.Display.UserInfo = oIncomingTerminalPunch.EmployeeStatus.EmployeeName
            ''        Select Case mCurrentPunch.PunchType
            ''            Case PunchType.Incomplete
            ''                'WorkArea
            ''                strWork = Translate("pre.in.plus", "{$25}Entrada$L")
            ''                strWork += Translate("pre.punchOf.plus", "$L{$14}${Punch} de las ${CurrentPunch.Time}")
            ''                If mCurrentPunch.PunchData.AttendanceData.IdCause > 0 Then strWork += Translate("pre.CauseWith.plus", " por ${CurrentPunch.IncidenceName}")
            ''                If mCurrentPunch.FirstDayPunch Then
            ''                    strWork += Translate("pre.FirstIn.plus", "$L{$14}Primera entrada de la jornada")
            ''                Else
            ''                    strWork += Translate("pre.OutAgo.plus", "$L{$14}Salió hace ${CurrentPunch.TimeBetweenLastPunch}")
            ''                End If
            ''                strWork += Translate("pre.ForgotPunchLastDay.plus", "$L{$14}Olvidó la salida del día anterior!")

            ''                ' Botones
            ''                ' TODO: Control de persmisos para fichar justificaciones
            ''                ' If mTerminal.AllowCauses AndAlso mCurrentEmployee.AllowedCauses <> "-1" Then
            ''                If True Then
            ''                    lRightButtons.Add(New roTerminalButton(Translate("pre.btn.Cause", "Motivo"), RESP_BTN_INCIDENCE, ICON_CAUSE))
            ''                End If
            ''                'If mTerminal.RDRAllowMoveChange(mCurrentPunch.Reader) And mCurrentEmployee.AllowRequestForgottenPunches And Not mCurrentPunch.LastMoveIsNotReliable Then
            ''                '    mSendMessage.data_display.addButton(Translate("pre.btn.NoIn", "No entro"), RESP_BTN_CHANGE, ICON_ISOUT)
            ''                'End If
            ''                'If mTerminal.RDRAllowABK(mCurrentPunch.Reader) And mCurrentEmployee.ID > 0 Then
            ''                '    mSendMessage.data_display.addButton(Translate("abk.button", "Abrir puerta"), "ABK", ICON_ACCESSBYKEY)
            ''                'End If
            ''                'oResponse.Display.setActiveInputs(False, True, True)
            ''            Case PunchType.AttIn
            ''                'strWork = Translate("pre.in.plus", "{$25}Entrada")
            ''                'strWork += Translate("pre.punchOf.plus", "$L{$14}Fichaje de las ${CurrentPunch.Time}")
            ''                'If mCurrentPunch.IDIncidence > 0 Then strWork += Translate("pre.CauseWith.plus", " por ${CurrentPunch.IncidenceName}")
            ''                'If mCurrentPunch.FirstPunch Then
            ''                '    strWork += Translate("pre.FirstIn.plus", "$L{$14}Primera entrada de la jornada")
            ''                'Else
            ''                '    strWork += Translate("pre.OutAgo.plus", "$L{$14}Salió hace ${CurrentPunch.TimeBetweenLastPunch}")
            ''                '    If mCurrentEmployee.PresenceToday(mCurrentPunch.DatePunch) > 5 Then strWork += Translate("pre.PresenceToday.plus", "$L{$12}Tiempo presencia de hoy es ${1}", clsTextHelper.Minutes2StringLong(mCurrentEmployee.PresenceToday(mCurrentPunch.DatePunch)))
            ''                'End If

            ''                ''Botones
            ''                'If mTerminal.AllowCauses AndAlso mCurrentEmployee.AllowedCauses <> "-1" Then mSendMessage.data_display.addButton(Translate("pre.btn.Cause", "Motivo"), RESP_BTN_INCIDENCE, ICON_CAUSE)
            ''                'If mTerminal.RDRAllowMoveChange(mCurrentPunch.Reader) And mCurrentEmployee.AllowRequestForgottenPunches And Not mCurrentPunch.LastMoveIsNotReliable Then
            ''                '    mSendMessage.data_display.addButton(Translate("pre.btn.NoIn", "No entro"), RESP_BTN_CHANGE, ICON_ISOUT)
            ''                'End If
            ''                'If mTerminal.RDRAllowABK(mCurrentPunch.Reader) And mCurrentEmployee.ID > 0 Then
            ''                '    mSendMessage.data_display.addButton(Translate("abk.button", "Abrir puerta"), "ABK", ICON_ACCESSBYKEY)
            ''                'End If
            ''                'mSendMessage.data_display.setActiveInputs(False, True, True)
            ''            Case PunchType.AttOut
            ''                'strWork = Translate("pre.out.plus", "{$25}Salida")
            ''                'strWork += Translate("pre.punchOf.plus", "$L{$14}Fichaje de las ${CurrentPunch.Time}")
            ''                'If mCurrentPunch.IDIncidence > 0 Then strWork += Translate("pre.CauseWith.plus", " por ${CurrentPunch.IncidenceName}")
            ''                'strWork += Translate("pre.InAgo.plus", "$L{$12}Entró hace ${CurrentPunch.TimeBetweenLastPunch}")
            ''                'If mCurrentEmployee.PresenceToday(mCurrentPunch.DatePunch) > 5 Then strWork += Translate("pre.PresenceToday.plus", "$L{$12}Tiempo presencia de hoy es ${1}", clsTextHelper.Minutes2StringLong(mCurrentEmployee.PresenceToday(mCurrentPunch.DatePunch)))

            ''                ''Botones
            ''                'If mTerminal.AllowCauses AndAlso mCurrentEmployee.AllowedCauses <> "-1" Then mSendMessage.data_display.addButton(Translate("pre.btn.Cause", "Motivo"), RESP_BTN_INCIDENCE, ICON_CAUSE)
            ''                'If mTerminal.RDRAllowMoveChange(mCurrentPunch.Reader) And mCurrentEmployee.AllowRequestForgottenPunches And Not mCurrentPunch.LastMoveIsNotReliable Then
            ''                '    mSendMessage.data_display.addButton(Translate("pre.btn.NoOut", "No salgo"), RESP_BTN_CHANGE, ICON_ISIN)
            ''                'End If
            ''                'If mTerminal.RDRAllowABK(mCurrentPunch.Reader) And mCurrentEmployee.ID > 0 Then
            ''                '    mSendMessage.data_display.addButton(Translate("abk.button", "Abrir puerta"), "ABK", ICON_ACCESSBYKEY)
            ''                'End If
            ''                'mSendMessage.data_display.setActiveInputs(False, True, True)
            ''            Case PunchType.RepeatIn
            ''                '' Se ha fichado Entrada hace unos instantes
            ''                'strWork = Translate("pre.ChangeRepeatedInQuestion.plus", "{$20}Cambio$L{$18}Ya ha fichado hace unos instantes$L¿Qué desea hacer?")

            ''                ''Botones
            ''                'mSendMessage.data_display.addButton(Translate("pre.btn.Out", "Salgo"), RESP_BTN_NEWOUT, ICON_ISOUT)
            ''                'mSendMessage.data_display.addButton(Translate("pre.btn.Nothing", "Nada"), RESP_BTN_BACK, ICON_CANCEL_PUNCH)
            ''                'If mTerminal.RDRAllowABK(mCurrentPunch.Reader) And mCurrentEmployee.ID > 0 Then
            ''                '    mSendMessage.data_display.addButton(Translate("abk.button", "Abrir puerta"), "ABK", ICON_ACCESSBYKEY)
            ''                'End If
            ''                'mSendMessage.data_display.setActiveInputs(False, False, False)
            ''            Case PunchType.RepeatOut
            ''                ' Se ha fichado Salida hace unos instantes

            ''                'WorkArea
            ''                '    strWork = Translate("pre.ChangeRepeatedOutQuestion.plus", "{$20}Cambio$L{$15}Ya ha fichado hace unos instantes$L¿Qué desea hacer?")

            ''                '    'Botones
            ''                '    mSendMessage.data_display.addButton(Translate("pre.btn.In", "Entro"), RESP_BTN_NEWIN, ICON_ISIN)
            ''                '    mSendMessage.data_display.addButton(Translate("pre.btn.Out", "Salgo"), RESP_BTN_NEWOUT, ICON_ISOUT)
            ''                '    If mTerminal.RDRAllowABK(mCurrentPunch.Reader) And mCurrentEmployee.ID > 0 Then
            ''                '        mSendMessage.data_display.addButton(Translate("abk.button", "Abrir puerta"), "ABK", ICON_ACCESSBYKEY)
            ''                '    End If
            ''                '    mSendMessage.data_display.setActiveInputs(False, False, False)
            ''                'Case PunchType.Cancel
            ''                '    strWork = Translate("pre.cancel.plus", "{$20}Operación cancelada.")
            ''        End Select
            ''    Else
            ''        'mSendMessage.data_display.UserInfo = " "
            ''        'strWork = Translate("pre.NoEmployeeSavePunch", "{$25}Código inexistente!$L{$15}Fichaje almacenado")
            ''        'mSendMessage.data_display.setActiveInputs(False, True, True)
            ''        'mSendMessage.data_display.Timeout = TimeOut_VeryShort
            ''    End If

            ''    oResponse.Display.WorkArea = strWork
            ''    If oResponse.Display.Timeout = 0 Then oResponse.Display.Timeout = InteractivePunchDisplayTimeout.TimeOut_Medium
            ''    mCurrentPunch.PunchState = ePunchState.PRE.ToString
            ''Catch ex As Exception

            ''End Try

            ''Return oResponse
            Return Nothing
        End Function

        Public Function STATE_OUT_IDLE(oIncomingTerminalPunch As roTerminalInteractivePunch, ByRef mCurrentPunch As roTerminalPunch) As roTerminalInteractivePunch
            Dim oResponse As New roTerminalInteractivePunch(InteractivePunchCommand.Idle)
            Try
                mCurrentPunch.PunchState = ePunchState.IDLE.ToString
            Catch ex As Exception

            End Try

            Return oResponse
        End Function

        Public Function STATE_OUT_PRE_INV(ByRef mCurrentPunch As roTerminalPunch) As roTerminalInteractivePunch
            Dim oResponse As New roTerminalInteractivePunch(InteractivePunchCommand.Display)
            Try
                oResponse.Display.WorkArea = "Fichaje no permitido"
                oResponse.Display.Timeout = InteractivePunchDisplayTimeout.TimeOut_VeryShort
                mCurrentPunch.PunchState = ePunchState.PRE_INV.ToString
            Catch ex As Exception

            End Try

            Return oResponse
        End Function

        Public Function STATE_IN_PRE(oIncomingTerminalPunch As roTerminalInteractivePunch, ByRef oCurrentPunch As roTerminalPunch) As roTerminalInteractivePunch
            Dim oResponse As New roTerminalInteractivePunch(InteractivePunchCommand.Display)
            Try
                Select Case oIncomingTerminalPunch.Display.Response.ToUpper
                    Case "TIMEOUT", "OK"
                        oResponse = STATE_OUT_MSG(oIncomingTerminalPunch, oCurrentPunch, "TEST: Fichaje almacenado")
                    Case Else
                        oResponse = STATE_OUT_MSG(oIncomingTerminalPunch, oCurrentPunch, "TEST: Acción desconocida")
                End Select
            Catch ex As Exception

            End Try
            Return oResponse
        End Function

        Public Function STATE_OUT_MSG(oIncomingTerminalPunch As roTerminalInteractivePunch, ByRef oCurrentPunch As roTerminalPunch, strMessage As String) As roTerminalInteractivePunch
            Dim oResponse As New roTerminalInteractivePunch(InteractivePunchCommand.Display)
            Try
                oResponse.Display.WorkArea = strMessage
                oCurrentPunch.PunchState = ePunchState.IDLE.ToString
            Catch ex As Exception

            End Try

            Return oResponse
        End Function

        Private Function Translate(sKey, sDefaultText) As String
            ' TODO: Traducir a partir de State basado en el empleado que ha fichado
            Return sDefaultText
        End Function

    End Class

End Namespace