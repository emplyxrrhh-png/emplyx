Imports System.Runtime.Serialization

Namespace DTOs

    ''' <summary>
    ''' Representa una abstracción de una imagen, codificada en Base64 para su envío de o desde el dispositivo
    ''' </summary>
    <DataContract()>
    Public Class WsImage
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Status As Long

        ''' <summary>
        ''' Cadena en Base64 que representa una imagen
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Base64StringContent As String

    End Class

    ''' <summary>
    '''
    ''' </summary>
    <DataContract()>
    Public Class WsView
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Status As Long

        ''' <summary>
        ''' Cadena con el contenido de la vista
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property viewContent As String

        ''' <summary>
        ''' Cadena con el contenido del javascript de la vista
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property jsContent As String
    End Class

End Namespace