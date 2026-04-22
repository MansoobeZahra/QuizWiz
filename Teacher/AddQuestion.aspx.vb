Imports System.IO

Partial Class Teacher_AddQuestion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AuthHelper.RequireRole(Me, "Teacher")
        If Not IsPostBack Then LoadSubjects()
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

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        pnlError.Visible   = False
        pnlSuccess.Visible = False

        ' Determine question type from hidden field (JS keeps it updated)
        Dim qType As String = hfQType.Value
        If qType = "" Then qType = If(rbMultiple.Checked, "Checkbox", If(rbParagraph.Checked, "Paragraph", "Radio"))

        ' Common validation
        If String.IsNullOrWhiteSpace(txtStatement.Text) Then ShowError("Question statement is required.") : Return

        ' Type-specific validation
        Dim correctOptions As String = ""
        Dim correctOption  As String = "A"  ' fallback for Radio

        If qType = "Radio" Then
            If String.IsNullOrWhiteSpace(txtA.Text) OrElse String.IsNullOrWhiteSpace(txtB.Text) OrElse _
               String.IsNullOrWhiteSpace(txtC.Text) OrElse String.IsNullOrWhiteSpace(txtD.Text) Then
                ShowError("All four options are required for Single Choice.") : Return
            End If
            If rbA.Checked Then
                correctOption = "A"
            ElseIf rbB.Checked Then
                correctOption = "B"
            ElseIf rbC.Checked Then
                correctOption = "C"
            ElseIf rbD.Checked Then
                correctOption = "D"
            Else
                ShowError("Select the correct option.") : Return
            End If
            correctOptions = correctOption

        ElseIf qType = "Checkbox" Then
            If String.IsNullOrWhiteSpace(txtA.Text) OrElse String.IsNullOrWhiteSpace(txtB.Text) OrElse _
               String.IsNullOrWhiteSpace(txtC.Text) OrElse String.IsNullOrWhiteSpace(txtD.Text) Then
                ShowError("All four options are required for Multiple Choice.") : Return
            End If
            Dim sel As New List(Of String)
            If cbCA.Checked Then sel.Add("A")
            If cbCB.Checked Then sel.Add("B")
            If cbCC.Checked Then sel.Add("C")
            If cbCD.Checked Then sel.Add("D")
            If sel.Count = 0 Then ShowError("Select at least one correct option for Multiple Choice.") : Return
            sel.Sort()
            correctOptions = String.Join(",", sel)
            correctOption  = sel(0)  ' first letter as legacy field

        ElseIf qType = "Paragraph" Then
            If String.IsNullOrWhiteSpace(txtModelAnswer.Text) Then
                ShowError("A model (correct) answer is required for Short Answer questions.") : Return
            End If
            correctOptions = txtModelAnswer.Text.Trim()
            correctOption  = "A"  ' placeholder — not used for scoring paragraph questions
        End If

        ' Image upload
        Dim imagePath As String = Nothing
        If fuImage.HasFile Then
            Dim ext = Path.GetExtension(fuImage.FileName).ToLower()
            If ext <> ".jpg" AndAlso ext <> ".jpeg" AndAlso ext <> ".png" AndAlso ext <> ".gif" Then
                ShowError("Only JPG, PNG or GIF images are allowed.") : Return
            End If
            If fuImage.FileBytes.Length > 2 * 1024 * 1024 Then ShowError("Image must be under 2 MB.") : Return
            Dim folder = Server.MapPath("~/Images/Questions/")
            If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)
            Dim fileName = Guid.NewGuid().ToString("N") & ext
            fuImage.SaveAs(Path.Combine(folder, fileName))
            imagePath = "~/Images/Questions/" & fileName
        End If

        ' For paragraph questions the options columns are not meaningful — store blanks
        Dim optA = If(qType = "Paragraph", "N/A", txtA.Text.Trim())
        Dim optB = If(qType = "Paragraph", "N/A", txtB.Text.Trim())
        Dim optC = If(qType = "Paragraph", "N/A", txtC.Text.Trim())
        Dim optD = If(qType = "Paragraph", "N/A", txtD.Text.Trim())

        Try
            DBHelper.ExecuteNonQuery( _
                "INSERT INTO QuestionsTable " & _
                "    (SubjectID,QuestionStatement,OptionA,OptionB,OptionC,OptionD, " & _
                "     CorrectOption,DifficultyLevel,ImagePath,CreatedBy,QuestionType,CorrectOptions) " & _
                "VALUES (@sid,@stmt,@a,@b,@c,@d,@co,@diff,@img,@by,@qt,@cops)", _
                DBHelper.Param("@sid",  CInt(ddlSubject.SelectedValue)), _
                DBHelper.Param("@stmt", txtStatement.Text.Trim()), _
                DBHelper.Param("@a",    optA), _
                DBHelper.Param("@b",    optB), _
                DBHelper.Param("@c",    optC), _
                DBHelper.Param("@d",    optD), _
                DBHelper.Param("@co",   correctOption), _
                DBHelper.Param("@diff", ddlDifficulty.SelectedValue), _
                DBHelper.Param("@img",  If(imagePath Is Nothing, CObj(DBNull.Value), CObj(imagePath))), _
                DBHelper.Param("@by",   CInt(Session("UserID"))), _
                DBHelper.Param("@qt",   qType), _
                DBHelper.Param("@cops", correctOptions))

            ' Reset form
            txtStatement.Text = "" : txtA.Text = "" : txtB.Text = "" : txtC.Text = "" : txtD.Text = ""
            txtModelAnswer.Text = ""
            rbA.Checked = False : rbB.Checked = False : rbC.Checked = False : rbD.Checked = False
            cbCA.Checked = False : cbCB.Checked = False : cbCC.Checked = False : cbCD.Checked = False
            rbSingle.Checked = True : rbMultiple.Checked = False : rbParagraph.Checked = False
            hfQType.Value = "Radio"
            pnlSuccess.Visible = True

        Catch ex As Exception
            ShowError("Database error: " & ex.Message)
        End Try
    End Sub

    Private Sub ShowError(msg As String)
        pnlError.Visible = True
        litError.Text    = msg
    End Sub

End Class
