Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports SwiftExcel

Namespace DataLink
    Public Class ProfileExportFree

#Region "Declarations- Constructor"

        Private mEmployeesFilter As String = ""
        Private mBeginDay As Integer = 0
        Private mBeginDate As Date = #12:00:00 AM#
        Private mEndDate As Date = #12:00:00 AM#
        Private mBody As ProfileExportBody
        Private oState As roDataLinkState
        Private mDataSourcePath As String = String.Empty

        ' Lanzamiento manual
        Public Sub New(ByVal EmployeesFilter As String, OutputFileName As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal sDatasourcePath As String, _State As roDataLinkState)
            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeesFilter = EmployeesFilter
            mBeginDate = BeginDate
            mEndDate = EndDate
            mBody = New ProfileExportBody(OutputFileName, ProfileExportBody.FileTypeExport.typ_2007, "", _State)
            mDataSourcePath = sDatasourcePath
        End Sub

#End Region

#Region "Properties"

        Public Property State() As roDataLinkState
            Get
                Return Me.oState
            End Get
            Set(ByVal NewValue As roDataLinkState)

            End Set
        End Property

        Public Property Profile() As ProfileExportBody
            Get
                Return Me.mBody
            End Get
            Set(ByVal value As ProfileExportBody)
                Me.mBody = value
            End Set
        End Property

        Public Property DatasourcePath() As String
            Get
                Return mDataSourcePath
            End Get
            Set(value As String)
                mDataSourcePath = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function ExportProfile() As Boolean
            Dim bolCloseFile As Boolean = False
            Dim bolRet As Boolean = False

            Try

                ' Determina la fecha inicial y final para importación automática
                If Me.mBeginDate = #12:00:00 AM# Then
                    ' Importación automática
                    Me.mBeginDate = CDate(Now.Year & "/" & Now.Month & "/" & 1)

                    ' La exportación siempre es del mes anterior
                    Me.mBeginDate = Me.mBeginDate.AddMonths(-1)

                    ' Si el dia de inicio de exportación es posterior al dia en que se lanza se debe restar un mes adicional para obtener el valor del mes entero.
                    If Now.Day < Me.mBeginDay Then Me.mBeginDate = Me.mBeginDate.AddMonths(-1)
                    Me.mEndDate = Me.mBeginDate.AddMonths(1)
                    Me.mEndDate = Me.mEndDate.AddDays(-1)
                End If

                ' Campos de la ficha
                Dim dtData As DataTable = Nothing

                ' Selecciona los empleados con contrato activo
                Dim sSQL As String = ""
                sSQL = IO.File.ReadAllText(Me.mDataSourcePath)

                ' Seguridad muy muy básica
                If sSQL.ToUpper.Contains("INSERT") OrElse sSQL.ToUpper.Contains("UPDATE") OrElse sSQL.ToUpper.Contains("DELETE") Then
                    Me.oState.Result = DataLinkResultEnum.IllegalStatement
                    Return False
                End If

                ' Incorporo filtro de empleados
                sSQL = sSQL.Replace("@employees", "@SELECT# * FROM [dbo].[SplitString] (@employees,',')")

                Dim sqlParameters As New List(Of DataLayer.CommandParameter)
                sqlParameters.Add(New DataLayer.CommandParameter("@datebegin", DataLayer.CommandParameter.ParameterType.tDateTime, mBeginDate))
                sqlParameters.Add(New DataLayer.CommandParameter("@dateend", DataLayer.CommandParameter.ParameterType.tDateTime, mEndDate))
                sqlParameters.Add(New DataLayer.CommandParameter("@employees", DataLayer.CommandParameter.ParameterType.tString, Me.mEmployeesFilter))
                sqlParameters.Add(New DataLayer.CommandParameter("@employeeID", DataLayer.CommandParameter.ParameterType.tString, Me.mEmployeesFilter))

                dtData = CreateDataTable(sSQL, sqlParameters, Nothing, "Data", False)

                Dim customizationCode As String = roBusinessSupport.GetCustomizationCode().ToUpper()

                If roTypes.Any2String(customizationCode) = "RAELABAUTUM" Then

                    Using ew As New ExcelWriter(Me.Profile.OutputFileName)

                        For i = 0 To Me.Profile.Fields.Count - 1
                            ew.Write(Me.Profile.Fields(i).Head, i + 1, 1)
                        Next i
                        Me.Profile.CurrentExcelRow = 1
                        For Each Row As DataRow In dtData.Rows

                            If Not LoadInfo(Row) Then Exit For

                            For Each formattedCell In Me.Profile.GetExcelCells(True)
                                ew.Write(formattedCell.Value.ToString(), formattedCell.Key, Me.Profile.CurrentExcelRow())
                            Next
                        Next

                    End Using
                Else

                    ' Crea el fichero de salida
                    If Profile.FileOpen() = False Then Exit Try
                    bolCloseFile = True

                    ' Para cada empleado, contrato y rotura
                    For Each Row As DataRow In dtData.Rows

                        ' Carga datos del registro de empleado
                        If Not LoadInfo(Row) Then Exit For
                        Me.Profile.CreateLine()
                    Next

                End If

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportCostCenters:ExportProfile")
            Finally
                ' Cierra el fichero
                If bolCloseFile Then Me.Profile.FileClose()

            End Try

            Return bolRet

        End Function

        Private Function JsonToDatatable() As DataTable
            Try
                Return JsonConvert.DeserializeObject(Of DataTable)(System.IO.File.ReadAllText("datatable.json"))
            Catch ex As Exception
                Return Nothing
            End Try

        End Function

        Private Sub DatatableToJson(ByRef dt As DataTable)
            Dim JSONresult As String
            JSONresult = JsonConvert.SerializeObject(dt)

            Dim objWriter As New System.IO.StreamWriter("datatable.json")

            objWriter.Write(JSONresult)
            objWriter.Close()

        End Sub

        Private Function LoadInfo(ByVal row As DataRow) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim i As Integer = 0

                Dim sRowname As String = String.Empty
                Dim scriptFileName As String = String.Empty

                ' Para cada columna
                For i = 0 To Me.Profile.Fields.Count - 1
                    Profile.Fields(i).Value = ""
                    sRowname = Profile.Fields(i).Source.Replace("ROW_", "")

                    If Not InStr(1, Profile.Fields(i).Source.ToUpper, "RBSB_") Then
                        Profile.Fields(i).Value = row(sRowname)
                    Else
                        scriptFileName = Profile.Fields(i).Source.Substring(5, Profile.Fields(i).Source.Length - 5)
                        Profile.Fields(i).Value = "RBS not supported"
                    End If
                Next

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportFree:LoadInfo")
            End Try

            Return bolRet
        End Function

#End Region

    End Class
End Namespace