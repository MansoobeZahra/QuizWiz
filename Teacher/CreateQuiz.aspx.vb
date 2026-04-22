Partial Class Teacher_CreateQuiz
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Teacher")
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
        Dim teacherSub = DBHelper.ExecuteScalar(
            "SELECT SubjectID FROM Users WHERE UserID=@id AND SubjectID IS NOT NULL",
            DBHelper.Param("@id", CInt(Session("UserID"))))
        If teacherSub IsNot Nothing AndAlso teacherSub IsNot DBNull.Value Then
            Dim item = ddlSubject.Items.FindByValue(teacherSub.ToString())
            If item IsNot Nothing Then item.Selected = True
        End If
    End Sub

    Private Sub LoadQuestions()
        Dim sid = ddlSubject.SelectedValue
        Dim dt  = DBHelper.GetDataTable( _
            "SELECT QuestionID, QuestionStatement, DifficultyLevel, CorrectOption, CorrectOptions, QuestionType " & _
            "FROM QuestionsTable WHERE SubjectID=@s AND CreatedBy=@t ORDER BY CreatedAt DESC",
            DBHelper.Param("@s", CInt(sid)),
            DBHelper.Param("@t", CInt(Session("UserID"))))
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

        Dim btn     = CType(sender, System.Web.UI.WebControls.Button)
        Dim publish = (btn.CommandArgument = "publish")

        If String.IsNullOrWhiteSpace(txtTitle.Text) Then ShowError("Quiz title is required.") : Return

        Dim totalQ, allowedTime As Integer
        If Not Integer.TryParse(txtTotalQ.Text, totalQ) OrElse totalQ < 1 Then ShowError("Set at least 1 question.") : Return
        If Not Integer.TryParse(txtTime.Text, allowedTime) OrElse allowedTime < 1 Then ShowError("Allowed time must be at least 1 minute.") : Return

        Dim selectedQ = hfSelected.Value.Trim()
        If String.IsNullOrWhiteSpace(selectedQ) Then ShowError("Select at least one question.") : Return

        Dim qids = selectedQ.Split(","c)
        If qids.Length < totalQ Then
            ShowError(String.Format("You selected {0} question(s) but set Total = {1}. Select at least {1} or lower the total.", qids.Length, totalQ)) : Return
        End If

        Dim negMarking = chkNegMarking.Checked
        Dim negMarks   As Decimal = 0.25D
        If negMarking Then
            If Not Decimal.TryParse(txtNegMarks.Text, negMarks) OrElse negMarks <= 0 Then
                ShowError("Negative marks per wrong answer must be a positive number.") : Return
            End If
        End If

        Dim startTime As Object = DBNull.Value
        Dim parsedStart As Date
        If Not String.IsNullOrWhiteSpace(txtStart.Text) AndAlso Date.TryParse(txtStart.Text, parsedStart) Then
            startTime = parsedStart
        End If

        Try
            Dim quizID As Integer = CInt(DBHelper.ExecuteScalar( _
                "INSERT INTO Quiz (QuizTitle,SubjectID,StartTime,AllowedTime,TotalQuestions, " & _
                "                  RandomizeQ,ShuffleOptions,Remarks,ReviewAnswer,IsPublished,CreatedBy, " & _
                "                  NegativeMarking,NegativeMarks) " & _
                "VALUES (@t,@s,@st,@at,@tq,@rq,@so,@rem,@rev,@pub,@by,@nm,@nmv); " & _
                "SELECT SCOPE_IDENTITY();", _
                DBHelper.Param("@t",   txtTitle.Text.Trim()), _
                DBHelper.Param("@s",   CInt(ddlSubject.SelectedValue)), _
                DBHelper.Param("@st",  startTime), _
                DBHelper.Param("@at",  allowedTime), _
                DBHelper.Param("@tq",  totalQ), _
                DBHelper.Param("@rq",  chkRandomize.Checked), _
                DBHelper.Param("@so",  chkShuffle.Checked), _
                DBHelper.Param("@rem", If(String.IsNullOrWhiteSpace(txtRemarks.Text), CObj(DBNull.Value), CObj(txtRemarks.Text.Trim()))), _
                DBHelper.Param("@rev", chkReview.Checked), _
                DBHelper.Param("@pub", publish), _
                DBHelper.Param("@by",  CInt(Session("UserID"))), _
                DBHelper.Param("@nm",  negMarking), _
                DBHelper.Param("@nmv", negMarks)))

            Dim order As Integer = 1
            For Each qidStr As String In qids
                Dim q As Integer = 0
                If Integer.TryParse(qidStr.Trim(), q) Then
                    DBHelper.ExecuteNonQuery("INSERT INTO QuizQuestions (QuizID,QuestionID,DisplayOrder) VALUES (@qz,@q,@o)",
                        DBHelper.Param("@qz", quizID), DBHelper.Param("@q", q), DBHelper.Param("@o", order))
                    order += 1
                End If
            Next

            If publish Then
                Dim students = DBHelper.GetDataTable("SELECT UserID FROM Users WHERE Role='Student' AND IsActive=1")
                For Each row As System.Data.DataRow In students.Rows
                    DBHelper.ExecuteNonQuery( _
                        "INSERT INTO Notifications(ToUserID,FromUserID,Message) VALUES(@to,@from,@msg)", _
                        DBHelper.Param("@to", CInt(row("UserID"))), _
                        DBHelper.Param("@from", CInt(Session("UserID"))), _
                        DBHelper.Param("@msg", String.Format("New quiz available: ""{0}"". Open your dashboard to attempt it.", txtTitle.Text.Trim())))
                Next
            End If

            pnlSuccess.Visible = True
            litSuccess.Text = If(publish,
                String.Format("Quiz ""{0}"" published. Students have been notified. <a href='Dashboard.aspx' style='color:inherit;font-weight:700;'>Back to Dashboard</a>", txtTitle.Text.Trim()),
                "Quiz saved as draft. <a href='Dashboard.aspx' style='color:inherit;font-weight:700;'>Back to Dashboard</a>")

            txtTitle.Text = "" : txtRemarks.Text = "" : hfSelected.Value = ""
            txtTime.Text = "30" : txtTotalQ.Text = "10"
            chkRandomize.Checked = True : chkShuffle.Checked = True : chkReview.Checked = False
            chkNegMarking.Checked = False : txtNegMarks.Text = "0.25"

        Catch ex As Exception
            ShowError("Error creating quiz: " & ex.Message)
        End Try
    End Sub

    Private Sub ShowError(msg As String)
        pnlError.Visible = True
        litError.Text    = msg
    End Sub

End Class
