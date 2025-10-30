function dynamicSort(property, column) {
    var sortOrder = 1;
    if (property[0] === "-") {
        sortOrder = -1;
        property = property.substr(1);
    }

    return function (a, b) {
        var aValue = '';
        var bValue = '';

        switch (property) {
            case '':
                aValue = a.Pos;
                bValue = b.Pos;
                break;
            case 'assignment':
                if (typeof a.PeriodData.DayData[column].AssigData != 'undefined' && a.PeriodData.DayData[column].AssigData != null) aValue = a.PeriodData.DayData[column].AssigData.Name;
                if (typeof b.PeriodData.DayData[column].AssigData != 'undefined' && b.PeriodData.DayData[column].AssigData != null) bValue = b.PeriodData.DayData[column].AssigData.Name;
                break;
            case 'group':
                aValue = a.EmployeeData.GroupName;
                bValue = b.EmployeeData.GroupName;
                break;
            case 'employee':
                aValue = a.EmployeeData.EmployeeName;
                bValue = b.EmployeeData.EmployeeName;
                break;
            case 'shift':
                if (typeof a.PeriodData.DayData[column].MainShift != 'undefined' && a.PeriodData.DayData[column].MainShift != null) aValue = a.PeriodData.DayData[column].MainShift.Name;
                if (typeof b.PeriodData.DayData[column].MainShift != 'undefined' && b.PeriodData.DayData[column].MainShift != null) bValue = b.PeriodData.DayData[column].MainShift.Name;
                break;
            case 'budget':
                if (typeof a.PeriodData.DayData[column].IDDailyBudgetPosition != 'undefined' && a.PeriodData.DayData[column].IDDailyBudgetPosition != null) aValue = a.PeriodData.DayData[column].IDDailyBudgetPosition;
                if (typeof b.PeriodData.DayData[column].IDDailyBudgetPosition != 'undefined' && b.PeriodData.DayData[column].IDDailyBudgetPosition != null) bValue = b.PeriodData.DayData[column].IDDailyBudgetPosition;
                break;
        }

        var result = (aValue < bValue) ? -1 : (aValue > bValue) ? 1 : 0;

        return result * sortOrder;
    }
}

function dynamicSortMultiple(sortParams, columnOrder) {
    /*
     * save the arguments object as it will be overwritten
     * note that arguments object is an array-like object
     * consisting of the names of the properties to sort by
     */
    var props = sortParams;
    return function (obj1, obj2) {
        var i = 0, result = 0, numberOfProperties = (props.length - 1);
        /* try getting a different result from 0 (equal)
         * as long as we have extra properties to compare
         */
        while (result === 0 && i < numberOfProperties) {
            result = dynamicSort(props[i], columnOrder)(obj1, obj2);
            i++;
        }
        return result;
    }
};