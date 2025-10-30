Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class ProfileExportFields

        Enum Type
            typ_Text
            typ_Numeric
            typ_Date
            typ_Time
        End Enum

        Private mSource As String = ""
        Private mFormat As String = ""
        Private mLenght As Integer = 0
        Private mIntegerLenght As Integer = 0
        Private mDecimalLenght As Integer = 0
        Private mType As ProfileExportFields.Type
        Private mPadding As Integer = 0
        Private mValue As Object = Nothing
        Private mShortName As String = ""
        Private mGetValueFromList As Boolean = False
        Private mHead As String = ""

        Public Sub New(ByVal Source As String, ByVal Format As String, ByVal Lenght As Integer, ByVal IntegerLenght As Integer, ByVal DecimalLenght As Integer, ByVal Type As ProfileExportFields.Type, ByVal Padding As Integer, ByVal Head As String)
            mSource = Source
            mFormat = Format
            mLenght = Lenght
            mIntegerLenght = IntegerLenght
            mDecimalLenght = DecimalLenght
            mType = Type
            If IntegerLenght Or DecimalLenght Then mType = ProfileExportFields.Type.typ_Numeric
            mPadding = Padding
            mValue = Nothing
            mShortName = ""
            mHead = Head
        End Sub

        Public Property Source() As String
            Get
                Return Me.mSource
            End Get
            Set(ByVal value As String)
                Me.mSource = value
            End Set
        End Property

        Public Property ShortName() As String
            Get
                Return Me.mShortName
            End Get
            Set(ByVal value As String)
                Me.mShortName = value
            End Set
        End Property

        Public Property Head() As String
            Get
                Return Me.mHead
            End Get
            Set(ByVal value As String)
                Me.mHead = value
            End Set
        End Property

        Public Property Value() As Object
            Get
                Return Me.mValue
            End Get
            Set(ByVal NewValue As Object)
                Me.mValue = NewValue
            End Set
        End Property

        Public Property GetValueFromList() As Boolean
            Get
                Return Me.mGetValueFromList
            End Get
            Set(ByVal NewValue As Boolean)
                Me.mGetValueFromList = NewValue
            End Set
        End Property

        Public ReadOnly Property DataType As Type
            Get
                Return mType
            End Get
        End Property

        Public ReadOnly Property DataFormat As String
            Get
                Return mFormat
            End Get
        End Property

        Public Property ValueFormated() As String
            Get
                Dim str As String = ""
                Dim aux As String = ""

                Try
                    ' Si no tiene valor asigna valor vacío en funcion del tipo de campo
                    Select Case mType
                        Case Type.typ_Date
                            If IsNothing(Me.mValue) Or IsDBNull(Me.Value) Then
                                Me.Value = #12:00:00 AM#
                            End If
                        Case Type.typ_Numeric
                            If IsNothing(Me.mValue) Or IsDBNull(Me.Value) Then Me.Value = 0
                        Case Type.typ_Text
                            If IsNothing(Me.mValue) Or IsDBNull(Me.Value) Then Me.Value = ""
                        Case Else
                            Me.Value = ""
                    End Select

                    ' Si es una lista
                    If GetValueFromList Then
                        aux = mValue

                        ' Obtiene los valores de una lista
                        If Me.mFormat <> "" Then
                            Dim s() As String = Me.mFormat.Split(";")
                            Dim i As Integer = 0
                            For i = 0 To s.Length - 1
                                Dim v() As String = s(i).Split("=")
                                If mValue.ToString.ToUpper = v(0) Then
                                    aux = v(1)
                                    Exit For
                                End If
                            Next
                        End If
                    Else
                        ' Comprueba formato
                        If Me.mFormat <> "" Then
                            Select Case mType
                                Case Type.typ_Date
                                    ' Formatea valor para campos tipo fecha
                                    If TypeOf (Me.Value) Is Double Or TypeOf (Me.Value) Is Decimal Then
                                        If Me.mFormat.ToUpper = "HH:MM" Then
                                            aux = roConversions.ConvertHoursToTime(CDbl(Me.mValue))
                                        End If
                                    ElseIf TypeOf (Me.Value) Is DateTime Or TypeOf (Me.Value) Is Date Then
                                        If Me.Value <> #12:00:00 AM# Then
                                            aux = Format(Me.mValue, Me.mFormat)
                                        End If
                                    Else
                                        If TypeOf (Me.Value) Is String AndAlso roTypes.Any2String(Me.Value) <> String.Empty Then
                                            Try
                                                Dim dateValue As DateTime = roTypes.Any2DateTime(Me.Value)
                                                aux = Format(dateValue, Me.mFormat)
                                            Catch ex As Exception
                                                aux = Me.Value
                                            End Try
                                        End If

                                    End If
                                Case Type.typ_Numeric
                                    Dim bInitiallyComa As Boolean = False
                                    Dim newFormat As String = Me.mFormat
                                    If Me.mFormat.Contains(",") Then
                                        bInitiallyComa = True
                                        newFormat = Me.mFormat.Replace(",", ".")
                                    End If

                                    aux = Format(Any2Double(Me.mValue), newFormat)

                                    If bInitiallyComa Then
                                        If aux.Contains(".") Then
                                            aux = aux.Replace(".", ",")
                                        End If
                                    Else
                                        If aux.Contains(",") Then
                                            aux = aux.Replace(",", ".")
                                        End If
                                    End If

                                Case Type.typ_Text
                                    ' No formatea el valor para tipo texto
                                    aux = Me.mValue
                            End Select
                        Else
                            ' Asigna el valor sin formatear
                            aux = Me.mValue
                        End If

                        ' Quita el caracter separador de decimal
                        If mType = Type.typ_Numeric Then
                            If Me.mLenght > 0 AndAlso Me.mDecimalLenght > 0 AndAlso Me.mIntegerLenght > 0 Then
                                If Me.mIntegerLenght + Me.mDecimalLenght = Me.mLenght Then
                                    Dim f() As String
                                    Dim bIsNegative As Boolean = False
                                    Dim strIntegerPart As String = ""
                                    Dim strDecimalPart As String = ""

                                    If aux.StartsWith("-") Then
                                        bIsNegative = True
                                        aux = aux.Substring(1)
                                    End If
                                    If InStr(aux, ",") > 0 Then
                                        f = aux.Split(",")
                                        If bIsNegative Then
                                            strIntegerPart = f(0).PadLeft(Me.mIntegerLenght - 1, "0")
                                        Else
                                            strIntegerPart = f(0).PadLeft(Me.mIntegerLenght, "0")
                                        End If
                                        If f(1).Length > Me.mDecimalLenght Then
                                            f(1) = f(1).Substring(0, Me.mDecimalLenght)
                                        End If
                                        strDecimalPart = f(1).PadRight(Me.mDecimalLenght, "0")
                                    ElseIf InStr(aux, ".") > 0 Then
                                        f = aux.Split(".")
                                        If bIsNegative Then
                                            strIntegerPart = f(0).PadLeft(Me.mIntegerLenght - 1, "0")
                                        Else
                                            strIntegerPart = f(0).PadLeft(Me.mIntegerLenght, "0")
                                        End If
                                        If f(1).Length > Me.mDecimalLenght Then
                                            f(1) = f(1).Substring(0, Me.mDecimalLenght)
                                        End If
                                        strDecimalPart = f(1).PadRight(Me.mDecimalLenght, "0")
                                    Else
                                        If bIsNegative Then
                                            strIntegerPart = aux.PadLeft(Me.mLenght - 1, "0")
                                        Else
                                            strIntegerPart = aux.PadLeft(Me.mLenght, "0")
                                        End If
                                    End If

                                    If bIsNegative Then
                                        aux = "-" & strIntegerPart & strDecimalPart
                                    Else
                                        aux = strIntegerPart & strDecimalPart
                                    End If
                                End If
                            Else
                                If Me.mLenght = 0 AndAlso Me.mDecimalLenght > 0 Then

                                    Dim f() As String
                                    Dim bIsNegative As Boolean = False
                                    Dim strIntegerPart As String = ""
                                    Dim strDecimalPart As String = ""
                                    Dim decimalSeparator As String = ""

                                    If aux.StartsWith("-") Then
                                        bIsNegative = True
                                        aux = aux.Substring(1)
                                    End If
                                    If InStr(aux, ",") > 0 Then
                                        f = aux.Split(",")
                                        strIntegerPart = f(0)

                                        If f(1).Length > Me.mDecimalLenght Then
                                            f(1) = f(1).Substring(0, Me.mDecimalLenght)
                                        End If
                                        strDecimalPart = f(1).PadRight(Me.mDecimalLenght, "0")
                                        decimalSeparator = ","
                                    ElseIf InStr(aux, ".") > 0 Then
                                        f = aux.Split(".")
                                        strIntegerPart = f(0)
                                        If f(1).Length > Me.mDecimalLenght Then
                                            f(1) = f(1).Substring(0, Me.mDecimalLenght)
                                        End If
                                        strDecimalPart = f(1).PadRight(Me.mDecimalLenght, "0")
                                        decimalSeparator = "."
                                    Else
                                        strIntegerPart = aux
                                    End If

                                    If bIsNegative Then
                                        aux = "-" & strIntegerPart & decimalSeparator & strDecimalPart
                                    Else
                                        aux = strIntegerPart & decimalSeparator & strDecimalPart
                                    End If
                                End If
                            End If
                        End If

                    End If

                    ' Longitud maxima
                    If Me.mLenght <> 0 Then
                        Select Case mType
                            Case Type.typ_Text
                                aux += New String(" ", Me.mLenght)
                                aux = aux.Substring(0, Me.mLenght)

                            Case Type.typ_Numeric
                                If aux.Length < Me.mLenght Then aux = New String(" ", Me.mLenght - aux.Length) & aux

                            Case Type.typ_Date
                                If aux.Length < Me.mLenght Then aux = New String(" ", Me.mLenght - aux.Length) & aux

                        End Select
                    End If

                    ' Padding
                    If Me.mPadding <> 0 Then aux += New String(" ", Me.mPadding)
                Catch ex As Exception
                    Console.Write(ex.Message)

                End Try

                str = aux
                Return str

            End Get

            Set(ByVal NewValue As String)
                'Me.mValue = NewValue
            End Set
        End Property

    End Class

End Namespace