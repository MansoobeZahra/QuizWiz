Imports System.Web.UI

Partial Class MasterPage
    Inherits System.Web.UI.MasterPage

    Public ReadOnly Property SessionRole As String
        Get
            If Session("Role") IsNot Nothing Then Return Session("Role").ToString()
            Return ""
        End Get
        
    End Property

    Public ReadOnly Property SessionFullName As String
        Get
            If Session("FullName") IsNot Nothing Then Return Session("FullName").ToString()
            Return "Guest"
        End Get
    End Property

    Public ReadOnly Property RoleCssClass As String
        Get
            Select Case SessionRole.ToLower()
                Case "admin"   : Return "role-admin"
                Case "teacher" : Return "role-teacher"
                Case "student" : Return "role-student"
                Case Else      : Return ""
            End Select
        End Get
    End Property

    ''' <summary>Returns "active" CSS class if the given virtual path matches current page.</summary>
    Public Function Active(virtualPath As String) As String
        Dim resolved As String = ResolveUrl(virtualPath).ToLower().TrimEnd("/"c)
        Dim current  As String = Request.AppRelativeCurrentExecutionFilePath.ToLower().TrimStart("~"c).TrimEnd("/"c)
        Dim resolvedClean As String = resolved.Replace(Request.ApplicationPath.ToLower().TrimEnd("/"c), "").TrimEnd("/"c)
        If resolvedClean = current OrElse ("/" & current.TrimStart("/"c)) = resolvedClean Then Return "active"
        Return ""
    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Redirect to login if not authenticated
        If Session("UserID") Is Nothing Then
            Response.Redirect("~/Login.aspx")
            Return
        End If

        Dim role As String = SessionRole
        pnlAdminNav.Visible   = (role = "Admin")
        pnlTeacherNav.Visible = (role = "Teacher")
        pnlStudentNav.Visible = (role = "Student")
    End Sub

End Class
