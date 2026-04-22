Imports System.Data

Partial Class Teacher_TestQuiz
    Inherits System.Web.UI.Page

    Private Const SK_QList  As String = "TQ_Questions"
    Private Const SK_Idx    As String = "TQ_Idx"
    Private Const SK_Total  As String = "TQ_Total"
    Private Const SK_Title  As String = "TQ_Title"

    Structure PreviewQuestion
        Dim QuestionID      As Integer
        Dim Statement       As String
        Dim OptionA, OptionB, OptionC, OptionD As String
        Dim CorrectOption   As String
        Dim CorrectOptions  As String
        Dim DifficultyLevel As String
        Dim SubjectName     As String
        Dim ImagePath       As String
        Dim QuestionType    As String
        Dim ShuffleOrder    As String()
    End Structure

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Teacher")
        If Not IsPostBack Then
            LoadQuizList()
            pnlSelect.Visible  = True
            pnlPreview.Visible = False
        Else
            If Session(SK_QList) IsNot Nothing Then
                ShowQuestion()
            End If
        End If
    End Sub

    Private Sub LoadQuizList()
        Dim dt = DBHelper.GetDataTable(
            "SELECT QuizID, QuizTitle FROM Quiz WHERE CreatedBy=@tid ORDER BY CreatedAt DESC",
            DBHelper.Param("@tid", CInt(Session("UserID"))))
        ddlQuiz.Items.Clear()
        ddlQuiz.Items.Add(New System.Web.UI.WebControls.ListItem("-- Select a quiz --", ""))
        For Each row As DataRow In dt.Rows
            ddlQuiz.Items.Add(New System.Web.UI.WebControls.ListItem(row("QuizTitle").ToString(), row("QuizID").ToString()))
        Next
    End Sub

    Protected Sub btnStart_Click(sender As Object, e As EventArgs)
        If String.IsNullOrEmpty(ddlQuiz.SelectedValue) Then
            pnlSelectMsg.Visible = True
            litSelectErr.Text    = "Please select a quiz first."
            Return
        End If
        InitPreview(CInt(ddlQuiz.SelectedValue))
    End Sub

    Private Sub InitPreview(quizID As Integer)
        Dim quizDt = DBHelper.GetDataTable(
            "SELECT q.QuizTitle, q.ShuffleOptions FROM Quiz q WHERE q.QuizID=@id",
            DBHelper.Param("@id", quizID))
        If quizDt.Rows.Count = 0 Then Return

        Dim shuffleOpts = CBool(quizDt.Rows(0)("ShuffleOptions"))
        Session(SK_Title) = quizDt.Rows(0)("QuizTitle").ToString()

        Dim qDt = DBHelper.GetDataTable( _
            "SELECT qt.QuestionID, qt.QuestionStatement, " & _
            "       qt.OptionA, qt.OptionB, qt.OptionC, qt.OptionD, " & _
            "       qt.CorrectOption, qt.CorrectOptions, qt.DifficultyLevel, " & _
            "       qt.ImagePath, qt.QuestionType, s.SubjectName " & _
            "FROM QuizQuestions qq " & _
            "JOIN QuestionsTable qt ON qq.QuestionID = qt.QuestionID " & _
            "JOIN Subjects s        ON qt.SubjectID  = s.SubjectID " & _
            "WHERE qq.QuizID = @qid ORDER BY qq.DisplayOrder",
            DBHelper.Param("@qid", quizID))

        Dim qList As New List(Of PreviewQuestion)
        For Each row As DataRow In qDt.Rows
            Dim letters() As String = {"A", "B", "C", "D"}
            If shuffleOpts Then ShuffleArray(letters)
            
            Dim q As New PreviewQuestion()
            q.QuestionID     = CInt(row("QuestionID"))
            q.Statement      = row("QuestionStatement").ToString()
            q.OptionA        = row("OptionA").ToString()
            q.OptionB        = row("OptionB").ToString()
            q.OptionC        = row("OptionC").ToString()
            q.OptionD        = row("OptionD").ToString()
            q.CorrectOption  = If(row("CorrectOption") Is DBNull.Value, "", row("CorrectOption").ToString())
            q.CorrectOptions = If(row("CorrectOptions") Is DBNull.Value, "", row("CorrectOptions").ToString())
            q.DifficultyLevel = row("DifficultyLevel").ToString()
            q.SubjectName    = row("SubjectName").ToString()
            q.ImagePath      = If(row("ImagePath") Is DBNull.Value, "", row("ImagePath").ToString())
            q.QuestionType   = row("QuestionType").ToString()
            q.ShuffleOrder   = letters
            qList.Add(q)
        Next

        Session(SK_QList)  = qList
        Session(SK_Idx)    = 0
        Session(SK_Total)  = qList.Count

        pnlSelect.Visible  = False
        pnlPreview.Visible = True
        ShowQuestion()
    End Sub

    Private Sub ShowQuestion()
        Dim qList = CType(Session(SK_QList), List(Of PreviewQuestion))
        Dim idx   = CInt(Session(SK_Idx))
        If idx >= qList.Count OrElse idx < 0 Then Return

        Dim qq   = qList(idx)
        Dim qNum = idx + 1
        Dim tot  = qList.Count

        litQuizTitle.Text = Session(SK_Title).ToString()
        litQNum.Text      = qNum.ToString()
        litQBadge.Text    = qNum.ToString()
        litQTotal.Text    = tot.ToString()
        litQuestion.Text  = qq.Statement
        litDiff.Text      = qq.DifficultyLevel
        litDiffClass.Text = qq.DifficultyLevel.ToLower()
        litSubject.Text   = qq.SubjectName
        litQType.Text     = qq.QuestionType
        hfQIdx.Value      = idx.ToString()

        ' Image
        If Not String.IsNullOrEmpty(qq.ImagePath) Then
            pnlImg.Visible = True
            Dim img = CType(pnlImg.FindControl("qImg"), System.Web.UI.HtmlControls.HtmlImage)
            If img IsNot Nothing Then img.Src = ResolveUrl(qq.ImagePath)
        Else
            pnlImg.Visible = False
        End If

        ' Question type display
        If qq.QuestionType = "Paragraph" Then
            pnlOptionsPreview.Visible = False
            pnlParaPreview.Visible    = True
            litModelAnswer.Text       = qq.CorrectOptions
        Else
            pnlOptionsPreview.Visible = True
            pnlParaPreview.Visible    = False

            ' Determine correct set
            Dim correctSet As New HashSet(Of String)(
                qq.CorrectOptions.Split(","c).Select(Function(x) x.Trim()),
                StringComparer.OrdinalIgnoreCase)

            Dim opts As New List(Of Object)
            Dim labels() As String = {"1", "2", "3", "4"}
            For i As Integer = 0 To 3
                Dim letter = qq.ShuffleOrder(i)
                Dim text   As String = ""
                Select Case letter
                    Case "A" : text = qq.OptionA
                    Case "B" : text = qq.OptionB
                    Case "C" : text = qq.OptionC
                    Case Else: text = qq.OptionD
                End Select
                opts.Add(New With {
                    .Letter       = letter,
                    .DisplayLabel = labels(i),
                    .Text         = text,
                    .IsCorrect    = correctSet.Contains(letter)
                })
            Next
            rptPreviewOpts.DataSource = opts
            rptPreviewOpts.DataBind()
        End If

        ' Navigation buttons
        btnPrev.Enabled      = (idx > 0)
        Dim isLast = (idx = tot - 1)
        btnNextPreview.Visible = Not isLast
        btnFinish.Visible      = isLast

        pnlSelect.Visible  = False
        pnlPreview.Visible = True
    End Sub

    Protected Sub btnNextPreview_Click(sender As Object, e As EventArgs)
        Dim idx = CInt(hfQIdx.Value)
        Session(SK_Idx) = idx + 1
        ShowQuestion()
    End Sub

    Protected Sub btnPrev_Click(sender As Object, e As EventArgs)
        Dim idx = CInt(hfQIdx.Value)
        If idx > 0 Then Session(SK_Idx) = idx - 1
        ShowQuestion()
    End Sub

    Protected Sub btnExitPreview_Click(sender As Object, e As EventArgs)
        Session.Remove(SK_QList)
        Session.Remove(SK_Idx)
        Session.Remove(SK_Total)
        Session.Remove(SK_Title)
        Response.Redirect("~/Teacher/Dashboard.aspx")
    End Sub

    Private Shared rng As New Random()
    Private Sub ShuffleArray(arr As String())
        For i As Integer = arr.Length - 1 To 1 Step -1
            Dim j = rng.Next(0, i + 1)
            Dim tmp = arr(i) : arr(i) = arr(j) : arr(j) = tmp
        Next
    End Sub

End Class
