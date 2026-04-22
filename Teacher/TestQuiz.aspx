<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TestQuiz.aspx.vb" Inherits="Teacher_TestQuiz" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Test / Preview Quiz</asp:Content>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server">
<style>
.preview-banner {
    background: linear-gradient(135deg,#f7971e,#ffd200);
    color:#1a1a1a;
    text-align:center;
    padding:12px 20px;
    font-weight:800;
    font-size:13px;
    letter-spacing:1.2px;
    text-transform:uppercase;
    border-radius:var(--radius-sm);
    margin-bottom:24px;
}
.preview-nav { display:flex; gap:12px; justify-content:space-between; }
</style>
</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
    <h1>Test / Preview Quiz</h1>
    <p>Preview your quiz exactly as students see it. No answers are saved.</p>
</div>

<!-- Quiz selector -->
<asp:Panel ID="pnlSelect" runat="server">
    <div class="form-panel mb-6" style="padding:20px 24px;">
        <h3>Select a Quiz to Preview</h3>
        <div class="form-row">
            <div class="form-group">
                <label>Your Quiz</label>
                <asp:DropDownList ID="ddlQuiz" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group" style="display:flex;align-items:flex-end;">
                <asp:Button ID="btnStart" runat="server" Text="Start Preview"
                    CssClass="btn btn-warning btn-lg" OnClick="btnStart_Click" />
            </div>
        </div>
        <asp:Panel ID="pnlSelectMsg" runat="server" Visible="false" CssClass="alert alert-danger mt-4">
            <asp:Literal ID="litSelectErr" runat="server" />
        </asp:Panel>
    </div>
</asp:Panel>

<!-- Preview in progress -->
<asp:Panel ID="pnlPreview" runat="server" Visible="false">

    <div class="preview-banner">
        PREVIEW MODE | No answers are saved. This is for teacher review only.
    </div>

    <div class="quiz-container">

        <div class="quiz-header-bar">
            <div>
                <div class="quiz-title-text"><asp:Literal ID="litQuizTitle" runat="server" /></div>
                <div class="quiz-progress">
                    Question <asp:Literal ID="litQNum" runat="server" /> of
                    <asp:Literal ID="litQTotal" runat="server" />
                </div>
            </div>
            <asp:Button ID="btnExitPreview" runat="server" Text="Exit Preview"
                CssClass="btn btn-danger btn-sm" OnClick="btnExitPreview_Click"
                CausesValidation="false" />
        </div>

        <div class="progress-bar-outer">
            <div class="progress-bar-inner" id="progressBar" style="width:0%"></div>
        </div>

        <div class="question-card">
            <div class="question-meta">
                <span class="q-num">Q <asp:Literal ID="litQBadge" runat="server" /></span>
                <span class='badge diff-<asp:Literal ID="litDiffClass" runat="server" />'>
                    <asp:Literal ID="litDiff" runat="server" />
                </span>
                <span class='badge badge-grey'>
                    <asp:Literal ID="litQType" runat="server" />
                </span>
                <span class="text-muted" style="font-size:12px;"><asp:Literal ID="litSubject" runat="server" /></span>
            </div>

            <asp:Panel ID="pnlImg" runat="server" Visible="false">
                <img runat="server" id="qImg" src="" alt="Question image" class="question-image" />
            </asp:Panel>

            <div class="question-text"><asp:Literal ID="litQuestion" runat="server" /></div>

            <!-- Options (radio preview — no interaction stored) -->
            <asp:Panel ID="pnlOptionsPreview" runat="server">
                <ul class="options-list">
                    <asp:Repeater ID="rptPreviewOpts" runat="server">
                        <ItemTemplate>
                            <li class="option-item">
                                <label class='option-label <%# If(CBool(Eval("IsCorrect")), "opt-correct", "") %>'>
                                    <span class="opt-letter"><%# Eval("DisplayLabel") %></span>
                                    <%# Eval("Text") %>
                                    <%# If(CBool(Eval("IsCorrect")), "<strong style='margin-left:8px;color:var(--green-light);font-size:11px;'>[CORRECT]</strong>", "") %>
                                </label>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </asp:Panel>

            <!-- Short answer preview -->
            <asp:Panel ID="pnlParaPreview" runat="server" Visible="false">
                <div style="background:rgba(255,255,255,.04);border:1px dashed var(--border);border-radius:var(--radius-sm);padding:16px;font-size:13px;color:var(--text-sec);">
                    Student types a short text answer here.
                </div>
                <div style="margin-top:12px;padding:12px 16px;background:rgba(86,171,47,.1);border:1px solid rgba(86,171,47,.3);border-radius:var(--radius-sm);">
                    <span style="font-size:12px;font-weight:700;color:var(--green-light);">MODEL ANSWER:</span>
                    <div style="margin-top:6px;font-size:14px;"><asp:Literal ID="litModelAnswer" runat="server" /></div>
                </div>
            </asp:Panel>
        </div>

        <asp:HiddenField ID="hfQIdx" runat="server" Value="0" />

        <div class="preview-nav">
            <asp:Button ID="btnPrev" runat="server" Text="Previous"
                CssClass="btn btn-outline" OnClick="btnPrev_Click"
                CausesValidation="false" />
            <asp:Button ID="btnNextPreview" runat="server" Text="Next Question"
                CssClass="btn btn-primary" OnClick="btnNextPreview_Click" />
            <asp:Button ID="btnFinish" runat="server" Text="Finish Preview"
                CssClass="btn btn-success" Visible="false"
                OnClick="btnExitPreview_Click" CausesValidation="false" />
        </div>
    </div>
</asp:Panel>

<script>
(function () {
    var pb = document.getElementById('progressBar');
    var total = parseInt('<%= IIf(Session("TQ_Total") Is Nothing, 1, Session("TQ_Total")) %>', 10);
    var curr  = parseInt('<%= IIf(Session("TQ_Idx") Is Nothing, 0, Session("TQ_Idx")) %>', 10);
    if (pb && total > 0) pb.style.width = Math.round((curr / total) * 100) + '%';
})();
</script>

</asp:Content>
