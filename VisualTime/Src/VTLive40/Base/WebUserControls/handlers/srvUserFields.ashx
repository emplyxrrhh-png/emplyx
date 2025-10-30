<%@ WebHandler Language="VB" Class="srvUserFields" %>

Imports System
Imports System.Web
Imports Robotics.Web.Base
Imports Robotics.Base.DTOs.UserFieldsTypes

Public Class srvUserFields
    Inherits handlerBase


    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)

        Select Case Request("action")
            Case "getUserFieldValues" ' Retorna la capcelera de la plana
                LoadUserFieldValues()
        End Select

    End Sub

#Region "Methods"
    Private Sub LoadUserFieldValues()
        Dim parameters As String = Request("FIELD_NAME")

        Dim oUserField As roUserField
        oUserField = API.UserFieldServiceMethods.GetUserField(Nothing, parameters, Types.EmployeeField, False, False)

        Dim strOutput As String = ""

        For Each strValue As String In oUserField.ListValues
            strOutput = strOutput & "*" & strValue
        Next

        Response.Write(strOutput)
    End Sub
#End Region


End Class