<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ManageQuestions.aspx.vb" Inherits="Teacher_ManageQuestions" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Question Bank</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header flex-between">
    <div>
        <h1> Question Bank</h1>
        <p>All questions you have created. Edit or delete as needed.</p>
    </div>
    <a href="AddQuestion.aspx" class="btn btn-primary"> Add Question</a>
</div>

<asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="alert alert-success mb-4">
     <asp:Literal ID="litMsg" runat="server" />
</asp:Panel>

<!-- Filter Bar -->
<div class="form-panel mb-6" style="padding:16px 20px;">
    <div class="form-row">
        <div class="form-group" style="margin-bottom:0">
            <label>Filter by Subject</label>
            <asp:DropDownList ID="ddlFilter" runat="server" CssClass="form-control"
                AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed" />
        </div>
        <div class="form-group" style="margin-bottom:0">
            <label>Filter by Difficulty</label>
            <asp:DropDownList ID="ddlDiffFilter" runat="server" CssClass="form-control"
                AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed">
                <asp:ListItem Value="">All Difficulties</asp:ListItem>
                <asp:ListItem Value="Easy">Easy</asp:ListItem>
                <asp:ListItem Value="Medium">Medium</asp:ListItem>
                <asp:ListItem Value="Hard">Hard</asp:ListItem>
                <asp:ListItem Value="Expert">Expert</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
</div>

<!-- Questions Table -->
<div class="card">
    <div class="table-wrapper">
        <asp:GridView ID="gvQuestions" runat="server"
            AutoGenerateColumns="false"
            CssClass="w-100"
            GridLines="None"
            EmptyDataText="No questions found."
            OnRowCommand="gvQuestions_RowCommand"
            OnRowEditing="gvQuestions_RowEditing"
            OnRowCancelingEdit="gvQuestions_CancelEdit"
            OnRowUpdating="gvQuestions_RowUpdating"
            DataKeyNames="QuestionID">
            <Columns>
                <asp:BoundField DataField="QuestionID" HeaderText="ID" ReadOnly="true" />
                <asp:TemplateField HeaderText="Question">
                    <ItemTemplate>
                        <div style="max-width:340px;font-size:13px;"><%# Eval("QuestionStatement") %></div>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditStmt" runat="server" CssClass="form-control"
                            TextMode="MultiLine" Rows="3"
                            Text='<%# Eval("QuestionStatement") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SubjectName" HeaderText="Subject" ReadOnly="true" />
                <asp:TemplateField HeaderText="Difficulty">
                    <ItemTemplate>
                        <span class="badge">
                            <%# Eval("DifficultyLevel") %>
                        </span>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlEditDiff" runat="server" CssClass="form-control">
                            <asp:ListItem Value="Easy">Easy</asp:ListItem>
                            <asp:ListItem Value="Medium">Medium</asp:ListItem>
                            <asp:ListItem Value="Hard">Hard</asp:ListItem>
                            <asp:ListItem Value="Expert">Expert</asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Correct">
                    <ItemTemplate>
                        <span class="badge"><%# Eval("CorrectOption") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton CommandName="Edit" runat="server"
                            CssClass="btn btn-outline btn-sm">Edit</asp:LinkButton>
                        &nbsp;
                        <asp:LinkButton CommandName="DeleteQ" runat="server"
                            CommandArgument='<%# Eval("QuestionID") %>'
                            CssClass="btn btn-danger btn-sm"
                            OnClientClick="return confirm('Delete this question?')">Delete</asp:LinkButton>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton CommandName="Update" runat="server"
                            CssClass="btn btn-success btn-sm"> Save</asp:LinkButton>
                        &nbsp;
                        <asp:LinkButton CommandName="Cancel" runat="server"
                            CssClass="btn btn-outline btn-sm">Cancel</asp:LinkButton>
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

</asp:Content>
