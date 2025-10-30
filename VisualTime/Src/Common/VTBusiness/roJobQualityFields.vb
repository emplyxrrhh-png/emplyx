Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper

Namespace Job

    ''' <summary>
    ''' Obtiene la configuración de campos para el control de calidad de los trabajos (sysroJobQualityFields)
    ''' </summary>
    ''' <remarks>Los nombres de los campos para el control de calidad no contiene el prefijo QLTY_.</remarks>
    Public Class roJobQualityFields

#Region "Declarations - Constructor"

        Private oState As roJobState

        Private strFields() As String

        Public Sub New()
            Me.oState = New roJobState
            ReDim Me.strFields(-1)
        End Sub

        Public Sub New(ByVal _State As roJobState)

            Me.oState = _State
            ReDim Me.strFields(-1)

            Me.Load()

        End Sub

#End Region

#Region "Properties"

        Public Property State() As roJobState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roJobState)
                Me.oState = value
            End Set
        End Property

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
                Me.oState.UpdateStateInfo(ex, "roJobQualityFields::Load: ")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roJobQualityFields::Load: ")
            End Try

        End Sub

        Public Function Save() As Boolean

            ' ...

        End Function

#End Region

    End Class

End Namespace