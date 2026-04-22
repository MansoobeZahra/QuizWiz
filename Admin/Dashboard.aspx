<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="Admin_Dashboard" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="ctTitle" ContentPlaceHolderID="PageTitle" runat="server">Admin Dashboard</asp:Content>

<asp:Content ID="ctMain" ContentPlaceHolderID="MainContent" runat="server">

<div class="page-header">
    <h1> Admin Dashboard</h1>
    <p>System overview — manage users, monitor quizzes and results.</p>
</div>

<!-- Stats -->
<div class="stats-grid mb-6">
    <div class="stat-card purple">
        <div>
            <div class="stat-value"><asp:Literal ID="litUsers" runat="server" /></div>
            <div class="stat-label">Total Users</div>
        </div>
    </div>
    <div class="stat-card cyan">
        <div>
            <div class="stat-value"><asp:Literal ID="litTeachers" runat="server" /></div>
            <div class="stat-label">Teachers</div>
        </div>
    </div>
    <div class="stat-card green">
        <div>
            <div class="stat-value"><asp:Literal ID="litStudents" runat="server" /></div>
            <div class="stat-label">Students</div>
        </div>
    </div>
    <div class="stat-card orange">
        <div>
            <div class="stat-value"><asp:Literal ID="litQuizzes" runat="server" /></div>
            <div class="stat-label">Total Quizzes</div>
        </div>
    </div>
    <div class="stat-card red">
        <div>
            <div class="stat-value"><asp:Literal ID="litQuestions" runat="server" /></div>
            <div class="stat-label">Questions</div>
        </div>
    <div class="stat-card purple">
        <div>
            <div class="stat-value"><asp:Literal ID="litAttempts" runat="server" /></div>
            <div class="stat-label">Attempts</div>
        </div>
    </div>
</div>

<div class="grid-2">
    <!-- Recent Quizzes -->
    <div class="card">
        <div class="card-header">
            <h3> Recent Quizzes</h3>
        </div>
        <div class="table-wrapper">
            <asp:GridView ID="gvQuizzes" runat="server"
                AutoGenerateColumns="false"
                CssClass="w-100"
                GridLines="None"
                EmptyDataText="No quizzes yet.">
                <Columns>
                    <asp:BoundField DataField="QuizTitle"   HeaderText="Title" />
                    <asp:BoundField DataField="CreatorName" HeaderText="Teacher" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='badge <%# If(CBool(Eval("IsPublished")),"badge-green","badge-grey") %>'>
                                <%# If(CBool(Eval("IsPublished")),"Published","Draft") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Attempts" HeaderText="Attempts" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Recent Results -->
    <div class="card">
        <div class="card-header">
            <h3> Recent Results</h3>
        </div>
        <div class="table-wrapper">
            <asp:GridView ID="gvResults" runat="server"
                AutoGenerateColumns="false"
                CssClass="w-100"
                GridLines="None"
                EmptyDataText="No results yet.">
                <Columns>
                    <asp:BoundField DataField="StudentName" HeaderText="Student" />
                    <asp:BoundField DataField="QuizTitle"   HeaderText="Quiz" />
                    <asp:TemplateField HeaderText="Score">
                        <ItemTemplate><strong><%# String.Format("{0:0.#}%", Eval("Percentage")) %></strong></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="AttemptDate" HeaderText="Date" DataFormatString="{0:dd-MMM}" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>

</asp:Content>
