Imports Robotics.Base.DTOs.Enums

Namespace Robotics.Base.DTOs

    Public Class ElementAttributes

        Public Property Id() As String
        Public Property ClassName() As String
        Public Property Icon() As String
        Public Property Type() As String

    End Class

    Public Class roItem

        Public Property Id() As Integer

        Public Property Text() As String
        Public Property Value() As String

    End Class

    Public Class roViewOptions

        Public Property ID() As Integer
        Public Property UniqueKey As String
        Public Property Name() As String
        Public Property Type() As eViewOptionType
        Public Property ControlType() As String
        Public Property Caption() As String

        Public Property LabelKey() As String
        Public Property LabelText() As String
        Public Property HtmlSelector() As String

        'Propiedades para los selects
        Public Property Values() As roItem()
        Public Property SelectedValue() As roItem

        Public Property ElementAttr() As ElementAttributes

    End Class

    Public Class roDataGridViewOptions
        Inherits roViewOptions
        Public Property DataGridColumnConfig() As List(Of roDataGridColumnConfig)

        Public Sub New(parent As roViewOptions, Optional ByVal columnConfigs As List(Of roDataGridColumnConfig) = Nothing)
            Me.BindParentAttributes(parent)
            Me.DataGridColumnConfig = columnConfigs
        End Sub

        Private Sub BindParentAttributes(parent As roViewOptions)
            Me.ID = parent.ID
            Me.UniqueKey = parent.UniqueKey
            Me.Name = parent.Name
            Me.Type = parent.Type
            Me.ControlType = parent.ControlType
            Me.LabelKey = parent.LabelKey
            Me.LabelText = parent.LabelText
            Me.HtmlSelector = parent.HtmlSelector
            Me.Values = parent.Values
            Me.SelectedValue = parent.SelectedValue
        End Sub

    End Class

    Public Class roDataGridColumnConfig

        Public Property caption As String
        Public Property dataField As String
        Public Property width As String
        Public Property alignment As String
        Public Property dataType As String
        Public Property format As String
        Public Property allowFiltering As String
        Public Property visible As Boolean?
        Public Property validationRules As List(Of roDataGridValidationRule)

    End Class

    Public Class roDataGridValidationRule
        Public Property type As String
        Public Property pattern As String
        Public Property message As String

    End Class

End Namespace