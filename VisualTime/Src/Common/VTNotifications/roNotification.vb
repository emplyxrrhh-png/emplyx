Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes
Imports System.Data.Common
Imports System.Runtime.Serialization

Namespace Notifications

    <DataContract()>
    Public Class roNotification

#Region "Declarations - Constructor"

        Private oState As roNotificationState

        Private intID As Integer
        Private intIDType As Integer
        Private strName As String
        Private oCondition As New roNotificationCondition
        Private oDestination As New roNotificationDestination
        Private strSchedule As String
        Private strComments As String
        Private bolActivated As Boolean
        Private bolAllowPortal As Boolean
        Private bolAllowVTPortal As Boolean
        Private bolAllowMail As Boolean
        Private bolShowOnDesktop As Boolean
        Private bolIsSystemNotification As Boolean = False
        Private intIDPassportDestination As Integer
        Private intIDCategory As CategoryType

        Public Sub New()
            Me.oState = New roNotificationState
            Me.ID = -1
            Me.IDType = -1
            strName = String.Empty

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roNotificationState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.ID = _ID
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Estado de la solicitud
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <IgnoreDataMember()>
        Public Property State() As roNotificationState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roNotificationState)
                Me.oState = value
            End Set
        End Property

        ''' <summary>
        ''' ID de la notificación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
            End Set
        End Property

        ''' <summary>
        ''' Tipo de la notificación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDType() As Integer
            Get
                Return intIDType
            End Get
            Set(ByVal value As Integer)
                intIDType = value
            End Set
        End Property

        ''' <summary>
        ''' Permite notificacion por el portal del supervisor desde Supervisor Portal
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property AllowPortal() As Boolean
            Get
                Return bolAllowPortal
            End Get
            Set(ByVal value As Boolean)
                bolAllowPortal = value
            End Set
        End Property

        ''' <summary>
        ''' Permite notificacion por el portal del empleado desde VTPortal
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property AllowVTPortal() As Boolean
            Get
                Return bolAllowVTPortal
            End Get
            Set(ByVal value As Boolean)
                bolAllowVTPortal = value
            End Set
        End Property

        ''' <summary>
        ''' Permite notificacion por el portal del supervisor mediante mail
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property AllowMail() As Boolean
            Get
                Return bolAllowMail
            End Get
            Set(ByVal value As Boolean)
                bolAllowMail = value
            End Set
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ShowOnDesktop() As Boolean
            Get
                Return bolShowOnDesktop
            End Get
            Set(ByVal value As Boolean)
                bolShowOnDesktop = value
            End Set
        End Property

        ''' <summary>
        ''' Pasaporte del destinatario
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDPassportDestination() As Integer
            Get
                Return intIDPassportDestination
            End Get
            Set(ByVal value As Integer)
                intIDPassportDestination = value
            End Set
        End Property

        <DataMember()>
        Public Property IDCategory() As CategoryType
            Get
                Return intIDCategory
            End Get
            Set(ByVal value As CategoryType)
                intIDCategory = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre descriptivo de la notificación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        ''' <summary>
        ''' Condición de la notificación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Condition() As roNotificationCondition
            Get
                Return oCondition
            End Get
            Set(ByVal value As roNotificationCondition)
                oCondition = value
            End Set
        End Property

        ''' <summary>
        ''' Destino de la notificación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Destination() As roNotificationDestination
            Get
                Return oDestination
            End Get
            Set(ByVal value As roNotificationDestination)
                oDestination = value
            End Set
        End Property

        ''' <summary>
        ''' Programación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Schedule() As String
            Get
                Return strSchedule
            End Get
            Set(ByVal value As String)
                strSchedule = value
            End Set
        End Property

        ''' <summary>
        ''' Estado de la notificación (Activada?)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Activated() As Boolean
            Get
                Return bolActivated
            End Get
            Set(ByVal value As Boolean)
                bolActivated = value
            End Set
        End Property

        ''' <summary>
        ''' Indica si se trata de una Notificación de sistema
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IsSystem() As Boolean
            Get
                Return bolIsSystemNotification
            End Get
            Set(ByVal value As Boolean)
                bolIsSystemNotification = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Si no me han informado el ID, y es una notificación de sistema y me han informado el tipo, recupero el ID
                If Me.ID = -1 AndAlso Me.IsSystem AndAlso Me.IDType > 0 Then
                    Me.ID = GetSystemNotificationIDByType(Me.IDType, oState)
                End If

                Dim strSQL As String = "@SELECT# Notifications.*, sysroNotificationTypes.IDCategory FROM Notifications INNER JOIN sysroNotificationTypes ON sysroNotificationTypes.ID =  Notifications.IDType WHERE Notifications.ID = " & Me.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    intIDType = oRow("IDType")
                    strName = oRow("Name")

                    intIDCategory = Any2Integer(oRow("IDCategory"))

                    If Not IsDBNull(oRow("Schedule")) Then
                        strSchedule = oRow("Schedule")
                    End If

                    bolActivated = oRow("Activated")

                    If Not IsDBNull(oRow("Condition")) Then
                        oCondition = New roNotificationCondition(oState, oRow("Condition"))
                    End If

                    If Not IsDBNull(oRow("Destination")) Then
                        oDestination = New roNotificationDestination(oState, oRow("Destination"))
                    End If

                    bolAllowPortal = oRow("AllowPortal")
                    bolAllowVTPortal = roTypes.Any2Boolean(oRow("AllowVTPortal"))
                    bolAllowMail = oRow("Allowmail")
                    bolShowOnDesktop = oRow("ShowOnDesktop")

                    If Not IsDBNull(oRow("IDPassportDestination")) Then
                        intIDPassportDestination = oRow("IDPassportDestination")
                    End If

                    bolRet = True

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, String.Empty, 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tNotification, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roNotification::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::Load")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene el siguiente ID disponible para dar de alta una nueva solicitud
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextID() As Integer

            Dim intRet As Integer = 5000

            Dim strSQL As String = "@SELECT# ISNULL(Max(ID) + 1,5000) FROM Notifications where CreatorID is null and ID >= 5000"

            Dim tb As DataTable = CreateDataTable(strSQL)
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet

        End Function

        Public Function Validate(Optional ByVal bolCheckNames As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try

                'Comprobar que el nombre de la notificacion no esté en uso
                If ExistName() Then
                    Me.oState.Result = NotificationResultEnum.NotificacionNameAlreadyExist
                    bolRet = False
                End If

                'Comprobar que el nombre de la notificacion tenga un nombre
                If bolRet AndAlso strName.Trim = String.Empty Then
                    Me.oState.Result = NotificationResultEnum.WithoutName
                    bolRet = False
                End If

                If bolRet AndAlso intIDType < 1 Then
                    oState.Result = NotificationResultEnum.NoSelectedType
                    bolRet = False
                End If

                If bolRet AndAlso Not bolAllowPortal AndAlso Not bolAllowVTPortal AndAlso Not bolAllowMail AndAlso oDestination.Destination = String.Empty AndAlso oCondition.MailListUserfield = String.Empty AndAlso (oCondition.ConditionRole Is Nothing OrElse oCondition.ConditionRole.Length = 0) Then
                    oState.Result = NotificationResultEnum.IncorrectSelectionDestination
                    bolRet = False
                End If

                If bolRet Then
                    If oCondition IsNot Nothing Then
                        Select Case intIDType
                            Case eNotificationType.Before_Begin_Contract '1
                                If Not oCondition.DaysBefore.HasValue Then
                                    oState.Result = NotificationResultEnum.ConditionsIncorrectDaysBefore
                                    bolRet = False
                                Else
                                    If oCondition.DaysBefore.Value < 1 Then
                                        oState.Result = NotificationResultEnum.ConditionsIncorrectDaysBefore
                                        bolRet = False
                                    End If
                                End If
                            Case eNotificationType.Before_Finish_Contract '2
                                If Not oCondition.DaysBefore.HasValue Then
                                    oState.Result = NotificationResultEnum.ConditionsIncorrectDaysBefore
                                    bolRet = False
                                Else
                                    If oCondition.DaysBefore.Value < 1 Then
                                        oState.Result = NotificationResultEnum.ConditionsIncorrectDaysBefore
                                        bolRet = False
                                    End If
                                End If
                            Case eNotificationType.Before_Begin_Programmed_Absence, eNotificationType.Before_Finish_Programmed_Absence '5,3
                                If Not oCondition.DaysBefore.HasValue Then
                                    oState.Result = NotificationResultEnum.ConditionsIncorrectDaysBefore
                                    bolRet = False
                                Else
                                    If oCondition.DaysBefore.Value < 1 Then
                                        oState.Result = NotificationResultEnum.ConditionsIncorrectDaysBefore
                                        bolRet = False
                                    End If
                                End If

                                If Not oCondition.IDCause.HasValue Then
                                    oState.Result = NotificationResultEnum.ConditionsNoIDCause
                                    bolRet = False
                                Else
                                    If oCondition.IDCause.Value < 1 Then
                                        oState.Result = NotificationResultEnum.ConditionsNoIDCause
                                        bolRet = False
                                    End If
                                End If
                            Case eNotificationType.Begin_Programmed_Absence '4
                                If Not oCondition.IDCause.HasValue Then
                                    oState.Result = NotificationResultEnum.ConditionsNoIDCause
                                    bolRet = False
                                Else
                                    If oCondition.IDCause.Value < 1 Then
                                        oState.Result = NotificationResultEnum.ConditionsNoIDCause
                                        bolRet = False
                                    End If
                                End If
                            Case eNotificationType.Finish_Programmed_Absence_and_dont_work_later '6
                                If Not oCondition.DaysBefore.HasValue Then
                                    oState.Result = NotificationResultEnum.ConditionsIncorrectDaysNoWorking
                                    bolRet = False
                                Else
                                    If oCondition.DaysBefore.Value < 1 Then
                                        oState.Result = NotificationResultEnum.ConditionsIncorrectDaysNoWorking
                                        bolRet = False
                                    End If
                                End If
                            Case eNotificationType.Cut_Programmed_Absence '7
                            Case eNotificationType.Punch_with_any_Cause '8
                                If Not oCondition.IDCause.HasValue Then
                                    oState.Result = NotificationResultEnum.ConditionsNoIDCause
                                    bolRet = False
                                Else
                                    If oCondition.IDCause.Value < 1 Then
                                        oState.Result = NotificationResultEnum.ConditionsNoIDCause
                                        bolRet = False
                                    End If
                                End If
                            Case eNotificationType.Invalid_Access '9
                            Case eNotificationType.Error_Messages '10
                            Case eNotificationType.Terminal_Disconnected '11
                            Case eNotificationType.Access_Dennied_Framework_Security '12

                            Case eNotificationType.Employee_Not_Arrived_or_Late '15

                            Case eNotificationType.End_Period_Employee '16
                                If Not bolAllowPortal Then
                                    If Not oCondition.DaysBefore.HasValue Then
                                        oState.Result = NotificationResultEnum.ConditionsIncorrectDaysBefore
                                        bolRet = False
                                    Else
                                        If oCondition.DaysBefore.Value < 1 Then
                                            oState.Result = NotificationResultEnum.ConditionsIncorrectDaysBefore
                                            bolRet = False
                                        Else
                                            If oCondition.DatePeriodUserfield = String.Empty Then
                                                oState.Result = NotificationResultEnum.IncorrectConditions
                                                bolRet = False
                                            End If
                                        End If
                                    End If
                                End If

                            Case eNotificationType.End_Period_Enterprise '17
                                'tiene que tener email o cmapo de la ficha. No pueden estar los dos vacios
                                If Not bolAllowPortal Then
                                    If Not oCondition.DaysBefore.HasValue Then
                                        oState.Result = NotificationResultEnum.ConditionsIncorrectDaysBefore
                                        bolRet = False
                                    Else
                                        If oCondition.DaysBefore.Value < 1 Then
                                            oState.Result = NotificationResultEnum.ConditionsIncorrectDaysBefore
                                            bolRet = False
                                        Else
                                            If oCondition.DatePeriodUserfield = String.Empty Then
                                                oState.Result = NotificationResultEnum.IncorrectConditions
                                                bolRet = False
                                            End If
                                        End If
                                    End If
                                End If

                            Case eNotificationType.Concept_Not_Reached ' 18
                                'tiene que tener email o cmapo de la ficha. No pueden estar los dos vacios
                                If Not bolAllowPortal Then
                                    'tiene que tener saldo
                                    If Not oCondition.IDConcept.HasValue OrElse oCondition.IDConcept <= 0 Then
                                        oState.Result = NotificationResultEnum.ConditionsNoIDConcept
                                        bolRet = False
                                    Else
                                        'tiene que tener comparacion
                                        If Not oCondition.tCompareConceptType.HasValue Then
                                            oState.Result = NotificationResultEnum.ConditionsNoCompareType
                                            bolRet = False
                                        Else
                                            'tiene que tener tipo
                                            If String.IsNullOrEmpty(oCondition.TargetTypeConcept) Then
                                                oState.Result = NotificationResultEnum.ConditionsNoTargetTypeConcept
                                                bolRet = False
                                            Else
                                                'tiene que tener saldo, comparacion, tipo valor o campo de la ficha
                                                If String.IsNullOrEmpty(oCondition.TargetConcept) Then
                                                    oState.Result = NotificationResultEnum.ConditionsNoTargetConcept
                                                    bolRet = False
                                                End If
                                            End If
                                        End If
                                    End If
                                End If

                            Case eNotificationType.Day_with_Unreliable_Time_Record, eNotificationType.Non_Justified_Incident, eNotificationType.IDCard_Not_Assigned_To_Employee, eNotificationType.Task_Close_to_Finish, eNotificationType.Task_Close_to_Start, eNotificationType.Task_Exceeding_Planned_Time, eNotificationType.Task_Exceeding_Finished_Date, eNotificationType.Kpi_Limit_OverTaken, eNotificationType.Task_exceeding_Started_Date, eNotificationType.Task_With_ALerts, eNotificationType.Document_Not_Delivered    '19,20,21,22,23,24,25,26,28

                        End Select
                    Else
                        oState.Result = NotificationResultEnum.IncorrectConditions
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::Validate")
            End Try

            Return bolRet

        End Function

        Private Function ExistName() As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@SELECT# ID FROM Notifications WHERE Name = '" & strName & "' AND ID <> " & intID
                Dim tb As DataTable = CreateDataTable(strSQL)
                bolRet = (tb.Rows.Count > 0)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::ExistName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDiningRoomTurn::ExistName")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.State.Result = NotificationResultEnum.XSSvalidationError
                    Return False
                End If

                Dim oNotificationOld As DataRow = Nothing
                Dim oNotificationNew As DataRow = Nothing

                If Me.Validate() Then

                    Dim oOldNotification As roNotification = Nothing

                    Dim tb As New DataTable("Notifications")
                    Dim strSQL As String = "@SELECT# * FROM Notifications WHERE ID = " & Me.ID
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("ID") = Me.GetNextID()
                        Me.ID = oRow("ID")
                    Else
                        oOldNotification = New roNotification(Me.ID, Me.State)
                        oRow = tb.Rows(0)
                        oNotificationOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("IDType") = intIDType
                    oRow("Name") = strName
                    If oCondition IsNot Nothing Then oRow("Condition") = oCondition.GetXml
                    If oDestination IsNot Nothing Then oRow("Destination") = oDestination.GetXml
                    oRow("Schedule") = strSchedule
                    oRow("Activated") = bolActivated

                    oRow("AllowPortal") = bolAllowPortal
                    oRow("AllowVTPortal") = bolAllowVTPortal
                    oRow("AllowMail") = bolAllowMail
                    oRow("ShowOnDesktop") = bolShowOnDesktop

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oNotificationNew = oRow

                    bolRet = True

                    If bolRet AndAlso bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oNotificationNew, oNotificationOld)
                        Dim oAuditAction As Audit.Action = IIf(oNotificationOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oNotificationNew("Name")
                        ElseIf oNotificationOld("Name") <> oNotificationNew("Name") Then
                            strObjectName = oNotificationOld("Name") & " -> " & oNotificationNew("Name")
                        Else
                            strObjectName = oNotificationNew("Name")
                        End If

                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tNotification, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::Save")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Borra la notificación
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                'Borramos la solicitud
                Dim DelQuerys As String() = {$"@DELETE# FROM sysroNotificationTasks Where IDNotification = {Me.ID}",
                                             $"@DELETE# FROM Notifications WHERE ID = {Me.ID}"}
                For n As Integer = 0 To DelQuerys.Length - 1
                    If Not ExecuteSql(DelQuerys(n)) Then
                        oState.Result = NotificationResultEnum.ConnectionError
                        Exit For
                    End If
                Next

                bolRet = (oState.Result = NotificationResultEnum.NoError)

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{NotificationName}", Me.strName, String.Empty, 1)
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tNotification, strName, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function DeleteIncompletePunchNotificationIfExist(ByVal _State As roNotificationState, ByVal _IDEmployee As Integer, ByVal _Date As Date) As Boolean
            Dim bolRet As Boolean = True

            Try

                Dim strSQL = "@delete# from sysroNotificationTasks where IDNotification = 1001 and Key1Numeric = " & _IDEmployee & " and Key3DateTime = " & roTypes.Any2Time(_Date.Date).SQLDateTime

                bolRet = ExecuteSql(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roNotification::ExecuteAssignedShiftNotification")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roNotification::ExecuteAssignedShiftNotification")
            End Try

            Return bolRet

        End Function

        Public Shared Function ExecuteAssignedShiftNotification(ByVal _State As roNotificationState, ByVal oNotifications As Generic.List(Of Notifications.roNotification), ByVal _IDEmployee As Integer,
                                                                ByVal _Date As Date, ByVal _IDShift As Integer, ByVal _IDOldShift As Integer, ByVal bFromCalendar As Boolean) As Boolean
            Dim bolRet As Boolean = True

            Try

                If oNotifications IsNot Nothing AndAlso oNotifications.Count > 0 Then
                    For Each oNotification As Notifications.roNotification In oNotifications
                        If oNotification.Condition.IDShifts IsNot Nothing AndAlso oNotification.Condition.IDShifts.Contains(_IDShift) AndAlso
                            ((oNotification.Condition.OnlyFromCalendar AndAlso bFromCalendar) OrElse (Not oNotification.Condition.OnlyFromCalendar)) Then
                            Dim sSql As String = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric,Key2Numeric, Key3DateTime, Key5Numeric, Executed, FiredDate, IsReaded) " &
                                     " @SELECT# " & oNotification.ID.ToString & "," & _IDEmployee.ToString & "," & _IDShift.ToString & "," & roTypes.Any2Time(_Date).SQLDateTime & "," & _IDOldShift.ToString & ",0," & roTypes.Any2Time(DateTime.Now).SQLDateTime & "," & IIf(oNotification.AllowVTPortal, "0", "1") & " WHERE Not EXISTS (@SELECT# * " &
                                    " From sysroNotificationTasks " &
                                    " Where sysroNotificationTasks.Key1Numeric = " & _IDEmployee.ToString &
                                    " And sysroNotificationTasks.Key2Numeric = " & _IDShift.ToString &
                                    " And sysroNotificationTasks.Key5Numeric = " & _IDOldShift.ToString &
                                    " And sysroNotificationTasks.Key3DateTime = " & roTypes.Any2Time(_Date).SQLDateTime &
                                    " And IDNotification=" & oNotification.ID & ") "
                            bolRet = ExecuteSqlWithoutTimeOut(sSql)
                        End If
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roNotification::ExecuteAssignedShiftNotification")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roNotification::ExecuteAssignedShiftNotification")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve un Datatable con todos las notificaciones
        ''' </summary>
        ''' <param name="_SQLFilter">Filtro SQL para el Where (ejemplo: 'NotificationType = 1 And Reque...')</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function GetNotifications(ByVal _SQLFilter As String, ByVal _State As roNotificationState,
                                                Optional ByVal bAudit As Boolean = False, Optional ByVal bolIncludeSystem As Boolean = False) As Generic.List(Of roNotification)

            Dim oRet As New Generic.List(Of roNotification)

            Try
                Dim isJune6610 As Boolean = VTBase.roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "June6610"))
                Dim strSQL As String = "@SELECT# * from Notifications"
                If _SQLFilter <> String.Empty Then strSQL &= " Where " & _SQLFilter
                If Not bolIncludeSystem Then
                    If _SQLFilter <> String.Empty Then
                        strSQL &= " AND "
                    Else
                        strSQL &= " WHERE "
                    End If
                    If Not isJune6610 Then
                        strSQL &= "ISNULL(CreatorID,'') <> 'SYSTEM' AND IDType NOT IN (83, 84, 72)"
                    Else
                        strSQL &= "ISNULL(CreatorID,'') <> 'SYSTEM'"
                    End If

                End If

                Dim dTbl As DataTable = CreateDataTable(strSQL, )

                If dTbl IsNot Nothing Then
                    For Each dRow As DataRow In dTbl.Rows
                        Dim oNotification As New roNotification(dRow("ID"), _State, False)
                        oRet.Add(oNotification)
                    Next
                End If

                If oRet.Count > 0 AndAlso bAudit Then
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Dim strAuditName As String = String.Empty
                    For Each oNotification As roNotification In oRet
                        strAuditName &= IIf(strAuditName <> String.Empty, ",", String.Empty) & oNotification.Name
                    Next
                    Extensions.roAudit.AddParameter(tbAuditParameters, "{NotificationNames}", strAuditName, String.Empty, 1)
                    _State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tNotification, strAuditName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roNotification::GetNotifications")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roNotification::GetNotifications")

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve un Dataset con todos los tipos de notificaciones
        ''' </summary>
        Public Shared Function GetNotificationsTypes(ByVal _State As roNotificationState,
                                                     Optional ByVal bolIncludeSystem As Boolean = False) As DataSet
            Dim dsRet As DataSet = Nothing

            Try
                Dim strSQL As String
                If Not bolIncludeSystem Then
                    strSQL = "@SELECT# ID, Name, Feature, OnlySystem, ISNULL(IDCategory,0) AS IDCategory FROM sysroNotificationTypes WHERE OnlySystem = 0"
                Else
                    strSQL = "@SELECT# ID, Name, Feature, OnlySystem , ISNULL(IDCategory,0) AS IDCategory FROM sysroNotificationTypes"
                End If

                dsRet = CreateDataSet(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roNotification::GetNotificationsTypes")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roNotification::GetNotificationsTypes")
            End Try

            Return dsRet

        End Function

        Public Shared Function LoadNotificationTypesWithCategories(ByRef oState As roSecurityCategoryState) As Generic.List(Of roNotificationType)
            Dim oRet As New Generic.List(Of roNotificationType)

            Try

                Dim bState = New roNotificationState(-1)
                Dim ds As DataSet = roNotification.GetNotificationsTypes(bState, False)

                Dim tb As DataTable = Nothing
                If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                    tb = ds.Tables(0)
                End If

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        Dim tmpNotificationType As New roNotificationType
                        tmpNotificationType.Id = oRow("ID")
                        tmpNotificationType.IDCategory = IIf(IsDBNull(oRow("IDCategory")), 6, oRow("IDCategory"))
                        tmpNotificationType.Name = oRow("Name")
                        tmpNotificationType.System = oRow("OnlySystem")
                        oRet.Add(tmpNotificationType)
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roSecurityCategorytManager::LoadNotificationTypesWithCategories")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityCategorytManager::LoadNotificationTypesWithCategories")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve un dataset con el id y nombre de todas las notificaciones
        ''' </summary>
        ''' <param name="_SQLFilter">Filtro SQL para el Where (ejemplo: 'NotificationType = 1 And Reque...')</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetNotificationList(ByVal _SQLFilter As String, ByVal _State As roNotificationState) As DataSet

            Dim dsRet As DataSet = Nothing

            Try

                Dim isJune6610 As Boolean = VTBase.roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "June6610"))

                Dim strSQL As String = "@SELECT# ID, Name from Notifications"
                If _SQLFilter <> String.Empty Then strSQL &= " Where " & _SQLFilter

                If _SQLFilter <> String.Empty Then
                    strSQL &= " AND "
                Else
                    strSQL &= " WHERE "
                End If

                If Not isJune6610 Then
                    strSQL &= "ISNULL(CreatorID,'') <> 'SYSTEM' AND IDType NOT IN (83, 84, 72)"
                Else
                    strSQL &= "ISNULL(CreatorID,'') <> 'SYSTEM'"
                End If



                strSQL &= " ORDER BY Name ASC"

                dsRet = CreateDataSet(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roNotification::GetNotificationList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roNotification::GetNotificationList")
            End Try

            Return dsRet

        End Function

        ''' <summary>
        ''' Devuelve un array con los tipos de notificaciones y los permisos sobre cada una en base a las funcionalidades del passport
        ''' </summary>
        ''' <param name="intIDPassport"></param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function GetPermissionOverNotifications(ByVal intIDPassport As Integer, ByRef oState As roNotificationState) As DataTable

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# sysroNotificationTypes.ID AS IDNotification, 0 AS Available, sysroNotificationTypes.Name " &
                         "FROM sysroNotificationTypes "
                strSQL &= "WHERE sysroNotificationTypes.ID NOT IN(9,10,12,15,13,14,27,28,32,33,35, 36,37,38,39,40)"
                strSQL &= " Order By sysroNotificationTypes.ID "

                tb = CreateDataTable(strSQL)
                For Each oRow As DataRow In tb.Rows
                    oRow("Available") = IsNotificationAvailable(oRow("IDNotification"), intIDPassport, oState)
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetPermissionOverNotifications")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetPermissionOverNotifications")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve true o false si el grupo de usuarios puede ver un tipo de notificacion
        ''' </summary>
        ''' <param name="intIDNotificationType"></param>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function IsNotificationAvailable(ByVal intIDNotificationType As Integer, ByVal intIDParentPassport As Integer, ByRef oState As roNotificationState) As Boolean
            Dim oRet As Boolean = False

            Try

                Dim strFeatureAlias As String = Any2String(ExecuteScalar("@SELECT# isnull(Feature, '') as Feature  FROM sysroNotificationTypes WHERE ID = " & intIDNotificationType))

                If strFeatureAlias.Length > 0 Then
                    Dim Result As Permission = WLHelper.GetPermissionOverFeature(intIDParentPassport, strFeatureAlias, "U")
                    If Result >= Permission.Write Then
                        oRet = True
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::IsNotificationAvailable")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::IsNotificationAvailable")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve un array con los tipos de notificaciones y el supervisor directo que debe atenderlas
        ''' </summary>
        ''' <param name="intIDPassport"></param>
        ''' <param name="intIDEmployee"></param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function GetNotificationsSupervisor(ByVal intIDPassport As Integer, ByVal intIDEmployee As Integer, ByRef oState As roNotificationState) As DataTable
            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String

                strSQL = $"@SELECT# sysroNotificationTypes.ID as ID, sysroNotificationTypes.Name as Name ,  1 as TypeFeature 
                           FROM sysroNotificationTypes
                           WHERE sysroNotificationTypes.ID Not In (9,10,12,15,13,14,22,27,28,32,33,35,36,37,38,39,40,66,76) and FeatureType = 'U' AND Onlysystem=0 
                           UNION 
                           @SELECT# sysroRequestType.IDType AS ID , sysroRequestType.Type as Name, 2 as TypeFeature 
                           FROM sysroRequestType where idtype not in (7,9,13,14) 
                           ORDER BY TypeFeature, ID "

                tb = CreateDataTable(strSQL, )
                tb.Columns.Add("SupervisorName", GetType(String))
                For Each oRow As DataRow In tb.Rows

                    oRow("SupervisorName") = GetNotificationSupervisorByEmployee(oRow("ID"), intIDEmployee, oRow("TypeFeature"), oState)

                    If oRow("TypeFeature") = 1 Then
                        oRow("Name") = oState.Language.Translate("NotifyType." & CType(oRow("ID"), eNotificationType).ToString & ".Name", String.Empty)
                    Else
                        oRow("Name") = oState.Language.Translate("RequestType." & CType(oRow("ID"), eRequestType).ToString & ".Name", String.Empty)
                    End If
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetNotificationsSupervisor")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetNotificationsSupervisor")
            End Try

            Return tb
        End Function

        Public Shared Function GetNotificationSupervisorByEmployee(ByVal ID As Integer, ByVal intIDEmployee As Integer, ByVal IDTypeFeature As Integer, ByRef oState As roNotificationState) As String
            Dim oRet As String = String.Empty

            Try

                Dim strSQL As String = String.Empty
                Dim strPassports As String = "-1"

                If IDTypeFeature = 1 Then
                    ' Notificaciones
                    strSQL = $"@SELECT# IdPassport FROM sysrofnSecurity_GeneralNotificationsDependencies({ID},{intIDEmployee}) WHERE ShouldBeNotified = 1 "
                    Dim tbCheckSecurity As DataTable = CreateDataTable(strSQL, )
                    If tbCheckSecurity.Rows.Count > 0 Then
                        For Each oRow As DataRow In tbCheckSecurity.Rows
                            ' si está vacío no la añadimos al array
                            If oRow(0).ToString.Length > 0 Then
                                strPassports = strPassports & "," & oRow(0)
                            End If
                        Next
                        tbCheckSecurity = CreateDataTable("@SELECT# isnull(sysroPassports.Name, '')  FROM sysroPassports WHERE ID IN(" & strPassports & ")", )
                        For Each oRow As DataRow In tbCheckSecurity.Rows
                            If oRet.Length = 0 Then
                                oRet = oRow(0)
                            Else
                                oRet = oRet & ", " & oRow(0)
                            End If
                        Next
                    End If
                Else
                    ' Solicitudes
                    strSQL = $" @SELECT# IdPassport from dbo.sysrofnSecurity_PermissionOverRequestTypes({ID},{intIDEmployee}) "
                    Dim tbCheckSecurity As DataTable = CreateDataTable(strSQL, )
                    For Each oRow As DataRow In tbCheckSecurity.Rows
                        strPassports &= "," & oRow("IdPassport")
                    Next
                    strPassports = strPassports.TrimEnd(CChar(","))
                    tbCheckSecurity = CreateDataTable("@SELECT# isnull(sysroPassports.Name, '')  FROM sysroPassports WHERE ID IN(" & strPassports & ")", )
                    For Each oRow As DataRow In tbCheckSecurity.Rows
                        If oRet.Length = 0 Then
                            oRet = oRow(0)
                        Else
                            oRet &= ", " & oRow(0)
                        End If
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetNotificationSupervisorByEmployee")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetNotificationSupervisorByEmployee")
            End Try

            Return oRet

        End Function

        Public Shared Function NotificationTaskExistsByType(iNotificationType As eNotificationType, ByRef oState As roNotificationState, Optional iKey1Numeric As Nullable(Of Integer) = Nothing, Optional iKey2Numeric As Nullable(Of Integer) = Nothing, Optional dKey3DateTime As Nullable(Of DateTime) = Nothing, Optional dKey4DateTime As Nullable(Of DateTime) = Nothing) As Generic.List(Of Integer)
            Dim oRet As New Generic.List(Of Integer)

            Try
                Dim sSQL As String

                sSQL = "@SELECT# sysroNotificationTasks.ID from sysroNotificationTasks, Notifications " &
                       " where  sysroNotificationTasks.IDNotification = Notifications.ID " &
                       " AND Notifications.IDType = " & iNotificationType

                If iKey1Numeric.HasValue Then sSQL = sSQL & " AND sysroNotificationTasks.Key1Numeric = " & iKey1Numeric.ToString
                If iKey2Numeric.HasValue Then sSQL = sSQL & " AND sysroNotificationTasks.Key2Numeric = " & iKey2Numeric.ToString
                If dKey3DateTime.HasValue Then sSQL = sSQL & " AND sysroNotificationTasks.Key3DateTime = " & roTypes.Any2Time(dKey3DateTime.Value).SQLSmallDateTime
                If dKey4DateTime.HasValue Then sSQL = sSQL & " AND sysroNotificationTasks.Key4DateTime = " & roTypes.Any2Time(dKey4DateTime.Value).SQLSmallDateTime

                Dim tbNT As DataTable = CreateDataTable(sSQL)

                If tbNT IsNot Nothing AndAlso tbNT.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbNT.Rows
                        oRet.Add(roTypes.Any2Integer(oRow("ID")))
                    Next
                End If
            Catch ex As Exception
                If oState IsNot Nothing Then oState.UpdateStateInfo(ex, "roNotification::NotificationTaskExistsByType Generic.List(Of Integer)")
            End Try
            Return oRet
        End Function

        Public Shared Function NotificationTaskExistsByType(ByVal iNotificationType As eNotificationType, ByRef iIDNotificationTask As Integer, ByRef oState As roNotificationState, Optional iKey1Numeric As Nullable(Of Integer) = Nothing, Optional iKey2Numeric As Nullable(Of Integer) = Nothing, Optional dKey3DateTime As Nullable(Of DateTime) = Nothing, Optional dKey4DateTime As Nullable(Of DateTime) = Nothing) As Boolean
            Dim bRet As Boolean = False
            Try

                Dim sSQL As String

                sSQL = "@SELECT# sysroNotificationTasks.ID from sysroNotificationTasks, Notifications " &
                       " where  sysroNotificationTasks.IDNotification = Notifications.ID " &
                       " AND Notifications.IDType = " & iNotificationType

                If iKey1Numeric.HasValue Then sSQL = sSQL & " AND sysroNotificationTasks.Key1Numeric = " & iKey1Numeric.ToString
                If iKey2Numeric.HasValue Then sSQL = sSQL & " AND sysroNotificationTasks.Key2Numeric = " & iKey2Numeric.ToString
                If dKey3DateTime.HasValue Then sSQL = sSQL & " AND sysroNotificationTasks.Key3DateTime = " & roTypes.Any2Time(dKey3DateTime.Value).SQLSmallDateTime
                If dKey4DateTime.HasValue Then sSQL = sSQL & " AND sysroNotificationTasks.Key4DateTime = " & roTypes.Any2Time(dKey4DateTime.Value).SQLSmallDateTime

                iIDNotificationTask = Any2Integer(ExecuteScalar(sSQL))
                bRet = (iIDNotificationTask <> 0)
            Catch ex As Exception
                If oState IsNot Nothing Then oState.UpdateStateInfo(ex, "roNotification::GetFeatureSupervisor")
            End Try
            Return bRet
        End Function

        ''' <summary>
        ''' Elimina una tarea de notifcación
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteTask(iID As Integer, ByRef oState As roNotificationState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Recojo el tipo de notificación antes de borrarla
                Dim IDType As Integer
                Dim strSQL As String = "@SELECT# idnotification from sysroNotificationTasks WHERE ID = " & iID.ToString
                IDType = roTypes.Any2Integer(ExecuteScalar(strSQL))

                ' Borro la tarea
                strSQL = "@DELETE# sysroNotificationTasks WHERE ID = " & iID.ToString
                bolRet = ExecuteSql(strSQL)

                If bolRet Then
                    'Actualizo fecha de modificación en la definición del tipo de notificación
                    strSQL = "@UPDATE# Notifications SET LastTaskDeleted = " & roTypes.Any2Time(Now).SQLDateTime & " WHERE ID = " & IDType & " AND CreatorID = 'SYSTEM' AND ShowOnDesktop = 1"
                    ExecuteSql(strSQL)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::DeleteTask")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::DeleteTask")
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Crea una tarea de notifcación
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function GenerateNotificationTask(ByVal OnlySystem As Boolean, ByVal IDType As eNotificationType, ByRef oState As roNotificationState, Optional iKey1Numeric As Nullable(Of Integer) = Nothing, Optional iKey2Numeric As Nullable(Of Integer) = Nothing, Optional dKey3DateTime As Nullable(Of DateTime) = Nothing, Optional dKey4DateTime As Nullable(Of DateTime) = Nothing, Optional sParameters As String = "#", Optional dFiredDate As Nullable(Of DateTime) = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim lstNotifications As Generic.List(Of Integer) = NotificationTaskExistsByType(IDType, oState, iKey1Numeric, iKey2Numeric, dKey3DateTime, dKey4DateTime)

                If lstNotifications.Count = 0 Then
                    ' Es nueva

                    ' Selecciona las notificaciones Request_Pending definidas por usuario y activadas
                    Dim tbNS As New DataTable("Notifications")
                    Dim strSQL As String
                    If OnlySystem Then
                        strSQL = "@SELECT# id, CreatorId from Notifications where idtype=" & IDType & " AND CreatorId='SYSTEM'"
                    Else
                        strSQL = "@SELECT# id, CreatorId from Notifications where idtype=" & IDType & " AND Activated = 1"
                    End If

                    tbNS = CreateDataTable(strSQL, "Notifications")

                    ' Lee la estructura de sysroNotificationTasks
                    Dim tbNT As New DataTable("sysroNotificationTasks")
                    strSQL = "@SELECT# * FROM sysroNotificationTasks where id=-1"
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tbNT)

                    For Each oRowNotifications As DataRow In tbNS.Rows
                        Dim oRow As DataRow
                        oRow = tbNT.NewRow
                        oRow("IDNotification") = oRowNotifications("id")
                        oRow("Executed") = 0
                        oRow("IsReaded") = 0
                        oRow("IdPassportReaded") = 0
                        oRow("InProgress") = 0

                        If iKey1Numeric.HasValue Then oRow("Key1Numeric") = iKey1Numeric
                        If iKey2Numeric.HasValue Then oRow("Key2Numeric") = iKey2Numeric
                        If dKey3DateTime.HasValue Then oRow("Key3DateTime") = dKey3DateTime
                        If dKey4DateTime.HasValue Then oRow("Key4DateTime") = dKey4DateTime

                        If dFiredDate.HasValue Then
                            oRow("FiredDate") = dFiredDate
                            oRow("NextDateTimeExecuted") = dFiredDate
                        Else
                            oRow("FiredDate") = DateTime.Now
                            oRow("NextDateTimeExecuted") = oRow("FiredDate")
                        End If

                        If sParameters <> "#" Then oRow("Parameters") = sParameters

                        tbNT.Rows.Add(oRow)
                        da.Update(tbNT)
                    Next
                Else
                    ' Es antigua y debemos volver a generarla.

                    For Each oNotificationTaskId As Integer In lstNotifications

                        Dim tbNT As New DataTable("sysroNotificationTasks")
                        Dim strSQL As String = "@SELECT# * FROM sysroNotificationTasks where id=" & oNotificationTaskId
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tbNT)

                        Dim oRow As DataRow = tbNT.Rows(0)
                        oRow("Executed") = 0
                        oRow("FiredDate") = DateTime.Now
                        oRow("InProgress") = 0

                        If sParameters <> "#" Then oRow("Parameters") = sParameters

                        da.Update(tbNT)
                    Next
                End If

                ' Si llego aquí se guardó ok
                bolRet = True
            Catch ex As DbException
                If oState IsNot Nothing Then oState.UpdateStateInfo(ex, "roNotification::CreateTask")
            Catch ex As Exception
                If oState IsNot Nothing Then oState.UpdateStateInfo(ex, "roNotification::CreateTask")
            End Try
            Return bolRet
        End Function

        Public Shared Function DeleteNotificationTask(ByVal IDType As eNotificationType, ByRef oState As roNotificationState, Optional iKey1Numeric As Nullable(Of Integer) = Nothing, Optional iKey2Numeric As Nullable(Of Integer) = Nothing, Optional dKey3DateTime As Nullable(Of DateTime) = Nothing, Optional dKey4DateTime As Nullable(Of DateTime) = Nothing, Optional sParameters As String = "#", Optional dFiredDate As Nullable(Of DateTime) = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim olog As New roLog("roNotification::DeleteSystemNotificationTask")

            Try

                ' Verifico si me han informado algo ...
                If False Then
                    oState.Result = NotificationResultEnum.NotEnoughInformation
                    Return False
                End If

                ' Borra todas las sysroNotificationTasks
                Dim strSQL As String = "@SELECT# sysroNotificationTasks.ID FROM sysroNotificationTasks, Notifications WHERE Notifications.ID = sysroNotificationTasks.IDNotification " &
                                       " AND Notifications.IDType = " & IDType
                If iKey1Numeric.HasValue Then strSQL &= $" AND sysroNotificationTasks.Key1Numeric = {iKey1Numeric}"
                If iKey2Numeric.HasValue Then strSQL &= $" AND sysroNotificationTasks.Key2Numeric = {iKey2Numeric}"
                If dKey3DateTime.HasValue Then strSQL &= $" AND sysroNotificationTasks.Key3DateTime = {dKey3DateTime}"
                If dKey4DateTime.HasValue Then strSQL &= $" AND sysroNotificationTasks.Key4DateTime = {dKey4DateTime}"

                bolRet = ExecuteSqlWithoutTimeOut("@DELETE# FROM sysroNotificationTasks WHERE ID in (" & strSQL & ")")

                ' Marco que se ha borrado una notificación del tipo en cuestión. Este valor lo usaremos para controlar el refresco de pantalla de alertas
                Try
                    strSQL = "@UPDATE# Notifications SET LastTaskDeleted = " & roTypes.Any2Time(Now).SQLDateTime & " WHERE IDType = " & IDType & " AND CreatorID = 'SYSTEM' AND ShowOnDesktop = 1"
                    ExecuteSql(strSQL)
                Catch ex As Exception
                    olog.logMessage(roLog.EventType.roError, "Error informing last task deleted timestamp. This could affect deskyop alerts screen accuracy", ex)
                End Try
            Catch ex As DbException
                If oState IsNot Nothing Then oState.UpdateStateInfo(ex, "roNotification::DeleteSystemNotificationTask")
            Catch ex As Exception
                If oState IsNot Nothing Then oState.UpdateStateInfo(ex, "roNotification::DeleteSystemNotificationTask")
            End Try
            Return bolRet
        End Function

        Public Shared Function GetSystemNotificationIDByType(iIDType As eNotificationType, ByRef oState As roNotificationState) As Integer
            Dim iRes As Integer = -1

            Try
                Dim sSQL As String = "@SELECT# ID FROM NOTIFICATIONS WHERE IDTYPE = " & Convert.ToInt32(iIDType).ToString() & " AND CREATORID = 'SYSTEM'"

                iRes = Any2Integer(ExecuteScalar(sSQL))
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetSystemNotificationIDByType")
            End Try

            Return iRes
        End Function

#Region "Employee Alerts"

        ''' <summary>
        ''' Devuelve las alertas a mostrar en DeskTop
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPortalAlerts(ByVal idemployee As Integer, ByRef oState As roNotificationState, ByVal dLastStatusChange As Nullable(Of DateTime), Optional ByVal bOnlyCheck As Boolean = False) As DTOs.EmployeeAlerts
            Dim oAlerts As New DTOs.EmployeeAlerts

            Dim dDateFrom As DateTime
            Dim dDateTo As DateTime = DateTime.Now.Date.AddDays(1)

            Try

                If (dLastStatusChange.HasValue) Then
                    dDateFrom = dLastStatusChange.Value
                Else
                    dDateFrom = DateTime.Now.Date.AddMonths(-1)
                End If

                '----------------------------- SOLICTUDES PENDIENTES DE GESTIONAR ------------------------------------
                oAlerts.RequestAlerts = GetPortalAlerts_PendingRequests(idemployee, dDateFrom, dDateTo, oState)
                If bOnlyCheck AndAlso oAlerts.RequestAlerts.Any() Then Return oAlerts

                '----------------------- EMPLEADOS PRESENTES SIN DOCUMENTACIÓN DE PRL EN REGLA -------------------------
                oAlerts.ExpiredDocAlert = GetPortalAlerts_EmployeesPresentWithExpiredDocs(idemployee, dDateFrom, dDateTo, False, oState)
                If bOnlyCheck AndAlso oAlerts.ExpiredDocAlert IsNot Nothing Then Return oAlerts

                '----------------------- FICHAJES IMPARES -------------------------
                oAlerts.IncompletePunches = GetPortalAlerts_EmployeesWithIncompletePunches(idemployee, dDateFrom, dDateTo, oState)
                If bOnlyCheck AndAlso oAlerts.IncompletePunches.Any() Then Return oAlerts

                '----------------------- DOCUMENTOS DE SEGUIMIENTO NO ENTREGADOS -------------------------
                ' oAlerts.ForecastDocuments = GetPortalAlerts_UndeliveredDocuments(idemployee, DocumentType.Employee, Nothing)

                '----------------------- DOCUMENTOS DE SEGUIMIENTO NO ENTREGADOS -------------------------
                oAlerts.Notifications = GetPortalAlerts_NotificationsNotReaded(idemployee, dDateFrom, dDateTo, oState)
                If bOnlyCheck AndAlso oAlerts.Notifications.Any() Then Return oAlerts

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetPortalAlerts")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetPortalAlerts")
            End Try

            Return oAlerts
        End Function

        ''' <summary>
        ''' SOLICTUDES PENDIENTES DE GESTIONAR
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPortalAlerts_PendingRequests(ByVal idEmployee As Integer, ByVal dDateFrom As DateTime, ByVal dDateTo As DateTime, ByRef oState As roNotificationState) As DTOs.RequestAlert()
            Dim oRequests As New Generic.List(Of DTOs.RequestAlert)

            Try
                Dim strSQL As String
                strSQL = "@SELECT# * from(" &
                            " @SELECT# r.RequestType, r.Id, r.Status, r.requestdate, r.Date1, r.Date2,MAX(ra.datetime) as Modified from requests r with (nolock) , RequestsApprovals ra with (nolock) " &
                            " where r.ID = ra.IDRequest AND r.IDEmployee = " & idEmployee & " And r.NotReaded = 1 AND r.RequestType <> 17" &
                            " Group by r.RequestType, r.ID,r.Status, r.RequestDate, r.Date1, r.Date2, ra.IDRequest) tmp " &
                            " where tmp.Modified > " & roTypes.Any2Time(dDateFrom).SQLDateTime &
                            " UNION " &
                            " @SELECT# r.RequestType, r.ID, r.Status, r.RequestDate, r.Date1, r.Date2, getdate() from Requests r where RequestType = 8 and IDEmployeeExchange = " & idEmployee &
                            " and ((r.Status = 0) OR (r.Status <> 0 and r.NotReaded = 1))"

                Dim tbAux As DataTable = CreateDataTable(strSQL, )
                If tbAux.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbAux.Rows
                        Dim oAlert As New DTOs.RequestAlert

                        oAlert.IdRequest = oRow("Id")
                        oAlert.IdRequestType = oRow("RequestType")
                        oAlert.Status = oRow("Status")
                        oAlert.DateTime = oRow("requestdate")

                        If oRow("requestdate") <= Now.AddDays(-3) Then
                            oAlert.IsUrgent = True
                        End If

                        If Not IsDBNull(oRow("Date1")) AndAlso roTypes.Any2DateTime(oRow("Date1")).Date >= Now.Date Then
                            oAlert.IsCritic = True
                        End If

                        oRequests.Add(oAlert)
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetNotificationsSupervisor")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetNotificationsSupervisor")
            End Try

            Return oRequests.ToArray()
        End Function

        ''' <summary>
        ''' Empleados presentes con PRL expirado
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPortalAlerts_EmployeesPresentWithExpiredDocs(ByVal idEmployee As Integer, ByVal dDateFrom As DateTime, ByVal dDateTo As DateTime, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DTOs.ExpiredDoc

            Dim oExpiredDoc As DTOs.ExpiredDoc = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# employees.name as empname, employeestatus.LastPunch, snt.id, snt.idnotification, noti.idtype, snty.name, snty.feature, snty.featuretype, Key1Numeric, Key2Numeric, Key3DateTime, Key4DateTime, Parameters,fireddate, '0' as exclude " &
                         "from sysroNotificationTasks snt with (nolock) , Notifications noti with (nolock), sysroNotificationTypes snty with (nolock) , employees with (nolock) , employeestatus with (nolock) " &
                         "where snt.idnotification = noti.ID " &
                         "and noti.IDType = snty.id " &
                         "and noti.showondesktop = 1 " &
                         "and snt.Key1Numeric = " & idEmployee & " and snt.Key1Numeric = employees.id  and snt.Key1Numeric = employeestatus.IDEmployee " &
                         "and noti.IDType = " & eNotificationType.Employee_Present_With_Expired_Documents & " " &
                         "and fireddate <= " & roTypes.Any2Time(dDateTo).SQLSmallDateTime & " " &
                         "and fireddate >= " & roTypes.Any2Time(dDateFrom).SQLSmallDateTime
                Dim tbAux As DataTable = CreateDataTable(strSQL, )

                If tbAux.Rows.Count > 0 Then
                    Dim bIsUrgent As Boolean = False
                    Dim alert As DateTime = DateTime.Now

                    For Each oRow As DataRow In tbAux.Rows

                        If alert > oRow("LastPunch") Then
                            alert = oRow("LastPunch")
                        End If
                        If Not bIsUrgent And oRow("LastPunch") <= Now.AddHours(-2) Then
                            bIsUrgent = True
                        End If
                    Next

                    oExpiredDoc = New DTOs.ExpiredDoc
                    oExpiredDoc.DateTime = alert
                    oExpiredDoc.IsUrgent = bIsUrgent
                    oExpiredDoc.IsCritic = bIsUrgent
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetPortalAlerts_EmployeesPresentWithExpiredDocs")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetPortalAlerts_EmployeesPresentWithExpiredDocs")
            End Try

            'Return tbRet
            Return oExpiredDoc
        End Function

        '' <summary>
        '' Documentos obligatorios que faltan
        '' </summary>
        '' <param name="idRelatedObject">Identificador del objeto empresa/empleado</param>
        '' <param name="type">Tipo del objecto que consultamos</param>
        '' <param name="oActiveTransaction"></param>
        '' <returns></returns>
        '' <remarks></remarks>
        Public Shared Function GetPortalAlerts_UndeliveredDocuments(ByVal idRelatedObject As Integer, ByVal type As DocumentType, Optional dOnDate As DateTime = Nothing) As DocumentAlert()

            Dim oMandatoryDocs As New Generic.List(Of DocumentAlert)
            Dim strQuery As String

            Dim ePassport As roPassportTicket = roPassportManager.GetPassportTicket(idRelatedObject, LoadType.Employee)
            Dim oState As New roNotificationState(ePassport.ID)

            Try

                ' Calculo días de offset para hacer validaciones de validez a futuro (para alertas con previsión)
                Dim dDaysOffset As Integer = 0
                If dOnDate.Year > 1 Then
                    dDaysOffset = dOnDate.Date.Subtract(Now.Date).Days
                End If

                ' DOCUMENTOS INCONDICIONALMENTE OBLIGATORIOS

                strQuery = "@SELECT# doct.id templateid, " & idRelatedObject.ToString & " idemp, doc.IdCompany idComp, NULL absenceid, doct.scope, NULL absencetype, NULL begindate, NULL enddate, NULL starthour, null endhour, null idcause " &
                            "From DocumentTemplates doct "
                strQuery = strQuery &
                                    "Left outer join (@SELECT# row_number() over (partition by IdEmployee, IdDocumentTemplate order by deliverydate desc) As 'RowNumber', * from documents where Status not in (" & DocumentStatus.Rejected & "," & DocumentStatus.Invalidated & "," & DocumentStatus.Expired & ")) doc on doc.IdDocumentTemplate = doct.id And doc.IDEmployee = " & idRelatedObject.ToString & " " & 'plantillas con documentos entregados, contando un documento por plantilla
                                    "where (DateAdd(Day, " & dDaysOffset & ", DateAdd(Day, DateDiff(Day, 0, GETDATE()), 0)) between doct.beginvalidity And doct.endvalidity) " & ' plantilla exigible por su fecha de validez '"where (getdate() between doct.beginvalidity And doct.endvalidity) " &
                                    "And (doc.RowNumber Is null And doct.Compulsory = 1) " & 'plantilla obligatorias de las que no hay ningún doc entregado
                                    "And doct.scope IN (" & DocumentScope.EmployeeContract & "," & DocumentScope.EmployeeField & ") "

                If idRelatedObject <= 0 Then strQuery = strQuery & " order by idemp asc"

                Dim tbAux As DataTable = CreateDataTable(strQuery)

                If tbAux.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbAux.Rows
                        Dim oAlert As New DocumentAlert
                        oAlert.IDDocument = oRow("templateid") ' Lo dejo informado por compatibilidad con versiones iniciales en las que no existía la propierada IDDocumentTemplate
                        oAlert.IDDocumentTemplate = oRow("templateid")
                        oAlert.DateTime = Now

                        oAlert.IdRelatedObject = If(IsDBNull(oRow("idemp")), idRelatedObject, oRow("idemp"))
                        oAlert.ObjectName = If(oAlert.IdRelatedObject > 0, ExecuteScalar("@SELECT# ISNULL(Name,'') FROM Employees WHERE ID=" & oAlert.IdRelatedObject), String.Empty)

                        'Mensaje para un empleado o empresa. No hace falta poner el nombre ....
                        oState.Language.ClearUserTokens()
                        oAlert.Description = oState.Language.Translate("roDocumentManager.Document.Undelivered", String.Empty)
                        oState.Language.ClearUserTokens()
                        oAlert.IsUrgent = True
                        oAlert.IsCritic = True

                        oMandatoryDocs.Add(oAlert)
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification:: GetDocumentationFaultAlerts_UndeliveredDocuments")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDocumentationFaultAlerts_UndeliveredDocuments")
            End Try

            Return oMandatoryDocs.ToArray
        End Function

        ''' <summary>
        ''' Días con fichajes impares
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPortalAlerts_EmployeesWithIncompletePunches(ByVal idEmployee As Integer, ByVal dDateFrom As DateTime, ByVal dDateTo As DateTime, ByRef oState As roNotificationState) As DTOs.IncompletePunch()

            Dim oIncompletePunches As New Generic.List(Of DTOs.IncompletePunch)

            Try
                Dim strSQL As String = "@SELECT# snt.id, snt.idnotification, noti.idtype, snty.name, snty.feature, snty.featuretype, Key1Numeric, Key2Numeric, Key3DateTime, Key4DateTime, Parameters,fireddate, '0' as exclude " &
                         "from sysroNotificationTasks snt with (nolock), Notifications noti with (nolock) , sysroNotificationTypes snty with (nolock) " &
                         "where snt.idnotification = noti.ID " &
                         "and noti.IDType = snty.id " &
                         "and noti.showondesktop = 1 " &
                         "and snt.Key1Numeric = " & idEmployee & " " &
                         "and noti.IDType = " & eNotificationType.Day_With_Unmatched_Time_Record & " " &
                         "and snt.key3datetime >= " & roTypes.Any2Time(dDateFrom).SQLSmallDateTime & " " &
                         "and snt.key3datetime <= " & roTypes.Any2Time(dDateTo).SQLSmallDateTime

                Dim tbAux As DataTable = CreateDataTable(strSQL, )

                If tbAux.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbAux.Rows
                        Dim oIPunch As New DTOs.IncompletePunch
                        oIPunch.DateTime = oRow("key3datetime")
                        'Urgente si hay incompletos con antiguedad superior a 7 días
                        If oRow("key3datetime") <= Now.AddDays(-7) Then
                            oIPunch.IsUrgent = True
                        End If
                        If roTypes.Any2DateTime(oRow("key3datetime")).Date = Now.Date Then
                            oIPunch.IsCritic = True
                        End If

                        oIncompletePunches.Add(oIPunch)
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetPortalAlerts_EmployeesWithIncompletePunches")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetPortalAlerts_EmployeesWithIncompletePunches")
            End Try

            Return oIncompletePunches.ToArray
        End Function

        ''' <summary>
        ''' Documentos de seguimiento no entregados
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPortalAlerts_NotificationsNotReaded(ByVal idEmployee As Integer, ByVal dDateFrom As DateTime, ByVal dDateTo As DateTime, ByRef oState As roNotificationState) As DTOs.EmployeeNotification()

            Dim oEmployeeNotifications As New Generic.List(Of DTOs.EmployeeNotification)

            Try

                Dim oAssignedShiftNotifications As Generic.List(Of roNotification) = roNotification.GetNotifications("IDType = 51 And Activated=1 And AllowVTPortal = 1", oState,, True)

                Dim strSQL As String = "@SELECT# snt.ID AS IdNotTask, key1Numeric, Key2Numeric, Key3DateTime, fireddate, noti.IDType As NotificationType, Parameters, snt.IDNotification, snt.Key5Numeric " &
                         "from sysroNotificationTasks snt, Notifications noti, sysroNotificationTypes snty  " &
                         "where snt.idnotification = noti.ID " &
                         "and noti.IDType = snty.id " &
                         "and snt.IsReaded = 0 " &
                         "and snt.Key1Numeric = " & idEmployee & " " &
                         "and noti.IDType IN (" & eNotificationType.Assign_Shift & "," & eNotificationType.Document_Validation_Action & "," & eNotificationType.Telecommuting_Change_For_Employee & " ) " &
                         "and fireddate <= " & roTypes.Any2Time(dDateTo).SQLSmallDateTime & " " &
                         "and fireddate >= " & roTypes.Any2Time(dDateFrom).SQLSmallDateTime

                Dim tbAux As DataTable = CreateDataTable(strSQL, )

                If tbAux.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbAux.Rows
                        Dim bInsert As Boolean = False
                        Dim oNotification As New DTOs.EmployeeNotification
                        oNotification.IdNotificationType = roTypes.Any2Integer(oRow("NotificationType"))

                        If oNotification.IdNotificationType = eNotificationType.Assign_Shift AndAlso oAssignedShiftNotifications.Find(Function(x) x.ID = roTypes.Any2Integer(oRow("IDNotification"))) IsNot Nothing Then
                            bInsert = True
                        ElseIf oNotification.IdNotificationType <> eNotificationType.Assign_Shift Then
                            bInsert = True
                        End If

                        If bInsert Then
                            oNotification.IdNotification = roTypes.Any2Integer(oRow("IdNotTask"))
                            oNotification.DateTime = roTypes.Any2DateTime(oRow("Key3DateTime"))

                            oNotification.Name = oState.Language.Translate("NotificationType." & roTypes.Any2String(oRow("NotificationType")) & ".Name", String.Empty)

                            Select Case oNotification.IdNotificationType
                                Case eNotificationType.Assign_Shift
                                    Dim strDate As String = Format(roTypes.Any2DateTime(oRow("Key3DateTime")), oState.Language.GetShortDateFormat())

                                    Dim strShiftSQL = "@SELECT# Name FROM Shifts WHERE ID = " & roTypes.Any2Integer(oRow("Key2Numeric").ToString)
                                    Dim sShiftName As String = roTypes.Any2String(ExecuteScalar(strShiftSQL))

                                    strShiftSQL = "@SELECT# Name FROM Shifts WHERE ID = " & roTypes.Any2Integer(oRow("Key5Numeric").ToString)
                                    Dim sOldShiftName As String = roTypes.Any2String(ExecuteScalar(strShiftSQL))

                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(sShiftName)
                                    oState.Language.AddUserToken(strDate)
                                    oState.Language.AddUserToken(sOldShiftName)
                                    oNotification.Description = oState.Language.Translate("NotificationType." & roTypes.Any2String(oRow("NotificationType")) & ".Description", String.Empty)
                                    oState.Language.ClearUserTokens()
                                Case eNotificationType.Document_Validation_Action
                                    oNotification.DateTime = roTypes.Any2DateTime(oRow("fireddate"))
                                    Dim strInfoSql As String = "@SELECT# Title, SupervisorRemarks from Documents where ID =" & roTypes.Any2Integer(oRow("key2Numeric"))
                                    Dim dtInfo As DataTable = CreateDataTable(strInfoSql)
                                    If dtInfo IsNot Nothing AndAlso dtInfo.Rows.Count > 0 Then
                                        oState.Language.ClearUserTokens()
                                        oState.Language.AddUserToken(dtInfo.Rows(0)("Title"))
                                        oState.Language.AddUserToken(dtInfo.Rows(0)("SupervisorRemarks"))
                                        oNotification.Description = oState.Language.Translate("NotificationType." & roTypes.Any2String(oRow("NotificationType")) & "." & roTypes.Any2String(oRow("Parameters")) & ".Description", String.Empty)
                                        oState.Language.ClearUserTokens()
                                    End If
                                Case eNotificationType.Telecommuting_Change_For_Employee
                                    oNotification.DateTime = roTypes.Any2DateTime(oRow("fireddate"))
                                    Dim sChange As String = String.Empty
                                    sChange = oRow("Parameters").ToString.Replace("_", String.Empty)
                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(Format(roTypes.Any2DateTime(oRow("Key3DateTime")), oState.Language.GetShortDateFormat()))
                                    If sChange = TelecommutingTypeEnum._AtHome.ToString.Replace("_", String.Empty) Then
                                        oNotification.Name = oState.Language.Translate("NotificationType." & roTypes.Any2String(oRow("NotificationType")) & ".AtHome.Name", String.Empty)
                                        oNotification.Description = oState.Language.Translate("NotificationType." & roTypes.Any2String(oRow("NotificationType")) & ".AtHome.Description", String.Empty)
                                    Else
                                        oNotification.Name = oState.Language.Translate("NotificationType." & roTypes.Any2String(oRow("NotificationType")) & ".AtOffice.Name", String.Empty)
                                        oNotification.Description = oState.Language.Translate("NotificationType." & roTypes.Any2String(oRow("NotificationType")) & ".AtOffice.Description", String.Empty)
                                    End If
                                    oState.Language.ClearUserTokens()
                            End Select

                            oEmployeeNotifications.Add(oNotification)
                        End If
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetPortalAlerts_TrackingDocumentsNotDelivered")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetPortalAlerts_TrackingDocumentsNotDelivered")
            End Try

            Return oEmployeeNotifications.ToArray
        End Function

        ''' <summary>
        ''' Documentos de seguimiento no entregados
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPortalAlerts_TrackingDocumentsNotDelivered(ByVal idEmployee As Integer, ByVal dDateFrom As DateTime, ByVal dDateTo As DateTime, ByRef oState As roNotificationState) As DTOs.TrackingDocument()

            Dim oTrackingDocuments As New Generic.List(Of DTOs.TrackingDocument)

            Try

                Dim strSQL As String = "@SELECT# AbsenceTracking.TrackDay, DocumentsAbsences.Name, Key1Numeric, Key2Numeric, Key3DateTime, Key4DateTime, Parameters,fireddate, '0' as exclude " &
                         "from sysroNotificationTasks snt, Notifications noti, sysroNotificationTypes snty, AbsenceTracking, DocumentsAbsences  " &
                         "where snt.idnotification = noti.ID " &
                         "and noti.IDType = snty.id " &
                         "and noti.showondesktop = 1 " &
                         "and snt.Key1Numeric = " & idEmployee & " and snt.Key1Numeric = AbsenceTracking.IDEmployee " &
                         "and noti.IDType = " & eNotificationType.Document_Not_Delivered & " " &
                         "and AbsenceTracking.ID = snt.Key2Numeric " &
                         "and DocumentsAbsences.ID = AbsenceTracking.IDDocument " &
                         "and fireddate <= " & roTypes.Any2Time(dDateTo).SQLSmallDateTime & " " &
                         "and fireddate >= " & roTypes.Any2Time(dDateFrom).SQLSmallDateTime

                Dim tbAux As DataTable = CreateDataTable(strSQL, )

                If tbAux.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbAux.Rows
                        Dim oDocument As New DTOs.TrackingDocument

                        oDocument.DateTime = oRow("TrackDay")
                        oDocument.Name = oRow("Name")
                        If oRow("TrackDay") <= Now.AddDays(-3) Then
                            oDocument.IsCritic = True
                        End If

                        If oRow("TrackDay") < Now Then
                            oDocument.IsUrgent = True
                        End If

                        oTrackingDocuments.Add(oDocument)
                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetPortalAlerts_TrackingDocumentsNotDelivered")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetPortalAlerts_TrackingDocumentsNotDelivered")
            End Try

            Return oTrackingDocuments.ToArray
        End Function

#End Region

#Region "Alertas"

        Public Shared Function GetDesktopAlertsForPortal(ByRef oState As roNotificationState, Optional dLastStatusChange As DateTime = Nothing, Optional bOnlyCheck As Boolean = False) As DTOs.SupervisorAlerts
            Dim dsAlerts As DataSet = GetDesktopAlerts(oState, dLastStatusChange,, bOnlyCheck, True)
            Dim oAlerts As New DTOs.SupervisorAlerts

            If oState.Result = NotificationResultEnum.NoError Then
                Dim tbAlerts As DataTable = dsAlerts.Tables(0)

                If tbAlerts IsNot Nothing AndAlso tbAlerts.Rows.Count > 0 Then
                    Dim oAlertsLst As New Generic.List(Of DTOs.SupervisorAlert)

                    For Each oRow As DataRow In tbAlerts.Rows
                        Dim oAlert As New DTOs.SupervisorAlert With {
                            .AlertType = roTypes.Any2Integer(oRow("IdAlertType")),
                            .Description = roTypes.Any2String(oRow("DesktopDescription")),
                            .IsUrgent = roTypes.Any2Boolean(oRow("IsUrgent")),
                            .IsCritic = roTypes.Any2Boolean(oRow("IsCritic")),
                            .DetailCount = roTypes.Any2Integer(oRow("DetailCount"))
                            }

                        oAlertsLst.Add(oAlert)
                    Next

                    oAlerts.Alerts = oAlertsLst.ToArray
                End If
            End If

            Return oAlerts
        End Function

        ''' <summary>
        ''' Devuelve las alertas a mostrar en DeskTop
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDesktopAlerts(ByRef oState As roNotificationState, Optional dLastStatusChange As DateTime = Nothing, Optional bLogStats As Boolean = False, Optional bOnlyCheck As Boolean = False, Optional bPortalRequest As Boolean = False) As DataSet
            Dim tbRes As DataTable = Nothing
            Dim tbAux As DataTable = Nothing
            Dim sResume As String = String.Empty
            Dim bIsUrgent As Boolean = False
            Dim bIsCritic As Boolean = False
            Dim dDateFrom As DateTime = Nothing
            Dim dDateTo As DateTime = Nothing
            Dim dicStats As New Dictionary(Of String, Double)
            Dim notificationRequired As Boolean = True

            Try
                tbRes = New DataTable
                tbRes.Columns.Add("IdAlertType", GetType(Integer))
                tbRes.Columns.Add("DesktopDescription", GetType(String))
                tbRes.Columns.Add("IsUrgent", GetType(Boolean))
                tbRes.Columns.Add("DateFrom", GetType(Date))
                tbRes.Columns.Add("DateTo", GetType(Date))
                tbRes.Columns.Add("IsCritic", GetType(Boolean))
                tbRes.Columns.Add("DetailCount", GetType(Integer))

                ' Stats ON
                Dim watch As Stopwatch = Nothing
                If Not bLogStats Then
                    Dim oParam As roAdvancedParameter
                    oParam = New roAdvancedParameter("AlertsStatsOn", New roAdvancedParameterState)
                    bLogStats = (roTypes.Any2String(oParam.Value) = "1")
                End If

                If bLogStats Then
                    'watch = New Stopwatch
                    watch = Stopwatch.StartNew
                End If

                'Si me pasan timestamp de cambios, miro si han habido cambios posteriores ...
                If Now.Subtract(dLastStatusChange).Days < 7 Then
                    Dim strSQL As String = String.Empty
                    strSQL = "@SELECT# count(*) " &
                                 "from sysroNotificationTasks snt " &
                                 "RIGHT OUTER JOIN Notifications noti  " &
                                 "on snt.idnotification = noti.ID " &
                                 "where noti.showondesktop = 1 and noti.Creatorid = 'SYSTEM' " &
                                 "and fireddate > " & roTypes.Any2Time(dLastStatusChange).SQLDateTime & " OR LastTaskDeleted > " & roTypes.Any2Time(dLastStatusChange).SQLDateTime
                    Dim iTotalNew As Integer = 0
                    iTotalNew = ExecuteScalar(strSQL)
                    ' Si no hay cambios, retorno nothing
                    If iTotalNew = 0 Then Return Nothing
                End If
                If bLogStats AndAlso watch IsNot Nothing Then
                    AddStat("CheckForChanges", dicStats, watch)
                End If

                '----------------------------- SOLICTUDES PENDIENTES DE GESTIONAR ------------------------------------
                tbAux = GetDesktopAlerts_PendingRequests(sResume, bIsUrgent, bIsCritic, False, oState)
                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim oResRow As DataRow = tbRes.NewRow
                    oResRow("IdAlertType") = eNotificationType.Request_Pending
                    oResRow("DesktopDescription") = sResume
                    oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                    oResRow("DateFrom") = DBNull.Value
                    oResRow("DateTo") = DBNull.Value
                    oResRow("IsCritic") = bIsCritic
                    oResRow("DetailCount") = tbAux.Rows.Count
                    tbRes.Rows.Add(oResRow)

                    If bOnlyCheck Then
                        Dim tmpDs As New DataSet()
                        tmpDs.Tables.Add(tbRes)
                        Return tmpDs
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("PendingRequests", dicStats, watch)
                '----------------------------- SOLICTUDES PENDIENTES DE GESTIONAR ------------------------------------

                '----------------------- EMPLEADOS PRESENTES SIN DOCUMENTACIÓN DE PRL EN REGLA -------------------------
                bIsUrgent = False
                tbAux = GetDesktopAlerts_EmployeesPresentWithExpiredDocs(sResume, bIsUrgent, bIsCritic, False, oState)
                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim oResRow As DataRow = tbRes.NewRow
                    oResRow("IdAlertType") = eNotificationType.Employee_Present_With_Expired_Documents
                    oResRow("DesktopDescription") = sResume
                    oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                    oResRow("DateFrom") = DBNull.Value
                    oResRow("DateTo") = DBNull.Value
                    oResRow("IsCritic") = bIsCritic
                    oResRow("DetailCount") = tbAux.Rows.Count
                    tbRes.Rows.Add(oResRow)


                    If bOnlyCheck Then
                        Dim tmpDs As New DataSet()
                        tmpDs.Tables.Add(tbRes)
                        Return tmpDs
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("EmployeesPresentWithExpiredDocs", dicStats, watch)
                '----------------------- EMPLEADOS PRESENTES SIN DOCUMENTACIÓN DE PRL EN REGLA -------------------------

                '----------------------- FICHAJES IMPARES -------------------------
                bIsUrgent = False
                tbAux = GetDesktopAlerts_EmployeesWithIncompletePunches(sResume, bIsUrgent, bIsCritic, dDateFrom, dDateTo, False, oState)
                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim oResRow As DataRow = tbRes.NewRow
                    oResRow("IdAlertType") = eNotificationType.Day_With_Unmatched_Time_Record
                    oResRow("DesktopDescription") = sResume
                    oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                    oResRow("DateFrom") = dDateFrom
                    oResRow("DateTo") = dDateTo
                    oResRow("IsCritic") = bIsCritic
                    oResRow("DetailCount") = tbAux.Rows.Count
                    tbRes.Rows.Add(oResRow)


                    If bOnlyCheck Then
                        Dim tmpDs As New DataSet()
                        tmpDs.Tables.Add(tbRes)
                        Return tmpDs
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("EmployeesWithIncompletePunches", dicStats, watch)
                '----------------------- FICHAJES IMPARES -------------------------

                '----------------------- EMPLEADOS QUE DEBERIAN ESTAR PRESENTES -------------------------
                bIsUrgent = False
                tbAux = GetDesktopAlerts_EmployeesThatShouldBePresent(sResume, bIsUrgent, bIsCritic, False, oState)
                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim oResRow As DataRow = tbRes.NewRow
                    oResRow("IdAlertType") = eNotificationType.Employee_Not_Arrived_or_Late
                    oResRow("DesktopDescription") = sResume
                    oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                    oResRow("DateFrom") = DBNull.Value
                    oResRow("DateTo") = DBNull.Value
                    oResRow("IsCritic") = bIsCritic
                    oResRow("DetailCount") = tbAux.Rows.Count
                    tbRes.Rows.Add(oResRow)


                    If bOnlyCheck Then
                        Dim tmpDs As New DataSet()
                        tmpDs.Tables.Add(tbRes)
                        Return tmpDs
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("EmployeesThatShouldBePresent", dicStats, watch)
                '----------------------- EMPLEADOS QUE DEBERIAN ESTAR PRESENTES -------------------------

                '----------------------- FICHAJES NO JUSTIFICADOS -------------------------
                bIsUrgent = False
                tbAux = GetDesktopAlerts_EmployeesWithNonJustifiedPunches(sResume, bIsUrgent, bIsCritic, dDateFrom, dDateTo, False, oState)
                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim oResRow As DataRow = tbRes.NewRow
                    oResRow("IdAlertType") = eNotificationType.Non_Justified_Incident
                    oResRow("DesktopDescription") = sResume
                    oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                    oResRow("DateFrom") = dDateFrom
                    oResRow("DateTo") = dDateTo
                    oResRow("IsCritic") = bIsCritic
                    oResRow("DetailCount") = tbAux.Rows.Count
                    tbRes.Rows.Add(oResRow)


                    If bOnlyCheck Then
                        Dim tmpDs As New DataSet()
                        tmpDs.Tables.Add(tbRes)
                        Return tmpDs
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("EmployeesWithNonJustifiedPunches", dicStats, watch)
                '----------------------- FICHAJES NO JUSTIFICADOS -------------------------

                '----------------------- FICHAJES NO FIABLES -------------------------
                bIsUrgent = False
                notificationRequired = (roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("VTPortal.NotifyUnreliablePunch", New AdvancedParameter.roAdvancedParameterState(oState.IDPassport)).Value).ToUpper <> "FALSE")
                If notificationRequired Then
                    tbAux = GetDesktopAlerts_EmployeesWithNonReliablePunches(sResume, bIsUrgent, bIsCritic, dDateFrom, dDateTo, False, oState)
                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim oResRow As DataRow = tbRes.NewRow
                        oResRow("IdAlertType") = eNotificationType.Day_with_Unreliable_Time_Record
                        oResRow("DesktopDescription") = sResume
                        oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                        oResRow("DateFrom") = dDateFrom
                        oResRow("DateTo") = dDateTo
                        oResRow("IsCritic") = bIsCritic
                        oResRow("DetailCount") = tbAux.Rows.Count
                        tbRes.Rows.Add(oResRow)


                        If bOnlyCheck Then
                            Dim tmpDs As New DataSet()
                            tmpDs.Tables.Add(tbRes)
                            Return tmpDs
                        End If
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("EmployeesWithNonReliablePunches", dicStats, watch)
                '----------------------- FICHAJES NO FIABLES -------------------------

                '----------------------- FICHAJES EN PERIODO DE CIERRE -------------------------
                If Not bPortalRequest Then
                    bIsUrgent = False
                    tbAux = GetDesktopAlerts_PunchesInLockDate(sResume, bIsUrgent, bIsCritic, dDateFrom, dDateTo, False, oState)
                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim oResRow As DataRow = tbRes.NewRow
                        oResRow("IdAlertType") = eNotificationType.Punches_In_LockDate
                        oResRow("DesktopDescription") = sResume
                        oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                        oResRow("DateFrom") = dDateFrom
                        oResRow("DateTo") = dDateTo
                        oResRow("IsCritic") = bIsCritic
                        oResRow("DetailCount") = tbAux.Rows.Count
                        tbRes.Rows.Add(oResRow)


                        If bOnlyCheck Then
                            Dim tmpDs As New DataSet()
                            tmpDs.Tables.Add(tbRes)
                            Return tmpDs
                        End If
                    End If
                    If bLogStats AndAlso watch IsNot Nothing Then AddStat("PunchesInLockDate", dicStats, watch)
                End If
                '----------------------- FICHAJES EN PERIODO DE CIERRE -------------------------

                '----------------------- Convenios Empleados que superan un saldo -------------------------
                If Not bPortalRequest Then
                    bIsUrgent = False
                    tbAux = GetDesktopAlerts_LabAgreeMaxAlert(sResume, bIsUrgent, bIsCritic, False, oState)
                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim oResRow As DataRow = tbRes.NewRow
                        oResRow("IdAlertType") = eNotificationType.LabAgree_Max_Exceeded
                        oResRow("DesktopDescription") = sResume
                        oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                        oResRow("DateFrom") = DBNull.Value
                        oResRow("DateTo") = DBNull.Value
                        oResRow("IsCritic") = bIsCritic
                        oResRow("DetailCount") = tbAux.Rows.Count
                        tbRes.Rows.Add(oResRow)


                        If bOnlyCheck Then
                            Dim tmpDs As New DataSet()
                            tmpDs.Tables.Add(tbRes)
                            Return tmpDs
                        End If
                    End If
                    If bLogStats AndAlso watch IsNot Nothing Then AddStat("LabAgreeMaxAlert", dicStats, watch)

                End If
                '----------------------- Convenios Empleados que superan un saldo -------------------------

                '----------------- Convenios Empleados que llegan al minimo de un saldo -------------------
                If Not bPortalRequest Then
                    bIsUrgent = False
                    tbAux = GetDesktopAlerts_LabAgreeMinAlert(sResume, bIsUrgent, bIsCritic, False, oState)
                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim oResRow As DataRow = tbRes.NewRow
                        oResRow("IdAlertType") = eNotificationType.LabAgree_Min_Reached
                        oResRow("DesktopDescription") = sResume
                        oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                        oResRow("DateFrom") = DBNull.Value
                        oResRow("DateTo") = DBNull.Value
                        oResRow("IsCritic") = bIsCritic
                        oResRow("DetailCount") = tbAux.Rows.Count
                        tbRes.Rows.Add(oResRow)


                        If bOnlyCheck Then
                            Dim tmpDs As New DataSet()
                            tmpDs.Tables.Add(tbRes)
                            Return tmpDs
                        End If
                    End If
                    If bLogStats AndAlso watch IsNot Nothing Then AddStat("LabAgreeMinAlert", dicStats, watch)
                End If
                '----------------- Convenios Empleados que llegan al minimo de un saldo -------------------

                '----------------- ProductiV: Tareas que empiezan hoy -------------------
                bIsUrgent = False
                tbAux = GetDesktopAlerts_TasksStartsToday(sResume, bIsUrgent, bIsCritic, False, oState)
                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim oResRow As DataRow = tbRes.NewRow
                    oResRow("IdAlertType") = eNotificationType.Task_Close_to_Start
                    oResRow("DesktopDescription") = sResume
                    oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                    oResRow("DateFrom") = DBNull.Value
                    oResRow("DateTo") = DBNull.Value
                    oResRow("IsCritic") = bIsCritic
                    oResRow("DetailCount") = tbAux.Rows.Count
                    tbRes.Rows.Add(oResRow)


                    If bOnlyCheck Then
                        Dim tmpDs As New DataSet()
                        tmpDs.Tables.Add(tbRes)
                        Return tmpDs
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("TasksStartsToday", dicStats, watch)
                '----------------- ProductiV: Tareas que empiezan hoy -------------------

                '----------------- ProductiV: Tareas que finalizan hoy -------------------
                bIsUrgent = False
                tbAux = GetDesktopAlerts_TasksEndsToday(sResume, bIsUrgent, bIsCritic, False, oState)
                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim oResRow As DataRow = tbRes.NewRow
                    oResRow("IdAlertType") = eNotificationType.Task_Close_to_Finish
                    oResRow("DesktopDescription") = sResume
                    oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                    oResRow("DateFrom") = DBNull.Value
                    oResRow("DateTo") = DBNull.Value
                    oResRow("IsCritic") = bIsCritic
                    oResRow("DetailCount") = tbAux.Rows.Count
                    tbRes.Rows.Add(oResRow)


                    If bOnlyCheck Then
                        Dim tmpDs As New DataSet()
                        tmpDs.Tables.Add(tbRes)
                        Return tmpDs
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("TasksEndsToday", dicStats, watch)
                '----------------- ProductiV: Tareas que finalizan hoy -------------------

                '----------------- ProductiV: Tareas que sobrepasan tiempo planificado -------------------
                bIsUrgent = False
                tbAux = GetDesktopAlerts_TasksExceededPlannedTime(sResume, bIsUrgent, bIsCritic, False, oState)
                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim oResRow As DataRow = tbRes.NewRow
                    oResRow("IdAlertType") = eNotificationType.Task_Exceeding_Planned_Time
                    oResRow("DesktopDescription") = sResume
                    oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                    oResRow("DateFrom") = DBNull.Value
                    oResRow("DateTo") = DBNull.Value
                    oResRow("IsCritic") = bIsCritic
                    oResRow("DetailCount") = tbAux.Rows.Count
                    tbRes.Rows.Add(oResRow)


                    If bOnlyCheck Then
                        Dim tmpDs As New DataSet()
                        tmpDs.Tables.Add(tbRes)
                        Return tmpDs
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("TasksExceededPlannedTime", dicStats, watch)
                '----------------- ProductiV: Tareas que sobrepasan tiempo planificado -------------------

                '----------------- ProductiV: Tareas que sobrepasan fecha inicio -------------------
                bIsUrgent = False
                tbAux = GetDesktopAlerts_TasksExceededStartDate(sResume, bIsUrgent, bIsCritic, False, oState)
                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim oResRow As DataRow = tbRes.NewRow
                    oResRow("IdAlertType") = eNotificationType.Task_exceeding_Started_Date
                    oResRow("DesktopDescription") = sResume
                    oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                    oResRow("DateFrom") = DBNull.Value
                    oResRow("DateTo") = DBNull.Value
                    oResRow("IsCritic") = bIsCritic
                    oResRow("DetailCount") = tbAux.Rows.Count
                    tbRes.Rows.Add(oResRow)


                    If bOnlyCheck Then
                        Dim tmpDs As New DataSet()
                        tmpDs.Tables.Add(tbRes)
                        Return tmpDs
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("TasksExceededStartDate", dicStats, watch)
                '----------------- ProductiV: Tareas que sobrepasan fecha inicio -------------------

                '----------------- ProductiV: Tareas que sobrepasan fecha fin -------------------
                bIsUrgent = False
                tbAux = GetDesktopAlerts_TasksExceededEndDate(sResume, bIsUrgent, bIsCritic, False, oState)
                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim oResRow As DataRow = tbRes.NewRow
                    oResRow("IdAlertType") = eNotificationType.Task_Exceeding_Finished_Date
                    oResRow("DesktopDescription") = sResume
                    oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                    oResRow("DateFrom") = DBNull.Value
                    oResRow("DateTo") = DBNull.Value
                    oResRow("IsCritic") = bIsCritic
                    oResRow("DetailCount") = tbAux.Rows.Count
                    tbRes.Rows.Add(oResRow)


                    If bOnlyCheck Then
                        Dim tmpDs As New DataSet()
                        tmpDs.Tables.Add(tbRes)
                        Return tmpDs
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("TasksExceededEndDate", dicStats, watch)
                '----------------- ProductiV: Tareas que sobrepasan fecha fin -------------------

                '----------------- ProductiV: Tareas con alertas pendientes -------------------
                bIsUrgent = False
                tbAux = GetDesktopAlerts_TasksWithAlerts(sResume, bIsUrgent, bIsCritic, False, oState)
                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim oResRow As DataRow = tbRes.NewRow
                    oResRow("IdAlertType") = eNotificationType.Task_With_ALerts
                    oResRow("DesktopDescription") = sResume
                    oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                    oResRow("DateFrom") = DBNull.Value
                    oResRow("DateTo") = DBNull.Value
                    oResRow("IsCritic") = bIsCritic
                    oResRow("DetailCount") = tbAux.Rows.Count
                    tbRes.Rows.Add(oResRow)


                    If bOnlyCheck Then
                        Dim tmpDs As New DataSet()
                        tmpDs.Tables.Add(tbRes)
                        Return tmpDs
                    End If
                End If
                If bLogStats AndAlso watch IsNot Nothing Then AddStat("TasksWithAlerts", dicStats, watch)
                '----------------- ProductiV: Tareas con alertas pendientes -------------------

                '----------------- ProductiV: Tareas pendientes de completar -------------------
                If Not bPortalRequest Then
                    bIsUrgent = False
                    tbAux = GetDesktopAlerts_TasksWaitingComplete(sResume, bIsUrgent, bIsCritic, False, oState)
                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim oResRow As DataRow = tbRes.NewRow
                        oResRow("IdAlertType") = eNotificationType.Tasks_Request_complete
                        oResRow("DesktopDescription") = sResume
                        oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                        oResRow("DateFrom") = DBNull.Value
                        oResRow("DateTo") = DBNull.Value
                        oResRow("IsCritic") = bIsCritic
                        oResRow("DetailCount") = tbAux.Rows.Count
                        tbRes.Rows.Add(oResRow)


                        If bOnlyCheck Then
                            Dim tmpDs As New DataSet()
                            tmpDs.Tables.Add(tbRes)
                            Return tmpDs
                        End If
                    End If
                    If bLogStats AndAlso watch IsNot Nothing Then AddStat("TasksWaitingComplete", dicStats, watch)
                End If

                '----------------- ProductiV: Tareas pendientes de completar -------------------

                '----------------- Seguridad: Departamentos Sin Supervisión -------------------
                If Not bPortalRequest Then
                    bIsUrgent = False
                    tbAux = GetDesktopAlerts_NonSupervisedDepartments(sResume, bIsUrgent, bIsCritic, False, oState)
                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim oResRow As DataRow = tbRes.NewRow
                        oResRow("IdAlertType") = eNotificationType.NonSupervisedDepartments
                        oResRow("DesktopDescription") = sResume
                        oResRow("IsUrgent") = bIsUrgent OrElse bIsCritic
                        oResRow("DateFrom") = DBNull.Value
                        oResRow("DateTo") = DBNull.Value
                        oResRow("IsCritic") = bIsCritic
                        oResRow("DetailCount") = tbAux.Rows.Count
                        tbRes.Rows.Add(oResRow)


                        If bOnlyCheck Then
                            Dim tmpDs As New DataSet()
                            tmpDs.Tables.Add(tbRes)
                            Return tmpDs
                        End If
                    End If
                    If bLogStats AndAlso watch IsNot Nothing Then AddStat("DepartmentsWithoutSupervisor", dicStats, watch)
                End If
                '----------------- Seguridad: Departamentos Sin Supervisión -------------------

                Try
                    If bLogStats Then roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotification::GetDesktopAlerts::Stats for passport " & oState.IDPassport.ToString & " -> Total: (" & dicStats.Sum(Function(pair) pair.Value) & ")" & vbCrLf & String.Join(vbCrLf, dicStats.ToArray))
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotification::GetDesktopAlerts::Unknown error", ex)
                End Try
                '----------------------- Scheduling: Cobertura insuficiente en UP -------------------------
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts")
            End Try

            Dim ds As New DataSet()
            ds.Tables.Add(tbRes)
            Return ds
        End Function

        ''' <summary>
        ''' Devuelve los detalles de un cierto tipo de alerta
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDesktopAlertsDetails(iType As Integer, ByRef oState As roNotificationState) As DataSet
            Dim tbAux As DataTable = Nothing
            Dim sResume As String = String.Empty
            Dim bIsUrgent As Boolean = False
            Dim bIsCritic As Boolean = False
            Dim dDateFrom As DateTime = Nothing
            Dim dDateTo As DateTime = Nothing

            Try

                Select Case iType
                    Case eNotificationType.Request_Pending
                        '----------------------------- SOLICTUDES PENDIENTES DE GESTIONAR ------------------------------------
                        tbAux = GetDesktopAlerts_PendingRequests(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.Employee_Present_With_Expired_Documents
                        '----------------------- EMPLEADOS PRESENTES SIN DOCUMENTACIÓN DE PRL EN REGLA -------------------------
                        tbAux = GetDesktopAlerts_EmployeesPresentWithExpiredDocs(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.Day_With_Unmatched_Time_Record
                        '----------------------- FICHAJES IMPARES -------------------------
                        tbAux = GetDesktopAlerts_EmployeesWithIncompletePunches(sResume, bIsUrgent, bIsCritic, dDateFrom, dDateTo, True, oState)

                    Case eNotificationType.Non_Justified_Incident
                        '----------------------- FICHAJES NO JUSTIFICADOS -------------------------
                        tbAux = GetDesktopAlerts_EmployeesWithNonJustifiedPunches(sResume, bIsUrgent, bIsCritic, dDateFrom, dDateTo, True, oState)

                    Case eNotificationType.Day_with_Unreliable_Time_Record
                        '----------------------- FICHAJES NO FIABLES -------------------------
                        Dim notificationRequired As Boolean = (roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("VTPortal.NotifyUnreliablePunch", New AdvancedParameter.roAdvancedParameterState(oState.IDPassport)).Value).ToUpper <> "FALSE")
                        If notificationRequired Then
                            tbAux = GetDesktopAlerts_EmployeesWithNonReliablePunches(sResume, bIsUrgent, bIsCritic, dDateFrom, dDateTo, True, oState)
                        End If
                    Case eNotificationType.Punches_In_LockDate
                        '----------------------- FICHAJES EN PERIODO DE CIERRE -------------------------
                        tbAux = GetDesktopAlerts_PunchesInLockDate(sResume, bIsUrgent, bIsCritic, dDateFrom, dDateTo, True, oState)

                    Case eNotificationType.Employee_Not_Arrived_or_Late
                        '----------------------- EMPLEADOS QUE DEBERIAN ESTAR PRESENTES -------------------------
                        tbAux = GetDesktopAlerts_EmployeesThatShouldBePresent(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.LabAgree_Max_Exceeded
                        '----------------------- Convenios maximo sobrepasado -------------------------
                        tbAux = GetDesktopAlerts_LabAgreeMaxAlert(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.LabAgree_Min_Reached
                        '----------------------- Convenios minimo alcanzado -------------------------
                        tbAux = GetDesktopAlerts_LabAgreeMinAlert(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.Task_Close_to_Start
                        '----------------------- ProductiV: Tareas que empiezan hoy -------------------------
                        tbAux = GetDesktopAlerts_TasksStartsToday(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.Task_Close_to_Finish
                        '----------------------- ProductiV: Tareas que finalizan hoy -------------------------
                        tbAux = GetDesktopAlerts_TasksEndsToday(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.Task_Exceeding_Planned_Time
                        '----------------------- ProductiV: Tareas que sobrepasan el tiempo planificado -------------------------
                        tbAux = GetDesktopAlerts_TasksExceededPlannedTime(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.Task_exceeding_Started_Date
                        '----------------------- ProductiV: Tareas que sobrepasan la fecha inico -------------------------
                        tbAux = GetDesktopAlerts_TasksExceededStartDate(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.Task_Exceeding_Finished_Date
                        '----------------------- ProductiV: Tareas que sobrepasan la fecha fin -------------------------
                        tbAux = GetDesktopAlerts_TasksExceededEndDate(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.Task_With_ALerts
                        '----------------------- Tareas con alertas -------------------------
                        tbAux = GetDesktopAlerts_TasksWithAlerts(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.Tasks_Request_complete
                        '----------------------- Tareas pendientes de completar -------------------------
                        tbAux = GetDesktopAlerts_TasksWaitingComplete(sResume, bIsUrgent, bIsCritic, True, oState)

                    Case eNotificationType.NonSupervisedDepartments
                        '----------------------- Departamentos sin supervisor -------------------------
                        tbAux = GetDesktopAlerts_NonSupervisedDepartments(sResume, bIsUrgent, bIsCritic, True, oState)
                End Select
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlertsDetails")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlertsDetails")
            End Try

            Dim ds As New DataSet()
            If tbAux Is Nothing Then
                tbAux = New DataTable
                tbAux.Columns.Add("ID", GetType(Integer))
                tbAux.Columns.Add("IDEmployee", GetType(Integer))
                tbAux.Columns.Add("Column1Detail", GetType(String))
                tbAux.Columns.Add("Column2Detail", GetType(String))
                tbAux.Columns.Add("Column3Detail", GetType(String))
            End If

            ds.Tables.Add(tbAux)
            Return ds
        End Function

#Region "Alertas de gestion horaria"

        ''' <summary>
        ''' SOLICTUDES PENDIENTES DE GESTIONAR
        ''' </summary>
        ''' <param name="sResume">Descripción a mostrar en pantalla de alerta</param>
        ''' <param name="bIsUrgent">Indica si hay que mostrar con alerta gráfica</param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDesktopAlerts_PendingRequests(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable = Nothing
            Dim tbRes As DataTable

            'Inicializamos a no critica la alerta
            bIsCritic = False
            bIsUrgent = False
            Try
                Dim strSQL As String = "if exists (@SELECT# distinct 1 from requests inner join sysrovwSecurity_PermissionOverEmployees poe on poe.IdEmployee = Requests.IDEmployee and poe.IdPassport = " & oState.IDPassport & " and convert(date,getdate()) between poe.BeginDate and poe.EndDate " &
                                        " where Status in (0,1) and RequestDate between dateadd(d,-30,getdate()) And getdate()) " &
                        " @SELECT# Requests.idemployee, requests.requestdate, requests.Date1, Requests.RequestType, Employees.Name as EmployeeName from Requests " &
                        " INNER JOIN sysrovwSecurity_PendingRequestsDependencies prd on prd.IdRequest = requests.ID and prd.IdPassport = " & oState.IDPassport & " and prd.DirectDependence = 1 and prd.RequestCurrentStatus in(0,1) " &
                        " INNER JOIN Employees on Employees.ID = Requests.IDEmployee " &
                        " WHERE requests.requestdate between dateadd(d,-30,getdate()) And getdate()"

                strSQL &= SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetDesktopAlerts_PendingRequests)

                tbAux = CreateDataTable(strSQL, )

                If Not bNeedDetails AndAlso tbAux IsNot Nothing Then
                    If tbAux.Rows.Count > 0 Then
                        Dim rowIndex As Integer = 0
                        Do While (Not bIsUrgent OrElse Not bIsCritic) AndAlso rowIndex < tbAux.Rows.Count
                            If tbAux.Rows(rowIndex)("requestdate") <= Now.AddDays(-3) Then bIsUrgent = True
                            If Not IsDBNull(tbAux.Rows(rowIndex)("Date1")) AndAlso roTypes.Any2DateTime(tbAux.Rows(rowIndex)("Date1")).Date >= Now.Date Then bIsCritic = True
                            rowIndex += 1
                        Loop
                    End If

                    oState.Language.ClearUserTokens()
                    oState.Language.AddUserToken(tbAux.Rows.Count)
                    If tbAux.Rows.Count > 1 Then
                        sResume = oState.Language.Translate("DesktopAlert.PendingRequests", String.Empty)
                    Else
                        sResume = oState.Language.Translate("DesktopAlert.PendingRequest", String.Empty)
                    End If
                Else
                    ' Creo tabla de resultado
                    tbRes = New DataTable
                    tbRes.Columns.Add("ID", GetType(Integer))
                    tbRes.Columns.Add("IDEmployee", GetType(Integer))
                    tbRes.Columns.Add("Column1Detail", GetType(String))
                    tbRes.Columns.Add("Column2Detail", GetType(String))
                    tbRes.Columns.Add("Column3Detail", GetType(String))

                    Dim oGrouppState As New roNotificationState
                    oGrouppState.IDPassport = oState.IDPassport

                    Dim ID As Integer = 0
                    If tbAux IsNot Nothing Then
                        For Each oRow As DataRow In tbAux.Rows
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("IDEmployee")
                            oResRow("Column1Detail") = oRow("EmployeeName")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(oState.Language.Translate("Request_type.NotificationDetails_" & roTypes.Any2String(oRow("RequestType")), String.Empty))
                            oState.Language.AddUserToken(roTypes.Any2DateTime(oRow("requestdate").ToShortTimeString))
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.EmployeesRequest.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next
                    End If

                    tbAux = tbRes
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetNotificationsSupervisor")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetNotificationsSupervisor")
            End Try

            Return tbAux
        End Function

        ''' <summary>
        ''' Empleados presentes con PRL expirado
        ''' </summary>
        ''' <param name="sResume">Descripción a mostrar en pantalla de alerta</param>
        ''' <param name="bIsUrgent">Indica si hay que mostrar con alerta gráfica</param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDesktopAlerts_EmployeesPresentWithExpiredDocs(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable = Nothing
            Dim tbRet As DataTable = Nothing
            Dim tbRes As DataTable = Nothing
            'Inicializamos a no critica la alerta 'Esta alerta siempre es crítica
            bIsCritic = True
            bIsUrgent = False
            Try
                Dim strSQL As String = " @SELECT# Employees.Id as idemployee, employees.name as empname, employeestatus.LastPunch " &
                            " from sysroNotificationTasks snt with (nolock) " &
                            " inner join Notifications noti with (nolock) on snt.IDNotification = noti.ID and noti.ShowOnDesktop = 1 and snt.fireddate between dateadd(d,-30,getdate()) And getdate() and noti.IDType= " & eNotificationType.Employee_Present_With_Expired_Documents & " " &
                            " inner join sysroNotificationTypes snty with (nolock) On snty.ID = noti.IDType " &
                            " inner join employees with (nolock) On Employees.ID = snt.Key1Numeric " &
                            " inner join employeestatus with (nolock) On employeestatus.IDEmployee = Employees.ID " &
                            " inner join sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = snty.FeatureType AND pof.FeatureAlias = snty.Feature AND pof.Permission > 3 " &
                            " inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport = " & oState.IDPassport.ToString & " And poe.IDEmployee = Employees.ID And CONVERT(DATE,GETDATE()) between poe.BeginDate And poe.EndDate "

                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing Then
                    If tbAux.Rows.Count > 0 AndAlso Not bNeedDetails Then
                        If tbAux IsNot Nothing Then
                            Dim rowIndex As Integer = 0
                            Do While Not bIsUrgent AndAlso rowIndex < tbAux.Rows.Count
                                If tbAux.Rows(rowIndex)("LastPunch") <= Now.AddHours(-2) Then bIsUrgent = True
                                rowIndex += 1
                            Loop

                            tbRes = tbAux
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(tbRes.Rows.Count)
                            If tbRes.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.EmployeesPresentWithoutPRL", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.EmployeePresentWithoutPRL", String.Empty)
                            End If
                        End If
                    Else

                        ' Creo tabla de resultado
                        tbRes = New DataTable
                        tbRes.Columns.Add("ID", GetType(Integer))
                        tbRes.Columns.Add("IDEmployee", GetType(Integer))
                        tbRes.Columns.Add("Column1Detail", GetType(String))
                        tbRes.Columns.Add("Column2Detail", GetType(String))
                        tbRes.Columns.Add("Column3Detail", GetType(String))

                        Dim oGrouppState As New roNotificationState
                        oGrouppState.IDPassport = oState.IDPassport
                        Dim ID As Integer = 0
                        If tbAux IsNot Nothing Then
                            For Each oRow As DataRow In tbAux.Rows
                                If Not bIsUrgent And oRow("LastPunch") <= Now.AddHours(-2) Then bIsUrgent = True
                                ' Añado registro al detalle
                                Dim oResRow As DataRow = tbRes.NewRow
                                oResRow("ID") = ID
                                ID += 1
                                oResRow("IDEmployee") = oRow("IDEmployee")
                                oResRow("Column1Detail") = oRow("empname")
                                oState.Language.ClearUserTokens()
                                oState.Language.AddUserToken(Any2DateTime(oRow("LastPunch")).Date)
                                oState.Language.AddUserToken(Any2DateTime(oRow("LastPunch")).ToShortTimeString)
                                oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.EmployeesPresentWithoutPRL.Detail", String.Empty)
                                tbRes.Rows.Add(oResRow)
                            Next

                            Dim dv As New DataView(tbAux)
                            tbRet = dv.ToTable

                            If tbRet.Rows.Count > 0 Then
                                oState.Language.ClearUserTokens()
                                oState.Language.AddUserToken(tbRet.Rows.Count)
                                If tbRet.Rows.Count > 1 Then
                                    sResume = oState.Language.Translate("DesktopAlert.EmployeesPresentWithoutPRL", String.Empty)
                                Else
                                    sResume = oState.Language.Translate("DesktopAlert.EmployeePresentWithoutPRL", String.Empty)
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_EmployeesPresentWithExpiredDocs")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_EmployeesPresentWithExpiredDocs")

            End Try

            Return tbRes
        End Function

        ''' <summary>
        ''' Fichajes de empleados en periodo de cierre
        ''' </summary>
        ''' <param name="sResume">Descripción a mostrar en pantalla de alerta</param>
        ''' <param name="bIsUrgent">Indica si hay que mostrar con alerta gráfica</param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDesktopAlerts_PunchesInLockDate(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByRef dDateFrom As DateTime, ByRef dDateTo As DateTime, ByVal bNeedDetail As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable = Nothing
            Dim tbRet As DataTable = Nothing
            Dim tbRes As DataTable = Nothing

            'Inicializamos a no critica la alerta
            bIsCritic = False
            bIsUrgent = False
            Try
                Dim strSQL As String = "@SELECT# emp.ID as idemployee, Key3DateTime, emp.name as EmployeeName " &
                          " from sysroNotificationTasks snt WITH (NOLOCK) " &
                          " inner join Notifications noti WITH (NOLOCK) on snt.IDNotification = noti.ID and noti.ShowOnDesktop = 1 and noti.IDType= " & eNotificationType.Punches_In_LockDate & " and snt.key3datetime between dateadd(d,-60,getdate()) and getdate() " &
                          " inner join sysroNotificationTypes snty  WITH (NOLOCK) on snty.ID = noti.IDType " &
                          " inner join Employees emp WITH (NOLOCK) ON emp.ID = snt.Key1Numeric " &
                          " inner join sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = snty.FeatureType AND pof.FeatureAlias = snty.Feature AND pof.Permission > 3 " &
                          " inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport = " & oState.IDPassport.ToString & " And poe.IDEmployee = emp.ID And CONVERT(DATE,GETDATE()) between poe.BeginDate And poe.EndDate "

                tbAux = CreateDataTable(strSQL, )

                If Not bNeedDetail Then
                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim rowIndex As Integer = 0
                        Do While Not bIsUrgent AndAlso rowIndex < tbAux.Rows.Count
                            If tbAux.Rows(rowIndex)("key3datetime") <= Now.AddDays(-7) Then bIsUrgent = True
                            rowIndex += 1
                        Loop

                        Dim dv As New DataView(tbAux)
                        tbRet = dv.ToTable

                        If tbRet.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(dv.Cast(Of DataRowView).GroupBy(Function(r) r("Key3DateTime")).ToList.Count)
                            oState.Language.AddUserToken(dv.Cast(Of DataRowView).GroupBy(Function(r) r("IdEmployee")).ToList.Count)
                            If tbRet.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.PunchesInLockDate", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.PuncheInLockDate", String.Empty)
                            End If
                            dDateFrom = dv.Cast(Of DataRowView).Min(Function(r) r("Key3DateTime"))
                            dDateTo = dv.Cast(Of DataRowView).Max(Function(r) r("Key3DateTime"))
                        End If
                    End If
                Else
                    ' Creo tabla de resultado
                    tbRes = New DataTable
                    tbRes.Columns.Add("ID", GetType(Integer))
                    tbRes.Columns.Add("IDEmployee", GetType(Integer))
                    tbRes.Columns.Add("Column1Detail", GetType(String))
                    tbRes.Columns.Add("Column2Detail", GetType(String))
                    tbRes.Columns.Add("Column3Detail", GetType(String))

                    Dim oGrouppState As New roNotificationState
                    oGrouppState.IDPassport = oState.IDPassport

                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim ID As Integer = 0
                        For Each oRow As DataRow In tbAux.Rows
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("IDEmployee")
                            oResRow("Column1Detail") = oRow("EmployeeName")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(roTypes.Any2DateTime(oRow("key3datetime")).ToShortDateString)
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.PunchesInLockDate.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next
                    End If

                    tbRet = tbRes
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_PunchesInLockDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_PunchesInLockDate")
            End Try

            Return tbRet

        End Function

        ''' <summary>
        ''' Días con fichajes impares
        ''' </summary>
        ''' <param name="sResume">Descripción a mostrar en pantalla de alerta</param>
        ''' <param name="bIsUrgent">Indica si hay que mostrar con alerta gráfica</param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDesktopAlerts_EmployeesWithIncompletePunches(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByRef dDateFrom As DateTime, ByRef dDateTo As DateTime, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable = Nothing
            Dim tbRes As DataTable = Nothing
            Dim tbRet As DataTable = Nothing

            'Inicializamos a no critica la alerta
            bIsCritic = False
            bIsUrgent = False
            Try
                Dim strSQL As String = " @SELECT# snt.Key1Numeric as idemployee, Key3DateTime, emp.Name as EmployeeName" &
                         " from sysroNotificationTasks snt WITH (NOLOCK) " &
                         " inner join Notifications noti WITH (NOLOCK) on snt.IDNotification = noti.ID and noti.ShowOnDesktop = 1 and noti.IDType= " & eNotificationType.Day_With_Unmatched_Time_Record & " and snt.key3datetime between dateadd(d,-30,getdate()) and getdate() " &
                         " inner join sysroNotificationTypes snty WITH (NOLOCK) on snty.ID = noti.IDType " &
                         " inner join employees emp WITH (NOLOCK) on emp.ID = snt.Key1Numeric " &
                         " inner join sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = snty.FeatureType AND pof.FeatureAlias = snty.Feature AND pof.Permission > 3 " &
                         " inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport = " & oState.IDPassport.ToString & " And poe.IDEmployee = emp.ID And CONVERT(DATE,GETDATE()) between poe.BeginDate And poe.EndDate "

                tbAux = CreateDataTable(strSQL, )

                If Not bNeedDetails Then
                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim rowIndex As Integer = 0
                        Do While (Not bIsUrgent OrElse Not bIsCritic) AndAlso rowIndex < tbAux.Rows.Count
                            If tbAux.Rows(rowIndex)("key3datetime") <= Now.AddDays(-7) Then bIsUrgent = True
                            If roTypes.Any2DateTime(tbAux.Rows(rowIndex)("key3datetime")).Date = Now.Date Then bIsCritic = True
                            rowIndex += 1
                        Loop

                        Dim dv As New DataView(tbAux)
                        tbRet = dv.ToTable

                        If tbRet.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(dv.Cast(Of DataRowView).GroupBy(Function(r) r("IdEmployee")).ToList.Count)
                            oState.Language.AddUserToken(dv.Cast(Of DataRowView).GroupBy(Function(r) r("Key3DateTime")).ToList.Count)
                            If tbRet.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.EmployeesWithIncompletePunches", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.EmployeeWithIncompletePunches", String.Empty)
                            End If
                            dDateFrom = dv.Cast(Of DataRowView).Min(Function(r) r("Key3DateTime"))
                            dDateTo = dv.Cast(Of DataRowView).Max(Function(r) r("Key3DateTime"))
                        End If
                    End If
                Else
                    ' Creo tabla de resultado
                    tbRes = New DataTable
                    tbRes.Columns.Add("ID", GetType(Integer))
                    tbRes.Columns.Add("IDEmployee", GetType(Integer))
                    tbRes.Columns.Add("Column1Detail", GetType(String))
                    tbRes.Columns.Add("Column2Detail", GetType(String))
                    tbRes.Columns.Add("Column3Detail", GetType(String))

                    Dim oGrouppState As New roNotificationState
                    oGrouppState.IDPassport = oState.IDPassport

                    Dim ID As Integer = 0
                    If tbAux IsNot Nothing Then
                        For Each oRow As DataRow In tbAux.Rows
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("IDEmployee")
                            oResRow("Column1Detail") = oRow("EmployeeName")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(roTypes.Any2DateTime(oRow("key3datetime")).ToShortDateString)
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.EmployeesWithIncompletePunches.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next
                    End If
                    tbRet = tbRes
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_EmployeesWithIncompletePunches")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_EmployeesWithIncompletePunches")
            End Try

            Return tbRet
        End Function

        ''' <summary>
        ''' Días con fichajes no justificados
        ''' </summary>
        ''' <param name="sResume">Descripción a mostrar en pantalla de alerta</param>
        ''' <param name="bIsUrgent">Indica si hay que mostrar con alerta gráfica</param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDesktopAlerts_EmployeesWithNonJustifiedPunches(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByRef dDateFrom As DateTime, ByRef dDateTo As DateTime, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable = Nothing
            Dim tbRet As DataTable = Nothing
            Dim tbRes As DataTable = Nothing

            'Inicializamos a no critica la alerta
            bIsCritic = False
            bIsUrgent = False
            Try
                Dim strSQL As String

                strSQL = "@SELECT# Employees.Name, DailyCauses.IDEmployee, DailyCauses.Date " &
                         "FROM Employees WITH (NOLOCK) " &
                         " INNER JOIN DailyCauses WITH (NOLOCK) ON Employees.ID = DailyCauses.IDEmployee and DailyCauses.IDCause = 0 and Date between " & roTypes.Any2Time(DateTime.Now.Date.AddDays(-31)).SQLSmallDateTime & " And  " & roTypes.Any2Time(DateTime.Now.Date.AddDays(-1)).SQLSmallDateTime & " " &
                         " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Calendar.JustifyIncidences' AND pof.Permission > 3 " &
                         " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport = " & oState.IDPassport.ToString & " And poe.IDEmployee = Employees.ID And DailyCauses.Date between poe.BeginDate And poe.EndDate "

                strSQL = strSQL & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetDesktopAlerts_EmployeesWithNonJustifiedPunches)

                tbAux = CreateDataTable(strSQL, )

                If Not bNeedDetails Then
                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim rowIndex As Integer = 0
                        Do While Not bIsUrgent AndAlso rowIndex < tbAux.Rows.Count
                            If tbAux.Rows(rowIndex)("Date") <= Now.AddDays(-7) Then bIsUrgent = True
                            rowIndex += 1
                        Loop

                        Dim dv As New DataView(tbAux)
                        tbRet = dv.ToTable

                        If tbRet.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(dv.Cast(Of DataRowView).GroupBy(Function(r) r("Date")).ToList.Count)
                            oState.Language.AddUserToken(dv.Cast(Of DataRowView).GroupBy(Function(r) r("IdEmployee")).ToList.Count)
                            If tbRet.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.EmployeesWithNonJustifiedPunches", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.EmployeeWithNonJustifiedPunches", String.Empty)
                            End If
                            dDateFrom = dv.Cast(Of DataRowView).Min(Function(r) r("Date"))
                            dDateTo = dv.Cast(Of DataRowView).Max(Function(r) r("Date"))
                        End If
                    End If
                Else
                    ' Creo tabla de resultado
                    tbRes = New DataTable
                    tbRes.Columns.Add("ID", GetType(Integer))
                    tbRes.Columns.Add("IDEmployee", GetType(Integer))
                    tbRes.Columns.Add("Column1Detail", GetType(String))
                    tbRes.Columns.Add("Column2Detail", GetType(String))
                    tbRes.Columns.Add("Column3Detail", GetType(String))

                    Dim oGrouppState As New roNotificationState
                    oGrouppState.IDPassport = oState.IDPassport

                    Dim ID As Integer = 0
                    If tbAux IsNot Nothing Then
                        For Each oRow As DataRow In tbAux.Rows
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("IDEmployee")
                            oResRow("Column1Detail") = oRow("Name")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(roTypes.Any2DateTime(oRow("Date")).ToShortDateString)
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.EmployeesWithNonJustifiedPunches.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next
                    End If

                    tbRet = tbRes
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_EmployeesWithNonJustifiedPunches")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_EmployeesWithNonJustifiedPunches")
            End Try

            Return tbRet
        End Function

        ''' <summary>
        ''' Días con fichajes no fiables
        ''' </summary>
        ''' <param name="sResume">Descripción a mostrar en pantalla de alerta</param>
        ''' <param name="bIsUrgent">Indica si hay que mostrar con alerta gráfica</param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDesktopAlerts_EmployeesWithNonReliablePunches(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByRef dDateFrom As DateTime, ByRef dDateTo As DateTime, ByVal bNeedDetail As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable = Nothing
            Dim tbRet As DataTable = Nothing
            Dim tbRes As DataTable = Nothing

            'Inicializamos a no critica la alerta
            bIsCritic = False
            bIsUrgent = False
            Try

                Dim strSQL As String = " @select# Employees.Name AS EmployeeName, Punches.IDEmployee, Punches.ShiftDate " &
                           "     from Employees WITH (NOLOCK) " &
                           " INNER JOIN Punches WITH (NOLOCK) ON Punches.IDEmployee = Employees.ID AND Punches.ActualType IN(1, 2) AND IsNotReliable = 1 " &
                           "            AND Punches.ShiftDate between " & roTypes.Any2Time(DateTime.Now.Date.AddDays(-31)).SQLSmallDateTime & " and " & roTypes.Any2Time(DateTime.Now.Date.AddDays(-1)).SQLSmallDateTime & " " &
                           " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Calendar.Punches.Punches' AND pof.Permission > 3 " &
                           " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport = " & oState.IDPassport.ToString & " And poe.IDEmployee = Punches.IDEmployee And Punches.ShiftDate between poe.BeginDate And poe.EndDate "

                strSQL = strSQL & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetDesktopAlerts_EmployeesWithNonReliablePunches)

                tbAux = CreateDataTable(strSQL, )

                If Not bNeedDetail Then
                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim rowIndex As Integer = 0
                        Do While Not bIsUrgent AndAlso rowIndex < tbAux.Rows.Count
                            If tbAux.Rows(rowIndex)("ShiftDate") <= Now.AddDays(-7) Then bIsUrgent = True
                            rowIndex += 1
                        Loop

                        Dim dv As New DataView(tbAux)
                        tbRet = dv.ToTable

                        If tbRet.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(dv.Cast(Of DataRowView).GroupBy(Function(r) r("ShiftDate")).ToList.Count)
                            oState.Language.AddUserToken(dv.Cast(Of DataRowView).GroupBy(Function(r) r("IdEmployee")).ToList.Count)
                            If tbRet.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.EmployeesWithUnreliablePunches", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.EmployeeWithUnreliablePunches", String.Empty)
                            End If
                            dDateFrom = dv.Cast(Of DataRowView).Min(Function(r) r("ShiftDate"))
                            dDateTo = dv.Cast(Of DataRowView).Max(Function(r) r("ShiftDate"))
                        End If
                    End If
                Else
                    ' Creo tabla de resultado
                    tbRes = New DataTable
                    tbRes.Columns.Add("ID", GetType(Integer))
                    tbRes.Columns.Add("IDEmployee", GetType(Integer))
                    tbRes.Columns.Add("Column1Detail", GetType(String))
                    tbRes.Columns.Add("Column2Detail", GetType(String))
                    tbRes.Columns.Add("Column3Detail", GetType(String))

                    Dim oGrouppState As New roNotificationState
                    oGrouppState.IDPassport = oState.IDPassport

                    Dim ID As Integer = 0
                    If tbAux IsNot Nothing Then
                        For Each oRow As DataRow In tbAux.Rows
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("IDEmployee")
                            oResRow("Column1Detail") = oRow("EmployeeName")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(roTypes.Any2DateTime(oRow("ShiftDate")).ToShortDateString)
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.EmployeesWithNonReliablePunches.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next
                    End If

                    tbRet = tbRes
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_EmployeesWithNonReliablePunches")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_EmployeesWithNonReliablePunches")
            End Try

            Return tbRet
        End Function

        ''' <summary>
        ''' Empleados que deberían estar presentes
        ''' </summary>
        ''' <param name="sResume">Descripción a mostrar en pantalla de alerta</param>
        ''' <param name="bIsUrgent">Indica si hay que mostrar con alerta gráfica</param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDesktopAlerts_EmployeesThatShouldBePresent(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetail As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable = Nothing
            Dim tbRet As DataTable = Nothing
            Dim tbRes As DataTable = Nothing

            'Inicializamos a no critica la alerta, esta alerta siempre es crítica
            bIsCritic = True

            Try
                Dim strSQL As String = " @SELECT# Employees.ID as idemployee, Employees.Name AS empname, EmployeeStatus.BeginMandatory, EmployeeStatus.LocalBeginMandatory, Key3DateTime AS Key3DatetimeEx  " &
                          " FROM( @SELECT# ROW_NUMBER() over (partition by noti.IDType, snt.key1numeric, Key3Datetime order by Key3Datetime desc) As 'RowNumber1', noti.IDType, snt.Key1Numeric as idemployee, Key3DateTime " &
                          " FROM sysroNotificationTasks snt WITH (NOLOCK) INNER JOIN Notifications noti WITH (NOLOCK) ON snt.IDNotification = noti.ID AND noti.ShowOnDesktop = 1 AND noti.IDType= " & eNotificationType.Employee_Not_Arrived_or_Late & " AND snt.key3datetime BETWEEN dateadd(d, -30, getdate()) AND getdate()) alert " &
                          " INNER JOIN sysroNotificationTypes snty WITH (NOLOCK) ON snty.ID = alert.IDType " &
                          " INNER JOIN Employees WITH (NOLOCK) ON Employees.ID = alert.idemployee " &
                          " INNER JOIN employeestatus WITH (NOLOCK) ON employeestatus.IDEmployee = alert.idemployee AND DATEPART(year, employeestatus.BeginMandatory) <> 2079 " &
                          " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = snty.FeatureType AND pof.FeatureAlias = snty.Feature AND pof.Permission > 3 " &
                          " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport = " & oState.IDPassport.ToString & " And poe.IDEmployee = alert.idemployee And CONVERT(DATE,alert.key3datetime) between poe.BeginDate And poe.EndDate " &
                          " WHERE alert.RowNumber1 = 1"
                strSQL = strSQL & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetDesktopAlerts_EmployeesThatShouldBePresent)
                tbAux = CreateDataTable(strSQL, )

                If Not bNeedDetail Then
                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                        Dim rowIndex As Integer = 0
                        Do While Not bIsUrgent AndAlso rowIndex < tbAux.Rows.Count
                            If tbAux.Rows(rowIndex)("Key3DatetimeEx") <= Now.AddHours(-2) Then bIsUrgent = True
                            rowIndex += 1
                        Loop

                        Dim dv As New DataView(tbAux)
                        tbRet = dv.ToTable

                        If tbRet.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(dv.Cast(Of DataRowView).GroupBy(Function(r) r("IdEmployee")).ToList.Count)
                            If tbRet.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.EmployeesShouldBePresent", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.EmployeeShouldBePresent", String.Empty)
                            End If
                        End If
                    End If
                Else
                    ' Creo tabla de resultado
                    tbRes = New DataTable
                    tbRes.Columns.Add("ID", GetType(Integer))
                    tbRes.Columns.Add("IDEmployee", GetType(Integer))
                    tbRes.Columns.Add("Column1Detail", GetType(String))
                    tbRes.Columns.Add("Column2Detail", GetType(String))
                    tbRes.Columns.Add("Column3Detail", GetType(String))

                    Dim oGrouppState As New roNotificationState
                    oGrouppState.IDPassport = oState.IDPassport

                    Dim ID As Integer = 0
                    If tbAux IsNot Nothing Then
                        For Each oRow As DataRow In tbAux.Rows
                            If Any2DateTime(oRow("BeginMandatory")).Year <> 2079 Then
                                Dim oResRow As DataRow = tbRes.NewRow
                                oResRow("ID") = ID
                                ID += 1
                                oResRow("IDEmployee") = oRow("IDEmployee")
                                oResRow("Column1Detail") = oRow("empname")
                                oState.Language.ClearUserTokens()
                                If IsDBNull(oRow("LocalBeginMandatory")) Then
                                    oState.Language.AddUserToken(Any2DateTime(oRow("BeginMandatory")).ToShortDateString)
                                    oState.Language.AddUserToken(Any2DateTime(oRow("BeginMandatory")).ToShortTimeString)
                                Else
                                    ' Empleado en otra zona horaria
                                    oState.Language.AddUserToken(Any2DateTime(oRow("LocalBeginMandatory")).ToShortDateString)
                                    oState.Language.AddUserToken(Any2DateTime(oRow("LocalBeginMandatory")).ToShortTimeString)
                                End If

                                oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.EmployeeShouldBePresent.Detail", String.Empty)
                                tbRes.Rows.Add(oResRow)
                            End If
                        Next
                    End If

                    tbRet = tbRes
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_EmployeesThatShouldBePresent")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_EmployeesThatShouldBePresent")
            End Try

            Return tbRet
        End Function

#End Region

#Region "Alertas de convenios"

        ''' <summary>
        ''' Convenios Empleados que superan un saldo
        ''' </summary>
        ''' <param name="sResume">Descripción a mostrar en pantalla de alerta</param>
        ''' <param name="bIsUrgent">Indica si hay que mostrar con alerta gráfica</param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDesktopAlerts_LabAgreeMaxAlert(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable = Nothing
            Dim tbRes As DataTable = Nothing

            'Esta alerta siempre es crítica
            bIsCritic = True
            bIsUrgent = False
            Try
                Dim strSQL As String = "@SELECT# ResolverValue1 AS IDEmployee, Employees.Name as EmployeeName ,ResolverValue2 As IDConcept, Concepts.Name as ConceptName, ResolverValue3 As IDContract " &
                        " from sysroUserTasks " &
                            " inner join Employees on Employees.ID = sysroUserTasks.ResolverValue1 " &
                            " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Calendar.JustifyIncidences' AND pof.Permission >3 " &
                            " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport = " & oState.IDPassport.ToString & " And poe.IDEmployee = Employees.ID And CONVERT(DATE,sysroUserTasks.DateCreated) between poe.BeginDate And poe.EndDate " &
                            " inner join Concepts on Concepts.ID = ResolverValue2 " &
                        "where ResolverURL = 'FN:\\ExceededMaxValue' " &
                                "and sysroUserTasks.DateCreated >= dateadd(d,-30,getdate()) and sysroUserTasks.DateCreated <= getdate() "
                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then

                    If Not bNeedDetails Then
                        bIsUrgent = True
                        tbRes = tbAux

                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(tbRes.Rows.Count)
                        If tbRes.Rows.Count > 1 Then
                            sResume = oState.Language.Translate("DesktopAlert.LabAgreeMaxExceededs", String.Empty)
                        Else
                            sResume = oState.Language.Translate("DesktopAlert.LabAgreeMaxExceeded", String.Empty)
                        End If
                    Else

                        ' Creo tabla de resultado
                        tbRes = New DataTable
                        tbRes.Columns.Add("ID", GetType(Integer))
                        tbRes.Columns.Add("IDEmployee", GetType(Integer))
                        tbRes.Columns.Add("Column1Detail", GetType(String))
                        tbRes.Columns.Add("Column2Detail", GetType(String))
                        tbRes.Columns.Add("Column3Detail", GetType(String))

                        Dim oGrouppState As New roNotificationState
                        oGrouppState.IDPassport = oState.IDPassport

                        Dim ID As Integer = 0
                        For Each oRow As DataRow In tbAux.Rows
                            bIsUrgent = True
                            ' Añado registro al detalle
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("IDEmployee")
                            oResRow("Column1Detail") = oRow("EmployeeName")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(Any2String(oRow("EmployeeName")))
                            oState.Language.AddUserToken(Any2String(oRow("ConceptName")))
                            oState.Language.AddUserToken(Any2String(oRow("IDContract")))
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.LabAgreeMaxExceeded.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next

                        If tbRes.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(tbRes.Rows.Count)
                            If tbRes.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.LabAgreeMaxExceededs", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.LabAgreeMaxExceeded", String.Empty)
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_LabAgreeMaxAlert")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_LabAgreeMaxAlert")
            End Try

            Return tbRes
        End Function

        ''' <summary>
        ''' Convenios Empleados que llegan al minimo de un saldo
        ''' </summary>
        ''' <param name="sResume">Descripción a mostrar en pantalla de alerta</param>
        ''' <param name="bIsUrgent">Indica si hay que mostrar con alerta gráfica</param>
        ''' <param name="oState"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDesktopAlerts_LabAgreeMinAlert(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable
            Dim tbRes As DataTable = Nothing

            'Esta alerta siempre es crítica
            bIsCritic = True

            Try
                Dim strSQL As String = "@SELECT# ResolverValue1 AS IDEmployee, Employees.Name as EmployeeName ,ResolverValue2 As IDConcept, Concepts.Name as ConceptName, ResolverValue3 As IDContract " &
                         "from sysroUserTasks " &
                            "inner join Employees on Employees.ID = sysroUserTasks.ResolverValue1 " &
                            " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Calendar.JustifyIncidences' AND pof.Permission >3 " &
                            " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport = " & oState.IDPassport.ToString & " And poe.IDEmployee = Employees.ID And CONVERT(DATE,sysroUserTasks.DateCreated) between poe.BeginDate And poe.EndDate " &
                            " INNER JOIN Concepts on Concepts.ID = ResolverValue2 " &
                         "where ResolverURL = 'FN:\\ExceededMinValue' " &
                                "and sysroUserTasks.DateCreated >= dateadd(d,-30,getdate()) and sysroUserTasks.DateCreated <= getdate() "
                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then

                    If Not bNeedDetails Then
                        bIsUrgent = True
                        tbRes = tbAux

                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(tbRes.Rows.Count)
                        If tbRes.Rows.Count > 1 Then
                            sResume = oState.Language.Translate("DesktopAlert.LabAgreeMinExceededs", String.Empty)
                        Else
                            sResume = oState.Language.Translate("DesktopAlert.LabAgreeMinExceeded", String.Empty)
                        End If
                    Else

                        ' Creo tabla de resultado
                        tbRes = New DataTable
                        tbRes.Columns.Add("ID", GetType(Integer))
                        tbRes.Columns.Add("IDEmployee", GetType(Integer))
                        tbRes.Columns.Add("Column1Detail", GetType(String))
                        tbRes.Columns.Add("Column2Detail", GetType(String))
                        tbRes.Columns.Add("Column3Detail", GetType(String))

                        Dim oGrouppState As New roNotificationState
                        oGrouppState.IDPassport = oState.IDPassport

                        Dim ID As Integer = 0
                        For Each oRow As DataRow In tbAux.Rows
                            bIsUrgent = True
                            ' Añado registro al detalle
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("IDEmployee")
                            oResRow("Column1Detail") = oRow("EmployeeName")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(Any2String(oRow("EmployeeName")))
                            oState.Language.AddUserToken(Any2String(oRow("ConceptName")))
                            oState.Language.AddUserToken(Any2String(oRow("IDContract")))
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.LabAgreeMinExceeded.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next

                        If tbRes.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(tbRes.Rows.Count)
                            If tbRes.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.LabAgreeMinExceededs", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.LabAgreeMinExceeded", String.Empty)
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_LabAgreeMinAlert")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_LabAgreeMinAlert")
            End Try

            Return tbRes
        End Function

#End Region

#Region "Alertas de productiV"

        Public Shared Function GetDesktopAlerts_TasksStartsToday(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable
            Dim tbRes As DataTable = Nothing

            'Esta alerta nunca es crítica
            bIsCritic = False

            Try

                Dim strSQL As String = "@SELECT# Tasks.Id, Tasks.Name, Tasks.Project from Tasks " &
                         " INNER JOIN sysrovwSecurity_PermissionOverCostCenters poc WITH (NOLOCK) on poc.IDPassport = " & oState.IDPassport.ToString & " And poc.IDCostCenter = Tasks.IDCenter " &
                         " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Tasks.Definition' AND pof.Permission >3 " &
                         " where ExpectedStartDate is not null and DATEADD(dd, 0, DATEDIFF(dd, 0, ExpectedStartDate)) = DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) and ID <> 0  and Status = 0 AND StartDate IS NULL "
                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then

                    If Not bNeedDetails Then
                        bIsUrgent = True
                        tbRes = tbAux

                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(tbRes.Rows.Count)
                        If tbRes.Rows.Count > 1 Then
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksStartsToday", String.Empty)
                        Else
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskStartsToday", String.Empty)
                        End If
                    Else

                        ' Creo tabla de resultado
                        tbRes = New DataTable
                        tbRes.Columns.Add("ID", GetType(Integer))
                        tbRes.Columns.Add("IDEmployee", GetType(Integer))
                        tbRes.Columns.Add("Column1Detail", GetType(String))
                        tbRes.Columns.Add("Column2Detail", GetType(String))
                        tbRes.Columns.Add("Column3Detail", GetType(String))

                        Dim oGrouppState As New roNotificationState
                        oGrouppState.IDPassport = oState.IDPassport

                        Dim ID As Integer = 0
                        For Each oRow As DataRow In tbAux.Rows
                            bIsUrgent = True
                            ' Añado registro al detalle
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("ID")
                            oResRow("Column1Detail") = oRow("Name")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(Any2String(oRow("Name")))
                            oState.Language.AddUserToken(Any2String(oRow("Project")))
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.ProductiVTaskStartsToday.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next

                        If tbRes.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(tbRes.Rows.Count)
                            If tbRes.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksStartsToday", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskStartsToday", String.Empty)
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksStartsToday")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksStartsToday")
            End Try

            Return tbRes
        End Function

        Public Shared Function GetDesktopAlerts_TasksEndsToday(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable
            Dim tbRes As DataTable = Nothing

            'Esta alerta siempre es crítica
            bIsCritic = False

            Try
                Dim strSQL As String
                strSQL = "@SELECT# Tasks.Id, Tasks.Name, Tasks.Project from Tasks " &
                         " INNER JOIN sysrovwSecurity_PermissionOverCostCenters poc WITH (NOLOCK) on poc.IDPassport = " & oState.IDPassport.ToString & " And poc.IDCostCenter = Tasks.IDCenter " &
                         " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Tasks.Definition' AND pof.Permission >3 " &
                         " where ExpectedEndDate is not null and DATEADD(dd, 0, DATEDIFF(dd, 0, ExpectedEndDate)) = DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) and ID <> 0  and Status = 0 AND EndDate IS NULL "
                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then

                    If Not bNeedDetails Then
                        bIsUrgent = True
                        tbRes = tbAux

                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(tbRes.Rows.Count)
                        If tbRes.Rows.Count > 1 Then
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksEndsToday", String.Empty)
                        Else
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskEndToday", String.Empty)
                        End If
                    Else

                        ' Creo tabla de resultado
                        tbRes = New DataTable
                        tbRes.Columns.Add("ID", GetType(Integer))
                        tbRes.Columns.Add("IDEmployee", GetType(Integer))
                        tbRes.Columns.Add("Column1Detail", GetType(String))
                        tbRes.Columns.Add("Column2Detail", GetType(String))
                        tbRes.Columns.Add("Column3Detail", GetType(String))

                        Dim oGrouppState As New roNotificationState
                        oGrouppState.IDPassport = oState.IDPassport

                        Dim ID As Integer = 0
                        For Each oRow As DataRow In tbAux.Rows
                            bIsUrgent = True
                            ' Añado registro al detalle
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("ID")
                            oResRow("Column1Detail") = oRow("Name")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(Any2String(oRow("Name")))
                            oState.Language.AddUserToken(Any2String(oRow("Project")))
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.ProductiVTaskEndToday.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next

                        If tbRes.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(tbRes.Rows.Count)
                            If tbRes.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksEndsToday", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskEndToday", String.Empty)
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksEndsToday")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksEndsToday")
            End Try

            Return tbRes
        End Function

        Public Shared Function GetDesktopAlerts_TasksExceededPlannedTime(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable
            Dim tbRes As DataTable = Nothing

            'Esta alerta siempre es crítica
            bIsCritic = True

            Try
                Dim strSQL As String = "@SELECT# tmpRes.Id, tmpRes.Name, tmpRes.Project from " &
                         " (@SELECT# Tasks.ID, Tasks.Name, Tasks.Project, tasks.InitialTime, sum(DailyTaskAccruals.Value) as Worked from Tasks " &
                         " INNER JOIN sysrovwSecurity_PermissionOverCostCenters poc WITH (NOLOCK) on poc.IDPassport = " & oState.IDPassport.ToString & " And poc.IDCostCenter = Tasks.IDCenter " &
                         " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Tasks.Definition' AND pof.Permission >3 " &
                         " INNER JOIN DailyTaskAccruals on Tasks.ID = DailyTaskAccruals.IDTask " &
                         " where Tasks.ID <> 0  and Tasks.Status in(0,1,3) and Tasks.InitialTime > 0 " &
                         " group by Tasks.ID, Tasks.Name, tasks.Project, tasks.InitialTime) tmpRes " &
                         " where tmpRes.Worked > tmpRes.InitialTime "
                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then

                    If Not bNeedDetails Then
                        bIsUrgent = True
                        tbRes = tbAux

                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(tbRes.Rows.Count)
                        If tbRes.Rows.Count > 1 Then
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksExceededPlannedTime", String.Empty)
                        Else
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskExceededPlannedTime", String.Empty)
                        End If
                    Else

                        ' Creo tabla de resultado
                        tbRes = New DataTable
                        tbRes.Columns.Add("ID", GetType(Integer))
                        tbRes.Columns.Add("IDEmployee", GetType(Integer))
                        tbRes.Columns.Add("Column1Detail", GetType(String))
                        tbRes.Columns.Add("Column2Detail", GetType(String))
                        tbRes.Columns.Add("Column3Detail", GetType(String))

                        Dim oGrouppState As New roNotificationState
                        oGrouppState.IDPassport = oState.IDPassport

                        Dim ID As Integer = 0
                        For Each oRow As DataRow In tbAux.Rows
                            bIsUrgent = True
                            ' Añado registro al detalle
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("ID")
                            oResRow("Column1Detail") = oRow("Name")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(Any2String(oRow("Name")))
                            oState.Language.AddUserToken(Any2String(oRow("Project")))
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.ProductiVTaskExceededPlannedTime.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next

                        If tbRes.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(tbRes.Rows.Count)
                            If tbRes.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksEndsToday", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskEndToday", String.Empty)
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_LabAgreeMaxAlert")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_LabAgreeMaxAlert")
            End Try

            Return tbRes
        End Function

        Public Shared Function GetDesktopAlerts_TasksExceededStartDate(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable
            Dim tbRes As DataTable = Nothing

            'Esta alerta siempre es crítica
            bIsCritic = True

            Try
                Dim strSQL As String = "@SELECT# Tasks.Id, Tasks.Name, Tasks.Project from Tasks " &
                         " INNER JOIN sysrovwSecurity_PermissionOverCostCenters poc WITH (NOLOCK) on poc.IDPassport = " & oState.IDPassport.ToString & " And poc.IDCostCenter = Tasks.IDCenter " &
                         " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Tasks.Definition' AND pof.Permission >3 " &
                         " where ExpectedStartDate is not null and DATEADD(dd, 0, DATEDIFF(dd, 0, ExpectedStartDate)) < DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) and ID <> 0  and Status = 0  AND StartDate IS NULL "

                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then

                    If Not bNeedDetails Then
                        bIsUrgent = True
                        tbRes = tbAux

                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(tbRes.Rows.Count)
                        If tbRes.Rows.Count > 1 Then
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksExceededStartDate", String.Empty)
                        Else
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskExceededStartDate", String.Empty)
                        End If
                    Else

                        ' Creo tabla de resultado
                        tbRes = New DataTable
                        tbRes.Columns.Add("ID", GetType(Integer))
                        tbRes.Columns.Add("IDEmployee", GetType(Integer))
                        tbRes.Columns.Add("Column1Detail", GetType(String))
                        tbRes.Columns.Add("Column2Detail", GetType(String))
                        tbRes.Columns.Add("Column3Detail", GetType(String))

                        Dim oGrouppState As New roNotificationState
                        oGrouppState.IDPassport = oState.IDPassport

                        Dim ID As Integer = 0
                        For Each oRow As DataRow In tbAux.Rows
                            bIsUrgent = True
                            ' Añado registro al detalle
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("ID")
                            oResRow("Column1Detail") = oRow("Name")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(Any2String(oRow("Name")))
                            oState.Language.AddUserToken(Any2String(oRow("Project")))
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.ProductiVTaskExceededStartDate.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next

                        If tbRes.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(tbRes.Rows.Count)
                            If tbRes.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksExceededStartDate", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskExceededStartDate", String.Empty)
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksExceededStartDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksExceededStartDate")
            End Try

            Return tbRes
        End Function

        Public Shared Function GetDesktopAlerts_TasksExceededEndDate(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable
            Dim tbRes As DataTable = Nothing

            'Esta alerta siempre es crítica
            bIsCritic = True

            Try
                Dim strSQL As String = "@SELECT# Tasks.Id, Tasks.Name, Tasks.Project from Tasks " &
                         " INNER JOIN sysrovwSecurity_PermissionOverCostCenters poc WITH (NOLOCK) on poc.IDPassport = " & oState.IDPassport.ToString & " And poc.IDCostCenter = Tasks.IDCenter " &
                         " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Tasks.Definition' AND pof.Permission >3 " &
                         " where ExpectedEndDate is not null and DATEADD(dd, 0, DATEDIFF(dd, 0, ExpectedEndDate)) < DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) and ID <> 0  and Status = 0 "
                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then

                    If Not bNeedDetails Then
                        bIsUrgent = True
                        tbRes = tbAux

                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(tbRes.Rows.Count)
                        If tbRes.Rows.Count > 1 Then
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksExceededEndDate", String.Empty)
                        Else
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskExceededEndDate", String.Empty)
                        End If
                    Else

                        ' Creo tabla de resultado
                        tbRes = New DataTable
                        tbRes.Columns.Add("ID", GetType(Integer))
                        tbRes.Columns.Add("IDEmployee", GetType(Integer))
                        tbRes.Columns.Add("Column1Detail", GetType(String))
                        tbRes.Columns.Add("Column2Detail", GetType(String))
                        tbRes.Columns.Add("Column3Detail", GetType(String))

                        Dim oGrouppState As New roNotificationState
                        oGrouppState.IDPassport = oState.IDPassport

                        Dim ID As Integer = 0
                        For Each oRow As DataRow In tbAux.Rows
                            bIsUrgent = True
                            ' Añado registro al detalle
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("ID")
                            oResRow("Column1Detail") = oRow("Name")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(Any2String(oRow("Name")))
                            oState.Language.AddUserToken(Any2String(oRow("Project")))
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.ProductiVTaskExceededEndDate.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next

                        If tbRes.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(tbRes.Rows.Count)
                            If tbRes.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksExceededEndDate", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskExceededEndDate", String.Empty)
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksExceededEndDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksExceededEndDate")
            End Try

            Return tbRes
        End Function

        Public Shared Function GetDesktopAlerts_TasksWithAlerts(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable
            Dim tbRes As DataTable = Nothing

            'Esta alerta siempre es crítica
            bIsCritic = True

            Try
                Dim strSQL As String = "@SELECT# Tasks.Id, Tasks.Name, Tasks.Project from Tasks " &
                         " INNER JOIN sysrovwSecurity_PermissionOverCostCenters poc WITH (NOLOCK) on poc.IDPassport = " & oState.IDPassport.ToString & " And poc.IDCostCenter = Tasks.IDCenter " &
                         " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Tasks.Definition' AND pof.Permission >3 " &
                         " INNER JOIN AlertsTask on Tasks.ID = AlertsTask.IDTask " &
                         " where Tasks.ID <> 0 and AlertsTask.IsReaded = 0 "
                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then

                    If Not bNeedDetails Then
                        bIsUrgent = True
                        tbRes = tbAux

                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(tbRes.Rows.Count)
                        If tbRes.Rows.Count > 1 Then
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksWithAlerts", String.Empty)
                        Else
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskWithAlerts", String.Empty)
                        End If
                    Else

                        ' Creo tabla de resultado
                        tbRes = New DataTable
                        tbRes.Columns.Add("ID", GetType(Integer))
                        tbRes.Columns.Add("IDEmployee", GetType(Integer))
                        tbRes.Columns.Add("Column1Detail", GetType(String))
                        tbRes.Columns.Add("Column2Detail", GetType(String))
                        tbRes.Columns.Add("Column3Detail", GetType(String))

                        Dim oGrouppState As New roNotificationState
                        oGrouppState.IDPassport = oState.IDPassport

                        Dim ID As Integer = 0
                        For Each oRow As DataRow In tbAux.Rows
                            bIsUrgent = True
                            ' Añado registro al detalle
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("ID")
                            oResRow("Column1Detail") = oRow("Name")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(Any2String(oRow("Name")))
                            oState.Language.AddUserToken(Any2String(oRow("Project")))
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.ProductiVTaskWithAlerts.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next

                        If tbRes.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(tbRes.Rows.Count)
                            If tbRes.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksWithAlerts", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskWithAlerts", String.Empty)
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksWithAlerts")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksWithAlerts")
            End Try

            Return tbRes
        End Function

        Public Shared Function GetDesktopAlerts_TasksWaitingComplete(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable
            Dim tbRes As DataTable = Nothing

            'Esta alerta nunca es crítica
            bIsCritic = False

            Try

                Dim strSQL As String = "@SELECT# Tasks.Id, Tasks.Name, Tasks.Project from Tasks " &
                         " INNER JOIN sysrovwSecurity_PermissionOverCostCenters poc WITH (NOLOCK) on poc.IDPassport = " & oState.IDPassport.ToString & " And poc.IDCostCenter = Tasks.IDCenter " &
                         " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Tasks.Definition' AND pof.Permission >3 " &
                         " where ID <> 0 AND Status = 3 "
                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    If Not bNeedDetails Then
                        bIsUrgent = True
                        tbRes = tbAux

                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(tbRes.Rows.Count)
                        If tbRes.Rows.Count > 1 Then
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksWaitingComplete", String.Empty)
                        Else
                            sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskWaitingComplete", String.Empty)
                        End If
                    Else

                        ' Creo tabla de resultado
                        tbRes = New DataTable
                        tbRes.Columns.Add("ID", GetType(Integer))
                        tbRes.Columns.Add("IDEmployee", GetType(Integer))
                        tbRes.Columns.Add("Column1Detail", GetType(String))
                        tbRes.Columns.Add("Column2Detail", GetType(String))
                        tbRes.Columns.Add("Column3Detail", GetType(String))

                        Dim oGrouppState As New roNotificationState
                        oGrouppState.IDPassport = oState.IDPassport

                        Dim ID As Integer = 0
                        For Each oRow As DataRow In tbAux.Rows
                            bIsUrgent = True
                            ' Añado registro al detalle
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            ID += 1
                            oResRow("IDEmployee") = oRow("ID")
                            oResRow("Column1Detail") = oRow("Name")
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(Any2String(oRow("Name")))
                            oState.Language.AddUserToken(Any2String(oRow("Project")))
                            oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.ProductiVTaskWaitingComplete.Detail", String.Empty)
                            tbRes.Rows.Add(oResRow)
                        Next

                        If tbRes.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(tbRes.Rows.Count)
                            If tbRes.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTasksWaitingComplete", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.ProductiVTaskWaitingComplete", String.Empty)
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksWaitingComplete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_TasksWaitingComplete")
            End Try

            Return tbRes
        End Function

#End Region

#Region "Alertas de seguridad"

        Public Shared Function GetDesktopAlerts_NonSupervisedDepartments(ByRef sResume As String, ByRef bIsUrgent As Boolean, ByRef bIsCritic As Boolean, ByVal bNeedDetails As Boolean, ByRef oState As roNotificationState) As DataTable
            Dim tbAux As DataTable
            Dim tbRes As DataTable = Nothing

            'Esta alerta nunca es crítica
            bIsCritic = True

            Try
                Dim strSQL As String = $"@SELECT# pof.Permission FROM sysrovwSecurity_PermissionOverFeatures pof WHERE " &
                         $"pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Administration.Security'"

                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                    Dim iPermision As Integer = tbAux.Rows(0)("Permission")
                    If iPermision <= 3 Then Return tbRes
                Else
                    Return tbRes
                End If

                strSQL = "@SELECT# * FROM sysrovwNonSupervisedDepartments "
                tbAux = CreateDataTable(strSQL, )

                If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then

                    If Not bNeedDetails Then
                        bIsUrgent = True
                        tbRes = tbAux

                        oState.Language.ClearUserTokens()
                        oState.Language.AddUserToken(tbRes.Rows.Count)
                        If tbRes.Rows.Count > 1 Then
                            sResume = oState.Language.Translate("DesktopAlert.NonSupervisedDepartments", String.Empty)
                        Else
                            sResume = oState.Language.Translate("DesktopAlert.NonSupervisedDepartment", String.Empty)
                        End If
                    Else
                        ' Creo tabla de resultado
                        tbRes = New DataTable
                        tbRes.Columns.Add("ID", GetType(Integer))
                        tbRes.Columns.Add("IDEmployee", GetType(Integer))
                        tbRes.Columns.Add("Column1Detail", GetType(String))
                        tbRes.Columns.Add("Column2Detail", GetType(String))
                        tbRes.Columns.Add("Column3Detail", GetType(String))

                        Dim oNotificationState As New roNotificationState
                        oNotificationState.IDPassport = oState.IDPassport

                        Dim ID As Integer = 0
                        For Each oRow As DataRow In tbAux.Rows
                            bIsUrgent = True
                            ' Añado registro al detalle
                            Dim oResRow As DataRow = tbRes.NewRow
                            oResRow("ID") = ID
                            oResRow("IDEmployee") = -1
                            Dim sFullGroupName As String = oRow("fullgroupname")
                            Dim lDepartments As String() = sFullGroupName.Split("\"c)
                            Dim sParentDepartment As String = String.Empty
                            If lDepartments.Length >= 2 Then
                                sParentDepartment = lDepartments(lDepartments.Length - 2).Trim
                            End If
                            oResRow("Column1Detail") = roTypes.Any2String(oRow("Name")).Trim.ToString
                            If sParentDepartment <> String.Empty Then
                                oResRow("Column1Detail") = $"{roTypes.Any2String(oRow("Name")).Trim} ({sParentDepartment})"
                            End If

                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(Any2String(oRow("Name")))
                            If sParentDepartment = String.Empty Then
                                oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.NonSupervisedCompany.Detail", String.Empty)
                            Else
                                oResRow("Column2Detail") = oState.Language.Translate("DesktopAlert.NonSupervisedDepartment.Detail", String.Empty)
                            End If

                            tbRes.Rows.Add(oResRow)
                            ID += 1
                        Next

                        If tbRes.Rows.Count > 0 Then
                            oState.Language.ClearUserTokens()
                            oState.Language.AddUserToken(tbRes.Rows.Count)
                            If tbRes.Rows.Count > 1 Then
                                sResume = oState.Language.Translate("DesktopAlert.NonSupervisedDepartments", String.Empty)
                            Else
                                sResume = oState.Language.Translate("DesktopAlert.NonSupervisedDepartment", String.Empty)
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_NonSupervisedDepartments")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roNotification::GetDesktopAlerts_NonSupervisedDepartments")
            End Try

            Return tbRes
        End Function

#End Region

#End Region

#End Region

#Region "Request Helper Methods"
        Public Shared Sub AddStat(sKey As String, ByRef dicStats As Dictionary(Of String, Double), ByRef watch As Stopwatch)
            Try
                watch.Stop()
                dicStats.Add(sKey, watch.Elapsed.TotalSeconds)
                watch.Restart()
            Catch ex As Exception
            End Try
        End Sub

        Public Shared Function GenerateNotificationsForRequest(ByVal IdRequest As Integer, ByVal bIsNewRequest As Boolean, ByVal _State As roNotificationState, Optional ByVal bGenerateEmployeeNotification As Boolean = False, Optional ByVal bCheckForExistingNotifications As Boolean = True) As Boolean
            Dim bRet As Boolean = True
            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim iIDNotificationTask As Integer = -1
                Dim bCreateNotification As Boolean = False
                Dim idEmployee As Integer = -1
                Dim requestStatus As eRequestStatus = eRequestStatus.Pending
                Dim iRequestType As eRequestType = eRequestType.None
                Dim idExchangeEmployee As Integer = -1

                Dim strSQL As String = "@SELECT# IDEmployee, Status, RequestType, IDEmployeeExchange  FROM Requests where ID=" & IdRequest

                Dim tbResult As DataTable = CreateDataTable(strSQL)
                If tbResult IsNot Nothing AndAlso tbResult.Rows.Count > 0 Then
                    idEmployee = roTypes.Any2Integer(tbResult.Rows(0)("IDEmployee"))
                    requestStatus = roTypes.Any2Integer(tbResult.Rows(0)("Status"))
                    iRequestType = roTypes.Any2Integer(tbResult.Rows(0)("RequestType"))
                    idExchangeEmployee = roTypes.Any2Integer(tbResult.Rows(0)("IDEmployeeExchange"))
                End If

                ' Declaración de Joranda MVP: Por el momento no hay notificaciones, ni para empleados ni para supervisores

                If iRequestType <> eRequestType.DailyRecord Then
                    If idEmployee > 0 Then
                        If bCheckForExistingNotifications Then
                            Dim oParam As New AdvancedParameter.roAdvancedParameter("AllSupervisorsLevelAreNotificated", New AdvancedParameter.roAdvancedParameterState(-1))
                            Dim bAllSupervisorLevelAreNotificated As Boolean = (oParam.Value.Trim = "1")
                            Dim notiType As eNotificationType = eNotificationType.Request_Pending

                            Select Case requestStatus
                                Case eRequestStatus.Canceled
                                    notiType = eNotificationType.Absence_Canceled_By_User
                            End Select

                            If notiType = eNotificationType.Request_Pending AndAlso Notifications.roNotification.NotificationTaskExistsByType(eNotificationType.Request_Pending, iIDNotificationTask, _State, IdRequest) Then
                                ' Ya existe la notificación. Si el estado deja de ser pendiente, elimino la notificación
                                If requestStatus = eRequestStatus.Accepted OrElse requestStatus = eRequestStatus.Denied OrElse requestStatus = eRequestStatus.Canceled Then
                                    If Not Notifications.roNotification.DeleteTask(iIDNotificationTask, Nothing) Then
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roRequest::Save::Unable to delete notification task for request id = " & IdRequest.ToString)
                                    End If
                                ElseIf bAllSupervisorLevelAreNotificated AndAlso requestStatus = eRequestStatus.OnGoing Then
                                    bCreateNotification = True
                                End If
                            Else
                                ' No existe la notificación, o no es relevante. La creo si el estado no es Aceptada o denegada
                                If requestStatus <> eRequestStatus.Accepted AndAlso requestStatus <> eRequestStatus.Denied Then
                                    bCreateNotification = True
                                End If
                            End If

                            If bCreateNotification Then
                                If Not Notifications.roNotification.GenerateNotificationTask(False, notiType, _State, IdRequest, , , , IIf(bIsNewRequest, "SENDALLWAYS", requestStatus.ToString.ToUpper), Now) Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotification::GenerateNotificationsForRequest::Unable to create notification task for request id = " & IdRequest.ToString)
                                End If
                            End If
                        End If

                        If bGenerateEmployeeNotification AndAlso requestStatus = eRequestStatus.Accepted OrElse requestStatus = eRequestStatus.Denied Then
                            strSQL = "@SELECT# IsNull(Activated,0) As Active from Notifications where ID = 1910"

                            Dim isActive As Boolean = Any2Boolean(ExecuteScalar(strSQL))

                            If isActive Then
                                strSQL = " IF NOT EXISTS (@SELECT# 1 FROM sysroNotificationTasks WHERE IDNotification=1910   AND Key1Numeric = " & idEmployee.ToString & " AND  Key2Numeric = " & IdRequest & " AND Parameters like '" & requestStatus.ToString.ToUpper & "') "
                                strSQL += "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters ) VALUES " &
                                            "(1910, " & idEmployee & "," & IdRequest & " ,'" & requestStatus.ToString.ToUpper & "')"

                                bRet = ExecuteSql(strSQL)

                                If iRequestType = Base.DTOs.eRequestType.ExchangeShiftBetweenEmployees Then
                                    strSQL = " IF NOT EXISTS (@SELECT# 1 FROM sysroNotificationTasks WHERE IDNotification=1910   AND Key1Numeric = " & idExchangeEmployee.ToString & " AND  Key2Numeric = " & IdRequest & " AND Parameters like '" & requestStatus.ToString.ToUpper & "') "
                                    strSQL += "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters ) VALUES " &
                                                "(1910, " & idExchangeEmployee & "," & IdRequest & " ,'" & requestStatus.ToString.ToUpper & "')"

                                    bRet = ExecuteSql(strSQL)
                                End If

                                Try
                                    If requestStatus = eRequestStatus.Accepted Then

                                        VTBase.Extensions.roPushNotification.SendNotificationPushToPassport(idEmployee, LoadType.Employee, _State.Language.Translate("Push.RequestSubject", String.Empty, False), _State.Language.Translate("Push.RequestApproved", String.Empty, False))
                                    Else
                                        VTBase.Extensions.roPushNotification.SendNotificationPushToPassport(idEmployee, LoadType.Employee, _State.Language.Translate("Push.RequestSubject", String.Empty, False), _State.Language.Translate("Push.RequestDenied", String.Empty, False))
                                    End If
                                Catch ex As Exception
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotification::GenerateNotificationsForRequest::Unknown", ex)
                                End Try
                            End If
                        End If
                    Else
                        bRet = False
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotification::GenerateNotificationsForRequest::Unable to manage (create or delete) notification task for request id = " & IdRequest.ToString)
                    End If
                End If
            Catch ex As Exception
                bRet = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotification::GenerateNotificationsForRequest::Unable to manage (create or delete) notification task for request id = " & IdRequest.ToString, ex)
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bRet)
            End Try

            Return bRet
        End Function

#End Region

    End Class

End Namespace