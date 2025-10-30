Imports System.ComponentModel
Imports Robotics.DataLayer
Imports Robotics.ExternalSystems
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports VT_XU_Base
Imports VT_XU_Common
Imports Xunit

Namespace Unit.Test





    <Collection("Types")>
    <CollectionDefinition("Types", DisableParallelization:=True)>
    <Category("Types")>
    Public Class roTypesTest

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("es-ES")
            Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("es-ES")
        End Sub

        'Asumimos que esto es válido en Robotics. Comparamos con esta funcion.
        Private Function Any2Time_Old(ByVal Value As Object, Optional ByVal EngineData As Boolean = False) As roTime
            Dim vResult As New roTime

            Dim Hours As Double
            Dim Minutes As Integer
            Dim Seconds As Integer

            Dim vAux As Object

            ' Procesa valor
            If TypeOf Value Is roTime Then
                ' Ya es un roTime
                vResult = Value
            Else
                Select Case VarType(Value)
                    Case vbDate
                        ' Ya es una fecha, devuelve tal cual
                        vResult.Value = Value

                    Case vbEmpty, vbNull
                        ' Datos vacios, devuelve tiempo vacio
                        If Not EngineData Then
                            vResult.Value = "00:00"
                        Else
                            vResult.Value = "1899/12/30"
                        End If

                    Case vbLong, vbSingle, vbDouble, vbDecimal, vbByte, vbInteger
                        ' Pasa a positivo
                        vAux = System.Math.Abs(Value)

                        ' Obtiene horas, minutos y segundos
                        Hours = Fix(vAux)
                        vAux = (vAux - Hours) * 60
                        Minutes = Fix(vAux)
                        vAux = (vAux - Minutes) * 60
                        Seconds = Fix(vAux)

                        ' Redondea (evita errores debidos a tener solo 6 digitos de precision)
                        If (vAux - Seconds) * 100 > 50 Then Seconds = Seconds + 1

                        ' Crea resultado
                        If Not EngineData Then
                            vResult.Value = DateAdd("h", System.Math.Sign(Value) * Hours, "00:00")
                        Else
                            vResult.Value = DateAdd("h", System.Math.Sign(Value) * Hours, "1899/12/30")
                        End If
                        vResult = vResult.Add(System.Math.Sign(Value) * Minutes, "n")
                        vResult = vResult.Add(System.Math.Sign(Value) * Seconds, "s")

                    Case vbString
                        ' Procesa un string
                        If IsDate(Value) Then
                            ' Si el string ya es una fecha, ya esta hecho
                            If EngineData Then
                                If CDate(Value).Year = 1 Then
                                    Value = "1899/12/30 " + Value
                                End If
                            End If
                            vResult.Value = Value

                        ElseIf IsNumeric(Value) Then
                            ' Es un string con un numero, pasa a horas
                            vResult = Any2Time_Old(roTypes.Any2Double(Value))
                        Else
                            Dim tmpDate As New DateTime()
                            Dim bRet As Boolean = DateTime.TryParse(Value, tmpDate)
                            If bRet Then
                                vResult.Value = tmpDate
                            Else
                                Dim arr As String() = Value.ToString.Split("/")
                                If arr.Length = 3 Then
                                    bRet = DateTime.TryParse(arr(1) & "/" & arr(0) & "/" & arr(2), tmpDate)
                                    If bRet Then
                                        vResult.Value = tmpDate
                                    End If
                                End If
                            End If
                            If Not bRet Then
                                ' Si no es una fecha directamente, procesa string
                                If roTypes.StringItemsCount(Value, ":") > 1 Then
                                    If Not EngineData Then
                                        ' Hay separadores de horas
                                        vResult = Any2Time_Old(DateAdd("h", Val(roTypes.String2Item(Value, 0, ":")), DateAdd("n", Val(roTypes.String2Item(Value, 1, ":")), "00:00")))
                                    Else
                                        vResult = Any2Time_Old(DateAdd("h", Val(roTypes.String2Item(Value, 0, ":")), DateAdd("n", Val(roTypes.String2Item(Value, 1, ":")), "1899/12/30")))
                                    End If
                                End If
                            End If
                        End If

                    Case Else
                        ' Error, tipo no soportado
                        Err.Raise(3455, "roSupport", "Any2Time: Value can't be converted to roTime.")
                End Select
            End If

            Return vResult

        End Function


        Private Sub AssertRoTime(resultTime As roTime, resultTimeOld As roTime, expectedYear As Integer, expectedMonth As Integer, expectedDay As Integer)
            Dim newDT As DateTime = resultTime.ValueDateTime
            Dim oldDT As DateTime = resultTimeOld.ValueDateTime

            Assert.True(newDT = oldDT AndAlso newDT.Year = expectedYear AndAlso newDT.Month = expectedMonth AndAlso newDT.Day = expectedDay)
        End Sub

        <Fact(DisplayName:="Should create roTime from a valid roTime")>
        Sub ShouldCreateRoTimeFromValidRoTime()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                Dim testDate As DateTime = roTypes.CreateDateTime(2024, 12, 30, 14, 25, 24)
                Dim oTime As New roTime(testDate)

                'Act
                Dim resultTime As roTime = roTypes.Any2Time(oTime)
                Dim resultTimeOld As roTime = Any2Time_Old(oTime)

                'Assert
                Assert.True(resultTime.ValueDateTime = resultTimeOld.ValueDateTime)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a valid Datetime")>
        Sub ShouldCreateRoTimeFromValidDatetime()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                Dim testDate As DateTime = roTypes.CreateDateTime(2024, 12, 30, 14, 25, 24)

                'Act
                Dim resultTime As roTime = roTypes.Any2Time(testDate)
                Dim resultTimeOld As roTime = Any2Time_Old(testDate)

                'Assert
                Assert.True(resultTime.ValueDateTime = resultTimeOld.ValueDateTime)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a valid number")>
        Sub ShouldCreateRoTimeFromValidNumber()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                Dim validNmber As Long = 230

                'Act
                Dim resultTime As roTime = roTypes.Any2Time(validNmber)
                Dim resultTimeOld As roTime = Any2Time_Old(validNmber)

                'Assert
                Assert.True(resultTime.ValueDateTime = resultTimeOld.ValueDateTime)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a valid date as string")>
        Sub ShouldCreateRoTimeFromValidDateAsString()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                Dim validStringDate As String = "10:21"

                'Act
                Dim resultTime As roTime = roTypes.Any2Time(validStringDate)
                Dim resultTimeOld As roTime = Any2Time_Old(validStringDate)

                'Assert
                Assert.True(resultTime.ValueDateTime = resultTimeOld.ValueDateTime AndAlso resultTime.ValueDateTime.Year = 1)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a valid date as string on engine")>
        Sub ShouldCreateRoTimeFromValidDateAsStringOnEngine()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                Dim validStringDate As String = "10:21"

                'Act
                Dim resultTime As roTime = roTypes.Any2Time(validStringDate, True)
                Dim resultTimeOld As roTime = Any2Time_Old(validStringDate, True)

                'Assert
                Assert.True(resultTime.ValueDateTime = resultTimeOld.ValueDateTime AndAlso resultTime.ValueDateTime.Year = 1899)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a valid int as string")>
        Sub ShouldCreateRoTimeFromValidIntAsString()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                Dim validStringDate As String = "10"

                'Act
                Dim resultTime As roTime = roTypes.Any2Time(validStringDate)
                Dim resultTimeOld As roTime = Any2Time_Old(validStringDate)

                'Assert
                Assert.True(resultTime.ValueDateTime = resultTimeOld.ValueDateTime AndAlso resultTime.ValueDateTime.Year = 1)
            End Using
        End Sub

        <Fact(DisplayName:="Should not create roTime when an invalid object is passed")>
        Sub ShouldNotCreateRoTimeWhenInvalidObjectIsPassed()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                Dim array As Byte() = New Byte() {1, 2, 3}
                'Act
                Dim newThrowsException As Boolean = False
                Try
                    Dim resultTime As roTime = roTypes.Any2Time(array)
                Catch ex As Exception
                    newThrowsException = True
                End Try


                Dim oldThrowsException As Boolean = False
                Try
                    Dim resultTimeOld As roTime = Any2Time_Old(array)
                Catch ex As Exception
                    oldThrowsException = True
                End Try


                'Assert
                Assert.True(newThrowsException = oldThrowsException And newThrowsException)
            End Using
        End Sub

        ' "dd-MM-yyyy", "dd/MM/yyyy", "dd-MM-yyyy HH:mm", "dd-MM-yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss.fff", "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss.fff"

        <Fact(DisplayName:="Should create roTime from a string in format (dd-MM-yyyy)")>
        Sub ShouldCreateRoTimeFromStringInFormat1()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("30-12-2024")
                Dim resultTimeOld As roTime = Any2Time_Old("30-12-2024")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (dd/MM/yyyy)")>
        Sub ShouldCreateRoTimeFromStringInFormat2()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("30/12/2024")
                Dim resultTimeOld As roTime = Any2Time_Old("30/12/2024")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub



        <Fact(DisplayName:="Should create roTime from a string in format (dd-MM-yyyy HH:mm)")>
        Sub ShouldCreateRoTimeFromStringInFormat3()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("30-12-2024 12:45")
                Dim resultTimeOld As roTime = Any2Time_Old("30-12-2024 12:45")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub


        <Fact(DisplayName:="Should create roTime from a string in format (dd-MM-yyyy HH:mm:ss)")>
        Sub ShouldCreateRoTimeFromStringInFormat4()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("30-12-2024 12:45:14")
                Dim resultTimeOld As roTime = Any2Time_Old("30-12-2024 12:45:14")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (dd-MM-yyyy HH:mm:ss.fff)")>
        Sub ShouldCreateRoTimeFromStringInFormat5()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("30-12-2024 12:45:14.345")
                Dim resultTimeOld As roTime = Any2Time_Old("30-12-2024 12:45:14.345")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (dd/MM/yyyy HH:mm)")>
        Sub ShouldCreateRoTimeFromStringInFormat6()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("30/12/2024 12:45")
                Dim resultTimeOld As roTime = Any2Time_Old("30/12/2024 12:45")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub



        <Fact(DisplayName:="Should create roTime from a string in format (dd/MM/yyyy HH:mm:ss)")>
        Sub ShouldCreateRoTimeFromStringInFormat7()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("30/12/2024 12:45:14")
                Dim resultTimeOld As roTime = Any2Time_Old("30/12/2024 12:45:14")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (dd/MM/yyyy HH:mm:ss.fff)")>
        Sub ShouldCreateRoTimeFromStringInFormat8()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("30/12/2024 12:45:14.345")
                Dim resultTimeOld As roTime = Any2Time_Old("30/12/2024 12:45:14.345")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub


        '"yyyy-MM-dd", "yyyy/MM/dd", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss.fff", "yyyy/MM/dd HH:mm", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss.fff"

        <Fact(DisplayName:="Should create roTime from a string in format (yyyy-MM-dd)")>
        Sub ShouldCreateRoTimeFromStringInFormat9()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("2024-12-30")
                Dim resultTimeOld As roTime = Any2Time_Old("2024-12-30")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (yyyy/MM/dd)")>
        Sub ShouldCreateRoTimeFromStringInFormat10()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("2024/12/30")
                Dim resultTimeOld As roTime = Any2Time_Old("2024/12/30")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (yyyy-MM-dd HH:mm)")>
        Sub ShouldCreateRoTimeFromStringInFormat11()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("2024-12-30 12:45")
                Dim resultTimeOld As roTime = Any2Time_Old("2024-12-30 12:45")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (yyyy-MM-dd HH:mm:ss)")>
        Sub ShouldCreateRoTimeFromStringInFormat12()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("2024-12-30 12:45:14")
                Dim resultTimeOld As roTime = Any2Time_Old("2024-12-30 12:45:14")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (yyyy-MM-dd HH:mm.fff)")>
        Sub ShouldCreateRoTimeFromStringInFormat13()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("2024-12-30 12:45:14.345")
                Dim resultTimeOld As roTime = Any2Time_Old("2024-12-30 12:45:14.345")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (yyyy/MM/dd HH:mm)")>
        Sub ShouldCreateRoTimeFromStringInFormat14()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("2024/12/30 12:45")
                Dim resultTimeOld As roTime = Any2Time_Old("2024/12/30 12:45")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (yyyy/MM/dd HH:mm:ss)")>
        Sub ShouldCreateRoTimeFromStringInFormat15()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("2024/12/30 12:45:14")
                Dim resultTimeOld As roTime = Any2Time_Old("2024/12/30 12:45:14")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (yyyy/MM/dd HH:mm:ss.fff)")>
        Sub ShouldCreateRoTimeFromStringInFormat16()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("2024/12/30 12:45:14.345")
                Dim resultTimeOld As roTime = Any2Time_Old("2024/12/30 12:45:14.345")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub


        '"MM-dd-yyyy", "MM/dd/yyyy", "MM-dd-yyyy HH:mm", "MM-dd-yyyy HH:mm:ss", "MM-dd-yyyy HH:mm:ss.fff", "yyyy-MM-dd HH:mm", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss.fff"

        <Fact(DisplayName:="Should create roTime from a string in format (MM-dd-yyyy)")>
        Sub ShouldCreateRoTimeFromStringInFormat17()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("12-30-2024")
                Dim resultTimeOld As roTime = Any2Time_Old("12-30-2024")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 1, 1, 1)
                'ara es ok
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (MM/dd/yyyy)")>
        Sub ShouldCreateRoTimeFromStringInFormat18()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("12/30/2024")
                Dim resultTimeOld As roTime = Any2Time_Old("12/30/2024")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (MM-dd-yyyy HH:mm)")>
        Sub ShouldCreateRoTimeFromStringInFormat19()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("12-30-2024 12:45")
                Dim resultTimeOld As roTime = Any2Time_Old("12-30-2024 12:45")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 1, 1, 1)
                'ara es ok
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (MM-dd-yyyy HH:mm:ss)")>
        Sub ShouldCreateRoTimeFromStringInFormat20()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("12-30-2024 12:45:14")
                Dim resultTimeOld As roTime = Any2Time_Old("12-30-2024 12:45:14")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 1, 1, 1)
                'ara es ok
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (MM-dd-yyyy HH:mm:ss.fff)")>
        Sub ShouldCreateRoTimeFromStringInFormat21()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("12-30-2024 12:45:14.345")
                Dim resultTimeOld As roTime = Any2Time_Old("12-30-2024 12:45:14.345")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 1, 1, 1)
                'ara es ok
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (MM/dd/yyyy HH:mm)")>
        Sub ShouldCreateRoTimeFromStringInFormat22()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("12/30/2024 12:45")
                Dim resultTimeOld As roTime = Any2Time_Old("12/30/2024 12:45")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (MM/dd/yyyy HH:mm:ss)")>
        Sub ShouldCreateRoTimeFromStringInFormat23()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("12/30/2024 12:45:14")
                Dim resultTimeOld As roTime = Any2Time_Old("12/30/2024 12:45:14")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub

        <Fact(DisplayName:="Should create roTime from a string in format (MM/dd/yyyy HH:mm:ss.fff)")>
        Sub ShouldCreateRoTimeFromStringInFormat24()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange                
                'Act
                Dim resultTime As roTime = roTypes.Any2Time("12/30/2024 12:45:14.345")
                Dim resultTimeOld As roTime = Any2Time_Old("12/30/2024 12:45:14.345")

                'Assert
                AssertRoTime(resultTime, resultTimeOld, 2024, 12, 30)
            End Using
        End Sub


    End Class

End Namespace
