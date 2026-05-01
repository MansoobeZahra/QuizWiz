<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="Student_Dashboard" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Dashboard</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<h2>Welcome Student!</h2>
<p>Current Stats:</p>
<ul>
    <li>Available Quizzes: <asp:Literal ID="litAvailable" runat="server">0</asp:Literal></li>
    <li>Completed: <asp:Literal ID="litAttempted" runat="server">0</asp:Literal></li>
    <li>My Avg Score: <asp:Literal ID="litAvgScore" runat="server">-</asp:Literal></li>
    <li>New Notifications: <asp:Literal ID="litNotifs" runat="server">0</asp:Literal></li>
</ul>

<hr />
<h3>Notifications</h3>
<asp:Panel ID="pnlNotifs" runat="server" Visible="false">
    <asp:LinkButton ID="btnMarkRead" runat="server" Text="Mark All Read" OnClick="btnMarkRead_Click" />
    <br />
    <asp:Repeater ID="rptNotifs" runat="server">
        <ItemTemplate>
            <div style="border:1px solid #ccc; padding:5px; margin-top:5px;">
                <%# Eval("Message") %> (<%# Eval("CreatedAt") %>)
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>

<hr />
<h3>Available Quizzes</h3>
<asp:Panel ID="pnlNoQuiz" runat="server" Visible="false" style="color:blue;">
    No quizzes available.
</asp:Panel>
<asp:GridView ID="gvQuizzes" runat="server" AutoGenerateColumns="false" Width="100%" BorderStyle="Solid" BorderWidth="1px">
    <Columns>
        <asp:BoundField DataField="QuizTitle" HeaderText="Quiz" />
        <asp:BoundField DataField="SubjectName" HeaderText="Subject" />
        <asp:BoundField DataField="AllowedTime" HeaderText="Time" />
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# If(CBool(Eval("AlreadyAttempted")), "Completed", "Available") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Action">
            <ItemTemplate>
                <%# If(CBool(Eval("AlreadyAttempted")), "<a href='MyResults.aspx'>Result</a>", "<a href='AttemptQuiz.aspx?quizid=" & Eval("QuizID") & "'>Start</a>") %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

</asp:Content>
