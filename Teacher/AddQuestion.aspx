<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddQuestion.aspx.vb" Inherits="Teacher_AddQuestion" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Add Question</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
    <h1>➕ Add New Question</h1>
    <p>Enter a question for your subject. Fill all fields then click Save.</p>
</div>

<asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success mb-4">
    ✅ Question saved successfully! <a href="AddQuestion.aspx" style="color:inherit;font-weight:700;">Add another</a> or <a href="ManageQuestions.aspx" style="color:inherit;font-weight:700;">view all questions</a>.
</asp:Panel>
<asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger mb-4">
    ⚠️ <asp:Literal ID="litError" runat="server" />
</asp:Panel>

<div class="form-panel">
    <h3>📝 Question Details</h3>

    <div class="form-row mb-4">
        <div class="form-group">
            <label>Subject</label>
            <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group">
            <label>Difficulty Level</label>
            <asp:DropDownList ID="ddlDifficulty" runat="server" CssClass="form-control">
                <asp:ListItem Value="Easy">Easy</asp:ListItem>
                <asp:ListItem Value="Medium" Selected="True">Medium</asp:ListItem>
                <asp:ListItem Value="Hard">Hard</asp:ListItem>
                <asp:ListItem Value="Expert">Expert</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <div class="form-group mb-4">
        <label>Question Statement</label>
        <asp:TextBox ID="txtStatement" runat="server" TextMode="MultiLine" Rows="4"
            CssClass="form-control" placeholder="Type the full question here…" />
    </div>

    <div class="form-group mb-4">
        <label>Question Image (optional)</label>
        <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" />
        <div class="text-muted" style="margin-top:6px;">Accepted: JPG, PNG, GIF (max 2 MB)</div>
    </div>
</div>

<div class="form-panel">
    <h3>🔤 Answer Options &amp; Correct Answer</h3>

    <div style="display:grid;grid-template-columns:auto 1fr;align-items:center;gap:12px 16px;">
        <!-- Option A -->
        <label class="check-item">
            <asp:RadioButton ID="rbA" runat="server" GroupName="Correct" /><span class="badge badge-purple">A</span>
        </label>
        <div class="form-group" style="margin-bottom:0">
            <asp:TextBox ID="txtA" runat="server" CssClass="form-control" placeholder="Option A text…" />
        </div>
        <!-- Option B -->
        <label class="check-item">
            <asp:RadioButton ID="rbB" runat="server" GroupName="Correct" /><span class="badge badge-cyan">B</span>
        </label>
        <div class="form-group" style="margin-bottom:0">
            <asp:TextBox ID="txtB" runat="server" CssClass="form-control" placeholder="Option B text…" />
        </div>
        <!-- Option C -->
        <label class="check-item">
            <asp:RadioButton ID="rbC" runat="server" GroupName="Correct" /><span class="badge badge-orange">C</span>
        </label>
        <div class="form-group" style="margin-bottom:0">
            <asp:TextBox ID="txtC" runat="server" CssClass="form-control" placeholder="Option C text…" />
        </div>
        <!-- Option D -->
        <label class="check-item">
            <asp:RadioButton ID="rbD" runat="server" GroupName="Correct" /><span class="badge badge-red">D</span>
        </label>
        <div class="form-group" style="margin-bottom:0">
            <asp:TextBox ID="txtD" runat="server" CssClass="form-control" placeholder="Option D text…" />
        </div>
    </div>

    <div class="text-muted mt-4" style="font-size:13px;">
        ☝️ Select the radio button next to the <strong>correct</strong> option.
    </div>
</div>

<div class="flex gap-2">
    <asp:Button ID="btnSave" runat="server" Text="💾 Save Question"
        CssClass="btn btn-primary btn-lg" OnClick="btnSave_Click" />
    <a href="ManageQuestions.aspx" class="btn btn-outline btn-lg">Cancel</a>
</div>

</asp:Content>
