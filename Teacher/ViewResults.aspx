<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ViewResults.aspx.vb" Inherits="Teacher_ViewResults" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Results</asp:Content>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- Chart.js removed for simplicity -->
</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<h1>Quiz Results</h1>

<div style="border:1px solid black; padding:10px; background:#f0f0f0;">
    Select Quiz: <asp:DropDownList ID="ddlQuiz" runat="server" />
    <asp:Button ID="btnView" runat="server" Text="View" OnClick="btnView_Click" />
</div>

<asp:Panel ID="pnlNoData" runat="server" Visible="false" style="color:blue; margin-top:10px;">
    No attempts yet.
</asp:Panel>

<asp:Panel ID="pnlResults" runat="server" Visible="false">
    <br />
    <div style="border:1px solid black; padding:10px;">
        <b>Summary:</b><br />
        Total Attempts: <asp:Literal ID="litTotalStudents" runat="server">0</asp:Literal><br />
        Average Score: <asp:Literal ID="litAvgScore" runat="server">0%</asp:Literal><br />
        Highest: <asp:Literal ID="litHighest" runat="server">0%</asp:Literal><br />
        Lowest: <asp:Literal ID="litLowest" runat="server">0%</asp:Literal>
    </div>

    <br />
    <h3>Student List</h3>
    <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false" Width="100%" BorderStyle="Solid" BorderWidth="1px">
        <Columns>
            <asp:BoundField DataField="StudentName" HeaderText="Student" />
            <asp:BoundField DataField="ObtainedMarks" HeaderText="Marks" />
            <asp:BoundField DataField="TotalMarks" HeaderText="Total" />
            <asp:TemplateField HeaderText="Percentage">
                <ItemTemplate><%# Eval("Percentage") %>%</ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Grade">
                <ItemTemplate><%# GetGradeHtml(Eval("Percentage")) %></ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="AttemptDate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
        </Columns>
    </asp:GridView>

    <asp:Literal ID="litChartJS" runat="server" Visible="false" />
</asp:Panel>

</asp:Content>
