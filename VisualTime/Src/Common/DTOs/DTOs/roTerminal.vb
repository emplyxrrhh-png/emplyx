Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum ScopeMode
        <EnumMember> ACCTAJOBTSKEIPDINCO
        <EnumMember> ACCTAJOBTSKEIPDIN
        <EnumMember> ACCTAJOBTSKEIPCO
        <EnumMember> ACCTAJOBTSKEIP
        <EnumMember> ACCTAJOBTSKDINCO
        <EnumMember> ACCTAJOBTSKDIN
        <EnumMember> ACCTAJOBTSKCO
        <EnumMember> ACCTAJOBTSK
        <EnumMember> ACCTAJOBEIPDINCO
        <EnumMember> ACCTAJOBEIPDIN
        <EnumMember> ACCTAJOBEIPCO
        <EnumMember> ACCTAJOBEIP
        <EnumMember> ACCTAJOBDINCO
        <EnumMember> ACCTAJOBDIN
        <EnumMember> ACCTAJOBCO
        <EnumMember> ACCTAJOB
        <EnumMember> ACCTATSKEIPDINCO
        <EnumMember> ACCTATSKEIPDIN
        <EnumMember> ACCTATSKEIPCO
        <EnumMember> ACCTATSKEIP
        <EnumMember> ACCTATSKDINCO
        <EnumMember> ACCTATSKDIN
        <EnumMember> ACCTATSKCO
        <EnumMember> ACCTATSK
        <EnumMember> ACCTAEIPDINCO
        <EnumMember> ACCTAEIPDIN
        <EnumMember> ACCTAEIPCO
        <EnumMember> ACCTAEIP
        <EnumMember> ACCTADINCO
        <EnumMember> ACCTADIN
        <EnumMember> ACCTACO
        <EnumMember> ACCTA
        <EnumMember> ACCJOBTSKEIPDINCO
        <EnumMember> ACCJOBTSKEIPDIN
        <EnumMember> ACCJOBTSKEIPCO
        <EnumMember> ACCJOBTSKEIP
        <EnumMember> ACCJOBTSKDINCO
        <EnumMember> ACCJOBTSKDIN
        <EnumMember> ACCJOBTSKCO
        <EnumMember> ACCJOBTSK
        <EnumMember> ACCJOBEIPDINCO
        <EnumMember> ACCJOBEIPDIN
        <EnumMember> ACCJOBEIPCO
        <EnumMember> ACCJOBEIP
        <EnumMember> ACCJOBDINCO
        <EnumMember> ACCJOBDIN
        <EnumMember> ACCJOBCO
        <EnumMember> ACCJOB
        <EnumMember> ACCTSKEIPDINCO
        <EnumMember> ACCTSKEIPDIN
        <EnumMember> ACCTSKEIPCO
        <EnumMember> ACCTSKEIP
        <EnumMember> ACCTSKDINCO
        <EnumMember> ACCTSKDIN
        <EnumMember> ACCTSKCO
        <EnumMember> ACCTSK
        <EnumMember> ACCEIPDINCO
        <EnumMember> ACCEIPDIN
        <EnumMember> ACCEIPCO
        <EnumMember> ACCEIP
        <EnumMember> ACCDINCO
        <EnumMember> ACCDIN
        <EnumMember> ACCCO
        <EnumMember> ACC
        <EnumMember> TAJOBTSKEIPDINCO
        <EnumMember> TAJOBTSKEIPDIN
        <EnumMember> TAJOBTSKEIPCO
        <EnumMember> TAJOBTSKEIP
        <EnumMember> TAJOBTSKDINCO
        <EnumMember> TAJOBTSKDIN
        <EnumMember> TAJOBTSKCO
        <EnumMember> TAJOBTSK
        <EnumMember> TAJOBEIPDINCO
        <EnumMember> TAJOBEIPDIN
        <EnumMember> TAJOBEIPCO
        <EnumMember> TAJOBEIP
        <EnumMember> TAJOBDINCO
        <EnumMember> TAJOBDIN
        <EnumMember> TAJOBCO
        <EnumMember> TAJOB
        <EnumMember> TATSKEIPDINCO
        <EnumMember> TATSKEIPDIN
        <EnumMember> TATSKEIPCO
        <EnumMember> TATSKEIP
        <EnumMember> TATSKDINCO
        <EnumMember> TATSKDIN
        <EnumMember> TATSKCO
        <EnumMember> TATSK
        <EnumMember> TAEIPDINCO
        <EnumMember> TAEIPDIN
        <EnumMember> TAEIPCO
        <EnumMember> TAEIP
        <EnumMember> TADINCO
        <EnumMember> TADIN
        <EnumMember> TACO
        <EnumMember> TA
        <EnumMember> JOBTSKEIPDINCO
        <EnumMember> JOBTSKEIPDIN
        <EnumMember> JOBTSKEIPCO
        <EnumMember> JOBTSKEIP
        <EnumMember> JOBTSKDINCO
        <EnumMember> JOBTSKDIN
        <EnumMember> JOBTSKCO
        <EnumMember> JOBTSK
        <EnumMember> JOBEIPDINCO
        <EnumMember> JOBEIPDIN
        <EnumMember> JOBEIPCO
        <EnumMember> JOBEIP
        <EnumMember> JOBDINCO
        <EnumMember> JOBDIN
        <EnumMember> JOBCO
        <EnumMember> JOB
        <EnumMember> TSKEIPDINCO
        <EnumMember> TSKEIPDIN
        <EnumMember> TSKEIPCO
        <EnumMember> TSKEIP
        <EnumMember> TSKDINCO
        <EnumMember> TSKDIN
        <EnumMember> TSKCO
        <EnumMember> TSK
        <EnumMember> EIPDINCO
        <EnumMember> EIPDIN
        <EnumMember> EIPCO
        <EnumMember> EIP
        <EnumMember> DINCO
        <EnumMember> DIN
        <EnumMember> CO
        <EnumMember> UNDEFINED
        <EnumMember> TAACC
    End Enum

    <DataContract>
    Public Enum InteractionMode
        <EnumMember> Blind
        <EnumMember> Fast
        <EnumMember> Interactive
    End Enum

    <DataContract>
    Public Enum InteractionAction
        <EnumMember> E
        <EnumMember> S
        <EnumMember> X
        <EnumMember> ES
    End Enum

    <DataContract>
    Public Enum ValidationMode
        <EnumMember> None
        <EnumMember> Local
        <EnumMember> Server
        <EnumMember> LocalServer
        <EnumMember> ServerLocal
        <EnumMember> LocalServerPesimistic
    End Enum

    <DataContract>
    Public Enum TerminalRegitrationState
        <EnumMember> DoesNotExists
        <EnumMember> ExistsButNotConfigured
        <EnumMember> ExistsAndConfigured
    End Enum

End Namespace