Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Concept

    <DataContract>
    Public Class roConceptComposition

#Region "Declarations - Constructor"

        Private oState As roConceptState

        Private intID As Integer

        Private intIDConcept As Integer
        Private intIDCause As Integer
        Private oConditions As New Generic.List(Of roConceptCondition)
        Private oFactorType As ValueType
        Private dblFactorValue As Double
        Private strFactorUserField As String

        Private intIDShift As Integer
        Private oIDType As CompositionType
        Private oTypeDayPlanned As TypeDayPlanned
        Private strCompositionUserField As String

        Public Sub New()

            Me.oState = New roConceptState

            Me.intIDConcept = -1
            Me.intIDCause = -1

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roConceptState)

            Me.oState = _State

            Me.intID = _ID
            Me.intIDCause = -1

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roConceptState, Optional ByVal bAudit As Boolean = False)

            Me.oState = _State

            Me.intID = _ID

            Me.Load(bAudit)

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal strXml As String, ByVal _FactorValue As Double)

            Me.oState = New roConceptState

            Me.intID = _ID

            Me.Load(False)

            Me.LoadFromXml(strXml, _FactorValue)

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roConceptState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roConceptState)
                Me.oState = value
                If Me.oConditions IsNot Nothing Then
                    For Each _Condition As roConceptCondition In Me.oConditions
                        _Condition.State = Me.oState
                    Next
                End If
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
        Public Property IDConcept() As Integer
            Get
                Return Me.intIDConcept
            End Get
            Set(ByVal value As Integer)
                Me.intIDConcept = value
            End Set
        End Property

        <DataMember>
        Public Property IDCause() As Integer
            Get
                Return Me.intIDCause
            End Get
            Set(ByVal value As Integer)
                Me.intIDCause = value
            End Set
        End Property

        <DataMember>
        Public Property Conditions() As Generic.List(Of roConceptCondition)
            Get
                Return Me.oConditions
            End Get
            Set(ByVal value As Generic.List(Of roConceptCondition))
                Me.oConditions = value
            End Set
        End Property

        <DataMember>
        Public Property FactorType() As ValueType
            Get
                Return Me.oFactorType
            End Get
            Set(ByVal value As ValueType)
                Me.oFactorType = value
            End Set
        End Property

        <DataMember>
        Public Property FactorValue() As Double
            Get
                Return Me.dblFactorValue
            End Get
            Set(ByVal value As Double)
                Me.dblFactorValue = value
            End Set
        End Property

        <DataMember>
        Public Property FactorUserField() As String
            Get
                Return Me.strFactorUserField
            End Get
            Set(ByVal value As String)
                Me.strFactorUserField = value
            End Set
        End Property

        <DataMember>
        Public Property IDShift() As Integer
            Get
                Return Me.intIDShift
            End Get
            Set(ByVal value As Integer)
                Me.intIDShift = value
            End Set
        End Property

        <DataMember>
        Public Property IDType() As CompositionType
            Get
                Return Me.oIDType
            End Get
            Set(ByVal value As CompositionType)
                Me.oIDType = value
            End Set
        End Property

        <DataMember>
        Public Property TypeDayPlanned() As TypeDayPlanned
            Get
                Return Me.oTypeDayPlanned
            End Get
            Set(ByVal value As TypeDayPlanned)
                Me.oTypeDayPlanned = value
            End Set
        End Property

        <DataMember>
        Public Property CompositionUserField() As String
            Get
                Return Me.strCompositionUserField
            End Get
            Set(ByVal value As String)
                Me.strCompositionUserField = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# ConceptCauses.*, Concepts.IDType AS ConceptType, isnull(Concepts.CustomType,0) as CustomType  " &
                                       "FROM ConceptCauses INNER JOIN Concepts " &
                                                "ON ConceptCauses.IDConcept = Concepts.ID " &
                                       "WHERE ConceptCauses.ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Dim strXml As String = Any2String(oRow("Composition"))

                    Me.intIDConcept = roTypes.Any2Integer(oRow("IDConcept"))
                    Me.intIDCause = roTypes.Any2Integer(oRow("IDCause"))

                    If oRow("IDShift") IsNot DBNull.Value Then
                        Me.intIDShift = roTypes.Any2Integer(oRow("IDShift"))
                    Else
                        Me.intIDShift = 0
                    End If

                    Me.oIDType = roTypes.Any2Integer(oRow("IDType"))
                    Me.oTypeDayPlanned = roTypes.Any2Integer(oRow("TypeDayPlanned"))

                    If oRow("FieldFactor") IsNot DBNull.Value Then
                        Me.strCompositionUserField = roTypes.Any2String(oRow("FieldFactor"))
                    Else
                        Me.strCompositionUserField = ""
                    End If

                    ' En función del tipo de acumulado, obtenemos el nombre del campo donde hay el valor del factor directo
                    Dim strFactorField As String = "HoursFactor"
                    ' En el caso de que el acumulado sea de dias o personalziado miramos el campo relacionado
                    If Any2String(oRow("ConceptType")) = "O" Or Any2Boolean(oRow("CustomType")) Then
                        strFactorField = "OccurrencesFactor"
                    End If

                    If strXml <> "" Then
                        ' Añadimos la composición a la colección
                        Me.LoadFromXml(strXml, Any2Double(oRow(strFactorField)))
                    Else
                        ' Añadimos la composición básica a la colección (para compatibilidad con veriones anteriores)
                        Me.oFactorType = ValueType.DirectValue
                        Me.dblFactorValue = Any2Double(oRow(strFactorField))
                    End If

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tConceptComposition, "", tbParameters, -1)
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roConceptComposition::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptComposition::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Private Sub LoadFromXml(ByVal strXml As String, ByVal _FactorValue As Double)

            If strXml <> "" Then

                ' Añadimos la composición a la colección
                Dim oComposition As New roCollection(strXml)

                Dim n As Integer = 1
                Dim oConditionNode As roCollection = oComposition.Node("Condition" & n.ToString)
                While oConditionNode IsNot Nothing
                    Me.oConditions.Add(New roConceptCondition(oConditionNode, Me.oState))
                    n += 1
                    oConditionNode = oComposition.Node("Condition" & n.ToString)
                End While

                Me.oFactorType = oComposition.Item("FactorType")
                Select Case Me.oFactorType
                    Case ValueType.DirectValue
                        Me.dblFactorValue = _FactorValue ' Any2Double(oComposition.Item("FactorValue"))
                    Case ValueType.UserField
                        Me.strFactorUserField = Any2String(oComposition.Item("FactorUserField"))
                End Select

            End If

        End Sub

        Public Function Validate() As Boolean
            Dim bolRet As Boolean = True

            Try

                ' Verificamos que la configuración del factor sea correcta
                Select Case Me.oFactorType
                    Case ValueType.DirectValue
                        ' Verificamos que el valor directo especificado sea distinto a zero
                        If Me.strCompositionUserField = String.Empty Then
                            If Me.dblFactorValue = 0 Then
                                bolRet = False
                                Me.oState.Result = ConceptResultEnum.CompositionFactorValueInvalid
                            End If
                        End If
                    Case ValueType.UserField
                        ' Verificamos que el campo de la ficha exista y tenga una tipo numérico
                        Dim oUserField As New VTUserFields.UserFields.roUserField
                        oUserField.FieldName = Me.strFactorUserField
                        If oUserField.Load(False, False) Then
                            If Not (oUserField.FieldType = FieldTypes.tNumeric Or oUserField.FieldType = FieldTypes.tDecimal) Then
                                bolRet = False
                                Me.oState.Result = ConceptResultEnum.CompositionFactorUserFieldInvalidDataType
                            End If
                        Else
                            bolRet = False
                            Me.oState.Result = ConceptResultEnum.CompositionFactorUserFieldRequired
                        End If
                    Case ValueType.IDCause
                        bolRet = False
                        Me.oState.Result = ConceptResultEnum.CompositionFactorIDCauseNotAllowed
                End Select

                If bolRet And Me.oConditions IsNot Nothing Then

                    For Each oCondition As roConceptCondition In Me.oConditions
                        bolRet = oCondition.Validate()
                        If Not bolRet Then Exit For
                    Next

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConceptComposition::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptComposition::Validate")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim strFactorField As String = "HoursFactor"
                    Dim tbConcepts As DataTable = CreateDataTable("@SELECT# IDType FROM Concepts WHERE ID = " & Me.IDConcept.ToString)
                    If tbConcepts IsNot Nothing AndAlso tbConcepts.Rows.Count = 1 Then
                        If tbConcepts.Rows(0).Item("IDType") = "O" Then strFactorField = "OccurrencesFactor"
                    End If

                    tbConcepts = CreateDataTable("@SELECT# isnull(CustomType,0) as CustomType  FROM Concepts WHERE ID = " & Me.IDConcept.ToString)
                    If tbConcepts IsNot Nothing AndAlso tbConcepts.Rows.Count = 1 Then
                        If Any2Boolean(tbConcepts.Rows(0).Item("CustomType")) Then strFactorField = "OccurrencesFactor"
                    End If

                    Dim tb As New DataTable("ConceptCauses")
                    Dim strSQL As String = "@SELECT# * FROM ConceptCauses " &
                                           "WHERE ID = " & Me.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                    Else
                        oRow = tb.Rows(0)
                    End If

                    oRow("IDCause") = Me.IDCause
                    oRow("IDConcept") = Me.IDConcept
                    oRow("IDShift") = Me.IDShift
                    oRow("FieldFactor") = Me.strCompositionUserField
                    oRow("IDType") = Me.oIDType
                    oRow("TypeDayPlanned") = Me.oTypeDayPlanned

                    oRow("HoursFactor") = 0
                    oRow("OccurrencesFactor") = 0
                    oRow(strFactorField) = Me.dblFactorValue
                    oRow("Composition") = Me.GetXml()

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    oAuditDataNew = oRow

                    If bolRet And bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String = ""
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tConceptComposition, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConceptComposition::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptComposition::Save")
            End Try

            Return bolRet

        End Function

        Public Function GetXml() As String

            Dim oComposition As New roCollection

            If Me.oConditions Is Nothing Then
                oConditions = New List(Of roConceptCondition)
            End If

            oComposition.Add("TotalConditions", Me.oConditions.Count)

            Dim n As Integer = 1
            For Each oCondition As roConceptCondition In Me.oConditions
                oComposition.Add("Condition" & n.ToString, New roCollection(oCondition.GetXml))
                n += 1
            Next

            oComposition.Add("FactorType", Me.oFactorType)

            Select Case Me.oFactorType
                Case ValueType.DirectValue
                    'oComposition.Add("FactorValue", Me.dblFactorValue)
                Case ValueType.UserField
                    oComposition.Add("FactorUserField", Me.strFactorUserField)
            End Select

            Return oComposition.XML

        End Function

#Region "Helper methods"

        Public Shared Function GetConceptCompositions(ByVal _IDConcept As Integer, ByVal _State As roConceptState, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roConceptComposition)

            Dim oRet As New Generic.List(Of roConceptComposition)

            Try

                Dim strSQL As String = "@SELECT# ConceptCauses.*, Concepts.IDType AS ConceptType, isnull(Concepts.CustomType,0) as CustomType  " &
                                       "FROM ConceptCauses INNER JOIN Concepts " &
                                                "ON ConceptCauses.IDConcept = Concepts.ID " &
                                       "WHERE ConceptCauses.IDConcept = " & _IDConcept.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                Dim strCompositionXml As String
                Dim strFactorField As String
                For Each oCompositionRow As DataRow In tb.Rows

                    ' En función del tipo de acumulado, obtenemos el nombre del campo donde hay el valor del factor directo
                    If Any2String(oCompositionRow("ConceptType")) = "H" And Not Any2Boolean(oCompositionRow("CustomType")) Then
                        strFactorField = "HoursFactor"
                    Else
                        strFactorField = "OccurrencesFactor"
                    End If

                    strCompositionXml = Any2String(oCompositionRow("Composition"))
                    If strCompositionXml <> "" Then
                        ' Añadimos la composición a la colección
                        oRet.Add(New roConceptComposition(oCompositionRow("ID"), strCompositionXml, oCompositionRow(strFactorField)))
                    Else
                        ' Añadimos la composición básica a la colección (para compatibilidad con veriones anteriores)
                        Dim oBasicComposition As New roConceptComposition(oCompositionRow("ID"), _State, bAudit)
                        oRet.Add(oBasicComposition)
                    End If
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConceptComposition::GetConceptCompositions")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConceptComposition::GetConceptCompositions")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveConceptCompositions(ByVal oCompositions As Generic.List(Of roConceptComposition), ByVal _State As roConceptState) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strIDConcepts As String = ""
                If oCompositions IsNot Nothing Then
                    For Each oComposition As roConceptComposition In oCompositions
                        strIDConcepts &= "," & oComposition.IDConcept.ToString
                    Next
                    If strIDConcepts <> "" Then strIDConcepts = strIDConcepts.Substring(1)

                    Dim strSQL As String = "@DELETE# FROM ConceptCauses WHERE IDConcept IN (" & strIDConcepts & ")"
                    bolRet = ExecuteSql(strSQL)

                    If bolRet Then
                        For Each oComposition As roConceptComposition In oCompositions
                            'Ponemos el id a -1 para asegurarnos que lo guarda como nuevo, ya que los hemos borrado todos anteriormente
                            oComposition.ID = -1
                            bolRet = oComposition.Save()
                            If Not bolRet Then Exit For
                        Next
                    End If
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConceptComposition::SaveConceptCompositions")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConceptComposition::SaveConceptCompositions")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteConceptCompositions(ByVal _IDConcept As Integer, ByVal _State As roConceptState, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@DELETE# FROM ConceptCauses WHERE IDConcept = " & _IDConcept.ToString
                bolRet = ExecuteSql(strSQL)

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = _State.Audit(Audit.Action.aDelete, Audit.ObjectType.tConceptComposition, "", Nothing, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConceptComposition::DeleteConceptCompositions")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConceptComposition::DeleteConceptCompositions")
            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateConceptCompositions(ByVal oCompositions As Generic.List(Of roConceptComposition), ByVal _State As roConceptState) As Boolean
            Dim bolRet As Boolean = False

            Try

                ' Verificar que
                If oCompositions.Count > 0 Then
                    For Each oComposition As roConceptComposition In oCompositions
                        bolRet = oComposition.Validate()
                        If Not bolRet Then Exit For
                    Next
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roConceptComposition::ValidateConceptCompositions")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roConceptComposition::ValidateConceptCompositions")
            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

    <DataContract>
    Public Class roConceptCondition

#Region "Declarations - Constructor"

        Private oState As roConceptState

        Private oIDCauses As Generic.List(Of roConceptConditionCause) ' IDCause1~+|-, IDCause2~+|-, ...
        Private oCompare As ConditionCompareType
        Private oValueType As ValueType
        Private xValue_Direct As DateTime
        Private intValue_IDCause As Integer
        Private strValue_UserField As String

        Public Sub New()

            Me.oState = New roConceptState
            Me.oIDCauses = New Generic.List(Of roConceptConditionCause)

        End Sub

        Public Sub New(ByVal _State As roConceptState)

            Me.oState = _State
            Me.oIDCauses = New Generic.List(Of roConceptConditionCause)

        End Sub

        Public Sub New(ByVal oCondition As roCollection, ByVal _State As roConceptState)

            Me.oState = _State
            Me.oIDCauses = New Generic.List(Of roConceptConditionCause)

            If oCondition IsNot Nothing Then
                Dim strNode As String = Any2String(oCondition.Item("IDCauses"))
                For Each strCause As String In strNode.Split("~")
                    Me.oIDCauses.Add(New roConceptConditionCause(strCause.Substring(0, strCause.Length - 1), strCause.Substring(strCause.Length - 1)))
                Next
                Me.oCompare = Any2Integer(oCondition.Item("Compare"))
                Me.oValueType = Any2Integer(oCondition.Item("ValueType"))
                Select Case Me.oValueType
                    Case ValueType.DirectValue
                        If oCondition.Item("Value_Direct") IsNot Nothing Then
                            Me.xValue_Direct = Any2Time(oCondition.Item("Value_Direct")).Value
                        End If
                    Case ValueType.IDCause
                        Me.intValue_IDCause = Any2Integer(oCondition.Item("Value_IDCause"))
                    Case ValueType.UserField
                        Me.strValue_UserField = Any2String(oCondition.Item("Value_UserField"))
                End Select
            End If

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roConceptState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roConceptState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property IDCauses() As Generic.List(Of roConceptConditionCause)
            Get
                Return Me.oIDCauses
            End Get
            Set(ByVal value As Generic.List(Of roConceptConditionCause))
                Me.oIDCauses = value
            End Set
        End Property

        <DataMember>
        Public Property Compare() As ConditionCompareType
            Get
                Return Me.oCompare
            End Get
            Set(ByVal value As ConditionCompareType)
                Me.oCompare = value
            End Set
        End Property

        <DataMember>
        Public Property Value_Type() As ValueType
            Get
                Return Me.oValueType
            End Get
            Set(ByVal value As ValueType)
                Me.oValueType = value
            End Set
        End Property

        <DataMember>
        Public Property Value_Direct() As DateTime
            Get
                Return Me.xValue_Direct
            End Get
            Set(ByVal value As DateTime)
                Me.xValue_Direct = value
            End Set
        End Property

        <DataMember>
        Public Property Value_IDCause() As Integer
            Get
                Return Me.intValue_IDCause
            End Get
            Set(ByVal value As Integer)
                Me.intValue_IDCause = value
            End Set
        End Property

        <DataMember>
        Public Property Value_UserField() As String
            Get
                Return Me.strValue_UserField
            End Get
            Set(ByVal value As String)
                Me.strValue_UserField = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function GetXml() As String

            Dim oCondition As New roCollection

            Dim strIDCauses As String = ""
            If Me.oIDCauses IsNot Nothing Then
                For Each oCause As roConceptConditionCause In Me.oIDCauses
                    strIDCauses &= "~" & oCause.IDCause.ToString & oCause.Operation
                Next
                If strIDCauses <> "" Then strIDCauses = strIDCauses.Substring(1)
            End If
            oCondition.Add("IDCauses", strIDCauses)
            oCondition.Add("Compare", Me.oCompare)
            oCondition.Add("ValueType", Me.oValueType)
            Select Case Me.oValueType
                Case ValueType.DirectValue
                    oCondition.Add("Value_Direct", Any2Time(Me.xValue_Direct).TimeOnly)
                Case ValueType.IDCause
                    oCondition.Add("Value_IDCause", Me.intValue_IDCause)
                Case ValueType.UserField
                    oCondition.Add("Value_UserField", Me.strValue_UserField)
            End Select

            Return oCondition.XML

        End Function

        Public Function Validate() As Boolean
            Dim bolRet As Boolean = True

            Try

                ' Verificamos que la justificaciones sean correctas

                Dim oCause As Cause.roCause
                If Me.oIDCauses IsNot Nothing AndAlso Me.oIDCauses.Count > 0 Then
                    For Each oConditionCause As roConceptConditionCause In Me.oIDCauses
                        oCause = New Cause.roCause(oConditionCause.IDCause, New Cause.roCauseState(Me.oState.IDPassport), False)
                        If oCause.ID < 0 Then
                            bolRet = False
                            Me.oState.Result = ConceptResultEnum.CompositionConditionIDCauseInvalid
                            Exit For
                        End If
                    Next
                Else
                    bolRet = False
                    Me.oState.Result = ConceptResultEnum.CompositionConditionIDCauseRequired
                End If

                If bolRet Then

                    ' Verificamos en función del tipo de valor
                    Select Case Me.oValueType
                        Case ValueType.DirectValue
                            ' Verificamos que el valor directo especificado sea distinto a zero
                            ''If Me.xValue_Direct = Nothing Then
                            ''    bolRet = False
                            ''    Me.oState.Result = conceptResultEnum.CompositionConditionDirectValueInvalid
                            ''End If
                        Case ValueType.UserField
                            ' Verificamos que el campo de la ficha exista y tenga una tipo 'tiempo'
                            Dim oUserField As New VTUserFields.UserFields.roUserField
                            oUserField.FieldName = Me.strValue_UserField
                            If oUserField.Load(False, False) Then
                                If Not oUserField.FieldType = FieldTypes.tTime Then
                                    bolRet = False
                                    Me.oState.Result = ConceptResultEnum.CompositionConditionUserFieldValueInvalidDataType
                                End If
                            Else
                                bolRet = False
                                Me.oState.Result = ConceptResultEnum.CompositionConditionUserFieldValueRequired
                            End If
                        Case ValueType.IDCause
                            ' Verificamos que la justificación exista
                            oCause = New Cause.roCause(Me.intValue_IDCause, New Cause.roCauseState(Me.oState.IDPassport), False)
                            If oCause.ID < 0 Then
                                bolRet = False
                                Me.oState.Result = ConceptResultEnum.CompositionConditionIDCauseValueInvalid
                            Else
                                ' Verificamos que la justificación no esté en la lista de justificaciones de la parte inicial de la comparación
                                For Each oConditionCause As roConceptConditionCause In Me.oIDCauses
                                    If oConditionCause.IDCause = Me.intValue_IDCause Then
                                        bolRet = False
                                        Me.oState.Result = ConceptResultEnum.CompositionConditionIDCausesMustBeDiferent
                                        Exit For
                                    End If
                                Next
                            End If
                    End Select

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roConceptCondition::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptCondition::Validate")
            End Try

            Return bolRet

        End Function

#End Region

        <DataContract>
        Public Class roConceptConditionCause

            <DataMember>
            Public IDCause As Integer
            <DataMember>
            Public Operation As Char

            Public Sub New()

            End Sub

            Public Sub New(ByVal _IDCause As Integer, ByVal _Operation As Char)
                Me.IDCause = _IDCause
                Me.Operation = _Operation
            End Sub

        End Class

    End Class

End Namespace