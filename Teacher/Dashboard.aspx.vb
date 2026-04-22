Partial Class Teacher_Dashboard
    Inherits System.Web.UI.Page

    Private ReadOnly Property TeacherID As Integer
        Get
            Return CInt(Session("UserID"))
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Role")?.ToString() <> "Teacher" Then Response.Redirect("~/Login.aspx") : Return
        If Not IsPostBack Then LoadDashboard()
    End Sub

    Private Sub LoadDashboard()
        Dim tid = TeacherID

        ' Stats
        litTotalQ.Text    = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM QuestionsTable WHERE CreatedBy=@id", DBHelper.Param("@id", tid)).ToString()
        litTotalQuiz.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Quiz WHERE CreatedBy=@id", DBHelper.Param("@id", tid)).ToString()
        litPublished.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Quiz WHERE CreatedBy=@id AND IsPublished=1", DBHelper.Param("@id", tid)).ToString()
        litAttempts.Text  = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM Results r JOIN Quiz q ON r.QuizID=q.QuizID WHERE q.CreatedBy=@id",
            DBHelper.Param("@id", tid)).ToString()

        ' Quiz list
        Dim sql As String = "
            SELECT q.QuizID, q.QuizTitle, s.SubjectName, q.AllowedTime, q.TotalQuestions, q.IsPublished,
                   (SELECT COUNT(*) FROM Results r WHERE r.QuizID = q.QuizID) AS AttemptCount
            FROM Quiz q
            JOIN Subjects s ON q.SubjectID = s.SubjectID
            WHERE q.CreatedBy = @id
            ORDER BY q.CreatedAt DESC"
        gvQuizzes.DataSource = DBHelper.GetDataTable(sql, DBHelper.Param("@id", tid))
        gvQuizzes.DataBind()
    End Sub

    Protected Sub gvQuizzes_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs)
        If e.CommandName = "PublishToggle" Then
            Dim qid = CInt(e.CommandArgument)
            DBHelper.ExecuteNonQuery("UPDATE Quiz SET IsPublished = 1 - IsPublished WHERE QuizID = @id",
                DBHelper.Param("@id", qid))
            ' Send notifications to all students when published
            Dim isNowPublished = CInt(DBHelper.ExecuteScalar("SELECT IsPublished FROM Quiz WHERE QuizID=@id", DBHelper.Param("@id", qid)))
            If isNowPublished = 1 Then
                Dim quizTitle = DBHelper.ExecuteScalar("SELECT QuizTitle FROM Quiz WHERE QuizID=@id", DBHelper.Param("@id", qid)).ToString()
                Dim students  = DBHelper.GetDataTable("SELECT UserID FROM Users WHERE Role='Student' AND IsActive=1")
                For Each row As System.Data.DataRow In students.Rows
                    DBHelper.ExecuteNonQuery(
                        "INSERT INTO Notifications(ToUserID,FromUserID,Message) VALUES(@to,@from,@msg)",
                        DBHelper.Param("@to", CInt(row("UserID"))),
                        DBHelper.Param("@from", TeacherID),
                        DBHelper.Param("@msg", $"New quiz published: '{quizTitle}'. Open your dashboard to attempt it!"))
                Next
            End If
            LoadDashboard()
        End If
    End Sub

End Class
