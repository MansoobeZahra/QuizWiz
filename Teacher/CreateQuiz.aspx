<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CreateQuiz.aspx.vb" Inherits="Teacher_CreateQuiz" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Create Quiz</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
    <h1>🎲 Create &amp; Publish Quiz</h1>
    <p>Configure quiz settings, pick questions, then save or publish.</p>
</div>

<asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success mb-4">
    ✅ <asp:Literal ID="litSuccess" runat="server" />
</asp:Panel>
<asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger mb-4">
    ⚠️ <asp:Literal ID="litError" runat="server" />
</asp:Panel>

<!-- Quiz Settings -->
<div class="form-panel">
    <h3>⚙️ Quiz Settings</h3>
    <div class="form-row mb-4">
        <div class="form-group">
            <label>Quiz Title</label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="e.g. CS Mid-Term Quiz" />
        </div>
        <div class="form-group">
            <label>Subject</label>
            <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control"
                AutoPostBack="true" OnSelectedIndexChanged="ddlSubject_Changed" />
        </div>
    </div>
    <div class="form-row mb-4">
        <div class="form-group">
            <label>Start Date &amp; Time</label>
            <asp:TextBox ID="txtStart" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
        </div>
        <div class="form-group">
            <label>Allowed Time (minutes)</label>
            <asp:TextBox ID="txtTime" runat="server" CssClass="form-control" TextMode="Number" Text="30" />
        </div>
        <div class="form-group">
            <label>Number of Questions</label>
            <asp:TextBox ID="txtTotalQ" runat="server" CssClass="form-control" TextMode="Number" Text="10" />
        </div>
    </div>
    <div class="form-group mb-4">
        <label>Remarks / Instructions</label>
        <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"
            placeholder="Optional instructions shown to students…" />
    </div>
    <div class="form-group">
        <label>Options</label>
        <div class="check-group">
            <label class="check-item">
                <asp:CheckBox ID="chkRandomize" runat="server" Checked="true" />
                <span>🔀 Randomize question order</span>
            </label>
            <label class="check-item">
                <asp:CheckBox ID="chkShuffle" runat="server" Checked="true" />
                <span>🔄 Shuffle answer options</span>
            </label>
            <label class="check-item">
                <asp:CheckBox ID="chkReview" runat="server" />
                <span>👁️ Allow result review after submit</span>
            </label>
        </div>
    </div>
</div>

<!-- Question Picker -->
<div class="form-panel">
    <h3>❓ Select Questions</h3>
    <div class="text-muted mb-4">Only questions for the selected subject are shown. Tick the ones to include.</div>

    <asp:Panel ID="pnlNoQ" runat="server" CssClass="alert alert-info" Visible="false">
        ℹ️ No questions found for this subject. <a href="AddQuestion.aspx" style="color:inherit;font-weight:700;">Add questions first.</a>
    </asp:Panel>

    <ul class="question-picker" id="qPicker">
        <asp:Repeater ID="rptQuestions" runat="server">
            <ItemTemplate>
                <li class="picker-item">
                    <input type="checkbox" name="selQ" value="<%# Eval("QuestionID") %>"
                           id="q<%# Eval("QuestionID") %>" />
                    <div>
                        <div class="q-text"><%# Eval("QuestionStatement") %></div>
                        <div class="q-meta">
                            <span class='badge diff-<%# Eval("DifficultyLevel").ToString().ToLower() %>'><%# Eval("DifficultyLevel") %></span>
                            &nbsp; Correct: <strong><%# Eval("CorrectOption") %></strong>
                        </div>
                    </div>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>

    <!-- Hidden field to carry selected IDs -->
    <asp:HiddenField ID="hfSelected" runat="server" />
</div>

<div class="flex gap-2">
    <asp:Button ID="btnSaveDraft" runat="server" Text="💾 Save as Draft"
        CssClass="btn btn-outline btn-lg" OnClick="btnSave_Click"
        CommandArgument="draft" />
    <asp:Button ID="btnPublish" runat="server" Text="🚀 Save &amp; Publish"
        CssClass="btn btn-primary btn-lg" OnClick="btnSave_Click"
        CommandArgument="publish" />
    <a href="Dashboard.aspx" class="btn btn-outline btn-lg">Cancel</a>
</div>

<script>
    // Collect checked question IDs into hidden field before postback
    document.querySelectorAll('input[type=submit],input[type=button]').forEach(function(btn){
        btn.addEventListener('click', function(){
            var ids = [];
            document.querySelectorAll('input[name="selQ"]:checked').forEach(function(cb){ ids.push(cb.value); });
            document.getElementById('<%= hfSelected.ClientID %>').value = ids.join(',');
        });
    });
    // Also on page-enter so selections survive postback (re-check)
    var savedIds = document.getElementById('<%= hfSelected.ClientID %>').value;
    if(savedIds){
        savedIds.split(',').forEach(function(id){
            var el = document.getElementById('q'+id);
            if(el) el.checked = true;
        });
    }
</script>

</asp:Content>
