Imports System.Web.UI

''' <summary>
''' Role-Based Access Control helper.
''' Call AuthHelper.RequireRole(Me, "Teacher") at the top of Page_Load on every protected page.
''' If the current user does not hold one of the allowed roles they are redirected to their
''' own dashboard (or to Login if not authenticated at all).
''' </summary>
Public Class AuthHelper

    ''' <summary>
    ''' Ensures the current session user holds one of the supplied roles.
    ''' Redirects immediately if not.
    ''' </summary>
    Public Shared Sub RequireRole(page As Page, ParamArray allowedRoles As String())
        Dim role As String = Nothing

        If page.Session("UserID") Is Nothing Then
            page.Response.Redirect("~/Login.aspx")
            Return
        End If

        If page.Session("Role") IsNot Nothing Then
            role = page.Session("Role").ToString()
        End If

        If String.IsNullOrEmpty(role) Then
            page.Response.Redirect("~/Login.aspx")
            Return
        End If

        For Each r As String In allowedRoles
            If String.Equals(r, role, StringComparison.OrdinalIgnoreCase) Then
                Return  ' Authorised — do nothing
            End If
        Next

        ' Not authorised — redirect to own dashboard
        RedirectHome(page, role)
    End Sub

    ''' <summary>Sends the user to their role's home dashboard.</summary>
    Public Shared Sub RedirectHome(page As Page, role As String)
        If role Is Nothing Then role = ""
        Select Case role.ToLower()
            Case "admin"   : page.Response.Redirect("~/Admin/Dashboard.aspx")
            Case "teacher" : page.Response.Redirect("~/Teacher/Dashboard.aspx")
            Case "student" : page.Response.Redirect("~/Student/Dashboard.aspx")
            Case Else      : page.Response.Redirect("~/Login.aspx")
        End Select
    End Sub

    ''' <summary>Returns True if the session user holds any of the supplied roles.</summary>
    Public Shared Function HasRole(page As Page, ParamArray roles As String()) As Boolean
        Dim current As String = ""
        If page.Session("Role") IsNot Nothing Then current = page.Session("Role").ToString()
        For Each r As String In roles
            If String.Equals(r, current, StringComparison.OrdinalIgnoreCase) Then Return True
        Next
        Return False
    End Function

End Class
