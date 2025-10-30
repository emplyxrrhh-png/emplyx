Imports System.Configuration
Imports System.Data.Common
Imports System.Runtime.Caching
Imports System.Threading
Imports System.Web
Imports FxResources
Imports Microsoft.ApplicationInsights
Imports Robotics.Base.DTOs
Imports Robotics.VTBase.roLog

Public Class roTelemetryInfo
    Private Shared memoryCache As MemoryCache = MemoryCache.Default

    Protected Shared _instance As roTelemetryInfo
    Public Shared ReadOnly Property GetInstance() As roTelemetryInfo
        Get
            If _instance Is Nothing Then
                _instance = New roTelemetryInfo
            End If
            Return _instance
        End Get
    End Property


    Public Function UpdateO11yInfo(ByVal dictionary As Dictionary(Of String, String)) As Boolean
        Dim sId As String

        If Not Robotics.VTBase.roConstants.IsMultitenantServiceEnabled Then
            sId = Robotics.VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID)
        Else
            sId = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID")
        End If

        If sId <> String.Empty Then
            memoryCache.Set("o11yInfo_" & sId, dictionary, DateTimeOffset.Now.AddDays(1))
            Return True
        Else
            Return False
        End If

    End Function


    Public Function GetO11yInfo() As Dictionary(Of String, String)
        Dim sId As String

        If Not Robotics.VTBase.roConstants.IsMultitenantServiceEnabled Then
            sId = Robotics.VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID)
        Else
            sId = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID")
        End If

        'Sólo aplicará si debugamos en local
        If sId <> String.Empty Then

            Dim o11Info As Dictionary(Of String, String) = memoryCache.Get("o11yInfo_" & sId)

            If o11Info Is Nothing Then
                o11Info = New Dictionary(Of String, String)
                UpdateO11yInfo(o11Info)
            End If

            Return o11Info
        Else
            Return Nothing
        End If
    End Function


    Public Function UpdateTraceInfo(ByVal dictionary As Dictionary(Of String, String)) As Boolean
        Dim sId As String

        If Not Robotics.VTBase.roConstants.IsMultitenantServiceEnabled Then
            sId = Robotics.VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID)
        Else
            sId = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID")
        End If

        If sId <> String.Empty Then
            memoryCache.Set("o11yTrace_" & sId, dictionary, DateTimeOffset.Now.AddDays(1))
            Return True
        Else
            Return False
        End If

    End Function


    Public Function GetTraceInfo() As Dictionary(Of String, String)
        Dim sId As String

        If Not Robotics.VTBase.roConstants.IsMultitenantServiceEnabled Then
            sId = Robotics.VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID)
        Else
            sId = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID")
        End If

        'Sólo aplicará si debugamos en local
        If sId <> String.Empty Then

            Dim o11Info As Dictionary(Of String, String) = memoryCache.Get("o11yTrace_" & sId)

            If o11Info Is Nothing Then
                o11Info = New Dictionary(Of String, String)
                UpdateTraceInfo(o11Info)
            End If

            Return o11Info
        Else
            Return Nothing
        End If
    End Function


    Public Function ClearO11yInfo() As Boolean
        Dim sId As String

        If Not Robotics.VTBase.roConstants.IsMultitenantServiceEnabled Then
            sId = Robotics.VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID)
        Else
            sId = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID")
        End If

        'Sólo aplicará si debugamos en local
        If sId <> String.Empty Then
            If memoryCache.Contains("o11yInfo_" & sId) Then memoryCache.Remove("o11yInfo_" & sId)
            If memoryCache.Contains("o11yInfo_" & sId) Then memoryCache.Remove("o11yTrace_" & sId)
            Return True
        Else
            Return False
        End If
    End Function



End Class