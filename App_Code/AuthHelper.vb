Imports System.Web.UI

Public Class AuthHelper

    Public Shared Sub RequireRole(page As Page, ParamArray allowedRoles() As String)
        Dim role As String = ""
        If page.Session("Role") IsNot Nothing Then role = page.Session("Role").ToString()
        If Not Array.Exists(allowedRoles, Function(r) r = role) Then
            page.Response.Redirect("Login.aspx")
        End If
    End Sub

    Public Shared Function IsRole(page As Page, role As String) As Boolean
        Return page.Session("Role") IsNot Nothing AndAlso page.Session("Role").ToString() = role
    End Function

    Public Shared Sub Logout(page As Page)
        page.Session.Clear()
        page.Session.Abandon()
        page.Response.Redirect("Login.aspx")
    End Sub

End Class
