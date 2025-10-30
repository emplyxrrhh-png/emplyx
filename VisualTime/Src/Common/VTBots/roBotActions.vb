Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBots
Imports Robotics.Base.VTUserFields
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports System.Data.Common

Public Class roBotActions

    Public Shared Function SetEmployeeUserFields(ByVal IDEmployee As Integer, ByVal Feature As String, ByVal FromEmployee As Integer, ByRef oState As roBotRuleState) As Boolean
        Dim bolRet As Boolean = False
        Dim strSQL As String = ""

        Try

            Dim oFromPassport As roPassport = Nothing

            Try
                Dim oPassportManager As New roPassportManager
                oFromPassport = oPassportManager.LoadPassport(FromEmployee, LoadType.Employee)
            Catch ex As Exception
                oFromPassport = Nothing
            End Try

            If oFromPassport Is Nothing Then
                Return True
            End If

            Dim idSystemPassport As Integer = roTypes.Any2Integer(roConstants.GetSystemUserId())
            Dim oPassport As roPassport = Nothing

            Dim bHasPermission = oState.IDPassport = idSystemPassport OrElse WLHelper.HasFeaturePermissionByEmployee(oState.IDPassport, Feature, Permission.Write, IDEmployee)
            Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()

            If bHasPermission AndAlso IDEmployee <> FromEmployee Then
                Try
                    Dim oPassportManager As New roPassportManager
                    oPassport = oPassportManager.LoadPassport(IDEmployee, LoadType.Employee)
                Catch ex As Exception
                    oPassport = Nothing
                End Try

                If oPassport IsNot Nothing Then
                    bolRet = True
                    Dim oUserFields As Generic.List(Of roEmployeeUserField) = Robotics.Base.VTUserFields.UserFields.roEmployeeUserField.GetUserFieldsList(FromEmployee, Now.Date, New Robotics.Base.VTUserFields.UserFields.roUserFieldState())
                    For Each userField In oUserFields
                        'Si el campo no es de sistema y no es el NIF, intentamos copiar el userfield
                        If Not userField.Definition.isSystem AndAlso userField.FieldName.ToUpper <> "NIF" AndAlso Not userField.Definition.Unique Then
                            Dim newUserfield = New roEmployeeUserField(IDEmployee, userField.FieldName, userField._Date, New Robotics.Base.VTUserFields.UserFields.roUserFieldState(oState.IDPassport), False)
                            'Si el usuario destino no tiene valor, le asignamos el del Source
                            If newUserfield.FieldValue Is Nothing Then
                                Dim fromUserFieldValue = userField.FieldValue
                                If fromUserFieldValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(roTypes.Any2String(fromUserFieldValue)) Then
                                    newUserfield.FieldValue = fromUserFieldValue
                                    bolRet = newUserfield.Save()

                                    If Not bolRet Then
                                        Exit For
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
            End If

            Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)

            'Auditoría
            If bolRet Then
                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                oState.AddAuditParameter(tbParameters, "{EmployeeName}", oPassport.Name, "", 1)
                oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tPassport, "", tbParameters, -1)

            End If
        Catch ex As DbException
            bolRet = False
            oState.UpdateStateInfo(ex, "roEmployee::SetEmployeeUserFields")
        End Try
        Return bolRet
    End Function

End Class
