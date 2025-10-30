Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class roJsonParser
    Public Function UnflattenJsonFile(filePath As String) As String
        Dim json As String = File.ReadAllText(filePath)
        Dim flattenedObject As JObject = JObject.Parse(json)

        Dim result As New JObject()

        For Each pair In flattenedObject
            Dim keys As String() = pair.Key.Split(New String() {"@@"}, StringSplitOptions.None)
            UnflattenJsonObject(result, keys, pair.Value.ToString(), 0)
        Next

        Dim resultJson As String = result.ToString()

        Return resultJson
    End Function

    Private Sub UnflattenJsonObject(jsonObject As JObject, keys As String(), value As String, index As Integer)
        Dim key As String = keys(index)

        If key.StartsWith("RES_") Then key = key.Substring(4)

        If index < keys.Length - 1 Then
            If Not jsonObject.ContainsKey(key) Then
                jsonObject.Add(key, New JObject())
            End If
            UnflattenJsonObject(jsonObject(key), keys, value, index + 1)
        Else
            jsonObject.Add(key, value)
        End If
    End Sub

    Public Function FlattenJsonFile(filePath As String) As String
        Dim json As String = File.ReadAllText(filePath)
        Dim jsonObject As JObject = JObject.Parse(json)


        Dim result As New Dictionary(Of String, String)

        For Each pair In jsonObject
            If TypeOf pair.Value Is JObject Then
                FlattenJsonObject(pair.Value, result, $"{pair.Key}")
            Else
                result($"RES_{pair.Key}") = pair.Value.ToString()
            End If
        Next

        Dim resultJson As String = JsonConvert.SerializeObject(result, Formatting.Indented)

        Return resultJson
    End Function

    Private Sub FlattenJsonObject(jsonObject As JObject, result As Dictionary(Of String, String), prefix As String)
        For Each pair In jsonObject
            If TypeOf pair.Value Is JObject Then
                FlattenJsonObject(pair.Value, result, $"{prefix}@@{pair.Key}")
            Else
                result($"RES_{prefix}@@{pair.Key}") = pair.Value.ToString()
            End If
        Next
    End Sub


End Class
