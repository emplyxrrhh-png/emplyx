Imports System.Runtime.Serialization

Namespace DTOs

    ''' <summary>
    ''' Representa toda la información de un campo de la ficha de empleado
    ''' </summary>
    <DataContract()>
    Public Class EmployeeUserField
        ''' <summary>
        ''' Nombre del campo de la ficha
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Name As String
        ''' <summary>
        ''' Descripción del campo de la ficha
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Caption As String
        ''' <summary>
        ''' Nombre del campo de la ficha
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Description As String
        ''' <summary>
        ''' Descripción detallada del campo de la ficha
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Value As String
        ''' <summary>
        ''' Valor del campo de la ficha
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ValueDate As Date
        ''' <summary>
        ''' Tipo del campo de la ficha, donde 0=Texto, 1=Numérico, 2=Fecha, 3=Decimal, 4=Hora, 5=Lista de valores, 6=Periodo de fechas, 7=Periodo de horas, 8=Link a documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Type As Integer
        ''' <summary>
        ''' Formato de campo de la ficha para campos de tipo fecha, hora, periodo de fechas y periodo de horas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TypeFormat As String
        ''' <summary>
        ''' Nivel de acceso necesario para poder ver el campo (LOPD), donde 0=Bajo, 1=Medio y 2=Alto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AccessLevel As Integer
        ''' <summary>
        ''' Indica si el campo de la ficha tiene un histórico de valores
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property HasHistory As Boolean

        ''' <summary>
        ''' Indica si el campo de la ficha tiene un histórico de valores
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CanDeliverDocument As Boolean

    End Class

    ''' <summary>
    ''' Representa una categoría de la ficha de empleado, conteniendo el nombre de dicha categoría y los campos de la ficha de dicha categoría
    ''' </summary>
    <DataContract()>
    Public Class Category
        ''' <summary>
        ''' Nombre de la categoría
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property key As String
        ''' <summary>
        ''' Array de objetos campo de la ficha (EmployeeUserField)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property items As EmployeeUserField()
    End Class

    ''' <summary>
    ''' Representa el conjunto de campo de la ficha de un empleado, agrupado por sus categorías
    ''' </summary>
    <DataContract()>
    Public Class UserFields
        ''' <summary>
        ''' Array de categorías de la ficha
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Categories As Category()
        ''' <summary>
        ''' Permiso del empleado para solicitar cambios en valores de la ficha
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CanCreateRequest As Boolean
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        <DataMember()>
        Public Status As Long
    End Class

    ''' <summary>
    ''' Representa el conjunto de posibles valores de un campo de la ficha de tipo Lista
    ''' </summary>
    <DataContract()>
    Public Class FieldValues
        ''' <summary>
        ''' Array con los valores posibles de un campo de la ficha de tipo Lista
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Values As String()
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        <DataMember()>
        Public Status As Long
    End Class

End Namespace