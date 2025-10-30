Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum TaskStatusEnum
        <EnumMember> _ON = 0         ' Activa
        <EnumMember> _COMPLETED = 1  ' Completada
        <EnumMember> _CANCELED = 2   '  Cancelada
        <EnumMember> _PENDING = 3   '  Penidente de confirmar
    End Enum

    <DataContract>
    Public Enum TaskTypeCollaborationEnum
        <EnumMember> _ANY = 0        ' Cualquiera que este autorizado
        <EnumMember> _THEFIRST = 1   ' Solo el primero que fiche de los empleados autorizados
    End Enum

    <DataContract>
    Public Enum TaskModeCollaborationEnum
        <EnumMember> _ALLTHESAMETIME = 0     ' Simultaneamente todas las personas autorizadas
        <EnumMember> _ONEPERSONATTIME = 1    ' Una persona a la vez
    End Enum

    <DataContract>
    Public Enum TaskTypeActivationEnum
        <EnumMember> _ALWAYS = 0         ' Siempre activa
        <EnumMember> _ATDATE = 1         ' Activa en una fecha
        <EnumMember> _ATFINISHTASK = 2   ' Al finalizar otra tarea
        <EnumMember> _ATRUNTASK = 3      ' Al iniciar otra tarea
    End Enum

    <DataContract>
    Public Enum TaskTypeClosingEnum
        <EnumMember> _UNDEFINED = 0  ' Nunca se cierra
        <EnumMember> _ATDATE = 1     ' Se cierra a una fecha concreta
    End Enum

    <DataContract>
    Public Enum TaskTypeAuthorizationEnum
        <EnumMember> _ANYEMPLOYEE = 0        ' Cualquier empleado
        <EnumMember> _SELECTEDEMPLOYEES = 1   ' Empleados seleccionados
        <EnumMember> _ASSIGNMENTEMPLOYEES = 2         ' Empleados que cubran un puesto concreto
    End Enum

    <DataContract>
    Public Enum TaskStatisticsViewEnum
        <EnumMember> _WorkingTime = 0 ' Tiempo Invertido
        <EnumMember> _Diversions = 1 ' Desvios
    End Enum

    <DataContract>
    Public Enum TaskStatisticsGroupByEnum
        <EnumMember> _ByEmployee = 0 ' por Empleado
        <EnumMember> _ByDate = 1 ' por Fecha
        <EnumMember> _ByGroup = 2 ' por Grupo
    End Enum

    'Public Enum TaskTypeCollaborationEnum
    '    _ANY = 0        ' Cualquiera que este autorizado
    '    _THEFIRST = 1   ' Solo el primero que fiche de los empleados autorizados
    'End Enum

    'Public Enum TaskModeCollaborationEnum
    '    _ALLTHESAMETIME = 0     ' Simultaneamente todas las personas autorizadas
    '    _ONEPERSONATTIME = 1    ' Una persona a la vez
    'End Enum

    'Public Enum TaskTypeActivationEnum
    '    _ALWAYS = 0         ' Siempre activa
    '    _ATDATE = 1         ' Activa en una fecha
    '    _ATFINISHTASK = 2   ' Al finalizar otra tarea
    '    _ATRUNTASK = 3      ' Al iniciar otra tarea
    'End Enum
    'Public Enum TaskTypeClosingEnum
    '    _UNDEFINED = 0  ' Nunca se cierra
    '    _ATDATE = 1     ' Se cierra a una fecha concreta
    'End Enum

    'Public Enum TaskTypeAuthorizationEnum
    '    _ANYEMPLOYEE = 0        ' Cualquier empleado
    '    _SELECTEDEMPLOYEES = 1   ' Empleados seleccionados
    '    _ASSIGNMENTEMPLOYEES = 2         ' Empleados que cubran un puesto concreto
    'End Enum

    'Public Enum TaskStatisticsViewEnum
    '    _WorkingTime = 0 ' Tiempo Invertido
    '    _Diversions = 1 ' Desvios
    'End Enum
    'Public Enum TaskStatisticsGroupByEnum
    '    _ByEmployee = 0 ' por Empleado
    '    _ByDate = 1 ' por Fecha
    '    _ByGroup = 2 ' por Grupo
    'End Enum

End Namespace