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

        Dim grade As String, gradeCls As String
        If pct >= 90  Then
            grade = "A+" : gradeCls = "grade-A"
        ElseIf pct >= 80 Then
            grade = "A"  : gradeCls = "grade-A"
        ElseIf pct >= 70 Then
            grade = "B"  : gradeCls = "grade-B"
        ElseIf pct >= 60 Then
            grade = "C"  : gradeCls = "grade-C"
        Else
            grade = "F" : gradeCls = "grade-F"
        End If

        litPct.Text      = pct.ToString("0.#") & "%"
        litObtained.Text = obtained.ToString("0.#")
        litTotal.Text    = total.ToString()
        litGrade.Text    = grade
        litGradeCls.Text = gradeCls
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

        Dim sb As New System.Text.StringBuilder()
        sb.AppendLine("<script>")
        sb.AppendLine("(function(){")
        sb.AppendLine("  var pieCtx = document.getElementById('myPieChart').getContext('2d');")
        sb.AppendLine("  new Chart(pieCtx,{")
        sb.AppendLine("    type:'pie',")
        sb.AppendLine("    data:{")
        sb.AppendLine("      labels:['Correct','Wrong','Skipped'],")
        sb.AppendLine("      datasets:[{")
        sb.AppendLine("        data:[" & correct & "," & wrong & "," & skipped & "],")
        sb.AppendLine("        backgroundColor:['#4CAF50','#F44336','#FF9800'],")
        sb.AppendLine("        borderColor:'#fff', borderWidth:2")
        sb.AppendLine("      }]")
        sb.AppendLine("    },")
        sb.AppendLine("    options:{")
        sb.AppendLine("      responsive:true, maintainAspectRatio:false,")
        sb.AppendLine("      plugins:{")
        sb.AppendLine("        legend:{position:'bottom',labels:{color:'#666',padding:12,font:{size:11}}},")
        sb.AppendLine("        tooltip:{callbacks:{label:function(c){return c.label+': '+c.parsed+' question(s)'}}}")
        sb.AppendLine("      }")
        sb.AppendLine("    }")
        sb.AppendLine("  });")

        sb.AppendLine("  var diffCtx = document.getElementById('diffChart').getContext('2d');")
        sb.AppendLine("  new Chart(diffCtx,{")
        sb.AppendLine("    type:'bar',")
        sb.AppendLine("    data:{")
        sb.AppendLine("      labels:['Easy','Medium','Hard','Expert'],")
        sb.AppendLine("      datasets:[")
        sb.AppendLine("        {label:'Correct',data:[" & easyC & "," & medC & "," & hardC & "," & expC & "],backgroundColor:'rgba(37, 150, 190, 0.7)',borderRadius:4},")
        sb.AppendLine("        {label:'Wrong',  data:[" & easyW & "," & medW & "," & hardW & "," & expW & "],backgroundColor:'rgba(244, 67, 54, 0.7)',borderRadius:4}")
        sb.AppendLine("      ]")
        sb.AppendLine("    },")
        sb.AppendLine("    options:{")
        sb.AppendLine("      responsive:true, maintainAspectRatio:false,")
        sb.AppendLine("      plugins:{ legend:{labels:{color:'#666'}} },")
        sb.AppendLine("      scales:{")
        sb.AppendLine("        x:{ticks:{color:'#666'},grid:{display:false},stacked:false},")
        sb.AppendLine("        y:{ticks:{color:'#666',stepSize:1},grid:{color:'#eee'},min:0}")
        sb.AppendLine("      }")
        sb.AppendLine("    }")
        sb.AppendLine("  });")
        sb.AppendLine("})();")
        sb.AppendLine("</script>")
        litChartScript.Text = sb.ToString()
    End Sub

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
