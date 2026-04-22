Partial Class Student_MyResults
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Role")?.ToString() <> "Student" Then Response.Redirect("~/Login.aspx") : Return
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
        Dim dt = DBHelper.GetDataTable("
            SELECT r.ObtainedMarks, r.TotalMarks, r.Percentage, q.QuizTitle
            FROM Results r
            JOIN Quiz q ON r.QuizID = q.QuizID
            WHERE r.StudentID=@s AND r.QuizID=@q",
            DBHelper.Param("@s", sid), DBHelper.Param("@q", quizID))

        If dt.Rows.Count = 0 Then Return

        Dim row     = dt.Rows(0)
        Dim pct     = Convert.ToDouble(row("Percentage"))
        Dim obtained = Convert.ToDecimal(row("ObtainedMarks"))
        Dim total    = CInt(row("TotalMarks"))

        Dim grade As String, gradeCls As String
        If pct >= 90  Then grade = "A+" : gradeCls = "grade-A"
        ElseIf pct >= 80 Then grade = "A"  : gradeCls = "grade-A"
        ElseIf pct >= 70 Then grade = "B"  : gradeCls = "grade-B"
        ElseIf pct >= 60 Then grade = "C"  : gradeCls = "grade-C"
        Else grade = "F" : gradeCls = "grade-F"
        End If

        litPct.Text      = pct.ToString("0.#") & "%"
        litObtained.Text = obtained.ToString("0.#")
        litTotal.Text    = total.ToString()
        litGrade.Text    = grade
        litGradeCls.Text = gradeCls
        litQuizName.Text = row("QuizTitle").ToString()
        pnlHero.Visible  = True

        ' Per-question detail
        Dim detailDt = DBHelper.GetDataTable("
            SELECT a.QNo, qt.QuestionStatement, qt.DifficultyLevel,
                   a.CorrectAns, a.StudentAns, a.Marks
            FROM Answers a
            JOIN QuestionsTable qt ON a.QuestionID = qt.QuestionID
            WHERE a.StudentID=@s AND a.QuizID=@q
            ORDER BY a.QNo",
            DBHelper.Param("@s", sid), DBHelper.Param("@q", quizID))

        gvDetail.DataSource = detailDt
        gvDetail.DataBind()

        ' Build chart data
        Dim correct   As Integer = CInt(obtained)
        Dim wrong     As Integer = 0
        Dim skipped   As Integer = 0
        Dim easyC=0, medC=0, hardC=0, expC=0
        Dim easyW=0, medW=0, hardW=0, expW=0

        For Each r As System.Data.DataRow In detailDt.Rows
            Dim sa  = If(r("StudentAns") Is DBNull.Value, "", r("StudentAns").ToString())
            Dim ca  = r("CorrectAns").ToString()
            Dim diff = r("DifficultyLevel").ToString()
            Dim isRight = (sa <> "" AndAlso sa = ca)
            Dim isSkip  = (sa = "")

            If isSkip Then
                skipped += 1
            ElseIf Not isRight Then
                wrong += 1
            End If

            Select Case diff
                Case "Easy"
                    If isRight Then easyC += 1 Else easyW += 1
                Case "Medium"
                    If isRight Then medC += 1 Else medW += 1
                Case "Hard"
                    If isRight Then hardC += 1 Else hardW += 1
                Case "Expert"
                    If isRight Then expC += 1 Else expW += 1
            End Select
        Next

        litChartScript.Text = $"<script>
(function(){{
  var pieCtx = document.getElementById('myPieChart').getContext('2d');
  new Chart(pieCtx,{{
    type:'pie',
    data:{{
      labels:['Correct','Wrong','Skipped'],
      datasets:[{{
        data:[{correct},{wrong},{skipped}],
        backgroundColor:['#56ab2f','#ff416c','#f7971e'],
        borderColor:'#0a0e27', borderWidth:2
      }}]
    }},
    options:{{
      responsive:true, maintainAspectRatio:false,
      plugins:{{
        legend:{{position:'bottom',labels:{{color:'#9090b8',padding:12,font:{{size:11}}}}}},
        tooltip:{{callbacks:{{label:function(c){{return c.label+': '+c.parsed+' question(s)'}}}}}}
      }}
    }}
  }});

  var diffCtx = document.getElementById('diffChart').getContext('2d');
  new Chart(diffCtx,{{
    type:'bar',
    data:{{
      labels:['Easy','Medium','Hard','Expert'],
      datasets:[
        {{label:'Correct',data:[{easyC},{medC},{hardC},{expC}],backgroundColor:'rgba(86,171,47,0.7)',borderRadius:4}},
        {{label:'Wrong',  data:[{easyW},{medW},{hardW},{expW}],backgroundColor:'rgba(255,65,108,0.7)',borderRadius:4}}
      ]
    }},
    options:{{
      responsive:true, maintainAspectRatio:false,
      plugins:{{ legend:{{labels:{{color:'#9090b8'}}}} }},
      scales:{{
        x:{{ticks:{{color:'#9090b8'}},grid:{{display:false}},stacked:false}},
        y:{{ticks:{{color:'#9090b8',stepSize:1}},grid:{{color:'rgba(255,255,255,0.06)'}},min:0}}
      }}
    }}
  }});
}})();
</script>"
    End Sub

    Private Sub LoadAllResults(sid As Integer)
        Dim dt = DBHelper.GetDataTable("
            SELECT r.ResultID, q.QuizTitle, s.SubjectName,
                   r.ObtainedMarks, r.TotalMarks, r.Percentage, r.AttemptDate
            FROM Results r
            JOIN Quiz q     ON r.QuizID    = q.QuizID
            JOIN Subjects s ON q.SubjectID = s.SubjectID
            WHERE r.StudentID=@sid
            ORDER BY r.AttemptDate DESC",
            DBHelper.Param("@sid", sid))
        gvAllResults.DataSource = dt
        gvAllResults.DataBind()
    End Sub

End Class
