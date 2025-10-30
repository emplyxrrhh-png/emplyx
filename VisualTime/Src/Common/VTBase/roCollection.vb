Imports System.Runtime.Serialization

'==================================================================
' roCOLLECTION
'
' roCollection es una Collection avanzada con soporte para
'   representar árboles y guardarse o recuperarse del registro de
'   Windows o de XML.
'==================================================================
<DataContract>
<Serializable>
Public Class roCollection

    ' Variable local para contener colección
    Private mCol As Collection
    Private mKeyCol As Collection
    Private mIndexCol As Collection

    Private mDupsBehavior As roDuplicatesBehavior

    ' Tipos de busquedas
    Public Enum roSearchMode
        <EnumMember> roByKey
        <EnumMember> roByIndex
    End Enum

    Public Enum roWhatToSearch
        <EnumMember> roSearchKeyByIndex
        <EnumMember> roSearchKeyByItem
    End Enum

    Public Enum roCollectionBehavior
        <EnumMember> roCollection
        <EnumMember> roTree
    End Enum

    Public Enum roDuplicatesBehavior
        <EnumMember> roAllowDuplicates
        <EnumMember> roReplaceDuplicates
        <EnumMember> roIgnoreduplicates
        <EnumMember> roErrorOnDuplicates
    End Enum

    ' Constantes de XML
    Private Const XMLBASEPROP = "roCollection"
    Private Const XMLBASEVERSION = "2.0"
    Private Const XMLBASE = "<?xml version=""1.0""?><" & XMLBASEPROP & " version=""" & XMLBASEVERSION & """" & "></" & XMLBASEPROP & ">"
    Private Const XMLTYPE_ROTIME = 201
    Private Const XMLTYPE_ROCOLLECTION = 200

    ' Inicializa colección
    Public Sub New()
        mCol = New Collection
        mKeyCol = New Collection
        mIndexCol = New Collection
        mDupsBehavior = roDuplicatesBehavior.roReplaceDuplicates
    End Sub

    ' Inicializa colección
    Public Sub New(ByVal strXml As String)
        mCol = New Collection
        mKeyCol = New Collection
        mIndexCol = New Collection
        mDupsBehavior = roDuplicatesBehavior.roReplaceDuplicates
        If strXml <> "" Then Me.LoadXMLString(strXml)
    End Sub

    ' Inicializa colección
    Public Sub New(ByVal strXml As String, ByRef oCultureInfo As Globalization.CultureInfo)
        mCol = New Collection
        mKeyCol = New Collection
        mIndexCol = New Collection
        mDupsBehavior = roDuplicatesBehavior.roReplaceDuplicates
        If strXml <> "" Then Me.LoadXMLString(strXml, oCultureInfo)
    End Sub

    ' Añade la clave de un objeto que acabamos de añadir
    Private Sub Add_Really(ByRef Obj As Object, ByVal Key As String)
        mCol.Add(Obj, Key)
        mKeyCol.Add(Key)
        mIndexCol.Add(mCol.Count, Key)
    End Sub

    ' Convierte un variant a un string para hacer de clave
    Private Function Any2Key(ByVal Key As Object) As String
        If VarType(Key) <> vbString Then
            Try
                Any2Key = CStr(Key)
            Catch
                Any2Key = roSupport.GetGUID()
            End Try
        Else
            Any2Key = Key
        End If
    End Function

    ' Construye codigo XML de un roCollection recursivamente
    Private Sub BuildXMLElements(ByVal XMLDoc As Xml.XmlDocument, ByVal XMLElement As Xml.XmlElement, ByVal Node As roCollection, ByVal Depth As Integer)
        Dim Index As Integer
        Dim newElement As Xml.XmlElement
        Dim NodeTag As String = String.Empty
        Dim Text As String = String.Empty

        Select Case Depth
            Case 1 : NodeTag = "Item"
            Case 2 : NodeTag = "SubItem"
            Case Is > 2 : NodeTag = "SubItem" & Depth
        End Select

        For Index = 1 To Node.Count
            newElement = XMLDoc.CreateElement(NodeTag)
            newElement.SetAttribute("key", Node.Key(Index))

            Select Case VarType(Node.Item(Index, roSearchMode.roByIndex))
                Case vbEmpty, vbNull, vbInteger, vbLong, vbSingle, vbDouble, vbCurrency, vbDate, vbString, vbVariant, vbBoolean, vbDecimal, vbByte
                    ' El nodo es un valor, añade como texto
                    If VarType(Node.Item(Index, roSearchMode.roByIndex)) = VariantType.Date Then
                        ' Si el valor es de tipo fecha, la grabamos en formato yyyy/MM/dd HH:mm:ss, para poder recuperarlo correctamente independientemente del idioma configurado
                        If CDate(Node.Item(Index, roSearchMode.roByIndex)).Date = Nothing Then
                            Text = Format(CDate(Node.Item(Index, roSearchMode.roByIndex)), "HH:mm:ss")
                        Else
                            Text = Format(CDate(Node.Item(Index, roSearchMode.roByIndex)), "yyyy/MM/dd HH:mm:ss")
                        End If
                    ElseIf VarType(Node.Item(Index, roSearchMode.roByIndex)) = VariantType.Double OrElse
                        VarType(Node.Item(Index, roSearchMode.roByIndex)) = VariantType.Decimal Then

                        Dim floatValue As String = roConversions.Any2String(Node.Item(Index, roSearchMode.roByIndex))
                        Text = floatValue.Replace(Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".")
                    Else

                        Text = roConversions.Any2String(Node.Item(Index, roSearchMode.roByIndex))
                    End If
                    Text = roSupport.StringEncodeControlChars(Text)
                    newElement.InnerText = Text
                    newElement.SetAttribute("type", roConversions.VarTypeVBNET2VB6(VarType(Node.Item(Index, roSearchMode.roByIndex))))

                Case vbObject
                    If TypeOf Node.Item(Index, roSearchMode.roByIndex) Is roCollection Then
                        ' El nodo es otro arbol, añade hijos
                        newElement.SetAttribute("type", XMLTYPE_ROCOLLECTION)
                        BuildXMLElements(XMLDoc, newElement, Node.Item(Index, roSearchMode.roByIndex), Depth + 1)
                    ElseIf TypeOf Node.Item(Index, roSearchMode.roByIndex) Is roTime Then
                        ' El nodo es un roTime, convierte a fecha/hora
                        Text = roConversions.Any2String(Node.Item(Index, roSearchMode.roByIndex).Value)
                        Text = roSupport.StringEncodeControlChars(Text)
                        newElement.SetAttribute("type", XMLTYPE_ROTIME)
                        newElement.InnerText = Text
                    ElseIf Node.Exists("ReportParameters") Then
                        'Lo aceptamos
                    Else
                        ' El nodo es un objeto desconocido
                        Err.Raise(9083, "roCollection", "Unsupported object type for XML export.")
                    End If
                Case Else
                    Err.Raise(9083, "roCollection", "Unsupported type (" & VarType(Node.Item(Index, roSearchMode.roByIndex)) & ") for XML export.")
            End Select

            ' Añade nuevo elemento al documento
            XMLElement.AppendChild(newElement)
        Next
    End Sub

    ' Construye codigo XML de un roCollection recursivamente teniendo en cuenta la cultura recibida
    Private Sub BuildXMLElements(ByVal XMLDoc As Xml.XmlDocument, ByVal XMLElement As Xml.XmlElement, ByVal Node As roCollection, ByVal Depth As Integer,
                                 ByRef oCultureInfo As Globalization.CultureInfo)
        Dim Index As Integer
        Dim newElement As Xml.XmlElement
        Dim NodeTag As String = String.Empty
        Dim Text As String = String.Empty

        Select Case Depth
            Case 1 : NodeTag = "Item"
            Case 2 : NodeTag = "SubItem"
            Case Is > 2 : NodeTag = "SubItem" & Depth
        End Select

        For Index = 1 To Node.Count
            newElement = XMLDoc.CreateElement(NodeTag)
            newElement.SetAttribute("key", Node.Key(Index))

            Select Case VarType(Node.Item(Index, roSearchMode.roByIndex))
                Case vbEmpty, vbNull, vbInteger, vbLong, vbSingle, vbDouble, vbCurrency, vbDate, vbString, vbVariant, vbBoolean, vbDecimal, vbByte
                    ' El nodo es un valor, añade como texto
                    If VarType(Node.Item(Index, roSearchMode.roByIndex)) = VariantType.Date Then
                        ' Si el valor es de tipo fecha, la grabamos en formato yyyy/MM/dd HH:mm:ss, para poder recuperarlo correctamente independientemente del idioma configurado
                        If CDate(Node.Item(Index, roSearchMode.roByIndex)).Date = Nothing Then
                            Text = Format(CDate(Node.Item(Index, roSearchMode.roByIndex)), "HH:mm:ss")
                        Else
                            Text = Format(CDate(Node.Item(Index, roSearchMode.roByIndex)), "yyyy/MM/dd HH:mm:ss")
                        End If
                    ElseIf VarType(Node.Item(Index, roSearchMode.roByIndex)) = VariantType.Double OrElse
                        VarType(Node.Item(Index, roSearchMode.roByIndex)) = VariantType.Decimal Then
                        Text = roConversions.Any2Double(Node.Item(Index, roSearchMode.roByIndex), oCultureInfo)
                        Text = Text.Replace(Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".")

                    Else
                        Text = roConversions.Any2String(Node.Item(Index, roSearchMode.roByIndex))
                    End If
                    Text = roSupport.StringEncodeControlChars(Text)
                    newElement.InnerText = Text
                    newElement.SetAttribute("type", roConversions.VarTypeVBNET2VB6(VarType(Node.Item(Index, roSearchMode.roByIndex))))

                Case vbObject
                    If TypeOf Node.Item(Index, roSearchMode.roByIndex) Is roCollection Then
                        ' El nodo es otro arbol, añade hijos
                        newElement.SetAttribute("type", XMLTYPE_ROCOLLECTION)
                        BuildXMLElements(XMLDoc, newElement, Node.Item(Index, roSearchMode.roByIndex), Depth + 1, oCultureInfo)
                    ElseIf TypeOf Node.Item(Index, roSearchMode.roByIndex) Is roTime Then
                        ' El nodo es un roTime, convierte a fecha/hora
                        Text = roConversions.Any2String(Node.Item(Index, roSearchMode.roByIndex).Value)
                        Text = roSupport.StringEncodeControlChars(Text)
                        newElement.SetAttribute("type", XMLTYPE_ROTIME)
                        newElement.InnerText = Text
                    Else
                        ' El nodo es un objeto desconocido
                        Err.Raise(9083, "roCollection", "Unsupported object type for XML export.")
                    End If
                Case Else
                    Err.Raise(9083, "roCollection", "Unsupported type (" & VarType(Node.Item(Index, roSearchMode.roByIndex)) & ") for XML export.")
            End Select

            ' Añade nuevo elemento al documento
            XMLElement.AppendChild(newElement)
        Next
    End Sub

    ' Cambia la clave de un elemento según la clave actual
    Public Sub ChangeKey(ByVal CurrentKey As Object, ByVal NewKey As Object)
        If mDupsBehavior <> roDuplicatesBehavior.roAllowDuplicates Then
            If Exists(NewKey) Then
                'Si ya existe un elemento y no permitimos duplicados actua segun comportamiento establecido
                Select Case mDupsBehavior
                    Case roDuplicatesBehavior.roReplaceDuplicates
                        Remove(NewKey)
                    Case roDuplicatesBehavior.roErrorOnDuplicates
                        Err.Raise(4567, "roCollection", "Can't have duplicate keys (set in behavior).")
                End Select
            End If
        End If

        ' Busca valor de la clave actual
        Dim myObject As Object
        myObject = Me.Item(CurrentKey)

        ' Borra valor actual
        Remove(CurrentKey)

        ' Añade el valor con la nueva clave
        Add(NewKey, myObject)
    End Sub

    ' Borra la coleccion actual
    Public Sub Clear()
        mCol = New Collection
        mKeyCol = New Collection
        mIndexCol = New Collection

    End Sub

    ''    Public Sub DebugWindow()
    ''        '
    ''        ' Funcion para debugar, muestra una ventana con el contenido del objeto
    ''        '
    ''        Dim fp As Integer

    ''        On Error Resume Next

    ''        ' Graba el XML en un fichero temporal
    ''        fp = FreeFile()
    ''Open App.Path & "\~tmp_roCollectionDebugWindow.xml" For Output As #fp
    ''Print #fp, XML
    ''Close #fp
    ''        ShellSync("START " & App.Path & "\~tmp_roCollectionDebugWindow.xml")
    ''        WaitSecs(1)
    ''        Kill(App.Path & "\~tmp_roCollectionDebugWindow.xml")

    ''    End Sub

    ' Obtiene el modo de tratamiento de los duplicados
    <DataMember>
    Public Property Duplicates() As roDuplicatesBehavior
        Get
            Return mDupsBehavior
        End Get
        Set(ByVal value As roDuplicatesBehavior)
            mDupsBehavior = value
        End Set
    End Property

    ' Añade un elemento (segun comportamiento establecido)
    Public Function Add(ByVal Key As Object, Optional ByRef Obj As Object = Nothing) As Object
        If mDupsBehavior <> roDuplicatesBehavior.roAllowDuplicates Then
            If Exists(Key) Then
                ' Si ya existe un elemento actua segun comportamiento establecido
                Select Case mDupsBehavior
                    Case roDuplicatesBehavior.roReplaceDuplicates
                        Remove(Key)
                    Case roDuplicatesBehavior.roIgnoreduplicates
                        Add = Nothing
                        Exit Function
                    Case roDuplicatesBehavior.roErrorOnDuplicates
                        Err.Raise(4567, "roCollection", "Can't have duplicate keys (set in behavior).")
                End Select
            End If
        End If

        ' Añade elemento
        Add_Really(Obj, Any2Key(Key))
        Add = Obj
    End Function

    ' Devuelve true si existe un nodo
    Public Function ExistsNode(ByVal PathOrIndex As Object, Optional ByVal SearchMode As roSearchMode = roSearchMode.roByKey) As Boolean
        Dim bolExists As Boolean = True
        Try
            Node(PathOrIndex, SearchMode)
        Catch
            bolExists = False
        End Try
        Return bolExists
    End Function

    ' Devuelve el indice a partir de una clave
    Public Function Index(ByVal Key As Object) As Integer
        Dim iIndex As Integer
        Key = Any2Key(Key)
        Try
            If mIndexCol.Count > 0 AndAlso mIndexCol.Contains(Key) Then
                iIndex = mIndexCol(Key)
            Else
                iIndex = 0
            End If
        Catch
            iIndex = 0
        End Try

        Return iIndex
    End Function

    ' Devuelve el nodo indicado
    Public Function Node(ByVal PathOrIndex As String, Optional ByVal SearchMode As roSearchMode = roSearchMode.roByKey) As roCollection
        Dim myNode As roCollection = Nothing
        Dim iLoop As Integer
        Dim iIndex As Integer
        If SearchMode = roSearchMode.roByIndex Then
            ' Busca nodo por indice
            iIndex = 0
            For iLoop = 1 To Me.Count
                If TypeOf Me.Item(iLoop, roSearchMode.roByIndex) Is roCollection Then
                    iIndex = iIndex + 1
                    If iIndex = Val(PathOrIndex) Then
                        Node = Me.Item(iLoop, roSearchMode.roByIndex)
                        Exit Function
                    End If
                End If
            Next
            Node = Nothing
            Err.Raise(3523, "roCollection", "Invalid node path.")
        Else
            ' Busca nodo por path
            If roConversions.StringItemsCount(PathOrIndex, "\") < 1 Then Err.Raise(3522, "roCollection", "Invalid node path.")

            ' Obtiene primer nodo
            Try
                myNode = Me.Item(roConversions.String2Item(PathOrIndex, 0, "\"))
            Catch
                ' Algo en el camino no es un roCollection, devuelve Null
                Node = Nothing
                Err.Raise(3523, "roCollection", "Invalid node path.")
            End Try

            ' Mira si debemos seguir buscando
            If roConversions.StringItemsCount(PathOrIndex, "\") > 1 Then
                Node = myNode.Node(Right$(PathOrIndex, Len(PathOrIndex) - Len(roConversions.String2Item(PathOrIndex, 0, "\")) - 1))
            Else
                Node = myNode
            End If
        End If
    End Function

    ' Devuelve el numero de nodos que hay
    Public Function NodeCount() As Long
        Dim lIndex As Integer = 0
        Dim lNodes As Integer = 0
        For lIndex = 1 To Me.Count
            If TypeOf Me.Item(lIndex, roSearchMode.roByIndex) Is roCollection Then
                lNodes = lNodes + 1
            End If
        Next
        NodeCount = lNodes
    End Function

    ' Devuelve la clave de un nodo
    Public Function NodeKey(ByVal Index As Integer) As Object
        Dim lIndex As Integer = 0
        Dim lNodes As Integer = 0
        For lIndex = 1 To Me.Count
            If TypeOf Me.Item(lIndex, roSearchMode.roByIndex) Is roCollection Then
                lNodes = lNodes + 1
                If lNodes = Index Then
                    NodeKey = Key(lIndex)
                    Exit Function
                End If
            End If
        Next
        NodeKey = Nothing
        Err.Raise(9342, "roCollection", "Invalid index.")
    End Function

    ' Añade un nodo a la clave indicada
    Public Function AddNode(ByVal Path As Object) As roCollection
        If roConversions.StringItemsCount(Path, "\") > 1 Then
            ' Debemos crear el path recursivamente
            Return Me.AddNode(roConversions.String2Item(Path, 0, "\")).AddNode(roConversions.String2RestItems(Path, 0, "\"))
        Else
            ' Es la ultima hoja a crear
            If mDupsBehavior <> roDuplicatesBehavior.roAllowDuplicates Then
                If ExistsNode(Path) Then
                    ' Si ya existe un elemento y no permitimos duplicados
                    '  actua segun comportamiento establecido
                    Select Case mDupsBehavior
                        Case roDuplicatesBehavior.roReplaceDuplicates, roDuplicatesBehavior.roIgnoreduplicates
                            ' En el caso de nodos, no lo reemplaza nunca
                            Return Me.Node(Path)
                        Case roDuplicatesBehavior.roErrorOnDuplicates
                            Err.Raise(4567, "roCollection", "Can't have duplicate keys (set in behavior).")
                    End Select
                End If
            End If

            ' Añade elemento
            Dim myNewCollection As roCollection
            myNewCollection = New roCollection
            Add_Really(myNewCollection, Any2Key(Path))
            Return myNewCollection
        End If
    End Function

    ' Devuelve True si existe el elemento en la colección
    Public Function Exists(ByVal Key As Object) As Boolean
        Exists = (Index(Key) <> 0)
    End Function

    ' Importa una roCollection dentro de esta
    Public Sub Import(ByRef ImportedCollection As roCollection)
        Dim l As Integer
        Dim myNewCollection As roCollection

        If ImportedCollection.Count > 0 Then
            ' Si hay elementos los importa uno a uno
            For l = 1 To ImportedCollection.Count
                If TypeOf ImportedCollection.Item(l, roSearchMode.roByIndex) Is roCollection Then
                    ' Si el elemento es otra roCollection, llamada recursiva
                    myNewCollection = New roCollection
                    myNewCollection.Import(ImportedCollection.Item(l, roSearchMode.roByIndex))
                    Add(ImportedCollection.Key(l), myNewCollection)
                ElseIf TypeOf ImportedCollection.Item(l, roSearchMode.roByIndex) Is Object Then
                    ' Si es un objeto generico, ni idea de como importar, devuelve error
                    Err.Raise(5211, "roCollection", "Can't import collections containing objects")
                    Exit Sub
                Else
                    ' Cualquier otro tipo de dato, lo copia directamente
                    Add(ImportedCollection.Key(l), ImportedCollection.Item(l, roSearchMode.roByIndex))
                End If
            Next
        End If
    End Sub

    Public Sub ImportKeyandDoubleValue(ByRef ImportedCollection As roCollection)

        Dim totalRows As Integer = ImportedCollection.Count
        For l As Integer = 1 To totalRows
            Dim keyCause As Integer = ImportedCollection.Key(l)
            Dim valueCause As Double = ImportedCollection.Item(l, roCollection.roSearchMode.roByIndex)
            Add(keyCause, valueCause)
        Next l
    End Sub

    ' Carga la roCollection a partir de un string
    Public Sub LoadXMLString(ByVal XMLString As String)
        Dim XMLDoc As New Xml.XmlDocument
        Dim RootElement As Xml.XmlElement = Nothing
        Dim Element As Xml.XmlElement

        ' Carga fichero
        Try
            XMLDoc.LoadXml(XMLString)
        Catch
            Err.Raise(9049, "roCollection", "Error loading XML string.")
        End Try

        ' Comprueba versión
        Try
            RootElement = XMLDoc.DocumentElement
            If RootElement.GetAttribute("version") > XMLBASEVERSION Then
                Err.Raise(9077, "roCollection", "XML roCollection format is newer than this version, can't load.")
                Exit Sub
            End If
        Catch
        End Try

        ' Carga datos recursivamente
        For Each Element In RootElement.ChildNodes
            Try
                ParseXMLElements(XMLDoc, Element, Me)
            Catch
                Err.Raise(9060, "roCollection", "Error building from XML file.")
                Exit Sub
            End Try
        Next
    End Sub

    Public Sub LoadXMLString(ByVal XMLString As String, ByRef oCultureInfo As Globalization.CultureInfo)
        Dim XMLDoc As New Xml.XmlDocument
        Dim RootElement As Xml.XmlElement = Nothing
        Dim Element As Xml.XmlElement

        ' Carga fichero
        Try
            XMLDoc.LoadXml(XMLString.Trim)
        Catch
            Err.Raise(9049, "roCollection", "Error loading XML string.")
        End Try

        ' Comprueba versión
        Try
            RootElement = XMLDoc.DocumentElement
            If RootElement.GetAttribute("version") > XMLBASEVERSION Then
                Err.Raise(9077, "roCollection", "XML roCollection format is newer than this version, can't load.")
                Exit Sub
            End If
        Catch
        End Try

        ' Carga datos recursivamente
        For Each Element In RootElement.ChildNodes
            Try
                ParseXMLElements(XMLDoc, Element, Me, oCultureInfo)
            Catch
                Err.Raise(9060, "roCollection", "Error building from XML file.")
                Exit Sub
            End Try
        Next
    End Sub

    ' Graba la roCollection como un fichero XML
    Public Function XML() As String
        Dim XMLDoc As New Xml.XmlDocument
        Dim RootElement As Xml.XmlElement

        Try
            ' Genera codigo XML
            XMLDoc.LoadXml(XMLBASE)
            RootElement = XMLDoc.SelectSingleNode(XMLBASEPROP)
            BuildXMLElements(XMLDoc, RootElement, Me, 1)
        Catch ex As Exception
            XML = String.Empty
            Err.Raise(9060, "roCollection", "Error building XML string.")
            Exit Function
        End Try

        ' Devuelve codigo XML
        XML = XMLDoc.OuterXml
    End Function

    ' Graba la roCollection como un fichero XML
    Public Function XML(ByRef oCultureInfo As Globalization.CultureInfo) As String
        Dim XMLDoc As New Xml.XmlDocument
        Dim RootElement As Xml.XmlElement

        Try
            ' Genera codigo XML
            XMLDoc.LoadXml(XMLBASE)
            RootElement = XMLDoc.SelectSingleNode(XMLBASEPROP)
            BuildXMLElements(XMLDoc, RootElement, Me, 1, oCultureInfo)
        Catch
            XML = String.Empty
            Err.Raise(9060, "roCollection", "Error building XML string.")
            Exit Function
        End Try

        ' Devuelve codigo XML
        XML = XMLDoc.OuterXml
    End Function

    ' Devuelve la clave de un elemento según indice o elemento
    Public Function Key(ByVal Index As Integer) As Object
        Key = mKeyCol(Index)
    End Function

    ' Modifica valor
    Default Public Property Item(ByVal KeyOrIndex As Object, Optional ByVal SearchMode As roSearchMode = roSearchMode.roByKey) As Object
        Get
            ' Si buscamos por clave, obtenemos el indice
            If SearchMode = roSearchMode.roByKey Then
                KeyOrIndex = Index(KeyOrIndex)
                If KeyOrIndex = 0 Then
                    ' Si piden una clave que no existe, devuelve Null
                    Item = Nothing
                    Exit Property
                End If
            End If

            ' Devolvemos valor (si existe)
            Item = mCol(KeyOrIndex)
        End Get

        Set(ByVal value As Object)
            ' Borramos el valor actual
            Remove(KeyOrIndex)

            ' Si buscamos por indice, obtenemos la clave
            If SearchMode = roSearchMode.roByIndex Then KeyOrIndex = Key(KeyOrIndex)

            ' Añadimos el nuevo valor con la misma clave
            Add(KeyOrIndex, value)
        End Set
    End Property

    ' Devuelve el numero de elementos en la colección
    <DataMember>
    Public Property Count() As Long
        Get
            If mCol IsNot Nothing Then
                Return mCol.Count
            Else
                Return 0
            End If

        End Get
        Set

        End Set
    End Property

    ' Carga datos recursivamente de un arbol XML
    Private Sub ParseXMLElements(ByRef XMLDoc As Xml.XmlDocument, ByRef XMLElement As Xml.XmlElement, ByRef Node As roCollection)
        Dim myType As Object
        Dim myKey As Object
        Dim Text As String

        Dim newNode As roCollection
        Dim childElement As Xml.XmlElement

        ' Obtiene tipo y key
        myType = XMLElement.GetAttribute("type")
        myKey = XMLElement.GetAttribute("key")
        Text = roSupport.StringDecodeControlChars(XMLElement.InnerText)

        ' Mira datos
        ' El nodo es un valor
        Select Case myType
            Case ""

            Case 0 : Node.Add(myKey) 'vbEmpty
            Case 1 : Node.Add(myKey, Nothing) 'vbNull
            Case 2 : Node.Add(myKey, CInt(Text)) 'vbInteger
            Case 3 : Node.Add(myKey, CLng(Text)) 'vbLong
            Case 4 : Node.Add(myKey, CSng(Text)) 'vbSingle

            Case 5, 14 'vbDouble, 'vbDecimal

                Text = Text.Replace(".", Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                Node.Add(myKey, roConversions.Any2Double(Text))


            Case 6 : Node.Add(myKey, CDec(Text)) 'vbCurrency
            Case 7 'vbDate

                Try

                    Dim xDate As Nullable(Of DateTime)

                    If Not IsDate(Text) Then
                        If InStr(UCase(Text), "A") > 0 Then
                            Text = Mid$(Text, 1, InStr(UCase(Text), "A") - 1)
                            'Si son las 12 de la noche debemos pasar a 0 horas (12 de la noche)
                            If Left(Text, 3) = "12:" Then Text = DateAdd("h", -12, CDate(Text))
                        End If
                        If InStr(UCase(Text), "P") > 0 Then
                            Text = Mid$(Text, 1, InStr(UCase(Text), "P") - 1)
                            Text = DateAdd("h", 12, CDate(Text))
                        End If
                    End If

                    If Text.Length = 10 Then
                        Dim intTemp As Integer
                        If Integer.TryParse(Text.Substring(0, 4), intTemp) Then
                            xDate = New Date(Text.Substring(0, 4), Text.Substring(5, 2), Text.Substring(8, 2))
                        ElseIf Integer.TryParse(Text.Substring(6, 4), intTemp) Then
                            xDate = New Date(Text.Substring(6, 4), Text.Substring(3, 2), Text.Substring(0, 2))
                        End If

                    ElseIf Text.Length = 8 Then '00:00:00
                        xDate = New Date
                        xDate = xDate.Value.AddHours(CInt(Text.Substring(0, 2)))
                        xDate = xDate.Value.AddMinutes(CInt(Text.Substring(3, 2)))
                        xDate = xDate.Value.AddSeconds(CInt(Text.Substring(6, 2)))

                    ElseIf Text.Length = 7 Then '0:00:00
                        xDate = New Date
                        xDate = xDate.Value.AddHours(CInt(Text.Substring(0, 1)))
                        xDate = xDate.Value.AddMinutes(CInt(Text.Substring(2, 2)))
                        xDate = xDate.Value.AddSeconds(CInt(Text.Substring(5, 2)))
                    Else
                        Dim intTemp As Integer
                        If Integer.TryParse(Text.Substring(0, 4), intTemp) Then
                            '"1899/12/30 6:00:00" = 18 caracteres
                            '"1899/12/30 06:00:00" = 19 caracteres
                            If Text.Length = 19 Then
                                xDate = New Date(Text.Substring(0, 4), Text.Substring(5, 2), Text.Substring(8, 2),
                                                 Text.Substring(11, 2), Text.Substring(14, 2), Text.Substring(17, 2))
                            Else
                                xDate = New Date(Text.Substring(0, 4), Text.Substring(5, 2), Text.Substring(8, 2),
                                                 Text.Substring(11, 1), Text.Substring(13, 2), Text.Substring(16, 2))
                            End If

                        ElseIf Integer.TryParse(Text.Substring(6, 4), intTemp) Then
                            '"1899/12/30 6:00:00" = 18 caracteres
                            '"1899/12/30 06:00:00" = 19 caracteres
                            If Text.Length = 19 Then
                                xDate = New Date(Text.Substring(6, 4), Text.Substring(3, 2), Text.Substring(0, 2),
                                                 Text.Substring(11, 2), Text.Substring(14, 2), Text.Substring(17, 2))
                            Else
                                xDate = New Date(Text.Substring(6, 4), Text.Substring(3, 2), Text.Substring(0, 2),
                                                 Text.Substring(11, 1), Text.Substring(13, 2), Text.Substring(16, 2))
                            End If
                        End If
                    End If

                    If xDate.HasValue Then
                        Node.Add(myKey, xDate)
                    End If
                Catch ex As Exception
                End Try

            Case 8 : Node.Add(myKey, CStr(Text)) 'vbString
            Case 12 : Node.Add(myKey, Text) 'vbVariant
            Case 11 : Node.Add(myKey, CBool(Text)) 'vbBoolean
            Case 17 : Node.Add(myKey, CByte(Text)) 'vbByte
            Case XMLTYPE_ROCOLLECTION
                ' El nodo es un roCollection con hijos
                newNode = New roCollection
                ' Añade cada hijo
                For Each childElement In XMLElement.ChildNodes
                    ParseXMLElements(XMLDoc, childElement, newNode)
                Next
                ' Añade la roCollection al nodo actual
                Node.Add(myKey, newNode)
            Case XMLTYPE_ROTIME
                ' El nodo es un roTime
                Node.Add(myKey, roConversions.Any2Time(Text))
            Case Else
                Err.Raise(9084, "roCollection", "Unsupported type for XML import.")
        End Select

    End Sub

    ' Carga datos recursivamente de un arbol XML teniendo en cuenta la cultura recibida
    Private Sub ParseXMLElements(ByRef XMLDoc As Xml.XmlDocument, ByRef XMLElement As Xml.XmlElement, ByRef Node As roCollection, ByRef oCultureInfo As Globalization.CultureInfo)
        Dim myType As Object
        Dim myKey As Object
        Dim Text As String

        Dim newNode As roCollection
        Dim childElement As Xml.XmlElement

        ' Obtiene tipo y key
        myType = XMLElement.GetAttribute("type")
        myKey = XMLElement.GetAttribute("key")
        Text = roSupport.StringDecodeControlChars(XMLElement.InnerText)

        ' Mira datos
        ' El nodo es un valor
        Select Case myType
            Case 0 : Node.Add(myKey) 'vbEmpty
            Case 1 : Node.Add(myKey, Nothing) 'vbNull
            Case 2 : Node.Add(myKey, CInt(Text)) 'vbInteger
            Case 3 : Node.Add(myKey, CLng(Text)) 'vbLong
            Case 4 : Node.Add(myKey, CSng(Text)) 'vbSingle
            Case 5 : Node.Add(myKey, roConversions.Any2Double(Text, oCultureInfo)) 'vbDouble
            Case 6 : Node.Add(myKey, CDec(Text)) 'vbCurrency
            Case 7 'vbDate

                Try

                    Dim xDate As Nullable(Of DateTime)

                    If Not IsDate(Text) Then
                        If InStr(UCase(Text), "A") > 0 Then
                            Text = Mid$(Text, 1, InStr(UCase(Text), "A") - 1)
                            'Si son las 12 de la noche debemos pasar a 0 horas (12 de la noche)
                            If Left(Text, 3) = "12:" Then Text = DateAdd("h", -12, CDate(Text))
                        End If
                        If InStr(UCase(Text), "P") > 0 Then
                            Text = Mid$(Text, 1, InStr(UCase(Text), "P") - 1)
                            Text = DateAdd("h", 12, CDate(Text))
                        End If
                    End If

                    If Text.Length = 10 Then
                        Dim intTemp As Integer
                        If Integer.TryParse(Text.Substring(0, 4), intTemp) Then
                            xDate = New Date(Text.Substring(0, 4), Text.Substring(5, 2), Text.Substring(8, 2))
                        ElseIf Integer.TryParse(Text.Substring(6, 4), intTemp) Then
                            xDate = New Date(Text.Substring(6, 4), Text.Substring(3, 2), Text.Substring(0, 2))
                        End If

                    ElseIf Text.Length = 8 Then
                        xDate = New Date
                        xDate = xDate.Value.AddHours(CInt(Text.Substring(0, 2)))
                        xDate = xDate.Value.AddMinutes(CInt(Text.Substring(3, 2)))
                        xDate = xDate.Value.AddSeconds(CInt(Text.Substring(6, 2)))
                    Else
                        Dim intTemp As Integer
                        If Integer.TryParse(Text.Substring(0, 4), intTemp) Then
                            xDate = New Date(Text.Substring(0, 4), Text.Substring(5, 2), Text.Substring(8, 2),
                                             Text.Substring(11, 2), Text.Substring(14, 2), Text.Substring(17, 2))

                        ElseIf Integer.TryParse(Text.Substring(6, 4), intTemp) Then
                            xDate = New Date(Text.Substring(6, 4), Text.Substring(3, 2), Text.Substring(0, 2),
                                             Text.Substring(11, 2), Text.Substring(14, 2), Text.Substring(17, 2))
                        End If
                    End If

                    If xDate.HasValue Then
                        Node.Add(myKey, xDate)
                    End If
                Catch ex As Exception
                End Try

            Case 8 : Node.Add(myKey, CStr(Text)) 'vbString
            Case 12 : Node.Add(myKey, Text) 'vbVariant
            Case 11 : Node.Add(myKey, CBool(Text)) 'vbBoolean
            Case 14 : Node.Add(myKey, CDec(Text)) 'vbDecimal
            Case 17 : Node.Add(myKey, CByte(Text)) 'vbByte
            Case XMLTYPE_ROCOLLECTION
                ' El nodo es un roCollection con hijos
                newNode = New roCollection
                ' Añade cada hijo
                For Each childElement In XMLElement.ChildNodes
                    ParseXMLElements(XMLDoc, childElement, newNode, oCultureInfo)
                Next
                ' Añade la roCollection al nodo actual
                Node.Add(myKey, newNode)
            Case XMLTYPE_ROTIME
                ' El nodo es un roTime
                Node.Add(myKey, roConversions.Any2Time(Text))
            Case Else
                Err.Raise(9084, "roCollection", "Unsupported type for XML import.")
        End Select

    End Sub

    ' Elimina un elemento según el indice o la clave
    Public Sub Remove(ByVal KeyOrIndex As Object, Optional ByVal SearchMode As roSearchMode = roSearchMode.roByKey)
        Dim Key As String
        Dim IndexValue As Integer
        Dim i As Integer

        ' Obtiene clave e indice
        If SearchMode = roSearchMode.roByKey Then
            Key = Any2Key(KeyOrIndex)
            IndexValue = Index(Key)
        Else
            IndexValue = KeyOrIndex
            Key = mKeyCol(IndexValue)
        End If

        If IndexValue > 0 Then
            ' Borramos valor
            mCol.Remove(IndexValue)
            mKeyCol.Remove(IndexValue)
            mIndexCol.Remove(Key)

            ' Actualizamos indices
            For i = IndexValue To mCol.Count
                Key = mKeyCol(i)
                mIndexCol.Remove(Key)
                mIndexCol.Add(i, Key)
            Next
        End If
    End Sub

End Class