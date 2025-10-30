Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.Security.Base

Public Class EmployeeHelper

    Public Property EmployeeSearchedByCard As Boolean = False
    Public Property NifSearchCounter As Integer = 0
    Public Property SearchForGetDatesOfContractInDate As Boolean = False
    Public Property SearchForGetDatesOfAnnualWorkPeriodsInDate As Boolean = False
    Public Property SearchForGetLastContractDatesInPeriod As Boolean = False
    Public Property NumberOfSearchForGetDatesOfAnnualWorkPeriodsInDateCall As Integer = 0
    Public Property ValidSummaryTypeInGetPeriodDatesByContractAtDate As Boolean = False
    Public Property GetPeriodDatesByContractAtDateCalled As Boolean = False
    Public Property GetDatesOfAnnualWorkPeriodsInDateCalled As Boolean = False
    Public Property SaveEmployeeCalled As Boolean = False
    Public Property EntraUserFieldSave As String = ""
    Public Property SaveEmployeeUserFieldCalled As Boolean = False

    Public Property UpdateEmployeeImageCalled As Boolean = False

    Public Property Image As Byte()

    Public Property SavedUserField As roUserField

    Function EmployeesAndContractsTableStub(ByVal withContract As Boolean, ByVal withOutContract As Boolean)
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString =
                Function(ByVal strQuery As String, ByVal cn As roBaseConnection, ByVal strTableName As String) As DataTable
                    Return FillMock(withContract, withOutContract)
                End Function
    End Function

    Function FillMock(ByVal withContract As Boolean, ByVal withOutContract As Boolean)
        Dim stubDataTable As New System.Data.DataTable
        stubDataTable.Columns.Add("ID")
        stubDataTable.Columns.Add("Name")
        stubDataTable.Columns.Add("EmployeeKey")
        stubDataTable.Columns.Add("ActiveContract")
        If withOutContract Then
            stubDataTable.Rows.Add("115", "Perico", "25", "0")
        End If
        If withContract Then
            stubDataTable.Rows.Add("116", "Lola", "26", "1")
        End If
        Return stubDataTable
    End Function

    Function GetContractsByIDEmployeeStub(ByVal empKey As String, ByVal numContracts As Integer)
        Robotics.Base.VTEmployees.Contract.Fakes.ShimroContract.GetContractsByIDEmployeeInt32roContractStateBoolean =
                Function(ByVal IDEmployee As Integer, ByVal _State As roContractState, ByVal bAudit As Boolean) As DataTable
                    Return FillEmployeeContractsMock(empKey, numContracts)
                End Function
    End Function

    Function FillEmployeeContractsMock(ByVal idEmployee As String, ByVal numOfContracts As Integer)
        Dim stubDataTable As New System.Data.DataTable
        stubDataTable.Columns.Add("IDEmployee")
        stubDataTable.Columns.Add("IDContract")
        For i = 0 To numOfContracts - 1
            stubDataTable.Rows.Add(idEmployee, i.ToString)
        Next
        Return stubDataTable
    End Function

    Function GetEmployeeIDByIDCardSpy()
        Fakes.ShimroBusinessSupport.GetEmployeesByIDCardStringStringStringStringroBusinessState = Function(ByVal _IDCard As String, ByVal _IDCardType As String, ByVal _IDCardCountry As String, ByVal _IDCardState As String, ByVal _State As roBusinessState)
                                                                                                      EmployeeSearchedByCard = True
                                                                                                  End Function
    End Function

    Function GetIDEmployeesFromUserFieldValueSpy()
        Robotics.Base.VTUserFields.UserFields.Fakes.ShimroEmployeeUserField.GetIDEmployeesFromUserFieldValueStringStringDateTimeroUserFieldState =
                        Function(ByVal strUserField As String, ByVal strUserFieldValue As String, ByVal xDate As Date, ByVal _State As roUserFieldState)
                            Dim stubDataTable As New System.Data.DataTable()

                            If strUserField = "NIF" Then
                                NifSearchCounter += 1
                            End If

                            Return stubDataTable
                        End Function
    End Function

    Function GetIDEmployeesFromUserFieldValueStub()
        Robotics.Base.VTUserFields.UserFields.Fakes.ShimroEmployeeUserField.GetIDEmployeesFromUserFieldValueStringStringDateTimeroUserFieldState =
                        Function(ByVal strUserField As String, ByVal strUserFieldValue As String, ByVal xDate As Date, ByVal _State As roUserFieldState)
                            Dim stubDataTable As New System.Data.DataTable()
                            stubDataTable.Columns.Add("IDEmployee")
                            stubDataTable.Rows.Add(strUserFieldValue)
                            Return stubDataTable
                        End Function
    End Function

    Function ContractLoadStub(idContract As String, idemployee As Integer)
        Contract.Fakes.ShimroContract.AllInstances.LoadBoolean =
        Function(xContract As Contract.roContract, ByVal Audit As Boolean)
            xContract.IDEmployee = idemployee
            xContract.IDContract = idContract
            Return True
        End Function
    End Function

    Function GetContractsByIdEmployeeStub(oContract As roContract)
        Contract.Fakes.ShimroContract.GetContractsByIDEmployeeInt32roContractStateBoolean = Function(ByVal IDEmployee As Integer, ByVal _State As roContractState, ByVal bAudit As Boolean)
                                                                                                Dim stubDataTable As New System.Data.DataTable
                                                                                                stubDataTable.Columns.Add("IDContract")
                                                                                                stubDataTable.Columns.Add("BeginDate")
                                                                                                stubDataTable.Columns.Add("EndDate")
                                                                                                stubDataTable.Rows.Add(oContract.IDContract, oContract.BeginDate, oContract.EndDate)
                                                                                                Return stubDataTable
                                                                                            End Function
    End Function

    Function GetEmployeeLockDatetoApplyStub(dDate As Date)
        Fakes.ShimroBusinessSupport.GetEmployeeLockDatetoApplyInt32BooleanRefroBusinessStateRefBoolean = Function(ByVal IDEmployee As Integer, ByRef EmployeeLockDateType As Boolean, ByRef oState As roBusinessState, ByVal bolAudit As Boolean)
                                                                                                             Return dDate
                                                                                                         End Function
    End Function

    Function GetDatesOfContractInDateFake(dStartDateContract As Date, dEndDateContract As Date)
        Fakes.ShimroBusinessSupport.GetDatesOfContractInDateInt32DateTimeroBusinessState = Function(IDEmployee As Integer, InDate As DateTime, oState As roBusinessState)
                                                                                               Dim retValue As New List(Of DateTime)

                                                                                               If InDate >= dStartDateContract AndAlso InDate <= dEndDateContract Then
                                                                                                   retValue.Add(dStartDateContract)
                                                                                                   retValue.Add(dEndDateContract)
                                                                                               End If

                                                                                               SearchForGetDatesOfContractInDate = True

                                                                                               Return retValue
                                                                                           End Function
    End Function

    Function GetLastContractDatesInPeriodStub(dStartDateContract As Date, dEndDateContract As Date)
        Fakes.ShimroBusinessSupport.GetLastContractDatesInPeriodInt32DateTimeDateTimeroBusinessState = Function(IDEmployee As Integer, BeginDate As Date, EndDate As Date, oState As roBusinessState)
                                                                                                           Dim retValue As New List(Of DateTime)

                                                                                                           retValue = roBusinessSupport.GetPeriodsIntersection(dStartDateContract, dEndDateContract, BeginDate, EndDate)

                                                                                                           If retValue.Count = 0 Then
                                                                                                               retValue.Add(New DateTime(1900, 1, 1, 0, 0, 0))
                                                                                                               retValue.Add(New DateTime(1900, 1, 1, 0, 0, 0))
                                                                                                           End If

                                                                                                           SearchForGetLastContractDatesInPeriod = True

                                                                                                           Return retValue
                                                                                                       End Function
    End Function

    Function GetDatesOfAnnualWorkPeriodsInDateFake(dStartDateContract As Date, dEndDateContract As Date)
        Fakes.ShimroBusinessSupport.GetDatesOfAnnualWorkPeriodsInDateInt32DateTimeroBusinessState = Function(IDEmployee As Integer, InDate As DateTime, oState As roBusinessState)
                                                                                                        Dim retValue As New List(Of DateTime)

                                                                                                        If InDate >= dStartDateContract AndAlso InDate <= dEndDateContract Then

                                                                                                            Dim tmpDate As New DateTime(InDate.Year, dStartDateContract.Month, dStartDateContract.Day)

                                                                                                            If InDate > tmpDate Then
                                                                                                                retValue.Add(New DateTime(InDate.Year, dStartDateContract.Month, dStartDateContract.Day))
                                                                                                                retValue.Add(New DateTime(InDate.Year, dStartDateContract.Month, dStartDateContract.Day).AddYears(1).AddDays(-1))
                                                                                                            Else
                                                                                                                retValue.Add(New DateTime(InDate.Year - 1, dStartDateContract.Month, dStartDateContract.Day))
                                                                                                                retValue.Add(New DateTime(InDate.Year - 1, dStartDateContract.Month, dStartDateContract.Day).AddYears(1).AddDays(-1))
                                                                                                            End If

                                                                                                        End If

                                                                                                        NumberOfSearchForGetDatesOfAnnualWorkPeriodsInDateCall += 1

                                                                                                        SearchForGetDatesOfAnnualWorkPeriodsInDate = True

                                                                                                        Return retValue
                                                                                                    End Function
    End Function

    Function GetDatesOfAnnualWorkPeriodsInDateStub(lstPeriods As List(Of List(Of Date)))
        Fakes.ShimroBusinessSupport.GetDatesOfAnnualWorkPeriodsInDateInt32DateTimeroBusinessState = Function(IDEmployee As Integer, InDate As DateTime, oState As roBusinessState)
                                                                                                        Dim retValue As New List(Of Date)
                                                                                                        retValue = lstPeriods.First(Function(x) x(0) <= InDate AndAlso x(1) >= InDate)
                                                                                                        Return retValue
                                                                                                    End Function
    End Function

    Function XGetPeriodDatesByContractAtDateCheckSummaryTypeFake(expected As SummaryType)
        Fakes.ShimroBusinessSupport.GetPeriodDatesByContractAtDateSummaryTypeInt32DateTimeroBusinessStateBoolean = Function(iSummaryType As SummaryType, IDEmployee As Integer, InDate As DateTime, oState As roBusinessState, bRaw As Boolean)
                                                                                                                       Dim retDates As New Generic.List(Of DateTime)
                                                                                                                       'TODO Mockear lo que se necesite

                                                                                                                       If iSummaryType = expected Then
                                                                                                                           ValidSummaryTypeInGetPeriodDatesByContractAtDate = True
                                                                                                                       End If

                                                                                                                       Return retDates
                                                                                                                   End Function
    End Function

    Function GetPeriodDatesByContractAtDateSpy()
        Fakes.ShimroBusinessSupport.GetPeriodDatesByContractAtDateSummaryTypeInt32DateTimeroBusinessStateBoolean = Function(iSummaryType As SummaryType, IDEmployee As Integer, InDate As DateTime, oState As roBusinessState, bRaw As Boolean)
                                                                                                                       GetPeriodDatesByContractAtDateCalled = True
                                                                                                                       Return New List(Of Date)
                                                                                                                   End Function
    End Function

    Function GetPeriodDatesByContractAtDateStub(Optional lstDates As List(Of DateTime) = Nothing)
        Fakes.ShimroBusinessSupport.GetPeriodDatesByContractAtDateSummaryTypeInt32DateTimeroBusinessStateBoolean = Function(type As SummaryType, ByVal IDEmployee As Integer, ByVal InDate As DateTime, ByVal oState As roBusinessState, ByVal bRaw As Boolean)
                                                                                                                       Dim retDates As New Generic.List(Of DateTime)
                                                                                                                       If (lstDates IsNot Nothing) Then
                                                                                                                           retDates = lstDates
                                                                                                                       Else
                                                                                                                           retDates.Add(New DateTime(2023, 3, 5))
                                                                                                                           retDates.Add(New DateTime(2023, 7, 31))
                                                                                                                       End If

                                                                                                                       Return retDates
                                                                                                                   End Function

    End Function

    Function GetDatesOfAnnualWorkPeriodsInDateSpy()
        Fakes.ShimroBusinessSupport.GetDatesOfAnnualWorkPeriodsInDateInt32DateTimeroBusinessState = Function(IDEmployee As Integer, InDate As DateTime, oState As roBusinessState)
                                                                                                        GetDatesOfAnnualWorkPeriodsInDateCalled = True
                                                                                                        Return New List(Of Date)
                                                                                                    End Function
    End Function

    Function SaveEmployeeSpy()
        VTEmployees.Employee.Fakes.ShimroEmployee.SaveEmployeeroEmployeeRefroEmployeeStateRefBooleanBoolean = Function(ByRef oEmployee As Employee.roEmployee, ByRef oState As Employee.roEmployeeState, ByVal bolAudit As Boolean, ByVal bolSavePhoto As Boolean)
                                                                                                                  SaveEmployeeCalled = True
                                                                                                                  Return True
                                                                                                              End Function
    End Function

    Function SaveEmployeeUserField()
        VTUserFields.UserFields.Fakes.ShimroEmployeeUserField.AllInstances.SaveBooleanBooleanBoolean = Function(oEmployeeUserField As VTUserFields.UserFields.roEmployeeUserField, bolAudit As Boolean, bolSavePhoto As Boolean, bolCheckUsedInProcess As Boolean)
                                                                                                           Return True
                                                                                                       End Function

    End Function

    Function SaveEmployeeUserFieldSpy()
        VTUserFields.UserFields.Fakes.ShimroEmployeeUserField.AllInstances.SaveBooleanBooleanBoolean = Function(oEmployeeUserField As VTUserFields.UserFields.roEmployeeUserField, bolAudit As Boolean, bolSavePhoto As Boolean, bolCheckUsedInProcess As Boolean)
                                                                                                           EntraUserFieldSave = oEmployeeUserField.FieldValue
                                                                                                           Return True
                                                                                                       End Function

    End Function

    Function SaveUserField()
        VTUserFields.UserFields.Fakes.ShimroUserField.AllInstances.SaveBoolean = Function(oUserField As VTUserFields.UserFields.roUserField, bolAudit As Boolean)
                                                                                     SavedUserField = oUserField
                                                                                     Return True
                                                                                 End Function

    End Function

    Function GetUserFieldsStub()
        VTUserFields.UserFields.Fakes.ShimroUserField.GetUserFieldsTypesroUserFieldStateStringBooleanBooleanBoolean = Function()
                                                                                                                          Dim oTable As New DataTable()
                                                                                                                          oTable.Columns.Add("FieldName")
                                                                                                                          Return oTable
                                                                                                                      End Function

    End Function

    Function GetCompanyByName()
        VTBusiness.Group.Fakes.ShimroGroup.GetCompanyByNameStringroGroupState = Function(sName As String, oState As VTBusiness.Group.roGroupState)
                                                                                    Dim retValue As New VTBusiness.Group.roGroup
                                                                                    retValue.ID = 1
                                                                                    retValue.Name = "Test"
                                                                                    Return retValue
                                                                                End Function
    End Function

    Function SaveGroupUserField()
        VTUserFields.UserFields.Fakes.ShimroGroupUserField.SaveUserFieldInt32StringObjectroUserFieldStateBoolean = Function(iIDGroup As Integer, sName As String, oValue As Object, oState As VTUserFields.UserFields.roUserFieldState, bolAudit As Boolean)
                                                                                                                       Return True
                                                                                                                   End Function
    End Function

    Function SaveMobility()
        VTEmployees.Employee.Fakes.ShimroMobility.SaveMobilityInt32roMobilityroEmployeeStateRefBooleanBoolean = Function(iIDMobility As Integer, oMobility As VTEmployees.Employee.roMobility, ByRef oState As VTEmployees.Employee.roEmployeeState, bolAudit As Boolean, bolSavePhoto As Boolean)
                                                                                                                    Return True
                                                                                                                End Function
    End Function

    Function SaveContract()
        VTEmployees.Contract.Fakes.ShimroContract.AllInstances.SaveBooleanBoolean = Function(oContract As VTEmployees.Contract.roContract, bolAudit As Boolean, bolSavePhoto As Boolean)
                                                                                        Return True
                                                                                    End Function
    End Function

    Function GetEmployee()
        VTEmployees.Employee.Fakes.ShimroEmployee.GetEmployeeInt32roEmployeeStateRefBoolean = Function(iIDEmployee As Integer, ByRef oState As VTEmployees.Employee.roEmployeeState, bolAudit As Boolean)
                                                                                                  Dim retValue As New VTEmployees.Employee.roEmployee
                                                                                                  retValue.ID = 1
                                                                                                  retValue.Name = "Test"
                                                                                                  Return retValue
                                                                                              End Function
    End Function

    Function GetEmployeesStub()
        VTBusiness.Common.Fakes.ShimroBusinessSupport.GetEmployeesStringStringStringroBusinessStateBoolean = Function(sName As String, sSurname As String, sSecondSurname As String, oState As roBusinessState, bolAudit As Boolean)
                                                                                                                 Dim stubDataTable As New System.Data.DataTable
                                                                                                                 stubDataTable.Columns.Add("IDEmployee")
                                                                                                                 stubDataTable.Rows.Add(1)
                                                                                                                 Return stubDataTable
                                                                                                             End Function
    End Function

    Function GetEmployeeUserFieldValueAtDateStub(ByVal parameters As Dictionary(Of String, String))
        VTUserFields.UserFields.Fakes.ShimroEmployeeUserField.GetEmployeeUserFieldValueAtDateStringStringDateTimeroUserFieldStateBoolean = Function(IdEmployee As String, FieldName As String, dDate As DateTime, oState As VTUserFields.UserFields.roUserFieldState, bolAudit As Boolean)
                                                                                                                                               Dim retValue As New VTUserFields.UserFields.roEmployeeUserField
                                                                                                                                               If parameters.ContainsKey(FieldName) Then
                                                                                                                                                   retValue.FieldValue = parameters(FieldName)
                                                                                                                                               Else
                                                                                                                                                   retValue.FieldValue = 1
                                                                                                                                               End If

                                                                                                                                               Return retValue
                                                                                                                                           End Function
    End Function

    Function UserFieldLoadStub()
        VTUserFields.UserFields.Fakes.ShimroEmployeeUserField.AllInstances.LoadBoolean = Function(oEmployeeUserField As VTUserFields.UserFields.roEmployeeUserField, bolAudit As Boolean)
                                                                                             Return True
                                                                                         End Function
    End Function

    Function UpdateEmployeePhoto()
        VTEmployees.Employee.Fakes.ShimroEmployee.AllInstances.ImageSetByteArray = Function(oEmployee As VTEmployees.Employee.roEmployee, oImage As Byte())
                                                                                       Image = oImage
                                                                                       UpdateEmployeeImageCalled = True
                                                                                   End Function
    End Function


    Public Sub AddSearchEmployeeCounterByNifSpy()
        Robotics.Base.VTUserFields.UserFields.Fakes.ShimroEmployeeUserField.GetIDEmployeesFromUserFieldValueStringStringDateTimeroUserFieldState =
                        Function(ByVal strUserField As String, ByVal strUserFieldValue As String, ByVal xDate As Date, ByVal _State As roUserFieldState)
                            Dim stubDataTable As New System.Data.DataTable()
                            stubDataTable.Columns.Add("idemployee")
                            stubDataTable.Rows.Add("115")

                            If strUserField = "NIF" Then
                                NifSearchCounter += 1
                            End If

                            Return stubDataTable
                        End Function
    End Sub

End Class