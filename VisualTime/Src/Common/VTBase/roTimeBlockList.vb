Imports Robotics.VTBase.roTimeBlockItem
Imports Robotics.VTBase.roTypes

Public Class roTimeBlockList

    Private mData As New Collection

    Public Sub Clear()
        '
        ' Elimina todos los datos
        '
        mData = Nothing
        mData = New Collection

    End Sub

    Public Sub Add(ByVal BlockOrList As Object)
        '
        ' Añade un bloque de tiempo o una lista de tiempos a nuestra lista
        '
        Dim Index As Long
        Dim myValue As Double

        If TypeOf BlockOrList Is roTimeBlockItem Then
            ' Añadimos un bloque

            ' Primero comprueba validez
            If Not BlockOrList.Period.IsValid Then
                Err.Raise(4981, "roTimeBlockList", "Invalid block specified.")
            Else
                ' Añade en orden
                myValue = BlockOrList.Period.Begin.VBNumericValue
                For Index = 1 To mData.Count
                    If mData(Index).Period.Begin.VBNumericValue > myValue Then
                        mData.Add(BlockOrList, , Index)
                        Exit Sub
                    End If
                Next
                mData.Add(BlockOrList)
            End If

        ElseIf TypeOf (BlockOrList) Is roTimeBlockList Then
            ' Añadimos una lista de bloques, añade uno a uno
            Dim myBlock As roTimeBlockItem
            For Each myBlock In BlockOrList
                Me.Add(myBlock)
            Next
        Else
            Err.Raise(4422, "roTimeBlockList", "BlockOrList parameter is neither a roTimeBlockItem nor a roTimeBlockList.")
        End If

    End Sub

    Public Function Clone() As roTimeBlockList
        '
        ' Clona esta lista (devuelve una copia de la misma)
        '
        Dim ClonedList As New roTimeBlockList
        Dim Block As roTimeBlockItem

        For Each Block In mData
            ClonedList.Add(Block.Clone)
        Next

        Clone = ClonedList

    End Function

    Public Function Count() As Long
        '
        ' Devuelve el numero de items
        '
        Count = mData.Count

    End Function

    Public Function DebugText() As String
        '
        ' Funcion para debugar, devuelve los datos de este objeto en texto legible
        '
        Dim myBlock As roTimeBlockItem
        Dim st As String

        st = "Block count:" & Count() & vbCrLf & vbNewLine & "Details:" & vbCrLf & vbNewLine
        For Each myBlock In mData
            st = st & "   " & myBlock.DebugText & vbCrLf & vbNewLine
        Next

        DebugText = st

    End Function

    Public Sub Remove(ByVal Index As Integer)
        '
        ' Elimina un bloque de tiempo
        '
        mData.Remove(Index)

    End Sub

    Public Function Item(ByVal Index As Integer) As roTimeBlockItem
        'Attribute Item.VB_UserMemId = 0
        Return mData(Index)
    End Function

    Public Sub RemoveInPeriod(ByVal Period As roTimePeriod, Optional ByVal RemovedBlockType As roBlockType = roBlockType.roBTAny)
        '
        ' Elimina las capas del tipo indicado en el periodo indicado.
        '
        Dim Index As Long
        Dim PeriodBegin As Double
        Dim PeriodFinish As Double

        If Not Period.IsValid Then
            Err.Raise(4988, "roTimeBlockList", "Invalid period specified.")
        Else
            PeriodBegin = Period.Begin.VBNumericValue
            PeriodFinish = Period.Finish.VBNumericValue

            ' Primero debemos hacer split por los bordes del periodo
            SplitAtTime(Period.Begin)
            SplitAtTime(Period.Finish)

            ' Luego eliminamos los bloques que se encuentren dentro del mismo
            Index = 1
            While Index <= mData.Count
                With Item(Index)
                    If .ValidatesFilter(RemovedBlockType) Then
                        If .Period.Begin.VBNumericValue >= PeriodBegin And .Period.Begin.VBNumericValue <= PeriodFinish Then
                            ' Eliminamos esta incidencia porque esta en el descanso
                            Remove(Index)
                            Index = Index - 1
                        End If
                    End If
                End With
                Index = Index + 1
            End While

        End If

    End Sub

    Private Sub Replace_SingleLayer(ByVal Layer As roTimeBlockItem, ByVal ReplacedBlockType As roBlockType)
        '
        ' Subfuncion de Replace que unicamente reemplaza un bloque
        '
        Dim TimeBlock As roTimeBlockItem
        Dim Index As Integer

        ' Comprueba datos
        If Not Layer.Period.IsValid Then
            Err.Raise(4988, "roTimeBlockList", "Invalid layer specified.")
        Else
            ' Lo primero que hacemos es romper los bloques actuales por los limites del bloque que nos
            '  pasan.
            SplitAtTime(Layer.Period.Begin, ReplacedBlockType)
            SplitAtTime(Layer.Period.Finish, ReplacedBlockType)

            ' Recorremos la lista buscando bloques que interseccionen con el nuestro
            For Index = 1 To mData.Count
                ' Comprobamos que sea el tipo que reemplazamos
                If Item(Index).ValidatesFilter(ReplacedBlockType) Then
                    ' Miramos si el bloque intersecciona
                    If Item(Index).Period.Finish.VBNumericValue > Layer.Period.Begin.VBNumericValue _
             And Item(Index).Period.Begin.VBNumericValue < Layer.Period.Finish.VBNumericValue Then
                        ' Este bloque se intersecciona con el que nos pasan, como ya hemos hecho el split
                        '  antes, esto significa que todo el bloque debe cambiarse por nuestro tipo.
                        If Item(Index).TimeValue.VBNumericValue > Layer.TimeValue.VBNumericValue Then
                            ' El bloque es mayor que el total a reemplazar, separamos en dos
                            TimeBlock = Item(Index).Clone
                            TimeBlock.BlockType = Layer.BlockType
                            TimeBlock.TimeValue = Layer.TimeValue
                            Item(Index).TimeValue = Item(Index).TimeValue.Substract(Layer.TimeValue)
                            ' Añade el bloque a la lista
                            Me.Add(TimeBlock)
                            ' Sale (ya hemos cambiado todo el tiempo)
                            Exit Sub
                        Else
                            ' El bloque es más pequeño o igual que el tamaño a reemplazar, convertimos
                            '  al nuevo tipo directamente
                            Item(Index).BlockType = Layer.BlockType
                            ' Ahora quitamos el tiempo que hemos reemplazado del total disponible

                            'todo>>
                            'No lo resta bien
                            'Layer.TimeValue.Substract Item(Index).TimeValue
                            'Lo restamos a manopla
                            Layer.TimeValue = Any2Time(Layer.TimeValue.NumericValue(True) - Item(Index).TimeValue.NumericValue(True))
                            ' Miramos si aun nos queda tiempo por reemplazar, sino salimos
                            If Layer.TimeValue.VBNumericValue = 0 Then Exit Sub
                        End If
                    End If
                End If
            Next

        End If

    End Sub

    Public Sub SetTimeZoneInPeriod(ByVal Period As roTimePeriod, ByVal ReplacedBlockType As roBlockType, ByVal NewTimeZone As Object)
        '
        ' Subfuncion de Replace que unicamente reemplaza un bloque
        '
        Dim Block As roTimeBlockItem

        ' Lo primero que hacemos es romper los bloques actuales por los limites del bloque que nos
        '  pasan.
        SplitAtTime(Period.Begin, ReplacedBlockType)
        SplitAtTime(Period.Finish, ReplacedBlockType)

        ' Recorremos la lista buscando bloques que interseccionen con el nuestro
        For Each Block In mData
            ' Comprobamos que sea el tipo que reemplazamos
            If Block.ValidatesFilter(ReplacedBlockType) Then
                ' Miramos si el bloque intersecciona
                If Block.Period.Finish.VBNumericValue > Period.Begin.VBNumericValue _
         And Block.Period.Begin.VBNumericValue < Period.Finish.VBNumericValue Then
                    ' Este bloque se intersecciona con el que nos pasan, como ya hemos hecho el split
                    '  antes, esto significa que todo el bloque es de nuestra zona.
                    Block.TimeZone = NewTimeZone
                End If
            End If
        Next

    End Sub

    Public Sub SplitAtTime(ByVal SplitAtTime As roTime, Optional ByVal SplitOnlyBlockType As roBlockType = roBlockType.roBTAny)
        '
        ' Parte los bloques de tiempo por la hora indicada, opcionalmente solo parte los tipos
        '  indicados
        '

        Dim Index As Long
        Dim myNewBlock As roTimeBlockItem
        Dim SplitValue As Double
        Dim AvailableTime As Double

        If Not SplitAtTime.IsValid Then
            Err.Raise(4987, "roTimeBlockList", "Invalid time specified.")
        Else
            SplitValue = SplitAtTime.VBNumericValue

            For Index = 1 To mData.Count
                With Item(Index)
                    If .ValidatesFilter(SplitOnlyBlockType) Then
                        If .Period.Begin.VBNumericValue < SplitValue And .Period.Finish.VBNumericValue > SplitValue Then
                            ' Hay que cortar este bloque en dos

                            ' Obtiene tiempo a recortar
                            AvailableTime = .TimeValue.VBNumericValue

                            ' Crea una copia del bloque
                            myNewBlock = .Clone

                            ' El bloque antiguo termina donde cortamos y el nuevo empieza
                            '  donde cortamos
                            .Period.Finish = SplitAtTime
                            myNewBlock.Period.Begin = SplitAtTime

                            ' El bloque antiguo se queda todo el tiempo posible, el resto
                            '  es para el bloque nuevo (si no hay tiempo restante, no crea
                            '  el bloque nuevo)

                            'OJO
                            AvailableTime = AvailableTime - .TimeValue.VBNumericValue

                            If AvailableTime > 0 Then
                                myNewBlock.TimeValue = Any2Time(Date.FromOADate(AvailableTime))
                                Me.Add(myNewBlock)
                            End If
                        End If
                    End If
                End With
            Next

        End If

    End Sub

    '    Public Property Get NewEnum() As IUnknown
    'Attribute NewEnum.VB_UserMemId = -4
    'Attribute NewEnum.VB_MemberFlags = "40"
    '    'esta propiedad permite enumerar
    '    'esta colección con la sintaxis For...Each
    '    Set NewEnum = mData.[_NewEnum]
    'End Property

    Public Function GetComplementary(ByVal IntersectingPeriod As roTimePeriod, ByVal ComplementedBlockType As roBlockType, ByVal ResultTargetType As roBlockType) As roTimeBlockList
        '
        ' Devuelve una lista de TimeBlocks que no interseccionan con el periodo IntersectingPeriod
        '  (es un complementario de la lista original del periodo indicado).
        '
        Dim ResultList As New roTimeBlockList
        Dim LastTime As roTime
        Dim LastTimeValue As Double
        Dim FinalTimeValue As Double
        Dim Index As Long
        Dim NewBlock As roTimeBlockItem

        LastTime = IntersectingPeriod.Begin
        LastTimeValue = LastTime.VBNumericValue

        FinalTimeValue = IntersectingPeriod.Finish.VBNumericValue

        For Index = 1 To mData.Count
            With Item(Index)
                If .Period.Begin.VBNumericValue >= FinalTimeValue Then Exit For
                If .ValidatesFilter(ComplementedBlockType) Then
                    If .Period.Begin.VBNumericValue > LastTimeValue Then
                        ' Tenemos un trozo por complementar

                        ' Creamos complemento
                        NewBlock = New roTimeBlockItem
                        NewBlock.Period.Begin = LastTime
                        NewBlock.Period.Finish = .Period.Begin
                        NewBlock.BlockType = ResultTargetType
                        ResultList.Add(NewBlock)
                    End If

                    ' Calculamos nuevo inicio
                    LastTime = roConversions.Max(.Period.Finish, LastTime)
                    LastTimeValue = LastTime.VBNumericValue
                End If
            End With
        Next

        ' Añade porcion del final si falta
        If LastTimeValue < IntersectingPeriod.Finish.VBNumericValue Then
            ' Creamos complemento
            NewBlock = New roTimeBlockItem
            NewBlock.Period.Begin = LastTime
            NewBlock.Period.Finish = IntersectingPeriod.Finish
            NewBlock.BlockType = ResultTargetType
            ResultList.Add(NewBlock)
        End If

        ' Devuelve resultado
        GetComplementary = ResultList

    End Function

    Public Sub Replace(ByVal LayerBlockOrList As Object, ByVal ReplacedBlockType As roBlockType)
        '
        ' Reemplaza los bloques existentes en la lista con el que insertamos (si se solapan y unicamente
        '  la parte solapada, sino no hace nada).
        ' Para que un bloque se reemplace, debe ser del tipo ReplacedBlockType.
        '
        ' El argumento LayerBlockOrList es el bloque o lista de bloques a reemplazar.
        '
        If TypeOf LayerBlockOrList Is roTimeBlockItem Then
            '
            ' Reemplazando los elementos de la lista dentro de un bloque
            '
            Replace_SingleLayer(LayerBlockOrList, ReplacedBlockType)
        ElseIf TypeOf LayerBlockOrList Is roTimeBlockList Then
            '
            ' Nos pasan una lista de bloques, reemplazamos cada bloque uno a uno.
            '
            Dim myBlock As roTimeBlockItem
            For Each myBlock In LayerBlockOrList
                Replace_SingleLayer(myBlock, ReplacedBlockType)
            Next
        Else
            Err.Raise(4122, "roTimeBlockList", "LayerBlockOrList parameter is neither a roTimeBlockItem nor a roTimeBlockList.")
        End If

    End Sub

    Public Sub ReplaceEx(ByVal Layer As roTimeBlockItem, ByVal TimeReplaced As Double, ByVal ReplacedBlockType As roBlockType)
        '
        ' Subfuncion de Replace que unicamente reemplaza un bloque
        '
        Dim TimeBlock As roTimeBlockItem
        Dim Index As Integer
        Dim TimeRemaining As Double
        Dim CutBegin As Double
        Dim CutFinish As Double

        ' Comprueba datos
        If Not Layer.Period.IsValid Then
            Err.Raise(4988, "roTimeBlockList", "Invalid layer specified.")
        Else
            ' Lo primero que hacemos es romper los bloques actuales por los limites del bloque que nos
            '  pasan.

            ' Establecemos tiempo que queda por reemplazar
            TimeRemaining = TimeReplaced
            Index = mData.Count

            ' Recorremos la lista buscando bloques que interseccionen con el nuestro
            While Index >= 1 And TimeRemaining > 0
                With Item(Index)
                    ' Comprobamos que sea el tipo que reemplazamos
                    If Not .ValidatesFilter(ReplacedBlockType) Then GoTo SkipThisBlock

                    ' Miramos los puntos de corte para reemplazar este bloque (si hay)
                    ' El punto final es el minimo entre los dos finales
                    CutFinish = roConversions.Min(.Period.Finish.VBNumericValue, Layer.Period.Finish.VBNumericValue)

                    ' El punto inicial es el maximo entre los dos inicios
                    CutBegin = roConversions.Max(.Period.Begin.VBNumericValue, Layer.Period.Begin.VBNumericValue)

                    ' El punto final es maximo entre el que hemos encontrado hasta ahora y el que
                    '  obtenemos de restar todo el tiempo a reemplazar desde el punto final
                    CutBegin = roConversions.Max(CutBegin, CutFinish - TimeRemaining)

                    ' El punto final lo ajustamos si el valor real no es el mismo que el del periodo para
                    '  que llegue como mucho al final real
                    CutFinish = roConversions.Min(CutFinish, CutBegin + .TimeValue.VBNumericValue)

                    ' Calculamos el tiempo a sustraer, si es negativo, no se solapan y no hacemos nada
                    If (CutFinish - CutBegin) <= 0 Then GoTo SkipThisBlock

                    ' Rompe en trozos antes de reemplazar (si es necesario)

                    ' Trozo posterior a la parte reemplazada
                    ' Solo se crea si hay una parte posterior y si ademas esa parte tendria algun tiempo si la crearamos
                    '  (es posible que quede con tiempo 0 si el valor real no es el mismo que el del periodo)
                    If (CutFinish < .Period.Finish.VBNumericValue) And (.TimeValue.VBNumericValue - (CutFinish - CutBegin)) > 0 Then
                        TimeBlock = .Clone
                        TimeBlock.Period.Begin = Any2Time(Date.FromOADate(CutFinish))
                        TimeBlock.TimeValue = Any2Time(Date.FromOADate(.TimeValue.VBNumericValue - (CutFinish - CutBegin)))

                        ' Añade el bloque a la lista
                        Me.Add(TimeBlock)
                    End If

                    ' Trozo anterior al que reemplazamos
                    If (CutBegin > .Period.Begin.VBNumericValue) Then
                        TimeBlock = .Clone
                        TimeBlock.Period.Finish = Any2Time(Date.FromOADate(CutBegin))
                        TimeBlock.TimeValue = TimeBlock.Period.PeriodTime
                        ' Añade el bloque a la lista
                        Me.Add(TimeBlock)
                    End If

                    ' Trozo del medio que reemplazamos
                    .BlockType = Layer.BlockType
                    .Period.Begin = Any2Time(Date.FromOADate(CutBegin))
                    .Period.Finish = Any2Time(Date.FromOADate(CutFinish))
                    .TimeValue = .Period.PeriodTime

                    ' Restamos el tiempo reemplazado del que queda pendiente
                    TimeRemaining = TimeRemaining - (CutFinish - CutBegin)

SkipThisBlock:
                End With
                Index = Index - 1
            End While

        End If

    End Sub

    Public Function GetDateTimeInPeriod(ByVal Period As roTimePeriod, ByVal AccountedBlockType As roBlockType) As Date
        '
        ' Cuenta el total de tiempo que se encuentra dentro de los limites del bloque especificado,
        '  comprobando unicamente los bloques del tipo indicado.
        '
        Dim myBlock As roTimeBlockItem
        Dim myTempTime As Object
        Dim LayerBegin As Double
        Dim LayerFinish As Double
        Dim Begin As Double
        Dim Finish As Double

        myTempTime = 0

        If Not Period.IsValid Then
            Err.Raise(4988, "roTimeBlockList", "Invalid period specified.")
        Else
            LayerBegin = Period.Begin.VBNumericValue
            LayerFinish = Period.Finish.VBNumericValue

            For Each myBlock In mData
                With myBlock
                    If .ValidatesFilter(AccountedBlockType) Then
                        Begin = roConversions.Max(LayerBegin, .Period.Begin.VBNumericValue)
                        Finish = roConversions.Min(LayerFinish, .Period.Finish.VBNumericValue)
                        If Finish > Begin Then
                            ' Añade tiempo al total calculado hasta ahora, hasta el máximo que permita el
                            '  timevalue del bloque interseccionado.
                            myTempTime = myTempTime + roConversions.Min(Finish - Begin, .TimeValue.VBNumericValue)
                        End If
                    End If
                End With
            Next
        End If
        'GetDateTimeInPeriod = CDate(myTempTime)
        Return Date.FromOADate(myTempTime)
    End Function

    Public Sub AddIncidence(ByVal Begin As roTime, ByVal Finish As roTime, ByVal IncidenceType As roBlockType, Optional ByVal TotalDateTime As Date = Nothing)
        '
        ' Añade una incidencia
        '
        Dim Incidence As New roTimeBlockItem

        ' Crea incidencia
        Incidence = New roTimeBlockItem
        Incidence.Period.Begin = Begin
        Incidence.Period.Finish = Finish
        Incidence.BlockType = IncidenceType
        If Not TotalDateTime = Nothing Then
            If Any2Time(TotalDateTime).VBNumericValue = 0 Then
                ' Si el tiempo es 0, no crea incidencia
                Exit Sub
            Else
                ' En caso contrario, crea incidencia
                Incidence.TimeValue = Any2Time(TotalDateTime)
            End If
        End If

        ' Añade a la lista
        Add(Incidence)

    End Sub

    Public Sub SubstractTimeInPeriod(ByVal Period As roTimePeriod, ByVal TimeSubstracted As Double, Optional ByVal SubstractedBlockType As roBlockType = roBlockType.roBTAny)
        '
        ' Elimina el tiempo indicado quitando tiempos de todos los bloques del tipo indicado
        ' que se encuentren en el periodo indicado, empezando por el primero.
        '
        Dim Index As Long
        Dim Block As roTimeBlockItem
        Dim PeriodBegin As Double
        Dim PeriodFinish As Double
        Dim CutBegin As Double
        Dim CutFinish As Double
        Dim SubstractedTime As Double
        Dim TimeRemaining As Double

        If Not Period.IsValid Then
            Err.Raise(4988, "roTimeBlockList", "Invalid period specified.")
        Else
            PeriodBegin = Period.Begin.VBNumericValue
            PeriodFinish = Period.Finish.VBNumericValue

            TimeRemaining = TimeSubstracted
            Index = 1
            While Index <= mData.Count And TimeRemaining > 0
                Block = Item(Index)
                If Block.ValidatesFilter(SubstractedBlockType) Then
                    CutBegin = roConversions.Max(PeriodBegin, Block.Period.Begin.VBNumericValue)
                    CutFinish = roConversions.Min(PeriodFinish, Block.Period.Finish.VBNumericValue)
                    If CutFinish > CutBegin Then
                        ' Descontamos tiempo de este bloque
                        ' TODO>>> Para que funcione bien al tener diferentes TimeZones deberiamos no
                        '  solo restar el tiempo sino partir la franja en 2. P.ej. Si debemos quitar
                        '  30 minutos de descanso de 19:00 a 19:30 que no se ficho en una franja entre
                        '  las 8:00 y las 22:30, deberiamos romper dicha franja en 2 porque como
                        '  funciona ahora quita el tiempo de todas partes y se carga las nocturnas >22h
                        SubstractedTime = roConversions.Min(CutFinish - CutBegin, Block.TimeValue.VBNumericValue)
                        SubstractedTime = roConversions.Min(SubstractedTime, TimeRemaining)
                        Block.TimeValue = Block.TimeValue.Substract(SubstractedTime)
                        TimeRemaining = TimeRemaining - SubstractedTime

                        ' Si el bloque tiene un tiempo=0, elimina
                        If Block.TimeValue.VBNumericValue = 0 Then
                            Block = Nothing
                            Remove(Index)
                            Index = Index - 1
                        End If
                    End If
                End If
                Index = Index + 1
            End While
        End If
    End Sub

End Class