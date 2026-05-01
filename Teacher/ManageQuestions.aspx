<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ManageQuestions.aspx.vb" Inherits="Teacher_ManageQuestions" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Question Bank</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<h2>My Questions</h2>
<asp:Panel ID="pnlMsg" runat="server" Visible="false" style="padding:10px; margin-bottom:10px; border:1px solid #ccc;">
    <asp:Literal ID="litMsg" runat="server" />
</asp:Panel>

<a href="AddQuestion.aspx" class="btn">Add New Question</a>
<br /><br />

<div style="border:1px solid #999; padding:10px;">
    Filters:<br />
    Subject: <asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed" />
    Difficulty: 
    <asp:DropDownList ID="ddlDiffFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed">
        <asp:ListItem Value="">All</asp:ListItem>
        <asp:ListItem>Easy</asp:ListItem>
        <asp:ListItem>Medium</asp:ListItem>
        <asp:ListItem>Hard</asp:ListItem>
        <asp:ListItem>Expert</asp:ListItem>
    </asp:DropDownList>
</div>

<br />

<asp:GridView ID="gvQuestions" runat="server" AutoGenerateColumns="false" Width="100%"
    OnRowCommand="gvQuestions_RowCommand" OnRowEditing="gvQuestions_RowEditing"
    OnRowCancelingEdit="gvQuestions_CancelEdit" OnRowUpdating="gvQuestions_RowUpdating"
    DataKeyNames="QuestionID" BorderStyle="Solid" BorderWidth="1px">
    <Columns>
        <asp:BoundField DataField="QuestionID" HeaderText="ID" ReadOnly="true" />
        <asp:TemplateField HeaderText="Question">
            <ItemTemplate><%# Eval("QuestionStatement") %></ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditStmt" runat="server" TextMode="MultiLine" Rows="2" Text='<%# Eval("QuestionStatement") %>' />
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
                    <asp:ListItem>Expert</asp:ListItem>
                </asp:DropDownList>
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="CorrectOption" HeaderText="Ans" />
        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:LinkButton CommandName="Edit" runat="server" Text="Edit" />
                |
                <asp:LinkButton CommandName="DeleteQ" runat="server" CommandArgument='<%# Eval("QuestionID") %>' Text="Delete" OnClientClick="return confirm('Delete?')" />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:LinkButton CommandName="Update" runat="server" Text="Save" />
                |
                <asp:LinkButton CommandName="Cancel" runat="server" Text="Cancel" />
            </EditItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

</asp:Content>
