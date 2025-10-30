Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Shift

    <DataContract()>
    Public Class roShiftLayer

        ' El orden de esta enumeracion debe coincidir con el orden
        ' en que se han insertado las keys en la lista arrayXmlKeys
        ' ya que su valor se utiliza como indice de recuperación de la key

        ' Tipos de tratamiento de filtros de incidencias
        Public Const roFilterTreatAsWork = "TreatAsWork"
        Public Const roFilterIgnore = "Ignore"
        Public Const roFilterGenerateIncidence = "Incidence"
        Public Const roFilterGenerateOvertime = "Overtime"

        Private oState As roShiftState

        Private intID As Integer
        Private intIDShift As Integer
        Private intLabelIndex As Integer
        Private intLayerType As roLayerTypes
        Private intParentID As Integer
        Private arrayData As New roCollection

        Public XmlKeys As roXmlLayerKeys ' Sólo se utiliza para serializar la enumeración

        Private aChildLayers As Generic.List(Of roShiftLayer)

#Region "Events"

        Public Sub New()
            Me.oState = New roShiftState()
            arrayData = New roCollection
        End Sub

        Public Sub New(ByVal _State As roShiftState)
            Me.oState = _State
            arrayData = New roCollection
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property IDShift() As Integer
            Get
                Return intIDShift
            End Get
            Set(ByVal value As Integer)
                intIDShift = value
                If Not aChildLayers Is Nothing Then
                    For Each oSL As roShiftLayer In aChildLayers
                        oSL.IDShift = value
                    Next
                End If
            End Set
        End Property

        <DataMember()>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
            End Set
        End Property

        <DataMember()>
        Public Property LabelIndex() As Integer
            Get
                Return intLabelIndex
            End Get
            Set(ByVal value As Integer)
                intLabelIndex = value
            End Set
        End Property
        <DataMember()>
        Public Property LayerType() As roLayerTypes
            Get
                Return intLayerType
            End Get
            Set(ByVal value As roLayerTypes)
                intLayerType = value
            End Set
        End Property
        <DataMember()>
        Public Property ParentID() As Integer
            Get
                Return intParentID
            End Get
            Set(ByVal value As Integer)
                intParentID = value
            End Set
        End Property

        <XmlIgnore()>
        <IgnoreDataMember()>
        Public Property Data() As roCollection
            Get
                Return arrayData
            End Get
            Set(ByVal value As roCollection)
                arrayData = value
            End Set
        End Property
        <DataMember()>
        Public Property DataStoredXML() As String
            Get
                Dim strXML As String = ""
                If Me.arrayData IsNot Nothing Then
                    strXML = Me.arrayData.XML()
                End If
                Return strXML
            End Get
            Set(ByVal value As String)
                Me.arrayData = New roCollection(value)
            End Set
        End Property
        <DataMember()>
        Public ReadOnly Property XmlKey() As String()
            Get
                Return roShiftEngineLayer.arrayXmlKeys
            End Get
        End Property
        <DataMember()>
        Public Property ChildLayers() As Generic.List(Of roShiftLayer)
            Get
                Return aChildLayers
            End Get
            Set(ByVal value As Generic.List(Of roShiftLayer))
                aChildLayers = value
            End Set
        End Property
        <IgnoreDataMember()>
        Public Property State() As roShiftState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roShiftState)
                Me.oState = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub ParseLayerData(ByVal Data As String)
            arrayData.Clear()
            arrayData.LoadXMLString(Data)
        End Sub

        Public Function Save(ByRef intLayerCounter As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strSql As String

                intLayerCounter = intLayerCounter + 1
                strSql = " @INSERT# INTO sysroShiftsLayers "
                strSql = strSql & " ( IDShift, ID, IDType, Definition, ParentLayerID ) "
                strSql = strSql & " Values "
                strSql = strSql & "( " & Me.IDShift & ", " & Me.ID & ", "
                strSql = strSql & Me.LayerType & ", '" & Me.DataStoredXML & "', "
                strSql = strSql & IIf(Me.ParentID = -1, 0, Me.ParentID)
                strSql = strSql & " ) "

                bolRet = ExecuteSql(strSql)
                If bolRet Then
                    If bAudit Then

                        Dim strQueryRow As String
                        Dim oShiftLayerNew As DataRow = Nothing

                        strQueryRow = "@SELECT# * " &
                                      "FROM sysroShiftsLayers " &
                                      "WHERE IDShift = " & Me.IDShift & " AND " &
                                            "[ID] = " & Me.ID
                        Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "sysroShiftsLayers")
                        If tbAuditNew.Rows.Count = 1 Then oShiftLayerNew = tbAuditNew.Rows(0)

                        ' Auditmos inserción capa horario
                        Dim tbParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbParameters, oShiftLayerNew, Nothing)
                        Dim strLayerType As String = System.Enum.GetName(GetType(roLayerTypes), Me.LayerType)
                        oState.AddAuditParameter(tbParameters, "{ShiftLayerType}", strLayerType, "", 1)
                        oState.AddAuditParameter(tbParameters, "{ShiftName}", Me.IDShift, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tShiftLayer, Me.ID & " " & strLayerType & " (" & Me.IDShift & ")", tbParameters, -1)
                    End If
                Else
                    bolRet = False ' Error en la grabación
                End If

                If bolRet = True Then
                    If Me.ChildLayers IsNot Nothing Then
                        For Each oLayer As roShiftLayer In Me.ChildLayers
                            intLayerCounter = intLayerCounter + 1
                            bolRet = oLayer.Save(intLayerCounter)
                            If bolRet = False Then Exit For
                        Next
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShiftLayer::SaveLayer")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftLayer::SaveLayer")
            End Try

            Return bolRet
        End Function

        Public Function ParseLayer() As String

            Dim strRet As String = ""

            Dim sText As String = ""

            Try

                Dim aLayer As roCollection = Me.arrayData
                Dim aParentLayer As New roCollection

                If Me.ChildLayers IsNot Nothing AndAlso Me.ChildLayers.Count > 0 Then
                    aParentLayer = Me.ChildLayers(0).Data
                End If

                Select Case Me.LayerType
                    Case 1200 'descanso
                        sText = roShift.ProcessString("CRUFLCOM.Shifts.BreakLayer", Me.oState) & vbNewLine
                        sText = sText & roShift.ProcessString("CRUFLCOM.Shifts.From", Me.oState) & " " & Any2Time(aLayer.Item("Begin", roCollection.roSearchMode.roByKey)).TimeOnly & " " & roShift.ProcessString("CRUFLCOM.Shifts.To", Me.oState) & " " & Any2Time(aLayer.Item("Finish", roCollection.roSearchMode.roByKey)).TimeOnly

                    Case 1100 'obligada

                        'Inicio
                        sText = roShift.ProcessString("CRUFLCOM.Shifts.RigidLayer", Me.oState) & vbNewLine
                        If aLayer.Exists("FloatingBeginUpto") Then
                            sText = sText & roShift.ProcessString("CRUFLCOM.Shifts.InBetween", Me.oState) & " " & Any2Time(aLayer.Item("Begin", roCollection.roSearchMode.roByKey)).TimeOnly & " " & roShift.ProcessString("CRUFLCOM.Shifts.And", Me.oState) & " " & aLayer.Item("FloatingBeginUpto", roCollection.roSearchMode.roByKey)
                        Else
                            sText = sText & roShift.ProcessString("CRUFLCOM.Shifts.InMove", Me.oState) & " " & Any2Time(aLayer.Item("Begin", roCollection.roSearchMode.roByKey)).TimeOnly
                        End If

                        'Fin
                        sText = sText & vbNewLine
                        If aLayer.Exists("FloatingFinishMinutes") Then
                            sText = sText & roShift.ProcessString("CRUFLCOM.Shifts.OutBetween", Me.oState) & " " & aLayer.Item("FloatingFinishMinutes", roCollection.roSearchMode.roByKey) / 60 & " " & roShift.ProcessString("CRUFLCOM.Shifts.After", Me.oState)
                        Else
                            sText = sText & roShift.ProcessString("CRUFLCOM.Shifts.OutMove", Me.oState) & " " & Any2Time(aLayer.Item("Finish", roCollection.roSearchMode.roByKey)).TimeOnly
                        End If

                    Case 1000 'flexibles
                        'Inicio
                        sText = roShift.ProcessString("CRUFLCOM.Shifts.FlexibleLayer", Me.oState) & vbNewLine
                        sText = sText & roShift.ProcessString("CRUFLCOM.Shifts.Present", Me.oState) & " " & Any2Time(aLayer.Item("Begin", roCollection.roSearchMode.roByKey)).TimeOnly & " " & roShift.ProcessString("CRUFLCOM.Shifts.And", Me.oState)
                        sText = sText & " " & Any2Time(aLayer.Item("Finish", roCollection.roSearchMode.roByKey)).TimeOnly & vbNewLine
                        sText = sText & roShift.ProcessString("CRUFLCOM.Shifts.PresentBetween", Me.oState) & " " & aParentLayer.Item("MinTime", roCollection.roSearchMode.roByKey) & " " & roShift.ProcessString("CRUFLCOM.Shifts.AndOnly", Me.oState) & " " & aParentLayer.Item("MaxTime", roCollection.roSearchMode.roByKey)
                        sText = sText & " " & roShift.ProcessString("CRUFLCOM.Shifts.Hours", Me.oState)
                End Select

                If Len(sText) > 0 Then
                    strRet = sText & vbNewLine
                Else
                    strRet = ""
                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roShiftLayer::ParseLayers")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShiftLayer::ParseLayers")
            End Try

            Return strRet

        End Function

#End Region

    End Class

End Namespace