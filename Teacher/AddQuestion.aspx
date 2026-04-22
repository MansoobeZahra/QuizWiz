<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddQuestion.aspx.vb" Inherits="Teacher_AddQuestion" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Add Question</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
    <h1>Add New Question</h1>
    <p>Select the question type first, then fill in the details.</p>
</div>

<asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success mb-4">
    Question saved. <a href="AddQuestion.aspx" style="color:inherit;font-weight:700;">Add another</a>
    &nbsp;|&nbsp; <a href="ManageQuestions.aspx" style="color:inherit;font-weight:700;">View all questions</a>
</asp:Panel>
<asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger mb-4">
    <asp:Literal ID="litError" runat="server" />
</asp:Panel>

<!-- Question Type -->
<div class="form-panel mb-4">
    <h3>Question Type</h3>
    <div class="qtype-selector">
        <label class="qtype-item">
            <asp:RadioButton ID="rbSingle" runat="server" GroupName="qType" Checked="true" />&nbsp;
            <span class="question-type-badge badge-purple">Single Choice</span>
            <span class="text-muted" style="font-size:12px;">One correct answer (radio button)</span>
        </label>
        <label class="qtype-item">
            <asp:RadioButton ID="rbMultiple" runat="server" GroupName="qType" />&nbsp;
            <span class="question-type-badge badge-cyan">Multiple Choice</span>
            <span class="text-muted" style="font-size:12px;">One or more correct answers (checkboxes)</span>
        </label>
        <label class="qtype-item">
            <asp:RadioButton ID="rbParagraph" runat="server" GroupName="qType" />&nbsp;
            <span class="question-type-badge badge-orange">Short Answer</span>
            <span class="text-muted" style="font-size:12px;">Student types a text answer</span>
        </label>
    </div>
    <asp:HiddenField ID="hfQType" runat="server" Value="Radio" />
</div>

<!-- Question Details -->
<div class="form-panel">
    <h3>Question Details</h3>
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
        <asp:TextBox ID="txtStatement" runat="server" TextMode="MultiLine" Rows="3"
            CssClass="form-control" placeholder="Type the question here..." />
    </div>
    <div class="form-group mb-4">
        <label>Question Image (optional — JPG/PNG, max 2 MB)</label>
        <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" />
    </div>
</div>

<!-- Options A-D (hidden for Short Answer) -->
<div class="form-panel" id="secOptions">
    <h3>Answer Options</h3>
    <div class="form-group mb-4">
        <label>Option A</label>
        <asp:TextBox ID="txtA" runat="server" CssClass="form-control" placeholder="Option A..." />
    </div>
    <div class="form-group mb-4">
        <label>Option B</label>
        <asp:TextBox ID="txtB" runat="server" CssClass="form-control" placeholder="Option B..." />
    </div>
    <div class="form-group mb-4">
        <label>Option C</label>
        <asp:TextBox ID="txtC" runat="server" CssClass="form-control" placeholder="Option C..." />
    </div>
    <div class="form-group">
        <label>Option D</label>
        <asp:TextBox ID="txtD" runat="server" CssClass="form-control" placeholder="Option D..." />
    </div>
</div>

<!-- Correct Answer — Single Choice (radio) -->
<div class="form-panel" id="secCorrectRadio">
    <h3>Correct Answer — Single Choice</h3>
    <p class="text-muted mb-4">Select the ONE correct option.</p>
    <div style="display:grid;grid-template-columns:auto 1fr;align-items:center;gap:12px 16px;">
        <label class="check-item"><asp:RadioButton ID="rbA" runat="server" GroupName="Correct" /><span class="badge badge-purple">A</span></label>
        <span class="text-sec" style="font-size:13px;">Option A (as entered above)</span>
        <label class="check-item"><asp:RadioButton ID="rbB" runat="server" GroupName="Correct" /><span class="badge badge-cyan">B</span></label>
        <span class="text-sec" style="font-size:13px;">Option B</span>
        <label class="check-item"><asp:RadioButton ID="rbC" runat="server" GroupName="Correct" /><span class="badge badge-orange">C</span></label>
        <span class="text-sec" style="font-size:13px;">Option C</span>
        <label class="check-item"><asp:RadioButton ID="rbD" runat="server" GroupName="Correct" /><span class="badge badge-red">D</span></label>
        <span class="text-sec" style="font-size:13px;">Option D</span>
    </div>
</div>

<!-- Correct Answer — Multiple Choice (checkboxes) -->
<div class="form-panel" id="secCorrectCheckbox" style="display:none;">
    <h3>Correct Answers — Multiple Choice</h3>
    <p class="text-muted mb-4">Tick ALL options that are correct. Students must select exactly these to get full marks.</p>
    <div class="check-group">
        <label class="check-item"><asp:CheckBox ID="cbCA" runat="server" />&nbsp;<span class="badge badge-purple">A</span></label>
        <label class="check-item"><asp:CheckBox ID="cbCB" runat="server" />&nbsp;<span class="badge badge-cyan">B</span></label>
        <label class="check-item"><asp:CheckBox ID="cbCC" runat="server" />&nbsp;<span class="badge badge-orange">C</span></label>
        <label class="check-item"><asp:CheckBox ID="cbCD" runat="server" />&nbsp;<span class="badge badge-red">D</span></label>
    </div>
</div>

<!-- Correct Answer — Short Answer (paragraph) -->
<div class="form-panel" id="secCorrectParagraph" style="display:none;">
    <h3>Model Answer — Short Answer</h3>
    <p class="text-muted mb-4">Enter the expected answer. Student responses will be matched (case-insensitive).</p>
    <asp:TextBox ID="txtModelAnswer" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"
        placeholder="Expected answer text..." />
</div>

<div class="flex gap-2">
    <asp:Button ID="btnSave" runat="server" Text="Save Question"
        CssClass="btn btn-primary btn-lg" OnClick="btnSave_Click" />
    <a href="ManageQuestions.aspx" class="btn btn-outline btn-lg">Cancel</a>
</div>

<script>
(function () {
    var hfQType = document.getElementById('<%= hfQType.ClientID %>');
    var rbSingle   = document.getElementById('<%= rbSingle.ClientID %>');
    var rbMultiple = document.getElementById('<%= rbMultiple.ClientID %>');
    var rbParagraph= document.getElementById('<%= rbParagraph.ClientID %>');

    var secOptions         = document.getElementById('secOptions');
    var secCorrectRadio    = document.getElementById('secCorrectRadio');
    var secCorrectCheckbox = document.getElementById('secCorrectCheckbox');
    var secCorrectParagraph= document.getElementById('secCorrectParagraph');

    function applyType(type) {
        hfQType.value = type;
        secOptions.style.display          = (type === 'Paragraph') ? 'none' : '';
        secCorrectRadio.style.display     = (type === 'Radio')     ? ''     : 'none';
        secCorrectCheckbox.style.display  = (type === 'Checkbox')  ? ''     : 'none';
        secCorrectParagraph.style.display = (type === 'Paragraph') ? ''     : 'none';
    }

    rbSingle.addEventListener('change',    function () { applyType('Radio'); });
    rbMultiple.addEventListener('change',  function () { applyType('Checkbox'); });
    rbParagraph.addEventListener('change', function () { applyType('Paragraph'); });

    // Re-apply on page load (in case of postback keeping state)
    var saved = hfQType.value || 'Radio';
    applyType(saved);
})();
</script>

</asp:Content>
