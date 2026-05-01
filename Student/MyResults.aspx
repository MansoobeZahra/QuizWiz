<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MyResults.aspx.vb" Inherits="Student_MyResults" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">My Results</asp:Content>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<asp:Panel ID="pnlHero" runat="server" Visible="false" style="border:2px solid #0078d4; padding:20px; margin-bottom:20px; background:#f0f7ff;">
    <h2 style="color:#0078d4; margin-top:0;">Quiz Result: <asp:Literal ID="litQuizName" runat="server" /></h2>
    <p>Score: <strong style="font-size:20px; color:#0078d4;"><asp:Literal ID="litPct" runat="server" /></strong></p>
    <p>Marks: <asp:Literal ID="litObtained" runat="server" /> / <asp:Literal ID="litTotal" runat="server" /></p>
    <p>Grade: <strong><asp:Literal ID="litGrade" runat="server" /></strong></p>
    <hr />
    <h3>Question Review</h3>
    <asp:GridView ID="gvDetail" runat="server" AutoGenerateColumns="false" Width="100%" BorderStyle="Solid" BorderWidth="1px">
        <Columns>
            <asp:BoundField DataField="QNo" HeaderText="#" />
            <asp:BoundField DataField="QuestionStatement" HeaderText="Question" />
            <asp:TemplateField HeaderText="Your Ans">
                <ItemTemplate>
                    <span style='color:<%# If(Eval("Marks") > 0, "green", "red") %>; font-weight:bold;'>
                        <%# Eval("StudentAns") %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CorrectAns" HeaderText="Correct" />
            <asp:BoundField DataField="Marks" HeaderText="Mark" />
        </Columns>
    </asp:GridView>
</asp:Panel>

<div style="border:1px solid #ccc; padding:20px; background:white;">
    <h3 style="margin-top:0;">All My Results History</h3>
    <asp:GridView ID="gvAllResults" runat="server" AutoGenerateColumns="false" Width="100%" BorderStyle="Solid" BorderWidth="1px">
        <Columns>
            <asp:BoundField DataField="QuizTitle" HeaderText="Quiz" />
            <asp:BoundField DataField="SubjectName" HeaderText="Subject" />
            <asp:BoundField DataField="ObtainedMarks" HeaderText="Marks" />
            <asp:BoundField DataField="TotalMarks" HeaderText="Total" />
            <asp:TemplateField HeaderText="Score">
                <ItemTemplate>
                    <strong style='color:<%# If(Eval("Percentage") >= 50, "green", "red") %>'>
                        <%# Eval("Percentage") %>%
                    </strong>
                </ItemTemplate>
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
