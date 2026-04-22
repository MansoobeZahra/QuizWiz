<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="Teacher_Dashboard" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Teacher Dashboard</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
    <h1>Teacher Dashboard</h1>
    <p>Welcome back, <strong><%= Session("FullName") %></strong> — manage your questions and quizzes below.</p>
</div>

<!-- Stats Row -->
<div class="stats-grid">
    <div class="stat-card anim-1">
        <div class="stat-icon purple">📝</div>
        <div>
            <div class="stat-value"><asp:Literal ID="litTotalQ" runat="server">0</asp:Literal></div>
            <div class="stat-label">My Questions</div>
        </div>
    </div>
    <div class="stat-card anim-2">
        <div class="stat-icon cyan">🎲</div>
        <div>
            <div class="stat-value"><asp:Literal ID="litTotalQuiz" runat="server">0</asp:Literal></div>
            <div class="stat-label">My Quizzes</div>
        </div>
    </div>
    <div class="stat-card anim-3">
        <div class="stat-icon green">✅</div>
        <div>
            <div class="stat-value"><asp:Literal ID="litPublished" runat="server">0</asp:Literal></div>
            <div class="stat-label">Published</div>
        </div>
    </div>
    <div class="stat-card anim-4">
        <div class="stat-icon orange">🎓</div>
        <div>
            <div class="stat-value"><asp:Literal ID="litAttempts" runat="server">0</asp:Literal></div>
            <div class="stat-label">Total Attempts</div>
        </div>
    </div>
</div>

<!-- Quick Actions -->
<div class="grid-2 mb-6">
    <a href="/Teacher/AddQuestion.aspx" class="card" style="text-decoration:none;cursor:pointer;display:block;">
        <div style="font-size:36px;margin-bottom:12px;">➕</div>
        <div class="fw-700" style="font-size:16px;margin-bottom:6px;">Add New Question</div>
        <div class="text-muted">Add a question to your subject question bank</div>
    </a>
    <a href="/Teacher/CreateQuiz.aspx" class="card" style="text-decoration:none;cursor:pointer;display:block;">
        <div style="font-size:36px;margin-bottom:12px;">🎲</div>
        <div class="fw-700" style="font-size:16px;margin-bottom:6px;">Create a Quiz</div>
        <div class="text-muted">Design and publish a new quiz for students</div>
    </a>
</div>

<!-- Recent Quizzes -->
<div class="card">
    <div class="card-header">
        <h3>📋 My Recent Quizzes</h3>
        <a href="/Teacher/CreateQuiz.aspx" class="btn btn-primary btn-sm">+ New Quiz</a>
    </div>
    <div class="table-wrapper">
        <asp:GridView ID="gvQuizzes" runat="server"
            AutoGenerateColumns="false"
            CssClass="w-100"
            GridLines="None"
            EmptyDataText="No quizzes created yet."
            OnRowCommand="gvQuizzes_RowCommand">
            <Columns>
                <asp:BoundField DataField="QuizTitle" HeaderText="Quiz Title" />
                <asp:BoundField DataField="SubjectName" HeaderText="Subject" />
                <asp:BoundField DataField="AllowedTime" HeaderText="Time (min)" />
                <asp:BoundField DataField="TotalQuestions" HeaderText="Questions" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge <%# If(CBool(Eval("IsPublished")), "badge-green", "badge-grey") %>'>
                            <%# If(CBool(Eval("IsPublished")), "Published", "Draft") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Attempts">
                    <ItemTemplate><%# Eval("AttemptCount") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="PublishToggle"
                            CommandArgument='<%# Eval("QuizID") %>'
                            CssClass='btn btn-sm <%# If(CBool(Eval("IsPublished")), "btn-warning", "btn-success") %>'
                            OnClientClick="return confirm('Toggle publish status?')">
                            <%# If(CBool(Eval("IsPublished")), "Unpublish", "Publish") %>
                        </asp:LinkButton>
                        &nbsp;
                        <a href='ViewResults.aspx?quizid=<%# Eval("QuizID") %>' class="btn btn-outline btn-sm">Results</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

</asp:Content>
