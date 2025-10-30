Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    Public Enum roIncidenceType
        <EnumMember> roITAny = 0

        <EnumMember> roITWorking = 1001
        <EnumMember> roITOverworking = 1010
        <EnumMember> roITAbsence = 1011
        <EnumMember> roITLateArrival = 1020
        <EnumMember> roITUnexpectedBreak = 1021
        <EnumMember> roITEarlyLeave = 1022
        <EnumMember> roITFlexibleOverworking = 1030
        <EnumMember> roITFlexibleUnderworking = 1031
        <EnumMember> roITBreak = 1040
        <EnumMember> roITOvertimeBreak = 1041
        <EnumMember> roITUndertimeBreak = 1042
    End Enum

    <DataContract>
    <Serializable>
    Public Class roShiftEngineLayer

        Public Shared arrayXmlKeys() As String = {"Begin", "Finish",
                                            "MaxTime", "MaxTimeAction",
                                            "MinTime", "FloatingBeginUpTo",
                                            "FloatingFinishMinutes", "Value",
                                            "Action", "Target", "MaxBreakTime",
                                            "MaxBreakAction", "MinBreakTime",
                                            "MinBreakAction", "NoPunchBreakTime",
                                            "AllowModifyIniHour", "AllowModifyDuration",
                                            "NotificationForUser", "NotificationForSupervisor",
                                            "NotificationForUserBeforeTime", "NotificationForUserAfterTime",
                                             "RealBegin", "RealFinish"}

#Region "Properties"

        <DataMember()>
        Public Property IDShift() As Integer

        <DataMember()>
        Public Property ID() As Integer

        <DataMember()>
        Public Property LabelIndex() As Integer

        <DataMember()>
        Public Property LayerType() As roLayerTypes

        <DataMember()>
        Public Property ParentID() As Integer

        'Hacer cast a rocollection
        <IgnoreDataMember()>
        Public Property Data() As Object

        <DataMember()>
        Public Property DataStoredXML() As String

        <DataMember()>
        Public Property XmlKey() As String()
            Get
                Return arrayXmlKeys
            End Get
            Set(ByVal value As String())
                arrayXmlKeys = value
            End Set
        End Property

        <DataMember()>
        Public Property ChildLayers() As Generic.List(Of roShiftEngineLayer)

        <DataMember()>
        Public Property BeginLayer() As DateTime

#End Region

    End Class

End Namespace