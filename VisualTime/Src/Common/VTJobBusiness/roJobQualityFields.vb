Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

''' <summary>
''' Obtiene la configuración de campos para el control de calidad de los trabajos (sysroJobQualityFields)
''' </summary>
''' <remarks>Los nombres de los campos para el control de calidad no contiene el prefijo QLTY_.</remarks>
Public Class roJobQualityFields

#Region "Declarations - Constructor"

    Private oLog As roLog

    Private strFields() As String

    Public Sub New(ByVal _Log As roLog)

        Me.oLog = _Log
        ReDim Me.strFields(-1)

        Me.Load()

    End Sub

#End Region

#Region "Properties"

    Public Property Fields() As String()
        Get
            Return Me.strFields
        End Get
        Set(ByVal value As String())
            Me.strFields = value
        End Set
    End Property

#End Region

#Region "Methods"

    ''' <summary>
    ''' Carga la lista de campos para el control de calidad en orden alfabético.
    ''' </summary>
    ''' <remarks>Los nombres de los campos no contienen el prefijo 'QLTY_'.</remarks>
    Private Sub Load()

        Try

            Dim strSQL As String = "@SELECT# FieldName FROM sysroJobQualityFields ORDER BY FieldName"
            Dim tb As DataTable = CreateDataTable(strSQL)
            ReDim Me.strFields(tb.Rows.Count - 1)
            For n As Integer = 0 To tb.Rows.Count - 1
                Me.strFields(n) = tb.Rows(n).Item("FieldName")
            Next
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roJobQualityFields::Load: ", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roJobQualityFields::Load: ", ex)
        End Try

    End Sub

    Public Function Save() As Boolean

        ' ...

    End Function

#End Region

End Class