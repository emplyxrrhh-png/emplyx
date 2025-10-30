Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Class GeniusHelper

    Public Property GeniusViewName As String
    Function FillMockForDBResults(numberOfRows As Integer) As DataTable
        Dim oDBResults As New DataTable()
        oDBResults.Columns.Add("Columna1", GetType(Integer))
        oDBResults.Columns.Add("Columna2", GetType(String))
        Dim random As New Random()

        For i As Integer = 1 To numberOfRows
            Dim newRow As DataRow = oDBResults.NewRow()

            newRow("Columna1") = random.Next(1, 1000) ' Números aleatorios entre 1 y 1000
            newRow("Columna2") = "ValorAleatorio" & random.Next(1, 1000) ' Cadena aleatoria

            oDBResults.Rows.Add(newRow)
        Next
        Return oDBResults
    End Function

    Sub GetGeniusExecutionById()
        Robotics.Base.Analytics.Manager.Fakes.ShimroGeniusViewManager.AllInstances.GetGeniusExecutionByIdInt32Boolean = Function(manager, executionId, load)
                                                                                                                            Dim geniusExecution As New roGeniusExecution()
                                                                                                                            geniusExecution.Id = 1
                                                                                                                            Return geniusExecution
                                                                                                                        End Function
    End Sub

    Sub GenerateDBCube()
        Robotics.Base.VTAnalyticsManager.Fakes.ShimroAnalyticsManager.AllInstances.GenerateDBCubeStringInt32Int32DateTimeDateTimeStringStringStringStringStringStringBooleanBooleanBooleanStringroLanguageDateTimeDateTimeroGeniusExecutionStringBoolean = Function(manager, cubeName, cubeType, cubeSubType, startDate, endDate, filter, groupBy, orderBy, orderByDirection, orderBy2, orderByDirection2, load, a, b, c, d, g, h, i, executionName, k)
                                                                                                                                                                                                                                                               GeniusViewName = executionName
                                                                                                                                                                                                                                                               Return True
                                                                                                                                                                                                                                                           End Function

    End Sub

    Sub UpdateGeniusExecution()
        Robotics.Base.Analytics.Manager.Fakes.ShimroGeniusViewManager.AllInstances.UpdateGeniusExecutionroGeniusExecutionBoolean = Function(manager, geniusExecution, load)
                                                                                                                                       Return True
                                                                                                                                   End Function
    End Sub

End Class