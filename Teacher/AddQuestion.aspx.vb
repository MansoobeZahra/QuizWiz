Imports System.IO

Partial Class Teacher_AddQuestion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Role")?.ToString() <> "Teacher" Then Response.Redirect("~/Login.aspx") : Return
        If Not IsPostBack Then LoadSubjects()
    End Sub

    Private Sub LoadSubjects()
        Dim dt = DBHelper.GetDataTable("SELECT SubjectID, SubjectName FROM Subjects ORDER BY SubjectName")
        ddlSubject.DataSource     = dt
        ddlSubject.DataTextField  = "SubjectName"
        ddlSubject.DataValueField = "SubjectID"
        ddlSubject.DataBind()

        ' Pre-select teacher's own subject if available
        Dim teacherSubject = DBHelper.ExecuteScalar(
            "SELECT SubjectID FROM Users WHERE UserID=@id AND SubjectID IS NOT NULL",
            DBHelper.Param("@id", CInt(Session("UserID"))))
        If teacherSubject IsNot Nothing AndAlso teacherSubject IsNot DBNull.Value Then
            Dim item = ddlSubject.Items.FindByValue(teacherSubject.ToString())
            If item IsNot Nothing Then item.Selected = True
        End If
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        pnlError.Visible   = False
        pnlSuccess.Visible = False

        ' Validate
        If String.IsNullOrWhiteSpace(txtStatement.Text) Then ShowError("Question statement is required.") : Return
        If String.IsNullOrWhiteSpace(txtA.Text) OrElse String.IsNullOrWhiteSpace(txtB.Text) OrElse
           String.IsNullOrWhiteSpace(txtC.Text) OrElse String.IsNullOrWhiteSpace(txtD.Text) Then
            ShowError("All four option fields are required.") : Return
        End If

        Dim correct As String = ""
        If rbA.Checked Then correct = "A"
        If rbB.Checked Then correct = "B"
        If rbC.Checked Then correct = "C"
        If rbD.Checked Then correct = "D"
        If correct = "" Then ShowError("Please select the correct option.") : Return

        ' Handle optional image upload
        Dim imagePath As String = Nothing
        If fuImage.HasFile Then
            Dim ext = Path.GetExtension(fuImage.FileName).ToLower()
            If ext <> ".jpg" AndAlso ext <> ".jpeg" AndAlso ext <> ".png" AndAlso ext <> ".gif" Then
                ShowError("Only JPG, PNG, or GIF images are allowed.") : Return
            End If
            If fuImage.FileBytes.Length > 2 * 1024 * 1024 Then
                ShowError("Image must be under 2 MB.") : Return
            End If
            Dim folderPath = Server.MapPath("~/Images/Questions/")
            If Not Directory.Exists(folderPath) Then Directory.CreateDirectory(folderPath)
            Dim fileName = Guid.NewGuid().ToString("N") & ext
            fuImage.SaveAs(Path.Combine(folderPath, fileName))
            imagePath = "~/Images/Questions/" & fileName
        End If

        Try
            Dim sql As String = "
                INSERT INTO QuestionsTable
                    (SubjectID, QuestionStatement, OptionA, OptionB, OptionC, OptionD,
                     CorrectOption, DifficultyLevel, ImagePath, CreatedBy)
                VALUES (@sid, @stmt, @a, @b, @c, @d, @correct, @diff, @img, @by)"

            DBHelper.ExecuteNonQuery(sql,
                DBHelper.Param("@sid",   CInt(ddlSubject.SelectedValue)),
                DBHelper.Param("@stmt",  txtStatement.Text.Trim()),
                DBHelper.Param("@a",     txtA.Text.Trim()),
                DBHelper.Param("@b",     txtB.Text.Trim()),
                DBHelper.Param("@c",     txtC.Text.Trim()),
                DBHelper.Param("@d",     txtD.Text.Trim()),
                DBHelper.Param("@correct", correct),
                DBHelper.Param("@diff",  ddlDifficulty.SelectedValue),
                DBHelper.Param("@img",   If(imagePath Is Nothing, DBNull.Value, CObj(imagePath))),
                DBHelper.Param("@by",    CInt(Session("UserID"))))

            ' Clear form
            txtStatement.Text = ""
            txtA.Text = "" : txtB.Text = "" : txtC.Text = "" : txtD.Text = ""
            rbA.Checked = False : rbB.Checked = False : rbC.Checked = False : rbD.Checked = False
            pnlSuccess.Visible = True

        Catch ex As Exception
            ShowError("Error saving question: " & ex.Message)
        End Try
    End Sub

    Private Sub ShowError(msg As String)
        pnlError.Visible = True
        litError.Text    = msg
    End Sub

End Class
