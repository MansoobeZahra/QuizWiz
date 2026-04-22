<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CreateQuiz.aspx.vb" Inherits="Teacher_CreateQuiz" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Create Quiz Wizard</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
    <h1> Quiz Creation Wizard</h1>
    <p>Follow the steps to prepare your quiz, add questions, test it, and publish.</p>
</div>

<asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger mb-4">
    <asp:Literal ID="litError" runat="server" />
</asp:Panel>

<asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success mb-4">
    <asp:Literal ID="litSuccess" runat="server" />
</asp:Panel>

<!-- Stepper -->
<div class="wizard-stepper mb-6" style="display:flex; justify-content:space-between; background:#fff; padding:16px; border-radius:8px; border:1px solid #eaeaea;">
    <div style="font-weight:700; color:<%= If(mvWizard.ActiveViewIndex = 0, "#a471f8", "#999") %>">Step 1: Settings</div>
    <div style="font-weight:700; color:<%= If(mvWizard.ActiveViewIndex = 1, "#a471f8", "#999") %>">Step 2: Add Questions</div>
    <div style="font-weight:700; color:<%= If(mvWizard.ActiveViewIndex = 2, "#a471f8", "#999") %>">Step 3: Publish</div>
</div>

<asp:MultiView ID="mvWizard" runat="server" ActiveViewIndex="0">

    <!-- ==================== STEP 1 ==================== -->
    <asp:View ID="vStep1" runat="server">
        <div class="form-panel">
            <h3 class="mb-4">Quiz Settings</h3>
            <div class="grid-2">
                <div class="form-group">
                    <label>Quiz Title <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group">
                    <label>Subject</label>
                    <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control" />
                </div>
            </div>
            
            <div class="grid-2">
                <div class="form-group">
                    <label>Total Questions to Display <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtTotalQ" runat="server" CssClass="form-control" TextMode="Number" Text="10" />
                </div>
                <div class="form-group">
                    <label>Allowed Time (Minutes) <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtTime" runat="server" CssClass="form-control" TextMode="Number" Text="30" />
                </div>
            </div>

            <div class="grid-2">
                <div class="form-group">
                    <label>Negative Marking?</label>
                    <asp:CheckBox ID="chkNegMarking" runat="server" Text=" Enable Negative Marking" AutoPostBack="true" OnCheckedChanged="chkNegMarking_Changed" />
                </div>
                <div class="form-group" id="divNegMarks" runat="server" visible="false">
                    <label>Negative Marks per Wrong Answer</label>
                    <asp:TextBox ID="txtNegMarks" runat="server" CssClass="form-control" Text="0.25" />
                </div>
            </div>

            <div class="grid-2">
                <div class="form-group">
                    <label>Behavior</label>
                    <div><asp:CheckBox ID="chkRandomize" runat="server" Text=" Randomize Questions" Checked="true" /></div>
                    <div><asp:CheckBox ID="chkShuffle" runat="server" Text=" Shuffle Options" Checked="true" /></div>
                    <div><asp:CheckBox ID="chkReview" runat="server" Text=" Allow Students to Review Answers Later" /></div>
                </div>
                <div class="form-group">
                    <label>Remarks / Instructions (Optional)</label>
                    <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                </div>
            </div>

            <div style="text-align:right;">
                <asp:Button ID="btnStep1Next" runat="server" Text="Save Settings and Next" CssClass="btn btn-primary" OnClick="btnStep1Next_Click" />
            </div>
        </div>
    </asp:View>

    <!-- ==================== STEP 2 ==================== -->
    <asp:View ID="vStep2" runat="server">
        <asp:HiddenField ID="hfQuizID" runat="server" />
        
        <div class="grid-2" style="grid-template-columns: 2fr 1fr; gap:24px;">
            <!-- Left Side: Add / Select Questions -->
            <div>
                <div class="card mb-4">
                    <div class="card-header" style="background:#f4f7fa;">
                        <h3>Add Questions to Quiz</h3>
                        <div>
                            <asp:Button ID="btnTabNew" runat="server" Text="Write New" CssClass="btn btn-sm btn-primary" OnClick="btnTabNew_Click" />
                            <asp:Button ID="btnTabBank" runat="server" Text="From Bank" CssClass="btn btn-sm btn-outline" OnClick="btnTabBank_Click" />
                        </div>
                    </div>

                    <!-- TAB: WRITE NEW QUESTION -->
                    <asp:Panel ID="pnlWriteNew" runat="server" style="padding:20px;">
                        <div class="form-group">
                            <label>Question Type</label>
                            <asp:DropDownList ID="ddlQType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlQType_Changed">
                                <asp:ListItem Value="Radio">Single Choice (Radio)</asp:ListItem>
                                <asp:ListItem Value="Checkbox">Multiple Choice (Checkboxes)</asp:ListItem>
                                <asp:ListItem Value="Paragraph">Short Answer (Paragraph)</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label>Question Statement <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtQStmt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                        </div>
                        
                        <div class="form-group">
                            <label>Difficulty</label>
                            <asp:DropDownList ID="ddlQDiff" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Easy">Easy</asp:ListItem>
                                <asp:ListItem Value="Medium">Medium</asp:ListItem>
                                <asp:ListItem Value="Hard">Hard</asp:ListItem>
                                <asp:ListItem Value="Expert">Expert</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <!-- Options for Radio/Checkbox -->
                        <asp:Panel ID="pnlOptions" runat="server">
                            <label>Options & Correct Answer <span class="text-danger">*</span></label>
                            <p class="text-muted" style="font-size:13px;">Check the box(es) next to the correct option(s).</p>
                            
                            <div style="display:flex; gap:10px; align-items:center; margin-bottom:8px;">
                                <asp:CheckBox ID="cbAnsA" runat="server" />
                                <span style="font-weight:700;">A)</span>
                                <asp:TextBox ID="txtOptA" runat="server" CssClass="form-control" />
                            </div>
                            <div style="display:flex; gap:10px; align-items:center; margin-bottom:8px;">
                                <asp:CheckBox ID="cbAnsB" runat="server" />
                                <span style="font-weight:700;">B)</span>
                                <asp:TextBox ID="txtOptB" runat="server" CssClass="form-control" />
                            </div>
                            <div style="display:flex; gap:10px; align-items:center; margin-bottom:8px;">
                                <asp:CheckBox ID="cbAnsC" runat="server" />
                                <span style="font-weight:700;">C)</span>
                                <asp:TextBox ID="txtOptC" runat="server" CssClass="form-control" />
                            </div>
                            <div style="display:flex; gap:10px; align-items:center; margin-bottom:8px;">
                                <asp:CheckBox ID="cbAnsD" runat="server" />
                                <span style="font-weight:700;">D)</span>
                                <asp:TextBox ID="txtOptD" runat="server" CssClass="form-control" />
                            </div>
                        </asp:Panel>

                        <!-- Paragraph Model Answer -->
                        <asp:Panel ID="pnlParagraph" runat="server" Visible="false">
                            <label>Model Answer (Keywords) <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtModelAns" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                        </asp:Panel>

                        <div style="margin-top:16px;">
                            <asp:Button ID="btnSaveQ" runat="server" Text="+ Save & Add to Quiz" CssClass="btn btn-success" OnClick="btnSaveQ_Click" />
                        </div>
                    </asp:Panel>

                    <!-- TAB: SELECT FROM BANK -->
                    <asp:Panel ID="pnlSelectBank" runat="server" Visible="false" style="padding:20px;">
                        <p class="text-muted">Select previously saved questions to add to this quiz.</p>
                        <div style="max-height:400px; overflow-y:auto; border:1px solid #eaeaea; border-radius:4px;">
                            <asp:GridView ID="gvBank" runat="server" AutoGenerateColumns="false" CssClass="w-100" GridLines="None" EmptyDataText="No questions found in your bank." DataKeyNames="QuestionID">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate><asp:CheckBox ID="chkSelect" runat="server" /></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="QuestionStatement" HeaderText="Question" />
                                    <asp:BoundField DataField="DifficultyLevel" HeaderText="Diff" />
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div style="margin-top:16px;">
                            <asp:Button ID="btnAddSelected" runat="server" Text="+ Add Selected to Quiz" CssClass="btn btn-success" OnClick="btnAddSelected_Click" />
                        </div>
                    </asp:Panel>
                </div>
            </div>

            <!-- Right Side: Current Quiz Questions -->
            <div>
                <div class="card" style="position:sticky; top:20px;">
                    <div class="card-header">
                        <h3>Current Quiz (<asp:Literal ID="litCurQCount" runat="server">0</asp:Literal>)</h3>
                    </div>
                    <div style="padding:16px; max-height:400px; overflow-y:auto;">
                        <asp:Repeater ID="rptCurrentQ" runat="server" OnItemCommand="rptCurrentQ_Command">
                            <ItemTemplate>
                                <div style="padding:12px; border-bottom:1px solid #eaeaea; position:relative;">
                                    <div style="font-size:13px; font-weight:600; margin-bottom:4px;"><%# Eval("QuestionStatement") %></div>
                                    <div style="font-size:11px; color:#999;"><%# Eval("QuestionType") %> | <%# Eval("DifficultyLevel") %></div>
                                    <asp:LinkButton runat="server" CommandName="Remove" CommandArgument='<%# Eval("QuizQuestionID") %>' style="position:absolute; right:10px; top:10px; color:#ff416c; text-decoration:none;">Remove</asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Panel ID="pnlNoCurQ" runat="server" Visible="false" style="padding:20px; text-align:center; color:#999;">
                            No questions added yet.
                        </asp:Panel>
                    </div>
                    <div style="padding:16px; border-top:1px solid #eaeaea; display:flex; justify-content:space-between;">
                        <asp:Button ID="btnStep2Prev" runat="server" Text="Back" CssClass="btn btn-outline" OnClick="btnStep2Prev_Click" />
                        <asp:Button ID="btnStep2Next" runat="server" Text="Next" CssClass="btn btn-primary" OnClick="btnStep2Next_Click" />
                    </div>
                </div>
            </div>
        </div>
    </asp:View>

    <!-- ==================== STEP 3 ==================== -->
    <asp:View ID="vStep3" runat="server">
        <div class="form-panel" style="text-align:center; padding:40px 20px;">
            <div style="font-size:24px; margin-bottom:20px; font-weight:bold; color:var(--primary);">Success</div>
            <h2>Quiz is ready!</h2>
            <p style="font-size:16px; color:#666; max-width:500px; margin:0 auto 30px auto;">
                You have successfully configured <strong><asp:Literal ID="litFinalTitle" runat="server" /></strong> 
                and added <strong><asp:Literal ID="litFinalCount" runat="server" /></strong> questions.
            </p>

            <div style="display:flex; gap:16px; justify-content:center; flex-wrap:wrap;">
                <asp:Button ID="btnStep3Prev" runat="server" Text="Back to Questions" CssClass="btn btn-outline" OnClick="btnStep3Prev_Click" />
                
                <asp:Button ID="btnTestQuiz" runat="server" Text="Test Quiz (Student POV)" CssClass="btn btn-purple" OnClick="btnTestQuiz_Click" />
                
                <asp:Button ID="btnPublish" runat="server" Text="Publish to Students" CssClass="btn btn-success" OnClick="btnPublish_Click" />
            </div>
            
            <p style="margin-top:20px; font-size:13px; color:#999;">If you leave this page, the quiz will remain safely saved as a Draft.</p>
        </div>
    </asp:View>

</asp:MultiView>

</asp:Content>
