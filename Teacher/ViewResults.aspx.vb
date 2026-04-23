Imports System.Data

Partial Class Teacher_ViewResults
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Teacher", "Admin") ' Allow teachers to view results
        If Not IsPostBack Then LoadQuizList()
    End Sub

    Private Sub LoadQuizList()
        Dim roleStr As String = ""
        If Session("Role") IsNot Nothing Then roleStr = Session("Role").ToString()
        
        Dim dt As DataTable
        If roleStr = "Admin" Then
             dt = DBHelper.GetDataTable("SELECT QuizID, QuizTitle FROM Quiz ORDER BY CreatedAt DESC")
        Else
             dt = DBHelper.GetDataTable(
                 "SELECT QuizID, QuizTitle FROM Quiz WHERE CreatedBy=@id ORDER BY CreatedAt DESC",
                 DBHelper.Param("@id", CInt(Session("UserID"))))
        End If

        ddlQuiz.DataSource     = dt
        ddlQuiz.DataTextField  = "QuizTitle"
        ddlQuiz.DataValueField = "QuizID"
        ddlQuiz.DataBind()

        ddlQuiz.Items.Insert(0, New System.Web.UI.WebControls.ListItem("-- Select a Quiz --", ""))
    End Sub

    Protected Sub btnView_Click(sender As Object, e As EventArgs)
        If String.IsNullOrEmpty(ddlQuiz.SelectedValue) Then Return

        Dim quizID = CInt(ddlQuiz.SelectedValue)

        ' Fetch Results
        Dim resDt = DBHelper.GetDataTable( _
            "SELECT r.ObtainedMarks, r.TotalMarks, r.Percentage, r.AttemptDate, " & _
            "       u.FullName AS StudentName " & _
            "FROM Results r " & _
            "JOIN Users u ON r.StudentID = u.UserID " & _
            "WHERE r.QuizID = @qid " & _
            "ORDER BY r.Percentage DESC",
            DBHelper.Param("@qid", quizID))

        If resDt.Rows.Count = 0 Then
            pnlNoData.Visible = True
            pnlResults.Visible = False
            Return
        End If

        pnlNoData.Visible  = False
        pnlResults.Visible = True

        gvResults.DataSource = resDt
        gvResults.DataBind()

        ' Calculate KPIs
        Dim totalStudents As Integer = resDt.Rows.Count
        Dim sumPct        As Double  = 0
        Dim highest       As Double  = -1
        Dim lowest        As Double  = 101

        Dim grades(4) As Integer ' A+, A, B, C, F
        Dim labelsJson As String = ""
        Dim scoresJson As String = ""

        For i As Integer = 0 To totalStudents - 1
            Dim row = resDt.Rows(i)
            Dim pct = Convert.ToDouble(row("Percentage"))
            Dim sName = row("StudentName").ToString()

            sumPct += pct
            If pct > highest Then highest = pct
            If pct < lowest  Then lowest  = pct

            If pct >= 90  Then
                grades(0) += 1
            ElseIf pct >= 80 Then
                grades(1) += 1
            ElseIf pct >= 70 Then
                grades(2) += 1
            ElseIf pct >= 60 Then
                grades(3) += 1
            Else
                grades(4) += 1
            End If

            labelsJson &= "'" & sName.Replace("'", "\'") & "',"
            scoresJson &= pct.ToString("0.##") & ","
        Next

        labelsJson = labelsJson.TrimEnd(","c)
        scoresJson = scoresJson.TrimEnd(","c)

        Dim avg = sumPct / totalStudents
        litTotalStudents.Text = totalStudents.ToString()
        litAvgScore.Text      = avg.ToString("0.##") & "%"
        litHighest.Text       = highest.ToString("0.##") & "%"
        litLowest.Text        = lowest.ToString("0.##") & "%"

        ' Overall Correct vs Incorrect
        Dim ansDt = DBHelper.GetDataTable( _
            "SELECT " & _
            "    SUM(CASE WHEN Marks > 0 THEN 1 ELSE 0 END) AS CorrectCount, " & _
            "    SUM(CASE WHEN Marks <= 0 THEN 1 ELSE 0 END) AS WrongCount " & _
            "FROM Answers " & _
            "WHERE QuizID = @qid", DBHelper.Param("@qid", quizID))

        Dim totalCorrect = 0
        Dim totalWrong = 0
        If ansDt.Rows.Count > 0 Then
            totalCorrect = CInt(If(ansDt.Rows(0)("CorrectCount") Is DBNull.Value, 0, ansDt.Rows(0)("CorrectCount")))
            totalWrong = CInt(If(ansDt.Rows(0)("WrongCount") Is DBNull.Value, 0, ansDt.Rows(0)("WrongCount")))
        End If

        ' Build JS Charts manually (No String Interpolation or Multiline strings)
        Dim sb As New System.Text.StringBuilder()
        sb.AppendLine("<script>")
        sb.AppendLine("(function() {")
        sb.AppendLine("    const ctxBar = document.getElementById('scoreBarChart').getContext('2d');")
        sb.AppendLine("    new Chart(ctxBar, {")
        sb.AppendLine("        type: 'bar',")
        sb.AppendLine("        data: {")
        sb.AppendLine("            labels: [" & labelsJson & "],")
        sb.AppendLine("            datasets: [{")
        sb.AppendLine("                label: 'Student Score (%)',")
        sb.AppendLine("                data: [" & scoresJson & "],")
        sb.AppendLine("                backgroundColor: 'rgba(37, 150, 190, 0.7)',")
        sb.AppendLine("                borderColor: '#2596be',")
        sb.AppendLine("                borderWidth: 1,")
        sb.AppendLine("                borderRadius: 4")
        sb.AppendLine("            }]")
        sb.AppendLine("        },")
        sb.AppendLine("        options: {")
        sb.AppendLine("            responsive: true,")
        sb.AppendLine("            maintainAspectRatio: false,")
        sb.AppendLine("            plugins: { legend: { display: false } },")
        sb.AppendLine("            scales: {")
        sb.AppendLine("                y: { beginAtZero: true, max: 100, ticks: { color: '#666' }, grid: { color: '#eee' } },")
        sb.AppendLine("                x: { ticks: { color: '#666' }, grid: { display: false } }")
        sb.AppendLine("            }")
        sb.AppendLine("        }")
        sb.AppendLine("    });")

        sb.AppendLine("    const ctxGrade = document.getElementById('gradePieChart').getContext('2d');")
        sb.AppendLine("    new Chart(ctxGrade, {")
        sb.AppendLine("        type: 'pie',")
        sb.AppendLine("        data: {")
        sb.AppendLine("            labels: ['A+ (90-100)','A (80-89)','B (70-79)','C (60-69)','F (<60)'],")
        sb.AppendLine("            datasets: [{")
        sb.AppendLine("                data: [" & grades(0) & ", " & grades(1) & ", " & grades(2) & ", " & grades(3) & ", " & grades(4) & "],")
        sb.AppendLine("                backgroundColor: ['#4CAF50','#8BC34A','#FFC107','#FF9800','#F44336'],")
        sb.AppendLine("                borderColor: '#fff', borderWidth: 2")
        sb.AppendLine("            }]")
        sb.AppendLine("        },")
        sb.AppendLine("        options: {")
        sb.AppendLine("            responsive: true, maintainAspectRatio: false,")
        sb.AppendLine("            plugins: { legend: { position: 'bottom', labels: { color: '#666' } } }")
        sb.AppendLine("        }")
        sb.AppendLine("    });")

        sb.AppendLine("    const ctxCorrect = document.getElementById('correctPieChart').getContext('2d');")
        sb.AppendLine("    new Chart(ctxCorrect, {")
        sb.AppendLine("        type: 'pie',")
        sb.AppendLine("        data: {")
        sb.AppendLine("            labels: ['Correct Answers', 'Incorrect / Skipped'],")
        sb.AppendLine("            datasets: [{")
        sb.AppendLine("                data: [" & totalCorrect & ", " & totalWrong & "],")
        sb.AppendLine("                backgroundColor: ['#4CAF50', '#F44336'],")
        sb.AppendLine("                borderColor: '#fff', borderWidth: 2")
        sb.AppendLine("            }]")
        sb.AppendLine("        },")
        sb.AppendLine("        options: {")
        sb.AppendLine("            responsive: true, maintainAspectRatio: false,")
        sb.AppendLine("            plugins: { legend: { position: 'bottom', labels: { color: '#666' } } }")
        sb.AppendLine("        }")
        sb.AppendLine("    });")
        sb.AppendLine("})();")
        sb.AppendLine("</script>")

        litChartJS.Text = sb.ToString()
    End Sub

    Public Function GetGradeHtml(percentageObj As Object) As String
        Dim p As Double = Convert.ToDouble(percentageObj)
        Dim g As String, gc As String
        If p >= 90 Then
            g = "A+" : gc = "grade-A"
        ElseIf p >= 80 Then
            g = "A" : gc = "grade-A"
        ElseIf p >= 70 Then
            g = "B" : gc = "grade-B"
        ElseIf p >= 60 Then
            g = "C" : gc = "grade-C"
        Else
            g = "F" : gc = "grade-F"
        End If
        Return String.Format("<span class=""fw-700 {0}"">{1}</span>", gc, g)
    End Function

End Class
