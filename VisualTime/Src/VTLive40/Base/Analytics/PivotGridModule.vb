Imports DevExpress.Web.ASPxPivotGrid
Imports DevExpress.XtraPivotGrid

Public Module Extensions

    <System.Runtime.CompilerServices.Extension>
    Public Function ToJson(ByVal dataSource As IDataSource) As String
        Dim result As New StringBuilder()
        result.Append("{")
        For Each viewName As String In dataSource.GetViewNames()
            result.AppendFormat("""{0}"":{1},", If(String.IsNullOrEmpty(viewName), "default", viewName), New ChartDataSourceCreator(CType(dataSource.GetView(viewName), PivotChartDataSourceView)).DataSource)
        Next viewName
        result.Remove(result.Length - 1, 1)
        result.Append("}")
        Return result.ToString().Replace("\", "\\")
    End Function

    Private Class ChartDataSourceCreator
        Private series As New List(Of String)()

        Public Sub New(ByVal view As PivotChartDataSourceView)
            Try
                fDataSource.Append("{""dataSource"":[")
                Dim data = From i In view.ChartDataSource.Cast(Of PivotChartDataSourceRowItem)() Group i By i.Argument Into Group Select Argument = Argument, Series = Group
                Dim hasData As Boolean = False
                For Each item In data
                    hasData = True
                    fDataSource.AppendFormat("{{""arg"":""{0}"",", item.Argument)
                    Dim seriesData = From i In item.Series Select New Tuple(Of String, Object)(CStr(i.Series), i.Value)
                    CreateSeriesData(seriesData)
                    fDataSource.Append("},")
                Next item
                If (hasData) Then fDataSource.Remove(fDataSource.Length - 1, 1)
                fDataSource.Append("],""series"":[")
                Dim hasSeries As Boolean = False
                For Each s As String In series
                    hasSeries = True
                    fDataSource.AppendFormat("{{""name"":""{0}"",""valueField"":""{1}""}},", s, s.ToSeriesFieldName())
                Next s
                If hasSeries Then fDataSource.Remove(fDataSource.Length - 1, 1)
                fDataSource.Append("]}")
            Catch ex As Exception
                fDataSource.Clear()
                fDataSource.Append("{""dataSource"":[],""series"":[]")
            End Try

        End Sub

        Private fDataSource As New StringBuilder()

        Public ReadOnly Property DataSource() As String
            Get
                Return fDataSource.ToString()
            End Get
        End Property

        Private Sub CreateSeriesData(ByVal data As IEnumerable(Of Tuple(Of String, Object)))
            For Each item As Tuple(Of String, Object) In data

                'fDataSource.AppendFormat("""{0}"":{1},", item.Item1.ToSeriesFieldName(), item.Item2)

                Select Case item.Item2.GetType().ToString.ToUpper
                    Case "SYSTEM.DECIMAL"
                        fDataSource.AppendFormat("""{0}"":{1},", item.Item1.ToSeriesFieldName(), item.Item2.ToString().Replace(",", "."))
                    Case "SYSTEM.DATETIME"
                        fDataSource.AppendFormat("""{0}"":{1},", item.Item1.ToSeriesFieldName(), item.Item2.ToString("s"))
                    Case "SYSTEM.DBNULL"
                        fDataSource.AppendFormat("""{0}"":{1},", item.Item1.ToSeriesFieldName(), "0")
                    Case Else
                        fDataSource.AppendFormat("""{0}"":{1},", item.Item1.ToSeriesFieldName(), item.Item2)

                End Select

                If (Not series.Contains(item.Item1)) Then
                    series.Add(item.Item1)
                End If
            Next item

            fDataSource.Remove(fDataSource.Length - 1, 1)
        End Sub

    End Class

    <System.Runtime.CompilerServices.Extension>
    Private Function ToSeriesFieldName(ByVal value As String) As String
        Return value.Replace(" | ", String.Empty).Replace(" ", "_")
    End Function

End Module