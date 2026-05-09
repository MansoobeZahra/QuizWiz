Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration

Public Class DBHelper

    Private Shared ReadOnly _connStr As String = ConfigurationManager.ConnectionStrings("QuizWizDB").ConnectionString

    Public Shared Function GetConnection() As SqlConnection
        Dim conn As New SqlConnection(_connStr)
        conn.Open()
        Return conn
    End Function

    Public Shared Function Param(name As String, value As Object) As SqlParameter
        Return New SqlParameter(name, If(value Is Nothing, DBNull.Value, value))
    End Function

    Public Shared Function ExecuteScalar(sql As String, ParamArray params() As SqlParameter) As Object
        Using conn = GetConnection()
            Using cmd As New SqlCommand(sql, conn)
                If params IsNot Nothing Then
                    For Each p In params
                        cmd.Parameters.Add(p)
                    Next
                End If
                Return cmd.ExecuteScalar()
            End Using
        End Using
    End Function

    Public Shared Function ExecuteNonQuery(sql As String, ParamArray params() As SqlParameter) As Integer
        Using conn = GetConnection()
            Using cmd As New SqlCommand(sql, conn)
                If params IsNot Nothing Then
                    For Each p In params
                        cmd.Parameters.Add(p)
                    Next
                End If
                Return cmd.ExecuteNonQuery()
            End Using
        End Using
    End Function

    Public Shared Function GetDataTable(sql As String, ParamArray params() As SqlParameter) As DataTable
        Using conn = GetConnection()
            Using cmd As New SqlCommand(sql, conn)
                If params IsNot Nothing Then
                    For Each p In params
                        cmd.Parameters.Add(p)
                    Next
                End If
                Dim dt As New DataTable()
                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)
                Return dt
            End Using
        End Using
    End Function

End Class
