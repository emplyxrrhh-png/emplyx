Imports Robotics.Base.VTBusiness.Common

Namespace VTUnits
    Public Class roProductiveUnitState
        Inherits roBusinessState





#Region "Declarations - Constructor"

        Private intResult As UnitResultEnum

        Public Sub New()
            MyBase.New("Robotics.Base.DTOs.roProductiveUnit", "ProductiveUnitService")
            Me.intResult = UnitResultEnum.NoError
        End Sub
        Public Sub New(ByVal _IDPassport As Integer)
            MyBase.New("Robotics.Base.DTOs.roProductiveUnit", "ProductiveUnitService", _IDPassport)
            Me.intResult = UnitResultEnum.NoError
        End Sub
        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext)
            MyBase.New("Robotics.Base.DTOs.roProductiveUnit", "ProductiveUnitService", _IDPassport, _Context)
            Me.intResult = UnitResultEnum.NoError
        End Sub
        Public Sub New(ByVal _IDPassport As Integer, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roProductiveUnit", "ProductiveUnitService", _IDPassport, , _ClientAddress)
            Me.intResult = UnitResultEnum.NoError
        End Sub
        Public Sub New(ByVal _IDPassport As Integer, ByVal _Context As System.Web.HttpContext, ByVal _ClientAddress As String)
            MyBase.New("Robotics.Base.DTOs.roProductiveUnit", "ProductiveUnitService", _IDPassport, _Context, _ClientAddress)
            Me.intResult = UnitResultEnum.NoError
        End Sub

#End Region

#Region "Properties"

        Public Property Result() As UnitResultEnum
            Get
                Return Me.intResult
            End Get
            Set(ByVal value As UnitResultEnum)
                Me.intResult = value
                If Me.intResult <> UnitResultEnum.NoError And Me.intResult <> UnitResultEnum.Exception Then
                    Me.ErrorText = Me.Language.Translate("UnitResultEnum." & Me.intResult.ToString, "")
                End If
            End Set
        End Property

#End Region

#Region "Methods"
        Public Overrides Sub UpdateAccessTime()

        End Sub

        Public Overrides Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
            MyBase.UpdateStateInfo(_Context)
            Me.intResult = UnitResultEnum.NoError
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = UnitResultEnum.Exception
        End Sub

        Public Overrides Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
            MyBase.UpdateStateInfo(Ex, strUbication)
            Me.intResult = UnitResultEnum.Exception
        End Sub

#End Region
    End Class
End Namespace

