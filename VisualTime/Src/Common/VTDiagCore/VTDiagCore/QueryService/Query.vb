Option Strict On

Imports System.IO
Imports System.Xml.Serialization

Namespace VTDiagCore.QueryService

    Public Class Query
        Public Property Id As Integer
        Public Property Name As String
        Public Property Description As String
        Public Property Value As String
        Public Property Parameters As List(Of Parameter)

        Private Shared parametersSerializer As XmlSerializer = New XmlSerializer(GetType(List(Of Parameter)))

        Public Shared Function FromDataRow(row As DataRow) As Query
            Return New Query With {
                .Id = CInt(row("Id")),
                .Name = CStr(row("Name")),
                .Description = CStr(row("Description")),
                .Value = CStr(row("Value")),
                .Parameters = (Function(data As Object) As List(Of Parameter)
                                   If (IsDBNull(data)) OrElse String.IsNullOrWhiteSpace(CStr(data)) Then Return New List(Of Parameter)
                                   Return CType(parametersSerializer.Deserialize(New StringReader(CStr(data))), List(Of Parameter))
                               End Function)(row("Parameters"))
            }
        End Function

    End Class

End Namespace