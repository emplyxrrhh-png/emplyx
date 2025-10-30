Namespace Helpers

    Public Class Constants

        Public Shared DefaultLanguageFile As String = "LiveAccess"
        Public Shared DefaultLanguagesEntries As String = "labelsEntries"
        Public Shared DefaultCardTreeType As String = "cardTreeType"
        Public Shared DefaultCardTreeData As String = "cardTreeData"
        Public Shared DefaultCardViewFilterExpression As String = "cardViewFilterExpression"
        Public Shared DefaultCardViewIdFilter As String = "cardViewIdArrayFilter"

        Public Shared DefaultTabBarButtonType As String = "tabBarButtonType"
        Public Shared DefaultTabBarButtonData As String = "tabBarButtonData"
        Public Shared DefaultBarButtonType As String = "barButtonType"
        Public Shared DefaultBarButtonData As String = "barButtonData"
        Public Shared DefaultViewTitle As String = "viewTitle"
        Public Shared DefaultViewDescription As String = "viewDescription"
        Public Shared SearchText As String = "searchText"
        Public Shared FilterText As String = "filterText"
        Public Shared DefaultViewCaption As String = "viewCaption"
        Public Shared DefaultViewIcon As String = "viewIcon"
        Public Shared DefaultViewCategories As String = "viewCategories"
        Public Shared ScriptVersion As String = "scriptsVersion"

        Public Shared CustomChangeTab As String = "customChangeTab"

        Public Shared OneMbInBytesSize As Integer = 1048576

        Public Shared ExcludedFileExtensions As HashSet(Of String) = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {".exe"}

    End Class

End Namespace