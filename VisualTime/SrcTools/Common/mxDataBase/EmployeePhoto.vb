Imports System.Data.SQLite

Namespace Database

    Public Class clsEmployeePhoto
        Private mConn As SQLiteConnection

        Private Const COL_idEmployee = 0
        Private Const COL_Photo = 1

        ''' <summary>
        ''' Se inicializan los array biometricos
        ''' </summary>
        ''' <param name="conn"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef conn As SQLiteConnection)
            Try
                mConn = conn

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeePhoto::New:", ex)
            End Try
        End Sub
#Region "Gestion de tabla"

        ''' <summary>
        ''' Crea la tabla en la BBDD
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CreateTable()
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand

                'Creamos la tabla de biometricos
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS Employeephotos (idEmployee INTEGER, Photo BLOB);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsEmployeePhoto::CreateDB:employeephoto table created.")

                SQLcommand.Dispose()


            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeePhoto::CreateTable:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Borra datos de la tabla
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ClearTable()
            Try
                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "DELETE FROM Employeephotos;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsEmployeePhoto::ClearTable:employeephoto table empty.")
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeePhoto::ClearTable:", ex)
            End Try
        End Sub

        Public Sub ClearFolder()
            Try
                For Each fil As String In IO.Directory.GetFiles(clsSystemHelper.PathEmployeePhoto)
                    If fil.IndexOf(".jpg") > 0 Then
                        Try
                            IO.File.Delete(fil)
                        Catch ex As Exception
                            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeePhoto::ClearFolder:Error deleting file (" + fil + ")", ex)
                        End Try
                    End If
                Next
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeePhoto::ClearFolder:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Añade un registro a la tabla
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <param name="Photo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function addRow(ByVal IDEmployee As Integer, ByVal Photo As Byte()) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "INSERT OR REPLACE INTO Employeephotos(idEmployee,Photo) VALUES(" + clsGlobal.Any2SQL(IDEmployee) + ",@photo);"
                Dim SQLparm As New SQLiteParameter("@photo", DbType.Binary)
                SQLparm.Value = Photo
                SQLcommand.Parameters.Add(SQLparm)
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsEmployeePhoto::addRow:New row(" + IDEmployee.ToString + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeePhoto::addRow:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Borra registro de la tabla
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function delRow(ByVal IDEmployee As Integer) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "DELETE FROM Employeephotos WHERE idEmployee=" + clsGlobal.Any2SQL(IDEmployee) + ";"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsEmployeePhoto::delRow:delete row(" + IDEmployee.ToString + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeePhoto::delRow:", ex)
                Return False
            End Try

        End Function

        Public Function getRow(ByVal IDEmployee As Integer, ByRef Photo As Byte()) As Boolean
            Try

                Dim SQLcommand As New SQLiteCommand("Select * from Employeephotos where idEmployee=" + IDEmployee.ToString, mConn)
                Dim sqReader As SQLiteDataReader = SQLcommand.ExecuteReader

                'Si hay datos los carga
                If sqReader.Read() Then
                    Dim len As Integer = 0
                    Dim bPhoto(400) As Byte

                    len = sqReader.GetBytes(COL_Photo, 0, Nothing, 0, 0)
                    ReDim bPhoto(len)
                    sqReader.GetBytes(COL_Photo, 0, bPhoto, 0, len)
                    Photo = bPhoto
                    Return True
                Else
                    Return False
                End If

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                Return False
            End Try
        End Function
#End Region

        Public Sub SavePhotos2Files(Optional ByVal DataBase As String = "", Optional ByVal DeleteOldFiles As Boolean = False)
            Try
                Dim SQLcommand As New SQLiteCommand("Select * from " + IIf(DataBase.Length > 0, DataBase + ".", "") + "Employeephotos", mConn)
                Dim sqReader As SQLiteDataReader = SQLcommand.ExecuteReader
                Dim Len As Integer
                Dim photo As Byte() = {0}
                Dim idEmployee As Integer = 0

                If DeleteOldFiles Then ClearFolder()

                While sqReader.Read
                    Len = sqReader.GetBytes(COL_Photo, 0, Nothing, 0, 0)
                    ReDim photo(Len - 1)
                    sqReader.GetBytes(COL_Photo, 0, photo, 0, Len)
                    idEmployee = clsGlobal.Any2Long(sqReader.GetValue(COL_idEmployee))

                    Dim ms As IO.FileStream = New IO.FileStream(IO.Path.Combine(clsSystemHelper.PathEmployeePhoto, idEmployee.ToString + ".jpg"), IO.FileMode.Create)
                    ms.Write(photo, 0, photo.Length)
                    ms.Close()
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsEmployeePhoto::SavePhotos2Files:Employee photo creaded.(" + idEmployee.ToString + ")")

                End While

                ClearTable()

            Catch ex As Exception

            End Try
        End Sub

    End Class
End Namespace
