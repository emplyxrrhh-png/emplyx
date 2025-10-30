Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class roGeniusExecution
        ''' <summary>
        ''' Identificador único de la analítica
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Id As Integer
        ''' <summary>
        ''' Nombre abreviado del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdGeniusView As String
        ''' <summary>
        ''' Nombre abreviado del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdTask As String
        ''' <summary>
        ''' Descripción del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property FileLink As String
        ''' <summary>
        ''' Tipo de Horario: De trabajo, Baja, Vacaciones o Descanso
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property CubeLayout As String
        ''' <summary>
        ''' Color del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ExecutionDate As DateTime

        <DataMember>
        Public Property AzureSaSKey As String

        <DataMember>
        Public Property FileSize As Integer

        <DataMember>
        Public Property SASLink As String
        <DataMember>
        Public Property ExecutionLanguage As String

    End Class

End Namespace