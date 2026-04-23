Partial Class Teacher_ManageQuestions
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Teacher")
        If Not IsPostBack Then
            LoadSubjects()
            LoadQuestions()
        End If
    End Sub

    Private Sub LoadSubjects()
        Dim dt = DBHelper.GetDataTable("SELECT SubjectID, SubjectName FROM Subjects ORDER BY SubjectName")
        ddlFilter.Items.Clear()
        ddlFilter.Items.Add(New System.Web.UI.WebControls.ListItem("All Subjects", ""))
        For Each row As System.Data.DataRow In dt.Rows
            Dim txt As String = row("SubjectName").ToString()
            Dim val As String = row("SubjectID").ToString()
            ddlFilter.Items.Add(New System.Web.UI.WebControls.ListItem(txt, val))
        Next
    End Sub

    Private Sub LoadQuestions()
        Dim sql = "SELECT q.QuestionID, q.QuestionStatement, q.DifficultyLevel, q.CorrectOption, s.SubjectName " & _
                  "FROM QuestionsTable q " & _
                  "JOIN Subjects s ON q.SubjectID = s.SubjectID " & _
                  "WHERE q.CreatedBy = @t"
                  
        Dim pList As New System.Collections.Generic.List(Of System.Data.SqlClient.SqlParameter)
        pList.Add(DBHelper.Param("@t", CInt(Session("UserID"))))

        If ddlFilter.SelectedValue <> "" Then
            sql &= " AND q.SubjectID = @s"
            pList.Add(DBHelper.Param("@s", CInt(ddlFilter.SelectedValue)))
        End If

        If ddlDiffFilter.SelectedValue <> "" Then
            sql &= " AND q.DifficultyLevel = @d"
            pList.Add(DBHelper.Param("@d", ddlDiffFilter.SelectedValue))
        End If

        sql &= " ORDER BY q.CreatedAt DESC"

        gvQuestions.DataSource = DBHelper.GetDataTable(sql, pList.ToArray())
        gvQuestions.DataBind()
    End Sub

    Protected Sub ddlFilter_Changed(sender As Object, e As EventArgs)
        LoadQuestions()
    End Sub

    Protected Sub gvQuestions_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs)
        If e.CommandName = "DeleteQ" Then
            Try
                Dim qid = CInt(e.CommandArgument)
                DBHelper.ExecuteNonQuery("DELETE FROM QuestionsTable WHERE QuestionID=@q AND CreatedBy=@t", _
                    DBHelper.Param("@q", qid), DBHelper.Param("@t", CInt(Session("UserID"))))
                ShowMsg("Question deleted.", True)
                LoadQuestions()
            Catch ex As Exception
                ShowMsg("Error deleting question (it might be in use in a quiz).", False)
            End Try
        End If
    End Sub

    Protected Sub gvQuestions_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs)
        gvQuestions.EditIndex = e.NewEditIndex
        LoadQuestions()
    End Sub

    Protected Sub gvQuestions_CancelEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs)
        gvQuestions.EditIndex = -1
        LoadQuestions()
    End Sub

    Protected Sub gvQuestions_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs)
        Dim qid = CInt(gvQuestions.DataKeys(e.RowIndex).Value)
        Dim row = gvQuestions.Rows(e.RowIndex)
        Dim txtStmt = CType(row.FindControl("txtEditStmt"), System.Web.UI.WebControls.TextBox)
        Dim ddlDiff = CType(row.FindControl("ddlEditDiff"), System.Web.UI.WebControls.DropDownList)

        If txtStmt IsNot Nothing AndAlso ddlDiff IsNot Nothing Then
            DBHelper.ExecuteNonQuery("UPDATE QuestionsTable SET QuestionStatement=@s, DifficultyLevel=@d WHERE QuestionID=@q AND CreatedBy=@t", _
                DBHelper.Param("@s", txtStmt.Text.Trim()), _
                DBHelper.Param("@d", ddlDiff.SelectedValue), _
                DBHelper.Param("@q", qid), _
                DBHelper.Param("@t", CInt(Session("UserID"))))
            ShowMsg("Question updated successfully.", True)
        End If

        gvQuestions.EditIndex = -1
        LoadQuestions()
    End Sub

    Private Sub ShowMsg(msg As String, success As Boolean)
        pnlMsg.Visible = True
        pnlMsg.CssClass = If(success, "alert alert-success mb-4", "alert alert-danger mb-4")
        litMsg.Text = msg
    End Sub

End Class
