<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Teacher_Results.aspx.vb" Inherits="Teacher_ViewResults" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Results - QuizWiz</title>
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
                <h2>Quiz Results</h2>
                <div style="border:1px solid #0078d4; padding:10px; background:#f0f7ff; margin-bottom:10px;">
                    Quiz: <asp:DropDownList ID="ddlQuiz" runat="server" />
                    <asp:Button ID="btnView" runat="server" Text="View" OnClick="btnView_Click" CssClass="btn btn-blue" />
                </div>
                <asp:Panel ID="pnlResults" runat="server" Visible="false">
                    <div style="display:flex; gap:10px; margin-bottom:10px;">
                        <div style="flex:1; border:1px solid #ccc; padding:10px; border-top:3px solid #0078d4;">
                            <b>Attempts:</b> <asp:Literal ID="litTotalStudents" runat="server" /><br />
                            <b>Average:</b> <asp:Literal ID="litAvgScore" runat="server" />
                        </div>
                        <div style="flex:1; border:1px solid #ccc; padding:10px; border-top:3px solid #28a745;">
                            <b>Highest:</b> <asp:Literal ID="litHighest" runat="server" /><br />
                            <b>Lowest:</b> <asp:Literal ID="litLowest" runat="server" />
                        </div>
                    </div>
                    <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false" Width="100%">
                        <Columns>
                            <asp:BoundField DataField="StudentName" HeaderText="Student" />
                            <asp:BoundField DataField="ObtainedMarks" HeaderText="Marks" />
                            <asp:BoundField DataField="TotalMarks" HeaderText="Total" />
                            <asp:TemplateField HeaderText="Score">
                                <ItemTemplate>
                                    <span style='color:<%# If(Eval("Percentage") >= 50, "green", "red") %>'><%# Eval("Percentage") %>%</span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="AttemptDate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
                        </Columns>
                    </asp:GridView>
                </asp:Panel>
            </form>
        </div>
    </div>
</body>
</html>
