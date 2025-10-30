(function () {
    var namespace = function (name) {
        var namespaces = name.split('.'),
            namespace = window,
            index;
        for (index = 0; index < namespaces.length; index += 1) {
            namespace = namespace[namespaces[index]] = namespace[namespaces[index]] || {};
        }
        return namespace;
    };

    namespace("Robotics.Client.Controls");
    namespace("Robotics.Client.Language");
    namespace("Robotics.Client.Constants");
}());

Robotics.Client.Language.Tags = {
    NoSolection: 'Language.NoSolection',
    Employee: 'Language.Employee',
    Employees: 'Language.Employees',
    Department: 'Language.Department',
    Departments: 'Language.Departments',
    Connector: 'Language.Connector',
    PlannedHours: 'Language.PlannedHours',
    Holidays: 'Language.Holidays',
    Mandatory: 'Language.Mandatory',
    Flexible: 'Language.Flexible',
    Complementary: 'Language.Complementary',
    ContextMenu_Details: 'Language.ContextMenu_Details',
    ContextMenu_RemoveHolidays: 'Language.ContextMenu_RemoveHolidays',
    ContextMenu_Copy: 'Language.ContextMenu_Copy',
    ContextMenu_Paste: 'Language.ContextMenu_Paste',
    ContextMenu_Block: 'Language.ContextMenu_Block',
    ContextMenu_UnBlock: 'Language.ContextMenu_UnBlock',
    ContextMenu_CancelSelection: 'Language.ContextMenu_CancelSelection',
    ContextMenu_EditComplementary: 'Language.ContextMenu_EditComplementary',
    ContextMenu_AdvPaste: 'Language.ContextMenu_AdvPaste',
    ContextMenu_CopyHolidays: 'Language.ContextMenu_CopyHolidays',
    ContextMenu_CopyWorking: 'Language.ContextMenu_CopyWorking',
    ContextMenu_CopyAssignments: 'Language.ContextMenu_CopyAssignments',
    ContextMenu_EditAssignments: 'Language.ContextMenu_EditAssignments',
    ContextMenu_SetCoverage: 'Language.ContextMenu_SetCoverage',
    ContextMenu_PlanifyCoverage: 'Language.ContextMenu_PlanifyCoverage',
    ContextMenu_Sort: 'Language.ContextMenu_Sort',
    ContextMenu_SetFeast: 'Language.ContextMenu_SetFeast',
    ContextMenu_RemoveFeast: 'Language.ContextMenu_RemoveFeast',
    Empty: 'Language.Empty',
    Accept: 'Language.Accept',
    Cancel: 'Language.Cancel',
    Download: 'Language.Download',
    Tooltip_absent: 'Language.Tooltip_absent',
    Tooltip_holidays: 'Language.Tooltip_holidays',
    Tooltip_absence: 'Language.Tooltip_absence',
    Tooltip_incidence: 'Language.Tooltip_incidence',
    Tooltip_overtime: 'Language.Tooltip_overtime',
    Tooltip_notes: 'Language.Tooltip_notes',
    Tooltip_remark1: 'Language.Tooltip_remark1',
    Tooltip_remark2: 'Language.Tooltip_remark2',
    Tooltip_remark3: 'Language.Tooltip_remark3',
    Tooltip_feast: 'Language.Tooltip_feast',
    Scheduler_Real: 'Language.Scheduler_Real',
    Scheduler_Planified: 'Language.Scheduler_Planified',
    Scheduler_Recursive: 'Language.Scheduler_Recursive',
    Scheduler_NonRecursive: 'Language.Scheduler_NonRecursive',
    Scheduler_Filter: 'Language.Scheduler_Filter',
    Scheduler_Shifts: 'Language.Scheduler_Shifts',
    Scheduler_Assignments: 'Language.Scheduler_Assignments',
    Scheduler_Concepts: 'Language.Scheduler_Concepts',
    Empty_Assignment: 'Language.Empty_Assignment',
    Empty_Assignment_SN: 'Language.Empty_Assignment_SN',
    Tooltip_plannedHoliday: 'Language.Tooltip_plannedHoliday'
};

Robotics.Client.Language.translator = function (langContainter) {
    this.langContainer = langContainter;
};

Robotics.Client.Language.translator.prototype.translate = function (langKey) {
    return this.langContainer.Get(langKey);
};

Robotics.Client.Constants.WorkMode = {
    roCalendar: 0,
    roProductiveUnit: 1,
    roBudget: 2,
    roDayDetail: 3
};

Robotics.Client.Constants.TypeView = {
    DayDetail: 0, //Read-only view Only used on detail calendar
    DaySchedule: 1, //Read-only view Only used on detail calendar
    Review: 0, //Only used on schedule calendar
    Definition: 0, //Only used on budget calendar
    Planification: 1,
    Detail: 2 //Only used on budget calendar
};

Robotics.Client.Constants.ViewRange = {
    Daily: 0,
    Period: 1
};

Robotics.Client.Constants.ShiftType = {
    Normal: 0,
    NormalFloating: 1,
    Holiday: 2,
    Holiday_NonWorking: 3
};

Robotics.Client.Constants.BudgetRowState = {
    NoChanged: 0,
    New: 1,
    Updated: 2,
    Deleted: 3
}

Robotics.Client.Constants.CoverageDayStatus = {
    OK: 0,
    KO: 1,
    OVERLOAD: 2,
    WITHOUTCOVERAGE: 3
};

Robotics.Client.Constants.CoverageDayView = {
    Planified: 0,
    Real: 1
};

Robotics.Client.Constants.EmployeeStatusOnDay = {
    Ok: 0,
    NoContract: 1,
    InOtherDepartment: 2
};

Robotics.Client.Constants.ProductiveUnitStatusOnDay = {
    Ok: 0,
    NoPlanned: 1
}

Robotics.Client.Constants.DailyHourType = {
    Untyped: 0,
    Mandatory: 1,
    Flexible: 2,
    Complementary: 3
};

Robotics.Client.Constants.TableNames = {
    Calendar: 'FullCalendar',
    ColShifts: 'ColumnShiftsTable',
    ColAssignments: 'ColumnAssignmentsTable',
    ColIndictments: 'ColIndictmentsTable',
    ColCapacity: 'ColCapacity',
    RowAccruals: 'RowAccrualsTable',
    RowShifts: 'RowShiftsTable',
    RowAssignments: 'RowAssignmentsTable',
    AccrualTotals: 'AccrualTotals'
};

Robotics.Client.Constants.LayoutNames = {
    Center: 'Center',
    East: 'East',
    Calendar: 'Main',
    ColumnTabs: 'ColumnTabs',
    RowTabs: 'RowTabs',
    Resume: 'Resume',
    ColumnTabShifts: 'tabColumnShifts',
    ColumnTabAssignments: 'tabColumnAssignments',
    ColumnTabIndictments: 'tabColumnIndictments',
    ColumnTabCapacity: 'tabColumnCapacity',
    RowTabAccruals: 'tabRowAccruals',
    RowTabShifts: 'tabRowShifts',
    RowTabAssignments: 'tabRowAssignments',
};

Robotics.Client.Constants.Actions = {
    LoadData: 'LOADCALENDAR',
    SaveChanges: 'SAVECALENDAR',
    LoadHourData: 'LOADHOURDATA',
    LoadDayDefinition: 'LOADDAYDEFINITION',
    ExportToExcel: 'EXPORTTOEXCEL',
    ImportFromExcel: 'IMPORTFROMEXCEL',
    ImportFromExcelKO: 'IMPORTFROMEXCELKO',
    ImportFromExcelWarning: 'IMPORTFROMEXCELWARNING',
    ShiftLayerDefinition: 'SHIFTLAYERDEFINITION',
    ShiftLayerDefinitionEdit: 'SHIFTLAYERDEFINITIONEDIT',
    SaveAndContinue: 'SAVEANDCONTINUE',
    DiscardAndContinue: 'DISCARDANDCONTINUE',
    LoadCoverages: 'LOADCOVERAGES',
    CopyCoverages: 'COPYCOVERAGES',
    PositionModeDayData: 'POSITIONMODEDAYDATA',
    UpdateCurrentDayData: 'UPDATECURRENTDAYDATA',
    LoadBudget: 'LOADBUDGET',
    SaveBudgetChanges: 'SAVEBUDGETCHANGES',
    DiscardBudgetAndContinue: 'DISCARDBUDGETANDCONTINUE',
    SaveBudgetChangesAndContinue: 'SAVEBUDGETCHANGESANDCONTINUE',
    GetNewBudgetRow: 'GETNEWBUDGETROW',
    DeleteBudgetRow: "DELETEBUDGETROW",
    GetBudgetHourPeriodDeinition: 'GETBUDGETHOURPERIODDEFINITION',
    GetAvailablePositionEmployees: 'GETAVAILABLEPOSITIONEMPLOYEES',
    GetAvailablePositionEmployeesForFulFill: 'GETAVAILABLEPOSITIONEMPLOYEESFORFULFILL',
    AddEmployeePlanOnPosition: "ADDEMPLOYEEPLANONPOSITION",
    RemoveEmployeeFromPosition: "REMOVEEMPLOYEEFROMPOSITION",
    LoadAvailableEmployeesDetail: "LOADAVAILABLEEMPLOYEESDETAIL",
    CheckCalendarIndictments: "CHECKCALENDARINDITMENTS",
    RunAIPlanner: "RUNAIPLANNER",
    CleanAIPlanner: "CLEANAIPLANNER",
    UpdateProductiveUnitQuantity: "UPDATEPRODUCTIVEUNITQUANTITY"
};