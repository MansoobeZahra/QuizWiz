Partial Class Teacher_ManageQuestions
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Role")?.ToString() <> "Teacher" Then Response.Redirect("~/Login.aspx") : Return
        If Not IsPostBack Then
            LoadSubjectFilter()
            LoadQuestions()
        End If
    End Sub

    Private Sub LoadSubjectFilter()
        Dim dt = DBHelper.GetDataTable("SELECT SubjectID, SubjectName FROM Subjects ORDER BY SubjectName")
        ddlFilter.Items.Clear()
        ddlFilter.Items.Add(New System.Web.UI.WebControls.ListItem("All Subjects", ""))
        For Each row As System.Data.DataRow In dt.Rows
            ddlFilter.Items.Add(New System.Web.UI.WebControls.ListItem(row("SubjectName").ToString(), row("SubjectID").ToString()))
        Next
    End Sub

    Private Sub LoadQuestions()
        Dim sid  = ddlFilter.SelectedValue
        Dim diff = ddlDiffFilter.SelectedValue
        Dim tid  = CInt(Session("UserID"))

        Dim where As String = " WHERE q.CreatedBy = @tid"
        If sid <> ""  Then where &= " AND q.SubjectID = @sid"
        If diff <> "" Then where &= " AND q.DifficultyLevel = @diff"

        Dim sql As String = "
            SELECT q.QuestionID, q.QuestionStatement, s.SubjectName,
                   q.DifficultyLevel, q.CorrectOption, q.CreatedAt
            FROM QuestionsTable q
            JOIN Subjects s ON q.SubjectID = s.SubjectID" &
            where & " ORDER BY q.CreatedAt DESC"

        Dim params As New List(Of System.Data.SqlClient.SqlParameter)
        params.Add(DBHelper.Param("@tid", tid))
        If sid  <> "" Then params.Add(DBHelper.Param("@sid",  CInt(sid)))
        If diff <> "" Then params.Add(DBHelper.Param("@diff", diff))

        gvQuestions.DataSource = DBHelper.GetDataTable(sql, params.ToArray())
        gvQuestions.DataBind()
    End Sub

    Protected Sub ddlFilter_Changed(sender As Object, e As EventArgs)
        gvQuestions.EditIndex = -1
        LoadQuestions()
    End Sub

    Protected Sub gvQuestions_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs)
        If e.CommandName = "DeleteQ" Then
            Dim qid = CInt(e.CommandArgument)
            ' Remove from any quiz first
            DBHelper.ExecuteNonQuery("DELETE FROM QuizQuestions WHERE QuestionID=@id", DBHelper.Param("@id", qid))
            DBHelper.ExecuteNonQuery("DELETE FROM QuestionsTable WHERE QuestionID=@id AND CreatedBy=@tid",
                DBHelper.Param("@id", qid), DBHelper.Param("@tid", CInt(Session("UserID"))))
            pnlMsg.Visible = True
            litMsg.Text    = "Question deleted successfully."
            LoadQuestions()
        End If
    End Sub

    Protected Sub gvQuestions_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs)
        gvQuestions.EditIndex = e.NewEditIndex
        LoadQuestions()
        ' Pre-select difficulty
        Dim row = gvQuestions.Rows(e.NewEditIndex)
        Dim ddl = CType(row.FindControl("ddlEditDiff"), System.Web.UI.WebControls.DropDownList)
        Dim qid = CInt(gvQuestions.DataKeys(e.NewEditIndex).Value)
        Dim diff = DBHelper.ExecuteScalar("SELECT DifficultyLevel FROM QuestionsTable WHERE QuestionID=@id", DBHelper.Param("@id", qid)).ToString()
        If ddl IsNot Nothing Then
            Dim item = ddl.Items.FindByValue(diff)
            If item IsNot Nothing Then item.Selected = True
        End If
    End Sub

    Protected Sub gvQuestions_CancelEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs)
        gvQuestions.EditIndex = -1
        LoadQuestions()
    End Sub

    Protected Sub gvQuestions_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs)
        Dim row  = gvQuestions.Rows(e.RowIndex)
        Dim qid  = CInt(gvQuestions.DataKeys(e.RowIndex).Value)
        Dim stmt = CType(row.FindControl("txtEditStmt"), System.Web.UI.WebControls.TextBox).Text.Trim()
        Dim ddl  = CType(row.FindControl("ddlEditDiff"), System.Web.UI.WebControls.DropDownList)
        Dim diff = If(ddl IsNot Nothing, ddl.SelectedValue, "Medium")

        DBHelper.ExecuteNonQuery(
            "UPDATE QuestionsTable SET QuestionStatement=@s, DifficultyLevel=@d WHERE QuestionID=@id AND CreatedBy=@tid",
            DBHelper.Param("@s",   stmt),
            DBHelper.Param("@d",   diff),
            DBHelper.Param("@id",  qid),
            DBHelper.Param("@tid", CInt(Session("UserID"))))

        gvQuestions.EditIndex = -1
        pnlMsg.Visible = True
        litMsg.Text    = "Question updated successfully."
        LoadQuestions()
    End Sub

End Class
