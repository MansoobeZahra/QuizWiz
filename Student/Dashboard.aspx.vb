Partial Class Student_Dashboard
    Inherits System.Web.UI.Page

    Private ReadOnly Property StudentID As Integer
        Get
            Return CInt(Session("UserID"))
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Role")?.ToString() <> "Student" Then Response.Redirect("~/Login.aspx") : Return
        If Not IsPostBack Then LoadDashboard()
    End Sub

    Private Sub LoadDashboard()
        Dim sid = StudentID

        ' Stats
        Dim available As Integer = CInt(DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM Quiz WHERE IsPublished=1", Nothing))
        Dim attempted As Integer = CInt(DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM Results WHERE StudentID=@sid", DBHelper.Param("@sid", sid)))
        litAvailable.Text = available.ToString()
        litAttempted.Text = attempted.ToString()

        If attempted > 0 Then
            Dim avg = DBHelper.ExecuteScalar(
                "SELECT AVG(Percentage) FROM Results WHERE StudentID=@sid", DBHelper.Param("@sid", sid))
            litAvgScore.Text = If(avg Is Nothing OrElse avg Is DBNull.Value, "—", CDbl(avg).ToString("0.#") & "%")
        End If

        ' Notifications
        Dim notifs = DBHelper.GetDataTable(
            "SELECT NotifID, Message, IsRead, CreatedAt FROM Notifications WHERE ToUserID=@sid ORDER BY CreatedAt DESC",
            DBHelper.Param("@sid", sid))
        Dim unread As Integer = 0
        For Each row As System.Data.DataRow In notifs.Rows
            If Not CBool(row("IsRead")) Then unread += 1
        Next
        litNotifs.Text    = unread.ToString()
        pnlNotifs.Visible = (notifs.Rows.Count > 0)
        rptNotifs.DataSource = notifs
        rptNotifs.DataBind()

        ' Available quizzes with attempt status
        Dim quizzes = DBHelper.GetDataTable("
            SELECT q.QuizID, q.QuizTitle, s.SubjectName, q.TotalQuestions,
                   q.AllowedTime, q.Remarks,
                   CAST(CASE WHEN EXISTS(
                       SELECT 1 FROM Results r WHERE r.QuizID=q.QuizID AND r.StudentID=@sid
                   ) THEN 1 ELSE 0 END AS BIT) AS AlreadyAttempted
            FROM Quiz q
            JOIN Subjects s ON q.SubjectID = s.SubjectID
            WHERE q.IsPublished = 1
            ORDER BY q.CreatedAt DESC",
            DBHelper.Param("@sid", sid))

        pnlNoQuiz.Visible    = (quizzes.Rows.Count = 0)
        gvQuizzes.DataSource = quizzes
        gvQuizzes.DataBind()
    End Sub

    Protected Sub btnMarkRead_Click(sender As Object, e As EventArgs)
        DBHelper.ExecuteNonQuery(
            "UPDATE Notifications SET IsRead=1 WHERE ToUserID=@sid",
            DBHelper.Param("@sid", StudentID))
        LoadDashboard()
    End Sub

End Class
