<%@ Page Language="VB" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>

<!DOCTYPE html>
<html>
<head>
    <title>Test Database Connection</title>
</head>
<body>
    <h1>VisualTime Database Connection Test</h1>
    <%
        Try
            Dim connString As String = ConfigurationManager.ConnectionStrings("VisualTimeConnectionString").ConnectionString
            Response.Write("<p><strong>Connection String:</strong> " & connString & "</p>")
            
            Using conn As New SqlConnection(connString)
                conn.Open()
                Response.Write("<p style='color:green;'><strong>? Database Connection: SUCCESSFUL!</strong></p>")
                
                ' Test query
                Dim cmd As New SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", conn)
                Dim tableCount As Integer = CInt(cmd.ExecuteScalar())
                Response.Write("<p><strong>Total Tables:</strong> " & tableCount.ToString() & "</p>")
                
                ' Get DB Version
                cmd.CommandText = "SELECT Data FROM sysroParameters WHERE ID = 'DBVERSION'"
                Dim dbVersion As String = cmd.ExecuteScalar().ToString()
                Response.Write("<p><strong>Database Version:</strong> " & dbVersion & "</p>")
                
                ' Get Users Count
                cmd.CommandText = "SELECT COUNT(*) FROM sysroUsers"
                Dim userCount As Integer = CInt(cmd.ExecuteScalar())
                Response.Write("<p><strong>Total Users:</strong> " & userCount.ToString() & "</p>")
                
                conn.Close()
            End Using
            
            Response.Write("<p style='color:green;font-size:20px;'><strong>? ALL TESTS PASSED!</strong></p>")
            Response.Write("<p><a href='/' style='font-size:18px;'>Go to VisualTime Application</a></p>")
        Catch ex As Exception
            Response.Write("<p style='color:red;'><strong>? Error:</strong> " & ex.Message & "</p>")
            Response.Write("<p><strong>Stack Trace:</strong><br/><pre>" & ex.StackTrace & "</pre></p>")
        End Try
    %>
</body>
</html>
