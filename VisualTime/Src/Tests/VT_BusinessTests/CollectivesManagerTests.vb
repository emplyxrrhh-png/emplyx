Imports Robotics.Base.DTOs
Imports Robotics.Base.VTCollectives
Imports Xunit

Namespace Unit.Test

    <TestClass>
    Public Class CollectivesManagerTests

#Region "ProcessTimeAgoExpression Tests"
        <Fact(DisplayName:="Should Return Correct SQL for Days")>
        Sub ProcessTimeAgoExpression_ShouldReturnCorrectSQL_ForDays()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim timeAgoExpression As String = "10*days"

            ' Act
            Dim result As String = manager.ProcessTimeAgoExpression(timeAgoExpression)

            ' Assert
            Assert.Equal("DATEADD(day,-10, CAST(GETDATE() AS DATE))", result)
        End Sub

        <Fact(DisplayName:="Should Return Correct SQL for Weeks")>
        Sub ProcessTimeAgoExpression_ShouldReturnCorrectSQL_ForWeeks()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim timeAgoExpression As String = "2*weeks"

            ' Act
            Dim result As String = manager.ProcessTimeAgoExpression(timeAgoExpression)

            ' Assert
            Assert.Equal("DATEADD(week,-2, CAST(GETDATE() AS DATE))", result)
        End Sub

        <Fact(DisplayName:="Should Return Correct SQL for Months")>
        Sub ProcessTimeAgoExpression_ShouldReturnCorrectSQL_ForMonths()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim timeAgoExpression As String = "15*months"

            ' Act
            Dim result As String = manager.ProcessTimeAgoExpression(timeAgoExpression)

            ' Assert
            Dim maxMonths As Integer = (Now.Year - 1900) * 12 + Now.Month - 1
            Dim expectedTimeAgo As Integer = Math.Min(15, maxMonths)
            Assert.Equal($"DATEADD(month,-{expectedTimeAgo}, CAST(GETDATE() AS DATE))", result)
        End Sub

        <Fact(DisplayName:="Should Handle Too Old Dates for Months")>
        Sub ProcessTimeAgoExpression_ShouldReturnCorrectSQL_ForMonths_ControlTooOldDates()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim timeAgoExpression As String = "2500*months"

            ' Act
            Dim result As String = manager.ProcessTimeAgoExpression(timeAgoExpression)

            ' Assert
            Dim maxMonths As Integer = (Now.Year - 1900) * 12 + Now.Month - 1
            Dim expectedTimeAgo As Integer = Math.Min(1500, maxMonths)
            Assert.Equal($"DATEADD(month,-{maxMonths}, CAST(GETDATE() AS DATE))", result)
        End Sub

        <Fact(DisplayName:="Should Return Correct SQL for Years")>
        Sub ProcessTimeAgoExpression_ShouldReturnCorrectSQL_ForYears()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim timeAgoExpression As String = "5*years"

            ' Act
            Dim result As String = manager.ProcessTimeAgoExpression(timeAgoExpression)

            ' Assert
            Dim maxYears As Integer = Now.Year - 1900
            Dim expectedTimeAgo As Integer = Math.Min(5, maxYears)
            Assert.Equal($"DATEADD(year,-{expectedTimeAgo}, CAST(GETDATE() AS DATE))", result)
        End Sub

        <Fact(DisplayName:="Should Handle Too Old Dates for Years")>
        Sub ProcessTimeAgoExpression_ShouldReturnCorrectSQL_ForYears_ControlTooOldDates()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim timeAgoExpression As String = "500*years"

            ' Act
            Dim result As String = manager.ProcessTimeAgoExpression(timeAgoExpression)

            ' Assert
            Dim maxYears As Integer = Now.Year - 1900
            Dim expectedTimeAgo As Integer = Math.Min(5, maxYears)
            Assert.Equal($"DATEADD(year,-{maxYears}, CAST(GETDATE() AS DATE))", result)
        End Sub

        <Fact(DisplayName:="Should Return Default SQL for Invalid Unit")>
        Sub ProcessTimeAgoExpression_ShouldReturnDefaultSQL_ForInvalidUnit()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim timeAgoExpression As String = "10*invalid"

            ' Act
            Dim result As String = manager.ProcessTimeAgoExpression(timeAgoExpression)

            ' Assert
            Assert.Equal("CAST(GETDATE() AS DATE)", result)
        End Sub
#End Region

#Region "BuildDateComparison Tests"

        <Fact(DisplayName:="TextComparison - Should generate the correct SQL clause")>
        Sub BuildTextComparison_ShouldReturnCorrectSqlClause()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("NombreCampo", "=", "ValorTexto")

            ' Act
            Dim result As String = manager.BuildTextComparison(comparison)

            ' Assert
            Dim expected As String = "SUM(CASE WHEN fieldname = 'NombreCampo' AND ValueText = 'ValorTexto' THEN 1 ELSE 0 END) > 0"
            Assert.Equal(expected, result)
        End Sub

        <Fact(DisplayName:="TextComparison - Should escape single quotes correctly")>
        Sub BuildTextComparison_ShouldEscapeSingleQuotes()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("NombreCampo", "=", "Valor'Con'Comillas")

            ' Act
            Dim result As String = manager.BuildTextComparison(comparison)

            ' Assert
            Dim expected As String = "SUM(CASE WHEN fieldname = 'NombreCampo' AND ValueText = 'Valor''Con''Comillas' THEN 1 ELSE 0 END) > 0"
            Assert.Equal(expected, result)
        End Sub

        <Fact(DisplayName:="NumericComparison - Should generate the correct SQL clause")>
        Sub BuildNumericComparison_ShouldReturnCorrectSqlClause()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("EdadCampo", ">", "25")

            ' Act
            Dim result As String = manager.BuildNumericComparison(comparison)

            ' Assert
            Dim expected As String = "SUM(CASE WHEN fieldname = 'EdadCampo' AND ValueInt > 25 THEN 1 ELSE 0 END) > 0"
            Assert.Equal(expected, result)
        End Sub

        <Fact(DisplayName:="NumericComparison - Should handle non-numeric values")>
        Sub BuildNumericComparison_ShouldHandleNonNumericValues()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("EdadCampo", ">", "NoEsNumero")

            ' Act
            Dim result As String = manager.BuildNumericComparison(comparison)

            ' Assert
            Dim expected As String = "SUM(CASE WHEN fieldname = 'EdadCampo' AND ValueInt > 0 THEN 1 ELSE 0 END) > 0"
            Assert.Equal(expected, result)
        End Sub

        <Fact(DisplayName:="DecimalComparison - Should generate the correct SQL clause")>
        Sub BuildDecimalComparison_ShouldReturnCorrectSqlClause()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("SalarioCampo", "<", "2500.5")

            ' Act
            Dim result As String = manager.BuildDecimalComparison(comparison)

            ' Assert
            ' Note: The format may vary depending on culture, we use InvariantCulture
            Dim expected As String = $"SUM(CASE WHEN fieldname = 'SalarioCampo' AND ValueDecimal < {"2500.5".ToString(System.Globalization.CultureInfo.InvariantCulture)} THEN 1 ELSE 0 END) > 0"
            Assert.Equal(expected, result)
        End Sub

        <Fact(DisplayName:="DecimalComparison - Should handle non-decimal values")>
        Sub BuildDecimalComparison_ShouldHandleNonDecimalValues()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("SalarioCampo", "<", "NoEsDecimal")

            ' Act
            Dim result As String = manager.BuildDecimalComparison(comparison)

            ' Assert
            Dim expected As String = "SUM(CASE WHEN fieldname = 'SalarioCampo' AND ValueDecimal < 0 THEN 1 ELSE 0 END) > 0"
            Assert.Equal(expected, result)
        End Sub

        <Fact(DisplayName:="DateComparison - Should generate the correct SQL clause", Skip:="Need to mock Any2Time")>
        Sub BuildDateComparison_ShouldReturnCorrectSqlClause()
            ' Arrange
            Dim manager As New roCollectivesManager()
            ' Simulate a valid date to ensure test stability
            Dim validDate As New DateTime(2025, 1, 1)
            Dim comparison As New Comparison("FechaCampo", ">", validDate.ToString())

            ' Simulate the conversion of the Any2Time method (this should be adjusted according to the real implementation)
            Dim mockedSQLDate As String = $"'{validDate.ToString("yyyy-MM-dd HH:mm:ss")}'"
            ' Use a helper method to simulate the conversion
            ' Act - Assuming roTypes.Any2Time would return approximately that SQL format
            Dim expectedResult As String = $"SUM(CASE WHEN fieldname = 'FechaCampo' AND ValueDate > {mockedSQLDate} THEN 1 ELSE 0 END) > 0"

            ' This test requires mocking of roTypes.Any2Time or a more specific implementation
            ' to represent exactly what BuildDateComparison would return
        End Sub

        <Fact(DisplayName:="DateComparison - Should handle timeAgo expressions", Skip:="Need to mock ProcessTimeAgoExpression")>
        Sub BuildDateComparison_ShouldHandleTimeAgoExpressions()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("FechaCampo", "<", "10*days*ago")

            ' Act
            ' This test requires mocking for ProcessTimeAgoExpression or indirect verification
        End Sub

        <Fact(DisplayName:="AnniversaryComparison - Should generate the correct SQL clause")>
        Sub BuildAnniversaryComparison_ShouldReturnCorrectSqlClause()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("FechaCumpleaños", "ANNIVERSARY", "")

            ' Act
            Dim result As String = manager.BuildAnniversaryComparison(comparison)

            ' Assert
            Dim expected As String = "SUM(CASE WHEN fieldname = 'FechaCumpleaños' AND MONTH(ValueDate) = MONTH(GETDATE()) AND DAY(ValueDate) = DAY(GETDATE()) THEN 1 ELSE 0 END) > 0"
            Assert.Equal(expected, result)
        End Sub

        <Fact(DisplayName:="ListComparison - Should generate the correct SQL clause")>
        Sub BuildListComparison_ShouldReturnCorrectSqlClause()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("EstadoCampo", "=", "Activo")

            ' Act
            Dim result As String = manager.BuildListComparison(comparison)

            ' Assert
            Dim expected As String = "SUM(CASE WHEN fieldname = 'EstadoCampo' AND ValueList = 'Activo' THEN 1 ELSE 0 END) > 0"
            Assert.Equal(expected, result)
        End Sub

        <Fact(DisplayName:="ListComparison - Should escape single quotes correctly")>
        Sub BuildListComparison_ShouldEscapeSingleQuotes()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("EstadoCampo", "=", "Valor'Con'Comillas")

            ' Act
            Dim result As String = manager.BuildListComparison(comparison)

            ' Assert
            Dim expected As String = "SUM(CASE WHEN fieldname = 'EstadoCampo' AND ValueList = 'Valor''Con''Comillas' THEN 1 ELSE 0 END) > 0"
            Assert.Equal(expected, result)
        End Sub

        <Fact(DisplayName:="GenericComparison - Should generate the correct SQL clause")>
        Sub BuildGenericComparison_ShouldReturnCorrectSqlClause()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("OtroCampo", "=", "ValorGenerico")

            ' Act
            Dim result As String = manager.BuildGenericComparison(comparison)

            ' Assert
            Dim expected As String = "SUM(CASE WHEN fieldname = 'OtroCampo' AND TRY_CAST(RawValue AS NVARCHAR) = 'ValorGenerico' THEN 1 ELSE 0 END) > 0"
            Assert.Equal(expected, result)
        End Sub

        <Fact(DisplayName:="GenericComparison - Should escape single quotes correctly")>
        Sub BuildGenericComparison_ShouldEscapeSingleQuotes()
            ' Arrange
            Dim manager As New roCollectivesManager()
            Dim comparison As New Comparison("OtroCampo", "=", "Valor'Con'Comillas")

            ' Act
            Dim result As String = manager.BuildGenericComparison(comparison)

            ' Assert
            Dim expected As String = "SUM(CASE WHEN fieldname = 'OtroCampo' AND TRY_CAST(RawValue AS NVARCHAR) = 'Valor''Con''Comillas' THEN 1 ELSE 0 END) > 0"
            Assert.Equal(expected, result)
        End Sub
#End Region

#Region "ProcessFilterExpression Tests"
        Private ReadOnly collectivesManager As roCollectivesManager
        Private ReadOnly comparisonClass As Type

        Public Sub New()
            ' Crear una instancia del manager a testear en el constructor
            collectivesManager = New roCollectivesManager()

            ' Obtener referencia a la clase Comparison usando reflection
            comparisonClass = GetType(Comparison)

        End Sub

        <Fact(DisplayName:="Empty filter expression should return empty list")>
        Public Sub ProcessFilterExpression_EmptyFilter_ReturnsEmptyList()
            ' Arrange
            Dim filterExpression As String = ""

            ' Act
            Dim result As List(Of Object) = collectivesManager.ProcessFilterExpression(filterExpression)

            ' Assert
            Assert.NotNull(result)
            Assert.Empty(result)
        End Sub

        <Fact(DisplayName:="Simple comparison filter should be correctly parsed")>
        Public Sub ProcessFilterExpression_SimpleComparisonFilter_ReturnsCorrectlyParsedObjects()
            ' Arrange
            Dim filterExpression As String = "[""Departamento"",""="",""Ventas""]"

            ' Act
            Dim result As List(Of Object) = collectivesManager.ProcessFilterExpression(filterExpression)

            ' Assert
            Assert.NotNull(result)
            Assert.Equal(3, result.Count)
            Assert.Equal("(", result(0).ToString())
            Assert.True(comparisonClass.IsInstanceOfType(result(1)))
            Assert.Equal(")", result(2).ToString())

            ' Verificar propiedades del objeto Comparison usando reflection
            Dim comparison As Object = result(1)
            Assert.Equal("Departamento", GetComparisonProperty(comparison, "Operand1"))
            Assert.Equal("=", GetComparisonProperty(comparison, "ComparisonOperator"))
            Assert.Equal("Ventas", GetComparisonProperty(comparison, "Operand2"))
        End Sub

        <Fact(DisplayName:="Negated comparison filter should be correctly parsed")>
        Public Sub ProcessFilterExpression_NegatedComparisonFilter_ReturnsCorrectlyParsedObjects()
            ' Arrange
            Dim filterExpression As String = "[""!"",[[""Puesto"",""="",""Director""],""or"",[""Puesto"",""="",""Gerente""]]]"

            ' Act
            Dim result As List(Of Object) = collectivesManager.ProcessFilterExpression(filterExpression)

            ' Assert
            Assert.NotNull(result)
            Assert.Equal(12, result.Count)
            Assert.Equal("(", result(0).ToString())
            Assert.Equal("NOT", result(1).ToString())
            Assert.True(comparisonClass.IsInstanceOfType(result(4)))
            Assert.Equal(")", result(5).ToString())

            ' Verificar propiedades del objeto Comparison
            Dim comparison As Object = result(4)
            Assert.Equal("Puesto", GetComparisonProperty(comparison, "Operand1"))
            Assert.Equal("=", GetComparisonProperty(comparison, "ComparisonOperator"))
            Assert.Equal("Director", GetComparisonProperty(comparison, "Operand2"))
        End Sub

        <Fact(DisplayName:="Complex filter with AND operator should be correctly parsed")>
        Public Sub ProcessFilterExpression_ComplexFilter_ReturnsCorrectlyParsedObjects()
            ' Arrange: Expresión compleja con operador AND
            Dim filterExpression As String = "[""Departamento"",""="",""Ventas""],""and"",[""Edad"","">="",""30""]"

            ' Act
            Dim result As List(Of Object) = collectivesManager.ProcessFilterExpression(filterExpression)

            ' Assert
            Assert.NotNull(result)
            Assert.Equal(7, result.Count)

            ' Primera comparación
            Assert.Equal("(", result(0).ToString())
            Assert.True(comparisonClass.IsInstanceOfType(result(1)))
            Assert.Equal(")", result(2).ToString())

            ' Operador lógico
            Assert.Equal(" AND ", result(3).ToString())

            ' Segunda comparación
            Assert.Equal("(", result(4).ToString())
            Assert.True(comparisonClass.IsInstanceOfType(result(5)))
            Assert.Equal(")", result(6).ToString())

            ' Verificar propiedades de las comparaciones
            Dim comparison1 As Object = result(1)
            Assert.Equal("Departamento", GetComparisonProperty(comparison1, "Operand1"))
            Assert.Equal("=", GetComparisonProperty(comparison1, "ComparisonOperator"))
            Assert.Equal("Ventas", GetComparisonProperty(comparison1, "Operand2"))

            Dim comparison2 As Object = result(5)
            Assert.Equal("Edad", GetComparisonProperty(comparison2, "Operand1"))
            Assert.Equal(">=", GetComparisonProperty(comparison2, "ComparisonOperator"))
            Assert.Equal("30", GetComparisonProperty(comparison2, "Operand2"))
        End Sub

        <Fact(DisplayName:="Nested filters should be correctly parsed")>
        Public Sub ProcessFilterExpression_NestedFilters_ReturnsCorrectlyParsedObjects()
            ' Arrange: Expresión con filtros anidados
            Dim filterExpression As String = "[[""Departamento"",""="",""Ventas""],""and"",[""Edad"","">="",""30""]],""or"",[""Departamento"",""="",""Marketing""]]"

            ' Act
            Dim result As List(Of Object) = collectivesManager.ProcessFilterExpression(filterExpression)

            ' Assert
            Assert.NotNull(result)
            Assert.Equal(14, result.Count)

            ' Verificar la estructura general (no todos los elementos)
            Assert.Equal("(", result(0).ToString())
            Assert.Equal(" OR ", result(9).ToString())
        End Sub

        <Fact(DisplayName:="Special operators like CONTAINS should be correctly transformed")>
        Public Sub ProcessFilterExpression_WithSpecialOperators_ReturnsCorrectlyParsedObjects()
            ' Arrange: Expresión con operadores especiales como CONTAINS
            Dim filterExpression As String = "[""Nombre"",""contains"",""Juan""]"

            ' Act
            Dim result As List(Of Object) = collectivesManager.ProcessFilterExpression(filterExpression)

            ' Assert
            Assert.NotNull(result)
            Assert.Equal(3, result.Count)

            ' Verificar que después del ParseComparison, los operadores especiales se transforman
            Dim comparison As Object = result(1)
            Assert.Equal("Nombre", GetComparisonProperty(comparison, "Operand1"))
            Assert.Equal("LIKE", GetComparisonProperty(comparison, "ComparisonOperator"))
            Assert.Equal("%Juan%", GetComparisonProperty(comparison, "Operand2"))
        End Sub

        <Fact(DisplayName:="ANNIVERSARY operator should be correctly processed")>
        Public Sub ProcessFilterExpression_WithAnniversaryOperator_ReturnsCorrectlyParsedObjects()
            ' Arrange: Expresión con operador especial ANNIVERSARY
            Dim filterExpression As String = "[""FechaNacimiento"",""="",""ANNIVERSARY""]"

            ' Act
            Dim result As List(Of Object) = collectivesManager.ProcessFilterExpression(filterExpression)

            ' Assert
            Assert.NotNull(result)
            Assert.Equal(3, result.Count)

            ' Verificar que el operador ANNIVERSARY se procesa correctamente
            Dim comparison As Object = result(1)
            Assert.Equal("FechaNacimiento", GetComparisonProperty(comparison, "Operand1"))
            Assert.Equal("ANNIVERSARY", GetComparisonProperty(comparison, "ComparisonOperator"))
            Assert.Equal("", GetComparisonProperty(comparison, "Operand2"))
        End Sub

        <Fact(DisplayName:="STARTSWITH operator should add % at the end of operand")>
        Public Sub ProcessFilterExpression_WithStartsWithOperator_ReturnsCorrectlyParsedObjects()
            ' Arrange: Expresión con operador STARTSWITH
            Dim filterExpression As String = "[""Nombre"",""startswith"",""Juan""]"

            ' Act
            Dim result As List(Of Object) = collectivesManager.ProcessFilterExpression(filterExpression)

            ' Assert
            Assert.NotNull(result)
            Assert.Equal(3, result.Count)

            Dim comparison As Object = result(1)
            Assert.Equal("Nombre", GetComparisonProperty(comparison, "Operand1"))
            Assert.Equal("LIKE", GetComparisonProperty(comparison, "ComparisonOperator"))
            Assert.Equal("Juan%", GetComparisonProperty(comparison, "Operand2"))
        End Sub

        <Fact(DisplayName:="ENDSWITH operator should add % at the beginning of operand")>
        Public Sub ProcessFilterExpression_WithEndsWithOperator_ReturnsCorrectlyParsedObjects()
            ' Arrange: Expresión con operador ENDSWITH
            Dim filterExpression As String = "[""Nombre"",""endswith"",""Juan""]"

            ' Act
            Dim result As List(Of Object) = collectivesManager.ProcessFilterExpression(filterExpression)

            ' Assert
            Assert.NotNull(result)
            Assert.Equal(3, result.Count)

            Dim comparison As Object = result(1)
            Assert.Equal("Nombre", GetComparisonProperty(comparison, "Operand1"))
            Assert.Equal("LIKE", GetComparisonProperty(comparison, "ComparisonOperator"))
            Assert.Equal("%Juan", GetComparisonProperty(comparison, "Operand2"))
        End Sub

        <Fact(DisplayName:="NOTCONTAINS operator should correctly transform to NOT LIKE")>
        Public Sub ProcessFilterExpression_WithNotContainsOperator_ReturnsCorrectlyParsedObjects()
            ' Arrange: Expresión con operador NOTCONTAINS
            Dim filterExpression As String = "[""Nombre"",""notcontains"",""Juan""]"

            ' Act
            Dim result As List(Of Object) = collectivesManager.ProcessFilterExpression(filterExpression)

            ' Assert
            Assert.NotNull(result)
            Assert.Equal(3, result.Count)

            Dim comparison As Object = result(1)
            Assert.Equal("Nombre", GetComparisonProperty(comparison, "Operand1"))
            Assert.Equal("NOT LIKE", GetComparisonProperty(comparison, "ComparisonOperator"))
            Assert.Equal("%Juan%", GetComparisonProperty(comparison, "Operand2"))
        End Sub

        ' Método auxiliar para obtener propiedades del objeto Comparison usando reflection
        Private Function GetComparisonProperty(comparison As Object, propertyName As String) As Object
            Dim propertyInfo = comparisonClass.GetProperty(propertyName)
            If propertyInfo IsNot Nothing Then
                Return propertyInfo.GetValue(comparison)
            End If
            Return Nothing
        End Function
#End Region

    End Class

End Namespace
