<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ViewResults.aspx.vb" Inherits="Teacher_ViewResults" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Quiz Results</asp:Content>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
    <h1>Quiz Results</h1>
    <p>Select a quiz to view analytics, score charts, and student performance.</p>
</div>

<!-- Select Quiz -->
<div class="form-panel mb-6">
    <div style="display:flex;gap:12px;align-items:flex-end;max-width:500px;">
        <div class="form-group" style="flex:1;">
            <label>Select Quiz</label>
            <asp:DropDownList ID="ddlQuiz" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group">
            <asp:Button ID="btnView" runat="server" Text="View Analytics"
                CssClass="btn btn-primary" OnClick="btnView_Click" />
        </div>
    </div>
</div>

<asp:Panel ID="pnlNoData" runat="server" Visible="false" CssClass="alert alert-info">
    No attempts have been recorded for this quiz yet.
</asp:Panel>

<!-- Analytics View -->
<asp:Panel ID="pnlResults" runat="server" Visible="false">

    <!-- KPI Stats -->
    <div class="stats-grid mb-6">
        <div class="stat-card purple">
            <div>
                <div class="stat-value"><asp:Literal ID="litTotalStudents" runat="server">0</asp:Literal></div>
                <div class="stat-label">Total Attempts</div>
            </div>
        </div>
        <div class="stat-card green">
            <div>
                <div class="stat-value"><asp:Literal ID="litAvgScore" runat="server">0%</asp:Literal></div>
                <div class="stat-label">Average Score</div>
            </div>
        </div>
        <div class="stat-card cyan">
            <div>
                <div class="stat-value"><asp:Literal ID="litHighest" runat="server">0%</asp:Literal></div>
                <div class="stat-label">Highest Score</div>
            </div>
        </div>
        <div class="stat-card red">
            <div>
                <div class="stat-value"><asp:Literal ID="litLowest" runat="server">0%</asp:Literal></div>
                <div class="stat-label">Lowest Score</div>
            </div>
        </div>
    </div>

    <!-- Charts -->
    <div class="grid-2 mb-6">
        <div class="chart-card flex-col">
            <h3>Correct vs Incorrect (Overall)</h3>
            <div class="chart-canvas-wrap">
                <canvas id="correctPieChart"></canvas>
            </div>
        </div>
        <div class="chart-card flex-col">
            <h3>Grade Breakdown</h3>
            <div class="chart-canvas-wrap">
                <canvas id="gradePieChart"></canvas>
            </div>
        </div>
    </div>
    
    <div class="chart-card mb-6">
        <h3>Score Distribution (Bar)</h3>
        <div class="chart-canvas-wrap" style="height:320px;">
            <canvas id="scoreBarChart"></canvas>
        </div>
    </div>

    <!-- Individual Results Table -->
    <div class="card">
        <div class="card-header">
            <h3>Individual Results</h3>
        </div>
        <div class="table-wrapper">
            <asp:GridView ID="gvResults" runat="server"
                AutoGenerateColumns="false"
                CssClass="w-100"
                GridLines="None">
                <Columns>
                    <asp:BoundField DataField="StudentName" HeaderText="Student" />
                    <asp:BoundField DataField="ObtainedMarks" HeaderText="Marks Earned" DataFormatString="{0:0.##}" />
                    <asp:BoundField DataField="TotalMarks" HeaderText="Total Marks" />
                    <asp:TemplateField HeaderText="Score">
                        <ItemTemplate>
                            <strong><%# String.Format("{0:0.##}%", Eval("Percentage")) %></strong>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Grade">
                        <ItemTemplate>
                            <%
                                Dim p As Double = Convert.ToDouble(Eval("Percentage"))
                                Dim g As String, gc As String
                                If p >= 90  Then
                                    g = "A+" : gc = "grade-A"
                                ElseIf p >= 80 Then
                                    g = "A"  : gc = "grade-A"
                                ElseIf p >= 70 Then
                                    g = "B"  : gc = "grade-B"
                                ElseIf p >= 60 Then
                                    g = "C"  : gc = "grade-C"
                                Else
                                    g = "F" : gc = "grade-F"
                                End If
                            %>
                            <span class="fw-700 <%=gc%>"><%=g%></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="AttemptDate" HeaderText="Date Attempted" DataFormatString="{0:dd-MMM-yyyy hh:mm tt}" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- JavaScript to render charts injected from code-behind -->
    <asp:Literal ID="litChartJS" runat="server" />

</asp:Panel>

</asp:Content>
