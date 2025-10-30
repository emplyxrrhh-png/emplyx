Imports System.Linq.Expressions
Imports System.Runtime.CompilerServices

Public Module PredicateBuilder

    Public Function [True](Of T)() As Expression(Of Func(Of T, Boolean))
        Return Function(f) True
    End Function

    Public Function [False](Of T)() As Expression(Of Func(Of T, Boolean))
        Return Function(f) False
    End Function

    <Extension()>
    Public Function [Or](Of T)(ByVal expr1 As Expression(Of Func(Of T, Boolean)), ByVal expr2 As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
        'Dim invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast(Of Expression)())
        Dim lst As New Generic.List(Of System.Linq.Expressions.Expression)()
        For Each item As ParameterExpression In expr1.Parameters
            lst.Add(DirectCast(item, Expression))
        Next
        Dim invokedExpr = Expression.Invoke(expr2, lst)
        Return Expression.Lambda(Of Func(Of T, Boolean))(Expression.[OrElse](expr1.Body, invokedExpr), expr1.Parameters)
    End Function

    <Extension()>
    Public Function [And](Of T)(ByVal expr1 As Expression(Of Func(Of T, Boolean)), ByVal expr2 As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
        'Dim invokedExpr = Nothing 'Expression.Invoke(expr2, expr1.Parameters.Cast(Of Expression)())
        Dim lst As New Generic.List(Of System.Linq.Expressions.Expression)()
        For Each item As ParameterExpression In expr1.Parameters
            lst.Add(DirectCast(item, Expression))
        Next
        Dim invokedExpr = Expression.Invoke(expr2, lst)
        Return Expression.Lambda(Of Func(Of T, Boolean))(Expression.[AndAlso](expr1.Body, invokedExpr), expr1.Parameters)
    End Function

End Module