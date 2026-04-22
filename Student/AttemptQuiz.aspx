<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AttemptQuiz.aspx.vb" Inherits="Student_AttemptQuiz" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Attempt Quiz</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<asp:Panel ID="pnlAlreadyDone" runat="server" Visible="false">
    <div class="alert alert-warning">
        You have already attempted this quiz.
        <a href="MyResults.aspx" style="color:inherit;font-weight:700;">View your result</a>
    </div>
</asp:Panel>

<asp:Panel ID="pnlQuiz" runat="server" Visible="false">
<div class="quiz-container">

    <!-- Header -->
    <div class="quiz-header-bar">
        <div>
            <div class="quiz-title-text"><asp:Literal ID="litQuizTitle" runat="server" /></div>
            <div class="quiz-progress">
                Question <asp:Literal ID="litQNum" runat="server" />
                of <asp:Literal ID="litQTotal" runat="server" />
                <asp:Panel ID="pnlNegBadge" runat="server" Visible="false"
                    style="display:inline-flex;margin-left:12px;">
                    <span class="badge badge-red" style="font-size:11px;">
                        Negative marking: -<asp:Literal ID="litNegVal" runat="server" /> per wrong answer
                    </span>
                </asp:Panel>
            </div>
        </div>
        <div class="timer-box" id="timerBox">
            <asp:Literal ID="litTimerLabel" runat="server">Time Left:</asp:Literal>
            &nbsp;<span id="timerDisplay">--:--</span>
        </div>
    </div>

    <div class="progress-bar-outer">
        <div class="progress-bar-inner" id="progressBar" style="width:0%"></div>
    </div>

    <!-- Question Card -->
    <div class="question-card">
        <div class="question-meta">
            <span class="q-num">Q <asp:Literal ID="litQBadge" runat="server" /></span>
            <span class='badge diff-<asp:Literal ID="litDiffClass" runat="server" />'>
                <asp:Literal ID="litDiffLabel" runat="server" />
            </span>
            <span class='badge badge-grey'>
                <asp:Literal ID="litQTypeLabel" runat="server" />
            </span>
            <span class="text-muted" style="font-size:12px;"><asp:Literal ID="litSubject" runat="server" /></span>
        </div>

        <asp:Panel ID="pnlImage" runat="server" Visible="false">
            <img runat="server" id="qImage" src="" alt="Question image" class="question-image" />
        </asp:Panel>

        <div class="question-text"><asp:Literal ID="litQuestion" runat="server" /></div>

        <!-- Hint for multiple choice -->
        <asp:Panel ID="pnlMultiHint" runat="server" Visible="false">
            <div class="alert alert-info" style="padding:8px 14px;font-size:13px;margin-bottom:12px;">
                Select all correct options that apply.
            </div>
        </asp:Panel>

        <!-- RADIO OPTIONS -->
        <asp:Panel ID="pnlRadio" runat="server">
            <ul class="options-list">
                <asp:Repeater ID="rptOptions" runat="server">
                    <ItemTemplate>
                        <li class="option-item">
                            <input type="radio" name="studentAnswer"
                                   id='opt_<%# Eval("Letter") %>'
                                   value='<%# Eval("Letter") %>' class="radio-opt" />
                            <label for='opt_<%# Eval("Letter") %>' class="option-label">
                                <span class="opt-letter"><%# Eval("DisplayLabel") %></span>
                                <%# Eval("Text") %>
                            </label>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </asp:Panel>

        <!-- CHECKBOX OPTIONS -->
        <asp:Panel ID="pnlCheckbox" runat="server" Visible="false">
            <ul class="options-list">
                <asp:Repeater ID="rptCheckboxOpts" runat="server">
                    <ItemTemplate>
                        <li class="option-item">
                            <input type="checkbox"
                                   id='cb_<%# Eval("Letter") %>'
                                   value='<%# Eval("Letter") %>'
                                   class="cb-hidden cb-opt"
                                   style="display:none;" />
                            <label for='cb_<%# Eval("Letter") %>' class="option-label">
                                <span class="opt-letter"><%# Eval("DisplayLabel") %></span>
                                <%# Eval("Text") %>
                            </label>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </asp:Panel>

        <!-- PARAGRAPH / SHORT ANSWER -->
        <asp:Panel ID="pnlParagraph" runat="server" Visible="false">
            <textarea id="txtParaAns" class="form-control" rows="4"
                      placeholder="Type your answer here..."></textarea>
        </asp:Panel>
    </div>

    <!-- Hidden fields -->
    <asp:HiddenField ID="hfAnswer"     runat="server" />
    <asp:HiddenField ID="hfQIndex"     runat="server" />
    <asp:HiddenField ID="hfTimeLeft"   runat="server" />
    <asp:HiddenField ID="hfQuestionType" runat="server" />

    <div class="flex gap-2" style="justify-content:flex-end;">
        <asp:Button ID="btnNext"   runat="server" Text="Save and Next"
            CssClass="btn btn-primary btn-lg" OnClick="btnNext_Click" />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit Quiz"
            CssClass="btn btn-success btn-lg" Visible="false"
            OnClick="btnSubmit_Click"
            OnClientClick="return confirm('Submit the quiz now? This cannot be undone.')" />
    </div>

</div>
</asp:Panel>

<script>
(function () {
    var secsLeft  = parseInt(document.getElementById('<%= hfTimeLeft.ClientID %>').value || '0', 10);
    var totalQ    = parseInt('<%= IIf(Session("AQ_Total") Is Nothing, "1", Session("AQ_Total").ToString()) %>', 10);
    var currQ     = parseInt('<%= IIf(Session("AQ_CurrentIdx") Is Nothing, "0", Session("AQ_CurrentIdx").ToString()) %>', 10) + 1;
    var qType     = (document.getElementById('<%= hfQuestionType.ClientID %>') || {}).value || 'Radio';
    var hfAns     = document.getElementById('<%= hfAnswer.ClientID %>');
    var pb        = document.getElementById('progressBar');
    var timerDisp = document.getElementById('timerDisplay');
    var timerBox  = document.getElementById('timerBox');

    if (pb && totalQ > 0) pb.style.width = Math.round(((currQ - 1) / totalQ) * 100) + '%';

    // Answer capture based on question type
    if (qType === 'Radio') {
        document.querySelectorAll('.radio-opt').forEach(function (rb) {
            rb.addEventListener('change', function () { hfAns.value = this.value; });
        });
    } else if (qType === 'Checkbox') {
        function updateCB() {
            var sel = Array.from(document.querySelectorAll('.cb-opt:checked'))
                          .map(function (c) { return c.value; }).sort();
            hfAns.value = sel.join(',');
        }
        document.querySelectorAll('.cb-opt').forEach(function (cb) {
            cb.addEventListener('change', updateCB);
        });
    } else if (qType === 'Paragraph') {
        var paraEl = document.getElementById('txtParaAns');
        if (paraEl) paraEl.addEventListener('input', function () { hfAns.value = this.value; });
    }

    // Timer countdown
    function pad(n) { return n < 10 ? '0' + n : n; }
    if (timerDisp && secsLeft > 0) {
        var interval = setInterval(function () {
            secsLeft--;
            var m = Math.floor(secsLeft / 60), s = secsLeft % 60;
            timerDisp.textContent = pad(m) + ':' + pad(s);
            if (secsLeft <= 60 && timerBox) timerBox.classList.add('warning');
            if (secsLeft <= 0) {
                clearInterval(interval);
                var sub = document.getElementById('<%= btnSubmit.ClientID %>');
                var nxt = document.getElementById('<%= btnNext.ClientID %>');
                if (sub && sub.style.display !== 'none') sub.click();
                else if (nxt) nxt.click();
            }
        }, 1000);
    }
})();
</script>

</asp:Content>
