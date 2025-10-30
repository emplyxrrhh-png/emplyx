Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace BusinessLogicLayer

    ''' <summary>
    ''' Modelo de datos para empleados en todos los terminales
    ''' </summary>
    Public Class EmployeesFile

        Private slEmployees As New SortedList
        Public HasChange As Boolean = False

        Public Sub Add(ByVal EmployeeID As Integer, ByVal EmployeeName As String, ByVal EmployeeImage As Image, ByVal LanguageKey As String, Optional ByVal PIN As String = "", Optional ByVal Merge As Integer = 0, Optional ByVal AllowedCauses As String = "", Optional ByVal IsOnline As Boolean = False, Optional ByVal ConsentRequired As Boolean = True, Optional sEmployeeImageCRC As String = "")
            Dim oEmployee As New Employee
            Dim sID As String
            sID = EmployeeID.ToString
            oEmployee.EmployeeID = EmployeeID
            oEmployee.EmployeeName = EmployeeName
            oEmployee.EmployeeImage = EmployeeImage
            oEmployee.EmployeeLanguageKey = LanguageKey
            oEmployee.NewEmployee = True
            oEmployee.PIN = PIN
            oEmployee.MergeMethod = Merge
            oEmployee.AllowedCauses = AllowedCauses
            oEmployee.IsOnline = IsOnline
            oEmployee.ConsentRequired = ConsentRequired
            If Not slEmployees.ContainsKey(sID) Then
                slEmployees.Add(sID, oEmployee)
            End If
            If sEmployeeImageCRC.Length > 0 Then
                oEmployee.EmployeeImageCRC = sEmployeeImageCRC
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim NewFile As String = ""
            Dim oEmployee As Employee
            For Each oEmployee In slEmployees.Values
                NewFile += oEmployee.ToString + BCGlobal.KeyNewLine
            Next
            Return NewFile
        End Function

        Public Function SaveToDataBase(terminalId As String,
                                       ByRef oBroadcasterManager As BroadcasterManager,
                                       Optional ByVal _PhotosPath As String = "",
                                       Optional ByVal _Prefix As String = "Photo_",
                                       Optional dbConnection As DbConnection = Nothing) As String

            Dim result As Boolean = True
            Dim insertCount = 0
            Dim timeStamp As DateTime = DateTime.Now

            Dim sqlInsert As String = "@INSERT# INTO TerminalsSyncEmployeesData (EmployeeId
                                                               ,Name
                                                               ,Language
                                                               ,PIN
                                                               ,MergeMethod
                                                               ,AllowedCauses
                                                               ,IsOnline
                                                               ,ConsentRequired
                                                               ,ImageCRC
                                                               ,TerminalId
                                                               ,TimeStamp) VALUES"
            Dim sqlValuesList As List(Of String) = New List(Of String)
            Dim sqlValues As String = String.Empty

            Try

                For Each oEmployee As Employee In slEmployees.Values
                    sqlValues &= $" ({oEmployee.EmployeeID},
                              '{oEmployee.EmployeeName.Replace("'", "''")}',
                              '{oEmployee.EmployeeLanguageKey}',
                              '{oEmployee.PIN}',
                               {oEmployee.MergeMethod},
                              '{oEmployee.AllowedCauses}',
                               {If(oEmployee.IsOnline, 1, 0)},
                               {If(oEmployee.ConsentRequired, 1, 0)},
                              '{oEmployee.EmployeeImageCRC}',
                               {terminalId},
                               CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)),"

                    insertCount += 1

                    If insertCount > 300 Then
                        sqlValuesList.Add(New String(sqlValues))
                        sqlValues = String.Empty
                        insertCount = 0
                    End If
                Next

                If Not sqlValues.Equals(String.Empty) Then sqlValuesList.Add(New String(sqlValues))

                If sqlValuesList.Count > 0 Then
                    For Each values As String In sqlValuesList
                        values = values.Substring(0, values.Length - 1)
                        result = result AndAlso AccessHelper.ExecuteSql(String.Concat(sqlInsert, values))
                    Next
                Else
                    result = result AndAlso AccessHelper.ExecuteSql($"@INSERT# INTO TerminalsSyncEmployeesData (TerminalId, TimeStamp) VALUES ({terminalId}, CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121))")
                End If

                If result Then
                    'ESBORRO LES LINIES ANTIGUES
                    result = result AndAlso DeleteEmployeesXmlFromDatabase(terminalId, timeStamp, False)
                End If

                If Not result Then
                    'ALGUNA COSA HA FALLAT EN SQL, ESBORRO LES NOVES LINIES.
                    Throw New Exception("SQL Execution result = False")
                End If
            Catch Ex As Exception
                DeleteEmployeesXmlFromDatabase(terminalId, timeStamp, True)

                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeesFile::SaveToDataBase: Unexpected error: ", Ex)
                result = False
            End Try

            Return result
        End Function

        Public Function DeleteEmployeesXmlFromDatabase(terminalId As String, timeStamp As DateTime, deleteNewerLines As Boolean) As Boolean
            Dim sql As String = $"@DELETE# FROM TerminalsSyncEmployeesData WHERE TerminalId = {terminalId} AND TimeStamp {If(deleteNewerLines, ">=", "<")} CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)"

            Try
                Return AccessHelper.ExecuteSql(sql)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeesFile::DeleteEmployeesXmlFromDatabase: Unexpected error: ", Ex)
            End Try

            Return False
        End Function

        Public Function CompareXmlFromDatabase(ByVal terminalId As Integer, ByRef oBroadcasterManager As BroadcasterManager, Optional ByVal PhotoPath As String = "") As Boolean
            Dim sql As String = $"@Declare# @xmldata XML;
                                    @SELECT# @xmldata = (@SELECT# EmployeeId AS IDEmployee
                                          ,Name
                                          ,Language
                                          ,PIN
                                          ,MergeMethod
                                          ,AllowedCauses
                                          ,IsOnline
                                          ,ConsentRequired
                                          ,ImageCRC
                                      FROM TerminalsSyncEmployeesData WITH (NOLOCK) WHERE TerminalId = {terminalId} FOR XML PATH('Employees'), ROOT('LocalDataSet'))
                                    @select# @xmldata as returnXml"

            Try
                Dim ds As New LocalDataSet
                Dim oTblXML As LocalDataSet.EmployeesDataTable = ds.Employees
                Dim oRowXML As LocalDataSet.EmployeesRow
                Dim oEmployeeDB As Employee
                Dim tbRet As Object

                tbRet = AccessHelper.ExecuteScalar(sql)

                If tbRet IsNot Nothing AndAlso Not tbRet.Equals(String.Empty) AndAlso Not IsDBNull(tbRet) Then
                    If Not tbRet.Equals("<LocalDataSet><Employees /></LocalDataSet>") Then
                        tbRet = tbRet.Replace("<LocalDataSet>", "<LocalDataSet xmlns=""http://tempuri.org/LocalDataSet.xsd"">")

                        Dim memoryStream = New MemoryStream()
                        Dim streamWriter = New StreamWriter(memoryStream, System.Text.Encoding.UTF8)
                        streamWriter.Write(tbRet)
                        streamWriter.Flush()
                        memoryStream.Position = 0

                        oTblXML.ReadXml(memoryStream)
                    End If

                    Dim strPIN As String = ""
                    Dim strAllowedCauses As String = ""
                    Dim bIsOnline = False
                    Dim bConsentRequired = False
                    Dim intMerge As Integer = 0

                    For Each oRowXML In oTblXML.Rows
                        If slEmployees.ContainsKey(oRowXML.IDEmployee.ToString) Then
                            oEmployeeDB = CType(slEmployees.Item(oRowXML.IDEmployee.ToString), Employee)
                            If Not oRowXML.IsPINNull Then
                                strPIN = oRowXML.PIN
                            Else
                                strPIN = ""
                            End If
                            If Not oRowXML.IsMergeMethodNull Then
                                intMerge = oRowXML.MergeMethod
                            Else
                                intMerge = 0
                            End If
                            If Not oRowXML.IsAllowedCausesNull Then
                                strAllowedCauses = oRowXML.AllowedCauses
                            Else
                                strAllowedCauses = ""
                            End If
                            If Not oRowXML.IsIsOnlineNull Then
                                bIsOnline = oRowXML.IsOnline
                            Else
                                bIsOnline = False
                            End If
                            If Not oRowXML.IsConsentRequiredNull Then
                                bConsentRequired = oRowXML.ConsentRequired
                            Else
                                bIsOnline = False
                            End If
                            If oEmployeeDB.EmployeeName <> oRowXML.Name OrElse oEmployeeDB.PIN <> strPIN OrElse oEmployeeDB.MergeMethod <> intMerge OrElse oEmployeeDB.IsOnline <> bIsOnline OrElse oEmployeeDB.ConsentRequired <> bConsentRequired OrElse oEmployeeDB.AllowedCauses <> strAllowedCauses Then
                                'Si han cambiado el nombre crea tarea
                                oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addemployee, oRowXML.IDEmployee)
                                oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addemployee, oRowXML.IDEmployee,,,, oEmployeeDB.AllowedCauses)

                                HasChange = True
                            End If
                            Try
                                'Si hay foto en la base de datos
                                If oEmployeeDB.EmployeeImage IsNot Nothing OrElse oEmployeeDB.EmployeeImageCRC.Length > 0 Then
                                    'Si la foto contiene datos
                                    If roTypes.Image2Bytes(oEmployeeDB.EmployeeImage).Length > 0 OrElse oEmployeeDB.EmployeeImageCRC.Length > 0 Then
                                        Try
                                            'Si hay foto en el xml
                                            If Not oRowXML.IsImageNull AndAlso oRowXML.Image IsNot DBNull.Value Then
                                                If oRowXML.Image.Length > 20 Then
                                                    If oEmployeeDB.EmployeeImage Is Nothing Then
                                                        ' Paso de imagen larga a CRC
                                                        Dim strAux As String
                                                        strAux = CryptographyHelper.EncryptWithMD5(Convert.ToBase64String(oRowXML.Image))
                                                        If oEmployeeDB.EmployeeImageCRC <> strAux Then
                                                            'Si han cambiado la foto se crea tarea
                                                            oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addphoto, oRowXML.IDEmployee)
                                                            HasChange = True
                                                        End If
                                                    Else
                                                        ' No debería ocurrir
                                                        Dim bits As Byte() = roTypes.Image2Bytes(Me.FixedSize(oEmployeeDB.EmployeeImage, 200, 200, True))
                                                        Dim md5 As System.Security.Cryptography.MD5CryptoServiceProvider = New System.Security.Cryptography.MD5CryptoServiceProvider

                                                        If System.Text.Encoding.UTF8.GetString(md5.ComputeHash(bits)) <> System.Text.Encoding.UTF8.GetString(md5.ComputeHash(oRowXML.Image)) Then
                                                            'Si han cambiado la foto se crea tarea
                                                            oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addphoto, oRowXML.IDEmployee)
                                                            HasChange = True
                                                        End If

                                                    End If
                                                Else
                                                    'Hay nueva foto
                                                    oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addphoto, oRowXML.IDEmployee)
                                                    HasChange = True
                                                End If
                                            Else
                                                If oRowXML.ImageCRC IsNot DBNull.Value Then
                                                    If oRowXML.ImageCRC <> oEmployeeDB.EmployeeImageCRC Then
                                                        oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addphoto, oRowXML.IDEmployee)
                                                        HasChange = True
                                                    End If
                                                Else
                                                    oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addphoto, oRowXML.IDEmployee)
                                                    HasChange = True
                                                End If
                                            End If
                                        Catch ex As Exception
                                            'Si falla al obtener la foto del XML es que no hay foto, se envia foto nueva
                                            oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addphoto, oRowXML.IDEmployee)
                                            HasChange = True
                                        End Try
                                    End If
                                End If
                            Catch ex As Exception
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeesFile::CompareXml:Compare image (" + oEmployeeDB.EmployeeID.ToString + "):Unexpected error: ", ex)
                            End Try
                            oEmployeeDB.NewEmployee = False
                        Else
                            'Si no existe actualmente lo borra
                            oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delemployee, oRowXML.IDEmployee)
                            oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delphoto, oRowXML.IDEmployee)
                            HasChange = True
                        End If
                    Next
                Else
                    'Si no exitia el anterior fichero borra todos los empleados
                    oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delallemployees, 0)
                    oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delallphotos, 0)
                    HasChange = True
                End If
                'Buscamos los empleados nuevos que no existian anteriormente
                For Each oEmployeeDB In slEmployees.Values
                    If oEmployeeDB.NewEmployee Then
                        'Si no existe actualmente lo añade
                        oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addemployee, oEmployeeDB.EmployeeID)
                        oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addemployee, oEmployeeDB.EmployeeID,,,, oEmployeeDB.AllowedCauses)
                        'Si tiene foto
                        If oEmployeeDB.EmployeeImage IsNot Nothing OrElse oEmployeeDB.EmployeeImageCRC IsNot Nothing Then
                            'Si la foto es valida
                            If (oEmployeeDB.EmployeeImage IsNot Nothing AndAlso roTypes.Image2Bytes(oEmployeeDB.EmployeeImage).Length > 20) OrElse (oEmployeeDB.EmployeeImageCRC IsNot Nothing AndAlso oEmployeeDB.EmployeeImageCRC.Length > 0) Then
                                oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addphoto, oEmployeeDB.EmployeeID)
                            End If
                        End If
                        HasChange = True
                    End If
                Next
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"EmployeesFile::CompareXmlFromDatabase: Unexpected error generating sync tasks for terminal {terminalId}: ", Ex)
            End Try

            Return HasChange
        End Function

        Public Function FixedSize(ByVal imgPhoto As Image, ByVal Width As Integer, ByVal Height As Integer, Optional ByVal bolPercent As Boolean = True) As Image

            Dim sourceWidth As Integer = imgPhoto.Width
            Dim sourceHeight As Integer = imgPhoto.Height
            Dim sourceX As Integer = 0
            Dim sourceY As Integer = 0
            Dim destX As Integer = 0
            Dim destY As Integer = 0

            Dim nPercent As Double = 0
            Dim nPercentW As Double = 0
            Dim nPercentH As Double = 0

            Dim destWidth As Integer
            Dim destHeight As Integer

            If bolPercent Then

                nPercentW = (CType(Width, Double) / CType(sourceWidth, Double))
                nPercentH = (CType(Height, Double) / CType(sourceHeight, Double))

                'if we have to pad the height pad both the top and the bottom
                'with the difference between the scaled height and the desired height
                If (nPercentH < nPercentW) Then
                    nPercent = nPercentH
                    destX = ((Width - (sourceWidth * nPercent)) / 2)
                Else
                    nPercent = nPercentW
                    destY = ((Height - (sourceHeight * nPercent)) / 2)
                End If

                destWidth = (sourceWidth * nPercent)
                destHeight = (sourceHeight * nPercent)
            Else

                destWidth = Width
                destHeight = Height
                destX = 0
                destY = 0

            End If

            Dim bmPhoto As Bitmap = New Bitmap(Width, Height, Imaging.PixelFormat.Format24bppRgb)
            'bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution)

            Dim grPhoto As Graphics = Graphics.FromImage(bmPhoto)
            grPhoto.Clear(Color.LightGray)
            'grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic

            grPhoto.DrawImage(imgPhoto,
             New Rectangle(destX, destY, destWidth, destHeight),
             New Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
             GraphicsUnit.Pixel)

            grPhoto.Dispose()
            Return bmPhoto

        End Function

    End Class

End Namespace