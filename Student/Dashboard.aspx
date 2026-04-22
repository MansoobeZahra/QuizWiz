<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="Student_Dashboard" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Student Dashboard</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
    <h1>🎓 Student Dashboard</h1>
    <p>Welcome, <strong><%= Session("FullName") %></strong>! Choose a quiz below to get started.</p>
</div>

<!-- Stats -->
<div class="stats-grid mb-6">
    <div class="stat-card anim-1">
        <div class="stat-icon purple">📝</div>
        <div>
            <div class="stat-value"><asp:Literal ID="litAvailable" runat="server">0</asp:Literal></div>
            <div class="stat-label">Available Quizzes</div>
        </div>
    </div>
    <div class="stat-card anim-2">
        <div class="stat-icon green">✅</div>
        <div>
            <div class="stat-value"><asp:Literal ID="litAttempted" runat="server">0</asp:Literal></div>
            <div class="stat-label">Completed</div>
        </div>
    </div>
    <div class="stat-card anim-3">
        <div class="stat-icon cyan">📊</div>
        <div>
            <div class="stat-value"><asp:Literal ID="litAvgScore" runat="server">—</asp:Literal></div>
            <div class="stat-label">My Avg Score</div>
        </div>
    </div>
    <div class="stat-card anim-4">
        <div class="stat-icon orange">🔔</div>
        <div>
            <div class="stat-value"><asp:Literal ID="litNotifs" runat="server">0</asp:Literal></div>
            <div class="stat-label">Notifications</div>
        </div>
    </div>
</div>

<!-- Notifications -->
<asp:Panel ID="pnlNotifs" runat="server" Visible="false" CssClass="card mb-6">
    <div class="card-header">
        <h3>🔔 New Notifications</h3>
        <asp:LinkButton ID="btnMarkRead" runat="server" CssClass="btn btn-outline btn-sm"
            OnClick="btnMarkRead_Click">Mark all read</asp:LinkButton>
    </div>
    <div class="notif-list">
        <asp:Repeater ID="rptNotifs" runat="server">
            <ItemTemplate>
                <div class='notif-item <%# If(Not CBool(Eval("IsRead")), "unread", "") %>'>
                    <div class="notif-icon">🔔</div>
                    <div>
                        <div class="notif-msg"><%# Eval("Message") %></div>
                        <div class="notif-time"><%# CDate(Eval("CreatedAt")).ToString("dd MMM yyyy  hh:mm tt") %></div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>

<!-- Available Quizzes -->
<div class="card mb-6">
    <div class="card-header">
        <h3>📋 Available Quizzes</h3>
        <a href="MyResults.aspx" class="btn btn-outline btn-sm">🏆 My Results</a>
    </div>
    <asp:Panel ID="pnlNoQuiz" runat="server" Visible="false" CssClass="alert alert-info">
        ℹ️ No quizzes are published yet. Check back soon!
    </asp:Panel>
    <div class="table-wrapper">
        <asp:GridView ID="gvQuizzes" runat="server"
            AutoGenerateColumns="false"
            CssClass="w-100"
            GridLines="None"
            EmptyDataText="No quizzes available.">
            <Columns>
                <asp:BoundField DataField="QuizTitle"   HeaderText="Quiz" />
                <asp:BoundField DataField="SubjectName" HeaderText="Subject" />
                <asp:BoundField DataField="TotalQuestions" HeaderText="Questions" />
                <asp:BoundField DataField="AllowedTime"    HeaderText="Time (min)" />
                <asp:TemplateField HeaderText="Remarks">
                    <ItemTemplate><span class="text-muted"><%# If(Eval("Remarks") Is DBNull.Value, "—", Eval("Remarks")) %></span></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <% Dim alreadyDone As Boolean = CBool(Eval("AlreadyAttempted")) %>
                        <% If alreadyDone Then %>
                            <span class="badge badge-green">✅ Completed</span>
                        <% Else %>
                            <span class="badge badge-purple">🟣 Available</span>
                        <% End If %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <% Dim alreadyDone2 As Boolean = CBool(Eval("AlreadyAttempted")) %>
                        <% If alreadyDone2 Then %>
                            <a href='MyResults.aspx' class="btn btn-outline btn-sm">View Result</a>
                        <% Else %>
                            <a href='AttemptQuiz.aspx?quizid=<%# Eval("QuizID") %>'
                               class="btn btn-primary btn-sm"
                               onclick="return confirm('Start quiz now? The timer will begin immediately.')">
                               🚀 Start Quiz
                            </a>
                        <% End If %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

</asp:Content>
