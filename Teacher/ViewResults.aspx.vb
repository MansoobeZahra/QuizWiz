Partial Class Teacher_ViewResults
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Role")?.ToString() <> "Teacher" Then Response.Redirect("~/Login.aspx") : Return
        If Not IsPostBack Then
            LoadQuizList()
            ' Pre-select from querystring
            Dim qid = Request.QueryString("quizid")
            If Not String.IsNullOrEmpty(qid) Then
                Dim item = ddlQuiz.Items.FindByValue(qid)
                If item IsNot Nothing Then
                    item.Selected = True
                    LoadResults(CInt(qid))
                End If
            End If
        End If
    End Sub

    Private Sub LoadQuizList()
        Dim dt = DBHelper.GetDataTable(
            "SELECT QuizID, QuizTitle FROM Quiz WHERE CreatedBy=@tid ORDER BY CreatedAt DESC",
            DBHelper.Param("@tid", CInt(Session("UserID"))))
        ddlQuiz.Items.Clear()
        ddlQuiz.Items.Add(New System.Web.UI.WebControls.ListItem("-- Select a Quiz --", ""))
        For Each row As System.Data.DataRow In dt.Rows
            ddlQuiz.Items.Add(New System.Web.UI.WebControls.ListItem(row("QuizTitle").ToString(), row("QuizID").ToString()))
        Next
    End Sub

    Protected Sub ddlQuiz_Changed(sender As Object, e As EventArgs)
        If ddlQuiz.SelectedValue = "" Then
            pnlResults.Visible   = False
            pnlSelectMsg.Visible = True
            Return
        End If
        LoadResults(CInt(ddlQuiz.SelectedValue))
    End Sub

    Private Sub LoadResults(quizID As Integer)
        Dim dt = DBHelper.GetDataTable("
            SELECT u.FullName, r.ObtainedMarks, r.TotalMarks, r.Percentage, r.AttemptDate
            FROM Results r
            JOIN Users u ON r.StudentID = u.UserID
            WHERE r.QuizID = @qid
            ORDER BY r.Percentage DESC",
            DBHelper.Param("@qid", quizID))

        If dt.Rows.Count = 0 Then
            pnlResults.Visible   = False
            pnlSelectMsg.Visible = True
            litChartScript.Text  = ""
            Return
        End If

        pnlResults.Visible   = True
        pnlSelectMsg.Visible = False

        ' Stats
        Dim total As Integer = dt.Rows.Count
        Dim avg   As Double  = 0
        Dim maxP  As Double  = 0
        Dim minP  As Double  = 100

        For Each row As System.Data.DataRow In dt.Rows
            Dim p = Convert.ToDouble(row("Percentage"))
            avg  += p
            If p > maxP Then maxP = p
            If p < minP Then minP = p
        Next
        avg = Math.Round(avg / total, 1)

        litTotal.Text = total.ToString()
        litAvg.Text   = avg.ToString("0.#")
        litMax.Text   = maxP.ToString("0.#")
        litMin.Text   = minP.ToString("0.#")

        gvResults.DataSource = dt
        gvResults.DataBind()

        ' Build chart data: bar = per-student scores, pie = grade buckets
        Dim labels   As New System.Text.StringBuilder()
        Dim scores   As New System.Text.StringBuilder()
        Dim gradeAP As Integer = 0, gradeA As Integer = 0, gradeB As Integer = 0
        Dim gradeC As Integer = 0, gradeF As Integer = 0

        For Each row As System.Data.DataRow In dt.Rows
            Dim name  = row("FullName").ToString().Split(" "c)(0)   ' first name
            Dim score = Convert.ToDouble(row("Percentage"))
            If labels.Length > 0 Then labels.Append(",")
            labels.Append($"""{name}""")
            If scores.Length > 0 Then scores.Append(",")
            scores.Append(score.ToString("0.#"))

            If score >= 90 Then gradeAP += 1
            ElseIf score >= 80 Then gradeA += 1
            ElseIf score >= 70 Then gradeB += 1
            ElseIf score >= 60 Then gradeC += 1
            Else gradeF += 1
            End If
        Next

        litChartScript.Text = $"<script>
(function(){{
  const barCtx = document.getElementById('barChart').getContext('2d');
  new Chart(barCtx, {{
    type:'bar',
    data:{{
      labels:[{labels}],
      datasets:[{{
        label:'Score %',
        data:[{scores}],
        backgroundColor:'rgba(108,99,255,0.6)',
        borderColor:'rgba(108,99,255,1)',
        borderWidth:1,
        borderRadius:6
      }}]
    }},
    options:{{
      responsive:true, maintainAspectRatio:false,
      plugins:{{ legend:{{display:false}}, tooltip:{{callbacks:{{label:function(c){{return c.parsed.y+'%'}}}}}} }},
      scales:{{
        y:{{ min:0, max:100, ticks:{{color:'#9090b8',callback:function(v){{return v+'%'}}}}, grid:{{color:'rgba(255,255,255,0.06)'}} }},
        x:{{ ticks:{{color:'#9090b8'}}, grid:{{display:false}} }}
      }}
    }}
  }});

  const pieCtx = document.getElementById('pieChart').getContext('2d');
  new Chart(pieCtx, {{
    type:'pie',
    data:{{
      labels:['A+ (≥90%)','A (80-89%)','B (70-79%)','C (60-69%)','F (<60%)'],
      datasets:[{{
        data:[{gradeAP},{gradeA},{gradeB},{gradeC},{gradeF}],
        backgroundColor:['#56ab2f','#a8e063','#5ee7df','#f7971e','#ff416c'],
        borderColor:'#0a0e27',
        borderWidth:2
      }}]
    }},
    options:{{
      responsive:true, maintainAspectRatio:false,
      plugins:{{
        legend:{{ position:'bottom', labels:{{color:'#9090b8',padding:12,font:{{size:11}}}} }},
        tooltip:{{callbacks:{{label:function(c){{return c.label+': '+c.parsed+' student(s)'}}}}}}
      }}
    }}
  }});
}})();
</script>"
    End Sub

End Class
