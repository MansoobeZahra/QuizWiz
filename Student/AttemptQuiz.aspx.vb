Imports System.Data
Imports System.Linq

Partial Class Student_AttemptQuiz
    Inherits System.Web.UI.Page

    Private Const SK_QUIZID   As String = "AQ_QuizID"
    Private Const SK_QList    As String = "AQ_Questions"
    Private Const SK_CurrIdx  As String = "AQ_CurrentIdx"
    Private Const SK_Total    As String = "AQ_Total"
    Private Const SK_Start    As String = "AQ_StartTime"
    Private Const SK_Allowed  As String = "AQ_AllowedMins"
    Private Const SK_NegMark  As String = "AQ_NegMarking"
    Private Const SK_NegVal   As String = "AQ_NegMarks"

    Structure QuizQuestion
        Dim QuestionID    As Integer
        Dim Statement     As String
        Dim OptionA, OptionB, OptionC, OptionD As String
        Dim CorrectOption  As String
        Dim CorrectOptions As String   ' authoritative: letter(s) or paragraph text
        Dim DifficultyLevel As String
        Dim SubjectName    As String
        Dim ImagePath      As String
        Dim QuestionType   As String   ' Radio | Checkbox | Paragraph
        Dim ShuffleOrder   As String() ' e.g. {"C","A","D","B"}
    End Structure

    Private ReadOnly Property StudentID As Integer
        Get
            Return CInt(Session("UserID"))
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Student")
        If Not IsPostBack Then
            Dim qidStr = Request.QueryString("quizid")
            If String.IsNullOrEmpty(qidStr) Then Response.Redirect("~/Student/Dashboard.aspx") : Return
            InitQuiz(CInt(qidStr))
        Else
            If Session(SK_QList) IsNot Nothing Then ShowCurrentQuestion()
        End If
    End Sub

    Private Sub InitQuiz(quizID As Integer)
        ' Already attempted?
        If DBHelper.Exists(
            "SELECT COUNT(*) FROM Results WHERE StudentID=@s AND QuizID=@q",
            DBHelper.Param("@s", StudentID), DBHelper.Param("@q", quizID)) Then
            pnlAlreadyDone.Visible = True : pnlQuiz.Visible = False : Return
        End If

        Dim quizDt = DBHelper.GetDataTable(
            "SELECT QuizTitle,AllowedTime,TotalQuestions,RandomizeQ,ShuffleOptions,
                    NegativeMarking,NegativeMarks
             FROM Quiz WHERE QuizID=@id AND IsPublished=1",
            DBHelper.Param("@id", quizID))
        If quizDt.Rows.Count = 0 Then Response.Redirect("~/Student/Dashboard.aspx") : Return

        Dim qr          = quizDt.Rows(0)
        Dim randomizeQ  = CBool(qr("RandomizeQ"))
        Dim shuffleOpts = CBool(qr("ShuffleOptions"))
        Dim totalQ      = CInt(qr("TotalQuestions"))
        Dim allowedMins = CInt(qr("AllowedTime"))
        Dim negMarking  = CBool(qr("NegativeMarking"))
        Dim negMarks    = CDec(qr("NegativeMarks"))

        Dim qDt = DBHelper.GetDataTable("
            SELECT qt.QuestionID, qt.QuestionStatement,
                   qt.OptionA, qt.OptionB, qt.OptionC, qt.OptionD,
                   qt.CorrectOption, qt.CorrectOptions, qt.DifficultyLevel,
                   qt.ImagePath, qt.QuestionType, s.SubjectName
            FROM QuizQuestions qq
            JOIN QuestionsTable qt ON qq.QuestionID = qt.QuestionID
            JOIN Subjects s        ON qt.SubjectID  = s.SubjectID
            WHERE qq.QuizID = @qid ORDER BY qq.DisplayOrder",
            DBHelper.Param("@qid", quizID))

        Dim qList As New List(Of QuizQuestion)
        For Each row As DataRow In qDt.Rows
            Dim letters() As String = {"A", "B", "C", "D"}
            If shuffleOpts Then ShuffleArray(letters)
            qList.Add(New QuizQuestion With {
                .QuestionID     = CInt(row("QuestionID")),
                .Statement      = row("QuestionStatement").ToString(),
                .OptionA        = row("OptionA").ToString(),
                .OptionB        = row("OptionB").ToString(),
                .OptionC        = row("OptionC").ToString(),
                .OptionD        = row("OptionD").ToString(),
                .CorrectOption  = If(row("CorrectOption") Is DBNull.Value, "", row("CorrectOption").ToString()),
                .CorrectOptions = If(row("CorrectOptions") Is DBNull.Value, "", row("CorrectOptions").ToString()),
                .DifficultyLevel = row("DifficultyLevel").ToString(),
                .SubjectName    = row("SubjectName").ToString(),
                .ImagePath      = If(row("ImagePath") Is DBNull.Value, "", row("ImagePath").ToString()),
                .QuestionType   = row("QuestionType").ToString(),
                .ShuffleOrder   = letters
            })
        Next

        If randomizeQ Then ShuffleList(qList)
        If qList.Count > totalQ Then qList = qList.Take(totalQ).ToList()

        Session(SK_QUIZID)  = quizID
        Session(SK_QList)   = qList
        Session(SK_CurrIdx) = 0
        Session(SK_Total)   = qList.Count
        Session(SK_Start)   = DateTime.Now
        Session(SK_Allowed) = allowedMins
        Session(SK_NegMark) = negMarking
        Session(SK_NegVal)  = negMarks

        pnlQuiz.Visible = True
        ShowCurrentQuestion()
    End Sub

    Private Sub ShowCurrentQuestion()
        Dim qList   = CType(Session(SK_QList), List(Of QuizQuestion))
        Dim currIdx = CInt(Session(SK_CurrIdx))
        If currIdx >= qList.Count Then FinaliseQuiz() : Return

        Dim qq          = qList(currIdx)
        Dim qNum        = currIdx + 1
        Dim negMarking  = CBool(Session(SK_NegMark))
        Dim negMarks    = CDec(Session(SK_NegVal))

        ' Timer
        Dim elapsed  = CInt((DateTime.Now - CDate(Session(SK_Start))).TotalSeconds)
        Dim secsLeft = Math.Max(0, CInt(Session(SK_Allowed)) * 60 - elapsed)
        If secsLeft = 0 Then FinaliseQuiz() : Return
        hfTimeLeft.Value = secsLeft.ToString()

        ' Quiz title
        litQuizTitle.Text  = DBHelper.ExecuteScalar("SELECT QuizTitle FROM Quiz WHERE QuizID=@id",
            DBHelper.Param("@id", CInt(Session(SK_QUIZID)))).ToString()
        litQNum.Text       = qNum.ToString()
        litQBadge.Text     = qNum.ToString()
        litQTotal.Text     = qList.Count.ToString()
        litQuestion.Text   = qq.Statement
        litDiffLabel.Text  = qq.DifficultyLevel
        litDiffClass.Text  = qq.DifficultyLevel.ToLower()
        litSubject.Text    = qq.SubjectName
        litQTypeLabel.Text = qq.QuestionType
        hfQIndex.Value     = currIdx.ToString()
        hfQuestionType.Value = qq.QuestionType

        ' Negative marking badge
        pnlNegBadge.Visible = negMarking
        If negMarking Then litNegVal.Text = negMarks.ToString("0.##")

        ' Image
        If Not String.IsNullOrEmpty(qq.ImagePath) Then
            pnlImage.Visible = True
            Dim img = CType(pnlImage.FindControl("qImage"), System.Web.UI.HtmlControls.HtmlImage)
            If img IsNot Nothing Then img.Src = ResolveUrl(qq.ImagePath)
        Else
            pnlImage.Visible = False
        End If

        ' Show correct question-type panel
        Dim isRadio    = (qq.QuestionType = "Radio")
        Dim isCheckbox = (qq.QuestionType = "Checkbox")
        Dim isPara     = (qq.QuestionType = "Paragraph")

        pnlRadio.Visible     = isRadio
        pnlCheckbox.Visible  = isCheckbox
        pnlParagraph.Visible = isPara
        pnlMultiHint.Visible = isCheckbox

        Dim opts = BuildOptionsList(qq)

        If isRadio Then
            rptOptions.DataSource = opts
            rptOptions.DataBind()
        ElseIf isCheckbox Then
            rptCheckboxOpts.DataSource = opts
            rptCheckboxOpts.DataBind()
        End If

        ' Nav
        Dim isLast = (currIdx = qList.Count - 1)
        btnNext.Visible   = Not isLast
        btnSubmit.Visible = isLast

        pnlQuiz.Visible = True
        hfAnswer.Value  = ""
    End Sub

    Private Function BuildOptionsList(qq As QuizQuestion) As List(Of Object)
        Dim labels() As String = {"1", "2", "3", "4"}
        Dim result    As New List(Of Object)
        For i As Integer = 0 To 3
            Dim letter = qq.ShuffleOrder(i)
            Dim text   As String
            Select Case letter
                Case "A" : text = qq.OptionA
                Case "B" : text = qq.OptionB
                Case "C" : text = qq.OptionC
                Case Else: text = qq.OptionD
            End Select
            result.Add(New With {.Letter = letter, .DisplayLabel = labels(i), .Text = text})
        Next
        Return result
    End Function

    Protected Sub btnNext_Click(sender As Object, e As EventArgs)
        SaveCurrentAnswer()
        Dim currIdx = CInt(Session(SK_CurrIdx))
        Dim elapsed = CInt((DateTime.Now - CDate(Session(SK_Start))).TotalSeconds)
        If elapsed >= CInt(Session(SK_Allowed)) * 60 Then FinaliseQuiz() : Return
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

        Dim qq       = qList(currIdx)
        Dim quizID   = CInt(Session(SK_QUIZID))
        Dim sid      = StudentID
        Dim qNum     = currIdx + 1
        Dim negMark  = CBool(Session(SK_NegMark))
        Dim negMarks = CDec(Session(SK_NegVal))

        If DBHelper.Exists(
            "SELECT COUNT(*) FROM Answers WHERE StudentID=@s AND QuizID=@q AND QNo=@n",
            DBHelper.Param("@s", sid), DBHelper.Param("@q", quizID), DBHelper.Param("@n", qNum)) Then Return

        Dim marks = CalculateMarks(qq, studentAns, negMark, negMarks)

        DBHelper.ExecuteNonQuery("
            INSERT INTO Answers (StudentID,QuizID,QuestionID,QNo,CorrectAns,StudentAns,Marks)
            VALUES (@sid,@qid,@qqid,@qno,@ca,@sa,@m)",
            DBHelper.Param("@sid",  sid),
            DBHelper.Param("@qid",  quizID),
            DBHelper.Param("@qqid", qq.QuestionID),
            DBHelper.Param("@qno",  qNum),
            DBHelper.Param("@ca",   qq.CorrectOptions),
            DBHelper.Param("@sa",   If(studentAns = "", CObj(DBNull.Value), CObj(studentAns))),
            DBHelper.Param("@m",    marks))

        hfAnswer.Value = ""
    End Sub

    Private Function CalculateMarks(qq As QuizQuestion, studentAns As String, negMarking As Boolean, negMarks As Decimal) As Decimal
        If qq.QuestionType = "Paragraph" Then
            If studentAns <> "" AndAlso
               String.Compare(studentAns.Trim(), qq.CorrectOptions.Trim(), StringComparison.OrdinalIgnoreCase) = 0 Then
                Return 1D
            End If
            Return 0D   ' No negative marking for paragraph

        ElseIf qq.QuestionType = "Checkbox" Then
            If studentAns = "" Then Return 0D
            Dim correctSet  = qq.CorrectOptions.Split(","c).Select(Function(x) x.Trim()).OrderBy(Function(x) x)
            Dim studentSet  = studentAns.Split(","c).Select(Function(x) x.Trim()).OrderBy(Function(x) x)
            Dim correctJoin = String.Join(",", correctSet)
            Dim studentJoin = String.Join(",", studentSet)
            If correctJoin = studentJoin Then Return 1D
            ' Check if any wrong option selected
            Dim correctList = qq.CorrectOptions.Split(","c).Select(Function(x) x.Trim()).ToList()
            Dim anyWrong    = studentAns.Split(","c).Any(Function(s) Not correctList.Contains(s.Trim()))
            If anyWrong AndAlso negMarking Then Return -negMarks
            Return 0D

        Else  ' Radio
            If studentAns = "" Then Return 0D
            If studentAns = qq.CorrectOptions Then Return 1D
            If negMarking Then Return -negMarks
            Return 0D
        End If
    End Function

    Private Sub FinaliseQuiz()
        Dim qList  = CType(Session(SK_QList), List(Of QuizQuestion))
        Dim quizID = CInt(Session(SK_QUIZID))
        Dim sid    = StudentID
        Dim totalQ = qList.Count

        Dim rawObtained = CDec(If(DBHelper.ExecuteScalar(
            "SELECT SUM(Marks) FROM Answers WHERE StudentID=@s AND QuizID=@q",
            DBHelper.Param("@s", sid), DBHelper.Param("@q", quizID)), 0))
        ' Clamp to 0 minimum (total negative marks cannot go below 0)
        Dim obtained = Math.Max(0D, rawObtained)
        Dim pct      = Math.Round(obtained / totalQ * 100, 2)

        If Not DBHelper.Exists(
            "SELECT COUNT(*) FROM Results WHERE StudentID=@s AND QuizID=@q",
            DBHelper.Param("@s", sid), DBHelper.Param("@q", quizID)) Then
            DBHelper.ExecuteNonQuery("
                INSERT INTO Results (StudentID,QuizID,TotalMarks,ObtainedMarks,Percentage)
                VALUES (@sid,@qid,@tot,@obt,@pct)",
                DBHelper.Param("@sid", sid), DBHelper.Param("@qid", quizID),
                DBHelper.Param("@tot", totalQ), DBHelper.Param("@obt", obtained),
                DBHelper.Param("@pct", pct))
        End If

        Session.Remove(SK_QUIZID) : Session.Remove(SK_QList) : Session.Remove(SK_CurrIdx)
        Session.Remove(SK_Total)  : Session.Remove(SK_Start) : Session.Remove(SK_Allowed)
        Session.Remove(SK_NegMark): Session.Remove(SK_NegVal)

        Response.Redirect($"~/Student/MyResults.aspx?quizid={quizID}&done=1")
    End Sub

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
