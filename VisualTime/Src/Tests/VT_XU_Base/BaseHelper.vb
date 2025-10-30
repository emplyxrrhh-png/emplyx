Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.VTBase

Public Class BaseHelper

    Public Property AuditWasCalled As Boolean
    Public Property SendPushNotificationWasCalled As Boolean = False

    Public Property RecalcPunchesCalled As Boolean = False

    Public Sub New()
        AuditWasCalled = False
    End Sub

    Public Function AuditSpy(auditAction As VTBase.Audit.Action, auditObjectType As VTBase.Audit.ObjectType)
        Fakes.ShimroBusinessState.AllInstances.AuditActionObjectTypeStringDataTableInt32 =
                        Function(ostate As roBusinessState, _Action As VTBase.Audit.Action, _ObjectType As VTBase.Audit.ObjectType, _ObjectName As String, _Parameters As DataTable, _SessionID As Integer)
                            AuditWasCalled = (_Action = auditAction AndAlso _ObjectType = auditObjectType)
                            Return True
                        End Function
    End Function


    Public Sub InitJSONSerializers()
        VTBase.Fakes.ShimroJSONHelper.ToGeniusJSONStringDataRowListOfroLayoutDescriptionUserFieldPropertiesArrayDictionaryOfStringStringDictionaryOfInt32StringDictionaryOfInt32StringDictionaryOfInt32StringDictionaryOfInt32StringDictionaryOfInt32StringDictionaryOfInt32StringDictionaryOfStringSt = Function()
                                                                                                                                                                                                                                                                                                             Return "test"
                                                                                                                                                                                                                                                                                                         End Function

        VTBase.Fakes.ShimroJSONHelper.ToGeniusJSONDefinitionStringDataTableListOfroLayoutDescriptionUserFieldPropertiesArray = Function()
                                                                                                                                   Return "test"
                                                                                                                               End Function
    End Sub

    Public Sub InitLanguage()

        Robotics.VTBase.Fakes.ShimroLanguage.Constructor = Function(a As roLanguage)
                                                               Return a
                                                           End Function

        Robotics.VTBase.Fakes.ShimroLanguage.AllInstances.TranslateStringStringBooleanString = Function(a As Robotics.VTBase.roLanguage, key As String, value As String, b As Boolean, c As String) value
    End Sub



    Public Function RecalcPunchesSpy()
        Robotics.VTBase.Extensions.Fakes.ShimroConnector.InitTaskTasksTyperoCollection =
            Function(oTaskType, oCollection)
                If oTaskType = TasksType.MOVES Then
                    RecalcPunchesCalled = True
                End If
                Return True
            End Function
    End Function

    Public Function SendNotificationPushToPassport()
        Robotics.VTBase.Extensions.Fakes.ShimroPushNotification.SendNotificationPushToPassportInt64LoadTypeStringString = Function(idUser As Long, passportType As LoadType, title As String, content As String)
                                                                                                                              SendPushNotificationWasCalled = True
                                                                                                                              Return True
                                                                                                                          End Function
    End Function
End Class