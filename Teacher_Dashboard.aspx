<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Teacher_Dashboard.aspx.vb" Inherits="Teacher_Dashboard" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Teacher Dashboard - QuizWiz</title>
    <link rel="stylesheet" href="Styles/site.css" />
</head>
<body>
    <div class="top-nav">
        <strong>QuizWiz</strong>&nbsp;&nbsp;
        <a href="Teacher_Dashboard.aspx">Dashboard</a>
        <a href="Teacher_AddQ.aspx">Add Question</a>
        <a href="Teacher_Bank.aspx">Question Bank</a>
        <a href="Teacher_CreateQuiz.aspx">Create Quiz</a>
        <a href="Teacher_Results.aspx">Results</a>
        <a href="Logout.aspx" style="float:right;">Logout</a>
    </div>
    <div class="layout-container">
        <div class="sidebar">
            <a href="Teacher_Dashboard.aspx">Dashboard</a>
            <a href="Teacher_AddQ.aspx">Add Question</a>
            <a href="Teacher_Bank.aspx">Question Bank</a>
            <a href="Teacher_CreateQuiz.aspx">Create Quiz</a>
            <a href="Teacher_Results.aspx">Results</a>
            <a href="Logout.aspx">Logout</a>
        </div>
        <div class="content-area">
            <form id="form1" runat="server">
                <h2>Welcome Teacher!</h2>
                <div style="background-color:#f4f4f4; padding:15px; border-left:5px solid #0078d4; margin-bottom:20px;">
                    <strong>Your Stats:</strong><br />
                    Total Questions: <asp:Literal ID="litTotalQ" runat="server">0</asp:Literal> |
                    Total Quizzes: <asp:Literal ID="litTotalQuiz" runat="server">0</asp:Literal> |
                    Published: <asp:Literal ID="litPublished" runat="server">0</asp:Literal> |
                    Attempts: <asp:Literal ID="litAttempts" runat="server">0</asp:Literal>
                </div>
                <h3>Quick Actions</h3>
                <a href="Teacher_AddQ.aspx" class="btn btn-blue">Add New Question</a>
                <a href="Teacher_CreateQuiz.aspx" class="btn btn-green">Create New Quiz</a>
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
                                <asp:LinkButton ID="btnToggle" runat="server" CommandName="PublishToggle" CommandArgument='<%# Eval("QuizID") %>'
                                    Text='<%# If(CBool(Eval("IsPublished")), "Unpublish", "Publish") %>'
                                    ForeColor='<%# If(CBool(Eval("IsPublished")), System.Drawing.Color.Orange, System.Drawing.Color.Blue) %>' />
                                |
                                <a href='Teacher_Results.aspx?quizid=<%# Eval("QuizID") %>' style="color:blue;">Results</a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </form>
        </div>
    </div>
</body>
</html>
