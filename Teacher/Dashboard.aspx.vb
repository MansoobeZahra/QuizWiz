Partial Class Teacher_Dashboard
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Teacher")
        If Not IsPostBack Then
            LoadStats()
            LoadQuizzes()
        End If
    End Sub

    Private Sub LoadStats()
        Dim uid = CInt(Session("UserID"))
        
        Dim totalQ = CInt(If(DBHelper.ExecuteScalar("SELECT COUNT(*) FROM QuestionsTable WHERE CreatedBy=@u", DBHelper.Param("@u", uid)), 0))
        litTotalQ.Text = totalQ.ToString()
        
        Dim totalQuiz = CInt(If(DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Quiz WHERE CreatedBy=@u", DBHelper.Param("@u", uid)), 0))
        litTotalQuiz.Text = totalQuiz.ToString()
        
        Dim pubQuiz = CInt(If(DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Quiz WHERE CreatedBy=@u AND IsPublished=1", DBHelper.Param("@u", uid)), 0))
        litPublished.Text = pubQuiz.ToString()
        
        Dim attempts = CInt(If(DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Results r JOIN Quiz q ON r.QuizID = q.QuizID WHERE q.CreatedBy=@u", DBHelper.Param("@u", uid)), 0))
        litAttempts.Text = attempts.ToString()
    End Sub

    Private Sub LoadQuizzes()
        Dim dt = DBHelper.GetDataTable( _
            "SELECT q.QuizID, q.QuizTitle, q.AllowedTime, q.TotalQuestions, q.IsPublished, s.SubjectName, " & _
            "       (SELECT COUNT(*) FROM Results r WHERE r.QuizID = q.QuizID) AS AttemptCount " & _
            "FROM Quiz q " & _
            "JOIN Subjects s ON q.SubjectID = s.SubjectID " & _
            "WHERE q.CreatedBy = @u " & _
            "ORDER BY q.CreatedAt DESC", _
            DBHelper.Param("@u", CInt(Session("UserID"))))
            
        gvQuizzes.DataSource = dt
        gvQuizzes.DataBind()
    End Sub

    Protected Sub gvQuizzes_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs)
        If e.CommandName = "PublishToggle" Then
            Dim qid = CInt(e.CommandArgument)
            DBHelper.ExecuteNonQuery( _
                "UPDATE Quiz SET IsPublished = 1 - IsPublished WHERE QuizID=@q AND CreatedBy=@u", _
                DBHelper.Param("@q", qid), DBHelper.Param("@u", CInt(Session("UserID"))))
            
            ' Fetch title and notify if published
            Dim check = DBHelper.GetDataTable("SELECT IsPublished, QuizTitle FROM Quiz WHERE QuizID=@q", DBHelper.Param("@q", qid))
            If check.Rows.Count > 0 AndAlso CBool(check.Rows(0)("IsPublished")) Then
                Dim title = check.Rows(0)("QuizTitle").ToString()
                Dim students = DBHelper.GetDataTable("SELECT UserID FROM Users WHERE Role='Student' AND IsActive=1")
                For Each r As System.Data.DataRow In students.Rows
                    DBHelper.ExecuteNonQuery( _
                        "INSERT INTO Notifications(ToUserID,FromUserID,Message) VALUES(@to,@from,@msg)", _
                        DBHelper.Param("@to", CInt(r("UserID"))), _
                        DBHelper.Param("@from", CInt(Session("UserID"))), _
                        DBHelper.Param("@msg", "New quiz available: """ & title & """. Open your dashboard to attempt it."))
                Next
            End If
            
            LoadStats()
            LoadQuizzes()
        End If
    End Sub

End Class
