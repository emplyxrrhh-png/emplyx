Imports System.ComponentModel.Design
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Security.Cryptography
Imports System.Text
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace Support

    Public Class roLiveSupport


#Region "nextExecution OptScheduler"

        Public Shared Function GetNextRun(ByVal strScheduler As String, ByVal lastExecutionDate As Nullable(Of Date), ByRef strEx As Exception) As Nullable(Of Date)
            Dim TimeRun As String
            Dim dLastDateTimeExecuted As Date
            Dim mDay As Integer
            Dim mWeeks As String
            Dim mMonths As String
            Dim mDays As String
            Dim mSuccessDay As String
            Dim z As Integer
            Dim CountDays As Integer
            Dim NextDay As Integer
            Dim mWeekDay As String
            Dim mType As String
            Dim GoodDay As Date
            Dim i As Integer
            Dim W As Integer

            Dim strRet As Nullable(Of Date) = Nothing
            strEx = Nothing
            Try
                dLastDateTimeExecuted = System.DateTime.Now
                If lastExecutionDate IsNot Nothing AndAlso lastExecutionDate.HasValue Then
                    dLastDateTimeExecuted = lastExecutionDate
                End If

                TimeRun = String2Item(strScheduler, 1, "@")

                'Constantes para tareas

                Select Case Any2Double(String2Item(strScheduler, 0, "@"))
                    Case 0 ' SUCCEDED_DAILY

                        mDays = Any2Double(String2Item(strScheduler, 2, "@")) - 1

                        If mDays = -1 Then
                            Return Nothing
                        End If

                        strRet = New Date(dLastDateTimeExecuted.Year, dLastDateTimeExecuted.Month, dLastDateTimeExecuted.Day, CInt(TimeRun.Split(":")(0)), CInt(TimeRun.Split(":")(1)), 0).AddDays(mDays)

                        While strRet < System.DateTime.Now
                            strRet = strRet.Value.AddDays(mDays + 1)
                        End While

                    Case 1 ' SUCCEDED_WEEKLY
                        mWeeks = Any2Double(String2Item(strScheduler, 2, "@")) - 1

                        mDays = Any2String(String2Item(strScheduler, 3, "@"))

                        If Any2Double(mDays) = 0 Then
                            Return Nothing
                        End If

                        'Buscamos el siguiente dia de la semana
                        mWeekDay = Weekday(dLastDateTimeExecuted, vbMonday)
                        NextDay = 0
                        If DateTime2Double(Any2Time(dLastDateTimeExecuted).TimeOnly) < DateTime2Double(Any2Time(TimeRun).TimeOnly) And DateTime2Double(Any2Time(TimeRun).TimeOnly) > DateTime2Double(Any2Time(Now).TimeOnly) Then
                            W = mWeekDay
                        Else
                            W = mWeekDay + 1
                        End If

                        For z = W To 7
                            If Mid$(mDays, z, 1) = "1" Then
                                NextDay = z
                                Exit For
                            End If
                        Next z

                        If NextDay > 0 Then
                            strRet = Any2Time(Any2Time(dLastDateTimeExecuted).DateOnly).Add(NextDay - mWeekDay, "d").ValueDateTime
                            strRet = New Date(strRet.Value.Year, strRet.Value.Month, strRet.Value.Day, CInt(TimeRun.Split(":")(0)), CInt(TimeRun.Split(":")(1)), 0)
                        Else

                            NextDay = 0
                            For z = 1 To 7
                                If Mid$(mDays, z, 1) = "1" Then
                                    NextDay = z
                                    Exit For
                                End If
                            Next z

                            mWeeks = mWeeks + Any2Double(String2Item(strScheduler, 2, "@"))
                            strRet = Any2Time(Any2Time(dLastDateTimeExecuted).DateOnly).Add(mWeeks, "ww").ValueDateTime

                            mWeekDay = Weekday(strRet, vbMonday) - NextDay

                            strRet = Any2Time(Any2Time(strRet).DateOnly).Add(mWeekDay * -1, "d").ValueDateTime
                            strRet = New Date(strRet.Value.Year, strRet.Value.Month, strRet.Value.Day, CInt(TimeRun.Split(":")(0)), CInt(TimeRun.Split(":")(1)), 0)
                        End If

                        If strRet < System.DateTime.Now Then
                            'Buscamos el siguiente dia de la semana
                            mWeekDay = 1
                            NextDay = 0
                            For z = mWeekDay To 7
                                If Mid$(mDays, z, 1) = "1" Then
                                    NextDay = z
                                    Exit For
                                End If
                            Next z

                            mWeekDay = Weekday(strRet, vbMonday) - NextDay

                            strRet = Any2Time(Any2Time(strRet).DateOnly).Add(Any2Double(String2Item(strScheduler, 2, "@")), "ww").ValueDateTime
                            strRet = Any2Time(Any2Time(strRet).DateOnly).Add(mWeekDay * -1, "d").ValueDateTime
                            strRet = New Date(strRet.Value.Year, strRet.Value.Month, strRet.Value.Day, CInt(TimeRun.Split(":")(0)), CInt(TimeRun.Split(":")(1)), 0)
                        End If

                    Case 2 ' SUCCEDED_MONTHLY
                        mType = Any2String(String2Item(strScheduler, 2, "@"))

                        If mType = 0 Then
                            'Un dia concreto del mes
                            mMonths = 0
                            mDays = Any2String(String2Item(strScheduler, 3, "@"))

                            If Any2Double(mDays) > 26 Then
                                Dim endDate As New Date(dLastDateTimeExecuted.Year, dLastDateTimeExecuted.Month, 1, 0, 0, 0)
                                Dim oLastDay As Integer = endDate.AddMonths(1).AddDays(-1).Day

                                If oLastDay < Any2Double(mDays) Then
                                    strRet = New Date(dLastDateTimeExecuted.Year, dLastDateTimeExecuted.Month, oLastDay, 0, 0, 0)
                                Else
                                    strRet = New Date(dLastDateTimeExecuted.Year, dLastDateTimeExecuted.Month, mDays, 0, 0, 0)
                                End If
                            Else
                                strRet = New Date(dLastDateTimeExecuted.Year, dLastDateTimeExecuted.Month, mDays, 0, 0, 0)
                            End If

                            strRet = New Date(strRet.Value.Year, strRet.Value.Month, strRet.Value.Day, TimeRun.Split(":")(0), TimeRun.Split(":")(1), 0)

                            While strRet < System.DateTime.Now
                                If Any2Double(mDays) > 26 Then
                                    Dim endDate As New Date(strRet.Value.Year, strRet.Value.Month, 1, 0, 0, 0)
                                    endDate = endDate.AddMonths(1)

                                    Dim oLastDay As Integer = endDate.AddMonths(1).AddDays(-1).Day

                                    If oLastDay < Any2Double(mDays) Then
                                        strRet = New Date(endDate.Year, endDate.Month, oLastDay, 0, 0, 0)
                                    Else
                                        strRet = New Date(endDate.Year, endDate.Month, mDays, 0, 0, 0)
                                    End If
                                Else
                                    strRet = New Date(strRet.Value.Year, strRet.Value.Month, mDays, 0, 0, 0)
                                    strRet = strRet.Value.AddMonths(1)
                                End If

                                strRet = New Date(strRet.Value.Year, strRet.Value.Month, strRet.Value.Day, TimeRun.Split(":")(0), TimeRun.Split(":")(1), 0)

                            End While
                        Else

                            'el primer/segundo/tercer/cuarto/ultimo
                            mSuccessDay = Any2String(String2Item(strScheduler, 3, "@"))
                            'lunes/martes
                            mDay = Any2String(String2Item(strScheduler, 4, "@"))
                            If mDay = 7 Then mDay = 0

                            mMonths = 0

                            strRet = New Date(dLastDateTimeExecuted.Year, dLastDateTimeExecuted.Month, 1, 0, 0, 0)

                            Dim endDate As New Date(dLastDateTimeExecuted.Year, dLastDateTimeExecuted.Month, 1, 0, 0, 0)
                            Dim oLastDay As Integer = endDate.AddMonths(1).AddDays(-1).Day

                            CountDays = 0
                            For i = 1 To oLastDay
                                strRet = New Date(strRet.Value.Year, strRet.Value.Month, i, 0, 0, 0)

                                If strRet.Value.DayOfWeek = mDay Then
                                    GoodDay = strRet
                                    CountDays = CountDays + 1
                                    If mSuccessDay = CountDays Then
                                        Exit For
                                    End If
                                End If

                            Next i

                            strRet = GoodDay
                            strRet = New Date(strRet.Value.Year, strRet.Value.Month, strRet.Value.Day, TimeRun.Split(":")(0), TimeRun.Split(":")(1), 0)

                            GoodDay = Nothing

                            While strRet < System.DateTime.Now
                                mMonths = mMonths + 1
                                strRet = New Date(strRet.Value.Year, strRet.Value.Month, strRet.Value.Day, 0, 0, 0).AddMonths(mMonths)

                                Dim tmpDate As New Date(strRet.Value.Year, strRet.Value.Month, 1, 0, 0, 0)
                                oLastDay = tmpDate.AddMonths(1).AddDays(-1).Day

                                CountDays = 0
                                For i = 1 To oLastDay
                                    strRet = New Date(strRet.Value.Year, strRet.Value.Month, i, 0, 0, 0)

                                    If strRet.Value.DayOfWeek = mDay Then
                                        GoodDay = strRet
                                        CountDays = CountDays + 1
                                        If mSuccessDay = CountDays Then
                                            Exit For
                                        End If
                                    End If
                                Next i

                                strRet = GoodDay
                                strRet = New Date(strRet.Value.Year, strRet.Value.Month, strRet.Value.Day, TimeRun.Split(":")(0), TimeRun.Split(":")(1), 0)
                            End While

                        End If

                    Case 3 ' SUCCEDED_ONEDATE
                        strRet = Any2String(String2Item(strScheduler, 2, "@"))
                        strRet = New Date(strRet.Value.Year, strRet.Value.Month, strRet.Value.Day, TimeRun.Split(":")(0), TimeRun.Split(":")(1), 0)
                        If strRet < System.DateTime.Now Then
                            strRet = Nothing
                        End If
                    Case 4 ' HOURLY
                        strRet = DateTime.Now.AddHours(roTypes.Any2Integer(TimeRun.Split(":")(0))).AddMinutes(roTypes.Any2Integer(TimeRun.Split(":")(1)))
                        If strRet < System.DateTime.Now Then
                            strRet = Nothing
                        End If

                End Select
            Catch ex As Exception
                strEx = ex
            End Try

            Return strRet
        End Function

#End Region


#Region "Audit helper"

        Public Shared Function Audit(ByVal _Action As VTBase.Audit.Action, ByVal _ObjectType As VTBase.Audit.ObjectType, ByVal _ObjectName As String, ByVal _ParametersName As List(Of String), ByVal _ParametersValue As List(Of String), ByVal oState As AuditState.wscAuditState) As Boolean
            Dim oRet As Boolean = False

            Try
                If oState IsNot Nothing Then
                    oState.UpdateStateInfo()
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    If _ParametersName.Count > 0 Then
                        For i As Integer = 0 To _ParametersName.Count - 1
                            oState.AddAuditParameter(tbParameters, _ParametersName(i), _ParametersValue(i), "", 1)
                        Next
                    End If
                    oRet = oState.Audit(_Action, _ObjectType, _ObjectName, tbParameters, -1)
                Else
                    oState = New AuditState.wscAuditState()
                    oState.Result = DTOs.AuditResultEnum.InvalidPassport
                End If
            Catch ex As Exception
                oRet = False
            End Try
            Return oRet
        End Function

#End Region

    End Class

    <DataContract()>
    Public Class roLicenseInfo

        Private _solutions As Generic.List(Of roLicenseSolution)
        Private _modules As Generic.List(Of roLicenseModule)

        Private _concurrentSessions As Long

        Public Sub New()
            _solutions = Nothing
            _modules = Nothing
            _concurrentSessions = 0
        End Sub

        <DataMember()>
        Public ReadOnly Property Solutions As Generic.List(Of roLicenseSolution)
            Get
                Return _solutions
            End Get
        End Property

        <DataMember()>
        Public ReadOnly Property Modules As Generic.List(Of roLicenseModule)
            Get
                Return _modules
            End Get
        End Property

        <DataMember()>
        Public ReadOnly Property ConcurrentSessions As Long
            Get
                Return _concurrentSessions
            End Get
        End Property

        Public Function Load(Optional ByVal strLicInfoFile As String = "") As Boolean
            Try
                Dim strInfoFilePath = String.Empty

                If strLicInfoFile = String.Empty Then
                    Dim oSettings As New roSettings
                    Dim strConfigPath As String = oSettings.GetVTSetting(eKeys.Config)
                    strInfoFilePath = IO.Path.Combine(strConfigPath, "licenseInfo.xml")
                Else
                    strInfoFilePath = strLicInfoFile
                End If

                Dim xmlInfoDoc As New Xml.XmlDocument()
                xmlInfoDoc.Load(strInfoFilePath)

                Dim solutions As Xml.XmlNode = xmlInfoDoc.SelectSingleNode("/LicenseInfo/Solutions")
                Dim modules As Xml.XmlNode = xmlInfoDoc.SelectSingleNode("/LicenseInfo/Modules")

                Dim oServerLicense As New roServerLicense

                Me._solutions = ParseSolutionItems(solutions, oServerLicense)
                Me._modules = ParseModuleItems(modules, oServerLicense)

                Me._concurrentSessions = oServerLicense.FeatureData("VisualTime Server", "MaxConcurrentSessions")

                Return True
            Catch ex As Exception
                Me._solutions = Nothing
                Me._modules = Nothing
                Me._concurrentSessions = 0

                Return False
            End Try

        End Function

        Private Function ParseSolutionItems(ByVal solutions As Xml.XmlNode, ByVal oServerLicense As roServerLicense) As Generic.List(Of roLicenseSolution)
            Dim list As New Generic.List(Of roLicenseSolution)

            Dim solutionNodeList As Xml.XmlNodeList = solutions.SelectNodes("Solution")
            For Each node As Xml.XmlNode In solutionNodeList
                Dim aSolution As New roLicenseSolution

                aSolution.LanguageTag = node.Attributes.GetNamedItem("Code").Value

                Dim empType As String = node.Attributes.GetNamedItem("Employees").Value
                Try
                    aSolution.LicenseEmployees = oServerLicense.FeatureData("VisualTime Server", empType)
                    Dim sql As String = String.Empty
                    Select Case empType
                        Case "MaxEmployees"
                            sql = "@SELECT# COUNT(IDEmployee) AS TotalGH From EmployeeContracts WHERE EndDate > getDate() AND BeginDate<= getDate()"
                        Case "MaxAccEmployees"
                            sql = "@SELECT# COUNT(IDEmployee) AS TotalGH From EmployeeContracts WHERE EndDate > getDate() AND BeginDate<= getDate()"
                        Case "MaxJobEmployees"
                            sql = "@SELECT# COUNT(Employees.ID) AS TotalProd From Employees,EmployeeContracts WHERE Employees.ID = EmployeeContracts.IDEmployee AND EmployeeContracts.EndDate > getDate() AND EmployeeContracts.BeginDate<=getDate() AND Employees.Type ='J'"
                        Case Else
                    End Select

                    aSolution.AvailableLicenseEmployees = ExecuteScalar(sql)
                Catch ex As Exception
                    aSolution.LicenseEmployees = 0
                    aSolution.AvailableLicenseEmployees = 0
                End Try

                aSolution.IsCorrect = True
                aSolution.IsAvailable = oServerLicense.FeatureIsInstalled(node.SelectSingleNode("MainRequierement").InnerText)

                If Not aSolution.IsAvailable Then
                    aSolution.IsCorrect = False
                    aSolution.MissingModules = String.Empty 'node.SelectSingleNode("MainRequierement").InnerText
                Else
                    Dim requierementsNodeList As Xml.XmlNodeList = node.SelectSingleNode("Requierements").SelectNodes("Requierement")

                    If requierementsNodeList IsNot Nothing AndAlso requierementsNodeList.Count > 0 Then
                        For Each reqNode As Xml.XmlNode In requierementsNodeList
                            If Not oServerLicense.FeatureIsInstalled(reqNode.InnerText) Then
                                aSolution.IsCorrect = False
                                aSolution.MissingModules &= reqNode.InnerText & ";"
                                Exit For
                            End If
                        Next

                        If aSolution.MissingModules <> String.Empty Then aSolution.MissingModules = aSolution.MissingModules.Substring(0, aSolution.MissingModules.Length - 1)
                    End If
                End If

                list.Add(aSolution)
            Next

            Return list
        End Function

        Private Function ParseModuleItems(ByVal modules As Xml.XmlNode, ByVal oServerLicense As roServerLicense) As Generic.List(Of roLicenseModule)
            Dim list As New Generic.List(Of roLicenseModule)

            Dim solutionNodeList As Xml.XmlNodeList = modules.SelectNodes("Module")
            For Each node As Xml.XmlNode In solutionNodeList
                Dim aModule As New roLicenseModule

                aModule.LanguageTag = node.Attributes.GetNamedItem("Code").Value

                aModule.IsCorrect = True
                aModule.IsAvailable = oServerLicense.FeatureIsInstalled(node.SelectSingleNode("MainRequierement").InnerText)

                If Not aModule.IsAvailable Then
                    aModule.IsCorrect = False
                    aModule.MissingModules = String.Empty 'node.SelectSingleNode("MainRequierement").InnerText
                Else
                    Dim requierementsNodeList As Xml.XmlNodeList = node.SelectSingleNode("Requierements").SelectNodes("Requierement")

                    If requierementsNodeList IsNot Nothing AndAlso requierementsNodeList.Count > 0 Then
                        For Each reqNode As Xml.XmlNode In requierementsNodeList
                            If Not oServerLicense.FeatureIsInstalled(reqNode.InnerText) Then
                                aModule.IsCorrect = False
                                aModule.MissingModules &= reqNode.InnerText & ";"
                                Exit For
                            End If
                        Next

                        If aModule.MissingModules <> String.Empty Then aModule.MissingModules = aModule.MissingModules.Substring(0, aModule.MissingModules.Length - 1)
                    End If
                End If

                list.Add(aModule)
            Next

            Return list
        End Function

    End Class

End Namespace