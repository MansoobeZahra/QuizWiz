<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ManageUsers.aspx.vb" Inherits="Admin_ManageUsers" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Manage Users</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header flex-between">
    <div>
        <h1> Manage Users</h1>
        <p>Create, edit, activate or deactivate user accounts.</p>
    </div>
    <asp:Button ID="btnShowAdd" runat="server" Text=" Add User"
        CssClass="btn btn-primary" OnClick="btnShowAdd_Click" />
</div>

<asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="alert mb-4">
    <asp:Literal ID="litMsg" runat="server" />
</asp:Panel>

<!-- Add/Edit User Form -->
<asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="form-panel mb-6">
    <h3><asp:Literal ID="litFormTitle" runat="server">Add New User</asp:Literal></h3>

    <asp:HiddenField ID="hfEditID" runat="server" Value="0" />

    <div class="form-row mb-4">
        <div class="form-group">
            <label>Full Name</label>
            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Full name" />
        </div>
        <div class="form-group">
            <label>Username</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Login username" />
        </div>
    </div>
    <div class="form-row mb-4">
        <div class="form-group">
            <label>Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control"
                placeholder="Leave blank when editing to keep current" />
        </div>
        <div class="form-group">
            <label>Role</label>
            <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control"
                AutoPostBack="true" OnSelectedIndexChanged="ddlRole_Changed">
                <asp:ListItem Value="Student">Student</asp:ListItem>
                <asp:ListItem Value="Teacher">Teacher</asp:ListItem>
                <asp:ListItem Value="Admin">Admin</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
    <div class="form-row mb-4">
        <asp:Panel ID="pnlSubject" runat="server">
            <div class="form-group">
                <label>Subject</label>
                <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control">
                    <asp:ListItem Value="">— None —</asp:ListItem>
                </asp:DropDownList>
            </div>
        </asp:Panel>
        <div class="form-group">
            <label>Active</label>
            <div class="check-group" style="margin-top:12px;">
                <label class="check-item">
                    <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                    <span>Account is active</span>
                </label>
            </div>
        </div>
    </div>

    <div class="flex gap-2">
        <asp:Button ID="btnSaveUser" runat="server" Text=" Save User"
            CssClass="btn btn-primary" OnClick="btnSaveUser_Click" />
        <asp:Button ID="btnCancelForm" runat="server" Text=" Cancel"
            CssClass="btn btn-outline" OnClick="btnCancelForm_Click" CausesValidation="false" />
    </div>
</asp:Panel>

<!-- Filter -->
<div class="form-panel mb-4" style="padding:14px 20px;">
    <div class="flex-center gap-2">
        <label style="font-weight:600;white-space:nowrap;">Filter by Role:</label>
        <asp:DropDownList ID="ddlFilter" runat="server" CssClass="form-control"
            AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed" style="max-width:200px;">
            <asp:ListItem Value="">All Roles</asp:ListItem>
            <asp:ListItem Value="Admin">Admin</asp:ListItem>
            <asp:ListItem Value="Teacher">Teacher</asp:ListItem>
            <asp:ListItem Value="Student">Student</asp:ListItem>
        </asp:DropDownList>
    </div>
</div>

<!-- Users Table -->
<div class="card">
    <div class="table-wrapper">
        <asp:GridView ID="gvUsers" runat="server"
            AutoGenerateColumns="false"
            CssClass="w-100"
            GridLines="None"
            DataKeyNames="UserID"
            EmptyDataText="No users found."
            OnRowCommand="gvUsers_RowCommand">
            <Columns>
                <asp:BoundField DataField="UserID"   HeaderText="ID" ReadOnly="true" />
                <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                <asp:BoundField DataField="Username" HeaderText="Username" />
                <asp:TemplateField HeaderText="Role">
                    <ItemTemplate>
                        <span class='user-role role-<%# Eval("Role").ToString().ToLower() %>'>
                            <%# Eval("Role") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SubjectName" HeaderText="Subject" NullDisplayText="—" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge <%# If(CBool(Eval("IsActive")), "badge-green", "badge-red") %>'>
                            <%# If(CBool(Eval("IsActive")), "Active", "Disabled") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CreatedAt" HeaderText="Joined" DataFormatString="{0:dd-MMM-yy}" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton CommandName="EditUser" runat="server"
                            CommandArgument='<%# Eval("UserID") %>'
                            CssClass="btn btn-outline btn-sm">️ Edit</asp:LinkButton>
                        &nbsp;
                        <asp:LinkButton CommandName="ToggleActive" runat="server"
                            CommandArgument='<%# Eval("UserID") %>'
                            CssClass='btn btn-sm <%# If(CBool(Eval("IsActive")), "btn-warning", "btn-success") %>'
                            OnClientClick="return confirm('Toggle account status?')">
                            <%# If(CBool(Eval("IsActive")), "Disable", "Enable") %>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

</asp:Content>
