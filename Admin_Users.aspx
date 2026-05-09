<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Admin_Users.aspx.vb" Inherits="Admin_ManageUsers" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Manage Users - QuizWiz</title>
    <link rel="stylesheet" href="Styles/site.css" />
</head>
<body>
    <div class="top-nav">
        <strong>QuizWiz</strong>
        &nbsp;&nbsp;
        <a href="Admin_Dashboard.aspx">Dashboard</a>
        <a href="Admin_Users.aspx">Manage Users</a>
        <a href="Logout.aspx" style="float:right;">Logout</a>
    </div>
    <div class="layout-container">
        <div class="sidebar">
            <a href="Admin_Dashboard.aspx">Dashboard</a>
            <a href="Admin_Users.aspx">Manage Users</a>
            <a href="Logout.aspx">Logout</a>
        </div>
        <div class="content-area">
            <form id="form1" runat="server">

<h2>Manage Users</h2>

<asp:Panel ID="pnlForm" runat="server" Visible="false" style="border:1px solid #0078d4; padding:10px; background:#f0f7ff; margin-bottom:10px;">
    <asp:HiddenField ID="hfEditID" runat="server" Value="0" />
    Name: <asp:TextBox ID="txtFullName" runat="server" /><br />
    Username: <asp:TextBox ID="txtUsername" runat="server" /><br />
    Password: <asp:TextBox ID="txtPassword" runat="server" /><br />
    Role: <asp:DropDownList ID="ddlRole" runat="server">
        <asp:ListItem>Student</asp:ListItem>
        <asp:ListItem>Teacher</asp:ListItem>
        <asp:ListItem>Admin</asp:ListItem>
    </asp:DropDownList><br /><br />
    <asp:Button ID="btnSaveUser" runat="server" Text="Save" OnClick="btnSaveUser_Click" CssClass="btn btn-blue" />
    <asp:Button ID="btnCancelForm" runat="server" Text="Cancel" OnClick="btnCancelForm_Click" CssClass="btn" />
</asp:Panel>

<div style="margin-bottom:10px;">
    <asp:Button ID="btnShowAdd" runat="server" Text="Add New User" OnClick="btnShowAdd_Click" CssClass="btn btn-blue" />
    Filter: <asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed">
        <asp:ListItem Value="">All</asp:ListItem>
        <asp:ListItem>Admin</asp:ListItem>
        <asp:ListItem>Teacher</asp:ListItem>
        <asp:ListItem>Student</asp:ListItem>
    </asp:DropDownList>
</div>

<asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" Width="100%" DataKeyNames="UserID" OnRowCommand="gvUsers_RowCommand">
    <Columns>
        <asp:BoundField DataField="UserID" HeaderText="ID" />
        <asp:BoundField DataField="FullName" HeaderText="Name" />
        <asp:BoundField DataField="Username" HeaderText="User" />
        <asp:BoundField DataField="Role" HeaderText="Role" />
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# If(CBool(Eval("IsActive")), "Active", "Disabled") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:LinkButton CommandName="EditUser" runat="server" CommandArgument='<%# Eval("UserID") %>' Text="Edit" /> |
                <asp:LinkButton CommandName="ToggleActive" runat="server" CommandArgument='<%# Eval("UserID") %>' Text='<%# If(CBool(Eval("IsActive")), "Disable", "Enable") %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

            </form>
        </div>
    </div>
</body>
</html>
