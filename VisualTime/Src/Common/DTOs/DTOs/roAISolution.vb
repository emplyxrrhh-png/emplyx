Namespace Robotics.Base.DTOs

    Namespace AIPlanner

        Public Class roAIPlannerSolution
            Public StartDate As Date
            Public EndDate As Date
            Public Assignments As New List(Of roAIAssignment)
        End Class

        Public Class roAIAssignment
            Public [Date] As Date = Nothing
            Public IdEmployee As Integer
            Public IdShift As Integer
            Public ShiftUID As String
            Public IdAssignment As Integer
            Public IdNode As Integer
            Public IdProductiveUnit As Integer
            Public IdDailyBudgetPosition As Integer
            Public DebugText As String
        End Class

    End Namespace

End Namespace