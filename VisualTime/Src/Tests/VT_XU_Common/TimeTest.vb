Imports Robotics.VTBase
Imports Xunit

Namespace Unit.Test

    Public Class TimeTest
        Sub New()

        End Sub

        <Theory>
        <MemberData(NameOf(TimeHelper.AddData), MemberType:=GetType(TimeHelper))>
        Public Sub AddTest(ByVal initialDate As roTime, ByVal value As Object, ByVal interval As String, ByVal expectedDate As roTime)
            ' Act
            Dim result = initialDate.Add(value, interval)

            ' Assert
            Assert.Equal(expectedDate.Value, result.Value)
        End Sub

        <Theory>
        <MemberData(NameOf(TimeHelper.SubstractData), MemberType:=GetType(TimeHelper))>
        Public Sub SubstractTest(ByVal initialDate As roTime, ByVal value As Object, ByVal interval As String, ByVal expectedDate As roTime)
            ' Act
            Dim result = initialDate.Substract(value, interval)

            ' Assert
            Assert.Equal(expectedDate.Value, result.Value)
        End Sub

        <Theory>
        <MemberData(NameOf(TimeHelper.NumericValueData), MemberType:=GetType(TimeHelper))>
        Public Sub NumericValueTest(ByVal initialDate As roTime, ByVal engineData As Boolean, ByVal expectedDouble As Double)
            ' Act
            Dim result = initialDate.NumericValue(engineData)

            ' Assert
            Assert.Equal(expectedDouble, result)
        End Sub
    End Class
End Namespace
