Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.ExternalSystems.CTAIMA.Core.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace DataLink.RoboticsExternAccess

    Public Class RoboticsExternAccess

        Private Shared _mInstance As Hashtable = Nothing

        Private oLog As New roLog("RoboticsExternAccess")
        Private _isLogEnabled As Boolean = False
        Private _ipsEnabled As String = String.Empty

        Private _strExternAccessUserName As String = ""
        Private _strExternAccessPwd As String = ""
        Private _oAudit As AuditState.wscAuditState = Nothing
        Private _primaryToken As String = String.Empty
        Private _secondaryToken As String = String.Empty
        Private _returnCode As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._UnknownError


#Region "Properties"
        Public Shared ReadOnly Property GetInstance(Optional ByVal bForceNew As Boolean = False, Optional ByVal strCompanyId As String = "VTLive") As RoboticsExternAccess
            Get
                Dim oApi As RoboticsExternAccess = Nothing
                If bForceNew Then
                    If System.Web.HttpContext.Current IsNot Nothing Then
                        Dim oWebApp As Web.HttpApplication = System.Web.HttpContext.Current.ApplicationInstance
                        Dim propertyInfo As System.Reflection.MethodInfo = oWebApp.GetType().GetMethod("OnApplicationReloadSharedData")
                        If propertyInfo IsNot Nothing Then
                            propertyInfo.Invoke(oWebApp, Nothing)
                        End If
                    End If
                    oApi = New RoboticsExternAccess(strCompanyId)
                Else
                    If (_mInstance Is Nothing) Then
                        _mInstance = New Hashtable
                    End If

                    If _mInstance.ContainsKey(strCompanyId) Then
                        oApi = _mInstance(strCompanyId)
                    Else
                        oApi = New RoboticsExternAccess(strCompanyId)
                        oApi.oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::Start(" + System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location).FileVersion + ") inicialized.")
                        _mInstance.Add(strCompanyId, oApi)
                    End If
                End If

                Return oApi
            End Get
        End Property
        Public ReadOnly Property IpsEnabled As String
            Get
                Return _ipsEnabled
            End Get
        End Property

        Public ReadOnly Property ReturnCode As Core.DTOs.ReturnCode
            Get
                Return _returnCode
            End Get
        End Property

#End Region

#Region "Constructors"
        Private Sub New(Optional ByVal strCompanyId As String = "VTLive")
            Try
                Dim oParam As New AdvancedParameter.roAdvancedParameter((Robotics.Base.DTOs.AdvancedParameterType.Customization).ToString, New AdvancedParameter.roAdvancedParameterState)

                oParam = New AdvancedParameter.roAdvancedParameter((Robotics.Base.DTOs.AdvancedParameterType.ExternAccessIPs).ToString, New AdvancedParameter.roAdvancedParameterState)
                _ipsEnabled = roTypes.Any2String(oParam.Value)

                oParam = New AdvancedParameter.roAdvancedParameter((Robotics.Base.DTOs.AdvancedParameterType.ExternAccessUserName).ToString, New AdvancedParameter.roAdvancedParameterState)
                _strExternAccessUserName = roTypes.Any2String(oParam.Value)

                oParam = New AdvancedParameter.roAdvancedParameter((Robotics.Base.DTOs.AdvancedParameterType.ExternAccessPassword).ToString, New AdvancedParameter.roAdvancedParameterState)
                _strExternAccessPwd = roTypes.Any2String(oParam.Value)

                oParam = New AdvancedParameter.roAdvancedParameter((Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken1).ToString, New AdvancedParameter.roAdvancedParameterState)
                _primaryToken = roTypes.Any2String(oParam.Value)

                oParam = New AdvancedParameter.roAdvancedParameter((Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken2).ToString, New AdvancedParameter.roAdvancedParameterState)
                _secondaryToken = roTypes.Any2String(oParam.Value)

                _oAudit = New AuditState.wscAuditState()
                Try
                    oParam = New AdvancedParameter.roAdvancedParameter((Robotics.Base.DTOs.AdvancedParameterType.CustomizationLogEnabled).ToString, New AdvancedParameter.roAdvancedParameterState)
                    _isLogEnabled = (roTypes.Any2Integer(oParam.Value) = 1)
                Catch ex As Exception
                    _isLogEnabled = False
                End Try
            Catch ex As Exception

            End Try

        End Sub
#End Region

#Region "Public Methods"

        Public Function CreateOrUpdateEmployee(ByVal oEmployee As roDatalinkStandarEmployee, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef iNewEmployeeId As Integer) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oEmployee)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateEmployee::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiEmployees() 'stateaquí

                bResult = oDataImport.CreateOrUpdateEmployee(oEmployee, strErrorMsg, iNewEmployeeId, False)

                If bResult Then
                    Extensions.roConnector.InitTask(TasksType.BROADCASTER)
                    returnCode = Core.DTOs.ReturnCode._OK

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateEmployee::OK::" & returnCode.ToString & "::" & oEmployee.NifEmpleado & $". {strErrorMsg}")
                Else
                    Select Case oDataImport.State.Result
                        Case DataLinkResultEnum.InvalidEmployee
                            returnCode = Core.DTOs.ReturnCode._InvalidEmployee
                        Case DataLinkResultEnum.InvalidGroup
                            returnCode = Core.DTOs.ReturnCode._InvalidGroup
                        Case DataLinkResultEnum.InvalidContract
                            returnCode = Core.DTOs.ReturnCode._InvalidContract
                        Case DataLinkResultEnum.InvalidCard
                            returnCode = Core.DTOs.ReturnCode._InvalidCard
                        Case DataLinkResultEnum.AuthorizationError
                            returnCode = Core.DTOs.ReturnCode._AuthorizationError
                        Case DataLinkResultEnum.SomeUserFieldsNotSaved
                            returnCode = Core.DTOs.ReturnCode._SomeUserFieldsNotSaved
                        Case DataLinkResultEnum.ExpiredContract
                            returnCode = Core.DTOs.ReturnCode._ContractAlreadyClosed
                        Case DataLinkResultEnum.InvalidLabAgree
                            returnCode = Core.DTOs.ReturnCode._InvalidLabAgree
                        Case DataLinkResultEnum.FreezeDateException
                            returnCode = Core.DTOs.ReturnCode._InvalidCloseDate
                        Case DataLinkResultEnum.FieldDataIncorrect, DataLinkResultEnum.FormatColumnIsWrong
                            returnCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                        Case DataLinkResultEnum.InvalidMovility
                            returnCode = Core.DTOs.ReturnCode._InvalidMovility
                        Case DataLinkResultEnum.NotAllowedChangeContractBeginDate
                            returnCode = Core.DTOs.ReturnCode._NotAllowedChangeContractBeginDate
                        Case DataLinkResultEnum.InvalidPhotoData
                            returnCode = Core.DTOs.ReturnCode._InvalidPhotoData
                        Case DataLinkResultEnum.InvalidLogin
                            returnCode = Core.DTOs.ReturnCode._InvalidLogin
                        Case DataLinkResultEnum.InvalidPinLength
                            returnCode = Core.DTOs.ReturnCode._InvalidPinLength
                        Case DataLinkResultEnum.ContractDataProtected, DataLinkResultEnum.HasPunchesAfterContractEndDate
                            returnCode = Core.DTOs.ReturnCode._ContractDataProtected
                        Case DataLinkResultEnum.NewEmployeeCannotBeSourceOfPlanning
                            returnCode = Core.DTOs.ReturnCode._NewEmployeeCannotBeSourceOfPlanning
                        Case Else
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                    End Select

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oEmployee)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateEmployee::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("CreateOrUpdateEmployee")

                oParam.Add("{sObjectId}")
                oValues.Add(oEmployee.NombreEmpleado)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString() & $". {strErrorMsg}" & ". Details:" & roJSONHelper.SerializeNewtonSoft(oEmployee))
                Else
                    oValues.Add(returnCode.ToString() & $". {strErrorMsg}")
                End If

                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::CreateOrUpdateEmployee::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function CreateOrUpdateEmployeeSAGE200c(ByVal oEmployee As roDatalinkStandarEmployee, ByVal aContracts As roDatalinkEmployeeContractsHistory, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode) As Boolean
            Dim bResult As Boolean = True
            Dim bHaveToClose As Boolean = False
            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oEmployee)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateEmployeeSAGE200c::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiEmployees()

                Dim lContracts As New List(Of roDatalinkEmployeeContract)
                lContracts = aContracts.Contracts.ToList
                Dim lContractsSortedList As List(Of roDatalinkEmployeeContract) = lContracts.OrderBy(Function(o) o.StartContractDate).ToList()

                bResult = oDataImport.ImportEmployeesSAGE200c_ResetContractsAndFixMobilitiesIfNeeded(oEmployee.NifEmpleado, oEmployee.UniqueEmployeeID, lContractsSortedList)
                If bResult Then
                    ' Una vez actualizado el histórico de contratos del empleado, actualizao los datos del empleado asociandolos al último contrato
                    Dim oLasContract As New roDatalinkEmployeeContract
                    oLasContract = lContractsSortedList.Last
                    oEmployee.LabAgreeName = oLasContract.LabAgreeName
                    oEmployee.IDContract = oLasContract.IDContract
                    oEmployee.StartContractDate = oLasContract.StartContractDate
                    oEmployee.EndContractDate = oLasContract.EndContractDate

                    Dim iNewEmployeeId As Integer
                    bResult = oDataImport.CreateOrUpdateEmployee(oEmployee, strErrorMsg, iNewEmployeeId, False)
                End If

                If bResult Then
                    Extensions.roConnector.InitTask(TasksType.BROADCASTER)
                    returnCode = Core.DTOs.ReturnCode._OK

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateEmployeeSAGE200c::OK::" & returnCode.ToString & "::" & oEmployee.NifEmpleado)
                Else
                    Select Case oDataImport.State.Result
                        Case DataLinkResultEnum.InvalidEmployee
                            returnCode = Core.DTOs.ReturnCode._InvalidEmployee
                        Case DataLinkResultEnum.InvalidGroup
                            returnCode = Core.DTOs.ReturnCode._InvalidGroup
                        Case DataLinkResultEnum.InvalidContract
                            returnCode = Core.DTOs.ReturnCode._InvalidContract
                        Case DataLinkResultEnum.InvalidCard
                            returnCode = Core.DTOs.ReturnCode._InvalidCard
                        Case DataLinkResultEnum.AuthorizationError
                            returnCode = Core.DTOs.ReturnCode._AuthorizationError
                        Case DataLinkResultEnum.SomeUserFieldsNotSaved
                            returnCode = Core.DTOs.ReturnCode._SomeUserFieldsNotSaved
                        Case DataLinkResultEnum.ExpiredContract
                            returnCode = Core.DTOs.ReturnCode._ContractAlreadyClosed
                        Case DataLinkResultEnum.InvalidLabAgree
                            returnCode = Core.DTOs.ReturnCode._InvalidLabAgree
                        Case DataLinkResultEnum.FreezeDateException
                            returnCode = Core.DTOs.ReturnCode._InvalidCloseDate
                        Case DataLinkResultEnum.FieldDataIncorrect, DataLinkResultEnum.FormatColumnIsWrong
                            returnCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                        Case DataLinkResultEnum.InvalidMovility
                            returnCode = Core.DTOs.ReturnCode._InvalidMovility
                        Case DataLinkResultEnum.InvalidContractHistory
                            returnCode = Core.DTOs.ReturnCode._InvalidContractHistory
                        Case DataLinkResultEnum.NoContracts
                            returnCode = Core.DTOs.ReturnCode._NoContracts
                        Case Else
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                    End Select

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oEmployee)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateEmployeeSAGE200c::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("CreateOrUpdateEmployeeSAGE200c")

                oParam.Add("{sObjectId}")
                oValues.Add(oEmployee.NombreEmpleado)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oEmployee))
                Else
                    oValues.Add(returnCode.ToString())
                End If

                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::CreateOrUpdateEmployeeSAGE200c::Exception::", ex)
                End If
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bResult)
            End Try

            Return bResult
        End Function

        Public Function CreateOrUpdateAbsence(ByVal oAbsence As roDatalinkStandarAbsence, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oAbsence)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateAbsence::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiAbsences()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                bResult = oDataImport.CreateOrUpdateAbsence(oAbsence, strErrorMsg, iReturnCode)

                If bResult Then
                    returnCode = Core.DTOs.ReturnCode._OK

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateAbsence::OK::" & oAbsence.NifEmpleado)
                Else
                    returnCode = iReturnCode

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oAbsence)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateAbsence::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("CreateOrUpdateAbsence")

                oParam.Add("{sObjectId}")
                oValues.Add(oAbsence.UniqueEmployeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oAbsence))
                Else
                    oValues.Add(returnCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::CreateOrUpdateAbsence::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetAbsences(ByVal oAbsenceCriteria As roDatalinkStandarAbsenceCriteria, ByRef oDatalinkStandarAbsenceResponse As roDatalinkStandarAbsenceResponse) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oAbsenceCriteria)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetAbsences::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiAbsences()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lAbsences As New Generic.List(Of roDatalinkStandarAbsence)
                bResult = oDataImport.GetAbsences(oAbsenceCriteria, lAbsences, strErrorMsg, iReturnCode)

                If bResult Then
                    oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandarAbsenceResponse.Absences = lAbsences

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetAbsences::OK::" & oAbsenceCriteria.NifEmpleado)
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            Case DataLinkResultEnum.InvalidEmployee
                                oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._InvalidEmployee
                            Case DataLinkResultEnum.InvalidCause
                                oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._InvalidCause
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandarAbsenceResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oAbsenceCriteria)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetAbsences::Error::" & oDatalinkStandarAbsenceResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetAbsences")

                oParam.Add("{sObjectId}")
                oValues.Add(oAbsenceCriteria.UniqueEmployeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(oDatalinkStandarAbsenceResponse.ResultCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandarAbsenceResponse))
                Else
                    oValues.Add(oDatalinkStandarAbsenceResponse.ResultCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetAbsences::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetAccruals(ByVal oAccrualCriteria As roDatalinkStandarAccrualCriteria, ByRef oDatalinkStandarAccrualResponse As roDatalinkStandarAccrualResponse, Optional ByVal bolToDate As Boolean = False) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oAccrualCriteria)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetAccruals::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiAccruals()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lAccruals As New Generic.List(Of roDatalinkStandarAccrual)
                bResult = oDataImport.GetAccruals(oAccrualCriteria, lAccruals, strErrorMsg, iReturnCode, bolToDate)

                If bResult Then
                    oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandarAccrualResponse.Accruals = lAccruals

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetAccruals::OK::" & oAccrualCriteria.NifEmpleado)
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            Case DataLinkResultEnum.InvalidEmployee
                                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._InvalidEmployee
                            Case DataLinkResultEnum.InvalidAccrual
                                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._InvalidAccrualData
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandarAccrualResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oAccrualCriteria)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetAccruals::Error::" & oDatalinkStandarAccrualResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetAccruals")

                oParam.Add("{sObjectId}")
                oValues.Add(oAccrualCriteria.UniqueEmployeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(oDatalinkStandarAccrualResponse.ResultCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandarAccrualResponse))
                Else
                    oValues.Add(oDatalinkStandarAccrualResponse.ResultCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetAccruals::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetTaskAccruals(ByVal oAccrualCriteria As roDatalinkStandarTaskAccrualCriteria, ByRef oDatalinkStandarAccrualResponse As roDatalinkStandarTaskAccrualResponse) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oAccrualCriteria)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetTaskAccruals::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiAccruals()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lAccruals As New Generic.List(Of roDatalinkStandarTaskAccrual)
                bResult = oDataImport.GetTaskAccruals(oAccrualCriteria, lAccruals, strErrorMsg, iReturnCode)

                If bResult Then
                    oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandarAccrualResponse.Accruals = lAccruals

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetTaskAccruals::OK::" & oAccrualCriteria.NifEmpleado)
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            Case DataLinkResultEnum.InvalidEmployee
                                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._InvalidEmployee
                            Case DataLinkResultEnum.InvalidAccrual
                                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._InvalidAccrualData
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandarAccrualResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oAccrualCriteria)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetTaskAccruals::Error::" & oDatalinkStandarAccrualResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetTaskAccruals")

                oParam.Add("{sObjectId}")
                oValues.Add(oAccrualCriteria.UniqueEmployeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(oDatalinkStandarAccrualResponse.ResultCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandarAccrualResponse))
                Else
                    oValues.Add(oDatalinkStandarAccrualResponse.ResultCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                oDatalinkStandarAccrualResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetTaskAccruals::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function CreateOrUpdateHolidays(ByVal oHolidays As roDatalinkStandarHolidays, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode) As Boolean
            Dim bResult As Boolean = True

            Try

                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oHolidays)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateHolidays::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiCalendar()

                bResult = oDataImport.CreateOrUpdateHolidays(oHolidays, strErrorMsg)

                If bResult Then
                    returnCode = Core.DTOs.ReturnCode._OK

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateHolidays::OK::" & oHolidays.NifEmpleado)
                Else
                    Select Case oDataImport.State.Result
                        Case DataLinkResultEnum.InvalidEmployee
                            returnCode = Core.DTOs.ReturnCode._InvalidEmployee
                        Case DataLinkResultEnum.InvalidShift
                            returnCode = Core.DTOs.ReturnCode._InvalidShift
                        Case DataLinkResultEnum.FormatColumnIsWrong
                            returnCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                        Case Else
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                    End Select

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oHolidays)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateHolidays::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("CreateOrUpdateHolidays")

                oParam.Add("{sObjectId}")
                oValues.Add(oHolidays.UniqueEmployeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oHolidays))
                Else
                    oValues.Add(returnCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::CreateOrUpdateHolidays::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function CreateOrUpdateCalendar(ByVal oCalendar As roDatalinkStandarCalendar, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oCalendar)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateCalendar::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiCalendar()
                bResult = oDataImport.CreateOrUpdateCalendar(oCalendar, strErrorMsg)

                If bResult Then
                    returnCode = Core.DTOs.ReturnCode._OK

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateCalendar::OK::" & oCalendar.NifEmpleado)
                Else
                    Select Case oDataImport.State.Result
                        Case DataLinkResultEnum.InvalidEmployee
                            returnCode = Core.DTOs.ReturnCode._InvalidEmployee
                        Case DataLinkResultEnum.InvalidShift
                            returnCode = Core.DTOs.ReturnCode._InvalidShift
                        Case DataLinkResultEnum.FormatColumnIsWrong
                            returnCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                        Case DataLinkResultEnum.InvalidData
                            returnCode = Core.DTOs.ReturnCode._InvalidCalendarData
                        Case Else
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                    End Select

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oCalendar)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateCalendar::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("CreateOrUpdateCalendar")

                oParam.Add("{sObjectId}")
                oValues.Add(oCalendar.UniqueEmployeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oCalendar))
                Else
                    oValues.Add(returnCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::CreateOrUpdateCalendar::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function CreateOrUpdateDocument(ByVal oDocument As roDatalinkStandardDocument, ByVal UserName As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnMsg As String) As Boolean
            Dim bResult As Boolean = True
            Dim strParameters As String = Nothing
            Dim strErrorMsg As String = String.Empty

            Try
                If _isLogEnabled Then
                    strParameters = roJSONHelper.SerializeNewtonSoft(oDocument)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateDocument::WSCall-Parameters::" & strParameters)
                End If

                Dim oDataImport As New roApiDocuments()

                If oDocument.DocumentTitle.Length = 0 Then
                    returnCode = Core.DTOs.ReturnCode._InvalidDocumentTitle
                    returnMsg = "Document should have a title"
                    Return False
                End If

                bResult = oDataImport.CreateOrUpdateDocument(oDocument, UserName, strErrorMsg)

                If bResult Then
                    returnCode = Core.DTOs.ReturnCode._OK
                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateDocument::OK::" & oDocument.NifEmpleado)
                Else
                    Select Case oDataImport.State.Result
                        Case DataLinkResultEnum.InvalidEmployee
                            returnCode = Core.DTOs.ReturnCode._InvalidEmployee
                        Case DataLinkResultEnum.InvalidDocumentType
                            returnCode = Core.DTOs.ReturnCode._InvalidDocumentType
                        Case DataLinkResultEnum.InvalidDocumentData
                            returnCode = Core.DTOs.ReturnCode._InvalidDocumentData
                        Case DataLinkResultEnum.ErrorSavingDocument
                            returnCode = Core.DTOs.ReturnCode._ErrorSavingDocument
                        Case DataLinkResultEnum.DocumentNotDeliverable
                            returnCode = Core.DTOs.ReturnCode._DocumentNotDeliverable
                        Case DataLinkResultEnum.UnexistingDocumentTemplate
                            returnCode = Core.DTOs.ReturnCode._UnexistentDocumentTemplate
                        Case DataLinkResultEnum.DocumentTooBig
                            returnCode = Core.DTOs.ReturnCode._DocumentTooBig
                        Case DataLinkResultEnum.EmployeeDocumentAlreadyExists
                            returnCode = Core.DTOs.ReturnCode._DocumentAlreadyExists
                        Case DataLinkResultEnum.ErrorDeletingDocument
                            returnCode = Core.DTOs.ReturnCode._ErrorDeletingDocument
                        Case DataLinkResultEnum.ExternalIdDuplicated
                            returnCode = Core.DTOs.ReturnCode._ExternalIdDuplicated
                        Case DataLinkResultEnum.Exception
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                        Case Else
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                    End Select

                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateDocument::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("CreateOrUpdateDocument")

                oParam.Add("{sObjectId}")
                oValues.Add(oDocument.UniqueEmployeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDocument))
                Else
                    oValues.Add(returnCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::CreateOrUpdateDocument::Exception::", ex)
                End If
            End Try

            returnMsg = strErrorMsg
            Return bResult
        End Function

        Public Function DeleteDocument(ByVal externalDocumentId As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnMsg As String) As Boolean
            Dim bResult As Boolean = True
            Dim strErrorMsg As String = String.Empty

            Try
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateDocument::WSCall-Parameters::" & externalDocumentId)
                End If

                Dim datalinkState As roDataLinkState = New roDataLinkState
                Dim dataImport As New roApiDocuments()

                Dim documentToBeDeleted As New DTOs.roDocument
                bResult = dataImport.DeleteDocument(externalDocumentId, strErrorMsg, documentToBeDeleted)

                If bResult Then
                    returnCode = Core.DTOs.ReturnCode._OK
                Else
                    Select Case dataImport.State.Result
                        Case DataLinkResultEnum.UnexistentDocument
                            returnCode = Core.DTOs.ReturnCode._UnexistentDocument
                        Case DataLinkResultEnum.ErrorDeletingDocument
                            returnCode = Core.DTOs.ReturnCode._ErrorDeletingDocument
                        Case DataLinkResultEnum.DocumentDeletedButExternalIdStillExists
                            returnCode = Core.DTOs.ReturnCode._DocumentDeletedButExternalIdStillExists
                        Case DataLinkResultEnum.Exception
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                        Case Else
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                    End Select

                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateCalendar::Error::" & returnCode.ToString & "::" & externalDocumentId)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aDelete

                oParam.Add("{sImportType}")
                oValues.Add("DeleteDocument")

                oParam.Add("{sObjectId}")
                oValues.Add(If(documentToBeDeleted IsNot Nothing AndAlso documentToBeDeleted.Title IsNot Nothing, $"{documentToBeDeleted.Title} ({externalDocumentId})", externalDocumentId))

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString() & "Details:" & If(documentToBeDeleted IsNot Nothing, roJSONHelper.SerializeNewtonSoft(documentToBeDeleted), externalDocumentId))
                Else
                    oValues.Add(returnCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::DeleteDocument::Exception::", ex)
                End If
            End Try

            returnMsg = strErrorMsg
            Return bResult
        End Function

        Public Function CreateOrUpdateEmployeePhoto(ByVal oPhoto As roDatalinkStandardPhoto, ByVal UserName As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnMsg As String) As Boolean
            Dim bResult As Boolean = True
            Dim strParameters As String = Nothing
            Dim strErrorMsg As String = String.Empty

            Try
                If _isLogEnabled Then
                    strParameters = roJSONHelper.SerializeNewtonSoft(oPhoto)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateEmployeePhoto::WSCall-Parameters::" & strParameters)
                End If

                Dim oDataImport As New roApiEmployees()

                bResult = oDataImport.CreateOrUpdateEmployeePhoto(oPhoto, UserName, strErrorMsg)

                If bResult Then
                    returnCode = Core.DTOs.ReturnCode._OK
                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateEmployeePhoto::OK::" & oPhoto.NifEmpleado)
                Else
                    Select Case oDataImport.State.Result
                        Case DataLinkResultEnum.InvalidEmployee
                            returnCode = Core.DTOs.ReturnCode._InvalidEmployee
                        Case DataLinkResultEnum.InvalidPhotoData
                            returnCode = Core.DTOs.ReturnCode._InvalidPhotoData
                        Case DataLinkResultEnum.Exception
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                        Case Else
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                    End Select

                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateEmployeePhoto::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("CreateOrUpdateEmployeePhoto")

                oParam.Add("{sObjectId}")
                oValues.Add(oPhoto.UniqueEmployeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oPhoto))
                Else
                    oValues.Add(returnCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::CreateOrUpdateEmployeePhoto::Exception::", ex)
                End If
            End Try

            returnMsg = strErrorMsg
            Return bResult
        End Function

        Public Function GetPunches(ByVal oPunchFilterType As PunchFilterType, ByVal Timestamp As DateTime, ByVal StartDate As Date, ByVal EndDate As Date, ByRef oDatalinkStandarPunchResponse As roDatalinkStandardPunchResponse, Optional ByVal EmployeeID As String = "") As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strParameters As String = ""
                If _isLogEnabled Then
                    strParameters = oPunchFilterType.ToString & "-" & Timestamp.ToString & "-" & StartDate.ToString & "-" & EndDate.ToString

                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPunches::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiPunches()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lPunches As New Generic.List(Of roDatalinkStandardPunch)
                bResult = oDataImport.GetPunches(oPunchFilterType, Timestamp, StartDate, EndDate, lPunches, strErrorMsg, iReturnCode, EmployeeID)

                If bResult Then
                    oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandarPunchResponse.Punches = lPunches

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPunches::OK")
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            'Case DataLinkResultEnum.InvalidEmployee
                            '    oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._InvalidEmployee
                            'Case DataLinkResultEnum.InvalidCause
                            '    oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._InvalidCause
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandarPunchResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPunches::Error::" & oDatalinkStandarPunchResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetPunchesBetweenDates")

                oParam.Add("{sObjectId}")
                oValues.Add(strParameters)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(CType(oDatalinkStandarPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandarPunchResponse))
                Else
                    oValues.Add(CType(oDatalinkStandarPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)

                bResult = True
            Catch ex As Exception
                bResult = False
                oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetPunches::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetPunchesEx(ByVal oPunchFilterType As PunchFilterType, ByVal Timestamp As DateTime, ByVal StartDate As Date, ByVal EndDate As Date, ByRef oDatalinkStandarPunchResponse As roDatalinkStandardPunchResponse, Optional ByVal EmployeeID As String = "") As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strParameters As String = ""
                If _isLogEnabled Then
                    strParameters = oPunchFilterType.ToString & "-" & Timestamp.ToString & "-" & StartDate.ToString & "-" & EndDate.ToString

                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPunches::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiPunches()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lPunches As New Generic.List(Of roDatalinkStandardPunch)
                bResult = oDataImport.GetPunchesEx(oPunchFilterType, Timestamp, StartDate, EndDate, lPunches, strErrorMsg, iReturnCode, EmployeeID)

                If bResult Then
                    oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandarPunchResponse.Punches = lPunches

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPunches::OK")
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            'Case DataLinkResultEnum.InvalidEmployee
                            '    oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._InvalidEmployee
                            'Case DataLinkResultEnum.InvalidCause
                            '    oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._InvalidCause
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandarPunchResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPunches::Error::" & oDatalinkStandarPunchResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetPunches")

                oParam.Add("{sObjectId}")
                oValues.Add(strParameters)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(CType(oDatalinkStandarPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandarPunchResponse))
                Else
                    oValues.Add(CType(oDatalinkStandarPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)

                bResult = True
            Catch ex As Exception
                bResult = False
                oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetPunches::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetPunchesWithIDEx(ByVal lIDPunches As List(Of String), ByRef oDatalinkStandardPunchResponse As roDatalinkStandardPunchResponse) As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strParameters As String = ""
                If _isLogEnabled Then
                    strParameters = String.Join(",", lIDPunches)

                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPunchesWithIDEx::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiPunches()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lPunches As New Generic.List(Of roDatalinkStandardPunch)
                bResult = oDataImport.GetPunchesWithIDEx(lIDPunches, lPunches, strErrorMsg, iReturnCode)

                If bResult Then
                    oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandardPunchResponse.Punches = lPunches

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPunchesWithIDEx::OK")
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandardPunchResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPunchesWithIDEx::Error::" & oDatalinkStandardPunchResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetPunchesWithIDEx")

                oParam.Add("{sObjectId}")
                oValues.Add(strParameters)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(CType(oDatalinkStandardPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandardPunchResponse))
                Else
                    oValues.Add(CType(oDatalinkStandardPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)

                bResult = True
            Catch ex As Exception
                bResult = False
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetPunchesWithIDEx::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function AddPunches(ByVal oPunchList As Generic.List(Of roDatalinkStandardPunch), ByRef oDatalinkStandarPunchResponse As roDatalinkStandardPunchResponse) As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strParameters As String = ""
                If _isLogEnabled Then
                    strParameters = strParameters = roJSONHelper.SerializeNewtonSoft(oPunchList)

                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::AddPunches::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiPunches()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lPunches As New Generic.List(Of roDatalinkStandardPunch)
                Dim lInvalidPunches As New Generic.List(Of roDatalinkStandardPunch)
                bResult = oDataImport.AddPunches(oPunchList, lInvalidPunches, strErrorMsg, iReturnCode)

                If bResult Then
                    oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandarPunchResponse.PunchesListError = lInvalidPunches

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::AddPunches::OK")
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            'Case DataLinkResultEnum.InvalidEmployee
                            '    oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._InvalidEmployee
                            'Case DataLinkResultEnum.InvalidCause
                            '    oDatalinkStandarAbsenceResponse.ResultCode = Core.DTOs.ReturnCode._InvalidCause
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandarPunchResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::AddPunches::Error::" & oDatalinkStandarPunchResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("AddPunches")

                oParam.Add("{sObjectId}")
                oValues.Add(strParameters)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(CType(oDatalinkStandarPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString() & " Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandarPunchResponse))
                Else
                    oValues.Add(CType(oDatalinkStandarPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                oDatalinkStandarPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::AddPunches::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function UpdatePunches(ByVal oPunchList As Generic.List(Of roDatalinkStandardPunch), ByRef oDatalinkStandardPunchResponse As roDatalinkStandardPunchResponse, ByVal oPunchCriteria As roPunchCriteria()) As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strParameters As String = ""
                If _isLogEnabled Then
                    strParameters = strParameters = roJSONHelper.SerializeNewtonSoft(oPunchList)

                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::UpdatePunches::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiPunches()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lPunches As New Generic.List(Of roDatalinkStandardPunch)
                Dim lInvalidPunches As New Generic.List(Of roDatalinkStandardPunch)
                bResult = oDataImport.UpdatePunches(oPunchList, lInvalidPunches, strErrorMsg, iReturnCode, oPunchCriteria)

                If bResult Then
                    oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandardPunchResponse.PunchesListError = lInvalidPunches

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::UpdatePunches::OK")
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandardPunchResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::UpdatePunches::Error::" & oDatalinkStandardPunchResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("UpdatePunches")

                oParam.Add("{sObjectId}")
                oValues.Add(strParameters)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(CType(oDatalinkStandardPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString() & " Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandardPunchResponse))
                Else
                    oValues.Add(CType(oDatalinkStandardPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::UpdatePunches::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function DeletePunches(ByVal oPunchList As Generic.List(Of roDatalinkStandardPunch), ByRef oDatalinkStandardPunchResponse As roDatalinkStandardPunchResponse) As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strParameters As String = ""
                If _isLogEnabled Then
                    strParameters = strParameters = roJSONHelper.SerializeNewtonSoft(oPunchList)

                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::DeletePunches::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiPunches()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lPunches As New Generic.List(Of roDatalinkStandardPunch)
                Dim lInvalidPunches As New Generic.List(Of roDatalinkStandardPunch)
                bResult = oDataImport.DeletePunches(oPunchList, lInvalidPunches, strErrorMsg, iReturnCode)

                If bResult Then
                    oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandardPunchResponse.PunchesListError = lInvalidPunches

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::DeletePunches::OK")
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandardPunchResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::DeletePunches::Error::" & oDatalinkStandardPunchResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("DeletePunches")

                oParam.Add("{sObjectId}")
                oValues.Add(strParameters)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(CType(oDatalinkStandardPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString() & " Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandardPunchResponse))
                Else
                    oValues.Add(CType(oDatalinkStandardPunchResponse.ResultCode, Core.DTOs.ReturnCode).ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                oDatalinkStandardPunchResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::DeletePunches::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetEmployees(ByRef oEmployees As Generic.List(Of roEmployee), ByVal onlyWithActiveContract As Boolean, ByVal IncludeOldData As Boolean, ByVal employeeID As String, ByVal FieldName As String, ByVal FieldValue As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnText As String, Optional ByVal Timestamp? As DateTime = Nothing) As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strParameters As String = "Details:" & onlyWithActiveContract.ToString & "," & IncludeOldData.ToString & "," & employeeID & "," & FieldName & "," & FieldValue

                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetEmployees::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataExport As New roApiEmployees

                returnCode = Core.DTOs.ReturnCode._UnknownError
                bResult = oDataExport.GetEmployees(oEmployees, onlyWithActiveContract, IncludeOldData, employeeID, FieldName, FieldValue, strErrorMsg, returnCode, Timestamp)

                If bResult Then

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetEmployees::OK")
                Else
                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetEmployees::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetEmployees")

                oParam.Add("{sObjectId}")
                oValues.Add(String.Empty)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString & " " & strParameters)
                Else
                    oValues.Add(returnCode.ToString)
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetEmployees::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetEmployeeById(ByRef oEmployee As roEmployee, ByVal employeeID As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnText As String) As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strErrorMsg As String = String.Empty
                Dim oDataExport As New roApiEmployees

                returnCode = Core.DTOs.ReturnCode._UnknownError
                oEmployee = oDataExport.GetEmployeeById(employeeID, strErrorMsg)

                If oEmployee IsNot Nothing Then
                    returnCode = Core.DTOs.ReturnCode._OK
                Else
                    returnCode = Core.DTOs.ReturnCode._InvalidEmployee
                End If
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetEmployees::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetGroups(ByRef oGroups As List(Of roGroup), ByVal bIncludeEmployees As Boolean, ByVal sGroupCode As String, ByRef returnCode As Core.DTOs.ReturnCode, ByRef returnText As String, Optional ByVal GroupID As String = "") As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strErrorMsg As String = String.Empty
                Dim oDataExport As New roApiEmployees

                bResult = oDataExport.GetGroups(oGroups, bIncludeEmployees, sGroupCode, strErrorMsg, returnCode, GroupID)

                Dim oParam As New List(Of String)
                Dim oValues As New List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetGroups")

                oParam.Add("{sObjectId}")
                oValues.Add(String.Empty)

                oParam.Add("{sReason}")
                oValues.Add(returnCode.ToString)

                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetGroups::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetHolidays(ByRef oHolidays As Generic.List(Of roHoliday), ByVal employeeID As String, ByVal StartDate As Date, ByVal EndDate As Date, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnText As String, Optional ByVal GetDeletedHolidays As Boolean = False) As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strParameters As String = "Details:" & employeeID & "," & StartDate.ToShortDateString & "," & EndDate.ToShortDateString

                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetHolidays::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataExport As New roApiCalendar

                returnCode = Core.DTOs.ReturnCode._UnknownError
                bResult = oDataExport.GetHolidays(oHolidays, employeeID, StartDate, EndDate, strErrorMsg, returnCode, GetDeletedHolidays)

                If bResult Then

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetHolidays::OK")
                Else
                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetHolidays::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetHolidays")

                oParam.Add("{sObjectId}")
                oValues.Add(employeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString & " " & strParameters)
                Else
                    oValues.Add(returnCode.ToString)
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetHolidays::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetShifts(ByRef oShifts As Generic.List(Of roShift), ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnText As String, Optional ByVal ShiftID As String = "") As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetShifts")
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataExport As New roApiCalendar

                returnCode = Core.DTOs.ReturnCode._UnknownError
                bResult = oDataExport.GetShifts(oShifts, strErrorMsg, returnCode, ShiftID)

                If bResult Then

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetShifts::OK")
                Else
                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetShifts::Error::" & returnCode.ToString)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetShifts")

                oParam.Add("{sObjectId}")
                oValues.Add("")

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString)
                Else
                    oValues.Add(returnCode.ToString)
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetShifts::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetPublicHolidays(ByRef oPublicHolidays As Generic.List(Of roPublicHoliday), ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnText As String) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPublicHolidays")
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataExport As New roApiCalendar

                returnCode = Core.DTOs.ReturnCode._UnknownError
                bResult = oDataExport.GetPublicHolidays(oPublicHolidays, strErrorMsg, returnCode)

                If bResult Then

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPublicHolidays::OK")
                Else
                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetPublicHolidays::Error::" & returnCode.ToString)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetPublicHolidays")

                oParam.Add("{sObjectId}")
                oValues.Add("")

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString)
                Else
                    oValues.Add(returnCode.ToString)
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetPublicHolidays::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetCalendar(ByVal oCalendarCriteria As roCalendarCriteria, ByRef oCalendarList As Generic.List(Of roCalendar), ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnText As String, Optional ByVal Timestamp As DateTime? = Nothing, Optional ByVal LoadScheduledLayers As Boolean? = False) As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oCalendarCriteria)
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetCalendar::WSCall-Parameters::" & strParameters)
                End If

                returnText = String.Empty
                Dim oDataExport As New roApiCalendar

                returnCode = Core.DTOs.ReturnCode._OK
                oCalendarList = New Generic.List(Of roCalendar)
                bResult = oDataExport.GetCalendar(oCalendarCriteria, oCalendarList, returnText, returnCode, Timestamp, LoadScheduledLayers)

                If bResult Then
                    returnCode = Core.DTOs.ReturnCode._OK

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetCalendar::OK")
                Else
                    If returnCode = Core.DTOs.ReturnCode._OK Then
                        returnCode = Core.DTOs.ReturnCode._UnknownError
                    End If

                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetCalendar::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetCalendar")

                oParam.Add("{sObjectId}")
                oValues.Add(strParameters)

                oParam.Add("{sReason}")
                oValues.Add(returnCode)
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetCalendar::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetRequests(ByVal oRequestCriteria As roDatalinkStandarRequestCriteria, ByRef oDatalinkStandarRequestResponse As roDatalinkStandarRequestResponse) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oRequestCriteria)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetRequests::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiRequests()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lRequests As New Generic.List(Of roDatalinkStandarRequest)
                bResult = oDataImport.GetRequests(oRequestCriteria, lRequests, strErrorMsg, iReturnCode)

                If bResult Then
                    oDatalinkStandarRequestResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandarRequestResponse.Requests = lRequests

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetRequests::OK::" & oRequestCriteria.NifEmpleado)
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            Case DataLinkResultEnum.InvalidEmployee
                                oDatalinkStandarRequestResponse.ResultCode = Core.DTOs.ReturnCode._InvalidEmployee
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandarRequestResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandarRequestResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandarRequestResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oRequestCriteria)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetRequests::Error::" & oDatalinkStandarRequestResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetRequests")

                oParam.Add("{sObjectId}")
                oValues.Add(oRequestCriteria.UniqueEmployeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(oDatalinkStandarRequestResponse.ResultCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandarRequestResponse))
                Else
                    oValues.Add(oDatalinkStandarRequestResponse.ResultCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                oDatalinkStandarRequestResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetRequests::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetTerminalConfiguration(ByRef oTerminalConfiguration As roTerminalConfiguration, ByVal terminalID As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnText As String) As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strParameters As String = "Details:" & terminalID

                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetTerminalConfiguration::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataExport As New roApiAccess

                returnCode = Core.DTOs.ReturnCode._UnknownError
                bResult = oDataExport.GetTerminalConfigurationById(oTerminalConfiguration, terminalID, strErrorMsg, returnCode)

                If bResult Then

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetTerminalConfiguration::OK")
                Else
                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetTerminalConfiguration::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetTerminalConfiguration")

                oParam.Add("{sObjectId}")
                oValues.Add(String.Empty)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString & " " & strParameters)
                Else
                    oValues.Add(returnCode.ToString)
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetTerminalConfiguration::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetTerminalDateTime(ByRef oTerminalDateTime As DateTime, ByVal terminalID As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnText As String) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetTerminalDateTime:: TerminalID:" & terminalID)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataExport As New roApiAccess

                returnCode = Core.DTOs.ReturnCode._UnknownError
                bResult = oDataExport.GetTerminalDateTimeById(oTerminalDateTime, terminalID, strErrorMsg, returnCode)

                If bResult Then
                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetTerminalDateTime::OK")
                Else
                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetTerminalDateTime::Error::" & returnCode.ToString & " :: " & strErrorMsg)
                    End If
                End If

            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetTerminalDateTime::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function CreateOrUpdateDailyCause(ByVal oDailyCause As roDatalinkDailyCause, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oDailyCause)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateDailyCause::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiCauses() 'stateaquí
                bResult = oDataImport.CreateOrUpdateDailyCause(oDailyCause, strErrorMsg, True)

                If bResult Then
                    returnCode = Core.DTOs.ReturnCode._OK
                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateDailyCause::OK::" & returnCode.ToString & "::" & oDailyCause.ShortCauseName)
                Else
                    Select Case oDataImport.State.Result
                        Case DataLinkResultEnum.InvalidEmployee
                            returnCode = Core.DTOs.ReturnCode._InvalidEmployee
                        Case DataLinkResultEnum.InvalidCause
                            returnCode = Core.DTOs.ReturnCode._InvalidCause
                        Case DataLinkResultEnum.InvalidContract
                            returnCode = Core.DTOs.ReturnCode._InvalidContract
                        Case DataLinkResultEnum.NoContracts
                            returnCode = Core.DTOs.ReturnCode._NoContracts
                        Case DataLinkResultEnum.FreezeDateException
                            returnCode = Core.DTOs.ReturnCode._InvalidCloseDate
                        Case DataLinkResultEnum.FutureDate
                            returnCode = Core.DTOs.ReturnCode._FutureDateTime
                        Case Else
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                    End Select

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oDailyCause)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::CreateOrUpdateDailyCause::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("CreateOrUpdateDailyCause")

                oParam.Add("{sObjectId}")
                oValues.Add(oDailyCause.ShortCauseName)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDailyCause))
                Else
                    oValues.Add(returnCode.ToString())
                End If

                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::CreateOrUpdateDailyCause::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function DeleteDailyCause(ByVal oDailyCause As roDatalinkDailyCause, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oDailyCause)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::DeleteDailyCause::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiCauses() 'stateaquí
                bResult = oDataImport.DeleteDailyCause(oDailyCause, strErrorMsg, True)

                If bResult Then
                    returnCode = Core.DTOs.ReturnCode._OK
                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::DeleteDailyCause::OK::" & returnCode.ToString & "::" & oDailyCause.ShortCauseName)
                Else
                    Select Case oDataImport.State.Result
                        Case DataLinkResultEnum.InvalidEmployee
                            returnCode = Core.DTOs.ReturnCode._InvalidEmployee
                        Case DataLinkResultEnum.InvalidCause
                            returnCode = Core.DTOs.ReturnCode._InvalidCause
                        Case DataLinkResultEnum.InvalidContract
                            returnCode = Core.DTOs.ReturnCode._InvalidContract
                        Case DataLinkResultEnum.NoContracts
                            returnCode = Core.DTOs.ReturnCode._NoContracts
                        Case DataLinkResultEnum.FreezeDateException
                            returnCode = Core.DTOs.ReturnCode._InvalidCloseDate
                        Case DataLinkResultEnum.FutureDate
                            returnCode = Core.DTOs.ReturnCode._FutureDateTime
                        Case DataLinkResultEnum.ManualCauseNotExists
                            returnCode = Core.DTOs.ReturnCode._ManualCauseNotExists
                        Case Else
                            returnCode = Core.DTOs.ReturnCode._UnknownError
                    End Select

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oDailyCause)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::DeleteDailyCause::Error::" & returnCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("CreateOrUpdateDailyCause")

                oParam.Add("{sObjectId}")
                oValues.Add(oDailyCause.ShortCauseName)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(returnCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDailyCause))
                Else
                    oValues.Add(returnCode.ToString())
                End If

                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::DeleteDailyCause::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetCauses(ByVal oCauseCriteria As roDatalinkStandarDailyCauseCriteria, ByRef oDatalinkStandarCauseResponse As roDatalinkStandarCauseResponse) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oCauseCriteria)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetCauses::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiCauses()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lCauses As New Generic.List(Of roDatalinkStandarDailyCause)
                bResult = oDataImport.GetCauses(oCauseCriteria, lCauses, strErrorMsg, iReturnCode)

                If bResult Then
                    oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandarCauseResponse.Causes = lCauses

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetCauses::OK::" & oCauseCriteria.NifEmpleado)
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            Case DataLinkResultEnum.InvalidEmployee
                                oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._InvalidEmployee
                            Case DataLinkResultEnum.InvalidCause
                                oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._InvalidCause
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandarCauseResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oCauseCriteria)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetCauses::Error::" & oDatalinkStandarCauseResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetCauses")

                oParam.Add("{sObjectId}")
                oValues.Add(oCauseCriteria.UniqueEmployeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(oDatalinkStandarCauseResponse.ResultCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandarCauseResponse))
                Else
                    oValues.Add(oDatalinkStandarCauseResponse.ResultCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetCauses::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetCausesByTimestamp(ByVal oCauseCriteria As roDatalinkStandarDailyCauseCriteria, ByRef oDatalinkStandarCauseResponse As roDatalinkStandarCauseResponse) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oCauseCriteria)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetCausesByTimestamp::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiCauses()

                Dim iReturnCode As Integer = CInt(Core.DTOs.ReturnCode._OK)
                Dim lCauses As New Generic.List(Of roDatalinkStandarDailyCause)
                bResult = oDataImport.GetCausesByTimestamp(oCauseCriteria, lCauses, strErrorMsg, iReturnCode)

                If bResult Then
                    oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    oDatalinkStandarCauseResponse.Causes = lCauses

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetCausesByTimestamp::OK::" & oCauseCriteria.NifEmpleado)
                Else
                    If iReturnCode = 0 Then
                        Select Case oDataImport.State.Result
                            Case DataLinkResultEnum.InvalidEmployee
                                oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._InvalidEmployee
                            Case DataLinkResultEnum.InvalidCause
                                oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._InvalidCause
                            Case DataLinkResultEnum.FormatColumnIsWrong
                                oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._MandatoryDataMissing
                            Case Else
                                oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                        End Select
                    Else
                        oDatalinkStandarCauseResponse.ResultCode = iReturnCode
                    End If

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(oCauseCriteria)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetCausesByTimestamp::Error::" & oDatalinkStandarCauseResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetCauses")

                oParam.Add("{sObjectId}")
                oValues.Add(oCauseCriteria.UniqueEmployeeID)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(oDatalinkStandarCauseResponse.ResultCode.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(oDatalinkStandarCauseResponse))
                Else
                    oValues.Add(oDatalinkStandarCauseResponse.ResultCode.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                oDatalinkStandarCauseResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetCausesByTimestamp::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetDocuments(ByVal documentCriteria As roDatalinkStandarDocumentCriteria, ByRef datalinkStandarDocumentlResponse As roDatalinkStandarDocumentResponse) As Boolean
            Dim bResult As Boolean = True

            Try
                If _isLogEnabled Then
                    Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(documentCriteria)
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetDocuments::WSCall-Parameters::" & strParameters)
                End If

                Dim strErrorMsg As String = String.Empty
                Dim oDataImport As New roApiDocuments()

                Dim iReturnCode As Core.DTOs.ReturnCode = Core.DTOs.ReturnCode._OK
                Dim documents As New Generic.List(Of roDocument)
                bResult = oDataImport.GetDocuments(documents, documentCriteria, strErrorMsg, iReturnCode)

                If bResult Then
                    datalinkStandarDocumentlResponse.ResultCode = Core.DTOs.ReturnCode._OK
                    datalinkStandarDocumentlResponse.ResultDetails = Core.DTOs.ReturnCode._OK.ToString()
                    datalinkStandarDocumentlResponse.Documents = documents

                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetDocuments::OK::" & documentCriteria.Title)
                Else

                    datalinkStandarDocumentlResponse.ResultCode = iReturnCode
                    datalinkStandarDocumentlResponse.ResultDetails = iReturnCode.ToString()

                    If _isLogEnabled Then
                        Dim strParameters As String = roJSONHelper.SerializeNewtonSoft(documentCriteria)
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetDocuments::Error::" & datalinkStandarDocumentlResponse.ResultCode.ToString & "::" & strParameters)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetDocuments")

                oParam.Add("{sObjectId}")
                oValues.Add(documentCriteria.Type)

                oParam.Add("{sReason}")
                If _isLogEnabled Then
                    oValues.Add(datalinkStandarDocumentlResponse.ResultDetails.ToString() & "Details:" & roJSONHelper.SerializeNewtonSoft(datalinkStandarDocumentlResponse))
                Else
                    oValues.Add(datalinkStandarDocumentlResponse.ResultDetails.ToString())
                End If
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                datalinkStandarDocumentlResponse.ResultCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetDocuments::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function
#End Region

#Region "Supervisors"
        Public Function CreateOrUpdateSupervisor(ByRef supervisor As roSupervisor) As Boolean
            Dim datalinkState As roDataLinkState
            Dim datalinkImport As New roApiSupervisors()
            Dim supervisorPassport As roPassport
            'Validate the supervisor object
            'XConvert: supervisor -> supervisorPassport
            datalinkImport.CreateOrUpdateSupervisor(supervisorPassport)
            'Mapeo de códigos de retorno
            Select Case datalinkState.Result
                'Case DataLinkResultEnum.NoError
                '    _returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                '    'supervisorPassport = New roPassport With {
                '    '    .Name = supervisor.Name,
                '    '    ...
                '    '}
                'Case ...
                '        _returnCode = ...
            End Select

            Throw New NotImplementedException("CreateOrUpdateSupervisor method is not implemented.")
        End Function

        Public Function GetAllSupervisors(ByRef oSupervisors As Generic.List(Of roSupervisor), ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnText As String) As Boolean
            Dim bResult As Boolean = True

            Try
                Dim strErrorMsg As String = String.Empty
                Dim datalinkExport As New roApiSupervisors()

                returnCode = Core.DTOs.ReturnCode._UnknownError
                bResult = datalinkExport.GetAllSupervisors(oSupervisors, strErrorMsg, returnCode)

                If bResult Then
                    returnCode = Core.DTOs.ReturnCode._OK
                    If _isLogEnabled Then oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetAllSupervisors::OK")
                Else
                    If returnCode = Core.DTOs.ReturnCode._OK Then
                        returnCode = Core.DTOs.ReturnCode._UnknownError
                    End If
                    If _isLogEnabled Then
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::GetAllSupervisors::Error::" & returnCode.ToString & "::" & strErrorMsg)
                    End If
                End If

                Dim oParam As New Generic.List(Of String)
                Dim oValues As New Generic.List(Of String)
                Dim auditAction As Audit.Action = Audit.Action.aExecuted

                oParam.Add("{sImportType}")
                oValues.Add("GetAllSupervisors")

                oParam.Add("{sObjectId}")
                oValues.Add(String.Empty)

                oParam.Add("{sReason}")
                oValues.Add(returnCode.ToString)

                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            Catch ex As Exception
                bResult = False
                returnCode = Core.DTOs.ReturnCode._UnknownError
                If _isLogEnabled Then
                    oLog.logMessage(roLog.EventType.roError, "CRoboticsExternAccess::GetAllSupervisors::Exception::", ex)
                End If
            End Try

            Return bResult
        End Function

        Public Function GetRoles(ByRef oRoles As List(Of roRole)) As Boolean
            Try
                Dim datalinkExport As New roApiSupervisors()
                Dim groupFeatures As List(Of roGroupFeature) = datalinkExport.GetRoles()
                oRoles = New List(Of roRole)()
                Dim datalinkState As roDataLinkState = datalinkExport.State

                Select Case datalinkState.Result
                    Case DataLinkResultEnum.NoError

                        oRoles = groupFeatures.ConvertAll(Of roRole)(AddressOf MapGroupFeatureToRole)
                        _returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                        Return True
                    Case Else
                        oRoles = Nothing
                        _returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._ErrorRecoveringData
                        Return False
                End Select

            Catch ex As Exception
                oRoles = Nothing
                _returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Return False
            End Try
        End Function

        Public Function GetCategories(ByRef oCategories As List(Of roCategory)) As Boolean
            Try
                Dim datalinkExport As New roApiSupervisors()
                Dim groupCategories As List(Of roSecurityCategory) = datalinkExport.GetCategories()
                Dim datalinkState As roDataLinkState = datalinkExport.State

                Select Case datalinkState.Result
                    Case DataLinkResultEnum.NoError
                        oCategories = groupCategories.ConvertAll(Of roCategory)(AddressOf MapGroupCategoryToCategory)
                        _returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                        Return True
                    Case Else
                        oCategories = Nothing
                        _returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._ErrorRecoveringData
                        Return False
                End Select

            Catch ex As Exception
                oCategories = Nothing
                _returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Return False
            End Try
        End Function

        Private Function MapGroupCategoryToCategory(groupCategory As roSecurityCategory) As roCategory
            Dim category As New roCategory()
            category.CategoryID = groupCategory.ID
            category.Name = groupCategory.Description
            Return category
        End Function

        Private Function MapGroupFeatureToRole(groupFeature As roGroupFeature) As roRole
            Dim role As New roRole()
            role.Name = groupFeature.Name
            role.ExternalId = groupFeature.ExternalId
            Return role
        End Function
#End Region

#Region "Security"
        Public Function ValidateUserNamePassword(ByVal userName As String, ByVal password As String, ByVal requestIP As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, Optional PwdEncryptwithMD5 As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            'El usuario y password no están vacios

            Dim oParam As New Generic.List(Of String)
            Dim oValues As New Generic.List(Of String)
            Dim auditAction As Audit.Action = Audit.Action.aConnect

            Dim tmpExternAccessPwd As String = Me._strExternAccessPwd

            If PwdEncryptwithMD5 Then tmpExternAccessPwd = CryptographyHelper.EncryptWithMD5(tmpExternAccessPwd)

            _oAudit.ClientAddress = requestIP

            If userName <> String.Empty AndAlso password <> String.Empty Then
                'El usuario y password són correctos
                If userName = Me._strExternAccessUserName AndAlso password = tmpExternAccessPwd Then
                    ' La ip de la petición es una ip válida
                    If EvaluateClientIP(IpsEnabled, requestIP) Then
                        bolRet = True
                        returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                    Else
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::Validation::Invalid IP origin")
                        returnCode = Core.DTOs.ReturnCode._PermissionDenied
                        auditAction = Audit.Action.aConnectFail
                    End If
                Else
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::Validation::UserPassword incorrect")
                    returnCode = Core.DTOs.ReturnCode._LoginError
                    auditAction = Audit.Action.aConnectFail
                End If
            Else
                oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::Validation::UserPassword incorrect")
                returnCode = Core.DTOs.ReturnCode._PasswordUsernameEmpty
                auditAction = Audit.Action.aConnectFail
            End If

            oParam.Add("{sUserName}")
            oValues.Add(userName)

            oParam.Add("{sReason}")
            oValues.Add(returnCode.ToString())

            If auditAction = Audit.Action.aConnectFail Then
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            End If

            Return bolRet
        End Function

        Public Shared Function GetCompanyFromToken(ByVal token As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode) As String
            Dim companyToken As String

            Try
                Dim myBase64ret As Byte() = Convert.FromBase64String(token)
                Dim myStr As String = System.Text.Encoding.UTF8.GetString(myBase64ret)

                Dim companyHash As String = myStr.Substring(0, (myStr.Length - 64))

                myBase64ret = Convert.FromBase64String(companyHash)

                companyToken = System.Text.Encoding.UTF8.GetString(myBase64ret)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTLiveApi::Could not get companyId from token::", ex)
                companyToken = String.Empty
            End Try

            If companyToken = String.Empty Then
                returnCode = Core.DTOs.ReturnCode._TokenValidation
            End If

            Return companyToken
        End Function

        Public Shared Function GetCompanyFromUserName(ByVal username As String) As String
            Dim companyName As String = String.Empty

            Try
                Dim positionSplit As Integer = username.IndexOf("\")

                If positionSplit >= 0 Then
                    companyName = username.Substring(0, positionSplit)
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTLiveApi::Could not get companyId from username::", ex)
                companyName = String.Empty
            End Try

            Return companyName
        End Function

        Public Function ValidateToken(ByVal token As String, ByVal requestIP As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, Optional PwdEncryptwithMD5 As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            'El usuario y password no están vacios

            Dim oParam As New Generic.List(Of String)
            Dim oValues As New Generic.List(Of String)
            Dim auditAction As Audit.Action = Audit.Action.aConnect

            _oAudit.ClientAddress = requestIP

            If token <> String.Empty Then
                'El usuario y password són correctos
                If token = Me._primaryToken OrElse token = _secondaryToken Then
                    ' La ip de la petición es una ip válida
                    If EvaluateClientIP(IpsEnabled, requestIP) Then
                        bolRet = True
                        returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                    Else
                        oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::ValidateToken::Invalid IP origin")
                        returnCode = Core.DTOs.ReturnCode._PermissionDenied
                        auditAction = Audit.Action.aConnectFail
                    End If
                Else
                    oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::ValidateToken::Invalid token")
                    returnCode = Core.DTOs.ReturnCode._LoginError
                    auditAction = Audit.Action.aConnectFail
                End If
            Else
                oLog.logMessage(roLog.EventType.roDebug, "CRoboticsExternAccess::Validation::Void token provided")
                returnCode = Core.DTOs.ReturnCode._PasswordUsernameEmpty
                auditAction = Audit.Action.aConnectFail
            End If

            oParam.Add("{sUserName}")
            oValues.Add("Login by token")

            oParam.Add("{sReason}")
            oValues.Add(returnCode.ToString())

            If auditAction = Audit.Action.aConnectFail Then
                Support.roLiveSupport.Audit(auditAction, Audit.ObjectType.tDatalinkWS, "DatalinkSvc", oParam, oValues, _oAudit)
            End If

            Return bolRet
        End Function

        Protected Function EvaluateClientIP(ByVal strAllowedIPs As String, ByVal strClientLocation As String) As Boolean
            Dim bIsValidLocation As Boolean = False

            If strClientLocation = "127.0.0.1" OrElse strClientLocation = "::1" Then
                bIsValidLocation = True
                Return bIsValidLocation
            End If

            Dim strOnlyAllowedAdress As String = strAllowedIPs
            Dim strListofAdress As String = ""
            If strOnlyAllowedAdress.Length > 0 Then
                For i As Integer = 0 To strOnlyAllowedAdress.Split("#").Length - 1
                    If Not strOnlyAllowedAdress.Split("#")(i).Contains(":") Then
                        strListofAdress = strListofAdress & ",@" & strOnlyAllowedAdress.Split("#")(i) & "@"
                    Else
                        Dim strRange As String = strOnlyAllowedAdress.Split("#")(i)
                        Dim strFrom As String = strRange.Split(":")(0)
                        Dim strTo As String = strRange.Split(":")(1)
                        Dim strTmp As String = ""

                        For z As Integer = 0 To strFrom.Split(".").Length - 2
                            If strTmp.Length = 0 Then
                                strTmp = strFrom.Split(".")(z)
                            Else
                                strTmp = strTmp & "." & strFrom.Split(".")(z)
                            End If
                        Next
                        For x As Integer = strFrom.Split(".")(strFrom.Split(".").Length - 1) To strTo
                            strListofAdress = strListofAdress & ",@" & strTmp & "." & x & "@"
                        Next
                    End If
                Next

                ' Si la ip esta dentro de la lista de IP validas permitimos el acceso
                If strListofAdress.Contains("@" & strClientLocation & "@") Then
                    bIsValidLocation = True
                End If
            Else
                bIsValidLocation = True
            End If

            Return bIsValidLocation
        End Function

        Private Sub addErrorToList(ByRef oPunchErrorResponse As roDatalinkStandardPunch, ByRef lstStandardPunchesResponse As roDatalinkStandardPunchResponse)
            If lstStandardPunchesResponse.PunchesListError Is Nothing Then
                lstStandardPunchesResponse.PunchesListError = New List(Of roDatalinkStandardPunch)
            End If
            lstStandardPunchesResponse.PunchesListError.Add(oPunchErrorResponse)
        End Sub

#End Region

    End Class

End Namespace