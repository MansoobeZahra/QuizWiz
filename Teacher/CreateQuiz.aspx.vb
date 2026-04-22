Imports System.Data

Partial Class Teacher_CreateQuiz
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Teacher")
        If Not IsPostBack Then
            LoadSubjects()
            mvWizard.ActiveViewIndex = 0
        End If
    End Sub

    Private Sub LoadSubjects()
        Dim dt = DBHelper.GetDataTable("SELECT SubjectID, SubjectName FROM Subjects ORDER BY SubjectName")
        ddlSubject.DataSource     = dt
        ddlSubject.DataTextField  = "SubjectName"
        ddlSubject.DataValueField = "SubjectID"
        ddlSubject.DataBind()
        
        Dim teacherSub = DBHelper.ExecuteScalar("SELECT SubjectID FROM Users WHERE UserID=@id AND SubjectID IS NOT NULL", DBHelper.Param("@id", CInt(Session("UserID"))))
        If teacherSub IsNot Nothing AndAlso teacherSub IsNot DBNull.Value Then
            Dim item = ddlSubject.Items.FindByValue(teacherSub.ToString())
            If item IsNot Nothing Then item.Selected = True
        End If
    End Sub

    Protected Sub chkNegMarking_Changed(sender As Object, e As EventArgs)
        divNegMarks.Visible = chkNegMarking.Checked
    End Sub

    ' --- STEP 1 ---
    Protected Sub btnStep1Next_Click(sender As Object, e As EventArgs)
        pnlError.Visible = False
        If String.IsNullOrWhiteSpace(txtTitle.Text) Then ShowError("Quiz title is required.") : Return
        
        Dim totalQ, allowedTime As Integer
        If Not Integer.TryParse(txtTotalQ.Text, totalQ) OrElse totalQ < 1 Then ShowError("Set at least 1 question.") : Return
        If Not Integer.TryParse(txtTime.Text, allowedTime) OrElse allowedTime < 1 Then ShowError("Allowed time must be at least 1 minute.") : Return

        Dim negMarks As Decimal = 0
        If chkNegMarking.Checked Then
            If Not Decimal.TryParse(txtNegMarks.Text, negMarks) OrElse negMarks <= 0 Then
                ShowError("Negative marks must be a positive number.") : Return
            End If
        End If

        Dim quizID As Integer
        If String.IsNullOrEmpty(hfQuizID.Value) Then
            ' Insert new draft
            quizID = CInt(DBHelper.ExecuteScalar( _
                "INSERT INTO Quiz (QuizTitle,SubjectID,AllowedTime,TotalQuestions,RandomizeQ,ShuffleOptions,Remarks,ReviewAnswer,IsPublished,CreatedBy,NegativeMarking,NegativeMarks) " & _
                "VALUES (@t,@s,@at,@tq,@rq,@so,@rem,@rev,0,@by,@nm,@nmv); SELECT SCOPE_IDENTITY();", _
                DBHelper.Param("@t", txtTitle.Text.Trim()), DBHelper.Param("@s", CInt(ddlSubject.SelectedValue)), _
                DBHelper.Param("@at", allowedTime), DBHelper.Param("@tq", totalQ), _
                DBHelper.Param("@rq", chkRandomize.Checked), DBHelper.Param("@so", chkShuffle.Checked), _
                DBHelper.Param("@rem", If(txtRemarks.Text.Trim()="", CObj(DBNull.Value), CObj(txtRemarks.Text.Trim()))), _
                DBHelper.Param("@rev", chkReview.Checked), DBHelper.Param("@by", CInt(Session("UserID"))), _
                DBHelper.Param("@nm", chkNegMarking.Checked), DBHelper.Param("@nmv", negMarks)))
            hfQuizID.Value = quizID.ToString()
        Else
            ' Update existing draft
            quizID = CInt(hfQuizID.Value)
            DBHelper.ExecuteNonQuery( _
                "UPDATE Quiz SET QuizTitle=@t, SubjectID=@s, AllowedTime=@at, TotalQuestions=@tq, RandomizeQ=@rq, ShuffleOptions=@so, Remarks=@rem, ReviewAnswer=@rev, NegativeMarking=@nm, NegativeMarks=@nmv WHERE QuizID=@q", _
                DBHelper.Param("@t", txtTitle.Text.Trim()), DBHelper.Param("@s", CInt(ddlSubject.SelectedValue)), _
                DBHelper.Param("@at", allowedTime), DBHelper.Param("@tq", totalQ), _
                DBHelper.Param("@rq", chkRandomize.Checked), DBHelper.Param("@so", chkShuffle.Checked), _
                DBHelper.Param("@rem", If(txtRemarks.Text.Trim()="", CObj(DBNull.Value), CObj(txtRemarks.Text.Trim()))), _
                DBHelper.Param("@rev", chkReview.Checked), DBHelper.Param("@nm", chkNegMarking.Checked), _
                DBHelper.Param("@nmv", negMarks), DBHelper.Param("@q", quizID))
        End If

        LoadCurrentQuestions()
        LoadBankQuestions()
        mvWizard.ActiveViewIndex = 1
    End Sub

    ' --- STEP 2 ---
    Protected Sub btnTabNew_Click(sender As Object, e As EventArgs)
        btnTabNew.CssClass = "btn btn-sm btn-primary"
        btnTabBank.CssClass = "btn btn-sm btn-outline"
        pnlWriteNew.Visible = True
        pnlSelectBank.Visible = False
    End Sub

    Protected Sub btnTabBank_Click(sender As Object, e As EventArgs)
        btnTabNew.CssClass = "btn btn-sm btn-outline"
        btnTabBank.CssClass = "btn btn-sm btn-primary"
        pnlWriteNew.Visible = False
        pnlSelectBank.Visible = True
    End Sub

    Protected Sub ddlQType_Changed(sender As Object, e As EventArgs)
        Dim qt = ddlQType.SelectedValue
        pnlOptions.Visible = (qt = "Radio" OrElse qt = "Checkbox")
        pnlParagraph.Visible = (qt = "Paragraph")
    End Sub

    Protected Sub btnSaveQ_Click(sender As Object, e As EventArgs)
        pnlError.Visible = False
        Dim qType = ddlQType.SelectedValue
        If String.IsNullOrWhiteSpace(txtQStmt.Text) Then ShowError("Question statement required.") : Return
        
        Dim cOption As String = "A"
        Dim cOptions As String = ""
        
        If qType = "Radio" Then
            If String.IsNullOrWhiteSpace(txtOptA.Text) OrElse String.IsNullOrWhiteSpace(txtOptB.Text) Then ShowError("At least Options A and B required.") : Return
            If cbAnsA.Checked Then
                cOption = "A"
            ElseIf cbAnsB.Checked Then
                cOption = "B"
            ElseIf cbAnsC.Checked Then
                cOption = "C"
            ElseIf cbAnsD.Checked Then
                cOption = "D"
            Else
                ShowError("Select correct option.") : Return
            End If
            cOptions = cOption
        ElseIf qType = "Checkbox" Then
            Dim sel As New System.Collections.Generic.List(Of String)
            If cbAnsA.Checked Then sel.Add("A")
            If cbAnsB.Checked Then sel.Add("B")
            If cbAnsC.Checked Then sel.Add("C")
            If cbAnsD.Checked Then sel.Add("D")
            If sel.Count = 0 Then ShowError("Select at least one correct option.") : Return
            sel.Sort()
            cOptions = String.Join(",", sel)
            cOption = sel(0)
        Else
            If String.IsNullOrWhiteSpace(txtModelAns.Text) Then ShowError("Model answer required.") : Return
            cOptions = txtModelAns.Text.Trim()
        End If

        Dim qid As Integer = CInt(DBHelper.ExecuteScalar( _
            "INSERT INTO QuestionsTable (SubjectID,QuestionStatement,OptionA,OptionB,OptionC,OptionD,CorrectOption,DifficultyLevel,CreatedBy,QuestionType,CorrectOptions) " & _
            "VALUES (@sid,@stmt,@a,@b,@c,@d,@co,@diff,@by,@qt,@cops); SELECT SCOPE_IDENTITY();", _
            DBHelper.Param("@sid", CInt(ddlSubject.SelectedValue)), DBHelper.Param("@stmt", txtQStmt.Text.Trim()), _
            DBHelper.Param("@a", If(qType="Paragraph","N/A",txtOptA.Text.Trim())), DBHelper.Param("@b", If(qType="Paragraph","N/A",txtOptB.Text.Trim())), _
            DBHelper.Param("@c", If(qType="Paragraph","N/A",txtOptC.Text.Trim())), DBHelper.Param("@d", If(qType="Paragraph","N/A",txtOptD.Text.Trim())), _
            DBHelper.Param("@co", cOption), DBHelper.Param("@diff", ddlQDiff.SelectedValue), _
            DBHelper.Param("@by", CInt(Session("UserID"))), DBHelper.Param("@qt", qType), DBHelper.Param("@cops", cOptions)))
            
        ' Add to quiz
        LinkQuestionToQuiz(qid)
        
        ' Reset
        txtQStmt.Text = "" : txtOptA.Text = "" : txtOptB.Text = "" : txtOptC.Text = "" : txtOptD.Text = "" : txtModelAns.Text = ""
        cbAnsA.Checked=False : cbAnsB.Checked=False : cbAnsC.Checked=False : cbAnsD.Checked=False
        LoadCurrentQuestions()
        LoadBankQuestions()
    End Sub

    Protected Sub btnAddSelected_Click(sender As Object, e As EventArgs)
        For Each row As System.Web.UI.WebControls.GridViewRow In gvBank.Rows
            Dim chk = CType(row.FindControl("chkSelect"), System.Web.UI.WebControls.CheckBox)
            If chk IsNot Nothing AndAlso chk.Checked Then
                Dim qid = CInt(gvBank.DataKeys(row.RowIndex).Value)
                LinkQuestionToQuiz(qid)
            End If
        Next
        LoadCurrentQuestions()
        LoadBankQuestions()
    End Sub

    Private Sub LinkQuestionToQuiz(qid As Integer)
        Dim quizID = CInt(hfQuizID.Value)
        Dim exists = DBHelper.Exists("SELECT COUNT(*) FROM QuizQuestions WHERE QuizID=@qz AND QuestionID=@q", DBHelper.Param("@qz", quizID), DBHelper.Param("@q", qid))
        If Not exists Then
            Dim maxOrder = CInt(If(DBHelper.ExecuteScalar("SELECT MAX(DisplayOrder) FROM QuizQuestions WHERE QuizID=@qz", DBHelper.Param("@qz", quizID)), 0))
            DBHelper.ExecuteNonQuery("INSERT INTO QuizQuestions (QuizID,QuestionID,DisplayOrder) VALUES (@qz,@q,@o)", _
                DBHelper.Param("@qz", quizID), DBHelper.Param("@q", qid), DBHelper.Param("@o", maxOrder + 1))
        End If
    End Sub

    Private Sub LoadBankQuestions()
        Dim dt = DBHelper.GetDataTable("SELECT QuestionID, QuestionStatement, DifficultyLevel FROM QuestionsTable WHERE SubjectID=@s AND CreatedBy=@by ORDER BY CreatedAt DESC", _
            DBHelper.Param("@s", CInt(ddlSubject.SelectedValue)), DBHelper.Param("@by", CInt(Session("UserID"))))
        gvBank.DataSource = dt
        gvBank.DataBind()
    End Sub

    Private Sub LoadCurrentQuestions()
        Dim dt = DBHelper.GetDataTable( _
            "SELECT qq.QuizQuestionID, q.QuestionStatement, q.QuestionType, q.DifficultyLevel " & _
            "FROM QuizQuestions qq JOIN QuestionsTable q ON qq.QuestionID = q.QuestionID " & _
            "WHERE qq.QuizID=@qz ORDER BY qq.DisplayOrder", DBHelper.Param("@qz", CInt(hfQuizID.Value)))
        rptCurrentQ.DataSource = dt
        rptCurrentQ.DataBind()
        litCurQCount.Text = dt.Rows.Count.ToString()
        pnlNoCurQ.Visible = (dt.Rows.Count = 0)
    End Sub

    Protected Sub rptCurrentQ_Command(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
        If e.CommandName = "Remove" Then
            DBHelper.ExecuteNonQuery("DELETE FROM QuizQuestions WHERE QuizQuestionID=@id", DBHelper.Param("@id", CInt(e.CommandArgument)))
            LoadCurrentQuestions()
        End If
    End Sub

    Protected Sub btnStep2Prev_Click(sender As Object, e As EventArgs)
        pnlError.Visible = False
        mvWizard.ActiveViewIndex = 0
    End Sub

    Protected Sub btnStep2Next_Click(sender As Object, e As EventArgs)
        pnlError.Visible = False
        If CInt(litCurQCount.Text) = 0 Then ShowError("You must add at least 1 question to the quiz before proceeding.") : Return
        
        litFinalTitle.Text = txtTitle.Text.Trim()
        litFinalCount.Text = litCurQCount.Text
        mvWizard.ActiveViewIndex = 2
    End Sub

    ' --- STEP 3 ---
    Protected Sub btnStep3Prev_Click(sender As Object, e As EventArgs)
        mvWizard.ActiveViewIndex = 1
    End Sub

    Protected Sub btnTestQuiz_Click(sender As Object, e As EventArgs)
        Response.Redirect("TestQuiz.aspx?quizid=" & hfQuizID.Value)
    End Sub

    Protected Sub btnPublish_Click(sender As Object, e As EventArgs)
        Dim quizID = CInt(hfQuizID.Value)
        DBHelper.ExecuteNonQuery("UPDATE Quiz SET IsPublished=1 WHERE QuizID=@q", DBHelper.Param("@q", quizID))
        
        Dim students = DBHelper.GetDataTable("SELECT UserID FROM Users WHERE Role='Student' AND IsActive=1")
        For Each row As DataRow In students.Rows
            DBHelper.ExecuteNonQuery( _
                "INSERT INTO Notifications(ToUserID,FromUserID,Message) VALUES(@to,@from,@msg)", _
                DBHelper.Param("@to", CInt(row("UserID"))), DBHelper.Param("@from", CInt(Session("UserID"))), _
                DBHelper.Param("@msg", "New quiz available: """ & txtTitle.Text.Trim() & """. Open your dashboard to attempt it."))
        Next
        
        Response.Redirect("Dashboard.aspx")
    End Sub

    Private Sub ShowError(msg As String)
        pnlError.Visible = True
        litError.Text = msg
    End Sub

End Class
