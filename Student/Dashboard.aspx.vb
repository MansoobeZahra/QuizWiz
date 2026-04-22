Partial Class Student_Dashboard
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim roleStr As String = ""
        If Session("Role") IsNot Nothing Then roleStr = Session("Role").ToString()
        If roleStr <> "Student" Then Response.Redirect("~/Login.aspx") : Return

        If Not IsPostBack Then
            LoadStats()
            LoadQuizzes()
            LoadNotifications()
        End If
    End Sub

    Private Sub LoadStats()
        Dim uid = CInt(Session("UserID"))
        
        ' Get Student's SubjectID
        Dim studentSub = DBHelper.ExecuteScalar("SELECT SubjectID FROM Users WHERE UserID=@u", DBHelper.Param("@u", uid))
        Dim subFilter As String = ""
        If studentSub IsNot Nothing AndAlso studentSub IsNot DBNull.Value Then
            subFilter = " AND SubjectID = " & studentSub.ToString()
        End If

        Dim totalQz = CInt(If(DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Quiz WHERE IsPublished=1" & subFilter), 0))
        litAvailable.Text = totalQz.ToString()
            
        Dim attempts = CInt(If(DBHelper.ExecuteScalar( _
            "SELECT COUNT(*) FROM Results WHERE StudentID=@s", DBHelper.Param("@s", uid)), 0))
        litAttempted.Text = attempts.ToString()
            
        Dim avg = DBHelper.ExecuteScalar( _
            "SELECT AVG(Percentage) FROM Results WHERE StudentID=@s", DBHelper.Param("@s", uid))
        litAvgScore.Text = If(avg Is DBNull.Value, "0", Convert.ToDouble(avg).ToString("0.##")) & "%"
        
        Dim notifs = CInt(If(DBHelper.ExecuteScalar( _
            "SELECT COUNT(*) FROM Notifications WHERE ToUserID=@u AND IsRead=0", DBHelper.Param("@u", uid)), 0))
        litNotifs.Text = notifs.ToString()
    End Sub

    Private Sub LoadNotifications()
        Dim dt = DBHelper.GetDataTable( _
            "SELECT TOP 5 Message, CreatedAt, IsRead FROM Notifications " & _
            "WHERE ToUserID=@u ORDER BY CreatedAt DESC", _
            DBHelper.Param("@u", CInt(Session("UserID"))))
            
        If dt.Rows.Count = 0 Then
            pnlNotifs.Visible = False
        Else
            pnlNotifs.Visible = True
            rptNotifs.DataSource = dt
            rptNotifs.DataBind()
        End If
    End Sub
    
    Protected Sub btnMarkRead_Click(sender As Object, e As EventArgs)
        DBHelper.ExecuteNonQuery("UPDATE Notifications SET IsRead=1 WHERE ToUserID=@u", _
            DBHelper.Param("@u", CInt(Session("UserID"))))
        LoadStats()
        LoadNotifications()
    End Sub

    Private Sub LoadQuizzes()
        ' Get Student's SubjectID
        Dim studentSub = DBHelper.ExecuteScalar("SELECT SubjectID FROM Users WHERE UserID=@u", DBHelper.Param("@u", CInt(Session("UserID"))))
        Dim sql = "SELECT q.QuizID, q.QuizTitle, q.AllowedTime, q.TotalQuestions, s.SubjectName, q.Remarks, " & _
                  "       CASE WHEN EXISTS(SELECT 1 FROM Results r WHERE r.StudentID=@s AND r.QuizID=q.QuizID) " & _
                  "       THEN 1 ELSE 0 END AS AlreadyAttempted " & _
                  "FROM Quiz q " & _
                  "JOIN Subjects s ON q.SubjectID = s.SubjectID " & _
                  "WHERE q.IsPublished=1 "
        
        If studentSub IsNot Nothing AndAlso studentSub IsNot DBNull.Value Then
            sql &= " AND q.SubjectID = " & studentSub.ToString()
        End If

        sql &= " ORDER BY q.CreatedAt DESC"

        Dim dt = DBHelper.GetDataTable(sql, DBHelper.Param("@s", CInt(Session("UserID"))))
            
        If dt.Rows.Count = 0 Then
            pnlNoQuiz.Visible = True
            gvQuizzes.Visible = False
        Else
            pnlNoQuiz.Visible = False
            gvQuizzes.Visible = True
            gvQuizzes.DataSource = dt
            gvQuizzes.DataBind()
        End If
    End Sub

End Class
