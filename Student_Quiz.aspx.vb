Imports System.Data

Partial Class Student_AttemptQuiz
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Student")
        If Not IsPostBack Then
            Dim qid = Request.QueryString("quizid")
            If qid = "" Then Response.Redirect("Student_Dashboard.aspx") : Return
            StartQuiz(CInt(qid))
        End If
    End Sub

    Private Sub StartQuiz(qid As Integer)
        Dim dt = DBHelper.GetDataTable("SELECT QuizTitle, AllowedTime, TotalQuestions FROM Quiz WHERE QuizID=@q", DBHelper.Param("@q", qid))
        If dt.Rows.Count = 0 Then Response.Redirect("Student_Dashboard.aspx") : Return
        Session("QuizID") = qid
        Session("QuizTitle") = dt.Rows(0)("QuizTitle").ToString()
        Session("TimeLeft") = CInt(dt.Rows(0)("AllowedTime")) * 60
        Session("QIndex") = 0
        
        Dim qDt = DBHelper.GetDataTable("SELECT TOP " & dt.Rows(0)("TotalQuestions").ToString() & " q.QuestionID, q.QuestionStatement, q.OptionA, q.OptionB, q.CorrectOptions, q.QuestionType FROM QuizQuestions qq JOIN QuestionsTable q ON qq.QuestionID = q.QuestionID WHERE qq.QuizID=@q", DBHelper.Param("@q", qid))
        Session("Questions") = qDt
        Session("QTotal") = qDt.Rows.Count
        
        pnlQuiz.Visible = True
        ShowQ()
    End Sub

    Private Sub ShowQ()
        Dim idx = CInt(Session("QIndex"))
        Dim dt = CType(Session("Questions"), DataTable)
        If idx >= dt.Rows.Count Then Finish() : Return

        Dim row = dt.Rows(idx)
        litQuizTitle.Text = Session("QuizTitle").ToString()
        litQNum.Text = (idx + 1).ToString()
        litQTotal.Text = Session("QTotal").ToString()
        litQuestion.Text = row("QuestionStatement").ToString()
        hfTimeLeft.Value = Session("TimeLeft").ToString()

        rblOptions.Items.Clear()
        rblOptions.Items.Add(New ListItem(row("OptionA").ToString(), "A"))
        rblOptions.Items.Add(New ListItem(row("OptionB").ToString(), "B"))
        
        btnNext.Visible = (idx < dt.Rows.Count - 1)
        btnSubmit.Visible = (idx = dt.Rows.Count - 1)
    End Sub

    Protected Sub btnNext_Click(sender As Object, e As EventArgs)
        SaveAns()
        Session("QIndex") = CInt(Session("QIndex")) + 1
        ShowQ()
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        SaveAns()
        Finish()
    End Sub

    Private Sub SaveAns()
        Dim idx = CInt(Session("QIndex"))
        Dim dt = CType(Session("Questions"), DataTable)
        Dim row = dt.Rows(idx)
        Dim ans = rblOptions.SelectedValue
        Dim marks = If(ans = row("CorrectOptions").ToString(), 1, 0)
        
        DBHelper.ExecuteNonQuery("INSERT INTO Answers (StudentID,QuizID,QuestionID,QNo,CorrectAns,StudentAns,Marks) VALUES (@s,@qz,@qn,@no,@ca,@sa,@m)", _
            DBHelper.Param("@s", Session("UserID")), DBHelper.Param("@qz", Session("QuizID")), DBHelper.Param("@qn", row("QuestionID")), _
            DBHelper.Param("@no", idx + 1), DBHelper.Param("@ca", row("CorrectOptions")), DBHelper.Param("@sa", ans), DBHelper.Param("@m", marks))
    End Sub

    Private Sub Finish()
        Dim qid = CInt(Session("QuizID"))
        Dim sid = CInt(Session("UserID"))
        Dim total = CInt(Session("QTotal"))
        Dim obtained = CInt(If(DBHelper.ExecuteScalar("SELECT SUM(Marks) FROM Answers WHERE StudentID=@s AND QuizID=@q", DBHelper.Param("@s", sid), DBHelper.Param("@q", qid)), 0))
        DBHelper.ExecuteNonQuery("INSERT INTO Results (StudentID,QuizID,TotalMarks,ObtainedMarks,Percentage) VALUES (@s,@q,@t,@o,@p)", _
            DBHelper.Param("@s", sid), DBHelper.Param("@q", qid), DBHelper.Param("@t", total), DBHelper.Param("@o", obtained), DBHelper.Param("@p", (obtained / total) * 100))
        Response.Redirect("Student_Results.aspx?done=1&quizid=" & qid)
    End Sub
End Class
