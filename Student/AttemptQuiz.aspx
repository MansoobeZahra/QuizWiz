<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AttemptQuiz.aspx.vb" Inherits="Student_AttemptQuiz" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Attempt Quiz</asp:Content>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    /* Override body BG for quiz immersion */
    .page-body { background: transparent; }
</style>
</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<!-- Already-attempted message -->
<asp:Panel ID="pnlAlreadyDone" runat="server" Visible="false">
    <div class="alert alert-warning">
        ⚠️ You have already attempted this quiz. <a href="MyResults.aspx" style="color:inherit;font-weight:700;">View your result →</a>
    </div>
</asp:Panel>

<!-- Quiz in progress -->
<asp:Panel ID="pnlQuiz" runat="server" Visible="false">
<div class="quiz-container">

    <!-- Header bar: title + timer -->
    <div class="quiz-header-bar">
        <div>
            <div class="quiz-title-text"><asp:Literal ID="litQuizTitle" runat="server" /></div>
            <div class="quiz-progress">Question <asp:Literal ID="litQNum" runat="server" /> of <asp:Literal ID="litQTotal" runat="server" /></div>
        </div>
        <div class="timer-box" id="timerBox">
            ⏱ <span id="timerDisplay">--:--</span>
        </div>
    </div>

    <!-- Progress bar -->
    <div class="progress-bar-outer">
        <div class="progress-bar-inner" id="progressBar" style="width:0%"></div>
    </div>

    <!-- Question Card -->
    <div class="question-card">
        <div class="question-meta">
            <span class="q-num">Q<asp:Literal ID="litQNumBadge" runat="server" /></span>
            <span class='badge diff-<asp:Literal ID="litDiffClass" runat="server" />'><asp:Literal ID="litDiffLabel" runat="server" /></span>
            <span class="text-muted" style="font-size:12px;"><asp:Literal ID="litSubject" runat="server" /></span>
        </div>

        <asp:Panel ID="pnlImage" runat="server" Visible="false">
            <img id="qImage" src="" alt="Question Image" class="question-image"
                 runat="server" />
        </asp:Panel>

        <div class="question-text"><asp:Literal ID="litQuestion" runat="server" /></div>

        <!-- Options – rendered by repeater so shuffle order works -->
        <ul class="options-list">
            <asp:Repeater ID="rptOptions" runat="server">
                <ItemTemplate>
                    <li class="option-item">
                        <input type="radio" name="studentAnswer"
                               id='opt_<%# Eval("Letter") %>'
                               value='<%# Eval("Letter") %>' />
                        <label for='opt_<%# Eval("Letter") %>' class="option-label">
                            <span class="opt-letter"><%# Eval("DisplayLabel") %></span>
                            <%# Eval("Text") %>
                        </label>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>

    <!-- Hidden fields -->
    <asp:HiddenField ID="hfAnswer"  runat="server" />
    <asp:HiddenField ID="hfQIndex" runat="server" />

    <!-- Navigation -->
    <div class="flex gap-2" style="justify-content:flex-end;">
        <asp:Button ID="btnNext"   runat="server" Text="Next Question →"
            CssClass="btn btn-primary btn-lg" OnClick="btnNext_Click" />
        <asp:Button ID="btnSubmit" runat="server" Text="✅ Submit Quiz"
            CssClass="btn btn-success btn-lg" Visible="false"
            OnClick="btnSubmit_Click"
            OnClientClick="return confirm('Submit the quiz now? You cannot go back.')" />
    </div>

    <!-- Timer hidden field -->
    <asp:HiddenField ID="hfTimeLeft" runat="server" />
    <asp:HiddenField ID="hfAutoSubmit" runat="server" Value="0" />

</div>
</asp:Panel>

<script>
(function () {
    var secsLeft = parseInt(document.getElementById('<%= hfTimeLeft.ClientID %>').value || '0', 10);
    var totalQ   = parseInt('<%= litQTotal.Text %>' || '1', 10);
    var currQ    = parseInt('<%= litQNumBadge.Text %>' || '1', 10);
    var timerDisp = document.getElementById('timerDisplay');
    var timerBox  = document.getElementById('timerBox');
    var progressBar = document.getElementById('progressBar');

    if (progressBar) {
        progressBar.style.width = Math.round(((currQ - 1) / totalQ) * 100) + '%';
    }

    if (!timerDisp || secsLeft <= 0) return;

    function pad(n) { return n < 10 ? '0' + n : n; }

    var interval = setInterval(function () {
        secsLeft--;
        var m = Math.floor(secsLeft / 60);
        var s = secsLeft % 60;
        timerDisp.textContent = pad(m) + ':' + pad(s);

        if (secsLeft <= 60 && timerBox) timerBox.classList.add('warning');

        if (secsLeft <= 0) {
            clearInterval(interval);
            // Auto-submit
            document.getElementById('<%= hfAutoSubmit.ClientID %>').value = '1';
            var submitBtn = document.getElementById('<%= btnSubmit.ClientID %>');
            var nextBtn   = document.getElementById('<%= btnNext.ClientID %>');
            if (submitBtn && submitBtn.style.display !== 'none') { submitBtn.click(); }
            else if (nextBtn) { nextBtn.click(); }
        }
    }, 1000);

    // Capture radio selection into hidden field before postback
    document.querySelectorAll('input[name="studentAnswer"]').forEach(function (rb) {
        rb.addEventListener('change', function () {
            document.getElementById('<%= hfAnswer.ClientID %>').value = this.value;
        });
    });
})();
</script>

</asp:Content>
