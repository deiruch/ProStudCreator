﻿<%@ Page Title="Projekte" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Projectlist.aspx.cs" Inherits="ProStudCreator.projectlist" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:PlaceHolder ID="AdminView" runat="server" Visible="false">
        <div class="well adminSettings non-selectable">
            <h3>Submitted projects:</h3>
            <div class="well" style="background-color: #ffffff">
                <asp:GridView ID="CheckProjects" ItemType="ProStudCreator.ProjectSingleElement" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="1200px" AutoGenerateColumns="False" OnRowCommand="CheckProjectsEvent">
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
                        <asp:BoundField DataField="advisorName" HeaderText="BetreuerIn" SortExpression="Advisor" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="advisorEmail" HeaderText="E-Mail" SortExpression="Advisor" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="projectName" HeaderText="Projektname" SortExpression="Advisor" ItemStyle-Width="250px" />
                        <asp:CheckBoxField HeaderText="P5" DataField="p5" SortExpression="Advisor" />
                        <asp:CheckBoxField HeaderText="P6" DataField="p6" SortExpression="Advisor" />
                        <asp:ImageField ItemStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType1" HeaderText="Projekttyp" ItemStyle-Width="20px" />
                        <asp:ImageField ItemStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType2" ItemStyle-Width="20px" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CommandName="showProject" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-eye-open"></asp:LinkButton>
                                <asp:LinkButton runat="server" CommandName="editProject" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-pencil"></asp:LinkButton>
                                <asp:LinkButton runat="server" CommandName="deleteProject" OnClientClick="return confirm('Wollen Sie wirklich dieses Projekt löschen?');" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-remove"></asp:LinkButton>
                                <asp:LinkButton runat="server" CommandName="SinglePDF" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-save"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EditRowStyle BackColor="#2461BF" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EFF3FB" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                    <SortedDescendingHeaderStyle BackColor="#4870BE" />
                </asp:GridView>
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="well usernSettings non-selectable">
        <asp:Button runat="server" ID="newProject" CssClass="btn btn-default buttonFont" Text="New project" OnClick="newProject_Click" />
        <asp:PlaceHolder ID="AdminViewPDF" runat="server" Visible="false">
            <asp:Button runat="server" ID="AllProjectsAsPDF" CssClass="btn btn-default buttonFont" Text="Create PDF" OnClick="AllProjectsAsPDF_Click" />

        </asp:PlaceHolder>
        <div class="radioButtonSettings non-selectable">
            <asp:RadioButtonList ID="ProjectsFilterAllProjects" RepeatDirection="Horizontal" runat="server" AutoPostBack="true">
                <asp:ListItem Value="AllFutureProjects">Projects next semester</asp:ListItem>
                <asp:ListItem Value="AllPastProjects">Projects last semester</asp:ListItem>
                <asp:ListItem Value="MyProjects">My projects (in progress)</asp:ListItem>
                <asp:ListItem Value="NotAvailable">Submitted projects</asp:ListItem>
                <asp:ListItem Value="Available">Published Projects</asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="well" style="background-color: #ffffff; margin-top: 10px;">
            <asp:GridView ID="AllProjects" ItemType="ProStudCreator.ProjectSingleElement" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="1200px" AutoGenerateColumns="False" OnRowCommand="AllProjectsEvent" OnRowDataBound="AllProjects_RowDataBound">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="ID" />
                    <asp:BoundField DataField="advisorName" HeaderText="BetreuerIn" SortExpression="Advisor" ItemStyle-Width="200px" />
                    <asp:BoundField DataField="advisorEmail" HeaderText="E-Mail" SortExpression="Advisor" ItemStyle-Width="200px" />
                    <asp:BoundField DataField="projectName" HeaderText="Projektname" SortExpression="Advisor" ItemStyle-Width="250px" />
                    <asp:CheckBoxField HeaderText="P5" DataField="p5" SortExpression="Advisor" />
                    <asp:CheckBoxField HeaderText="P6" DataField="p6" SortExpression="Advisor" />
                    <asp:ImageField ItemStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType1" HeaderText="Projekttyp" ItemStyle-Width="20px" />
                    <asp:ImageField ItemStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType2" ItemStyle-Width="20px" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="showProjectButton" CommandName="showProject" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-eye-open pull-right"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="editProjectButton" CommandName="editProject" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-pencil"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="deleteProjectButton" CommandName="deleteProject" OnClientClick="return confirm('Wollen Sie wirklich dieses Projekt löschen?');" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-remove"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="SinglePDFButton" CommandName="SinglePDF" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-save"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EditRowStyle BackColor="#2461BF" />
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#EFF3FB" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                <SortedDescendingHeaderStyle BackColor="#4870BE" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
