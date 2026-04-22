Imports System.Data

Partial Class Student_AttemptQuiz
    Inherits System.Web.UI.Page

    ' Session keys
    Private Const SK_QUIZID   As String = "AQ_QuizID"
    Private Const SK_QList    As String = "AQ_Questions"    ' List(Of QuizQuestion)
    Private Const SK_CurrIdx  As String = "AQ_CurrentIdx"
    Private Const SK_Start    As String = "AQ_StartTime"
    Private Const SK_Allowed  As String = "AQ_AllowedMins"
    Private Const SK_Shuffle  As String = "AQ_ShuffleOpts"

    Structure QuizQuestion
        Dim QuestionID   As Integer
        Dim Statement    As String
        Dim OptionA, OptionB, OptionC, OptionD As String
        Dim CorrectOption As String
        Dim DifficultyLevel As String
        Dim SubjectName  As String
        Dim ImagePath    As String
        Dim ShuffleOrder As String()  ' Array of {"A","B","C","D"} in display order
    End Structure

    Private ReadOnly Property StudentID As Integer
        Get
            Return CInt(Session("UserID"))
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Role")?.ToString() <> "Student" Then Response.Redirect("~/Login.aspx") : Return

        If Not IsPostBack Then
            Dim qidStr = Request.QueryString("quizid")
            If String.IsNullOrEmpty(qidStr) Then Response.Redirect("~/Student/Dashboard.aspx") : Return
            Dim quizID As Integer = CInt(qidStr)
            InitQuiz(quizID)
        Else
            If Session(SK_QList) IsNot Nothing Then
                ShowCurrentQuestion()
            End If
        End If
    End Sub

    Private Sub InitQuiz(quizID As Integer)
        ' Check already attempted
        Dim alreadyDone = DBHelper.Exists(
            "SELECT COUNT(*) FROM Results WHERE StudentID=@s AND QuizID=@q",
            DBHelper.Param("@s", StudentID), DBHelper.Param("@q", quizID))

        If alreadyDone Then
            pnlAlreadyDone.Visible = True
            pnlQuiz.Visible        = False
            Return
        End If

        ' Load quiz settings
        Dim quizDt = DBHelper.GetDataTable(
            "SELECT q.QuizTitle, q.AllowedTime, q.TotalQuestions, q.RandomizeQ, q.ShuffleOptions
             FROM Quiz q WHERE q.QuizID=@id AND q.IsPublished=1",
            DBHelper.Param("@id", quizID))

        If quizDt.Rows.Count = 0 Then Response.Redirect("~/Student/Dashboard.aspx") : Return

        Dim quizRow   = quizDt.Rows(0)
        Dim randomizeQ  = CBool(quizRow("RandomizeQ"))
        Dim shuffleOpts = CBool(quizRow("ShuffleOptions"))
        Dim totalQ      = CInt(quizRow("TotalQuestions"))
        Dim allowedMins = CInt(quizRow("AllowedTime"))

        ' Load questions linked to quiz
        Dim qDt = DBHelper.GetDataTable("
            SELECT qt.QuestionID, qt.QuestionStatement,
                   qt.OptionA, qt.OptionB, qt.OptionC, qt.OptionD,
                   qt.CorrectOption, qt.DifficultyLevel, qt.ImagePath,
                   s.SubjectName
            FROM QuizQuestions qq
            JOIN QuestionsTable qt ON qq.QuestionID = qt.QuestionID
            JOIN Subjects s        ON qt.SubjectID  = s.SubjectID
            WHERE qq.QuizID = @qid
            ORDER BY qq.DisplayOrder",
            DBHelper.Param("@qid", quizID))

        ' Build list and optionally randomize
        Dim qList As New List(Of QuizQuestion)
        For Each row As DataRow In qDt.Rows
            Dim qq As New QuizQuestion With {
                .QuestionID    = CInt(row("QuestionID")),
                .Statement     = row("QuestionStatement").ToString(),
                .OptionA       = row("OptionA").ToString(),
                .OptionB       = row("OptionB").ToString(),
                .OptionC       = row("OptionC").ToString(),
                .OptionD       = row("OptionD").ToString(),
                .CorrectOption = row("CorrectOption").ToString(),
                .DifficultyLevel = row("DifficultyLevel").ToString(),
                .SubjectName   = row("SubjectName").ToString(),
                .ImagePath     = If(row("ImagePath") Is DBNull.Value, "", row("ImagePath").ToString())
            }
            ' Build shuffle order
            Dim letters() As String = {"A", "B", "C", "D"}
            If shuffleOpts Then ShuffleArray(letters)
            qq.ShuffleOrder = letters
            qList.Add(qq)
        Next

        If randomizeQ Then ShuffleList(qList)

        ' Take only totalQ questions
        If qList.Count > totalQ Then qList = qList.Take(totalQ).ToList()

        ' Store in Session
        Session(SK_QUIZID)  = quizID
        Session(SK_QList)   = qList
        Session(SK_CurrIdx) = 0
        Session(SK_Start)   = DateTime.Now
        Session(SK_Allowed) = allowedMins
        Session(SK_Shuffle) = shuffleOpts

        litQuizTitle.Text = quizRow("QuizTitle").ToString()
        litQTotal.Text    = qList.Count.ToString()

        pnlQuiz.Visible = True
        ShowCurrentQuestion()
    End Sub

    Private Sub ShowCurrentQuestion()
        Dim qList   = CType(Session(SK_QList), List(Of QuizQuestion))
        Dim currIdx = CInt(Session(SK_CurrIdx))

        If currIdx >= qList.Count Then
            FinaliseQuiz() : Return
        End If

        Dim qq = qList(currIdx)
        Dim qNum = currIdx + 1

        ' Timer
        Dim startTime   = CDate(Session(SK_Start))
        Dim allowedMins = CInt(Session(SK_Allowed))
        Dim elapsed     = CInt((DateTime.Now - startTime).TotalSeconds)
        Dim secsLeft    = Math.Max(0, allowedMins * 60 - elapsed)
        hfTimeLeft.Value  = secsLeft.ToString()

        ' Fill UI
        litQuizTitle.Text  = DBHelper.ExecuteScalar(
            "SELECT QuizTitle FROM Quiz WHERE QuizID=@id",
            DBHelper.Param("@id", CInt(Session(SK_QUIZID)))).ToString()
        litQNum.Text       = qNum.ToString()
        litQNumBadge.Text  = qNum.ToString()
        litQTotal.Text     = qList.Count.ToString()
        litQuestion.Text   = qq.Statement
        litDiffLabel.Text  = qq.DifficultyLevel
        litDiffClass.Text  = qq.DifficultyLevel.ToLower()
        litSubject.Text    = qq.SubjectName
        hfQIndex.Value     = currIdx.ToString()

        ' Image
        If Not String.IsNullOrEmpty(qq.ImagePath) Then
            pnlImage.Visible = True
            Dim img = CType(pnlImage.FindControl("qImage"), System.Web.UI.HtmlControls.HtmlImage)
            If img IsNot Nothing Then img.Src = ResolveUrl(qq.ImagePath)
        Else
            pnlImage.Visible = False
        End If

        ' Build options list respecting shuffle order
        Dim opts = New List(Of Object)
        Dim labels() As String = {"1", "2", "3", "4"}
        For i As Integer = 0 To 3
            Dim letter = qq.ShuffleOrder(i)
            Dim text   As String
            Select Case letter
                Case "A" : text = qq.OptionA
                Case "B" : text = qq.OptionB
                Case "C" : text = qq.OptionC
                Case Else : text = qq.OptionD
            End Select
            opts.Add(New With {
                .Letter       = letter,
                .DisplayLabel = labels(i),
                .Text         = text
            })
        Next
        rptOptions.DataSource = opts
        rptOptions.DataBind()

        ' Show Next or Submit
        Dim isLast = (currIdx = qList.Count - 1)
        btnNext.Visible   = Not isLast
        btnSubmit.Visible = isLast

        pnlQuiz.Visible = True
    End Sub

    Protected Sub btnNext_Click(sender As Object, e As EventArgs)
        SaveCurrentAnswer()
        Dim currIdx = CInt(Session(SK_CurrIdx))

        ' Check time
        Dim startTime   = CDate(Session(SK_Start))
        Dim allowedMins = CInt(Session(SK_Allowed))
        Dim secsLeft    = CInt((DateTime.Now - startTime).TotalSeconds)
        If secsLeft >= allowedMins * 60 Then
            FinaliseQuiz() : Return
        End If

        Session(SK_CurrIdx) = currIdx + 1
        ShowCurrentQuestion()
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        SaveCurrentAnswer()
        FinaliseQuiz()
    End Sub

    Private Sub SaveCurrentAnswer()
        Dim studentAns = hfAnswer.Value.Trim()
        Dim currIdx    = CInt(hfQIndex.Value)
        Dim qList      = CType(Session(SK_QList), List(Of QuizQuestion))
        If currIdx >= qList.Count Then Return

        Dim qq      = qList(currIdx)
        Dim quizID  = CInt(Session(SK_QUIZID))
        Dim sid     = StudentID
        Dim qNum    = currIdx + 1

        ' Check not already saved (prevent double-save on F5)
        Dim exists = DBHelper.Exists(
            "SELECT COUNT(*) FROM Answers WHERE StudentID=@s AND QuizID=@q AND QNo=@n",
            DBHelper.Param("@s", sid), DBHelper.Param("@q", quizID), DBHelper.Param("@n", qNum))
        If exists Then Return

        Dim isCorrect As Boolean = (studentAns <> "" AndAlso studentAns = qq.CorrectOption)
        Dim marks     As Decimal = If(isCorrect, 1D, 0D)

        DBHelper.ExecuteNonQuery("
            INSERT INTO Answers (StudentID,QuizID,QuestionID,QNo,CorrectAns,StudentAns,Marks)
            VALUES (@sid,@qid,@qqid,@qno,@ca,@sa,@m)",
            DBHelper.Param("@sid",  sid),
            DBHelper.Param("@qid",  quizID),
            DBHelper.Param("@qqid", qq.QuestionID),
            DBHelper.Param("@qno",  qNum),
            DBHelper.Param("@ca",   qq.CorrectOption),
            DBHelper.Param("@sa",   If(studentAns = "", CObj(DBNull.Value), CObj(studentAns))),
            DBHelper.Param("@m",    marks))

        ' Reset hidden field
        hfAnswer.Value = ""
    End Sub

    Private Sub FinaliseQuiz()
        Dim qList  = CType(Session(SK_QList), List(Of QuizQuestion))
        Dim quizID = CInt(Session(SK_QUIZID))
        Dim sid    = StudentID

        Dim totalQ    = qList.Count
        Dim obtained  = CDec(DBHelper.ExecuteScalar(
            "SELECT SUM(Marks) FROM Answers WHERE StudentID=@s AND QuizID=@q",
            DBHelper.Param("@s", sid), DBHelper.Param("@q", quizID)))
        Dim pct = Math.Round(obtained / totalQ * 100, 2)

        ' Insert result if not already there
        Dim resultExists = DBHelper.Exists(
            "SELECT COUNT(*) FROM Results WHERE StudentID=@s AND QuizID=@q",
            DBHelper.Param("@s", sid), DBHelper.Param("@q", quizID))

        If Not resultExists Then
            DBHelper.ExecuteNonQuery("
                INSERT INTO Results (StudentID,QuizID,TotalMarks,ObtainedMarks,Percentage)
                VALUES (@sid,@qid,@tot,@obt,@pct)",
                DBHelper.Param("@sid", sid),
                DBHelper.Param("@qid", quizID),
                DBHelper.Param("@tot", totalQ),
                DBHelper.Param("@obt", obtained),
                DBHelper.Param("@pct", pct))
        End If

        ' Clear quiz session
        Session.Remove(SK_QUIZID)
        Session.Remove(SK_QList)
        Session.Remove(SK_CurrIdx)
        Session.Remove(SK_Start)
        Session.Remove(SK_Allowed)
        Session.Remove(SK_Shuffle)

        Response.Redirect($"~/Student/MyResults.aspx?quizid={quizID}&done=1")
    End Sub

    ' --- Helpers ---
    Private Shared rng As New Random()

    Private Sub ShuffleArray(arr As String())
        For i As Integer = arr.Length - 1 To 1 Step -1
            Dim j = rng.Next(0, i + 1)
            Dim tmp = arr(i) : arr(i) = arr(j) : arr(j) = tmp
        Next
    End Sub

    Private Sub ShuffleList(lst As List(Of QuizQuestion))
        For i As Integer = lst.Count - 1 To 1 Step -1
            Dim j = rng.Next(0, i + 1)
            Dim tmp = lst(i) : lst(i) = lst(j) : lst(j) = tmp
        Next
    End Sub

End Class
