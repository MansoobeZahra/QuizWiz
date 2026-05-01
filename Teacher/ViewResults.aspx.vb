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

        ' Chart generation removed for simplicity
        litChartJS.Text = ""
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

End Class
