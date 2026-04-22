Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration

''' <summary>
''' Shared database utility class for QuizWiz application.
''' All DB access goes through these helpers to keep connection management centralised.
''' </summary>
Public Class DBHelper

    ''' <summary>Returns a new (closed) SqlConnection using the Web.config connection string.</summary>
    Public Shared Function GetConnection() As SqlConnection
        Dim connStr As String = ConfigurationManager.ConnectionStrings("QuizWizDB").ConnectionString
        Return New SqlConnection(connStr)
    End Function

    ''' <summary>Executes a non-query SQL statement (INSERT / UPDATE / DELETE). Returns rows affected.</summary>
    Public Shared Function ExecuteNonQuery(sql As String, ParamArray params As SqlParameter()) As Integer
        Using conn As SqlConnection = GetConnection()
            conn.Open()
            Using cmd As New SqlCommand(sql, conn)
                cmd.CommandTimeout = 30
                If params IsNot Nothing Then cmd.Parameters.AddRange(params)
                Return cmd.ExecuteNonQuery()
            End Using
        End Using
    End Function

    ''' <summary>Executes a scalar query and returns the first column of the first row.</summary>
    Public Shared Function ExecuteScalar(sql As String, ParamArray params As SqlParameter()) As Object
        Using conn As SqlConnection = GetConnection()
            conn.Open()
            Using cmd As New SqlCommand(sql, conn)
                cmd.CommandTimeout = 30
                If params IsNot Nothing Then cmd.Parameters.AddRange(params)
                Return cmd.ExecuteScalar()
            End Using
        End Using
    End Function

    ''' <summary>Returns a filled DataTable for the given SELECT query.</summary>
    Public Shared Function GetDataTable(sql As String, ParamArray params As SqlParameter()) As DataTable
        Using conn As SqlConnection = GetConnection()
            conn.Open()
            Using cmd As New SqlCommand(sql, conn)
                cmd.CommandTimeout = 30
                If params IsNot Nothing Then cmd.Parameters.AddRange(params)
                Using da As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Return dt
                End Using
            End Using
        End Using
    End Function

    ''' <summary>Helper: wraps a value in a SqlParameter detecting type automatically.</summary>
    Public Shared Function Param(name As String, value As Object) As SqlParameter
        If value Is Nothing Then
            Return New SqlParameter(name, DBNull.Value)
        End If
        Return New SqlParameter(name, value)
    End Function

    ''' <summary>Checks whether any rows exist for the given query.</summary>
    Public Shared Function Exists(sql As String, ParamArray params As SqlParameter()) As Boolean
        Dim result As Object = ExecuteScalar(sql, params)
        If result Is Nothing OrElse result Is DBNull.Value Then Return False
        Return CInt(result) > 0
    End Function

End Class
