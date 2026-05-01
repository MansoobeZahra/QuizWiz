<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>QuizWiz – Login</title>
    <meta name="description" content="Login to QuizWiz online quiz system." />
    <link rel="stylesheet" href="Styles/site.css" />
</head>
<body>
<div class="login-wrapper">
    <form id="form1" runat="server">

        <div class="login-card">
            <!-- Logo -->
            <div class="login-logo">
                <img src="Assets/logo vertical.png" alt="QuizWiz Logo" style="max-height:240px; width:auto; margin-bottom:20px; height: 138px;" />
                <p style="color:var(--text-sec); font-size:14px;">Online Quiz Preparation &amp; Conduction</p>
            </div>

            <h2>Sign In</h2>

            <!-- Error message -->
            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                <asp:Literal ID="litError" runat="server" />
            </asp:Panel>

            <!-- Username -->
            <div class="form-group">
                <label for="txtUsername">Username</label>
                <asp:TextBox ID="txtUsername" runat="server"
                    CssClass="form-control"
                    placeholder="Enter your username"
                    autocomplete="username" />
            </div>

            <!-- Password -->
            <div class="form-group">
                <label for="txtPassword">Password</label>
                <asp:TextBox ID="txtPassword" runat="server"
                    CssClass="form-control"
                    TextMode="Password"
                    placeholder="Enter your password"
                    autocomplete="current-password" />
            </div>

            <!-- Login Button -->
            <asp:Button ID="btnLogin" runat="server" Text="Sign In"
                CssClass="btn btn-primary btn-full btn-lg"
                OnClick="btnLogin_Click" />

            <!-- Demo credentials hint -->
            <div style="margin-top:20px; padding:10px; border:1px solid var(--border); background:#f9f9f9;">
                <p style="font-size:11px; color:#666; margin-bottom:5px; font-weight:600;">Demo Accounts</p>
                <div style="font-size:11px; color:#444; line-height:1.5;">
                    <div>Admin: admin / Admin@123</div>
                    <div>Teacher: t_ali / Teacher@1</div>
                    <div>Student: s_usman / Student@1</div>
                </div>
            </div>
        </div>

    </form>
</div>
</body>
</html>
