Public Class EmployeesHelper

    Public Property EmployeeLoaded As Boolean = False

    Public Sub New()
    End Sub

    Public Function InitializeEmployeeServiceMethods()

        Robotics.Web.Base.API.Fakes.ShimEmployeeServiceMethods.GetEmployeePageInt32Boolean =
            Function(reference, idEmployee, bAudit)
                EmployeeLoaded = True
                Return New Robotics.Base.VTEmployees.Employee.roEmployee() With {.ID = idEmployee}

            End Function
    End Function

End Class