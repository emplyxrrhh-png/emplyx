Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs
Imports SwaggerWcf.Attributes

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    Public Enum VSL_WorkSheetsExcelColumns
        EmpleadoNombre
        Dia
        Proyecto
        Dieta
        KM
        PA
        HN
        HE
        HF
        HS
        EmpleadoID
        Parte
    End Enum

    Public Interface IDatalinkVSL_WorkSheets

        Function GetEmployeeColumnsDefinition(ByRef ColumnsVal As String(), ByRef ColumnsUsrName As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

End Namespace