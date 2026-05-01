<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MyResults.aspx.vb" Inherits="Student_MyResults" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">My Results</asp:Content>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- Charts removed for simplicity -->
</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<asp:Panel ID="pnlHero" runat="server" Visible="false" style="border:1px solid black; padding:15px; margin-bottom:20px;">
    <h2>Quiz Result: <asp:Literal ID="litQuizName" runat="server" /></h2>
    <p>Score: <asp:Literal ID="litPct" runat="server" /></p>
    <p>Marks: <asp:Literal ID="litObtained" runat="server" /> / <asp:Literal ID="litTotal" runat="server" /></p>
    <p>Grade: <asp:Literal ID="litGrade" runat="server" /></p>
    <hr />
    <h3>Question Review</h3>
    <asp:GridView ID="gvDetail" runat="server" AutoGenerateColumns="false" Width="100%" BorderStyle="Solid" BorderWidth="1px">
        <Columns>
            <asp:BoundField DataField="QNo" HeaderText="#" />
            <asp:BoundField DataField="QuestionStatement" HeaderText="Question" />
            <asp:TemplateField HeaderText="Your Ans">
                <ItemTemplate><%# GetAnswerHtml(Eval("StudentAns"), Eval("CorrectAns")) %></ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CorrectAns" HeaderText="Correct" />
            <asp:BoundField DataField="Marks" HeaderText="Mark" />
        </Columns>
    </asp:GridView>
</asp:Panel>

<div style="border:1px solid black; padding:15px;">
    <h3>All My Results</h3>
    <asp:GridView ID="gvAllResults" runat="server" AutoGenerateColumns="false" Width="100%" BorderStyle="Solid" BorderWidth="1px">
        <Columns>
            <asp:BoundField DataField="QuizTitle" HeaderText="Quiz" />
            <asp:BoundField DataField="SubjectName" HeaderText="Subject" />
            <asp:BoundField DataField="ObtainedMarks" HeaderText="Marks" />
            <asp:BoundField DataField="TotalMarks" HeaderText="Total" />
            <asp:TemplateField HeaderText="Score">
                <ItemTemplate><%# Eval("Percentage") %>%</ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Grade">
                <ItemTemplate><%# GetGradeHtml(Eval("Percentage")) %></ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="AttemptDate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
        </Columns>
    </asp:GridView>
</div>

<asp:Literal ID="litChartScript" runat="server" Visible="false" />

</asp:Content>
