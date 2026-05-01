Partial Class Student_MyResults
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim roleStr As String = ""
        If Session("Role") IsNot Nothing Then roleStr = Session("Role").ToString()
        If roleStr <> "Student" Then Response.Redirect("~/Login.aspx") : Return

        If Not IsPostBack Then
            Dim sid   = CInt(Session("UserID"))
            Dim qidStr = Request.QueryString("quizid")
            Dim done   = (Request.QueryString("done") = "1")

            pnlJustDone.Visible = done

            If Not String.IsNullOrEmpty(qidStr) Then
                LoadSingleResult(sid, CInt(qidStr))
            End If

            LoadAllResults(sid)
        End If
    End Sub

    Private Sub LoadSingleResult(sid As Integer, quizID As Integer)
        ' Get result row
        Dim dt = DBHelper.GetDataTable( _
            "SELECT r.ObtainedMarks, r.TotalMarks, r.Percentage, q.QuizTitle " & _
            "FROM Results r " & _
            "JOIN Quiz q ON r.QuizID = q.QuizID " & _
            "WHERE r.StudentID=@s AND r.QuizID=@q", _
            DBHelper.Param("@s", sid), DBHelper.Param("@q", quizID))

        If dt.Rows.Count = 0 Then Return

        Dim row     = dt.Rows(0)
        Dim pct     = Convert.ToDouble(row("Percentage"))
        Dim obtained = Convert.ToDecimal(row("ObtainedMarks"))
        Dim total    = CInt(row("TotalMarks"))

        Dim grade As String
        If pct >= 90  Then
            grade = "A+"
        ElseIf pct >= 80 Then
            grade = "A"
        ElseIf pct >= 70 Then
            grade = "B"
        ElseIf pct >= 60 Then
            grade = "C"
        Else
            grade = "F"
        End If

        litPct.Text      = pct.ToString("0.#") & "%"
        litObtained.Text = obtained.ToString("0.#")
        litTotal.Text    = total.ToString()
        litGrade.Text    = grade
        litQuizName.Text = row("QuizTitle").ToString()
        pnlHero.Visible  = True

        ' Per-question detail
        Dim detailDt = DBHelper.GetDataTable( _
            "SELECT a.QNo, qt.QuestionStatement, qt.DifficultyLevel, " & _
            "       a.CorrectAns, a.StudentAns, a.Marks " & _
            "FROM Answers a " & _
            "JOIN QuestionsTable qt ON a.QuestionID = qt.QuestionID " & _
            "WHERE a.StudentID=@s AND a.QuizID=@q " & _
            "ORDER BY a.QNo", _
            DBHelper.Param("@s", sid), DBHelper.Param("@q", quizID))

        gvDetail.DataSource = detailDt
        gvDetail.DataBind()

        ' Chart generation removed for simplicity
        litChartScript.Text = ""
    End Sub

    Public Function GetGradeHtml(percentageObj As Object) As String
        Dim p As Double = Convert.ToDouble(percentageObj)
        Dim g As String, bc As String = "badge"
        If p >= 90 Then
            g = "A+" : bc = "badge-green"
        ElseIf p >= 80 Then
            g = "A" : bc = "badge-green"
        ElseIf p >= 70 Then
            g = "B" : bc = "badge-purple"
        ElseIf p >= 60 Then
            g = "C" : bc = "badge-purple"
        Else
            g = "F" : bc = "badge-red"
        End If
        Return String.Format("<span class=""{0}"">{1}</span>", bc, g)
    End Function

    Public Function GetAnswerHtml(studentAnsObj As Object, correctAnsObj As Object) As String
        Dim sa As String = If(studentAnsObj Is DBNull.Value OrElse studentAnsObj.ToString() = "", "-", studentAnsObj.ToString())
        Dim ca As String = If(correctAnsObj Is DBNull.Value, "", correctAnsObj.ToString())
        
        If sa = "-" Then
            Return "<span class=""badge badge-grey"">- Skipped</span>"
        ElseIf String.Compare(sa.Trim(), ca.Trim(), StringComparison.OrdinalIgnoreCase) = 0 Then
            Return String.Format("<span class=""badge badge-green"">{0}</span>", sa)
        Else
            Return String.Format("<span class=""badge badge-red"">{0}</span>", sa)
        End If
    End Function

    Private Sub LoadAllResults(sid As Integer)
        Dim dt = DBHelper.GetDataTable( _
            "SELECT r.ResultID, q.QuizTitle, s.SubjectName, " & _
            "       r.ObtainedMarks, r.TotalMarks, r.Percentage, r.AttemptDate " & _
            "FROM Results r " & _
            "JOIN Quiz q     ON r.QuizID    = q.QuizID " & _
            "JOIN Subjects s ON q.SubjectID = s.SubjectID " & _
            "WHERE r.StudentID=@sid " & _
            "ORDER BY r.AttemptDate DESC", _
            DBHelper.Param("@sid", sid))
        gvAllResults.DataSource = dt
        gvAllResults.DataBind()
    End Sub

End Class
