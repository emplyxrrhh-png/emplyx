Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class roGeniusView

        Public Sub New()
            Me.Id = -1
            Me.Name = String.Empty
            Me.Description = String.Empty
            Me.TypeView = 0
            Me.DateFilterType = "0"
            Me.CreatedOn = DateTime.Now
            Me.DateInf = DateTime.Now
            Me.DateSup = DateTime.Now

            Me.IdPassport = 0
            Me.Employees = String.Empty
            Me.UserFields = String.Empty
            Me.Employees = String.Empty
            Me.CubeLayout = String.Empty
            Me.UserFields = String.Empty
            Me.BusinessCenters = String.Empty

            Me.IdParentShared = -1

            Me.DSFunction = String.Empty
            Me.Feature = String.Empty
            Me.RequieredFeature = String.Empty
            Me.RequieredLicense = String.Empty

            Me.CustomFields = New roGeniusCustomFields
            Me.Executions = New List(Of roGeniusExecution)
            Me.ContextMenu = String.Empty
            Me.CheckedCheckBoxes = New Integer() {}
            Me.IdSystemView = Nothing
        End Sub

        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property Name As String
        <DataMember>
        Public Property Description As String
        <DataMember>
        Public Property DS As GeniusTypeEnum
        <DataMember>
        Public Property TypeView As Integer
        <DataMember>
        Public Property CreatedOn As DateTime
        <DataMember>
        Public Property IdPassport As Integer
        <DataMember>
        Public Property IdParentShared As Integer
        <DataMember>
        Public Property Employees As String
        <DataMember>
        Public Property Concepts As String
        <DataMember>
        Public Property Causes As String
        <DataMember>
        Public Property DateFilterType As String
        <DataMember>
        Public Property DateInf As DateTime
        <DataMember>
        Public Property DateSup As DateTime
        <DataMember>
        Public Property TimeInf As DateTime
        <DataMember>
        Public Property TimeSup As DateTime
        <DataMember>
        Public Property CubeLayout As String
        <DataMember>
        Public Property UserFields As String
        <DataMember()>
        Public Property BusinessCenters As String
        <DataMember()>
        Public Property CustomFields As roGeniusCustomFields
        <DataMember()>
        Public Property DSFunction As String
        <DataMember()>
        Public Property Feature As String
        <DataMember()>
        Public Property RequieredFeature As String
        <DataMember()>
        Public Property RequieredLicense As String
        <DataMember()>
        Public Property CheckedCheckBoxes As Integer()
        <DataMember()>
        Public Property IdSystemView As Integer

        <DataMember()>
        Public Property Executions As List(Of roGeniusExecution)
        <DataMember()>
        Public Property ContextMenu As String

        Public Sub initialize(userPassport As Integer, geniusViewBase As roGeniusView, language As String)
            Me.Description = ""
            If Me.Concepts = Nothing Then
                Me.Concepts = ""
            End If
            If Me.Causes = Nothing Then
                Me.Causes = ""
            End If
            If Me.BusinessCenters = Nothing Then
                Me.BusinessCenters = ""
            End If
            If Me.UserFields = Nothing Then
                Me.UserFields = ""
            End If
            Me.CustomFields.LanguageKey = language
            Me.DateFilterType = TypePeriodEnum.PeriodOther
            Me.IdPassport = userPassport
            Me.IdSystemView = geniusViewBase.Id
            Me.Id = -1
            Me.DSFunction = geniusViewBase.DSFunction
            Me.DS = geniusViewBase.DS
            Me.TypeView = geniusViewBase.TypeView
            Me.CubeLayout = geniusViewBase.CubeLayout
            Me.Feature = geniusViewBase.Feature
            Me.RequieredFeature = geniusViewBase.RequieredFeature
            Me.RequieredLicense = geniusViewBase.RequieredLicense
            Me.ContextMenu = geniusViewBase.ContextMenu
        End Sub

        Public Sub initialize()
            If Me.Description = Nothing Then
                Me.Description = ""
            End If
            If Me.Concepts = Nothing Then
                Me.Concepts = ""
            End If
            If Me.Causes = Nothing Then
                Me.Causes = ""
            End If
            If Me.UserFields = Nothing Then
                Me.UserFields = ""
            End If
            If Me.BusinessCenters = Nothing Then
                Me.BusinessCenters = ""
            End If
            If Me.DateFilterType = Nothing Then
                Me.DateFilterType = TypePeriodEnum.PeriodOther
            End If
        End Sub

    End Class

    Public Class roGeniusCheckbox

        Public Sub New()
            Me.Id = -1
            Me.Name = String.Empty
            Me.Classes = New List(Of String)()
            Me.RequieredFeature = String.Empty
            Me.RequieredLicense = String.Empty
        End Sub

        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property Name As String
        <DataMember>
        Public Property Classes As List(Of String)
        <DataMember>
        Public ReadOnly Property Classes2Str As String
            Get
                Dim classesStr As String = ""
                For Each class1 As String In Me.Classes
                    If classesStr = "" Then
                        classesStr = class1
                    Else
                        classesStr += " " + class1
                    End If
                Next
                Return classesStr
            End Get
        End Property
        <DataMember()>
        Public Property RequieredFeature As String

        <DataMember()>
        Public Property RequieredLicense As String

    End Class

    Public Class roGeniusCustomFields

        Public Sub New()
            Me.IncludeZeros = False
            Me.IncludeZeroBusinessCenter = False
            Me.LanguageKey = ""
            Me.RequestTypes = ""
            Me.IncludeZeroCauses = False
            Me.SendEmail = False
            Me.OverwriteResults = False
            Me.DownloadBI = False
        End Sub

        Public Property IncludeZeros As Boolean
        Public Property IncludeZeroCauses As Boolean
        Public Property IncludeZeroBusinessCenter As Boolean

        Public Property LanguageKey As String
        Public Property RequestTypes As String
        Public Property SendEmail As Boolean
        Public Property OverwriteResults As Boolean
        Public Property DownloadBI As Boolean

    End Class

End Namespace