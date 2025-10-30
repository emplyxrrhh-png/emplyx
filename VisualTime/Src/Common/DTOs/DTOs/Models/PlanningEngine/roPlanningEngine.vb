Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class Criteria
        <DataMember()>
        Public Property maxExecutionTime As Integer
        <DataMember()>
        Public Property startHorizonDate As Object
        <DataMember()>
        Public Property endHorizonDate As Object
        <DataMember()>
        Public Property replanificationSoft As Boolean
        <DataMember()>
        Public Property replanificationHard As Boolean
        <DataMember()>
        Public Property notAssignedSoft As Boolean
        <DataMember()>
        Public Property notAssignedHard As Boolean
        <DataMember()>
        Public Property replanificationWeight As Integer
        <DataMember()>
        Public Property notAssignedWeight As Integer
        <DataMember()>
        Public Property maxMinutesPerDateWeight As Integer
        <DataMember()>
        Public Property minMinutesPerDateWeight As Integer
        <DataMember()>
        Public Property minMinutesRestBetweenDatesWeight As Integer
        '<DataMember()>
        'Public Property maxConsecutiveDatesWithCategoryWeight As Integer
        '<DataMember()>
        'Public Property minConsecutiveDatesWithCategoryWeight As Integer
        '<DataMember()>
        'Public Property maxConsecutiveDatesWeight As Integer
        '<DataMember()>
        'Public Property maxConsecutiveMinutesWeight As Integer
        '<DataMember()>
        'Public Property maxDatesWithCategoryInIntervalWeight As Integer
        '<DataMember()>
        'Public Property minDatesWithCategoryInIntervalWeight As Integer
        '<DataMember()>
        'Public Property maxFestiveDatesWeight As Integer
        '<DataMember()>
        'Public Property maxNonLaborableDatesWeight As Integer
        '<DataMember()>
        'Public Property maxDatesWeight As Integer
        '<DataMember()>
        'Public Property maxMinutesInIntervalWeight As Integer
        '<DataMember()>
        'Public Property minMinutesInIntervalWeight As Integer
        '<DataMember()>
        'Public Property maxMinutesRestBetweenPositionsInDateWeight As Integer
        '<DataMember()>
        'Public Property minMinutesRestBetweenPositionsInDateWeight As Integer
        '<DataMember()>
        'Public Property minConsecutiveFreeDatesWeight As Integer

        Public Sub New()
            maxExecutionTime = 5
            startHorizonDate = Nothing
            endHorizonDate = Nothing
            replanificationSoft = False
            replanificationHard = False
            notAssignedSoft = False
            notAssignedHard = False
            replanificationWeight = 1
            notAssignedWeight = 1
            maxMinutesPerDateWeight = 1
            minMinutesPerDateWeight = 1
            minMinutesRestBetweenDatesWeight = 1
            'maxConsecutiveDatesWithCategoryWeight = 1
            'minConsecutiveDatesWithCategoryWeight = 1
            'maxConsecutiveDatesWeight = 1
            'maxConsecutiveMinutesWeight = 1
            'maxDatesWithCategoryInIntervalWeight = 1
            'minDatesWithCategoryInIntervalWeight = 1
            'maxFestiveDatesWeight = 1
            'maxNonLaborableDatesWeight = 1
            'maxDatesWeight = 1
            'maxMinutesInIntervalWeight = 1
            'minMinutesInIntervalWeight = 1
            'maxMinutesRestBetweenPositionsInDateWeight = 1
            'minMinutesRestBetweenPositionsInDateWeight = 1
            'minConsecutiveFreeDatesWeight = 1
        End Sub
    End Class
    <DataContract>
    Public Class [Date]
        <DataMember()>
        Public Property year As Integer
        <DataMember()>
        Public Property month As Integer
        <DataMember()>
        Public Property day As Integer

        Public Sub New(iYear As Integer, iMonth As Integer, iDay As Integer)
            year = iYear
            month = iMonth
            day = iDay
        End Sub
    End Class
    <DataContract>
    Public Class Availability
        <DataMember()>
        Public Property [start] As DateTime
        <DataMember()>
        Public Property [end] As DateTime
        Public Sub New(dStart As DateTime, dEnd As DateTime)
            [start] = dStart
            [end] = dEnd
        End Sub
    End Class
    Public Class PreAssignedPosition
        <DataMember()>
        Public Property idPosition As String
        <DataMember()>
        Public Property idTeam As String
        <DataMember()>
        Public Property isModifiable As Boolean
        <DataMember()>
        Public Property modifiable As Boolean
    End Class

    Public Class CalendarDate
        <DataMember()>
        Public Property [date] As [Date]
        <DataMember()>
        Public Property availabilities As Availability()
        <DataMember()>
        Public Property type As String
        <DataMember()>
        Public Property preAssignedPositions As PreAssignedPosition()
    End Class

    Public Class MaxMinutesPerDateConstraint
        <DataMember()>
        Public Property maximum As List(Of Integer)
        <DataMember()>
        Public Property hard As Boolean

        Public Sub New(bIsHard As Boolean, aMaximum As Integer())
            hard = bIsHard
            maximum = If(aMaximum IsNot Nothing, aMaximum.ToList, Array.Empty(Of Integer).ToList)
        End Sub
    End Class

    Public Class MinMinutesPerDateConstraint
        <DataMember()>
        Public Property minimum As List(Of Integer)
        <DataMember()>
        Public Property hard As Boolean

        Public Sub New(bIsHard As Boolean, aMinimum As Integer())
            hard = bIsHard
            minimum = If(aMinimum IsNot Nothing, aMinimum.ToList, Array.Empty(Of Integer).ToList)
        End Sub
    End Class

    Public Class MaxDatesWithCategoryInIntervalConstraint
        <DataMember()>
        Public Property isHard As Boolean
        <DataMember()>
        Public Property maximums As New Hashtable
        <DataMember()>
        Public Property hard As Boolean

        Public Sub New(bHard As Boolean, sShift As String, sPeriod As String, iVal As Integer)
            ''isHard = bHard
            ''hard = bHard
            ''Dim oElement As New Hashtable
            ''oElement.Add(sPeriod, iVal)
            ''maximums.Add(sShift, oElement)
        End Sub
    End Class

    Public Class MinMinutesRestBetweenDatesConstraint
        <DataMember()>
        Public Property minimum As Integer
        <DataMember()>
        Public Property hard As Boolean

        Public Sub New(bIsHard As Boolean, minimumminutes As Integer)
            hard = bIsHard
            minimum = minimumminutes
        End Sub
    End Class

    Public Class Constraints
        <DataMember()>
        Public Property minMinutesPerDateConstraint As MinMinutesPerDateConstraint
        <DataMember()>
        Public Property maxMinutesPerDateConstraint As MaxMinutesPerDateConstraint
        <DataMember()>
        Public Property minMinutesInIntervalConstraint As Object
        <DataMember()>
        Public Property maxMinutesInIntervalConstraint As Object
        <DataMember()>
        Public Property minMinutesRestBetweenDatesConstraint As Object
        <DataMember()>
        Public Property maxMinutesRestBetweenPositionsInDateConstraint As Object
        <DataMember()>
        Public Property minMinutesRestBetweenPositionsInDateConstraint As Object
        <DataMember()>
        Public Property maxWorkingDatesInHorizonConstraint As Object
        <DataMember()>
        Public Property maxFestiveWorkingDatesInHorizonConstraint As Object
        <DataMember()>
        Public Property maxNonLaborableWorkingDatesConstraint As Object
        <DataMember()>
        Public Property maxConsecutiveWorkingDatesConstraint As Object
        <DataMember()>
        Public Property maxConsecutiveWorkingMinutesConstraint As Object
        <DataMember()>
        Public Property minConsecutiveFreeDatesConstraint As Object
        <DataMember()>
        Public Property maxDatesWithCategoryInIntervalConstraint As MaxDatesWithCategoryInIntervalConstraint
        <DataMember()>
        Public Property maxConsecutiveDatesWithCategoryConstraint As Object
        <DataMember()>
        Public Property minDatesWithCategoryInIntervalConstraint As Object
        <DataMember()>
        Public Property minConsecutiveDatesWithCategoryConstraint As Object
    End Class

    <DataContract>
    Public Class Costs
        <DataMember()>
        Public Property costWorkingDay As Double
        <DataMember()>
        Public Property costNonWorkingDay As Double
        <DataMember()>
        Public Property costFestiveDay As Double
        <DataMember()>
        Public Property costExtraHour As Double
        <DataMember()>
        Public Property costNotBeingAsigned As Double

        Public Sub New(iWorking As Integer, iNonWorking As Integer, iFestive As Integer, iExtra As Double, iNotBeingAsigned As Double)
            costWorkingDay = iWorking
            costNonWorkingDay = iNonWorking
            costFestiveDay = iFestive
            costExtraHour = iExtra
            costNotBeingAsigned = iNotBeingAsigned
        End Sub
    End Class

    Public Class Person
        <DataMember()>
        Public Property id As String
        <DataMember()>
        Public Property roles As String()
        <DataMember()>
        Public Property locations As String()
        <DataMember()>
        Public Property typeContract As String
        <DataMember()>
        Public Property calendar As CalendarDate()
        <DataMember()>
        Public Property constraints As Constraints
        <DataMember()>
        Public Property costs As Costs
    End Class

    <DataContract>
    Public Class Slot
        <DataMember()>
        Public Property idShift As String
        <DataMember()>
        Public Property category As String
        <DataMember()>
        Public Property [start] As DateTime
        <DataMember()>
        Public Property [end] As DateTime
        <DataMember()>
        Public Property totalTime As Integer

        Public Sub New(sIdShift As String, sCategory As String, dStart As DateTime, dEnd As DateTime, iExpectedMinutes As Integer)
            idShift = sIdShift
            category = sCategory
            [start] = dStart
            [end] = dEnd
            totalTime = iExpectedMinutes
        End Sub
    End Class

    Public Class Position
        <DataMember()>
        Public Property id As String
        <DataMember()>
        Public Property required As Boolean
        <DataMember()>
        Public Property rol As String
        <DataMember()>
        Public Property slot As Slot
        <DataMember()>
        Public Property location As String
        <DataMember()>
        Public Property notCoveredCost As Double
    End Class

    Public Class Team
        <DataMember()>
        Public Property id As String
        <DataMember()>
        Public Property datePositions As Hashtable
    End Class

    Public Class roPlanningEngineRequest
        <DataMember()>
        Public Property scenarioRequest As ScenarioRequest
    End Class

    Public Class ScenarioRequest
        <DataMember()>
        Public Property algorithm As String
        <DataMember()>
        Public Property id As String
        <DataMember()>
        Public Property criteria As Criteria
        <DataMember()>
        Public Property people As Person()
        <DataMember()>
        Public Property teams As Team()
    End Class

    Public Class AssignedPosition
        <DataMember()>
        Public Property positionId As String
        <DataMember()>
        Public Property slot As Slot
        <DataMember()>
        Public Property assignedPersonId As String
        <DataMember()>
        Public Property required As Boolean
    End Class

    Public Class AssignedTeam
        <DataMember()>
        Public Property teamId As String
        <DataMember()>
        Public Property dateAssignedPositionsList As Dictionary(Of String, Dictionary(Of String, AssignedPosition))
    End Class

    Public Class ScenarioSolution
        <DataMember()>
        Public Property assignedTeams As Dictionary(Of String, AssignedTeam)
        <DataMember()>
        Public Property maxConsecutiveDatesWithCategoryScore As Object
        <DataMember()>
        Public Property maxConsecutiveWorkingDatesScore As Object
        <DataMember()>
        Public Property maxConsecutiveWorkingMinutesScore As Object
        <DataMember()>
        Public Property maxDatesWithCategoryIntervalScore As Object
        <DataMember()>
        Public Property maxFestiveWorkingDatesScore As Object
        <DataMember()>
        Public Property maxMinutesIntervalScore As Object
        <DataMember()>
        Public Property maxMinutesPerDateScore As Object
        <DataMember()>
        Public Property maxMinutesRestBetweenPositionsScore As Object
        <DataMember()>
        Public Property maxNonLaborableWorkingDatesScore As Object
        <DataMember()>
        Public Property maxWorkingDatesScore As Object
        <DataMember()>
        Public Property minConsecutiveDatesWithCategoryScore As Object
        <DataMember()>
        Public Property minConsecutiveFreeDatesScore As Object
        <DataMember()>
        Public Property minDatesWithCategoryIntervalScore As Object
        <DataMember()>
        Public Property minMinutesIntervalScore As Object
        <DataMember()>
        Public Property minMinutesPerDateScore As Object
        <DataMember()>
        Public Property minMinutesRestBetweenDatesScore As Object
        <DataMember()>
        Public Property minMinutesRestBetweenPositionsScore As Object
        <DataMember()>
        Public Property salaryScore As Object
        <DataMember()>
        Public Property replanificationScore As Object
        <DataMember()>
        Public Property notAssignedScore As Object
        <DataMember()>
        Public Property incompatibleScore As Object
    End Class

    Public Class Score
        <DataMember()>
        Public Property hard As Integer
        <DataMember()>
        Public Property medium As Integer
        <DataMember()>
        Public Property soft As Integer
    End Class

    Public Class ScenarioResponse
        <DataMember()>
        Public Property id As String
        <DataMember()>
        Public Property scenarioRequest As ScenarioRequest
        <DataMember()>
        Public Property status As String
        <DataMember()>
        Public Property feedback As Object
        <DataMember()>
        Public Property scenarioSolution As ScenarioSolution
        <DataMember()>
        Public Property score As Score
    End Class

    Public Class roPlanningEngineResponse
        <DataMember()>
        Public Property scenarioResponse As ScenarioResponse
    End Class

    Public Class ErrorResponse
        <DataMember()>
        Public Property timestamp As DateTime
        <DataMember()>
        Public Property status As Integer
        <DataMember()>
        Public Property [error] As String
        <DataMember()>
        Public Property message As String
        <DataMember()>
        Public Property path As String
    End Class

End Namespace