<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>
<!DOCTYPE html>
<html>
<head>
    <title>QuizWiz Login</title>
    <link rel="stylesheet" href="Styles/site.css" />
</head>
<body style="background-color:#eee;">
    <div style="width:350px; margin: 100px auto; border: 1px solid #0078d4; padding: 20px; background-color: white;">
        <h1 style="text-align:center; color:#0078d4; margin-top:0;">QuizWiz</h1>
        <form id="form1" runat="server">
            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-err">
                <asp:Literal ID="litError" runat="server" />
            </asp:Panel>
            
            <div class="form-item">
                <label>Username:</label><br />
                <asp:TextBox ID="txtUsername" runat="server" style="width:100%; box-sizing:border-box;" />
            </div>
            
            <div class="form-item">
                <label>Password:</label><br />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" style="width:100%; box-sizing:border-box;" />
            </div>
            
            <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" CssClass="btn btn-blue" style="width:100%;" />
            
            <div style="margin-top:15px; font-size:11px; color:#666;">
                admin / Admin@123<br />
                t_ali / Teacher@1<br />
                s_usman / Student@1
            </div>
        </form>
    </div>
</body>
</html>
