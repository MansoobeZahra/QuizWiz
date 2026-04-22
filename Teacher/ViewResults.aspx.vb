Imports System.Data

Partial Class Teacher_ViewResults
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Teacher", "Admin") ' Allow teachers to view results
        If Not IsPostBack Then LoadQuizList()
    End Sub

    Private Sub LoadQuizList()
        Dim role = Session("Role")?.ToString()
        Dim dt As DataTable
        
        If role = "Admin" Then
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
        Dim resDt = DBHelper.GetDataTable("
            SELECT r.ObtainedMarks, r.TotalMarks, r.Percentage, r.AttemptDate,
                   u.FullName AS StudentName
            FROM Results r
            JOIN Users u ON r.StudentID = u.UserID
            WHERE r.QuizID = @qid
            ORDER BY r.Percentage DESC",
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

            If pct >= 90  Then grades(0) += 1
            ElseIf pct >= 80 Then grades(1) += 1
            ElseIf pct >= 70 Then grades(2) += 1
            ElseIf pct >= 60 Then grades(3) += 1
            Else grades(4) += 1
            End If

            labelsJson &= $"'{sName.Replace("'", "\'")}',"
            scoresJson &= $"{pct.ToString("0.##")},"
        Next

        labelsJson = labelsJson.TrimEnd(","c)
        scoresJson = scoresJson.TrimEnd(","c)

        Dim avg = sumPct / totalStudents
        litTotalStudents.Text = totalStudents.ToString()
        litAvgScore.Text      = avg.ToString("0.##") & "%"
        litHighest.Text       = highest.ToString("0.##") & "%"
        litLowest.Text        = lowest.ToString("0.##") & "%"

        ' Overall Correct vs Incorrect
        Dim ansDt = DBHelper.GetDataTable("
            SELECT 
                SUM(CASE WHEN Marks > 0 THEN 1 ELSE 0 END) AS CorrectCount,
                SUM(CASE WHEN Marks <= 0 THEN 1 ELSE 0 END) AS WrongCount
            FROM Answers 
            WHERE QuizID = @qid", DBHelper.Param("@qid", quizID))

        Dim totalCorrect = 0
        Dim totalWrong = 0
        If ansDt.Rows.Count > 0 Then
            totalCorrect = CInt(If(ansDt.Rows(0)("CorrectCount") Is DBNull.Value, 0, ansDt.Rows(0)("CorrectCount")))
            totalWrong = CInt(If(ansDt.Rows(0)("WrongCount") Is DBNull.Value, 0, ansDt.Rows(0)("WrongCount")))
        End If

        ' Build JS Charts
        litChartJS.Text = $"<script>
(function() {{
    const ctxBar = document.getElementById('scoreBarChart').getContext('2d');
    new Chart(ctxBar, {{
        type: 'bar',
        data: {{
            labels: [{labelsJson}],
            datasets: [{{
                label: 'Student Score (%)',
                data: [{scoresJson}],
                backgroundColor: 'rgba(164,113,248,0.8)',
                borderColor: 'rgba(164,113,248,1)',
                borderWidth: 1,
                borderRadius: 4
            }}]
        }},
        options: {{
            responsive: true,
            maintainAspectRatio: false,
            plugins: {{ legend: {{ display: false }} }},
            scales: {{
                y: {{ beginAtZero: true, max: 100, ticks: {{ color: '#9090b8' }}, grid: {{ color: 'rgba(255,255,255,0.05)' }} }},
                x: {{ ticks: {{ color: '#9090b8' }}, grid: {{ display: false }} }}
            }}
        }}
    }});

    const ctxGrade = document.getElementById('gradePieChart').getContext('2d');
    new Chart(ctxGrade, {{
        type: 'pie',
        data: {{
            labels: ['A+ (90-100)','A (80-89)','B (70-79)','C (60-69)','F (<60)'],
            datasets: [{{
                data: [{grades(0)}, {grades(1)}, {grades(2)}, {grades(3)}, {grades(4)}],
                backgroundColor: ['#56ab2f','#8dc26f','#ffc107','#fd7e14','#ff416c'],
                borderColor: '#0a0e27', borderWidth: 2
            }}]
        }},
        options: {{
            responsive: true, maintainAspectRatio: false,
            plugins: {{ legend: {{ position: 'bottom', labels: {{ color: '#9090b8' }} }} }}
        }}
    }});

    const ctxCorrect = document.getElementById('correctPieChart').getContext('2d');
    new Chart(ctxCorrect, {{
        type: 'pie',
        data: {{
            labels: ['Correct Answers', 'Incorrect / Skipped'],
            datasets: [{{
                data: [{totalCorrect}, {totalWrong}],
                backgroundColor: ['#56ab2f', '#ff416c'],
                borderColor: '#0a0e27', borderWidth: 2
            }}]
        }},
        options: {{
            responsive: true, maintainAspectRatio: false,
            plugins: {{ legend: {{ position: 'bottom', labels: {{ color: '#9090b8' }} }} }}
        }}
    }});
}})();
</script>"
    End Sub

End Class
