Imports System.IO
Imports Newtonsoft.Json

Public Class JSONHelper

    Shared Function Serialize(ByVal obj As Object) As String
        Dim serializer As System.Runtime.Serialization.Json.DataContractJsonSerializer = New System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType())
        Dim ms As MemoryStream = New MemoryStream()
        serializer.WriteObject(ms, obj)
        Dim retVal As String = Encoding.UTF8.GetString(ms.ToArray())
        Return retVal
    End Function

    Shared Function Deserialize(ByVal json As String, ByVal oType As System.Type) As Object
        Dim obj As Object = Activator.CreateInstance(Of Object)()
        ''Dim ms As MemoryStream = New MemoryStream(Encoding.Unicode.GetBytes(json))
        Dim ms As MemoryStream = New MemoryStream(Encoding.Unicode.GetBytes(json))
        'Dim serializer As System.Runtime.Serialization.Json.DataContractJsonSerializer = New System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType())
        Dim serializer As System.Runtime.Serialization.Json.DataContractJsonSerializer = New System.Runtime.Serialization.Json.DataContractJsonSerializer(oType)
        obj = CObj(serializer.ReadObject(ms))
        ms.Close()
        Return obj
    End Function

    Shared Function DeserializeNewtonSoft(ByVal json As String, ByVal oType As System.Type) As Object

        Dim obj As Object = Activator.CreateInstance(Of Object)()
        Dim textReader As TextReader = New StringReader(json)
        Dim serializer As New JsonSerializer

        serializer.NullValueHandling = NullValueHandling.Ignore
        serializer.MissingMemberHandling = MissingMemberHandling.Ignore

        obj = serializer.Deserialize(textReader, oType)

        Return obj
    End Function

    Shared Function SerializeNewtonSoft(ByVal obj As Object) As String

        Dim serializer As New JsonSerializer
        serializer.NullValueHandling = NullValueHandling.Ignore
        serializer.MissingMemberHandling = MissingMemberHandling.Ignore

        Dim sb As New StringBuilder()
        Dim sw As New StringWriter(sb)

        Dim jsonWriter As New JsonTextWriter(sw)

        serializer.Serialize(jsonWriter, obj)

        Return sw.ToString()
    End Function

End Class