Imports System.Data.Common
Imports System.Globalization
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes
Imports Robotics.Base.VTSelectorManager.roSelectorManager

Namespace UserFields

    <DataContract>
    <Serializable>
    Public Class roEmployeeUserField

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roUserFieldState

        Private intIDEmployee As Integer
        Private strFieldName As String
        Private strLocalizedFieldName As String
        Private ObjFieldValue As Object
        Private xDate As Date
        Private xOriginalDate As Nullable(Of Date)
        Private strFieldRawValue As String

        'Private StrFieldCaption As String
        'Private eFieldType As UserField.FieldTypes
        Private oDefinition As roUserField

        Public Sub New()
            Me.oState = New roUserFieldState()
            Me.intIDEmployee = -1
            Me.xDate = New Date(1900, 1, 1)
        End Sub

        Public Sub New(ByVal _State As roUserFieldState)
            Me.oState = _State
            Me.intIDEmployee = -1
            Me.xDate = New Date(1900, 1, 1)
        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _FieldName As String, ByVal _FieldDate As Date, ByVal _State As roUserFieldState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.IDEmployee = _IDEmployee
            Me.FieldName = _FieldName
            Me._Date = _FieldDate
            Me.Load(bAudit)
        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal oDataRow As DataRow, ByVal _State As roUserFieldState)
            Me.oState = _State
            Me.intIDEmployee = _IDEmployee
            Me.xDate = New Date(1900, 1, 1)
            If oDataRow IsNot Nothing Then Me.LoadFromDataRow(oDataRow)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roUserFieldState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roUserFieldState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property IDEmployee() As Integer
            Get
                Return Me.intIDEmployee
            End Get
            Set(ByVal value As Integer)
                Me.intIDEmployee = value
            End Set
        End Property

        <DataMember>
        Public Property FieldName() As String
            Get
                Return strFieldName
            End Get
            Set(ByVal value As String)
                ' Filtramos el nombre de campo, por si viene con el prefijo USR_ .
                If value.ToUpper.StartsWith("[USR_") Then
                    value = value.Substring(5)
                    value = value.Substring(0, value.Length - 1)
                ElseIf value.ToUpper.StartsWith("USR_") Then
                    value = value.Substring(4)
                End If
                strFieldName = value
            End Set
        End Property

        <DataMember>
        Public Property LocalizedFieldName() As String
            Get
                Return strLocalizedFieldName
            End Get
            Set(ByVal value As String)
                strLocalizedFieldName = value
            End Set
        End Property

        <DataMember>
        Public Property FieldValue() As Object
            Get
                Return ObjFieldValue
            End Get
            Set(ByVal value As Object)
                If IsDBNull(value) Then
                    ObjFieldValue = ""
                Else
                    ObjFieldValue = value
                End If
            End Set
        End Property

        <DataMember>
        Public Property FieldRawValue() As String
            Get
                If Not strFieldRawValue Is Nothing Then
                    Return Any2String(strFieldRawValue).Replace(".", roConversions.GetDecimalDigitFormat)
                Else
                    Return String.Empty
                End If
            End Get
            Set(value As String)
                strFieldRawValue = value
            End Set
        End Property

        <DataMember>
        Public Property _Date() As Date
            Get
                Return Me.xDate
            End Get
            Set(ByVal value As Date)
                Me.xDate = value
            End Set
        End Property

        <DataMember>
        Public Property Definition() As roUserField
            Get
                Return Me.oDefinition
            End Get
            Set(ByVal value As roUserField)
                Me.oDefinition = value
            End Set
        End Property

        <DataMember>
        Public Property OriginalDate() As Nullable(Of Date)
            Get
                Return Me.xOriginalDate
            End Get
            Set(ByVal value As Nullable(Of Date))
                Me.xOriginalDate = value
            End Set
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Comparación de objetos EmployeeUserField
        ''' </summary>
        ''' <param name="oObject"></param>
        ''' <returns>Devuelve 1 si el objeto pasado como parámetro es mayor que el de instancia</returns>

        Public Function IsEqual(ByVal oCompareTo As roEmployeeUserField) As Boolean
            Dim oRet As Boolean = False
            Try

                ' Aproximación 1: Tipos distintos => son diferentes
                If Me._Date <> oCompareTo._Date Then
                    oRet = False
                ElseIf Me.FieldValue IsNot Nothing AndAlso oCompareTo IsNot Nothing Then
                    Select Case Me.oDefinition.FieldType
                        Case FieldTypes.tText
                            oRet = (CStr(Me.FieldValue).Trim.ToUpper = CStr(oCompareTo.FieldValue).Trim.ToUpper)
                        Case FieldTypes.tNumeric
                            oRet = (CLng(Me.FieldValue) = CLng(oCompareTo.FieldValue))
                        Case FieldTypes.tDecimal

                            Dim ci As CultureInfo = CultureInfo.GetCultureInfo("ES-es")
                            Dim originalUserField As Double = Convert.ToDouble(CStr(oCompareTo.FieldValue).Replace(roConversions.GetDecimalDigitFormat, ".").Trim.ToUpper, ci)
                            Dim newUserField As Double = Convert.ToDouble(CStr(Me.FieldValue).Replace(roConversions.GetDecimalDigitFormat, ".").Trim.ToUpper, ci)

                            oRet = (originalUserField = newUserField)

                        Case FieldTypes.tDate
                            oRet = (Format(CDate(Me.FieldValue), "yyyy/MM/dd") = Format(CDate(oCompareTo.FieldValue), "yyyy/MM/dd"))
                        Case FieldTypes.tDatePeriod
                            'Son cadenas de texto con formato fecha1*fecha2
                            If CStr(Me.FieldValue).Contains("*") AndAlso CStr(oCompareTo.FieldValue).Contains("*") Then
                                Dim MeDateFrom As String = CStr(Me.FieldValue).Split("*")(0).Trim
                                Dim MeDateTo As String = CStr(Me.FieldValue).Split("*")(1).Trim
                                Dim CompareToDateFrom As String = CStr(oCompareTo.FieldValue).Split("*")(0).Trim
                                Dim CompareToDateTo As String = CStr(oCompareTo.FieldValue).Split("*")(1).Trim

                                If MeDateFrom = String.Empty Then MeDateFrom = "1900/01/01"
                                If MeDateTo = String.Empty Then MeDateFrom = "1900/01/01"
                                If CompareToDateFrom = String.Empty Then MeDateFrom = "1900/01/01"
                                If CompareToDateTo = String.Empty Then MeDateFrom = "1900/01/01"

                                oRet = (Format(CDate(MeDateFrom), "yyyy/MM/dd") = Format(CDate(CompareToDateFrom), "yyyy/MM/dd")) AndAlso (Format(CDate(MeDateTo), "yyyy/MM/dd") = Format(CDate(CompareToDateTo), "yyyy/MM/dd"))
                            ElseIf CStr(Me.FieldValue).Contains("*") OrElse CStr(oCompareTo.FieldValue).Contains("*") Then
                                oRet = False
                            End If
                        Case FieldTypes.tList
                            oRet = (CStr(Me.FieldValue).Trim.ToUpper = CStr(oCompareTo.FieldValue).Trim.ToUpper)
                        Case FieldTypes.tTime
                            ' TODO: Revisar
                            If CStr(Me.FieldValue).Trim.ToUpper.Contains(":") Then
                                oRet = CStr(oCompareTo.FieldValue).Trim.ToUpper.Contains(":") AndAlso (CStr(roConversions.ConvertTimeToHours(CStr(Me.FieldValue))).Replace(roConversions.GetDecimalDigitFormat, ".").Trim = CStr(roConversions.ConvertTimeToHours(CStr(oCompareTo.FieldValue))).Replace(roConversions.GetDecimalDigitFormat, ".").Trim)
                            ElseIf CStr(Me.FieldValue).Contains(".") Then
                                oRet = CStr(oCompareTo.FieldValue).Trim.ToUpper.Contains(".") AndAlso (CStr(Me.FieldValue).Trim.ToUpper = CStr(oCompareTo.FieldValue).Trim.ToUpper)
                            Else
                                oRet = Any2Double(Me.FieldValue) = Any2Double(oCompareTo.FieldValue)
                            End If
                        Case FieldTypes.tTimePeriod
                            'TODO: Revisar
                            oRet = (CStr(Me.FieldValue).Trim.ToUpper = CStr(oCompareTo.FieldValue).Trim.ToUpper)
                        Case FieldTypes.tLink
                            oRet = (CStr(Me.FieldValue).Trim.ToUpper = CStr(oCompareTo.FieldValue).Trim.ToUpper)
                        Case FieldTypes.tDocument
                            oRet = (CLng(Me.FieldValue) = CLng(oCompareTo.FieldValue))
                        Case Else
                            oRet = (CStr(Me.FieldValue).Trim.ToUpper = CStr(oCompareTo.FieldValue).Trim.ToUpper)
                    End Select
                ElseIf Me.FieldValue Is Nothing AndAlso oCompareTo Is Nothing Then
                    oRet = True
                Else
                    oRet = False
                End If

                Return oRet
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeUserField::IsEqual")
                Return False
            End Try
        End Function

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                Me.xOriginalDate = Me.xDate

                Dim strSQL As String

                Dim tb As New DataTable

                ' Recuperar el valor del campo de la ficha
                strSQL = "@SELECT# * FROM EmployeeUserFieldValues " &
                         "WHERE IDEmployee = " & IDEmployee.ToString & " AND " &
                               "FieldName = @FieldName AND " &
                               "Date = @Date"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                AddParameter(cmd, "@FieldName", DbType.String, 50)
                AddParameter(cmd, "@Date", DbType.Date)
                cmd.Parameters("@FieldName").Value = Me.FieldName
                cmd.Parameters("@Date").Value = Me.xOriginalDate.Value

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                ' Obtenemos la definición del campo de la ficha
                Me.oDefinition = Me.GetDefinition()

                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    If Not IsDBNull(tb.Rows(0).Item("Value")) Then
                        Me.FieldRawValue = Any2String(tb.Rows(0).Item("Value"))
                        Select Case Me.oDefinition.FieldType
                            Case FieldTypes.tText
                                Me.FieldValue = CStr(tb.Rows(0).Item("Value"))
                            Case FieldTypes.tNumeric
                                Me.FieldValue = Any2Long(tb.Rows(0).Item("Value"))
                            Case FieldTypes.tDecimal
                                Me.FieldValue = Any2Double(CStr(tb.Rows(0).Item("Value")).Replace(".", roConversions.GetDecimalDigitFormat))
                            Case FieldTypes.tDate
                                If CStr(tb.Rows(0).Item("Value")).Split("/").Length = 3 Then
                                    Try
                                        Me.FieldValue = New Date(Any2Integer(CStr(tb.Rows(0).Item("Value")).Split("/")(0)), Any2Integer(CStr(tb.Rows(0).Item("Value")).Split("/")(1)), Any2Integer(CStr(tb.Rows(0).Item("Value")).Split("/")(2)))
                                    Catch ex As Exception
                                        Dim tmpDate As DateTime
                                        If DateTime.TryParse(CStr(tb.Rows(0).Item("Value")), tmpDate) Then
                                            Me.FieldValue = tmpDate
                                        Else
                                            Me.FieldValue = DBNull.Value
                                        End If

                                    End Try

                                End If
                            Case FieldTypes.tDatePeriod
                                'TODO: Sólo se deberían aceptar cadenas de texto con formato fecha1*fecha2
                                Me.FieldValue = CStr(tb.Rows(0).Item("Value"))
                            Case FieldTypes.tList
                                Me.FieldValue = CStr(tb.Rows(0).Item("Value"))
                            Case FieldTypes.tTime
                                If CStr(tb.Rows(0).Item("Value")) <> "" Then
                                    If CStr(tb.Rows(0).Item("Value")).IndexOf(":") < 0 Then
                                        Me.FieldValue = roConversions.ConvertHoursToTime(Any2Double(CStr(tb.Rows(0).Item("Value")).Replace(".", roConversions.GetDecimalDigitFormat)))
                                    Else
                                        Me.FieldValue = CStr(tb.Rows(0).Item("Value"))
                                    End If
                                Else
                                    Me.FieldValue = DBNull.Value
                                End If
                            Case FieldTypes.tTimePeriod
                                Me.FieldValue = CStr(tb.Rows(0).Item("Value"))
                            Case FieldTypes.tLink
                                Me.FieldValue = CStr(tb.Rows(0).Item("Value"))
                            Case FieldTypes.tDocument
                                Me.FieldValue = Any2Long(tb.Rows(0).Item("Value"))

                        End Select
                    Else
                        Me.FieldValue = DBNull.Value
                        Me.FieldRawValue = String.Empty
                    End If

                    bolRet = True

                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{IDEmployee}", Me.intIDEmployee, "", 1)
                    oState.AddAuditParameter(tbParameters, "{FieldName}", Me.strFieldName, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tUserField, Me.strFieldName, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployeeUserField::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeUserField::Load")
            End Try

            Return bolRet

        End Function

        Public Function LoadFromDataRow(ByVal oRow As DataRow) As Boolean

            Dim bolRet As Boolean = False

            Try

                Me.xOriginalDate = Me.xDate

                Me.FieldName = oRow("FieldName")
                Me.LocalizedFieldName = oRow("FieldName") ' TODO: Traducir campo de la ficha al idioma del usuario, si es de los campos de la ficha de sistema o existentes por defecto en BBDD inicial
                If Not IsDBNull(oRow("Date")) Then
                    Me.xDate = oRow("Date")
                Else
                    Me.xDate = New Date(1900, 1, 1)
                End If

                Me.oDefinition = Me.GetDefinition()

                If Not IsDBNull(oRow.Item("Value")) Then
                    Select Case Me.oDefinition.FieldType
                        Case FieldTypes.tText
                            Me.FieldValue = CStr(oRow.Item("Value"))
                        Case FieldTypes.tNumeric
                            Me.FieldValue = Any2Long(oRow.Item("Value"))
                        Case FieldTypes.tDecimal
                            Me.FieldValue = CStr(oRow.Item("Value")).Replace(".", roConversions.GetDecimalDigitFormat)
                        Case FieldTypes.tDate
                            If CStr(oRow.Item("Value")).Split("/").Length = 3 Then
                                Try
                                    Me.FieldValue = New Date(Any2Integer(CStr(oRow.Item("Value")).Split("/")(0)), Any2Integer(CStr(oRow.Item("Value")).Split("/")(1)), Any2Integer(CStr(oRow.Item("Value")).Split("/")(2)))
                                Catch ex As Exception
                                    Dim tmpDate As DateTime
                                    If DateTime.TryParse(CStr(oRow.Item("Value")), tmpDate) Then
                                        Me.FieldValue = tmpDate
                                    Else
                                        Me.FieldValue = DBNull.Value
                                    End If
                                End Try
                            End If
                        Case FieldTypes.tDatePeriod
                            'TODO: Sólo se deberían aceptar cadenas de texto con formato fecha1*fecha2
                            Me.FieldValue = CStr(oRow.Item("Value"))
                        Case FieldTypes.tList
                            Me.FieldValue = CStr(oRow.Item("Value"))
                        Case FieldTypes.tTime
                            If CStr(oRow.Item("Value")) <> "" Then
                                If CStr(oRow.Item("Value")).IndexOf(":") < 0 Then
                                    Me.FieldValue = roConversions.ConvertHoursToTime(Any2Double(CStr(oRow.Item("Value")).Replace(".", roConversions.GetDecimalDigitFormat)))
                                Else
                                    Me.FieldValue = CStr(oRow.Item("Value"))
                                End If
                            Else
                                Me.FieldValue = DBNull.Value
                            End If
                        Case FieldTypes.tTimePeriod
                            Me.FieldValue = CStr(oRow.Item("Value"))
                        Case FieldTypes.tLink
                            Me.FieldValue = CStr(oRow.Item("Value"))
                        Case FieldTypes.tDocument
                            Me.FieldValue = Any2Long(oRow.Item("Value"))

                    End Select
                Else
                    Me.FieldValue = DBNull.Value
                End If

                bolRet = True

                If bolRet Then
                    ' Auditamos

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployeeUserField::LoadFromDataRow")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeUserField::LoadFromDataRow")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bolRecalculate As Boolean = True, Optional ByVal bAudit As Boolean = False, Optional ByVal bCalculatedUsedInProcess As Boolean = True) As Boolean

            Dim bolRet As Boolean = False
            Try

                Dim bIsNew As Boolean = False
                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim oOldEmployeeUserField As roEmployeeUserField = Nothing

                Dim tb As New DataTable


                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = UserFieldResultEnum.XSSvalidationError
                    Return False
                End If

                Dim strSQL As String
                strSQL = "@SELECT# * FROM EmployeeUserFieldValues " &
                         "WHERE IDEmployee = " & Me.IDEmployee.ToString & " AND " &
                               "FieldName = @FieldName AND " &
                               "Date = @Date"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                AddParameter(cmd, "@FieldName", DbType.String, 50)
                AddParameter(cmd, "@Date", DbType.Date)
                cmd.Parameters("@FieldName").Value = Me.FieldName
                cmd.Parameters("@Date").Value = Me.xOriginalDate.Value

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow = Nothing
                If tb.Rows.Count = 0 Then
                    bIsNew = True
                    oRow = tb.NewRow
                    oRow("IDEmployee") = Me.IDEmployee
                    oRow("FieldName") = Me.FieldName
                    tb.Rows.Add(oRow)
                Else
                    oOldEmployeeUserField = New roEmployeeUserField(Me.IDEmployee, Me.FieldName, Me.xOriginalDate.Value, Me.State)
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                End If

                oRow("Date") = Me._Date.Date

                If Me.FieldValue IsNot Nothing AndAlso Not IsDBNull(Me.FieldValue) Then
                    Me.strFieldRawValue = CStr(Me.FieldValue)
                Else
                    Me.strFieldRawValue = String.Empty
                End If

                ' Obtenemos la definición del campo de la ficha
                If Me.oDefinition Is Nothing Then Me.oDefinition = Me.GetDefinition()
                If Me.oDefinition IsNot Nothing Then
                    Select Case Me.oDefinition.FieldType
                        Case FieldTypes.tText
                            If Me.FieldValue IsNot Nothing AndAlso Not IsDBNull(Me.FieldValue) Then
                                oRow("Value") = CStr(Me.FieldValue)
                            Else
                                oRow("Value") = DBNull.Value
                            End If
                        Case FieldTypes.tNumeric
                            If Me.FieldValue IsNot Nothing AndAlso Not IsDBNull(Me.FieldValue) AndAlso IsNumeric(Me.FieldValue) Then
                                oRow("Value") = CLng(Me.FieldValue)
                            Else
                                oRow("Value") = DBNull.Value
                            End If
                        Case FieldTypes.tDecimal
                            If Me.FieldValue IsNot Nothing AndAlso Not IsDBNull(Me.FieldValue) AndAlso IsNumeric(Me.FieldValue) Then
                                ''oRow("Value") = CDbl(Me.FieldValue)
                                oRow("Value") = CStr(Me.FieldValue).Replace(roConversions.GetDecimalDigitFormat, ".")
                            Else
                                oRow("Value") = DBNull.Value
                            End If
                        Case FieldTypes.tDate
                            If Me.FieldValue IsNot Nothing AndAlso Not IsDBNull(Me.FieldValue) AndAlso IsDate(Me.FieldValue) Then
                                oRow("Value") = Format(CDate(Me.FieldValue), "yyyy/MM/dd")
                            Else
                                oRow("Value") = DBNull.Value
                            End If
                        Case FieldTypes.tDatePeriod
                            'TODO: Sólo se deberían aceptar cadenas de texto con formato fecha1*fecha2
                            If Me.FieldValue IsNot Nothing AndAlso Not IsDBNull(Me.FieldValue) Then
                                oRow("Value") = CStr(Me.FieldValue)
                            Else
                                oRow("Value") = DBNull.Value
                            End If
                        Case FieldTypes.tList
                            If Me.FieldValue IsNot Nothing AndAlso Not IsDBNull(Me.FieldValue) Then
                                oRow("Value") = CStr(Me.FieldValue)
                            Else
                                oRow("Value") = DBNull.Value
                            End If
                        Case FieldTypes.tTime
                            If Me.FieldValue IsNot Nothing AndAlso Not IsDBNull(Me.FieldValue) AndAlso Me.FieldValue <> "" Then ' AndAlso IsDate(Me.FieldValue) Then
                                Dim str As String = CStr(Me.FieldValue)
                                If str.Contains(":") Then
                                    oRow("Value") = CStr(roConversions.ConvertTimeToHours(CStr(Me.FieldValue))).Replace(roConversions.GetDecimalDigitFormat, ".")
                                Else
                                    If str.Contains(".") Then
                                        oRow("Value") = str
                                    Else
                                        oRow("Value") = Any2Double(str)
                                    End If
                                End If
                            Else
                                oRow("Value") = DBNull.Value
                            End If
                        Case FieldTypes.tTimePeriod
                            If Me.FieldValue IsNot Nothing AndAlso Not IsDBNull(Me.FieldValue) Then
                                oRow("Value") = CStr(Me.FieldValue)
                            Else
                                oRow("Value") = DBNull.Value
                            End If
                        Case FieldTypes.tLink
                            If Me.FieldValue IsNot Nothing AndAlso Not IsDBNull(Me.FieldValue) Then
                                oRow("Value") = CStr(Me.FieldValue)
                            Else
                                oRow("Value") = DBNull.Value
                            End If
                        Case FieldTypes.tDocument
                            If Me.FieldValue IsNot Nothing AndAlso Not IsDBNull(Me.FieldValue) Then
                                oRow("Value") = CLng(Me.FieldValue)
                            Else
                                oRow("Value") = DBNull.Value
                            End If

                    End Select
                Else
                    oRow("Value") = Me.FieldValue
                End If

                oAuditDataNew = oRow

                Dim bReadOnlyField As Boolean = False
                If Not bIsNew AndAlso Me.Definition.ReadOnly AndAlso oOldEmployeeUserField IsNot Nothing AndAlso Me.FieldRawValue <> oOldEmployeeUserField.FieldRawValue Then
                    bReadOnlyField = True
                    Me.oState.Result = UserFieldResultEnum.ReadOnlyField
                End If

                ' De ser un campo con valores únicos, verificamos que no exista otro campo con el mismo valor en otro empleado
                Dim uniqueConstraintBreached As Boolean = False
                If Me.Definition.Unique AndAlso Me.ValueAlreadyExistsOnOtherEmployee() Then
                    uniqueConstraintBreached = True
                    Me.oState.Result = UserFieldResultEnum.UniqueValueAlreadyExists
                    Me.oState.Language.ClearUserTokens()
                    Me.oState.Language.AddUserToken(Me.FieldName)
                    Me.oState.ErrorText = Me.oState.Language.Translate("UniqueValueAlreadyExists", "")
                End If

                ' Verifico si se puede realizar el cambio en función de la fecha de congelación (sólo para los campos que se usan en procesos)
                Dim bolCanSave As Boolean = True
                Dim bolChange As Boolean = True
                If Not uniqueConstraintBreached AndAlso Not bReadOnlyField Then
                    If oOldEmployeeUserField IsNot Nothing Then
                        bolChange = Not Me.IsEqual(oOldEmployeeUserField)
                    Else
                        bolChange = (Any2String(Me.FieldValue) <> "")
                    End If

                    If bCalculatedUsedInProcess Then
                        bCalculatedUsedInProcess = Me.UsedInProcess()
                    End If

                    If bCalculatedUsedInProcess AndAlso bolChange AndAlso Me.oDefinition.History Then
                        ' Obtengo la fecha de congelación
                        Dim xFreezingDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.intIDEmployee, False, Me.oState)

                        If xFreezingDate <> Nothing Then ' Si hay definida una fecha de congelación
                            ' Miro si la fecha del valor del campo está dentro de periodo de congelación
                            bolCanSave = (Me._Date > xFreezingDate)
                            If bolCanSave AndAlso oOldEmployeeUserField IsNot Nothing Then
                                bolCanSave = (oOldEmployeeUserField._Date > xFreezingDate)
                            End If
                            If Not bolCanSave Then
                                Me.oState.Result = UserFieldResultEnum.InPeriodOfFreezing
                            End If
                        End If
                    End If
                Else
                    bolCanSave = False
                End If

                If bolCanSave Then

                    da.Update(tb)

                    If bolChange Then VTBase.Extensions.roTimeStamps.UpdateEmployeeTimestamp(Me.IDEmployee)

                    bolRet = True

                    ' Si se ha modificado un campo de PRL, marcamos el estado del empleado para que se actualice su estado de PRL
                    If oDefinition.AccessValidation <> AccessValidation.None Then
                        Dim olog As New roLog("roDailyEmployeeUserField::Save")
                        Try
                            strSQL = "@UPDATE# EmployeeStatus SET PRLStatus = '?' WHERE IDEmployee = " & Me.IDEmployee.ToString
                            ' Será el proceso EOG quien indique el estado del empleado, y quien gestione la notificación si corresponde
                            If Not ExecuteSql(strSQL) Then
                                olog.logMessage(roLog.EventType.roDebug, "Error updating PRLStatus in EmployeeStatus table!")
                            End If
                        Catch ex As Exception
                            olog.logMessage(roLog.EventType.roError, "Error updating PRLStatus in EmployeeStatus table!", ex)
                        End Try
                    End If

                    If bolRecalculate AndAlso bolChange Then
                        bolRet = Me.Recalculate(oOldEmployeeUserField, bCalculatedUsedInProcess)
                    End If

                    If bolRet AndAlso bAudit AndAlso bolChange Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        'Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        'AUDITSAMPLE
                        Robotics.VTBase.Extensions.roAudit.AddParameter(tbAuditParameters, "{UserFieldValue}", Me.FieldValue, "", 1)

                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("FieldName") & " (" & oAuditDataNew("IDEmployee") & ")"
                        Else
                            strObjectName = oAuditDataOld("FieldName") & " (" & oAuditDataOld("IDEmployee") & ")" & " -> " & oAuditDataNew("FieldName") & " (" & oAuditDataNew("IDEmployee") & ")"
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tUserField, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roEmployeeUserField::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roEmployeeUserField::Save")
            End Try

            Return bolRet

        End Function

        Public Function ValueAlreadyExistsOnOtherEmployee() As Boolean
            Dim duplicatedValueFound As Boolean = False

            Try

                If Me.FieldRawValue Is Nothing OrElse (Me.Definition.FieldType = FieldTypes.tText AndAlso Me.FieldRawValue.Trim = String.Empty) OrElse (Me.Definition.FieldType = FieldTypes.tNumeric AndAlso roTypes.Any2Integer(Me.FieldRawValue) = 0) Then
                    'No considero valores repetidos ni la cadena vacía para textos, ni el 0 para numéricos
                    Return False
                End If

                Dim sql As String = $"@SELECT# IdEmployee 
                                         FROM EmployeeUserFieldValues WITH (NOLOCK)  
                                         WHERE IDEmployee <> {Me.IDEmployee} 
                                           AND FieldName = '{Me.FieldName}'
                                           AND LOWER(CONVERT(NVARCHAR(4000), ISNULL(Value, ''))) = '{Me.FieldRawValue.Trim.ToLower}'"

                Dim tbvalues As DataTable = CreateDataTable(sql)

                If tbvalues IsNot Nothing Then
                    Dim idEmployeeList As List(Of Integer) = tbvalues.AsEnumerable().Select(Function(row) roTypes.Any2Integer(row.Field(Of Object)("IdEmployee"))).ToList()
                    If idEmployeeList.FindAll(Function(x) x <> Me.IDEmployee).Any() Then
                        duplicatedValueFound = True
                    End If
                End If

            Catch ex As Exception
                ' Si algo fue mal, no permito guardar para prevenir duplicados en un campo único
                duplicatedValueFound = True
            End Try
            Return duplicatedValueFound
        End Function

        Public Function Recalculate(ByVal oOldEmployeeUserField As roEmployeeUserField, Optional ByVal bCalculatedUsedInProcess As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try

                ' Primero miramos si el campo se està utilizando en algún proceso que requiera recálculo en el servidor
                If bCalculatedUsedInProcess Then
                    bCalculatedUsedInProcess = Me.UsedInProcess()
                End If
                If bCalculatedUsedInProcess Then
                    Dim oStartupValuesList As New Generic.List(Of Integer)
                    Dim oCauseLimitValuesList As New Generic.List(Of Integer)
                    Dim oLabAgreeRulesList As New Generic.List(Of Integer)
                    Dim oConceptsList As New Generic.List(Of Integer)
                    Dim oShiftsList As New Generic.List(Of Integer)

                    Dim bolMustRecalc As Boolean = True
                    Dim xRecalcDate As Date = Me._Date
                    If oOldEmployeeUserField IsNot Nothing Then
                        bolMustRecalc = (oOldEmployeeUserField._Date <> Me._Date Or
                                         Any2String(oOldEmployeeUserField.FieldValue) <> Any2String(Me.FieldValue))
                        ' Buscamos la fecha más antigua para recalcular
                        If oOldEmployeeUserField._Date < Me._Date Then xRecalcDate = oOldEmployeeUserField._Date
                    End If

                    If bolMustRecalc Then

                        ' Especial UPF, no utilizamos customization para no tener que sobrecargar la carga de la clase
                        ' Unicamente utilizamos un tag en la descripcion del campo
                        If Me.Definition.Description.ToUpper.Contains("UPF_ANT") Then
                            ' En el caso que el campo de la ficha sea el de años de antigüedad, y se haya modificado un valor que implique recalculo
                            ' Debemos recalcular siempre desde el 01/01 para volver a calcular el valor inicial de los saldos con antigüedad
                            Dim _StartupDate As Date = New Date(xRecalcDate.Year, 1, 1)
                            If xRecalcDate > _StartupDate Then
                                xRecalcDate = _StartupDate
                            End If
                        End If

                        ' Miramos si el campo se està utilizando en algún valor inicial del convenio asignado al empleado
                        Dim tbStartValues As DataTable = roUserField.GetUserFieldUsedInStartupValues(Me.FieldName, Me.State, Me.IDEmployee)
                        If tbStartValues IsNot Nothing Then
                            For Each oRow As DataRow In tbStartValues.Rows
                                oStartupValuesList.Add(roTypes.Any2Integer(oRow("IDStartupValue")))
                            Next
                        End If
                        If bolRet Then
                            Dim tbCauseLimitValues As DataTable = roUserField.GetUserFieldUsedInCauseLimitValues(Me.FieldName, Me.State, Me.IDEmployee)
                            If tbCauseLimitValues IsNot Nothing Then
                                'TODO ISM Lanzamos el recálculo para cada limite de justificacion relacionado
                                For Each oRow As DataRow In tbCauseLimitValues.Rows
                                    oCauseLimitValuesList.Add(roTypes.Any2Integer(oRow("IDCauseLimitValue")))
                                Next
                            End If
                        End If

                        If bolRet Then
                            ' Miramos si el campo se està utilizando en alguna regla de convenio asignada al empleado
                            Dim tbLabAgreeRules As DataTable = roUserField.GetUserFieldUsedInLabAgreeRules(Me.FieldName, Me.State, Me.IDEmployee)
                            If tbLabAgreeRules IsNot Nothing Then
                                'TODO ISM  Lanzamos el recálculo para cada regla de convenio relacionada.
                                For Each oRow As DataRow In tbLabAgreeRules.Rows
                                    oLabAgreeRulesList.Add(roTypes.Any2Integer(oRow("IdAccrualsRule")))
                                Next
                            End If
                        End If

                        If bolRet Then
                            ' Miramos si el campo se està utilizando en algún saldo
                            Dim tbConcepts As DataTable = roUserField.GetUserFieldUsedInConcepts(Me.FieldName, Me.State)
                            If tbConcepts IsNot Nothing Then
                                'TODO ISM  Lanzamos el recálculo para cada saldo relacionado.
                                For Each oRow As DataRow In tbConcepts.Rows
                                    oConceptsList.Add(roTypes.Any2Integer(oRow("ID")))
                                Next
                            End If
                        End If

                        If bolRet Then
                            ' Miramos si el campo se està utilizando en algún horario
                            Dim tbShifts As DataTable = roUserField.GetUserFieldUsedInShifts(Me.FieldName, Me.State)
                            If tbShifts IsNot Nothing Then
                                'TODO ISM Lanzamos el recálculo para cada horario relacionado.
                                For Each oRow As DataRow In tbShifts.Rows
                                    oShiftsList.Add(roTypes.Any2Integer(oRow("ID")))
                                Next
                            End If
                        End If

                        Dim oParameters As New roCollection

                        oParameters.Add("IDEmployee", IDEmployee)
                        oParameters.Add("RecalcDate", xRecalcDate.ToString("yyyy/MM/dd HH:mm"))

                        oParameters.Add("StartupValueIDs", String.Join(",", oStartupValuesList))
                        oParameters.Add("CauseLimitValueIDs", String.Join(",", oCauseLimitValuesList))
                        oParameters.Add("LabAgreeRuleIDs", String.Join(",", oLabAgreeRulesList))
                        oParameters.Add("ConceptIDs", String.Join(",", oConceptsList))
                        oParameters.Add("ShiftIDs", String.Join(",", oShiftsList))

                        Return roLiveTask.CreateLiveTask(roLiveTaskTypes.ConsolidateData, oParameters, New roLiveTaskState(Me.State.IDPassport))

                    End If
                End If

                If bolRet Then

                    Dim oServerLicense As New roServerLicense
                    ' Si hay licencia para prevención de riesgos laborales
                    If oServerLicense.FeatureIsInstalled("Feature\OHP") Then

                        ' Si está configurado con validación de acceso
                        If Me.oDefinition IsNot Nothing AndAlso Me.oDefinition.AccessValidation <> AccessValidation.None Then
                            ' Notificamos al servidor que tiene que lanzar el broadcaster
                            ' ** Queda pendiente informar los IDs de los terminales a regenerar. Actualmente regenera los ficheros para todos los terminales tipo mx6
                            roConnector.InitTask(TasksType.BROADCASTER)
                        End If

                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roEmployeeUserField::Recalculate")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roEmployeeUserField::Recalculate")
                bolRet = False
            Finally
                ''If bolCloseTrans AndAlso oTrans IsNot Nothing Then
                ''    If bolRet Then
                ''        oTrans.Commit()
                ''    Else
                ''        oTrans.Rollback()
                ''    End If
                ''End If

            End Try

            Return bolRet

        End Function

        Public Function GetDefinition(Optional ByVal oUserFieldState As roUserFieldState = Nothing) As roUserField

            If oUserFieldState Is Nothing Then oUserFieldState = New roUserFieldState(Me.oState.IDPassport)

            Return New roUserField(oUserFieldState, Me.FieldName, Types.EmployeeField, False)

        End Function

        Public Function Delete(Optional ByVal bolCheckCanSave As Boolean = True, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos la definición del campo de la ficha
                If Me.oDefinition Is Nothing Then Me.oDefinition = Me.GetDefinition()

                ' Verifico si se puede realizar el cambio en función de la fecha de congelación (sólo para los campos que se usan en procesos y si el parámetro bolCheckCanSave está a True )
                Dim bolCanSave As Boolean = True
                If bolCheckCanSave Then

                    If Me.UsedInProcess() Then

                        ' Obtengo la fecha de congelación
                        Dim xFreezingDate As Date = Nothing
                        'Dim oParameters As New roParameters("OPTIONS", True)
                        'If oParameters.Parameter(roParameters.Parameters.FirstDate) IsNot Nothing AndAlso IsDate(oParameters.Parameter(roParameters.Parameters.FirstDate)) Then
                        '    xFreezingDate = CDate(oParameters.Parameter(roParameters.Parameters.FirstDate))
                        'End If
                        xFreezingDate = roBusinessSupport.GetEmployeeLockDatetoApply(Me.intIDEmployee, False, Me.oState)

                        If xFreezingDate <> Nothing Then ' Si hay definida una fecha de congelación
                            ' Miro si la fecha del valor del campo está dentro de periodo de congelación
                            bolCanSave = (Me._Date > xFreezingDate)
                            If Not bolCanSave Then
                                Me.oState.Result = UserFieldResultEnum.InPeriodOfFreezing
                            End If
                        End If

                    End If

                End If

                If bolCanSave Then

                    Dim cmd As DbCommand = Nothing
                    Dim strSQL As String

                    strSQL = "@DELETE# FROM EmployeeUserFieldValues " &
                             "WHERE IDEmployee = @IDEmployee AND " &
                                   "FieldName = @FieldName AND " &
                                   "Date = @Date"
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@IDEmployee", DbType.Int64)
                    AddParameter(cmd, "@FieldName", DbType.String, 50)
                    AddParameter(cmd, "@Date", DbType.Date)
                    cmd.Parameters("@IDEmployee").Value = Me.intIDEmployee
                    cmd.Parameters("@FieldName").Value = Me.strFieldName
                    cmd.Parameters("@Date").Value = Me.xDate
                    cmd.ExecuteNonQuery()

                    bolRet = True

                    If bolRet Then

                        ' Notificar recalculo en el servidor
                        bolRet = Me.Recalculate(Nothing)

                    End If

                    If bolRet And bAudit Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tUserField, Me.strFieldName & " (" & Me.IDEmployee & ")", Nothing, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roEmployeeUserField::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roEmployeeUserField::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function UsedInProcess() As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Miramos si el campo se està utilizando en algún valor inicial de algún convenio asignado al empleado
                Dim dTblStartValues As DataTable = roUserField.GetUserFieldUsedInStartupValues(Me.FieldName, Me.State, Me.IDEmployee)
                If dTblStartValues.Rows.Count > 0 Then
                    bolRet = True
                End If

                If Not bolRet Then
                    ' Miramos si el campo se està utilizando en algun limite de justificacion
                    Dim dTblCauseLimitValues As DataTable = roUserField.GetUserFieldUsedInCauseLimitValues(Me.FieldName, Me.State, Me.IDEmployee)
                    If dTblCauseLimitValues.Rows.Count > 0 Then
                        bolRet = True
                    End If

                End If

                If Not bolRet Then
                    ' Miramos si el campo se està utilizando en alguna regla de convenio asignada al empleado
                    Dim dTblLabAgreeRules As DataTable = roUserField.GetUserFieldUsedInLabAgreeRules(Me.FieldName, Me.State, Me.IDEmployee)
                    If dTblLabAgreeRules.Rows.Count > 0 Then
                        bolRet = True
                    End If

                End If

                If Not bolRet Then
                    ' Miramos si el campo se està utilizando en algún saldo
                    Dim tbConcepts As DataTable = roUserField.GetUserFieldUsedInConcepts(Me.FieldName, Me.State)
                    If tbConcepts IsNot Nothing Then
                        bolRet = (tbConcepts.Rows.Count > 0)
                    End If

                End If

                If Not bolRet Then
                    ' Miramos si el campo se està utilizando en algún horario
                    Dim tbShifts As DataTable = roUserField.GetUserFieldUsedInShifts(Me.FieldName, Me.State)
                    If tbShifts IsNot Nothing Then
                        bolRet = (tbShifts.Rows.Count > 0)
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roEmployeeUserField::UsedInProcess")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roEmployeeUserField::UsedInProcess")
            Finally

            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        ''' <summary>
        ''' Obtiene la lista de valores de la ficha del empleado a una fecha en concreto.
        ''' </summary>
        ''' <param name="IDEmployee">Código del empleado</param>
        ''' <param name="xDate">Fecha de validez de los valores devueltos</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetUserFieldsList(ByVal IDEmployee As Integer, ByVal xDate As Date, ByVal _State As roUserFieldState) As Generic.List(Of roEmployeeUserField)
            ' Devuelvo todos los userfields de un empleado en una clase wscUserFields
            Dim oRet As New Generic.List(Of roEmployeeUserField)

            _State.UpdateStateInfo()

            Try

                Dim oUserFieldState As New roUserFieldState(_State.IDPassport, _State.Context, _State.ClientAddress)
                roBusinessState.CopyTo(_State, oUserFieldState)

                Dim strSQL As String

                ' Primero recupero los campos personalizados que se estan usando
                ' Y los meto en StrSelectFields que usare mas tarde para componer la select
                ' Formato de StrSelectFields: "Usr_codigopostal, Usr_Direccion, Usr_Telefono"
                Dim strSelectFields As String = ""
                Dim oSelectedFields As Generic.List(Of String) = roUserField.GetUserFieldsListWithType(Types.EmployeeField, IDEmployee, oUserFieldState, "Used=1")
                If oSelectedFields IsNot Nothing Then
                    For Each strFieldName As String In oSelectedFields
                        strSelectFields &= ",'" & strFieldName.Replace("'", "''") & "'"
                    Next
                    If strSelectFields <> "" Then strSelectFields = strSelectFields.Substring(1)
                End If

                If strSelectFields <> "" Then

                    strSQL = "@DECLARE# @Date smalldatetime SET @Date = " & Any2Time(xDate).SQLSmallDateTime & " " &
                             "@SELECT# FieldName, Value, Date " &
                             "FROM GetEmployeeAllUserFieldValue(" & IDEmployee.ToString & ", @Date) " &
                             "WHERE FieldName IN (" & strSelectFields & ") " &
                             "ORDER BY FieldName"
                    Dim tbFieldValues As DataTable = CreateDataTable(strSQL, )

                    If tbFieldValues IsNot Nothing Then

                        Dim oEmployeeUserField As roEmployeeUserField

                        For Each oRow As DataRow In tbFieldValues.Rows

                            oEmployeeUserField = New roEmployeeUserField(IDEmployee, oRow, _State)
                            If oEmployeeUserField.oDefinition Is Nothing Then oEmployeeUserField.oDefinition = oEmployeeUserField.GetDefinition(oUserFieldState)

                            oRet.Add(oEmployeeUserField)

                        Next

                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetUserFieldsList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetUserFieldsList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetUserFieldsDataTable(ByVal IDEmployee As Integer, ByVal xDate As Date, ByVal _State As roUserFieldState) As DataTable
            ' Devuelvo todos los userfields de un empleado en un Dataset
            Dim oRet As DataTable = Nothing

            _State.UpdateStateInfo()

            ' Verificamos si el passport asociado es de tipo empleado. Si se trata del mismo empleado del que se están consultando los valores de la ficha, se tendrá en cuenta la configuración de permisos del campo de la ficha (Visibilidad).
            Dim intIDPassportEmployee As Integer = -1
            If _State.ActivePassportType(intIDPassportEmployee) <> "E" Then
                intIDPassportEmployee = -1
            End If

            Dim dtOHP As DataTable = Nothing

            Dim strSQL As String = "@DECLARE# @ara smalldatetime " &
                                    "set @ara = GETDATE() " &
                                    "@EXEC# EmployeeDocuments " & IDEmployee & ",@ara"

            dtOHP = CreateDataTable(strSQL)

            Dim oEmployeeUserFields As Generic.List(Of roEmployeeUserField) = roEmployeeUserField.GetUserFieldsList(IDEmployee, xDate, _State)
            If _State.Result = UserFieldResultEnum.NoError Then

                Try

                    oRet = New DataTable

                    Dim oDataColumn As DataColumn

                    oDataColumn = New DataColumn
                    oDataColumn.ColumnName = "FieldCaption"
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn
                    oDataColumn.ColumnName = "FieldName"
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn
                    oDataColumn.ColumnName = "Type"
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn("Value", GetType(String))
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn("Date", GetType(DateTime))
                    oRet.Columns.Add(oDataColumn)
                    oDataColumn = New DataColumn("OriginalDate", GetType(DateTime))
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn("ValueDateTime", GetType(DateTime))
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn
                    oDataColumn.ColumnName = "Category"
                    oRet.Columns.Add(oDataColumn)

                    oRet.Columns.Add(New DataColumn("AccessLevel", GetType(Integer)))

                    oDataColumn = New DataColumn("Description", GetType(String))
                    oRet.Columns.Add(oDataColumn)

                    oRet.Columns.Add(New DataColumn("AccessValidation", GetType(Integer)))

                    oRet.Columns.Add(New DataColumn("History", GetType(Boolean)))

                    oRet.Columns.Add(New DataColumn("IsExpired", GetType(Boolean)))

                    oRet.Columns.Add(New DataColumn("ReadOnly", GetType(Boolean)))

                    If oEmployeeUserFields IsNot Nothing Then

                        Dim oNewRow As DataRow = Nothing

                        For Each oEmployeeUserField As roEmployeeUserField In oEmployeeUserFields

                            If intIDPassportEmployee <> IDEmployee OrElse (oEmployeeUserField.Definition IsNot Nothing AndAlso oEmployeeUserField.Definition.RequestVisible(IDEmployee)) Then

                                oNewRow = oRet.NewRow

                                oNewRow("FieldCaption") = oEmployeeUserField.FieldName ' oUserField.FieldCaption
                                oNewRow("FieldName") = oEmployeeUserField.FieldName
                                oNewRow("Type") = Val(oEmployeeUserField.Definition.FieldType)
                                oNewRow("Value") = oEmployeeUserField.FieldValue
                                oNewRow("Date") = oEmployeeUserField._Date
                                oNewRow("OriginalDate") = oEmployeeUserField.OriginalDate
                                If oEmployeeUserField.Definition IsNot Nothing Then
                                    If oEmployeeUserField.Definition.FieldType = FieldTypes.tDate Then 'Or oEmployeeUserField.Definition.FieldType = FieldTypes.tTime Then
                                        If Not IsDBNull(oNewRow("Value")) AndAlso oNewRow("Value") <> "" Then
                                            oNewRow("ValueDateTime") = oEmployeeUserField.FieldValue
                                        End If
                                    End If
                                    oNewRow("Category") = oEmployeeUserField.Definition.Category
                                    oNewRow("AccessLevel") = oEmployeeUserField.Definition.AccessLevel
                                    oNewRow("Description") = oEmployeeUserField.Definition.Description
                                    oNewRow("AccessValidation") = oEmployeeUserField.Definition.AccessValidation
                                    oNewRow("History") = oEmployeeUserField.Definition.History
                                    oNewRow("ReadOnly") = oEmployeeUserField.Definition.ReadOnly
                                End If

                                Dim ohpRows() As DataRow = dtOHP.Select("Name = '" & oEmployeeUserField.FieldName & "'")

                                If oNewRow("AccessValidation") IsNot DBNull.Value AndAlso roTypes.Any2Integer(oNewRow("AccessValidation")) > 0 Then
                                    If ohpRows IsNot Nothing AndAlso ohpRows.Count > 0 Then
                                        oNewRow("IsExpired") = ohpRows(0)("Expired")
                                    Else
                                        oNewRow("IsExpired") = True
                                    End If
                                End If

                                oRet.Rows.Add(oNewRow)
                            End If

                        Next

                    End If
                Catch ex As DbException
                    _State.UpdateStateInfo(ex, "roEmployeeUserField::GetUserFieldsDataTable")
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roEmployeeUserField::GetUserFieldsDataTable")
                End Try

            End If

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene la lista de valores del histórico del campo de la ficha del empleado.
        ''' </summary>
        ''' <param name="IDEmployee">Código del empleado</param>
        ''' <param name="_FieldName">Nombre del campo de la ficha del empleado</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetUserFieldHistoryDataTable(ByVal IDEmployee As Integer, ByVal _FieldName As String, ByVal _State As roUserFieldState) As DataTable

            Dim tbRet As DataTable = Nothing

            _State.UpdateStateInfo()

            Try

                Dim oUserFieldState As New roUserFieldState : roBusinessState.CopyTo(_State, oUserFieldState)

                ' Obtenemos los permisos de usuario para el acceso a la información de la ficha para el passpor actual en función del nivel de acceso del campo de la ficha.
                Dim bolLowAccess As Boolean = True
                Dim bolMediumAccess As Boolean = True
                Dim bolHighAccess As Boolean = True
                Select Case _State.ActivePassportType()
                    Case "U"
                        bolLowAccess = WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Employees.UserFields.Information.Low", Permission.Read, IDEmployee, )
                        bolMediumAccess = WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Employees.UserFields.Information.Medium", Permission.Read, IDEmployee, )
                        bolHighAccess = WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Employees.UserFields.Information.High", Permission.Read, IDEmployee, )
                    Case "E"
                        bolLowAccess = WLHelper.HasFeaturePermission(_State.IDPassport, "UserFields.Query", Permission.Read, "E")
                        bolMediumAccess = bolLowAccess
                        bolHighAccess = bolLowAccess
                End Select

                ' Primero recupero la definición del campo de la ficha y verifico que el passport actual tiene permisos de lectura sobre el tipo de campo.
                Dim oUserField As New roUserField(oUserFieldState, _FieldName, Types.EmployeeField, False)

                roBusinessState.CopyTo(oUserFieldState, _State)

                If (oUserField.AccessLevel = AccessLevels.aLow And bolLowAccess) Or
                   (oUserField.AccessLevel = AccessLevels.aMedium And bolMediumAccess) Or
                   (oUserField.AccessLevel = AccessLevels.aHigh And bolHighAccess) Then

                    Dim strSQL As String

                    tbRet = New DataTable

                    strSQL = "@SELECT# ROW_NUMBER() OVER (ORDER BY Date) AS ID, *, Date AS OriginalDate FROM EmployeeUserFieldValues " &
                             "WHERE IDEmployee = @IDEmployee AND " &
                                   "FieldName = @FieldName " &
                             "ORDER BY Date"

                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    AddParameter(cmd, "@IDEmployee", DbType.Int64)
                    AddParameter(cmd, "@FieldName", DbType.String, 50)
                    cmd.Parameters("@IDEmployee").Value = IDEmployee
                    cmd.Parameters("@FieldName").Value = _FieldName

                    Dim ad As DbDataAdapter = CreateDataAdapter(cmd, False)

                    ad.Fill(tbRet)
                Else
                    _State.Result = UserFieldResultEnum.AccessDenied
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetUserFieldHistoryDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetUserFieldHistoryDataTable")
            Finally

            End Try

            Return tbRet

        End Function

        Public Shared Function SaveUserFieldHistory(ByVal _IDEmployee As Integer, ByVal tbHistory As DataTable, ByVal _State As roUserFieldState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oEmployeeUserField As roEmployeeUserField
                Dim bolSaved As Boolean = True

                If tbHistory.Rows.Count = 0 Then
                    bolRet = True
                End If

                For Each oRow As DataRow In tbHistory.Rows

                    bolSaved = True

                    Select Case oRow.RowState
                        Case DataRowState.Added
                            Dim xDate As Date = oRow("OriginalDate")
                            oEmployeeUserField = New roEmployeeUserField(_IDEmployee, oRow("FieldName"), xDate, _State)
                            With oEmployeeUserField
                                ._Date = xDate
                                If Not IsDBNull(oRow("Value")) Then
                                    .FieldValue = oRow("Value")
                                Else
                                    .FieldValue = DBNull.Value
                                End If
                            End With
                            bolRet = oEmployeeUserField.Save(, True)
                        Case DataRowState.Modified
                            oEmployeeUserField = New roEmployeeUserField(_IDEmployee, oRow("FieldName"), oRow("Date"), _State)
                            With oEmployeeUserField
                                ._Date = oRow("OriginalDate")
                                If Not IsDBNull(oRow("Value")) Then
                                    .FieldValue = oRow("Value")
                                Else
                                    .FieldValue = DBNull.Value
                                End If
                            End With
                            bolRet = oEmployeeUserField.Save(, True)

                        Case DataRowState.Deleted
                            oRow.RejectChanges() ' Cmabiar el estado de la fila para poder leer sus datos
                            oEmployeeUserField = New roEmployeeUserField(_IDEmployee, oRow("FieldName"), oRow("OriginalDate"), _State)
                            bolRet = oEmployeeUserField.Delete(, True)

                        Case Else
                            bolRet = True
                            bolSaved = False

                    End Select

                    If Not bolRet Then

                        Exit For
                    End If

                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::SaveUserFieldHistory")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::SaveUserFieldHistory")
            Finally
                ''If bolCloseTrans Then
                ''    If bolRet Then : oTrans.Commit()
                ''    Else : oTrans.Rollback()
                ''    End If
                ''End If

            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteUserFieldHistory(ByVal _IDEmployee As Integer, ByVal _FieldName As String, ByVal _FromDate As Date, ByVal bolCheckCanDelete As Boolean, ByVal _State As roUserFieldState) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False
            Try

                Dim oEmployeeUserField As roEmployeeUserField

                Dim tbHistory As DataTable = roEmployeeUserField.GetUserFieldHistoryDataTable(_IDEmployee, _FieldName, _State)
                If tbHistory IsNot Nothing Then

                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    bolRet = True

                    For Each oRow As DataRow In tbHistory.Rows
                        If CDate(oRow("Date")) >= _FromDate.Date Then
                            oEmployeeUserField = New roEmployeeUserField(oRow("IDEmployee"), oRow("FieldName"), oRow("Date"), _State)
                            bolRet = oEmployeeUserField.Delete(bolCheckCanDelete, True)
                            If Not bolRet Then Exit For
                        End If
                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::DeleteUserFieldHistory")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::DeleteUserFieldHistory")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina el histórico de valores de un campo de la ficha para todos los empleados.
        ''' </summary>
        ''' <param name="_FieldName">Campo de la ficha a eliminar</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteUserFieldHistoryAllEmployees(ByVal _FieldName As String, ByVal _State As roUserFieldState) As Boolean

            Dim bolRet As Boolean = False
            Try

                Dim strSQL As String = "@DELETE# FROM EmployeeUserFieldValues WHERE FieldName = @FieldName"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                AddParameter(cmd, "@FieldName", DbType.String, 50)
                cmd.Parameters("@FieldName").Value = _FieldName
                cmd.ExecuteNonQuery()

                bolRet = True

                ' Auditamos
                ' ...
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::DeleteUserFieldHistoryAllEmployees")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::DeleteUserFieldHistoryAllEmployees")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Modifica el nombre del campo de la ficha del  Elimina el histórico de valores de un campo de la ficha para todos los empleados.
        ''' </summary>
        ''' <param name="_FieldNameOld">Nombre del campo de la ficha anterior</param>
        ''' <param name="_FieldNameNew">Nuevo nombre del campo de la ficha</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RenameUserFieldHistoryAllEmployees(ByVal _FieldNameOld As String, ByVal _FieldNameNew As String, ByVal _State As roUserFieldState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@UPDATE# EmployeeUserFieldValues " &
                                       "SET FieldName = @FieldNameNew " &
                                       "WHERE FieldName = @FieldNameOld"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                AddParameter(cmd, "@FieldNameNew", DbType.String, 50)
                AddParameter(cmd, "@FieldNameOld", DbType.String, 50)
                cmd.Parameters("@FieldNameNew").Value = _FieldNameNew
                cmd.Parameters("@FieldNameOld").Value = _FieldNameOld
                cmd.ExecuteNonQuery()

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::RenameUserFieldHistoryAllEmployees")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::RenameUserFieldHistoryAllEmployees")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveUserFields(ByVal _IDEmployee As Integer, ByVal _UserFields As Generic.List(Of roEmployeeUserField), ByRef _State As roUserFieldState,
                                              Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If _UserFields.Count > 0 Then

                    For Each oEmployeeUserField As roEmployeeUserField In _UserFields

                        If oEmployeeUserField.Definition Is Nothing Then
                            oEmployeeUserField.Definition = oEmployeeUserField.GetDefinition()
                        End If

                        bolRet = oEmployeeUserField.Save(, bAudit)

                        If Not bolRet Then Exit For

                    Next
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::SaveUserFields")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::SaveUserFields")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve los distintos valores del campo de la ficha de empleado por una fecha.
        ''' </summary>
        ''' <param name="strUserField">Nombre del campo de la ficha</param>
        ''' <param name="xDate">Fecha validez de los valores</param>
        ''' <param name="strParent">Para identificar la ruta de los valores en formato árbol.</param>
        ''' <param name="strSeparator">Separador utilizado para generar el árbol de valores</param>
        ''' <param name="strFieldWhere">Filtro sobre el empleado por campos de la ficha. Opcional. </param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetUserFieldValues(ByVal strUserField As String, ByVal xDate As Date, ByVal strParent As String, ByVal strSeparator As String, ByVal strFieldWhere As String, ByVal _State As roUserFieldState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                'IMPORTANTE:El parametro strFieldWhere se debe procesar adecuadamente. Según la forma Expresion1 + "#@#" +  Expresion2
                If strFieldWhere <> String.Empty Then
                    If strFieldWhere.IndexOf("#@#") > 0 Then
                        Dim strSplit As String() = {"#@#"}
                        strFieldWhere = strFieldWhere.Split(strSplit, StringSplitOptions.None)(1)
                    End If
                End If

                Dim strUserFieldName As String = strUserField
                If strUserFieldName.ToUpper.StartsWith("USR_") Then strUserFieldName = strUserFieldName.Substring(4)
                strUserFieldName = strUserFieldName.Replace("'", "''")
                strParent = strParent.Replace("'", "''")
                Dim strSQL As String
                strSQL = "@DECLARE# @Date smalldatetime SET @Date = " & Any2Time(xDate).SQLSmallDateTime & " "
                strSQL &= "@SELECT# DISTINCT "
                If strParent = "" Then
                    strSQL &= "CONVERT(varchar, FieldValues.[Value]) AS 'Name', CONVERT(varchar, FieldValues.[Value]) AS 'Path' "
                Else
                    strSQL &= "SUBSTRING(CONVERT(varchar, FieldValues.[Value]), LEN('" & strParent & strSeparator & "')+1, LEN(CONVERT(varchar, FieldValues.[Value])) - LEN('" & strParent & strSeparator & "')) AS [Name], CONVERT(varchar, FieldValues.[Value]) AS Path "
                End If
                strSQL &= "FROM GetAllEmployeeUserFieldValue('" & strUserFieldName & "', @Date) FieldValues "
                If strFieldWhere <> "" Then
                    strSQL &= "INNER JOIN Employees ON FieldValues.IDEmployee = Employees.ID "
                End If
                If strParent = "" Then
                    ''    ' Si estamos obteniendo los valores de la raiz:
                    ''    ' Obtenemos los que no tienen el carácter de separación ('\') y
                    ''    ' los que si que lo tienen, pero no hay ningún otro valor con la parte izquierda del separador
                    ''    ' (p.ej. si hay un valor 'C\Mayor', miramos si existe un valor 'C', y si no lo hay devolvemos el valor 'C\Mayor' cómo raiz)
                    strSQL &= "WHERE (CHARINDEX('" & strSeparator & "', CONVERT(varchar, FieldValues.[Value])) = 0 OR " &
                                     "(CHARINDEX('" & strSeparator & "', CONVERT(varchar, FieldValues.[Value])) > 0 AND " &
                                      "(@SELECT# COUNT(*) FROM GetAllEmployeeUserFieldValue('" & strUserFieldName & "', @Date) FieldValues2 " &
                                       "WHERE CONVERT(varchar, FieldValues2.[Value]) = SUBSTRING(CONVERT(varchar, FieldValues.[Value]), 0, CHARINDEX('" & strSeparator & "', CONVERT(varchar, FieldValues.[Value])))) = 0)) AND " &
                                     "CONVERT(varchar, FieldValues.[Value]) <> '' "
                Else
                    strSQL &= "WHERE CONVERT(varchar, FieldValues.[Value]) LIKE '" & strParent & strSeparator & "%' AND " &
                                    "CHARINDEX('" & strSeparator & "', SUBSTRING(CONVERT(varchar, FieldValues.[Value]), LEN('" & strParent & strSeparator & "')+1, LEN(CONVERT(varchar, FieldValues.[Value])) - LEN('" & strParent & strSeparator & "'))) = 0 "
                End If

                If strFieldWhere <> "" Then
                    strSQL &= " AND " & strFieldWhere & " "
                End If
                strSQL &= "ORDER BY CONVERT(varchar, FieldValues.[Value])"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetUserFieldValues")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetUserFieldValues")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetIDEmployeesFromUserFieldIdAndValue(ByVal idUserField As Integer, ByVal strUserFieldValue As String, ByVal xDate As Date, ByVal _State As roUserFieldState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strUserFieldName As String = String.Empty

                Dim strSQL As String
                strSQL = $"@SELECT# FieldName FROM sysroUserFields WHERE Id = {idUserField}"
                strUserFieldName = roTypes.Any2String(AccessHelper.ExecuteScalar(strSQL, ))

                oRet = GetIDEmployeesFromUserFieldValue(strUserFieldName, strUserFieldValue, xDate, _State)

            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetIDEmployeesFromUserFieldIdAndValue")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetIDEmployeesFromUserFieldIdAndValue")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene los empleados que tengan el valor indicado en el campo de la ficha por una fecha.
        ''' </summary>
        ''' <param name="strUserField">Nombre del campo de la ficha de empleado</param>
        ''' <param name="strUserFieldValue">Valor del campo a buscar</param>
        ''' <param name="xDate">Fecha de validez del valor</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetIDEmployeesFromUserFieldValue(ByVal strUserField As String, ByVal strUserFieldValue As String, ByVal xDate As Date, ByVal _State As roUserFieldState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strUserFieldName As String = strUserField
                If strUserFieldName.ToUpper.StartsWith("USR_") Then strUserFieldName = strUserFieldName.Substring(4)
                strUserFieldName = strUserFieldName.Replace("'", "''")
                strUserFieldValue = strUserFieldValue.Replace("'", "''")

                Dim strSQL As String
                strSQL = "@DECLARE# @Date smalldatetime SET @Date = " & Any2Time(xDate).SQLSmallDateTime & " "
                strSQL &= " @SELECT# idemployee from dbo.GetAllEmployeeUserFieldValue('" & strUserFieldName & "', @Date) "
                strSQL &= " WHERE CONVERT(NVARCHAR(4000), ISNULL(Value, '')) = '" & strUserFieldValue & "' "

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetEmployeesFromUserField")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetEmployeesFromUserField")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene los empleados que tengan el valor indicado en el campo de la ficha por una fecha.
        ''' </summary>
        ''' <param name="strUserField">Nombre del campo de la ficha de empleado</param>
        ''' <param name="strUserFieldValue">Valor del campo a buscar</param>
        ''' <param name="xDate">Fecha de validez del valor</param>
        ''' <param name="strFieldWhere">Filtro sobre los empleados per campos de la ficha. Opcional.</param>
        ''' <param name="Feature">Nombre de funcionalidad de seguridad para filtrar los empleados devueltos. Opcional.</param>
        ''' <param name="Type">Tipo de funcionalidad de seguridad.</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesFromUserFieldWithType(ByVal strUserField As String, ByVal strUserFieldValue As String, ByVal xDate As Date, ByVal strFieldWhere As String, ByVal Feature As String, ByVal Type As String, ByVal _State As roUserFieldState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strUserFieldName As String = strUserField
                If strUserFieldName.ToUpper.StartsWith("USR_") Then strUserFieldName = strUserFieldName.Substring(4)
                strUserFieldName = strUserFieldName.Replace("'", "''")
                strUserFieldValue = strUserFieldValue.Replace("'", "''")

                Dim strSQL As String
                Dim strDeclareSQL = "@DECLARE# @Date smalldatetime SET @Date = " & Any2Time(xDate).SQLSmallDateTime & " "
                strSQL = "@SELECT# sysrovwAllEmployeeGroups.* FROM sysrovwAllEmployeeGroups, GetAllEmployeeUserFieldValue('" & strUserFieldName & "', @Date) V "
                If strFieldWhere <> "" Then
                    strSQL &= ", Employees "
                End If
                strSQL &= "WHERE sysrovwAllEmployeeGroups.IDEmployee = V.idEmployee AND " &
                                "CONVERT(varchar, ISNULL(V.[Value], '')) = '" & strUserFieldValue & "' "

                'IMPORTANTE:El parametro strFieldWhere se debe procesar adecuadamente. Según la forma Expresion1 + "#@#" +  Expresion2
                If strFieldWhere <> "" Then
                    If strFieldWhere.IndexOf("#@#") > 0 Then
                        Dim strSplit As String() = {"#@#"}
                        strFieldWhere = strFieldWhere.Split(strSplit, StringSplitOptions.None)(1) 'Contiene el filtro real
                    End If
                    strSQL &= " AND Employees.ID = sysrovwAllEmployeeGroups.IDEmployee AND " & strFieldWhere
                End If

                If Feature <> String.Empty Then

                    strSQL = strDeclareSQL & " @SELECT# tmp.* FROM (" & strSQL & ") tmp " & roSelector.GetEmployeePermissonInnerJoin(_State.IDPassport, Permission.Read, Feature, Type)
                    'strSQL &= " AND" & WLHelper.GetEmployeePermissonWhereClause(_State.IDPassport, Permission.Read, Feature, Type, "", "sysrovwAllEmployeeGroups.IDEmployee", roTypes.Any2Time(xDate).SQLSmallDateTime())
                Else
                    strSQL = strDeclareSQL & strSQL
                End If

                oRet = CreateDataTable(strSQL, )

                ' Configuro la tabla de resultado
                Dim oDataColumn As New DataColumn
                oDataColumn.ColumnName = "Type"
                oRet.Columns.Add(oDataColumn)

                For Each oDataRow As DataRow In oRet.Rows

                    If oDataRow("CurrentEmployee") = 0 And oDataRow("Begindate") >= Now Then
                        ' El empleado es una futura incorporación
                        oDataRow("Type") = 4
                    Else
                        If oDataRow("CurrentEmployee") = 0 And oDataRow("Begindate") < Now Then
                            ' El empleado es una baja
                            oDataRow("Type") = 3
                        Else
                            If oDataRow("CurrentEmployee") = 1 And oDataRow("Enddate") <> CDate("01/01/2079") Then
                                ' Empleado con movilidad
                                oDataRow("Type") = 2
                            Else
                                ' Empleado normal
                                oDataRow("Type") = 1
                            End If
                        End If
                    End If
                Next

                oRet.AcceptChanges()
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetEmployeesFromUserField")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetEmployeesFromUserField")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeeUserFieldValueAtDateById(ByVal idEmployee As Integer, ByVal idUserField As Integer, ByVal atDate As Date, ByVal state As roUserFieldState,
                                                                Optional ByVal bAudit As Boolean = False) As roEmployeeUserField
            Dim returnValue As roEmployeeUserField = Nothing

            Try

                Dim strUserFieldName As String = String.Empty

                Dim strSQL As String
                strSQL = $"@SELECT# FieldName FROM sysroUserFields WHERE Id = {idUserField}"
                strUserFieldName = roTypes.Any2String(AccessHelper.ExecuteScalar(strSQL, ))

                returnValue = GetEmployeeUserFieldValueAtDate(idEmployee, strUserFieldName, atDate, state)

            Catch ex As DbException
                state.UpdateStateInfo(ex, "roEmployeeUserField::GetIDEmployeesFromUserFieldIdAndValue")
            Catch ex As Exception
                state.UpdateStateInfo(ex, "roEmployeeUserField::GetIDEmployeesFromUserFieldIdAndValue")
            End Try

            Return returnValue

        End Function

        ''' <summary>
        ''' Obtiene el valor de un campo de la ficha para un empleado vigente a una fecha en concreto.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado.</param>
        ''' <param name="_FieldName">Nombre del campo de la ficha.</param>
        ''' <param name="_Date">Fecha en la que es vigente el valor.</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns>Objeto 'roEmployeeUserField' con el valor del campo de la ficha.</returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeeUserFieldValueAtDate(ByVal _IDEmployee As String, ByVal _FieldName As String, ByVal _Date As Date, ByVal _State As roUserFieldState,
                                                                Optional ByVal bAudit As Boolean = False) As roEmployeeUserField

            Dim oRet As roEmployeeUserField = Nothing

            Try

                Dim strSQL As String

                strSQL = "@DECLARE# @Date smalldatetime " &
                         "SET @Date = " & Any2Time(_Date).SQLSmallDateTime & " " &
                         "@SELECT# * FROM GetEmployeeUserFieldValue(" & _IDEmployee.ToString & ",'" & _FieldName & "', @Date)"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    oRet = New roEmployeeUserField(_IDEmployee, _FieldName, tb.Rows(0).Item("Date"), _State)

                    If bAudit Then
                        ' Auditamos
                        Dim tbParameters As DataTable = _State.CreateAuditParameters()
                        _State.AddAuditParameter(tbParameters, "{IDEmployee}", _IDEmployee, "", 1)
                        _State.AddAuditParameter(tbParameters, "{FieldName}", _FieldName, "", 1)
                        _State.Audit(Audit.Action.aSelect, Audit.ObjectType.tUserField, _FieldName, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetEmployeeUserFieldValueAtDate")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeUserField::GetEmployeeUserFieldValueAtDate")
            Finally

            End Try

            Return oRet

        End Function

#End Region

#End Region

    End Class

    <DataContract>
    <Serializable>
    Public Class roGroupUserField

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roUserFieldState

        Private StrFieldName As String
        Private ObjFieldValue As Object
        Private oDefinition As roUserField

        Public Sub New()
            Me.oState = New roUserFieldState()
        End Sub

        Public Sub New(ByVal _State As roUserFieldState)
            Me.oState = _State
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roUserFieldState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roUserFieldState)
                Me.oState = value
            End Set
        End Property
        <DataMember>
        Public Property FieldName() As String
            Get
                Return StrFieldName
            End Get
            Set(ByVal value As String)
                StrFieldName = value
            End Set
        End Property
        <DataMember>
        Public Property FieldValue() As Object
            Get
                Return ObjFieldValue
            End Get
            Set(ByVal value As Object)
                If IsDBNull(value) Then
                    ObjFieldValue = ""
                Else
                    ObjFieldValue = value
                End If
            End Set
        End Property
        <DataMember>
        Public Property Definition() As roUserField
            Get
                Return Me.oDefinition
            End Get
            Set(ByVal value As roUserField)
                Me.oDefinition = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function GetDefinition(Optional ByVal oUserFieldState As roUserFieldState = Nothing) As roUserField

            If oUserFieldState Is Nothing Then oUserFieldState = New roUserFieldState(Me.oState.IDPassport)

            Return New roUserField(oUserFieldState, Me.FieldName, Types.GroupField, False)

        End Function

#Region "Helper methods"

        Public Shared Function GetUserField(ByVal IDGroup As Integer, ByVal UserFieldName As String, ByVal _State As roUserFieldState) As roGroupUserField
            'Devuelve el UserField especificado del empleado solicitdado

            Dim oRet As roGroupUserField = Nothing

            _State.UpdateStateInfo()

            Try

                Dim strSQL As String

                ' Recuperar el valor de UserFieldName
                strSQL = "@SELECT# " & UserFieldName & " " &
                         "FROM Groups " &
                         "WHERE ID = " & IDGroup
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing Then

                    oRet = New roGroupUserField
                    oRet.FieldName = tb.Columns(0).ColumnName.Remove(0, 4)
                    oRet.FieldValue = tb.Rows(0).Item(0)
                    oRet.Definition = oRet.GetDefinition()

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roGroupUserField::GetUserField")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roGroupUserField::GetUserField")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetUserFieldsList(ByVal IDGroup As Integer, ByVal _State As roUserFieldState) As Generic.List(Of roGroupUserField)
            ' Devuelvo todos los userfields de un grupo
            Dim oRet As New Generic.List(Of roGroupUserField)

            _State.UpdateStateInfo()

            Try

                Dim oUserFieldState As New roUserFieldState(_State.IDPassport)

                Dim strSelectFields As String = ""

                ' Obtenemos los permisos de acceso a la información de la ficha para el passpor actual en función del nivel de acceso.
                Dim bolLowAccess As Boolean = True
                Dim bolMediumAccess As Boolean = True
                Dim bolHighAccess As Boolean = True
                If _State.IDPassport > 0 Then
                    bolLowAccess = WLHelper.HasFeaturePermissionByGroup(_State.IDPassport, "Employees.UserFields.Information.Low", Permission.Read, IDGroup)
                    bolMediumAccess = WLHelper.HasFeaturePermissionByGroup(_State.IDPassport, "Employees.UserFields.Information.Medium", Permission.Read, IDGroup)
                    bolHighAccess = WLHelper.HasFeaturePermissionByGroup(_State.IDPassport, "Employees.UserFields.Information.High", Permission.Read, IDGroup)
                End If

                ' Primero recupero los campos personalizados que se estan usando
                ' Y los meto en StrSelectFields que usare mas tarde para componer la select
                ' Formato de StrSelectFields: "Usr_codigopostal, Usr_Direccion, Usr_Telefono"
                Dim oFieldsList As Generic.List(Of roUserField) = roUserField.GetUserFieldsList(Types.GroupField, False, oUserFieldState, "Used=1")
                If oFieldsList IsNot Nothing Then
                    For Each oUserField As roUserField In oFieldsList
                        If (oUserField.AccessLevel = AccessLevels.aLow And bolLowAccess) Or
                           (oUserField.AccessLevel = AccessLevels.aMedium And bolMediumAccess) Or
                           (oUserField.AccessLevel = AccessLevels.aHigh And bolHighAccess) Then
                            strSelectFields &= ",[Usr_" & oUserField.FieldName & "]"
                        End If
                    Next
                    If strSelectFields <> "" Then strSelectFields = strSelectFields.Substring(1)
                End If

                If strSelectFields <> "" Then

                    Dim strSQL As String

                    ' Recuperar esos valores de la tabla employees.
                    strSQL = "@SELECT# " & strSelectFields & " " &
                             "FROM Groups " &
                             "WHERE ID = " & IDGroup

                    Dim tbGroupFields As DataTable = CreateDataTable(strSQL, )
                    If tbGroupFields IsNot Nothing Then

                        Dim oGroupUserField As roGroupUserField

                        For nCol As Integer = 0 To tbGroupFields.Columns.Count - 1
                            oGroupUserField = New roGroupUserField
                            With oGroupUserField
                                .FieldName = tbGroupFields.Columns(nCol).ColumnName.Substring(4)
                                .FieldValue = tbGroupFields.Rows(0).Item(nCol)
                                .oDefinition = .GetDefinition(oUserFieldState)
                            End With
                            oRet.Add(oGroupUserField)
                        Next

                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roGroupUserField::GetUserFieldsList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roGroupUserField::GetUserFieldsList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetUserFieldsDataTable(ByVal IDGroup As Integer, ByVal _State As roUserFieldState) As DataTable
            ' Devuelvo todos los userfields de un grupo en un Dataset
            Dim oRet As DataTable = Nothing

            _State.UpdateStateInfo()

            Dim oGroupUserFields As Generic.List(Of roGroupUserField) = roGroupUserField.GetUserFieldsList(IDGroup, _State)
            If _State.Result = UserFieldResultEnum.NoError Then

                Try

                    oRet = New DataTable

                    Dim oDataColumn As DataColumn

                    oDataColumn = New DataColumn
                    oDataColumn.ColumnName = "FieldCaption"
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn
                    oDataColumn.ColumnName = "FieldName"
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn
                    oDataColumn.ColumnName = "Type"
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn("Value", GetType(String))
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn("ValueDateTime", GetType(DateTime))
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn
                    oDataColumn.ColumnName = "Category"
                    oRet.Columns.Add(oDataColumn)

                    oRet.Columns.Add(New DataColumn("AccessLevel", GetType(Integer)))

                    oDataColumn = New DataColumn("Description", GetType(String))
                    oRet.Columns.Add(oDataColumn)

                    oRet.Columns.Add(New DataColumn("AccessValidation", GetType(Integer)))

                    If oGroupUserFields IsNot Nothing Then

                        Dim oNewRow As DataRow = Nothing

                        For Each oGroupUserField As roGroupUserField In oGroupUserFields

                            oNewRow = oRet.NewRow

                            oNewRow("FieldCaption") = oGroupUserField.FieldName ' oUserField.FieldCaption
                            oNewRow("FieldName") = oGroupUserField.FieldName
                            oNewRow("Type") = Val(oGroupUserField.Definition.FieldType)
                            oNewRow("Value") = oGroupUserField.FieldValue
                            If oGroupUserField.Definition IsNot Nothing Then
                                If oGroupUserField.Definition.FieldType = FieldTypes.tDate Then 'Or oGroupUserField.Definition.FieldType = FieldTypes.tTime Then
                                    If Not IsDBNull(oNewRow("Value")) AndAlso
                                       oNewRow("Value") <> "" Then
                                        oNewRow("ValueDateTime") = oGroupUserField.FieldValue
                                    End If
                                End If
                                oNewRow("Category") = oGroupUserField.Definition.Category
                                oNewRow("AccessLevel") = oGroupUserField.Definition.AccessLevel
                                oNewRow("Description") = oGroupUserField.Definition.Description
                                oNewRow("AccessValidation") = oGroupUserField.Definition.AccessValidation
                            End If

                            oRet.Rows.Add(oNewRow)

                        Next

                    End If
                Catch ex As DbException
                    _State.UpdateStateInfo(ex, "roGroupUserField::GetUserFieldsDataTable")
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roGroupUserField::GetUserFieldsDataTable")
                End Try

            End If

            Return oRet

        End Function

        Public Shared Function SaveUserField(ByVal IDGroup As Integer, ByVal UserFieldName As String, ByVal UserFieldValue As Object, ByVal _State As roUserFieldState,
                                             Optional ByVal bAudit As Boolean = False) As Boolean
            ' Guarda el dato de un campo personalizado de la tabla grupos

            _State.UpdateStateInfo()

            Try

                Dim oOldValue As Object = DBNull.Value
                Dim bolValueChanged As Boolean = False

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing
                Dim strObjectName As String = ""

                Dim strQuery As String

                If Not DataLayer.roSupport.IsXSSSafe(UserFieldValue) Then
                    _State.Result = UserFieldResultEnum.XSSvalidationError
                    Return False
                End If


                strQuery = "@SELECT# [ID], "
                If UserFieldName.ToUpper.StartsWith("USR_") Then
                    strQuery &= "[" & UserFieldName & "] "
                    strObjectName = UserFieldName.Substring(3)
                Else
                    strQuery &= "[Usr_" & UserFieldName & "] "
                    strObjectName = UserFieldName
                End If
                strQuery &= "FROM Groups WHERE ID = " & IDGroup.ToString

                Dim tb As New DataTable
                Dim cmd As DbCommand = CreateCommand(strQuery)
                Dim ad As DbDataAdapter = CreateDataAdapter(cmd, True)
                ad.Fill(tb)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    If Not IsDBNull(tb.Rows(0).Item(1)) Then
                        oOldValue = tb.Rows(0).Item(1)
                        oAuditDataOld = Extensions.roAudit.CloneRow(tb.Rows(0))
                    End If

                    If UserFieldValue IsNot Nothing Then
                        tb.Rows(0).Item(1) = UserFieldValue
                    Else
                        tb.Rows(0).Item(1) = DBNull.Value
                    End If
                    oAuditDataNew = Extensions.roAudit.CloneRow(tb.Rows(0))
                    ad.Update(tb)
                    bolValueChanged = (Any2String(oOldValue) <> Any2String(tb.Rows(0).Item(1)))
                End If

                If bolValueChanged Then

                    Dim oServerLicense As New roServerLicense
                    ' Si hay licencia para prevención de riesgos laborales
                    If oServerLicense.FeatureIsInstalled("Feature\OHP") Then

                        If UserFieldName.ToUpper.StartsWith("USR_") Then UserFieldName = UserFieldName.Substring(4)

                        ' Obtenemos la definición del campo de la ficha
                        Dim oUserFieldState As New roUserFieldState(_State.IDPassport)
                        Dim oField As New roUserField(oUserFieldState, UserFieldName, Types.GroupField, False)
                        If oField IsNot Nothing Then
                            ' Si está configurada para validación de acceso
                            If oField.AccessValidation > 0 Then
                                ' Notificamos al servidor que tiene que lanzar el broadcaster
                                ' ** Queda pendiente informar los IDs de los terminales a regenerar. Actualmente regenera los ficheros para todos los terminales tipo mx6
                                roConnector.InitTask(TasksType.BROADCASTER)
                            End If
                        End If

                    End If

                    If bAudit Then
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        _State.Audit(oAuditAction, Audit.ObjectType.tGroupUserField, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roGroupUserField::SaveUserField")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roGroupUserField::SaveUserField")
            Finally

            End Try

            Return (_State.Result = UserFieldResultEnum.NoError)

        End Function

        Public Shared Function SaveUserFields(ByVal _IDGroup As Integer, ByVal _UserFields As Generic.List(Of roGroupUserField), ByRef _State As roUserFieldState) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Miramos si hay licencia para prevención de riesgos laborales
                Dim oServerLicense As New roServerLicense
                Dim bolOHPLicense As Boolean = oServerLicense.FeatureIsInstalled("Feature\OHP")
                Dim bolBroadcaster As Boolean = False

                Dim oOldValue As Object = DBNull.Value
                Dim bolValueChanged As Boolean = False

                Dim strSQL As String

                Dim tb As DataTable
                Dim cmd As DbCommand
                Dim ad As DbDataAdapter

                For Each oGroupUserField As roGroupUserField In _UserFields

                    If oGroupUserField.Definition Is Nothing Then
                        oGroupUserField.Definition = oGroupUserField.GetDefinition()
                    End If

                    strSQL = "@SELECT# [ID], "
                    If oGroupUserField.FieldName.ToUpper.StartsWith("USR_") Then
                        strSQL &= "[" & oGroupUserField.FieldName & "] "
                    Else
                        strSQL &= "[Usr_" & oGroupUserField.FieldName & "] "
                    End If
                    strSQL &= "FROM Groups WHERE ID = " & _IDGroup.ToString

                    tb = New DataTable
                    cmd = CreateCommand(strSQL)
                    ad = CreateDataAdapter(cmd, True)
                    ad.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                        If Not IsDBNull(tb.Rows(0).Item(1)) Then oOldValue = tb.Rows(0).Item(1)
                        If oGroupUserField.FieldValue IsNot Nothing AndAlso Not IsDBNull(oGroupUserField.FieldValue) Then
                            Select Case oGroupUserField.Definition.FieldType
                                Case FieldTypes.tNumeric, FieldTypes.tDecimal,
                                     FieldTypes.tDate, FieldTypes.tTime, FieldTypes.tDocument
                                    If TypeOf oGroupUserField.FieldValue Is String AndAlso oGroupUserField.FieldValue = "" Then
                                        tb.Rows(0).Item(1) = DBNull.Value
                                    Else
                                        tb.Rows(0).Item(1) = oGroupUserField.FieldValue
                                    End If

                                Case Else
                                    tb.Rows(0).Item(1) = oGroupUserField.FieldValue
                            End Select
                        Else
                            tb.Rows(0).Item(1) = DBNull.Value
                        End If
                        ad.Update(tb)
                        bolValueChanged = (Any2String(oOldValue) <> Any2String(tb.Rows(0).Item(1)))
                    End If

                    If bolValueChanged AndAlso bolOHPLicense AndAlso oGroupUserField.Definition IsNot Nothing AndAlso oGroupUserField.Definition.AccessValidation > 0 Then
                        bolBroadcaster = True
                    End If

                Next

                If bolBroadcaster Then
                    ' Notificamos al servidor que tiene que lanzar el broadcaster
                    roConnector.InitTask(TasksType.BROADCASTER)
                End If

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roGroupUserField::SaveUserFields")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roGroupUserField::SaveUserFields")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function GetUserFieldValues(ByVal strUserField As String, ByVal xDate As Date, ByVal strParent As String, ByVal strSeparator As String, ByVal strFieldWhere As String, ByVal _State As roUserFieldState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# DISTINCT "

                'IMPORTANTE:El parametro strFieldWhere se debe procesar adecuadamente. Según la forma Expresion1 + "#@#" +  Expresion2
                If strFieldWhere <> String.Empty Then
                    If strFieldWhere.IndexOf("#@#") > 0 Then
                        Dim strSplit As String() = {"#@#"}
                        Dim tmpFilter = strFieldWhere.Split(strSplit, StringSplitOptions.None)

                        strSQL = tmpFilter(0) & " " & strSQL 'Contiene la expresion del declare para el parametro de funciones de la BBDD
                        strFieldWhere = tmpFilter(1) 'Contiene el filtro real
                    End If
                End If

                Dim strUserFieldName As String = strUserField
                If strUserFieldName.ToUpper.StartsWith("USR_") Then strUserFieldName = strUserFieldName.Substring(4)

                Dim oUserFieldState As New roUserFieldState(_State.IDPassport, _State.Context, _State.ClientAddress)
                Dim oUserField As New roUserField(oUserFieldState, strUserFieldName, Types.GroupField, False)

                Dim strUF As String = "CONVERT(varchar, [" & strUserField & "])"

                If strParent = "" Then
                    strSQL &= strUF & " AS [Name], " & strUF & " AS Path "
                Else
                    strSQL &= "SUBSTRING(" & strUF & ", LEN('" & strParent & strSeparator & "')+1, LEN(" & strUF & ") - LEN('" & strParent & strSeparator & "')) AS [Name], " & strUF & " AS Path "
                End If
                strSQL &= "FROM Groups "
                If strParent = "" Then
                    strSQL &= "WHERE CHARINDEX('" & strSeparator & "', " & strUF & ") = 0 AND " & strUF & " <> '' "
                Else
                    strSQL &= "WHERE " & strUF & " LIKE '" & strParent & strSeparator & "%' AND " &
                                    "CHARINDEX('" & strSeparator & "', SUBSTRING(" & strUF & ", LEN('" & strParent & strSeparator & "')+1, LEN(" & strUF & ") - LEN('" & strParent & strSeparator & "'))) = 0 "
                End If
                If strFieldWhere <> "" Then
                    strSQL &= " AND " & strFieldWhere & " "
                End If
                strSQL &= "ORDER BY " & strUF
                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roGroupUserField::GetUserFieldValues")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roGroupUserField::GetUserFieldValues")
            Finally

            End Try

            Return oRet

        End Function

#End Region

#End Region

    End Class

    <DataContract>
    <Serializable>
    Public Class roTaskFieldDefinition

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roTaskFieldState

        Private intID As Integer
        Private strName As String
        Private oType As FieldTypes
        Private oAction As ActionTypes
        Private TypeValue As Integer
        Private oValueType As ValueTypes
        Private oListValues As Generic.List(Of String)

        Public Sub New()
            Me.oState = New roTaskFieldState()
            Me.strName = ""
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            Me.oState = New roTaskFieldState(_IDPassport)
            Me.strName = ""
        End Sub

        Public Sub New(ByVal _State As roTaskFieldState, ByVal _ID As Integer,
                       Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intID = _ID
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roTaskFieldState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roTaskFieldState)
                Me.oState = value
            End Set
        End Property
        <DataMember>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property
        <DataMember>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property
        <DataMember>
        Public Property Type() As FieldTypes
            Get
                Return Me.oType
            End Get
            Set(ByVal value As FieldTypes)
                Me.oType = value
            End Set
        End Property
        <DataMember>
        Public Property Action() As ActionTypes
            Get
                Return Me.oAction
            End Get
            Set(ByVal value As ActionTypes)
                Me.oAction = value
            End Set
        End Property
        <DataMember>
        Public Property ValueType() As ValueTypes
            Get
                Return Me.oValueType
            End Get
            Set(ByVal value As ValueTypes)
                Me.oValueType = value
            End Set
        End Property
        <DataMember>
        Public Property ListValues() As Generic.List(Of String)
            Get
                Return Me.oListValues
            End Get
            Set(ByVal value As Generic.List(Of String))
                Me.oListValues = value
            End Set
        End Property
        <Xml.Serialization.XmlIgnore()>
        <IgnoreDataMember()>
        Public Property ListValuesString() As String
            Get
                Dim strValues As String = ""
                If Me.oListValues IsNot Nothing Then
                    For Each strValue As String In Me.oListValues
                        strValues &= "~" & strValue
                    Next
                    If strValues.Length > 0 Then strValues = strValues.Substring(1)
                End If
                Return strValues
            End Get
            Set(ByVal value As String)
                Me.oListValues = New Generic.List(Of String)
                If value.Length > 0 Then
                    For Each strValue As String In value.Split("~")
                        Me.oListValues.Add(strValue)
                    Next
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroFieldsTask WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    If IsDBNull(oRow("Type")) Then
                        Me.oType = FieldTypes.tText
                    Else
                        Me.oType = oRow("Type")
                    End If

                    Me.Name = oRow("Name")

                    If IsDBNull(oRow("Action")) Then
                        Me.oAction = ActionTypes.aCreate
                    Else
                        Me.oAction = oRow("Action")
                    End If

                    If IsDBNull(oRow("TypeValue")) Then
                        Me.oValueType = FieldTypes.tText
                    Else
                        Me.oValueType = oRow("TypeValue")
                    End If

                    Me.ListValuesString = roTypes.Any2String(oRow("ListValues"))

                    bolRet = True

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{TaskFieldName}", Me.strName, "", 1)
                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tTaskFieldDefinition, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTaskFieldDefinition::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTaskFieldDefinition::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim oAuditDataOld As DataRow = Nothing
            Dim oAuditDataNew As DataRow = Nothing
            Dim bHaveToClose As Boolean = False

            Try

                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = UserFieldResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Validate() Then

                    Dim tb As New DataTable("sysroFieldsTask")
                    Dim strSQL As String = "@SELECT# * FROM sysroFieldsTask WHERE ID = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)

                    oRow("Name") = Me.strName
                    oRow("Action") = Me.oAction
                    oRow("TypeValue") = Me.oValueType
                    oRow("ListValues") = Me.ListValuesString

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    oAuditDataNew = oRow

                    If bolRet And bAudit Then
                        'bolRet = False
                        '' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tTaskFieldDefinition, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTaskFieldDefinition::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTaskFieldDefinition::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = False

            Try

                bolRet = True

                ' No permitimos cambiar la accion del campo de la ficha en caso que ya este asignado a alguna tarea o plantilla
                ' con el actual comportamiento

                Dim oTaskFieldDefinition As New roTaskFieldDefinition(Me.oState, Me.ID)

                If Me.Action <> oTaskFieldDefinition.Action Then

                    Dim intCount As Integer = 0
                    intCount = Any2Integer(ExecuteScalar("@SELECT# ISNULL(COUNT(*), 0) as Total FROM FieldsTask where IDField =" & Me.ID))
                    If intCount <> 0 Then
                        bolRet = False
                    End If

                    If Not bolRet Then
                        intCount = Any2Integer(ExecuteScalar("@SELECT# ISNULL(COUNT(*), 0) as Total FROM FieldsTaskTemplate where IDField =" & Me.ID))
                        If intCount <> 0 Then
                            bolRet = False
                        End If

                        If Not bolRet Then
                            intCount = Any2Integer(ExecuteScalar("@SELECT# ISNULL(COUNT(*), 0) as Total FROM FieldsProjectTemplate where IDField =" & Me.ID))
                            If intCount <> 0 Then
                                bolRet = False
                            End If
                        End If
                    End If

                    If Not bolRet Then
                        Me.oState.Result = TaskFieldResultEnum.TaskFieldUsedInTasks
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTaskFieldDefinition::Validate")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTaskFieldDefinition::Validate")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

        Public Shared Function SaveTaskFields(ByVal tbTaskFields As DataTable, ByVal _State As roTaskFieldState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oTaskField As roTaskFieldDefinition
                Dim bolSaved As Boolean = True

                For Each oRow As DataRow In tbTaskFields.Rows

                    bolSaved = True

                    Select Case oRow.RowState
                        Case DataRowState.Added, DataRowState.Modified

                            oTaskField = New roTaskFieldDefinition(_State, oRow("ID"), False)
                            With oTaskField
                                .Name = oRow("Name")
                                .Type = oRow("Type")
                                .Action = oRow("Action")
                                .ListValuesString = Any2String(oRow("ListValues"))
                                .ValueType = oRow("TypeValue")
                            End With
                            bolRet = oTaskField.Save(bAudit)

                            If Not bolRet Then
                                _State.Result = oTaskField.oState.Result
                            End If

                        Case Else
                            bolRet = True
                            bolSaved = False
                    End Select

                    If Not bolRet Then
                        Exit For
                    End If

                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskFieldDefinition::SaveTaskFields")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::SaveTaskFields")
            Finally

                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

    End Class

    <DataContract()>
    <Serializable>
    Public Class roTaskFieldProjectTemplate

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roTaskFieldState

        Private intIDProjectTemplate As Integer
        Private intIDField As Integer
        Private strFieldName As String

        Public Sub New()
            Me.oState = New roTaskFieldState()
        End Sub

        Public Sub New(ByVal _State As roTaskFieldState)
            Me.oState = _State
        End Sub

        Public Sub New(ByVal _State As roTaskFieldState, ByVal _IDProjectTemplate As Integer, ByVal _IDFieldTask As Integer)
            Me.oState = _State
            Me.intIDField = _IDFieldTask
            Me.intIDProjectTemplate = _IDProjectTemplate

            Dim ostate As New roTaskFieldState()
            Dim oTaskFieldDefinition As New roTaskFieldDefinition(ostate, _IDFieldTask, )

            If oTaskFieldDefinition IsNot Nothing Then
                Me.strFieldName = oTaskFieldDefinition.Name
            End If

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roTaskFieldState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roTaskFieldState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property IDProjectTemplate() As Integer
            Get
                Return intIDProjectTemplate
            End Get
            Set(ByVal value As Integer)
                intIDProjectTemplate = value
            End Set
        End Property
        <DataMember()>
        Public Property IDField() As Integer
            Get
                Return intIDField
            End Get
            Set(ByVal value As Integer)
                intIDField = value
            End Set
        End Property
        <DataMember()>
        Public Property FieldName() As String
            Get
                Return strFieldName
            End Get
            Set(ByVal value As String)
                strFieldName = value
            End Set
        End Property

#End Region

#Region "Methods"

#Region "Helper methods"

        Public Shared Function GetTaskFieldsProjectTemplateDataTable(ByVal IDProjectTemplate As Integer, ByVal _State As roTaskFieldState) As DataTable
            ' Devuelvo todos los taskfieldsTemplates de un proyecto de plantilla en un Dataset
            Dim oRet As DataTable = Nothing

            _State.UpdateStateInfo()

            Dim TaskFieldsProjectTemplate As Generic.List(Of roTaskFieldProjectTemplate) = roTaskFieldProjectTemplate.GetTaskFieldsProjectTemplateList(IDProjectTemplate, _State)
            If _State.Result = TaskFieldResultEnum.NoError Then

                Try

                    oRet = New DataTable

                    Dim oDataColumn As DataColumn

                    oRet.Columns.Add(New DataColumn("IDProjectTemplate", GetType(Integer)))
                    oRet.Columns.Add(New DataColumn("IDField", GetType(Integer)))

                    oDataColumn = New DataColumn("FieldName", GetType(String))
                    oRet.Columns.Add(oDataColumn)

                    If TaskFieldsProjectTemplate IsNot Nothing Then

                        Dim oNewRow As DataRow = Nothing

                        For Each oTaskFieldProjectTemplate As roTaskFieldProjectTemplate In TaskFieldsProjectTemplate

                            oNewRow = oRet.NewRow

                            oNewRow("IDProjectTemplate") = oTaskFieldProjectTemplate.IDProjectTemplate
                            oNewRow("IDField") = oTaskFieldProjectTemplate.IDField
                            oNewRow("FieldName") = Any2String(oTaskFieldProjectTemplate.FieldName)

                            oRet.Rows.Add(oNewRow)

                        Next

                    End If
                Catch ex As DbException
                    _State.UpdateStateInfo(ex, "roTaskFieldProjectTemplate::GetTaskFieldsProjectTemplateDataTable")
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roTaskFieldProjectTemplate::GetTaskFieldsProjectTemplateDataTable")
                End Try

            End If

            Return oRet

        End Function

        Public Shared Function GetAvailableTaskFieldsProjectTemplateDataTable(ByVal IDProjectTemplate As Integer, ByVal _State As roTaskFieldState) As DataTable
            ' Devuelvo todos los taskfieldsTemplates de un proyecto de plantilla en un Dataset
            Dim oRet As DataTable = Nothing

            _State.UpdateStateInfo()

            Dim TaskFieldsProjectTemplate As Generic.List(Of roTaskFieldProjectTemplate) = roTaskFieldProjectTemplate.GetAvailableTaskFieldsProjectTemplateList(IDProjectTemplate, _State)
            If _State.Result = TaskFieldResultEnum.NoError Then

                Try

                    oRet = New DataTable

                    Dim oDataColumn As DataColumn

                    oRet.Columns.Add(New DataColumn("IDProjectTemplate", GetType(Integer)))
                    oRet.Columns.Add(New DataColumn("IDField", GetType(Integer)))

                    oDataColumn = New DataColumn("FieldName", GetType(String))
                    oRet.Columns.Add(oDataColumn)

                    If TaskFieldsProjectTemplate IsNot Nothing Then

                        Dim oNewRow As DataRow = Nothing

                        For Each oTaskFieldProjectTemplate As roTaskFieldProjectTemplate In TaskFieldsProjectTemplate

                            oNewRow = oRet.NewRow

                            oNewRow("IDProjectTemplate") = oTaskFieldProjectTemplate.IDProjectTemplate
                            oNewRow("IDField") = oTaskFieldProjectTemplate.IDField
                            oNewRow("FieldName") = Any2String(oTaskFieldProjectTemplate.FieldName)

                            oRet.Rows.Add(oNewRow)

                        Next

                    End If
                Catch ex As DbException
                    _State.UpdateStateInfo(ex, "roTaskFieldProjectTemplate::GetTaskFieldsProjectTemplateDataTable")
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roTaskFieldProjectTemplate::GetTaskFieldsProjectTemplateDataTable")
                End Try

            End If

            Return oRet

        End Function

        Public Shared Function GetTaskFieldsProjectTemplateList(ByVal _IDTaskTemplate As Integer, ByVal _State As roTaskFieldState) As Generic.List(Of roTaskFieldProjectTemplate)

            Dim oRet As New Generic.List(Of roTaskFieldProjectTemplate)

            Try

                Dim strSQL As String = "@SELECT# IDProjectTemplate, IDField FROM FieldsProjectTemplate " &
                                       "WHERE IDProjectTemplate = " & _IDTaskTemplate

                Dim tbFields As DataTable = CreateDataTable(strSQL, )

                If tbFields IsNot Nothing AndAlso tbFields.Rows.Count > 0 Then

                    For Each oRow As DataRow In tbFields.Rows
                        oRet.Add(New roTaskFieldProjectTemplate(_State, oRow("IDProjectTemplate"), oRow("IDField")))
                    Next

                    ' Auditamos consulta masiva
                    'Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                    'Dim strAuditName As String = ""
                    'For Each oRow As DataRow In tbFields.Rows
                    '    strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("FieldName")
                    'Next
                    'Audit.roAudit.AddParameter(tbAuditParameters, "{UserFieldNames}", strAuditName, "", 1)
                    '_State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tUserField, strAuditName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskFieldProyectTemplate::GetTaskFieldsProjectTemplateList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskFieldProyectTemplate::GetTaskFieldsProjectTemplateList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetAvailableTaskFieldsProjectTemplateList(ByVal _IDTaskTemplate As Integer, ByVal _State As roTaskFieldState) As Generic.List(Of roTaskFieldProjectTemplate)

            Dim oRet As New Generic.List(Of roTaskFieldProjectTemplate)

            Try

                Dim strSQL As String = "@SELECT# ID FROM sysroFieldsTask WHERE" &
                        "(ID NOT IN (@SELECT# DISTINCT IDField FROM FieldsTaskTemplate WHERE " &
                        "(IDTaskTemplate IN (@SELECT# ID FROM TaskTemplates WHERE IDProject = " & _IDTaskTemplate & "))))"

                Dim tbFields As DataTable = CreateDataTable(strSQL, )

                If tbFields IsNot Nothing AndAlso tbFields.Rows.Count > 0 Then

                    For Each oRow As DataRow In tbFields.Rows
                        oRet.Add(New roTaskFieldProjectTemplate(_State, _IDTaskTemplate, oRow("ID")))
                    Next

                    ' Auditamos consulta masiva
                    'Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                    'Dim strAuditName As String = ""
                    'For Each oRow As DataRow In tbFields.Rows
                    '    strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("FieldName")
                    'Next
                    'Audit.roAudit.AddParameter(tbAuditParameters, "{UserFieldNames}", strAuditName, "", 1)
                    '_State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tUserField, strAuditName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskFieldProyectTemplate::GetTaskFieldsProjectTemplateList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskFieldProyectTemplate::GetTaskFieldsProjectTemplateList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveTaskFieldsProjectTemplate(ByVal _IDprojectTemplate As Integer, ByVal _TaskFieldsProjectTemplate As Generic.List(Of roTaskFieldProjectTemplate), ByRef _State As roTaskFieldState) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String

                strSQL = "@DELETE# FROM FieldsProjectTemplate WHERE "
                strSQL &= "IDProjectTemplate =" & _IDprojectTemplate
                bolRet = ExecuteSql(strSQL)

                If bolRet Then

                    For Each oTaskFieldProjectTemplate As roTaskFieldProjectTemplate In _TaskFieldsProjectTemplate
                        strSQL = "@INSERT# INTO FieldsProjectTemplate (IDProjectTemplate, IDField) VALUES ("
                        strSQL = strSQL & oTaskFieldProjectTemplate.IDProjectTemplate & ","
                        strSQL = strSQL & oTaskFieldProjectTemplate.IDField & ")"
                        bolRet = ExecuteSql(strSQL)
                    Next
                End If

                If bolRet Then
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Dim strAuditName As String = ""
                    Dim strName As String = Any2String(ExecuteScalar("@SELECT# Project FROM ProjectTemplates WHERE [ID] = " & _IDprojectTemplate))

                    Dim tbParameters As DataTable = _State.CreateAuditParameters()
                    _State.AddAuditParameter(tbParameters, "{Name}", strName, "", 1)
                    _State.Audit(Audit.Action.aUpdate, Audit.ObjectType.tProjectTemplate, strName, tbParameters, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskFieldProjectField::SaveTaskFieldsProjectTemplate")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskFieldProjectField::SaveTaskFieldsProjectTemplate")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

    <DataContract()>
    <Serializable>
    Public Class roTaskFieldTaskTemplate

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roTaskFieldState

        Private intIDTaskTemplate As Integer
        Private intIDField As Integer
        Private strFieldName As String

        Public Sub New()
            Me.oState = New roTaskFieldState()
        End Sub

        Public Sub New(ByVal _State As roTaskFieldState)
            Me.oState = _State
        End Sub

        Public Sub New(ByVal _State As roTaskFieldState, ByVal _IDTaskTemplate As Integer, ByVal _IDFieldTask As Integer)
            Me.oState = _State
            Me.intIDField = _IDFieldTask
            Me.intIDTaskTemplate = _IDTaskTemplate

            Dim ostate As New roTaskFieldState()
            Dim oTaskFieldDefinition As New roTaskFieldDefinition(ostate, _IDFieldTask, )

            If oTaskFieldDefinition IsNot Nothing Then
                Me.strFieldName = oTaskFieldDefinition.Name
            End If

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roTaskFieldState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roTaskFieldState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property IDTaskTemplate() As Integer
            Get
                Return intIDTaskTemplate
            End Get
            Set(ByVal value As Integer)
                intIDTaskTemplate = value
            End Set
        End Property
        <DataMember()>
        Public Property IDField() As Integer
            Get
                Return intIDField
            End Get
            Set(ByVal value As Integer)
                intIDField = value
            End Set
        End Property
        <DataMember()>
        Public Property FieldName() As String
            Get
                Return strFieldName
            End Get
            Set(ByVal value As String)
                strFieldName = value
            End Set
        End Property

#End Region

#Region "Methods"

#Region "Helper methods"

        Public Shared Function GetTaskFieldsTaskTemplateDataTable(ByVal IDTaskTemplate As Integer, ByVal _State As roTaskFieldState) As DataTable
            ' Devuelvo todos los taskfieldsTemplates de un proyecto de plantilla en un Dataset
            Dim oRet As DataTable = Nothing

            _State.UpdateStateInfo()

            Dim TaskFieldsTaskTemplate As Generic.List(Of roTaskFieldTaskTemplate) = roTaskFieldTaskTemplate.GetTaskFieldsTaskTemplateList(IDTaskTemplate, _State)
            If _State.Result = TaskFieldResultEnum.NoError Then

                Try

                    oRet = New DataTable

                    Dim oDataColumn As DataColumn

                    oRet.Columns.Add(New DataColumn("IDTaskTemplate", GetType(Integer)))
                    oRet.Columns.Add(New DataColumn("IDField", GetType(Integer)))

                    oDataColumn = New DataColumn("FieldName", GetType(String))
                    oRet.Columns.Add(oDataColumn)

                    If TaskFieldsTaskTemplate IsNot Nothing Then

                        Dim oNewRow As DataRow = Nothing

                        For Each oTaskFieldTaskTemplate As roTaskFieldTaskTemplate In TaskFieldsTaskTemplate

                            oNewRow = oRet.NewRow

                            oNewRow("IDTaskTemplate") = oTaskFieldTaskTemplate.IDTaskTemplate
                            oNewRow("IDField") = oTaskFieldTaskTemplate.IDField
                            oNewRow("FieldName") = Any2String(oTaskFieldTaskTemplate.FieldName)

                            oRet.Rows.Add(oNewRow)

                        Next

                    End If
                Catch ex As DbException
                    _State.UpdateStateInfo(ex, "roTaskFieldTaskTemplate::GetTaskFieldsTaskTemplateDataTable")
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roTaskFieldTaskTemplate::GetTaskFieldsTaskTemplateDataTable")
                End Try

            End If

            Return oRet

        End Function

        Public Shared Function GetAvailableTaskFieldsTaskTemplateDataTable(ByVal IDTaskTemplate As Integer, ByVal _State As roTaskFieldState) As DataTable
            ' Devuelvo todos los taskfieldsTemplates de un proyecto de plantilla en un Dataset
            Dim oRet As DataTable = Nothing

            _State.UpdateStateInfo()

            Dim TaskFieldsTaskTemplate As Generic.List(Of roTaskFieldTaskTemplate) = roTaskFieldTaskTemplate.GetAvailableTaskFieldsTaskTemplateList(IDTaskTemplate, _State)
            If _State.Result = TaskFieldResultEnum.NoError Then

                Try

                    oRet = New DataTable

                    Dim oDataColumn As DataColumn

                    oRet.Columns.Add(New DataColumn("IDTaskTemplate", GetType(Integer)))
                    oRet.Columns.Add(New DataColumn("IDField", GetType(Integer)))

                    oDataColumn = New DataColumn("FieldName", GetType(String))
                    oRet.Columns.Add(oDataColumn)

                    If TaskFieldsTaskTemplate IsNot Nothing Then

                        Dim oNewRow As DataRow = Nothing

                        For Each oTaskFieldTaskTemplate As roTaskFieldTaskTemplate In TaskFieldsTaskTemplate

                            oNewRow = oRet.NewRow

                            oNewRow("IDTaskTemplate") = oTaskFieldTaskTemplate.IDTaskTemplate
                            oNewRow("IDField") = oTaskFieldTaskTemplate.IDField
                            oNewRow("FieldName") = Any2String(oTaskFieldTaskTemplate.FieldName)

                            oRet.Rows.Add(oNewRow)

                        Next

                    End If
                Catch ex As DbException
                    _State.UpdateStateInfo(ex, "roTaskFieldTaskTemplate::GetTaskFieldsTaskTemplateDataTable")
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roTaskFieldTaskTemplate::GetTaskFieldsTaskTemplateDataTable")
                End Try

            End If

            Return oRet

        End Function

        Public Shared Function GetTaskFieldsTaskTemplateList(ByVal _IDTaskTemplate As Integer, ByVal _State As roTaskFieldState) As Generic.List(Of roTaskFieldTaskTemplate)

            Dim oRet As New Generic.List(Of roTaskFieldTaskTemplate)

            Try

                Dim strSQL As String = "@SELECT# IDTaskTemplate, IDField FROM FieldsTaskTemplate " &
                                       "WHERE IDTaskTemplate = " & _IDTaskTemplate

                Dim tbFields As DataTable = CreateDataTable(strSQL, )

                If tbFields IsNot Nothing AndAlso tbFields.Rows.Count > 0 Then

                    For Each oRow As DataRow In tbFields.Rows
                        oRet.Add(New roTaskFieldTaskTemplate(_State, oRow("IDTaskTemplate"), oRow("IDField")))
                    Next

                    ' Auditamos consulta masiva
                    'Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                    'Dim strAuditName As String = ""
                    'For Each oRow As DataRow In tbFields.Rows
                    '    strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("FieldName")
                    'Next
                    'Audit.roAudit.AddParameter(tbAuditParameters, "{UserFieldNames}", strAuditName, "", 1)
                    '_State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tUserField, strAuditName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskFieldProyectTemplate::GetTaskFieldsTaskTemplateList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskFieldProyectTemplate::GetTaskFieldsTaskTemplateList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetAvailableTaskFieldsTaskTemplateList(ByVal _IDTaskTemplate As Integer, ByVal _State As roTaskFieldState) As Generic.List(Of roTaskFieldTaskTemplate)

            Dim oRet As New Generic.List(Of roTaskFieldTaskTemplate)

            Try

                Dim strSQL As String = " @SELECT# ID FROM sysroFieldsTask WHERE " &
                    "(ID NOT IN (@SELECT# IDField FROM FieldsProjectTemplate WHERE " &
                    "(IDProjectTemplate IN (@SELECT# IDProject FROM TaskTemplates WHERE ID = " & _IDTaskTemplate & "))))"

                Dim tbFields As DataTable = CreateDataTable(strSQL, )

                If tbFields IsNot Nothing AndAlso tbFields.Rows.Count > 0 Then

                    For Each oRow As DataRow In tbFields.Rows
                        oRet.Add(New roTaskFieldTaskTemplate(_State, _IDTaskTemplate, oRow("ID")))
                    Next

                    ' Auditamos consulta masiva
                    'Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                    'Dim strAuditName As String = ""
                    'For Each oRow As DataRow In tbFields.Rows
                    '    strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("FieldName")
                    'Next
                    'Audit.roAudit.AddParameter(tbAuditParameters, "{UserFieldNames}", strAuditName, "", 1)
                    '_State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tUserField, strAuditName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskFieldProyectTemplate::GetTaskFieldsTaskTemplateList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskFieldProyectTemplate::GetTaskFieldsTaskTemplateList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveTaskFieldsTaskTemplate(ByVal _IDTaskTemplate As Integer, ByVal _TaskFieldsTaskTemplate As Generic.List(Of roTaskFieldTaskTemplate), ByRef _State As roTaskFieldState) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String

                strSQL = "@DELETE# FROM FieldsTaskTemplate WHERE "
                strSQL &= "IDTaskTemplate =" & _IDTaskTemplate
                bolRet = ExecuteSql(strSQL)

                If bolRet Then

                    For Each oTaskFieldTaskTemplate As roTaskFieldTaskTemplate In _TaskFieldsTaskTemplate
                        strSQL = "@INSERT# INTO FieldsTaskTemplate (IDTaskTemplate, IDField) VALUES ("
                        strSQL = strSQL & oTaskFieldTaskTemplate.IDTaskTemplate & ","
                        strSQL = strSQL & oTaskFieldTaskTemplate.IDField & ")"
                        bolRet = ExecuteSql(strSQL)
                    Next
                End If

                If bolRet Then
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Dim strAuditName As String = ""

                    Dim strTaskName As String = Any2String(ExecuteScalar("@SELECT# name from TaskTemplates where id=" & _IDTaskTemplate))

                    Dim tbParameters As DataTable = _State.CreateAuditParameters()
                    _State.AddAuditParameter(tbParameters, "{Name}", strTaskName, "", 1)
                    _State.Audit(Audit.Action.aUpdate, Audit.ObjectType.tTaskTemplate, strTaskName, tbParameters, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskFieldProjectField::SaveTaskFieldsTaskTemplate")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskFieldProjectField::SaveTaskFieldsTaskTemplate")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

    <DataContract()>
    <Serializable>
    Public Class roTaskField

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roTaskFieldState

        Private intIDTask As Integer
        Private intIDField As Integer
        Private strFieldName As String
        Private ObjFieldValue As Object
        Private oType As FieldTypes
        Private oAction As ActionTypes
        Private oValueType As ValueTypes
        Private oListValues As Generic.List(Of String)

        Public Sub New()
            Me.oState = New roTaskFieldState()
        End Sub

        Public Sub New(ByVal _State As roTaskFieldState)
            Me.oState = _State
        End Sub

        Public Sub New(ByVal _State As roTaskFieldState, ByVal _IDTask As Integer, ByVal _IDFieldTask As Integer)
            Me.oState = _State
            Me.intIDField = _IDFieldTask
            Me.intIDTask = _IDTask

            Dim ostate As New roTaskFieldState()
            Dim oTaskFieldDefinition As New roTaskFieldDefinition(ostate, _IDFieldTask)

            If oTaskFieldDefinition IsNot Nothing Then
                Me.strFieldName = oTaskFieldDefinition.Name
                Me.oType = oTaskFieldDefinition.Type
                Me.oAction = oTaskFieldDefinition.Action
                Me.oValueType = oTaskFieldDefinition.ValueType
                Me.ListValuesString = oTaskFieldDefinition.ListValuesString

            End If

            Me.FieldValue = GetTaskFieldValue(_IDTask, _IDFieldTask, oType, _State)

        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property Type() As FieldTypes
            Get
                Return Me.oType
            End Get
            Set(ByVal value As FieldTypes)
                Me.oType = value
            End Set
        End Property

        <DataMember()>
        Public Property Action() As ActionTypes
            Get
                Return Me.oAction
            End Get
            Set(ByVal value As ActionTypes)
                Me.oAction = value
            End Set
        End Property
        <DataMember()>
        Public Property FieldValue() As Object
            Get
                Return ObjFieldValue
            End Get
            Set(ByVal value As Object)
                If IsDBNull(value) Then
                    ObjFieldValue = ""
                Else
                    ObjFieldValue = value
                End If
            End Set
        End Property
        <DataMember()>
        Public Property ValueType() As ValueTypes
            Get
                Return Me.oValueType
            End Get
            Set(ByVal value As ValueTypes)
                Me.oValueType = value
            End Set
        End Property
        <DataMember()>
        Public Property ListValues() As Generic.List(Of String)
            Get
                Return Me.oListValues
            End Get
            Set(ByVal value As Generic.List(Of String))
                Me.oListValues = value
            End Set
        End Property
        <Xml.Serialization.XmlIgnore()>
        <IgnoreDataMember()>
        Public Property ListValuesString() As String
            Get
                Dim strValues As String = ""
                If Me.oListValues IsNot Nothing Then
                    For Each strValue As String In Me.oListValues
                        strValues &= "~" & strValue
                    Next
                    If strValues.Length > 0 Then strValues = strValues.Substring(1)
                End If
                Return strValues
            End Get
            Set(ByVal value As String)
                Me.oListValues = New Generic.List(Of String)
                If value.Length > 0 Then
                    For Each strValue As String In value.Split("~")
                        Me.oListValues.Add(strValue)
                    Next
                End If
            End Set
        End Property

        <IgnoreDataMember()>
        Public Property State() As roTaskFieldState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roTaskFieldState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property IDTask() As Integer
            Get
                Return intIDTask
            End Get
            Set(ByVal value As Integer)
                intIDTask = value
            End Set
        End Property
        <DataMember()>
        Public Property IDField() As Integer
            Get
                Return intIDField
            End Get
            Set(ByVal value As Integer)
                intIDField = value
            End Set
        End Property
        <DataMember()>
        Public Property FieldName() As String
            Get
                Return strFieldName
            End Get
            Set(ByVal value As String)
                strFieldName = value
            End Set
        End Property

#End Region

#Region "Methods"

#Region "Helper methods"

        Public Shared Function GetTaskFieldValue(ByVal _IDTask As Integer, ByVal _IDFieldTask As Integer, ByVal _Type As FieldTypes, ByVal _State As roTaskFieldState) As Object

            Dim oRet As Object = DBNull.Value

            Dim strSQL As String = ""

            Try

                strSQL = "@SELECT# [Field" & _IDFieldTask & "]  as Value" &
                        " FROM Tasks WHERE ID = " & _IDTask
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing Then

                    If Not IsDBNull(tb.Rows(0).Item("Value")) Then
                        Select Case _Type
                            Case FieldTypes.tText
                                oRet = CStr(tb.Rows(0).Item("Value"))
                            Case FieldTypes.tNumeric, FieldTypes.tDecimal
                                oRet = Any2Double(CStr(tb.Rows(0).Item("Value")).Replace(".", roConversions.GetDecimalDigitFormat))
                            Case Else
                                oRet = CStr(tb.Rows(0).Item("Value"))
                        End Select
                    Else
                        oRet = DBNull.Value
                    End If
                Else
                    oRet = DBNull.Value
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskField::GetTaskFieldValue")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskField::GetTaskFieldValue")
            Finally

            End Try

            Return oRet
        End Function

        Public Shared Function GetTaskFieldsDataTable(ByVal IDTask As Integer, ByVal _State As roTaskFieldState) As DataTable
            ' Devuelvo todos los taskfieldsTemplates de un proyecto de plantilla en un Dataset
            Dim oRet As DataTable = Nothing

            _State.UpdateStateInfo()

            Dim TaskFieldsTask As Generic.List(Of roTaskField) = roTaskField.GetTaskFieldsList(IDTask, _State)
            If _State.Result = TaskFieldResultEnum.NoError Then

                Try

                    oRet = New DataTable

                    Dim oDataColumn As DataColumn

                    oRet.Columns.Add(New DataColumn("IDTask", GetType(Integer)))
                    oRet.Columns.Add(New DataColumn("IDField", GetType(Integer)))

                    oDataColumn = New DataColumn("FieldName", GetType(String))
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn("Value", GetType(String))
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn("Type", GetType(Integer))
                    oRet.Columns.Add(oDataColumn)

                    oDataColumn = New DataColumn("Action", GetType(Integer))
                    oRet.Columns.Add(oDataColumn)

                    If TaskFieldsTask IsNot Nothing Then

                        Dim oNewRow As DataRow = Nothing

                        For Each oTaskFieldTask As roTaskField In TaskFieldsTask

                            oNewRow = oRet.NewRow

                            oNewRow("IDTask") = oTaskFieldTask.IDTask
                            oNewRow("IDField") = oTaskFieldTask.IDField
                            oNewRow("FieldName") = Any2String(oTaskFieldTask.FieldName)
                            oNewRow("Value") = Any2String(oTaskFieldTask.FieldValue)
                            oNewRow("Type") = Any2Integer(oTaskFieldTask.Type)
                            oNewRow("Action") = Any2Integer(oTaskFieldTask.Action)

                            oRet.Rows.Add(oNewRow)

                        Next

                    End If
                Catch ex As DbException
                    _State.UpdateStateInfo(ex, "roTaskFieldTask::GetTaskFieldsDataTable")
                Catch ex As Exception
                    _State.UpdateStateInfo(ex, "roTaskFieldTask::GetTaskFieldsDataTable")
                End Try

            End If

            Return oRet

        End Function

        Public Shared Function GetTaskFieldsList(ByVal _IDTask As Integer, ByVal _State As roTaskFieldState) As Generic.List(Of roTaskField)

            Dim oRet As New Generic.List(Of roTaskField)

            Try

                Dim strSQL As String = "@SELECT# IDTask, IDField FROM FieldsTask " &
                                       "WHERE IDTask = " & _IDTask

                Dim tbFields As DataTable = CreateDataTable(strSQL, )

                If tbFields IsNot Nothing AndAlso tbFields.Rows.Count > 0 Then

                    For Each oRow As DataRow In tbFields.Rows
                        oRet.Add(New roTaskField(_State, oRow("IDTask"), oRow("IDField")))
                    Next

                    ' Auditamos consulta masiva
                    'Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                    'Dim strAuditName As String = ""
                    'For Each oRow As DataRow In tbFields.Rows
                    '    strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("FieldName")
                    'Next
                    'Audit.roAudit.AddParameter(tbAuditParameters, "{UserFieldNames}", strAuditName, "", 1)
                    '_State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tUserField, strAuditName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskField::GetTaskFieldsList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskField::GetTaskFieldsList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveTaskFields(ByVal _IDTask As Integer, ByVal _TaskFieldsTask As Generic.List(Of roTaskField), ByRef _State As roTaskFieldState, Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bolRes As Boolean = True
            Dim strSQL As String = ""
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                strSQL = "@SELECT# * FROM FieldsTask WHERE IDTask= " & _IDTask
                ' Obtenemos los campos asignados actuales
                Dim ActualTaskFieldsTask As New Generic.List(Of roTaskField)
                Dim tbFields As DataTable = CreateDataTable(strSQL)

                If tbFields IsNot Nothing AndAlso tbFields.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbFields.Rows
                        ActualTaskFieldsTask.Add(New roTaskField(_State, _IDTask, oRow("IDField")))
                    Next
                End If

                bolRet = True

                ' Miramos que campos hay que eliminar
                For Each oTaskFieldTask As roTaskField In ActualTaskFieldsTask
                    Dim bolExistField As Boolean = False
                    For Each oActualTaskFieldTask As roTaskField In _TaskFieldsTask
                        If oActualTaskFieldTask.IDField = oTaskFieldTask.IDField Then
                            bolExistField = True
                            Exit For
                        End If
                    Next

                    bolRes = True
                    If Not bolExistField Then
                        ' Eliminamos los valores de la base de datos
                        Select Case oTaskFieldTask.Action
                            Case ActionTypes.aBegin, ActionTypes.aCreate
                                strSQL = "@UPDATE# Tasks SET [Field" & oTaskFieldTask.IDField & "]= NULL WHERE ID=" & _IDTask
                                bolRes = ExecuteSql(strSQL)

                            Case Else
                                strSQL = "@UPDATE# Punches SET [Field" & oTaskFieldTask.IDField & "]= NULL WHERE Type=4 AND TypeData=" & _IDTask
                                bolRes = ExecuteSql(strSQL)

                                If bolRes Then
                                    strSQL = "@UPDATE# DailyTaskAccruals SET [Field" & oTaskFieldTask.IDField & "]= NULL WHERE IDTask=" & _IDTask
                                    bolRes = ExecuteSql(strSQL)
                                End If
                        End Select

                        If bolRes Then
                            ' Eliminamos la asignacion del campo a la tarea
                            strSQL = "@DELETE# FROM FieldsTask WHERE IDTask="
                            strSQL = strSQL & oTaskFieldTask.IDTask & " AND IDField="
                            strSQL = strSQL & oTaskFieldTask.IDField
                            bolRes = ExecuteSql(strSQL)
                        End If

                    End If

                    If Not bolRes Then
                        bolRet = bolRes
                        Exit For
                    End If
                Next

                If bolRet Then
                    ' Asignamos los nuevos campos a la tarea
                    bolRes = True
                    For Each oTaskFieldTask As roTaskField In _TaskFieldsTask
                        Dim bolExistField As Boolean = False
                        For Each oActualTaskFieldTask As roTaskField In ActualTaskFieldsTask
                            If oActualTaskFieldTask.IDField = oTaskFieldTask.IDField Then
                                bolExistField = True
                                Exit For
                            End If
                        Next

                        If Not bolExistField Then
                            ' Asignamos el campo a la tarea
                            strSQL = "@INSERT# INTO FieldsTask (IDTask, IDField) VALUES ("
                            strSQL = strSQL & oTaskFieldTask.IDTask & ","
                            strSQL = strSQL & oTaskFieldTask.IDField & ")"
                            bolRes = ExecuteSql(strSQL)
                        End If

                        If bolRes = False Then
                            bolRet = bolRes
                            Exit For
                        End If
                    Next

                End If

                If bolRet Then
                    ' Asignamos los nuevos valores de los campos de la tarea
                    bolRes = True
                    For Each oActualTaskFieldTask As roTaskField In _TaskFieldsTask
                        Select Case oActualTaskFieldTask.Action
                            Case ActionTypes.aBegin, ActionTypes.aCreate
                                Dim strValue As String = ""
                                If oActualTaskFieldTask.FieldValue Is Nothing Then
                                    strValue = Nothing
                                Else
                                    Select Case oActualTaskFieldTask.Type
                                        Case FieldTypes.tText
                                            strValue = "'" & Replace(oActualTaskFieldTask.FieldValue, "'", "''") & "'"
                                        Case FieldTypes.tNumeric, FieldTypes.tDecimal
                                            If oActualTaskFieldTask.FieldValue = "" Then oActualTaskFieldTask.FieldValue = 0
                                            strValue = "convert(numeric(18,6)," & roTypes.Any2String(oActualTaskFieldTask.FieldValue).Replace(roConversions.GetDecimalDigitFormat(), ".") & ")"
                                    End Select
                                End If

                                If strValue IsNot Nothing Then
                                    strSQL = "@UPDATE# Tasks SET [Field" & oActualTaskFieldTask.IDField & "]=  " & strValue & "   WHERE ID=" & _IDTask
                                    bolRes = ExecuteSql(strSQL)
                                End If
                        End Select

                        If Not bolRes Then
                            bolRet = bolRes
                            Exit For
                        End If
                    Next
                End If

                If bolRet And _Audit Then
                    ' Auditamos modificacion asignacion de la ficha y valores
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Dim strAuditName As String = ""

                    Dim strTaskName As String = Any2String(ExecuteScalar("@SELECT# name from tasks where id=" & _IDTask))

                    Dim tbParameters As DataTable = _State.CreateAuditParameters()
                    _State.AddAuditParameter(tbParameters, "{Name}", strTaskName, "", 1)
                    _State.Audit(Audit.Action.aUpdate, Audit.ObjectType.tTask, strTaskName, tbParameters, -1)

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTaskField::SaveTaskFields")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTaskField::SaveTaskFields")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#End Region

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

    End Class

    <DataContract>
    <Serializable>
    Public Class roUserField

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roUserFieldState

        Private strFieldName As String
        Private intID As Integer
        Private oType As Types
        Private oFieldType As FieldTypes
        Private bolUsed As Boolean
        Private oAccessLevel As AccessLevels
        Private oConvertTo As ConvertibleValues
        Private strCategory As String
        Private oListValues As Generic.List(Of String)
        Private strDescription As String
        Private oAccessValidation As AccessValidation
        Private bolHistory As Boolean
        Private intRequestPermissions As Integer
        Private lstRequestsConditions As Generic.List(Of roUserFieldCondition)

        Private bolInProcess As Boolean
        Private bolInTimeGate As Boolean

        Private bSystem As Boolean
        Private strAliasFieldName As String

        Private strOriginalFieldName As String
        Private bolOriginalHistory As Boolean
        Private iDocumentTemplateId As Integer = 0
        Private bReadOnly As Boolean
        Private isUnique As Boolean

        Public Sub New()
            Me.intID = -1
            Me.oState = New roUserFieldState()
            Me.strFieldName = ""
            Me.oType = Types.EmployeeField
            Me.strOriginalFieldName = ""
            Me.bolOriginalHistory = False
            Me.strDescription = ""
            Me.intRequestPermissions = 0
            Me.bSystem = False
            Me.strAliasFieldName = ""
            Me.bReadOnly = False
            Me.isUnique = False
            Me.lstRequestsConditions = New Generic.List(Of roUserFieldCondition)
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            Me.oState = New roUserFieldState(_IDPassport)
            Me.intID = -1
            Me.strFieldName = ""
            Me.oType = Types.EmployeeField
            Me.strOriginalFieldName = ""
            Me.strDescription = ""
            Me.bolOriginalHistory = False
            Me.intRequestPermissions = 0
            Me.bSystem = False
            Me.strAliasFieldName = ""
            Me.bReadOnly = False
            Me.isUnique = False
            Me.lstRequestsConditions = New Generic.List(Of roUserFieldCondition)
        End Sub

        Public Sub New(ByVal _State As roUserFieldState, ByVal _FieldName As String, ByVal _Type As Types, ByVal _UsedInProcess As Boolean,
                       Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.strFieldName = _FieldName
            Me.oType = _Type
            Me.strDescription = ""
            Me.bolInProcess = _UsedInProcess
            Me.intRequestPermissions = 0
            Me.lstRequestsConditions = New Generic.List(Of roUserFieldCondition)
            Me.bSystem = False
            Me.strAliasFieldName = ""
            Me.intID = -1
            Me.bReadOnly = False
            Me.isUnique = False
            Me.Load(_UsedInProcess, bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roUserFieldState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roUserFieldState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property Id() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property

        <DataMember>
        Public Property FieldName() As String
            Get
                Return Me.strFieldName
            End Get
            Set(ByVal value As String)
                Me.strFieldName = value
            End Set
        End Property

        <DataMember>
        Public Property AliasFieldName() As String
            Get
                Return Me.strAliasFieldName
            End Get
            Set(ByVal value As String)
                Me.strAliasFieldName = value
            End Set
        End Property

        <DataMember>
        Public Property isSystem() As Boolean
            Get
                Return Me.bSystem
            End Get
            Set(ByVal value As Boolean)
                Me.bSystem = value
            End Set
        End Property

        <DataMember>
        Public Property FieldType() As FieldTypes
            Get
                Return Me.oFieldType
            End Get
            Set(ByVal value As FieldTypes)
                Me.oFieldType = value
            End Set
        End Property

        <DataMember>
        Public Property Used() As Boolean
            Get
                Return Me.bolUsed
            End Get
            Set(ByVal value As Boolean)
                Me.bolUsed = value
            End Set
        End Property

        <DataMember>
        Public Property AccessLevel() As AccessLevels
            Get
                Return Me.oAccessLevel
            End Get
            Set(ByVal value As AccessLevels)
                Me.oAccessLevel = value
            End Set
        End Property

        <DataMember>
        Public Property ConvertTo() As ConvertibleValues
            Get
                Return Me.oConvertTo
            End Get
            Set(ByVal value As ConvertibleValues)
                Me.oConvertTo = value
            End Set
        End Property

        <DataMember>
        Public Property OriginalFieldName() As String
            Get
                Return Me.strOriginalFieldName
            End Get
            Set(ByVal value As String)
                Me.strOriginalFieldName = value
            End Set
        End Property

        <DataMember>
        Public Property OriginalHistory() As Boolean
            Get
                Return Me.bolOriginalHistory
            End Get
            Set(ByVal value As Boolean)
                Me.bolOriginalHistory = value
            End Set
        End Property

        <DataMember>
        Public Property Category() As String
            Get
                Return Me.strCategory
            End Get
            Set(ByVal value As String)
                Me.strCategory = value
            End Set
        End Property

        <DataMember>
        Public Property ListValues() As Generic.List(Of String)
            Get
                Return Me.oListValues
            End Get
            Set(ByVal value As Generic.List(Of String))
                Me.oListValues = value
            End Set
        End Property

        <IgnoreDataMember()>
        Public Property ListValuesString() As String
            Get
                Dim strValues As String = ""
                If Me.oListValues IsNot Nothing Then
                    For Each strValue As String In Me.oListValues
                        strValues &= "~" & strValue
                    Next
                    If strValues.Length > 0 Then strValues = strValues.Substring(1)
                End If
                Return strValues
            End Get
            Set(ByVal value As String)
                Me.oListValues = New Generic.List(Of String)
                If value.Length > 0 Then
                    For Each strValue As String In value.Split("~")
                        Me.oListValues.Add(strValue)
                    Next
                End If
            End Set
        End Property

        <DataMember>
        Public Property Type() As Types
            Get
                Return Me.oType
            End Get
            Set(ByVal value As Types)
                Me.oType = value
            End Set
        End Property

        <DataMember>
        Public Property Description() As String
            Get
                Return Me.strDescription
            End Get
            Set(ByVal value As String)
                Me.strDescription = value
            End Set
        End Property

        <DataMember>
        Public Property AccessValidation() As AccessValidation
            Get
                Return Me.oAccessValidation
            End Get
            Set(ByVal value As AccessValidation)
                Me.oAccessValidation = value
            End Set
        End Property

        <DataMember>
        Public Property History() As Boolean
            Get
                Return Me.bolHistory
            End Get
            Set(ByVal value As Boolean)
                Me.bolHistory = value
            End Set
        End Property

        <DataMember>
        Public Property InProcess() As Boolean
            Get
                Return Me.bolInProcess
            End Get
            Set(ByVal value As Boolean)
                Me.bolInProcess = value
            End Set
        End Property

        <DataMember>
        Public Property InTimeGate() As Boolean
            Get
                Return Me.bolInTimeGate
            End Get
            Set(ByVal value As Boolean)
                Me.bolInTimeGate = value
            End Set
        End Property

        <DataMember>
        Public Property RequestPermissions() As Integer
            Get
                Return Me.intRequestPermissions
            End Get
            Set(ByVal value As Integer)
                Me.intRequestPermissions = value
            End Set
        End Property

        <DataMember>
        Public Property RequestConditions() As Generic.List(Of roUserFieldCondition)
            Get
                Return Me.lstRequestsConditions
            End Get
            Set(ByVal value As Generic.List(Of roUserFieldCondition))
                Me.lstRequestsConditions = value
            End Set
        End Property

        <DataMember>
        Public Property DocumentTemplateId As Integer
            Get
                Return Me.iDocumentTemplateId
            End Get
            Set(value As Integer)
                Me.iDocumentTemplateId = value
            End Set
        End Property

        <DataMember>
        Public Property [ReadOnly] As Boolean
            Get
                Return Me.bReadOnly
            End Get
            Set(value As Boolean)
                Me.bReadOnly = value
            End Set
        End Property

        <DataMember>
        Public Property [Unique] As Boolean
            Get
                Return Me.isUnique
            End Get
            Set(value As Boolean)
                Me.isUnique = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(ByVal _CheckUsedInProcess As Boolean, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.strFieldName <> String.Empty Then
                    Me.strOriginalFieldName = Me.strFieldName

                    Dim strSQL As String = "@SELECT# * FROM sysroUserFields WHERE (ISNULL(Alias,FieldName) = '" & Me.strOriginalFieldName & "' OR FieldName = '" & Me.strOriginalFieldName & "') AND [Type] = " & Me.oType
                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb.Rows.Count = 1 Then

                        Dim oRow As DataRow = tb.Rows(0)

                        If IsDBNull(oRow("FieldType")) Then
                            Me.oFieldType = FieldTypes.tText
                        Else
                            Me.oFieldType = oRow("FieldType")
                        End If
                        Me.bolUsed = oRow("Used")
                        Me.intID = oRow("ID")
                        If IsDBNull(oRow("AccessLevel")) Then
                            Me.oAccessLevel = AccessLevels.aMedium
                        Else
                            Me.oAccessLevel = oRow("AccessLevel")
                        End If
                        Me.strCategory = roTypes.Any2String(oRow("Category"))
                        Me.ListValuesString = roTypes.Any2String(oRow("ListValues"))
                        Me.strDescription = Any2String(oRow("Description"))
                        Me.oAccessValidation = Any2Integer(oRow("AccessValidation"))
                        Me.bolHistory = Any2Boolean(oRow("History"))
                        Me.bolOriginalHistory = Me.bolHistory
                        Me.intRequestPermissions = Any2Integer(oRow("RequestPermissions"))
                        Me.bSystem = Any2Boolean(oRow("isSystem"))
                        Me.strAliasFieldName = Any2String(oRow("Alias"))

                        If IsDBNull(oRow("ReadOnly")) Then
                            Me.bReadOnly = False
                        Else
                            Me.bReadOnly = roTypes.Any2Boolean(oRow("ReadOnly"))
                        End If

                        Me.isUnique = roTypes.Any2Boolean(oRow("Unique"))

                        Dim oUserFieldState As New roUserFieldState
                        roBusinessState.CopyTo(Me.oState, oUserFieldState)
                        Me.lstRequestsConditions = roUserFieldCondition.LoadFromXml(Any2String(oRow("RequestCriteria")), oUserFieldState)
                        roBusinessState.CopyTo(oUserFieldState, Me.oState)

                        If _CheckUsedInProcess Then
                            Me.bolInProcess = UsedInProcess(Me.strOriginalFieldName, Me.oType, Me.oState)
                        End If

                        bolRet = True

                        ' Auditar lectura
                        If bAudit Then
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            oState.AddAuditParameter(tbParameters, "{UserFieldName}", Me.strFieldName, "", 1)
                            Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tUserField, Me.strFieldName, tbParameters, -1)
                        End If

                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roUserField::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roUserField::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim oAuditDataOld As DataRow = Nothing
            Dim oAuditDataNew As DataRow = Nothing

            Dim bHaveToClose As Boolean = False
            Try

                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = UserFieldResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim bolIsNew As Boolean = False

                If Me.Validate() Then
                    Dim tb As New DataTable("sysroUserFields")
                    'Dim strSQL As String = "@SELECT# * FROM sysroUserFields WHERE (ISNULL(Alias,FieldName) = '" & Me.strOriginalFieldName & "' OR FieldName = '" & Me.strOriginalFieldName & "') AND [Type] = " & Me.oType
                    Dim strSQL As String = "@SELECT# * FROM sysroUserFields WHERE ID = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        bolIsNew = True
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("FieldName") = Me.strFieldName
                    oRow("Type") = Me.oType
                    oRow("FieldType") = Me.oFieldType
                    oRow("Used") = Me.bolUsed
                    oRow("AccessLevel") = Me.oAccessLevel
                    oRow("Category") = Me.strCategory
                    oRow("ListValues") = Me.ListValuesString
                    oRow("Description") = Me.strDescription
                    oRow("AccessValidation") = Me.oAccessValidation
                    oRow("History") = Me.bolHistory
                    oRow("RequestPermissions") = Me.intRequestPermissions
                    oRow("RequestCriteria") = roUserFieldCondition.GetXml(Me.lstRequestsConditions)
                    oRow("ReadOnly") = Me.bReadOnly

                    oRow("Unique") = Me.isUnique

                    If Me.strAliasFieldName = String.Empty Then
                        oRow("Alias") = DBNull.Value
                    Else
                        oRow("Alias") = Me.strAliasFieldName
                    End If

                    oRow("isSystem") = Me.bSystem

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    oAuditDataNew = oRow

                    Dim bolExistField As Boolean = True

                    If Me.oType = Types.EmployeeField Then

                        If Me.strOriginalFieldName <> Me.strFieldName Then
                            ' Renombramos los valores del histórico del campo de la ficha para todos los empleados
                            Dim oEmployeeState As New roUserFieldState : roBusinessState.CopyTo(Me.oState, oEmployeeState)
                            bolRet = roEmployeeUserField.RenameUserFieldHistoryAllEmployees(Me.strOriginalFieldName, Me.strFieldName, oEmployeeState)
                        End If

                    ElseIf Me.oType = Types.GroupField Then

                        ' Miro si existe el campo en la tabla de empleados/grupos
                        Try
                            strSQL = "@SELECT# [USR_" & Me.strFieldName & "] FROM Groups"
                            Dim tbTest As DataTable = CreateDataTable(strSQL)
                        Catch
                            bolExistField = False
                        End Try

                        bolRet = True

                        If Me.bolUsed Then 'If Me.bolUsed And Not bolExistField Then

                            If Not bolExistField Then
                                strSQL = " @ALTER# TABLE Groups ADD [USR_" & Me.strFieldName & "] "
                                Select Case Me.oFieldType
                                    Case FieldTypes.tText
                                        strSQL &= " Text NULL "
                                    Case FieldTypes.tNumeric
                                        strSQL &= " Int NULL "
                                    Case FieldTypes.tDate
                                        strSQL &= " DateTime NULL "
                                    Case FieldTypes.tDecimal
                                        strSQL &= " Numeric(16,6) NULL "
                                    Case FieldTypes.tList
                                        strSQL &= " Text NULL "
                                    Case FieldTypes.tDatePeriod
                                        strSQL &= " nvarchar(21) NULL " ' yyyy/MM/dd*yyyy/MM/dd
                                    Case FieldTypes.tTimePeriod
                                        strSQL &= " nvarchar(11) NULL " ' HH:mm*HH:mm
                                    Case FieldTypes.tTime
                                        'strSQL &= " DateTime NULL "
                                        strSQL &= " float NULL " ' HHHH:mm:ss
                                    Case FieldTypes.tLink
                                        strSQL &= " Text NULL "
                                    Case FieldTypes.tDocument
                                        strSQL &= " Text NULL "
                                    Case Else
                                        oState.Result = UserFieldResultEnum.InvalidUserdFieldType
                                End Select
                                bolRet = ExecuteSql(strSQL)
                            End If

                            If Not bolIsNew AndAlso Me.strOriginalFieldName <> Me.strFieldName Then
                                ' Pasamos los valores del campo anterior al nuevo
                                strSQL = "@UPDATE# Groups SET [USR_" & Me.strFieldName & "] = [USR_" & Me.strOriginalFieldName & "]"
                                bolRet = ExecuteSql(strSQL)
                                ' Borramos el campo anterior
                                strSQL = "@ALTER# TABLE Groups @DROP# COLUMN [USR_" & Me.strOriginalFieldName & "]"
                                bolRet = ExecuteSql(strSQL)
                            End If

                        ElseIf Not Me.bolUsed AndAlso bolExistField Then
                            strSQL = "@ALTER# TABLE Groups @DROP# COLUMN [USR_" & Me.strFieldName & "]"
                            bolRet = ExecuteSql(strSQL)

                        End If

                    End If

                    If bolRet AndAlso bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("FieldName")
                        Else
                            strObjectName = oAuditDataOld("FieldName") & " -> " & oAuditDataNew("FieldName")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tUserField, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roUserField::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roUserField::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = False
            Dim strSQL As String = String.Empty

            Try

                bolRet = True

                ' No permitiom desasignar el campo de la ficha si se usa en algún proceso de recálculo y
                ' tampoco dejamos modificar el nombre del campo si está asignado a la ficha y se utiliza en algún proceso de recálculo.
                If Not Me.Used AndAlso UsedInProcess(Me.strOriginalFieldName, Me.oType, Me.oState) OrElse
                   (Me.strOriginalFieldName <> "" AndAlso Me.Used AndAlso Me.strOriginalFieldName <> Me.strFieldName AndAlso UsedInProcess(Me.strOriginalFieldName, Me.oType, Me.oState)) Then
                    bolRet = False
                    Me.oState.Result = UserFieldResultEnum.UserFieldUsedInProcess
                End If

                ' No permite desasignar el historico si algún empleado ya tiene asignado valores
                If bolRet AndAlso (Me.bolOriginalHistory <> Me.bolHistory) AndAlso Not Me.bolHistory AndAlso Me.oType = Types.EmployeeField Then
                    strSQL = $"@SELECT# count(*) as Total FROM EmployeeUserFieldValues WHERE FieldName = '{Me.strOriginalFieldName}' AND Date <> '19000101'"
                    If Any2Double(ExecuteScalar(strSQL)) > 0 Then
                        bolRet = False
                        Me.oState.Result = UserFieldResultEnum.HistoryValues
                    End If
                End If

                ' No permite que un campo se llame como un alias ya definido
                If bolRet AndAlso Me.strAliasFieldName <> String.Empty Then
                    strSQL = $"@SELECT# count(*) as Total FROM sysroUserFields WHERE FieldName = '{Me.strAliasFieldName}' "
                    If Any2Double(ExecuteScalar(strSQL)) > 0 Then
                        bolRet = False
                        Me.oState.Result = UserFieldResultEnum.UserFieldNamePrivate
                    End If
                End If

                ' No permite que se duplique el nombre de un campo
                If bolRet AndAlso Me.strFieldName <> String.Empty Then
                    strSQL = $"@SELECT# count(*) as Total FROM sysroUserFields WHERE FieldName = '{Me.strFieldName}' and [Type] = {CInt(Me.oType)} AND ID <> {Me.intID}"

                    If Any2Double(ExecuteScalar(strSQL)) > 0 Then
                        bolRet = False
                        Me.oState.Result = UserFieldResultEnum.UserFieldNameDuplicated
                    End If
                End If

                ' No permite que se utilicen comas en un campo
                If bolRet AndAlso Me.strFieldName <> String.Empty AndAlso Me.strFieldName.Contains(",") Then
                    bolRet = False
                    Me.oState.Result = UserFieldResultEnum.InvalidName
                End If

                ' No permite desasignar un campo que se está usando como identificador para fichaje en Time Gate
                If bolRet AndAlso Me.Unique AndAlso Not Me.Used Then
                    Dim parameter As roAdvancedParameter = New roAdvancedParameter("Timegate.Identification.CustomUserFieldId", New roAdvancedParameterState(oState.IDPassport), False)
                    If parameter IsNot Nothing AndAlso parameter.Value.Trim.Length > 0 Then
                        Dim timegateConfiguration As TimegateConfiguration
                        timegateConfiguration = roJSONHelper.DeserializeNewtonSoft(parameter.Value, GetType(TimegateConfiguration))
                        If timegateConfiguration IsNot Nothing AndAlso timegateConfiguration.UserFieldId = Me.Id AndAlso Me.Id > -1 Then
                            bolRet = False
                            Me.oState.Result = UserFieldResultEnum.UserFieldUsedAsIdForTimeGate
                        End If
                    End If
                End If

                ' Solo se puede marcar como únicos los campos de tipo texto y los de tipo numérico
                If bolRet AndAlso Me.isUnique AndAlso Me.FieldType <> FieldTypes.tText AndAlso Me.FieldType <> FieldTypes.tNumeric Then
                    bolRet = False
                    Me.oState.Result = UserFieldResultEnum.InvalidUniqueFieldType
                End If

            Catch ex As DbException
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roUserField::Validate")
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roUserField::Validate")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                If Not Me.Used OrElse Not UsedInProcess(Me.strOriginalFieldName, Me.oType, Me.oState) Then

                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    Dim cmd As DbCommand = Nothing
                    Dim strSQL As String

                    bolRet = True

                    If Me.oType = Types.EmployeeField Then
                        ' Borramos todos los valores de histórico de este campo para todos los empleados
                        Dim oEmployeeState As New roUserFieldState : roBusinessState.CopyTo(Me.oState, oEmployeeState)
                        bolRet = roEmployeeUserField.DeleteUserFieldHistoryAllEmployees(Me.strFieldName, oEmployeeState)
                    End If

                    If bolRet Then
                        strSQL = "@DELETE# FROM sysroUserFields WHERE FieldName = @FieldName AND [Type] = " & Me.oType
                        cmd = CreateCommand(strSQL)
                        AddParameter(cmd, "@FieldName", DbType.String, 50)
                        cmd.Parameters("@FieldName").Value = Me.strFieldName
                        cmd.ExecuteNonQuery()
                    End If

                    If bolRet And Me.oType = Types.GroupField Then
                        If Me.ExistFieldInTable() Then
                            strSQL = "@ALTER# TABLE " & Me.TableName & " @DROP# COLUMN [USR_" & Me.strFieldName & "]"
                            bolRet = ExecuteSql(strSQL)
                        End If
                    End If

                    If bolRet Then

                        ' Miramos si hay la licencia de prevención de riesgos laborales
                        Dim oServerLicense As New roServerLicense
                        If oServerLicense.FeatureIsInstalled("Feature\OHP") Then

                            ' Miramos si es necesario ejecutar el broadcaster
                            If Me.Used And Me.AccessValidation <> AccessValidation.None Then
                                roConnector.InitTask(TasksType.BROADCASTER)
                            End If

                        End If

                    End If

                    If bolRet And bAudit Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tUserField, Me.strFieldName, Nothing, -1)
                    End If
                Else
                    bolRet = False
                    Me.oState.Result = UserFieldResultEnum.UserFieldUsedInProcess
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roUserField::Delete")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roUserField::Delete")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function ExistFieldInTable() As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim strSQL As String = "@SELECT# [USR_" & Me.strFieldName & "] FROM Groups"
                Dim tbTest As DataTable = CreateDataTable(strSQL, )
            Catch
                bolRet = False
            Finally

            End Try

            Return bolRet

        End Function

        Private Function TableName() As String

            Dim strTable As String = "EmployeeUserFieldValues"
            Select Case Me.oType
                Case Types.EmployeeField
                    strTable = "EmployeeUserFieldValues"
                Case Types.GroupField
                    strTable = "GroupUserFieldValues"
            End Select

            Return strTable

        End Function

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oState Is Nothing Then
                Me.oState = New roUserFieldState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
            End If
        End Sub

#Region "Helper methods"

        Public Shared Function GetUserFields(ByVal _Type As Types, ByVal _State As roUserFieldState, Optional ByVal strWhere As String = "",
                                             Optional ByVal bAudit As Boolean = False, Optional ByVal bolCheckUsedInProcess As Boolean = True, Optional ByVal bolIsEmergencyReport As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# *, FieldName AS OriginalFieldName, 0 AS UsedInProcess, 0 AS UsedInTimeGate FROM sysroUserFields " &
                                       "WHERE ISNULL(Type, 0) = " & _Type
                If strWhere <> "" Then strSQL &= " AND (" & strWhere & ")"
                If bolIsEmergencyReport.Equals(True) Then strSQL &= " AND ((Alias NOT LIKE 'sysroEmergencyNumber') or (Alias is null))"

                strSQL &= " ORDER BY FieldName ASC"

                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then

                    ' Verificamos seguridad
                    Dim bolLowAccess As Boolean = True
                    Dim bolMediumAccess As Boolean = True
                    Dim bolHighAccess As Boolean = True
                    If _State.IDPassport > 0 Then
                        bolLowAccess = WLHelper.HasFeaturePermission(_State.IDPassport, "Employees.UserFields.Information.Low", Permission.Read)
                        bolMediumAccess = WLHelper.HasFeaturePermission(_State.IDPassport, "Employees.UserFields.Information.Medium", Permission.Read)
                        bolHighAccess = WLHelper.HasFeaturePermission(_State.IDPassport, "Employees.UserFields.Information.High", Permission.Read)
                    End If

                    For Each oRow As DataRow In oRet.Rows
                        If (Any2Integer(oRow("AccessLevel")) = AccessLevels.aLow And Not bolLowAccess) Or
                           (Any2Integer(oRow("AccessLevel")) = AccessLevels.aMedium And Not bolMediumAccess) Or
                           (Any2Integer(oRow("AccessLevel")) = AccessLevels.aHigh And Not bolHighAccess) Then
                            oRow.Delete()
                        End If
                    Next

                    oRet.AcceptChanges()

                    ' Actualizamos la columna 'UsedInProcess' para indicar si los campos se utilizan en algún proceso.
                    If bolCheckUsedInProcess Then SetUsedInProcess(oRet, _State)

                    ' Actualizamos la columna 'ReadOnly' si el campo no permite cambio de nombre.
                    ' TODO: Se podría hacer para todos. Porque ahora, los que no se usan en cálculos, permiten cambiar el nombre, pero lo que hace realmente es crear uno nuevo !!! (efecto dereivado de que el id de la tabla sea el nombre ...)
                    Dim parameter As roAdvancedParameter = New roAdvancedParameter("Timegate.Identification.CustomUserFieldId", New roAdvancedParameterState(-1), False)
                    If Any2String(parameter.Value).Trim.Length > 0 Then
                        Dim timegateConfiguration As TimegateConfiguration
                        timegateConfiguration = roJSONHelper.DeserializeNewtonSoft(parameter.Value, GetType(TimegateConfiguration))
                        If timegateConfiguration IsNot Nothing Then
                            ' Suponiendo que tienes un DataTable llamado "miDataTable"
                            Dim rows As DataRow() = oRet.Select($"ID = {timegateConfiguration.UserFieldId}")
                            If rows.Length > 0 Then
                                rows(0)("UsedInTimeGate") = True
                            End If
                        End If
                    End If

                    oRet.AcceptChanges()

                    If bAudit Then
                        ' Auditamos consulta masiva
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Dim strAuditName As String = ""
                        For Each oRow As DataRow In oRet.Rows
                            strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("FieldName")
                        Next
                        Extensions.roAudit.AddParameter(tbAuditParameters, "{UserFieldNames}", strAuditName, "", 1)
                        _State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tUserField, strAuditName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFields")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFields")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetTaskFields(ByVal _Type As Types, ByVal _State As roUserFieldState, Optional ByVal strWhere As String = "",
                                             Optional ByVal bAudit As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# ID, Name, Type , Action, TypeValue, ListValues  FROM sysroFieldsTask "
                If strWhere <> "" Then strSQL &= " AND (" & strWhere & ")"
                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then

                    If bAudit Then
                        '' Auditamos consulta masiva
                        'Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                        'Dim strAuditName As String = ""
                        'For Each oRow As DataRow In oRet.Rows
                        '    strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("FieldName")
                        'Next
                        'Audit.roAudit.AddParameter(tbAuditParameters, "{UserFieldNames}", strAuditName, "", 1)
                        '_State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tUserField, strAuditName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetTaskFields")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetTaskFields")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetBusinessCenterFields(ByVal _Type As Types, ByVal _State As roUserFieldState, Optional ByVal strWhere As String = "",
                                            Optional ByVal bAudit As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# ID, Name  FROM sysroFieldsBusinessCenters "
                If strWhere <> "" Then strSQL &= " AND (" & strWhere & ")"
                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then

                    If bAudit Then
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetBusinessCenterFields")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetBusinessCenterFields")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetUserFieldsList(ByVal _Type As Types, ByVal bolCheckInProcess As Boolean, ByVal _State As roUserFieldState, Optional ByVal strWhere As String = "", Optional ByVal bolAudit As Boolean = True) As Generic.List(Of roUserField)

            Dim oRet As New Generic.List(Of roUserField)

            Try

                ' Obtenemos los permisos de acceso a la información de la ficha para el passpor actual en función del nivel de acceso para el empleado indicado.
                Dim bolLowAccess As Boolean = True
                Dim bolMediumAccess As Boolean = True
                Dim bolHighAccess As Boolean = True
                Select Case _State.ActivePassportType()
                    Case "U"
                        bolLowAccess = WLHelper.HasFeaturePermission(_State.IDPassport, "Employees.UserFields.Information.Low", Permission.Read, )
                        bolMediumAccess = WLHelper.HasFeaturePermission(_State.IDPassport, "Employees.UserFields.Information.Medium", Permission.Read, )
                        bolHighAccess = WLHelper.HasFeaturePermission(_State.IDPassport, "Employees.UserFields.Information.High", Permission.Read, )
                    Case "E"
                        bolLowAccess = WLHelper.HasFeaturePermission(_State.IDPassport, "UserFields.Query", Permission.Read, "E")
                        bolMediumAccess = bolLowAccess
                        bolHighAccess = bolLowAccess
                End Select

                Dim strSQL As String = "@SELECT# *, FieldName AS OriginalFieldName FROM sysroUserFields " &
                                       "WHERE ISNULL(Type, 0) = " & _Type
                If strWhere <> "" Then strSQL &= " AND (" & strWhere & ")"
                strSQL = $"{strSQL} ORDER BY FieldName ASC"

                Dim tbFields As DataTable = CreateDataTable(strSQL, )

                If tbFields IsNot Nothing AndAlso tbFields.Rows.Count > 0 Then
                    Dim oAccessLevel As AccessLevels
                    For Each oRow As DataRow In tbFields.Rows
                        If Not IsDBNull(oRow("AccessLevel")) Then
                            oAccessLevel = oRow("AccessLevel")
                        Else
                            oAccessLevel = AccessLevels.aMedium
                        End If
                        If (oAccessLevel = AccessLevels.aLow And bolLowAccess) Or
                           (oAccessLevel = AccessLevels.aMedium And bolMediumAccess) Or
                           (oAccessLevel = AccessLevels.aHigh And bolHighAccess) Then
                            oRet.Add(New roUserField(_State, oRow("FieldName"), oRow("Type"), False))
                        End If
                    Next

                    ' Informamos por cada campo de la ficha si se utiliza en algún proceso
                    If bolCheckInProcess Then roUserField.SetUsedInProcess(oRet, _State)
                    If bolAudit Then
                        ' Auditamos consulta masiva
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Dim strAuditName As String = ""
                        For Each oRow As DataRow In tbFields.Rows
                            strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("FieldName")
                        Next
                        Extensions.roAudit.AddParameter(tbAuditParameters, "{UserFieldNames}", strAuditName, "", 1)
                        _State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tUserField, strAuditName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetUserFieldsListWithType(ByVal _Type As Types, ByVal _IDEmployee As String, ByVal _State As roUserFieldState, Optional ByVal strWhere As String = "") As Generic.List(Of String)

            Dim oRet As New Generic.List(Of String)

            Try

                ' Obtenemos los permisos de acceso a la información de la ficha para el passpor actual en función del nivel de acceso para el empleado indicado.
                Dim bolLowAccess As Boolean = True
                Dim bolMediumAccess As Boolean = True
                Dim bolHighAccess As Boolean = True
                Select Case _State.ActivePassportType()
                    Case "U"
                        bolLowAccess = WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Employees.UserFields.Information.Low", Permission.Read, _IDEmployee, )
                        bolMediumAccess = WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Employees.UserFields.Information.Medium", Permission.Read, _IDEmployee, )
                        bolHighAccess = WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Employees.UserFields.Information.High", Permission.Read, _IDEmployee, )
                    Case "E"
                        bolLowAccess = WLHelper.HasFeaturePermission(_State.IDPassport, "UserFields.Query", Permission.Read, "E")
                        bolMediumAccess = bolLowAccess
                        bolHighAccess = bolLowAccess
                End Select

                Dim strSQL As String = "@SELECT# FieldName, AccessLevel FROM sysroUserFields " &
                                       "WHERE ISNULL(Type, 0) = " & _Type
                If strWhere <> "" Then strSQL &= " AND (" & strWhere & ")"

                Dim tbUserFields As DataTable = CreateDataTable(strSQL, )
                If tbUserFields IsNot Nothing Then
                    Dim oAccessLevel As AccessLevels
                    For Each oRow As DataRow In tbUserFields.Rows
                        If Not IsDBNull(oRow("AccessLevel")) Then
                            oAccessLevel = oRow("AccessLevel")
                        Else
                            oAccessLevel = AccessLevels.aMedium
                        End If
                        If (oAccessLevel = AccessLevels.aLow And bolLowAccess) Or
                           (oAccessLevel = AccessLevels.aMedium And bolMediumAccess) Or
                           (oAccessLevel = AccessLevels.aHigh And bolHighAccess) Then
                            oRet.Add(oRow("FieldName"))
                        End If
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUSerFieldsListWithType")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUSerFieldsListWithType")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveUserFields(ByVal tbUserFields As DataTable, ByVal _State As roUserFieldState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oUserField As roUserField
                Dim bolSaved As Boolean = True

                bolRet = True

                For Each oRow As DataRow In tbUserFields.Rows

                    bolSaved = True

                    Select Case oRow.RowState
                        Case DataRowState.Added, DataRowState.Modified

                            oUserField = New roUserField(_State, oRow("FieldName"), oRow("Type"), False, False)
                            With oUserField
                                .FieldType = oRow("FieldType")
                                .AccessLevel = IIf(Not IsDBNull(oRow("AccessLevel")), oRow("AccessLevel"), AccessLevels.aMedium)
                                .Used = oRow("Used")
                                .Unique = oRow("Unique")
                                .Category = roTypes.Any2String(oRow("Category"))
                                .ListValuesString = roTypes.Any2String(oRow("ListValues"))
                                .Type = Any2Integer(oRow("Type"))
                                .Description = Any2String(oRow("Description"))
                                .AccessValidation = Any2Integer(oRow("AccessValidation"))
                                .History = Any2Boolean(oRow("History"))
                                .RequestPermissions = Any2Integer(oRow("RequestPermissions"))
                                .RequestConditions = roUserFieldCondition.LoadFromXml(Any2String(oRow("RequestCriteria")), _State)
                                If Not IsDBNull(oRow("OriginalFieldName")) Then .OriginalFieldName = oRow("OriginalFieldName")
                                If Not IsDBNull(oRow("Alias")) Then .AliasFieldName = oRow("Alias")
                                .bSystem = Any2Boolean(oRow("isSystem"))
                            End With
                            bolRet = oUserField.Save(bAudit)

                        Case DataRowState.Deleted
                            oRow.RejectChanges() ' Cmabiar el estado de la fila para poder leer sus datos
                            oUserField = New roUserField(_State, oRow("FieldName"), oRow("Type"), False)
                            bolRet = oUserField.Delete(bAudit)

                        Case Else
                            bolRet = True
                            bolSaved = False

                    End Select

                    If Not bolRet Then

                        Exit For
                    End If

                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::SaveUserFields")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::SaveUserFields")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function GetUserFieldValues(ByVal strUserField As String, ByVal _Type As Types, ByVal xDate As Date, ByVal strParent As String, ByVal strSeparator As String, ByVal strFieldWhere As String, ByVal _State As roUserFieldState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                If _Type = Types.EmployeeField Then

                    Dim oEmployeeState As New roUserFieldState(_State.IDPassport, _State.Context, _State.ClientAddress)
                    oRet = roEmployeeUserField.GetUserFieldValues(strUserField, xDate, strParent, strSeparator, strFieldWhere, oEmployeeState)
                    _State.Result = oEmployeeState.Result
                    _State.ErrorText = oEmployeeState.ErrorText
                    _State.ErrorNumber = oEmployeeState.ErrorNumber

                ElseIf _Type = Types.GroupField Then

                    Dim oGroupState As New roUserFieldState(_State.IDPassport, _State.Context, _State.ClientAddress)
                    oRet = roGroupUserField.GetUserFieldValues(strUserField, xDate, strParent, strSeparator, strFieldWhere, oGroupState)
                    _State.Result = oGroupState.Result
                    _State.ErrorText = oGroupState.ErrorText
                    _State.ErrorNumber = oGroupState.ErrorNumber

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldValues")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldValues")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCategories(ByVal bolOnlyUsed As Boolean, ByVal _State As roUserFieldState) As Generic.List(Of String)

            Dim oRet As New Generic.List(Of String)

            Try

                Dim strSQL As String = "@SELECT# DISTINCT Category FROM sysroUserFields WHERE ISNULL(Category, '') <> ''"
                If bolOnlyUsed Then strSQL &= " AND Used = 1 "
                strSQL &= " ORDER BY Category"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        If Not IsDBNull(oRow("Category")) Then
                            oRet.Add(oRow("Category"))
                        End If
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetCategories")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetCategories")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Actualiza la propiedad 'Used' la lista de la ficha, en función de si se utilizan en algún valor inicial o alguna regla de convenio o en alguna composición de saldo.
        ''' </summary>
        ''' <param name="UserFields"></param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <remarks></remarks>
        Public Shared Sub SetUsedInProcess(ByVal UserFields As Generic.List(Of roUserField), ByVal _State As roUserFieldState)

            Dim UserFieldNamesUsed As Generic.List(Of String) = Nothing

            UserFieldNamesUsed = roUserField.GetUserFieldsUsedInStartupValues(_State)
            If UserFieldNamesUsed IsNot Nothing Then
                For Each strUserFieldName As String In UserFieldNamesUsed
                    For Each oUserField As roUserField In UserFields
                        If strUserFieldName.ToLower = oUserField.FieldName.ToLower Then
                            oUserField.Used = True
                        End If
                    Next
                Next
            End If

            UserFieldNamesUsed = roUserField.GetUserFieldsUsedInLabAgreeRules(_State)
            If UserFieldNamesUsed IsNot Nothing Then
                For Each strUserFieldName As String In UserFieldNamesUsed
                    For Each oUserField As roUserField In UserFields
                        If strUserFieldName.ToLower = oUserField.FieldName.ToLower Then
                            oUserField.Used = True
                        End If
                    Next
                Next
            End If

            UserFieldNamesUsed = roUserField.GetUserFieldsUsedInCauseLimitValues(_State)
            If UserFieldNamesUsed IsNot Nothing Then
                For Each strUserFieldName As String In UserFieldNamesUsed
                    For Each oUserField As roUserField In UserFields
                        If strUserFieldName.ToLower = oUserField.FieldName.ToLower Then
                            oUserField.Used = True
                        End If
                    Next
                Next
            End If

            UserFieldNamesUsed = roUserField.GetUserFieldsUsedInConcepts(_State)
            If UserFieldNamesUsed IsNot Nothing Then
                For Each strUserFieldName As String In UserFieldNamesUsed
                    For Each oUserField As roUserField In UserFields
                        If strUserFieldName.ToLower = oUserField.FieldName.ToLower Then
                            oUserField.Used = True
                        End If
                    Next
                Next
            End If

            UserFieldNamesUsed = roUserField.GetUserFieldsUsedInShifts(_State)
            If UserFieldNamesUsed IsNot Nothing Then
                For Each strUserFieldName As String In UserFieldNamesUsed
                    For Each oUserField As roUserField In UserFields
                        If strUserFieldName.ToLower = oUserField.FieldName.ToLower Then
                            oUserField.Used = True
                        End If
                    Next
                Next
            End If

        End Sub

        ''' <summary>
        ''' Actualiza la columnas 'UsedInProcess' de la tabla de la ficha, en función de si se utilizan en algún valor inicial o alguna regla de convenio o en alguna composición de saldo.
        ''' </summary>
        ''' <param name="UserFieldsDataTable">Tabla con los campos de la ficha.</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <remarks></remarks>
        Public Shared Sub SetUsedInProcess(ByVal UserFieldsDataTable As DataTable, ByVal _State As roUserFieldState)

            Dim UserFieldNamesUsed As Generic.List(Of String) = Nothing

            Dim oLabAgreeState As New UserFields.roUserFieldState
            roBusinessState.CopyTo(_State, oLabAgreeState)

            UserFieldNamesUsed = roUserField.GetUserFieldsUsedInStartupValues(oLabAgreeState)
            If UserFieldNamesUsed IsNot Nothing Then
                For Each strUserFieldName As String In UserFieldNamesUsed
                    For Each oRow As DataRow In UserFieldsDataTable.Rows
                        If strUserFieldName.ToLower = Any2String(oRow("FieldName")).ToLower Then
                            oRow("UsedInProcess") = True
                        End If
                    Next
                Next
            End If

            UserFieldNamesUsed = roUserField.GetUserFieldsUsedInLabAgreeRules(oLabAgreeState)
            If UserFieldNamesUsed IsNot Nothing Then
                For Each strUserFieldName As String In UserFieldNamesUsed
                    For Each oRow As DataRow In UserFieldsDataTable.Rows
                        If strUserFieldName.ToLower = Any2String(oRow("FieldName")).ToLower Then
                            oRow("UsedInProcess") = True
                        End If
                    Next
                Next
            End If

            UserFieldNamesUsed = roUserField.GetUserFieldsUsedInCauseLimitValues(oLabAgreeState)
            If UserFieldNamesUsed IsNot Nothing Then
                For Each strUserFieldName As String In UserFieldNamesUsed
                    For Each oRow As DataRow In UserFieldsDataTable.Rows
                        If strUserFieldName.ToLower = Any2String(oRow("FieldName")).ToLower Then
                            oRow("UsedInProcess") = True
                        End If
                    Next
                Next
            End If

            UserFieldNamesUsed = roUserField.GetUserFieldsUsedInConcepts(oLabAgreeState)
            If UserFieldNamesUsed IsNot Nothing Then
                For Each strUserFieldName As String In UserFieldNamesUsed
                    For Each oRow As DataRow In UserFieldsDataTable.Rows
                        If strUserFieldName.ToLower = Any2String(oRow("FieldName")).ToLower Then
                            oRow("UsedInProcess") = True
                        End If
                    Next
                Next
            End If

            UserFieldNamesUsed = roUserField.GetUserFieldsUsedInShifts(oLabAgreeState)
            If UserFieldNamesUsed IsNot Nothing Then
                For Each strUserFieldName As String In UserFieldNamesUsed
                    For Each oRow As DataRow In UserFieldsDataTable.Rows
                        If strUserFieldName.ToLower = Any2String(oRow("FieldName")).ToLower Then
                            oRow("UsedInProcess") = True
                        End If
                    Next
                Next
            End If

        End Sub

        ''' <summary>
        ''' Indica si el campo de la ficha se está utilizando en algún proceso. (Actualmente sólo en valores iniciales)
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function UsedInProcess(ByVal strFieldName As String, ByVal oType As Types, ByVal _State As roUserFieldState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oServerLicense As New roServerLicense

                If oType = Types.EmployeeField Then
                    ' Miramos si el campo se està utilizando en algún valor inicial de algún convenio asignado al empleado
                    Dim dTblStartValues As DataTable = roUserField.GetUserFieldUsedInStartupValues(strFieldName, _State)
                    If dTblStartValues.Rows.Count > 0 Then
                        bolRet = True
                    End If

                    If Not bolRet Then
                        ' Miramos si el campo se està utilizando en alguna regla de convenio asignada al empleado
                        Dim dTblCauseLimitValues As DataTable = roUserField.GetUserFieldUsedInCauseLimitValues(strFieldName, _State)
                        If dTblCauseLimitValues.Rows.Count > 0 Then
                            bolRet = True
                        End If
                    End If

                    If Not bolRet Then
                        ' Miramos si el campo se està utilizando en alguna regla de convenio asignada al empleado
                        Dim dTblLabAgreeRules As DataTable = roUserField.GetUserFieldUsedInLabAgreeRules(strFieldName, _State)
                        If dTblLabAgreeRules.Rows.Count > 0 Then
                            bolRet = True
                        End If
                    End If

                    If Not bolRet Then

                        ' Miramos si el campo se està utilizando en algún saldo
                        Dim tbConcepts As DataTable = roUserField.GetUserFieldUsedInConcepts(strFieldName, _State)
                        If tbConcepts IsNot Nothing Then
                            bolRet = (tbConcepts.Rows.Count > 0)
                        End If

                    End If

                    If Not bolRet Then
                        ' miramos si el campo se està utilizando en algún horario
                        Dim tbshifts As DataTable = roUserField.GetUserFieldUsedInShifts(strFieldName, _State)
                        If tbshifts IsNot Nothing Then
                            bolRet = (tbshifts.Rows.Count > 0)
                        End If
                    End If

                    If Not bolRet Then
                        ' miramos si el campo se està utilizando en algún colectivo
                        bolRet = roUserField.CheckIfUserFieldIsUsedInCollectives(strFieldName, _State)
                    End If

                    If Not bolRet AndAlso oServerLicense.FeatureIsInstalled("Feature\HRScheduling") Then
                        Dim oParameters As New roParameters("OPTIONS", True)
                        Dim strFieldCost As String = ""
                        If oParameters.Parameter(Parameters.EmployeeFieldCost) IsNot Nothing Then
                            strFieldCost = Any2String(oParameters.Parameter(Parameters.EmployeeFieldCost))
                            If strFieldCost.ToUpper = "USR_" & strFieldName.ToUpper Then
                                bolRet = True
                            End If
                        End If

                    End If

                ElseIf oType = Types.GroupField Then

                    If oServerLicense.FeatureIsInstalled("Feature\HRScheduling") Then
                        ' Miramos si el campo se està utilizando en algún campo de planificacion
                        Dim dTblAssigments As DataTable = roUserField.GetUserFieldUsedInAssignments(strFieldName, _State)
                        If dTblAssigments.Rows.Count > 0 Then
                            bolRet = True
                        End If
                    Else
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::UsedInProcess")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::UsedInProcess")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Informa si el campo de la ficha actual es visible para solicitudes para un empleado.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RequestVisible(ByVal _IDEmployee As Integer) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.oType = Types.EmployeeField Then

                    If Me.intRequestPermissions = 0 Then ' El campo es visible para solicitudes para todos los empleados

                        bolRet = True

                    ElseIf Me.intRequestPermissions = 2 Then ' Según criterio

                        If Me.lstRequestsConditions IsNot Nothing AndAlso Me.lstRequestsConditions.Count > 0 Then

                            ' Verificar si el empleado actual cumple la condición
                            Dim strWhere As String = "Employees.ID = " & _IDEmployee.ToString & " "
                            For Each oCondition As roUserFieldCondition In Me.lstRequestsConditions
                                strWhere &= "AND " & oCondition.GetFilter(_IDEmployee) & " "
                            Next

                            strWhere = "Employees.ID = " & _IDEmployee.ToString & strWhere
                            Dim tbEmployee As DataTable = roBusinessSupport.GetEmployees(strWhere, "", "", Me.State)
                            If tbEmployee IsNot Nothing AndAlso tbEmployee.Rows.Count > 0 Then
                                ' El empleado cumple los criterios de selección
                                bolRet = True
                            End If

                        End If

                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roUserField::RequestVisible")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roUserField::RequestVisible")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function GetUserFieldType(ByVal idUserField As Integer, ByVal _State As roUserFieldState) As Integer
            Dim fieldType As Integer = -1
            Try
                Dim strSQL As String = $"@SELECT# FieldType FROM sysroUserFields WHERE Id = {idUserField}"
                Dim queryResult As Object = AccessHelper.ExecuteScalar(strSQL)

                If Not IsDBNull(queryResult) Then
                    fieldType = roTypes.Any2Integer(queryResult)
                End If

            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldType")
            End Try

            Return fieldType
        End Function


        Public Shared Function CanUserFieldApplyUniqueConstraint(ByVal userFieldName As String, ByVal userFieldId As Integer, ByVal _State As roUserFieldState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim canBeUnique As Boolean = False

            Try
                Dim sql As String = $"@SELECT# Total, FieldName FROM (
                                        @SELECT# COUNT(*) Total, Fieldname, LOWER(FieldValue) FieldValue
                                        FROM (
                                            @SELECT# ROW_NUMBER() OVER (Partition BY IDEmployee, EmployeeUserFieldValues.Fieldname, CONVERT(NVARCHAR(400), Value) ORDER BY IDEmployee ASC, EmployeeUserFieldValues.FieldName ASC) as RowNum, 
                                                        IDEmployee, EmployeeUserFieldValues.FieldName, CONVERT(NVARCHAR(400), Value) FieldValue
                                            FROM EmployeeUserFieldValues
		                                    INNER JOIN sysroUserFields ON sysroUserFields.FieldName = EmployeeUserFieldValues.FieldName
                                            WHERE EmployeeUserFieldValues.FieldName = '{userFieldName}' 
			                                    AND ((sysroUserFields.FieldType = 0 AND CONVERT(NVARCHAR(400), Value) <> '') OR (sysroUserFields.FieldType = 1 AND CONVERT(NVARCHAR(400), Value) <> '0'))
                                        ) AUXTAB 
                                        WHERE RowNum = 1 
                                        GROUP BY FieldName, LOWER(FieldValue)
                                    ) AUXTAB1
                                    WHERE Total > 1"
                Dim tb As DataTable = CreateDataTable(sql, )
                If tb IsNot Nothing Then
                    canBeUnique = tb.Rows.Count = 0
                End If

            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::CanUserFieldApplyUniqueConstraint")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::CanUserFieldApplyUniqueConstraint")
            End Try

            Return canBeUnique

        End Function

        Public Shared Function SetUniqueConstraintToUserField(ByVal userFieldName As String, ByVal userFieldId As Integer, ByVal _State As roUserFieldState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim saved As Boolean = False

            Try
                Dim canBeUnique As Boolean = roUserField.CanUserFieldApplyUniqueConstraint(userFieldName, userFieldId, _State, bAudit)
                If canBeUnique Then
                    Dim sql As String = $"@UPDATE# sysroUserFields SET [Unique] = 1 WHERE ID = {userFieldId}"
                    saved = DataLayer.AccessHelper.ExecuteSql(sql)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::SetUniqueConstraintToUserField")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::SetUniqueConstraintToUserField")
            End Try

            Return saved

        End Function


#End Region

#End Region

#Region "Used In process"

        '' <summary>
        '' Obtiene una lista con los nombres de la ficha que se utilizan en algún valor inicial.
        '' </summary>
        '' <param name="_State"></param>
        '' <param name="oActiveTransaction"></param>
        '' <remarks></remarks>
        Public Shared Function GetUserFieldsUsedInStartupValues(ByVal _State As roUserFieldState) As Generic.List(Of String)

            Dim oRet As New Generic.List(Of String)

            Try

                Dim strSQL As String
                strSQL = "@SELECT# DISTINCT StartupValues.StartValue " &
                         "FROM StartupValues "
                strSQL &= " WHERE StartupValues.StartValueType = 2"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        If Not oRet.Contains(oRow.Item("StartValue")) Then
                            oRet.Add(oRow.Item("StartValue"))
                        End If
                    Next

                End If

                strSQL = "@SELECT# DISTINCT StartupValues.StartValueBase " &
                         "FROM StartupValues "
                strSQL &= " WHERE StartupValues.StartValueType = 3 AND StartupValues.StartValueBaseType = 1"
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        If Not oRet.Contains(oRow.Item("StartValueBase")) Then
                            oRet.Add(oRow.Item("StartValueBase"))
                        End If
                    Next
                End If

                strSQL = "@SELECT# DISTINCT StartupValues.TotalPeriodBase " &
                         "FROM StartupValues "
                strSQL &= " WHERE StartupValues.StartValueType = 3 AND StartupValues.TotalPeriodBaseType = 1"
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        If Not oRet.Contains(oRow.Item("TotalPeriodBase")) Then
                            oRet.Add(oRow.Item("TotalPeriodBase"))
                        End If
                    Next
                End If

                strSQL = "@SELECT# DISTINCT StartupValues.AccruedValue " &
                         "FROM StartupValues "
                strSQL &= " WHERE StartupValues.StartValueType = 3 AND StartupValues.AccruedValueType = 1"
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        If Not oRet.Contains(oRow.Item("AccruedValue")) Then
                            oRet.Add(oRow.Item("AccruedValue"))
                        End If
                    Next
                End If

                strSQL = "@SELECT# DISTINCT StartupValues.MaximumValue " &
                         "FROM StartupValues "
                strSQL &= " WHERE StartupValues.MaximumValueType = 2"

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        If Not oRet.Contains(oRow.Item("MaximumValue")) Then
                            oRet.Add(oRow.Item("MaximumValue"))
                        End If
                    Next
                End If

                strSQL = "@SELECT# DISTINCT StartupValues.MinimumValue " &
                         "FROM StartupValues "
                strSQL &= " WHERE StartupValues.MinimumValueType = 2"

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        If Not oRet.Contains(oRow.Item("MinimumValue")) Then
                            oRet.Add(oRow.Item("MinimumValue"))
                        End If
                    Next
                End If

                strSQL = "@SELECT# DISTINCT StartupValues.NewContractExceptionCondition " &
                         "FROM StartupValues "
                strSQL &= " WHERE StartupValues.NewContractException = 1"

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        Try
                            Dim m_Condition As New roCollection
                            m_Condition.LoadXMLString(oRow("NewContractExceptionCondition"))
                            If Not oRet.Contains(m_Condition("Condition1")("UserFieldName")) Then
                                oRet.Add(m_Condition("Condition1")("UserFieldName"))
                            End If
                        Catch ex As Exception
                        End Try
                    Next
                End If

                strSQL = "@SELECT# DISTINCT StartupValues.EndCustomPeriodUserField " &
                         "FROM StartupValues "
                strSQL &= " WHERE isnull(StartupValues.ApplyEndCustomPeriod,0) = 1"

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        If Not oRet.Contains(oRow.Item("EndCustomPeriodUserField")) Then
                            oRet.Add(oRow.Item("EndCustomPeriodUserField"))
                        End If
                    Next
                End If

                strSQL = "@SELECT# DISTINCT StartupValues.ScalingUserField " &
                         "FROM StartupValues "
                strSQL &= " WHERE StartupValues.StartValueType = 3 AND len(isnull(StartupValues.ScalingUserField, '')) > 0 "

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        If Any2String(oRow.Item("ScalingUserField")).Trim.Length > 0 Then
                            If Not oRet.Contains(oRow.Item("ScalingUserField")) Then
                                oRet.Add(oRow.Item("ScalingUserField"))
                            End If
                        End If
                    Next
                End If

                strSQL = "@SELECT# DISTINCT StartupValues.ScalingCoefficientUserField " &
                         "FROM StartupValues "
                strSQL &= " WHERE StartupValues.StartValueType = 3 AND len(isnull(StartupValues.ScalingCoefficientUserField, '')) > 0 "

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        If Any2String(oRow.Item("ScalingCoefficientUserField")).Trim.Length > 0 Then
                            If Not oRet.Contains(oRow.Item("ScalingCoefficientUserField")) Then
                                oRet.Add(oRow.Item("ScalingCoefficientUserField"))
                            End If
                        End If
                    Next
                End If

            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsUsedInStartupValues")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsUsedInStartupValues")

            End Try

            Return oRet

        End Function

        '' <summary>
        '' Obtiene una lista con los nombres de la ficha que se utilizan en algún valor inicial.
        '' </summary>
        '' <param name="_State"></param>
        '' <param name="oActiveTransaction"></param>
        '' <remarks></remarks>
        Public Shared Function GetUserFieldsUsedInCauseLimitValues(ByVal _State As roUserFieldState) As Generic.List(Of String)

            Dim oRet As New Generic.List(Of String)

            Try

                Dim strSQL As String
                strSQL = "@SELECT# DISTINCT CauseLimitValues.MaximumAnnualValue " &
                         "FROM CauseLimitValues "
                strSQL &= " WHERE CauseLimitValues.MaximumAnnualValueType = 2"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        If Not oRet.Contains(oRow.Item("MaximumAnnualValue")) Then
                            oRet.Add(oRow.Item("MaximumAnnualValue"))
                        End If
                    Next

                End If

                strSQL = "@SELECT# DISTINCT CauseLimitValues.MaximumMonthlyValue " &
                         "FROM CauseLimitValues "
                strSQL &= " WHERE CauseLimitValues.MaximumMonthlyType = 2"

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    For Each oRow As DataRow In tb.Rows
                        If Not oRet.Contains(oRow.Item("MaximumMonthlyValue")) Then
                            oRet.Add(oRow.Item("MaximumMonthlyValue"))
                        End If
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsUsedInStartupValues")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsUsedInStartupValues")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetUserFieldsUsedInShifts(ByVal _State As roUserFieldState) As Generic.List(Of String)

            Dim oRet As New Generic.List(Of String)
            Dim oShift As New DataTable

            Try

                Dim strSQL As String = ""

                strSQL &= "If Exists(@SELECT# * from tempdb..sysobjects Where id = object_id('tempdb.dbo.#rulesxml')) " &
                            " @DROP# table #rulesxml " &
                            " @SELECT# tmp.* into #rulesxml  from ( @SELECT# convert(Xml,Definition) as ruleXml from( " &
                            " @SELECT# distinct CONVERT(NVARCHAR(MAX),Definition) as Definition from sysroShiftsCausesRules where RuleType = 3)xmlDef) tmp"

                ' Obtenemos los horaros que tienen reglas diarias que usen dicha justificacion
                strSQL &= " @SELECT# 1 AS RuleNumber, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""Action_1"")]/text())[1]', 'nvarchar(max)'),-1) AS RuleConfigurationActive, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOver_1"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverUserFieldValue_1"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionUserField, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverAction_1"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverResultAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverActionUserFieldValue_1"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionResultUserField, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusAction_1"")]/text())[1]', 'nvarchar(max)'),-1) as PlusAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValue_1"")]/text())[1]', 'nvarchar(max)'),'') as PlusActionUserField, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusActionResult_1"")]/text())[1]', 'nvarchar(max)'),-1) as PlusResultAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValueResult_1"")]/text())[1]', 'nvarchar(max)'),'') as PlusResultActionUserField " &
                            "From #rulesxml " &
                            "UNION " &
                            "@SELECT# 2 As RuleNumber, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""Action_2"")]/text())[1]', 'nvarchar(max)'),-1) AS RuleConfigurationActive, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOver_2"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverUserFieldValue_2"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionUserField, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverAction_2"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverResultAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverActionUserFieldValue_2"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionResultUserField, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusAction_2"")]/text())[1]', 'nvarchar(max)'),-1) as PlusAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValue_2"")]/text())[1]', 'nvarchar(max)'),'') as PlusActionUserField, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusActionResult_2"")]/text())[1]', 'nvarchar(max)'),-1) as PlusResultAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValueResult_2"")]/text())[1]', 'nvarchar(max)'),'') as PlusResultActionUserField " &
                            "From #rulesxml " &
                            "UNION " &
                            "@SELECT# 3 As RuleNumber, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""Action_3"")]/text())[1]', 'nvarchar(max)'),-1) AS RuleConfigurationActive, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOver_3"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverUserFieldValue_3"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionUserField, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverAction_3"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverResultAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverActionUserFieldValue_3"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionResultUserField, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusAction_3"")]/text())[1]', 'nvarchar(max)'),-1) as PlusAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValue_3"")]/text())[1]', 'nvarchar(max)'),'') as PlusActionUserField, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusActionResult_3"")]/text())[1]', 'nvarchar(max)'),-1)  as PlusResultAction, " &
                            "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValueResult_3"")]/text())[1]', 'nvarchar(max)'),'') as PlusResultActionUserField " &
                            $"From #rulesxml {DataLayer.SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetUserFieldsUsedInShifts)}"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        Dim strFactorFieldValue As String = String.Empty
                        Select Case roTypes.Any2Integer(oRow("RuleConfigurationActive"))
                            Case 0 'CarryOverAction
                                If roTypes.Any2Integer(oRow("CarryOverAction")) = 1 Then
                                    strFactorFieldValue = roTypes.Any2String(oRow("CarryOverActionUserField"))
                                    If strFactorFieldValue <> String.Empty AndAlso Not oRet.Contains(strFactorFieldValue) Then
                                        oRet.Add(strFactorFieldValue)
                                    End If
                                End If
                                If roTypes.Any2Integer(oRow("CarryOverResultAction")) = 1 Then
                                    strFactorFieldValue = roTypes.Any2String(oRow("CarryOverActionResultUserField"))
                                    If strFactorFieldValue <> String.Empty AndAlso Not oRet.Contains(strFactorFieldValue) Then
                                        oRet.Add(strFactorFieldValue)
                                    End If
                                End If
                            Case 1 'PlusAction
                                If roTypes.Any2Integer(oRow("PlusAction")) = 1 Then
                                    strFactorFieldValue = roTypes.Any2String(oRow("PlusActionUserField"))
                                    If strFactorFieldValue <> String.Empty AndAlso Not oRet.Contains(strFactorFieldValue) Then
                                        oRet.Add(strFactorFieldValue)
                                    End If
                                End If

                                If roTypes.Any2Integer(oRow("PlusResultAction")) = 1 Then
                                    strFactorFieldValue = roTypes.Any2String(oRow("PlusResultActionUserField"))
                                    If strFactorFieldValue <> String.Empty AndAlso Not oRet.Contains(strFactorFieldValue) Then
                                        oRet.Add(strFactorFieldValue)
                                    End If
                                End If
                            Case Else 'NotConfigured
                        End Select
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsUsedInShifts")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsUsedInShifts")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene una lista con los nombres de la ficha que se utilizan en alguna regla de convenio.
        ''' </summary>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <remarks></remarks>
        Public Shared Function GetUserFieldsUsedInLabAgreeRules(ByVal _State As roUserFieldState) As Generic.List(Of String)

            Dim oRet As New Generic.List(Of String)

            Try

                Dim strSQL As String
                strSQL = "@SELECT# CONVERT(xml, AccrualsRules.Definition).value('(/roCollection/Item[@key=(""ValueType"")]/text())[1]', 'nvarchar(max)') as ValueType, " &
                            "Convert(Xml, AccrualsRules.Definition).value('(/roCollection/Item[@key=(""PlusUserFieldValue_1"")]/text())[1]', 'nvarchar(max)') as ValueUserField, " &
                            "Convert(Xml, AccrualsRules.Definition).value('(/roCollection/Item[@key=(""Dif"")]/text())[1]', 'nvarchar(max)') as Dif, " &
                            "Convert(Xml, AccrualsRules.Definition).value('(/roCollection/Item[@key=(""UntilUserField"")]/text())[1]', 'nvarchar(max)') as UntilUserField " &
                        "From AccrualsRules Where IdAccrualsRule In(@SELECT# la.IdAccrualsRules from LabAgreeAccrualsRules la) "

                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count Then
                    For Each oRow As DataRow In tb.Rows
                        Select Case roTypes.Any2Integer(oRow("ValueType"))
                            Case 1 'eRuleDefinitionValueType.UserFieldValue = 2
                                Dim strFactorFieldValue As String = roTypes.Any2String(oRow("ValueUserField"))
                                If strFactorFieldValue <> String.Empty AndAlso Not oRet.Contains(strFactorFieldValue) Then
                                    oRet.Add(strFactorFieldValue)
                                End If
                            Case Else 'NotConfigured
                        End Select

                        Select Case roTypes.Any2Integer(oRow("Dif"))
                            Case 4 'eRuleDefinitionDif.UserFieldUntilValue = 4
                                Dim strFactorFieldValue As String = roTypes.Any2String(oRow("UntilUserField"))
                                If strFactorFieldValue <> String.Empty AndAlso Not oRet.Contains(strFactorFieldValue) Then
                                    oRet.Add(strFactorFieldValue)
                                End If
                            Case Else 'NotConfigured
                        End Select
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsUsedInLabAgreeRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsUsedInLabAgreeRules")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene una lista con los nombres de la ficha que se utilizan en algún saldo.
        ''' </summary>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <remarks></remarks>
        Public Shared Function GetUserFieldsUsedInConcepts(ByVal _State As roUserFieldState) As Generic.List(Of String)

            Dim oRet As New Generic.List(Of String)

            Try

                Dim strSQL As String
                strSQL = "@SELECT# Convert(Xml, ConceptCauses.Composition).value('(/roCollection/Item[@key=(""FactorType"")]/text())[1]', 'nvarchar(max)') as FactorType, " &
                            "Convert(Xml, ConceptCauses.Composition).value('(/roCollection/Item[@key=(""FactorUserField"")]/text())[1]', 'nvarchar(max)') as FactorUserField, " &
                            "CONVERT(xml, ConceptCauses.Composition).value('(/roCollection/Item[@key=(""CompositionUserField"")]/text())[1]', 'nvarchar(max)') as CompositionUserField " &
                        "From ConceptCauses"

                Dim tb As DataTable = CreateDataTable(strSQL, )

                Dim strFactorFieldValue As String = String.Empty
                If tb IsNot Nothing AndAlso tb.Rows.Count Then
                    For Each oRow As DataRow In tb.Rows
                        Select Case roTypes.Any2Integer(oRow("FactorType"))
                            Case 1 'eRuleDefinitionValueType.UserFieldValue = 2
                                strFactorFieldValue = roTypes.Any2String(oRow("FactorUserField"))
                                If strFactorFieldValue <> String.Empty AndAlso Not oRet.Contains(strFactorFieldValue) Then
                                    oRet.Add(strFactorFieldValue)
                                End If
                            Case Else 'NotConfigured
                        End Select

                        strFactorFieldValue = roTypes.Any2String(oRow("CompositionUserField"))
                        If strFactorFieldValue <> String.Empty AndAlso Not oRet.Contains(strFactorFieldValue) Then
                            oRet.Add(strFactorFieldValue)
                        End If
                    Next
                End If

                strSQL = "@SELECT# Convert(Xml, Concepts.AutomaticAccrualCriteria).value('(/roCollection/Item[@key=(""FactorValue"")]/text())[1]', 'nvarchar(max)') as FactorValue, " &
                            "Convert(Xml, Concepts.AutomaticAccrualCriteria).value('(/roCollection/Item[@key=(""FactorField"")]/text())[1]', 'nvarchar(max)') as FactorField " &
                        "From Concepts"

                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count Then
                    For Each oRow As DataRow In tb.Rows
                        Select Case roTypes.Any2Integer(oRow("FactorValue"))
                            Case 1 'eFactorType.UserField = 1
                                strFactorFieldValue = roTypes.Any2String(oRow("FactorField"))
                                If strFactorFieldValue <> String.Empty AndAlso Not oRet.Contains(strFactorFieldValue) Then
                                    oRet.Add(strFactorFieldValue)
                                End If
                            Case Else 'NotConfigured
                        End Select
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsUsedInConcepts")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldsUsedInConcepts")
            Finally

            End Try

            Return oRet

        End Function

        '' <summary>
        '' Obtiene los valores iniciales que están utilizando el campo de la ficha. Opcionalmente se le puede indicar un empleado, devolviendo solo la lista de valores iniciales que afecten a los convenios del empleado.
        '' </summary>
        '' <param name="UserFieldName">Nombre del campo de la ficha.</param>
        '' <param name="_State"></param>
        '' <param name="_IDEmployee">Opcional. Código del empleado.</param>
        '' <param name="oActiveTransaction"></param>
        '' <returns></returns>
        '' <remarks></remarks>
        Public Shared Function GetUserFieldUsedInStartupValues(ByVal UserFieldName As String, ByVal _State As roUserFieldState, Optional ByVal _IDEmployee As Integer = -1) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# StartupValues.* " &
                         "FROM StartupValues "
                strSQL &= " INNER JOIN LabAgreeStartupValues ON StartupValues.IDStartupValue = LabAgreeStartupValues.IDStartupValue "
                If _IDEmployee <> -1 Then
                    strSQL &= " INNER JOIN EmployeeContracts ON LabAgreeStartupValues.IDLabAgree = EmployeeContracts.IDLabAgree "
                End If
                strSQL &= " WHERE ((StartupValues.StartValueType = 2 And StartupValues.StartValue = '" & UserFieldName.Replace("'", "''") & "') OR " &
                                  "(StartupValues.MaximumValueType = 2 and StartupValues.MaximumValue = '" & UserFieldName.Replace("'", "''") & "') OR " &
                                  "(StartupValues.MinimumValueType = 2 and StartupValues.MinimumValue = '" & UserFieldName.Replace("'", "''") & "') OR " &
                                  "(StartupValues.StartValueType = 3 And  StartupValues.StartValueBaseType = 1 AND StartupValues.StartValueBase = '" & UserFieldName.Replace("'", "''") & "') OR " &
                                  "(StartupValues.StartValueType = 3 And  StartupValues.TotalPeriodBaseType = 1 AND StartupValues.TotalPeriodBase = '" & UserFieldName.Replace("'", "''") & "') OR " &
                                  "(StartupValues.StartValueType = 3 And  StartupValues.AccruedValueType = 1 AND StartupValues.AccruedValue = '" & UserFieldName.Replace("'", "''") & "') OR " &
                                  "(StartupValues.StartValueType = 3 And  StartupValues.ScalingUserField = '" & UserFieldName.Replace("'", "''") & "') OR " &
                                  "(StartupValues.StartValueType = 3 And  StartupValues.ScalingCoefficientUserField = '" & UserFieldName.Replace("'", "''") & "') OR " &
                                  "(isnull(StartupValues.ApplyEndCustomPeriod,0) = 1 And StartupValues.EndCustomPeriodUserField = '" & UserFieldName.Replace("'", "''") & "') OR " &
                                  "(StartupValues.NewContractException = 1  AND CONVERT(NVARCHAR(4000), isnull(StartupValues.NewContractExceptionCondition,'')) like  '%" & UserFieldName.Replace("'", "''") & "%') OR " &
                                  "(StartupValues.StartValueType = 3 And StartupValues.ScalingUserField = '" & UserFieldName.Replace("'", "''") & "'))"

                If _IDEmployee <> -1 Then
                    strSQL &= " AND EmployeeContracts.IDEmployee = " & _IDEmployee.ToString
                End If
                strSQL &= " ORDER BY StartupValues.Name"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInStartupValues")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInStartupValues")
            Finally

            End Try

            Return oRet

        End Function

        '' <summary>
        '' Comprueba si un campo de la ficha está en uso en la definición de algún colectivo
        '' </summary>
        '' <param name="UserFieldName">Nombre del campo de la ficha.</param>
        '' <param name="_State"></param>
        '' <returns></returns>
        '' <remarks></remarks>
        Public Shared Function CheckIfUserFieldIsUsedInCollectives(ByVal userFieldName As String, ByVal _State As roUserFieldState) As Boolean

            Dim returnValue As Boolean = True

            Try

                Dim strSQL As String
                strSQL = $"@SELECT# ID FROM CollectivesDefinitions WHERE Definition LIKE '%""{userFieldName}""%'"

                Dim resulTable As DataTable = CreateDataTable(strSQL, )

                returnValue = (resulTable IsNot Nothing AndAlso resulTable.Rows.Count > 0)

            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::CheckIfUserFieldIsUsedInCollectives")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::CheckIfUserFieldIsUsedInCollectives")

            End Try

            Return returnValue

        End Function

        '' <summary>
        '' Obtiene los valores iniciales que están utilizando el campo de la ficha. Opcionalmente se le puede indicar un empleado, devolviendo solo la lista de valores iniciales que afecten a los convenios del empleado.
        '' </summary>
        '' <param name="UserFieldName">Nombre del campo de la ficha.</param>
        '' <param name="_State"></param>
        '' <param name="_IDEmployee">Opcional. Código del empleado.</param>
        '' <param name="oActiveTransaction"></param>
        '' <returns></returns>
        '' <remarks></remarks>
        Public Shared Function GetUserFieldUsedInCauseLimitValues(ByVal UserFieldName As String, ByVal _State As roUserFieldState, Optional ByVal _IDEmployee As Integer = -1) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# LabAgreeCauseLimitValues.* " &
                         "FROM CauseLimitValues "
                strSQL &= " INNER JOIN LabAgreeCauseLimitValues ON CauseLimitValues.IDCauseLimitValue = LabAgreeCauseLimitValues.IDCauseLimitValue "
                If _IDEmployee <> -1 Then
                    strSQL &= " INNER JOIN EmployeeContracts ON LabAgreeCauseLimitValues.IDLabAgree = EmployeeContracts.IDLabAgree "
                End If
                strSQL &= " WHERE ((CauseLimitValues.MaximumAnnualValueType = 2 And CauseLimitValues.MaximumAnnualValue = '" & UserFieldName & "') OR " &
                                  "(CauseLimitValues.MaximumMonthlyType = 2 and CauseLimitValues.MaximumMonthlyValue = '" & UserFieldName & "'))"
                If _IDEmployee <> -1 Then
                    strSQL &= " AND EmployeeContracts.IDEmployee = " & _IDEmployee.ToString
                End If
                strSQL &= " ORDER BY CauseLimitValues.Name"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInCauseLimitValues")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInCauseLimitValues")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene las reglas de convenio que están utilizando el campo de la ficha. Opcionalmente se le puede indicar un empleado, devolviendo solo la lista de reglas de convenio que afecten a los convenios del empleado.
        ''' </summary>
        ''' <param name="UserFieldName">Nombre del campo de la ficha.</param>
        ''' <param name="_State"></param>
        ''' <param name="_IDEmployee">Opcional. Código del empleado.</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetUserFieldUsedInLabAgreeRules(ByVal UserFieldName As String, ByVal _State As roUserFieldState, Optional ByVal _IDEmployee As Integer = -1) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "set arithabort on; " &
                    "@SELECT# DISTINCT AccrualsRules.IdAccrualsRule, " &
                            "CONVERT(xml, AccrualsRules.Definition).value('(/roCollection/Item[@key=(""ValueType"")]/text())[1]', 'nvarchar(max)') as ValueType, " &
                            "Convert(Xml, AccrualsRules.Definition).value('(/roCollection/Item[@key=(""PlusUserFieldValue_1"")]/text())[1]', 'nvarchar(max)') as ValueUserField, " &
                            "Convert(Xml, AccrualsRules.Definition).value('(/roCollection/Item[@key=(""Dif"")]/text())[1]', 'nvarchar(max)') as Dif, " &
                            "Convert(Xml, AccrualsRules.Definition).value('(/roCollection/Item[@key=(""UntilUserField"")]/text())[1]', 'nvarchar(max)') as UntilUserField " &
                         "FROM AccrualsRules "
                strSQL &= " INNER JOIN LabAgreeAccrualsRules ON AccrualsRules.IdAccrualsRule = LabAgreeAccrualsRules.IdAccrualsRules "
                If _IDEmployee <> -1 Then
                    strSQL &= " INNER JOIN EmployeeContracts ON LabAgreeAccrualsRules.IDLabAgree = EmployeeContracts.IDLabAgree "
                End If

                strSQL &= " where IdAccrualsRule in( @SELECT# la.IdAccrualsRules from LabAgreeAccrualsRules la) "

                If _IDEmployee <> -1 Then
                    strSQL &= " AND EmployeeContracts.IDEmployee = " & _IDEmployee.ToString
                End If
                strSQL &= " ORDER BY AccrualsRules.IdAccrualsRule"
                strSQL &= " set arithabort off;"

                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing Then
                    Dim bolUse As Boolean = False

                    For Each oRow As DataRow In oRet.Rows
                        bolUse = False

                        Select Case roTypes.Any2Integer(oRow("ValueType"))
                            Case 1 'eRuleDefinitionValueType.UserFieldValue = 2
                                Dim strFactorFieldValue As String = roTypes.Any2String(oRow("ValueUserField"))
                                If strFactorFieldValue <> String.Empty AndAlso strFactorFieldValue.ToLower = UserFieldName.ToLower Then
                                    bolUse = True
                                End If
                            Case Else 'NotConfigured
                        End Select

                        Select Case roTypes.Any2Integer(oRow("Dif"))
                            Case 4 'eRuleDefinitionDif.UserFieldUntilValue = 4
                                Dim strFactorFieldValue As String = roTypes.Any2String(oRow("UntilUserField"))
                                If strFactorFieldValue <> String.Empty AndAlso strFactorFieldValue.ToLower = UserFieldName.ToLower Then
                                    bolUse = True
                                End If
                            Case Else 'NotConfigured
                        End Select

                        If Not bolUse Then
                            oRow.Delete()
                        End If

                    Next

                    oRet.AcceptChanges()
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInLabAgreeRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInLabAgreeRules")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene los saldos que están utilizando el campo de la ficha.
        ''' </summary>
        ''' <param name="UserFieldName">Nombre del campo de la ficha.</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>'GetConceptsUseUserField
        Public Shared Function GetUserFieldUsedInConcepts(ByVal UserFieldName As String, ByVal _State As roUserFieldState) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim bolUse As Boolean = False

                Dim oConceptsIDList As New Generic.List(Of Integer)

                Dim strSQL As String
                strSQL = "@SELECT# IDConcept, Convert(Xml, ConceptCauses.Composition).value('(/roCollection/Item[@key=(""FactorType"")]/text())[1]', 'nvarchar(max)') as FactorType, " &
                            "Convert(Xml, ConceptCauses.Composition).value('(/roCollection/Item[@key=(""FactorUserField"")]/text())[1]', 'nvarchar(max)') as FactorUserField, " &
                            "CONVERT(xml, ConceptCauses.Composition).value('(/roCollection/Item[@key=(""CompositionUserField"")]/text())[1]', 'nvarchar(max)') as CompositionUserField, FieldFactor " &
                        "From ConceptCauses"

                Dim tb As DataTable = CreateDataTable(strSQL, )

                Dim strFactorFieldValue As String = String.Empty
                If tb IsNot Nothing AndAlso tb.Rows.Count Then
                    For Each oRow As DataRow In tb.Rows
                        bolUse = False

                        Select Case roTypes.Any2Integer(oRow("FactorType"))
                            Case 1 'eRuleDefinitionValueType.UserFieldValue = 2
                                strFactorFieldValue = roTypes.Any2String(oRow("FactorUserField"))
                                If strFactorFieldValue <> String.Empty AndAlso UserFieldName.ToLower = strFactorFieldValue.ToLower Then
                                    bolUse = True

                                    If Not oConceptsIDList.Contains(roTypes.Any2Integer(oRow("IDConcept"))) Then
                                        oConceptsIDList.Add(roTypes.Any2Integer(oRow("IDConcept")))
                                    End If

                                End If
                            Case Else 'NotConfigured
                        End Select

                        If Not bolUse Then
                            strFactorFieldValue = roTypes.Any2String(oRow("CompositionUserField"))
                            If strFactorFieldValue <> String.Empty AndAlso UserFieldName.ToLower = strFactorFieldValue.ToLower Then
                                bolUse = True

                                If Not oConceptsIDList.Contains(roTypes.Any2Integer(oRow("IDConcept"))) Then
                                    oConceptsIDList.Add(roTypes.Any2Integer(oRow("IDConcept")))
                                End If
                            End If
                        End If

                        If Not bolUse Then
                            strFactorFieldValue = roTypes.Any2String(oRow("FieldFactor"))
                            If strFactorFieldValue <> String.Empty AndAlso UserFieldName.ToLower = strFactorFieldValue.ToLower Then
                                bolUse = True

                                If Not oConceptsIDList.Contains(roTypes.Any2Integer(oRow("IDConcept"))) Then
                                    oConceptsIDList.Add(roTypes.Any2Integer(oRow("IDConcept")))
                                End If
                            End If
                        End If

                    Next
                End If

                strSQL = "@SELECT# ID, Convert(Xml, Concepts.AutomaticAccrualCriteria).value('(/roCollection/Item[@key=(""FactorValue"")]/text())[1]', 'nvarchar(max)') as FactorValue, " &
                            "Convert(Xml, Concepts.AutomaticAccrualCriteria).value('(/roCollection/Item[@key=(""FactorField"")]/text())[1]', 'nvarchar(max)') as FactorField " &
                        "From Concepts"

                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count Then
                    For Each oRow As DataRow In tb.Rows
                        bolUse = False

                        Select Case roTypes.Any2Integer(oRow("FactorValue"))
                            Case 1 'eFactorType.UserField = 1
                                strFactorFieldValue = roTypes.Any2String(oRow("FactorField"))
                                If strFactorFieldValue <> String.Empty AndAlso UserFieldName.ToLower = strFactorFieldValue.ToLower Then
                                    bolUse = True

                                    If Not oConceptsIDList.Contains(roTypes.Any2Integer(oRow("IDConcept"))) Then
                                        oConceptsIDList.Add(roTypes.Any2Integer(oRow("IDConcept")))
                                    End If
                                End If
                            Case Else 'NotConfigured
                        End Select
                    Next
                End If

                oRet = New DataTable
                oRet.Columns.Add("ID", GetType(Integer))

                If oRet IsNot Nothing Then
                    For Each oConceptID As Integer In oConceptsIDList

                        Dim oNewRow As DataRow = oRet.NewRow
                        oNewRow("ID") = oConceptID

                        oRet.Rows.Add(oNewRow)
                    Next

                    oRet.AcceptChanges()
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInConcepts")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInConcepts")
            Finally

            End Try

            Return oRet

        End Function

        'GetShiftsUseUserField
        Public Shared Function GetUserFieldUsedInShifts(ByVal UserFieldName As String, ByVal _State As roUserFieldState) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim oShiftsIDList As New Generic.List(Of Integer)
                Dim strSQL As String = ""
                Dim mExistField As Boolean = False

                ' En BBDD con muchas reglas diarias esto puede penalizar mucho, de igual modo hay otras reglas que no se estas verificando como son las simples
                ' por regla general se puede entender que si se cambia un campo de la ficha que afecta a una regla de horario, no tenga efecto retroactivo
                ' por lo que por defecto no se validara, y en el caso que un cliente realmente quiera validar las reglas diarias lo activaremos

                Dim param As New AdvancedParameter.roAdvancedParameter("CheckFieldUsedInShifts", New AdvancedParameter.roAdvancedParameterState)
                If Any2Double(param.Value) = 1 Then
                    strSQL &= "If Exists(@SELECT# * from tempdb..sysobjects Where id = object_id('tempdb.dbo.#rulesxml')) " &
                            " @DROP# table #rulesxml " &
                            " @SELECT# tmp.* into #rulesxml  from ( @SELECT# IDShift, convert(Xml,Definition) as ruleXml from( " &
                            " @SELECT# distinct IDShift, CONVERT(NVARCHAR(MAX),Definition) as Definition from sysroShiftsCausesRules where RuleType = 3)xmlDef) tmp"

                    ' Obtenemos los horaros que tienen reglas diarias que usen dicha justificacion
                    strSQL &= " @SELECT# IDShift, 1 AS RuleNumber, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""Action_1"")]/text())[1]', 'nvarchar(max)'),-1) AS RuleConfigurationActive, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOver_1"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverUserFieldValue_1"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionUserField, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverAction_1"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverResultAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverActionUserFieldValue_1"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionResultUserField, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusAction_1"")]/text())[1]', 'nvarchar(max)'),-1) as PlusAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValue_1"")]/text())[1]', 'nvarchar(max)'),'') as PlusActionUserField, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusActionResult_1"")]/text())[1]', 'nvarchar(max)'),-1) as PlusResultAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValueResult_1"")]/text())[1]', 'nvarchar(max)'),'') as PlusResultActionUserField " &
                                "From #rulesxml " &
                                "UNION " &
                                "@SELECT# IDShift, 2 As RuleNumber, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""Action_2"")]/text())[1]', 'nvarchar(max)'),-1) AS RuleConfigurationActive, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOver_2"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverUserFieldValue_2"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionUserField, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverAction_2"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverResultAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverActionUserFieldValue_2"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionResultUserField, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusAction_2"")]/text())[1]', 'nvarchar(max)'),-1) as PlusAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValue_2"")]/text())[1]', 'nvarchar(max)'),'') as PlusActionUserField, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusActionResult_2"")]/text())[1]', 'nvarchar(max)'),-1) as PlusResultAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValueResult_2"")]/text())[1]', 'nvarchar(max)'),'') as PlusResultActionUserField " &
                                "From #rulesxml " &
                                "UNION " &
                                "@SELECT# IDShift, 3 As RuleNumber, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""Action_3"")]/text())[1]', 'nvarchar(max)'),-1) AS RuleConfigurationActive, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOver_3"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverUserFieldValue_3"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionUserField, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverAction_3"")]/text())[1]', 'nvarchar(max)'),-1) as CarryOverResultAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""CarryOverActionUserFieldValue_3"")]/text())[1]', 'nvarchar(max)'),'') as CarryOverActionResultUserField, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusAction_3"")]/text())[1]', 'nvarchar(max)'),-1) as PlusAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValue_3"")]/text())[1]', 'nvarchar(max)'),'') as PlusActionUserField, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusActionResult_3"")]/text())[1]', 'nvarchar(max)'),-1)  as PlusResultAction, " &
                                "isnull(#rulesxml.ruleXml.value('(/roCollection/Item[@key=(""PlusUserFieldValueResult_3"")]/text())[1]', 'nvarchar(max)'),'') as PlusResultActionUserField " &
                                "From #rulesxml "

                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        For Each oRow As DataRow In tb.Rows
                            Dim strFactorFieldValue As String = String.Empty
                            Select Case roTypes.Any2Integer(oRow("RuleConfigurationActive"))
                                Case 0 'CarryOverAction
                                    If roTypes.Any2Integer(oRow("CarryOverAction")) = 1 Then
                                        strFactorFieldValue = roTypes.Any2String(oRow("CarryOverActionUserField"))
                                        If strFactorFieldValue <> String.Empty AndAlso UserFieldName.ToLower = strFactorFieldValue.ToLower Then
                                            mExistField = True
                                            If Not oShiftsIDList.Contains(roTypes.Any2Integer(oRow("IDShift"))) Then oShiftsIDList.Add(roTypes.Any2Integer(oRow("IDShift")))
                                        End If
                                    End If
                                    If roTypes.Any2Integer(oRow("CarryOverResultAction")) = 1 Then
                                        strFactorFieldValue = roTypes.Any2String(oRow("CarryOverActionResultUserField"))
                                        If strFactorFieldValue <> String.Empty AndAlso UserFieldName.ToLower = strFactorFieldValue.ToLower Then
                                            mExistField = True
                                            If Not oShiftsIDList.Contains(roTypes.Any2Integer(oRow("IDShift"))) Then oShiftsIDList.Add(roTypes.Any2Integer(oRow("IDShift")))
                                        End If
                                    End If
                                Case 1 'PlusAction
                                    If roTypes.Any2Integer(oRow("PlusAction")) = 1 Then
                                        strFactorFieldValue = roTypes.Any2String(oRow("PlusActionUserField"))
                                        If strFactorFieldValue <> String.Empty AndAlso UserFieldName.ToLower = strFactorFieldValue.ToLower Then
                                            mExistField = True
                                            If Not oShiftsIDList.Contains(roTypes.Any2Integer(oRow("IDShift"))) Then oShiftsIDList.Add(roTypes.Any2Integer(oRow("IDShift")))
                                        End If
                                    End If

                                    If roTypes.Any2Integer(oRow("PlusResultAction")) = 1 Then
                                        strFactorFieldValue = roTypes.Any2String(oRow("PlusResultActionUserField"))
                                        If strFactorFieldValue <> String.Empty AndAlso UserFieldName.ToLower = strFactorFieldValue.ToLower Then
                                            mExistField = True
                                            If Not oShiftsIDList.Contains(roTypes.Any2Integer(oRow("IDShift"))) Then oShiftsIDList.Add(roTypes.Any2Integer(oRow("IDShift")))
                                        End If
                                    End If
                                Case Else 'NotConfigured
                            End Select
                        Next
                    End If
                End If

                oRet = New DataTable
                oRet.Columns.Add("ID", GetType(Integer))

                If oRet IsNot Nothing Then
                    For Each oConceptID As Integer In oShiftsIDList

                        Dim oNewRow As DataRow = oRet.NewRow
                        oNewRow("ID") = oConceptID

                        oRet.Rows.Add(oNewRow)
                    Next

                    oRet.AcceptChanges()
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInShifts")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInShifts")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene los puestos que están utilizando algun campo de la ficha. Opcionalmente se le puede indicar un grupo, devolviendo solo la lista de puestos que afecten.
        ''' </summary>
        ''' <param name="UserFieldName">Nombre del campo de la ficha.</param>
        ''' <param name="_State"></param>
        ''' <param name="_IDGroup">Opcional. Código del grupo.</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetUserFieldUsedInAssignments(ByVal UserFieldName As String, ByVal _State As roUserFieldState, Optional ByVal _IDGroup As Integer = -1) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# Assignments.* " &
                         "FROM Assignments "

                strSQL &= " WHERE CostField= 'USR_" & UserFieldName & "' "
                If _IDGroup <> -1 Then
                    strSQL &= " AND ID= " & _IDGroup.ToString
                End If
                strSQL &= " ORDER BY Assignments.Name"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roUserField::GetUserFieldUsedInAssignments")
            Finally

            End Try

            Return oRet

        End Function

#End Region

    End Class

    <DataContract>
    <Serializable()>
    Public Class roScalingValues
        Private strServiceValue As String
        Private strAccumValue As String

        Public Sub New()

        End Sub

        Public Sub New(ByVal _strUserField As String, ByVal _strAccumValue As String)

            Me.strServiceValue = _strUserField
            Me.strAccumValue = _strAccumValue

        End Sub

        <DataMember>
        Public Property UserField() As String
            Get
                Return Me.strServiceValue
            End Get
            Set(ByVal value As String)
                Me.strServiceValue = value
            End Set
        End Property

        <DataMember>
        Public Property AccumValue() As String
            Get
                Return Me.strAccumValue
            End Get
            Set(ByVal value As String)
                Me.strAccumValue = value
            End Set
        End Property
    End Class

    <DataContract>
    <Serializable>
    Public Class roUserFieldCondition

#Region "Declarations - Constructor"

        Private oUserField As roUserField
        Private oCompare As CompareType
        Private oValueType As CompareValueType
        Private strValue As String

        Public Sub New()

        End Sub

        Public Sub New(ByVal _UserField As roUserField, ByVal _Compare As CompareType, ByVal _ValueType As CompareValueType, Optional ByVal _Value As String = "")

            Me.oUserField = _UserField
            Me.oCompare = _Compare
            Me.oValueType = _ValueType
            Me.strValue = _Value

        End Sub

        Public Sub New(ByVal _State As roUserFieldState, ByVal _UserFieldName As String, ByVal _Compare As CompareType, ByVal _ValueType As CompareValueType, Optional ByVal _Value As String = "", Optional ByVal _UserFieldType As Types = Types.EmployeeField)

            Me.New(New roUserField(_State, _UserFieldName, _UserFieldType, False), _Compare, _ValueType, _Value)

        End Sub

        Public Sub New(ByVal oCondition As roCollection, ByVal _State As roUserFieldState)

            If oCondition IsNot Nothing Then

                Me.oUserField = New roUserField(_State, Any2String(oCondition.Item("UserFieldName")), Any2Integer(oCondition.Item("UserFieldType")), False)
                Me.oCompare = Any2Integer(oCondition.Item("Compare"))
                Me.oValueType = Any2Integer(oCondition.Item("ValueType"))
                Me.strValue = Any2String(oCondition.Item("Value"))

            End If

        End Sub

        Public Sub New(ByVal oCondition As roCollection, ByVal _State As roUserFieldState, ByVal CheckUsedInProcess As Boolean)

            If oCondition IsNot Nothing Then

                Me.oUserField = New roUserField(_State, Any2String(oCondition.Item("UserFieldName")), Any2Integer(oCondition.Item("UserFieldType")), CheckUsedInProcess)
                Me.oCompare = Any2Integer(oCondition.Item("Compare"))
                Me.oValueType = Any2Integer(oCondition.Item("ValueType"))
                Me.strValue = Any2String(oCondition.Item("Value"))

            End If

        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property UserField() As roUserField
            Get
                Return Me.oUserField
            End Get
            Set(ByVal value As roUserField)
                Me.oUserField = value
            End Set
        End Property

        <DataMember>
        Public Property Compare() As CompareType
            Get
                Return Me.oCompare
            End Get
            Set(ByVal value As CompareType)
                Me.oCompare = value
            End Set
        End Property

        <DataMember>
        Public Property ValueType() As CompareValueType
            Get
                Return Me.oValueType
            End Get
            Set(ByVal value As CompareValueType)
                Me.oValueType = value
            End Set
        End Property

        <DataMember>
        Public Property Value() As String
            Get
                Return Me.strValue
            End Get
            Set(ByVal value As String)
                Me.strValue = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function GetXml() As String

            Dim oCondition As New roCollection

            oCondition.Add("UserFieldName", Me.oUserField.FieldName)
            oCondition.Add("UserFieldDataType", Me.oUserField.FieldType)
            oCondition.Add("UserFieldType", Me.oUserField.Type)
            oCondition.Add("Compare", Me.oCompare)
            oCondition.Add("ValueType", Me.oValueType)
            oCondition.Add("Value", Me.strValue)

            Return oCondition.XML

        End Function

        Public Function GetFilter(ByVal IDEmployee As Integer, Optional dDate As Date = Nothing) As String

            Return BuildFilterFromCondition(IDEmployee, Me.oUserField.FieldName.Replace("'", "''"), Me.strValue, Me.oUserField.FieldType, Me.oCompare, dDate)

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetXml(ByVal UserFieldConditions As Generic.List(Of roUserFieldCondition)) As String

            Dim oConditions As New roCollection

            oConditions.Add("TotalConditions", UserFieldConditions.Count)

            Dim n As Integer = 1
            For Each oCondition As roUserFieldCondition In UserFieldConditions
                oConditions.Add("Condition" & n.ToString, New roCollection(oCondition.GetXml))
                n += 1
            Next

            Return oConditions.XML

        End Function

        Public Shared Function LoadFromXml(ByVal strXml As String, ByVal oState As roUserFieldState) As Generic.List(Of roUserFieldCondition)

            Dim oRet As New Generic.List(Of roUserFieldCondition)

            If strXml <> "" Then

                Dim oConditions As New roCollection(strXml)

                Dim n As Integer = 1
                Dim oConditionNode As roCollection = oConditions.Node("Condition" & n.ToString)
                While oConditionNode IsNot Nothing
                    oRet.Add(New roUserFieldCondition(oConditionNode, oState))
                    n += 1
                    oConditionNode = oConditions.Node("Condition" & n.ToString)
                End While

            End If

            Return oRet

        End Function

        Public Shared Function LoadFromXml(ByVal strXml As String, ByVal oState As roUserFieldState, ByVal CheckUsedInProcess As Boolean) As Generic.List(Of roUserFieldCondition)

            Dim oRet As New Generic.List(Of roUserFieldCondition)

            If strXml <> "" Then

                Dim oConditions As New roCollection(strXml)

                Dim n As Integer = 1
                Dim oConditionNode As roCollection = oConditions.Node("Condition" & n.ToString)
                While oConditionNode IsNot Nothing
                    oRet.Add(New roUserFieldCondition(oConditionNode, oState, CheckUsedInProcess))
                    n += 1
                    oConditionNode = oConditions.Node("Condition" & n.ToString)
                End While

            End If

            Return oRet

        End Function

        Public Shared Function GetUserFieldConditionFilterGlobal(ByVal FilterList As Generic.List(Of roUserFieldCondition),
                                                                 ByVal FilterListCondition As Generic.List(Of String), ByVal oState As roUserFieldState) As String

            Dim strFilterGlobal As String = String.Empty
            Try
                For IndexItem As Byte = 0 To FilterList.Count - 1
                    Dim ItemField As roUserFieldCondition = FilterList.ElementAt(IndexItem)
                    strFilterGlobal = strFilterGlobal & GetFilterFromField(-1, ItemField)
                    If IndexItem < FilterList.Count - 1 Then
                        strFilterGlobal = strFilterGlobal & " " & FilterListCondition.ElementAt(IndexItem) & " "
                    End If
                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roUserFieldCondition::GetUserFieldConditionFilterGlobal")
            End Try

            If strFilterGlobal <> String.Empty Then
                Dim strDeclare As String = "@DECLARE# @Date smalldatetime SET @Date = " & roTypes.SQLSmallDateTime(DateTime.Today) & "#@#"
                strFilterGlobal = strDeclare & "(" & strFilterGlobal & ")"
            End If

            Return strFilterGlobal

        End Function

        Private Shared Function GetFilterFromField(ByVal IDEmployee As Integer, ByVal ItemField As roUserFieldCondition) As String

            Return BuildFilterFromCondition(IDEmployee, ItemField.UserField.FieldName.Replace("'", "''"),
                                                        ItemField.strValue, ItemField.UserField.FieldType, ItemField.Compare)

        End Function

#End Region

    End Class

    <DataContract>
    <Serializable()>
    Public Class roBusinessCenterFieldDefinition

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roTaskFieldState

        Private intID As Integer
        Private strName As String

        Public Sub New()
            Me.oState = New roTaskFieldState()
            Me.strName = ""
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            Me.oState = New roTaskFieldState(_IDPassport)
            Me.strName = ""
        End Sub

        Public Sub New(ByVal _State As roTaskFieldState, ByVal _ID As Integer,
                       Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intID = _ID
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roTaskFieldState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roTaskFieldState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property

        <DataMember>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroFieldsBusinessCenters WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.Name = oRow("Name")

                    bolRet = True

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{TaskFieldName}", Me.strName, "", 1)
                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tTaskFieldDefinition, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBusinessCenterFieldDefinition::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBusinessCenterFieldDefinition::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oAuditDataOld As DataRow = Nothing
            Dim oAuditDataNew As DataRow = Nothing

            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    oState.Result = UserFieldResultEnum.XSSvalidationError
                    Return False
                End If

                If Me.Validate() Then

                    Dim tb As New DataTable("sysroFieldsBusinessCenters")
                    Dim strSQL As String = "@SELECT# * FROM sysroFieldsBusinessCenters WHERE ID = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)

                    oRow("Name") = Me.strName

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    oAuditDataNew = oRow

                    If bolRet And bAudit Then
                        'bolRet = False
                        '' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tTaskFieldDefinition, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBusinessCenterFieldDefinition::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBusinessCenterFieldDefinition::Save")
            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = False

            Try

                bolRet = True

                If Me.Name = "" Then
                    bolRet = False
                End If

                Dim intCount As Integer = 0
                intCount = Any2Integer(ExecuteScalar("@SELECT# ISNULL(COUNT(*), 0) as Total FROM sysroFieldsBusinessCenters where ID <> " & Me.ID & " AND Name like '" & Me.Name.Replace("'", "''") & "'"))
                If intCount <> 0 Then
                    bolRet = False
                End If

                If Not bolRet Then
                    Me.oState.Result = TaskFieldResultEnum.TaskFieldCannotBeNull
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBusinessCenterFieldDefinition::Validate")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBusinessCenterFieldDefinition::Validate")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

        Public Shared Function SaveBusinessCenterFields(ByVal tbBusinessCenterFields As DataTable, ByVal _State As roTaskFieldState,
                              Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim bolSaved As Boolean = True

                For Each oRow As DataRow In tbBusinessCenterFields.Rows

                    bolSaved = True
                    Select Case oRow.RowState
                        Case DataRowState.Added, DataRowState.Modified

                            Dim oBusinessCenterField = New roBusinessCenterFieldDefinition(_State, oRow("ID"), False)
                            With oBusinessCenterField
                                .Name = oRow("Name")
                            End With
                            bolRet = oBusinessCenterField.Save(bAudit)

                            If Not bolRet Then
                                _State.Result = oBusinessCenterField.oState.Result
                            End If

                        Case Else
                            bolRet = True
                            bolSaved = False
                    End Select

                    If Not bolRet Then
                        Exit For
                    End If

                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roBusinessCenterFieldDefinition::SaveBusinessCenterFields")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roBusinessCenterFieldDefinition::SaveBusinessCenterFields")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function


    End Class

End Namespace