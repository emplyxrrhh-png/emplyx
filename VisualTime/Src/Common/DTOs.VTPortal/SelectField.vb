Imports System.Runtime.Serialization

Namespace DTOs

    ''' <summary>
    ''' Representa un elemento genérico de una lista de elementos que se usará para cargar desplegables (combos)
    ''' </summary>
    <DataContract()>
    Public Class SelectField
        ''' <summary>
        ''' Nombre del elemento de la lista
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FieldName As String
        ''' <summary>
        ''' Valor del elemento de la lista
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FieldValue As String
        ''' <summary>
        ''' Información relacionada con el objeto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RelatedInfo As String

        Public Sub New()
            FieldName = String.Empty
            FieldValue = String.Empty
            RelatedInfo = String.Empty
        End Sub

        Public Sub New(name As String, value As String)
            FieldName = name
            FieldValue = value
            RelatedInfo = ""
        End Sub
    End Class

    ''' <summary>
    ''' Representa una lista genérica de elementos que se usará para cargar desplegables (combos)
    ''' </summary>
    <DataContract()>
    Public Class GenericList
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Status As Long
        ''' <summary>
        ''' Array de elementos de una lista
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SelectFields As SelectField()
    End Class

End Namespace