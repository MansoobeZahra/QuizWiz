Partial Class Teacher_CreateQuiz
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Role")?.ToString() <> "Teacher" Then Response.Redirect("~/Login.aspx") : Return
        If Not IsPostBack Then
            LoadSubjects()
            LoadQuestions()
        End If
    End Sub

    Private Sub LoadSubjects()
        Dim dt = DBHelper.GetDataTable("SELECT SubjectID, SubjectName FROM Subjects ORDER BY SubjectName")
        ddlSubject.DataSource     = dt
        ddlSubject.DataTextField  = "SubjectName"
        ddlSubject.DataValueField = "SubjectID"
        ddlSubject.DataBind()

        Dim teacherSubject = DBHelper.ExecuteScalar(
            "SELECT SubjectID FROM Users WHERE UserID=@id AND SubjectID IS NOT NULL",
            DBHelper.Param("@id", CInt(Session("UserID"))))
        If teacherSubject IsNot Nothing AndAlso teacherSubject IsNot DBNull.Value Then
            Dim item = ddlSubject.Items.FindByValue(teacherSubject.ToString())
            If item IsNot Nothing Then item.Selected = True
        End If
    End Sub

    Private Sub LoadQuestions()
        Dim sid = ddlSubject.SelectedValue
        Dim dt = DBHelper.GetDataTable(
            "SELECT QuestionID, QuestionStatement, DifficultyLevel, CorrectOption
             FROM QuestionsTable WHERE SubjectID=@sid AND CreatedBy=@tid ORDER BY CreatedAt DESC",
            DBHelper.Param("@sid", CInt(sid)),
            DBHelper.Param("@tid", CInt(Session("UserID"))))

        pnlNoQ.Visible = (dt.Rows.Count = 0)
        rptQuestions.DataSource = dt
        rptQuestions.DataBind()
    End Sub

    Protected Sub ddlSubject_Changed(sender As Object, e As EventArgs)
        hfSelected.Value = ""
        LoadQuestions()
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        pnlError.Visible   = False
        pnlSuccess.Visible = False

        Dim btn       = CType(sender, System.Web.UI.WebControls.Button)
        Dim publish   = (btn.CommandArgument = "publish")
        Dim selectedQ = hfSelected.Value.Trim()

        ' Validate
        If String.IsNullOrWhiteSpace(txtTitle.Text) Then ShowError("Quiz title is required.") : Return
        Dim totalQ As Integer = 0
        If Not Integer.TryParse(txtTotalQ.Text, totalQ) OrElse totalQ < 1 Then
            ShowError("Number of questions must be at least 1.") : Return
        End If
        Dim allowedTime As Integer = 0
        If Not Integer.TryParse(txtTime.Text, allowedTime) OrElse allowedTime < 1 Then
            ShowError("Allowed time must be at least 1 minute.") : Return
        End If
        If String.IsNullOrWhiteSpace(selectedQ) Then
            ShowError("Please select at least one question.") : Return
        End If

        Dim qids As String() = selectedQ.Split(","c)
        If qids.Length < totalQ Then
            ShowError($"You selected {qids.Length} question(s) but set Total Questions = {totalQ}. Please select at least {totalQ} question(s) or lower the total.")
            Return
        End If

        Dim startTime As Object = DBNull.Value
        If Not String.IsNullOrWhiteSpace(txtStart.Text) Then
            Dim dt As Date
            If Date.TryParse(txtStart.Text, dt) Then startTime = dt
        End If

        Try
            ' Insert quiz
            Dim quizID As Integer = CInt(DBHelper.ExecuteScalar("
                INSERT INTO Quiz (QuizTitle,SubjectID,StartTime,AllowedTime,TotalQuestions,
                                  RandomizeQ,ShuffleOptions,Remarks,ReviewAnswer,IsPublished,CreatedBy)
                VALUES (@t,@s,@st,@at,@tq,@rq,@so,@rem,@rev,@pub,@by);
                SELECT SCOPE_IDENTITY();",
                DBHelper.Param("@t",   txtTitle.Text.Trim()),
                DBHelper.Param("@s",   CInt(ddlSubject.SelectedValue)),
                DBHelper.Param("@st",  startTime),
                DBHelper.Param("@at",  allowedTime),
                DBHelper.Param("@tq",  totalQ),
                DBHelper.Param("@rq",  chkRandomize.Checked),
                DBHelper.Param("@so",  chkShuffle.Checked),
                DBHelper.Param("@rem", If(String.IsNullOrWhiteSpace(txtRemarks.Text), DBNull.Value, CObj(txtRemarks.Text.Trim()))),
                DBHelper.Param("@rev", chkReview.Checked),
                DBHelper.Param("@pub", publish),
                DBHelper.Param("@by",  CInt(Session("UserID")))))

            ' Insert quiz-question links
            Dim order As Integer = 1
            For Each qid As String In qids
                Dim q As Integer = 0
                If Integer.TryParse(qid.Trim(), q) Then
                    DBHelper.ExecuteNonQuery(
                        "INSERT INTO QuizQuestions (QuizID,QuestionID,DisplayOrder) VALUES (@qz,@q,@o)",
                        DBHelper.Param("@qz", quizID),
                        DBHelper.Param("@q",  q),
                        DBHelper.Param("@o",  order))
                    order += 1
                End If
            Next

            ' Notify students if publishing
            If publish Then
                Dim students = DBHelper.GetDataTable("SELECT UserID FROM Users WHERE Role='Student' AND IsActive=1")
                For Each row As System.Data.DataRow In students.Rows
                    DBHelper.ExecuteNonQuery(
                        "INSERT INTO Notifications(ToUserID,FromUserID,Message) VALUES(@to,@from,@msg)",
                        DBHelper.Param("@to",   CInt(row("UserID"))),
                        DBHelper.Param("@from",  CInt(Session("UserID"))),
                        DBHelper.Param("@msg",   $"New quiz published: ""{txtTitle.Text.Trim()}"". Open your dashboard to attempt it!"))
                Next
            End If

            pnlSuccess.Visible = True
            litSuccess.Text    = If(publish,
                $"Quiz ""{txtTitle.Text.Trim()}"" published! Students have been notified. <a href='Dashboard.aspx' style='color:inherit;font-weight:700;'>Back to Dashboard</a>",
                $"Quiz saved as draft. <a href='Dashboard.aspx' style='color:inherit;font-weight:700;'>Back to Dashboard</a>")

            ' Reset form
            txtTitle.Text = "" : txtRemarks.Text = "" : hfSelected.Value = ""
            txtTime.Text = "30" : txtTotalQ.Text = "10"
            chkRandomize.Checked = True : chkShuffle.Checked = True : chkReview.Checked = False

        Catch ex As Exception
            ShowError("Error creating quiz: " & ex.Message)
        End Try
    End Sub

    Private Sub ShowError(msg As String)
        pnlError.Visible = True
        litError.Text    = msg
    End Sub

End Class
