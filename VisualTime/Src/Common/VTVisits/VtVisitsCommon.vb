Imports System.IO
Imports System.Runtime.Serialization.Json
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase.roConversions

Namespace VTVisits

    Public Module VtVisitsCommon
        Public uri As String
        Private oOpt As roOptions

        Public Function CurrentOptions(Optional force As Boolean = False) As roOptions
            Try
                If oOpt Is Nothing Or force Then oOpt = New roOptions(Nothing)
            Catch ex As Exception

            End Try
            Return oOpt
        End Function

        Public Function any2SQLHTML(ByVal value As String)
            Dim tmp As String = ""
            Try
                tmp = System.Web.HttpUtility.HtmlEncode(value)
            Catch ex As Exception

            End Try
            Return value
        End Function

        Public Function HTML2String(ByVal value As String)
            Dim tmp As String = ""
            Try
                tmp = System.Web.HttpUtility.HtmlDecode(value)
            Catch ex As Exception

            End Try
            Return tmp
        End Function

        Public Function tojson(ByVal obj As Object)
            Dim st As New IO.MemoryStream
            Dim ser As DataContractJsonSerializer = New DataContractJsonSerializer(obj.GetType)
            ser.WriteObject(st, obj)
            st.Position = 0
            Dim sr As StreamReader = New StreamReader(st)
            Console.Write("JSON serialized Person object: ")
            Return (sr.ReadToEnd())
        End Function

        Public Function parsejson(ByVal json As String) As Dictionary(Of String, String)
            Dim dict As New Dictionary(Of String, String)
            Dim result As New Dictionary(Of String, String)
            Try
                Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer()
                dict = jss.Deserialize(Of Dictionary(Of String, String))(json)
                '20160526 - rtort - transform key dictionary on htmlcode
                For Each pair As KeyValuePair(Of String, String) In dict
                    result.Add(any2SQLHTML(pair.Key), pair.Value)
                Next
            Catch ex As Exception

            End Try
            Return result

        End Function

        Public Function parsejsonlist(ByVal json As String) As List(Of Dictionary(Of String, String))
            Dim list As New List(Of Dictionary(Of String, String))
            Dim dict As New Dictionary(Of String, String)
            Try
                Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer()
                For Each Str As String In json.Substring(1, json.Length - 2).Split(",")
                    dict = jss.Deserialize(Of Dictionary(Of String, String))(Str)
                    list.Add(dict)
                Next
            Catch ex As Exception

            End Try
            Return list

        End Function

        Public Function File2base64(ByVal Path As String) As String
            Try
                Dim fileStream As New FileStream(Path, FileMode.Open, FileAccess.Read)

                Dim ImageData As Byte() = New Byte(fileStream.Length - 1) {}
                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                fileStream.Close()

                Return Convert.ToBase64String(ImageData)
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Enum eStrType
            None = 0
            Alfanumeric = 1
            Alfa = 2
            Numeric = 3
        End Enum

        Public Function RandomString(len As Integer, StrType As eStrType)
            Dim r As New Random
            Dim s As String = ""

            Select Case StrType
                Case eStrType.Alfa
                    s = "ABCDEFGHIJKLMNPQRSTUVWXYZPOIUYTREWQASDFGHJKLZXCVBNMQWERTYUIPASDFGHJKLZXCVBNM"
                Case eStrType.Alfanumeric
                    s = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789POIUYTREWQ9876ASDFGHJKL54321ZXCVBNM75326981QWERTYUIPASDFGHJKLZXCVBNM789456123"
                Case eStrType.Numeric
                    s = "1234567891596324789563214789547852136985214785236985214569874563215478963254776518964763216987465419874654946541684"
                Case Else
            End Select

            Dim sb As String = ""
            For i As Integer = 1 To len
                Dim idx As Integer = r.Next(0, s.Length)
                sb += (s.Substring(idx, 1).ToUpper)
            Next
            Return sb.ToString()
        End Function

        Public Function GetParameter(ByVal name As String) As String
            Dim value As String = ""
            Try
                Dim sSQL As String = "@SELECT# Value from sysroLiveAdvancedParameters where ParameterName='" + name + "'" 'Visits.UID.Type'"
                value = Any2String(ExecuteScalar(sSQL))
            Catch ex As Exception

            End Try

            Return value
        End Function

    End Module

End Namespace