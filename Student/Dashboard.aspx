<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="Student_Dashboard" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Dashboard</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<h2>Welcome Student!</h2>
<div style="background-color:#f4f4f4; padding:15px; border-left:5px solid #0078d4; margin-bottom:20px;">
    <strong>My Overview:</strong><br />
    Available Quizzes: <asp:Literal ID="litAvailable" runat="server">0</asp:Literal> |
    Completed: <asp:Literal ID="litAttempted" runat="server">0</asp:Literal> |
    Avg Score: <asp:Literal ID="litAvgScore" runat="server">-</asp:Literal> |
    Notifications: <asp:Literal ID="litNotifs" runat="server">0</asp:Literal>
</div>

<asp:Panel ID="pnlNotifs" runat="server" Visible="false" style="border:1px solid #ffc107; background:#fff9e6; padding:15px; margin-bottom:20px;">
    <h3 style="color:#856404; margin-top:0;">New Notifications</h3>
    <asp:LinkButton ID="btnMarkRead" runat="server" Text="Mark All Read" OnClick="btnMarkRead_Click" style="font-size:12px; color:#856404;" />
    <asp:Repeater ID="rptNotifs" runat="server">
        <ItemTemplate>
            <div style="border-bottom:1px solid #ffeeba; padding:5px; margin-top:5px; font-size:13px;">
                <%# Eval("Message") %> <span style="color:#999;">(<%# Eval("CreatedAt") %>)</span>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>

<h3>Available Quizzes</h3>
<asp:Panel ID="pnlNoQuiz" runat="server" Visible="false" CssClass="alert alert-info">
    No quizzes available for your subject right now.
</asp:Panel>
<asp:GridView ID="gvQuizzes" runat="server" AutoGenerateColumns="false" Width="100%" BorderStyle="Solid" BorderWidth="1px">
    <Columns>
        <asp:BoundField DataField="QuizTitle" HeaderText="Quiz Title" />
        <asp:BoundField DataField="SubjectName" HeaderText="Subject" />
        <asp:BoundField DataField="AllowedTime" HeaderText="Time (min)" />
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <span style='color:<%# If(CBool(Eval("AlreadyAttempted")), "gray", "green") %>; font-weight:bold;'>
                    <%# If(CBool(Eval("AlreadyAttempted")), "Completed", "Available") %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Action">
            <ItemTemplate>
                <%# If(CBool(Eval("AlreadyAttempted")), "<a href='MyResults.aspx' style='color:blue;'>View Result</a>", "<a href='AttemptQuiz.aspx?quizid=" & Eval("QuizID") & "' class='btn btn-primary' style='padding:3px 10px;'>Start Quiz</a>") %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

</asp:Content>
