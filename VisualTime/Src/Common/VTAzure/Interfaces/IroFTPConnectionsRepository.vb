Imports Robotics.Base.DTOs

Public Interface IroFTPConnectionsRepository

    Function GetFTPConnection(ByVal IdCompany As String) As roFTPConnection

    Function GetConnections() As List(Of roFTPConnection)

End Interface