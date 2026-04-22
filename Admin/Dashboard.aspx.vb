Partial Class Admin_Dashboard
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Role")?.ToString() <> "Admin" Then Response.Redirect("~/Login.aspx") : Return
        If Not IsPostBack Then LoadStats()
    End Sub

    Private Sub LoadStats()
        litUsers.Text     = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Users").ToString()
        litTeachers.Text  = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Users WHERE Role='Teacher'").ToString()
        litStudents.Text  = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Users WHERE Role='Student'").ToString()
        litQuizzes.Text   = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Quiz").ToString()
        litQuestions.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM QuestionsTable").ToString()
        litAttempts.Text  = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Results").ToString()

        ' Recent quizzes
        gvQuizzes.DataSource = DBHelper.GetDataTable("
            SELECT TOP 8 q.QuizTitle, u.FullName AS CreatorName, q.IsPublished,
                   (SELECT COUNT(*) FROM Results r WHERE r.QuizID=q.QuizID) AS Attempts
            FROM Quiz q JOIN Users u ON q.CreatedBy=u.UserID
            ORDER BY q.CreatedAt DESC")
        gvQuizzes.DataBind()

        ' Recent results
        gvResults.DataSource = DBHelper.GetDataTable("
            SELECT TOP 8 u.FullName AS StudentName, q.QuizTitle, r.Percentage, r.AttemptDate
            FROM Results r
            JOIN Users u ON r.StudentID=u.UserID
            JOIN Quiz q  ON r.QuizID=q.QuizID
            ORDER BY r.AttemptDate DESC")
        gvResults.DataBind()
    End Sub

End Class
