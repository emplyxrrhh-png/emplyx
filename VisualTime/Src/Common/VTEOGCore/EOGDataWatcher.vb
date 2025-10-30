Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTEOGManager

    Public Class EOGDataWatcher
        Public Enum RunEvery
            Once
            Hour
            Day
        End Enum

        Public Function GetDetectionQueries(ByVal type As RunEvery) As List(Of DetectionQuery)
            Dim queries As New List(Of DetectionQuery)

            Dim sWhere As String = String.Empty

            If type = RunEvery.Day Then
                ' Si el tipo es diario, solo ejecuto las detecciones de tipo diario
                sWhere = $"RunEvery = {CInt(RunEvery.Day)}"
            Else
                ' Si el tipo es horario, ejecuto las detecciones de tipo horario o una sola vez
                sWhere = $"(RunEvery = {CInt(type)} OR RunEvery = 0)"
            End If

            Dim strSQL As String = $"@SELECT# Id, QueryText, RunEvery, LastExecution FROM sysroDetectionQueries WHERE {sWhere} AND Active = 1 ORDER BY ID ASC"

            Dim tb As DataTable = CreateDataTable(strSQL, )

            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each row As DataRow In tb.Rows
                    Dim query As New DetectionQuery With {
                                                            .Id = roTypes.Any2Integer(row("Id")),
                                                            .QueryText = roTypes.Any2String(row("QueryText")),
                                                            .RunEvery = roTypes.Any2Integer(row("RunEvery")),
                                                            .LastExecution = roTypes.Any2DateTime(row("LastExecution"))
                                                            }
                    queries.Add(query)
                Next
            End If

            Return queries
        End Function

        Public Function GetCorrectionQueries(detectionQueryId As Integer) As List(Of CorrectionQuery)
            Dim queries As New List(Of CorrectionQuery)

            Dim strSQL As String = $"@SELECT# Id, QueryText, Parameters, EngineTask, LastExecution FROM sysroCorrectionQueries WHERE DetectionQueryId = {detectionQueryId}"

            Dim tb As DataTable = CreateDataTable(strSQL, )

            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each row As DataRow In tb.Rows
                    Dim query As New CorrectionQuery With {
                                                                .Id = roTypes.Any2Integer(row("Id")),
                                                                .DetectionQueryId = detectionQueryId,
                                                                .QueryText = roTypes.Any2String(row("QueryText")),
                                                                .Parameters = roTypes.Any2String(row("Parameters")),
                                                                .EngineTask = roTypes.Any2String(row("EngineTask")),
                                                                .LastExecution = roTypes.Any2DateTime(row("LastExecution"))
                                                            }
                    queries.Add(query)
                Next
            End If

            Return queries
        End Function

        Public Function ApplyParameters(query As String, parameters As String, row As DataRow) As String

            Try
                Dim parameterPairs = parameters.Split(","c)
                For Each pair In parameterPairs
                    Dim parts = pair.Split("="c)
                    If parts.Length = 2 Then
                        Dim paramName = parts(0).Trim()
                        Dim columnParts = parts(1).Trim().Split("("c)
                        Dim columnName = columnParts(0).Trim()
                        Dim columnType = columnParts(1).Replace(")", "").Trim()

                        Dim columnValue As String = roTypes.Any2String(row(columnName))

                        Select Case columnType.ToLower()
                            Case "int"
                                columnValue = roTypes.Any2String(row(columnName))
                            Case "string"
                                columnValue = $"'{roTypes.Any2String(row(columnName))}'"
                            Case "date"
                                columnValue = roTypes.Any2Time(row(columnName)).SQLSmallDateTime
                        End Select

                        query = query.Replace(paramName, columnValue)
                    End If
                Next
            Catch ex As Exception
                query = String.Empty
            End Try

            Return query
        End Function

        Public Function ExecuteMonitoring(type As RunEvery) As Boolean
            Dim bRet As Boolean = True
            Dim lTriggers As New List(Of TasksType)

            Try
                Dim detectionQueries As List(Of DetectionQuery) = GetDetectionQueries(type)

                For Each detectionQuery In detectionQueries
                    Dim anomalies As DataTable = Nothing
                    Try
                        anomalies = CreateDataTable(detectionQuery.QueryText)
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, $"VTEOGCOre::EOGDataWatcher::ExecuteMonitoring::Error executing detection query {detectionQuery.Id}", ex)
                    End Try

                    If anomalies IsNot Nothing AndAlso anomalies.Rows.Count > 0 Then
                        'Obtengo las correcciones asociadas a la consulta de detección
                        Dim correctionQueries As List(Of CorrectionQuery) = GetCorrectionQueries(detectionQuery.Id)
                        Dim correctionsApplied As Integer = 0

                        If correctionQueries.Any() Then
                            For Each row As DataRow In anomalies.Rows
                                'Para cada registro de anomalía, ejecuto las correcciones ...
                                For Each correctionQuery In correctionQueries
                                    Dim correctionSQL As String = correctionQuery.QueryText
                                    correctionSQL = ApplyParameters(correctionSQL, correctionQuery.Parameters, row)

                                    If correctionSQL <> String.Empty Then
                                        Try
                                            ExecuteSql(correctionSQL)
                                            correctionsApplied += 1

                                            ' Actualizo la última ejecución de la consulta de corrección
                                            ExecuteSql($"@UPDATE# sysroCorrectionQueries SET LastExecution = GETDATE() WHERE Id = {correctionQuery.Id}")

                                            ' Registro triger si se especificó para aplicarlo al final del proceso
                                            Dim result As TasksType
                                            If correctionQuery.EngineTask.Trim.Length > 0 AndAlso [Enum].TryParse(correctionQuery.EngineTask, True, result) Then
                                                If Not lTriggers.Contains(result) Then lTriggers.Add(result)
                                            End If
                                        Catch ex As Exception
                                            roLog.GetInstance().logMessage(roLog.EventType.roError, $"VTEOGCOre::EOGDataWatcher::ExecuteMonitoring::Error executing correction query {correctionQuery.Id}", ex)
                                        End Try
                                    Else
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"VTEOGCOre::EOGDataWatcher::ExecuteMonitoring::Anomaly detected, but no correction script or could not be built")
                                    End If
                                Next
                            Next
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, $"VTEOGCOre::EOGDataWatcher::ExecuteMonitoring::Anomaly detected, but no correction script configured")
                        End If

                        'Actualizo la última ejecución de la consulta de detección
                        ExecuteSql($"@UPDATE# sysroDetectionQueries SET LastExecution = GETDATE() WHERE Id = {detectionQuery.Id}")

                        ' Si RunEvery es Once, desactivo la consulta de detección
                        If detectionQuery.RunEvery = RunEvery.Once Then
                            ExecuteSql($"@UPDATE# sysroDetectionQueries SET Active = 0 WHERE Id = {detectionQuery.Id}")
                        End If

                        ' Logs ...
                        If anomalies.Rows.Count > 0 Then
                            roLog.GetInstance().logMessage(roLog.EventType.roInfo, $"VTEOGCOre::EOGDataWatcher::{anomalies.Rows.Count} anomalies found for detection query {detectionQuery.Id}. {correctionsApplied} corrections applied.")
                        End If

                    End If
                Next

                ' Aplicamos triggers si es necesario ...
                If lTriggers.Any() Then
                    For Each trigger In lTriggers
                        roConnector.InitTask(trigger)
                    Next
                End If

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"VTEOGCOre::EOGDataWatcher::ExecuteMonitoring::Unknown error executing data monitoring", ex)
                bRet = False
            End Try

            Return bRet

        End Function

    End Class

    Public Class DetectionQuery
        Public Property Id As Integer
        Public Property QueryText As String
        Public Property RunEvery As EOGDataWatcher.RunEvery
        Public Property LastExecution As DateTime
    End Class

    Public Class CorrectionQuery
        Public Property Id As Integer
        Public Property DetectionQueryId As Integer
        Public Property QueryText As String
        Public Property Parameters As String
        Public Property EngineTask As String
        Public Property LastExecution As DateTime
    End Class

End Namespace