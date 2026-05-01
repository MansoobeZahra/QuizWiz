<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="Admin_Dashboard" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Admin Dashboard</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<h1>Admin Dashboard</h1>
<p>System Overview:</p>
<ul>
    <li>Total Users: <asp:Literal ID="litUsers" runat="server" /></li>
    <li>Teachers: <asp:Literal ID="litTeachers" runat="server" /></li>
    <li>Students: <asp:Literal ID="litStudents" runat="server" /></li>
    <li>Total Quizzes: <asp:Literal ID="litQuizzes" runat="server" /></li>
    <li>Total Questions: <asp:Literal ID="litQuestions" runat="server" /></li>
    <li>Total Attempts: <asp:Literal ID="litAttempts" runat="server" /></li>
</ul>

<hr />
<h3>Recent Quizzes</h3>
<asp:GridView ID="gvQuizzes" runat="server" AutoGenerateColumns="false" Width="100%" BorderStyle="Solid" BorderWidth="1px">
    <Columns>
        <asp:BoundField DataField="QuizTitle" HeaderText="Title" />
        <asp:BoundField DataField="CreatorName" HeaderText="Teacher" />
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# If(CBool(Eval("IsPublished")), "Published", "Draft") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Attempts" HeaderText="Attempts" />
    </Columns>
</asp:GridView>

<hr />
<h3>Recent Results</h3>
<asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false" Width="100%" BorderStyle="Solid" BorderWidth="1px">
    <Columns>
        <asp:BoundField DataField="StudentName" HeaderText="Student" />
        <asp:BoundField DataField="QuizTitle" HeaderText="Quiz" />
        <asp:TemplateField HeaderText="Score">
            <ItemTemplate><%# Eval("Percentage") %>%</ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="AttemptDate" HeaderText="Date" DataFormatString="{0:dd-MMM}" />
    </Columns>
</asp:GridView>

</asp:Content>
