Imports System.Runtime.Serialization

Namespace ReportModel

    <DataContract>
    Public Class roReportExecution
        Private _executionDate As DateTime
        Private _status As Integer

        <DataMember>
        Public Property ExecutionDate As DateTime
            Get
                Return Me._executionDate
            End Get
            Set(value As DateTime)
                Me._executionDate = value
            End Set
        End Property

        <DataMember>
        Public Property Status As Integer
            Get
                Return Me._status
            End Get
            Set(value As Integer)
                Me._status = value
            End Set
        End Property

    End Class

    <DataContract>
    Public Class roReportModel
        Private _layoutId As Integer
        Private _layoutName As String
        Private _categories As List(Of roReportCategory)
        Private _executionsList As List(Of roReportExecution)

        <DataMember>
        Public Property LayoutId As Integer
            Get
                Return Me._layoutId
            End Get
            Set(value As Integer)
                Me._layoutId = value
            End Set
        End Property

        <DataMember>
        Public Property LayoutName As String
            Get
                Return Me._layoutName
            End Get
            Set(value As String)
                Me._layoutName = value
            End Set
        End Property

        <DataMember>
        Public Property Categories As List(Of roReportCategory)
            Get
                Return Me._categories
            End Get
            Set(value As List(Of roReportCategory))
                Me._categories = value
            End Set
        End Property

        <DataMember>
        Public Property ExecutionsList As Generic.List(Of roReportExecution)
            Get
                Return Me._executionsList
            End Get
            Set(value As Generic.List(Of roReportExecution))
                Me._executionsList = value
            End Set
        End Property

        <DataMember>
        Public Property LastExecutionDate As String
            Get
                If (Me._executionsList Is Nothing OrElse Me._executionsList.Count = 0) Then
                    Return ""
                Else
                    Return ExecutionsList.OrderByDescending(Function(x) x.ExecutionDate).FirstOrDefault().ExecutionDate.ToShortDateString()
                End If
            End Get
            Set(value As String)

            End Set
        End Property

    End Class

    <DataContract>
    Public Class roReportCategory
        Private _id As Integer
        Private _name As String

        <DataMember>
        Public Property Id As Integer
            Get
                Return Me._id
            End Get
            Set(value As Integer)
                Me._id = value
            End Set
        End Property

        <DataMember>
        Public Property Name As String
            Get
                Return Me._name
            End Get
            Set(value As String)
                Me._name = value
            End Set
        End Property

    End Class

    <DataContract>
    Public Class roCategoryBranch
        Private _id As Integer
        Private _category As roReportCategory
        Private _motherCategoryId As Integer

        <DataMember>
        Public Property Id As Integer
            Get
                Return Me._id
            End Get
            Set(value As Integer)
                Me._id = value
            End Set
        End Property

        <DataMember>
        Public Property Category As roReportCategory
            Get
                Return Me._category
            End Get
            Set(value As roReportCategory)
                Me._category = value
            End Set
        End Property

        <DataMember>
        Public Property MotherCategoryId As Integer
            Get
                Return Me._motherCategoryId
            End Get
            Set(value As Integer)
                Me._motherCategoryId = value
            End Set
        End Property

    End Class

    <DataContract>
    Public Class roCategoryTree
        Private _categoryTree As List(Of roCategoryBranch)

        <DataMember>
        Public Property CategoryTree As List(Of roCategoryBranch)
            Get
                Return Me._categoryTree
            End Get
            Set(value As List(Of roCategoryBranch))
                Me._categoryTree = value
            End Set
        End Property

    End Class

End Namespace