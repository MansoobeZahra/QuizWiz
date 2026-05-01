Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration

Public Class DBHelper
    Public Shared Function GetConnection() As SqlConnection
        Dim connStr As String = ConfigurationManager.ConnectionStrings("QuizWizDB").ConnectionString
        Return New SqlConnection(connStr)
    End Function

    Public Shared Function ExecuteNonQuery(sql As String, ParamArray params As SqlParameter()) As Integer
        Using conn As SqlConnection = GetConnection()
            conn.Open()
            Using cmd As New SqlCommand(sql, conn)
                If params IsNot Nothing Then cmd.Parameters.AddRange(params)
                Return cmd.ExecuteNonQuery()
            End Using
        End Using
    End Function

    Public Shared Function ExecuteScalar(sql As String, ParamArray params As SqlParameter()) As Object
        Using conn As SqlConnection = GetConnection()
            conn.Open()
            Using cmd As New SqlCommand(sql, conn)
                If params IsNot Nothing Then cmd.Parameters.AddRange(params)
                Return cmd.ExecuteScalar()
            End Using
        End Using
    End Function

    Public Shared Function GetDataTable(sql As String, ParamArray params As SqlParameter()) As DataTable
        Using conn As SqlConnection = GetConnection()
            conn.Open()
            Using cmd As New SqlCommand(sql, conn)
                If params IsNot Nothing Then cmd.Parameters.AddRange(params)
                Dim dt As New DataTable()
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
                Return dt
            End Using
        End Using
    End Function

    Public Shared Function Exists(sql As String, ParamArray params As SqlParameter()) As Boolean
        Dim count = ExecuteScalar(sql, params)
        Return CInt(If(count Is Nothing, 0, count)) > 0
    End Function

    Public Shared Function Param(name As String, value As Object) As SqlParameter
        Return New SqlParameter(name, If(value Is Nothing, DBNull.Value, value))
    End Function
End Class
