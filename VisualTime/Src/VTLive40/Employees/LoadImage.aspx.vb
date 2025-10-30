Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Web.Base

Partial Class LoadImage
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.NameFoto"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Dim intIDEmployee As Integer = Val(Request.Params("IDEmployee"))
        'If intIDEmployee > 0 Then

        '    Dim tb As DataTable = CreateDataTable("@SELECT# Image FROM Employees WHERE [ID] = " & intIDEmployee.ToString)
        '    If tb IsNot Nothing AndAlso tb.Rows.Count = 1 AndAlso Not IsDBNull(tb.Rows(0).Item("Image")) Then
        '        Response.Clear()
        '        Response.ContentType = "image/jpeg"
        '        Response.BinaryWrite(tb.Rows(0).Item("Image"))
        '    Else
        '        Dim oStrm As New System.IO.FileStream("C:\Work\VisualTime Live\Dev\WebPortal\Images\Empleado-48x48.gif", IO.FileMode.Open, IO.FileAccess.Read)
        '        Dim oReader As New System.IO.BinaryReader(oStrm)
        '        Dim bPhoto(oReader.BaseStream.Length - 1) As Byte
        '        oReader.Read(bPhoto, 0, oReader.BaseStream.Length)
        '        Response.Clear()
        '        Response.ContentType = "image/gif"
        '        Response.BinaryWrite(bPhoto)
        '        oReader.Close()
        '        oStrm.Close()
        '    End If

        'End If

        Dim intIDEmployee As Integer = Val(Request.Params("IDEmployee"))

        If Me.GetFeaturePermissionByEmployee(FeatureAlias, intIDEmployee) = Permission.None Then
            intIDEmployee = 0
        End If

        Dim bolNoFoto As Boolean = True

        If intIDEmployee > 0 Then
            Dim oEmployee As roEmployee = Session("Employees_CurrentEmployee")
            If oEmployee Is Nothing OrElse oEmployee.ID <> intIDEmployee Then

                'Dim oServiceEmployee As EmployeeService.wsEmployeeWse = WebServices.EmployeeService
                'Dim oState As New  EmployeeSvc.roEmployeeState
                'oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                'oEmployee = oServiceEmployee.GetEmployee(intIDEmployee, oState, False)

                oEmployee = API.EmployeeServiceMethods.GetEmployee(Me, intIDEmployee, False)

            End If
            If oEmployee IsNot Nothing Then
                If oEmployee.Image IsNot Nothing Then
                    Response.Clear()
                    Response.ContentType = "image/gif"
                    Response.BinaryWrite(oEmployee.Image)
                    bolNoFoto = False
                End If
            End If
        End If

        If bolNoFoto Then

            Dim oStrm As New System.IO.FileStream(Me.Server.MapPath("~\Base\Images\userDefault.png"), IO.FileMode.Open, IO.FileAccess.Read)
            Dim oReader As New System.IO.BinaryReader(oStrm)
            Dim bPhoto(oReader.BaseStream.Length - 1) As Byte
            oStrm.Read(bPhoto, 0, bPhoto.Length)

            oReader.Read(bPhoto, 0, oReader.BaseStream.Length)
            Response.Clear()
            Response.ContentType = "image/png"
            Response.BinaryWrite(bPhoto)
            oReader.Close()
            oStrm.Close()

        End If

    End Sub

End Class