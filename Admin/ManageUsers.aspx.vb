Partial Class Admin_ManageUsers
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim roleStr As String = ""
        If Session("Role") IsNot Nothing Then roleStr = Session("Role").ToString()
        If roleStr <> "Admin" Then Response.Redirect("~/Login.aspx") : Return

        If Not IsPostBack Then
            LoadSubjects()
            LoadUsers()
        End If
    End Sub

    Private Sub LoadSubjects()
        Dim dt = DBHelper.GetDataTable("SELECT SubjectID, SubjectName FROM Subjects ORDER BY SubjectName")
        ddlSubject.Items.Clear()
        ddlSubject.Items.Add(New System.Web.UI.WebControls.ListItem("- None -", ""))
        For Each row As System.Data.DataRow In dt.Rows
            Dim txt As String = row("SubjectName").ToString()
            Dim val As String = row("SubjectID").ToString()
            Dim item As New System.Web.UI.WebControls.ListItem(txt, val)
            ddlSubject.Items.Add(item)
        Next
    End Sub

    Private Sub LoadUsers()
        Dim role = ddlFilter.SelectedValue
        Dim sql As String = _
            "SELECT u.UserID, u.FullName, u.Username, u.Role, " & _
            "       s.SubjectName, u.IsActive, u.CreatedAt " & _
            "FROM Users u " & _
            "LEFT JOIN Subjects s ON u.SubjectID = s.SubjectID"
            
        If role <> "" Then sql &= " WHERE u.Role = @role"
        sql &= " ORDER BY u.Role, u.FullName"

        Dim params As New List(Of System.Data.SqlClient.SqlParameter)
        If role <> "" Then params.Add(DBHelper.Param("@role", role))

        gvUsers.DataSource = DBHelper.GetDataTable(sql, params.ToArray())
        gvUsers.DataBind()
    End Sub

    Protected Sub btnShowAdd_Click(sender As Object, e As EventArgs)
        hfEditID.Value    = "0"
        litFormTitle.Text = " Add New User"
        txtFullName.Text  = "" : txtUsername.Text = "" : txtPassword.Text = ""
        ddlRole.SelectedIndex = 0 : chkActive.Checked = True
        ddlSubject.SelectedIndex = 0
        pnlSubject.Visible = False
        pnlForm.Visible    = True
    End Sub

    Protected Sub btnCancelForm_Click(sender As Object, e As EventArgs)
        pnlForm.Visible = False
    End Sub

    Protected Sub ddlRole_Changed(sender As Object, e As EventArgs)
        pnlSubject.Visible = (ddlRole.SelectedValue = "Teacher" OrElse ddlRole.SelectedValue = "Student")
    End Sub

    Protected Sub ddlFilter_Changed(sender As Object, e As EventArgs)
        LoadUsers()
    End Sub

    Protected Sub gvUsers_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs)
        Dim uid = CInt(e.CommandArgument)

        If e.CommandName = "ToggleActive" Then
            DBHelper.ExecuteNonQuery("UPDATE Users SET IsActive = 1 - IsActive WHERE UserID=@id",
                DBHelper.Param("@id", uid))
            ShowMsg("User status updated.", "success")
            LoadUsers()

        ElseIf e.CommandName = "EditUser" Then
            Dim dt = DBHelper.GetDataTable( _
                "SELECT u.*, s.SubjectID AS SID FROM Users u LEFT JOIN Subjects s ON u.SubjectID=s.SubjectID WHERE u.UserID=@id",
                DBHelper.Param("@id", uid))
            If dt.Rows.Count = 0 Then Return
            Dim row = dt.Rows(0)

            hfEditID.Value    = uid.ToString()
            litFormTitle.Text = " Edit User"
            txtFullName.Text  = row("FullName").ToString()
            txtUsername.Text  = row("Username").ToString()
            txtPassword.Text  = ""  ' Don't pre-fill password
            chkActive.Checked = CBool(row("IsActive"))

            Dim roleItem = ddlRole.Items.FindByValue(row("Role").ToString())
            If roleItem IsNot Nothing Then roleItem.Selected = True
            pnlSubject.Visible = (row("Role").ToString() = "Teacher" OrElse row("Role").ToString() = "Student")

            If row("SubjectID") IsNot DBNull.Value Then
                Dim si = ddlSubject.Items.FindByValue(row("SubjectID").ToString())
                If si IsNot Nothing Then si.Selected = True
            Else
                ddlSubject.SelectedIndex = 0
            End If

            pnlForm.Visible = True
        End If
    End Sub

    Protected Sub btnSaveUser_Click(sender As Object, e As EventArgs)
        pnlMsg.Visible = False
        Dim editID  = CInt(hfEditID.Value)
        Dim name    = txtFullName.Text.Trim()
        Dim uname   = txtUsername.Text.Trim()
        Dim pwd     = txtPassword.Text.Trim()
        Dim role    = ddlRole.SelectedValue
        Dim subID   As Object = DBNull.Value
        Dim active  = chkActive.Checked

        If String.IsNullOrEmpty(name) OrElse String.IsNullOrEmpty(uname) Then
            ShowMsg("Full name and username are required.", "danger") : Return
        End If
        If editID = 0 AndAlso String.IsNullOrEmpty(pwd) Then
            ShowMsg("Password is required for new users.", "danger") : Return
        End If

        If (role = "Teacher" OrElse role = "Student") AndAlso ddlSubject.SelectedValue <> "" Then
            subID = CInt(ddlSubject.SelectedValue)
        End If

        Try
            If editID = 0 Then
                ' Insert
                Dim dupCheck = DBHelper.Exists( _
                    "SELECT COUNT(*) FROM Users WHERE Username=@u", DBHelper.Param("@u", uname))
                If dupCheck Then ShowMsg("Username already exists.", "danger") : Return

                DBHelper.ExecuteNonQuery( _
                    "INSERT INTO Users (FullName,Username,Password,Role,SubjectID,IsActive) " & _
                    "VALUES (@n,@u,@p,@r,@s,@a)", _
                    DBHelper.Param("@n", name), DBHelper.Param("@u", uname), _
                    DBHelper.Param("@p", pwd),  DBHelper.Param("@r", role), _
                    DBHelper.Param("@s", subID), DBHelper.Param("@a", active))
                ShowMsg("User created successfully!", "success")
            Else
                ' Update
                Dim pwdSql As String
                Dim pList As New List(Of System.Data.SqlClient.SqlParameter)
                pList.Add(DBHelper.Param("@n", name))
                pList.Add(DBHelper.Param("@u", uname))
                pList.Add(DBHelper.Param("@r", role))
                pList.Add(DBHelper.Param("@s", subID))
                pList.Add(DBHelper.Param("@a", active))
                pList.Add(DBHelper.Param("@id", editID))

                If String.IsNullOrEmpty(pwd) Then
                    pwdSql = "UPDATE Users SET FullName=@n,Username=@u,Role=@r,SubjectID=@s,IsActive=@a WHERE UserID=@id"
                Else
                    pwdSql = "UPDATE Users SET FullName=@n,Username=@u,Password=@p,Role=@r,SubjectID=@s,IsActive=@a WHERE UserID=@id"
                    pList.Insert(2, DBHelper.Param("@p", pwd))
                End If

                DBHelper.ExecuteNonQuery(pwdSql, pList.ToArray())
                ShowMsg("User updated successfully!", "success")
            End If

            pnlForm.Visible = False
            LoadUsers()

        Catch ex As Exception
            ShowMsg("Error: " & ex.Message, "danger")
        End Try
    End Sub

    Private Sub ShowMsg(msg As String, kind As String)
        pnlMsg.Visible    = True
        pnlMsg.CssClass   = String.Format("alert alert-{0} mb-4", kind)
        litMsg.Text       = msg
    End Sub

End Class
