Public Interface ISchedulerControls
    Inherits ISchedulerControlsBase

    Sub RefreshControl(Optional ByVal reload As Boolean = False)

    Sub Hide()

    Function SaveData() As Boolean

End Interface