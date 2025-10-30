Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security.Base

Namespace VTBudgets

    Public Class roProductiveUnitState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As ProductiveUnitResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roProductiveUnit", "ProductiveUnitService")
            Me.intResult = ProductiveUnitResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roProductiveUnit", "ProductiveUnitService", _IDPassport)
            Me.intResult = ProductiveUnitResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roProductiveUnit", "ProductiveUnitService", _IDPassport, _Context)
            Me.intResult = ProductiveUnitResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roProductiveUnit", "ProductiveUnitService", _IDPassport, , _ClientAddress)
            Me.intResult = ProductiveUnitResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roProductiveUnit", "ProductiveUnitService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = ProductiveUnitResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As ProductiveUnitResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As ProductiveUnitResultEnum)
                Me.intResult = value
                If Me.intResult <> ProductiveUnitResultEnum.NoError And Me.intResult <> ProductiveUnitResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("ProductiveUnitResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = ProductiveUnitResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ProductiveUnitResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = ProductiveUnitResultEnum.Exception
        End Sub

#End Region

    End Class

    Public Class roBudgetState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As BudgetResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService")
            Me.intResult = BudgetResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport)
            Me.intResult = BudgetResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport, _Context)
            Me.intResult = BudgetResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport, , _ClientAddress)
            Me.intResult = BudgetResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = BudgetResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As BudgetResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As BudgetResultEnum)
                Me.intResult = value
                If Me.intResult <> BudgetResultEnum.NoError And Me.intResult <> BudgetResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("BudgetResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = BudgetResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BudgetResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BudgetResultEnum.Exception
        End Sub

#End Region

    End Class

    Public Class roBudgetRowState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As BudgetRowResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService")
            Me.intResult = BudgetRowResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport)
            Me.intResult = BudgetRowResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport, _Context)
            Me.intResult = BudgetRowResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport, , _ClientAddress)
            Me.intResult = BudgetRowResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = BudgetRowResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As BudgetRowResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As BudgetRowResultEnum)
                Me.intResult = value
                If Me.intResult <> BudgetRowResultEnum.NoError And Me.intResult <> BudgetRowResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("BudgetRowResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = BudgetRowResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BudgetRowResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BudgetRowResultEnum.Exception
        End Sub

#End Region

    End Class

    Public Class roBudgetRowPeriodDataState
        Inherits roBusinessState

#Region "Declarations - Constructor"

        Private intResult As BudgetRowPeriodDataResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService")
            Me.intResult = BudgetRowPeriodDataResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport)
            Me.intResult = BudgetRowPeriodDataResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport, _Context)
            Me.intResult = BudgetRowPeriodDataResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport, , _ClientAddress)
            Me.intResult = BudgetRowPeriodDataResultEnum.NoError
        End Sub

        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roBudget", "BudgetService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = BudgetRowPeriodDataResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As BudgetRowPeriodDataResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As BudgetRowPeriodDataResultEnum)
                Me.intResult = value
                If Me.intResult <> BudgetRowPeriodDataResultEnum.NoError And Me.intResult <> BudgetRowPeriodDataResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("BudgetRowPeriodDataResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = BudgetRowPeriodDataResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BudgetRowPeriodDataResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = BudgetRowPeriodDataResultEnum.Exception
        End Sub

#End Region

    End Class

End Namespace