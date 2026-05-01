<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="Teacher_Dashboard" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Teacher Dashboard</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<h2>Welcome Teacher!</h2>
<p>Here are your stats:</p>
<ul style="list-style:none; padding:0;">
    <li>Total Questions: <asp:Literal ID="litTotalQ" runat="server">0</asp:Literal></li>
    <li>Total Quizzes: <asp:Literal ID="litTotalQuiz" runat="server">0</asp:Literal></li>
    <li>Published Quizzes: <asp:Literal ID="litPublished" runat="server">0</asp:Literal></li>
    <li>Total Attempts: <asp:Literal ID="litAttempts" runat="server">0</asp:Literal></li>
</ul>

<hr />
<h3>Quick Actions</h3>
<a href="AddQuestion.aspx" class="btn">Add New Question</a>
<a href="CreateQuiz.aspx" class="btn">Create New Quiz</a>

<hr />
<h3>My Quizzes</h3>
<asp:GridView ID="gvQuizzes" runat="server" AutoGenerateColumns="false" Width="100%" OnRowCommand="gvQuizzes_RowCommand">
    <Columns>
        <asp:BoundField DataField="QuizTitle" HeaderText="Title" />
        <asp:BoundField DataField="SubjectName" HeaderText="Subject" />
        <asp:BoundField DataField="AllowedTime" HeaderText="Time" />
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# If(CBool(Eval("IsPublished")), "Published", "Draft") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:LinkButton runat="server" CommandName="PublishToggle" CommandArgument='<%# Eval("QuizID") %>' Text='<%# If(CBool(Eval("IsPublished")), "Unpublish", "Publish") %>' />
                &nbsp;|&nbsp;
                <a href='ViewResults.aspx?quizid=<%# Eval("QuizID") %>'>View Results</a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

</asp:Content>
