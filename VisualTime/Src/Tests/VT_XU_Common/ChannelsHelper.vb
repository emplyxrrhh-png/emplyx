Imports Robotics.Base.DTOs

Public Class ChannelsHelper

    Public Shared Function GetNewChannel(includeEmployees As Boolean, includeGroups As Boolean, includeSupervisors As Boolean) As roChannel
        Dim oChannel As roChannel = New roChannel
        oChannel.Id = 0
        oChannel.Title = "Canal de pruebas"
        oChannel.CreatedBy = 1
        oChannel.Status = 0
        If includeEmployees Then oChannel.Employees = {1}
        If includeGroups Then oChannel.Groups = {1}
        If includeSupervisors Then oChannel.SubscribedSupervisors = {1}
        Return oChannel
    End Function

    Public Shared Function GetExistingChannel(idChannel As Integer, idCreatedBy As Integer, includeEmployees As Boolean, includeGroups As Boolean, includeSupervisors As Boolean) As roChannel
        Dim oChannel As roChannel = New roChannel
        oChannel.Id = idChannel
        oChannel.Title = "Canal de pruebas"
        oChannel.CreatedBy = idCreatedBy
        oChannel.Status = 0
        If includeEmployees Then oChannel.Employees = {1}
        If includeGroups Then oChannel.Groups = {1}
        If includeSupervisors Then oChannel.SubscribedSupervisors = {1}
        Return oChannel
    End Function

End Class