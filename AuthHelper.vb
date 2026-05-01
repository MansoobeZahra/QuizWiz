Public Class AuthHelper
    Public Shared Sub RequireRole(page As System.Web.UI.Page, ParamArray roles As String())
        If page.Session("UserID") Is Nothing Then
            page.Response.Redirect("~/Login.aspx")
            Return
        End If

        Dim userRole As String = page.Session("Role").ToString()
        Dim allowed As Boolean = False
        For Each r In roles
            If r = userRole Then
                allowed = True
                Exit For
            End If
        Next

        If Not allowed Then
            page.Response.Redirect("~/Login.aspx")
        End If
    End Sub
End Class
