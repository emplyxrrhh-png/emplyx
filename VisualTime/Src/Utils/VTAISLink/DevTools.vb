Imports Robotics.Base.DTOs
Imports Robotics.Business.PlanningEngine
Imports Robotics.VTBase


Public Class DevTools
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnTestRequest.Click
        PrepareScenario()
    End Sub


    Private Sub PrepareScenario()
        Dim oRequest As New roPlanningEngineRequest

        Dim oScenario As New ScenarioRequest
        ' 0.- Identificador
        oScenario.algorithm = "cegid_VTAI_v0"
        oScenario.id = "300"
        ' 0.1.- Criterios generales
        Dim oScenarioCriteria As New Criteria
        oScenario.criteria = oScenarioCriteria
        ' 1.- Personas
        Dim oPerson As New Person
        oPerson.id = "p1"
        oPerson.typeContract = "FIX"
        oPerson.roles = {"rol1"}
        ' 1.1.- Calendario
        Dim oPersonCalendarDate As New CalendarDate
        Dim oDate As New [Date](1, 1, 1)
        oPersonCalendarDate.date = oDate
        oPersonCalendarDate.availabilities = {New Availability(New DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local), New DateTime(2020, 1, 1, 9, 0, 0, DateTimeKind.Local))}
        oPersonCalendarDate.type = "LABORABLE"
        oPersonCalendarDate.preAssignedPositions = {}
        oPerson.calendar = {oPersonCalendarDate}
        ' 1.2.- Constraints
        Dim oPersonConstraints As New Constraints
        oPersonConstraints.maxMinutesPerDateConstraint = New MaxMinutesPerDateConstraint(True, {300, 300, 300, 300, 300, 0, 0})
        oPersonConstraints.maxDatesWithCategoryInIntervalConstraint = New MaxDatesWithCategoryInIntervalConstraint(True, "morning", "1-1-1/1-1-1", 1)

        oPerson.constraints = oPersonConstraints
        ' 1.3.- Costes
        oPerson.costs = New Costs(96, 120, 150, 0.2, 96)

        oScenario.people = {oPerson}
        ' 2.- Equipos
        Dim oTeam As New Team
        oTeam.id = "t1"

        Dim oPosition As New Position
        oPosition.id = "t1p1"
        oPosition.required = True
        oPosition.rol = "rol1"
        oPosition.slot = New Slot("slot1", "morning", New DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local), New DateTime(2020, 1, 1, 9, 0, 0, DateTimeKind.Local), 300)
        Dim aDatePositions As New Hashtable
        aDatePositions.Add("1-1-1", {oPosition})

        oTeam.datePositions = aDatePositions

        oScenario.teams = {oTeam}

        Dim str As String
        oRequest.scenarioRequest = oScenario
        str = roJSONHelper.SerializeNewtonSoft(oRequest)
        MsgBox(str)

    End Sub


    Private Sub SendRequest(sender As Object, e As EventArgs) Handles btnRequest.Click
        Dim oEngineResponse As roPlanningEngineResponse
        Dim oEngineState As New roPlanningEngineState(-1)
        Dim oEngine As New roPlanningEngineManager(oEngineState)
        oEngine.SolverURI = "http://84.88.176.103:10003/robotics/api/solver"
        oEngineResponse = oEngine.SendRequest(txtRequest.Text).Result
        If oEngineState.Result <> roPlanningEngineState.ResultEnum.NoError Then
            MsgBox("Oops: --- " & oEngineState.ResultDetail)
        Else
            txtID.Text = oEngineResponse.scenarioResponse.id
        End If
    End Sub

    Private Sub CheckRequest(sender As Object, e As EventArgs) Handles btnCheckRequest.Click
        Dim oEngineResponse As roPlanningEngineResponse = Nothing
        Dim oEngine As New roPlanningEngineManager(New roPlanningEngineState(-1))
        oEngine.SolverURI = "http://84.88.176.103:10003/robotics/api/solver"
        Dim dStart As DateTime = Now
        While oEngineResponse Is Nothing OrElse oEngineResponse.scenarioResponse.status <> "COMPLETED"
            oEngineResponse = oEngine.CheckRequest(txtID.Text).Result
            If oEngineResponse Is Nothing OrElse oEngineResponse.scenarioResponse.status <> "COMPLETED" Then
                Threading.Thread.Sleep(1000)
            End If
        End While
        lblElapsed.Text = Now.Subtract(dStart).TotalSeconds.ToString & " segundos"
        txtResponse.Text = roJSONHelper.SerializeNewtonSoft(oEngineResponse)
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Dim oTeam As New Team
        oTeam.id = "t1"

        Dim oPosition As New Position
        oPosition.id = "t1p1"
        oPosition.required = True
        oPosition.rol = "rol1"
        oPosition.slot = New Slot("slot1", "morning", New DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Local), New DateTime(2020, 1, 1, 9, 0, 0, DateTimeKind.Local), 300)
        Dim aDatePositions As New Hashtable
        aDatePositions.Add("1-1-1", {oPosition})

        oTeam.datePositions = aDatePositions

        Dim str As String = roJSONHelper.SerializeNewtonSoft(oTeam)

        Dim oTeamRecover As Team = roJSONHelper.DeserializeNewtonSoft(str, GetType(Team))

        MsgBox(oTeamRecover.id)

    End Sub
End Class