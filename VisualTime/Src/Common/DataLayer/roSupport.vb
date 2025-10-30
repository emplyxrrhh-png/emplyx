Imports System.Reflection
Imports System.Text.RegularExpressions
Imports Robotics.Azure
Imports Robotics.VTBase
Imports System.Xml
Imports Newtonsoft.Json.Linq


Public Class roSupport


    Private Shared Function XSSSanitize(text As String, xssRegex As String) As String
        If String.IsNullOrEmpty(text) Then Return text

        Return Regex.Replace(text, xssRegex, "")
    End Function


    Public Shared Function IsXSSSafe(source As Object) As Boolean

        Try
            Dim htmlProperties As String = "rocommunique.message#roterminal.confdata#roterminal.data#rointeractiveconfig.printertext#rochannel.privacypolicy"

            Dim enableValidation = roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "Global.ValidateTextIntegrity")
            If enableValidation <> String.Empty AndAlso Not roTypes.Any2Boolean(enableValidation) Then Return True

            Dim xssRegex As String = roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "Global.ValidateTextRegex")
            If xssRegex.Trim = String.Empty Then xssRegex = "<[^>]*\s*\bon\w+\s*=|<[^>|~]+>|'[^>|~]*[<>]"


            Dim htmlPropertiesExtra = roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "Global.HtmlProperties")

            If htmlPropertiesExtra <> String.Empty Then htmlProperties = $"{htmlProperties}#{htmlPropertiesExtra}"


            Return roSupport.IsXSSSafeVT(source, xssRegex, htmlProperties)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupport::isXSSSafe::", ex)
        End Try
    End Function

    Private Shared Function IsXMLXSSSafe(source As String, xssRegex As String, allowedHtmlProperties As String) As Boolean
        If source = String.Empty Then Return True

        Dim isValidXML As Boolean = True

        Dim xmlDoc As New XmlDocument()
        Try
            xmlDoc.LoadXml(source)
        Catch ex As Exception
            isValidXML = True
        End Try

        If isValidXML Then

            Dim itemNodes As XmlNodeList = xmlDoc.SelectNodes("//Item[@type]")

            For Each itemNode As XmlNode In itemNodes
                Dim typeAttribute As String = itemNode.Attributes("type").Value
                Dim nodeValue As String = itemNode.InnerText
                If roTypes.Any2Integer(typeAttribute) = 8 AndAlso Not roSupport.IsXSSSafeVT(nodeValue, xssRegex, allowedHtmlProperties) Then Return False
            Next

        Else
            Return False
        End If

        Return True
    End Function

    Private Shared Function IsJSONXSSSafe(source As String, xssRegex As String, allowedHtmlProperties As String) As Boolean
        If source = String.Empty Then Return True

        Try

            If source.StartsWith("[{") Then
                Dim jsonArray As JArray = JArray.Parse(source)

                For Each jsonObject As JObject In jsonArray.Children()
                    ' Iterar sobre todas las propiedades del JSON
                    For Each jsonProperty As JProperty In jsonObject.Properties()
                        If jsonProperty.Value.Type = JTokenType.String Then
                            Dim propertyValue As String = jsonProperty.Value.ToString()
                            If Not IsXSSSafeVT(propertyValue, xssRegex, allowedHtmlProperties) Then
                                Return False
                            End If
                        End If
                    Next
                Next
            Else
                Dim jsonObject As JObject = JObject.Parse(source)

                ' Iterar sobre todas las propiedades del JSON
                For Each jsonProperty As JProperty In jsonObject.Properties()
                    If jsonProperty.Value.Type = JTokenType.String Then
                        Dim propertyValue As String = jsonProperty.Value.ToString()
                        If Not IsXSSSafeVT(propertyValue, xssRegex, allowedHtmlProperties) Then
                            Return False
                        End If
                    End If
                Next
            End If



        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupport::IsJSONXSSSafe::", ex)
            Return False
        End Try


        Return True
    End Function

    Private Shared Function IsXSSSafeVT(source As Object, xssRegex As String, allowedHtmlProperties As String) As Boolean

        Try
            If source Is Nothing OrElse IsDBNull(source) Then Return True

            Dim sourceType As Type = source.GetType()

            If sourceType = GetType(String) Then
                ' Recibo como parámetro un string
                Dim sourceValue As String = String.Empty
                If Not String.IsNullOrEmpty(source) Then sourceValue = source.ToString().Trim()

                Dim sanitizedValue As String = roSupport.XSSSanitize(sourceValue, xssRegex)
                If sourceValue.ToLower().StartsWith("<?xml") OrElse sourceValue.ToLower().StartsWith("<localdataset") Then
                    Return IsXMLXSSSafe(sourceValue, xssRegex, allowedHtmlProperties)
                ElseIf (sourceValue.ToLower().StartsWith("{") AndAlso sourceValue.ToLower().EndsWith("}")) OrElse
                    (sourceValue.ToLower().StartsWith("[{") AndAlso sourceValue.ToLower().EndsWith("}]")) Then
                    Return IsJSONXSSSafe(sourceValue, xssRegex, allowedHtmlProperties)
                ElseIf sourceValue.Length <> sanitizedValue.Length Then
                    Return False
                End If
            ElseIf GetType(IEnumerable).IsAssignableFrom(sourceType) Then
                ' Recibo como parámetro un enumerable de 'algo'
                For Each item As Object In CType(source, IEnumerable)
                    If Not roSupport.IsXSSSafeVT(item, xssRegex, allowedHtmlProperties) Then Return False
                Next
            ElseIf sourceType = GetType(DataSet) Then
                Dim propertyValue As DataSet = CType(source, DataSet)
                For Each dataTable As DataTable In propertyValue.Tables
                    If Not roSupport.IsXSSSafeVT(dataTable, xssRegex, allowedHtmlProperties) Then Return False
                Next

            ElseIf sourceType = GetType(DataTable) Then
                Dim propertyValue As DataTable = CType(source, DataTable)

                For Each row As DataRow In propertyValue.Rows
                    If Not roSupport.IsXSSSafeVT(row, xssRegex, allowedHtmlProperties) Then Return False
                Next
            ElseIf sourceType = GetType(DataRow) Then
                Dim propertyValue As DataRow = CType(source, DataRow)

                For Each oValue As Object In propertyValue.ItemArray
                    If Not roSupport.IsXSSSafeVT(oValue, xssRegex, allowedHtmlProperties) Then Return False
                Next
            Else
                ' Obtener todas las propiedades del objeto
                Dim properties As New Generic.List(Of PropertyInfo)

                If sourceType.GetProperties().Length > 0 Then
                    properties.AddRange(sourceType.GetProperties())
                    properties = properties.FindAll(
                    Function(x)
                        Return ((x.PropertyType = GetType(String)) OrElse (Not x.PropertyType.IsPrimitive AndAlso Not x.PropertyType.IsValueType)) AndAlso
                                                    Not x.PropertyType.IsEnum
                    End Function)
                End If

                ' Iterar sobre cada propiedad
                For Each propiedad As PropertyInfo In properties
                    ' Obtener el valor de la propiedad
                    Dim propertyValue As Object = Nothing
                    If propiedad.GetIndexParameters().Length = 0 Then propertyValue = propiedad.GetValue(source, Nothing)


                    If allowedHtmlProperties.Contains((propiedad.ReflectedType.Name & "." & propiedad.Name).ToLower()) Then Return True

                    If Not roSupport.IsXSSSafeVT(propertyValue, xssRegex, allowedHtmlProperties) Then Return False
                Next
            End If

        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roSupport::IsXSSSafeVT::", ex)
        End Try

        Return True
    End Function




End Class