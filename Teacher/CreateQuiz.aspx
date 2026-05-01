<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CreateQuiz.aspx.vb" Inherits="Teacher_CreateQuiz" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Create Quiz</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<h1>Create Quiz</h1>

<asp:Panel ID="pnlError" runat="server" Visible="false" style="color:red; border:1px solid red; padding:5px;">
    <asp:Literal ID="litError" runat="server" />
</asp:Panel>

<asp:Panel ID="pnlSuccess" runat="server" Visible="false" style="color:green; border:1px solid green; padding:5px;">
    <asp:Literal ID="litSuccess" runat="server" />
</asp:Panel>

<br />

<asp:MultiView ID="mvWizard" runat="server" ActiveViewIndex="0">

    <!-- STEP 1 -->
    <asp:View ID="vStep1" runat="server">
        <div style="border:1px solid black; padding:15px;">
            <h3>Step 1: Settings</h3>
            Title: <asp:TextBox ID="txtTitle" runat="server" /><br /><br />
            Subject: <asp:DropDownList ID="ddlSubject" runat="server" /><br /><br />
            Total Questions: <asp:TextBox ID="txtTotalQ" runat="server" Text="10" /><br /><br />
            Time (min): <asp:TextBox ID="txtTime" runat="server" Text="30" /><br /><br />
            <asp:CheckBox ID="chkRandomize" runat="server" Text="Randomize Questions" Checked="true" /><br />
            <asp:CheckBox ID="chkShuffle" runat="server" Text="Shuffle Options" Checked="true" /><br />
            <asp:CheckBox ID="chkReview" runat="server" Text="Allow Review" /><br /><br />
            <asp:CheckBox ID="chkNegMarking" runat="server" Text="Negative Marking" AutoPostBack="true" OnCheckedChanged="chkNegMarking_Changed" /><br />
            <asp:Panel ID="divNegMarks" runat="server" Visible="false">
                Neg Marks: <asp:TextBox ID="txtNegMarks" runat="server" Text="0.25" />
            </asp:Panel>
            <br />
            Remarks:<br />
            <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" Rows="2" Width="300px" />
            <br /><br />
            <asp:Button ID="btnStep1Next" runat="server" Text="Next" OnClick="btnStep1Next_Click" />
        </div>
    </asp:View>

    <!-- STEP 2 -->
    <asp:View ID="vStep2" runat="server">
        <asp:HiddenField ID="hfQuizID" runat="server" />
        <div style="display:flex; gap:20px;">
            <div style="flex:1; border:1px solid black; padding:15px;">
                <h3>Step 2: Add Questions</h3>
                <asp:Button ID="btnTabNew" runat="server" Text="New" OnClick="btnTabNew_Click" />
                <asp:Button ID="btnTabBank" runat="server" Text="Bank" OnClick="btnTabBank_Click" />
                <hr />
                <asp:Panel ID="pnlWriteNew" runat="server">
                    Type: <asp:DropDownList ID="ddlQType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlQType_Changed">
                        <asp:ListItem Value="Radio">Single</asp:ListItem>
                        <asp:ListItem Value="Checkbox">Multiple</asp:ListItem>
                        <asp:ListItem Value="Paragraph">Short</asp:ListItem>
                    </asp:DropDownList><br /><br />
                    Text: <asp:TextBox ID="txtQStmt" runat="server" TextMode="MultiLine" Rows="2" Width="100%" /><br /><br />
                    Difficulty: 
                    <asp:DropDownList ID="ddlQDiff" runat="server">
                        <asp:ListItem>Easy</asp:ListItem>
                        <asp:ListItem Selected="True">Medium</asp:ListItem>
                        <asp:ListItem>Hard</asp:ListItem>
                        <asp:ListItem>Expert</asp:ListItem>
                    </asp:DropDownList><br /><br />
                    <asp:Panel ID="pnlOptions" runat="server">
                        A: <asp:CheckBox ID="cbAnsA" runat="server" /><asp:TextBox ID="txtOptA" runat="server" /><br />
                        B: <asp:CheckBox ID="cbAnsB" runat="server" /><asp:TextBox ID="txtOptB" runat="server" /><br />
                        C: <asp:CheckBox ID="cbAnsC" runat="server" /><asp:TextBox ID="txtOptC" runat="server" /><br />
                        D: <asp:CheckBox ID="cbAnsD" runat="server" /><asp:TextBox ID="txtOptD" runat="server" /><br />
                    </asp:Panel>
                    <asp:Panel ID="pnlParagraph" runat="server" Visible="false">
                        Ans: <asp:TextBox ID="txtModelAns" runat="server" />
                    </asp:Panel>
                    <br />
                    <asp:Button ID="btnSaveQ" runat="server" Text="Add" OnClick="btnSaveQ_Click" />
                </asp:Panel>
                <asp:Panel ID="pnlSelectBank" runat="server" Visible="false">
                    <asp:GridView ID="gvBank" runat="server" AutoGenerateColumns="false" DataKeyNames="QuestionID">
                        <Columns>
                            <asp:TemplateField><ItemTemplate><asp:CheckBox ID="chkSelect" runat="server" /></ItemTemplate></asp:TemplateField>
                            <asp:BoundField DataField="QuestionStatement" HeaderText="Q" />
                        </Columns>
                    </asp:GridView>
                    <br />
                    <asp:Button ID="btnAddSelected" runat="server" Text="Add Selected" OnClick="btnAddSelected_Click" />
                </asp:Panel>
            </div>
            <div style="flex:1; border:1px solid black; padding:15px;">
                <h3>Current Quiz (<asp:Literal ID="litCurQCount" runat="server">0</asp:Literal>)</h3>
                <asp:Repeater ID="rptCurrentQ" runat="server" OnItemCommand="rptCurrentQ_Command">
                    <ItemTemplate>
                        <div style="border-bottom:1px solid #ccc; padding:5px;">
                            <%# Eval("QuestionStatement") %> 
                            <asp:LinkButton runat="server" CommandName="Remove" CommandArgument='<%# Eval("QuizQuestionID") %>' Text="[X]" ForeColor="Red" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlNoCurQ" runat="server" Visible="false" style="padding:10px; color:#999;">
                    No questions added yet.
                </asp:Panel>
                <br />
                <asp:Button ID="btnStep2Prev" runat="server" Text="Back" OnClick="btnStep2Prev_Click" />
                <asp:Button ID="btnStep2Next" runat="server" Text="Next" OnClick="btnStep2Next_Click" />
            </div>
        </div>
    </asp:View>

    <!-- STEP 3 -->
    <asp:View ID="vStep3" runat="server">
        <div style="border:1px solid black; padding:15px;">
            <h3>Step 3: Done</h3>
            <p>Quiz <b><asp:Literal ID="litFinalTitle" runat="server" /></b> ready with <asp:Literal ID="litFinalCount" runat="server" /> questions.</p>
            <asp:Button ID="btnStep3Prev" runat="server" Text="Back" OnClick="btnStep3Prev_Click" />
            <asp:Button ID="btnTestQuiz" runat="server" Text="Test" OnClick="btnTestQuiz_Click" />
            <asp:Button ID="btnPublish" runat="server" Text="Publish" OnClick="btnPublish_Click" />
        </div>
    </asp:View>

</asp:MultiView>

</asp:Content>
