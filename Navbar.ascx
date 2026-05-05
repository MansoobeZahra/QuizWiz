<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Navbar.ascx.vb" Inherits="Navbar" %>
<style>
    .top-nav { background-color: #0078d4; color: white; padding: 20px; border-bottom: 1px solid #005a9e; }
    .top-nav strong { font-size: 20px; }
    .top-nav a { color: white; text-decoration: none; margin-right: 15px; font-size: 14px; }
    .layout-container { display: flex; }
    .sidebar { width: 180px; background: #f4f4f4; min-height: 90vh; padding: 10px; border-right: 1px solid #ccc; }
    .sidebar a { display: block; padding: 10px; color: #333; text-decoration: none; border-bottom: 1px solid #ddd; font-size: 14px; }
    .sidebar a:hover { background: #e0e0e0; }
</style>

<div class="top-nav" style="display:flex; align-items:center;">
    <img src='<%= ResolveUrl("~/Assets/discard.png") %>' alt="QuizWiz" style="height:60px; margin-right:15px;" />
    <span style="margin-left:15px;">User: <%=SessionFullName%> (<%=SessionRole%>)</span>
    <a href='<%= ResolveUrl("~/Logout.aspx") %>' style="margin-left:auto;">Logout</a>
</div>

<div class="sidebar">
    <asp:Panel ID="pnlAdminNav" runat="server" Visible="false">
        <a href='<%= ResolveUrl("~/Admin_Dashboard.aspx") %>'>Dashboard</a>
        <a href='<%= ResolveUrl("~/Admin_Users.aspx") %>'>Users</a>
    </asp:Panel>
    <asp:Panel ID="pnlTeacherNav" runat="server" Visible="false">
        <a href='<%= ResolveUrl("~/Teacher_Dashboard.aspx") %>'>Dashboard</a>
        <a href='<%= ResolveUrl("~/Teacher_AddQ.aspx") %>'>Add Q</a>
        <a href='<%= ResolveUrl("~/Teacher_Bank.aspx") %>'>Bank</a>
        <a href='<%= ResolveUrl("~/Teacher_CreateQuiz.aspx") %>'>Create Quiz</a>
        <a href='<%= ResolveUrl("~/Teacher_Results.aspx") %>'>Results</a>
    </asp:Panel>
    <asp:Panel ID="pnlStudentNav" runat="server" Visible="false">
        <a href='<%= ResolveUrl("~/Student_Dashboard.aspx") %>'>Dashboard</a>
        <a href='<%= ResolveUrl("~/Student_Results.aspx") %>'>Results</a>
    </asp:Panel>
</div>
