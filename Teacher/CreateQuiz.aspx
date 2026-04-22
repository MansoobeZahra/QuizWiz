<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CreateQuiz.aspx.vb" Inherits="Teacher_CreateQuiz" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Create Quiz</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
    <h1>Create and Publish Quiz</h1>
    <p>Configure quiz settings, select questions, then save as draft or publish directly.</p>
</div>

<asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success mb-4">
    <asp:Literal ID="litSuccess" runat="server" />
</asp:Panel>
<asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger mb-4">
    <asp:Literal ID="litError" runat="server" />
</asp:Panel>

<!-- Settings -->
<div class="form-panel">
    <h3>Quiz Settings</h3>
    <div class="form-row mb-4">
        <div class="form-group">
            <label>Quiz Title</label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="e.g. CS Mid-Term 2025" />
        </div>
        <div class="form-group">
            <label>Subject</label>
            <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control"
                AutoPostBack="true" OnSelectedIndexChanged="ddlSubject_Changed" />
        </div>
    </div>
    <div class="form-row mb-4">
        <div class="form-group">
            <label>Start Date and Time</label>
            <asp:TextBox ID="txtStart" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
        </div>
        <div class="form-group">
            <label>Allowed Time (minutes)</label>
            <asp:TextBox ID="txtTime" runat="server" CssClass="form-control" TextMode="Number" Text="30" />
        </div>
        <div class="form-group">
            <label>Number of Questions to Show</label>
            <asp:TextBox ID="txtTotalQ" runat="server" CssClass="form-control" TextMode="Number" Text="10" />
        </div>
    </div>
    <div class="form-group mb-4">
        <label>Remarks / Student Instructions</label>
        <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"
            placeholder="Optional message shown to students before the quiz..." />
    </div>

    <!-- Behaviour Options -->
    <div class="form-row mb-4">
        <div>
            <label>Quiz Behaviour</label>
            <div class="check-group" style="margin-top:10px;">
                <label class="check-item">
                    <asp:CheckBox ID="chkRandomize" runat="server" Checked="true" />
                    <span>Randomize question order</span>
                </label>
                <label class="check-item">
                    <asp:CheckBox ID="chkShuffle" runat="server" Checked="true" />
                    <span>Shuffle answer options</span>
                </label>
                <label class="check-item">
                    <asp:CheckBox ID="chkReview" runat="server" />
                    <span>Allow result review after submit</span>
                </label>
            </div>
        </div>

        <!-- Negative Marking -->
        <div>
            <label>Negative Marking</label>
            <div class="check-group" style="margin-top:10px;">
                <label class="check-item">
                    <asp:CheckBox ID="chkNegMarking" runat="server" />
                    <span>Enable negative marking for wrong answers</span>
                </label>
            </div>
            <div id="negMarkSection" style="display:none;margin-top:12px;">
                <label style="font-size:13px;color:var(--text-sec);font-weight:600;">
                    Marks deducted per wrong answer
                </label>
                <asp:TextBox ID="txtNegMarks" runat="server" CssClass="form-control"
                    TextMode="Number" Text="0.25"
                    style="max-width:160px;margin-top:6px;" />
                <div class="text-muted" style="font-size:12px;margin-top:4px;">
                    Typical values: 0.25 (1/4), 0.33 (1/3), 0.5 (1/2), 1 (full)
                </div>
            </div>
        </div>
    </div>
</div>

<!-- Question Picker -->
<div class="form-panel">
    <h3>Select Questions</h3>
    <p class="text-muted mb-4">Showing questions for the selected subject. Tick those to include in this quiz.</p>

    <asp:Panel ID="pnlNoQ" runat="server" CssClass="alert alert-info" Visible="false">
        No questions found for this subject.
        <a href="AddQuestion.aspx" style="color:inherit;font-weight:700;">Add questions first.</a>
    </asp:Panel>

    <ul class="question-picker">
        <asp:Repeater ID="rptQuestions" runat="server">
            <ItemTemplate>
                <li class="picker-item">
                    <input type="checkbox" name="selQ" value="<%# Eval("QuestionID") %>"
                           id="q<%# Eval("QuestionID") %>" />
                    <div>
                        <div class="q-text"><%# Eval("QuestionStatement") %></div>
                        <div class="q-meta" style="margin-top:4px;display:flex;gap:8px;align-items:center;">
                            <span class='badge diff-<%# Eval("DifficultyLevel").ToString().ToLower() %>'>
                                <%# Eval("DifficultyLevel") %>
                            </span>
                            <span class='badge badge-grey'><%# Eval("QuestionType") %></span>
                            Correct: <strong><%# If(Eval("QuestionType").ToString()="Paragraph","[text]",Eval("CorrectOptions")) %></strong>
                        </div>
                    </div>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>

    <asp:HiddenField ID="hfSelected" runat="server" />
</div>

<div class="flex gap-2">
    <asp:Button ID="btnSaveDraft" runat="server" Text="Save as Draft"
        CssClass="btn btn-outline btn-lg" OnClick="btnSave_Click" CommandArgument="draft" />
    <asp:Button ID="btnPublish" runat="server" Text="Save and Publish"
        CssClass="btn btn-primary btn-lg" OnClick="btnSave_Click" CommandArgument="publish" />
    <a href="Dashboard.aspx" class="btn btn-outline btn-lg">Cancel</a>
</div>

<script>
(function () {
    // Collect checkbox IDs into hidden field before submit
    document.querySelectorAll('input[type=submit],input[type=button],[type=submit]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var ids = [];
            document.querySelectorAll('input[name="selQ"]:checked').forEach(function (cb) { ids.push(cb.value); });
            document.getElementById('<%= hfSelected.ClientID %>').value = ids.join(',');
        });
    });

    // Re-check saved on postback
    var saved = document.getElementById('<%= hfSelected.ClientID %>').value;
    if (saved) {
        saved.split(',').forEach(function (id) {
            var el = document.getElementById('q' + id);
            if (el) el.checked = true;
        });
    }

    // Negative marking toggle
    var chkNeg = document.getElementById('<%= chkNegMarking.ClientID %>');
    var negSec = document.getElementById('negMarkSection');
    function toggleNeg() { negSec.style.display = chkNeg.checked ? '' : 'none'; }
    chkNeg.addEventListener('change', toggleNeg);
    toggleNeg();
})();
</script>

</asp:Content>
