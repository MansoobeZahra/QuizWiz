Imports System.Data.SqlClient

Partial Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' If already logged in, redirect
        If Session("UserID") IsNot Nothing Then
            RedirectByRole(Session("Role").ToString())
        End If
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
        Dim username As String = txtUsername.Text.Trim()
        Dim password As String = txtPassword.Text.Trim()

        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            ShowError("Please enter both username and password.")
            Return
        End If

        Try
            Dim sql As String = "SELECT UserID, FullName, Role, IsActive FROM Users WHERE Username = @u AND Password = @p"
            Dim dt = DBHelper.GetDataTable(sql,
                DBHelper.Param("@u", username),
                DBHelper.Param("@p", password))

            If dt.Rows.Count = 0 Then
                ShowError("Invalid username or password.")
                Return
            End If

            Dim row = dt.Rows(0)

            If Not CBool(row("IsActive")) Then
                ShowError("Your account has been deactivated. Please contact the administrator.")
                Return
            End If

            ' Set session variables
            Session("UserID")   = CInt(row("UserID"))
            Session("Username") = username
            Session("FullName") = row("FullName").ToString()
            Session("Role")     = row("Role").ToString()

            RedirectByRole(row("Role").ToString())

        Catch ex As Exception
            ShowError("Database error: " & ex.Message)
        End Try
    End Sub

    Private Sub RedirectByRole(role As String)
        Select Case role
            Case "Admin"   : Response.Redirect("~/Admin/Dashboard.aspx")
            Case "Teacher" : Response.Redirect("~/Teacher/Dashboard.aspx")
            Case "Student" : Response.Redirect("~/Student/Dashboard.aspx")
            Case Else      : Response.Redirect("~/Login.aspx")
        End Select
    End Sub

    Private Sub ShowError(msg As String)
        pnlError.Visible = True
        litError.Text    = msg
    End Sub

End Class
