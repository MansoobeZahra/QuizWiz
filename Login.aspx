<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>
<!DOCTYPE html>
<html>
<head>
    <title>QuizWiz Login</title>
    <link rel="stylesheet" href="Styles/site.css" />
</head>
<body>
    <div style="width:400px; margin: 100px auto; border: 1px solid black; padding: 20px; background-color: white;">
        <h1 style="text-align:center;">QuizWiz</h1>
        <form id="form1" runat="server">
            <asp:Panel ID="pnlError" runat="server" Visible="false" style="color:red; margin-bottom:10px;">
                <asp:Literal ID="litError" runat="server" />
            </asp:Panel>
            
            <div class="form-group">
                <label>Username:</label>
                <asp:TextBox ID="txtUsername" runat="server" style="width:100%;" />
            </div>
            
            <div class="form-group">
                <label>Password:</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" style="width:100%;" />
            </div>
            
            <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" style="width:100%; height:40px;" />
            
            <div style="margin-top:20px; font-size:12px;">
                <b>Demo:</b> admin/Admin@123, t_ali/Teacher@1, s_usman/Student@1
            </div>
        </form>
    </div>
</body>
</html>
