Partial Class Student_Dashboard
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Student")
        If Not IsPostBack Then
            LoadStats()
            LoadQuizzes()
            LoadNotifications()
        End If
    End Sub

    Private Sub LoadStats()
        Dim uid = CInt(Session("UserID"))
        litTotalAttempts.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM Results WHERE StudentID=@s", DBHelper.Param("@s", uid)).ToString()
            
        Dim avg = DBHelper.ExecuteScalar(
            "SELECT AVG(Percentage) FROM Results WHERE StudentID=@s", DBHelper.Param("@s", uid))
        litAvgScore.Text = If(avg Is DBNull.Value, "0", Convert.ToDouble(avg).ToString("0.##")) & "%"
        
        Dim highest = DBHelper.ExecuteScalar(
            "SELECT MAX(Percentage) FROM Results WHERE StudentID=@s", DBHelper.Param("@s", uid))
        litHighestScore.Text = If(highest Is DBNull.Value, "0", Convert.ToDouble(highest).ToString("0.##")) & "%"
    End Sub

    Private Sub LoadNotifications()
        Dim dt = DBHelper.GetDataTable( _
            "SELECT TOP 5 Message, CreatedAt FROM Notifications " & _
            "WHERE ToUserID=@u ORDER BY CreatedAt DESC",
            DBHelper.Param("@u", CInt(Session("UserID"))))
            
        If dt.Rows.Count = 0 Then
            pnlNoNotif.Visible = True
        Else
            rptNotif.DataSource = dt
            rptNotif.DataBind()
        End If
    End Sub

    Private Sub LoadQuizzes()
        Dim dt = DBHelper.GetDataTable( _
            "SELECT q.QuizID, q.QuizTitle, q.AllowedTime, q.TotalQuestions, s.SubjectName " & _
            "FROM Quiz q " & _
            "JOIN Subjects s ON q.SubjectID = s.SubjectID " & _
            "WHERE q.IsPublished=1 AND q.QuizID NOT IN (SELECT QuizID FROM Results WHERE StudentID=@s) " & _
            "ORDER BY q.CreatedAt DESC",
            DBHelper.Param("@s", CInt(Session("UserID"))))
            
        If dt.Rows.Count = 0 Then
            pnlNoQuizzes.Visible = True
        Else
            rptQuizzes.DataSource = dt
            rptQuizzes.DataBind()
        End If
    End Sub

End Class
