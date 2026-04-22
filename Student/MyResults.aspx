<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MyResults.aspx.vb" Inherits="Student_MyResults" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">My Results</asp:Content>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<!-- Just-finished banner -->
<asp:Panel ID="pnlJustDone" runat="server" Visible="false" CssClass="alert alert-success mb-4" style="font-size:16px;">
     Quiz submitted! Here is your result below.
</asp:Panel>

<!-- Result Hero (shown for single quiz result) -->
<asp:Panel ID="pnlHero" runat="server" Visible="false">
<div class="result-hero mb-6">
    <div class="result-score-circle">
        <div class="score-pct"><asp:Literal ID="litPct" runat="server" /></div>
        <div class="score-label">Score</div>
    </div>
    <div class="result-marks">
        <asp:Literal ID="litObtained" runat="server" /> / <asp:Literal ID="litTotal" runat="server" /> marks
    </div>
    <div class="result-grade <asp:Literal ID="litGradeCls" runat="server" />">
        Grade: <strong><asp:Literal ID="litGrade" runat="server" /></strong>
        &nbsp;|&nbsp; <asp:Literal ID="litQuizName" runat="server" />
    </div>
</div>

<!-- Charts -->
<div class="charts-grid mb-6">
    <div class="chart-card">
        <h3> Your Score Breakdown</h3>
        <div class="chart-canvas-wrap">
            <canvas id="myPieChart"></canvas>
        </div>
    </div>
    <div class="chart-card">
        <h3> Difficulty Breakdown</h3>
        <div class="chart-canvas-wrap">
            <canvas id="diffChart"></canvas>
        </div>
    </div>
</div>

<!-- Per-question detail -->
<div class="card mb-6">
    <div class="card-header">
        <h3> Question-by-Question Review</h3>
        <span class="text-muted" style="font-size:12px;">Answer locked on Next click — no changes allowed</span>
    </div>
    <div class="table-wrapper">
        <asp:GridView ID="gvDetail" runat="server"
            AutoGenerateColumns="false"
            CssClass="w-100"
            GridLines="None">
            <Columns>
                <asp:BoundField DataField="QNo" HeaderText="#" />
                <asp:TemplateField HeaderText="Question">
                    <ItemTemplate>
                        <div style="max-width:320px;font-size:13px;"><%# Eval("QuestionStatement") %></div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Difficulty">
                    <ItemTemplate>
                        <span class='badge diff-<%# Eval("DifficultyLevel").ToString().ToLower() %>'>
                            <%# Eval("DifficultyLevel") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Your Answer">
                    <ItemTemplate>
                        <% Dim sa As String = If(Eval("StudentAns") Is DBNull.Value, "-", Eval("StudentAns").ToString()) %>
                        <% Dim ca As String = Eval("CorrectAns").ToString() %>
                        <% If sa = "-" Then %>
                            <span class="badge badge-grey">- Skipped</span>
                        <% ElseIf sa = ca Then %>
                            <span class="badge badge-green"> <%=sa%></span>
                        <% Else %>
                            <span class="badge badge-red"> <%=sa%></span>
                        <% End If %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Correct Answer">
                    <ItemTemplate>
                        <span class="badge badge-green"><%# Eval("CorrectAns") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Marks">
                    <ItemTemplate>
                        <strong><%# Eval("Marks") %></strong> / 1
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>
</asp:Panel>

<!-- All Results History -->
<div class="card">
    <div class="card-header">
        <h3> All My Results</h3>
        <a href="Dashboard.aspx" class="btn btn-outline btn-sm">← Dashboard</a>
    </div>
    <div class="table-wrapper">
        <asp:GridView ID="gvAllResults" runat="server"
            AutoGenerateColumns="false"
            CssClass="w-100"
            GridLines="None"
            EmptyDataText="You have not attempted any quizzes yet.">
            <Columns>
                <asp:BoundField DataField="QuizTitle"     HeaderText="Quiz" />
                <asp:BoundField DataField="SubjectName"   HeaderText="Subject" />
                <asp:BoundField DataField="ObtainedMarks" HeaderText="Obtained" DataFormatString="{0:0.#}" />
                <asp:BoundField DataField="TotalMarks"    HeaderText="Total" />
                <asp:TemplateField HeaderText="Score">
                    <ItemTemplate><strong><%# String.Format("{0:0.##}%", Eval("Percentage")) %></strong></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Grade">
                    <ItemTemplate>
                        <%
                            Dim p2 As Double = Convert.ToDouble(Eval("Percentage"))
                            Dim g As String, gc As String
                            If p2 >= 90  Then
                                g = "A+" : gc = "grade-A"
                            ElseIf p2 >= 80 Then
                                g = "A"  : gc = "grade-A"
                            ElseIf p2 >= 70 Then
                                g = "B"  : gc = "grade-B"
                            ElseIf p2 >= 60 Then
                                g = "C"  : gc = "grade-C"
                            Else
                                g = "F" : gc = "grade-F"
                            End If
                        %>
                        <span class="fw-700 <%=gc%>"><%=g%></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="AttemptDate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
            </Columns>
        </asp:GridView>
    </div>
</div>

<asp:Literal ID="litChartScript" runat="server" />

</asp:Content>
