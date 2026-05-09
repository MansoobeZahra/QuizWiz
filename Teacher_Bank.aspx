<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Teacher_Bank.aspx.vb" Inherits="Teacher_ManageQuestions" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Question Bank - QuizWiz</title>
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
                <h2>Question Bank</h2>
                <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="alert">
                    <asp:Literal ID="litMsg" runat="server" />
                </asp:Panel>
                <a href="Teacher_AddQ.aspx" class="btn btn-blue">Add New Question</a>
                <br /><br />
                <div style="border:1px solid #0078d4; padding:10px; background:#f0f7ff; margin-bottom:10px;">
                    <b>Filters:</b><br />
                    Subject: <asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed" />
                    Difficulty:
                    <asp:DropDownList ID="ddlDiffFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed">
                        <asp:ListItem Value="">All</asp:ListItem>
                        <asp:ListItem>Easy</asp:ListItem>
                        <asp:ListItem>Medium</asp:ListItem>
                        <asp:ListItem>Hard</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:GridView ID="gvQuestions" runat="server" AutoGenerateColumns="false" Width="100%"
                    OnRowCommand="gvQuestions_RowCommand" OnRowEditing="gvQuestions_RowEditing"
                    OnRowCancelingEdit="gvQuestions_CancelEdit" OnRowUpdating="gvQuestions_RowUpdating"
                    DataKeyNames="QuestionID" BorderStyle="Solid" BorderWidth="1px">
                    <Columns>
                        <asp:BoundField DataField="QuestionID" HeaderText="ID" ReadOnly="true" />
                        <asp:TemplateField HeaderText="Question">
                            <ItemTemplate><%# Eval("QuestionStatement") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditStmt" runat="server" TextMode="MultiLine" Rows="2" Width="100%" Text='<%# Eval("QuestionStatement") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="SubjectName" HeaderText="Subject" ReadOnly="true" />
                        <asp:TemplateField HeaderText="Difficulty">
                            <ItemTemplate><%# Eval("DifficultyLevel") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlEditDiff" runat="server">
                                    <asp:ListItem>Easy</asp:ListItem>
                                    <asp:ListItem>Medium</asp:ListItem>
                                    <asp:ListItem>Hard</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CorrectOptions" HeaderText="Ans" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton CommandName="Edit" runat="server" Text="Edit" style="color:blue;" /> |
                                <asp:LinkButton CommandName="DeleteQ" runat="server" CommandArgument='<%# Eval("QuestionID") %>' Text="Delete" style="color:red;" OnClientClick="return confirm('Delete?')" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton CommandName="Update" runat="server" Text="Save" style="color:green;" /> |
                                <asp:LinkButton CommandName="Cancel" runat="server" Text="Cancel" style="color:gray;" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </form>
        </div>
    </div>
</body>
</html>
