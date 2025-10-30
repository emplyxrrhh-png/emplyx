Imports DevExpress.XtraTreeList
Imports Newtonsoft.Json
Imports Robotics.VTBase
Imports System.IO
Imports System.Net.Mime
Imports System.Reflection
Imports System.Web.Mvc

Public Class IsoDateJsonResult
    Inherits ActionFilterAttribute

    Private ReadOnly _romanceTimeZone As TimeZoneInfo

    Public Overrides Sub OnActionExecuting(filterContext As ActionExecutingContext)
        ' Procesar todos los parámetros de acción
        For Each paramKey In filterContext.ActionParameters.Keys.ToList()
            Dim paramValue = filterContext.ActionParameters(paramKey)

            ' Convertir el parámetro si es una fecha o contiene fechas
            If paramValue IsNot Nothing Then
                ' Si el parámetro es directamente un DateTime
                If TypeOf paramValue Is DateTime Then
                    filterContext.ActionParameters(paramKey) = ConvertToRomanceTime(DirectCast(paramValue, DateTime))
                Else
                    ' Si es un objeto complejo, procesarlo recursivamente
                    ProcessComplexObject(paramValue)
                End If
            End If
        Next

        MyBase.OnActionExecuting(filterContext)
    End Sub

    Public Overrides Sub OnActionExecuted(filterContext As ActionExecutedContext)
        If TypeOf filterContext.Result Is JsonResult Then
            Dim jsonResult As JsonResult = DirectCast(filterContext.Result, JsonResult)

            ' Crear un nuevo JSON result con configuración personalizada
            Dim customResult As New CustomJsonResult() With {
                .Data = jsonResult.Data,
                .ContentEncoding = jsonResult.ContentEncoding,
                .ContentType = jsonResult.ContentType,
                .JsonRequestBehavior = jsonResult.JsonRequestBehavior,
                .MaxJsonLength = jsonResult.MaxJsonLength
            }

            filterContext.Result = customResult
        End If

        MyBase.OnActionExecuted(filterContext)
    End Sub


    Private Sub ProcessComplexObject(obj As Object)
        If obj Is Nothing Then Return

        Dim objType = obj.GetType()

        ' Ignorar tipos primitivos, strings y otros tipos simples
        If objType.IsPrimitive OrElse objType Is GetType(String) OrElse objType Is GetType(Decimal) _
           OrElse objType Is GetType(Single) OrElse objType Is GetType(Double) Then
            Return
        End If

        ' Procesar colecciones
        If TypeOf obj Is IEnumerable AndAlso Not TypeOf obj Is String Then
            For Each item In DirectCast(obj, IEnumerable)
                ProcessComplexObject(item)
            Next
            Return
        End If

        ' Procesar propiedades públicas
        For Each prop In objType.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            ' Saltar propiedades de solo lectura
            If Not prop.CanWrite Then Continue For

            ' Obtener el valor actual
            Dim value = prop.GetValue(obj)

            ' Si la propiedad es DateTime, convertirla
            If prop.PropertyType Is GetType(DateTime) OrElse prop.PropertyType Is GetType(DateTime?) Then
                If value IsNot Nothing Then
                    If TypeOf value Is DateTime Then
                        Dim convertedValue = ConvertToRomanceTime(DirectCast(value, DateTime))
                        prop.SetValue(obj, convertedValue)
                    End If
                End If
            Else
                ' Procesar recursivamente propiedades complejas
                ProcessComplexObject(value)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Convierte una fecha UTC a Romance Standard Time
    ''' </summary>
    Private Function ConvertToRomanceTime(dateTime As DateTime) As DateTime
        Dim sourceDate As Date = dateTime.ToUniversalTime()
        Return roTypes.CreateDateTime(sourceDate.Year, sourceDate.Month, sourceDate.Day, sourceDate.Hour, sourceDate.Minute, sourceDate.Second, DateTimeKind.Unspecified)
    End Function

End Class

Public Class CustomJsonResult
    Inherits JsonResult

    Public Overrides Sub ExecuteResult(context As ControllerContext)
        If context Is Nothing Then
            Throw New ArgumentNullException("context")
        End If

        Dim response = context.HttpContext.Response
        response.ContentType = If(Not String.IsNullOrEmpty(ContentType), ContentType, "application/json")

        If ContentEncoding IsNot Nothing Then
            response.ContentEncoding = ContentEncoding
        End If

        If Data IsNot Nothing Then
            ' Configurar el serializador con formato ISO
            Dim serializer As New JsonSerializer() With {
                .DateFormatHandling = DateFormatHandling.IsoDateFormat,
                .DateTimeZoneHandling = DateTimeZoneHandling.Utc
            }

            Using writer As New JsonTextWriter(New StreamWriter(response.OutputStream))
                serializer.Serialize(writer, Data)
                writer.Flush()
            End Using
        End If
    End Sub
End Class