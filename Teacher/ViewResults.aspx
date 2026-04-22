<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ViewResults.aspx.vb" Inherits="Teacher_ViewResults" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">View Results</asp:Content>

<asp:Content ID="ctHead" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
    <h1>📊 Quiz Results</h1>
    <p>View detailed results and score distribution for each quiz.</p>
</div>

<!-- Quiz Selector -->
<div class="form-panel mb-6" style="padding:16px 24px;">
    <div class="flex-center gap-2">
        <label style="font-weight:600;white-space:nowrap;">Select Quiz:</label>
        <asp:DropDownList ID="ddlQuiz" runat="server" CssClass="form-control"
            AutoPostBack="true" OnSelectedIndexChanged="ddlQuiz_Changed" />
    </div>
</div>

<asp:Panel ID="pnlResults" runat="server" Visible="false">

    <!-- Summary Stats -->
    <div class="stats-grid mb-6">
        <div class="stat-card">
            <div class="stat-icon purple">🎓</div>
            <div>
                <div class="stat-value"><asp:Literal ID="litTotal" runat="server" /></div>
                <div class="stat-label">Attempts</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon green">📈</div>
            <div>
                <div class="stat-value"><asp:Literal ID="litAvg" runat="server" />%</div>
                <div class="stat-label">Average Score</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon cyan">🏆</div>
            <div>
                <div class="stat-value"><asp:Literal ID="litMax" runat="server" />%</div>
                <div class="stat-label">Highest Score</div>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon red">📉</div>
            <div>
                <div class="stat-value"><asp:Literal ID="litMin" runat="server" />%</div>
                <div class="stat-label">Lowest Score</div>
            </div>
        </div>
    </div>

    <!-- Charts -->
    <div class="charts-grid mb-6">
        <div class="chart-card">
            <h3>📊 Score Distribution (Bar)</h3>
            <div class="chart-canvas-wrap">
                <canvas id="barChart"></canvas>
            </div>
        </div>
        <div class="chart-card">
            <h3>🥧 Grade Breakdown (Pie)</h3>
            <div class="chart-canvas-wrap">
                <canvas id="pieChart"></canvas>
            </div>
        </div>
    </div>

    <!-- Results Table -->
    <div class="card">
        <div class="card-header">
            <h3>🗒️ Individual Results</h3>
            <span class="text-muted" style="font-size:13px;">Sorted by score descending</span>
        </div>
        <div class="table-wrapper">
            <asp:GridView ID="gvResults" runat="server"
                AutoGenerateColumns="false"
                CssClass="w-100"
                GridLines="None"
                EmptyDataText="No students have attempted this quiz yet.">
                <Columns>
                    <asp:BoundField DataField="FullName"      HeaderText="Student" />
                    <asp:BoundField DataField="ObtainedMarks" HeaderText="Obtained" DataFormatString="{0:0.#}" />
                    <asp:BoundField DataField="TotalMarks"    HeaderText="Total" />
                    <asp:TemplateField HeaderText="Percentage">
                        <ItemTemplate>
                            <strong><%# String.Format("{0:0.##}%", Eval("Percentage")) %></strong>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Grade">
                        <ItemTemplate>
                            <%
                                Dim pct As Double = Convert.ToDouble(Eval("Percentage"))
                                Dim grade As String, cls As String
                                If pct >= 90 Then grade = "A+" : cls = "grade-A"
                                ElseIf pct >= 80 Then grade = "A" : cls = "grade-A"
                                ElseIf pct >= 70 Then grade = "B" : cls = "grade-B"
                                ElseIf pct >= 60 Then grade = "C" : cls = "grade-C"
                                Else grade = "F" : cls = "grade-F"
                                End If
                            %>
                            <span class="fw-700 <%=cls%>"><%=grade%></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="AttemptDate" HeaderText="Attempted On" DataFormatString="{0:dd-MMM-yyyy HH:mm}" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Panel>

<asp:Panel ID="pnlSelectMsg" runat="server" CssClass="alert alert-info">
    ℹ️ Select a quiz above to view results.
</asp:Panel>

<!-- Chart Init -->
<asp:Literal ID="litChartScript" runat="server" />

</asp:Content>
