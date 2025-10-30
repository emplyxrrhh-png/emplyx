Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Public Class SchedulerCtrlBase
    Inherits UserControlBase
    Implements ISchedulerControlsBase

#Region "Private properties"

    Private _pageBase As PageBase

#End Region

#Region "Public Properties"

    Public Overridable ReadOnly Property EditingData(Optional ByVal _Grid As Robotics.WebControls2.GridViewControl.roGridViewControl = Nothing) As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overridable Property HasChanges() As Boolean
        Get
            Return False
        End Get
        Set(ByVal value As Boolean)
            ' do nothing
        End Set
    End Property

    Public Property IDEmployee() As Integer Implements ISchedulerControlsBase.IDEmployee
        Get
            Return ViewState("IDEmployee")
        End Get
        Set(ByVal value As Integer)
            ViewState("IDEmployee") = value
        End Set
    End Property

    Public Property IDCause() As Integer Implements ISchedulerControlsBase.IDCause
        Get
            Return ViewState("IDCause")
        End Get
        Set(ByVal value As Integer)
            ViewState("IDCause") = value
        End Set
    End Property

    Public Property BeginDate() As Date Implements ISchedulerControlsBase.BeginDate
        Get
            Return CDate(ViewState("BeginDate"))
        End Get
        Set(ByVal value As Date)
            ViewState("BeginDate") = value
        End Set
    End Property

    Public Property DateMoves() As Date Implements ISchedulerControlsBase.DateMoves
        Get
            Return CDate(ViewState("DateMoves"))
        End Get
        Set(ByVal value As Date)
            ViewState("DateMoves") = value
        End Set
    End Property

    Public Property Status() As Integer Implements ISchedulerControlsBase.Status
        Get
            Return ViewState("Status")
        End Get
        Set(ByVal value As Integer)
            ViewState("Status") = value
        End Set
    End Property

    Public Property FreezingDate() As Date Implements ISchedulerControlsBase.FreezingDate
        Get
            Return CDate(ViewState("FreezingDate"))
        End Get
        Set(ByVal value As Date)
            ViewState("FreezingDate") = value
        End Set
    End Property

    ' Permiso configurado sobre la funcionalidad 'Edición de fichajes' (Calendar.Punches)
    Public Property Permission() As Permission
        Get
            Return ViewState("Permission")
        End Get
        Set(ByVal value As Permission)
            ViewState("Permission") = value
        End Set
    End Property

    Public Overridable Property CurrentPermission() As Permission
        Get
            Return ViewState("CurrentPermission")
        End Get
        Set(ByVal value As Permission)
            ViewState("CurrentPermission") = value
        End Set
    End Property

    Public Property View() As Integer Implements ISchedulerControlsBase.View
        Get
            Return ViewState("View")
        End Get
        Set(ByVal value As Integer)
            ViewState("View") = value
        End Set
    End Property

#End Region

    Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        ViewState.Clear()
        GC.Collect()
        GC.WaitForPendingFinalizers()
    End Sub

End Class