Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace DataAccess

    Friend NotInheritable Class ConnectionAccess
        Public Shared Function IsProductiVEmployee(ByVal idpassport As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# count(*) FROM Employees inner join sysropassports on Employees.ID = sysropassports.IDEmployee WHERE Type='J' AND sysropassports.ID =" & idpassport
                Dim isProd As Integer = VTBase.roTypes.Any2Integer(AccessHelper.ExecuteScalar(strSQL))

                If isProd = 1 Then
                    bolRet = True
                Else
                    bolRet = False
                End If
            Catch ex As DbException
                bolRet = False
            Catch ex As Exception
                bolRet = False
            Finally

            End Try

            Return bolRet
        End Function

        Public Shared Function ValidateRecoverPasswordRequestKey(ByVal idPassport As Integer, ByVal strRequestKey As String, ByVal appType As roAppType) As Boolean
            Dim oret As Boolean = False

            Dim strSQL As String

            strSQL = "@SELECT# RecoverKey FROM sysroPassports_Data WHERE recoverkey is not null and IDPassport =" & idPassport & " And AppCode='" & appType.ToString() & "'"

            Dim keyObject As Object = AccessHelper.ExecuteScalar(strSQL)

            If Not IsDBNull(keyObject) Then
                Dim strServerKey = VTBase.roTypes.Any2String(keyObject)

                oret = (strServerKey = strRequestKey)
            End If

            If oret Then
                strSQL = "@UPDATE# sysroPassports_Data SET RecoverKey = NULL WHERE IDPassport =" & idPassport & " And AppCode='" & appType.ToString() & "'"
                AccessHelper.ExecuteSql(strSQL)
            End If

            Return oret
        End Function

        Public Shared Function CreateRecoverPasswordKey(ByVal idPassport As Integer, ByVal idEmployee As Integer, ByVal appType As roAppType) As Boolean
            Dim oret As Boolean = False

            Dim strSQL As String

            strSQL = "@SELECT# count(*) FROM sysroPassports_Data where IDPassport =" & idPassport & " And AppCode='" & appType.ToString & "'"
            Dim bRegExists As Boolean = VTBase.roTypes.Any2Boolean(VTBase.roTypes.Any2String(AccessHelper.ExecuteScalar(strSQL)))

            Dim Letters As New List(Of Integer)
            'add ASCII codes for numbers
            For i As Integer = 48 To 57
                Letters.Add(i)
            Next
            'lowercase letters
            For i As Integer = 97 To 122
                Letters.Add(i)
            Next
            'uppercase letters
            For i As Integer = 65 To 90
                Letters.Add(i)
            Next
            'select 8 random integers from number of items in Letters
            'then convert those random integers to characters and
            'add each to a string and display in Textbox
            Dim Rnd As New Random
            Dim SB As New System.Text.StringBuilder
            Dim Temp As Integer
            For count As Integer = 1 To 8
                Temp = Rnd.Next(0, Letters.Count)
                SB.Append(Chr(Letters(Temp)))
            Next

            If bRegExists Then
                strSQL = "@UPDATE# sysroPassports_Data SET RecoverKey = '" & SB.ToString & "' WHERE IDPassport =" & idPassport & " And AppCode='" & appType.ToString & "'"
            Else
                strSQL = "@INSERT# INTO sysroPassports_Data(IDPassport,RecoverKey,AppCode,Id,KeyValidated) Values(" & idPassport & ",'" & SB.ToString & "','" & appType.ToString & "','" & Guid.NewGuid.ToString & "',1)"
            End If
            AccessHelper.ExecuteSql(strSQL)

            strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric) VALUES (1904, " & idEmployee & ", " & idPassport & ")"
            AccessHelper.ExecuteSql(strSQL)
            oret = True

            Return oret
        End Function

    End Class

End Namespace