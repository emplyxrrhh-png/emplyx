Imports Robotics.VTBase
Imports Robotics.Web.Base

Public Class frmShiftSelector
    Inherits UserControlBase

    <Runtime.Serialization.DataContract()>
    Private Class ShiftDefinition

        <Runtime.Serialization.DataMember(Name:="Id")>
        Public Id As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="ShortName")>
        Public ShortName As String

        <Runtime.Serialization.DataMember(Name:="AllowFloatingData")>
        Public AllowFloatingData As Integer

        <Runtime.Serialization.DataMember(Name:="AllowComplementary")>
        Public AllowComplementary As Integer

        <Runtime.Serialization.DataMember(Name:="AllowAssignments")>
        Public AllowAssignments As Integer

        <Runtime.Serialization.DataMember(Name:="ShiftType")>
        Public ShiftType As Integer

        <Runtime.Serialization.DataMember(Name:="StartHour")>
        Public StartHour As DateTime

        <Runtime.Serialization.DataMember(Name:="ShiftColor")>
        Public ShiftColor As String

        <Runtime.Serialization.DataMember(Name:="ShiftHours")>
        Public ShiftHours As Long

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class AssignmentDefinition

        <Runtime.Serialization.DataMember(Name:="Id")>
        Public Id As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="ShortName")>
        Public ShortName As String

        <Runtime.Serialization.DataMember(Name:="Color")>
        Public Color As String

    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        cmbAvailableShifts.ClientInstanceName = Me.ClientID & "_cmbAvailableShifts"

        If Not IsPostBack Then
            cmbAvailableShifts.Items.Clear()
            cmbAvailableShifts.SelectedIndex = 0
            cmbAvailableShifts.DropDownRows = 4
            CreateShiftsDynamic()
        End If
    End Sub

    Private Sub CreateShiftsDynamic()
        Dim dTbl As DataTable = API.ShiftServiceMethods.GetShiftsPlanification(Me.Page, -1)

        Dim bolHRSchedulingLicense As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling")
        Dim shiftsList = dTbl.Select("", "Name")

        If (shiftsList.Count > 0) Then

            Dim olst As New Generic.List(Of ShiftDefinition)

            For Each shift As DataRow In shiftsList
                If (roTypes.Any2Integer(shift("Assignments")) > 0) Then
                    If (shift("Name").ToString().Equals("???")) Then Continue For

                    cmbAvailableShifts.Items.Add(New DevExpress.Web.ListEditItem(shift("Name"), shift("ID")))

                    Dim oItem As New ShiftDefinition
                    oItem.Id = roTypes.Any2Integer(shift("ID"))
                    oItem.Name = roTypes.Any2String(shift("Name"))
                    oItem.ShortName = roTypes.Any2String(shift("ShortName"))
                    oItem.AllowFloatingData = IIf(roTypes.Any2Boolean(shift("AllowFloatingData")), "1", "0")
                    oItem.AllowComplementary = IIf(roTypes.Any2Boolean(shift("AllowComplementary")), "1", "0")
                    oItem.AllowAssignments = IIf(bolHRSchedulingLicense AndAlso (roTypes.Any2Integer(shift("Assignments")) > 0), "1", "0")

                    If roTypes.Any2Boolean(shift("IsFloating")) Then
                        oItem.ShiftType = 1
                        oItem.StartHour = roTypes.Any2DateTime(shift("StartFloating")).ToString("yyyy/MM/dd HH:mm")
                    Else
                        If roTypes.Any2Integer(shift("ShiftType")) = 1 Then
                            oItem.StartHour = roTypes.Any2DateTime(shift("StartLimit")).ToString("yyyy/MM/dd HH:mm")
                            oItem.ShiftType = 0
                        Else
                            oItem.ShiftType = IIf(roTypes.Any2Boolean(shift("AreWorkingDays")), 2, 3)
                            oItem.StartHour = DateTime.Now.Date.ToString("yyyy/MM/dd HH:mm")
                        End If
                    End If

                    Dim auxColor As Drawing.Color = Drawing.ColorTranslator.FromWin32(shift("Color"))
                    Dim oHTMLColor As String = HexConverter(auxColor)
                    oItem.ShiftColor = oHTMLColor
                    oItem.ShiftHours = roTypes.Any2Time(roTypes.Any2Double(shift("ExpectedWorkingHours"))).Minutes

                    olst.Add(oItem)
                End If

            Next

            cmbAvailableShifts.JSProperties.Add("cpShiftDefinition", roJSONHelper.SerializeNewtonSoft(olst.ToArray))
        End If

        Dim dAssignmentTbl As DataTable = API.AssignmentServiceMethods.GetAssignmentsDataTable(Me.Page, " Name ASC ", False)

        If (dAssignmentTbl IsNot Nothing AndAlso dAssignmentTbl.Rows.Count > 0) Then

            Dim oAssignmentList As New Generic.List(Of AssignmentDefinition)

            For Each oAssignment As DataRow In dAssignmentTbl.Rows
                Dim oItem As New AssignmentDefinition
                oItem.Id = roTypes.Any2Integer(oAssignment("ID"))
                oItem.Name = roTypes.Any2String(oAssignment("Name"))
                oItem.ShortName = roTypes.Any2String(oAssignment("Name"))
                Dim auxColor As Drawing.Color = Drawing.ColorTranslator.FromWin32(oAssignment("Color"))
                Dim oHTMLColor As String = HexConverter(auxColor)
                oItem.Color = oHTMLColor

                oAssignmentList.Add(oItem)

            Next
            cmbAvailableShifts.JSProperties.Add("cpAssignmentsDefinition", roJSONHelper.SerializeNewtonSoft(oAssignmentList.ToArray))

        End If

    End Sub

    Private Shared Function HexConverter(c As System.Drawing.Color) As String
        Return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2")
    End Function

    Private Function InvertMeAColour(ColourToInvert As Drawing.Color) As Drawing.Color
        Return Drawing.Color.FromArgb(255 - ColourToInvert.R, 255 - ColourToInvert.G, 255 - ColourToInvert.B)
    End Function

End Class