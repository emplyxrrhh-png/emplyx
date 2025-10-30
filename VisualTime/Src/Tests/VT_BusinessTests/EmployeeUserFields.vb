Imports System.ComponentModel
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.Comms.Base
Imports VT_XU_Base
Imports VT_XU_Security

Imports Xunit

Namespace Unit.Test

    <Collection("EmployeeUserField")>
    <CollectionDefinition("EmployeeUserField", DisableParallelization:=True)>
    <Category("EmployeeUserField")>
    Public Class EmployeeUserFieldsTest

        Private ReadOnly helperDataLayer As DatalayerHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>

        Sub New()
            helperDataLayer = New DatalayerHelper
        End Sub

        <Fact(DisplayName:="Unique Constraint Breach Should Not Be Thrown If User Field Value Does Not Exists On Any Employee")>
        Public Sub UniqueConstraintBreachShouldNotBeThrownIfUserFieldValueDoesNotExistsOnAnyEmployee()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                tMock = New DataTableMock With {.columns = {"IdEmployee"},
                    .values = New Object()() {}}
                dDataTStub.Add("EmployeeUserFieldValues", tMock)
                Dim dtEmployeeUserFieldValues As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                Dim oState As New roUserFieldState(-1)
                Dim employeeUserField As New roEmployeeUserField(oState)
                employeeUserField.Definition = New roUserField()
                employeeUserField.Definition.FieldType = UserFieldsTypes.FieldTypes.tText
                employeeUserField.FieldName = "WhateverName"
                employeeUserField.FieldRawValue = "WhateverValue"
                employeeUserField.FieldValue = "WhateverValue"
                employeeUserField.IDEmployee = 1

                Dim result As Boolean = employeeUserField.ValueAlreadyExistsOnOtherEmployee()

                ' Assert
                Assert.False(result)
            End Using
        End Sub

        <Fact(DisplayName:="Unique Constraint Breach Should Not Be Thrown If User Field Value Exists But Only In Own Employee")>
        Public Sub UniqueConstraintBreachShouldNotBeThrownIfUserFieldValueExistsButOnlyInOwnEmployee()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()

                ' Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                tMock = New DataTableMock With {.columns = {"IdEmployee"},
                    .values = New Object()() {New Object() {"1"}}}
                dDataTStub.Add("EmployeeUserFieldValues", tMock)
                Dim dtEmployeeUserFieldValues As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                Dim oState As New roUserFieldState(-1)
                Dim employeeUserField As New roEmployeeUserField(oState)
                employeeUserField.Definition = New roUserField()
                employeeUserField.Definition.FieldType = UserFieldsTypes.FieldTypes.tText
                employeeUserField.FieldName = "WhateverName"
                employeeUserField.FieldRawValue = "WhateverValue"
                employeeUserField.FieldValue = "WhateverValue"
                employeeUserField.IDEmployee = 1

                Dim result As Boolean = employeeUserField.ValueAlreadyExistsOnOtherEmployee()

                ' Assert
                Assert.False(result)
            End Using
        End Sub

        <Fact(DisplayName:="Unique Constraint Breach Should Be Thrown If User Field Value Exists In Other Employees")>
        Public Sub UniqueConstraintBreachShouldBeThrownIfUserFieldValueExistsOnOtherEmployee()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()

                ' Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                tMock = New DataTableMock With {.columns = {"IdEmployee"},
                    .values = New Object()() {New Object() {"2"}, New Object() {"3"}, New Object() {"1"}}}
                dDataTStub.Add("EmployeeUserFieldValues", tMock)

                Dim dtEmployeeUserFieldValues As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                Dim oState As New roUserFieldState(-1)
                Dim employeeUserField As New roEmployeeUserField(oState)
                employeeUserField.Definition = New roUserField()
                employeeUserField.Definition.FieldType = UserFieldsTypes.FieldTypes.tText
                employeeUserField.FieldName = "WhateverName"
                employeeUserField.FieldRawValue = "WhateverValue"
                employeeUserField.FieldValue = "WhateverValue"
                employeeUserField.IDEmployee = 1

                Dim result As Boolean = employeeUserField.ValueAlreadyExistsOnOtherEmployee()

                ' Assert
                Assert.True(result)
            End Using
        End Sub

        <Fact(DisplayName:="Unique Constraint Breach Should Not Be Thrown If User Field Value Is Empty Despite Other Employees Has Also Empty value")>
        Public Sub UniqueConstraintBreachShouldNotBeThrownIfUserFieldValueIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()

                ' Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                tMock = New DataTableMock With {.columns = {"IdEmployee"},
                    .values = New Object()() {New Object() {"2"}, New Object() {"3"}, New Object() {"1"}}}
                dDataTStub.Add("GetAllEmployeeUserFieldValue", tMock)
                Dim dtEmployeeUserFieldValues As DataTable = helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                Dim oState As New roUserFieldState(-1)
                Dim employeeUserField As New roEmployeeUserField(oState)
                employeeUserField.Definition = New roUserField()
                employeeUserField.Definition.FieldType = UserFieldsTypes.FieldTypes.tText
                employeeUserField.FieldName = "WhateverName"
                employeeUserField.FieldRawValue = "WhateverValue"
                employeeUserField.FieldValue = String.Empty
                employeeUserField.IDEmployee = 1

                Dim result As Boolean = employeeUserField.ValueAlreadyExistsOnOtherEmployee()

                ' Assert
                Assert.False(result)
            End Using
        End Sub
    End Class

End Namespace