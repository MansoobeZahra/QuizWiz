<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="Teacher_Dashboard" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Teacher Dashboard</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<h2>Welcome Teacher!</h2>
<div style="background-color:#f4f4f4; padding:15px; border-left:5px solid #0078d4; margin-bottom:20px;">
    <strong>Your Stats:</strong><br />
    Total Questions: <asp:Literal ID="litTotalQ" runat="server">0</asp:Literal> |
    Total Quizzes: <asp:Literal ID="litTotalQuiz" runat="server">0</asp:Literal> |
    Published: <asp:Literal ID="litPublished" runat="server">0</asp:Literal> |
    Attempts: <asp:Literal ID="litAttempts" runat="server">0</asp:Literal>
</div>

<h3>Quick Actions</h3>
<a href="AddQuestion.aspx" class="btn btn-primary">Add New Question</a>
<a href="CreateQuiz.aspx" class="btn btn-success">Create New Quiz</a>

<hr />
<h3>My Quizzes</h3>
<asp:GridView ID="gvQuizzes" runat="server" AutoGenerateColumns="false" Width="100%" OnRowCommand="gvQuizzes_RowCommand">
    <Columns>
        <asp:BoundField DataField="QuizTitle" HeaderText="Title" />
        <asp:BoundField DataField="SubjectName" HeaderText="Subject" />
        <asp:BoundField DataField="AllowedTime" HeaderText="Time" />
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <span style='color:<%# If(CBool(Eval("IsPublished")), "green", "gray") %>; font-weight:bold;'>
                    <%# If(CBool(Eval("IsPublished")), "Published", "Draft") %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:LinkButton runat="server" CommandName="PublishToggle" CommandArgument='<%# Eval("QuizID") %>' 
                    Text='<%# If(CBool(Eval("IsPublished")), "Unpublish", "Publish") %>' 
                    style='color:<%# If(CBool(Eval("IsPublished")), "orange", "blue") %>;' />
                &nbsp;|&nbsp;
                <a href='ViewResults.aspx?quizid=<%# Eval("QuizID") %>' style="color:blue;">View Results</a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

</asp:Content>
