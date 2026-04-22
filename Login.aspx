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
                <div class="logo-icon">🎯</div>
                <h1>QuizWiz</h1>
                <p>Online Quiz Preparation &amp; Conduction</p>
            </div>

            <h2>Sign In</h2>

            <!-- Error message -->
            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                ⚠️ <asp:Literal ID="litError" runat="server" />
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
            <asp:Button ID="btnLogin" runat="server" Text="Sign In →"
                CssClass="btn btn-primary btn-full btn-lg"
                OnClick="btnLogin_Click" />

            <!-- Demo credentials hint -->
            <div style="margin-top:24px;padding:14px;background:rgba(255,255,255,.03);border:1px solid rgba(255,255,255,.07);border-radius:8px;">
                <p style="font-size:12px;color:var(--text-muted);margin-bottom:8px;font-weight:600;letter-spacing:.5px;text-transform:uppercase;">Demo Accounts</p>
                <div style="display:grid;grid-template-columns:1fr 1fr;gap:4px;font-size:12px;color:var(--text-sec);">
                    <span>👑 admin / Admin@123</span>
                    <span>📚 t_ali / Teacher@1</span>
                    <span>📚 t_sara / Teacher@2</span>
                    <span>🎓 s_usman / Student@1</span>
                    <span>🎓 s_ayesha / Student@2</span>
                    <span>🎓 s_bilal / Student@3</span>
                </div>
            </div>
        </div>

    </form>
</div>
</body>
</html>
