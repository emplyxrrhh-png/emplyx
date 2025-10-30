Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class ProfileExportCostCenters

#Region "Declarations- Constructor"

        Private mEmployeeFilterTable As String = ""
        Private mCausesFilter As String = ""
        Private mCentersFilter As String = ""

        Private mBeginDay As Integer = 0
        Private mBeginDate As Date = #12:00:00 AM#
        Private mEndDate As Date = #12:00:00 AM#

        Private mBody As ProfileExportBody

        Private mExportCausesWithDate As Boolean = False
        Private mOutputFileType As ProfileExportBody.FileTypeExport = Nothing
        Private mCausesFilteredBy As Integer
        Private mCentersFilteredBy As Integer

        Private dblField1 As Nullable(Of Double) = Nothing
        Private dblField2 As Nullable(Of Double) = Nothing
        Private strField3 As String = Nothing
        Private strField4 As String = Nothing
        Private xField5 As Nullable(Of Date) = Nothing
        Private xField6 As Nullable(Of Date) = Nothing

        Private oState As roDataLinkState

        Private mRoundCauses As Boolean = False

        Public Sub New(ByVal tmpEmployeeFilterTable As String, ByVal Causesfilter As String, ByVal CentersFilter As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDay As Integer, ByVal _State As roDataLinkState,
                   Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional ByVal RoundCauses As Boolean = False)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeeFilterTable = tmpEmployeeFilterTable
            mCausesFilter = Causesfilter
            mCentersFilter = CentersFilter
            mBeginDay = BeginDay
            mOutputFileType = OutputFileType
            mRoundCauses = RoundCauses
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)
        End Sub

        ' Lanzamiento manual
        Public Sub New(ByVal tmpEmployeeFilterTable As String, ByVal Causesfilter As String, ByVal CentersFilter As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDate As Date, ByVal EndDate As Date, _State As roDataLinkState,
                   Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing, Optional ByVal RoundCauses As Boolean = False)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeeFilterTable = tmpEmployeeFilterTable
            mBeginDate = BeginDate
            mEndDate = EndDate
            mOutputFileType = OutputFileType
            mCausesFilter = Causesfilter
            mCentersFilter = CentersFilter
            mRoundCauses = RoundCauses
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)

        End Sub

#End Region

#Region "Properties"

        Public Property State() As roDataLinkState
            Get
                Return Me.oState
            End Get
            Set(ByVal NewValue As roDataLinkState)

            End Set
        End Property

        Public Property OutputFileType() As Boolean
            Get
                Return Me.mOutputFileType
            End Get
            Set(ByVal NewValue As Boolean)
                Me.mOutputFileType = NewValue
            End Set
        End Property

        Public Property Profile() As ProfileExportBody
            Get
                Return Me.mBody
            End Get
            Set(ByVal value As ProfileExportBody)
                Me.mBody = value
            End Set
        End Property

        Public Property CausesFilteredBy() As Integer
            Get
                Return Me.mCausesFilteredBy
            End Get
            Set(ByVal value As Integer)
                Me.mCausesFilteredBy = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function ExportProfile() As Boolean
            Dim bolCloseFile As Boolean = False
            Dim bolRet As Boolean = False

            Try

                ' Determina la fecha inicial y final para importación automática
                If Me.mBeginDate = #12:00:00 AM# Then
                    ' Importación automática
                    Me.mBeginDate = CDate(Now.Year & "/" & Now.Month & "/" & Me.mBeginDay)

                    ' La exportación siempre es del mes anterior
                    Me.mBeginDate = Me.mBeginDate.AddMonths(-1)

                    ' Si el dia de inicio de exportación es posterior al dia en que se lanza se debe restar un mes adicional para obtener el valor del mes entero.
                    If Now.Day < Me.mBeginDay Then Me.mBeginDate = Me.mBeginDate.AddMonths(-1)
                    Me.mEndDate = Me.mBeginDate.AddMonths(1)
                    Me.mEndDate = Me.mEndDate.AddDays(-1)
                End If

                ' Determina si la exportación es diaria o global
                For i As Integer = 0 To Me.Profile.Fields.Count - 1
                    If Me.Profile.Fields(i).Source.ToUpper = "CC_FECHA" Then
                        Me.mExportCausesWithDate = True
                        Exit For
                    End If
                Next

                ' Campos de la ficha
                Dim dtEmpUsrFields As DataTable = Nothing

                ' Campos de los centros de coste
                Dim dtCCUsrFields As DataTable = CreateDataTable("@SELECT# id, Name from sysroFieldsBusinessCenters", "CCUsrFields")

                ' Definicion de las justificaciones
                Dim dtCauses As DataTable = CreateDataTable("@SELECT# id, Name,RoundingBy,RoundingType,RoundingByDailyScope,convert(numeric(18,6), isnull(CostFactor, 0)) as CostFactor from Causes", "CCCauses")

                ' Crea el fichero de salida
                If Profile.FileOpen() = False Then Exit Try
                bolCloseFile = True

                ' Selecciona los empleados con contrato activo
                Dim sSQL As String = CreateDataAdapter_SQL()
                Dim dtEmployees As DataTable = CreateDataTableWithoutTimeouts(sSQL, , "Employees")

                Dim n As Long = 0
                Dim EmpAnt As Integer = 0

                ' Para cada empleado, contrato y rotura
                For Each Row As DataRow In dtEmployees.Rows
                    n += 1

                    ' Campos de la ficha
                    If EmpAnt <> Row("idEmployee") Then
                        dtEmpUsrFields = DataLinkDynamicCode.CreateDataTable_EmployeeUserFields(Row("idEmployee"), Me.mEndDate, Me.oState)
                        EmpAnt = Row("idEmployee")
                    End If

                    ' Carga datos del registro de empleado
                    If Not LoadInfo(dtEmployees, Row, dtEmpUsrFields) Then Exit For

                    ' Asigna valores
                    Dim strAccrualKey As String = ""
                    Dim PreviousDate As Date = #12:00:00 AM#

                    ' Exporta justificaciones
                    ExportCausesOneConceptByLine(dtCCUsrFields, dtEmployees, dtCauses, Row)
                Next

                'Se deja un mensaje informativo dentro del archivo ascii
                If Profile.MemoryStreamWriter IsNot Nothing AndAlso Profile.MemoryStreamWriter.Length = 0 Then
                    Dim textErrror As Byte() = System.Text.Encoding.Unicode.GetBytes(Me.oState.Language.Translate("roDataLinkExport.ExportProfile.NoInfo", ""))
                    Profile.MemoryStreamWriter.Write(textErrror, 0, textErrror.Length)
                End If

                If Not IsNothing(dtEmpUsrFields) Then dtEmpUsrFields.Dispose()

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportCostCenters:ExportProfile")
            Finally
                ' Cierra el fichero
                If bolCloseFile Then Me.Profile.FileClose()

            End Try

            Return bolRet

        End Function

        Private Function ExportCausesOneConceptByLine(dtCCUserFields As DataTable, ByVal dtRowsToExport As DataTable, ByVal dtCauses As DataTable, ByVal cRow As DataRow)
            Dim bolRet As Boolean = False

            Try
                Dim strAccrualKey As String = ""
                Dim i As Integer = 0
                Dim rows() As DataRow
                Dim idCC As String = ""

                ' Exporta un saldo por línea
                For i = 0 To Me.Profile.Fields.Count - 1
                    If Profile.Fields(i).Source.Length > "CC_".Length AndAlso Profile.Fields(i).Source.ToUpper.Substring(0, 3) = "CC_" Then
                        strAccrualKey = Profile.Fields(i).Source.Split("_")(1)
                        Select Case strAccrualKey.ToUpper
                            Case "V"
                                If mRoundCauses And mExportCausesWithDate Then
                                    ' Si se tienen que redondear los valores de las justificaciones
                                    Dim intIDCause As Integer = Any2Integer(cRow("IDCause"))
                                    Dim RoundingByDailyScope As Boolean = False
                                    Dim RoundingBy As Double = 1
                                    Dim RoundingType As eRoundingType = eRoundingType.Round_Near

                                    rows = dtCauses.Select("ID=" & cRow("IDCause"))
                                    If rows.Length > 0 Then
                                        RoundingByDailyScope = Any2Boolean(rows(0)("RoundingByDailyScope"))
                                        RoundingBy = Any2Double(rows(0)("RoundingBy"))
                                        Select Case Any2String(rows(0)("RoundingType"))
                                            Case "+"
                                                RoundingType = eRoundingType.Round_UP
                                            Case "-"
                                                RoundingType = eRoundingType.Round_Down
                                            Case "~"
                                                RoundingType = eRoundingType.Round_Near
                                        End Select

                                        ' Si se tiene que redondear el valor diario
                                        If RoundingByDailyScope And RoundingBy <> 1 Then
                                            Dim CurrentValue As Double
                                            If cRow("TotalValue") >= 0 Then
                                                CurrentValue = Any2Time(cRow("TotalValue"), True).VBNumericValue
                                            Else
                                                CurrentValue = Any2Time(cRow("TotalValue") * -1, True).VBNumericValue
                                            End If

                                            ' Si esta causa se redondea diariamente lo hace ahora
                                            CurrentValue = RoundTime(CurrentValue, RoundingType, RoundingBy)

                                            ' Ahora pasa a formato Robotics
                                            If cRow("TotalValue") >= 0 Then
                                                Profile.Fields(i).Value = Any2Time(Date.FromOADate(CurrentValue), True).NumericValue(True)
                                            Else
                                                ' Los negativos hay que tratarlos de manera especial
                                                Profile.Fields(i).Value = Any2Time(Date.FromOADate(CurrentValue * -1), True).NumericValue(True) * -1
                                            End If

                                            ' Si se tiene que redondear el valor individualmente
                                        ElseIf (Not RoundingByDailyScope) And RoundingBy <> 1 Then
                                            ' Volvemos a obtener los valores diarios de la justificacion para ese empleado/fecha/centro de coste
                                            ' y redondeamos los valores individualmente

                                            Dim dtCCDaily As DataTable = CreateDataTable("@SELECT# value from DailyCauses where idemployee=" & cRow("idemployee") & " and Date=" & Any2Time(cRow("Date")).SQLSmallDateTime & " AND IDCause=" & cRow("IDCause") & " AND IDCenter=" & Any2Integer(cRow("IDCenter")), "DailyCause")
                                            Dim CurrentSumValue As Double = 0
                                            If dtCCDaily IsNot Nothing AndAlso dtCCDaily.Rows.Count > 0 Then
                                                For Each oDaily As DataRow In dtCCDaily.Rows
                                                    Dim CurrentValue As Double = 0
                                                    If oDaily("Value") >= 0 Then
                                                        CurrentValue = Any2Time(oDaily("Value"), True).VBNumericValue
                                                    Else
                                                        CurrentValue = Any2Time(oDaily("Value") * -1, True).VBNumericValue
                                                    End If

                                                    Try
                                                        If oDaily("Value") >= 0 Then
                                                            CurrentSumValue = CurrentSumValue + RoundTime(CurrentValue, RoundingType, RoundingBy)
                                                        Else
                                                            CurrentSumValue = CurrentSumValue + (RoundTime(CurrentValue, RoundingType, RoundingBy) * -1)
                                                        End If
                                                    Catch ex As Exception
                                                    End Try
                                                Next
                                            End If

                                            ' Ahora el total diario lo pasamos a formato Robotics
                                            If CurrentSumValue >= 0 Then
                                                Profile.Fields(i).Value = Any2Time(Date.FromOADate(CurrentSumValue), True).NumericValue(True)
                                            Else
                                                ' Los negativos hay que tratarlos de manera especial
                                                Profile.Fields(i).Value = Any2Time(Date.FromOADate(CurrentSumValue * -1), True).NumericValue(True) * -1
                                            End If
                                        Else
                                            ' Si no tiene que redondear nada
                                            Profile.Fields(i).Value = cRow("TotalValue")
                                        End If
                                    End If
                                Else
                                    Profile.Fields(i).Value = cRow("TotalValue")
                                End If

                            Case "NC" : Profile.Fields(i).Value = cRow("CauseShortName")
                            Case "FECHA" : Profile.Fields(i).Value = cRow("Date")
                            Case "NOMBREJUSTIFICACION" : Profile.Fields(i).Value = cRow("CauseName")
                            Case "EQUIVALENCIA" : Profile.Fields(i).Value = cRow("CauseExport")
                            Case "NOMBRECENTROCOSTE" : Profile.Fields(i).Value = cRow("CenterName")
                            Case "FACTORJUSTIFICACION" : Profile.Fields(i).Value = cRow("CauseCostFactor")
                            Case "USR"
                                idCC = Profile.Fields(i).Source.Split("_")(2)
                                rows = dtCCUserFields.Select("Name='" & idCC & "'")
                                If rows.Length > 0 Then
                                    Profile.Fields(i).Value = Any2String(cRow("Field" & rows(0)("id")))
                                End If

                            Case Else
                                If InStr(1, Profile.Fields(i).Source.ToUpper, "CC_RBS_") Then ' Robotics script
                                    Dim scriptFileName As String = Profile.Fields(i).Source.Substring(7, Profile.Fields(i).Source.Length - 7)
                                    Profile.Fields(i).Value = "RBS not supported"
                                End If
                        End Select
                    End If
                Next i

                ' Graba la línea
                Me.Profile.CreateLine()

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:ExportOneConceptByLine")
            End Try

            Return bolRet
        End Function

        Private Function RoundTime(ByVal VBTime As Double, ByVal RoundingType As eRoundingType, ByVal RoundingBy As Integer) As Double
            '
            ' Redondea un tiempo
            '
            Dim TotalMinutes As Long
            Dim RoundedMinutes As Double
            Dim Divergence As Double
            Dim Base As Double
            Dim bolRet As Double = 0

            Try

                ' Obtiene tiempo a justificar en minutos
                'TotalMinutes = DateDiff("n", "00:00", Date.FromOADate(VBTime))
                TotalMinutes = DateDiff("n", "1899/12/30", Date.FromOADate(VBTime))

                ' Calcula tiempo redondeado
                If RoundingBy <> 0 Then Divergence = TotalMinutes Mod RoundingBy
                Base = TotalMinutes - Divergence

                Select Case RoundingType
                    Case eRoundingType.Round_Near   ' Por aproximación
                        RoundedMinutes = Base
                        If Divergence > (RoundingBy / 2) Then RoundedMinutes = RoundedMinutes + RoundingBy

                    Case eRoundingType.Round_UP   ' Por exceso
                        RoundedMinutes = Base
                        If Divergence > 0 Then RoundedMinutes = RoundedMinutes + RoundingBy

                    Case eRoundingType.Round_Down   ' Por defecto
                        RoundedMinutes = Base
                End Select

                ' Devuelve el tiempo redondeado en formato VB
                'bolRet = Any2Double(DateAdd("n", RoundedMinutes, "00:00"))
                bolRet = Any2Double(DateAdd("n", RoundedMinutes, "1899/12/30"))
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts: RoundTime")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts: RoundTime")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function LoadInfo(ByVal dtRowsToExport As DataTable, ByVal row As DataRow, ByVal dtEmpUsrFields As DataTable) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim bolGroup1 As Boolean = False
                Dim strAccrualKey As String = ""
                Dim i As Integer = 0

                Dim dt As New DataTable

                ' Determina el empleado
                Dim idEmpleado As Long = row("idEmployee")

                ' Para cada columna

                For i = 0 To Me.Profile.Fields.Count - 1
                    Profile.Fields(i).Value = ""

                    Select Case Profile.Fields(i).Source.ToUpper
                        Case "FECHA_INICIO_EXPORTACION", "FECHA_INICIO_EXPORTACIÓN"
                            Profile.Fields(i).Value = mBeginDate

                        Case "FECHA_FINAL_EXPORTACION", "FECHA_FINAL_EXPORTACIÓN"
                            Profile.Fields(i).Value = mEndDate

                        Case "GRUPO"
                            Profile.Fields(i).Value = row("GroupName")

                        Case "GRUPO_COMPLETO"
                            Profile.Fields(i).Value = row("FullGroupName")

                        Case "NOMBRE"
                            Profile.Fields(i).Value = row("EmployeeName")

                        Case "CONTRATO"
                            Profile.Fields(i).Value = row("idContract")

                        Case "CONTRATO_FECHA_INICIO"
                            Profile.Fields(i).Value = row("BeginDate")

                        Case "CONTRATO_FECHA_FINAL"
                            Profile.Fields(i).Value = row("EndDate")

                        Case Else
                            ' Determina el tipo de campo
                            If InStr(1, Profile.Fields(i).Source.ToUpper, "USR_") Then
                                ' Lee el dato del campo de la ficha
                                If dtEmpUsrFields.Rows.Count > 0 Then
                                    Dim FieldName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                    Dim r() As DataRow = dtEmpUsrFields.Select("FieldName='" & FieldName & "'")
                                    If r.Length > 0 AndAlso Not IsDBNull(r(0)("Value")) Then Me.Profile.Fields(i).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue(r(0)("Value"), r(0)("FieldType"))
                                End If

                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "LITERAL_") Then
                                Profile.Fields(i).Value = Profile.Fields(i).Source.Substring(8, Profile.Fields(i).Source.Length - 8)
                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "CC_") Then ' Justificaciones
                                If InStr(1, Profile.Fields(i).Source.ToUpper, "CC_RBS") = 0 Then
                                    If UBound(Profile.Fields(i).Source.Split("_")) = 1 Then bolGroup1 = True
                                    If UBound(Profile.Fields(i).Source.Split("_")) > 1 Then
                                        strAccrualKey = Profile.Fields(i).Source.Split("_")(2)
                                        Profile.Fields(i).ShortName = strAccrualKey.ToUpper
                                    End If
                                End If
                            End If
                    End Select
                Next

                dt.Dispose()

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:LoadInfo")
            End Try

            Return bolRet
        End Function

        Private Function CreateDataAdapter_SQL() As String
            Dim strSQL As String = ""

            strSQL = "" &
            "@SELECT# DAT.*,EG.IDGroup, EC.BeginDate, EC.EndDate, " &
            "   C.Name AS CauseName, C.ShortName AS CauseShortName, C.Export AS CauseExport, convert(numeric(18,6), isnull(c.CostFactor, 0)) as CauseCostFactor, " &
            "   isnull(BC.Name, '') AS CenterName,  isnull(BC.Field1, '') AS Field1,  isnull(BC.Field2, '') AS Field2 ,  isnull(BC.Field3, '') AS Field3,  isnull(BC.Field4, '') AS Field4, " &
            "   E.Name AS EmployeeName, Groups.Name AS GroupName, " &
            "   Groups.FullGroupName " &
            "from " &
            "   (@SELECT# DC.NumContrato as idContract, DC.IDEmployee, DC.IDCause, DC.IDCenter, "

            If mExportCausesWithDate = False Then
                strSQL += "sum(DC.Value) AS TotalValue "
            Else
                strSQL += "sum(DC.Value) AS TotalValue, DC.Date  "
            End If

            strSQL += " From sysroDailyCausesByContract DC with (nolock) "
            If Me.mEmployeeFilterTable <> "" Then strSQL &= " INNER JOIN " & Me.mEmployeeFilterTable & " ON " & Me.mEmployeeFilterTable & ".Id = DC.IDEmployee "

            strSQL += " WHERE DC.DATE BETWEEN " & Any2Time(mBeginDate).SQLSmallDateTime & " and " & Any2Time(mEndDate).SQLSmallDateTime

            If Me.mCausesFilter <> "" Then strSQL += " and DC.idCause in (" & Me.mCausesFilter & ") "
            If Me.mCentersFilter <> "" Then strSQL += " and DC.idCenter in (" & Me.mCentersFilter & ") "

            ' Solo exportamos los centros de coste que el supervisor pueda gestionar
            strSQL += " and (DC.IDCenter IN (@SELECT# DISTINCT IDCostCenter FROM sysrovwSecurity_PermissionOverCostCenters c WHERE c.IDPassport= " & Me.oState.IDPassport.ToString & ") or DC.IDCenter=0) "


            If mExportCausesWithDate = False Then
                strSQL += "group by DC.NumContrato,DC.IDEmployee, dc.idCause, DC.idcenter"
            Else
                strSQL += "group by DC.NumContrato, DC.IDEmployee, DC.IDCAUSE, DC.IDCENTER, DC.Date "
            End If
            strSQL += ") as DAT " &
            "   INNER JOIN EmployeeGroups EG with (nolock) ON EG.IDEmployee =DAT.IDEmployee AND GETDATE() BETWEEN EG.BeginDate AND EG.EndDate " &
            "   INNER JOIN Causes C with (nolock) on DAT.idCause=C.id	" &
            "   INNER JOIN EmployeeContracts EC with (nolock) ON DAT.IDEmployee = EC.IDEmployee AND DAT.idContract=EC.idContract " &
            "   INNER JOIN Groups with (nolock) ON EG.IDGroup = Groups.ID " &
            "   LEFT OUTER JOIN Employees E with (nolock) ON DAT.IDEmployee = E.ID " &
            "   LEFT OUTER JOIN BusinessCenters BC with (nolock) ON DAT.IDCenter = BC.ID " &
            "ORDER BY EmployeeName "
            If mExportCausesWithDate = True Then strSQL += ", Date"
            Return strSQL
        End Function

#End Region

    End Class

End Namespace
