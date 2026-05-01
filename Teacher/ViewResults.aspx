<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ViewResults.aspx.vb" Inherits="Teacher_ViewResults" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Results</asp:Content>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<h1>Quiz Results</h1>

<div style="border:1px solid #0078d4; padding:15px; background:#f0f7ff; margin-bottom:20px;">
    <strong>Select Quiz:</strong> 
    <asp:DropDownList ID="ddlQuiz" runat="server" />
    <asp:Button ID="btnView" runat="server" Text="View Results" OnClick="btnView_Click" CssClass="btn btn-primary" />
</div>

<asp:Panel ID="pnlNoData" runat="server" Visible="false" CssClass="alert alert-info">
    No student attempts found for this quiz yet.
</asp:Panel>

<asp:Panel ID="pnlResults" runat="server" Visible="false">
    <div style="display:flex; gap:20px; margin-bottom:20px;">
        <div style="flex:1; border:1px solid #ccc; padding:15px; background:white; border-top:5px solid #0078d4;">
            <b style="color:#0078d4;">Performance Summary:</b><br /><br />
            Total Attempts: <asp:Literal ID="litTotalStudents" runat="server">0</asp:Literal><br />
            Average Score: <asp:Literal ID="litAvgScore" runat="server">0%</asp:Literal>
        </div>
        <div style="flex:1; border:1px solid #ccc; padding:15px; background:white; border-top:5px solid #28a745;">
            <b style="color:#28a745;">High/Low Scores:</b><br /><br />
            Highest: <asp:Literal ID="litHighest" runat="server">0%</asp:Literal><br />
            Lowest: <asp:Literal ID="litLowest" runat="server">0%</asp:Literal>
        </div>
    </div>

    <h3>Student Detailed List</h3>
    <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false" Width="100%" BorderStyle="Solid" BorderWidth="1px">
        <Columns>
            <asp:BoundField DataField="StudentName" HeaderText="Student" />
            <asp:BoundField DataField="ObtainedMarks" HeaderText="Marks" />
            <asp:BoundField DataField="TotalMarks" HeaderText="Total" />
            <asp:TemplateField HeaderText="Percentage">
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

    <asp:Literal ID="litChartJS" runat="server" Visible="false" />
</asp:Panel>

</asp:Content>
