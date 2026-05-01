<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddQuestion.aspx.vb" Inherits="Teacher_AddQuestion" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Add Question</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<h1>Add New Question</h1>

<asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
    Done! <a href="AddQuestion.aspx">Add another</a> | <a href="ManageQuestions.aspx">Back to list</a>
</asp:Panel>

<asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
    <asp:Literal ID="litError" runat="server" />
</asp:Panel>

<div style="border:1px solid #0078d4; padding:15px; background:#f0f7ff; margin-bottom:15px;">
    <b style="color:#0078d4;">Select Type:</b><br />
    <asp:RadioButton ID="rbSingle" runat="server" GroupName="qType" Checked="true" Text="Single Choice" /><br />
    <asp:RadioButton ID="rbMultiple" runat="server" GroupName="qType" Text="Multiple Choice" /><br />
    <asp:RadioButton ID="rbParagraph" runat="server" GroupName="qType" Text="Short Answer" />
    <asp:HiddenField ID="hfQType" runat="server" Value="Radio" />
</div>

<div style="border:1px solid #ccc; padding:15px; background:white; margin-bottom:15px;">
    <b style="color:#333;">Details:</b><br /><br />
    Subject: <asp:DropDownList ID="ddlSubject" runat="server" /><br /><br />
    Difficulty: 
    <asp:DropDownList ID="ddlDifficulty" runat="server">
        <asp:ListItem>Easy</asp:ListItem>
        <asp:ListItem Selected="True">Medium</asp:ListItem>
        <asp:ListItem>Hard</asp:ListItem>
        <asp:ListItem>Expert</asp:ListItem>
    </asp:DropDownList>
    <br /><br />
    Question Text:<br />
    <asp:TextBox ID="txtStatement" runat="server" TextMode="MultiLine" Rows="3" Width="400px" />
    <br /><br />
    Image (optional): <asp:FileUpload ID="fuImage" runat="server" />
</div>

<div id="secOptions" style="border:1px solid #ccc; padding:15px; background:white; margin-bottom:15px;">
    <b style="color:#333;">Options:</b><br />
    A: <asp:TextBox ID="txtA" runat="server" Width="300px" /><br />
    B: <asp:TextBox ID="txtB" runat="server" Width="300px" /><br />
    C: <asp:TextBox ID="txtC" runat="server" Width="300px" /><br />
    D: <asp:TextBox ID="txtD" runat="server" Width="300px" /><br />
</div>

<div id="secCorrectRadio" style="border:1px solid #28a745; padding:15px; background:#f0fff4; margin-bottom:15px;">
    <b style="color:#28a745;">Correct Answer:</b><br />
    <asp:RadioButton ID="rbA" runat="server" GroupName="Correct" Text="A" />
    <asp:RadioButton ID="rbB" runat="server" GroupName="Correct" Text="B" />
    <asp:RadioButton ID="rbC" runat="server" GroupName="Correct" Text="C" />
    <asp:RadioButton ID="rbD" runat="server" GroupName="Correct" Text="D" />
</div>

<div id="secCorrectCheckbox" style="border:1px solid #28a745; padding:15px; background:#f0fff4; margin-bottom:15px; display:none;">
    <b style="color:#28a745;">Correct Answers:</b><br />
    <asp:CheckBox ID="cbCA" runat="server" Text="A" />
    <asp:CheckBox ID="cbCB" runat="server" Text="B" />
    <asp:CheckBox ID="cbCC" runat="server" Text="C" />
    <asp:CheckBox ID="cbCD" runat="server" Text="D" />
</div>

<div id="secCorrectParagraph" style="border:1px solid #28a745; padding:15px; background:#f0fff4; margin-bottom:15px; display:none;">
    <b style="color:#28a745;">Model Answer:</b><br />
    <asp:TextBox ID="txtModelAnswer" runat="server" TextMode="MultiLine" Rows="2" Width="400px" />
</div>

<br />
<asp:Button ID="btnSave" runat="server" Text="Save Question" OnClick="btnSave_Click" CssClass="btn btn-primary" Width="150px" />
<a href="ManageQuestions.aspx" style="color:red; margin-left:10px;">Cancel</a>

<script>
    function updateVisibility() {
        var type = "";
        if (document.getElementById('<%= rbSingle.ClientID %>').checked) type = "Radio";
        else if (document.getElementById('<%= rbMultiple.ClientID %>').checked) type = "Checkbox";
        else type = "Paragraph";

        document.getElementById('<%= hfQType.ClientID %>').value = type;
        document.getElementById('secOptions').style.display = (type === 'Paragraph') ? 'none' : 'block';
        document.getElementById('secCorrectRadio').style.display = (type === 'Radio') ? 'block' : 'none';
        document.getElementById('secCorrectCheckbox').style.display = (type === 'Checkbox') ? 'block' : 'none';
        document.getElementById('secCorrectParagraph').style.display = (type === 'Paragraph') ? 'block' : 'none';
    }
    document.getElementById('<%= rbSingle.ClientID %>').onclick = updateVisibility;
    document.getElementById('<%= rbMultiple.ClientID %>').onclick = updateVisibility;
    document.getElementById('<%= rbParagraph.ClientID %>').onclick = updateVisibility;
    updateVisibility();
</script>

</asp:Content>
